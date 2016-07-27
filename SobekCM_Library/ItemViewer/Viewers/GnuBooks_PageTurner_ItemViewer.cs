using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using SobekCM.Core.BriefItem;
using SobekCM.Core.Navigation;
using SobekCM.Core.UI_Configuration;
using SobekCM.Core.UI_Configuration.StaticResources;
using SobekCM.Core.Users;
using SobekCM.Engine_Library.Configuration;
using SobekCM.Library.HTML;
using SobekCM.Library.ItemViewer.Menu;
using SobekCM.Library.UI;
using SobekCM.Tools;

namespace SobekCM.Library.ItemViewer.Viewers
{
    /// <summary> Pageturner item viewer prototyper, which is used to check to see if this item can be viewed in the GnuBooks pageturner
    /// library, and to create the viewer itself if the user selects that option </summary>
    public class GnuBooks_PageTurner_ItemViewer_Prototyper : iItemViewerPrototyper
    {
        /// <summary> Constructor for a new instance of the GnuBooks_PageTurner_ItemViewer_Prototyper class </summary>
        public GnuBooks_PageTurner_ItemViewer_Prototyper()
        {
            ViewerType = "PAGE_TURNER";
            ViewerCode = "pageturner";
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
            // Must be at least two pages, with JPEGS, included
            if ((CurrentItem.Images == null) || (CurrentItem.Images.Count < 2))
                return false;

            // Do the pages have JPEGs?
            return CurrentItem.Web.Contains_File_Extension("JPG");
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
            return !IpRestricted;
        }

        /// <summary> Gets the menu items related to this viewer that should be included on the main item (digital resource) menu </summary>
        /// <param name="CurrentItem"> Digital resource object, which can be used to ensure if and how this viewer should appear 
        /// in the main item (digital resource) menu </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        /// <param name="MenuItems"> List of menu items, to which this method may add one or more menu items </param>
        /// <param name="IpRestricted"> Flag indicates if this item is IP restricted AND if the current user is outside the ranges </param>
        public void Add_Menu_items(BriefItemInfo CurrentItem, User_Object CurrentUser, Navigation_Object CurrentRequest, List<Item_MenuItem> MenuItems, bool IpRestricted )
        {
           
            // Get the URL for this
            string previous_code = CurrentRequest.ViewerCode;
            CurrentRequest.ViewerCode = ViewerCode;
            string url = UrlWriterHelper.Redirect_URL(CurrentRequest);
            CurrentRequest.ViewerCode = previous_code;

            // Start with the default label on the menu
            string label = "Page Turner";

            // Allow the label to be implemented for this viewer
            BriefItem_BehaviorViewer thisViewerInfo = CurrentItem.Behaviors.Get_Viewer(ViewerCode);

            // If this is found, and has a custom label, use that 
            if ((thisViewerInfo != null) && (!String.IsNullOrWhiteSpace(thisViewerInfo.Label)))
                label = thisViewerInfo.Label;

            // Add the item menu information
            Item_MenuItem menuItem = new Item_MenuItem(label, null, null, url, ViewerCode);
            MenuItems.Add(menuItem);
        }

        /// <summary> Creates and returns the an instance of the <see cref="GnuBooks_PageTurner_ItemViewer"/> class for showing the
        /// digital resource as an online flip book within a Gnu library page turner interface </summary>
        /// <param name="CurrentItem"> Digital resource object </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Fully built and initialized <see cref="GnuBooks_PageTurner_ItemViewer"/> object </returns>
        /// <remarks> This method is called whenever a request requires the actual viewer to be created to render the HTML for
        /// the digital resource requested.  The created viewer is then destroyed at the end of the request </remarks>
        public iItemViewer Create_Viewer(BriefItemInfo CurrentItem, User_Object CurrentUser, Navigation_Object CurrentRequest, Custom_Tracer Tracer)
        {
            return new GnuBooks_PageTurner_ItemViewer(CurrentItem, CurrentUser, CurrentRequest);
        }
    }

    /// <summary> Pageturner item viewer shows the jpegs images attached to an item in the GnuBooks viewer </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractNoPaginationItemViewer"/> and implements the 
    /// <see cref="iItemViewer" /> interface. </remarks>
    public class GnuBooks_PageTurner_ItemViewer : abstractNoPaginationItemViewer
    {
        /// <summary> Constructor for a new instance of the GnuBooks_PageTurner_ItemViewer class, used to display the 
        /// jpegs images attached to an item in the GnuBooks viewer </summary>
        /// <param name="BriefItem"> Digital resource object </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        public GnuBooks_PageTurner_ItemViewer(BriefItemInfo BriefItem, User_Object CurrentUser, Navigation_Object CurrentRequest)
        {
            // Save the arguments for use later
            this.BriefItem = BriefItem;
            this.CurrentUser = CurrentUser;
            this.CurrentRequest = CurrentRequest;

            // Set the behavior properties to the empy behaviors ( in the base class )
            Behaviors = EmptyBehaviors;
        }

        /// <summary> CSS ID for the viewer viewport for this particular viewer </summary>
        /// <value> This always returns the value 'sbkGbiv_Viewer' </value>
        public override string ViewerBox_CssId
        {
            get { return "sbkGbiv_Viewer"; }
        }

        /// <summary> Write the item viewer main section as HTML directly to the HTTP output stream </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("GnuBooks_PageTurner_ItemViewer.Write_Main_Viewer_Section", "");
            }

            // Add the division
            Output.WriteLine("          <div id=\"GnuBook\"><p style=\"font-size: 14px;\">Book Turner presentations require a Javascript-enabled browser.</p></div>" + Environment.NewLine);


            // Add the javascript
            Output.WriteLine("<script type=\"text/javascript\"> ");
            Output.WriteLine("  //<![CDATA[");


            // Get the list of jpegs, along with widths and heights
            List<int> width = new List<int>();
            List<int> height = new List<int>();
            List<string> files = new List<string>();
            List<string> pagename = new List<string>();
            foreach (BriefItem_FileGrouping thisPage in BriefItem.Images)
            {
                // Step through each page looking for the jpeg
                foreach (BriefItem_File thisFile in thisPage.Files)
                {
                    if ((thisFile.MIME_Type == "image/jpeg") && (thisFile.Name.ToUpper().IndexOf("THM.JPG") < 0))
                    {
                        if (!files.Contains(thisFile.Name))
                        {
                            pagename.Add(thisPage.Label);
                            files.Add(thisFile.Name);
                            width.Add(thisFile.Width.HasValue ? thisFile.Width.Value : 800);
                            height.Add(thisFile.Height.HasValue ? thisFile.Height.Value : 1000);
                        }


                        break;
                    }
                }
            }


            // Add the script for this resource
            Output.WriteLine("    // Create the GnuBook object");
            Output.WriteLine("    gb = new GnuBook();");
            Output.WriteLine();
            Output.WriteLine("    // Return the width of a given page");
            Output.WriteLine("    gb.getPageWidth = function(index) {");
            if (width[0] > 0)
            {
                Output.WriteLine("        if (index <= 2) return " + width[0] + ";");
            }
            for (int i = 1; i < width.Count; i++)
            {
                if (width[i] > 0)
                {
                    Output.WriteLine("        if (index == " + (i + 2) + ") return " + width[i] + ";");
                }
            }
            Output.WriteLine("        return 638;");
            Output.WriteLine("    }");
            Output.WriteLine();
            Output.WriteLine("    // Return the height of a given page");
            Output.WriteLine("    gb.getPageHeight = function(index) {");
            if (height[0] > 0)
            {
                Output.WriteLine("        if (index <= 2) return " + height[0] + ";");
            }
            for (int i = 1; i < height.Count; i++)
            {
                if (height[i] > 0)
                {
                    Output.WriteLine("        if (index == " + (i + 2) + ") return " + height[i] + ";");
                }
            }
            Output.WriteLine("        return 825;");
            Output.WriteLine("    }");
            Output.WriteLine();
            Output.WriteLine("    // Return the URI for a page, by index");
            Output.WriteLine("    gb.getPageURI = function(index) {");
            Output.WriteLine("        var imgStr = (index).toString();");
            Output.WriteLine("        if (index < 2) return '" + CurrentRequest.Base_URL + "default/images/bookturner/emptypage.jpg';");
            for (int i = 0; i < files.Count; i++)
            {
                Output.WriteLine("        if (index == " + (i + 2) + ") imgStr = '" + files[i] + "';");
            }
            Output.WriteLine("        if (index > " + (files.Count + 1) + ") return '" + CurrentRequest.Base_URL + "default/images/bookturner/emptypage.jpg';");
            string source_url = BriefItem.Web.Source_URL.Replace("\\", "/");
            if (source_url[source_url.Length - 1] != '/')
                source_url = source_url + "/";
            Output.WriteLine("        return '" + source_url + "' + imgStr;");
            Output.WriteLine("    }");
            Output.WriteLine();


            Output.WriteLine("    // Return the page label for a page, by index");
            Output.WriteLine("    gb.getPageName = function(index) {");
            Output.WriteLine("        var imgStr = '" + UI_ApplicationCache_Gateway.Translation.Get_Translation("Page", CurrentRequest.Language) + "' + this.getPageNum(index);");
            for (int i = 0; i < files.Count; i++)
            {
                Output.WriteLine("        if (index == " + (i + 2) + ") imgStr = '" + pagename[i].Replace("&", "&amp;").Replace("'", "&apos;").Replace("\"", "&quot;") + "';");

            }


            Output.WriteLine("        return imgStr;");
            Output.WriteLine("    }");
            Output.WriteLine();


            Output.WriteLine("    // Return which side, left or right, that a given page should be displayed on");
            Output.WriteLine("    gb.getPageSide = function(index) {");
            Output.WriteLine("        if (0 == (index & 0x1)) {");
            Output.WriteLine("            return 'R';");
            Output.WriteLine("        } else {");
            Output.WriteLine("            return 'L';");
            Output.WriteLine("        }");
            Output.WriteLine("    }");
            Output.WriteLine();
            Output.WriteLine("    // This function returns the left and right indices for the user-visible");
            Output.WriteLine("    // spread that contains the given index.  The return values may be");
            Output.WriteLine("    // null if there is no facing page or the index is invalid.");
            Output.WriteLine("    gb.getSpreadIndices = function(pindex) {   ");
            Output.WriteLine("        var spreadIndices = [null, null]; ");
            Output.WriteLine("        if ('rl' == this.pageProgression) {");
            Output.WriteLine("            // Right to Left");
            Output.WriteLine("            if (this.getPageSide(pindex) == 'R') {");
            Output.WriteLine("                spreadIndices[1] = pindex;");
            Output.WriteLine("                spreadIndices[0] = pindex + 1;");
            Output.WriteLine("            } else {");
            Output.WriteLine("            // Given index was LHS");
            Output.WriteLine("                spreadIndices[0] = pindex;");
            Output.WriteLine("                spreadIndices[1] = pindex - 1;");
            Output.WriteLine("            }");
            Output.WriteLine("        } else {");
            Output.WriteLine("            // Left to right");
            Output.WriteLine("            if (this.getPageSide(pindex) == 'L') {");
            Output.WriteLine("                spreadIndices[0] = pindex;");
            Output.WriteLine("                spreadIndices[1] = pindex + 1;");
            Output.WriteLine("            } else {");
            Output.WriteLine("                // Given index was RHS");
            Output.WriteLine("                spreadIndices[1] = pindex;");
            Output.WriteLine("                spreadIndices[0] = pindex - 1;");
            Output.WriteLine("            }");
            Output.WriteLine("        }");
            Output.WriteLine();
            Output.WriteLine("        return spreadIndices;");
            Output.WriteLine("    }");
            Output.WriteLine();
            Output.WriteLine("    // For a given \"accessible page index\" return the page number in the book.");
            Output.WriteLine("    // For example, index 5 might correspond to \"Page 1\" if there is front matter such");
            Output.WriteLine("    // as a title page and table of contents.");
            Output.WriteLine("    gb.getPageNum = function(index) {");
            Output.WriteLine("        return index;");
            Output.WriteLine("    }");
            Output.WriteLine();
            Output.WriteLine("    // Total number of leafs");


            // TRUE FOR EVEN PAGE BOOKS at least
            Output.WriteLine("    gb.numLeafs = " + (files.Count + 4) + ";");
            Output.WriteLine();
            Output.WriteLine("    // Book title and the URL used for the book title link");
            Output.WriteLine("    gb.bookTitle= '" + BriefItem.Title.Replace("'", "") + "';");
            Output.WriteLine("    gb.bookUrl = '" + CurrentRequest.Base_URL + BriefItem.BibID + "/" + BriefItem.VID + "';");
            Output.WriteLine();
            Output.WriteLine("    // Let's go!");
            Output.WriteLine("    gb.init();");


            Output.Write("  //]]>");
            Output.Write("</script>");
        }


        /// <summary> Write any additional values within the HTML Head of the final served page </summary>
        /// <param name="Output"> Output stream currently within the HTML head tags </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <remarks> By default this does nothing, but can be overwritten by all the individual item viewers </remarks>
        public override void Write_Within_HTML_Head(TextWriter Output, Custom_Tracer Tracer)
        {
            Output.WriteLine("  <link rel=\"stylesheet\" type=\"text/css\" href=\"" + Static_Resources_Gateway.Sobekcm_Bookturner_Css + "\" /> ");
            Output.WriteLine("  <script type=\"text/javascript\" src=\"" + Static_Resources_Gateway.Jquery_1_2_6_Min_Js + "\"></script> ");
            Output.WriteLine("  <script type=\"text/javascript\" src=\"" + Static_Resources_Gateway.Jquery_Easing_1_3_Js + "\"></script> ");
            Output.WriteLine("  <script type=\"text/javascript\" src=\"" + Static_Resources_Gateway.Bookturner_Js + "\"></script>    ");
        }


        /// <summary> Gets the collection of special behaviors which this item viewer
        /// requests from the main HTML subwriter. </summary>
        public override List<HtmlSubwriter_Behaviors_Enum> ItemViewer_Behaviors
        {
            get
            {
                return new List<HtmlSubwriter_Behaviors_Enum>
                    {
                        HtmlSubwriter_Behaviors_Enum.Item_Subwriter_NonWindowed_Mode,
                        HtmlSubwriter_Behaviors_Enum.Suppress_Footer,
                        HtmlSubwriter_Behaviors_Enum.Suppress_Internal_Header,
                        HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Suppress_Item_Menu,
                        HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Suppress_Left_Navigation_Bar,
						HtmlSubwriter_Behaviors_Enum.Suppress_Header,
                        HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Suppress_Titlebar
                    };
            }
        }


        /// <summary> Gets the collection of body attributes to be included 
        /// within the HTML body tag (usually to add events to the body) </summary>
        /// <param name="Body_Attributes"> List of body attributes to be included </param>
        public override void Add_ViewerSpecific_Body_Attributes(List<Tuple<string, string>> Body_Attributes)
        {
            Body_Attributes.Clear();
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
