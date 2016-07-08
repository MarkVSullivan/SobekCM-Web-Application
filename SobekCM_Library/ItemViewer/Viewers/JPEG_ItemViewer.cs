using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using SobekCM.Core.BriefItem;
using SobekCM.Core.FileSystems;
using SobekCM.Core.Navigation;
using SobekCM.Core.Users;
using SobekCM.Library.ItemViewer.Menu;
using SobekCM.Tools;

namespace SobekCM.Library.ItemViewer.Viewers
{
    /// <summary> JPEG viewer prototyper, which is used to check to see if a (non-thumbnail) JPEG file exists, 
    /// to create the link in the main menu, and to create the viewer itself if the user selects that option </summary>
    public class JPEG_ItemViewer_Prototyper : abstractItemViewerPrototyper
    {
        /// <summary> Constructor for a new instance of the JPEG_ItemViewer_Prototyper class </summary>
        public JPEG_ItemViewer_Prototyper()
        {
            ViewerType = "JPEG";
            ViewerCode = "#j";
            FileExtensions = new string[] { "JPG" };
        }

        /// <summary> Indicates if the specified item matches the basic requirements for this viewer, or
        /// if this viewer should be ignored for this item </summary>
        /// <param name="CurrentItem"> Digital resource to examine to see if this viewer really should be included </param>
        /// <returns> TRUE if this viewer should generally be included with this item, otherwise FALSE </returns>
        public override bool Include_Viewer(BriefItemInfo CurrentItem)
        {
            // Check to see if there are any PDF files attached, but allow the configuration 
            // to actually rule which files are necessary to be shown ( i.e., maybe 'PDFA' will be an extension
            // in the future )
            if (FileExtensions != null)
            {
                return FileExtensions.Any(Extension => CurrentItem.Web.Contains_File_Extension(Extension));
            }

            return CurrentItem.Web.Contains_File_Extension("JPG");
        }

        /// <summary> Flag indicates if this viewer should be override on checkout </summary>
        /// <param name="CurrentItem"> Digital resource to examine to see if this viewer should really be overriden </param>
        /// <returns> TRUE always, since PDFs should never be shown if an item is checked out </returns>
        public override bool Override_On_Checkout(BriefItemInfo CurrentItem)
        {
            return true;
        }

        /// <summary> Flag indicates if the current user has access to this viewer for the item </summary>
        /// <param name="CurrentItem"> Digital resource to see if the current user has correct permissions to use this viewer </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="IpRestricted"> Flag indicates if this item is IP restricted AND if the current user is outside the ranges </param>
        /// <returns> TRUE if the user has access to use this viewer, otherwise FALSE </returns>
        public override bool Has_Access(BriefItemInfo CurrentItem, User_Object CurrentUser, bool IpRestricted)
        {
            return !IpRestricted;
        }

        /// <summary> Gets the menu items related to this viewer that should be included on the main item (digital resource) menu </summary>
        /// <param name="CurrentItem"> Digital resource object, which can be used to ensure if and how this viewer should appear 
        /// in the main item (digital resource) menu </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        /// <param name="MenuItems"> List of menu items, to which this method may add one or more menu items </param>
        public override void Add_Menu_items(BriefItemInfo CurrentItem, User_Object CurrentUser, Navigation_Object CurrentRequest, List<Item_MenuItem> MenuItems)
        {
            // Get the URL for this
            string previous_code = CurrentRequest.ViewerCode;
            CurrentRequest.ViewerCode = ViewerCode.Replace("#","1");
            string url = UrlWriterHelper.Redirect_URL(CurrentRequest);
            CurrentRequest.ViewerCode = previous_code;

            // Add the item menu information
            Item_MenuItem menuItem = new Item_MenuItem("Page Images", "Standard", null, url, ViewerCode);
            MenuItems.Add(menuItem);
        }

        /// <summary> Creates and returns the an instance of the <see cref="JPEG_ItemViewer"/> class for showing a  
        /// JPEG image from a page within a digital resource during execution of a single HTTP request. </summary>
        /// <param name="CurrentItem"> Digital resource object </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Fully built and initialized <see cref="JPEG_ItemViewer"/> object </returns>
        /// <remarks> This method is called whenever a request requires the actual viewer to be created to render the HTML for
        /// the digital resource requested.  The created viewer is then destroyed at the end of the request </remarks>
        public override iItemViewer Create_Viewer(BriefItemInfo CurrentItem, User_Object CurrentUser, Navigation_Object CurrentRequest, Custom_Tracer Tracer)
        {
            return new JPEG_ItemViewer(CurrentItem, CurrentUser, CurrentRequest, Tracer, ViewerCode.ToLower(), FileExtensions);
        }
    }

    /// <summary> Item page viewer displays the a JPEG from the page images within a digital resource. </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractPageFilesItemViewer"/> and implements the 
    /// <see cref="iItemViewer" /> interface. </remarks>
    public class JPEG_ItemViewer : abstractPageFilesItemViewer
    {
        // information about the page to display
        private readonly int page;
        private int width;
        private int height;
        private string filename;

        // properties about linking to the zoomable file
        private bool includeLinkToZoomable;
        private readonly string zoomableViewerCode;

        /// <summary> Constructor for a new instance of the JPEG_ItemViewer class, used to display JPEGs linked to
        /// pages in a digital resource </summary>
        /// <param name="BriefItem"> Digital resource object </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <param name="JPEG_ViewerCode"> JPEG viewer code, as determined by configuration files </param>
        /// <param name="FileExtensions"> File extensions that this viewer allows, as determined by configuration files </param>
        public JPEG_ItemViewer(BriefItemInfo BriefItem, User_Object CurrentUser, Navigation_Object CurrentRequest, Custom_Tracer Tracer, string JPEG_ViewerCode, string[] FileExtensions)
        {
            // Add the trace
            if ( Tracer != null )
                Tracer.Add_Trace("JPEG_ItemViewer.Constructor");

            // Save the arguments for use later
            this.BriefItem = BriefItem;
            this.CurrentUser = CurrentUser;
            this.CurrentRequest = CurrentRequest;

            // Set the behavior properties
            Behaviors = EmptyBehaviors;

            // Is the JPEG2000 viewer included in this item?
            bool zoomableViewerIncluded = BriefItem.UI.Includes_Viewer_Type("JPEG2000");
            string[] jpeg2000_extensions = null;
            if (zoomableViewerIncluded)
            {
                iItemViewerPrototyper jp2Prototyper = ItemViewer_Factory.Get_Viewer_By_ViewType("JPEG2000");
                if (jp2Prototyper == null)
                    zoomableViewerIncluded = false;
                else
                {
                    zoomableViewerCode = jp2Prototyper.ViewerCode;
                    jpeg2000_extensions = jp2Prototyper.FileExtensions;
                }
            }

            // Set some default values
            width = 500;
            height = -1;
            includeLinkToZoomable = false;

            // Determine the page
            page = 1;
            if (!String.IsNullOrEmpty(CurrentRequest.ViewerCode))
            {
                int tempPageParse;
                if (Int32.TryParse(CurrentRequest.ViewerCode.Replace(JPEG_ViewerCode.Replace("#",""), ""), out tempPageParse))
                    page = tempPageParse;
            }

            // Just a quick range check
            if (page > BriefItem.Images.Count)
                page = 1;

            // Try to set the file information here
            if ((!set_file_information(FileExtensions, zoomableViewerIncluded, jpeg2000_extensions)) && (page != 1))
            {
                // If there was an error, just set to the first page
                page = 1;
                set_file_information(FileExtensions, zoomableViewerIncluded, jpeg2000_extensions);
            }

            // Since this is a paging viewer, set the viewer code
            if (String.IsNullOrEmpty(CurrentRequest.ViewerCode))
                CurrentRequest.ViewerCode = JPEG_ViewerCode.Replace("#", page.ToString());

        }

        private bool set_file_information(string[] FileExtensions, bool zoomableViewerIncluded, string[] zoomableFileExtensions )
        {
            bool returnValue = false;
            includeLinkToZoomable = false;
            bool width_found = false;

            // Find the page information
            BriefItem_FileGrouping imagePage = BriefItem.Images[page - 1];
            if (imagePage.Files != null)
            {
                // Step through each file in this page
                foreach (BriefItem_File thisFile in imagePage.Files)
                {
                    // Get this file extension
                    string extension = thisFile.File_Extension.Replace(".","");

                    // Step through all permissable file extensions
                    foreach( string thisPossibleFileExtension in FileExtensions )
                    {
                        if (String.Compare(extension, thisPossibleFileExtension, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            // If a return value was already found, look to see if this one is bigger, in which case it will be used
                            // This is a convenient way to get around thumbnails issue without looking for "thm.jpg"
                            if (returnValue)
                            {
                                // Are their widths present?
                                if (width_found && thisFile.Width.HasValue)
                                {
                                    if (thisFile.Width.Value > width)
                                    {
                                        // THis file is bigger (wider)
                                        filename = thisFile.Name;
                                        width = thisFile.Width.Value;
                                        if (thisFile.Height.HasValue) height = thisFile.Height.Value;
                                    }
                                }
                                else
                                {
                                    // Since no width was found, just go for a shorter filename
                                    if (filename.Length > thisFile.Name.Length)
                                    {
                                        // This name is shorter... assuming it doesn't include thm.jpg then
                                        filename = thisFile.Name;
                                        if (thisFile.Width.HasValue)
                                        {
                                            width = thisFile.Width.Value;
                                            width_found = true;
                                        }
                                        else
                                        {
                                            width = 500;
                                        }
                                        if (thisFile.Height.HasValue) height = thisFile.Height.Value;
                                    }
                                }
                            }
                            else
                            {
                                // Get the JPEG information
                                filename = thisFile.Name;
                                if (thisFile.Width.HasValue)
                                {
                                    width = thisFile.Width.Value;
                                    width_found = true;
                                }
                                if (thisFile.Height.HasValue) height = thisFile.Height.Value;
                            }



                            // Get the JPEG information

                            filename = thisFile.Name;
                            if (thisFile.Width.HasValue)
                            {
                                width = thisFile.Width.Value;
                                width_found = true;
                            }
                            if (thisFile.Height.HasValue) height = thisFile.Height.Value;

                            // Found a value to return
                            returnValue = true;
                        }
                    }

                    // Also look for the JPEG2000 viewers
                    if (zoomableViewerIncluded)
                    {
                        // Step through all JPEG2000 extensions
                        foreach (string thisPossibleFileExtension in zoomableFileExtensions)
                        {
                            if (String.Compare(extension, thisPossibleFileExtension, StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                // Found a jpeg2000
                                includeLinkToZoomable = true;
                                break;
                            }
                        }
                    }
                }

                // Finished looking at all the page files and found the file to display, so return TRUE
                if (returnValue) return true;
            }

            return false;
        }

        /// <summary> Any additional inline style for this viewer that affects the main box around this</summary>
        /// <remarks> This returns the width of the image for the width of the viewer port </remarks>
        public override string ViewerBox_InlineStyle
        {
            get
            {
                if (width < 500)
                    return "width:500px;";
                return "width:" + width + "px;";
            }
        }

        /// <summary> Write the item viewer main section as HTML directly to the HTTP output stream </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("JPEG_ItemViewer.Write_Main_Viewer_Section", "");
            }

            string displayFileName = SobekFileSystem.Resource_Web_Uri(BriefItem, filename);

            // MAKE THIS USE THE FILES.ASPX WEB PAGE if this is restricted (or dark)
            if ((BriefItem.Behaviors.Dark_Flag) || (BriefItem.Behaviors.IP_Restriction_Membership > 0))
                displayFileName = CurrentRequest.Base_URL + "files/" + BriefItem.BibID + "/" + BriefItem.VID + "/" + filename;


            string name_for_image = HttpUtility.HtmlEncode(BriefItem.Title);


            if ((BriefItem.Images != null) && (BriefItem.Images.Count > 1) && (Current_Page - 1 < BriefItem.Images.Count))
            {
                string name_of_page = BriefItem.Images[Current_Page - 1].Label;
                name_for_image = name_for_image + " - " + HttpUtility.HtmlEncode(name_of_page);
            }



            // Add the HTML for the image
            if (includeLinkToZoomable)
            {
                string currViewer = CurrentRequest.ViewerCode;
                CurrentRequest.ViewerCode = zoomableViewerCode.Replace("#", page.ToString());
                string toZoomable = UrlWriterHelper.Redirect_URL(CurrentRequest);
                CurrentRequest.ViewerCode = currViewer;
                Output.WriteLine("\t\t<td id=\"sbkJiv_ImageZoomable\">");
                Output.WriteLine("Click on image below to switch to zoomable version<br />");
                Output.WriteLine("<a href=\"" + toZoomable + "\" title=\"Click on image to switch to zoomable version\">");

                Output.Write("\t\t\t<img itemprop=\"primaryImageOfPage\" ");
                if ((height > 0) && (width > 0))
                    Output.Write("style=\"height:" + height + "px;width:" + width + "px;\" ");
                Output.WriteLine("src=\"" + displayFileName + "\" alt=\"" + name_for_image + "\" />");

                Output.WriteLine("</a>");
            }
            else
            {
                Output.WriteLine("\t\t<td align=\"center\" id=\"sbkJiv_Image\">");

                Output.Write("\t\t\t<img itemprop=\"primaryImageOfPage\" ");
                if ((height > 0) && (width > 0))
                    Output.Write("style=\"height:" + height + "px;width:" + width + "px;\" ");
                Output.WriteLine("src=\"" + displayFileName + "\" alt=\"" + name_for_image + "\" />");
            }

            Output.WriteLine("\t\t</td>");
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
