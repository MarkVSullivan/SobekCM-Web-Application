#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;
using SobekCM.Library.Aggregations;
using SobekCM.Library.Configuration;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.Navigation;
using SobekCM.Library.Search;
using SobekCM.Library.Settings;

#endregion

namespace SobekCM.Library.AggregationViewer.Viewers
{
	/// <summary> Renders the advanced search with year range options for a given item aggregation </summary>
	/// <remarks> This class implements the <see cref="iAggregationViewer"/> interface and extends the <see cref="abstractAggregationViewer"/> class.<br /><br />
	/// Aggregation viewers are used when displaying aggregation home pages, searches, browses, and information pages.<br /><br />
	/// During a valid html request to display the advanced search page, the following steps occur:
	/// <ul>
	/// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
	/// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="SobekCM_Navigation_Object"/> </li>
	/// <li>Main writer is created for rendering the output, in this case the <see cref="Html_MainWriter"/> </li>
	/// <li>The HTML writer will create the necessary subwriter.  For a collection-level request, an instance of the  <see cref="Aggregation_HtmlSubwriter"/> class is created. </li>
	/// <li>To display the requested collection view, the collection subwriter will creates an instance of this class </li>
	/// </ul></remarks>
	public class Advanced_Search_YearRange_AggregationViewer: abstractAggregationViewer
    {
        /// <summary> Constructor for a new instance of the Advanced_Search_YearRange_AggregationViewer class </summary>
        /// <param name="Current_Aggregation"> Current item aggregation object </param>
        /// <param name="Current_Mode"> Mode / navigation information for the current request</param>
		public Advanced_Search_YearRange_AggregationViewer(Item_Aggregation Current_Aggregation, SobekCM_Navigation_Object Current_Mode)
			: base(Current_Aggregation, Current_Mode)
        {
            // Compute the redirect stem to use
            string fields = currentMode.Search_Fields;
            string searchCollections = currentMode.SubAggregation;
            Display_Mode_Enum lastMode = currentMode.Mode;
			Aggregation_Type_Enum aggrType = currentMode.Aggregation_Type;
            currentMode.SubAggregation = String.Empty;
            string searchString = currentMode.Search_String;
            currentMode.Search_String = String.Empty;
            currentMode.Search_Fields = String.Empty;
            currentMode.Mode = Display_Mode_Enum.Results;
            currentMode.Search_Precision = Search_Precision_Type_Enum.Inflectional_Form;
            string redirectStem = currentMode.Redirect_URL();
            currentMode.Search_String = searchString;
            currentMode.Search_Fields = fields;
            currentMode.SubAggregation = searchCollections;
            currentMode.Mode = lastMode;
	        currentMode.Aggregation_Type = aggrType;

            // If there are children under this hierarchy that can be selected
            //script_action_name = "Javascript:advanced_select_search_sobekcm('" + redirect_stem + "', '" + sub_code + "')";
            //script_include_name = "<script src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_search.js\" type=\"text/javascript\"></script>";

            scriptActionName = "Javascript:advanced_search_years_sobekcm('" + redirectStem + "')";
            scriptIncludeName = "<script src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_search.js\" type=\"text/javascript\"></script>";

        }

        /// <summary> Gets the type of collection view or search supported by this collection viewer </summary>
        /// <value> This returns the <see cref="Item_Aggregation.CollectionViewsAndSearchesEnum.Advanced_Search"/> enumerational value </value>
        public override Item_Aggregation.CollectionViewsAndSearchesEnum Type
        {
            get { return Item_Aggregation.CollectionViewsAndSearchesEnum.Advanced_Search_YearRange; }
        }

        /// <summary>Flag indicates whether the subaggregation selection panel is displayed for this collection viewer</summary>
        /// <value> This property always returns the <see cref="Selection_Panel_Display_Enum.Always"/> enumerational value </value>
        public override Selection_Panel_Display_Enum Selection_Panel_Display
        {
            get
            {
                return Selection_Panel_Display_Enum.Never;
            }
        }

        /// <summary> Flag indicates whether the secondary text requires controls </summary>
        /// <value> This property always returns the value FALSE </value>
        public override bool Always_Display_Home_Text
        {
            get
            {
                return false;
            }
        }

		/// <summary> Add the HTML to be displayed in the search box </summary>
		/// <param name="Output"> Textwriter to write the HTML for this viewer</param>
		/// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
		public override void Add_Search_Box_HTML(TextWriter Output, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("Advanced_Search_AggregationViewer.Add_Search_Box_HTML", "Adding html for search box");
			}

			// Get the list of years for this aggregation
			string aggrCode = currentCollection.Code.ToLower();
			string key = aggrCode + "_YearRanges";
			List<int> yearRange = HttpContext.Current.Cache[key] as List<int>;
			if (yearRange == null)
			{
				yearRange = new List<int>();
				List<string> yearRangeString = SobekCM_Database.Get_Item_Aggregation_Metadata_Browse(aggrCode, "Temporal Year", Tracer);
				foreach (string thisYear in yearRangeString)
				{
					int result;
					if (Int32.TryParse(thisYear, out result))
						yearRange.Add(result);
				}
				HttpContext.Current.Cache.Insert(key, yearRange, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(5));
			}

			string searchLanguage = "Search for:";
			string inLanguage = "in";
			string searchButtonText = "Search";
			string searchOptions = "Search Options";
			string precision = "Precision";
			string contains_exactly = "Contains exactly the search terms";
			string contains_any_form = "Contains any form of the search terms";
			const string CONTAINS_MEANING = "Contains the search term or terms of similar meaning";


			//string select_collect_groups = "Select collection groups to include in search:";
			//string select_collect = "Select collections to include in search:";
			//string select_subcollect = "Select subcollections to include in search:";


			if (currentMode.Language == Web_Language_Enum.Spanish)
			{
				searchLanguage = "Búsqueda de la:";
				inLanguage = "en";
				searchButtonText = "Buscar";
				searchOptions = "Opciones de Búsqueda";
				precision = "Precisión";
				contains_exactly = "Contiene exactamente los términos de búsqueda";
				contains_any_form = "Contiene todas las formas de los términos de búsqueda";
			}

			if (currentMode.Language == Web_Language_Enum.French)
			{
				searchLanguage = "Recherche de:";
				inLanguage = "en";
				//select_collect_groups = "Choisir les group de collection pour inclure dans votre recherche:";
				//select_collect = "Choisir les collections pour inclure dans votre recherche:";
				//select_subcollect = "Choisir les souscollections pour inclure dans votre recherche:";
				searchButtonText = "Search";
			}

			// Now, populate the search terms, if there was one or some
			string text1 = String.Empty;
			string text2 = String.Empty;
			string text3 = String.Empty;
			string text4 = String.Empty;
			if (currentMode.Search_String.Length > 0)
			{
				string[] splitter = currentMode.Search_String.Split(",".ToCharArray());
				text1 = splitter[0].Replace(" =", " or ");
				if (splitter.Length > 1)
				{
					text2 = splitter[1].Replace(" =", " or ");
				}
				if (splitter.Length > 2)
				{
					text3 = splitter[2].Replace(" =", " or ");
				}
				if (splitter.Length > 3)
				{
					text4 = splitter[3].Replace(" =", " or ");
				}
			}

			// Were the search fields specified?
			string andOrValue1 = "+";
			string andOrValue2 = "+";
			string andOrValue3 = "+";
			string dropDownValue1 = "ZZ";
			string dropDownValue2 = "TI";
			string dropDownValue3 = "AU";
			string dropDownValue4 = "TO";

			if (currentMode.Search_Fields.Length > 0)
			{
				// Parse by commas
				string[] fieldSplitter = currentMode.Search_Fields.Replace(" ", "+").Split(",".ToCharArray());

				dropDownValue1 = fieldSplitter[0];

				if ((fieldSplitter.Length > 1) && (fieldSplitter[1].Length > 1))
				{
					andOrValue1 = fieldSplitter[1][0].ToString();
					dropDownValue2 = fieldSplitter[1].Substring(1);
				}

				if ((fieldSplitter.Length > 2) && (fieldSplitter[2].Length > 1))
				{
					andOrValue2 = fieldSplitter[2][0].ToString();
					dropDownValue3 = fieldSplitter[2].Substring(1);
				}
				if ((fieldSplitter.Length > 3) && (fieldSplitter[3].Length > 1))
				{
					andOrValue3 = fieldSplitter[3][0].ToString();
					dropDownValue4 = fieldSplitter[3].Substring(1);
				}
			}

			Output.WriteLine("  <table width=\"100%\" id=\"AdvancedSearchPanel\" >");
			Output.WriteLine("    <tr valign=\"bottom\">");
			Output.WriteLine("      <td width=\"28%\" align=\"right\"><label for=\"Textbox1\">" + searchLanguage + "</label></td>");
			Output.WriteLine("      <td width=\"3%\">&nbsp;</td>");
			Output.WriteLine("      <td width=\"58%\">");
			Output.WriteLine("        <input name=\"Textbox1\" type=\"text\" id=\"Textbox1\" class=\"SobekAdvSearchBox\" value=\"" + text1 + "\" onfocus=\"javascript:textbox_enter('Textbox1', 'SobekAdvSearchBox_focused')\" onblur=\"javascript:textbox_leave('Textbox1', 'SobekAdvSearchBox')\"/>");
			Output.WriteLine("      </td>");
			Output.WriteLine("      <td width=\"3%\" align=\"center\">" + inLanguage + "</td>");
			Output.WriteLine("      <td width=\"8%\">");
			Output.WriteLine("        <select name=\"Dropdownlist1\" id=\"Dropdownlist1\" style=\"width:128px;\" >");

			add_drop_down_options(Output, dropDownValue1);

			Output.WriteLine("        </select>");
			Output.WriteLine("      </td>");
			Output.WriteLine("    </tr>");

			Output.WriteLine("    <tr valign=\"bottom\" style=\"height: 30px;\">");
			Output.WriteLine("      <td colspan=\"2\" align=\"right\">");
			Output.WriteLine("        <select name=\"andOrNotBox1\" id=\"andOrNotBox1\" style=\"width:75px;\" >");
			add_and_or_not_options(Output, andOrValue1);
			Output.WriteLine("        </select>");
			Output.WriteLine("      </td>");
			Output.WriteLine("      <td width=\"58%\">");
			Output.WriteLine("        <input name=\"Textbox2\" type=\"text\" id=\"Textbox2\" class=\"SobekAdvSearchBox\" value=\"" + text2 + "\" onfocus=\"javascript:textbox_enter('Textbox2', 'SobekAdvSearchBox_focused')\" onblur=\"javascript:textbox_leave('Textbox2', 'SobekAdvSearchBox')\"/>");
			Output.WriteLine("      </td>");
			Output.WriteLine("      <td width=\"3%\" align=\"center\">" + inLanguage + "</td>");
			Output.WriteLine("      <td width=\"8%\">");
			Output.WriteLine("        <select name=\"Dropdownlist2\" id=\"Dropdownlist2\" style=\"width:128px;\" >");
			add_drop_down_options(Output, dropDownValue2);
			Output.WriteLine("        </select>");
			Output.WriteLine("      </td>");
			Output.WriteLine("    </tr>");

			Output.WriteLine("    <tr valign=\"bottom\" style=\"height: 30px;\">");
			Output.WriteLine("      <td colspan=\"2\" align=\"right\">");
			Output.WriteLine("        <select name=\"andOrNotBox2\" id=\"andOrNotBox2\" style=\"width:75px;\" >");
			add_and_or_not_options(Output, andOrValue2);
			Output.WriteLine("        </select>");
			Output.WriteLine("      </td>");
			Output.WriteLine("      <td width=\"58%\">");
			Output.WriteLine("        <input name=\"Textbox3\" type=\"text\" id=\"Textbox3\" class=\"SobekAdvSearchBox\" value=\"" + text3 + "\" onfocus=\"javascript:textbox_enter('Textbox3', 'SobekAdvSearchBox_focused')\" onblur=\"javascript:textbox_leave('Textbox3', 'SobekAdvSearchBox')\"/>");
			Output.WriteLine("      </td>");
			Output.WriteLine("      <td width=\"3%\" align=\"center\">" + inLanguage + "</td>");
			Output.WriteLine("      <td width=\"8%\">");
			Output.WriteLine("        <select name=\"Dropdownlist3\" id=\"Dropdownlist3\" style=\"width:128px;\">");
			add_drop_down_options(Output, dropDownValue3);
			Output.WriteLine("        </select>");
			Output.WriteLine("      </td>");
			Output.WriteLine("    </tr>");

			Output.WriteLine("    <tr valign=\"bottom\" style=\"height: 30px;\">");
			Output.WriteLine("      <td colspan=\"2\" align=\"right\">");
			Output.WriteLine("        <select name=\"andOrNotBox3\" id=\"andOrNotBox3\" style=\"width:75px;\" >");
			add_and_or_not_options(Output, andOrValue3);
			Output.WriteLine("        </select>");
			Output.WriteLine("      </td>");
			Output.WriteLine("      <td width=\"58%\">");
			Output.WriteLine("        <input name=\"Textbox4\" type=\"text\" id=\"Textbox4\" class=\"SobekAdvSearchBox\" value=\"" + text4 + "\"  onfocus=\"javascript:textbox_enter('Textbox4', 'SobekAdvSearchBox_focused')\" onblur=\"javascript:textbox_leave('Textbox4', 'SobekAdvSearchBox')\"/>");
			Output.WriteLine("      </td>");
			Output.WriteLine("      <td width=\"3%\" align=\"center\">" + inLanguage + "</td>");
			Output.WriteLine("      <td width=\"8%\">");
			Output.WriteLine("        <select name=\"Dropdownlist4\" id=\"Dropdownlist4\" style=\"width:128px;\">");
			add_drop_down_options(Output, dropDownValue4);
			Output.WriteLine("        </select>");
			Output.WriteLine("      </td>");
			Output.WriteLine("    </tr>");

			// Show the year range data, if there are any years in this
			const string YEAR_RANGE = "Limit by Year";
			if (yearRange.Count > 0)
			{
				Output.WriteLine("    <tr style=\"align:right; height:50px;\">");
				Output.WriteLine("      <td align=\"right\">" + YEAR_RANGE + ":</td>");
				Output.WriteLine("      <td>&nbsp;</td>");
				Output.WriteLine("      <td align=\"left\" colspan=\"2\">");

				Output.WriteLine("        <select name=\"YearDropDown1\" id=\"YearDropDown1\" >");
				//	Output.WriteLine("          <option value=\"ZZ\"> </option>");
				int currYear1 = currentMode.DateRange_Year1;
				if ((currYear1 != -1) && (!yearRange.Contains(currYear1)))
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

				Output.WriteLine("        <select name=\"YearDropDown2\" id=\"YearDropDown2\" >");
				//	Output.WriteLine("          <option value=\"ZZ\"> </option>");
				int currYear2 = currentMode.DateRange_Year1;
				if ((currYear2 != -1) && (!yearRange.Contains(currYear2)))
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
				Output.WriteLine("      <td align=\"right\">");
				Output.WriteLine("        <span id=\"circular_progress\" class=\"hidden_progress\">&nbsp;</span> &nbsp; ");


				if (currentCollection.Children_Count > 0)
				{
					Output.WriteLine("        <a onmousedown=\"" + scriptActionName + "\"><input type=\"button\" name=\"searchButton\" value=\"" + searchButtonText + "\" id=\"searchButton\" class=\"SobekSearchButton\" /></a>");
				}
				else
				{
					Output.WriteLine("        <a onmousedown=\"" + scriptActionName + "\"><input type=\"button\" name=\"searchButton\" value=\"" + searchButtonText + "\" id=\"searchButton\" class=\"SobekSearchButton\" /></a>");
				}

				Output.WriteLine("      </td>");
				Output.WriteLine("    </tr>");
			}
			else
			{
				Output.WriteLine("    <tr valign=\"bottom\" style=\"height: 30px;\">");
				Output.WriteLine("      <td colspan=\"5\" align=\"right\">");
				Output.WriteLine("        <span id=\"circular_progress\" class=\"hidden_progress\">&nbsp;</span> &nbsp; ");


				if (currentCollection.Children_Count > 0)
				{
					Output.WriteLine("        <a onmousedown=\"" + scriptActionName + "\"><input type=\"button\" name=\"searchButton\" value=\"" + searchButtonText + "\" id=\"searchButton\" class=\"SobekSearchButton\" /></a>");
				}
				else
				{
					Output.WriteLine("        <a onmousedown=\"" + scriptActionName + "\"><input type=\"button\" name=\"searchButton\" value=\"" + searchButtonText + "\" id=\"searchButton\" class=\"SobekSearchButton\" /></a>");
				}

				Output.WriteLine("      </td>");
				Output.WriteLine("    </tr>");
			}


			Output.WriteLine("    <tr valign=\"bottom\" style=\"height: 30px;\">");
			Output.WriteLine("      <td colspan=\"2\" align=\"right\" valign=\"middle\"><span style=\"color:#888; font-size:1.1em\"><b>" + searchOptions + "</b></span></td>");
			Output.WriteLine("      <td valign=\"middle\" align=\"left\"> &nbsp; &nbsp; <a href=\"" + currentMode.Base_URL + "help\" target=\"SEARCHHELP\" ><img src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/help_button.jpg\" border=\"0px\" alt=\"HELP\" /></a></td>");
			Output.WriteLine("      <td colspan=\"2\">&nbsp;</td>");
			Output.WriteLine("    </tr>");
			Output.WriteLine("    <tr>");
			Output.WriteLine("      <td colspan=\"5\">");
			Output.WriteLine("        <table>");
			Output.WriteLine("           <tr align=\"left\" valign=\"top\">");
			Output.WriteLine("             <td width=\"120px\">&nbsp;</td>");
			Output.WriteLine("             <td>" + precision + ": &nbsp; </td>");
			Output.WriteLine("             <td>");
			Output.WriteLine("               <input type=\"radio\" name=\"precision\" id=\"precisionContains\" value=\"contains\" /> <label for=\"precisionContains\">" + contains_exactly + "</label> <br />");
			Output.WriteLine("               <input type=\"radio\" name=\"precision\" id=\"precisionResults\" value=\"results\" checked=\"checked\" /> <label for=\"precisionResults\">" + contains_any_form + "</label> <br />");
			if (currentMode.Language == Web_Language_Enum.English)
			{
				Output.WriteLine("               <input type=\"radio\" name=\"precision\" id=\"precisionLike\" value=\"resultslike\" /> <label for=\"precisionLike\">" + CONTAINS_MEANING + "</label> ");
			}
			Output.WriteLine("             </td>");
			Output.WriteLine("           </tr>   ");
			Output.WriteLine("         </table>");
			Output.WriteLine("       </td>");
			Output.WriteLine("       </tr>");
			Output.WriteLine("  </table>");

			Output.WriteLine();
			Output.WriteLine("<!-- Focus on first search box -->");
			Output.WriteLine("<script type=\"text/javascript\">focus_element('Textbox1');</script>");
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
                Tracer.Add_Trace("Advanced_Search_AggregationViewer.Add_Secondary_HTML", "Adding simple search tips");
            }

            Add_Simple_Search_Tips(Output, Tracer);
        }

        private void add_drop_down_options(TextWriter Output, string DropValue )
        {
            foreach (Metadata_Search_Field searchField in currentCollection.Advanced_Search_Fields.Select(SobekCM_Library_Settings.Metadata_Search_Field_By_ID).Where(SearchField => SearchField != null))
            {
                if (searchField.Web_Code == DropValue)
                {
                    Output.WriteLine("          <option value=\"" + searchField.Web_Code + "\" selected=\"selected\" >" + translator.Get_Translation(searchField.Display_Term, currentMode.Language) + "</option>");
                }
                else
                {
                    Output.WriteLine("          <option value=\"" + searchField.Web_Code + "\">" + translator.Get_Translation(searchField.Display_Term, currentMode.Language) + "</option>");
                }
            }
        }

        private void add_and_or_not_options(TextWriter Output, string AndOrValue)
        {
            string and_language = "and";
            string or_language = "or";
            string and_not_language = "and not";
            if (currentMode.Language == Web_Language_Enum.French)
            {
                and_language = "et";
                or_language = "ou";
                and_not_language = "et non";
            }
            if (currentMode.Language == Web_Language_Enum.Spanish)
            {
                and_language = "y";
                or_language = "o";
                and_not_language = "y no";
            }

            if (AndOrValue == "+")
            {
                Output.WriteLine("          <option value=\"+\" selected=\"selected\" >" + and_language + "</option>");
            }
            else
            {
                Output.WriteLine("          <option value=\"+\" >" + and_language + "</option>");
            }

            if (AndOrValue == "=")
            {
                Output.WriteLine("          <option value=\"=\" selected=\"selected\" >" + or_language + "</option>");
            }
            else
            {
                Output.WriteLine("          <option value=\"=\">" + or_language + "</option>");
            }

            if (AndOrValue == "-")
            {
                Output.WriteLine("          <option value=\"-\" selected=\"selected\" >" + and_not_language + "</option>");
            }
            else
            {
                Output.WriteLine("          <option value=\"-\">" + and_not_language + "</option>");
            }
        }
    }
}
