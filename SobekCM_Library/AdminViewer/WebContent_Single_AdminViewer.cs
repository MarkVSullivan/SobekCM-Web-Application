using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SobekCM.Core.Aggregations;
using SobekCM.Core.Client;
using SobekCM.Core.Configuration;
using SobekCM.Core.Navigation;
using SobekCM.Core.WebContent;
using SobekCM.Library.Database;
using SobekCM.Library.Settings;
using SobekCM.Library.UI;
using SobekCM.Library.UploadiFive;
using SobekCM.Tools;

namespace SobekCM.Library.AdminViewer
{
    /// <summary> Class allows an administrator to manage all the information about a single web content page or redirect </summary>
    /// <remarks> This class extends the <see cref="abstract_AdminViewer"/> class. </remarks>
    public class WebContent_Single_AdminViewer: abstract_AdminViewer
    {
		private string actionMessage;
      //  private readonly Complete_Item_Aggregation itemAggregation;


        private readonly HTML_Based_Content webContent;
        private readonly string webContentDirectory;
        private readonly int webContentId;

		private readonly int page;

		private string childPageCode;
		private string childPageLabel;
		private string childPageVisibility;
		private string childPageParent;


        /// <summary> Constructor for a new instance of the WebContent_Single_AdminViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
		/// <remarks> Postback from handling an edit or new aggregation is handled here in the constructor </remarks>
        public WebContent_Single_AdminViewer(RequestCache RequestSpecificValues) : base(RequestSpecificValues)
		{
            RequestSpecificValues.Tracer.Add_Trace("WebContent_Single_AdminViewer.Constructor", String.Empty);


            // If not logged in, send to main home page
            if ((RequestSpecificValues.Current_User == null) || (!RequestSpecificValues.Current_User.LoggedOn))
            {
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Aggregation;
                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                return;
            }

            // If no web content id was provided, send back
            if ( !RequestSpecificValues.Current_Mode.WebContentID.HasValue )
            {
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.My_Sobek;
                RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                return;
            }

			// Set some defaults and get the web content id
			actionMessage = String.Empty;
            webContentId = RequestSpecificValues.Current_Mode.WebContentID.Value;

			// If the RequestSpecificValues.Current_User cannot edit this, go back
            if ((!RequestSpecificValues.Current_User.Is_Portal_Admin) && (!RequestSpecificValues.Current_User.Is_System_Admin) && (!RequestSpecificValues.Current_User.Is_Host_Admin))
			{
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.My_Sobek;
                RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
				return;
			}

			// Load the item aggregation, either currenlty from the session (if already editing this aggregation )
			// or by reading all the appropriate XML and reading data from the database
			object possibleEditAggregation = HttpContext.Current.Session["Edit_WebContent|" + webContentId];
            HTML_Based_Content cachedInstance = possibleEditAggregation as HTML_Based_Content;
		    if (cachedInstance != null)
		    {
                webContent = cachedInstance;
		    }
		    else
		    {
                webContent = SobekEngineClient.WebContent.Get_HTML_Based_Content(webContentId, false, RequestSpecificValues.Tracer);
		    }

			// If unable to retrieve this aggregation, send to home
            if (webContent == null)
			{
				RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
				UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
				return;
			}

			// Get the aggregation directory and ensure it exists
            webContentDirectory = Path.GetDirectoryName(webContent.Source);
            if (!Directory.Exists(webContentDirectory))
                Directory.CreateDirectory(webContentDirectory);

			// Determine the page
			page = 1;
		    if (!String.IsNullOrEmpty(RequestSpecificValues.Current_Mode.My_Sobek_SubMode))
		    {
                //if (RequestSpecificValues.Current_Mode.My_Sobek_SubMode == "b")   RESERVED FOR LANGUAGE SUPPORT
                //    page = 2;
                //else if (RequestSpecificValues.Current_Mode.My_Sobek_SubMode == "c")  RESERVED FOR SITEMAP SUPPORT
                //    page = 3; 
                if (RequestSpecificValues.Current_Mode.My_Sobek_SubMode == "d")
                    page = 4;
                else if (RequestSpecificValues.Current_Mode.My_Sobek_SubMode == "e")
                    page = 5;
		    }


		    // If this is a postback, handle any events first
			if (RequestSpecificValues.Current_Mode.isPostBack)
			{
				try
				{
					// Pull the standard values
					NameValueCollection form = HttpContext.Current.Request.Form;

					// Get the curret action
					string action = form["admin_webcontent_save"];

					// If no action, then we should return to the current tab page
					if (action.Length == 0)
						action = RequestSpecificValues.Current_Mode.My_Sobek_SubMode;

					// If this is to cancel, handle that here; no need to handle post-back from the
					// editing form page first
					if (action == "z")
					{
						// Clear the aggregation from the sessions
                        HttpContext.Current.Session["Edit_WebContent|" + webContentId] = null;

						// Redirect the RequestSpecificValues.Current_User
						RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Aggregation;
						RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Home;
						UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
						return;
					}

					// Save the returned values, depending on the page
					switch (page)
					{
						case 1:
							Save_Page_General_Postback(form);
							break;

                        //case 2: RESERVED FOR LANGUAGE SUPPORT
                        //    Save_Page_2_Postback(form);
                        //    break;

                        //case 3: RESERVED FOR SITEMAP SUPPORT
                        //    Save_Page_3_Postback(form);
                        //    break;

						case 4:
							Save_Child_Pages_Postback(form);
							break;

						case 5:
							Save_Uploads_Postback(form);
							break;
					}

					// Should this be saved to the database?
                    if ((action == "save") || (action == "save_exit") || (action == "save_wizard"))
                    {
                        actionMessage = "Saving temporarily disabled";

                        //// Get the current aggrgeation information, for comparison
                        //Complete_Item_Aggregation currentAggregation = SobekEngineClient.Aggregations.Get_Complete_Aggregation(code, true, RequestSpecificValues.Tracer);

                        //// Backup the old aggregation info
                        //string backup_folder = UI_ApplicationCache_Gateway.Settings.Base_Design_Location + itemAggregation.ObjDirectory.Replace("/","\\") + "backup\\configs";
                        //if (!Directory.Exists(backup_folder))
                        //    Directory.CreateDirectory(backup_folder);
                        //string current_config = UI_ApplicationCache_Gateway.Settings.Base_Design_Location + itemAggregation.ObjDirectory + "\\" + itemAggregation.Code + ".xml";
                        //if (File.Exists(current_config))
                        //{
                        //    // Use the last modified date as the name of the backup
                        //    DateTime lastModifiedDate = (new FileInfo(current_config)).LastWriteTime;
                        //    string backup_name = itemAggregation.Code + lastModifiedDate.Year + lastModifiedDate.Month.ToString().PadLeft(2, '0') + lastModifiedDate.Day.ToString().PadLeft(2, '0') + lastModifiedDate.Hour.ToString().PadLeft(2, '0') + lastModifiedDate.Minute.ToString().PadLeft(2, '0') + ".xml";
                        //    if (!File.Exists(backup_folder + "\\" + backup_name))
                        //        File.Copy(current_config, backup_folder + "\\" + backup_name, false );
                        //}

                        //// Save the new configuration file
                        //string save_error = String.Empty;
                        //bool successful_save = true;
                        //if (!itemAggregation.Write_Configuration_File(UI_ApplicationCache_Gateway.Settings.Base_Design_Location + itemAggregation.ObjDirectory))
                        //{
                        //    successful_save = false;
                        //    save_error = "<br /><br />Error saving the configuration file";
                        //}

                        //// Save to the database
                        //if (!Item_Aggregation_Utilities.Save_To_Database(itemAggregation, RequestSpecificValues.Current_User.Full_Name, null))
                        //{
                        //    successful_save = false;
                        //    save_error = "<br /><br />Error saving to the database.";

                        //    if (Engine_Database.Last_Exception != null)
                        //    {
                        //        save_error = save_error + "<br /><br />" + Engine_Database.Last_Exception.Message;
                        //    }
                        //}

                        //// Save the link between this item and the thematic heading
                        //int thematicHeadingId = -1;
                        //if (itemAggregation.Thematic_Heading != null)
                        //    thematicHeadingId = itemAggregation.Thematic_Heading.ID;
                        //UI_ApplicationCache_Gateway.Aggregations.Set_Aggregation_Thematic_Heading(itemAggregation.Code, thematicHeadingId);


                        //// Clear the aggregation from the cache
                        //CachedDataManager.Aggregations.Remove_Item_Aggregation(itemAggregation.Code, null);
                        //CachedDataManager.Aggregations.Clear_Aggregation_Hierarchy();
                        //Engine_ApplicationCache_Gateway.RefreshCodes();
                        //Engine_ApplicationCache_Gateway.RefreshThematicHeadings();



                        //// Forward back to the aggregation home page, if this was successful
                        //if (successful_save)
                        //{
                        //    // Also, update the information that was changed
                        //    try
                        //    {
                        //        List<string> changes = Complete_Item_Aggregation_Comparer.Compare(currentAggregation, itemAggregation);
                        //        if ((changes != null) && (changes.Count > 0))
                        //        {
                        //            StringBuilder builder = new StringBuilder(changes[0]);
                        //            for (int i = 1; i < changes.Count; i++)
                        //            {
                        //                builder.Append("\n" + changes[i]);
                        //            }
                        //            SobekCM_Database.Save_Item_Aggregation_Milestone(itemAggregation.Code, builder.ToString(), RequestSpecificValues.Current_User.Full_Name);

                        //        }
                        //        else
                        //        {
                        //            SobekCM_Database.Save_Item_Aggregation_Milestone(itemAggregation.Code, "Configuration edited", RequestSpecificValues.Current_User.Full_Name);
                        //        }
                        //    }
                        //    catch
                        //    {
                        //        SobekCM_Database.Save_Item_Aggregation_Milestone(itemAggregation.Code, "Configuration edited", RequestSpecificValues.Current_User.Full_Name);
                        //    }


                        //    // Clear the aggregation from the sessions
                        //    HttpContext.Current.Session["Edit_Aggregation_" + itemAggregation.Code] = null;
                        //    HttpContext.Current.Session["Item_Aggr_Edit_" + itemAggregation.Code + "_NewLanguages"] = null;

                        //    // Redirect the RequestSpecificValues.Current_User
                        //    if (action == "save_exit")
                        //    {
                        //        RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Aggregation;
                        //        RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Home;
                        //        UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                        //    }
                        //    else if (action == "save_wizard")
                        //    {

                        //        RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Add_Collection_Wizard;
                        //        string wizard_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
                        //        RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Aggregations_Mgmt;

                        //        if (wizard_url.IndexOf("?") < 0)
                        //            wizard_url = wizard_url + "?parent=" + itemAggregation.Code;
                        //        else
                        //            wizard_url = wizard_url + "&parent=" + itemAggregation.Code;

                        //        RequestSpecificValues.Current_Mode.Request_Completed = true;
                        //        HttpContext.Current.Response.Redirect(wizard_url, false);
                        //        HttpContext.Current.ApplicationInstance.CompleteRequest();

                        //    }
                        //    else
                        //    {
                        //        UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                        //    }
                        //}
                        //else
                        //{
                        //    actionMessage = "Error saving aggregation information!" + save_error;
                        //}
                    }
					else 
					{
						// In some cases, skip this part
						if (((page == 8) && (action == "h")) || ((page == 7) && (action == "g")))
							return;

						// Save to the admins session
						HttpContext.Current.Session["Edit_WebContent|" + webContentId] = webContent;
						RequestSpecificValues.Current_Mode.My_Sobek_SubMode = action;
						HttpContext.Current.Response.Redirect(UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode), false);
						HttpContext.Current.ApplicationInstance.CompleteRequest();
						RequestSpecificValues.Current_Mode.Request_Completed = true;
					}
				}
				catch
				{
					actionMessage = "Unable to correctly parse postback data.";
				}
			}
		}

		/// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
		/// <value> This always returns the value 'HTML Skins' </value>
		public override string Web_Title
		{
			get { return "Administer Web Content Page"; }
		}

        /// <summary> Gets the URL for the icon related to this administrative task </summary>
        public override string Viewer_Icon
        {
            get { return String.Empty; }
        }

		/// <summary> Add the HTML to be displayed in the main SobekCM viewer area (outside of the forms)</summary>
		/// <param name="Output"> Textwriter to write the HTML for this viewer</param>
		/// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <remarks> This class does nothing, since the interface list is added as controls, not HTML </remarks>
		public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
		{
			Tracer.Add_Trace("WebContent_Single_AdminViewer.Write_HTML", "Do nothing");
		}

		/// <summary> This is an opportunity to write HTML directly into the main form before any controls are placed in the main place holder </summary>
		/// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <remarks> This text will appear within the ItemNavForm form tags </remarks>
		public override void Write_ItemNavForm_Opening(TextWriter Output, Custom_Tracer Tracer)
		{
			Tracer.Add_Trace("WebContent_Single_AdminViewer.Write_ItemNavForm_Opening", "Add the majority of the HTML before the placeholder");

			// Add the hidden field
			Output.WriteLine("<!-- Hidden field is used for postbacks to indicate what to save and reset -->");
			Output.WriteLine("<input type=\"hidden\" id=\"admin_webcontent_reset\" name=\"admin_webcontent_reset\" value=\"\" />");
			Output.WriteLine("<input type=\"hidden\" id=\"admin_webcontent_save\" name=\"admin_webcontent_save\" value=\"\" />");
			Output.WriteLine("<input type=\"hidden\" id=\"admin_webcontent_action\" name=\"admin_webcontent_action\" value=\"\" />");
			Output.WriteLine(); 

			Tracer.Add_Trace("WebContent_Single_AdminViewer.Write_ItemNavForm_Closing", "Add the rest of the form");

			Output.WriteLine("<!-- Users_AdminViewer.Write_ItemNavForm_Closing -->");

			Output.WriteLine("<script src=\"" + Static_Resources.Sobekcm_Admin_Js + "\" type=\"text/javascript\"></script>");
			Output.WriteLine();

			Output.WriteLine("<div id=\"sbkSaav_PageContainer\">");

			// Add the buttons (unless this is a sub-page like editing the CSS file)
			if (page < 10)
			{
				string last_mode = RequestSpecificValues.Current_Mode.My_Sobek_SubMode;
				RequestSpecificValues.Current_Mode.My_Sobek_SubMode = String.Empty;
				Output.WriteLine("  <div class=\"sbkSaav_ButtonsDiv\">");
				Output.WriteLine("    <button title=\"Do not apply changes\" class=\"sbkAdm_RoundButton\" onclick=\"return new_webcontent_edit_page('z');\"><img src=\"" + Static_Resources.Button_Previous_Arrow_Png + "\" class=\"sbkAdm_RoundButton_LeftImg\" alt=\"\" /> CANCEL</button> &nbsp; &nbsp; ");
                Output.WriteLine("    <button title=\"Save changes to this web content page or redirect\" class=\"sbkAdm_RoundButton\" onclick=\"return save_webcontent_edits(false);\"> SAVE </button> &nbsp; &nbsp; ");
                Output.WriteLine("    <button title=\"Save changes to this web content page or redirect and exit the admin screens\" class=\"sbkAdm_RoundButton\" onclick=\"return save_webcontent_edits(true);\">SAVE & EXIT <img src=\"" + Static_Resources.Button_Next_Arrow_Png + "\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button>");
				Output.WriteLine("  </div>");
				Output.WriteLine();
				RequestSpecificValues.Current_Mode.My_Sobek_SubMode = last_mode;
			}


            Output.WriteLine("  <div class=\"sbkAdm_TitleDiv\" style=\"padding-left:20px\">");
            Output.WriteLine("    <img id=\"sbkAdm_TitleDivImg\" src=\"" + Static_Resources.Admin_View_Img + "\" alt=\"\" />");
            Output.WriteLine("    <h1>Web Content Administration : " + webContent.Title + "</h1>");
            Output.WriteLine("  </div>");
            Output.WriteLine();

			// Start the outer tab containe
			Output.WriteLine("  <div id=\"tabContainer\" class=\"fulltabs\">");

			// Add all the possible tabs (unless this is a sub-page like editing the CSS file)
			if (page < 12)
			{
				Output.WriteLine("    <div class=\"tabs\">");
				Output.WriteLine("      <ul>");

				
				// Draw all the page tabs for this form
                const string GENERAL = "General";
				if (page == 1)
				{
					Output.WriteLine("    <li id=\"tabHeader_1\" class=\"tabActiveHeader\">" + GENERAL + "</li>");
				}
				else
				{
					Output.WriteLine("    <li id=\"tabHeader_1\" onclick=\"return new_webcontent_edit_page('a');\">" + GENERAL + "</li>");
				}


                //const string LOCALIZATION = "Localization";
                //if (page == 2)
                //{
                //    Output.WriteLine("    <li id=\"tabHeader_2\" class=\"tabActiveHeader\">" + LOCALIZATION + "</li>");
                //}
                //else
                //{
                //    Output.WriteLine("    <li id=\"tabHeader_2\" onclick=\"return new_webcontent_edit_page('b');\">" + LOCALIZATION + "</li>");
                //}

                //const string SITEMAP = "Sitemap";
                //if (page == 3)
                //{
                //    Output.WriteLine("    <li id=\"tabHeader_2\" class=\"tabActiveHeader\">" + SITEMAP + "</li>");
                //}
                //else
                //{
                //    Output.WriteLine("    <li id=\"tabHeader_2\" onclick=\"return new_webcontent_edit_page('c');\">" + SITEMAP + "</li>");
                //}

                const string CHILD_PAGES = "Child Pages";
                if (page == 4)
                {
                    Output.WriteLine("    <li id=\"tabHeader_3\" class=\"tabActiveHeader\">" + CHILD_PAGES + "</li>");
                }
                else
                {
                    Output.WriteLine("    <li id=\"tabHeader_3\" onclick=\"return new_webcontent_edit_page('d');\">" + CHILD_PAGES + "</li>");
                }

                const string UPLOADS = "Uploads";
                if (page == 5)
                {
                    Output.WriteLine("    <li id=\"tabHeader_4\" class=\"tabActiveHeader\">" + UPLOADS + "</li>");
                }
                else
                {
                    Output.WriteLine("    <li id=\"tabHeader_4\" onclick=\"return new_webcontent_edit_page('e');\">" + UPLOADS + "</li>");
                }


			    Output.WriteLine("      </ul>");
				Output.WriteLine("    </div>");
			}

			// Add the single tab.  When users click on a tab, it goes back to the server (here)
			// to render the correct tab content
			Output.WriteLine("    <div class=\"tabscontent\">");
			Output.WriteLine("    	<div class=\"tabpage\" id=\"tabpage_1\">");


			switch (page)
			{
				case 1:
					Add_Page_General(Output );
					break;

                //case 2:
                //    Add_Page_Localization(Output);  RESERVED FOR LANGUAGE SUPPORT
                //    break;

                //case 3:
                //    Add_Page_SiteMap(Output);   RESERVED FOR SITEMAP SUPPORT
                //    break;

				case 4:
					Add_Page_Child_Pages(Output);
					break;

				case 5:
                    Add_Page_Uploads(Output);
                    break;
			}
		}

		/// <summary> This is an opportunity to write HTML directly into the main form, without
		/// using the pop-up html form architecture </summary>
		/// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <remarks> This text will appear within the ItemNavForm form tags </remarks>
		public override void Write_ItemNavForm_Closing(TextWriter Output, Custom_Tracer Tracer)
		{
			Tracer.Add_Trace("WebContent_Single_AdminViewer.Write_ItemNavForm_Closing", "Add any html after the placeholder and close tabs");

			switch (page)
			{
				case 5:
                    Finish_Page_Uploads(Output);
                    break;
			}


			 Output.WriteLine("    </div>");
			 Output.WriteLine("  </div>");
			 Output.WriteLine("</div>");
			 Output.WriteLine("<br />");
		}

		#region Methods to render (and parse) page 1 - General Information

		private void Save_Page_General_Postback(NameValueCollection Form)
		{
            if (Form["admin_webcontent_title"] != null) webContent.Title = Form["admin_webcontent_title"];
            if (Form["admin_webcontent_author"] != null) webContent.Author = Form["admin_webcontent_author"];
            if (Form["admin_webcontent_desc"] != null) webContent.Description = Form["admin_webcontent_desc"];
            if (Form["admin_webcontent_keywords"] != null) webContent.Keywords = Form["admin_webcontent_keywords"];
            if (Form["admin_webcontent_head"] != null) webContent.Extra_Head_Info = Form["admin_webcontent_head"];
            if (Form["admin_webcontent_redirect"] != null) webContent.Redirect = Form["admin_webcontent_redirect"];
            if (Form["admin_webcontent_skin"] != null) webContent.Web_Skin = Form["admin_webcontent_skin"];
            if (Form["admin_webcontent_banner"] != null) webContent.Banner = Form["admin_webcontent_banner"];
            if (Form["admin_webcontent_sitemap"] != null) webContent.SiteMap = Form["admin_webcontent_sitemap"];
            webContent.IncludeMenu = Form["admin_webcontent_menu"] != null;
		}

		private void Add_Page_General( TextWriter Output )
		{
			// Help constants (for now)
            const string TITLE_HELP = "Help for title";
            const string AUTHOR_HELP = "Help for author";
            const string DESCRIPTION_HELP = "Help for description";
            const string KEYWORD_HELP = "Help for keywords";
            const string EXTRA_HEADER_HELP = "Help for extra head info";
            const string REDIRECT_HELP = "Help for redirect";
            const string WEBSKIN_HELP = "Help for webskin";
            const string BANNER_HELP = "Help for banner";
            const string SITEMAP_HELP = "Help for sitemap";
            const string MAINMENU_HELP = "Help for main menu";

			Output.WriteLine("<table class=\"sbkAdm_PopupTable\">");

			Output.WriteLine("  <tr class=\"sbkSaav_TitleRow\"><td colspan=\"3\">Basic Information</td></tr>");
			Output.WriteLine("  <tr class=\"sbkSaav_TextRow\"><td colspan=\"3\"><p>The information in this section is the basic information about the web content page and includes much of the metadata that is provided to search engines to increase page rank on relevant searches.</p><p>For more information about the settings on this tab, <a href=\"" + UI_ApplicationCache_Gateway.Settings.Help_URL(RequestSpecificValues.Current_Mode.Base_URL) + "adminhelp/singleaggr\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</p></td></tr>");

			// Add the URL
			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
			Output.WriteLine("    <td style=\"width:50px\">&nbsp;</td>");
			Output.WriteLine("    <td style=\"width: 145px\" class=\"sbkSaav_TableLabel\">URL:</td>");
			Output.WriteLine("    <td> " + webContent.URL( UI_ApplicationCache_Gateway.Settings.Base_URL ) + "</td>");
			Output.WriteLine("  </tr>");

			// Add the Title
			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
			Output.WriteLine("    <td>&nbsp;</td>");
            Output.WriteLine("    <td class=\"sbkSaav_TableLabel\"><label for=\"admin_webcontent_title\">Title:</label></td>");
			Output.WriteLine("    <td>");
            Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td><input class=\"sbkSaav_large_input sbkAdmin_Focusable\" name=\"admin_webcontent_title\" id=\"admin_webcontent_title\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(webContent.Title) + "\" /></td>");
            Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + TITLE_HELP + "');\"  title=\"" + TITLE_HELP + "\" /></td></tr></table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");

			// Add the Author
			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
			Output.WriteLine("    <td>&nbsp;</td>");
            Output.WriteLine("    <td class=\"sbkSaav_TableLabel\"><label for=\"admin_webcontent_author\">Author:</label></td>");
			Output.WriteLine("    <td>");
            Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td><input class=\"sbkSaav_medium_input sbkAdmin_Focusable\" name=\"admin_webcontent_author\" id=\"admin_webcontent_author\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(webContent.Author) + "\" /></td>");
            Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + AUTHOR_HELP + "');\"  title=\"" + AUTHOR_HELP + "\" /></td></tr></table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");

			// Add the Description/Summary box
			Output.WriteLine("  <tr class=\"sbkSaav_TallRow\">");
			Output.WriteLine("    <td>&nbsp;</td>");
			Output.WriteLine("    <td class=\"sbkSaav_TableLabel2\"><label for=\"admin_webcontent_desc\">Description:</label></td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <table class=\"sbkSaav_InnerTable2\"><tr style=\"vertical-align:top\"><td><textarea class=\"sbkSaav_large_textbox sbkAdmin_Focusable\" rows=\"6\" name=\"admin_webcontent_desc\" id=\"admin_webcontent_desc\">" + HttpUtility.HtmlEncode(webContent.Description) + "</textarea></td>");
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + DESCRIPTION_HELP + "');\"  title=\"" + DESCRIPTION_HELP + "\" /></td></tr></table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");

			// Add the Keywords
			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
			Output.WriteLine("    <td>&nbsp;</td>");
            Output.WriteLine("    <td class=\"sbkSaav_TableLabel\"><label for=\"admin_webcontent_keywords\">Keywords:</label></td>");
			Output.WriteLine("    <td>");
            Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td><input class=\"sbkSaav_large_input sbkAdmin_Focusable\" name=\"admin_webcontent_keywords\" id=\"admin_webcontent_keywords\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(webContent.Keywords) + "\" /></td>");
            Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + KEYWORD_HELP + "');\"  title=\"" + KEYWORD_HELP + "\" /></td></tr></table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");

            // Add the place to just add extra lines into the header, as needed
            Output.WriteLine("  <tr class=\"sbkSaav_TallRow\">");
            Output.WriteLine("    <td>&nbsp;</td>");
            Output.WriteLine("    <td class=\"sbkSaav_TableLabel2\"><label for=\"admin_webcontent_head\">Extra Head Content:</label></td>");
            Output.WriteLine("    <td>");
            Output.WriteLine("      <table class=\"sbkSaav_InnerTable2\"><tr style=\"vertical-align:top\"><td><textarea class=\"sbkSaav_large_textbox sbkAdmin_Focusable\" rows=\"6\" name=\"admin_webcontent_head\" id=\"admin_webcontent_head\">" + HttpUtility.HtmlEncode(webContent.Extra_Head_Info) + "</textarea></td>");
            Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + EXTRA_HEADER_HELP + "');\"  title=\"" + EXTRA_HEADER_HELP + "\" /></td></tr></table>");
            Output.WriteLine("     </td>");
            Output.WriteLine("  </tr>");

            // Add the redirect URL
            Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
            Output.WriteLine("    <td>&nbsp;</td>");
            Output.WriteLine("    <td class=\"sbkSaav_TableLabel\"><label for=\"admin_webcontent_redirect\">Redirect URL:</label></td>");
            Output.WriteLine("    <td>");
            Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td><input class=\"sbkSaav_large_input sbkAdmin_Focusable\" name=\"admin_webcontent_redirect\" id=\"admin_webcontent_redirect\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(webContent.Redirect) + "\" /></td>");
            Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + REDIRECT_HELP + "');\"  title=\"" + REDIRECT_HELP + "\" /></td></tr></table>");
            Output.WriteLine("     </td>");
            Output.WriteLine("  </tr>");



			Output.WriteLine("  <tr class=\"sbkSaav_TitleRow2\"><td colspan=\"3\">Appearance</td></tr>");
			Output.WriteLine("  <tr class=\"sbkSaav_TextRow\"><td colspan=\"3\"><p>The values in this section determine how this web content page appears to users by allowing a banner to be selected and a web skin to be selected.</p></td></tr>");


			// Add the web skin
			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
			Output.WriteLine("    <td>&nbsp;</td>");
			Output.WriteLine("    <td class=\"sbkSaav_TableLabel\">Web Skin:</label></td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td>");

            // Start the select box
            Output.Write("          <select class=\"sbkSaav_SelectSkin\" name=\"admin_webcontent_skin\" id=\"admin_webcontent_skin\">");

            // Add the NONE option first
            Output.Write(webContent.Web_Skin.Length == 0 ? "<option value=\"\" selected=\"selected\" ></option>" : "<option value=\"\"></option>");

            // Get the ordered list of all skin codes
            List<string> skinCodes = UI_ApplicationCache_Gateway.Web_Skin_Collection.Ordered_Skin_Codes;

            // Add each web skin code to the select box
            foreach (string skinCode in skinCodes)
            {
                if (String.Compare(webContent.Web_Skin, skinCode, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    Output.Write("<option value=\"" + skinCode + "\" selected=\"selected\" >" + HttpUtility.HtmlEncode(skinCode) + "</option>");
                }
                else
                {
                    Output.Write("<option value=\"" + skinCode + "\">" + HttpUtility.HtmlEncode(skinCode) + "</option>");
                }
            }
            Output.WriteLine("</select>");

			Output.WriteLine("        </td>");
            Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + WEBSKIN_HELP + "');\"  title=\"" + WEBSKIN_HELP + "\" /></td></tr></table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");


            // Add the banner
            Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
            Output.WriteLine("    <td>&nbsp;</td>");
            Output.WriteLine("    <td class=\"sbkSaav_TableLabel\"><label for=\"admin_webcontent_banner\">Banner:</label></td>");
            Output.WriteLine("    <td>");
            Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td><input class=\"sbkSaav_large_input sbkAdmin_Focusable\" name=\"admin_webcontent_banner\" id=\"admin_webcontent_banner\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(webContent.Banner) + "\" /></td>");
            Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + BANNER_HELP + "');\"  title=\"" + BANNER_HELP + "\" /></td></tr></table>");
            Output.WriteLine("     </td>");
            Output.WriteLine("  </tr>");



            Output.WriteLine("  <tr class=\"sbkSaav_TitleRow2\"><td colspan=\"3\">Site Map</td></tr>");
            Output.WriteLine("  <tr class=\"sbkSaav_TextRow\"><td colspan=\"3\"><p>This item can be displayed within a larger collection of pages called a site map.</p></td></tr>");


            // Add the site map
            Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
            Output.WriteLine("    <td>&nbsp;</td>");
            Output.WriteLine("    <td class=\"sbkSaav_TableLabel\"><label for=\"admin_webcontent_sitemap\">SiteMap:</label></td>");
            Output.WriteLine("    <td>");
            Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td><input class=\"sbkSaav_large_input sbkAdmin_Focusable\" name=\"admin_webcontent_sitemap\" id=\"admin_webcontent_sitemap\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(webContent.Banner) + "\" /></td>");
            Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + SITEMAP_HELP + "');\"  title=\"" + SITEMAP_HELP + "\" /></td></tr></table>");
            Output.WriteLine("     </td>");
            Output.WriteLine("  </tr>");


            // Add the Include Menu behavior
            Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
            Output.WriteLine("    <td>&nbsp;</td>");
            Output.WriteLine("    <td class=\"sbkSaav_TableLabel\"><label for=\"admin_webcontent_email\">SiteMap Menu:</label></td>");
            Output.WriteLine("    <td>");
		    Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td>");
            Output.WriteLine((webContent.IncludeMenu.HasValue && webContent.IncludeMenu.Value )
                    ? "        <input class=\"sbkSaav_checkbox\" type=\"checkbox\" name=\"admin_webcontent_menu\" id=\"admin_webcontent_menu\" checked=\"checked\" /> <label for=\"admin_webcontent_menu\">Include sitemap main menu</label> "
                    : "        <input class=\"sbkSaav_checkbox\" type=\"checkbox\" name=\"admin_webcontent_menu\" id=\"admin_webcontent_menu\" /> <label for=\"admin_webcontent_menu\">Include sitemap main menu</label> ");
            Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + MAINMENU_HELP + "');\"  title=\"" + MAINMENU_HELP + "\" /></td></tr></table>");
            Output.WriteLine("     </td>");
            Output.WriteLine("  </tr>");
            



			Output.WriteLine("</table>");
			Output.WriteLine("<br />");
		}

		#endregion

		#region Methods to render (and parse) page 4 - Child pages

        private void Save_Child_Pages_Postback(NameValueCollection Form)
		{
            //string action = Form["admin_webcontent_action"];
            //if (!String.IsNullOrEmpty(action))
            //{

            //    if (action == "save_childpage")
            //    {
            //        childPageCode = Form["admin_webcontent_code"];
            //        childPageLabel = Form["admin_webcontent_label"];
            //        childPageVisibility = Form["admin_webcontent_visibility"];
            //        childPageParent = Form["admin_webcontent_parent"];

            //        // Convert to the integer id for the parent and begin to do checking
            //        List<string> errors = new List<string>();

            //        // Validate the code
            //        if (childPageCode.Length > 20)
            //        {
            //            errors.Add("New child page code must be twenty characters long or less");
            //        }
            //        else if (childPageCode.Length == 0)
            //        {
            //            errors.Add("You must enter a CODE for this child page");

            //        }
            //        else if (UI_ApplicationCache_Gateway.Settings.Reserved_Keywords.Contains(childPageCode.ToLower()))
            //        {
            //            errors.Add("That code is a system-reserved keyword.  Try a different code.");
            //        }
            //        else if (itemAggregation.Child_Page_By_Code(childPageCode.ToUpper()) != null)
            //        {
            //            errors.Add("New code must be unique... <i>" + childPageCode + "</i> already exists");
            //        }


            //        if (childPageLabel.Trim().Length == 0)
            //            errors.Add("You must enter a LABEL for this child page");
            //        if (childPageVisibility.Trim().Length == 0)
            //            errors.Add("You must select a VISIBILITY for this child page");

            //        if (errors.Count > 0)
            //        {
            //            // Create the error message
            //            actionMessage = "ERROR: Invalid entry for new item child page<br />";
            //            foreach (string error in errors)
            //                actionMessage = actionMessage + "<br />" + error;
            //        }
            //        else
            //        {
            //            Complete_Item_Aggregation_Child_Page newPage = new Complete_Item_Aggregation_Child_Page { Code = childPageCode, Parent_Code = childPageParent, Source_Data_Type = Item_Aggregation_Child_Source_Data_Enum.Static_HTML };
            //            newPage.Add_Label(childPageLabel, UI_ApplicationCache_Gateway.Settings.Default_UI_Language);
            //            switch (childPageVisibility)
            //            {
            //                case "none":
            //                    newPage.Browse_Type = Item_Aggregation_Child_Visibility_Enum.None;
            //                    newPage.Parent_Code = String.Empty;
            //                    break;

            //                case "browse":
            //                    newPage.Browse_Type = Item_Aggregation_Child_Visibility_Enum.Main_Menu;
            //                    break;

            //                case "browseby":
            //                    newPage.Browse_Type = Item_Aggregation_Child_Visibility_Enum.Metadata_Browse_By;
            //                    newPage.Parent_Code = String.Empty;
            //                    break;
            //            }
            //            string html_source_dir =  "\\html\\browse";
            //            if (!Directory.Exists(html_source_dir))
            //                Directory.CreateDirectory(html_source_dir);
            //            string html_source_file = html_source_dir + "\\" + childPageCode + "_" + Web_Language_Enum_Converter.Enum_To_Code(UI_ApplicationCache_Gateway.Settings.Default_UI_Language) + ".html";
            //            if (!File.Exists(html_source_file))
            //            {
            //                HTML_Based_Content htmlContent = new HTML_Based_Content
            //                {
            //                    Content = "<br /><br />This is a new browse page.<br /><br />" + childPageLabel + "<br /><br />The code for this browse is: " + childPageCode, 
            //                    Author = RequestSpecificValues.Current_User.Full_Name, 
            //                    Date = DateTime.Now.ToLongDateString(), 
            //                    Title = childPageLabel
            //                };
            //                htmlContent.Save_To_File(html_source_file);
            //            }
            //            newPage.Add_Static_HTML_Source("html\\browse\\" + childPageCode + "_" + Web_Language_Enum_Converter.Enum_To_Code(UI_ApplicationCache_Gateway.Settings.Default_UI_Language) + ".html", UI_ApplicationCache_Gateway.Settings.Default_UI_Language);

            //            itemAggregation.Add_Child_Page(newPage);

            //            // Save to the admins session
            //            HttpContext.Current.Session["Edit_Aggregation_" + itemAggregation.Code] = itemAggregation;

            //        }
            //    }
            //}
		}

        private void Add_Page_Child_Pages(TextWriter Output)
		{
			const string CODE_HELP = "Enter the code for the new child page.  This code should be less than 20 characters and be as descriptive of the content of your new page as possible.  This code will appear in the URL for the new child page.";
			const string LABEL_HELP = "Enter the title for this new child page.  This title should be short, but can include spaces.  This will appear above the child page text.  If this child page appears in the main menu, this will also appear on the menu.  If this child page appears as a browse by, this will appear in the list of possible browse bys as well.";
			const string VISIBILITY_HELP = "Choose how a link to this child page should appear for the web users.\\n\\nIf you select MAIN MENU, this will appear in the collection main menu system.\\n\\nIf you select BROWSE BY, this will appear with metadata browse bys on the main menu under the BROWSE BY menu item.\\n\\nIf you select NONE, then you will need to add a link to the new child page yourself by editing the text of the home page or an existing linked child page.";
			const string PARENT_HELP = "If this child page will appear on the main menu, you can select a parent child page already on the main menu.  This will create a drop down menu under, or next to, the parent.";

			if (actionMessage.Length > 0)
			{
				Output.WriteLine("  <br />");
				Output.WriteLine("  <div id=\"sbkAdm_ActionMessage\" style=\"color:Maroon;\">" + actionMessage + "</div>");
			}


			Output.WriteLine("<table class=\"sbkAdm_PopupTable\">");

			Output.WriteLine("  <tr class=\"sbkSaav_TitleRow\"><td colspan=\"3\">Child Pages</td></tr>");
			Output.WriteLine("  <tr class=\"sbkSaav_TextRow\"><td colspan=\"3\"><p>Child pages are pages related to the aggregation and allow additional information to be presented within the same aggregational branding.  These can appear in the aggregation main menu, with any metadata browses pulled from the database, or you can set them to for no automatic visibility, in which case they are only accessible by links in the home page or other child pages.</p><p>For more information about the settings on this tab, <a href=\"" + UI_ApplicationCache_Gateway.Settings.Help_URL(RequestSpecificValues.Current_Mode.Base_URL) + "adminhelp/singleaggr\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</p></td></tr>");

			// Put in alphabetical order
            SortedList<string, Complete_Item_Aggregation_Child_Page> sortedChildren = new SortedList<string, Complete_Item_Aggregation_Child_Page>();
            //if (itemAggregation.Child_Pages != null)
            //{
            //    foreach (Complete_Item_Aggregation_Child_Page childPage in itemAggregation.Child_Pages)
            //    {
            //        if (childPage.Source_Data_Type == Item_Aggregation_Child_Source_Data_Enum.Static_HTML)
            //        {
            //            sortedChildren.Add(childPage.Code, childPage);
            //        }
            //    }
            //}


		    // Collect all the static-html based browse and info pages 
			if (sortedChildren.Count == 0)
			{
				Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
				Output.WriteLine("    <td style=\"width:50px\">&nbsp;</td>");
				Output.WriteLine("    <td style=\"width: 165px\" class=\"sbkSaav_TableLabel\">Existing Child Pages:</td>");
				Output.WriteLine("    <td style=\"font-style:italic\">This aggregation currently has no child pages</td>");
				Output.WriteLine("  </tr>");
			}
			else
			{
				// Add EXISTING subcollections
				Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
				Output.WriteLine("    <td style=\"width:50px\">&nbsp;</td>");
				Output.WriteLine("    <td style=\"width: 165px\" class=\"sbkSaav_TableLabel2\" colspan=\"2\">Existing Child Pages:</td>");
				Output.WriteLine("  </tr>");
				Output.WriteLine("  <tr class=\"sbkSaav_TallRow\">");
				Output.WriteLine("    <td>&nbsp;</td>");
				Output.WriteLine("    <td colspan=\"2\">");
				Output.WriteLine("      <table class=\"sbkSaav_ChildPageTable sbkSaav_Table\">");
				Output.WriteLine("        <tr>");
				Output.WriteLine("          <th class=\"sbkSaav_ChildPageTableHeader1\">ACTION</th>");
				Output.WriteLine("          <th class=\"sbkSaav_ChildPageTableHeader2\">CODE</th>");
				Output.WriteLine("          <th class=\"sbkSaav_ChildPageTableHeader3\">TITLE</th>");
				Output.WriteLine("          <th class=\"sbkSaav_ChildPageTableHeader4\">VISIBILITY</th>");
				Output.WriteLine("          <th class=\"sbkSaav_ChildPageTableHeader5\">PARENT</th>");
				Output.WriteLine("          <th class=\"sbkSaav_ChildPageTableHeader6\">LANGUAGE(S)</th>");
				Output.WriteLine("        </tr>");

				foreach (Complete_Item_Aggregation_Child_Page childPage in sortedChildren.Values)
				{
					Output.WriteLine("        <tr>");
					Output.Write("          <td class=\"sbkAdm_ActionLink\" style=\"padding-left: 5px;\" >( ");
					RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Aggregation;
					RequestSpecificValues.Current_Mode.Aggregation_Type = childPage.Browse_Type == Item_Aggregation_Child_Visibility_Enum.Metadata_Browse_By ? Aggregation_Type_Enum.Browse_By : Aggregation_Type_Enum.Browse_Info;
					RequestSpecificValues.Current_Mode.Info_Browse_Mode = childPage.Code;

					Output.Write("<a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\" title=\"View this child page\" target=\"VIEW_" + childPage.Code + "\">view</a> | ");

					RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Administrative;
					RequestSpecificValues.Current_Mode.My_Sobek_SubMode = "g_" + childPage.Code;
					Output.Write("<a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\" title=\"Edit this child page\" >edit</a> | ");
					Output.WriteLine("<a title=\"Click to delete this child page\" href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return edit_aggr_delete_child_page('" + childPage.Code + "');\">delete</a> )</td>");

					Output.WriteLine("          <td>" + childPage.Code + "</td>");
					Output.WriteLine("          <td>" + childPage.Get_Label(UI_ApplicationCache_Gateway.Settings.Default_UI_Language) + "</td>");

					switch (childPage.Browse_Type)
					{
						case Item_Aggregation_Child_Visibility_Enum.Main_Menu:
							Output.WriteLine("          <td>Main Menu</td>");
							break;

						case Item_Aggregation_Child_Visibility_Enum.Metadata_Browse_By:
							Output.WriteLine("          <td>Browse By</td>");
							break;

						case Item_Aggregation_Child_Visibility_Enum.None:
							Output.WriteLine("          <td>None</td>");
							break;
					}
					Output.WriteLine("          <td>" + childPage.Parent_Code + "</td>");

					Output.Write("          <td>");
					int language_count = 0;
				    if (childPage.Source_Dictionary != null)
				    {
				        int total_language_count = childPage.Source_Dictionary.Count;
				        foreach (Web_Language_Enum thisLanguage in childPage.Source_Dictionary.Keys)
				        {
				            string languageName = Web_Language_Enum_Converter.Enum_To_Name(thisLanguage);
				            if ((thisLanguage == Web_Language_Enum.DEFAULT) || (thisLanguage == Web_Language_Enum.UNDEFINED) || (thisLanguage == RequestSpecificValues.Current_Mode.Default_Language))
				                languageName = "<span style=\"font-style:italic\">default</span>";
				            if (language_count == 0)
				                Output.Write(languageName);
				            else
				                Output.Write(", " + languageName);

				            language_count++;
				            if ((language_count > 4) && (language_count < total_language_count - 1))
				            {
				                Output.Write("... (" + (total_language_count - language_count) + "more)");
				                break;
				            }
				        }
				    }

				    Output.WriteLine("</td>");

					Output.WriteLine("        </tr>");
				}
				RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Administrative;

				Output.WriteLine("      </table>");
				Output.WriteLine("    </td>");
				Output.WriteLine("  </tr>");
			}

			// Add ability to add NEW chid pages
			Output.WriteLine("  <tr class=\"sbkSaav_TallRow\">");
			Output.WriteLine("    <td>&nbsp;</td>");
			Output.WriteLine("    <td class=\"sbkSaav_TableLabel2\" style=\"width:145px\">New Child Page:</td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <table class=\"sbkSaav_ChildInnerTable\">");

			// Add line for child page code
			Output.WriteLine("        <tr>");
			Output.WriteLine("          <td style=\"width:120px;\"><label for=\"admin_webcontent_code\">Code:</label></td>");
			Output.WriteLine("          <td style=\"width:165px\"><input class=\"sbkSaav_NewChildCode sbkAdmin_Focusable\" name=\"admin_webcontent_code\" id=\"admin_webcontent_code\" type=\"text\" value=\"" + ( childPageCode ?? String.Empty ) + "\" /></td>");
			Output.WriteLine("          <td colspan=\"2\"><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + CODE_HELP + "');\"  title=\"" + CODE_HELP + "\" /></td>");
			Output.WriteLine("        </tr>");

			// Add the default language label
			Output.WriteLine("        <tr>");
			Output.WriteLine("          <td><label for=\"admin_webcontent_label\">Title (default):</label></td>");
			Output.WriteLine("          <td colspan=\"2\"><input class=\"sbkSaav_SubLargeInput sbkAdmin_Focusable\" name=\"admin_webcontent_label\" id=\"admin_webcontent_label\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(childPageLabel ?? String.Empty) + "\" /></td>");
			Output.WriteLine("          <td style=\"width:30px\"><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + LABEL_HELP + "');\"  title=\"" + LABEL_HELP + "\" /></td>");
			Output.WriteLine("        </tr>");

			// Add the visibility line
			Output.WriteLine("        <tr>");
			Output.WriteLine("          <td><label for=\"admin_webcontent_visibility\">Visibility:</label></td>");
			Output.Write("          <td><select class=\"sbkSaav_SubTypeSelect\" name=\"admin_webcontent_visibility\" id=\"admin_webcontent_visibility\" onchange=\"admin_webcontent_child_page_visibility_change();\">");
			Output.Write    ("<option value=\"\"></option>");

			if (( !String.IsNullOrEmpty(childPageVisibility)) && ( childPageVisibility == "browse"))
				Output.Write    ("<option value=\"browse\" selected=\"selected\">Main Menu</option>");
			else
				Output.Write("<option value=\"browse\">Main Menu</option>");

			if ((!String.IsNullOrEmpty(childPageVisibility)) && (childPageVisibility == "browseby"))
				Output.Write("<option value=\"browseby\" selected=\"selected\">Browse By</option>");
			else
				Output.Write("<option value=\"browseby\">Browse By</option>");

			if ((!String.IsNullOrEmpty(childPageVisibility)) && (childPageVisibility == "none"))
				Output.Write("<option value=\"none\" selected=\"selected\">None</option>");
			else
				Output.Write("<option value=\"none\">None</option>");

			Output.WriteLine("</select></td>");
			Output.WriteLine("          <td colspan=\"2\"><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + VISIBILITY_HELP + "');\"  title=\"" + VISIBILITY_HELP + "\" /></td>");
			Output.WriteLine("        </tr>");

			// Add line for parent code
			if ((!String.IsNullOrEmpty(childPageVisibility)) && (childPageVisibility == "browse"))
				Output.WriteLine("        <tr id=\"admin_webcontent_parent_row\" style=\"display:table-row\">");
			else
				Output.WriteLine("        <tr id=\"admin_webcontent_parent_row\" style=\"display:none\">");

			Output.WriteLine("          <td><label for=\"admin_webcontent_parent\">Parent:</label></td>");
			Output.Write("          <td><select class=\"sbkSaav_SubTypeSelect\" name=\"admin_webcontent_parent\" id=\"admin_webcontent_parent\">");
			Output.Write("<option value=\"\">(none - top level)</option>");
			foreach (Complete_Item_Aggregation_Child_Page childPage in sortedChildren.Values)
			{
				// Only show main menu stuff
				if (childPage.Browse_Type != Item_Aggregation_Child_Visibility_Enum.Main_Menu)
					continue;

				if ( childPageParent == childPage.Code )
					Output.Write("<option value=\"" + childPage.Code + "\" selected=\"selected\">" + childPage.Code + "</option>");
				else
					Output.Write("<option value=\"" + childPage.Code + "\">" + childPage.Code + "</option>");

			}
			Output.WriteLine("</select></td>");
			Output.WriteLine("          <td colspan=\"2\"><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + PARENT_HELP + "');\"  title=\"" + PARENT_HELP + "\" /></td>");
			Output.WriteLine("        </tr>");

			// Add line for button
			Output.WriteLine("        <tr>");
			Output.WriteLine("          <td></td>");
			Output.WriteLine("          <td colspan=\"3\" style=\"text-align: left; padding-left: 50px;\"><button title=\"Save new child page\" class=\"sbkAdm_RoundButton\" onclick=\"return save_new_child_page();\">ADD</button></td>");
			Output.WriteLine("        </tr>");

			// Add the SAVE button
			Output.WriteLine("      </table>");
			Output.WriteLine("    </td>");
			Output.WriteLine("  </tr>");


			Output.WriteLine("</table>");
			Output.WriteLine("<br />");
		}

		#endregion

        #region Methods to render (and parse) page 5 -  Uploads

        private void Save_Uploads_Postback(NameValueCollection Form)
        {
            if (HttpContext.Current.Session["WebContent|" + webContentId + "|Uploads"] != null)
            {
                string files = HttpContext.Current.Session["WebContent|" + webContentId + "|Uploads"].ToString().Replace("|", ", ");
              //  SobekCM_Database.Save_Item_Aggregation_Milestone(itemAggregation.Code, "Uploaded file(s) " + files, RequestSpecificValues.Current_User.Full_Name);
                HttpContext.Current.Session.Remove("WebContent|" + webContentId + "|Uploads");
            }

            string action = Form["admin_webcontent_action"];
            if ((action.Length > 0) && ( action.IndexOf("delete_") == 0))
            {
                string file = action.Substring(7);
                string path_file = Path.Combine(webContentDirectory, file);
                if (File.Exists(path_file))
                {
                    try
                    {
                        File.Delete(path_file);
                       // SobekCM_Database.Save_Item_Aggregation_Milestone(itemAggregation.Code, "Deleted upload file " + file, RequestSpecificValues.Current_User.Full_Name);
                    }
                    catch { }
                }
            }
        }


        private void Add_Page_Uploads(TextWriter Output)
        {
            // Help constants (for now)
            const string UPLOAD_BANNER_HELP = "Press the SELECT FILES button here to upload new images or documents to associated with this collection.   You will be able to access the image files when you are editing the home page text or the text of a child page through the HTML editor.\\n\\nThe following image types can be uploaded: bmp, gif, jpg, png.  The following other documents can also be uploaded: ai, doc, docx, eps, pdf, psd, pub, txt, vsd, vsdx, xls, xlsx, zip.";



            Output.WriteLine("<table class=\"sbkAdm_PopupTable\">");

            Output.WriteLine("  <tr class=\"sbkSaav_TitleRow\"><td colspan=\"3\">Uploaded Images and Documents</td></tr>");
            Output.WriteLine("  <tr class=\"sbkSaav_TextRow\"><td colspan=\"3\"><p>Manage your uploaded images which can be included in your home page or static child pages or other document types which can be uploaded and associated with this aggregation.</p><p>The following image types can be uploaded: bmp, gif, jpg, png.  The following other documents can also be uploaded: ai, doc, docx, eps, kml, pdf, psd, pub, txt, vsd, vsdx, xls, xlsx, xml, zip.</p><p>These files are not associated with any digital resources, but are loosely retained with this collection.</p><p>For more information about the settings on this tab, <a href=\"" + UI_ApplicationCache_Gateway.Settings.Help_URL(RequestSpecificValues.Current_Mode.Base_URL) + "adminhelp/singleaggr\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</p></td></tr>");


            Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\"><td colspan=\"3\">&nbsp;</td></tr>");

            Output.WriteLine("  <tr class=\"sbkSaav_TitleRow\"><td colspan=\"3\">Upload New Images and Documents</td></tr>");
            Output.WriteLine("  <tr class=\"sbkSaav_UploadRow\">");
            Output.WriteLine("    <td style=\"width:100px\">&nbsp;</td>");
            Output.WriteLine("    <td colspan=\"2\">");
            Output.WriteLine("       <table class=\"sbkSaav_InnerTable\">");
            Output.WriteLine("         <tr>");
            Output.WriteLine("           <td class=\"sbkSaav_UploadInstr\">To upload one or more images or documents press SELECT FILES, browse to the file, and then select UPLOAD</td>");
            Output.WriteLine("           <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + UPLOAD_BANNER_HELP + "');\"  title=\"" + UPLOAD_BANNER_HELP + "\" /></td>");
            Output.WriteLine("         </tr>");
            Output.WriteLine("         <tr>");
            Output.WriteLine("           <td colspan=\"2\">");
        }

        private void Finish_Page_Uploads(TextWriter Output)
        {
            Output.WriteLine("           </td>");
            Output.WriteLine("         </tr>");
            Output.WriteLine("       </table>");
            Output.WriteLine("     </td>");
            Output.WriteLine("  </tr>");
            Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\"><td colspan=\"3\">&nbsp;</td></tr>");

            string uploads_dir = webContentDirectory;
            if (Directory.Exists(uploads_dir))
            {
                // Add existing IMAGES
                string[] image_files = SobekCM_File_Utilities.GetFiles(uploads_dir, "*.jpg|*.jpeg|*.bmp|*.gif|*.png");
                if (image_files.Length > 0)
                {
                    Output.WriteLine("  <tr class=\"sbkSaav_TitleRow\"><td colspan=\"3\">Existing Images</td></tr>");
                    Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\"><td colspan=\"3\">&nbsp;</td></tr>");
                    Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
                    Output.WriteLine("    <td colspan=\"3\" style=\"text-align:left\">");


                    Output.WriteLine("  <table class=\"sbkSaav_UploadTable\">");
                    Output.WriteLine("    <tr>");

                    int unused_column = 0;
                    foreach (string thisImage in image_files)
                    {
                        string thisImageFile = Path.GetFileName(thisImage);
                        string thisImageFile_URL = RequestSpecificValues.Current_Mode.Base_URL + "design/webcontent/" + webContent.UrlSegments + "/" + thisImageFile;
                        string display_name = thisImageFile;

                        Output.Write("      <td>");
                        Output.Write("<a href=\"" + thisImageFile_URL + "\" target=\"_" + thisImageFile + "\" title=\"" + display_name + "\">");
                        Output.Write("<img class=\"sbkSaav_UploadThumbnail\" src=\"" + thisImageFile_URL + "\" alt=\"Missing Thumbnail\" title=\"" + thisImageFile + "\" /></a>");

                        
                        if (( !String.IsNullOrEmpty(display_name)) && (display_name.Length > 25))
                        {
                            Output.Write("<br /><span class=\"sbkSaav_UploadTitle\"><abbr title=\"" + display_name + "\">" + thisImageFile.Substring(0, 20) + "..." + Path.GetExtension(thisImage) + "</abbr></span>");
                        }
                        else
                        {
                            Output.Write("<br /><span class=\"sbkSaav_UploadTitle\">" + thisImageFile + "</span>");
                        }

                        

                        // Build the action links
                        Output.Write("<br /><span class=\"sbkAdm_ActionLink\" >( ");
                        Output.Write("<a href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return delete_aggr_upload_file('" + thisImageFile + "');\" title=\"Delete this uploaded file\">delete</a> | ");
                        Output.Write("<a href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "l/technical/javascriptrequired\" onclick=\"window.prompt('Below is the URL, available to copy to your clipboard.  To copy to clipboard, press Ctrl+C (or Cmd+C) and Enter', '" + thisImageFile_URL + "'); return false;\" title=\"View the URL for this file\">view url</a>");

                        Output.WriteLine(" )</span></td>");

                        unused_column++;

                        // Start a new row?
                        if (unused_column >= 4)
                        {
                            Output.WriteLine("    </tr>");
                            Output.WriteLine("    <tr>");
                            unused_column = 0;
                        }
                    }

                    // Finish the table cells and row
                    while (unused_column < 4)
                    {
                        Output.WriteLine("      <td></td>");
                        unused_column++;
                    }
                    Output.WriteLine("    </tr>");

                    Output.WriteLine("  </table>");

                    Output.WriteLine("    </td>");
                    Output.WriteLine("  </tr>");
                }

                // Add existing DOCUMENTS
                string[] documents_files = SobekCM_File_Utilities.GetFiles(uploads_dir, "*.ai|*.doc|*.docx|*.eps|*.kml|*.pdf|*.psd|*.pub|*.txt|*.vsd|*.vsdx|*.xls|*.xlsx|*.xml|*.zip");
                if (documents_files.Length > 0)
                {
                    Output.WriteLine("  <tr class=\"sbkSaav_TitleRow\"><td colspan=\"3\">Existing Documents</td></tr>");
                    Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\"><td colspan=\"3\">&nbsp;</td></tr>");
                    Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
                    Output.WriteLine("    <td colspan=\"3\" style=\"text-align:left\">");


                    Output.WriteLine("  <table class=\"sbkSaav_UploadTable\">");
                    Output.WriteLine("    <tr>");

                    int unused_column = 0;
                    foreach (string thisDocument in documents_files)
                    {
                        string thisDocFile = Path.GetFileName(thisDocument);
                        string thisDocFile_URL = RequestSpecificValues.Current_Mode.Base_URL + "design/webcontent/" + webContent.UrlSegments + "/" + thisDocFile;

                        // Determine which image to use for this document
                        string extension = Path.GetExtension(thisDocument);
                        if (String.IsNullOrEmpty(extension))
                            continue;

                        string thisDocFileImage = Static_Resources.File_TXT_Img;
                        switch (extension.ToUpper().Replace(".", ""))
                        {
                            case "AI":
                                thisDocFileImage = Static_Resources.File_AI_Img;
                                break;

                            case "DOC":
                            case "DOCX":
                                thisDocFileImage = Static_Resources.File_Word_Img;
                                break;

                            case "EPS":
                                thisDocFileImage = Static_Resources.File_EPS_Img;
                                break;

                            case "KML":
                                thisDocFileImage = Static_Resources.File_KML_Img;
                                break;

                            case "PDF":
                                thisDocFileImage = Static_Resources.File_PDF_Img;
                                break;

                            case "PSD":
                                thisDocFileImage = Static_Resources.File_PSD_Img;
                                break;

                            case "PUB":
                                thisDocFileImage = Static_Resources.File_PUB_Img;
                                break;

                            case "TXT":
                                thisDocFileImage = Static_Resources.File_TXT_Img;
                                break;

                            case "VSD":
                            case "VSDX":
                                thisDocFileImage = Static_Resources.File_VSD_Img;
                                break;

                            case "XLS":
                            case "XLSX":
                                thisDocFileImage = Static_Resources.File_Excel_Img;
                                break;

                            case "XML":
                                thisDocFileImage = Static_Resources.File_XML_Img;
                                break;

                            case "ZIP":
                                thisDocFileImage = Static_Resources.File_ZIP_Img;
                                break;
                        }
                        Output.Write("      <td>");
                        Output.Write("<a href=\"" + thisDocFile_URL + "\" target=\"_" + thisDocFile + "\" title=\"View this uploaded image\">");
                        Output.Write("<img class=\"sbkSaav_UploadThumbnail2\" src=\"" + thisDocFileImage + "\" alt=\"Document\" title=\"" + thisDocFile + "\" /></a>");


                        string display_name = thisDocFile;
                        
                        if (( !String.IsNullOrEmpty(display_name)) && ( display_name.Length > 25))
                        {
                            Output.Write("<br /><span class=\"sbkSaav_UploadTitle\"><abbr title=\"" + display_name + "\">" + thisDocFile.Substring(0, 20) + "..." + extension + "</abbr></span>");
                        }
                        else
                        {
                            Output.Write("<br /><span class=\"sbkSaav_UploadTitle\">" + thisDocFile + "</span>");
                        }



                        // Build the action links
                        Output.Write("<br /><span class=\"sbkAdm_ActionLink\" >( ");
                        Output.Write("<a href=\"" + thisDocFile_URL + "\" target=\"_" + thisDocFile + "\" title=\"Download and view this uploaded file\">download</a> | ");
                        Output.Write("<a href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return delete_aggr_upload_file('" + thisDocFile + "');\" title=\"Delete this uploaded file\">delete</a> | ");
                        Output.Write("<a href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "l/technical/javascriptrequired\" onclick=\"window.prompt('Below is the URL, available to copy to your clipboard.  To copy to clipboard, press Ctrl+C (or Cmd+C) and Enter', '" + thisDocFile_URL + "'); return false;\" title=\"View the URL for this file\">view url</a>");

                        Output.WriteLine(" )</span></td>");

                        unused_column++;

                        // Start a new row?
                        if (unused_column >= 4)
                        {
                            Output.WriteLine("    </tr>");
                            Output.WriteLine("    <tr>");
                            unused_column = 0;
                        }
                    }

                    // Finish the table cells and row
                    while (unused_column < 4)
                    {
                        Output.WriteLine("      <td></td>");
                        unused_column++;
                    }
                    Output.WriteLine("    </tr>");

                    Output.WriteLine("  </table>");

                    Output.WriteLine("    </td>");
                    Output.WriteLine("  </tr>");
                }
            }
            Output.WriteLine("</table>");
            Output.WriteLine("<br />");
        }

        #endregion

		#region Methods to add file upload controls to the page

		/// <summary> Add controls directly to the form in the main control area placeholder </summary>
		/// <param name="MainPlaceHolder"> Main place holder to which all main controls are added </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		public override void Add_Controls(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
		{
			Tracer.Add_Trace("File_Managament_MySobekViewer.Add_Controls", String.Empty);

			switch (page)
			{
                case 9:
                    add_upload_controls(MainPlaceHolder, ".gif,.bmp,.jpg,.png,.jpeg,.ai,.doc,.docx,.eps,.kml,.pdf,.psd,.pub,.txt,.vsd,.vsdx,.xls,.xlsx,.xml,.zip", webContentDirectory, String.Empty, true, webContent.WebContentID + "|Uploads", Tracer);
                    break;
			}
		}

		private void add_upload_controls(PlaceHolder UploadFilesPlaceHolder, string FileExtensions, string UploadDirectory, string ServerSideName, bool UploadMultiple, string ReturnToken, Custom_Tracer Tracer)
		{
			Tracer.Add_Trace("File_Managament_MySobekViewer.add_upload_controls", String.Empty);

			// Ensure the directory exists
			if (!File.Exists(UploadDirectory))
				Directory.CreateDirectory(UploadDirectory);

			StringBuilder filesBuilder = new StringBuilder(2000);

			LiteralControl filesLiteral2 = new LiteralControl(filesBuilder.ToString());
			UploadFilesPlaceHolder.Controls.Add(filesLiteral2);
			filesBuilder.Remove(0, filesBuilder.Length);

			UploadiFiveControl uploadControl = new UploadiFiveControl
			{
			    UploadPath = UploadDirectory, 
                UploadScript = RequestSpecificValues.Current_Mode.Base_URL + "UploadiFiveFileHandler.ashx", 
                AllowedFileExtensions = FileExtensions, 
                SubmitWhenQueueCompletes = true, 
                RemoveCompleted = true, 
                Multi = UploadMultiple, 
                ServerSideFileName = ServerSideName, 
                ReturnToken = ReturnToken
			};
		    UploadFilesPlaceHolder.Controls.Add(uploadControl);

			LiteralControl literal1 = new LiteralControl(filesBuilder.ToString());
			UploadFilesPlaceHolder.Controls.Add(literal1);
		}

		#endregion

    }
}
