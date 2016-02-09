#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms.VisualStyles;
using SobekCM.Core.Client;
using SobekCM.Core.Configuration;
using SobekCM.Core.Configuration.Authentication;
using SobekCM.Core.Configuration.Engine;
using SobekCM.Core.Configuration.Extensions;
using SobekCM.Core.Configuration.OAIPMH;
using SobekCM.Core.Navigation;
using SobekCM.Core.Settings;
using SobekCM.Core.UI_Configuration;
using SobekCM.Core.Users;
using SobekCM.Core.WebContent;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.ResultsViewer;
using SobekCM.Library.UI;
using SobekCM.Resource_Object.Configuration;
using SobekCM.Tools;

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
		private readonly string actionMessage;
		private readonly StringBuilder errorBuilder;

		private bool isValid;
	  
		private readonly bool category_view;
		private readonly bool limitedRightsMode;
		private readonly bool readonlyMode;
		private readonly Admin_Setting_Collection currSettings;
		private SortedList<string, string> tabPageNames;
		private Dictionary<string, List<Admin_Setting_Value>> settingsByPage;
	    private List<Admin_Setting_Value> builderSettings;

        private readonly Settings_Mode_Enum mainMode = Settings_Mode_Enum.NONE;
        private readonly Settings_Builder_SubMode_Enum builderSubEnum = Settings_Builder_SubMode_Enum.NONE;
	    private readonly Settings_Metadata_SubMode_Enum metadataSubEnum = Settings_Metadata_SubMode_Enum.NONE;
        private readonly Settings_Engine_SubMode_Enum engineSubEnum = Settings_Engine_SubMode_Enum.NONE;
        private readonly Settings_UI_SubMode_Enum uiSubEnum = Settings_UI_SubMode_Enum.NONE;
        private readonly Settings_HTML_SubMode_Enum htmlSubEnum = Settings_HTML_SubMode_Enum.NONE;
	    private readonly int extensionSubMode = -1;

		#region Enumeration of the main modes of the settings, as well as submodes

		private enum Settings_Mode_Enum : byte
		{
			NONE,
	
			Settings,

			Builder,

            Metadata,

			Engine,

			UI,

			HTML,

			Extensions
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
					// If this was read-only, really shouldn't do anything here
					if (readonlyMode)
						return;

					// First, create the setting lookup by ID, and the list of IDs to look for
					List<short> settingIds = new List<short>();
					Dictionary<short, Admin_Setting_Value> settingsObjsById = new Dictionary<short, Admin_Setting_Value>();
					foreach (Admin_Setting_Value Value in currSettings.Settings)
					{
						// If this is readonly, will not prepare to update
						if ((!Is_Value_ReadOnly(Value, readonlyMode, limitedRightsMode )) && ( !Value.Hidden ))
						{
							settingIds.Add(Value.SettingID);
							settingsObjsById[Value.SettingID] = Value;
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

					if ( isValid )
					{
						// Try to save each setting
						//errors += newSettings.Keys.Count(ThisKey => !SobekCM_Database.Set_Setting(ThisKey, newSettings[ThisKey]));

						// Prepare the action message
					   // if (errors > 0)
					   //     actionMessage = "Save completed, but with " + errors + " errors.";
					   // else
							actionMessage = "Settings saved";

						// Assign this to be used by the system
						UI_ApplicationCache_Gateway.ResetSettings();

						// Also, reset the source for static files, as thatmay have changed
						Static_Resources.Config_Read_Attempted = false;

						// Get all the settings again 
						build_setting_objects_for_display();
					}
					else
					{
						actionMessage = errorBuilder.ToString().Replace("\n", "<br />");
					}
				}
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
			get { return Static_Resources.Settings_Img; }
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
			Output.WriteLine("<script src=\"" + Static_Resources.Sobekcm_Admin_Js + "\" type=\"text/javascript\"></script>");
			Output.WriteLine();

			Output.WriteLine("<div class=\"sbkAdm_HomeText\">");
			Output.WriteLine("  <div id=\"sbkSeav_Explanation\">");
			Output.WriteLine("    <p>This form allows a user to view and edit all the main system-wide settings which allow the SobekCM web application and assorted related applications to function correctly within each custom architecture and each institution.</p>");
			Output.WriteLine("    <p>For more information about these settings, <a href=\"" + UI_ApplicationCache_Gateway.Settings.System.Help_URL(RequestSpecificValues.Current_Mode.Base_URL) + "adminhelp/settings\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</p>");

			// Add portal admin message
			if (!RequestSpecificValues.Current_User.Is_System_Admin)
			{
				Output.WriteLine("    <p>Portal Admins have rights to see these settings. System Admins can change these settings.</p>");
			}

			Output.WriteLine("  </div>");
			Output.WriteLine("</div>");
			Output.WriteLine();

			Output.WriteLine("<table id=\"sbkSeav_MainTable\">");
			Output.WriteLine("  <tr>");
			Output.WriteLine("    <td id=\"sbkSeav_TocArea\">");


			// Determine the redirect URL now
			string[] origUrlSegments = RequestSpecificValues.Current_Mode.Remaining_Url_Segments;
			RequestSpecificValues.Current_Mode.Remaining_Url_Segments = new string[] { "%SETTINGSCODE%" };
			string redirectUrl = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
			RequestSpecificValues.Current_Mode.Remaining_Url_Segments = origUrlSegments;

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

			Output.WriteLine(add_leftnav_h2_link("Builder", "builder", redirectUrl, currentViewerCode));
			Output.WriteLine("        <ul>");
			Output.WriteLine(add_leftnav_li_link("Builder Settings", "builder/settings", redirectUrl, currentViewerCode));
			Output.WriteLine(add_leftnav_li_link("Incoming Folders", "builder/folders", redirectUrl, currentViewerCode));
			Output.WriteLine(add_leftnav_li_link("Builder Modules", "builder/modules", redirectUrl, currentViewerCode));
			Output.WriteLine("        </ul>");

            Output.WriteLine(add_leftnav_h2_link("Metadata Configuration", "metadata", redirectUrl, currentViewerCode));
            Output.WriteLine("        <ul>");
            Output.WriteLine(add_leftnav_li_link("File Readers/Writers", "metadata/filereaders", redirectUrl, currentViewerCode));
            Output.WriteLine(add_leftnav_li_link("METS Section Readers/Writers", "metadata/metsreaders", redirectUrl, currentViewerCode));
            Output.WriteLine(add_leftnav_li_link("METS Writing Profiles", "metadata/metsprofiles", redirectUrl, currentViewerCode));
            Output.WriteLine(add_leftnav_li_link("Standard Metadata Modules", "metadata/modules", redirectUrl, currentViewerCode));
            Output.WriteLine("        </ul>");

			Output.WriteLine(add_leftnav_h2_link("Engine Configuration", "engine", redirectUrl, currentViewerCode));
			Output.WriteLine("        <ul>");
			Output.WriteLine(add_leftnav_li_link("Authentication", "engine/authentication", redirectUrl, currentViewerCode));
			Output.WriteLine(add_leftnav_li_link("Brief Item Mapping", "engine/briefitem", redirectUrl, currentViewerCode));
			Output.WriteLine(add_leftnav_li_link("Contact Form", "engine/contact", redirectUrl, currentViewerCode));  /** UI? **/
			Output.WriteLine(add_leftnav_li_link("Engine Server Endpoints", "engine/endpoints", redirectUrl, currentViewerCode));
			Output.WriteLine(add_leftnav_li_link("OAI-PMH Protocol", "engine/oaipmh", redirectUrl, currentViewerCode));
			Output.WriteLine(add_leftnav_li_link("Quality Control Tool", "engine/qctool", redirectUrl, currentViewerCode));    /** UI? **/
			Output.WriteLine("        </ul>");

			Output.WriteLine(add_leftnav_h2_link("UI Configuration", "ui", redirectUrl, currentViewerCode));
			Output.WriteLine("        <ul>");
			Output.WriteLine(add_leftnav_li_link("Citation Viewer", "ui/citation", redirectUrl, currentViewerCode));
			Output.WriteLine(add_leftnav_li_link("Map Editor", "ui/mapeditor", redirectUrl, currentViewerCode));
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

				Output.WriteLine("    <button title=\"Do not apply changes\" class=\"sbkAdm_RoundButton\" onclick=\"window.location.href='" + RequestSpecificValues.Current_Mode.Base_URL + "my/admin'; return false;\"><img src=\"" + Static_Resources.Button_Previous_Arrow_Png + "\" class=\"sbkAdm_RoundButton_LeftImg\" alt=\"\" /> CANCEL</button> &nbsp; &nbsp; ");
				Output.WriteLine("    <button title=\"Save changes\" class=\"sbkAdm_RoundButton\" onclick=\"admin_settings_save(); return false;\">SAVE <img src=\"" + Static_Resources.Button_Next_Arrow_Png + "\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button>");
			}
			else
			{
				Output.WriteLine("    <button class=\"sbkAdm_RoundButton\" onclick=\"window.location.href='" + RequestSpecificValues.Current_Mode.Base_URL + "my/admin'; return false;\"><img src=\"" + Static_Resources.Button_Previous_Arrow_Png + "\" class=\"sbkAdm_RoundButton_LeftImg\" alt=\"\" /> BACK</button> &nbsp; &nbsp; ");
			}
			Output.WriteLine("  </div>");
			Output.WriteLine();
		}

		#endregion

		#region HTML helper method for the top-level information page

		private void add_top_level_info(TextWriter Output)
		{
			Output.WriteLine("TOP LEVEL INFO HERE");
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
				Output.WriteLine("SETTINGS TOP LEVEL PAGE");
			}
			else
			{
				string tabPageNameKey = tabPageNames.Keys[tabPage - 1];
				string tabPageName = tabPageNames[tabPageNameKey];
				Output.WriteLine("  <h2>" + tabPageName.Trim() + "</h2>");

				// Add the buttons
				add_buttons(Output);

				Output.WriteLine();
				add_tab_page_info(Output, tabPageName, settingsByPage[tabPageName]);

				Output.WriteLine("<br />");
				Output.WriteLine();

				// Add final buttons
				add_buttons(Output);
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
				if (!settingsByPage.ContainsKey(thisValue.TabPage))
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
			string tabNameForConfig = null;
			foreach (string thisTabName in tabPageNames.Values)
			{
				if (thisTabName.IndexOf("Server", StringComparison.OrdinalIgnoreCase) >= 0)
				{
					tabNameForConfig = thisTabName;
					break;
				}
			}
			if (String.IsNullOrEmpty(tabNameForConfig))
			{
				foreach (string thisTabName in tabPageNames.Values)
				{
					if (thisTabName.IndexOf("System", StringComparison.OrdinalIgnoreCase) >= 0)
					{
						tabNameForConfig = thisTabName;
						break;
					}
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
				Output.WriteLine("                    <img  class=\"sbkSeav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + Value.Help.Replace("'", "").Replace("\"", "").Replace("\n", "\\n") + "');\"  title=\"" + Value.Help.Replace("'", "").Replace("\"", "").Replace("\n", "\\n") + "\" />");
			Output.WriteLine("                  </td>");
			Output.WriteLine("                </tr>");
			Output.WriteLine("              </table>");
			Output.WriteLine("            </td>");
			Output.WriteLine("          </tr>");
		}

		#region Methods related to special validations

		private bool validate_update_entered_data(List<Simple_Setting> newValues)
		{
			isValid = true;
			foreach (Simple_Setting thisSetting in newValues)
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
			bool missing_start = true;
			foreach (string possibleStart in StartsWith)
			{
				if (NewSetting.Value.StartsWith(possibleStart, StringComparison.OrdinalIgnoreCase))
				{
					missing_start = false;
					break;
				}
			}

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
			bool missing_start = true;
			foreach (string possibleStart in StartsWith)
			{
				if (NewSetting.Value.StartsWith(possibleStart, StringComparison.OrdinalIgnoreCase))
				{
					missing_start = false;
					break;
				}
			}

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
            
            Output.WriteLine("  <h2>Builder Information</h2>");

            Output.WriteLine("  <table style=\"padding-left:50px\">");
            Output.WriteLine("    <tr><td>Last Run:</td><td>" + lastRun + "</td></tr>");
            Output.WriteLine("    <tr><td>Last Result:</td><td>" + lastResult + "</td></tr>");
            Output.WriteLine("    <tr><td>Builder Version:</td><td>" + builderVersion + "</td></tr>");
            Output.WriteLine("    <tr><td>Builder Operation:</td><td>" + currentBuilderMode + "</td></tr>");
            Output.WriteLine("  </table>");

            // Look to see if any builder folders exist
	        if ((UI_ApplicationCache_Gateway.Settings.Builder.IncomingFolders == null) || (UI_ApplicationCache_Gateway.Settings.Builder.IncomingFolders.Count == 0))
	        {
	            Output.WriteLine("  <br /><br />");
                Output.WriteLine("  <strong>YOU HAVE NO BUILDER FOLDERS DEFINED!!</strong>");
	        }


	    }

        private void add_builder_settings_info(TextWriter Output)
        {
            Output.WriteLine("  <h2>Builder Settings</h2>");

            // Add the buttons
            add_buttons(Output);

            Output.WriteLine();
            add_tab_page_info(Output, "Builder Settings", builderSettings, "Status");

            Output.WriteLine("<br />");
            Output.WriteLine();

            // Add final buttons
            add_buttons(Output);
        }

        private void add_builder_folders_info(TextWriter Output)
        {
            Output.WriteLine("  <h2>Builder Folders</h2>");

            // Look to see if any builder folders exist
            if ((UI_ApplicationCache_Gateway.Settings.Builder.IncomingFolders == null) || (UI_ApplicationCache_Gateway.Settings.Builder.IncomingFolders.Count == 0))
            {
                Output.WriteLine("  <br /><br />");
                Output.WriteLine("  <strong>YOU HAVE NO BUILDER FOLDERS DEFINED!!</strong>");
                return;
            }

            // Show the information for each builder folder
            foreach (Builder_Source_Folder incomingFolder in UI_ApplicationCache_Gateway.Settings.Builder.IncomingFolders)
            {
                // Add the basic folder information
                Output.WriteLine("  <span class=\"sbkSeav_BuilderFolderName\">" + incomingFolder.Folder_Name + "</span>");
                Output.WriteLine("  <table class=\"sbkSeav_BuilderFolderTable\">");
                Output.WriteLine("    <tr><td>Network Folder:</td><td>" +  incomingFolder.Inbound_Folder + "</td></tr>");
                Output.WriteLine("    <tr><td>Processing Folder:</td><td>" + incomingFolder.Processing_Folder + "</td></tr>");
                Output.WriteLine("    <tr><td>Error Folder:</td><td>" + incomingFolder.Failures_Folder + "</td></tr>");

                // If there are BibID restrictions, add them
                if ( !String.IsNullOrEmpty(incomingFolder.BibID_Roots_Restrictions))
                    Output.WriteLine("    <tr><td>BibID Restrictions:</td><td>" + incomingFolder.BibID_Roots_Restrictions + "</td></tr>");

                // Collect all the options and display them
                StringBuilder builder = new StringBuilder();
                if (incomingFolder.Allow_Deletes) builder.Append("Allow deletes ; ");
                if (incomingFolder.Allow_Folders_No_Metadata) builder.Append("Allow folder with no metadata ; ");
                if (incomingFolder.Allow_Metadata_Updates) builder.Append("Allow metadata updates ; ");
                if (incomingFolder.Archive_All_Files) builder.Append("Archive all files ; ");
                else if (incomingFolder.Archive_TIFFs) builder.Append("Archive TIFFs ; ");
                if (incomingFolder.Perform_Checksum) builder.Append("Perform checksum ; ");
                Output.WriteLine("    <tr><td>Options</td><td>" + builder.ToString() + "</td></tr>");
            
                // Add the possible actions here
                Output.WriteLine("    <tr><td>Actions:</td><td><a href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "admin/builderfolders/" + incomingFolder.IncomingFolderID + "\">edit</a></td></tr>");
                Output.WriteLine("  </table>");
            }
        }

        private void add_builder_modules_info(TextWriter Output)
        {
            Output.WriteLine("  <h2>Builder Modules</h2>");

            // Add all the PRE PROCESS MODULE settings
            if ((UI_ApplicationCache_Gateway.Settings.Builder.PreProcessModulesSettings != null) && (UI_ApplicationCache_Gateway.Settings.Builder.PreProcessModulesSettings.Count > 0))
            {
                Output.WriteLine("  <h3>Pre-Process Modules</h3>");
                Output.WriteLine("  <table class=\"sbkSeav_BuilderModulesTable\">");
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
                Output.WriteLine("  <table class=\"sbkSeav_BuilderModulesTable\">");
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
                Output.WriteLine("  <table class=\"sbkSeav_BuilderModulesTable\">");
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


            // Add all the PRE PROCESS MODULE settings
            if ((UI_ApplicationCache_Gateway.Settings.Builder.PostProcessModulesSettings != null) && (UI_ApplicationCache_Gateway.Settings.Builder.PostProcessModulesSettings.Count > 0))
            {
                Output.WriteLine("  <h3>Pre-Process Modules</h3>");
                Output.WriteLine("  <table class=\"sbkSeav_BuilderModulesTable\">");
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

        private void add_metadata_file_readers_info(TextWriter Output)
        {
            Output.WriteLine("  <h2>Metadata File Reader and Writers</h2>");
            Output.WriteLine("  <p>This is the complete list of different metadata readers and writers available within the system.</p>");

            Output.WriteLine("  <table class=\"sbkSeav_MetadataReadersTable\">");
            Output.WriteLine("    <tr>");
            Output.WriteLine("      <th class=\"sbkSeav_MetadataReadersTable_TypeCol\">Type</th>");
            Output.WriteLine("      <th class=\"sbkSeav_MetadataReadersTable_LabelCol\">Label</th>");
            Output.WriteLine("      <th class=\"sbkSeav_MetadataReadersTable_CanReadCol\">Can Read</th>");
            Output.WriteLine("      <th class=\"sbkSeav_MetadataReadersTable_CanWriteCol\">Can Write</th>");
            Output.WriteLine("      <th class=\"sbkSeav_MetadataReadersTable_ClassCol\">Class</th>");
            Output.WriteLine("      <th class=\"sbkSeav_MetadataReadersTable_OptionsCol\">Options</th>");
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
                    Output.WriteLine("      <td><img src=\"" + Static_Resources.Checkmark2_Png + "\" alt=\"yes\" /></td>");
                else
                    Output.WriteLine("      <td><img src=\"" + Static_Resources.Checkmark_Png + "\" alt=\"no\" /></td>");

                // Add checkmark for can write
                if (metadataReader.canWrite)
                    Output.WriteLine("      <td><img src=\"" + Static_Resources.Checkmark2_Png + "\" alt=\"yes\" /></td>");
                else
                    Output.WriteLine("      <td><img src=\"" + Static_Resources.Checkmark_Png + "\" alt=\"no\" /></td>");

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
                            Output.WriteLine("        " + optionPair.Key + " = " + optionPair.Value);
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
            Output.WriteLine("  <p>These metadata readers/writers read individual sections within the METS file that are found in either the bibliographic (dmdSec) or adminstrative (amdSec) portions of the METS file.</p>");

            Output.WriteLine("  <table class=\"sbkSeav_MetsSectionsReadersTable\">");
            Output.WriteLine("    <tr>");
            Output.WriteLine("      <th class=\"sbkSeav_MetsSectionsReadersTable_TypeCol\">Type</th>");
            Output.WriteLine("      <th class=\"sbkSeav_MetsSectionsReadersTable_LabelCol\">Label</th>");
            Output.WriteLine("      <th class=\"sbkSeav_MetsSectionsReadersTable_CanReadCol\">Can Read</th>");
            Output.WriteLine("      <th class=\"sbkSeav_MetsSectionsReadersTable_CanWriteCol\">Can Write</th>");
            Output.WriteLine("      <th class=\"sbkSeav_MetsSectionsReadersTable_ClassCol\">Class</th>");
            Output.WriteLine("      <th class=\"sbkSeav_MetsSectionsReadersTable_OptionsCol\">Options</th>");
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
                    Output.WriteLine("      <td><img src=\"" + Static_Resources.Checkmark2_Png + "\" alt=\"yes\" /></td>");
                else
                    Output.WriteLine("      <td><img src=\"" + Static_Resources.Checkmark_Png + "\" alt=\"no\" /></td>");

                // Add checkmark for can write
                if (metadataReader.canWrite)
                    Output.WriteLine("      <td><img src=\"" + Static_Resources.Checkmark2_Png + "\" alt=\"yes\" /></td>");
                else
                    Output.WriteLine("      <td><img src=\"" + Static_Resources.Checkmark_Png + "\" alt=\"no\" /></td>");

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
                            Output.WriteLine("        " + optionPair.Key + " = " + optionPair.Value);
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

        private void add_metadata_mets_profiles_info(TextWriter Output)
        {
            Output.WriteLine("METADATA METS PROFILE INFO HERE");
        }

        private void add_metadata_modules_info(TextWriter Output)
        {
            Output.WriteLine("METADATA MODULES TO INCLUDE INFO HERE");
        }

        private void add_metadata_toplevel_info(TextWriter Output)
        {
            Output.WriteLine("METADATA TOP-LEVEL INFO HERE");
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

            Output.WriteLine("  <h3>Authentication Modes</h3>");
            Output.WriteLine("  <table class=\"sbkSeav_AuthModesTable\">");
            Output.WriteLine("    <tr>");
            Output.WriteLine("      <td>SobekCM Standard Authentication</td>");
            Output.WriteLine("      <td>ENABLED</td>");
            Output.WriteLine("    </tr>");
            Output.WriteLine("    <tr>");
            Output.WriteLine("      <td>Shibboleth Authentication</td>");

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
                Output.WriteLine("  <table class=\"sbkSeav_ShibDetailsTable\">");
                Output.WriteLine("    <tr>");
                Output.WriteLine("      <td>Display Label</td>");
                Output.WriteLine("      <td>" + shibConfig.Label + "</td>");
                Output.WriteLine("    </tr>");
                Output.WriteLine("    <tr>");
                Output.WriteLine("      <td>URL</td>");
                Output.WriteLine("      <td>" + shibConfig.ShibbolethURL + "</td>");
                Output.WriteLine("    </tr>");
                Output.WriteLine("    <tr>");
                Output.WriteLine("      <td>User Identity Attribute</td>");
                Output.WriteLine("      <td>" + shibConfig.UserIdentityAttribute + "</td>");
                Output.WriteLine("    </tr>");
                Output.WriteLine("    <tr>");
                Output.WriteLine("      <td>Debug Mode</td>");
                Output.WriteLine("      <td>" + shibConfig.Debug.ToString().ToUpper() + "</td>");
                Output.WriteLine("    </tr>");
                Output.WriteLine("  </table>");

                // Add any attribute mappings
	            if ((shibConfig.AttributeMapping != null) && (shibConfig.AttributeMapping.Count > 0))
	            {
                    Output.WriteLine("  <p>When a user first signs on via Shibboleth, the following attributes returned from Shibboleth are mapped to certain SobekCM user attributes.</p>");
                    Output.WriteLine("  <table class=\"sbkSeav_ShibDetails2Table\">");
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
                    Output.WriteLine("  <table class=\"sbkSeav_ShibDetails3Table\">");
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
                    Output.WriteLine("  <table class=\"sbkSeav_ShibDetails3Table\">");
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

            string defaultSetName = UI_ApplicationCache_Gateway.Configuration.BriefItemMapping.DefaultSetName;
            BriefItemMapping_Set defaultSet = UI_ApplicationCache_Gateway.Configuration.BriefItemMapping.GetMappingSet(defaultSetName);
            if (defaultSet != null)
            {
                Output.WriteLine("  <h3>" + defaultSetName + " (DEFAULT SET)</h3>");
                Output.WriteLine("  <table class=\"sbkSeav_BriefItemMappingTable\">");
                Output.WriteLine("    <tr>");
                Output.WriteLine("      <th class=\"sbkSeav_BriefItemMappingTable_Enabled\">Enabled</th>");
                Output.WriteLine("      <th class=\"sbkSeav_BriefItemMappingTable_ClassCol\">Class</th>");
                Output.WriteLine("    </tr>");
                foreach (BriefItemMapping_Mapper thisModule in defaultSet.Mappings)
                {
                    Output.WriteLine("    <tr>");
                    if ( thisModule.Enabled )
                        Output.WriteLine("      <td>enabled</td>");
                    else
                        Output.WriteLine("      <td>&nbsp;</td>");


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
                    Output.WriteLine("  <table class=\"sbkSeav_BriefItemMappingTable\">");
                    Output.WriteLine("    <tr>");
                    Output.WriteLine("      <th class=\"sbkSeav_BriefItemMappingTable_EnabledCol\">Enabled</th>");
                    Output.WriteLine("      <th class=\"sbkSeav_BriefItemMappingTable_ClassCol\">Class</th>");
                    Output.WriteLine("    </tr>");
                    foreach (BriefItemMapping_Mapper thisModule in thisSet.Mappings)
                    {
                        Output.WriteLine("    <tr>");
                        if (thisModule.Enabled)
                            Output.WriteLine("      <td><img src=\"" + Static_Resources.Checkmark2_Png + "\" alt=\"yes\" /></td>");
                        else
                            Output.WriteLine("      <td><img src=\"" + Static_Resources.Checkmark_Png + "\" alt=\"no\" /></td>");


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


	        Output.WriteLine("  <table class=\"sbkSeav_ContactFormTable\">");
	        Output.WriteLine("    <tr>");
	        Output.WriteLine("      <th class=\"sbkSeav_ContactForm_NameCol\">Name</th>");
	        Output.WriteLine("      <th class=\"sbkSeav_ContactForm_PromptCol\">Prompt</th>");
            Output.WriteLine("      <th class=\"sbkSeav_ContactForm_TypeCol\">Type</th>");
            Output.WriteLine("      <th class=\"sbkSeav_ContactForm_ChoicesCol\">Choices</th>");
            Output.WriteLine("      <th class=\"sbkSeav_ContactForm_NotesCol\">Notes</th>");
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
            Output.WriteLine("  <p>This section details all of the endpoints exposed via the SobekCM engine.</p>");


            Output.WriteLine("  <h3>Engine Endpoints</h3>");
            Output.WriteLine("  <table class=\"sbkSeav_EngineServerEndpointsTable\">");
            Output.WriteLine("    <tr>");
            Output.WriteLine("      <th class=\"sbkSeav_EngineServerEndpointsTable_UrlCol\">URL</th>");
            Output.WriteLine("      <th class=\"sbkSeav_EngineServerEndpointsTable_VerbCol\">Verb</th>");
            Output.WriteLine("      <th class=\"sbkSeav_EngineServerEndpointsTable_ProtocolCol\">Protocol</th>");
            Output.WriteLine("      <th class=\"sbkSeav_EngineServerEndpointsTable_MethodCol\">Method</th>");
            Output.WriteLine("      <th class=\"sbkSeav_EngineServerEndpointsTable_ComponentCol\">Component ID</th>");
            Output.WriteLine("      <th class=\"sbkSeav_EngineServerEndpointsTable_RestrictionsCol\">IP Restrictions</th>");
            Output.WriteLine("    </tr>");

            // Step through all the roots
            foreach (Engine_Path_Endpoint rootEndpoint in UI_ApplicationCache_Gateway.Configuration.Engine.RootPaths)
            {
                recursively_write_all_endpoints(rootEndpoint, Output, rootEndpoint.Segment);
            }

            Output.WriteLine("  </table>");

            // Add the engine components lookup table
            Output.WriteLine("  <h3>Engine Components</h3>");
            Output.WriteLine("  <table class=\"sbkSeav_EngineServerComponentsTable\">");
            Output.WriteLine("    <tr>");
            Output.WriteLine("      <th class=\"sbkSeav_EngineServerComponentsTable_IdCol\">Component ID</th>");
            Output.WriteLine("      <th class=\"sbkSeav_EngineServerComponentsTable_ClassCol\">Class</th>");
            Output.WriteLine("    </tr>");
            foreach (Engine_Component thisComponent in UI_ApplicationCache_Gateway.Configuration.Engine.Components)
            {
                Output.WriteLine("    <tr>");
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
            Output.WriteLine("  <table class=\"sbkSeav_EngineServerRestrictionsTable\">");
            Output.WriteLine("    <tr>");
            Output.WriteLine("      <th class=\"sbkSeav_ContactForm_IdCol\">Range ID</th>");
            Output.WriteLine("      <th class=\"sbkSeav_ContactForm_DescCol\">Range Description</th>");
            Output.WriteLine("      <th class=\"sbkSeav_ContactForm_IpCol\">IP Range</th>");
            Output.WriteLine("      <th class=\"sbkSeav_ContactForm_IpLabelCol\">IP Range Label</th>");
            Output.WriteLine("    </tr>");
            foreach (Engine_RestrictionRange thisRestrictionRange in UI_ApplicationCache_Gateway.Configuration.Engine.RestrictionRanges)
            {
                // Get the number of ips
                int ip_count = 0;
                if (thisRestrictionRange.IpRanges != null)
                    ip_count = thisRestrictionRange.IpRanges.Count;

                Output.WriteLine("    <tr>");
                Output.WriteLine("      <td rowspan=\"" + Math.Max(1, ip_count) + "\">" + thisRestrictionRange.ID + "</td>");
                Output.WriteLine("      <td rowspan=\"" + Math.Max(1, ip_count) + "\">" + thisRestrictionRange.Label + "</td>");

                if (ip_count == 0)
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

	    private void add_single_verb_mapping_in_table(Engine_VerbMapping mapping, TextWriter Output)
	    {
            // Add the request type ( i.e., GET, POST, PUT, etc.. )
            switch (mapping.RequestType)
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
            switch (mapping.Protocol)
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


            Output.WriteLine("      <td>" + mapping.Method + "</td>");
            Output.WriteLine("      <td>" + mapping.ComponentId + "</td>");
            Output.WriteLine("      <td>" + mapping.RestrictionRangeSetId + "</td>");
        }

        private void add_engine_oai_pmh_info(TextWriter Output)
        {
            OAI_PMH_Configuration config = UI_ApplicationCache_Gateway.Configuration.OAI_PMH;

            Output.WriteLine("  <h2>OAI-PMH Configuration</h2>");

            Output.WriteLine("  <h3>Basic Repository Information</h3>");
            Output.WriteLine("  <table class=\"sbkSeav_OaiPmhBasicTable\">");
            Output.WriteLine("    <tr>");
            Output.WriteLine("      <td class=\"sbkSeav_OaiPmhBasicTable_FirstCol\">OAI-PMH Enabled?</td>");
            if (config.Enabled)
                Output.WriteLine("      <td class=\"sbkSeav_OaiPmhBasicTable_SecondCol\"><img src=\"" + Static_Resources.Checkmark2_Png + "\" alt=\"yes\" /></td>");
            else
                Output.WriteLine("      <td class=\"sbkSeav_OaiPmhBasicTable_SecondCol\"><img src=\"" + Static_Resources.Checkmark_Png + "\" alt=\"no\" /></td>");
            Output.WriteLine("    </tr>");

            Output.WriteLine("    <tr><td>Repository Name:</td><td>" + config.Name + "</td></tr>");
            Output.WriteLine("    <tr><td>Repository Identifier:</td><td>" + config.Identifier + "</td></tr>");

            if ((config.Descriptions != null) && (config.Descriptions.Count > 0))
            {
                Output.WriteLine("    <tr>");
                Output.WriteLine("      <td>Additional Descriptions</td>");
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
                Output.WriteLine("      <td>Admin Email Addresses:</td>");
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
            Output.WriteLine("    <tr><td>Resource Identifier:</td><td>" + config.Identifier_Base + "</td></tr>");
            Output.WriteLine("  </table>");

                Output.WriteLine("  <h3>Metadata Prefixes</h3>");
            Output.WriteLine("  <p>This details the different metadata formats available in this repository for OAI-PMH sharing.</p>");


                // Add any constants 
            if ((config.Metadata_Prefixes != null) && (config.Metadata_Prefixes.Count > 0))
            {
                Output.WriteLine("  <table class=\"sbkSeav_OaiPmhPrefixesTable\">");
                Output.WriteLine("    <tr>");
                Output.WriteLine("      <th class=\"sbkSeav_OaiPmhPrefixesTable_EnabledCol\">Enabled</th>");
                Output.WriteLine("      <th class=\"sbkSeav_OaiPmhPrefixesTable_PrefixCol\">Prefix</th>");
                Output.WriteLine("      <th class=\"sbkSeav_OaiPmhPrefixesTable_SchemaCol\">Schema</th>");
                Output.WriteLine("      <th class=\"sbkSeav_OaiPmhPrefixesTable_NamespaceCol\">Metadata Namespace</th>");
                Output.WriteLine("      <th class=\"sbkSeav_OaiPmhPrefixesTable_ClassCol\">Class</th>");
                Output.WriteLine("    </tr>");

                foreach (OAI_PMH_Metadata_Format thisMapping in config.Metadata_Prefixes)
                {
                    Output.WriteLine("    <tr>");

                    // Enabled flag
                    if (thisMapping.Enabled)
                        Output.WriteLine("      <td><img src=\"" + Static_Resources.Checkmark2_Png + "\" alt=\"yes\" /></td>");
                    else
                        Output.WriteLine("      <td><img src=\"" + Static_Resources.Checkmark_Png + "\" alt=\"no\" /></td>");

                    // Prefix
                    Output.WriteLine("      <td>" + thisMapping.Prefix + "</td>");

                    // Schema
                    Output.WriteLine("      <td>" + thisMapping.Schema + "</td>");

                    // Namespace
                    Output.WriteLine("      <td>" + thisMapping.MetadataNamespace + "</td>");

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
	        Output.WriteLine("ENGINE QC TOOL INFO HERE");
	    }

        private void add_engine_toplevel_info(TextWriter Output)
        {
            Output.WriteLine("ENGINE TOP-LEVEL INFO HERE");
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
            Output.WriteLine("UI CITATION INFO HERE");
        }

        private void add_ui_map_editor_info(TextWriter Output)
        {
            Output.WriteLine("UI MAP EDITOR INFO HERE");
        }

        private void add_ui_client_endpoints_info(TextWriter Output)
        {
            Output.WriteLine("UI MICROSERVICE CLIENT ENDPOINTS INFO HERE");
        }

        private void add_ui_template_elements_info(TextWriter Output)
        {
            Output.WriteLine("UI TEMPLATE ELEMENTS INFO HERE");
        }

        private void add_ui_viewers_info(TextWriter Output)
        {
            Output.WriteLine("UI VIEWER INFO HERE");
        }

        private void add_ui_toplevel_info(TextWriter Output)
        {
            Output.WriteLine("UI TOP-LEVEL INFO HERE");
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
            Output.WriteLine("  <h2>No Results HTML Snippet</h2>");

            // Add the buttons
            add_buttons(Output);

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
        }

        private void add_html_no_results_info(TextWriter Output)
        {
            Output.WriteLine("  <h2>No Results HTML Snippet</h2>");

            // Add the buttons
            add_buttons(Output);

            Output.WriteLine();

            string noResultsSnippet = No_Results_ResultsViewer.Get_NoResults_Text();
            Output.WriteLine("  <textarea id=\"sbkSeav_HtmlEdit\" name=\"sbkSeav_NoResultsHtmlEdit\" style=\"width:800px; height:400px\" >");
            Output.WriteLine(noResultsSnippet.Replace("<%", "[%").Replace("%>", "%]"));
            Output.WriteLine("  </textarea>");

            Output.WriteLine("<br />");
            Output.WriteLine();

            // Add final buttons
            add_buttons(Output);
        }

        private void add_html_toplevel_info(TextWriter Output)
        {
            Output.WriteLine("HTML TOP-LEVEL INFO HERE");
        }

		#endregion


		#region HTML helper methods for the extensions main page and subpages

		private void add_extensions_info(TextWriter Output)
		{
			Output.WriteLine("EXTENSIONS INFO HERE");
		}

		#endregion


		/// <summary> Gets the CSS class of the container that the page is wrapped within </summary>
		/// <value> Returns 'sbkAsav_ContainerInner' </value>
		public override string Container_CssClass { get { return "sbkSeav_ContainerInner"; } }

	}
}