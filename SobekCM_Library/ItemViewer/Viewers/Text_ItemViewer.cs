using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// <summary> Text item viewer prototyper, which is used to check if an item has page text to display, 
    /// and to create the viewer itself if the user selects that option </summary>
    public class Text_ItemViewer_Prototyper : iItemViewerPrototyper
    {
        /// <summary> Constructor for a new instance of the Text_ItemViewer_Prototyper class </summary>
        public Text_ItemViewer_Prototyper()
        {
            ViewerType = "TEXT";
            ViewerCode = "#t";
            FileExtensions = new string[] { "TXT" };
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
            CurrentRequest.ViewerCode = ViewerCode.Replace("#", "1");
            string url = UrlWriterHelper.Redirect_URL(CurrentRequest);
            CurrentRequest.ViewerCode = previous_code;

            // Add the item menu information
            Item_MenuItem menuItem = new Item_MenuItem("Page Images", "Text", null, url, ViewerCode);
            MenuItems.Add(menuItem);
        }

        /// <summary> Creates and returns the an instance of the <see cref="Text_ItemViewer"/> class for showing the
        /// full text of individual pages for a digital resource during execution of a single HTTP request. </summary>
        /// <param name="CurrentItem"> Digital resource object </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Fully built and initialized <see cref="Text_ItemViewer"/> object </returns>
        /// <remarks> This method is called whenever a request requires the actual viewer to be created to render the HTML for
        /// the digital resource requested.  The created viewer is then destroyed at the end of the request </remarks>
        public iItemViewer Create_Viewer(BriefItemInfo CurrentItem, User_Object CurrentUser, Navigation_Object CurrentRequest, Custom_Tracer Tracer)
        {
            return new Text_ItemViewer(CurrentItem, CurrentUser, CurrentRequest, Tracer, ViewerCode, FileExtensions);
        }
    }

    /// <summary> Text item viewer displays the text associated with the pages for a digital resource </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractPageFilesItemViewer"/> and implements the 
    /// <see cref="iItemViewer" /> interface. </remarks>
    public class Text_ItemViewer: abstractPageFilesItemViewer
    {
        // information about the page to display
        private readonly int page;
        private string filename;

        private string text_from_file;
        private bool file_does_not_exist;
        private bool error_occurred;
        private int width;

        /// <summary> Constructor for a new instance of the Text_ItemViewer class, used to display the text 
        /// associated with the pages for a digital resource </summary>
        /// <param name="BriefItem"> Digital resource object </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <param name="Text_ViewerCode"> Text viewer code, as determined by configuration files </param>
        /// <param name="FileExtensions"> File extensions that this viewer allows, as determined by configuration files </param>
        public Text_ItemViewer(BriefItemInfo BriefItem, User_Object CurrentUser, Navigation_Object CurrentRequest, Custom_Tracer Tracer, string Text_ViewerCode, string[] FileExtensions)
        {
            // Add the trace
            if ( Tracer != null )
                Tracer.Add_Trace("Text_ItemViewer.Constructor");

            // Save the arguments for use later
            this.BriefItem = BriefItem;
            this.CurrentUser = CurrentUser;
            this.CurrentRequest = CurrentRequest;

            // Set the behavior properties
            Behaviors = EmptyBehaviors;

            // Determine the page
            page = 1;
            if (!String.IsNullOrEmpty(CurrentRequest.ViewerCode))
            {
                int tempPageParse;
                if (Int32.TryParse(CurrentRequest.ViewerCode.Replace(Text_ViewerCode.Replace("#", ""), ""), out tempPageParse))
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
            if (String.IsNullOrEmpty(CurrentRequest.ViewerCode))
                CurrentRequest.ViewerCode = Text_ViewerCode.Replace("#", page.ToString());

            try
            {
                // Set some defaults
                text_from_file = String.Empty;
                file_does_not_exist = false;
                error_occurred = false;
                width = -1;

                if (filename.Length > 0)
                {
                    text_from_file = SobekFileSystem.ReadToEnd(BriefItem, filename);

                    // Did this work?
                    if (text_from_file.Length > 0)
                    {
                        string[] splitter = text_from_file.Split("\n".ToCharArray());
                        foreach (string thisString in splitter)
                        {
                            width = Math.Max(width, thisString.Length * 9);
                        }
                        // width = Math.Min(width, 800);
                    }
                }
                else
                {
                    file_does_not_exist = true;
                }
            }
            catch (Exception ee)
            {
                text_from_file = "EXCEPTION CAUGHT: " + ee.Message;
            }
        }


        /// <summary> Any additional inline style for this viewer that affects the main box around this</summary>
        /// <remarks> This returns the width of the image for the width of the viewer port </remarks>
        public override string ViewerBox_InlineStyle
        {
            get
            {
                if (width < 600)
                    return "width:600px;";
                return "width:" + width + "px;";
            }
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
                            // Get the TEXT information
                            filename = thisFile.Name;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary> Write the item viewer main section as HTML directly to the HTTP output stream </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Text_ItemViewer.Write_Main_Viewer_Section", "");
            }

            if ((error_occurred) || (file_does_not_exist) || (text_from_file.Trim().Length == 0))
            {
                Output.WriteLine("\t\t<td id=\"sbkTiv_ErrorArea\">");
                if (error_occurred)
                {
                    Output.WriteLine("Unknown error while retrieving text");
                }
                else if (file_does_not_exist)
                {
                    Output.WriteLine("No text file exists for this page");
                }
                else
                {
                    Output.WriteLine("No text is recorded for this page");
                }
            }
            else
            {
                Output.WriteLine("\t\t<td id=\"sbkTiv_MainArea\">");

                // If there was a term search here, highlight it
                if (!String.IsNullOrWhiteSpace(CurrentRequest.Text_Search))
                {
                    // Get any search terms
                    List<string> terms = new List<string>();
                    string[] splitter = CurrentRequest.Text_Search.Replace("\"", "").Split(" ".ToCharArray());
                    terms.AddRange(from thisSplit in splitter where thisSplit.Trim().Length > 0 select thisSplit.Trim());

                    Output.WriteLine(Text_Search_Term_Highlighter.Hightlight_Term_In_HTML(text_from_file, terms, "<span class=\"sbkTiv_TextHighlight\">", "</span>"));
                }
                else
                {
                    Output.Write(text_from_file);
                }
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
