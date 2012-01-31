#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using SobekCM.Library.Aggregations;
using SobekCM.Library.Application_State;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.Navigation;

#endregion

namespace SobekCM.Library.AggregationViewer.Viewers
{
    /// <summary> Renders the basic search / home page for a given item aggregation </summary>
    /// <remarks> This class implements the <see cref="iAggregationViewer"/> interface and extends the <see cref="abstractAggregationViewer"/> class.<br /><br />
    /// Collection viewers are used when displaying collection home pages, searches, browses, and information pages.<br /><br />
    /// During a valid html request to display the home page with basic search, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="SobekCM_Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in this case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  For a collection-level request, an instance of the  <see cref="Aggregation_HtmlSubwriter"/> class is created. </li>
    /// <li>To display the requested collection view, the collection subwriter will creates an instance of this class </li>
    /// </ul></remarks>
    public class Rotating_Highlight_Search_AggregationViewer : abstractAggregationViewer
    {
        private readonly string arg1;
        private readonly string arg2;
        private readonly string browse_url;
        private readonly string textBoxValue;

        /// <summary> Constructor for a new instance of the Basic_Search_AggregationViewer class </summary>
        /// <param name="Current_Aggregation"> Current item aggregation object </param>
        /// <param name="Current_Mode"> Mode / navigation information for the current request</param>
        public Rotating_Highlight_Search_AggregationViewer(Item_Aggregation Current_Aggregation, SobekCM_Navigation_Object Current_Mode): base(Current_Aggregation, Current_Mode)
        {
            // Determine the sub text to use
            const string subCode = "s=";
            Sharing_Buttons_HTML = String.Empty;

            // Save the search term
            if (currentMode.Search_String.Length > 0)
            {
                textBoxValue = currentMode.Search_String;
            }

            // Determine the complete script action name
            Display_Mode_Enum displayMode = currentMode.Mode;
            Search_Type_Enum searchType = currentMode.Search_Type;
            currentMode.Mode = Display_Mode_Enum.Results;
            currentMode.Search_Type = Search_Type_Enum.Basic;
            currentMode.Search_Precision = Search_Precision_Type_Enum.Inflectional_Form;
            string search_string = currentMode.Search_String;
            currentMode.Search_String = String.Empty;
            currentMode.Search_Fields = String.Empty;
            arg2 = String.Empty;
            arg1 = currentMode.Redirect_URL();
            currentMode.Mode = Display_Mode_Enum.Aggregation_Browse_Info;
            currentMode.Info_Browse_Mode = "all";
            browse_url = currentMode.Redirect_URL();
            currentMode.Info_Browse_Mode = String.Empty;
            currentMode.Mode = Display_Mode_Enum.Aggregation_Home;
            if ((!currentMode.Show_Selection_Panel) || (Current_Aggregation.Children_Count == 0))
            {
                scriptActionName = "basic_search_sobekcm('" + arg1 + "', '" + browse_url + "')";
                scriptIncludeName = "<script src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_search.js\" type=\"text/javascript\"></script>";
            }
            else
            {
                scriptActionName = "basic_select_search_sobekcm('" + arg1 + "', '" + subCode + "')";
                arg2 = subCode;
                scriptIncludeName = "<script src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_search.js\" type=\"text/javascript\"></script>";
            }
            currentMode.Mode = displayMode;
            currentMode.Search_Type = searchType;
            currentMode.Search_String = search_string;
        }

        /// <summary> Sets the sharing buttons HTML to display over the banner </summary>
        public string Sharing_Buttons_HTML { private get; set; }

        /// <summary> Gets the type of collection view or search supported by this collection viewer </summary>
        /// <value> This returns the <see cref="Item_Aggregation.CollectionViewsAndSearchesEnum.Basic_Search"/> enumerational value </value>
        public override Item_Aggregation.CollectionViewsAndSearchesEnum Type
        {
            get { return Item_Aggregation.CollectionViewsAndSearchesEnum.Rotating_Highlight_Search; }
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
                Tracer.Add_Trace("Rotating_Highlight_Search_AggregationViewer.Add_Search_Box_HTML", "Adding html for search box");
            }

            string search_collection = "Search Collection";
            //string include_privates = "Include non-public items";
            if (currentMode.Language == Language_Enum.Spanish)
            {
                search_collection = "Buscar en la colección";
            }

            if (currentMode.Language == Language_Enum.French)
            {
                search_collection = "Recherche dans la collection";
            }

            if (currentCollection.Highlights.Count > 1)
            {
                Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/contentslider.js\" > </script>");
                Output.WriteLine("<!-- *****************************************");
                Output.WriteLine( "    * Featured Content Slider- (c) Dynamic Drive DHTML code library (www.dynamicdrive.com)");
                Output.WriteLine("     * This notice MUST stay intact for legal use");
                Output.WriteLine("     * Visit Dynamic Drive at http://www.dynamicdrive.com/ for this script and 100s more");
                Output.WriteLine("     *********************************************** -->");
            }

            Output.WriteLine("<table cellpadding=\"0px\" cellspacing=\"0px\" class=\"SobekHomeBanner\">");
            Output.WriteLine("  <tr valign=\"top\">");

            if (currentCollection.Front_Banner_Left_Side)
            {
                string banner_image = currentMode.Base_URL + "design/" + currentCollection.objDirectory + currentCollection.Front_Banner_Image(currentMode.Language);
                Output.Write("    <td style=\"background-image: url( " + banner_image + "); width: " + currentCollection.Front_Banner_Width + "px; height: " + currentCollection.Front_Banner_Height + "px;\" ");
                Output.WriteLine("align=\"left\">");
                Output.WriteLine(Sharing_Buttons_HTML.Replace("span", "div"));
                Output.WriteLine("");
                Output.WriteLine("      <div style=\"padding-top: 145px;padding-left: 240px;\">");
                Output.WriteLine("        <table>");
                Output.WriteLine("          <tr>");
                Output.WriteLine("            <td colspan=\"2\">");
                Output.WriteLine("               <b><label for=\"SobekHomeBannerSearchBox\">" + search_collection + ":</label></b>");
                Output.WriteLine("            </td>");
                Output.WriteLine("          </tr>");
                Output.WriteLine("          <tr valign=\"bottom\">");
                Output.WriteLine("            <td>");
                Output.Write("              <input name=\"u_search\" type=\"text\" class=\"SobekHomeBannerSearchBox\" id=\"SobekHomeBannerSearchBox\" value=\"" + textBoxValue + "\" onfocus=\"textbox_enter('SobekHomeBannerSearchBox', 'SobekHomeBannerSearchBox_focused');\" onblur=\"textbox_leave('SobekHomeBannerSearchBox', 'SobekHomeBannerSearchBox');\"");
                if (currentMode.Browser_Type.IndexOf("IE") >= 0)
                    Output.WriteLine(" onkeydown=\"return fnTrapKD(event, 'basic', '" + arg1 + "', '" + arg2 + "','" + browse_url + "');\" />");
                else
                    Output.WriteLine(" onkeydown=\"return fnTrapKD(event, 'basic', '" + arg1 + "', '" + arg2 + "','" + browse_url + "');\" />");

                Output.WriteLine("            </td>");
                Output.WriteLine("            <td>");
                Output.WriteLine("              <a onclick=\"" + scriptActionName + "\"><img name=\"jsbutton\" id=\"jsbutton\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/go_button.gif\" border=\"0\" alt=\"" + search_collection + "\" /></a>");
                Output.WriteLine("            </td>");
                Output.WriteLine("          </tr>");
                Output.WriteLine("        </table>");
                Output.WriteLine("      </div>");
                Output.WriteLine("    </td>");
            }

            Output.WriteLine("    <td>");
            string base_design_location = currentMode.Base_URL + "design/aggregations/" + currentMode.Aggregation + "/";
            if (currentMode.Aggregation.Length == 0)
                base_design_location = currentMode.Base_URL + "design/aggregations/all/";

            // Either draw all the highlights to flip through, or just the one highlight
            if (currentCollection.Highlights.Count > 1)
            {
                int width = 754 - currentCollection.Front_Banner_Width;

                Output.WriteLine("      <div id=\"slider1\" class=\"sliderwrapper\" style=\"width:" + width + "px\">");

                // Copy over just the eight highlights we should use 
                List<Item_Aggregation_Highlights> highlights_to_use = new List<Item_Aggregation_Highlights>();
                int day_integer = DateTime.Now.DayOfYear + 1;
                int highlight_to_use = day_integer % currentCollection.Highlights.Count;
                int number = Math.Min(8, currentCollection.Highlights.Count);
                for (int i = 0; i < number; i++)
                {
                    highlights_to_use.Add(currentCollection.Highlights[highlight_to_use]);
                    highlight_to_use++;
                    if (highlight_to_use >= currentCollection.Highlights.Count)
                        highlight_to_use = 0;
                }

                foreach (Item_Aggregation_Highlights highlight in highlights_to_use)
                {
                    Output.WriteLine("        <div class=\"contentdiv\" style=\"width:" + width + "px\">");
                    string highlight_text = HttpUtility.HtmlEncode(highlight.Get_Tooltip(currentMode.Language));
                    if (highlight.Link.Length > 0)
                    {
                        Output.WriteLine("          <a href=\"" + highlight.Link + "\" title=\"" + highlight_text + "\" >");
                    }
                    if (highlight.Image.IndexOf("http:") >= 0)
                        Output.WriteLine("            <img src=\"" + highlight.Image + "\" border=\"0px\" title=\"" + highlight_text + "\" alt=\"" + highlight_text + "\" />");
                    else
                        Output.WriteLine("            <img src=\"" + base_design_location + highlight.Image + "\" border=\"0px\" title=\"" + highlight_text + "\" alt=\"" + highlight_text + "\" />");
                    if (highlight.Link.Length > 0)
                    {
                        Output.WriteLine("          </a>");
                    }
                    Output.WriteLine("        </div>");
                }
                Output.WriteLine("      </div>");
                int pagination_width = 735 - currentCollection.Front_Banner_Width;
                Output.WriteLine("      <div id=\"paginate-slider1\" class=\"pagination\" style=\"width:" + pagination_width + "px\" >  </div>");



                Output.WriteLine("      <script type=\"text/javascript\">");
                Output.WriteLine("          featuredcontentslider.init({");
                Output.WriteLine("              id: \"slider1\",  //id of main slider DIV");
                Output.WriteLine("              contentsource: [\"inline\", \"\"],  //Valid values: [\"inline\", \"\"] or [\"ajax\", \"path_to_file\"]");
                Output.WriteLine("              toc: \"#increment\",  //Valid values: \"#increment\", \"markup\", [\"label1\", \"label2\", etc]");
                Output.WriteLine("              nextprev: [\"\", \"\"], //nextprev: [\"Previous\", \"Next\"],  labels for \"prev\" and \"next\" links. Set to \"\" to hide.");
                Output.WriteLine("              revealtype: \"click\", //Behavior of pagination links to reveal the slides: \"click\" or \"mouseover\"");
                Output.WriteLine("              enablefade: [true, 0.05],  //[true/false, fadedegree]");
                Output.WriteLine("              autorotate: [true, 5000],  //[true/false, pausetime]");
                Output.WriteLine("              onChange: function(previndex, curindex) {  //event handler fired whenever script changes slide");
                Output.WriteLine("                  //previndex holds index of last slide viewed b4 current (1=1st slide, 2nd=2nd etc)");
                Output.WriteLine("                  //curindex holds index of currently shown slide (1=1st slide, 2nd=2nd etc)");
                Output.WriteLine("              }");
                Output.WriteLine("          })");
                Output.WriteLine("      </script>");
            }
            else
            {
                if (currentCollection.Highlights.Count > 0)
                {
                    Output.WriteLine("          <a href=\"" + currentCollection.Highlights[0].Link + "\" title=\"" + HttpUtility.HtmlEncode(currentCollection.Highlights[0].Get_Text(currentMode.Language)) + "\" >");
                    if (currentCollection.Highlights[0].Image.IndexOf("http:") >= 0)
                        Output.WriteLine("            <img src=\"" + currentCollection.Highlights[0].Image + "\" />");
                    else
                        Output.WriteLine("            <img src=\"" + base_design_location + currentCollection.Highlights[0].Image + "\" />");
                    Output.WriteLine("          </a>");
                }
            }


            Output.WriteLine("    </td>");


            if (!currentCollection.Front_Banner_Left_Side)
            {
                string banner_image = currentMode.Base_URL + "design/" + currentCollection.objDirectory + currentCollection.Front_Banner_Image(currentMode.Language);
                Output.Write("    <td style=\"background-image: url( " + banner_image + "); width: " + currentCollection.Front_Banner_Width + "px; height: " + currentCollection.Front_Banner_Height + "px;\" ");
                Output.WriteLine("align=\"left\">");
                Output.WriteLine(Sharing_Buttons_HTML.Replace("span", "div"));
                Output.WriteLine("");
                Output.WriteLine("      <div style=\"padding-top: 145px; padding-left: 20px; \">");
                Output.WriteLine("        <table>");
                Output.WriteLine("          <tr>");
                Output.WriteLine("            <td colspan=\"2\">");
                Output.WriteLine("               <b><label for=\"SobekHomeBannerSearchBox\">" + search_collection + ":</label></b>");
                Output.WriteLine("            </td>");
                Output.WriteLine("          </tr>");
                Output.WriteLine("          <tr valign=\"bottom\">");
                Output.WriteLine("            <td>");
                Output.Write("              <input name=\"u_search\" type=\"text\" class=\"SobekHomeBannerSearchBox\" id=\"SobekHomeBannerSearchBox\" value=\"" + textBoxValue + "\" onfocus=\"textbox_enter('SobekHomeBannerSearchBox', 'SobekHomeBannerSearchBox_focused');\" onblur=\"textbox_leave('SobekHomeBannerSearchBox', 'SobekHomeBannerSearchBox');\"");
                if (currentMode.Browser_Type.IndexOf("IE") >= 0)
                    Output.WriteLine(" onkeydown=\"return fnTrapKD(event, 'basic', '" + arg1 + "', '" + arg2 + "');\" />");
                else
                    Output.WriteLine(" onkeydown=\"return fnTrapKD(event, 'basic', '" + arg1 + "', '" + arg2 + "');\" />");

                Output.WriteLine("            </td>");
                Output.WriteLine("            <td>");
                Output.WriteLine("              <a onclick=\"" + scriptActionName + "\"><img name=\"jsbutton\" id=\"jsbutton\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/go_button.gif\" border=\"0\" alt=\"" + search_collection + "\" /></a>");
                Output.WriteLine("            </td>");
                Output.WriteLine("          </tr>");
                Output.WriteLine("        </table>");
                Output.WriteLine("      </div>");
                Output.WriteLine("    </td>");
            }


            Output.WriteLine("  </tr>");
            Output.WriteLine("</table>");

            //if ((currentUser != null) && (currentUser.Is_System_Admin))
            //{
            //    Output.WriteLine("    <tr align=\"right\">");
            //    Output.WriteLine("      <td>&nbsp;</td>");
            //    Output.WriteLine("      <td align=\"left\" colspan=\"4\">");
            //    Output.WriteLine("          &nbsp; &nbsp; &nbsp; &nbsp; <input type=\"checkbox\" value=\"PRIVATE_ITEMS\" name=\"privatecheck\" id=\"privatecheck\" unchecked onclick=\"focus_element( 'SobekHomeSearchBox');\" /><label for=\"privatecheck\">" + include_privates + "</label>");
            //    Output.WriteLine("      </td>");
            //    Output.WriteLine("    </tr>");
            //}

            Output.WriteLine();
            Output.WriteLine("<!-- Focus on search box -->");
            Output.WriteLine("<script type=\"text/javascript\">focus_element('SobekHomeBannerSearchBox');</script>");
            Output.WriteLine();
        }
    }
}
