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
using System.Runtime.Serialization;

namespace darrenjohnstone.net.FileUpload
{
    /// <summary>
    /// This is a test processor similar to the FileSystemProcessor. This processor
    /// demonstrates the use of the PreviousFields parameter to take action based
    /// on user fields populated before the current file input but on the same form.
    /// </summary>
    [Serializable()]
    public class FieldTestProcessor : IFileProcessor
    {
        #region Declarations

        [NonSerialized()]
        FileStream _fs;

        string _outputPath;

        [NonSerialized()]
        string _fileName;

        [NonSerialized()]
        string _fullFileName = String.Empty;

        [NonSerialized()]
        bool _errorState;

        [NonSerialized()]
        Dictionary<string, string> _headerItems;

        #endregion

        #region Properties

        /// <summary>
        /// Gets/sets the output folder path.
        /// </summary>
        public string OutputPath
        {
            get { return _outputPath; }
            set
            {
                value = value.Trim();

                if (!value.EndsWith(@"\")) value += @"\";

                if (!Directory.Exists(value))
                    throw new ArgumentException("Directory does not exist:" + value);

                _outputPath = value;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public FieldTestProcessor()
        {
            // Default to the root of the web application
            _outputPath = System.Web.HttpContext.Current.Server.MapPath("~/");
        }

        #endregion

        #region IFileProcessor Members

        /// <summary>
        /// Starts a new file.
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <param name="contentType">The content type of the file.</param>
        /// <param name="headerItems">A dictionary of items pulled from the header of the field.</param>
        /// <param name="previousFields">A dictionary of previous fields.</param>
        /// <returns>An optional object used to identify the item in the storage container.</returns>
        public object StartNewFile(string fileName, string contentType, Dictionary<string, string> headerItems, Dictionary<string, string> previousFields)
        {
            string prefix = String.Empty;

            _errorState = false;
            _headerItems = headerItems;

            // Rename the file to fit the SobekCM requirements
            fileName = fileName.Replace(" ", "_").Replace("!", "").Replace("@", "").Replace("#", "").Replace("$", "").Replace("%", "").Replace("^", "").Replace("&", "").Replace("(", "").Replace(")", "");

            // Get the prefix from the drop down list
            if (previousFields.ContainsKey("lstFilePrefix"))
            {
                prefix = previousFields["lstFilePrefix"];
            }

            try
            {
                _fileName = fileName;
                _fullFileName = _outputPath + prefix + Path.GetFileName(fileName);
                _fs = new FileStream(_fullFileName, FileMode.Create);
            }
            catch (Exception ex)
            {
                _errorState = true;
                throw ex;
            }

            return null;
        }

        /// <summary>
        /// Writes to the output file.
        /// </summary>
        /// <param name="buffer">Buffer to write from.</param>
        /// <param name="offset">Offset in the buffer to write from.</param>
        /// <param name="count">Count of bytes to write.</param>
        public void Write(byte[] buffer, int offset, int count)
        {
            if (_errorState) return;

            try
            {
                _fs.Write(buffer, offset, count);
            }
            catch (Exception ex)
            {
                _errorState = true;
                throw ex;
            }
        }

        /// <summary>
        /// Ends current file processing.
        /// </summary>
        public void EndFile()
        {
            if (_errorState) return;

            if (_fs != null)
            {
                _fs.Flush();
                _fs.Close();
                _fs.Dispose();
            }
        }

        /// <summary>
        /// Returns the name of the file that is currently being processed.
        /// Null if there is no file.
        /// </summary>
        /// <returns>The file name.</returns>
        public string GetFileName()
        {
            return _fileName;
        }

        /// <summary>
        /// Returns the container identifier.
        /// </summary>
        /// <returns>The container identifier.</returns>
        public object GetIdentifier()
        {
            return null;
        }

        /// <summary>
        /// Gets the header items.
        /// </summary>
        public Dictionary<string, string> GetHeaderItems()
        {
            return _headerItems;
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Dispose of the object.
        /// </summary>
        void IDisposable.Dispose()
        {
            if (_fs != null)
            {
                _fs.Dispose();
            }
        }

        #endregion
    }
}
