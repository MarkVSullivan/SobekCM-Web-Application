#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using SobekCM.Core.Aggregations;
using SobekCM.Core.Configuration;
using SobekCM.Core.Navigation;
using SobekCM.Engine_Library.Navigation;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.AggregationViewer.Viewers
{
    /// <summary> Renders the dLOC search / home page for a given item aggregation </summary>
    /// <remarks> This class implements the <see cref="iAggregationViewer"/> interface and extends the <see cref="abstractAggregationViewer"/> class.<br /><br />
    /// Aggregation viewers are used when displaying aggregation home pages, searches, browses, and information pages.<br /><br />
    /// During a valid html request to display the home page with dLOC search, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in this case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  For a collection-level request, an instance of the  <see cref="Aggregation_HtmlSubwriter"/> class is created. </li>
    /// <li>To display the requested collection view, the collection subwriter will creates an instance of this class </li>
    /// </ul></remarks>
    public class dLOC_Search_AggregationViewer : abstractAggregationViewer
    {
        private readonly string arg1;
        private readonly string arg2;
        private readonly string browse_url;
        private readonly string textBoxValue;

        /// <summary> Constructor for a new instance of the dLOC_Search_AggregationViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        public dLOC_Search_AggregationViewer(RequestCache RequestSpecificValues) : base(RequestSpecificValues)
        {
            textBoxValue = String.Empty;
            if (RequestSpecificValues.Current_Mode.Search_String.Length > 0)
            {
                textBoxValue = RequestSpecificValues.Current_Mode.Search_String.Replace(",newspaper", "");
            }

            // Determine the complete script action name
            Display_Mode_Enum displayMode = RequestSpecificValues.Current_Mode.Mode;
	        Aggregation_Type_Enum aggrType = RequestSpecificValues.Current_Mode.Aggregation_Type;
            Search_Type_Enum searchType = RequestSpecificValues.Current_Mode.Search_Type;
            RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Results;
            RequestSpecificValues.Current_Mode.Search_Type = Search_Type_Enum.Full_Text;
            string search_string = RequestSpecificValues.Current_Mode.Search_String;
            RequestSpecificValues.Current_Mode.Search_String = String.Empty;
            RequestSpecificValues.Current_Mode.Search_Fields = String.Empty;
            arg2 = String.Empty;
            RequestSpecificValues.Current_Mode.Search_Precision = Search_Precision_Type_Enum.Inflectional_Form;
            arg1 = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
            RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Aggregation;
			RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Browse_Info;
            RequestSpecificValues.Current_Mode.Info_Browse_Mode = "all";
            browse_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);



            Search_Script_Action = "dloc_search_sobekcm('" + arg1 + "', '" + browse_url + "');";

            RequestSpecificValues.Current_Mode.Mode = displayMode;
	        RequestSpecificValues.Current_Mode.Aggregation_Type = aggrType;
            RequestSpecificValues.Current_Mode.Search_Type = searchType;
            RequestSpecificValues.Current_Mode.Search_String = search_string;
            RequestSpecificValues.Current_Mode.Info_Browse_Mode = String.Empty;
        }

        /// <summary> Gets the type of collection view or search supported by this collection viewer </summary>
        /// <value> This returns the <see cref="Item_Aggregation_Views_Searches_Enum.DLOC_FullText_Search"/> enumerational value </value>
        public override Item_Aggregation_Views_Searches_Enum Type
        {
            get { return Item_Aggregation_Views_Searches_Enum.DLOC_FullText_Search; }
        }

        /// <summary>Flag indicates whether the subaggregation selection panel is displayed for this collection viewer</summary>
        /// <value> This propoerty always returns the <see cref="Selection_Panel_Display_Enum.Never"/> enumerational value </value>
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
        public override void Add_Search_Box_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("dLOC_Search_AggregationViewer.Add_Search_Box_HTML", "Adding html for search box");
            }

            string search_collection = "Search full text";
            string include_newspaper = "Include newspapers?";

            if (RequestSpecificValues.Current_Mode.Language == Web_Language_Enum.Spanish)
            {
                search_collection = "Buscar en la colección";
                include_newspaper = "¿Incluir periódicos?";
            }

            if (RequestSpecificValues.Current_Mode.Language == Web_Language_Enum.French)
            {
                search_collection = "Recherche dans la collection";
                include_newspaper = "Accès aux journaux inclus?";
            }

			Output.WriteLine("  <table id=\"sbkDsav_SearchPanel\" >");
			Output.WriteLine("    <tr>");
			Output.WriteLine("      <td style=\"text-align:right;width:27%;\" id=\"sbkBsav_SearchPrompt\"><label for=\"SobekHomeSearchBox\">" + search_collection + ":</label></td>");
			Output.WriteLine("      <td style=\"width:3%;\">&nbsp;</td>");
			Output.WriteLine("      <td style=\"width:60%;\"><input name=\"u_search\" type=\"text\" class=\"sbkBsav_SearchBox sbk_Focusable\" id=\"SobekHomeSearchBox\" value=\"" + textBoxValue + "\" onkeydown=\"return fnTrapKD(event, 'dloc', '" + arg1 + "', '" + arg2 + "','" + browse_url + "');\" /></td>");
			Output.WriteLine("      <td style=\"width:10%;\"><button class=\"sbk_GoButton\" title=\"" + search_collection + "\" onclick=\"" + Search_Script_Action + ";return false;\">Go</button></td>");
			Output.WriteLine("      <td><div id=\"circular_progress\" name=\"circular_progress\" class=\"hidden_progress\">&nbsp;</div></td>");
			Output.WriteLine("    </tr>");
            Output.WriteLine("    <tr style=\"vertical-align:top\">");
            Output.WriteLine("      <td colspan=\"2\">&nbsp;</td>");
            Output.WriteLine("      <td colspan=\"3\">");
            Output.WriteLine("          &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; <input type=\"checkbox\" value=\"NEWSPAPER\" name=\"newscheck\" id=\"newscheck\" unchecked onclick=\"focus_element( 'SobekHomeSearchBox');\" /><label for=\"newscheck\">" + include_newspaper + "</label>");
            Output.WriteLine("      </td>");
            Output.WriteLine("    </tr>");
            Output.WriteLine("  </table>");

            Output.WriteLine();
            Output.WriteLine("<!-- Focus on search box -->");
            Output.WriteLine("<script type=\"text/javascript\">focus_element('SobekHomeSearchBox');</script>");
            Output.WriteLine();
        }

        /// <summary> Add the HTML to be displayed below the search box </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This adds the search tips by calling the base method <see cref="abstractAggregationViewer.Add_Simple_Search_Tips"/> </remarks>
        public override void Add_Secondary_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("dLOC_Search_AggregationViewer.Add_Secondary_HTML", "Adding simple search tips");
            }

            Add_Simple_Search_Tips(Output, Tracer);
        }
    }
}
