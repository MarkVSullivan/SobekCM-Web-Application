#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using SobekCM.Core.Aggregations;
using SobekCM.Core.Configuration;
using SobekCM.Core.Navigation;
using SobekCM.Engine_Library.Navigation;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.Settings;
using SobekCM.Library.UI;
using SobekCM.Tools;

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
	    /// <summary> Constructor for a new instance of the Static_Browse_Info_AggregationViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        public Static_Browse_Info_AggregationViewer(RequestCache RequestSpecificValues) : base(RequestSpecificValues)
        {
			bool isAdmin = (RequestSpecificValues.Current_User != null) && (RequestSpecificValues.Current_User.Is_Aggregation_Admin(RequestSpecificValues.Hierarchy_Object.Code));
			if (( RequestSpecificValues.Current_Mode.Aggregation_Type == Aggregation_Type_Enum.Child_Page_Edit) && ( !isAdmin))
				RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Browse_Info;

			NameValueCollection form = HttpContext.Current.Request.Form;
			if ((RequestSpecificValues.Current_Mode.Aggregation_Type == Aggregation_Type_Enum.Child_Page_Edit) && (form["sbkSbia_ChildTextEdit"] != null) && ( RequestSpecificValues.Current_User != null ))
			{
				string aggregation_folder = UI_ApplicationCache_Gateway.Settings.Base_Design_Location + "aggregations\\" + RequestSpecificValues.Hierarchy_Object.Code + "\\";

			    string file = aggregation_folder + RequestSpecificValues.Browse_Object.Source;

				// Get the header information as well
				if ( !String.IsNullOrEmpty(form["admin_childpage_title"]))
				{
					RequestSpecificValues.Static_Web_Content.Title = form["admin_childpage_title"];
				}
				if (form["admin_childpage_author"] != null)
					RequestSpecificValues.Static_Web_Content.Author = form["admin_childpage_author"];
				if (form["admin_childpage_date"] != null)
					RequestSpecificValues.Static_Web_Content.Date = form["admin_childpage_date"];
				if (form["admin_childpage_description"] != null)
					RequestSpecificValues.Static_Web_Content.Description = form["admin_childpage_description"];
				if (form["admin_childpage_keywords"] != null)
					RequestSpecificValues.Static_Web_Content.Keywords = form["admin_childpage_keywords"];
				if (form["admin_childpage_extrahead"] != null)
					RequestSpecificValues.Static_Web_Content.Extra_Head_Info = form["admin_childpage_extrahead"];


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
                RequestSpecificValues.Static_Web_Content.Content = form["sbkSbia_ChildTextEdit"].Replace("%]", "%>").Replace("[%", "<%");
                RequestSpecificValues.Static_Web_Content.Date = DateTime.Now.ToLongDateString();
                RequestSpecificValues.Static_Web_Content.Save_To_File(file);

				// Also save this change
				SobekCM_Database.Save_Item_Aggregation_Milestone(RequestSpecificValues.Hierarchy_Object.Code, "Child page '" + RequestSpecificValues.Browse_Object.Code + "' edited (" + Web_Language_Enum_Converter.Enum_To_Name(RequestSpecificValues.Current_Mode.Language) + ")", RequestSpecificValues.Current_User.Full_Name);

				// Forward along
				RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Browse_Info;
                if (RequestSpecificValues.Browse_Object.Browse_Type == Item_Aggregation_Child_Visibility_Enum.Metadata_Browse_By)
					RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Browse_By;
				
				string redirect_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
				if (redirect_url.IndexOf("?") > 0)
					redirect_url = redirect_url + "&refresh=always";
				else
					redirect_url = redirect_url + "?refresh=always";
				RequestSpecificValues.Current_Mode.Request_Completed = true;
				HttpContext.Current.Response.Redirect(redirect_url, false);
				HttpContext.Current.ApplicationInstance.CompleteRequest();
			}
        }

        /// <summary> Gets the type of collection view or search supported by this collection viewer </summary>
        /// <value> This returns the <see cref="Item_Aggregation_Views_Searches_Enum.Static_Browse_Info"/> enumerational value </value>
        public override Item_Aggregation_Views_Searches_Enum Type
        {
            get { return Item_Aggregation_Views_Searches_Enum.Static_Browse_Info; }
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

        /// <summary> Gets the collection of special behaviors which this aggregation viewer  requests from the main HTML subwriter. </summary>
        public override List<HtmlSubwriter_Behaviors_Enum> AggregationViewer_Behaviors
        {
            get
            {
                return new List<HtmlSubwriter_Behaviors_Enum>
                        {
                            HtmlSubwriter_Behaviors_Enum.Aggregation_Suppress_Home_Text
                        };
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

            Output.WriteLine("  <h1>" + RequestSpecificValues.Browse_Object.Label + "</h1>");
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
            if (RequestSpecificValues.Static_Web_Content == null)
                return;

            string static_browse_info_text = RequestSpecificValues.Static_Web_Content.Apply_Settings_To_Static_Text(RequestSpecificValues.Static_Web_Content.Content, RequestSpecificValues.Hierarchy_Object, RequestSpecificValues.HTML_Skin.Skin_Code, RequestSpecificValues.HTML_Skin.Base_Skin_Code, RequestSpecificValues.Current_Mode.Base_URL, UrlWriterHelper.URL_Options(RequestSpecificValues.Current_Mode), Tracer);
			bool isAdmin = (RequestSpecificValues.Current_User != null) && (RequestSpecificValues.Current_User.Is_Aggregation_Admin(RequestSpecificValues.Hierarchy_Object.Code));


			if ((RequestSpecificValues.Current_Mode.Aggregation_Type == Aggregation_Type_Enum.Child_Page_Edit) && (isAdmin))
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
				Output.WriteLine("        <td><input class=\"sbkSbia_HeaderInput sbk_Focusable\" name=\"admin_childpage_title\" id=\"admin_childpage_title\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(RequestSpecificValues.Static_Web_Content.Title) + "\" /></td>");
				Output.WriteLine("        <td><img class=\"sbkSbia_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + TITLE_HELP + "');\"  title=\"" + TITLE_HELP + "\" /></td>");
				Output.WriteLine("      </tr>");
				Output.WriteLine("      <tr>");
				Output.WriteLine("        <td>&nbsp;</td>");
				Output.WriteLine("        <td class=\"sbkSbia_HeaderTableLabel\"><label for=\"admin_childpage_author\">Author:</label></td>");
				Output.WriteLine("        <td><input class=\"sbkSbia_HeaderInput sbk_Focusable\" name=\"admin_childpage_author\" id=\"admin_childpage_author\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(RequestSpecificValues.Static_Web_Content.Author) + "\" /></td>");
				Output.WriteLine("        <td><img class=\"sbkSbia_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + AUTHOR_HELP + "');\"  title=\"" + AUTHOR_HELP + "\" /></td>");
				Output.WriteLine("      </tr>");
				Output.WriteLine("      <tr>");
				Output.WriteLine("        <td>&nbsp;</td>");
				Output.WriteLine("        <td class=\"sbkSbia_HeaderTableLabel\"><label for=\"admin_childpage_date\">Date:</label></td>");
				Output.WriteLine("        <td><input class=\"sbkSbia_HeaderInput sbk_Focusable\" name=\"admin_childpage_date\" id=\"admin_childpage_date\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(RequestSpecificValues.Static_Web_Content.Date) + "\" /></td>");
				Output.WriteLine("        <td><img class=\"sbkSbia_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + DATE_HELP + "');\"  title=\"" + DATE_HELP + "\" /></td>");
				Output.WriteLine("      </tr>");
				Output.WriteLine("      <tr>");
				Output.WriteLine("        <td>&nbsp;</td>");
				Output.WriteLine("        <td class=\"sbkSbia_HeaderTableLabel\"><label for=\"admin_childpage_description\">Description:</label></td>");
				Output.WriteLine("        <td><input class=\"sbkSbia_HeaderInput sbk_Focusable\" name=\"admin_childpage_description\" id=\"admin_childpage_description\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(RequestSpecificValues.Static_Web_Content.Description) + "\" /></td>");
				Output.WriteLine("        <td><img class=\"sbkSbia_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + DESCRIPTION_HELP + "');\"  title=\"" + DESCRIPTION_HELP + "\" /></td>");
				Output.WriteLine("      </tr>");
				Output.WriteLine("      <tr>");
				Output.WriteLine("        <td>&nbsp;</td>");
				Output.WriteLine("        <td class=\"sbkSbia_HeaderTableLabel\"><label for=\"admin_childpage_keywords\">Keywords:</label></td>");
				Output.WriteLine("        <td><input class=\"sbkSbia_HeaderInput sbk_Focusable\" name=\"admin_childpage_keywords\" id=\"admin_childpage_keywords\" type=\"text\" value=\"" + HttpUtility.HtmlEncode(RequestSpecificValues.Static_Web_Content.Keywords) + "\" /></td>");
				Output.WriteLine("        <td><img class=\"sbkSbia_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + KEYWORDS_HELP + "');\"  title=\"" + KEYWORDS_HELP + "\" /></td>");
				Output.WriteLine("      </tr>");
				Output.WriteLine("      <tr style=\"vertical-align:top;\">");
				Output.WriteLine("        <td>&nbsp;</td>");
				Output.WriteLine("        <td class=\"sbkSbia_HeaderTableLabel\" style=\"padding-top:5px\"><label for=\"admin_childpage_extrahead\">HTML Head Info:</label></td>");
                string extra_head_info = RequestSpecificValues.Static_Web_Content.Extra_Head_Info ?? String.Empty;
                Output.WriteLine("        <td><textarea rows=\"3\" class=\"sbkSbia_HeaderTextArea sbk_Focusable\" name=\"admin_childpage_extrahead\" id=\"admin_childpage_extrahead\" type=\"text\">" + HttpUtility.HtmlEncode(extra_head_info) + "</textarea></td>");
				Output.WriteLine("        <td><img class=\"sbkSbia_HelpButton\" src=\"" + Static_Resources.Help_Button_Jpg + "\" onclick=\"alert('" + EXTRA_HEAD_HELP + "');\"  title=\"" + EXTRA_HEAD_HELP + "\" /></td>");
				Output.WriteLine("      </tr>");
				Output.WriteLine("    </table>");
				Output.WriteLine("    <br />");
				Output.WriteLine("  </div>");


                Output.WriteLine("  <div id=\"sbkSbia_MainText\">");
				Output.WriteLine("  <textarea id=\"sbkSbia_ChildTextEdit\" name=\"sbkSbia_ChildTextEdit\" >");
				Output.WriteLine(static_browse_info_text.Replace("<%","[%").Replace("%>","%]"));
				Output.WriteLine("  </textarea>");
                Output.WriteLine("  </div>");
				Output.WriteLine();

				Output.WriteLine("  <div id=\"sbkSbia_TextEditButtons\">");
				RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Browse_Info;
				if ( RequestSpecificValues.Browse_Object.Browse_Type == Item_Aggregation_Child_Visibility_Enum.Metadata_Browse_By )
					RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Browse_By;
				Output.WriteLine("    <button title=\"Do not apply changes\" class=\"roundbutton\" onclick=\"window.location.href='" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "';return false;\"><img src=\"" + Static_Resources.Button_Previous_Arrow_Png + "\" class=\"roundbutton_img_left\" alt=\"\" /> CANCEL</button> &nbsp; &nbsp; ");
                Output.WriteLine("    <button title=\"Save changes to this child page text\" class=\"roundbutton\" type=\"submit\" onclick=\"CKEDITOR.instances.sbkSbia_ChildTextEdit.updateElement();\">SAVE <img src=\"" + Static_Resources.Button_Next_Arrow_Png + "\" class=\"roundbutton_img_right\" alt=\"\" /></button>");
				Output.WriteLine("  </div>");
				Output.WriteLine("</form>");
				Output.WriteLine("<br /><br />");
				Output.WriteLine();
			}
			else
			{
				Aggregation_Type_Enum aggrType = RequestSpecificValues.Current_Mode.Aggregation_Type;
			    Output.WriteLine("<section id=\"main-content\" role=\"main\">");

				// Output the adjusted home html
				if (isAdmin)
				{
					Output.WriteLine("<div id=\"sbkSbia_MainTextEditable\">");
					Output.WriteLine(static_browse_info_text);
					RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Child_Page_Edit;
                    Output.WriteLine("  <div id=\"sbkSbia_EditableTextLink\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\" title=\"Edit this page's text\"><img src=\"" + Static_Resources.Edit_Gif + "\" alt=\"\" />edit content</a></div>");
					RequestSpecificValues.Current_Mode.Aggregation_Type = aggrType;
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

			    Output.WriteLine("</section>");
			}

           
            Output.WriteLine("<br />");
            Output.WriteLine();
        }
    }
}
