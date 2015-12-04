#region Using directives

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;
using SobekCM.Tools;

#endregion

namespace SobekCM.Resource_Object.Configuration
{
    /// <summary> Standard metadata file types </summary>
    public enum Metadata_File_Type_Enum : byte
    {
        /// <summary> Dublin Core </summary>
        DC,

        /// <summary> Encoded Archival Descriptor (EAD) </summary>
        EAD,

        /// <summary> MARC21 Exchange Format </summary>
        MARC21,

        /// <summary> Marc21 in XML format </summary>
        MARCXML,

        /// <summary> Metadata Encoding and Transmission Scheme (METS) </summary>
        METS,

        /// <summary> Metadata Object Description Standard (MODS) </summary>
        MODS,

        /// <summary> Non-standard metadata </summary>
        OTHER
    }

    /// <summary> Configuration information about a reader/writer class used to read stand-along metadata files </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("MetadataFileReaderWriterConfig")]
    public class Metadata_File_ReaderWriter_Config
    {
        /// <summary> Constructor for a new Metadata_File_ReaderWriter_Config object </summary>
        public Metadata_File_ReaderWriter_Config()
        {
            // Set some defaults
            MD_Type = Metadata_File_Type_Enum.OTHER;
            Other_MD_Type = String.Empty;
            Label = String.Empty;
            Code_Namespace = String.Empty;
            Code_Class = String.Empty;
            Code_Assembly = String.Empty;
            canRead = true;
            canWrite = true;
        }

        /// <summary> Metadata type, if this is a standard metadata type </summary>
        [DataMember(Name = "mdType")]
        [XmlAttribute("mdType")]
        [ProtoMember(1)]
        public Metadata_File_Type_Enum MD_Type { get; set; }

        /// <summary> If this is a non-standard metadata type, code for this metadata type </summary>
        [DataMember(Name = "otherMdType")]
        [XmlAttribute("otherMdType")]
        [ProtoMember(2)]
        public string Other_MD_Type { get; set; }

        /// <summary> Short label which describes this metadata type </summary>
        [DataMember(Name = "label")]
        [XmlAttribute("label")]
        [ProtoMember(3)]
        public string Label { get; set; }

        /// <summary> Namespace within which this reader/writer class appears </summary>
        /// <remarks> For all standard reader/writers, this returns 'SobekCM.Resource_Object.Metadata_File_ReaderWriters'.<br /><br />
        /// This is used for instantiating the reader/writer class. </remarks>
        [DataMember(Name = "namespace")]
        [XmlAttribute("namespace")]
        [ProtoMember(4)]
        public string Code_Namespace { get; set; }

        /// <summary> Class name for the associated reader/writer </summary>
        /// <remarks> This is used for instantiating the reader/writer class. </remarks>
        [DataMember(Name = "class")]
        [XmlAttribute("class")]
        [ProtoMember(5)]
        public string Code_Class { get; set; }

        /// <summary> Assembly name of the DLL which holds this associated reader/writer </summary>
        /// <remarks> For all standard reader/writers, this is an empty string, since they are in this DLL. <br /><br />
        ///  This is used for instantiating the reader/writer class. </remarks>
        [DataMember(Name = "assembly")]
        [XmlAttribute("assembly")]
        [ProtoMember(6)]
        public string Code_Assembly { get; set; }

        /// <summary> Flag indicates if this class should be able to read a METS section </summary>
        [DataMember(Name = "canRead")]
        [XmlAttribute("canRead")]
        [ProtoMember(7)]
        public bool canRead { get; set; }

        /// <summary> Flag indicates if this class should be able to write a METS section </summary>
        [DataMember(Name = "canWrite")]
        [XmlAttribute("canWrite")]
        [ProtoMember(8)]
        public bool canWrite { get; set; }

        /// <summary> List of all the associated standard options, read from the metadata configuration XML file </summary>
        [DataMember(Name = "options")]
        [XmlArray("options")]
        [XmlArrayItem("option", typeof(StringKeyValuePair))]
        [ProtoMember(9)]
        public List<StringKeyValuePair> Options { get; set; }

        /// <summary> Add a new option to the dictionary of associated standard options </summary>
        /// <param name="Key"> Key of this option </param>
        /// <param name="Value"> Value of this option </param>
        public void Add_Option(string Key, string Value)
        {
            if (Options == null)
                Options = new List<StringKeyValuePair>();

            Options.Add(new StringKeyValuePair(Key, Value));
        }

        #region Methods that controls XML serialization

        /// <summary> Method suppresses XML Serialization of the Options collection if it is empty </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeOptions()
        {
            return (Options != null ) && ( Options.Count > 0 );
        }

        /// <summary> Method suppresses XML Serialization of the Code_Assembly property if it is empty </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeCode_Assembly()
        {
            return (!String.IsNullOrEmpty(Code_Assembly));
        }

        /// <summary> Method suppresses XML Serialization of the Other_MD_Type property if it is empty </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeOther_MD_Type()
        {
            return (!String.IsNullOrEmpty(Other_MD_Type));
        }

        #endregion
    }
}