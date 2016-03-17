#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using SobekCM.Core.ApplicationState;
using SobekCM.Core.BriefItem;
using SobekCM.Core.Client;
using SobekCM.Core.Configuration.Localization;
using SobekCM.Core.Items;
using SobekCM.Core.MemoryMgmt;
using SobekCM.Core.Navigation;
using SobekCM.Core.UI_Configuration;
using SobekCM.Core.Users;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Engine_Library.Database;
using SobekCM.Library.Database;
using SobekCM.Library.Email;
using SobekCM.Library.ItemViewer;
using SobekCM.Library.ItemViewer.Viewers;
using SobekCM.Library.UI;
using SobekCM.Resource_Object.Behaviors;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.HTML
{
    /// <summary> Item html subwriter renders views on a single digital resource </summary>
    /// <remarks> This class extends the <see cref="abstractHtmlSubwriter"/> abstract class. </remarks>
    public class Item_HtmlSubwriter : abstractHtmlSubwriter
    {
        #region Private class members 
        
        private bool itemCheckedOutByOtherUser;
        private readonly bool itemRestrictedFromUserByIp;
        private readonly int searchResultsCount;
        private readonly bool showToc;
        private readonly bool showZoomable;
        private bool tocSelectedComplete;
        private TreeView treeView1;
        private readonly bool userCanEditItem;
        private readonly List<HtmlSubwriter_Behaviors_Enum> behaviors;
        private string buttonsHtml;
        private string pageLinksHtml;
        private readonly string restriction_message;

        private readonly bool is_ead;
        private readonly bool is_bib_level;

        private BriefItemInfo currentItem;
        private SobekCM_Items_In_Title itemsInTitle;

        
        private iItemViewer pageViewer;
        private List<HtmlSubwriter_Behaviors_Enum> pageViewerBehaviors;

        #endregion

        #region Constructor(s)

        /// <summary> Constructor for a new instance of the Item_HtmlSubwriter class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        public Item_HtmlSubwriter( RequestCache RequestSpecificValues) : base(RequestSpecificValues)
        {
            showZoomable = (String.IsNullOrEmpty(UI_ApplicationCache_Gateway.Settings.Servers.JP2ServerUrl));

            searchResultsCount = 0;

            // Determine if the TOC should be shown
            showToc = false;
            if (HttpContext.Current.Session["Show TOC"] != null)
            {
                Boolean.TryParse(HttpContext.Current.Session["Show TOC"].ToString(), out showToc);
            }

            // Try to get the current item
            currentItem = SobekEngineClient.Items.Get_Item_Brief(RequestSpecificValues.Current_Mode.BibID, RequestSpecificValues.Current_Mode.VID, true, RequestSpecificValues.Tracer);
            
            // Ensure the UI portion has been configured for this user interface
            ItemViewer_Factory.Configure_Brief_Item_Viewers(currentItem);

            // Set some flags based on the resource type
            is_bib_level = (String.Compare(currentItem.Type, "BIB_LEVEL", StringComparison.OrdinalIgnoreCase) == 0);
            is_ead = (String.Compare(currentItem.Type, "EAD", StringComparison.OrdinalIgnoreCase) == 0);

            // Determine if this user can edit this item
            userCanEditItem = false;
            if (RequestSpecificValues.Current_User != null)
            {
                userCanEditItem = RequestSpecificValues.Current_User.Can_Edit_This_Item(currentItem.BibID, currentItem.Type, currentItem.Behaviors.Source_Institution_Aggregation, currentItem.Behaviors.Holding_Location_Aggregation, currentItem.Behaviors.Aggregation_Code_List );
            }

            // Check that this item is not checked out by another user
            itemCheckedOutByOtherUser = false;
            if (currentItem.Behaviors.Single_Use)
            {
                if (!Engine_ApplicationCache_Gateway.Checked_List.Check_Out(currentItem.Web.ItemID, HttpContext.Current.Request.UserHostAddress))
                {
                    itemCheckedOutByOtherUser = true;
                }
            }

            // Check to see if this is IP restricted
            restriction_message = String.Empty;
            if (currentItem.Behaviors.IP_Restriction_Membership > 0)
            {
                if (HttpContext.Current != null)
                {
                    int user_mask = (int)HttpContext.Current.Session["IP_Range_Membership"];
                    int comparison = currentItem.Behaviors.IP_Restriction_Membership & user_mask;
                    if (comparison == 0)
                    {
                        int restriction = currentItem.Behaviors.IP_Restriction_Membership;
                        int restriction_counter = 1;
                        while (restriction % 2 != 1)
                        {
                            restriction = restriction >> 1;
                            restriction_counter++;
                        }
                        if (Engine_ApplicationCache_Gateway.IP_Restrictions[restriction_counter] != null)
                            restriction_message = Engine_ApplicationCache_Gateway.IP_Restrictions[restriction_counter].Item_Restricted_Statement;
                        else
                            restriction_message = "Restricted Item";
                    }
                }
            }

            // If this item is restricted by IP than alot of the upcoming code is unnecessary
            if ((RequestSpecificValues.Current_User != null) && ((!itemRestrictedFromUserByIp) || (userCanEditItem) || (RequestSpecificValues.Current_User.Is_Internal_User)))
            {
                #region Region suppressed currently - was for adding feature to a map image?

                //// Searching for EAD/EAC type items is different from others
                //if (!isEadTypeItem)
                //{
                //    // If there is a coordinate search, and polygons, do that
                //    // GEt the geospatial metadata module
                //    GeoSpatial_Information geoInfo = RequestSpecificValues.Current_Item.Get_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY) as GeoSpatial_Information;
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

                // Is this a postback?
                if (RequestSpecificValues.Current_Mode.isPostBack)
                {
                    // Handle any actions from standard user action (i.e., email, add to bookshelf, etc. )
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
                                string cc_list = RequestSpecificValues.Current_User.Email;
                                if (address.ToUpper().IndexOf(RequestSpecificValues.Current_User.Email.ToUpper()) >= 0)
                                    cc_list = String.Empty;

                                // Send the email
                                HttpContext.Current.Session.Add("ON_LOAD_MESSAGE", !Item_Email_Helper.Send_Email(address, cc_list, comments, RequestSpecificValues.Current_User.Full_Name, RequestSpecificValues.Current_Mode.Instance_Abbreviation, currentItem, is_html_format, HttpContext.Current.Items["Original_URL"].ToString(), RequestSpecificValues.Current_User.UserID)
                                    ? "Error encountered while sending email" : "Your email has been sent");

                                HttpContext.Current.Response.Redirect(HttpContext.Current.Items["Original_URL"].ToString(), false);
                                HttpContext.Current.ApplicationInstance.CompleteRequest();
                                RequestSpecificValues.Current_Mode.Request_Completed = true;
                                return;
                            }
                        }

                        if (action == "add_item")
                        {
                            string usernotes = HttpContext.Current.Request.Form["add_notes"].Trim();
                            string foldername = HttpContext.Current.Request.Form["add_bookshelf"].Trim();
                            bool open_bookshelf = HttpContext.Current.Request.Form["open_bookshelf"] != null;

                            if (SobekCM_Database.Add_Item_To_User_Folder(RequestSpecificValues.Current_User.UserID, foldername, currentItem.BibID, currentItem.VID, 0, usernotes, RequestSpecificValues.Tracer))
                            {
                                RequestSpecificValues.Current_User.Add_Bookshelf_Item(currentItem.BibID, currentItem.VID);

                                // Ensure this user folder is not sitting in the cache
                                CachedDataManager.Remove_User_Folder_Browse(RequestSpecificValues.Current_User.UserID, foldername, RequestSpecificValues.Tracer);

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
                            RequestSpecificValues.Current_Mode.Request_Completed = true;
                            return;
                        }

                        if (action == "remove")
                        {
                            if (SobekCM_Database.Delete_Item_From_User_Folders(RequestSpecificValues.Current_User.UserID, currentItem.BibID, currentItem.VID, RequestSpecificValues.Tracer))
                            {
                                RequestSpecificValues.Current_User.Remove_From_Bookshelves(currentItem.BibID, currentItem.VID);
                                CachedDataManager.Remove_All_User_Folder_Browses(RequestSpecificValues.Current_User.UserID, RequestSpecificValues.Tracer);
                                HttpContext.Current.Session.Add("ON_LOAD_MESSAGE", "Item was removed from your bookshelves.");
                            }
                            else
                            {
                                HttpContext.Current.Session.Add("ON_LOAD_MESSAGE", "ERROR encountered while trying to remove item from your bookshelves.");
                            }

                            HttpContext.Current.Response.Redirect(HttpContext.Current.Items["Original_URL"].ToString(), false);
                            HttpContext.Current.ApplicationInstance.CompleteRequest();
                            RequestSpecificValues.Current_Mode.Request_Completed = true;
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
                            int new_tagid = SobekCM_Database.Add_Description_Tag(RequestSpecificValues.Current_User.UserID, tagid, currentItem.Web.ItemID, description, RequestSpecificValues.Tracer);
                            if (new_tagid > 0)
                            {
                                currentItem.Web.Add_User_Tag(RequestSpecificValues.Current_User.UserID, RequestSpecificValues.Current_User.Full_Name, description, DateTime.Now, new_tagid);
                                RequestSpecificValues.Current_User.Has_Descriptive_Tags = true;
                            }

                            HttpContext.Current.Response.Redirect(HttpContext.Current.Items["Original_URL"].ToString(), false);
                            HttpContext.Current.ApplicationInstance.CompleteRequest();
                            RequestSpecificValues.Current_Mode.Request_Completed = true;
                            return;
                        }

                        if (action.IndexOf("delete_tag") == 0)
                        {
                            if (action.Replace("delete_tag", "").Length > 0)
                            {
                                int tagid = Convert.ToInt32(action.Replace("delete_tag_", ""));
                                if (currentItem.Web.Delete_User_Tag(tagid, RequestSpecificValues.Current_User.UserID))
                                {
                                    SobekCM_Database.Delete_Description_Tag(tagid, RequestSpecificValues.Tracer);
                                }
                            }
                            HttpContext.Current.Response.Redirect(HttpContext.Current.Items["Original_URL"].ToString(), false);
                            HttpContext.Current.ApplicationInstance.CompleteRequest();
                            RequestSpecificValues.Current_Mode.Request_Completed = true;
                            return;
                        }
                    }
                }

                // Handle any request from the internal header for the item
                if ((HttpContext.Current != null) && (HttpContext.Current.Request.Form["internal_header_action"] != null) && (RequestSpecificValues.Current_User != null))
                {
                    // Pull the action value
                    string internalHeaderAction = HttpContext.Current.Request.Form["internal_header_action"].Trim();

                    // Was this to save the item comments?
                    if (internalHeaderAction == "save_comments")
                    {
                        string new_comments = HttpContext.Current.Request.Form["intheader_internal_notes"].Trim();
                        if (Resource_Object.Database.SobekCM_Database.Save_Item_Internal_Comments(currentItem.Web.ItemID, new_comments))
                            currentItem.Web.Internal_Comments = new_comments;
                    }
                }
            }

            // Set the code for bib level mets to show the volume tree by default
            if ((is_bib_level) && (String.IsNullOrEmpty(RequestSpecificValues.Current_Mode.ViewerCode)))
            {
                RequestSpecificValues.Current_Mode.ViewerCode = "allvolumes1";
            }

            // If there is a file name included, look for the sequence of that file
            if (!String.IsNullOrEmpty(RequestSpecificValues.Current_Mode.Page_By_FileName))
            {
                int page_sequence = currentItem.Page_Sequence_By_FileName(RequestSpecificValues.Current_Mode.Page_By_FileName);
                if (page_sequence > 0)
                {
                    RequestSpecificValues.Current_Mode.ViewerCode = page_sequence.ToString();
                    RequestSpecificValues.Current_Mode.Page = (ushort)page_sequence;
                }
            }

            // Get the valid viewer code
            RequestSpecificValues.Tracer.Add_Trace("Item_HtmlSubwriter.Add_Controls", "Getting the appropriate item viewer");
            iItemViewerPrototyper prototyper = ItemViewer_Factory.Get_Item_Viewer(currentItem, RequestSpecificValues.Current_Mode.ViewerCode);
            if (prototyper.Has_Access(currentItem, RequestSpecificValues.Current_User, !String.IsNullOrEmpty(restriction_message)))
                pageViewer = prototyper.Create_Viewer(currentItem, RequestSpecificValues.Current_User, RequestSpecificValues.Current_Mode);


            // If execution should end, do it now
            if (RequestSpecificValues.Current_Mode.Request_Completed)
                return;

            RequestSpecificValues.Tracer.Add_Trace("Html_MainWriter.Add_Controls", "Created " + pageViewer.GetType().ToString().Replace("SobekCM.Library.ItemViewer.Viewers.", ""));

            // Assign the rest of the information, if a page viewer was created
            if (pageViewer != null)
            {
                // Get the list of any special behaviors
                behaviors = pageViewer.ItemViewer_Behaviors;
            }
            else
            {
                behaviors = new List<HtmlSubwriter_Behaviors_Enum>();
            }

            // ALways suppress the banner and skip to main content
            if (behaviors == null)
                behaviors = new List<HtmlSubwriter_Behaviors_Enum>();

            if (!behaviors.Contains(HtmlSubwriter_Behaviors_Enum.Suppress_Banner))
                behaviors.Add(HtmlSubwriter_Behaviors_Enum.Suppress_Banner);
            if (!behaviors.Contains(HtmlSubwriter_Behaviors_Enum.Include_Skip_To_Main_Content_Link))
                behaviors.Add(HtmlSubwriter_Behaviors_Enum.Include_Skip_To_Main_Content_Link);

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

        /// <summary> Flag indicates this item is currently checked out by another user </summary>
        public bool Item_Checked_Out_By_Other_User 
        {
            set                                  
            {
                //// Override the page viewer at this point
                //if ((value) && (pageViewer.Override_On_Checked_Out))
                //{
                //    pageViewer = new Checked_Out_ItemViewer();
                //}
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

        /// <summary> Flag indicates if the internal header should included </summary>
        public override bool Include_Internal_Header
        {
            get
            {
                // If no user, do not show
                if ((RequestSpecificValues.Current_User == null) || (!RequestSpecificValues.Current_User.LoggedOn))
                    return false;

                // Always show for admins
                if ((RequestSpecificValues.Current_User.Is_System_Admin) || (RequestSpecificValues.Current_User.Is_Portal_Admin))
                    return true;

                if (RequestSpecificValues.Current_User.Can_Edit_This_Item(currentItem.BibID, currentItem.Type, currentItem.Behaviors.Source_Institution_Aggregation, currentItem.Behaviors.Holding_Location_Aggregation, currentItem.Behaviors.Aggregation_Code_List))
                    return true;

                // Otherwise, do not show
                return false;
            }
        }


        /// <summary> Adds the internal header HTML for this specific HTML writer </summary>
        /// <param name="Output"> Stream to which to write the HTML for the internal header information </param>
        /// <param name="Current_User"> Currently logged on user, to determine specific rights </param>
        public override void Write_Internal_Header_HTML(TextWriter Output, User_Object Current_User)
        {
            // If this is for a fragment, do nothing
            if (!String.IsNullOrEmpty(RequestSpecificValues.Current_Mode.Fragment))
                return;

            string currentViewerCode = RequestSpecificValues.Current_Mode.ViewerCode;

            Output.WriteLine("  <table id=\"sbk_InternalHeader\">");
            Output.WriteLine("    <tr style=\"height:30px;\">");
            Output.WriteLine("      <td style=\"text-align:left\">");
            Output.WriteLine("          <button title=\"Hide Internal Header\" class=\"sbkIsw_intheader_button hide_intheader_button2\" onclick=\"return hide_internal_header();\"></button>");
            Output.WriteLine("      </td>");

            if (is_bib_level)
            {
                Output.WriteLine("      <td style=\"text-align:center;\"><h2>" + currentItem.BibID + "</h2></td>");
            }
            else
            {
                Output.WriteLine("      <td style=\"text-align:center;\"><h2><a href=\"" + RequestSpecificValues.Current_Mode.Base_URL + currentItem.BibID + "/00000\">" + currentItem.BibID + "</a> : " + currentItem.VID + "</h2></td>");
            }

            Write_Internal_Header_Search_Box(Output);
            Output.WriteLine("    </tr>");

            if (!is_bib_level)
            {
                Output.WriteLine("    <tr style=\"height:40px;\">");
                Output.WriteLine("      <td colspan=\"3\" style=\"text-align:center;vertical-align:middle;\">");

                // Should we add ability to edit this item to the quick links?
                if (userCanEditItem)
                {
                    // Add ability to edit metadata for this item
                    RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.My_Sobek;
                    RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Edit_Item_Metadata;
                    RequestSpecificValues.Current_Mode.My_Sobek_SubMode = "1";
                    Output.WriteLine("          <button title=\"Edit Metadata\" class=\"sbkIsw_intheader_button edit_metadata_button\" onclick=\"window.location.href='" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "';return false;\"></button>");

                    // Add ability to edit behaviors for this item
                    RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Edit_Item_Behaviors;
                    RequestSpecificValues.Current_Mode.My_Sobek_SubMode = "1";
                    Output.WriteLine("          <button title=\"Edit Behaviors\" class=\"sbkIsw_intheader_button edit_behaviors_button\" onclick=\"window.location.href='" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "';return false;\"></button>");
                    RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Item_Display;


                    // Add ability to edit behaviors for this item
                    if ((currentItem.Images == null ) || ( currentItem.Images.Count == 0 ))
                    {
                        RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.My_Sobek;
                        RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Page_Images_Management;
                        Output.WriteLine("          <button title=\"Perform Quality Control\" class=\"sbkIsw_intheader_button qualitycontrol_button\" onclick=\"window.location.href='" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "';return false;\"></button>");
                        RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Item_Display;
                    }
                    else
                    {
                        RequestSpecificValues.Current_Mode.ViewerCode = "qc";
                        Output.WriteLine("          <button title=\"Perform Quality Control\" class=\"sbkIsw_intheader_button qualitycontrol_button\" onclick=\"window.location.href='" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "';return false;\"></button>");
                    }

                    // Get ready to send to item permissions
                    RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.My_Sobek;
                    RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Edit_Item_Permissions;
                    RequestSpecificValues.Current_Mode.My_Sobek_SubMode = "1";

                    // Check if this item is DARK first
                    if (currentItem.Behaviors.Dark_Flag)
                    {
                        Output.WriteLine("          <button title=\"Dark Resource\" class=\"sbkIsw_intheader_button dark_resource_button_fixed\" onclick=\"window.location.href='" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "';return false;\"></button>");
                    }
                    else
                    {
                        // If the item is currently PUBLIC, only internal or system admins can reset back to PRIVATE
                        if (currentItem.Behaviors.IP_Restriction_Membership >= 0)
                        {
                            if ((RequestSpecificValues.Current_User.Is_Internal_User) || (RequestSpecificValues.Current_User.Is_System_Admin))
                            {
                                Output.WriteLine(currentItem.Behaviors.IP_Restriction_Membership == 0
                                                     ? "          <button title=\"Change Access Restriction\" class=\"sbkIsw_intheader_button public_resource_button\" onclick=\"window.location.href='" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "';return false;\"></button>"
                                                     : "          <button title=\"Change Access Restriction\" class=\"sbkIsw_intheader_button restricted_resource_button\" onclick=\"window.location.href='" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "';return false;\"></button>");
                            }
                            else
                            {
                                Output.WriteLine(currentItem.Behaviors.IP_Restriction_Membership == 0
                                                     ? "          <button title=\"Public Resource\" class=\"sbkIsw_intheader_button public_resource_button_fixed\" onclick=\"window.location.href='" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "';return false;\"></button>"
                                                     : "          <button title=\"IP Restriced Resource\" class=\"sbkIsw_intheader_button restricted_resource_button_fixed\" onclick=\"window.location.href='" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "';return false;\"></button>");
                            }
                        }
                        else
                        {
                            Output.WriteLine("          <button title=\"Change Access Restriction\" class=\"sbkIsw_intheader_button private_resource_button\" onclick=\"window.location.href='" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "';return false;\"></button>");
                        }
                    }
                }
                else
                {
                    // Check if this item is DARK first
                    if (currentItem.Behaviors.Dark_Flag)
                    {
                        Output.WriteLine("          <button title=\"Dark Resource\" class=\"sbkIsw_intheader_button dark_resource_button_fixed\" onclick=\"window.location.href='" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "';return false;\"></button>");
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

                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Item_Display;
                RequestSpecificValues.Current_Mode.ViewerCode = "tracking";
                Output.WriteLine("          <button title=\"View Work Log\" class=\"sbkIsw_intheader_button view_worklog_button\" onclick=\"window.location.href='" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "';return false;\"></button>");
                RequestSpecificValues.Current_Mode.ViewerCode = currentViewerCode;

                // Add ability to edit behaviors for this item
                if (userCanEditItem)
                {
                    RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.My_Sobek;
                    RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.File_Management;
                    Output.WriteLine("          <button title=\"Manage Files\" class=\"sbkIsw_intheader_button manage_files_button\" onclick=\"window.location.href='" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "';return false;\"></button>");
                    RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Item_Display;
                }

                // Add the HELP icon next
                Output.WriteLine("<span id=\"sbk_InternalHeader_Help\"><a href=\"" + UI_ApplicationCache_Gateway.Settings.System.Help_URL(RequestSpecificValues.Current_Mode.Base_URL) + "help/itemheader\" title=\"Help regarding this header\"><img src=\"" + Static_Resources.Help_Button_Darkgray_Jpg + "\" alt=\"?\" title=\"Help regarding this header\" /></a></span>");

                Output.WriteLine("      </td>");
                Output.WriteLine("    </tr>");

                // Display the comments and allow change?
                string internal_comments_normalized = currentItem.Web.Internal_Comments ?? String.Empty;
                if ((userCanEditItem) || (RequestSpecificValues.Current_User.Is_Internal_User) || (RequestSpecificValues.Current_User.Is_System_Admin))
                {
                    const int ROWS = 1;
                    const int ACTUAL_COLS = 70;

                    // Add the internal comments row ( hidden content initially )
                    Output.WriteLine("    <tr style=\"text-align:center; height:14px;\">");
                    Output.WriteLine("      <td colspan=\"3\" style=\"text-align:center;\">");
                    Output.WriteLine("        <table id=\"internal_notes_div\">");
                    Output.WriteLine("          <tr style=\"text-align:left; height:14px;\">");
                    Output.WriteLine("            <td class=\"intheader_label\">COMMENTS:</td>");
                    Output.WriteLine("            <td>");
                    Output.WriteLine("              <textarea rows=\"" + ROWS + "\" cols=\"" + ACTUAL_COLS + "\" name=\"intheader_internal_notes\" id=\"intheader_internal_notes\" class=\"intheader_comments_input sbkIsw_Focusable\">" + HttpUtility.HtmlEncode(internal_comments_normalized) + "</textarea>");
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
                    const int ROWS = 1;
                    const int ACTUAL_COLS = 80;

                    // Add the internal comments row ( hidden content initially )
                    Output.WriteLine("    <tr style=\"text-align:center; height:14px;\">");
                    Output.WriteLine("      <td colspan=\"2\">");
                    Output.WriteLine("        <table id=\"internal_notes_div\">");
                    Output.WriteLine("          <tr style=\"text-align:left; height:14px;\">");
                    Output.WriteLine("            <td class=\"intheader_label\">COMMENTS:</td>");
                    Output.WriteLine("            <td>");
                    Output.WriteLine("              <textarea readonly=\"readonly\" rows=\"" + ROWS + "\" cols=\"" + ACTUAL_COLS + "\" name=\"intheader_internal_notes\" id=\"intheader_internal_notes\" class=\"intheader_comments_input\" onfocus=\"javascript:textbox_enter('intheader_internal_notes','intheader_comments_input_focused')\" onblur=\"javascript:textbox_leave('intheader_internal_notes','intheader_comments_input')\">" + HttpUtility.HtmlEncode(internal_comments_normalized) + "</textarea>");
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
                    RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.My_Sobek;
                    RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Edit_Group_Behaviors;
                    RequestSpecificValues.Current_Mode.My_Sobek_SubMode = "1";
                    Output.WriteLine("          <button title=\"Edit Behaviors\" class=\"sbkIsw_intheader_button edit_behaviors_button\" onclick=\"window.location.href='" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "';return false;\"></button>");

                    // Add ability to add a new item/volume to this title
                    RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Group_Add_Volume;
                    Output.WriteLine("          <button title=\"Add Volume\" class=\"sbkIsw_intheader_button add_volume_button\" onclick=\"window.location.href='" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "';return false;\"></button>");

                    // Add ability to auto-fill a number of new items/volumes to this title
                    RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Group_AutoFill_Volumes;
                    Output.WriteLine("          <button title=\"Auto-Fill Volumes\" class=\"sbkIsw_intheader_button autofill_volumes_button\" onclick=\"window.location.href='" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "';return false;\"></button>");

                    // Add ability to edit the serial hierarchy online
                    RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Edit_Group_Serial_Hierarchy;
                    Output.WriteLine("          <button title=\"Edit Serial Hierarchy\" class=\"sbkIsw_intheader_button serial_hierarchy_button\" onclick=\"window.location.href='" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "';return false;\"></button>");

                    // Add ability to mass update the items behaviors under this title
                    RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Group_Mass_Update_Items;
                    Output.WriteLine("          <button title=\"Mass Update Volumes\" class=\"sbkIsw_intheader_button mass_update_button\" onclick=\"window.location.href='" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "';return false;\"></button>");

                    Output.WriteLine("      </td>");
                    Output.WriteLine("    </tr>");
                }

            }

            Output.WriteLine("  </table>");

            RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Item_Display;
            RequestSpecificValues.Current_Mode.ViewerCode = currentViewerCode;
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

            Tracer.Add_Trace("Item_HtmlSubwriter.Write_HTML", "Begin writing the item viewer, up to the item-level main menu");


            // If this is for a fragment, do nothing
            if (!String.IsNullOrEmpty(RequestSpecificValues.Current_Mode.Fragment))
                return false;


            Output.WriteLine();

            //// Just return if this is the pageviewer
            //if (pageViewer.ItemViewer_Type == ItemViewer_Type_Enum.GnuBooks_PageTurner)
            //{
            //    return true;
            //}

            // Put an itemscope div around here for micro-data purposes
            Output.WriteLine("<!-- Unstyled div placed around entire item information to support schema.org microdata -->");
            Output.WriteLine("<section itemscope itemtype=\"http:schema.org/ItemPage\">");
            Output.WriteLine();

            // The item viewer can choose to override the standard item titlebar
            if (!behaviors.Contains(HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Suppress_Titlebar))
            {
                Output.WriteLine("<!-- Show the title and any other important item information -->");
                Output.WriteLine("<section id=\"sbkIsw_Titlebar\" role=\"banner\">");

                if ( is_bib_level )
                {
                    string grouptitle = currentItem.Behaviors.GroupTitle;
                    if (grouptitle.Length > 125)
                    {
                        Output.WriteLine("\t<h1 itemprop=\"name\"><abbr title=\"" + grouptitle + "\">" + grouptitle.Substring(0, 120) + "..</abbr></h1>");
                    }
                    else
                    {
                        Output.WriteLine("\t<h1 itemprop=\"name\">" + grouptitle + "</h1>");
                    }
                }
                else
                {
                    string final_title = currentItem.Title ?? "NO TITLE";

                    // Add the Title if there is one
                    if (final_title.Length > 0)
                    {
                        // Is this a newspaper?
                        bool newspaper = (String.Compare(currentItem.Behaviors.GroupType, "NEWSPAPER", StringComparison.OrdinalIgnoreCase) == 0);

                        // Does a custom setting override the default behavior to add a date?
                        if ((newspaper) && (UI_ApplicationCache_Gateway.Settings.Contains_Additional_Setting("Item Viewer.Include Date In Title")) && (UI_ApplicationCache_Gateway.Settings.Get_Additional_Setting("Item Viewer.Include Date In Title").ToUpper() == "NEVER"))
                            newspaper = false;

                        // Add the date if it should be added
                        if ((newspaper) && ( !String.IsNullOrEmpty(currentItem.Web.Date)))
                        {
                            if (final_title.Length > 125)
                            {
                                Output.WriteLine("\t<h1 itemprop=\"name\"><abbr title=\"" + final_title + "\">" + final_title.Substring(0, 120) + "..</abbr> ( " + currentItem.Web.Date + " )</h1>");
                            }
                            else
                            {
                                Output.WriteLine("\t<h1 itemprop=\"name\">" + final_title + " ( " + currentItem.Web.Date + " )</h1>");
                            }
                        }
                        else
                        {
                            if (final_title.Length > 125)
                            {
                                Output.WriteLine("\t<h1 itemprop=\"name\"><abbr title=\"" + final_title + "\">" + final_title.Substring(0, 120) + "..</abbr></h1>");
                            }
                            else
                            {
                                Output.WriteLine("\t<h1 itemprop=\"name\">" + final_title + "</h1>");
                            }
                        }
                    }


                    // Add the link if there is one  
                    // Links_BriefItemMapper
                    if (!String.IsNullOrEmpty(currentItem.Web.Title_Box_Additional_Link))
                    {
                        // Get the translated TYPE
                        string type = UI_ApplicationCache_Gateway.Translation.Get_Translation((currentItem.Web.Title_Box_Additional_Link_Type ?? "Related Link"), RequestSpecificValues.Current_Mode.Language);

                        // Add the link
                        Output.WriteLine("\t" + currentItem.Web.Title_Box_Additional_Link + " ( " + type + " )<br />");
                    }


                    // If there is an ACCESSION number and this is an ARTIFACT, include that at the top
                    BriefItem_DescriptiveTerm accessNumber = currentItem.Get_Description("Accession Number");
                    if ((accessNumber != null) && (accessNumber.Values != null) && (accessNumber.Values.Count > 0 ))
                    {
                        Output.WriteLine("\t" + UI_ApplicationCache_Gateway.Translation.Get_Translation("Accession number", RequestSpecificValues.Current_Mode.Language) + " " + accessNumber.Values[0].Value + "<br />");
                    }
                }


                Output.WriteLine("</section>");
                Output.WriteLine();
            }

            // The item viewer can choose to override the standard item menu
            if (!behaviors.Contains(HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Suppress_Item_Menu))
            {
                // Can this user (if there is one) edit this item?
                bool canManage = (RequestSpecificValues.Current_User != null) && (RequestSpecificValues.Current_User.Can_Edit_This_Item(currentItem.BibID, currentItem.Type, currentItem.Behaviors.Source_Institution_Aggregation, currentItem.Behaviors.Holding_Location_Aggregation, currentItem.Behaviors.Aggregation_Code_List));

                // Add the item views
                Output.WriteLine("<!-- Add the different view and social options -->");
                Output.WriteLine("<nav class=\"sbkMenu_Bar\" id=\"sbkIsw_MenuBar\" role=\"navigation\" aria-label=\"Item menu\">");
                Output.WriteLine("<h2 class=\"hidden-element\">Item menu</h2>");

                // Add the sharing buttons if this is not restricted by IP address or checked out
                if ((!itemRestrictedFromUserByIp) && (!itemCheckedOutByOtherUser) && (!RequestSpecificValues.Current_Mode.Is_Robot))
                {
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

                    string logOnUrl = String.Empty;
                    bool isLoggedOn = RequestSpecificValues.Current_User != null && RequestSpecificValues.Current_User.LoggedOn;
                    if (!isLoggedOn)
                    {
                        string returnUrl = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);

                        RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.My_Sobek;
                        RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Logon;
                        RequestSpecificValues.Current_Mode.Return_URL = returnUrl;
                        logOnUrl = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
                        RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Item_Display;
                        RequestSpecificValues.Current_Mode.Return_URL = String.Empty;
                    }

                    Output.WriteLine("\t<div id=\"menu-right-actions\">");

                    if (currentItem.Web.ItemID > 0)
                    {
                        Output.WriteLine("\t\t<span id=\"printbuttonitem\" class=\"action-sf-menu-item\" onclick=\"print_form_open();\"><img src=\"" + Static_Resources.Printer_Png + "\" alt=\"\" style=\"vertical-align:middle\" /><span id=\"printbuttonspan\">" + print_text + "</span></span>");
                    }
                    else
                    {
                        Output.WriteLine("\t\t<span id=\"printbuttonitem\" class=\"action-sf-menu-item\" onclick=\"window.print();return false;\"><img src=\"" + Static_Resources.Printer_Png + "\" alt=\"\" style=\"vertical-align:middle\" /><span id=\"printbuttonspan\">" + print_text + "</span></span>");
                    }


                    if (isLoggedOn)
                    {
                        Output.WriteLine("\t\t<span id=\"sendbuttonitem\" class=\"action-sf-menu-item\" onclick=\"email_form_open();\"><img src=\"" + Static_Resources.Email_Png + "\" alt=\"\" style=\"vertical-align:middle\" /><span id=\"sendbuttonspan\">" + send_text + "</span></span>");


                        if (currentItem.Web.ItemID > 0)
                        {
                            if (RequestSpecificValues.Current_User.Is_In_Bookshelf(currentItem.BibID, currentItem.VID))
                            {
                                Output.WriteLine("\t\t<span id=\"addbuttonitem\" class=\"action-sf-menu-item\" onclick=\"return remove_item_itemviewer();\"><img src=\"" + Static_Resources.Minussign_Png + "\" alt=\"\" style=\"vertical-align:middle\" /><span id=\"addbuttonspan\">" + remove_text + "</span></span>");
                            }
                            else
                            {
                                Output.WriteLine("\t\t<span id=\"addbuttonitem\" class=\"action-sf-menu-item\" onclick=\"add_item_form_open();return false;\"><img src=\"" + Static_Resources.Plussign_Png + "\" alt=\"\" style=\"vertical-align:middle\" /><span id=\"addbuttonspan\">" + add_text + "</span></span>");
                            }
                        }
                    }
                    else
                    {
                        Output.WriteLine("\t\t<span id=\"sendbuttonitem\" class=\"action-sf-menu-item\" onclick=\"window.location='" + logOnUrl + "';return false;\"><img src=\"" + Static_Resources.Email_Png + "\" alt=\"\" style=\"vertical-align:middle\" /><span id=\"sendbuttonspan\">" + send_text + "</span></span>");

                        if (currentItem.Web.ItemID > 0)
                            Output.WriteLine("\t\t<span id=\"addbuttonitem\" class=\"action-sf-menu-item\" onclick=\"window.location='" + logOnUrl + "';return false;\"><img src=\"" + Static_Resources.Plussign_Png + "\" alt=\"\" style=\"vertical-align:middle\" /><span id=\"addbuttonspan\">" + add_text + "</span></span>");
                    }

                    Output.WriteLine("\t\t<span id=\"sharebuttonitem\" class=\"action-sf-menu-item\" onclick=\"toggle_share_form('share_button');\"><span id=\"sharebuttonspan\">Share</span></span>");


                    Output.WriteLine("\t</div>");
                    Output.WriteLine();
                }


                Output.WriteLine("\t<ul class=\"sf-menu\" id=\"sbkIhs_Menu\">");


                // Save the current view type
                ushort page = RequestSpecificValues.Current_Mode.Page.HasValue ? RequestSpecificValues.Current_Mode.Page.Value : (ushort)1;
                ushort subpage = RequestSpecificValues.Current_Mode.SubPage.HasValue ? RequestSpecificValues.Current_Mode.SubPage.Value : (ushort)1;
                string viewerCode = RequestSpecificValues.Current_Mode.ViewerCode;
                RequestSpecificValues.Current_Mode.SubPage = 0;

                // Add any PRE-MENU instance options
                string first_pre_menu_option = String.Empty;
                string second_pre_menu_option = String.Empty;
                string third_pre_menu_option = String.Empty;
                if (UI_ApplicationCache_Gateway.Settings.Contains_Additional_Setting("Item Viewer.Static First Menu Item"))
                    first_pre_menu_option = UI_ApplicationCache_Gateway.Settings.Get_Additional_Setting("Item Viewer.Static First Menu Item");
                if (UI_ApplicationCache_Gateway.Settings.Contains_Additional_Setting("Item Viewer.Static Second Menu Item"))
                    second_pre_menu_option = UI_ApplicationCache_Gateway.Settings.Get_Additional_Setting("Item Viewer.Static Second Menu Item");
                if (UI_ApplicationCache_Gateway.Settings.Contains_Additional_Setting("Item Viewer.Static Third Menu Item"))
                    third_pre_menu_option = UI_ApplicationCache_Gateway.Settings.Get_Additional_Setting("Item Viewer.Static Third Menu Item");
                if ((first_pre_menu_option.Length > 0) || (second_pre_menu_option.Length > 0) || (third_pre_menu_option.Length > 0))
                {
                    if (first_pre_menu_option.Length > 0)
                    {
                        string[] first_splitter = first_pre_menu_option.Replace("[", "").Replace("]", "").Split(";".ToCharArray());
                        if (first_splitter.Length > 0)
                        {
                            Output.WriteLine("\t\t<li><a href=\"" + first_splitter[1] + "\" title=\"" + HttpUtility.HtmlEncode(first_splitter[0]) + "\">" + HttpUtility.HtmlEncode(first_splitter[0]) + "</a></li>");
                        }
                    }
                    if (second_pre_menu_option.Length > 0)
                    {
                        string[] second_splitter = second_pre_menu_option.Replace("[", "").Replace("]", "").Split(";".ToCharArray());
                        if (second_splitter.Length > 0)
                        {
                            Output.WriteLine("\t\t<li><a href=\"" + second_splitter[1] + "\" title=\"" + HttpUtility.HtmlEncode(second_splitter[0]) + "\">" + HttpUtility.HtmlEncode(second_splitter[0]) + "</a></li>");
                        }
                    }
                    if (third_pre_menu_option.Length > 0)
                    {
                        string[] third_splitter = third_pre_menu_option.Replace("[", "").Replace("]", "").Split(";".ToCharArray());
                        if (third_splitter.Length > 0)
                        {
                            Output.WriteLine("\t\t<li><a href=\"" + third_splitter[1] + "\" title=\"" + HttpUtility.HtmlEncode(third_splitter[0]) + "\">" + HttpUtility.HtmlEncode(third_splitter[0]) + "</a></li>");
                        }
                    }
                }

                //// Add the item level views
                //foreach (BriefItem_BehaviorViewer thisView in currentItem.Behaviors.Viewers)
                //{
                //    if (((!itemRestrictedFromUserByIp) && (!currentItem.Behaviors.Dark_Flag)) || (thisView.View_Type == View_Enum.CITATION) ||
                //        (thisView.View_Type == View_Enum.ALL_VOLUMES) ||
                //        (thisView.View_Type == View_Enum.RELATED_IMAGES))
                //    {
                //        // Special code for the CITATION view (TEMPORARY - v.3.2)
                //        if (thisView.View_Type == View_Enum.CITATION)
                //        {
                //            if (RequestSpecificValues.Current_Mode.Is_Robot)
                //            {
                //                Output.Write("\t\t<li class=\"selected-sf-menu-item-link\"><a href=\"\">Description</a></li>");
                //            }
                //            else
                //            {
                //                RequestSpecificValues.Current_Mode.ViewerCode = "citation";
                //                if (currentItem.Bib_Info.SobekCM_Type == TypeOfResource_SobekCM_Enum.EAD)
                //                    RequestSpecificValues.Current_Mode.ViewerCode = "description";
                //                if ((viewerCode == "citation") || (viewerCode == "marc") || (viewerCode == "metadata") ||
                //                    (viewerCode == "usage") || (viewerCode == "description"))
                //                {
                //                    Output.Write("\t\t<li class=\"selected-sf-menu-item-link\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Description</a>");
                //                }
                //                else
                //                {
                //                    Output.Write("\t\t<li><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Description</a>");
                //                }
                //                Output.WriteLine("<ul>");


                //                if (currentItem.Bib_Info.SobekCM_Type == TypeOfResource_SobekCM_Enum.EAD)
                //                    Output.WriteLine("\t\t\t<li><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Archival Description</a></li>");
                //                else
                //                    Output.WriteLine("\t\t\t<li><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Standard View</a></li>");



                //                RequestSpecificValues.Current_Mode.ViewerCode = "marc";
                //                Output.WriteLine("\t\t\t<li><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">MARC View</a></li>");


                //                RequestSpecificValues.Current_Mode.ViewerCode = "metadata";
                //                Output.WriteLine("\t\t\t<li><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Metadata</a></li>");


                //                RequestSpecificValues.Current_Mode.ViewerCode = "usage";
                //                Output.WriteLine("\t\t\t<li><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Usage Statistics</a></li>");


                //                Output.WriteLine("\t\t</ul></li>");
                //                RequestSpecificValues.Current_Mode.ViewerCode = viewerCode;
                //            }
                //        }
                //        else if (thisView.View_Type == View_Enum.ALL_VOLUMES)
                //        {
                //            string resource_type_upper = currentItem.Bib_Info.SobekCM_Type_String.ToUpper();
                //            string all_volumes = "All Volumes";
                //            if (resource_type_upper.IndexOf("NEWSPAPER") >= 0)
                //            {
                //                all_volumes = "All Issues";
                //            }
                //            else if (resource_type_upper.IndexOf("MAP") >= 0)
                //            {
                //                all_volumes = "Related Maps";
                //            }
                //            else if (resource_type_upper.IndexOf("AERIAL") >= 0)
                //            {
                //                all_volumes = "Related Flights";
                //            }

                //            if (RequestSpecificValues.Current_Mode.Is_Robot)
                //            {
                //                Output.Write("\t\t<li><a href=\"" + UI_ApplicationCache_Gateway.Settings.Servers.Base_URL + "\\" + currentItem.BibID + "\">" + all_volumes + "</a></li>");
                //            }
                //            else
                //            {

                //                RequestSpecificValues.Current_Mode.ViewerCode = "allvolumes";
                //                if ((viewerCode == "allvolumes") || (viewerCode == "allvolumes2") ||
                //                    (viewerCode == "allvolumes3"))
                //                {
                //                    Output.Write("\t\t<li class=\"selected-sf-menu-item-link\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + all_volumes + "</a>");
                //                }
                //                else
                //                {
                //                    Output.Write("\t\t<li><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + all_volumes + "</a>");
                //                }
                //                Output.WriteLine("<ul>");


                //                RequestSpecificValues.Current_Mode.ViewerCode = "allvolumes";
                //                Output.WriteLine("\t\t\t<li><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Tree View</a></li>");


                //                RequestSpecificValues.Current_Mode.ViewerCode = "allvolumes2";
                //                Output.WriteLine("\t\t\t<li><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Thumbnails</a></li>");


                //                if ((RequestSpecificValues.Current_User != null) && (RequestSpecificValues.Current_User.LoggedOn) && (RequestSpecificValues.Current_User.Is_Internal_User))
                //                {
                //                    RequestSpecificValues.Current_Mode.ViewerCode = "allvolumes3";
                //                    Output.WriteLine("\t\t\t<li><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">List View</a></li>");
                //                }


                //                Output.WriteLine("\t\t</ul></li>");
                //                RequestSpecificValues.Current_Mode.ViewerCode = viewerCode;
                //            }
                //        }
                //        else
                //        {
                //            List<string> item_nav_bar_link = Item_Nav_Bar_HTML_Factory.Get_Nav_Bar_HTML(thisView, currentItem.Bib_Info.SobekCM_Type_String, RequestSpecificValues.HTML_Skin.Base_Skin_Code, RequestSpecificValues.Current_Mode, -1, UI_ApplicationCache_Gateway.Translation, showZoomable, currentItem);


                //            // Add each nav bar link
                //            foreach (string this_link in item_nav_bar_link)
                //            {
                //                Output.WriteLine("\t\t" + this_link + "");
                //            }
                //        }
                //    }
                //}


                //// If this is citation or index mode, the number may be an invalid page sequence
                //if ((page <= 0) ||
                //    (RequestSpecificValues.Current_Mode.ViewerCode == View_Object.Viewer_Code_By_Type(View_Enum.RELATED_IMAGES)[0]))
                //{
                //    RequestSpecificValues.Current_Mode.Page = 1;
                //}


                //if ((currentItem.Web.Static_PageCount > 0) && (RequestSpecificValues.Current_Page == null))
                //{
                //    RequestSpecificValues.Current_Page = currentItem.Web.Pages_By_Sequence[0];
                //}


                //// Add each page display type
                //if ((RequestSpecificValues.Current_Page != null) && (!itemRestrictedFromUserByIp))
                //{
                //    int page_seq = RequestSpecificValues.Current_Mode.Page.HasValue ? RequestSpecificValues.Current_Mode.Page.Value : 1;
                //    string resourceType = currentItem.Bib_Info.SobekCM_Type_String.ToUpper();
                //    if (currentItem.Behaviors.Item_Level_Page_Views_Count > 0)
                //    {
                //        List<string> pageViewLinks = new List<string>();

                //        foreach (View_Object thisPageView in currentItem.Behaviors.Item_Level_Page_Views)
                //        {
                //            View_Enum thisViewType = thisPageView.View_Type;
                //            foreach (SobekCM_File_Info thisFile in RequestSpecificValues.Current_Page.Files)
                //            {
                //                View_Object fileObject = thisFile.Get_Viewer();
                //                if ((fileObject != null) && (fileObject.View_Type == thisViewType))
                //                {
                //                    pageViewLinks.AddRange(Item_Nav_Bar_HTML_Factory.Get_Nav_Bar_HTML(thisFile.Get_Viewer(), resourceType, RequestSpecificValues.HTML_Skin.Base_Skin_Code, RequestSpecificValues.Current_Mode, page_seq, UI_ApplicationCache_Gateway.Translation, showZoomable, currentItem));
                //                }
                //            }
                //        }


                //        if (currentItem.BibID == "UF00001672")
                //        {
                //            string filename = RequestSpecificValues.Current_Page.Files[0].File_Name_Sans_Extension + ".txt";
                //            SobekCM_File_Info newFile = new SobekCM_File_Info(filename);
                //            pageViewLinks.AddRange(Item_Nav_Bar_HTML_Factory.Get_Nav_Bar_HTML(newFile.Get_Viewer(), resourceType, RequestSpecificValues.HTML_Skin.Base_Skin_Code, RequestSpecificValues.Current_Mode, page_seq, UI_ApplicationCache_Gateway.Translation, showZoomable, currentItem));
                //        }

                //        // Only continue if there were views
                //        if (pageViewLinks.Count > 0)
                //        {
                //            // Determine the name for this menu item
                //            string menu_title = "Page Image";
                //            if (resourceType.IndexOf("MAP") >= 0)
                //            {
                //                menu_title = "Map Image";
                //            }
                //            else if ((resourceType.IndexOf("AERIAL") >= 0) || (resourceType.IndexOf("PHOTOGRAPH") >= 0))
                //            {
                //                menu_title = "Image";
                //            }
                //            if ((currentItem.Images != null ) && ( currentItem.Images.Count > 1 ))
                //                menu_title = menu_title + "s";


                //            // Get the link for the first page view
                //            string link = pageViewLinks[0].Substring(pageViewLinks[0].IndexOf("href=\"") + 6);
                //            link = link.Substring(0, link.IndexOf("\""));


                //            // Was this a match?
                //            if ((RequestSpecificValues.Current_Mode.ViewerCode == page_seq + "t") || (RequestSpecificValues.Current_Mode.ViewerCode == page_seq + "x") || (RequestSpecificValues.Current_Mode.ViewerCode == page_seq + "j"))
                //            {
                //                Output.Write("\t\t<li class=\"selected-sf-menu-item-link\"><a href=\"" + link + "\">" + menu_title + "</a>");
                //            }
                //            else
                //            {
                //                Output.Write("\t\t<li><a href=\"" + link + "\">" + menu_title + "</a>");
                //            }
                //            Output.WriteLine("<ul>");


                //            foreach (string pageLink in pageViewLinks)
                //            {
                //                Output.WriteLine("\t\t\t<li>" + pageLink + "</li>");
                //            }


                //            Output.WriteLine("\t\t</ul></li>");
                //        }
                //    }
                //}


                //if (itemRestrictedFromUserByIp)
                //{
                //    List<string> restricted_nav_bar_link = Item_Nav_Bar_HTML_Factory.Get_Nav_Bar_HTML(new View_Object(View_Enum.RESTRICTED), currentItem.Bib_Info.SobekCM_Type_String.ToUpper(), RequestSpecificValues.HTML_Skin.Base_Skin_Code, RequestSpecificValues.Current_Mode, 0, UI_ApplicationCache_Gateway.Translation, showZoomable, currentItem);
                //    Output.WriteLine("\t\t" + restricted_nav_bar_link[0] + "");
                //}


                // Add the MANAGE button?
                if (userCanEditItem)
                {
                    // Get the MANAGE URL (which is actually an item view)
                    RequestSpecificValues.Current_Mode.ViewerCode = "manage";
                    string manage_menu_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);

                    if ( is_bib_level )
                    {

                        // Get all the mySObek URLs
                        RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.My_Sobek;
                        RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Edit_Item_Metadata;
                        string edit_metadata_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
                        RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Edit_Item_Behaviors;
                        string edit_behaviors_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
                        RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Page_Images_Management;
                        string page_images_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
                        RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.File_Management;
                        string manage_downloads = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);

                        RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Item_Display;


                        Output.WriteLine("\t\t<li><a href=\"" + manage_menu_url + "\">Manage</a><ul>");

                        Output.WriteLine("\t\t\t<li><a href=\"" + edit_metadata_url + "\">Edit Metadata</a></li>");
                        Output.WriteLine("\t\t\t<li><a href=\"" + edit_behaviors_url + "\">Edit Item Behaviors</a></li>");
                        Output.WriteLine("\t\t\t<li><a href=\"" + manage_downloads + "\">Manage Download Files</a></li>");

                        if ((currentItem.Images == null ) || ( currentItem.Images.Count == 0 ))
                            Output.WriteLine("\t\t\t<li><a href=\"" + page_images_url + "\">Manage Pages and Divisions</a></li>");
                        else
                        {
                            RequestSpecificValues.Current_Mode.ViewerCode = "qc";
                            Output.WriteLine("\t\t\t<li><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Manage Pages and Divisions</a></li>");
                        }

                        RequestSpecificValues.Current_Mode.ViewerCode = "mapedit";
                        Output.WriteLine("\t\t\t<li><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Manage Geo-Spatial Data (beta)</a></li>");

                        RequestSpecificValues.Current_Mode.ViewerCode = "ts";
                        Output.WriteLine("\t\t\t<li><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">View Tracking Sheet</a></li>");

                        Output.WriteLine("\t\t</ul></li>");
                    }
                    else
                    {
                        // Get all the mySObek URLs
                        RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.My_Sobek;
                        RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Edit_Group_Behaviors;
                        string edit_behaviors_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
                        RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Group_Add_Volume;
                        string add_volume_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
                        RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Group_Mass_Update_Items;
                        string mass_update_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);

                        RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Item_Display;


                        Output.WriteLine("\t\t<li><a href=\"" + manage_menu_url + "\">Manage</a><ul>");

                        Output.WriteLine("\t\t\t<li><a href=\"" + edit_behaviors_url + "\">Edit Item Group Behaviors</a></li>");
                        Output.WriteLine("\t\t\t<li><a href=\"" + add_volume_url + "\">Add New Volume</a></li>");
                        Output.WriteLine("\t\t\t<li><a href=\"" + mass_update_url + "\">Mass Update Item Behaviors</a></li>");

                        Output.WriteLine("\t\t</ul></li>");
                    }
                }


                // Set current submode back
                RequestSpecificValues.Current_Mode.Page = page;
                RequestSpecificValues.Current_Mode.ViewerCode = viewerCode;
                RequestSpecificValues.Current_Mode.SubPage = subpage;



                Output.WriteLine("\t</ul>");
                Output.WriteLine("</nav>");
                Output.WriteLine();


                Output.WriteLine("<!-- Initialize the main item menu -->");
                Output.WriteLine("<script>");
                Output.WriteLine("\tjQuery(document).ready(function () { jQuery('ul.sf-menu').superfish(); });");
                Output.WriteLine("</script>");
                Output.WriteLine();
            }

            return true;
	    }


	    /// <summary> Writes the html to the output stream open the itemNavForm, which appears just before the TocPlaceHolder </summary>
        /// <param name="Output">Stream to directly write to</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Write_ItemNavForm_Opening(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Item_HtmlSubwriter.Write_ItemNavForm_Opening", "Start the left navigational bar");

            Tracer.Add_Trace("Item_HtmlSubwriter.Write_ItemNavForm_Opening", "Start the left navigational bar");

            // Add the divs for loading the pop-up forms
            Output.WriteLine("<!-- Place holders for pop-up forms which load dynamically if required -->");
            Output.WriteLine("<div class=\"print_popup_div\" id=\"form_print\" style=\"display:none;\"></div>");
            Output.WriteLine("<div class=\"email_popup_div\" id=\"form_email\" style=\"display:none;\"></div>");
            Output.WriteLine("<div class=\"add_popup_div\" id=\"add_item_form\" style=\"display:none;\"></div>");
            Output.WriteLine("<div class=\"share_popup_div\" id=\"share_form\" style=\"display:none;\"></div>");
            Output.WriteLine();

            if (ShouldLeftNavigationBarBeShown)
            {
                // Show the entire item viewer in a presentation table.  Tried using divs, but continued to have problems
                // especially with smaller screens
                Output.WriteLine("<!-- Presentation table to enforce layout -->");
                Output.WriteLine("<table role=\"presentation\"><tr style=\"vertical-align:top\"><td>");

                // Start the item viewer
                Output.WriteLine("<!-- Begin the left navigational bar -->");

                Output.WriteLine(pageViewerBehaviors.Contains(HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Requires_Left_Navigation_Bar) || showToc ? "<nav id=\"sbkIsw_Leftnavbar_hack\" role=\"complementary\">" : "<nav id=\"sbkIsw_Leftnavbar\" role=\"complementary\">");

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
                if ((!showToc) && (currentItem.Images_TOC != null ) && ( currentItem.Images_TOC.Count > 0 ))
                {
                    string show_toc_text = "SHOW TABLE OF CONTENTS";

                    if (RequestSpecificValues.Current_Mode.Language == Web_Language_Enum.French)
                    {
                        show_toc_text = "VOIR L'INDEX";
                    }
                    if (RequestSpecificValues.Current_Mode.Language == Web_Language_Enum.Spanish)
                    {
                        show_toc_text = "MOSTRAR INDICE";
                    }

                    Output.WriteLine("\t<div class=\"sbkIsw_ShowTocRow\">");
                    string redirect_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode).Replace("&", "&amp;");
                    if (redirect_url.IndexOf("?") < 0)
                        redirect_url = redirect_url + "?toc=y";
                    else
                        redirect_url = redirect_url + "&toc=y";
                    Output.WriteLine("\t\t<div class=\"sbkIsw_DownTOC\"><a href=\"" + redirect_url + "\">" + show_toc_text + "<img src=\"" + Static_Resources.Button_Down_Arrow_Png + "\" alt=\"\" /></a></div>");
                    // Output.WriteLine("\t\t<a href=\"" + redirect_url + "\">" + show_toc_text + "<div class=\"downarrow\"></div></a>");
                    Output.WriteLine("\t</div>");
                }

                // Allow the item/page viewer to show anything in the left navigational menu
                if (pageViewer != null)
                    pageViewer.Write_Left_Nav_Menu_Section(Output, Tracer);
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
            if (!String.IsNullOrEmpty(RequestSpecificValues.Current_Mode.Fragment))
                return;

            Tracer.Add_Trace("Item_HtmlSubwriter.Write_Additional_HTML", "Rendering HTML ( finish left navigation bar, begin main viewer section )");


            if (ShouldLeftNavigationBarBeShown)
            {
                // If this is an EAD-type item, show the table of contents here since it is all done
                // in HTML, and does not use the tree control

                //if (isEadTypeItem)
                //{
                //    EAD_Info eadInfo = (EAD_Info) currentItem.Get_Metadata_Module(GlobalVar.EAD_METADATA_MODULE_KEY);
                //    if ((eadInfo != null) && (eadInfo.TOC_Included_Sections.Count > 0))
                //    {
                //        // Determine the URL to use for most of these
                //        string description_link = String.Empty;
                //        if (RequestSpecificValues.Current_Mode.ViewerCode != "description")
                //            description_link = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode, "description");

                //        // Add the TOC as a floating DIV
                //        Output.WriteLine("      <div id=\"sbkEad_FloatingTOC\">");
                //        Output.WriteLine("      <ul class=\"sbkEad_TocMenu\">");
                //        Output.WriteLine("        <li class=\"sbkEad_TocHeader\">TABLE OF CONTENTS &nbsp; <span style=\"color:#eeeeee\"><a href=\"#\" title=\"Return to the top of this document\"><img src=\"" + RequestSpecificValues.Current_Mode.Base_URL + "design/skins/" + RequestSpecificValues.Current_Mode.Base_Skin_Or_Skin + "/buttons/up_arrow.gif\" /></a></span></li>");

                //        foreach (EAD_TOC_Included_Section thisMatch in eadInfo.TOC_Included_Sections)
                //        {
                //            Output.WriteLine("        <li><a href=\"" + description_link + "#" + thisMatch.Internal_Link_Name + "\">" + thisMatch.Section_Title + "</a></li>");
                //        }

                //        // Add the container list if there is one
                //        if (eadInfo.Container_Hierarchy.Containers.Count > 0)
                //        {
                //            Output.WriteLine("        <li><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode, "container") + "\">Container List</a></li>");
                //        }

                //        Output.WriteLine("      </ul>");
                //        Output.WriteLine("      </div>");
                //    }
                //}

                // Add any wordmarks
                if ((currentItem.Behaviors.Wordmarks != null) && (currentItem.Behaviors.Wordmarks.Count > 0))
                {
                    Output.WriteLine("\t<div id=\"sbkIsw_Wordmarks\">");

                    // Compute the URL options which may be needed
                    string url_options = UrlWriterHelper.URL_Options(RequestSpecificValues.Current_Mode);
                    string urlOptions1 = String.Empty;
                    string urlOptions2 = String.Empty;
                    if (url_options.Length > 0)
                    {
                        urlOptions1 = "?" + url_options;
                        urlOptions2 = "&" + url_options;
                    }

                    // Step through each wordmark mentioned in the brief item
                    foreach (string thisIcon in currentItem.Behaviors.Wordmarks)
                    {
                        // Look for a match in the dictionary
                        if (UI_ApplicationCache_Gateway.Icon_List.ContainsKey(thisIcon))
                        {
                            Wordmark_Icon wordmarkInfo = UI_ApplicationCache_Gateway.Icon_List[thisIcon];

                            Output.WriteLine("\t\t" + wordmarkInfo.HTML.Replace("<%BASEURL%>", RequestSpecificValues.Current_Mode.Base_URL).Replace("<%URLOPTS%>", url_options).Replace("<%?URLOPTS%>", urlOptions1).Replace("<%&URLOPTS%>", urlOptions2));
                        }
                    }

                    Output.WriteLine("\t</div>");
                }
                else
                {
                    Output.WriteLine("\t<div id=\"sbkIsw_NoWordmarks\">&nbsp;</div>");
                }


                Output.WriteLine("</nav>");
                Output.WriteLine();

                Output.WriteLine("<!-- Presentation table first cell ends and next will begin-->");
                Output.WriteLine("</td><td>");
            }


            // Begin the document display portion
            Output.WriteLine("<!-- Begin the main item viewing area -->");
            Output.WriteLine("<section id=\"main-content\" role=\"main\">");
            if (behaviors.Contains(HtmlSubwriter_Behaviors_Enum.Item_Subwriter_NonWindowed_Mode))
            {
                //if (pageViewer != null && pageViewer.Viewer_Height > 0)
                //    Output.WriteLine("<table id=\"sbkIsw_DocumentNonWindowed\" style=\"height:" + pageViewer.Viewer_Height + "px;\" >");
                //else
                    Output.WriteLine("<table id=\"sbkIsw_DocumentNonWindowed\" >");
            }
            else
            {
                //if ((pageViewer == null) || (pageViewer.Viewer_Width < 0))
                //{
                    Output.WriteLine("<table id=\"sbkIsw_DocumentDisplay2\" >");
                //}
                //else
                //{
                //    Output.WriteLine("<table id=\"sbkIsw_DocumentDisplay\" style=\"width:" + pageViewer.Viewer_Width + "px;\" >");
                //}

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

            #region Add navigation rows here (buttons for first, previous, next, last, etc.)

            // Add navigation row here (buttons and viewer specific)
            if (pageViewer != null)
            {
                // Allow the pageviewer to add any special elements to the main 
                // item viewer above the pagination
                pageViewer.Write_Top_Additional_Navigation_Row(Output, Tracer);

                // Should buttons be included here?
                if (pageViewer.PageCount != 1)
                {
                    Output.WriteLine("\t<tr>");
                    Output.WriteLine("\t\t<td>");

                    // ADD NAVIGATION BUTTONS
                    if (pageViewer.PageCount != 1)
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

                        if (RequestSpecificValues.Current_Mode.Language == Web_Language_Enum.Spanish)
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

                        if (RequestSpecificValues.Current_Mode.Language == Web_Language_Enum.French)
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
                        string firstButtonURL = pageViewer.First_Page_URL;
                        string prevButtonURL = pageViewer.Previous_Page_URL;

                        // Only continue if there is an item and mode, and there is previous pages to go to
                        if ((pageViewer.Current_Page > 1) && ((firstButtonURL.Length > 0) || (prevButtonURL.Length > 0)))
                        {
                            buttonsHtmlBuilder.AppendLine("\t\t\t\t<span class=\"sbkIsw_LeftPaginationButtons\">");
                            buttonsHtmlBuilder.AppendLine("\t\t\t\t\t<button title=\"" + first_page + "\" class=\"sbkIsw_RoundButton\" onclick=\"window.location='" + firstButtonURL + "'; return false;\"><img src=\"" + Static_Resources.Button_First_Arrow_Png + "\" class=\"roundbutton_img_left\" alt=\"\" />" + first_page_text + "</button>&nbsp;");
                            buttonsHtmlBuilder.AppendLine("\t\t\t\t\t<button title=\"" + previous_page + "\" class=\"sbkIsw_RoundButton\" onclick=\"window.location='" + prevButtonURL + "'; return false;\"><img src=\"" + Static_Resources.Button_Previous_Arrow_Png + "\" class=\"roundbutton_img_left\" alt=\"\" />" + previous_page_text + "</button>");
                            buttonsHtmlBuilder.AppendLine("\t\t\t\t</span>");
                        }

                        // Get the URL for the first and previous buttons
                        string lastButtonURL = pageViewer.Last_Page_URL;
                        string nextButtonURL = pageViewer.Next_Page_URL;

                        // Only continue if there is an item and mode, and there is previous pages to go to
                        if ((pageViewer.Current_Page < pageViewer.PageCount) && ((lastButtonURL.Length > 0) || (nextButtonURL.Length > 0)))
                        {
                            buttonsHtmlBuilder.AppendLine("\t\t\t\t<span class=\"sbkIsw_RightPaginationButtons\">");
                            buttonsHtmlBuilder.AppendLine("\t\t\t\t\t<button title=\"" + next_page + "\" class=\"sbkIsw_RoundButton\" onclick=\"window.location='" + nextButtonURL + "'; return false;\">" + next_page_text + "<img src=\"" + Static_Resources.Button_Next_Arrow_Png + "\" class=\"roundbutton_img_right\" alt=\"\" /></button>&nbsp;");
                            buttonsHtmlBuilder.AppendLine("\t\t\t\t\t<button title=\"" + last_page + "\" class=\"sbkIsw_RoundButton\" onclick=\"window.location='" + lastButtonURL + "'; return false;\">" + last_page_text + "<img src=\"" + Static_Resources.Button_Last_Arrow_Png + "\" class=\"roundbutton_img_right\" alt=\"\" /></button>");
                            buttonsHtmlBuilder.AppendLine("\t\t\t\t</span>");
                        }

                        // Write the buttons and save the HTML for the bottom of the page
                        buttonsHtml = buttonsHtmlBuilder.ToString();
                        Output.WriteLine(buttonsHtml);

                        // Show a pageselector, if one was selected
                        switch (pageViewer.Page_Selector)
                        {
                            case ItemViewer_PageSelector_Type_Enum.DropDownList:
                                string[] pageNames = pageViewer.Go_To_Names;
                                if (pageNames.Length > 0)
                                {
                                    // Determine if these page names are very long at all
                                    if (pageNames.Any(ThisName => ThisName.Length > 25))
                                    {
                                        // Long page names, so move the Go To: to the next line (new div)
                                        Output.WriteLine("\t\t\t</div>");
                                        Output.WriteLine("\t\t\t<div class=\"sbkIsw_PageNavBar2\">");
                                    }

                                    Output.WriteLine("\t\t\t\t<span id=\"sbkIsw_GoToSpan\"><label for=\"page_select\">" + go_to + "</label></span>");
                                    string orig_viewercode = RequestSpecificValues.Current_Mode.ViewerCode;
                                    string viewercode_only = RequestSpecificValues.Current_Mode.ViewerCode.Replace(RequestSpecificValues.Current_Mode.Page.ToString(), "");
                                    RequestSpecificValues.Current_Mode.ViewerCode = "XX1234567890XX";
                                    string url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
                                    RequestSpecificValues.Current_Mode.ViewerCode = orig_viewercode;

                                    Output.WriteLine("\t\t\t\t<select id=\"page_select\" name=\"page_select\" onchange=\"javascript:item_jump_sobekcm('" + url + "')\">");

                                    // Add all the page selection items to the combo box
                                    int page_index = 1;
                                    foreach (string thisName in pageNames)
                                    {
                                        if (thisName.Length > 75)
                                        {
                                            if (RequestSpecificValues.Current_Mode.Page == page_index)
                                            {
                                                Output.WriteLine("\t\t\t\t\t<option value=\"" + page_index + viewercode_only + "\" selected=\"selected\" >" + thisName.Substring(0, 75) + "..</option>");
                                            }
                                            else
                                            {
                                                Output.WriteLine("\t\t\t\t\t<option value=\"" + page_index + viewercode_only + "\">" + thisName.Substring(0, 75) + "..</option>");
                                            }
                                        }
                                        else
                                        {
                                            if (RequestSpecificValues.Current_Mode.Page == page_index)
                                            {
                                                Output.WriteLine("\t\t\t\t\t<option value=\"" + page_index + viewercode_only + "\" selected=\"selected\" >" + thisName + "</option>");
                                            }
                                            else
                                            {
                                                Output.WriteLine("\t\t\t\t\t<option value=\"" + page_index + viewercode_only + "\">" + thisName + "</option>");
                                            }
                                        }
                                        page_index++;
                                    }

                                    Output.WriteLine("\t\t\t\t</select>");
                                }
                                break;

                            case ItemViewer_PageSelector_Type_Enum.PageLinks:
                                // Create the page selection if that is the type to display.  This is where it is actually
                                // built as well, althouogh it is subsequently used further up the page
                                if (pageViewer.Page_Selector == ItemViewer_PageSelector_Type_Enum.PageLinks)
                                {
                                    StringBuilder pageLinkBuilder = new StringBuilder();

                                    //Get the total page count
                                    int num_of_pages = pageViewer.PageCount;
                                    string[] page_urls = pageViewer.Go_To_Names;

                                    pageLinkBuilder.AppendLine("\t\t\t\t<div class=\"sbkIsw_PageLinks\">");

                                    //Display the first, last, current page numbers, and 2 pages before and after the current page
                                    if (num_of_pages <= 7 && num_of_pages > 1)
                                    {
                                        for (int i = 1; i <= num_of_pages; i++)
                                        {
                                            if (i == pageViewer.Current_Page)
                                                pageLinkBuilder.AppendLine("\t\t\t\t\t" + i + "&nbsp;");
                                            else
                                                pageLinkBuilder.AppendLine("\t\t\t\t\t<a href=\"" + page_urls[i - 1] + "\">" + i + "</a>&nbsp;");
                                        }
                                    }
                                    else if (num_of_pages > 7)
                                    {
                                        if (pageViewer.Current_Page > 4 && pageViewer.Current_Page < num_of_pages - 3)
                                        {
                                            pageLinkBuilder.AppendLine("\t\t\t\t\t<a href=\"" + page_urls[0] + "\">" + 1 + "</a>" + "..");
                                            for (int i = pageViewer.Current_Page - 2; i <= pageViewer.Current_Page + 2; i++)
                                            {
                                                if (i == pageViewer.Current_Page)
                                                    pageLinkBuilder.AppendLine("\t\t\t\t\t" + i + "&nbsp;");
                                                else
                                                    pageLinkBuilder.AppendLine("\t\t\t\t\t<a href=\"" + page_urls[i - 1] + "\">" + i + "</a>&nbsp;");
                                            }
                                            pageLinkBuilder.AppendLine("\t\t\t\t\t.." + "<a href=\"" + page_urls[page_urls.Length - 1] + "\">" + num_of_pages + "</a>");
                                        }

                                        else if (pageViewer.Current_Page <= 4 && pageViewer.Current_Page < num_of_pages - 3)
                                        {
                                            for (int i = 1; i <= (pageViewer.Current_Page + 2); i++)
                                            {
                                                if (i == pageViewer.Current_Page)
                                                    pageLinkBuilder.AppendLine("\t\t\t\t\t" + i + "&nbsp;");
                                                else
                                                    pageLinkBuilder.AppendLine("\t\t\t\t\t<a href=\"" + page_urls[i - 1] + "\">" + i + "</a>&nbsp;");
                                            }
                                            pageLinkBuilder.AppendLine("\t\t\t\t\t.." + "<a href=\"" + page_urls[page_urls.Length - 1] + "\">" + num_of_pages + "</a>");
                                        }

                                        else if (pageViewer.Current_Page > 4 && pageViewer.Current_Page >= num_of_pages - 3)
                                        {
                                            pageLinkBuilder.AppendLine("\t\t\t\t\t<a href=\"" + page_urls[0] + "\">" + 1 + "</a>" + "..");
                                            for (int i = pageViewer.Current_Page - 2; i <= num_of_pages; i++)
                                            {
                                                if (i == pageViewer.Current_Page)
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

            // Add the HTML from the pageviewer, the main viewer section
            Tracer.Add_Trace("Item_MainWriter.Write_Additional_HTML", "Allowing page viewer to write directly to the output to add main viewer section");
            pageViewer.Write_Main_Viewer_Section(Output, Tracer);
        }

        /// <summary> Performs the final HTML writing which completes the item table and adds the final page navigation buttons at the bottom of the page </summary>
        /// <param name="Main_PlaceHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form, widely used throughout the application</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public void Add_Main_Viewer_Section(PlaceHolder Main_PlaceHolder, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Item_HtmlSubwriter.Add_Main_Viewer_Section", "Rendering HTML ( add any controls which the item viewer needs to add )");

            Tracer.Add_Trace("Item_HtmlSubwriter.Add_Main_Viewer_Section", "Rendering HTML ( add any controls which the item viewer needs to add )");

            // Add the main viewer section
            if (pageViewer != null)
            {
                Tracer.Add_Trace("Html_MainWriter.Add_Controls", "Allowing page viewer to add main viewer section to <i>mainPlaceHolder</i>");
                pageViewer.Add_Main_Viewer_Section(Main_PlaceHolder, Tracer);
            }
        }

        /// <summary> Writes final HTML to the output stream after all the placeholders and just before the itemNavForm is closed.  </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Write_ItemNavForm_Closing(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Item_HtmlSubwriter.Write_ItemNavForm_Closing", "Close the item viewer and add final pagination");

            Tracer.Add_Trace("Item_HtmlSubwriter.Write_ItemNavForm_Closing", "Close the item viewer and add final pagination");

            // If this is the page turner viewer, don't draw anything else
            //if ((pageViewer != null) && (pageViewer.ItemViewer_Type == ItemViewer_Type_Enum.GnuBooks_PageTurner))
            //{
            //    return;
            //}


            Output.WriteLine("\t</tr>");

            if ((pageViewer != null) && (pageViewer.PageCount != 1) && (!behaviors.Contains(HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Suppress_Bottom_Pagination)))
            {
                Output.WriteLine("\t<tr>");
                Output.WriteLine("\t\t<td>");

                // ADD NAVIGATION BUTTONS
                if (pageViewer.PageCount != 1)
                {
                    Output.WriteLine("\t\t\t<div class=\"sbkIsw_PageNavBar\">");
                    Output.WriteLine(buttonsHtml);

                    // Create the page selection if that is the type to display.  This is where it is actually
                    // built as well, althouogh it is subsequently used further up the page
                    if (pageViewer.Page_Selector == ItemViewer_PageSelector_Type_Enum.PageLinks)
                    {
                        Output.WriteLine(pageLinksHtml);
                    }

                    Output.WriteLine("\t\t\t</div>");
                }

                Output.WriteLine("\t\t</td>");
                Output.WriteLine("\t</tr>");
            }

            //if (pageViewer != null && ((currentItem.Behaviors.Single_Use) && (pageViewer.ItemViewer_Type != ItemViewer_Type_Enum.Checked_Out)))
            //{
            //    Output.WriteLine("<tr><td><span id=\"sbkIsw_CheckOutRequired\">This item contains copyrighted material and is reserved for single (fair) use.  Once you finish working with this item,<br />it will return to the digital stacks in fifteen minutes for another patron to use.<br /><br /></span></td></tr>");
            //}
            Output.WriteLine("</table>");
            Output.WriteLine("</section>");

            // Add a spot for padding
            Output.WriteLine();
            Output.WriteLine("<!-- Division is used to add extra bottom padding, if the left nav bar is taller than the item viewer -->");
            Output.WriteLine("<div id=\"sbkIsw_BottomPadding\"></div>");
            Output.WriteLine();

            // Close the item scope div
            Output.WriteLine("<!-- Close microdata itemscope div -->");
            Output.WriteLine("</section>");
            Output.WriteLine();

            Output.WriteLine("<!-- Close the presentation table -->");
            Output.WriteLine("</td></tr></table>");

            // None of the sharing options are available if the user is restricted from this item
            // or if we are generating this as a static page source for robots
            if ((!itemRestrictedFromUserByIp) && (!itemCheckedOutByOtherUser) && (!RequestSpecificValues.Current_Mode.Is_Robot))
            {
                // Add the hidden field
                Output.WriteLine("<!-- Hidden field is used for postbacks to indicate what to save and reset -->");
                Output.WriteLine("<input type=\"hidden\" id=\"item_action\" name=\"item_action\" value=\"\" />");
                Output.WriteLine();
            }
        }

        /// <summary> Spot to write any final HTML to the response stream  </summary>
        /// <param name="Output"> Stream to which to write the HTML for this subwriter </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Final_HTML(TextWriter Output, Custom_Tracer Tracer )
        {
            Tracer.Add_Trace("Item_HtmlSubwriter.Write_Final_Html", "Add reference to draggable jquery ui");
	        if (!behaviors.Contains(HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Full_JQuery_UI))
	        {
                Output.WriteLine("<script type=\"text/javascript\" src=\"" + Static_Resources.Jquery_Ui_1_10_3_Draggable_Js + "\"></script>");
	        }


        }

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
                if (pageViewer != null)
                    pageViewer.Add_ViewerSpecific_Body_Attributes(returnValue);
                return returnValue;
            }
        }

        /// <summary> Title for this web page </summary>
        public override string WebPage_Title
        {
            get
            {
                return currentItem != null ? currentItem.Title : "{0} Item";
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
            Output.WriteLine("  <link href=\"" + Static_Resources.Sobekcm_Item_Css + "\" rel=\"stylesheet\" type=\"text/css\" />");

            // Add any viewer specific tags that need to reside within the HTML head
            if (pageViewer != null)
                pageViewer.Write_Within_HTML_Head(Output, Tracer);

            // Add a thumbnail to this item
            if ((currentItem != null) && (currentItem.Behaviors.Main_Thumbnail.Length > 0))
            {
                string image_src = currentItem.Web.Source_URL + "/" + currentItem.Behaviors.Main_Thumbnail;

                Output.WriteLine("  <link rel=\"image_src\" href=\"" + image_src.Replace("\\", "/").Replace("//", "/").Replace("http:/", "http://") + "\" />");
            }
        }

		/// <summary> Gets the CSS class of the container that the page is wrapped within </summary>
		/// <value> Always returns an empty string </value>
		public override string Container_CssClass
		{
			get { return String.Empty; }
		}

        private bool ShouldLeftNavigationBarBeShown
        {
            get
            {
                // If this is for a fragment, do nothing
                if (!String.IsNullOrEmpty(RequestSpecificValues.Current_Mode.Fragment))
                    return false;

                if (behaviors.Contains(HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Suppress_Left_Navigation_Bar))
                    return false;

                if (behaviors.Contains(HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Requires_Left_Navigation_Bar))
                    return true;

                // If there are any icons, need to show the bar
                if (( currentItem.Behaviors.Wordmarks != null ) && ( currentItem.Behaviors.Wordmarks.Count > 0))
                    return true;

                //// If the item can be described, include the quick links still
                //if (currentItem.Behaviors.Can_Be_Described)
                //    return true;

                // If a TOC could be shown for this item, need a navigation bar
                if ((currentItem.Images_TOC != null) && (currentItem.Images_TOC.Count > 1))
                    return true;

                // Search results are also included in the left navigation bar
                if (searchResultsCount > 0)
                    return true;

                return false;
            }
        }

        #region Code to add the table of contents tree as a control into the left navigation bar

        /// <summary> Adds the table of contents as a control in the left navigation bar </summary>
        /// <param name="TocPlaceHolder"> TOC place holder ( &quot;tocPlaceHolder&quot; ) in the itemNavForm form, widely used throughout the application</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public void Add_Standard_TOC(PlaceHolder TocPlaceHolder, Custom_Tracer Tracer)
        {
            if (!ShouldLeftNavigationBarBeShown)
                return;

            if ((showToc) && (currentItem.Images_TOC != null ) && (currentItem.Images_TOC.Count > 1))
            {
                Tracer.Add_Trace("Item_HtmlSubwriter.Add_Standard_TOC", "Adding Table of Contents control to <i>TOC_PlaceHolder</i>");

                string table_of_contents = "TABLE OF CONTENTS";
                string hide_toc = "HIDE TABLE OF CONTENTS";

                if (RequestSpecificValues.Current_Mode.Language == Web_Language_Enum.French)
                {
                    table_of_contents = "TABLE DES MATIERES";
                    hide_toc = "MASQUER L'INDEX";
                }

                if (RequestSpecificValues.Current_Mode.Language == Web_Language_Enum.Spanish)
                {
                    table_of_contents = "INDICE";
                    hide_toc = "ESCONDA INDICE";
                }

                // Add the HTML to start this menu section
                Literal menuStartLiteral = new Literal();
                string redirect_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode).Replace("&", "&amp;");
                if (redirect_url.IndexOf("?") < 0)
                    redirect_url = redirect_url + "?toc=n";
                else
                    redirect_url = redirect_url + "&toc=n";


                menuStartLiteral.Text = string.Format("        <div class=\"sbkIsw_ShowTocRow\">" + Environment.NewLine +
                    "          <a href=\"{1}\"><div class=\"sbkIsw_UpToc\">{4}<img src=\"" + Static_Resources.Button_Up_Arrow_Png + "\" alt=\"\" /></div></a>" + Environment.NewLine +
                    "        </div>", table_of_contents, redirect_url, RequestSpecificValues.Current_Mode.Base_URL, RequestSpecificValues.HTML_Skin.Base_Skin_Code, hide_toc);
                TocPlaceHolder.Controls.Add(menuStartLiteral);

                // Create the treeview
                treeView1 = new TreeView { CssClass = "sbkIsw_TocTreeView", ExpandDepth = 0, NodeIndent = 15 };
                treeView1.SelectedNodeChanged += treeView1_SelectedNodeChanged;

                // load the table of contents in the tree
                Create_TreeView_From_Divisions(treeView1);

                // Add the tree view to the placeholder
                TocPlaceHolder.Controls.Add(treeView1);
            }
            else
            {
                Tracer.Add_Trace("Item_HtmlSubwriter.Add_TOC", "Table of contents is currently hidden");
            }
        }

        /// <summary> Event is fired if the currently selected tree view node changes. </summary>
        /// <param name="Sender"></param>
        /// <param name="E"></param>
        internal void treeView1_SelectedNodeChanged(object Sender, EventArgs E)
        {
            if (treeView1.SelectedNode != null)
            {
                string currentNodeID = treeView1.SelectedNode.Value;
                if ((currentNodeID.Length > 0) && (Convert.ToInt32(currentNodeID) > 0))
                {
                    RequestSpecificValues.Current_Mode.Page = Convert.ToUInt16(currentNodeID);
                    RequestSpecificValues.Current_Mode.ViewerCode = currentNodeID;
                }
                else
                {
                    RequestSpecificValues.Current_Mode.Page = 0;
                    RequestSpecificValues.Current_Mode.ViewerCode = "1";
                }

                // Redirect
                if (HttpContext.Current != null)
                    UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
            }
        }

        #endregion

        #region Methods to create the treeview control for the table of contents

        /// <summary> Populates the tree view with the divisions from the current digital resource item </summary>
        /// <param name="TreeViewArg"> Tree view control to populate </param>
        public void Create_TreeView_From_Divisions(TreeView TreeViewArg)
        {
            tocSelectedComplete = false;

            // Get the current mode page
            List<TreeNode> nodes = new List<TreeNode>();
            List<TreeNode> selectedNodes = new List<TreeNode>();

            //int sequence = 0;
            foreach (BriefItem_TocElement divNode in currentItem.Images_TOC)
            {
                TreeNode treeViewNode = new TreeNode { Text = string.Format("<span class=\"sbkIsw_TocTreeViewItem\" Title=\"{0}\">{1}</span>", divNode.Name, divNode.Shortened_Name) };
                TreeViewArg.Nodes.Add(treeViewNode);
                nodes.Add(treeViewNode);
               // List<TreeNode> pathNodes = new List<TreeNode> { treeViewNode };
               // recurse_through_tree(divNode, treeViewNode, nodes, selectedNodes, pathNodes, ref sequence);
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

        //private void recurse_through_tree(BriefItem_TocElement ParentNode, TreeNode ParentViewNode, List<TreeNode> Nodes, List<TreeNode> SelectedNodes, List<TreeNode> PathNodes, ref int Sequence)
        //{
        //    foreach (abstract_TreeNode absNode in ParentNode.Nodes)
        //    {
        //        if (absNode.Page)
        //        {
        //            Sequence++;

        //            foreach (TreeNode thisNode in Nodes)
        //            {
        //                thisNode.Value = Sequence.ToString();
        //            }
        //            if (Sequence >= RequestSpecificValues.Current_Mode.Page)
        //            {
        //                if (!tocSelectedComplete)
        //                {
        //                    SelectedNodes.AddRange(PathNodes);
        //                    tocSelectedComplete = true;
        //                }
        //                else
        //                {
        //                    if (Sequence == RequestSpecificValues.Current_Mode.Page)
        //                    {
        //                        SelectedNodes.AddRange(PathNodes);
        //                    }
        //                }
        //            }
        //            Nodes.Clear();
        //        }
        //        else
        //        {
        //            Division_TreeNode divNode = (Division_TreeNode)absNode;
        //            TreeNode treeViewNode = new TreeNode { Text = string.Format("<span class=\"SobekTocTreeViewItem\" Title='{0}'>{1}</span>", divNode.Display_Label, divNode.Display_Short_Label) };
        //            ParentViewNode.ChildNodes.Add(treeViewNode);
        //            Nodes.Add(treeViewNode);
        //            List<TreeNode> pathNodes2 = new List<TreeNode> { treeViewNode };
        //            recurse_through_tree(divNode, treeViewNode, Nodes, SelectedNodes, pathNodes2, ref Sequence);
        //        }
        //    }
        //}

        #endregion
    }
}
