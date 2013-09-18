#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using SobekCM.Library.Settings;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Resource_Object.Metadata_Modules;
using SobekCM.Resource_Object.Metadata_Modules.EAD;
using SobekCM.Resource_Object.Behaviors;
using SobekCM.Library.Aggregations;
using SobekCM.Library.Application_State;
using SobekCM.Library.Configuration;
using SobekCM.Library.Database;
using SobekCM.Library.Email;
using SobekCM.Library.ItemViewer;
using SobekCM.Library.ItemViewer.Fragments;
using SobekCM.Library.ItemViewer.Viewers;
using SobekCM.Library.Items;
using SobekCM.Library.MemoryMgmt;
using SobekCM.Library.Navigation;
using SobekCM.Library.Users;

#endregion

namespace SobekCM.Library.HTML
{
    /// <summary> Item html subwriter renders views on a single digital resource </summary>
    /// <remarks> This class extends the <see cref="abstractHtmlSubwriter"/> abstract class. </remarks>
    public class Item_HtmlSubwriter : abstractHtmlSubwriter
    {
        #region Private class members 

        private readonly SobekCM_Item currentItem;
        private Page_TreeNode currentPage;
        private readonly User_Object currentUser;
        private readonly bool isEadTypeItem;
        private bool itemCheckedOutByOtherUser;
        private readonly bool itemRestrictedFromUserByIp;
        private readonly SobekCM_Items_In_Title itemsInTitle;
        private readonly int searchResultsCount;
        private readonly bool showToc;
        private readonly bool showZoomable;
        private bool tocSelectedComplete;
        private readonly Language_Support_Info translations;
        private TreeView treeView1;
        private readonly bool userCanEditItem;
        private readonly List<HtmlSubwriter_Behaviors_Enum> behaviors;
        private string buttonsHtml;
        private string pageLinksHtml;

        #endregion

        #region Constructor(s)

        /// <summary> Constructor for a new instance of the Item_HtmlSubwriter class </summary>
        /// <param name="Current_Item">Current item to display </param>
        /// <param name="Current_Page"> Current page within the item</param>
        /// <param name="Current_User"> Currently logged on user for determining rights over this item </param>
        /// <param name="Code_Manager"> List of valid collection codes, including mapping from the Sobek collections to Greenstone collections</param>
        /// <param name="Translator"> Language support object which handles simple translational duties </param>
        /// <param name="Show_TOC"> Flag indicates whether to show the table of contents open for this item </param>
        /// <param name="Show_Zoomable"> Flag indicates if the zoomable server is available </param>
        /// <param name="Current_Mode"> Mode / navigation information for the current request</param>
        /// <param name="Current_Collection"> Current item aggregation this item is being displayed from (if there is one) </param>
        /// <param name="Item_Restricted_Message"> Message to be shown because this item is restriced from the current user by IP address </param>
        /// <param name="Items_In_Title"> List of items within a title (for item group display in particular) </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public Item_HtmlSubwriter(SobekCM_Item Current_Item, Page_TreeNode Current_Page, User_Object Current_User,
                                  Aggregation_Code_Manager Code_Manager,
                                  Language_Support_Info Translator, bool Show_TOC, bool Show_Zoomable,
                                  SobekCM_Navigation_Object Current_Mode,
                                  Item_Aggregation Current_Collection,
                                  string Item_Restricted_Message,
                                  SobekCM_Items_In_Title Items_In_Title,
                                  Custom_Tracer Tracer )
        {
            Mode = Current_Mode;
            currentUser = Current_User;
            currentItem = Current_Item;
            currentPage = Current_Page;
            itemsInTitle = Items_In_Title;
            translations = Translator;
            showToc = Show_TOC;
            showZoomable = Show_Zoomable;
            Hierarchy_Object = Current_Collection;
            itemCheckedOutByOtherUser = false;
            userCanEditItem = false;
            searchResultsCount = 0;

            // Determine if this item is an EAD
            isEadTypeItem = (currentItem.Get_Metadata_Module(GlobalVar.EAD_METADATA_MODULE_KEY) != null);

            // Determine if this item is actually restricted 
            itemRestrictedFromUserByIp = Item_Restricted_Message.Length > 0;

            // If this item is restricted by IP than alot of the upcoming code is unnecessary
            if (!itemRestrictedFromUserByIp)
            {
                #region Region suppressed currently - was for adding feature to a map image?

                //// Searching for EAD/EAC type items is different from others
                //if (!isEadTypeItem)
                //{
                //    // If there is a coordinate search, and polygons, do that
                //    // GEt the geospatial metadata module
                //    GeoSpatial_Information geoInfo = currentItem.Get_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY) as GeoSpatial_Information;
                //    if ((geoInfo != null) && (geoInfo.hasData))
                //    {
                //        if ((currentMode.Coordinates.Length > 0) && (geoInfo.Polygon_Count > 1))
                //        {
                //            // Determine the coordinates in this search
                //            string[] splitter = currentMode.Coordinates.Split(",".ToCharArray());

                //            if (((splitter.Length > 1) && (splitter.Length < 4)) || ((splitter.Length == 4) && (splitter[2].Length == 0) && (splitter[3].Length == 0)))
                //            {
                //                Double.TryParse(splitter[0], out providedMaxLat);
                //                Double.TryParse(splitter[1], out providedMaxLong);
                //                providedMinLat = providedMaxLat;
                //                providedMinLong = providedMaxLong;
                //            }
                //            else if (splitter.Length >= 4)
                //            {
                //                Double.TryParse(splitter[0], out providedMaxLat);
                //                Double.TryParse(splitter[1], out providedMaxLong);
                //                Double.TryParse(splitter[2], out providedMinLat);
                //                Double.TryParse(splitter[3], out providedMinLong);
                //            }


                //            // Now, if there is length, determine the count of results
                //            searchResultsString = new List<string>();
                //            if (searchResultsString.Count > 0)
                //            {
                //                searchResultsCount = searchResultsString.Count;

                //                // Also, look to see where the current point lies in the matching, current polygon
                //                if ((providedMaxLong == providedMinLong) && (providedMaxLat == providedMinLat))
                //                {
                //                    foreach (Coordinate_Polygon itemPolygon in geoInfo.Polygons)
                //                    {
                //                        // Is this the current page?
                //                        if (itemPolygon.Page_Sequence == currentMode.Page)
                //                        {
                //                            if (itemPolygon.is_In_Bounding_Box(providedMaxLat, providedMaxLong))
                //                            {
                //                                searchMatchOnThisPage = true;
                //                                ReadOnlyCollection<Coordinate_Point> boundingBox = itemPolygon.Bounding_Box;
                //                                featureYRatioLocation = Math.Abs(((providedMaxLat - boundingBox[0].Latitude)/(boundingBox[0].Latitude - boundingBox[1].Latitude)));
                //                                featureXRatioLocation = Math.Abs(((providedMaxLong - boundingBox[0].Longitude)/(boundingBox[0].Longitude - boundingBox[1].Longitude)));
                //                            }
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}

                #endregion

                // Determine if this user can edit this item
                if (currentUser != null)
                {
                    userCanEditItem = currentUser.Can_Edit_This_Item(currentItem);
                }

                // Is this a postback?
                if ((currentMode.isPostBack) && ( currentUser != null))
                {
                    // Handle any actions from standard user action (i.e., email, add to bookshelf, etc.. )
                    if (HttpContext.Current.Request.Form["item_action"] != null)
                    {
                        string action = HttpContext.Current.Request.Form["item_action"].ToLower().Trim();

                        if (action == "email")
                        {
                            string address = HttpContext.Current.Request.Form["email_address"].Replace(";", ",").Trim();
                            string comments = HttpContext.Current.Request.Form["email_comments"].Trim();
                            string format = HttpContext.Current.Request.Form["email_format"].Trim().ToUpper();
                            if (address.Length > 0)
                            {
                                // Determine the email format
                                bool is_html_format = format != "TEXT";

                                // CC: the user, unless they are already on the list
                                string cc_list = currentUser.Email;
                                if (address.ToUpper().IndexOf(currentUser.Email.ToUpper()) >= 0)
                                    cc_list = String.Empty;

                                // Send the email
                                HttpContext.Current.Session.Add("ON_LOAD_MESSAGE", !Item_Email_Helper.Send_Email(address, cc_list, comments, currentUser.Full_Name,currentMode.SobekCM_Instance_Abbreviation,currentItem,is_html_format,HttpContext.Current.Items["Original_URL"].ToString(), currentUser.UserID)
                                    ? "Error encountered while sending email" : "Your email has been sent");

                                HttpContext.Current.Response.Redirect( HttpContext.Current.Items["Original_URL"].ToString(), false);
                                HttpContext.Current.ApplicationInstance.CompleteRequest();
                                Current_Mode.Request_Completed = true;
                                return;
                            }
                        }

                        if (action == "add_item")
                        {
                            string usernotes = HttpContext.Current.Request.Form["add_notes"].Trim();
                            string foldername = HttpContext.Current.Request.Form["add_bookshelf"].Trim();
                            bool open_bookshelf = HttpContext.Current.Request.Form["open_bookshelf"] != null;

                            if (SobekCM_Database.Add_Item_To_User_Folder(currentUser.UserID, foldername, currentItem.BibID, currentItem.VID, 0, usernotes, Tracer))
                            {
                                currentUser.Add_Bookshelf_Item(currentItem.BibID, currentItem.VID);

                                // Ensure this user folder is not sitting in the cache
                                Cached_Data_Manager.Remove_User_Folder_Browse(currentUser.UserID, foldername, Tracer);

                                HttpContext.Current.Session.Add("ON_LOAD_MESSAGE", "Item was saved to your bookshelf.");

                                if (open_bookshelf)
                                {
                                    HttpContext.Current.Session.Add("ON_LOAD_WINDOW", "?m=lmfl" + foldername.Replace("\"", "%22").Replace("'", "%27").Replace("=", "%3D").Replace("&", "%26") + "&vp=1");
                                }
                            }
                            else
                            {
                                HttpContext.Current.Session.Add("ON_LOAD_MESSAGE", "ERROR encountered while trying to save to your bookshelf.");
                            }

                            HttpContext.Current.Response.Redirect(HttpContext.Current.Items["Original_URL"].ToString(), false);
                            HttpContext.Current.ApplicationInstance.CompleteRequest();
                            Current_Mode.Request_Completed = true;
                            return;
                        }

                        if (action == "remove")
                        {
                            if (SobekCM_Database.Delete_Item_From_User_Folders(currentUser.UserID, currentItem.BibID, currentItem.VID, Tracer))
                            {
                                currentUser.Remove_From_Bookshelves(currentItem.BibID, currentItem.VID);
                                Cached_Data_Manager.Remove_All_User_Folder_Browses(currentUser.UserID, Tracer);
                                HttpContext.Current.Session.Add("ON_LOAD_MESSAGE", "Item was removed from your bookshelves.");
                            }
                            else
                            {
                                HttpContext.Current.Session.Add("ON_LOAD_MESSAGE", "ERROR encountered while trying to remove item from your bookshelves.");
                            }

                            HttpContext.Current.Response.Redirect(HttpContext.Current.Items["Original_URL"].ToString(), false);
                            HttpContext.Current.ApplicationInstance.CompleteRequest();
                            Current_Mode.Request_Completed = true;
                            return;
                        }

                        if (action.IndexOf("add_tag") == 0)
                        {
                            int tagid = -1;
                            if (action.Replace("add_tag", "").Length > 0)
                            {
                                tagid = Convert.ToInt32(action.Replace("add_tag_", ""));
                            }
                            string description = HttpContext.Current.Request.Form["add_tag"].Trim();
                            int new_tagid = SobekCM_Database.Add_Description_Tag(currentUser.UserID, tagid, currentItem.Web.ItemID, description, Tracer);
                            if (new_tagid > 0)
                            {
                                currentItem.Behaviors.Add_User_Tag(currentUser.UserID, currentUser.Full_Name, description, DateTime.Now, new_tagid);
                                currentUser.Has_Descriptive_Tags = true;
                            }

                            HttpContext.Current.Response.Redirect(HttpContext.Current.Items["Original_URL"].ToString(), false);
                            HttpContext.Current.ApplicationInstance.CompleteRequest();
                            Current_Mode.Request_Completed = true;
                            return;
                        }

                        if (action.IndexOf("delete_tag") == 0)
                        {
                            if (action.Replace("delete_tag", "").Length > 0)
                            {
                                int tagid = Convert.ToInt32(action.Replace("delete_tag_", ""));
                                if (currentItem.Behaviors.Delete_User_Tag(tagid, currentUser.UserID))
                                {
                                    SobekCM_Database.Delete_Description_Tag(tagid, Tracer);
                                }
                            }
                            HttpContext.Current.Response.Redirect(HttpContext.Current.Items["Original_URL"].ToString(), false);
                            HttpContext.Current.ApplicationInstance.CompleteRequest();
                            Current_Mode.Request_Completed = true;
                            return;
                        }
                    }
                }

                // Handle any request from the internal header for the item
                if ((HttpContext.Current != null) && (HttpContext.Current.Request.Form["internal_header_action"] != null) && ( currentUser != null ))
                {
                    // Pull the action value
                    string internalHeaderAction = HttpContext.Current.Request.Form["internal_header_action"].Trim();

                    // Was this to save the item comments?
                    if (internalHeaderAction == "save_comments")
                    {
                        string new_comments = HttpContext.Current.Request.Form["intheader_internal_notes"].Trim();
                        if ( Resource_Object.Database.SobekCM_Database.Save_Item_Internal_Comments( currentItem.Web.ItemID, new_comments)) 
                            currentItem.Tracking.Internal_Comments = new_comments;
                    }

                    // Is this to change accessibility?
                    if ((internalHeaderAction == "public") || (internalHeaderAction == "private") || (internalHeaderAction == "restricted"))
                    {
                        int current_mask = currentItem.Behaviors.IP_Restriction_Membership;
                        switch (internalHeaderAction)
                        {
                            case "public":
                                currentItem.Behaviors.IP_Restriction_Membership = 0;
                                break;

                            case "private":
                                currentItem.Behaviors.IP_Restriction_Membership = -1;
                                break;

                            case "restricted":
                                currentItem.Behaviors.IP_Restriction_Membership = 1;
                                break;
                        }

                        // Save the new visibility
                        if (currentItem.Behaviors.IP_Restriction_Membership != current_mask)
                        {
                            if (Resource_Object.Database.SobekCM_Database.Set_IP_Restriction_Mask(currentItem.Web.ItemID, currentItem.Behaviors.IP_Restriction_Membership, currentUser.UserName, String.Empty))
                            {
                                Cached_Data_Manager.Remove_Digital_Resource_Object(currentItem.BibID, currentItem.VID, Tracer);
                                Cached_Data_Manager.Store_Digital_Resource_Object(currentItem.BibID, currentItem.VID, currentItem, Tracer);
                            }
                        }
                    }
                }
            }

            // Set the code for bib level mets to show the volume tree by default
            if ((currentItem.METS_Header.RecordStatus_Enum == METS_Record_Status.BIB_LEVEL) && (currentMode.ViewerCode.Length == 0))
            {
                currentMode.ViewerCode = "allvolumes1";
            }

            // If there is a file name included, look for the sequence of that file
            if (currentMode.Page_By_FileName.Length > 0)
            {
                int page_sequence = currentItem.Divisions.Physical_Tree.Page_Sequence_By_FileName(currentMode.Page_By_FileName);
                if (page_sequence > 0)
                {
                    currentMode.ViewerCode = page_sequence.ToString();
                    currentMode.Page = (ushort) page_sequence;
                }
            }

            // Get the valid viewer code
            Tracer.Add_Trace("Html_MainWriter.Add_Controls", "Getting the appropriate item viewer");

            if ((currentMode.ViewerCode.Length == 0) && (currentMode.Coordinates.Length > 0))
            {
                currentMode.ViewerCode = "map";
            }
            currentMode.ViewerCode = currentItem.Web.Get_Valid_Viewer_Code(currentMode.ViewerCode, currentMode.Page);
            View_Object viewObject = currentItem.Web.Get_Viewer(currentMode.ViewerCode);
            PageViewer = ItemViewer_Factory.Get_Viewer(viewObject, currentItem.Bib_Info.SobekCM_Type_String.ToUpper(), currentItem, currentUser, currentMode);

            // If this was in fact restricted by IP address, restrict now
            if (itemRestrictedFromUserByIp)
            {
                if ((PageViewer.ItemViewer_Type != ItemViewer_Type_Enum.Citation) &&
                    (PageViewer.ItemViewer_Type != ItemViewer_Type_Enum.MultiVolume) &&
                    (PageViewer.ItemViewer_Type != ItemViewer_Type_Enum.Related_Images))
                {
                    PageViewer = new Restricted_ItemViewer(Item_Restricted_Message);
                    currentMode.ViewerCode = "res";
                }
            }

            // If execution should end, do it now
            if (currentMode.Request_Completed)
                return;

            Tracer.Add_Trace("Html_MainWriter.Add_Controls", "Created " + PageViewer.GetType().ToString().Replace("SobekCM.Library.ItemViewer.Viewers.", ""));

            // Assign the rest of the information, if a page viewer was created
            if (PageViewer != null)
            {
                PageViewer.CurrentItem = currentItem;
                PageViewer.CurrentMode = currentMode; 
                PageViewer.Translator = Translator;
                PageViewer.CurrentUser = currentUser;

                // Special code if this is the citation viewer
                Citation_ItemViewer viewer = PageViewer as Citation_ItemViewer;
                if (viewer != null)
                {
                    viewer.Code_Manager = Code_Manager;
                    viewer.Item_Restricted = itemRestrictedFromUserByIp;
                }

                // Special code if this is the multi-volumes viewer
                var itemViewer = PageViewer as MultiVolumes_ItemViewer;
                if (itemViewer != null)
                {
                    if (itemsInTitle == null)
                    {
                        // Look in the cache first
                        itemsInTitle = Cached_Data_Manager.Retrieve_Items_In_Title(currentItem.BibID, Tracer);

                        // If still null, try to pull from the database
                        if (itemsInTitle == null)
                        {
                            // Get list of information about this item group and save the item list
                            DataSet itemDetails = SobekCM_Database.Get_Item_Group_Details(currentItem.BibID, Tracer);
                            itemsInTitle = new SobekCM_Items_In_Title(itemDetails.Tables[1]);

                            //// Add the related titles, if there are some
                            //if ((currentGroup.Tables.Count > 3) && (currentGroup.Tables[3].Rows.Count > 0))
                            //{
                            //    foreach (DataRow thisRow in currentGroup.Tables[3].Rows)
                            //    {
                            //        string relationship = thisRow["Relationship"].ToString();
                            //        string title = thisRow["GroupTitle"].ToString();
                            //        string bibid = thisRow["BibID"].ToString();
                            //        string link_and_title = "<a href=\"" + currentMode.Base_URL + bibid + "<%URL_OPTS%>\">" + title + "</a>";
                            //        currentItem.Behaviors.All_Related_Titles.Add(new SobekCM.Resource_Object.Behaviors.Related_Titles(relationship, link_and_title));
                            //    }
                            //}

                            // Store in cache if retrieved
                            if (itemsInTitle != null)
                            {
                                Cached_Data_Manager.Store_Items_In_Title(currentItem.BibID, itemsInTitle, Tracer);
                            }
                        }
                    }

                    itemViewer.Item_List = itemsInTitle;
                }

                // Finally, perform any necessary work before display
                PageViewer.Perform_PreDisplay_Work(Tracer);

                // Get the list of any special behaviors
                behaviors = PageViewer.ItemViewer_Behaviors;
            }
            else
            {
                behaviors = new List<HtmlSubwriter_Behaviors_Enum>();
            }

            // ALways suppress the banner
            behaviors.Add(HtmlSubwriter_Behaviors_Enum.Suppress_Banner);

            //if ((searchMatchOnThisPage) && ((PageViewer.ItemViewer_Type == ItemViewer_Type_Enum.JPEG) || (PageViewer.ItemViewer_Type == ItemViewer_Type_Enum.JPEG2000)))
            //{
            //    if (PageViewer.ItemViewer_Type == ItemViewer_Type_Enum.JPEG2000)
            //    {
            //        Aware_JP2_ItemViewer jp2_viewer = (Aware_JP2_ItemViewer) PageViewer;
            //        jp2_viewer.Add_Feature("Red", "DrawEllipse", ((int) (featureXRatioLocation*jp2_viewer.Width)), ((int) (featureYRatioLocation*jp2_viewer.Height)), 800, 800);

            //    }
            //}
        }

        #endregion

        private bool should_left_navigation_bar_be_shown
        {
            get
            {
                // If this is for a fragment, do nothing
                if (!String.IsNullOrEmpty(currentMode.Fragment))
                    return false;

                if (behaviors.Contains(HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Suppress_Left_Navigation_Bar))
                    return false;

                if (behaviors.Contains(HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Requires_Left_Navigation_Bar))
                    return true;

                // If there are any icons, need to show the bar
                if (currentItem.Behaviors.Wordmark_Count > 0)
                    return true;
                
                // If the item can be described, include the quick links still
                if (currentItem.Behaviors.Can_Be_Described)
                    return true;
                
                // If a TOC could be shown for this item, need a navigation bar
                if (currentItem.Web.Static_Division_Count > 1)
                    return true;
                
                // Search results are also included in the left navigation bar
                if (searchResultsCount > 0)
                    return true;

                return false;
            }
        }

        /// <summary> Gets and sets the page viewer used to display the current item </summary>
        public abstractItemViewer PageViewer { get; set; }

        /// <summary> Flag indicates this item is currently checked out by another user </summary>
        public bool Item_Checked_Out_By_Other_User 
        {
            set                                  
            {
                // Override the page viewer at this point
                if ((value) && (PageViewer.Override_On_Checked_Out))
                {
                    PageViewer = new Checked_Out_ItemViewer();
                }
                itemCheckedOutByOtherUser = value; 
            }
            get { return itemCheckedOutByOtherUser; }
        }

        /// <summary> Gets the collection of special behaviors which this subwriter
        /// requests from the main HTML subwriter. </summary>
        /// <remarks> By default, this returns an empty list </remarks>
        public override List<HtmlSubwriter_Behaviors_Enum> Subwriter_Behaviors
        {
            get
            {
                return behaviors;
            }
        }

        #region Code to add the table of contents tree as a control into the left navigation bar

        /// <summary> Adds the table of contents as a control in the left navigation bar </summary>
        /// <param name="TOC_PlaceHolder"> TOC place holder ( &quot;tocPlaceHolder&quot; ) in the itemNavForm form, widely used throughout the application</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public void Add_Standard_TOC(PlaceHolder TOC_PlaceHolder, Custom_Tracer Tracer)
        {
            if (!should_left_navigation_bar_be_shown)
                return;

            if ((showToc) && (currentItem.Web.Static_PageCount > 1) && (currentItem.Web.Static_Division_Count > 1))
            {
                Tracer.Add_Trace("Item_HtmlSubwriter.Add_Standard_TOC", "Adding Table of Contents control to <i>TOC_PlaceHolder</i>");

                string table_of_contents = "TABLE OF CONTENTS";
                string hide_toc = "HIDE TABLE OF CONTENTS";

                if (currentMode.Language == Web_Language_Enum.French)
                {
                    table_of_contents = "TABLE DES MATIERES";
                    hide_toc = "MASQUER L'INDEX";
                }

                if (currentMode.Language == Web_Language_Enum.Spanish)
                {
                    table_of_contents = "INDICE";
                    hide_toc = "ESCONDA INDICE";
                }

                // Add the HTML to start this menu section
                Literal menuStartLiteral = new Literal();
                string redirect_url = currentMode.Redirect_URL().Replace("&","&amp;");
                if (redirect_url.IndexOf("?") < 0)
                    redirect_url = redirect_url + "?toc=n";
                else
                    redirect_url = redirect_url + "&toc=n";

                
                menuStartLiteral.Text = string.Format("        <div class=\"sbkIsw_ShowTocRow\">" + Environment.NewLine +
                    "          <a href=\"{1}\"><div class=\"sbkIsw_UpToc\">{4}<img src=\"" + currentMode.Base_URL + "default/images/button_up_arrow.png\" alt=\"\" /></div></a>" + Environment.NewLine + 
                    "        </div>", table_of_contents, redirect_url, currentMode.Base_URL, htmlSkin.Base_Skin_Code, hide_toc);
                TOC_PlaceHolder.Controls.Add(menuStartLiteral);

                // Create the treeview
                treeView1 = new TreeView { CssClass = "sbkIsw_TocTreeView", ExpandDepth = 0, NodeIndent = 15 };
                treeView1.SelectedNodeChanged += treeView1_SelectedNodeChanged;

                // load the table of contents in the tree
                Create_TreeView_From_Divisions(treeView1);

                // Add the tree view to the placeholder
                TOC_PlaceHolder.Controls.Add(treeView1);
            }
            else
            {
                Tracer.Add_Trace("Item_HtmlSubwriter.Add_TOC", "Table of contents is currently hidden");
            }
        }

        /// <summary> Event is fired if the currently selected tree view node changes. </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void treeView1_SelectedNodeChanged(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                string currentNodeID = treeView1.SelectedNode.Value;
                if ((currentNodeID.Length > 0) && (Convert.ToInt32(currentNodeID) > 0))
                {
                    currentMode.Page = Convert.ToUInt16(currentNodeID);
                    currentMode.ViewerCode = currentNodeID;
                }
                else
                {
                    currentMode.Page = 0;
                    currentMode.ViewerCode = "1";
                }

                // Redirect
                if (HttpContext.Current != null)
                    currentMode.Redirect();
            }
        }

        #endregion

        /// <summary> Adds the internal header HTML for this specific HTML writer </summary>
        /// <param name="Output"> Stream to which to write the HTML for the internal header information </param>
        /// <param name="Current_User"> Currently logged on user, to determine specific rights </param>
        public override void Write_Internal_Header_HTML(TextWriter Output, User_Object Current_User)
        {
            // If this is for a fragment, do nothing
            if (!String.IsNullOrEmpty(currentMode.Fragment))
                return;

            string currentViewerCode = currentMode.ViewerCode;

            Output.WriteLine("  <table id=\"sbkIsw_Internalheader\">");
            Output.WriteLine("    <tr style=\"height:30px;\">");
            Output.WriteLine("      <td style=\"text-align:left\">");
            Output.WriteLine("          <button title=\"Hide Internal Header\" class=\"sbkIsw_intheader_button hide_intheader_button2\" onclick=\"return hide_internal_header();\"></button>");
            Output.WriteLine("      </td>");
            if (currentItem.METS_Header.RecordStatus_Enum == METS_Record_Status.BIB_LEVEL)
            {
                Output.WriteLine("      <td style=\"text-align:center;\"><h2>" + currentItem.BibID + "</h2></td>");
            }
            else
            {
                Output.WriteLine("      <td style=\"text-align:center;\"><h2><a href=\"" + currentMode.Base_URL + currentItem.BibID + "/00000\">" + currentItem.BibID + "</a> : " + currentItem.VID + "</h2></td>");
            }

            Write_Internal_Header_Search_Box(Output);
            Output.WriteLine("    </tr>");

            if (currentItem.METS_Header.RecordStatus_Enum != METS_Record_Status.BIB_LEVEL)
            {
                Output.WriteLine("    <tr style=\"height:40px;\">");
                Output.WriteLine("      <td colspan=\"3\" style=\"text-align:center;vertical-align:middle;\">");

                // Should we add ability to edit this item to the quick links?
                bool allow_access_change = false;
                if (userCanEditItem)
                {
                    // Add ability to edit metadata for this item
                    currentMode.Mode = Display_Mode_Enum.My_Sobek;
                    currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Edit_Item_Metadata;
                    currentMode.My_Sobek_SubMode = "1";
                    Output.WriteLine("          <button title=\"Edit Metadata\" class=\"sbkIsw_intheader_button edit_metadata_button\" onclick=\"window.location.href='" + currentMode.Redirect_URL() + "';return false;\"></button>");

                    // Add ability to edit behaviors for this item
                    currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Edit_Item_Behaviors;
                    currentMode.My_Sobek_SubMode = "1";
                    Output.WriteLine("          <button title=\"Edit Behaviors\" class=\"sbkIsw_intheader_button edit_behaviors_button\" onclick=\"window.location.href='" + currentMode.Redirect_URL() + "';return false;\"></button>");
                    currentMode.Mode = Display_Mode_Enum.Item_Display;


                    // Add ability to edit behaviors for this item
                    if (currentItem.Web.Static_PageCount == 0)
                    {
                        currentMode.Mode = Display_Mode_Enum.My_Sobek;
                        currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Page_Images_Management;
                        Output.WriteLine("          <button title=\"Perform Quality Control\" class=\"sbkIsw_intheader_button qualitycontrol_button\" onclick=\"window.location.href='" + currentMode.Redirect_URL() + "';return false;\"></button>");
                        currentMode.Mode = Display_Mode_Enum.Item_Display;
                    }
                    else
                    {
                        currentMode.ViewerCode = "qc";
                        Output.WriteLine("          <button title=\"Perform Quality Control\" class=\"sbkIsw_intheader_button qualitycontrol_button\" onclick=\"window.location.href='" + currentMode.Redirect_URL() + "';return false;\"></button>");
					}

                    // Check if this item is DARK first
                    if (currentItem.Behaviors.Dark_Flag)
                    {
                        Output.WriteLine("          <button title=\"Dark Resource\" class=\"sbkIsw_intheader_button dark_resource_button_fixed\" onclick=\"return false;\"></button>");
                    }
                    else
                    {
                        // If the item is currently PUBLIC, only internal or system admins can reset back to PRIVATE
                        if (currentItem.Behaviors.IP_Restriction_Membership >= 0)
                        {
                            if ((currentUser.Is_Internal_User) || (currentUser.Is_System_Admin))
                            {
                                allow_access_change = true;
                                Output.WriteLine(currentItem.Behaviors.IP_Restriction_Membership == 0
                                                     ? "          <button title=\"Change Access Restriction\" class=\"sbkIsw_intheader_button public_resource_button\" onclick=\"open_access_restrictions(); return false;\"></button>"
                                                     : "          <button title=\"Change Access Restriction\" class=\"sbkIsw_intheader_button restricted_resource_button\" onclick=\"open_access_restrictions(); return false;\"></button>");
                            }
                            else
                            {
                                Output.WriteLine(currentItem.Behaviors.IP_Restriction_Membership == 0
                                                     ? "          <button title=\"Public Resource\" class=\"sbkIsw_intheader_button public_resource_button_fixed\" onclick=\"return false;\"></button>"
                                                     : "          <button title=\"IP Restriced Resource\" class=\"sbkIsw_intheader_button restricted_resource_button_fixed\" onclick=\"return false;\"></button>");
                            }
                        }
                        else
                        {
                            allow_access_change = true;
                            Output.WriteLine("          <button title=\"Change Access Restriction\" class=\"sbkIsw_intheader_button private_resource_button\" onclick=\"open_access_restrictions(); return false;\"></button>");
                        }
                    }
                }
                else
                {
                    // Check if this item is DARK first
                    if (currentItem.Behaviors.Dark_Flag)
                    {
                        Output.WriteLine("          <button title=\"Dark Resource\" class=\"sbkIsw_intheader_button dark_resource_button_fixed\" onclick=\"return false;\"></button>");
                    }
                    else
                    {
                        // Still show that the item is public, private, restricted
                        if (currentItem.Behaviors.IP_Restriction_Membership > 0)
                        {
                            Output.WriteLine("          <button title=\"IP Restriced Resource\" class=\"sbkIsw_intheader_button restricted_resource_button_fixed\" onclick=\"return false;\"></button>");
                        }
                        if (currentItem.Behaviors.IP_Restriction_Membership == 0)
                        {
                            Output.WriteLine("          <button title=\"Public Resource\" class=\"sbkIsw_intheader_button public_resource_button_fixed\" onclick=\"return false;\"></button>");
                        }
                        if (currentItem.Behaviors.IP_Restriction_Membership < 0)
                        {
                            Output.WriteLine("          <button title=\"Private Resource\" class=\"sbkIsw_intheader_button private_resource_button_fixed\" onclick=\"return false;\"></button>");
                        }
                    }
                }

                currentMode.ViewerCode = "milestones";
                Output.WriteLine("          <button title=\"View Work Log\" class=\"sbkIsw_intheader_button view_worklog_button\" onclick=\"window.location.href='" + currentMode.Redirect_URL() + "';return false;\"></button>");
                currentMode.ViewerCode = currentViewerCode;

                // Add ability to edit behaviors for this item
                if (userCanEditItem)
                {
                    currentMode.Mode = Display_Mode_Enum.My_Sobek;
                    currentMode.My_Sobek_Type = My_Sobek_Type_Enum.File_Management;
                    Output.WriteLine("          <button title=\"Manage Files\" class=\"sbkIsw_intheader_button manage_files_button\" onclick=\"window.location.href='" + currentMode.Redirect_URL() + "';return false;\"></button>");
                    currentMode.Mode = Display_Mode_Enum.Item_Display;
                }

                // Add the HELP icon next
                Output.WriteLine("<span class=\"intheader_help\"><a href=\"" + SobekCM_Library_Settings.Help_URL(currentMode.Base_URL) + "help/itemheader\" title=\"Help regarding this header\"><img src=\"" + currentMode.Base_URL + "default/images/help_button_darkgray.jpg\" alt=\"?\" title=\"Help regarding this header\" /></a></span>");

                Output.WriteLine("      </td>");
                Output.WriteLine("    </tr>");

                // Display the comments and allow change?
                if ((userCanEditItem) || (currentUser.Is_Internal_User) || (currentUser.Is_System_Admin))
                {
                    const int rows = 1;
                    const int actualCols = 70;

                    // Add the internal comments row ( hidden content initially )
                    Output.WriteLine("    <tr style=\"text-align:center; height:14px;\">");
                    Output.WriteLine("      <td colspan=\"3\">");
                    Output.WriteLine("        <table id=\"internal_notes_div\">");
                    Output.WriteLine("          <tr style=\"text-align:left; height:14px;\">");
                    Output.WriteLine("            <td class=\"intheader_label\">COMMENTS:</td>");
                    Output.WriteLine("            <td>");
                    Output.WriteLine("              <textarea rows=\"" + rows + "\" cols=\"" + actualCols + "\" name=\"intheader_internal_notes\" id=\"intheader_internal_notes\" class=\"intheader_comments_input\" onfocus=\"javascript:textbox_enter('intheader_internal_notes','intheader_comments_input_focused')\" onblur=\"javascript:textbox_leave('intheader_internal_notes','intheader_comments_input')\">" + HttpUtility.HtmlEncode(currentItem.Tracking.Internal_Comments) + "</textarea>");
                    Output.WriteLine("            </td>");
                    Output.WriteLine("            <td>");
                    Output.WriteLine("              <button title=\"Save new internal comments\" class=\"internalheader_button\" onclick=\"save_internal_notes(); return false;\">SAVE</button>");
                    Output.WriteLine("            </td>");
                    Output.WriteLine("          </tr>");
                    Output.WriteLine("        </table>");
                    Output.WriteLine("      </td>");
                    Output.WriteLine("    </tr>");
                }
                else
                {
                    const int rows = 1;
                    const int actualCols = 80;

                    // Add the internal comments row ( hidden content initially )
                    Output.WriteLine("    <tr style=\"text-align:center; height:14px;\">");
                    Output.WriteLine("      <td colspan=\"2\">");
                    Output.WriteLine("        <table id=\"internal_notes_div\">");
                    Output.WriteLine("          <tr style=\"text-align:left; height:14px;\">");
                    Output.WriteLine("            <td class=\"intheader_label\">COMMENTS:</td>");
                    Output.WriteLine("            <td>");
                    Output.WriteLine("              <textarea readonly=\"readonly\" rows=\"" + rows + "\" cols=\"" + actualCols + "\" name=\"intheader_internal_notes\" id=\"intheader_internal_notes\" class=\"intheader_comments_input\" onfocus=\"javascript:textbox_enter('intheader_internal_notes','intheader_comments_input_focused')\" onblur=\"javascript:textbox_leave('intheader_internal_notes','intheader_comments_input')\">" + HttpUtility.HtmlEncode(currentItem.Tracking.Internal_Comments) + "</textarea>");
                    Output.WriteLine("            </td>");
                    Output.WriteLine("          </tr>");
                    Output.WriteLine("        </table>");
                    Output.WriteLine("      </td>");
                    Output.WriteLine("    </tr>");
                }

          
                if (allow_access_change)
                {
                    // Add the access restriction row ( hidden content initially )
                    Output.WriteLine("    <tr style=\"text-align:center;\">");
                    Output.WriteLine("      <td colspan=\"3\">");
                    Output.WriteLine("        <table id=\"access_restrictions_div\" style=\"display:none;\">");
                    Output.WriteLine("          <tr style=\"text-align:left;\">");
                    Output.WriteLine("            <td style=\"vertical-align:top\" class=\"intheader_label\">SET ACCESS RESTRICTIONS: </td>");
                    Output.WriteLine("            <td>");
                    Output.WriteLine("              <button title=\"Make item public\" class=\"sbkIsw_intheader_button public_resource_button\" onclick=\"set_item_access('public'); return false;\"></button>");
                    Output.WriteLine("              <button title=\"Add IP restriction to this item\" class=\"sbkIsw_intheader_button restricted_resource_button\" onclick=\"set_item_access('restricted'); return false;\"></button>");
                    Output.WriteLine("              <button title=\"Make item private\" class=\"sbkIsw_intheader_button private_resource_button\" onclick=\"set_item_access('private'); return false;\"></button>");

                    // Should we add ability to delete this item?
                    if (currentUser.Can_Delete_This_Item(currentItem))
                    {
                        // Determine the delete URL
                        currentMode.Mode = Display_Mode_Enum.My_Sobek;
                        currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Delete_Item;
                        string delete_url = currentMode.Redirect_URL();
                        currentMode.Mode = Display_Mode_Enum.Item_Display;
                        Output.WriteLine("              <button title=\"Delete this item\" class=\"sbkIsw_intheader_button delete_button\" onclick=\"if(confirm('Delete this item completely?')) window.location.href = '" + delete_url + "'; return false;\"></button>");
                    }

                    Output.WriteLine("            </td>");
                    Output.WriteLine("            <td style=\"vertical-align:top\">");
                    Output.WriteLine("              <button title=\"Cancel changes\" class=\"internalheader_button\" onclick=\"open_access_restrictions(); return false;\">CANCEL</button>");
                    Output.WriteLine("            </td>");
                    Output.WriteLine("          </tr>");
                    Output.WriteLine("        </table>");
                    Output.WriteLine("      </td>");
                    Output.WriteLine("    </tr>");
                }
            }
            else
            {

                if (userCanEditItem)
                {
                    Output.WriteLine("    <tr style=\"height:45px;\">");
                    Output.WriteLine("      <td colspan=\"3\" style=\"text-align:center;vertical-align:middle;\">");

                    // Add ability to edit behaviors for this item group
                    currentMode.Mode = Display_Mode_Enum.My_Sobek;
                    currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Edit_Group_Behaviors;
                    currentMode.My_Sobek_SubMode = "1";
                    Output.WriteLine("          <button title=\"Edit Behaviors\" class=\"sbkIsw_intheader_button edit_behaviors_button\" onclick=\"window.location.href='" + currentMode.Redirect_URL() + "';return false;\"></button>");

                    // Add ability to add a new item/volume to this title
                    currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Group_Add_Volume;
                    Output.WriteLine("          <button title=\"Add Volume\" class=\"sbkIsw_intheader_button add_volume_button\" onclick=\"window.location.href='" + currentMode.Redirect_URL() + "';return false;\"></button>");

                    // Add ability to auto-fill a number of new items/volumes to this title
                    currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Group_AutoFill_Volumes;
                    Output.WriteLine("          <button title=\"Auto-Fill Volumes\" class=\"sbkIsw_intheader_button autofill_volumes_button\" onclick=\"window.location.href='" + currentMode.Redirect_URL() + "';return false;\"></button>");

                    // Add ability to edit the serial hierarchy online
                    currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Edit_Group_Serial_Hierarchy;
                    Output.WriteLine("          <button title=\"Edit Serial Hierarchy\" class=\"sbkIsw_intheader_button serial_hierarchy_button\" onclick=\"window.location.href='" + currentMode.Redirect_URL() + "';return false;\"></button>");

                    // Add ability to mass update the items behaviors under this title
                    currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Group_Mass_Update_Items;
                    Output.WriteLine("          <button title=\"Mass Update Volumes\" class=\"sbkIsw_intheader_button mass_update_button\" onclick=\"window.location.href='" + currentMode.Redirect_URL() + "';return false;\"></button>");

                    Output.WriteLine("      </td>");
                    Output.WriteLine("    </tr>");
                }

            }

            Output.WriteLine("  </table>");

            currentMode.Mode = Display_Mode_Enum.Item_Display;
            currentMode.ViewerCode = currentViewerCode;
        }

        //public void Set_Text_Language(Application_State.Language_Enum Language)
        //{
        //    switch (Language)
        //    {
        //        case Application_State.Language_Enum.French:
        //            search = "RECHERCHE";
        //            search_doc = "Recherche Ce Document";
        //            view = "VUE";
        //            full_citation = "Notice"; // "Citation Complètes";
        //            browse_images = "Revue des Images";
        //            view_image = "Revue l'Image";
        //            browse_text = "Revue la Texte";
        //            language = "LANGUE";
        //            english = "Anglais";
        //            french = "Français";
        //            spanish = "Espagñol";
        //            download = "TÉLÉCHARGEMENT";
        //            help = "AIDE";
        //            using_site = "Navigation";
        //            contact = "Assistance";
        //            contents = "TABLE DES MATIÈRES";
        //            break;

        //        case Application_State.Language_Enum.Spanish:
        //            search = "BUSCAR";
        //            search_all = "Busque Todas las Colecciones";
        //            search_this = "Busque Esta Colección";
        //            last_search = "Resultados Anteriores";
        //            search_doc = "Busque Este Documento";
        //            view = "VER";
        //            full_citation = "Cita Completa";
        //            browse_images = "Navegar Imagenes";
        //            view_image = "Ver Imagen";
        //            browse_text = "Navegar Texto";
        //            language = "IDIOMA";
        //            english = "Inglés";
        //            french = "Francés";
        //            spanish = "Español";
        //            download = "TRANSFERENCIA DIRECTA";
        //            help = "AYUDA";
        //            using_site = "Usando este sitio";
        //            contact = "Contacto";
        //            contents = "INDICE";
        //            break;
        //    }
        //}

        /// <summary> Writes the HTML generated by this item html subwriter directly to the response stream </summary>
        /// <param name="Output"> Stream to which to write the HTML for this subwriter </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Value indicating if html writer should finish the page immediately after this, or if there are other controls or routines which need to be called first </returns>
        /// <remarks> This begins writing this page, up to the item-level main menu</remarks>
        public override bool Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Item_HtmlSubwriter.Write_HTML", "Begin writing the item viewer, up to the item-level main menu");


            // If this is for a fragment, do nothing
            if (!String.IsNullOrEmpty(currentMode.Fragment))
                return false;


            Output.WriteLine();




            if (PageViewer.ItemViewer_Type != ItemViewer_Type_Enum.GnuBooks_PageTurner)
            {
                Output.WriteLine("<!-- Show the title and any other important item information -->");
                Output.WriteLine("<div id=\"sbkIsw_Titlebar\">");
                if (currentItem.METS_Header.RecordStatus_Enum == METS_Record_Status.BIB_LEVEL)
                {
                    string grouptitle = currentItem.Behaviors.GroupTitle;
                    if (grouptitle.Length > 125)
                    {
                        Output.WriteLine("\t<h1><abbr title=\"" + grouptitle + "\">" + grouptitle.Substring(0, 120) + "...</abbr></h1>");
                    }
                    else
                    {
                        Output.WriteLine("\t<h1>" + grouptitle + "</h1>");
                    }
                }
                else
                {
                    string final_title = currentItem.Bib_Info.Main_Title.Title;
                    if (currentItem.Bib_Info.Main_Title.NonSort.Length > 0)
                    {
                        if (currentItem.Bib_Info.Main_Title.NonSort[currentItem.Bib_Info.Main_Title.NonSort.Length - 1] == ' ')
                            final_title = currentItem.Bib_Info.Main_Title.NonSort + currentItem.Bib_Info.Main_Title.Title;
                        else
                        {
                            if (currentItem.Bib_Info.Main_Title.NonSort[currentItem.Bib_Info.Main_Title.NonSort.Length - 1] == '\'')
                            {
                                final_title = currentItem.Bib_Info.Main_Title.NonSort + currentItem.Bib_Info.Main_Title.Title;
                            }
                            else
                            {
                                final_title = currentItem.Bib_Info.Main_Title.NonSort + " " + currentItem.Bib_Info.Main_Title.Title;
                            }
                        }
                    }


                    // Add the Title if there is one
                    if (final_title.Length > 0)
                    {
                        // Is this a newspaper?
                        bool newspaper = currentItem.Behaviors.GroupType.ToUpper() == "NEWSPAPER";
                        if ((newspaper) && ((currentItem.Bib_Info.Origin_Info.Date_Created.Length > 0) || (currentItem.Bib_Info.Origin_Info.Date_Issued.Length > 0)))
                        {
                            string date = currentItem.Bib_Info.Origin_Info.Date_Created;
                            if (currentItem.Bib_Info.Origin_Info.Date_Created.Length == 0)
                                date = currentItem.Bib_Info.Origin_Info.Date_Issued;


                            if (final_title.Length > 125)
                            {
                                Output.WriteLine("\t<h1><abbr title=\"" + final_title + "\">" + final_title.Substring(0, 120) + "...</abbr> ( " + date + " )</h1>");
                            }
                            else
                            {
                                Output.WriteLine("\t<h1>" + final_title + " ( " + date + " )</h1>");
                            }
                        }
                        else
                        {
                            if (final_title.Length > 125)
                            {
                                Output.WriteLine("\t<h1><abbr title=\"" + final_title + "\">" + final_title.Substring(0, 120) + "...</abbr></h1>");
                            }
                            else
                            {
                                Output.WriteLine("\t<h1>" + final_title + "</h1>");
                            }
                        }
                    }


                    // Add the link if there is one
                    if ((currentItem.Bib_Info.hasLocationInformation) && (currentItem.Bib_Info.Location.Other_URL.Length > 0))
                    {
                        if (currentItem.Bib_Info.Location.Other_URL.ToLower().IndexOf("www.youtube.com") < 0)
                        {


                            // Determine the type of link
                            string type = translations.Get_Translation("Related Link", currentMode.Language);
                            if (currentItem.Bib_Info.Location.Other_URL_Display_Label.Length > 0)
                            {
                                type = translations.Get_Translation(currentItem.Bib_Info.Location.Other_URL_Display_Label, currentMode.Language);
                            }


                            // Determine the display value
                            string note = currentItem.Bib_Info.Location.Other_URL;
                            if (currentItem.Bib_Info.Location.Other_URL_Note.Length > 0)
                            {
                                note = currentItem.Bib_Info.Location.Other_URL_Note;
                            }


                            // Add the link
                            Output.WriteLine("\t<a href=\"" + currentItem.Bib_Info.Location.Other_URL + "\">" + note + " ( " + type + " )</a><br />");
                        }
                    }


                    // If there is an ACCESSION number and this is an ARTIFACT, include that at the top
                    if ((currentItem.Bib_Info.SobekCM_Type == TypeOfResource_SobekCM_Enum.Artifact) && (currentItem.Bib_Info.Identifiers_Count > 0))
                    {
                        foreach (Identifier_Info thisIdentifier in currentItem.Bib_Info.Identifiers)
                        {
                            if (thisIdentifier.Type.ToUpper().IndexOf("ACCESSION") >= 0)
                            {
                                Output.WriteLine("\t" + translations.Get_Translation("Accession number", currentMode.Language) + " " + thisIdentifier.Identifier + "<br />");
                                break;
                            }
                        }
                    }
                }


                Output.WriteLine("</div>");
                Output.WriteLine();


                // The item viewer can choose to override the standard item menu
                if (!behaviors.Contains(HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Suppress_Item_Menu))
                {
					// Can this user (if there is one) edit this item?
	                bool canManage = (currentUser != null) && (currentUser.Can_Edit_This_Item(currentItem));


	                // Add the item views
                    Output.WriteLine("<!-- Add the different view and social options -->");
                    Output.WriteLine("<div id=\"sf-menubar\">");


                    // Add the sharing buttons if this is not restricted by IP address or checked out
                    if ((!itemRestrictedFromUserByIp) && (!itemCheckedOutByOtherUser) && (!currentMode.Is_Robot))
                    {
                        Output.WriteLine("<div id=\"menu-right-actions\">");
                        Output.WriteLine(
                            "\t\t<span id=\"sharebuttonitem\" class=\"action-sf-menu-item\" onclick=\"toggle_share_form('share_button');\"><span id=\"sharebuttonspan\">Share</span></span>");




                        //if (currentItem.Behaviors.Can_Be_Described)
                        //{
                        //    if (currentUser != null)
                        //    {
                        //        Output.Write("<a href=\"?m=hmh\" onmouseover=\"document.getElementById('describe_button').src='" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/describe_rect_button_h.gif'\" onmouseout=\"document.getElementById('describe_button').src='" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/describe_rect_button.gif'\"  onclick=\"return describe_item_form_open( 'describe_button' );\"><img class=\"ResultSavePrintButtons\" border=\"0px\" name=\"describe_button\" id=\"describe_button\" src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/describe_rect_button.gif\" title=\"Add a description to this item\" alt=\"DESCRIBE\" /></a>");
                        //    }
                        //    else
                        //    {
                        //        Output.Write("<a href=\"?m=hmh\" onmouseover=\"document.getElementById('describe_button').src='" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/describe_rect_button_h.gif'\" onmouseout=\"document.getElementById('describe_button').src='" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/describe_rect_button.gif'\"><img class=\"ResultSavePrintButtons\" border=\"0px\" name=\"describe_button\" id=\"describe_button\" src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/describe_rect_button.gif\" title=\"Add a description to this item\" alt=\"DESCRIBE\" /></a>");
                        //    }
                        //}

	                    string add_text = "Add";
	                    string remove_text = "Remove";
	                    string send_text = "Send";
	                    string print_text = "Print";
						if (canManage)
						{
							add_text = String.Empty;
							remove_text = String.Empty;
							send_text = String.Empty;
							print_text = String.Empty;
						}


                        if ((currentUser != null))
                        {
                            if (currentItem.Web.ItemID > 0)
                            {
                                if (currentUser.Is_In_Bookshelf(currentItem.BibID, currentItem.VID))
                                {
                                    Output.WriteLine("\t\t<span id=\"addbuttonitem\" class=\"action-sf-menu-item\" onclick=\"return remove_item_itemviewer();\"><img src=\"" + currentMode.Base_URL + "default/images/minussign.png\" alt=\"\" style=\"vertical-align:middle\" /><span id=\"addbuttonspan\">" + remove_text + "</span></span>");
                                }
                                else
                                {
                                    Output.WriteLine("\t\t<span id=\"addbuttonitem\" class=\"action-sf-menu-item\" onclick=\"add_item_form_open();\"><img src=\"" + currentMode.Base_URL + "default/images/plussign.png\" alt=\"\" style=\"vertical-align:middle\" /><span id=\"addbuttonspan\">" + add_text + "</span></span>");
                                }
                            }


                            Output.WriteLine("\t\t<span id=\"sendbuttonitem\" class=\"action-sf-menu-item\" onclick=\"email_form_open();\"><img src=\"" + currentMode.Base_URL + "default/images/email.png\" alt=\"\" style=\"vertical-align:middle\" /><span id=\"sendbuttonspan\">" + send_text + "</span></span>");
                        }
                        else
                        {
                            if (currentItem.Web.ItemID > 0)
                                Output.WriteLine("\t\t<span id=\"addbuttonitem\" class=\"action-sf-menu-item\" onclick=\"window.location='?m=hmh';\"><img src=\"" + currentMode.Base_URL + "default/images/plussign.png\" alt=\"\" style=\"vertical-align:middle\" /><span id=\"addbuttonspan\">" + add_text + "</span></span>");


                            Output.WriteLine("\t\t<span id=\"sendbuttonitem\" class=\"action-sf-menu-item\" onclick=\"window.location='?m=hmh';\"><img src=\"" + currentMode.Base_URL + "default/images/email.png\" alt=\"\" style=\"vertical-align:middle\" /><span id=\"sendbuttonspan\">" + send_text + "</span></span>");
                        }


                        if (currentItem.Web.ItemID > 0)
                        {
                            Output.WriteLine("\t\t<span id=\"printbuttonitem\" class=\"action-sf-menu-item\" onclick=\"print_form_open();\"><img src=\"" + currentMode.Base_URL + "default/images/printer.png\" alt=\"\" style=\"vertical-align:middle\" /><span id=\"printbuttonspan\">" + print_text + "</span></span>");
                        }
                        else
                        {
							Output.WriteLine("\t\t<span id=\"printbuttonitem\" class=\"action-sf-menu-item\" onclick=\"window.print();return false;\"><img src=\"" + currentMode.Base_URL + "default/images/printer.png\" alt=\"\" style=\"vertical-align:middle\" /><span id=\"printbuttonspan\">" + print_text + "</span></span>");
                        }


                        Output.WriteLine("</div>");
                    }
                    Output.WriteLine("<ul class=\"sf-menu\">");


                    // Save the current view type
                    ushort page = currentMode.Page;
	                string viewerCode = currentMode.ViewerCode;


                    // Add the item level views
                    foreach (View_Object thisView in currentItem.Behaviors.Views)
                    {
                        if (((!itemRestrictedFromUserByIp) && (!currentItem.Behaviors.Dark_Flag)) || (thisView.View_Type == View_Enum.CITATION) ||
                            (thisView.View_Type == View_Enum.ALL_VOLUMES) ||
                            (thisView.View_Type == View_Enum.RELATED_IMAGES))
                        {
                            // Special code for the CITATION view (TEMPORARY - v.3.2)
                            if (thisView.View_Type == View_Enum.CITATION)
                            {
                                currentMode.ViewerCode = "citation";
                                if (currentItem.Bib_Info.SobekCM_Type == TypeOfResource_SobekCM_Enum.EAD)
                                    currentMode.ViewerCode = "description";
                                if ((viewerCode == "citation") || (viewerCode == "marc") || (viewerCode == "metadata") ||
                                    (viewerCode == "usage") || (viewerCode == "description"))
                                {
                                    Output.Write("\t\t<li id=\"selected-sf-menu-item-link\"><a href=\"" + currentMode.Redirect_URL() + "\">Description</a>");
                                }
                                else
                                {
                                    Output.Write("\t\t<li><a href=\"" + currentMode.Redirect_URL() + "\">Description</a>");
                                }
                                Output.WriteLine("<ul>");


                                if (currentItem.Bib_Info.SobekCM_Type == TypeOfResource_SobekCM_Enum.EAD)
                                    Output.WriteLine("\t\t\t<li><a href=\"" + currentMode.Redirect_URL() + "\">Archival Description</a></li>");
                                else
                                    Output.WriteLine("\t\t\t<li><a href=\"" + currentMode.Redirect_URL() + "\">Standard View</a></li>");



                                currentMode.ViewerCode = "marc";
                                Output.WriteLine("\t\t\t<li><a href=\"" + currentMode.Redirect_URL() + "\">MARC View</a></li>");


                                currentMode.ViewerCode = "metadata";
                                Output.WriteLine("\t\t\t<li><a href=\"" + currentMode.Redirect_URL() + "\">Metadata</a></li>");


                                currentMode.ViewerCode = "usage";
                                Output.WriteLine("\t\t\t<li><a href=\"" + currentMode.Redirect_URL() + "\">Usage Statistics</a></li>");


                                Output.WriteLine("\t\t</ul></li>");
                                currentMode.ViewerCode = viewerCode;
                            }
                            else if (thisView.View_Type == View_Enum.ALL_VOLUMES)
                            {
                                string resource_type_upper = currentItem.Bib_Info.SobekCM_Type_String.ToUpper();
                                string all_volumes = "All Volumes";
                                if (resource_type_upper.IndexOf("NEWSPAPER") >= 0)
                                {
                                    all_volumes = "All Issues";
                                }
                                else if (resource_type_upper.IndexOf("MAP") >= 0)
                                {
                                    all_volumes = "Related Maps";
                                }
                                else if (resource_type_upper.IndexOf("AERIAL") >= 0)
                                {
                                    all_volumes = "Related Flights";
                                }


                                currentMode.ViewerCode = "allvolumes";
                                if ((viewerCode == "allvolumes") || (viewerCode == "allvolumes2") ||
                                    (viewerCode == "allvolumes3"))
                                {
                                    Output.Write("\t\t<li id=\"selected-sf-menu-item-link\"><a href=\"" + currentMode.Redirect_URL() + "\">" + all_volumes + "</a>");
                                }
                                else
                                {
                                    Output.Write("\t\t<li><a href=\"" + currentMode.Redirect_URL() + "\">" + all_volumes + "</a>");
                                }
                                Output.WriteLine("<ul>");


                                currentMode.ViewerCode = "allvolumes";
                                Output.WriteLine("\t\t\t<li><a href=\"" + currentMode.Redirect_URL() + "\">Tree View</a></li>");




                                currentMode.ViewerCode = "allvolumes2";
                                Output.WriteLine("\t\t\t<li><a href=\"" + currentMode.Redirect_URL() + "\">Thumbnails</a></li>");


                                if (currentMode.Internal_User)
                                {
                                    currentMode.ViewerCode = "allvolumes3";
                                    Output.WriteLine("\t\t\t<li><a href=\"" + currentMode.Redirect_URL() + "\">List View</a></li>");
                                }


                                Output.WriteLine("\t\t</ul></li>");
                                currentMode.ViewerCode = viewerCode;
                            }
                            else
                            {
                                List<string> item_nav_bar_link = Item_Nav_Bar_HTML_Factory.Get_Nav_Bar_HTML(thisView, currentItem.Bib_Info.SobekCM_Type_String, htmlSkin.Base_Skin_Code, currentMode, -1, translations, showZoomable, currentItem);


                                // Add each nav bar link
                                foreach (string this_link in item_nav_bar_link)
                                {
                                    Output.WriteLine("\t\t" + this_link + "");
                                }
                            }
                        }
                    }


                    // If this is citation or index mode, the number may be an invalid page sequence
                    if ((page <= 0) ||
                        (currentMode.ViewerCode == View_Object.Viewer_Code_By_Type(View_Enum.RELATED_IMAGES)[0]))
                    {
                        currentMode.Page = 1;


                    }


                    if ((currentItem.Web.Static_PageCount > 0) && (currentPage == null))
                    {
                        currentPage = currentItem.Web.Pages_By_Sequence[0];
                    }


                    // Add each page display type
                    if ((currentPage != null) && (!itemRestrictedFromUserByIp))
                    {
                        int page_seq = currentMode.Page;
                        string resourceType = currentItem.Bib_Info.SobekCM_Type_String.ToUpper();
                        if (currentItem.Behaviors.Item_Level_Page_Views_Count > 0)
                        {
                            List<string> pageViewLinks = new List<string>();

                            foreach (View_Object thisPageView in currentItem.Behaviors.Item_Level_Page_Views)
                            {
                                View_Enum thisViewType = thisPageView.View_Type;
                                foreach (SobekCM_File_Info thisFile in currentPage.Files)
                                {
                                    View_Object fileObject = thisFile.Get_Viewer();
                                    if ((fileObject != null) && (fileObject.View_Type == thisViewType))
                                    {
                                        pageViewLinks.AddRange(Item_Nav_Bar_HTML_Factory.Get_Nav_Bar_HTML(thisFile.Get_Viewer(), resourceType, htmlSkin.Base_Skin_Code, currentMode, page_seq, translations, showZoomable, currentItem));
                                    }
                                }
                            }


                            if (currentItem.BibID == "UF00001672")
                            {
                                string filename = currentPage.Files[0].File_Name_Sans_Extension + ".txt";
                                SobekCM_File_Info newFile = new SobekCM_File_Info(filename);
                                pageViewLinks.AddRange(Item_Nav_Bar_HTML_Factory.Get_Nav_Bar_HTML(newFile.Get_Viewer(), resourceType, htmlSkin.Base_Skin_Code, currentMode, page_seq, translations, showZoomable, currentItem));


                            }




                            // Only continue if there were views
                            if (pageViewLinks.Count > 0)
                            {
                                // Determine the name for this menu item
                                string menu_title = "Page Image";
                                if (resourceType.IndexOf("MAP") >= 0)
                                {
                                    menu_title = "Map Image";
                                }
                                else if ((resourceType.IndexOf("AERIAL") >= 0) || (resourceType.IndexOf("PHOTOGRAPH") >= 0))
                                {
                                    menu_title = "Image";
                                }
                                if (currentItem.Web.Static_PageCount > 1)
                                    menu_title = menu_title + "s";


                                // Get the link for the first page view
                                string link = pageViewLinks[0].Substring(pageViewLinks[0].IndexOf("href=\"") + 6);
                                link = link.Substring(0, link.IndexOf("\""));


                                // Was this a match?
                                if ((currentMode.ViewerCode == page_seq + "t") || (currentMode.ViewerCode == page_seq + "x") || (currentMode.ViewerCode == page_seq + "j"))
                                {
                                    Output.Write("\t\t<li id=\"selected-sf-menu-item-link\"><a href=\"" + link + "\">" + menu_title + "</a>");
                                }
                                else
                                {
                                    Output.Write("\t\t<li><a href=\"" + link + "\">" + menu_title + "</a>");
                                }
                                Output.WriteLine("<ul>");


                                foreach (string pageLink in pageViewLinks)
                                {
                                    Output.WriteLine("\t\t\t<li>" + pageLink + "</li>");
                                }


                                Output.WriteLine("\t\t</ul></li>");
                            }
                        }
                    }


                    if (itemRestrictedFromUserByIp)
                    {
                        List<string> restricted_nav_bar_link = Item_Nav_Bar_HTML_Factory.Get_Nav_Bar_HTML(new View_Object(View_Enum.RESTRICTED), currentItem.Bib_Info.SobekCM_Type_String.ToUpper(), htmlSkin.Base_Skin_Code, currentMode, 0, translations, showZoomable, currentItem);
                        Output.WriteLine("\t\t" + restricted_nav_bar_link[0] + "");
                    }


					// Add the MANAGE button?
					if (userCanEditItem)
					{
						// Get all the mySObek URLs
						currentMode.Mode = Display_Mode_Enum.My_Sobek;
						currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Edit_Item_Metadata;
						string edit_metadata_url = currentMode.Redirect_URL();
						currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Edit_Item_Behaviors;
						string edit_behaviors_url = currentMode.Redirect_URL();
						currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Page_Images_Management;
						string page_images_url = currentMode.Redirect_URL();
						currentMode.My_Sobek_Type = My_Sobek_Type_Enum.File_Management;
						string manage_downloads = currentMode.Redirect_URL();

						currentMode.Mode = Display_Mode_Enum.Item_Display;
						

						Output.WriteLine("\t\t<li><a href=\"" + edit_metadata_url + "\">Manage</a><ul>");

						Output.WriteLine("\t\t\t<li><a href=\"" + edit_metadata_url + "\">Edit Metadata</a></li>");
						Output.WriteLine("\t\t\t<li><a href=\"" + edit_behaviors_url + "\">Edit Behaviors</a></li>");
						Output.WriteLine("\t\t\t<li><a href=\"" + manage_downloads + "\">Manage Downloads</a></li>");

						if ( currentItem.Web.Static_PageCount == 0 )
							Output.WriteLine("\t\t\t<li><a href=\"" + page_images_url + "\">Manage Pages and Divisions</a></li>");
						else
						{
							currentMode.ViewerCode = "qc";
							Output.WriteLine("\t\t\t<li><a href=\"" + currentMode.Redirect_URL() + "\">Manage Pages and Divisions</a></li>");
						}

						if ((currentUser.Is_Portal_Admin) || (currentUser.Is_System_Admin))
						{
							currentMode.ViewerCode = "mapedit";
							Output.WriteLine("\t\t\t<li><a href=\"" + currentMode.Redirect_URL() + "\">Manage Geo-Spatial Data (beta)</a></li>");
						}

						Output.WriteLine("\t\t</ul></li>");
					}


                    // Set current submode back
                    currentMode.Page = page;
	                currentMode.ViewerCode = viewerCode;

                    

                    Output.WriteLine("\t</ul></div>");
                    Output.WriteLine();


                    Output.WriteLine("<!-- Initialize the main item menu -->");
                    Output.WriteLine(" <script>");
                    Output.WriteLine();
                    Output.WriteLine("jQuery(document).ready(function () {");
                    Output.WriteLine("     jQuery('ul.sf-menu').superfish();");
                    Output.WriteLine("  });");
                    Output.WriteLine();
                    Output.WriteLine("</script>");
                    Output.WriteLine();
                }


            }




            return true;
        }


        /// <summary> Writes the html to the output stream open the itemNavForm, which appears just before the TocPlaceHolder </summary>
        /// <param name="Output">Stream to directly write to</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Write_ItemNavForm_Opening(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Item_HtmlSubwriter.Write_ItemNavForm_Opening", "Start the left navigational bar");

            // Add the divs for loading the pop-up forms
            Output.WriteLine("<!-- Place holders for pop-up forms which load dynamically if required -->");
            Output.WriteLine("<div class=\"print_popup_div\" id=\"form_print\" style=\"display:none;\"></div>");
            Output.WriteLine("<div class=\"email_popup_div\" id=\"form_email\" style=\"display:none;\"></div>");
            Output.WriteLine("<div class=\"add_popup_div\" id=\"add_item_form\" style=\"display:none;\"></div>");
            Output.WriteLine("<div class=\"share_popup_div\" id=\"share_form\" style=\"display:none;\"></div>");
            Output.WriteLine();

            if (should_left_navigation_bar_be_shown)
            {
                // Start the item viewer
                Output.WriteLine("<!-- Begin the left navigational bar -->");

                Output.WriteLine(PageViewer.ItemViewer_Type == ItemViewer_Type_Enum.JPEG2000 || showToc ? "<div id=\"sbkIsw_Leftnavbar_hack\">" : "<div id=\"sbkIsw_Leftnavbar\">");

                //// Compute the URL options which may be needed
                //string url_options = currentMode.URL_Options();
                //string urlOptions1 = String.Empty;
                //string urlOptions2 = String.Empty;
                //if (url_options.Length > 0)
                //{
                //    urlOptions1 = "?" + url_options;
                //    urlOptions2 = "&" + url_options;
                //}

                //// Show search results if there is a saved result
                //if (searchResultsCount > 0)
                //{
                //    Output.WriteLine("\t<ul class=\"SobekNavBarMenu\">");
                //    Output.WriteLine(currentMode.Text_Search.Length > 0
                //                                 ? "\t\t<li class=\"SobekNavBarHeader\">MATCHING PAGES</li>"
                //                                 : "\t\t<li class=\"SobekNavBarHeader\">MATCHING TILES</li>");

                //    foreach (string thisMatch in searchResultsString)
                //    {
                //        Output.WriteLine("\t\t<li>" + thisMatch.Replace("<%URLOPTS%>", url_options).Replace("<%?URLOPTS%>", urlOptions1).Replace("<%&URLOPTS%>", urlOptions2) + "</li>");
                //    }
                //    Output.WriteLine("\t</ul>");
                //    Output.WriteLine();
                //}

                // Provide way to expand TOC
                if ((!showToc) && (currentItem.Web.Static_PageCount > 1) && (currentItem.Web.Static_Division_Count > 1))
                {
                    string show_toc_text = "SHOW TABLE OF CONTENTS";

                    if (currentMode.Language == Web_Language_Enum.French)
                    {
                        show_toc_text = "VOIR L'INDEX";
                    }
                    if (currentMode.Language == Web_Language_Enum.Spanish)
                    {
                        show_toc_text = "MOSTRAR INDICE";
                    }

                    Output.WriteLine("\t<div class=\"sbkIsw_ShowTocRow\">");
                    string redirect_url = currentMode.Redirect_URL().Replace("&", "&amp;");
                    if (redirect_url.IndexOf("?") < 0)
                        redirect_url = redirect_url + "?toc=y";
                    else
                        redirect_url = redirect_url + "&toc=y";
                    Output.WriteLine("\t\t<div class=\"sbkIsw_DownTOC\"><a href=\"" + redirect_url + "\">" + show_toc_text + "<img src=\"" + currentMode.Base_URL + "default/images/button_down_arrow.png\" alt=\"\" /></a></div>");
                    // Output.WriteLine("\t\t<a href=\"" + redirect_url + "\">" + show_toc_text + "<div class=\"downarrow\"></div></a>");
                    Output.WriteLine("\t</div>");
                }

                // Allow the item/page viewer to show anything in the left navigational menu
                if (PageViewer != null)
                    PageViewer.Write_Left_Nav_Menu_Section(Output, Tracer);
            }
        }

        /// <summary> Writes the HTML generated by this item html subwriter directly to the response stream </summary>
        /// <param name="Output"> Stream to which to write the HTML for this subwriter </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Value indicating if html writer should finish the page immediately after this, or if there are other controls or routines which need to be called first </returns>
        /// <remarks> This continues writing this item from finishing the left navigation bar to the popup forms to the page navigation controls at the top of the item viewer's main area</remarks>
        public override void Write_Additional_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            // If this is for a fragment, do nothing
            if (!String.IsNullOrEmpty(currentMode.Fragment))
                return;

            Tracer.Add_Trace("Item_HtmlSubwriter.Write_Additional_HTML", "Rendering HTML ( finish left navigation bar, begin main viewer section )");

            // Add google map stuff if that is selected
            if ((PageViewer != null) && (PageViewer.ItemViewer_Type == ItemViewer_Type_Enum.Google_Map))
            {
                ((Google_Map_ItemViewer)PageViewer).Add_Google_Map_Scripts(Output, Tracer);
            }


            if (should_left_navigation_bar_be_shown)
            {
                // If this is an EAD-type item, show the table of contents here since it is all done
                // in HTML, and does not use the tree control
                
                if (isEadTypeItem) 
                {
                    EAD_Info eadInfo = (EAD_Info)currentItem.Get_Metadata_Module(GlobalVar.EAD_METADATA_MODULE_KEY);
                    if ((eadInfo != null) && (eadInfo.TOC_Included_Sections.Count > 0))
                    {
                        // Determine the URL to use for most of these
                        string description_link = String.Empty;
                        if (currentMode.ViewerCode != "description")
                            description_link = currentMode.Redirect_URL("description");

                        // Add the TOC as a floating DIV
                        Output.WriteLine("      <div id=\"sbkEad_FloatingTOC\">");
                        Output.WriteLine("      <ul class=\"sbkEad_TocMenu\">");
                        Output.WriteLine("        <li class=\"sbkEad_TocHeader\">TABLE OF CONTENTS &nbsp; <span style=\"color:#eeeeee\"><a href=\"#\" title=\"Return to the top of this document\"><img src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/up_arrow.gif\" /></a></span></li>");

                        foreach (EAD_TOC_Included_Section thisMatch in eadInfo.TOC_Included_Sections)
                        {
                            Output.WriteLine("        <li><a href=\"" + description_link + "#" + thisMatch.Internal_Link_Name + "\">" + thisMatch.Section_Title + "</a></li>");
                        }

                        // Add the container list if there is one
                        if (eadInfo.Container_Hierarchy.Containers.Count > 0)
                        {
                            Output.WriteLine("        <li><a href=\"" + currentMode.Redirect_URL("container") + "\">Container List</a></li>");
                        }

                        Output.WriteLine("      </ul>");
                        Output.WriteLine("      </div>");
                    }
                }

                if (currentItem.Behaviors.Wordmark_Count > 0)
                {
                    Output.WriteLine("\t<div id=\"sbkIsw_Wordmarks\">");

                    // Compute the URL options which may be needed
                    string url_options = currentMode.URL_Options();
                    string urlOptions1 = String.Empty;
                    string urlOptions2 = String.Empty;
                    if (url_options.Length > 0)
                    {
                        urlOptions1 = "?" + url_options;
                        urlOptions2 = "&" + url_options;
                    }

                    if (currentItem.Behaviors.Wordmark_Count > 0)
                    {
                        foreach (Wordmark_Info thisIcon in currentItem.Behaviors.Wordmarks)
                        {
                            Output.WriteLine("\t\t" + thisIcon.HTML.Replace("<%BASEURL%>", currentMode.Base_URL).Replace("<%URLOPTS%>", url_options).Replace("<%?URLOPTS%>", urlOptions1).Replace("<%&URLOPTS%>", urlOptions2));
                        }
                    }

                    Output.WriteLine("\t</div>");
                }
                else
                {
                    Output.WriteLine("\t<div id=\"sbkIsw_NoWordmarks\">&nbsp;</div>");
                }


                Output.WriteLine("</div>");
                Output.WriteLine();
            }


            // Begin the document display portion
            Output.WriteLine("<!-- Begin the main item viewing area -->");
            if (behaviors.Contains(HtmlSubwriter_Behaviors_Enum.Item_Subwriter_NonWindowed_Mode))
            {
                if ( PageViewer.Viewer_Height > 0 )
                    Output.WriteLine("<table id=\"sbkIsw_DocumentNonWindowed\" style=\"height:" + PageViewer.Viewer_Height + "px;\" >");
                else
                    Output.WriteLine("<table id=\"sbkIsw_DocumentNonWindowed\" >");
            }
            else
            {
                if ((PageViewer == null) || (PageViewer.Viewer_Width < 0))
                {
                    Output.WriteLine("<table id=\"sbkIsw_DocumentDisplay2\" >");
                }
                else
                {
                    Output.WriteLine("<table id=\"sbkIsw_DocumentDisplay\" style=\"width:" + PageViewer.Viewer_Width + "px;\" >");
                }

                // In this format, add the DARK and RESTRICTED information
                if (currentItem.Behaviors.Dark_Flag)
                {
                    Output.WriteLine("\t<tr id=\"sbkIsw_RestrictedRow\">");
                    Output.WriteLine("\t\t<td>");
                    Output.WriteLine("\t\t\t<span style=\"font-size:larger; font-weight: bold;\">DARK ITEM</span>");
                    Output.WriteLine("\t\t</td>");
                    Output.WriteLine("\t</tr>");
                }
                else if (currentItem.Behaviors.IP_Restriction_Membership < 0)
                {
                    Output.WriteLine("\t<tr id=\"sbkIsw_RestrictedRow\">");
                    Output.WriteLine("\t\t<td>");
                    Output.WriteLine("\t\t\t<span style=\"font-size:larger; font-weight: bold;\">PRIVATE ITEM</span>");
                    Output.WriteLine("\t\t\tDigitization of this item is currently in progress.");
                    Output.WriteLine("\t\t</td>");
                    Output.WriteLine("\t</tr>");
                }
            }

            #region Add navigation rows here (buttons for first, previous, next, last, etc..)

            // Add navigation row here (buttons and viewer specific)
            if (PageViewer != null)
            {
                // Allow the pageviewer to add any special elements to the main 
                // item viewer above the pagination
                PageViewer.Write_Top_Additional_Navigation_Row(Output, Tracer );

                // Should buttons be included here?
                if (PageViewer.PageCount != 1) 
                {
                    Output.WriteLine("\t<tr>");
                    Output.WriteLine("\t\t<td>");

                    // ADD NAVIGATION BUTTONS
                    if (PageViewer.PageCount != 1)
                    {
                        string go_to = "Go To:";
                        string first_page = "First Page";
                        string previous_page = "Previous Page";
                        string next_page = "Next Page";
                        string last_page = "Last Page";
                        string first_page_text = "First";
                        string previous_page_text = "Previous";
                        string next_page_text = "Next";
                        string last_page_text = "Last";

                        if (currentMode.Language == Web_Language_Enum.Spanish)
                        {
                            go_to = "Ir a:";
                            first_page = "Primera Página";
                            previous_page = "Página Anterior";
                            next_page = "Página Siguiente";
                            last_page = "Última Página";
                            first_page_text = "Primero";
                            previous_page_text = "Anterior";
                            next_page_text = "Proximo";
                            last_page_text = "Último";
                        }

                        if (currentMode.Language == Web_Language_Enum.French) 
                        {
                            go_to = "Aller à:";
                            first_page = "Première Page";
                            previous_page = "Page Précédente";
                            next_page = "Page Suivante";
                            last_page = "Dernière Page";
                            first_page_text = "Première";
                            previous_page_text = "Précédente";
                            next_page_text = "Suivante";
                            last_page_text = "Derniere";
                        }

                        Output.WriteLine("\t\t\t<div class=\"sbkIsw_PageNavBar\">");
                        StringBuilder buttonsHtmlBuilder = new StringBuilder(1000);

                        // Get the URL for the first and previous buttons
                        string firstButtonURL = PageViewer.First_Page_URL;
                        string prevButtonURL = PageViewer.Previous_Page_URL;

                        // Only continue if there is an item and mode, and there is previous pages to go to
                        if ((PageViewer.Current_Page > 1) && ((firstButtonURL.Length > 0) || (prevButtonURL.Length > 0)))
                        {
                            buttonsHtmlBuilder.AppendLine("\t\t\t\t<span class=\"sbkIsw_LeftPaginationButtons\">");
                            buttonsHtmlBuilder.AppendLine("\t\t\t\t\t<button title=\"" + first_page + "\" class=\"sbkIsw_RoundButton\" onclick=\"window.location='" + firstButtonURL + "'; return false;\"><img src=\"" + currentMode.Base_URL + "default/images/button_first_arrow.png\" class=\"roundbutton_img_left\" alt=\"\" />" + first_page_text + "</button>&nbsp;");
                            buttonsHtmlBuilder.AppendLine("\t\t\t\t\t<button title=\"" + previous_page + "\" class=\"sbkIsw_RoundButton\" onclick=\"window.location='" + prevButtonURL + "'; return false;\"><img src=\"" + currentMode.Base_URL + "default/images/button_previous_arrow.png\" class=\"roundbutton_img_left\" alt=\"\" />" + previous_page_text + "</button>");
                            buttonsHtmlBuilder.AppendLine("\t\t\t\t</span>");
                        }
                         
                        // Get the URL for the first and previous buttons
                        string lastButtonURL = PageViewer.Last_Page_URL;
                        string nextButtonURL = PageViewer.Next_Page_URL;

                        // Only continue if there is an item and mode, and there is previous pages to go to
                        if ((PageViewer.Current_Page < PageViewer.PageCount) && ((lastButtonURL.Length > 0) || (nextButtonURL.Length > 0)))
                        {
                            buttonsHtmlBuilder.AppendLine("\t\t\t\t<span class=\"sbkIsw_RightPaginationButtons\">");
                            buttonsHtmlBuilder.AppendLine("\t\t\t\t\t<button title=\"" + next_page + "\" class=\"sbkIsw_RoundButton\" onclick=\"window.location='" + nextButtonURL + "'; return false;\">" + next_page_text + "<img src=\"" + currentMode.Base_URL + "default/images/button_next_arrow.png\" class=\"roundbutton_img_right\" alt=\"\" /></button>&nbsp;");
                            buttonsHtmlBuilder.AppendLine("\t\t\t\t\t<button title=\"" + last_page + "\" class=\"sbkIsw_RoundButton\" onclick=\"window.location='" + lastButtonURL + "'; return false;\">" + last_page_text + "<img src=\"" + currentMode.Base_URL + "default/images/button_last_arrow.png\" class=\"roundbutton_img_right\" alt=\"\" /></button>");
                            buttonsHtmlBuilder.AppendLine("\t\t\t\t</span>");
                        }

                        // Write the buttons and save the HTML for the bottom of the page
                        buttonsHtml = buttonsHtmlBuilder.ToString();
                        Output.WriteLine(buttonsHtml);

                        // Show a pageselector, if one was selected
                        switch ( PageViewer.Page_Selector )
                        {
                            case ItemViewer_PageSelector_Type_Enum.DropDownList:
                                string[] pageNames = PageViewer.Go_To_Names;
                                if (pageNames.Length > 0)
                                {
                                    // Determine if these page names are very long at all
                                    if (pageNames.Any(ThisName => ThisName.Length > 25))
                                    {
                                        // Long page names, so move the Go To: to the next line (new div)
                                        Output.WriteLine("\t\t\t</div>");
                                        Output.WriteLine("\t\t\t<div class=\"sbkIsw_PageNavBar2\">");
                                    }

                                    Output.WriteLine("\t\t\t\t<span id=\"sbkIsw_GoToSpan\">" + go_to + "</span>");
                                    string orig_viewercode = currentMode.ViewerCode;
                                    string viewercode_only = currentMode.ViewerCode.Replace(currentMode.Page.ToString(), "");
                                    currentMode.ViewerCode = "XX1234567890XX";
                                    string url = currentMode.Redirect_URL();
                                    currentMode.ViewerCode = orig_viewercode;

                                    Output.WriteLine("\t\t\t\t<select id=\"page_select\" name=\"page_select\" onchange=\"javascript:item_jump_sobekcm('" + url + "')\">");

                                    // Add all the page selection items to the combo box
                                    int page_index = 1;
                                    foreach (string thisName in pageNames)
                                    {
                                        if (currentMode.Page == page_index)
                                        {
                                            Output.WriteLine("\t\t\t\t\t<option value=\"" + page_index + viewercode_only + "\" selected=\"selected\" >" + thisName + "</option>");
                                        }
                                        else
                                        {
                                            Output.WriteLine("\t\t\t\t\t<option value=\"" + page_index + viewercode_only + "\">" + thisName + "</option>");
                                        }
                                        page_index++;
                                    }

                                    Output.WriteLine("\t\t\t\t</select>");
                                }
                                break;

                            case ItemViewer_PageSelector_Type_Enum.PageLinks:
                                // Create the page selection if that is the type to display.  This is where it is actually
                                // built as well, althouogh it is subsequently used further up the page
                                if (PageViewer.Page_Selector == ItemViewer_PageSelector_Type_Enum.PageLinks)
                                {
                                    StringBuilder pageLinkBuilder = new StringBuilder();

                                    //Get the total page count
                                    int num_of_pages = PageViewer.PageCount;
                                    string[] page_urls = PageViewer.Go_To_Names;

                                    pageLinkBuilder.AppendLine("\t\t\t\t<div class=\"sbkIsw_PageLinks\">");

                                    //Display the first, last, current page numbers, and 2 pages before and after the current page
                                    if (num_of_pages <= 7 && num_of_pages > 1)
                                    {
                                        for (int i = 1; i <= num_of_pages; i++)
                                        {
                                            if (i == PageViewer.Current_Page)
                                                pageLinkBuilder.AppendLine("\t\t\t\t\t" + i + "&nbsp;");
                                            else
                                                pageLinkBuilder.AppendLine("\t\t\t\t\t<a href=\"" + page_urls[i - 1] + "\">" + i + "</a>&nbsp;");
                                        }
                                    }
                                    else if (num_of_pages > 7)
                                    {
                                        if (PageViewer.Current_Page > 4 && PageViewer.Current_Page < num_of_pages - 3)
                                        {
                                            pageLinkBuilder.AppendLine("\t\t\t\t\t<a href=\"" + page_urls[0] + "\">" + 1 + "</a>" + "...");
                                            for (int i = PageViewer.Current_Page - 2; i <= PageViewer.Current_Page + 2; i++)
                                            {
                                                if (i == PageViewer.Current_Page)
                                                    pageLinkBuilder.AppendLine("\t\t\t\t\t" + i + "&nbsp;");
                                                else
                                                    pageLinkBuilder.AppendLine("\t\t\t\t\t<a href=\"" + page_urls[i - 1] + "\">" + i + "</a>&nbsp;");
                                            }
                                            pageLinkBuilder.AppendLine("\t\t\t\t\t..." + "<a href=\"" + page_urls[page_urls.Length - 1] + "\">" + num_of_pages + "</a>");
                                        }

                                        else if (PageViewer.Current_Page <= 4 && PageViewer.Current_Page < num_of_pages - 3)
                                        {
                                            for (int i = 1; i <= (PageViewer.Current_Page + 2); i++)
                                            {
                                                if (i == PageViewer.Current_Page)
                                                    pageLinkBuilder.AppendLine("\t\t\t\t\t" + i + "&nbsp;");
                                                else
                                                    pageLinkBuilder.AppendLine("\t\t\t\t\t<a href=\"" + page_urls[i - 1] + "\">" + i + "</a>&nbsp;");
                                            }
                                            pageLinkBuilder.AppendLine("\t\t\t\t\t..." + "<a href=\"" + page_urls[page_urls.Length - 1] + "\">" + num_of_pages + "</a>");
                                        }

                                        else if (PageViewer.Current_Page > 4 && PageViewer.Current_Page >= num_of_pages - 3)
                                        {
                                            pageLinkBuilder.AppendLine("\t\t\t\t\t<a href=\"" + page_urls[0] + "\">" + 1 + "</a>" + "...");
                                            for (int i = PageViewer.Current_Page - 2; i <= num_of_pages; i++)
                                            {
                                                if (i == PageViewer.Current_Page)
                                                    pageLinkBuilder.AppendLine("\t\t\t\t\t" + i + "&nbsp;");
                                                else
                                                    pageLinkBuilder.AppendLine("\t\t\t\t\t<a href=\"" + page_urls[i - 1] + "\">" + i + "</a>&nbsp;");
                                            }

                                        }
                                    }

                                    pageLinkBuilder.AppendLine("\t\t\t\t</div>");

                                    pageLinksHtml = pageLinkBuilder.ToString();
                                    Output.WriteLine(pageLinksHtml);
                                }
                                break;
                        }

                        Output.WriteLine("\t\t\t</div>");
                    }


                    Output.WriteLine("\t\t</td>");
                    Output.WriteLine("\t</tr>");
                }
            }

            #endregion

            Output.WriteLine("\t<tr>");

            // Add the HTML from the pageviewer
            // Add the main viewer section
            if ((PageViewer != null) && (String.IsNullOrEmpty(currentMode.Fragment)))
            {
                Tracer.Add_Trace("Item_MainWriter.Write_Additional_HTML", "Allowing page viewer to write directly to the output to add main viewer section");
                PageViewer.Write_Main_Viewer_Section(Output, Tracer);
            }
        }

        /// <summary> Performs the final HTML writing which completes the item table and adds the final page navigation buttons at the bottom of the page </summary>
        /// <param name="Main_PlaceHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form, widely used throughout the application</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public void Add_Main_Viewer_Section(PlaceHolder Main_PlaceHolder, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Item_HtmlSubwriter.Add_Main_Viewer_Section", "Rendering HTML ( add any controls which the item viewer needs to add )");

            // Add the main viewer section
            if ( PageViewer != null)
            {
                // Was this to draw a fragment?
                if ( !String.IsNullOrEmpty(currentMode.Fragment))
                {
                    switch( currentMode.Fragment )
                    {
                        case "printform":
                            PrintForm_Fragment_ItemViewer printform_viewer = new PrintForm_Fragment_ItemViewer(currentPage, PageViewer) {CurrentItem = currentItem, CurrentMode = currentMode, Translator = PageViewer.Translator, CurrentUser = currentUser};
                            printform_viewer.Add_Main_Viewer_Section(Main_PlaceHolder, Tracer);
                            break;

                        case "shareform":
                            Share_Fragment_ItemViewer share_viewer = new Share_Fragment_ItemViewer {CurrentItem = currentItem, CurrentMode = currentMode, Translator = PageViewer.Translator, CurrentUser = currentUser};
                            share_viewer.Add_Main_Viewer_Section(Main_PlaceHolder, Tracer);
                            break;

                        case "addform":
                            AddRemove_Fragment_ItemViewer add_viewer = new AddRemove_Fragment_ItemViewer {CurrentItem = currentItem, CurrentMode = currentMode, Translator = PageViewer.Translator, CurrentUser = currentUser};
                            add_viewer.Add_Main_Viewer_Section(Main_PlaceHolder, Tracer);
                            break;

                        case "sendform":
                            Send_Fragment_ItemViewer send_viewer = new Send_Fragment_ItemViewer {CurrentItem = currentItem, CurrentMode = currentMode, Translator = PageViewer.Translator, CurrentUser = currentUser};
                            send_viewer.Add_Main_Viewer_Section(Main_PlaceHolder, Tracer);
                            break;
                    }
                }
                else
                {
                    Tracer.Add_Trace("Html_MainWriter.Add_Controls", "Allowing page viewer to add main viewer section to <i>mainPlaceHolder</i>");
                    PageViewer.Add_Main_Viewer_Section(Main_PlaceHolder, Tracer);
                }
            }
        }

        /// <summary> Writes final HTML to the output stream after all the placeholders and just before the itemNavForm is closed.  </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Write_ItemNavForm_Closing(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Item_HtmlSubwriter.Write_ItemNavForm_Closing", "Close the item viewer and add final pagination");

            // If this is the page turner viewer, don't draw anything else
            if ((PageViewer != null) && (PageViewer.ItemViewer_Type == ItemViewer_Type_Enum.GnuBooks_PageTurner))
            {
                return;
            }


            Output.WriteLine("\t</tr>");

            if ((PageViewer != null) && (PageViewer.PageCount != 1) && (!behaviors.Contains(HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Suppress_Bottom_Pagination)))
            {
                Output.WriteLine("\t<tr>");
                Output.WriteLine("\t\t<td>");

                // ADD NAVIGATION BUTTONS
                if (PageViewer.PageCount != 1)
                {
                    Output.WriteLine("\t\t\t<div class=\"sbkIsw_PageNavBar\">");
                    Output.WriteLine(buttonsHtml);

                    // Create the page selection if that is the type to display.  This is where it is actually
                    // built as well, althouogh it is subsequently used further up the page
                    if (PageViewer.Page_Selector == ItemViewer_PageSelector_Type_Enum.PageLinks)
                    {
                        Output.WriteLine(pageLinksHtml);
                    }

                    Output.WriteLine("\t\t\t</div>");
                }

                Output.WriteLine("\t\t</td>");
                Output.WriteLine("\t</tr>");
            }

            if (PageViewer != null && ((currentItem.Behaviors.CheckOut_Required) && (PageViewer.ItemViewer_Type != ItemViewer_Type_Enum.Checked_Out)))
            {
                Output.WriteLine("<tr><td><span id=\"sbkIsw_CheckOutRequired\">This item contains copyrighted material and is reserved for single (fair) use.  Once you finish working with this item,<br />it will return to the digital stacks in fifteen minutes for another patron to use.<br /><br /></span></td></tr>");
            }
            Output.WriteLine("</table>");

            // Add a spot for padding
            Output.WriteLine();
            Output.WriteLine("<!-- Division is used to add extra bottom padding, if the left nav bar is taller than the item viewer -->");
            Output.WriteLine("<div id=\"sbkIsw_BottomPadding\"></div>");
            Output.WriteLine();

            // None of the sharing options are available if the user is restricted from this item
            // or if we are generating this as a static page source for robots
	        if ((!itemRestrictedFromUserByIp) && (!itemCheckedOutByOtherUser) && (!currentMode.Is_Robot))
	        {
		        // Add the hidden field
		        Output.WriteLine("<!-- Hidden field is used for postbacks to indicate what to save and reset -->");
		        Output.WriteLine("<input type=\"hidden\" id=\"item_action\" name=\"item_action\" value=\"\" />");
		        Output.WriteLine();
	        }

	        // Add the scripts needed
            Output.WriteLine("<!-- Add references to the sobekcm javascript files and libraries needed for the user menu -->");
            Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_form.js\" ></script>");
            Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_item.js\" ></script>");
            Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/superfish/hoverIntent.js\" ></script>");
            Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/superfish/superfish.js\" ></script>");
            Output.WriteLine();
        }

        /// <summary> Spot to write any final HTML to the response stream  </summary>
        /// <param name="Output"> Stream to which to write the HTML for this subwriter </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Final_HTML(TextWriter Output, Custom_Tracer Tracer )
        {
            Tracer.Add_Trace("Item_HtmlSubwriter.Write_Final_Html", "Do nothing");
        }

        #region Methods to create the treeview control for the table of contents

        /// <summary> Populates the tree view with the divisions from the current digital resource item </summary>
        /// <param name="TreeViewArg"> Tree view control to populate </param>
        public void Create_TreeView_From_Divisions(TreeView TreeViewArg )
        {
            tocSelectedComplete = false;

            // Get the current mode page
            List<TreeNode> nodes = new List<TreeNode>();
            List<TreeNode> selectedNodes = new List<TreeNode>();

            int sequence = 0;
            foreach (abstract_TreeNode absNode in currentItem.Divisions.Physical_Tree.Roots)
            {
                Division_TreeNode divNode = (Division_TreeNode) absNode;
                TreeNode treeViewNode = new TreeNode { Text = string.Format("<span class=\"sbkIsw_TocTreeViewItem\" Title=\"{0}\">{1}</span>", divNode.Display_Label, divNode.Display_Short_Label) };
                TreeViewArg.Nodes.Add( treeViewNode );
                nodes.Add(treeViewNode);
                List<TreeNode> pathNodes = new List<TreeNode> {treeViewNode};
                recurse_through_tree(divNode, treeViewNode, nodes, selectedNodes, pathNodes, ref sequence );
            }

            foreach (TreeNode selectedNode in selectedNodes)
            {
                selectedNode.Text = selectedNode.Text.Replace("sbkIsw_TocTreeViewItem", "sbkIsw_SelectedTocTreeViewItem");
                TreeNode selectedNodeExpander = selectedNode;
                while (selectedNodeExpander.Parent != null) 
                {
                    (selectedNodeExpander.Parent).Expand();
                    selectedNodeExpander = selectedNodeExpander.Parent;
                }
            }
        }

        private void recurse_through_tree(Division_TreeNode parentNode, TreeNode parentViewNode, List<TreeNode> nodes, List<TreeNode> selectedNodes, List<TreeNode> pathNodes, ref int sequence)
        {
            foreach (abstract_TreeNode absNode in parentNode.Nodes)
            {
                if (absNode.Page)
                {
                    sequence++;

                    foreach (TreeNode thisNode in nodes)
                    {
                        thisNode.Value = sequence.ToString();
                    }
                    if (sequence >= currentMode.Page)
                    {
                        if (!tocSelectedComplete)
                        {
                            selectedNodes.AddRange(pathNodes);
                            tocSelectedComplete = true;
                        }
                        else
                        {
                            if (sequence == currentMode.Page)
                            {
                                selectedNodes.AddRange(pathNodes);
                            }
                        }
                    }
                    nodes.Clear();
                }
                else
                {
                    Division_TreeNode divNode = (Division_TreeNode)absNode;
                    TreeNode treeViewNode = new TreeNode { Text = string.Format("<span class=\"SobekTocTreeViewItem\" Title='{0}'>{1}</span>", divNode.Display_Label, divNode.Display_Short_Label) };
                    parentViewNode.ChildNodes.Add(treeViewNode);
                    nodes.Add(treeViewNode);
                    List<TreeNode> pathNodes2 = new List<TreeNode> { treeViewNode };
                    recurse_through_tree(divNode, treeViewNode, nodes, selectedNodes, pathNodes2, ref sequence);
                }
            }
        }

        #endregion

        /// <summary> Gets the collection of body attributes to be included 
        /// within the HTML body tag (usually to add events to the body) </summary>
        public override List<Tuple<string, string>> Body_Attributes
        {
            get
            {
                List<Tuple<string, string>> returnValue = new List<Tuple<string, string>>
                    {
                        new Tuple<string, string>("onload", "itemwriter_load();"), 
                        new Tuple<string, string>("onresize", "itemwriter_load();"),
						new Tuple<string, string>("id", "itembody")
                    };

                // Add default script attachments

                // Add any viewer specific body attributes
                if (PageViewer != null)
                    PageViewer.Add_ViewerSpecific_Body_Attributes(returnValue);
                return returnValue;
            }
        }

        /// <summary> Title for this web page </summary>
        public override string WebPage_Title
        {
            get
            {
                return currentItem != null ? currentItem.Bib_Info.Main_Title.Title : "{0} Item";
            }
        }

        /// <summary> Write any additional values within the HTML Head of the
        /// final served page </summary>
        /// <param name="Output"> Output stream currently within the HTML head tags </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Within_HTML_Head(TextWriter Output, Custom_Tracer Tracer)
        {
            // ROBOTS SHOULD BE SENT TO THE CMS PAGE FOR THIS
            Output.WriteLine("  <meta name=\"robots\" content=\"noindex, nofollow\" />");

            // Write the main SobekCM item style sheet to use 
            Output.WriteLine("  <link href=\"" + currentMode.Base_URL + "default/SobekCM_Item.css\" rel=\"stylesheet\" type=\"text/css\" title=\"standard\" />");
            Output.WriteLine("  <link href=\"" + currentMode.Base_URL + "default/SobekCM_ItemMenus.css\" rel=\"stylesheet\" type=\"text/css\" title=\"standard\" />");

            // Add any viewer specific tags that need to reside within the HTML head
            if (PageViewer != null)
                PageViewer.Write_Within_HTML_Head(Output, Tracer);

            // Add a thumbnail to this item
            if (currentItem != null)
            {
                string image_src = currentMode.Base_URL + "/" + currentItem.Web.AssocFilePath + "/" + currentItem.Behaviors.Main_Thumbnail;

                Output.WriteLine("  <link rel=\"image_src\" href=\"" + image_src.Replace("\\", "/").Replace("//", "/").Replace("http:/", "http://") + "\" />");
            }
        }
    }
}
