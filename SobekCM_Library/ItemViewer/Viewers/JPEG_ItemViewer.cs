using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using SobekCM.Core.BriefItem;
using SobekCM.Core.Navigation;
using SobekCM.Core.Users;
using SobekCM.Tools;

namespace SobekCM.Library.ItemViewer.Viewers
{
    /// <summary> JPEG viewer prototyper, which is used to check to see if a (non-thumbnail) JPEG file exists, 
    /// to create the link in the main menu, and to create the viewer itself if the user selects that option </summary>
    public class JPEG_ItemViewer_Prototyper : iItemViewerPrototyper
    {
        /// <summary> Constructor for a new instance of the JPEG_ItemViewer_Prototyper class </summary>
        public JPEG_ItemViewer_Prototyper()
        {
            ViewerType = "JPEG";
            ViewerCode = "#j";
            FileExtensions = new string[] { "JPG" };
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
        public bool Override_On_Checkout(BriefItemInfo CurrentItem)
        {
            return true;
        }

        /// <summary> Flag indicates if the current user has access to this viewer for the item </summary>
        /// <param name="CurrentItem"> Digital resource to see if the current user has correct permissions to use this viewer </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="IpRestricted"> Flag indicates if this item is IP restricted AND if the current user is outside the ranges </param>
        /// <returns> TRUE if the user has access to use this viewer, otherwise FALSE </returns>
        public bool Has_Access(BriefItemInfo CurrentItem, User_Object CurrentUser, bool IpRestricted)
        {
            return !IpRestricted;
        }

        /// <summary> Gets the menu items related to this viewer that should be included on the main item (digital resource) menu </summary>
        /// <param name="CurrentItem"> Digital resource object, which can be used to ensure if and how this viewer should appear 
        /// in the main item (digital resource) menu </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        /// <param name="MenuItems"> List of menu items, to which this method may add one or more menu items </param>
        public void Add_Menu_items(BriefItemInfo CurrentItem, User_Object CurrentUser, Navigation_Object CurrentRequest, List<Item_MenuItem> MenuItems)
        {
            Item_MenuItem menuItem = new Item_MenuItem("Page Images", "Standard", null, CurrentItem.Web.Source_URL + ViewerCode);
            MenuItems.Add(menuItem);
        }

        /// <summary> Creates and returns the an instance of the <see cref="JPEG_ItemViewer"/> class for showing a  
        /// JPEG image from a page within a digital resource during execution of a single HTTP request. </summary>
        /// <param name="CurrentItem"> Digital resource object </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        /// <returns> Fully built and initialized <see cref="JPEG_ItemViewer"/> object </returns>
        /// <remarks> This method is called whenever a request requires the actual viewer to be created to render the HTML for
        /// the digital resource requested.  The created viewer is then destroyed at the end of the request </remarks>
        public iItemViewer Create_Viewer(BriefItemInfo CurrentItem, User_Object CurrentUser, Navigation_Object CurrentRequest)
        {
            return new JPEG_ItemViewer(CurrentItem, CurrentUser, CurrentRequest);
        }
    }

    /// <summary> Item page viewer displays the a JPEG from the page images within a digital resource. </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractPageFilesItemViewer"/> and implements the 
    /// <see cref="iItemViewer" /> interface. </remarks>
    public class JPEG_ItemViewer : abstractPageFilesItemViewer
    {
        private int width;
        private int height;
        private string filename;

        /// <summary> Constructor for a new instance of the JPEG_ItemViewer class, used to display JPEGs linked to
        /// pages in a digital resource </summary>
        /// <param name="BriefItem"> Digital resource object </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        public JPEG_ItemViewer(BriefItemInfo BriefItem, User_Object CurrentUser, Navigation_Object CurrentRequest)
        {
            // Save the arguments for use later
            this.BriefItem = BriefItem;
            this.CurrentUser = CurrentUser;
            this.CurrentRequest = CurrentRequest;

            // Set the behavior properties
            Behaviors = EmptyBehaviors;
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

            // Determine if there is a zoomable version of this page
            bool isZoomable = false;
            if ((BriefItem.Images != null) && (BriefItem.Images.Count > CurrentRequest.Page - 1))
            {
                int currentPageIndex = CurrentRequest.Page.HasValue ? CurrentRequest.Page.Value : 1;
                foreach (BriefItem_File thisFile in BriefItem.Images[currentPageIndex - 1].Files)
                {
                    if (String.Compare(thisFile.File_Extension, ".jp2", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        isZoomable = true;
                        break;
                    }
                }
            }

            // Now, check to see if the JPEG2000 viewer is included here
            bool zoomableViewerIncluded = false;
            foreach (BriefItem_BehaviorViewer viewer in BriefItem.Behaviors.Viewers)
            {
                if (String.Compare(viewer.ViewerType, "JPEG2000", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    zoomableViewerIncluded = true;
                    break;
                }
            }

            string displayFileName = BriefItem.Web.Source_URL + "/" + filename;

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
            if ((isZoomable) && (zoomableViewerIncluded))
            {
                string currViewer = CurrentRequest.ViewerCode;
                CurrentRequest.ViewerCode = CurrentRequest.ViewerCode.ToLower().Replace("j", "") + "x";
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
