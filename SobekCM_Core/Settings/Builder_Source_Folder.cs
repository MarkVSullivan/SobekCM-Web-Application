#region Using directives

using System;
using System.Data;

#endregion

namespace SobekCM.Core.Settings
{
    /// <summary> Class which contains all the specifications for an source folder which may contain 
    /// packages destined to be bulk loaded into a SobekCM library incoming FTP folder, including 
    /// network locations, archiving instructions, and what type of packages are permissable </summary>
    public class Builder_Source_Folder
    {

        /// <summary> Constructor for a new instance of the Builder_Source_Folder class </summary>
        /// <param name="Source_Data_Row"> Data row from the builder settings procedure which contains all the specifications 
        /// for this incoming source folder, including network locations, archiving instructions, and what type of packages are permissable </param>
        public Builder_Source_Folder( DataRow Source_Data_Row )
        {
            Folder_Name = Source_Data_Row["FolderName"].ToString();
            Inbound_Folder = Source_Data_Row["NetworkFolder"].ToString();
            Failures_Folder = Source_Data_Row["ErrorFolder"].ToString();
            Processing_Folder = Source_Data_Row["ProcessingFolder"].ToString();
            Perform_Checksum = Convert.ToBoolean(Source_Data_Row["Perform_Checksum_Validation"]);
            Archive_TIFFs = Convert.ToBoolean(Source_Data_Row["Archive_TIFF"]);
            Archive_All_Files = Convert.ToBoolean(Source_Data_Row["Archive_All_Files"]);
            Allow_Deletes = Convert.ToBoolean(Source_Data_Row["Allow_Deletes"]);
            Allow_Folders_No_Metadata = Convert.ToBoolean(Source_Data_Row["Allow_Folders_No_Metadata"]);
            Allow_Metadata_Updates = Convert.ToBoolean(Source_Data_Row["Allow_Metadata_Updates"]);
			BibID_Roots_Restrictions = Source_Data_Row["BibID_Roots_Restrictions"].ToString();
	        if (Source_Data_Row["Can_Move_To_Content_Folder"] == DBNull.Value)
		        Can_Move_To_Content_Folder = null;
	        else
				Can_Move_To_Content_Folder = Convert.ToBoolean(Source_Data_Row["Can_Move_To_Content_Folder"]);
        }

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
        public string Folder_Name { get; protected set; }

        /// <summary> Network folder where packages which fail validation and other checks are placed </summary>
        public string Failures_Folder { get; protected set; }

        /// <summary> Network inbound folder which is checked for new incoming packages or metadata files </summary>
        public string Inbound_Folder { get; protected set; }

        /// <summary> Network processing folder where packages are moved while being loaded into the library </summary>
        public string Processing_Folder { get; protected set; }

        /// <summary> Flag indicates if incoming packages should be subjected to a checksum validation if
        /// checksums exist within the METS file </summary>
        public bool Perform_Checksum { get; protected set; }

        /// <summary> Flag indicates if any incoming, unarchived TIFF files should be copied to the archiving directory </summary>
        public bool Archive_TIFFs { get; protected set; }

        /// <summary> Flag indicates if all incoming, unarchived files (regardless of type) should be copied to the archiving directory </summary>
        public bool Archive_All_Files { get; protected set; }

        /// <summary> Flag indicates if this folder accepts DELETE metadata files, or if these should be rejected </summary>
        public bool Allow_Deletes { get; protected set; }

        /// <summary> Flag indicates if this folder accepts folders with resource files but lacking metadata, or if these should be rejected </summary>
        public bool Allow_Folders_No_Metadata { get; protected set; }

        /// <summary> Flag indicates if this folder accepts METADATA UPDATE files, or if these should be rejected </summary>
        public bool Allow_Metadata_Updates { get; protected set; }

        /// <summary> If there are any BibID root restrictions related to this incoming folder, they are 
        /// listed here, with 'pipes' between them.  i.e., 'UF|UCF|CA001' </summary>
        public string BibID_Roots_Restrictions { get; protected set; }

		/// <summary> Flag indicates if content can be moved, or must be copied, from this incoming
		/// folder to the content folder.  Essentially, set to TRUE if on the same system </summary>
		public Nullable<bool> Can_Move_To_Content_Folder { get; set; }

 
    }
}
