#region Using directives

using System;
using System.Data;
using System.IO;
using SobekCM.Library.Aggregations;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.Navigation;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.AggregationViewer.Viewers
{
    /// <summary> Renders the item count information for a single aggregation </summary>
    /// <remarks> This class implements the <see cref="iAggregationViewer"/> interface and extends the <see cref="abstractAggregationViewer"/> class.<br /><br />
    /// Aggregation viewers are used when displaying aggregation home pages, searches, browses, and information pages.<br /><br />
    /// During a valid html request to display the item count information, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="Navigation.SobekCM_Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in this case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  For a collection-level request, an instance of the  <see cref="Aggregation_HtmlSubwriter"/> class is created. </li>
    /// <li>To display the requested collection view, the collection subwriter will creates an instance of this class </li>
    /// </ul></remarks>
    public class Item_Count_AggregationViewer : abstractAggregationViewer
    {

        /// <summary> Constructor for a new instance of the Item_Count_AggregationViewer class </summary>
        /// <param name="Current_Mode"> Mode / navigation information for the current request</param>
        /// <param name="Current_Aggregation"> Current item aggregation object to display </param>
        public Item_Count_AggregationViewer(SobekCM_Navigation_Object Current_Mode, Item_Aggregation Current_Aggregation ): base(Current_Aggregation, Current_Mode)
        {
            // All work done in the base constructor
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

        /// <summary> Gets the type of collection view or search supported by this collection viewer </summary>
        /// <value> This returns the <see cref="Item_Aggregation.CollectionViewsAndSearchesEnum.Item_Count"/> enumerational value </value>
        public override Item_Aggregation.CollectionViewsAndSearchesEnum Type
        {
            get { return Item_Aggregation.CollectionViewsAndSearchesEnum.Item_Count; }
        }

        /// <summary> Add the HTML to be displayed in the search box </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This adds the title of the into the box </remarks>
        public override void Add_Search_Box_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Output.WriteLine("<h1>Resource Count in Aggregation</h1>");
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

            DataTable value = SobekCM_Database.Tracking_Get_Milestone_Report(currentCollection.Code, Tracer);

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
                Output.WriteLine("    <td>" + Int_To_Comma_String(Convert.ToInt32(thisRow[3])) + "</td>");
                Output.WriteLine("    <td>" + Int_To_Comma_String(Convert.ToInt32(thisRow[4])) + "</td>");
                Output.WriteLine("  </tr>");
            }

            // End the table
            Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"5\"></td></tr>");
            Output.WriteLine("</table>");

            Output.WriteLine("<br />");
            Output.WriteLine("<br />");
        }

        private static string Int_To_Comma_String(int value)
        {
            if (value < 1000)
                return value.ToString();

            string valueString = value.ToString();
            if ((value >= 1000) && (value < 1000000))
            {
                return valueString.Substring(0, valueString.Length - 3) + "," + valueString.Substring(valueString.Length - 3);
            }

            return valueString.Substring(0, valueString.Length - 6) + "," + valueString.Substring(valueString.Length - 6, 3) + "," + valueString.Substring(valueString.Length - 3);
        }
    }
}
