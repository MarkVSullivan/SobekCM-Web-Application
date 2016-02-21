#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Caching;
using SobekCM.Core.Aggregations;
using SobekCM.Core.Configuration.Localization;
using SobekCM.Core.Navigation;
using SobekCM.Engine_Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.AggregationViewer.Viewers
{
	/// <summary> Renders the basic search with year range searching / home page for a given item aggregation </summary>
	/// <remarks> This class implements the <see cref="iAggregationViewer"/> interface and extends the <see cref="abstractAggregationViewer"/> class.<br /><br />
	/// Aggregation viewers are used when displaying aggregation home pages, searches, browses, and information pages.<br /><br />
	/// During a valid html request to display the home page with basic search and year range, the following steps occur:
	/// <ul>
	/// <li>Application state is built/verified by the Application_State_Builder </li>
	/// <li>Request is analyzed by the QueryString_Analyzer and output as a <see cref="Navigation_Object"/> </li>
	/// <li>Main writer is created for rendering the output, in this case the <see cref="Html_MainWriter"/> </li>
	/// <li>The HTML writer will create the necessary subwriter.  For a collection-level request, an instance of the  <see cref="Aggregation_HtmlSubwriter"/> class is created. </li>
	/// <li>To display the requested collection view, the collection subwriter will creates an instance of this class </li>
	/// </ul></remarks>
	public class Basic_Search_YearRange_AggregationViewer: abstractAggregationViewer
    {
        private readonly string arg1;
        private readonly string arg2;
        private readonly string browse_url;
        private readonly string textBoxValue;

		/// <summary> Constructor for a new instance of the Basic_Search_YearRange_AggregationViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        /// <param name="ViewBag"> Aggregation-specific request information, such as aggregation object and any browse object requested </param>
        public Basic_Search_YearRange_AggregationViewer(RequestCache RequestSpecificValues, AggregationViewBag ViewBag)
            : base(RequestSpecificValues, ViewBag)
        {
            // Save the search term
            if (RequestSpecificValues.Current_Mode.Search_String.Length > 0)
            {
                textBoxValue = RequestSpecificValues.Current_Mode.Search_String;
            }

            // Determine the complete script action name
            Display_Mode_Enum displayMode = RequestSpecificValues.Current_Mode.Mode;
            Search_Type_Enum searchType = RequestSpecificValues.Current_Mode.Search_Type;
			Aggregation_Type_Enum aggrType = RequestSpecificValues.Current_Mode.Aggregation_Type;

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

		    Search_Script_Action = "basic_search_years_sobekcm('" + arg1 + "', '" + browse_url + "');";

            RequestSpecificValues.Current_Mode.Mode = displayMode;
			RequestSpecificValues.Current_Mode.Aggregation_Type = aggrType;
            RequestSpecificValues.Current_Mode.Search_Type = searchType;
            RequestSpecificValues.Current_Mode.Search_String = search_string;
            RequestSpecificValues.Current_Mode.Info_Browse_Mode = String.Empty;

			
        }

        /// <summary> Gets the type of collection view or search supported by this collection viewer </summary>
		/// <value> This returns the <see cref="Item_Aggregation_Views_Searches_Enum.Basic_Search_YearRange"/> enumerational value </value>
        public override Item_Aggregation_Views_Searches_Enum Type
        {
            get { return Item_Aggregation_Views_Searches_Enum.Basic_Search_YearRange; }
        }

        /// <summary>Flag indicates whether the subaggregation selection panel is displayed for this collection viewer</summary>
        /// <value> This property always returns the <see cref="Selection_Panel_Display_Enum.Selectable"/> enumerational value </value>
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
        public override void Add_Search_Box_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Basic_Search_AggregationViewer.Add_Search_Box_HTML", "Adding html for search box");
            }

			// Get the list of years for this aggregation
	        string aggrCode = ViewBag.Hierarchy_Object.Code.ToLower();
	        string key = aggrCode + "_YearRanges";
	        List<int> yearRange = HttpContext.Current.Cache[key] as List<int>;
			if (yearRange == null)
			{
				yearRange = new List<int>();
				List<string> yearRangeString = Engine_Database.Get_Item_Aggregation_Metadata_Browse(aggrCode, "Temporal Year", Tracer);
				foreach (string thisYear in yearRangeString)
				{
					int result;
					if ( Int32.TryParse(thisYear, out result))
						yearRange.Add(result);
				}
				HttpContext.Current.Cache.Insert(key, yearRange, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(5));
			}

            string search_collection = "Search Collection";
	        const string YEAR_RANGE = "Limit by Year";
            if (RequestSpecificValues.Current_Mode.Language == Web_Language_Enum.Spanish)
            {
                search_collection = "Buscar en la colección";
            }

            if (RequestSpecificValues.Current_Mode.Language == Web_Language_Enum.French)
            {
                search_collection = "Recherche dans la collection";
            }

			Output.WriteLine("  <table id=\"sbkBsav_SearchPanel_Years\" >");
			Output.WriteLine("    <tr>");
			Output.WriteLine("      <td style=\"text-align:right;width:27%;\" id=\"sbkBsav_SearchPrompt\"><label for=\"SobekHomeSearchBox\">" + search_collection + ":</label></td>");
			Output.WriteLine("      <td style=\"width:3%;\">&nbsp;</td>");
			Output.WriteLine("      <td style=\"width:60%;\"><input name=\"u_search\" type=\"text\" class=\"sbkBsav_SearchBox sbk_Focusable\" id=\"SobekHomeSearchBox\" value=\"" + textBoxValue + "\" onkeydown=\"return fnTrapKD(event, 'basicyears', '" + arg1 + "', '" + arg2 + "','" + browse_url + "');\" /></td>");
			Output.WriteLine("      <td style=\"width:10%;\"><button class=\"sbk_GoButton\" title=\"" + search_collection + "\" onclick=\"" + Search_Script_Action + ";return false;\">Go</button></td>");
			Output.WriteLine("      <td><div id=\"circular_progress\" name=\"circular_progress\" class=\"hidden_progress\">&nbsp;</div></td>");
			Output.WriteLine("    </tr>");

			// Show the year range data, if there are any years in this
	        if (yearRange.Count > 0)
	        {
		        Output.WriteLine("    <tr style=\"align:right; height:45px;\">");
		        Output.WriteLine("      <td align=\"right\" id=\"sbkBsav_SearchYearPrompt\">" + YEAR_RANGE + ":</td>");
		        Output.WriteLine("      <td>&nbsp;</td>");
		        Output.WriteLine("      <td style=\"text-align:left;\" colspan=\"3\">");

				Output.WriteLine("        <select name=\"YearDropDown1\" id=\"YearDropDown1\" class=\"sbkBsav_YearDropDown\">");
			//	Output.WriteLine("          <option value=\"ZZ\"> </option>");
                int currYear1 = RequestSpecificValues.Current_Mode.DateRange_Year1.HasValue ? RequestSpecificValues.Current_Mode.DateRange_Year1.Value : -1;
		        if (( currYear1 != -1 ) && ( !yearRange.Contains(currYear1)))
			        Output.WriteLine("          <option selected=\"selected\" value=\"" + currYear1 + "\">" + currYear1 + "</option>");
		        if (currYear1 == -1)
			        currYear1 = yearRange[0];
		        foreach (int thisYear in yearRange)
		        {
			        if (thisYear == currYear1)
			        {
						Output.WriteLine("          <option selected=\"selected\" value=\"" + thisYear + "\">" + thisYear + "</option>");
			        }
			        else
			        {
						Output.WriteLine("          <option value=\"" + thisYear + "\">" + thisYear + "</option>");
			        }
		        }
		        Output.WriteLine("        </select>");

				Output.WriteLine("&nbsp; through &nbsp;");

				Output.WriteLine("        <select name=\"YearDropDown2\" id=\"YearDropDown2\" class=\"sbkBsav_YearDropDown\">");
			//	Output.WriteLine("          <option value=\"ZZ\"> </option>");
                int currYear2 = RequestSpecificValues.Current_Mode.DateRange_Year2.HasValue ? RequestSpecificValues.Current_Mode.DateRange_Year2.Value : -1;
				if (( currYear2 != -1 ) && ( !yearRange.Contains(currYear2)))
					Output.WriteLine("          <option selected=\"selected\" value=\"" + currYear2 + "\">" + currYear2 + "</option>");
		        if (currYear2 == -1)
			        currYear2 = yearRange[yearRange.Count - 1];
				foreach (int thisYear in yearRange)
				{
					if (thisYear == currYear2)
					{
						Output.WriteLine("          <option selected=\"selected\" value=\"" + thisYear + "\">" + thisYear + "</option>");
					}
					else
					{
						Output.WriteLine("          <option value=\"" + thisYear + "\">" + thisYear + "</option>");
					}
				}
				Output.WriteLine("        </select>");

		        Output.WriteLine("      </td>");
		        Output.WriteLine("    </tr>");
	        }

			//if (( currentUser != null ) && (currentUser.Is_System_Admin))
			//{
			//	Output.WriteLine("    <tr align=\"right\">");
			//	Output.WriteLine("      <td>&nbsp;</td>");
			//	Output.WriteLine("      <td align=\"left\" colspan=\"4\">");
			//	Output.WriteLine("          &nbsp; &nbsp; &nbsp; &nbsp; <input type=\"checkbox\" value=\"PRIVATE_ITEMS\" name=\"privatecheck\" id=\"privatecheck\" unchecked onclick=\"focus_element( 'SobekHomeSearchBox');\" /><label for=\"privatecheck\">" + INCLUDE_PRIVATES + "</label>");
			//	Output.WriteLine("      </td>");
			//	Output.WriteLine("    </tr>");
			//}

            Output.WriteLine("  </table>");
            Output.WriteLine("<br />");
            Output.WriteLine();
            Output.WriteLine("<!-- Focus on search box -->");
            Output.WriteLine("<script type=\"text/javascript\">focus_element('SobekHomeSearchBox');</script>");
            Output.WriteLine();
        }
    }
}
