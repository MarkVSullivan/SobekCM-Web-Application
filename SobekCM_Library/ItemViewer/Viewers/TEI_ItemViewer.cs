using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using SobekCM.Core.BriefItem;
using SobekCM.Core.FileSystems;
using SobekCM.Core.Navigation;
using SobekCM.Core.Users;
using SobekCM.Core.XSLT;
using SobekCM.Library.ItemViewer.Menu;
using SobekCM.Library.UI;
using SobekCM.Tools;

namespace SobekCM.Library.ItemViewer.Viewers
{
    /// <summary> TEI viewer prototyper, which is used to check to see if the TEI viewer should really be shown, 
    /// to create the link in the main menu, and to create the viewer itself if the user selects that option </summary>
    public class TEI_ItemViewer_Prototyper : iItemViewerPrototyper
    {
        /// <summary> Constructor for a new instance of the TEI_ItemViewer_Prototyper class </summary>
        public TEI_ItemViewer_Prototyper()
        {
            ViewerType = "TEI";
            ViewerCode = "tei";
            FileExtensions = new string[] { "XML" };
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
            // Look for the source TEI in the item settings
            if (( CurrentItem.Behaviors.Settings == null ) || ( CurrentItem.Behaviors.Settings.Count == 0 ))
            {
                return false;
            }

            // Try to get the TEI file settings
            string tei_file = CurrentItem.Behaviors.Get_Setting("TEI.Source_File");
            string xslt_file = CurrentItem.Behaviors.Get_Setting("TEI.XSLT");
            if ((tei_file != null) && (xslt_file != null ))
            {
                // Ensure the TEI file really exists
                if (!SobekFileSystem.FileExists(CurrentItem, tei_file))
                    return false;

                // Ensure the XSLT file really exists
                if (!File.Exists(xslt_file))
                {
                    // This may just not have the path on it
                    if ((xslt_file.IndexOf("/") < 0) && (xslt_file.IndexOf("\\") < 0))
                    {
                        xslt_file = Path.Combine( UI_ApplicationCache_Gateway.Settings.Servers.Application_Server_Network, "plugins\\tei\\xslt", xslt_file);
                        if (!File.Exists(xslt_file)) return false;
                    }
                    else return false;
                }

                // Everything exists
                return true;
            }

            // Since no TEI source file was found, skip this
            return false;
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
        /// <param name="IpRestricted"> Flag indicates if this item is IP restricted AND if the current user is outside the ranges </param>
        public void Add_Menu_items(BriefItemInfo CurrentItem, User_Object CurrentUser, Navigation_Object CurrentRequest, List<Item_MenuItem> MenuItems, bool IpRestricted)
        {
            // Try to get the TEI file name
            string tei_file = CurrentItem.Behaviors.Get_Setting("TEI.Source_File");
            if (tei_file == null)
            {
                // Ensure the TEI file really exists
                return;
            }

            // Ensure the TEI file really exists
            if (!SobekFileSystem.FileExists(CurrentItem, tei_file))
                return;

            // Look for the label in the METS structure map
            string first_label = "TEI";
            if (CurrentItem.Downloads != null)
            {
                foreach (BriefItem_FileGrouping thisPage in CurrentItem.Downloads)
                {
                    // Look for a flash file on each page
                    foreach (BriefItem_File thisFile in thisPage.Files)
                    {
                        if (String.Compare(thisFile.Name, tei_file, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            first_label = thisPage.Label.Replace(".xml", "").Replace(".XML", "");
                            break;
                        }
                    }
                }
            }

            // Allow the label to be implemented for this viewer from the database as well
            BriefItem_BehaviorViewer thisViewerInfo = CurrentItem.Behaviors.Get_Viewer(ViewerCode);

            // If this is found, and has a custom label, use that 
            if ((thisViewerInfo != null) && (!String.IsNullOrWhiteSpace(thisViewerInfo.Label)))
                first_label = thisViewerInfo.Label;

            // Get the URL for this
            string previous_code = CurrentRequest.ViewerCode;
            CurrentRequest.ViewerCode = ViewerCode;
            string url = UrlWriterHelper.Redirect_URL(CurrentRequest);
            CurrentRequest.ViewerCode = previous_code;

            // Add the item menu information
            Item_MenuItem menuItem = new Item_MenuItem(first_label, null, null, url, ViewerCode);
            MenuItems.Add(menuItem);
        }

        /// <summary> Creates and returns the an instance of the <see cref="TEI_ItemViewer"/> class for showing a TEI file with a XSLT
        /// from a digital resource during execution of a single HTTP request. </summary>
        /// <param name="CurrentItem"> Digital resource object </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Fully built and initialized <see cref="TEI_ItemViewer"/> object </returns>
        /// <remarks> This method is called whenever a request requires the actual viewer to be created to render the HTML for
        /// the digital resource requested.  The created viewer is then destroyed at the end of the request </remarks>
        public iItemViewer Create_Viewer(BriefItemInfo CurrentItem, User_Object CurrentUser, Navigation_Object CurrentRequest, Custom_Tracer Tracer)
        {
            return new TEI_ItemViewer(CurrentItem, CurrentUser, CurrentRequest);
        }
    }

    /// <summary> Item viewer displays a TEI file through a XSLT transform </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractNoPaginationItemViewer"/> and implements the 
    /// <see cref="iItemViewer" /> interface. </remarks>
    public class TEI_ItemViewer : abstractNoPaginationItemViewer
    {
        private readonly string tei_file;
        private readonly string xslt_file;
        private string css_file;
        private readonly string head_info;
        private readonly string tei_string_to_display;

        /// <summary> Constructor for a new instance of the TEI_ItemViewer class, used to display a
        /// TEI file for a given digital resource </summary>
        /// <param name="BriefItem"> Digital resource object </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        public TEI_ItemViewer(BriefItemInfo BriefItem, User_Object CurrentUser, Navigation_Object CurrentRequest)
        {
            // Save the arguments for use later
            this.BriefItem = BriefItem;
            this.CurrentUser = CurrentUser;
            this.CurrentRequest = CurrentRequest;

            // Set the behavior properties to the empy behaviors ( in the base class )
            Behaviors = EmptyBehaviors;

            // Get the TEI files to use
            tei_file = BriefItem.Behaviors.Get_Setting("TEI.Source_File");
            xslt_file = BriefItem.Behaviors.Get_Setting("TEI.XSLT");
            css_file = BriefItem.Behaviors.Get_Setting("TEI.CSS");

            // Ensure the XSLT file really exists
            if (!File.Exists(xslt_file))
            {
                // This may just not have the path on it
                if ((xslt_file.IndexOf("/") < 0) && (xslt_file.IndexOf("\\") < 0))
                {
                    xslt_file = Path.Combine(UI_ApplicationCache_Gateway.Settings.Servers.Application_Server_Network, "plugins\\tei\\xslt", xslt_file);
                }
            }

            string tei_file_network = SobekFileSystem.Resource_Network_Uri(BriefItem, tei_file);

            XSLT_Transformer_ReturnArgs returnArgs = XSLT_Transformer.Transform(tei_file_network, xslt_file);
            tei_string_to_display = String.Empty;
            if (returnArgs.Successful)
            {
                tei_string_to_display = returnArgs.TransformedString;

                // FInd the head information
                int head_start = tei_string_to_display.IndexOf("<head", StringComparison.OrdinalIgnoreCase);
                int head_end = tei_string_to_display.IndexOf("</head>", StringComparison.OrdinalIgnoreCase);
                if ((head_start > 0) && (head_end > 0))
                {
                    head_info = tei_string_to_display.Substring(head_start, head_end - head_start);
                    int end_bracket = head_info.IndexOf(">");
                    head_info = head_info.Substring(end_bracket + 1);
                }


                // Trim down to the body
                int body_start = tei_string_to_display.IndexOf("<body", StringComparison.OrdinalIgnoreCase);
                int body_end = tei_string_to_display.IndexOf("</body>", StringComparison.OrdinalIgnoreCase);

                if ((body_start > 0) && (body_end > 0))
                {
                    tei_string_to_display = tei_string_to_display.Substring(body_start, body_end - body_start);
                    int end_bracket = tei_string_to_display.IndexOf(">");
                    tei_string_to_display = tei_string_to_display.Substring(end_bracket + 1);
                }

            }
            else
            {
                tei_string_to_display = "Error during XSLT transform of TEI<br /><br />" + returnArgs.ErrorMessage;
            }
        }

        /// <summary> CSS ID for the viewer viewport for this particular viewer </summary>
        /// <value> This always returns the value 'sbkFliv_Viewer' </value>
        public override string ViewerBox_CssId
        {
            get { return "sbkTeiiv_Viewer"; }
        }

        /// <summary> Write the item viewer main section as HTML directly to the HTTP output stream </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("TEI_ItemViewer.Add_Main_Viewer_Section", "");
            }



            // Build the value
            Output.WriteLine("          <td>");
            Output.WriteLine("            <div id=\"sbkEad_MainArea\">");

            if (!String.IsNullOrWhiteSpace( CurrentRequest.Text_Search))
            {
                // Get any search terms
                List<string> terms = new List<string>();
                if (CurrentRequest.Text_Search.Trim().Length > 0)
                {
                    string[] splitter = CurrentRequest.Text_Search.Replace("\"", "").Split(" ".ToCharArray());
                    terms.AddRange(from thisSplit in splitter where thisSplit.Trim().Length > 0 select thisSplit.Trim());
                }

                Output.Write(Text_Search_Term_Highlighter.Hightlight_Term_In_HTML(tei_string_to_display, terms));
            }
            else
            {
                Output.Write(tei_string_to_display);
            }

            Output.WriteLine("            </div>");
            Output.WriteLine("          </td>");
        }

        /// <summary> Allows controls to be added directory to a place holder, rather than just writing to the output HTML stream </summary>
        /// <param name="MainPlaceHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the bulk of the item viewer's output is displayed</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <remarks> This method does nothing, since nothing is added to the place holder as a control for this item viewer </remarks>
        public override void Add_Main_Viewer_Section(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
        {
            // Do nothing
        }

        /// <summary> Write any additional values within the HTML Head of the final served page </summary>
        /// <param name="Output"> Output stream currently within the HTML head tags </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Within_HTML_Head(TextWriter Output, Custom_Tracer Tracer)
        {
            if (!String.IsNullOrEmpty(head_info))
            {
                Output.WriteLine(head_info);
            }

            if (!String.IsNullOrEmpty(css_file))
            {
                if ((css_file.IndexOf("http:") < 0) && (css_file.IndexOf("https:") < 0))
                    css_file = UI_ApplicationCache_Gateway.Settings.Servers.Application_Server_URL + "/plugins/tei/css/" + css_file + ".css";

                Output.WriteLine("  <link rel=\"stylesheet\" type=\"text/css\" href=\"" + css_file + "\" /> ");
            }


        }
    }
}
