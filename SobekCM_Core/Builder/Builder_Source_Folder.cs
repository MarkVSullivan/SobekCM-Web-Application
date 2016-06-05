#region Using directives

using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.Builder
{
    /// <summary> Class which contains all the specifications for an source folder which may contain 
    /// packages destined to be bulk loaded into a SobekCM library incoming FTP folder, including 
    /// network locations, archiving instructions, and what type of packages are permissable </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("builderFolder")]
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
	        //Can_Move_To_Content_Folder = true;
	        BibID_Roots_Restrictions = String.Empty;
            IncomingFolderID = -1;

            Builder_Module_Set = new Builder_Module_Set_Info();
        }

        /// <summary> Human readable label for this folder </summary>
        [DataMember(Name = "name", EmitDefaultValue = false)]
        [XmlAttribute("name")]
        [ProtoMember(1)]
        public string Folder_Name { get; set; }

        /// <summary> Network folder where packages which fail validation and other checks are placed </summary>
        [DataMember(Name = "failuresFolder", EmitDefaultValue = false)]
        [XmlElement("failuresFolder")]
        [ProtoMember(2)]
        public string Failures_Folder { get; set; }

        /// <summary> Network inbound folder which is checked for new incoming packages or metadata files </summary>
        [DataMember(Name = "inboundFolder", EmitDefaultValue = false)]
        [XmlElement("inboundFolder")]
        [ProtoMember(3)]
        public string Inbound_Folder { get; set; }

        /// <summary> Network processing folder where packages are moved while being loaded into the library </summary>
        [DataMember(Name = "processingFolder", EmitDefaultValue = false)]
        [XmlElement("processingFolder")]
        [ProtoMember(4)]
        public string Processing_Folder { get; set; }

        /// <summary> Flag indicates if incoming packages should be subjected to a checksum validation if
        /// checksums exist within the METS file </summary>
        [DataMember(Name = "performChecksum", EmitDefaultValue = false)]
        [XmlElement("performChecksum")]
        [ProtoMember(5)]
        public bool Perform_Checksum { get; set; }

        /// <summary> Flag indicates if any incoming, unarchived TIFF files should be copied to the archiving directory </summary>
        [DataMember(Name = "archiveTiffs", EmitDefaultValue = false)]
        [XmlElement("archiveTiffs")]
        [ProtoMember(6)]
        public bool Archive_TIFFs { get; set; }

        /// <summary> Flag indicates if all incoming, unarchived files (regardless of type) should be copied to the archiving directory </summary>
        [DataMember(Name = "archiveAll", EmitDefaultValue = false)]
        [XmlElement("archiveAll")]
        [ProtoMember(7)]
        public bool Archive_All_Files { get; set; }

        /// <summary> Flag indicates if this folder accepts DELETE metadata files, or if these should be rejected </summary>
        [DataMember(Name = "allowDeletes", EmitDefaultValue = false)]
        [XmlElement("allowDeletes")]
        [ProtoMember(8)]
        public bool Allow_Deletes { get; set; }

        /// <summary> Flag indicates if this folder accepts folders with resource files but lacking metadata, or if these should be rejected </summary>
        [DataMember(Name = "allowFoldersNoMetadata", EmitDefaultValue = false)]
        [XmlElement("allowFoldersNoMetadata")]
        [ProtoMember(9)]
        public bool Allow_Folders_No_Metadata { get; set; }

        /// <summary> Flag indicates if this folder accepts METADATA UPDATE files, or if these should be rejected </summary>
        [DataMember(Name = "allowMetadataUpdates", EmitDefaultValue = false)]
        [XmlElement("allowMetadataUpdates")]
        [ProtoMember(10)]
        public bool Allow_Metadata_Updates { get; set; }

        /// <summary> If there are any BibID root restrictions related to this incoming folder, they are 
        /// listed here, with 'pipes' between them.  i.e., 'UF|UCF|CA001' </summary>
        [DataMember(Name = "bibIdRootsRestrictions", EmitDefaultValue = false)]
        [XmlElement("bibIdRootsRestrictions")]
        [ProtoMember(11)]
        public string BibID_Roots_Restrictions { get; set; }

        /// <summary> Set of the builder modules used by this incoming folder </summary>
        [DataMember(Name = "builderModuleSet", EmitDefaultValue = false)]
        [XmlElement("builderModuleSet")]
        [ProtoMember(12)]
        public Builder_Module_Set_Info Builder_Module_Set { get; set; }

        /// <summary> Primary key for this incoming folder in the database </summary>
        [DataMember(Name = "id", EmitDefaultValue = false)]
        [XmlAttribute("id")]
        [ProtoMember(13)]
        public int IncomingFolderID { get; set; }

        /// <summary> Return a shallow copy of this builder source folder </summary>
        /// <returns> Shallow copy of this builder source folder </returns>
        public Builder_Source_Folder Copy()
        {
            return new Builder_Source_Folder
            {
                Allow_Deletes = Allow_Deletes, 
                Allow_Folders_No_Metadata = Allow_Folders_No_Metadata, 
                Allow_Metadata_Updates = Allow_Metadata_Updates, 
                Archive_All_Files = Archive_All_Files, 
                Archive_TIFFs = Archive_TIFFs, 
                BibID_Roots_Restrictions = BibID_Roots_Restrictions, 
                Builder_Module_Set = Builder_Module_Set, 
                Failures_Folder = Failures_Folder, 
                Folder_Name = Folder_Name, 
                Inbound_Folder = Inbound_Folder, 
                IncomingFolderID = IncomingFolderID, 
                Perform_Checksum = Perform_Checksum, 
                Processing_Folder = Processing_Folder
            };
        }
    }
}
