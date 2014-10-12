#region Using directives

using System;
using System.Data;
using System.Runtime.Serialization;

#endregion

namespace SobekCM.Core.Settings
{
    /// <summary> Class which contains all the specifications for an source folder which may contain 
    /// packages destined to be bulk loaded into a SobekCM library incoming FTP folder, including 
    /// network locations, archiving instructions, and what type of packages are permissable </summary>
    [DataContract]
    public class Builder_Source_Folder
    {

        /// <summary> Constructor for a new instance of the Builder_Source_Folder class </summary>
        public Builder_Source_Folder()
        {
            Folder_Name = String.Empty;
            Inbound_Folder = String.Empty;
            Failures_Folder = String.Empty;
            Processing_Folder = String.Empty;
            Perform_Checksum = false;
            Archive_TIFFs = true;
            Archive_All_Files = true;
            Allow_Deletes = false;
            Allow_Folders_No_Metadata = false;
            Allow_Metadata_Updates = false;
	        Can_Move_To_Content_Folder = true;
	        BibID_Roots_Restrictions = String.Empty;
        }

        /// <summary> Human readable label for this folder </summary>
        [DataMember]
        public string Folder_Name { get; set; }

        /// <summary> Network folder where packages which fail validation and other checks are placed </summary>
        [DataMember]
        public string Failures_Folder { get; set; }

        /// <summary> Network inbound folder which is checked for new incoming packages or metadata files </summary>
        [DataMember]
        public string Inbound_Folder { get; set; }

        /// <summary> Network processing folder where packages are moved while being loaded into the library </summary>
        [DataMember]
        public string Processing_Folder { get; set; }

        /// <summary> Flag indicates if incoming packages should be subjected to a checksum validation if
        /// checksums exist within the METS file </summary>
        [DataMember]
        public bool Perform_Checksum { get; set; }

        /// <summary> Flag indicates if any incoming, unarchived TIFF files should be copied to the archiving directory </summary>
        [DataMember]
        public bool Archive_TIFFs { get; set; }

        /// <summary> Flag indicates if all incoming, unarchived files (regardless of type) should be copied to the archiving directory </summary>
        [DataMember]
        public bool Archive_All_Files { get; set; }

        /// <summary> Flag indicates if this folder accepts DELETE metadata files, or if these should be rejected </summary>
        [DataMember]
        public bool Allow_Deletes { get; set; }

        /// <summary> Flag indicates if this folder accepts folders with resource files but lacking metadata, or if these should be rejected </summary>
        [DataMember]
        public bool Allow_Folders_No_Metadata { get; set; }

        /// <summary> Flag indicates if this folder accepts METADATA UPDATE files, or if these should be rejected </summary>
        [DataMember]
        public bool Allow_Metadata_Updates { get; set; }

        /// <summary> If there are any BibID root restrictions related to this incoming folder, they are 
        /// listed here, with 'pipes' between them.  i.e., 'UF|UCF|CA001' </summary>
        [DataMember]
        public string BibID_Roots_Restrictions { get; set; }

		/// <summary> Flag indicates if content can be moved, or must be copied, from this incoming
		/// folder to the content folder.  Essentially, set to TRUE if on the same system </summary>
        [DataMember]
		public Nullable<bool> Can_Move_To_Content_Folder { get; set; }

        /// <summary> Contains the raw, unanalyzed, module configuration information, as a string with XML </summary>
        [DataMember]
        public string Module_Configuration_Raw { get; set;  }

 
    }
}
