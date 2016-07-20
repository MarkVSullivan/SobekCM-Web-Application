using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using SobekCM.Core.BriefItem;
using SobekCM.Core.Client;
using SobekCM.Core.Configuration.Localization;
using SobekCM.Core.FileSystems;
using SobekCM.Core.Items;
using SobekCM.Core.Navigation;
using SobekCM.Core.Users;
using SobekCM.Library.HTML;
using SobekCM.Library.ItemViewer.Menu;
using SobekCM.Library.UI;
using SobekCM.Tools;

namespace SobekCM.Library.ItemViewer.Viewers
{
    /// <summary> Multi-volume viewer prototyper, which is used to check to see if there are other volumes to display, 
    /// to create the link in the main menu, and to create the viewer itself if the user selects that option </summary>
    public class MultiVolumes_ItemViewer_Prototyper : iItemViewerPrototyper
    {
        /// <summary> Constructor for a new instance of the MultiVolumes_ItemViewer_Prototyper class </summary>
        public MultiVolumes_ItemViewer_Prototyper()
        {
            ViewerType = "ALL_VOLUMES";
            ViewerCode = "allvolumes";
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
            // Are there siblings for this item?
            return (CurrentItem.Web.Siblings > 1);
        }

        /// <summary> Flag indicates if this viewer should be override on checkout </summary>
        /// <param name="CurrentItem"> Digital resource to examine to see if this viewer should really be overriden </param>
        /// <returns> FALSE always, since even if one issue is checked out, the rest of the serial may not </returns>
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
            // Determine the label to show
            string resource_type_upper = CurrentItem.Type.ToUpper();
            string label = "All Volumes";
            if (resource_type_upper.IndexOf("NEWSPAPER") >= 0)
            {
                label = "All Issues";
            }
            else if (resource_type_upper.IndexOf("MAP") >= 0)
            {
                label = "Related Maps";
            }
            else if (resource_type_upper.IndexOf("AERIAL") >= 0)
            {
                label = "Related Flights";
            }

            // Get the URL for this
            string previous_code = CurrentRequest.ViewerCode;
            CurrentRequest.ViewerCode = ViewerCode;
            string url = UrlWriterHelper.Redirect_URL(CurrentRequest);
            CurrentRequest.ViewerCode = previous_code;

            // Allow the label to be implemented for this viewer from the database override value
            BriefItem_BehaviorViewer thisViewerInfo = CurrentItem.Behaviors.Get_Viewer(ViewerCode);

            // If this is found, and has a custom label, use that 
            if ((thisViewerInfo != null) && (!String.IsNullOrWhiteSpace(thisViewerInfo.Label)))
                label = thisViewerInfo.Label;

            // Add the item menu information
            Item_MenuItem menuItem = new Item_MenuItem(label, null, null, url, ViewerCode);
            MenuItems.Add(menuItem);
        }

        /// <summary> Creates and returns the an instance of the <see cref="MultiVolumes_ItemViewer"/> class for showing 
        /// the volumes (VIDs) that are associated with a single BibID during execution of a single HTTP request. </summary>
        /// <param name="CurrentItem"> Digital resource object </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Fully built and initialized <see cref="MultiVolumes_ItemViewer"/> object </returns>
        /// <remarks> This method is called whenever a request requires the actual viewer to be created to render the HTML for
        /// the digital resource requested.  The created viewer is then destroyed at the end of the request </remarks>
        public iItemViewer Create_Viewer(BriefItemInfo CurrentItem, User_Object CurrentUser, Navigation_Object CurrentRequest, Custom_Tracer Tracer)
        {
            return new MultiVolumes_ItemViewer(CurrentItem, CurrentUser, CurrentRequest, Tracer);
        }
    }

    /// <summary> Item viewer displays the volumes (VIDs) that are associated with a single BibID </summary>
    /// <remarks> This class extends the implements the <see cref="iItemViewer" /> interface directly. </remarks>
    public class MultiVolumes_ItemViewer : iItemViewer
    {
        private readonly string issues_type;
        private readonly View_Type viewType;
        private readonly int thumbnail_count;

        private readonly BriefItemInfo briefItem;
        private readonly Navigation_Object currentRequest;
        private readonly User_Object currentUser;

        // Empty list of behaviors, prevents an empty set from having to be created over and over
        private static readonly List<HtmlSubwriter_Behaviors_Enum> emptyBehaviors = new List<HtmlSubwriter_Behaviors_Enum>();

        private readonly List<Item_Hierarchy_Details> allVolumes;

        /// <summary> Constructor for a new instance of the MultiVolumes_ItemViewer class, used to display
        ///  the volumes (VIDs) that are associated with a single BibID </summary>
        /// <param name="BriefItem"> Digital resource object </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public MultiVolumes_ItemViewer(BriefItemInfo BriefItem, User_Object CurrentUser, Navigation_Object CurrentRequest, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("MultiVolumes_ItemViewer.Constructor");

            // Save the arguments for use later
            briefItem = BriefItem;
            currentUser = CurrentUser;
            currentRequest = CurrentRequest;

            // Determine the citation type
            viewType = View_Type.Tree;
            if (( currentRequest.Remaining_Url_Segments != null ) && ( currentRequest.Remaining_Url_Segments.Length > 0 ))
                switch (currentRequest.Remaining_Url_Segments[0])
            {
                case "2":
                    viewType = View_Type.Thumbnail;
                    break;

                case "3":
                    viewType = View_Type.List;
                    break;
            }

            string volumes_text = "All Volumes";
            string issues_text = "All Issues";
            string map_text = "Related Map Sets";
            const string AERIAL_TEXT = "Related Flights";

            if (currentRequest.Language == Web_Language_Enum.French)
            {
                volumes_text = "Tous les Volumes";
                issues_text = "Tous les Éditions";
                map_text = "Définit la Carte Connexes";
            }

            if (currentRequest.Language == Web_Language_Enum.Spanish)
            {
                volumes_text = "Todos los Volumenes";
                issues_text = "Todas las Ediciones";
                map_text = "Relacionado Mapa Conjuntos";
            }

            issues_type = volumes_text;
            if (BriefItem.Behaviors.GroupType.ToUpper().IndexOf("NEWSPAPER") >= 0)
            {
                issues_type = issues_text;
            }
            else if (BriefItem.Behaviors.GroupType.ToUpper().IndexOf("MAP") >= 0)
            {
                issues_type = map_text;
            }
            else if (BriefItem.Behaviors.GroupType.ToUpper().IndexOf("AERIAL") >= 0)
            {
                issues_type = AERIAL_TEXT;
            }

            // Get the list of other volumes
            Tracer.Add_Trace("MultiVolumes_ItemViewer.Constructor", "Get the list of items under " + BriefItem.BibID);
            try
            {
                allVolumes = SobekEngineClient.Items.Get_Multiple_Volumes(BriefItem.BibID, Tracer);
            }
            catch (Exception ee)
            {
                Tracer.Add_Trace("Tracking_ItemViewer.Constructor", "Unable to pull volumes under " + BriefItem.BibID);
                CurrentRequest.Mode = Display_Mode_Enum.Error;
                CurrentRequest.Error_Message = "Internal Error : Unable to pull volumes under " + BriefItem.BibID;
                return;
            }


            // If the view type is the thumbnails (where we only show PUBLIC and RESTRICTED items) 
            // need a count of the number of public items
            thumbnail_count = 0;
            if (viewType == View_Type.Thumbnail)
            {
                thumbnail_count = allVolumes.Count(OtherVolume => (OtherVolume.IP_Restriction_Mask >= 0) && (!OtherVolume.Dark));
            }
        }

        /// <summary> CSS ID for the viewer viewport for this particular viewer </summary>
        /// <value> This returns the value 'sbkMviv_Viewer', unless this is displaying in tree mode
        /// in which case it returns 'sbkMviv_ViewerTree'. </value>
        public string ViewerBox_CssId
        {
            get
            {
                return viewType == View_Type.Tree ? "sbkMviv_ViewerTree" : "sbkMviv_Viewer";
            }
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
            // Do nothing 
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
            // Do nothing
        }

        /// <summary> Write the item viewer main section as HTML directly to the HTTP output stream </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer)
        {
            if (viewType != View_Type.Tree)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("MultiVolumes_ItemViewer.Write_Main_Viewer_Section", "Write the main viewer section (for tree and list view)");
                }

                // Build the value
                Output.WriteLine("          <td><div id=\"sbkMviv_ViewerTitle\">" + issues_type + "</div></td>");
                Output.WriteLine("        </tr>");
                Output.WriteLine("        <tr>");
                Output.WriteLine("          <td>");
                Output.WriteLine(viewType == View_Type.Thumbnail ? "            <div id=\"sbkMviv_ThumbnailsArea\">" : "            <div id=\"sbkMviv_MainArea\">");

                if (viewType == View_Type.List)
                {
                    Write_List(Output);
                }
                else if (viewType == View_Type.Thumbnail)
                {
                    Write_Thumbnails(Output);
                }
            }
        }

        /// <summary> Allows controls to be added directory to a place holder, rather than just writing to the output HTML stream </summary>
        /// <param name="MainPlaceHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the bulk of the item viewer's output is displayed</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <remarks> In tree view mode, this adds a tree control directly to the place holder </remarks>
        public void Add_Main_Viewer_Section(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
        {
            // Add the tree view as controls
            if (viewType == View_Type.Tree)
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("MultiVolumes_ItemViewer.Add_Main_Viewer_Section", "Adds tree view control");
                }

                // Build the value
                StringBuilder builder = new StringBuilder(5000);
                builder.AppendLine("          <td><div id=\"sbkMviv_ViewerTitle\">" + issues_type + "</div></td>");
                builder.AppendLine("        </tr>");
                builder.AppendLine("        <tr>");
                builder.AppendLine("          <td>");
                builder.AppendLine(viewType == View_Type.Thumbnail ? "            <div id=\"sbkMviv_ThumbnailsArea\">" : "            <div id=\"sbkMviv_MainArea\">");

                // Add the HTML for the image
                Literal mainLiteral = new Literal { Text = builder.ToString() };
                MainPlaceHolder.Controls.Add(mainLiteral);

                // Add the tree view
                TreeView treeView1 = new TreeView { EnableViewState = false, CssClass = "sbkMviv_Tree" };
                Build_Tree(treeView1);
                MainPlaceHolder.Controls.Add(treeView1);
            }

            // Add the final HTML
            Literal secondLiteral = new Literal();
            if ((briefItem.Web.Related_Titles != null ) && ( briefItem.Web.Related_Titles.Count > 0 ))
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("MultiVolumes_ItemViewer.Add_Main_Viewer_Section", "Add the related titles and close the remaining divs");
                }

                StringBuilder relatedBuilder = new StringBuilder(1000);
                relatedBuilder.AppendLine("<table id=\"sbkMviv_RelatedTitles\">");
                relatedBuilder.AppendLine("  <tr>");
                relatedBuilder.AppendLine("    <td colspan=\"2\"><h2>Related Titles</h2></td>");
                relatedBuilder.AppendLine("  </tr>");
                string url_opts = UrlWriterHelper.URL_Options(currentRequest);
                foreach (BriefItem_Related_Titles thisTitle in briefItem.Web.Related_Titles)
                {
                    relatedBuilder.AppendLine("  <tr>");
                    relatedBuilder.AppendLine("    <td class=\"sbkMviv_RelatedTitlesRelation\">" + thisTitle.Relationship + ": </td>");
                    relatedBuilder.AppendLine("    <td><a href=\"" + thisTitle.Link.Replace("<%URL_OPTS%>", url_opts) + "\">" + thisTitle.Title + "</a></td>");
                    relatedBuilder.AppendLine("  <tr>");
                }
                relatedBuilder.AppendLine("</table>");
                secondLiteral.Text = "" + Environment.NewLine + relatedBuilder + "</div><!-- FINISHING -->" + Environment.NewLine + "</td>" + Environment.NewLine;
            }
            else
            {
                if (Tracer != null)
                {
                    Tracer.Add_Trace("MultiVolumes_ItemViewer.Add_Main_Viewer_Section", "Close the remaining divs");
                }

                secondLiteral.Text = "" + Environment.NewLine + "            </div><!-- FINISHING -->" + Environment.NewLine + "</td>" + Environment.NewLine;
            }
            MainPlaceHolder.Controls.Add(secondLiteral);
        }

        #region Paging related properties ( custom paging with XX thumbnails displayed per page in thumbnail mode )

        /// <summary> Gets the flag that indicates if the page selector should be shown </summary>
        /// <value> This always returns NONE (somewhat strangely, since I think it uses pagingation! ) </value>
        public ItemViewer_PageSelector_Type_Enum Page_Selector
        {
            get
            {
                return ItemViewer_PageSelector_Type_Enum.NONE;
            }
        }

        /// <summary> Gets the number of pages for this viewer </summary>
        /// <value> This is generally single page viewer, so this property usually returns the value 1.  If there
        /// are over 100 volumes and it is currently in thumbnail view, then it will return a value so that 
        /// 60 volumes appear on each page.</value>
        public int PageCount
        {
            get
            {
                if (allVolumes == null)
                    return 1;

                if (viewType != View_Type.Thumbnail)
                    return 1;

                if (currentRequest.SubPage <= 0)
                    currentRequest.SubPage = 1;

                if (thumbnail_count <= 100)
                    return 1;

                return (((thumbnail_count - 1) / 60)) + 1;
            }
        }

        /// <summary> Gets the current page for paging purposes </summary>
        /// <value> This returns either 1, or the <see cref="Navigation_Object.SubPage"/> value from the current reqeust mode</value>
        public int Current_Page
        {
            get
            {
                return currentRequest.SubPage.HasValue ? Math.Max(currentRequest.SubPage.Value, ((short)1)) : 1;
            }
        }

        /// <summary> Gets the url to go to the first page </summary>
        /// <remarks> This only returns a value when the view mode is set to thumbnail and there are over 100 volumes associated with the title </remarks>
        public string First_Page_URL
        {
            get
            {
                if (viewType != View_Type.Thumbnail)
                    return String.Empty;

                ushort? subpage = currentRequest.SubPage;
                currentRequest.SubPage = 1;
                string returnVal = UrlWriterHelper.Redirect_URL(currentRequest);
                currentRequest.SubPage = subpage;
                return returnVal;
            }
        }

        /// <summary> Gets the url to go to the previous page </summary>
        /// <remarks> This only returns a value when the view mode is set to thumbnail and there are over 100 volumes associated with the title </remarks>
        public string Previous_Page_URL
        {
            get
            {
                if (viewType != View_Type.Thumbnail)
                    return String.Empty;

                ushort? subpage = currentRequest.SubPage;
                ushort subpage_current = currentRequest.SubPage.HasValue ? Math.Max(currentRequest.SubPage.Value, ((ushort)1)) : ((ushort)1);

                currentRequest.SubPage = (ushort)(subpage_current - 1);
                string returnVal = UrlWriterHelper.Redirect_URL(currentRequest);
                currentRequest.SubPage = subpage;
                return returnVal;
            }
        }

        /// <summary> Gets the url to go to the next page </summary>
        /// <remarks> This only returns a value when the view mode is set to thumbnail and there are over 100 volumes associated with the title </remarks>
        public string Next_Page_URL
        {
            get
            {
                if (viewType != View_Type.Thumbnail)
                    return String.Empty;

                ushort? subpage = currentRequest.SubPage;
                ushort subpage_current = currentRequest.SubPage.HasValue ? Math.Max(currentRequest.SubPage.Value, ((ushort)1)) : ((ushort)1);

                currentRequest.SubPage = (ushort)(subpage_current + 1);
                string returnVal = UrlWriterHelper.Redirect_URL(currentRequest);
                currentRequest.SubPage = subpage;
                return returnVal;
            }
        }

        /// <summary> Gets the url to go to the last page </summary>
        /// <remarks> This only returns a value when the view mode is set to thumbnail and there are over 100 volumes associated with the title </remarks>
        public string Last_Page_URL
        {
            get
            {
                if (viewType != View_Type.Thumbnail)
                    return String.Empty;

                ushort? subpage = currentRequest.SubPage;
                currentRequest.SubPage = (ushort)((thumbnail_count / 60));
                if ((thumbnail_count % 60) != 0)
                    currentRequest.SubPage = (ushort)(currentRequest.SubPage + 1);
                string returnVal = UrlWriterHelper.Redirect_URL(currentRequest);
                currentRequest.SubPage = subpage;
                return returnVal;
            }
        }

        /// <summary> Gets the names to show in the Go To combo box </summary>
        /// <value> This always returns NULL, since this does not have paging enabled?? </value>
        public string[] Go_To_Names
        {
            get { return null; }
        }

        #endregion

        #region Method to add the volume list as a list

        /// <summary> Writes the list of volumes associated with the same title as a digital resource to the output stream </summary>
        /// <param name="Output"> HTML output response stream </param>
        protected internal void Write_List(TextWriter Output)
        {
            // Save the current viewer code
            string current_view_code = currentRequest.ViewerCode;
            ushort current_view_page = currentRequest.Page.HasValue ? currentRequest.Page.Value : (ushort)1;

            // Compute the base redirect URL
            string current_vid = currentRequest.VID;
            currentRequest.VID = "<%VID%>";
            string redirect_url = UrlWriterHelper.Redirect_URL(currentRequest, String.Empty);
            currentRequest.VID = current_vid;

            // Determine the max depth on this item
            int depth = 1;
            foreach (Item_Hierarchy_Details thisItem in allVolumes)
            {
                if (( !String.IsNullOrEmpty(thisItem.Level2_Text)) && (depth < 2))
                {
                    depth = 2;
                }
                if ((!String.IsNullOrEmpty(thisItem.Level3_Text)) && (depth < 3))
                {
                    depth = 3;
                    break;
                }
            }

            // Start the table
            Output.WriteLine("<table id=\"sbkMviv_Table\">");
            Output.WriteLine("  <tr id=\"sbkMviv_TableHeaderRow\">");
            Output.WriteLine("    <th style=\"width:50px;\">VID</th>");
            Output.WriteLine("    <th>LEVEL 1</th>");
            if (depth > 1)
            {
                Output.WriteLine("    <th>LEVEL 2</th>");
            }
            if (depth > 2)
            {
                Output.WriteLine("    <th>LEVEL 3</th>");
            }
            Output.WriteLine("    <th style=\"width:65px;\">ACCESS</th>");
            Output.WriteLine("  </tr>");

            foreach (Item_Hierarchy_Details thisItem in allVolumes)
            {
                int access_int = thisItem.IP_Restriction_Mask;
                bool dark = thisItem.Dark;
                if (dark) access_int = -1;

                if (access_int < 0)
                {
                    Output.WriteLine("  <tr class=\"sbkMviv_TablePrivateItem\">");
                }
                else
                {
                    Output.WriteLine(access_int == 0 ? "  <tr class=\"sbkMviv_TablePublicItem\">" : "  <tr class=\"sbkMviv_TableRestrictedItem\">");
                }

                Output.WriteLine("  <tr>");
                Output.WriteLine("    <td><a href=\"" + redirect_url.Replace("<%VID%>", thisItem.VID) + "\">" + thisItem.VID + "</a></td>");
                if ( String.IsNullOrEmpty(thisItem.Level1_Text))
                {
                    Output.WriteLine("    <td>" + HttpUtility.HtmlEncode(thisItem.Title) + "</td>");
                }
                else
                {
                    Output.WriteLine("    <td>" + HttpUtility.HtmlEncode(thisItem.Level1_Text) + "</td>");
                }
                if (depth > 1)
                {
                    if ( !String.IsNullOrEmpty(thisItem.Level2_Text))
                        Output.WriteLine("    <td>" + HttpUtility.HtmlEncode(thisItem.Level2_Text) + "</td>");
                    else
                        Output.WriteLine("    <td>&nbsp;</td>");
                }
                if (depth > 2)
                {
                    if (!String.IsNullOrEmpty(thisItem.Level3_Text))
                        Output.WriteLine("    <td>" + HttpUtility.HtmlEncode(thisItem.Level3_Text) + "</td>");
                    else
                        Output.WriteLine("    <td>&nbsp;</td>");
                }

                if (access_int < 0)
                {
                    Output.WriteLine(dark ? "    <td>dark</td>" : "    <td>private</td>");
                }
                else
                {
                    Output.WriteLine(access_int == 0 ? "    <td>public</td>" : "    <td>restricted</td>");
                }

                Output.WriteLine("  </tr>");
                Output.WriteLine("  <tr><td class=\"sbkMviv_TableRowSeperator\" colspan=\"" + (depth + 2) + "\"></td></tr>");
            }
            Output.WriteLine("</table>");

            // Restore the mode
            currentRequest.ViewerCode = current_view_code;
            currentRequest.Page = current_view_page;
        }


        #endregion

        #region Method to add the volume list as thumbnails

        /// <summary> Writes the collection of thumbnails for volumes associated with the same title as a digital resource to the output stream </summary>
        /// <param name="Output"> HTML output response stream </param>
        protected internal void Write_Thumbnails(TextWriter Output)
        {
            if (currentRequest.SubPage <= 0)
                currentRequest.SubPage = 1;

            // Is this a newspaper?
            bool newspaper = (briefItem.Behaviors.GroupType.ToUpper().IndexOf("NEWSPAPER") >= 0);

            // Save the current viewer code
            string current_view_code = currentRequest.ViewerCode;
            ushort current_view_page = currentRequest.Page.HasValue ? currentRequest.Page.Value : (ushort)1;

            //Outer div which contains all the thumbnails
            Output.WriteLine("<div style=\"margin:5px;text-align:center;\">");

            // Find the base address for this thumbnail
            string jpeg_base = SobekFileSystem.Resource_Web_Uri(briefItem);

            // Compute the base redirect URL
            string current_vid = currentRequest.VID;
            string viewercode = currentRequest.ViewerCode;
            ushort? subpage = currentRequest.SubPage;
            currentRequest.ViewerCode = String.Empty;
            currentRequest.SubPage = 0;
            currentRequest.VID = "<%VID%>";
            string redirect_url = UrlWriterHelper.Redirect_URL(currentRequest, String.Empty);
            currentRequest.ViewerCode = viewercode;
            currentRequest.SubPage = subpage;
            currentRequest.VID = current_vid;

            // Get the rows which match the requirements (either PUBLIC or RESTRICTED)
            List<Item_Hierarchy_Details> publicItems = allVolumes.Where(ThisItem => (ThisItem.IP_Restriction_Mask >= 0) && (!ThisItem.Dark)).ToList();

            // Step through item in the results
            int currenSubPage = currentRequest.SubPage.HasValue ? currentRequest.SubPage.Value : 0;
            int startItemCount = (currenSubPage - 1) * 60;
            int endItemCount = currenSubPage * 60;
            if (publicItems.Count < 100)
                endItemCount = publicItems.Count;
            for (int i = startItemCount; (i < endItemCount) && (i < publicItems.Count); i++)
            {
                Item_Hierarchy_Details thisItem = publicItems[i];

                string thumbnail_text = thisItem.Title;
                if (!String.IsNullOrEmpty(thisItem.Level1_Text))
                {
                    if (!String.IsNullOrEmpty(thisItem.Level2_Text))
                    {
                        if (!String.IsNullOrEmpty(thisItem.Level3_Text))
                        {
                            if (newspaper)
                            {
                                thumbnail_text = thisItem.Level2_Text + " " + thisItem.Level3_Text + ", " + thisItem.Level1_Text;
                            }
                            else
                            {
                                thumbnail_text = thisItem.Level1_Text + ". " + thisItem.Level2_Text + ". " + thisItem.Level3_Text + ".";
                            }
                        }
                        else
                        {
                            thumbnail_text = thisItem.Level1_Text + ". " + thisItem.Level2_Text + ".";
                        }
                    }
                    else
                    {
                        thumbnail_text = thisItem.Level1_Text + ".";
                    }
                }

                string url = redirect_url.Replace("<%VID%>", thisItem.VID).Replace("&", "&amp;");

                Output.WriteLine(thisItem.ItemID == briefItem.Web.ItemID ? "  <table class=\"sbkMviv_Thumbnail\" id=\"sbkMviv_ThumbnailCurrent\" style=\"width:165px\">" : "  <table class=\"sbkMviv_Thumbnail\" style=\"width:165px\">");
                Output.WriteLine("    <tr>");
                Output.WriteLine("      <td>");
                Output.WriteLine("        <a href=\"" + url + "\" title=\"" + thumbnail_text + "\">");
                Output.WriteLine("          <img src=\"" + jpeg_base + thisItem.VID + "/" + thisItem.MainThumbnail + "\" alt=\"MISSING THUMBNAIL\" />");
                Output.WriteLine("        </a>");
                Output.WriteLine("      </td>");
                Output.WriteLine("    </tr>");
                Output.WriteLine("    <tr>");
                Output.WriteLine("      <td style=\"text-align:center\">" + thumbnail_text + "</td>");
                Output.WriteLine("    </tr>");
                Output.WriteLine("  </table>");
                Output.WriteLine();
            }

            //Close the outer div
            Output.WriteLine("</div>");

            // Restore the mode
            currentRequest.ViewerCode = current_view_code;
            currentRequest.Page = current_view_page;
        }


        #endregion

        #region Method to add the volume list in tree view

        /// <summary> Populates a tree view control with the hierarchical collection of volumes associated with the same title as a digital resource </summary>
        /// <param name="TreeView1"> Treeview control to populate with the associated volumes </param>
        protected internal void Build_Tree(TreeView TreeView1)
        {
            const int LINE_TO_LONG = 100;

            // Add the root node
            TreeNode rootNode = new TreeNode("<span id=\"sbkMviv_TableGroupTitle\">" + briefItem.Behaviors.GroupTitle + "</span>");
            if (briefItem.Behaviors.GroupTitle.Length > LINE_TO_LONG)
                rootNode.Text = "<span id=\"sbkMviv_TableGroupTitle\">" + briefItem.Behaviors.GroupTitle.Substring(0, LINE_TO_LONG) + "...</span>";
            rootNode.SelectAction = TreeNodeSelectAction.None;
            TreeView1.Nodes.Add(rootNode);

            // Is this a newspaper?
            bool newspaper = briefItem.Behaviors.GroupType.ToUpper() == "NEWSPAPER";

            // Add the first layer of nodes
            //  Hashtable nodeHash = new Hashtable();
            string lastNodeText1 = String.Empty;
            string lastNodeText2 = String.Empty;
            string lastNodeText3 = String.Empty;
            string lastNodeText4 = String.Empty;
            TreeNode lastNode1 = null;
            TreeNode lastNode2 = null;
            TreeNode lastNode3 = null;
            TreeNode lastNode4 = null;
            TreeNode currentSelectedNode = null;

            // Compute the base redirect URL
            string current_vid = currentRequest.VID;
            currentRequest.VID = "<%VID%>";
            string redirect_url = UrlWriterHelper.Redirect_URL(currentRequest, String.Empty);
            currentRequest.VID = current_vid;

            // Does this user have special rights on the item?
            bool specialRights = ((currentUser != null) && ((currentUser.Is_System_Admin) || (currentUser.Is_Internal_User) || (currentUser.Can_Edit_This_Item(briefItem.BibID, briefItem.Type, briefItem.Behaviors.Source_Institution_Aggregation, briefItem.Behaviors.Holding_Location_Aggregation, briefItem.Behaviors.Aggregation_Code_List))));

            foreach (Item_Hierarchy_Details thisItem in allVolumes)
            {
                // Do not show PRIVATE items in this tree view
                int access_int = thisItem.IP_Restriction_Mask;
                bool dark = thisItem.Dark;
                if (dark) access_int = -1;
                if ((access_int >= 0) || (specialRights))
                {
                    // Set the access string and span name
                    string access_string = String.Empty;
                    string access_span_start = String.Empty;
                    string access_span_end = String.Empty;
                    if (dark)
                    {
                        access_span_start = "<span class=\"sbkMviv_TreeDarkNode\">";
                        access_string = " ( dark )";
                        access_span_end = "</span>";
                    }
                    else
                    {
                        if (access_int < 0)
                        {
                            access_span_start = "<span class=\"sbkMviv_TreePrivateNode\">";
                            access_string = " ( private )";
                            access_span_end = "</span>";
                        }
                        else if (access_int > 0)
                        {
                            access_span_start = "<span class=\"sbkMviv_TreeRestrictedNode\">";
                            access_string = " ( some restrictions apply )";
                            access_span_end = "</span>";
                        }
                    }

                    // Determine the text for all the levels (and nodes)
                    string level1_text = UI_ApplicationCache_Gateway.Translation.Get_Translation(thisItem.Level1_Text, currentRequest.Language);
                    string level2_text = String.Empty;
                    string level3_text = String.Empty;
                    string level4_text = String.Empty;
                    string level5_text = String.Empty;
                    string title = thisItem.Title;
                    if (level1_text.Length == 0)
                    {
                        TreeNode singleNode = new TreeNode(access_span_start + title + access_string + access_span_end);
                        if (title.Length > LINE_TO_LONG)
                        {
                            singleNode.ToolTip = title;
                            title = title.Substring(0, LINE_TO_LONG) + "...";
                            singleNode.Text = access_span_start + title + access_string + access_span_end;
                        }
                        if (thisItem.ItemID == briefItem.Web.ItemID)
                        {
                            currentSelectedNode = singleNode;
                            singleNode.SelectAction = TreeNodeSelectAction.None;
                            singleNode.Text = "<span id=\"sbkMviv_TreeSelectedNode\">" + singleNode.Text + "</span>";

                        }
                        else
                        {
                            singleNode.NavigateUrl = redirect_url.Replace("<%VID%>", thisItem.VID);
                        }
                        rootNode.ChildNodes.Add(singleNode);
                    }

                    // Look at the first level
                    if (level1_text.Length > 0)
                    {
                        level2_text = UI_ApplicationCache_Gateway.Translation.Get_Translation(thisItem.Level2_Text, currentRequest.Language);
                        if (level2_text.Length == 0)
                        {
                            TreeNode singleNode1 = new TreeNode(access_span_start + level1_text + access_string + access_span_end);
                            if (thisItem.Level1_Text.Length > LINE_TO_LONG)
                            {
                                singleNode1.ToolTip = level1_text;
                                level1_text = level1_text.Substring(0, LINE_TO_LONG) + "...";
                                singleNode1.Text = access_span_start + level1_text + access_string + access_span_end;
                            }
                            if (thisItem.ItemID == briefItem.Web.ItemID)
                            {
                                currentSelectedNode = singleNode1;
                                singleNode1.SelectAction = TreeNodeSelectAction.None;
                                singleNode1.Text = "<span id=\"sbkMviv_TreeSelectedNode\">" + singleNode1.Text + "</span>";

                            }
                            else
                            {
                                singleNode1.NavigateUrl = redirect_url.Replace("<%VID%>", thisItem.VID);
                            }
                            rootNode.ChildNodes.Add(singleNode1);
                        }
                        else
                        {
                            if ((lastNode1 == null) || (lastNodeText1 != level1_text.ToUpper()))
                            {
                                // Since this is the TOP level, let's look down and see if there are any non-private, non-dark items
                                string nontranslated = thisItem.Level1_Text;
                                bool allPrivate = allVolumes.All(ThisVolume => (thisItem.IP_Restriction_Mask < 0) || (dark) || (String.Compare(ThisVolume.Level1_Text, nontranslated, StringComparison.OrdinalIgnoreCase) != 0) || (thisItem.Level1_Index != ThisVolume.Level1_Index));

                                lastNode1 = new TreeNode(level1_text);
                                if (level1_text.Length > LINE_TO_LONG)
                                {
                                    lastNode1.ToolTip = lastNode1.Text;
                                    level1_text = level1_text.Substring(0, LINE_TO_LONG) + "...";
                                    lastNode1.Text = level1_text;
                                }

                                if (allPrivate)
                                {
                                    lastNode1.Text = "<span class=\"sbkMviv_TreePrivateNode\">" + level1_text + " ( all private or dark )</span>";
                                }

                                lastNode1.SelectAction = TreeNodeSelectAction.None;

                                lastNodeText1 = level1_text.ToUpper();
                                rootNode.ChildNodes.Add(lastNode1);

                                lastNode2 = null;
                                lastNodeText2 = String.Empty;
                                lastNode3 = null;
                                lastNodeText3 = String.Empty;
                                lastNode4 = null;
                                lastNodeText4 = String.Empty;
                            }
                        }
                    }

                    // Look at the second level
                    if ((level2_text.Length > 0) && (lastNode1 != null))
                    {
                        level3_text = UI_ApplicationCache_Gateway.Translation.Get_Translation(thisItem.Level3_Text, currentRequest.Language);
                        if (level3_text.Length == 0)
                        {
                            TreeNode singleNode2 = new TreeNode(access_span_start + level2_text + access_string + access_span_end);
                            if (level2_text.Length > LINE_TO_LONG)
                            {
                                singleNode2.ToolTip = level2_text;
                                level2_text = level2_text.Substring(0, LINE_TO_LONG) + "...";
                                singleNode2.Text = access_span_start + level2_text + access_string + access_span_end;
                            }
                            if (thisItem.ItemID == briefItem.Web.ItemID)
                            {
                                currentSelectedNode = singleNode2;
                                singleNode2.SelectAction = TreeNodeSelectAction.None;
                                singleNode2.Text = "<span id=\"sbkMviv_TreeSelectedNode\">" + level2_text + access_string + "</span>";
                            }
                            else
                            {
                                singleNode2.NavigateUrl = redirect_url.Replace("<%VID%>", thisItem.VID);
                            }
                            lastNode1.ChildNodes.Add(singleNode2);
                        }
                        else
                        {
                            if ((lastNode2 == null) || (lastNodeText2 != level2_text.ToUpper()))
                            {
                                lastNode2 = new TreeNode(level2_text);
                                if (level2_text.Length > LINE_TO_LONG)
                                {
                                    lastNode2.ToolTip = lastNode2.Text;
                                    lastNode2.Text = level2_text.Substring(0, LINE_TO_LONG) + "...";
                                }
                                lastNode2.SelectAction = TreeNodeSelectAction.None;
                                lastNodeText2 = level2_text.ToUpper();
                                lastNode1.ChildNodes.Add(lastNode2);

                                lastNode3 = null;
                                lastNodeText3 = String.Empty;
                                lastNode4 = null;
                                lastNodeText4 = String.Empty;
                            }
                        }
                    }

                    // Look at the third level
                    if ((level3_text.Length > 0) && (lastNode2 != null))
                    {
                        level4_text = UI_ApplicationCache_Gateway.Translation.Get_Translation(thisItem.Level4_Text, currentRequest.Language);
                        if (level4_text.Length == 0)
                        {
                            TreeNode singleNode3 = new TreeNode(access_span_start + level3_text + access_string + access_span_end);
                            if (level3_text.Length > LINE_TO_LONG)
                            {
                                singleNode3.ToolTip = level3_text;
                                level3_text = level3_text.Substring(0, LINE_TO_LONG) + "...";
                                singleNode3.Text = access_span_start + level3_text + access_string + access_span_end;
                            }
                            if (newspaper)
                            {
                                level3_text = access_span_start + level2_text + " " + level3_text + ", " + level1_text + access_string + access_span_end;
                                singleNode3.Text = level3_text;
                            }

                            if (thisItem.ItemID == briefItem.Web.ItemID)
                            {
                                currentSelectedNode = singleNode3;
                                singleNode3.SelectAction = TreeNodeSelectAction.None;
                                singleNode3.Text = "<span id=\"sbkMviv_TreeSelectedNode\">" + level3_text + access_string + "</span>";
                            }
                            else
                            {
                                singleNode3.NavigateUrl = redirect_url.Replace("<%VID%>", thisItem.VID);
                            }

                            lastNode2.ChildNodes.Add(singleNode3);
                        }
                        else
                        {
                            if ((lastNode3 == null) || (lastNodeText3 != level3_text.ToUpper()))
                            {
                                lastNode3 = new TreeNode(level3_text);
                                if (level3_text.Length > LINE_TO_LONG)
                                {
                                    lastNode3.ToolTip = lastNode3.Text;
                                    lastNode3.Text = level3_text.Substring(0, LINE_TO_LONG) + "...";
                                }
                                lastNode3.SelectAction = TreeNodeSelectAction.None;
                                lastNodeText3 = level3_text.ToUpper();
                                lastNode2.ChildNodes.Add(lastNode3);

                                lastNode4 = null;
                                lastNodeText4 = String.Empty;
                            }
                        }
                    }

                    // Look at the fourth level
                    if ((level4_text.Length > 0) && (lastNode3 != null))
                    {
                        UI_ApplicationCache_Gateway.Translation.Get_Translation(thisItem.Level5_Text, currentRequest.Language);
                        if (level5_text.Length == 0)
                        {
                            TreeNode singleNode4 = new TreeNode(access_span_start + level4_text + access_string + access_span_end);
                            if (level4_text.Length > LINE_TO_LONG)
                            {
                                singleNode4.ToolTip = level4_text;
                                level4_text = level4_text.Substring(0, LINE_TO_LONG) + "...";
                                singleNode4.Text = access_span_start + level4_text + access_string + access_span_end;
                            }
                            if (thisItem.ItemID == briefItem.Web.ItemID)
                            {
                                currentSelectedNode = singleNode4;
                                singleNode4.SelectAction = TreeNodeSelectAction.None;
                                singleNode4.Text = "<span id=\"sbkMviv_TreeSelectedNode\">" + level4_text + access_string + "</span>";
                            }
                            else
                            {
                                singleNode4.NavigateUrl = redirect_url.Replace("<%VID%>", thisItem.VID);
                            }
                            lastNode3.ChildNodes.Add(singleNode4);
                        }
                        else
                        {
                            if ((lastNode4 == null) || (lastNodeText4 != level4_text.ToUpper()))
                            {
                                lastNode4 = new TreeNode(level4_text);
                                if (level4_text.Length > LINE_TO_LONG)
                                {
                                    lastNode4.ToolTip = lastNode4.Text;
                                    lastNode4.Text = level4_text.Substring(0, LINE_TO_LONG) + "...";
                                }
                                lastNode4.SelectAction = TreeNodeSelectAction.None;
                                lastNodeText4 = level4_text.ToUpper();
                                lastNode3.ChildNodes.Add(lastNode4);
                            }
                        }
                    }

                    // Look at the fifth level
                    if ((level5_text.Length > 0) && (lastNode4 != null))
                    {
                        TreeNode lastNode5 = new TreeNode(access_span_start + level5_text + access_string + access_span_end);
                        if (level5_text.Length > LINE_TO_LONG)
                        {
                            lastNode5.ToolTip = level5_text;
                            level5_text = level5_text.Substring(0, LINE_TO_LONG) + "...";
                            lastNode5.Text = access_span_start + level5_text + access_string + access_span_end;
                        }
                        if (thisItem.ItemID == briefItem.Web.ItemID)
                        {
                            currentSelectedNode = lastNode5;
                            lastNode5.SelectAction = TreeNodeSelectAction.None;
                            lastNode5.Text = "<span id=\"sbkMviv_TreeSelectedNode\">" + level5_text + access_string + "</span>";
                        }
                        else
                        {
                            lastNode5.NavigateUrl = redirect_url.Replace("<%VID%>", thisItem.VID);
                        }
                        lastNode4.ChildNodes.Add(lastNode5);
                    }
                }
            }

            rootNode.CollapseAll();
            rootNode.Expand();

            if (currentSelectedNode == null) return;

            while ((currentSelectedNode != rootNode) && (currentSelectedNode != null))
            {
                currentSelectedNode.Expand();
                currentSelectedNode = currentSelectedNode.Parent;
            }
        }

        #endregion

        #region Nested type: View_Type

        private enum View_Type : byte { Tree = 1, Thumbnail, List };

        #endregion
    }
}
