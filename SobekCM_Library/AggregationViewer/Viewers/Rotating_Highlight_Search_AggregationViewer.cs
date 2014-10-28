#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
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
	    private readonly Item_Aggregation_Front_Banner frontBannerInfo;

        /// <summary> Constructor for a new instance of the Basic_Search_AggregationViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        public Rotating_Highlight_Search_AggregationViewer(RequestCache RequestSpecificValues) : base(RequestSpecificValues)
        {
            // Determine the sub text to use
            const string SUB_CODE = "s=";
            Sharing_Buttons_HTML = String.Empty;

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
            RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Browse_Info;
            RequestSpecificValues.Current_Mode.Info_Browse_Mode = "all";
            browse_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
            RequestSpecificValues.Current_Mode.Info_Browse_Mode = String.Empty;
            RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Home;
            if ((!RequestSpecificValues.Current_Mode.Show_Selection_Panel) || (RequestSpecificValues.Hierarchy_Object.Children_Count == 0))
            {
                Search_Script_Action = "basic_search_sobekcm('" + arg1 + "', '" + browse_url + "')";
             }
            else
            {
                Search_Script_Action = "basic_select_search_sobekcm('" + arg1 + "', '" + SUB_CODE + "')";
                arg2 = SUB_CODE;
             }
            RequestSpecificValues.Current_Mode.Mode = displayMode;
	        RequestSpecificValues.Current_Mode.Aggregation_Type = aggrType;
            RequestSpecificValues.Current_Mode.Search_Type = searchType;
            RequestSpecificValues.Current_Mode.Search_String = search_string;

			// Get the front banner
	        frontBannerInfo = RequestSpecificValues.Hierarchy_Object.Front_Banner_Image(RequestSpecificValues.Current_Mode.Language);
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
            if (RequestSpecificValues.Current_Mode.Language == Web_Language_Enum.Spanish)
            {
                search_collection = "Buscar en la colección";
            }

            if (RequestSpecificValues.Current_Mode.Language == Web_Language_Enum.French)
            {
                search_collection = "Recherche dans la collection";
            }

            if (RequestSpecificValues.Hierarchy_Object.Highlights.Count > 1)
            {
                Output.WriteLine("<script type=\"text/javascript\" src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "default/scripts/contentslider.js\" > </script>");
                Output.WriteLine("<!-- *****************************************");
                Output.WriteLine( "    * Featured Content Slider- (c) Dynamic Drive DHTML code library (www.dynamicdrive.com)");
                Output.WriteLine("     * This notice MUST stay intact for legal use");
                Output.WriteLine("     * Visit Dynamic Drive at http://www.dynamicdrive.com/ for this script and 100s more");
                Output.WriteLine("     *********************************************** -->");
            }

			Output.WriteLine("<div id=\"sbkRhav_BannerBack\">");
            Output.WriteLine("  <table class=\"sbkRhav_RotatingBanner\">");
            Output.WriteLine("    <tr>");



			if (frontBannerInfo.Banner_Type == Item_Aggregation_Front_Banner.Item_Aggregation_Front_Banner_Type.LEFT)
			{
				string banner_image = RequestSpecificValues.Current_Mode.Base_URL + "design/" + RequestSpecificValues.Hierarchy_Object.ObjDirectory + frontBannerInfo.Image_File.Replace("\\", "/");
				Output.Write("      <td class=\"sbkRhav_RotatingBannerLeft\" style=\"background-image: url( " + banner_image + "); width: " + frontBannerInfo.Width + "px; height: " + frontBannerInfo.Height + "px;\">");
                Output.WriteLine(Sharing_Buttons_HTML.Replace("span", "div"));
                Output.WriteLine("");
				Output.WriteLine("        <div class=\"sbkRhav_RotatingBannerLeftSearch\">");
                Output.WriteLine("          <table>");
                Output.WriteLine("            <tr>");
				Output.WriteLine("              <td colspan=\"2\" class=\"sbkRhav_SearchPrompt\">");
                Output.WriteLine("                 <label for=\"SobekHomeBannerSearchBox\">" + search_collection + ":</label>");
                Output.WriteLine("              </td>");
                Output.WriteLine("            </tr>");
                Output.WriteLine("            <tr style=\"vertical-align:bottom\">");
                Output.WriteLine("              <td><input name=\"u_search\" type=\"text\" id=\"SobekHomeBannerSearchBox\" class=\"sbkRhav_SearchBox sbk_Focusable\" value=\"" + textBoxValue + "\" onkeydown=\"return fnTrapKD(event, 'basic', '" + arg1 + "', '" + arg2 + "','" + browse_url + "');\" /></td>");
                Output.WriteLine("              <td><button class=\"sbk_GoButton\" title=\"" + search_collection + "\" onclick=\"" + Search_Script_Action + ";return false;\">Go</button></td>");
                Output.WriteLine("            </tr>");
                Output.WriteLine("          </table>");
                Output.WriteLine("        </div>");
                Output.WriteLine("      </td>");
            }

            Output.WriteLine("      <td>");
            string base_design_location = RequestSpecificValues.Current_Mode.Base_URL + "design/aggregations/" + RequestSpecificValues.Current_Mode.Aggregation + "/";
            if (RequestSpecificValues.Current_Mode.Aggregation.Length == 0)
                base_design_location = RequestSpecificValues.Current_Mode.Base_URL + "design/aggregations/all/";

            // Either draw all the highlights to flip through, or just the one highlight
            if (RequestSpecificValues.Hierarchy_Object.Highlights.Count > 1)
            {
                int width = 754 - frontBannerInfo.Width;

                Output.WriteLine("        <div id=\"slider1\" class=\"sliderwrapper\" style=\"width:" + width + "px\">");

                // Copy over just the eight highlights we should use 
                List<Item_Aggregation_Highlights> highlights_to_use = new List<Item_Aggregation_Highlights>();
                int day_integer = DateTime.Now.DayOfYear + 1;
                int highlight_to_use = day_integer % RequestSpecificValues.Hierarchy_Object.Highlights.Count;
                int number = Math.Min(8, RequestSpecificValues.Hierarchy_Object.Highlights.Count);
                for (int i = 0; i < number; i++)
                {
                    highlights_to_use.Add(RequestSpecificValues.Hierarchy_Object.Highlights[highlight_to_use]);
                    highlight_to_use++;
                    if (highlight_to_use >= RequestSpecificValues.Hierarchy_Object.Highlights.Count)
                        highlight_to_use = 0;
                }

                foreach (Item_Aggregation_Highlights highlight in highlights_to_use)
                {
                    Output.WriteLine("          <div class=\"contentdiv\" style=\"width:" + width + "px\">");
                    string highlight_text = HttpUtility.HtmlEncode(highlight.Get_Tooltip(RequestSpecificValues.Current_Mode.Language));
                    if (highlight.Link.Length > 0)
                    {
                        Output.WriteLine("            <a href=\"" + highlight.Link + "\" title=\"" + highlight_text + "\" >");
                    }
                    if (highlight.Image.IndexOf("http:") >= 0)
                        Output.WriteLine("              <img src=\"" + highlight.Image + "\" border=\"0px\" title=\"" + highlight_text + "\" alt=\"" + highlight_text + "\" />");
                    else
                        Output.WriteLine("              <img src=\"" + base_design_location + highlight.Image + "\" border=\"0px\" title=\"" + highlight_text + "\" alt=\"" + highlight_text + "\" />");
                    if (highlight.Link.Length > 0)
                    {
                        Output.WriteLine("            </a>");
                    }
                    Output.WriteLine("          </div>");
                }
                Output.WriteLine("        </div>");
                int pagination_width = 735 - frontBannerInfo.Width;
                Output.WriteLine("        <div id=\"paginate-slider1\" class=\"pagination\" style=\"width:" + pagination_width + "px\" >  </div>");



                Output.WriteLine("        <script type=\"text/javascript\">");
                Output.WriteLine("            featuredcontentslider.init({");
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
                Output.WriteLine("            })");
                Output.WriteLine("        </script>");
            }
            else
            {
                if (RequestSpecificValues.Hierarchy_Object.Highlights.Count > 0)
                {
                    Output.WriteLine("            <a href=\"" + RequestSpecificValues.Hierarchy_Object.Highlights[0].Link + "\" title=\"" + HttpUtility.HtmlEncode(RequestSpecificValues.Hierarchy_Object.Highlights[0].Get_Text(RequestSpecificValues.Current_Mode.Language)) + "\" >");
                    if (RequestSpecificValues.Hierarchy_Object.Highlights[0].Image.IndexOf("http:") >= 0)
                        Output.WriteLine("              <img src=\"" + RequestSpecificValues.Hierarchy_Object.Highlights[0].Image + "\" />");
                    else
                        Output.WriteLine("              <img src=\"" + base_design_location + RequestSpecificValues.Hierarchy_Object.Highlights[0].Image + "\" />");
                    Output.WriteLine("            </a>");
                }
            }


            Output.WriteLine("      </td>");


            if (frontBannerInfo.Banner_Type == Item_Aggregation_Front_Banner.Item_Aggregation_Front_Banner_Type.RIGHT)
            {
				string banner_image = RequestSpecificValues.Current_Mode.Base_URL + "design/" + RequestSpecificValues.Hierarchy_Object.ObjDirectory + frontBannerInfo.Image_File.Replace("\\", "/"); 
				Output.WriteLine("      <td class=\"sbkRhav_RotatingBannerRight\" style=\"background-image: url( " + banner_image + "); width: " + frontBannerInfo.Width + "px; height: " + frontBannerInfo.Height + "px;\">");
                Output.WriteLine(Sharing_Buttons_HTML.Replace("span", "div"));
                Output.WriteLine("");
				Output.WriteLine("        <div class=\"sbkRhav_RotatingBannerRightSearch\">");
                Output.WriteLine("          <table>");
                Output.WriteLine("            <tr>");
                Output.WriteLine("              <td colspan=\"2\" class=\"sbkRhav_SearchPrompt\">");
                Output.WriteLine("                 <label for=\"SobekHomeBannerSearchBox\">" + search_collection + ":</label>");
                Output.WriteLine("              </td>");
                Output.WriteLine("            </tr>");
                Output.WriteLine("            <tr style=\"vertical-align:bottom\">");
                Output.WriteLine("              <td><input name=\"u_search\" type=\"text\" id=\"SobekHomeBannerSearchBox\" class=\"sbkRhav_SearchBox sbk_Focusable\" value=\"" + textBoxValue + "\" onkeydown=\"return fnTrapKD(event, 'basic', '" + arg1 + "', '" + arg2 + "','" + browse_url + "');\" /></td>");
				Output.WriteLine("              <td><button class=\"sbk_GoButton\" title=\"" + search_collection + "\" onclick=\"" + Search_Script_Action + ";return false;\">Go</button></td>");
                Output.WriteLine("            </tr>");
                Output.WriteLine("          </table>");
                Output.WriteLine("        </div>");
                Output.WriteLine("      </td>");
            }


            Output.WriteLine("    </tr>");
            Output.WriteLine("  </table>");
			Output.WriteLine("</div>");

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
