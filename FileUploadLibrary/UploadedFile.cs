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
using System.Web;

namespace darrenjohnstone.net.FileUpload
{
    /// <summary>
    /// Contains identifying information about an uploaded file.
    /// </summary>
    public class UploadedFile
    {
        #region Declarations

        string _fileName;
        object _identifier;
        Dictionary<string, string> _headerItems;
        Exception _exception;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the file name.
        /// </summary>
        public string FileName
        {
            get { return _fileName; }
        }

        /// <summary>
        /// Gets the container identifier returned from the processor.
        /// </summary>
        public object Identifier
        {
            get { return _identifier; }
        }

        /// <summary>
        /// Gets a dictionary of all items in the header.
        /// </summary>
        public Dictionary<string, string> HeaderItems
        {
            get { return _headerItems; }
        }

        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        /// <value>The exception.</value>
        public Exception Exception
        {
            get { return _exception; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadedFile"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="identifier">The identifier.</param>
        /// <param name="headerItems">The header items.</param>
        public UploadedFile(string fileName, object identifier, Dictionary<string, string> headerItems)
        {
            _fileName = fileName;
            _identifier = identifier;
            _headerItems = headerItems;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadedFile"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="identifier">The identifier.</param>
        /// <param name="headerItems">The header items.</param>
        /// <param name="ex">The exception that was raised.</param>
        public UploadedFile(string fileName, object identifier, Dictionary<string, string> headerItems, Exception ex) : this(fileName, identifier, headerItems)
        {
            _exception = ex;
        }

        #endregion
    }
}
