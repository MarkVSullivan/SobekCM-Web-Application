#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SobekCM.Core.Aggregations;
using SobekCM.Core.Configuration;
using SobekCM.Core.Navigation;
using SobekCM.Core.Search;
using SobekCM.Core.WebContent;
using SobekCM.Engine_Library.Aggregations;
using SobekCM.Engine_Library.Database;
using SobekCM.Engine_Library.Navigation;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Engine.MemoryMgmt;
using SobekCM.Library.UploadiFive;
using SobekCM.Tools;
using SobekCM.UI_Library;
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
	/// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
	/// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="Navigation.SobekCM_Navigation_Object"/> </li>
	/// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
	/// <li>The HTML writer will create the necessary subwriter.  Since this action requires authentication, an instance of the  <see cref="MySobek_HtmlSubwriter"/> class is created. </li>
	/// <li>The mySobek subwriter creates an instance of this viewer to view and edit information related to a single item aggregation</li>
	/// </ul></remarks>
	public class Aggregation_Single_AdminViewer : abstract_AdminViewer
	{
		private string actionMessage;
		private readonly string aggregationDirectory;
        private Item_Aggregation itemAggregation;

		private readonly int page;

		private string enteredCode;
		private string enteredDescription;
		private bool enteredIsActive;
		private bool enteredIsHidden;
		private string enteredName;
		private string enteredShortname;
		private string enteredType;

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
			Item_Aggregation cachedInstance = null;
			if (possibleEditAggregation != null)
				cachedInstance = (Item_Aggregation)possibleEditAggregation;

            itemAggregation = Item_Aggregation_Utilities.Get_Item_Aggregation(code, String.Empty, cachedInstance, false, false, RequestSpecificValues.Tracer);
			
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
			else if (RequestSpecificValues.Current_Mode.My_Sobek_SubMode == "y")
				page = 9;
			else if (RequestSpecificValues.Current_Mode.My_Sobek_SubMode.IndexOf("g_") == 0)
				page = 10;

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
							Save_Page_5_Postback(form);
							break;

						case 6:
							Save_Page_6_Postback(form);
							break;

						case 7:
							Save_Page_7_Postback(form);
							break;

						case 8:
							Save_Page_8_Postback(form);
							break;

						case 9:
							Save_Page_CSS_Postback(form);
							break;

						case 10:
							Save_Child_Page_Postback(form);
							break;
					}

					// Should this be saved to the database?
					if (action == "save")
					{
						// Save the new configuration file
						bool successful_save = (itemAggregation.Write_Configuration_File(UI_ApplicationCache_Gateway.Settings.Base_Design_Location + itemAggregation.ObjDirectory));

						// Save to the database
                        if (!Item_Aggregation_Utilities.Save_To_Database(itemAggregation, RequestSpecificValues.Current_User.Full_Name, null))
							successful_save = false;

						// Save the link between this item and the thematic heading
						UI_ApplicationCache_Gateway.Aggregations.Set_Aggregation_Thematic_Heading(itemAggregation.Code, itemAggregation.Thematic_Heading_ID);


						// Clear the aggregation from the cache
						Cached_Data_Manager.Remove_Item_Aggregation(itemAggregation.Code, null);

						// Forward back to the aggregation home page, if this was successful
						if (successful_save)
						{
							// Clear the aggregation from the sessions
							HttpContext.Current.Session["Edit_Aggregation_" + itemAggregation.Code] = null;
							HttpContext.Current.Session["Item_Aggr_Edit_" + itemAggregation.Code + "_NewLanguages"] = null;

							// Redirect the RequestSpecificValues.Current_User
							RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Aggregation;
							RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Home;
							UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
						}
						else
						{
							actionMessage = "Error saving aggregation information!";
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

			Output.WriteLine("<script src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/scripts/sobekcm_admin.js\" type=\"text/javascript\"></script>");
			Output.WriteLine();

			Output.WriteLine("<div id=\"sbkSaav_PageContainer\">");

			// Add the buttons (unless this is a sub-page like editing the CSS file)
			if (page < 9)
			{
				string last_mode = RequestSpecificValues.Current_Mode.My_Sobek_SubMode;
				RequestSpecificValues.Current_Mode.My_Sobek_SubMode = String.Empty;
				Output.WriteLine("  <div class=\"sbkSaav_ButtonsDiv\">");
				Output.WriteLine("    <button title=\"Do not apply changes\" class=\"sbkAdm_RoundButton\" onclick=\"return new_aggr_edit_page('z');\"><img src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/button_previous_arrow.png\" class=\"sbkAdm_RoundButton_LeftImg\" alt=\"\" /> CANCEL</button> &nbsp; &nbsp; ");
				Output.WriteLine("    <button title=\"Save changes to this item Aggregation\" class=\"sbkAdm_RoundButton\" onclick=\"return save_aggr_edits();\">SAVE <img src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/button_next_arrow.png\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button>");
				Output.WriteLine("  </div>");
				Output.WriteLine();
				RequestSpecificValues.Current_Mode.My_Sobek_SubMode = last_mode;
			}
			else if (page == 10)
			{
				Output.WriteLine("  <div class=\"sbkSaav_ButtonsDiv\">");
				Output.WriteLine("    <button title=\"Close this child page details and return to main admin pages\" class=\"sbkAdm_RoundButton\" onclick=\"return new_aggr_edit_page('g');\"><img src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/button_previous_arrow.png\" class=\"sbkAdm_RoundButton_LeftImg\" alt=\"\" /> BACK </button>"); 
				Output.WriteLine("  </div>");
			}


			Output.WriteLine("  <div class=\"sbkSaav_HomeText\">");
			Output.WriteLine("    <br />");
			Output.WriteLine("    <h1>" + itemAggregation.Aggregation_Type + " Administration : " + itemAggregation.Code.ToUpper() + "</h1>");
			Output.WriteLine("  </div>");
			Output.WriteLine();

			// Start the outer tab containe
			Output.WriteLine("  <div id=\"tabContainer\" class=\"fulltabs\">");

			// Add all the possible tabs (unless this is a sub-page like editing the CSS file)
			if (page < 9)
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

				// Draw all the page tabs for this form
				if (page == 1)
				{
					Output.WriteLine("    <li id=\"tabHeader_1\" class=\"tabActiveHeader\">" + GENERAL + "</li>");
				}
				else
				{
					Output.WriteLine("    <li id=\"tabHeader_1\" onclick=\"return new_aggr_edit_page('a');\">" + GENERAL + "</li>");
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

				if (page == 5)
				{
					Output.WriteLine("    <li id=\"tabHeader_4\" class=\"tabActiveHeader\">" + APPEARANCE + "</li>");
				}
				else
				{
					Output.WriteLine("    <li id=\"tabHeader_4\" onclick=\"return new_aggr_edit_page('e');\">" + APPEARANCE + "</li>");
				}

				if (page == 6)
				{
					Output.WriteLine("    <li id=\"tabHeader_5\" class=\"tabActiveHeader\">" + HIGHLIGHTS + "</li>");
				}
				else
				{
					Output.WriteLine("    <li id=\"tabHeader_5\" onclick=\"return new_aggr_edit_page('f');\">" + HIGHLIGHTS + "</li>");
				}

				if (page == 7)
				{
					Output.WriteLine("    <li id=\"tabHeader_6\" class=\"tabActiveHeader\">" + STATIC_PAGES + "</li>");
				}
				else
				{
					Output.WriteLine("    <li id=\"tabHeader_6\" onclick=\"return new_aggr_edit_page('g');\">" + STATIC_PAGES + "</li>");
				}

				if (page == 8)
				{
					Output.WriteLine("    <li id=\"tabHeader_6\" class=\"tabActiveHeader\">" + SUBCOLLECTIONS + "</li>");
				}
				else
				{
					Output.WriteLine("    <li id=\"tabHeader_6\" onclick=\"return new_aggr_edit_page('h');\">" + SUBCOLLECTIONS + "</li>");
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
					Add_Page_5(Output);
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
					Add_Page_CSS(Output);
					break;

				case 10:
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
			}


			 Output.WriteLine("    </div>");
			 Output.WriteLine("  </div>");
			 Output.WriteLine("</div>");
			 Output.WriteLine("<br />");
		}

		#region Methods to render (and parse) page 1 - Basic Information

		private void Save_Page_1_Postback(NameValueCollection Form)
		{
			// Was a button uploaded?
			if (HttpContext.Current.Items["Uploaded File"] != null)
			{
				string newButton = HttpContext.Current.Items["Uploaded File"].ToString();
				if (File.Exists(newButton))
				{
					string coll_gif = aggregationDirectory + "\\images\\buttons\\coll.gif";
					string coll_gif_old = aggregationDirectory + "\\images\\buttons\\coll_old.gif";
					if (File.Exists(coll_gif_old))
						File.Delete(coll_gif_old);
					if (File.Exists(coll_gif))
						File.Move(coll_gif, coll_gif_old);

					File.Move(newButton, coll_gif);

					// Also save this change
					SobekCM_Database.Save_Item_Aggregation_Milestone(itemAggregation.Code, "Button changed", RequestSpecificValues.Current_User.Full_Name);

				}
			}
			


			if (Form["admin_aggr_name"] != null) itemAggregation.Name = Form["admin_aggr_name"];
			if (Form["admin_aggr_shortname"] != null) itemAggregation.ShortName = Form["admin_aggr_shortname"];
			if (Form["admin_aggr_link"] != null) itemAggregation.External_Link = Form["admin_aggr_link"];
			if ( Form["admin_aggr_desc"] != null ) itemAggregation.Description = Form["admin_aggr_desc"];
			if (Form["admin_aggr_email"] != null) itemAggregation.Contact_Email = Form["admin_aggr_email"];
			itemAggregation.Is_Active = Form["admin_aggr_isactive"] != null;
			itemAggregation.Hidden = Form["admin_aggr_ishidden"] == null;
			if ((RequestSpecificValues.Current_User.Is_System_Admin) || (RequestSpecificValues.Current_User.Is_Portal_Admin))
			{
				if (Form["admin_aggr_heading"] != null)
					itemAggregation.Thematic_Heading_ID = Convert.ToInt32(Form["admin_aggr_heading"]);
			}

		}

		private void Add_Page_1( TextWriter Output )
		{
			// Help constants (for now)
			const string LONG_NAME_HELP = "Long name help place holder";
			const string SHORT_NAME_HELP = "Short name help place holder";
			const string LINK_HELP = "Link help place holder";
			const string DESCRIPTION_HELP = "Description help place holder";
			const string EMAIL_HELP = "Email help place holder";
			const string ACTIVE_HELP = "Active checkbox help place holder";
			const string HIDDEN_HELP = "Hidden checkbox help place holder";
			const string COLLECTION_BUTTON_HELP = "Collection button help place holder";
			

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
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + LONG_NAME_HELP + "');\"  title=\"" + LONG_NAME_HELP + "\" /></td></tr></table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");

			// Add the short name line
			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
			Output.WriteLine("    <td>&nbsp;</td>");
			Output.WriteLine("    <td class=\"sbkSaav_TableLabel\"><label for=\"admin_aggr_shortname\">Name (short):</label></td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td><input class=\"sbkSaav_medium_input sbkAdmin_Focusable\" name=\"admin_aggr_shortname\" id=\"admin_aggr_shortname\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(itemAggregation.ShortName) + "\" /></td>");
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + SHORT_NAME_HELP + "');\"  title=\"" + SHORT_NAME_HELP + "\" /></td></tr></table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");

			// Add the description box
			Output.WriteLine("  <tr class=\"sbkSaav_TallRow\">");
			Output.WriteLine("    <td>&nbsp;</td>");
			Output.WriteLine("    <td class=\"sbkSaav_TableLabel2\"><label for=\"admin_aggr_desc\">Description:</label></td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <table class=\"sbkSaav_InnerTable2\"><tr style=\"vertical-align:top\"><td><textarea class=\"sbkSaav_large_textbox sbkAdmin_Focusable\" rows=\"6\" name=\"admin_aggr_desc\" id=\"admin_aggr_desc\">" + HttpUtility.HtmlEncode(itemAggregation.Description) + "</textarea></td>");
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + DESCRIPTION_HELP + "');\"  title=\"" + DESCRIPTION_HELP + "\" /></td></tr></table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");

			// Add the email line
			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
			Output.WriteLine("    <td>&nbsp;</td>");
			Output.WriteLine("    <td class=\"sbkSaav_TableLabel\"><label for=\"admin_aggr_email\">Contact Email:</label></td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td><input class=\"sbkSaav_large_input sbkAdmin_Focusable\" name=\"admin_aggr_email\" id=\"admin_aggr_email\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(itemAggregation.Contact_Email) + "\" /></td>");
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + EMAIL_HELP + "');\"  title=\"" + EMAIL_HELP + "\" /></td></tr></table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");

			// Add the link line
			if (itemAggregation.Aggregation_Type.IndexOf("Institution", StringComparison.OrdinalIgnoreCase) >= 0)
			{
				Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
				Output.WriteLine("    <td>&nbsp;</td>");
				Output.WriteLine("    <td class=\"sbkSaav_TableLabel\"><label for=\"admin_aggr_link\">External Link:</label></td>");
				Output.WriteLine("    <td>");
				Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td><input class=\"sbkSaav_large_input sbkAdmin_Focusable\" name=\"admin_aggr_link\" id=\"admin_aggr_link\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(itemAggregation.External_Link) + "\" /></td>");
				Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + LINK_HELP + "');\"  title=\"" + LINK_HELP + "\" /></td></tr></table>");
				Output.WriteLine("     </td>");
				Output.WriteLine("  </tr>");
			}

			Output.WriteLine("  <tr class=\"sbkSaav_TitleRow2\"><td colspan=\"3\">Collection Visibility</td></tr>");
			Output.WriteLine("  <tr class=\"sbkSaav_TextRow\"><td colspan=\"3\"><p>The values in this section determine if the collection is currently visible at all, whether it is eligible to appear on the collection list at the bottom of the parent page, and the collection button used in that case.  Thematic headings are used to place this collection on the main home page.</p><p>For more information about the settings on this tab, <a href=\"" + UI_ApplicationCache_Gateway.Settings.Help_URL(RequestSpecificValues.Current_Mode.Base_URL) + "adminhelp/singleaggr\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</p></td></tr>");


			// Add the behavior lines
			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
			Output.WriteLine("    <td>&nbsp;</td>");
			Output.WriteLine("    <td class=\"sbkSaav_TableLabel\">Behavior:</label></td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td>");
			Output.WriteLine(itemAggregation.Is_Active
			   ? "          <input class=\"sbkSaav_checkbox\" type=\"checkbox\" name=\"admin_aggr_isactive\" id=\"admin_aggr_isactive\" checked=\"checked\" /> <label for=\"admin_aggr_isactive\">Active?</label> "
			   : "          <input class=\"sbkSaav_checkbox\" type=\"checkbox\" name=\"admin_aggr_isactive\" id=\"admin_aggr_isactive\" /> <label for=\"admin_aggr_isactive\">Active?</label> ");
			Output.WriteLine("        </td>");
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + ACTIVE_HELP + "');\"  title=\"" + ACTIVE_HELP + "\" /></td></tr></table>");
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
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + HIDDEN_HELP + "');\"  title=\"" + HIDDEN_HELP + "\" /></td></tr></table>");
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
			Output.WriteLine("           <td><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + COLLECTION_BUTTON_HELP + "');\"  title=\"" + COLLECTION_BUTTON_HELP + "\" /></td>");
			Output.WriteLine("         </tr>");
			Output.WriteLine("         <tr>");
			Output.WriteLine("           <td colspan=\"2\">");




		}

		private void Finish_Page_1(TextWriter Output)
		{
			const string THEMATIC_HELP = "Thematic heading help place holder";

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
				Output.WriteLine(itemAggregation.Thematic_Heading_ID < 0 ? "            <option value=\"-1\" selected=\"selected\" ></option>" : "            <option value=\"-1\"></option>");
				foreach (Thematic_Heading thisHeading in UI_ApplicationCache_Gateway.Thematic_Headings)
				{
					if (itemAggregation.Thematic_Heading_ID == thisHeading.ThematicHeadingID)
					{
						Output.WriteLine("            <option value=\"" + thisHeading.ThematicHeadingID + "\" selected=\"selected\" >" + HttpUtility.HtmlEncode(thisHeading.ThemeName) + "</option>");
					}
					else
					{
						Output.WriteLine("            <option value=\"" + thisHeading.ThematicHeadingID + "\">" + HttpUtility.HtmlEncode(thisHeading.ThemeName) + "</option>");
					}
				}
				Output.WriteLine("          </select>");
				Output.WriteLine("        </td>");
				Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + THEMATIC_HELP + "');\"  title=\"" + THEMATIC_HELP + "\" /></td></tr></table>");
				Output.WriteLine("     </td>");
				Output.WriteLine("  </tr>");
			}

			Output.WriteLine("</table>");
			Output.WriteLine("<br />");
		}

		#endregion

		#region Methods to render (and parse) page 2 - Search

		private void Save_Page_2_Postback(NameValueCollection Form)
		{
			if (Form["admin_aggr_mapsearch_type"] != null)
				itemAggregation.Map_Search = Convert.ToUInt16(Form["admin_aggr_mapsearch_type"]);

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
            if (Form["admin_aggr_mapsearchbeta"] != null) displayOptionsBldr.Append("Q");
			if (Form["admin_aggr_mapbrowse"] != null) displayOptionsBldr.Append("G");
			if (Form["admin_aggr_allitems"] != null) displayOptionsBldr.Append("I");
            

			itemAggregation.Display_Options = displayOptionsBldr.ToString();
		}

		private void Add_Page_2(TextWriter Output)
		{
			// Help constants (for now)
			const string BASIC_HELP = "Basic search help place holder";
			const string ADVANCED_HELP = "Advanced search help place holder";
			const string BASIC_YEARS_HELP = "Basic search with year range help place holder";
			const string ADVANCED_YEARS_HELP = "Advanced search with year range help place holder";
			const string FULLTEXT_HELP = "Full text saerch help place holder";
			const string MAP_SEARCH_HELP = "Map search help place holder";
            const string MAP_SEARCH_BETA_HELP = "This enables the map searcher [beta]";
			const string DLOC_SEARCH_HELP = "dLOC-Specific search help place holder";
			const string NEWSPAPER_SEARCH_HELP = "Newspaper search help place holder";
			const string ALL_ITEMS_HELP = "All and New Item browses help place holder";
			const string MAP_BROWSE_HELP = "Map browse help place holder";
			const string MAP_SEARCH_BOUNDING_HELP = "Map search starting bounding box help place holder";
            const string BASIC_MIMETYPE_SEARCH_HELP = "Basic search which enables you to filter out or include items that have digital resources.";
		    const string ADVANCED_MIMETYPE_HELP = "Advanced search which enables you to filter out or include items that have digital resources.";

			Output.WriteLine("<table class=\"sbkAdm_PopupTable\">");

			Output.WriteLine("  <tr class=\"sbkSaav_TitleRow\"><td colspan=\"3\">Search Options</td></tr>");
			Output.WriteLine("  <tr class=\"sbkSaav_TextRow\"><td colspan=\"3\"><p>These options control how searching works within this aggregation, such as which search options are made publicly available.</p><p>For more information about the settings on this tab, <a href=\"" + UI_ApplicationCache_Gateway.Settings.Help_URL(RequestSpecificValues.Current_Mode.Base_URL) + "adminhelp/singleaggr\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</p></td></tr>");

			// Add line for basic search type
			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
			Output.WriteLine("    <td style=\"width:50px;\">&nbsp;</td>");
			Output.WriteLine("    <td  style=\"width:175px\" class=\"sbkSaav_TableLabel\">Search Types:</label></td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td>");
			Output.Write("          <input class=\"sbkSaav_checkbox\" type=\"checkbox\" name=\"admin_aggr_basicsearch\" id=\"admin_aggr_basicsearch\"");
			if (( itemAggregation.Display_Options.IndexOf("B") >= 0 ) || (itemAggregation.Display_Options.IndexOf("D") >= 0 ))
				Output.Write(" checked=\"checked\"");
			Output.WriteLine(" /> <label for=\"admin_aggr_basicsearch\">Basic Search</label>");
			Output.WriteLine("        </td>");
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + BASIC_HELP + "');\"  title=\"" + BASIC_HELP + "\" /></td></tr></table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");

			// Add line for basic search with year range
			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
			Output.WriteLine("    <td colspan=\"2\">&nbsp;</td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td>");
			Output.Write("          <input class=\"sbkSaav_checkbox\" type=\"checkbox\" name=\"admin_aggr_basicsearch_years\" id=\"admin_aggr_basicsearch_years\"");
			if (itemAggregation.Display_Options.IndexOf("Y") >= 0)
				Output.Write(" checked=\"checked\"");
			Output.WriteLine(" /> <label for=\"admin_aggr_basicsearch_years\">Basic Search (with Year Range)</label>");
			Output.WriteLine("        </td>");
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + BASIC_YEARS_HELP + "');\"  title=\"" + BASIC_YEARS_HELP + "\" /></td></tr></table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");

            // Add line for basic search ( with mime-type exclusion )
            Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
            Output.WriteLine("    <td colspan=\"2\">&nbsp;</td>");
            Output.WriteLine("    <td>");
            Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td>");
            Output.Write("          <input class=\"sbkSaav_checkbox\" type=\"checkbox\" name=\"admin_aggr_basicsearch_mimetype\" id=\"admin_aggr_basicsearch_mimetype\"");
            if (itemAggregation.Display_Options.IndexOf("W") >= 0)
                Output.Write(" checked=\"checked\"");
            Output.WriteLine(" /> <label for=\"admin_aggr_basicsearch_mimetype\">Basic search (with mime-type filter)</label>");
            Output.WriteLine("        </td>");
            Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + BASIC_MIMETYPE_SEARCH_HELP + "');\"  title=\"" + BASIC_MIMETYPE_SEARCH_HELP + "\" /></td></tr></table>");
            Output.WriteLine("     </td>");
            Output.WriteLine("  </tr>");

			// Add line for advanced search type
			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
			Output.WriteLine("    <td colspan=\"2\">&nbsp;</td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td>");
			Output.Write("          <input class=\"sbkSaav_checkbox\" type=\"checkbox\" name=\"admin_aggr_advsearch\" id=\"admin_aggr_advsearch\"");
			if (itemAggregation.Display_Options.IndexOf("A") >= 0)
				Output.Write(" checked=\"checked\"");
			Output.WriteLine(" /> <label for=\"admin_aggr_advsearch\">Advanced Search</label>");
			Output.WriteLine("        </td>");
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + ADVANCED_HELP + "');\"  title=\"" + ADVANCED_HELP + "\" /></td></tr></table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");

			// Add line for advanced search with year range
			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
			Output.WriteLine("    <td colspan=\"2\">&nbsp;</td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td>");
			Output.Write("          <input class=\"sbkSaav_checkbox\" type=\"checkbox\" name=\"admin_aggr_advsearch_years\" id=\"admin_aggr_advsearch_years\"");
			if (itemAggregation.Display_Options.IndexOf("Z") >= 0)
				Output.Write(" checked=\"checked\"");
			Output.WriteLine(" /> <label for=\"admin_aggr_advsearch_years\">Advanced Search (with Year Range)</label>");
			Output.WriteLine("        </td>");
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + ADVANCED_YEARS_HELP + "');\"  title=\"" + ADVANCED_YEARS_HELP + "\" /></td></tr></table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");

            // Add line for advanced search with MIME-TYPE exclusion type
            Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
            Output.WriteLine("    <td colspan=\"2\">&nbsp;</td>");
            Output.WriteLine("    <td>");
            Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td>");
            Output.Write("          <input class=\"sbkSaav_checkbox\" type=\"checkbox\" name=\"admin_aggr_advsearch_mimetype\" id=\"admin_aggr_advsearch_mimetype\"");
            if (itemAggregation.Display_Options.IndexOf("X") >= 0)
                Output.Write(" checked=\"checked\"");
            Output.WriteLine(" /> <label for=\"admin_aggr_advsearch\">Advanced Search (with mime-type filter)</label>");
            Output.WriteLine("        </td>");
            Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + ADVANCED_HELP + "');\"  title=\"" + ADVANCED_MIMETYPE_HELP + "\" /></td></tr></table>");
            Output.WriteLine("     </td>");
            Output.WriteLine("  </tr>");

			// Add line for full text search
			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
			Output.WriteLine("    <td colspan=\"2\">&nbsp;</td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td>");
			Output.Write("          <input class=\"sbkSaav_checkbox\" type=\"checkbox\" name=\"admin_aggr_textsearch\" id=\"admin_aggr_textsearch\"");
			if (itemAggregation.Display_Options.IndexOf("F") >= 0)
				Output.Write(" checked=\"checked\"");
			Output.WriteLine(" /> <label for=\"admin_aggr_textsearch\">Full Text Search</label>");
			Output.WriteLine("        </td>");
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + FULLTEXT_HELP + "');\"  title=\"" + FULLTEXT_HELP + "\" /></td></tr></table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");

			// Add line for newspaper search
			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
			Output.WriteLine("    <td colspan=\"2\">&nbsp;</td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td>");
			Output.Write("          <input class=\"sbkSaav_checkbox\" type=\"checkbox\" name=\"admin_aggr_newspsearch\" id=\"admin_aggr_newspsearch\"");
			if (itemAggregation.Display_Options.IndexOf("N") >= 0)
				Output.Write(" checked=\"checked\"");
			Output.WriteLine(" /> <label for=\"admin_aggr_newspsearch\">Newspaper Search</label>");
			Output.WriteLine("        </td>");
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + NEWSPAPER_SEARCH_HELP + "');\"  title=\"" + NEWSPAPER_SEARCH_HELP + "\" /></td></tr></table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");

			// Add line for dLOC full text saerch
			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
			Output.WriteLine("    <td colspan=\"2\">&nbsp;</td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td>");
			Output.Write("          <input class=\"sbkSaav_checkbox\" type=\"checkbox\" name=\"admin_aggr_dloctextsearch\" id=\"admin_aggr_dloctextsearch\"");
			if (itemAggregation.Display_Options.IndexOf("C") >= 0)
				Output.Write(" checked=\"checked\"");
			Output.WriteLine(" /> <label for=\"admin_aggr_dloctextsearch\">dLOC Full Text Search</label>");
			Output.WriteLine("        </td>");
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + DLOC_SEARCH_HELP + "');\"  title=\"" + DLOC_SEARCH_HELP + "\" /></td></tr></table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");

			// Add line for Map saerch
			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
			Output.WriteLine("    <td colspan=\"2\">&nbsp;</td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td>");
			Output.Write("          <input class=\"sbkSaav_checkbox\" type=\"checkbox\" name=\"admin_aggr_mapsearch\" id=\"admin_aggr_mapsearch\"");
			if (itemAggregation.Display_Options.IndexOf("M") >= 0)
				Output.Write(" checked=\"checked\"");
			Output.WriteLine(" /> <label for=\"admin_aggr_mapsearch\">Map Search (Legacy)</label>");
			Output.WriteLine("        </td>");
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + MAP_SEARCH_HELP + "');\"  title=\"" + MAP_SEARCH_HELP + "\" /></td></tr></table>");
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
            //Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + MAP_SEARCH_BETA_HELP + "');\"  title=\"" + MAP_SEARCH_HELP + "\" /></td></tr></table>");
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
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + ALL_ITEMS_HELP + "');\"  title=\"" + ALL_ITEMS_HELP + "\" /></td></tr></table>");
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
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + MAP_BROWSE_HELP + "');\"  title=\"" + MAP_BROWSE_HELP + "\" /></td></tr></table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");


			// Add line for all/new item browses type
			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
			Output.WriteLine("    <td>&nbsp;</td>");
			Output.WriteLine("    <td class=\"sbkSaav_TableLabel\">Map Search Default Area:</label></td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td>");
			Output.WriteLine("          <select class=\"sbkSaav_SelectSingle\" name=\"admin_aggr_mapsearch_type\" id=\"admin_aggr_mapsearch_type\">");

			Output.WriteLine(itemAggregation.Map_Search % 100 == 0
							 ? "            <option value=\"0\" selected=\"selected\" >World</option>"
							 : "            <option value=\"0\">World</option>");

			Output.Write(itemAggregation.Map_Search % 100 == 1
							 ? "            <option value=\"1\" selected=\"selected\" >Florida</option>"
							 : "            <option value=\"1\">Florida</option>");

			Output.Write(itemAggregation.Map_Search % 100 == 2
							 ? "            <option value=\"2\" selected=\"selected\" >United States</option>"
							 : "            <option value=\"2\">United States</option>");

			Output.Write(itemAggregation.Map_Search % 100 == 3
							 ? "            <option value=\"3\" selected=\"selected\" >North America</option>"
							 : "            <option value=\"3\">North America</option>");

			Output.Write(itemAggregation.Map_Search % 100 == 4
							 ? "            <option value=\"4\" selected=\"selected\" >Caribbean</option>"
							 : "            <option value=\"4\">Caribbean</option>");

			Output.Write(itemAggregation.Map_Search % 100 == 5
							 ? "            <option value=\"5\" selected=\"selected\" >South America</option>"
							 : "            <option value=\"5\">South America</option>");

			Output.Write(itemAggregation.Map_Search % 100 == 6
							 ? "            <option value=\"6\" selected=\"selected\" >Africa</option>"
							 : "            <option value=\"6\">Africa</option>");

			Output.Write(itemAggregation.Map_Search % 100 == 7
							 ? "            <option value=\"7\" selected=\"selected\" >Europe</option>"
							 : "            <option value=\"7\">Europe</option>");

			Output.Write(itemAggregation.Map_Search % 100 == 8
							 ? "            <option value=\"8\" selected=\"selected\" >Asia</option>"
							 : "            <option value=\"8\">Asia</option>");

			Output.Write(itemAggregation.Map_Search % 100 == 9
							 ? "            <option value=\"9\" selected=\"selected\" >Middle East</option>"
							 : "            <option value=\"9\">Middle East</option>");

			Output.WriteLine("          </select>");
			Output.WriteLine("        </td>");
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + MAP_SEARCH_BOUNDING_HELP + "');\"  title=\"" + MAP_SEARCH_BOUNDING_HELP + "\" /></td></tr></table>");
			Output.WriteLine("     </td>");
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

					case "full":
						itemAggregation.Default_Result_View = Result_Display_Type_Enum.Full_Citation;
						itemAggregation.Result_Views.Add( Result_Display_Type_Enum.Full_Citation );
						break;
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

				case "full":
						if ( !itemAggregation.Result_Views.Contains( Result_Display_Type_Enum.Full_Citation ))
							itemAggregation.Result_Views.Add( Result_Display_Type_Enum.Full_Citation );
						break;
				}
		}

		private void Add_Page_3( TextWriter Output )
		{
			// Help constants (for now)
			const string FACETS_HELP = "Facets help place holder";
			const string DEFAULT_VIEW_HELP = "Default results view help place holder";
			const string RESULTS_VIEWS_HELP = "Results view help place holder";

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
			Output.WriteLine("          <td><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + FACETS_HELP + "');\"  title=\"" + FACETS_HELP + "\" /></td>");
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
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + DEFAULT_VIEW_HELP + "');\"  title=\"" + DEFAULT_VIEW_HELP + "\" /></td></tr></table>");
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
			Output.WriteLine("          <td><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + RESULTS_VIEWS_HELP + "');\"  title=\"" + RESULTS_VIEWS_HELP + "\" /></td>");
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

			Output.Write(Result_Type == Result_Display_Type_Enum.Full_Citation ? "<option value=\"full\" selected=\"selected\" >Full View</option>" : "<option value=\"full\">Full View</option>");
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
			List<Item_Aggregation_Child_Page> metadata_browse_bys = itemAggregation.Browse_By_Pages(UI_ApplicationCache_Gateway.Settings.Default_UI_Language).Where(ThisBrowse => ThisBrowse.Browse_Type == Item_Aggregation_Child_Page.Visibility_Type.METADATA_BROWSE_BY).Where(ThisBrowse => ThisBrowse.Source == Item_Aggregation_Child_Page.Source_Type.Database).ToList();

			// Remove all these browse by's
			foreach (Item_Aggregation_Child_Page browseBy in metadata_browse_bys)
			{
				itemAggregation.Remove_Child_Page(browseBy);
			}

			// Look for the default browse by
			short default_browseby_id = 0;
			itemAggregation.Default_BrowseBy = String.Empty;
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
							Item_Aggregation_Child_Page newBrowse = new Item_Aggregation_Child_Page(Item_Aggregation_Child_Page.Visibility_Type.METADATA_BROWSE_BY, Item_Aggregation_Child_Page.Source_Type.Database, field.Display_Term, String.Empty, field.Display_Term);
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
							Item_Aggregation_Child_Page newBrowse = new Item_Aggregation_Child_Page(Item_Aggregation_Child_Page.Visibility_Type.METADATA_BROWSE_BY, Item_Aggregation_Child_Page.Source_Type.Database, field.Display_Term, String.Empty, field.Display_Term);
							itemAggregation.Add_Child_Page(newBrowse);
						}
					}
				}
			}

			itemAggregation.OAI_Flag = Form["admin_aggr_oai_flag"] != null;

			if (Form["admin_aggr_oai_metadata"] != null)
				itemAggregation.OAI_Metadata = Form["admin_aggr_oai_metadata"];
		}

		private void Add_Page_4(TextWriter Output)
		{
			// Get the metadata browses
			List<string> metadata_browse_bys = new List<string>();
			string default_browse_by = itemAggregation.Default_BrowseBy;
			List<string> otherBrowseBys = new List<string>();
			foreach (Item_Aggregation_Child_Page thisBrowse in itemAggregation.Browse_By_Pages(UI_ApplicationCache_Gateway.Settings.Default_UI_Language))
			{
				if (thisBrowse.Browse_Type == Item_Aggregation_Child_Page.Visibility_Type.METADATA_BROWSE_BY)
				{
					if (thisBrowse.Source == Item_Aggregation_Child_Page.Source_Type.Database)
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
			const string DEFAULT_HELP = "Default browse by help place holder";
			const string METADATA_BROWSES_HELP = "Metadata browses help place holder";
			const string OAI_FLAG_HELP = "OAI-PMH Enabled flag help place holder";
			const string OAI_METADATA_HELP = "OAI-PMH Additional metadata help place holder";


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
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + DEFAULT_HELP + "');\"  title=\"" + DEFAULT_HELP + "\" /></td></tr></table>");
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
			Output.WriteLine("          <td><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + METADATA_BROWSES_HELP + "');\"  title=\"" + METADATA_BROWSES_HELP + "\" /></td>");
			Output.WriteLine("         </tr>");
			Output.WriteLine("       </table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");


			Output.WriteLine("  <tr class=\"sbkSaav_TitleRow2\"><td colspan=\"3\">OAI-PMH Settings</td></tr>");
			Output.WriteLine("  <tr class=\"sbkSaav_TextRow\"><td colspan=\"3\"><p>You can use OAI-PMH to expose all the metadata of the resources within this aggregation for automatic harvesting by other repositories.  Additionally, you can choose to attach metadata to the collection-level OAI-PMH record.  This should be coded as dublin core tags.</p><p>For more information about the settings on this tab, <a href=\"" + UI_ApplicationCache_Gateway.Settings.Help_URL(RequestSpecificValues.Current_Mode.Base_URL) + "adminhelp/singleaggr\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</p></td></tr>");


			// Add the oai-pmh enabled flag
			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
			Output.WriteLine("    <td>&nbsp;</td>");
			Output.WriteLine("    <td class=\"sbkSaav_TableLabel\">Enabled Flag:</td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td>");
			Output.Write("           <input class=\"sbkSaav_checkbox\" type=\"checkbox\" name=\"admin_aggr_oai_flag\" id=\"admin_aggr_oai_flag\"");
			if (itemAggregation.OAI_Flag)
				Output.Write(" checked=\"checked\"");
			Output.WriteLine(" />");
			Output.WriteLine("           <label for=\"admin_aggr_oai_flag\">Include in OAI-PMH as a set?</label>");
			Output.WriteLine("        </td>");
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + OAI_FLAG_HELP + "');\"  title=\"" + OAI_FLAG_HELP + "\" /></td></tr></table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");

			// Add label for adding metadata to this OAI-SET
			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
			Output.WriteLine("    <td>&nbsp;</td>");
			Output.WriteLine("    <td class=\"sbkSaav_TableLabel\" colspan=\"2\">");
			Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr>");
			Output.WriteLine("        <td><label for=\"admin_aggr_oai_metadata\">Additional dublin core metadata to include in OAI-PMH set list:</label></td>");
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + OAI_METADATA_HELP + "');\"  title=\"" + OAI_METADATA_HELP + "\" /></td></tr></table>");
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

		#region Methods to render (and parse) page 5 -  Appearance

		private void Save_Page_5_Postback(NameValueCollection Form)
		{
			// Some interesting custom actions on this page, so get the actions
			// query string first
			string action = Form["admin_aggr_action"];
			if (action.Length > 0)
			{
				switch (action)
				{
                    case "enable_custom_home":
                        string custom_home = aggregationDirectory + "\\html\\custom\\home\\custom_home.html";
						if (!Directory.Exists(aggregationDirectory + "\\html\\custom\\home"))
							Directory.CreateDirectory(aggregationDirectory + "\\html\\custom\\home");
                        itemAggregation.Custom_Home_Page_Source_File = "html\\custom\\home\\custom_home.html";;
                        if (!File.Exists(custom_home))
						{
                            StreamWriter writer = new StreamWriter(custom_home);
                            writer.WriteLine("<html>");
                            writer.WriteLine("<head>");
                            writer.WriteLine("<title>Custom home page for " + itemAggregation.ShortName + "</title>");
                            writer.WriteLine("</head>");
                            writer.WriteLine("<body>");
							writer.WriteLine("<br /><br />THIS IS THE DEFAULT CUSTOM HOME PAGE FOR THIS COLLECTION<br /><br /><br />");
                            writer.WriteLine("</body>");
                            writer.WriteLine("</html>");
							writer.WriteLine();
							writer.Flush();
							writer.Close();
						}
						break;

                    case "delete_custom_home":
				        string file_to_delete = Form["admin_aggr_custom_home"];
				        if (!String.IsNullOrEmpty(file_to_delete))
				        {
				            string custom_home_delete_file = aggregationDirectory + "\\html\\custom\\home\\" + file_to_delete;
				            if (File.Exists(custom_home_delete_file))
				            {
                                try
                                {
                                    File.Delete(custom_home_delete_file);
                                }
                                catch { }
				            }
				        }
                        if ( itemAggregation.Custom_Home_Page_Source_File == "html\\custom\\home\\" + file_to_delete )
                            itemAggregation.Custom_Home_Page_Source_File = String.Empty;
						break;

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
                        itemAggregation.CSS_File = String.Empty;
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

							itemAggregation.Home_Page_File_Dictionary[enumVal] = "html\\home\\" + new_file_name;

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
								itemAggregation.Banner_Dictionary[enumVal] = "images\\banners\\" + bfile;
							}
							else
							{
								Item_Aggregation_Front_Banner.Item_Aggregation_Front_Banner_Type btypeEnum = Item_Aggregation_Front_Banner.Item_Aggregation_Front_Banner_Type.FULL;
								if ( btype == "left" )
									btypeEnum = Item_Aggregation_Front_Banner.Item_Aggregation_Front_Banner_Type.LEFT;
								if ( btype == "right" )
									btypeEnum = Item_Aggregation_Front_Banner.Item_Aggregation_Front_Banner_Type.RIGHT;
								Item_Aggregation_Front_Banner newFront = new Item_Aggregation_Front_Banner("images\\banners\\" + bfile) {Banner_Type = btypeEnum};

								try
								{
									string banner_file = aggregationDirectory + "\\images\\banners\\" + bfile;
									if (File.Exists(banner_file))
									{
										using (Image bannerImage = Image.FromFile(banner_file))
										{
											newFront.Width = (ushort) bannerImage.Width;
											newFront.Height = (ushort) bannerImage.Height;
											bannerImage.Dispose();
										}
									}
								}
								catch (Exception)
								{

								}


								itemAggregation.Front_Banner_Dictionary[enumVal] = newFront;
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
						if ( action.IndexOf("delete_standard_") == 0 )
						{
							string code_to_delete = action.Replace("delete_standard_", "");
							Web_Language_Enum enum_to_delete = Web_Language_Enum_Converter.Code_To_Enum(code_to_delete);
							itemAggregation.Banner_Dictionary.Remove(enum_to_delete);
						}
						if (action.IndexOf("delete_front_") == 0)
						{
							string code_to_delete = action.Replace("delete_front_", "");
							Web_Language_Enum enum_to_delete = Web_Language_Enum_Converter.Code_To_Enum(code_to_delete);
							itemAggregation.Front_Banner_Dictionary.Remove(enum_to_delete);
						}
						break;

				}
			}

			// Set the web skin
			itemAggregation.Web_Skins.Clear();
			itemAggregation.Default_Skin = String.Empty;
			if (( Form["admin_aggr_skin_1"] != null ) && ( Form["admin_aggr_skin_1"].Length > 0 ))
			{
				itemAggregation.Web_Skins.Add( Form["admin_aggr_skin_1"] );
				itemAggregation.Default_Skin = Form["admin_aggr_skin_1"];
			}

			// Set the custom home source file
			if (Form["admin_aggr_custom_home"] != null)
			{
			    string custom_home_file_name = Form["admin_aggr_custom_home"];
                if ((String.IsNullOrEmpty(custom_home_file_name)) || (!File.Exists(aggregationDirectory + "\\html\\custom\\home\\" + custom_home_file_name)))
			    {
			        itemAggregation.Custom_Home_Page_Source_File = String.Empty;
			    }
			    else
                {
                    itemAggregation.Custom_Home_Page_Source_File = "html\\custom\\home\\" + custom_home_file_name;
                }
			}
		}


		private void Add_Page_5(TextWriter Output)
		{
			// Help constants (for now)
			const string WEB_SKIN_HELP = "Web skin help place holder";
			const string CSS_HELP = "Aggregation-level CSS help place holder";
			const string CUSTOM_HOME_PAGE = "Custom home page help place holder";
			const string NEW_HOME_PAGE_HELP = "New home page help place holder";
			const string NEW_BANNER_HELP = "New banner help place holder";
			const string UPLOAD_BANNER_HELP = "Upload new banner help place holder";



			Output.WriteLine("<table class=\"sbkAdm_PopupTable\">");

			Output.WriteLine("  <tr class=\"sbkSaav_TitleRow\"><td colspan=\"3\">Appearance Options</td></tr>");
			Output.WriteLine("  <tr class=\"sbkSaav_TextRow\"><td colspan=\"3\"><p>These three settings have the most profound affects on the appearance of this aggregation, by forcing it to appear under a particular web skin, allowing a custom aggregation-level stylesheet, or completely overriding the system-generated home page for a custom home page HTML source file.</p><p>For more information about the settings on this tab, <a href=\"" + UI_ApplicationCache_Gateway.Settings.Help_URL(RequestSpecificValues.Current_Mode.Base_URL) + "adminhelp/singleaggr\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</p></td></tr>");

			// Add the web skin code
			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\" >");
			Output.WriteLine("    <td style=\"width:50px;\">&nbsp;</td>");
			Output.WriteLine("    <td class=\"sbkSaav_TableLabel\" style=\"width:140px\">Web Skin:</label></td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <table class=\"sbkSaav_InnerTable\">");
			Output.WriteLine("      <tr>");
			Output.WriteLine("        <td>");
			// Get the ordered list of all skin codes
            ReadOnlyCollection<string> skinCodes = UI_ApplicationCache_Gateway.Web_Skin_Collection.Ordered_Skin_Codes;
			for (int i = 0; i < 1; i++) // itemAggregation.Web_Skins.Count + 5; i++)
			{
				string skin = String.Empty;
				if (i < itemAggregation.Web_Skins.Count)
					skin = itemAggregation.Web_Skins[i];
				Skin_Writer_Helper(Output, "admin_aggr_skin_" + (i + 1).ToString(), skin, skinCodes);
				if ((i + 1) % 3 == 0)
					Output.WriteLine("<br />");
			}
			Output.WriteLine("        </td>");
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + WEB_SKIN_HELP + "');\"  title=\"" + WEB_SKIN_HELP + "\" /></td></tr></table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");


			// Add the css line
			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
			Output.WriteLine("    <td>&nbsp;</td>");
			Output.WriteLine("    <td class=\"sbkSaav_TableLabel\"><label for=\"admin_aggr_shortname\">Custom Stylesheet:</label></td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <table class=\"sbkSaav_InnerTable\">");
			Output.WriteLine("        <tr>");

			if (itemAggregation.CSS_File.Length == 0)
			{
				Output.WriteLine("          <td><span style=\"font-style:italic; padding-right:20px;\">No custom aggregation-level stylesheet</span></td>");
				Output.WriteLine("          <td><button title=\"Enable an aggregation-level stylesheet\" class=\"sbkAdm_RoundButton\" onclick=\"return aggr_edit_enable_css();\">ENABLE</button></td>");
			}
			else
			{
				string css_url = RequestSpecificValues.Current_Mode.Base_Design_URL + "aggregations/" + itemAggregation.Code + "/" + itemAggregation.CSS_File;
				Output.WriteLine("          <td style=\"padding-right:20px;\"><a href=\"" + css_url + "\" title=\"View CSS file\">" + itemAggregation.CSS_File + "</a></td>");
				Output.WriteLine("          <td style=\"padding-right:10px;\"><button title=\"Disable this aggregation-level stylesheet\" class=\"sbkAdm_RoundButton\" onclick=\"return aggr_edit_disable_css();\">DISABLE</button></td>");
				Output.WriteLine("          <td><button title=\"Edit this aggregation-level stylesheet\" class=\"sbkAdm_RoundButton\" onclick=\"return new_aggr_edit_page('y');\">EDIT</button></td>");
			}
			Output.WriteLine("          <td><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + CSS_HELP + "');\"  title=\"" + CSS_HELP + "\" /></td>");
			Output.WriteLine("        </tr>");
			Output.WriteLine("      </table>");
			Output.WriteLine("    </td>");
			Output.WriteLine("  </tr>");

			// Add the custom home page HTML for this aggregation
			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\" >");
			Output.WriteLine("    <td style=\"width:50px;\">&nbsp;</td>");
			Output.WriteLine("    <td class=\"sbkSaav_TableLabel\" style=\"width:140px\">Custom Home Page:</label></td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <table class=\"sbkSaav_InnerTable\">");
			Output.WriteLine("        <tr>");
			

			// Get the list of all HTML at the top-level
		    if (!Directory.Exists(aggregationDirectory + "\\html\\custom\\home"))
		        Directory.CreateDirectory(aggregationDirectory + "\\html\\custom\\home");
            string[] html_files = Directory.GetFiles(aggregationDirectory + "\\html\\custom\\home", "*.htm*");
			if (html_files.Length == 0)
			{
				Output.WriteLine("          <td><span style=\"font-style:italic; padding-right: 5px;\">No html source files</span></td>");
                Output.WriteLine("          <td><button title=\"Enable a completely custom home page\" class=\"sbkAdm_RoundButton\" onclick=\"return aggr_edit_enable_custom_home();\">ENABLE</button></td>");
			}
			else
			{
                Output.WriteLine("          <td>");

				// Start the select box
                Output.Write("            <select class=\"sbkSaav_SelectSingle\" name=\"admin_aggr_custom_home\" id=\"admin_aggr_custom_home\" onchange=\"aggr_edit_custom_home_selectchange();\">");

				// Add the NONE option first
			    bool custom_home_exists = itemAggregation.Custom_Home_Page_Source_File.Length > 0;
                Output.Write(!custom_home_exists ? "<option value=\"\" selected=\"selected\" ></option>" : "<option value=\"\"></option>");

				// Add each possible source file
			    string current_file = itemAggregation.Custom_Home_Page_Source_File.Replace("html\\custom\\home\\", "");

				foreach (string thisFile in html_files)
				{
					string thisFileName = (new FileInfo(thisFile)).Name;

                    if (String.Compare(current_file, thisFileName, StringComparison.OrdinalIgnoreCase) == 0)
					{
						Output.Write("<option value=\"" + thisFileName + "\" selected=\"selected\" >" + HttpUtility.HtmlEncode(thisFileName) + "</option>");
					}
					else
					{
						Output.Write("<option value=\"" + thisFileName + "\">" + HttpUtility.HtmlEncode(thisFileName) + "</option>");
					}
				}
				Output.WriteLine("</select> &nbsp; &nbsp; ");
                Output.WriteLine("          </td>");
			    Output.Write("          <td style=\"padding-right:10px;\"><button title=\"Delete this custom home page file\" class=\"sbkAdm_RoundButton\" id=\"customHomePageDeleteButton\" onclick=\"return aggr_edit_delete_custom_home();\"");
                if ( !custom_home_exists )
    			    Output.Write(" disabled=\"disabled\"");
                Output.WriteLine(">DELETE</button></td>");
                Output.WriteLine("          <td><button title=\"Add a new custom home page file\" class=\"sbkAdm_RoundButton\" onclick=\"return aggr_edit_enable_css();\">ADD</button></td>");
			}
			
			Output.WriteLine("          <td><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + CUSTOM_HOME_PAGE + "');\"  title=\"" + CUSTOM_HOME_PAGE + "\" /></td>");
            Output.WriteLine("        </tr>");
            Output.WriteLine("      </table>");
			Output.WriteLine("    </td>");
			Output.WriteLine("  </tr>");

			Output.WriteLine("  <tr class=\"sbkSaav_TitleRow2\"><td colspan=\"3\">Home Page Text</td></tr>");
			Output.WriteLine("  <tr class=\"sbkSaav_TextRow\"><td colspan=\"3\"><p>This section controls all the language-specific (and default) text which appears on the home page.</p><p>For more information about the settings on this tab, <a href=\"" + UI_ApplicationCache_Gateway.Settings.Help_URL(RequestSpecificValues.Current_Mode.Base_URL) + "adminhelp/singleaggr\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</p></td></tr>");

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
			Output.WriteLine("        </tr>");

			// Get the list of all recently added home page languages
			List<Web_Language_Enum> newLanguages = HttpContext.Current.Session["Item_Aggr_Edit_" + itemAggregation.Code + "_NewLanguages"] as List<Web_Language_Enum> ?? new List<Web_Language_Enum>();

			// Add all the home page information
			Web_Language_Enum currLanguage = RequestSpecificValues.Current_Mode.Language;
			RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Aggregation;
			List<string> existing_languages = new List<string>();
			foreach (KeyValuePair<Web_Language_Enum, string> thisHomeSource in itemAggregation.Home_Page_File_Dictionary)
			{
				Output.WriteLine("        <tr>");
				bool canDelete = true;
				if ((thisHomeSource.Key == Web_Language_Enum.DEFAULT) || (thisHomeSource.Key == Web_Language_Enum.UNDEFINED ) || (thisHomeSource.Key == UI_ApplicationCache_Gateway.Settings.Default_UI_Language))
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

				string file = RequestSpecificValues.Current_Mode.Base_Design_URL + "aggregations/" + itemAggregation.Code + "/" + thisHomeSource.Value.Replace("\\","/");

				Output.WriteLine("          <td><a href=\"" + file + "\" title=\"View source file\">" + thisHomeSource.Value.Replace("html\\home\\", "") + "</a></td>");
				Output.Write("          <td class=\"sbkAdm_ActionLink\" >( ");

				if (!newLanguages.Contains(thisHomeSource.Key))
				{
					if (canDelete)
					{
						RequestSpecificValues.Current_Mode.Language = thisHomeSource.Key;
						RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Home;
						Output.Write("<a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\" title=\"View this home page in " + Web_Language_Enum_Converter.Enum_To_Name(thisHomeSource.Key)  + "\" target=\"VIEW" + itemAggregation.Code + "_" + Web_Language_Enum_Converter.Enum_To_Code(thisHomeSource.Key) + "\">view</a> | ");

						RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Home_Edit;
						Output.Write("<a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\" title=\"Edit this home page in " + Web_Language_Enum_Converter.Enum_To_Name(thisHomeSource.Key)  + "\" target=\"EDIT" + itemAggregation.Code + "_" + Web_Language_Enum_Converter.Enum_To_Code(thisHomeSource.Key) + "\">edit</a> ");
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
				Output.WriteLine("        </tr>");
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
				if ( !existing_languages.Contains(possible_language))
					Output.Write("<option value=\"" + Web_Language_Enum_Converter.Name_To_Code(possible_language) + "\">" + HttpUtility.HtmlEncode(possible_language) + "</option>");
			}
			Output.WriteLine();
			Output.WriteLine("        </td>");
			Output.WriteLine("        <td style=\"padding-left:35px;\">Copy from existing home: </td>");
			Output.WriteLine("        <td>");
			Output.Write("          <select id=\"admin_aggr_new_home_copy\" name=\"admin_aggr_new_home_copy\">");
			Output.Write("<option value=\"\" selected=\"selected\"></option>");
			foreach (KeyValuePair<Web_Language_Enum, string> thisHomeSource in itemAggregation.Home_Page_File_Dictionary)
			{
				if ((thisHomeSource.Key == Web_Language_Enum.DEFAULT) || (thisHomeSource.Key == UI_ApplicationCache_Gateway.Settings.Default_UI_Language))
				{
					Output.Write("<option value=\"" + thisHomeSource.Value  + "\">" + HttpUtility.HtmlEncode(Web_Language_Enum_Converter.Enum_To_Name(UI_ApplicationCache_Gateway.Settings.Default_UI_Language)) + "</option>");
				}
				else
				{
					Output.Write("<option value=\"" + thisHomeSource.Value + "\">" + HttpUtility.HtmlEncode(Web_Language_Enum_Converter.Enum_To_Name(thisHomeSource.Key)) + "</option>");
				}
			}

			Output.WriteLine("</select>");
			Output.WriteLine("        </td>");
			Output.WriteLine("        <td style=\"padding-left:20px\"><button title=\"Add new home page\" class=\"sbkAdm_RoundButton\" onclick=\"return new_aggr_add_home();\">ADD</button></td>");
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + NEW_HOME_PAGE_HELP + "');\"  title=\"" + NEW_HOME_PAGE_HELP + "\" /></td></tr></table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");


			Output.WriteLine("  <tr class=\"sbkSaav_TitleRow2\"><td colspan=\"3\">Banners</td></tr>");
			Output.WriteLine("  <tr class=\"sbkSaav_TextRow\"><td colspan=\"3\"><p>This section shows all the existing language-specific banners for this aggregation and allows you upload new banners for this aggregation.</p><p>For more information about the settings on this tab, <a href=\"" + UI_ApplicationCache_Gateway.Settings.Help_URL(RequestSpecificValues.Current_Mode.Base_URL) + "adminhelp/singleaggr\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</p></td></tr>");


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
				switch (thisBannerInfo.Value.Banner_Type)
				{
					case Item_Aggregation_Front_Banner.Item_Aggregation_Front_Banner_Type.FULL:
						Output.WriteLine("          <td>Home Page</td>");
						break;

					case Item_Aggregation_Front_Banner.Item_Aggregation_Front_Banner_Type.LEFT:
						Output.WriteLine("          <td>Home Page - Left</td>");
						break;

					case Item_Aggregation_Front_Banner.Item_Aggregation_Front_Banner_Type.RIGHT:
						Output.WriteLine("          <td>Home Page - Right</td>");
						break;

				}
				

				string file = RequestSpecificValues.Current_Mode.Base_Design_URL + "aggregations/" + itemAggregation.Code + "/" + thisBannerInfo.Value.Image_File.Replace("\\", "/");

				Output.Write("          <td class=\"sbkAdm_ActionLink\" > ( <a  href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return aggr_edit_delete_banner('" + Web_Language_Enum_Converter.Enum_To_Code(thisBannerInfo.Key) + "', 'front');\" title=\"Delete this banner\" >delete</a> )</td>");

				
				Output.WriteLine("          <td><a href=\"" + file + "\" title=\"View banner image file\" target=\"" + itemAggregation.Code + "_" + thisBannerInfo.Value.Image_File.Replace("\\", "_").Replace("/", "_") + "\"><img src=\"" + file + "\" alt=\"THIS BANNER IMAGE IS MISSING\" class=\"sbkSaav_BannerImage\" /></a></td>");
				Output.WriteLine("        </tr>");
			}

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

			Output.WriteLine("      </table>");
			Output.WriteLine("    </td>");
			Output.WriteLine("  </tr>");

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

			// Write the add new banner information
			if (banner_files.Length > 0)
			{
				Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
				Output.WriteLine("    <td>&nbsp;</td>");
				Output.WriteLine("    <td class=\"sbkSaav_TableLabel2\">New Banner:</td>");
				Output.WriteLine("    <td></td>");
				Output.WriteLine("  </tr>");
				Output.WriteLine("  <tr class=\"sbkSaav_TallRow\">");
				Output.WriteLine("    <td>&nbsp;</td>");
				Output.WriteLine("    <td colspan=\"2\">");

				string current_banner = RequestSpecificValues.Current_Mode.Base_Design_URL + "aggregations/" + itemAggregation.Code + "/images/banners/" + Path.GetFileName(banner_files[0]);
				Output.WriteLine("      <div style=\"width:510px; float:right;\"><img id=\"sbkSaav_SelectedBannerImage\" name=\"sbkSaav_SelectedBannerImage\" src=\"" + current_banner + "\" alt=\"Missing\" Title=\"Selected image file\" /></div>");

				Output.WriteLine("      <table class=\"sbkSaav_BannerInnerTable\">");
				Output.WriteLine("        <tr>");
				Output.WriteLine("          <td>Language:</td>");
				Output.WriteLine("          <td>");
				Output.Write("            <select class=\"sbkSaav_SelectSingle\" id=\"admin_aggr_new_banner_lang\" name=\"admin_aggr_new_banner_lang\">");

				// Add each language in the combo box
				foreach (string possible_language in Web_Language_Enum_Converter.Language_Name_Array)
				{
					Output.Write("<option value=\"" + Web_Language_Enum_Converter.Name_To_Code(possible_language) + "\">" + HttpUtility.HtmlEncode(possible_language) + "</option>");
				}
				Output.WriteLine();
				Output.WriteLine("          </td>");
				Output.WriteLine("          <td><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + NEW_BANNER_HELP + "');\"  title=\"" + NEW_BANNER_HELP + "\" /></td>");
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
				if ( !String.IsNullOrEmpty(last_added_banner))
				{
					Output.Write("<option selected=\"selected\" value=\"" + last_added_banner + "\">" + last_added_banner + "</option>");
				}
				foreach (string thisFile in banner_files)
				{
					string name = Path.GetFileName(thisFile);
					if ( name != last_added_banner )
						Output.Write("<option value=\"" + name + "\">" + name + "</option>");
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
			Output.WriteLine("           <td><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + UPLOAD_BANNER_HELP + "');\"  title=\"" + UPLOAD_BANNER_HELP + "\" /></td>");
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

		private void Skin_Writer_Helper(TextWriter Output, string SkinID, string Skin, IEnumerable<string> Skin_Codes )
		{
			// Start the select box
			Output.Write("          <select class=\"sbkSaav_SelectSingle\" name=\"" + SkinID + "\" id=\"" + SkinID + "\">");

			// Add the NONE option first
			Output.Write(Skin.Length == 0 ? "<option value=\"\" selected=\"selected\" ></option>" : "<option value=\"\"></option>");

			// Add each metadata field to the select boxes
			foreach ( string skinCode in Skin_Codes)
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

		#region Methods to render (and parse) page 6 - Highlights

		private void Save_Page_6_Postback(NameValueCollection Form)
		{

		}

		private void Add_Page_6(TextWriter Output)
		{
			Output.WriteLine("<table class=\"popup_table\">");

			// Add the highlight type
			Output.Write("<tr><td width=\"120px\">Highlights Type:</td><td><input type=\"radio\" name=\"admin_aggr_highlight_type\" id=\"rotating\" value=\"rotating\"");
			if (itemAggregation.Rotating_Highlights)
				Output.Write(" checked=\"checked\"");
			Output.Write("/><label for=\"rotating\">Rotating</label> &nbsp; <input type=\"radio\" name=\"admin_aggr_highlight_type\" id=\"static\" value=\"static\"");
			if (!itemAggregation.Rotating_Highlights)
				Output.Write(" checked=\"checked\"");
			Output.WriteLine("/><label for=\"static\">Static</label></td></tr>");

			// Determine the maximum number of languages used in tooltips and text
			int max_tooltips = 0;
			int max_text = 0;
			foreach (Item_Aggregation_Highlights thisHighlight in itemAggregation.Highlights)
			{
				max_tooltips = Math.Max(max_tooltips, thisHighlight.Tooltip_Dictionary.Count);
				max_text = Math.Max(max_text, thisHighlight.Text_Dictionary.Count);
			}
			max_tooltips += 1;
			max_text += 1;

			// Add each highlight
			for (int i = 0; i < itemAggregation.Highlights.Count + 5; i++)
			{
				// Add some space and a line
				Output.WriteLine("<tr><td colspan=\"2\">&nbsp;</td></tr>");
				Output.WriteLine("<tr style=\"background:#333333\"><td colspan=\"2\"></td></tr>");
				Output.WriteLine("<tr><td colspan=\"2\">&nbsp;</td></tr>");

				// Either get the highlight, or just make one
				Item_Aggregation_Highlights emptyHighlight = new Item_Aggregation_Highlights();
				if (i < itemAggregation.Highlights.Count)
					emptyHighlight = itemAggregation.Highlights[i];

				// Now, add it to the form
				Highlight_Writer_Helper(Output, i + 1, emptyHighlight, max_text, max_tooltips);
			}

			Output.WriteLine("</table>");
			Output.WriteLine("<br />");
		}

		private void Highlight_Writer_Helper(TextWriter Output, int HighlightCounter, Item_Aggregation_Highlights Highlight, int Max_Text, int Max_Tooltips)
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
						Item_Aggregation_Child_Page newPage = new Item_Aggregation_Child_Page {Code = childPageCode, Parent_Code = childPageParent, Source = Item_Aggregation_Child_Page.Source_Type.Static_HTML };
						newPage.Add_Label(childPageLabel, UI_ApplicationCache_Gateway.Settings.Default_UI_Language);
						switch (childPageVisibility)
						{
							case "none":
								newPage.Browse_Type = Item_Aggregation_Child_Page.Visibility_Type.NONE;
								break;

							case "browse":
								newPage.Browse_Type = Item_Aggregation_Child_Page.Visibility_Type.MAIN_MENU;
								break;

							case "browseby":
								newPage.Browse_Type = Item_Aggregation_Child_Page.Visibility_Type.METADATA_BROWSE_BY;
								break;
						}
						string html_source_dir = aggregationDirectory + "\\html\\browse";
						if (!Directory.Exists(html_source_dir))
							Directory.CreateDirectory(html_source_dir);
						string html_source_file = html_source_dir + "\\" + childPageCode + "_" + Web_Language_Enum_Converter.Enum_To_Code(UI_ApplicationCache_Gateway.Settings.Default_UI_Language) + ".html";
						if (!File.Exists(html_source_file))
						{
							HTML_Based_Content htmlContent = new HTML_Based_Content();
							htmlContent.Static_Text = "<br /><br />This is a new browse page.<br /><br />" + childPageLabel + "<br /><br />The code for this browse is: " + childPageCode;
							htmlContent.Author = RequestSpecificValues.Current_User.Full_Name;
							htmlContent.Date = DateTime.Now.ToLongDateString();
							htmlContent.Title = childPageLabel;
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
			const string CODE_HELP = "New child page code help placeholder";
			const string LABEL_HELP = "New child page label help placeholder";
			const string VISIBILITY_HELP = "New child page visibility help placeholder";
			const string PARENT_HELP = "New child page parent help place holder";

			if (actionMessage.Length > 0)
			{
				Output.WriteLine("  <br />");
				Output.WriteLine("  <div id=\"sbkAdm_ActionMessage\" style=\"color:Maroon;\">" + actionMessage + "</div>");
			}


			Output.WriteLine("<table class=\"sbkAdm_PopupTable\">");

			Output.WriteLine("  <tr class=\"sbkSaav_TitleRow\"><td colspan=\"3\">Child Pages</td></tr>");
			Output.WriteLine("  <tr class=\"sbkSaav_TextRow\"><td colspan=\"3\"><p>Child pages are pages related to the aggregation and allow additional information to be presented within the same aggregational branding.  These can appear in the aggregation main menu, with any metadata browses pulled from the database, or you can set them to for no automatic visibility, in which case they are only accessible by links in the home page or other child pages.</p><p>For more information about the settings on this tab, <a href=\"" + UI_ApplicationCache_Gateway.Settings.Help_URL(RequestSpecificValues.Current_Mode.Base_URL) + "adminhelp/singleaggr\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</p></td></tr>");

			// Put in alphabetical order
			SortedList<string, Item_Aggregation_Child_Page> sortedChildren = new SortedList<string, Item_Aggregation_Child_Page>();
			foreach (Item_Aggregation_Child_Page childPage in itemAggregation.Child_Pages)
			{
				if (childPage.Source == Item_Aggregation_Child_Page.Source_Type.Static_HTML)
				{
					sortedChildren.Add(childPage.Code, childPage);
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

				foreach (Item_Aggregation_Child_Page childPage in sortedChildren.Values)
				{
					Output.WriteLine("        <tr>");
					Output.Write("          <td class=\"sbkAdm_ActionLink\" style=\"padding-left: 5px;\" >( ");
					RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Aggregation;
					RequestSpecificValues.Current_Mode.Aggregation_Type = childPage.Browse_Type == Item_Aggregation_Child_Page.Visibility_Type.METADATA_BROWSE_BY ? Aggregation_Type_Enum.Browse_By : Aggregation_Type_Enum.Browse_Info;
					RequestSpecificValues.Current_Mode.Info_Browse_Mode = childPage.Code;

					Output.Write("<a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\" title=\"View this child page\" target=\"VIEW_" + childPage.Code + "\">view</a> | ");

					RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Administrative;
					RequestSpecificValues.Current_Mode.My_Sobek_SubMode = "g_" + childPage.Code;
					Output.Write("<a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\" title=\"Edit this child page\" >edit</a> | ");
					Output.WriteLine("<a title=\"Click to delete this subcollection\" href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "l/technical/javascriptrequired\" onclick=\"return edit_aggr_delete_child_page('" + childPage.Code + "');\">delete</a> )</td>");

					Output.WriteLine("          <td>" + childPage.Code + "</td>");
					Output.WriteLine("          <td>" + childPage.Get_Label(UI_ApplicationCache_Gateway.Settings.Default_UI_Language) + "</td>");

					switch (childPage.Browse_Type)
					{
						case Item_Aggregation_Child_Page.Visibility_Type.MAIN_MENU:
							Output.WriteLine("          <td>Main Menu</td>");
							break;

						case Item_Aggregation_Child_Page.Visibility_Type.METADATA_BROWSE_BY:
							Output.WriteLine("          <td>Browse By</td>");
							break;

						case Item_Aggregation_Child_Page.Visibility_Type.NONE:
							Output.WriteLine("          <td>None</td>");
							break;
					}
					Output.WriteLine("          <td>" + childPage.Parent_Code + "</td>");

					Output.Write("          <td>");
					int language_count = 0;
					int total_language_count = childPage.Source_Dictionary.Count;
					foreach (Web_Language_Enum thisLanguage in childPage.Source_Dictionary.Keys)
					{
						string languageName = Web_Language_Enum_Converter.Enum_To_Name(thisLanguage);
						if ((thisLanguage == Web_Language_Enum.DEFAULT) || (thisLanguage == Web_Language_Enum.UNDEFINED) || (thisLanguage == RequestSpecificValues.Current_Mode.Default_Language))
							languageName = "<span style=\"font-style:italic\">default</span>";
						if ( language_count == 0 )
							Output.Write(languageName);
						else
							Output.Write(", " + languageName);

						language_count++;
						if ((language_count > 4) && (language_count < total_language_count - 1))
						{
							Output.Write("... (" + ( total_language_count - language_count ) + "more)");
							break;
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
			Output.WriteLine("          <td colspan=\"2\"><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + CODE_HELP + "');\"  title=\"" + CODE_HELP + "\" /></td>");
			Output.WriteLine("        </tr>");

			// Add the default language label
			Output.WriteLine("        <tr>");
			Output.WriteLine("          <td><label for=\"admin_aggr_label\">Title (default):</label></td>");
			Output.WriteLine("          <td colspan=\"2\"><input class=\"sbkSaav_SubLargeInput sbkAdmin_Focusable\" name=\"admin_aggr_label\" id=\"admin_aggr_label\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(childPageLabel ?? String.Empty) + "\" /></td>");
			Output.WriteLine("          <td style=\"width:30px\"><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + LABEL_HELP + "');\"  title=\"" + LABEL_HELP + "\" /></td>");
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
			Output.WriteLine("          <td colspan=\"2\"><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + VISIBILITY_HELP + "');\"  title=\"" + VISIBILITY_HELP + "\" /></td>");
			Output.WriteLine("        </tr>");

			// Add line for parent code
			if ((!String.IsNullOrEmpty(childPageVisibility)) && (childPageVisibility == "browse"))
				Output.WriteLine("        <tr id=\"admin_aggr_parent_row\" style=\"display:table-row\">");
			else
				Output.WriteLine("        <tr id=\"admin_aggr_parent_row\" style=\"display:none\">");

			Output.WriteLine("          <td><label for=\"admin_aggr_parent\">Parent:</label></td>");
			Output.Write("          <td><select class=\"sbkSaav_SubTypeSelect\" name=\"admin_aggr_parent\" id=\"admin_aggr_parent\">");
			Output.Write("<option value=\"\">(none - top level)</option>");
			foreach (Item_Aggregation_Child_Page childPage in sortedChildren.Values)
			{
				// Only show main menu stuff
				if (childPage.Browse_Type != Item_Aggregation_Child_Page.Visibility_Type.MAIN_MENU)
					continue;

				if ( childPageParent == childPage.Code )
					Output.Write("<option value=\"" + childPage.Code + "\" selected=\"selected\">" + childPage.Code + "</option>");
				else
					Output.Write("<option value=\"" + childPage.Code + "\">" + childPage.Code + "</option>");

			}
			Output.WriteLine("</select></td>");
			Output.WriteLine("          <td colspan=\"2\"><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + PARENT_HELP + "');\"  title=\"" + PARENT_HELP + "\" /></td>");
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

			string new_aggregation_code = String.Empty;
			if (Form["admin_aggr_code"] != null)
				new_aggregation_code = Form["admin_aggr_code"].ToUpper().Trim();

			
			// Was this to delete the aggregation?
			if ((action.IndexOf("delete_") == 0) && ( action.Length > 7))
			{
				string code_to_delete = action.Substring(7);

				string delete_error;
				int errorCode = SobekCM_Database.Delete_Item_Aggregation(code_to_delete, RequestSpecificValues.Current_User.Is_System_Admin, RequestSpecificValues.Current_User.Full_Name, null, out delete_error);
				if (errorCode <= 0)
				{
					string delete_folder = UI_ApplicationCache_Gateway.Settings.Base_Design_Location + "aggregations\\" + code_to_delete;
					if (SobekCM_File_Utilities.Delete_Folders_Recursively(delete_folder))
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

				return;
			}


			// If there was a save value continue to pull the rest of the data


			bool is_active = false;
			bool is_hidden = true;


			// Pull the values from the submitted form
			string new_type = Form["admin_aggr_type"];
			string new_name = Form["admin_aggr_name"].Trim();
			string new_shortname = Form["admin_aggr_shortname"].Trim();
			string new_description = Form["admin_aggr_desc"].Trim();

			object temp_object = Form["admin_aggr_isactive"];
			if (temp_object != null)
			{
				is_active = true;
			}

			temp_object = Form["admin_aggr_ishidden"];
			if (temp_object != null)
			{
				is_hidden = false;
			}

			// Convert to the integer id for the parent and begin to do checking
			List<string> errors = new List<string>();

			// Validate the code
			if (new_aggregation_code.Length > 20)
			{
				errors.Add("New aggregation code must be twenty characters long or less");
			}
			else if (new_aggregation_code.Length == 0)
			{
				errors.Add("You must enter a CODE for this item aggregation");

			}
            else if (UI_ApplicationCache_Gateway.Aggregations[new_aggregation_code.ToUpper()] != null)
			{
				errors.Add("New code must be unique... <i>" + new_aggregation_code + "</i> already exists");
			}
			else if (UI_ApplicationCache_Gateway.Settings.Reserved_Keywords.Contains(new_aggregation_code.ToLower()))
			{
				errors.Add("That code is a system-reserved keyword.  Try a different code.");
			}

			// Was there a type and name
			if (new_type.Length == 0)
			{
				errors.Add("You must select a TYPE for this new aggregation");
			}
			if (new_description.Length == 0)
			{
				errors.Add("You must enter a DESCRIPTION for this new aggregation");
			}
			if (new_name.Length == 0)
			{
				errors.Add("You must enter a NAME for this new aggregation");
			}
			else
			{
				if (new_shortname.Length == 0)
					new_shortname = new_name;
			}

			if (errors.Count > 0)
			{
				// Create the error message
				actionMessage = "ERROR: Invalid entry for new item aggregation<br />";
				foreach (string error in errors)
					actionMessage = actionMessage + "<br />" + error;

				// Save all the values that were entered
				enteredCode = new_aggregation_code;
				enteredDescription = new_description;
				enteredIsActive = is_active;
				enteredIsHidden = is_hidden;
				enteredName = new_name;
				enteredShortname = new_shortname;
				enteredType = new_type;
			}
			else
			{
				// Get the correct type
				string correct_type = "Collection";
				switch (new_type)
				{
					case "coll":
						correct_type = "Collection";
						break;

					case "group":
						correct_type = "Collection Group";
						break;

					case "subcoll":
						correct_type = "SubCollection";
						break;

					case "inst":
						correct_type = "Institution";
						break;

					case "exhibit":
						correct_type = "Exhibit";
						break;

					case "subinst":
						correct_type = "Institutional Division";
						break;
				}

				// Make sure inst and subinst start with 'i'
				if (new_type.IndexOf("inst") >= 0)
				{
					if (new_aggregation_code[0] == 'I')
						new_aggregation_code = "i" + new_aggregation_code.Substring(1);
					if (new_aggregation_code[0] != 'i')
						new_aggregation_code = "i" + new_aggregation_code;
				}


				// Try to save the new item aggregation
				if (Engine_Database.Save_Item_Aggregation(new_aggregation_code, new_name, new_shortname, new_description, -1, correct_type, is_active, is_hidden, String.Empty, itemAggregation.Aggregation_ID, RequestSpecificValues.Current_User.Full_Name, null))
				{
					// Ensure a folder exists for this, otherwise create one
					try
					{
						string folder = UI_ApplicationCache_Gateway.Settings.Base_Design_Location + "aggregations\\" + new_aggregation_code.ToLower();
						if (!Directory.Exists(folder))
						{
							// Create this directory and all the subdirectories
							Directory.CreateDirectory(folder);
							Directory.CreateDirectory(folder + "/html");
							Directory.CreateDirectory(folder + "/images");
							Directory.CreateDirectory(folder + "/html/home");
							Directory.CreateDirectory(folder + "/images/buttons");
							Directory.CreateDirectory(folder + "/images/banners");

							// Create a default home text file
							StreamWriter writer = new StreamWriter(folder + "/html/home/text.html");
							writer.WriteLine("<br />New collection home page text goes here.<br /><br />To edit this, log on as the aggregation admin and hover over this text to edit it.<br /><br />");
							writer.Flush();
							writer.Close();

							// Copy the default banner and buttons from images
							if (File.Exists(UI_ApplicationCache_Gateway.Settings.Base_Directory + "default/images/default_button.png"))
								File.Copy(UI_ApplicationCache_Gateway.Settings.Base_Directory + "default/images/default_button.png", folder + "/images/buttons/coll.png");
							if (File.Exists(UI_ApplicationCache_Gateway.Settings.Base_Directory + "default/images/default_button.gif"))
								File.Copy(UI_ApplicationCache_Gateway.Settings.Base_Directory + "default/images/default_button.gif", folder + "/images/buttons/coll.gif");
							if (File.Exists(UI_ApplicationCache_Gateway.Settings.Base_Directory + "default/images/default_banner.jpg"))
								File.Copy(UI_ApplicationCache_Gateway.Settings.Base_Directory + "default/images/default_banner.jpg", folder + "/images/banners/coll.jpg");

							// Now, try to create the item aggregation and write the configuration file
                            Item_Aggregation childAggregation = Item_Aggregation_Utilities.Get_Item_Aggregation(new_aggregation_code, String.Empty, null, false, false, null);
							childAggregation.Write_Configuration_File(UI_ApplicationCache_Gateway.Settings.Base_Design_Location + childAggregation.ObjDirectory);
						}
					}
					catch
					{
						actionMessage = "ERROR saving the new item aggregation to the database";
					}

					// Reload the list of all codes, to include this new one and the new hierarchy
                    lock (UI_ApplicationCache_Gateway.Aggregations)
					{
                        Engine_Database.Populate_Code_Manager(UI_ApplicationCache_Gateway.Aggregations, null);
					}
					if (!String.IsNullOrEmpty(actionMessage))
						actionMessage = "New item aggregation <i>" + new_aggregation_code + "</i> saved successfully";
				}
				else
				{
					actionMessage = "ERROR saving the new item aggregation to the database";
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
				Output.WriteLine("          <th class=\"sbkSaav_SubCollectionTableHeader4\">ACTIVE?</th>");
				Output.WriteLine("          <th class=\"sbkSaav_SubCollectionTableHeader5\">NAME</th>");
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
						if (relatedAggr.Active)
							Output.WriteLine("          <td style=\"text-align: center\"><img src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/checkmark2.png\" alt=\"YES\" /></td>");
						else
							Output.WriteLine("          <td></td>");

						Output.WriteLine("          <td>" + relatedAggr.Name + "</td>");
						Output.WriteLine("        </tr>");
					}
				}
				RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Administrative;
				RequestSpecificValues.Current_Mode.My_Sobek_SubMode = itemAggregation.Code;

				Output.WriteLine("      </table>");
				Output.WriteLine("    </td>");
				Output.WriteLine("  </tr>");
			}

			if (itemAggregation.Aggregation_Type.ToUpper() != "EXHIBIT")
			{
				// Add ability to add NEW subcollections
				Output.WriteLine("  <tr class=\"sbkSaav_TallRow\">");
				Output.WriteLine("    <td style=\"width:50px\">&nbsp;</td>");
				Output.WriteLine("    <td class=\"sbkSaav_TableLabel2\" style=\"width:145px\">New Subcollection:</td>");
				Output.WriteLine("    <td>");
				Output.WriteLine("      <table class=\"sbkSaav_SubInnerTable\">");
				// Add line for aggregation code and aggregation type
				Output.WriteLine("      <tr>");
				Output.WriteLine("        <td style=\"width:120px;\"><label for=\"admin_aggr_code\">Code:</label></td>");
				Output.WriteLine("        <td><input class=\"sbkAsav_small_input sbkAdmin_Focusable\" name=\"admin_aggr_code\" id=\"admin_aggr_code\" type=\"text\" value=\"" + enteredCode + "\" /></td>");
				Output.WriteLine("        <td style=\"text-align:right;\">");
				Output.WriteLine("          <label for=\"admin_aggr_type\">Type:</label> &nbsp; ");
				Output.WriteLine("          <select class=\"sbkSaav_SubTypeSelect\" name=\"admin_aggr_type\" id=\"admin_aggr_type\">");
				if (enteredType == String.Empty)
					Output.WriteLine("            <option value=\"\" selected=\"selected\" ></option>");

				if ((itemAggregation.Aggregation_Type.IndexOf("Institution") < 0) && (itemAggregation.Aggregation_Type.IndexOf("Group") > 0))
				{
					Output.WriteLine(enteredType == "coll"
						                 ? "            <option value=\"coll\" selected=\"selected\" >Collection</option>"
						                 : "            <option value=\"coll\">Collection</option>");
				}

				Output.WriteLine(enteredType == "exhibit"
					                 ? "            <option value=\"exhibit\" selected=\"selected\" >Exhibit</option>"
					                 : "            <option value=\"exhibit\">Exhibit</option>");

				if (itemAggregation.Aggregation_Type.IndexOf("Institution") == 0)
				{
					Output.WriteLine(enteredType == "subinst"
						                 ? "            <option value=\"subinst\" selected=\"selected\" >Institutional Division</option>"
						                 : "            <option value=\"subinst\">Institutional Division</option>");
				}

				if (itemAggregation.Aggregation_Type.IndexOf("Institution") < 0)
				{
					Output.WriteLine(enteredType == "subcoll"
						                 ? "            <option value=\"subcoll\" selected=\"selected\" >SubCollection</option>"
						                 : "            <option value=\"subcoll\">SubCollection</option>");
				}

				Output.WriteLine("          </select>");
				Output.WriteLine("        </td>");
				Output.WriteLine("      </tr>");

				// Add the full name line
				Output.WriteLine("      <tr><td><label for=\"admin_aggr_name\">Name (full):</label></td><td colspan=\"2\"><input class=\"sbkSaav_SubLargeInput sbkAdmin_Focusable\" name=\"admin_aggr_name\" id=\"admin_aggr_name\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(enteredName) + "\" /></td></tr>");

				// Add the short name line
				Output.WriteLine("      <tr><td><label for=\"admin_aggr_shortname\">Name (short):</label></td><td colspan=\"2\"><input class=\"sbkSaav_SubLargeInput sbkAdmin_Focusable\" name=\"admin_aggr_shortname\" id=\"admin_aggr_shortname\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(enteredShortname) + "\" /></td></tr>");

				// Add the description box
				Output.WriteLine("      <tr style=\"vertical-align:top\"><td><label for=\"admin_aggr_desc\">Description:</label></td><td colspan=\"2\"><textarea rows=\"6\" name=\"admin_aggr_desc\" id=\"admin_aggr_desc\" class=\"sbkSaav_SubInput sbkAdmin_Focusable\">" + HttpUtility.HtmlEncode(enteredDescription) + "</textarea></td></tr>");

				// Add checkboxes for is active and is hidden
				Output.WriteLine("      <tr style=\"vertical-align:top\"><td><label for=\"admin_aggr_desc\">Behaviors:</label></td>");
				Output.WriteLine("        <td colspan=\"2\">");

				Output.WriteLine("        <div style=\"float:right\"><button title=\"Save new subcollection\" class=\"sbkAdm_RoundButton\" onclick=\"return save_new_child_aggr();\">ADD</button></div>");


				Output.WriteLine(enteredIsActive
								 ? "        <input class=\"sbkAsav_checkbox\" type=\"checkbox\" name=\"admin_aggr_isactive\" id=\"admin_aggr_isactive\" checked=\"checked\" /> <label for=\"admin_aggr_isactive\">Active?</label><br />"
					             : "        <input class=\"sbkAsav_checkbox\" type=\"checkbox\" name=\"admin_aggr_isactive\" id=\"admin_aggr_isactive\" /> <label for=\"admin_aggr_isactive\">Active?</label><br />");


				Output.WriteLine(enteredIsHidden
					             ? "          <input class=\"sbkAsav_checkbox\" type=\"checkbox\" name=\"admin_aggr_ishidden\" id=\"admin_aggr_ishidden\" checked=\"checked\" /> <label for=\"admin_aggr_ishidden\">Show in parent collection home page?</label>"
					             : "          <input class=\"sbkAsav_checkbox\" type=\"checkbox\" name=\"admin_aggr_ishidden\" id=\"admin_aggr_ishidden\" /> <label for=\"admin_aggr_ishidden\">Show in parent collection home page?</label>");


				Output.WriteLine("        </td>");
				Output.WriteLine("      </tr>");

				// Add the SAVE button
				Output.WriteLine("    </table>");
				Output.WriteLine("    </td>");
				Output.WriteLine("  </tr>");
				//Output.WriteLine("          <td><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + NEW_SUBCOLLECTION_HELP + "');\"  title=\"" + NEW_SUBCOLLECTION_HELP + "\" /></td>");
			}



			Output.WriteLine("</table>");
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
				string file = aggregationDirectory + "\\" + itemAggregation.CSS_File;
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
			Output.WriteLine("      <button title=\"Do not apply changes\" class=\"sbkAdm_RoundButton\" onclick=\"return new_aggr_edit_page('e');\"><img src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/button_previous_arrow.png\" class=\"sbkAdm_RoundButton_LeftImg\" alt=\"\" /> CANCEL</button> &nbsp; &nbsp; ");
			Output.WriteLine("      <button title=\"Save changes to this stylesheet\" class=\"sbkAdm_RoundButton\" onclick=\"return save_css_edits();\">SAVE <img src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/button_next_arrow.png\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button>");
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
			Item_Aggregation_Child_Page childPage = itemAggregation.Child_Page_By_Code(code);

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
						HTML_Based_Content htmlContent = new HTML_Based_Content();
						htmlContent.Static_Text = "<br /><br />This is a new " + Web_Language_Enum_Converter.Enum_To_Name(languageEnum) + " browse page.<br /><br />" + title + "<br /><br />The code for this browse is: " + childPage.Code;
						htmlContent.Author = RequestSpecificValues.Current_User.Full_Name;
						htmlContent.Date = DateTime.Now.ToLongDateString();
						htmlContent.Title = title;
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
			else if ((action.IndexOf("delete_") == 0) && ( action.Length > 7 ))
			{
				string delete_code = action.Substring(7);
				childPage.Remove_Language(Web_Language_Enum_Converter.Code_To_Enum(delete_code));

			}
		}

		private void Add_Child_Page(TextWriter Output)
		{
			const string VISIBILITY_HELP = "Existing child page visibility help placeholder";
			const string PARENT_HELP = "Existing child page parent help place holder";
			const string NEW_VERSION_LANGUAGE_HELP = "New version language help place holder";
			const string NEW_VERSION_TITLE_HELP = "New version title help place holder";
			const string NEW_VERSION_COPY_HELP = "New version copy help place holder";

			string code = RequestSpecificValues.Current_Mode.My_Sobek_SubMode.Substring(2);
			Item_Aggregation_Child_Page childPage = itemAggregation.Child_Page_By_Code(code);

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

			if (childPage.Browse_Type == Item_Aggregation_Child_Page.Visibility_Type.MAIN_MENU)
				Output.Write("<option value=\"browse\" selected=\"selected\">Main Menu</option>");
			else
				Output.Write("<option value=\"browse\">Main Menu</option>");

			if (childPage.Browse_Type == Item_Aggregation_Child_Page.Visibility_Type.METADATA_BROWSE_BY)
				Output.Write("<option value=\"browseby\" selected=\"selected\">Browse By</option>");
			else
				Output.Write("<option value=\"browseby\">Browse By</option>");

			if (childPage.Browse_Type == Item_Aggregation_Child_Page.Visibility_Type.NONE)
				Output.Write("<option value=\"none\" selected=\"selected\">None</option>");
			else
				Output.Write("<option value=\"none\">None</option>");

			Output.WriteLine("</select>");
			Output.WriteLine("        </td>");
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + VISIBILITY_HELP + "');\"  title=\"" + VISIBILITY_HELP + "\" /></td></tr></table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");



			// Put OTHER children in alphabetical order
			SortedList<string, Item_Aggregation_Child_Page> sortedChildren = new SortedList<string, Item_Aggregation_Child_Page>();
			foreach (Item_Aggregation_Child_Page childPage2 in itemAggregation.Child_Pages)
			{
				if (childPage2.Source == Item_Aggregation_Child_Page.Source_Type.Static_HTML)
				{
					sortedChildren.Add(childPage2.Code, childPage2);
				}
			}

			// Add line for parent code
			if (childPage.Browse_Type == Item_Aggregation_Child_Page.Visibility_Type.MAIN_MENU)
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
			foreach (Item_Aggregation_Child_Page childPage2 in sortedChildren.Values)
			{
				// Don't show itself in the possible parent list
				if (String.Compare(childPage.Code, childPage2.Code, StringComparison.OrdinalIgnoreCase) == 0)
					continue;

				// Only show main menu stuff
				if (childPage2.Browse_Type != Item_Aggregation_Child_Page.Visibility_Type.MAIN_MENU)
					continue;

				if (String.Compare(childPage.Parent_Code, childPage2.Code, StringComparison.OrdinalIgnoreCase) == 0)
					Output.Write("<option value=\"" + childPage2.Code + "\" selected=\"selected\">" + childPage.Code + "</option>");
				else
					Output.Write("<option value=\"" + childPage2.Code + "\">" + childPage.Code + "</option>");

			}
			Output.WriteLine("</select>");
			Output.WriteLine("        </td>");
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + PARENT_HELP + "');\"  title=\"" + PARENT_HELP + "\" /></td></tr></table>");
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
			if (childPage.Browse_Type == Item_Aggregation_Child_Page.Visibility_Type.METADATA_BROWSE_BY)
				RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Browse_By;
			RequestSpecificValues.Current_Mode.Info_Browse_Mode = childPage.Code;

			List<string> existing_languages = new List<string>();
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
			Output.WriteLine("          <td style=\"width:145px\"><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + NEW_VERSION_LANGUAGE_HELP + "');\"  title=\"" + NEW_VERSION_LANGUAGE_HELP + "\" /></td>");
			Output.WriteLine("          <td></td>");
			Output.WriteLine("        </tr>");

			Output.WriteLine("        <tr>");
			Output.WriteLine("          <td><label for=\"admin_aggr_new_version_label\">Title:</label></td>");
			Output.WriteLine("          <td colspan=\"2\"><input class=\"sbkSaav_medium_input sbkAdmin_Focusable\" name=\"admin_aggr_new_version_label\" id=\"admin_aggr_new_version_label\" type=\"text\" value=\"\" /></td>");
			Output.WriteLine("          <td><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + NEW_VERSION_TITLE_HELP + "');\"  title=\"" + NEW_VERSION_TITLE_HELP + "\" /></td>");
			Output.WriteLine("        </tr>");

			Output.WriteLine("        <tr>");

			Output.WriteLine("          <td><label for=\"admin_aggr_new_version_copy\">Copy from existing:</label></td>");
			Output.WriteLine("          <td>");
			Output.Write("            <select class=\"sbkSaav_SelectSingle\" id=\"admin_aggr_new_version_copy\" name=\"admin_aggr_new_version_copy\">");
			Output.Write("<option value=\"\" selected=\"selected\"></option>");
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

			Output.WriteLine("</select>");
			Output.WriteLine("          </td>");
			Output.WriteLine("          <td colspan=\"2\"><img class=\"sbkSaav_HelpButton\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + NEW_VERSION_COPY_HELP + "');\"  title=\"" + NEW_VERSION_COPY_HELP + "\" /></td>");
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
					add_upload_controls(MainPlaceHolder, ".gif", aggregationDirectory + "\\images\\buttons", "coll.gif", Tracer);
					break;

				case 5:
					add_upload_controls(MainPlaceHolder, ".gif,.bmp,.jpg,.png", aggregationDirectory + "\\images\\banners", String.Empty, Tracer);
					break;
			}
		}

		private void add_upload_controls(PlaceHolder UploadFilesPlaceHolder, string FileExtensions, string UploadDirectory, string ServerSideName, Custom_Tracer Tracer)
		{
			Tracer.Add_Trace("File_Managament_MySobekViewer.add_upload_controls", String.Empty);

			// Ensure the directory exists
			if (!File.Exists(UploadDirectory))
				Directory.CreateDirectory(UploadDirectory);

			StringBuilder filesBuilder = new StringBuilder(2000);

			LiteralControl filesLiteral2 = new LiteralControl(filesBuilder.ToString());
			UploadFilesPlaceHolder.Controls.Add(filesLiteral2);
			filesBuilder.Remove(0, filesBuilder.Length);

			UploadiFiveControl uploadControl = new UploadiFiveControl();
			uploadControl.UploadPath = UploadDirectory;
			uploadControl.UploadScript = RequestSpecificValues.Current_Mode.Base_URL + "UploadiFiveFileHandler.ashx";
			uploadControl.AllowedFileExtensions = FileExtensions;
			uploadControl.SubmitWhenQueueCompletes = true;
			uploadControl.RemoveCompleted = true;
			uploadControl.Multi = false;
			uploadControl.ServerSideFileName = "coll.gif";
			UploadFilesPlaceHolder.Controls.Add(uploadControl);

			LiteralControl literal1 = new LiteralControl(filesBuilder.ToString());
			UploadFilesPlaceHolder.Controls.Add(literal1);
		}

		#endregion


	}
}
