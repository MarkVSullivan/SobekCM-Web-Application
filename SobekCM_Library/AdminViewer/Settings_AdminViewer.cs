#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using SobekCM.Core.Builder;
using SobekCM.Core.Client;
using SobekCM.Core.Configuration;
using SobekCM.Core.Configuration.Authentication;
using SobekCM.Core.Configuration.Engine;
using SobekCM.Core.Configuration.Extensions;
using SobekCM.Core.Configuration.OAIPMH;
using SobekCM.Core.MicroservicesClient;
using SobekCM.Core.Navigation;
using SobekCM.Core.Search;
using SobekCM.Core.Settings;
using SobekCM.Core.Settings.DbItemViewers;
using SobekCM.Core.UI_Configuration;
using SobekCM.Core.UI_Configuration.Citation;
using SobekCM.Core.UI_Configuration.StaticResources;
using SobekCM.Core.UI_Configuration.TemplateElements;
using SobekCM.Core.UI_Configuration.Viewers;
using SobekCM.Core.Users;
using SobekCM.Core.WebContent;
using SobekCM.Engine_Library.Configuration;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.ResultsViewer;
using SobekCM.Library.UI;
using SobekCM.Resource_Object.Configuration;
using SobekCM.Tools;
using Microservice_Endpoint_Protocol_Enum = SobekCM.Core.Configuration.Engine.Microservice_Endpoint_Protocol_Enum;

#endregion

namespace SobekCM.Library.AdminViewer
{
	/// <summary> Class allows an authenticated system admin to view and edit the library-wide system settings in this library </summary>
	/// <remarks> This class extends the <see cref="abstract_AdminViewer"/> class.<br /><br />
	/// MySobek Viewers are used for registration and authentication with mySobek, as well as performing any task which requires
	/// authentication, such as online submittal, metadata editing, and system administrative tasks.<br /><br />
	/// During a valid html request, the following steps occur:
	/// <ul>
	/// <li>Application state is built/verified by the Application_State_Builder </li>
	/// <li>Request is analyzed by the QueryString_Analyzer and output as a <see cref="Navigation_Object"/>  </li>
	/// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
	/// <li>The HTML writer will create the necessary subwriter.  Since this action requires authentication, an instance of the  <see cref="MySobek_HtmlSubwriter"/> class is created. </li>
	/// <li>The mySobek subwriter creates an instance of this viewer to show the library-wide system settings in this digital library</li>
	/// </ul></remarks>
	public class Settings_AdminViewer : abstract_AdminViewer
	{
		private string actionMessage;
		private StringBuilder errorBuilder;

		private bool isValid;
	  
		private readonly bool category_view;
		private readonly bool limitedRightsMode;
		private readonly bool readonlyMode;
		private readonly Admin_Setting_Collection currSettings;
		private SortedList<string, string> tabPageNames;
		private Dictionary<string, List<Admin_Setting_Value>> settingsByPage;
	    private List<Admin_Setting_Value> builderSettings;

        private readonly Settings_Mode_Enum mainMode = Settings_Mode_Enum.NONE;
        private readonly Settings_Configuration_SubMode_Enum configSubMode = Settings_Configuration_SubMode_Enum.NONE;
        private readonly Settings_Builder_SubMode_Enum builderSubEnum = Settings_Builder_SubMode_Enum.NONE;
	    private readonly Settings_Metadata_SubMode_Enum metadataSubEnum = Settings_Metadata_SubMode_Enum.NONE;
        private readonly Settings_Engine_SubMode_Enum engineSubEnum = Settings_Engine_SubMode_Enum.NONE;
        private readonly Settings_UI_SubMode_Enum uiSubEnum = Settings_UI_SubMode_Enum.NONE;
        private readonly Settings_HTML_SubMode_Enum htmlSubEnum = Settings_HTML_SubMode_Enum.NONE;
	    private readonly int extensionSubMode = -1;

	    private readonly string redirectUrl;

        #region Enumeration of the main modes of the settings, as well as submodes

        private enum Settings_Mode_Enum : byte
		{
			NONE,
	
			Settings,

            Configuration,

			Builder,

            Metadata,

			Engine,

			UI,

			HTML,

			Extensions
		}

        private enum Settings_Configuration_SubMode_Enum : byte
        {
            NONE,

            Files,

            Reading_Log
        }

	    private enum Settings_Builder_SubMode_Enum : byte
	    {
	        NONE,

            Builder_Settings,

            Builder_Folders,

            Builder_Modules
	    }

        private enum Settings_Metadata_SubMode_Enum : byte
        {
            NONE,

            Fields,

            Metdata_Reader_Writers,

            METS_Section_Reader_Writers,

            METS_Writing_Profiles,

            Metadata_Modules_To_Include
        }

        private enum Settings_Engine_SubMode_Enum : byte
        {
            NONE,

            Authentication,

            Brief_Item_Mapping,

            Contact_Form,

            Engine_Server_Endpoints,
            
            OAI_PMH,

            QcTool
        }

        private enum Settings_UI_SubMode_Enum : byte
        {
            NONE,

            Citation_Viewer,

            Map_Editor,

            Microservice_Client_Endpoints,

            Template_Elements,

            HTML_Viewer_Subviewers
        }

        private enum Settings_HTML_SubMode_Enum : byte
        {
            NONE,

            Missing_Page,

            No_Results
        }

		#endregion


		/// <summary> Constructor for a new instance of the Thematic_Headings_AdminViewer class </summary>
		/// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
		/// <remarks> Postback from handling saving the new settings is handled here in the constructor </remarks>
		public Settings_AdminViewer(RequestCache RequestSpecificValues) : base(RequestSpecificValues)
		{
			// If the RequestSpecificValues.Current_User cannot edit this, go back
			if (( RequestSpecificValues.Current_User == null ) || ((!RequestSpecificValues.Current_User.Is_System_Admin) && (!RequestSpecificValues.Current_User.Is_Portal_Admin)))
			{
				RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.My_Sobek;
				RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
				UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
				return;
			}

			// Determine if this user has limited rights
			if ((!RequestSpecificValues.Current_User.Is_System_Admin) && (!RequestSpecificValues.Current_User.Is_Host_Admin))
			{
				limitedRightsMode = true;
				readonlyMode = true;
			}
			else
			{
				limitedRightsMode = ((!RequestSpecificValues.Current_User.Is_Host_Admin) && (UI_ApplicationCache_Gateway.Settings.Servers.isHosted)); 
				readonlyMode = false;
			}

			// Load the settings either from the local session, or from the engine
			currSettings = HttpContext.Current.Session["Admin_Settings"] as Admin_Setting_Collection;
			if (currSettings == null)
			{
				currSettings = SobekEngineClient.Admin.Get_Admin_Settings(RequestSpecificValues.Tracer);
				if (currSettings != null)
				{
					HttpContext.Current.Session["Admin_Settigs"] = currSettings;

					// Build the setting values
					build_setting_objects_for_display();
				}
				else
				{
					actionMessage = "Error pulling the settings from the engine";
				}
            }

            #region Determine the mode and submode

            // Determine the current mode and submode
            if ((RequestSpecificValues.Current_Mode.Remaining_Url_Segments != null) && (RequestSpecificValues.Current_Mode.Remaining_Url_Segments.Length > 0))
            {
                switch (RequestSpecificValues.Current_Mode.Remaining_Url_Segments[0].ToLower())
                {
                    case "settings":
                        mainMode = Settings_Mode_Enum.Settings;
                        break;

                    case "config":
                        mainMode = Settings_Mode_Enum.Configuration;
                        if (RequestSpecificValues.Current_Mode.Remaining_Url_Segments.Length > 1)
                        {
                            switch (RequestSpecificValues.Current_Mode.Remaining_Url_Segments[1].ToLower())
                            {
                                case "files":
                                    configSubMode = Settings_Configuration_SubMode_Enum.Files;
                                    break;

                                case "log":
                                    configSubMode = Settings_Configuration_SubMode_Enum.Reading_Log;
                                    break;
                            }
                        }
                        break;

                    case "builder":
                        mainMode = Settings_Mode_Enum.Builder;
                        if (RequestSpecificValues.Current_Mode.Remaining_Url_Segments.Length > 1)
                        {
                            switch (RequestSpecificValues.Current_Mode.Remaining_Url_Segments[1].ToLower())
                            {
                                case "settings":
                                    builderSubEnum = Settings_Builder_SubMode_Enum.Builder_Settings;
                                    break;

                                case "folders":
                                    builderSubEnum = Settings_Builder_SubMode_Enum.Builder_Folders;
                                    break;

                                case "modules":
                                    builderSubEnum = Settings_Builder_SubMode_Enum.Builder_Modules;
                                    break;
                            }
                        }
                        break;

                    case "metadata":
                        mainMode = Settings_Mode_Enum.Metadata;
                        if (RequestSpecificValues.Current_Mode.Remaining_Url_Segments.Length > 1)
                        {
                            switch (RequestSpecificValues.Current_Mode.Remaining_Url_Segments[1].ToLower())
                            {
                                case "fields":
                                    metadataSubEnum = Settings_Metadata_SubMode_Enum.Fields;
                                    break;

                                case "filereaders":
                                    metadataSubEnum = Settings_Metadata_SubMode_Enum.Metdata_Reader_Writers;
                                    break;

                                case "metsreaders":
                                    metadataSubEnum = Settings_Metadata_SubMode_Enum.METS_Section_Reader_Writers;
                                    break;

                                case "metsprofiles":
                                    metadataSubEnum = Settings_Metadata_SubMode_Enum.METS_Writing_Profiles;
                                    break;

                                case "modules":
                                    metadataSubEnum = Settings_Metadata_SubMode_Enum.Metadata_Modules_To_Include;
                                    break;
                            }
                        }
                        break;

                    case "engine":
                        mainMode = Settings_Mode_Enum.Engine;
                        if (RequestSpecificValues.Current_Mode.Remaining_Url_Segments.Length > 1)
                        {
                            switch (RequestSpecificValues.Current_Mode.Remaining_Url_Segments[1].ToLower())
                            {
                                case "authentication":
                                    engineSubEnum = Settings_Engine_SubMode_Enum.Authentication;
                                    break;

                                case "briefitem":
                                    engineSubEnum = Settings_Engine_SubMode_Enum.Brief_Item_Mapping;
                                    break;

                                case "contact":
                                    engineSubEnum = Settings_Engine_SubMode_Enum.Contact_Form;
                                    break;

                                case "endpoints":
                                    engineSubEnum = Settings_Engine_SubMode_Enum.Engine_Server_Endpoints;
                                    break;

                                case "oaipmh":
                                    engineSubEnum = Settings_Engine_SubMode_Enum.OAI_PMH;
                                    break;

                                case "qctool":
                                    engineSubEnum = Settings_Engine_SubMode_Enum.QcTool;
                                    break;
                            }
                        }
                        break;

                    case "ui":
                        mainMode = Settings_Mode_Enum.UI;
                        if (RequestSpecificValues.Current_Mode.Remaining_Url_Segments.Length > 1)
                        {
                            switch (RequestSpecificValues.Current_Mode.Remaining_Url_Segments[1].ToLower())
                            {
                                case "citation":
                                    uiSubEnum = Settings_UI_SubMode_Enum.Citation_Viewer;
                                    break;

                                case "mapeditor":
                                    uiSubEnum = Settings_UI_SubMode_Enum.Map_Editor;
                                    break;

                                case "microservices":
                                    uiSubEnum = Settings_UI_SubMode_Enum.Microservice_Client_Endpoints;
                                    break;

                                case "template":
                                    uiSubEnum = Settings_UI_SubMode_Enum.Template_Elements;
                                    break;

                                case "viewers":
                                    uiSubEnum = Settings_UI_SubMode_Enum.HTML_Viewer_Subviewers;
                                    break;
                            }
                        }
                        break;

                    case "html":
                        mainMode = Settings_Mode_Enum.HTML;
                        if (RequestSpecificValues.Current_Mode.Remaining_Url_Segments.Length > 1)
                        {
                            switch (RequestSpecificValues.Current_Mode.Remaining_Url_Segments[1].ToLower())
                            {
                                case "missing":
                                    htmlSubEnum = Settings_HTML_SubMode_Enum.Missing_Page;
                                    break;

                                case "noresults":
                                    htmlSubEnum = Settings_HTML_SubMode_Enum.No_Results;
                                    break;
                            }
                        }
                        break;

                    case "extensions":
                        mainMode = Settings_Mode_Enum.Extensions;
                        int extensionsCount = 0;
                        if (UI_ApplicationCache_Gateway.Configuration.Extensions.Extensions != null)
                            extensionsCount = UI_ApplicationCache_Gateway.Configuration.Extensions.Extensions.Count;
                        if ((extensionsCount > 0 ) && ( RequestSpecificValues.Current_Mode.Remaining_Url_Segments.Length > 1))
                        {
                            int tryExtensionNum;
                            if (Int32.TryParse(RequestSpecificValues.Current_Mode.Remaining_Url_Segments[1], out tryExtensionNum))
                            {
                                if ((tryExtensionNum > 0) && (tryExtensionNum <= extensionsCount))
                                    extensionSubMode = tryExtensionNum;
                            }
                        }
                        break;
                }
            }

            #endregion
            
            // Establish some default, starting values
			actionMessage = String.Empty;
			category_view = Convert.ToBoolean(RequestSpecificValues.Current_User.Get_Setting("Settings_AdminViewer:Category_View", "false"));

            // Determine the redirect URL now
            string[] origUrlSegments = RequestSpecificValues.Current_Mode.Remaining_Url_Segments;
            RequestSpecificValues.Current_Mode.Remaining_Url_Segments = new string[] { "%SETTINGSCODE%" };
            redirectUrl = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
            RequestSpecificValues.Current_Mode.Remaining_Url_Segments = origUrlSegments;

            // Is this a post-back requesting to save all this data?
            if (RequestSpecificValues.Current_Mode.isPostBack)
			{
				NameValueCollection form = HttpContext.Current.Request.Form;

				if (form["admin_settings_order"] == "category")
				{
					RequestSpecificValues.Current_User.Add_Setting("Settings_AdminViewer:Category_View", "true");
					SobekCM_Database.Set_User_Setting(RequestSpecificValues.Current_User.UserID, "Settings_AdminViewer:Category_View", "true");
					category_view = true;
				}

				if (form["admin_settings_order"] == "alphabetical")
				{
					RequestSpecificValues.Current_User.Add_Setting("Settings_AdminViewer:Category_View", "false");
					SobekCM_Database.Set_User_Setting(RequestSpecificValues.Current_User.UserID, "Settings_AdminViewer:Category_View", "false");
					category_view = false;
				}

				string action_value = form["admin_settings_action"];
			    if ((action_value == "save") && (RequestSpecificValues.Current_User.Is_System_Admin))
			    {
    			    save_setting_values(RequestSpecificValues, mainMode );
			    }
			}
		}

	    private void save_setting_values(RequestCache RequestSpecificValues, Settings_Mode_Enum MainMode)
	    {
	        List<Admin_Setting_Value> pageSettings = null;

	        // Determine which page of settings to save
	        if (MainMode == Settings_Mode_Enum.Settings)
	        {
	            // Get the tab page index
	            int tabPage = -1;
	            if ((RequestSpecificValues.Current_Mode.Remaining_Url_Segments != null) && (RequestSpecificValues.Current_Mode.Remaining_Url_Segments.Length > 1))
	            {
	                if (!Int32.TryParse(RequestSpecificValues.Current_Mode.Remaining_Url_Segments[1], out tabPage))
	                    tabPage = -1;
	            }

	            // Only try to save if it is valid
	            if ((tabPage >= 1) && (tabPage <= tabPageNames.Count))
	            {
	                // Get the tab page settings
	                string tabPageNameKey = tabPageNames.Keys[tabPage - 1];
	                string tabPageName = tabPageNames[tabPageNameKey];
	                pageSettings = settingsByPage[tabPageName];
	            }
	        }

	        // Pretty simple if this is the builder setting page
	        if (MainMode == Settings_Mode_Enum.Builder)
	            pageSettings = builderSettings;

	        // If no settings found for this .. just return
	        if (pageSettings == null)
	            return;

	        // Get the collection of values posted back from user
	        NameValueCollection form = HttpContext.Current.Request.Form;

	        // First, create the setting lookup by ID, and the list of IDs to look for
	        List<short> settingIds = new List<short>();
	        Dictionary<short, Admin_Setting_Value> settingsObjsById = new Dictionary<short, Admin_Setting_Value>();
	        foreach (Admin_Setting_Value value in pageSettings)
	        {
	            // If this is readonly, will not prepare to update
	            if ((!Is_Value_ReadOnly(value, readonlyMode, limitedRightsMode)) && (!value.Hidden))
	            {
	                settingIds.Add(value.SettingID);
	                settingsObjsById[value.SettingID] = value;
	            }
	        }

	        // Now, step through and get the values for each of these
	        List<Simple_Setting> newValues = new List<Simple_Setting>();
	        foreach (short id in settingIds)
	        {
	            // Get the setting information
	            Admin_Setting_Value thisValue = settingsObjsById[id];

	            if (form["setting" + id] != null)
	            {
	                newValues.Add(new Simple_Setting(thisValue.Key, form["setting" + id], thisValue.SettingID));
	            }
	            else
	            {
	                newValues.Add(new Simple_Setting(thisValue.Key, String.Empty, thisValue.SettingID));
	            }
	        }

	        // Now, validate all this
	        errorBuilder = new StringBuilder();
	        isValid = true;
	        validate_update_entered_data(newValues);

	        // Now, assign the values from the simple settings back to the main settings
	        // object.  This is to allow for the validation process to make small changes
	        // to the values the user entered, like different starts or endings
	        foreach (Simple_Setting thisSetting in newValues)
	        {
	            settingsObjsById[thisSetting.SettingID].Value = thisSetting.Value;
	        }

	        if (isValid)
	        {
	            // Try to save each setting
	            int errors = newValues.Count(NewSetting => !SobekCM_Database.Set_Setting(NewSetting.Key, NewSetting.Value));

	            // Prepare the action message
	            if (errors > 0)
	                actionMessage = "Save completed, but with " + errors + " errors.";
	            else
	                actionMessage = "Settings saved";

	            // Assign this to be used by the system
	            UI_ApplicationCache_Gateway.ResetSettings();


	            // Get all the settings again 
	            build_setting_objects_for_display();
	        }
	        else
	        {
	            actionMessage = errorBuilder.ToString().Replace("\n", "<br />");
	        }
	    }

	    private static bool Is_Value_ReadOnly(Admin_Setting_Value Value, bool ReadOnlyMode, bool LimitedRightsMode )
		{
		   return ((ReadOnlyMode) || (Value.Reserved > 2) || ((LimitedRightsMode) && (Value.Reserved != 0)));
		}



		/// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
		/// <value> This always returns the value 'System-Wide Settings' </value>
		public override string Web_Title
		{
			get { return "System-Wide Settings"; }
		}
		
		/// <summary> Gets the URL for the icon related to this administrative task </summary>
		public override string Viewer_Icon
		{
			get { return Static_Resources_Gateway.Settings_Img; }
		}


        /// <summary> Gets the collection of special behaviors which this admin or mySobek viewer
        /// requests from the main HTML subwriter. </summary>
        public override List<HtmlSubwriter_Behaviors_Enum> Viewer_Behaviors
        {
            get
            {
                return new List<HtmlSubwriter_Behaviors_Enum> { HtmlSubwriter_Behaviors_Enum.Suppress_Banner };
            }
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
		public override void Write_ItemNavForm_Closing(TextWriter Output, Custom_Tracer Tracer)
		{
			Tracer.Add_Trace("Settings_AdminViewer.Write_ItemNavForm_Closing", "Write the rest of the form ");


			// Add the hidden field
			Output.WriteLine("<!-- Hidden field is used for postbacks to indicate what to save and reset -->");
			Output.WriteLine("<input type=\"hidden\" id=\"admin_settings_action\" name=\"admin_settings_action\" value=\"\" />");
			if ( category_view )	
				Output.WriteLine("<input type=\"hidden\" id=\"admin_settings_order\" name=\"admin_settings_order\" value=\"category\" />");
			else
				Output.WriteLine("<input type=\"hidden\" id=\"admin_settings_order\" name=\"admin_settings_order\" value=\"alphabetical\" />");

			Output.WriteLine();

			Output.WriteLine("<!-- Settings_AdminViewer.Write_ItemNavForm_Closing -->");
			Output.WriteLine("<script src=\"" + Static_Resources_Gateway.Sobekcm_Admin_Js + "\" type=\"text/javascript\"></script>");
			Output.WriteLine();


		    Output.WriteLine("</div> <!-- ends PageContainer div momentarily for this extra wide table -->");

			Output.WriteLine("<table id=\"sbkSeav_MainTable\">");
			Output.WriteLine("  <tr>");
			Output.WriteLine("    <td id=\"sbkSeav_TocArea\">");


			// Determine the current viewer code
			string currentViewerCode = String.Empty;
			if ((RequestSpecificValues.Current_Mode.Remaining_Url_Segments != null) && (RequestSpecificValues.Current_Mode.Remaining_Url_Segments.Length > 0))
			{
				if (RequestSpecificValues.Current_Mode.Remaining_Url_Segments.Length == 1)
					currentViewerCode = RequestSpecificValues.Current_Mode.Remaining_Url_Segments[0];
				else if (RequestSpecificValues.Current_Mode.Remaining_Url_Segments.Length == 2)
					currentViewerCode = RequestSpecificValues.Current_Mode.Remaining_Url_Segments[0] + "/" + RequestSpecificValues.Current_Mode.Remaining_Url_Segments[1];
				else
					currentViewerCode = RequestSpecificValues.Current_Mode.Remaining_Url_Segments[0] + "/" + RequestSpecificValues.Current_Mode.Remaining_Url_Segments[1] + "/" + RequestSpecificValues.Current_Mode.Remaining_Url_Segments[2];
			}

			// Write all the values in the left nav bar
			Output.WriteLine(add_leftnav_h2_link("Settings", "settings", redirectUrl, currentViewerCode));
			Output.WriteLine("        <ul>");

			// Add all the different setting options (tabpages) in the left nav bar
			int tab_count = 1;
			foreach (string tabPageName in tabPageNames.Values)
			{
				Output.WriteLine(add_leftnav_li_link(tabPageName.Trim(), "settings/" + tab_count, redirectUrl, currentViewerCode));
				//add_tab_page_info(Output, tabPageName, settingsByPage[tabPageName]);
				tab_count++;
			}
			Output.WriteLine("        </ul>");

            Output.WriteLine(add_leftnav_h2_link("Configuration Files", "config", redirectUrl, currentViewerCode));
            Output.WriteLine("        <ul>");
            Output.WriteLine(add_leftnav_li_link("Source Files", "config/files", redirectUrl, currentViewerCode));
            Output.WriteLine(add_leftnav_li_link("Reading Log", "config/log", redirectUrl, currentViewerCode));
            Output.WriteLine("        </ul>");

			Output.WriteLine(add_leftnav_h2_link("Builder", "builder", redirectUrl, currentViewerCode));
			Output.WriteLine("        <ul>");
			Output.WriteLine(add_leftnav_li_link("Builder Settings", "builder/settings", redirectUrl, currentViewerCode));
			Output.WriteLine(add_leftnav_li_link("Incoming Folders", "builder/folders", redirectUrl, currentViewerCode));
			Output.WriteLine(add_leftnav_li_link("Builder Modules", "builder/modules", redirectUrl, currentViewerCode));
			Output.WriteLine("        </ul>");

            Output.WriteLine(add_leftnav_h2_link("Metadata Configuration", "metadata", redirectUrl, currentViewerCode));
            Output.WriteLine("        <ul>");
            Output.WriteLine(add_leftnav_li_link("Metadata Search Fields", "metadata/fields", redirectUrl, currentViewerCode));
            Output.WriteLine(add_leftnav_li_link("File Readers/Writers", "metadata/filereaders", redirectUrl, currentViewerCode));
            Output.WriteLine(add_leftnav_li_link("METS Section Readers/Writers", "metadata/metsreaders", redirectUrl, currentViewerCode));
            Output.WriteLine(add_leftnav_li_link("METS Writing Profiles", "metadata/metsprofiles", redirectUrl, currentViewerCode));
        //    Output.WriteLine(add_leftnav_li_link("Standard Metadata Modules", "metadata/modules", redirectUrl, currentViewerCode));
            Output.WriteLine("        </ul>");

			Output.WriteLine(add_leftnav_h2_link("Engine Configuration", "engine", redirectUrl, currentViewerCode));
			Output.WriteLine("        <ul>");
			Output.WriteLine(add_leftnav_li_link("Authentication", "engine/authentication", redirectUrl, currentViewerCode));
			Output.WriteLine(add_leftnav_li_link("Brief Item Mapping", "engine/briefitem", redirectUrl, currentViewerCode));
			Output.WriteLine(add_leftnav_li_link("Contact Form", "engine/contact", redirectUrl, currentViewerCode));  // Move to UI? 
			Output.WriteLine(add_leftnav_li_link("Engine Server Endpoints", "engine/endpoints", redirectUrl, currentViewerCode));
			Output.WriteLine(add_leftnav_li_link("OAI-PMH Protocol", "engine/oaipmh", redirectUrl, currentViewerCode));
			Output.WriteLine(add_leftnav_li_link("Quality Control Tool", "engine/qctool", redirectUrl, currentViewerCode));    // Move to UI?
			Output.WriteLine("        </ul>");

			Output.WriteLine(add_leftnav_h2_link("UI Configuration", "ui", redirectUrl, currentViewerCode));
			Output.WriteLine("        <ul>");
			Output.WriteLine(add_leftnav_li_link("Citation Viewer", "ui/citation", redirectUrl, currentViewerCode));
		//	Output.WriteLine(add_leftnav_li_link("Map Editor", "ui/mapeditor", redirectUrl, currentViewerCode));
			Output.WriteLine(add_leftnav_li_link("Microservice Client Endpoints", "ui/microservices", redirectUrl, currentViewerCode));
			Output.WriteLine(add_leftnav_li_link("Template Elements", "ui/template", redirectUrl, currentViewerCode));
			Output.WriteLine(add_leftnav_li_link("HTML Viewers/Subviewers", "ui/viewers", redirectUrl, currentViewerCode));
			Output.WriteLine("        </ul>");

			Output.WriteLine(add_leftnav_h2_link("HTML Snippets", "html", redirectUrl, currentViewerCode));
			Output.WriteLine("        <ul>");
			Output.WriteLine(add_leftnav_li_link("Missing page", "html/missing", redirectUrl, currentViewerCode));
			Output.WriteLine(add_leftnav_li_link("No results", "html/noresults", redirectUrl, currentViewerCode));
			Output.WriteLine("        </ul>");

			Output.WriteLine(add_leftnav_h2_link("Extensions", "extensions", redirectUrl, currentViewerCode));
			if ((UI_ApplicationCache_Gateway.Configuration.Extensions != null) && (UI_ApplicationCache_Gateway.Configuration.Extensions.Extensions != null) && (UI_ApplicationCache_Gateway.Configuration.Extensions.Extensions.Count > 0))
			{
				Output.WriteLine("        <ul>");
				int extensionNumber = 1;
				foreach (ExtensionInfo thisExtension in UI_ApplicationCache_Gateway.Configuration.Extensions.Extensions)
				{
					Output.WriteLine(add_leftnav_li_link(thisExtension.Name, "extensions/" + extensionNumber, redirectUrl, currentViewerCode));
					extensionNumber++;
				}
				Output.WriteLine("        </ul>");
			}
			Output.WriteLine("    </td>");

			// Start the main area
			Output.WriteLine("    <td id=\"sbkSeav_MainArea\">");

			Output.WriteLine("<div class=\"sbkAdm_HomeText\">");

			if (actionMessage.Length > 0)
			{
				Output.WriteLine("  <br />");
				Output.WriteLine("  <div id=\"sbkAdm_ActionMessage\">" + actionMessage + "</div>");
			}

			// Add the content, based on the main mode
			switch (mainMode)
			{
				case Settings_Mode_Enum.NONE:
					add_top_level_info(Output);
					break;

				case Settings_Mode_Enum.Settings:
					add_settings_info(Output);
					break;

                case Settings_Mode_Enum.Configuration:
                    add_configuration_info(Output);
                    break;

				case Settings_Mode_Enum.Builder:
					add_builder_info(Output);
					break;

                case Settings_Mode_Enum.Metadata:
                    add_metadata_info(Output);
                    break;

				case Settings_Mode_Enum.Engine:
					add_engine_info(Output);
					break;

				case Settings_Mode_Enum.UI:
					add_ui_info(Output);
					break;

				case Settings_Mode_Enum.HTML:
					add_html_info(Output);
					break;

				case Settings_Mode_Enum.Extensions:
					add_extensions_info(Output);
					break;
			}


			Output.WriteLine("<br />");
			Output.WriteLine("<br />");


			Output.WriteLine("  <br />");
			Output.WriteLine("</div>");


			Output.WriteLine("    </td>");
			Output.WriteLine("  </tr>");
			Output.WriteLine("</table>");

            Output.WriteLine("<div id=\"pagecontainer_resumed\">");

			Output.WriteLine();
		}

		#region HTML helper methods for the left TOC portion 

		private static string add_leftnav_h2_link(string Text, string LinkCode, string RedirectUrl, string CurrentLinkCode )
		{
			if ( String.Compare(LinkCode, CurrentLinkCode, StringComparison.OrdinalIgnoreCase) == 0 )
				return "      <h2 id=\"sbkSeav_TocArea_SelectedH2\">" + Text + "</h2>";
			return "      <h2><a href=\"" + RedirectUrl.Replace("%SETTINGSCODE%", LinkCode) + "\">" + Text + " </a></h2>";
		}

		private static string add_leftnav_li_link(string Text, string LinkCode, string RedirectUrl, string CurrentLinkCode)
		{
			if (String.Compare(LinkCode, CurrentLinkCode, StringComparison.OrdinalIgnoreCase) == 0)
				return "      <li id=\"sbkSeav_TocArea_SelectedLI\">" + Text + "</li>";
			return "          <li><a href=\"" + RedirectUrl.Replace("%SETTINGSCODE%", LinkCode) + "\">" + Text + " </a></li>";
		}

		#endregion

		#region HTML helper methods for snippets used in multiple spots, like buttons

		private void add_buttons(TextWriter Output)
		{
			Output.WriteLine("  <div class=\"sbkSeav_ButtonsDiv\">");
			if (RequestSpecificValues.Current_User.Is_System_Admin)
			{

				Output.WriteLine("    <button title=\"Do not apply changes\" class=\"sbkAdm_RoundButton\" onclick=\"window.location.href='" + RequestSpecificValues.Current_Mode.Base_URL + "my/admin'; return false;\"><img src=\"" + Static_Resources_Gateway.Button_Previous_Arrow_Png + "\" class=\"sbkAdm_RoundButton_LeftImg\" alt=\"\" /> CANCEL</button> &nbsp; &nbsp; ");
				Output.WriteLine("    <button title=\"Save changes\" class=\"sbkAdm_RoundButton\" onclick=\"admin_settings_save(); return false;\">SAVE <img src=\"" + Static_Resources_Gateway.Button_Next_Arrow_Png + "\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button>");
			}
			else
			{
				Output.WriteLine("    <button class=\"sbkAdm_RoundButton\" onclick=\"window.location.href='" + RequestSpecificValues.Current_Mode.Base_URL + "my/admin'; return false;\"><img src=\"" + Static_Resources_Gateway.Button_Previous_Arrow_Png + "\" class=\"sbkAdm_RoundButton_LeftImg\" alt=\"\" /> BACK</button> &nbsp; &nbsp; ");
			}
			Output.WriteLine("  </div>");
			Output.WriteLine();
		}

		#endregion

		#region HTML helper method for the top-level information page

		private void add_top_level_info(TextWriter Output)
		{;
		    Output.WriteLine("  <h2>System-Wide Settings</h2>");
            Output.WriteLine("  <p>This form allows a user to view all the main system-wide settings and configurations which allow the SobekCM web application and assorted related applications to function correctly within each custom architecture and each institution.  This page highlights the great degree of customizations that can be done within the SobekCM framework, through the settings in the database and the configuration files.</p>");
            Output.WriteLine("  <p>For more information about these settings, <a href=\"" + UI_ApplicationCache_Gateway.Settings.System.Help_URL(RequestSpecificValues.Current_Mode.Base_URL) + "adminhelp/settings\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</p>");

            // Add portal admin message
            if (!RequestSpecificValues.Current_User.Is_System_Admin)
            {
                Output.WriteLine("    <p>Portal Admins have rights to see these settings. System Admins can change these settings.</p>");
            }

            Output.WriteLine("  <div id=\"sbkSeav_SubPageDesc\">");

            Output.WriteLine("    <p id=\"sbkSeav_SubPageTitle\">This screen contains the following sections:</p>");
            Output.WriteLine("    <h4><a href=\"" + redirectUrl.Replace("%SETTINGSCODE%", "settings") + "\">Settings</a></h4>");
            Output.WriteLine("    <p>This section includes all the basic setting information which governs the very basic operation of this SobekCM instance. These settings include information about the actual servers, email information, help settings, top-level search settings, and more. These settings are retained in the database and can be changed directly from these forms.</p>");

            Output.WriteLine("    <h4><a href=\"" + redirectUrl.Replace("%SETTINGSCODE%", "config") + "\">Configuration Files</a></h4>");
            Output.WriteLine("    <p>The SobekCM is highly configurable and extensible through the use of configuration files and plug-ins/extensions. Most of the content displayed in this form is derived from the configuration files which are read when the application starts.  This section provides information on the files that were read and any errors that may have occurred.</p>");

            Output.WriteLine("    <h4><a href=\"" + redirectUrl.Replace("%SETTINGSCODE%", "builder") + "\">Builder</a></h4>");
            Output.WriteLine("    <p>The builder runs in the background and handles bulk loading, reprocessing of recently loaded materials, and other regular maintenance tasks. Generally, the builder will look at incoming folders and look for newly loaded materials once every 60 seconds.  The subpages under this section provide information on the basic settings for the builder, the incoming folders which the builder watches for newly bulk loaded materials, and the builder modules that define the work it completes.</p>");

            Output.WriteLine("    <h4><a href=\"" + redirectUrl.Replace("%SETTINGSCODE%", "metadata") + "\">Metadata Configuration</a></h4>");
            Output.WriteLine("    <p>Metadata reading and writing within the system can be completely customized by utilizing the appropriate portions of the metadata configuration. This can control how overall metadata files are written as well as how the sections within a METS file are read and written.  This section provides information on this configuration and all of the different metadata readers and writers the system can employ.</p>");

            Output.WriteLine("    <h4><a href=\"" + redirectUrl.Replace("%SETTINGSCODE%", "engine") + "\">Engine Configuration</a></h4>");
            Output.WriteLine("    <p>Some of the core functionality is controlled by configuration files that are primarily consumed by the engine and provided to the user interface as needed.  The configuration information displayed here controls how the final display digital resource object is constructed from the METS-based object, what authentication is available for users of the system, and the overall behavior and metadata support for OAI-PMH.  In addition, this includes the list of all the microservice endpoints which are exposed by the engine for consumption by the user interface or other reporting tools.</p>");

            Output.WriteLine("    <h4><a href=\"" + redirectUrl.Replace("%SETTINGSCODE%", "ui") + "\">UI Configuration</a></h4>");
            Output.WriteLine("    <p>Many aspects of the final display of aggregations and digital resources are controlled by configuration files.  This configuration includes how the citation, or description, appears for digital resources, how the overall system displays HTML, which metadata fields can be exposed for editing, and the overall operation through the configuration of the microservices employed by the user interface.</p>");

            Output.WriteLine("    <h4><a href=\"" + redirectUrl.Replace("%SETTINGSCODE%", "html") + "\">HTML Snippets</a></h4>");
            Output.WriteLine("    <p>Several small snippets of HTML exist that allow an instance to easily customize several standard messages for your users. The subpages here allow you to view and edit these HTML snippets.</p>");

            Output.WriteLine("  </div>");

		}

		#endregion

		#region HTML helper methods for the settings main page and subpages

		private void add_settings_info(TextWriter Output)
		{
			int tabPage = -1;
			if ((RequestSpecificValues.Current_Mode.Remaining_Url_Segments != null) && (RequestSpecificValues.Current_Mode.Remaining_Url_Segments.Length > 1))
			{
				if (!Int32.TryParse(RequestSpecificValues.Current_Mode.Remaining_Url_Segments[1], out tabPage))
					tabPage = -1;
			}

			if ((tabPage < 1) || ( tabPage > tabPageNames.Count ))
			{
			    Output.WriteLine("  <span class=\"sbkSeav_BackUpLink\"><a href=\"" + redirectUrl.Replace("%SETTINGSCODE%", "") + "\">Back to top</a></span>");

				Output.WriteLine("  <h2>Settings</h2>");
                Output.WriteLine("  <p>This section includes all the basic setting information which governs the very basic operation of this SobekCM instance.  These settings include information about the actual servers, email information, help settings, top-level search settings, and more.  These settings are retained in the database and can be changed directly from these forms.</p>");
                Output.WriteLine("  <p>The (non-builder) settings are split between the following subpages:</p>");

                int tab_count = 1;
			    Output.WriteLine("  <ul>");
                foreach (string tabPageName in tabPageNames.Values)
                {
                    Output.WriteLine("    <li><a href=\"" + redirectUrl.Replace("%SETTINGSCODE%", "settings/" + tab_count ) + "\">" + tabPageName.Trim() + "</a></li>");
                    tab_count++;
                }
                Output.WriteLine("  </ul>");

                Output.WriteLine("  <p>All of the variables used to display the settings, such as the subpage names, the grouping of the settings, and the help information, are found in the database.  This list of settings can be extended by plug-ins and other custom mechanisms, if necessary.");

            }
            else
			{
                Output.WriteLine("  <span class=\"sbkSeav_BackUpLink\"><a href=\"" + redirectUrl.Replace("%SETTINGSCODE%", "settings") + "\">Back to Settings</a></span>");

                string tabPageNameKey = tabPageNames.Keys[tabPage - 1];
				string tabPageName = tabPageNames[tabPageNameKey];
				Output.WriteLine("  <h2>" + tabPageName.Trim() + "</h2>");

			    Output.WriteLine("  <div id=\"sbkSeav_SmallerPageWrapper\">");

				// Add the buttons
				add_buttons(Output);

				Output.WriteLine();
                Output.WriteLine("  <br /><br />");
                Output.WriteLine();

				add_tab_page_info(Output, tabPageName, settingsByPage[tabPageName]);

				Output.WriteLine("<br />");
				Output.WriteLine();

				// Add final buttons
				add_buttons(Output);

			    Output.WriteLine("  </div>");
			}
		}

		private void build_setting_objects_for_display()
		{
			// First step, get all the tab headings (excluding Deprecated and Builder)
			// and also categorize the values by tab page to start with
			tabPageNames = new SortedList<string, string>();
			settingsByPage = new Dictionary<string, List<Admin_Setting_Value>>();
            builderSettings = new List<Admin_Setting_Value>();
			foreach (Admin_Setting_Value thisValue in currSettings.Settings)
			{
				// If this is hidden, just do nothing
				if (thisValue.Hidden) continue;

				// If deprecated skip here
                if (String.Compare(thisValue.TabPage, "Deprecated", StringComparison.OrdinalIgnoreCase) == 0) continue;

                // Handle builder settings separately
			    if (String.Compare(thisValue.TabPage, "Builder", StringComparison.OrdinalIgnoreCase) == 0)
			    {
			        builderSettings.Add(thisValue);
			        continue;
			    }

				// Was this tab page already added?
				if (( thisValue.TabPage != null ) && (!settingsByPage.ContainsKey(thisValue.TabPage)))
				{
					// We are going to move 'General.." up to the front, others are in alphabetical order
					if (thisValue.TabPage.IndexOf("General", StringComparison.OrdinalIgnoreCase) == 0)
						tabPageNames.Add("00", thisValue.TabPage);
					else
						tabPageNames.Add(thisValue.TabPage, thisValue.TabPage);
					settingsByPage[thisValue.TabPage] = new List<Admin_Setting_Value> { thisValue };
				}
				else
				{
					settingsByPage[thisValue.TabPage].Add(thisValue);
				}
			}

			// Add some readonly configuration information from the config file
			// First, look for a server tab name
			string tabNameForConfig = tabPageNames.Values.FirstOrDefault(ThisTabName => ThisTabName.IndexOf("Server", StringComparison.OrdinalIgnoreCase) >= 0);
		    if (String.IsNullOrEmpty(tabNameForConfig))
			{
				foreach (string thisTabName in tabPageNames.Values.Where(ThisTabName => ThisTabName.IndexOf("System", StringComparison.OrdinalIgnoreCase) >= 0)) {
				    tabNameForConfig = thisTabName;
				    break;
				}
			}
			if (String.IsNullOrEmpty(tabNameForConfig))
			{
				tabNameForConfig = tabPageNames.Values[0];
			}

			// Build the values to add
			Admin_Setting_Value dbString = new Admin_Setting_Value
			{
				Heading = "Configuration Settings",
				Help = "Connection string used to connect to the SobekCM database\n\nThis value resides in the configuration file on the web server.  See your database and web server administrator to change this value.",
				Hidden = false,
				Key = "Database Connection String",
				Reserved = 3,
				SettingID = 9990,
				Value = UI_ApplicationCache_Gateway.Settings.Database_Connections[0].Connection_String
			};

			Admin_Setting_Value dbType = new Admin_Setting_Value
			{
				Heading = "Configuration Settings",
				Help = "Type of database used to drive the SobekCM system.\n\nCurrently, only Microsoft SQL Server is allowed with plans to add PostgreSQL and MySQL to the supported database system.\n\nThis value resides in the configuration on the web server.  See your database and web server administrator to change this value.",
				Hidden = false,
				Key = "Database Type",
				Reserved = 3,
				SettingID = 9991,
				Value = UI_ApplicationCache_Gateway.Settings.Database_Connections[0].Database_Type_String
			};

			Admin_Setting_Value isHosted = new Admin_Setting_Value
			{
				Heading = "Configuration Settings",
				Help = "Flag indicates if this instance is set as 'hosted', in which case a new Host Administrator role is added and some rights are reserved to that role which are normally assigned to system administrators.\n\nThis value resides in the configuration on the web server.  See your database and web server administrator to change this value.",
				Hidden = false,
				Key = "Hosted Intance",
				Reserved = 3,
				SettingID = 9994,
				Value = UI_ApplicationCache_Gateway.Settings.Servers.isHosted.ToString().ToLower()
			};

			Admin_Setting_Value errorEmails = new Admin_Setting_Value
			{
				Heading = "Configuration Settings",
				Help = "Email address for the web application to mail for any errors encountered while executing requests.\n\nThis account will be notified of inabilities to connect to servers, potential attacks, missing files, etc..\n\nIf the system is able to connect to the database, the 'System Error Email' address listed there, if there is one, will be used instead.\n\nUse a semi-colon betwen email addresses if multiple addresses are included.\n\nExample: 'person1@corp.edu;person2@corp2.edu'.\n\nThis value resides in the web.config file on the web server.  See your web server administrator to change this value.",
				Hidden = false,
				Key = "Error Emails",
				Reserved = 3,
				SettingID = 9992,
				Value = UI_ApplicationCache_Gateway.Settings.Email.System_Error_Email
			};

			Admin_Setting_Value errorWebPage = new Admin_Setting_Value
			{
				Heading = "Configuration Settings",
				Help = "Static page the user should be redirected towards if an unexpected exception occurs which cannot be handled by the web application.\n\nExample: 'http://ufdc.ufl.edu/error.html'.\n\nThis value resides in the web.config file on the web server.  See your web server administrator to change this value.",
				Hidden = false,
				Key = "Error Web Page",
				Reserved = 3,
				SettingID = 9993,
				Value = UI_ApplicationCache_Gateway.Settings.Servers.System_Error_URL
			};

			// Add them all to the tab page
			List<Admin_Setting_Value> settings = settingsByPage[tabNameForConfig];
			settings.Add(dbType);
			settings.Add(dbString);
			settings.Add(isHosted);
			settings.Add(errorEmails);
			settings.Add(errorWebPage);


		}

	    private void add_tab_page_info(TextWriter Output, string TabPageName, List<Admin_Setting_Value> AdminSettingValues, string OmitHeading = null)
		{
			// Start this table
			Output.WriteLine("        <table class=\"sbkSeav_SettingsTable\">");

			// Is this set for categorized via subheadings?
			if (category_view)
			{
				// Sort these admin settings within the headings
				SortedList<string, string> headingSorted = new SortedList<string, string>();
				Dictionary<string, SortedList<string, Admin_Setting_Value>> headingValuesSorted = new Dictionary<string, SortedList<string, Admin_Setting_Value>>();

				foreach (Admin_Setting_Value thisValue in AdminSettingValues)
				{
					if (!headingSorted.ContainsKey(thisValue.Heading))
					{
						headingSorted.Add(thisValue.Heading, thisValue.Heading);
						SortedList<string, Admin_Setting_Value> sortedList = new SortedList<string, Admin_Setting_Value> { { thisValue.Key, thisValue } };
						headingValuesSorted[thisValue.Heading] = sortedList;
					}
					else
					{
						headingValuesSorted[thisValue.Heading].Add(thisValue.Key, thisValue);
					}
				}

				// For now, just draw these
				bool firstHeading = true;
				bool oddRow = true;
				foreach (string thisHeading in headingSorted.Values)
				{
                    // If there was a heading value to omit, skip it here
				    if ((!String.IsNullOrEmpty(OmitHeading)) && (String.Compare(OmitHeading, thisHeading, StringComparison.OrdinalIgnoreCase) == 0))
				        continue;

					// Add a general heading
					Add_Setting_Table_Heading(Output, thisHeading, firstHeading);
					foreach (Admin_Setting_Value thisValueD in headingValuesSorted[thisHeading].Values)
					{
						// If this should be hidden, hide it
						if ((limitedRightsMode) && (thisValueD.Reserved >= 2)) continue;

						// Add this settings
						Add_Setting_Table_Setting(Output, thisValueD, oddRow);
						oddRow = !oddRow;
					}

					firstHeading = false;
				}
			}
			else  // Just add all the values alphabetically witout headers
			{
				// Sort these admin settings
				SortedList<string, Admin_Setting_Value> valuesSorted = new SortedList<string, Admin_Setting_Value>();

                // Add each value alphabetically
                // If there was a heading value to omit, skip it here
			    if (!String.IsNullOrEmpty(OmitHeading))
			    {
			        foreach (Admin_Setting_Value thisValue in AdminSettingValues)
			        {
			            if (String.Compare(OmitHeading, thisValue.Heading, StringComparison.OrdinalIgnoreCase) == 0)
			                continue;
			            valuesSorted[thisValue.Key] = thisValue;
			        }
			    }
			    else
			    {
			        foreach (Admin_Setting_Value thisValue in AdminSettingValues)
			        {
			            valuesSorted[thisValue.Key] = thisValue;
			        }
			    }

			    // Add a general heading
				Add_Setting_Table_Heading(Output, TabPageName, true);

				bool oddRow = true;
				foreach (Admin_Setting_Value thisValueD in valuesSorted.Values)
				{
					// If this should be hidden, hide it
					if ((limitedRightsMode) && (thisValueD.Reserved >= 2)) continue;

					// Add this settings
					Add_Setting_Table_Setting(Output, thisValueD, oddRow);
					oddRow = !oddRow;
				}
			}

			// Close this tab
			Output.WriteLine("        </table>");
		}

		private void Add_Setting_Table_Heading(TextWriter Output, string Heading, bool IsFirst)
		{
			Output.WriteLine("          <tr>");

			if (IsFirst)
			{
				Output.WriteLine("            <th colspan=\"2\">");
				Output.WriteLine("              " + Heading.ToUpper());
				Output.WriteLine("              <div style=\"float: right; text-align:right; padding-right: 40px;text-transform:none\">Order: <select id=\"reorder_select\" name=\"reorder_select\" onchange=\"settings_reorder(this);\"><option value=\"alphabetical\" selected=\"selected\">Alphabetical</option><option value=\"category\">Categories</option></select></div>");
				Output.WriteLine("            </th>");
			}
			else
			{
				Output.WriteLine("            <th colspan=\"2\">" + Heading.ToUpper() + "</th>");
			}

			Output.WriteLine("          </tr>");
		}

		private void Add_Setting_Table_Setting(TextWriter Output, Admin_Setting_Value Value, bool OddRow)
		{
			// Determine how to show this
			bool constant = Is_Value_ReadOnly(Value, readonlyMode, limitedRightsMode);

			Output.WriteLine(OddRow
					 ? "          <tr class=\"sbkSeav_TableEvenRow\">"
					 : "          <tr class=\"sbkSeav_TableOddRow\">");

			if (constant)
				Output.WriteLine("            <td class=\"sbkSeav_TableKeyCell\">" + Value.Key + ":</td>");
			else
				Output.WriteLine("            <td class=\"sbkSeav_TableKeyCell\"><label for=\"setting" + Value.SettingID + "\">" + Value.Key + "</label>:</td>");

			Output.WriteLine("            <td>");


			if (constant)
				Output.WriteLine("              <table class=\"sbkSeav_InnerTableConstant\">");
			else
				Output.WriteLine("              <table class=\"sbkSeav_InnerTable\">");
			Output.WriteLine("                <tr style=\"vertical-align:middle;border:0;\">");
			Output.WriteLine("                  <td style=\"max-width: 650px;\">");

			// Determine how to show this
			if (constant)
			{
				// Readonly for this value
				if (String.IsNullOrWhiteSpace(Value.Value))
				{
					Output.WriteLine("                    <em>( no value )</em>");
				}
				else
				{
					Output.WriteLine("                    " + HttpUtility.HtmlEncode(Value.Value).Replace(",", ", "));
				}
			}
			else
			{
				// Get the value, for easy of additional checks
				string setting_value = String.IsNullOrEmpty(Value.Value) ? String.Empty : Value.Value;


				if ((Value.Options != null) && (Value.Options.Count > 0))
				{

					Output.WriteLine("                    <select id=\"setting" + Value.SettingID + "\" name=\"setting" + Value.SettingID + "\" class=\"sbkSeav_select\" >");

					bool option_found = false;
					foreach (string thisValue in Value.Options)
					{
						if (String.Compare(thisValue, setting_value, StringComparison.OrdinalIgnoreCase) == 0)
						{
							option_found = true;
							Output.WriteLine("                      <option selected=\"selected\">" + setting_value + "</option>");
						}
						else
						{
							Output.WriteLine("                      <option>" + thisValue + "</option>");
						}
					}

					if (!option_found)
					{
						Output.WriteLine("                      <option selected=\"selected\">" + setting_value + "</option>");
					}
					Output.WriteLine("                    </select>");
				}
				else
				{
					if ((Value.Width.HasValue) && (Value.Width.Value > 0))
						Output.WriteLine("                    <input id=\"setting" + Value.SettingID + "\" name=\"setting" + Value.SettingID + "\" class=\"sbkSeav_input sbkAdmin_Focusable\" type=\"text\"  style=\"width: " + Value.Width + "px;\" value=\"" + HttpUtility.HtmlEncode(setting_value) + "\" />");
					else
						Output.WriteLine("                    <input id=\"setting" + Value.SettingID + "\" name=\"setting" + Value.SettingID + "\" class=\"sbkSeav_input sbkAdmin_Focusable\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(setting_value) + "\" />");
				}
			}

			Output.WriteLine("                  </td>");
			Output.WriteLine("                  <td>");
			if (!String.IsNullOrEmpty(Value.Help))
				Output.WriteLine("                    <img  class=\"sbkSeav_HelpButton\" src=\"" + Static_Resources_Gateway.Help_Button_Jpg + "\" onclick=\"alert('" + Value.Help.Replace("'", "").Replace("\"", "").Replace("\n", "\\n") + "');\"  title=\"" + Value.Help.Replace("'", "").Replace("\"", "").Replace("\n", "\\n") + "\" />");
			Output.WriteLine("                  </td>");
			Output.WriteLine("                </tr>");
			Output.WriteLine("              </table>");
			Output.WriteLine("            </td>");
			Output.WriteLine("          </tr>");
		}

		#region Methods related to special validations

		private bool validate_update_entered_data(List<Simple_Setting> NewValues)
		{
			isValid = true;
			foreach (Simple_Setting thisSetting in NewValues)
			{
				string value = thisSetting.Value;
				string key = thisSetting.Key;

				switch (key)
				{
					case "Application Server Network":
						must_end_with(thisSetting, "\\");
						break;

					case "Application Server URL":
						must_start_end_with(thisSetting, new string[] { "http://", "https://" }, "/");
						break;

					case "Document Solr Index URL":
						must_start_end_with(thisSetting, new string[] { "http://", "https://" }, "/");
						break;

					case "Files To Exclude From Downloads":
						must_be_valid_regular_expression(thisSetting);
						break;

					case "Image Server Network":
						must_end_with(thisSetting, "\\");
						break;

					case "Image Server URL":
						must_start_end_with(thisSetting, new string[] { "http://", "https://" }, "/");
						break;

					case "JPEG Height":
						must_be_positive_number(thisSetting);
						break;

					case "JPEG Width":
						must_be_positive_number(thisSetting);
						break;

					case "Log Files Directory":
						must_end_with(thisSetting, "\\");
						break;

					case "Log Files URL":
						must_start_end_with(thisSetting, new string[] { "http://", "https://" }, "/");
						break;

					case "Mango Union Search Base URL":
						must_start_with(thisSetting, new string[] { "http://", "https://" });
						break;

					case "Mango Union Search Text":
						if (value.Trim().Length > 0)
						{
							if (value.IndexOf("%1") < 0)
							{
								isValid = false;
								errorBuilder.AppendLine(key + ": Value must contain the '%1' string.  See help for more information.");
							}
						}
						break;

					case "MarcXML Feed Location":
						must_end_with(thisSetting, "\\");
						break;

					case "OAI Resource Identifier Base":
						must_end_with(thisSetting, ":");
						break;

					case "Page Solr Index URL":
						must_start_end_with(thisSetting, new string[] { "http://", "https://" }, "/");
						break;

					case "PostArchive Files To Delete":
						must_be_valid_regular_expression(thisSetting);
						break;

					case "PreArchive Files To Delete":
						must_be_valid_regular_expression(thisSetting);
						break;

					case "Static Pages Location":
						must_end_with(thisSetting, "\\");
						break;

					case "System Base Abbreviation":
						if (value.Trim().Length == 0)
						{
							isValid = false;
							errorBuilder.AppendLine(key + ": Field is required.");
						}
						break;

					case "System Base URL":
						if (value.Trim().Length == 0)
						{
							isValid = false;
							errorBuilder.AppendLine(key + ": Field is required.");
						}
						else
						{
							must_start_end_with(thisSetting, new string[] { "http://", "https://" }, "/");
						}
						break;

					case "Thumbnail Height":
						must_be_positive_number(thisSetting);
						break;

					case "Thumbnail Width":
						must_be_positive_number(thisSetting);
						break;

					case "Web In Process Submission Location":
						must_end_with(thisSetting, "\\");
						break;
				}
			}

			return isValid;
		}

		private void must_be_positive_number(Simple_Setting NewSetting)
		{
			bool appears_valid = false;
			int number;
			if ((Int32.TryParse(NewSetting.Value, out number)) && (number >= 0))
			{
				appears_valid = true;
			}

			if (!appears_valid)
			{
				isValid = false;
				errorBuilder.AppendLine(NewSetting.Key + ": Value must be a positive integer or zero.");
			}
		}

		private void must_be_valid_regular_expression(Simple_Setting NewSetting)
		{
			if (NewSetting.Value.Length == 0)
				return;

			// This is really just a check that it is a valid regular expression by 
			// attempting to perform a regular expression match.  The match itself 
			// ( and the resulting value ) is not important.
			try
			{
				Regex.Match("any_old_file.tif", NewSetting.Value);
			}
			catch (ArgumentException)
			{
				isValid = false;
				errorBuilder.AppendLine(NewSetting.Key + ": Value must be empty or a valid regular expression.");
			}
		}

		private static void must_start_with(Simple_Setting NewSetting, string StartsWith)
		{
			if (NewSetting.Value.Length == 0)
				return;

			if (!NewSetting.Value.StartsWith(StartsWith, StringComparison.OrdinalIgnoreCase))
			{
				NewSetting.Value = StartsWith + NewSetting.Value;
			}
		}

		private static void must_start_with(Simple_Setting NewSetting, string[] StartsWith)
		{
			if (NewSetting.Value.Length == 0)
				return;

			// Check for the start against all possible combinations
			bool missing_start = StartsWith.All(possibleStart => !NewSetting.Value.StartsWith(possibleStart, StringComparison.OrdinalIgnoreCase));

		    if (missing_start)
			{
				NewSetting.Value = StartsWith[0] + NewSetting.Value;
			}
		}

		private static void must_end_with(Simple_Setting NewSetting, string EndsWith)
		{
			if (NewSetting.Value.Length == 0)
				return;

			if (!NewSetting.Value.EndsWith(EndsWith, StringComparison.OrdinalIgnoreCase))
			{
				NewSetting.Value = NewSetting.Value + EndsWith;
			}
		}

		private static void must_start_end_with(Simple_Setting NewSetting, string StartsWith, string EndsWith)
		{
			if (NewSetting.Value.Length == 0)
				return;

			if ((!NewSetting.Value.StartsWith(StartsWith, StringComparison.OrdinalIgnoreCase)) || (!NewSetting.Value.EndsWith(EndsWith, StringComparison.InvariantCultureIgnoreCase)))
			{
				if (!NewSetting.Value.StartsWith(StartsWith, StringComparison.OrdinalIgnoreCase))
					NewSetting.Value = StartsWith + NewSetting.Value;
				if (!NewSetting.Value.EndsWith(EndsWith, StringComparison.OrdinalIgnoreCase))
					NewSetting.Value = NewSetting.Value + EndsWith;
			}
		}

		private static void must_start_end_with(Simple_Setting NewSetting, string[] StartsWith, string EndsWith)
		{
			if (NewSetting.Value.Length == 0)
				return;

			// Check for the start against all possible combinations
			bool missing_start = StartsWith.All(PossibleStart => !NewSetting.Value.StartsWith(PossibleStart, StringComparison.OrdinalIgnoreCase));

		    if ((missing_start) || (!NewSetting.Value.EndsWith(EndsWith, StringComparison.OrdinalIgnoreCase)))
			{
				if (missing_start)
					NewSetting.Value = StartsWith[0] + NewSetting.Value;
				if (!NewSetting.Value.EndsWith(EndsWith, StringComparison.OrdinalIgnoreCase))
					NewSetting.Value = NewSetting.Value + EndsWith;
			}
		}

		#endregion

		#endregion

        #region HTML helper methods for the configuration main pages and subpages


        private void add_configuration_info(TextWriter Output)
        {
            // If a submode existed, call that method
            switch (configSubMode)
            {
                case Settings_Configuration_SubMode_Enum.Files:
                    add_configuration_file_info(Output);
                    break;

                case Settings_Configuration_SubMode_Enum.Reading_Log:
                    add_configuration_log_info(Output);
                    break;

                default:
                    add_configuration_toplevel_info(Output);
                    break;
            }
        }

        private void add_configuration_toplevel_info(TextWriter Output)
        {
            Output.WriteLine("  <span class=\"sbkSeav_BackUpLink\"><a href=\"" + redirectUrl.Replace("%SETTINGSCODE%", "") + "\">Back to top</a></span>");

            Output.WriteLine("  <h2>Configuration Files</h2>");

            Output.WriteLine("  <p>The SobekCM is highly configurable and extensible through the use of configuration files and plug-ins/extensions.  Most of the content displayed in this form is derived from the configuration files which are read when the application starts.</p>");
            Output.WriteLine("  <p>The subpages under this page provide information on the files that were read and any errors that may have occurred.</p>");
            Output.WriteLine("  <div id=\"sbkSeav_SubPageDesc\">");
            Output.WriteLine("    <p id=\"sbkSeav_SubPageTitle\">This page contains the following subpages:</p>");
            Output.WriteLine("    <h4><a href=\"" + redirectUrl.Replace("%SETTINGSCODE%", "config/files") + "\">Source Files</a></h4>");
            Output.WriteLine("    <p>This subpage lists all the configuration files which were discovered during system start-up and lists the files in the order they would be read.</p>");
            Output.WriteLine("    <p>If the configuration from a particular file does not appear to be working, this is a great place to start.  Here, you can see whether the configuration file is marked to be read and see the order in which it is being applied.</p>");


            Output.WriteLine("    <h4><a href=\"" + redirectUrl.Replace("%SETTINGSCODE%", "config/files") + "\">Reading Log</a></h4>");
            Output.WriteLine("    <p>The reading log subpage shows the log which was written as the configuration files were read.  This log includes which configuration files were read, which configuration sections were found in each file, and any errors that may have occurred during the reading process.</p>");
            Output.WriteLine("    <p>If a configuration file's contents are not being applied correctly, and you have already verified the file is included in the list in the subpage above, this is a great next step.  From here, you can verify if the file was read, if any errors occurred during that process, and which sections of the configuration file were recognized by the reader.</p>");
            Output.WriteLine("  </div>");

        }

        private void add_configuration_file_info(TextWriter Output)
        {
            Output.WriteLine("  <span class=\"sbkSeav_BackUpLink\"><a href=\"" + redirectUrl.Replace("%SETTINGSCODE%", "config") + "\">Back to Configuration Files</a></span>");

            Output.WriteLine("  <h2>Configuration Files</h2>");
            Output.WriteLine("  <p>The great bulk of the content displayed in this form is derived from the configuration files which are read when the application starts.</p>");
            Output.WriteLine("  <p>Below is the list of all the configuration files that were discovered during system start-up and lists the file in the order they would be read.</p>");
            Output.WriteLine("  <p>In general, the order that configuration files are read is:</p>");
            Output.WriteLine("    <ol>");
            Output.WriteLine("      <li>First, all the .config and .xml files under the config/default subfolder are read in alphabetical order.</li>");
            Output.WriteLine("      <li>All the .config and .xml files found in plug-in subfolders under the plugins subfolder are read.</li>");
            Output.WriteLine("      <li>Finally, all the .config and .xml files under the config/users subfolder are read and provide a last chance for an instance to override any default configuration or plug-in configuration.  This can be particularly useful if a collision is detected between two different plug-ins.</li>");
            Output.WriteLine("    </ol>");



            Output.WriteLine("  <table class=\"sbkSeav_BaseTable\" id=\"sbkSeav_ConfigFilesTable\">");
            Output.WriteLine("    <tr><th>Configuration File</th></tr>");

            foreach (string thisFile in UI_ApplicationCache_Gateway.Configuration.Source.Files)
            {
                Output.WriteLine("    <tr><td>" + thisFile + "</td></tr>");
            }
            Output.WriteLine("  </table>");
        }

        private void add_configuration_log_info(TextWriter Output)
        {
            Output.WriteLine("  <span class=\"sbkSeav_BackUpLink\"><a href=\"" + redirectUrl.Replace("%SETTINGSCODE%", "config") + "\">Back to Configuration Files</a></span>");

            Output.WriteLine("  <h2>Configuration Reading Log</h2>");

            Output.WriteLine("  <p>Below is the log which was written as the configuration files were read.  This log includes which configuration files were read, which configuration sections were found in each file, and any errors that may have occurred during the reading process.</p>");


            Output.WriteLine("  <div id=\"sbkSeav_ConfigReadingLog\">");
            Output.WriteLine("    <pre>");

            foreach (string thisLog in UI_ApplicationCache_Gateway.Configuration.Source.ReadingLog)
            {
                Output.WriteLine("      " + thisLog);
            }
            Output.WriteLine("    </pre>");
            Output.WriteLine("  </div>");
        }


        #endregion

        #region HTML helper methods for the builder main page and subpages

        private void add_builder_info(TextWriter Output)
		{
            // If a submode existed, call that method
		    switch (builderSubEnum)
		    {
		        case Settings_Builder_SubMode_Enum.Builder_Settings:
		            add_builder_settings_info(Output);
		            break;

		        case Settings_Builder_SubMode_Enum.Builder_Folders:
		            add_builder_folders_info(Output);
		            break;

		        case Settings_Builder_SubMode_Enum.Builder_Modules:
		            add_builder_modules_info(Output);
		            break;

                default:
		            add_builder_toplevel_info(Output);
		            break;
		    }
		}

	    private void add_builder_toplevel_info(TextWriter Output)
	    {
            // Look for some values
	        string lastRun = String.Empty;
	        string builderVersion = String.Empty;
	        string lastResult = String.Empty;
	        string currentBuilderMode = String.Empty;
	        foreach (Admin_Setting_Value thisValue in builderSettings)
	        {
	            switch (thisValue.Key.ToLower())
	            {
                    case "builder last run finished":
	                    lastRun = thisValue.Value;
	                    break;

                    case "builder version":
	                    builderVersion = thisValue.Value;
	                    break;

                    case "builder last message":
	                    lastResult = thisValue.Value;
	                    break;

                    case "builder operation flag":
	                    currentBuilderMode = thisValue.Value;
	                    break;
	            }
	        }

            Output.WriteLine("  <span class=\"sbkSeav_BackUpLink\"><a href=\"" + redirectUrl.Replace("%SETTINGSCODE%", "") + "\">Back to top</a></span>");

            Output.WriteLine("  <h2>Builder Information</h2>");

            // Look to see if any builder folders exist
	        if (String.IsNullOrEmpty(lastRun))
	        {
                Output.WriteLine("  <div id=\"sbkAdm_ActionMessageWarning\">The builder has never run against this instance!<br /><br /><p>Make sure the builder is running and includes a reference to the database for this instance.</p></div>");
	        }
            else if ((UI_ApplicationCache_Gateway.Settings.Builder.IncomingFolders == null) || (UI_ApplicationCache_Gateway.Settings.Builder.IncomingFolders.Count == 0))
            {
                Output.WriteLine("  <div id=\"sbkAdm_ActionMessageWarning\">There are no incoming builder folders defined!<br /><br />You should probably add a builder folder by <a href=\"" + redirectUrl.Replace("%SETTINGSCODE%", "builder/folders") + "\">clicking here</a>.</div>");
            }
            
            Output.WriteLine("  <p>The builder runs in the background and handles bulk loading, reprocessing of recently loaded materials, and other regular maintenance tasks.  Generally, the builder will look at incoming folders and look for newly loaded materials once every 60 seconds.</p>");

            Output.WriteLine("  <p>The table below reflects information obtained the last time the builder ran against this SobekCM instance.</p>");

            Output.WriteLine("  <table class=\"sbkSeav_BaseTableVert\" id=\"sbkSeav_BuilderTopInfo\">");
            Output.WriteLine("    <tr><th>Last Run:</th><td>" + lastRun + "</td></tr>");
            Output.WriteLine("    <tr><th>Last Result:</th><td>" + lastResult + "</td></tr>");
            Output.WriteLine("    <tr><th>Builder Version:</th><td>" + builderVersion + "</td></tr>");
            Output.WriteLine("    <tr><th>Builder Operation:</th><td>" + currentBuilderMode + "</td></tr>");
            Output.WriteLine("  </table>");

            Output.WriteLine("  <p>The subpages under this page provide information on the basic settings for the builder, the incoming folders which the builder watches for newly bulk loaded materials, and the builder modules that define the work it completes. </p>");

            Output.WriteLine("  <div id=\"sbkSeav_SubPageDesc\">");
            Output.WriteLine("    <p id=\"sbkSeav_SubPageTitle\">This page contains the following subpages:</p>");
            Output.WriteLine("    <h4><a href=\"" + redirectUrl.Replace("%SETTINGSCODE%", "builder/settings") + "\">Builder Settings</a></h4>");
            Output.WriteLine("    <p>The builder settings subpage lists all settings that help to control the behavior while processing items.  These values are all from the database and generally reflect the capability of the system running the builder.</p>");

            Output.WriteLine("    <h4><a href=\"" + redirectUrl.Replace("%SETTINGSCODE%", "builder/folders") + "\">Incoming Folders</a></h4>");
	        Output.WriteLine("    <p>This subpage lists all of the existing incoming folders.  In addition, it allows new incoming builder folders to be added and existing incoming builder folders to be modified.</p>");
	        Output.WriteLine("    <p>One of the primary tasks of the builder is to handle bulk ingest of digital resources and digital resource files.  To facilitate this process, the builder can look in multiple different incoming folders and can process the files differently, depending on which folder they come in on.  Usually one incoming folder is set to a FTP location as well, to allow FTP of new materials directly into your SobekCM instance.</p>");

            Output.WriteLine("    <h4><a href=\"" + redirectUrl.Replace("%SETTINGSCODE%", "builder/modules") + "\">Builder Modules</a></h4>");
	        Output.WriteLine("    <p>The builder modules subpage shows the list of all the builder modules which will run during each phase of the builder's processing.</p>");
            Output.WriteLine("    <p>The builder operates as a framework of builder modules operating at different levels.  Several module types operate across the breadth of the entire repository, or for a single polling.  Other modules work at the level of each individual incoming folder.  And finally, other modules perform work against each individual incoming resource or resource flagged for additional work.</p>");
            Output.WriteLine("  </div>");

	    }

        private void add_builder_settings_info(TextWriter Output)
        {
            Output.WriteLine("  <h2>Builder Settings</h2>");

            Output.WriteLine("  <p>The builder utilizes a number of settings, stored within the database, to control its behavior while processing items.  These settings can be changed here by a system administrator</p>");

            Output.WriteLine("  <p>The first group of settings controls files which the application makes available to be archived.  If an archival folder is set, the builder can copy all incoming materials to this folder.  This folder would become the point of entry into your own archiving system.  In addition, files can be selected to be deleted before archiving (if extraneous files are entering your archiving workflow) or after archiving (if you want the files to not be retained with the other digital resource files).</p>");

            Output.WriteLine("  <p>The second group of settings reflect the ability of the machine that is running the builder and provides several spots to override the default behavior.  For example, if you want to modify the standard JPEG2000 creation script, you can provide your own script to be called.</p>");

            Output.WriteLine("  <p>Many of these settings work in concert with the builder module configuration.  For example, if you deselect the JPEG2000 creation builder module, it doesn't matter what you put in the field to override the JPEG2000 creation script.</p>");

            Output.WriteLine("  <p>Changes you make here will be reflected almost immediately by the builder.</p>");

            Output.WriteLine("  <div id=\"sbkSeav_SmallerPageWrapper\">");

            // Add the buttons
            add_buttons(Output);

            Output.WriteLine();
            Output.WriteLine("<br /><br />");
            Output.WriteLine();

            Output.WriteLine();
            add_tab_page_info(Output, "Builder Settings", builderSettings, "Status");

            Output.WriteLine("<br />");
            Output.WriteLine();

            // Add final buttons
            add_buttons(Output);

            Output.WriteLine("  </div>");
        }

        private void add_builder_folders_info(TextWriter Output)
        {
            Output.WriteLine("  <h2>Builder Folders</h2>");

            Output.WriteLine("  <p>One of the primary tasks of the builder is to handle bulk ingest of digital resources and digital resource files.  To facilitate this process, the builder can look in multiple different incoming folders and can process the files differently, depending on which folder they come in on.  Usually one incoming folder is set to a FTP location as well, to allow FTP of new materials directly into your SobekCM instance.</p>");

            bool allowEdit = (((!UI_ApplicationCache_Gateway.Settings.Servers.isHosted) && ( RequestSpecificValues.Current_User.Is_System_Admin )) || (RequestSpecificValues.Current_User.Is_Host_Admin));
            if (allowEdit)
            {
                Output.WriteLine("  <p>You can edit or delete an existing builder folder from this form.  In addition, new builder folders can also be added from this form by selecting Add New Builder Folder button below.</p>");

                Output.WriteLine("  <p>Changes you make here will be reflected almost immediately by the builder.</p>");
            }

            Output.WriteLine("  <h3>Existing Builder Folders</h3>");
            // Look to see if any builder folders exist
            if ((UI_ApplicationCache_Gateway.Settings.Builder.IncomingFolders == null) || (UI_ApplicationCache_Gateway.Settings.Builder.IncomingFolders.Count == 0))
            {
                Output.WriteLine("  <br /><br />");
                Output.WriteLine("  <strong>YOU HAVE NO BUILDER FOLDERS DEFINED!!</strong>");
            }
            else
            {
                if (allowEdit)
                {
                    Output.WriteLine("  <p>Below is the list of all builder incoming folders, along with the network folders and indicated options.  You can edit an existing folder by selected EDIT under the actions of that individual folder and you can additionally add new incoming folders.</p>");
                }
                else
                {
                    Output.WriteLine("  <p>Below is the list of all builder incoming folders, along with the network folders and indicated options. </p>");
                }

                // Show the information for each builder folder
                foreach (Builder_Source_Folder incomingFolder in UI_ApplicationCache_Gateway.Settings.Builder.IncomingFolders)
                {
                    // Add the basic folder information
                    Output.WriteLine("  <span class=\"sbkSeav_BuilderFolderName\">" + incomingFolder.Folder_Name + "</span>");
                    Output.WriteLine("  <table class=\"sbkSeav_BaseTableVert sbkSeav_BuilderFolderTable\">");
                    Output.WriteLine("    <tr><th>Inbound Folder:</th><td>" + incomingFolder.Inbound_Folder + "</td></tr>");
                    Output.WriteLine("    <tr><th>Processing Folder:</th><td>" + incomingFolder.Processing_Folder + "</td></tr>");
                    Output.WriteLine("    <tr><th>Failures Folder:</th><td>" + incomingFolder.Failures_Folder + "</td></tr>");

                    // If there are BibID restrictions, add them
                    if (!String.IsNullOrEmpty(incomingFolder.BibID_Roots_Restrictions))
                        Output.WriteLine("    <tr><th>BibID Restrictions:</th><td>" + incomingFolder.BibID_Roots_Restrictions + "</td></tr>");

                    // Collect all the options and display them
                    StringBuilder builder = new StringBuilder();
                    if (incomingFolder.Allow_Deletes) builder.Append("Allow deletes ; ");
                    if (incomingFolder.Allow_Folders_No_Metadata) builder.Append("Allow folder with no metadata ; ");
                    if (incomingFolder.Allow_Metadata_Updates) builder.Append("Allow metadata updates ; ");
                    if (incomingFolder.Archive_All_Files) builder.Append("Archive all files ; ");
                    else if (incomingFolder.Archive_TIFFs) builder.Append("Archive TIFFs ; ");
                    if (incomingFolder.Perform_Checksum) builder.Append("Perform checksum ; ");
                    Output.WriteLine("    <tr><th>Options</th><td>" + builder + "</td></tr>");

                    // Add the possible actions here
                    if (allowEdit)
                    {
                        Output.WriteLine("    <tr><th>Actions:</th><td><a href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "admin/builderfolder/" + incomingFolder.IncomingFolderID + "\">edit</a></td></tr>");
                    }
                    Output.WriteLine("  </table>");
                }
            }

            if (allowEdit)
            {
                Output.WriteLine("  <h3>New Builder Folder</h3>");
                Output.WriteLine("  <table id=\"sbkSeav_BuilderNewFolderDisplay\">");
                Output.WriteLine("    <tr>");
                Output.WriteLine("      <td id=\"sbkSeav_BuilderNewFolderDisplay_TextCol\">");
                Output.WriteLine("        <p>Press the button to the right to add the configuration for a new builder incoming folder.  If this is the first folder you add, it will also be the default for doing bulk imports via the SMaRT tool.</p>");
                Output.WriteLine("      </td>");
                Output.WriteLine("      <td id=\"sbkSeav_BuilderNewFolderDisplay_ButtonCol\">");
                Output.WriteLine("        <button title=\"Add configuration for a new builder incoming folder\" id=\"sbkSeav_BuilderNewFolderButton\" class=\"sbkAdm_RoundButton\" onclick=\"window.location.href='" + RequestSpecificValues.Current_Mode.Base_URL + "admin/builderfolder/new';return false;\">ADD NEW<br />BUILDER FOLDER</button>");
                Output.WriteLine("      </td>");
                Output.WriteLine("    </tr>");
                Output.WriteLine("  </table>");
            }



        }

        private void add_builder_modules_info(TextWriter Output)
        {
            Output.WriteLine("  <h2>Builder Modules</h2>");

            Output.WriteLine("  <p>The builder operates as a framework of builder modules operating at different levels.  Several module types operate across the breadth of the entire repository, or for a single polling.  Other modules work at the level of each individual incoming folder.  And finally, other modules perform work against each individual incoming resource or resource flagged for additional work.</p>");
            Output.WriteLine("  <p>This architecture module allows for the overall process of the builder (and hence the SobekCM instance) to be extremely customizable.  By creating a DLL and a configuration file, you can greatly impact the overall process of the builder.</p>");

            // Add all the PRE PROCESS MODULE settings
            if ((UI_ApplicationCache_Gateway.Settings.Builder.PreProcessModulesSettings != null) && (UI_ApplicationCache_Gateway.Settings.Builder.PreProcessModulesSettings.Count > 0))
            {
                Output.WriteLine("  <h3>Pre-Process Modules</h3>");
                Output.WriteLine("  <p>Pre-process modules are executed before the builder polls the database and incoming folders for work to complete.  These modules can either be a good place to perform repository-wide work that should occur very frequently, or to do some preparation work such as putting resources or metadata files INTO the incoming folders that are checked next.</p>");
                Output.WriteLine("  <p>In addition, if a custom module in another portion of the processing needs configuration data, these modules can pull data from the database or read configuration files.</p>");
                Output.WriteLine("  <table class=\"sbkSeav_BaseTable sbkSeav_BuilderModulesTable\">");
                Output.WriteLine("    <tr>");
                Output.WriteLine("      <th class=\"sbkSeav_BuilderModulesTable_ClassCol\">Class</th>");
                Output.WriteLine("      <th class=\"sbkSeav_BuilderModulesTable_DescCol\">Description</th>");
                Output.WriteLine("      <th class=\"sbkSeav_BuilderModulesTable_ArgsCol\">Arguments</th>");
                Output.WriteLine("    </tr>");
                foreach (Builder_Module_Setting thisModule in UI_ApplicationCache_Gateway.Settings.Builder.PreProcessModulesSettings)
                {
                    Output.WriteLine("    <tr>");
                    if ( !String.IsNullOrEmpty(thisModule.Assembly))
                        Output.WriteLine("      <td>" +  thisModule.Class + " ( " + thisModule.Assembly + " )</td>");
                    else
                        Output.WriteLine("      <td>" + thisModule.Class.Replace("SobekCM.Builder_Library.Modules.PreProcess.", "") + "</td>");
                    Output.WriteLine("      <td>" +  thisModule.Description + "</td>");

                    if ((!String.IsNullOrEmpty(thisModule.Argument1)) || (!String.IsNullOrEmpty(thisModule.Argument2)) || (!String.IsNullOrEmpty(thisModule.Argument3)))
                    {
                        Output.WriteLine("      <td>");
                        if (!String.IsNullOrEmpty(thisModule.Argument1))
                        {
                            Output.WriteLine("        (1) " + thisModule.Argument1);
                            Output.WriteLine();
                        }
                        if (!String.IsNullOrEmpty(thisModule.Argument2))
                        {
                            Output.WriteLine("        (2) " + thisModule.Argument2);
                            Output.WriteLine();
                        }
                        if (!String.IsNullOrEmpty(thisModule.Argument3))
                        {
                            Output.WriteLine("        (3) " + thisModule.Argument3);
                            Output.WriteLine();
                        }
                        Output.WriteLine("      </td>");
                    }
                    else
                    {
                        Output.WriteLine("      <td></td>");
                    }
                    Output.WriteLine("    </tr>");
                }
                Output.WriteLine("  </table>");
            }

            // Add all the ITEM PROCESS MODULE settings
            if ((UI_ApplicationCache_Gateway.Settings.Builder.ItemProcessModulesSettings != null) && (UI_ApplicationCache_Gateway.Settings.Builder.ItemProcessModulesSettings.Count > 0))
            {
                Output.WriteLine("  <h3>Item Processing Modules</h3>");
                Output.WriteLine("  <p>These are the modules that are used to process both new incoming (non-delete) packages as well as existing packages that are flagged to have additional work completed.</p>");
                Output.WriteLine("  <table class=\"sbkSeav_BaseTable sbkSeav_BuilderModulesTable\">");
                Output.WriteLine("    <tr>");
                Output.WriteLine("      <th class=\"sbkSeav_BuilderModulesTable_ClassCol\">Class</th>");
                Output.WriteLine("      <th class=\"sbkSeav_BuilderModulesTable_DescCol\">Description</th>");
                Output.WriteLine("      <th class=\"sbkSeav_BuilderModulesTable_ArgsCol\">Arguments</th>");
                Output.WriteLine("    </tr>");
                foreach (Builder_Module_Setting thisModule in UI_ApplicationCache_Gateway.Settings.Builder.ItemProcessModulesSettings)
                {
                    Output.WriteLine("    <tr>");
                    if (!String.IsNullOrEmpty(thisModule.Assembly))
                        Output.WriteLine("      <td>" + thisModule.Class + " ( " + thisModule.Assembly + " )</td>");
                    else
                        Output.WriteLine("      <td>" + thisModule.Class.Replace("SobekCM.Builder_Library.Modules.Items.", "") + "</td>");
                    Output.WriteLine("      <td>" + thisModule.Description + "</td>");

                    if ((!String.IsNullOrEmpty(thisModule.Argument1)) || (!String.IsNullOrEmpty(thisModule.Argument2)) || (!String.IsNullOrEmpty(thisModule.Argument3)))
                    {
                        Output.WriteLine("      <td>");
                        if (!String.IsNullOrEmpty(thisModule.Argument1))
                        {
                            Output.WriteLine("        (1) " + thisModule.Argument1);
                            Output.WriteLine();
                        }
                        if (!String.IsNullOrEmpty(thisModule.Argument2))
                        {
                            Output.WriteLine("        (2) " + thisModule.Argument2);
                            Output.WriteLine();
                        }
                        if (!String.IsNullOrEmpty(thisModule.Argument3))
                        {
                            Output.WriteLine("        (3) " + thisModule.Argument3);
                            Output.WriteLine();
                        }
                        Output.WriteLine("      </td>");
                    }
                    else
                    {
                        Output.WriteLine("      <td></td>");
                    }
                    Output.WriteLine("    </tr>");
                }
                Output.WriteLine("  </table>");
            }
            else
            {
                Output.WriteLine("  <h3>ERROR: NO ITEM PROCESSING MODULES SET!</h3>");
                Output.WriteLine("  <p>Any request for the processor to process items, either from an incoming folder or submitted online will fail.  This should be corrected immediately!</p>");
                Output.WriteLine("  <br /><br />");
            }

            // Add all the ITEM DELETE MODULE settings
            if ((UI_ApplicationCache_Gateway.Settings.Builder.ItemDeleteModulesSettings != null) && (UI_ApplicationCache_Gateway.Settings.Builder.ItemDeleteModulesSettings.Count > 0))
            {
                Output.WriteLine("  <h3>Item Deletion Modules</h3>");
                Output.WriteLine("  <p>This set of modules are run when an item is deleted from this instance.  Adding custom modules here can be useful for cleaning up resources that may be placed on other auxiliary servers or doing any other final cleanup actions.</p>");
                Output.WriteLine("  <table class=\"sbkSeav_BaseTable sbkSeav_BuilderModulesTable\">");
                Output.WriteLine("    <tr>");
                Output.WriteLine("      <th class=\"sbkSeav_BuilderModulesTable_ClassCol\">Class</th>");
                Output.WriteLine("      <th class=\"sbkSeav_BuilderModulesTable_DescCol\">Description</th>");
                Output.WriteLine("      <th class=\"sbkSeav_BuilderModulesTable_ArgsCol\">Arguments</th>");
                Output.WriteLine("    </tr>");
                foreach (Builder_Module_Setting thisModule in UI_ApplicationCache_Gateway.Settings.Builder.ItemDeleteModulesSettings)
                {
                    Output.WriteLine("    <tr>");
                    if (!String.IsNullOrEmpty(thisModule.Assembly))
                        Output.WriteLine("      <td>" + thisModule.Class + " ( " + thisModule.Assembly + " )</td>");
                    else
                        Output.WriteLine("      <td>" + thisModule.Class.Replace("SobekCM.Builder_Library.Modules.Items.","") + "</td>");
                    Output.WriteLine("      <td>" + thisModule.Description + "</td>");

                    if ((!String.IsNullOrEmpty(thisModule.Argument1)) || (!String.IsNullOrEmpty(thisModule.Argument2)) || (!String.IsNullOrEmpty(thisModule.Argument3)))
                    {
                        Output.WriteLine("      <td>");
                        if (!String.IsNullOrEmpty(thisModule.Argument1))
                        {
                            Output.WriteLine("        (1) " + thisModule.Argument1);
                            Output.WriteLine();
                        }
                        if (!String.IsNullOrEmpty(thisModule.Argument2))
                        {
                            Output.WriteLine("        (2) " + thisModule.Argument2);
                            Output.WriteLine();
                        }
                        if (!String.IsNullOrEmpty(thisModule.Argument3))
                        {
                            Output.WriteLine("        (3) " + thisModule.Argument3);
                            Output.WriteLine();
                        }
                        Output.WriteLine("      </td>");
                    }
                    else
                    {
                        Output.WriteLine("      <td></td>");
                    }
                    Output.WriteLine("    </tr>");
                }
                Output.WriteLine("  </table>");
            }
            else
            {
                Output.WriteLine("  <h3>ERROR: NO ITEM DELETE MODULES SET!</h3>");
                Output.WriteLine("  <p>Any deletion requests coming through an incoming builder folder will fail as a result.</p>");
                Output.WriteLine("  <br /><br />");
            }


            // Add all the POST PROCESS MODULE settings
            if ((UI_ApplicationCache_Gateway.Settings.Builder.PostProcessModulesSettings != null) && (UI_ApplicationCache_Gateway.Settings.Builder.PostProcessModulesSettings.Count > 0))
            {
                Output.WriteLine("  <h3>Post-Process Modules</h3>");
                Output.WriteLine("  <p>Post-process modules are executed after the builder completes all other tasks for a single execution.  These modules can either be a good place to perform repository-wide work that should occur after all new materials load or to release resources.</p>");

                Output.WriteLine("  <table class=\"sbkSeav_BaseTable sbkSeav_BuilderModulesTable\">");
                Output.WriteLine("    <tr>");
                Output.WriteLine("      <th class=\"sbkSeav_BuilderModulesTable_ClassCol\">Class</th>");
                Output.WriteLine("      <th class=\"sbkSeav_BuilderModulesTable_DescCol\">Description</th>");
                Output.WriteLine("      <th class=\"sbkSeav_BuilderModulesTable_ArgsCol\">Arguments</th>");
                Output.WriteLine("    </tr>");
                foreach (Builder_Module_Setting thisModule in UI_ApplicationCache_Gateway.Settings.Builder.PostProcessModulesSettings)
                {
                    Output.WriteLine("    <tr>");
                    if (!String.IsNullOrEmpty(thisModule.Assembly))
                        Output.WriteLine("      <td>" + thisModule.Class + " ( " + thisModule.Assembly + " )</td>");
                    else
                        Output.WriteLine("      <td>" + thisModule.Class.Replace("SobekCM.Builder_Library.Modules.PostProcess.", "") + "</td>");
                    Output.WriteLine("      <td>" + thisModule.Description + "</td>");

                    if ((!String.IsNullOrEmpty(thisModule.Argument1)) || (!String.IsNullOrEmpty(thisModule.Argument2)) || (!String.IsNullOrEmpty(thisModule.Argument3)))
                    {
                        Output.WriteLine("      <td>");
                        if (!String.IsNullOrEmpty(thisModule.Argument1))
                        {
                            Output.WriteLine("        (1) " + thisModule.Argument1);
                            Output.WriteLine();
                        }
                        if (!String.IsNullOrEmpty(thisModule.Argument2))
                        {
                            Output.WriteLine("        (2) " + thisModule.Argument2);
                            Output.WriteLine();
                        }
                        if (!String.IsNullOrEmpty(thisModule.Argument3))
                        {
                            Output.WriteLine("        (3) " + thisModule.Argument3);
                            Output.WriteLine();
                        }
                        Output.WriteLine("      </td>");
                    }
                    else
                    {
                        Output.WriteLine("      <td></td>");
                    }
                    Output.WriteLine("    </tr>");
                }
                Output.WriteLine("  </table>");
            }
        }

	    #endregion

        #region HTML methods for the metadata main page and subpages

        private void add_metadata_info(TextWriter Output)
		{
            // If a submode existed, call that method
            switch (metadataSubEnum)
            {
                case Settings_Metadata_SubMode_Enum.Fields:
                    add_metadata_fields_info(Output);
                    break;

                case Settings_Metadata_SubMode_Enum.Metdata_Reader_Writers:
                    add_metadata_file_readers_info(Output);
                    break;

                case Settings_Metadata_SubMode_Enum.METS_Section_Reader_Writers:
                    add_metadata_mets_sections_info(Output);
                    break;

                case Settings_Metadata_SubMode_Enum.METS_Writing_Profiles:
                    add_metadata_mets_profiles_info(Output);
                    break;

                case Settings_Metadata_SubMode_Enum.Metadata_Modules_To_Include:
                    add_metadata_modules_info(Output);
                    break;

                default:
                    add_metadata_toplevel_info(Output);
                    break;
            }
		}

        private void add_metadata_fields_info(TextWriter Output)
        {
            Output.WriteLine("  <h2>Metadata Search Fields</h2>");
            Output.WriteLine("  <p>These are all of the current search fields in the system that are utilized by the main searching methods.  These terms (and their link into the solr/lucene indexes) are defined in the database.</p>");
            Output.WriteLine("  <p>The columns are defined below:</p>");
            Output.WriteLine("  <ul>");
            Output.WriteLine("    <li><span style=\"font-weight:bold\">Name</span> - The name field is a unique value that defines the metadata field and is referenced while saving the metadata for a single digital resource.</li>");
            Output.WriteLine("    <li><span style=\"font-weight:bold\">Web Code</span> - The web code is the code used in the URL during an advanced search to indicate that this metadata field should be individually searched.</li>");
            Output.WriteLine("    <li><span style=\"font-weight:bold\">Display Term</span> - This field is the version that is generally used throughout the system when referencing this field.</li>");
            Output.WriteLine("    <li><span style=\"font-weight:bold\">Facet Term</span> - The facet term is the value used as the heading if facets from this field are displayed while searching or browsing digital resources within an aggregation.  This can be different than the general display term.</li>");
            Output.WriteLine("    <li><span style=\"font-weight:bold\">Solr Field</span> - This maps this metadata field from a digital resource into the appropriate solr/lucene field for indexing and searching.</li>");
            Output.WriteLine("  </ul>");

            Output.WriteLine("  <p>Generally, new metadata terms do not need to be added directly to the database.  If a new metadata module is added to the METS file, when the values are saved, new metadata fields will automatically be claimed by the SobekCM system and will then appear in the list below.</p>");

            // Create the data table
            DataTable tempTable = new DataTable();
            tempTable.Columns.Add("Name");
            tempTable.Columns.Add("WebCode");
            tempTable.Columns.Add("DisplayTerm");
            tempTable.Columns.Add("FacetTerm");
            tempTable.Columns.Add("SolrField");
            foreach (Metadata_Search_Field metadata in UI_ApplicationCache_Gateway.Settings.Metadata_Search_Fields)
            {
                DataRow newRow = tempTable.NewRow();
                newRow[0] = !String.IsNullOrEmpty(metadata.Name) ? metadata.Name : String.Empty;
                newRow[1] = !String.IsNullOrEmpty(metadata.Web_Code) ? metadata.Web_Code : String.Empty;
                newRow[2] = !String.IsNullOrEmpty(metadata.Display_Term) ? metadata.Display_Term : String.Empty;
                newRow[3] = !String.IsNullOrEmpty(metadata.Facet_Term) ? metadata.Facet_Term : String.Empty;
                newRow[4] = !String.IsNullOrEmpty(metadata.Solr_Field) ? metadata.Solr_Field : String.Empty;
                tempTable.Rows.Add(newRow);
            }

            // Determine the column to soret 
            string columnSort = "Name";
            string possibleOrder = HttpContext.Current.Request.QueryString["o"];
            if (!String.IsNullOrEmpty(possibleOrder))
            {
                switch (possibleOrder)
                {
                    case "2":
                        columnSort = "WebCode";
                        break;

                    case "3":
                        columnSort = "DisplayTerm";
                        break;

                    case "4":
                        columnSort = "FacetTerm";
                        break;

                    case "5":
                        columnSort = "SolrField";
                        break;
                }
            }

            // Create the data view
            DataView sortMetadata = new DataView(tempTable) {Sort = columnSort + " ASC"};

            Output.WriteLine("  <table class=\"sbkSeav_BaseTable\" id=\"sbkSeav_MetadataFieldsTable\">");
            Output.WriteLine("    <tr>");
         //   Output.WriteLine("      <th id=\"sbkSeav_MetadataReadersTable_TypeCol\">ID</th>");
            Output.WriteLine("      <th id=\"sbkSeav_MetadataFieldsTable_NameCol\">Name</th>");
            Output.WriteLine("      <th id=\"sbkSeav_MetadataFieldsTable_WebCodeCol\">Web Code</th>");
            Output.WriteLine("      <th id=\"sbkSeav_MetadataFieldsTable_DisplayTermCol\">Display Term</th>");
            Output.WriteLine("      <th id=\"sbkSeav_MetadataFieldsTable_FacetTermCol\">Facet Term</th>");
            Output.WriteLine("      <th id=\"sbkSeav_MetadataFieldsTable_SolrFieldCol\">Solr Field</th>");
            Output.WriteLine("    </tr>");

            // Step through all the basic metadata fields
            foreach (DataRowView thisRow in sortMetadata )
            {
                Output.WriteLine("    <tr>");
            //    Output.WriteLine("      <td>" + metadata.ID + "</td>");
                Output.WriteLine("      <td>" + thisRow[0] + "</td>");
                Output.WriteLine("      <td class=\"sbkSeav_TableCenterCell\">" + thisRow[1] + "</td>");
                Output.WriteLine("      <td>" + thisRow[2] + "</td>");
                Output.WriteLine("      <td>" + thisRow[3] + "</td>");
                Output.WriteLine("      <td>" + thisRow[4] + "</td>");

                Output.WriteLine("    </tr>");
            }
            Output.WriteLine("  </table>");
        }

        private void add_metadata_file_readers_info(TextWriter Output)
        {
            Output.WriteLine("  <h2>Metadata File Reader and Writers</h2>");
            Output.WriteLine("  <p>This is the complete list of different metadata readers and writers available within the system.  The referenced objects can read a complete metadata file and/or write a complete metadata file.   These are not the readers/writers used for individual sections within a METS file.</p>");
            Output.WriteLine("  <p>Just adding a new reader/writer in the configuration does not add any additional functionality to the system, as the system must know to reference the appropriate reader/writer.  However, it is possible to override the exsting class to change the way an individual exiting metadata file is used.  For example, by override the standard METS writer, you could begin to add a new section within the METS, or change the way the structMap is read and written.</p>");


            Output.WriteLine("  <table class=\"sbkSeav_BaseTable\" id=\"sbkSeav_MetadataReadersTable\">");
            Output.WriteLine("    <tr>");
            Output.WriteLine("      <th id=\"sbkSeav_MetadataReadersTable_TypeCol\">Type</th>");
            Output.WriteLine("      <th id=\"sbkSeav_MetadataReadersTable_LabelCol\">Label</th>");
            Output.WriteLine("      <th id=\"sbkSeav_MetadataReadersTable_CanReadCol\">Can Read</th>");
            Output.WriteLine("      <th id=\"sbkSeav_MetadataReadersTable_CanWriteCol\">Can Write</th>");
            Output.WriteLine("      <th id=\"sbkSeav_MetadataReadersTable_ClassCol\">Class</th>");
            Output.WriteLine("      <th id=\"sbkSeav_MetadataReadersTable_OptionsCol\">Options</th>");
            Output.WriteLine("    </tr>");

            // Step through all the basic metadata reader/writers
            foreach (Metadata_File_ReaderWriter_Config metadataReader in UI_ApplicationCache_Gateway.Configuration.Metadata.Metadata_File_ReaderWriter_Configs)
            {
                Output.WriteLine("    <tr>");
                switch (metadataReader.MD_Type)
                {
                    case Metadata_File_Type_Enum.DC:
                        Output.WriteLine("      <td>DC</td>");
                        break;

                    case Metadata_File_Type_Enum.EAD:
                        Output.WriteLine("      <td>EAD</td>");
                        break;

                    case Metadata_File_Type_Enum.MARC21:
                        Output.WriteLine("      <td>MARC21</td>");
                        break;

                    case Metadata_File_Type_Enum.MARCXML:
                        Output.WriteLine("      <td>MarcXML</td>");
                        break;

                    case Metadata_File_Type_Enum.METS:
                        Output.WriteLine("      <td>METS</td>");
                        break;

                    case Metadata_File_Type_Enum.MODS:
                        Output.WriteLine("      <td>MODS</td>");
                        break;

                    default:
                        Output.WriteLine("      <td>" + metadataReader.Other_MD_Type + "</td>");
                        break;
                }
                Output.WriteLine("      <td>" + metadataReader.Label + "</td>");

                // Add checkmark for can read
                if (metadataReader.canRead)
                    Output.WriteLine("      <td class=\"sbkSeav_TableCenterCell\"><img src=\"" + Static_Resources_Gateway.Checkmark2_Png + "\" alt=\"yes\" /></td>");
                else
                    Output.WriteLine("      <td class=\"sbkSeav_TableCenterCell\"><img src=\"" + Static_Resources_Gateway.Checkmark_Png + "\" alt=\"no\" /></td>");

                // Add checkmark for can write
                if (metadataReader.canWrite)
                    Output.WriteLine("      <td class=\"sbkSeav_TableCenterCell\"><img src=\"" + Static_Resources_Gateway.Checkmark2_Png + "\" alt=\"yes\" /></td>");
                else
                    Output.WriteLine("      <td class=\"sbkSeav_TableCenterCell\"><img src=\"" + Static_Resources_Gateway.Checkmark_Png + "\" alt=\"no\" /></td>");

                // Add the class, and optionally assembly
                string class_name_full = metadataReader.Code_Class;
                if (!String.IsNullOrEmpty(metadataReader.Code_Namespace))
                    class_name_full = metadataReader.Code_Namespace + "." + metadataReader.Code_Class;
                if (!String.IsNullOrEmpty(metadataReader.Code_Assembly))
                    Output.WriteLine("      <td>" + class_name_full.Replace("SobekCM.Resource_Object.Metadata_File_ReaderWriters.", "") + " ( " + metadataReader.Code_Assembly + " )</td>");
                else
                    Output.WriteLine("      <td>" + class_name_full.Replace("SobekCM.Resource_Object.Metadata_File_ReaderWriters.", "") + "</td>");

                // Add any options
                if ((metadataReader.Options != null) && (metadataReader.Options.Count > 0))
                {
                    if (metadataReader.Options.Count == 1)
                        Output.WriteLine("      <td>" + metadataReader.Options[0].Key + " = " + metadataReader.Options[0].Value + "</td>");
                    else
                    {
                        Output.WriteLine("      <td>");
                        foreach (StringKeyValuePair optionPair in metadataReader.Options)
                        {
                            Output.WriteLine("        " + optionPair.Key + " = " + optionPair.Value + "<br />");
                        }

                        Output.WriteLine("      </td>");
                    }
                }
                else
                {
                    Output.WriteLine("      <td></td>");
                }
                Output.WriteLine("    </tr>");
            }
            Output.WriteLine("  </table>");
        }

        private void add_metadata_mets_sections_info(TextWriter Output)
        {
            Output.WriteLine("  <h2>METS Sections Readers and Writers</h2>");
            Output.WriteLine("  <p>These metadata readers/writers read individual sections within the METS file that are found in either the bibliographic (dmdSec) or administrative (amdSec) portions of the METS file.</p>");
            Output.WriteLine("  <p>These reader/writers will automatically be utilized if a METS file is read with a matching MD Type found in either the dmdSec or amdSec.</p>");

            Output.WriteLine("  <p>The columns are defined below:</p>");
            Output.WriteLine("  <ul>");
            Output.WriteLine("    <li><span style=\"font-weight:bold\">Is Active?</span> - Flag indicates if this reader is active and will be utilized if a matching METS section is found</li>");
            Output.WriteLine("    <li><span style=\"font-weight:bold\">Label</span> - Label for the reader/writer which is also included in the label tag of the METS section when written.</li>");
            Output.WriteLine("    <li><span style=\"font-weight:bold\">MD Type</span> - If this matches either the MDTYPE, OTHERMDTYPE, or LABEL of a METS section, this reader will be utilized.  The first MD Type listed is the default that will generally be used when writing the METS section, unless otherwise indicated in the METS writing profile.</li>");
            Output.WriteLine("    <li><span style=\"font-weight:bold\">METS Section</span> - This indicates which METS section the reader should be employed for, either the bibliographic (dmdSec) or administrative (amdSec).  The amdSec also has some different subtypes which are also listed here.</li>");
            Output.WriteLine("    <li><span style=\"font-weight:bold\">Code Class</span> - The class for the C# reader/writer object to be created and utilized for any matching METS sections.</li>");
            Output.WriteLine("  </ul>");

            Output.WriteLine("  <table class=\"sbkSeav_BaseTable sbkSeav_MetsWritersTable\">");
            Output.WriteLine("    <tr>");
            Output.WriteLine("      <th class=\"sbkSeav_MetsWritersTable_ActiveCol\">Is Active?</th>");
      //      Output.WriteLine("      <th class=\"sbkSeav_MetsWritersTable_TypeCol\">ID</th>");
            Output.WriteLine("      <th class=\"sbkSeav_MetsWritersTable_BaseTypeCol\">Label</th>");
            Output.WriteLine("      <th class=\"sbkSeav_MetsProfileTable_LabelCol\">MD Type</th>");
            Output.WriteLine("      <th class=\"sbkSeav_MetsProfileTable_SectionCol\">METS Section</th>");
            Output.WriteLine("      <th class=\"sbkSeav_MetsWritersTable_NameableCol\">Code Class</th>");
     //       Output.WriteLine("      <th class=\"sbkSeav_MetsWritersTable_NameableCol\">Options</th>");
            Output.WriteLine("    </tr>");

            // Step through all the basic metadata reader/writers
            foreach (METS_Section_ReaderWriter_Config metadataReader in UI_ApplicationCache_Gateway.Configuration.Metadata.METS_Section_File_ReaderWriter_Configs)
            {
                add_metadata_single_config(Output, metadataReader, false);
            }
            Output.WriteLine("  </table>");
        }

        private void add_metadata_mets_profiles_info(TextWriter Output)
        {
            Output.WriteLine("  <h2>Metadata METS Writing Profiles</h2>");
            Output.WriteLine("  <p>These are profiles that can be used to write the METS files for the SobekCM system.  During the reading process, every enabled METS section reader/writer is utilized if a match is found in the existing METS file.  However, when writing the METS, the profiles indicate which METS sections should be utilized when a digital resource is saved to a METS file.</p>");
            Output.WriteLine("  <p>The individual metadata METS section writers are linked to the levels of the package hierarchy where they can write.  A writer can be tagged to the top, or package, level.  In addition, it can also be linked to the division level, in which case it will write information tied to the division level of the digital resource.  And, finally, the writers can also be linked to the individual file level, to write information which will be linked to an individual file within the METS.</p>");
            Output.WriteLine("  <p>While multiple METS writing profiles can be defined, the system will only utilized the default profile, which appears first in the list below.</p>");

            METS_Writing_Profile defaultProfile = UI_ApplicationCache_Gateway.Configuration.Metadata.Default_METS_Writing_Profile;
            if (defaultProfile != null)
            {
                Output.WriteLine("  <h3>" + defaultProfile.Profile_Name + " ( <span class=\"sbkSeav_DefaultTextSpan\">default profile</span> )</h3>");

                add_single_metadata_writing_profile(Output, defaultProfile);
            }

            // Now, loop through the rest of the profiles and display them (if they exist)
            foreach (METS_Writing_Profile thisProfile in UI_ApplicationCache_Gateway.Configuration.Metadata.MetsWritingProfiles)
            {
                if (thisProfile != defaultProfile)
                {
                    Output.WriteLine("  <h3>" + thisProfile.Profile_Name + "</h3>");

                    add_single_metadata_writing_profile(Output, thisProfile);
                }
            }
        }

        private void add_single_metadata_writing_profile(TextWriter Output, METS_Writing_Profile Profile )
	    {
            if (!String.IsNullOrEmpty(Profile.Profile_Description))
                Output.WriteLine("  <p>" + Profile.Profile_Description + "</p>");

            Output.WriteLine("  <table class=\"sbkSeav_BaseTable sbkSeav_MetsProfileTable\">");
            Output.WriteLine("    <tr>");
            Output.WriteLine("      <th class=\"sbkSeav_MetsProfileTable_ActiveCol\">Is Active?</th>");
            Output.WriteLine("      <th class=\"sbkSeav_MetsProfileTable_LabelCol\">Label</th>");
            Output.WriteLine("      <th class=\"sbkSeav_MetsProfileTable_LabelCol\">MD Type</th>");
            Output.WriteLine("      <th class=\"sbkSeav_MetsProfileTable_SectionCol\">METS Section</th>");
            Output.WriteLine("      <th class=\"sbkSeav_MetsProfileTable_Codeol\">Code Class</th>");
         //   Output.WriteLine("      <th class=\"sbkSeav_MetsProfileTable_OptionsCol\">Options</th>");
            Output.WriteLine("    </tr>");


            if ((Profile.Package_Level_AmdSec_Writer_Configs.Count > 0) || (Profile.Package_Level_DmdSec_Writer_Configs.Count > 0))
            {
                // Add a row for this
                Output.WriteLine("    <tr class=\"sbkSeav_MetsProfileTable_LevelRow\">");
                Output.WriteLine("      <td colspan=\"5\">Package Level Writers</td>");
                Output.WriteLine("    </tr>");

                foreach (METS_Section_ReaderWriter_Config config in Profile.Package_Level_DmdSec_Writer_Configs)
                {
                    add_metadata_single_config(Output, config, true);
                }

                foreach (METS_Section_ReaderWriter_Config config in Profile.Package_Level_AmdSec_Writer_Configs)
                {
                    add_metadata_single_config(Output, config, true);
                }
            }

            if ((Profile.Division_Level_DmdSec_Writer_Configs.Count > 0) || (Profile.Division_Level_AmdSec_Writer_Configs.Count > 0))
            {
                // Add a row for this
                Output.WriteLine("    <tr class=\"sbkSeav_MetsProfileTable_LevelRow\">");
                Output.WriteLine("      <td colspan=\"5\">Division Level Writers</td>");
                Output.WriteLine("    </tr>");

                foreach (METS_Section_ReaderWriter_Config config in Profile.Division_Level_DmdSec_Writer_Configs)
                {
                    add_metadata_single_config(Output, config, true);
                }

                foreach (METS_Section_ReaderWriter_Config config in Profile.Division_Level_AmdSec_Writer_Configs)
                {
                    add_metadata_single_config(Output, config, true);
                }
            }


            if ((Profile.File_Level_DmdSec_Writer_Configs.Count > 0) || (Profile.File_Level_AmdSec_Writer_Configs.Count > 0))
            {
                // Add a row for this
                Output.WriteLine("    <tr class=\"sbkSeav_MetsProfileTable_LevelRow\">");
                Output.WriteLine("      <td colspan=\"5\">File Level Writers</td>");
                Output.WriteLine("    </tr>");

                foreach (METS_Section_ReaderWriter_Config config in Profile.File_Level_DmdSec_Writer_Configs)
                {
                    add_metadata_single_config(Output, config, true);
                }

                foreach (METS_Section_ReaderWriter_Config config in Profile.File_Level_AmdSec_Writer_Configs)
                {
                    add_metadata_single_config(Output, config, true);
                }
            }
            Output.WriteLine("  </table>");
	    }

	    private void add_metadata_single_config(TextWriter Output, METS_Section_ReaderWriter_Config Config, bool onlyShowDefaultMd)
	    {
            Output.WriteLine("    <tr>");

            if (Config.isActive)
                Output.WriteLine("      <td class=\"sbkSeav_TableCenterCell\"><img src=\"" + Static_Resources_Gateway.Checkmark2_Png + "\" alt=\"yes\" /></td>");
            else
                Output.WriteLine("      <td class=\"sbkSeav_TableCenterCell\"><img src=\"" + Static_Resources_Gateway.Checkmark_Png + "\" alt=\"no\" /></td>");

       //     Output.WriteLine("      <td>" + config.ID + "</td>");
            Output.WriteLine("      <td>" + Config.Label + "</td>");

            // Add all the mappings (or only the default)
	        if (onlyShowDefaultMd)
	        {
	            if (Config.Default_Mapping != null)
	            {
                    if ( !String.IsNullOrEmpty(Config.Default_Mapping.Other_MD_Type))
                        Output.WriteLine("      <td>" + Config.Default_Mapping.Other_MD_Type + "</td>");
                    else
                        Output.WriteLine("      <td>" + Config.Default_Mapping.MD_Type + "</td>");
	            }
	            else
	            {
                    Output.WriteLine("      <td></td>");
	            }
	        }
	        else
	        {
	            Output.WriteLine("      <td>");
                // Start with the default
	            METS_Section_ReaderWriter_Mapping defaultMapping = Config.Default_Mapping;
                if (defaultMapping != null)
                {
                    if (!String.IsNullOrEmpty(defaultMapping.Other_MD_Type))
                        Output.WriteLine("        " + defaultMapping.Other_MD_Type + "<br />");
                    else
                        Output.WriteLine("        " + defaultMapping.MD_Type + "<br />");
                }
	            foreach (METS_Section_ReaderWriter_Mapping mapping in Config.Mappings)
	            {
	                if ((defaultMapping == null) || (defaultMapping != mapping))
	                {
	                    if (!String.IsNullOrEmpty(mapping.Other_MD_Type))
	                        Output.WriteLine("        " + mapping.Other_MD_Type + "<br />");
	                    else
	                        Output.WriteLine("        " + mapping.MD_Type + "<br />");
	                }
	            }
	            Output.WriteLine("      </td>");
	        }



            // Add the METS sections
            switch (Config.METS_Section)
            {
                case METS_Section_Type_Enum.DmdSec:
                    Output.WriteLine("      <td>dmdSec</td>");
                    break;
                case METS_Section_Type_Enum.AmdSec:
                    switch (Config.AmdSecType)
                    {
                        case METS_amdSec_Type_Enum.DigiProvMD:
                            Output.WriteLine("      <td>amdSec ( digiProvMD )</td>");
                            break;

                        case METS_amdSec_Type_Enum.RightsMD:
                            Output.WriteLine("      <td>amdSec ( rightsMD )</td>");
                            break;

                        case METS_amdSec_Type_Enum.SourceMD:
                            Output.WriteLine("      <td>amdSec ( sourceMD )</td>");
                            break;

                        case METS_amdSec_Type_Enum.TechMD:
                            Output.WriteLine("      <td>amdSec ( techMD )</td>");
                            break;

                        default:
                            Output.WriteLine("      <td>amdSec </td>");
                            break;
                    }
                    break;
            }


            if (!String.IsNullOrEmpty(Config.Code_Assembly))
                Output.WriteLine("      <td>" + Config.Code_Namespace + "." + Config.Code_Class + " ( " + Config.Code_Assembly + " )</td>");
            else
                Output.WriteLine("      <td>" + (Config.Code_Namespace + "." + Config.Code_Class).Replace("SobekCM.Resource_Object.METS_Sec_ReaderWriters.","") + "</td>");




            //if (Config.Options == null)
            //    Output.WriteLine("      <td></td>");
            //else
            //{
            //    Output.WriteLine("      <td>");
            //    foreach (StringKeyValuePair thisOption in Config.Options)
            //    {
            //        Output.WriteLine("        " + thisOption.Key + " = " + thisOption.Value + "<br />");
            //    }
            //    Output.WriteLine("      </td>");
            //}

            Output.WriteLine("    </tr>");
	    }

        private void add_metadata_modules_info(TextWriter Output)
        {
            Output.WriteLine("<h2>Metadata Modules</h2>");
            Output.WriteLine("<p>These are extra metadata modules that are selected to always be added to digital resource when built.  Usually modules are added by reader/writers, but these could be forced to be added by default whenever a new package is added.</p>");
        }

        private void add_metadata_toplevel_info(TextWriter Output)
        {
            Output.WriteLine("  <span class=\"sbkSeav_BackUpLink\"><a href=\"" + redirectUrl.Replace("%SETTINGSCODE%", "") + "\">Back to top</a></span>");

            Output.WriteLine("  <h2>Metadata Configuration</h2>");

            Output.WriteLine("  <p>Metadata reading and writing within the system can be completely customized by utilizing the appropriate portions of the metadata configuration.  This can control how overall metadata files are written as well as how the sections within a METS file are read and written.</p>");

            Output.WriteLine("  <p>The subpages under this page provide information on this configuration and all of the different metadata readers and writers the system can employ.</p>");

            Output.WriteLine("  <div id=\"sbkSeav_SubPageDesc\">");
            Output.WriteLine("    <p id=\"sbkSeav_SubPageTitle\">This page contains the following subpages:</p>");
            Output.WriteLine("    <h4><a href=\"" + redirectUrl.Replace("%SETTINGSCODE%", "metadata/fields") + "\">Metadata Search Fields</a></h4>");
            Output.WriteLine("    <p>The metadata search fields subpage lists all of the current search fields in the system that are utilized by the main searching methods.  These terms (and their link into the solr/lucene indexes) are defined in the database.</p>");

            Output.WriteLine("    <h4><a href=\"" + redirectUrl.Replace("%SETTINGSCODE%", "metadata/filereaders") + "\">File Readers/Writers</a></h4>");
            Output.WriteLine("    <p>The metadata file readers/writers subpage is the complete list of different metadata readers and writers available within the system.  The referenced objects can read a complete metadata file and/or write a complete metadata file.   These are not the readers/writers used for individual sections within a METS file.</p>");

            Output.WriteLine("    <h4><a href=\"" + redirectUrl.Replace("%SETTINGSCODE%", "metadata/metsreaders") + "\">METS Section Reader/Writers</a></h4>");
            Output.WriteLine("    <p>These metadata readers/writers read individual sections within the METS file that are found in either the bibliographic (dmdSec) or administrative (amdSec) portions of the METS file.</p>");

            Output.WriteLine("    <h4><a href=\"" + redirectUrl.Replace("%SETTINGSCODE%", "metadata/metsprofiles") + "\">METS Writing Profiles</a></h4>");
            Output.WriteLine("    <p>This subpage includes all the METS writing profiles that can be used to write the METS files for the SobekCM system.  During the reading process, every enabled METS section reader/writer is utilized if a match is found in the existing METS file.  However, when writing the METS, the profiles indicate which METS sections should be written at each level of the hierarchy when a digital resource is saved to a METS file.</p>");
            Output.WriteLine("  </div>");

        }



        #endregion

        #region HTML helper methods for the engine main page and subpages

        private void add_engine_info(TextWriter Output)
		{
            // If a submode existed, call that method
            switch (engineSubEnum)
            {
                case Settings_Engine_SubMode_Enum.Authentication:
                    add_engine_authentication_info(Output);
                    break;

                case Settings_Engine_SubMode_Enum.Brief_Item_Mapping:
                    add_engine_brief_item_mapping_info(Output);
                    break;

                case Settings_Engine_SubMode_Enum.Contact_Form:
                    add_engine_contact_info(Output);
                    break;

                case Settings_Engine_SubMode_Enum.Engine_Server_Endpoints:
                    add_engine_server_endpoints_info(Output);
                    break;

                case Settings_Engine_SubMode_Enum.OAI_PMH:
                    add_engine_oai_pmh_info(Output);
                    break;

                case Settings_Engine_SubMode_Enum.QcTool:
                    add_engine_qc_tool_info(Output);
                    break;

                default:
                    add_engine_toplevel_info(Output);
                    break;
            }
		}


	    private void add_engine_authentication_info(TextWriter Output)
	    {
            Output.WriteLine("  <h2>Authentication Configuration</h2>");

	        Output.WriteLine("  <p>Information about different authentication systems allowed for signing up and logging into this system are detailed here.  This includes the highest level information, such as whether Shibboleth and the standard authentication system is enabled.</p>");
            Output.WriteLine("  <p>If Shibboleth, or any other specialized authentication systems are enabled, additional details will display in this section as well.  In the case of Shibboleth, this includes the basic information (such as URL, name, etc..) for the Shibboleth authentication, as well as how certain data fields returned with the Shiboleth token should be handled.</p>");

            Output.WriteLine("  <h3>Authentication Modes</h3>");
            Output.WriteLine("  <table class=\"sbkSeav_BaseTableVert\" id=\"sbkSeav_AuthModesTable\">");
            Output.WriteLine("    <tr>");
            Output.WriteLine("      <th>SobekCM Standard Authentication:</th>");
            Output.WriteLine("      <td>ENABLED</td>");
            Output.WriteLine("    </tr>");
            Output.WriteLine("    <tr>");
            Output.WriteLine("      <th>Shibboleth Authentication:</th>");

	        bool shib_enabled = ((UI_ApplicationCache_Gateway.Configuration.Authentication.Shibboleth != null) && (UI_ApplicationCache_Gateway.Configuration.Authentication.Shibboleth.Enabled));
            if ( shib_enabled )
                Output.WriteLine("      <td>ENABLED</td>");
            else
                Output.WriteLine("      <td>NOT ENABLED</td>");
            Output.WriteLine("    </tr>");
            Output.WriteLine("  </table>");

	        if (shib_enabled)
	        {
	            Shibboleth_Configuration shibConfig = UI_ApplicationCache_Gateway.Configuration.Authentication.Shibboleth;
                Output.WriteLine("  <h3>Shibboleth Details</h3>");
	            Output.WriteLine("  <p>Below are some of the top-level values related to the Shibboleth configuration, including the URL and display label.  The <i>User Identity Attribute</i> is the Shibboleth attribute passed back after the user is authenticated which uniquely identifies the user for the purposes of this system.</p>");
                Output.WriteLine("  <table class=\"sbkSeav_BaseTableVert\" id=\"sbkSeav_ShibDetailsTable\">");
                Output.WriteLine("    <tr>");
                Output.WriteLine("      <th>Display Label</th>");
                Output.WriteLine("      <td>" + shibConfig.Label + "</td>");
                Output.WriteLine("    </tr>");
                Output.WriteLine("    <tr>");
                Output.WriteLine("      <th>URL</th>");
                Output.WriteLine("      <td>" + shibConfig.ShibbolethURL + "</td>");
                Output.WriteLine("    </tr>");
                Output.WriteLine("    <tr>");
                Output.WriteLine("      <th>User Identity Attribute</th>");
                Output.WriteLine("      <td>" + shibConfig.UserIdentityAttribute + "</td>");
                Output.WriteLine("    </tr>");
                Output.WriteLine("    <tr>");
                Output.WriteLine("      <th>Debug Mode</th>");
                Output.WriteLine("      <td>" + shibConfig.Debug.ToString().ToUpper() + "</td>");
                Output.WriteLine("    </tr>");
                Output.WriteLine("  </table>");

                // Add any attribute mappings
	            if ((shibConfig.AttributeMapping != null) && (shibConfig.AttributeMapping.Count > 0))
	            {
                    Output.WriteLine("  <p>When a user first signs on via Shibboleth, the following attributes returned from Shibboleth are mapped to certain SobekCM user attributes.</p>");
                    Output.WriteLine("  <table class=\"sbkSeav_BaseTable\" id=\"sbkSeav_ShibDetails2Table\">");
                    Output.WriteLine("    <tr>");
                    Output.WriteLine("      <th>Shibboleth Attribute</th>");
                    Output.WriteLine("      <th>SobekCM User Attribute</th>");
                    Output.WriteLine("    </tr>");

                    foreach( Shibboleth_Configuration_Mapping thisMapping in shibConfig.AttributeMapping )
	                {
                        Output.WriteLine("    <tr>");
                        Output.WriteLine("      <td>" + thisMapping.Value + "</td>");
                        Output.WriteLine("      <td>" + User_Object_Attribute_Mapping_Enum_Converter.ToString( thisMapping.Mapping ) + "</td>");
                        Output.WriteLine("    </tr>");
                    }

                    Output.WriteLine("  </table>");
                }

                // Add any constants 
                if ((shibConfig.Constants != null) && (shibConfig.Constants.Count > 0))
                {
                    Output.WriteLine("  <p>Constants can also be applied to the SobekCM user record when a user first signs on via Shibboleth.  This allows certain permissions, templates, and default metadata to be applied to these users automatically. </p>");
                    Output.WriteLine("  <table class=\"sbkSeav_BaseTable\" id=\"sbkSeav_ShibDetails3Table\">");
                    Output.WriteLine("    <tr>");
                    Output.WriteLine("      <th>SobekCM User Attribute</th>");
                    Output.WriteLine("      <th>Default Value</th>");
                    Output.WriteLine("    </tr>");

                    foreach (Shibboleth_Configuration_Mapping thisMapping in shibConfig.Constants)
                    {
                        Output.WriteLine("    <tr>");
                        Output.WriteLine("      <td>" + User_Object_Attribute_Mapping_Enum_Converter.ToString(thisMapping.Mapping) + "</td>");
                        Output.WriteLine("      <td>" + thisMapping.Value + "</td>");
                        Output.WriteLine("    </tr>");
                    }

                    Output.WriteLine("  </table>");
                }

                // Add any indicators that a user can submit
                if ((shibConfig.CanSubmitIndicators != null) && (shibConfig.CanSubmitIndicators.Count > 0))
                {
                    Output.WriteLine("  <p>Indicators can be used to determine if users should automatically get the permission to submit, based on the Shibboleth authentication attributes.  So, for example, if a user type of 'F' indicates this user is faculty and should automatically be allowed to submit,  this could be set in this portion of the configuration.</p>");
                    Output.WriteLine("  <table class=\"sbkSeav_BaseTable\" id=\"sbkSeav_ShibDetails4Table\">");
                    Output.WriteLine("    <tr>");
                    Output.WriteLine("      <th>Shibboleth Attribute</th>");
                    Output.WriteLine("      <th>Indicative Value</th>");
                    Output.WriteLine("    </tr>");

                    foreach (StringKeyValuePair thisMapping in shibConfig.CanSubmitIndicators)
                    {
                        Output.WriteLine("    <tr>");
                        Output.WriteLine("      <td>" + thisMapping.Key + "</td>");
                        Output.WriteLine("      <td>" + thisMapping.Value + "</td>");
                        Output.WriteLine("    </tr>");
                    }

                    Output.WriteLine("  </table>");
                }
            }
	    }

        private void add_engine_brief_item_mapping_info(TextWriter Output)
	    {
            Output.WriteLine("  <h2>Brief Item Mapping Sets Configuration</h2>");

            Output.WriteLine("  <p>When an item is requested, the METS file is read into a rich complete SobekCM item object.  In addition, data from the database is mapped into this object.  For display purposes, the data from this SobekCM item is mapped into a Brief Item which is what is generally transferred via the REST API from the engine to the user interface.  The Brief Item is what is used for displaying an item in the public interface.</p>");
            Output.WriteLine("  <p>The mapping from the rich SobekCM Item to the Brief Item is controlled by a number of brief item mappers.  If new metadata fields are being added for display, you will almost certainly need to create a new BriefItemMapper object, to ensure the metadata is available for display online.  In addition, by overriding an existing mapper, you can change how metadata is displayed in the citation, or how the item behaves within your instance of SobekCM.</p>");
            Output.WriteLine("  <p>There can be multiple mapping sets in your instance.  It is most common to have the following mapping sets:</p>");
            Output.WriteLine("  <ol>");
            Output.WriteLine("    <li>'Citation' mapping set, which only maps over the descriptive citation information</li>");
            Output.WriteLine("    <li>'Standard' mapping set, which brings over all the file and division information</li>");
            Output.WriteLine("    <li>'Internal' mapping set, which is generally restricted and includes some private information and primary keys</li>");
            Output.WriteLine("  </ol>");

            string defaultSetName = UI_ApplicationCache_Gateway.Configuration.BriefItemMapping.DefaultSetName;
            BriefItemMapping_Set defaultSet = UI_ApplicationCache_Gateway.Configuration.BriefItemMapping.GetMappingSet(defaultSetName);
            if (defaultSet != null)
            {
                Output.WriteLine("  <h3>" + defaultSetName + " ( <span class=\"sbkSeav_DefaultTextSpan\">default set</span> )</h3>");
                Output.WriteLine("  <table class=\"sbkSeav_BaseTable sbkSeav_BriefItemMappingTable\">");
                Output.WriteLine("    <tr>");
                Output.WriteLine("      <th class=\"sbkSeav_BriefItemMappingTable_EnabledCol\">Enabled</th>");
                Output.WriteLine("      <th class=\"sbkSeav_BriefItemMappingTable_ClassCol\">Class</th>");
                Output.WriteLine("    </tr>");
                foreach (BriefItemMapping_Mapper thisModule in defaultSet.Mappings)
                {
                    Output.WriteLine("    <tr>");
                    if (thisModule.Enabled)
                        Output.WriteLine("      <td class=\"sbkSeav_TableCenterCell\"><img src=\"" + Static_Resources_Gateway.Checkmark2_Png + "\" alt=\"yes\" /></td>");
                    else
                        Output.WriteLine("      <td class=\"sbkSeav_TableCenterCell\"><img src=\"" + Static_Resources_Gateway.Checkmark_Png + "\" alt=\"no\" /></td>");


                    if (!String.IsNullOrEmpty(thisModule.Assembly))
                        Output.WriteLine("      <td>" + thisModule.Class + " ( " + thisModule.Assembly + " )</td>");
                    else
                        Output.WriteLine("      <td>" + thisModule.Class.Replace("SobekCM.Engine_Library.Items.BriefItems.Mappers.", "") + "</td>");
                    
                    Output.WriteLine("    </tr>");
                }
                Output.WriteLine("  </table>");
            }

            // Now, loop through the rest of the sets and display them (if they exist)
            foreach (BriefItemMapping_Set thisSet in UI_ApplicationCache_Gateway.Configuration.BriefItemMapping.MappingSets)
            {
                if (String.Compare(thisSet.SetName, defaultSetName, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    Output.WriteLine("  <h3>" + thisSet.SetName + "</h3>");
                    Output.WriteLine("  <table class=\"sbkSeav_BaseTable sbkSeav_BriefItemMappingTable\">");
                    Output.WriteLine("    <tr>");
                    Output.WriteLine("      <th class=\"sbkSeav_BriefItemMappingTable_EnabledCol\">Enabled</th>");
                    Output.WriteLine("      <th class=\"sbkSeav_BriefItemMappingTable_ClassCol\">Class</th>");
                    Output.WriteLine("    </tr>");
                    foreach (BriefItemMapping_Mapper thisModule in thisSet.Mappings)
                    {
                        Output.WriteLine("    <tr>");
                        if (thisModule.Enabled)
                            Output.WriteLine("      <td class=\"sbkSeav_TableCenterCell\"><img src=\"" + Static_Resources_Gateway.Checkmark2_Png + "\" alt=\"yes\" /></td>");
                        else
                            Output.WriteLine("      <td class=\"sbkSeav_TableCenterCell\"><img src=\"" + Static_Resources_Gateway.Checkmark_Png + "\" alt=\"no\" /></td>");


                        if (!String.IsNullOrEmpty(thisModule.Assembly))
                            Output.WriteLine("      <td>" + thisModule.Class + " ( " + thisModule.Assembly + " )</td>");
                        else
                            Output.WriteLine("      <td>" + thisModule.Class.Replace("SobekCM.Engine_Library.Items.BriefItems.Mappers.", "") + "</td>");

                        Output.WriteLine("    </tr>");
                    }
                    Output.WriteLine("  </table>");
                }
            }
	    }

	    private void add_engine_contact_info(TextWriter Output)
	    {
	        Output.WriteLine("  <h2>Contact Us Form Configuration</h2>");

            Output.WriteLine("  <p>This controls the behavior of the main, top-level, contact form within SobekCM.  If you wish to use this form, you can control which fields display, the order of display, and the options for a user to select.</p>");
            Output.WriteLine("  <p>This can also be customized at the collection level, but those custom contact us forms would not display here, since they are embedded within the aggregation folders.</p>");


            Output.WriteLine("  <table class=\"sbkSeav_BaseTable\" id=\"sbkSeav_ContactFormTable\">");
	        Output.WriteLine("    <tr>");
	        Output.WriteLine("      <th id=\"sbkSeav_ContactForm_NameCol\">Name</th>");
	        Output.WriteLine("      <th id=\"sbkSeav_ContactForm_PromptCol\">Prompt</th>");
            Output.WriteLine("      <th id=\"sbkSeav_ContactForm_TypeCol\">Type</th>");
            Output.WriteLine("      <th id=\"sbkSeav_ContactForm_ChoicesCol\">Choices</th>");
            Output.WriteLine("      <th id=\"sbkSeav_ContactForm_NotesCol\">Notes</th>");
            Output.WriteLine("    </tr>");
	        foreach (ContactForm_Configuration_Element thisElement in UI_ApplicationCache_Gateway.Configuration.ContactForm.FormElements)
	        {
	            Output.WriteLine("    <tr>");
                if ( !String.IsNullOrEmpty(thisElement.Name ))
                    Output.WriteLine("      <td>" + thisElement.Name + "</td>");
                else
                    Output.WriteLine("      <td>" + ContactForm_Configuration_Element_Type_Enum_Converter.ToString(thisElement.Element_Type) + "</td>");

                Output.WriteLine("      <td>" + thisElement.QueryText.DefaultValue + "</td>");
                Output.WriteLine("      <td>" + ContactForm_Configuration_Element_Type_Enum_Converter.ToString(thisElement.Element_Type) + "</td>");

                // End the choices the user can pick, where applicable
                if (( thisElement.Options == null ) || (thisElement.Options.Count == 0 ))
                    Output.WriteLine("      <td></td>");
                else
                {
                    Output.WriteLine("      <td>");
                    foreach (string thisOption in thisElement.Options)
                    {
                        Output.WriteLine("        " + thisOption + "<br />");
                    }
                    Output.WriteLine("      </td>");
                }

                // Add additional notes about this element
                if (( !thisElement.Required ) && ( thisElement.UserAttribute == User_Object_Attribute_Mapping_Enum.NONE))
                    Output.WriteLine("      <td></td>");
                else
                {
                    Output.WriteLine("      <td>");
                    if ( thisElement.Required )
                        Output.WriteLine("        Required field<br />");
                    if (thisElement.UserAttribute != User_Object_Attribute_Mapping_Enum.NONE)
                    {
                        Output.WriteLine("        Mapped from user attribute '" + User_Object_Attribute_Mapping_Enum_Converter.ToString(thisElement.UserAttribute) + "'<br />");

                        if (!thisElement.AlwaysShow)
                        {
                            Output.WriteLine("        Field hidden for logged on users<br />");
                        }
                    }
                    Output.WriteLine("      </td>");
                }

                Output.WriteLine("    </tr>");
	        }
	        Output.WriteLine("  </table>");
	    }

        private void add_engine_server_endpoints_info(TextWriter Output)
        {
            Output.WriteLine("  <h2>Engine Configuration</h2>");
            Output.WriteLine("  <p>This section details all of the REST API endpoints exposed via the SobekCM engine.  This details each endpoint that is made available, including which HTTP verb(s) are supported (GET, POST, PUT, and/or DELETE), which transfer schemas are supported (JSON, JSON-P, XML, ProtoBuf, or something else), and which method is called to support this endpoint.  In addition, any IP restrictions and component information is included.</p>");
            Output.WriteLine("  <p>The first table lists all of the endpoints and links to the two tables at the bottom of the page.  At the bottom of the page is the list of componentns, which are linked to the endpoints by Component ID.  Also at the bottom is each IP restriction range, which again is linked to the endpoint by ID.  This lists all of the IP ranges that are allowed access to those particular endpoints.</p>");


            Output.WriteLine("  <h3>Engine Endpoints</h3>");
            Output.WriteLine("  <p>These are all the endpoints that are suppored by the SobekCM engine in your instance.</p>");
            Output.WriteLine("  <table class=\"sbkSeav_BaseTable\" id=\"sbkSeav_EngineServerEndpointsTable\">");
            Output.WriteLine("    <tr>");
            Output.WriteLine("      <th id=\"sbkSeav_EngineServerEndpointsTable_UrlCol\">URL</th>");
            Output.WriteLine("      <th id=\"sbkSeav_EngineServerEndpointsTable_VerbCol\">Verb</th>");
            Output.WriteLine("      <th id=\"sbkSeav_EngineServerEndpointsTable_ProtocolCol\">Protocol</th>");
            Output.WriteLine("      <th id=\"sbkSeav_EngineServerEndpointsTable_MethodCol\">Method</th>");
            Output.WriteLine("      <th id=\"sbkSeav_EngineServerEndpointsTable_ComponentCol\">Component</th>");
            Output.WriteLine("      <th id=\"sbkSeav_EngineServerEndpointsTable_RestrictionsCol\">Restrictions</th>");
            Output.WriteLine("    </tr>");

            // Step through all the roots
            foreach (Engine_Path_Endpoint rootEndpoint in UI_ApplicationCache_Gateway.Configuration.Engine.RootPaths)
            {
                recursively_write_all_endpoints(rootEndpoint, Output, rootEndpoint.Segment);
            }

            Output.WriteLine("  </table>");

            // Add the engine components lookup table
            Output.WriteLine("  <h3>Engine Components</h3>");
            Output.WriteLine("  <p>These are all the components which are loaded, either from the default assembly or a referenced assembly, to support all the endpoints above.</p>");
            Output.WriteLine("  <table class=\"sbkSeav_BaseTable\" id=\"sbkSeav_EngineServerComponentsTable\">");
            Output.WriteLine("    <tr>");
            Output.WriteLine("      <th id=\"sbkSeav_EngineServerComponentsTable_IdCol\">Component ID</th>");
            Output.WriteLine("      <th id=\"sbkSeav_EngineServerComponentsTable_ClassCol\">Class</th>");
            Output.WriteLine("    </tr>");
            foreach (Engine_Component thisComponent in UI_ApplicationCache_Gateway.Configuration.Engine.Components)
            {
                Output.WriteLine("    <tr id=\"CO" + thisComponent.ID + "\">");
                Output.WriteLine("      <td>" + thisComponent.ID + "</td>");

                if (!String.IsNullOrEmpty(thisComponent.Assembly))
                    Output.WriteLine("      <td>" + thisComponent.Class + " ( " + thisComponent.Assembly + " )</td>");
                else
                    Output.WriteLine("      <td>" + thisComponent.Class.Replace("SobekCM.Engine_Library.Endpoints.", "") + "</td>");

                Output.WriteLine("    </tr>");
            }
            Output.WriteLine("  </table>");

            // Add the engine restrictions table
            Output.WriteLine("  <h3>Engine IP Restrictions</h3>");
            Output.WriteLine("  <p>This table lists all of the IP restriction ranges which exist in this instance to limit access to some of the endpoints.</p>");
            Output.WriteLine("  <table class=\"sbkSeav_BaseTable\" id=\"sbkSeav_EngineServerRestrictionsTable\">");
            Output.WriteLine("    <tr>");
            Output.WriteLine("      <th id=\"sbkSeav_ContactForm_IdCol\">Range ID</th>");
            Output.WriteLine("      <th id=\"sbkSeav_ContactForm_DescCol\">Range Description</th>");
            Output.WriteLine("      <th id=\"sbkSeav_ContactForm_IpCol\">IP Range</th>");
            Output.WriteLine("      <th id=\"sbkSeav_ContactForm_IpLabelCol\">IP Range Label</th>");
            Output.WriteLine("    </tr>");
            foreach (Engine_RestrictionRange thisRestrictionRange in UI_ApplicationCache_Gateway.Configuration.Engine.RestrictionRanges)
            {
                // Get the number of ips
                int ip_count = 0;
                if (thisRestrictionRange.IpRanges != null)
                    ip_count = thisRestrictionRange.IpRanges.Count;

                Output.WriteLine("    <tr id=\"RR" + thisRestrictionRange.ID + "\">");
                Output.WriteLine("      <td rowspan=\"" + Math.Max(1, ip_count) + "\">" + thisRestrictionRange.ID + "</td>");
                Output.WriteLine("      <td rowspan=\"" + Math.Max(1, ip_count) + "\">" + thisRestrictionRange.Label + "</td>");

                if ((ip_count == 0) || ( thisRestrictionRange.IpRanges == null ))
                {
                    Output.WriteLine("      <td></td>");
                    Output.WriteLine("      <td></td>");
                    Output.WriteLine("    </tr>");
                }
                else
                {
                    bool first = true;
                    foreach (Engine_IpRange thisRange in thisRestrictionRange.IpRanges)
                    {
                        if (!first)
                            Output.WriteLine("    <tr>");
                        if (!String.IsNullOrEmpty(thisRange.EndIp))
                            Output.WriteLine("      <td>" + thisRange.StartIp + " - " + thisRange.EndIp + "</td>");
                        else
                            Output.WriteLine("      <td>" + thisRange.StartIp + "</td>");

                        Output.WriteLine("      <td>" + thisRange.Label + "</td>");
                        Output.WriteLine("    </tr>");

                        first = false;
                    }
                }
            }

            Output.WriteLine("  </table>");
        }

	    private void recursively_write_all_endpoints( Engine_Path_Endpoint RootEndpoint , TextWriter Output , string Url )
	    {
	        if (RootEndpoint.IsEndpoint)
	        {
                // How many verbs are defined for this endpoint?
	            List<Engine_VerbMapping> mappings = RootEndpoint.AllVerbMappings;

	            if (mappings.Count == 1)
	            {
	                Output.WriteLine("    <tr>");
	                Output.WriteLine("      <td>" + Url + "</td>");
                    add_single_verb_mapping_in_table(mappings[0], Output);
	                Output.WriteLine("    </tr>");
	            }
                else if (mappings.Count > 1)
                {
                    Output.WriteLine("    <tr>");
                    Output.WriteLine("      <td rowspan=\"" + mappings.Count + "\">" + Url + "</td>");
                    add_single_verb_mapping_in_table(mappings[0], Output);
                    Output.WriteLine("    </tr>");

                    for (int i = 1; i < mappings.Count; i++)
                    {
                        Output.WriteLine("    <tr>");
                        add_single_verb_mapping_in_table(mappings[i], Output);
                        Output.WriteLine("    </tr>");
                    }
                }
	        }
	        else
	        {
	            foreach (Engine_Path_Endpoint childEndpoint in RootEndpoint.Children)
	            {
	                recursively_write_all_endpoints(childEndpoint, Output, Url + "/" + childEndpoint.Segment);
	            }
	        }
	    }

	    private void add_single_verb_mapping_in_table(Engine_VerbMapping Mapping, TextWriter Output)
	    {
            // Add the request type ( i.e., GET, POST, PUT, etc.. )
            switch (Mapping.RequestType)
            {
                case Microservice_Endpoint_RequestType_Enum.GET:
                    Output.WriteLine("      <td>GET</td>");
                    break;

                case Microservice_Endpoint_RequestType_Enum.POST:
                    Output.WriteLine("      <td>POST</td>");
                    break;

                case Microservice_Endpoint_RequestType_Enum.PUT:
                    Output.WriteLine("      <td>PUT</td>");
                    break;

                case Microservice_Endpoint_RequestType_Enum.DELETE:
                    Output.WriteLine("      <td>DELETE</td>");
                    break;

                default:
                    Output.WriteLine("      <td>ERROR</td>");
                    break;
            }


            // Add the protocol type ( i.e., XML, JSON, PROTOBUF, etc.. )
            switch (Mapping.Protocol)
            {
                case Microservice_Endpoint_Protocol_Enum.BINARY:
                    Output.WriteLine("      <td>BINARY</td>");
                    break;

                case Microservice_Endpoint_Protocol_Enum.CACHE:
                    Output.WriteLine("      <td>Cache</td>");
                    break;

                case Microservice_Endpoint_Protocol_Enum.JSON:
                    Output.WriteLine("      <td>JSON</td>");
                    break;

                case Microservice_Endpoint_Protocol_Enum.JSON_P:
                    Output.WriteLine("      <td>JSON-P</td>");
                    break;

                case Microservice_Endpoint_Protocol_Enum.PROTOBUF:
                    Output.WriteLine("      <td>ProtoBuf</td>");
                    break;

                case Microservice_Endpoint_Protocol_Enum.SOAP:
                    Output.WriteLine("      <td>SOAP</td>");
                    break;

                case Microservice_Endpoint_Protocol_Enum.TEXT:
                    Output.WriteLine("      <td>Text</td>");
                    break;

                case Microservice_Endpoint_Protocol_Enum.XML:
                    Output.WriteLine("      <td>XML</td>");
                    break;

                default:
                    Output.WriteLine("      <td>ERROR</td>");
                    break;
            }


            Output.WriteLine("      <td>" + Mapping.Method + "</td>");
            Output.WriteLine("      <td><a href=\"#CO" + Mapping.ComponentId + "\">" + Mapping.ComponentId + "</a></td>");
            Output.WriteLine("      <td><a href=\"#RR" + Mapping.RestrictionRangeSetId + "\">" + Mapping.RestrictionRangeSetId + "</a></td>");
        }

        private void add_engine_oai_pmh_info(TextWriter Output)
        {
            OAI_PMH_Configuration config = UI_ApplicationCache_Gateway.Configuration.OAI_PMH;

            // Determine some global settings
            string oai_resource_identifier_base = config.Identifier_Base;
            string oai_repository_name = config.Name;
            string oai_repository_identifier = config.Identifier;

            if (String.IsNullOrEmpty(oai_resource_identifier_base)) oai_resource_identifier_base = "oai:" + UI_ApplicationCache_Gateway.Settings.System.System_Abbreviation + ":";
            if (String.IsNullOrEmpty(oai_repository_name)) oai_repository_name = UI_ApplicationCache_Gateway.Settings.System.System_Name;
            if (String.IsNullOrEmpty(oai_repository_identifier)) oai_repository_identifier = UI_ApplicationCache_Gateway.Settings.System.System_Abbreviation;

            Output.WriteLine("  <h2>OAI-PMH Configuration</h2>");
            Output.WriteLine("  <p>The top-level information for this repository and OAI-PMH ( the <a href=\"https://www.openarchives.org/pmh/\">Open Archives Initiative Protocol for Metadata Harvesting</a> ) is displayed below.  This includes whether it is enabled to be available for this repository and the basic identifying information about this repository as well as the different metadata formats that can be shared via OAI-PMH.</p>");
            Output.WriteLine("  <p>This just configures how OAI-PMH will work with this overall instance and how the repository responds to the basic Identify verb.  Each individual aggregation can be selected to include as an OAI-PMH set from within the single aggregation administrative screens.</p>");

            Output.WriteLine("  <h3>Basic Repository Information</h3>");
            Output.WriteLine("  <p>Below is the top-level information for this repository, including if OAI-PMH is enabled in your instance.</p>");
            Output.WriteLine("  <table class=\"sbkSeav_BaseTableVert\" id=\"sbkSeav_OaiPmhBasicTable\">");
            Output.WriteLine("    <tr>");
            Output.WriteLine("      <th id=\"sbkSeav_OaiPmhBasicTable_FirstCol\">OAI-PMH Enabled?</th>");
            if (config.Enabled)
                Output.WriteLine("      <td class=\"sbkSeav_OaiPmhBasicTable_SecondCol\">ENABLED</td>");
            else
                Output.WriteLine("      <td class=\"sbkSeav_OaiPmhBasicTable_SecondCol\">NOT ENABLED</td>");
            Output.WriteLine("    </tr>");

            Output.WriteLine("    <tr><th>Repository Name:</th><td>" + oai_repository_name + "</td></tr>");
            Output.WriteLine("    <tr><th>Repository Identifier:</th><td>" + oai_repository_identifier + "</td></tr>");

            if ((config.Descriptions != null) && (config.Descriptions.Count > 0))
            {
                Output.WriteLine("    <tr>");
                Output.WriteLine("      <th>Additional Descriptions</th>");
                if (config.Descriptions.Count == 1)
                    Output.WriteLine("      <td>" + config.Descriptions[0].Replace("<","&lt;").Replace(">","&gt;") + "</td>");
                else
                {
                    Output.WriteLine("      <td>");
                    foreach( string thisDesc in config.Descriptions )
                        Output.WriteLine("        " + thisDesc.Replace("<", "&lt;").Replace(">", "&gt;") + "<br />");
                    Output.WriteLine("      </td>");
                }
                Output.WriteLine("    </tr>");
            }

            if ((config.Admin_Emails != null) && (config.Admin_Emails.Count > 0))
            {
                Output.WriteLine("    <tr>");
                Output.WriteLine("      <th>Admin Email Addresses:</th>");
                if (config.Admin_Emails.Count == 1)
                    Output.WriteLine("      <td>" + config.Admin_Emails[0] + "</td>");
                else
                {
                    Output.WriteLine("      <td>");
                    foreach (string thisEmail in config.Admin_Emails)
                        Output.WriteLine("        " + thisEmail + "<br />");
                    Output.WriteLine("      </td>");
                }
                Output.WriteLine("    </tr>");
            }
            Output.WriteLine("    <tr><th>Resource Identifier:</th><td>" + oai_resource_identifier_base + "</td></tr>");
            Output.WriteLine("  </table>");

                Output.WriteLine("  <h3>Metadata Prefixes</h3>");
            Output.WriteLine("  <p>This details the different metadata formats available in this repository for OAI-PMH sharing.  New formats can be supported by adding new metadata namespaces and classes to render the metadata.  In addition, the existing output could be modified by overriding the default metadata writer to use a class you include within a DLL on your site.</p>");


                // Add any constants 
            if ((config.Metadata_Prefixes != null) && (config.Metadata_Prefixes.Count > 0))
            {
                Output.WriteLine("  <table class=\"sbkSeav_BaseTable\" id=\"sbkSeav_OaiPmhPrefixesTable\">");
                Output.WriteLine("    <tr>");
                Output.WriteLine("      <th id=\"sbkSeav_OaiPmhPrefixesTable_EnabledCol\">Enabled</th>");
                Output.WriteLine("      <th id=\"sbkSeav_OaiPmhPrefixesTable_PrefixCol\">Prefix</th>");
                Output.WriteLine("      <th id=\"sbkSeav_OaiPmhPrefixesTable_SchemaCol\">Metadata Namespace / Schema</th>");
                Output.WriteLine("      <th id=\"sbkSeav_OaiPmhPrefixesTable_ClassCol\">Class</th>");
                Output.WriteLine("    </tr>");

                foreach (OAI_PMH_Metadata_Format thisMapping in config.Metadata_Prefixes)
                {
                    Output.WriteLine("    <tr>");

                    // Enabled flag
                    if (thisMapping.Enabled)
                        Output.WriteLine("      <td class=\"sbkSeav_TableCenterCell\"><img src=\"" + Static_Resources_Gateway.Checkmark2_Png + "\" alt=\"yes\" /></td>");
                    else
                        Output.WriteLine("      <td class=\"sbkSeav_TableCenterCell\"><img src=\"" + Static_Resources_Gateway.Checkmark_Png + "\" alt=\"no\" /></td>");

                    // Prefix
                    Output.WriteLine("      <td>" + thisMapping.Prefix + "</td>");

                    // Namespace / Schema
                    Output.WriteLine("      <td>" + thisMapping.MetadataNamespace + "<br />" + thisMapping.Schema + "</td>");

                    // Add the class, and optionally assembly
                    string class_name_full = thisMapping.Class;
                    if (!String.IsNullOrEmpty(thisMapping.Namespace))
                        class_name_full = thisMapping.Namespace + "." + thisMapping.Class;
                    if (!String.IsNullOrEmpty(thisMapping.Assembly))
                        Output.WriteLine("      <td>" + class_name_full.Replace("SobekCM.Resource_Object.OAI.Writer.", "") + " ( " + thisMapping.Assembly + " )</td>");
                    else
                        Output.WriteLine("      <td>" + class_name_full.Replace("SobekCM.Resource_Object.OAI.Writer.", "") + "</td>");

                    Output.WriteLine("    </tr>");
                }

                Output.WriteLine("  </table>");
            }
            else
            {
                Output.WriteLine("<p>NO METADATA FORMATS DEFINED!!</p>");
            }

	    }

        private void add_engine_qc_tool_info(TextWriter Output)
	    {
            Output.WriteLine("  <h2>Quality Control Configuration</h2>");
            Output.WriteLine("  <p>This configuration controls the divisions which are available for different users when creating the structural metadata online through the quality control tool.  This does not control which types the system supports, only the types that appear for a particular user in the division drop down in the quality control tool used to create structural metadata.  So, if you are working on a project where you know there may only be three types of divisions in the very uniform material, you could create your own profile to make it easier to use and more controlled.</p>");
            Output.WriteLine("  <p>The columns are defined below:</p>");
            Output.WriteLine("  <ul>");
            Output.WriteLine("    <li><span style=\"font-weight:bold\">Division Type</span> - This is the division type the user selects and the type that is most likely to appear within the METS file as the type of division.</li>");
            Output.WriteLine("    <li><span style=\"font-weight:bold\">Base Division Type</span> - If there is a base division type, this base division type will actually be used in the METS and the division type selected will become a division label.</li>");
            Output.WriteLine("    <li><span style=\"font-weight:bold\">Is Active?</span> - This flag indicates if this division type is available for a user to select.</li>");
            Output.WriteLine("    <li><span style=\"font-weight:bold\">Is Nameable?</span> - The flag determines if a user should be allowed to enter a custom name for this division.  For example, if the division is a chapter, then the user can enter the name of the chapter if this flag id set.</li>");
             Output.WriteLine("  </ul>");

            string defaultSetName = UI_ApplicationCache_Gateway.Configuration.QualityControlTool.DefaultProfile;

            QualityControl_Profile defaultProfile = UI_ApplicationCache_Gateway.Configuration.QualityControlTool.Get_Default_Profile();
            if (defaultProfile != null)
            {
                Output.WriteLine("  <h3>" + defaultSetName + " ( <span class=\"sbkSeav_DefaultTextSpan\">default profile</span> )</h3>");

                if ( !String.IsNullOrEmpty(defaultProfile.Profile_Description))
                    Output.WriteLine("  <p>" + defaultProfile.Profile_Description + "</p>");


                Output.WriteLine("  <table class=\"sbkSeav_BaseTable sbkSeav_QcToolProfileTable\">");
                Output.WriteLine("    <tr>");
                Output.WriteLine("      <th class=\"sbkSeav_QcToolProfileTable_TypeCol\">Division Type</th>");
                Output.WriteLine("      <th class=\"sbkSeav_QcToolProfileTable_BaseTypeCol\">Base Division Type</th>");
                Output.WriteLine("      <th class=\"sbkSeav_QcToolProfileTable_ActiveCol\">Is Active?</th>");
                Output.WriteLine("      <th class=\"sbkSeav_QcToolProfileTable_NameableCol\">Is Nameable?</th>");
                Output.WriteLine("    </tr>");
                foreach (QualityControl_Division_Config thisDivision in defaultProfile.Division_Types)
                {
                    Output.WriteLine("    <tr>");

                    Output.WriteLine("      <td>" + thisDivision.TypeName + "</td>");

                    if ( !String.IsNullOrEmpty(thisDivision.BaseTypeName ))
                        Output.WriteLine("      <td>" + thisDivision.TypeName + "</td>");
                    else
                        Output.WriteLine("      <td></td>");

                    if (thisDivision.isActive)
                        Output.WriteLine("      <td class=\"sbkSeav_TableCenterCell\"><img src=\"" + Static_Resources_Gateway.Checkmark2_Png + "\" alt=\"yes\" /></td>");
                    else
                        Output.WriteLine("      <td class=\"sbkSeav_TableCenterCell\"><img src=\"" + Static_Resources_Gateway.Checkmark_Png + "\" alt=\"no\" /></td>");

                    if (thisDivision.isNameable)
                        Output.WriteLine("      <td class=\"sbkSeav_TableCenterCell\"><img src=\"" + Static_Resources_Gateway.Checkmark2_Png + "\" alt=\"yes\" /></td>");
                    else
                        Output.WriteLine("      <td class=\"sbkSeav_TableCenterCell\"><img src=\"" + Static_Resources_Gateway.Checkmark_Png + "\" alt=\"no\" /></td>");

                    Output.WriteLine("    </tr>");
                }
                Output.WriteLine("  </table>");
            }

            // Now, loop through the rest of the profiles and display them (if they exist)
            foreach (QualityControl_Profile thisProfile in UI_ApplicationCache_Gateway.Configuration.QualityControlTool.Profiles)
            {
                if ( thisProfile != defaultProfile )
                {
                    Output.WriteLine("  <h3>" + thisProfile.Profile_Name + "</h3>");

                    if (!String.IsNullOrEmpty(thisProfile.Profile_Description))
                        Output.WriteLine("  <p>" + thisProfile.Profile_Description + "</p>");


                    Output.WriteLine("  <table class=\"sbkSeav_BaseTable sbkSeav_QcToolProfileTable\">");
                    Output.WriteLine("    <tr>");
                    Output.WriteLine("      <th class=\"sbkSeav_QcToolProfileTable_TypeCol\">Division Type</th>");
                    Output.WriteLine("      <th class=\"sbkSeav_QcToolProfileTable_BaseTypeCol\">Base Division Type</th>");
                    Output.WriteLine("      <th class=\"sbkSeav_QcToolProfileTable_ActiveCol\">Is Active?</th>");
                    Output.WriteLine("      <th class=\"sbkSeav_QcToolProfileTable_NameableCol\">Is Nameable?</th>");
                    Output.WriteLine("    </tr>");
                    foreach (QualityControl_Division_Config thisDivision in thisProfile.Division_Types)
                    {
                        Output.WriteLine("    <tr>");

                        Output.WriteLine("      <td>" + thisDivision.TypeName + "</td>");

                        if (!String.IsNullOrEmpty(thisDivision.BaseTypeName))
                            Output.WriteLine("      <td>" + thisDivision.TypeName + "</td>");
                        else
                            Output.WriteLine("      <td></td>");

                        if (thisDivision.isActive)
                            Output.WriteLine("      <td class=\"sbkSeav_TableCenterCell\"><img src=\"" + Static_Resources_Gateway.Checkmark2_Png + "\" alt=\"yes\" /></td>");
                        else
                            Output.WriteLine("      <td class=\"sbkSeav_TableCenterCell\"><img src=\"" + Static_Resources_Gateway.Checkmark_Png + "\" alt=\"no\" /></td>");

                        if (thisDivision.isNameable)
                            Output.WriteLine("      <td class=\"sbkSeav_TableCenterCell\"><img src=\"" + Static_Resources_Gateway.Checkmark2_Png + "\" alt=\"yes\" /></td>");
                        else
                            Output.WriteLine("      <td class=\"sbkSeav_TableCenterCell\"><img src=\"" + Static_Resources_Gateway.Checkmark_Png + "\" alt=\"no\" /></td>");

                        Output.WriteLine("    </tr>");
                    }
                    Output.WriteLine("  </table>");
                }
            }
	    }

        private void add_engine_toplevel_info(TextWriter Output)
        {
            Output.WriteLine("  <span class=\"sbkSeav_BackUpLink\"><a href=\"" + redirectUrl.Replace("%SETTINGSCODE%", "") + "\">Back to top</a></span>");

            Output.WriteLine("  <h2>Engine Configuration</h2>");

            Output.WriteLine("  <p>Some of the core functionality is controlled by configuration files that are primarily consumed by the engine and provided to the user interface as needed.</p>");
            Output.WriteLine("  <p>The configuration information displayed here controls how the final display digital resource object is constructed from the METS-based object, what authentication is available for users of the system, and the overall behavior and metadata support for OAI-PMH.  In addition, this includes the list of all the microservice endpoints which are exposed by the engine for consumption by the user interface or other reporting tools.</p>");

            Output.WriteLine("  <div id=\"sbkSeav_SubPageDesc\">");

            Output.WriteLine("    <p id=\"sbkSeav_SubPageTitle\">This page contains the following subpages:</p>");
            Output.WriteLine("    <h4><a href=\"" + redirectUrl.Replace("%SETTINGSCODE%", "engine/authentication") + "\">Authentication</a></h4>");
            Output.WriteLine("    <p>This subpage contains information about different authentication systems allowed for signing up and logging into this system are detailed here.  This includes the highest level information, such as whether Shibboleth and the standard authentication system is enabled.</p>");
            Output.WriteLine("    <p>If Shibboleth, or any other specialized authentication systems are enabled, additional details will display in this section as well.  In the case of Shibboleth, this includes the basic information (such as URL, name, etc..) for the Shibboleth authentication, as well as how certain data fields returned with the Shiboleth token should be handled.</p>");

            Output.WriteLine("    <h4><a href=\"" + redirectUrl.Replace("%SETTINGSCODE%", "engine/briefitem") + "\">Brief Item Mapping</a></h4>");
            Output.WriteLine("    <p>When an item is requested, the METS file is read into a rich complete SobekCM item object.  In addition, data from the database is mapped into this object.  For display purposes, the data from this SobekCM item is mapped into a Brief Item which is what is generally transferred via the REST API from the engine to the user interface.  The Brief Item is what is used for displaying an item in the public interface.</p>");
            Output.WriteLine("    <p>The mapping from the rich SobekCM Item to the Brief Item is controlled by a number of brief item mappers and displayed in this subpage.  If new metadata fields are being added for display, you will almost certainly need to create a new BriefItemMapper object, to ensure the metadata is available for display online.  In addition, by overriding an existing mapper, you can change how metadata is displayed in the citation, or how the item behaves within your instance of SobekCM.</p>");

            Output.WriteLine("    <h4><a href=\"" + redirectUrl.Replace("%SETTINGSCODE%", "engine/contact") + "\">Contact Form</a></h4>");
            Output.WriteLine("    <p>This controls the behavior of the main, top-level, contact form within SobekCM.  If you wish to use this form, you can control which fields display, the order of display, and the options for a user to select.</p>");
            Output.WriteLine("    <p>This can also be customized at the collection level, but those custom contact us forms would not display here, since they are embedded within the aggregation folders.</p>");

            Output.WriteLine("    <h4><a href=\"" + redirectUrl.Replace("%SETTINGSCODE%", "engine/engine") + "\">Engine Server Endpoints</a></h4>");
            Output.WriteLine("    <p>This subpage details all of the REST API endpoints exposed via the SobekCM engine.  This details each endpoint that is made available, including which HTTP verb(s) are supported (GET, POST, PUT, and/or DELETE), which transfer schemas are supported (JSON, JSON-P, XML, ProtoBuf, or something else), and which method is called to support this endpoint.  In addition, any IP restrictions and component information is included.</p>");

            Output.WriteLine("    <h4><a href=\"" + redirectUrl.Replace("%SETTINGSCODE%", "engine/oaipmh") + "\">OAI-PMH Protocol</a></h4>");
            Output.WriteLine("    <p>The oai-pmh protocol subpage displays all the top-level information for this repository and OAI-PMH ( the <a href=\"https://www.openarchives.org/pmh/\">Open Archives Initiative Protocol for Metadata Harvesting</a> ) is displayed below.  This includes whether it is enabled to be available for this repository and the basic identifying information about this repository as well as the different metadata formats that can be shared via OAI-PMH.</p>");

            Output.WriteLine("    <h4><a href=\"" + redirectUrl.Replace("%SETTINGSCODE%", "engine/qctool") + "\">Quality Control Tool</a></h4>");
            Output.WriteLine("    <p>This subpage displays all of the configuration which controls the divisions which are available for different users when creating the structural metadata online through the quality control tool.  This does not control which types the system supports, only the types that appear for a particular user in the division drop down in the quality control tool used to create structural metadata.  So, if you are working on a project where you know there may only be three types of divisions in the very uniform material, you could create your own profile to make it easier to use and more controlled.</p>");


            Output.WriteLine("  </div>");

        }

		#endregion

		#region HTML helper methods for the UI main page and subpages

		private void add_ui_info(TextWriter Output)
		{
            // If a submode existed, call that method
            switch (uiSubEnum)
            {
                case Settings_UI_SubMode_Enum.Citation_Viewer:
                    add_ui_citation_info(Output);
                    break;

                case Settings_UI_SubMode_Enum.Map_Editor:
                    add_ui_map_editor_info(Output);
                    break;

                case Settings_UI_SubMode_Enum.Microservice_Client_Endpoints:
                    add_ui_client_endpoints_info(Output);
                    break;

                case Settings_UI_SubMode_Enum.Template_Elements:
                    add_ui_template_elements_info(Output);
                    break;

                case Settings_UI_SubMode_Enum.HTML_Viewer_Subviewers:
                    add_ui_viewers_info(Output);
                    break;

                default:
                    add_ui_toplevel_info(Output);
                    break;
            }
        }


        private void add_ui_citation_info(TextWriter Output)
        {
            Output.WriteLine("  <h2>Citation Display Configuration</h2>");
            Output.WriteLine("  <p>Below is the configuration of the display of the citation, or description, of a single digital resource.  The display of the citation is completely configurable through the configuration files. For more extreme customizations, custom citation section writers can be created.</p>");
            Output.WriteLine("  <p>The citation set is split into a series of field sets.  Field sets are used to group similar citation elements under a heading.</p>");
            Output.WriteLine("  <p>It is possible to have multiple citation sets, although the system currently only uses the default set.  However, you could reference the other set in code if you were to customize a citation viewer or add a new item view entirely.</p>");

            // Get the default set
            CitationSet defaultSet = UI_ApplicationCache_Gateway.Configuration.UI.CitationViewer.Get_CitationSet();
            Output.WriteLine("  <h3>" + defaultSet.Name + " Citation Set ( <span class=\"sbkSeav_DefaultTextSpan\">default</span> )</h3>");
            add_ui_citation_set_info(Output, defaultSet);

            // Add the other sets
            foreach (CitationSet thisSet in UI_ApplicationCache_Gateway.Configuration.UI.CitationViewer.CitationSets)
            {
                if (thisSet != defaultSet)
                {
                    Output.WriteLine("  <h3>" + thisSet.Name + " Citation Set ( <span class=\"sbkSeav_DefaultTextSpan\">default</span> )</h3>");
                    add_ui_citation_set_info(Output, thisSet);
                }
            }
        }

	    private void add_ui_citation_set_info(TextWriter Output, CitationSet SetInfo )
	    {
            Output.WriteLine("  <table class=\"sbkSeav_BaseTable sbkSeav_UiCitationTable\">");
            Output.WriteLine("    <tr>");
            Output.WriteLine("      <th class=\"sbkSeav_UiCitationTable_TermCol\">Metadata Term</th>");
            Output.WriteLine("      <th class=\"sbkSeav_UiCitationTable_DisplayCol\">Display Term</th>");
            Output.WriteLine("      <th class=\"sbkSeav_UiCitationTable_SearchCodeCol\">Search Code</th>");
            Output.WriteLine("      <th class=\"sbkSeav_UiCitationTable_OtherCol\">Other</th>");
            Output.WriteLine("    </tr>");

            // Add each field set
	        foreach (CitationFieldSet fieldSet in SetInfo.FieldSets)
	        {
                // Add a row for this
                Output.WriteLine("    <tr class=\"sbkSeav_UiCitationTable_SetRow\">");
                Output.WriteLine("      <td colspan=\"4\">" + fieldSet.Heading + " ( " + fieldSet.ID + " )</td>");
                Output.WriteLine("    </tr>");

                // Now, add each individual citation element
                foreach (CitationElement thisElement in fieldSet.Elements)
	            {
                    Output.WriteLine("    <tr>");
	                Output.WriteLine("      <td style=\"padding-left: 50px;\">" + thisElement.MetadataTerm + "</td>");
                    Output.WriteLine("      <td>" + thisElement.DisplayTerm + "</td>");
                    Output.WriteLine("      <td class=\"sbkSeav_TableCenterCell\">" + thisElement.SearchCode + "</td>");
                    Output.WriteLine("      <td>");
                    if ( !String.IsNullOrEmpty(thisElement.ItemProp ))
                        Output.WriteLine("        microservice itemprop of '" + thisElement.ItemProp + "'.<br />");
                    if ( thisElement.OverrideDisplayTerm == CitationElement_OverrideDispayTerm_Enum.subterm )
                        Output.WriteLine("        display label can be override by subterm (or display term).<br />");
	                if ((thisElement.SectionWriter != null) && ( !String.IsNullOrEmpty(thisElement.SectionWriter.Class_Name)))
	                {
                        if ( !String.IsNullOrEmpty(thisElement.SectionWriter.Assembly))
                            Output.WriteLine("        custom display ( " + thisElement.SectionWriter.Assembly + "." + thisElement.SectionWriter.Class_Name + " ).<br />");
                        else
                            Output.WriteLine("        custom display ( " + thisElement.SectionWriter.Class_Name + " ).<br />");
	                }
	                Output.WriteLine("      </td>");
                    Output.WriteLine("    </tr>");
	            }
	        }
            Output.WriteLine("  </table>");
	    }

	    private void add_ui_map_editor_info(TextWriter Output)
        {
            Output.WriteLine("UI MAP EDITOR INFO HERE");
        }

        private void add_ui_client_endpoints_info(TextWriter Output)
        {
            Output.WriteLine("  <h2>Microservice Endpoints Configuration</h2>");
            Output.WriteLine("  <p>Just as the engine exposes endpoints for the user interface, the user interface utilizes endpoints to retrieve the data.  In general, there will be great symmetry between the endpoints seen under the engine configuration, and the microservice client endpoints, although the engine generally exposes a single endpoint in multiple formats (i.e., xml, json, json-p, etc.. ) where the user interface will just use one of the formats for each endpoint.</p>");
            Output.WriteLine("  <p>It would also be possible to point a set of endpoints to an entirely different system or create custom endpoints outside the application.  The microservice endpoints configuration is where you would instruct the user interface to use those different endpoints.</p>");

            // Create the data table
            DataTable tempTable = new DataTable();
            tempTable.Columns.Add("Key");
            tempTable.Columns.Add("Protocol");
            tempTable.Columns.Add("URL");
            foreach (MicroservicesClient_Endpoint microserviceEndpoint in SobekEngineClient.ConfigObj.Endpoints)
            {
                DataRow newRow = tempTable.NewRow();
                newRow[0] = !String.IsNullOrEmpty(microserviceEndpoint.Key) ? microserviceEndpoint.Key : String.Empty;
                newRow[1] = String.Empty;
                switch (microserviceEndpoint.Protocol)
                {
                    case Core.MicroservicesClient.Microservice_Endpoint_Protocol_Enum.DIRECT:
                        newRow[1] = "DIRECT";
                        break;

                    case Core.MicroservicesClient.Microservice_Endpoint_Protocol_Enum.JSON:
                        newRow[1] = "JSON";
                        break;

                    case Core.MicroservicesClient.Microservice_Endpoint_Protocol_Enum.JSON_P:
                        newRow[1] = "JSON-P";
                        break;

                    case Core.MicroservicesClient.Microservice_Endpoint_Protocol_Enum.PROTOBUF:
                        newRow[1] = "ProtoBuf";
                        break;

                    case Core.MicroservicesClient.Microservice_Endpoint_Protocol_Enum.XML:
                        newRow[1] = "XML";
                        break;
                }
                newRow[2] = !String.IsNullOrEmpty(microserviceEndpoint.URL) ? microserviceEndpoint.URL : String.Empty;
                tempTable.Rows.Add(newRow);
            }

            // Create the data view
            DataView sortMetadata = new DataView(tempTable) {Sort = "Key ASC"};

            Output.WriteLine("  <h3>Microservice Endpoints</h3>");
            Output.WriteLine("  <table class=\"sbkSeav_BaseTable\" id=\"sbkSeav_UiMicroservicesEndpointsTable\">");
            Output.WriteLine("    <tr>");
            Output.WriteLine("      <th id=\"sbkSeav_UiMicroservicesEndpointsTablee_KeyCol\">Key</th>");
            Output.WriteLine("      <th id=\"sbkSeav_UiMicroservicesEndpointsTable_ProtocolCol\">Protocol</th>");
            Output.WriteLine("      <th id=\"sbkSeav_UiMicroservicesEndpointsTable_UrlCol\">URL</th>");
            Output.WriteLine("    </tr>");

            // Step through all the endpoint rows
            foreach (DataRowView thisRow in sortMetadata)
            {
                Output.WriteLine("    <tr>");
                Output.WriteLine("      <td>" + thisRow[0] + "</td>");
                Output.WriteLine("      <td>" + thisRow[1] + "</td>");
                Output.WriteLine("      <td>" + thisRow[2] + "</td>");
                Output.WriteLine("    </tr>");
            }

            Output.WriteLine("  </table>");
        }

        private void add_ui_template_elements_info(TextWriter Output)
        {
            Output.WriteLine("  <h2>Template Elements</h2>");
            Output.WriteLine("  <p>This section lists all of the individual template elements that can be used to create new digital resources online or edit existing metadata.  The template files are XML-driven and reference each individual template element via a <span style=\"font-style:italic\">Type</span> and <span style=\"font-style:italic\">Subtype</span>. This configuration maps from those attributes in the template XML file to the class to use to provide access to edit and enter that metadata.</p>");
            Output.WriteLine("  <p>There are often multiple formats of entry for the same main metadata elements.  This allows different levels of complexity for different templates and different projects.</p>");
            Output.WriteLine("  <p>This configuration allows you to override existing elements or add your own custom template elements for the templates.</p>");


            // Create the data table
            DataTable tempTable = new DataTable();
            tempTable.Columns.Add("Type");
            tempTable.Columns.Add("Subtype");
            tempTable.Columns.Add("Class");
            foreach (TemplateElementConfig element in UI_ApplicationCache_Gateway.Configuration.UI.TemplateElements.Elements)
            {
                DataRow newRow = tempTable.NewRow();
                newRow[0] = element.Type;
                newRow[1] = !String.IsNullOrEmpty(element.Subtype) ? element.Subtype : String.Empty;
                if (!String.IsNullOrEmpty(element.Assembly))
                    newRow[2] = element.Class + "( " + element.Assembly + " )";
                else
                    newRow[2] = element.Class;
                tempTable.Rows.Add(newRow);
            }

            Output.WriteLine("  <table class=\"sbkSeav_BaseTable\" id=\"sbkSeav_UiTemplateElementsTable\">");
            Output.WriteLine("    <tr>");
            Output.WriteLine("      <th id=\"sbkSeav_UiTemplateElementsTable_TypeCol\">Type</th>");
            Output.WriteLine("      <th id=\"sbkSeav_UiTemplateElementsTable_SubtypeCol\">Subtype</th>");
            Output.WriteLine("      <th id=\"sbkSeav_UiTemplateElementsTable_ClassCol\">Class</th>");
            Output.WriteLine("    </tr>");

            // Create the data view
            DataView sortMetadata = new DataView(tempTable) {Sort = "Type ASC, Subtype ASC"};

            // Step through all the roots
            foreach (DataRowView thisRow in sortMetadata)
            {
                Output.WriteLine("    <tr>");
                Output.WriteLine("      <td>" + thisRow[0] + "</td>");
                Output.WriteLine("      <td>" + thisRow[1] + "</td>");
                Output.WriteLine("      <td>" + thisRow[2] + "</td>");
                Output.WriteLine("    </tr>");
            }

            Output.WriteLine("  </table>");
        }

        private void add_ui_viewers_info(TextWriter Output)
        {
            Output.WriteLine("  <h2>HTML Writers and Viewers</h2>");
            Output.WriteLine("  <p>This section includes all of the configuration for the HTML writers and subviewers used by those writers.  This allows complete customization of the item viewers, aggregation viewers, and many of the other views in the system.  You could also add new custom viewers through this configuration.</p>");
            Output.WriteLine("  <p>Currently, only the item viewers utilize the configuration files to determine viewers, but as of version 4.11, the rest of the viewer information should also appear here.</p>");

            if (!String.IsNullOrEmpty(UI_ApplicationCache_Gateway.Configuration.UI.WriterViewers.Items.Assembly))
            {
                Output.WriteLine("  <h3>Item Writer - " + UI_ApplicationCache_Gateway.Configuration.UI.WriterViewers.Items.Class + " ( " + UI_ApplicationCache_Gateway.Configuration.UI.WriterViewers.Items.Assembly + " )</h3>");
            }
            else
            {
                Output.WriteLine("  <h3>Item Writer - " + UI_ApplicationCache_Gateway.Configuration.UI.WriterViewers.Items.Class.Replace("SobekCM.Library.ItemViewer.Viewers.", "") + "</h3>");
            }

            // This is a little complicated since we are adding from TWO sources.. the database
            // settings list and the configuration which points to the classes in the UI
            Dictionary<string, string> viewerAdded = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase );

            // Create the data table
            DataTable tempTable = new DataTable();
            DataColumn enabledCol = tempTable.Columns.Add("Enabled?");
            DataColumn defaultCol = tempTable.Columns.Add("Default?");
            DataColumn alwaysAddCol = tempTable.Columns.Add("AlwaysAdd");
            DataColumn typeCol = tempTable.Columns.Add("ViewerType");
            DataColumn codeCol = tempTable.Columns.Add("ViewerCode");
            DataColumn orderCol = tempTable.Columns.Add("Order");
            DataColumn menuOrderCol = tempTable.Columns.Add("MenuOrder");
            DataColumn classCol = tempTable.Columns.Add("Class");
            DataColumn extensionsCol = tempTable.Columns.Add("Extensions");
            foreach (ItemSubViewerConfig viewer in UI_ApplicationCache_Gateway.Configuration.UI.WriterViewers.Items.Viewers)
            {
                DataRow newRow = tempTable.NewRow();
                if (viewer.Enabled)
                    newRow[enabledCol] = "<img src=\"" + Static_Resources_Gateway.Checkmark2_Png + "\" alt=\"yes\" />";
                else
                    newRow[enabledCol] = "<img src=\"" + Static_Resources_Gateway.Checkmark_Png + "\" alt=\"no\" />";
                newRow[typeCol] = viewer.ViewerType;
                newRow[codeCol] = viewer.ViewerCode;
                if (!String.IsNullOrEmpty(viewer.Assembly))
                    newRow[classCol] = viewer.Class + " ( " + viewer.Assembly + " )";
                else
                    newRow[classCol] = viewer.Class.Replace("SobekCM.Library.ItemViewer.Viewers.", "");
                if (viewer.ManagementViewer)
                    newRow[alwaysAddCol] = "<img src=\"" + Static_Resources_Gateway.Checkmark2_Png + "\" alt=\"yes\" />";
                else
                    newRow[alwaysAddCol] = "<img src=\"" + Static_Resources_Gateway.Checkmark_Png + "\" alt=\"no\" />";
                StringBuilder extensionsBldr = new StringBuilder();
                if ((viewer.FileExtensions != null) && ( viewer.FileExtensions.Length > 0 ))
                {
                    foreach (string thisExtension in viewer.FileExtensions)
                    {
                        if (extensionsBldr.Length > 0)
                            extensionsBldr.Append(", " + thisExtension);
                        else
                            extensionsBldr.Append(thisExtension);
                    }
                }
                else if ((viewer.PageExtensions != null) && ( viewer.PageExtensions.Length > 0 ))
                {
                    foreach (string thisExtension in viewer.PageExtensions)
                    {
                        if (extensionsBldr.Length > 0)
                            extensionsBldr.Append(", " + thisExtension);
                        else
                            extensionsBldr.Append(thisExtension);
                    }
                }
                newRow[extensionsCol] = extensionsBldr.ToString();

                // Is there information on this viewer in the database?
                DbItemViewerType dbType = UI_ApplicationCache_Gateway.Settings.DbItemViewers.Get_ViewerType(viewer.ViewerType);
                if (dbType != null)
                {
                    // Set the default column
                    if (dbType.DefaultView)
                        newRow[defaultCol] = "<img src=\"" + Static_Resources_Gateway.Checkmark2_Png + "\" alt=\"yes\" />";
                    else
                        newRow[defaultCol] = "<img src=\"" + Static_Resources_Gateway.Checkmark_Png + "\" alt=\"no\" />";


                    // Set the order column and menu order
                    newRow[orderCol] = dbType.Order;
                    newRow[menuOrderCol] = dbType.MenuOrder;
                }

                // Since this was added, add it to the dictionary to avoid being double added
                viewerAdded[viewer.ViewerType] = viewer.ViewerType;

                // Add this row to the growing table
                tempTable.Rows.Add(newRow);
            }

            // Step through all the data from the database about viewers
            foreach (DbItemViewerType dbType in UI_ApplicationCache_Gateway.Settings.DbItemViewers.ViewerTypes)
            {
                // Was this already added?
                if (viewerAdded.ContainsKey(dbType.ViewType))
                    continue;

                // Not added, so add a new row
                DataRow newRow = tempTable.NewRow();

                // Set some empty or default rows
                newRow[codeCol] = String.Empty;
                newRow[typeCol] = dbType.ViewType; 
                newRow[enabledCol] = "-";
                newRow[alwaysAddCol] = "-";
                newRow[classCol] = "<span style=\"color:#999999;font-style:italic\">not implemented</span>";
                newRow[extensionsCol] = String.Empty;
                
                // Set the default column
                if (dbType.DefaultView)
                    newRow[defaultCol] = "<img src=\"" + Static_Resources_Gateway.Checkmark2_Png + "\" alt=\"yes\" />";
                else
                    newRow[defaultCol] = "<img src=\"" + Static_Resources_Gateway.Checkmark_Png + "\" alt=\"no\" />";

                // Set the order column and menu order
                newRow[orderCol] = dbType.Order;
                newRow[menuOrderCol] = dbType.MenuOrder;

                // Add this row to the growing table
                tempTable.Rows.Add(newRow);
            }

            // Create the data view
            DataView sortMetadata = new DataView(tempTable) {Sort = "ViewerType ASC"};

            Output.WriteLine("  <table class=\"sbkSeav_BaseTable\" id=\"sbkSeav_UiViewersTable\">");
            Output.WriteLine("    <tr>");
            Output.WriteLine("      <th id=\"sbkSeav_UiViewersTable_EnabledCol\">Enabled?</th>");
            Output.WriteLine("      <th id=\"sbkSeav_UiViewersTable_TypeCol\">Viewer Type</th>");
            Output.WriteLine("      <th id=\"sbkSeav_UiViewersTable_CodeCol\">Viewer Code</th>");
            Output.WriteLine("      <th id=\"sbkSeav_UiViewersTable_DefaultCol\">Default<br />View?</th>");
            Output.WriteLine("      <th id=\"sbkSeav_UiViewersTable_AlwaysAddCol\">Always<br />Add?</th>");
            Output.WriteLine("      <th id=\"sbkSeav_UiViewersTable_OrderCol\">View<br />Order</th>");
            Output.WriteLine("      <th id=\"sbkSeav_UiViewersTable_MenuOrderCol\">Menu<br />Order</th>");
            Output.WriteLine("      <th id=\"sbkSeav_UiViewersTable_ClassCol\">Class</th>");
            Output.WriteLine("      <th id=\"sbkSeav_UiViewersTable_ExtensionsCol\">Extensions</th>");
            Output.WriteLine("    </tr>");

            foreach (DataRowView thisRow in sortMetadata)
            {
                DataRow sourceRow = thisRow.Row;

                Output.WriteLine("    <tr>");
                Output.WriteLine("      <td class=\"sbkSeav_TableCenterCell\">" + sourceRow[enabledCol] + "</td>");
                Output.WriteLine("      <td>" + sourceRow[typeCol] + "</td>");
                Output.WriteLine("      <td>" + sourceRow[codeCol] + "</td>");
                Output.WriteLine("      <td class=\"sbkSeav_TableCenterCell\">" + sourceRow[defaultCol] + "</td>");
                Output.WriteLine("      <td class=\"sbkSeav_TableCenterCell\">" + sourceRow[alwaysAddCol] + "</td>");
                Output.WriteLine("      <td class=\"sbkSeav_TableCenterCell\">" + sourceRow[orderCol] + "</td>");
                Output.WriteLine("      <td class=\"sbkSeav_TableCenterCell\">" + sourceRow[menuOrderCol] + "</td>");
                Output.WriteLine("      <td>" + sourceRow[classCol] + "</td>");
                Output.WriteLine("      <td>" + sourceRow[extensionsCol] + "</td>");
                Output.WriteLine("    </tr>");
            }

            Output.WriteLine("  </table>");
        }

        private void add_ui_toplevel_info(TextWriter Output)
        {
            Output.WriteLine("  <span class=\"sbkSeav_BackUpLink\"><a href=\"" + redirectUrl.Replace("%SETTINGSCODE%", "") + "\">Back to top</a></span>");

            Output.WriteLine("  <h2>User Interface Configuration</h2>");

            Output.WriteLine("  <p>Many aspects of the final display of aggregations and digital resources are controlled by configuration files.  This configuration includes how the citation, or description, appears for digital resources, how the overall system displays HTML, which metadata fields can be exposed for editing, and the overall operation through the configuration of the microservices employed by the user interface.</p>");

            Output.WriteLine("  <div id=\"sbkSeav_SubPageDesc\">");

            Output.WriteLine("    <p id=\"sbkSeav_SubPageTitle\">This page contains the following subpages:</p>");
            Output.WriteLine("    <h4><a href=\"" + redirectUrl.Replace("%SETTINGSCODE%", "ui/citation") + "\">Citation Viewer</a></h4>");
            Output.WriteLine("    <p>The citation viewer subpage includes the configuration of the display of the citation, or description, of a single digital resource.  The display of the citation is completely configurable through the configuration files. For more extreme customizations, custom citation section writers can be created.</p>");

            Output.WriteLine("    <h4><a href=\"" + redirectUrl.Replace("%SETTINGSCODE%", "ui/microservices") + "\">Microservice Client Endpoints</a></h4>");
            Output.WriteLine("    <p>Just as the engine exposes endpoints for the user interface, the user interface utilizes endpoints to retrieve the data.  In general, there will be great symmetry between the endpoints seen under the engine configuration, and the microservice client endpoints, although the engine generally exposes a single endpoint in multiple formats (i.e., xml, json, json-p, etc.. ) where the user interface will just use one of the formats for each endpoint.</p>");

            Output.WriteLine("    <h4><a href=\"" + redirectUrl.Replace("%SETTINGSCODE%", "ui/template") + "\">Template Elements</a></h4>");
            Output.WriteLine("    <p>This subpage lists all of the individual template elements that can be used to create new digital resources online or edit existing metadata.  The template files are XML-driven and reference each individual template element via a <span style=\"font-style:italic\">Type</span> and <span style=\"font-style:italic\">Subtype</span>. This configuration maps from those attributes in the template XML file to the class to use to provide access to edit and enter that metadata.</p>");

            Output.WriteLine("    <h4><a href=\"" + redirectUrl.Replace("%SETTINGSCODE%", "ui/viewers") + "\">HTML Viewers/Subviewers</a></h4>");
            Output.WriteLine("    <p>This subpage includes all of the configuration for the HTML writers and subviewers used by those writers.  This allows complete customization of the item viewers, aggregation viewers, and many of the other views in the system.  You could also add new custom viewers through this configuration.</p>");


            Output.WriteLine("  </div>");
        }

		#endregion

		#region HTML helper methods for the HTML snippets main page and subpages

		private void add_html_info(TextWriter Output)
		{
            // If a submode existed, call that method
            switch (htmlSubEnum)
            {
                case Settings_HTML_SubMode_Enum.Missing_Page:
                    add_html_missing_page_info(Output);
                    break;

                case Settings_HTML_SubMode_Enum.No_Results:
                    add_html_no_results_info(Output);
                    break;

                default:
                    add_html_toplevel_info(Output);
                    break;
            }
        }


        private void add_html_missing_page_info(TextWriter Output)
        {
            Output.WriteLine("  <h2>Missing Page HTML Snippet</h2>");

            Output.WriteLine("  <p>This special snippet of HTML is displayed when a non-administrative user requests a page that does not exist.  In addition, a 404 error code is returned.</p>");
            Output.WriteLine("  <p>Administrative users see a special missing page message where they have options to add web content pages, aggregations, or items to match the errant URL.  To see this snippet in action, you will need to logout and then request a non-existant item, aggregation, or page.</p>");

            Output.WriteLine("  <div id=\"sbkSeav_SmallerPageWrapper\">");

            // Add the buttons
            add_buttons(Output);

            Output.WriteLine();
            Output.WriteLine("<br /><br />");
            Output.WriteLine();

            HTML_Based_Content missingContent = SobekEngineClient.WebContent.Get_Special_Missing_Page(RequestSpecificValues.Tracer);
            if ((missingContent == null) || ( String.IsNullOrEmpty(missingContent.Content)))
            {
                Output.WriteLine("<h3>ERROR!  Missing web content object was returned from client as NULL or without content</h3>");
            }
            else
            {
                string snippet = missingContent.Content;
                Output.WriteLine("  <textarea id=\"sbkSeav_HtmlEdit\" name=\"sbkSeav_NoResultsHtmlEdit\" style=\"width:800px; height:400px\" >");
                Output.WriteLine(snippet.Replace("<%", "[%").Replace("%>", "%]"));
                Output.WriteLine("  </textarea>");
            }


            Output.WriteLine("<br />");
            Output.WriteLine();

            // Add final buttons
            add_buttons(Output);

            Output.WriteLine("  </div>");
        }

        private void add_html_no_results_info(TextWriter Output)
        {
            Output.WriteLine("  <h2>No Results HTML Snippet</h2>");
            Output.WriteLine("  <p>This special HTML snippet is used to provide users's a customized message when no results are found for a search (or a browse).  This mesasge is in place of the generic 'Your search returned no results' message.</p>");

            Output.WriteLine("  <div id=\"sbkSeav_SmallerPageWrapper\">");

            // Add the buttons
            add_buttons(Output);

            Output.WriteLine();
            Output.WriteLine("<br /><br />");
            Output.WriteLine();

            string noResultsSnippet = No_Results_ResultsViewer.Get_NoResults_Text();
            Output.WriteLine("  <textarea id=\"sbkSeav_HtmlEdit\" name=\"sbkSeav_NoResultsHtmlEdit\" style=\"width:800px; height:400px\" >");
            Output.WriteLine(noResultsSnippet.Replace("<%", "[%").Replace("%>", "%]"));
            Output.WriteLine("  </textarea>");

            Output.WriteLine("<br />");
            Output.WriteLine();

            // Add final buttons
            add_buttons(Output);

            Output.WriteLine("  </div>");
        }

        private void add_html_toplevel_info(TextWriter Output)
        {
            Output.WriteLine("  <span class=\"sbkSeav_BackUpLink\"><a href=\"" + redirectUrl.Replace("%SETTINGSCODE%", "") + "\">Back to top</a></span>");

            Output.WriteLine("  <h2>Special HTML Source Snippets</h2>");

            Output.WriteLine("  <p>Several small snippets of HTML exist that allow an instance to easily customize several standard messages for your users.  The subpages here allow you to view and edit these HTML snippets.</p>");

            Output.WriteLine("  <div id=\"sbkSeav_SubPageDesc\">");

            Output.WriteLine("    <p id=\"sbkSeav_SubPageTitle\">This page contains the following subpages:</p>");

            Output.WriteLine("    <h4><a href=\"" + redirectUrl.Replace("%SETTINGSCODE%", "html/missing") + "\">Missing Page</a></h4>");
            Output.WriteLine("    <p>This special snippet of HTML is displayed when a non-administrative user requests a page that does not exist.  In addition, a 404 error code is returned.</p>");

            Output.WriteLine("    <h4><a href=\"" + redirectUrl.Replace("%SETTINGSCODE%", "html/noresults") + "\">No Results</a></h4>");
            Output.WriteLine("    <p>This special HTML snippet is used to provide users's a customized message when no results are found for a search (or a browse).  This message is used in place of the generic 'Your search returned no results' message.</p>");
             
            Output.WriteLine("  </div>");
        }

		#endregion

		#region HTML helper methods for the extensions main page and subpages

		private void add_extensions_info(TextWriter Output)
		{
            Output.WriteLine("  <span class=\"sbkSeav_BackUpLink\"><a href=\"" + redirectUrl.Replace("%SETTINGSCODE%", "") + "\">Back to top</a></span>");

            Output.WriteLine("EXTENSIONS INFO HERE");
		}

		#endregion


	    /// <summary> Gets the CSS class of the container that the page is wrapped within </summary>
	    /// <value> Returns .... </value>
	    public override string Container_CssClass
	    {
	        get
	        {
	            switch (mainMode)
	            {
	                case Settings_Mode_Enum.Settings:
                        return "sbkSeav_ContainerInner1300"; 

                    case Settings_Mode_Enum.Builder:
                        if ( builderSubEnum == Settings_Builder_SubMode_Enum.Builder_Modules )
                            return "sbkSeav_ContainerInner1500";
                        return "sbkSeav_ContainerInner1300"; 

                    case Settings_Mode_Enum.Metadata:
                        if (( metadataSubEnum == Settings_Metadata_SubMode_Enum.METS_Section_Reader_Writers ) || ( metadataSubEnum == Settings_Metadata_SubMode_Enum.Metdata_Reader_Writers ))
                            return "sbkSeav_ContainerInner1600";
                        if ( metadataSubEnum == Settings_Metadata_SubMode_Enum.NONE )
                            return "sbkSeav_ContainerInner1300";
                        return "sbkSeav_ContainerInner1500"; 


	            }
	            return "sbkSeav_ContainerInner"; 
	            
	        }
	    }

	}
}