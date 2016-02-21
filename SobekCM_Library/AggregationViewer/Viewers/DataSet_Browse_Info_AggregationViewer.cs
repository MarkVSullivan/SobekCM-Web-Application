#region Using directives

using System.Collections.Generic;
using System.IO;
using System.Web.UI.WebControls;
using SobekCM.Core.Aggregations;
using SobekCM.Core.Navigation;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.AggregationViewer.Viewers
{
    /// <summary> Renders the item list matching a browse or search against an item aggregation </summary>
    /// <remarks> This class implements the <see cref="iAggregationViewer"/> interface and extends the <see cref="abstractAggregationViewer"/> class.<br /><br />
    /// Aggregation viewers are used when displaying aggregation home pages, searches, browses, and information pages.<br /><br />
    /// During a valid html request to display items matching a browse or search, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the Application_State_Builder </li>
    /// <li>Request is analyzed by the QueryString_Analyzer and output as a <see cref="Navigation_Object"/>  </li>
    /// <li>Main writer is created for rendering the output, in this case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  For a collection-level request, an instance of the  <see cref="Aggregation_HtmlSubwriter"/> class is created. </li>
    /// <li>To display the requested collection view, the collection subwriter will creates an instance of this class </li>
    /// <li>To display the actual results, this class will create an instance of the <see cref="PagedResults_HtmlSubwriter"/> class</li>
    /// <li>That subwriter creates its own results viewer which extends the <see cref="ResultsViewer.abstract_ResultsViewer"/> class </li>
    /// </ul></remarks>
    public class DataSet_Browse_Info_AggregationViewer : abstractAggregationViewer
    {
        private PagedResults_HtmlSubwriter writeResult;


        /// <summary> Constructor for a new instance of the DataSet_Browse_Info_AggregationViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        /// <param name="ViewBag"> Aggregation-specific request information, such as aggregation object and any browse object requested </param>
        public DataSet_Browse_Info_AggregationViewer(RequestCache RequestSpecificValues, AggregationViewBag ViewBag)
            : base(RequestSpecificValues, ViewBag)
        {
            // Do nothing
        }

        /// <summary> Gets the type of collection view or search supported by this collection viewer </summary>
        /// <value> This returns the <see cref="Item_Aggregation_Views_Searches_Enum.DataSet_Browse"/> enumerational value </value>
        public override Item_Aggregation_Views_Searches_Enum Type
        {
            get { return Item_Aggregation_Views_Searches_Enum.DataSet_Browse; }
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

        /// <summary> Gets the collection of special behaviors which this aggregation viewer  requests from the main HTML subwriter. </summary>
        public override List<HtmlSubwriter_Behaviors_Enum> AggregationViewer_Behaviors
        {
            get
            {
                return new List<HtmlSubwriter_Behaviors_Enum>
                        {
                            HtmlSubwriter_Behaviors_Enum.Aggregation_Suppress_Home_Text,
                            HtmlSubwriter_Behaviors_Enum.Suppress_SearchForm
                        };
            }
        }

        /// <summary> Gets flag which indicates whether the secondary text requires controls </summary>
        /// <value> This property always returns the value TRUE</value>
        public override bool Secondary_Text_Requires_Controls
        {
            get
            {
                return true;
            }
        }

        /// <summary> Add the HTML to be displayed in the search box </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Add_Search_Box_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("DataSet_Browse_Info_AggregationViewer.Write_HTML", "Writing HTML from result_dataset_html_subwriter ");
            }

            writeResult.Write_HTML(Output, Tracer);

        }

        /// <summary> Add controls to the placeholder below the search box </summary>
        /// <param name="MainPlaceHolder"> Placeholder into which to place controls to be rendered</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This adds the results from the dataset into the space below the search box.<br /><br />
        /// This creates and uses a <see cref="PagedResults_HtmlSubwriter"/> to write the results. </remarks>
        public override void Add_Secondary_Controls(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("DataSet_Browse_Info_AggregationViewer.Add_Secondary_Controls", "Adding HTML");
            }

            writeResult = new PagedResults_HtmlSubwriter(RequestSpecificValues) { Browse_Title = ViewBag.Browse_Object.Label };
            writeResult.Add_Controls(MainPlaceHolder, Tracer);


			//if ( resultsStatistics.Total_Items > 0)
			//{
			//	Literal literal = new Literal
			//						  {
			//							  Text = "<div class=\"sbkPrsw_ResultsNavBar\">" + Environment.NewLine + "  " + writeResult.Buttons + "" + Environment.NewLine + "  " + writeResult.Showing_Text + Environment.NewLine + "</div>" + Environment.NewLine + "<br />" + Environment.NewLine 
			//						  };
			//	MainPlaceHolder.Controls.Add(literal);
			//}
        }

    }
}
