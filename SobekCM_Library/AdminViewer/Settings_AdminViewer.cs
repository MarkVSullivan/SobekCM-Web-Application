#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using SobekCM.Library.Application_State;
using SobekCM.Library.Configuration;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.Navigation;
using SobekCM.Library.Users;

#endregion

namespace SobekCM.Library.AdminViewer
{
    /// <summary> Class allows an authenticated system admin to view and edit the library-wide system settings in this library </summary>
    /// <remarks> This class extends the <see cref="abstract_AdminViewer"/> class.<br /><br />
    /// MySobek Viewers are used for registration and authentication with mySobek, as well as performing any task which requires
    /// authentication, such as online submittal, metadata editing, and system administrative tasks.<br /><br />
    /// During a valid html request, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="Navigation.SobekCM_Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  Since this action requires authentication, an instance of the  <see cref="MySobek_HtmlSubwriter"/> class is created. </li>
    /// <li>The mySobek subwriter creates an instance of this viewer to show the library-wide system settings in this digital library</li>
    /// </ul></remarks>
    class Settings_AdminViewer : abstract_AdminViewer
    {
        private readonly string actionMessage;
        private readonly StringBuilder errorBuilder;
        private readonly Dictionary<int, Setting_Info> idToSetting;
        private bool isValid;
        private int settingCounter;
        private readonly Dictionary<string, string> settings;

        /// <summary> Constructor for a new instance of the Thematic_Headings_AdminViewer class </summary>
        /// <param name="User"> Authenticated user information </param>
        /// <param name="Current_Mode">  Mode / navigation information for the current request </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> Postback from handling saving the new settings is handled here in the constructor </remarks>
        public Settings_AdminViewer(User_Object User, SobekCM_Navigation_Object Current_Mode, Custom_Tracer Tracer) : base(User)
        {
            // If the user cannot edit this, go back
            if (!User.Is_System_Admin)
            {
                currentMode.Mode = Display_Mode_Enum.My_Sobek;
                currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                currentMode.Redirect();
            }

            // Establish some default, starting values
            idToSetting = new Dictionary<int, Setting_Info>();
            settingCounter = 1;
            actionMessage = String.Empty;

            // Get the current settings from the database
            settings = SobekCM_Database.Get_Settings(Tracer);
            
            // Get the default URL and default system location
            string default_url = Current_Mode.Base_URL;
            string default_location = HttpContext.Current.Request.PhysicalApplicationPath;

            // NOTE: This code is exactly the same as found in the SobekCM_Configuration tool.  That is why
            // this feels a little strange, to be loading information about all the settings, and then handling
            // the display portion late.  This allows the settings to be added or subtracted from rather easily 
            // in code and also keeps the exact same code chunk to keep the configuration tool and the web 
            // app both kept current simultaneously.
            string[] empty_options = new string[] { };
            string[] boolean_options = new[] { "true", "false" };
            string[] language_options = Web_Language_Enum_Converter.Language_Name_Array;
            Add_Setting_UI("Allow Page Image File Management", 70, boolean_options, "This option allows or disallows adding page images through the online web interface.  If it is set to false, new pages being added to an existing item must be sent through the bulk loader process.", false );
            Add_Setting_UI("Application Server Network", -1, empty_options, "Server share for the web application's network location.\n\nExample: '\\\\lib-sandbox\\Production\\'", false, default_location);
            Add_Setting_UI("Application Server URL", -1, empty_options, "Base URL which points to the web application.\n\nExamples: 'http://localhost/sobekcm/', 'http://ufdc.ufl.edu/', etc..", false, default_url );
            Add_Setting_UI("Archive DropBox", -1, empty_options, "Network location for the archive drop box.  If this is set to a value, the builder/bulk loader will place a copy of the package in this folder for archiving purposes.  This folder is where any of your archiving processes should look for new packages.", false );
            Add_Setting_UI("Builder Log Expiration in Days", 200, new[] { "10", "30", "365", "99999" }, "Number of days the SobekCM Builder logs are retained.", false, "10");
            Add_Setting_UI("Builder Operation Flag", 200, new[] { "STANDARD OPERATION", "PAUSE REQUESTED", "ABORT REQUESTED", "NO BUILDER REQUESTED" }, "Last flag set when the builder/bulk loader ran.", false );
            Add_Setting_UI("Builder Seconds Between Polls", 200, new[] { "15", "60", "300", "600" }, "Number of seconds the builder remains idle before checking for new incoming package again.", false, "60");
            Add_Setting_UI("Caching Server", -1, empty_options, "URL for the AppFabric Cache host machine, if a caching server/cluster is in use in this system.", false );
            Add_Setting_UI("Can Submit Edit Online", 70, boolean_options, "Flag dictates if users can submit items online, or if this is disabled in this system.", false );
            Add_Setting_UI("Convert Office Files to PDF", 70, boolean_options, "Flag dictates if users can submit items online, or if this is disabled in this system.", false, "false");
            Add_Setting_UI("Create MARC Feed By Default", 70, boolean_options, "Flag indicates if the builder/bulk loader should create the MARC feed by default when operating in background mode.", false );
            Add_Config_Setting("Database Type",  SobekCM_Library_Settings.Database_Type_String , "Type of database used to drive the SobekCM system.\n\nCurrently, only Microsoft SQL Server is allowed with plans to add PostgreSQL to the supported database system.\n\nThis value resides in the configuration on the web server.  See your database and web server administrator to change this value.");
            Add_Config_Setting("Database Connection String", SobekCM_Library_Settings.Database_Connection_String, "Connection string used to connect to the SobekCM database\n\nThis value resides in the configuration file on the web server.  See your database and web server administrator to change this value.");
            Add_Setting_UI("Document Solr Index URL", -1, empty_options, "URL for the document-level solr index.\n\nExample: 'http://localhost:8080/documents'", false );
            Add_Config_Setting("Error Emails", SobekCM_Library_Settings.System_Error_Email, "Email address for the web application to mail for any errors encountered while executing requests.\n\nThis account will be notified of inabilities to connect to servers, potential attacks, missing files, etc..\n\nIf the system is able to connect to the database, the 'System Error Email' address listed there, if there is one, will be used instead.\n\nUse a semi-colon betwen email addresses if multiple addresses are included.\n\nExample: 'person1@corp.edu;person2@corp2.edu'.\n\nThis value resides in the web.config file on the web server.  See your web server administrator to change this value.");
            Add_Config_Setting("Error HTML Page", SobekCM_Library_Settings.System_Error_URL, "Static page the user should be redirected towards if an unexpected exception occurs which cannot be handled by the web application.\n\nExample: 'http://ufdc.ufl.edu/error.html'.\n\nThis value resides in the web.config file on the web server.  See your web server administrator to change this value.");
            Add_Setting_UI("FDA Report DropBox", -1, empty_options, "Location for the builder/bulk loader to look for incoming Florida Dark Archive XML reports to process and add to the history of digital resources.", true );
            Add_Setting_UI("Files To Exclude From Downloads", -1, empty_options, "Regular expressions used to exclude files from being added by default to the downloads of resources.\n\nExample: '((.*?)\\.(jpg|tif|jp2|jpx|bmp|jpeg|gif|png|txt|pro|mets|db|xml|bak|job)$|qc_error.html)'", false );
            Add_Setting_UI("Help URL", -1, empty_options, "URL used for the main help pages about this system's basic functionality.\n\nExample (and default): 'http://ufdc.ufl.edu/'", false );
            Add_Setting_UI("Help Metadata URL", -1, empty_options, "URL used for the help pages when users request help on metadata elements during online submit and editing.\n\nExample (and default): 'http://ufdc.ufl.edu/'", false );
            Add_Setting_UI("Image Server Network", -1, empty_options, "Network location to the content for all of the digital resources (images, metadata, etc.).\n\nExample: 'C:\\inetpub\\wwwroot\\UFDC Web\\SobekCM\\content\\' or '\\\\ufdc-images\\content\\'", false, default_location + "content\\");
            Add_Setting_UI("Image Server URL", -1, empty_options, "URL which points to the digital resource images.\n\nExample: 'http://localhost/sobekcm/content/' or 'http://ufdcimages.uflib.ufl.edu/'", false, default_url + "content/");
            Add_Setting_UI("Include Partners On System Home", 70, boolean_options, "This option controls whether a PARTNERS option appears on the main system home page, assuming there are multiple institutional aggregations.", false, "false" );
            Add_Setting_UI("Include TreeView On System Home", 70, boolean_options, "This option controls whether a TREE VIEW option appears on the main system home page which displays all the active aggregations hierarchically in a tree view.", false, "false" );
            Add_Setting_UI("JPEG Height", 60, empty_options, "Restriction on the size of the jpeg page images' height (in pixels) when generated automatically by the builder/bulk loader.\n\nDefault: '1000'", false );
            Add_Setting_UI("JPEG Width", 60, empty_options, "Restriction on the size of the jpeg page images' width (in pixels) when generated automatically by the builder/bulk loader.\n\nDefault: '630'", false );
            Add_Setting_UI("JPEG2000 Server", -1, empty_options, "URL for the Aware JPEG2000 Server for displaying JPEG2000 images.", false );
            Add_Setting_UI("Kakadu JP2 Command", -1, empty_options, "Kakadu JPEG2000 script will override the specifications used when creating zoomable images.\n\nIf this is blank, the default specifications will be used which match those used by the National Digital Newspaper Program and University of Florida Digital Collections.", false );
            Add_Setting_UI("Log Files Directory", -1, empty_options, "Network location for the share within which the builder/bulk loader logs should be copied to become web accessible.\n\nExample: '\\\\lib-sandbox\\Design\\extra\\logs\\'", false, default_location + "design\\extra\\logs\\");
            Add_Setting_UI("Log Files URL", -1, empty_options, "URL for the builder/bulk loader logs files.\n\nExample: 'http://ufdc.ufl.edu/design/extra/logs/'", false, default_url + "design/exra/logs/");
            Add_Setting_UI("Mango Union Search Base URL", -1, empty_options, "Florida SUS state-wide catalog base URL for determining the number of physical holdings which match a given search.\n\nExample: 'http://solrcits.fcla.edu/citsZ.jsp?type=search&base=uf'", true );
            Add_Setting_UI("Mango Union Search Text", -1, empty_options, "Text to display the number of hits found in the Florida SUS-wide catalog.\n\nUse the value '%1' in the string where the number of hits should be inserted.\n\nExample: '%1 matches found in the statewide catalog'", true );
            Add_Setting_UI("MarcXML Feed Location", -1, empty_options, "Network location or share where any geneated MarcXML feed should be written.\n\nExample: '\\\\lib-sandbox\\Data\\'", false, default_location + "data\\" );
            Add_Setting_UI("OAI Repository Identifier", 200, empty_options, "Identification for this repository when serving the OAI-PMH feeds.\n\nThis appears when a as 'Repository Identifier' under the OAI-Identifier when a user identifies the OAI-PMH repository.\n\nExamples: 'ufdc', 'sobekcm', 'dloc', etc..", false );
            Add_Setting_UI("OAI Repository Name", -1, empty_options, "Complete name for this repository when serving the OAI-PMH feeds.\n\nThis appears when a as 'Repository Name' when a user identifies the OAI-PMH repository.\n\nExamples: 'University of Florida Digital Collections', 'Digital Library of the Caribbean', etc..", false );
            Add_Setting_UI("OAI Resource Identifier Base", 200, empty_options, "Base identifier used for all resources found within this repository when serving them via OAI-PMH.\n\nThe complete item identifier begins with this base and then adds the bibliographic identifier (BibID) to complete the unique resource identifier.\n\nExamples: 'oai:sobekcm:', 'oai:www.uflib.ufl.edu.ufdc:', etc..", false );
            Add_Setting_UI("OCR Engine Command", -1, empty_options, "If you wish to utilize an OCR engine in the builder/bulk loader, add the command-line call to the engine here.\n\nUse %1 as a place holder for the ingoing image file name and %2 as a placeholder for the output text file name.\n\nExample: 'C:\\OCR\\Engine.exe -in %1 -out %2'", false );
            Add_Setting_UI("Page Solr Index URL", -1, empty_options, "URL for the resource-level solr index used when searching for matching pages within a single document.\n\nExample: 'http://localhost:8080/pages'", false );
            Add_Setting_UI("PostArchive Files To Delete", -1, empty_options, "Regular expression indicates which files should be deleted AFTER being archived by the builder/bulk loader.\n\nExample: '(.*?)\\.(tif)'", false );
            Add_Setting_UI("PreArchive Files To Delete", -1, empty_options, "Regular expression indicates which files should be deleted BEFORE being archived by the builder/bulk loader.\n\nExample: '(.*?)\\.(QC.jpg)'", false );
            Add_Setting_UI("Privacy Email Address", -1, empty_options, "Email address which receives notification if personal information (such as Social Security Numbers) is potentially found while loading or post-processing an item.\n\nIf you are using multiple email addresses, seperate them with a semi-colon.\n\nExample: 'person1@corp.edu;person2@corp.edu'", false );
            Add_Setting_UI("Shibboleth System Name", 200, empty_options, "Local name of the Shibboleth authentication system.\n\nExamples: 'GatorLink', 'PantherSoft', etc..", false );
            Add_Setting_UI("Shibboleth System URL", -1, empty_options, "URL for the Shibboleth authentication process.", false );
            Add_Setting_UI("Show Florida SUS Settings", 70, boolean_options, "Some system settings are only applicable to institutions which are part of the Florida State University System.  Setting this value to TRUE will show these settings, while FALSE will suppress them.\n\nIf this value is changed, you willl need to save the settings for it to reload and reflect the change.", false, "false");
            Add_Setting_UI("SobekCM Image Server", -1, empty_options, "URL for the SobekCM Image Server application which overlays JPEGs and JPEG2000s with highlighting on regions of interest.", false );
            Add_Setting_UI("Static Pages Location", -1, empty_options, "Location where the static files are located for providing the full citation and text for indexing, either on the same server as the web application or as a network share.\n\nIt is recommended that these files be on the same server as the web server, rather than remote storage, to increase the speed in which requests from search engine indexers can be fulfilled.\n\nExample: 'C:\\inetpub\\wwwroot\\UFDC Web\\SobekCM\\data\\'.", false, default_location + "data\\");
            Add_Setting_UI("Statistics Caching Enabled", 70, boolean_options, "Flag indicates if the basic usage and item count information should be cached for up to 24 hours as static XML files written in the web server's temp directory.\n\nThis should be enabled if your library is quite large as it can take a fair amount of time to retrieve this information and these screens are made available for search engine index robots for indexing.", false );
            Add_Setting_UI("System Base Abbreviation", 100, empty_options, "Base abbreviation to be used when the system refers to itself to the user, such as the main tabs to take a user to the home pages.\n\nThis abbreviation should be kept as short as possible.\n\nExamples: 'UFDC', 'dLOC', 'Sobek', etc..", false );
            Add_Setting_UI("System Base Name", -1, empty_options, "Overall name of the system, to be used when creating MARC records and in several other locations.", false );
            Add_Setting_UI("System Base URL", -1, empty_options, "Base URL which points to the web application.\n\nExamples: 'http://localhost/sobekcm/', 'http://ufdc.ufl.edu/', etc..", false, default_url);
            Add_Setting_UI("System Default Language", 150, language_options, "Default system user interface language.  If the user's HTML request does not include a language supported by the interface or which does not include specific translations for a field, this default language is utilized.", false, "English");
            Add_Setting_UI("System Email", -1, empty_options, "Default email address for the system, which is sent emails when users opt to contact the administrators.\n\nThis can be changed for individual aggregations but at least one email is required for the overall system.\n\nIf you are using multiple email addresses, seperate them with a semi-colon.\n\nExample: 'person1@corp.edu;person2@corp.edu'", false );
            Add_Setting_UI("System Error Email", -1, empty_options, "Email address used when a critical system error occurs which may require investigation or correction.\n\nIf you are using multiple email addresses, seperate them with a semi-colon.\n\nExample: 'person1@corp.edu;person2@corp.edu'", false );
            Add_Setting_UI("Thumbnail Height", 60, empty_options, "Restriction on the size of the page image thumbnails' height (in pixels) when generated automatically by the builder/bulk loader.\n\nDefault: '300'", false );
            Add_Setting_UI("Thumbnail Width", 60, empty_options, "Restriction on the size of the page image thumbnails' width (in pixels) when generated automatically by the builder/bulk loader.\n\nDefault: '150'", false );
            Add_Setting_UI("Web In Process Submission Location", -1, empty_options, "Location where packages are built by users during online submissions and metadata updates.\n\nThis generally needs to be on the web server and have appropriate access for read/write.\n\nIf nothing is indicated in this field, the system will automatically use the 'mySobek\\InProcess' subfolder under the web application.", false, default_location + "mySobek\\InProcess\\" );
            Add_Setting_UI("Web Output Caching Minutes", 200, new[] { "0", "1", "2", "3", "5", "10", "15" }, "This setting controls how long the client's browser is instructed to cache the served web page.\n\nSetting this value higher removes the round-trip when requesting a recently requested page.  It also means that some changes may not be reflected until the refresh button is pressed.\n\nIn general, this setting is only applied to public-style pages, and not personalized pages, such as the bookshelf views.", false, "0");

            // Is this a post-back requesting to save all this data?
            if (Current_Mode.isPostBack)
            {
                NameValueCollection form = HttpContext.Current.Request.Form;
                string action_value = form["admin_settings_action"];
                if (action_value == "save")
                {
                    // Populate all the new settings
                    Dictionary<string, string> newSettings = new Dictionary<string, string>();

                    int errors = 0;
                    // Step through each setting
                    foreach (int thisSettingCounter in idToSetting.Keys)
                    {
                        Setting_Info thisSetting = idToSetting[thisSettingCounter];
                        if (thisSetting.isVariable)
                        {
                            string setting_key = thisSetting.Key;
                            if (form["setting" + thisSettingCounter] != null)
                            {
                                string setting_value = form["setting" + thisSettingCounter];
                                newSettings[setting_key] = setting_value;
                            }
                        }
                    }

                    // Now, validate all this
                    errorBuilder = new StringBuilder();
                    isValid = true;
                    if (validate_update_entered_data(newSettings))
                    {
                        // Try to save each setting
                        errors += newSettings.Keys.Count(thisKey => !SobekCM_Database.Set_Setting(thisKey, newSettings[thisKey]));

                        // Prepare the action message
                        if (errors > 0)
                            actionMessage = "Save completed, but with " + errors + " errors.";
                        else
                            actionMessage = "Settings saved";

                        // Get the current settings from the database
                        settings = SobekCM_Database.Get_Settings(Tracer);

                        // Assign this to be used by the system
                        SobekCM_Library_Settings.Refresh(SobekCM_Database.Get_Settings_Complete(null));
                    }
                    else
                    {
                        actionMessage = errorBuilder.ToString().Replace("\n", "<br />");
                        settings = newSettings;
                    }
                }
            }
        }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <value> This always returns the value 'System-Wide Settings' </value>
        public override string Web_Title
        {
            get { return "System-Wide Settings"; }
        }

        private bool validate_update_entered_data( Dictionary<string, string> newSettings )
        {
            isValid = true;
            List<string> keys = newSettings.Keys.ToList();
            foreach( string key in keys )
            {
                string value = newSettings[key];

                switch (key)
                {
                    case "Application Server Network":
                        must_end_with(value, key, "\\", newSettings);
                        break;

                    case "Application Server URL":
                        must_start_end_with(value, key, "http://", "/", newSettings);
                        break;

                    case "Document Solr Index URL":
                        must_start_end_with(value, key, "http://", "/", newSettings);
                        break;

                    case "Files To Exclude From Downloads":
                        must_be_valid_regular_expression(value, key);
                        break;

                    case "Image Server Network":
                        must_end_with(value, key, "\\", newSettings);
                        break;

                    case "Image Server URL":
                        must_start_end_with(value, key, "http://", "/", newSettings);
                        break;

                    case "JPEG Height":
                        must_be_positive_number(value, key);
                        break;

                    case "JPEG Width":
                        must_be_positive_number(value, key);
                        break;

                    case "Log Files Directory":
                        must_end_with(value, key, "\\", newSettings);
                        break;

                    case "Log Files URL":
                        must_start_end_with(value, key, "http://", "/", newSettings);
                        break;

                    case "Mango Union Search Base URL":
                        must_start_with(value, key, "http://", newSettings);
                        break;

                    case "Mango Union Search Text":
                        if (value.Trim().Length > 0)
                        {
                            if (value.IndexOf("%1") < 0)
                            {
                                isValid = false;
                                errorBuilder.AppendLine( key + ": Value must contain the '%1' string.  See help for more information.");
                            }
                        }
                        break;

                    case "MarcXML Feed Location":
                        must_end_with(value, key, "\\", newSettings);
                        break;

                    case "OAI Resource Identifier Base":
                        must_end_with(value, key, ":", newSettings);
                        break;

                    case "Page Solr Index URL":
                        must_start_end_with(value, key, "http://", "/", newSettings);
                        break;

                    case "PostArchive Files To Delete":
                        must_be_valid_regular_expression(value, key);
                        break;

                    case "PreArchive Files To Delete":
                        must_be_valid_regular_expression(value, key);
                        break;

                    case "Static Pages Location":
                        must_end_with(value, key, "\\", newSettings);
                        break;

                    case "System Base Abbreviation":
                        if (value.Trim().Length == 0)
                        {
                            isValid = false;
                            errorBuilder.AppendLine( key + ": Field is required.");
                        }
                        break;

                    case "System Base URL":
                        if (value.Trim().Length == 0)
                        {
                            isValid = false;
                            errorBuilder.AppendLine( key + ": Field is required.");
                        }
                        else
                        {
                            must_start_end_with(value, key, "http://", "/", newSettings);
                        }
                        break;

                    case "Thumbnail Height":
                        must_be_positive_number(value, key);
                        break;

                    case "Thumbnail Width":
                        must_be_positive_number(value, key);
                        break;

                    case "Web In Process Submission Location":
                        must_end_with(value, key, "\\", newSettings);
                        break;
                }
            }

            return isValid;
        }

        private void Add_Setting_UI(string key, int fixed_input_size, string[] options, string help_message, bool is_florida_sus_setting )
        {
            // Create the settings option struct
            Variable_Setting_Info newSettingInfo = new Variable_Setting_Info(key, fixed_input_size, options, help_message, is_florida_sus_setting);

            // First, save this in the setting dictionary
            idToSetting[settingCounter] = newSettingInfo;

            // Increment in preparation for any next setting
            settingCounter++;
        }

        private void Add_Setting_UI(string key, int fixed_input_size, string[] options, string help_message, bool is_florida_sus_setting, string default_value)
        {
            // Create the settings option struct
            Variable_Setting_Info newSettingInfo = new Variable_Setting_Info(key, fixed_input_size, options, help_message, is_florida_sus_setting, default_value );

            // First, save this in the setting dictionary
            idToSetting[settingCounter] = newSettingInfo;

            // Increment in preparation for any next setting
            settingCounter++;
        }

        private void Add_Config_Setting(string key, string value, string help_message)
        {
            // Create the settings option struct
            Constant_Setting_Info newSettingInfo = new Constant_Setting_Info(key, value, help_message);

            // First, save this in the setting dictionary
            idToSetting[settingCounter] = newSettingInfo;

            // Increment in preparation for any next setting
            settingCounter++;
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This class does nothing, since the themes list is added as controls, not HTML </remarks>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Settings_AdminViewer.Write_HTML", "Do nothing");
        }

        /// <summary> This is an opportunity to write HTML directly into the main form, without
        /// using the pop-up html form architecture </summary>
        /// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This text will appear within the ItemNavForm form tags </remarks>
        public override void Add_HTML_In_Main_Form(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Settings_AdminViewer.Add_HTML_In_Main_Form", "Write the rest of the form ");


            // Add the hidden field
            Output.WriteLine("<!-- Hidden field is used for postbacks to indicate what to save and reset -->");
            Output.WriteLine("<input type=\"hidden\" id=\"admin_settings_action\" name=\"admin_settings_action\" value=\"\" />");
            Output.WriteLine();

            Output.WriteLine("<!-- Settings_AdminViewer.Add_HTML_In_Main_Form -->");
            Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_form.js\" ></script>");
            Output.WriteLine("<script src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_admin.js\" type=\"text/javascript\"></script>");
            Output.WriteLine("<div class=\"SobekHomeText\">");

            if (actionMessage.Length > 0)
            {
                Output.WriteLine("  <br />");
                Output.WriteLine("  <center><b>" + actionMessage + "</b></center>");
            }

            // Determine if the Florida SUS settings should be displayed
            bool show_florida_sus = false;
            if ((settings.ContainsKey("Show Florida SUS Settings")) && (String.Compare(settings["Show Florida SUS Settings"], "true", true) == 0))
                show_florida_sus = true;

            Output.WriteLine("  <blockquote>");
            Output.WriteLine("    This form allows a user to view and edit all the main system-wide settings which allow the SobekCM web application and assorted related applications to function correctly within each custom architecture and each institution.<br /><br />");
            Output.WriteLine("    For more information about these settings, <a href=\"" + SobekCM_Library_Settings.Help_URL(currentMode.Base_URL) + "admin/settings\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.");
            Output.WriteLine("  </blockquote>");
            Output.WriteLine();
            Output.WriteLine("  <table width=\"100%\">");
            Output.WriteLine("    <tr>");
            Output.WriteLine("      <td width=\"300px\" align=\"left\">");
            Output.WriteLine("        <span class=\"SobekAdminTitle\">Current System-Wide Settings</span>");
            Output.WriteLine("      </td>");
            Output.WriteLine("      <td align=\"right\">");
            Output.WriteLine("        <a onmousedown=\"window.location.href='" + currentMode.Base_URL + "my/admin'; return false;\"><img style=\"cursor: pointer;\" border=\"0px\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/cancel_button_g.gif\" alt=\"CANCEL\" /></a> &nbsp; &nbsp; ");
            Output.WriteLine("        <a onmousedown=\"admin_settings_save(); return false;\"><img style=\"cursor: pointer;\" border=\"0px\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/save_button_g.gif\" alt=\"SAVE\" /></a>");
            Output.WriteLine("      </td>");
            Output.WriteLine("      <td width=\"20px\">&nbsp;</td>");
            Output.WriteLine("    </tr>");
            Output.WriteLine("  </table>");
            Output.WriteLine();
            Output.WriteLine("  <blockquote>");
            Output.WriteLine("  <table border=\"0px\" cellspacing=\"0px\" class=\"statsTable\" width=\"100%\">");
            Output.WriteLine("    <tr align=\"left\" bgcolor=\"#0022a7\" height=\"30px\" >");
            Output.WriteLine("      <th width=\"230px\" align=\"left\"><span style=\"color: White\"> &nbsp; SETTING KEY</span></th>");
            Output.WriteLine("      <th width=\"420px\" align=\"center\"><span style=\"color: White\">SETTING VALUE</span></th>");
            Output.WriteLine("     </tr>");
            Output.WriteLine("    <tr><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");

            // Write the data for each interface
            bool odd_row = true;
            foreach ( int setting_counter in idToSetting.Keys )
            {
                // Get the current setting information
                Setting_Info settingInfo = idToSetting[setting_counter];

                // If no Florida SUS-specific settings should be displayed, check to see
                // if this setting should be skipped
                if ((show_florida_sus) || (!settingInfo.Is_Florida_SUS_Setting))
                {
                    // Also, look for this value in the current settings
                    string setting_value = String.Empty;
                    if (settingInfo.isVariable)
                    {
                        if (settings.ContainsKey(settingInfo.Key))
                            setting_value = settings[settingInfo.Key];
                        if ((setting_value.Length == 0) && (((Variable_Setting_Info)settingInfo).Default_Value.Length > 0))
                            setting_value = ((Variable_Setting_Info)settingInfo).Default_Value;
                    }
                    else
                    {
                        setting_value = ((Constant_Setting_Info)settingInfo).Value;
                    }


                    // Build the action links
                    Output.WriteLine(odd_row
                                         ? "    <tr align=\"left\" valign=\"middle\" height=\"30px\" >"
                                         : "    <tr align=\"left\" valign=\"middle\" height=\"30px\" bgcolor=\"#eeeeee\" >");
                    Output.WriteLine("      <td><strong>" + settingInfo.Key + ":</strong></td>");
                    Output.WriteLine("      <td>");
                    Output.WriteLine("        <table>");
                    Output.WriteLine("          <tr valign=\"middle\">");
                    Output.WriteLine("            <td>");

                    // Is this a variable setting, which the user can change?
                    if (settingInfo.isVariable)
                    {
                        Variable_Setting_Info varSettingInfo = (Variable_Setting_Info)settingInfo;
                        if (varSettingInfo.Options.Length > 0)
                        {
                            Output.WriteLine("              <select id=\"setting" + setting_counter + "\" name=\"setting" + setting_counter + "\" class=\"admin_settings_select\" >");

                            bool option_found = false;
                            foreach (string thisValue in varSettingInfo.Options)
                            {
                                if (String.Compare(thisValue, setting_value, true) == 0)
                                {
                                    option_found = true;
                                    Output.WriteLine("                <option selected=\"selected\">" + setting_value + "</option>");
                                }
                                else
                                {
                                    Output.WriteLine("                <option>" + thisValue + "</option>");
                                }
                            }

                            if (!option_found)
                            {
                                Output.WriteLine("                <option selected=\"selected\">" + setting_value + "</option>");
                            }
                            Output.WriteLine("              </select>");
                        }
                        else
                        {
                            if ((varSettingInfo.Fixed_Input_Size > 0) && (varSettingInfo.Fixed_Input_Size < 360))
                                Output.WriteLine("              <input id=\"setting" + setting_counter + "\" name=\"setting" + setting_counter + "\" class=\"admin_settings_input\" type=\"text\"  style=\"width: " + varSettingInfo.Fixed_Input_Size + "px;\" value=\"" + HttpUtility.HtmlEncode(setting_value) + "\" onfocus=\"javascript:textbox_enter('setting" + setting_counter + "', 'admin_settings_input_focused')\" onblur=\"javascript:textbox_leave('setting" + setting_counter + "', 'admin_settings_input')\" />");
                            else
                                Output.WriteLine("              <input id=\"setting" + setting_counter + "\" name=\"setting" + setting_counter + "\" class=\"admin_settings_input\" type=\"text\" style=\"width: 360px;\" value=\"" + HttpUtility.HtmlEncode(setting_value) + "\" onfocus=\"javascript:textbox_enter('setting" + setting_counter + "', 'admin_settings_input_focused')\" onblur=\"javascript:textbox_leave('setting" + setting_counter + "', 'admin_settings_input')\" />");
                        }
                    }
                    else
                    {
                        if (setting_value.Trim().Length == 0)
                        {
                            Output.WriteLine("              <em>( no value )</em>");
                        }
                        else
                        {
                            Output.WriteLine("              " + HttpUtility.HtmlEncode(setting_value));
                        }
                    }

                    Output.WriteLine("            </td>");
                    Output.WriteLine("            <td>");
                    Output.WriteLine("              <img border=\"0px\" style=\"padding-left:5px; margin-top:3px; cursor:pointer; cursor:hand;\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + settingInfo.Help_Message.Replace("'", "").Replace("\\", "\\\\").Replace("\n", "\\n") + "');\" />");
                    Output.WriteLine("            </td>");
                    Output.WriteLine("          </tr valign=\"middle\">");
                    Output.WriteLine("        </table>");
                    Output.WriteLine("      </td>");
                    Output.WriteLine("    </tr>");
                    Output.WriteLine("    <tr><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");

                    odd_row = !odd_row;
                }
            }

            Output.WriteLine("  </table>");
            Output.WriteLine("  </blockquote>");

            Output.WriteLine();
            Output.WriteLine("  <table width=\"100%\">");
            Output.WriteLine("    <tr>");
            Output.WriteLine("      <td width=\"300px\" align=\"left\">&nbsp;</td>");
            Output.WriteLine("      <td align=\"right\">");
            Output.WriteLine("        <a onmousedown=\"window.location.href='" + currentMode.Base_URL + "my/admin'; return false;\"><img style=\"cursor: pointer;\" border=\"0px\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/cancel_button_g.gif\" alt=\"CANCEL\" /></a> &nbsp; &nbsp; ");
            Output.WriteLine("        <a onmousedown=\"admin_settings_save(); return false;\"><img style=\"cursor: pointer;\" border=\"0px\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/save_button_g.gif\" alt=\"SAVE\" /></a>");
            Output.WriteLine("      </td>");
            Output.WriteLine("      <td width=\"20px\">&nbsp;</td>");
            Output.WriteLine("    </tr>");
            Output.WriteLine("  </table>");

            Output.WriteLine("  <br />");
            Output.WriteLine("</div>");
            Output.WriteLine();
        }

        private void must_be_positive_number(string value, string key)
        {
            bool appears_valid = false;
            int number;
            if ((Int32.TryParse(value, out number)) && (number >= 0))
            {
                appears_valid = true;
            }

            if (!appears_valid)
            {
                isValid = false;
                errorBuilder.AppendLine(key + ": Value must be a positive integer or zero.");
            }
        }

        private void must_be_valid_regular_expression(string value, string key)
        {
            if (value.Length == 0)
                return;

            try
            {
                Regex.Match("any_old_file.tif", value);
                return;
            }
            catch (ArgumentException)
            {
                isValid = false;
                errorBuilder.AppendLine(key + ": Value must be empty or a valid regular expression.");
                return;
            }
        }

        private static void must_start_with(string value, string key, string startsWith, Dictionary<string, string> newSettings)
        {
            if (value.Length == 0)
                return;

            if (!value.StartsWith(startsWith, StringComparison.InvariantCultureIgnoreCase))
            {
                newSettings[key] = startsWith + value;
            }
        }

        private static void must_end_with(string value, string key, string endsWith, Dictionary<string, string> newSettings)
        {
            if (value.Length == 0)
                return;

            if (!value.EndsWith(endsWith, StringComparison.InvariantCultureIgnoreCase))
            {
                newSettings[key] = value + endsWith;
            }
        }

        private static void must_start_end_with(string value, string key, string startsWith, string endsWith, Dictionary<string, string> newSettings)
        {
            if (value.Length == 0)
                return;

            if ((!value.StartsWith(startsWith, StringComparison.InvariantCultureIgnoreCase)) || (!value.EndsWith(endsWith, StringComparison.InvariantCultureIgnoreCase)))
            {
                if (!value.StartsWith(startsWith, StringComparison.InvariantCultureIgnoreCase))
                    value = startsWith + value;
                if (!value.EndsWith(endsWith, StringComparison.InvariantCultureIgnoreCase))
                    value = value + endsWith;
                newSettings[key] = value;
            }
        }

        #region Nested type: Constant_Setting_Info

        /// <summary> Structure holds the basic information about a single constant setting read from the web configuration file</summary>
        protected class Constant_Setting_Info : Setting_Info
        {
            /// <summary> Value for this constant setting from the web configuration file </summary>
            public readonly string Value;

            /// <summary> Contstructor for a new object of this type </summary>
            /// <param name="Key"> Key for this constant setting from the web configuration file</param>
            /// <param name="Value">Value for this constant setting from the web configuration file </param>
            /// <param name="Help_Message"> Message to be displayed if user requests help on this setting </param>
            public Constant_Setting_Info(string Key, string Value, string Help_Message) : base(Key, Help_Message, false, String.Empty )
            {
                this.Value = Value;
            }

            /// <summary> Flag indicates if this is a variable setting which will display, but not be editable </summary>
            /// <value> 'FALSE' is always returned for obejcts of this  class </value>
            public override bool isVariable
            {
                get { return false; }
            }
        }

        #endregion

        #region Nested type: Setting_Info

        /// <summary> Structure holds the basic information about a single setting which is held in the database and can be changed from the web application's admin screens </summary>
        protected abstract class Setting_Info
        {
            /// <summary> Message to be displayed if user requests help on this setting </summary>
            public readonly string Help_Message;

            /// <summary> Key for this setting </summary>
            public readonly string Key;

            /// <summary> Default value to use if the key is not found in the currrent settings </summary>
            public readonly string Default_Value;

            /// <summary> Flag indicates if this is a Florida SUS setting, to be generally suppressed from displaying </summary>
            public readonly bool Is_Florida_SUS_Setting;

            /// <summary> Contstructor for a new object of this type </summary>
            /// <param name="Key"> Key for this setting </param>
            /// <param name="Help_Message"> Message to be displayed if user requests help on this setting </param>
            protected Setting_Info(string Key, string Help_Message, bool Is_Florida_SUS_Setting, string Default_Value)
            {
                this.Key = Key;
                this.Help_Message = Help_Message;
                this.Is_Florida_SUS_Setting = Is_Florida_SUS_Setting;
                this.Default_Value = Default_Value;
            }

            /// <summary> Flag indicates if this is a variable setting which will display, but not be editable </summary>
            public abstract bool isVariable { get; }
        }

        #endregion

        #region Nested type: Variable_Setting_Info

        /// <summary> Structure holds the basic information about a single setting which is held in the database and can be changed from the web application's admin screens </summary>
        protected class Variable_Setting_Info : Setting_Info
        {
            /// <summary> If the input should be a fixed size, size in pixels </summary>
            public readonly int Fixed_Input_Size;

            /// <summary> If there are only a limited number of options for this setting, this holds all possible options </summary>
            public readonly string[] Options;

            /// <summary> Contstructor for a new object of this type </summary>
            /// <param name="Key"> Key for this setting in the database (also the display label) </param>
            /// <param name="Fixed_Input_Size"> If the input should be a fixed size, size in pixels </param>
            /// <param name="Options"> If there are only a limited number of options for this setting, this holds all possible options </param>
            /// <param name="Help_Message"> Message to be displayed if user requests help on this setting </param>
            public Variable_Setting_Info(string Key, int Fixed_Input_Size, string[] Options, string Help_Message, bool Is_Florida_SUS_Setting ):base(Key, Help_Message, Is_Florida_SUS_Setting, String.Empty )
            {
                this.Fixed_Input_Size = Fixed_Input_Size;
                this.Options = Options;
            }

            /// <summary> Contstructor for a new object of this type </summary>
            /// <param name="Key"> Key for this setting in the database (also the display label) </param>
            /// <param name="Fixed_Input_Size"> If the input should be a fixed size, size in pixels </param>
            /// <param name="Options"> If there are only a limited number of options for this setting, this holds all possible options </param>
            /// <param name="Help_Message"> Message to be displayed if user requests help on this setting </param>
            /// <param name="Default_Value">Default value to use if the key is not found in the currrent settings</param>
            public Variable_Setting_Info(string Key, int Fixed_Input_Size, string[] Options, string Help_Message, bool Is_Florida_SUS_Setting, string Default_Value ) : base(Key, Help_Message, Is_Florida_SUS_Setting, Default_Value)
            {
                this.Fixed_Input_Size = Fixed_Input_Size;
                this.Options = Options;
            }

            /// <summary> Flag indicates if this is a variable setting which will display, but not be editable </summary>
            /// <value> 'TRUE' is always returned for obejcts of this  class </value>
            public override bool isVariable
            {
                get { return true;  }
            }
        }

        #endregion
    }
}