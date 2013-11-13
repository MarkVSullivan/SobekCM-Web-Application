using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using SobekCM.Library.AggregationViewer;
using SobekCM.Library.Aggregations;
using SobekCM.Library.Application_State;
using SobekCM.Library.Configuration;
using SobekCM.Library.Navigation;
using SobekCM.Library.Search;
using SobekCM.Library.Settings;
using SobekCM.Library.Users;

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
		/// <param name="Mode"> Mode / navigation information for the current request</param>
		/// <param name="User"> Currently logged on user (or object representing the unlogged on user's preferences) </param>
		/// <param name="Current_Aggregation"> Aggregation object which may have additional aggregation-level child pages to display in the main menu  </param>
		/// <param name="Translations"> Language support object for writing the name of the view in the appropriate interface language </param>
		/// <param name="Code_Manager"> List of valid collection codes, including mapping from the Sobek collections to Greenstone collections</param>
		public static void Add_Aggregation_Main_Menu(TextWriter Output, SobekCM_Navigation_Object Mode, User_Object User, Item_Aggregation Current_Aggregation, Language_Support_Info Translations, Aggregation_Code_Manager Code_Manager )
		{
			Output.WriteLine("<!-- Add the main aggregation menu -->");
			Output.WriteLine("<div id=\"sbkAgm_MenuBar\" class=\"sbkMenu_Bar\">");
			Output.WriteLine("<ul class=\"sf-menu\" id=\"sbkAgm_Menu\">");

			// Get ready to draw the tabs
			string home = "Home";
			string collection_home = Translations.Get_Translation(Current_Aggregation.ShortName, Mode.Language ) + " Home";
			string sobek_home_text = Mode.SobekCM_Instance_Abbreviation + " Home";
			string viewItems = "View Items";
			string allItems = "View All Items";
			string newItems = "View Recently Added Items";
			string myCollections = "MY COLLECTIONS";
			string partners = "BROWSE PARTNERS";
			string browseBy = "BROWSE BY";
			const string BROWSE_MAP = "MAP BROWSE";
			const string list_view_text = "List View";
			const string brief_view_text = "Brief View";
			const string tree_view_text = "Tree View";
			const string partners_text = "Browse Partners";

			if (Mode.Language == Web_Language_Enum.Spanish)
			{
				home = "INICIO";
				collection_home = "INICIO " + Translations.Get_Translation(Current_Aggregation.ShortName, Mode.Language);
				sobek_home_text = "INICIO " + Mode.SobekCM_Instance_Abbreviation.ToUpper();
				allItems = "TODOS LOS ARTÍCULOS";
				newItems = "NUEVOS ARTÍCULOS";
				browseBy = "BÚSQUEDA POR";
				partners = "AFILIADOS";
				myCollections = "MIS COLECCIONES";
			}

			if (Mode.Language == Web_Language_Enum.French)
			{
				home = "PAGE D'ACCUEIL";
				sobek_home_text = "PAGE D'ACCUEIL";
				allItems = "TOUS LES ARTICLES";
				newItems = "NOUVEAUX ARTICLES";
				browseBy = "PARCOURIR PAR";

			}

			// Save the current mode and browse
			Display_Mode_Enum thisMode = Mode.Mode;
			Aggregation_Type_Enum thisAggrType = Mode.Aggregation_Type;
			Search_Type_Enum thisSearch = Mode.Search_Type;
			Home_Type_Enum thisHomeType = Mode.Home_Type;
			Result_Display_Type_Enum resultsType = Mode.Result_Display_Type;
			ushort page = Mode.Page;
			string browse_code = Mode.Info_Browse_Mode;
			string aggregation = Mode.Aggregation;
			if ((thisMode == Display_Mode_Enum.Aggregation) && ((thisAggrType == Aggregation_Type_Enum.Browse_Info) || (thisAggrType == Aggregation_Type_Enum.Child_Page_Edit)))
			{
				browse_code = Mode.Info_Browse_Mode;
			}

			// Get the home search type (just to do a matching in case it was explicitly requested)
			Item_Aggregation.CollectionViewsAndSearchesEnum homeView = Item_Aggregation.CollectionViewsAndSearchesEnum.Basic_Search;
			if (Current_Aggregation.Views_And_Searches.Count > 0)
			{
				homeView = Current_Aggregation.Views_And_Searches[0];
			}

			// Remove any search string
			string current_search = Mode.Search_String;
			Mode.Search_String = String.Empty;

			// Add any PRE-MENU instance options
			bool pre_menu_options_exist = false;
			string first_pre_menu_option = String.Empty;
			string second_pre_menu_option = String.Empty;
			if (SobekCM_Library_Settings.Additional_Settings.ContainsKey("Aggregation Viewer.Static First Menu Item"))
				first_pre_menu_option = SobekCM_Library_Settings.Additional_Settings["Aggregation Viewer.Static First Menu Item"];
			if (SobekCM_Library_Settings.Additional_Settings.ContainsKey("Aggregation Viewer.Static Second Menu Item"))
				second_pre_menu_option = SobekCM_Library_Settings.Additional_Settings["Aggregation Viewer.Static Second Menu Item"];
			if ((first_pre_menu_option.Length > 0) || (second_pre_menu_option.Length > 0))
			{
				pre_menu_options_exist = true;
				if (first_pre_menu_option.Length > 0)
				{
					string[] first_splitter = first_pre_menu_option.Replace("[", "").Replace("]", "").Split(";".ToCharArray());
					if (first_splitter.Length > 0)
					{
						Output.WriteLine("\t\t<li><a href=\"" + first_splitter[1] + "\" title=\"" + System.Web.HttpUtility.HtmlEncode(first_splitter[0]) + "\">" + System.Web.HttpUtility.HtmlEncode(first_splitter[0]) + "</a></li>");
					}
				}
				if (second_pre_menu_option.Length > 0)
				{
					string[] second_splitter = second_pre_menu_option.Replace("[", "").Replace("]", "").Split(";".ToCharArray());
					if (second_splitter.Length > 0)
					{
						Output.WriteLine("\t\t<li><a href=\"" + second_splitter[1] + "\" title=\"" + System.Web.HttpUtility.HtmlEncode(second_splitter[0]) + "\">" + System.Web.HttpUtility.HtmlEncode(second_splitter[0]) + "</a></li>");
					}
				}
			}

			bool isOnHome = (((Mode.Mode == Display_Mode_Enum.Aggregation) && (Mode.Aggregation_Type == Aggregation_Type_Enum.Home)) ||
				 ((Mode.Mode == Display_Mode_Enum.Search) &&
				  (Aggregation_Nav_Bar_HTML_Factory.Do_Search_Types_Match(homeView, Mode.Search_Type))));

			// Add the HOME tab
			if ((Current_Aggregation.Code == "all") || ( Current_Aggregation.Code == Mode.Default_Aggregation))
			{
				// Add the 'SOBEK HOME' first menu option and suboptions
				Mode.Mode = Display_Mode_Enum.Aggregation;
				Mode.Aggregation_Type = Aggregation_Type_Enum.Home;
				Mode.Home_Type = Home_Type_Enum.List;

				if (Current_Aggregation.Code == "all")
				{
					// What is considered HOME changes here at the top level
					if ((thisHomeType == Home_Type_Enum.Partners_List) || (thisHomeType == Home_Type_Enum.Partners_Thumbnails) || (thisHomeType == Home_Type_Enum.Personalized))
						isOnHome = false;

					// If some instance-wide pre-menu items existed, don't use the home image
					if (pre_menu_options_exist)
					{
						if (isOnHome)
							Output.WriteLine("\t\t<li class=\"selected-sf-menu-item-link\"><a href=\"" + Mode.Redirect_URL() + "\">" + sobek_home_text + "</a><ul id=\"sbkAgm_HomeSubMenu\">");
						else
							Output.WriteLine("\t\t<li><a href=\"" + Mode.Redirect_URL() + "\">" + sobek_home_text + "</a><ul id=\"sbkAgm_HomeSubMenu\">");
					}
					else
					{
						if ( isOnHome )
							Output.WriteLine("\t\t<li id=\"sbkAgm_Home\" class=\"sbkMenu_Home selected-sf-menu-item-link\"><a href=\"" + Mode.Redirect_URL() + "\" class=\"sbkMenu_NoPadding\"><img src=\"" + Mode.Default_Images_URL + "home.png\" /> <div class=\"sbkMenu_HomeText\">" + sobek_home_text + "</div></a><ul id=\"sbkAgm_HomeSubMenu\">");
						else
							Output.WriteLine("\t\t<li id=\"sbkAgm_Home\" class=\"sbkMenu_Home\"><a href=\"" + Mode.Redirect_URL() + "\" class=\"sbkMenu_NoPadding\"><img src=\"" + Mode.Default_Images_URL + "home.png\" /> <div class=\"sbkMenu_HomeText\">" + sobek_home_text + "</div></a><ul id=\"sbkAgm_HomeSubMenu\">");
					}

					Output.WriteLine("\t\t\t<li id=\"sbkAgm_HomeListView\"><a href=\"" + Mode.Redirect_URL() + "\">" + list_view_text + "</a></li>");
					Mode.Home_Type = Home_Type_Enum.Descriptions;
					Output.WriteLine("\t\t\t<li id=\"sbkAgm_HomeBriefView\"><a href=\"" + Mode.Redirect_URL() + "\">" + brief_view_text + "</a></li>");
					if (SobekCM_Library_Settings.Include_TreeView_On_System_Home)
					{
						Mode.Home_Type = Home_Type_Enum.Tree_Collapsed;
						Output.WriteLine("\t\t\t<li id=\"sbkAgm_HomeTreeView\"><a href=\"" + Mode.Redirect_URL() + "\">" + tree_view_text + "</a></li>");
					}
					Output.WriteLine("\t\t</ul></li>");
				}
				else
				{
					Output.WriteLine("\t\t<li id=\"sbkAgm_Home\"><a href=\"" + Mode.Redirect_URL() + "\" class=\"sbkAgm_NoPadding\"><img src=\"" + Mode.Default_Images_URL + "home.png\" /> <div class=\"sbkAgm_HomeText\">" + sobek_home_text + "</div></a></li>");
				}
			}
			else
			{

				// Add the 'SOBEK HOME' first menu option and suboptions
				Mode.Mode = Display_Mode_Enum.Aggregation;
				Mode.Aggregation_Type = Aggregation_Type_Enum.Home;
				Mode.Home_Type = Home_Type_Enum.List;

				// If some instance-wide pre-menu items existed, don't use the home image
				if (pre_menu_options_exist)
				{
					if (isOnHome)
					{
						Output.WriteLine("\t\t<li id=\"sbkAgm_Home\" class=\"selected-sf-menu-item-link\"><a href=\"" + Mode.Redirect_URL() + "\">" + home + "</a><ul id=\"sbkAgm_HomeSubMenu\">");
					}
					else
					{
						Output.WriteLine("\t\t<li id=\"sbkAgm_Home\"><a href=\"" + Mode.Redirect_URL() + "\">" + home + "</a><ul id=\"sbkAgm_HomeSubMenu\">");
					}
				}
				else
				{
					if (isOnHome)
					{
						Output.WriteLine("\t\t<li id=\"sbkAgm_Home\" class=\"selected-sf-menu-item-link sbkMenu_Home\"><a href=\"" + Mode.Redirect_URL() + "\" class=\"sbkMenu_NoPadding\"><img src=\"" + Mode.Default_Images_URL + "home.png\" /> <div class=\"sbkMenu_HomeText\">" + home + "</div></a><ul id=\"sbkAgm_HomeSubMenu\">");
					}
					else
					{
						Output.WriteLine("\t\t<li id=\"sbkAgm_Home\" class=\"sbkMenu_Home\"><a href=\"" + Mode.Redirect_URL() + "\" class=\"sbkMenu_NoPadding\"><img src=\"" + Mode.Default_Images_URL + "home.png\" /> <div class=\"sbkMenu_HomeText\">" + home + "</div></a><ul id=\"sbkAgm_HomeSubMenu\">");
					}
				}

				Output.WriteLine("\t\t\t<li id=\"sbkAgm_AggrHome\"><a href=\"" + Mode.Redirect_URL() + "\">" + collection_home + "</a></li>");

				Mode.Aggregation = String.Empty;
				if (Mode.Default_Aggregation != "all")
				{
					Output.WriteLine("\t\t\t<li id=\"sbkAgm_InstanceHome\"><a href=\"" + Mode.Redirect_URL() + "\">" + sobek_home_text + "</a></li>");
				}
				else
				{
					Output.WriteLine("\t\t\t<li id=\"sbkAgm_InstanceHome\"><a href=\"" + Mode.Redirect_URL() + "\">" + sobek_home_text + "</a><ul id=\"sbkAgm_InstanceHomeSubMenu\">");
					Output.WriteLine("\t\t\t\t<li id=\"sbkAgm_HomeListView\"><a href=\"" + Mode.Redirect_URL() + "\">" + list_view_text + "</a></li>");
					Mode.Home_Type = Home_Type_Enum.Descriptions;
					Output.WriteLine("\t\t\t\t<li id=\"sbkAgm_HomeBriefView\"><a href=\"" + Mode.Redirect_URL() + "\">" + brief_view_text + "</a></li>");
					if (SobekCM_Library_Settings.Include_TreeView_On_System_Home)
					{
						Mode.Home_Type = Home_Type_Enum.Tree_Collapsed;
						Output.WriteLine("\t\t\t\t<li id=\"sbkAgm_HomeTreeView\"><a href=\"" + Mode.Redirect_URL() + "\">" + tree_view_text + "</a></li>");
					}
					if ((User != null) && (User.LoggedOn))
					{
						Mode.Home_Type = Home_Type_Enum.Personalized;
						Output.WriteLine("\t\t\t\t<li id=\"sbkAgm_HomePersonalized\"><a href=\"" + Mode.Redirect_URL() + "\">" + myCollections + "</a></li>");
					}
					if (SobekCM_Library_Settings.Include_Partners_On_System_Home)
					{
						Mode.Home_Type = Home_Type_Enum.Partners_List;
						Output.WriteLine("\t\t\t\t<li id=\"sbkAgm_HomePartners\"><a href=\"" + Mode.Redirect_URL() + "\">" + partners_text + "</a></li>");
					}
					Output.WriteLine("\t\t\t</ul></li>");
				}

				Output.WriteLine("\t\t</ul></li>");

				Mode.Aggregation = Current_Aggregation.Code;
			}

			// Add any additional search types
			Mode.Mode = thisMode;
			for (int i = 1; i < Current_Aggregation.Views_And_Searches.Count; i++)
			{
				Output.Write(Aggregation_Nav_Bar_HTML_Factory.Menu_Get_Nav_Bar_HTML(Current_Aggregation.Views_And_Searches[i], Mode, Translations));
			}

			// Replace any search string
			Mode.Search_String = current_search;

			// Check for the existence of any BROWSE BY pages
			if (Current_Aggregation.Has_Browse_By_Pages)
			{
				Mode.Mode = Display_Mode_Enum.Aggregation;
				Mode.Aggregation_Type = Aggregation_Type_Enum.Browse_By;
				Mode.Info_Browse_Mode = String.Empty;

				// Get sorted collection of (public) browse bys linked to this aggregation
				ReadOnlyCollection<Item_Aggregation_Child_Page> public_browses = Current_Aggregation.Browse_By_Pages(Mode.Language);
				if (public_browses.Count > 0)
				{
					if (((thisMode == Display_Mode_Enum.Aggregation) && (thisAggrType == Aggregation_Type_Enum.Browse_By)) || (Mode.Is_Robot))
					{
						Output.WriteLine("\t\t<li id=\"sbkAgm_BrowseBy\" class=\"selected-sf-menu-item-link\"><a href=\"" + Mode.Redirect_URL() + "\">" + browseBy + "</a><ul id=\"sbkAgm_BrowseBySubMenu\">");
					}
					else
					{
						Output.WriteLine("\t\t<li id=\"sbkAgm_BrowseBy\"><a href=\"" + Mode.Redirect_URL() + "\">" + browseBy + "</a><ul id=\"sbkAgm_BrowseBySubMenu\">");
					}

					foreach (Item_Aggregation_Child_Page thisBrowse in public_browses)
					{
						// Static HTML or metadata browse by?
						if (thisBrowse.Source == Item_Aggregation_Child_Page.Source_Type.Static_HTML)
						{
							Mode.Info_Browse_Mode = thisBrowse.Code;
							Output.WriteLine("\t\t\t<li><a href=\"" + Mode.Redirect_URL().Replace("&", "&amp") + "\">" + thisBrowse.Get_Label(Mode.Language) + "</a></li>");
						}
						else
						{
							Metadata_Search_Field facetField = SobekCM_Library_Settings.Metadata_Search_Field_By_Facet_Name(thisBrowse.Code);
							if (facetField != null)
							{
								Mode.Info_Browse_Mode = thisBrowse.Code.ToLower().Replace(" ", "_");
								Output.WriteLine("\t\t\t<li><a href=\"" + Mode.Redirect_URL().Replace("&", "&amp") + "\">" + facetField.Facet_Term + "</a></li>");
							}
						}
					}

					Output.WriteLine("\t\t</ul></li>");
				}
			}

			// Check for the existence of any MAP BROWSE pages
			if (Current_Aggregation.Views_And_Searches.Contains(Item_Aggregation.CollectionViewsAndSearchesEnum.Map_Browse))
			{
				Mode.Mode = Display_Mode_Enum.Aggregation;
				Mode.Aggregation_Type = Aggregation_Type_Enum.Browse_Map;
				Mode.Info_Browse_Mode = String.Empty;

				if ((thisMode == Display_Mode_Enum.Aggregation) && (thisAggrType == Aggregation_Type_Enum.Browse_Map))
				{
					Output.WriteLine("\t\t<li id=\"sbkAgm_MapBrowse\" class=\"selected-sf-menu-item-link\"><a href=\"" + Mode.Redirect_URL() + "\">" + BROWSE_MAP + "</a></li>");
				}
				else
				{
					Output.WriteLine("\t\t<li id=\"sbkAgm_MapBrowse\"><a href=\"" + Mode.Redirect_URL() + "\">" + BROWSE_MAP + "</a></li>");
				}
			}

			// Add all the browses and child pages
			Mode.Mode = Display_Mode_Enum.Aggregation;
			Mode.Aggregation_Type = Aggregation_Type_Enum.Browse_Info;

			// Find the URL for all these browses
			Mode.Info_Browse_Mode = "XYXYXYXYXY";
			string redirect_url = Mode.Redirect_URL();
			Mode.Page = 1;

			// Only show ALL and NEW if they are in the collection list of searches and views
			int included_browses = 0;
			if (Current_Aggregation.Views_And_Searches.Contains(Item_Aggregation.CollectionViewsAndSearchesEnum.All_New_Items))
			{
				// First, look for 'ALL'
				if (Current_Aggregation.Contains_Browse_Info("all"))
				{
					bool includeNew = ((Current_Aggregation.Contains_Browse_Info("new")) && (!Mode.Is_Robot));
					if (includeNew)
					{
						if ((browse_code == "all") || (browse_code == "new" ))
						{
							Output.WriteLine("\t\t<li id=\"sbkAgm_ViewItems\" class=\"selected-sf-menu-item-link\"><a href=\"" + redirect_url.Replace("XYXYXYXYXY", "all").Replace("/info/", "/") + "\">" + viewItems + "</a><ul id=\"sbkAgm_ViewItemsSubMenu\">");
						}
						else
						{
							Output.WriteLine("\t\t<li id=\"sbkAgm_ViewItems\"><a href=\"" + redirect_url.Replace("XYXYXYXYXY", "all").Replace("/info/", "/") + "\">" + viewItems + "</a><ul id=\"sbkAgm_ViewItemsSubMenu\">");
						}

						Output.WriteLine("\t\t<li id=\"sbkAgm_AllBrowse\"><a href=\"" + redirect_url.Replace("XYXYXYXYXY", "all").Replace("/info/", "/") + "\">" + allItems + "</a></li>");
						Output.WriteLine("\t\t<li id=\"sbkAgm_NewBrowse\"><a href=\"" + redirect_url.Replace("XYXYXYXYXY", "new").Replace("/info/", "/") + "\">" + newItems + "</a></li>");

						Output.WriteLine("\t\t</ul></li>");
					}
					else
					{
						if (browse_code == "all") 
						{
							Output.WriteLine("\t\t<li id=\"sbkAgm_ViewItems\" class=\"selected-sf-menu-item-link\"><a href=\"" + redirect_url.Replace("XYXYXYXYXY", "all").Replace("/info/", "/") + "\">" + viewItems + "</a></li>");
						}
						else
						{
							Output.WriteLine("\t\t<li id=\"sbkAgm_ViewItems\"><a href=\"" + redirect_url.Replace("XYXYXYXYXY", "all").Replace("/info/", "/") + "\">" + viewItems + "</a></li>");
						}
					}

					included_browses++;
				}
			}

			Mode.Result_Display_Type = Result_Display_Type_Enum.NONE;
			redirect_url = Mode.Redirect_URL();

			// Are there any additional browses to include?
			ReadOnlyCollection<Item_Aggregation_Child_Page> otherBrowses = Current_Aggregation.Browse_Home_Pages(Mode.Language);
			if (otherBrowses.Count > included_browses)
			{
				// Now, step through the sorted list
				foreach (Item_Aggregation_Child_Page thisBrowseObj in otherBrowses.Where(thisBrowseObj => (thisBrowseObj.Code != "all") && (thisBrowseObj.Code != "new")))
				{
					Mode.Info_Browse_Mode = thisBrowseObj.Code;
					if (browse_code == thisBrowseObj.Code)
					{
						Output.WriteLine("\t\t<li id=\"sbkAgm_NewBrowse\" class=\"selected-sf-menu-item-link\"><a href=\"" + redirect_url.Replace("XYXYXYXYXY", thisBrowseObj.Code) + "\">" + thisBrowseObj.Get_Label(Mode.Language) + "</a></li>");
					}
					else
					{
						Output.WriteLine("\t\t<li id=\"sbkAgm_NewBrowse\"><a href=\"" + redirect_url.Replace("XYXYXYXYXY", thisBrowseObj.Code) + "\">" + thisBrowseObj.Get_Label(Mode.Language) + "</a></li>");
					}
				}
			}

			// If this is NOT the all collection, then show subcollections
			if ((Current_Aggregation.Code != "all") && (Current_Aggregation.Children_Count > 0))
			{
				// Verify some of the children are active and not hidden
				// Keep the last aggregation alias
				string lastAlias = Mode.Aggregation_Alias;
				Mode.Aggregation_Alias = String.Empty;
				Mode.Info_Browse_Mode = String.Empty;

				// Collect the html to write (this alphabetizes the children)
				List<string> html_list = new List<string>();
				foreach (Item_Aggregation_Related_Aggregations childAggr in Current_Aggregation.Children)
				{
					Item_Aggregation_Related_Aggregations latest = Code_Manager[childAggr.Code];
					if ((latest != null) && (!latest.Hidden) && (latest.Active))
					{
						string name = childAggr.ShortName;
						if (name.ToUpper() == "ADDED AUTOMATICALLY")
							name = childAggr.Code + " ( Added Automatically )";

						Mode.Aggregation = childAggr.Code.ToLower();
						html_list.Add("\t\t\t<li><a href=\"" + Mode.Redirect_URL() + "\">" + Translations.Get_Translation(name, Mode.Language) + "</a></li>");
					}
				}

				if (html_list.Count > 0)
				{
					string childTypes = Current_Aggregation.Child_Types.Trim();
					Output.WriteLine("\t\t<li id=\"sbkAgm_SubCollections\"><a href=\"#subcolls\">" + Translations.Get_Translation(childTypes, Mode.Language) + "</a><ul id=\"sbkAgm_SubCollectionsMenu\">");
					foreach (string thisHtml in html_list)
					{
						Output.WriteLine(thisHtml);
					}
					Output.WriteLine("\t\t</ul></li>");

					// Restore the old alias
					Mode.Aggregation_Alias = lastAlias;
				}
			}

			// If there is a user and this is the main home page, show MY COLLECTIONS
			if ((User != null) && ( User.LoggedOn ))
			{
				if (Current_Aggregation.Code == "all")
				{
					Mode.Mode = Display_Mode_Enum.Aggregation;
					Mode.Aggregation_Type = Aggregation_Type_Enum.Home;
					Mode.Home_Type = Home_Type_Enum.Personalized;

					// Show personalized
					if (thisHomeType == Home_Type_Enum.Personalized)
					{
						Output.WriteLine("\t\t<li id=\"sbkAgm_MyCollections\" class=\"selected-sf-menu-item-link\"><a href=\"" + Mode.Redirect_URL() + "\">" + myCollections + "</a></li>");
					}
					else
					{
						Output.WriteLine("\t\t<li id=\"sbkAgm_MyCollections\"><a href=\"" + Mode.Redirect_URL() + "\">" + myCollections + "</a></li>");
					}
				}
				else
				{
					if (User.Is_Aggregation_Admin(Current_Aggregation.Code))
					{
						// Return the code and mode back
						Mode.Info_Browse_Mode = String.Empty;
						Mode.Search_Type = thisSearch;
						Mode.Mode = thisMode;
						Mode.Home_Type = thisHomeType;

						Output.Write(Aggregation_Nav_Bar_HTML_Factory.Menu_Get_Nav_Bar_HTML(Item_Aggregation.CollectionViewsAndSearchesEnum.Admin_View, Mode, Translations));
					}
				}
			}

			// Show institutional lists?
			if (Current_Aggregation.Code == "all")
			{
				// Is this library set to show the partners tab?
				if (SobekCM_Library_Settings.Include_Partners_On_System_Home)
				{
					Mode.Mode = Display_Mode_Enum.Aggregation;
					Mode.Aggregation_Type = Aggregation_Type_Enum.Home;
					Mode.Home_Type = Home_Type_Enum.Partners_List;

					if (((thisHomeType == Home_Type_Enum.Partners_List) || (thisHomeType == Home_Type_Enum.Partners_Thumbnails)))
					{
						Output.WriteLine("\t\t<li id=\"sbkAgm_Partners\" class=\"selected-sf-menu-item-link\"><a href=\"" + Mode.Redirect_URL() + "\">" + partners + "</a></li>");
					}
					else
					{
						Output.WriteLine("\t\t<li id=\"sbkAgm_Partners\"><a href=\"" + Mode.Redirect_URL() + "\">" + partners + "</a></li>");
					}
				}
			}

			// Return the code and mode back
			Mode.Info_Browse_Mode = browse_code;
			Mode.Aggregation_Type = thisAggrType;
			Mode.Search_Type = thisSearch;
			Mode.Mode = thisMode;
			Mode.Home_Type = thisHomeType;
			Mode.Result_Display_Type = resultsType;
			Mode.Aggregation = aggregation;
			Mode.Page = page;

			Output.WriteLine("\t</ul></div>");
			Output.WriteLine();

			// Add the scripts needed
			Output.WriteLine("<!-- Add references to the superfish and hoverintent libraries for the main user menu -->");
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
		/// <param name="Mode"> Mode / navigation information for the current request</param>
		/// <param name="User"> Currently logged on user (or object representing the unlogged on user's preferences) </param>
        public static void Add_UserSpecific_Main_Menu(TextWriter Output, SobekCM_Navigation_Object Mode, User_Object User)
        {
            // Add the item views
            Output.WriteLine("<!-- Add the main user-specific menu -->");
			Output.WriteLine("<div id=\"sbkUsm_MenuBar\" class=\"sbkMenu_Bar\">");
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
			const string myCollections = "My Collections";
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
            Mode.Mode = Display_Mode_Enum.Aggregation;
	        Mode.Aggregation_Type = Aggregation_Type_Enum.Home;
            Mode.Home_Type = Home_Type_Enum.List;
			Output.WriteLine("\t\t<li id=\"sbkUsm_Home\" class=\"sbkMenu_Home\"><a href=\"" + Mode.Redirect_URL() + "\" class=\"sbkMenu_NoPadding\"><img src=\"" + Mode.Default_Images_URL + "home.png\" /> <div class=\"sbkMenu_HomeText\">" + sobek_home_text + "</div></a><ul id=\"sbkUsm_HomeSubMenu\">");
            Output.WriteLine("\t\t\t<li id=\"sbkUsm_HomeListView\"><a href=\"" + Mode.Redirect_URL() + "\">" + list_view_text + "</a></li>");
            Mode.Home_Type = Home_Type_Enum.Descriptions;
            Output.WriteLine("\t\t\t<li id=\"sbkUsm_HomeBriefView\"><a href=\"" + Mode.Redirect_URL() + "\">" + brief_view_text + "</a></li>");
            if (SobekCM_Library_Settings.Include_TreeView_On_System_Home)
            {
                Mode.Home_Type = Home_Type_Enum.Tree_Collapsed;
                Output.WriteLine("\t\t\t<li id=\"sbkUsm_HomeTreeView\"><a href=\"" + Mode.Redirect_URL() + "\">" + tree_view_text + "</a></li>");
            }
			if (User != null)
			{
				Mode.Home_Type = Home_Type_Enum.Personalized;
				Output.WriteLine("\t\t\t<li id=\"sbkUsm_HomePersonalized\"><a href=\"" + Mode.Redirect_URL() + "\">" + myCollections + "</a></li>");
			}
            if (SobekCM_Library_Settings.Include_Partners_On_System_Home)
            {
                Mode.Home_Type = Home_Type_Enum.Partners_List;
                Output.WriteLine("\t\t\t<li id=\"sbkUsm_HomePartners\"><a href=\"" + Mode.Redirect_URL() + "\">" + partners_text + "</a></li>");
            }
            string advanced_url = Mode.Base_URL + "/advanced";
            Output.WriteLine("\t\t\t<li id=\"sbkUsm_HomeAdvSearch\"><a href=\"" + advanced_url + "\">" + advanced_search_text + "</a></li>");
            Output.WriteLine("\t\t</ul></li>");

	        if (User != null)
	        {
		        // Add the 'mySOBEK HOME' second menu option and suboptions
		        Mode.Mode = Display_Mode_Enum.My_Sobek;
		        Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
		        Output.WriteLine("\t\t<li><a href=\"" + Mode.Redirect_URL() + "\">" + my_sobek_home_text + "</a><ul id=\"sbkUsm_MySubMenu\">");

		        // If a user can submit, add a link to start a new item
		        if ((User.Can_Submit) && (SobekCM_Library_Settings.Online_Edit_Submit_Enabled))
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
		        Output.WriteLine("\t\t<li id=\"sbkUsm_Bookshelf\"><a href=\"" + Mode.Redirect_URL() + "\">" + myLibrary + "</a></li>");

		        // Add a link to my account (repeat of option in mySobek menu)
		        Mode.My_Sobek_Type = My_Sobek_Type_Enum.Preferences;
		        Output.WriteLine("\t\t\t<li id=\"sbkUsm_Account\"><a href=\"" + Mode.Redirect_URL() + "\">" + myPreferences + "</a></li>");

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
			        //    CurrentMode.Mode = Display_Mode_Enum.Internal;
			        //    Output.WriteLine("  <a href=\"" + CurrentMode.Redirect_URL() + "\">" + Selected_Tab_Start + internalTab + Selected_Tab_End + "</a>");
			        //    CurrentMode.Mode = Display_Mode_Enum.My_Sobek;
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

			        // Edit forwarding
			        Mode.Admin_Type = Admin_Type_Enum.Aliases;
			        Output.WriteLine("\t\t\t<li id=\"sbkUsm_AdminForwarding\"><a href=\"" + Mode.Redirect_URL() + "\"><img src=\"" + Mode.Default_Images_URL + "forwarding.png\" /> <div class=\"sbkUsm_TextWithImage\">Aggregation Aliases</div></a></li>");

					// Edit item aggregations
					Mode.Admin_Type = Admin_Type_Enum.Aggregations_Mgmt;
					Output.WriteLine("\t\t\t<li id=\"sbkUsm_AdminAggr\"><a href=\"" + Mode.Redirect_URL() + "\"><img src=\"" + Mode.Default_Images_URL + "building.gif\" /> <div class=\"sbkUsm_TextWithImage\">Aggregation Management</div></a></li>");


			        // View and set SobekCM Builder Status
			        Mode.Admin_Type = Admin_Type_Enum.Builder_Status;
			        Output.WriteLine("\t\t\t<li id=\"sbkUsm_AdminStatus\"><a href=\"" + Mode.Redirect_URL() + "\"><img src=\"" + Mode.Default_Images_URL + "gears.png\" /> <div class=\"sbkUsm_TextWithImage\">Builder Status</div></a></li>");

			        // Edit Default_Metadata
			        Mode.Admin_Type = Admin_Type_Enum.Default_Metadata;
			        Output.WriteLine("\t\t\t<li id=\"sbkUsm_AdminProjects\"><a href=\"" + Mode.Redirect_URL() + "\"><img src=\"" + Mode.Default_Images_URL + "pmets.gif\" /> <div class=\"sbkUsm_TextWithImage\">Default Metadata</div></a></li>");

			        // Edit IP Restrictions
			        Mode.Admin_Type = Admin_Type_Enum.IP_Restrictions;
			        Output.WriteLine("\t\t\t<li id=\"sbkUsm_AdminRestrictions\"><a href=\"" + Mode.Redirect_URL() + "\"><img src=\"" + Mode.Default_Images_URL + "firewall.png\" /> <div class=\"sbkUsm_TextWithImage\">IP Restriction Ranges</div></a></li>");

			        // Edit Settings
			        Mode.Admin_Type = Admin_Type_Enum.Settings;
			        Output.WriteLine("\t\t\t<li id=\"sbkUsm_AdminSettings\"><a href=\"" + Mode.Redirect_URL() + "\"><img src=\"" + Mode.Default_Images_URL + "wrench.png\" /> <div class=\"sbkUsm_TextWithImage\">System-Wide Settings</div></a></li>");

			        // Edit Thematic Headings
			        Mode.Admin_Type = Admin_Type_Enum.Thematic_Headings;
			        Output.WriteLine("\t\t\t<li id=\"sbkUsm_AdminThematic\"><a href=\"" + Mode.Redirect_URL() + "\"><div class=\"sbkUsm_TextNoImage\">Thematic Headings</div></a></li>");

			        // Edit URL Portals
			        Mode.Admin_Type = Admin_Type_Enum.URL_Portals;
			        Output.WriteLine("\t\t\t<li id=\"sbkUsm_AdminPortals\"><a href=\"" + Mode.Redirect_URL() + "\"><img src=\"" + Mode.Default_Images_URL + "portals.png\" /> <div class=\"sbkUsm_TextWithImage\">URL Portals</div></a></li>");

			        if (User.Is_System_Admin)
			        {
				        // Edit users
				        Mode.Admin_Type = Admin_Type_Enum.Users;
				        Output.WriteLine("\t\t\t<li id=\"sbkUsm_AdminUsers\"><a href=\"" + Mode.Redirect_URL() + "\"><img src=\"" + Mode.Default_Images_URL + "users.png\" /> <div class=\"sbkUsm_TextWithImage\">Users and Groups</div></a></li>");
			        }

			        // Edit interfaces
			        Mode.Admin_Type = Admin_Type_Enum.Skins;
			        Output.WriteLine("\t\t\t<li id=\"sbkUsm_AdminSkin\"><a href=\"" + Mode.Redirect_URL() + "\"><img src=\"" + Mode.Default_Images_URL + "skins.png\" /> <div class=\"sbkUsm_TextWithImage\">Web Skins</div></a></li>");

			        // Edit wordmarks
			        Mode.Admin_Type = Admin_Type_Enum.Wordmarks;
			        Output.WriteLine("\t\t\t<li id=\"sbkUsm_AdminWordmarks\"><a href=\"" + Mode.Redirect_URL() + "\"><img src=\"" + Mode.Default_Images_URL + "wordmarks.gif\" /> <div class=\"sbkUsm_TextWithImage\">Wordmarks / Icons</div></a></li>");

			        // Reset cache
			        Mode.Admin_Type = Admin_Type_Enum.Reset;
			        Output.WriteLine("\t\t\t<li id=\"sbkUsm_AdminReset\"><a href=\"" + Mode.Redirect_URL() + "\"><img src=\"" + Mode.Default_Images_URL + "refresh.png\" /> <div class=\"sbkUsm_TextWithImage\">Reset Cache</div></a></li>");

			        Output.WriteLine("\t\t</ul></li>");
		        }
	        }


	        Output.WriteLine("\t</ul></div>");
            Output.WriteLine();

            Output.WriteLine("<!-- Initialize the main user menu -->");
            Output.WriteLine("<script>");
            Output.WriteLine("  jQuery(document).ready(function () {");
            Output.WriteLine("     jQuery('ul.sf-menu').superfish();");
            Output.WriteLine("  });");
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
