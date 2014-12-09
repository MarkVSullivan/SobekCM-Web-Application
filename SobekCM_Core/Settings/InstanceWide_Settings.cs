#region Using directives

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SobekCM.Core.Configuration;
using SobekCM.Core.Search;
using SobekCM.Core.Serialization;
using SobekCM.Core.Users;

#endregion

namespace SobekCM.Core.Settings
{
    /// <summary> Class provides context to constant settings based on the basic information about this instance of the application and server information </summary>
    [DataContract]
    public class InstanceWide_Settings : iSerializationEvents
    {
        /// TODO: Remove the database information from this object, since this will be passed to the web via REST


        private readonly Dictionary<int, Disposition_Option> dispositionLookup;
        private readonly Dictionary<string, Metadata_Search_Field> metadataFieldsByCode;
        private readonly Dictionary<short, Metadata_Search_Field> metadataFieldsByID;
        private readonly Dictionary<string, Metadata_Search_Field> metadataFieldsByFacetName;
        private readonly Dictionary<string, Metadata_Search_Field> metadataFieldsByDisplayName;
        private readonly Dictionary<string, Metadata_Search_Field> metadataFieldsByName;

        private string inProcessLocationOverride;

        /// <summary> constructor sets all the values to default empty strings </summary>
        public InstanceWide_Settings()
        {
            try
            {
                // Set some default values
                Database_Connections = new List<Database_Instance_Configuration>();

                Base_SobekCM_Location_Relative = String.Empty;
                JP2ServerUrl = String.Empty;
                JP2ServerType = String.Empty;
                Base_URL = String.Empty;
                Base_Directory = String.Empty;
                Image_URL = String.Empty;
                SobekCM_ImageServer = String.Empty;
                Online_Edit_Submit_Enabled = false;
                Caching_Server = String.Empty;
                Privacy_Email_Address = String.Empty;
                Metadata_Help_URL_Base = String.Empty;
                Help_URL_Base = String.Empty;
                Include_Partners_On_System_Home = false;
                Include_TreeView_On_System_Home = false;
                Default_UI_Language = Web_Language_Enum.English;
                Web_Output_Caching_Minutes = 0;
                Builder_Verbose_Flag = false;
                Upload_File_Types = String.Empty;
                Upload_Image_Types = String.Empty;
                Kakadu_JP2_Create_Command = String.Empty;
                OCR_Command_Prompt = String.Empty;
                Builder_Override_Seconds_Between_Polls = -1;
                Builder_Logs_Publish_Directory = String.Empty;
                Facets_Collapsible = false;
                Can_Remove_Single_Term = true;
                isHosted = false;

                // Define new empty collections
                dispositionLookup = new Dictionary<int, Disposition_Option>();
                Metadata_Search_Fields = new List<Metadata_Search_Field>();
                metadataFieldsByCode = new Dictionary<string, Metadata_Search_Field>();
                metadataFieldsByID = new Dictionary<short, Metadata_Search_Field>();
                metadataFieldsByFacetName = new Dictionary<string, Metadata_Search_Field>();
                metadataFieldsByDisplayName = new Dictionary<string, Metadata_Search_Field>();
                metadataFieldsByName = new Dictionary<string, Metadata_Search_Field>();
                Incoming_Folders = new List<Builder_Source_Folder>();
                Additional_Settings = new Dictionary<string, string>();
                Workflow_Types = new List<Workflow_Type>();
                Disposition_Options = new List<Disposition_Option>();
            }
            catch (Exception ee)
            {
                // Do nothing here
                Mango_Union_Search_Base_URL = ee.Message;
            }
        }

        /// <summary> Database connection string(s) built from the system config file (usually sits in a config subfolder)</summary>
        [DataMember]
        public List<Database_Instance_Configuration> Database_Connections { get; set; }

        #region Methods related to the workflow types and disposition types

        /// <summary> Gets the list of all possible workflows </summary>
        [DataMember]
        public List<Workflow_Type> Workflow_Types { get; set; }

        /// <summary> Gets the list of all possible dispositions </summary>
        [DataMember]
        public List<Disposition_Option> Disposition_Options { get; set; }

        /// <summary> Gets the disposition term, in past tense, by primary key </summary>
        /// <param name="DispositionID"> Primary key for the disposition term </param>
        /// <returns> Term in past tense, or "UNKNOWN" </returns>
        public string Disposition_Term_Past(int DispositionID)
        {
            return dispositionLookup.ContainsKey(DispositionID) ? dispositionLookup[DispositionID].Past : "UNKNOWN";
        }

        /// <summary> Gets the disposition term, in future tense, by primary key </summary>
        /// <param name="DispositionID"> Primary key for the disposition term </param>
        /// <returns> Term in future tense, or "UNKNOWN" </returns>
        public string Disposition_Term_Future(int DispositionID)
        {
            return dispositionLookup.ContainsKey(DispositionID) ? dispositionLookup[DispositionID].Future : "UNKNOWN";
        }

        /// <summary> Gets the primary key for a disposition type, by the past tense term </summary>
        /// <param name="Disposition_Term"> Disposition term, in past tense </param>
        /// <returns> Primary key for the matching disposition, or -1 </returns>
        public int Disposition_ID_Past(string Disposition_Term)
        {
            foreach (Disposition_Option thisOption in Disposition_Options)
            {
                if (String.Compare(thisOption.Past, Disposition_Term, StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    return thisOption.Key;
                }
            }
            return -1;
        }

        /// <summary> Gets the primary key for a disposition type, by the future tense term </summary>
        /// <param name="Disposition_Term"> Disposition term, in future tense </param>
        /// <returns> Primary key for the matching disposition, or -1 </returns>
        public int Disposition_ID_Future(string Disposition_Term)
        {
            foreach (Disposition_Option thisOption in Disposition_Options)
            {
                if (String.Compare(thisOption.Future, Disposition_Term, StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    return thisOption.Key;
                }
            }
            return -1;
        }

        #endregion

        #region Methods related to the metadata types (search fields)

        /// <summary> Gets the list of all metadata search fields </summary>
        [DataMember]
        public List<Metadata_Search_Field> Metadata_Search_Fields { get; set; }


        /// <summary> Gets a single metadata search field, by SobekCM web code </summary>
        /// <param name="SobekCM_Code"> Code for this metadata search field, employed within the URLs </param>
        /// <returns> Metadata search field, or else NULL </returns>
        public Metadata_Search_Field Metadata_Search_Field_By_Code(string SobekCM_Code)
        {
             return metadataFieldsByCode.ContainsKey(SobekCM_Code) ? metadataFieldsByCode[SobekCM_Code] : null;
        }

        /// <summary> Gets a single metadata search field, by the metadata column name </summary>
        /// <param name="Metadata_Name"> Name for this field and name of the column in the database </param>
        /// <returns> Metadata search field, or else NULL </returns>
        public Metadata_Search_Field Metadata_Search_Field_By_Name(string Metadata_Name)
        {
            return metadataFieldsByName.ContainsKey(Metadata_Name.Replace("_", " ").ToLower()) ? metadataFieldsByName[Metadata_Name.Replace("_", " ").ToLower()] : null;
        }

        /// <summary> Gets a single metadata search field, by the display name </summary>
        /// <param name="Display_Name"> Name for this field and name of the column in the database </param>
        /// <returns> Metadata search field, or else NULL </returns>
        public Metadata_Search_Field Metadata_Search_Field_By_Display_Name(string Display_Name)
        {
            return metadataFieldsByDisplayName.ContainsKey(Display_Name.Replace("_", " ").ToLower()) ? metadataFieldsByDisplayName[Display_Name.Replace("_", " ").ToLower()] : null;
        }

        /// <summary> Gets a single metadata search field, by the facet name </summary>
        /// <param name="Facet_Name"> Name for this field for faceting purposes </param>
        /// <returns> Metadata search field, or else NULL </returns>
        public Metadata_Search_Field Metadata_Search_Field_By_Facet_Name(string Facet_Name)
        {
            return metadataFieldsByFacetName.ContainsKey(Facet_Name.Replace("_", " ").ToLower()) ? metadataFieldsByFacetName[Facet_Name.Replace("_", " ").ToLower()] : null;
        }


        /// <summary> Gets a single metadata search field, by primary identifiere </summary>
        /// <param name="MetadataTypeID"> Primary identifier for the metadata search field </param>
        /// <returns> Metadata search field, or else NULL </returns>
        public Metadata_Search_Field Metadata_Search_Field_By_ID(short MetadataTypeID)
        {
            return metadataFieldsByID.ContainsKey(MetadataTypeID) ? metadataFieldsByID[MetadataTypeID] : null;
        }

        #endregion

        #region Public Properties

        /// <summary> Returns the default user interface language </summary>
        [DataMember]
        public Web_Language_Enum Default_UI_Language { get; set; }

        /// <summary> Set the default UI language, by passing in a string </summary>
        public string Default_UI_Language_String
        {
            set
            {
                Default_UI_Language = Web_Language_Enum_Converter.Code_To_Enum(Web_Language_Enum_Converter.Name_To_Code(value));
            }
            get { return Web_Language_Enum_Converter.Enum_To_Name(Default_UI_Language); }
        }

        /// <summary> Gets the base name for this system </summary>
        [DataMember]
        public string System_Name { get; set; }

        /// <summary> Email address used when a possible privacy issue appears, such as an apaprent social security number </summary>
        [DataMember]
        public string Privacy_Email_Address { get; set; }

        /// <summary> Flag indicates if the partners browse should be displayed on the home page </summary>
        [DataMember]
        public bool Include_Partners_On_System_Home { get; set; }

        /// <summary> Flag indicates if the tree view should be displayed on the home page </summary>
        [DataMember]
        public bool Include_TreeView_On_System_Home { get; set; }

        /// <summary> Flag indicates if the builder should try to convert office files (Word and Powerpoint) to PDF during load and post-processing </summary>
        [DataMember]
        public bool Convert_Office_Files_To_PDF { get; set; }

        /// <summary> Kakadu JPEG2000 script will override the specifications used when creating zoomable images </summary>
        [DataMember]
        public string Kakadu_JP2_Create_Command { get; set; }

        /// <summary> Command to launch the OCR engine against a single TIFF to produce a single TEXT file </summary>
        [DataMember]
        public string OCR_Command_Prompt { get; set; }

        /// <summary> Flag indicates if the statistics information should be cached for very quick 
        /// retrieval for search engine robots. </summary>
        [DataMember]
        public bool Statistics_Caching_Enabled { get; set; }

        /// <summary> Returns the base string for the resource identifiers within OAI.</summary>
        /// <remarks> This indicates the repository from which the material is pulled usually</remarks>
        [DataMember]
        public string OAI_Resource_Identifier_Base { get; set; }

        /// <summary> Returns the OAI repository identifier, which is usually the system abbreviation </summary>
        [DataMember]
        public string OAI_Repository_Identifier { get; set; }

        /// <summary> Returns the OAI repository name </summary>
        [DataMember]
        public string OAI_Repository_Name { get; set; }

        /// <summary> Gets the regular expression for matching files names to delete BEFORE archiving
        /// incoming digital resource files </summary>
        [DataMember]
        public string PreArchive_Files_To_Delete { get; set; }

        /// <summary> Gets the regular expression for matching files names to delete AFTER archiving
        /// incoming digital resource files </summary>
        [DataMember]
        public string PostArchive_Files_To_Delete { get; set; }

        /// <summary> Gets regular expression for matching file names (with extension) to exclude 
        /// from automatically adding gto the downloads for incoming digital resources </summary>
        [DataMember]
        public string Files_To_Exclude_From_Downloads { get; set; }

        /// <summary> Gets the library-wide setting for width of created jpeg derivatives </summary>
        [DataMember]
        public int JPEG_Width { get; set; }

        /// <summary> Gets the library-wide setting for height created jpeg derivatives </summary>
        [DataMember]
        public int JPEG_Height { get; set; }

        /// <summary> Gets the library-wide setting for width for created jpeg thumbnails </summary>
        [DataMember]
        public int Thumbnail_Width { get; set; }

        /// <summary> Gets the library-wide setting for height for created jpeg thumbnails </summary>
        [DataMember]
        public int Thumbnail_Height { get; set; }

        /// <summary> Directory where the local logs are written </summary>
        /// <remarks> This is determined the first time this class is referenced and is just
        /// a <b>Logs</b> subfolder uynder the application startup path</remarks>
        [DataMember]
        public string Local_Log_Directory { get; set; }

        /// <summary> Folder where files bound for archiving are placed </summary>
        [DataMember]
        public string Package_Archival_Folder { get; set; }

        /// <summary> Final destination of the processing log (usually the web server) </summary>
        [DataMember]
        public string Log_Files_Directory { get; set; }

        /// <summary> Final URL for the processing log, for any links between different failure logs </summary>
        [DataMember]
        public string Log_Files_URL { get; set; }

        /// <summary> Location where all the item-level page exist for search engine indexing </summary>
        [DataMember]
        public string Static_Pages_Location { get; set; }

        /// <summary> List of all the incoming folders which should be checked for new resources </summary>
        [DataMember]
        public List<Builder_Source_Folder> Incoming_Folders { get; set; }

        /// <summary> Network directory for the image server which holds all the resource files </summary>
        [DataMember]
        public string Image_Server_Network { get; set; }

        /// <summary> Network directory for the SobekCM web application server </summary>
        [DataMember]
        public string Application_Server_Network { get; set; }

        /// <summary> Primary URL for this instance of the SobekCM web application server </summary>
        [DataMember]
        public string Application_Server_URL { get; set; }

        /// <summary> Location where the MarcXML feeds should be placed </summary>
        [DataMember]
        public string MarcXML_Feed_Location { get; set; }

        /// <summary> Dropbox to check for any <a href="http://fclaweb.fcla.edu/FDA_landing_page">Florida Digital Archive</a> ingest reports </summary>
        [DataMember]
        public string FDA_Report_DropBox { get; set; }

        /// <summary> Drop box where packages can be placed to be archived locally </summary>
        [DataMember]
        public string Archive_DropBox { get; set; }

        /// <summary> URL for the Solr/Lucene index for the document metadata and text </summary>
        [DataMember]
        public string Document_Solr_Index_URL { get; set; }

        /// <summary> URL for the Solr/Lucene index for the page text </summary>
        [DataMember]
        public string Page_Solr_Index_URL { get; set; }

        /// <summary> Flag indicates if the MARC feed should be built by default by the bulk loader </summary>
        [DataMember]
        public bool Build_MARC_Feed_By_Default { get; set; }

        /// <summary> Email address for system errors </summary>
        [DataMember]
        public string System_Error_Email { get; set; }

        /// <summary> Main email address for this system </summary>
        [DataMember]
        public string System_Email { get; set; }

        /// <summary> Gets the Mango Union search base URL, in support of Florida SUS's</summary>
        [DataMember]
        public string Mango_Union_Search_Base_URL { get; set; }

        /// <summary> Gets the Mango Union search text to be displayed, in support of Florida SUS's</summary>
        [DataMember]
        public string Mango_Union_Search_Text { get; set; }

        /// <summary> Flag indicates whether the facets should be pulled during a browse </summary>
        [DataMember]
        public bool Pull_Facets_On_Browse { get; set; }

        /// <summary> Flag indicates whether the facets should be pulled during a search </summary>
        [DataMember]
        public bool Pull_Facets_On_Search { get; set; }

        /// <summary> Name of the caching server (or blank if disabled) </summary>
        [DataMember]
        public string Caching_Server { get; set; }

        /// <summary> URL to the SobekCM Image Server, initially used just when features need to be drawn on images </summary>
        [DataMember]
        public string SobekCM_ImageServer { get; set; }

        /// <summary> Flag indicates if online submissions and edits can occur at the moment </summary>
        [DataMember]
        public bool Online_Edit_Submit_Enabled { get; set; }

        /// <summary> Gets the error web page to send users to when a catastrophic error occurs </summary>
        /// <value> For example, for UFDC this always returns 'http://ufdc.ufl.edu/error.html' </value>
        [DataMember]
        public string System_Error_URL { get; set; }

        /// <summary> Gets the complete url to this instance of SobekCM library software </summary>
        /// <value> Currently this always returns 'http://ufdc.ufl.edu/' </value>
        [DataMember]
        public string System_Base_URL { get; set; }

        /// <summary> Gets the abbrevation used to refer to this digital library </summary>
        [DataMember]
        public string System_Abbreviation { get; set; }

        /// <summary> Relative location to the folders on the web server </summary>
        /// <remarks> This is only used when building pages in the SobekCM Builder, which allows for
        /// the replacement of all the relative references ( i.e., '/design/skins/dloc/dloc.css') with the full
        /// link ( i.e., 'http://example.edu/design/skins/dloc/dloc.css' ) </remarks>
        [DataMember]
        public string Base_SobekCM_Location_Relative { get; set; }

        /// <summary> URL to the Aware JPEG2000 zoomable image server </summary>
        [DataMember]
        public string JP2ServerUrl { get; set; }

        /// <summary> Gets the TYPE of the JPEG2000 server - allowing the system to support different
        /// types of the zoomable server ( i.e., Aware, Djatoka, etc.. ) </summary>
        [DataMember]
        public string JP2ServerType { get; set; }

        /// <summary> Number of seconds between polls, from the configuration file (not the database) </summary>
        /// <remarks> This is used if the SobekCM Builder is working between multiple instances. If the SobekCM
        /// Builder is only servicing a single instance, then the data can be pulled from the database. </remarks>
        [DataMember]
        public int Builder_Override_Seconds_Between_Polls { get; set; }

        /// <summary> Base directory where the ASP.net application is running on the application server </summary>
        [DataMember]
        public string Base_Directory { get; set; }

        /// <summary> Gets the base URL for this instance, without the application name </summary>
        [DataMember]
        public string Base_URL { get; set; }

        /// <summary> Base image URL for all digital resource images </summary>
        [DataMember]
        public string Image_URL { get; set; }

        /// <summary> Number of seconds the builder waits between polls </summary>
        [DataMember]
        public int Builder_Seconds_Between_Polls { get; set; }

        /// <summary> Number of days builder logs remain before the builder will try to delete it </summary>
        [DataMember]
        public int Builder_Log_Expiration_Days { get; set; }

        /// <summary> Flag indicates if the builder should be extra verbose in the log files (used for debugging purposes mostly) </summary>
        [DataMember]
        public bool Builder_Verbose_Flag { get; set; }

        /// <summary> Number of minutes clients are suggested to cache the web output </summary>
        [DataMember]
        public int Web_Output_Caching_Minutes { get; set; }

        /// <summary> ImageMagick executable file </summary>
        [DataMember]
        public string ImageMagick_Executable { get; set; }

        /// <summary> Ghostscript executable file </summary>
        [DataMember]
        public string Ghostscript_Executable { get; set; }

        /// <summary> Returns the network location for the main builder, which runs essentially
        /// without restrictions </summary>
        [DataMember]
        public string Main_Builder_Input_Folder { get; set; }

        /// <summary> IP address for the SobekCM web server </summary>
        [DataMember]
        public string SobekCM_Web_Server_IP { get; set; }

        /// <summary> List of file type extensions which can be uploaded in the
        /// file management interface. These should all treated as downloads in the system. </summary>
        [DataMember]
        public string Upload_File_Types { get; set; }

        /// <summary> List of file type extensions which can be uploaded in the page
        /// image upload interface. These should all be treated as page files in the system </summary>
        [DataMember]
        public string Upload_Image_Types { get; set; }

        /// <summary> Additional custom settings associated with this SobekCM system at
        /// the highest level </summary>
        [DataMember]
        public Dictionary<string, string> Additional_Settings { get; set; }

        /// <summary> Directory where the builder should publish log to, before sleeping between
        /// actions </summary>
        [DataMember]
        public string Builder_Logs_Publish_Directory { get; set; }

        /// <summary> Flag indicates if facets start out collapsed </summary>
        [DataMember]
        public bool Facets_Collapsible { get; set; }

        /// <summary> Flag indicates if users can remove a single part of their search term </summary>
        [DataMember]
        public bool Can_Remove_Single_Term { get; set; }

        /// <summary> Flag determines if the detailed view of user permissions for items in an aggregation should show </summary>
        [DataMember]
        public bool Detailed_User_Aggregation_Permissions { get; set; }

        /// <summary> Flag indicates if this is a 'hosted' solution of SobekCM, in which case
        /// certain fields should not be made available, even to "system admins" </summary>
        [DataMember]
        public bool isHosted { get; set; }

        /// <summary> List of possible page image extensions </summary>
        [DataMember]
        public List<string> Page_Image_Extensions { get; set; }

        /// <summary> Name for the backup files folder within each digital resource </summary>
        [DataMember]
        public string Backup_Files_Folder_Name { get; set; }

        /// <summary> Current version number associated with this SobekCM digital repository web application </summary>
        [DataMember]
        public string Current_Web_Version { get; set; }

        /// <summary> Current version number associated with this SobekCM builder application </summary>
        [DataMember]
        public string Current_Builder_Version { get; set; }

        /// <summary> Number of ticks that a complete package must age before being processed </summary>
        /// <value> This is currently set to 15 minutes (in ticks) </value>
        [DataMember]
        public long Complete_Package_Required_Aging { get; set; }

        /// <summary> Number of ticks that a metadata only package must age before being processed </summary>
        /// <value> This is currently set to 1 minute (in ticks) </value>
        [DataMember]
        public long METS_Only_Package_Required_Aging { get; set; }

        /// <summary> Flag indicates whether checksums should be verified </summary>
        [DataMember]
        public bool VerifyCheckSum { get; set; }

        /// <summary> Gets the list of reserved keywords that cannot be used
        /// for aggregation codes or aggregation aliases </summary>
        /// <remarks> These are all lower case </remarks>
        [DataMember]
        public List<string> Reserved_Keywords { get; set; }

        /// <summary> Flag indicates if the florida SUS settings should be included </summary>
        [DataMember]
        public bool Show_Florida_SUS_Settings { get; set; } 

        #endregion

        #region Derivative properties which return the base directory or base url with a constant ending to indicate the SobekCM standard subfolders

        /// <summary> Directory for this application's mySobek folder, where the template and project files reside for online submittal and editing</summary>
        /// <value> [Base_Directory] + 'mySobek\' </value>
        public string Base_MySobek_Directory
        {
            get { return Base_Directory + "mySobek\\"; }
        }

        /// <summary> Directory for this application's DATA folder, where the OAI source files reside </summary>
        /// <value> [Base_Dir] + 'data\' </value>
        public string Base_Data_Directory
        {
            get { return Base_Directory + "data\\"; }
        }

        /// <summary> Directory for this application's TEMP folder, where some slow-changing data is stored in XML format </summary>
        /// <value> [Base_Dir] + 'temp\' </value>
        public string Base_Temporary_Directory
        {
            get { return Base_Directory + "temp\\"; }
        }

        /// <summary> Directory for this application's DESIGN folder, where all the aggregation and interface folders reside </summary>
        /// <value> [Base_Directory] + 'design\' </value>
        public string Base_Design_Location
        {
            get { return Base_Directory + "design\\"; }
        }

        /// <summary> Gets the location that submission packets are built before being submitted into the regular
        /// digital resource location </summary>
        public string In_Process_Submission_Location
        {
            get
            {
                if (String.IsNullOrEmpty(inProcessLocationOverride))
                    return Base_Directory + "\\mySobek\\InProcess";
                return inProcessLocationOverride;
            }
            set { inProcessLocationOverride = value; }
        }

        /// <summary> Network location of the recycle bin, where deleted items and
        /// files are placed for a while, in case of accidental deletion </summary>
        public string Recycle_Bin
        {
            get { return Image_Server_Network + "\\RECYCLE BIN"; }
        }

        #endregion

        #region Properties and derivative methods about the help page URLs

        /// <summary> Base URL for the metadata help </summary>
        [DataMember]
        public string Metadata_Help_URL_Base { get; set; }

        /// <summary> Base URL for most the help pages </summary>
        [DataMember]
        public string Help_URL_Base { get; set; }

        /// <summary> Get the URL for all metadata help pages which are used when users request 
        /// help while submitting a new item or editing an existing item </summary>
        /// <param name="Current_Base_URL"> Current base url for the current user's request </param>
        /// <returns> Base URL to use for the metadata help page links </returns>
        public string Metadata_Help_URL(string Current_Base_URL)
        {
            return Metadata_Help_URL_Base.Length > 0 ? Help_URL_Base : Current_Base_URL;
        }

        /// <summary> URL used for the main help pages about this system's basic functionality </summary>
        /// <param name="Current_Base_URL"> Current base url for the current user's request </param>
        /// <returns> Base URL to use for the main help page links </returns>
        public string Help_URL(string Current_Base_URL)
        {
            return Help_URL_Base.Length > 0 ? Help_URL_Base : Current_Base_URL;
        }

        #endregion


        public Shibboleth_Configuration Shibboleth { get; set; }

        /// <summary> Gets the user-in-process directory </summary>
        /// <param name="CurrentUser"> Current user, for which to find the in-process directory </param>
        /// <param name="DirectoryName"> Subdirectory requested </param>
        /// <returns> Full path to the requested user-in-process directory </returns>
        public string User_InProcess_Directory(User_Object CurrentUser, string DirectoryName)
        {
            // Determine the in process directory for this
            string userInProcessDirectory = In_Process_Submission_Location + "\\" + CurrentUser.UserName.Replace(".", "").Replace("@", "") + "\\" + DirectoryName;
            if (CurrentUser.ShibbID.Trim().Length > 0)
                userInProcessDirectory = In_Process_Submission_Location + "\\" + CurrentUser.ShibbID + "\\" + DirectoryName;

            return userInProcessDirectory;
        }

        public void PostUnSerialization()
        {
            // Populate the dictionaries for looking up metadata search fields by code, id, name, etc..
            metadataFieldsByCode.Clear();
            metadataFieldsByID.Clear();
            metadataFieldsByName.Clear();
            metadataFieldsByDisplayName.Clear();
            metadataFieldsByFacetName.Clear();

            // Now add the rest of the fields
            foreach (Metadata_Search_Field thisField in Metadata_Search_Fields)
            {
                // Add this to the collections
                if (thisField.ID >= -1)
                    metadataFieldsByID[thisField.ID] = thisField;

                if (thisField.Web_Code.Length > 0)
                {
                    metadataFieldsByCode[thisField.Web_Code] = thisField;
                    metadataFieldsByName[thisField.Name.ToLower().Replace("_", " ")] = thisField;
                    metadataFieldsByDisplayName[thisField.Display_Term.ToLower().Replace("_", " ")] = thisField;

                    if (thisField.Facet_Term.Length > 0)
                    {
                        metadataFieldsByFacetName[thisField.Facet_Term.Replace("_", " ").ToLower()] = thisField;
                    }
                }
            }

            // Fill up the disposition lookup object
            dispositionLookup.Clear();
            foreach (Disposition_Option dispOption in Disposition_Options)
            {
                dispositionLookup[dispOption.Key] = dispOption;
            }
        }
    }
}
