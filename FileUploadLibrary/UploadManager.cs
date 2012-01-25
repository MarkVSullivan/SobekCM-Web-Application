#region License
/*

File Upload HTTP module for ASP.Net (v 2.0)
Copyright (C) 2007-2008 Darren Johnstone (http://darrenjohnstone.net)

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA

*/
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;

namespace darrenjohnstone.net.FileUpload
{
    /// <summary>
    /// Event arguments for the ProcessorInit event.
    /// </summary>
    public class FileProcessorInitEventArgs : EventArgs
    {
        #region Declarations

        IFileProcessor _processor;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the file processor.
        /// </summary>
        public IFileProcessor Processor
        {
            get { return _processor; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="processor">File processor instance.</param>
        public FileProcessorInitEventArgs(IFileProcessor processor)
        {
            _processor = processor;
        }

        #endregion
    }

    /// <summary>
    /// Delegate for the ProcessorInit event.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="args">Event args.</param>
    public delegate void FileProcessorInitEventHandler(object sender, FileProcessorInitEventArgs args);

    /// <summary>
    /// Manages uploads and acts as a factory class for file processors.
    /// </summary>
    public sealed class UploadManager
    {
        #region Declarations

        static UploadManager _instance = null;
        static readonly object _padlock = new object();
        const int MIN_BUFFER_SIZE = 1024;
        const int DEF_BUFFER_SIZE = 1024 * 28;
        Type _processorType;
        int _bufferSize;
        bool _moduleInstalled;

        static byte[] DefaultValidationKey = null;
        static byte[] DefaultEncryptionKey = null;

        /// <summary> Constant status key used in the generated HTML </summary>
        public const string STATUS_KEY = "DJUploadStatus";

        #endregion

        #region Constructor


        /// <summary>
        /// Initializes a new instance of the <see cref="UploadManager"/> class.
        /// </summary>
        UploadManager()
        {
            // Set up the default processor.
            _processorType = typeof(DummyProcessor);
            _bufferSize = DEF_BUFFER_SIZE;
        }

        /// <summary>
        /// Initializes the <see cref="UploadManager"/> class.
        /// </summary>
        static UploadManager()
		{
			using (KeyedHashAlgorithm macAlg = KeyedHashAlgorithm.Create())
			{
				DefaultValidationKey = new byte[(macAlg.HashSize+7)/8];
				RandomNumberGenerator rng = RandomNumberGenerator.Create();
				rng.GetBytes(DefaultValidationKey);
			}
			
			using (SymmetricAlgorithm cipher = SymmetricAlgorithm.Create())
			{
				cipher.GenerateKey();
				DefaultEncryptionKey = cipher.Key;
			}
		}

        #endregion

        #region Events

        /// <summary>
        /// Fired when a processor is initialised but before it is used.
        /// Set processor properties here.
        /// </summary>
        public event FileProcessorInitEventHandler ProcessorInit;

        /// <summary>
        /// Fires the ProcessorInit event.
        /// </summary>
        /// <param name="processor">File processor.</param>
        public void OnProcessorInit(IFileProcessor processor)
        {
            if (ProcessorInit != null)
                ProcessorInit(this, new FileProcessorInitEventArgs(processor));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Sets the upload status.
        /// </summary>
        /// <param name="status">Status to set.</param>
        /// <param name="key">Upload key.</param>
        internal void SetStatus(UploadStatus status, string key)
        {
            HttpContext.Current.Application[STATUS_KEY + key] = status;
        }

        /// <summary>
        /// Gets/sets a boolean value indicating if the HTTP module is installed.
        /// </summary>
        public bool ModuleInstalled
        {
            get { return _moduleInstalled; }
            internal set { _moduleInstalled = value; }
        }

        /// <summary>
        /// Gets the current upload status.
        /// </summary>
        public UploadStatus Status
        {
            get
            {
                string key;

                key = HttpContext.Current.Request.QueryString[STATUS_KEY];
                if (key == null)
                {
                    key = (string)HttpContext.Current.Items[STATUS_KEY];
                    if (key == null)
                        return null;
                }
                
                return HttpContext.Current.Application[STATUS_KEY + key] as UploadStatus;
            }
            internal set
            {
                string key;

                key = HttpContext.Current.Request.QueryString[STATUS_KEY];
                if (key != null)
                {
                    SetStatus(value, key);
                }
            }
        }

        /// <summary>
        /// Gets/sets the buffer size for reading from the request stream.
        /// </summary>
        public int BufferSize
        {
            get { return _bufferSize; }
            set 
            {
                if (_bufferSize <= MIN_BUFFER_SIZE)
                    throw new ArgumentException("Minimum buffer size violation");
                _bufferSize = value; 
            }
        }

        /// <summary>
        /// Gets the singleton instance in a thread safe manner.
        /// </summary>
        public static UploadManager Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new UploadManager();
                    }
                    return _instance;
                }
            }
        }

        /// <summary>
        /// Gets/sets the processor type (must implement IFileProcessor).
        /// </summary>
        public Type ProcessorType
        {
            get { return _processorType; }
            set 
            {
                if (value == null || value.GetInterface("IFileProcessor", false) == null)
                    throw new ArgumentException("File processor must implement IFileProcessor");
                _processorType = value; 
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Factory method creates a new instance of IFileProcessor.
        /// </summary>
        /// <returns>The created file processor.</returns>
        public IFileProcessor GetProcessor()
        {
            IFileProcessor processor;

            processor = (IFileProcessor)Activator.CreateInstance(_processorType);
            OnProcessorInit(processor);
            return processor;
        }

        /// <summary>
        /// Serializes a processor to a string. The string is encrypted and validated.
        /// Thanks to Dean Brettle for help with this http://www.brettle.com.
        /// </summary>
        /// <param name="processor">Processor to serialize.</param>
        /// <returns>The serialized processor.</returns>
        internal string SerializeProcessor(IFileProcessor processor)
        {
            SettingsStorageObject so = new SettingsStorageObject();
            MemoryStream ms = new MemoryStream();
            MemoryStream outStream = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, processor);

            // Encrypt the serialized object
            MemoryStream cipherTextStream = new MemoryStream();
            SymmetricAlgorithm cipher = SymmetricAlgorithm.Create();
            cipher.Mode = CipherMode.CBC;
            cipher.Padding = PaddingMode.PKCS7;
            cipher.Key = DefaultEncryptionKey;
            CryptoStream cryptoStream = new CryptoStream(cipherTextStream, cipher.CreateEncryptor(), CryptoStreamMode.Write);
            byte[] cryptoBytes = ms.ToArray();
            cryptoStream.Write(cryptoBytes, 0, cryptoBytes.Length);
            cryptoStream.Close();

            so.CipherText = cipherTextStream.ToArray();
            so.CipherIV = cipher.IV;

            // Generate a hash for the encrypted data
            KeyedHashAlgorithm kh = KeyedHashAlgorithm.Create();
            kh.Key = DefaultValidationKey;
            so.Hash = kh.ComputeHash(so.CipherText);

            bf.Serialize(outStream, so);
            return Convert.ToBase64String(outStream.ToArray());
        }

        /// <summary>
        /// Deserializes a processor from an encrypted string. The string is encrypted and validated.
        /// Thanks to Dean Brettle for help with this http://www.brettle.com.
        /// </summary>
        /// <param name="input">The encrypted and signed input string.</param>
        /// <returns>The deserialized processor.</returns>
        internal IFileProcessor DeserializeProcessor(string input)
        {
            MemoryStream ms;
            BinaryFormatter bf = new BinaryFormatter();
            byte[] bytes = Convert.FromBase64String(input);
            ms = new MemoryStream(bytes);

            SettingsStorageObject so = (SettingsStorageObject)bf.Deserialize(ms);

            // Compute and check the hash
            KeyedHashAlgorithm macAlgorithm = KeyedHashAlgorithm.Create();
            MemoryStream hashStream = new MemoryStream(so.CipherText);
            macAlgorithm.Key = DefaultValidationKey;
            byte[] expectedHash = macAlgorithm.ComputeHash(hashStream);

            bool valid = true;

            if (expectedHash.Length != so.Hash.Length)
            {
                valid = false;
            }
            else
            {
                for (int i = 0; i < expectedHash.Length; i++)
                {
                    if (expectedHash[i] != so.Hash[i])
                    {
                        valid = false;
                        break;
                    }
                }
            }

            if (!valid) throw new System.Security.SecurityException("Processor settings invalid");

            // Decrypt the settings
            MemoryStream cipherTextStream = new MemoryStream(so.CipherText);
            SymmetricAlgorithm cipher = SymmetricAlgorithm.Create();
            cipher.Mode = CipherMode.CBC;
            cipher.Padding = PaddingMode.PKCS7;
            cipher.Key = DefaultEncryptionKey;
            cipher.IV = so.CipherIV;
            CryptoStream cryptoStream = new CryptoStream(cipherTextStream, cipher.CreateDecryptor(), CryptoStreamMode.Read);

            return (IFileProcessor)bf.Deserialize(cryptoStream);
        }

        #endregion

    }
}
