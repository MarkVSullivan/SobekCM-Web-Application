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
using System.Configuration;

namespace darrenjohnstone.net.FileUpload
{
    /// <summary>
    /// Configuration section for the upload module. Defines global settings for the module
    /// in web.config.
    /// </summary>
    public sealed class UploadConfigurationSection : ConfigurationSection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UploadConfigurationSection"/> class.
        /// </summary>
        public UploadConfigurationSection()
        {
        }

        /// <summary>
        /// Gets or sets the HTTP status code to return when the maximum length is exceeded.
        /// </summary>
        /// <value>The allowed file extensions.</value>
        [ConfigurationProperty("lengthExceededHttpCode", DefaultValue = 400, IsRequired = false)]
        public int LengthExceededHttpCode
        {
            get { return (int)this["lengthExceededHttpCode"]; }
        }

        /// <summary>
        /// Gets or sets the allowed file extensions (a comma separated list .pdf,.zip,.gif).
        /// </summary>
        /// <value>The allowed file extensions.</value>
        [ConfigurationProperty("allowedFileExtensions", DefaultValue = "", IsRequired = false)]
        public string AllowedFileExtensions
        {
            get { return this["allowedFileExtensions"] as string; }
        }

        /// <summary>
        /// Gets or sets the path to the script file.
        /// </summary>
        /// <value>The script path.</value>
        [ConfigurationProperty("scriptPath", DefaultValue = "/upload_scripts", IsRequired = false)]
        public string ScriptPath
        {
            get { return this["scriptPath"] as string; }
        }

        /// <summary>
        /// Gets or sets the path to the css file.
        /// </summary>
        /// <value>The image path.</value>
        [ConfigurationProperty("cssPath", DefaultValue = "/upload_styles", IsRequired = false)]
        public string CSSPath
        {
            get { return this["cssPath"] as string; }
        }

        /// <summary>
        /// Gets or sets the image path.
        /// </summary>
        /// <value>The image path.</value>
        [ConfigurationProperty("imagePath", DefaultValue = "/upload_images", IsRequired = false)]
        public string ImagePath
        {
            get { return this["imagePath"] as string; }
        }

        /// <summary>
        /// Gets or sets the progress page.
        /// </summary>
        /// <value>The URL of the progress page.</value>
        [ConfigurationProperty("progressUrl", DefaultValue = "UploadProgress.aspx", IsRequired = false)]
        public string ProgressUrl
        {
            get { return this["progressUrl"] as string; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the cancel button should be shown.
        /// </summary>
        /// <value><c>true</c> if the cancel button should be shown; otherwise, <c>false</c>.</value>
        [ConfigurationProperty("showCancelButton", DefaultValue = true, IsRequired = false)]
        public bool ShowCancelButton
        {
            get { return (bool)this["showCancelButton"]; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the progress bar should be shown.
        /// </summary>
        /// <value><c>true</c> if the progress bar should be shown; otherwise, <c>false</c>.</value>
        [ConfigurationProperty("showProgressBar", DefaultValue = true, IsRequired = false)]
        public bool ShowProgressBar
        {
            get { return (bool)this["showProgressBar"]; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether manual processing should be allowed if the
        /// upload module is not installed.
        /// </summary>
        /// <value><c>true</c> if manual processing is allowed; otherwise, <c>false</c>.</value>
        [ConfigurationProperty("enableManualProcessing", DefaultValue = true, IsRequired = false)]
        public bool EnableManualProcessing
        {
            get { return (bool)this["enableManualProcessing"]; }
        }

        /// <summary>
        /// Gets the configuration section.
        /// </summary>
        /// <returns>The configuration section.</returns>
        public static UploadConfigurationSection GetConfig()
        {
            return ConfigurationManager.GetSection("uploadSettings") as UploadConfigurationSection;
        }
    }
}
