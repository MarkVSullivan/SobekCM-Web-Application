#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Web;
using SobekCM.Core.Aggregations;
using SobekCM.Core.Configuration;
using SobekCM.Core.Navigation;
using SobekCM.Core.Search;
using SobekCM.Library.AggregationViewer;
using SobekCM.Library.Settings;
using SobekCM.Library.UI;

#endregion

namespace SobekCM.Library.HTML
{
    /// <summary> Static class is used to write the main menus used for the aggregation 
    /// pages and the top-level pages which require a logon. </summary>
    /// <remarks> This is not used to write the item-level menus.  That is done
    /// directly in the <see cref="Item_HtmlSubwriter"/> class.</remarks>
    public static class MainMenus_Helper_HtmlSubWriter
    {
        /// <summary> Add the aggregation-level main menu </summary>
        /// <param name="Output"> Stream to which to write the HTML for this menu </param>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        public static void Add_Aggregation_Main_Menu(TextWriter Output, RequestCache RequestSpecificValues )
        {
            Output.WriteLine("<!-- Add the main aggregation menu -->");
            Output.WriteLine("<nav id=\"sbkAgm_MenuBar\" class=\"sbkMenu_Bar\" role=\"navigation\" aria-label=\"Aggregation menu\">");
            Output.WriteLine("<h2 class=\"hidden-element\">Aggregation menu</h2>");
            Output.WriteLine("  <ul class=\"sf-menu\" id=\"sbkAgm_Menu\">");

            // Get ready to draw the tabs
            string home = "Home";
            string collection_home = UI_ApplicationCache_Gateway.Translation.Get_Translation(RequestSpecificValues.Hierarchy_Object.ShortName, RequestSpecificValues.Current_Mode.Language ) + " Home";
            string sobek_home_text = RequestSpecificValues.Current_Mode.Instance_Abbreviation + " Home";
            string viewItems = "View Items";
            string allItems = "View All Items";
            string newItems = "View Recently Added Items";
            string myCollections = "My Collections";
            string partners = "Browse Partners";
            string browseBy = "Browse By";
            const string BROWSE_MAP = "Map Browse";
            const string list_view_text = "List View";
            const string brief_view_text = "Brief View";
            const string tree_view_text = "Tree View";
            const string partners_text = "Browse Partners";

            if (RequestSpecificValues.Current_Mode.Language == Web_Language_Enum.Spanish)
            {
                home = "INICIO";
                collection_home = "INICIO " + UI_ApplicationCache_Gateway.Translation.Get_Translation(RequestSpecificValues.Hierarchy_Object.ShortName, RequestSpecificValues.Current_Mode.Language);
                sobek_home_text = "INICIO " + RequestSpecificValues.Current_Mode.Instance_Abbreviation.ToUpper();
                allItems = "TODOS LOS ARTÍCULOS";
                newItems = "NUEVOS ARTÍCULOS";
                browseBy = "BÚSQUEDA POR";
                partners = "AFILIADOS";
                myCollections = "MIS COLECCIONES";
                viewItems = "VER ARTÍCULOS";
            }

            if (RequestSpecificValues.Current_Mode.Language == Web_Language_Enum.French)
            {
                home = "PAGE D'ACCUEIL";
                sobek_home_text = "PAGE D'ACCUEIL";
                allItems = "TOUS LES ARTICLES";
                newItems = "NOUVEAUX ARTICLES";
                browseBy = "PARCOURIR PAR";
                viewItems = "VOIR ARTICLES";
            }

            // Save the current mode and browse
            Display_Mode_Enum thisMode = RequestSpecificValues.Current_Mode.Mode;
            Aggregation_Type_Enum thisAggrType = RequestSpecificValues.Current_Mode.Aggregation_Type;
            Search_Type_Enum thisSearch = RequestSpecificValues.Current_Mode.Search_Type;
            Home_Type_Enum thisHomeType = RequestSpecificValues.Current_Mode.Home_Type;
            Result_Display_Type_Enum resultsType = RequestSpecificValues.Current_Mode.Result_Display_Type;
            ushort? page = RequestSpecificValues.Current_Mode.Page;
            string browse_code = RequestSpecificValues.Current_Mode.Info_Browse_Mode;
            string aggregation = RequestSpecificValues.Current_Mode.Aggregation;
            if ((thisMode == Display_Mode_Enum.Aggregation) && ((thisAggrType == Aggregation_Type_Enum.Browse_Info) || (thisAggrType == Aggregation_Type_Enum.Child_Page_Edit)))
            {
                browse_code = RequestSpecificValues.Current_Mode.Info_Browse_Mode;
            }

            // Get the home search type (just to do a matching in case it was explicitly requested)
            Item_Aggregation_Views_Searches_Enum homeView = Item_Aggregation_Views_Searches_Enum.Basic_Search;
            if (RequestSpecificValues.Hierarchy_Object.Views_And_Searches.Count > 0)
            {
                homeView = RequestSpecificValues.Hierarchy_Object.Views_And_Searches[0];
            }

            // Remove any search string
            string current_search = RequestSpecificValues.Current_Mode.Search_String;
            RequestSpecificValues.Current_Mode.Search_String = String.Empty;

            // Add any PRE-MENU instance options
            bool pre_menu_options_exist = false;
            string first_pre_menu_option = String.Empty;
            string second_pre_menu_option = String.Empty;
            if (UI_ApplicationCache_Gateway.Settings.Additional_Settings.ContainsKey("Aggregation Viewer.Static First Menu Item"))
                first_pre_menu_option = UI_ApplicationCache_Gateway.Settings.Additional_Settings["Aggregation Viewer.Static First Menu Item"];
            if (UI_ApplicationCache_Gateway.Settings.Additional_Settings.ContainsKey("Aggregation Viewer.Static Second Menu Item"))
                second_pre_menu_option = UI_ApplicationCache_Gateway.Settings.Additional_Settings["Aggregation Viewer.Static Second Menu Item"];
            if ((first_pre_menu_option.Length > 0) || (second_pre_menu_option.Length > 0))
            {
                pre_menu_options_exist = true;
                if (first_pre_menu_option.Length > 0)
                {
                    string[] first_splitter = first_pre_menu_option.Replace("[", "").Replace("]", "").Split(";".ToCharArray());
                    if (first_splitter.Length > 0)
                    {
                        Output.WriteLine("    <li><a href=\"" + first_splitter[1] + "\" title=\"" + HttpUtility.HtmlEncode(first_splitter[0]) + "\">" + HttpUtility.HtmlEncode(first_splitter[0]) + "</a></li>");
                    }
                }
                if (second_pre_menu_option.Length > 0)
                {
                    string[] second_splitter = second_pre_menu_option.Replace("[", "").Replace("]", "").Split(";".ToCharArray());
                    if (second_splitter.Length > 0)
                    {
                        Output.WriteLine("    <li><a href=\"" + second_splitter[1] + "\" title=\"" + HttpUtility.HtmlEncode(second_splitter[0]) + "\">" + HttpUtility.HtmlEncode(second_splitter[0]) + "</a></li>");
                    }
                }
            }

            bool isOnHome = (((RequestSpecificValues.Current_Mode.Mode == Display_Mode_Enum.Aggregation) && (RequestSpecificValues.Current_Mode.Aggregation_Type == Aggregation_Type_Enum.Home)) ||
                 ((RequestSpecificValues.Current_Mode.Mode == Display_Mode_Enum.Search) &&
                  (Aggregation_Nav_Bar_HTML_Factory.Do_Search_Types_Match(homeView, RequestSpecificValues.Current_Mode.Search_Type))));

            // Add the HOME tab
            if ((RequestSpecificValues.Hierarchy_Object.Code == "all") || ( RequestSpecificValues.Hierarchy_Object.Code == RequestSpecificValues.Current_Mode.Default_Aggregation))
            {
                // Add the 'SOBEK HOME' first menu option and suboptions
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Aggregation;
                RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Home;
                RequestSpecificValues.Current_Mode.Home_Type = Home_Type_Enum.List;

                if (RequestSpecificValues.Hierarchy_Object.Code == "all")
                {
                    // What is considered HOME changes here at the top level
                    if ((thisHomeType == Home_Type_Enum.Partners_List) || (thisHomeType == Home_Type_Enum.Partners_Thumbnails) || (thisHomeType == Home_Type_Enum.Personalized))
                        isOnHome = false;

                    // If some instance-wide pre-menu items existed, don't use the home image
                    if (pre_menu_options_exist)
                    {
                        if (isOnHome)
                            Output.Write("    <li id=\"selected-sf-menu-item-link\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + sobek_home_text + "</a>");
                        else
                            Output.Write("    <li><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + sobek_home_text + "</a>");
                    }
                    else
                    {
                        if ( isOnHome )
                            Output.Write("    <li id=\"sbkAgm_Home\" class=\"sbkMenu_Home selected-sf-menu-item-link\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\" class=\"sbkMenu_NoPadding\"><img src=\"" + Static_Resources.Home_Png + "\" alt=\"Home\" /> <div class=\"sbkMenu_HomeText\">" + sobek_home_text + "</div></a>");
                        else
                            Output.Write("    <li id=\"sbkAgm_Home\" class=\"sbkMenu_Home\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\" class=\"sbkMenu_NoPadding\"><img src=\"" + Static_Resources.Home_Png + "\" alt=\"Home\" /> <div class=\"sbkMenu_HomeText\">" + sobek_home_text + "</div></a>");
                    }

                    if (( UI_ApplicationCache_Gateway.Thematic_Headings != null ) && ( UI_ApplicationCache_Gateway.Thematic_Headings.Count > 0 ))
                    {
                        Output.WriteLine("<ul id=\"sbkAgm_HomeSubMenu\">");
                        Output.WriteLine("      <li id=\"sbkAgm_HomeListView\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + list_view_text + "</a></li>");
                        RequestSpecificValues.Current_Mode.Home_Type = Home_Type_Enum.Descriptions;
                        Output.WriteLine("      <li id=\"sbkAgm_HomeBriefView\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + brief_view_text + "</a></li>");
                        if (UI_ApplicationCache_Gateway.Settings.System.Include_TreeView_On_System_Home)
                        {
                            RequestSpecificValues.Current_Mode.Home_Type = Home_Type_Enum.Tree;
                            Output.WriteLine("      <li id=\"sbkAgm_HomeTreeView\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + tree_view_text + "</a></li>");
                        }
                        Output.WriteLine("    </ul></li>");
                    }
                    else
                    {
                        Output.WriteLine("</li>");
                    }
                }
                else
                {
                    Output.WriteLine("    <li id=\"sbkAgm_Home\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\" class=\"sbkAgm_NoPadding\"><img src=\"" + Static_Resources.Home_Png + "\" alt=\"\" /> <div class=\"sbkAgm_HomeText\">" + sobek_home_text + "</div></a></li>");
                }
            }
            else
            {

                // Add the 'SOBEK HOME' first menu option and suboptions
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Aggregation;
                RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Home;
                RequestSpecificValues.Current_Mode.Home_Type = Home_Type_Enum.List;

                // If some instance-wide pre-menu items existed, don't use the home image
                if (pre_menu_options_exist)
                {
                    if (isOnHome)
                    {
                        Output.WriteLine("    <li id=\"sbkAgm_Home\" class=\"selected-sf-menu-item-link\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + home + "</a><ul id=\"sbkAgm_HomeSubMenu\">");
                    }
                    else
                    {
                        Output.WriteLine("    <li id=\"sbkAgm_Home\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + home + "</a><ul id=\"sbkAgm_HomeSubMenu\">");
                    }
                }
                else
                {
                    if (isOnHome)
                    {
                        Output.WriteLine("    <li id=\"sbkAgm_Home\" class=\"selected-sf-menu-item-link sbkMenu_Home\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\" class=\"sbkMenu_NoPadding\"><img src=\"" + Static_Resources.Home_Png + "\" alt=\"Home\" /> <div class=\"sbkMenu_HomeText\">" + home + "</div></a><ul id=\"sbkAgm_HomeSubMenu\">");
                    }
                    else
                    {
                        Output.WriteLine("    <li id=\"sbkAgm_Home\" class=\"sbkMenu_Home\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\" class=\"sbkMenu_NoPadding\"><img src=\"" + Static_Resources.Home_Png + "\" alt=\"Home\" /> <div class=\"sbkMenu_HomeText\">" + home + "</div></a><ul id=\"sbkAgm_HomeSubMenu\">");
                    }
                }

                Output.WriteLine("      <li id=\"sbkAgm_AggrHome\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + collection_home + "</a></li>");

                RequestSpecificValues.Current_Mode.Aggregation = String.Empty;
                if (RequestSpecificValues.Current_Mode.Default_Aggregation != "all")
                {
                    Output.WriteLine("      <li id=\"sbkAgm_InstanceHome\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + sobek_home_text + "</a></li>");
                }
                else
                {
                    Output.Write("      <li id=\"sbkAgm_InstanceHome\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + sobek_home_text + "</a>");
                    Output.WriteLine("<ul id=\"sbkAgm_InstanceHomeSubMenu\">");

                    if ((UI_ApplicationCache_Gateway.Thematic_Headings != null) && (UI_ApplicationCache_Gateway.Thematic_Headings.Count > 0))
                    {
                        Output.WriteLine("        <li id=\"sbkAgm_HomeListView\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + list_view_text + "</a></li>");
                        RequestSpecificValues.Current_Mode.Home_Type = Home_Type_Enum.Descriptions;
                        Output.WriteLine("        <li id=\"sbkAgm_HomeBriefView\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + brief_view_text + "</a></li>");
                        if (UI_ApplicationCache_Gateway.Settings.System.Include_TreeView_On_System_Home)
                        {
                            RequestSpecificValues.Current_Mode.Home_Type = Home_Type_Enum.Tree;
                            Output.WriteLine("        <li id=\"sbkAgm_HomeTreeView\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + tree_view_text + "</a></li>");
                        }
                    }
                    if ((RequestSpecificValues.Current_User != null) && (RequestSpecificValues.Current_User.LoggedOn))
                    {
                        RequestSpecificValues.Current_Mode.Home_Type = Home_Type_Enum.Personalized;
                        Output.WriteLine("        <li id=\"sbkAgm_HomePersonalized\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + myCollections + "</a></li>");
                    }
                    if (UI_ApplicationCache_Gateway.Settings.System.Include_Partners_On_System_Home)
                    {
                        RequestSpecificValues.Current_Mode.Home_Type = Home_Type_Enum.Partners_List;
                        Output.WriteLine("        <li id=\"sbkAgm_HomePartners\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + partners_text + "</a></li>");
                    }
                    Output.WriteLine("      </ul></li>");
                }

                Output.WriteLine("    </ul></li>");

                RequestSpecificValues.Current_Mode.Aggregation = RequestSpecificValues.Hierarchy_Object.Code;
            }

            // Add any additional search types
            RequestSpecificValues.Current_Mode.Mode = thisMode;
            for (int i = 1; i < RequestSpecificValues.Hierarchy_Object.Views_And_Searches.Count; i++)
            {
                Output.Write("    " + Aggregation_Nav_Bar_HTML_Factory.Menu_Get_Nav_Bar_HTML(RequestSpecificValues.Hierarchy_Object.Views_And_Searches[i], RequestSpecificValues.Current_Mode, UI_ApplicationCache_Gateway.Translation));
            }

            // Replace any search string
            RequestSpecificValues.Current_Mode.Search_String = current_search;

            // Check for the existence of any BROWSE BY pages
            if (RequestSpecificValues.Hierarchy_Object.Has_Browse_By_Pages)
            {
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Aggregation;
                RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Browse_By;
                RequestSpecificValues.Current_Mode.Info_Browse_Mode = String.Empty;

                // Get sorted collection of (public) browse bys linked to this aggregation
                ReadOnlyCollection<Item_Aggregation_Child_Page> public_browses = RequestSpecificValues.Hierarchy_Object.Browse_By_Pages;
                if (public_browses.Count > 0)
                {
                    if (((thisMode == Display_Mode_Enum.Aggregation) && (thisAggrType == Aggregation_Type_Enum.Browse_By)) || (RequestSpecificValues.Current_Mode.Is_Robot))
                    {
                        Output.WriteLine("    <li id=\"sbkAgm_BrowseBy\" class=\"selected-sf-menu-item-link\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + browseBy + "</a><ul id=\"sbkAgm_BrowseBySubMenu\">");
                    }
                    else
                    {
                        Output.WriteLine("    <li id=\"sbkAgm_BrowseBy\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + browseBy + "</a><ul id=\"sbkAgm_BrowseBySubMenu\">");
                    }

                    foreach (Item_Aggregation_Child_Page thisBrowse in public_browses)
                    {
                        // Static HTML or metadata browse by?
                        if (thisBrowse.Source_Data_Type == Item_Aggregation_Child_Source_Data_Enum.Static_HTML)
                        {
                            RequestSpecificValues.Current_Mode.Info_Browse_Mode = thisBrowse.Code;
                            Output.WriteLine("      <li><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode).Replace("&", "&amp") + "\">" + thisBrowse.Label + "</a></li>");
                        }
                        else
                        {
                            Metadata_Search_Field facetField = UI_ApplicationCache_Gateway.Settings.Metadata_Search_Field_By_Display_Name(thisBrowse.Code);
                            if (facetField != null)
                            {
                                RequestSpecificValues.Current_Mode.Info_Browse_Mode = thisBrowse.Code.ToLower().Replace(" ", "_");
                                Output.WriteLine("      <li><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode).Replace("&", "&amp") + "\">" + facetField.Display_Term + "</a></li>");
                            }
                        }
                    }

                    Output.WriteLine("    </ul></li>");
                }
            }

            // Check for the existence of any MAP BROWSE pages
            if (RequestSpecificValues.Hierarchy_Object.Views_And_Searches.Contains(Item_Aggregation_Views_Searches_Enum.Map_Browse))
            {
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Aggregation;
                RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Browse_Map;
                RequestSpecificValues.Current_Mode.Info_Browse_Mode = String.Empty;

                if ((thisMode == Display_Mode_Enum.Aggregation) && (thisAggrType == Aggregation_Type_Enum.Browse_Map))
                {
                    Output.WriteLine("    <li id=\"sbkAgm_MapBrowse\" class=\"selected-sf-menu-item-link\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + BROWSE_MAP + "</a></li>");
                }
                else
                {
                    Output.WriteLine("    <li id=\"sbkAgm_MapBrowse\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + BROWSE_MAP + "</a></li>");
                }
            }

            // Add all the browses and child pages
            RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Aggregation;
            RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Browse_Info;

            // Find the URL for all these browses
            RequestSpecificValues.Current_Mode.Info_Browse_Mode = "XYXYXYXYXY";
            string redirect_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
            RequestSpecificValues.Current_Mode.Page = 1;
           // RequestSpecificValues.Current_Mode.Mode = thisMode;

            // Only show ALL and NEW if they are in the collection list of searches and views
            int included_browses = 0;
            if (RequestSpecificValues.Hierarchy_Object.Views_And_Searches.Contains(Item_Aggregation_Views_Searches_Enum.All_New_Items))
            {
                // First, look for 'ALL'
                if (RequestSpecificValues.Hierarchy_Object.Contains_Browse_Info("all"))
                {
                    bool includeNew = ((RequestSpecificValues.Hierarchy_Object.Contains_Browse_Info("new")) && (!RequestSpecificValues.Current_Mode.Is_Robot));
                    if (includeNew)
                    {
                        if (( thisMode == Display_Mode_Enum.Aggregation ) && ((browse_code == "all") || (browse_code == "new" )))
                        {
                            Output.WriteLine("    <li id=\"sbkAgm_ViewItems\" class=\"selected-sf-menu-item-link\"><a href=\"" + redirect_url.Replace("XYXYXYXYXY", "all").Replace("/info/", "/") + "\">" + viewItems + "</a><ul id=\"sbkAgm_ViewItemsSubMenu\">");
                        }
                        else
                        {
                            Output.WriteLine("    <li id=\"sbkAgm_ViewItems\"><a href=\"" + redirect_url.Replace("XYXYXYXYXY", "all").Replace("/info/", "/") + "\">" + viewItems + "</a><ul id=\"sbkAgm_ViewItemsSubMenu\">");
                        }

                        Output.WriteLine("    <li id=\"sbkAgm_AllBrowse\"><a href=\"" + redirect_url.Replace("XYXYXYXYXY", "all").Replace("/info/", "/") + "\">" + allItems + "</a></li>");
                        Output.WriteLine("    <li id=\"sbkAgm_NewBrowse\"><a href=\"" + redirect_url.Replace("XYXYXYXYXY", "new").Replace("/info/", "/") + "\">" + newItems + "</a></li>");

                        Output.WriteLine("    </ul></li>");
                    }
                    else
                    {
                        if (( thisMode == Display_Mode_Enum.Aggregation ) && (browse_code == "all"))
                        {
                            Output.WriteLine("    <li id=\"sbkAgm_ViewItems\" class=\"selected-sf-menu-item-link\"><a href=\"" + redirect_url.Replace("XYXYXYXYXY", "all").Replace("/info/", "/") + "\">" + viewItems + "</a></li>");
                        }
                        else
                        {
                            Output.WriteLine("    <li id=\"sbkAgm_ViewItems\"><a href=\"" + redirect_url.Replace("XYXYXYXYXY", "all").Replace("/info/", "/") + "\">" + viewItems + "</a></li>");
                        }
                    }

                    included_browses++;
                }
            }

            RequestSpecificValues.Current_Mode.Result_Display_Type = Result_Display_Type_Enum.NONE;
            redirect_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);

            // Are there any additional browses to include?
            ReadOnlyCollection<Item_Aggregation_Child_Page> otherBrowses = RequestSpecificValues.Hierarchy_Object.Browse_Home_Pages;
            if (otherBrowses.Count > included_browses)
            {
                // Determine the hierarchy
                List<Item_Aggregation_Child_Page> menuPages = new List<Item_Aggregation_Child_Page>();
                List<Item_Aggregation_Child_Page> topPages = new List<Item_Aggregation_Child_Page>();
                Dictionary<string, Item_Aggregation_Child_Page> pagesDictionary = new Dictionary<string, Item_Aggregation_Child_Page>();
                Dictionary<Item_Aggregation_Child_Page, List<Item_Aggregation_Child_Page>> parentToChild = new Dictionary<Item_Aggregation_Child_Page, List<Item_Aggregation_Child_Page>>();


                // Now, step through the sorted list
                foreach (Item_Aggregation_Child_Page thisBrowseObj in otherBrowses.Where(ThisBrowseObj => (ThisBrowseObj.Code != "all") && (ThisBrowseObj.Code != "new")))
                {
                    // Add to the list, to avoid re-iterating through this
                    menuPages.Add(thisBrowseObj);

                    // Add this child page to the dictionary
                    pagesDictionary.Add(thisBrowseObj.Code.ToLower(), thisBrowseObj);

                    // Is this a  top-level page?
                    if (String.IsNullOrEmpty(thisBrowseObj.Parent_Code))
                        topPages.Add(thisBrowseObj);
                }

                // Now, build the hierarchy
                foreach (Item_Aggregation_Child_Page thisPage in menuPages)
                {
                    if (!String.IsNullOrEmpty(thisPage.Parent_Code))
                    {
                        if (pagesDictionary.ContainsKey(thisPage.Parent_Code.ToLower()))
                        {
                            Item_Aggregation_Child_Page parentPage = pagesDictionary[thisPage.Parent_Code.ToLower()];
                            if (!parentToChild.ContainsKey(parentPage))
                                parentToChild[parentPage] = new List<Item_Aggregation_Child_Page>();
                            parentToChild[parentPage].Add(thisPage);
                        }
                    }
                }

                // Now, add each top-level page, with children
                foreach (Item_Aggregation_Child_Page topPage in topPages)
                {
                    RequestSpecificValues.Current_Mode.Info_Browse_Mode = topPage.Code;
                    bool selected = false;
                    if (browse_code == topPage.Code)
                        selected = true;
                    else
                    {
                        if ((parentToChild.ContainsKey(topPage)) && (parentToChild[topPage].Count > 0))
                        {
                            foreach (Item_Aggregation_Child_Page middlePages in parentToChild[topPage])
                            {
                                if (browse_code == middlePages.Code)
                                {
                                    selected = true;
                                }
                            }
                        }
                    }

                    if (selected)
                    {
                        Output.Write("    <li id=\"sbkAgm_" + topPage.Code.Replace(" ", "") + "Browse\" class=\"selected-sf-menu-item-link\"><a href=\"" + redirect_url.Replace("XYXYXYXYXY", topPage.Code) + "\">" + topPage.Label + "</a>");
                    }
                    else
                    { 

                        Output.Write("    <li id=\"sbkAgm_" + topPage.Code.Replace(" ", "") + "Browse\"><a href=\"" + redirect_url.Replace("XYXYXYXYXY", topPage.Code) + "\">" + topPage.Label + "</a>");
                    }

                    if ((parentToChild.ContainsKey(topPage)) && ( parentToChild[topPage].Count > 0 ))
                    {
                        Output.WriteLine("<ul id=\"sbkAgm_" + topPage.Code.Replace(" ", "") + " + SubMenu\">");
                        foreach (Item_Aggregation_Child_Page middlePages in parentToChild[topPage])
                        {
                            RequestSpecificValues.Current_Mode.Info_Browse_Mode = middlePages.Code;
                            if (browse_code == middlePages.Code)
                            {
                                Output.Write("    <li id=\"sbkAgm_" + middlePages.Code.Replace(" ", "") + "Browse\" class=\"selected-sf-menu-item-link submenu-item-selected\"><a href=\"" + redirect_url.Replace("XYXYXYXYXY", middlePages.Code) + "\">" + middlePages.Label + "</a>");
                            }
                            else
                            {
                                Output.Write("    <li id=\"sbkAgm_" + middlePages.Code.Replace(" ", "") + "Browse\"><a href=\"" + redirect_url.Replace("XYXYXYXYXY", middlePages.Code) + "\">" + middlePages.Label + "</a>");
                            }

                            if ((parentToChild.ContainsKey(middlePages)) && (parentToChild[middlePages].Count > 0))
                            {
                                Output.WriteLine("<ul id=\"sbkAgm_" + middlePages.Code.Replace(" ", "") + " + SubMenu\">");
                                foreach (Item_Aggregation_Child_Page bottomPages in parentToChild[middlePages])
                                {
                                    RequestSpecificValues.Current_Mode.Info_Browse_Mode = bottomPages.Code;
                                    if (browse_code == bottomPages.Code)
                                    {
                                        Output.Write("    <li id=\"sbkAgm_" + bottomPages.Code.Replace(" ", "") + "Browse\" class=\"selected-sf-menu-item-link submenu-item-selected\"><a href=\"" + redirect_url.Replace("XYXYXYXYXY", bottomPages.Code) + "\">" + bottomPages.Label + "</a></li>");
                                    }
                                    else
                                    {
                                        Output.Write("    <li id=\"sbkAgm_" + bottomPages.Code.Replace(" ", "") + "Browse\"><a href=\"" + redirect_url.Replace("XYXYXYXYXY", bottomPages.Code) + "\">" + bottomPages.Label + "</a></li>");
                                    }
                                }
                                Output.Write("    </ul>");
                            }
                            Output.WriteLine("</li>");
                        }
                        Output.Write("    </ul>");
                    }

                    Output.WriteLine("</li>");
                }
            }


            // If this is NOT the all collection, then show subcollections
            if ((RequestSpecificValues.Hierarchy_Object.Code != "all") && (RequestSpecificValues.Hierarchy_Object.Children_Count > 0))
            {
                // Verify some of the children are active and not hidden
                // Keep the last aggregation alias
                string lastAlias = RequestSpecificValues.Current_Mode.Aggregation_Alias;
                RequestSpecificValues.Current_Mode.Aggregation_Alias = String.Empty;
                RequestSpecificValues.Current_Mode.Info_Browse_Mode = String.Empty;

                // Collect the html to write (this alphabetizes the children)
                List<string> html_list = new List<string>();
                foreach (Item_Aggregation_Related_Aggregations childAggr in RequestSpecificValues.Hierarchy_Object.Children)
                {
                    Item_Aggregation_Related_Aggregations latest = UI_ApplicationCache_Gateway.Aggregations[childAggr.Code];
                    if ((latest != null) && (!latest.Hidden) && (latest.Active))
                    {
                        string name = childAggr.ShortName;
                        if (name.ToUpper() == "ADDED AUTOMATICALLY")
                            name = childAggr.Code + " ( Added Automatically )";

                        RequestSpecificValues.Current_Mode.Aggregation = childAggr.Code.ToLower();
                        html_list.Add("      <li><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + UI_ApplicationCache_Gateway.Translation.Get_Translation(name, RequestSpecificValues.Current_Mode.Language) + "</a></li>");
                    }
                }

                if (html_list.Count > 0)
                {
                    string childTypes = RequestSpecificValues.Hierarchy_Object.Child_Types.Trim();
                    Output.WriteLine("    <li id=\"sbkAgm_SubCollections\"><a href=\"#subcolls\">" + UI_ApplicationCache_Gateway.Translation.Get_Translation(childTypes, RequestSpecificValues.Current_Mode.Language) + "</a><ul id=\"sbkAgm_SubCollectionsMenu\">");
                    foreach (string thisHtml in html_list)
                    {
                        Output.WriteLine(thisHtml);
                    }
                    Output.WriteLine("    </ul></li>");

                    // Restore the old alias
                    RequestSpecificValues.Current_Mode.Aggregation_Alias = lastAlias;
                }
            }

            // If there is a user and this is the main home page, show MY COLLECTIONS
            if ((RequestSpecificValues.Current_User != null) && (RequestSpecificValues.Current_User.LoggedOn))
            {
                if (RequestSpecificValues.Hierarchy_Object.Code == "all")
                {
                    RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Aggregation;
                    RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Home;
                    RequestSpecificValues.Current_Mode.Home_Type = Home_Type_Enum.Personalized;

                    // Show personalized
                    if (thisHomeType == Home_Type_Enum.Personalized)
                    {
                        Output.WriteLine("    <li id=\"sbkAgm_MyCollections\" class=\"selected-sf-menu-item-link\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + myCollections + "</a></li>");
                    }
                    else
                    {
                        Output.WriteLine("    <li id=\"sbkAgm_MyCollections\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + myCollections + "</a></li>");
                    }
                }
                else
                {
                    if (RequestSpecificValues.Current_User.Is_Aggregation_Admin(RequestSpecificValues.Hierarchy_Object.Code))
                    {
                        // Return the code and mode back
                        RequestSpecificValues.Current_Mode.Info_Browse_Mode = String.Empty;
                        RequestSpecificValues.Current_Mode.Search_Type = thisSearch;
                        RequestSpecificValues.Current_Mode.Mode = thisMode;
                        RequestSpecificValues.Current_Mode.Home_Type = thisHomeType;

                        Output.Write(Aggregation_Nav_Bar_HTML_Factory.Menu_Get_Nav_Bar_HTML(Item_Aggregation_Views_Searches_Enum.Admin_View, RequestSpecificValues.Current_Mode, UI_ApplicationCache_Gateway.Translation));
                    }
                }
            }

            // Show institutional lists?
            if (RequestSpecificValues.Hierarchy_Object.Code == "all")
            {
                // Is this library set to show the partners tab?
                if (UI_ApplicationCache_Gateway.Settings.System.Include_Partners_On_System_Home)
                {
                    RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Aggregation;
                    RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Home;
                    RequestSpecificValues.Current_Mode.Home_Type = Home_Type_Enum.Partners_List;

                    if (((thisHomeType == Home_Type_Enum.Partners_List) || (thisHomeType == Home_Type_Enum.Partners_Thumbnails)))
                    {
                        Output.WriteLine("    <li id=\"sbkAgm_Partners\" class=\"selected-sf-menu-item-link\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + partners + "</a></li>");
                    }
                    else
                    {
                        Output.WriteLine("    <li id=\"sbkAgm_Partners\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + partners + "</a></li>");
                    }
                }
            }

            // Return the code and mode back
            RequestSpecificValues.Current_Mode.Info_Browse_Mode = browse_code;
            RequestSpecificValues.Current_Mode.Aggregation_Type = thisAggrType;
            RequestSpecificValues.Current_Mode.Search_Type = thisSearch;
            RequestSpecificValues.Current_Mode.Mode = thisMode;
            RequestSpecificValues.Current_Mode.Home_Type = thisHomeType;
            RequestSpecificValues.Current_Mode.Result_Display_Type = resultsType;
            RequestSpecificValues.Current_Mode.Aggregation = aggregation;
            RequestSpecificValues.Current_Mode.Page = page;

            Output.WriteLine("  </ul>");
			Output.WriteLine("</nav>");
            Output.WriteLine();

            Output.WriteLine("<!-- Initialize the main user menu -->");
            Output.WriteLine("<script>");
            Output.WriteLine("  jQuery(document).ready(function () {");
            Output.WriteLine("     jQuery('ul.sf-menu').superfish({");

            Output.WriteLine("          onBeforeShow: function() { ");
            Output.WriteLine("               if ( $(this).attr('id') == 'sbkAgm_SubCollectionsMenu')");
            Output.WriteLine("               {");
            Output.WriteLine("                 var thisWidth = $(this).width();");
            Output.WriteLine("                 var parent = $('#sbkAgm_SubCollections');");
            Output.WriteLine("                 var offset = $('#sbkAgm_SubCollections').offset();");
            Output.WriteLine("                 if ( $(window).width() < offset.left + thisWidth )");
            Output.WriteLine("                 {");
            Output.WriteLine("                   var newleft = thisWidth - parent.width();");
            Output.WriteLine("                   $(this).css('left', '-' + newleft + 'px');");
            Output.WriteLine("                 }");
            Output.WriteLine("               }");
            Output.WriteLine("          }");
            
            Output.WriteLine("    });");
            Output.WriteLine("  });");
            Output.WriteLine("</script>");
            Output.WriteLine();
        }


        /// <summary> Add the search results (and item browses) main menu </summary>
        /// <param name="Output"> Stream to which to write the HTML for this menu </param>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        /// <param name="Include_Bookshelf_View"> Flag indicates if the bookshelf view should be included in the list of possible views </param>
        public static void Add_Aggregation_Search_Results_Menu(TextWriter Output, RequestCache RequestSpecificValues, bool Include_Bookshelf_View )
        {
            Output.WriteLine("<!-- Add the main search results menu -->");
            Output.WriteLine("<nav id=\"sbkAgm_MenuBar\" class=\"sbkMenu_Bar\" role=\"navigation\" aria-label=\"Search results menu\">");
            Output.WriteLine("<h2 class=\"hidden-element\">Search results menu</h2>");

            // Get ready to draw the tabs
            string home = "Home";
            string collection_home = UI_ApplicationCache_Gateway.Translation.Get_Translation(RequestSpecificValues.Hierarchy_Object.ShortName, RequestSpecificValues.Current_Mode.Language) + " Home";
            string sobek_home_text = RequestSpecificValues.Current_Mode.Instance_Abbreviation + " Home";
            string myCollections = "My Collections";
            string otherSearches_text = "Search Options";
            const string list_view_text = "List View";
            const string brief_view_text = "Brief View";
            const string tree_view_text = "Tree View";
            const string partners_text = "Browse Partners";
            string bookshelf_view = "Bookshelf View";
            string brief_view = "Brief View";
            string map_view = "Map View";
            string table_view = "Table View";
            string thumbnail_view = "Thumbnail View";
            const string EXPORT_VIEW = "Export";


            if (RequestSpecificValues.Current_Mode.Language == Web_Language_Enum.Spanish)
            {
                home = "INICIO";
                collection_home = "INICIO " + UI_ApplicationCache_Gateway.Translation.Get_Translation(RequestSpecificValues.Hierarchy_Object.ShortName, RequestSpecificValues.Current_Mode.Language);
                sobek_home_text = "INICIO " + RequestSpecificValues.Current_Mode.Instance_Abbreviation.ToUpper();
                myCollections = "MIS COLECCIONES";
                bookshelf_view = "VISTA BIBLIOTECA";
                map_view = "VISTA MAPA";
                brief_view = "VISTA BREVE";
                table_view = "VISTA TABLERA";
                thumbnail_view = "VISTA MINIATURA";
            }

            if (RequestSpecificValues.Current_Mode.Language == Web_Language_Enum.French)
            {
                home = "PAGE D'ACCUEIL";
                sobek_home_text = "PAGE D'ACCUEIL";
                bookshelf_view = "MODE MA BIBLIOTHEQUE";
                map_view = "MODE CARTE";
                brief_view = "MODE SIMPLE";
                table_view = "MODE DE TABLE";
                thumbnail_view = "MODE IMAGETTE";
            }

            // Add the sharing buttons if this is not restricted by IP address or checked out
            if (!RequestSpecificValues.Current_Mode.Is_Robot)
            {
                Output.WriteLine("  <div id=\"menu-right-actions\">");

                string save_text = "Save";
                string send_text = "Send";
                string print_text = "Print";

                Output.WriteLine("    <span id=\"printbutton\" class=\"action-sf-menu-item\" onclick=\"window.print();return false;\"><img src=\"" + Static_Resources.Printer_Png + "\" alt=\"\" style=\"vertical-align:middle\" /><span id=\"printbuttonspan\">" + print_text + "</span></span>");

                if (RequestSpecificValues.Current_Mode.Mode != Display_Mode_Enum.My_Sobek)
                {
                    if ((RequestSpecificValues.Current_User != null) && (RequestSpecificValues.Current_User.LoggedOn))
                    {
                        Output.WriteLine("    <span id=\"sendbutton\" class=\"action-sf-menu-item\" onclick=\"email_form_open();\"><img src=\"" + Static_Resources.Email_Png + "\" alt=\"\" style=\"vertical-align:middle\" /><span id=\"sendbuttonspan\">" + send_text + "</span></span>");

                        Output.WriteLine("    <span id=\"savebutton\" class=\"action-sf-menu-item\" onclick=\"save_search_form_open();\"><img src=\"" + Static_Resources.Plussign_Png + "\" alt=\"\" style=\"vertical-align:middle\" /><span id=\"addbuttonspan\">" + save_text + "</span></span>");
                    }
                    else
                    {
                        string returnUrl = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
                        Display_Mode_Enum currMode = RequestSpecificValues.Current_Mode.Mode;
                        RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.My_Sobek;
                        RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Logon;
                        RequestSpecificValues.Current_Mode.Return_URL = returnUrl;
                        string logOnUrl = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);
                        RequestSpecificValues.Current_Mode.Mode = currMode;
                        RequestSpecificValues.Current_Mode.Return_URL = String.Empty;


                        Output.WriteLine("    <span id=\"sendbutton\" class=\"action-sf-menu-item\" onclick=\"window.location='" + logOnUrl + "';\"><img src=\"" + Static_Resources.Email_Png + "\" alt=\"\" style=\"vertical-align:middle\" /><span id=\"sendbuttonspan\">" + send_text + "</span></span>");

                        Output.WriteLine("    <span id=\"savebutton\" class=\"action-sf-menu-item\" onclick=\"window.location='" + logOnUrl + "';\"><img src=\"" + Static_Resources.Plussign_Png + "\" alt=\"\" style=\"vertical-align:middle\" /><span id=\"addbuttonspan\">" + save_text + "</span></span>");
                    }

                    Output.WriteLine("    <span id=\"sharebutton\" class=\"action-sf-menu-item\" onclick=\"return toggle_share_form2('share_button');\"><span id=\"sharebuttonspan\">Share</span></span>");
                }
                Output.WriteLine("  </div>");
                Output.WriteLine();
            }

            // Save the current mode and browse
            Display_Mode_Enum thisMode = RequestSpecificValues.Current_Mode.Mode;
            Aggregation_Type_Enum thisAggrType = RequestSpecificValues.Current_Mode.Aggregation_Type;
            Search_Type_Enum thisSearch = RequestSpecificValues.Current_Mode.Search_Type;
            Home_Type_Enum thisHomeType = RequestSpecificValues.Current_Mode.Home_Type;
            Result_Display_Type_Enum resultsType = RequestSpecificValues.Current_Mode.Result_Display_Type;
            ushort? page = RequestSpecificValues.Current_Mode.Page;
            string browse_code = RequestSpecificValues.Current_Mode.Info_Browse_Mode;
            string aggregation = RequestSpecificValues.Current_Mode.Aggregation;
            if ((thisMode == Display_Mode_Enum.Aggregation) && ((thisAggrType == Aggregation_Type_Enum.Browse_Info) || (thisAggrType == Aggregation_Type_Enum.Child_Page_Edit)))
            {
                browse_code = RequestSpecificValues.Current_Mode.Info_Browse_Mode;
            }

            // Remove any search string
            string current_search = RequestSpecificValues.Current_Mode.Search_String;
            RequestSpecificValues.Current_Mode.Search_String = String.Empty;

            Output.WriteLine("  <ul class=\"sf-menu\" id=\"sbkAgm_Menu\">");

            // Add any PRE-MENU instance options
            bool pre_menu_options_exist = false;
            string first_pre_menu_option = String.Empty;
            string second_pre_menu_option = String.Empty;
            if (UI_ApplicationCache_Gateway.Settings.Additional_Settings.ContainsKey("Aggregation Viewer.Static First Menu Item"))
                first_pre_menu_option = UI_ApplicationCache_Gateway.Settings.Additional_Settings["Aggregation Viewer.Static First Menu Item"];
            if (UI_ApplicationCache_Gateway.Settings.Additional_Settings.ContainsKey("Aggregation Viewer.Static Second Menu Item"))
                second_pre_menu_option = UI_ApplicationCache_Gateway.Settings.Additional_Settings["Aggregation Viewer.Static Second Menu Item"];
            if ((first_pre_menu_option.Length > 0) || (second_pre_menu_option.Length > 0))
            {
                pre_menu_options_exist = true;
                if (first_pre_menu_option.Length > 0)
                {
                    string[] first_splitter = first_pre_menu_option.Replace("[", "").Replace("]", "").Split(";".ToCharArray());
                    if (first_splitter.Length > 0)
                    {
                        Output.WriteLine("    <li><a href=\"" + first_splitter[1] + "\" title=\"" + HttpUtility.HtmlEncode(first_splitter[0]) + "\">" + HttpUtility.HtmlEncode(first_splitter[0]) + "</a></li>");
                    }
                }
                if (second_pre_menu_option.Length > 0)
                {
                    string[] second_splitter = second_pre_menu_option.Replace("[", "").Replace("]", "").Split(";".ToCharArray());
                    if (second_splitter.Length > 0)
                    {
                        Output.WriteLine("    <li><a href=\"" + second_splitter[1] + "\" title=\"" + HttpUtility.HtmlEncode(second_splitter[0]) + "\">" + HttpUtility.HtmlEncode(second_splitter[0]) + "</a></li>");
                    }
                }
            }

            // Add the HOME tab
            if ((RequestSpecificValues.Hierarchy_Object.Code == "all") || (RequestSpecificValues.Hierarchy_Object.Code == RequestSpecificValues.Current_Mode.Default_Aggregation))
            {
                // Add the 'SOBEK HOME' first menu option and suboptions
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Aggregation;
                RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Home;
                RequestSpecificValues.Current_Mode.Home_Type = Home_Type_Enum.List;

                if (RequestSpecificValues.Hierarchy_Object.Code == "all")
                {

                    // If some instance-wide pre-menu items existed, don't use the home image
                    if (pre_menu_options_exist)
                    {
                        Output.Write("    <li><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + sobek_home_text + "</a>");
                    }
                    else
                    {
                        Output.Write("    <li id=\"sbkAgm_Home\" class=\"sbkMenu_Home\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\" class=\"sbkMenu_NoPadding\"><img src=\"" + Static_Resources.Home_Png + "\" alt=\"Home\" /> <div class=\"sbkMenu_HomeText\">" + sobek_home_text + "</div></a>");
                    }

                    if ((UI_ApplicationCache_Gateway.Thematic_Headings != null) && (UI_ApplicationCache_Gateway.Thematic_Headings.Count > 0))
                    {
                        Output.WriteLine("<ul id=\"sbkAgm_HomeSubMenu\">");
                        Output.WriteLine("      <li id=\"sbkAgm_HomeListView\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + list_view_text + "</a></li>");
                        RequestSpecificValues.Current_Mode.Home_Type = Home_Type_Enum.Descriptions;
                        Output.WriteLine("      <li id=\"sbkAgm_HomeBriefView\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + brief_view_text + "</a></li>");
                        if (UI_ApplicationCache_Gateway.Settings.System.Include_TreeView_On_System_Home)
                        {
                            RequestSpecificValues.Current_Mode.Home_Type = Home_Type_Enum.Tree;
                            Output.WriteLine("      <li id=\"sbkAgm_HomeTreeView\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + tree_view_text + "</a></li>");
                        }
                        Output.WriteLine("    </ul></li>");
                    }
                    else
                    {
                        Output.WriteLine("</li>");
                    }
                }
                else
                {
                    Output.WriteLine("    <li id=\"sbkAgm_Home\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\" class=\"sbkAgm_NoPadding\"><img src=\"" + Static_Resources.Home_Png + "\" /> <div class=\"sbkAgm_HomeText\">" + sobek_home_text + "</div></a></li>");
                }
            }
            else
            {

                // Add the 'SOBEK HOME' first menu option and suboptions
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Aggregation;
                RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Home;
                RequestSpecificValues.Current_Mode.Home_Type = Home_Type_Enum.List;

                // If some instance-wide pre-menu items existed, don't use the home image
                if (pre_menu_options_exist)
                {
                    Output.WriteLine("    <li id=\"sbkAgm_Home\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + home + "</a><ul id=\"sbkAgm_HomeSubMenu\">");
                }
                else
                {
                    Output.WriteLine("    <li id=\"sbkAgm_Home\" class=\"sbkMenu_Home\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\" class=\"sbkMenu_NoPadding\"><img src=\"" + Static_Resources.Home_Png + "\" alt=\"Home\" /> <div class=\"sbkMenu_HomeText\">" + home + "</div></a><ul id=\"sbkAgm_HomeSubMenu\">");
                }

                Output.WriteLine("      <li id=\"sbkAgm_AggrHome\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + collection_home + "</a></li>");

                RequestSpecificValues.Current_Mode.Aggregation = String.Empty;
                if (RequestSpecificValues.Current_Mode.Default_Aggregation != "all")
                {
                    Output.WriteLine("      <li id=\"sbkAgm_InstanceHome\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + sobek_home_text + "</a></li>");
                }
                else
                {
                    Output.WriteLine("      <li id=\"sbkAgm_InstanceHome\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + sobek_home_text + "</a><ul id=\"sbkAgm_InstanceHomeSubMenu\">");

                    if ((UI_ApplicationCache_Gateway.Thematic_Headings != null) && (UI_ApplicationCache_Gateway.Thematic_Headings.Count > 0))
                    {
                        Output.WriteLine("        <li id=\"sbkAgm_HomeListView\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + list_view_text + "</a></li>");
                        RequestSpecificValues.Current_Mode.Home_Type = Home_Type_Enum.Descriptions;
                        Output.WriteLine("        <li id=\"sbkAgm_HomeBriefView\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + brief_view_text + "</a></li>");
                        if (UI_ApplicationCache_Gateway.Settings.System.Include_TreeView_On_System_Home)
                        {
                            RequestSpecificValues.Current_Mode.Home_Type = Home_Type_Enum.Tree;
                            Output.WriteLine("        <li id=\"sbkAgm_HomeTreeView\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + tree_view_text + "</a></li>");
                        }
                    }
                    else
                    {
                        Output.WriteLine("        <li id=\"sbkAgm_HomeListView\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + sobek_home_text + "</a></li>");
                    }
                    if ((RequestSpecificValues.Current_User != null) && (RequestSpecificValues.Current_User.LoggedOn))
                    {
                        RequestSpecificValues.Current_Mode.Home_Type = Home_Type_Enum.Personalized;
                        Output.WriteLine("        <li id=\"sbkAgm_HomePersonalized\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + myCollections + "</a></li>");
                    }
                    if (UI_ApplicationCache_Gateway.Settings.System.Include_Partners_On_System_Home)
                    {
                        RequestSpecificValues.Current_Mode.Home_Type = Home_Type_Enum.Partners_List;
                        Output.WriteLine("        <li id=\"sbkAgm_HomePartners\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + partners_text + "</a></li>");
                    }
                    Output.WriteLine("      </ul></li>");
                }

                Output.WriteLine("    </ul></li>");

                RequestSpecificValues.Current_Mode.Aggregation = RequestSpecificValues.Hierarchy_Object.Code;
            }

            // Add any additional search types
            RequestSpecificValues.Current_Mode.Mode = thisMode;
            List<string> other_searches = new List<string>();
            for (int i = 1; i < RequestSpecificValues.Hierarchy_Object.Views_And_Searches.Count; i++)
            {
                other_searches.Add(Aggregation_Nav_Bar_HTML_Factory.Menu_Get_Nav_Bar_HTML(RequestSpecificValues.Hierarchy_Object.Views_And_Searches[i], RequestSpecificValues.Current_Mode, UI_ApplicationCache_Gateway.Translation));
            }
            if (other_searches.Count == 1)
            {
                Output.WriteLine(other_searches[0]);
            }
            else if (other_searches.Count > 1)
            {
                Output.WriteLine("    <li><a href=\"\">" + otherSearches_text + "</a><ul id=\"sbkAgm_SearchSubMenu\">");
                foreach (string thisOtherSearch in other_searches)
                {
                    Output.Write("      " + thisOtherSearch);
                }

                Output.WriteLine("    </ul></li>");
            }

            // Replace any search string
            RequestSpecificValues.Current_Mode.Search_String = current_search;
            RequestSpecificValues.Current_Mode.Info_Browse_Mode = browse_code;
            RequestSpecificValues.Current_Mode.Aggregation_Type = thisAggrType;
            RequestSpecificValues.Current_Mode.Search_Type = thisSearch;
            RequestSpecificValues.Current_Mode.Mode = thisMode;
            RequestSpecificValues.Current_Mode.Home_Type = thisHomeType;
            RequestSpecificValues.Current_Mode.Result_Display_Type = resultsType;
            RequestSpecificValues.Current_Mode.Aggregation = aggregation;
            RequestSpecificValues.Current_Mode.Page = page;

            Result_Display_Type_Enum resultView = RequestSpecificValues.Current_Mode.Result_Display_Type;
            if (Include_Bookshelf_View)
            {
                if (resultView == Result_Display_Type_Enum.Bookshelf)
                {
                    Output.WriteLine("    <li class=\"selected-sf-menu-item-link\"><a href=\"\">" + bookshelf_view + "</a></li>");
                }
                else
                {
                    RequestSpecificValues.Current_Mode.Result_Display_Type = Result_Display_Type_Enum.Bookshelf;
                    Output.WriteLine("    <li><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode).Replace("&", "&amp;") + "\">" + bookshelf_view + "</a></li>");
                }
            }

            if (( !String.IsNullOrEmpty(RequestSpecificValues.Current_Mode.Coordinates)) || (RequestSpecificValues.Hierarchy_Object.Result_Views.Contains(Result_Display_Type_Enum.Map)))
            {
                if (resultView == Result_Display_Type_Enum.Map)
                {
                    Output.WriteLine("    <li class=\"selected-sf-menu-item-link\"><a href=\"\">" + map_view + "</a></li>");
                }
                else
                {
                    RequestSpecificValues.Current_Mode.Result_Display_Type = Result_Display_Type_Enum.Map;
                    Output.WriteLine("    <li><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode).Replace("&", "&amp;") + "\">" + map_view + "</a></li>");
                }
            }

            if (RequestSpecificValues.Hierarchy_Object.Result_Views.Contains(Result_Display_Type_Enum.Brief))
            {
                if (resultView == Result_Display_Type_Enum.Brief)
                {
                    Output.WriteLine("    <li class=\"selected-sf-menu-item-link\"><a href=\"\">" + brief_view + "</a></li>");
                }
                else
                {
                    RequestSpecificValues.Current_Mode.Result_Display_Type = Result_Display_Type_Enum.Brief;
                    Output.WriteLine("    <li><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode).Replace("&", "&amp;") + "\">" + brief_view + "</a></li>");
                }
            }

            if (RequestSpecificValues.Hierarchy_Object.Result_Views.Contains(Result_Display_Type_Enum.Table))
            {
                if (resultView == Result_Display_Type_Enum.Table)
                {
                    Output.WriteLine("    <li class=\"selected-sf-menu-item-link\"><a href=\"\">" + table_view + "</a></li>");
                }
                else
                {
                    RequestSpecificValues.Current_Mode.Result_Display_Type = Result_Display_Type_Enum.Table;
                    Output.WriteLine("    <li><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode).Replace("&", "&amp;") + "\">" + table_view + "</a></li>");
                }
            }

            if (RequestSpecificValues.Hierarchy_Object.Result_Views.Contains(Result_Display_Type_Enum.Thumbnails))
            {
                if (resultView == Result_Display_Type_Enum.Thumbnails)
                {
                    Output.WriteLine("    <li class=\"selected-sf-menu-item-link\"><a href=\"\">" + thumbnail_view + "</a></li>");
                }
                else
                {
                    RequestSpecificValues.Current_Mode.Result_Display_Type = Result_Display_Type_Enum.Thumbnails;
                    Output.WriteLine("    <li><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode).Replace("&", "&amp;") + "\">" + thumbnail_view + "</a></li>");
                }
            }

            if ((Include_Bookshelf_View) && ((resultView == Result_Display_Type_Enum.Export) || (RequestSpecificValues.Current_Mode.Writer_Type == Writer_Type_Enum.HTML_LoggedIn)))
            {
                RequestSpecificValues.Current_Mode.Page = 1;
                if (resultView == Result_Display_Type_Enum.Export)
                {
                    Output.WriteLine("    <li class=\"selected-sf-menu-item-link\"><a href=\"\">" + EXPORT_VIEW + "</a></li>");
                }
                else
                {
                    RequestSpecificValues.Current_Mode.Result_Display_Type = Result_Display_Type_Enum.Export;
                    Output.WriteLine("    <li><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode).Replace("&", "&amp;") + "\">" + EXPORT_VIEW + "</a></li>");
                }
            }

            // Return the code and mode back
            RequestSpecificValues.Current_Mode.Info_Browse_Mode = browse_code;
            RequestSpecificValues.Current_Mode.Aggregation_Type = thisAggrType;
            RequestSpecificValues.Current_Mode.Search_Type = thisSearch;
            RequestSpecificValues.Current_Mode.Mode = thisMode;
            RequestSpecificValues.Current_Mode.Home_Type = thisHomeType;
            RequestSpecificValues.Current_Mode.Result_Display_Type = resultsType;
            RequestSpecificValues.Current_Mode.Aggregation = aggregation;
            RequestSpecificValues.Current_Mode.Page = page;

            Output.WriteLine("  </ul>");
            Output.WriteLine("</nav>");
            Output.WriteLine();

            Output.WriteLine("<!-- Initialize the main user menu -->");
            Output.WriteLine("<script>");
            Output.WriteLine("  jQuery(document).ready(function () {");
            Output.WriteLine("     jQuery('ul.sf-menu').superfish({");

            Output.WriteLine("          onBeforeShow: function() { ");
            Output.WriteLine("               if ( $(this).attr('id') == 'sbkAgm_SubCollectionsMenu')");
            Output.WriteLine("               {");
            Output.WriteLine("                 var thisWidth = $(this).width();");
            Output.WriteLine("                 var parent = $('#sbkAgm_SubCollections');");
            Output.WriteLine("                 var offset = $('#sbkAgm_SubCollections').offset();");
            Output.WriteLine("                 if ( $(window).width() < offset.left + thisWidth )");
            Output.WriteLine("                 {");
            Output.WriteLine("                   var newleft = thisWidth - parent.width();");
            Output.WriteLine("                   $(this).css('left', '-' + newleft + 'px');");
            Output.WriteLine("                 }");
            Output.WriteLine("               }");
            Output.WriteLine("          }");

            Output.WriteLine("    });");
            Output.WriteLine("  });");
            Output.WriteLine("</script>");
            Output.WriteLine();
        }



        /// <summary> Add the user-specific main menu user for most of the pages which 
        /// require a user to be logged in, such as the mySobek pages, the internal user
        /// pages, and the system administration pages </summary>
        /// <param name="Output"> Stream to which to write the HTML for this menu </param>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        public static void Add_UserSpecific_Main_Menu(TextWriter Output, RequestCache RequestSpecificValues )
        {
            // Add the item views
            Output.WriteLine("<!-- Add the main user-specific menu -->");
            Output.WriteLine("<nav id=\"sbkUsm_MenuBar\" class=\"sbkMenu_Bar\" role=\"navigation\" aria-label=\"User menu\">");
            Output.WriteLine("<h2 class=\"hidden-element\">User menu</h2>");
            Output.WriteLine("  <ul class=\"sf-menu\">");

            // Save the current view information type
            Display_Mode_Enum currentMode = RequestSpecificValues.Current_Mode.Mode;
            Admin_Type_Enum adminType = RequestSpecificValues.Current_Mode.Admin_Type;
            My_Sobek_Type_Enum mySobekType = RequestSpecificValues.Current_Mode.My_Sobek_Type;
            Internal_Type_Enum internalType = RequestSpecificValues.Current_Mode.Internal_Type;
            Result_Display_Type_Enum resultType = RequestSpecificValues.Current_Mode.Result_Display_Type;
            string mySobekSubmode = RequestSpecificValues.Current_Mode.My_Sobek_SubMode;
            ushort? page = RequestSpecificValues.Current_Mode.Page;

            // Ensure some values that SHOULD be blank really are
            RequestSpecificValues.Current_Mode.Aggregation = String.Empty;

            // Get ready to draw the tabs
            string sobek_home_text = RequestSpecificValues.Current_Mode.Instance_Abbreviation + " Home";
            const string myCollections = "My Collections";
            string my_sobek_home_text = "<span style=\"text-transform:lowercase\">my</span>" + RequestSpecificValues.Current_Mode.Instance_Abbreviation + " Home";
            const string myLibrary = "My Library";
            const string myPreferences = "My Account";
            const string internal_text = "Internal";
            string sobek_admin_text = "System Admin";
            if ((RequestSpecificValues.Current_User != null) && (RequestSpecificValues.Current_User.Is_Portal_Admin) && (!RequestSpecificValues.Current_User.Is_System_Admin))
                sobek_admin_text = "Portal Admin";
            const string list_view_text = "List View";
            const string brief_view_text = "Brief View";
            const string tree_view_text = "Tree View";
            const string partners_text = "Browse Partners";
            const string advanced_search_text = "Advanced Search";

            string collection_details_text = "Collection List";
            string collection_tree_text = "Collection Hierarchy";
            string new_items_text = "New Items";
            string memory_mgmt_text = "Memory Management";
            string wordmarks_text = "Wordmarks";
            string build_failures_text = "Build Failures";

            if (RequestSpecificValues.Current_Mode.Language == Web_Language_Enum.Spanish)
            {
                //title = "INICIO";
                collection_details_text = "DETALLES DE LA COLECCIÓN";
                new_items_text = "NUEVOS ARTÍCULOS";
                memory_mgmt_text = "MEMORIA";
            }

            if (RequestSpecificValues.Current_Mode.Language == Web_Language_Enum.French)
            {
                //title = "PAGE D'ACCUEIL";
                collection_details_text = "DETAILS DE LA COLLECTION";
                new_items_text = "LES NOUVEAUX DOCUMENTS";
                memory_mgmt_text = "MÉMOIRE";
            }

            // Add the 'SOBEK HOME' first menu option and suboptions
            RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Aggregation;
            RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Home;
            RequestSpecificValues.Current_Mode.Home_Type = Home_Type_Enum.List;
            Output.WriteLine("    <li id=\"sbkUsm_Home\" class=\"sbkMenu_Home\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\" class=\"sbkMenu_NoPadding\"><img src=\"" + Static_Resources.Home_Png + "\" alt=\"Home\" /> <div class=\"sbkMenu_HomeText\">" + sobek_home_text + "</div></a><ul id=\"sbkUsm_HomeSubMenu\">");

            if ((UI_ApplicationCache_Gateway.Thematic_Headings != null) && (UI_ApplicationCache_Gateway.Thematic_Headings.Count > 0))
            {
                Output.WriteLine("      <li id=\"sbkUsm_HomeListView\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + list_view_text + "</a></li>");
                RequestSpecificValues.Current_Mode.Home_Type = Home_Type_Enum.Descriptions;
                Output.WriteLine("      <li id=\"sbkUsm_HomeBriefView\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + brief_view_text + "</a></li>");
                if (UI_ApplicationCache_Gateway.Settings.System.Include_TreeView_On_System_Home)
                {
                    RequestSpecificValues.Current_Mode.Home_Type = Home_Type_Enum.Tree;
                    Output.WriteLine("      <li id=\"sbkUsm_HomeTreeView\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + tree_view_text + "</a></li>");
                }
            }
            else
            {
                Output.WriteLine("      <li id=\"sbkUsm_HomeListView\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + sobek_home_text + "</a></li>");

            }
            if (RequestSpecificValues.Current_User != null)
            {
                RequestSpecificValues.Current_Mode.Home_Type = Home_Type_Enum.Personalized;
                Output.WriteLine("      <li id=\"sbkUsm_HomePersonalized\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + myCollections + "</a></li>");
            }
            if (UI_ApplicationCache_Gateway.Settings.System.Include_Partners_On_System_Home)
            {
                RequestSpecificValues.Current_Mode.Home_Type = Home_Type_Enum.Partners_List;
                Output.WriteLine("      <li id=\"sbkUsm_HomePartners\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + partners_text + "</a></li>");
            }
            string advanced_url = RequestSpecificValues.Current_Mode.Base_URL + "/advanced";
            Output.WriteLine("      <li id=\"sbkUsm_HomeAdvSearch\"><a href=\"" + advanced_url + "\">" + advanced_search_text + "</a></li>");
            Output.WriteLine("    </ul></li>");

            if (RequestSpecificValues.Current_User != null)
            {
                // Add the 'mySOBEK HOME' second menu option and suboptions
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.My_Sobek;
                RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                Output.WriteLine("    <li><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + my_sobek_home_text + "</a><ul id=\"sbkUsm_MySubMenu\">");

                // If a user can submit, add a link to start a new item
                if ((RequestSpecificValues.Current_User.Can_Submit) && (UI_ApplicationCache_Gateway.Settings.Resources.Online_Item_Submit_Enabled))
                {
                    RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.New_Item;
                    RequestSpecificValues.Current_Mode.My_Sobek_SubMode = "1";
                    Output.WriteLine("      <li id=\"sbkUsm_MyStartNew\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.New_Item_Img_Small + "\" /> <div class=\"sbkUsm_TextWithImage\">Start a new item</div></a></li>");
                }

                // If the user has already submitted stuff, add a link to all submitted items
                if (RequestSpecificValues.Current_User.Items_Submitted_Count > 0)
                {
                    RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Folder_Management;
                    RequestSpecificValues.Current_Mode.Result_Display_Type = Result_Display_Type_Enum.Brief;
                    RequestSpecificValues.Current_Mode.My_Sobek_SubMode = "Submitted Items";
                    Output.WriteLine("      <li id=\"sbkUsm_MySubmittedItems\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Submitted_Items_Gif + "\" /> <div class=\"sbkUsm_TextWithImage\">View my submitted items</div></a></li>");
                }

                // If this user is linked to item statistics, add that link as well
                if (RequestSpecificValues.Current_User.Has_Item_Stats)
                {
                    // Add link to folder management
                    RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.User_Usage_Stats;
                    RequestSpecificValues.Current_Mode.My_Sobek_SubMode = String.Empty;
                    Output.WriteLine("      <li id=\"sbkUsm_MyItemStats\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Usage_Img + "\" /> <div class=\"sbkUsm_TextWithImage\">View usage for my items</div></a></li>");
                }

                // If the user has submitted some descriptive tags, or has the kind of rights that let them
                // view lists of tags, add that
                if ((RequestSpecificValues.Current_User.Has_Descriptive_Tags) || (RequestSpecificValues.Current_User.Is_System_Admin) || (RequestSpecificValues.Current_User.Is_A_Collection_Manager_Or_Admin))
                {
                    // Add link to folder management
                    RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.User_Tags;
                    RequestSpecificValues.Current_Mode.My_Sobek_SubMode = String.Empty;
                    Output.WriteLine("      <li id=\"sbkUsm_MyTags\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Chat_Png + "\" /> <div class=\"sbkUsm_TextWithImage\">View my descriptive tags</div></a></li>");
                }

                // Add link to folder management
                RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Folder_Management;
                RequestSpecificValues.Current_Mode.My_Sobek_SubMode = String.Empty;
                RequestSpecificValues.Current_Mode.Result_Display_Type = Result_Display_Type_Enum.Bookshelf;
                Output.WriteLine("      <li id=\"sbkUsm_MyBookshelf\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Bookshelf_Img + "\" /> <div class=\"sbkUsm_TextWithImage\">View my bookshelves</div></a></li>");

                // Add a link to view all saved searches
                RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Saved_Searches;
                Output.WriteLine("      <li id=\"sbkUsm_MySearches\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Saved_Searches_Img + "\" /> <div class=\"sbkUsm_TextWithImage\">View my saved searches</div></a></li>");

                // Add a link to edit your preferences
                RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Preferences;
                Output.WriteLine("      <li id=\"sbkUsm_MyAccount\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Settings_Img + "\" /> <div class=\"sbkUsm_TextWithImage\">Account preferences</div></a></li>");

                // Add a log out link
                RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Log_Out;
                Output.WriteLine("      <li id=\"sbkUsm_MyLogOut\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Exit_Gif + "\" /> <div class=\"sbkUsm_TextWithImage\">Log Out</div></a></li>");

                Output.WriteLine("    </ul></li>");


                // If this user is internal, add that
                if ((RequestSpecificValues.Current_User.Is_Internal_User) || (RequestSpecificValues.Current_User.Is_Portal_Admin) || (RequestSpecificValues.Current_User.Is_System_Admin))
                {
                    RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Internal;
                    RequestSpecificValues.Current_Mode.Internal_Type = Internal_Type_Enum.Aggregations_List;
                    Output.WriteLine("    <li id=\"sbkUsm_Internal\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + internal_text + "</a><ul id=\"sbkUsm_InternalSubMenu\">");

                    RequestSpecificValues.Current_Mode.Internal_Type = Internal_Type_Enum.Aggregations_List;
                    Output.WriteLine("      <li id=\"sbkUsm_InternalAggregations\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + collection_details_text + "</a><ul id=\"sbkUsm_InternalAggrSubMenu\">");

                    Output.WriteLine("      <li id=\"sbkUsm_InternalAggrList\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + collection_details_text + "</a></li>");

                    RequestSpecificValues.Current_Mode.Internal_Type = Internal_Type_Enum.Aggregations_Tree;
                    Output.WriteLine("      <li id=\"sbkUsm_InternalAggrTree\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + collection_tree_text + "</a></li>");
                    Output.WriteLine("      </ul></li>");

                    RequestSpecificValues.Current_Mode.Internal_Type = Internal_Type_Enum.New_Items;
                    Output.WriteLine("      <li id=\"sbkUsm_InternalNewItems\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + new_items_text + "</a></li>");

                    RequestSpecificValues.Current_Mode.Internal_Type = Internal_Type_Enum.Build_Failures;
                    Output.WriteLine("      <li id=\"sbkUsm_InternalBuildFailures\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + build_failures_text + "</a></li>");

                    RequestSpecificValues.Current_Mode.Internal_Type = Internal_Type_Enum.Cache;
                    Output.WriteLine("      <li id=\"sbkUsm_InternalCache\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + memory_mgmt_text + "</a></li>");

                    RequestSpecificValues.Current_Mode.Internal_Type = Internal_Type_Enum.Wordmarks;
                    Output.WriteLine("      <li id=\"sbkUsm_InternalWordmarks\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + wordmarks_text + "</a></li>");

                    //// The only time we don't show the standard INTERNAL view selectors is when NEW ITEMS is selected
                    //// and there are recenly added NEW ITEMS
                    //DataTable New_Items = null;
                    //if (type == Internal_Type_Enum.New_Items)
                    //    New_Items = SobekCM_Database.Tracking_Update_List(Tracer);

                    //// If this user is internal, add that
                    //if ((New_Items != null) && (New_Items.Rows.Count > 0))
                    //{
                    //    CurrentMode.Mode = Display_Mode_Enum.Internal;
                    //    Output.WriteLine("  <a href=\"" + UrlWriterHelper.Redirect_URL(CurrentMode) + "\">" + Selected_Tab_Start + internalTab + Selected_Tab_End + "</a>");
                    //    CurrentMode.Mode = Display_Mode_Enum.My_Sobek;
                    //}
                    //else
                    //{
                    //    Output.WriteLine(Selected_Tab_Start + internalTab + Selected_Tab_End);
                    //}

                    Output.WriteLine("    </ul></li>");
                }

                // If this user is a sys admin or portal admin, add that
                if ((RequestSpecificValues.Current_User.Is_System_Admin) || (RequestSpecificValues.Current_User.Is_Portal_Admin))
                {
                    RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Administrative;
                    RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Home;
                    string current_url = UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode);

                    Output.WriteLine("    <li id=\"sbkUsm_Admin\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + sobek_admin_text + "</a><ul id=\"sbkUsm_AdminSubMenu\">");

                    // Common tasks menu
                    Output.WriteLine("      <li id=\"sbkUsm_AdminCommonTasks\"><a href=\"" + current_url + "#common\"> <div class=\"sbkUsm_TextWithImage\">Common Tasks</div></a><ul>");

                    RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Add_Collection_Wizard;
                    Output.WriteLine("        <li id=\"sbkUsm_AdminAggrWizard\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Wizard_Img + "\" /> <div class=\"sbkUsm_TextWithImage\">Add Collection Wizard</div></a></li>");

                    RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Skins_Single;
                    RequestSpecificValues.Current_Mode.My_Sobek_SubMode = RequestSpecificValues.Current_Mode.Skin;
                    Output.WriteLine("        <li id=\"sbkUsm_AdminCurrSkin\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Skins_Img_Small + "\" /> <div class=\"sbkUsm_TextWithImage\">Edit Current Web Skin</div></a></li>");
                    RequestSpecificValues.Current_Mode.My_Sobek_SubMode = String.Empty;

                    if (RequestSpecificValues.Current_User.Is_System_Admin)
                    {
                        // Edit users
                        RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Users;
                        Output.WriteLine("        <li id=\"sbkUsm_AdminUsers\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Users_Img_Small + "\" /> <div class=\"sbkUsm_TextWithImage\">Users and Groups</div></a></li>");
                    }

                    Output.WriteLine("      </ul></li>");

                    // Appearance submenu
                    Output.WriteLine("      <li id=\"sbkUsm_AdminAppearance\"><a href=\"" + current_url + "#appearance\"> <div class=\"sbkUsm_TextWithImage\">Appearance</div></a><ul>");

                    RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Skins_Single;
                    RequestSpecificValues.Current_Mode.My_Sobek_SubMode = RequestSpecificValues.Current_Mode.Skin;
                    Output.WriteLine("        <li id=\"sbkUsm_AdminCurrSkin\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Skins_Img_Small + "\" /> <div class=\"sbkUsm_TextWithImage\">Edit Current Web Skin</div></a></li>");
                    RequestSpecificValues.Current_Mode.My_Sobek_SubMode = String.Empty;

                    // Edit URL Portals
                    RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.URL_Portals;
                    Output.WriteLine("        <li id=\"sbkUsm_AdminPortals\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Portals_Img_Small + "\" /> <div class=\"sbkUsm_TextWithImage\">URL Portals</div></a></li>");

                    // Edit interfaces
                    RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Skins_Mgmt;
                    Output.WriteLine("        <li id=\"sbkUsm_AdminSkin\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Skins_Img_Small + "\" /> <div class=\"sbkUsm_TextWithImage\">Web Skins</div></a></li>");

                    Output.WriteLine("      </ul></li>");

                    // Collections submenu
                    Output.WriteLine("      <li id=\"sbkUsm_AdminCollections\"><a href=\"" + current_url + "#collections\"> <div class=\"sbkUsm_TextWithImage\">Collections</div></a><ul>");


                    RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Add_Collection_Wizard;
                    Output.WriteLine("        <li id=\"sbkUsm_AdminAggrWizard\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Wizard_Img + "\" /> <div class=\"sbkUsm_TextWithImage\">Add Collection Wizard</div></a></li>");

                    // Edit forwarding
                    RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Aliases;
                    Output.WriteLine("        <li id=\"sbkUsm_AdminForwarding\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Aliases_Img_Small + "\" /> <div class=\"sbkUsm_TextWithImage\">Aggregation Aliases</div></a></li>");

                    // Edit item aggregationPermissions
                    RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Aggregations_Mgmt;
                    Output.WriteLine("        <li id=\"sbkUsm_AdminAggr\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Aggregations_Img + "\" /> <div class=\"sbkUsm_TextWithImage\">Aggregation Management</div></a></li>");

                    // Edit Thematic Headings
                    RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Thematic_Headings;
                    Output.WriteLine("        <li id=\"sbkUsm_AdminThematic\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Thematic_Heading_Img_Small + "\" /> <div class=\"sbkUsm_TextWithImage\">Thematic Headings</div></a></li>");


                    Output.WriteLine("      </ul></li>");

                    // Items submenu
                    Output.WriteLine("      <li id=\"sbkUsm_AdminItems\"><a href=\"" + current_url + "#items\"> <div class=\"sbkUsm_TextWithImage\">Items</div></a><ul>");

                    // Edit Default_Metadata
                    RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Default_Metadata;
                    Output.WriteLine("        <li id=\"sbkUsm_AdminProjects\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Pmets_Img + "\" /> <div class=\"sbkUsm_TextWithImage\">Default Metadata</div></a></li>");

                    // Edit wordmarks
                    RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Wordmarks;
                    Output.WriteLine("        <li id=\"sbkUsm_AdminWordmarks\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Wordmarks_Img_Small + "\" /> <div class=\"sbkUsm_TextWithImage\">Wordmarks / Icons</div></a></li>");

                    // View and set SobekCM Builder Status
                    RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Builder_Status;
                    Output.WriteLine("        <li id=\"sbkUsm_AdminStatus\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Gears_Img_Small + "\" /> <div class=\"sbkUsm_TextWithImage\">Builder Status</div></a></li>");

                    Output.WriteLine("      </ul></li>");
                    
                    // Settings submenu
                    Output.WriteLine("      <li id=\"sbkUsm_AdminSettings\"><a href=\"" + current_url + "#settings\"> <div class=\"sbkUsm_TextWithImage\">Settings</div></a><ul>");



                    // Edit IP Restrictions
                    RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.IP_Restrictions;
                    Output.WriteLine("        <li id=\"sbkUsm_AdminRestrictions\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Firewall_Img_Small + "\" /> <div class=\"sbkUsm_TextWithImage\">IP Restriction Ranges</div></a></li>");

                    // Edit Settings
                    RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Settings;
                    Output.WriteLine("        <li id=\"sbkUsm_AdminSettings\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Settings_Img_Small + "\" /> <div class=\"sbkUsm_TextWithImage\">System-Wide Settings</div></a></li>");

                    
                    // Reset cache
                    RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Reset;
                    Output.WriteLine("        <li id=\"sbkUsm_AdminReset\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Refresh_Img_Small + "\" /> <div class=\"sbkUsm_TextWithImage\">Reset Cache</div></a></li>");

                    Output.WriteLine("      </ul></li>");

                    // Permissions submenu
                    Output.WriteLine("      <li id=\"sbkUsm_AdminPermissions\"><a href=\"" + current_url + "#permissions\"> <div class=\"sbkUsm_TextWithImage\">Users & Permissions</div></a><ul>");

                    // Edit users
                    RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.User_Permissions_Reports;
                    Output.WriteLine("        <li id=\"sbkUsm_AdminUserReport\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.User_Permission_Img + "\" /> <div class=\"sbkUsm_TextWithImage\">User Permissions Reports</div></a></li>");

                    // Edit users
                    if (RequestSpecificValues.Current_User.Is_System_Admin)
                    {
                        // Edit users
                        RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.Users;
                        Output.WriteLine("        <li id=\"sbkUsm_AdminUsers\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.Users_Img_Small + "\" /> <div class=\"sbkUsm_TextWithImage\">Users and Groups</div></a></li>");
                    }

                    Output.WriteLine("      </ul></li>");


                    // Web content pages
                    Output.WriteLine("      <li id=\"sbkUsm_WebContentMenu\"><a href=\"" + current_url + "#webcontent\"> <div class=\"sbkUsm_TextWithImage\">Web Content Pages</div></a><ul>");

                    // Manage web content pages
                    RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.WebContent_Mgmt;
                    Output.WriteLine("        <li id=\"sbkUsm_WebContentPages\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.WebContent_Img_Small + "\" /> <div class=\"sbkUsm_TextWithImage\">Manage Web Content Pages</div></a></li>");

                    // Manage web content pages
                    RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.WebContent_History;
                    Output.WriteLine("        <li id=\"sbkUsm_WebContentHistory\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.WebContent_History_Img_Small + "\" /> <div class=\"sbkUsm_TextWithImage\">Web Content Recent Updates</div></a></li>");

                    // Manage web content pages
                    RequestSpecificValues.Current_Mode.Admin_Type = Admin_Type_Enum.WebContent_Usage;
                    Output.WriteLine("        <li id=\"sbkUsm_WebContentUsage\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\"><img src=\"" + Static_Resources.WebContent_Usage_Img_Small + "\" /> <div class=\"sbkUsm_TextWithImage\">Web Content Usage Reports</div></a></li>");

                    Output.WriteLine("      </ul></li>");
                    Output.WriteLine("    </ul></li>");
                }
           
            }


            // Add link to my libary (repeat of option in mySobek menu)
            RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.My_Sobek;
            RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Folder_Management;
            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = String.Empty;
            RequestSpecificValues.Current_Mode.Result_Display_Type = Result_Display_Type_Enum.Bookshelf;
            Output.WriteLine("    <li id=\"sbkUsm_Bookshelf\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + myLibrary + "</a></li>");

            // Add a link to my account (repeat of option in mySobek menu)
            RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Preferences;
            Output.WriteLine("      <li id=\"sbkUsm_Account\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + myPreferences + "</a></li>");


            Output.WriteLine("  </ul>");
			Output.WriteLine("</nav>");
            Output.WriteLine();

            Output.WriteLine("<!-- Initialize the main user menu -->");
            Output.WriteLine("<script>");
            Output.WriteLine("  jQuery(document).ready(function () {");
            Output.WriteLine("     jQuery('ul.sf-menu').superfish();");
            Output.WriteLine("  });");
            Output.WriteLine("</script>");
            Output.WriteLine();

            // Restore the current view information type
            RequestSpecificValues.Current_Mode.Mode = currentMode;
            RequestSpecificValues.Current_Mode.Admin_Type = adminType;
            RequestSpecificValues.Current_Mode.My_Sobek_Type = mySobekType;
            RequestSpecificValues.Current_Mode.Internal_Type = internalType;
            RequestSpecificValues.Current_Mode.Result_Display_Type = resultType;
            RequestSpecificValues.Current_Mode.My_Sobek_SubMode = mySobekSubmode;
            RequestSpecificValues.Current_Mode.Page = page;
        }
    }
}
