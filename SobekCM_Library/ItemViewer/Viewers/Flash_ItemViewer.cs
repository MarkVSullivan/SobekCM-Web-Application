using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using SobekCM.Core.BriefItem;
using SobekCM.Core.Navigation;
using SobekCM.Core.Users;
using SobekCM.Tools;

namespace SobekCM.Library.ItemViewer.Viewers
{
    /// <summary> Flash (SWF) viewer prototyper, which is used to check to see if the flash viewer should really be shown, 
    /// to create the link in the main menu, and to create the viewer itself if the user selects that option </summary>
    public class Flash_ItemViewer_Prototyper : iItemViewerPrototyper
    {
        /// <summary> Constructor for a new instance of the Flash_ItemViewer_Prototyper class </summary>
        public Flash_ItemViewer_Prototyper()
        {
            ViewerType = "FLASH";
            ViewerCode = "swf";
            FileExtensions = new string[] { "SWF" };
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

            return CurrentItem.Web.Contains_File_Extension("SWF");
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
        /// <param name="MenuItems"> List of menu items, to which this method may add one or more menu items </param>
        public void Add_Menu_items(BriefItemInfo CurrentItem, User_Object CurrentUser, List<Item_MenuItem> MenuItems)
        {
            string first_label = "FLASH";
            foreach (BriefItem_FileGrouping thisPage in CurrentItem.Downloads)
            {
                // Look for a flash file on each page
                foreach (BriefItem_File thisFile in thisPage.Files)
                {
                    if (String.Compare(thisFile.File_Extension, ".SWF", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        first_label = thisPage.Label;
                        break;
                    }
                }
            }

            Item_MenuItem menuItem = new Item_MenuItem(first_label, null, null, CurrentItem.Web.Source_URL + ViewerCode);
            MenuItems.Add(menuItem);
        }

        /// <summary> Creates and returns the an instance of the <see cref="Flash_ItemViewer"/> class for showing a flash file
        /// from a digital resource during execution of a single HTTP request. </summary>
        /// <param name="CurrentItem"> Digital resource object </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        /// <returns> Fully built and initialized <see cref="Flash_ItemViewer"/> object </returns>
        /// <remarks> This method is called whenever a request requires the actual viewer to be created to render the HTML for
        /// the digital resource requested.  The created viewer is then destroyed at the end of the request </remarks>
        public iItemViewer Create_Viewer(BriefItemInfo CurrentItem, User_Object CurrentUser, Navigation_Object CurrentRequest)
        {
            return new Flash_ItemViewer(CurrentItem, CurrentUser, CurrentRequest);
        }
    }

    /// <summary> Item viewer displays a single flash file for a digital resource </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractNoPaginationItemViewer"/> and implements the 
    /// <see cref="iItemViewer" /> interface. </remarks>
    public class Flash_ItemViewer : abstractNoPaginationItemViewer
    {
        private readonly string flash_file;
        private readonly string flash_label;

        /// <summary> Constructor for a new instance of the Flash_ItemViewer class, used to display a
        /// flash file, or flash video, for a given digital resource </summary>
        /// <param name="BriefItem"> Digital resource object </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        public Flash_ItemViewer(BriefItemInfo BriefItem, User_Object CurrentUser, Navigation_Object CurrentRequest)
        {
            // Save the arguments for use later
            this.BriefItem = BriefItem;
            this.CurrentUser = CurrentUser;
            this.CurrentRequest = CurrentRequest;

            // Set the behavior properties to the empy behaviors ( in the base class )
            Behaviors = EmptyBehaviors;

            // Find the appropriate flash files and label
            flash_file = String.Empty;
            flash_label = String.Empty;
            string first_label = String.Empty;
            string first_file = String.Empty;

            int current_flash_index = 0;
            const int FLASH_INDEX = 0;
            foreach (BriefItem_FileGrouping thisPage in BriefItem.Downloads)
            {
                // Look for a flash file on each page
                foreach (BriefItem_File thisFile in thisPage.Files)
                {
                    if (String.Compare(thisFile.File_Extension, ".SWF", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        // If this is the first one, assign it
                        if (String.IsNullOrEmpty(first_file))
                        {
                            first_file = thisFile.Name;
                            first_label = thisPage.Label;
                        }

                        if (current_flash_index == FLASH_INDEX)
                        {
                            flash_file = thisFile.Name;
                            flash_label = thisPage.Label;
                            break;
                        }
                        current_flash_index++;
                    }
                }
            }

            // If none found, but a first was found, use that
            if ((String.IsNullOrEmpty(flash_file)) && (!String.IsNullOrEmpty(first_file)))
            {
                flash_file = first_file;
                flash_label = first_label;
            }

            // If this is not already a link format, make it one
            if (( !String.IsNullOrEmpty(flash_file)) && ( flash_file.IndexOf("http:") < 0))
            {
                flash_file = BriefItem.Web.Source_URL + "/" + flash_file;
            }
        }

        /// <summary> CSS ID for the viewer viewport for this particular viewer </summary>
        /// <value> This always returns the value 'sbkFliv_Viewer' </value>
        public override string ViewerBox_CssId
        {
            get { return "sbkFliv_Viewer"; }
        }

        /// <summary> Write the item viewer main section as HTML directly to the HTTP output stream </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Flash_ItemViewer.Add_Main_Viewer_Section", "");
            }

            // Add the HTML for the image
            Output.WriteLine("        <!-- FLASH VIEWER OUTPUT -->");
            Output.WriteLine("          <td><div id=\"sbkFiv_ViewerTitle\">" + flash_label + "</div></td>");
            Output.WriteLine("        </tr>");
            Output.WriteLine("        <tr>");
            Output.WriteLine("          <td id=\"sbkFiv_MainArea\">");
            Output.WriteLine("            <object style=\"width:630px;height:630px;\">");
            Output.WriteLine("            <param name=\"allowScriptAccess\" value=\"always\" />");
            Output.WriteLine("            <param name=\"movie\" value=\"" + flash_file + "\">");
            Output.WriteLine("            <embed src=\"" + flash_file + "\" AllowScriptAccess=\"always\" style=\"width:630px;height:630px;\">");
            Output.WriteLine("            </embed>");
            Output.WriteLine("            </object>");
            Output.WriteLine("          </td>");
            Output.WriteLine("        <!-- END FLASH VIEWER OUTPUT -->");
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
