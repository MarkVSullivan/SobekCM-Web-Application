using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SobekCM.Resource_Object.METS_Sec_ReaderWriters;

namespace SobekCM.Resource_Object.Configuration
{
    /// <summary> Profile defines which METS sections reader/writers are used when writing 
    /// a METS file. </summary>
    public class METS_Writing_Profile
    {
        private readonly List<METS_Section_ReaderWriter_Config> packageLevelAmdSecWriterConfigs;
        private readonly List<METS_Section_ReaderWriter_Config> packageLevelDmdSecWriterConfigs;
        private readonly List<METS_Section_ReaderWriter_Config> divisionLevelAmdSecWriterConfigs;
        private readonly List<METS_Section_ReaderWriter_Config> divisionLevelDmdSecWriterConfigs;
        private readonly List<METS_Section_ReaderWriter_Config> fileLevelAmdSecWriterConfigs;
        private readonly List<METS_Section_ReaderWriter_Config> fileLevelDmdSecWriterConfigs;

        /// <summary> Name associated with this profile </summary>
        public string Profile_Name { get; set; }

        /// <summary> Description associated with this profile </summary>
        public string Profile_Description { get; set; }

        /// <summary> Flag indicates if this is the default profile </summary>
        public bool Default_Profile { get; set;  }

        /// <summary> Constructor for a new instance of a METS_Writing_Profile </summary>
        public METS_Writing_Profile()
        {
            packageLevelAmdSecWriterConfigs = new List<METS_Section_ReaderWriter_Config>();
            packageLevelDmdSecWriterConfigs = new List<METS_Section_ReaderWriter_Config>();
            divisionLevelAmdSecWriterConfigs = new List<METS_Section_ReaderWriter_Config>();
            divisionLevelDmdSecWriterConfigs = new List<METS_Section_ReaderWriter_Config>();
            fileLevelAmdSecWriterConfigs = new List<METS_Section_ReaderWriter_Config>();
            fileLevelDmdSecWriterConfigs = new List<METS_Section_ReaderWriter_Config>();

            Default_Profile = false;
            Profile_Name = String.Empty;
            Profile_Description = String.Empty;
        }

        /// <summary> Clears all the information about which reader/writers to use 
        /// for this METS writing profile </summary>
        public void Clear()
        {
            packageLevelAmdSecWriterConfigs.Clear();
            packageLevelDmdSecWriterConfigs.Clear();
            divisionLevelAmdSecWriterConfigs.Clear();
            divisionLevelAmdSecWriterConfigs.Clear();
            fileLevelAmdSecWriterConfigs.Clear();
            fileLevelDmdSecWriterConfigs.Clear();
        }

        /// <summary> Add a new package-level amdSec reader/writer configuration </summary>
        /// <param name="NewConfig"> New METS section reader writer configuration </param>
        public void Add_Package_Level_AmdSec_Writer_Config( METS_Section_ReaderWriter_Config NewConfig )
        {
            if (NewConfig == null)
                return;

            if ( NewConfig.ReaderWriterObject is iPackage_amdSec_ReaderWriter )
                packageLevelAmdSecWriterConfigs.Add(NewConfig);
        }

        /// <summary> Add a new package-level dmdSec reader/writer configuration </summary>
        /// <param name="NewConfig"> New METS section reader writer configuration </param>
        public void Add_Package_Level_DmdSec_Writer_Config(METS_Section_ReaderWriter_Config NewConfig)
        {
            if (NewConfig == null)
                return;

            if (NewConfig.ReaderWriterObject is iPackage_dmdSec_ReaderWriter)
                packageLevelDmdSecWriterConfigs.Add(NewConfig);
        }

        /// <summary> Add a new division-level amdSec reader/writer configuration </summary>
        /// <param name="NewConfig"> New METS section reader writer configuration </param>
        public void Add_Division_Level_AmdSec_Writer_Config(METS_Section_ReaderWriter_Config NewConfig)
        {
            if (NewConfig == null)
                return;

            if (NewConfig.ReaderWriterObject is iDivision_amdSec_ReaderWriter)
                divisionLevelAmdSecWriterConfigs.Add(NewConfig);
        }

        /// <summary> Add a new division-level dmdSec reader/writer configuration </summary>
        /// <param name="NewConfig"> New METS section reader writer configuration </param>
        public void Add_Division_Level_DmdSec_Writer_Config(METS_Section_ReaderWriter_Config NewConfig)
        {
            if (NewConfig == null)
                return;

            if (NewConfig.ReaderWriterObject is iDivision_dmdSec_ReaderWriter)
                divisionLevelDmdSecWriterConfigs.Add(NewConfig);
        }

        /// <summary> Add a new file-level amdSec reader/writer configuration </summary>
        /// <param name="NewConfig"> New METS section reader writer configuration </param>
        public void Add_File_Level_AmdSec_Writer_Config(METS_Section_ReaderWriter_Config NewConfig)
        {
            if (NewConfig == null)
                return;

            if (NewConfig.ReaderWriterObject is iFile_amdSec_ReaderWriter)
                fileLevelAmdSecWriterConfigs.Add(NewConfig);
        }

        /// <summary> Add a new file-level dmdSec reader/writer configuration </summary>
        /// <param name="NewConfig"> New METS section reader writer configuration </param>
        public void Add_File_Level_DmdSec_Writer_Config(METS_Section_ReaderWriter_Config NewConfig)
        {
            if (NewConfig == null)
                return;

            if (NewConfig.ReaderWriterObject is iFile_dmdSec_ReaderWriter)
                fileLevelDmdSecWriterConfigs.Add(NewConfig);
        }

        /// <summary> Collection of all the package-level amdSec reader/writer configurations </summary>
        public ReadOnlyCollection<METS_Section_ReaderWriter_Config> Package_Level_AmdSec_Writer_Configs
        {
            get { return new ReadOnlyCollection<METS_Section_ReaderWriter_Config>(packageLevelAmdSecWriterConfigs); }
        }

        /// <summary> Collection of all the package-level dmdSec reader/writer configurations </summary>
        public ReadOnlyCollection<METS_Section_ReaderWriter_Config> Package_Level_DmdSec_Writer_Configs
        {
            get { return new ReadOnlyCollection<METS_Section_ReaderWriter_Config>(packageLevelDmdSecWriterConfigs); }
        }

        /// <summary> Collection of all the division-level amdSec reader/writer configurations </summary>
        public ReadOnlyCollection<METS_Section_ReaderWriter_Config> Division_Level_AmdSec_Writer_Configs
        {
            get { return new ReadOnlyCollection<METS_Section_ReaderWriter_Config>(divisionLevelAmdSecWriterConfigs); }
        }

        /// <summary> Collection of all the division-level dmdSec reader/writer configurations </summary>
        public ReadOnlyCollection<METS_Section_ReaderWriter_Config> Division_Level_DmdSec_Writer_Configs
        {
            get { return new ReadOnlyCollection<METS_Section_ReaderWriter_Config>(divisionLevelDmdSecWriterConfigs); }
        }

        /// <summary> Collection of all the file-level amdSec reader/writer configurations </summary>
        public ReadOnlyCollection<METS_Section_ReaderWriter_Config> File_Level_AmdSec_Writer_Configs
        {
            get { return new ReadOnlyCollection<METS_Section_ReaderWriter_Config>(fileLevelAmdSecWriterConfigs); }
        }

        /// <summary> Collection of all the file-level dmdSec reader/writer configurations </summary>
        public ReadOnlyCollection<METS_Section_ReaderWriter_Config> File_Level_DmdSec_Writer_Configs
        {
            get { return new ReadOnlyCollection<METS_Section_ReaderWriter_Config>(fileLevelDmdSecWriterConfigs); }
        }


    }
}
