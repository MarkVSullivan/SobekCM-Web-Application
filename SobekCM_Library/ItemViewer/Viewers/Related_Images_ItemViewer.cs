using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using SobekCM.Core.BriefItem;
using SobekCM.Core.Configuration.Localization;
using SobekCM.Core.Navigation;
using SobekCM.Core.UI_Configuration;
using SobekCM.Core.Users;
using SobekCM.Library.HTML;
using SobekCM.Tools;

namespace SobekCM.Library.ItemViewer.Viewers
{
    /// <summary> Related images viewer prototyper, which is used to check to see if there are thumbnails to display, 
    /// to create the link in the main menu, and to create the viewer itself if the user selects that option </summary>
    public class Related_Images_ItemViewer_Prototyper : iItemViewerPrototyper
    {
        /// <summary> Constructor for a new instance of the Related_Images_ItemViewer_Prototyper class </summary>
        public Related_Images_ItemViewer_Prototyper()
        {
            ViewerType = "RELATED_IMAGES";
            ViewerCode = "thumbs";
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
            // Are there NO page files? 
            if ((CurrentItem.Images == null) || (CurrentItem.Images.Count == 0))
                return false;

            // (we are going to assume for now all pages include a thumbnail?)

            // Finally, return true
            return false;
        }

        /// <summary> Flag indicates if this viewer should be override on checkout </summary>
        /// <param name="CurrentItem"> Digital resource to examine to see if this viewer should really be overriden </param>
        /// <returns> FALSE always, since thumbnails can always be shown even if an item is checked out </returns>
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
        public void Add_Menu_items(BriefItemInfo CurrentItem, User_Object CurrentUser, Navigation_Object CurrentRequest, List<Item_MenuItem> MenuItems)
        {
            Item_MenuItem menuItem = new Item_MenuItem("Thumbnails", null, null, CurrentItem.Web.Source_URL + ViewerCode);
            MenuItems.Add(menuItem);
        }

        /// <summary> Creates and returns the an instance of the <see cref="Related_Images_ItemViewer"/> class for showing the 
        /// thumbnail images (i.e., related images) associated with a digital resource during execution of a single HTTP request. </summary>
        /// <param name="CurrentItem"> Digital resource object </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Fully built and initialized <see cref="Related_Images_ItemViewer"/> object </returns>
        /// <remarks> This method is called whenever a request requires the actual viewer to be created to render the HTML for
        /// the digital resource requested.  The created viewer is then destroyed at the end of the request </remarks>
        public iItemViewer Create_Viewer(BriefItemInfo CurrentItem, User_Object CurrentUser, Navigation_Object CurrentRequest, Custom_Tracer Tracer)
        {
            return new Related_Images_ItemViewer(CurrentItem, CurrentUser, CurrentRequest);
        }
    }

    /// <summary> Item viewer displays the thumbnail images (i.e., related images) associated with a digital resource </summary>
    /// <remarks> This class extends the implements the <see cref="iItemViewer" /> interface directly. </remarks>
    public class Related_Images_ItemViewer : iItemViewer
    {
        private readonly int pageCount;
        private readonly int thumbnailsPerPage;
        private readonly int thumbnailSize;

        private readonly BriefItemInfo briefItem;
        private readonly Navigation_Object currentRequest;

        // Empty list of behaviors, prevents an empty set from having to be created over and over
        private static readonly List<HtmlSubwriter_Behaviors_Enum> emptyBehaviors = new List<HtmlSubwriter_Behaviors_Enum>();

        /// <summary> Constructor for a new instance of the Related_Images_ItemViewer class, used to display 
        /// the thumbnail images (i.e., related images) associated with a digital resource </summary>
        /// <param name="BriefItem"> Digital resource object </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        public Related_Images_ItemViewer(BriefItemInfo BriefItem, User_Object CurrentUser, Navigation_Object CurrentRequest)
        {
            // Save the arguments for use later
            briefItem = BriefItem;
            currentRequest = CurrentRequest;

            // Get the proper number of thumbnails per page
            if (CurrentUser != null)
            {
                // First, pull the thumbnails per page from the user options
                thumbnailsPerPage = CurrentUser.Get_Setting("Related_Images_ItemViewer:ThumbnailsPerPage", 50);

                // Or was there a new value in the URL?
                if ((currentRequest.Thumbnails_Per_Page.HasValue) && (currentRequest.Thumbnails_Per_Page.Value >= -1))
                {
                    CurrentUser.Add_Setting("Related_Images_ItemViewer:ThumbnailsPerPage", currentRequest.Thumbnails_Per_Page);
                    thumbnailsPerPage = currentRequest.Thumbnails_Per_Page.Value;
                }
            }
            else
            {
                int tempValue = 50;
                object sessionValue = HttpContext.Current.Session["Related_Images_ItemViewer:ThumbnailsPerPage"];
                if (sessionValue != null)
                {
                    int.TryParse(sessionValue.ToString(), out tempValue);
                }
                thumbnailsPerPage = tempValue;

                // Or was there a new value in the URL?
                if ((currentRequest.Thumbnails_Per_Page.HasValue) && (currentRequest.Thumbnails_Per_Page.Value >= -1))
                {
                    HttpContext.Current.Session["Related_Images_ItemViewer:ThumbnailsPerPage"] = currentRequest.Thumbnails_Per_Page;
                    thumbnailsPerPage = currentRequest.Thumbnails_Per_Page.Value;
                }
            }

            // -1 means to display all thumbnails
            if (thumbnailsPerPage == -1)
                thumbnailsPerPage = int.MaxValue;

            // Now, reset the value in the navigation object, since we won't need to set it again
            currentRequest.Thumbnails_Per_Page = -100;

            // Get the proper size of thumbnails per page
            if (CurrentUser != null)
            {
                // First, pull the thumbnails per page from the user options
                thumbnailSize = CurrentUser.Get_Setting("Related_Images_ItemViewer:ThumbnailSize", 1);

                // Or was there a new value in the URL?
                if ((currentRequest.Size_Of_Thumbnails.HasValue) && (currentRequest.Size_Of_Thumbnails.Value > -1))
                {
                    CurrentUser.Add_Setting("Related_Images_ItemViewer:ThumbnailSize", currentRequest.Size_Of_Thumbnails);
                    thumbnailSize = currentRequest.Size_Of_Thumbnails.Value;
                }
            }
            else
            {
                int tempValue = 1;
                object sessionValue = HttpContext.Current.Session["Related_Images_ItemViewer:ThumbnailSize"];
                if (sessionValue != null)
                {
                    int.TryParse(sessionValue.ToString(), out tempValue);
                }
                thumbnailSize = tempValue;

                // Or was there a new value in the URL?
                if ((currentRequest.Size_Of_Thumbnails.HasValue) && (currentRequest.Size_Of_Thumbnails.Value > -1))
                {
                    HttpContext.Current.Session["Related_Images_ItemViewer:ThumbnailSize"] = currentRequest.Size_Of_Thumbnails;
                    thumbnailSize = currentRequest.Size_Of_Thumbnails.Value;
                }
            }

            // Now, reset the value in the navigation object, since we won't need to set it again
            currentRequest.Size_Of_Thumbnails = -1;

            // Get the number of thumbnails for this item
            pageCount = briefItem.Images != null ? briefItem.Images.Count : 0;
        }

        /// <summary> CSS ID for the viewer viewport for this particular viewer </summary>
        /// <value> This always returns the value 'sbkRiiv_Viewer' </value>
        public string ViewerBox_CssId
        {
            get { return "sbkRiiv_Viewer"; }
        }

        /// <summary> Any additional inline style for this viewer that affects the main box around this</summary>
        /// <remarks> This returns NULL, since no inlinse styling is used for the viewer box </remarks>
        public virtual string ViewerBox_InlineStyle
        {
            get { return null; }
        }

        /// <summary> Gets the collection of special behaviors which this item viewer
        /// requests from the main HTML subwriter. </summary>
        public List<HtmlSubwriter_Behaviors_Enum> ItemViewer_Behaviors
        {
            get { return emptyBehaviors; }
        }

        /// <summary> Write any additional values within the HTML Head of the final served page </summary>
        /// <param name="Output"> Output stream currently within the HTML head tags </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public void Write_Within_HTML_Head(TextWriter Output, Custom_Tracer Tracer)
        {
            // Do nothing
        }

        /// <summary> Gets the collection of body attributes to be included 
        /// within the HTML body tag (usually to add events to the body) </summary>
        /// <param name="Body_Attributes"> List of body attributes to be included </param>
        public void Add_ViewerSpecific_Body_Attributes(List<Tuple<string, string>> Body_Attributes)
        {
            Body_Attributes.Add(new Tuple<string, string>("onload", "MakeSpanFlashOnPageLoad();"));
        }

        /// <summary> Adds any viewer_specific information to the left Navigation Bar Menu Section  </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public void Write_Left_Nav_Menu_Section(TextWriter Output, Custom_Tracer Tracer)
        {
            // Do nothing
        }

        /// <summary> Adds any viewer_specific information to the item viewer above the standard pagination buttons </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public void Write_Top_Additional_Navigation_Row(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Related_Images_ItemViewer.Write_Top_Additional_Navigation_Row", "");
            }

            string numOfThumbnails = "thumbnails per page";
            string goToThumbnail = "Go to thumbnail";
            const string SMALL_THUMBNAILS = "Switch to small thumbnails";
            const string MEDIUM_THUMBNAILS = "Switch to medium thumbnails";
            const string LARGE_THUMBNAILS = "Switch to large thumbnails";

            if (currentRequest.Language == Web_Language_Enum.French)
            {
                numOfThumbnails = "vignettes par page";
                //Size_Of_Thumbnail = "la taille des vignettes";
                goToThumbnail = "Aller à l'Vignette";
            }

            if (currentRequest.Language == Web_Language_Enum.Spanish)
            {
                numOfThumbnails = "miniaturas por página";
                //Size_Of_Thumbnail = "Miniatura de tamaño";
                goToThumbnail = "Ir a la miniatura";
            }

            //Start building the top nav bar
            Output.WriteLine("<tr>");
            Output.WriteLine("<td>");
            Output.WriteLine("\t\t<!-- RELATED IMAGES VIEWER TOP NAV ROW -->");

            //Include the js files
            Output.WriteLine("<script type=\"text/javascript\" src=\"" + Static_Resources.Jquery_Ui_1_10_3_Custom_Js + "\"></script>");
            Output.WriteLine("<script type=\"text/javascript\" src=\"" + Static_Resources.Jquery_Color_2_1_1_Js + "\"></script>");
            Output.WriteLine("<table style=\"width: 100%\">");
            Output.WriteLine("\t<tr>");

            //Add the dropdown for the number of thumbnails per page, only if there are >25 thumbnails
            if (pageCount > 25)
            {
                //Redirect to the first page of results when the number of thumbnails option is changed by the user
                string current_viewercode = currentRequest.ViewerCode;
                UrlWriterHelper.Redirect_URL(currentRequest, "1thumbs");

                //   currentRequest.Thumbnails_Per_Page = -1;
                //  string current_Page_url = UrlWriterHelper.Redirect_URL(currentRequest, "1thumbs");

                // Collect the list of options to display
                List<int> thumbsOptions = new List<int> { 25 };
                if (pageCount > 50) thumbsOptions.Add(50);
                if (pageCount > 100) thumbsOptions.Add(100);
                if (pageCount > 250) thumbsOptions.Add(250);
                if (pageCount > 500) thumbsOptions.Add(500);

                // Start the drop down select list 
                Output.WriteLine("\t\t<td style=\"valign:top;text-align:left;padding-left: 20px;\">");
                Output.WriteLine("\t\t\t<select id=\"selectNumOfThumbnails\" onchange=\"location=this.options[this.selectedIndex].value;\">");

                // Step through all the options
                foreach (int thumbOption in thumbsOptions)
                {
                    currentRequest.Thumbnails_Per_Page = (short)thumbOption;
                    if (thumbnailsPerPage == thumbOption)
                    {
                        Output.WriteLine("\t\t\t\t<option value=\"" + UrlWriterHelper.Redirect_URL(currentRequest) + "\" selected=\"selected\">" + thumbOption + " " + numOfThumbnails + "</option>");
                    }
                    else
                    {

                        Output.WriteLine("\t\t\t\t<option value=\"" + UrlWriterHelper.Redirect_URL(currentRequest) + "\">" + thumbOption + " " + numOfThumbnails + "</option>");
                    }
                }

                currentRequest.Thumbnails_Per_Page = -1;
                if (thumbnailsPerPage == int.MaxValue)
                {
                    Output.WriteLine("\t\t\t\t<option value=\"" + UrlWriterHelper.Redirect_URL(currentRequest) + "\" selected=\"selected\">All thumbnails</option>");
                }
                else
                {
                    Output.WriteLine("\t\t\t\t<option value=\"" + UrlWriterHelper.Redirect_URL(currentRequest) + "\">All thumbnails</option>");
                }

                //Reset the Current Mode Thumbnails_Per_Page

                currentRequest.ViewerCode = current_viewercode;
                Output.WriteLine("\t\t\t</select>");
                Output.WriteLine("\t\t</td>");

            }
            currentRequest.Thumbnails_Per_Page = -100;


            //Add the control for the thumbnail size

            //Get the icons for the thumbnail sizes
            Output.WriteLine("\t\t<td id=\"sbkRi_Thumbnailsizeselect\">");
            if (thumbnailSize == 1)
                Output.Write("\t\t\t<img src=\"" + Static_Resources.Thumbs3_Selected_Gif + "\" alt=\"Small\" />");
            else
            {
                currentRequest.Size_Of_Thumbnails = 1;
                Output.Write("\t\t\t<a href=\"" + UrlWriterHelper.Redirect_URL(currentRequest, "1thumbs") + "\" title=\"" + SMALL_THUMBNAILS + "\"><img src=\"" + Static_Resources.Thumbs3_Gif + "\" alt=\"Small\" /></a>");
            }

            if (thumbnailSize == 2)
                Output.Write("<img src=\"" + Static_Resources.Thumbs2_Selected_Gif + "\" alt=\"Medium\" />");
            else
            {
                currentRequest.Size_Of_Thumbnails = 2;
                Output.Write("<a href=\"" + UrlWriterHelper.Redirect_URL(currentRequest, "1thumbs") + "\" title=\"" + MEDIUM_THUMBNAILS + "\"><img src=\"" + Static_Resources.Thumbs2_Gif + "\" alt=\"Medium\" /></a>");
            }
            if (thumbnailSize == 3)
                Output.Write("<img src=\"" + Static_Resources.Thumbs2_Selected_Gif + "\" alt=\"Large\" />");
            else
            {
                currentRequest.Size_Of_Thumbnails = 3;
                Output.Write("<a href=\"" + UrlWriterHelper.Redirect_URL(currentRequest, "1thumbs") + "\" title=\"" + LARGE_THUMBNAILS + "\"><img src=\"" + Static_Resources.Thumbs1_Gif + "\" alt=\"Large\" /></a>");
            }
            //Reset the current mode
            currentRequest.Size_Of_Thumbnails = -1;
            Output.WriteLine("\t\t</td>");


            //Add the dropdown for the thumbnail anchor within the page to directly navigate to
            Output.WriteLine("\t\t<td style=\"valign:top;text-align:right;font-weight:bold;padding-right: 20px;\">");
            Output.WriteLine(goToThumbnail + ":");
            Output.WriteLine("\t\t\t<select onchange=\"location=this.options[this.selectedIndex].value; AddAnchorDivEffect(this.options[this.selectedIndex].value);\" >");

            //iterate through the page items and add each label?
            if (pageCount > 0)
            {
                int thumbnail_count = 0;
                foreach (BriefItem_FileGrouping thisFile in briefItem.Images)
                {
                    string currentPageURL1 = UrlWriterHelper.Redirect_URL(currentRequest, (thumbnail_count / thumbnailsPerPage + (thumbnail_count % thumbnailsPerPage == 0 ? 0 : 1)).ToString() + "thumbs");

                    //  Output.WriteLine("<option value=\"" + current_Page_url1 + "#" + thisFile.Label + "\">" + thisFile.Label + "</option>");
                    if (String.IsNullOrEmpty(thisFile.Label))
                        Output.WriteLine("\t\t\t\t<option value=\"" + currentPageURL1 + "#" + thumbnail_count + "\">" + "(page " + thumbnail_count + ")" + "</option>");
                    else
                    {
                        if (thisFile.Label.Length > 50)
                            Output.WriteLine("\t\t\t\t<option value=\"" + currentPageURL1 + "#" + thumbnail_count + "\">" + thisFile.Label.Substring(0, 50) + "...</option>");
                        else
                            Output.WriteLine("\t\t\t\t<option value=\"" + currentPageURL1 + "#" + thumbnail_count + "\">" + thisFile.Label + "</option>");
                    }

                    thumbnail_count++;
                }
            }
            Output.WriteLine("\t\t\t</select>");

            Output.WriteLine("\t\t</td>");
            Output.WriteLine("\t</tr>");
            Output.WriteLine("</table>");

            // Finish the nav row controls
            Output.WriteLine("\t\t<!-- END RELATED IMAGES VIEWER NAV ROW -->");
            Output.WriteLine("\t\t</td>");
            Output.WriteLine("\t</tr>");
        }

        /// <summary> Write the item viewer main section as HTML directly to the HTTP output stream </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Related_Images_ItemViewer.Write_Main_Viewer_Section", "");
            }

            int images_per_page = thumbnailsPerPage;
            int size_of_thumbnails = thumbnailSize;

            // Save the current viewer code
            string current_view_code = currentRequest.ViewerCode;
            ushort current_view_page = currentRequest.Page.HasValue ? currentRequest.Page.Value : ((ushort)1);

            // Start the citation table
            Output.WriteLine("\t\t<td>");
            Output.WriteLine("\t\t<!-- RELATED IMAGES VIEWER OUTPUT -->");

            // Start the main div for the thumbnails

            ushort page = (ushort)(currentRequest.Page - 1);
            if (page > (pageCount - 1) / images_per_page)
                page = (ushort)((pageCount - 1) / images_per_page);

            //Outer div which contains all the thumbnails
            Output.WriteLine("<div style=\"margin:5px;text-align:center;\">");

            // Get any search terms for highlighting purposes
            List<string> terms = new List<string>();
            if (!String.IsNullOrWhiteSpace(currentRequest.Text_Search))
            {
                string[] splitter = currentRequest.Text_Search.Replace("\"", "").Split(" ".ToCharArray());
                terms.AddRange(from thisSplit in splitter where thisSplit.Trim().Length > 0 select thisSplit.Trim());
            }


            // Step through each page in the item
            for (int page_index = page * images_per_page; (page_index < (page + 1) * images_per_page) && (page_index < pageCount); page_index++)
            {
                // Get this page
                BriefItem_FileGrouping thisPage = briefItem.Images[page_index];

                // Find the jpeg and thumbnail images
                string jpeg = String.Empty;
                string thumbnail = String.Empty;

                foreach (BriefItem_File thisFile in thisPage.Files)
                {
                    if ( String.Compare(thisFile.File_Extension, ".jpg", StringComparison.OrdinalIgnoreCase) == 0 )
                    {
                        if (thisFile.Name.ToLower().IndexOf("thm.jpg") > 0)
                            thumbnail = thisFile.Name;
                        else
                            jpeg = thisFile.Name;
                    }
                }

                // If the thumbnail is not in the METS, just guess its existence
                if (thumbnail.Length == 0)
                    thumbnail = jpeg.ToLower().Replace(".jpg", "thm.jpg");

                // Get the image URL
                currentRequest.Page = (ushort)(page_index + 1);
                currentRequest.ViewerCode = (page_index + 1).ToString();
                string url = UrlWriterHelper.Redirect_URL(currentRequest);

                // Determine the width information and the URL for the image
                string image_url; // = (briefItem.Web.Source_URL + "/" + thumbnail).Replace("\\", "/").Replace("//", "/").Replace("http:/", "http://");
                int width = -1;
                switch (size_of_thumbnails)
                {
                    case 2:
                        image_url = (briefItem.Web.Source_URL + "/" + jpeg).Replace("\\", "/").Replace("//", "/").Replace("http:/", "http://");
                        width = 315;
                        break;

                    case 3:
                        image_url = (briefItem.Web.Source_URL + "/" + jpeg).Replace("\\", "/").Replace("//", "/").Replace("http:/", "http://");
                        width = 472;
                        break;

                    case 4:
                        image_url = (briefItem.Web.Source_URL + "/" + jpeg).Replace("\\", "/").Replace("//", "/").Replace("http:/", "http://");
                        break;

                    default:
                        image_url = (briefItem.Web.Source_URL + "/" + thumbnail).Replace("\\", "/").Replace("//", "/").Replace("http:/", "http://");
                        width = 150;
                        break;

                }

                if (width > 0)
                    Output.WriteLine("  <table class=\"sbkRi_Thumbnail\" id=\"span" + page_index + "\" style=\"width:" + (width + 15) + "px\">");
                else
                    Output.WriteLine("  <table class=\"sbkRi_Thumbnail\" id=\"span" + page_index + "\">");

                Output.WriteLine("    <tr>");
                Output.WriteLine("      <td>");
                Output.WriteLine("        <a id=\"" + page_index + "\" href=\"" + url + "\" title=\"" + thisPage.Label + "\">");
                if (width > 0)
                    Output.WriteLine("          <img src=\"" + image_url + "\" style=\"width:" + width + "px;\" alt=\"MISSING THUMBNAIL\" />");
                else
                    Output.WriteLine("          <img src=\"" + image_url + "\" alt=\"MISSING THUMBNAIL\" />");
                Output.WriteLine("        </a>");
                Output.WriteLine("      </td>");
                Output.WriteLine("    </tr>");
                Output.WriteLine("    <tr>");
                Output.WriteLine("      <td style=\"text-align:center\">" + Text_Search_Term_Highlighter.Hightlight_Term_In_HTML(thisPage.Label, terms, "<span class=\"sbkRi_TextHighlight\">", "</span>") + "</td>");
                Output.WriteLine("    </tr>");
                Output.WriteLine("  </table>");
                Output.WriteLine();
            }

            //Close the outer div
            Output.WriteLine("</div>");

            // Restore the mode
            currentRequest.ViewerCode = current_view_code;
            currentRequest.Page = current_view_page;

            Output.WriteLine("<script type=\"text/javascript\"> WindowResizeActions();</script>");

            // Finish the citation table
            Output.WriteLine("\t\t</td>");
            Output.WriteLine("\t\t<!-- END RELATED IMAGES VIEWER OUTPUT -->");
        }

        /// <summary> Allows controls to be added directory to a place holder, rather than just writing to the output HTML stream </summary>
        /// <param name="MainPlaceHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the bulk of the item viewer's output is displayed</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <remarks> This method does nothing, since nothing is added to the place holder as a control for this item viewer </remarks>
        public void Add_Main_Viewer_Section(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
        {
            // Do nothing
        }

        #region Paging related properties ( custom paging with XX thumbnails displayed per page )

        /// <summary> Gets the flag that indicates if the page selector should be shown </summary>
        /// <value> This uses somewhat unique paging, so the selection ItemViewer_PageSelector_Type_Enum.PageLinks is always returned</value>
        public ItemViewer_PageSelector_Type_Enum Page_Selector
        {
            get
            {
                return ItemViewer_PageSelector_Type_Enum.PageLinks;
            }
        }

        /// <summary> Gets the number of pages for this viewer </summary>
        public int PageCount
        {
            get { return briefItem.Images != null ? ((briefItem.Images.Count - 1) / thumbnailsPerPage) + 1 : 1; }
        }

        /// <summary> Gets the current page for paging purposes </summary>
        public int Current_Page
        {
            get { return currentRequest.Page.HasValue ? currentRequest.Page.Value : 1; }
        }

        /// <summary> Gets the url to go to the first page of thumbnails </summary>
        public string First_Page_URL
        {
            get
            {
                //Get the querystring, if any, from the current url
                string curr_url = HttpContext.Current.Request.RawUrl;
                string queryString = null;

                //Check if query string variables exist in the url
                int index_queryString = curr_url.IndexOf('?');

                if (index_queryString > 0)
                {
                    queryString = (index_queryString < curr_url.Length - 1) ? curr_url.Substring(index_queryString) : String.Empty;
                }

                return ((PageCount > 1) && (currentRequest.Page > 1)) ? UrlWriterHelper.Redirect_URL(currentRequest, "1thumbs") + queryString : String.Empty;
            }
        }

        /// <summary> Gets the url to go to the preivous page of thumbnails </summary>
        public string Previous_Page_URL
        {
            get
            {
                return ((PageCount > 1) && (currentRequest.Page > 1)) ? UrlWriterHelper.Redirect_URL(currentRequest, (currentRequest.Page - 1).ToString() + "thumbs") : String.Empty;
            }
        }

        /// <summary> Gets the url to go to the next page of thumbnails </summary>
        public string Next_Page_URL
        {
            get
            {
                int temp_page_count = PageCount;
                return (temp_page_count > 1) && (currentRequest.Page < temp_page_count) ? UrlWriterHelper.Redirect_URL(currentRequest, (currentRequest.Page + 1).ToString() + "thumbs") : String.Empty;
            }
        }

        /// <summary> Gets the url to go to the last page of thumbnails </summary>
        public string Last_Page_URL
        {
            get
            {
                int temp_page_count = PageCount;
                return (temp_page_count > 1) && (currentRequest.Page < temp_page_count) ? UrlWriterHelper.Redirect_URL(currentRequest, temp_page_count.ToString() + "thumbs") : String.Empty;
            }
        }


        /// <summary> Gets the names to show in the Go To combo box </summary>
        public string[] Go_To_Names
        {
            get
            {
                List<string> goToUrls = new List<string>();
                for (int i = 1; i <= PageCount; i++)
                {
                    goToUrls.Add(UrlWriterHelper.Redirect_URL(currentRequest, i + "thumbs"));
                }
                return goToUrls.ToArray();
            }
        }

        #endregion

    }
}
