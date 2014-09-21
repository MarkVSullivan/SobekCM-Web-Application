#region Using directives

using System;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using SobekCM.Library.Aggregations;
using SobekCM.Library.Configuration;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.Navigation;
using SobekCM.Library.Settings;
using SobekCM.Core.Users;
using SobekCM.Library.WebContent;
using SobekCM.Tools;
using SobekCM_UI_Library.Navigation;

#endregion

namespace SobekCM.Library.AggregationViewer.Viewers
{
    /// <summary> Renders the static information or browse pages for an item aggregation </summary>
    /// <remarks> This class implements the <see cref="iAggregationViewer"/> interface and extends the <see cref="abstractAggregationViewer"/> class.<br /><br />
    /// Aggregation viewers are used when displaying aggregation home pages, searches, browses, and information pages.<br /><br />
    /// During a valid html request to display a static browse or info page, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="Navigation.SobekCM_Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in this case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  For a collection-level request, an instance of the  <see cref="Aggregation_HtmlSubwriter"/> class is created. </li>
    /// <li>To display the requested collection view, the collection subwriter will creates an instance of this class </li>
    /// </ul></remarks>
    public class Static_Browse_Info_AggregationViewer : abstractAggregationViewer
    {
        private readonly Item_Aggregation_Child_Page browseObject;
        private readonly HTML_Based_Content staticWebContent;

	    /// <summary> Constructor for a new instance of the Static_Browse_Info_AggregationViewer class </summary>
	    /// <param name="Browse_Object"> Browse or information object to be displayed </param>
	    /// <param name="Static_Web_Content"> HTML content-based browse, info, or imple CMS-style web content objects.  These are objects which are read from a static HTML file and much of the head information must be maintained </param>
	    /// <param name="Current_Collection"> Current collection being displayed</param>
		/// <param name="Current_Mode"> Mode / navigation information for the current request</param>
	    /// <param name="Current_User"> Current user/session information </param>
	    public Static_Browse_Info_AggregationViewer(Item_Aggregation_Child_Page Browse_Object, HTML_Based_Content Static_Web_Content, Item_Aggregation Current_Collection, SobekCM_Navigation_Object Current_Mode, User_Object Current_User ):base(Current_Collection, Current_Mode )
        {
            browseObject = Browse_Object;
            staticWebContent = Static_Web_Content;
	        currentUser = Current_User;

			bool isAdmin = (currentUser != null) && (currentUser.Is_Aggregation_Admin(currentCollection.Code));
			if (( currentMode.Aggregation_Type == Aggregation_Type_Enum.Child_Page_Edit) && ( !isAdmin))
				currentMode.Aggregation_Type = Aggregation_Type_Enum.Browse_Info;

			NameValueCollection form = HttpContext.Current.Request.Form;
			if ((currentMode.Aggregation_Type == Aggregation_Type_Enum.Child_Page_Edit) && (form["sbkSbia_ChildTextEdit"] != null) && ( currentUser != null ))
			{
				string aggregation_folder = SobekCM_Library_Settings.Base_Design_Location + "aggregations\\" + currentCollection.Code + "\\";
				string file = aggregation_folder + browseObject.Get_Static_HTML_Source(currentMode.Language);

				// Get the header information as well
				if ( !String.IsNullOrEmpty(form["admin_childpage_title"]))
				{
					staticWebContent.Title = form["admin_childpage_title"];
				}
				if (form["admin_childpage_author"] != null)
					staticWebContent.Author = form["admin_childpage_author"];
				if (form["admin_childpage_date"] != null)
					staticWebContent.Date = form["admin_childpage_date"];
				if (form["admin_childpage_description"] != null)
					staticWebContent.Description = form["admin_childpage_description"];
				if (form["admin_childpage_keywords"] != null)
					staticWebContent.Keywords = form["admin_childpage_keywords"];
				if (form["admin_childpage_extrahead"] != null)
					staticWebContent.Extra_Head_Info = form["admin_childpage_extrahead"];


				// Make a backup from today, if none made yet
				if (File.Exists(file))
				{
					DateTime lastWrite = (new FileInfo(file)).LastWriteTime;
					string new_file = file.ToLower().Replace(".txt", "").Replace(".html", "").Replace(".htm", "") + lastWrite.Year + lastWrite.Month.ToString().PadLeft(2, '0') + lastWrite.Day.ToString() .PadLeft(2, '0')+ ".bak";
					if (File.Exists(new_file))
						File.Delete(new_file);
					File.Move(file, new_file);
				}


				// Assign the new text
				Static_Web_Content.Static_Text = form["sbkSbia_ChildTextEdit"];
				Static_Web_Content.Date = DateTime.Now.ToLongDateString();
				Static_Web_Content.Save_To_File(file);

				// Also save this change
				SobekCM_Database.Save_Item_Aggregation_Milestone(currentCollection.Code, "Child page '" + browseObject.Code + "' edited (" + Web_Language_Enum_Converter.Enum_To_Name(currentMode.Language) + ")", currentUser.Full_Name);

				// Forward along
				currentMode.Aggregation_Type = Aggregation_Type_Enum.Browse_Info;
				if ( Browse_Object.Browse_Type == Item_Aggregation_Child_Page.Visibility_Type.METADATA_BROWSE_BY )
					currentMode.Aggregation_Type = Aggregation_Type_Enum.Browse_By;
				
				string redirect_url = currentMode.Redirect_URL();
				if (redirect_url.IndexOf("?") > 0)
					redirect_url = redirect_url + "&refresh=always";
				else
					redirect_url = redirect_url + "?refresh=always";
				currentMode.Request_Completed = true;
				HttpContext.Current.Response.Redirect(redirect_url, false);
				HttpContext.Current.ApplicationInstance.CompleteRequest();
			}
        }

        /// <summary> Gets the type of collection view or search supported by this collection viewer </summary>
        /// <value> This returns the <see cref="Item_Aggregation.CollectionViewsAndSearchesEnum.Static_Browse_Info"/> enumerational value </value>
        public override Item_Aggregation.CollectionViewsAndSearchesEnum Type
        {
            get { return Item_Aggregation.CollectionViewsAndSearchesEnum.Static_Browse_Info; }
        }

        /// <summary> Gets flag which indicates whether to always use the home text as the secondary text </summary>
        public override bool Always_Display_Home_Text
        {
            get
            {
                return false;
            }
        }

        /// <summary>Flag indicates whether the subaggregation selection panel is displayed for this collection viewer</summary>
        /// <value> This property always returns the <see cref="Selection_Panel_Display_Enum.Never"/> enumerational value </value>
        public override Selection_Panel_Display_Enum Selection_Panel_Display
        {
            get
            {
                return Selection_Panel_Display_Enum.Never;
            }
        }

        /// <summary> Add the HTML to be displayed in the search box </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This adds the title of the static browse or info into the box </remarks>
        public override void Add_Search_Box_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Static_Browse_Info_AggregationViewer.Add_Search_Box_HTML", "Adding HTML");
            }

            Output.WriteLine("  <h1>" + browseObject.Get_Label(currentMode.Language) + "</h1>");
        }

        /// <summary> Add the HTML to be displayed below the search box </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This writes the HTML from the static browse or info page here  </remarks>
        public override void Add_Secondary_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Static_Browse_Info_AggregationViewer.Add_Secondary_HTML", "Adding HTML");
            }

			// Get the adjusted text for this user's session
			string static_browse_info_text = staticWebContent.Apply_Settings_To_Static_Text(staticWebContent.Static_Text, currentCollection, htmlSkin.Skin_Code, htmlSkin.Base_Skin_Code, currentMode.Base_URL, currentMode.URL_Options(), Tracer);
			bool isAdmin = (currentUser != null) && (currentUser.Is_Aggregation_Admin(currentCollection.Code));


			if ((currentMode.Aggregation_Type == Aggregation_Type_Enum.Child_Page_Edit) && (isAdmin))
			{
				const string TITLE_HELP = "Help for the title place holder";
				const string AUTHOR_HELP = "Help for the author place holder";
				const string DATE_HELP = "Help for the date place holder";
				const string DESCRIPTION_HELP = "Help for the description place holder";
				const string KEYWORDS_HELP = "Help for the keywords place holder";
				const string EXTRA_HEAD_HELP = "Help for the extra head place holder";


				string post_url = HttpUtility.HtmlEncode(HttpContext.Current.Items["Original_URL"].ToString());
				Output.WriteLine("<form name=\"home_edit_form\" method=\"post\" action=\"" + post_url + "\" id=\"addedForm\" >");

				Output.WriteLine("  <a href=\"\" onclick=\"return show_header_info()\" id=\"sbkSbia_HeaderInfoDivShowLink\">show header data (advanced)</a><br />");
				Output.WriteLine("  <div id=\"sbkSbia_HeaderInfoDiv\" style=\"display:none;\">");
				Output.WriteLine("    <div style=\"font-style:italic; padding:0 5px 5px 5px; text-align:left;\">The data below describes the content of this static child page and is used by some search engine indexing algorithms.  By default, it will not show in text of the page, but will be included in the head tag of the page.</div>");

				Output.WriteLine("    <table id=\"sbkSbia_HeaderTable\">");
				Output.WriteLine("      <tr>");
				Output.WriteLine("        <td style=\"width:50px\">&nbsp;</td>");
				Output.WriteLine("        <td class=\"sbkSbia_HeaderTableLabel\"><label for=\"admin_childpage_title\">Title:</label></td>");
				Output.WriteLine("        <td><input class=\"sbkSbia_HeaderInput sbk_Focusable\" name=\"admin_childpage_title\" id=\"admin_childpage_title\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(staticWebContent.Title) + "\" /></td>");
				Output.WriteLine("        <td><img class=\"sbkSbia_HelpButton\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + TITLE_HELP + "');\"  title=\"" + TITLE_HELP + "\" /></td>");
				Output.WriteLine("      </tr>");
				Output.WriteLine("      <tr>");
				Output.WriteLine("        <td>&nbsp;</td>");
				Output.WriteLine("        <td class=\"sbkSbia_HeaderTableLabel\"><label for=\"admin_childpage_author\">Author:</label></td>");
				Output.WriteLine("        <td><input class=\"sbkSbia_HeaderInput sbk_Focusable\" name=\"admin_childpage_author\" id=\"admin_childpage_author\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(staticWebContent.Author) + "\" /></td>");
				Output.WriteLine("        <td><img class=\"sbkSbia_HelpButton\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + AUTHOR_HELP + "');\"  title=\"" + AUTHOR_HELP + "\" /></td>");
				Output.WriteLine("      </tr>");
				Output.WriteLine("      <tr>");
				Output.WriteLine("        <td>&nbsp;</td>");
				Output.WriteLine("        <td class=\"sbkSbia_HeaderTableLabel\"><label for=\"admin_childpage_date\">Date:</label></td>");
				Output.WriteLine("        <td><input class=\"sbkSbia_HeaderInput sbk_Focusable\" name=\"admin_childpage_date\" id=\"admin_childpage_date\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(staticWebContent.Date) + "\" /></td>");
				Output.WriteLine("        <td><img class=\"sbkSbia_HelpButton\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + DATE_HELP + "');\"  title=\"" + DATE_HELP + "\" /></td>");
				Output.WriteLine("      </tr>");
				Output.WriteLine("      <tr>");
				Output.WriteLine("        <td>&nbsp;</td>");
				Output.WriteLine("        <td class=\"sbkSbia_HeaderTableLabel\"><label for=\"admin_childpage_description\">Description:</label></td>");
				Output.WriteLine("        <td><input class=\"sbkSbia_HeaderInput sbk_Focusable\" name=\"admin_childpage_description\" id=\"admin_childpage_description\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(staticWebContent.Description) + "\" /></td>");
				Output.WriteLine("        <td><img class=\"sbkSbia_HelpButton\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + DESCRIPTION_HELP + "');\"  title=\"" + DESCRIPTION_HELP + "\" /></td>");
				Output.WriteLine("      </tr>");
				Output.WriteLine("      <tr>");
				Output.WriteLine("        <td>&nbsp;</td>");
				Output.WriteLine("        <td class=\"sbkSbia_HeaderTableLabel\"><label for=\"admin_childpage_keywords\">Keywords:</label></td>");
				Output.WriteLine("        <td><input class=\"sbkSbia_HeaderInput sbk_Focusable\" name=\"admin_childpage_keywords\" id=\"admin_childpage_keywords\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(staticWebContent.Keywords) + "\" /></td>");
				Output.WriteLine("        <td><img class=\"sbkSbia_HelpButton\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + KEYWORDS_HELP + "');\"  title=\"" + KEYWORDS_HELP + "\" /></td>");
				Output.WriteLine("      </tr>");
				Output.WriteLine("      <tr style=\"vertical-align:top;\">");
				Output.WriteLine("        <td>&nbsp;</td>");
				Output.WriteLine("        <td class=\"sbkSbia_HeaderTableLabel\" style=\"padding-top:5px\"><label for=\"admin_childpage_extrahead\">HTML Head Info:</label></td>");
				Output.WriteLine("        <td><textarea rows=\"3\" class=\"sbkSbia_HeaderTextArea sbk_Focusable\" name=\"admin_childpage_extrahead\" id=\"admin_childpage_extrahead\" type=\"text\">" + HttpUtility.HtmlEncode(staticWebContent.Extra_Head_Info) + "</textarea></td>");
				Output.WriteLine("        <td><img class=\"sbkSbia_HelpButton\" src=\"" + currentMode.Base_URL + "default/images/help_button.jpg\" onclick=\"alert('" + EXTRA_HEAD_HELP + "');\"  title=\"" + EXTRA_HEAD_HELP + "\" /></td>");
				Output.WriteLine("      </tr>");
				Output.WriteLine("    </table>");
				Output.WriteLine("    <br />");
				Output.WriteLine("  </div>");



				Output.WriteLine("  <textarea id=\"sbkSbia_ChildTextEdit\" name=\"sbkSbia_ChildTextEdit\" >");
				Output.WriteLine(static_browse_info_text);
				Output.WriteLine("  </textarea>");
				Output.WriteLine();

				Output.WriteLine("  <div id=\"sbkSbia_TextEditButtons\">");
				currentMode.Aggregation_Type = Aggregation_Type_Enum.Browse_Info;
				if ( browseObject.Browse_Type == Item_Aggregation_Child_Page.Visibility_Type.METADATA_BROWSE_BY )
					currentMode.Aggregation_Type = Aggregation_Type_Enum.Browse_By;
				Output.WriteLine("    <button title=\"Do not apply changes\" class=\"roundbutton\" onclick=\"window.location.href='" + currentMode.Redirect_URL() + "';return false;\"><img src=\"" + currentMode.Base_URL + "default/images/button_previous_arrow.png\" class=\"roundbutton_img_left\" alt=\"\" /> CANCEL</button> &nbsp; &nbsp; ");
				Output.WriteLine("    <button title=\"Save changes to this child page text\" class=\"roundbutton\" type=\"submit\">SAVE <img src=\"" + currentMode.Base_URL + "default/images/button_next_arrow.png\" class=\"roundbutton_img_right\" alt=\"\" /></button>");
				Output.WriteLine("  </div>");
				Output.WriteLine("</form>");
				Output.WriteLine("<br /><br />");
				Output.WriteLine();
			}
			else
			{
				Aggregation_Type_Enum aggrType = currentMode.Aggregation_Type;

				// Output the adjusted home html
				if (isAdmin)
				{
					Output.WriteLine("<div id=\"sbkSbia_MainTextEditable\">");
					Output.WriteLine(static_browse_info_text);
					currentMode.Aggregation_Type = Aggregation_Type_Enum.Child_Page_Edit;
					Output.WriteLine("  <div id=\"sbkSbia_EditableTextLink\"><a href=\"" + currentMode.Redirect_URL() + "\" title=\"Edit this home text\"><img src=\"" + currentMode.Base_URL + "default/images/edit.gif\" alt=\"\" />edit content</a></div>");
					currentMode.Aggregation_Type = aggrType;
					Output.WriteLine("</div>");
					Output.WriteLine();

					Output.WriteLine("<script>");
					Output.WriteLine("  $(\"#sbkSbia_MainTextEditable\").mouseover(function() { $(\"#sbkSbia_EditableTextLink\").css(\"display\",\"inline-block\"); });");
					Output.WriteLine("  $(\"#sbkSbia_MainTextEditable\").mouseout(function() { $(\"#sbkSbia_EditableTextLink\").css(\"display\",\"none\"); });");
					Output.WriteLine("</script>");
					Output.WriteLine();

				}
				else
				{
					Output.WriteLine("<div id=\"sbkSbia_MainText\">");
					Output.WriteLine(static_browse_info_text);
					Output.WriteLine("</div>");
				}
			}

           
            Output.WriteLine("<br />");
            Output.WriteLine();
        }
    }
}
