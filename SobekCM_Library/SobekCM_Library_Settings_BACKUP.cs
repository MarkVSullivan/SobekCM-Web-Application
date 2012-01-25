using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using SobekCM.Library.Application_State;
using SobekCM.Library.Search;

namespace SobekCM.Library
{
    /// <summary> Class provides static context to constant settings based on the basic information about this instance of the application and server information </summary>
	public class SobekCM_Library_Settings
	{
        /// <summary> Number of ticks that a complete package must age before being processed </summary>
        /// <value> This is currently set to 15 minutes (in ticks) </value>
        public static long Complete_Package_Required_Aging = 15L * 60L * 1000000L;     // 15 minutes (in ticks)

        /// <summary> Number of ticks that a metadata only package must age before being processed </summary>
        /// <value> This is currently set to 1 minute (in ticks) </value>
        public static long METS_Only_Package_Required_Aging = 60L * 10000000L;          // 1 Minute (in ticks)



        // Values pulled from database 
        private static bool initialization_error;
        private static string local_log_directory;
        private static string package_archival_folder, log_files_directory, log_files_url, static_pages_location;
        private static string image_server_network, application_server_network, application_server_url, marcxml_feed_location;
        private static string fda_report_dropbox, image_magick_path, archive_dropbox;
        private static string document_solr_url, page_solr_url;
        private static bool build_marc_feed_by_default;
        private static List<SobekCM.Library.Builder.Builder_Source_Folder> incomingFolders;
        private static int jpeg_width, jpeg_height;
        private static int thumbnail_width, thumbnail_height;
        private static string files_to_exclude_from_downloads;
        private static string prearchive_files_to_delete;
        private static string postarchive_files_to_delete;
        private static string jpeg2000_server;
        private static string database_string;
        private static string system_error_email, system_email;
        private static string shibboleth_system_url, shibboleth_system_name;
        private static string system_base_url;
        private static string system_error_url;
        private static string system_base_abbreviation;
        private static string oai_resource_identifier_base;
        private static string oai_repository_identifier;
        private static string oai_repository_name;
        private static string ocr_command_prompt;
        private static int web_output_caching_minutes;
        private static bool statistics_caching_enabled;
        private static bool allow_page_image_file_management;
        private static bool include_treeview_on_system_home;

        private static string base_url, base_directory,
            image_server_url, base_sobek_location_relative, in_process_submission_location,
            sobekcm_imageserver, caching_server;
        private static bool canSubmit;

        private static Dictionary<int, KeyValuePair<int, string>> dispositionFutureTypes;
        private static Dictionary<int, KeyValuePair<int, string>> dispositionPastTypes;
        private static Dictionary<int, KeyValuePair<int, string>> workflowTypes;


        private static List<Metadata_Search_Field> metadataFields;
        private static Dictionary<string, Metadata_Search_Field> metadataFieldsByCode;
        private static Dictionary<short, Metadata_Search_Field> metadataFieldsByID;

        private static string mango_union_search_base_url;  // "http://solrcits.fcla.edu/citsZ.jsp?type=search&base=uf";
        private static string mango_union_search_text;

        private static List<string> searchStopWords;

        private static Object thisLock = new Object();

        /// <summary> Static constructor sets all the values to default empty strings </summary>
        static SobekCM_Library_Settings()
        {
            initialization_error = false;

            try
            {
                // Set some default values
                base_sobek_location_relative = String.Empty;
                jpeg2000_server = String.Empty;
                database_string = String.Empty;
                base_url = String.Empty;
                base_directory = String.Empty;
                image_server_url = String.Empty;
                in_process_submission_location = String.Empty;
                sobekcm_imageserver = String.Empty;
                canSubmit = false;
                caching_server = String.Empty;

                // Define new empty collections
                dispositionFutureTypes = new Dictionary<int, KeyValuePair<int, string>>();
                dispositionPastTypes = new Dictionary<int, KeyValuePair<int, string>>();
                workflowTypes = new Dictionary<int, KeyValuePair<int, string>>();
                metadataFields = new List<Metadata_Search_Field>();
                metadataFieldsByCode = new Dictionary<string, Metadata_Search_Field>();
                metadataFieldsByID = new Dictionary<short, Metadata_Search_Field>();
                incomingFolders = new List<SobekCM.Library.Builder.Builder_Source_Folder>();
                searchStopWords = new List<string>();

                // Set the system error email
                system_error_url = System.Configuration.ConfigurationSettings.AppSettings["Error_HTML_Page"];

                // Get the database connection from the app settings for this app 
                string database_source = System.Configuration.ConfigurationSettings.AppSettings["Database_Source"];
                string database_name = System.Configuration.ConfigurationSettings.AppSettings["Database_Name"];
                if ((database_name.Length > 0) && (database_source.Length > 0))
                {
                    database_string = "data source=" + database_source + ";initial catalog=" + database_name + ";integrated security=Yes;";
                    if ( String.IsNullOrEmpty(SobekCM.Library.Database.SobekCM_Database.Connection_String))
                        SobekCM.Library.Database.SobekCM_Database.Connection_String = database_string;
                    Refresh(SobekCM.Library.Database.SobekCM_Database.Get_Settings_Complete(null));
                }

                web_output_caching_minutes = 15;
                try
                {
                    web_output_caching_minutes = Convert.ToInt32(System.Configuration.ConfigurationSettings.AppSettings["Web_Output_Caching_Minutes"]);
                }
                catch
                {

                }

            }
            catch (Exception ee)
            {
                initialization_error = true;
            }
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
                        incomingFolders.Add(new SobekCM.Library.Builder.Builder_Source_Folder(thisRow));
                    }
                }
                
                // Get the settings table
                DataTable settingsTable = SobekCM_Settings.Tables[0];

                // Create the dictionary for quick lookups for the next work
                Dictionary<string, string> settingsDictionary = new Dictionary<string, string>();
                foreach (DataRow thisRow in settingsTable.Rows)
                {
                    settingsDictionary[thisRow["Setting_Key"].ToString()] = thisRow["Setting_Value"].ToString();
                }

                // Pull all of the builder settings value ( from UFDC_Builder_Settings )
                Get_Boolean_Value(settingsDictionary, "Allow Page Image File Management", ref allow_page_image_file_management, ref error, false);
                Get_String_Value(settingsDictionary, "Application Server Network", ref application_server_network, ref error);
                Get_String_Value(settingsDictionary, "Application Server URL", ref application_server_url, ref error);
                Get_String_Value(settingsDictionary, "Archive DropBox", ref archive_dropbox, ref error);
                Get_String_Value(settingsDictionary, "Caching Server", ref caching_server, ref error);
                Get_Boolean_Value(settingsDictionary, "Can Submit Edit Online", ref canSubmit, ref error, false);
                Get_Boolean_Value(settingsDictionary, "Create MARC Feed By Default", ref build_marc_feed_by_default, ref error, false);
                Get_String_Value(settingsDictionary, "Document Solr Index URL", ref document_solr_url, ref error);
                Get_String_Value(settingsDictionary, "FDA Report DropBox", ref fda_report_dropbox, ref error);
                Get_String_Value(settingsDictionary, "Files To Exclude From Downloads", ref files_to_exclude_from_downloads, ref error);
                Get_String_Value(settingsDictionary, "Image Server Network", ref image_server_network, ref error);
                Get_String_Value(settingsDictionary, "Image Server URL", ref image_server_url, ref error);
                Get_Boolean_Value(settingsDictionary, "Include TreeView On System Home", ref include_treeview_on_system_home, ref error, true);
                Get_Integer_Value(settingsDictionary, "JPEG Height", ref jpeg_height, ref error, -1);
                Get_Integer_Value(settingsDictionary, "JPEG Width", ref jpeg_width, ref error, -1);
                Get_String_Value(settingsDictionary, "JPEG2000 Server", ref jpeg2000_server, ref error);
                Get_String_Value(settingsDictionary, "Log Files Directory", ref log_files_directory, ref error);
                Get_String_Value(settingsDictionary, "Log Files URL", ref log_files_url, ref error);
                Get_String_Value(settingsDictionary, "Mango Union Search Base URL", ref mango_union_search_base_url, ref error);
                Get_String_Value(settingsDictionary, "Mango Union Search Text", ref mango_union_search_text, ref error);
                Get_String_Value(settingsDictionary, "MarcXML Feed Location", ref marcxml_feed_location, ref error);
                Get_String_Value(settingsDictionary, "OAI Resource Identifier Base", ref oai_resource_identifier_base, ref error);
                Get_String_Value(settingsDictionary, "OAI Repository Identifier", ref oai_repository_identifier, ref error);
                Get_String_Value(settingsDictionary, "OAI Repository Name", ref oai_repository_name, ref error);
                Get_String_Value(settingsDictionary, "OCR Command Prompt", ref ocr_command_prompt, ref error, String.Empty);
                Get_String_Value(settingsDictionary, "Package Archival Folder", ref package_archival_folder, ref error, String.Empty);
                Get_String_Value(settingsDictionary, "Page Solr Index URL", ref page_solr_url, ref error, String.Empty);
                Get_String_Value(settingsDictionary, "PostArchive Files To Delete", ref postarchive_files_to_delete, ref error, String.Empty);
                Get_String_Value(settingsDictionary, "PreArchive Files To Delete", ref prearchive_files_to_delete, ref error, String.Empty);
                Get_String_Value(settingsDictionary, "Shibboleth System URL", ref shibboleth_system_url, ref error, String.Empty);
                Get_String_Value(settingsDictionary, "Shibboleth System Name", ref shibboleth_system_name, ref error, String.Empty);
                Get_String_Value(settingsDictionary, "SobekCM Image Server", ref sobekcm_imageserver, ref error, String.Empty);
                Get_String_Value(settingsDictionary, "Static Pages Location", ref static_pages_location, ref error);
                Get_Boolean_Value(settingsDictionary, "Statistics Caching Enabled", ref statistics_caching_enabled, ref error, false);
                Get_String_Value(settingsDictionary, "System Base Abbreviation", ref system_base_abbreviation, ref error, String.Empty);
                Get_String_Value(settingsDictionary, "System Base URL", ref system_base_url, ref error, String.Empty);
                Get_String_Value(settingsDictionary, "System Email", ref system_email, ref error);
                Get_String_Value(settingsDictionary, "System Error Email", ref system_error_email, ref error, String.Empty);
                Get_Integer_Value(settingsDictionary, "Thumbnail Height", ref thumbnail_height, ref error, -1);
                Get_Integer_Value(settingsDictionary, "Thumbnail Width", ref thumbnail_width, ref error, -1);
                Get_String_Value(settingsDictionary, "Web In Process Submission Location", ref in_process_submission_location, ref error, String.Empty);

                return !error;
            }
            catch ( Exception ee )
            {
                return false;
            }
        }

        private static void Get_String_Value(Dictionary<string, string> Settings_Dictionary, string Key, ref string Setting, ref bool Error)
        {
            if (Settings_Dictionary.ContainsKey(Key))
            {
                Setting = Settings_Dictionary[Key];
            }
            else
            {
                Error = true;
            }
        }

        private static void Get_String_Value(Dictionary<string, string> Settings_Dictionary, string Key, ref string Setting, ref bool Error, string Default_Value)
        {
            if (Settings_Dictionary.ContainsKey( Key ))
            {
                Setting = Settings_Dictionary[Key];
            }
            else
            {
                if (Default_Value.Length != null)
                {
                    Setting = Default_Value;
                }
                else
                {
                    Error = true;
                }
            }
        }

        private static void Get_Boolean_Value(Dictionary<string, string> Settings_Dictionary, string Key, ref bool Setting, ref bool Error, Nullable<bool> Default_Value)
        {
            if (Settings_Dictionary.ContainsKey(Key))
            {
                try
                {
                    Setting = Convert.ToBoolean(Settings_Dictionary[Key]);
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
                    if ((dispositionPastTypes.Count == 0) && (workflowTypes.Count == 0))
                        return true;
                    return false;
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

        /// <summary> Gets the list of all possible disposition types in future tense </summary>
        public static List<string> Disposition_Types_Future
        {
            get
            {
                lock (thisLock)
                {
                    List<string> returnValue = new List<string>();
                    foreach (KeyValuePair<int, string> thisValue in dispositionFutureTypes.Values)
                    {
                        returnValue.Add(thisValue.Value);
                    }
                    return returnValue;
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
                    List<string> returnValue = new List<string>();
                    foreach (KeyValuePair<int, string> thisValue in dispositionPastTypes.Values)
                    {
                        returnValue.Add(thisValue.Value);
                    }
                    return returnValue;
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
                    List<string> returnValue = new List<string>();
                    foreach (KeyValuePair<int, string> thisValue in workflowTypes.Values)
                    {
                        returnValue.Add(thisValue.Value);
                    }
                    return returnValue;
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
                if (dispositionPastTypes.ContainsKey(DispositionID))
                {
                    return dispositionPastTypes[DispositionID].Value;
                }
                return "UNKNOWN";
            }
        }


        /// <summary> Gets the disposition term, in future tense, by primary key </summary>
        /// <param name="DispositionID"> Primary key for the disposition term </param>
        /// <returns> Term in future tense, or "UNKNOWN" </returns>
        public static string Disposition_Term_Future(int DispositionID)
        {
            lock (thisLock)
            {
                if (dispositionFutureTypes.ContainsKey(DispositionID))
                {
                    return dispositionFutureTypes[DispositionID].Value;
                }
                return "UNKNOWN";
            }
        }

        /// <summary> Gets the primary key for a disposition type, by the past tense term </summary>
        /// <param name="Disposition_Term"> Disposition term, in past tense </param>
        /// <returns> Primary key for the matching disposition, or -1 </returns>
        public static int Disposition_ID_Past(string Disposition_Term)
        {
            lock (thisLock)
            {
                foreach (KeyValuePair<int, string> thisValue in dispositionPastTypes.Values)
                {
                    if (Disposition_Term == thisValue.Value)
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
                foreach (KeyValuePair<int, string> thisValue in dispositionFutureTypes.Values)
                {
                    if (Disposition_Term == thisValue.Value)
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
                    if (metadataFields.Count == 0)
                        return true;
                    return false;
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
                Metadata_Search_Field anywhere = new Metadata_Search_Field(-1, "INVALID", "Anywhere", "ZZ", "all");
                metadataFields.Add(anywhere);
                metadataFieldsByCode["ZZ"] = anywhere;
                metadataFieldsByID[-1] = anywhere;

                // Add OCLC
                Metadata_Search_Field oclc = new Metadata_Search_Field(-1, "INVALID", "OCLC", "OC", "oclc");
                metadataFieldsByCode["OC"] = oclc;

                // Add ALEPH
                Metadata_Search_Field aleph = new Metadata_Search_Field(-1, "INVALID", "ALEPH", "AL", "aleph");
                metadataFieldsByCode["AL"] = aleph;

                // Add Full Text
                Metadata_Search_Field fulltext = new Metadata_Search_Field(-1, "INVALID", "Full Text", "TX", "fulltext");
                metadataFieldsByCode["TX"] = fulltext;

                // Get the data columns
                DataColumn idColumn = MetadataTypesTable.Columns["MetadataTypeID"];
                DataColumn nameColumn = MetadataTypesTable.Columns["MetadataName"];
                DataColumn codeColumn = MetadataTypesTable.Columns["SobekCode"];
                DataColumn displayColumn = MetadataTypesTable.Columns["DisplayTerm"];
                DataColumn facetColumn = MetadataTypesTable.Columns["FacetTerm"];
                DataColumn solrColumn = MetadataTypesTable.Columns["SolrCode"];

                // Now add the rest of the fields
                foreach (DataRow thisRow in MetadataTypesTable.Rows)
                {
                    // Only add here if there is a sobek code
                    if (thisRow[codeColumn] != DBNull.Value)
                    {
                        // Retrieve each individual value
                        short id = Convert.ToInt16(thisRow[idColumn]);
                        string name = thisRow[nameColumn].ToString();
                        string code = thisRow[codeColumn].ToString();
                        string display = thisRow[displayColumn].ToString();
                        string facet = thisRow[facetColumn].ToString();
                        string solr = thisRow[solrColumn].ToString();

                        // Create the new field object
                        Metadata_Search_Field newField = new Metadata_Search_Field(id, facet, display, code, solr);

                        // Add this to the collections
                        metadataFields.Add(newField);
                        metadataFieldsByCode[code] = newField;
                        metadataFieldsByID[id] = newField;
                    }
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

        /// <summary> Gets a single metadata search field, by SobekCM web code  </summary>
        /// <param name="SobekCM_Code"> Code for this metadata search field, employed within the URLs </param>
        /// <returns> Metadata search field, or else NULL </returns>
        public static Metadata_Search_Field Metadata_Search_Field_By_Code(string SobekCM_Code)
        {
            lock (thisLock)
            {
                if (metadataFieldsByCode.ContainsKey(SobekCM_Code))
                    return metadataFieldsByCode[SobekCM_Code];
                else
                    return null;
            }
        }

        /// <summary> Gets a single metadata search field, by primary identifiere  </summary>
        /// <param name="MetadataTypeID"> Primary identifier for the metadata search field </param>
        /// <returns> Metadata search field, or else NULL </returns>
        public static Metadata_Search_Field Metadata_Search_Field_By_ID(short MetadataTypeID)
        {
            lock (thisLock)
            {
                if (metadataFieldsByID.ContainsKey(MetadataTypeID))
                    return metadataFieldsByID[MetadataTypeID];
                else
                    return null;
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
                    if (searchStopWords.Count == 0)
                        return true;
                    return false;
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

        /// <summary> Flag indicates if the tree view should be displayed on the home page </summary>
        public static bool Include_TreeView_On_System_Home
        {
            get
            {
                return include_treeview_on_system_home;
            }
        }

        /// <summary> Command to launch the OCR engine against a single TIFF to produce a single TEXT file </summary>
        public static string OCR_Command_Prompt
        {
            get
            {
                return ocr_command_prompt;
            }
        }

        /// <summary> Flag indicates if users can perform simple file management against page images, or
        /// whether they are restricted to file management on download files only </summary>
        public static bool Allow_Page_Image_File_Management
        {
            get
            {
                return allow_page_image_file_management;
            }
        }

        /// <summary> Flag indicates if the statistics information should be cached for very quick 
        /// retrieval for search engine robots. </summary>
        public static bool Statistics_Caching_Enabled
        {
            get
            {
                return statistics_caching_enabled;
            }
        }

        /// <summary> Number of minutes for the web output to be cached on client browser </summary>
        public static int Web_Output_Caching_Minutes
        {
            get
            {
                return web_output_caching_minutes;
            }
        }

        /// <summary> Returns the base string for the resource identifiers within OAI.</summary>
        /// <remarks> This indicates the repository from which the material is pulled usually</remarks>
        public static string OAI_Resource_Identifier_Base
        {
            get { return oai_resource_identifier_base; }
        }

        /// <summary> Returns the OAI repository identifier, which is usually the system abbreviation </summary>
        public static string OAI_Repository_Identifier
        {
            get { return oai_repository_identifier; }
        }

        /// <summary> Returns the OAI repository name </summary>
        public static string OAI_Repository_Name
        {
            get { return oai_repository_name; }
        }

        /// <summary> Gets the URL for any related Shibboleth authentication system </summary>
        public static string Shibboleth_System_URL
        {
            get { return shibboleth_system_url; }
        }

        /// <summary> Gets the system name for any related Shibboleth authentication system (i.e., Gatorlink )</summary>
        public static string Shibboleth_System_Name
        {
            get { return shibboleth_system_name; }
        }

        /// <summary> Gets the regular expression for matching files names to delete BEFORE archiving
        /// incoming digital resource files </summary>
        public static string PreArchive_Files_To_Delete
        {
            get
            {
                return prearchive_files_to_delete;
            }
        }

        /// <summary> Gets the regular expression for matching files names to delete AFTER archiving
        /// incoming digital resource files </summary>
        public static string PostArchive_Files_To_Delete
        {
            get
            {
                return postarchive_files_to_delete;
            }
        }

        /// <summary> Gets regular expression for matching file names (with extension) to exclude 
        /// from automatically adding gto the downloads for incoming digital resources  </summary>
        public static string Files_To_Exclude_From_Downloads
        {
            get
            {
                return files_to_exclude_from_downloads;
            }
        }

        /// <summary> Gets the library-wide setting for width of created jpeg derivatives </summary>
        public static int JPEG_Width
        {
            get
            {
                return jpeg_width;
            }
        }

        /// <summary> Gets the library-wide setting for height created jpeg derivatives </summary>
        public static int JPEG_Height
        {
            get
            {
                return jpeg_height;
            }
        }

        /// <summary> Gets the library-wide setting for width for created jpeg thumbnails </summary>
        public static int Thumbnail_Width
        {
            get
            {
                return thumbnail_width;
            }
        }

        /// <summary> Gets the library-wide setting for height for created jpeg thumbnails </summary>
        public static int Thumbnail_Height
        {
            get
            {
                return thumbnail_height;
            }
        }

        /// <summary> Database connection string for the SobekCM database </summary>
        /// <remarks> This is assembled from the <b>Database_Source</b> and <b>Database_Name</b> 
        /// application settings from the application configuration xml file (app.config) </remarks>
        public static string DB_ConnectionString
        {
            get
            {
                return database_string;
            }
        }

        /// <summary> Directory where the local logs are written </summary>
        /// <remarks> This is determined the first time this class is referenced and is just
        /// a <b>Logs</b> subfolder uynder the application startup path</remarks>
        public static string Local_Log_Directory
        {
            get { return local_log_directory; }
            set { local_log_directory = value; }
        }

        /// <summary> Directory where the image magick executables currently reside </summary>
        public static string Image_Magick_Path
        {
            get { return image_magick_path; }
        }

        /// <summary> Folder where files bound for archiving are placed </summary>
        public static string Package_Archival_Folder
        {
            get { return package_archival_folder; }
        }

        /// <summary> Final destination of the processing log (usually the web server) </summary>
        public static string Log_Files_Directory
        {
            get { return log_files_directory; }
            set { log_files_directory = value; }
        }

        /// <summary> Final URL for the processing log, for any links between different failure logs </summary>
        public static string Log_Files_URL
        {
            get { return log_files_url; }
        }

        /// <summary> Location where all the item-level static page exist for search engine indexing </summary>
        public static string Static_Pages_Location
        {
            get { return static_pages_location; }
        }

        /// <summary> List of all the incoming folders which should be checked for new resources </summary>
        public static List<SobekCM.Library.Builder.Builder_Source_Folder> Incoming_Folders
        {
            get { return incomingFolders; }
        }

        /// <summary> Network directory for the image server which holds all the resource files </summary>
        public static string Image_Server_Network
        {
            get { return image_server_network; }
        }

        /// <summary> Network directory for the SobekCM web application server </summary>
        public static string Application_Server_Network
        {
            get { return application_server_network; }
        }

        /// <summary> Primary URL for this instance of the SobekCM web application server </summary>
        public static string Application_Server_URL
        {
            get { return application_server_url; }
        }

        /// <summary> Location where the MarcXML feeds should be placed </summary>
        public static string MarcXML_Feed_Location
        {
            get { return marcxml_feed_location; }
        }

        /// <summary> Dropbox to check for any <a href="http://fclaweb.fcla.edu/FDA_landing_page">Florida Digital Archive</a> ingest reports  </summary>
        public static string FDA_Report_DropBox
        {
            get { return fda_report_dropbox; }
        }

        /// <summary> Drop box where packages can be placed to be archived locally </summary>
        public static string Archive_DropBox
        {
            get { return archive_dropbox; }
        }


        /// <summary> URL for the Solr/Lucene index for the document metadata and text </summary>
        public static string Document_Solr_Index_URL
        {
            get { return document_solr_url; }
        }

        /// <summary> URL for the Solr/Lucene index for the page text </summary>
        public static string Page_Solr_Index_URL
        {
            get { return page_solr_url; }
        }

        /// <summary> Flag indicates if the MARC feed should be built by default by the bulk loader </summary>
        public static bool Build_MARC_Feed_By_Default
        {
            get
            {
                return build_marc_feed_by_default;
            }
        }

        /// <summary> Email address for system errors </summary>
        public static string System_Error_Email
        {
            get { return system_error_email; }
        }

        /// <summary> Main email address for this system </summary>
        public static string System_Email
        {
            get { return system_email; }
        }

        /// <summary> Gets the Mango Union search base URL, in support of Florida SUS's</summary>
        public static string Mango_Union_Search_Base_URL
        {
            get { return mango_union_search_base_url; }
        }

        /// <summary> Gets the Mango Union search text to be displayed, in support of Florida SUS's</summary>
        public static string Mango_Union_Search_Text
        {
            get { return mango_union_search_text; }
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
            get { return caching_server; }
        }

        /// <summary> URL to the SobekCM Image Server, initially used just when features need to be drawn on images </summary>
        public static string SobekCM_ImageServer
        {
            get { return sobekcm_imageserver; }
        }

        /// <summary> Flag indicates if online submissions and edits can occur at the moment </summary>
        public static bool Online_Edit_Submit_Enabled
        {
            get { return canSubmit; }
        }


        ///// <summary> Gets the default email name to use when sending emails to users </summary>
        ///// <value> Currently this always returns 'University of Florida Digital Collections' </value>
        //public static string System_Source_Email_Description
        //{
        //    get { return "University of Florida Digital Collections"; }
        //}

        /// <summary> Gets the error web page to send users to when a catastrophic error occurs </summary>
        /// <value> For example, for UFDC this always returns 'http://ufdc.ufl.edu/error.html' </value>
        public static string System_Error_URL
        {
            get { return system_error_url; }
        }

        /// <summary> Gets the complete url to this instance of SobekCM library software </summary>
        /// <value> Currently this always returns 'http://ufdc.ufl.edu/' </value>
        public static string System_Base_URL
        {
            get { return system_base_url; }
        }

        /// <summary> Gets the abbrevation used to refer to this digital library </summary>
        public static string System_Abbreviation
        {
            get { return system_base_abbreviation; }
        }

        /// <summary> Gets the location that submission packets are built before being submitted into the regular
        /// digital resource location </summary>
        public static string In_Process_Submission_Location
        {
            get 
            {
                if (in_process_submission_location.Length > 0)
                {
                    return in_process_submission_location;
                }
                else
                {
                    return base_directory + "\\mySobek\\InProcess";
                }
            }
        }

        /// <summary> Relative location to the folders on the web server </summary>
        /// <remarks> This is only used when building static pages in the SobekCM Builder, which allows for
        /// the replacement of all the relative references ( i.e., '/design/skins/dloc/dloc.css') with the full
        /// link ( i.e., 'http://ufdc.ufl.edu/design/skins/dloc/dloc.css' ) </remarks>
        public static string Base_SobekCM_Location_Relative
        {
            get { return base_sobek_location_relative; }
            set { base_sobek_location_relative = value; }
        }

        /// <summary> URL to the Aware JPEG2000 zoomable image server </summary>
		public static string JP2_Server
		{
            get { return jpeg2000_server; }
		}

        /// <summary> Database connection string is built from the application config file </summary>
		public static string Database_Connection_String
		{
            get { return database_string; }
		}

        /// <summary> Base directory where the ASP.net application is running on the application server </summary>
		public static string Base_Directory
		{
            get { return base_directory; }
            set { base_directory = value; }
		}

        /// <summary> Gets the base URL for this instance, without the application name </summary>
        public static string Base_URL
        {
            get {   return base_url;    }
        }

        /// <summary> Base image URL for all digital resource images </summary>
        public static string Image_URL
		{
            get { return image_server_url; }
        }

        #region Methods which return the base directory or base url with a constant ending to indicate the SobekCM standard subfolders
        
        /// <summary> Directory for this application's myUFDC folder, where the template and project files reside for online submittal and editing</summary>
        /// <value> [Base_Directory] + 'myUFDC\' </value>
        public static string Base_MySobek_Directory
        {
            get { return base_directory + "mySobek\\"; }
        }

        /// <summary> Directory for this application's DATA folder, where the OAI source files reside </summary>
        /// <value> [Base_Dir] + 'data\' </value>
        public static string Base_Data_Directory
        {
            get { return base_directory + "data\\"; }
        }

        /// <summary> Directory for this application's TEMP folder, where some slow-changing data is stored in XML format </summary>
        /// <value> [Base_Dir] + 'temp\' </value>
        public static string Base_Temporary_Directory
        {
            get { return base_directory + "temp\\"; }
        }

        /// <summary> Directory for this application's DESIGN folder, where all the aggregation and interface folders reside </summary>
        /// <value> [Base_Directory] + 'design\' </value>
        public static string Base_Design_Location
        {
            get { return base_directory + "design\\"; }
        }

        #endregion

        #endregion
    }
}
