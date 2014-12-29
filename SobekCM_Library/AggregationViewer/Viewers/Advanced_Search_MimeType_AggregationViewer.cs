#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SobekCM.Core.Aggregations;
using SobekCM.Core.Configuration;
using SobekCM.Core.Navigation;
using SobekCM.Core.Search;
using SobekCM.Engine_Library.Navigation;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Tools;
using SobekCM.UI_Library;

#endregion

namespace SobekCM.Library.AggregationViewer.Viewers
{
    /// <summary> Renders the advanced search page for a given item aggregation </summary>
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
    public class Advanced_Search_MimeType_AggregationViewer : abstractAggregationViewer
    {
        /// <summary> Constructor for a new instance of the Advanced_Search_MimeType_AggregationViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        public Advanced_Search_MimeType_AggregationViewer(RequestCache RequestSpecificValues) : base(RequestSpecificValues)
        {
            // Compute the redirect stem to use
            string fields = RequestSpecificValues.Current_Mode.Search_Fields;
            string searchCollections = RequestSpecificValues.Current_Mode.SubAggregation;
            Display_Mode_Enum lastMode = RequestSpecificValues.Current_Mode.Mode;
            Aggregation_Type_Enum aggrType = RequestSpecificValues.Current_Mode.Aggregation_Type;
            RequestSpecificValues.Current_Mode.SubAggregation = String.Empty;
            string searchString = RequestSpecificValues.Current_Mode.Search_String;

            RequestSpecificValues.Current_Mode.Search_String = String.Empty;
            RequestSpecificValues.Current_Mode.Search_Fields = String.Empty;
            RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Results;
            RequestSpecificValues.Current_Mode.Search_Precision = Search_Precision_Type_Enum.Inflectional_Form;
            string redirectStem = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);

            RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Aggregation;
            RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Browse_Info;
            RequestSpecificValues.Current_Mode.Info_Browse_Mode = "all";
            string browse_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);

            RequestSpecificValues.Current_Mode.Search_String = searchString;
            RequestSpecificValues.Current_Mode.Search_Fields = fields;
            RequestSpecificValues.Current_Mode.SubAggregation = searchCollections;
            RequestSpecificValues.Current_Mode.Mode = lastMode;
            RequestSpecificValues.Current_Mode.Aggregation_Type = aggrType;
            Search_Script_Action = "advanced_search_sobekcm('" + redirectStem + "', '" + browse_url + "')";

        }

        /// <summary> Gets the type of collection view or search supported by this collection viewer </summary>
        /// <value> This returns the <see cref="Item_Aggregation.CollectionViewsAndSearchesEnum.Advanced_Search_MimeType"/> enumerational value </value>
        public override Item_Aggregation.CollectionViewsAndSearchesEnum Type
        {
            get { return Item_Aggregation.CollectionViewsAndSearchesEnum.Advanced_Search_MimeType; }
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
                Tracer.Add_Trace("Advanced_Search_AggregationViewer.Add_Search_Box_HTML", "Adding html for search box");
            }

            string searchLanguage = "Search for:";
            string inLanguage = "in";
            string searchButtonText = "Search";
            string searchOptions = "Search Options";
            string precision = "Precision";
            string contains_exactly = "Contains exactly the search terms";
            string contains_any_form = "Contains any form of the search terms";
            const string CONTAINS_MEANING = "Contains the search term or terms of similar meaning";
            const string INCLUDE_NO_MIMETYPE = "Include items with records only";

            //string select_collect_groups = "Select collection groups to include in search:";
            //string select_collect = "Select collections to include in search:";
            //string select_subcollect = "Select subcollections to include in search:";


            if (RequestSpecificValues.Current_Mode.Language == Web_Language_Enum.Spanish)
            {
                searchLanguage = "Búsqueda de la:";
                inLanguage = "en";
                searchButtonText = "Buscar";
                searchOptions = "Opciones de Búsqueda";
                precision = "Precisión";
                contains_exactly = "Contiene exactamente los términos de búsqueda";
                contains_any_form = "Contiene todas las formas de los términos de búsqueda";
            }

            if (RequestSpecificValues.Current_Mode.Language == Web_Language_Enum.French)
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
            if (RequestSpecificValues.Current_Mode.Search_String.Length > 0)
            {
                string[] splitter = RequestSpecificValues.Current_Mode.Search_String.Split(",".ToCharArray());
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

            if (RequestSpecificValues.Current_Mode.Search_Fields.Length > 0)
            {
                // Parse by commas
                string[] fieldSplitter = RequestSpecificValues.Current_Mode.Search_Fields.Replace(" ", "+").Split(",".ToCharArray());

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

            Output.WriteLine("  <table id=\"sbkAsav_SearchPanel\" >");
            Output.WriteLine("    <tr>");
            Output.WriteLine("      <td style=\"width:28%;text-align:right;\"><label for=\"Textbox1\">" + searchLanguage + "</label></td>");
            Output.WriteLine("      <td style=\"width:3%;\">&nbsp;</td>");
            Output.WriteLine("      <td style=\"width:58%;\">");
            Output.WriteLine("        <input name=\"Textbox1\" type=\"text\" id=\"Textbox1\" class=\"sbkAsav_SearchBox sbk_Focusable\" value=\"" + text1 + "\" />");
            Output.WriteLine("      </td>");
            Output.WriteLine("      <td style\"width:3%;text-align:center;\">" + inLanguage + "</td>");
            Output.WriteLine("      <td style=\"width:8%;\">");
            Output.WriteLine("        <select name=\"Dropdownlist1\" id=\"Dropdownlist1\" class=\"sbkAsav_DropDownList\" style=\"width:128px;\" >");

            add_drop_down_options(Output, dropDownValue1);

            Output.WriteLine("        </select>");
            Output.WriteLine("      </td>");
            Output.WriteLine("    </tr>");

            Output.WriteLine("    <tr>");
            Output.WriteLine("      <td colspan=\"2\" style=\"text-align:right;\">");
            Output.WriteLine("        <select name=\"andOrNotBox1\" id=\"andOrNotBox1\" class=\"sbkAsav_AndOrNotBox\">");
            add_and_or_not_options(Output, andOrValue1);
            Output.WriteLine("        </select>");
            Output.WriteLine("      </td>");
            Output.WriteLine("      <td style=\"width:58%;\">");
            Output.WriteLine("        <input name=\"Textbox2\" type=\"text\" id=\"Textbox2\" class=\"sbkAsav_SearchBox sbk_Focusable\" value=\"" + text2 + "\" />");
            Output.WriteLine("      </td>");
            Output.WriteLine("      <td style\"width:3%;text-align:center;\">" + inLanguage + "</td>");
            Output.WriteLine("      <td style=\"width:8%;\">");
            Output.WriteLine("        <select name=\"Dropdownlist2\" id=\"Dropdownlist2\" class=\"sbkAsav_DropDownList\" style=\"width:128px;\" >");
            add_drop_down_options(Output, dropDownValue2);
            Output.WriteLine("        </select>");
            Output.WriteLine("      </td>");
            Output.WriteLine("    </tr>");

            Output.WriteLine("    <tr>");
            Output.WriteLine("      <td colspan=\"2\" style=\"text-align:right;\">");
            Output.WriteLine("        <select name=\"andOrNotBox2\" id=\"andOrNotBox2\" class=\"sbkAsav_AndOrNotBox\">");
            add_and_or_not_options(Output, andOrValue2);
            Output.WriteLine("        </select>");
            Output.WriteLine("      </td>");
            Output.WriteLine("      <td style=\"width:58%;\">");
            Output.WriteLine("        <input name=\"Textbox3\" type=\"text\" id=\"Textbox3\" class=\"sbkAsav_SearchBox sbk_Focusable\" value=\"" + text3 + "\" />");
            Output.WriteLine("      </td>");
            Output.WriteLine("      <td style\"width:3%;text-align:center;\">" + inLanguage + "</td>");
            Output.WriteLine("      <td style=\"width:8%;\">");
            Output.WriteLine("        <select name=\"Dropdownlist3\" id=\"Dropdownlist3\" class=\"sbkAsav_DropDownList\" style=\"width:128px;\">");
            add_drop_down_options(Output, dropDownValue3);
            Output.WriteLine("        </select>");
            Output.WriteLine("      </td>");
            Output.WriteLine("    </tr>");

            Output.WriteLine("    <tr>");
            Output.WriteLine("      <td colspan=\"2\" style=\"text-align:right;\">");
            Output.WriteLine("        <select name=\"andOrNotBox3\" id=\"andOrNotBox3\" class=\"sbkAsav_AndOrNotBox\">");
            add_and_or_not_options(Output, andOrValue3);
            Output.WriteLine("        </select>");
            Output.WriteLine("      </td>");
            Output.WriteLine("      <td style=\"width:58%;\">");
            Output.WriteLine("        <input name=\"Textbox4\" type=\"text\" id=\"Textbox4\" class=\"sbkAsav_SearchBox sbk_Focusable\" value=\"" + text4 + "\" />");
            Output.WriteLine("      </td>");
            Output.WriteLine("      <td style\"width:3%;text-align:center;\">" + inLanguage + "</td>");
            Output.WriteLine("      <td style=\"width:8%;\">");
            Output.WriteLine("        <select name=\"Dropdownlist4\" id=\"Dropdownlist4\" class=\"sbkAsav_DropDownList\" style=\"width:128px;\">");
            add_drop_down_options(Output, dropDownValue4);
            Output.WriteLine("        </select>");
            Output.WriteLine("      </td>");
            Output.WriteLine("    </tr>");

            Output.WriteLine("    <tr style=\"height:50px; vertical-align:middle;\">");
            Output.WriteLine("      <td colspan=\"4\">&nbsp; &nbsp; &nbsp; &nbsp; <input type=\"checkbox\" value=\"MIME_TYPE\" name=\"sbkAsav_mimetypeCheck\" id=\"sbkAsav_mimetypeCheck\" unchecked onclick=\"focus_element( 'SobekHomeSearchBox');\" /><label for=\"sbkAsav_mimetypeCheck\">" + INCLUDE_NO_MIMETYPE + "</label></td>");
            Output.WriteLine("      <td style=\"text-align:right;\">");
            Output.WriteLine("        <span id=\"circular_progress\" class=\"hidden_progress\">&nbsp;</span> &nbsp; ");


            if (RequestSpecificValues.Hierarchy_Object.Children_Count > 0)
            {
                Output.WriteLine("        <button name=\"searchButton\" id=\"searchButton\" class=\"sbk_SearchButton\" onclick=\"" + Search_Script_Action + ";return false;\">" + searchButtonText + "<img id=\"sbkAsav_ButtonArrow\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/button_next_arrow2.png\" alt=\"\" /></button>");
            }
            else
            {
                Output.WriteLine("        <button name=\"searchButton\" id=\"searchButton\" class=\"sbk_SearchButton\" onclick=\"" + Search_Script_Action + ";return false;\">" + searchButtonText + "<img id=\"sbkAsav_ButtonArrow\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/images/button_next_arrow2.png\" alt=\"\" /></button>");
            }

            Output.WriteLine("      </td>");
            Output.WriteLine("    </tr>");
            Output.WriteLine("    <tr>");
            Output.WriteLine("      <td colspan=\"2\" class=\"sbkAsav_SearchOptions\">" + searchOptions + "</span></td>");
            Output.WriteLine("      <td style=\"vertical-align:middle;text-align:left;\"> &nbsp; &nbsp; <a href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "help\" target=\"SEARCHHELP\" ><img src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "design/skins/" + RequestSpecificValues.Current_Mode.Base_Skin + "/buttons/help_button.jpg\" alt=\"HELP\" /></a></td>");
            Output.WriteLine("      <td colspan=\"2\">&nbsp;</td>");
            Output.WriteLine("    </tr>");
            Output.WriteLine("    <tr>");
            Output.WriteLine("      <td colspan=\"5\">");
            Output.WriteLine("        <table>");
            Output.WriteLine("           <tr style=\"text-align:left;vertical-align:top;\">");
            Output.WriteLine("             <td style=\"width:120px;\">&nbsp;</td>");
            Output.WriteLine("             <td>" + precision + ": &nbsp; </td>");
            Output.WriteLine("             <td>");
            Output.WriteLine("               <input type=\"radio\" name=\"precision\" id=\"precisionContains\" value=\"contains\" /> <label for=\"precisionContains\">" + contains_exactly + "</label> <br />");
            Output.WriteLine("               <input type=\"radio\" name=\"precision\" id=\"precisionResults\" value=\"results\" checked=\"checked\" /> <label for=\"precisionResults\">" + contains_any_form + "</label> <br />");
            if (RequestSpecificValues.Current_Mode.Language == Web_Language_Enum.English)
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

        private void add_drop_down_options(TextWriter Output, string DropValue)
        {
            foreach (Metadata_Search_Field searchField in RequestSpecificValues.Hierarchy_Object.Advanced_Search_Fields.Select(UI_ApplicationCache_Gateway.Settings.Metadata_Search_Field_By_ID).Where(SearchField => SearchField != null))
            {
                if (searchField.Web_Code == DropValue)
                {
                    Output.WriteLine("          <option value=\"" + searchField.Web_Code + "\" selected=\"selected\" >" + UI_ApplicationCache_Gateway.Translation.Get_Translation(searchField.Display_Term, RequestSpecificValues.Current_Mode.Language) + "</option>");
                }
                else
                {
                    Output.WriteLine("          <option value=\"" + searchField.Web_Code + "\">" + UI_ApplicationCache_Gateway.Translation.Get_Translation(searchField.Display_Term, RequestSpecificValues.Current_Mode.Language) + "</option>");
                }
            }
        }

        private void add_and_or_not_options(TextWriter Output, string AndOrValue)
        {
            string and_language = "and";
            string or_language = "or";
            string and_not_language = "and not";
            if (RequestSpecificValues.Current_Mode.Language == Web_Language_Enum.French)
            {
                and_language = "et";
                or_language = "ou";
                and_not_language = "et non";
            }
            if (RequestSpecificValues.Current_Mode.Language == Web_Language_Enum.Spanish)
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
