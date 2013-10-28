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
using SobekCM.Library.Aggregations;
using SobekCM.Library.Application_State;
using SobekCM.Library.Configuration;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.Navigation;
using SobekCM.Library.Search;
using SobekCM.Library.Settings;
using SobekCM.Library.Skins;
using SobekCM.Library.Users;
using darrenjohnstone.net.FileUpload;

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
		private readonly string actionMessage;
		private readonly string aggregationDirectory;
		private readonly Aggregation_Code_Manager codeManager;
		private readonly Item_Aggregation itemAggregation;
		private readonly List<Thematic_Heading> thematicHeadings;
		private readonly SobekCM_Skin_Collection webSkins;

		private DJAccessibleProgressBar djAccessibleProgrssBar1;
		private DJFileUpload djFileUpload1;
		private DJUploadController djUploadController1;

		private readonly int page;


		/// <summary> Constructor for a new instance of the Aggregation_Single_AdminViewer class </summary>
		/// <param name="User"> Authenticated user information </param>
		/// <param name="Current_Mode"> Mode / navigation information for the current request</param>
		/// <param name="Code_Manager"> List of valid collection codes, including mapping from the Sobek collections to Greenstone collections</param>
		/// <param name="Thematic_Headings"> Headings under which all the highlighted collections on the home page are organized </param>
		/// <param name="Web_Skin_Collection">  Contains the collection of all the default skins and the data to create any additional skins on request</param>
		/// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <remarks> Postback from handling an edit or new aggregation is handled here in the constructor </remarks>
		public Aggregation_Single_AdminViewer(User_Object User, SobekCM_Navigation_Object Current_Mode, Aggregation_Code_Manager Code_Manager, List<Thematic_Heading> Thematic_Headings, SobekCM_Skin_Collection Web_Skin_Collection, Custom_Tracer Tracer)
			: base(User)
		{
			Tracer.Add_Trace("Aggregation_Single_AdminViewer.Constructor", String.Empty);

			// Save the parameters
			thematicHeadings = Thematic_Headings;
			webSkins = Web_Skin_Collection;
			codeManager = Code_Manager;
			currentMode = Current_Mode;

			// Set some defaults
			actionMessage = String.Empty;

			// Get the code for the aggregation being edited
			string code = currentMode.Aggregation;

			// If the user cannot edit this, go back
			if (!user.Is_Aggregation_Curator(code))
			{
				Current_Mode.Mode = Display_Mode_Enum.My_Sobek;
				currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
				currentMode.Redirect();
				return;
			}

			// Load the item aggregation, either currenlty from the session (if already editing this aggregation )
			// or by reading all the appropriate XML and reading data from the database
			object possibleEditAggregation = HttpContext.Current.Session["Edit_Aggregation_" + code];
			Item_Aggregation cachedInstance = null;
			if (possibleEditAggregation != null)
				cachedInstance = (Item_Aggregation)possibleEditAggregation;

			itemAggregation = Item_Aggregation_Builder.Get_Item_Aggregation(code, String.Empty, cachedInstance, false, Tracer);
			
			// If unable to retrieve this aggregation, send to home
			if (itemAggregation == null)
			{
				currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
				currentMode.Redirect();
				return;
			}

			// Get the aggregation directory and ensure it exists
			aggregationDirectory = HttpContext.Current.Server.MapPath("design/aggregations/" + itemAggregation.Code );
			if (!Directory.Exists(aggregationDirectory))
				Directory.CreateDirectory(aggregationDirectory);

			// Determine the page
			page = 1;
			if (currentMode.My_Sobek_SubMode == "b" )
				page = 2;
			else if (currentMode.My_Sobek_SubMode == "c")
				page = 3;
			else if (currentMode.My_Sobek_SubMode == "d")
				page = 4;
			else if (currentMode.My_Sobek_SubMode == "e")
				page = 5;
			else if (currentMode.My_Sobek_SubMode == "f")
				page = 6;
			else if (currentMode.My_Sobek_SubMode == "g")
				page = 7;

			// If this is a postback, handle any events first
			if (currentMode.isPostBack)
			{
				try
				{
					// Pull the standard values
					NameValueCollection form = HttpContext.Current.Request.Form;

					// Get the curret action
					string action = form["admin_aggr_save"];

					// If this is to cancel, handle that here; no need to handle post-back from the
					// editing form page first
					if (action == "z")
					{
						// Clear the aggregation from the sessions
						HttpContext.Current.Session["Edit_Aggregation_" + itemAggregation.Code] = null;
						// Redirect the user
						currentMode.Mode = Display_Mode_Enum.Aggregation;
						currentMode.Aggregation_Type = Aggregation_Type_Enum.Home;
						currentMode.Redirect();
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
					}

					// Should this be saved to the database?
					if (action == "save")
					{
						// Save the new configuration file
						bool successful_save = (itemAggregation.Write_Configuration_File(SobekCM_Library_Settings.Base_Design_Location + itemAggregation.ObjDirectory));

						// Save to the database
						if (!itemAggregation.Save_To_Database(null))
							successful_save = false;

						// Save the link between this item and the thematic heading
						codeManager.Set_Aggregation_Thematic_Heading(itemAggregation.Code, itemAggregation.Thematic_Heading_ID);


						// Clear the aggregation from the cache
						MemoryMgmt.Cached_Data_Manager.Remove_Item_Aggregation(itemAggregation.Code, null);

						// Forward back to the aggregation home page, if this was successful
						if (successful_save)
						{
							// Clear the aggregation from the sessions
							HttpContext.Current.Session["Edit_Aggregation_" + itemAggregation.Code] = null;

							// Redirect the user
							currentMode.Mode = Display_Mode_Enum.Aggregation;
							currentMode.Aggregation_Type = Aggregation_Type_Enum.Home;
							currentMode.Redirect();
						}
						else
						{
							actionMessage = "Error saving aggregation information!";
						}
					}
					else
					{
						// Save to the admins session
						HttpContext.Current.Session["Edit_Aggregation_" + itemAggregation.Code] = itemAggregation;
						currentMode.My_Sobek_SubMode = action;
						HttpContext.Current.Response.Redirect(currentMode.Redirect_URL(), false);
						HttpContext.Current.ApplicationInstance.CompleteRequest();
						currentMode.Request_Completed = true;
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
			Output.WriteLine();

			Tracer.Add_Trace("Aggregation_Single_AdminViewer.Write_ItemNavForm_Closing", "Add the rest of the form");

			Output.WriteLine("<!-- Users_AdminViewer.Write_ItemNavForm_Closing -->");

			Output.WriteLine("<script src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_admin.js\" type=\"text/javascript\"></script>");
			Output.WriteLine("<script src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_form.js\" type=\"text/javascript\"></script>");
			Output.WriteLine();

			Output.WriteLine("<div id=\"sbkSaav_PageContainer\">");

			// Add the buttons
			string last_mode = currentMode.My_Sobek_SubMode;
			currentMode.My_Sobek_SubMode = String.Empty;
			Output.WriteLine("  <div class=\"sbkSaav_ButtonsDiv\">");
			Output.WriteLine("    <button title=\"Do not apply changes\" class=\"sbkAdm_RoundButton\" onclick=\"return new_aggr_edit_page('z');\"><img src=\"" + currentMode.Base_URL + "default/images/button_previous_arrow.png\" class=\"sbkAdm_RoundButton_LeftImg\" alt=\"\" /> CANCEL</button> &nbsp; &nbsp; ");
			Output.WriteLine("    <button title=\"Save changes to this item Aggregation\" class=\"sbkAdm_RoundButton\" onclick=\"return save_aggr_edits();\">SAVE <img src=\"" + currentMode.Base_URL + "default/images/button_next_arrow.png\" class=\"sbkAdm_RoundButton_RightImg\" alt=\"\" /></button>");
			Output.WriteLine("  </div>");
			Output.WriteLine();
			currentMode.My_Sobek_SubMode = last_mode;


			Output.WriteLine("  <div class=\"sbkSaav_HomeText\">");
			Output.WriteLine("  <br />");
			Output.WriteLine("  <h1>" + itemAggregation.Aggregation_Type + " Administration : " + itemAggregation.Code.ToUpper() + "</h1>");
			//Output.WriteLine("    <ul>");
			//Output.WriteLine("      <li>Enter the new values for this aggreagtion and press the SAVE button when all your edits are complete.</li>");
			//Output.WriteLine("      <li>For clarification of any terms on this form, <a href=\"" + SobekCM_Library_Settings.Help_URL(currentMode.Base_URL) + "adminhelp/singleaggr\" target=\"ADMIN_AGGR_HELP\" >click here to view the help page</a>.</li>");
			//Output.WriteLine("     </ul>");
			Output.WriteLine("  </div>");
			Output.WriteLine();



			// Add all the possible tabs
			Output.WriteLine("  <div id=\"tabContainer\" class=\"one\">");
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
				Output.WriteLine("    <li id=\"tabHeader_6\" onclick=\"return new_aggr_edit_page('g');\">" + SUBCOLLECTIONS + "</li>");
			}


			Output.WriteLine("      </ul>");
			Output.WriteLine("    </div>");

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
			}


			 Output.WriteLine("    </div>");
			 Output.WriteLine("  </div>");
			 Output.WriteLine("</div>");
			 Output.WriteLine("<br />");
		}

		#region Methods to render (and parse) page 1 - Basic Information

		private void Save_Page_1_Postback(NameValueCollection Form)
		{
			if (Form["admin_aggr_name"] != null) itemAggregation.Name = Form["admin_aggr_name"];
			if (Form["admin_aggr_shortname"] != null) itemAggregation.ShortName = Form["admin_aggr_shortname"];
			if (Form["admin_aggr_link"] != null) itemAggregation.External_Link = Form["admin_aggr_link"];
			if ( Form["admin_aggr_desc"] != null ) itemAggregation.Description = Form["admin_aggr_desc"];
			if (Form["admin_aggr_email"] != null) itemAggregation.Contact_Email = Form["admin_aggr_email"];
			itemAggregation.Is_Active = Form["admin_aggr_isactive"] != null;
			itemAggregation.Hidden = Form["admin_aggr_ishidden"] == null;
			if (Form["admin_aggr_heading"] != null)
				itemAggregation.Thematic_Heading_ID = Convert.ToInt32(Form["admin_aggr_heading"]);
		
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

		//	Output.WriteLine("  <tr class=\"sbkSaav_TextRow\"><td colspan=\"3\">For more information about the settings on this tab, <a href=\"" + SobekCM_Library_Settings.Help_URL(currentMode.Base_URL) + "adminhelp/singleaggr\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</td></tr>");

			Output.WriteLine("  <tr class=\"sbkSaav_TitleRow\"><td colspan=\"3\">Basic Information</td></tr>");
			Output.WriteLine("  <tr class=\"sbkSaav_TextRow\"><td colspan=\"3\"><p>The information in this section is the basic information about the aggregation, such as the full name, the shortened name used for breadcrumbs, the description, and the email contact.</p><p>For more information about the settings on this tab, <a href=\"" + SobekCM_Library_Settings.Help_URL(currentMode.Base_URL) + "adminhelp/singleaggr\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</p></td></tr>");

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
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + LONG_NAME_HELP + "');\"  title=\"" + LONG_NAME_HELP + "\" /></td></tr></table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");

			// Add the short name line
			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
			Output.WriteLine("    <td>&nbsp;</td>");
			Output.WriteLine("    <td class=\"sbkSaav_TableLabel\"><label for=\"admin_aggr_shortname\">Name (short):</label></td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td><input class=\"sbkSaav_medium_input sbkAdmin_Focusable\" name=\"admin_aggr_shortname\" id=\"admin_aggr_shortname\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(itemAggregation.ShortName) + "\" /></td>");
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + SHORT_NAME_HELP + "');\"  title=\"" + SHORT_NAME_HELP + "\" /></td></tr></table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");

			// Add the description box
			Output.WriteLine("  <tr class=\"sbkSaav_TallRow\">");
			Output.WriteLine("    <td>&nbsp;</td>");
			Output.WriteLine("    <td class=\"sbkSaav_TableLabel2\"><label for=\"admin_aggr_desc\">Description:</label></td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <table class=\"sbkSaav_InnerTable2\"><tr style=\"vertical-align:top\"><td><textarea class=\"sbkSaav_large_textbox sbkAdmin_Focusable\" rows=\"6\" name=\"admin_aggr_desc\" id=\"admin_aggr_desc\">" + HttpUtility.HtmlEncode(itemAggregation.Description) + "</textarea></td>");
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + DESCRIPTION_HELP + "');\"  title=\"" + DESCRIPTION_HELP + "\" /></td></tr></table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");

			// Add the email line
			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
			Output.WriteLine("    <td>&nbsp;</td>");
			Output.WriteLine("    <td class=\"sbkSaav_TableLabel\"><label for=\"admin_aggr_email\">Contact Email:</label></td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td><input class=\"sbkSaav_large_input sbkAdmin_Focusable\" name=\"admin_aggr_email\" id=\"admin_aggr_email\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(itemAggregation.Contact_Email) + "\" /></td>");
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + EMAIL_HELP + "');\"  title=\"" + EMAIL_HELP + "\" /></td></tr></table>");
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
				Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + LINK_HELP + "');\"  title=\"" + LINK_HELP + "\" /></td></tr></table>");
				Output.WriteLine("     </td>");
				Output.WriteLine("  </tr>");
			}

			Output.WriteLine("  <tr class=\"sbkSaav_TitleRow2\"><td colspan=\"3\">Collection Visibility</td></tr>");
			Output.WriteLine("  <tr class=\"sbkSaav_TextRow\"><td colspan=\"3\"><p>The values in this section determine if the collection is currently visible at all, whether it is eligible to appear on the collection list at the bottom of the parent page, and the collection button used in that case.  Thematic headings are used to place this collection on the main home page.</p><p>For more information about the settings on this tab, <a href=\"" + SobekCM_Library_Settings.Help_URL(currentMode.Base_URL) + "adminhelp/singleaggr\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</p></td></tr>");


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
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + ACTIVE_HELP + "');\"  title=\"" + ACTIVE_HELP + "\" /></td></tr></table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");

			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
			Output.WriteLine("    <td colspan=\"2\">&nbsp;</td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td>");
			Output.WriteLine(!itemAggregation.Hidden
						   ? "          <input class=\"sbkSaav_checkbox\" type=\"checkbox\" name=\"admin_aggr_ishidden\" id=\"admin_aggr_ishidden\" checked=\"checked\" /> <label for=\"admin_aggr_ishidden\">Show in parent collection home page?</label> "
						   : "          <input class=\"sbkSaav_checkbox\" type=\"checkbox\" name=\"admin_aggr_ishidden\" id=\"admin_aggr_ishidden\" /> <label for=\"admin_aggr_ishidden\">Show in parent collection home page?</label> ");
			Output.WriteLine("        </td>");
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + HIDDEN_HELP + "');\"  title=\"" + HIDDEN_HELP + "\" /></td></tr></table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");

			// Add the collection button
			Output.WriteLine("  <tr class=\"sbkSaav_ButtonRow\">");
			Output.WriteLine("    <td>&nbsp;</td>");
			Output.WriteLine("    <td class=\"sbkSaav_TableLabel\"><label for=\"admin_aggr_name\">Collection Button:</label></td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("       <img class=\"sbkSaav_ButtonImg\" src=\"" + currentMode.Base_Design_URL + "aggregations/" + itemAggregation.Code + "/images/buttons/coll.gif\" alt=\"NONE\" />");

			Output.WriteLine("       <table class=\"sbkSaav_InnerTable\">");
			Output.WriteLine("         <tr>");
			Output.WriteLine("           <td class=\"sbkSaav_UploadInstr\">To change, browse to a 50x50 pixel GIF file, and then select UPLOAD</td>");
			Output.WriteLine("           <td><img class=\"sbkSaav_HelpButton\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + COLLECTION_BUTTON_HELP + "');\"  title=\"" + COLLECTION_BUTTON_HELP + "\" /></td>");
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

			// Add the thematic heading line
			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
			Output.WriteLine("    <td>&nbsp;</td>");
			Output.WriteLine("    <td class=\"sbkSaav_TableLabel\"><label for=\"admin_aggr_heading\">Thematic Heading:</label></td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td>");
			Output.WriteLine("          <select class=\"sbkSaav_select_large\" name=\"admin_aggr_heading\" id=\"admin_aggr_heading\">");
			Output.WriteLine(itemAggregation.Thematic_Heading_ID < 0 ? "            <option value=\"-1\" selected=\"selected\" ></option>" : "            <option value=\"-1\"></option>");
			foreach (Thematic_Heading thisHeading in thematicHeadings)
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
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + THEMATIC_HELP + "');\"  title=\"" + THEMATIC_HELP + "\" /></td></tr></table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");

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
			if (Form["admin_aggr_advsearch"] != null) displayOptionsBldr.Append("A");
			if (Form["admin_aggr_advsearch_years"] != null) displayOptionsBldr.Append("Z");
			if (Form["admin_aggr_textsearch"] != null) displayOptionsBldr.Append("F");
			if (Form["admin_aggr_newspsearch"] != null) displayOptionsBldr.Append("N");
			if (Form["admin_aggr_dloctextsearch"] != null) displayOptionsBldr.Append("C");
			if (Form["admin_aggr_allitems"] != null) displayOptionsBldr.Append("I");
			if (Form["admin_aggr_mapsearch"] != null) displayOptionsBldr.Append("M");
			if (Form["admin_aggr_mapbrowse"] != null) displayOptionsBldr.Append("G");
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
			const string DLOC_SEARCH_HELP = "dLOC-Specific search help place holder";
			const string NEWSPAPER_SEARCH_HELP = "Newspaper search help place holder";
			const string ALL_ITEMS_HELP = "All and New Item browses help place holder";
			const string MAP_BROWSE_HELP = "Map browse help place holder";
			const string MAP_SEARCH_BOUNDING_HELP = "Map search starting bounding box help place holder";

			Output.WriteLine("<table class=\"sbkAdm_PopupTable\">");

			Output.WriteLine("  <tr class=\"sbkSaav_TitleRow\"><td colspan=\"3\">Search Options</td></tr>");
			Output.WriteLine("  <tr class=\"sbkSaav_TextRow\"><td colspan=\"3\"><p>These options control how searching works within this aggregation, such as which search options are made publicly available.</p><p>For more information about the settings on this tab, <a href=\"" + SobekCM_Library_Settings.Help_URL(currentMode.Base_URL) + "adminhelp/singleaggr\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</p></td></tr>");

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
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + BASIC_HELP + "');\"  title=\"" + BASIC_HELP + "\" /></td></tr></table>");
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
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + BASIC_YEARS_HELP + "');\"  title=\"" + BASIC_YEARS_HELP + "\" /></td></tr></table>");
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
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + ADVANCED_HELP + "');\"  title=\"" + ADVANCED_HELP + "\" /></td></tr></table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");

			// Add line for advanced saerch with year range
			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
			Output.WriteLine("    <td colspan=\"2\">&nbsp;</td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td>");
			Output.Write("          <input class=\"sbkSaav_checkbox\" type=\"checkbox\" name=\"admin_aggr_advsearch_years\" id=\"admin_aggr_advsearch_years\"");
			if (itemAggregation.Display_Options.IndexOf("Z") >= 0)
				Output.Write(" checked=\"checked\"");
			Output.WriteLine(" /> <label for=\"admin_aggr_advsearch_years\">Advanced Search (with Year Range)</label>");
			Output.WriteLine("        </td>");
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + ADVANCED_YEARS_HELP + "');\"  title=\"" + ADVANCED_YEARS_HELP + "\" /></td></tr></table>");
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
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + FULLTEXT_HELP + "');\"  title=\"" + FULLTEXT_HELP + "\" /></td></tr></table>");
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
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + NEWSPAPER_SEARCH_HELP + "');\"  title=\"" + NEWSPAPER_SEARCH_HELP + "\" /></td></tr></table>");
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
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + DLOC_SEARCH_HELP + "');\"  title=\"" + DLOC_SEARCH_HELP + "\" /></td></tr></table>");
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
			Output.WriteLine(" /> <label for=\"admin_aggr_mapsearch\">Map Search</label>");
			Output.WriteLine("        </td>");
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + MAP_SEARCH_HELP + "');\"  title=\"" + MAP_SEARCH_HELP + "\" /></td></tr></table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");


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
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + ALL_ITEMS_HELP + "');\"  title=\"" + ALL_ITEMS_HELP + "\" /></td></tr></table>");
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
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + MAP_BROWSE_HELP + "');\"  title=\"" + MAP_BROWSE_HELP + "\" /></td></tr></table>");
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
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + MAP_SEARCH_BOUNDING_HELP + "');\"  title=\"" + MAP_SEARCH_BOUNDING_HELP + "\" /></td></tr></table>");
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
			Output.WriteLine("  <tr class=\"sbkSaav_TextRow\"><td colspan=\"3\"><p>The information in this section controls how search results or item browses appears.  The facet options control which metadata values appear to the left of the results, to allow users to narrow their results.  The search results values determine which options are available for viewing the results and what are the aggregation defaults.  Finally, the result fields determines which values are displayed with each individual result in the result set.</p><p>For more information about the settings on this tab, <a href=\"" + SobekCM_Library_Settings.Help_URL(currentMode.Base_URL) + "adminhelp/singleaggr\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</p></td></tr>");

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
			Output.WriteLine("          <td><img class=\"sbkSaav_HelpButton\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + FACETS_HELP + "');\"  title=\"" + FACETS_HELP + "\" /></td>");
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
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + DEFAULT_VIEW_HELP + "');\"  title=\"" + DEFAULT_VIEW_HELP + "\" /></td></tr></table>");
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
			Output.WriteLine("          <td><img class=\"sbkSaav_HelpButton\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + RESULTS_VIEWS_HELP + "');\"  title=\"" + RESULTS_VIEWS_HELP + "\" /></td>");
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
			foreach (Metadata_Search_Field metadataField in SobekCM_Library_Settings.Metadata_Search_Fields )
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
			Output.WriteLine("</select>");
		}
		#endregion

		#region Methods to render (and parse) page 4 - Metdada (Metadata browses and OAI/PMH)

		private void Save_Page_4_Postback(NameValueCollection Form)
		{
			// Get the metadata browses
			List<Item_Aggregation_Browse_Info> metadata_browse_bys = new List<Item_Aggregation_Browse_Info>();
			foreach (Item_Aggregation_Browse_Info thisBrowse in itemAggregation.Browse_By_Pages(SobekCM_Library_Settings.Default_UI_Language))
			{
				if (thisBrowse.Browse_Type == Item_Aggregation_Browse_Info.Browse_Info_Type.Browse_By)
				{
					if (thisBrowse.Source == Item_Aggregation_Browse_Info.Source_Type.Database)
					{
						metadata_browse_bys.Add(thisBrowse);
					}
				}
			}

			// Remove all these browse by's
			foreach (Item_Aggregation_Browse_Info browseBy in metadata_browse_bys)
			{
				itemAggregation.Remove_Browse_Info_Page(browseBy);
			}

			// Look for the default browse by
			short default_browseby_id = 0;
			if (Form["admin_aggr_default_browseby"] != null)
			{
				string default_browseby = Form["admin_aggr_default_browseby"];
				if (Int16.TryParse(default_browseby, out default_browseby_id))
				{
					Metadata_Search_Field field = SobekCM_Library_Settings.Metadata_Search_Field_By_ID(default_browseby_id);
					if (field != null)
					{
						Item_Aggregation_Browse_Info newBrowse = new Item_Aggregation_Browse_Info(Item_Aggregation_Browse_Info.Browse_Info_Type.Browse_By, Item_Aggregation_Browse_Info.Source_Type.Database, field.Display_Term, String.Empty, field.Display_Term);
						itemAggregation.Add_Browse_Info(newBrowse);
						itemAggregation.Default_BrowseBy = field.Display_Term;
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
						Metadata_Search_Field field = SobekCM_Library_Settings.Metadata_Search_Field_By_ID(browseby_id);
						if (field != null)
						{
							Item_Aggregation_Browse_Info newBrowse = new Item_Aggregation_Browse_Info(Item_Aggregation_Browse_Info.Browse_Info_Type.Browse_By, Item_Aggregation_Browse_Info.Source_Type.Database, field.Display_Term, String.Empty, field.Display_Term);
							itemAggregation.Add_Browse_Info(newBrowse);
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
			foreach (Item_Aggregation_Browse_Info thisBrowse in itemAggregation.Browse_By_Pages(SobekCM_Library_Settings.Default_UI_Language))
			{
				if (thisBrowse.Browse_Type == Item_Aggregation_Browse_Info.Browse_Info_Type.Browse_By)
				{
					if (thisBrowse.Source == Item_Aggregation_Browse_Info.Source_Type.Database)
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
			Output.WriteLine("  <tr class=\"sbkSaav_TextRow\"><td colspan=\"3\"><p>The metadata browses can be used to expose all the metadata of the resources within this aggregation for public browsing.  Select the metadata fields you would like have available below.</p><p>For more information about the settings on this tab, <a href=\"" + SobekCM_Library_Settings.Help_URL(currentMode.Base_URL) + "adminhelp/singleaggr\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</p></td></tr>");

			// Add the default metadata browse view view
			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
			Output.WriteLine("    <td style=\"width:50px\">&nbsp;</td>");
			Output.WriteLine("    <td style=\"width:145px\" class=\"sbkSaav_TableLabel\"><label for=\"admin_aggr_default_browseby\">Default Browse:</label></td>");
			Output.WriteLine("    <td>");
			Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr><td>");
			BrowseBy_Writer_Helper(Output, "admin_aggr_default_browseby", "( NO DEFAULT )", default_browse_by, otherBrowseBys.ToArray(), "sbkSaav_SelectSingle");
			Output.WriteLine("        </td>");
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + DEFAULT_HELP + "');\"  title=\"" + DEFAULT_HELP + "\" /></td></tr></table>");
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
			Output.WriteLine("          <td><img class=\"sbkSaav_HelpButton\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + METADATA_BROWSES_HELP + "');\"  title=\"" + METADATA_BROWSES_HELP + "\" /></td>");
			Output.WriteLine("         </tr>");
			Output.WriteLine("       </table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");


			Output.WriteLine("  <tr class=\"sbkSaav_TitleRow2\"><td colspan=\"3\">OAI-PMH Settings</td></tr>");
			Output.WriteLine("  <tr class=\"sbkSaav_TextRow\"><td colspan=\"3\"><p>You can use OAI-PMH to expose all the metadata of the resources within this aggregation for automatic harvesting by other repositories.  Additionally, you can choose to attach metadata to the collection-level OAI-PMH record.  This should be coded as dublin core tags.</p><p>For more information about the settings on this tab, <a href=\"" + SobekCM_Library_Settings.Help_URL(currentMode.Base_URL) + "adminhelp/singleaggr\" target=\"ADMIN_USER_HELP\" >click here to view the help page</a>.</p></td></tr>");


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
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + OAI_FLAG_HELP + "');\"  title=\"" + OAI_FLAG_HELP + "\" /></td></tr></table>");
			Output.WriteLine("     </td>");
			Output.WriteLine("  </tr>");

			// Add label for adding metadata to this OAI-SET
			Output.WriteLine("  <tr class=\"sbkSaav_SingleRow\">");
			Output.WriteLine("    <td>&nbsp;</td>");
			Output.WriteLine("    <td class=\"sbkSaav_TableLabel\" colspan=\"2\">");
			Output.WriteLine("      <table class=\"sbkSaav_InnerTable\"><tr>");
			Output.WriteLine("        <td><label for=\"admin_aggr_oai_metadata\">Additional dublin core metadata to include in OAI-PMH set list:</label></td>");
			Output.WriteLine("        <td><img class=\"sbkSaav_HelpButton\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + OAI_METADATA_HELP + "');\"  title=\"" + OAI_METADATA_HELP + "\" /></td></tr></table>");
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
			foreach (Metadata_Search_Field metadataField in SobekCM_Library_Settings.Metadata_Search_Fields)
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
			Output.WriteLine("</select>");
		}

		#endregion

		#region Methods to render (and parse) page 5 -  Appearance

		private void Save_Page_5_Postback(NameValueCollection Form)
		{
			// Set the web skin
			itemAggregation.Web_Skins.Clear();
			itemAggregation.Default_Skin = String.Empty;
			if (( Form["admin_aggr_skin_1"] != null ) && ( Form["admin_aggr_skin_1"].Length > 0 ))
			{
				itemAggregation.Web_Skins.Add( Form["admin_aggr_skin_1"] );
				itemAggregation.Default_Skin = Form["admin_aggr_skin_1"];
			}

			// Get the front banner specificiations information
			if (Form["admin_aggr_frontbanner_height"] != null)
			{
				ushort front_banner_height;
				if (UInt16.TryParse(Form["admin_aggr_frontbanner_height"], out front_banner_height))
					itemAggregation.Front_Banner_Height = front_banner_height;
			}
			if (Form["admin_aggr_frontbanner_width"] != null)
			{
				ushort front_banner_width;
				if (UInt16.TryParse(Form["admin_aggr_frontbanner_width"], out front_banner_width))
					itemAggregation.Front_Banner_Width = front_banner_width;                        
			}
			if ((Form["admin_aggr_frontbanner_side"] != null) && (Form["admin_aggr_frontbanner_side"] == "left' "))
				itemAggregation.Front_Banner_Left_Side = true;
			else
				itemAggregation.Front_Banner_Left_Side = false;

			// Clear the front banners and banners
			int front_banner_dictionary_length = itemAggregation.Front_Banner_Dictionary.Count;
			int banner_dictionary_length = itemAggregation.Banner_Dictionary.Count;
		   itemAggregation.Clear_Banners();

			// Read the front banners
			for (int i = 0; i < front_banner_dictionary_length + 2; i++)
			{
				read_banner(Form, "admin_aggr_frontbanner_" + (i + 1).ToString(), true );
			}

			// Read the standard banners
			for (int i = 0; i < banner_dictionary_length + 2; i++)
			{
				read_banner(Form, "admin_aggr_banner_" + (i + 1).ToString(), false);
			}

			// Clear the home dictionary
			int home_page_length = itemAggregation.Home_Page_File_Dictionary.Count;
			itemAggregation.Home_Page_File_Dictionary.Clear();

			// Read the home page source information
			for ( int i = 0 ; i < home_page_length + 2 ; i++ )
			{
				read_home( Form, "admin_aggr_home_" + (i+1).ToString());
			}
		}

		private void read_home(NameValueCollection Form, string IDPrefix)
		{
			Web_Language_Enum language = SobekCM_Library_Settings.Default_UI_Language;
			string source = String.Empty;
			if (Form[IDPrefix + "_lang"] != null)
				language = Web_Language_Enum_Converter.Code_To_Enum(Form[IDPrefix + "_lang"]);
			if ( Form[IDPrefix + "_source"] != null )
				source = Form[IDPrefix + "_source"];
			if ( source.Length > 0 )
			{
				itemAggregation.Add_Home_Page_File( source, language );
			}
		}

		private void read_banner(NameValueCollection Form, string IDPrefix, bool FrontBanner)
		{
			Web_Language_Enum language = SobekCM_Library_Settings.Default_UI_Language;
			string source = String.Empty;
			if (Form[IDPrefix + "_lang"] != null)
				language = Web_Language_Enum_Converter.Code_To_Enum(Form[IDPrefix + "_lang"]);
			if ( Form[IDPrefix + "_source"] != null )
				source = Form[IDPrefix + "_source"];
			if ( source.Length > 0 )
			{
				if ( !FrontBanner )
				{
					itemAggregation.Add_Banner_Image( source, language);
				}
				else
				{
					itemAggregation.Add_Front_Banner_Image( source, language);
				}
			}
		}

		private void Add_Page_5(TextWriter Output)
		{
			Output.WriteLine("<table class=\"popup_table\">");

			// Get the ordered list of all skin codes
			ReadOnlyCollection<string> skinCodes = webSkins.Ordered_Skin_Codes;
		   
			// Add all the web skins
			Output.Write("<tr valign=\"top\"><td><div class=\"admin_aggr_label2\">Web Skin:</div></td><td>");
			for (int i = 0; i <1 ; i++ ) // itemAggregation.Web_Skins.Count + 5; i++)
			{
				string skin = String.Empty;
				if (i < itemAggregation.Web_Skins.Count)
					skin = itemAggregation.Web_Skins[i];
				Skin_Writer_Helper(Output, "admin_aggr_skin_" + (i+1).ToString(), skin, skinCodes );
				if ( (i+1) % 3 == 0 )
					Output.WriteLine("<br />");
			}
			Output.WriteLine("</td></tr>");

			// Add some space
			Output.WriteLine("<tr><td colspan=\"2\">&nbsp;</td></tr>");


			 // Add the banner height line
			Output.WriteLine("<tr><td><label for=\"admin_aggr_frontbanner_height\">Front Banner Height:</label></td><td><input class=\"admin_aggr_small_input\" name=\"admin_aggr_frontbanner_height\" id=\"admin_aggr_frontbanner_height\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(itemAggregation.Front_Banner_Height) + "\" onfocus=\"javascript:textbox_enter('admin_aggr_frontbanner_height', 'admin_aggr_small_input_focused')\" onblur=\"javascript:textbox_leave('admin_aggr_frontbanner_height', 'admin_aggr_small_input')\" /></td></tr>");

			// Add the banner width line
			Output.WriteLine("<tr><td><label for=\"admin_aggr_frontbanner_width\">Front Banner Width:</label></td><td><input class=\"admin_aggr_small_input\" name=\"admin_aggr_frontbanner_width\" id=\"admin_aggr_frontbanner_width\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(itemAggregation.Front_Banner_Width) + "\" onfocus=\"javascript:textbox_enter('admin_aggr_frontbanner_width', 'admin_aggr_small_input_focused')\" onblur=\"javascript:textbox_leave('admin_aggr_frontbanner_width', 'admin_aggr_small_input')\" /></td></tr>");

			// Add the radio buttons for the banner side (left or right)
			Output.WriteLine(" <tr><td>Front Banner Side:</td><td><input type=\"radio\" name=\"admin_aggr_frontbanner_side\" id=\"left\" value=\"left\"");
			if ( itemAggregation.Front_Banner_Left_Side )
				Output.Write(" checked=\"checked\"");
			Output.Write("/><label for=\"left\">Left</label> &nbsp; <input type=\"radio\" name=\"admin_aggr_frontbanner_side\" id=\"right\" value=\"right\"");
			if (! itemAggregation.Front_Banner_Left_Side )
				Output.Write(" checked=\"checked\"");
			Output.WriteLine("/><label for=\"right\">Right</label></td></tr>");

			// Add some space
			Output.WriteLine("<tr><td colspan=\"2\">&nbsp;</td></tr>");

			// Add the front banners
			Output.Write("<tr valign=\"top\"><td><div class=\"admin_aggr_label2\">Front Banners:</div></td><td>");
			for (int i = 0; i < itemAggregation.Front_Banner_Dictionary.Count + 2; i++)
			{
				// Find the language and banner source
				Web_Language_Enum language = Web_Language_Enum.DEFAULT; 
				string banner_source = String.Empty;
				if (i < itemAggregation.Front_Banner_Dictionary.Count)
				{
					language = itemAggregation.Front_Banner_Dictionary.Keys.ElementAt(i);
					banner_source = itemAggregation.Front_Banner_Dictionary[language];
				}
				else if (i == 0)
					language = SobekCM_Library_Settings.Default_UI_Language;

				// Add this banner
				Banner_Writer_Helper(Output, "admin_aggr_frontbanner_" + (i + 1).ToString(), language, banner_source);
			}
			Output.WriteLine("</td></tr>");

			// Add some space
			Output.WriteLine("<tr><td colspan=\"2\">&nbsp;</td></tr>");

			// Add the standard banners
			Output.Write("<tr valign=\"top\"><td><div class=\"admin_aggr_label2\">Banners:</div></td><td>");
			for (int i = 0; i < itemAggregation.Banner_Dictionary.Count + 2; i++)
			{
				// Find the language and banner source
				Web_Language_Enum language = Web_Language_Enum.DEFAULT;
				string banner_source = String.Empty;
				if (i < itemAggregation.Banner_Dictionary.Count)
				{
					language = itemAggregation.Banner_Dictionary.Keys.ElementAt(i);
					banner_source = itemAggregation.Banner_Dictionary[language];
				}
				else if (i == 0)
					language = SobekCM_Library_Settings.Default_UI_Language;

				// Add this banner
				Banner_Writer_Helper( Output, "admin_aggr_banner_" + ( i+1).ToString(), language, banner_source );
			}
			Output.WriteLine("</td></tr>");

			// Add some space
			Output.WriteLine("<tr><td colspan=\"2\">&nbsp;</td></tr>");

			// Add the home source files
			Output.Write("<tr valign=\"top\"><td><div class=\"admin_aggr_label2\">Home Page:</div></td><td>");
			for (int i = 0; i < itemAggregation.Home_Page_File_Dictionary.Count + 2; i++)
			{
				// Find the language and banner source
				Web_Language_Enum language = Web_Language_Enum.DEFAULT;
				string source_file = String.Empty;
				if (i < itemAggregation.Home_Page_File_Dictionary.Count)
				{
					language = itemAggregation.Home_Page_File_Dictionary.Keys.ElementAt(i);
					source_file = itemAggregation.Home_Page_File_Dictionary[language];
				}
				else if (i == 0)
					language = SobekCM_Library_Settings.Default_UI_Language;

				// Add this banner
				Home_Writer_Helper(Output, "admin_aggr_home_" + (i + 1).ToString(), language, source_file);
			}
			Output.WriteLine("</td></tr>");

			Output.WriteLine("</table>");
			Output.WriteLine("<br />");
		}


		private void Home_Writer_Helper(TextWriter Output, string HomeID, Web_Language_Enum Language, string Source)
		{
			string directory = (SobekCM_Library_Settings.Base_Design_Location + itemAggregation.ObjDirectory).ToLower().Replace("/", "\\");

			Output.Write("<select class=\"admin_aggr_select2\" name=\"" + HomeID + "_lang\" id=\"" + HomeID + "_lang\">");

			// Add the blank language, if this is a blank
			if (Language == Web_Language_Enum.DEFAULT)
				Output.Write("<option value=\"\" selected=\"selected\"></option>");

			// Add each language in the combo box
			foreach (string possible_language in Web_Language_Enum_Converter.Language_Name_Array)
			{
				if (Language == Web_Language_Enum_Converter.Code_To_Enum(possible_language))
				{
					Output.Write("<option value=\"" + possible_language + "\" selected=\"selected\" >" + HttpUtility.HtmlEncode(possible_language) + "</option>");
				}
				else
				{
					Output.Write("<option value=\"" + possible_language + "\">" + HttpUtility.HtmlEncode(possible_language) + "</option>");
				}
			}
			Output.WriteLine("</select> <br /> ");

			// Add the text to the text box
			if ((Source.ToLower().IndexOf("http://") < 0) && (Source.IndexOf("<%BASEURL%>") < 0))
			{
				Source = Source.ToLower().Replace("/", "\\").Replace(directory, "");
			}
			Output.Write("<input class=\"admin_aggr_large_input\" name=\"" + HomeID + "_source\" id=\"" + HomeID + "_source\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(Source) + "\" onfocus=\"javascript:textbox_enter('" + HomeID + "_source', 'admin_aggr_large_input_focused')\" onblur=\"javascript:textbox_leave('" + HomeID + "_source', 'admin_aggr_large_input')\" /><br /><br />");
		}

		private void Banner_Writer_Helper(TextWriter Output, string BannerID, Web_Language_Enum Language, string Source)
		{
			string directory = (SobekCM_Library_Settings.Base_Design_Location + itemAggregation.ObjDirectory).ToLower().Replace("/", "\\");

			Output.Write("<select class=\"admin_aggr_select2\" name=\"" + BannerID + "_lang\" id=\"" + BannerID + "_lang\">");

			// Add the blank language, if this is a blank
			if (Language == Web_Language_Enum.DEFAULT)
				Output.Write("<option value=\"\" selected=\"selected\"></option>");

			// Add each language in the combo box
			foreach (string possible_language in Web_Language_Enum_Converter.Language_Name_Array)
			{
				if (Language == Web_Language_Enum_Converter.Code_To_Enum(possible_language))
				{
					Output.Write("<option value=\"" + possible_language + "\" selected=\"selected\" >" + HttpUtility.HtmlEncode(possible_language) + "</option>");
				}
				else
				{
					Output.Write("<option value=\"" + possible_language + "\">" + HttpUtility.HtmlEncode(possible_language) + "</option>");
				}
			}
			Output.WriteLine("</select> <br /> ");

			// Add the text to the text box
			if ((Source.ToLower().IndexOf("http://") < 0) && (Source.IndexOf("<%BASEURL%>") < 0))
			{
				Source = Source.ToLower().Replace("/", "\\").Replace(directory, "");
			}
			Output.Write("<input class=\"admin_aggr_large_input\" name=\"" + BannerID + "_source\" id=\"" + BannerID + "_source\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(Source) + "\" onfocus=\"javascript:textbox_enter('" + BannerID + "_source', 'admin_aggr_large_input_focused')\" onblur=\"javascript:textbox_leave('" + BannerID + "_source', 'admin_aggr_large_input')\" /><br /><br />");
		}

		private void Skin_Writer_Helper(TextWriter Output, string SkinID, string Skin, IEnumerable<string> Skin_Codes )
		{
			// Start the select box
			Output.Write("<select class=\"admin_aggr_select\" name=\"" + SkinID + "\" id=\"" + SkinID + "\">");

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
			Output.Write("</select>");
		}

		#endregion

		#region Methods to render (and parse) page 6 - Static Pages

		private void Save_Page_6_Postback(NameValueCollection Form)
		{

		}

		private void Add_Page_6(TextWriter Output)
		{
			Output.WriteLine("<table class=\"popup_table\">");

			// Determine the maximum number of languages used in tooltips and text
			int max_labels = 0;
			int max_sources = 0;
			List<Item_Aggregation_Browse_Info> browse_infos = new List<Item_Aggregation_Browse_Info>();
			foreach (Item_Aggregation_Browse_Info thisBrowse in itemAggregation.Browse_Home_Pages(SobekCM_Library_Settings.Default_UI_Language))
			{
				if (thisBrowse.Source == Item_Aggregation_Browse_Info.Source_Type.Static_HTML)
				{
					max_labels = Math.Max(max_labels, thisBrowse.Label_Dictionary.Count);
					max_sources = Math.Max(max_sources, thisBrowse.Source_Dictionary.Count);
					browse_infos.Add(thisBrowse);
				}
			}
			foreach (Item_Aggregation_Browse_Info thisBrowse in itemAggregation.Browse_By_Pages(SobekCM_Library_Settings.Default_UI_Language))
			{
				if (thisBrowse.Source == Item_Aggregation_Browse_Info.Source_Type.Static_HTML)
				{
					max_labels = Math.Max(max_labels, thisBrowse.Label_Dictionary.Count);
					max_sources = Math.Max(max_sources, thisBrowse.Source_Dictionary.Count);
					browse_infos.Add(thisBrowse);
				}
			}
			foreach (Item_Aggregation_Browse_Info thisBrowse in itemAggregation.Info_Pages)
			{
				if (thisBrowse.Source == Item_Aggregation_Browse_Info.Source_Type.Static_HTML)
				{
					max_labels = Math.Max(max_labels, thisBrowse.Label_Dictionary.Count);
					max_sources = Math.Max(max_sources, thisBrowse.Source_Dictionary.Count);
					browse_infos.Add(thisBrowse);
				}
			}
			max_labels += 1;
			max_sources += 1;

			// Add each browse and info page
			for (int i = 0; i < browse_infos.Count + 1; i++)
			{
				if (i > 0)
				{
					// Add some space and a line
					Output.WriteLine("<tr><td colspan=\"2\">&nbsp;</td></tr>");
					Output.WriteLine("<tr style=\"background:#333333\"><td colspan=\"2\"></td></tr>");
					Output.WriteLine("<tr><td colspan=\"2\">&nbsp;</td></tr>");
				}

				// Get the info/browse object to display, or make an empty one
				Item_Aggregation_Browse_Info emptyBrowseInfo = new Item_Aggregation_Browse_Info {Source = Item_Aggregation_Browse_Info.Source_Type.Static_HTML, Browse_Type = Item_Aggregation_Browse_Info.Browse_Info_Type.Browse_Home};
				if (i < browse_infos.Count)
					emptyBrowseInfo = browse_infos[i];

				// Now, add it to the form
				Browse_Writer_Helper(Output, i + 1, emptyBrowseInfo, max_labels, max_sources);
			}

			Output.WriteLine("</table>");
			Output.WriteLine("<br />");
		}


		private void Browse_Writer_Helper(TextWriter Output, int BrowseCounter, Item_Aggregation_Browse_Info ThisBrowse, int Max_Labels, int Max_Sources)
		{
			string directory = (SobekCM_Library_Settings.Base_Design_Location + itemAggregation.ObjDirectory).ToLower().Replace("/", "\\");

			// Add the code line
			Output.WriteLine("<tr><td width=\"120px\"> &nbsp; &nbsp; <label for=\"admin_aggr_browse_code_" + BrowseCounter + "\">Code:</label></td><td><input class=\"admin_aggr_small_input\" name=\"admin_aggr_browse_code_" + BrowseCounter + "\" id=\"admin_aggr_browse_code_" + BrowseCounter + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(ThisBrowse.Code) + "\" onfocus=\"javascript:textbox_enter('admin_aggr_browse_code_" + BrowseCounter + "', 'admin_aggr_small_input_focused')\" onblur=\"javascript:textbox_leave('admin_aggr_browse_code_" + BrowseCounter + "', 'admin_aggr_small_input')\" /></td></tr>");

			// Add the type line
			Output.Write("<tr><td> &nbsp; &nbsp; <label for=\"admin_aggr_link_" + BrowseCounter + "\">Type:</label></td><td><select class=\"admin_aggr_select\" name=\"admin_aggr_browse_type_" + BrowseCounter + "\" id=\"admin_aggr_browse_type_" + BrowseCounter + "\">");
			Output.Write(ThisBrowse.Browse_Type == Item_Aggregation_Browse_Info.Browse_Info_Type.Browse_Home ? "<option value=\"browse\" selected=\"selected\">Browse</option>" : "<option value=\"browse\">Browse</option>");

			Output.Write(ThisBrowse.Browse_Type == Item_Aggregation_Browse_Info.Browse_Info_Type.Browse_By ? "<option value=\"browseby\" selected=\"selected\">Browse By</option>" : "<option value=\"browseby\">Browse By</option>");

			Output.Write(ThisBrowse.Browse_Type == Item_Aggregation_Browse_Info.Browse_Info_Type.Info ? "<option value=\"info\" selected=\"selected\">Info</option>" : "<option value=\"info\">Info</option>");

			Output.WriteLine("</td></tr>");

			// Add lines for the label
			Output.Write(Max_Labels == 1 ? "<tr><td> &nbsp; &nbsp; Label:</td><td>" : "<tr valign=\"top\"><td><br /> &nbsp; &nbsp; Label:</td><td>");
			for (int j = 0; j < Max_Labels; j++)
			{
				Web_Language_Enum language = SobekCM_Library_Settings.Default_UI_Language;
				string text = String.Empty;
				if (j < ThisBrowse.Label_Dictionary.Count)
				{
					language = ThisBrowse.Label_Dictionary.Keys.ElementAt(j);
					text = ThisBrowse.Label_Dictionary[language];
				}

				// Start the select box
				string id = "admin_aggr_label_lang_" + BrowseCounter + "_" + (j + 1).ToString();
				string id2 = "admin_aggr_label_" + BrowseCounter + "_" + (j + 1).ToString();
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

			// Add sources by language
			Output.Write(Max_Sources == 1 ? "<tr><td> &nbsp; &nbsp; HTML Source:</td><td>" : "<tr valign=\"top\"><td><br /> &nbsp; &nbsp; HTML Source:</td><td>");
			for (int j = 0; j < Max_Sources; j++)
			{
				Web_Language_Enum language = SobekCM_Library_Settings.Default_UI_Language;
				string text = String.Empty;
				if (j < ThisBrowse.Source_Dictionary.Count)
				{
					language = ThisBrowse.Source_Dictionary.Keys.ElementAt(j);
					text = ThisBrowse.Source_Dictionary[language];
				}

				// Start the select box
				string id = "admin_aggr_source_lang_" + BrowseCounter + "_" + (j + 1).ToString();
				string id2 = "admin_aggr_source_" + BrowseCounter + "_" + (j + 1).ToString();
				Output.Write("<hr /><select class=\"admin_aggr_select2\" name=\"" + id + "\" id=\"" + id + "\">");

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
				Output.WriteLine("</select> <br /> ");

				// Add the text to the text box
				if ((text.ToLower().IndexOf("http://") < 0) && (text.IndexOf("<%BASEURL%>") < 0))
				{

					text = text.ToLower().Replace("/", "\\").Replace(directory, "");
				}
				Output.Write("<input class=\"admin_aggr_large_input\" name=\"" + id2 + "\" id=\"" + id2 + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(text) + "\" onfocus=\"javascript:textbox_enter('" + id2 + "', 'admin_aggr_large_input_focused')\" onblur=\"javascript:textbox_leave('" + id2 + "', 'admin_aggr_large_input')\" /><br />");
			}
			Output.WriteLine("</td></tr>");

			// Add a delete option
			Output.WriteLine("<tr><td colspan=\"2\" align=\"right\"><a href=\"\">DELETE THIS PAGE</a></td></tr>");

		}

		#endregion

		#region Methods to render (and parse) page 7 - Metdata Browse

		private void Save_Page_7_Postback(NameValueCollection Form)
		{

		}

		private void Add_Page_7(TextWriter Output)
		{

		}



		#endregion

		#region Methods to render (and parse) page 8 -  Highlights

		private void Save_Page_8_Postback(NameValueCollection Form)
		{

		}

		private void Add_Page_8(TextWriter Output)
		{
			Output.WriteLine("<table class=\"popup_table\">");

			// Add the highlight type
			Output.Write("<tr><td width=\"120px\">Highlights Type:</td><td><input type=\"radio\" name=\"admin_aggr_highlight_type\" id=\"rotating\" value=\"rotating\"");
			if ( itemAggregation.Rotating_Highlights )
				Output.Write(" checked=\"checked\"");
			Output.Write("/><label for=\"rotating\">Rotating</label> &nbsp; <input type=\"radio\" name=\"admin_aggr_highlight_type\" id=\"static\" value=\"static\"");
			if (! itemAggregation.Rotating_Highlights )
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

		private void Highlight_Writer_Helper( TextWriter Output, int HighlightCounter, Item_Aggregation_Highlights Highlight, int Max_Text, int Max_Tooltips )
		{
			// Add the image line
			Output.WriteLine("<tr><td> &nbsp; &nbsp; <label for=\"admin_aggr_image_" + HighlightCounter + "\">Image:</label></td><td colspan=\"2\"><input class=\"admin_aggr_large_input\" name=\"admin_aggr_image_" + HighlightCounter + "\" id=\"admin_aggr_image_" + HighlightCounter + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(Highlight.Image) + "\" onfocus=\"javascript:textbox_enter('admin_aggr_image_" + HighlightCounter + "', 'admin_aggr_large_input_focused')\" onblur=\"javascript:textbox_leave('admin_aggr_image_" + HighlightCounter + "', 'admin_aggr_large_input')\" /></td></tr>");

			// Add the link line
			Output.WriteLine("<tr><td> &nbsp; &nbsp; <label for=\"admin_aggr_link_" + HighlightCounter + "\">Link:</label></td><td colspan=\"2\"><input class=\"admin_aggr_large_input\" name=\"admin_aggr_link_" + HighlightCounter + "\" id=\"admin_aggr_image_" + HighlightCounter + "\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(Highlight.Link) + "\" onfocus=\"javascript:textbox_enter('admin_aggr_image_" + HighlightCounter + "', 'admin_aggr_large_input_focused')\" onblur=\"javascript:textbox_leave('admin_aggr_link_" + HighlightCounter + "', 'admin_aggr_large_input')\" /></td></tr>");

			// Add lines for the text
			Output.Write(Max_Text == 1 ? "<tr><td> &nbsp; &nbsp; Text:</td><td>" : "<tr valign=\"top\"><td><br /> &nbsp; &nbsp; Text:</td><td>");
			for (int j = 0; j < Max_Text; j++)
			{
				Web_Language_Enum language = SobekCM_Library_Settings.Default_UI_Language;
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
				foreach (string possible_language in Web_Language_Enum_Converter.Language_Name_Array )
				{
						if ( language == Web_Language_Enum_Converter.Code_To_Enum( possible_language ))
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
				Web_Language_Enum language = SobekCM_Library_Settings.Default_UI_Language;
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
					add_upload_controls(MainPlaceHolder, ".gif", aggregationDirectory + "\\images\\buttons", Tracer);
					break;
			}
		}

		private void add_upload_controls(PlaceHolder UploadFilesPlaceHolder, string FileExtensions, string UploadDirectory, Custom_Tracer Tracer)
		{
			Tracer.Add_Trace("File_Managament_MySobekViewer.add_upload_controls", String.Empty);

			StringBuilder filesBuilder = new StringBuilder(2000);

			LiteralControl filesLiteral2 = new LiteralControl(filesBuilder.ToString());
			UploadFilesPlaceHolder.Controls.Add(filesLiteral2);
			filesBuilder.Remove(0, filesBuilder.Length);

			djUploadController1 = new DJUploadController
			{
				CSSPath = currentMode.Base_URL + "default/scripts/upload_styles",
				ImagePath = currentMode.Base_URL + "default/scripts/upload_images",
				ScriptPath = currentMode.Base_URL + "default/scripts/upload_scripts",
				AllowedFileExtensions = FileExtensions
			};
			UploadFilesPlaceHolder.Controls.Add(djUploadController1);

			djAccessibleProgrssBar1 = new DJAccessibleProgressBar();
			UploadFilesPlaceHolder.Controls.Add(djAccessibleProgrssBar1);

			djFileUpload1 = new DJFileUpload { ShowAddButton = false, ShowUploadButton = true, MaxFileUploads = 1, AllowedFileExtensions = ".jpg,.png,.gif,.bmp,.jpeg", GoButton_CSS = "sbkAdm_UploadButton" };
			UploadFilesPlaceHolder.Controls.Add(djFileUpload1);

			// Set the default processor
			FileSystemProcessor fs = new FileSystemProcessor { OutputPath = UploadDirectory };
			djUploadController1.DefaultFileProcessor = fs;

			// Change the file processor and set it's properties.
			FieldTestProcessor fsd = new FieldTestProcessor { OutputPath = UploadDirectory };
			djFileUpload1.FileProcessor = fsd;

			LiteralControl literal1 = new LiteralControl(filesBuilder.ToString());
			UploadFilesPlaceHolder.Controls.Add(literal1);
		}

		#endregion

	}
}
