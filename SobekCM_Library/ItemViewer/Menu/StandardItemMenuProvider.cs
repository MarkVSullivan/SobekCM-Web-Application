using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using System.Windows.Forms.VisualStyles;
using SobekCM.Core.BriefItem;
using SobekCM.Core.Navigation;
using SobekCM.Core.UI_Configuration;
using SobekCM.Core.Users;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Library.ItemViewer.Viewers;
using SobekCM.Library.UI;
using SobekCM.Tools;

namespace SobekCM.Library.ItemViewer.Menu
{
    public class StandardItemMenuProvider : iItemMenuProvider
    {

        public void Add_Main_Menu(TextWriter Output, string CurrentCode, bool ItemRestrictedFromUserByIP, bool ItemCheckedOutByOtherUser, BriefItemInfo CurrentItem, Navigation_Object CurrentMode, User_Object CurrentUser, Custom_Tracer Tracer)
        {
            // Set some flags based on the resource type
            bool is_bib_level = (String.Compare(CurrentItem.Type, "BIB_LEVEL", StringComparison.OrdinalIgnoreCase) == 0);
            bool is_ead = (String.Compare(CurrentItem.Type, "EAD", StringComparison.OrdinalIgnoreCase) == 0);

            // Determine if this user can edit this item
            bool userCanEditItem = false;
            if (CurrentUser != null)
            {
                userCanEditItem = CurrentUser.Can_Edit_This_Item(CurrentItem.BibID, CurrentItem.Type, CurrentItem.Behaviors.Source_Institution_Aggregation, CurrentItem.Behaviors.Holding_Location_Aggregation, CurrentItem.Behaviors.Aggregation_Code_List);
            }

            // Can this user (if there is one) edit this item?
            bool canManage = (CurrentUser != null) && (CurrentUser.Can_Edit_This_Item(CurrentItem.BibID, CurrentItem.Type, CurrentItem.Behaviors.Source_Institution_Aggregation, CurrentItem.Behaviors.Holding_Location_Aggregation, CurrentItem.Behaviors.Aggregation_Code_List));

            // Add the item views
            Output.WriteLine("<!-- Add the different view and social options -->");
            Output.WriteLine("<nav class=\"sbkMenu_Bar\" id=\"sbkIsw_MenuBar\" role=\"navigation\" aria-label=\"Item menu\">");
            Output.WriteLine("\t<h2 class=\"hidden-element\">Item menu</h2>");

            // Check that this item is not checked out by another user
            bool itemCheckedOutByOtherUser = false;
            if (CurrentItem.Behaviors.Single_Use)
            {
                if (!Engine_ApplicationCache_Gateway.Checked_List.Check_Out(CurrentItem.Web.ItemID, HttpContext.Current.Request.UserHostAddress))
                {
                    itemCheckedOutByOtherUser = true;
                }
            }

            // Add the sharing buttons if this is not restricted by IP address or checked out
            if ((!ItemRestrictedFromUserByIP) && (!ItemCheckedOutByOtherUser) && (!CurrentMode.Is_Robot))
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
                bool isLoggedOn = CurrentUser != null && CurrentUser.LoggedOn;
                if (!isLoggedOn)
                {
                    string returnUrl = UrlWriterHelper.Redirect_URL(CurrentMode);

                    CurrentMode.Mode = Display_Mode_Enum.My_Sobek;
                    CurrentMode.My_Sobek_Type = My_Sobek_Type_Enum.Logon;
                    CurrentMode.Return_URL = returnUrl;
                    logOnUrl = UrlWriterHelper.Redirect_URL(CurrentMode);
                    CurrentMode.Mode = Display_Mode_Enum.Item_Display;
                    CurrentMode.Return_URL = String.Empty;
                }

                Output.WriteLine("\t<div id=\"menu-right-actions\">");

                if (CurrentItem.Web.ItemID > 0)
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


                    if (CurrentItem.Web.ItemID > 0)
                    {
                        if (CurrentUser.Is_In_Bookshelf(CurrentItem.BibID, CurrentItem.VID))
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

                    if (CurrentItem.Web.ItemID > 0)
                        Output.WriteLine("\t\t<span id=\"addbuttonitem\" class=\"action-sf-menu-item\" onclick=\"window.location='" + logOnUrl + "';return false;\"><img src=\"" + Static_Resources.Plussign_Png + "\" alt=\"\" style=\"vertical-align:middle\" /><span id=\"addbuttonspan\">" + add_text + "</span></span>");
                }

                Output.WriteLine("\t\t<span id=\"sharebuttonitem\" class=\"action-sf-menu-item\" onclick=\"toggle_share_form('share_button');\"><span id=\"sharebuttonspan\">Share</span></span>");


                Output.WriteLine("\t</div>");
                Output.WriteLine();
            }


            Output.WriteLine("\t<ul class=\"sf-menu\" id=\"sbkIhs_Menu\">");


            // Save the current view type
            ushort page = CurrentMode.Page.HasValue ? CurrentMode.Page.Value : (ushort)1;
            ushort subpage = CurrentMode.SubPage.HasValue ? CurrentMode.SubPage.Value : (ushort)1;
            string viewerCode = CurrentMode.ViewerCode;
            CurrentMode.SubPage = 0;

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

            // Add the item level viewers - collect the menu portions
            List<Item_MenuItem> menuItems = new List<Item_MenuItem>();
            foreach (string viewType in CurrentItem.UI.Viewers_Menu_Order)
            {
                iItemViewerPrototyper prototyper = ItemViewer_Factory.Get_Viewer_By_ViewType(viewType);
                if (prototyper.Has_Access(CurrentItem, CurrentUser, ItemRestrictedFromUserByIP))
                {
                    prototyper.Add_Menu_items(CurrentItem, CurrentUser, CurrentMode, menuItems);
                }
            }

            // Now, get ready to start adding the menu items
            Dictionary<string, List<Item_MenuItem>> topMenuToChildren = new Dictionary<string, List<Item_MenuItem>>(StringComparer.OrdinalIgnoreCase);
            foreach (Item_MenuItem menuItem in menuItems)
            {
                if (topMenuToChildren.ContainsKey(menuItem.MenuStripText))
                    topMenuToChildren[menuItem.MenuStripText].Add(menuItem);
                else
                {
                    topMenuToChildren[menuItem.MenuStripText] = new List<Item_MenuItem> {menuItem};
                }
            }
            
            // Now, step through the menu items
            foreach (Item_MenuItem topMenuItem in menuItems)
            {
                HTML_Helper(Output, topMenuItem, CurrentMode, CurrentCode, topMenuToChildren );
            }


            //// Add the item level views
            //foreach (BriefItem_BehaviorViewer thisView in CurrentItem.Behaviors.Viewers)
            //{
            //    if (((!itemRestrictedFromUserByIp) && (!CurrentItem.Behaviors.Dark_Flag)) || (thisView.View_Type == View_Enum.CITATION) ||
            //        (thisView.View_Type == View_Enum.ALL_VOLUMES) ||
            //        (thisView.View_Type == View_Enum.RELATED_IMAGES))
            //    {
            //        // Special code for the CITATION view (TEMPORARY - v.3.2)
            //        if (thisView.View_Type == View_Enum.CITATION)
            //        {
            //            if (CurrentMode.Is_Robot)
            //            {
            //                Output.Write("\t\t<li class=\"selected-sf-menu-item-link\"><a href=\"\">Description</a></li>");
            //            }
            //            else
            //            {
            //                CurrentMode.ViewerCode = "citation";
            //                if (CurrentItem.Bib_Info.SobekCM_Type == TypeOfResource_SobekCM_Enum.EAD)
            //                    CurrentMode.ViewerCode = "description";
            //                if ((viewerCode == "citation") || (viewerCode == "marc") || (viewerCode == "metadata") ||
            //                    (viewerCode == "usage") || (viewerCode == "description"))
            //                {
            //                    Output.Write("\t\t<li class=\"selected-sf-menu-item-link\"><a href=\"" + UrlWriterHelper.Redirect_URL(CurrentMode) + "\">Description</a>");
            //                }
            //                else
            //                {
            //                    Output.Write("\t\t<li><a href=\"" + UrlWriterHelper.Redirect_URL(CurrentMode) + "\">Description</a>");
            //                }
            //                Output.WriteLine("<ul>");


            //                if (CurrentItem.Bib_Info.SobekCM_Type == TypeOfResource_SobekCM_Enum.EAD)
            //                    Output.WriteLine("\t\t\t<li><a href=\"" + UrlWriterHelper.Redirect_URL(CurrentMode) + "\">Archival Description</a></li>");
            //                else
            //                    Output.WriteLine("\t\t\t<li><a href=\"" + UrlWriterHelper.Redirect_URL(CurrentMode) + "\">Standard View</a></li>");



            //                CurrentMode.ViewerCode = "marc";
            //                Output.WriteLine("\t\t\t<li><a href=\"" + UrlWriterHelper.Redirect_URL(CurrentMode) + "\">MARC View</a></li>");


            //                CurrentMode.ViewerCode = "metadata";
            //                Output.WriteLine("\t\t\t<li><a href=\"" + UrlWriterHelper.Redirect_URL(CurrentMode) + "\">Metadata</a></li>");


            //                CurrentMode.ViewerCode = "usage";
            //                Output.WriteLine("\t\t\t<li><a href=\"" + UrlWriterHelper.Redirect_URL(CurrentMode) + "\">Usage Statistics</a></li>");


            //                Output.WriteLine("\t\t</ul></li>");
            //                CurrentMode.ViewerCode = viewerCode;
            //            }
            //        }
            //        else if (thisView.View_Type == View_Enum.ALL_VOLUMES)
            //        {
            //            string resource_type_upper = CurrentItem.Bib_Info.SobekCM_Type_String.ToUpper();
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

            //            if (CurrentMode.Is_Robot)
            //            {
            //                Output.Write("\t\t<li><a href=\"" + UI_ApplicationCache_Gateway.Settings.Servers.Base_URL + "\\" + CurrentItem.BibID + "\">" + all_volumes + "</a></li>");
            //            }
            //            else
            //            {

            //                CurrentMode.ViewerCode = "allvolumes";
            //                if ((viewerCode == "allvolumes") || (viewerCode == "allvolumes2") ||
            //                    (viewerCode == "allvolumes3"))
            //                {
            //                    Output.Write("\t\t<li class=\"selected-sf-menu-item-link\"><a href=\"" + UrlWriterHelper.Redirect_URL(CurrentMode) + "\">" + all_volumes + "</a>");
            //                }
            //                else
            //                {
            //                    Output.Write("\t\t<li><a href=\"" + UrlWriterHelper.Redirect_URL(CurrentMode) + "\">" + all_volumes + "</a>");
            //                }
            //                Output.WriteLine("<ul>");


            //                CurrentMode.ViewerCode = "allvolumes";
            //                Output.WriteLine("\t\t\t<li><a href=\"" + UrlWriterHelper.Redirect_URL(CurrentMode) + "\">Tree View</a></li>");


            //                CurrentMode.ViewerCode = "allvolumes2";
            //                Output.WriteLine("\t\t\t<li><a href=\"" + UrlWriterHelper.Redirect_URL(CurrentMode) + "\">Thumbnails</a></li>");


            //                if ((CurrentUser != null) && (CurrentUser.LoggedOn) && (CurrentUser.Is_Internal_User))
            //                {
            //                    CurrentMode.ViewerCode = "allvolumes3";
            //                    Output.WriteLine("\t\t\t<li><a href=\"" + UrlWriterHelper.Redirect_URL(CurrentMode) + "\">List View</a></li>");
            //                }


            //                Output.WriteLine("\t\t</ul></li>");
            //                CurrentMode.ViewerCode = viewerCode;
            //            }
            //        }
            //        else
            //        {
            //            List<string> item_nav_bar_link = Item_Nav_Bar_HTML_Factory.Get_Nav_Bar_HTML(thisView, CurrentItem.Bib_Info.SobekCM_Type_String, RequestSpecificValues.HTML_Skin.Base_Skin_Code, CurrentMode, -1, UI_ApplicationCache_Gateway.Translation, showZoomable, CurrentItem);


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
            //    (CurrentMode.ViewerCode == View_Object.Viewer_Code_By_Type(View_Enum.RELATED_IMAGES)[0]))
            //{
            //    CurrentMode.Page = 1;
            //}


            //if ((CurrentItem.Web.Static_PageCount > 0) && (RequestSpecificValues.Current_Page == null))
            //{
            //    RequestSpecificValues.Current_Page = CurrentItem.Web.Pages_By_Sequence[0];
            //}


            //// Add each page display type
            //if ((RequestSpecificValues.Current_Page != null) && (!itemRestrictedFromUserByIp))
            //{
            //    int page_seq = CurrentMode.Page.HasValue ? CurrentMode.Page.Value : 1;
            //    string resourceType = CurrentItem.Bib_Info.SobekCM_Type_String.ToUpper();
            //    if (CurrentItem.Behaviors.Item_Level_Page_Views_Count > 0)
            //    {
            //        List<string> pageViewLinks = new List<string>();

            //        foreach (View_Object thisPageView in CurrentItem.Behaviors.Item_Level_Page_Views)
            //        {
            //            View_Enum thisViewType = thisPageView.View_Type;
            //            foreach (SobekCM_File_Info thisFile in RequestSpecificValues.Current_Page.Files)
            //            {
            //                View_Object fileObject = thisFile.Get_Viewer();
            //                if ((fileObject != null) && (fileObject.View_Type == thisViewType))
            //                {
            //                    pageViewLinks.AddRange(Item_Nav_Bar_HTML_Factory.Get_Nav_Bar_HTML(thisFile.Get_Viewer(), resourceType, RequestSpecificValues.HTML_Skin.Base_Skin_Code, CurrentMode, page_seq, UI_ApplicationCache_Gateway.Translation, showZoomable, CurrentItem));
            //                }
            //            }
            //        }


            //        if (CurrentItem.BibID == "UF00001672")
            //        {
            //            string filename = RequestSpecificValues.Current_Page.Files[0].File_Name_Sans_Extension + ".txt";
            //            SobekCM_File_Info newFile = new SobekCM_File_Info(filename);
            //            pageViewLinks.AddRange(Item_Nav_Bar_HTML_Factory.Get_Nav_Bar_HTML(newFile.Get_Viewer(), resourceType, RequestSpecificValues.HTML_Skin.Base_Skin_Code, CurrentMode, page_seq, UI_ApplicationCache_Gateway.Translation, showZoomable, CurrentItem));
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
            //            if ((CurrentItem.Images != null ) && ( CurrentItem.Images.Count > 1 ))
            //                menu_title = menu_title + "s";


            //            // Get the link for the first page view
            //            string link = pageViewLinks[0].Substring(pageViewLinks[0].IndexOf("href=\"") + 6);
            //            link = link.Substring(0, link.IndexOf("\""));


            //            // Was this a match?
            //            if ((CurrentMode.ViewerCode == page_seq + "t") || (CurrentMode.ViewerCode == page_seq + "x") || (CurrentMode.ViewerCode == page_seq + "j"))
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
            //    List<string> restricted_nav_bar_link = Item_Nav_Bar_HTML_Factory.Get_Nav_Bar_HTML(new View_Object(View_Enum.RESTRICTED), CurrentItem.Bib_Info.SobekCM_Type_String.ToUpper(), RequestSpecificValues.HTML_Skin.Base_Skin_Code, CurrentMode, 0, UI_ApplicationCache_Gateway.Translation, showZoomable, CurrentItem);
            //    Output.WriteLine("\t\t" + restricted_nav_bar_link[0] + "");
            //}

            // Set current submode back
            CurrentMode.Page = page;
            CurrentMode.ViewerCode = viewerCode;
            CurrentMode.SubPage = subpage;

            Output.WriteLine("\t</ul>");
            Output.WriteLine("</nav>");
            Output.WriteLine();


            Output.WriteLine("<!-- Initialize the main item menu -->");
            Output.WriteLine("<script>");
            Output.WriteLine("\tjQuery(document).ready(function () { jQuery('ul.sf-menu').superfish(); });");
            Output.WriteLine("</script>");
            Output.WriteLine();
        }

        private static void HTML_Helper(TextWriter Output, Item_MenuItem MenuItem, Navigation_Object Current_Mode, string CurrentViewerCode, Dictionary<string, List<Item_MenuItem>> topMenuToChildren )
        {
            // If there are NO matches, left this top-level menu part was already taken care of
            if ((!topMenuToChildren.ContainsKey(MenuItem.MenuStripText)) || (topMenuToChildren[MenuItem.MenuStripText].Count == 0))
                return;

            // Is there only one menu part here
            List<Item_MenuItem> children = topMenuToChildren[MenuItem.MenuStripText];
            if ((children.Count == 1) && ( String.IsNullOrEmpty(MenuItem.MidMenuText )))
            {
                if (String.Compare(MenuItem.Code, CurrentViewerCode, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    Output.WriteLine("\t\t<li class=\"selected-sf-menu-item\">" + MenuItem.MenuStripText + "</li>");
                    return;
                }

                // When rendering for robots, provide the text and image, but not the link
                if (Current_Mode.Is_Robot)
                {
                    Output.WriteLine("\t\t<li class=\"selected-sf-menu-item\">" + MenuItem.MenuStripText + "</li>");
                }

                Output.WriteLine("\t\t<li><a href=\"" + MenuItem.Link + "\">" + MenuItem.MenuStripText + "</a></li>");
            }
            else
            {
                // Step through and see if this is selected
                bool selected = false;
                foreach (Item_MenuItem childMenu in children)
                {
                    if (String.Compare(childMenu.Code, CurrentViewerCode, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        selected = true;
                        break;
                    }
                }

                // Add the top-level
                string url = children[0].Link;
                if (selected)
                {
                    Output.WriteLine("\t\t<li class=\"selected-sf-menu-item-link\"><a href=\"" + url + "\">" + MenuItem.MenuStripText + "</a><ul>");
                }
                else
                {
                    Output.WriteLine("\t\t<li><a href=\"" + url + "\">" + MenuItem.MenuStripText + "</a><ul>");
                }

                // Add all the children
                foreach (Item_MenuItem childMenu in children)
                {
                    //if ((String.Compare(childMenu.Code, CurrentViewerCode, StringComparison.OrdinalIgnoreCase) == 0) || (Current_Mode.Is_Robot))
                    //{
                    //    Output.WriteLine("            <li class=\"selected-sf-menu-item\">" + childMenu.MidMenuText + "</li>");
                    //}
                    //else
                    //{
                    Output.WriteLine("\t\t\t<li><a href=\"" + childMenu.Link + "\">" + childMenu.MidMenuText + "</a></li>");
                    //}
                }

                // Close this top-level list and menu
                Output.WriteLine("\t\t</ul></li>");
            }

            // Clear this
            topMenuToChildren[MenuItem.MenuStripText].Clear();
        }
    }
}
