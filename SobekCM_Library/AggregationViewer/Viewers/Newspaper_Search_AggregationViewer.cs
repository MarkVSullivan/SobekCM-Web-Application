#region Using directives

using System;
using System.IO;
using SobekCM.Core.Aggregations;
using SobekCM.Core.Configuration;
using SobekCM.Core.Navigation;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.AggregationViewer.Viewers
{
    /// <summary> Renders the newspaper-specific search / home page for a given item aggregation </summary>
    /// <remarks> This class implements the <see cref="iAggregationViewer"/> interface and extends the <see cref="abstractAggregationViewer"/> class.<br /><br />
    /// Collection viewers are used when displaying collection home pages, searches, browses, and information pages.<br /><br />
    /// During a valid html request to display the home page with newspaper-specific search, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in this case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  For a collection-level request, an instance of the  <see cref="Aggregation_HtmlSubwriter"/> class is created. </li>
    /// <li>To display the requested collection view, the collection subwriter will creates an instance of this class </li>
    /// </ul></remarks>
    public class Newspaper_Search_AggregationViewer : abstractAggregationViewer
    {
        private readonly string arg1;
        private readonly string arg2;
        private readonly string browse_url;
        private readonly string textBoxValue;

        /// <summary> Constructor for a new instance of the Newspaper_Search_AggregationViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        public Newspaper_Search_AggregationViewer(RequestCache RequestSpecificValues) : base(RequestSpecificValues)
        {
            // Determine the sub text to use
            const string SUB_CODE = "s=";

            // Save the search term
            if (RequestSpecificValues.Current_Mode.Search_String.Length > 0)
            {
                textBoxValue = RequestSpecificValues.Current_Mode.Search_String;
            }

            // Compute the redirect stem to use
            string fields = RequestSpecificValues.Current_Mode.Search_Fields;
            string search_string = RequestSpecificValues.Current_Mode.Search_String;
			Aggregation_Type_Enum aggrType = RequestSpecificValues.Current_Mode.Aggregation_Type;
            RequestSpecificValues.Current_Mode.Search_String = String.Empty;
            RequestSpecificValues.Current_Mode.Search_Fields = String.Empty;
            RequestSpecificValues.Current_Mode.Home_Type = Home_Type_Enum.List;
            RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Results;
            RequestSpecificValues.Current_Mode.Search_Precision = Search_Precision_Type_Enum.Inflectional_Form;
            string redirect_stem = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode).Replace("&m=hhh", "").Replace("m=hht", "").Replace("&m=lhh", "").Replace("m=lht", "");
			RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Aggregation;
            RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Browse_Info;
            RequestSpecificValues.Current_Mode.Info_Browse_Mode = "all";
            browse_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
            RequestSpecificValues.Current_Mode.Search_String = search_string;
            RequestSpecificValues.Current_Mode.Search_Fields = fields;
	        RequestSpecificValues.Current_Mode.Aggregation_Type = aggrType;
            RequestSpecificValues.Current_Mode.Info_Browse_Mode = String.Empty;

            // Write the advanced search box
            arg2 = String.Empty;
            arg1 = redirect_stem;

            if ((RequestSpecificValues.Hierarchy_Object.Children_Count > 0) && ((RequestSpecificValues.Current_Mode.Show_Selection_Panel.HasValue) && (RequestSpecificValues.Current_Mode.Show_Selection_Panel.Value )))
            {
                Search_Script_Action = "newspaper_select_search_sobekcm('" + arg1 + "', '" + SUB_CODE + "', '" + browse_url + "');";
                arg2 = SUB_CODE;
            }
            else
            {
                Search_Script_Action = "newspaper_search_sobekcm('" + arg1 + "');";
            }
        }

        /// <summary> Gets the type of collection view or search supported by this collection viewer </summary>
        /// <value> This returns the <see cref="Item_Aggregation_Views_Searches_Enum.Newspaper_Search"/> enumerational value </value>
        public override Item_Aggregation_Views_Searches_Enum Type
        {
            get { return Item_Aggregation_Views_Searches_Enum.Newspaper_Search; }
        }

        /// <summary>Flag indicates whether the subaggregation selection panel is displayed for this collection viewer</summary>
        /// <value> This property always returns the <see cref="Selection_Panel_Display_Enum.Selectable"/> enumerational value </value>
        public override Selection_Panel_Display_Enum Selection_Panel_Display
        {
            get
            {
                return Selection_Panel_Display_Enum.Selectable;
            }
        }

        /// <summary> Add the HTML to be displayed in the search box </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Add_Search_Box_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Newspaper_Search_AggregationViewer.Add_Search_Box_HTML", "Adding html for search box");
            }

            string search_language = "Search for:";
            string in_language = "in";
            if (RequestSpecificValues.Current_Mode.Language == Web_Language_Enum.Spanish)
            {
                search_language = "Búsqueda de la:";
                in_language = "en";
            }

            if (RequestSpecificValues.Current_Mode.Language == Web_Language_Enum.French)
            {
                search_language = "Recherche de:";
                in_language = "en";
            }

			Output.WriteLine("  <table id=\"sbkNsav_SearchPanel\" >");
            Output.WriteLine("    <tr>");
			Output.WriteLine("      <td style=\"text-align:right;width:18%;\" id=\"sbkBsav_SearchPrompt\"><label for=\"Textbox1\">" + search_language + "</label></td>");
            Output.WriteLine("      <td style=\"width:3%;\">&nbsp;</td>");
			Output.WriteLine("      <td style=\"width:46%;\"><input name=\"Textbox1\" type=\"text\" id=\"Textbox1\" class=\"sbkNsav_SearchBox sbk_Focusable\" value=\"" + textBoxValue + "\" onkeydown=\"fnTrapKD(event, 'newspaper', '" + arg1 + "', '" + arg2 +"','" + browse_url + "');\" /></td>");
            Output.WriteLine("      <td style=\"width:5%;text-align:center;\">" + in_language + "</td>");
            Output.WriteLine("      <td style=\"width:8%;\">");
			Output.WriteLine("        <select name=\"Dropdownlist1\" id=\"Dropdownlist1\" class=\"sbkNsav_DropDownList\" >");
            Output.WriteLine("          <option value=\"ZZ\" selected=\"selected\">Full Citation</option>");
            Output.WriteLine("          <option value=\"TX\">Full Text</option>");
            Output.WriteLine("          <option value=\"TI\">Newspaper Title</option>");
            Output.WriteLine("          <option value=\"PP\">Location</option>");
            Output.WriteLine("        </select>");
            Output.WriteLine("      </td>");
			Output.WriteLine("      <td> &nbsp; <button class=\"sbk_GoButton\" onclick=\"" + Search_Script_Action + ";return false;\">Go</button></td>");
            Output.WriteLine("      <td><div id=\"circular_progress\" name=\"circular_progress\" class=\"hidden_progress\">&nbsp;</div></td>");     
            Output.WriteLine("    </tr>");
            Output.WriteLine("  </table>");

            Output.WriteLine();
            Output.WriteLine("<!-- Focus on search box -->");
            Output.WriteLine("<script type=\"text/javascript\">focus_element('Textbox1');</script>");
            Output.WriteLine();
        }
    }
}
