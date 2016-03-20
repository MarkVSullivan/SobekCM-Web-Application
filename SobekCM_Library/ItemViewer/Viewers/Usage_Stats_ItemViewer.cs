﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using SobekCM.Core.BriefItem;
using SobekCM.Core.Navigation;
using SobekCM.Core.Users;
using SobekCM.Engine_Library.Database;
using SobekCM.Library.Database;
using SobekCM.Library.UI;
using SobekCM.Tools;

namespace SobekCM.Library.ItemViewer.Viewers
{
    /// <summary> Usage statisticsitem viewer prototyper (for readership/viewers), which is used to create the link in the main menu, 
    /// and to create the viewer itself if the user selects that option </summary>
    public class Usage_Stats_ItemViewer_Prototyper : iItemViewerPrototyper
    {
        /// <summary> Constructor for a new instance of the Usage_Stats_ItemViewer_Prototyper class </summary>
        public Usage_Stats_ItemViewer_Prototyper()
        {
            ViewerType = "USAGE";
            ViewerCode = "usage";
        }

        /// <summary> Name of this viewer, which matches the viewer name from the database and 
        /// in the configuration files as well.  This is actually populate by the configuration information </summary>
        public string ViewerType { get; set; }

        /// <summary> Code for this viewer, which can also be set from the configuration information </summary>
        public string ViewerCode { get; set; }

        /// <summary> If this viewer is tied to certain files existing in the digital resource, this lists all the 
        /// possible file extensions this supports (from the configuration file usually) </summary>
        public string[] FileExtensions { get; set; }

        /// <summary> Indicates if the specified item matches the basic requirements for this viewer, or
        /// if this viewer should be ignored for this item </summary>
        /// <param name="CurrentItem"> Digital resource to examine to see if this viewer really should be included </param>
        /// <returns> TRUE if this viewer should generally be included with this item, otherwise FALSE </returns>
        public bool Include_Viewer(BriefItemInfo CurrentItem)
        {
            // This should always be included 
            return true;
        }

        /// <summary> Flag indicates if this viewer should be override on checkout </summary>
        /// <param name="CurrentItem"> Digital resource to examine to see if this viewer should really be overriden </param>
        /// <returns> FALSE always, since citation type information should always be shown, even if an item is checked out </returns>
        public bool Override_On_Checkout(BriefItemInfo CurrentItem)
        {
            return false;
        }

        /// <summary> Flag indicates if the current user has access to this viewer for the item </summary>
        /// <param name="CurrentItem"> Digital resource to see if the current user has correct permissions to use this viewer </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="IpRestricted"> Flag indicates if this item is IP restricted AND if the current user is outside the ranges </param>
        /// <returns> TRUE if the user has access to use this viewer, otherwise FALSE </returns>
        public bool Has_Access(BriefItemInfo CurrentItem, User_Object CurrentUser, bool IpRestricted)
        {
            // It can always be shown
            return true;
        }

        /// <summary> Gets the menu items related to this viewer that should be included on the main item (digital resource) menu </summary>
        /// <param name="CurrentItem"> Digital resource object, which can be used to ensure if and how this viewer should appear 
        /// in the main item (digital resource) menu </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        /// <param name="MenuItems"> List of menu items, to which this method may add one or more menu items </param>
        public void Add_Menu_items(BriefItemInfo CurrentItem, User_Object CurrentUser, Navigation_Object CurrentRequest, List<Item_MenuItem> MenuItems)
        {
            Item_MenuItem menuItem = new Item_MenuItem("Description", "Usage Statistics", null, CurrentItem.Web.Source_URL + ViewerCode);
            MenuItems.Add(menuItem);
        }

        /// <summary> Creates and returns the an instance of the <see cref="Usage_Stats_ItemViewer"/> class for showing the
        /// usage statistics ( readership / visitors ) over time for a digital resource during execution of a single HTTP request. </summary>
        /// <param name="CurrentItem"> Digital resource object </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        /// <returns> Fully built and initialized <see cref="Usage_Stats_ItemViewer"/> object </returns>
        /// <remarks> This method is called whenever a request requires the actual viewer to be created to render the HTML for
        /// the digital resource requested.  The created viewer is then destroyed at the end of the request </remarks>
        public iItemViewer Create_Viewer(BriefItemInfo CurrentItem, User_Object CurrentUser, Navigation_Object CurrentRequest)
        {
            return new Usage_Stats_ItemViewer(CurrentItem, CurrentUser, CurrentRequest);
        }
    }

    /// <summary> Item viewer displays the usage statistics (i.e., readership / visitors ) related to a digital resource </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractNoPaginationItemViewer"/> and implements the 
    /// <see cref="iItemViewer" /> interface. </remarks>
    public class Usage_Stats_ItemViewer : abstractNoPaginationItemViewer
    {
        /// <summary> Constructor for a new instance of the Usage_Stats_ItemViewer class, used to display the 
        /// usage statistics (i.e., readership / visitors ) of a digital resource </summary>
        /// <param name="BriefItem"> Digital resource object </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        public Usage_Stats_ItemViewer(BriefItemInfo BriefItem, User_Object CurrentUser, Navigation_Object CurrentRequest)
        {
            // Save the arguments for use later
            this.BriefItem = BriefItem;
            this.CurrentUser = CurrentUser;
            this.CurrentRequest = CurrentRequest;

            // Set the behavior properties to the empy behaviors ( in the base class )
            Behaviors = EmptyBehaviors;
        }

        /// <summary> CSS ID for the viewer viewport for this particular viewer </summary>
        /// <value> This always returns the value 'sbkUsiv_Viewer' </value>
        public override string ViewerBox_CssId
        {
            get { return "sbkUsiv_Viewer"; }
        }

        /// <summary> Write the item viewer main section as HTML directly to the HTTP output stream </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Usage_Stats_ItemViewer.Write_Main_Viewer_Section", "Write the usage statistics information directly to the output stream");
            }

            // Determine if user can edit
            bool userCanEditItem = false;
            if (CurrentUser != null)
            {
                userCanEditItem = CurrentUser.Can_Edit_This_Item(BriefItem.BibID, BriefItem.Type, BriefItem.Behaviors.Source_Institution_Aggregation, BriefItem.Behaviors.Holding_Location_Aggregation, BriefItem.Behaviors.Aggregation_Code_List);
            }

            // Add the HTML for the citation
            Output.WriteLine("        <!-- USAGE STATS ITEM VIEWER OUTPUT -->");
            Output.WriteLine("        <td>");

            // If this is DARK and the user cannot edit and the flag is not set to show citation, show nothing here
            if ((BriefItem.Behaviors.Dark_Flag) && (!userCanEditItem) && (!UI_ApplicationCache_Gateway.Settings.Resources.Show_Citation_For_Dark_Items))
            {
                Output.WriteLine("          <div id=\"darkItemSuppressCitationMsg\">This item is DARK and cannot be viewed at this time</div>" + Environment.NewLine + "</td>" + Environment.NewLine + "  <!-- END CITATION VIEWER OUTPUT -->");
                return;
            }

            string viewer_code = CurrentRequest.ViewerCode;

            // Get any search terms
            List<string> terms = new List<string>();
            if (!String.IsNullOrWhiteSpace(CurrentRequest.Text_Search))
            {
                string[] splitter = CurrentRequest.Text_Search.Replace("\"", "").Split(" ".ToCharArray());
                terms.AddRange(from thisSplit in splitter where thisSplit.Trim().Length > 0 select thisSplit.Trim());
            }

            // Add the main wrapper division
            Output.WriteLine("<div id=\"sbkCiv_Citation\">");

            if (!CurrentRequest.Is_Robot)
                Citation_Standard_ItemViewer.Add_Citation_View_Tabs(Output, BriefItem, CurrentRequest, "USAGE");

            // Now, add the text
            Output.WriteLine();
            Output.WriteLine(Statistics_String(Tracer) + "  </td>" + Environment.NewLine + "  <!-- END CITATION VIEWER OUTPUT -->");

            CurrentRequest.ViewerCode = viewer_code;
        }

        #region Section returns the item level statistics

        /// <summary> Returns the string which contains the item and title level statistics </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Sttring with the statistical usage information for this item and title</returns>
        protected string Statistics_String(Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Citation_ItemViewer.Statistics_String", "Create the statistics html");
            }

            int hits = 0;
            int sessions = 0;
            int jpeg_views = 0;
            int zoom_views = 0;
            int thumb_views = 0;
            int flash_views = 0;
            int google_map_views = 0;
            int download_views = 0;
            int citation_views = 0;
            int text_search_views = 0;
            int static_views = 0;

            // Pull the item statistics
            DataSet stats = null; //Engine_Database.Get_Item_Statistics_History(BriefItem.BibID, BriefItem.VID, Tracer);

            StringBuilder builder = new StringBuilder(2000);

            builder.AppendLine("  <p>This item was has been viewed <%HITS%> times within <%SESSIONS%> visits.  Below are the details for overall usage for this item within this library.<br /><br />For definitions of these terms, see the <a href=\"" + CurrentRequest.Base_URL + "stats/usage/definitions\" target=\"_BLANK\">definitions on the main statistics page</a>.</p>");

            builder.AppendLine("  <table class=\"sbkCiv_StatsTable\">");
            builder.AppendLine("    <tr class=\"sbkCiv_StatsTableHeaderRow\">");
            builder.AppendLine("      <th style=\"width:120px\">Date</th>");
            builder.AppendLine("      <th style=\"width:90px\">Views</th>");
            builder.AppendLine("      <th style=\"width:90px\">Visits</th>");
            builder.AppendLine("      <th style=\"width:90px\">JPEG<br />Views</th>");
            builder.AppendLine("      <th style=\"width:90px\">Zoomable<br />Views</th>");
            builder.AppendLine("      <th style=\"width:90px\">Citation<br />Views</th>");
            builder.AppendLine("      <th style=\"width:90px\">Thumbnail<br />Views</th>");
            builder.AppendLine("      <th style=\"width:90px\">Text<br />Searches</th>");
            builder.AppendLine("      <th style=\"width:90px\">Flash<br />Views</th>");
            builder.AppendLine("      <th style=\"width:90px\">Map<br />Views</th>");
            builder.AppendLine("      <th style=\"width:90px\">Download<br />Views</th>");
            builder.AppendLine("      <th style=\"width:90px\">Static<br />Views</th>");
            builder.AppendLine("    </tr>");

            const int COLUMNS = 12;
            string last_year = String.Empty;
            if (stats != null)
            {
                foreach (DataRow thisRow in stats.Tables[1].Rows)
                {
                    if (thisRow["Year"].ToString() != last_year)
                    {
                        builder.AppendLine("    <tr><td class=\"sbkCiv_StatsTableYearRow\" colspan=\"" + COLUMNS + "\">" + thisRow["Year"] + " STATISTICS</td></tr>");
                        last_year = thisRow["Year"].ToString();
                    }
                    else
                    {
                        builder.AppendLine("    <tr><td class=\"sbkCiv_StatsTableRowSeperator\" colspan=\"" + COLUMNS + "\"></td></tr>");
                    }
                    builder.AppendLine("    <tr>");
                    builder.AppendLine("      <td style=\"text-align: left\">" + Month_From_Int(Convert.ToInt32(thisRow["Month"])) + " " + thisRow["Year"] + "</td>");

                    if (thisRow[5] != DBNull.Value)
                    {
                        hits += Convert.ToInt32(thisRow[5]);
                        builder.AppendLine("      <td>" + thisRow[5] + "</td>");
                    }
                    else
                    {
                        builder.AppendLine("      <td>0</td>");
                    }

                    if (thisRow[6] != DBNull.Value)
                    {
                        sessions += Convert.ToInt32(thisRow[6]);
                        builder.AppendLine("      <td>" + thisRow[6] + "</td>");
                    }
                    else
                    {
                        builder.AppendLine("      <td>0</td>");
                    }

                    if (thisRow[7] != DBNull.Value)
                    {
                        jpeg_views += Convert.ToInt32(thisRow[10]);
                        builder.AppendLine("      <td>" + thisRow[7] + "</td>");
                    }
                    else
                    {
                        builder.AppendLine("      <td>0</td>");
                    }
                    if (thisRow[8] != DBNull.Value)
                    {
                        zoom_views += Convert.ToInt32(thisRow[8]);
                        builder.AppendLine("      <td>" + thisRow[8] + "</td>");
                    }
                    else
                    {
                        builder.AppendLine("      <td>0</td>");
                    }
                    if (thisRow[9] != DBNull.Value)
                    {
                        citation_views += Convert.ToInt32(thisRow[9]);
                        builder.AppendLine("      <td>" + thisRow[9] + "</td>");
                    }
                    else
                    {
                        builder.AppendLine("      <td>0</td>");
                    }
                    if (thisRow[10] != DBNull.Value)
                    {
                        thumb_views += Convert.ToInt32(thisRow[10]);
                        builder.AppendLine("      <td>" + thisRow[10] + "</td>");
                    }
                    else
                    {
                        builder.AppendLine("      <td>0</td>");
                    }
                    if (thisRow[11] != DBNull.Value)
                    {
                        text_search_views += Convert.ToInt32(thisRow[11]);
                        builder.AppendLine("      <td>" + thisRow[11] + "</td>");
                    }
                    else
                    {
                        builder.AppendLine("      <td>0</td>");
                    }
                    if (thisRow[12] != DBNull.Value)
                    {
                        flash_views += Convert.ToInt32(thisRow[12]);
                        builder.AppendLine("      <td>" + thisRow[12] + "</td>");
                    }
                    else
                    {
                        builder.AppendLine("      <td>0</td>");
                    }
                    if (thisRow[13] != DBNull.Value)
                    {
                        google_map_views += Convert.ToInt32(thisRow[13]);
                        builder.AppendLine("      <td>" + thisRow[13] + "</td>");
                    }
                    else
                    {
                        builder.AppendLine("      <td>0</td>");
                    }
                    if (thisRow[14] != DBNull.Value)
                    {
                        download_views += Convert.ToInt32(thisRow[14]);
                        builder.AppendLine("      <td>" + thisRow[14] + "</td>");
                    }
                    else
                    {
                        builder.AppendLine("      <td>0</td>");
                    }
                    if (thisRow[15] != DBNull.Value)
                    {
                        static_views += Convert.ToInt32(thisRow[15]);
                        builder.AppendLine("      <td>" + thisRow[15] + "</td>");
                    }
                    else
                    {
                        builder.AppendLine("      <td>0</td>");
                    }
                    builder.AppendLine("    </tr>");
                }

                builder.AppendLine("    <tr><td class=\"sbkCiv_StatsTableFinalSeperator\" colspan=\"" + COLUMNS + "\"></td></tr>");
                builder.AppendLine("    <tr id=\"sbkCiv_StatsTableTotalRow\" >");
                builder.AppendLine("      <td style=\"text-align:left\">TOTAL</td>");
                builder.AppendLine("      <td>" + hits + "</td>");
                builder.AppendLine("      <td>" + sessions + "</td>");
                builder.AppendLine("      <td>" + jpeg_views + "</td>");
                builder.AppendLine("      <td>" + zoom_views + "</td>");
                builder.AppendLine("      <td>" + citation_views + "</td>");
                builder.AppendLine("      <td>" + thumb_views + "</td>");
                builder.AppendLine("      <td>" + text_search_views + "</td>");
                builder.AppendLine("      <td>" + flash_views + "</td>");
                builder.AppendLine("      <td>" + google_map_views + "</td>");
                builder.AppendLine("      <td>" + download_views + "</td>");
                builder.AppendLine("      <td>" + static_views + "</td>");
                builder.AppendLine("    </tr>");
                builder.AppendLine("  </table>");
                builder.AppendLine("  <br />");
            }

            builder.AppendLine("</div>");
            return builder.ToString().Replace("<%HITS%>", hits.ToString()).Replace("<%SESSIONS%>", sessions.ToString());

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

        #endregion

        /// <summary> Allows controls to be added directory to a place holder, rather than just writing to the output HTML stream </summary>
        /// <param name="MainPlaceHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the bulk of the item viewer's output is displayed</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <remarks> This method does nothing, since nothing is added to the place holder as a control for this item viewer </remarks>
        public override void Add_Main_Viewer_Section(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
        {
            // Do nothing
        }
    }
}
