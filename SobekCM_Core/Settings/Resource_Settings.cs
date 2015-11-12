using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.Settings
{
    /// <summary> Settings related to default values for the digital resource files 
    /// and how resource files should be handeled </summary>
    [Serializable, DataContract, ProtoContract]
    public class Resource_Settings
    {
        /// <summary> Constructor for a new instance of the Resource_Settings class </summary>
        public Resource_Settings()
        {
            Upload_File_Types = String.Empty;
            Upload_Image_Types = String.Empty;
            Online_Item_Submit_Enabled = false;
            Backup_Files_Folder_Name = "sobek_files";


            // Current hard coded
            JPEG_Maximum_Height = 1600;
            JPEG_Maximum_Width = 1200;
        }


        /// <summary> Name for the backup files folder within each digital resource </summary>
        [DataMember(Name = "backupFilesFolderName")]
        [XmlElement("backupFilesFolderName")]
        [ProtoMember(1)]
        public string Backup_Files_Folder_Name { get; set; }

        /// <summary> Gets regular expression for matching file names (with extension) to exclude 
        /// from automatically adding gto the downloads for incoming digital resources </summary>
        [DataMember(Name = "filesToExcludeFromDownloads", EmitDefaultValue = false)]
        [XmlElement("filesToExcludeFromDownloads")]
        [ProtoMember(2)]
        public string Files_To_Exclude_From_Downloads { get; set; }

        /// <summary> Gets the library-wide setting for width of created jpeg derivatives </summary>
        [DataMember(Name = "jpegWidth")]
        [XmlElement("jpegWidth")]
        [ProtoMember(3)]
        public int JPEG_Width { get; set; }

        /// <summary> Gets the library-wide setting for height of created jpeg derivatives </summary>
        [DataMember(Name = "jpegHeight")]
        [XmlElement("jpegHeight")]
        [ProtoMember(4)]
        public int JPEG_Height { get; set; }

        /// <summary> Gets the library-wide setting for MAXIMUM width of uploaded jpegs before they are downsampled and a zoomable image created </summary>
        [DataMember(Name = "jpegMaxWidth")]
        [XmlElement("jpegMaxWidth")]
        [ProtoMember(5)]
        public int JPEG_Maximum_Width { get; set; }

        /// <summary> Gets the library-wide setting for MAXIMUM height of uploaded jpegs before they are downsampled and a zoomable image created </summary>
        [DataMember(Name = "jpegMaxHeight")]
        [XmlElement("jpegMaxHeight")]
        [ProtoMember(6)]
        public int JPEG_Maximum_Height { get; set; }

        /// <summary> Flag indicates if online submissions and edits can occur at the moment </summary>
        [DataMember(Name = "onlineItemSubmitEnabled")]
        [XmlElement("onlineItemSubmitEnabled")]
        [ProtoMember(7)]
        public bool Online_Item_Submit_Enabled { get; set; }

        /// <summary> Flag indicates if the citation should be show publicly for items that are currently dark </summary>
        [DataMember(Name = "showCitationForDarkItems")]
        [XmlElement("showCitationForDarkItems")]
        [ProtoMember(8)]
        public bool Show_Citation_For_Dark_Items { get { return false; } }

        /// <summary> Gets the library-wide setting for height for created jpeg thumbnails </summary>
        [DataMember(Name = "thumbnailHeight")]
        [XmlElement("thumbnailHeight")]
        [ProtoMember(9)]
        public int Thumbnail_Height { get; set; }

        /// <summary> Gets the library-wide setting for width for created jpeg thumbnails </summary>
        [DataMember(Name = "thumbnailWidth")]
        [XmlElement("thumbnailWidth")]
        [ProtoMember(10)]
        public int Thumbnail_Width { get; set; }

        /// <summary> List of file type extensions which can be uploaded in the
        /// file management interface. These should all treated as downloads in the system. </summary>
        [DataMember(Name = "uploadFileTypes")]
        [XmlElement("uploadFileTypes")]
        [ProtoMember(11)]
        public string Upload_File_Types { get; set; }

        /// <summary> List of file type extensions which can be uploaded in the page
        /// image upload interface. These should all be treated as page files in the system </summary>
        [DataMember(Name = "uploadImageTypes")]
        [XmlElement("uploadImageTypes")]
        [ProtoMember(12)]
        public string Upload_Image_Types { get; set; }
    }
}
