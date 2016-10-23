#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using SobekCM.Core.Aggregations;
using SobekCM.Core.Navigation;
using SobekCM.Core.UI_Configuration;
using SobekCM.Core.UI_Configuration.StaticResources;
using SobekCM.Engine_Library.Configuration;
using SobekCM.Engine_Library.Database;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.AggregationViewer.Viewers
{
    /// <summary> Renders the item count information for a single aggregation </summary>
    /// <remarks> This class implements the <see cref="iAggregationViewer"/> interface and extends the <see cref="abstractAggregationViewer"/> class.<br /><br />
    /// Aggregation viewers are used when displaying aggregation home pages, searches, browses, and information pages.<br /><br />
    /// During a valid html request to display the item count information, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the Application_State_Builder </li>
    /// <li>Request is analyzed by the QueryString_Analyzer and output as a <see cref="Navigation_Object"/>  </li>
    /// <li>Main writer is created for rendering the output, in this case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  For a collection-level request, an instance of the  <see cref="Aggregation_HtmlSubwriter"/> class is created. </li>
    /// <li>To display the requested collection view, the collection subwriter will creates an instance of this class </li>
    /// </ul></remarks>
    public class Item_Count_AggregationViewer : abstractAggregationViewer
    {

        /// <summary> Constructor for a new instance of the Item_Count_AggregationViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        /// <param name="ViewBag"> Aggregation-specific request information, such as aggregation object and any browse object requested </param>
        public Item_Count_AggregationViewer(RequestCache RequestSpecificValues, AggregationViewBag ViewBag)
            : base(RequestSpecificValues, ViewBag)
        {
            // All work done in the base constructor
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
                            HtmlSubwriter_Behaviors_Enum.Aggregation_Suppress_Home_Text
                        };
            }
        }

        /// <summary> Gets the type of collection view or search supported by this collection viewer </summary>
        /// <value> This returns the <see cref="Item_Aggregation_Views_Searches_Enum.Item_Count"/> enumerational value </value>
        public override Item_Aggregation_Views_Searches_Enum Type
        {
            get { return Item_Aggregation_Views_Searches_Enum.Item_Count; }
        }

        /// <summary> Gets flag which indicates whether this is an internal view, which may have a 
        /// slightly different design feel </summary>
        /// <remarks> This returns FALSE by default, but can be overriden by individual viewer implementations</remarks>
        public override bool Is_Internal_View
        {
            get { return true; }
        }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        public override string Viewer_Title
        {
            get { return "Resource Count in Collection"; }
        }

        /// <summary> Gets the URL for the icon related to this aggregational viewer task </summary>
        public override string Viewer_Icon
        {
            get { return Static_Resources_Gateway.Item_Count_Img; }
        }

        /// <summary> Add the HTML to be displayed in the search box </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This adds the title of the into the box </remarks>
        public override void Add_Search_Box_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            // Do nothing
        }

        /// <summary> Add the HTML to be displayed below the search box </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This writes the HTML from the static browse or info page here  </remarks>
        public override void Add_Secondary_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Item_Count_AggregationViewer.Add_Secondary_HTML", "Adding HTML");
            }

            DataTable value = Engine_Database.Tracking_Get_Milestone_Report(ViewBag.Hierarchy_Object.Code, Tracer);

            Output.WriteLine("<div class=\"SobekText\">");
            Output.WriteLine("<br />");
            Output.WriteLine("<p>Below is the number of titles and items for all items within this aggregation, including currently online items as well as items in process.</p>");
            Output.WriteLine("<br />");

            Output.WriteLine("</div>");
            // Start the table
            Output.WriteLine("<table width=\"700px\" border=\"0px\" cellspacing=\"0px\" class=\"statsTable\">");
            Output.WriteLine("  <tr align=\"left\" bgcolor=\"#0022a7\">");
            Output.WriteLine("    <th align=\"left\"><span style=\"color: White\"><b>LAST MILESTONE</b></span></th>");
            Output.WriteLine("    <th align=\"left\"><span style=\"color: White\"><b>TITLE COUNT</b></span></th>");
            Output.WriteLine("    <th align=\"left\"><span style=\"color: White\"><b>ITEM COUNT</b></span></th>");
            Output.WriteLine("    <th align=\"left\"><span style=\"color: White\"><b>PAGE COUNT</b></span></th>");
            Output.WriteLine("    <th align=\"left\"><span style=\"color: White\"><b>FILE COUNT</b></span></th>");
            Output.WriteLine("  </tr>");

            foreach( DataRow thisRow in value.Rows )
            {
                Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"5\"></td></tr>");

                Output.WriteLine("  <tr align=\"left\">");
                Output.WriteLine("    <td>" + thisRow[0] + "</td>");
                Output.WriteLine("    <td>" + Int_To_Comma_String( Convert.ToInt32(thisRow[1])) + "</td>");
                Output.WriteLine("    <td>" + Int_To_Comma_String(Convert.ToInt32(thisRow[2])) + "</td>");
                if ( thisRow[3] != DBNull.Value )
                    Output.WriteLine("    <td>" + Int_To_Comma_String(Convert.ToInt32(thisRow[3])) + "</td>");
                else
                    Output.WriteLine("    <td>0</td>");
                if ( thisRow[4] != DBNull.Value )
                    Output.WriteLine("    <td>" + Int_To_Comma_String(Convert.ToInt32(thisRow[4])) + "</td>");
                else
                    Output.WriteLine("    <td>0</td>");
                Output.WriteLine("  </tr>");
            }

            // End the table
            Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"5\"></td></tr>");
            Output.WriteLine("</table>");

            Output.WriteLine("<br />");
            Output.WriteLine("<br />");
        }

        private static string Int_To_Comma_String(int Value)
        {
            if (Value < 1000)
                return Value.ToString();

            string valueString = Value.ToString();
            if ((Value >= 1000) && (Value < 1000000))
            {
                return valueString.Substring(0, valueString.Length - 3) + "," + valueString.Substring(valueString.Length - 3);
            }

            return valueString.Substring(0, valueString.Length - 6) + "," + valueString.Substring(valueString.Length - 6, 3) + "," + valueString.Substring(valueString.Length - 3);
        }
    }
}
