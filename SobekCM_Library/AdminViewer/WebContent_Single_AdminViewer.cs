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
using SobekCM.Core.MemoryMgmt;
using SobekCM.Core.Navigation;
using SobekCM.Core.WebContent;
using SobekCM.Engine_Library.Aggregations;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Engine_Library.Database;
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
		private readonly string aggregationDirectory;
        private readonly Complete_Item_Aggregation itemAggregation;

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

			// Set some defaults
			actionMessage = String.Empty;
		    string code = RequestSpecificValues.Hierarchy_Object.Code;

			// If the RequestSpecificValues.Current_User cannot edit this, go back
            if (!RequestSpecificValues.Current_User.Is_Aggregation_Curator(code))
			{
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.My_Sobek;
                RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
				return;
			}

			// Load the item aggregation, either currenlty from the session (if already editing this aggregation )
			// or by reading all the appropriate XML and reading data from the database
			object possibleEditAggregation = HttpContext.Current.Session["Edit_Aggregation_" + code];
            Complete_Item_Aggregation cachedInstance = possibleEditAggregation as Complete_Item_Aggregation;
		    if (cachedInstance != null)
		    {
		        itemAggregation = cachedInstance;
		    }
		    else
		    {
		        itemAggregation = SobekEngineClient.Aggregations.Get_Complete_Aggregation(code, false, RequestSpecificValues.Tracer);
		    }

			// If unable to retrieve this aggregation, send to home
			if (itemAggregation == null)
			{
				RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
				UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
				return;
			}

			// Get the aggregation directory and ensure it exists
			aggregationDirectory = HttpContext.Current.Server.MapPath("design/aggregations/" + itemAggregation.Code );
			if (!Directory.Exists(aggregationDirectory))
				Directory.CreateDirectory(aggregationDirectory);

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
                else if (RequestSpecificValues.Current_Mode.My_Sobek_SubMode == "j")
                    page = 10;
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
						HttpContext.Current.Session["Edit_Aggregation_" + itemAggregation.Code] = null;
						HttpContext.Current.Session["Item_Aggr_Edit_" + itemAggregation.Code + "_NewLanguages"] = null;

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

						case 10:
							Save_Page_CSS_Postback(form);
							break;
					}

					// Should this be saved to the database?
                    if ((action == "save") || (action == "save_exit") || (action == "save_wizard"))
					{
                        // Get the current aggrgeation information, for comparison
                        Complete_Item_Aggregation currentAggregation = SobekEngineClient.Aggregations.Get_Complete_Aggregation(code, true, RequestSpecificValues.Tracer);

                        // Backup the old aggregation info
					    string backup_folder = UI_ApplicationCache_Gateway.Settings.Base_Design_Location + itemAggregation.ObjDirectory.Replace("/","\\") + "backup\\configs";
					    if (!Directory.Exists(backup_folder))
					        Directory.CreateDirectory(backup_folder);
					    string current_config = UI_ApplicationCache_Gateway.Settings.Base_Design_Location + itemAggregation.ObjDirectory + "\\" + itemAggregation.Code + ".xml";
					    if (File.Exists(current_config))
					    {
                            // Use the last modified date as the name of the backup
					        DateTime lastModifiedDate = (new FileInfo(current_config)).LastWriteTime;
                            string backup_name = itemAggregation.Code + lastModifiedDate.Year + lastModifiedDate.Month.ToString().PadLeft(2, '0') + lastModifiedDate.Day.ToString().PadLeft(2, '0') + lastModifiedDate.Hour.ToString().PadLeft(2, '0') + lastModifiedDate.Minute.ToString().PadLeft(2, '0') + ".xml";
                            if (!File.Exists(backup_folder + "\\" + backup_name))
    					        File.Copy(current_config, backup_folder + "\\" + backup_name, false );
					    }

						// Save the new configuration file
                        string save_error = String.Empty;
                        bool successful_save = true;
					    if (!itemAggregation.Write_Configuration_File(UI_ApplicationCache_Gateway.Settings.Base_Design_Location + itemAggregation.ObjDirectory))
					    {
                            successful_save = false;
					        save_error = "<br /><br />Error saving the configuration file";
					    }

					    // Save to the database
					    if (!Item_Aggregation_Utilities.Save_To_Database(itemAggregation, RequestSpecificValues.Current_User.Full_Name, null))
					    {
					        successful_save = false;
                            save_error = "<br /><br />Error saving to the database.";

					        if (Engine_Database.Last_Exception != null)
					        {
					            save_error = save_error + "<br /><br />" + Engine_Database.Last_Exception.Message;
					        }
					    }

					    // Save the link between this item and the thematic heading
					    int thematicHeadingId = -1;
                        if (itemAggregation.Thematic_Heading != null)
                            thematicHeadingId = itemAggregation.Thematic_Heading.ID;
                        UI_ApplicationCache_Gateway.Aggregations.Set_Aggregation_Thematic_Heading(itemAggregation.Code, thematicHeadingId);


						// Clear the aggregation from the cache
						CachedDataManager.Aggregations.Remove_Item_Aggregation(itemAggregation.Code, null);
					    CachedDataManager.Aggregations.Clear_Aggregation_Hierarchy();
					    Engine_ApplicationCache_Gateway.RefreshCodes();
					    Engine_ApplicationCache_Gateway.RefreshThematicHeadings();



						// Forward back to the aggregation home page, if this was successful
						if (successful_save)
						{
                            // Also, update the information that was changed
						    try
						    {
						        List<string> changes = Complete_Item_Aggregation_Comparer.Compare(currentAggregation, itemAggregation);
						        if ((changes != null) && (changes.Count > 0))
						        {
						            StringBuilder builder = new StringBuilder(changes[0]);
						            for (int i = 1; i < changes.Count; i++)
						            {
						                builder.Append("\n" + changes[i]);
						            }
						            SobekCM_Database.Save_Item_Aggregation_Milestone(itemAggregation.Code, builder.ToString(), RequestSpecificValues.Current_User.Full_Name);

						        }
						        else
						        {
						            SobekCM_Database.Save_Item_Aggregation_Milestone(itemAggregation.Code, "Configuration edited", RequestSpecificValues.Current_User.Full_Name);
						        }
						    }
						    catch
						    {
                                SobekCM_Database.Save_Item_Aggregation_Milestone(itemAggregation.Code, "Configuration edited", RequestSpecificValues.Current_User.Full_Name);
						    }


							// Clear the aggregation from the sessions
							HttpContext.Current.Session["Edit_Aggregation_" + itemAggregation.Code] = null;
							HttpContext.Current.Session["Item_Aggr_Edit_" + itemAggregation.Code + "_NewLanguages"] = null;

							// Redirect the RequestSpecificValues.Current_User
						    if (action == "save_exit")
						    {
						        RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Aggregation;
						        RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Home;
						        UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
						    }
                            else if (action == "save_wizard")
                            {

                                RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Add_Collection_Wizard;
                                string wizard_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
                                RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Aggregations_Mgmt;

                                if (wizard_url.IndexOf("?") < 0)
                                    wizard_url = wizard_url + "?parent=" + itemAggregation.Code;
                                else
                                    wizard_url = wizard_url + "&parent=" + itemAggregation.Code;

                                RequestSpecificValues.Current_Mode.Request_Completed = true;
                                HttpContext.Current.Response.Redirect(wizard_url, false);
                                HttpContext.Current.ApplicationInstance.CompleteRequest();

                            }
                            else
                            {
                                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                            }
						}
						else
						{
                            actionMessage = "Error saving aggregation information!" + save_error;
						}
					}
					else 
					{
						// In some cases, skip this part
						if (((page == 8) && (action == "h")) || ((page == 7) && (action == "g")))
							return;

						// Save to the admins session
						HttpContext.Current.Session["Edit_Aggregation_" + itemAggregation.Code] = itemAggregation;
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
                Output.WriteLine("    <button title=\"Save changes to this item Aggregation\" class=\"sbkAdm_RoundButton\" onclick=\"return save_webcontent_edits(false);\"> SAVE </button> &nbsp; &nbsp; ");
				Output.WriteLine("    <button title=\"Save changes to this item Aggregation and exit the admin screens\" class=\"sbkAdm_RoundButton\" onclick=\"return save_webcontent_edits(true);\">SAVE & EXIT <img src=\"" + Static_Resources.Button_Next_Arrow_Png + "\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button>");
				Output.WriteLine("  </div>");
				Output.WriteLine();
				RequestSpecificValues.Current_Mode.My_Sobek_SubMode = last_mode;
			}


            Output.WriteLine("  <div class=\"sbkAdm_TitleDiv\" style=\"padding-left:20px\">");
            Output.WriteLine("    <img id=\"sbkAdm_TitleDivImg\" src=\"" + Static_Resources.Admin_View_Img + "\" alt=\"\" />");
            Output.WriteLine("    <h1>Collection Administration : " + itemAggregation.Code.ToUpper() + "</h1>");
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


                const string LOCALIZATION = "Localization";
                if (page == 2)
                {
                    Output.WriteLine("    <li id=\"tabHeader_2\" class=\"tabActiveHeader\">" + LOCALIZATION + "</li>");
                }
                else
                {
                    Output.WriteLine("    <li id=\"tabHeader_2\" onclick=\"return new_webcontent_edit_page('b');\">" + LOCALIZATION + "</li>");
                }

                const string SITEMAP = "Sitemap";
                if (page == 3)
                {
                    Output.WriteLine("    <li id=\"tabHeader_2\" class=\"tabActiveHeader\">" + SITEMAP + "</li>");
                }
                else
                {
                    Output.WriteLine("    <li id=\"tabHeader_2\" onclick=\"return new_webcontent_edit_page('c');\">" + SITEMAP + "</li>");
                }

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

				case 10:
					Add_Page_CSS(Output);
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
            // Log any uploaded button
            if (HttpContext.Current.Session[itemAggregation.Code + "|Button"] != null)
            {
                SobekCM_Database.Save_Item_Aggregation_Milestone(itemAggregation.Code, "Button changed" , RequestSpecificValues.Current_User.Full_Name);
                HttpContext.Current.Session.Remove(itemAggregation.Code + "|Button");
            }

			if (Form["admin_aggr_name"] != null) itemAggregation.Name = Form["admin_aggr_name"];
			if (Form["admin_aggr_shortname"] != null) itemAggregation.ShortName = Form["admin_aggr_shortname"];
			if (Form["admin_aggr_link"] != null) itemAggregation.External_Link = Form["admin_aggr_link"];
			if ( Form["admin_aggr_desc"] != null ) itemAggregation.Description = Form["admin_aggr_desc"];
			if (Form["admin_aggr_email"] != null) itemAggregation.Contact_Email = Form["admin_aggr_email"];
			itemAggregation.Active = Form["admin_aggr_isactive"] != null;
			itemAggregation.Hidden = Form["admin_aggr_ishidden"] == null;
			if ((RequestSpecificValues.Current_User.Is_System_Admin) || (RequestSpecificValues.Current_User.Is_Portal_Admin))
			{
			    if ((Form["admin_aggr_heading"] != null) && (Form["admin_aggr_heading"] != "-1"))
			    {
			        itemAggregation.Thematic_Heading = new Thematic_Heading(Convert.ToInt32(Form["admin_aggr_heading"]), String.Empty);
			    }
			    else
			        itemAggregation.Thematic_Heading = null;
			}

		}

		private void Add_Page_General( TextWriter Output )
		{
            // Basic Information
            //      Title
            //      Author
            //      Description/Summary
            //      Keywords
            //      HTML Heading Info
            //      Redirect URL

            // Appearance
            //      Web_Skin
            //      Banner

            // SiteMap
            //      SiteMap
            //      IncludeMenu


			// Help constants (for now)
            const string LONG_NAME_HELP = "The full name for this collection. This will be used throughout the system to identify this collection. The only place this will not appear is in the breadcrumbs, where the shorter version below will be used.";
            const string SHORT_NAME_HELP = "A shorter version of the name to be used in the breadcrumbs. Generally, try to keep this as short as possible, as items may appear in multiple collections.";
            const string LINK_HELP = "Institutional collections can have an external link added. The link will be displayed in the citation of any digital resources associated with this institution, linked to the source institution or holding location text.";
            const string DESCRIPTION_HELP = "Brief description of this collection. This description is public and will appear wherever the collection appears, such as under the thematic headings on the home page or as a subcollection under the parent collection(s).";
			const string EMAIL_HELP = "Email address that will receive messages from the built-in contact forms, when a user is in this collection.  If this is left blank, the system default will be used.";
			const string ACTIVE_HELP = "Flag indicates if this collection should be active. Active collections appear in breadcrumbs when you view digital resources and generally appear in all public lists of collections. You can add items to inactive collections and build the collection prior to &quot;publishing&quot; it later by making it active.";
            const string HIDDEN_HELP = "Flag indicates if this collection should appear in the home page of the parent collection. In all other respects, a hidden collection works just like an active collection.";
            const string COLLECTION_BUTTON_HELP = "Upload a button for this new collection. Buttons appear on the home page or parent collection home page once a collection is active and not hidden.";
			

			Output.WriteLine("<table class=\"sbkAdm_PopupTable\">");

			Output.WriteLine("  <tr class=\"sbkSaav_TitleRow\"><td colspan=\"3\">Basic Information</td></tr>");
			Output.WriteLine("  <tr class=\"sbkSaav_TextRow\"><td colspan=\"3\"><p>The information in this section is the basic information about the aggregation, such as the full name, the shortened name used for breadcrumbs, the description, and the email contact.</p><p>For more information about the settings on this tab, <a href=\"" + UI_ApplicationCache_Gateway.Settings.Help_URL(RequestSpecificValues.Current_Mode.Base_URL) + "adminhelp/singleaggr\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</p></td></tr>");

			// Add the parent code(s)
			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
			Output.WriteLine("    <td style=\"width:50px\">&nbsp;</td>");
			Output.WriteLine("    <td style=\"width: 145px\" class=\"sbkSaav_TableLabel\">Parent Code(s):</td>");
			Output.WriteLine("    <td> " + HttpUtility.HtmlEncode(itemAggregation.Parent_Codes) + "</td>");
			Output.WriteLine("  </tr>");

			// Add the full name line
			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
			Output.WriteLine("    <td>&nbsp;</td>");
			Output.WriteLine("    <td class=\"sbkSaav_TableLabel\"><label for=\"admin_aggr_name\">Name (full):</label></td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td><input class=\"sbkSaav_large_input sbkAdmin_Focusable\" name=\"admin_aggr_name\" id=\"admin_aggr_name\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(itemAggregation.Name) + "\" /></td>");
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + LONG_NAME_HELP + "');\"  title=\"" + LONG_NAME_HELP + "\" /></td></tr></table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");

			// Add the short name line
			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
			Output.WriteLine("    <td>&nbsp;</td>");
			Output.WriteLine("    <td class=\"sbkSaav_TableLabel\"><label for=\"admin_aggr_shortname\">Name (short):</label></td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td><input class=\"sbkSaav_medium_input sbkAdmin_Focusable\" name=\"admin_aggr_shortname\" id=\"admin_aggr_shortname\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(itemAggregation.ShortName) + "\" /></td>");
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + SHORT_NAME_HELP + "');\"  title=\"" + SHORT_NAME_HELP + "\" /></td></tr></table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");

			// Add the description box
			Output.WriteLine("  <tr class=\"sbkSaav_TallRow\">");
			Output.WriteLine("    <td>&nbsp;</td>");
			Output.WriteLine("    <td class=\"sbkSaav_TableLabel2\"><label for=\"admin_aggr_desc\">Description:</label></td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <table class=\"sbkSaav_InnerTable2\"><tr style=\"vertical-align:top\"><td><textarea class=\"sbkSaav_large_textbox sbkAdmin_Focusable\" rows=\"6\" name=\"admin_aggr_desc\" id=\"admin_aggr_desc\">" + HttpUtility.HtmlEncode(itemAggregation.Description) + "</textarea></td>");
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + DESCRIPTION_HELP + "');\"  title=\"" + DESCRIPTION_HELP + "\" /></td></tr></table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");

			// Add the email line
			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
			Output.WriteLine("    <td>&nbsp;</td>");
			Output.WriteLine("    <td class=\"sbkSaav_TableLabel\"><label for=\"admin_aggr_email\">Contact Email:</label></td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td><input class=\"sbkSaav_large_input sbkAdmin_Focusable\" name=\"admin_aggr_email\" id=\"admin_aggr_email\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(itemAggregation.Contact_Email) + "\" /></td>");
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + EMAIL_HELP + "');\"  title=\"" + EMAIL_HELP + "\" /></td></tr></table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");

			// Add the link line
			if (itemAggregation.Type.IndexOf("Institution", StringComparison.OrdinalIgnoreCase) >= 0)
			{
				Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
				Output.WriteLine("    <td>&nbsp;</td>");
				Output.WriteLine("    <td class=\"sbkSaav_TableLabel\"><label for=\"admin_aggr_link\">External Link:</label></td>");
				Output.WriteLine("    <td>");
				Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td><input class=\"sbkSaav_large_input sbkAdmin_Focusable\" name=\"admin_aggr_link\" id=\"admin_aggr_link\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(itemAggregation.External_Link) + "\" /></td>");
				Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + LINK_HELP + "');\"  title=\"" + LINK_HELP + "\" /></td></tr></table>");
				Output.WriteLine("     </td>");
				Output.WriteLine("  </tr>");
			}

			Output.WriteLine("  <tr class=\"sbkSaav_TitleRow2\"><td colspan=\"3\">Collection Visibility</td></tr>");
			Output.WriteLine("  <tr class=\"sbkSaav_TextRow\"><td colspan=\"3\"><p>The values in this section determine if the collection is currently visible at all, whether it is eligible to appear on the collection list at the bottom of the parent page, and the collection button used in that case.  Thematic headings are used to place this collection on the main home page.</p></td></tr>");


			// Add the behavior lines
			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
			Output.WriteLine("    <td>&nbsp;</td>");
			Output.WriteLine("    <td class=\"sbkSaav_TableLabel\">Behavior:</label></td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td>");
			Output.WriteLine(itemAggregation.Active
			   ? "          <input class=\"sbkSaav_checkbox\" type=\"checkbox\" name=\"admin_aggr_isactive\" id=\"admin_aggr_isactive\" checked=\"checked\" /> <label for=\"admin_aggr_isactive\">Active?</label> "
			   : "          <input class=\"sbkSaav_checkbox\" type=\"checkbox\" name=\"admin_aggr_isactive\" id=\"admin_aggr_isactive\" /> <label for=\"admin_aggr_isactive\">Active?</label> ");
			Output.WriteLine("        </td>");
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + ACTIVE_HELP + "');\"  title=\"" + ACTIVE_HELP + "\" /></td></tr></table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");

			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
			Output.WriteLine("    <td colspan=\"2\">&nbsp;</td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td>");
			Output.WriteLine(!itemAggregation.Hidden
						   ? "          <input class=\"sbkSaav_checkbox\" type=\"checkbox\" name=\"admin_aggr_ishidden\" id=\"admin_aggr_ishidden\" checked=\"checked\" /> <label for=\"admin_aggr_ishidden\">Show in parent collection home page?</label> "
						   : "          <input class=\"sbkSaav_checkbox\" type=\"checkbox\" name=\"admin_aggr_ishidden\" id=\"admin_aggr_ishidden\" /> <label for=\"admin_aggr_ishidden\">Show in parent collection home page (and tree view)?</label> ");
			Output.WriteLine("        </td>");
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + HIDDEN_HELP + "');\"  title=\"" + HIDDEN_HELP + "\" /></td></tr></table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");

			// Add the collection button
			Output.WriteLine("  <tr class=\"sbkSaav_ButtonRow\">");
			Output.WriteLine("    <td>&nbsp;</td>");
			Output.WriteLine("    <td class=\"sbkSaav_TableLabel\">Collection Button:</td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("       <img class=\"sbkSaav_ButtonImg\" src=\"" + RequestSpecificValues.Current_Mode.Base_Design_URL + "aggregations/" + itemAggregation.Code + "/images/buttons/coll.gif\" alt=\"NONE\" />");

			Output.WriteLine("       <table class=\"sbkSaav_InnerTable\">");
			Output.WriteLine("         <tr>");
			Output.WriteLine("           <td class=\"sbkSaav_UploadInstr\">To change, browse to a 50x50 pixel GIF file, and then select UPLOAD</td>");
			Output.WriteLine("           <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + COLLECTION_BUTTON_HELP + "');\"  title=\"" + COLLECTION_BUTTON_HELP + "\" /></td>");
			Output.WriteLine("         </tr>");
			Output.WriteLine("         <tr>");
			Output.WriteLine("           <td colspan=\"2\">");

            const string THEMATIC_HELP = "To make this collection appear on the home page of this repository, you must add it to an existing thematic heading. Thematic headings categorize the collections within your repository.";

			Output.WriteLine("           </td>");
			Output.WriteLine("         </tr>");
			Output.WriteLine("       </table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");


			if ((RequestSpecificValues.Current_User.Is_System_Admin) || (RequestSpecificValues.Current_User.Is_Portal_Admin))
			{
				// Add the thematic heading line
				Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
				Output.WriteLine("    <td>&nbsp;</td>");
				Output.WriteLine("    <td class=\"sbkSaav_TableLabel\"><label for=\"admin_aggr_heading\">Thematic Heading:</label></td>");
				Output.WriteLine("    <td>");
				Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td>");
				Output.WriteLine("          <select class=\"sbkSaav_select_large\" name=\"admin_aggr_heading\" id=\"admin_aggr_heading\">");
			    int thematic_heading_id = -1;
                if (itemAggregation.Thematic_Heading != null)
                    thematic_heading_id = itemAggregation.Thematic_Heading.ID;
                Output.WriteLine(thematic_heading_id == -1 ? "            <option value=\"-1\" selected=\"selected\" ></option>" : "            <option value=\"-1\"></option>");
				foreach (Thematic_Heading thisHeading in UI_ApplicationCache_Gateway.Thematic_Headings)
				{
					if (thematic_heading_id == thisHeading.ID)
					{
						Output.WriteLine("            <option value=\"" + thisHeading.ID + "\" selected=\"selected\" >" + HttpUtility.HtmlEncode(thisHeading.Text) + "</option>");
					}
					else
					{
						Output.WriteLine("            <option value=\"" + thisHeading.ID + "\">" + HttpUtility.HtmlEncode(thisHeading.Text) + "</option>");
					}
				}
				Output.WriteLine("          </select>");
				Output.WriteLine("        </td>");
				Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + THEMATIC_HELP + "');\"  title=\"" + THEMATIC_HELP + "\" /></td></tr></table>");
				Output.WriteLine("     </td>");
				Output.WriteLine("  </tr>");
			}

			Output.WriteLine("</table>");
			Output.WriteLine("<br />");
		}

		#endregion

		#region Methods to render (and parse) page 4 - Child pages

        private void Save_Child_Pages_Postback(NameValueCollection Form)
		{
			string action = Form["admin_webcontent_action"];
			if (!String.IsNullOrEmpty(action))
			{
				if ((action.IndexOf("delete_") == 0) && ( action.Length > 7 ))
				{
					string code_to_delete = action.Substring(7);
					itemAggregation.Remove_Child_Page(code_to_delete);

					// Save to the admins session
					HttpContext.Current.Session["Edit_Aggregation_" + itemAggregation.Code] = itemAggregation;
				}

				if (action == "save_childpage")
				{
					childPageCode = Form["admin_aggr_code"];
					childPageLabel = Form["admin_aggr_label"];
					childPageVisibility = Form["admin_aggr_visibility"];
					childPageParent = Form["admin_aggr_parent"];

					// Convert to the integer id for the parent and begin to do checking
					List<string> errors = new List<string>();

					// Validate the code
					if (childPageCode.Length > 20)
					{
						errors.Add("New child page code must be twenty characters long or less");
					}
					else if (childPageCode.Length == 0)
					{
						errors.Add("You must enter a CODE for this child page");

					}
					else if (UI_ApplicationCache_Gateway.Settings.Reserved_Keywords.Contains(childPageCode.ToLower()))
					{
						errors.Add("That code is a system-reserved keyword.  Try a different code.");
					}
					else if (itemAggregation.Child_Page_By_Code(childPageCode.ToUpper()) != null)
					{
						errors.Add("New code must be unique... <i>" + childPageCode + "</i> already exists");
					}


					if (childPageLabel.Trim().Length == 0)
						errors.Add("You must enter a LABEL for this child page");
					if (childPageVisibility.Trim().Length == 0)
						errors.Add("You must select a VISIBILITY for this child page");

					if (errors.Count > 0)
					{
						// Create the error message
						actionMessage = "ERROR: Invalid entry for new item child page<br />";
						foreach (string error in errors)
							actionMessage = actionMessage + "<br />" + error;
					}
					else
					{
                        Complete_Item_Aggregation_Child_Page newPage = new Complete_Item_Aggregation_Child_Page { Code = childPageCode, Parent_Code = childPageParent, Source_Data_Type = Item_Aggregation_Child_Source_Data_Enum.Static_HTML };
						newPage.Add_Label(childPageLabel, UI_ApplicationCache_Gateway.Settings.Default_UI_Language);
						switch (childPageVisibility)
						{
							case "none":
                                newPage.Browse_Type = Item_Aggregation_Child_Visibility_Enum.None;
                                newPage.Parent_Code = String.Empty;
								break;

							case "browse":
								newPage.Browse_Type = Item_Aggregation_Child_Visibility_Enum.Main_Menu;
								break;

							case "browseby":
								newPage.Browse_Type = Item_Aggregation_Child_Visibility_Enum.Metadata_Browse_By;
                                newPage.Parent_Code = String.Empty;
								break;
						}
						string html_source_dir = aggregationDirectory + "\\html\\browse";
						if (!Directory.Exists(html_source_dir))
							Directory.CreateDirectory(html_source_dir);
						string html_source_file = html_source_dir + "\\" + childPageCode + "_" + Web_Language_Enum_Converter.Enum_To_Code(UI_ApplicationCache_Gateway.Settings.Default_UI_Language) + ".html";
						if (!File.Exists(html_source_file))
						{
							HTML_Based_Content htmlContent = new HTML_Based_Content
							{
							    Content = "<br /><br />This is a new browse page.<br /><br />" + childPageLabel + "<br /><br />The code for this browse is: " + childPageCode, 
                                Author = RequestSpecificValues.Current_User.Full_Name, 
                                Date = DateTime.Now.ToLongDateString(), 
                                Title = childPageLabel
							};
						    htmlContent.Save_To_File(html_source_file);
						}
						newPage.Add_Static_HTML_Source("html\\browse\\" + childPageCode + "_" + Web_Language_Enum_Converter.Enum_To_Code(UI_ApplicationCache_Gateway.Settings.Default_UI_Language) + ".html", UI_ApplicationCache_Gateway.Settings.Default_UI_Language);

						itemAggregation.Add_Child_Page(newPage);

						// Save to the admins session
						HttpContext.Current.Session["Edit_Aggregation_" + itemAggregation.Code] = itemAggregation;

					}
				}
			}
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
		    if (itemAggregation.Child_Pages != null)
		    {
                foreach (Complete_Item_Aggregation_Child_Page childPage in itemAggregation.Child_Pages)
		        {
		            if (childPage.Source_Data_Type == Item_Aggregation_Child_Source_Data_Enum.Static_HTML)
		            {
		                sortedChildren.Add(childPage.Code, childPage);
		            }
		        }
		    }


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
			Output.WriteLine("          <td style=\"width:120px;\"><label for=\"admin_aggr_code\">Code:</label></td>");
			Output.WriteLine("          <td style=\"width:165px\"><input class=\"sbkSaav_NewChildCode sbkAdmin_Focusable\" name=\"admin_aggr_code\" id=\"admin_aggr_code\" type=\"text\" value=\"" + ( childPageCode ?? String.Empty ) + "\" /></td>");
			Output.WriteLine("          <td colspan=\"2\"><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + CODE_HELP + "');\"  title=\"" + CODE_HELP + "\" /></td>");
			Output.WriteLine("        </tr>");

			// Add the default language label
			Output.WriteLine("        <tr>");
			Output.WriteLine("          <td><label for=\"admin_aggr_label\">Title (default):</label></td>");
			Output.WriteLine("          <td colspan=\"2\"><input class=\"sbkSaav_SubLargeInput sbkAdmin_Focusable\" name=\"admin_aggr_label\" id=\"admin_aggr_label\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(childPageLabel ?? String.Empty) + "\" /></td>");
			Output.WriteLine("          <td style=\"width:30px\"><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + LABEL_HELP + "');\"  title=\"" + LABEL_HELP + "\" /></td>");
			Output.WriteLine("        </tr>");

			// Add the visibility line
			Output.WriteLine("        <tr>");
			Output.WriteLine("          <td><label for=\"admin_aggr_visibility\">Visibility:</label></td>");
			Output.Write("          <td><select class=\"sbkSaav_SubTypeSelect\" name=\"admin_aggr_visibility\" id=\"admin_aggr_visibility\" onchange=\"admin_aggr_child_page_visibility_change();\">");
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
				Output.WriteLine("        <tr id=\"admin_aggr_parent_row\" style=\"display:table-row\">");
			else
				Output.WriteLine("        <tr id=\"admin_aggr_parent_row\" style=\"display:none\">");

			Output.WriteLine("          <td><label for=\"admin_aggr_parent\">Parent:</label></td>");
			Output.Write("          <td><select class=\"sbkSaav_SubTypeSelect\" name=\"admin_aggr_parent\" id=\"admin_aggr_parent\">");
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
            if (HttpContext.Current.Session[itemAggregation.Code + "|Uploads"] != null)
            {
                string files = HttpContext.Current.Session[itemAggregation.Code + "|Uploads"].ToString().Replace("|", ", ");
                SobekCM_Database.Save_Item_Aggregation_Milestone(itemAggregation.Code, "Uploaded file(s) " + files, RequestSpecificValues.Current_User.Full_Name);
                HttpContext.Current.Session.Remove(itemAggregation.Code + "|Uploads");
            }
            string action = Form["admin_webcontent_action"];
            if ((action.Length > 0) && ( action.IndexOf("delete_") == 0))
            {
                string file = action.Substring(7);
                string path_file = aggregationDirectory + "\\uploads\\" + file;
                if (File.Exists(path_file))
                {
                    try
                    {
                        File.Delete(path_file);
                        SobekCM_Database.Save_Item_Aggregation_Milestone(itemAggregation.Code, "Deleted upload file " + file, RequestSpecificValues.Current_User.Full_Name);
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

            string uploads_dir = aggregationDirectory + "\\uploads";
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
                        string thisImageFile_URL = RequestSpecificValues.Current_Mode.Base_URL + "design/aggregations/" + itemAggregation.Code + "/uploads/" + thisImageFile;
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
                        string thisDocFile_URL = RequestSpecificValues.Current_Mode.Base_URL + "design/aggregations/" + itemAggregation.Code + "/uploads/" + thisDocFile;

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

		#region Methods to render (and parse) CSS page

		private void Save_Page_CSS_Postback(NameValueCollection Form)
		{
			// Check for action flag
			string action = Form["admin_webcontent_action"];
			if (action == "save_css")
			{
				string css_contents = Form["admin_aggr_css_edit"].Trim();
				if ( css_contents.Length == 0 )
					css_contents = "/**  Aggregation-level CSS for " + itemAggregation.Code + " **/";
			    string file = aggregationDirectory + "\\" + itemAggregation.Code + ".css";
                
                // Just in case there was a custom CSS referenced
			    if (!String.IsNullOrEmpty(itemAggregation.CSS_File))
			    {
			        file = aggregationDirectory + "\\" + itemAggregation.CSS_File;
			    }
			    else // this WAS null.. so actually assign this back
			    {
                    itemAggregation.CSS_File = itemAggregation.Code + ".css";
			    }
				StreamWriter writer = new StreamWriter(file, false);
				writer.WriteLine(css_contents);
				writer.WriteLine();
				writer.Flush();
				writer.Close();
			}
		}

		private void Add_Page_CSS(TextWriter Output)
		{
			// Get the CSS file's contents
			string css_contents;
			string file = aggregationDirectory + "\\" + itemAggregation.CSS_File;
			if (File.Exists(file))
			{
				StreamReader reader = new StreamReader(file);
				css_contents = reader.ReadToEnd();
				reader.Close();
			}
			else
			{
				css_contents = "/**  Aggregation-level CSS for " + itemAggregation.Code + " **/";
			}

			Output.WriteLine("<table class=\"sbkAdm_PopupTable\">");

			Output.WriteLine("  <tr class=\"sbkSaav_TitleRow\"><td colspan=\"3\">Aggregation-level Custom Stylesheet (CSS)</td></tr>");
			Output.WriteLine("  <tr class=\"sbkSaav_TextRow\"><td colspan=\"3\"><p>You can edit the contents of the aggregation-level custom stylesheet (css) file here.  Click SAVE when complete to return to the main aggregation administration screen.</p><p>NOTE: You may need to refresh your browser for your changes to take affect.</p></td></tr>");

			// Add the css edit textarea code
			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\" >");
			Output.WriteLine("    <td style=\"width:40px;\">&nbsp;</td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <textarea class=\"sbkSaav_EditCssTextarea sbkAdmin_Focusable\" id=\"admin_aggr_css_edit\" name=\"admin_aggr_css_edit\">");
			Output.WriteLine(css_contents);
			Output.WriteLine("      </textarea>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");


			// Add the button line
			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\" style=\"height:60px\">");
			Output.WriteLine("    <td>&nbsp;</td>");
			Output.WriteLine("    <td style=\"text-align:right; padding-right: 100px\">");
			Output.WriteLine("      <button title=\"Do not apply changes\" class=\"sbkAdm_RoundButton\" onclick=\"return new_webcontent_edit_page('e');\"><img src=\"" + Static_Resources.Button_Previous_Arrow_Png + "\" class=\"sbkAdm_RoundButton_LeftImg\" alt=\"\" /> CANCEL</button> &nbsp; &nbsp; ");
			Output.WriteLine("      <button title=\"Save changes to this stylesheet\" class=\"sbkAdm_RoundButton\" onclick=\"return save_css_edits();\">SAVE <img src=\"" + Static_Resources.Button_Next_Arrow_Png + "\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button>");
			Output.WriteLine("    </td>");
			Output.WriteLine("  </tr>");


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
                    add_upload_controls(MainPlaceHolder, ".gif,.bmp,.jpg,.png,.jpeg,.ai,.doc,.docx,.eps,.kml,.pdf,.psd,.pub,.txt,.vsd,.vsdx,.xls,.xlsx,.xml,.zip", aggregationDirectory + "\\uploads", String.Empty, true, itemAggregation.Code + "|Uploads", Tracer);
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
