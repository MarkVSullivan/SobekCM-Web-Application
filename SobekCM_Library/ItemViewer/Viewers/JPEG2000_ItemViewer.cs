using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using SobekCM.Core.BriefItem;
using SobekCM.Core.Configuration.Localization;
using SobekCM.Core.FileSystems;
using SobekCM.Core.Navigation;
using SobekCM.Core.Users;
using SobekCM.Library.HTML;
using SobekCM.Library.ItemViewer.Menu;
using SobekCM.Library.UI;
using SobekCM.Tools;

namespace SobekCM.Library.ItemViewer.Viewers
{
    /// <summary> JPEG2000 viewer prototyper, which is used to check to see if a JPEG2000 file exists, 
    /// to create the link in the main menu, and to create the viewer itself if the user selects that option </summary>
    public class JPEG2000_ItemViewer_Prototyper : iItemViewerPrototyper
    {
        /// <summary> Constructor for a new instance of the JPEG2000_ItemViewer_Prototyper class </summary>
        public JPEG2000_ItemViewer_Prototyper()
        {
            ViewerType = "JPEG2000";
            ViewerCode = "#x";
            FileExtensions = new string[] { "JP2" };
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

            return CurrentItem.Web.Contains_File_Extension("JP2");
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
            // Get the URL for this
            string previous_code = CurrentRequest.ViewerCode;
            CurrentRequest.ViewerCode = ViewerCode.Replace("#", "1");
            string url = UrlWriterHelper.Redirect_URL(CurrentRequest);
            CurrentRequest.ViewerCode = previous_code;

            // Add the item menu information
            Item_MenuItem menuItem = new Item_MenuItem("Page Images", "Zoomable", null, url, ViewerCode);
            MenuItems.Add(menuItem);
        }

        /// <summary> Creates and returns the an instance of the <see cref="JPEG2000_ItemViewer"/> class for showing a zoomable 
        /// JPEG2000 image from a page within a digital resource during execution of a single HTTP request. </summary>
        /// <param name="CurrentItem"> Digital resource object </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Fully built and initialized <see cref="JPEG2000_ItemViewer"/> object </returns>
        /// <remarks> This method is called whenever a request requires the actual viewer to be created to render the HTML for
        /// the digital resource requested.  The created viewer is then destroyed at the end of the request </remarks>
        public iItemViewer Create_Viewer(BriefItemInfo CurrentItem, User_Object CurrentUser, Navigation_Object CurrentRequest, Custom_Tracer Tracer)
        {
            return new JPEG2000_ItemViewer(CurrentItem, CurrentUser, CurrentRequest, Tracer, ViewerCode, FileExtensions);
        }
    }

    /// <summary> Item page viewer displays the a zoomable JPEG2000 from the page images within a digital resource. </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractPageFilesItemViewer"/> and implements the 
    /// <see cref="iItemViewer" /> interface. </remarks>
    public class JPEG2000_ItemViewer : abstractPageFilesItemViewer
    {
        private readonly bool suppressNavigator;

        // information about the file to display
        private readonly int page;
        private string filename;

        /// <summary> Constructor for a new instance of the JPEG2000_ItemViewer class, used to display JPEG2000s linked to
        /// pages in a digital resource </summary>
        /// <param name="BriefItem"> Digital resource object </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        public JPEG2000_ItemViewer(BriefItemInfo BriefItem, User_Object CurrentUser, Navigation_Object CurrentRequest, Custom_Tracer Tracer, string ViewerCode, string[] FileExtensions)
        {
            // Add the trace
            if (Tracer != null)
                Tracer.Add_Trace("JPEG2000_ItemViewer.Constructor");

            // Save the arguments for use later
            this.BriefItem = BriefItem;
            this.CurrentUser = CurrentUser;
            this.CurrentRequest = CurrentRequest;

            // Determine if the navigator ( in the left nav bar ) should be suppressed
            suppressNavigator = false;
            if (UI_ApplicationCache_Gateway.Settings.Contains_Additional_Setting("JPEG2000 ItemViewer.Suppress Navigator"))
            {
                if (UI_ApplicationCache_Gateway.Settings.Get_Additional_Setting("JPEG2000 ItemViewer.Suppress Navigator").ToLower().Trim() != "false")
                    suppressNavigator = true;
            }

            // Determine the page
            page = 1;
            if (!String.IsNullOrEmpty(CurrentRequest.ViewerCode))
            {
                int tempPageParse;
                if (Int32.TryParse(CurrentRequest.ViewerCode.Replace(ViewerCode.Replace("#", ""), ""), out tempPageParse))
                    page = tempPageParse;
            }

            // Just a quick range check
            if (page > BriefItem.Images.Count)
                page = 1;

            // Try to set the file information here
            if ((!set_file_information(FileExtensions)) && (page != 1))
            {
                // If there was an error, just set to the first page
                page = 1;
                set_file_information(FileExtensions);
            }

            // Since this is a paging viewer, set the viewer code
            if ( String.IsNullOrEmpty(CurrentRequest.ViewerCode))
                CurrentRequest.ViewerCode = ViewerCode.Replace("#", page.ToString());
        }

        private bool set_file_information(string[] FileExtensions)
        {
            // Find the page information
            BriefItem_FileGrouping imagePage = BriefItem.Images[page - 1];
            if (imagePage.Files != null)
            {
                // Step through each file in this page
                foreach (BriefItem_File thisFile in imagePage.Files)
                {
                    // Get this file extension
                    string extension = thisFile.File_Extension.Replace(".", "");

                    // Step through all permissable file extensions
                    foreach (string thisPossibleFileExtension in FileExtensions)
                    {
                        if (String.Compare(extension, thisPossibleFileExtension, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            // Get the JPEG information
                            filename = thisFile.Name;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary> Gets the collection of special behaviors which this item viewer
        /// requests from the main HTML subwriter. </summary>
        public override List<HtmlSubwriter_Behaviors_Enum> ItemViewer_Behaviors
        {
            get
            {
                // If the navigator will be  shown, we need a left nav bar, so return different behaviors
                if (!suppressNavigator)
                {
                    return new List<HtmlSubwriter_Behaviors_Enum> { HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Suppress_Bottom_Pagination, HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Requires_Left_Navigation_Bar };
                }
                return new List<HtmlSubwriter_Behaviors_Enum> { HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Suppress_Bottom_Pagination };

            }
        }

        /// <summary> Write any additional values within the HTML Head of the final served page </summary>
        /// <param name="Output"> Output stream currently within the HTML head tags </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Within_HTML_Head(TextWriter Output, Custom_Tracer Tracer)
        {
            Output.WriteLine("<script src=\"http://cdn.sobekrepository.org/includes/openseadragon/1.2.1/openseadragon.min.js\"></script>");
        }

        /// <summary> Gets the collection of body attributes to be included 
        /// within the HTML body tag (usually to add events to the body) </summary>
        /// <param name="Body_Attributes"> List of body attributes to be included </param>
        public override void Add_ViewerSpecific_Body_Attributes(List<Tuple<string, string>> Body_Attributes)
        {
            Body_Attributes.Add(new Tuple<string, string>("onload", "jp2_set_fullscreen();"));
            Body_Attributes.Add(new Tuple<string, string>("onresize", "jp2_set_fullscreen();"));
        }


        /// <summary> Adds any viewer_specific information to the left Navigation Bar Menu Section  </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Left_Nav_Menu_Section(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("JPEG2000_ItemViewer.Write_Nav_Bar_Menu_Section", "Adds small thumbnail for image navigation");
            }

            if (suppressNavigator)
                return;

            string thumnbnail_text = "THUMBNAIL";

            if (CurrentRequest.Language == Web_Language_Enum.French)
            {
                thumnbnail_text = "MINIATURE";
            }

            if (CurrentRequest.Language == Web_Language_Enum.Spanish)
            {
                thumnbnail_text = "MINIATURA";
            }

            Output.WriteLine("        <ul class=\"sbkIsw_NavBarMenu\">");
            Output.WriteLine("          <li class=\"sbkIsw_NavBarHeader\"> " + thumnbnail_text + " </li>");
            Output.WriteLine("          <li class=\"sbkIsw_NavBarMenuNonLink\">");
            Output.WriteLine("            <div id=\"sbkJp2_Navigator\"></div>");
            Output.WriteLine("            <br />");
            Output.WriteLine("          </li>");
            Output.WriteLine("        </ul>");
        }

        /// <summary> Write the item viewer main section as HTML directly to the HTTP output stream </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("JPEG2000_ItemViewer.Write_Main_Viewer_Section", "Adds the container for the zoomable image");
            }

            Output.WriteLine("<td>");
            Output.WriteLine("<div id=\"sbkJp2_Container\" ></div>");
            Output.WriteLine();
            Output.WriteLine("<script type=\"text/javascript\">");
            Output.WriteLine("   viewer = OpenSeadragon({");
            Output.WriteLine("      id: \"sbkJp2_Container\",");
            Output.WriteLine("      prefixUrl : \"http://cdn.sobekrepository.org/includes/openseadragon/1.2.1/images/\",");

            if (suppressNavigator)
            {

                Output.WriteLine("      showNavigator:  false");
            }
            else
            {
                Output.WriteLine("      showNavigator:  true,");
                Output.WriteLine("      navigatorId:  \"sbkJp2_Navigator\",");

                // Doesn't actually set the navigator size (the CSS does), but setting this means
                // OpenSeaDragon won't try to set the width/height as a ratio of the main image.
                Output.WriteLine("      navigatorWidth:  \"195px\",");
                Output.WriteLine("      navigatorHeight:  \"195px\"");
            }

            Output.WriteLine("   });");
            Output.WriteLine();


            //add by Keven for FIU dPanther's separate image server
            if (UI_ApplicationCache_Gateway.Settings.Servers.Image_Server_Root != null)
                Output.WriteLine("   viewer.open(\"" + CurrentRequest.Base_URL + "iipimage/iipsrv.fcgi?DeepZoom=" + UI_ApplicationCache_Gateway.Settings.Servers.Image_Server_Root.Replace("\\", "/") + SobekFileSystem.AssociFilePath(BriefItem).Replace("\\", "/") + filename + ".dzi\");");
            else
                Output.WriteLine("   viewer.open(\"" + CurrentRequest.Base_URL + "iipimage/iipsrv.fcgi?DeepZoom=" + UI_ApplicationCache_Gateway.Settings.Servers.Image_Server_Network.Replace("\\", "/") + SobekFileSystem.AssociFilePath(BriefItem).Replace("\\", "/") + filename + ".dzi\");");

            Output.WriteLine("</script>");
            Output.WriteLine("</td>");
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
