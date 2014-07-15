#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using SobekCM.Resource_Object.METS_Sec_ReaderWriters;

#endregion

namespace SobekCM.Resource_Object.Configuration
{
    /// <summary> Metadata configuration for all the classes used to read/write metadata files
    /// or sections within a METS file </summary>
    public static class Metadata_Configuration
    {
        private static bool attemptedRead;

        private static List<Metadata_File_ReaderWriter_Config> metadataFileReaderWriterConfigs;
        private static List<METS_Section_ReaderWriter_Config> metsSectionReaderWriterConfigs;

        private static Dictionary<string, METS_Writing_Profile> metsWritingProfiles;
        private static METS_Writing_Profile defaultWritingProfile;

        private static List<Additional_Metadata_Module_Config> metadataModules;

        private static Dictionary<Tuple<string, string>, iPackage_amdSec_ReaderWriter> packageAmdSecDictionary;
        private static Dictionary<Tuple<string, string>, iPackage_dmdSec_ReaderWriter> packageDmdSecDictionary;
        private static Dictionary<Tuple<string, string>, iDivision_dmdSec_ReaderWriter> divisionDmdSecDictionary;
        private static Dictionary<Tuple<string, string>, iDivision_amdSec_ReaderWriter> divisionAmdSecDictionary;
        private static Dictionary<Tuple<string, string>, iFile_dmdSec_ReaderWriter> fileDmdSecDictionary;
        private static Dictionary<Tuple<string, string>, iFile_amdSec_ReaderWriter> fileAmdSecDictionary;

        private static List<string> errorsEncountered;

        /// <summary> Static constructor for the Metadata_Configuration class </summary>
        static Metadata_Configuration()
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

            // Set default reader/writer values to have a baseline in case there is
            // no file to be read 
            Set_Default_Values();

        }

        /// <summary> Clear all the current metadata configuration information </summary>
        private static void Clear()
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
        public static bool Attempted_To_Read_Config_File
        {
            get { return attemptedRead;  }
        }

        /// <summary> Default METS writing profile utilized by the METS writer </summary>
        public static METS_Writing_Profile Default_METS_Writing_Profile
        {
            get
            {
                if ( defaultWritingProfile != null )
                    return defaultWritingProfile;
                if (metsWritingProfiles.Count > 0)
                    return metsWritingProfiles[metsWritingProfiles.Keys.FirstOrDefault()];
                return null;
            }
        }

        /// <summary> Adds a new METS writing profile to this metadata configuration </summary>
        /// <param name="NewProfile"> New metadata profile to add </param>
        public static void Add_METS_Writing_Profile( METS_Writing_Profile NewProfile )
        {
            metsWritingProfiles[NewProfile.Profile_Name] = NewProfile;
            if (NewProfile.Default_Profile)
                defaultWritingProfile = NewProfile;
        }

        /// <summary> Add new metadata module configuration information for a module which should always be included </summary>
        /// <param name="MetadatModuleConfig"> New metadata module to include with all new items </param>
        public static void Add_Metadata_Module_Config( Additional_Metadata_Module_Config MetadatModuleConfig )
        {
            metadataModules.Add( MetadatModuleConfig );
        }

        /// <summary> List of metadata modules to be included with all bibliographic items </summary>
        public static ReadOnlyCollection<Additional_Metadata_Module_Config> Metadata_Modules_To_Include
        {
            get
            {
                return new ReadOnlyCollection<Additional_Metadata_Module_Config>(metadataModules);
            }
        }

        /// <summary> Collection of metadata file reader/writer configurations </summary>
        public static ReadOnlyCollection<Metadata_File_ReaderWriter_Config> Metadata_File_ReaderWriter_Configs
        {
            get { return new ReadOnlyCollection<Metadata_File_ReaderWriter_Config>(metadataFileReaderWriterConfigs); }
        }

        /// <summary> Add a new metadata file reader/writer configuration </summary>
        /// <param name="New_ReaderWriter"> New metadata file reader/writer configuration </param>
        public static void Add_Metadata_File_ReaderWriter(Metadata_File_ReaderWriter_Config New_ReaderWriter)
        {
            metadataFileReaderWriterConfigs.Add(New_ReaderWriter);
        }

        /// <summary> Collection of all the METS section reader/writer configurations </summary>
        public static ReadOnlyCollection<METS_Section_ReaderWriter_Config> METS_Section_File_ReaderWriter_Configs
        {
            get { return new ReadOnlyCollection<METS_Section_ReaderWriter_Config>(metsSectionReaderWriterConfigs); }
        }

        /// <summary> Adds a new METS section reader/writer configuration </summary>
        /// <param name="New_ReaderWriter"> New METS section reader/writer</param>
        /// <remarks> This instantiates the actual reader/writer class to determine which interfaces
        /// are implemeneted. </remarks>
        public static void Add_METS_Section_ReaderWriter(METS_Section_ReaderWriter_Config New_ReaderWriter)
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
        /// <param name="MD_Type"> MDTYPE attribute from the METS file section being read </param>
        /// <param name="Other_MD_Type"> OTHERMDTYPE attribute from the METS file section being read </param>
        /// <returns> Reader/writer class ready to read the METS section </returns>
        public static iPackage_amdSec_ReaderWriter Get_Package_AmdSec_ReaderWriter( string MD_Type, string Other_MD_Type )
        {
            Tuple<string, string> thisMappingKey = new Tuple<string, string>(MD_Type.ToUpper(), Other_MD_Type.ToUpper());
            if (packageAmdSecDictionary.ContainsKey(thisMappingKey))
                return packageAmdSecDictionary[thisMappingKey];
            else
                return null;
        }

        /// <summary> Get a package-level dmdSec METS section reader/writer by the METS file MDTYPE and OTHERMDTYPE attributes
        /// present in the METS section tag </summary>
        /// <param name="MD_Type"> MDTYPE attribute from the METS file section being read </param>
        /// <param name="Other_MD_Type"> OTHERMDTYPE attribute from the METS file section being read </param>
        /// <returns> Reader/writer class ready to read the METS section </returns>
        public static iPackage_dmdSec_ReaderWriter Get_Package_DmdSec_ReaderWriter(string MD_Type, string Other_MD_Type)
        {
            Tuple<string, string> thisMappingKey = new Tuple<string, string>(MD_Type.ToUpper(), Other_MD_Type.ToUpper());
            if (packageDmdSecDictionary.ContainsKey(thisMappingKey))
                return packageDmdSecDictionary[thisMappingKey];
            else
                return null;
        }

        /// <summary> Get a division-level dmdSec METS section reader/writer by the METS file MDTYPE and OTHERMDTYPE attributes
        /// present in the METS section tag </summary>
        /// <param name="MD_Type"> MDTYPE attribute from the METS file section being read </param>
        /// <param name="Other_MD_Type"> OTHERMDTYPE attribute from the METS file section being read </param>
        /// <returns> Reader/writer class ready to read the METS section </returns>
        public static iDivision_dmdSec_ReaderWriter Get_Division_DmdSec_ReaderWriter(string MD_Type, string Other_MD_Type)
        {
            Tuple<string, string> thisMappingKey = new Tuple<string, string>(MD_Type.ToUpper(), Other_MD_Type.ToUpper());
            if (divisionDmdSecDictionary.ContainsKey(thisMappingKey))
                return divisionDmdSecDictionary[thisMappingKey];
            else
                return null;
        }

        /// <summary> Get a division-level amdSec METS section reader/writer by the METS file MDTYPE and OTHERMDTYPE attributes
        /// present in the METS section tag </summary>
        /// <param name="MD_Type"> MDTYPE attribute from the METS file section being read </param>
        /// <param name="Other_MD_Type"> OTHERMDTYPE attribute from the METS file section being read </param>
        /// <returns> Reader/writer class ready to read the METS section </returns>
        public static iDivision_amdSec_ReaderWriter Get_Division_AmdSec_ReaderWriter(string MD_Type, string Other_MD_Type)
        {
            Tuple<string, string> thisMappingKey = new Tuple<string, string>(MD_Type.ToUpper(), Other_MD_Type.ToUpper());
            if (divisionAmdSecDictionary.ContainsKey(thisMappingKey))
                return divisionAmdSecDictionary[thisMappingKey];
            else
                return null;
        }

        /// <summary> Get a file-level amdSec METS section reader/writer by the METS file MDTYPE and OTHERMDTYPE attributes
        /// present in the METS section tag </summary>
        /// <param name="MD_Type"> MDTYPE attribute from the METS file section being read </param>
        /// <param name="Other_MD_Type"> OTHERMDTYPE attribute from the METS file section being read </param>
        /// <returns> Reader/writer class ready to read the METS section </returns>
        public static iFile_amdSec_ReaderWriter Get_File_AmdSec_ReaderWriter(string MD_Type, string Other_MD_Type)
        {
            Tuple<string, string> thisMappingKey = new Tuple<string, string>(MD_Type.ToUpper(), Other_MD_Type.ToUpper());
            if (fileAmdSecDictionary.ContainsKey(thisMappingKey))
                return fileAmdSecDictionary[thisMappingKey];
            else
                return null;
        }

        /// <summary> Get a file-level dmdSec METS section reader/writer by the METS file MDTYPE and OTHERMDTYPE attributes
        /// present in the METS section tag </summary>
        /// <param name="MD_Type"> MDTYPE attribute from the METS file section being read </param>
        /// <param name="Other_MD_Type"> OTHERMDTYPE attribute from the METS file section being read </param>
        /// <returns> Reader/writer class ready to read the METS section </returns>
        public static iFile_dmdSec_ReaderWriter Get_File_DmdSec_ReaderWriter(string MD_Type, string Other_MD_Type)
        {
            Tuple<string, string> thisMappingKey = new Tuple<string, string>(MD_Type.ToUpper(), Other_MD_Type.ToUpper());
            if (fileDmdSecDictionary.ContainsKey(thisMappingKey))
                return fileDmdSecDictionary[thisMappingKey];
            else
                return null;
        }

        #region Code to set the default values

        /// <summary> Clears the current metadata configuration and restores all the values
        /// to the default reader/writers and profiles </summary>
        public static void Set_Default_Values()
        {
            Clear();

            // Add the dublin core file reader/writer
            Metadata_File_ReaderWriter_Config DC_File = new Metadata_File_ReaderWriter_Config();
            DC_File.MD_Type = Metadata_File_Type_Enum.DC;
            DC_File.Label = "Dublin Core File";
            DC_File.canRead = true;
            DC_File.canWrite = true;
            DC_File.Code_Assembly = String.Empty;
            DC_File.Code_Namespace = "SobekCM.Resource_Object.Metadata_File_ReaderWriters";
            DC_File.Code_Class = "DC_File_ReaderWriter";
            DC_File.Add_Option("RDF_Style", "false");
            Add_Metadata_File_ReaderWriter(DC_File);

            // Add the dublin core file reader/writer
            Metadata_File_ReaderWriter_Config DC_File2 = new Metadata_File_ReaderWriter_Config();
            DC_File2.MD_Type = Metadata_File_Type_Enum.DC;
            DC_File2.Label = "Dublin Core File (RDF Style)";
            DC_File2.canRead = false;
            DC_File2.canWrite = true;
            DC_File2.Code_Assembly = String.Empty;
            DC_File2.Code_Namespace = "SobekCM.Resource_Object.Metadata_File_ReaderWriters";
            DC_File2.Code_Class = "DC_File_ReaderWriter";
            DC_File2.Add_Option("RDF_Style", "true");
            Add_Metadata_File_ReaderWriter(DC_File2);

            // Add the EAD file reader/writer
            Metadata_File_ReaderWriter_Config EAD_File = new Metadata_File_ReaderWriter_Config();
            EAD_File.MD_Type = Metadata_File_Type_Enum.EAD;
            EAD_File.Label = "Encoded Archival Descriptor (EAD)";
            EAD_File.canRead = true;
            EAD_File.canWrite = false;
            EAD_File.Code_Assembly = String.Empty;
            EAD_File.Code_Namespace = "SobekCM.Resource_Object.Metadata_File_ReaderWriters";
            EAD_File.Code_Class = "EAD_File_ReaderWriter";
            EAD_File.Add_Option("Analyze_Description", "true");
            Add_Metadata_File_ReaderWriter(EAD_File);

            // Add the MARC21 file reader/writer
            Metadata_File_ReaderWriter_Config MARC21_File = new Metadata_File_ReaderWriter_Config();
            MARC21_File.MD_Type = Metadata_File_Type_Enum.MARC21;
            MARC21_File.Label = "MARC21 Single Record File";
            MARC21_File.canRead = true;
            MARC21_File.canWrite = true;
            MARC21_File.Code_Assembly = String.Empty;
            MARC21_File.Code_Namespace = "SobekCM.Resource_Object.Metadata_File_ReaderWriters";
            MARC21_File.Code_Class = "Marc21_File_ReaderWriter";
            Add_Metadata_File_ReaderWriter(MARC21_File);

            // Add the MarcXML file reader/writer
            Metadata_File_ReaderWriter_Config MARCXML_File = new Metadata_File_ReaderWriter_Config();
            MARCXML_File.MD_Type = Metadata_File_Type_Enum.MARCXML;
            MARCXML_File.Label = "MarcXML Single Record File";
            MARCXML_File.canRead = true;
            MARCXML_File.canWrite = true;
            MARCXML_File.Code_Assembly = String.Empty;
            MARCXML_File.Code_Namespace = "SobekCM.Resource_Object.Metadata_File_ReaderWriters";
            MARCXML_File.Code_Class = "MarcXML_File_ReaderWriter";
            Add_Metadata_File_ReaderWriter(MARCXML_File);

            // Add the METS file reader/writer
            Metadata_File_ReaderWriter_Config METS_File = new Metadata_File_ReaderWriter_Config();
            METS_File.MD_Type = Metadata_File_Type_Enum.METS;
            METS_File.Label = "Metadata Encoding and Transmission Standard (METS)";
            METS_File.canRead = true;
            METS_File.canWrite = true;
            METS_File.Code_Assembly = String.Empty;
            METS_File.Code_Namespace = "SobekCM.Resource_Object.Metadata_File_ReaderWriters";
            METS_File.Code_Class = "METS_File_ReaderWriter";
            METS_File.Add_Option("Minimize_File_Info", "false");
            METS_File.Add_Option("Support_Divisional_dmdSec_amdSec", "true");
            Add_Metadata_File_ReaderWriter(METS_File);

            // Add the MODS file reader/writer
            Metadata_File_ReaderWriter_Config MODS_File = new Metadata_File_ReaderWriter_Config();
            MODS_File.MD_Type = Metadata_File_Type_Enum.MODS;
            MODS_File.Label = "Metadata Object Description Standard (MODS)";
            MODS_File.canRead = true;
            MODS_File.canWrite = true;
            MODS_File.Code_Assembly = String.Empty;
            MODS_File.Code_Namespace = "SobekCM.Resource_Object.Metadata_File_ReaderWriters";
            MODS_File.Code_Class = "MODS_File_ReaderWriter";
            Add_Metadata_File_ReaderWriter(MODS_File);

            // Add the INFO file reader/writer
            Metadata_File_ReaderWriter_Config INFO_File = new Metadata_File_ReaderWriter_Config();
            INFO_File.MD_Type = Metadata_File_Type_Enum.OTHER;
            INFO_File.Other_MD_Type = "INFO";
            INFO_File.Label = "Legacy UF INFO Files";
            INFO_File.canRead = true;
            INFO_File.canWrite = false;
            INFO_File.Code_Assembly = String.Empty;
            INFO_File.Code_Namespace = "SobekCM.Resource_Object.Metadata_File_ReaderWriters";
            INFO_File.Code_Class = "INFO_File_ReaderWriter";
            Add_Metadata_File_ReaderWriter(INFO_File);

            // Add the MXF file reader/writer
            Metadata_File_ReaderWriter_Config MXF_File = new Metadata_File_ReaderWriter_Config();
            MXF_File.MD_Type = Metadata_File_Type_Enum.OTHER;
            MXF_File.Other_MD_Type = "MXF";
            MXF_File.Label = "MXF File";
            MXF_File.canRead = true;
            MXF_File.canWrite = false;
            MXF_File.Code_Assembly = String.Empty;
            MXF_File.Code_Namespace = "SobekCM.Resource_Object.Metadata_File_ReaderWriters";
            MXF_File.Code_Class = "MXF_File_ReaderWriter";
            Add_Metadata_File_ReaderWriter(MXF_File);

            // Add the OAI-PMH file reader/writer
            Metadata_File_ReaderWriter_Config OAI_File = new Metadata_File_ReaderWriter_Config();
            OAI_File.MD_Type = Metadata_File_Type_Enum.OTHER;
            OAI_File.Other_MD_Type = "OAI";
            OAI_File.Label = "OAI-PMH File";
            OAI_File.canRead = false;
            OAI_File.canWrite = true;
            OAI_File.Code_Assembly = String.Empty;
            OAI_File.Code_Namespace = "SobekCM.Resource_Object.Metadata_File_ReaderWriters";
            OAI_File.Code_Class = "OAI_File_ReaderWriter";
            Add_Metadata_File_ReaderWriter(OAI_File);


            // Add the MODS section reader/writer
            METS_Section_ReaderWriter_Config MODS_Section = new METS_Section_ReaderWriter_Config();
            MODS_Section.ID = "MODS";
            MODS_Section.Label = "MODS";
            MODS_Section.Code_Assembly = String.Empty;
            MODS_Section.Code_Namespace = "SobekCM.Resource_Object.METS_Sec_ReaderWriters";
            MODS_Section.Code_Class = "MODS_METS_dmdSec_ReaderWriter";
            MODS_Section.isActive = true;
            MODS_Section.METS_Section = METS_Section_Type_Enum.dmdSec;
            MODS_Section.Add_Mapping(new METS_Section_ReaderWriter_Mapping("MODS", "MODS Metadata", true));
            MODS_Section.Add_Mapping(new METS_Section_ReaderWriter_Mapping("Metadata Object Description Standard", false ));
            Add_METS_Section_ReaderWriter(MODS_Section);

            // Add the dublin core section reader/writer
            METS_Section_ReaderWriter_Config DC_Section = new METS_Section_ReaderWriter_Config();
            DC_Section.ID = "DC";
            DC_Section.Label = "Dublin Core";
            DC_Section.Code_Assembly = String.Empty;
            DC_Section.Code_Namespace = "SobekCM.Resource_Object.METS_Sec_ReaderWriters";
            DC_Section.Code_Class = "DC_METS_dmdSec_ReaderWriter";
            DC_Section.isActive = true;
            DC_Section.METS_Section = METS_Section_Type_Enum.dmdSec;
            DC_Section.Add_Mapping(new METS_Section_ReaderWriter_Mapping("DC", "Dublin Core Metadata", true));
            DC_Section.Add_Mapping(new METS_Section_ReaderWriter_Mapping("DUBLIN CORE", false));
            Add_METS_Section_ReaderWriter(DC_Section);

            // Add the MarcXML section reader/writer
            METS_Section_ReaderWriter_Config MarcXML_Section = new METS_Section_ReaderWriter_Config();
            MarcXML_Section.ID = "MarcXML";
            MarcXML_Section.Label = "MarcXML";
            MarcXML_Section.Code_Assembly = String.Empty;
            MarcXML_Section.Code_Namespace = "SobekCM.Resource_Object.METS_Sec_ReaderWriters";
            MarcXML_Section.Code_Class = "MarcXML_METS_dmdSec_ReaderWriter";
            MarcXML_Section.isActive = true;
            MarcXML_Section.METS_Section = METS_Section_Type_Enum.dmdSec;
            MarcXML_Section.Add_Mapping(new METS_Section_ReaderWriter_Mapping("MARCXML", "MarcXML Metadata", true));
            Add_METS_Section_ReaderWriter(MarcXML_Section);

            // Add the DarwinCore section reader/writer
            METS_Section_ReaderWriter_Config Darwin_Section = new METS_Section_ReaderWriter_Config();
            Darwin_Section.ID = "DARWIN";
            Darwin_Section.Label = "DarwinCore";
            Darwin_Section.Code_Assembly = String.Empty;
            Darwin_Section.Code_Namespace = "SobekCM.Resource_Object.METS_Sec_ReaderWriters";
            Darwin_Section.Code_Class = "DarwinCore_METS_dmdSec_ReaderWriter";
            Darwin_Section.isActive = true;
            Darwin_Section.METS_Section = METS_Section_Type_Enum.dmdSec;
            Darwin_Section.Add_Mapping(new METS_Section_ReaderWriter_Mapping("OTHER", "DARWINCORE", "DarwinCore Zoological Taxonomic Information", true));
            Add_METS_Section_ReaderWriter(Darwin_Section);

            // Add the ETD section reader/writer
            METS_Section_ReaderWriter_Config ETD_Section = new METS_Section_ReaderWriter_Config();
            ETD_Section.ID = "ETD";
            ETD_Section.Label = "ETD";
            ETD_Section.Code_Assembly = String.Empty;
            ETD_Section.Code_Namespace = "SobekCM.Resource_Object.METS_Sec_ReaderWriters";
            ETD_Section.Code_Class = "ETD_METS_dmdSec_ReaderWriter";
            ETD_Section.isActive = true;
            ETD_Section.METS_Section = METS_Section_Type_Enum.dmdSec;
            ETD_Section.Add_Mapping(new METS_Section_ReaderWriter_Mapping("OTHER", "PALMM", "PALMM ETD Extension", true));
            ETD_Section.Add_Mapping(new METS_Section_ReaderWriter_Mapping("OTHER", "PALMM Extensions", "PALMM ETD Extension", false));
            Add_METS_Section_ReaderWriter(ETD_Section);

            // Add the SobekCM section reader/writer
            METS_Section_ReaderWriter_Config SobekCM_Section = new METS_Section_ReaderWriter_Config();
            SobekCM_Section.ID = "SOBEK1";
            SobekCM_Section.Label = "SobekCM";
            SobekCM_Section.Code_Assembly = String.Empty;
            SobekCM_Section.Code_Namespace = "SobekCM.Resource_Object.METS_Sec_ReaderWriters";
            SobekCM_Section.Code_Class = "SobekCM_METS_dmdSec_ReaderWriter";
            SobekCM_Section.isActive = true;
            SobekCM_Section.METS_Section = METS_Section_Type_Enum.dmdSec;
            SobekCM_Section.Add_Mapping(new METS_Section_ReaderWriter_Mapping("OTHER", "SOBEKCM", "SobekCM Custom Metadata", true));
            SobekCM_Section.Add_Mapping(new METS_Section_ReaderWriter_Mapping("OTHER", "UFDC", "SobekCM Custom Metadata", false));
            SobekCM_Section.Add_Mapping(new METS_Section_ReaderWriter_Mapping("OTHER", "DLOC", "SobekCM Custom Metadata", false));
            Add_METS_Section_ReaderWriter(SobekCM_Section);

            // Add the SobekCM Map section reader/writer
            METS_Section_ReaderWriter_Config SobekCM_Map_Section = new METS_Section_ReaderWriter_Config();
            SobekCM_Map_Section.ID = "SOBEK2";
            SobekCM_Map_Section.Label = "SobekCM Map";
            SobekCM_Map_Section.Code_Assembly = String.Empty;
            SobekCM_Map_Section.Code_Namespace = "SobekCM.Resource_Object.METS_Sec_ReaderWriters";
            SobekCM_Map_Section.Code_Class = "SobekCM_Map_METS_dmdSec_ReaderWriter";
            SobekCM_Map_Section.isActive = true;
            SobekCM_Map_Section.METS_Section = METS_Section_Type_Enum.dmdSec;
            SobekCM_Map_Section.Add_Mapping(new METS_Section_ReaderWriter_Mapping("OTHER", "SOBEK_MAP", "SobekCM Custom Map Authority Metadata", true));
            SobekCM_Map_Section.Add_Mapping(new METS_Section_ReaderWriter_Mapping("OTHER", "UFDC_MAP", "SobekCM Custom Map Authority Metadata", false));
            Add_METS_Section_ReaderWriter(SobekCM_Map_Section);

            // Add the DAITSS section reader/writer
            METS_Section_ReaderWriter_Config DAITSS_Section = new METS_Section_ReaderWriter_Config();
            DAITSS_Section.ID = "DAITSS";
            DAITSS_Section.Label = "DAITSS";
            DAITSS_Section.Code_Assembly = String.Empty;
            DAITSS_Section.Code_Namespace = "SobekCM.Resource_Object.METS_Sec_ReaderWriters";
            DAITSS_Section.Code_Class = "DAITSS_METS_amdSec_ReaderWriter";
            DAITSS_Section.isActive = true;
            DAITSS_Section.METS_Section = METS_Section_Type_Enum.amdSec;
            DAITSS_Section.amdSecType = METS_amdSec_Type_Enum.digiProvMD;
            DAITSS_Section.Add_Mapping(new METS_Section_ReaderWriter_Mapping("OTHER", "DAITSS", "DAITSS Archiving Information", true));
            Add_METS_Section_ReaderWriter(DAITSS_Section);

            // Add the RightsMD section reader/writer
            METS_Section_ReaderWriter_Config Rights_Section = new METS_Section_ReaderWriter_Config();
            Rights_Section.ID = "RIGHTS";
            Rights_Section.Label = "RightsMD";
            Rights_Section.Code_Assembly = String.Empty;
            Rights_Section.Code_Namespace = "SobekCM.Resource_Object.METS_Sec_ReaderWriters";
            Rights_Section.Code_Class = "RightsMD_METS_amdSec_ReaderWriter";
            Rights_Section.isActive = true;
            Rights_Section.METS_Section = METS_Section_Type_Enum.amdSec;
            Rights_Section.amdSecType = METS_amdSec_Type_Enum.rightsMD;
            Rights_Section.Add_Mapping(new METS_Section_ReaderWriter_Mapping("OTHER", "RIGHTSMD", "Rights Information", true));
            Add_METS_Section_ReaderWriter(Rights_Section);

            // Add the SobekCM fileinfo section reader/writer
            METS_Section_ReaderWriter_Config SobekCM_File_Section = new METS_Section_ReaderWriter_Config();
            SobekCM_File_Section.ID = "SOBEK3";
            SobekCM_File_Section.Label = "SobekCM FileInfo";
            SobekCM_File_Section.Code_Assembly = String.Empty;
            SobekCM_File_Section.Code_Namespace = "SobekCM.Resource_Object.METS_Sec_ReaderWriters";
            SobekCM_File_Section.Code_Class = "SobekCM_FileInfo_METS_amdSec_ReaderWriter";
            SobekCM_File_Section.isActive = true;
            SobekCM_File_Section.METS_Section = METS_Section_Type_Enum.amdSec;
            SobekCM_File_Section.amdSecType = METS_amdSec_Type_Enum.techMD;
            SobekCM_File_Section.Add_Mapping(new METS_Section_ReaderWriter_Mapping("OTHER", "SOBEKCM", "SobekCM File Technical Details", true));
            Add_METS_Section_ReaderWriter(SobekCM_File_Section);

            // Add the GML section reader/writer
            METS_Section_ReaderWriter_Config GML_Section = new METS_Section_ReaderWriter_Config();
            GML_Section.ID = "GML";
            GML_Section.Label = "GML Coordinate";
            GML_Section.Code_Assembly = String.Empty;
            GML_Section.Code_Namespace = "SobekCM.Resource_Object.METS_Sec_ReaderWriters";
            GML_Section.Code_Class = "GML_METS_dmdSec_ReaderWriter";
            GML_Section.isActive = true;
            GML_Section.METS_Section = METS_Section_Type_Enum.dmdSec;
            GML_Section.Add_Mapping(new METS_Section_ReaderWriter_Mapping("OTHER", "GML", "Geographic Markup Language", true));
            Add_METS_Section_ReaderWriter(GML_Section);

            // Add the IEEE-LOM section reader/writer
            METS_Section_ReaderWriter_Config LOM_Section = new METS_Section_ReaderWriter_Config();
            LOM_Section.ID = "IEEE-LOM";
            LOM_Section.Label = "IEEE-LOM: Learning Object Metadata";
            LOM_Section.Code_Assembly = String.Empty;
            LOM_Section.Code_Namespace = "SobekCM.Resource_Object.METS_Sec_ReaderWriters";
            LOM_Section.Code_Class = "LOM_IEEE_METS_dmdSec_ReaderWriter";
            LOM_Section.isActive = true;
            LOM_Section.METS_Section = METS_Section_Type_Enum.dmdSec;
            LOM_Section.Add_Mapping(new METS_Section_ReaderWriter_Mapping("OTHER", "IEEE-LOM", "Learning Object Metadata", true));
            Add_METS_Section_ReaderWriter(LOM_Section);

            // Add the VRACore section reader/writer
            METS_Section_ReaderWriter_Config VRA_Section = new METS_Section_ReaderWriter_Config();
            VRA_Section.ID = "VRACORE";
            VRA_Section.Label = "VRACore Visual Resource Metadata";
            VRA_Section.Code_Assembly = String.Empty;
            VRA_Section.Code_Namespace = "SobekCM.Resource_Object.METS_Sec_ReaderWriters";
            VRA_Section.Code_Class = "VRACore_METS_dmdSec_ReaderWriter";
            VRA_Section.isActive = true;
            VRA_Section.METS_Section = METS_Section_Type_Enum.dmdSec;
            VRA_Section.Add_Mapping(new METS_Section_ReaderWriter_Mapping("OTHER", "VRACore", "VRACore Visual Resource Metadata", true));
            Add_METS_Section_ReaderWriter(VRA_Section);

            // Add the default METS writig profile
            METS_Writing_Profile defaultProfile = new METS_Writing_Profile();
            defaultProfile.Default_Profile = true;
            defaultProfile.Profile_Name = "Complete MODS Writer";
            defaultProfile.Profile_Description = "This profile includes almost all of the possible sub-writers but the main bibliographic data is stored in MODS";
            defaultProfile.Add_Package_Level_DmdSec_Writer_Config(MODS_Section);
            defaultProfile.Add_Package_Level_DmdSec_Writer_Config(SobekCM_Section);
            defaultProfile.Add_Package_Level_DmdSec_Writer_Config(SobekCM_Map_Section);
            defaultProfile.Add_Package_Level_DmdSec_Writer_Config(Darwin_Section);
            defaultProfile.Add_Package_Level_DmdSec_Writer_Config(ETD_Section);
            defaultProfile.Add_Package_Level_DmdSec_Writer_Config(GML_Section);
            defaultProfile.Add_Package_Level_DmdSec_Writer_Config(LOM_Section);
            defaultProfile.Add_Package_Level_DmdSec_Writer_Config(VRA_Section);
            defaultProfile.Add_Package_Level_AmdSec_Writer_Config(DAITSS_Section);
            defaultProfile.Add_Package_Level_AmdSec_Writer_Config(Rights_Section);
            defaultProfile.Add_Package_Level_AmdSec_Writer_Config(SobekCM_File_Section);
         //   defaultProfile.Add_Division_Level_DmdSec_Writer_Config(MODS_Section);
            defaultProfile.Add_Division_Level_DmdSec_Writer_Config(GML_Section);
            Add_METS_Writing_Profile(defaultProfile);

            // Add the default METS writig profile
            METS_Writing_Profile dcProfile = new METS_Writing_Profile();
            dcProfile.Default_Profile = false;
            dcProfile.Profile_Name = "Simple Dublin Core Writer";
            dcProfile.Profile_Description = "This is a simplified profile which uses Dublin Core to describe all levels of the METS";
            dcProfile.Add_Package_Level_DmdSec_Writer_Config(DC_Section);
            dcProfile.Add_Package_Level_AmdSec_Writer_Config(DAITSS_Section);
            dcProfile.Add_Package_Level_AmdSec_Writer_Config(Rights_Section);
            dcProfile.Add_Package_Level_AmdSec_Writer_Config(SobekCM_File_Section);
            dcProfile.Add_Division_Level_DmdSec_Writer_Config(DC_Section);
            dcProfile.Add_File_Level_DmdSec_Writer_Config(DC_Section);
            Add_METS_Writing_Profile(dcProfile);


        }

        #endregion

        #region Code to save this metadata configuration to a XML file

        /// <summary> Save this metadata configuration to a XML config file </summary>
        /// <param name="FilePath"> File/path for the resulting XML config file </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public static bool Save_To_Config_File(string FilePath)
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

                    if (fileConfig.canRead)
                        writer.Write("canRead=\"true\" ");
                    else
                        writer.Write("canRead=\"false\" ");
                    if (fileConfig.canWrite)
                        writer.Write("canWrite=\"true\" ");
                    else
                        writer.Write("canWrite=\"false\" ");
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

                    if (fileConfig.isActive)
                        writer.Write("isActive=\"true\" ");
                    else
                        writer.Write("isActive=\"false\" ");

                    switch (fileConfig.METS_Section)
                    {
                        case METS_Section_Type_Enum.dmdSec:
                            writer.Write("section=\"dmdSec\" ");
                            break;

                        case METS_Section_Type_Enum.amdSec:
                            writer.Write("section=\"amdSec\" ");
                            switch (fileConfig.amdSecType)
                            {
                                case METS_amdSec_Type_Enum.digiProvMD:
                                    writer.Write("amdSecType=\"digiProvMD\" ");
                                    break;

                                case METS_amdSec_Type_Enum.rightsMD:
                                    writer.Write("amdSecType=\"rightsMD\" ");
                                    break;

                                case METS_amdSec_Type_Enum.sourceMD:
                                    writer.Write("amdSecType=\"sourceMD\" ");
                                    break;

                                case METS_amdSec_Type_Enum.techMD:
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
                        foreach (string thisKey in fileConfig.Options.Keys)
                        {
                            writer.WriteLine("\t\t\t\t\t<Option key=\"" + Convert_String_To_XML_Safe(thisKey) + "\" value=\"" + Convert_String_To_XML_Safe(fileConfig.Options[thisKey]) + "\" />");
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


                    ReadOnlyCollection<METS_Section_ReaderWriter_Config> divAmdSec = profile.Division_Level_AmdSec_Writer_Configs;
                    ReadOnlyCollection<METS_Section_ReaderWriter_Config> divDmdSec = profile.Division_Level_DmdSec_Writer_Configs;
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

                    ReadOnlyCollection<METS_Section_ReaderWriter_Config> fileAmdSec = profile.File_Level_AmdSec_Writer_Configs;
                    ReadOnlyCollection<METS_Section_ReaderWriter_Config> fileDmdSec = profile.File_Level_DmdSec_Writer_Configs;
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
        /// <param name="element"> Element data to convert </param>
        /// <returns> Data converted into an XML-safe string</returns>
        private static string Convert_String_To_XML_Safe(string element)
        {
            if (element == null)
                return string.Empty;

            string xml_safe = element;
            int i = xml_safe.IndexOf("&");
            while (i >= 0)
            {
                if ((i != xml_safe.IndexOf("&amp;", i)) && (i != xml_safe.IndexOf("&quot;", i)) &&
                    (i != xml_safe.IndexOf("&gt;", i)) && (i != xml_safe.IndexOf("&lt;", i)))
                {
                    xml_safe = xml_safe.Substring(0, i + 1) + "amp;" + xml_safe.Substring(i + 1);
                }

                i = xml_safe.IndexOf("&", i + 1);
            }
            return xml_safe.Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
        }

        #endregion

        #region Code for reading the configuration from a XML file
        
        /// <summary> Read the metadata configuration from a correctly-formatted metadata configuration XML file </summary>
        /// <param name="Configuration_XML_File"> File/path for the metadata configuration XML file to read </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public static bool Read_Metadata_Configuration(string Configuration_XML_File)
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
                                read_metadata_modules_config(readerXml.ReadSubtree(), readerWriters);
                                break;
                        }
                    }
                }
            }
            catch (Exception ee)
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

        private static void read_metadata_file_readerwriter_configs(XmlReader readerXml)
        {
            while (readerXml.Read())
            {
                if ((readerXml.NodeType == XmlNodeType.Element) && (readerXml.Name.ToLower() == "readerwriter"))
                {
                    read_metadata_file_readerwriter_config(readerXml.ReadSubtree());
                }
            }
        }

        private static void read_metadata_file_readerwriter_config(XmlReader readerXml)
        {
            Metadata_File_ReaderWriter_Config returnValue = new Metadata_File_ReaderWriter_Config();
            readerXml.Read();

            // Move to and save the basic attributes
            if (readerXml.MoveToAttribute("mdtype"))
            {
                switch (readerXml.Value.ToUpper())
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
                        if (readerXml.MoveToAttribute("othermdtype"))
                            returnValue.Other_MD_Type = readerXml.Value;
                        break;
                }
            }

            if (readerXml.MoveToAttribute("label"))
                returnValue.Label = readerXml.Value;
            if (readerXml.MoveToAttribute("namespace"))
                returnValue.Code_Namespace = readerXml.Value;
            if (readerXml.MoveToAttribute("class"))
                returnValue.Code_Class = readerXml.Value;
            if (readerXml.MoveToAttribute("assembly"))
                returnValue.Code_Assembly = readerXml.Value;
            if ((readerXml.MoveToAttribute("canRead")) && (readerXml.Value.ToLower() == "false"))
            {
                returnValue.canRead = false;
            }
            if ((readerXml.MoveToAttribute("canWrite")) && (readerXml.Value.ToLower() == "false"))
            {
                returnValue.canWrite = false;
            }

            while (readerXml.Read())
            {
                if ((readerXml.NodeType == XmlNodeType.Element) && (readerXml.Name.ToLower() == "option"))
                {
                    string key = String.Empty;
                    string value = String.Empty;
                    if (readerXml.MoveToAttribute("key"))
                        key = readerXml.Value;
                    if (readerXml.MoveToAttribute("value"))
                        value = readerXml.Value;
                    if ((key.Length > 0) && (value.Length > 0))
                        returnValue.Add_Option(key, value);
                }
            }

            Metadata_Configuration.Add_Metadata_File_ReaderWriter(returnValue);
        }

        private static void read_mets_readerwriter_configs(XmlReader readerXml, Dictionary<string, METS_Section_ReaderWriter_Config> readerWriters)
        {
            while (readerXml.Read())
            {
                if ((readerXml.NodeType == XmlNodeType.Element) && (readerXml.Name.ToLower() == "readerwriter"))
                {
                    METS_Section_ReaderWriter_Config singleReaderWriter = read_mets_section_readerwriter_config(readerXml.ReadSubtree());
                    readerWriters.Add(singleReaderWriter.ID.ToUpper(), singleReaderWriter);
                    Metadata_Configuration.Add_METS_Section_ReaderWriter(singleReaderWriter);
                }
            }
        }

        private static METS_Section_ReaderWriter_Config read_mets_section_readerwriter_config(XmlReader readerXml)
        {
            METS_Section_ReaderWriter_Config returnValue = new METS_Section_ReaderWriter_Config();

            readerXml.Read();

            // Move to and save the basic attributes
            if (readerXml.MoveToAttribute("ID"))
                returnValue.ID = readerXml.Value;
            if (readerXml.MoveToAttribute("label"))
                returnValue.Label = readerXml.Value;
            if (readerXml.MoveToAttribute("namespace"))
                returnValue.Code_Namespace = readerXml.Value;
            if (readerXml.MoveToAttribute("class"))
                returnValue.Code_Class = readerXml.Value;
            if (readerXml.MoveToAttribute("assembly"))
                returnValue.Code_Assembly = readerXml.Value;
            if ((readerXml.MoveToAttribute("isActive")) && (readerXml.Value.ToLower() == "false"))
            {
                returnValue.isActive = false;
            }
            if (readerXml.MoveToAttribute("section"))
            {
                switch (readerXml.Value.ToLower())
                {
                    case "amdsec":
                        returnValue.METS_Section = METS_Section_Type_Enum.amdSec;
                        if (readerXml.MoveToAttribute("amdSecType"))
                        {
                            switch( readerXml.Value.ToLower())
                            {
                                case "techmd":
                                    returnValue.amdSecType = METS_amdSec_Type_Enum.techMD;
                                    break;

                                case "rightsmd":
                                    returnValue.amdSecType = METS_amdSec_Type_Enum.rightsMD;
                                    break;

                                case "digiprovmd":
                                    returnValue.amdSecType = METS_amdSec_Type_Enum.digiProvMD;
                                    break;

                                case "sourcemd":
                                    returnValue.amdSecType = METS_amdSec_Type_Enum.sourceMD;
                                    break;

                            }
                        }
                        break;

                    case "dmdsec":
                        returnValue.METS_Section = METS_Section_Type_Enum.dmdSec;
                        break;
                }
            }

            while (readerXml.Read())
            {
                if (readerXml.NodeType == XmlNodeType.Element)
                {
                    switch (readerXml.Name.ToLower())
                    {
                        case "mapping":
                            METS_Section_ReaderWriter_Mapping newMapping = new METS_Section_ReaderWriter_Mapping();
                            if (readerXml.MoveToAttribute("mdtype"))
                                newMapping.MD_Type = readerXml.Value;
                            if (readerXml.MoveToAttribute("othermdtype"))
                                newMapping.Other_MD_Type = readerXml.Value;
                            if (readerXml.MoveToAttribute("label"))
                                newMapping.Label = readerXml.Value;
                            if ((readerXml.MoveToAttribute("isDefault")) && (readerXml.Value.ToLower() == "true"))
                                newMapping.isDefault = true;
                            returnValue.Add_Mapping(newMapping);
                            break;

                        case "option":
                            string key = String.Empty;
                            string value = String.Empty;
                            if (readerXml.MoveToAttribute("key"))
                                key = readerXml.Value;
                            if (readerXml.MoveToAttribute("value"))
                                value = readerXml.Value;
                            if ((key.Length > 0) && (value.Length > 0))
                                returnValue.Add_Option(key, value);
                            break;
                    }
                }
            }

            return returnValue;
        }

        private static void read_metadata_modules_config(XmlReader readerXml, Dictionary<string, METS_Section_ReaderWriter_Config> readerWriters)
        {
            bool inPackage = false;
            bool inDivision = false;
            bool inFile = false;
            bool inDmdSec = true;
            METS_Writing_Profile profile = null;
            int unnamed_profile_counter = 1;

            while (readerXml.Read())
            {
                if (( readerXml.NodeType == XmlNodeType.Element) && ( readerXml.Name.ToLower() == "metadatamodule"))
                {
                    // read all the values
                    Additional_Metadata_Module_Config module = new Additional_Metadata_Module_Config();
                    if (readerXml.MoveToAttribute("key"))
                        module.Key = readerXml.Value.Trim();
                    if (readerXml.MoveToAttribute("assembly"))
                        module.Code_Assembly = readerXml.Value;
                    if (readerXml.MoveToAttribute("namespace"))
                        module.Code_Namespace = readerXml.Value;
                    if (readerXml.MoveToAttribute("class"))
                        module.Code_Class = readerXml.Value;
                    
                    // Only add if valid
                    if ((module.Key.Length > 0) && (module.Code_Class.Length > 0) && (module.Code_Namespace.Length > 0))
                    {
                        Add_Metadata_Module_Config(module);
                    }
                }
            }
        }
        
        private static void read_mets_writing_config(XmlReader readerXml, Dictionary<string, METS_Section_ReaderWriter_Config> readerWriters)
        {
            bool inPackage = false;
            bool inDivision = false;
            bool inFile = false;
            bool inDmdSec = true;
            METS_Writing_Profile profile = null;
            int unnamed_profile_counter = 1;

            while (readerXml.Read())
            {
                if (readerXml.NodeType == XmlNodeType.Element)
                {
                    switch (readerXml.Name.ToLower())
                    {
                        case "profile":
                            profile = new METS_Writing_Profile();
                            if (readerXml.MoveToAttribute("name"))
                                profile.Profile_Name = readerXml.Value.Trim();
                            if (readerXml.MoveToAttribute("description"))
                                profile.Profile_Description = readerXml.Value;
                            if (readerXml.MoveToAttribute("isDefault"))
                            {
                                bool tempValue;
                                if (bool.TryParse(readerXml.Value, out tempValue))
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
                            if (readerXml.MoveToAttribute("ID"))
                            {
                                string id = readerXml.Value.ToUpper();
                                if (( readerWriters.ContainsKey(id)) && ( profile != null ))
                                {
                                    METS_Section_ReaderWriter_Config readerWriter = readerWriters[id];
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