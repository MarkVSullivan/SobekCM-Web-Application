#region Using directives

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SobekCM.Core.Aggregations;
using SobekCM.Core.Client;
using SobekCM.Core.Configuration;
using SobekCM.Core.MemoryMgmt;
using SobekCM.Core.Navigation;
using SobekCM.Core.Search;
using SobekCM.Core.WebContent;
using SobekCM.Engine_Library.Aggregations;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Engine_Library.Database;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.Settings;
using SobekCM.Library.UI;
using SobekCM.Library.UploadiFive;
using SobekCM.Tools;
using Image = System.Drawing.Image;

#endregion

namespace SobekCM.Library.AdminViewer
{
	/// <summary> Class allows an authenticated aggregation admin to edit information related to a single item aggregation. </summary>
	/// <remarks> This class extends the <see cref="abstract_AdminViewer"/> class.<br /><br />
	/// MySobek Viewers are used for registration and authentication with mySobek, as well as performing any task which requires
	/// authentication, such as online submittal, metadata editing, and system administrative tasks.<br /><br />
	/// During a valid html request, the following steps occur:
	/// <ul>
	/// <li>Application state is built/verified by the Application_State_Builder </li>
	/// <li>Request is analyzed by the QueryString_Analyzer and output as a <see cref="Navigation_Object"/>  </li>
	/// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
	/// <li>The HTML writer will create the necessary subwriter.  Since this action requires authentication, an instance of the  <see cref="MySobek_HtmlSubwriter"/> class is created. </li>
	/// <li>The mySobek subwriter creates an instance of this viewer to view and edit information related to a single item aggregation</li>
	/// </ul></remarks>
	public class Aggregation_Single_AdminViewer : abstract_AdminViewer
	{
		private string actionMessage;
		private readonly string aggregationDirectory;
        private readonly Complete_Item_Aggregation itemAggregation;

		private readonly int page;

		private string childPageCode;
		private string childPageLabel;
		private string childPageVisibility;
		private string childPageParent;


		/// <summary> Constructor for a new instance of the Aggregation_Single_AdminViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
		/// <remarks> Postback from handling an edit or new aggregation is handled here in the constructor </remarks>
        public Aggregation_Single_AdminViewer(RequestCache RequestSpecificValues)  : base(RequestSpecificValues)
		{
            RequestSpecificValues.Tracer.Add_Trace("Aggregation_Single_AdminViewer.Constructor", String.Empty);

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
		        if (RequestSpecificValues.Current_Mode.My_Sobek_SubMode == "b")
		            page = 2;
		        else if (RequestSpecificValues.Current_Mode.My_Sobek_SubMode == "c")
		            page = 3;
		        else if (RequestSpecificValues.Current_Mode.My_Sobek_SubMode == "d")
		            page = 4;
		        else if (RequestSpecificValues.Current_Mode.My_Sobek_SubMode == "e")
		            page = 5;
		        else if (RequestSpecificValues.Current_Mode.My_Sobek_SubMode == "f")
		            page = 6;
		        else if (RequestSpecificValues.Current_Mode.My_Sobek_SubMode == "g")
		            page = 7;
		        else if (RequestSpecificValues.Current_Mode.My_Sobek_SubMode == "h")
		            page = 8;
		        else if (RequestSpecificValues.Current_Mode.My_Sobek_SubMode == "i")
		            page = 9;
		        else if (RequestSpecificValues.Current_Mode.My_Sobek_SubMode == "j")
		            page = 10;
		        else if (RequestSpecificValues.Current_Mode.My_Sobek_SubMode == "k")
		            page = 11;
		        else if (RequestSpecificValues.Current_Mode.My_Sobek_SubMode == "y")
		            page = 12;
		        else if (RequestSpecificValues.Current_Mode.My_Sobek_SubMode.IndexOf("g_") == 0)
		            page = 13;
		    }


		    // If this is a postback, handle any events first
			if (RequestSpecificValues.Current_Mode.isPostBack)
			{
				try
				{
					// Pull the standard values
					NameValueCollection form = HttpContext.Current.Request.Form;

					// Get the curret action
					string action = form["admin_aggr_save"];

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
							Save_Page_1_Postback(form);
							break;

						case 2:
							Save_Page_2_Postback(form);
							break;

						case 3:
							Save_Page_3_Postback(form);
							break;

						case 4:
							Save_Page_4_Postback(form);
							break;

						case 5:
							Save_Page_Appearance_Postback(form);
							break;

						case 6:
							Save_Page_6_Postback();
							break;

						case 7:
							Save_Page_7_Postback(form);
							break;

						case 8:
							Save_Page_8_Postback(form);
							break;

                        case 9:
                            Save_Page_Uploads_Postback(form);
                            break;

						case 12:
							Save_Page_CSS_Postback(form);
							break;

						case 13:
							Save_Child_Page_Postback(form);
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
			get { return itemAggregation != null ? "Edit " + itemAggregation.ShortName : "Edit Item Aggregation"; }
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
			Tracer.Add_Trace("Aggregation_Single_AdminViewer.Write_HTML", "Do nothing");
		}

		/// <summary> This is an opportunity to write HTML directly into the main form before any controls are placed in the main place holder </summary>
		/// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <remarks> This text will appear within the ItemNavForm form tags </remarks>
		public override void Write_ItemNavForm_Opening(TextWriter Output, Custom_Tracer Tracer)
		{
			Tracer.Add_Trace("Aggregation_Single_AdminViewer.Write_ItemNavForm_Opening", "Add the majority of the HTML before the placeholder");

			// Add the hidden field
			Output.WriteLine("<!-- Hidden field is used for postbacks to indicate what to save and reset -->");
			Output.WriteLine("<input type=\"hidden\" id=\"admin_aggr_reset\" name=\"admin_aggr_reset\" value=\"\" />");
			Output.WriteLine("<input type=\"hidden\" id=\"admin_aggr_save\" name=\"admin_aggr_save\" value=\"\" />");
			Output.WriteLine("<input type=\"hidden\" id=\"admin_aggr_action\" name=\"admin_aggr_action\" value=\"\" />");
			Output.WriteLine(); 

			Tracer.Add_Trace("Aggregation_Single_AdminViewer.Write_ItemNavForm_Closing", "Add the rest of the form");

			Output.WriteLine("<!-- Users_AdminViewer.Write_ItemNavForm_Closing -->");

			Output.WriteLine("<script src=\"" + Static_Resources.Sobekcm_Admin_Js + "\" type=\"text/javascript\"></script>");
			Output.WriteLine();

			Output.WriteLine("<div id=\"sbkSaav_PageContainer\">");

			// Add the buttons (unless this is a sub-page like editing the CSS file)
			if (page < 12)
			{
				string last_mode = RequestSpecificValues.Current_Mode.My_Sobek_SubMode;
				RequestSpecificValues.Current_Mode.My_Sobek_SubMode = String.Empty;
				Output.WriteLine("  <div class=\"sbkSaav_ButtonsDiv\">");
				Output.WriteLine("    <button title=\"Do not apply changes\" class=\"sbkAdm_RoundButton\" onclick=\"return new_aggr_edit_page('z');\"><img src=\"" + Static_Resources.Button_Previous_Arrow_Png + "\" class=\"sbkAdm_RoundButton_LeftImg\" alt=\"\" /> CANCEL</button> &nbsp; &nbsp; ");
                Output.WriteLine("    <button title=\"Save changes to this item Aggregation\" class=\"sbkAdm_RoundButton\" onclick=\"return save_aggr_edits(false);\"> SAVE </button> &nbsp; &nbsp; ");
				Output.WriteLine("    <button title=\"Save changes to this item Aggregation and exit the admin screens\" class=\"sbkAdm_RoundButton\" onclick=\"return save_aggr_edits(true);\">SAVE & EXIT <img src=\"" + Static_Resources.Button_Next_Arrow_Png + "\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button>");
				Output.WriteLine("  </div>");
				Output.WriteLine();
				RequestSpecificValues.Current_Mode.My_Sobek_SubMode = last_mode;
			}
			else if (page == 13)
			{
				Output.WriteLine("  <div class=\"sbkSaav_ButtonsDiv\">");
				Output.WriteLine("    <button title=\"Close this child page details and return to main admin pages\" class=\"sbkAdm_RoundButton\" onclick=\"return new_aggr_edit_page('g');\"><img src=\"" + Static_Resources.Button_Previous_Arrow_Png + "\" class=\"sbkAdm_RoundButton_LeftImg\" alt=\"\" /> BACK </button>"); 
				Output.WriteLine("  </div>");
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

				const string GENERAL = "General";
				const string SEARCH = "Search";
				const string RESULTS = "Results";
				const string METADATA = "Metadata";
				const string APPEARANCE = "Appearance";
				const string HIGHLIGHTS = "Highlights";
				const string STATIC_PAGES = "Child Pages";
				const string SUBCOLLECTIONS = "SubCollections";
                const string UPLOADS = "Uploads";

				// Draw all the page tabs for this form
				if (page == 1)
				{
					Output.WriteLine("    <li id=\"tabHeader_1\" class=\"tabActiveHeader\">" + GENERAL + "</li>");
				}
				else
				{
					Output.WriteLine("    <li id=\"tabHeader_1\" onclick=\"return new_aggr_edit_page('a');\">" + GENERAL + "</li>");
				}

                if (page == 5)
                {
                    Output.WriteLine("    <li id=\"tabHeader_4\" class=\"tabActiveHeader\">" + APPEARANCE + "</li>");
                }
                else
                {
                    Output.WriteLine("    <li id=\"tabHeader_4\" onclick=\"return new_aggr_edit_page('e');\">" + APPEARANCE + "</li>");
                }

				if (page == 2)
				{
					Output.WriteLine("    <li id=\"tabHeader_2\" class=\"tabActiveHeader\">" + SEARCH + "</li>");
				}
				else
				{
					Output.WriteLine("    <li id=\"tabHeader_2\" onclick=\"return new_aggr_edit_page('b');\">" + SEARCH + "</li>");
				}

				if (page == 3)
				{
					Output.WriteLine("    <li id=\"tabHeader_2\" class=\"tabActiveHeader\">" + RESULTS + "</li>");
				}
				else
				{
					Output.WriteLine("    <li id=\"tabHeader_2\" onclick=\"return new_aggr_edit_page('c');\">" + RESULTS + "</li>");
				}

				if (page == 4)
				{
					Output.WriteLine("    <li id=\"tabHeader_3\" class=\"tabActiveHeader\">" + METADATA + "</li>");
				}
				else
				{
					Output.WriteLine("    <li id=\"tabHeader_3\" onclick=\"return new_aggr_edit_page('d');\">" + METADATA + "</li>");
				}

			    if ((itemAggregation.Highlights != null ) && ( itemAggregation.Highlights.Count > 0))
			    {
			        if (page == 6)
			        {
			            Output.WriteLine("    <li id=\"tabHeader_5\" class=\"tabActiveHeader\">" + HIGHLIGHTS + "</li>");
			        }
			        else
			        {
			            Output.WriteLine("    <li id=\"tabHeader_5\" onclick=\"return new_aggr_edit_page('f');\">" + HIGHLIGHTS + "</li>");
			        }
			    }

			    if (page == 7)
				{
					Output.WriteLine("    <li id=\"tabHeader_6\" class=\"tabActiveHeader\">" + STATIC_PAGES + "</li>");
				}
				else
				{
					Output.WriteLine("    <li id=\"tabHeader_6\" onclick=\"return new_aggr_edit_page('g');\">" + STATIC_PAGES + "</li>");
				}

			    if (itemAggregation.Code.ToLower() != "all")
			    {
			        if (page == 8)
			        {
			            Output.WriteLine("    <li id=\"tabHeader_6\" class=\"tabActiveHeader\">" + SUBCOLLECTIONS + "</li>");
			        }
			        else
			        {
			            Output.WriteLine("    <li id=\"tabHeader_6\" onclick=\"return new_aggr_edit_page('h');\">" + SUBCOLLECTIONS + "</li>");
			        }
			    }
                if (page == 9)
                {
                    Output.WriteLine("    <li id=\"tabHeader_6\" class=\"tabActiveHeader\">" + UPLOADS + "</li>");
                }
                else
                {
                    Output.WriteLine("    <li id=\"tabHeader_6\" onclick=\"return new_aggr_edit_page('i');\">" + UPLOADS + "</li>");
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
					Add_Page_1(Output );
					break;

				case 2:
					Add_Page_2(Output);
					break;

				case 3:
					Add_Page_3(Output);
					break;

				case 4:
					Add_Page_4(Output);
					break;

				case 5:
					Add_Page_Appearance(Output);
					break;
                
				case 6:
					Add_Page_6(Output);
					break;

				case 7:
					Add_Page_7(Output);
					break;

				case 8:
					Add_Page_8(Output);
					break;

                case 9:
                    Add_Page_Uploads(Output);
                    break;

				case 12:
					Add_Page_CSS(Output);
					break;

				case 13:
					Add_Child_Page(Output);
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
			 Tracer.Add_Trace("Aggregation_Single_AdminViewer.Write_ItemNavForm_Closing", "Add any html after the placeholder and close tabs");

			switch (page)
			{
				case 1:
					Finish_Page_1(Output);
					break;

				case 5:
					Finish_Page_5(Output);
					break;

                case 9:
                    Finish_Page_Uploads(Output);
                    break;
			}


			 Output.WriteLine("    </div>");
			 Output.WriteLine("  </div>");
			 Output.WriteLine("</div>");
			 Output.WriteLine("<br />");
		}

		#region Methods to render (and parse) page 1 - Basic Information

		private void Save_Page_1_Postback(NameValueCollection Form)
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

		private void Add_Page_1( TextWriter Output )
		{
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




		}

		private void Finish_Page_1(TextWriter Output)
		{
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


        #region Methods to render (and parse) -  Appearance

        private void Save_Page_Appearance_Postback(NameValueCollection Form)
        {
            // Log any uploaded banners
            if (HttpContext.Current.Session[itemAggregation.Code + "|Banners"] != null)
            {
                string files = HttpContext.Current.Session[itemAggregation.Code + "|Banners"].ToString().Replace("|", ", ");
                SobekCM_Database.Save_Item_Aggregation_Milestone(itemAggregation.Code, "Added banner file " + files, RequestSpecificValues.Current_User.Full_Name);
                HttpContext.Current.Session.Remove(itemAggregation.Code + "|Banners");
            }

            // Some interesting custom actions on this page, so get the actions
            // query string first
            string action = Form["admin_aggr_action"];
            if (action.Length > 0)
            {
                switch (action)
                {
                    case "enable_css":
                        itemAggregation.CSS_File = itemAggregation.Code + ".css";
                        string file = aggregationDirectory + "\\" + itemAggregation.CSS_File;
                        if (!File.Exists(file))
                        {
                            StreamWriter writer = new StreamWriter(file);
                            writer.WriteLine("/**  Aggregation-level CSS for " + itemAggregation.Code + " **/");
                            writer.WriteLine();
                            writer.Flush();
                            writer.Close();
                        }
                        break;

                    case "disable_css":
                        itemAggregation.CSS_File = null;
                        break;

                    case "add_home":
                        string language = Form["admin_aggr_new_home_lang"];
                        string copyFrom = Form["admin_aggr_new_home_copy"];
                        if (language.Length > 0)
                        {
                            Web_Language_Enum enumVal = Web_Language_Enum_Converter.Code_To_Enum(language);
                            string new_file_name = "text_" + language.ToLower() + ".html";
                            string new_file = aggregationDirectory + "\\html\\home\\" + new_file_name;
                            if (!Directory.Exists(aggregationDirectory + "\\html\\home"))
                                Directory.CreateDirectory(aggregationDirectory + "\\html\\home");
                            bool created_exists = false;
                            if (copyFrom.Length > 0)
                            {
                                string copy_file = aggregationDirectory + "\\" + copyFrom;
                                if (File.Exists(copy_file))
                                {
                                    File.Copy(copy_file, new_file, true);
                                    created_exists = true;
                                }
                            }
                            if ((!created_exists) && (!File.Exists(new_file)))
                            {
                                StreamWriter writer = new StreamWriter(new_file);
                                writer.WriteLine("New home page text in " + language + " goes here.");
                                writer.Flush();
                                writer.Close();
                            }

                            itemAggregation.Add_Home_Page_File("html\\home\\" + new_file_name, enumVal, false);

                            // Add this to the list of JUST ADDED home pages, which can't be edited or viewed until saved
                            List<Web_Language_Enum> newLanguages = HttpContext.Current.Session["Item_Aggr_Edit_" + itemAggregation.Code + "_NewLanguages"] as List<Web_Language_Enum> ?? new List<Web_Language_Enum>();
                            newLanguages.Add(enumVal);
                            HttpContext.Current.Session["Item_Aggr_Edit_" + itemAggregation.Code + "_NewLanguages"] = newLanguages;
                        }
                        break;

                    case "add_banner":
                        string blanguage = Form["admin_aggr_new_banner_lang"];
                        string bfile = Form["admin_aggr_new_banner_image"];
                        string btype = Form["admin_aggr_new_banner_type"];
                        if (blanguage.Length > 0)
                        {
                            Web_Language_Enum enumVal = Web_Language_Enum_Converter.Code_To_Enum(blanguage);
                            if (btype == "standard")
                            {
                                itemAggregation.Add_Banner_Image("images\\banners\\" + bfile, enumVal);
                            }
                            else
                            {
                                Item_Aggregation_Front_Banner_Type_Enum btypeEnum = Item_Aggregation_Front_Banner_Type_Enum.Full;
                                if (btype == "left")
                                    btypeEnum = Item_Aggregation_Front_Banner_Type_Enum.Left;
                                if (btype == "right")
                                    btypeEnum = Item_Aggregation_Front_Banner_Type_Enum.Right;
                                Item_Aggregation_Front_Banner newFront = new Item_Aggregation_Front_Banner("images\\banners\\" + bfile) { Type = btypeEnum };

                                try
                                {
                                    string banner_file = aggregationDirectory + "\\images\\banners\\" + bfile;
                                    if (File.Exists(banner_file))
                                    {
                                        using (Image bannerImage = Image.FromFile(banner_file))
                                        {
                                            newFront.Width = (ushort)bannerImage.Width;
                                            newFront.Height = (ushort)bannerImage.Height;
                                            bannerImage.Dispose();
                                        }
                                    }
                                }
                                catch (Exception)
                                {

                                }


                                itemAggregation.Add_Front_Banner_Image(newFront, enumVal);
                            }
                        }


                        break;

                    default:
                        if (action.IndexOf("delete_home_") == 0)
                        {
                            string code_to_delete = action.Replace("delete_home_", "");
                            Web_Language_Enum enum_to_delete = Web_Language_Enum_Converter.Code_To_Enum(code_to_delete);
                            itemAggregation.Delete_Home_Page(enum_to_delete);
                        }
                        if (action.IndexOf("delete_standard_") == 0)
                        {
                            string code_to_delete = action.Replace("delete_standard_", "");
                            Web_Language_Enum enum_to_delete = Web_Language_Enum_Converter.Code_To_Enum(code_to_delete);
                            if (itemAggregation.Banner_Dictionary != null)
                                itemAggregation.Banner_Dictionary.Remove(enum_to_delete);
                        }
                        if (action.IndexOf("delete_front_") == 0)
                        {
                            string code_to_delete = action.Replace("delete_front_", "");
                            Web_Language_Enum enum_to_delete = Web_Language_Enum_Converter.Code_To_Enum(code_to_delete);
                            if (itemAggregation.Front_Banner_Dictionary != null)
                                itemAggregation.Front_Banner_Dictionary.Remove(enum_to_delete);
                        }
                        if ((action.Length > 0) && (action.IndexOf("delete_image_") == 0))
                        {
                            string banner_file = action.Replace("delete_image_", "");
                            string path_file = aggregationDirectory + "\\images\\banners\\" + banner_file;
                            if (File.Exists(path_file))
                            {
                                try
                                {
                                    File.Delete(path_file);
                                    SobekCM_Database.Save_Item_Aggregation_Milestone(itemAggregation.Code, "Deleted unused banner file " + banner_file, RequestSpecificValues.Current_User.Full_Name);
                                }
                                catch
                                {
                                }
                            }
                        }
                        if ((action.IndexOf("customize_") == 0) || (action.IndexOf("uncustomize_") == 0))
                        {
                            string code = action.Replace("uncustomize_", "").Replace("customize_", "");
                            Web_Language_Enum asEnum = Web_Language_Enum_Converter.Code_To_Enum(code);
                            if (itemAggregation.Home_Page_File_Dictionary.ContainsKey(asEnum))
                            {
                                itemAggregation.Home_Page_File_Dictionary[asEnum].isCustomHome = (action.IndexOf("uncustomize_") != 0);
                            }
                        }

                        break;

                }
            }

            // Set the web skin
            itemAggregation.Web_Skins = null;
            itemAggregation.Default_Skin = null;
            foreach (string thisKey in Form.AllKeys)
            {
                if ((thisKey.IndexOf("admin_aggr_skin_") == 0) && ( Form[thisKey] != null ) && ( Form[thisKey].Length > 0 ))
                {
                    if (itemAggregation.Web_Skins == null)
                        itemAggregation.Web_Skins = new List<string>();
                    itemAggregation.Web_Skins.Add(Form[thisKey]);
                    if (String.IsNullOrEmpty(itemAggregation.Default_Skin))
                        itemAggregation.Default_Skin = Form[thisKey];
                }
            }
            //if ((Form["admin_aggr_skin_1"] != null) && (Form["admin_aggr_skin_1"].Length > 0))
            //{
            //    itemAggregation.Web_Skins = new List<string> { Form["admin_aggr_skin_1"] };
            //    itemAggregation.Default_Skin = Form["admin_aggr_skin_1"];
            //}
        }


	    private void Add_Page_Appearance(TextWriter Output)
	    {
	        // Help constants (for now)
	        const string WEB_SKIN_HELP = "This collection can be forced to only display under specific web skins by selecting it here.  If there are no web skins selected, the current web skin (determined by the URL portal) will be used.  Otherwise, if the current web skin is not in this list, the first web skin in this list will be used.";
	        const string CSS_HELP = "You can add style definitions to this collection by enabling and editing the collection-level css stylesheet here.";
	        const string NEW_HOME_PAGE_HELP = "Use this option to add special support for a new language to your home page.  For example, if you have just translated your home page text into Spanish, and would like users that have set Spanish as their browser preference to see your translations by default, select Spanish from the drop down.  Choosing a home page to copy from will allow the new home page to not start completely blank.";
	        const string NEW_BANNER_HELP = "Select the language and the already uploaded banner to use and click ADD.  This will allow you to customize the banner image, based on the language preference of your web user.  To change an existing banner, simply select the existing language and the new banner image and press ADD.";
	        const string UPLOAD_BANNER_HELP = "Before you can choose to use a new banner image for a language, you must upload the new banner.  Pressing the large SELECT button in this section allows you to upload this new image, or overwrite an existing banner image.";



	        Output.WriteLine("<table class=\"sbkAdm_PopupTable\">");

	        Output.WriteLine("  <tr class=\"sbkSaav_TitleRow\"><td colspan=\"3\">Appearance Options</td></tr>");
	        Output.WriteLine("  <tr class=\"sbkSaav_TextRow\"><td colspan=\"3\"><p>These three settings have the most profound affects on the appearance of this aggregation, by forcing it to appear under a particular web skin, allowing a custom aggregation-level stylesheet, or completely overriding the system-generated home page for a custom home page HTML source file.</p><p>For more information about the settings on this tab, <a href=\"" + UI_ApplicationCache_Gateway.Settings.Help_URL(RequestSpecificValues.Current_Mode.Base_URL) + "adminhelp/singleaggr\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</p></td></tr>");

	        // Add the web skin code
            int skin_inputs = 5;
            if ((itemAggregation.Web_Skins != null) && (itemAggregation.Web_Skins.Count > 4))
                skin_inputs = itemAggregation.Web_Skins.Count + 1;

	        Output.WriteLine(skin_inputs > 5 ? "  <tr class=\"sbkSaav_TallRow\">" : "  <tr class=\"sbkSaav_SingleRow\" >");
	        Output.WriteLine("    <td style=\"width:50px;\">&nbsp;</td>");
	        Output.WriteLine("    <td class=\"sbkSaav_TableLabel\" style=\"width:140px\">Web Skin(s):</label></td>");
	        Output.WriteLine("    <td>");
	        Output.WriteLine("      <table class=\"sbkSaav_InnerTable\">");
	        Output.WriteLine("      <tr>");
	        Output.WriteLine("        <td>");

	        // Get the ordered list of all skin codes
	        List<string> skinCodes = UI_ApplicationCache_Gateway.Web_Skin_Collection.Ordered_Skin_Codes;
	        for (int i = 0; i < skin_inputs; i++) // itemAggregation.Web_Skins.Count + 5; i++)
	        {
	            string skin = String.Empty;
	            if ((itemAggregation.Web_Skins != null) && (i < itemAggregation.Web_Skins.Count))
	                skin = itemAggregation.Web_Skins[i];
	            Skin_Writer_Helper(Output, "admin_aggr_skin_" + (i + 1), skin, skinCodes);
	            if ((i + 1)%5 == 0)
	                Output.WriteLine("<br />");
	        }
	        Output.WriteLine("        </td>");
	        Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + WEB_SKIN_HELP + "');\"  title=\"" + WEB_SKIN_HELP + "\" /></td></tr></table>");
	        Output.WriteLine("     </td>");
	        Output.WriteLine("  </tr>");


	        // Add the css line
	        Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
	        Output.WriteLine("    <td style=\"width:50px;\">&nbsp;</td>");
	        Output.WriteLine("    <td class=\"sbkSaav_TableLabel\" style=\"width:140px\"><label for=\"admin_aggr_shortname\">Custom Stylesheet:</label></td>");
	        Output.WriteLine("    <td>");
	        Output.WriteLine("      <table class=\"sbkSaav_InnerTable\">");
	        Output.WriteLine("        <tr>");

	        if (String.IsNullOrEmpty(itemAggregation.CSS_File))
	        {
	            Output.WriteLine("          <td><span style=\"font-style:italic; padding-right:20px;\">No custom aggregation-level stylesheet</span></td>");
	            Output.WriteLine("          <td><button title=\"Enable an aggregation-level stylesheet\" class=\"sbkAdm_RoundButton\" onclick=\"return aggr_edit_enable_css();\">ENABLE</button></td>");
	        }
	        else
	        {
	            string css_url = RequestSpecificValues.Current_Mode.Base_Design_URL + "aggregations/" + itemAggregation.Code + "/" + itemAggregation.CSS_File;
	            Output.WriteLine("          <td style=\"padding-right:20px;\"><a href=\"" + css_url + "\" title=\"View CSS file\" target=\"" + itemAggregation.CSS_File + "\">" + itemAggregation.CSS_File + "</a></td>");
	            Output.WriteLine("          <td style=\"padding-right:10px;\"><button title=\"Disable this aggregation-level stylesheet\" class=\"sbkAdm_RoundButton\" onclick=\"return aggr_edit_disable_css();\">DISABLE</button></td>");
	            Output.WriteLine("          <td><button title=\"Edit this aggregation-level stylesheet\" class=\"sbkAdm_RoundButton\" onclick=\"return new_aggr_edit_page('y');\">EDIT</button></td>");
	        }
	        Output.WriteLine("          <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + CSS_HELP + "');\"  title=\"" + CSS_HELP + "\" /></td>");
	        Output.WriteLine("        </tr>");
	        Output.WriteLine("      </table>");
	        Output.WriteLine("    </td>");
	        Output.WriteLine("  </tr>");

	        Output.WriteLine("  <tr class=\"sbkSaav_TitleRow2\"><td colspan=\"3\">Home Page Text</td></tr>");
	        Output.WriteLine("  <tr class=\"sbkSaav_TextRow\"><td colspan=\"3\"><p>This section controls all the language-specific (and default) text which appears on the home page.</p></td></tr>");

	        // Add all the existing home page information
	        Output.WriteLine("  <tr class=\"sbkSaav_TallRow\">");
	        Output.WriteLine("    <td>&nbsp;</td>");
	        Output.WriteLine("    <td class=\"sbkSaav_TableLabel2\">Existing Home Pages:</td>");
	        Output.WriteLine("    <td>");

	        Output.WriteLine("      <table class=\"sbkSaav_HomeTable sbkSaav_Table\">");
	        Output.WriteLine("        <tr>");
	        Output.WriteLine("          <th class=\"sbkSaav_HomeTableHeader1\">LANGUAGE</th>");
	        Output.WriteLine("          <th class=\"sbkSaav_HomeTableHeader2\">SOURCE FILE</th>");
	        Output.WriteLine("          <th class=\"sbkSaav_HomeTableHeader3\">ACTIONS</th>");
	        Output.WriteLine("          <th class=\"sbkSaav_HomeTableHeader3\">CUSTOM</th>");
	        Output.WriteLine("        </tr>");

	        // Get the list of all recently added home page languages
	        List<Web_Language_Enum> newLanguages = HttpContext.Current.Session["Item_Aggr_Edit_" + itemAggregation.Code + "_NewLanguages"] as List<Web_Language_Enum> ?? new List<Web_Language_Enum>();

	        // Add all the home page information
	        Web_Language_Enum currLanguage = RequestSpecificValues.Current_Mode.Language;
	        RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Aggregation;
	        List<string> existing_languages = new List<string>();
	        if (itemAggregation.Home_Page_File_Dictionary != null)
	        {
	            foreach (KeyValuePair<Web_Language_Enum, Complete_Item_Aggregation_Home_Page> thisHomeSource in itemAggregation.Home_Page_File_Dictionary)
	            {
	                Output.WriteLine("        <tr>");
	                bool canDelete = true;
	                if ((thisHomeSource.Key == Web_Language_Enum.DEFAULT) || (thisHomeSource.Key == Web_Language_Enum.UNDEFINED) || (thisHomeSource.Key == UI_ApplicationCache_Gateway.Settings.Default_UI_Language))
	                {
	                    canDelete = false;
	                    existing_languages.Add(Web_Language_Enum_Converter.Enum_To_Name(UI_ApplicationCache_Gateway.Settings.Default_UI_Language));
	                    Output.WriteLine("          <td style=\"font-style:italic; padding-left:5px;\">default</td>");
	                }
	                else
	                {
	                    existing_languages.Add(Web_Language_Enum_Converter.Enum_To_Name(thisHomeSource.Key));
	                    Output.WriteLine("          <td style=\"padding-left:5px;\">" + Web_Language_Enum_Converter.Enum_To_Name(thisHomeSource.Key) + "</td>");
	                }

	                string file = RequestSpecificValues.Current_Mode.Base_Design_URL + "aggregations/" + itemAggregation.Code + "/" + thisHomeSource.Value.Source.Replace("\\", "/");

	                Output.WriteLine("          <td><a href=\"" + file + "\" title=\"View source file\">" + thisHomeSource.Value.Source.Replace("html\\home\\", "") + "</a></td>");
	                Output.Write("          <td class=\"sbkAdm_ActionLink\" >( ");

	                if (!newLanguages.Contains(thisHomeSource.Key))
	                {
	                    if (canDelete)
	                    {
	                        RequestSpecificValues.Current_Mode.Language = thisHomeSource.Key;
	                        RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Home;
	                        Output.Write("<a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\" title=\"View this home page in " + Web_Language_Enum_Converter.Enum_To_Name(thisHomeSource.Key) + "\" target=\"VIEW" + itemAggregation.Code + "_" + Web_Language_Enum_Converter.Enum_To_Code(thisHomeSource.Key) + "\">view</a> | ");

	                        RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Home_Edit;
	                        Output.Write("<a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\" title=\"Edit this home page in " + Web_Language_Enum_Converter.Enum_To_Name(thisHomeSource.Key) + "\" target=\"EDIT" + itemAggregation.Code + "_" + Web_Language_Enum_Converter.Enum_To_Code(thisHomeSource.Key) + "\">edit</a> ");
	                    }
	                    else
	                    {
	                        RequestSpecificValues.Current_Mode.Language = thisHomeSource.Key;
	                        RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Home;
	                        Output.Write("<a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\" title=\"View this home page\" target=\"VIEW" + itemAggregation.Code + "_" + Web_Language_Enum_Converter.Enum_To_Code(thisHomeSource.Key) + "\">view</a> | ");

	                        RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Home_Edit;
	                        Output.Write("<a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\" title=\"Edit this home page\" target=\"EDIT" + itemAggregation.Code + "_" + Web_Language_Enum_Converter.Enum_To_Code(thisHomeSource.Key) + "\">edit</a> ");
	                    }
	                }
	                else
	                {
	                    Output.Write("<a href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "l/technical/javascriptrequired\" onclick=\"alert('You must SAVE your changes before you can view or edit newly added home pages.');return false\">view</a> | ");
	                    Output.Write("<a href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "l/technical/javascriptrequired\" onclick=\"alert('You must SAVE your changes before you can view or edit newly added home pages.');return false\">edit</a> ");
	                }

	                if (canDelete)
	                {
	                    Output.Write("| <a  href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return aggr_edit_delete_home('" + Web_Language_Enum_Converter.Enum_To_Code(thisHomeSource.Key) + "');\" title=\"Delete this " + Web_Language_Enum_Converter.Enum_To_Name(thisHomeSource.Key) + " home page\" >delete</a> ");
	                }

	                Output.WriteLine(" )</td>");

	                // Add checkbox for language home page being custom
	                string langCode = Web_Language_Enum_Converter.Enum_To_Code(thisHomeSource.Key);
	                string langTerm = Web_Language_Enum_Converter.Enum_To_Name(thisHomeSource.Key);
	                Output.Write("          <td><input type=\"checkbox\" id=\"custom_" + langCode + "_check\" name=\"custom_" + langCode + "_check\" ");
	                if (thisHomeSource.Value.isCustomHome)
	                    Output.Write("checked=\"checked\" onclick=\"return change_custom_home_flag('" + langTerm + "','" + langCode + "', true);\" ");
	                else
	                    Output.Write("onclick=\"return change_custom_home_flag('" + langTerm + "','" + langCode + "', false);\" ");
	                Output.WriteLine("/></td>");
	                Output.WriteLine("        </tr>");
	            }
	        }
	        Output.WriteLine("      </table>");
	        Output.WriteLine("    </td>");
	        Output.WriteLine("  </tr>");
	        RequestSpecificValues.Current_Mode.Language = currLanguage;
	        RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Administrative;

	        // Write the add new home page information
	        Output.WriteLine("  <tr class=\"sbkSaav_TallRow\">");
	        Output.WriteLine("    <td>&nbsp;</td>");
	        Output.WriteLine("    <td class=\"sbkSaav_TableLabel2\">New Home Page:</td>");
	        Output.WriteLine("    <td>");
	        Output.WriteLine("      <table class=\"sbkSaav_InnerTable\">");
	        Output.WriteLine("      <tr>");
	        Output.WriteLine("        <td>");

	        Output.Write("          <select class=\"sbkSaav_SelectSingle\" id=\"admin_aggr_new_home_lang\" name=\"admin_aggr_new_home_lang\">");

	        // Add each language in the combo box
	        foreach (string possible_language in Web_Language_Enum_Converter.Language_Name_Array)
	        {
	            if (!existing_languages.Contains(possible_language))
	                Output.Write("<option value=\"" + Web_Language_Enum_Converter.Name_To_Code(possible_language) + "\">" + HttpUtility.HtmlEncode(possible_language) + "</option>");
	        }
	        Output.WriteLine();
	        Output.WriteLine("        </td>");
	        Output.WriteLine("        <td style=\"padding-left:35px;\">Copy from existing home: </td>");
	        Output.WriteLine("        <td>");
	        Output.Write("          <select id=\"admin_aggr_new_home_copy\" name=\"admin_aggr_new_home_copy\">");
	        Output.Write("<option value=\"\" selected=\"selected\"></option>");
	        if (itemAggregation.Home_Page_File_Dictionary != null)
	        {
	            foreach (KeyValuePair<Web_Language_Enum, Complete_Item_Aggregation_Home_Page> thisHomeSource in itemAggregation.Home_Page_File_Dictionary)
	            {
	                if ((thisHomeSource.Key == Web_Language_Enum.DEFAULT) || (thisHomeSource.Key == UI_ApplicationCache_Gateway.Settings.Default_UI_Language))
	                {
	                    Output.Write("<option value=\"" + thisHomeSource.Value + "\">" + HttpUtility.HtmlEncode(Web_Language_Enum_Converter.Enum_To_Name(UI_ApplicationCache_Gateway.Settings.Default_UI_Language)) + "</option>");
	                }
	                else
	                {
	                    Output.Write("<option value=\"" + thisHomeSource.Value + "\">" + HttpUtility.HtmlEncode(Web_Language_Enum_Converter.Enum_To_Name(thisHomeSource.Key)) + "</option>");
	                }
	            }
	        }

	        Output.WriteLine("</select>");
	        Output.WriteLine("        </td>");
	        Output.WriteLine("        <td style=\"padding-left:20px\"><button title=\"Add new home page\" class=\"sbkAdm_RoundButton\" onclick=\"return new_aggr_add_home();\">ADD</button></td>");
	        Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + NEW_HOME_PAGE_HELP + "');\"  title=\"" + NEW_HOME_PAGE_HELP + "\" /></td></tr></table>");
	        Output.WriteLine("     </td>");
	        Output.WriteLine("  </tr>");


	        Output.WriteLine("  <tr class=\"sbkSaav_TitleRow2\"><td colspan=\"3\">Banners</td></tr>");
	        Output.WriteLine("  <tr class=\"sbkSaav_TextRow\"><td colspan=\"3\"><p>This section shows all the existing language-specific banners for this aggregation and allows you upload new banners for this aggregation.</p></td></tr>");

            // Get list of existing banners
            string banner_folder = aggregationDirectory + "\\images\\banners";
            if (!Directory.Exists(banner_folder))
                Directory.CreateDirectory(banner_folder);
            string[] banner_files = SobekCM_File_Utilities.GetFiles(banner_folder, "*.jpg|*.bmp|*.gif|*.png");
            string last_added_banner = String.Empty;
            if (HttpContext.Current.Items["Uploaded File"] != null)
            {
                string newBanner = HttpContext.Current.Items["Uploaded File"].ToString();
                last_added_banner = Path.GetFileName(newBanner);
            }
            else
            {
                DateTime lastModDate = DateTime.MinValue;
                foreach (string thisBannerFile in banner_files)
                {
                    DateTime thisDate = (new FileInfo(thisBannerFile)).LastWriteTime;
                    if (thisDate.CompareTo(lastModDate) > 0)
                    {
                        lastModDate = thisDate;
                        last_added_banner = Path.GetFileName(thisBannerFile);
                    }
                }
            }

            // Also, build the list to keep track of unused banners
	        List<string> unused_banners = new List<string>();
	        if (banner_files != null)
	        {
	            unused_banners.AddRange(banner_files.Select(Path.GetFileName));
	        }


	        // Add all the EXISTING banner information
	        Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
	        Output.WriteLine("    <td>&nbsp;</td>");
	        Output.WriteLine("    <td class=\"sbkSaav_TableLabel2\">Existing Banners:</td>");
	        Output.WriteLine("    <td></td>");
	        Output.WriteLine("  </tr>");
	        Output.WriteLine("  <tr class=\"sbkSaav_TallRow\">");
	        Output.WriteLine("    <td>&nbsp;</td>");
	        Output.WriteLine("    <td colspan=\"2\">");
	        Output.WriteLine("      <table class=\"sbkSaav_BannerTable sbkSaav_Table\">");
	        Output.WriteLine("        <tr style=\"height:25px;\">");
	        Output.WriteLine("          <th class=\"sbkSaav_BannerTableHeader1\">LANGUAGE</th>");
	        Output.WriteLine("          <th class=\"sbkSaav_BannerTableHeader2\">TYPE</th>");
	        Output.WriteLine("          <th class=\"sbkSaav_BannerTableHeader3\">ACTION</th>");
	        Output.WriteLine("          <th class=\"sbkSaav_BannerTableHeader4\">IMAGE</th>");
	        Output.WriteLine("        </tr>");
	        if (itemAggregation.Front_Banner_Dictionary != null)
	        {
	            foreach (KeyValuePair<Web_Language_Enum, Item_Aggregation_Front_Banner> thisBannerInfo in itemAggregation.Front_Banner_Dictionary)
	            {
	                Output.WriteLine("        <tr>");
	                if ((thisBannerInfo.Key == Web_Language_Enum.DEFAULT) || (thisBannerInfo.Key == UI_ApplicationCache_Gateway.Settings.Default_UI_Language))
	                {
	                    Output.WriteLine("          <td style=\"font-style:italic; padding-left:5px;\">default</td>");
	                }
	                else
	                {
	                    Output.WriteLine("          <td style=\"padding-left:5px;\">" + Web_Language_Enum_Converter.Enum_To_Name(thisBannerInfo.Key) + "</td>");
	                }

	                // Show the TYPE
	                switch (thisBannerInfo.Value.Type)
	                {
	                    case Item_Aggregation_Front_Banner_Type_Enum.Full:
	                        Output.WriteLine("          <td>Home Page</td>");
	                        break;

	                    case Item_Aggregation_Front_Banner_Type_Enum.Left:
	                        Output.WriteLine("          <td>Home Page - Left</td>");
	                        break;

	                    case Item_Aggregation_Front_Banner_Type_Enum.Right:
	                        Output.WriteLine("          <td>Home Page - Right</td>");
	                        break;

	                }


	                string file = RequestSpecificValues.Current_Mode.Base_Design_URL + "aggregations/" + itemAggregation.Code + "/" + thisBannerInfo.Value.File.Replace("\\", "/");

	                if (unused_banners.Contains(Path.GetFileName(file)))
                        unused_banners.Remove(Path.GetFileName(file));

	                Output.Write("          <td class=\"sbkAdm_ActionLink\" > ( <a  href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return aggr_edit_delete_banner('" + Web_Language_Enum_Converter.Enum_To_Code(thisBannerInfo.Key) + "', 'front');\" title=\"Delete this banner\" >delete</a> )</td>");


	                Output.WriteLine("          <td><a href=\"" + file + "\" title=\"View banner image file\" target=\"" + itemAggregation.Code + "_" + thisBannerInfo.Value.File.Replace("\\", "_").Replace("/", "_") + "\"><img src=\"" + file + "\" alt=\"THIS BANNER IMAGE IS MISSING\" class=\"sbkSaav_BannerImage\" /></a></td>");
	                Output.WriteLine("        </tr>");
	            }
	        }



	        if (itemAggregation.Banner_Dictionary != null)
	        {
	            foreach (KeyValuePair<Web_Language_Enum, string> thisBannerInfo in itemAggregation.Banner_Dictionary)
	            {
	                Output.WriteLine("        <tr>");
	                bool canDelete = true;
	                if ((thisBannerInfo.Key == Web_Language_Enum.DEFAULT) || (thisBannerInfo.Key == UI_ApplicationCache_Gateway.Settings.Default_UI_Language))
	                {
	                    canDelete = false;
	                    Output.WriteLine("          <td style=\"font-style:italic; padding-left:5px;\">default</td>");
	                }
	                else
	                {
	                    Output.WriteLine("          <td style=\"padding-left:5px;\">" + Web_Language_Enum_Converter.Enum_To_Name(thisBannerInfo.Key) + "</td>");
	                }

	                // Show the TYPE
	                Output.WriteLine("          <td>Standard</td>");

	                string file = RequestSpecificValues.Current_Mode.Base_Design_URL + "aggregations/" + itemAggregation.Code + "/" + thisBannerInfo.Value.Replace("\\", "/");

                    if (unused_banners.Contains(Path.GetFileName(file)))
                        unused_banners.Remove(Path.GetFileName(file));

	                if (canDelete)
	                {
	                    Output.Write("          <td class=\"sbkAdm_ActionLink\" > ( <a  href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return aggr_edit_delete_banner('" + Web_Language_Enum_Converter.Enum_To_Code(thisBannerInfo.Key) + "', 'standard');\" title=\"Delete this banner\" >delete</a> )</td>");
	                }
	                else
	                {
	                    Output.WriteLine("          <td></td>");
	                }


	                Output.WriteLine("          <td><a href=\"" + file + "\" title=\"View banner image file\" target=\"" + itemAggregation.Code + "_" + thisBannerInfo.Value.Replace("\\", "_").Replace("/", "_") + "\"><img src=\"" + file + "\" alt=\"THIS BANNER IMAGE IS MISSING\" class=\"sbkSaav_BannerImage\" /></a></td>");
	                Output.WriteLine("        </tr>");
	            }
	        }

	        Output.WriteLine("      </table>");
	        Output.WriteLine("    </td>");
	        Output.WriteLine("  </tr>");



	        // Write the add new banner information
	        if (banner_files.Length > 0)
	        {
	            if (String.IsNullOrEmpty(last_added_banner))
	                last_added_banner = Path.GetFileName(banner_files[0]);

	            Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
	            Output.WriteLine("    <td>&nbsp;</td>");
	            Output.WriteLine("    <td class=\"sbkSaav_TableLabel2\">New Banner:</td>");
	            Output.WriteLine("    <td></td>");
	            Output.WriteLine("  </tr>");
	            Output.WriteLine("  <tr class=\"sbkSaav_TallRow\">");
	            Output.WriteLine("    <td>&nbsp;</td>");
	            Output.WriteLine("    <td colspan=\"2\">");

	            string current_banner = RequestSpecificValues.Current_Mode.Base_Design_URL + "aggregations/" + itemAggregation.Code + "/images/banners/" + last_added_banner;
	            Output.WriteLine("      <div style=\"width:510px; float:right;\"><img id=\"sbkSaav_SelectedBannerImage\" name=\"sbkSaav_SelectedBannerImage\" style=\"border: 1px #888888 solid;\" src=\"" + current_banner + "\" alt=\"Missing\" Title=\"Selected image file\" /></div>");

	            Output.WriteLine("      <table class=\"sbkSaav_BannerInnerTable\">");
	            Output.WriteLine("        <tr>");
	            Output.WriteLine("          <td>Language:</td>");
	            Output.WriteLine("          <td>");
	            Output.Write("            <select class=\"sbkSaav_SelectSingle\" id=\"admin_aggr_new_banner_lang\" name=\"admin_aggr_new_banner_lang\">");

	            // Add each language in the combo box
	            string language_name_default = Web_Language_Enum_Converter.Enum_To_Name(UI_ApplicationCache_Gateway.Settings.Default_UI_Language);
	            foreach (string possible_language in Web_Language_Enum_Converter.Language_Name_Array)
	            {
	                if (possible_language == language_name_default)
	                    Output.Write("<option value=\"" + Web_Language_Enum_Converter.Name_To_Code(possible_language) + "\" selected=\"selected\">" + HttpUtility.HtmlEncode(possible_language) + "</option>");
	                else
	                    Output.Write("<option value=\"" + Web_Language_Enum_Converter.Name_To_Code(possible_language) + "\">" + HttpUtility.HtmlEncode(possible_language) + "</option>");

	            }
	            Output.WriteLine();
	            Output.WriteLine("          </td>");
	            Output.WriteLine("          <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + NEW_BANNER_HELP + "');\"  title=\"" + NEW_BANNER_HELP + "\" /></td>");
	            Output.WriteLine("        <tr>");
	            Output.WriteLine("        <tr>");
	            Output.WriteLine("          <td>Banner Type:</td>");
	            Output.WriteLine("          <td>");
	            Output.Write("            <select class=\"sbkSaav_SelectSingle\" id=\"admin_aggr_new_banner_type\" name=\"admin_aggr_new_banner_type\">");
	            Output.Write("<option selected=\"selected\" value=\"standard\">Standard</option>");
	            Output.Write("<option value=\"home\">Home Page</option>");
	            Output.Write("<option value=\"left\">Home Page - Left</option>");
	            Output.WriteLine("<option value=\"right\">Home Page - Right</option></select>");
	            Output.WriteLine("          </td>");
	            Output.WriteLine("          <td></td>");
	            Output.WriteLine("        </tr>");
	            Output.WriteLine("        <tr>");
	            Output.WriteLine("          <td>Image:</td>");
	            Output.WriteLine("          <td>");
	            Output.Write("            <select class=\"sbkSaav_SelectSingle\" id=\"admin_aggr_new_banner_image\" name=\"admin_aggr_new_banner_image\"  onchange=\"edit_aggr_banner_select_changed('" + RequestSpecificValues.Current_Mode.Base_URL + "design/aggregations/" + itemAggregation.Code + "/images/banners/');\">");
	            foreach (string thisFile in banner_files)
	            {
	                string name = Path.GetFileName(thisFile);
	                if ((String.IsNullOrEmpty(last_added_banner)) || (name != last_added_banner))
	                    Output.Write("<option value=\"" + name + "\">" + name + "</option>");
	                else
	                    Output.Write("<option selected=\"selected\" value=\"" + last_added_banner + "\">" + last_added_banner + "</option>");

	            }
	            Output.WriteLine("</select>");
	            Output.WriteLine("          </td>");
	            Output.WriteLine("          <td></td>");
	            Output.WriteLine("        </tr>");
	            Output.WriteLine("        <tr>");
	            Output.WriteLine("          <td colspan=\"2\" style=\"text-align: center;\"><button title=\"Add new banner\" class=\"sbkAdm_RoundButton\" onclick=\"return new_aggr_add_banner();\">ADD</button></td>");
	            Output.WriteLine("          <td></td>");
	            Output.WriteLine("        </tr>");
	            Output.WriteLine("      </table>");
	            Output.WriteLine("    </td>");
	            Output.WriteLine("  </tr>");

	            if (unused_banners.Count > 0)
	            {
                    Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
                    Output.WriteLine("    <td>&nbsp;</td>");
                    Output.WriteLine("    <td class=\"sbkSaav_TableLabel2\"><br />Unused Banner(s):</td>");
                    Output.WriteLine("    <td></td>");
                    Output.WriteLine("  </tr>");

	                Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
	                Output.WriteLine("    <td></td>");
	                Output.WriteLine("    <td colspan=\"2\">");



	                Output.WriteLine("  <table id=\"sbkSaav_UploadTable\" class=\"statsTable\" style=\"padding-left: 100px;\">");
	                Output.WriteLine("    <tr>");

	                int unused_column = 0;
                    foreach (string thisImage in unused_banners)
	                {
	                    string thisImageFile = thisImage;
	                    string thisImageFile_URL = RequestSpecificValues.Current_Mode.Base_URL + "design/aggregations/" + itemAggregation.Code + "/images/banners/" + thisImageFile;

	                    Output.Write("      <td>");
	                    Output.Write("<img class=\"sbkSaav_UploadThumbnail\" src=\"" + thisImageFile_URL + "\" alt=\"Missing Thumbnail\" title=\"" + thisImageFile + "\" />");


	                    string display_name = thisImageFile;
	                    if (display_name.Length > 25)
	                    {
	                        Output.Write("<br /><span class=\"sbkSaav_UploadTitle\"><abbr title=\"" + display_name + "\">" + thisImageFile.Substring(0, 20) + "..." + Path.GetExtension(thisImage) + "</abbr></span>");
	                    }
	                    else
	                    {
	                        Output.Write("<br /><span class=\"sbkSaav_UploadTitle\">" + thisImageFile + "</span>");
	                    }



	                    // Build the action links
	                    Output.Write("<br /><span class=\"sbkAdm_ActionLink\" >( ");
	                    Output.Write("<a href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return delete_aggr_banner_file('" + thisImageFile + "');\" title=\"Delete this unused banner\">delete</a> ");
	                    Output.WriteLine(" )</span></td>");

	                    unused_column++;

	                    if (unused_column >= 3)
	                    {
	                        Output.WriteLine("    </tr>");
	                        Output.WriteLine("    <tr>");
	                        unused_column = 0;
	                    }
	                }

	                Output.WriteLine("  </table>");

	                Output.WriteLine("    </td>");
	                Output.WriteLine("  </tr>");
	            }
	        }

	        Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
	        Output.WriteLine("    <td colspan=\"3\">&nbsp;</td>");
	        Output.WriteLine("  </tr>");

	        Output.WriteLine("  <tr class=\"sbkSaav_UploadRow\">");
	        Output.WriteLine("    <td>&nbsp;</td>");
	        Output.WriteLine("    <td class=\"sbkSaav_TableLabel\">Upload New Banner Image:</td>");
	        Output.WriteLine("    <td>");
	        Output.WriteLine("       <table class=\"sbkSaav_InnerTable\">");
	        Output.WriteLine("         <tr>");
	        Output.WriteLine("           <td class=\"sbkSaav_UploadInstr\">To upload, browse to a GIF, PNG, JPEG, or BMP file, and then select UPLOAD</td>");
	        Output.WriteLine("           <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + UPLOAD_BANNER_HELP + "');\"  title=\"" + UPLOAD_BANNER_HELP + "\" /></td>");
	        Output.WriteLine("         </tr>");
	        Output.WriteLine("         <tr>");
	        Output.WriteLine("           <td colspan=\"2\">");
	    }

	    private void Finish_Page_5(TextWriter Output)
        {
            Output.WriteLine("           </td>");
            Output.WriteLine("         </tr>");
            Output.WriteLine("       </table>");
            Output.WriteLine("     </td>");
            Output.WriteLine("  </tr>");
            Output.WriteLine("</table>");
            Output.WriteLine("<br />");
        }

        private void Skin_Writer_Helper(TextWriter Output, string SkinID, string Skin, IEnumerable<string> Skin_Codes)
        {
            // Start the select box
            Output.Write("          <select class=\"sbkSaav_SelectSkin\" name=\"" + SkinID + "\" id=\"" + SkinID + "\">");

            // Add the NONE option first
            Output.Write(Skin.Length == 0 ? "<option value=\"\" selected=\"selected\" ></option>" : "<option value=\"\"></option>");

            // Add each metadata field to the select boxes
            foreach (string skinCode in Skin_Codes)
            {
                if (String.Compare(Skin, skinCode, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    Output.Write("<option value=\"" + skinCode + "\" selected=\"selected\" >" + HttpUtility.HtmlEncode(skinCode) + "</option>");
                }
                else
                {
                    Output.Write("<option value=\"" + skinCode + "\">" + HttpUtility.HtmlEncode(skinCode) + "</option>");
                }
            }
            Output.WriteLine("</select>");
        }

        #endregion


		#region Methods to render (and parse) page 2 - Search

		private void Save_Page_2_Postback(NameValueCollection Form)
		{
            // Get the map search type
		    decimal latitude = 0;
		    decimal longitude = 0;
		    int zoom = 1;
		    if (Form["admin_aggr_mapsearch_type"] != null)
		    {
                // Ensure this is not null
		        int map_search_type = Convert.ToInt32(Form["admin_aggr_mapsearch_type"]);
                if (map_search_type == -1)
                {
                    itemAggregation.Map_Search_Display = new Item_Aggregation_Map_Coverage_Info(Item_Aggregation_Map_Coverage_Type_Enum.EXTENT);
                    if ((Form["admin_aggr_mapsearch_zoom"] != null) && (Form["admin_aggr_mapsearch_latitude"] != null) && (Form["admin_aggr_mapsearch_longitude"] != null))
                    {
                        decimal customLatitude;
                        decimal customLongitude;
                        int customZoom;
                        if ((Int32.TryParse(Form["admin_aggr_mapsearch_zoom"], out customZoom)) && (Decimal.TryParse(Form["admin_aggr_mapsearch_longitude"], out customLongitude)) && (Decimal.TryParse(Form["admin_aggr_mapsearch_latitude"], out customLatitude)))
                        {
                            itemAggregation.Map_Search_Display = new Item_Aggregation_Map_Coverage_Info(Item_Aggregation_Map_Coverage_Type_Enum.FIXED, customZoom, customLongitude, customLatitude);
                        }
                    }
                }
                else if (map_search_type == -2)
                {
                    itemAggregation.Map_Search_Display = new Item_Aggregation_Map_Coverage_Info(Item_Aggregation_Map_Coverage_Type_Enum.EXTENT);
                }
                else
                {
                    switch (map_search_type)
                    {
                        case 0: // WORLD
                            latitude = 0;
                            longitude = 0;
                            zoom = 1;
                            break;

                        case 1: // FLORIDA
                            latitude = 28;
                            longitude = -84.5m;
                            zoom = 6;
                            break;

                        case 2: // NORTH AMERICA
                            latitude = 48;
                            longitude = -95;
                            zoom = 3;
                            break;

                        case 3: // CARIBBEAN
                            latitude = 19;
                            longitude = -74;
                            zoom = 4;
                            break;

                        case 4: // SOUTH AMERICA
                            latitude = -22;
                            longitude = -60;
                            zoom = 3;
                            break;

                        case 5: // AFRICA 
                            latitude = 6;
                            longitude = 19.5m;
                            zoom = 3;
                            break;

                        case 6: // EUROPE
                            latitude = 49.5m;
                            longitude = 13.35m;
                            zoom = 4;
                            break;

                        case 7: // ASIA
                            latitude = 36;
                            longitude = 96;
                            zoom = 3;
                            break;

                        case 8: // MIDDLE EAST
                            latitude = 31;
                            longitude = 39;
                            zoom = 4;
                            break;
                    }
                    itemAggregation.Map_Search_Display = new Item_Aggregation_Map_Coverage_Info(Item_Aggregation_Map_Coverage_Type_Enum.FIXED, zoom, longitude, latitude);
                }
		    }

            // Get the map browse type
            latitude = 0;
            longitude = 0;
            zoom = 1;
            if (Form["admin_aggr_mapbrowse_type"] != null)
            {
                // Ensure this is not null
                int map_browse_type = Convert.ToInt32(Form["admin_aggr_mapbrowse_type"]);
                if (map_browse_type == -1)
                {
                    itemAggregation.Map_Browse_Display = new Item_Aggregation_Map_Coverage_Info(Item_Aggregation_Map_Coverage_Type_Enum.EXTENT);
                    if ((Form["admin_aggr_mapbrowse_zoom"] != null) && (Form["admin_aggr_mapbrowse_latitude"] != null) && (Form["admin_aggr_mapbrowse_longitude"] != null))
                    {
                        decimal customLatitude;
                        decimal customLongitude;
                        int customZoom;
                        if ((Int32.TryParse(Form["admin_aggr_mapbrowse_zoom"], out customZoom)) && (Decimal.TryParse(Form["admin_aggr_mapbrowse_longitude"], out customLongitude)) && (Decimal.TryParse(Form["admin_aggr_mapbrowse_latitude"], out customLatitude)))
                        {
                            itemAggregation.Map_Browse_Display = new Item_Aggregation_Map_Coverage_Info(Item_Aggregation_Map_Coverage_Type_Enum.FIXED, customZoom, customLongitude, customLatitude);
                        }
                    }
                }
                else if (map_browse_type == -2)
                {
                    itemAggregation.Map_Browse_Display = new Item_Aggregation_Map_Coverage_Info(Item_Aggregation_Map_Coverage_Type_Enum.EXTENT);
                }
                else
                {
                    switch (map_browse_type)
                    {
                        case 0: // WORLD
                            latitude = 0;
                            longitude = 0;
                            zoom = 1;
                            break;

                        case 1: // FLORIDA
                            latitude = 28;
                            longitude = -84.5m;
                            zoom = 6;
                            break;

                        case 2: // NORTH AMERICA
                            latitude = 48;
                            longitude = -95;
                            zoom = 3;
                            break;

                        case 3: // CARIBBEAN
                            latitude = 19;
                            longitude = -74;
                            zoom = 4;
                            break;

                        case 4: // SOUTH AMERICA
                            latitude = -22;
                            longitude = -60;
                            zoom = 3;
                            break;

                        case 5: // AFRICA 
                            latitude = 6;
                            longitude = 19.5m;
                            zoom = 3;
                            break;

                        case 6: // EUROPE
                            latitude = 49.5m;
                            longitude = 13.35m;
                            zoom = 4;
                            break;

                        case 7: // ASIA
                            latitude = 36;
                            longitude = 96;
                            zoom = 3;
                            break;

                        case 8: // MIDDLE EAST
                            latitude = 31;
                            longitude = 39;
                            zoom = 4;
                            break;
                    }
                    itemAggregation.Map_Browse_Display = new Item_Aggregation_Map_Coverage_Info(Item_Aggregation_Map_Coverage_Type_Enum.FIXED, zoom, longitude, latitude);
                }
            }


		    //if (Form["admin_aggr_mapsearch_type"] != null)
            //    itemAggregation.Map_Search = Convert.ToUInt16(Form["admin_aggr_mapsearch_type"]);

			// Build the display options string
			StringBuilder displayOptionsBldr = new StringBuilder();
			if (Form["admin_aggr_basicsearch"] != null) displayOptionsBldr.Append("B");
			if (Form["admin_aggr_basicsearch_years"] != null) displayOptionsBldr.Append("Y");
            if (Form["admin_aggr_basicsearch_mimetype"] != null) displayOptionsBldr.Append("W");
			if (Form["admin_aggr_dloctextsearch"] != null) displayOptionsBldr.Append("C");
			if (Form["admin_aggr_textsearch"] != null) displayOptionsBldr.Append("F");
			if (Form["admin_aggr_newspsearch"] != null) displayOptionsBldr.Append("N");
			if (Form["admin_aggr_advsearch"] != null) displayOptionsBldr.Append("A");
			if (Form["admin_aggr_advsearch_years"] != null) displayOptionsBldr.Append("Z");
            if (Form["admin_aggr_advsearch_mimetype"] != null) displayOptionsBldr.Append("X");
			if (Form["admin_aggr_mapsearch"] != null) displayOptionsBldr.Append("M");
          //  if (Form["admin_aggr_mapsearchbeta"] != null) displayOptionsBldr.Append("Q");
			if (Form["admin_aggr_mapbrowse"] != null) displayOptionsBldr.Append("G");
			if (Form["admin_aggr_allitems"] != null) displayOptionsBldr.Append("I");
            

			itemAggregation.Display_Options = displayOptionsBldr.ToString();
		}

		private void Add_Page_2(TextWriter Output)
		{
			// Help constants (for now)
			const string ALL_ITEMS_HELP = "Include, or exclude, the special button to allow users to browse all items, or all new items.  Users can always browse all items by running an empty search.";
			const string MAP_BROWSE_HELP = "Include the map browse feature on this collection, allowing users to see where all the items in this collection appear, on a map.";
			const string MAP_SEARCH_BOUNDING_HELP = "Default map search location.";

			Output.WriteLine("<table class=\"sbkAdm_PopupTable\">");

			Output.WriteLine("  <tr class=\"sbkSaav_TitleRow\"><td colspan=\"3\">Search Options</td></tr>");
			Output.WriteLine("  <tr class=\"sbkSaav_TextRow\"><td colspan=\"3\"><p>These options control how searching works within this aggregation, such as which search options are made publicly available.</p><p>For more information about the settings on this tab, <a href=\"" + UI_ApplicationCache_Gateway.Settings.Help_URL(RequestSpecificValues.Current_Mode.Base_URL) + "adminhelp/singleaggr\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</p></td></tr>");

			// Add line for basic search type
            Output.WriteLine("  <tr class=\"sbkSaav_SearchCheckRow\" style=\"vertical-align:top\">");
			Output.WriteLine("    <td style=\"width:50px;\">&nbsp;</td>");
			Output.WriteLine("    <td  style=\"width:175px; vertical-align:top;\" class=\"sbkSaav_TableLabel\">Search Types:</label></td>");
			Output.WriteLine("    <td>");
            Output.Write("      <div class=\"sbkSaav_SearchCheckDiv\"><input class=\"sbkSaav_checkbox\" type=\"checkbox\" name=\"admin_aggr_basicsearch\" id=\"admin_aggr_basicsearch\"");
			if (( itemAggregation.Display_Options.IndexOf("B") >= 0 ) || (itemAggregation.Display_Options.IndexOf("D") >= 0 ))
				Output.Write(" checked=\"checked\"");
            Output.WriteLine(" /> <label for=\"admin_aggr_basicsearch\">Basic Search</label></div>");
            Output.WriteLine("      <img class=\"sbkSaav_SearchImg\" src=\"" + Static_Resources.Search_Basic_Img + "\" onclick=\"expand_contract_search_img(this);\"  title=\"Click to expand or reduce this image.\" />");
			Output.WriteLine("    </td>");
			Output.WriteLine("  </tr>");

			// Add line for basic search with year range
            Output.WriteLine("  <tr class=\"sbkSaav_SearchCheckRow\" style=\"vertical-align:top\">");
			Output.WriteLine("    <td colspan=\"2\">&nbsp;</td>");
			Output.WriteLine("    <td>");
            Output.Write("      <div class=\"sbkSaav_SearchCheckDiv\"><input class=\"sbkSaav_checkbox\" type=\"checkbox\" name=\"admin_aggr_basicsearch_years\" id=\"admin_aggr_basicsearch_years\"");
			if (itemAggregation.Display_Options.IndexOf("Y") >= 0)
				Output.Write(" checked=\"checked\"");
            Output.WriteLine(" /> <label for=\"admin_aggr_basicsearch_years\">Basic Search<br /> &nbsp; &nbsp; &nbsp; &nbsp; (with Year Range)</label></div>");
            Output.WriteLine("      <img class=\"sbkSaav_SearchImg\" src=\"" + Static_Resources.Search_Basic_Year_Range_Img + "\" onclick=\"expand_contract_search_img(this);\"  title=\"Click to expand or reduce this image.\" />");
            Output.WriteLine("    </td>");
            Output.WriteLine("  </tr>");

            // Add line for basic search ( with mime-type exclusion )
            Output.WriteLine("  <tr class=\"sbkSaav_SearchCheckRow\" style=\"vertical-align:top\">");
            Output.WriteLine("    <td colspan=\"2\">&nbsp;</td>");
            Output.WriteLine("    <td>");
            Output.Write("      <div class=\"sbkSaav_SearchCheckDiv\"><input class=\"sbkSaav_checkbox\" type=\"checkbox\" name=\"admin_aggr_basicsearch_mimetype\" id=\"admin_aggr_basicsearch_mimetype\"");
            if (itemAggregation.Display_Options.IndexOf("W") >= 0)
                Output.Write(" checked=\"checked\"");
            Output.WriteLine(" /> <label for=\"admin_aggr_basicsearch_mimetype\">Basic search<br /> &nbsp; &nbsp; &nbsp; &nbsp; (with mime-type filter)</label></div>");
            Output.WriteLine("      <img class=\"sbkSaav_SearchImg\" src=\"" + Static_Resources.Search_Basic_MimeType_Img + "\" onclick=\"expand_contract_search_img(this);\"  title=\"Click to expand or reduce this image.\" />");
            Output.WriteLine("    </td>");
            Output.WriteLine("  </tr>");

			// Add line for advanced search type
            Output.WriteLine("  <tr class=\"sbkSaav_SearchCheckRow\" style=\"vertical-align:top\">");
			Output.WriteLine("    <td colspan=\"2\">&nbsp;</td>");
			Output.WriteLine("    <td>");
			Output.Write(    "      <div class=\"sbkSaav_SearchCheckDiv\"><input class=\"sbkSaav_checkbox\" type=\"checkbox\" name=\"admin_aggr_advsearch\" id=\"admin_aggr_advsearch\"");
			if (itemAggregation.Display_Options.IndexOf("A") >= 0)
				Output.Write(" checked=\"checked\"");
            Output.WriteLine(" /> <label for=\"admin_aggr_advsearch\">Advanced Search</label></div></div>");
            Output.WriteLine("      <img class=\"sbkSaav_SearchImg\" src=\"" + Static_Resources.Search_Advanced_Img + "\" onclick=\"expand_contract_search_img(this);\"  title=\"Click to expand or reduce this image.\" />");
            Output.WriteLine("    </td>");
            Output.WriteLine("  </tr>");

			// Add line for advanced search with year range
            Output.WriteLine("  <tr class=\"sbkSaav_SearchCheckRow\" style=\"vertical-align:top\">");
			Output.WriteLine("    <td colspan=\"2\">&nbsp;</td>");
			Output.WriteLine("    <td>");
            Output.Write("      <div class=\"sbkSaav_SearchCheckDiv\"><input class=\"sbkSaav_checkbox\" type=\"checkbox\" name=\"admin_aggr_advsearch_years\" id=\"admin_aggr_advsearch_years\"");
			if (itemAggregation.Display_Options.IndexOf("Z") >= 0)
				Output.Write(" checked=\"checked\"");
            Output.WriteLine(" /> <label for=\"admin_aggr_advsearch_years\">Advanced Search<br /> &nbsp; &nbsp; &nbsp; &nbsp; (with Year Range)</label></div>");
            Output.WriteLine("      <img class=\"sbkSaav_SearchImg\" src=\"" + Static_Resources.Search_Advanced_Year_Range_Img + "\" onclick=\"expand_contract_search_img(this);\"  title=\"Click to expand or reduce this image.\" />");
            Output.WriteLine("    </td>");
            Output.WriteLine("  </tr>");

            // Add line for advanced search with MIME-TYPE exclusion type
            Output.WriteLine("  <tr class=\"sbkSaav_SearchCheckRow\" style=\"vertical-align:top\">");
            Output.WriteLine("    <td colspan=\"2\">&nbsp;</td>");
            Output.WriteLine("    <td>");
            Output.Write("      <div class=\"sbkSaav_SearchCheckDiv\"><input class=\"sbkSaav_checkbox\" type=\"checkbox\" name=\"admin_aggr_advsearch_mimetype\" id=\"admin_aggr_advsearch_mimetype\"");
            if (itemAggregation.Display_Options.IndexOf("X") >= 0)
                Output.Write(" checked=\"checked\"");
            Output.WriteLine(" /> <label for=\"admin_aggr_advsearch\">Advanced Search<br /> &nbsp; &nbsp; &nbsp; &nbsp; (with mime-type filter)</label></div>");
            Output.WriteLine("      <img class=\"sbkSaav_SearchImg\" src=\"" + Static_Resources.Search_Advanced_MimeType_Img + "\" onclick=\"expand_contract_search_img(this);\"  title=\"Click to expand or reduce this image.\" />");
            Output.WriteLine("    </td>");
            Output.WriteLine("  </tr>");

			// Add line for full text search
            Output.WriteLine("  <tr class=\"sbkSaav_SearchCheckRow\" style=\"vertical-align:top\">");
			Output.WriteLine("    <td colspan=\"2\">&nbsp;</td>");
			Output.WriteLine("    <td>");
            Output.Write("      <div class=\"sbkSaav_SearchCheckDiv\"><input class=\"sbkSaav_checkbox\" type=\"checkbox\" name=\"admin_aggr_textsearch\" id=\"admin_aggr_textsearch\"");
			if (itemAggregation.Display_Options.IndexOf("F") >= 0)
				Output.Write(" checked=\"checked\"");
            Output.WriteLine(" /> <label for=\"admin_aggr_textsearch\">Full Text Search</label></div>");
            Output.WriteLine("      <img class=\"sbkSaav_SearchImg\" src=\"" + Static_Resources.Search_Full_Text_Img + "\" onclick=\"expand_contract_search_img(this);\"  title=\"Click to expand or reduce this image.\" />");
            Output.WriteLine("    </td>");
            Output.WriteLine("  </tr>");

            // Add line for dLOC full text saerch
            Output.WriteLine("  <tr class=\"sbkSaav_SearchCheckRow\" style=\"vertical-align:top\">");
            Output.WriteLine("    <td colspan=\"2\">&nbsp;</td>");
            Output.WriteLine("    <td>");
            Output.Write("      <div class=\"sbkSaav_SearchCheckDiv\"><input class=\"sbkSaav_checkbox\" type=\"checkbox\" name=\"admin_aggr_dloctextsearch\" id=\"admin_aggr_dloctextsearch\"");
            if (itemAggregation.Display_Options.IndexOf("C") >= 0)
                Output.Write(" checked=\"checked\"");
            Output.WriteLine(" /> <label for=\"admin_aggr_dloctextsearch\">Full Text Search<br /> &nbsp; &nbsp; &nbsp; &nbsp; (w/ newspaper exclusion option)</label></div>");
            Output.WriteLine("      <img class=\"sbkSaav_SearchImg\" src=\"" + Static_Resources.Search_Full_Text_Exlude_Newspapers_Img + "\" onclick=\"expand_contract_search_img(this);\"  title=\"Click to expand or reduce this image.\" />");
            Output.WriteLine("    </td>");
            Output.WriteLine("  </tr>");

			// Add line for newspaper search
            Output.WriteLine("  <tr class=\"sbkSaav_SearchCheckRow\" style=\"vertical-align:top\">");
			Output.WriteLine("    <td colspan=\"2\">&nbsp;</td>");
			Output.WriteLine("    <td>");
            Output.Write("      <div class=\"sbkSaav_SearchCheckDiv\"><input class=\"sbkSaav_checkbox\" type=\"checkbox\" name=\"admin_aggr_newspsearch\" id=\"admin_aggr_newspsearch\"");
			if (itemAggregation.Display_Options.IndexOf("N") >= 0)
				Output.Write(" checked=\"checked\"");
            Output.WriteLine(" /> <label for=\"admin_aggr_newspsearch\">Newspaper Search</label></div>");
            Output.WriteLine("      <img class=\"sbkSaav_SearchImg\" src=\"" + Static_Resources.Search_Newspaper_Img + "\" onclick=\"expand_contract_search_img(this);\"  title=\"Click to expand or reduce this image.\" />");
            Output.WriteLine("    </td>");
            Output.WriteLine("  </tr>");

			// Add line for Map saerch
            Output.WriteLine("  <tr class=\"sbkSaav_SearchCheckRow\" style=\"vertical-align:top\">");
			Output.WriteLine("    <td colspan=\"2\">&nbsp;</td>");
			Output.WriteLine("    <td>");
            Output.Write("      <div class=\"sbkSaav_SearchCheckDiv\"><input class=\"sbkSaav_checkbox\" type=\"checkbox\" name=\"admin_aggr_mapsearch\" id=\"admin_aggr_mapsearch\"");
			if (itemAggregation.Display_Options.IndexOf("M") >= 0)
				Output.Write(" checked=\"checked\"");
			Output.WriteLine(" /> <label for=\"admin_aggr_mapsearch\">Map Search</label></div>");
            Output.WriteLine("       <img class=\"sbkSaav_SearchImg\" src=\"" + Static_Resources.Search_Map_Img + "\" onclick=\"expand_contract_search_img(this);\"  title=\"Click to expand or reduce this image.\" />");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");

            //// Add line for Map saerch beta
            //Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
            //Output.WriteLine("    <td colspan=\"2\">&nbsp;</td>");
            //Output.WriteLine("    <td>");
            //Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td>");
            //Output.Write("          <input class=\"sbkSaav_checkbox\" type=\"checkbox\" name=\"admin_aggr_mapsearchbeta\" id=\"admin_aggr_mapsearchbeta\"");
            //if (itemAggregation.Display_Options.IndexOf("Q") >= 0)
            //    Output.Write(" checked=\"checked\"");
            //Output.WriteLine(" /> <label for=\"admin_aggr_mapsearchbeta\">Map Search (Beta)</label>");
            //Output.WriteLine("        </td>");
            //Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + MAP_SEARCH_BETA_HELP + "');\"  title=\"" + MAP_SEARCH_HELP + "\" /></td></tr></table>");
            //Output.WriteLine("     </td>");
            //Output.WriteLine("  </tr>");


            


			// Add line for all/new item browses type
			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
			Output.WriteLine("    <td>&nbsp;</td>");
			Output.WriteLine("    <td class=\"sbkSaav_TableLabel\">Other Display Types:</label></td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td>");
			Output.Write("          <input class=\"sbkSaav_checkbox\" type=\"checkbox\" name=\"admin_aggr_allitems\" id=\"admin_aggr_allitems\"");
			if (itemAggregation.Display_Options.IndexOf("I") >= 0)
				Output.Write(" checked=\"checked\"");
			Output.WriteLine(" /> <label for=\"admin_aggr_allitems\">Include All / New Item Browses</label>");
			Output.WriteLine("        </td>");
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + ALL_ITEMS_HELP + "');\"  title=\"" + ALL_ITEMS_HELP + "\" /></td></tr></table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");

			// Add line for map browse
			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
			Output.WriteLine("    <td colspan=\"2\">&nbsp;</td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td>");
			Output.Write("          <input class=\"sbkSaav_checkbox\" type=\"checkbox\" name=\"admin_aggr_mapbrowse\" id=\"admin_aggr_mapbrowse\"");
			if (itemAggregation.Display_Options.IndexOf("G") >= 0)
				Output.Write(" checked=\"checked\"");
			Output.WriteLine(" /> <label for=\"admin_aggr_mapbrowse\">Include Map Browse</label>");
			Output.WriteLine("        </td>");
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + MAP_BROWSE_HELP + "');\"  title=\"" + MAP_BROWSE_HELP + "\" /></td></tr></table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");

            // Determine the value for the map SEARCH drop down
		    int search_area_value = -2;
		    decimal latitude = 0;
            decimal longitude = 0;
		    int zoom = 1;
		    if (itemAggregation.Map_Search_Display != null)
		    {
		        if (itemAggregation.Map_Search_Display.Type == Item_Aggregation_Map_Coverage_Type_Enum.FIXED)
		        {
		            search_area_value = -1;
		            latitude = 0;
		            longitude = 0;
		            zoom = 1;
		            if ((itemAggregation.Map_Search_Display.ZoomLevel.HasValue) && (itemAggregation.Map_Search_Display.Latitude.HasValue) && (itemAggregation.Map_Search_Display.Longitude.HasValue))
		            {
		                latitude = itemAggregation.Map_Search_Display.Latitude.Value;
		                longitude = itemAggregation.Map_Search_Display.Longitude.Value;
		                zoom = itemAggregation.Map_Search_Display.ZoomLevel.Value;

		            }
		            if (zoom <= 1)
		                search_area_value = 0;
		            else if ((latitude == 28m) && (longitude == -84.5m) && (zoom == 6))
		            {
		                search_area_value = 1; // Florida
		            }
		            else if ((latitude == 48m) && (longitude == -95m) && (zoom == 3))
		            {
		                search_area_value = 2; // North American
		            }
		            else if ((latitude == 19m) && (longitude == -74m) && (zoom == 4))
		            {
		                search_area_value = 3; // Caribbean
		            }
		            else if ((latitude == -22m) && (longitude == -60m) && (zoom == 3))
		            {
		                search_area_value = 4; // South America
		            }
		            else if ((latitude == 6m) && (longitude == 19.5m) && (zoom == 3))
		            {
		                search_area_value = 5; // Africa
		            }
		            else if ((latitude == 49.5m) && (longitude == 13.35m) && (zoom == 4))
		            {
		                search_area_value = 6; // Europe
		            }
		            else if ((latitude == 36m) && (longitude == 96m) && (zoom == 3))
		            {
		                search_area_value = 7; // Asia
		            }
		            else if ((latitude == 31m) && (longitude == 39m) && (zoom == 4))
		            {
		                search_area_value = 8; // Middle east
		            }
		        }
		        else
		        {
                    if ((itemAggregation.Map_Search_Display.ZoomLevel.HasValue) && (itemAggregation.Map_Search_Display.Latitude.HasValue) && (itemAggregation.Map_Search_Display.Longitude.HasValue))
                    {
                        latitude = itemAggregation.Map_Search_Display.Latitude.Value;
                        longitude = itemAggregation.Map_Search_Display.Longitude.Value;
                        zoom = itemAggregation.Map_Search_Display.ZoomLevel.Value;
                    }
		        }
		    }

			// Add line the map SEARCH coverage drop down
			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
			Output.WriteLine("    <td>&nbsp;</td>");
			Output.WriteLine("    <td class=\"sbkSaav_TableLabel\">Map Search Default Area:</label></td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <table class=\"sbkSaav_InnerTable\">");
            Output.WriteLine("        <tr>");
            Output.WriteLine("          <td style=\"width:190px\">");
            Output.WriteLine("            <select class=\"sbkSaav_SelectSingle\" name=\"admin_aggr_mapsearch_type\" id=\"admin_aggr_mapsearch_type\" onchange=\"return aggr_mapsearch_changed();\">");

            Output.WriteLine(search_area_value == -2
                           ? "              <option value=\"-2\" selected=\"selected\" >(zoom to extent)</option>"
                           : "              <option value=\"-2\">(zoom to extent)</option>");

            Output.WriteLine(search_area_value == -1
                           ? "              <option value=\"-1\" selected=\"selected\" >(custom)</option>"
                           : "              <option value=\"-1\">(custom)</option>");

            Output.WriteLine(search_area_value == 0
							? "             <option value=\"0\" selected=\"selected\" >World</option>"
							: "             <option value=\"0\">World</option>");

            Output.Write(search_area_value == 5
                            ? "             <option value=\"5\" selected=\"selected\" >Africa</option>"
                            : "             <option value=\"5\">Africa</option>");


            Output.Write(search_area_value == 7
                            ? "             <option value=\"7\" selected=\"selected\" >Asia</option>"
                            : "             <option value=\"7\">Asia</option>");

            Output.Write(search_area_value == 6
                            ? "             <option value=\"6\" selected=\"selected\" >Europe</option>"
                            : "             <option value=\"6\">Europe</option>");

            Output.Write(search_area_value == 2
							? "             <option value=\"2\" selected=\"selected\" >North America</option>"
							: "             <option value=\"2\">North America</option>");

            Output.Write(search_area_value == 4
							? "             <option value=\"4\" selected=\"selected\" >Caribbean</option>"
							: "             <option value=\"4\">South America</option>");

            Output.Write(search_area_value == 8
							? "             <option value=\"8\" selected=\"selected\" >Middle East</option>"
							: "             <option value=\"8\">Middle East</option>");

            Output.Write(search_area_value == 3
                            ? "             <option value=\"3\" selected=\"selected\" >Caribbean</option>"
                            : "             <option value=\"3\">Caribbean</option>");

            Output.Write(search_area_value == 1
                            ? "             <option value=\"1\" selected=\"selected\" >Florida</option>"
                            : "             <option value=\"1\">Florida</option>");

            Output.WriteLine("            </select>");
			Output.WriteLine("          </td>");
            Output.WriteLine("          <td style=\"text-align:left;\"><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + MAP_SEARCH_BOUNDING_HELP + "');\"  title=\"" + MAP_SEARCH_BOUNDING_HELP + "\" /></td>");
		    Output.WriteLine("        </tr>");
            if (search_area_value == -1)
                Output.WriteLine("        <tr id=\"admin_aggr_mapsearch_custom_row\">");
            else
                Output.WriteLine("        <tr id=\"admin_aggr_mapsearch_custom_row\" style=\"display:none\">");
            Output.WriteLine("          <td colspan=\"2\">");
		    Output.WriteLine("            Zoom: <input class=\"sbkSaav_small_input sbkAdmin_Focusable\" name=\"admin_aggr_mapsearch_zoom\" id=\"admin_aggr_mapsearch_zoom\" type=\"text\" value=\"" + zoom + "\" /> ");
            Output.WriteLine("            Latitude: <input class=\"sbkSaav_small_input sbkAdmin_Focusable\" name=\"admin_aggr_mapsearch_latitude\" id=\"admin_aggr_mapsearch_latitude\" type=\"text\" value=\"" + latitude + "\" /> ");
            Output.WriteLine("            Longitude: <input class=\"sbkSaav_small_input sbkAdmin_Focusable\" name=\"admin_aggr_mapsearch_longitude\" id=\"admin_aggr_mapsearch_longitude\" type=\"text\" value=\"" + longitude + "\" /> ");
            Output.WriteLine("          </td>");
            Output.WriteLine("        </tr>");
            Output.WriteLine("      </table>");
			Output.WriteLine("    </td>");
			Output.WriteLine("  </tr>");


            // Determine the value for the map BROWSE drop down
            search_area_value = -2;
            latitude = 0;
            longitude = 0;
            zoom = 1;
            if (itemAggregation.Map_Browse_Display != null)
            {
                if (itemAggregation.Map_Browse_Display.Type == Item_Aggregation_Map_Coverage_Type_Enum.FIXED)
                {
                    search_area_value = -1;
                    latitude = 0;
                    longitude = 0;
                    zoom = 1;
                    if ((itemAggregation.Map_Browse_Display.ZoomLevel.HasValue) && (itemAggregation.Map_Browse_Display.Latitude.HasValue) && (itemAggregation.Map_Browse_Display.Longitude.HasValue))
                    {
                        latitude = itemAggregation.Map_Browse_Display.Latitude.Value;
                        longitude = itemAggregation.Map_Browse_Display.Longitude.Value;
                        zoom = itemAggregation.Map_Browse_Display.ZoomLevel.Value;

                    }
                    if (zoom <= 1)
                        search_area_value = 0;
                    else if ((latitude == 28m) && (longitude == -84.5m) && (zoom == 6))
                    {
                        search_area_value = 1; // Florida
                    }
                    else if ((latitude == 48m) && (longitude == -95m) && (zoom == 3))
                    {
                        search_area_value = 2; // North American
                    }
                    else if ((latitude == 19m) && (longitude == -74m) && (zoom == 4))
                    {
                        search_area_value = 3; // Caribbean
                    }
                    else if ((latitude == -22m) && (longitude == -60m) && (zoom == 3))
                    {
                        search_area_value = 4; // South America
                    }
                    else if ((latitude == 6m) && (longitude == 19.5m) && (zoom == 3))
                    {
                        search_area_value = 5; // Africa
                    }
                    else if ((latitude == 49.5m) && (longitude == 13.35m) && (zoom == 4))
                    {
                        search_area_value = 6; // Europe
                    }
                    else if ((latitude == 36m) && (longitude == 96m) && (zoom == 3))
                    {
                        search_area_value = 7; // Asia
                    }
                    else if ((latitude == 31m) && (longitude == 39m) && (zoom == 4))
                    {
                        search_area_value = 8; // Middle east
                    }
                }
            }

            // Add line the map BROWSE coverage drop down
            Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
            Output.WriteLine("    <td>&nbsp;</td>");
            Output.WriteLine("    <td class=\"sbkSaav_TableLabel\">Map Browse Default Area:</label></td>");
            Output.WriteLine("    <td>");
            Output.WriteLine("      <table class=\"sbkSaav_InnerTable\">");
            Output.WriteLine("        <tr>");
            Output.WriteLine("          <td style=\"width:190px\">");
            Output.WriteLine("          <select class=\"sbkSaav_SelectSingle\" name=\"admin_aggr_mapbrowse_type\" id=\"admin_aggr_mapbrowse_type\" onchange=\"return aggr_mapbrowse_changed();\">");

            Output.WriteLine(search_area_value == -2
                            ? "            <option value=\"-2\" selected=\"selected\" >(zoom to extent)</option>"
                            : "            <option value=\"-2\">(zoom to extent)</option>");

            Output.WriteLine(search_area_value == -1
                            ? "            <option value=\"-1\" selected=\"selected\" >(custom)</option>"
                            : "            <option value=\"-1\">(custom)</option>");

            Output.WriteLine(search_area_value == 0
                            ? "             <option value=\"0\" selected=\"selected\" >World</option>"
                            : "             <option value=\"0\">World</option>");

            Output.Write(search_area_value == 5
                            ? "             <option value=\"5\" selected=\"selected\" >Africa</option>"
                            : "             <option value=\"5\">Africa</option>");


            Output.Write(search_area_value == 7
                            ? "             <option value=\"7\" selected=\"selected\" >Asia</option>"
                            : "             <option value=\"7\">Asia</option>");

            Output.Write(search_area_value == 6
                            ? "             <option value=\"6\" selected=\"selected\" >Europe</option>"
                            : "             <option value=\"6\">Europe</option>");

            Output.Write(search_area_value == 2
                            ? "             <option value=\"2\" selected=\"selected\" >North America</option>"
                            : "             <option value=\"2\">North America</option>");

            Output.Write(search_area_value == 4
                            ? "             <option value=\"4\" selected=\"selected\" >Caribbean</option>"
                            : "             <option value=\"4\">South America</option>");

            Output.Write(search_area_value == 8
                            ? "             <option value=\"8\" selected=\"selected\" >Middle East</option>"
                            : "             <option value=\"8\">Middle East</option>");

            Output.Write(search_area_value == 3
                            ? "             <option value=\"3\" selected=\"selected\" >Caribbean</option>"
                            : "             <option value=\"3\">Caribbean</option>");

            Output.Write(search_area_value == 1
                            ? "             <option value=\"1\" selected=\"selected\" >Florida</option>"
                            : "             <option value=\"1\">Florida</option>");


            Output.WriteLine("            </select>");
            Output.WriteLine("          </td>");
            Output.WriteLine("          <td style=\"text-align:left;\"><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + MAP_SEARCH_BOUNDING_HELP + "');\"  title=\"" + MAP_SEARCH_BOUNDING_HELP + "\" /></td>");
            Output.WriteLine("        </tr>");
            if ( search_area_value == -1)
                Output.WriteLine("        <tr id=\"admin_aggr_mapbrowse_custom_row\">");
            else
                Output.WriteLine("        <tr id=\"admin_aggr_mapbrowse_custom_row\" style=\"display:none\">");
            Output.WriteLine("          <td colspan=\"2\">");
            Output.WriteLine("            Zoom: <input class=\"sbkSaav_small_input sbkAdmin_Focusable\" name=\"admin_aggr_mapbrowse_zoom\" id=\"admin_aggr_mapbrowse_zoom\" type=\"text\" value=\"" + zoom + "\" /> ");
            Output.WriteLine("            Latitude: <input class=\"sbkSaav_small_input sbkAdmin_Focusable\" name=\"admin_aggr_mapbrowse_latitude\" id=\"admin_aggr_mapbrowse_latitude\" type=\"text\" value=\"" + latitude + "\" /> ");
            Output.WriteLine("            Longitude: <input class=\"sbkSaav_small_input sbkAdmin_Focusable\" name=\"admin_aggr_mapbrowse_longitude\" id=\"admin_aggr_mapbrowse_longitude\" type=\"text\" value=\"" + longitude + "\" /> ");
            Output.WriteLine("          </td>");
            Output.WriteLine("        </tr>");
            Output.WriteLine("      </table>");
            Output.WriteLine("    </td>");
            Output.WriteLine("  </tr>");

			Output.WriteLine("</table>");
			Output.WriteLine("<br />");

		}

		#endregion

		#region Methods to render (and parse) page 3 - Facets and result views

		private void Save_Page_3_Postback(NameValueCollection Form)
		{
			// Reset the facets
			itemAggregation.Clear_Facets();
			if (( Form["admin_aggr_facet1"] != null ) && ( Convert.ToInt16( Form["admin_aggr_facet1"]) > 0 ))
				itemAggregation.Add_Facet( Convert.ToInt16( Form["admin_aggr_facet1"] ));
			if (( Form["admin_aggr_facet2"] != null ) && ( Convert.ToInt16( Form["admin_aggr_facet2"]) > 0 ))
				itemAggregation.Add_Facet( Convert.ToInt16( Form["admin_aggr_facet2"] ));
			if (( Form["admin_aggr_facet3"] != null ) && ( Convert.ToInt16( Form["admin_aggr_facet3"]) > 0 ))
				itemAggregation.Add_Facet( Convert.ToInt16( Form["admin_aggr_facet3"] ));
			if (( Form["admin_aggr_facet4"] != null ) && ( Convert.ToInt16( Form["admin_aggr_facet4"]) > 0 ))
				itemAggregation.Add_Facet( Convert.ToInt16( Form["admin_aggr_facet4"] ));
			if (( Form["admin_aggr_facet5"] != null ) && ( Convert.ToInt16( Form["admin_aggr_facet5"]) > 0 ))
				itemAggregation.Add_Facet( Convert.ToInt16( Form["admin_aggr_facet5"] ));
			if (( Form["admin_aggr_facet6"] != null ) && ( Convert.ToInt16( Form["admin_aggr_facet6"]) > 0 ))
				itemAggregation.Add_Facet( Convert.ToInt16( Form["admin_aggr_facet6"] ));
			if (( Form["admin_aggr_facet7"] != null ) && ( Convert.ToInt16( Form["admin_aggr_facet7"]) > 0 ))
				itemAggregation.Add_Facet( Convert.ToInt16( Form["admin_aggr_facet7"] ));
			if (( Form["admin_aggr_facet8"] != null ) && ( Convert.ToInt16( Form["admin_aggr_facet8"]) > 0 ))
				itemAggregation.Add_Facet( Convert.ToInt16( Form["admin_aggr_facet8"] ));

			// Reset the result views
			itemAggregation.Result_Views.Clear();
			itemAggregation.Default_Result_View = Result_Display_Type_Enum.Default;

			// Add the default result view
			if (Form["admin_add_default_view"] != null)
			{
				switch( Form["admin_add_default_view"])
				{
					case "brief":
						itemAggregation.Default_Result_View = Result_Display_Type_Enum.Brief;
						itemAggregation.Result_Views.Add( Result_Display_Type_Enum.Brief );
						break;

					case "thumbnails":
						itemAggregation.Default_Result_View = Result_Display_Type_Enum.Thumbnails;
						itemAggregation.Result_Views.Add( Result_Display_Type_Enum.Thumbnails );
						break;

					case "table":
						itemAggregation.Default_Result_View = Result_Display_Type_Enum.Table;
						itemAggregation.Result_Views.Add( Result_Display_Type_Enum.Table );
						break;

                    //case "full":
                    //    itemAggregation.Default_Result_View = Result_Display_Type_Enum.Full_Citation;
                    //    itemAggregation.Result_Views.Add( Result_Display_Type_Enum.Full_Citation );
                    //    break;
				}
			}

			// Add the result views
			if ( Form["admin_aggr_result_view1"] != null ) add_result_view( Form["admin_aggr_result_view1"] );
			if ( Form["admin_aggr_result_view2"] != null ) add_result_view( Form["admin_aggr_result_view2"] );
			if ( Form["admin_aggr_result_view3"] != null ) add_result_view( Form["admin_aggr_result_view3"] );
			if ( Form["admin_aggr_result_view4"] != null ) add_result_view( Form["admin_aggr_result_view4"] );
			if ( Form["admin_aggr_result_view5"] != null ) add_result_view( Form["admin_aggr_result_view5"] );
		}

		private void add_result_view( string Result )
		{
			switch( Result )
				{
					case "brief":
						if ( !itemAggregation.Result_Views.Contains( Result_Display_Type_Enum.Brief ))
							itemAggregation.Result_Views.Add( Result_Display_Type_Enum.Brief );
						break;

					case "thumbnails":
						if ( !itemAggregation.Result_Views.Contains( Result_Display_Type_Enum.Thumbnails ))
							itemAggregation.Result_Views.Add( Result_Display_Type_Enum.Thumbnails );
						break;

					case "table":
						if ( !itemAggregation.Result_Views.Contains( Result_Display_Type_Enum.Table ))
							itemAggregation.Result_Views.Add( Result_Display_Type_Enum.Table );
						break;

                //case "full":
                //        if ( !itemAggregation.Result_Views.Contains( Result_Display_Type_Enum.Full_Citation ))
                //            itemAggregation.Result_Views.Add( Result_Display_Type_Enum.Full_Citation );
                //        break;
				}
		}

		private void Add_Page_3( TextWriter Output )
		{
			// Help constants (for now)
			const string FACETS_HELP = "When a user searches or browses a collection, the selected facets appear to the left of the search results and include all the terms in the selected metadata fields that exist in the search results.  This allows the user to easily navigate the entire set of results and narrow their search.\\n\\nYou can select which metadata fields appear in those facets by changing the values here.\\n\\nFacets will only appear if some metadata exists in the selected field in the search or browse results.";
			const string DEFAULT_VIEW_HELP = "Set the default view that will be used when a user searches or browses the items within this collection.";
			const string RESULTS_VIEWS_HELP = "Select which result views should be offered to users who search or browse the items within this collection.";

			Output.WriteLine("<table class=\"sbkAdm_PopupTable\">");

			Output.WriteLine("  <tr class=\"sbkSaav_TitleRow\"><td colspan=\"3\">Results Options</td></tr>");
			Output.WriteLine("  <tr class=\"sbkSaav_TextRow\"><td colspan=\"3\"><p>The information in this section controls how search results or item browses appears.  The facet options control which metadata values appear to the left of the results, to allow users to narrow their results.  The search results values determine which options are available for viewing the results and what are the aggregation defaults.  Finally, the result fields determines which values are displayed with each individual result in the result set.</p><p>For more information about the settings on this tab, <a href=\"" + UI_ApplicationCache_Gateway.Settings.Help_URL(RequestSpecificValues.Current_Mode.Base_URL) + "adminhelp/singleaggr\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</p></td></tr>");

			Output.WriteLine("  <tr class=\"sbkSaav_TallRow\">");
			Output.WriteLine("    <td style=\"width:50px\">&nbsp;</td>");
			Output.WriteLine("    <td style=\"width:145px\" class=\"sbkSaav_TableLabel2\"><label for=\"admin_aggr_desc\">Facets:</label></td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <table class=\"sbkSaav_InnerTable2\">");
			Output.WriteLine("        <tr style=\"vertical-align:top\">");
			Output.WriteLine("          <td>");

			for (int i = 0; i < 8; i++)
			{
				short thisFacet = -1;
				if (itemAggregation.Facets.Count > i)
					thisFacet = itemAggregation.Facets[i];
				Facet_Writer_Helper(Output, thisFacet, i + 1);
				Output.WriteLine(i < 7 ? "<br />" : String.Empty);
			}
			
			
			Output.WriteLine("          </td>");
			Output.WriteLine("          <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + FACETS_HELP + "');\"  title=\"" + FACETS_HELP + "\" /></td>");
			Output.WriteLine("         </tr>");
			Output.WriteLine("       </table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");

			// Add the default result view
			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\" style=\"height:60px\">");
			Output.WriteLine("    <td>&nbsp;</td>");
			Output.WriteLine("    <td class=\"sbkSaav_TableLabel\">Default Result View:</label></td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td>");
			Result_Writer_Helper(Output, "admin_aggr_default_view", "( NO DEFAULT )", itemAggregation.Default_Result_View, "sbkSaav_SelectSingle");
			Output.WriteLine("        </td>");
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + DEFAULT_VIEW_HELP + "');\"  title=\"" + DEFAULT_VIEW_HELP + "\" /></td></tr></table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");


			// Add all the possible result views
			Output.WriteLine("  <tr class=\"sbkSaav_TallRow\">");
			Output.WriteLine("    <td style=\"width:50px\">&nbsp;</td>");
			Output.WriteLine("    <td style=\"width:145px\" class=\"sbkSaav_TableLabel2\"><label for=\"admin_aggr_desc\">Result Views:</label></td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <table class=\"sbkSaav_InnerTable2\">");
			Output.WriteLine("        <tr style=\"vertical-align:top\">");
			Output.WriteLine("          <td>");
			for (int i = 0; i < 5; i++)
			{
				Result_Display_Type_Enum thisResult = Result_Display_Type_Enum.Default;
				if (itemAggregation.Result_Views.Count > i)
					thisResult = itemAggregation.Result_Views[i];
				if (i == 2)
				{
					Result_Writer_Helper(Output, "admin_aggr_result_view" + (i + 1).ToString(), "", thisResult, "sbkSaav_select");
					Output.WriteLine("<br />");
				}
				else
				{
					Result_Writer_Helper(Output, "admin_aggr_result_view" + (i + 1).ToString(), "", thisResult, "sbkSaav_MetadataSelect");
					Output.WriteLine(" ");
				}
			}
			Output.WriteLine("          </td>");
			Output.WriteLine("          <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + RESULTS_VIEWS_HELP + "');\"  title=\"" + RESULTS_VIEWS_HELP + "\" /></td>");
			Output.WriteLine("         </tr>");
			Output.WriteLine("       </table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");
			Output.WriteLine("</table>");

		}

		private void Result_Writer_Helper( TextWriter Output, string FieldName, string NoOption, Result_Display_Type_Enum Result_Type, string HtmlClass )
		{
			// Start the select box
			Output.Write("<select class=\"" + HtmlClass + "\" name=\"" + FieldName + "\" id=\"" + FieldName + "\">");

			// Add the NONE option first
			if (Result_Type ==  Result_Display_Type_Enum.Default )
			{
				Output.Write("<option value=\"\" selected=\"selected\" >" + NoOption + "</option>");
			}
			else
			{
				Output.Write("<option value=\"\">" + NoOption + "</option>");
			}

			// Add each result view to the select boxes ( brief, map, table, thumbnails, full )
			Output.Write(Result_Type == Result_Display_Type_Enum.Brief ? "<option value=\"brief\" selected=\"selected\" >Brief View</option>" : "<option value=\"brief\">Brief View</option>");

			Output.Write(Result_Type == Result_Display_Type_Enum.Table ? "<option value=\"table\" selected=\"selected\" >Table View</option>" : "<option value=\"table\">Table View</option>");

			Output.Write(Result_Type == Result_Display_Type_Enum.Thumbnails ? "<option value=\"thumbnails\" selected=\"selected\" >Thumbnail View</option>" : "<option value=\"thumbnails\">Thumbnail View</option>");

			//Output.Write(Result_Type == Result_Display_Type_Enum.Full_Citation ? "<option value=\"full\" selected=\"selected\" >Full View</option>" : "<option value=\"full\">Full View</option>");
			Output.WriteLine("</select>");
		}

		private void Facet_Writer_Helper(  TextWriter Output, short FacetID, int FacetCounter )
		{
			// Start the select box
			Output.Write("<select class=\"sbkSaav_select\" name=\"admin_aggr_facet" + FacetCounter + "\" id=\"admin_aggr_facet" + FacetCounter + "\">");

			// Add the NONE option first
			Output.Write(FacetID == - 1 ? "<option value=\"-1\" selected=\"selected\" ></option>" : "<option value=\"-1\"></option>");

			// Add each metadata field to the select boxes
			foreach (Metadata_Search_Field metadataField in UI_ApplicationCache_Gateway.Settings.Metadata_Search_Fields )
			{
				if (metadataField.Web_Code.Length > 0)
				{
					// Anywhere as -1 is in the list, so leave that out
					if ((metadataField.ID > 0) && (metadataField.Display_Term != "Undefined"))
					{
						if (metadataField.ID == FacetID)
						{
							Output.Write("<option value=\"" + metadataField.ID + "\" selected=\"selected\" >" + HttpUtility.HtmlEncode(metadataField.Display_Term) + "</option>");
						}
						else
						{
							Output.Write("<option value=\"" + metadataField.ID + "\">" + HttpUtility.HtmlEncode(metadataField.Display_Term) + "</option>");
						}
					}
				}
			}
			Output.WriteLine("</select>");
		}
		#endregion

		#region Methods to render (and parse) page 4 - Metadata (Metadata browses and OAI/PMH)

		private void Save_Page_4_Postback(NameValueCollection Form)
		{
			// Get the metadata browses
			List<Complete_Item_Aggregation_Child_Page> metadata_browse_bys = itemAggregation.Browse_By_Pages(UI_ApplicationCache_Gateway.Settings.Default_UI_Language).Where(ThisBrowse => ThisBrowse.Browse_Type == Item_Aggregation_Child_Visibility_Enum.Metadata_Browse_By).Where(ThisBrowse => ThisBrowse.Source_Data_Type == Item_Aggregation_Child_Source_Data_Enum.Database_Table).ToList();

			// Remove all these browse by's
            foreach (Complete_Item_Aggregation_Child_Page browseBy in metadata_browse_bys)
			{
				itemAggregation.Remove_Child_Page(browseBy);
			}

			// Look for the default browse by
			short default_browseby_id = 0;
			itemAggregation.Default_BrowseBy = null;
			if (Form["admin_aggr_default_browseby"] != null)
			{
				string default_browseby = Form["admin_aggr_default_browseby"];
				if (Int16.TryParse(default_browseby, out default_browseby_id))
				{
					if (default_browseby_id > 0)
					{
						Metadata_Search_Field field = UI_ApplicationCache_Gateway.Settings.Metadata_Search_Field_By_ID(default_browseby_id);
						if (field != null)
						{
							Complete_Item_Aggregation_Child_Page newBrowse = new Complete_Item_Aggregation_Child_Page(Item_Aggregation_Child_Visibility_Enum.Metadata_Browse_By, Item_Aggregation_Child_Source_Data_Enum.Database_Table, field.Display_Term, String.Empty, field.Display_Term);
							itemAggregation.Add_Child_Page(newBrowse);
							itemAggregation.Default_BrowseBy = field.Display_Term;
						}
					}
				}
				else
				{
					itemAggregation.Default_BrowseBy = default_browseby;
				}
			}

			// Now, get all the new browse bys
			for (int i = 0; i < metadata_browse_bys.Count + 10; i++)
			{
				if (Form["admin_aggr_browseby_" + i] != null)
				{
					short browseby_id = Convert.ToInt16(Form["admin_aggr_browseby_" + i]);
					if ((browseby_id > 0) && (default_browseby_id != browseby_id))
					{
						Metadata_Search_Field field = UI_ApplicationCache_Gateway.Settings.Metadata_Search_Field_By_ID(browseby_id);
						if (field != null)
						{
                            Complete_Item_Aggregation_Child_Page newBrowse = new Complete_Item_Aggregation_Child_Page(Item_Aggregation_Child_Visibility_Enum.Metadata_Browse_By, Item_Aggregation_Child_Source_Data_Enum.Database_Table, field.Display_Term, String.Empty, field.Display_Term);
							itemAggregation.Add_Child_Page(newBrowse);
						}
					}
				}
			}

			itemAggregation.OAI_Enabled = Form["admin_aggr_oai_flag"] != null;

			if (Form["admin_aggr_oai_metadata"] != null)
				itemAggregation.OAI_Metadata = Form["admin_aggr_oai_metadata"];
		}

		private void Add_Page_4(TextWriter Output)
		{
			// Get the metadata browses
			List<string> metadata_browse_bys = new List<string>();
			string default_browse_by = itemAggregation.Default_BrowseBy ?? String.Empty;
			List<string> otherBrowseBys = new List<string>();
            foreach (Complete_Item_Aggregation_Child_Page thisBrowse in itemAggregation.Browse_By_Pages(UI_ApplicationCache_Gateway.Settings.Default_UI_Language))
			{
				if (thisBrowse.Browse_Type == Item_Aggregation_Child_Visibility_Enum.Metadata_Browse_By)
				{
					if (thisBrowse.Source_Data_Type == Item_Aggregation_Child_Source_Data_Enum.Database_Table)
					{
						metadata_browse_bys.Add(thisBrowse.Code);
					}
					else
					{
						otherBrowseBys.Add(thisBrowse.Code);
					}
				}
			}

			// Help constants (for now)
			const string DEFAULT_HELP = "Use this option to select the default metadata BROWSE BY to display if the user selects the BROWSE BY option on the main collection menu, without selecting the specific metadata browse by to view.";
			const string METADATA_BROWSES_HELP = "By selecting certain metadata fields to be browseable, users can choose to view all the distinct values within a particular metadata field.  These browses become accessible from the collection main menu.";
			const string OAI_FLAG_HELP = "Enable or disable OAI-PMH for this collection.  If enabled, this collection becomes a set offered for individual browsing via the OAI-PMH metadata sharing protocol.";
			const string OAI_METADATA_HELP = "Additional metadata included here will show with this set when a OAI-PMH user lists the sets within this repository.  This has limited usability, as most harvesters probably do not harvest this data.\\n\\nTo use this, your included metadata which describes the entire collection should be included within dublin core tags, such as <dc:subject>World War II</dc:subject>.";


			Output.WriteLine("<table class=\"sbkAdm_PopupTable\">");

			Output.WriteLine("  <tr class=\"sbkSaav_TitleRow\"><td colspan=\"3\">Metadata Browses</td></tr>");
			Output.WriteLine("  <tr class=\"sbkSaav_TextRow\"><td colspan=\"3\"><p>The metadata browses can be used to expose all the metadata of the resources within this aggregation for public browsing.  Select the metadata fields you would like have available below.</p><p>For more information about the settings on this tab, <a href=\"" + UI_ApplicationCache_Gateway.Settings.Help_URL(RequestSpecificValues.Current_Mode.Base_URL) + "adminhelp/singleaggr\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</p></td></tr>");

			// Add the default metadata browse view view
			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
			Output.WriteLine("    <td style=\"width:50px\">&nbsp;</td>");
			Output.WriteLine("    <td style=\"width:145px\" class=\"sbkSaav_TableLabel\"><label for=\"admin_aggr_default_browseby\">Default Browse:</label></td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td>");
			BrowseBy_Writer_Helper(Output, "admin_aggr_default_browseby", "( NO DEFAULT )", default_browse_by, otherBrowseBys.ToArray(), "sbkSaav_SelectSingle");
			Output.WriteLine("        </td>");
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + DEFAULT_HELP + "');\"  title=\"" + DEFAULT_HELP + "\" /></td></tr></table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");

			// Add all the other metadata browses
			Output.WriteLine("  <tr class=\"sbkSaav_TallRow\">");
			Output.WriteLine("    <td>&nbsp;</td>");
			Output.WriteLine("    <td class=\"sbkSaav_TableLabel2\">Metadata Browses:</td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <table class=\"sbkSaav_InnerTable2\">");
			Output.WriteLine("        <tr style=\"vertical-align:top\">");
			Output.WriteLine("          <td>");

			// Get the additional values include
			string[] empty_set = new string[0];

			// Add all the browse by's
			int column = 0;
			int additional_boxes = (3 - (metadata_browse_bys.Count%3)) + 9;
			for (int i = 0; i < metadata_browse_bys.Count + additional_boxes; i++)
			{
				string browse_by = String.Empty;
				if (i < metadata_browse_bys.Count)
					browse_by = metadata_browse_bys[i];
				
				column++;
				if (column == 3)
				{
					BrowseBy_Writer_Helper(Output, "admin_aggr_browseby_" + i, String.Empty, browse_by, empty_set, "sbkSaav_select");
					Output.WriteLine("<br />");
					column = 0;
				}
				else
				{
					BrowseBy_Writer_Helper(Output, "admin_aggr_browseby_" + i, String.Empty, browse_by, empty_set, "sbkSaav_MetadataSelect");
					Output.WriteLine(" ");
				}
			}
			Output.WriteLine("          </td>");
			Output.WriteLine("          <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + METADATA_BROWSES_HELP + "');\"  title=\"" + METADATA_BROWSES_HELP + "\" /></td>");
			Output.WriteLine("         </tr>");
			Output.WriteLine("       </table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");


			Output.WriteLine("  <tr class=\"sbkSaav_TitleRow2\"><td colspan=\"3\">OAI-PMH Settings</td></tr>");
			Output.WriteLine("  <tr class=\"sbkSaav_TextRow\"><td colspan=\"3\"><p>You can use OAI-PMH to expose all the metadata of the resources within this aggregation for automatic harvesting by other repositories.  Additionally, you can choose to attach metadata to the collection-level OAI-PMH record.  This should be coded as dublin core tags.</p></td></tr>");


			// Add the oai-pmh enabled flag
			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
			Output.WriteLine("    <td>&nbsp;</td>");
			Output.WriteLine("    <td class=\"sbkSaav_TableLabel\">Enabled Flag:</td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td>");
			Output.Write("           <input class=\"sbkSaav_checkbox\" type=\"checkbox\" name=\"admin_aggr_oai_flag\" id=\"admin_aggr_oai_flag\"");
			if (itemAggregation.OAI_Enabled)
				Output.Write(" checked=\"checked\"");
			Output.WriteLine(" />");
			Output.WriteLine("           <label for=\"admin_aggr_oai_flag\">Include in OAI-PMH as a set?</label>");
			Output.WriteLine("        </td>");
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + OAI_FLAG_HELP + "');\"  title=\"" + OAI_FLAG_HELP + "\" /></td></tr></table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");

			// Add label for adding metadata to this OAI-SET
			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
			Output.WriteLine("    <td>&nbsp;</td>");
			Output.WriteLine("    <td class=\"sbkSaav_TableLabel\" colspan=\"2\">");
			Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr>");
			Output.WriteLine("        <td><label for=\"admin_aggr_oai_metadata\">Additional dublin core metadata to include in OAI-PMH set list:</label></td>");
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + OAI_METADATA_HELP + "');\"  title=\"" + OAI_METADATA_HELP + "\" /></td></tr></table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");



			// Add text box for adding metadata to this OAI-SET
			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
			Output.WriteLine("    <td colspan=\"2\">&nbsp;</td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("       <textarea rows=\"12\" name=\"admin_aggr_oai_metadata\" id=\"admin_aggr_oai_metadata\" class=\"sbkSaav_large_textbox sbkAdmin_Focusable\">" + HttpUtility.HtmlEncode(itemAggregation.OAI_Metadata) + "</textarea>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");
			Output.WriteLine("</table>");
			Output.WriteLine("<br />");
		}

		private void BrowseBy_Writer_Helper(TextWriter Output, string ID, string Default, string Value, IEnumerable<string> OtherValues, string HtmlClass )
		{
			// Start the select box
			Output.Write("<select class=\"" + HtmlClass + "\" name=\"" + ID + "\" id=\"" + ID + "\">");

			// Add the NONE option first
			if (Value.Length == 0)
			{
				Output.Write("<option value=\"-1\" selected=\"selected\" >" + Default + "</option>");
			}
			else
			{
				Output.Write("<option value=\"-1\">" + Default + "</option>");
			}

			// Add any other values
			foreach (string thisOtherValue in OtherValues)
			{
				if (String.Equals(thisOtherValue, Value, StringComparison.OrdinalIgnoreCase))
				{
					Output.Write("<option value=\"" + thisOtherValue + "\" selected=\"selected\" >" + thisOtherValue + "</option>");
				}
				else
				{
					Output.Write("<option value=\"" + thisOtherValue + "\">" + thisOtherValue + "</option>");
				}
			}

			// Add each metadata field to the select boxes
			foreach (Metadata_Search_Field metadataField in UI_ApplicationCache_Gateway.Settings.Metadata_Search_Fields)
			{
				if (metadataField.Web_Code.Length > 0)
				{
					// Anywhere as -1 is in the list, so leave that out
					if ((metadataField.ID > 0) && (metadataField.Display_Term != "Undefined"))
					{
						if (String.Equals(metadataField.Display_Term, Value, StringComparison.OrdinalIgnoreCase))
						{
							Output.Write("<option value=\"" + metadataField.ID + "\" selected=\"selected\" >" + HttpUtility.HtmlEncode(metadataField.Display_Term) + "</option>");
						}
						else
						{
							Output.Write("<option value=\"" + metadataField.ID + "\">" + HttpUtility.HtmlEncode(metadataField.Display_Term) + "</option>");
						}
					}
				}
			}
			Output.WriteLine("</select>");
		}

		#endregion

		#region Methods to render (and parse) page 6 - Highlights

		private void Save_Page_6_Postback()
		{
            // This does not currently save
		}

		private void Add_Page_6(TextWriter Output)
		{
			Output.WriteLine("<table class=\"popup_table\">");

			// Add the highlight type
			Output.Write("<tr><td width=\"120px\">Highlights Type:</td><td><input type=\"radio\" name=\"admin_aggr_highlight_type\" id=\"rotating\" value=\"rotating\"");
			if ((itemAggregation.Rotating_Highlights.HasValue ) && ( itemAggregation.Rotating_Highlights.Value ))
				Output.Write(" checked=\"checked\"");
			Output.Write("/><label for=\"rotating\">Rotating</label> &nbsp; <input type=\"radio\" name=\"admin_aggr_highlight_type\" id=\"static\" value=\"static\"");
			if ((!itemAggregation.Rotating_Highlights.HasValue ) || (!itemAggregation.Rotating_Highlights.Value))
				Output.Write(" checked=\"checked\"");
			Output.WriteLine("/><label for=\"static\">Static</label></td></tr>");

			// Determine the maximum number of languages used in tooltips and text
			int max_tooltips = 0;
			int max_text = 0;
		    if (itemAggregation.Highlights != null)
		    {
		        foreach (Complete_Item_Aggregation_Highlights thisHighlight in itemAggregation.Highlights)
		        {
		            max_tooltips = Math.Max(max_tooltips, thisHighlight.Tooltip_Dictionary.Count);
		            max_text = Math.Max(max_text, thisHighlight.Text_Dictionary.Count);
		        }
		    }
		    max_tooltips += 1;
			max_text += 1;

			// Add each highlight
		    if (itemAggregation.Highlights != null)
		    {
		        for (int i = 0; i < itemAggregation.Highlights.Count + 5; i++)
		        {
		            // Add some space and a line
		            Output.WriteLine("<tr><td colspan=\"2\">&nbsp;</td></tr>");
		            Output.WriteLine("<tr style=\"background:#333333\"><td colspan=\"2\"></td></tr>");
		            Output.WriteLine("<tr><td colspan=\"2\">&nbsp;</td></tr>");

		            // Either get the highlight, or just make one
                    Complete_Item_Aggregation_Highlights emptyHighlight = new Complete_Item_Aggregation_Highlights();
		            if (i < itemAggregation.Highlights.Count)
		                emptyHighlight = itemAggregation.Highlights[i];

		            // Now, add it to the form
		            Highlight_Writer_Helper(Output, i + 1, emptyHighlight, max_text, max_tooltips);
		        }
		    }

		    Output.WriteLine("</table>");
			Output.WriteLine("<br />");
		}

		private void Highlight_Writer_Helper(TextWriter Output, int HighlightCounter, Complete_Item_Aggregation_Highlights Highlight, int Max_Text, int Max_Tooltips)
		{
			// Add the image line
			Output.WriteLine("<tr><td> &nbsp; &nbsp; <label for=\"admin_aggr_image_" + HighlightCounter + "\">Image:</label></td><td colspan=\"2\"><input class=\"admin_aggr_large_input\" name=\"admin_aggr_image_" + HighlightCounter + "\" id=\"admin_aggr_image_" + HighlightCounter + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(Highlight.Image) + "\" onfocus=\"javascript:textbox_enter('admin_aggr_image_" + HighlightCounter + "', 'admin_aggr_large_input_focused')\" onblur=\"javascript:textbox_leave('admin_aggr_image_" + HighlightCounter + "', 'admin_aggr_large_input')\" /></td></tr>");

			// Add the link line
			Output.WriteLine("<tr><td> &nbsp; &nbsp; <label for=\"admin_aggr_link_" + HighlightCounter + "\">Link:</label></td><td colspan=\"2\"><input class=\"admin_aggr_large_input\" name=\"admin_aggr_link_" + HighlightCounter + "\" id=\"admin_aggr_image_" + HighlightCounter + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(Highlight.Link) + "\" onfocus=\"javascript:textbox_enter('admin_aggr_image_" + HighlightCounter + "', 'admin_aggr_large_input_focused')\" onblur=\"javascript:textbox_leave('admin_aggr_link_" + HighlightCounter + "', 'admin_aggr_large_input')\" /></td></tr>");

			// Add lines for the text
			Output.Write(Max_Text == 1 ? "<tr><td> &nbsp; &nbsp; Text:</td><td>" : "<tr valign=\"top\"><td><br /> &nbsp; &nbsp; Text:</td><td>");
			for (int j = 0; j < Max_Text; j++)
			{
				Web_Language_Enum language = UI_ApplicationCache_Gateway.Settings.Default_UI_Language;
				string text = String.Empty;
				if (j < Highlight.Text_Dictionary.Count)
				{
					language = Highlight.Text_Dictionary.Keys.ElementAt(j);
					text = Highlight.Text_Dictionary[language];
				}

				// Start the select box
				string id = "admin_aggr_text_lang_" + HighlightCounter + "_" + (j + 1).ToString();
				string id2 = "admin_aggr_text_" + HighlightCounter + "_" + (j + 1).ToString();
				Output.Write("<select class=\"admin_aggr_select2\" name=\"" + id + "\" id=\"" + id + "\">");

				// Add each language in the combo box
				foreach (string possible_language in Web_Language_Enum_Converter.Language_Name_Array)
				{
					if (language == Web_Language_Enum_Converter.Code_To_Enum(possible_language))
					{
						Output.Write("<option value=\"" + possible_language + "\" selected=\"selected\" >" + HttpUtility.HtmlEncode(possible_language) + "</option>");
					}
					else
					{
						Output.Write("<option value=\"" + possible_language + "\">" + HttpUtility.HtmlEncode(possible_language) + "</option>");
					}
				}
				Output.WriteLine("</select> &nbsp; &nbsp; ");

				// Add the text to the text box
				Output.Write("<input class=\"admin_aggr_medium_input\" name=\"" + id2 + "\" id=\"" + id2 + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(text) + "\" onfocus=\"javascript:textbox_enter('" + id2 + "', 'admin_aggr_medium_input_focused')\" onblur=\"javascript:textbox_leave('" + id2 + "', 'admin_aggr_medium_input')\" /><br />");
			}
			Output.WriteLine("</td></tr>");

			// Add lines for the tooltips
			Output.Write(Max_Tooltips == 1 ? "<tr><td> &nbsp; &nbsp; Tooltip:</td><td>" : "<tr valign=\"top\"><td><br /> &nbsp; &nbsp; Tooltip:</td><td>");
			for (int j = 0; j < Max_Tooltips; j++)
			{
				Web_Language_Enum language = UI_ApplicationCache_Gateway.Settings.Default_UI_Language;
				string text = String.Empty;
				if (j < Highlight.Tooltip_Dictionary.Count)
				{
					language = Highlight.Tooltip_Dictionary.Keys.ElementAt(j);
					text = Highlight.Tooltip_Dictionary[language];
				}

				// Start the select box
				string id = "admin_aggr_tooltip_lang_" + HighlightCounter + "_" + (j + 1).ToString();
				string id2 = "admin_aggr_tooltip_" + HighlightCounter + "_" + (j + 1).ToString();
				Output.Write("<select class=\"admin_aggr_select2\" name=\"" + id + "\" id=\"" + id + "\">");

				// Add each language in the combo box
				foreach (string possible_language in Web_Language_Enum_Converter.Language_Name_Array)
				{
					if (language == Web_Language_Enum_Converter.Code_To_Enum(possible_language))
					{
						Output.Write("<option value=\"" + possible_language + "\" selected=\"selected\" >" + HttpUtility.HtmlEncode(possible_language) + "</option>");
					}
					else
					{
						Output.Write("<option value=\"" + possible_language + "\">" + HttpUtility.HtmlEncode(possible_language) + "</option>");
					}
				}
				Output.WriteLine("</select> &nbsp; &nbsp; ");

				// Add the text to the text box
				Output.Write("<input class=\"admin_aggr_medium_input\" name=\"" + id2 + "\" id=\"" + id2 + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(text) + "\" onfocus=\"javascript:textbox_enter('" + id2 + "', 'admin_aggr_medium_input_focused')\" onblur=\"javascript:textbox_leave('" + id2 + "', 'admin_aggr_medium_input')\" /><br />");
			}
			Output.WriteLine("</td></tr>");

			// Add a delete option
			Output.WriteLine("<tr><td colspan=\"2\" align=\"right\"><a href=\"\">DELETE THIS HIGHLIGHT</a></td></tr>");

		}

		#endregion

		#region Methods to render (and parse) page 7 - Child pages

		private void Save_Page_7_Postback(NameValueCollection Form)
		{
			string action = Form["admin_aggr_action"];
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

		private void Add_Page_7(TextWriter Output)
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

		#region Methods to render (and parse) page 8 -  Subcollections

		private void Save_Page_8_Postback(NameValueCollection Form)
		{
			string action = Form["admin_aggr_action"];
			if ((String.IsNullOrEmpty(action)) || ((action != "save_aggr") && ( action.IndexOf("delete_") < 0 )))
			{
				return;
			}

		
			// Was this to delete the aggregation?
			if ((action.IndexOf("delete_") == 0) && ( action.Length > 7))
			{
				string code_to_delete = action.Substring(7);

				string delete_error;
				int errorCode = SobekCM_Database.Delete_Item_Aggregation(code_to_delete, RequestSpecificValues.Current_User.Is_System_Admin, RequestSpecificValues.Current_User.Full_Name, null, out delete_error);
				if (errorCode <= 0)
				{
					string delete_folder = UI_ApplicationCache_Gateway.Settings.Base_Design_Location + "aggregations\\" + code_to_delete;
					if (!SobekCM_File_Utilities.Delete_Folders_Recursively(delete_folder))
						actionMessage = "Deleted '" + code_to_delete + "' subcollection<br /><br />Unable to remove subcollection directory<br /><br />Some of the files may be in use";
					else
						actionMessage = "Deleted '" + code_to_delete + "' subcollection";

					itemAggregation.Remove_Child(code_to_delete);
					HttpContext.Current.Session["Edit_Aggregation_" + itemAggregation.Code] = itemAggregation;

				}
				else
				{
					actionMessage = delete_error;
				}


				// Reload the list of all codes, to include this new one and the new hierarchy
                lock (UI_ApplicationCache_Gateway.Aggregations)
				{
                    Engine_Database.Populate_Code_Manager(UI_ApplicationCache_Gateway.Aggregations, null);
				}
			}
		}

	    private void Add_Page_8(TextWriter Output)
	    {
	        //	const string NEW_SUBCOLLECTION_HELP = "New subcollection help place holder";


	        if (actionMessage.Length > 0)
	        {
	            Output.WriteLine("  <br />");
	            Output.WriteLine("  <div id=\"sbkAdm_ActionMessage\" style=\"color:Maroon;\">" + actionMessage + "</div>");
	        }


	        Output.WriteLine("<table class=\"sbkAdm_PopupTable\">");

	        Output.WriteLine("  <tr class=\"sbkSaav_TitleRow\"><td colspan=\"3\">SubCollections</td></tr>");
	        Output.WriteLine("  <tr class=\"sbkSaav_TextRow\"><td colspan=\"3\"><p>You can view existing subcollections or add new subcollections to this aggregation from this tab.  You will have full curatorial rights over any new subcollections you add.  Currently, only system administrators can DELETE subcollections.</p><p>For more information about the settings on this tab, <a href=\"" + UI_ApplicationCache_Gateway.Settings.Help_URL(RequestSpecificValues.Current_Mode.Base_URL) + "adminhelp/singleaggr\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</p></td></tr>");

	        if (itemAggregation.Children_Count <= 0)
	        {
	            Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
	            Output.WriteLine("    <td style=\"width:50px\">&nbsp;</td>");
	            Output.WriteLine("    <td style=\"width: 165px\" class=\"sbkSaav_TableLabel\">Existing Subcollections:</td>");
	            Output.WriteLine("    <td style=\"font-style:italic\">This aggregation currently has no subcollections</td>");
	            Output.WriteLine("  </tr>");
	        }
	        else
	        {
	            // Add EXISTING subcollections
	            Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
	            Output.WriteLine("    <td style=\"width:50px\">&nbsp;</td>");
	            Output.WriteLine("    <td class=\"sbkSaav_TableLabel2\" colspan=\"2\">Existing Subcollections:</td>");
	            Output.WriteLine("  </tr>");
	            Output.WriteLine("  <tr class=\"sbkSaav_TallRow\">");
	            Output.WriteLine("    <td>&nbsp;</td>");
	            Output.WriteLine("    <td colspan=\"2\">");
	            Output.WriteLine("      <table class=\"sbkSaav_SubCollectionTable sbkSaav_Table\">");
	            Output.WriteLine("        <tr>");
	            Output.WriteLine("          <th class=\"sbkSaav_SubCollectionTableHeader1\">ACTION</th>");
	            Output.WriteLine("          <th class=\"sbkSaav_SubCollectionTableHeader2\">CODE</th>");
	            Output.WriteLine("          <th class=\"sbkSaav_SubCollectionTableHeader3\">TYPE</th>");
                Output.WriteLine("          <th class=\"sbkSaav_SubCollectionTableHeader5\">NAME</th>");
	            Output.WriteLine("          <th class=\"sbkSaav_SubCollectionTableHeader4\">ACTIVE</th>");
                Output.WriteLine("          <th class=\"sbkSaav_SubCollectionTableHeader6\">ON HOME</th>");
	            Output.WriteLine("        </tr>");

	            // Put in alphabetical order
	            SortedDictionary<string, Item_Aggregation_Related_Aggregations> sortedChildren = new SortedDictionary<string, Item_Aggregation_Related_Aggregations>();
	            foreach (Item_Aggregation_Related_Aggregations childAggrs in itemAggregation.Children)
	                sortedChildren[childAggrs.Code] = childAggrs;

	            foreach (KeyValuePair<string, Item_Aggregation_Related_Aggregations> childAggrs in sortedChildren)
	            {
	                string code = childAggrs.Key;
	                Item_Aggregation_Related_Aggregations relatedAggr = UI_ApplicationCache_Gateway.Aggregations[code];
	                if (relatedAggr != null)
	                {
	                    Output.WriteLine("        <tr>");
	                    Output.Write("          <td class=\"sbkAdm_ActionLink\" style=\"padding-left: 5px;\" >( ");
	                    RequestSpecificValues.Current_Mode.Aggregation = childAggrs.Key;
	                    RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Aggregation;
	                    RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Home;
	                    Output.Write("<a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\" title=\"View this subcollection\" target=\"VIEW_" + childAggrs.Key + "\">view</a> | ");

	                    RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Administrative;
	                    RequestSpecificValues.Current_Mode.My_Sobek_SubMode = childAggrs.Key;
	                    Output.Write("<a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\" title=\"Edit this subcollection\" target=\"EDIT_" + childAggrs.Key + "\">edit</a> | ");
	                    Output.WriteLine("<a title=\"Click to delete this subcollection\" href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return edit_aggr_delete_child_aggr('" + childAggrs.Value.Code + "');\">delete</a> )</td>");


	                    Output.WriteLine("          <td>" + childAggrs.Key + "</td>");
	                    Output.WriteLine("          <td>" + childAggrs.Value.Type + "</td>");
                        Output.WriteLine("          <td>" + relatedAggr.Name + "</td>");

	                    if (relatedAggr.Active)
	                        Output.WriteLine("          <td style=\"text-align: center\"><img src=\"" + Static_Resources.Checkmark2_Png + "\" alt=\"YES\" /></td>");
	                    else
                            Output.WriteLine("          <td style=\"text-align: center\"><img src=\"" + Static_Resources.Checkmark_Png + "\" alt=\"NO\" /></td>");


                        if (!relatedAggr.Hidden)
                            Output.WriteLine("          <td style=\"text-align: center\"><img src=\"" + Static_Resources.Checkmark2_Png + "\" alt=\"YES\" /></td>");
                        else
                            Output.WriteLine("          <td style=\"text-align: center\"><img src=\"" + Static_Resources.Checkmark_Png + "\" alt=\"NO\" /></td>");

	                    
	                    Output.WriteLine("        </tr>");
	                }
	            }
	            RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Administrative;
	            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = itemAggregation.Code;

	            Output.WriteLine("      </table>");
	            Output.WriteLine("    </td>");
	            Output.WriteLine("  </tr>");
	        }


	        // Add ability to add NEW subcollections
	        Output.WriteLine("  <tr class=\"sbkSaav_TallRow\">");
	        Output.WriteLine("    <td style=\"width:50px\">&nbsp;</td>");
	        Output.WriteLine("    <td class=\"sbkSaav_TableLabel2\" style=\"width:145px\">New Subcollection:</td>");
	        Output.WriteLine("    <td>");
            Output.WriteLine("  <table>");
            Output.WriteLine("    <tr>");
            Output.WriteLine("      <td>");
            Output.WriteLine("        Use the new Add New Collection Wizard to add a single new subcollection.");
            Output.WriteLine("      </td>");
            Output.WriteLine("      <td style=\"padding-left: 30px;\">");
            Output.WriteLine("        <button title=\"Use the wizard to add a new collection\" class=\"sbkAdm_RoundButton\" onclick=\"return save_wizard();\"> &nbsp; NEW COLLECTION &nbsp; <br />WIZARD</button>");
            Output.WriteLine("      </td>");
            Output.WriteLine("    </tr>");
            Output.WriteLine("  </table>");


	        Output.WriteLine("    </td>");
	        Output.WriteLine("  </tr>");

	        Output.WriteLine("</table>");
	    }


	    #endregion

        #region Methods to render (and parse) page 9 -  Uploads

        private void Save_Page_Uploads_Postback(NameValueCollection Form)
        {
            if (HttpContext.Current.Session[itemAggregation.Code + "|Uploads"] != null)
            {
                string files = HttpContext.Current.Session[itemAggregation.Code + "|Uploads"].ToString().Replace("|", ", ");
                SobekCM_Database.Save_Item_Aggregation_Milestone(itemAggregation.Code, "Uploaded file(s) " + files, RequestSpecificValues.Current_User.Full_Name);
                HttpContext.Current.Session.Remove(itemAggregation.Code + "|Uploads");
            }
            string action = Form["admin_aggr_action"];
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
			string action = Form["admin_aggr_action"];
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
			Output.WriteLine("      <button title=\"Do not apply changes\" class=\"sbkAdm_RoundButton\" onclick=\"return new_aggr_edit_page('e');\"><img src=\"" + Static_Resources.Button_Previous_Arrow_Png + "\" class=\"sbkAdm_RoundButton_LeftImg\" alt=\"\" /> CANCEL</button> &nbsp; &nbsp; ");
			Output.WriteLine("      <button title=\"Save changes to this stylesheet\" class=\"sbkAdm_RoundButton\" onclick=\"return save_css_edits();\">SAVE <img src=\"" + Static_Resources.Button_Next_Arrow_Png + "\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button>");
			Output.WriteLine("    </td>");
			Output.WriteLine("  </tr>");


			Output.WriteLine("</table>");
			Output.WriteLine("<br />");
		}

		#endregion

		#region Methods to render (and parse) the single Child Page 

		private void Save_Child_Page_Postback(NameValueCollection Form)
		{
			string code = RequestSpecificValues.Current_Mode.My_Sobek_SubMode.Substring(2);
			Complete_Item_Aggregation_Child_Page childPage = itemAggregation.Child_Page_By_Code(code);

			// Check for action flag
			string action = Form["admin_aggr_action"];
			if (action == "add_version")
			{
				try
				{
					string language = Form["admin_aggr_new_version_lang"];
					string title = Form["admin_aggr_new_version_label"];
					string copyFrom = Form["admin_aggr_new_version_copy"];

					string file = "html\\browse\\" + childPage.Code + "_" + language + ".html";
					string fileDir = aggregationDirectory + "\\" + file;
					Web_Language_Enum languageEnum = Web_Language_Enum_Converter.Code_To_Enum(language);

					// Create the source file FIRST
					string copyFromFull = aggregationDirectory + "\\" + copyFrom;
					if ((copyFrom.Length > 0) && (File.Exists(copyFromFull)))
					{
						File.Copy(copyFromFull, fileDir, true );
					}
					else if ( !File.Exists(fileDir))
					{
						HTML_Based_Content htmlContent = new HTML_Based_Content
						{
						    Content = "<br /><br />This is a new " + Web_Language_Enum_Converter.Enum_To_Name(languageEnum) + " browse page.<br /><br />" + title + "<br /><br />The code for this browse is: " + childPage.Code, 
                            Author = RequestSpecificValues.Current_User.Full_Name, 
                            Date = DateTime.Now.ToLongDateString(), 
                            Title = title
						};
					    htmlContent.Save_To_File(fileDir);
					}

					// Add to this child page
					childPage.Add_Label(title, languageEnum);
					childPage.Add_Static_HTML_Source(file, languageEnum);

				}
				catch
				{
					actionMessage = "Error adding new version to this child page";
				}

			}
            else if ((action.IndexOf("delete_") == 0) && (action.Length > 7))
            {
                string delete_code = action.Substring(7);
                childPage.Remove_Language(Web_Language_Enum_Converter.Code_To_Enum(delete_code));

            }
            else
            {
                childPageVisibility = Form["admin_aggr_visibility"];
                childPage.Parent_Code = Form["admin_aggr_parent"];

                switch (childPageVisibility)
                {
                    case "none":
                        childPage.Browse_Type = Item_Aggregation_Child_Visibility_Enum.None;
                        break;

                    case "browse":
                        childPage.Browse_Type = Item_Aggregation_Child_Visibility_Enum.Main_Menu;
                        break;

                    case "browseby":
                        childPage.Browse_Type = Item_Aggregation_Child_Visibility_Enum.Metadata_Browse_By;
                        break;
                }
            }
		}

		private void Add_Child_Page(TextWriter Output)
		{
            const string VISIBILITY_HELP = "Choose how a link to this child page should appear for the web users.\\n\\nIf you select MAIN MENU, this will appear in the collection main menu system.\\n\\nIf you select BROWSE BY, this will appear with metadata browse bys on the main menu under the BROWSE BY menu item.\\n\\nIf you select NONE, then you will need to add a link to the new child page yourself by editing the text of the home page or an existing linked child page.";
            const string PARENT_HELP = "If this child page will appear on the main menu, you can select a parent child page already on the main menu.  This will create a drop down menu under, or next to, the parent.";
			const string NEW_VERSION_LANGUAGE_HELP = "To add a translated version, or alternate language version, to an existing child page, select the new language you wish to support.";
            const string NEW_VERSION_TITLE_HELP = "Enter the translated title for the new language support you are adding to this child page.  This title should be short, but can include spaces.  This will appear above the child page text.  If this child page appears in the main menu, this will also appear on the menu.  If this child page appears as a browse by, this will appear in the list of possible browse bys as well.";
			const string NEW_VERSION_COPY_HELP = "Choose which existing child page text to copy this new language support page from.  Without copying the text from an existing version, the text for the new version of the child page will begin blank.";

			string code = RequestSpecificValues.Current_Mode.My_Sobek_SubMode.Substring(2);
			Complete_Item_Aggregation_Child_Page childPage = itemAggregation.Child_Page_By_Code(code);

			Output.WriteLine("<table class=\"sbkAdm_PopupTable\">");

			Output.WriteLine("  <tr class=\"sbkSaav_TitleRow\"><td colspan=\"3\">Edit Child Page Details : " + code.ToUpper() + "</td></tr>");
			Output.WriteLine("  <tr class=\"sbkSaav_TextRow\"><td colspan=\"3\"><p>This page allows you to edit the basic information about a single child page and add the ability to display this child page in alternate languages.</p><p>For more information about the settings on this tab, <a href=\"" + UI_ApplicationCache_Gateway.Settings.Help_URL(RequestSpecificValues.Current_Mode.Base_URL) + "adminhelp/singleaggr\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</p></td></tr>");


			// Add the visibility line
			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
			Output.WriteLine("    <td style=\"width:50px\">&nbsp;</td>");
			Output.WriteLine("    <td class=\"sbkSaav_TableLabel\" style=\"width:145px\"><label for=\"admin_aggr_visibility\">Visibility:</label></td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td>");
			Output.Write("          <select class=\"sbkSaav_SelectSingle\" name=\"admin_aggr_visibility\" id=\"admin_aggr_visibility\" onchange=\"admin_aggr_child_page_visibility_change();\">");

			if (childPage.Browse_Type == Item_Aggregation_Child_Visibility_Enum.Main_Menu)
				Output.Write("<option value=\"browse\" selected=\"selected\">Main Menu</option>");
			else
				Output.Write("<option value=\"browse\">Main Menu</option>");

			if (childPage.Browse_Type == Item_Aggregation_Child_Visibility_Enum.Metadata_Browse_By)
				Output.Write("<option value=\"browseby\" selected=\"selected\">Browse By</option>");
			else
				Output.Write("<option value=\"browseby\">Browse By</option>");

            if (childPage.Browse_Type == Item_Aggregation_Child_Visibility_Enum.None)
				Output.Write("<option value=\"none\" selected=\"selected\">None</option>");
			else
				Output.Write("<option value=\"none\">None</option>");

			Output.WriteLine("</select>");
			Output.WriteLine("        </td>");
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + VISIBILITY_HELP + "');\"  title=\"" + VISIBILITY_HELP + "\" /></td></tr></table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");



			// Put OTHER children in alphabetical order
            SortedList<string, Complete_Item_Aggregation_Child_Page> sortedChildren = new SortedList<string, Complete_Item_Aggregation_Child_Page>();
		    if (itemAggregation.Child_Pages != null)
		    {
		        foreach (Complete_Item_Aggregation_Child_Page childPage2 in itemAggregation.Child_Pages)
		        {
		            if (childPage2.Source_Data_Type == Item_Aggregation_Child_Source_Data_Enum.Static_HTML)
		            {
		                sortedChildren.Add(childPage2.Code, childPage2);
		            }
		        }
		    }

            // Get all the children of this code
		    List<string> childCodes = new List<string>();
		    foreach (Complete_Item_Aggregation_Child_Page childPage2 in sortedChildren.Values)
		    {
		        if (!String.IsNullOrEmpty(childPage2.Parent_Code))
		        {
		            if (String.Compare(childPage2.Parent_Code, childPage.Code, StringComparison.OrdinalIgnoreCase) == 0)
		            {
		                childCodes.Add(childPage2.Code.ToLower());
		            }
		        }
		    }
            foreach (Complete_Item_Aggregation_Child_Page childPage2 in sortedChildren.Values)
            {
                if (!String.IsNullOrEmpty(childPage2.Parent_Code))
                {
                    if (childCodes.Contains(childPage2.Parent_Code.ToLower()))
                        childCodes.Add(childPage2.Code);
                }
            }

		    // Add line for parent code
			if (childPage.Browse_Type == Item_Aggregation_Child_Visibility_Enum.Main_Menu)
			{
				Output.WriteLine("  <tr id=\"admin_aggr_parent_row\" class=\"sbkSaav_SingleRow\" style=\"display:table-row;\">");
			}
			else
			{
				Output.WriteLine("  <tr id=\"admin_aggr_parent_row\" class=\"sbkSaav_SingleRow\" style=\"display:none;\">");
			}
			Output.WriteLine("    <td>&nbsp;</td>");
			Output.WriteLine("    <td class=\"sbkSaav_TableLabel\"><label for=\"admin_aggr_parent\">Parent:</label></td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td>");
			Output.Write("          <select class=\"sbkSaav_SelectSingle\" name=\"admin_aggr_parent\" id=\"admin_aggr_parent\">");
			Output.Write("<option value=\"\">(none - top level)</option>");
			foreach (Complete_Item_Aggregation_Child_Page childPage2 in sortedChildren.Values)
			{
				// Don't show itself in the possible parent list
				if (String.Compare(childPage.Code, childPage2.Code, StringComparison.OrdinalIgnoreCase) == 0)
					continue;

                // Don't show any child ones
                if (childCodes.Contains(childPage2.Code.ToLower()))
                    continue;

				// Only show main menu stuff
				if (childPage2.Browse_Type != Item_Aggregation_Child_Visibility_Enum.Main_Menu)
					continue;

				if (String.Compare(childPage.Parent_Code, childPage2.Code, StringComparison.OrdinalIgnoreCase) == 0)
					Output.Write("<option value=\"" + childPage2.Code + "\" selected=\"selected\">" + childPage2.Code + "</option>");
				else
					Output.Write("<option value=\"" + childPage2.Code + "\">" + childPage2.Code + "</option>");

			}
			Output.WriteLine("</select>");
			Output.WriteLine("        </td>");
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + PARENT_HELP + "');\"  title=\"" + PARENT_HELP + "\" /></td></tr></table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");

			// Add all the existing child page version information
			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
			Output.WriteLine("    <td style=\"width: 50px\">&nbsp;</td>");
			Output.WriteLine("    <td class=\"sbkSaav_TableLabel2\" colspan=\"2\">Existing Versions:</td>");
			Output.WriteLine("  </tr>");


			Output.WriteLine("  <tr class=\"sbkSaav_TallRow\">");
			Output.WriteLine("    <td>&nbsp;</td>");
			Output.WriteLine("    <td colspan=\"2\">");
			Output.WriteLine("      <table class=\"sbkSaav_SingleChildTable sbkSaav_Table\">");
			Output.WriteLine("        <tr>");
			Output.WriteLine("          <th class=\"sbkSaav_SingleChildHeader1\">LANGUAGE</th>");
			Output.WriteLine("          <th class=\"sbkSaav_SingleChildHeader2\">TITLE</th>");
			Output.WriteLine("          <th class=\"sbkSaav_SingleChildHeader3\">SOURCE FILE</th>");
			Output.WriteLine("          <th class=\"sbkSaav_SingleChildHeader4\">ACTIONS</th>");
			Output.WriteLine("        </tr>");

			// Get the list of all recently added child page version languages
			List<Web_Language_Enum> newLanguages = HttpContext.Current.Session["Item_Aggr_Edit_" + itemAggregation.Code + "_" + childPage + "_NewLanguages"] as List<Web_Language_Enum> ?? new List<Web_Language_Enum>();

			// Add all the version information for this child page 
			Web_Language_Enum currLanguage = RequestSpecificValues.Current_Mode.Language;
			RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Aggregation;
			RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Browse_Info;
			if (childPage.Browse_Type == Item_Aggregation_Child_Visibility_Enum.Metadata_Browse_By)
				RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Browse_By;
			RequestSpecificValues.Current_Mode.Info_Browse_Mode = childPage.Code;

			List<string> existing_languages = new List<string>();
		    if (childPage.Source_Dictionary != null)
		    {
		        foreach (KeyValuePair<Web_Language_Enum, string> thisHomeSource in childPage.Source_Dictionary)
		        {
		            RequestSpecificValues.Current_Mode.Language = thisHomeSource.Key;

		            Output.WriteLine("        <tr>");
		            bool canDelete = true;
		            if ((thisHomeSource.Key == Web_Language_Enum.DEFAULT) || (thisHomeSource.Key == Web_Language_Enum.UNDEFINED) || (thisHomeSource.Key == UI_ApplicationCache_Gateway.Settings.Default_UI_Language))
		            {
		                canDelete = false;
		                existing_languages.Add(Web_Language_Enum_Converter.Enum_To_Name(UI_ApplicationCache_Gateway.Settings.Default_UI_Language));
		                Output.WriteLine("          <td style=\"font-style:italic; padding-left:5px;\">default</td>");
		            }
		            else
		            {
		                existing_languages.Add(Web_Language_Enum_Converter.Enum_To_Name(thisHomeSource.Key));
		                Output.WriteLine("          <td style=\"padding-left:5px;\">" + Web_Language_Enum_Converter.Enum_To_Name(thisHomeSource.Key) + "</td>");
		            }

		            string label = childPage.Get_Label(thisHomeSource.Key);
		            Output.WriteLine("          <td>" + label + "</td>");

		            string file = RequestSpecificValues.Current_Mode.Base_Design_URL + "aggregations/" + itemAggregation.Code + "/" + thisHomeSource.Value.Replace("\\", "/");
		            string[] file_splitter = file.Split("\\/".ToCharArray());
		            string filename = file_splitter[file_splitter.Length - 1];
		            Output.WriteLine("          <td><a href=\"" + file + "\" title=\"View source file\">" + filename + "</a></td>");

		            Output.Write("          <td class=\"sbkAdm_ActionLink\" >( ");

		            if (!newLanguages.Contains(thisHomeSource.Key))
		            {
		                RequestSpecificValues.Current_Mode.Language = thisHomeSource.Key;

		                if (canDelete)
		                {
		                    Output.Write("<a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\" title=\"View this child page in " + Web_Language_Enum_Converter.Enum_To_Name(thisHomeSource.Key) + "\" target=\"VIEW" + itemAggregation.Code + "_" + Web_Language_Enum_Converter.Enum_To_Code(thisHomeSource.Key) + "\">view</a> | ");
		                    Output.Write("<a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\" title=\"Edit this child page in " + Web_Language_Enum_Converter.Enum_To_Name(thisHomeSource.Key) + "\" target=\"EDIT" + itemAggregation.Code + "_" + Web_Language_Enum_Converter.Enum_To_Code(thisHomeSource.Key) + "\">edit</a> ");
		                }
		                else
		                {
		                    Output.Write("<a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\" title=\"View this child page\" target=\"VIEW" + itemAggregation.Code + "_" + Web_Language_Enum_Converter.Enum_To_Code(thisHomeSource.Key) + "\">view</a> | ");
		                    Output.Write("<a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\" title=\"Edit this child page\" target=\"EDIT" + itemAggregation.Code + "_" + Web_Language_Enum_Converter.Enum_To_Code(thisHomeSource.Key) + "\">edit</a> ");
		                }
		            }
		            else
		            {
		                Output.Write("<a href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "l/technical/javascriptrequired\" onclick=\"alert('You must SAVE your changes before you can view or edit newly added child page versions.');return false\">view</a> | ");
		                Output.Write("<a href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "l/technical/javascriptrequired\" onclick=\"alert('You must SAVE your changes before you can view or edit newly added child page versions.');return false\">edit</a> ");
		            }

		            if (canDelete)
		            {
		                Output.Write("| <a  href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return aggr_edit_delete_child_version('" + Web_Language_Enum_Converter.Enum_To_Name(thisHomeSource.Key) + "', '" + Web_Language_Enum_Converter.Enum_To_Code(thisHomeSource.Key) + "');\" title=\"Delete this " + Web_Language_Enum_Converter.Enum_To_Name(thisHomeSource.Key) + " version\" >delete</a> ");
		            }

		            Output.WriteLine(" )</td>");
		            Output.WriteLine("        </tr>");
		        }
		    }
		    Output.WriteLine("      </table>");
			Output.WriteLine("    </td>");
			Output.WriteLine("  </tr>");
			RequestSpecificValues.Current_Mode.Language = currLanguage;
			RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Administrative;

			// Write the add new home page information
			Output.WriteLine("  <tr class=\"sbkSaav_TallRow\">");
			Output.WriteLine("    <td>&nbsp;</td>");
			Output.WriteLine("    <td class=\"sbkSaav_TableLabel2\">New Version:</td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <div class=\"sbkSaav_NewVersionButton\"><button title=\"Save new version of this child page\" class=\"sbkAdm_RoundButton\" onclick=\"return save_new_child_page_version();\">ADD</button></div>");

			Output.WriteLine("      <table class=\"sbkSaav_NewVersionTable\">");
			Output.WriteLine("        <tr>");
			Output.WriteLine("          <td style=\"width:160px\"><label for=\"admin_aggr_new_version_lang\">Language:</label></td>");
			Output.WriteLine("          <td style=\"width:160px\">");
			Output.Write("            <select class=\"sbkSaav_SelectSingle\" id=\"admin_aggr_new_version_lang\" name=\"admin_aggr_new_version_lang\">");

			// Add each language in the combo box
			foreach (string possible_language in Web_Language_Enum_Converter.Language_Name_Array)
			{
				if (!existing_languages.Contains(possible_language))
					Output.Write("<option value=\"" + Web_Language_Enum_Converter.Name_To_Code(possible_language) + "\">" + HttpUtility.HtmlEncode(possible_language) + "</option>");
			}
			Output.WriteLine();
			Output.WriteLine("          </td>");
			Output.WriteLine("          <td style=\"width:145px\"><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + NEW_VERSION_LANGUAGE_HELP + "');\"  title=\"" + NEW_VERSION_LANGUAGE_HELP + "\" /></td>");
			Output.WriteLine("          <td></td>");
			Output.WriteLine("        </tr>");

			Output.WriteLine("        <tr>");
			Output.WriteLine("          <td><label for=\"admin_aggr_new_version_label\">Title:</label></td>");
			Output.WriteLine("          <td colspan=\"2\"><input class=\"sbkSaav_medium_input sbkAdmin_Focusable\" name=\"admin_aggr_new_version_label\" id=\"admin_aggr_new_version_label\" type=\"text\" value=\"\" /></td>");
			Output.WriteLine("          <td><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + NEW_VERSION_TITLE_HELP + "');\"  title=\"" + NEW_VERSION_TITLE_HELP + "\" /></td>");
			Output.WriteLine("        </tr>");

			Output.WriteLine("        <tr>");

			Output.WriteLine("          <td><label for=\"admin_aggr_new_version_copy\">Copy from existing:</label></td>");
			Output.WriteLine("          <td>");
			Output.Write("            <select class=\"sbkSaav_SelectSingle\" id=\"admin_aggr_new_version_copy\" name=\"admin_aggr_new_version_copy\">");
			Output.Write("<option value=\"\" selected=\"selected\"></option>");
		    if (childPage.Source_Dictionary != null)
		    {
		        foreach (KeyValuePair<Web_Language_Enum, string> thisHomeSource in childPage.Source_Dictionary)
		        {
		            if ((thisHomeSource.Key == Web_Language_Enum.DEFAULT) || (thisHomeSource.Key == UI_ApplicationCache_Gateway.Settings.Default_UI_Language))
		            {
		                Output.Write("<option value=\"" + thisHomeSource.Value + "\">" + HttpUtility.HtmlEncode(Web_Language_Enum_Converter.Enum_To_Name(UI_ApplicationCache_Gateway.Settings.Default_UI_Language)) + "</option>");
		            }
		            else
		            {
		                Output.Write("<option value=\"" + thisHomeSource.Value + "\">" + HttpUtility.HtmlEncode(Web_Language_Enum_Converter.Enum_To_Name(thisHomeSource.Key)) + "</option>");
		            }
		        }
		    }

		    Output.WriteLine("</select>");
			Output.WriteLine("          </td>");
			Output.WriteLine("          <td colspan=\"2\"><img class=\"sbkSaav_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + NEW_VERSION_COPY_HELP + "');\"  title=\"" + NEW_VERSION_COPY_HELP + "\" /></td>");
			Output.WriteLine("        </tr>");
			Output.WriteLine("      </table>");
			Output.WriteLine("    </td>");
			Output.WriteLine("  </tr>");

			Output.WriteLine("</table>");
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
				case 1:
					add_upload_controls(MainPlaceHolder, ".gif", aggregationDirectory + "\\images\\buttons", "coll.gif", false, itemAggregation.Code + "|Button", Tracer);
					break;

				case 5:
					add_upload_controls(MainPlaceHolder, ".gif,.bmp,.jpg,.png,.jpeg", aggregationDirectory + "\\images\\banners", String.Empty, false, itemAggregation.Code + "|Banners", Tracer);
					break;

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
