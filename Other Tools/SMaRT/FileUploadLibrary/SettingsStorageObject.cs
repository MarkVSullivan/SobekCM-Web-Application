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

namespace darrenjohnstone.net.FileUpload
{
    /// <summary>
    /// Stores settings for a processor object.
    /// </summary>
    [Serializable()]
    internal class SettingsStorageObject
    {
        /// <summary>
        /// The encrypted settings cipher text.
        /// </summary>
        public byte[] CipherText;

        /// <summary>
        /// The validation hash.
        /// </summary>
        public byte[] Hash;

        /// <summary>
        /// The cipher initialistion vector.
        /// </summary>
        public byte[] CipherIV;
    }
}
