#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;
using SobekCM.Resource_Object.METS_Sec_ReaderWriters;

#endregion

namespace SobekCM.Resource_Object.Configuration
{
    /// <summary> Profile defines which METS sections reader/writers are used when writing 
    /// a METS file. </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("MetsWritingProfile")]
    public class METS_Writing_Profile
    {
        /// <summary> Name associated with this profile </summary>
        [DataMember(Name = "name", EmitDefaultValue = false)]
        [XmlAttribute("name")]
        [ProtoMember(1)]
        public string Profile_Name { get; set; }

        /// <summary> Description associated with this profile </summary>
        [DataMember(Name = "description", EmitDefaultValue = false)]
        [XmlAttribute("description")]
        [ProtoMember(2)]
        public string Profile_Description { get; set; }

        /// <summary> Flag indicates if this is the default profile </summary>
        [DataMember(Name = "default", EmitDefaultValue = false)]
        [XmlAttribute("default")]
        [ProtoMember(3)]
        public bool Default_Profile { get; set;  }

        /// <summary> Collection of all the package-level amdSec reader/writer configurations </summary>
        [DataMember(Name = "packageAmdSecWriterConfigs")]
        [XmlArray("packageAmdSecWriterConfigs")]
        [XmlArrayItem("writerConfig", typeof(METS_Section_ReaderWriter_Config))]
        [ProtoMember(4)]
        public List<METS_Section_ReaderWriter_Config> Package_Level_AmdSec_Writer_Configs { get; set; }

        /// <summary> Collection of all the package-level dmdSec reader/writer configurations </summary>
        [DataMember(Name = "packageDmdSecWriterConfigs")]
        [XmlArray("packageDmdSecWriterConfigs")]
        [XmlArrayItem("writerConfig", typeof(METS_Section_ReaderWriter_Config))]
        [ProtoMember(5)]
        public List<METS_Section_ReaderWriter_Config> Package_Level_DmdSec_Writer_Configs { get; set; }

        /// <summary> Collection of all the division-level amdSec reader/writer configurations </summary>
        [DataMember(Name = "divisionAmdSecWriterConfigs")]
        [XmlArray("divisionAmdSecWriterConfigs")]
        [XmlArrayItem("writerConfig", typeof(METS_Section_ReaderWriter_Config))]
        [ProtoMember(6)]
        public List<METS_Section_ReaderWriter_Config> Division_Level_AmdSec_Writer_Configs { get; set; }

        /// <summary> Collection of all the division-level dmdSec reader/writer configurations </summary>
        [DataMember(Name = "divisionDmdSecWriterConfigs")]
        [XmlArray("divisionDmdSecWriterConfigs")]
        [XmlArrayItem("writerConfig", typeof(METS_Section_ReaderWriter_Config))]
        [ProtoMember(7)]
        public List<METS_Section_ReaderWriter_Config> Division_Level_DmdSec_Writer_Configs { get; set; }

        /// <summary> Collection of all the file-level amdSec reader/writer configurations </summary>
        [DataMember(Name = "fileAmdSecWriterConfigs")]
        [XmlArray("fileAmdSecWriterConfigs")]
        [XmlArrayItem("writerConfig", typeof(METS_Section_ReaderWriter_Config))]
        [ProtoMember(8)]
        public List<METS_Section_ReaderWriter_Config> File_Level_AmdSec_Writer_Configs { get; set; }

        /// <summary> Collection of all the file-level dmdSec reader/writer configurations </summary>
        [DataMember(Name = "fileDmdSecWriterConfigs")]
        [XmlArray("fileDmdSecWriterConfigs")]
        [XmlArrayItem("writerConfig", typeof(METS_Section_ReaderWriter_Config))]
        [ProtoMember(9)]
        public List<METS_Section_ReaderWriter_Config> File_Level_DmdSec_Writer_Configs { get; set; }

        /// <summary> Constructor for a new instance of a METS_Writing_Profile </summary>
        public METS_Writing_Profile()
        {
            Package_Level_AmdSec_Writer_Configs = new List<METS_Section_ReaderWriter_Config>();
            Package_Level_DmdSec_Writer_Configs = new List<METS_Section_ReaderWriter_Config>();
            Division_Level_AmdSec_Writer_Configs = new List<METS_Section_ReaderWriter_Config>();
            Division_Level_DmdSec_Writer_Configs = new List<METS_Section_ReaderWriter_Config>();
            File_Level_AmdSec_Writer_Configs = new List<METS_Section_ReaderWriter_Config>();
            File_Level_DmdSec_Writer_Configs = new List<METS_Section_ReaderWriter_Config>();

            Default_Profile = false;
            Profile_Name = String.Empty;
            Profile_Description = String.Empty;
        }

        /// <summary> Clears all the information about which reader/writers to use 
        /// for this METS writing profile </summary>
        public void Clear()
        {
            Package_Level_AmdSec_Writer_Configs.Clear();
            Package_Level_DmdSec_Writer_Configs.Clear();
            Division_Level_AmdSec_Writer_Configs.Clear();
            Division_Level_DmdSec_Writer_Configs.Clear();
            File_Level_AmdSec_Writer_Configs.Clear();
            File_Level_DmdSec_Writer_Configs.Clear();
        }

        /// <summary> Add a new package-level amdSec reader/writer configuration </summary>
        /// <param name="NewConfig"> New METS section reader writer configuration </param>
        public void Add_Package_Level_AmdSec_Writer_Config( METS_Section_ReaderWriter_Config NewConfig )
        {
            if (NewConfig == null)
                return;

           // if ( NewConfig.ReaderWriterObject is iPackage_amdSec_ReaderWriter )
                Package_Level_AmdSec_Writer_Configs.Add(NewConfig);
        }

        /// <summary> Add a new package-level dmdSec reader/writer configuration </summary>
        /// <param name="NewConfig"> New METS section reader writer configuration </param>
        public void Add_Package_Level_DmdSec_Writer_Config(METS_Section_ReaderWriter_Config NewConfig)
        {
            if (NewConfig == null)
                return;

           // if (NewConfig.ReaderWriterObject is iPackage_dmdSec_ReaderWriter)
                Package_Level_DmdSec_Writer_Configs.Add(NewConfig);
        }

        /// <summary> Add a new division-level amdSec reader/writer configuration </summary>
        /// <param name="NewConfig"> New METS section reader writer configuration </param>
        public void Add_Division_Level_AmdSec_Writer_Config(METS_Section_ReaderWriter_Config NewConfig)
        {
            if (NewConfig == null)
                return;

          //  if (NewConfig.ReaderWriterObject is iDivision_amdSec_ReaderWriter)
                Division_Level_AmdSec_Writer_Configs.Add(NewConfig);
        }

        /// <summary> Add a new division-level dmdSec reader/writer configuration </summary>
        /// <param name="NewConfig"> New METS section reader writer configuration </param>
        public void Add_Division_Level_DmdSec_Writer_Config(METS_Section_ReaderWriter_Config NewConfig)
        {
            if (NewConfig == null)
                return;

           // if (NewConfig.ReaderWriterObject is iDivision_dmdSec_ReaderWriter)
                Division_Level_DmdSec_Writer_Configs.Add(NewConfig);
        }

        /// <summary> Add a new file-level amdSec reader/writer configuration </summary>
        /// <param name="NewConfig"> New METS section reader writer configuration </param>
        public void Add_File_Level_AmdSec_Writer_Config(METS_Section_ReaderWriter_Config NewConfig)
        {
            if (NewConfig == null)
                return;

           // if (NewConfig.ReaderWriterObject is iFile_amdSec_ReaderWriter)
                File_Level_AmdSec_Writer_Configs.Add(NewConfig);
        }

        /// <summary> Add a new file-level dmdSec reader/writer configuration </summary>
        /// <param name="NewConfig"> New METS section reader writer configuration </param>
        public void Add_File_Level_DmdSec_Writer_Config(METS_Section_ReaderWriter_Config NewConfig)
        {
            if (NewConfig == null)
                return;

          //  if (NewConfig.ReaderWriterObject is iFile_dmdSec_ReaderWriter)
                File_Level_DmdSec_Writer_Configs.Add(NewConfig);
        }
    }
}
