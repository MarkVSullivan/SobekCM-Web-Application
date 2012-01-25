#region Using directives

using System.IO;
using SobekCM.Library.Aggregations;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.WebContent;

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
        private readonly Item_Aggregation_Browse_Info browseObject;
        private readonly HTML_Based_Content staticWebContent;

        /// <summary> Constructor for a new instance of the Static_Browse_Info_AggregationViewer class </summary>
        /// <param name="Browse_Object"> Browse or information object to be displayed </param>
        /// <param name="Static_Web_Content"> HTML content-based browse, info, or imple CMS-style web content objects.  These are objects which are read from a static HTML file and much of the head information must be maintained </param>
        public Static_Browse_Info_AggregationViewer(Item_Aggregation_Browse_Info Browse_Object, HTML_Based_Content Static_Web_Content ):base(null, null)
        {
            browseObject = Browse_Object;
            staticWebContent = Static_Web_Content;
        }

        /// <summary> Gets the type of collection view or search supported by this collection viewer </summary>
        /// <value> This returns the <see cref="Item_Aggregation.CollectionViewsAndSearchesEnum.Static_Browse_Info"/> enumerational value </value>
        public override Item_Aggregation.CollectionViewsAndSearchesEnum Type
        {
            get { return Item_Aggregation.CollectionViewsAndSearchesEnum.Static_Browse_Info; }
        }

        /// <summary> Gets flag which indicates whether to always use the home text as the secondary text </summary>
        public override bool Always_Display_Home_Text
        {
            get
            {
                return false;
            }
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

            Output.WriteLine("  <h1>" + browseObject.Get_Label(currentMode.Language) + "</h1>");
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

            Output.WriteLine("<div class=\"SobekResultsPanel\" align=\"center\">");
            
            // Get the adjusted text for this user's session
            string static_browse_info_text = staticWebContent.Apply_Settings_To_Static_Text(staticWebContent.Static_Text, currentCollection, htmlSkin.Skin_Code, htmlSkin.Base_Skin_Code, currentMode.Base_URL, currentMode.URL_Options(), Tracer);

            //Output this static, adjusted text
            Output.WriteLine(static_browse_info_text);
            

            Output.WriteLine("</div>");
            Output.WriteLine("<br />");
            Output.WriteLine();
        }
    }
}
