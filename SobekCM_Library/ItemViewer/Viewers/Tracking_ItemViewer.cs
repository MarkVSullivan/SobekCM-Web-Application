using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using SobekCM.Core.BriefItem;
using SobekCM.Core.Client;
using SobekCM.Core.Navigation;
using SobekCM.Core.Users;
using SobekCM.Library.Database;
using SobekCM.Library.ItemViewer.Menu;
using SobekCM.Library.UI;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Resource_Object.Tracking;
using SobekCM.Tools;

namespace SobekCM.Library.ItemViewer.Viewers
{
    /// <summary> Manage menu item viewer prototyper, which is used to check to see if a user has access to view the tracking (both
    /// milestone and full work history) for a digital resource, and to create the viewer itself if the user selects that option </summary>
    public class Tracking_ItemViewer_Prototyper : iItemViewerPrototyper
    {
        /// <summary> Constructor for a new instance of the Tracking_ItemViewer_Prototyper class </summary>
        public Tracking_ItemViewer_Prototyper()
        {
            ViewerType = "TRACKING";
            ViewerCode = "history";
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
        /// <param name="currentItem"> Digital resource to examine to see if this viewer really should be included </param>
        /// <returns> TRUE if this viewer should generally be included with this item, otherwise FALSE </returns>
        public bool Include_Viewer(BriefItemInfo currentItem)
        {
            // This should always be included (although it won't be accessible or shown to everyone)
            return true;
        }

        /// <summary> Flag indicates if this viewer should be override on checkout </summary>
        /// <param name="currentItem"> Digital resource to examine to see if this viewer should really be overriden </param>
        /// <returns> TRUE always, since PDFs should never be shown if an item is checked out </returns>
        public bool Override_On_Checkout(BriefItemInfo currentItem)
        {
            return false;
        }

        /// <summary> Flag indicates if the current user has access to this viewer for the item </summary>
        /// <param name="currentItem"> Digital resource to see if the current user has correct permissions to use this viewer </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="IpRestricted"> Flag indicates if this item is IP restricted AND if the current user is outside the ranges </param>
        /// <returns> TRUE if the user has access to use this viewer, otherwise FALSE </returns>
        public bool Has_Access(BriefItemInfo currentItem, User_Object CurrentUser, bool IpRestricted)
        {
            // If there is no user (or they aren't logged in) then obviously, they can't edit this
            if ((CurrentUser == null) || (!CurrentUser.LoggedOn))
            {
                return false;
            }

            // If INTERNAL, user has access
            if ((CurrentUser.Is_Host_Admin) || (CurrentUser.Is_System_Admin) || (CurrentUser.Is_Portal_Admin) || (CurrentUser.Is_Internal_User))
                return true;

            // See if this user can edit this item
            bool userCanEditItem = CurrentUser.Can_Edit_This_Item(currentItem.BibID, currentItem.Type, currentItem.Behaviors.Source_Institution_Aggregation, currentItem.Behaviors.Holding_Location_Aggregation, currentItem.Behaviors.Aggregation_Code_List);
            if (!userCanEditItem)
            {
                // Can't edit, so don't show and return FALSE
                return false;
            }

            // Apparently it can be shown
            return true;
        }

        /// <summary> Gets the menu items related to this viewer that should be included on the main item (digital resource) menu </summary>
        /// <param name="currentItem"> Digital resource object, which can be used to ensure if and how this viewer should appear 
        /// in the main item (digital resource) menu </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        /// <param name="MenuItems"> List of menu items, to which this method may add one or more menu items </param>
        public void Add_Menu_items(BriefItemInfo currentItem, User_Object CurrentUser, Navigation_Object CurrentRequest, List<Item_MenuItem> MenuItems)
        {
            // Do nothing since this is already handed and added to the menu by the MANAGE MENU item viewer and INTERNAL header
        }

        /// <summary> Creates and returns the an instance of the <see cref="Tracking_ItemViewer"/> class for showing the
        /// tracking information ( both milestones and workflow history ) for a digital resource during execution of a single HTTP request. </summary>
        /// <param name="currentItem"> Digital resource object </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Fully built and initialized <see cref="Tracking_ItemViewer"/> object </returns>
        /// <remarks> This method is called whenever a request requires the actual viewer to be created to render the HTML for
        /// the digital resource requested.  The created viewer is then destroyed at the end of the request </remarks>
        public iItemViewer Create_Viewer(BriefItemInfo currentItem, User_Object CurrentUser, Navigation_Object CurrentRequest, Custom_Tracer Tracer)
        {
            return new Tracking_ItemViewer(currentItem, CurrentUser, CurrentRequest, Tracer);
        }
    }

    /// <summary> Item viewer displays the tracking information for a digital resource (both milestones and workflow history) </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractNoPaginationItemViewer"/> and implements the 
    /// <see cref="iItemViewer" /> interface. </remarks>
    public class Tracking_ItemViewer : abstractNoPaginationItemViewer
    {
        private bool userCanEditItem;
        private SobekCM_Item currentItem;

        /// <summary> Constructor for a new instance of the Tracking_ItemViewer class, used display
        /// the tracking information for a digital resource (both milestones and workflow history) </summary>
        /// <param name="BriefItem"> Digital resource object </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public Tracking_ItemViewer(BriefItemInfo BriefItem, User_Object CurrentUser, Navigation_Object CurrentRequest, Custom_Tracer Tracer )
        {
            // Save the arguments for use later
            this.BriefItem = BriefItem;
            this.CurrentUser = CurrentUser;
            this.CurrentRequest = CurrentRequest;

            // Set the behavior properties to the empy behaviors ( in the base class )
            Behaviors = EmptyBehaviors;

            userCanEditItem = false;
            if (CurrentUser != null)
            {
                userCanEditItem = CurrentUser.Can_Edit_This_Item(BriefItem.BibID, BriefItem.Type, BriefItem.Behaviors.Source_Institution_Aggregation, BriefItem.Behaviors.Holding_Location_Aggregation, BriefItem.Behaviors.Aggregation_Code_List);
            }


            // Get the full oibject
            Tracer.Add_Trace("Tracking_ItemViewer.Constructor", "Try to pull this sobek complete item");
            currentItem = SobekEngineClient.Items.Get_Sobek_Item(CurrentRequest.BibID, CurrentRequest.VID, Tracer);
            if (currentItem == null)
            {
                Tracer.Add_Trace("Edit_Item_Behaviors_MySobekViewer.Constructor", "Unable to build complete item");
                CurrentRequest.Mode = Display_Mode_Enum.Error;
                CurrentRequest.Error_Message = "Invalid Request : Unable to build complete item";
                return;
            }

        }

        /// <summary> CSS ID for the viewer viewport for this particular viewer </summary>
        /// <value> This always returns the value 'sbkDiv_Viewer' </value>
        public override string ViewerBox_CssId
        {
            get { return "sbkDiv_Viewer"; }
        }

        /// <summary> Write the item viewer main section as HTML directly to the HTTP output stream </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Tracking_ItemViewer.Write_Main_Viewer_Section", "");



            // If this is an internal user or can edit this item, ensure the extra information 
            // has been pulled for this item
            if ((userCanEditItem) || ((CurrentUser != null) && (CurrentUser.LoggedOn) && (CurrentUser.Is_Internal_User)) || (CurrentRequest.ViewerCode == "tracking") || (CurrentRequest.ViewerCode == "media") || (CurrentRequest.ViewerCode == "archive"))
            {
                if (!currentItem.Tracking.Tracking_Info_Pulled)
                {
                    DataSet data = SobekCM_Database.Tracking_Get_History_Archives(currentItem.Web.ItemID, Tracer);
                    currentItem.Tracking.Set_Tracking_Info(data);
                }
            }

            // Determine the citation type
            Tracking_Type citationType = Tracking_Type.Milestones;
            switch (CurrentRequest.ViewerCode)
            {
                case "tracking":
                    citationType = Tracking_Type.History;
                    break;

                case "media":
                    citationType = Tracking_Type.Media;
                    break;

                case "archive":
                    citationType = Tracking_Type.Archives;
                    break;

                case "directory":
                    citationType = Tracking_Type.Directory_List;
                    break;
            }

            // Add the HTML for the image
            Output.WriteLine("<!-- TRACKING ITEM VIEWER OUTPUT -->");

            // Start the citation table
            Output.WriteLine("  <td align=\"left\"><span class=\"sbkTrk_ViewerTitle\">Tracking Information</span></td>");
            Output.WriteLine("</tr>");
            Output.WriteLine("<tr>");
            Output.WriteLine("  <td class=\"sbkTrk_MainArea\">");

            // Set the text
            const string MILESTONES_VIEW = "MILESTONES";
            const string TRACKING_VIEW = "HISTORY";
            const string MEDIA_VIEW = "MEDIA";
            const string ARCHIVE_VIEW = "ARCHIVES";
            const string DIRECTORY_VIEW = "DIRECTORY";

            // Add the tabs for the different citation information
            string viewer_code = CurrentRequest.ViewerCode;
            Output.WriteLine("    <div id=\"sbkTrk_ViewSelectRow\">");
            Output.WriteLine("      <ul class=\"sbk_FauxDownwardTabsList\">");

            if (currentItem.METS_Header.RecordStatus_Enum != METS_Record_Status.BIB_LEVEL)
            {
                if (citationType == Tracking_Type.Milestones)
                {
                    Output.WriteLine("        <li class=\"current\">" + MILESTONES_VIEW + "</li>");
                }
                else
                {
                    Output.WriteLine("        <li><a href=\"" + UrlWriterHelper.Redirect_URL(CurrentRequest, "milestones") + "\">" + MILESTONES_VIEW + "</a></li>");
                }

                if ((citationType == Tracking_Type.History) || ((currentItem.Tracking.hasHistoryInformation)))
                {
                    if (citationType == Tracking_Type.History)
                    {
                        Output.WriteLine("        <li class=\"current\">" + TRACKING_VIEW + "</li>");
                    }
                    else
                    {
                        Output.WriteLine("        <li><a href=\"" + UrlWriterHelper.Redirect_URL(CurrentRequest, "tracking") + "\">" + TRACKING_VIEW + "</a></li>");
                    }
                }

                if ((citationType == Tracking_Type.Media) || ((currentItem.Tracking.hasMediaInformation)))
                {
                    if (citationType == Tracking_Type.Media)
                    {
                        Output.WriteLine("        <li class=\"current\">" + MEDIA_VIEW + "</li>");
                    }
                    else
                    {
                        Output.WriteLine("        <li><a href=\"" + UrlWriterHelper.Redirect_URL(CurrentRequest, "media") + "\">" + MEDIA_VIEW + "</a></li>");
                    }
                }

                if ((citationType == Tracking_Type.Archives) || ((currentItem.Tracking.hasArchiveInformation) && (((CurrentUser != null) && (CurrentUser.LoggedOn) && (CurrentUser.Is_Internal_User)) || (userCanEditItem))))
                {
                    if (citationType == Tracking_Type.Archives)
                    {
                        Output.WriteLine("        <li class=\"current\">" + ARCHIVE_VIEW + "</li>");
                    }
                    else
                    {
                        Output.WriteLine("        <li><a href=\"" + UrlWriterHelper.Redirect_URL(CurrentRequest, "archive") + "\">" + ARCHIVE_VIEW + "</a></li>");
                    }
                }

                if ((citationType == Tracking_Type.Directory_List) || ((CurrentUser != null) && (CurrentUser.LoggedOn) && (CurrentUser.Is_Internal_User)) || (userCanEditItem))
                {
                    if (citationType == Tracking_Type.Directory_List)
                    {
                        Output.WriteLine("        <li class=\"current\">" + DIRECTORY_VIEW + "</li>");
                    }
                    else
                    {
                        Output.WriteLine("        <li><a href=\"" + UrlWriterHelper.Redirect_URL(CurrentRequest, "directory") + "\">" + DIRECTORY_VIEW + "</a></li>");
                    }
                }
            }

            Output.WriteLine("              </div>");

            // Now, add the text
            switch (citationType)
            {
                case Tracking_Type.Milestones:
                    Output.WriteLine(Milestones_String(Tracer) + "  </td>" + Environment.NewLine + "  <!-- END TRACKING VIEWER OUTPUT -->");
                    break;

                case Tracking_Type.History:
                    Output.WriteLine(History_String(Tracer) + "  </td>" + Environment.NewLine + "  <!-- END TRACKING VIEWER OUTPUT -->");
                    break;

                case Tracking_Type.Media:
                    Output.WriteLine(Media_String(Tracer) + "  </td>" + Environment.NewLine + "  <!-- END TRACKING VIEWER OUTPUT -->");
                    break;

                case Tracking_Type.Archives:
                    Output.WriteLine(Archives_String(Tracer) + "  </td>" + Environment.NewLine + "  <!-- END TRACKING VIEWER OUTPUT -->");
                    break;

                case Tracking_Type.Directory_List:
                    Output.WriteLine(Directory_String(Tracer) + "  </td>" + Environment.NewLine + "  <!-- END TRACKING VIEWER OUTPUT -->");
                    break;
            }

            CurrentRequest.ViewerCode = viewer_code;
        }

        #region Section returns the history tab data

        /// <summary> Returns the information from history/worklog tracking information as an HTML string </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Work_History/worklog information about this digital resource as an HTML string</returns>
        protected string History_String(Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Tracking_ItemViewer.History_String", "Displaying history/worklog tracking information");
            }

            StringBuilder builder = new StringBuilder(3000);
            if (!currentItem.Tracking.hasHistoryInformation)
            {
                builder.AppendLine("<br /><br /><br /><center><strong>ITEM HAS NO HISTORY</strong></center><br /><br /><br />");
            }
            else
            {
                builder.AppendLine("<br />\n");
                builder.AppendLine("<table border=\"1px\" cellpadding=\"5px\" cellspacing=\"0px\" rules=\"cols\" frame=\"void\" bordercolor=\"#e7e7e7\" width=\"100%\">");
                builder.AppendLine("  <tr align=\"center\" bgcolor=\"#0022a7\"><td colspan=\"4\"><span style=\"color: White\"><b>ITEM HISTORY</b></span></td></tr>");
                builder.AppendLine("  <tr align=\"left\" bgcolor=\"#7d90d5\">");
                builder.AppendLine("    <th><span style=\"color: White\">WORKFLOW NAME</span></th>");
                builder.AppendLine("    <th><span style=\"color: White\">COMPLETED DATE</span></th>");
                builder.AppendLine("    <th><span style=\"color: White\">USER</span></th>");
                builder.AppendLine("    <th><span style=\"color: White\">LOCATION / NOTES</span></th>");
                builder.AppendLine("  </tr>");

                foreach (Tracking_Progress worklog in currentItem.Tracking.Work_History)
                {
                    builder.AppendLine("  <tr>");
                    builder.AppendLine("    <td>" + worklog.Workflow_Name + "</td>");
                    if (worklog.Completed_Date.HasValue)
                        builder.AppendLine("    <td>" + worklog.Completed_Date.Value.ToShortDateString() + "</td>");
                    else
                        builder.AppendLine("    <td>n/a</td>");

                    builder.AppendLine("    <td>" + worklog.Work_Performed_By + "</td>");
                    if ((worklog.FilePath.Length == 0) && (worklog.Note.Length == 0))
                    {
                        builder.AppendLine("    <td>&nbsp;</td>");
                    }
                    else
                    {
                        if ((worklog.FilePath.Length > 0) && (worklog.Note.Length > 0))
                        {
                            builder.AppendLine("    <td>" + worklog.FilePath + "<br />" + worklog.Note + "</td>");
                        }
                        else
                        {
                            if (worklog.FilePath.Length > 0)
                            {
                                builder.AppendLine("    <td>" + worklog.FilePath + "</td>");
                            }
                            else
                            {
                                builder.AppendLine("    <td>" + worklog.Note + "</td>");
                            }
                        }
                    }
                    builder.AppendLine("  </tr>");
                }

                builder.AppendLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"4\"></td></tr>");
                builder.AppendLine("</table>\n");
            }

            builder.AppendLine("<br /> <br />");

            return builder.ToString();
        }

        #endregion

        #region Section returns the media tab data

        /// <summary> Returns the media tracking information  ( i.e., CD Archvie information ) as an HTML string </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Media information about this digital resource ( history and archive information ) as an HTML string</returns>
        protected string Media_String(Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Tracking_ItemViewer.Media_String", "Displaying media archive information");
            }

            StringBuilder builder = new StringBuilder(3000);
            if (!currentItem.Tracking.hasMediaInformation)
            {
                builder.AppendLine("<br /><br /><br /><center><strong>ITEM IS NOT ARCHIVED TO MEDIA</strong></center><br /><br /><br />");
            }
            else
            {
                builder.AppendLine("<br />");
                builder.AppendLine("<table border=\"1px\" cellpadding=\"5px\" cellspacing=\"0px\" rules=\"cols\" frame=\"void\" bordercolor=\"#e7e7e7\" >");
                builder.AppendLine("  <tr align=\"center\" bgcolor=\"#0022a7\"><td colspan=\"5\"><span style=\"color: White\"><b>CD/DVD ARCHIVE</b></span></td></tr>");
                builder.AppendLine("  <tr align=\"left\" bgcolor=\"#7d90d5\">");
                builder.AppendLine("    <th><span style=\"color: White\">CD NUMBER</span></th>");
                builder.AppendLine("    <th width=\"350px\"><span style=\"color: White\">FILE RANGE</span></th>");
                builder.AppendLine("    <th><span style=\"color: White\">IMAGES</span></th>");
                builder.AppendLine("    <th><span style=\"color: White\">SIZE</span></th>");
                builder.AppendLine("    <th><span style=\"color: White\">DATE BURNED</span></th>");
                builder.AppendLine("  </tr>");

                foreach (Tracking_ArchiveMedia thisRow in currentItem.Tracking.Archive_Media)
                {
                    builder.AppendLine("  <tr>");
                    builder.AppendLine("    <td>" + thisRow.Media_Number + "</td>");
                    builder.AppendLine("    <td width=\"350px\">" + thisRow.FileRange + "</td>");
                    if (thisRow.Images > 0)
                        builder.AppendLine("    <td>" + thisRow.Images + "</td>");
                    else
                        builder.AppendLine("    <td>&nbsp;</td>");
                    builder.AppendLine("    <td>" + thisRow.Size + "</td>");
                    builder.AppendLine("    <td>" + thisRow.Date_Burned.ToShortDateString() + "</td>");
                    builder.AppendLine("  </tr>");
                }
                builder.AppendLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"5\"></td></tr>");
                builder.AppendLine("</table>");
            }

            builder.AppendLine("<br /> <br />");

            return builder.ToString();
        }

        #endregion

        #region Section return the archives tab data

        /// <summary> Returns the archive tracking information as an HTML string </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Archive information about this digital resource as an HTML string</returns>
        protected string Archives_String(Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Tracking_ItemViewer.Archives_String", "Displaying archive tracking information");
            }

            StringBuilder builder = new StringBuilder(3000);
            if (!currentItem.Tracking.hasArchiveInformation)
            {
                builder.AppendLine("<br /><br /><br /><center><strong>ITEM HAS NO ARCHIVE INFORMATION</strong></center><br /><br /><br />");
            }
            else
            {
                // Now, pull the list of all archived files
                DataSet data = SobekCM_Database.Tracking_Get_History_Archives(currentItem.Web.ItemID, Tracer);
                DataTable archiveTable = data.Tables[2];
                DataColumn filenameColumn = archiveTable.Columns["Filename"];
                DataColumn sizeColumn = archiveTable.Columns["Size"];
                DataColumn lastWriteDateColumn = archiveTable.Columns["LastWriteDate"];
                DataColumn archiveDateColumn = archiveTable.Columns["ArchiveDate"];


                builder.AppendLine("<br />");
                builder.AppendLine("<table border=\"1px\" cellpadding=\"1px\" cellspacing=\"0px\" rules=\"cols\" frame=\"void\" bordercolor=\"#e7e7e7\" width=\"100%\">");
                builder.AppendLine("  <tr align=\"center\" bgcolor=\"#0022a7\" height=\"25px\"><td colspan=\"4\"><span style=\"color: White\"><b>ARCHIVED FILE INFORMATION</b></span></td></tr>");
                builder.AppendLine("  <tr align=\"left\" bgcolor=\"#7d90d5\" height=\"25px\">");
                builder.AppendLine("    <th><span style=\"color: White\">FILENAME</span></th>");
                builder.AppendLine("    <th><span style=\"color: White\">SIZE</span></th>");
                builder.AppendLine("    <th><span style=\"color: White\">LAST WRITE DATE</span></th>");
                builder.AppendLine("    <th><span style=\"color: White\">ARCHIVED DATE</span></th>");
                builder.AppendLine("  </tr>");


                foreach (DataRow thisRow in archiveTable.Rows)
                {
                    builder.AppendLine("  <tr height=\"25px\" >");
                    builder.AppendLine("    <td>" + thisRow[filenameColumn] + "</td>");
                    builder.AppendLine("    <td>" + thisRow[sizeColumn] + "</td>");
                    builder.AppendLine("    <td>" + thisRow[lastWriteDateColumn] + "</td>");
                    builder.AppendLine("    <td>" + thisRow[archiveDateColumn] + "</td>");
                    builder.AppendLine("  </tr>");
                    builder.AppendLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"4\"></td></tr>");
                }

                builder.AppendLine("</table>");
            }

            builder.AppendLine("<br /> <br />");

            return builder.ToString();
        }

        #endregion

        #region Section returns the directory tab data

        /// <summary> Returns the directory list for this digital resource </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> HTML string with the directory list for this digital resource </returns>
        public string Directory_String(Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Tracking_ItemViewer.Directory_String", "Pulling and displaying files in the image directory");
            }

            try
            {
                string directory = UI_ApplicationCache_Gateway.Settings.Servers.Image_Server_Network + currentItem.Web.AssocFilePath;
                string url = UI_ApplicationCache_Gateway.Settings.Servers.Image_URL + currentItem.Web.AssocFilePath;

                FileInfo[] files = (new DirectoryInfo(directory)).GetFiles();

                StringBuilder builder = new StringBuilder(3000);

                // Get all the file info objects and order by name
                SortedList<string, FileInfo> sortedFiles = new SortedList<string, FileInfo>();
                foreach (FileInfo thisFile in files)
                {
                    sortedFiles.Add(thisFile.Name.ToUpper(), thisFile);
                }

                // Remove the THUMBS.DB file, if it exists
                if (sortedFiles.ContainsKey("THUMBS.DB"))
                    sortedFiles.Remove("THUMBS.DB");

                // Start the file table
                builder.AppendLine("<br />");
                builder.AppendLine("<br />");
                builder.AppendLine("<blockquote>");
                builder.AppendLine(" &nbsp; &nbsp; <a href=\"" + directory + "\">" + directory + "</a>");
                builder.AppendLine("</blockquote>");

                builder.AppendLine("<blockquote>");


                // Add all the page images first
                List<abstract_TreeNode> nodes = currentItem.Divisions.Physical_Tree.Pages_PreOrder;
                if ((nodes != null) && (nodes.Count > 0))
                {
                    builder.AppendLine("<span style=\"font-size:1.4em; color:#888888;\"><b>PAGE FILES</b></span><br />");
                    builder.AppendLine("<table border=\"0px\" cellspacing=\"0px\" class=\"statsTable\">");
                    builder.AppendLine("<tr align=\"left\" bgcolor=\"#0022a7\" height=\"20px\" >");
                    builder.AppendLine("<th align=\"left\"><span style=\"color: White\">NAME</span></th>");
                    builder.AppendLine("<th width=\"10px\">&nbsp;</th>");
                    builder.AppendLine("<th align=\"left\" width=\"170px\"><span style=\"color: White\">DATE MODIFIED</span></th>");
                    builder.AppendLine("<th align=\"left\" width=\"180px\"><span style=\"color: White\">TYPE</span></th>");
                    builder.AppendLine("<th width=\"10px\">&nbsp;</th>");
                    builder.AppendLine("<th align=\"right\"><span style=\"color: White\">SIZE</span></th>");
                    builder.AppendLine("</tr>");

                    List<string> file_names_added = new List<string>();
                    foreach (Page_TreeNode thisNode in nodes)
                    {
                        // Only show pages with files
                        if (thisNode.Files.Count > 0)
                        {
                            // Ensure that if a page is repeated, it only is written once
                            string[] filename_splitter = thisNode.Files[0].System_Name.Split(".".ToCharArray());

                            string fileName = filename_splitter[0].ToUpper();
                            if (filename_splitter.Length > 1)
                                fileName = filename_splitter[filename_splitter.Length - 2].ToUpper();
                            if (!file_names_added.Contains(fileName))
                            {
                                file_names_added.Add(fileName);

                                builder.AppendLine("<tr align=\"left\" bgcolor=\"#7d90d5\">");
                                string pageName = thisNode.Label;
                                if (pageName.Length == 0)
                                    pageName = "PAGE";

                                builder.AppendLine("<td colspan=\"6\" ><span style=\"color: White\"><b>" + pageName.ToUpper() + "</b></span></td>");
                                builder.AppendLine("</tr>");

                                // Now, check for each file
                                foreach (SobekCM_File_Info thisFile in thisNode.Files)
                                {
                                    string thisFileUpper = thisFile.System_Name.ToUpper();
                                    if (sortedFiles.ContainsKey(thisFileUpper))
                                    {
                                        // string file = UI_ApplicationCache_Gateway.Settings.Servers.Image_Server_Network + currentItem.Web.AssocFilePath + thisFile.System_Name;
                                        Add_File_HTML(sortedFiles[thisFileUpper], builder, url, true);
                                        sortedFiles.Remove(thisFileUpper);
                                    }
                                }

                                // Ensure that there still aren't some page files that exist that were not linked
                                string[] other_page_file_endings = new[] { ".JPG", ".JP2", "THM.JPG", ".TXT", ".PRO", ".QC.JPG" };
                                foreach (string thisFileEnder in other_page_file_endings)
                                {
                                    if (sortedFiles.ContainsKey(fileName.ToUpper() + thisFileEnder))
                                    {
                                        //string file = UI_ApplicationCache_Gateway.Settings.Servers.Image_Server_Network + currentItem.Web.AssocFilePath + fileName + thisFileEnder.ToLower();
                                        Add_File_HTML(sortedFiles[fileName.ToUpper() + thisFileEnder], builder, url, true);
                                        sortedFiles.Remove(fileName.ToUpper() + thisFileEnder);
                                    }
                                }
                            }
                        }
                    }

                    // FInish the table
                    builder.AppendLine("</table>");
                    builder.AppendLine("<br /><br />");
                }

                // Add all the metadata files
                builder.AppendLine("<span style=\"font-size:1.4em; color:#888888;\"><b>METADATA FILES</b></span><br />");
                builder.AppendLine("<table border=\"0px\" cellspacing=\"0px\" class=\"statsTable\">");
                builder.AppendLine("<tr align=\"left\" bgcolor=\"#0022a7\" height=\"20px\" >");
                builder.AppendLine("<th align=\"left\"><span style=\"color: White\">NAME</span></th>");
                builder.AppendLine("<th width=\"10px\">&nbsp;</th>");
                builder.AppendLine("<th align=\"left\" width=\"170px\"><span style=\"color: White\">DATE MODIFIED</span></th>");
                builder.AppendLine("<th align=\"left\" width=\"180px\"><span style=\"color: White\">TYPE</span></th>");
                builder.AppendLine("<th width=\"10px\">&nbsp;</th>");
                builder.AppendLine("<th align=\"right\"><span style=\"color: White\">SIZE</span></th>");
                builder.AppendLine("</tr>");

                // Add each metadata file
                List<string> files_handled = new List<string>();
                foreach (string thisFile in sortedFiles.Keys.Where(ThisFile => (ThisFile.IndexOf(".METS.BAK") > 0) || (ThisFile.IndexOf(".METS.XML") > 0) || (ThisFile == "DOC.XML") || (ThisFile == "MARC.XML") || (ThisFile == "CITATION_METS.XML") || (ThisFile == currentItem.BibID.ToUpper() + "_" + currentItem.VID + ".HTML")))
                {
                    files_handled.Add(thisFile);
                    Add_File_HTML(sortedFiles[thisFile], builder, url, true);
                }

                // REmove all handled files
                foreach (string thisKey in files_handled)
                    sortedFiles.Remove(thisKey);

                // FInish the table
                builder.AppendLine("</table>");
                builder.AppendLine("<br /><br />");

                // Finally, add all the remaining files
                if (sortedFiles.Count > 0)
                {
                    builder.AppendLine("<span style=\"font-size:1.4em; color:#888888;\"><b>OTHER FILES</b></span><br />");
                    builder.AppendLine("<table border=\"0px\" cellspacing=\"0px\" class=\"statsTable\">");
                    builder.AppendLine("<tr align=\"left\" bgcolor=\"#0022a7\" height=\"20px\" >");
                    builder.AppendLine("<th align=\"left\"><span style=\"color: White\">NAME</span></th>");
                    builder.AppendLine("<th width=\"10px\">&nbsp;</th>");
                    builder.AppendLine("<th align=\"left\" width=\"170px\"><span style=\"color: White\">DATE MODIFIED</span></th>");
                    builder.AppendLine("<th align=\"left\" width=\"180px\"><span style=\"color: White\">TYPE</span></th>");
                    builder.AppendLine("<th width=\"10px\">&nbsp;</th>");
                    builder.AppendLine("<th align=\"right\"><span style=\"color: White\">SIZE</span></th>");
                    builder.AppendLine("</tr>");

                    // Now add all the information
                    foreach (FileInfo thisFile in sortedFiles.Values)
                    {
                        Add_File_HTML(thisFile, builder, url, true);
                    }

                    // FInish the table
                    builder.AppendLine("</table>");
                }
                builder.AppendLine("</blockquote>");
                builder.AppendLine("<br />");

                return builder.ToString();
            }
            catch
            {
                return "<br /><center><strong>UNABLE TO PULL DIRECTORY INFORMATION</strong></center><br />";
            }
        }

        private void Add_File_HTML(FileInfo thisFileInfo, StringBuilder builder, string url, bool includeSizeAndDate)
        {
            builder.AppendLine("<tr>");
            builder.AppendLine("<td><a href=\"" + url + thisFileInfo.Name + "\">" + thisFileInfo.Name + "</a></td>");
            builder.AppendLine("<td>&nbsp;</td>");

            if (includeSizeAndDate)
            {
                builder.AppendLine("<td>" + thisFileInfo.LastWriteTime.ToString() + "</td>");
            }

            string extension = thisFileInfo.Extension.ToUpper().Replace(".", "");
            string type = Extension_To_File_Type(extension, thisFileInfo.Name);
            builder.AppendLine("<td>" + type + "</td>");

            if (includeSizeAndDate)
            {
                builder.AppendLine("<td>&nbsp;</td>");
                int size_kb = (int)(thisFileInfo.Length / 1024);
                if (size_kb > 1024)
                {
                    size_kb = size_kb / 1024;
                    builder.AppendLine("<td align=\"right\">" + size_kb + " MB</td>");
                }
                else
                {
                    if (size_kb == 0)
                        size_kb = 1;
                    builder.AppendLine("<td align=\"right\">" + size_kb + " KB</td>");
                }
            }

            builder.AppendLine("</tr>");
            builder.AppendLine(includeSizeAndDate ? "<tr><td bgcolor=\"#e7e7e7\" colspan=\"6\"></td></tr>" : "<tr><td bgcolor=\"#e7e7e7\" colspan=\"3\"></td></tr>");
        }

        private string Extension_To_File_Type(string extension, string fullname)
        {
            string type = extension + " File";

            switch (extension)
            {
                case "JPG":
                case "JPEG":
                    type = "JPEG image";
                    if (fullname.ToUpper().IndexOf("THM.JPG") > 0)
                        type = "Thumbnail image";
                    if (fullname.ToUpper().IndexOf(".QC.JPG") > 0)
                        type = "QC JPEG image";

                    break;

                case "TIF":
                case "TIFF":
                    type = "Archival TIFF image";
                    break;

                case "JP2":
                    type = "JPEG2000 Zoomable image";
                    break;

                case "PDF":
                    type = "Adobe Acrobat Document";
                    break;

                case "TXT":
                case "TEXT":
                    type = "Text file";
                    break;

                case "XLS":
                case "XLSX":
                    type = "Microsoft Office Excel Worksheet";
                    break;

                case "DOC":
                case "DOCX":
                    type = "Microsoft Office Word Document";
                    break;

                case "PPT":
                case "PPTX":
                    type = "Microsoft Office Powerpoint Presentation";
                    break;

                case "SWF":
                    type = "Shockwave Flash Object";
                    break;

                case "PRO":
                    type = "Prime Recognition Output File";
                    break;

                case "HTML":
                case "HTM":
                    type = fullname.ToUpper() == currentItem.BibID.ToUpper() + "_" + currentItem.VID + ".HTML" ? "Static citation page" : "HTML Document";
                    break;

                case "XML":
                    switch (fullname.ToUpper())
                    {
                        case "CITATION_METS.XML":
                            type = "Citation-only METS File";
                            break;

                        case "MARC.XML":
                            type = "MARC XML File";
                            break;

                        case "SobekCM_METS.XML":
                            type = "SobekCM Service METS File";
                            break;

                        case "DOC.XML":
                            type = "Text Indexing File";
                            break;

                        default:
                            type = "XML Document";
                            break;
                    }
                    if (fullname.ToUpper().IndexOf(".METS.XML") > 0)
                        type = "User-submitted METS File";
                    if (fullname.ToUpper().IndexOf("INGEST.XML") > 0)
                        type = "FDA Ingest Report";
                    break;

                case "BAK":
                    type = fullname.ToUpper().IndexOf(".METS.BAK") > 0 ? "Previous METS File Version" : "Backup File";
                    break;
            }
            return type;
        }

        #endregion

        #region Section return the milestones tab data

        /// <summary> Returns the milestones tracking information as an HTML string </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Milestone information about this digital resource as an HTML string</returns>
        protected string Milestones_String(Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Tracking_ItemViewer.Archives_String", "Displaying archive tracking information");
            }

            StringBuilder result = new StringBuilder();
            result.AppendLine("<br />");
            result.AppendLine("<br />");
            result.AppendLine("<span style=\"font-size:1.1em\">");
            result.AppendLine("<blockquote>");
            result.AppendLine("<table width=\"450px\">");
            result.AppendLine("<tr height=\"20px\" bgcolor=\"#7d90d5\"><td colspan=\"3\"><span style=\"color: White\"><strong> &nbsp; DIGITIZATION MILESTONES</strong></span></td></tr>");
            if (currentItem.Tracking.Digital_Acquisition_Milestone.HasValue)
            {
                result.AppendLine("<tr><td width=\"25px\">&nbsp;</td><td>Digital Acquisition</td><td>" + currentItem.Tracking.Digital_Acquisition_Milestone.Value.ToShortDateString() + "</td></tr>");
            }
            else
            {
                result.AppendLine("<tr><td width=\"25px\">&nbsp;</td><td>Digital Acquisition</td><td>n/a</td></tr>");
            }
            result.AppendLine("<tr><td></td><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");

            if (currentItem.Tracking.Image_Processing_Milestone.HasValue)
            {
                result.AppendLine("<tr><td width=\"25px\">&nbsp;</td><td>Post-Acquisition Processing</td><td>" + currentItem.Tracking.Image_Processing_Milestone.Value.ToShortDateString() + "</td></tr>");
            }
            else
            {
                result.AppendLine("<tr><td width=\"25px\">&nbsp;</td><td>Post-Acquisition Processing</td><td>n/a</td></tr>");
            }
            result.AppendLine("<tr><td></td><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");

            if (currentItem.Tracking.Quality_Control_Milestone.HasValue)
            {
                result.AppendLine("<tr><td width=\"25px\">&nbsp;</td><td>Quality Control Performed</td><td>" + currentItem.Tracking.Quality_Control_Milestone.Value.ToShortDateString() + "</td></tr>");
            }
            else
            {
                result.AppendLine("<tr><td width=\"25px\">&nbsp;</td><td>Quality Control Performed</td><td>n/a</td></tr>");
            }
            result.AppendLine("<tr><td></td><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");

            if (currentItem.Tracking.Online_Complete_Milestone.HasValue)
            {
                result.AppendLine("<tr><td width=\"25px\">&nbsp;</td><td>Online Complete</td><td>" + currentItem.Tracking.Online_Complete_Milestone.Value.ToShortDateString() + "</td></tr>");
            }
            else
            {
                result.AppendLine("<tr><td width=\"25px\">&nbsp;</td><td>Online Complete</td><td>n/a</td></tr>");
            }
            result.AppendLine("<tr><td></td><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");
            result.AppendLine("</table>");
            result.AppendLine("<br />");
            result.AppendLine("<br />");

            result.AppendLine("<table width=\"450px\">");
            result.AppendLine("<tr height=\"20px\" bgcolor=\"#7d90d5\"><td colspan=\"3\"><span style=\"color: White\"><strong> &nbsp; PHYSICAL MATERIAL MILESTONES</strong></span></td></tr>");
            if (currentItem.Tracking.Material_Received_Date.HasValue)
            {
                if (currentItem.Tracking.Material_Rec_Date_Estimated)
                    result.AppendLine("<tr><td width=\"25px\">&nbsp;</td><td>Materials Received</td><td>" + currentItem.Tracking.Material_Received_Date.Value.ToShortDateString() + " (estimated) </td></tr>");
                else
                    result.AppendLine("<tr><td width=\"25px\">&nbsp;</td><td>Materials Received</td><td>" + currentItem.Tracking.Material_Received_Date.Value.ToShortDateString() + "</td></tr>");
            }
            else
            {
                result.AppendLine("<tr><td width=\"25px\">&nbsp;</td><td>Materials Received</td><td>n/a</td></tr>");
            }
            result.AppendLine("<tr><td></td><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");

            if (currentItem.Tracking.Disposition_Date.HasValue)
            {
                result.AppendLine("<tr><td width=\"25px\">&nbsp;</td><td>Disposition Date</td><td>" + currentItem.Tracking.Disposition_Date.Value.ToShortDateString() + "</td></tr>");
            }
            else
            {
                result.AppendLine("<tr><td width=\"25px\">&nbsp;</td><td>Disposition Date</td><td>n/a</td></tr>");
            }
            result.AppendLine("<tr><td></td><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");

            if (currentItem.Tracking.Tracking_Box.Length > 0)
            {
                result.AppendLine("<tr><td width=\"25px\">&nbsp;</td><td>Tracking Box</td><td>" + currentItem.Tracking.Tracking_Box + "</td></tr>");
                result.AppendLine("<tr><td></td><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");
            }

            result.AppendLine("</table>");
            result.AppendLine("<br />");
            result.AppendLine("<br />");

            if ((currentItem.Tracking.Born_Digital) || (currentItem.Tracking.Disposition_Advice > 0))
            {
                result.AppendLine("<table width=\"450px\">");
                result.AppendLine("<tr height=\"20px\" bgcolor=\"#7d90d5\"><td colspan=\"3\"><span style=\"color: White\"><strong> &nbsp; PHYSICAL MATERIAL RELATED FIELDS</strong></span></td></tr>");
                if (currentItem.Tracking.Born_Digital)
                {
                    result.AppendLine("<tr><td width=\"25px\">&nbsp;</td><td>Born Digital</td><td>&nbsp;</td></tr>");
                    result.AppendLine("<tr><td></td><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");

                }

                if (currentItem.Tracking.Disposition_Advice > 0)
                {
                    result.AppendLine("<tr><td width=\"25px\">&nbsp;</td><td>Disposition Advice</td><td>" + currentItem.Tracking.Disposition_Advice + "</td></tr>");
                    result.AppendLine("<tr><td></td><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");
                }

                result.AppendLine("</table>");
                result.AppendLine("<br />");
                result.AppendLine("<br />");
            }

            result.AppendLine("<table width=\"450px\">");
            result.AppendLine("<tr height=\"20px\" bgcolor=\"#7d90d5\"><td colspan=\"3\"><span style=\"color: White\"><strong> &nbsp; ARCHIVING MILESTONES</strong></span></td></tr>");
            if ((!currentItem.Tracking.Locally_Archived) && (!currentItem.Tracking.Remotely_Archived))
            {
                result.AppendLine("<tr><td width=\"25px\">&nbsp;</td><td>NOT ARCHIVED</td><td>&nbsp;</td></tr>");
                result.AppendLine("<tr><td></td><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");

            }
            else
            {
                if (currentItem.Tracking.Locally_Archived)
                {
                    result.AppendLine("<tr><td width=\"25px\">&nbsp;</td><td>Locally Stored on CD or Tape</td><td>&nbsp;</td></tr>");
                    result.AppendLine("<tr><td></td><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");

                }

                if (currentItem.Tracking.Remotely_Archived)
                {
                    result.AppendLine("<tr><td width=\"25px\">&nbsp;</td><td>Archived Remotely (FDA)</td><td>&nbsp;</td></tr>");
                    result.AppendLine("<tr><td></td><td bgcolor=\"#e7e7e7\" colspan=\"2\"></td></tr>");
                }
            }

            result.AppendLine("</table>");
            result.AppendLine("<br />");
            result.AppendLine("<br />");

            result.AppendLine("</blockquote>");
            result.AppendLine("</span>");
            result.AppendLine("<br />");
            return result.ToString();

        }


        #endregion

        #region Nested type: Tracking_Type

        private enum Tracking_Type : byte { Milestones, History, Media, Archives, Directory_List };

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
