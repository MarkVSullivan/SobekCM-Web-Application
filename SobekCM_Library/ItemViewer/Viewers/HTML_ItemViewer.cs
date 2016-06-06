using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using SobekCM.Core.BriefItem;
using SobekCM.Core.FileSystems;
using SobekCM.Core.Navigation;
using SobekCM.Core.Users;
using SobekCM.Library.ItemViewer.Menu;
using SobekCM.Tools;

namespace SobekCM.Library.ItemViewer.Viewers
{
    /// <summary> HTML item viewer prototyper, which is used to check to see if there is a HTML file to display, 
    /// to create the link in the main menu, and to create the viewer itself if the user selects that option </summary>
    public class HTML_ItemViewer_Prototyper : iItemViewerPrototyper
    {
        /// <summary> Constructor for a new instance of the HTML_ItemViewer_Prototyper class </summary>
        public HTML_ItemViewer_Prototyper()
        {
            ViewerType = "HTML";
            ViewerCode = "html";
            FileExtensions = new string[] { "HTML", "HTM" };
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
            return FileExtensions.Any(Extension => CurrentItem.Web.Contains_File_Extension(Extension));
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
            // Start with an empty label
            string first_label = String.Empty;

            // First, look at the viewer information from the database
            BriefItem_BehaviorViewer thisViewer = CurrentItem.Behaviors.Get_Viewer("HTML");
            if (!String.IsNullOrWhiteSpace(thisViewer.Label))
                first_label = thisViewer.Label;

            // Next, look for a page name in the METS
            if (String.IsNullOrEmpty(first_label))
            {
                foreach (BriefItem_FileGrouping thisPage in CurrentItem.Downloads)
                {
                    // Look for a HTML file on each page
                    foreach (BriefItem_File thisFile in thisPage.Files)
                    {
                        if (thisFile.File_Extension.IndexOf(".HTM", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            if (!String.IsNullOrWhiteSpace(thisPage.Label))
                                first_label = thisPage.Label;
                            break;
                        }
                    }
                }
            }

            // Finally, just default to HTML otherwise
            if (String.IsNullOrEmpty(first_label))
                first_label = "HTML";

            // Get the URL for this
            string previous_code = CurrentRequest.ViewerCode;
            CurrentRequest.ViewerCode = ViewerCode;
            string url = UrlWriterHelper.Redirect_URL(CurrentRequest);
            CurrentRequest.ViewerCode = previous_code;

            // Add the item menu information
            Item_MenuItem menuItem = new Item_MenuItem(first_label, null, null, url, ViewerCode);
            MenuItems.Add(menuItem);
        }

        /// <summary> Creates and returns the an instance of the <see cref="HTML_ItemViewer"/> class for showing an
        /// HTML source file that is loaded locally with the digital resource during execution of a single HTTP request. </summary>
        /// <param name="CurrentItem"> Digital resource object </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Fully built and initialized <see cref="HTML_ItemViewer"/> object </returns>
        /// <remarks> This method is called whenever a request requires the actual viewer to be created to render the HTML for
        /// the digital resource requested.  The created viewer is then destroyed at the end of the request </remarks>
        public iItemViewer Create_Viewer(BriefItemInfo CurrentItem, User_Object CurrentUser, Navigation_Object CurrentRequest, Custom_Tracer Tracer)
        {
            return new HTML_ItemViewer(CurrentItem, CurrentUser, CurrentRequest);
        }
    }


    /// <summary> Item viewer displays the a HTML source file related to this digital resource embedded into the SobekCM window for viewing. </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractNoPaginationItemViewer"/> and implements the 
    /// <see cref="iItemViewer" /> interface. </remarks>
    public class HTML_ItemViewer : abstractNoPaginationItemViewer
    {
        private readonly string htmlFile;

        /// <summary> Constructor for a new instance of the HTML_ItemViewer class, used to display a HTML file from a digital resource </summary>
        /// <param name="BriefItem"> Digital resource object </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        public HTML_ItemViewer(BriefItemInfo BriefItem, User_Object CurrentUser, Navigation_Object CurrentRequest)
        {
            // Save the arguments for use later
            this.BriefItem = BriefItem;
            this.CurrentUser = CurrentUser;
            this.CurrentRequest = CurrentRequest;

            // Set the behavior properties to the empy behaviors ( in the base class )
            Behaviors = EmptyBehaviors;

            // Find the first HTML file loaded
            foreach (BriefItem_FileGrouping thisPage in BriefItem.Downloads)
            {
                // Look for a HTML file on each page
                foreach (BriefItem_File thisFile in thisPage.Files)
                {
                    if (thisFile.File_Extension.IndexOf(".HTM", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        htmlFile = thisFile.Name;
                        break;
                    }
                }
            }
        }

        /// <summary> Write the item viewer main section as HTML directly to the HTTP output stream </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("HTML_ItemViewer.Write_Main_Viewer_Section", "");
            }

            // Save the current viewer code
            string current_view_code = CurrentRequest.ViewerCode;

            // Start the citation table
            Output.WriteLine("\t\t<!-- HTML VIEWER OUTPUT -->");
            Output.WriteLine("\t\t<td style=\"align:left;\">");

            // Determine some replacement strings here
            string itemURL = SobekFileSystem.Resource_Web_Uri(BriefItem);
            string itemLink = CurrentRequest.Base_URL + "/" + BriefItem.BibID + "/" + BriefItem.VID;

            // Determine the source string
            string sourceString = SobekFileSystem.Resource_Web_Uri(BriefItem) + htmlFile;
            if ((htmlFile.IndexOf("http://") == 0) || (htmlFile.IndexOf("https://") == 0) || (htmlFile.IndexOf("[%BASEURL%]") == 0) || (htmlFile.IndexOf("<%BASEURL%>") == 0))
            {
                sourceString = htmlFile.Replace("[%BASEURL%]", CurrentRequest.Base_URL).Replace("<%BASEURL%>", CurrentRequest.Base_URL);
            }

            // Try to get the HTML for this
            if (Tracer != null)
            {
                Tracer.Add_Trace("HTML_ItemViewer.Write_Main_Viewer_Section", "Reading html for this view from static page");
            }
            string map;
            try
            {
                map = SobekFileSystem.ReadToEnd(BriefItem, sourceString);
            }
            catch
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("<div style=\"background-color: White; color: black;text-align:center; width:630px;\">");
                builder.AppendLine("  <br /><br />");
                builder.AppendLine("  <span style=\"font-weight:bold;font-size:1.4em\">Unable to pull html view for item ( <a href=\"" + sourceString + "\">source</a> )</span><br /><br />");
                builder.AppendLine("  <span style=\"font-size:1.2em\">We apologize for the inconvenience.</span><br /><br />");

                string returnurl = CurrentRequest.Base_URL + "/contact";
                builder.AppendLine("  <span style=\"font-size:1.2em\">Click <a href=\"" + returnurl + "\">here</a> to report the problem.</span>");
                builder.AppendLine("  <br /><br />");
                builder.AppendLine("</div>");
                map = builder.ToString();
            }

            // Write the HTML 
            string url_options = UrlWriterHelper.URL_Options(CurrentRequest);
            string urlOptions1 = String.Empty;
            string urlOptions2 = String.Empty;
            if (url_options.Length > 0)
            {
                urlOptions1 = "?" + url_options;
                urlOptions2 = "&" + url_options;
            }
            Output.WriteLine(map.Replace("<%URLOPTS%>", url_options).Replace("<%?URLOPTS%>", urlOptions1).Replace("<%&URLOPTS%>", urlOptions2).Replace("<%ITEMURL%>", itemURL).Replace("<%ITEM_LINK%>", itemLink));

            // Finish the table
            Output.WriteLine("\t\t</td>");
            Output.WriteLine("\t\t<!-- END HTML VIEWER OUTPUT -->");

            // Restore the mode
            CurrentRequest.ViewerCode = current_view_code;
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
