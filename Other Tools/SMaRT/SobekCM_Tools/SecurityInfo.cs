#region Using directives

using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading;
using Microsoft.Win32;

#endregion

namespace SobekCM.Tools
{
	/// <summary> Object used to determine and ensure security.  It allows
	/// for reading from the registry, checking local users and computer information, 
	/// and writing and reading to encrypted files. <br /><br />
	/// </summary>
	/// <remarks> This class allows for the following actions: <ul>
	/// <li type="circle" /> Encrypting and Decrypting strings. [ <see cref="SecurityInfo.EncryptString"/> and <see cref="SecurityInfo.DecryptString"/> ] 
	/// <li type="circle" /> Reading and Writing to encrypted files. [ <see cref="SecurityInfo.ReadFromEncryptedFile"/> and <see cref="SecurityInfo.WriteToEncryptedFile"/> ] 
	/// <li type="circle" /> Getting the current username. [ <see cref="SecurityInfo.UserName"/> ]
	/// <li type="circle" /> Getting username and security level information from a security database.
	/// </ul> <br /> <br />
	/// Object created by Mark V Sullivan (2003) for University of Florida's Digital Library Center. </remarks>
	public class SecurityInfo
	{
	    ///// <summary> Gets the MAC address of the network adapter for the current computer </summary>
        ///// <remarks> The MAC address is returned as a string in the form 00:##:##:##:##:##. </remarks>
        //public string MAC_Address
        //{
        //    get
        //    {
        //        ManagementClass oNetworkAdapter = new ManagementClass ("Win32_NetworkAdapter");
        //        ManagementObjectCollection moc = oNetworkAdapter.GetInstances();
        //        foreach(ManagementObject mo in moc)
        //        {
        //            try
        //            {
        //                if ( ( mo["MACAddress"] != null ) && ( mo["MACAddress"].ToString().Substring(0,2) == "00" ) )
        //                {
        //                    return (string)mo["MACAddress"];
        //                }
        //            }
        //            catch
        //            {
        //                return "";
        //            }
        //        }
        //        return "";
        //    }
        //}

        ///// <summary> Returns the hard drive serial number for the hard drive indicated by drive letter </summary>
        ///// <param name="driveLetter"> Drive letter for the drive in question </param>
        ///// <returns> Hard drive serial number </returns>
        //public string HardDriveSerial( char driveLetter )
        //{
        //    ManagementObject disk = new ManagementObject("Win32_Logicaldisk=" + "\"" + driveLetter + ":\"");
        //    string SerialNumber = disk.Properties["Volumeserialnumber"].Value.ToString();
        //    return SerialNumber;
        //}

		/// <summary> Gets the complete current users name as a string. </summary>
		/// <remarks> This name is returned in the form 'DOMAIN\username'. </remarks>
		public string UserName
		{
			get
			{
				AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
				WindowsPrincipal principal = (WindowsPrincipal)Thread.CurrentPrincipal;
				WindowsIdentity identity = (WindowsIdentity)principal.Identity;
				return identity.Name;
			}
		}

        /// <summary> Gets the complete current users name as a string. </summary>
        /// <remarks> This name is returned in the form 'DOMAIN\username'. </remarks>
        public static string Current_UserName
        {
            get
            {
                AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
                WindowsPrincipal principal = (WindowsPrincipal)Thread.CurrentPrincipal;
                WindowsIdentity identity = (WindowsIdentity)principal.Identity;
                return identity.Name;
            }
        }


		/// <summary> Returns a string value from the registry under HKEY_LOCAL_MACHINE. </summary>
		/// <param name="keyLocation"> Location of the key (i.e. "Control Panel\Desktop") </param>
		/// <param name="valueName"> Name of the value to retrieve </param>
		/// <returns> String value from the registry, or "-1" if an error occurs </returns>
		public string LocalMachineKey( string keyLocation, string valueName )
		{
			try
			{
				RegistryKey fetcher = Registry.LocalMachine;
				fetcher = fetcher.OpenSubKey( keyLocation );
			    if (fetcher != null)
			    {
			        string fetchedValue = (string) fetcher.GetValue(valueName);
			        fetcher.Close();
			        return fetchedValue ?? "-1";
			    }
			}
			catch
			{
				return "-1";
			}
            return "-1";
		}

        /// <summary> Returns a string value from the registry under HKEY_CURRENT_USER. </summary>
        /// <param name="keyLocation"> Location of the key (i.e. "Control Panel\Desktop") </param>
        /// <param name="valueName"> Name of the value to retrieve </param>
        /// <returns> String value from the registry, or "-1" if an error occurs </returns>
		public string CurrentUserKey( string keyLocation, string valueName )
		{
			try
			{

				RegistryKey fetcher = Registry.CurrentUser;
				fetcher = fetcher.OpenSubKey( keyLocation );
			    if (fetcher != null)
			    {
			        string fetchedValue = (string) fetcher.GetValue(valueName);
			        fetcher.Close();
			        return fetchedValue ?? "-1";
			    }
			}
			catch
			{
				return "-1";
			}
            return "-1";
		}

		/// <summary> Reads text from a file encrypted in DES encryption. (128 bit symmetric encryption) </summary>
		/// <param name="filename"> Path and name of file to be read from</param>
		/// <param name="key"> 8 character (64bit) key for decryption</param>
		/// <param name="IV"> 8 character (64bit) initialization vector for decryption</param>
		/// <param name="position"> Character position to start reading from</param>
		/// <param name="length"> Number of characters to read from the file</param>
		/// <returns> Character array of data read and decrypted from file or a NULL if there was an error</returns>
		public char[] ReadFromEncryptedFile ( string filename, string key, string IV, int position, int length )
		{
			char[] temp = new char[length];

			try
			{
				// Open the necessary file streams
				FileStream projectDataFile = new FileStream(filename, FileMode.Open, FileAccess.Read);
			    DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider
			                                               {Key = Encoding.ASCII.GetBytes(key), IV = Encoding.ASCII.GetBytes(IV)};
			    CryptoStream cryptoStreamDecrypt = new CryptoStream(projectDataFile, desProvider.CreateDecryptor(), CryptoStreamMode.Read );
				StreamReader streamInput = new StreamReader(cryptoStreamDecrypt);

				// Jump to position provided and read the data requested
				projectDataFile.Position = position;
				streamInput.Read(temp, 0, length );

				// Close the file stream
				projectDataFile.Close();
				return temp;
			}
			catch
			{
				// if there was any error, return NULL
				return null;
			}
		}

		/// <summary> Writes text to a file encrypted in DES encryption. (128 bit symmetric encryption) </summary>
		/// <param name="textToWrite"> Text which will be written to the file</param>
		/// <param name="filename"> Path and name of file to be written to</param>
		/// <param name="key"> 8 character (64bit) key for encryption</param>
		/// <param name="IV"> 8 character (64bit) initialization vector for encryption</param>
		/// <param name="position"> Character position in file to write the text</param>
		/// <returns> TRUE if written successfully, otherwise FALSE </returns>
		public bool WriteToEncryptedFile ( string textToWrite, string filename, string key, string IV, int position )
		{
			try
			{
				// Open file streams necessary
				FileStream projectDataFile = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write);
			    DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider
			                                               {Key = Encoding.ASCII.GetBytes(key), IV = Encoding.ASCII.GetBytes(IV)};
			    CryptoStream cryptoStreamEncrypt = new CryptoStream(projectDataFile, desProvider.CreateEncryptor(), CryptoStreamMode.Write );

				// Get to the correct position in the file to write
				projectDataFile.Position = position;

				// Break string into a byte array and write to file
				byte[] temp = new byte[textToWrite.Length];
				for ( int i = 0 ; i < textToWrite.Length ; ++i )
					temp[i] = Convert.ToByte(Convert.ToChar(textToWrite.Substring(i, 1)));
				cryptoStreamEncrypt.Write(temp, 0, textToWrite.Length);

				// Close the file stream
				cryptoStreamEncrypt.Close( );
				projectDataFile.Close( );
				return true;
			}
			catch
			{
				// if there was any error, return false
				return false;
			}
		}

        /// <summary> Encrypt a string, given the string.  </summary>
        /// <param name="Source"> String to encrypt </param>
        /// <returns> The encrypted string </returns>
        public static string SHA1_EncryptString(string Source )
        {
            byte[] bytIn = Encoding.ASCII.GetBytes(Source);

            // set the private key
            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] bytOut = sha.ComputeHash(bytIn);

            // convert into Base64 so that the result can be used in xml
            return Convert.ToBase64String(bytOut, 0, bytOut.Length);
        }

        /// <summary> Encrypt a string, given the string.  </summary>
        /// <param name="Source"> String to encrypt </param>
        /// <param name="Key"> Key for the encryption </param>
        /// <param name="IV"> Initialization Vector for the encryption </param>
        /// <returns> The encrypted string </returns>
        public static string DES_EncryptString(string Source, string Key, string IV )
        {
            byte[] bytIn = Encoding.ASCII.GetBytes(Source);
            // create a MemoryStream so that the process can be done without I/O files
            MemoryStream ms = new MemoryStream();

            // set the private key
            DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider
                                                       {
                                                           Key = Encoding.ASCII.GetBytes(Key),
                                                           IV = Encoding.ASCII.GetBytes(IV)
                                                       };

            // create an Encryptor from the Provider Service instance
            ICryptoTransform encrypto = desProvider.CreateEncryptor();

            // create Crypto Stream that transforms a stream using the encryption
            CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);

            // write out encrypted content into MemoryStream
            cs.Write(bytIn, 0, bytIn.Length);
            cs.Close();

            // Write out from the Memory stream to an array of bytes
            byte[] bytOut = ms.ToArray();
            ms.Close();

            // convert into Base64 so that the result can be used in xml
            return Convert.ToBase64String(bytOut, 0, bytOut.Length);
        }

		/// <summary> Encrypt a string, given the string, the key, and the IV values.  </summary>
		/// <param name="Source"> String to encrypt </param>
		/// <param name="Key"> Key for the encryption </param>
		/// <param name="IV"> Initialization Vector for the encryption </param>
		/// <returns> The encrypted string </returns>
		public string EncryptString(string Source, string Key, string IV )
		{
			byte[] bytIn = Encoding.ASCII.GetBytes(Source);
			// create a MemoryStream so that the process can be done without I/O files
			MemoryStream ms = new MemoryStream();

			// set the private key
		    DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider
		                                               {Key = Encoding.ASCII.GetBytes(Key), IV = Encoding.ASCII.GetBytes(IV)};

		    // create an Encryptor from the Provider Service instance
			ICryptoTransform encrypto = desProvider.CreateEncryptor();

			// create Crypto Stream that transforms a stream using the encryption
			CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);

			// write out encrypted content into MemoryStream
			cs.Write(bytIn, 0, bytIn.Length);
			cs.Close();

			// Write out from the Memory stream to an array of bytes
			byte[] bytOut = ms.ToArray();
			ms.Close();
                    
			// convert into Base64 so that the result can be used in xml
			return Convert.ToBase64String(bytOut, 0, bytOut.Length);
		}

		/// <summary> Decrypt a string, given the string, the key, and the IV values.  </summary>
		/// <param name="Source"> String to decrypt </param>
		/// <param name="Key"> Key for the encryption </param>
		/// <param name="IV"> Initialization Vector for the encryption </param>
		/// <returns> The decrypted string </returns>
		public string DecryptString(string Source, string Key, string IV )
		{
			// convert from Base64 to binary
			byte[] bytIn = Convert.FromBase64String(Source);
			// create a MemoryStream with the input
			MemoryStream ms = new MemoryStream(bytIn, 0, bytIn.Length);

			// set the private key
		    DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider
		                                               {Key = Encoding.ASCII.GetBytes(Key), IV = Encoding.ASCII.GetBytes(IV)};

		    // create a Decryptor from the Provider Service instance
			ICryptoTransform encrypto = desProvider.CreateDecryptor();
 
			// create Crypto Stream that transforms a stream using the decryption
			CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read);

			// read out the result from the Crypto Stream
			StreamReader sr = new StreamReader( cs );
			return sr.ReadToEnd();
		}
	}
}
