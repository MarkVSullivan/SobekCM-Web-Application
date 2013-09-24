#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using SobekCM.Library.Builder;
using SobekCM.Library.Configuration;
using SobekCM.Library.Database;
using SobekCM.Library.Search;

#endregion

namespace SobekCM.Library.Settings
{
    /// <summary> Class provides static context to constant settings based on the basic information about this instance of the application and server information </summary>
    public class SobekCM_Library_Settings
    {

        /// <summary> Name for the backup files folder within each digital resource </summary>
        public const string Backup_Files_Folder_Name = "sobek_files";

        /// <summary> Current version number associated with this SobekCM digital repository web application </summary>
        public const string CURRENT_WEB_VERSION = "3.20 BETA";

        /// <summary> Current version number associated with this SobekCM builder application </summary>
        public const string CURRENT_BUILDER_VERSION = "3.20 BETA";

        /// <summary> Number of ticks that a complete package must age before being processed </summary>
        /// <value> This is currently set to 15 minutes (in ticks) </value>
        public static long Complete_Package_Required_Aging = 15L * 60L * 1000000L;     // 15 minutes (in ticks)

        /// <summary> Number of ticks that a metadata only package must age before being processed </summary>
        /// <value> This is currently set to 1 minute (in ticks) </value>
        public static long METS_Only_Package_Required_Aging = 60L * 10000000L;          // 1 Minute (in ticks)

		/// <summary> Flag indicates whether checksums should be verified </summary>
		public static readonly bool Verify_CheckSum = true;

        // Values pulled from database 
        private static string packageArchivalFolder, logFilesDirectory, logFilesUrl, staticPagesLocation;
        private static string imageServerNetwork, applicationServerNetwork, applicationServerUrl, marcxmlFeedLocation;
        private static string fdaReportDropbox, archiveDropbox;
        private static string metadataHelpUrl, helpUrl;
        private static string documentSolrUrl, pageSolrUrl;
        private static bool buildMarcFeedByDefault;
        private static readonly List<Builder_Source_Folder> incomingFolders;
        private static int jpegWidth, jpegHeight;
        private static int thumbnailWidth, thumbnailHeight;
        private static string filesToExcludeFromDownloads;
        private static string prearchiveFilesToDelete;
        private static string postarchiveFilesToDelete;
        private static string jpeg2000Server;
        private static string systemErrorEmail, systemEmail;
        private static string shibbolethSystemUrl, shibbolethSystemName;
        private static string systemBaseUrl;
        private static string systemBaseName;
        private static string systemBaseAbbreviation;
        private static string oaiResourceIdentifierBase;
        private static string oaiRepositoryIdentifier;
        private static string oaiRepositoryName;
        private static string ocrCommandPrompt;
        private static string imageMagickExecutable;
        private static string ghostscriptExecutable;
        private static bool statisticsCachingEnabled;
        private static bool includeTreeviewOnSystemHome;
        private static bool includePartnersOnSystemHome;
        private static bool convertOfficeFilesToPdf;
        private static bool builderVerbose;
        private static string privacyEmailAddress;
        private static int builderSecondsBetweenPolls;
        private static int builderLogExpirationDays;
        private static int webOutputCachingMinutes;
        private static string mainBuilderInputFolder;
        private static string webServerIp;
        private static string uploadFileTypes;
        private static string uploadImageTypes;

        private static readonly string base_url;

        private static string baseDirectory,
            imageServerUrl, inProcessSubmissionLocation,
            sobekcmImageserver, cachingServer;

        private static bool canSubmit;
        private static bool showFloridaSusSettings;

        private static readonly Dictionary<int, KeyValuePair<int, string>> dispositionFutureTypes;
        private static readonly Dictionary<int, KeyValuePair<int, string>> dispositionPastTypes;
        private static readonly Dictionary<int, KeyValuePair<int, string>> workflowTypes;



        private static readonly List<Metadata_Search_Field> metadataFields;
        private static readonly Dictionary<string, Metadata_Search_Field> metadataFieldsByCode;
        private static readonly Dictionary<short, Metadata_Search_Field> metadataFieldsByID;
        private static readonly Dictionary<string, Metadata_Search_Field> metadataFieldsByFacetName;
		private static readonly Dictionary<string, Metadata_Search_Field> metadataFieldsByName;

        private static string mangoUnionSearchBaseUrl; 
        private static string mangoUnionSearchText;
        private static List<string> searchStopWords;
        private static readonly Object thisLock = new Object();


	    private static List<Database_Instance_Configuration> databaseInfo; 

        private static Dictionary<string, string> additionalGlobalSettings;

        /// <summary> Static constructor sets all the values to default empty strings </summary>
        static SobekCM_Library_Settings()
        {
            try
            {
                // Set some default values
				databaseInfo = new List<Database_Instance_Configuration>();
                
                Base_SobekCM_Location_Relative = String.Empty;
                jpeg2000Server = String.Empty;
                base_url = String.Empty;
                baseDirectory = String.Empty;
                imageServerUrl = String.Empty;
                inProcessSubmissionLocation = String.Empty;
                sobekcmImageserver = String.Empty;
                canSubmit = false;
                cachingServer = String.Empty;
                privacyEmailAddress = String.Empty;
                metadataHelpUrl = String.Empty;
                helpUrl = String.Empty;
                includePartnersOnSystemHome = false;
                includeTreeviewOnSystemHome = false;
                Default_UI_Language = Web_Language_Enum.English;
                webOutputCachingMinutes = 0;
                builderVerbose = false;
                uploadFileTypes = String.Empty;
                uploadImageTypes = String.Empty;

                // Define new empty collections
                dispositionFutureTypes = new Dictionary<int, KeyValuePair<int, string>>();
                dispositionPastTypes = new Dictionary<int, KeyValuePair<int, string>>();
                workflowTypes = new Dictionary<int, KeyValuePair<int, string>>();
                metadataFields = new List<Metadata_Search_Field>();
                metadataFieldsByCode = new Dictionary<string, Metadata_Search_Field>();
                metadataFieldsByID = new Dictionary<short, Metadata_Search_Field>();
                metadataFieldsByFacetName = new Dictionary<string, Metadata_Search_Field>();
				metadataFieldsByName = new Dictionary<string, Metadata_Search_Field>();
                incomingFolders = new List<Builder_Source_Folder>();
                searchStopWords = new List<string>();
                additionalGlobalSettings = new Dictionary<string, string>();

                // Should we read the configuration file?
                if (String.IsNullOrEmpty(SobekCM_Database.Connection_String))
                {
                    Read_Configuration_File();
	                SobekCM_Database.Connection_String = databaseInfo[0].Connection_String;
                }
                
                Refresh(SobekCM_Database.Get_Settings_Complete(null));
            }
            catch (Exception)
            {
                // Do nothing here
            }
        }

		/// <summary> Reads the inficated configuration file </summary>
		/// <exception>File is checked for existence first, otherwise all encountered exceptions will be thrown</exception>
        public static void Read_Configuration_File()
        {
            baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            Read_Configuration_File(baseDirectory + "\\config\\sobekcm.config");
        }

		/// <summary> Reads the inficated configuration file </summary>
		/// <param name="ConfigFile"> Configuration file to read </param>
		/// <exception>File is checked for existence first, otherwise all encountered exceptions will be thrown</exception>
        public static void Read_Configuration_File( string ConfigFile )
        {
            if (!File.Exists(ConfigFile))
                return;

			databaseInfo.Clear();

            StreamReader reader = new StreamReader(ConfigFile);
            System.Xml.XmlTextReader xmlReader = new System.Xml.XmlTextReader(reader);
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == System.Xml.XmlNodeType.Element)
                {
                    string node_name = xmlReader.Name.ToLower();
                    switch (node_name)
                    {
                        case "connection_string":
							Database_Instance_Configuration newDb = new Database_Instance_Configuration();
                            if (xmlReader.MoveToAttribute("type"))
                            {
                                if (xmlReader.Value.ToLower() == "postgresql")
                                    newDb.Database_Type = SobekCM_Database_Type_Enum.PostgreSQL;

                            }
                            xmlReader.Read();
                            newDb.Connection_String = xmlReader.Value;
							databaseInfo.Add(newDb);
                            break;

                        case "error_emails":
                            xmlReader.Read();
                            systemErrorEmail = xmlReader.Value;
                            break;

                        case "error_page":
                            xmlReader.Read();
                            System_Error_URL = xmlReader.Value;
                            break;

                        case "ghostscript_executable":
                            xmlReader.Read();
                            ghostscriptExecutable = xmlReader.Value;
                            break;

                        case "imagemagick_executable":
                            xmlReader.Read();
                            imageMagickExecutable = xmlReader.Value;
                            break;
                    }
                }
            }

            xmlReader.Close();
            reader.Close();
        }

        #region Methods to load data from the dataset

        /// <summary> Refreshes the values from the database settings </summary>
        /// <param name="SobekCM_Settings"> DataSet with all of the current settings for configuration </param>
        /// <returns> TRUE if no error is encountered, otherwise FALSE </returns>
        public static bool Refresh(DataSet SobekCM_Settings )
        {
            try
            {
                bool error = false;

                // Clear the existing server information
                incomingFolders.Clear();
                if (SobekCM_Settings.Tables.Count > 1)
                {
                    foreach (DataRow thisRow in SobekCM_Settings.Tables[1].Rows)
                    {
                        incomingFolders.Add(new Builder_Source_Folder(thisRow));
                    }
                }
                
                // Get the settings table
                DataTable settingsTable = SobekCM_Settings.Tables[0];

                // Create the dictionary for quick lookups for the next work
                Dictionary<string, string> settingsDictionary = new Dictionary<string, string>();
                foreach (DataRow thisRow in settingsTable.Rows)
                {
                    settingsDictionary[thisRow["Setting_Key"].ToString()] = thisRow["Setting_Value"].ToString().Trim();
                }

                // Pull all of the builder settings value ( from UFDC_Builder_Settings )
                Get_String_Value(settingsDictionary, "Application Server Network", ref applicationServerNetwork, ref error);
                Get_String_Value(settingsDictionary, "Application Server URL", ref applicationServerUrl, ref error);
                Get_String_Value(settingsDictionary, "Archive DropBox", ref archiveDropbox, ref error);
                Get_Integer_Value(settingsDictionary, "Builder Log Expiration in Days", ref builderLogExpirationDays, ref error, 10 );
                Get_Integer_Value(settingsDictionary, "Builder Seconds Between Polls", ref builderSecondsBetweenPolls, ref error, 60);
                Get_Boolean_Value(settingsDictionary, "Builder Verbose Flag", ref builderVerbose, ref error, false);
                Get_String_Value(settingsDictionary, "Caching Server", ref cachingServer, ref error);
                Get_Boolean_Value(settingsDictionary, "Can Submit Edit Online", ref canSubmit, ref error, false);
                Get_Boolean_Value(settingsDictionary, "Convert Office Files to PDF", ref convertOfficeFilesToPdf, ref error, false);                
                Get_Boolean_Value(settingsDictionary, "Create MARC Feed By Default", ref buildMarcFeedByDefault, ref error, false);
                Get_String_Value(settingsDictionary, "Document Solr Index URL", ref documentSolrUrl, ref error);
                Get_String_Value(settingsDictionary, "FDA Report DropBox", ref fdaReportDropbox, ref error);
                Get_String_Value(settingsDictionary, "Files To Exclude From Downloads", ref filesToExcludeFromDownloads, ref error);
                Get_String_Value(settingsDictionary, "Help URL", ref helpUrl, "http://ufdc.ufl.edu/");
                Get_String_Value(settingsDictionary, "Help Metadata URL", ref metadataHelpUrl, "http://ufdc.ufl.edu/");
                Get_String_Value(settingsDictionary, "Image Server Network", ref imageServerNetwork, ref error);
                Get_String_Value(settingsDictionary, "Image Server URL", ref imageServerUrl, ref error);
                Get_Boolean_Value(settingsDictionary, "Include TreeView On System Home", ref includeTreeviewOnSystemHome, ref error, false);
                Get_Boolean_Value(settingsDictionary, "Include Partners On System Home", ref includePartnersOnSystemHome, ref error, false);               
                Get_Integer_Value(settingsDictionary, "JPEG Height", ref jpegHeight, ref error, -1);
                Get_Integer_Value(settingsDictionary, "JPEG Width", ref jpegWidth, ref error, -1);
                Get_String_Value(settingsDictionary, "JPEG2000 Server", ref jpeg2000Server, ref error);
                Get_String_Value(settingsDictionary, "Log Files Directory", ref logFilesDirectory, ref error);
                Get_String_Value(settingsDictionary, "Log Files URL", ref logFilesUrl, ref error);
                Get_String_Value(settingsDictionary, "Main Builder Input Folder", ref mainBuilderInputFolder, String.Empty);
                Get_String_Value(settingsDictionary, "Mango Union Search Base URL", ref mangoUnionSearchBaseUrl, ref error);
                Get_String_Value(settingsDictionary, "Mango Union Search Text", ref mangoUnionSearchText, ref error);
                Get_String_Value(settingsDictionary, "MarcXML Feed Location", ref marcxmlFeedLocation, ref error);
                Get_String_Value(settingsDictionary, "OAI Resource Identifier Base", ref oaiResourceIdentifierBase, ref error);
                Get_String_Value(settingsDictionary, "OAI Repository Identifier", ref oaiRepositoryIdentifier, ref error);
                Get_String_Value(settingsDictionary, "OAI Repository Name", ref oaiRepositoryName, ref error);
                Get_String_Value(settingsDictionary, "OCR Command Prompt", ref ocrCommandPrompt, String.Empty);
                Get_String_Value(settingsDictionary, "Package Archival Folder", ref packageArchivalFolder, String.Empty);
                Get_String_Value(settingsDictionary, "Page Solr Index URL", ref pageSolrUrl, String.Empty);
                Get_String_Value(settingsDictionary, "PostArchive Files To Delete", ref postarchiveFilesToDelete, String.Empty);
                Get_String_Value(settingsDictionary, "PreArchive Files To Delete", ref prearchiveFilesToDelete, String.Empty);
                Get_String_Value(settingsDictionary, "Privacy Email Address", ref privacyEmailAddress, String.Empty);
                Get_String_Value(settingsDictionary, "Shibboleth System URL", ref shibbolethSystemUrl, String.Empty);
                Get_String_Value(settingsDictionary, "Shibboleth System Name", ref shibbolethSystemName, String.Empty);
                Get_Boolean_Value(settingsDictionary, "Show Florida SUS Settings", ref showFloridaSusSettings, ref error, false);
                Get_String_Value(settingsDictionary, "SobekCM Image Server", ref sobekcmImageserver, String.Empty);
                Get_String_Value(settingsDictionary, "SobekCM Web Server IP", ref webServerIp, String.Empty);
                Get_String_Value(settingsDictionary, "Static Pages Location", ref staticPagesLocation, ref error);
                Get_Boolean_Value(settingsDictionary, "Statistics Caching Enabled", ref statisticsCachingEnabled, ref error, false);
                Get_String_Value(settingsDictionary, "System Base Abbreviation", ref systemBaseAbbreviation, String.Empty);
                Get_String_Value(settingsDictionary, "System Base Name", ref systemBaseName, systemBaseAbbreviation);
                Get_String_Value(settingsDictionary, "System Base URL", ref systemBaseUrl, String.Empty);
                Get_String_Value(settingsDictionary, "System Email", ref systemEmail, ref error);
                Get_String_Value(settingsDictionary, "System Error Email", ref systemErrorEmail, String.Empty);
                Get_Integer_Value(settingsDictionary, "Thumbnail Height", ref thumbnailHeight, ref error, -1);
                Get_Integer_Value(settingsDictionary, "Thumbnail Width", ref thumbnailWidth, ref error, -1);
                Get_String_Value(settingsDictionary, "Upload File Types", ref uploadFileTypes, ".aif,.aifc,.aiff,.au,.avi,.bz2,.c,.c++,.css,.dbf,.ddl,.doc,.docx,.dtd,.dvi,.flac,.gz,.htm,.html,.java,.jps,.js,.m4p,.mid,.midi,.mp2,.mp3,.mpg,.odp,.ogg,.pdf,.pgm,.ppt,.pptx,.ps,.ra,.ram,.rar,.rm,.rtf,.sgml,.swf,.sxi,.tbz2,.tgz,.wav,.wave,.wma,.wmv,.xls,.xlsx,.xml,.zip");
                Get_String_Value(settingsDictionary, "Upload Image Types", ref uploadImageTypes, ".txt,.tif,.jpg,.jp2,.pro");
                Get_String_Value(settingsDictionary, "Web In Process Submission Location", ref inProcessSubmissionLocation, String.Empty);
                Get_Integer_Value(settingsDictionary, "Web Output Suggested Caching", ref webOutputCachingMinutes, ref error, 0);
                

                // Pull the language last, since it must be converted into a Language_Enum
                string default_ui_language_string = "English";
                Get_String_Value(settingsDictionary, "System Default Language", ref default_ui_language_string, "English");
                Default_UI_Language = Web_Language_Enum_Converter.Code_To_Enum(default_ui_language_string);


                return !error;
            }
            catch
            {
                return false;
            }
        }

        private static void Get_String_Value(Dictionary<string, string> Settings_Dictionary, string Key, ref string Setting, ref bool Error)
        {
            if (Settings_Dictionary.ContainsKey(Key))
            {
                Setting = Settings_Dictionary[Key];
                Settings_Dictionary.Remove(Key);
            }
            else
            {
                Error = true;
            }
        }

        private static void Get_String_Value(Dictionary<string, string> Settings_Dictionary, string Key, ref string Setting, string Default_Value)
        {
            if (Settings_Dictionary.ContainsKey( Key ))
            {
                Setting = Settings_Dictionary[Key];
                Settings_Dictionary.Remove(Key);
            }
            else
            {
                Setting = Default_Value;
            }
        }

        private static void Get_Boolean_Value(Dictionary<string, string> Settings_Dictionary, string Key, ref bool Setting, ref bool Error, bool? Default_Value)
        {
            if (Settings_Dictionary.ContainsKey(Key))
            {
                try
                {
                    Setting = Convert.ToBoolean(Settings_Dictionary[Key]);
                    Settings_Dictionary.Remove(Key);
                }
                catch
                {
                    if (Default_Value.HasValue)
                        Setting = Default_Value.Value;
                    else
                        Error = true;
                }
            }
            else
            {
                if (Default_Value.HasValue)
                    Setting = Default_Value.Value;
                else
                    Error = true;
            }
        }

        private static void Get_Integer_Value(Dictionary<string, string> Settings_Dictionary, string Key, ref int Setting, ref bool Error, int Default_Value)
        {
            if (Settings_Dictionary.ContainsKey( Key ))
            {
                try
                {
                    Setting = Convert.ToInt16(Settings_Dictionary[Key]);
                    Settings_Dictionary.Remove(Key);
                }
                catch
                {
                    Error = true;
                }
            }
            else
            {
                Setting = Default_Value;
            }
        }

        #endregion

        #region Methods related to the workflow types and disposition types

        /// <summary> Returns flag that indicates whether the possible workflow types and disposition types 
        /// data has been loaded into this class </summary>
        public static bool Need_Workflow_And_Disposition_Types
        {
            get
            {
                lock (thisLock)
                {
                    return (dispositionPastTypes.Count == 0) && (workflowTypes.Count == 0);
                }
            }
        }

        /// <summary> Gets the list of all possible disposition types in future tense </summary>
        public static List<string> Disposition_Types_Future
        {
            get
            {
                lock (thisLock)
                {
                    return dispositionFutureTypes.Values.Select(ThisValue => ThisValue.Value).ToList();
                }
            }
        }

        /// <summary> Gets the list of all possible disposition types in past tense </summary>
        public static List<string> Disposition_Types_Past
        {
            get
            {
                lock (thisLock)
                {
                    return dispositionPastTypes.Values.Select(ThisValue => ThisValue.Value).ToList();
                }
            }
        }

        /// <summary> Gets the list of all possible workflows </summary>
        public static List<string> Workflows
        {
            get
            {
                lock (thisLock)
                {
                    return workflowTypes.Values.Select(ThisValue => ThisValue.Value).ToList();
                }
            }
        }

        /// <summary> Saves the possible workflow types and disposition values from the database, to be treated as constant settings
        /// for the period of time the web application functios </summary>
        /// <param name="WorkflowTable"> DataTable with all the possible workflow types, from the database </param>
        /// <param name="DispositionTypeTable"> DataTable with all the possible disposition types, from the database </param>
        public static void Set_Workflow_And_Disposition_Types(DataTable WorkflowTable, DataTable DispositionTypeTable)
        {
            lock (thisLock)
            {
                foreach (DataRow thisRow in DispositionTypeTable.Rows)
                {
                    int id = Convert.ToInt32(thisRow["DispositionID"]);
                    string future = thisRow["DispositionFuture"].ToString();
                    string past = thisRow["DispositionPast"].ToString();

                    KeyValuePair<int, string> futureValue = new KeyValuePair<int, string>(id, future);
                    KeyValuePair<int, string> pastValue = new KeyValuePair<int, string>(id, past);
                    dispositionFutureTypes[id] = futureValue;
                    dispositionPastTypes[id] = pastValue;
                }

                foreach (DataRow thisRow in WorkflowTable.Rows)
                {
                    int id = Convert.ToInt32(thisRow["WorkFlowID"]);
                    string workflow = thisRow["WorkFlowName"].ToString();

                    KeyValuePair<int, string> newValue = new KeyValuePair<int, string>(id, workflow);
                    workflowTypes[id] = newValue;
                }
            }
        }

        /// <summary> Gets the disposition term, in past tense, by primary key </summary>
        /// <param name="DispositionID"> Primary key for the disposition term </param>
        /// <returns> Term in past tense, or "UNKNOWN" </returns>
        public static string Disposition_Term_Past(int DispositionID)
        {
            lock (thisLock)
            {
                return dispositionPastTypes.ContainsKey(DispositionID) ? dispositionPastTypes[DispositionID].Value : "UNKNOWN";
            }
        }

        /// <summary> Gets the disposition term, in future tense, by primary key </summary>
        /// <param name="DispositionID"> Primary key for the disposition term </param>
        /// <returns> Term in future tense, or "UNKNOWN" </returns>
        public static string Disposition_Term_Future(int DispositionID)
        {
            lock (thisLock)
            {
                return dispositionFutureTypes.ContainsKey(DispositionID) ? dispositionFutureTypes[DispositionID].Value : "UNKNOWN";
            }
        }

        /// <summary> Gets the primary key for a disposition type, by the past tense term </summary>
        /// <param name="Disposition_Term"> Disposition term, in past tense </param>
        /// <returns> Primary key for the matching disposition, or -1 </returns>
        public static int Disposition_ID_Past(string Disposition_Term)
        {
            lock (thisLock)
            {
                foreach (KeyValuePair<int, string> thisValue in dispositionPastTypes.Values.Where(ThisValue => Disposition_Term == ThisValue.Value))
                {
                    return thisValue.Key;
                }
                return -1;
            }
        }

        /// <summary> Gets the primary key for a disposition type, by the future tense term </summary>
        /// <param name="Disposition_Term"> Disposition term, in future tense </param>
        /// <returns> Primary key for the matching disposition, or -1 </returns>
        public static int Disposition_ID_Future(string Disposition_Term)
        {
            lock (thisLock)
            {
                foreach (KeyValuePair<int, string> thisValue in dispositionFutureTypes.Values.Where(ThisValue => Disposition_Term == ThisValue.Value))
                {
                    return thisValue.Key;
                }
                return -1;
            }
        }

        #endregion

        #region Methods related to the metadata types (search fields)

        /// <summary> Returns flag that indicates whether the metadata types 
        /// data has been loaded into this class </summary>
        public static bool Need_Metadata_Types
        {
            get
            {
                lock (thisLock)
                {
                    return metadataFields.Count == 0;
                }
            }
        }

        /// <summary> Gets the list of all metadata search fields </summary>
        public static List<Metadata_Search_Field> Metadata_Search_Fields
        {
            get
            {
                lock (thisLock)
                {
                    return metadataFields;
                }
            }
        }

        /// <summary> Saves all the metadata types from the database, to be treated as constant settings
        /// for the period of time the web application functios </summary>
        /// <param name="MetadataTypesTable"> DataTable with all the possible metadata types, from the database </param>
        public static void Set_Metadata_Types(DataTable MetadataTypesTable)
        {
            lock (thisLock)
            {
                // Add ANYWHERE
                Metadata_Search_Field anywhere = new Metadata_Search_Field(-1, String.Empty, "Anywhere", "ZZ", "all");
                metadataFields.Add(anywhere);
                metadataFieldsByCode["ZZ"] = anywhere;
                metadataFieldsByID[-1] = anywhere;

                // Add OCLC
                Metadata_Search_Field oclc = new Metadata_Search_Field(-1, String.Empty, "OCLC", "OC", "oclc");
                metadataFieldsByCode["OC"] = oclc;

                // Add ALEPH
                Metadata_Search_Field aleph = new Metadata_Search_Field(-1, String.Empty, "ALEPH", "AL", "aleph");
                metadataFieldsByCode["AL"] = aleph;

                // Add Full Text
                Metadata_Search_Field fulltext = new Metadata_Search_Field(-1, String.Empty, "Full Text", "TX", "fulltext");
                metadataFieldsByCode["TX"] = fulltext;

                // Get the data columns
                DataColumn idColumn = MetadataTypesTable.Columns["MetadataTypeID"];
                DataColumn codeColumn = MetadataTypesTable.Columns["SobekCode"];
                DataColumn displayColumn = MetadataTypesTable.Columns["DisplayTerm"];
                DataColumn facetColumn = MetadataTypesTable.Columns["FacetTerm"];
                DataColumn solrColumn = MetadataTypesTable.Columns["SolrCode"];
				DataColumn nameColumn = MetadataTypesTable.Columns["MetadataName"];

                // Now add the rest of the fields
                foreach (DataRow thisRow in MetadataTypesTable.Rows)
                {
                    // Only add here if there is a sobek code
                    if (thisRow[codeColumn] == DBNull.Value) continue;

                    // Retrieve each individual value
                    short id = Convert.ToInt16(thisRow[idColumn]);
                    string code = thisRow[codeColumn].ToString();
                    string display = thisRow[displayColumn].ToString();
                    string facet = thisRow[facetColumn].ToString();
                    string solr = thisRow[solrColumn].ToString();
	                string name = thisRow[nameColumn].ToString();

                    // Create the new field object
                    Metadata_Search_Field newField = new Metadata_Search_Field(id, facet, display, code, solr);

                    // Add this to the collections
                    metadataFields.Add(newField);
                    metadataFieldsByCode[code] = newField;
                    metadataFieldsByID[id] = newField;
	                metadataFieldsByName[name.ToLower().Replace("_", " ")] = newField;

                    if (facet.Length > 0)
                    {
                        metadataFieldsByFacetName[facet.Replace("_", " ").ToLower()] = newField;
                    }
                }
            }
        }

        /// <summary> Gets a single metadata search field, by SobekCM web code  </summary>
        /// <param name="SobekCM_Code"> Code for this metadata search field, employed within the URLs </param>
        /// <returns> Metadata search field, or else NULL </returns>
        public static Metadata_Search_Field Metadata_Search_Field_By_Code(string SobekCM_Code)
        {
            lock (thisLock)
            {
                return metadataFieldsByCode.ContainsKey(SobekCM_Code) ? metadataFieldsByCode[SobekCM_Code] : null;
            }
        }

		/// <summary> Gets a single metadata search field, by the metadata column name </summary>
		/// <param name="Metadata_Name"> Name for this field and name of the column in the database </param>
		/// <returns> Metadata search field, or else NULL </returns>
		public static Metadata_Search_Field Metadata_Search_Field_By_Name(string Metadata_Name)
		{
			lock (thisLock)
			{
				return metadataFieldsByName.ContainsKey(Metadata_Name.Replace("_", " ").ToLower()) ? metadataFieldsByName[Metadata_Name.Replace("_", " ").ToLower()] : null;
			}
		}

        /// <summary> Gets a single metadata search field, by the facet name </summary>
        /// <param name="Facet_Name"> Name for this field for faceting purposes </param>
        /// <returns> Metadata search field, or else NULL </returns>
        public static Metadata_Search_Field Metadata_Search_Field_By_Facet_Name(string Facet_Name )
        {
            lock (thisLock)
            {
                return metadataFieldsByFacetName.ContainsKey(Facet_Name.Replace("_", " ").ToLower()) ? metadataFieldsByFacetName[Facet_Name.Replace("_", " ").ToLower()] : null;
            }
        }


        /// <summary> Gets a single metadata search field, by primary identifiere  </summary>
        /// <param name="MetadataTypeID"> Primary identifier for the metadata search field </param>
        /// <returns> Metadata search field, or else NULL </returns>
        public static Metadata_Search_Field Metadata_Search_Field_By_ID(short MetadataTypeID)
        {
            lock (thisLock)
            {
                return metadataFieldsByID.ContainsKey(MetadataTypeID) ? metadataFieldsByID[MetadataTypeID] : null;
            }
        }

        /// <summary> Returns the list of all metadat fields in this library </summary>
        public static ReadOnlyCollection<Metadata_Search_Field> All_Metadata_Fields
        {
            get
            {
                return new ReadOnlyCollection<Metadata_Search_Field>(metadataFields);
            }
        }
        
        #endregion

        #region Methods related to the search stop words

        /// <summary> Returns flag that indicates whether the search stop words
        /// data has been loaded into this class </summary>
        public static bool Need_Search_Stop_Words
        {
            get
            {
                lock (thisLock)
                {
                    return searchStopWords.Count == 0;
                }
            }
        }

        /// <summary> Gets the list of all stop words ignored during metadata searching (such as 'The', 'A', etc..) </summary>
        public static List<string> Search_Stop_Words
        {
            get
            {
                lock (thisLock)
                {
                    return searchStopWords;
                }
            }
            set
            {
                lock (thisLock)
                {
                    searchStopWords = value;
                }
            }
        }

        #endregion

        #region Static Public Properties

        /// <summary> Returns the default user interface language </summary>
        public static Web_Language_Enum Default_UI_Language { get; private set; }

        /// <summary> Gets the base name for this system </summary>
        public static string System_Name
        {
            get
            {
                return systemBaseName;
            }
        }

        /// <summary> Email address used when a possible privacy issue appears, such as an apaprent social security number </summary>
        public static string Privacy_Email_Address
        {
            get
            {
                return privacyEmailAddress;
            }
        }

        /// <summary> Flag indicates if the partners browse should be displayed on the home page </summary>
        public static bool Include_Partners_On_System_Home
        {
            get
            {
                return includePartnersOnSystemHome;
            }
        }

        /// <summary> Flag indicates if the tree view should be displayed on the home page </summary>
        public static bool Include_TreeView_On_System_Home
        {
            get
            {
                return includeTreeviewOnSystemHome;
            }
        }

        /// <summary> Flag indicates if the builder should try to convert office files (Word and Powerpoint) to PDF during load and post-processing </summary>
        public static bool Convert_Office_Files_To_PDF
        {
            get
            {
                return convertOfficeFilesToPdf;
            }
        }

        /// <summary> Command to launch the OCR engine against a single TIFF to produce a single TEXT file </summary>
        public static string OCR_Command_Prompt
        {
            get
            {
                return ocrCommandPrompt;
            }
        }

        /// <summary> Flag indicates if the statistics information should be cached for very quick 
        /// retrieval for search engine robots. </summary>
        public static bool Statistics_Caching_Enabled
        {
            get
            {
                return statisticsCachingEnabled;
            }
        }

        /// <summary> Returns the base string for the resource identifiers within OAI.</summary>
        /// <remarks> This indicates the repository from which the material is pulled usually</remarks>
        public static string OAI_Resource_Identifier_Base
        {
            get { return oaiResourceIdentifierBase; }
        }

        /// <summary> Returns the OAI repository identifier, which is usually the system abbreviation </summary>
        public static string OAI_Repository_Identifier
        {
            get { return oaiRepositoryIdentifier; }
        }

        /// <summary> Returns the OAI repository name </summary>
        public static string OAI_Repository_Name
        {
            get { return oaiRepositoryName; }
        }

        /// <summary> Gets the URL for any related Shibboleth authentication system </summary>
        public static string Shibboleth_System_URL
        {
            get { return shibbolethSystemUrl; }
        }

        /// <summary> Gets the system name for any related Shibboleth authentication system (i.e., Gatorlink )</summary>
        public static string Shibboleth_System_Name
        {
            get { return shibbolethSystemName; }
        }

        /// <summary> Gets the regular expression for matching files names to delete BEFORE archiving
        /// incoming digital resource files </summary>
        public static string PreArchive_Files_To_Delete
        {
            get
            {
                return prearchiveFilesToDelete;
            }
        }

        /// <summary> Gets the regular expression for matching files names to delete AFTER archiving
        /// incoming digital resource files </summary>
        public static string PostArchive_Files_To_Delete
        {
            get
            {
                return postarchiveFilesToDelete;
            }
        }

        /// <summary> Gets regular expression for matching file names (with extension) to exclude 
        /// from automatically adding gto the downloads for incoming digital resources  </summary>
        public static string Files_To_Exclude_From_Downloads
        {
            get
            {
                return filesToExcludeFromDownloads;
            }
        }

        /// <summary> Gets the library-wide setting for width of created jpeg derivatives </summary>
        public static int JPEG_Width
        {
            get
            {
                return jpegWidth;
            }
        }

        /// <summary> Gets the library-wide setting for height created jpeg derivatives </summary>
        public static int JPEG_Height
        {
            get
            {
                return jpegHeight;
            }
        }

        /// <summary> Gets the library-wide setting for width for created jpeg thumbnails </summary>
        public static int Thumbnail_Width
        {
            get
            {
                return thumbnailWidth;
            }
        }

        /// <summary> Gets the library-wide setting for height for created jpeg thumbnails </summary>
        public static int Thumbnail_Height
        {
            get
            {
                return thumbnailHeight;
            }
        }

        /// <summary> Directory where the local logs are written </summary>
        /// <remarks> This is determined the first time this class is referenced and is just
        /// a <b>Logs</b> subfolder uynder the application startup path</remarks>
        public static string Local_Log_Directory { get; set; }

        /// <summary> Folder where files bound for archiving are placed </summary>
        public static string Package_Archival_Folder
        {
            get { return packageArchivalFolder; }
        }

        /// <summary> Final destination of the processing log (usually the web server) </summary>
        public static string Log_Files_Directory
        {
            get { return logFilesDirectory; }
            set { logFilesDirectory = value; }
        }

        /// <summary> Final URL for the processing log, for any links between different failure logs </summary>
        public static string Log_Files_URL
        {
            get { return logFilesUrl; }
        }

        /// <summary> Location where all the item-level static page exist for search engine indexing </summary>
        public static string Static_Pages_Location
        {
            get { return staticPagesLocation; }
        }

        /// <summary> List of all the incoming folders which should be checked for new resources </summary>
        public static List<Builder_Source_Folder> Incoming_Folders
        {
            get { return incomingFolders; }
        }

        /// <summary> Network directory for the image server which holds all the resource files </summary>
        public static string Image_Server_Network
        {
            get { return imageServerNetwork; }
        }

        /// <summary> Network directory for the SobekCM web application server </summary>
        public static string Application_Server_Network
        {
            get { return applicationServerNetwork; }
        }

        /// <summary> Primary URL for this instance of the SobekCM web application server </summary>
        public static string Application_Server_URL
        {
            get { return applicationServerUrl; }
        }

        /// <summary> Location where the MarcXML feeds should be placed </summary>
        public static string MarcXML_Feed_Location
        {
            get { return marcxmlFeedLocation; }
        }

        /// <summary> Dropbox to check for any <a href="http://fclaweb.fcla.edu/FDA_landing_page">Florida Digital Archive</a> ingest reports  </summary>
        public static string FDA_Report_DropBox
        {
            get { return fdaReportDropbox; }
        }

        /// <summary> Drop box where packages can be placed to be archived locally </summary>
        public static string Archive_DropBox
        {
            get { return archiveDropbox; }
        }

        /// <summary> URL for the Solr/Lucene index for the document metadata and text </summary>
        public static string Document_Solr_Index_URL
        {
            get { return documentSolrUrl; }
        }

        /// <summary> URL for the Solr/Lucene index for the page text </summary>
        public static string Page_Solr_Index_URL
        {
            get { return pageSolrUrl; }
        }

        /// <summary> Flag indicates if the MARC feed should be built by default by the bulk loader </summary>
        public static bool Build_MARC_Feed_By_Default
        {
            get
            {
                return buildMarcFeedByDefault;
            }
        }

        /// <summary> Email address for system errors </summary>
        public static string System_Error_Email
        {
            get { return systemErrorEmail; }
        }

        /// <summary> Main email address for this system </summary>
        public static string System_Email
        {
            get { return systemEmail; }
        }

        /// <summary> Gets the Mango Union search base URL, in support of Florida SUS's</summary>
        public static string Mango_Union_Search_Base_URL
        {
            get { return mangoUnionSearchBaseUrl; }
        }

        /// <summary> Gets the Mango Union search text to be displayed, in support of Florida SUS's</summary>
        public static string Mango_Union_Search_Text
        {
            get { return mangoUnionSearchText; }
        }

        /// <summary> Flag indicates whether the facets should be pulled during a browse </summary>
        /// <value> This always returns TRUE is only retained here in case this is an option that is wanted in the future</value>
        public static bool Pull_Facets_On_Browse
        {
            get {   return true;  }
        }

        /// <summary> Flag indicates whether the facets should be pulled during a search </summary>
        /// <value> This always returns TRUE is only retained here in case this is an option that is wanted in the future</value>
        public static bool Pull_Facets_On_Search
        {
            get {   return true;  }
        }

        /// <summary> Name of the caching server (or blank if disabled) </summary>
        public static string Caching_Server
        {
            get { return cachingServer; }
        }

        /// <summary> URL to the SobekCM Image Server, initially used just when features need to be drawn on images </summary>
        public static string SobekCM_ImageServer
        {
            get { return sobekcmImageserver; }
        }

        /// <summary> Flag indicates if online submissions and edits can occur at the moment </summary>
        public static bool Online_Edit_Submit_Enabled
        {
            get { return canSubmit; }
        }

        /// <summary> Gets the error web page to send users to when a catastrophic error occurs </summary>
        /// <value> For example, for UFDC this always returns 'http://ufdc.ufl.edu/error.html' </value>
        public static string System_Error_URL 
        {
            get;
            set;
        }

        /// <summary> Gets the complete url to this instance of SobekCM library software </summary>
        /// <value> Currently this always returns 'http://ufdc.ufl.edu/' </value>
        public static string System_Base_URL
        {
            get { return systemBaseUrl; }
        }

        /// <summary> Gets the abbrevation used to refer to this digital library </summary>
        public static string System_Abbreviation
        {
            get { return systemBaseAbbreviation; }
        }

        /// <summary> Gets the location that submission packets are built before being submitted into the regular
        /// digital resource location </summary>
        public static string In_Process_Submission_Location
        {
            get
            {
                return (inProcessSubmissionLocation.Length > 0) ?  inProcessSubmissionLocation :  baseDirectory + "\\mySobek\\InProcess";
            }
        }

        /// <summary> Relative location to the folders on the web server </summary>
        /// <remarks> This is only used when building static pages in the SobekCM Builder, which allows for
        /// the replacement of all the relative references ( i.e., '/design/skins/dloc/dloc.css') with the full
        /// link ( i.e., 'http://ufdc.ufl.edu/design/skins/dloc/dloc.css' ) </remarks>
        public static string Base_SobekCM_Location_Relative { get; set; }

        /// <summary> URL to the Aware JPEG2000 zoomable image server </summary>
        public static string JP2_Server
        {
            get { return jpeg2000Server; }
        }

	    /// <summary> Database connection string(s) built from the system config file (usually sits in a config subfolder)</summary>
	    public static ReadOnlyCollection<Database_Instance_Configuration> Database_Connections
	    {
		    get
		    {
			    return new ReadOnlyCollection<Database_Instance_Configuration>(databaseInfo);
		    }
	    }


            


        /// <summary> Base directory where the ASP.net application is running on the application server </summary>
        public static string Base_Directory
        {
            get { return baseDirectory; }
            set { baseDirectory = value; }
        }

        /// <summary> Gets the base URL for this instance, without the application name </summary>
        public static string Base_URL
        {
            get {   return base_url;    }
        }

        /// <summary> Base image URL for all digital resource images </summary>
        public static string Image_URL
        {
            get { return imageServerUrl; }
        }

        /// <summary> Number of seconds the builder waits between polls </summary>
        public static int Builder_Seconds_Between_Polls
        {
            get { return builderSecondsBetweenPolls; }
        }

        /// <summary> Number of days builder logs remain before the builder will try to delete it </summary>
        public static int Builder_Log_Expiration_Days
        {
            get { return builderLogExpirationDays; }
        }

        /// <summary> Flag indicates if the builder should be extra verbose in the log files (used for debugging purposes mostly) </summary>
        public static bool Builder_Verbose_Flag
        {
            get { return builderVerbose; }
        }

        /// <summary> Number of minutes clients are suggested to cache the web output </summary>
        public static int Web_Output_Caching_Minutes
        {
            get { return webOutputCachingMinutes; }
        }

        /// <summary> ImageMagick executable file </summary>
        public static string ImageMagick_Executable
        {
            get { return imageMagickExecutable; }
            set { imageMagickExecutable = value; }
        }

        /// <summary> Ghostscript executable file </summary>
        public static string Ghostscript_Executable
        {
            get { return ghostscriptExecutable; }
            set { ghostscriptExecutable = value; }
        }

        /// <summary> Returns the network location for the main builder, which runs essentially
        /// without restrictions  </summary>
        public static string Main_Builder_Input_Folder
        {
            get { return mainBuilderInputFolder; }
        }

        /// <summary> IP address for the SobekCM web server </summary>
        public static string SobekCM_Web_Server_IP
        {
            get { return webServerIp; }
            set { webServerIp = value; }
        }

        /// <summary> Network location of the recycle bin, where deleted items and
        /// files are placed for a while, in case of accidental deletion </summary>
        public static string Recycle_Bin
        {
            get { return imageServerNetwork + "\\RECYCLE BIN"; }
        }

        /// <summary> List of file type extensions which can be uploaded in the
        /// file management interface.  These should all treated as downloads in the system. </summary>
        public static string Upload_File_Types
        {
            get { return uploadFileTypes; }
        }

        /// <summary> List of file type extensions which can be uploaded in the page
        /// image upload interface.  These should all be treated as page files in the system </summary>
        public static string Upload_Image_Types
        {
            get { return uploadImageTypes; }
        }

        #region Methods which return the base directory or base url with a constant ending to indicate the SobekCM standard subfolders
        
        /// <summary> Directory for this application's myUFDC folder, where the template and project files reside for online submittal and editing</summary>
        /// <value> [Base_Directory] + 'myUFDC\' </value>
        public static string Base_MySobek_Directory
        {
            get { return baseDirectory + "mySobek\\"; }
        }

        /// <summary> Directory for this application's DATA folder, where the OAI source files reside </summary>
        /// <value> [Base_Dir] + 'data\' </value>
        public static string Base_Data_Directory
        {
            get { return baseDirectory + "data\\"; }
        }

        /// <summary> Directory for this application's TEMP folder, where some slow-changing data is stored in XML format </summary>
        /// <value> [Base_Dir] + 'temp\' </value>
        public static string Base_Temporary_Directory
        {
            get { return baseDirectory + "temp\\"; }
        }

        /// <summary> Directory for this application's DESIGN folder, where all the aggregation and interface folders reside </summary>
        /// <value> [Base_Directory] + 'design\' </value>
        public static string Base_Design_Location
        {
            get { return baseDirectory + "design\\"; }
        }

        #endregion

        #endregion

        /// <summary> Get the URL for all metadata help pages which are used when users request 
        /// help while submitting a new item or editing an existing item </summary>
        /// <param name="Current_Base_URL"> Current base url for the current user's request </param>
        /// <returns> Base URL to use for the metadata help page links </returns>
        public static string Metadata_Help_URL(string Current_Base_URL)
        {
            return metadataHelpUrl.Length > 0 ? helpUrl : Current_Base_URL;
        }

        /// <summary> URL used for the main help pages about this system's basic functionality </summary>
        /// <param name="Current_Base_URL"> Current base url for the current user's request </param>
        /// <returns> Base URL to use for the main help page links </returns>
        public static string Help_URL(string Current_Base_URL)
        {
            return helpUrl.Length > 0 ? helpUrl : Current_Base_URL;
        }
    }
}
