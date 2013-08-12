using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SobekCM.Library.Configuration;
using SobekCM.Library.Navigation;
using SobekCM.Library.Users;

namespace SobekCM.Library.HTML
{
    /// <summary> Static class is used to write the main menu used for most of
    /// the pages which require a user to be logged in, such as the mySobek pages,
    /// the internal user pages, and the system administration pages </summary>
    public static class UserSpecific_MainMenu_Writer
    {


        public static void Add_Main_Menu(TextWriter Output, SobekCM_Navigation_Object Mode, User_Object User)
        {
            // Add the item views
            Output.WriteLine("<!-- Add the main user-specific menu -->");
            Output.WriteLine("<div id=\"sf-menubar\">");
            Output.WriteLine("<ul class=\"sf-menu\">");

            // Save the current view information type
            Display_Mode_Enum currentMode = Mode.Mode;
            Admin_Type_Enum adminType = Mode.Admin_Type;
            My_Sobek_Type_Enum mySobekType = Mode.My_Sobek_Type;
            Internal_Type_Enum internalType = Mode.Internal_Type;
            Result_Display_Type_Enum resultType = Mode.Result_Display_Type;
            string mySobekSubmode = Mode.My_Sobek_SubMode;
            ushort page = Mode.Page;

            // Ensure some values that SHOULD be blank really are
            Mode.Aggregation = String.Empty;

            // Get ready to draw the tabs
            string sobek_home_text = Mode.SobekCM_Instance_Abbreviation + " Home";
            string my_sobek_home_text = "<span style=\"text-transform:lowercase\">my</span>" + Mode.SobekCM_Instance_Abbreviation + " Home";
            const string myLibrary = "My Library";
            const string myPreferences = "My Account";
            const string internal_text = "Internal";
            string sobek_admin_text = "System Admin";
            if ((User != null) && (User.Is_Portal_Admin) && (!User.Is_System_Admin))
                sobek_admin_text = "Portal Admin";
            const string list_view_text = "List View";
            const string brief_view_text = "Brief View";
            const string tree_view_text = "Tree View";
            const string partners_text = "Browse Partners";
            const string advanced_search_text = "Advanced Search";

            string collection_details_text = "Collection Hierarchy";
            string new_items_text = "New Items";
            string memory_mgmt_text = "Memory Management";
            string wordmarks_text = "Wordmarks";
            string build_failures_text = "Build Failures";

            if (Mode.Language == Web_Language_Enum.Spanish)
            {
                //title = "INICIO";
                collection_details_text = "DETALLES DE LA COLECCIÓN";
                new_items_text = "NUEVOS ARTÍCULOS";
                memory_mgmt_text = "MEMORIA";
            }

            if (Mode.Language == Web_Language_Enum.French)
            {
                //title = "PAGE D'ACCUEIL";
                collection_details_text = "DETAILS DE LA COLLECTION";
                new_items_text = "LES NOUVEAUX DOCUMENTS";
                memory_mgmt_text = "MÉMOIRE";
            }

            // Add the 'SOBEK HOME' first menu option and suboptions
            Mode.Mode = Display_Mode_Enum.Aggregation_Home;
            Mode.Home_Type = Home_Type_Enum.List;
            Output.WriteLine("\t\t<li id=\"sbkUsm_Home\"><a href=\"" + Mode.Redirect_URL() + "\" class=\"sbkUsm_NoPadding\"><img src=\"" + Mode.Default_Images_URL + "home.png\" /> <div class=\"sbkUsm_HomeText\">" + sobek_home_text + "</div></a><ul id=\"sbkUsm_HomeSubMenu\">");
            Output.WriteLine("\t\t\t<li id=\"sbkUsm_HomeListView\"><a href=\"" + Mode.Redirect_URL() + "\">" + list_view_text + "</a></li>");
            Mode.Home_Type = Home_Type_Enum.Descriptions;
            Output.WriteLine("\t\t\t<li id=\"sbkUsm_HomeBriefView\"><a href=\"" + Mode.Redirect_URL() + "\">" + brief_view_text + "</a></li>");
            if (SobekCM_Library_Settings.Include_TreeView_On_System_Home)
            {
                Mode.Home_Type = Home_Type_Enum.Tree_Collapsed;
                Output.WriteLine("\t\t\t<li id=\"sbkUsm_HomeTreeView\"><a href=\"" + Mode.Redirect_URL() + "\">" + tree_view_text + "</a></li>");
            }
            if (SobekCM_Library_Settings.Include_Partners_On_System_Home)
            {
                Mode.Home_Type = Home_Type_Enum.Partners_List;
                Output.WriteLine("\t\t\t<li id=\"sbkUsm_HomePartners\"><a href=\"" + Mode.Redirect_URL() + "\">" + partners_text + "</a></li>");
            }
            string advanced_url = Mode.Base_URL + "/advanced";
            Output.WriteLine("\t\t\t<li id=\"sbkUsm_HomeAdvSearch\"><a href=\"" + advanced_url + "\">" + advanced_search_text + "</a></li>");
            Output.WriteLine("\t\t</ul></li>");

            // Add the 'mySOBEK HOME' second menu option and suboptions
            Mode.Mode = Display_Mode_Enum.My_Sobek;
            Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
            Output.WriteLine("\t\t<li><a href=\"" + Mode.Redirect_URL() + "\">" + my_sobek_home_text + "</a><ul id=\"sbkUsm_MySubMenu\">");

            // If a user can submit, add a link to start a new item
            if ((User.Can_Submit) && ( SobekCM_Library_Settings.Online_Edit_Submit_Enabled ))
            {
                    Mode.My_Sobek_Type = My_Sobek_Type_Enum.New_Item;
                    Mode.My_Sobek_SubMode = "1";
                    Output.WriteLine("\t\t\t<li id=\"sbkUsm_MyStartNew\"><a href=\"" + Mode.Redirect_URL() + "\"><img src=\"" + Mode.Default_Images_URL + "new_item.gif\" /> <div class=\"sbkUsm_TextWithImage\">Start a new item</div></a></li>");
            }

            // If the user has already submitted stuff, add a link to all submitted items
            if (User.Items_Submitted_Count > 0)
            {
                Mode.My_Sobek_Type = My_Sobek_Type_Enum.Folder_Management;
                Mode.Result_Display_Type = Result_Display_Type_Enum.Brief;
                Mode.My_Sobek_SubMode = "Submitted Items";
                Output.WriteLine("\t\t\t<li id=\"sbkUsm_MySubmittedItems\"><a href=\"" + Mode.Redirect_URL() + "\"><img src=\"" + Mode.Default_Images_URL + "submitted_items.gif\" /> <div class=\"sbkUsm_TextWithImage\">View my submitted items</div></a></li>");
            }

            // If this user is linked to item statistics, add that link as well
            if (User.Has_Item_Stats)
            {
                // Add link to folder management
                Mode.My_Sobek_Type = My_Sobek_Type_Enum.User_Usage_Stats;
                Mode.My_Sobek_SubMode = String.Empty;
                Output.WriteLine("\t\t\t<li id=\"sbkUsm_MyItemStats\"><a href=\"" + Mode.Redirect_URL() + "\"><img src=\"" + Mode.Default_Images_URL + "usage.png\" /> <div class=\"sbkUsm_TextWithImage\">View usage for my items</div></a></li>");
            }

            // If the user has submitted some descriptive tags, or has the kind of rights that let them
            // view lists of tags, add that
            if ((User.Has_Descriptive_Tags) || (User.Is_System_Admin) || (User.Is_A_Collection_Manager_Or_Admin))
            {
                // Add link to folder management
                Mode.My_Sobek_Type = My_Sobek_Type_Enum.User_Tags;
                Mode.My_Sobek_SubMode = String.Empty;
                Output.WriteLine("\t\t\t<li id=\"sbkUsm_MyTags\"><a href=\"" + Mode.Redirect_URL() + "\"><img src=\"" + Mode.Default_Images_URL + "chat.png\" /> <div class=\"sbkUsm_TextWithImage\">View my descriptive tags</div></a></li>");
            }

            // Add link to folder management
            Mode.My_Sobek_Type = My_Sobek_Type_Enum.Folder_Management;
            Mode.My_Sobek_SubMode = String.Empty;
            Mode.Result_Display_Type = Result_Display_Type_Enum.Bookshelf;
            Output.WriteLine("\t\t\t<li id=\"sbkUsm_MyBookshelf\"><a href=\"" + Mode.Redirect_URL() + "\"><img src=\"" + Mode.Default_Images_URL + "bookshelf.png\" /> <div class=\"sbkUsm_TextWithImage\">View my bookshelves</div></a></li>");

            // Add a link to view all saved searches
            Mode.My_Sobek_Type = My_Sobek_Type_Enum.Saved_Searches;
            Output.WriteLine("\t\t\t<li id=\"sbkUsm_MySearches\"><a href=\"" + Mode.Redirect_URL() + "\"><img src=\"" + Mode.Default_Images_URL + "saved_searches.gif\" /> <div class=\"sbkUsm_TextWithImage\">View my saved searches</div></a></li>");

            // Add a link to edit your preferences
            Mode.My_Sobek_Type = My_Sobek_Type_Enum.Preferences;
            Output.WriteLine("\t\t\t<li id=\"sbkUsm_MyAccount\"><a href=\"" + Mode.Redirect_URL() + "\"><img src=\"" + Mode.Default_Images_URL + "settings.gif\" /> <div class=\"sbkUsm_TextWithImage\">Account preferences</div></a></li>");

            // Add a log out link
            Mode.My_Sobek_Type = My_Sobek_Type_Enum.Log_Out;
            Output.WriteLine("\t\t\t<li id=\"sbkUsm_MyLogOut\"><a href=\"" + Mode.Redirect_URL() + "\"><img src=\"" + Mode.Default_Images_URL + "exit.gif\" /> <div class=\"sbkUsm_TextWithImage\">Log Out</div></a></li>");

            Output.WriteLine("\t\t</ul></li>");


            // Add link to my libary (repeat of option in mySobek menu)
            Mode.My_Sobek_Type = My_Sobek_Type_Enum.Folder_Management;
            Mode.My_Sobek_SubMode = String.Empty;
            Mode.Result_Display_Type = Result_Display_Type_Enum.Bookshelf;
            Output.WriteLine("\t\t<li id=\"sbkUsm_Bookshelf\"><a href=\"" + Mode.Redirect_URL() + "\">My Library</a></li>");

            // Add a link to my account (repeat of option in mySobek menu)
            Mode.My_Sobek_Type = My_Sobek_Type_Enum.Preferences;
            Output.WriteLine("\t\t\t<li id=\"sbkUsm_Account\"><a href=\"" + Mode.Redirect_URL() + "\">My Account</a></li>");

            // If this user is internal, add that
            if ((User.Is_Internal_User) || (User.Is_Portal_Admin) || (User.Is_System_Admin))
            {
                Mode.Mode = Display_Mode_Enum.Internal;
                Mode.Internal_Type = Internal_Type_Enum.Aggregations_List;
                Output.WriteLine("\t\t<li id=\"sbkUsm_Internal\"><a href=\"" + Mode.Redirect_URL() + "\">" + internal_text + "</a><ul id=\"sbkUsm_InternalSubMenu\">");

                Mode.Internal_Type = Internal_Type_Enum.Aggregations_List;
                Output.WriteLine("\t\t\t<li id=\"sbkUsm_InternalAggregations\"><a href=\"" + Mode.Redirect_URL() + "\">" + collection_details_text + "</a></li>");

                Mode.Internal_Type = Internal_Type_Enum.New_Items;
                Output.WriteLine("\t\t\t<li id=\"sbkUsm_InternalNewItems\"><a href=\"" + Mode.Redirect_URL() + "\">" + new_items_text + "</a></li>");

                Mode.Internal_Type = Internal_Type_Enum.Build_Failures;
                Output.WriteLine("\t\t\t<li id=\"sbkUsm_InternalBuildFailures\"><a href=\"" + Mode.Redirect_URL() + "\">" + build_failures_text + "</a></li>");

                Mode.Internal_Type = Internal_Type_Enum.Cache;
                Output.WriteLine("\t\t\t<li id=\"sbkUsm_InternalCache\"><a href=\"" + Mode.Redirect_URL() + "\">" + memory_mgmt_text + "</a></li>");

                Mode.Internal_Type = Internal_Type_Enum.Wordmarks;
                Output.WriteLine("\t\t\t<li id=\"sbkUsm_InternalWordmarks\"><a href=\"" + Mode.Redirect_URL() + "\">" + wordmarks_text + "</a></li>");

                //// The only time we don't show the standard INTERNAL view selectors is when NEW ITEMS is selected
                //// and there are recenly added NEW ITEMS
                //DataTable New_Items = null;
                //if (type == Internal_Type_Enum.New_Items)
                //    New_Items = SobekCM_Database.Tracking_Update_List(Tracer);

                //// If this user is internal, add that
                //if ((New_Items != null) && (New_Items.Rows.Count > 0))
                //{
                //    currentMode.Mode = Display_Mode_Enum.Internal;
                //    Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\">" + Selected_Tab_Start + internalTab + Selected_Tab_End + "</a>");
                //    currentMode.Mode = Display_Mode_Enum.My_Sobek;
                //}
                //else
                //{
                //    Output.WriteLine(Selected_Tab_Start + internalTab + Selected_Tab_End);
                //}

                Output.WriteLine("\t\t</ul></li>");
            }

            // If this user is a sys admin or portal admin, add that
            if ((User.Is_System_Admin) || (User.Is_Portal_Admin))
            {
                Mode.Mode = Display_Mode_Enum.Administrative;
                Mode.Admin_Type = Admin_Type_Enum.Home;
                Output.WriteLine("\t\t<li id=\"sbkUsm_Admin\"><a href=\"" + Mode.Redirect_URL() + "\">" + sobek_admin_text + "</a><ul id=\"sbkUsm_AdminSubMenu\">");

                // Edit item aggregations
                Mode.Admin_Type = Admin_Type_Enum.Aggregations_Mgmt;
                Output.WriteLine("\t\t\t<li id=\"sbkUsm_AdminAggr\"><a href=\"" + Mode.Redirect_URL() + "\"><img src=\"" + Mode.Default_Images_URL + "building.gif\" /> <div class=\"sbkUsm_TextWithImage\">Item Aggregations</div></a></li>");

                // Edit interfaces
                Mode.Admin_Type = Admin_Type_Enum.Interfaces;
                Output.WriteLine("\t\t\t<li id=\"sbkUsm_AdminSkin\"><a href=\"" + Mode.Redirect_URL() + "\"><img src=\"" + Mode.Default_Images_URL + "skins.png\" /> <div class=\"sbkUsm_TextWithImage\">Web Skins</div></a></li>");

                // Edit wordmarks
                Mode.Admin_Type = Admin_Type_Enum.Wordmarks;
                Output.WriteLine("\t\t\t<li id=\"sbkUsm_AdminWordmarks\"><a href=\"" + Mode.Redirect_URL() + "\"><img src=\"" + Mode.Default_Images_URL + "wordmarks.gif\" /> <div class=\"sbkUsm_TextWithImage\">Wordmarks / Icons</div></a></li>");

                // Edit forwarding
                Mode.Admin_Type = Admin_Type_Enum.Forwarding;
                Output.WriteLine("\t\t\t<li id=\"sbkUsm_AdminForwarding\"><a href=\"" + Mode.Redirect_URL() + "\"><img src=\"" + Mode.Default_Images_URL + "forwarding.png\" /> <div class=\"sbkUsm_TextWithImage\">Aggregation Aliases</div></a></li>");

                // Edit Projects
                Mode.Admin_Type = Admin_Type_Enum.Projects;
                Output.WriteLine("\t\t\t<li id=\"sbkUsm_AdminProjects\"><a href=\"" + Mode.Redirect_URL() + "\"><img src=\"" + Mode.Default_Images_URL + "pmets.gif\" /> <div class=\"sbkUsm_TextWithImage\">Projects</div></a></li>");

                // Edit Thematic Headings
                Mode.Admin_Type = Admin_Type_Enum.Thematic_Headings;
                Output.WriteLine("\t\t\t<li id=\"sbkUsm_AdminThematic\"><a href=\"" + Mode.Redirect_URL() + "\"><div class=\"sbkUsm_TextNoImage\">Thematic Headings</div></a></li>");

                if (User.Is_System_Admin)
                {
                    // Edit users
                    Mode.Admin_Type = Admin_Type_Enum.Users;
                    Output.WriteLine("\t\t\t<li id=\"sbkUsm_AdminUsers\"><a href=\"" + Mode.Redirect_URL() + "\"><img src=\"" + Mode.Default_Images_URL + "users.png\" /> <div class=\"sbkUsm_TextWithImage\">Registered Users and Groups</div></a></li>");

                    // Edit IP Restrictions
                    Mode.Admin_Type = Admin_Type_Enum.IP_Restrictions;
                    Output.WriteLine("\t\t\t<li id=\"sbkUsm_AdminRestrictions\"><a href=\"" + Mode.Redirect_URL() + "\"><img src=\"" + Mode.Default_Images_URL + "firewall.png\" /> <div class=\"sbkUsm_TextWithImage\">IP Restriction Ranges</div></a></li>");

                    // Edit URL Portals
                    Mode.Admin_Type = Admin_Type_Enum.URL_Portals;
                    Output.WriteLine("\t\t\t<li id=\"sbkUsm_AdminPortals\"><a href=\"" + Mode.Redirect_URL() + "\"><img src=\"" + Mode.Default_Images_URL + "portals.png\" /> <div class=\"sbkUsm_TextWithImage\">URL Portals</div></a></li>");

                    // Edit Settings
                    Mode.Admin_Type = Admin_Type_Enum.Settings;
                    Output.WriteLine("\t\t\t<li id=\"sbkUsm_AdminSettings\"><a href=\"" + Mode.Redirect_URL() + "\"><img src=\"" + Mode.Default_Images_URL + "wrench.png\" /> <div class=\"sbkUsm_TextWithImage\">System-Wide Settings</div></a></li>");

                    // View and set SobekCM Builder Status
                    Mode.Admin_Type = Admin_Type_Enum.Builder_Status;
                    Output.WriteLine("\t\t\t<li id=\"sbkUsm_AdminStatus\"><a href=\"" + Mode.Redirect_URL() + "\"><img src=\"" + Mode.Default_Images_URL + "gears.png\" /> <div class=\"sbkUsm_TextWithImage\">SobekCM Builder Status</div></a></li>");

                    // Reset cache
                    Mode.Admin_Type = Admin_Type_Enum.Reset;
                    Output.WriteLine("\t\t\t<li id=\"sbkUsm_AdminReset\"><a href=\"" + Mode.Redirect_URL() + "\"><img src=\"" + Mode.Default_Images_URL + "refresh.png\" /> <div class=\"sbkUsm_TextWithImage\">Reset Cache</div></a></li>");
                }

                Output.WriteLine("\t\t</ul></li>");
            }


            Output.WriteLine("\t</ul></div>");
            Output.WriteLine();

            // Add the scripts needed
            Output.WriteLine("<!-- Add references to the superfish and hoverintent libraries for the main user menu -->");
            Output.WriteLine("<script type=\"text/javascript\" src=\"" + Mode.Base_URL + "default/scripts/superfish/hoverIntent.js\" ></script>");
            Output.WriteLine("<script type=\"text/javascript\" src=\"" + Mode.Base_URL + "default/scripts/superfish/superfish.js\" ></script>");
            Output.WriteLine();

            Output.WriteLine("<!-- Initialize the main user menu -->");
            Output.WriteLine("<script>");
            Output.WriteLine();
            Output.WriteLine("jQuery(document).ready(function () {");
            Output.WriteLine("     jQuery('ul.sf-menu').superfish();");
            Output.WriteLine("  });");
            Output.WriteLine();
            Output.WriteLine("</script>");
            Output.WriteLine();

            // Restore the current view information type
            Mode.Mode = currentMode;
            Mode.Admin_Type = adminType;
            Mode.My_Sobek_Type = mySobekType;
            Mode.Internal_Type = internalType;
            Mode.Result_Display_Type = resultType;
            Mode.My_Sobek_SubMode = mySobekSubmode;
            Mode.Page = page;
        }
    }
}
