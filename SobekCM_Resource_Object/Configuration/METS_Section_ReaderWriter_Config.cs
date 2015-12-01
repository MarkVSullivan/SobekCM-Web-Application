#region Using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;
using SobekCM.Tools;

#endregion

namespace SobekCM.Resource_Object.Configuration
{
    /// <summary> Type of METS administrative section supported by this reader/writer </summary>
    public enum METS_amdSec_Type_Enum : byte
    {
        /// <summary> Not specified - DEFAULT </summary>
        UNSPECIFIED,

        /// <summary> digiProvMD amdSec</summary>
        DigiProvMD,

        /// <summary> rightsMD amdSec </summary>
        RightsMD,

        /// <summary> sourceMD amdSec </summary>
        SourceMD,

        /// <summary> techMD amdSec </summary>
        TechMD
    }

    /// <summary> Type of METS section supported by this reader/writer </summary>
    public enum METS_Section_Type_Enum : byte
    {
        /// <summary> Not specified - DEFAULT </summary>
        UNSPECIFIED,

        /// <summary> Administrative section </summary>
        AmdSec,

        /// <summary> Descriptive (bibliographic) section </summary>
        DmdSec,
    }

    /// <summary> Configuration information about a reader/writer class used to read or
    /// write a section within a METS file </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("MetsWritingConfig")]
    public class METS_Section_ReaderWriter_Config
    {
        private METS_Section_ReaderWriter_Mapping defaultMapping;
        private Dictionary<string, string> optionsDictionary;

        /// <summary> Constructor for a new METS_Section_ReaderWriter_Config object </summary>
        public METS_Section_ReaderWriter_Config()
        {
            // Set some defaults
            isActive = true;
            Label = String.Empty;
            Code_Namespace = String.Empty;
            Code_Class = String.Empty;
            Code_Assembly = String.Empty;
            ID = String.Empty;
            METS_Section = METS_Section_Type_Enum.UNSPECIFIED;
            AmdSecType = METS_amdSec_Type_Enum.UNSPECIFIED;

            // instantiate collections
            Mappings = new List<METS_Section_ReaderWriter_Mapping>();
            Options = new List<StringKeyValuePair>();
            optionsDictionary = new Dictionary<string, string>();
        }

        /// <summary> ID for this METS section reader/writer </summary>
        [DataMember(Name = "id", EmitDefaultValue = false)]
        [XmlAttribute("id")]
        [ProtoMember(1)]
        public string ID { get; set; }

        /// <summary> Label associated with this METS section reader/writer </summary>
        [DataMember(Name = "label", EmitDefaultValue = false)]
        [XmlAttribute("label")]
        [ProtoMember(2)]
        public string Label { get; set; }

        /// <summary> Namespace within which this reader/writer class appears </summary>
        /// <remarks> For all standard reader/writers, this returns 'SobekCM.Resource_Object.METS_Sec_ReaderWriters'.<br /><br />
        /// This is used for instantiating the reader/writer class. </remarks>
        [DataMember(Name = "namespace", EmitDefaultValue = false)]
        [XmlAttribute("namespace")]
        [ProtoMember(3)]
        public string Code_Namespace { get; set; }

        /// <summary> Class name for the associated reader/writer </summary>
        /// <remarks> This is used for instantiating the reader/writer class. </remarks>
        [DataMember(Name = "class", EmitDefaultValue = false)]
        [XmlAttribute("class")]
        [ProtoMember(4)]
        public string Code_Class { get; set; }

        /// <summary> Assembly name of the DLL which holds this associated reader/writer </summary>
        /// <remarks> For all standard reader/writers, this is an empty string, since they are in this DLL. <br /><br />
        ///  This is used for instantiating the reader/writer class. </remarks>
        [DataMember(Name = "assembly", EmitDefaultValue = false)]
        [XmlAttribute("assembly")]
        [ProtoMember(5)]
        public string Code_Assembly { get; set; }

        /// <summary> Flag indicates if this METS section reader/writer is active </summary>
        [DataMember(Name = "enabled")]
        [XmlAttribute("enabled")]
        [ProtoMember(6)]
        public bool isActive { get; set; }

        /// <summary> If this reader/writer targets an amdSec, the type of amdSec </summary>
        [DataMember(Name = "amdSecType")]
        [XmlAttribute("amdSecType")]
        [ProtoMember(7)]
        public METS_amdSec_Type_Enum AmdSecType { get; set; }

        /// <summary> Which METS section this reader/writer targets </summary>
        [DataMember(Name = "metsSection")]
        [XmlAttribute("metsSection")]
        [ProtoMember(8)]
        public METS_Section_Type_Enum METS_Section { get; set; }

        /// <summary> METS section reader/writer object associated with this configuration </summary>
        [XmlIgnore]
        [IgnoreDataMember]
        public object ReaderWriterObject { get; private set;  }

        /// <summary> If there is an error while creating the reader/writer object, the error is stored here </summary>
        [DataMember(Name = "loadError")]
        [XmlElement("loadError")]
        [ProtoMember(9)]
        public string ReaderWriterObject_Creation_Error { get; private set; }

        /// <summary> Creates the reader/writer object, from the provided code assembly, namespace, and class </summary>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public bool Create_ReaderWriterObject()
        {
            try
            {
                // Using reflection, create an object from the class namespace/name
                //     
                Assembly dllAssembly = Assembly.GetExecutingAssembly();
                if (Code_Assembly.Length > 0)
                {
                    dllAssembly = Assembly.LoadFrom(Code_Assembly);
                }
                
              //  Assembly dllAssembly = Assembly..LoadFrom( Code_Assembly );
                Type readerWriterType = dllAssembly.GetType(Code_Namespace + "." + Code_Class);
                ReaderWriterObject = Activator.CreateInstance(readerWriterType); 
                return true;
            }
            catch (Exception ee)
            {
                ReaderWriterObject_Creation_Error = "Unable to load class from assembly. ( " + Code_Namespace + "." + Code_Class + " ) : " + ee.Message;
                return false;
            }
        }

        /// <summary> Gets the default mapping rom the METS file MDTYPE and OTHERMDTYPE attributes 
        /// which this is utilized when writing a METS file </summary>
        [DataMember(Name = "defaultMapping")]
        [XmlElement("defaultMapping")]
        [ProtoMember(10)]
        public METS_Section_ReaderWriter_Mapping Default_Mapping
        {
            get
            {
                if (defaultMapping != null)
                    return defaultMapping;
                if (Mappings.Count > 0)
                    return Mappings[0];
                return null;
            }
            set { defaultMapping = value; }
        }

        /// <summary> Number of mappings from the METS file MDTYPE and OTHERMDTYPE attributes 
        /// which this METS Section Reader/Writer supports </summary>
        [XmlIgnore]
        [IgnoreDataMember]
        public int Mappings_Count
        {
            get { return Mappings.Count; }
        }

        /// <summary> Collection of all mappings from the METS file MDTYPE and OTHERMDTYPE attributes 
        /// which this METS Section Reader/Writer supports </summary>
        [DataMember(Name = "mappings")]
        [XmlArray("mappings")]
        [XmlArrayItem("mapping", typeof(METS_Section_ReaderWriter_Mapping))]
        [ProtoMember(11)]
        public List<METS_Section_ReaderWriter_Mapping> Mappings { get; set; }


        /// <summary> Add a new mapping from the METS file MDTYPE and OTHERMDTYPE attributes 
        /// which this METS Section Reader/Writer supports </summary>
        /// <param name="New_Mapping"> New mapping </param>
        public void Add_Mapping(METS_Section_ReaderWriter_Mapping New_Mapping)
        {
            Mappings.Add(New_Mapping);
            if (New_Mapping.isDefault)
                defaultMapping = New_Mapping;
        }

        /// <summary> Dictionary of all the associated standard options, read from the metadata configuration XML file </summary>
        [DataMember(Name = "options")]
        [XmlArray("options")]
        [XmlArrayItem("option", typeof(StringKeyValuePair))]
        [ProtoMember(12)]
        public List<StringKeyValuePair> Options { get; set; }

        /// <summary> Add a new option to the dictionary of associated standard options </summary>
        /// <param name="Key"> Key of this option </param>
        /// <param name="Value"> Value of this option </param>
        public void Add_Option(string Key, string Value)
        {
            // Ensure the dictionary is built
            if ((optionsDictionary == null) || ((optionsDictionary.Count != Options.Count)))
            {
                optionsDictionary = new Dictionary<string, string>();
                foreach (StringKeyValuePair thisPair in Options)
                {
                    optionsDictionary[thisPair.Key] = thisPair.Value;
                }
            }

            // Did this key already exist?
            if (optionsDictionary.ContainsKey(Key))
            {
                // Look for it in the list version
                StringKeyValuePair delete = null;
                foreach (StringKeyValuePair thisPair in Options)
                {
                    if (thisPair.Key == Key)
                    {
                        delete = thisPair;
                        break;
                    }
                }

                // If found, remove it
                if (delete != null)
                    Options.Remove(delete);
            }

            // Now, add this one
            optionsDictionary[Key] = Value;
            Options.Add(new StringKeyValuePair(Key, Value));
        }
    }

    /// <summary> Information about how a METS Section reader/writer maps into a METS file </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("MetsSectionMapping")]
    public class METS_Section_ReaderWriter_Mapping
    {
        /// <summary> Constructor for a new instance of the METS_Section_ReaderWriter_Mapping class </summary>
        public METS_Section_ReaderWriter_Mapping()
        {
            MD_Type = String.Empty;
            Other_MD_Type = String.Empty;
            Label = String.Empty;
            isDefault = false;
        }

        /// <summary> Constructor for a new instance of the METS_Section_ReaderWriter_Mapping class </summary>
        /// <param name="MD_Type"> Standard metadata type within the METS dmdSec/amdSec metadata definition tags (or OTHER)</param>
        /// <param name="isDefault"> Flag indicates if this is the default mapping, which means it would be used when writing a new METS file</param>
        public METS_Section_ReaderWriter_Mapping(string MD_Type, bool isDefault)
        {
            this.MD_Type = MD_Type;
            Other_MD_Type = String.Empty;
            Label = String.Empty;
            this.isDefault = isDefault;
        }

        /// <summary> Constructor for a new instance of the METS_Section_ReaderWriter_Mapping class </summary>
        /// <param name="MD_Type"> Standard metadata type within the METS dmdSec/amdSec metadata definition tags (or OTHER)</param>
        /// <param name="Label"> Label associated with this METS section within the METS dmdSec/amdSec metadata definition tags (used when writing)</param>
        /// <param name="isDefault"> Flag indicates if this is the default mapping, which means it would be used when writing a new METS file</param>
        public METS_Section_ReaderWriter_Mapping( string MD_Type, string Label, bool isDefault )
        {
            this.MD_Type = MD_Type;
            Other_MD_Type = String.Empty;
            this.Label = Label;
            this.isDefault = isDefault;
        }

        /// <summary> Constructor for a new instance of the METS_Section_ReaderWriter_Mapping class </summary>
        /// <param name="MD_Type"> Standard metadata type within the METS dmdSec/amdSec metadata definition tags (or OTHER)</param>
        /// <param name="Other_MD_Type"> Other metadata type within the METS dmdSec/amdSec metadata definition tags</param>
        /// <param name="Label"> Label associated with this METS section within the METS dmdSec/amdSec metadata definition tags (used when writing)</param>
        /// <param name="isDefault"> Flag indicates if this is the default mapping, which means it would be used when writing a new METS file</param>
        public METS_Section_ReaderWriter_Mapping(string MD_Type, string Other_MD_Type, string Label, bool isDefault)
        {
            this.MD_Type = MD_Type;
            this.Other_MD_Type = Other_MD_Type;
            this.Label = Label;
            this.isDefault = isDefault;
        }

        /// <summary> Standard metadata type within the METS dmdSec/amdSec metadata definition tags (or OTHER)</summary>
        [DataMember(Name = "mdType", EmitDefaultValue = false)]
        [XmlAttribute("mdType")]
        [ProtoMember(1)]
        public string MD_Type { get; set; }

        /// <summary> Other metadata type within the METS dmdSec/amdSec metadata definition tags </summary>
        [DataMember(Name = "otherMdType", EmitDefaultValue = false)]
        [XmlAttribute("otherMdType")]
        [ProtoMember(2)]
        public string Other_MD_Type { get; set; }

        /// <summary> Label associated with this METS section within the METS dmdSec/amdSec metadata definition tags </summary>
        [DataMember(Name = "label", EmitDefaultValue = false)]
        [XmlAttribute("label")]
        [ProtoMember(3)]
        public string Label { get; set; }

        /// <summary> Flag indicates if this is the default mapping, which means it would be used when writing 
        /// a new METS file </summary>
        [DataMember(Name = "default", EmitDefaultValue = false)]
        [XmlAttribute("default")]
        [ProtoMember(4)]
        public bool isDefault { get; set; }
    }
}