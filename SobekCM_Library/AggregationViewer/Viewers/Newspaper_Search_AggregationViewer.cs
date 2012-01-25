#region Using directives

using System;
using System.IO;
using SobekCM.Library.Aggregations;
using SobekCM.Library.Application_State;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.Navigation;

#endregion

namespace SobekCM.Library.AggregationViewer.Viewers
{
    /// <summary> Renders the newspaper-specific search / home page for a given item aggregation </summary>
    /// <remarks> This class implements the <see cref="iAggregationViewer"/> interface and extends the <see cref="abstractAggregationViewer"/> class.<br /><br />
    /// Collection viewers are used when displaying collection home pages, searches, browses, and information pages.<br /><br />
    /// During a valid html request to display the home page with newspaper-specific search, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="SobekCM_Navigation_Object"/> </li>
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
        /// <param name="Current_Aggregation"> Current item aggregation object </param>
        /// <param name="Current_Mode"> Mode / navigation information for the current request</param>
        public Newspaper_Search_AggregationViewer(Item_Aggregation Current_Aggregation, SobekCM_Navigation_Object Current_Mode): base(Current_Aggregation, Current_Mode)
        {
            // Determine the sub text to use
            const string subCode = "s=";

            // Save the search term
            if (currentMode.Search_String.Length > 0)
            {
                textBoxValue = currentMode.Search_String;
            }

            // Compute the redirect stem to use
            string fields = currentMode.Search_Fields;
            string search_string = currentMode.Search_String;
            currentMode.Search_String = String.Empty;
            currentMode.Search_Fields = String.Empty;
            currentMode.Home_Type = Home_Type_Enum.List;
            currentMode.Mode = Display_Mode_Enum.Results;
            currentMode.Search_Precision = Search_Precision_Type_Enum.Inflectional_Form;
            string redirect_stem = currentMode.Redirect_URL().Replace("&m=hhh", "").Replace("m=hht", "").Replace("&m=lhh", "").Replace("m=lht", "");
            currentMode.Mode = Display_Mode_Enum.Aggregation_Browse_Info;
            currentMode.Info_Browse_Mode = "all";
            browse_url = currentMode.Redirect_URL();
            currentMode.Search_String = search_string;
            currentMode.Search_Fields = fields;
            currentMode.Mode = Display_Mode_Enum.Aggregation_Home;
            currentMode.Info_Browse_Mode = String.Empty;

            // Write the advanced search box
            arg2 = String.Empty;
            arg1 = redirect_stem;

            if ((Current_Aggregation.Children_Count > 0) && (currentMode.Show_Selection_Panel))
            {
                scriptActionName = "newspaper_select_search_sobekcm('" + arg1 + "', '" + subCode + "', '" + browse_url + "');";
                arg2 = subCode;
                scriptIncludeName = "<script src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_search.js\" type=\"text/javascript\"></script>";
            }
            else
            {
                scriptActionName = "newspaper_search_sobekcm('" + arg1 + "');";
                scriptIncludeName = "<script src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_search.js\" type=\"text/javascript\"></script>";
            }
        }

        /// <summary> Gets the type of collection view or search supported by this collection viewer </summary>
        /// <value> This returns the <see cref="Item_Aggregation.CollectionViewsAndSearchesEnum.Newspaper_Search"/> enumerational value </value>
        public override Item_Aggregation.CollectionViewsAndSearchesEnum Type
        {
            get { return Item_Aggregation.CollectionViewsAndSearchesEnum.Newspaper_Search; }
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
            if (currentMode.Language == Language_Enum.Spanish)
            {
                search_language = "Búsqueda de la:";
                in_language = "en";
            }

            if (currentMode.Language == Language_Enum.French)
            {
                search_language = "Recherche de:";
                in_language = "en";
            }

            Output.WriteLine("  <table width=\"100%\" id=\"MetadataSearchPanel\" >");
            Output.WriteLine("    <tr align=\"right\">");
            Output.WriteLine("      <td align=\"right\" width=\"18%\"><b><label for=\"Textbox1\">" + search_language + "</label></b></td>");
            Output.WriteLine("      <td width=\"3%\">&nbsp;</td>");
            Output.Write("      <td width=\"46%\"><input name=\"Textbox1\" type=\"text\" id=\"Textbox1\" class=\"NewspaperSearchBox\" value=\"" + textBoxValue + "\" onfocus=\"textbox_enter('Textbox1', 'NewspaperSearchBox_focused');\" onblur=\"textbox_leave('Textbox1', 'NewspaperSearchBox');\" ");
            Output.Write(currentMode.Browser_Type.IndexOf("IE") >= 0 ? " onkeydown=\"fnTrapKD(event, 'newspaper', '" + arg1 + "', '" + arg2 +"','" + browse_url + "');\"" : "  onkeydown=\"return fnTrapKD(event, 'newspaper', '" + arg1 + "', '" + arg2 + "','" + browse_url + "');\"");
            Output.WriteLine(" /></td>");
            Output.WriteLine("      <td width=\"5%\" align=\"center\">" + in_language + "</td>");
            Output.WriteLine("      <td width=\"8%\">");
            Output.WriteLine("        <select name=\"Dropdownlist1\" id=\"Dropdownlist1\" style=\"width:128px;\" >");
            Output.WriteLine("          <option value=\"ZZ\" selected=\"selected\">Full Citation</option>");
            Output.WriteLine("          <option value=\"TX\">Full Text</option>");
            Output.WriteLine("          <option value=\"TI\">Newspaper Title</option>");
            Output.WriteLine("          <option value=\"PP\">Location</option>");
            Output.WriteLine("        </select>");
            Output.WriteLine("      </td>");
            Output.WriteLine("      <td align=\"left\"> &nbsp; <a onmousedown=\"" + scriptActionName + "\"><img name=\"jsbutton\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Skin + "/buttons/go_button.gif\" border=\"0\" alt=\"" + search_language + "\" /></a></td>");
            Output.WriteLine("      <td align=\"left\"><div id=\"circular_progress\" name=\"circular_progress\" class=\"hidden_progress\">&nbsp;</div></td>");     
            Output.WriteLine("    </tr>");
            Output.WriteLine("  </table>");

            Output.WriteLine();
            Output.WriteLine("<!-- Focus on search box -->");
            Output.WriteLine("<script type=\"text/javascript\">focus_element('Textbox1');</script>");
            Output.WriteLine();
        }
    }
}
