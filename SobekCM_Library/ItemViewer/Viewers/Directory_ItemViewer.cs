using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using SobekCM.Core.BriefItem;
using SobekCM.Core.FileSystems;
using SobekCM.Core.Navigation;
using SobekCM.Core.Users;
using SobekCM.Library.ItemViewer.Menu;
using SobekCM.Library.UI;
using SobekCM.Tools;

namespace SobekCM.Library.ItemViewer.Viewers
{
    /// <summary> Directory item viewer prototyper, which is used to check to see if a user has access to view the full file
    /// list from the directory for a digital resource, and to create the viewer itself if the user selects that option </summary>
    public class Directory_ItemViewer_Prototyper : iItemViewerPrototyper
    {
        /// <summary> Constructor for a new instance of the Directory_ItemViewer_Prototyper class </summary>
        public Directory_ItemViewer_Prototyper()
        {
            ViewerType = "DIRECTORY";
            ViewerCode = "directory";
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
            // This should always be included (although it won't be accessible or shown to everyone)
            return true;
        }

        /// <summary> Flag indicates if this viewer should be override on checkout </summary>
        /// <param name="CurrentItem"> Digital resource to examine to see if this viewer should really be overriden </param>
        /// <returns> TRUE always, since PDFs should never be shown if an item is checked out </returns>
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
            // If there is no user (or they aren't logged in) then obviously, they can't edit this
            if ((CurrentUser == null) || (!CurrentUser.LoggedOn))
            {
                return false;
            }

            // If INTERNAL, user has access
            if ((CurrentUser.Is_Host_Admin) || (CurrentUser.Is_System_Admin) || (CurrentUser.Is_Portal_Admin) || (CurrentUser.Is_Internal_User))
                return true;

            // See if this user can edit this item
            bool userCanEditItem = CurrentUser.Can_Edit_This_Item(CurrentItem.BibID, CurrentItem.Type, CurrentItem.Behaviors.Source_Institution_Aggregation, CurrentItem.Behaviors.Holding_Location_Aggregation, CurrentItem.Behaviors.Aggregation_Code_List);
            if (!userCanEditItem)
            {
                // Can't edit, so don't show and return FALSE
                return false;
            }

            // Apparently it can be shown
            return true;
        }

        /// <summary> Gets the menu items related to this viewer that should be included on the main item (digital resource) menu </summary>
        /// <param name="CurrentItem"> Digital resource object, which can be used to ensure if and how this viewer should appear 
        /// in the main item (digital resource) menu </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        /// <param name="MenuItems"> List of menu items, to which this method may add one or more menu items </param>
        /// <param name="IpRestricted"> Flag indicates if this item is IP restricted AND if the current user is outside the ranges </param>
        public void Add_Menu_items(BriefItemInfo CurrentItem, User_Object CurrentUser, Navigation_Object CurrentRequest, List<Item_MenuItem> MenuItems, bool IpRestricted)
        {
            // Do nothing since this is already handed and added to the menu by the MANAGE MENU item viewer and INTERNAL header
        }

        /// <summary> Creates and returns the an instance of the <see cref="Directory_ItemViewer"/> class for showing the
        /// list of all files in the resource directory during execution of a single HTTP request. </summary>
        /// <param name="CurrentItem"> Digital resource object </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Fully built and initialized <see cref="Directory_ItemViewer"/> object </returns>
        /// <remarks> This method is called whenever a request requires the actual viewer to be created to render the HTML for
        /// the digital resource requested.  The created viewer is then destroyed at the end of the request </remarks>
        public iItemViewer Create_Viewer(BriefItemInfo CurrentItem, User_Object CurrentUser, Navigation_Object CurrentRequest, Custom_Tracer Tracer)
        {
            return new Directory_ItemViewer(CurrentItem, CurrentUser, CurrentRequest, Tracer);
        }
    }

    /// <summary> Item viewer displays the list of files from the resource directory </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractNoPaginationItemViewer"/> and implements the 
    /// <see cref="iItemViewer" /> interface. </remarks>
    public class Directory_ItemViewer : abstractNoPaginationItemViewer
    {
        /// <summary> Constructor for a new instance of the Directory_ItemViewer class, used display
        /// the tracking milestones information for a digital resource </summary>
        /// <param name="BriefItem"> Digital resource object </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public Directory_ItemViewer(BriefItemInfo BriefItem, User_Object CurrentUser, Navigation_Object CurrentRequest, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Directory_ItemViewer.Constructor");

            // Save the arguments for use later
            this.BriefItem = BriefItem;
            this.CurrentUser = CurrentUser;
            this.CurrentRequest = CurrentRequest;

            // Set the behavior properties to the empy behaviors ( in the base class )
            Behaviors = EmptyBehaviors;
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
            Tracer.Add_Trace("Directory_ItemViewer.Write_Main_Viewer_Section", "");

            // Add the HTML for the image
            Output.WriteLine("<!-- DIRECTORY ITEM VIEWER OUTPUT -->");

            // Start the citation table
            Output.WriteLine("  <td align=\"left\"><span class=\"sbkTrk_ViewerTitle\">Tracking Information</span></td>");
            Output.WriteLine("</tr>");
            Output.WriteLine("<tr>");
            Output.WriteLine("  <td class=\"sbkTrk_MainArea\">");

            // Add the tabs for related admin viewers (if they exist)
            Tracking_ItemViewer.Write_Tracking_Tabs(Output, CurrentRequest, BriefItem);

            Tracer.Add_Trace("Directory_ItemViewer.Directory_String", "Pulling and displaying files in the image directory");

            try
            {
                string directory = SobekFileSystem.Resource_Network_Uri(BriefItem);
                string url = SobekFileSystem.Resource_Web_Uri(BriefItem);
                List<SobekFileSystem_FileInfo> files = SobekFileSystem.GetFiles(BriefItem);

                // Get all the file info objects and order by name
                SortedList<string, SobekFileSystem_FileInfo> sortedFiles = new SortedList<string, SobekFileSystem_FileInfo>();
                foreach (SobekFileSystem_FileInfo thisFile in files)
                {
                    sortedFiles.Add(thisFile.Name.ToUpper(), thisFile);
                }

                // Remove the THUMBS.DB file, if it exists
                if (sortedFiles.ContainsKey("THUMBS.DB"))
                    sortedFiles.Remove("THUMBS.DB");

                // Start the file table
                Output.WriteLine("<br />");
                Output.WriteLine("<br />");
                Output.WriteLine("<blockquote>");
                Output.WriteLine(" &nbsp; &nbsp; <a href=\"" + directory + "\">" + directory + "</a>");
                Output.WriteLine("</blockquote>");

                Output.WriteLine("<blockquote>");

                // Add all the page images first
                List<BriefItem_FileGrouping> nodes = BriefItem.Images;
                if ((nodes != null) && (nodes.Count > 0))
                {
                    Output.WriteLine("<span style=\"font-size:1.4em; color:#888888;\"><b>PAGE FILES</b></span><br />");
                    Output.WriteLine("<table border=\"0px\" cellspacing=\"0px\" class=\"statsTable\">");
                    Output.WriteLine("<tr align=\"left\" bgcolor=\"#0022a7\" height=\"20px\" >");
                    Output.WriteLine("<th align=\"left\"><span style=\"color: White\">NAME</span></th>");
                    Output.WriteLine("<th width=\"10px\">&nbsp;</th>");
                    Output.WriteLine("<th align=\"left\" width=\"170px\"><span style=\"color: White\">DATE MODIFIED</span></th>");
                    Output.WriteLine("<th align=\"left\" width=\"180px\"><span style=\"color: White\">TYPE</span></th>");
                    Output.WriteLine("<th width=\"10px\">&nbsp;</th>");
                    Output.WriteLine("<th align=\"right\"><span style=\"color: White\">SIZE</span></th>");
                    Output.WriteLine("</tr>");

                    List<string> file_names_added = new List<string>();
                    foreach (BriefItem_FileGrouping thisNode in nodes)
                    {
                        // Only show pages with files
                        if (thisNode.Files.Count > 0)
                        {
                            // Ensure that if a page is repeated, it only is written once
                            string[] filename_splitter = thisNode.Files[0].Name.Split(".".ToCharArray());

                            string fileName = filename_splitter[0].ToUpper();
                            if (filename_splitter.Length > 1)
                                fileName = filename_splitter[filename_splitter.Length - 2].ToUpper();
                            if (!file_names_added.Contains(fileName))
                            {
                                file_names_added.Add(fileName);

                                Output.WriteLine("<tr align=\"left\" bgcolor=\"#7d90d5\">");
                                string pageName = thisNode.Label;
                                if (pageName.Length == 0)
                                    pageName = "PAGE";

                                Output.WriteLine("<td colspan=\"6\" ><span style=\"color: White\"><b>" + pageName.ToUpper() + "</b></span></td>");
                                Output.WriteLine("</tr>");

                                // Now, check for each file
                                foreach (BriefItem_File thisFile in thisNode.Files)
                                {
                                    string thisFileUpper = thisFile.Name.ToUpper();
                                    if (sortedFiles.ContainsKey(thisFileUpper))
                                    {
                                        // string file = UI_ApplicationCache_Gateway.Settings.Servers.Image_Server_Network + currentItem.Web.AssocFilePath + thisFile.System_Name;
                                        Add_File_HTML(sortedFiles[thisFileUpper], Output, url, true);
                                        sortedFiles.Remove(thisFileUpper);
                                    }
                                }

                                // Ensure that there still aren't some page files that exist that were not linked
                                string[] other_page_file_endings = { ".JPG", ".JP2", "THM.JPG", ".TXT", ".PRO", ".QC.JPG" };
                                foreach (string thisFileEnder in other_page_file_endings)
                                {
                                    if (sortedFiles.ContainsKey(fileName.ToUpper() + thisFileEnder))
                                    {
                                        //string file = UI_ApplicationCache_Gateway.Settings.Servers.Image_Server_Network + currentItem.Web.AssocFilePath + fileName + thisFileEnder.ToLower();
                                        Add_File_HTML(sortedFiles[fileName.ToUpper() + thisFileEnder], Output, url, true);
                                        sortedFiles.Remove(fileName.ToUpper() + thisFileEnder);
                                    }
                                }
                            }
                        }
                    }

                    // FInish the table
                    Output.WriteLine("</table>");
                    Output.WriteLine("<br /><br />");
                }

                // Add all the metadata files
                Output.WriteLine("<span style=\"font-size:1.4em; color:#888888;\"><b>METADATA FILES</b></span><br />");
                Output.WriteLine("<table border=\"0px\" cellspacing=\"0px\" class=\"statsTable\">");
                Output.WriteLine("<tr align=\"left\" bgcolor=\"#0022a7\" height=\"20px\" >");
                Output.WriteLine("<th align=\"left\"><span style=\"color: White\">NAME</span></th>");
                Output.WriteLine("<th width=\"10px\">&nbsp;</th>");
                Output.WriteLine("<th align=\"left\" width=\"170px\"><span style=\"color: White\">DATE MODIFIED</span></th>");
                Output.WriteLine("<th align=\"left\" width=\"180px\"><span style=\"color: White\">TYPE</span></th>");
                Output.WriteLine("<th width=\"10px\">&nbsp;</th>");
                Output.WriteLine("<th align=\"right\"><span style=\"color: White\">SIZE</span></th>");
                Output.WriteLine("</tr>");

                // Add each metadata file
                List<string> files_handled = new List<string>();
                foreach (string thisFile in sortedFiles.Keys.Where(ThisFile => (ThisFile.IndexOf(".METS.BAK") > 0) || (ThisFile.IndexOf(".METS.XML") > 0) || (ThisFile == "DOC.XML") || (ThisFile == "MARC.XML") || (ThisFile == "CITATION_METS.XML") || (ThisFile == BriefItem.BibID.ToUpper() + "_" + BriefItem.VID + ".HTML")))
                {
                    files_handled.Add(thisFile);
                    Add_File_HTML(sortedFiles[thisFile], Output, url, true);
                }

                // REmove all handled files
                foreach (string thisKey in files_handled)
                    sortedFiles.Remove(thisKey);

                // FInish the table
                Output.WriteLine("</table>");
                Output.WriteLine("<br /><br />");

                // Finally, add all the remaining files
                if (sortedFiles.Count > 0)
                {
                    Output.WriteLine("<span style=\"font-size:1.4em; color:#888888;\"><b>OTHER FILES</b></span><br />");
                    Output.WriteLine("<table border=\"0px\" cellspacing=\"0px\" class=\"statsTable\">");
                    Output.WriteLine("<tr align=\"left\" bgcolor=\"#0022a7\" height=\"20px\" >");
                    Output.WriteLine("<th align=\"left\"><span style=\"color: White\">NAME</span></th>");
                    Output.WriteLine("<th width=\"10px\">&nbsp;</th>");
                    Output.WriteLine("<th align=\"left\" width=\"170px\"><span style=\"color: White\">DATE MODIFIED</span></th>");
                    Output.WriteLine("<th align=\"left\" width=\"180px\"><span style=\"color: White\">TYPE</span></th>");
                    Output.WriteLine("<th width=\"10px\">&nbsp;</th>");
                    Output.WriteLine("<th align=\"right\"><span style=\"color: White\">SIZE</span></th>");
                    Output.WriteLine("</tr>");

                    // Now add all the information
                    foreach (SobekFileSystem_FileInfo thisFile in sortedFiles.Values)
                    {
                        Add_File_HTML(thisFile, Output, url, true);
                    }

                    // FInish the table
                    Output.WriteLine("</table>");
                }
                Output.WriteLine("</blockquote>");
                Output.WriteLine("<br />");
            }
            catch
            {
                Output.WriteLine("<br /><center><strong>UNABLE TO PULL DIRECTORY INFORMATION</strong></center><br />");
            }

            Output.WriteLine("  </td>");
            Output.WriteLine("  <!-- END DIRECTORY VIEWER OUTPUT -->");
        }

        private void Add_File_HTML(SobekFileSystem_FileInfo thisFileInfo, TextWriter Output, string url, bool includeSizeAndDate)
        {
            Output.WriteLine("<tr>");
            Output.WriteLine("<td><a href=\"" + url + thisFileInfo.Name + "\">" + thisFileInfo.Name + "</a></td>");
            Output.WriteLine("<td>&nbsp;</td>");

            if (includeSizeAndDate)
            {
                Output.WriteLine("<td>" + thisFileInfo.LastWriteTime.ToString() + "</td>");
            }

            string extension = thisFileInfo.Extension.ToUpper().Replace(".", "");
            string type = Extension_To_File_Type(extension, thisFileInfo.Name);
            Output.WriteLine("<td>" + type + "</td>");

            if (includeSizeAndDate)
            {
                Output.WriteLine("<td>&nbsp;</td>");
                int size_kb = (int)(thisFileInfo.Length / 1024);
                if (size_kb > 1024)
                {
                    size_kb = size_kb / 1024;
                    Output.WriteLine("<td align=\"right\">" + size_kb + " MB</td>");
                }
                else
                {
                    if (size_kb == 0)
                        size_kb = 1;
                    Output.WriteLine("<td align=\"right\">" + size_kb + " KB</td>");
                }
            }

            Output.WriteLine("</tr>");
            Output.WriteLine(includeSizeAndDate ? "<tr><td bgcolor=\"#e7e7e7\" colspan=\"6\"></td></tr>" : "<tr><td bgcolor=\"#e7e7e7\" colspan=\"3\"></td></tr>");
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
                    type = fullname.ToUpper() == BriefItem.BibID.ToUpper() + "_" + BriefItem.VID + ".HTML" ? "Static citation page" : "HTML Document";
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
