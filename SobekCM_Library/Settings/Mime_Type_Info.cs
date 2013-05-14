using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SobekCM.Library.Settings
{
    /// <summary> Information about a single file extension, associated MIME type, and related
    /// system settings </summary>
    public class Mime_Type_Info
    {
        /// <summary> File extension for this MIME type  </summary>
        public string Extension { get; private set; }

        /// <summary> MIME type for files of this extension </summary>
        public string MIME_Type { get; private set; }

        /// <summary> Flag indicates files of this extension are explicitly blocked </summary>
        public bool isBlocked { get; private set; }

        /// <summary> Flag indicates if there are special features within IIS for which
        /// files of this type should be handed to IIS to handle (for example, byte-enabled video) </summary>
        public bool shouldForward { get; private set; }

        /// <summary> Constructor for a new instance of the Mime_Type_Info class </summary>
        /// <param name="Extension">File extension for this MIME type</param>
        /// <param name="MIME_Type">MIME type for files of this extension</param>
        /// <param name="isBlocked">Flag indicates files of this extension are explicitly blocked</param>
        /// <param name="shouldForward">Flag indicates if there are special features within IIS for which files of this type should be handed to IIS to handle (for example, byte-enabled video)</param>
        public Mime_Type_Info( string Extension, string MIME_Type, bool isBlocked, bool shouldForward )
        {
            this.Extension = Extension;
            this.MIME_Type = MIME_Type;
            this.isBlocked = isBlocked;
            this.shouldForward = false; // shouldForward;
        }
    }
}
