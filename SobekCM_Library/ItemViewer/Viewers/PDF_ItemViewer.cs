using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using SobekCM.Core.BriefItem;
using SobekCM.Core.FileSystems;
using SobekCM.Core.Navigation;
using SobekCM.Core.UI_Configuration;
using SobekCM.Core.UI_Configuration.StaticResources;
using SobekCM.Core.Users;
using SobekCM.Engine_Library.Configuration;
using SobekCM.Library.HTML;
using SobekCM.Library.ItemViewer.Menu;
using SobekCM.Tools;

namespace SobekCM.Library.ItemViewer.Viewers
{
    /// <summary> PDF viewer prototyper, which is used to check to see if the PDF viewer should really be shown, 
    /// to create the link in the main menu, and to create the viewer itself if the user selects that option </summary>
    public class PDF_ItemViewer_Prototyper : iItemViewerPrototyper
    {
        /// <summary> Constructor for a new instance of the PDF_ItemViewer_Prototyper class </summary>
        public PDF_ItemViewer_Prototyper()
        {
            ViewerType = "PDF";
            ViewerCode = "pdf";
            FileExtensions = new string[] {"PDF"};
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

            return CurrentItem.Web.Contains_File_Extension("PDF");
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
            CurrentRequest.ViewerCode = ViewerCode;
            string url = UrlWriterHelper.Redirect_URL(CurrentRequest);
            CurrentRequest.ViewerCode = previous_code;

            // Add the item menu information
            Item_MenuItem menuItem = new Item_MenuItem("PDF", null, null, url, ViewerCode);
            MenuItems.Add(menuItem);
        }

        /// <summary> Creates and returns the an instance of the <see cref="PDF_ItemViewer"/> class for showing a PDF 
        /// from a digital resource during execution of a single HTTP request. </summary>
        /// <param name="CurrentItem"> Digital resource object </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Fully built and initialized <see cref="PDF_ItemViewer"/> object </returns>
        /// <remarks> This method is called whenever a request requires the actual viewer to be created to render the HTML for
        /// the digital resource requested.  The created viewer is then destroyed at the end of the request </remarks>
        public iItemViewer Create_Viewer(BriefItemInfo CurrentItem, User_Object CurrentUser, Navigation_Object CurrentRequest, Custom_Tracer Tracer)
        {
            return new PDF_ItemViewer(CurrentItem, CurrentUser, CurrentRequest, Tracer, FileExtensions);
        }
    }


    /// <summary> Item viewer displays the a PDF related to this digital resource embedded into the SobekCM window for viewing. </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractNoPaginationItemViewer"/> and implements the 
    /// <see cref="iItemViewer" /> interface. </remarks>
    public class PDF_ItemViewer : abstractNoPaginationItemViewer
    {
        private readonly bool writeAsIframe;
        private readonly int pdf;
        private readonly List<string> pdfFileNames;
        private readonly List<string> pdfLabels;

        /// <summary> Constructor for a new instance of the PDF_ItemViewer class, used to display a PDF file from a digital resource </summary>
        /// <param name="BriefItem"> Digital resource object </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <param name="FileExtensions"> List of file extensions this video viewer should show </param>
        public PDF_ItemViewer(BriefItemInfo BriefItem, User_Object CurrentUser, Navigation_Object CurrentRequest, Custom_Tracer Tracer, string[] FileExtensions)
        {
            // Save the arguments for use later
            this.BriefItem = BriefItem;
            this.CurrentUser = CurrentUser;
            this.CurrentRequest = CurrentRequest;

            // Determine if this should be written as an iFrame
            writeAsIframe = ((!String.IsNullOrEmpty(CurrentRequest.Browser_Type)) && (CurrentRequest.Browser_Type.IndexOf("CHROME") == 0));

            // Set the behavior properties
            Behaviors = new List<HtmlSubwriter_Behaviors_Enum> {HtmlSubwriter_Behaviors_Enum.Suppress_Footer};

            // Determine if a particular video was selected 
            pdf = 1;
            if (!String.IsNullOrEmpty(HttpContext.Current.Request.QueryString["pdf"]))
            {
                int tryPdf;
                if (Int32.TryParse(HttpContext.Current.Request.QueryString["pdf"], out tryPdf))
                {
                    if (tryPdf < 1)
                        tryPdf = 1;
                    pdf = tryPdf;
                }
            }

            // Collect the list of pdf by stepping through each download page
            pdfFileNames = new List<string>();
            pdfLabels = new List<string>();
            foreach (BriefItem_FileGrouping downloadPage in BriefItem.Downloads)
            {
                foreach (BriefItem_File thisFileInfo in downloadPage.Files)
                {
                    string extension = thisFileInfo.File_Extension.Replace(".", "");
                    foreach (string thisPossibleFileExtension in FileExtensions)
                    {
                        if (String.Compare(extension, thisPossibleFileExtension, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            pdfFileNames.Add(thisFileInfo.Name);
                            pdfLabels.Add(downloadPage.Label);
                        }
                    }
                }
            }

            // Ensure the pdf count wasn't too large
            if (pdf > pdfFileNames.Count)
                pdf = 1;
        }


        /// <summary> Write the item viewer main section as HTML directly to the HTTP output stream </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
			{
				Tracer.Add_Trace("PDF_ItemViewer.Write_Main_Viewer_Section", "");
			}

			// Save the current viewer code
            string current_view_code = CurrentRequest.ViewerCode;
            string pdf_url = SobekFileSystem.Resource_Web_Uri(BriefItem, pdfFileNames[pdf - 1]);

            // Find the PDF download
            string displayFileName = null;
            foreach (BriefItem_FileGrouping downloadGroup in BriefItem.Downloads)
            {
                foreach (BriefItem_File thisFile in downloadGroup.Files)
                {
                    if (String.Compare(thisFile.File_Extension, ".pdf", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        displayFileName = thisFile.Name;
                        break;
                    }
                }
            }

            // If the PDF was not there.. that is strange, so show a message
            if (String.IsNullOrEmpty(displayFileName))
            {
                Output.WriteLine("\t\t<!-- PDF ITEM VIEWER OUTPUT -->");
                Output.WriteLine("\t\t<td id=\"sbkPdf_MainArea\">ERROR: UNABLE TO FIND A PDF FILE IN THIS ITEM TO DISPLAY!</td>");
                Output.WriteLine("\t\t<!-- END PDF VIEWER OUTPUT -->");
                return;
            }

            // If the display name is NOT a web, than add the source URL onto it now
			if (displayFileName.IndexOf("http") < 0)
			{
                // MAKE THIS USE THE FILES.ASPX WEB PAGE if this is restricted (or dark)
                if ((BriefItem.Behaviors.Dark_Flag) || (BriefItem.Behaviors.IP_Restriction_Membership > 0))
                    displayFileName = CurrentRequest.Base_URL + "files/" + BriefItem.BibID + "/" + BriefItem.VID + "/" + displayFileName;
                else
                    displayFileName = SobekFileSystem.Resource_Web_Uri(BriefItem) + displayFileName;
			}

            // Ensure all the slashes are going the right way (had issues with this in the past)
			displayFileName = displayFileName.Replace("\\", "/").Replace("//", "/").Replace("http:/", "http://");

            //replace single & double quote with ascII characters
            if (displayFileName.Contains("'") || displayFileName.Contains("\""))
            {
                displayFileName = displayFileName.Replace("'", "&#39;");
                displayFileName = displayFileName.Replace("\"", "&#34;");
            }

			// Start the PDF display table
			Output.WriteLine("\t\t<!-- PDF ITEM VIEWER OUTPUT -->" );
            Output.WriteLine("\t\t<td id=\"sbkPdf_MainArea\">" );
            Output.WriteLine("<table style=\"width:95%;\"><tr>" );
            Output.WriteLine("<td style=\"text-align:left\"><a id=\"sbkPdf_DownloadFileLink\" href=\"" + displayFileName + "\">Download this PDF</a></td>");
            Output.WriteLine("<td style=\"text-align:right\"><a id=\"sbkPdf_DownloadAdobeReaderLink\" href=\"http://get.adobe.com/reader/\"><img src=\"" + Static_Resources_Gateway.Get_Adobe_Reader_Png + "\" alt=\"Download Adobe Reader\" /></a></td>");
            Output.WriteLine("</tr></table><br />");
           

            // Write as an iFrame, or as embed
            if (writeAsIframe)
            {
                Output.WriteLine("                  <iframe id=\"sbkPdf_Container\" src='" + displayFileName + "' href='" + displayFileName + "' style=\"width:100%;\"></iframe>");
            }
            else
            {
                if (!String.IsNullOrWhiteSpace(CurrentRequest.Text_Search))
                {
                    displayFileName = displayFileName + "#search=\"" + CurrentRequest.Text_Search.Replace("\"", "").Replace("+", " ").Replace("-", " ") + "\"";
                }

                Output.WriteLine("                  <embed id=\"sbkPdf_Container\" src='" + displayFileName + "' href='" + displayFileName + "' style=\"width:100%;\"></embed>");
            }

            // Finish the table
            Output.WriteLine("\t\t</td>");
            Output.WriteLine("\t\t<!-- END PDF VIEWER OUTPUT -->" );

			// Restore the mode
            CurrentRequest.ViewerCode = current_view_code;
		}

        /// <summary> Gets the collection of body attributes to be included 
        /// within the HTML body tag (usually to add events to the body) </summary>
        /// <param name="Body_Attributes"> List of body attributes to be included </param>
        public override void Add_ViewerSpecific_Body_Attributes(List<Tuple<string, string>> Body_Attributes)
        {
            Body_Attributes.Add(new Tuple<string, string>("onload", "pdf_set_fullscreen(" + writeAsIframe.ToString().ToLower() + ");"));
            Body_Attributes.Add(new Tuple<string, string>("onresize", "pdf_set_fullscreen(" + writeAsIframe.ToString().ToLower() + ");"));
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
