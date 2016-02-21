using System;
using System.IO;
using SobekCM.Core.Aggregations;
using SobekCM.Core.Configuration.Localization;
using SobekCM.Core.Navigation;
using SobekCM.Tools;

namespace SobekCM.Library.AggregationViewer.Viewers
{
    /// <summary> Basic search option for an aggregation, that has a checkbox that determines if the full text
    /// is searched or not </summary>
    public class Basic_Text_Search_Combined_AggregationViewer : abstractAggregationViewer
    {
        private readonly string arg1;
        private readonly string arg2;
        private readonly string browse_url;
        private readonly string textBoxValue;

        /// <summary> Constructor for a new instance of the Basic_Text_Search_Combined_AggregationViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        /// <param name="ViewBag"> Aggregation-specific request information, such as aggregation object and any browse object requested </param>
        public Basic_Text_Search_Combined_AggregationViewer(RequestCache RequestSpecificValues, AggregationViewBag ViewBag)
            : base(RequestSpecificValues, ViewBag)
        {
            // Determine the sub text to use
            const string SUB_CODE = "s=";

            // Save the search term
            if (RequestSpecificValues.Current_Mode.Search_String.Length > 0)
            {
                textBoxValue = RequestSpecificValues.Current_Mode.Search_String;
            }

            // Determine the complete script action name
            Display_Mode_Enum displayMode = RequestSpecificValues.Current_Mode.Mode;
	        Aggregation_Type_Enum aggrType = RequestSpecificValues.Current_Mode.Aggregation_Type;
            Search_Type_Enum searchType = RequestSpecificValues.Current_Mode.Search_Type;
            RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Results;
            RequestSpecificValues.Current_Mode.Search_Type = Search_Type_Enum.Basic;
            RequestSpecificValues.Current_Mode.Search_Precision = Search_Precision_Type_Enum.Inflectional_Form;
            string search_string = RequestSpecificValues.Current_Mode.Search_String;
            RequestSpecificValues.Current_Mode.Search_String = String.Empty;
            RequestSpecificValues.Current_Mode.Search_Fields = String.Empty;
            arg2 = String.Empty;
            arg1 = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
            
            // Get the browse all url, if enabled
            browse_url = String.Empty;
            if (ViewBag.Hierarchy_Object.Can_Browse_Items)
            {
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Aggregation;
                RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Browse_Info;
                RequestSpecificValues.Current_Mode.Info_Browse_Mode = "all";
                browse_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
            }

            if ((!RequestSpecificValues.Current_Mode.Show_Selection_Panel.HasValue) || ( !RequestSpecificValues.Current_Mode.Show_Selection_Panel.Value ) || (ViewBag.Hierarchy_Object.Children_Count == 0))
            {
                Search_Script_Action = "basic_search_sobekcm('" + arg1 + "', '" + browse_url + "');";
            } 
            else
            {
                Search_Script_Action = "basic_select_search_sobekcm('" + arg1 + "', '" + SUB_CODE + "')";
                arg2 = SUB_CODE;
            }
            RequestSpecificValues.Current_Mode.Mode = displayMode;
            RequestSpecificValues.Current_Mode.Search_Type = searchType;
            RequestSpecificValues.Current_Mode.Search_String = search_string;
            RequestSpecificValues.Current_Mode.Info_Browse_Mode = String.Empty;
	        RequestSpecificValues.Current_Mode.Aggregation_Type = aggrType;
        }

        /// <summary> Gets the type of collection view or search supported by this collection viewer </summary>
        /// <value> This returns the <see cref="Item_Aggregation_Views_Searches_Enum.Basic_Search"/> enumerational value </value>
        public override Item_Aggregation_Views_Searches_Enum Type
        {
            get { return Item_Aggregation_Views_Searches_Enum.Basic_Search; }
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
                Tracer.Add_Trace("Basic_Search_AggregationViewer.Add_Search_Box_HTML", "Adding html for search box");
            }

            string search_collection = "Search Collection";
            const string INCLUDE_FULLTEXT = "Include the document text in search";
            const string INCLUDE_PRIVATES = "Include non-public items";
            if (RequestSpecificValues.Current_Mode.Language == Web_Language_Enum.Spanish)
            {
                search_collection = "Buscar en la colección";
            }

            if (RequestSpecificValues.Current_Mode.Language == Web_Language_Enum.French)
            {
                search_collection = "Recherche dans la collection";
            }

            Output.WriteLine("  <div id=\"sbkBsav_SearchPanel\" role=\"search\" >");
            Output.WriteLine("    <label for=\"SobekHomeSearchBox\" id=\"sbkBsav_SearchPrompt\">" + search_collection + ":</label>");
			Output.WriteLine("    <input name=\"u_search\" type=\"text\" class=\"sbkBsav_SearchBox sbk_Focusable\" id=\"SobekHomeSearchBox\" value=\"" + textBoxValue + "\" onkeydown=\"return fnTrapKD(event, 'basic', '" + arg1 + "', '" + arg2 + "','" + browse_url + "');\" />");
            Output.WriteLine("    <button id=\"sbkBsav_SearchButton\" class=\"sbk_GoButton\" title=\"" + search_collection + "\" onclick=\"" + Search_Script_Action + ";return false;\">Go</button>");
            Output.WriteLine("    <div id=\"circular_progress\" name=\"circular_progress\" class=\"hidden_progress\">&nbsp;</div>");

            Output.WriteLine("    <div id=\"sbkBsav_MimeType\"><input type=\"checkbox\" value=\"MIME_TYPE\" name=\"sbkBsav_fullTextCheck\" id=\"sbkBsav_fullTextCheck\" unchecked onclick=\"focus_element( 'SobekHomeSearchBox');\" /><label for=\"sbkBsav_fullTextCheck\">" + INCLUDE_FULLTEXT + "</label></div>");


            if (( RequestSpecificValues.Current_User != null ) && (RequestSpecificValues.Current_User.Is_System_Admin))
            {
                Output.WriteLine("    <div id=\"sbkBsav_PrivateCheck\"><input type=\"checkbox\" value=\"PRIVATE_ITEMS\" name=\"privatecheck\" id=\"privatecheck\" unchecked onclick=\"focus_element( 'SobekHomeSearchBox');\" /><label for=\"privatecheck\">" + INCLUDE_PRIVATES + "</label></div>");
            }

            Output.WriteLine("  </div>");

            Output.WriteLine();
            Output.WriteLine("<!-- Focus on search box -->");
            Output.WriteLine("<script type=\"text/javascript\">focus_element('SobekHomeSearchBox');</script>");
            Output.WriteLine();
        }
    }
}
