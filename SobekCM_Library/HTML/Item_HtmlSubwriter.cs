#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Resource_Object.Metadata_Modules;
using SobekCM.Resource_Object.Metadata_Modules.EAD;
using SobekCM.Resource_Object.Metadata_Modules.GeoSpatial;
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
        private readonly double featureXRatioLocation = -1D;
        private readonly double featureYRatioLocation = -1D;
        private readonly bool isEadTypeItem;
        private bool itemCheckedOutByOtherUser;
        private readonly bool itemRestrictedFromUserByIp;
        private readonly SobekCM_Items_In_Title itemsInTitle;
        private readonly double providedMaxLat;
        private readonly double providedMaxLong;
        private readonly double providedMinLat;
        private readonly double providedMinLong;
        private readonly bool searchMatchOnThisPage;
        private readonly int searchResultsCount;
        private readonly List<string> searchResultsString;
        private readonly bool showToc;
        private readonly bool showZoomable;
        private bool tocSelectedComplete;
        private readonly Language_Support_Info translations;
        private TreeView treeView1;
        private readonly bool userCanEditItem;
        private string pageselectorhtml;
        private List<HtmlSubwriter_Behaviors_Enum> behaviors;

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
            Nav_Bar_Menu_Section_Added = false;
            itemCheckedOutByOtherUser = false;
            userCanEditItem = false;
            searchResultsCount = 0;

            // Determine if this item is an EAD
            isEadTypeItem = (currentItem.Get_Metadata_Module(GlobalVar.EAD_METADATA_MODULE_KEY) != null);

            // Determine if this item is actually restricted 
            itemRestrictedFromUserByIp = false;
            if (Item_Restricted_Message.Length > 0)
                itemRestrictedFromUserByIp = true;

            // If this item is restricted by IP than alot of the upcoming code is unnecessary
            if (!itemRestrictedFromUserByIp)
            {
                // Searching for EAD/EAC type items is different from others
                if (!isEadTypeItem)
                {
                    // If there is a coordinate search, and polygons, do that
                    // GEt the geospatial metadata module
                    GeoSpatial_Information geoInfo = currentItem.Get_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY) as GeoSpatial_Information;
                    if ((geoInfo != null) && (geoInfo.hasData))
                    {
                        if ((currentMode.Coordinates.Length > 0) && (geoInfo.Polygon_Count > 1))
                        {
                            // Determine the coordinates in this search
                            string[] splitter = currentMode.Coordinates.Split(",".ToCharArray());

                            if (((splitter.Length > 1) && (splitter.Length < 4)) || ((splitter.Length == 4) && (splitter[2].Length == 0) && (splitter[3].Length == 0)))
                            {
                                Double.TryParse(splitter[0], out providedMaxLat);
                                Double.TryParse(splitter[1], out providedMaxLong);
                                providedMinLat = providedMaxLat;
                                providedMinLong = providedMaxLong;
                            }
                            else if (splitter.Length >= 4)
                            {
                                Double.TryParse(splitter[0], out providedMaxLat);
                                Double.TryParse(splitter[1], out providedMaxLong);
                                Double.TryParse(splitter[2], out providedMinLat);
                                Double.TryParse(splitter[3], out providedMinLong);
                            }


                            // Now, if there is length, determine the count of results
                            searchResultsString = new List<string>();
                            if (searchResultsString.Count > 0)
                            {
                                searchResultsCount = searchResultsString.Count;

                                // Also, look to see where the current point lies in the matching, current polygon
                                if ((providedMaxLong == providedMinLong) && (providedMaxLat == providedMinLat))
                                {
                                    foreach (Coordinate_Polygon itemPolygon in geoInfo.Polygons)
                                    {
                                        // Is this the current page?
                                        if (itemPolygon.Page_Sequence == currentMode.Page)
                                        {
                                            if (itemPolygon.is_In_Bounding_Box(providedMaxLat, providedMaxLong))
                                            {
                                                searchMatchOnThisPage = true;
                                                ReadOnlyCollection<Coordinate_Point> boundingBox = itemPolygon.Bounding_Box;
                                                featureYRatioLocation = Math.Abs(((providedMaxLat - boundingBox[0].Latitude)/(boundingBox[0].Latitude - boundingBox[1].Latitude)));
                                                featureXRatioLocation = Math.Abs(((providedMaxLong - boundingBox[0].Longitude)/(boundingBox[0].Longitude - boundingBox[1].Longitude)));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

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
                                bool is_html_format = !(format == "TEXT");

                                // CC: the user, unless they are already on the list
                                string cc_list = currentUser.Email;
                                if (address.ToUpper().IndexOf(currentUser.Email.ToUpper()) >= 0)
                                    cc_list = String.Empty;

                                // Send the email
                                HttpContext.Current.Session.Add("ON_LOAD_MESSAGE", !Item_Email_Helper.Send_Email(address, cc_list, comments, currentUser.Full_Name,currentMode.SobekCM_Instance_Abbreviation,currentItem,is_html_format,HttpContext.Current.Items["Original_URL"].ToString())
                                    ? "Error encountered while sending email" : "Your email has been sent");

                                HttpContext.Current.Response.Redirect( HttpContext.Current.Items["Original_URL"].ToString(), false);
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

            Tracer.Add_Trace("Html_MainWriter.Add_Controls", "Created " + PageViewer.GetType().ToString().Replace("SobekCM.Library.ItemViewer.Viewers.", ""));

            // Assign the rest of the information, if a page viewer was created
            if (PageViewer != null)
            {
                PageViewer.CurrentItem = currentItem;
                PageViewer.CurrentMode = currentMode; 
                PageViewer.Translator = Translator;
                PageViewer.CurrentUser = currentUser;

                if (PageViewer is Citation_ItemViewer)
                {
                        
                    ((Citation_ItemViewer)PageViewer).Code_Manager = Code_Manager;
                    ((Citation_ItemViewer)PageViewer).Current_User = Current_User;
                    ((Citation_ItemViewer)PageViewer).Item_Restricted = itemRestrictedFromUserByIp;
                }

                // The JPEG2000 viewer should always include the nav bar to include the navigation thumbnail image
                if (PageViewer is Aware_JP2_ItemViewer)
                {
                    Nav_Bar_Menu_Section_Added = true;
                }

                //if (page_viewer is ItemViewer.Viewers.Google_Map_ItemViewer)
                //{
                //    ((ItemViewer.Viewers.Google_Map_ItemViewer)page_viewer).Matching_Tiles_List = search_results_string;
                //}

                if (PageViewer is MultiVolumes_ItemViewer)
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

                    ((MultiVolumes_ItemViewer)PageViewer).Item_List = itemsInTitle;
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

            if ((searchMatchOnThisPage) && ((PageViewer.ItemViewer_Type == ItemViewer_Type_Enum.JPEG) || (PageViewer.ItemViewer_Type == ItemViewer_Type_Enum.JPEG2000)))
            {
                if (PageViewer.ItemViewer_Type == ItemViewer_Type_Enum.JPEG2000)
                {
                    Aware_JP2_ItemViewer jp2_viewer = (Aware_JP2_ItemViewer)PageViewer;
                    jp2_viewer.Add_Feature("Red", "DrawEllipse", ((int)(featureXRatioLocation * jp2_viewer.Width)), ((int)(featureYRatioLocation * jp2_viewer.Height)), 800, 800);
                }

            }
        }

        #endregion

        private bool should_left_navigation_bar_be_shown
        {
            get
            {
                // If this is for a fragment, do nothing
                if (!String.IsNullOrEmpty(currentMode.Fragment))
                    return false;

                // The pageturner does not use the nav bar
                if ((PageViewer != null) && (PageViewer is GnuBooks_PageTurner_ItemViewer))
                    return false;

                // The pageturner does not use the nav bar
                if ((PageViewer != null) && (PageViewer is QC_ItemViewer))
                    return false;

                // The pageturner does not use the nav bar
                if ((PageViewer != null) && (PageViewer is Google_Coordinate_Entry_ItemViewer))
                    return false;

                // If the flag eas explicitly set, return TRUE
                if (Nav_Bar_Menu_Section_Added)
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

                // EAD type items almost always have tables of contents programmatically generated
                if (isEadTypeItem)
                    return true;

                // JPEG2000's should always show the nav bar for the navigational thumbnail
                if ((PageViewer != null) && (PageViewer is Aware_JP2_ItemViewer))
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

        /// <summary> Flag indicates if the naviogation bar menu section is added  </summary>
        public bool Nav_Bar_Menu_Section_Added { private get; set; }

        /// <summary> Gets the collection of special behaviors which this subwriter
        /// requests from the main HTML subwriter. </summary>
        /// <remarks> By default, this returns an empty list </remarks>
        public override List<HtmlSubwriter_Behaviors_Enum> Subwriter_Behaviors
        {
            get
            {
                List<HtmlSubwriter_Behaviors_Enum> behaviors = new List<HtmlSubwriter_Behaviors_Enum>();
                ;
                return new List<HtmlSubwriter_Behaviors_Enum>
                    {
                        HtmlSubwriter_Behaviors_Enum.Suppress_Banner
                    };
            }
        }

        #region Code to add the table of contents tree as a control into the left navigation bar

        /// <summary> Adds any viewer_specific information to the Navigation Bar Menu Section </summary>
        /// <param name="Navigation_Place_Holder"> Additional place holder ( &quot;navigationPlaceHolder&quot; ) in the itemNavForm form allows item-viewer-specific controls to be added to the left navigation bar</param>
        /// <param name="Internet_Explorer"> Flag indicates if the current browser is internet explorer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <remarks> For this item viewer, a small thumbnail of the entire image showing the current viewport location is placed in the left navigation bar </remarks>
        public void Add_Nav_Bar_Menu_Section(PlaceHolder Navigation_Place_Holder, bool Internet_Explorer, Custom_Tracer Tracer)
        {
            // If this is for a fragment, do nothing
            if ( !String.IsNullOrEmpty(currentMode.Fragment))
                return;
            
            StringBuilder buildResponse = new StringBuilder(2000);
            buildResponse.AppendLine();


            // Add the divs for loading the pop-up forms
            buildResponse.AppendLine("<!-- Place holders for pop-up forms which load dynamically if required -->");
            buildResponse.AppendLine("<div class=\"print_popup_div\" id=\"form_print\" style=\"display:none;\"></div>");
            buildResponse.AppendLine("<div class=\"email_popup_div\" id=\"form_email\" style=\"display:none;\"></div>");
            buildResponse.AppendLine("<div class=\"add_popup_div\" id=\"add_item_form\" style=\"display:none;\"></div>");
            buildResponse.AppendLine("<div class=\"share_popup_div\" id=\"share_form\" style=\"display:none;\"></div>");
            buildResponse.AppendLine();

            if (PageViewer.ItemViewer_Type != ItemViewer_Type_Enum.GnuBooks_PageTurner)
            {
                buildResponse.AppendLine("<!-- Show the title and any other important item information -->");
                buildResponse.AppendLine("<div id=\"itemtitlebar\">");
                if (currentItem.METS_Header.RecordStatus_Enum == METS_Record_Status.BIB_LEVEL)
                {
                    string grouptitle = currentItem.Behaviors.GroupTitle;
                    if (grouptitle.Length > 125)
                    {
                        buildResponse.AppendLine("\t<h1><abbr title=\"" + grouptitle + "\">" + grouptitle.Substring(0, 120) + "...</abbr></h1>");
                    }
                    else
                    {
                        buildResponse.AppendLine("\t<h1>" + grouptitle + "</h1>");
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
                        if (final_title.Length > 125)
                        {
                            buildResponse.AppendLine("\t<h1><abbr title=\"" + final_title + "\">" + final_title.Substring(0, 120) + "...</abbr></h1>");
                        }
                        else
                        {
                            buildResponse.AppendLine("\t<h1>" + final_title + "</h1>");
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
                            buildResponse.AppendLine("\t<a href=\"" + currentItem.Bib_Info.Location.Other_URL + "\">" + note + " ( " + type + " )</a><br />");
                        }
                    }

                    // If there is an ACCESSION number and this is an ARTIFACT, include that at the top
                    if ((currentItem.Bib_Info.SobekCM_Type == TypeOfResource_SobekCM_Enum.Artifact) && (currentItem.Bib_Info.Identifiers_Count > 0))
                    {
                        foreach (Identifier_Info thisIdentifier in currentItem.Bib_Info.Identifiers)
                        {
                            if (thisIdentifier.Type.ToUpper().IndexOf("ACCESSION") >= 0)
                            {
                                buildResponse.AppendLine("\t" + translations.Get_Translation("Accession number", currentMode.Language) + " " + thisIdentifier.Identifier + "<br />");
                                break;
                            }
                        }
                    }
                }

                buildResponse.AppendLine("</div>");
                buildResponse.AppendLine();

                // The item viewer can choose to override the standard item menu
                if ( !behaviors.Contains( HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Suppress_Item_Menu ))
                {
                    // Add the item views
                    buildResponse.AppendLine("<!-- Add the different view and social options -->");
                    buildResponse.AppendLine("<div id=\"itemviewersbar\" style=\"text-align: center\">");
                    buildResponse.AppendLine("\t<ul class=\"sf-menu\">");

                    // Save the current view type
                    ushort page = currentMode.Page;

                    // Add the item level views
                    foreach (View_Object thisView in currentItem.Behaviors.Views)
                    {
                        if ((!itemRestrictedFromUserByIp) || (thisView.View_Type == View_Enum.CITATION) ||
                            (thisView.View_Type == View_Enum.ALL_VOLUMES) ||
                            (thisView.View_Type == View_Enum.RELATED_IMAGES))
                        {
                            // Special code for the CITATION view (TEMPORARY - v.3.2)
                            if (thisView.View_Type == View_Enum.CITATION)
                            {
                                string viewerCode = currentMode.ViewerCode;
                                currentMode.ViewerCode = "citation";
                                if ((viewerCode == "citation") || (viewerCode == "marc") || (viewerCode == "metadata") ||
                                    (viewerCode == "usage"))
                                {
                                    buildResponse.Append("\t\t<li id=\"selected-sf-menu-item\">Citation");
                                }
                                else
                                {
                                    buildResponse.Append("\t\t<li><a href=\"" + currentMode.Redirect_URL() +
                                                         "\">Citation</a>");
                                }
                                buildResponse.AppendLine("<ul>");

                                buildResponse.AppendLine("\t\t\t<li><a href=\"" + currentMode.Redirect_URL() +
                                                         "\">Standard View</a></li>");

                                currentMode.ViewerCode = "marc";
                                buildResponse.AppendLine("\t\t\t<li><a href=\"" + currentMode.Redirect_URL() +
                                                         "\">MARC View</a></li>");

                                currentMode.ViewerCode = "metadata";
                                buildResponse.AppendLine("\t\t\t<li><a href=\"" + currentMode.Redirect_URL() +
                                                         "\">Metadata</a></li>");

                                currentMode.ViewerCode = "usage";
                                buildResponse.AppendLine("\t\t\t<li><a href=\"" + currentMode.Redirect_URL() +
                                                         "\">Usage Statistics</a></li>");

                                buildResponse.AppendLine("\t\t</ul></li>");
                                currentMode.ViewerCode = viewerCode;
                            }
                            else if (thisView.View_Type == View_Enum.ALL_VOLUMES)
                            {
                                string viewerCode = currentMode.ViewerCode;
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
                                    buildResponse.Append("\t\t<li id=\"selected-sf-menu-item\">" + all_volumes);
                                }
                                else
                                {
                                    buildResponse.Append("\t\t<li><a href=\"" + currentMode.Redirect_URL() + "\">" +
                                                         all_volumes + "</a>");
                                }
                                buildResponse.AppendLine("<ul>");

                                currentMode.ViewerCode = "allvolumes";
                                buildResponse.AppendLine("\t\t\t<li><a href=\"" + currentMode.Redirect_URL() +
                                                         "\">Tree View</a></li>");


                                currentMode.ViewerCode = "allvolumes2";
                                buildResponse.AppendLine("\t\t\t<li><a href=\"" + currentMode.Redirect_URL() +
                                                         "\">Thumbnails</a></li>");

                                if (currentMode.Internal_User)
                                {
                                    currentMode.ViewerCode = "allvolumes3";
                                    buildResponse.AppendLine("\t\t\t<li><a href=\"" + currentMode.Redirect_URL() +
                                                             "\">List View</a></li>");
                                }

                                buildResponse.AppendLine("\t\t</ul></li>");
                                currentMode.ViewerCode = viewerCode;
                            }
                            else
                            {
                                List<string> item_nav_bar_link = Item_Nav_Bar_HTML_Factory.Get_Nav_Bar_HTML(thisView,
                                                                                                            currentItem
                                                                                                                .Bib_Info
                                                                                                                .SobekCM_Type_String,
                                                                                                            htmlSkin
                                                                                                                .Base_Skin_Code,
                                                                                                            currentMode,
                                                                                                            -1,
                                                                                                            translations,
                                                                                                            showZoomable,
                                                                                                            currentItem);
                                // Add each nav bar link
                                foreach (string this_link in item_nav_bar_link)
                                {
                                    buildResponse.AppendLine("\t\t" + this_link + "");
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
                        if (currentItem.Behaviors.Item_Level_Page_Views_Count > 0)
                        {
                            foreach (View_Object thisPageView in currentItem.Behaviors.Item_Level_Page_Views)
                            {
                                View_Enum thisViewType = thisPageView.View_Type;
                                foreach (List<string> page_nav_bar_link in from thisFile in currentPage.Files
                                                                           let fileObject = thisFile.Get_Viewer()
                                                                           where fileObject != null
                                                                           where fileObject.View_Type == thisViewType
                                                                           select
                                                                               Item_Nav_Bar_HTML_Factory
                                                                               .Get_Nav_Bar_HTML(thisFile.Get_Viewer(),
                                                                                                 currentItem.Bib_Info
                                                                                                            .SobekCM_Type_String
                                                                                                            .ToUpper(),
                                                                                                 htmlSkin.Base_Skin_Code,
                                                                                                 currentMode, page_seq,
                                                                                                 translations,
                                                                                                 showZoomable,
                                                                                                 currentItem))
                                {
                                    foreach (string nav_link in page_nav_bar_link)
                                    {
                                        buildResponse.AppendLine("\t\t" + nav_link + "");
                                    }
                                    break;
                                }
                            }
                        }
                    }

                    if (itemRestrictedFromUserByIp)
                    {
                        List<string> restricted_nav_bar_link =
                            Item_Nav_Bar_HTML_Factory.Get_Nav_Bar_HTML(new View_Object(View_Enum.RESTRICTED),
                                                                       currentItem.Bib_Info.SobekCM_Type_String.ToUpper(),
                                                                       htmlSkin.Base_Skin_Code, currentMode, 0,
                                                                       translations, showZoomable, currentItem);
                        buildResponse.AppendLine("\t\t" + restricted_nav_bar_link[0] + "");
                    }

                    // Set current submode back
                    currentMode.Page = page;


                    // Add the sharing buttons if this is not restricted by IP address or checked out
                    if ((!itemRestrictedFromUserByIp) && (!itemCheckedOutByOtherUser) && (!currentMode.Is_Robot))
                    {
                        buildResponse.AppendLine(
                            "\t\t<li id=\"sharebuttonitem\" class=\"action-sf-menu-item\" style=\"float:right;\" onclick=\"toggle_share_form('share_button');\"><span id=\"sharebuttonspan\">Share</span></li>");


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


                        if ((currentUser != null))
                        {
                            if (currentItem.Web.ItemID > 0)
                            {
                                if (currentUser.Is_In_Bookshelf(currentItem.BibID, currentItem.VID))
                                {
                                    buildResponse.AppendLine(
                                        "\t\t<li id=\"addbuttonitem\" class=\"action-sf-menu-item\" style=\"float:right;\" onclick=\"return remove_item_itemviewer();\"><img src=\"" +
                                        currentMode.Base_URL +
                                        "default/images/minussign.png\" alt=\"\" style=\"vertical-align:middle\" /><span id=\"addbuttonspan\">Remove</span></li>");
                                }
                                else
                                {
                                    buildResponse.AppendLine(
                                        "\t\t<li id=\"addbuttonitem\" class=\"action-sf-menu-item\" style=\"float:right;\" onclick=\"add_item_form_open();\"><img src=\"" +
                                        currentMode.Base_URL +
                                        "default/images/plussign.png\" alt=\"\" style=\"vertical-align:middle\" /><span id=\"addbuttonspan\">Add</span></li>");
                                }
                            }

                            buildResponse.AppendLine(
                                "\t\t<li id=\"sendbuttonitem\" class=\"action-sf-menu-item\" style=\"float:right;\" onclick=\"email_form_open();\"><img src=\"" +
                                currentMode.Base_URL +
                                "default/images/email.png\" alt=\"\" style=\"vertical-align:middle\" /><span id=\"sendbuttonspan\">Send</span></li>");
                        }
                        else
                        {
                            if (currentItem.Web.ItemID > 0)
                                buildResponse.AppendLine(
                                    "\t\t<li id=\"addbuttonitem\" class=\"action-sf-menu-item\" style=\"float:right;\" onclick=\"window.location='?m=hmh';\"><img src=\"" +
                                    currentMode.Base_URL +
                                    "default/images/plussign.png\" alt=\"\" style=\"vertical-align:middle\" /><span id=\"addbuttonspan\">Add</span></li>");

                            buildResponse.AppendLine(
                                "\t\t<li id=\"sendbuttonitem\" class=\"action-sf-menu-item\" style=\"float:right;\" onclick=\"window.location='?m=hmh';\"><img src=\"" +
                                currentMode.Base_URL +
                                "default/images/email.png\" alt=\"\" style=\"vertical-align:middle\" /><span id=\"sendbuttonspan\">Send</span></li>");
                        }

                        if (currentItem.Web.ItemID > 0)
                        {
                            buildResponse.AppendLine(
                                "\t\t<li id=\"printbuttonitem\" class=\"action-sf-menu-item\" style=\"float:right;\" onclick=\"print_form_open();\"><img src=\"" +
                                currentMode.Base_URL +
                                "default/images/printer.png\" alt=\"\" style=\"vertical-align:middle\" /><span id=\"printbuttonspan\">Print</span></li>");
                        }
                        else
                        {
                            buildResponse.AppendLine(
                                "\t\t<li id=\"printbuttonitem\" class=\"action-sf-menu-item\" style=\"float:right;\" onclick=\"window.print();return false;\"><img src=\"" +
                                currentMode.Base_URL +
                                "default/images/printer.png\" alt=\"\" style=\"vertical-align:middle\" /><span id=\"printbuttonspan\">Print</span></li>");
                        }



                    }

                    buildResponse.AppendLine("\t</ul>");
                    buildResponse.AppendLine();

                    buildResponse.AppendLine("</div>");
                    buildResponse.AppendLine();
                }

            }


            if (should_left_navigation_bar_be_shown)
            {
                // Start the item viewer
                buildResponse.AppendLine("<!-- Begin the left navigational bar -->");
                if ( PageViewer.ItemViewer_Type == ItemViewer_Type_Enum.JPEG2000 )
                    buildResponse.AppendLine("<div id=\"itemviewleftnavbar_hack\">");
                else
                    buildResponse.AppendLine("<div id=\"itemviewleftnavbar\">");

                // Compute the URL options which may be needed
                string url_options = currentMode.URL_Options();
                string urlOptions1 = String.Empty;
                string urlOptions2 = String.Empty;
                if (url_options.Length > 0)
                {
                    urlOptions1 = "?" + url_options;
                    urlOptions2 = "&" + url_options;
                }

                // Show search results if there is a saved result
                if (searchResultsCount > 0)
                {
                    buildResponse.AppendLine("\t<ul class=\"SobekNavBarMenu\">");
                    buildResponse.AppendLine(currentMode.Text_Search.Length > 0
                                                 ? "\t\t<li class=\"SobekNavBarHeader\">MATCHING PAGES</li>"
                                                 : "\t\t<li class=\"SobekNavBarHeader\">MATCHING TILES</li>");

                    foreach (string thisMatch in searchResultsString)
                    {
                        buildResponse.AppendLine("\t\t<li>" + thisMatch.Replace("<%URLOPTS%>", url_options).Replace("<%?URLOPTS%>", urlOptions1).Replace("<%&URLOPTS%>", urlOptions2) + "</li>");
                    }
                    buildResponse.AppendLine("\t</ul>");
                    buildResponse.AppendLine();
                }

                // Provide way to expand TOC
                if ((!showToc) && (currentItem.Web.Static_PageCount > 1) && (currentItem.Web.Static_Division_Count > 1))
                {
                    string show_toc_text = "SHOW TABLE OF CONTENTS";
                    int width = 180;

                    if (currentMode.Language == Web_Language_Enum.French)
                    {
                        show_toc_text = "VOIR L'INDEX";
                        width = 120;
                    }
                    if (currentMode.Language == Web_Language_Enum.Spanish)
                    {
                        show_toc_text = "MOSTRAR INDICE";
                        width = 140;
                    }

                    buildResponse.AppendLine("\t<div class=\"ShowTocRow\" style=\"width: " + width + "px;\" >");
                    string redirect_url = currentMode.Redirect_URL().Replace("&", "&amp;");
                    if (redirect_url.IndexOf("?") < 0)
                        redirect_url = redirect_url + "?toc=y";
                    else
                        redirect_url = redirect_url + "&toc=y";
                    buildResponse.AppendLine("\t\t<a href=\"" + redirect_url + "\">");
                    buildResponse.AppendLine("\t\t\t<img src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/tabs/cLDG.gif\" alt=\"\" /><img src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/tabs/AD.gif\" border=\"0\" alt=\"\" /><span class=\"tab\">" + show_toc_text + "</span><img src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/tabs/cRDG.gif\" alt=\"\" />");
                    buildResponse.AppendLine("\t\t</a>");
                    buildResponse.AppendLine("\t</div>");
                }

                // Anything to add?
                if (buildResponse.Length > 0)
                {
                    Literal newLiteral = new Literal {Text = buildResponse.ToString()};
                    Navigation_Place_Holder.Controls.Add(newLiteral);
                }


                // Add the navigation part to this form
                if (PageViewer != null)
                {
                    Tracer.Add_Trace("Html_MainWriter.Add_Controls", "Allowing page viewer to add left navigation bar section to <i>navigationPlaceHolder</i>");
                    Nav_Bar_Menu_Section_Added = PageViewer.Add_Nav_Bar_Menu_Section(Navigation_Place_Holder, Internet_Explorer, Tracer);
                }
            }
            else
            {
                // Anything to add?
                if (buildResponse.Length > 0)
                {
                    Literal newLiteral = new Literal { Text = buildResponse.ToString() };
                    Navigation_Place_Holder.Controls.Add(newLiteral);
                }
            }
        }

        /// <summary> Adds the table of contents as a control in the left navigation bar </summary>
        /// <param name="placeHolder"> TOC place holder ( &quot;tocPlaceHolder&quot; ) in the itemNavForm form, widely used throughout the application</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public void Add_Standard_TOC(PlaceHolder placeHolder, Custom_Tracer Tracer)
        {
            // If this is for a fragment, do nothing
            if (!String.IsNullOrEmpty(currentMode.Fragment))
                return;

            // EAD type items do not display the TOC like this, as a control
            if (isEadTypeItem)
                return;

            // QC and GNU Page turner doesn't show this either
            if ((PageViewer.ItemViewer_Type == ItemViewer_Type_Enum.GnuBooks_PageTurner) || (PageViewer.ItemViewer_Type == ItemViewer_Type_Enum.Quality_Control))
                return;

            if ((showToc) && (currentItem.Web.Static_PageCount > 1) && (currentItem.Web.Static_Division_Count > 1))
            {
                Tracer.Add_Trace("Item_HtmlSubwriter.Add_Standard_TOC", "Adding Table of Contents control to <i>placeHolder</i>");

                string table_of_contents = "TABLE OF CONTENTS";
                string hide_toc = "HIDE";

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

                menuStartLiteral.Text = string.Format("        <ul class=\"SobekNavBarMenu\">" + Environment.NewLine + "          <li class=\"SobekNavBarHeader\"> {0} </li>" + Environment.NewLine + "        </ul>" + Environment.NewLine  + "        <div class=\"HideTocRow\">" + Environment.NewLine + "          <a href=\"{1}\">" + Environment.NewLine + "            <img src=\"{2}design/skins/{3}/tabs/cLG.gif\" alt=\"\" /><img src=\"{2}design/skins/{3}/tabs/AU.gif\" alt=\"\" /><span class=\"tab\">{4}</span><img src=\"{2}design/skins/{3}/tabs/cRG.gif\" alt=\"\" />" + Environment.NewLine + "          </a>" + Environment.NewLine + "        </div>", table_of_contents, redirect_url, currentMode.Base_URL, htmlSkin.Base_Skin_Code, hide_toc);
                placeHolder.Controls.Add(menuStartLiteral);

                // Create the treeview
                treeView1 = new TreeView {CssClass = "SobekTocTreeView", ExpandDepth = 0, NodeIndent = 15};
                treeView1.SelectedNodeChanged += treeView1_SelectedNodeChanged;

                // load the table of contents in the tree
                Create_TreeView_From_Divisions(treeView1);

                // Add the tree view to the placeholder
                placeHolder.Controls.Add(treeView1);

                // Set flag that something was added to the nav bar
                Nav_Bar_Menu_Section_Added = true;
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
                    HttpContext.Current.Response.Redirect(currentMode.Redirect_URL());
            }
        }

        #endregion

        /// <summary> Adds the internal header HTML for this specific HTML writer </summary>
        /// <param name="Output"> Stream to which to write the HTML for the internal header information </param>
        /// <param name="Current_User"> Currently logged on user, to determine specific rights </param>
        public override void Add_Internal_Header_HTML(TextWriter Output, User_Object Current_User)
        {
            // If this is for a fragment, do nothing
            if (!String.IsNullOrEmpty(currentMode.Fragment))
                return;

            Output.WriteLine("  <table cellspacing=\"0\" id=\"internalheader_item\">");
            Output.WriteLine("    <tr height=\"30px\">");
            Output.WriteLine("      <td align=\"left\">");
            Output.WriteLine("          <button title=\"Hide Internal Header\" class=\"intheader_button hide_intheader_button2\" onclick=\"return hide_internal_header();\"></button>");
            Output.WriteLine("      </td>");
            if (currentItem.METS_Header.RecordStatus_Enum == METS_Record_Status.BIB_LEVEL)
            {
                Output.WriteLine("      <td><center><h2>" + currentItem.BibID + "</h2></center></td>");
            }
            else
            {
                Output.WriteLine("      <td><center><h2><a href=\"" + currentMode.Base_URL + currentItem.BibID + "/00000\">" + currentItem.BibID + "</a> : " + currentItem.VID + "</h2></center></td>");
            }

            Add_Internal_Header_Search_Box(Output);
            Output.WriteLine("    </tr>");

            if (currentItem.METS_Header.RecordStatus_Enum != METS_Record_Status.BIB_LEVEL)
            {
                Output.WriteLine("    <tr height=\"40px\">");
                Output.WriteLine("      <td colspan=\"3\" align=\"center\" valign=\"middle\">");

                // Should we add ability to edit this item to the quick links?
                bool allow_access_change = false;
                if (userCanEditItem)
                {
                    // Add ability to edit metadata for this item
                    currentMode.Mode = Display_Mode_Enum.My_Sobek;
                    currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Edit_Item_Metadata;
                    currentMode.My_Sobek_SubMode = "1";
                    Output.WriteLine("          <button title=\"Edit Metadata\" class=\"intheader_button edit_metadata_button\" onclick=\"window.location.href='" + currentMode.Redirect_URL() + "';return false;\"></button>");

                    // Add ability to edit behaviors for this item
                    currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Edit_Item_Behaviors;
                    currentMode.My_Sobek_SubMode = "1";
                    Output.WriteLine("          <button title=\"Edit Behaviors\" class=\"intheader_button edit_behaviors_button\" onclick=\"window.location.href='" + currentMode.Redirect_URL() + "';return false;\"></button>");
                    currentMode.Mode = Display_Mode_Enum.Item_Display;


                    // Add ability to edit behaviors for this item
                    string currentViewerCode = currentMode.ViewerCode;
                    currentMode.ViewerCode = "qc";
                    Output.WriteLine("          <button title=\"Perform Quality Control\" class=\"intheader_button qualitycontrol_button\" onclick=\"window.location.href='" + currentMode.Redirect_URL() + "';return false;\"></button>");
                    currentMode.Mode = Display_Mode_Enum.Item_Display;
                    currentMode.ViewerCode = currentViewerCode;

                    // Check if this item is DARK first
                    if (currentItem.Behaviors.Dark_Flag)
                    {
                        Output.WriteLine("          <button title=\"Dark Resource\" class=\"intheader_button dark_resource_button_fixed\" onclick=\"return false;\"></button>");
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
                                                     ? "          <button title=\"Change Access Restriction\" class=\"intheader_button public_resource_button\" onclick=\"open_access_restrictions(); return false;\"></button>"
                                                     : "          <button title=\"Change Access Restriction\" class=\"intheader_button restricted_resource_button\" onclick=\"open_access_restrictions(); return false;\"></button>");
                            }
                            else
                            {
                                Output.WriteLine(currentItem.Behaviors.IP_Restriction_Membership == 0
                                                     ? "          <button title=\"Public Resource\" class=\"intheader_button public_resource_button_fixed\" onclick=\"return false;\"></button>"
                                                     : "          <button title=\"IP Restriced Resource\" class=\"intheader_button restricted_resource_button_fixed\" onclick=\"return false;\"></button>");
                            }
                        }
                        else
                        {
                            allow_access_change = true;
                            Output.WriteLine("          <button title=\"Change Access Restriction\" class=\"intheader_button private_resource_button\" onclick=\"open_access_restrictions(); return false;\"></button>");
                        }
                    }
                }
                else
                {
                    // Check if this item is DARK first
                    if (currentItem.Behaviors.Dark_Flag)
                    {
                        Output.WriteLine("          <button title=\"Dark Resource\" class=\"intheader_button dark_resource_button_fixed\" onclick=\"return false;\"></button>");
                    }
                    else
                    {
                        // Still show that the item is public, private, restricted
                        if (currentItem.Behaviors.IP_Restriction_Membership > 0)
                        {
                            Output.WriteLine("          <button title=\"IP Restriced Resource\" class=\"intheader_button restricted_resource_button_fixed\" onclick=\"return false;\"></button>");
                        }
                        if (currentItem.Behaviors.IP_Restriction_Membership == 0)
                        {
                            Output.WriteLine("          <button title=\"Public Resource\" class=\"intheader_button public_resource_button_fixed\" onclick=\"return false;\"></button>");
                        }
                        if (currentItem.Behaviors.IP_Restriction_Membership < 0)
                        {
                            Output.WriteLine("          <button title=\"Private Resource\" class=\"intheader_button private_resource_button_fixed\" onclick=\"return false;\"></button>");
                        }
                    }
                }

                string currentViewCode = currentMode.ViewerCode;
                currentMode.ViewerCode = "milestones";
                Output.WriteLine("          <button title=\"View Work Log\" class=\"intheader_button view_worklog_button\" onclick=\"window.location.href='" + currentMode.Redirect_URL() + "';return false;\"></button>");
                currentMode.ViewerCode = currentViewCode;

                // Add ability to edit behaviors for this item
                if (userCanEditItem)
                {
                    currentMode.Mode = Display_Mode_Enum.My_Sobek;
                    currentMode.My_Sobek_Type = My_Sobek_Type_Enum.File_Management;
                    Output.WriteLine("          <button title=\"Manage Files\" class=\"intheader_button manage_files_button\" onclick=\"window.location.href='" + currentMode.Redirect_URL() + "';return false;\"></button>");
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
                    Output.WriteLine("    <tr align=\"center\" height=\"14px\">");
                    Output.WriteLine("      <td colspan=\"3\">");
                    Output.WriteLine("        <center>");
                    Output.WriteLine("        <table id=\"internal_notes_div\">");
                    Output.WriteLine("          <tr align=\"left\" height=\"14px\">");
                    Output.WriteLine("            <td class=\"intheader_label\">COMMENTS:</td>");
                    Output.WriteLine("            <td>");
                    Output.WriteLine("              <textarea rows=\"" + rows + "\" cols=\"" + actualCols + "\" name=\"intheader_internal_notes\" id=\"intheader_internal_notes\" class=\"intheader_comments_input\" onfocus=\"javascript:textbox_enter('intheader_internal_notes','intheader_comments_input_focused')\" onblur=\"javascript:textbox_leave('intheader_internal_notes','intheader_comments_input')\">" + HttpUtility.HtmlEncode(currentItem.Tracking.Internal_Comments) + "</textarea>");
                    Output.WriteLine("            </td>");
                    Output.WriteLine("            <td>");
                    Output.WriteLine("              <button title=\"Save new internal comments\" class=\"intheader_button intheader_save_button\" onclick=\"save_internal_notes(); return false;\"></button>");
                    Output.WriteLine("            </td>");
                    Output.WriteLine("          </tr>");
                    Output.WriteLine("        </table>");
                    Output.WriteLine("        </center>");
                    Output.WriteLine("      </td>");
                    Output.WriteLine("    </tr>");
                }
                else
                {
                    const int rows = 1;
                    const int actualCols = 80;

                    // Add the internal comments row ( hidden content initially )
                    Output.WriteLine("    <tr align=\"center\" height=\"14px\">");
                    Output.WriteLine("      <td colspan=\"2\">");
                    Output.WriteLine("        <center>");
                    Output.WriteLine("        <table id=\"internal_notes_div\">");
                    Output.WriteLine("          <tr align=\"left\" height=\"14px\">");
                    Output.WriteLine("            <td class=\"intheader_label\">COMMENTS:</td>");
                    Output.WriteLine("            <td>");
                    Output.WriteLine("              <textarea readonly=\"readonly\" rows=\"" + rows + "\" cols=\"" + actualCols + "\" name=\"intheader_internal_notes\" id=\"intheader_internal_notes\" class=\"intheader_comments_input\" onfocus=\"javascript:textbox_enter('intheader_internal_notes','intheader_comments_input_focused')\" onblur=\"javascript:textbox_leave('intheader_internal_notes','intheader_comments_input')\">" + HttpUtility.HtmlEncode(currentItem.Tracking.Internal_Comments) + "</textarea>");
                    Output.WriteLine("            </td>");
                    Output.WriteLine("          </tr>");
                    Output.WriteLine("        </table>");
                    Output.WriteLine("        </center>");
                    Output.WriteLine("      </td>");
                    Output.WriteLine("    </tr>");
                }

          
                if (allow_access_change)
                {
                    // Add the access restriction row ( hidden content initially )
                    Output.WriteLine("    <tr align=\"center\">");
                    Output.WriteLine("      <td colspan=\"3\">");
                    Output.WriteLine("        <center>");
                    Output.WriteLine("        <table id=\"access_restrictions_div\" style=\"display:none;\">");
                    Output.WriteLine("          <tr align=\"left\">");
                    Output.WriteLine("            <td valign=\"top\" class=\"intheader_label\">SET ACCESS RESTRICTIONS: </td>");
                    Output.WriteLine("            <td>");
                    Output.WriteLine("              <button title=\"Make item public\" class=\"intheader_button public_resource_button\" onclick=\"set_item_access('public'); return false;\"></button>");
                    Output.WriteLine("              <button title=\"Add IP restriction to this item\" class=\"intheader_button restricted_resource_button\" onclick=\"set_item_access('restricted'); return false;\"></button>");
                    Output.WriteLine("              <button title=\"Make item private\" class=\"intheader_button private_resource_button\" onclick=\"set_item_access('private'); return false;\"></button>");

                    // Should we add ability to delete this item?
                    if ((currentUser.Is_System_Admin) || ( currentUser.UserName.ToLower() == "neldamaxs"))
                    {
                        // Determine the delete URL
                        currentMode.Mode = Display_Mode_Enum.My_Sobek;
                        currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Delete_Item;
                        string delete_url = currentMode.Redirect_URL();
                        currentMode.Mode = Display_Mode_Enum.Item_Display;
                        Output.WriteLine("              <button title=\"Delete this item\" class=\"intheader_button delete_button\" onclick=\"if(confirm('Delete this item completely?')) window.location.href = '" + delete_url + "'; return false;\"></button>");
                    }

                    Output.WriteLine("            </td>");
                    Output.WriteLine("            <td valign=\"top\">");
                    Output.WriteLine("              <button title=\"Cancel changes\" class=\"intheader_button intheader_cancel_button\" onclick=\"open_access_restrictions(); return false;\"></button>");
                    Output.WriteLine("            </td>");
                    Output.WriteLine("          </tr>");
                    Output.WriteLine("        </table>");
                    Output.WriteLine("        </center>");
                    Output.WriteLine("      </td>");
                    Output.WriteLine("    </tr>");
                }
            }
            else
            {

                if (userCanEditItem)
                {
                    Output.WriteLine("    <tr height=\"45px\">");
                    Output.WriteLine("      <td colspan=\"3\" align=\"center\" valign=\"middle\">");

                    // Add ability to edit behaviors for this item group
                    currentMode.Mode = Display_Mode_Enum.My_Sobek;
                    currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Edit_Group_Behaviors;
                    currentMode.My_Sobek_SubMode = "1";
                    Output.WriteLine("          <button title=\"Edit Behaviors\" class=\"intheader_button edit_behaviors_button\" onclick=\"window.location.href='" + currentMode.Redirect_URL() + "';return false;\"></button>");

                    // Add ability to add a new item/volume to this title
                    currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Group_Add_Volume;
                    Output.WriteLine("          <button title=\"Add Volume\" class=\"intheader_button add_volume_button\" onclick=\"window.location.href='" + currentMode.Redirect_URL() + "';return false;\"></button>");

                    // Add ability to auto-fill a number of new items/volumes to this title
                    currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Group_AutoFill_Volumes;
                    Output.WriteLine("          <button title=\"Auto-Fill Volumes\" class=\"intheader_button autofill_volumes_button\" onclick=\"window.location.href='" + currentMode.Redirect_URL() + "';return false;\"></button>");

                    // Add ability to edit the serial hierarchy online
                    currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Edit_Group_Serial_Hierarchy;
                    Output.WriteLine("          <button title=\"Edit Serial Hierarchy\" class=\"intheader_button serial_hierarchy_button\" onclick=\"window.location.href='" + currentMode.Redirect_URL() + "';return false;\"></button>");

                    // Add ability to mass update the items behaviors under this title
                    currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Group_Mass_Update_Items;
                    Output.WriteLine("          <button title=\"Mass Update Volumes\" class=\"intheader_button mass_update_button\" onclick=\"window.location.href='" + currentMode.Redirect_URL() + "';return false;\"></button>");

                    currentMode.Mode = Display_Mode_Enum.Item_Display;

                    Output.WriteLine("      </td>");
                    Output.WriteLine("    </tr>");
                }

            }

            Output.WriteLine("  </table>");
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
        /// <remarks> This begins writing this page, starting with the left navigation bar up to the (possible) Table of Contents tree control</remarks>
        public override bool Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Item_HtmlSubwriter.Write_HTML", "Do nothing");
            return true;
        }

        /// <summary> Writes the HTML generated by this item html subwriter directly to the response stream </summary>
        /// <param name="Output"> Stream to which to write the HTML for this subwriter </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Value indicating if html writer should finish the page immediately after this, or if there are other controls or routines which need to be called first </returns>
        /// <remarks> This continues writing this item from finishing the left navigation bar to the popup forms to the page navigation controls at the top of the item viewer's main area</remarks>
        public bool Write_Additional_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            // If this is for a fragment, do nothing
            if (!String.IsNullOrEmpty(currentMode.Fragment))
                return false;

            Tracer.Add_Trace("Item_HtmlSubwriter.Write_Additional_HTML", "Rendering HTML ( finish left navigation bar, begin main viewer section )");

            // Add google map stuff if that is selected
            if ((PageViewer != null) && (PageViewer.ItemViewer_Type == ItemViewer_Type_Enum.Google_Map))
            {
                ((Google_Map_ItemViewer)PageViewer).Add_Google_Map_Scripts(Output, Tracer);
            }

            // If this is the page turner viewer, don't draw anything
            if ((PageViewer != null) && (PageViewer.ItemViewer_Type == ItemViewer_Type_Enum.GnuBooks_PageTurner))
            {
                return true;
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
                        Output.WriteLine("      <div class=\"floating-toc\">");
                        Output.WriteLine("      <ul class=\"SobekNavBarMenu\">");
                        Output.WriteLine("        <li class=\"SobekNavBarHeader\">TABLE OF CONTENTS &nbsp; &nbsp; &nbsp; <span style=\"color:#eeeeee\"><a href=\"#top\" title=\"Return to the top of this document\"><img src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/up_arrow.gif\" /></a></span></li>");

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
            }

            if ( should_left_navigation_bar_be_shown )
            {
                Output.WriteLine("\t<div id=\"itemwordmarks\">");

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
                Output.WriteLine("</div>");
                Output.WriteLine();
            }


            // Begin the document display portion
            Output.WriteLine("<!-- Begin the main item viewing area -->");
            if ((PageViewer == null) || (PageViewer.Viewer_Width < 0))
            {
                if ((PageViewer != null) && (PageViewer.Viewer_Width == -100))
                {
                    Output.WriteLine("<table id=\"SobekDocumentDisplay2\" >");
                }
                else
                {
                    Output.WriteLine("<table id=\"SobekDocumentDisplay2\" >");
                }
            }
            else
            {
                Output.WriteLine("<table id=\"SobekDocumentDisplay\" style=\"width:" + PageViewer.Viewer_Width + "px;\" >");
            }


            // If this item is PRIVATE or DARK, show information to that affect here
            if (currentItem.Behaviors.Dark_Flag)
            {
                Output.WriteLine("\t<tr id=\"itemrestrictedrow\">");
                Output.WriteLine("\t\t<td>");
                Output.WriteLine("\t\t\t<span style=\"font-size:larger; font-weight: bold;\">DARK ITEM</span>");
                Output.WriteLine("\t\t</td>");
                Output.WriteLine("\t</tr>");
            }
            else if ( currentItem.Behaviors.IP_Restriction_Membership < 0 )
            {
                Output.WriteLine("\t<tr id=\"itemrestrictedrow\">");
                Output.WriteLine("\t\t<td>");
                Output.WriteLine("\t\t\t<span style=\"font-size:larger; font-weight: bold;\">PRIVATE ITEM</span>");
                Output.WriteLine("\t\t\tDigitization of this item is currently in progress.");
                Output.WriteLine("\t\t</td>");
                Output.WriteLine("\t</tr>");
            }

            // Add navigation row here (buttons and viewer specific)
            if (PageViewer != null)
            {
                string navigation_row = PageViewer.NavigationRow;
                if ((PageViewer.PageCount != 1) || (navigation_row.Length > 0))
                {
                    Output.WriteLine("\t<tr>");
                    Output.WriteLine("\t\t<td>");

                    // ADD THE Viewer Nav Bar part from the viewer
                    if (navigation_row.Length > 0)
                    {
                        Output.WriteLine(navigation_row);
                    }

                    // ADD NAVIGATION BUTTONS
                    if (PageViewer.PageCount != 1)
                    {
                        string go_to = "Go To:";
                        string first_page = "First Page";
                        string previous_page = "Previous Page";
                        string next_page = "Next Page";
                        string last_page = "Last Page";

                        if (currentMode.Language == Web_Language_Enum.Spanish)
                        {
                            go_to = "Ir a:";
                            first_page = "Primera Página";
                            previous_page = "Página Anterior";
                            next_page = "Página Siguiente";
                            last_page = "Última Página";
                        }

                        if (currentMode.Language == Web_Language_Enum.French)
                        {
                            go_to = "Aller à:";
                            first_page = "Première Page";
                            previous_page = "Page Précédente";
                            next_page = "Page Suivante";
                            last_page = "Dernière Page";
                        }

                        string language_suffix = currentMode.Language_Code;
                        if (language_suffix.Length > 0)
                            language_suffix = "_" + language_suffix;

                        Output.WriteLine("\t\t\t<div class=\"SobekPageNavBar\">");

                        // Get the URL for the first and previous buttons
                        string firstButtonURL = PageViewer.First_Page_URL;
                        string prevButtonURL = PageViewer.Previous_Page_URL;

                        // Only continue if there is an item and mode, and there is previous pages to go to
                        if ((PageViewer.Current_Page > 1) && ((firstButtonURL.Length > 0) || (prevButtonURL.Length > 0)))
                        {
                            Output.WriteLine("\t\t\t\t<span class=\"leftButtons\">");
                            Output.WriteLine("\t\t\t\t\t<a href=\"" + firstButtonURL + "\"><img src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/first_button" + language_suffix + ".gif\" alt=\"" + first_page + "\" /></a>&nbsp;");
                            Output.WriteLine("\t\t\t\t\t<a href=\"" + prevButtonURL + "\"><img src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/previous_button" + language_suffix + ".gif\" alt=\"" + previous_page + "\" /></a>");
                            Output.WriteLine("\t\t\t\t</span>");
                        }

                        // Get the URL for the first and previous buttons
                        string lastButtonURL = PageViewer.Last_Page_URL;
                        string nextButtonURL = PageViewer.Next_Page_URL;

                        // Only continue if there is an item and mode, and there is previous pages to go to
                        if ((PageViewer.Current_Page < PageViewer.PageCount) && ((lastButtonURL.Length > 0) || (nextButtonURL.Length > 0)))
                        {
                            Output.WriteLine("\t\t\t\t<span class=\"rightButtons\">");
                            Output.WriteLine("\t\t\t\t\t<a href=\"" + nextButtonURL + "\"><img src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/next_button" + language_suffix + ".gif\" alt=\"" + next_page + "\" /></a>&nbsp;");
                            Output.WriteLine("\t\t\t\t\t<a href=\"" + lastButtonURL + "\"><img src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/last_button" + language_suffix + ".gif\" alt =\"" + last_page + "\" /></a>");
                            Output.WriteLine("\t\t\t\t</span>");
                        }

                        // Show a pageselector, if one was selected
                        switch ( PageViewer.Page_Selector )
                        {
                            case ItemViewer_PageSelector_Type_Enum.DropDownList:
                                string[] pageNames = PageViewer.Go_To_Names;
                                if (pageNames.Length > 0)
                                {

                                    // Determine if these page names are very long at all
                                    if (pageNames.Any(thisName => thisName.Length > 25))
                                    {
                                        // Long page names, so move the Go To: to the next line (new div)
                                        Output.WriteLine("\t\t\t</div>");
                                        Output.WriteLine("\t\t\t<div class=\"SobekPageNavBar2\">");
                                    }

                                    Output.WriteLine("\t\t\t\t" + go_to);
                                    string orig_viewercode = currentMode.ViewerCode;
                                    string viewercode_only = currentMode.ViewerCode.Replace(currentMode.Page.ToString(), "");
                                    currentMode.ViewerCode = "XX1234567890XX";
                                    string url = currentMode.Redirect_URL();
                                    currentMode.ViewerCode = orig_viewercode;

                                    Output.WriteLine("\t\t\t\t<select id=\"page_select\" onchange=\"javascript:item_jump_sobekcm('" + url + "')\" name=\"page_select\">");

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
                                if (!String.IsNullOrEmpty(pageselectorhtml))
                                    Output.WriteLine(pageselectorhtml);
                                break;
                        }

                        Output.WriteLine("\t\t\t</div>");
                    }


                    Output.WriteLine("\t\t</td>");
                    Output.WriteLine("\t</tr>");
                }
            }

            Output.WriteLine("\t<tr>");

            // Add the HTML from the pageviewer
            // Add the main viewer section
            if (PageViewer != null)
            {
                // Was this to draw a fragment?
                if (String.IsNullOrEmpty(currentMode.Fragment))
                {
                    Tracer.Add_Trace("Html_MainWriter.Write_Additional_HTML", "Allowing page viewer to add main viewer section to <i>mainPlaceHolder</i>");
                    PageViewer.Write_Main_Viewer_Section(Output, Tracer);

                    //if (PageViewer is Citation_ItemViewer)
                    //    ((Citation_ItemViewer) PageViewer).Write_Main_Viewer_Section(Output, Tracer);
                    //else
                    //{
                    //    PageViewer.Write_Main_Viewer_Section(Output, Tracer);
                    //}
                }
            }

            return true;
        }

        /// <summary> Performs the final HTML writing which completes the item table and adds the final page navigation buttons at the bottom of the page </summary>
        /// <param name="placeHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form, widely used throughout the application</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public void Add_Main_Viewer_Section(PlaceHolder placeHolder, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Item_HtmlSubwriter.Add_Main_Viewer_Section", "Rendering HTML ( add page view and finish the main viewer section )");

            // Add the main viewer section
            if ( PageViewer != null)
            {
                // Was this to draw a fragment?
                if ( !String.IsNullOrEmpty(currentMode.Fragment))
                {
                    switch( currentMode.Fragment )
                    {
                        case "printform":
                            PrintForm_Fragment_ItemViewer printform_viewer = new PrintForm_Fragment_ItemViewer(currentPage, PageViewer);
                            printform_viewer.CurrentItem = currentItem;
                            printform_viewer.CurrentMode = currentMode;
                            printform_viewer.Translator = PageViewer.Translator;
                            printform_viewer.CurrentUser = currentUser;
                            printform_viewer.Add_Main_Viewer_Section(placeHolder, Tracer);
                            break;

                        case "shareform":
                            Share_Fragment_ItemViewer share_viewer = new Share_Fragment_ItemViewer();
                            share_viewer.CurrentItem = currentItem;
                            share_viewer.CurrentMode = currentMode;
                            share_viewer.Translator = PageViewer.Translator;
                            share_viewer.CurrentUser = currentUser;
                            share_viewer.Add_Main_Viewer_Section(placeHolder, Tracer);
                            break;

                        case "addform":
                            AddRemove_Fragment_ItemViewer add_viewer = new AddRemove_Fragment_ItemViewer();
                            add_viewer.CurrentItem = currentItem;
                            add_viewer.CurrentMode = currentMode;
                            add_viewer.Translator = PageViewer.Translator;
                            add_viewer.CurrentUser = currentUser;
                            add_viewer.Add_Main_Viewer_Section(placeHolder, Tracer);
                            break;

                        case "sendform":
                            Send_Fragment_ItemViewer send_viewer = new Send_Fragment_ItemViewer();
                            send_viewer.CurrentItem = currentItem;
                            send_viewer.CurrentMode = currentMode;
                            send_viewer.Translator = PageViewer.Translator;
                            send_viewer.CurrentUser = currentUser;
                            send_viewer.Add_Main_Viewer_Section(placeHolder, Tracer);
                            break;


                    }

                    return;
                }
                else
                {
                    Tracer.Add_Trace("Html_MainWriter.Add_Controls", "Allowing page viewer to add main viewer section to <i>mainPlaceHolder</i>");
                    PageViewer.Add_Main_Viewer_Section(placeHolder, Tracer);
                }
            }

            // If this is the page turner viewer, don't draw anything else
            if ((PageViewer != null) && (PageViewer.ItemViewer_Type == ItemViewer_Type_Enum.GnuBooks_PageTurner))
            {
                return;
            }
            

            StringBuilder buildResult = new StringBuilder();

            buildResult.AppendLine("\t</tr>");

            if ((PageViewer != null) && (PageViewer.PageCount != 1))
            {
                buildResult.AppendLine("\t<tr>");
                buildResult.AppendLine("\t\t<td>");

                // ADD NAVIGATION BUTTONS
                if (PageViewer.PageCount != 1)
                {
                    string first_page = "First Page";
                    string previous_page = "Previous Page";
                    string next_page = "Next Page";
                    string last_page = "Last Page";

                    if (currentMode.Language == Web_Language_Enum.Spanish)
                    {
                        first_page = "Primera Página";
                        previous_page = "Página Anterior";
                        next_page = "Página Siguiente";
                        last_page = "Última Página";
                    }

                    if (currentMode.Language == Web_Language_Enum.French)
                    {
                        first_page = "Première Page";
                        previous_page = "Page Précédente";
                        next_page = "Page Suivante";
                        last_page = "Dernière Page";
                    }

                    string language_suffix = currentMode.Language_Code;
                    if (language_suffix.Length > 0)
                        language_suffix = "_" + language_suffix;

                    buildResult.AppendLine("\t\t\t<div class=\"SobekPageNavBar\">");

                    // Get the URL for the first and previous buttons
                    string firstButtonURL = PageViewer.First_Page_URL;
                    string prevButtonURL = PageViewer.Previous_Page_URL;

                    // Only continue if there is an item and mode, and there is previous pages to go to
                    if ((PageViewer.Current_Page > 1) && ((firstButtonURL.Length > 0) || (prevButtonURL.Length > 0)))
                    {
                        buildResult.AppendLine("              <span class=\"leftButtons\">");
                        buildResult.AppendLine("                <a href=\"" + firstButtonURL + "\"><img src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/first_button" + language_suffix + ".gif\" alt=\"" + first_page + "\" /></a>&nbsp;");
                        buildResult.AppendLine("                <a href=\"" + prevButtonURL + "\"><img src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/previous_button" + language_suffix + ".gif\" alt=\"" + previous_page + "\" /></a>");
                        buildResult.AppendLine("              </span>");
                    }

                    // Get the URL for the first and previous buttons
                    string lastButtonURL = PageViewer.Last_Page_URL;
                    string nextButtonURL = PageViewer.Next_Page_URL;

                    // Only continue if there is an item and mode, and there is previous pages to go to
                    if ((PageViewer.Current_Page < PageViewer.PageCount) && ((lastButtonURL.Length > 0) || (nextButtonURL.Length > 0)))
                    {
                        buildResult.AppendLine("              <span class=\"rightButtons\">");
                        buildResult.AppendLine("                <a href=\"" + nextButtonURL + "\"><img src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/next_button" + language_suffix + ".gif\" alt=\"" + next_page + "\" /></a>&nbsp;");
                        buildResult.AppendLine("                <a href=\"" + lastButtonURL + "\"><img src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/last_button" + language_suffix + ".gif\" alt =\"" + last_page + "\" /></a>");
                        buildResult.AppendLine("              </span>");
                    }

                    // Create the page selection if that is the type to display.  This is where it is actually
                    // built as well, althouogh it is subsequently used further up the page
                    if (PageViewer.Page_Selector == ItemViewer_PageSelector_Type_Enum.PageLinks)
                    {
                        StringBuilder pageLinkBuilder = new StringBuilder();

                        //Get the total page count
                        int num_of_pages = PageViewer.PageCount;
                        string[] page_urls = PageViewer.Go_To_Names;

                        pageLinkBuilder.AppendLine("\t\t\t\t<div id=\"pageNumbersBottom\" style=\"float:center; font-weight: bold\">");

                        //Display the first, last, current page numbers, and 2 pages before and after the current page
                        if (num_of_pages <= 7 && num_of_pages > 1)
                        {
                            for (int i = 1; i <= num_of_pages; i++)
                            {
                                if (i == PageViewer.Current_Page)
                                    pageLinkBuilder.AppendLine("\t\t\t\t\t<a class=\"thumbnailPageNumberCurrent\" href=\"" + page_urls[i - 1] + "\">" + i + "</a>&nbsp;");
                                else
                                    pageLinkBuilder.AppendLine("\t\t\t\t\t<a class=\"thumbnailPageNumber\" href=\"" + page_urls[i - 1] + "\">" + i + "</a>&nbsp;");
                            }
                        }
                        else if (num_of_pages > 7)
                        {
                            if (PageViewer.Current_Page > 4 && PageViewer.Current_Page < num_of_pages - 3)
                            {
                                pageLinkBuilder.AppendLine("\t\t\t\t\t<a class=\"thumbnailPageNumber\" href=\"" + page_urls[0] + "\">" + 1 + "</a>" + "...");
                                for (int i = PageViewer.Current_Page - 2; i <= PageViewer.Current_Page + 2; i++)
                                {
                                    if (i == PageViewer.Current_Page)
                                        pageLinkBuilder.AppendLine("\t\t\t\t\t<a class=\"thumbnailPageNumberCurrent\" href=\"" + page_urls[i - 1] + "\">" + i + "</a>&nbsp;");
                                    else
                                        pageLinkBuilder.AppendLine("\t\t\t\t\t<a class=\"thumbnailPageNumber\" href=\"" + page_urls[i - 1] + "\">" + i + "</a>&nbsp;");
                                }
                                pageLinkBuilder.AppendLine("\t\t\t\t\t..." + "<a class=\"thumbnailPageNumber\" href=\"" + page_urls[page_urls.Length - 1] + "\">" + num_of_pages + "</a>");
                            }

                            else if (PageViewer.Current_Page <= 4 && PageViewer.Current_Page < num_of_pages - 3)
                            {
                                for (int i = 1; i <= (PageViewer.Current_Page + 2); i++)
                                {
                                    if (i == PageViewer.Current_Page)
                                        pageLinkBuilder.AppendLine("\t\t\t\t\t<a class=\"thumbnailPageNumberCurrent\" href=\"" + page_urls[i - 1] + "\">" + i + "</a>&nbsp;");
                                    else
                                        pageLinkBuilder.AppendLine("\t\t\t\t\t<a class=\"thumbnailPageNumber\" href=\"" + page_urls[i - 1] + "\">" + i + "</a>&nbsp;");
                                }
                                pageLinkBuilder.AppendLine("\t\t\t\t\t..." + "<a href=\"" + page_urls[page_urls.Length - 1] + "\">" + num_of_pages + "</a>");
                            }

                            else if (PageViewer.Current_Page > 4 && PageViewer.Current_Page >= num_of_pages - 3)
                            {
                                pageLinkBuilder.AppendLine("\t\t\t\t\t<a class=\"thumbnailPageNumber\" href=\"" + page_urls[0] + "\">" + 1 + "</a>" + "...");
                                for (int i = PageViewer.Current_Page - 2; i <= num_of_pages; i++)
                                {
                                    if (i == PageViewer.Current_Page)
                                        pageLinkBuilder.AppendLine("\t\t\t\t\t<a class=\"thumbnailPageNumberCurrent\" href=\"" + page_urls[i - 1] + "\">" + i + "</a>&nbsp;");
                                    else
                                        pageLinkBuilder.AppendLine("\t\t\t\t\t<a class=\"thumbnailPageNumber\" href=\"" + page_urls[i - 1] + "\">" + i + "</a>&nbsp;");
                                }

                            }
                        }

                        pageLinkBuilder.AppendLine("\t\t\t\t</div>");

                        pageselectorhtml = pageLinkBuilder.ToString();
                        buildResult.AppendLine(pageselectorhtml);

                    }


                    buildResult.AppendLine("\t\t\t</div>");
                }

                buildResult.AppendLine("\t\t</td>");
                buildResult.AppendLine("\t</tr>");
            }

            if (PageViewer != null && ((currentItem.Behaviors.CheckOut_Required) && (PageViewer.ItemViewer_Type != ItemViewer_Type_Enum.Checked_Out)))
            {
                buildResult.AppendLine("<tr><td><span style=\"color:gray; font-size: 0.8em\">This item contains copyrighted material and is reserved for single (fair) use.  Once you finish working with this item,<br />it will return to the digital stacks in fifteen minutes for another patron to use.<br /><br /></span></td></tr>");
            }
            buildResult.AppendLine("</table>");

            // Add a spot for padding
            buildResult.AppendLine();
            buildResult.AppendLine("<!-- Division is used to add extra bottom padding, if the left nav bar is taller than the item viewer -->");
            buildResult.AppendLine("<div id=\"itemviewerbottompadding\"></div>");
            buildResult.AppendLine();

            // None of the sharing options are available if the user is restricted from this item
            // or if we are generating this as a static page source for robots
            if ((!itemRestrictedFromUserByIp) && (!itemCheckedOutByOtherUser) && (!currentMode.Is_Robot))
            {
                // Add the hidden field
                buildResult.AppendLine("<!-- Hidden field is used for postbacks to indicate what to save and reset -->");
                buildResult.AppendLine("<input type=\"hidden\" id=\"item_action\" name=\"item_action\" value=\"\" />");
                buildResult.AppendLine();

                // Add the scripts needed
                buildResult.AppendLine("<!-- Add references to the jquery and sobekcm javascript files -->");

                //I took these out mainly because we updated jquery and jquery ui to include everything. (also they were interfering with google maps coordinate viewer)
                //buildResult.AppendLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/jquery/jquery-1.6.2.min.js\"></script>");
                //buildResult.AppendLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/jquery/jquery-ui-1.8.16.custom.min.js\"></script>");

                buildResult.AppendLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_form.js\" ></script>");
                buildResult.AppendLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_item.js\" ></script>");
                buildResult.AppendLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/superfish/hoverIntent.js\" ></script>");
                buildResult.AppendLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/superfish/superfish.js\" ></script>");
                buildResult.AppendLine();

                // Initialize the navigation menu

            }


            Literal closeTableLiteral = new Literal {Text = buildResult.ToString()};
            placeHolder.Controls.Add(closeTableLiteral);

        }

        /// <summary> [DEPRECATED] Spot to write any final HTML to the response stream (currently unused) </summary>
        /// <param name="Output"> Stream to which to write the HTML for this subwriter </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Always returns TRUE</returns>
        public bool Write_Final_HTML(TextWriter Output, Custom_Tracer Tracer )
        {
            Tracer.Add_Trace("Item_HtmlSubwriter.Write_Final_Html", "Do nothing");
            return true;
        }

        private void recurse_through_tree( Division_TreeNode parentNode, TreeNode parentViewNode, List<TreeNode> nodes, List<TreeNode> selectedNodes, List<TreeNode> pathNodes, ref int sequence )
        {
            foreach (abstract_TreeNode absNode in parentNode.Nodes )
            {
                if ( absNode.Page )
                {
                    sequence++;

                    foreach( TreeNode thisNode in nodes )
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
                    Division_TreeNode divNode = (Division_TreeNode) absNode;
                    TreeNode treeViewNode = new TreeNode
                                                { Text = string.Format("<span class=\"SobekTocTreeViewItem\" Title='{0}'>{1}</span>", divNode.Display_Label, divNode.Display_Short_Label) };
                    parentViewNode.ChildNodes.Add( treeViewNode );
                    nodes.Add(treeViewNode);
                    List<TreeNode> pathNodes2 = new List<TreeNode> {treeViewNode};
                    recurse_through_tree(divNode, treeViewNode, nodes, selectedNodes, pathNodes2, ref sequence );
                }
            }
        }

        /// <summary> Populates the tree view with the divisions from the current digital resource item </summary>
        /// <param name="treeViewArg"> Tree view control to populate </param>
        public void Create_TreeView_From_Divisions(TreeView treeViewArg )
        {
            tocSelectedComplete = false;

            // Get the current mode page
            List<TreeNode> nodes = new List<TreeNode>();
            List<TreeNode> selectedNodes = new List<TreeNode>();

            int sequence = 0;
            foreach (abstract_TreeNode absNode in currentItem.Divisions.Physical_Tree.Roots)
            {
                Division_TreeNode divNode = (Division_TreeNode) absNode;
                TreeNode treeViewNode = new TreeNode
                                            { Text =string.Format( "<span class=\"SobekTocTreeViewItem\" Title=\"{0}\">{1}</span>", divNode.Display_Label, divNode.Display_Short_Label) };
                treeViewArg.Nodes.Add( treeViewNode );
                nodes.Add(treeViewNode);
                List<TreeNode> pathNodes = new List<TreeNode> {treeViewNode};
                recurse_through_tree(divNode, treeViewNode, nodes, selectedNodes, pathNodes, ref sequence );
            }

            foreach (TreeNode selectedNode in selectedNodes)
            {
                selectedNode.Text = selectedNode.Text.Replace("SobekTocTreeViewItem", "SobekSelectedTocTreeViewItem");
                TreeNode selectedNodeExpander = selectedNode;
                while (selectedNodeExpander.Parent != null) 
                {
                    (selectedNodeExpander.Parent).Expand();
                    selectedNodeExpander = selectedNodeExpander.Parent;
                }
            }
        }

        /// <summary> Gets the collection of body attributes to be included 
        /// within the HTML body tag (usually to add events to the body) </summary>
        public override List<Tuple<string, string>> Body_Attributes
        {
            get
            {
                List<Tuple<string, string>> returnValue = new List<Tuple<string, string>>();

                // Add any viewer specific body attributes
                if (PageViewer != null)
                    PageViewer.Add_ViewerSpecific_Body_Attributes(returnValue);

                // Add default script attachments
                returnValue.Add(new Tuple<string, string>("onload", "itemwriter_load();"));
                returnValue.Add(new Tuple<string, string>("onresize", "itemwriter_load();"));

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

            // Write the style sheet to use 
            Output.WriteLine("  <link href=\"" + currentMode.Base_URL + "default/SobekCM_Item.css\" rel=\"stylesheet\" type=\"text/css\" title=\"standard\" />");

            // Add code for the GnuBooks page turner if that is the valid viewer
            if (PageViewer != null)
                PageViewer.Write_Within_HTML_Head(Output, Tracer);

            // Add a thumbnail to this item
            if (currentItem != null)
            {
                string image_src = currentMode.Base_URL + "/" + currentItem.Web.AssocFilePath + "/" + currentItem.Behaviors.Main_Thumbnail;

                Output.WriteLine("  <link rel=\"image_src\" href=\"" + image_src.Replace("\\", "/").Replace("//", "/") + "\" />");
            }
        }
    }
}
