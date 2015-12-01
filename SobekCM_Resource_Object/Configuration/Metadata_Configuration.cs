#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using SobekCM.Resource_Object.METS_Sec_ReaderWriters;
using SobekCM.Tools;

#endregion

namespace SobekCM.Resource_Object.Configuration
{
    /// <summary> Metadata configuration for all the classes used to read/write metadata files
    /// or sections within a METS file </summary>
    public  class Metadata_Configuration
    {
        private bool attemptedRead;

        private readonly List<Metadata_File_ReaderWriter_Config> metadataFileReaderWriterConfigs;
        private readonly List<METS_Section_ReaderWriter_Config> metsSectionReaderWriterConfigs;

        private readonly Dictionary<string, METS_Writing_Profile> metsWritingProfiles;
        private METS_Writing_Profile defaultWritingProfile;

        private readonly List<Additional_Metadata_Module_Config> metadataModules;

        private readonly Dictionary<Tuple<string, string>, iPackage_amdSec_ReaderWriter> packageAmdSecDictionary;
        private readonly Dictionary<Tuple<string, string>, iPackage_dmdSec_ReaderWriter> packageDmdSecDictionary;
        private readonly Dictionary<Tuple<string, string>, iDivision_dmdSec_ReaderWriter> divisionDmdSecDictionary;
        private readonly Dictionary<Tuple<string, string>, iDivision_amdSec_ReaderWriter> divisionAmdSecDictionary;
        private readonly Dictionary<Tuple<string, string>, iFile_dmdSec_ReaderWriter> fileDmdSecDictionary;
        private readonly Dictionary<Tuple<string, string>, iFile_amdSec_ReaderWriter> fileAmdSecDictionary;

        private readonly List<string> errorsEncountered;

        /// <summary> constructor for the Metadata_Configuration class </summary>
        public Metadata_Configuration()
        {
            // Declare all the new collections in this configuration 
            metadataFileReaderWriterConfigs = new List<Metadata_File_ReaderWriter_Config>();
            metsSectionReaderWriterConfigs = new List<METS_Section_ReaderWriter_Config>();
            metsWritingProfiles = new Dictionary<string, METS_Writing_Profile>();
            metadataModules = new List<Additional_Metadata_Module_Config>();
            packageAmdSecDictionary = new Dictionary<Tuple<string, string>, iPackage_amdSec_ReaderWriter>();
            packageDmdSecDictionary = new Dictionary<Tuple<string, string>, iPackage_dmdSec_ReaderWriter>();
            divisionDmdSecDictionary = new Dictionary<Tuple<string, string>, iDivision_dmdSec_ReaderWriter>();
            divisionAmdSecDictionary = new Dictionary<Tuple<string, string>, iDivision_amdSec_ReaderWriter>();
            fileDmdSecDictionary = new Dictionary<Tuple<string, string>, iFile_dmdSec_ReaderWriter>();
            fileAmdSecDictionary = new Dictionary<Tuple<string, string>, iFile_amdSec_ReaderWriter>();

            // Set some default values
            attemptedRead = false;
            errorsEncountered = new List<string>();

            // Set default reader/writer values to have a baseline in case there is no file to be read 
            Set_Default_Values();

        }

        /// <summary> Clear all the current metadata configuration information </summary>
        private void Clear()
        {
            metadataFileReaderWriterConfigs.Clear();
            metsSectionReaderWriterConfigs.Clear();
            packageAmdSecDictionary.Clear();
            packageDmdSecDictionary.Clear();
            divisionDmdSecDictionary.Clear();
            divisionAmdSecDictionary.Clear();
            fileDmdSecDictionary.Clear();
            fileAmdSecDictionary.Clear();
            defaultWritingProfile = null;
            metsWritingProfiles.Clear();
            metadataModules.Clear();
            errorsEncountered.Clear();
        }

        /// <summary> Flag indicates if the method to read the configuration file has been called </summary>
        /// <remarks> Even if the read is unsuccesful for any reason, this returns TRUE to prevent 
        /// the read method from being called over and over </remarks>
        public bool Attempted_To_Read_Config_File
        {
            get { return attemptedRead;  }
        }

        /// <summary> Default METS writing profile utilized by the METS writer </summary>
        public METS_Writing_Profile Default_METS_Writing_Profile
        {
            get
            {
                if ( defaultWritingProfile != null )
                    return defaultWritingProfile;
                if (metsWritingProfiles.Count > 0)
                {
                    string first = metsWritingProfiles.Keys.FirstOrDefault();
                    if ( first != null )
                        return metsWritingProfiles[first];
                }
                return null;
            }
        }

        /// <summary> Adds a new METS writing profile to this metadata configuration </summary>
        /// <param name="NewProfile"> New metadata profile to add </param>
        public void Add_METS_Writing_Profile( METS_Writing_Profile NewProfile )
        {
            metsWritingProfiles[NewProfile.Profile_Name] = NewProfile;
            if (NewProfile.Default_Profile)
                defaultWritingProfile = NewProfile;
        }

        /// <summary> Clear all the METS writing profiles from this configuration </summary>
        public void Clear_METS_Writing_Profiles()
        {
            metsWritingProfiles.Clear();
            defaultWritingProfile = null;
        }

        /// <summary> Add new metadata module configuration information for a module which should always be included </summary>
        /// <param name="MetadatModuleConfig"> New metadata module to include with all new items </param>
        public void Add_Metadata_Module_Config( Additional_Metadata_Module_Config MetadatModuleConfig )
        {
            metadataModules.Add( MetadatModuleConfig );
        }

        /// <summary> List of metadata modules to be included with all bibliographic items </summary>
        public ReadOnlyCollection<Additional_Metadata_Module_Config> Metadata_Modules_To_Include
        {
            get
            {
                return new ReadOnlyCollection<Additional_Metadata_Module_Config>(metadataModules);
            }
        }

        /// <summary> Collection of metadata file reader/writer configurations </summary>
        public ReadOnlyCollection<Metadata_File_ReaderWriter_Config> Metadata_File_ReaderWriter_Configs
        {
            get { return new ReadOnlyCollection<Metadata_File_ReaderWriter_Config>(metadataFileReaderWriterConfigs); }
        }

        /// <summary> Add a new metadata file reader/writer configuration </summary>
        /// <param name="New_ReaderWriter"> New metadata file reader/writer configuration </param>
        public void Add_Metadata_File_ReaderWriter(Metadata_File_ReaderWriter_Config New_ReaderWriter)
        {
            metadataFileReaderWriterConfigs.Add(New_ReaderWriter);
        }

        /// <summary> Collection of all the METS section reader/writer configurations </summary>
        public ReadOnlyCollection<METS_Section_ReaderWriter_Config> METS_Section_File_ReaderWriter_Configs
        {
            get { return new ReadOnlyCollection<METS_Section_ReaderWriter_Config>(metsSectionReaderWriterConfigs); }
        }

        /// <summary> Adds a new METS section reader/writer configuration </summary>
        /// <param name="New_ReaderWriter"> New METS section reader/writer</param>
        /// <remarks> This instantiates the actual reader/writer class to determine which interfaces
        /// are implemeneted. </remarks>
        public void Add_METS_Section_ReaderWriter(METS_Section_ReaderWriter_Config New_ReaderWriter)
        {
            // Add to list of all METS sections readers/writers
            metsSectionReaderWriterConfigs.Add(New_ReaderWriter);

            // Create an instance of this reader/writer from the config
            if (!New_ReaderWriter.Create_ReaderWriterObject())
                return;
            object testObj = New_ReaderWriter.ReaderWriterObject;

            // Set flag defaults
            bool isAmdPackage = false;
            bool isDmdPackage = false;
            bool isDmdDivision = false;
            bool isAmdDivision = false;
            bool isDmdFile = false;
            bool isAmdFile = false;

            // Test for interface inheritence
            if (testObj is iPackage_amdSec_ReaderWriter) isAmdPackage = true;
            if (testObj is iPackage_dmdSec_ReaderWriter) isDmdPackage = true;
            if (testObj is iDivision_dmdSec_ReaderWriter) isDmdDivision = true;
            if (testObj is iDivision_amdSec_ReaderWriter) isAmdDivision = true;
            if (testObj is iFile_dmdSec_ReaderWriter) isDmdFile = true;
            if (testObj is iFile_amdSec_ReaderWriter) isAmdFile = true;

            // Step through all the mappings and add to the dictionaries
            foreach (METS_Section_ReaderWriter_Mapping thisMapping in New_ReaderWriter.Mappings)
            {
                // Create the dictionay key for this mapping
                Tuple<string, string> thisMappingKey = new Tuple<string, string>(thisMapping.MD_Type.ToUpper(), thisMapping.Other_MD_Type.ToUpper());

                // Add to the appropriate dictionary
                if (isAmdPackage)
                    packageAmdSecDictionary[thisMappingKey] = (iPackage_amdSec_ReaderWriter)testObj;
                if (isDmdDivision)
                    divisionDmdSecDictionary[thisMappingKey] = (iDivision_dmdSec_ReaderWriter)testObj;
                if (isDmdPackage)
                    packageDmdSecDictionary[thisMappingKey] = (iPackage_dmdSec_ReaderWriter)testObj;
                if (isAmdDivision)
                    divisionAmdSecDictionary[thisMappingKey] = (iDivision_amdSec_ReaderWriter)testObj;
                if (isDmdFile)
                    fileDmdSecDictionary[thisMappingKey] = (iFile_dmdSec_ReaderWriter)testObj;
                if (isAmdFile)
                    fileAmdSecDictionary[thisMappingKey] = (iFile_amdSec_ReaderWriter)testObj;
            }
        }

        /// <summary> Get a package-level amdSec METS section reader/writer by the METS file MDTYPE and OTHERMDTYPE attributes
        /// present in the METS section tag </summary>
        /// <param name="MdType"> MDTYPE attribute from the METS file section being read </param>
        /// <param name="OtherMdType"> OTHERMDTYPE attribute from the METS file section being read </param>
        /// <returns> Reader/writer class ready to read the METS section </returns>
        public iPackage_amdSec_ReaderWriter Get_Package_AmdSec_ReaderWriter( string MdType, string OtherMdType )
        {
            Tuple<string, string> thisMappingKey = new Tuple<string, string>(MdType.ToUpper(), OtherMdType.ToUpper());
            return packageAmdSecDictionary.ContainsKey(thisMappingKey) ? packageAmdSecDictionary[thisMappingKey] : null;
        }

        /// <summary> Get a package-level dmdSec METS section reader/writer by the METS file MDTYPE and OTHERMDTYPE attributes
        /// present in the METS section tag </summary>
        /// <param name="MdType"> MDTYPE attribute from the METS file section being read </param>
        /// <param name="OtherMdType"> OTHERMDTYPE attribute from the METS file section being read </param>
        /// <returns> Reader/writer class ready to read the METS section </returns>
        public iPackage_dmdSec_ReaderWriter Get_Package_DmdSec_ReaderWriter(string MdType, string OtherMdType)
        {
            Tuple<string, string> thisMappingKey = new Tuple<string, string>(MdType.ToUpper(), OtherMdType.ToUpper());
            return packageDmdSecDictionary.ContainsKey(thisMappingKey) ? packageDmdSecDictionary[thisMappingKey] : null;
        }

        /// <summary> Get a division-level dmdSec METS section reader/writer by the METS file MDTYPE and OTHERMDTYPE attributes
        /// present in the METS section tag </summary>
        /// <param name="MdType"> MDTYPE attribute from the METS file section being read </param>
        /// <param name="OtherMdType"> OTHERMDTYPE attribute from the METS file section being read </param>
        /// <returns> Reader/writer class ready to read the METS section </returns>
        public iDivision_dmdSec_ReaderWriter Get_Division_DmdSec_ReaderWriter(string MdType, string OtherMdType)
        {
            Tuple<string, string> thisMappingKey = new Tuple<string, string>(MdType.ToUpper(), OtherMdType.ToUpper());
            return divisionDmdSecDictionary.ContainsKey(thisMappingKey) ? divisionDmdSecDictionary[thisMappingKey] : null;
        }

        /// <summary> Get a division-level amdSec METS section reader/writer by the METS file MDTYPE and OTHERMDTYPE attributes
        /// present in the METS section tag </summary>
        /// <param name="MdType"> MDTYPE attribute from the METS file section being read </param>
        /// <param name="OtherMdType"> OTHERMDTYPE attribute from the METS file section being read </param>
        /// <returns> Reader/writer class ready to read the METS section </returns>
        public iDivision_amdSec_ReaderWriter Get_Division_AmdSec_ReaderWriter(string MdType, string OtherMdType)
        {
            Tuple<string, string> thisMappingKey = new Tuple<string, string>(MdType.ToUpper(), OtherMdType.ToUpper());
            return divisionAmdSecDictionary.ContainsKey(thisMappingKey) ? divisionAmdSecDictionary[thisMappingKey] : null;
        }

        /// <summary> Get a file-level amdSec METS section reader/writer by the METS file MDTYPE and OTHERMDTYPE attributes
        /// present in the METS section tag </summary>
        /// <param name="MdType"> MDTYPE attribute from the METS file section being read </param>
        /// <param name="OtherMdType"> OTHERMDTYPE attribute from the METS file section being read </param>
        /// <returns> Reader/writer class ready to read the METS section </returns>
        public iFile_amdSec_ReaderWriter Get_File_AmdSec_ReaderWriter(string MdType, string OtherMdType)
        {
            Tuple<string, string> thisMappingKey = new Tuple<string, string>(MdType.ToUpper(), OtherMdType.ToUpper());
            return fileAmdSecDictionary.ContainsKey(thisMappingKey) ? fileAmdSecDictionary[thisMappingKey] : null;
        }

        /// <summary> Get a file-level dmdSec METS section reader/writer by the METS file MDTYPE and OTHERMDTYPE attributes
        /// present in the METS section tag </summary>
        /// <param name="MdType"> MDTYPE attribute from the METS file section being read </param>
        /// <param name="OtherMdType"> OTHERMDTYPE attribute from the METS file section being read </param>
        /// <returns> Reader/writer class ready to read the METS section </returns>
        public iFile_dmdSec_ReaderWriter Get_File_DmdSec_ReaderWriter(string MdType, string OtherMdType)
        {
            Tuple<string, string> thisMappingKey = new Tuple<string, string>(MdType.ToUpper(), OtherMdType.ToUpper());
            return fileDmdSecDictionary.ContainsKey(thisMappingKey) ? fileDmdSecDictionary[thisMappingKey] : null;
        }

        #region Code to set the default values

        /// <summary> Clears the current metadata configuration and restores all the values
        /// to the default reader/writers and profiles </summary>
        public void Set_Default_Values()
        {
            Clear();

            // Add the dublin core file reader/writer
            Metadata_File_ReaderWriter_Config dcFile = new Metadata_File_ReaderWriter_Config {MD_Type = Metadata_File_Type_Enum.DC, Label = "Dublin Core File", canRead = true, canWrite = true, Code_Assembly = String.Empty, Code_Namespace = "SobekCM.Resource_Object.Metadata_File_ReaderWriters", Code_Class = "DC_File_ReaderWriter"};
            dcFile.Add_Option("RDF_Style", "false");
            Add_Metadata_File_ReaderWriter(dcFile);

            // Add the dublin core file reader/writer
            Metadata_File_ReaderWriter_Config dcFile2 = new Metadata_File_ReaderWriter_Config {MD_Type = Metadata_File_Type_Enum.DC, Label = "Dublin Core File (RDF Style)", canRead = false, canWrite = true, Code_Assembly = String.Empty, Code_Namespace = "SobekCM.Resource_Object.Metadata_File_ReaderWriters", Code_Class = "DC_File_ReaderWriter"};
            dcFile2.Add_Option("RDF_Style", "true");
            Add_Metadata_File_ReaderWriter(dcFile2);

            // Add the EAD file reader/writer
            Metadata_File_ReaderWriter_Config eadFile = new Metadata_File_ReaderWriter_Config {MD_Type = Metadata_File_Type_Enum.EAD, Label = "Encoded Archival Descriptor (EAD)", canRead = true, canWrite = false, Code_Assembly = String.Empty, Code_Namespace = "SobekCM.Resource_Object.Metadata_File_ReaderWriters", Code_Class = "EAD_File_ReaderWriter"};
            eadFile.Add_Option("Analyze_Description", "true");
            Add_Metadata_File_ReaderWriter(eadFile);

            // Add the MARC21 file reader/writer
            Metadata_File_ReaderWriter_Config marc21File = new Metadata_File_ReaderWriter_Config {MD_Type = Metadata_File_Type_Enum.MARC21, Label = "MARC21 Single Record File", canRead = true, canWrite = true, Code_Assembly = String.Empty, Code_Namespace = "SobekCM.Resource_Object.Metadata_File_ReaderWriters", Code_Class = "Marc21_File_ReaderWriter"};
            Add_Metadata_File_ReaderWriter(marc21File);

            // Add the MarcXML file reader/writer
            Metadata_File_ReaderWriter_Config marcxmlFile = new Metadata_File_ReaderWriter_Config {MD_Type = Metadata_File_Type_Enum.MARCXML, Label = "MarcXML Single Record File", canRead = true, canWrite = true, Code_Assembly = String.Empty, Code_Namespace = "SobekCM.Resource_Object.Metadata_File_ReaderWriters", Code_Class = "MarcXML_File_ReaderWriter"};
            Add_Metadata_File_ReaderWriter(marcxmlFile);

            // Add the METS file reader/writer
            Metadata_File_ReaderWriter_Config metsFile = new Metadata_File_ReaderWriter_Config {MD_Type = Metadata_File_Type_Enum.METS, Label = "Metadata Encoding and Transmission Standard (METS)", canRead = true, canWrite = true, Code_Assembly = String.Empty, Code_Namespace = "SobekCM.Resource_Object.Metadata_File_ReaderWriters", Code_Class = "METS_File_ReaderWriter"};
            metsFile.Add_Option("Minimize_File_Info", "false");
            metsFile.Add_Option("Support_Divisional_dmdSec_amdSec", "true");
            Add_Metadata_File_ReaderWriter(metsFile);

            // Add the MODS file reader/writer
            Metadata_File_ReaderWriter_Config modsFile = new Metadata_File_ReaderWriter_Config {MD_Type = Metadata_File_Type_Enum.MODS, Label = "Metadata Object Description Standard (MODS)", canRead = true, canWrite = true, Code_Assembly = String.Empty, Code_Namespace = "SobekCM.Resource_Object.Metadata_File_ReaderWriters", Code_Class = "MODS_File_ReaderWriter"};
            Add_Metadata_File_ReaderWriter(modsFile);

            // Add the INFO file reader/writer
            Metadata_File_ReaderWriter_Config infoFile = new Metadata_File_ReaderWriter_Config {MD_Type = Metadata_File_Type_Enum.OTHER, Other_MD_Type = "INFO", Label = "Legacy UF INFO Files", canRead = true, canWrite = false, Code_Assembly = String.Empty, Code_Namespace = "SobekCM.Resource_Object.Metadata_File_ReaderWriters", Code_Class = "INFO_File_ReaderWriter"};
            Add_Metadata_File_ReaderWriter(infoFile);

            // Add the MXF file reader/writer
            Metadata_File_ReaderWriter_Config mxfFile = new Metadata_File_ReaderWriter_Config {MD_Type = Metadata_File_Type_Enum.OTHER, Other_MD_Type = "MXF", Label = "MXF File", canRead = true, canWrite = false, Code_Assembly = String.Empty, Code_Namespace = "SobekCM.Resource_Object.Metadata_File_ReaderWriters", Code_Class = "MXF_File_ReaderWriter"};
            Add_Metadata_File_ReaderWriter(mxfFile);

            // Add the OAI-PMH file reader/writer
            Metadata_File_ReaderWriter_Config oaiFile = new Metadata_File_ReaderWriter_Config {MD_Type = Metadata_File_Type_Enum.OTHER, Other_MD_Type = "OAI", Label = "OAI-PMH File", canRead = false, canWrite = true, Code_Assembly = String.Empty, Code_Namespace = "SobekCM.Resource_Object.Metadata_File_ReaderWriters", Code_Class = "OAI_File_ReaderWriter"};
            Add_Metadata_File_ReaderWriter(oaiFile);

            // Add the MODS section reader/writer
            METS_Section_ReaderWriter_Config modsSection = new METS_Section_ReaderWriter_Config {ID = "MODS", Label = "MODS", Code_Assembly = String.Empty, Code_Namespace = "SobekCM.Resource_Object.METS_Sec_ReaderWriters", Code_Class = "MODS_METS_dmdSec_ReaderWriter", isActive = true, METS_Section = METS_Section_Type_Enum.DmdSec};
            modsSection.Add_Mapping(new METS_Section_ReaderWriter_Mapping("MODS", "MODS Metadata", true));
            modsSection.Add_Mapping(new METS_Section_ReaderWriter_Mapping("Metadata Object Description Standard", false ));
            Add_METS_Section_ReaderWriter(modsSection);

            // Add the dublin core section reader/writer
            METS_Section_ReaderWriter_Config dcSection = new METS_Section_ReaderWriter_Config {ID = "DC", Label = "Dublin Core", Code_Assembly = String.Empty, Code_Namespace = "SobekCM.Resource_Object.METS_Sec_ReaderWriters", Code_Class = "DC_METS_dmdSec_ReaderWriter", isActive = true, METS_Section = METS_Section_Type_Enum.DmdSec};
            dcSection.Add_Mapping(new METS_Section_ReaderWriter_Mapping("DC", "Dublin Core Metadata", true));
            dcSection.Add_Mapping(new METS_Section_ReaderWriter_Mapping("DUBLIN CORE", false));
            Add_METS_Section_ReaderWriter(dcSection);

            // Add the MarcXML section reader/writer
            METS_Section_ReaderWriter_Config marcXMLSection = new METS_Section_ReaderWriter_Config {ID = "MARCXML", Label = "MARCXML", Code_Assembly = String.Empty, Code_Namespace = "SobekCM.Resource_Object.METS_Sec_ReaderWriters", Code_Class = "MarcXML_METS_dmdSec_ReaderWriter", isActive = true, METS_Section = METS_Section_Type_Enum.DmdSec};
            marcXMLSection.Add_Mapping(new METS_Section_ReaderWriter_Mapping("MARCXML", "MarcXML Metadata", true));
            Add_METS_Section_ReaderWriter(marcXMLSection);

            // Add the DarwinCore section reader/writer
            METS_Section_ReaderWriter_Config darwinSection = new METS_Section_ReaderWriter_Config {ID = "DARWIN", Label = "DarwinCore", Code_Assembly = String.Empty, Code_Namespace = "SobekCM.Resource_Object.METS_Sec_ReaderWriters", Code_Class = "DarwinCore_METS_dmdSec_ReaderWriter", isActive = true, METS_Section = METS_Section_Type_Enum.DmdSec};
            darwinSection.Add_Mapping(new METS_Section_ReaderWriter_Mapping("OTHER", "DARWINCORE", "DarwinCore Zoological Taxonomic Information", true));
            Add_METS_Section_ReaderWriter(darwinSection);

			// Add the ETD section reader/writer
			METS_Section_ReaderWriter_Config etdSection = new METS_Section_ReaderWriter_Config {ID = "ETD", Label = "ETD", Code_Assembly = String.Empty, Code_Namespace = "SobekCM.Resource_Object.METS_Sec_ReaderWriters", Code_Class = "ETD_SobekCM_METS_dmdSec_ReaderWriter", isActive = true, METS_Section = METS_Section_Type_Enum.DmdSec};
            etdSection.Add_Mapping(new METS_Section_ReaderWriter_Mapping("OTHER", "SOBEK_ETD", "SobekCM ETD Extension", true));
			Add_METS_Section_ReaderWriter(etdSection);

            // Add the ETD section reader/writer
            METS_Section_ReaderWriter_Config etd2Section = new METS_Section_ReaderWriter_Config {ID = "ETD2", Label = "ETD2", Code_Assembly = String.Empty, Code_Namespace = "SobekCM.Resource_Object.METS_Sec_ReaderWriters", Code_Class = "ETD_PALMM_METS_dmdSec_ReaderWriter", isActive = true, METS_Section = METS_Section_Type_Enum.DmdSec};
            etd2Section.Add_Mapping(new METS_Section_ReaderWriter_Mapping("OTHER", "PALMM", "PALMM ETD Extension", true));
            etd2Section.Add_Mapping(new METS_Section_ReaderWriter_Mapping("OTHER", "PALMM Extensions", "PALMM ETD Extension", false));
            Add_METS_Section_ReaderWriter(etd2Section);

            // Add the SobekCM section reader/writer
            METS_Section_ReaderWriter_Config sobekCMSection = new METS_Section_ReaderWriter_Config {ID = "SOBEK1", Label = "SobekCM", Code_Assembly = String.Empty, Code_Namespace = "SobekCM.Resource_Object.METS_Sec_ReaderWriters", Code_Class = "SobekCM_METS_dmdSec_ReaderWriter", isActive = true, METS_Section = METS_Section_Type_Enum.DmdSec};
            sobekCMSection.Add_Mapping(new METS_Section_ReaderWriter_Mapping("OTHER", "SOBEKCM", "SobekCM Custom Metadata", true));
            sobekCMSection.Add_Mapping(new METS_Section_ReaderWriter_Mapping("OTHER", "UFDC", "SobekCM Custom Metadata", false));
            sobekCMSection.Add_Mapping(new METS_Section_ReaderWriter_Mapping("OTHER", "DLOC", "SobekCM Custom Metadata", false));
            Add_METS_Section_ReaderWriter(sobekCMSection);

            // Add the SobekCM Map section reader/writer
            METS_Section_ReaderWriter_Config sobekCMMapSection = new METS_Section_ReaderWriter_Config {ID = "SOBEK2", Label = "SobekCM Map", Code_Assembly = String.Empty, Code_Namespace = "SobekCM.Resource_Object.METS_Sec_ReaderWriters", Code_Class = "SobekCM_Map_METS_dmdSec_ReaderWriter", isActive = true, METS_Section = METS_Section_Type_Enum.DmdSec};
            sobekCMMapSection.Add_Mapping(new METS_Section_ReaderWriter_Mapping("OTHER", "SOBEK_MAP", "SobekCM Custom Map Authority Metadata", true));
            sobekCMMapSection.Add_Mapping(new METS_Section_ReaderWriter_Mapping("OTHER", "UFDC_MAP", "SobekCM Custom Map Authority Metadata", false));
            Add_METS_Section_ReaderWriter(sobekCMMapSection);

            // Add the DAITSS section reader/writer
            METS_Section_ReaderWriter_Config daitssSection = new METS_Section_ReaderWriter_Config {ID = "DAITSS", Label = "DAITSS", Code_Assembly = String.Empty, Code_Namespace = "SobekCM.Resource_Object.METS_Sec_ReaderWriters", Code_Class = "DAITSS_METS_amdSec_ReaderWriter", isActive = true, METS_Section = METS_Section_Type_Enum.AmdSec, AmdSecType = METS_amdSec_Type_Enum.DigiProvMD};
            daitssSection.Add_Mapping(new METS_Section_ReaderWriter_Mapping("OTHER", "DAITSS", "DAITSS Archiving Information", true));
            Add_METS_Section_ReaderWriter(daitssSection);

            // Add the RightsMD section reader/writer
            METS_Section_ReaderWriter_Config rightsSection = new METS_Section_ReaderWriter_Config {ID = "RIGHTS", Label = "RightsMD", Code_Assembly = String.Empty, Code_Namespace = "SobekCM.Resource_Object.METS_Sec_ReaderWriters", Code_Class = "RightsMD_METS_amdSec_ReaderWriter", isActive = true, METS_Section = METS_Section_Type_Enum.AmdSec, AmdSecType = METS_amdSec_Type_Enum.RightsMD};
            rightsSection.Add_Mapping(new METS_Section_ReaderWriter_Mapping("OTHER", "RIGHTSMD", "Rights Information", true));
            Add_METS_Section_ReaderWriter(rightsSection);

            // Add the SobekCM fileinfo section reader/writer
            METS_Section_ReaderWriter_Config sobekCMFileSection = new METS_Section_ReaderWriter_Config {ID = "SOBEK3", Label = "SobekCM FileInfo", Code_Assembly = String.Empty, Code_Namespace = "SobekCM.Resource_Object.METS_Sec_ReaderWriters", Code_Class = "SobekCM_FileInfo_METS_amdSec_ReaderWriter", isActive = true, METS_Section = METS_Section_Type_Enum.AmdSec, AmdSecType = METS_amdSec_Type_Enum.TechMD};
            sobekCMFileSection.Add_Mapping(new METS_Section_ReaderWriter_Mapping("OTHER", "SOBEKCM", "SobekCM File Technical Details", true));
            Add_METS_Section_ReaderWriter(sobekCMFileSection);

            // Add the GML section reader/writer
            METS_Section_ReaderWriter_Config gmlSection = new METS_Section_ReaderWriter_Config {ID = "GML", Label = "GML Coordinate", Code_Assembly = String.Empty, Code_Namespace = "SobekCM.Resource_Object.METS_Sec_ReaderWriters", Code_Class = "GML_METS_dmdSec_ReaderWriter", isActive = true, METS_Section = METS_Section_Type_Enum.DmdSec};
            gmlSection.Add_Mapping(new METS_Section_ReaderWriter_Mapping("OTHER", "GML", "Geographic Markup Language", true));
            Add_METS_Section_ReaderWriter(gmlSection);

            // Add the IEEE-LOM section reader/writer
            METS_Section_ReaderWriter_Config lomSection = new METS_Section_ReaderWriter_Config {ID = "IEEE-LOM", Label = "IEEE-LOM: Learning Object Metadata", Code_Assembly = String.Empty, Code_Namespace = "SobekCM.Resource_Object.METS_Sec_ReaderWriters", Code_Class = "LOM_IEEE_METS_dmdSec_ReaderWriter", isActive = true, METS_Section = METS_Section_Type_Enum.DmdSec};
            lomSection.Add_Mapping(new METS_Section_ReaderWriter_Mapping("OTHER", "IEEE-LOM", "Learning Object Metadata", true));
            Add_METS_Section_ReaderWriter(lomSection);

            // Add the VRACore section reader/writer
            METS_Section_ReaderWriter_Config vraSection = new METS_Section_ReaderWriter_Config {ID = "VRACORE", Label = "VRACore Visual Resource Metadata", Code_Assembly = String.Empty, Code_Namespace = "SobekCM.Resource_Object.METS_Sec_ReaderWriters", Code_Class = "VRACore_METS_dmdSec_ReaderWriter", isActive = true, METS_Section = METS_Section_Type_Enum.DmdSec};
            vraSection.Add_Mapping(new METS_Section_ReaderWriter_Mapping("OTHER", "VRACore", "VRACore Visual Resource Metadata", true));
            Add_METS_Section_ReaderWriter(vraSection);

            // Add the default METS writig profile
            METS_Writing_Profile defaultProfile = new METS_Writing_Profile {Default_Profile = true, Profile_Name = "Complete MODS Writer", Profile_Description = "This profile includes almost all of the possible sub-writers but the main bibliographic data is stored in MODS"};
            defaultProfile.Add_Package_Level_DmdSec_Writer_Config(modsSection);
            defaultProfile.Add_Package_Level_DmdSec_Writer_Config(sobekCMSection);
            defaultProfile.Add_Package_Level_DmdSec_Writer_Config(sobekCMMapSection);
            defaultProfile.Add_Package_Level_DmdSec_Writer_Config(darwinSection);
            defaultProfile.Add_Package_Level_DmdSec_Writer_Config(etdSection);
            defaultProfile.Add_Package_Level_DmdSec_Writer_Config(gmlSection);
            defaultProfile.Add_Package_Level_DmdSec_Writer_Config(lomSection);
            defaultProfile.Add_Package_Level_DmdSec_Writer_Config(vraSection);
            defaultProfile.Add_Package_Level_AmdSec_Writer_Config(daitssSection);
            defaultProfile.Add_Package_Level_AmdSec_Writer_Config(rightsSection);
            defaultProfile.Add_Package_Level_AmdSec_Writer_Config(sobekCMFileSection);
         //   defaultProfile.Add_Division_Level_DmdSec_Writer_Config(MODS_Section);
            defaultProfile.Add_Division_Level_DmdSec_Writer_Config(gmlSection);
            Add_METS_Writing_Profile(defaultProfile);

            // Add the default METS writig profile
            METS_Writing_Profile dcProfile = new METS_Writing_Profile {Default_Profile = false, Profile_Name = "Simple Dublin Core Writer", Profile_Description = "This is a simplified profile which uses Dublin Core to describe all levels of the METS"};
            dcProfile.Add_Package_Level_DmdSec_Writer_Config(dcSection);
            dcProfile.Add_Package_Level_AmdSec_Writer_Config(daitssSection);
            dcProfile.Add_Package_Level_AmdSec_Writer_Config(rightsSection);
            dcProfile.Add_Package_Level_AmdSec_Writer_Config(sobekCMFileSection);
            dcProfile.Add_Division_Level_DmdSec_Writer_Config(dcSection);
            dcProfile.Add_File_Level_DmdSec_Writer_Config(dcSection);
            Add_METS_Writing_Profile(dcProfile);


        }

        #endregion

        #region Code to save this metadata configuration to a XML file

        /// <summary> Save this metadata configuration to a XML config file </summary>
        /// <param name="FilePath"> File/path for the resulting XML config file </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public bool Save_To_Config_File(string FilePath)
        {
            bool returnValue = true;
            StreamWriter writer = null;
            try
            {
                writer = new StreamWriter(FilePath, false, Encoding.UTF8);
                writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                writer.WriteLine("<SobekCM_Config xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" ");
                writer.WriteLine("\txmlns=\"http://digital.uflib.ufl.edu/metadata/sobekcm_config\" ");
                writer.WriteLine("\txsi:schemaLocation=\"http://digital.uflib.ufl.edu/metadata/sobekcm_config ");
                writer.WriteLine("\t\thttp://digital.uflib.ufl.edu/metadata/sobekcm_config/sobekcm_config.xsd\">");
                writer.WriteLine("\t<Metadata>");
                writer.WriteLine("\t\t<Metadata_File_ReaderWriters>");
                foreach (Metadata_File_ReaderWriter_Config fileConfig in metadataFileReaderWriterConfigs)
                {
                    writer.Write("\t\t\t<ReaderWriter ");
                    switch (fileConfig.MD_Type)
                    {
                        case Metadata_File_Type_Enum.DC:
                            writer.Write("mdtype=\"DC\" ");
                            break;

                        case Metadata_File_Type_Enum.EAD:
                            writer.Write("mdtype=\"EAD\" ");
                            break;

                        case Metadata_File_Type_Enum.MARCXML:
                            writer.Write("mdtype=\"MARCXML\" ");
                            break;

                        case Metadata_File_Type_Enum.MARC21:
                            writer.Write("mdtype=\"MARC21\" ");
                            break;

                        case Metadata_File_Type_Enum.METS:
                            writer.Write("mdtype=\"METS\" ");
                            break;

                        case Metadata_File_Type_Enum.MODS:
                            writer.Write("mdtype=\"MODS\" ");
                            break;

                        case Metadata_File_Type_Enum.OTHER:
                            writer.Write("mdtype=\"OTHER\" othermdtype=\"" + Convert_String_To_XML_Safe(fileConfig.Other_MD_Type) + "\" ");
                            break;
                    }
                    writer.Write("label=\"" + Convert_String_To_XML_Safe(fileConfig.Label) + "\" namespace=\"" + Convert_String_To_XML_Safe(fileConfig.Code_Namespace) + "\" class=\"" + Convert_String_To_XML_Safe(fileConfig.Code_Class) + "\" ");

                    if (fileConfig.Code_Assembly.Length > 0)
                        writer.Write("assembly=\"" + Convert_String_To_XML_Safe(fileConfig.Code_Assembly) + "\" ");

                    writer.Write(fileConfig.canRead ? "canRead=\"true\" " : "canRead=\"false\" ");
                    writer.Write(fileConfig.canWrite ? "canWrite=\"true\" " : "canWrite=\"false\" ");
                    if (fileConfig.Options.Count > 0)
                    {
                        writer.WriteLine(">");
                        writer.WriteLine("\t\t\t\t<Options>");
                        foreach (string thisKey in fileConfig.Options.Keys)
                        {
                            writer.WriteLine("\t\t\t\t\t<Option key=\"" + Convert_String_To_XML_Safe(thisKey) + "\" value=\"" + Convert_String_To_XML_Safe(fileConfig.Options[thisKey]) + "\" />");
                        }
                        writer.WriteLine("\t\t\t\t</Options>");
                        writer.WriteLine("\t\t\t</ReaderWriter>");
                    }
                    else
                    {
                        writer.WriteLine("/>");
                    }
                }
                writer.WriteLine("\t\t</Metadata_File_ReaderWriters>");

                writer.WriteLine("\t\t<METS_Sec_ReaderWriters>");
                foreach (METS_Section_ReaderWriter_Config fileConfig in metsSectionReaderWriterConfigs)
                {
                    writer.Write("\t\t\t<ReaderWriter ID=\"" + Convert_String_To_XML_Safe(fileConfig.ID) + "\" label=\"" + Convert_String_To_XML_Safe(fileConfig.Label) + "\" namespace=\"" + Convert_String_To_XML_Safe(fileConfig.Code_Namespace) + "\" class=\"" + Convert_String_To_XML_Safe(fileConfig.Code_Class) + "\" ");

                    if (fileConfig.Code_Assembly.Length > 0)
                        writer.Write("assembly=\"" + Convert_String_To_XML_Safe(fileConfig.Code_Assembly) + "\" ");

                    writer.Write(fileConfig.isActive ? "isActive=\"true\" " : "isActive=\"false\" ");

                    switch (fileConfig.METS_Section)
                    {
                        case METS_Section_Type_Enum.DmdSec:
                            writer.Write("section=\"dmdSec\" ");
                            break;

                        case METS_Section_Type_Enum.AmdSec:
                            writer.Write("section=\"amdSec\" ");
                            switch (fileConfig.AmdSecType)
                            {
                                case METS_amdSec_Type_Enum.DigiProvMD:
                                    writer.Write("amdSecType=\"digiProvMD\" ");
                                    break;

                                case METS_amdSec_Type_Enum.RightsMD:
                                    writer.Write("amdSecType=\"rightsMD\" ");
                                    break;

                                case METS_amdSec_Type_Enum.SourceMD:
                                    writer.Write("amdSecType=\"sourceMD\" ");
                                    break;

                                case METS_amdSec_Type_Enum.TechMD:
                                    writer.Write("amdSecType=\"techMD\" ");
                                    break;
                            }
                            break;
                    }
                    writer.WriteLine(">");
                    if (fileConfig.Mappings_Count > 0)
                    {
                        writer.WriteLine("\t\t\t\t<Mappings>");
                        foreach (METS_Section_ReaderWriter_Mapping thisMapping in fileConfig.Mappings)
                        {
                            writer.Write("\t\t\t\t\t<Mapping mdtype=\"" + Convert_String_To_XML_Safe(thisMapping.MD_Type) + "\" ");
                            if (thisMapping.Other_MD_Type.Length > 0)
                                writer.Write("othermdtype=\"" + Convert_String_To_XML_Safe(thisMapping.Other_MD_Type) + "\" ");
                            if (thisMapping.Label.Length > 0)
                                writer.Write("label=\"" + Convert_String_To_XML_Safe(thisMapping.Label) + "\" ");
                            if (thisMapping.isDefault)
                                writer.Write("isDefault=\"true\" ");
                            writer.WriteLine("/>");
                        }
                        writer.WriteLine("\t\t\t\t</Mappings>");
                    }
                    if (fileConfig.Options.Count > 0)
                    {
                        writer.WriteLine("\t\t\t\t<Options>");
                        foreach (StringKeyValuePair thisKey in fileConfig.Options)
                        {
                            writer.WriteLine("\t\t\t\t\t<Option key=\"" + Convert_String_To_XML_Safe(thisKey.Key) + "\" value=\"" + Convert_String_To_XML_Safe(thisKey.Value) + "\" />");
                        }
                        writer.WriteLine("\t\t\t\t</Options>");
                    }
                    writer.WriteLine("\t\t\t</ReaderWriter>");
                }
                writer.WriteLine("\t\t</METS_Sec_ReaderWriters>");

                writer.WriteLine("\t\t<METS_Writing>");


                foreach (METS_Writing_Profile profile in metsWritingProfiles.Values)
                {
                    writer.Write("\t\t\t<Profile ");
                    if (profile.Default_Profile)
                        writer.Write("isDefault=\"true\" ");
                    writer.Write("name=\"" + Convert_String_To_XML_Safe(profile.Profile_Name) + "\" ");
                    writer.Write("description=\"" + Convert_String_To_XML_Safe(profile.Profile_Description) + "\">");

                    writer.WriteLine("\t\t\t\t<Package_Scope>");

                    if (profile.Package_Level_DmdSec_Writer_Configs.Count > 0)
                    {
                        writer.WriteLine("\t\t\t\t\t<dmdSec>");
                        foreach (METS_Section_ReaderWriter_Config thisWriter in profile.Package_Level_DmdSec_Writer_Configs)
                        {
                            writer.WriteLine("\t\t\t\t\t\t<ReaderWriterRef ID=\"" + thisWriter.ID + "\" />");
                        }
                        writer.WriteLine("\t\t\t\t\t</dmdSec>");
                    }
                    if (profile.Package_Level_AmdSec_Writer_Configs.Count > 0)
                    {
                        writer.WriteLine("\t\t\t\t\t<amdSec>");
                        foreach (METS_Section_ReaderWriter_Config thisWriter in profile.Package_Level_AmdSec_Writer_Configs)
                        {
                            writer.WriteLine("\t\t\t\t\t\t<ReaderWriterRef ID=\"" + thisWriter.ID + "\" />");
                        }
                        writer.WriteLine("\t\t\t\t\t</amdSec>");
                    }
                    writer.WriteLine("\t\t\t\t</Package_Scope>");


                    List<METS_Section_ReaderWriter_Config> divAmdSec = profile.Division_Level_AmdSec_Writer_Configs;
                    List<METS_Section_ReaderWriter_Config> divDmdSec = profile.Division_Level_DmdSec_Writer_Configs;
                    if ((divAmdSec.Count > 0) || (divDmdSec.Count > 0))
                    {
                        writer.WriteLine("\t\t\t\t<Division_Scope>");

                        if (divDmdSec.Count > 0)
                        {
                            writer.WriteLine("\t\t\t\t\t<dmdSec>");
                            foreach (METS_Section_ReaderWriter_Config thisWriter in divDmdSec)
                            {
                                writer.WriteLine("\t\t\t\t\t\t<ReaderWriterRef ID=\"" + thisWriter.ID + "\" />");
                            }
                            writer.WriteLine("\t\t\t\t\t</dmdSec>");
                        }
                        if (divAmdSec.Count > 0)
                        {
                            writer.WriteLine("\t\t\t\t\t<amdSec>");
                            foreach (METS_Section_ReaderWriter_Config thisWriter in divAmdSec)
                            {
                                writer.WriteLine("\t\t\t\t\t\t<ReaderWriterRef ID=\"" + thisWriter.ID + "\" />");
                            }
                            writer.WriteLine("\t\t\t\t\t</amdSec>");
                        }
                        writer.WriteLine("\t\t\t\t</Division_Scope>");
                    }

                    List<METS_Section_ReaderWriter_Config> fileAmdSec = profile.File_Level_AmdSec_Writer_Configs;
                    List<METS_Section_ReaderWriter_Config> fileDmdSec = profile.File_Level_DmdSec_Writer_Configs;
                    if ((fileAmdSec.Count > 0) || (fileDmdSec.Count > 0))
                    {
                        writer.WriteLine("\t\t\t\t<File_Scope>");

                        if (fileDmdSec.Count > 0)
                        {
                            writer.WriteLine("\t\t\t\t\t<dmdSec>");
                            foreach (METS_Section_ReaderWriter_Config thisWriter in fileDmdSec)
                            {
                                writer.WriteLine("\t\t\t\t\t\t<ReaderWriterRef ID=\"" + thisWriter.ID + "\" />");
                            }
                            writer.WriteLine("\t\t\t\t\t</dmdSec>");
                        }
                        if (fileAmdSec.Count > 0)
                        {
                            writer.WriteLine("\t\t\t\t\t<amdSec>");
                            foreach (METS_Section_ReaderWriter_Config thisWriter in fileAmdSec)
                            {
                                writer.WriteLine("\t\t\t\t\t\t<ReaderWriterRef ID=\"" + thisWriter.ID + "\" />");
                            }
                            writer.WriteLine("\t\t\t\t\t</amdSec>");
                        }
                        writer.WriteLine("\t\t\t\t</File_Scope>");
                    }

                    writer.WriteLine("\t\t\t</Profile>");
                }

                writer.WriteLine("\t\t</METS_Writing>");

                // Any additional metadata module info to include?
                if (metadataModules.Count > 0)
                {
                    writer.WriteLine("\t\t<Additional_Metadata_Modules>");

                    foreach (Additional_Metadata_Module_Config config in metadataModules)
                    {
                        writer.Write("\t\t\t<MetadataModule key=\"" + Convert_String_To_XML_Safe(config.Key) + "\" ");
                        if (config.Code_Assembly.Length > 0)
                        {
                            writer.Write("assembly=\"" + Convert_String_To_XML_Safe(config.Code_Assembly) + "\" ");
                        }
                        writer.WriteLine("namespace=\"" + Convert_String_To_XML_Safe(config.Code_Namespace) + "\" class=\"" + Convert_String_To_XML_Safe(config.Code_Class) + "\" />");
                    }


                    writer.WriteLine("\t\t</Additional_Metadata_Modules>");
                }


                writer.WriteLine("\t</Metadata>");
                writer.WriteLine("</SobekCM_Config>");
                writer.Flush();
                writer.Close();
            }
            catch
            {
                returnValue = false;
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }

            return returnValue;
        }

        /// <summary> Converts a basic string into an XML-safe string </summary>
        /// <param name="Element"> Element data to convert </param>
        /// <returns> Data converted into an XML-safe string</returns>
        private string Convert_String_To_XML_Safe(string Element)
        {
            if (Element == null)
                return string.Empty;

            string xml_safe = Element;
            int i = xml_safe.IndexOf("&");
            while (i >= 0)
            {
                if ((i != xml_safe.IndexOf("&amp;", i, StringComparison.Ordinal)) && (i != xml_safe.IndexOf("&quot;", i, StringComparison.Ordinal)) &&
                    (i != xml_safe.IndexOf("&gt;", i, StringComparison.Ordinal)) && (i != xml_safe.IndexOf("&lt;", i, StringComparison.Ordinal)))
                {
                    xml_safe = xml_safe.Substring(0, i + 1) + "amp;" + xml_safe.Substring(i + 1);
                }

                i = xml_safe.IndexOf("&", i + 1, StringComparison.Ordinal);
            }
            return xml_safe.Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
        }

        #endregion

        #region Code for reading the configuration from a XML file
        
        /// <summary> Read the metadata configuration from a correctly-formatted metadata configuration XML file </summary>
        /// <param name="Configuration_XML_File"> File/path for the metadata configuration XML file to read </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public bool Read_Metadata_Configuration(string Configuration_XML_File)
        {
            attemptedRead = true;

            // Clear all the values first
            Clear();

            bool returnValue = true;
            // Streams used for reading
            Stream readerStream = null;
            XmlTextReader readerXml = null;

            try
            {
                // Some collections to read into
                Dictionary<string, METS_Section_ReaderWriter_Config> readerWriters = new Dictionary<string, METS_Section_ReaderWriter_Config>();

                // Open a link to the file
                readerStream = new FileStream(Configuration_XML_File, FileMode.Open, FileAccess.Read);

                // Open a XML reader connected to the file
                readerXml = new XmlTextReader(readerStream);

                while (readerXml.Read())
                {
                    if (readerXml.NodeType == XmlNodeType.Element)
                    {
                        switch (readerXml.Name.ToLower())
                        {
                            case "metadata_file_readerwriters":
                                read_metadata_file_readerwriter_configs(readerXml.ReadSubtree());
                                break;

                            case "mets_sec_readerwriters":
                                read_mets_readerwriter_configs(readerXml.ReadSubtree(), readerWriters);
                                break;

                            case "mets_writing":
                                read_mets_writing_config(readerXml.ReadSubtree(), readerWriters);
                                break;

                            case "additional_metadata_modules":
                                read_metadata_modules_config(readerXml.ReadSubtree());
                                break;
                        }
                    }
                }
            }
            catch 
            {
                returnValue = false;
            }
            finally
            {
                if (readerXml != null)
                {
                    readerXml.Close();
                }
                if (readerStream != null)
                {
                    readerStream.Close();
                }
            }

            // If there was an error while reading, use the system defaults
            if (!returnValue)
            {
                Clear();
                Set_Default_Values();
            }

            return returnValue;
        }

        private void read_metadata_file_readerwriter_configs(XmlReader ReaderXml)
        {
            while (ReaderXml.Read())
            {
                if ((ReaderXml.NodeType == XmlNodeType.Element) && (ReaderXml.Name.ToLower() == "readerwriter"))
                {
                    read_metadata_file_readerwriter_config(ReaderXml.ReadSubtree());
                }
            }
        }

        private void read_metadata_file_readerwriter_config(XmlReader ReaderXml)
        {
            Metadata_File_ReaderWriter_Config returnValue = new Metadata_File_ReaderWriter_Config();
            ReaderXml.Read();

            // Move to and save the basic attributes
            if (ReaderXml.MoveToAttribute("mdtype"))
            {
                switch (ReaderXml.Value.ToUpper())
                {
                    case "EAD":
                        returnValue.MD_Type = Metadata_File_Type_Enum.EAD;
                        break;

                    case "DC":
                        returnValue.MD_Type = Metadata_File_Type_Enum.DC;
                        break;

                    case "MARC21":
                        returnValue.MD_Type = Metadata_File_Type_Enum.MARC21;
                        break;

                    case "MARCXML":
                        returnValue.MD_Type = Metadata_File_Type_Enum.MARCXML;
                        break;

                    case "METS":
                        returnValue.MD_Type = Metadata_File_Type_Enum.METS;
                        break;

                    case "MODS":
                        returnValue.MD_Type = Metadata_File_Type_Enum.MODS;
                        break;

                    case "OTHER":
                        returnValue.MD_Type = Metadata_File_Type_Enum.OTHER;
                        if (ReaderXml.MoveToAttribute("othermdtype"))
                            returnValue.Other_MD_Type = ReaderXml.Value;
                        break;
                }
            }

            if (ReaderXml.MoveToAttribute("label"))
                returnValue.Label = ReaderXml.Value;
            if (ReaderXml.MoveToAttribute("namespace"))
                returnValue.Code_Namespace = ReaderXml.Value;
            if (ReaderXml.MoveToAttribute("class"))
                returnValue.Code_Class = ReaderXml.Value;
            if (ReaderXml.MoveToAttribute("assembly"))
                returnValue.Code_Assembly = ReaderXml.Value;
            if ((ReaderXml.MoveToAttribute("canRead")) && (ReaderXml.Value.ToLower() == "false"))
            {
                returnValue.canRead = false;
            }
            if ((ReaderXml.MoveToAttribute("canWrite")) && (ReaderXml.Value.ToLower() == "false"))
            {
                returnValue.canWrite = false;
            }

            while (ReaderXml.Read())
            {
                if ((ReaderXml.NodeType == XmlNodeType.Element) && (ReaderXml.Name.ToLower() == "option"))
                {
                    string key = String.Empty;
                    string value = String.Empty;
                    if (ReaderXml.MoveToAttribute("key"))
                        key = ReaderXml.Value;
                    if (ReaderXml.MoveToAttribute("value"))
                        value = ReaderXml.Value;
                    if ((key.Length > 0) && (value.Length > 0))
                        returnValue.Add_Option(key, value);
                }
            }

            Add_Metadata_File_ReaderWriter(returnValue);
        }

        private void read_mets_readerwriter_configs(XmlReader ReaderXml, Dictionary<string, METS_Section_ReaderWriter_Config> ReaderWriters)
        {
            while (ReaderXml.Read())
            {
                if ((ReaderXml.NodeType == XmlNodeType.Element) && (ReaderXml.Name.ToLower() == "readerwriter"))
                {
                    METS_Section_ReaderWriter_Config singleReaderWriter = read_mets_section_readerwriter_config(ReaderXml.ReadSubtree());
                    ReaderWriters.Add(singleReaderWriter.ID.ToUpper(), singleReaderWriter);
                    Add_METS_Section_ReaderWriter(singleReaderWriter);
                }
            }
        }

        private METS_Section_ReaderWriter_Config read_mets_section_readerwriter_config(XmlReader ReaderXml)
        {
            METS_Section_ReaderWriter_Config returnValue = new METS_Section_ReaderWriter_Config();

            ReaderXml.Read();

            // Move to and save the basic attributes
            if (ReaderXml.MoveToAttribute("ID"))
                returnValue.ID = ReaderXml.Value;
            if (ReaderXml.MoveToAttribute("label"))
                returnValue.Label = ReaderXml.Value;
            if (ReaderXml.MoveToAttribute("namespace"))
                returnValue.Code_Namespace = ReaderXml.Value;
            if (ReaderXml.MoveToAttribute("class"))
                returnValue.Code_Class = ReaderXml.Value;
            if (ReaderXml.MoveToAttribute("assembly"))
                returnValue.Code_Assembly = ReaderXml.Value;
            if ((ReaderXml.MoveToAttribute("isActive")) && (ReaderXml.Value.ToLower() == "false"))
            {
                returnValue.isActive = false;
            }
            if (ReaderXml.MoveToAttribute("section"))
            {
                switch (ReaderXml.Value.ToLower())
                {
                    case "amdsec":
                        returnValue.METS_Section = METS_Section_Type_Enum.AmdSec;
                        if (ReaderXml.MoveToAttribute("amdSecType"))
                        {
                            switch( ReaderXml.Value.ToLower())
                            {
                                case "techmd":
                                    returnValue.AmdSecType = METS_amdSec_Type_Enum.TechMD;
                                    break;

                                case "rightsmd":
                                    returnValue.AmdSecType = METS_amdSec_Type_Enum.RightsMD;
                                    break;

                                case "digiprovmd":
                                    returnValue.AmdSecType = METS_amdSec_Type_Enum.DigiProvMD;
                                    break;

                                case "sourcemd":
                                    returnValue.AmdSecType = METS_amdSec_Type_Enum.SourceMD;
                                    break;

                            }
                        }
                        break;

                    case "dmdsec":
                        returnValue.METS_Section = METS_Section_Type_Enum.DmdSec;
                        break;
                }
            }

            while (ReaderXml.Read())
            {
                if (ReaderXml.NodeType == XmlNodeType.Element)
                {
                    switch (ReaderXml.Name.ToLower())
                    {
                        case "mapping":
                            METS_Section_ReaderWriter_Mapping newMapping = new METS_Section_ReaderWriter_Mapping();
                            if (ReaderXml.MoveToAttribute("mdtype"))
                                newMapping.MD_Type = ReaderXml.Value;
                            if (ReaderXml.MoveToAttribute("othermdtype"))
                                newMapping.Other_MD_Type = ReaderXml.Value;
                            if (ReaderXml.MoveToAttribute("label"))
                                newMapping.Label = ReaderXml.Value;
                            if ((ReaderXml.MoveToAttribute("isDefault")) && (ReaderXml.Value.ToLower() == "true"))
                                newMapping.isDefault = true;
                            returnValue.Add_Mapping(newMapping);
                            break;

                        case "option":
                            string key = String.Empty;
                            string value = String.Empty;
                            if (ReaderXml.MoveToAttribute("key"))
                                key = ReaderXml.Value;
                            if (ReaderXml.MoveToAttribute("value"))
                                value = ReaderXml.Value;
                            if ((key.Length > 0) && (value.Length > 0))
                                returnValue.Add_Option(key, value);
                            break;
                    }
                }
            }

            return returnValue;
        }

        private void read_metadata_modules_config(XmlReader ReaderXml)
        {
            while (ReaderXml.Read())
            {
                if (( ReaderXml.NodeType == XmlNodeType.Element) && ( ReaderXml.Name.ToLower() == "metadatamodule"))
                {
                    // read all the values
                    Additional_Metadata_Module_Config module = new Additional_Metadata_Module_Config();
                    if (ReaderXml.MoveToAttribute("key"))
                        module.Key = ReaderXml.Value.Trim();
                    if (ReaderXml.MoveToAttribute("assembly"))
                        module.Code_Assembly = ReaderXml.Value;
                    if (ReaderXml.MoveToAttribute("namespace"))
                        module.Code_Namespace = ReaderXml.Value;
                    if (ReaderXml.MoveToAttribute("class"))
                        module.Code_Class = ReaderXml.Value;
                    
                    // Only add if valid
                    if ((module.Key.Length > 0) && (module.Code_Class.Length > 0) && (module.Code_Namespace.Length > 0))
                    {
                        Add_Metadata_Module_Config(module);
                    }
                }
            }
        }
        
        private void read_mets_writing_config(XmlReader ReaderXml, Dictionary<string, METS_Section_ReaderWriter_Config> ReaderWriters)
        {
            bool inPackage = false;
            bool inDivision = false;
            bool inFile = false;
            bool inDmdSec = true;
            METS_Writing_Profile profile = null;
            int unnamed_profile_counter = 1;

            while (ReaderXml.Read())
            {
                if (ReaderXml.NodeType == XmlNodeType.Element)
                {
                    switch (ReaderXml.Name.ToLower())
                    {
                        case "profile":
                            profile = new METS_Writing_Profile();
                            if (ReaderXml.MoveToAttribute("name"))
                                profile.Profile_Name = ReaderXml.Value.Trim();
                            if (ReaderXml.MoveToAttribute("description"))
                                profile.Profile_Description = ReaderXml.Value;
                            if (ReaderXml.MoveToAttribute("isDefault"))
                            {
                                bool tempValue;
                                if (bool.TryParse(ReaderXml.Value, out tempValue))
                                {
                                    profile.Default_Profile = tempValue;
                                }
                            }
                            // Enforce a name for this profile (should have one according to XSD)
                            if (profile.Profile_Name.Length == 0)
                            {
                                profile.Profile_Name = "Unnamed" + unnamed_profile_counter;
                                unnamed_profile_counter++;
                            }
                            Add_METS_Writing_Profile(profile);
                            break;

                        case "package_scope":
                            inPackage = true;
                            inDivision = false;
                            inFile = false;
                            break;

                        case "division_scope":
                            inPackage = false;
                            inDivision = true;
                            inFile = false;
                            break;

                        case "file_scope":
                            inPackage = false;
                            inDivision = false;
                            inFile = true;
                            break;

                        case "dmdsec":
                            inDmdSec = true;
                            break;

                        case "amdsec":
                            inDmdSec = false;
                            break;

                        case "readerwriterref":
                            if (ReaderXml.MoveToAttribute("ID"))
                            {
                                string id = ReaderXml.Value.ToUpper();
                                if (( ReaderWriters.ContainsKey(id)) && ( profile != null ))
                                {
                                    METS_Section_ReaderWriter_Config readerWriter = ReaderWriters[id];
                                    if (inPackage)
                                    {
                                        if (inDmdSec)
                                            profile.Add_Package_Level_DmdSec_Writer_Config(readerWriter);
                                        else
                                            profile.Add_Package_Level_AmdSec_Writer_Config(readerWriter);
                                    }
                                    else if ( inDivision )
                                    {
                                        if (inDmdSec)
                                            profile.Add_Division_Level_DmdSec_Writer_Config(readerWriter);
                                        else
                                            profile.Add_Division_Level_AmdSec_Writer_Config(readerWriter);
                                    }
                                    else if (inFile)
                                    {
                                        if (inDmdSec)
                                            profile.Add_File_Level_DmdSec_Writer_Config(readerWriter);
                                        else
                                            profile.Add_File_Level_AmdSec_Writer_Config(readerWriter);
                                    }
                                }
                            }
                            break;
                    }
                }
            }
        }

        #endregion

    }
}