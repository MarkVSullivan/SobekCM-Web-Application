#region Using directives

using System.Runtime.Serialization;

#endregion

namespace SobekCM.Core.Settings
{
    /// <summary> Information about a single file extension, associated MIME type, and related
    /// system settings </summary>
    [DataContract]
    public class Mime_Type_Info
    {
        /// <summary> File extension for this MIME type  </summary>
        [DataMember(EmitDefaultValue = false)]
        public string Extension { get; set; }

        /// <summary> MIME type for files of this extension </summary>
        [DataMember(EmitDefaultValue = false)]
        public string MIME_Type { get; set; }

        /// <summary> Flag indicates files of this extension are explicitly blocked </summary>
        [DataMember]
        public bool isBlocked { get; set; }

        /// <summary> Flag indicates if there are special features within IIS for which
        /// files of this type should be handed to IIS to handle (for example, byte-enabled video) </summary>
        [DataMember]
        public bool shouldForward { get; set; }

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
