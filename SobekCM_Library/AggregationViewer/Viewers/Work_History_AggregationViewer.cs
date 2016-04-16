#region Using directives

using System;
using System.Data;
using System.IO;
using SobekCM.Core.Aggregations;
using SobekCM.Core.Navigation;
using SobekCM.Core.UI_Configuration;
using SobekCM.Core.UI_Configuration.StaticResources;
using SobekCM.Engine_Library.Configuration;
using SobekCM.Library.AggregationViewer.Viewers;
using SobekCM.Library.Database;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.AggregationViewer
{
    /// <summary> Aggregation viewer displays all the work history related to an item aggregation </summary>
    public class Work_History_AggregationViewer : abstractAggregationViewer
    {
        /// <summary> Constructor for a new instance of the Work_History_AggregationViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        /// <param name="ViewBag"> Aggregation-specific request information, such as aggregation object and any browse object requested </param>
        public Work_History_AggregationViewer(RequestCache RequestSpecificValues, AggregationViewBag ViewBag)
            : base(RequestSpecificValues, ViewBag)
        {
            // User must AT LEAST be logged on, return
            if ((RequestSpecificValues.Current_User == null) || (!RequestSpecificValues.Current_User.LoggedOn))
            {
                RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Home;
                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                return;
            }

            // If the user is not an admin of some type, also return
            if ((!RequestSpecificValues.Current_User.Is_System_Admin) && (!RequestSpecificValues.Current_User.Is_Portal_Admin) && (!RequestSpecificValues.Current_User.Is_Aggregation_Curator(ViewBag.Hierarchy_Object.Code)))
            {
                RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Home;
                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
            }
        }

        /// <summary> Gets the type of collection view or search supported by this collection viewer </summary>
        public override Item_Aggregation_Views_Searches_Enum Type
        {
            get { return Item_Aggregation_Views_Searches_Enum.Work_History; }
        }

        /// <summary> Flag which indicates whether the selection panel should be displayed </summary>
        /// <value> This defaults to <see cref="Selection_Panel_Display_Enum.Selectable"/> but is overwritten by most collection viewers </value>
        public override Selection_Panel_Display_Enum Selection_Panel_Display
        {
            get { return Selection_Panel_Display_Enum.Never; }
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
            get { return "Collection Change Log"; }
        }

        /// <summary> Gets the URL for the icon related to this aggregational viewer task </summary>
        public override string Viewer_Icon
        {
            get { return Static_Resources_Gateway.View_Work_Log_Img; }
        }

        /// <summary> Add the HTML to be displayed in the search box </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This does nothing - as an internal type view, this will not be called </remarks>
        public override void Add_Search_Box_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            // Do nothing
        }

        /// <summary> Add the main HTML to be added to the page </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This writes the HTML from the static browse or info page here  </remarks>
        public override void Add_Secondary_HTML(TextWriter Output, Custom_Tracer Tracer)
        {

            DataTable historyTbl = SobekCM_Database.Get_Aggregation_Change_Log(ViewBag.Hierarchy_Object.Code, RequestSpecificValues.Tracer);

            if ((historyTbl == null) || ( historyTbl.Rows.Count == 0 ))
            {
                Output.WriteLine("<p>No history found for this collection!</p>");

                Output.WriteLine("<p>This may be due to an error, or this may be a legacy collection which has not been edited in a very long time.</p>");

                return;
            }

            Output.WriteLine("<p style=\"text-align: left; padding:0 20px 0 20px;\">Below is the change log for this collection and the design files under this collection.  This does not include the history of digital reources loaded into this collection.</p>");

            Output.WriteLine("  <table class=\"sbkWhav_Table\">");
            Output.WriteLine("    <tr>");
            Output.WriteLine("      <th style=\"width:100px;\">Date</th>");
            Output.WriteLine("      <th style=\"width:180px;\">User</th>");
            Output.WriteLine("      <th style=\"width:500px;\">Change Description</th>");
            Output.WriteLine("    </tr>");

            foreach (DataRow thisChange in historyTbl.Rows)
            {
                Output.WriteLine("    <tr>");
                Output.WriteLine("      <td>" + Convert.ToDateTime(thisChange[1]).ToShortDateString() + "</td>");
                Output.WriteLine("      <td>" + thisChange[2] + "</td>");
                Output.WriteLine("      <td>" + thisChange[0].ToString().Replace("\n","<br />") + "</td>");
                Output.WriteLine("    </tr>");
                Output.WriteLine("    <tr class=\"sbkWhav_TableRule\"><td colspan=\"3\"></td></tr>");
            }

            Output.WriteLine("  </table>");
            Output.WriteLine("  <br /><br />");
        }
    }
}
