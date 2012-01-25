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

namespace darrenjohnstone.net.FileUpload
{
    /// <summary>
    /// Delegate for file events.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="fileName">File name.</param>
    /// <param name="identifier">
    /// An optional identifier passed from the processor and 
    /// used to identify the item in the storage container.
    /// </param>
    public delegate void FileEventHandler(object sender, string fileName, object identifier);

    /// <summary>
    /// Delegate for file error events.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="fileName">File name.</param>
    /// <param name="identifier">
    /// An optional identifier passed from the processor and 
    /// used to identify the item in the storage container.
    /// </param>
    /// <param name="ex">The exception that was raised.</param>
    public delegate void FileErrorEventHandler(object sender, string fileName, object identifier, Exception ex);

    /// <summary>
    /// Implements a stream which can be used to parse an RFC1867 (HTTP upload)
    /// compliant HTTP request.
    /// 
    /// The stream writes it's output to an IFileProcessor implementation on a
    /// file by file basis.
    /// </summary>
    class FormStream : Stream, IDisposable
    {
        #region Declarations

        IFileProcessor _processor;
        IFileProcessor _defaultProcessor;
        Encoding _encoding;
        MemoryStream _formContent;
        MemoryStream _currentField;
        long _position;
        int _keepBackLength;
        bool _fileError;
        Exception _ex;
        string _fileName;
        string _statusKey = String.Empty;
        string _currentFieldName = String.Empty;
        Dictionary<string, string> _previousFields;

        byte[] EOF;
        byte[] BOUNDARY;
        byte[] EOH;
        byte[] CRLF;
        byte[] ID_TAG;
        byte[] DEFAULT_PARAMS_TAG;
        byte[] PARAMS_TAG;
        byte[] END_TAG;

        #endregion

        #region Events

        /// <summary>
        /// Fired when an exception occurs.
        /// </summary>
        public event ErrorEventHandler Error;

        /// <summary>
        /// Fires the error event.
        /// </summary>
        /// <param name="ex">Exception information.</param>
        protected void OnError(Exception ex)
        {
            _ex = ex;

            if (Error != null)
            {
                Error(this, new ErrorEventArgs(ex));
            }
        }


        /// <summary>
        /// Fired when a new file is started.
        /// </summary>
        public event FileEventHandler FileStarted;

        /// <summary>
        /// Fires the FileStarted event.
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <param name="identifier">
        /// An optional identifier passed from the processor and 
        /// used to identify the item in the storage container.
        /// </param>
        protected void OnFileStarted(string fileName, object identifier)
        {
            if (FileStarted != null)
                FileStarted(this, fileName, identifier);
        }

        /// <summary>
        /// Fired when a file is completed sucessfully.
        /// </summary>
        public event FileEventHandler FileCompleted;

        /// <summary>
        /// Fires the FileCompleted event.
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <param name="identifier">
        /// An optional identifier passed from the processor and 
        /// used to identify the item in the storage container.
        /// </param>
        protected void OnFileCompleted(string fileName, object identifier)
        {
            if (FileCompleted != null)
                FileCompleted(this, fileName, identifier);
        }

        /// <summary>
        /// Fired when a file is discarded because of an error.
        /// </summary>
        public event FileErrorEventHandler FileCompletedError;

        /// <summary>
        /// Fires the FileCompletedError event.
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <param name="identifier">
        /// An optional identifier passed from the processor and 
        /// used to identify the item in the storage container.
        /// </param>
        /// <param name="ex">The exception raised.</param>
        protected void OnFileCompletedError(string fileName, object identifier, Exception ex)
        {
            if (FileCompletedError != null)
                FileCompletedError(this, fileName, identifier, ex);
        }

        #endregion

        #region SectionResult

        /// <summary>
        /// Defines the result of processing of a single section.
        /// </summary>
        class SectionResult
        {
            #region Declarations

            SectionAction _nextAction;
            int _nextOffset;

            #endregion

            /// <summary>
            /// Defines the possible actions to take at the end of a section.
            /// </summary>
            public enum SectionAction
            {
                /// <summary>
                /// A boundary has been reached. End the current file or field and look for the next header.
                /// The next header may also by the end of the content.
                /// </summary>
                BoundaryReached,

                /// <summary>
                /// No boundary was found. Keep back a buffer and move on.
                /// </summary>
                NoBoundaryKeepBuffer
            }

            /// <summary>
            /// Get/sets the next action to carry out.
            /// </summary>
            public SectionAction NextAction
            {
                get { return _nextAction; }
            }

            /// <summary>
            /// Gets/sets the next offset to process.
            /// </summary>
            public int NextOffset
            {
                get { return _nextOffset; }
            }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="nextOffset">The next offset to process.</param>
            /// <param name="nextAction">The next action to complete.</param>
            public SectionResult(int nextOffset, SectionAction nextAction)
            {
                _nextAction = nextAction;
                _nextOffset = nextOffset;
            }
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the previous fileds.
        /// </summary>
        public Dictionary<string, string> PreviousFields
        {
            get { return _previousFields; }
        }

        /// <summary>
        /// Gets a byte array containing the content of the form without
        /// any uploaded files.
        /// </summary>
        public byte[] ContentMinusFiles
        {
            get { return _formContent.ToArray(); }
        }

        /// <summary>
        /// Gets the status key.
        /// </summary>
        /// <value>The status key.</value>
        public string StatusKey
        {
            get { return _statusKey; }
        }

        /// <summary>
        /// Gets the processor.
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
        /// <param name="processor">The file processor to use for file persistance.</param>
        /// <param name="boundary">The boundary (delimiter) of the post.</param>
        /// <param name="encoding">The encoder of the request stream.</param>
        public FormStream(IFileProcessor processor, string boundary, Encoding encoding)
        {
            _defaultProcessor = processor;
            _processor = processor;
            _formContent = new MemoryStream();
            _encoding = encoding;
            _headerNeeded = true;
            _position = 0;
            _buffer = null;
            _inField = false;
            _inField = false;
            _keepBackLength = boundary.Length + 6;
            _fileError = false;
            _previousFields = new Dictionary<string, string>();

            BOUNDARY = _encoding.GetBytes(boundary);
            EOF = _encoding.GetBytes(boundary + "--\r\n");
            EOH = _encoding.GetBytes("\r\n\r\n");
            CRLF = _encoding.GetBytes("\r\n");
            ID_TAG = _encoding.GetBytes(DJUploadController.UPLOAD_ID_TAG);
            DEFAULT_PARAMS_TAG = _encoding.GetBytes(DJUploadController.UPLOAD_DEFAULT_PARAMETER_TAG);
            PARAMS_TAG = _encoding.GetBytes(DJFileUpload.UPLOAD_PARAMETER_TAG);
            END_TAG = _encoding.GetBytes(DJFileUpload.UPLOAD_END_TAG);
        }

        #endregion

        #region Stream implementation

        /// <summary>
        /// Determines if the stream can be read.
        /// </summary>
        public override bool CanRead
        {
            get { return false; }
        }

        /// <summary>
        /// Determines if seek operations are valid on the stream.
        /// </summary>
        public override bool CanSeek
        {
            get { return false; }
        }

        /// <summary>
        /// Determines if write operations are valid on the stream.
        /// </summary>
        public override bool CanWrite
        {
            get { return true; }
        }

        /// <summary>
        /// Flushes the output buffer of the stream.
        /// </summary>
        public override void Flush()
        {
            if (_buffer != null && _buffer.Length > 0)
            {
                _formContent.Write(_buffer, 0, _buffer.Length);
            }
        }

        /// <summary>
        /// Gets the current length of the stream.
        /// </summary>
        public override long Length
        {
            get { return _position; }
        }

        /// <summary>
        /// Gets the current position in the stream.
        /// </summary>
        public override long Position
        {
            get
            {
                return _position;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Reads data from the stream.
        /// </summary>
        /// <param name="buffer">Buffer to write to.</param>
        /// <param name="offset">Offset to read from.</param>
        /// <param name="count">Count of bytes to read.</param>
        /// <returns>The actual number of bytes read.</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Seeks a position in the stream.
        /// </summary>
        /// <param name="offset">Offset to read from.</param>
        /// <param name="origin">Origin.</param>
        /// <returns>The position in the stream.</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the length of the stream.
        /// </summary>
        /// <param name="value">Length.</param>
        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        bool _headerNeeded;
        bool _inField;
        bool _inFile;
        byte[] _buffer;

        /// <summary>
        /// Writes data to the stream.
        /// </summary>
        /// <param name="bytes">Buffer to write from.</param>
        /// <param name="offset">Offset in the buffer to write from.</param>
        /// <param name="count">Count of the bytes to write.</param>
        public override void Write(byte[] bytes, int offset, int count)
        {
            byte[] input;
            int start = 0;
            int end = 0;

            // System.Diagnostics.Debug.WriteLine("Entering write");

            if (_buffer != null)
            {
                input = new byte[_buffer.Length + count];
                Buffer.BlockCopy(_buffer, 0, input, 0, _buffer.Length);
                Buffer.BlockCopy(bytes, offset, input, _buffer.Length, count);
            }
            else
            {
                input = new byte[count];
                Buffer.BlockCopy(bytes, offset, input, 0, count);
            }

            _position += count;

            while (true)
            {
                if (_headerNeeded)
                {
                    int prevStart = start;

                    start = IndexOf(input, BOUNDARY, start);

                    if (start >= 0)
                    {
                        end = IndexOf(input, EOF, start);

                        if (end == start)
                        {
                            // This is the end of the stream
                            WriteBytes(false, input, start, input.Length - start);
                            // System.Diagnostics.Debug.WriteLine("EOF");
                            break;
                        }

                        // Do we have the whole header?
                        end = IndexOf(input, EOH, start);

                        if (end >= 0)
                        {
                            Dictionary<string, string> headerItems;

                            // We have a full header
                            _inField = true;
                            _headerNeeded = false;

                            headerItems = ParseHeader(input, start);

                            if (headerItems == null)
                            {
                                throw new Exception("Malformed header");
                            }

                            if (headerItems.ContainsKey("filename") && headerItems.ContainsKey("Content-Type"))
                            {
                                string fn = headerItems["filename"].Trim('"').Trim();

                                if (!String.IsNullOrEmpty(fn))
                                {
                                    try
                                    {
                                        object id;

                                        _fileName = headerItems["filename"].Trim('"');
                                        _inFile = true;
                                        id = _processor.StartNewFile(fn, headerItems["Content-Type"], headerItems, _previousFields);

                                        OnFileStarted(fn, id);
                                    }
                                    catch (Exception ex)
                                    {
                                        _fileError = true;
                                        OnError(ex);
                                    }

                                }
                            }
                            else
                            {
                                _inFile = false;
                                _currentField = new MemoryStream();
                                _currentFieldName = headerItems["name"];
                            }

                            start = end + 4;
                        }
                        else
                        {
                            _buffer = new byte[input.Length - start];
                            Buffer.BlockCopy(input, start, _buffer, 0, input.Length - start);
                            // System.Diagnostics.Debug.WriteLine("Breaking because no header found (2)");
                            break;
                        }
                    }
                    else
                    {
                        _buffer = new byte[input.Length - prevStart];
                        Buffer.BlockCopy(input, prevStart, _buffer, 0, input.Length - prevStart);
                        // System.Diagnostics.Debug.WriteLine("Breaking because no header found (1)");
                        break;
                    }
                }

                SectionResult res = null;

                if (_inField)
                {
                    _buffer = null; // Reset the buffer

                    // Process data
                    res = ProcessField(input, start);

                    if (res.NextAction == SectionResult.SectionAction.BoundaryReached)
                    {
                        // System.Diagnostics.Debug.WriteLine("Found a new boundary");
                        _headerNeeded = true;
                        _inField = false;
                        start = res.NextOffset;

                        if (_inFile)
                        {
                            _inFile = false;

                            try
                            {
                                _processor.EndFile();
                            }
                            catch (Exception ex)
                            {
                                OnError(ex);
                                _fileError = true;
                            }
                            finally
                            {
                                if (_fileError)
                                    OnFileCompletedError(_fileName, _processor.GetIdentifier(), _ex);
                                else
                                    OnFileCompleted(_fileName, _processor.GetIdentifier());
                            }
                        }
                    }
                    else if (res.NextAction == SectionResult.SectionAction.NoBoundaryKeepBuffer)
                    {
                        // System.Diagnostics.Debug.WriteLine("Keeping back " + (input.Length - res.NextOffset).ToString() + " bytes");
                        _buffer = new byte[input.Length - res.NextOffset];
                        Buffer.BlockCopy(input, res.NextOffset, _buffer, 0, input.Length - res.NextOffset);
                        break;

                    }
                }

                if (!_headerNeeded && !_inField)
                {
                    throw new Exception("Malformed input file- don't know what to do so aborting.");
                }
            }

            // System.Diagnostics.Debug.WriteLine("Leaving write");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Parses a header and returns a dictionary of name/value pairs representing the fields.
        /// </summary>
        /// <param name="bytes">Input bytes.</param>
        /// <param name="pos">Position to start at.</param>
        /// <returns>A dictionary of name/value pairs representing the fields.</returns>
        Dictionary<string, string> ParseHeader(byte[] bytes, int pos)
        {
            Dictionary<string, string> items;
            string header;
            string[] headerLines;
            int start;
            int end;

            string input = _encoding.GetString(bytes, pos, bytes.Length - pos);

            start = input.IndexOf("\r\n", 0);
            if (start < 0) return null;

            end = input.IndexOf("\r\n\r\n", start);
            if (end < 0) return null;

            WriteBytes(false, bytes, pos, end + 4 - 0); // Write the header to the form content

            header = input.Substring(start, end - start);

            items = new Dictionary<string, string>();

            headerLines = System.Text.RegularExpressions.Regex.Split(header, "\r\n");

            foreach (string hl in headerLines)
            {
                string[] lineParts = hl.Split(';');

                for (int i = 0; i < lineParts.Length; i++)
                {
                    string[] p;

                    if (i == 0)
                        p = lineParts[i].Split(':');
                    else
                        p = lineParts[i].Split('=');

                    if (p.Length == 2)
                    {
                        items.Add(p[0].Trim(), p[1].Trim());
                    }
                }
            }

            return items;
        }

        /// <summary>
        /// Attempts to find an action field in a byte array.
        /// </summary>
        /// <param name="tag">The action tag.</param>
        /// <param name="bytes">Input bytes.</param>
        /// <param name="result">Result string.</param>
        /// <param name="boundaryPos">The index of the next boundary if found.</param>
        /// <returns>True if an action field was located.</returns>
        bool TryParseActionField(byte[] tag, byte[] bytes, out string result, int boundaryPos)
        {
            int length = boundaryPos == -1 ? bytes.Length : boundaryPos;
            int startPos = IndexOf(bytes, tag, 0, length);

            if (startPos == 0)
            {
                byte[] idBytes;

                idBytes = new byte[length - 2];
                Buffer.BlockCopy(bytes, 0, idBytes, 0, idBytes.Length);
                result = _encoding.GetString(idBytes);

                return true;
            }
            else
            {
                result = String.Empty;
                return false;
            }
        }

        /// <summary>
        /// Checks the current field for actions (ID tag, processor parameters, etc).
        /// </summary>
        void CheckForActionFields()
        {
            byte[] bytes = _currentField.ToArray();
            string statusKey = string.Empty;
            int boundaryPos = IndexOf(bytes, BOUNDARY);

            // Status ID field
            if (!TryParseActionField(ID_TAG, bytes, out statusKey, boundaryPos))
            {
                string field = String.Empty;

                // Processor parameters
                if (TryParseActionField(DEFAULT_PARAMS_TAG, bytes, out field, boundaryPos))
                {
                    // Switch the default processor
                    _defaultProcessor = UploadManager.Instance.DeserializeProcessor(field.Substring(DEFAULT_PARAMS_TAG.Length));
                    _processor = _defaultProcessor;
                    _currentField = new MemoryStream();
                }
                else if (TryParseActionField(PARAMS_TAG, bytes, out field, boundaryPos))
                {
                    // Switch to the new processor
                    _processor = UploadManager.Instance.DeserializeProcessor(field.Substring(PARAMS_TAG.Length));
                    _currentField = new MemoryStream();
                }
                else if (TryParseActionField(END_TAG, bytes, out field, boundaryPos))
                {
                    // Revert to the default processor
                    _processor = _defaultProcessor;
                    _currentField = new MemoryStream();
                }
                else
                {
                    _currentFieldName = _currentFieldName.Replace("\"", String.Empty);

                    if (!_previousFields.ContainsKey(_currentFieldName))
                    {
                        try
                        {
                            string content;

                            if (boundaryPos > 0)
                            {
                                content = _encoding.GetString(bytes, 0, boundaryPos - 2);
                            }
                            else
                            {
                                content = _encoding.GetString(bytes, 0, bytes.Length - 2);
                            }

                            _previousFields.Add(_currentFieldName, content);
                        }
                        catch
                        {
                            _previousFields.Add(_currentFieldName, null);
                        }
                    }
                }
            }
            else
            {
                // Save the status key
                _statusKey = statusKey;
                _currentField = new MemoryStream();
            }
        }

        /// <summary>
        /// Processes a field.
        /// </summary>
        /// <param name="bytes">The input string</param>
        /// <param name="pos">The position to start at.</param>
        /// <returns>The SectionResult class giving the next actions for the processor.</returns>
        SectionResult ProcessField(byte[] bytes, int pos)
        {
            int end = -1;

            if (pos < bytes.Length - 1)
            {
                end = IndexOf(bytes, BOUNDARY, pos + 1);

                // Skip the last 2 bytes of the boundary - thanks to Vince for this
                if (end != -1 && _inFile) end -= 2;
            }

            if (end >= 0)
            {
                WriteBytes(_inFile, bytes, pos, end - pos);

                if (!_inFile)
                {
                    CheckForActionFields();
                }

                return new SectionResult(end, SectionResult.SectionAction.BoundaryReached);
            }
            else
            {
                end = bytes.Length - _keepBackLength;

                // If we don't have enough bytes to hold back a boundary then just keep what we can
                if (end > pos)
                {
                    WriteBytes(_inFile, bytes, pos, end - pos);
                }
                else
                {
                    end = pos;
                }

                return new SectionResult(end, SectionResult.SectionAction.NoBoundaryKeepBuffer);
            }
        }

        /// <summary>
        /// Writes a string to the output buffer.
        /// </summary>
        /// <param name="toFile">True to write to a file, false for form content.</param>
        /// <param name="bytes">Bytes to write.</param>
        /// <param name="pos">Position to write from.</param>
        /// <param name="count">Character count.</param>
        void WriteBytes(bool toFile, byte[] bytes, int pos, int count)
        {
            if (toFile)
            {
                if (!_fileError)
                {
                    try
                    {
                        _processor.Write(bytes, pos, count);
                    }
                    catch (Exception ex)
                    {
                        _fileError = true;
                        OnError(ex);
                    }
                }
            }
            else
            {
                _fileError = false;
                _formContent.Write(bytes, pos, count);
                if (_currentField != null) _currentField.Write(bytes, pos, count);
            }
        }

        /// <summary>
        /// Finds the position of a byte array within a byte array.
        /// </summary>
        /// <param name="buffer">Array to search within.</param>
        /// <param name="checkFor">Bytes to check for.</param>
        /// <returns>The index of the byte array.</returns>
        int IndexOf(byte[] buffer, byte[] checkFor)
        {
            return IndexOf(buffer, checkFor, 0, buffer.Length);
        }

        /// <summary>
        /// Finds the position of a byte array within a byte array.
        /// </summary>
        /// <param name="buffer">Array to search within.</param>
        /// <param name="checkFor">Bytes to check for.</param>
        /// <param name="start">Position to start at.</param>
        /// <returns>The index of the byte array.</returns>
        int IndexOf(byte[] buffer, byte[] checkFor, int start)
        {
            return IndexOf(buffer, checkFor, start, buffer.Length - start);
        }

        /// <summary>
        /// Finds the position of a byte array within a byte array.
        /// </summary>
        /// <param name="buffer">Array to search within.</param>
        /// <param name="checkFor">Bytes to check for.</param>
        /// <param name="start">Position to start at.</param>
        /// <param name="count">Number of bytes to search.</param>
        /// <returns>The index of the byte array.</returns>
        int IndexOf(byte[] buffer, byte[] checkFor, int start, int count)
        {
            int index = 0;

            int startPos = Array.IndexOf(buffer, checkFor[0], start);

            if (startPos != -1)
            {
                while ((startPos + index) < buffer.Length)
                {
                    if (buffer[startPos + index] == checkFor[index])
                    {
                        index++;
                        if (index == checkFor.Length)
                        {
                            return startPos;
                        }
                    }
                    else
                    {
                        startPos = Array.IndexOf<byte>(buffer, checkFor[0], startPos + index);
                        if (startPos == -1)
                        {
                            return -1;
                        }
                        index = 0;
                    }
                }
            }

            return -1;
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Dispose of the object.
        /// </summary>
        void IDisposable.Dispose()
        {
            if (_processor != null)
            {
                _processor.Dispose();
            }

            Flush();
            Close();
        }

        #endregion
    }
}
