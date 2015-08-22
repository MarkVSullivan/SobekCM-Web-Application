using System;
using System.IO;
using SobekCM.Core;
using SobekCM.Core.Client;
using SobekCM.Core.Navigation;
using SobekCM.Core.WebContent.Single;
using SobekCM.Library.Settings;
using SobekCM.Tools;

namespace SobekCM.Library.WebContentViewer.Viewers
{
    /// <summary> Web content viewer shows the change log of all updates performed against a web content page or redirect </summary>
    /// <remarks> This viewer extends the <see cref="abstractWebContentViewer" /> abstract class and implements the <see cref="iWebContentViewer"/> interface. </remarks>
    public class Work_History_WebContentViewer: abstractWebContentViewer
    {
        /// <summary>  Constructor for a new instance of the Work_History_WebContentViewer class  </summary>
        /// <param name="RequestSpecificValues">  All the necessary, non-global data specific to the current request  </param>
        public Work_History_WebContentViewer(RequestCache RequestSpecificValues) : base ( RequestSpecificValues )
        {
            
        }


        /// <summary> Gets the type of specialized web content viewer </summary>
        public override WebContent_Type_Enum Type { get { return WebContent_Type_Enum.Milestones; }}

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        public override string Viewer_Title
        {
            get { return "Web Content Change Log"; }
        }

        /// <summary> Gets the URL for the icon related to this web content viewer task </summary>
        public override string Viewer_Icon
        {
            get { return Static_Resources.View_Work_Log_Img; }
        }

        /// <summary> Add the HTML to be displayed </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Add_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Work_History_WebContentViewer.Add_HTML", "No html added");
            }

            Output.WriteLine("<div class=\"Wchs_Text\">");
            Output.WriteLine("  <p>The list of changes, including the user that performed the change, appear below.</p>");
            Output.WriteLine("</div>");

            // Get the stats
            int webcontentid = -1;
            if (RequestSpecificValues.Current_Mode.WebContentID.HasValue)
                webcontentid = RequestSpecificValues.Current_Mode.WebContentID.Value;

            Single_WebContent_Change_Report stats;
            try
            {
                stats = SobekEngineClient.WebContent.Get_Single_Milestones(webcontentid, Tracer);
            }
            catch (Exception ee)
            {
                Output.WriteLine("<div id=\"apiExceptionMsg\">Exception caught: " + ee.Message + "</div>");
                return;
            }


            // If no stats, show a message for that
            if ((stats == null) || (stats.Changes == null) || (stats.Changes.Count == 0))
            {
                Output.WriteLine("<div id=\"sbkWchs_NoDataMsg\">No change history</div>");
                return;
            }

            Output.WriteLine("  <br />");
            Output.WriteLine("  <table class=\"sbkStatsTbl\" style=\"width: 500px; margin-left:auto; margin-right: auto;\">");
            Output.WriteLine("    <tr>");
            Output.WriteLine("      <th style=\"text-align:left;\">DATE</th>");
            Output.WriteLine("      <th>USER</th>");
            Output.WriteLine("      <th>CHANGE</th>");
            Output.WriteLine("    </tr>");


            // Add the usage data rows
            foreach (Milestone_Entry change in stats.Changes)
            {
                // Add the data row
                Output.WriteLine("    <tr>");
                Output.WriteLine("      <td style=\"text-align:left;\">" + change.MilestoneDate + "</td>");
                Output.WriteLine("      <td>" + change.User + "</td>");
                Output.WriteLine("      <td>" + change.Notes + "</td>");
                Output.WriteLine("    </tr>");
            }
            Output.WriteLine("  </table>");

            Output.WriteLine("  <br /> <br />");
        }
    }
}
