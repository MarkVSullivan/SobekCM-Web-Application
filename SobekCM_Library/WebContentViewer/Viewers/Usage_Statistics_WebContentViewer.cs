using System;
using System.IO;
using SobekCM.Core.Client;
using SobekCM.Core.Navigation;
using SobekCM.Core.UI_Configuration;
using SobekCM.Core.WebContent.Single;
using SobekCM.Tools;

namespace SobekCM.Library.WebContentViewer.Viewers
{
    /// <summary> Web content viewer shows the historic usage for this web content page </summary>
    /// <remarks> This viewer extends the <see cref="abstractWebContentViewer" /> abstract class and implements the <see cref="iWebContentViewer"/> interface. </remarks>
    public class Usage_Statistics_WebContentViewer : abstractWebContentViewer
    {
        /// <summary>  Constructor for a new instance of the Usage_Statistics_WebContentViewer class  </summary>
        /// <param name="RequestSpecificValues">  All the necessary, non-global data specific to the current request  </param>
        public Usage_Statistics_WebContentViewer(RequestCache RequestSpecificValues) : base ( RequestSpecificValues )
        {
            
        }


        /// <summary> Gets the type of specialized web content viewer </summary>
        public override WebContent_Type_Enum Type { get { return WebContent_Type_Enum.Usage; }}

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        public override string Viewer_Title
        {
            get { return "Web Content Usage Statistics"; }
        }

        /// <summary> Gets the URL for the icon related to this web content viewer task </summary>
        public override string Viewer_Icon
        {
            get { return Static_Resources.Usage_Img; }
        }

        /// <summary> Add the HTML to be displayed </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Add_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Usage_Statistics_WebContentViewer.Add_HTML", "No html added");
            }

            Output.WriteLine("<div class=\"Wchs_Text\">");
            Output.WriteLine("  <p>Usage statistics for this web content page (or rediect) over time appears below, broken down by month and year.");
            Output.WriteLine("     Views are the number of times this page was requested.  Hierarchical views includes the hits on this page, as well as all the child pages.");
            Output.WriteLine("     Usage statistics are collected from the web logs by the SobekCM builder in a regular, monthly automated process.</p>");
            Output.WriteLine("</div>");

            // Get the stats
            int webcontentid = -1;
            if (RequestSpecificValues.Current_Mode.WebContentID.HasValue)
                webcontentid = RequestSpecificValues.Current_Mode.WebContentID.Value;

            Single_WebContent_Usage_Report stats;
            try
            {
                stats = SobekEngineClient.WebContent.Get_Single_Usage_Report(webcontentid, Tracer);
            }
            catch (Exception ee)
            {
                Output.WriteLine("<div id=\"apiExceptionMsg\">Exception caught: " + ee.Message + "</div>");
                return;
            }


            // If no stats, show a message for that
            if ((stats == null) || (stats.Usage == null) || (stats.Usage.Count == 0))
            {
                Output.WriteLine("<div id=\"sbkWchs_NoDataMsg\">No usage statistics collected</div>");
                return;
            }

            Output.WriteLine("  <br />");
            Output.WriteLine("  <table class=\"sbkStatsTbl\" style=\"width: 450px; margin-left:auto; margin-right: auto;\">");
            Output.WriteLine("    <tr>");
            Output.WriteLine("      <th style=\"text-align:left;\">DATE</th>");
            Output.WriteLine("      <th>VIEWS</th>");
            Output.WriteLine("      <th>HIERARCHICAL VIEWS</th>");
            Output.WriteLine("    </tr>");

            int totalHits = 0;
            int totalHierarchicalHits = 0;
            int lastYear = -1;
            const int COLUMNS = 3;

            // Add the usage data rows
            foreach (Single_WebContent_Usage usage in stats.Usage)
            {
                // If this is a new year, add that year sub heading
                if (usage.Year != lastYear)
                {
                    Output.WriteLine("    <tr style=\"text-align:left !important;\"><td class=\"sbkStatsTblSubHeading\" colspan=\"" + COLUMNS + "\">" + usage.Year + " STATISTICS</td></tr>");
                    lastYear = usage.Year;
                }

                // Add the data row
                Output.WriteLine("    <tr>");
                Output.WriteLine("      <td style=\"text-align:left;\">" + Month_From_Int(usage.Month) + " " + usage.Year + "</td>");
                 
                Output.WriteLine("      <td>" + usage.Hits + "</td>");
                totalHits += usage.Hits;

                if (usage.HitsHierarchical.HasValue)
                {
                    totalHierarchicalHits += usage.HitsHierarchical.Value;
                    Output.WriteLine("      <td>" + usage.HitsHierarchical.Value + "</td>");
                }
                else
                {
                    totalHierarchicalHits += usage.Hits;
                    Output.WriteLine("      <td>" + usage.Hits + "</td>");
                }

                Output.WriteLine("    </tr>");
            }

            Output.WriteLine("    <tr style=\"font-weight: bold\">");
            Output.WriteLine("      <td style=\"text-align:left;\">TOTAL</td>");
            Output.WriteLine("      <td>" + totalHits + "</td>");
            Output.WriteLine("      <td>" + totalHierarchicalHits + "</td>");
            Output.WriteLine("    </tr>");
            Output.WriteLine("  </table>");

            Output.WriteLine("  <br /> <br />");
        }

        private static string Month_From_Int(int Month_Int)
        {
            string monthString1 = "Invalid";
            switch (Month_Int)
            {
                case 1:
                    monthString1 = "January";
                    break;

                case 2:
                    monthString1 = "February";
                    break;

                case 3:
                    monthString1 = "March";
                    break;

                case 4:
                    monthString1 = "April";
                    break;

                case 5:
                    monthString1 = "May";
                    break;

                case 6:
                    monthString1 = "June";
                    break;

                case 7:
                    monthString1 = "July";
                    break;

                case 8:
                    monthString1 = "August";
                    break;

                case 9:
                    monthString1 = "September";
                    break;

                case 10:
                    monthString1 = "October";
                    break;

                case 11:
                    monthString1 = "November";
                    break;

                case 12:
                    monthString1 = "December";
                    break;
            }
            return monthString1;
        }
    }
}
