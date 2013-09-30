#region Using directives

using System;
using SobekCM.Library.Aggregations;
using SobekCM.Library.Application_State;
using SobekCM.Library.HTML;
using SobekCM.Library.Navigation;

#endregion

namespace SobekCM.Library.AggregationViewer
{
    /// <summary> HTML factory class that helps to render the HTML for the navigational tabs which correspond to different collection views into the same item aggregation </summary>
    /// <remarks> This is used by <see cref="Aggregation_HtmlSubwriter"/> class </remarks>
    public class Aggregation_Nav_Bar_HTML_Factory
    {
        /// <summary> Compares the collection view type and the search type from the current http request </summary>
        /// <param name="SearchType1"> Search type from the list of collection views and searches </param>
        /// <param name="SearchType2"> Search type from the current http request </param>
        /// <returns> TRUE if they are analagous, otherwise FALSE </returns>
        public static bool Do_Search_Types_Match(Item_Aggregation.CollectionViewsAndSearchesEnum SearchType1, Search_Type_Enum SearchType2)
        {
            switch (SearchType1)
            {
                case Item_Aggregation.CollectionViewsAndSearchesEnum.Advanced_Search:
                    return SearchType2 == Search_Type_Enum.Advanced;

				case Item_Aggregation.CollectionViewsAndSearchesEnum.Advanced_Search_YearRange:
					return SearchType2 == Search_Type_Enum.Advanced;

                case Item_Aggregation.CollectionViewsAndSearchesEnum.Basic_Search:
                    return SearchType2 == Search_Type_Enum.Basic;

				case Item_Aggregation.CollectionViewsAndSearchesEnum.Basic_Search_YearRange:
					return SearchType2 == Search_Type_Enum.Basic;

                case Item_Aggregation.CollectionViewsAndSearchesEnum.FullText_Search:
                    return SearchType2 == Search_Type_Enum.Full_Text;

                case Item_Aggregation.CollectionViewsAndSearchesEnum.Map_Search:
                    return SearchType2 == Search_Type_Enum.Map;

                case Item_Aggregation.CollectionViewsAndSearchesEnum.Newspaper_Search:
                    return SearchType2 == Search_Type_Enum.Newspaper;

                case Item_Aggregation.CollectionViewsAndSearchesEnum.dLOC_FullText_Search:
                    return SearchType2 == Search_Type_Enum.dLOC_Full_Text;

                default:
                    return false;
            }
        }

        /// <summary> Returns the HTML for one tab which appears over the search box in the collection view </summary>
        /// <param name="ThisView"> Collection view type for this tab </param>
        /// <param name="Current_Mode"> Mode / navigation information for the current request, to see if the tab is currently selected or not and determine current skin language </param>
        /// <param name="Translations"> Language support object for writing the name of the view in the appropriate interface language </param>
        /// <param name="Downward_Tabs"> Flag indicates if this tab faces downward, rather than the default upward </param>
        /// <returns> HTML to display the tab, including the link if it is not currently selected </returns>
        public static string Get_Nav_Bar_HTML( Item_Aggregation.CollectionViewsAndSearchesEnum ThisView, SobekCM_Navigation_Object Current_Mode, Language_Support_Info Translations, bool Downward_Tabs )
        {
            string skinCode = Current_Mode.Base_Skin;

            switch (ThisView)
            {
                case Item_Aggregation.CollectionViewsAndSearchesEnum.Advanced_Search:
				case Item_Aggregation.CollectionViewsAndSearchesEnum.Advanced_Search_YearRange:
                    if (Current_Mode.Is_Robot)
                    {
                        return "<img src=\"" + Current_Mode.Base_URL + "design/skins/" + skinCode + "/tabs/cL.gif\" border=\"0\" class=\"tab_image\" alt=\"\" /><span class=\"tab\"> ADVANCED SEARCH </span><img src=\"" + Current_Mode.Base_URL + "design/skins/" + skinCode + "/tabs/cR.gif\" border=\"0\" class=\"tab_image\" alt=\"\" />" + Environment.NewLine ;
                    }
                    return HTML_Helper(skinCode, Search_Type_Enum.Advanced, Translations.Get_Translation("ADVANCED SEARCH", Current_Mode.Language), Current_Mode, Downward_Tabs);

                case Item_Aggregation.CollectionViewsAndSearchesEnum.Basic_Search:
				case Item_Aggregation.CollectionViewsAndSearchesEnum.Basic_Search_YearRange:
                    return HTML_Helper(skinCode, Search_Type_Enum.Basic, Translations.Get_Translation("BASIC SEARCH", Current_Mode.Language), Current_Mode, Downward_Tabs);

                case Item_Aggregation.CollectionViewsAndSearchesEnum.Map_Search:
                    return HTML_Helper(skinCode, Search_Type_Enum.Map, Translations.Get_Translation("MAP SEARCH", Current_Mode.Language), Current_Mode, Downward_Tabs);

                case Item_Aggregation.CollectionViewsAndSearchesEnum.Newspaper_Search:
                    return HTML_Helper(skinCode, Search_Type_Enum.Newspaper, Translations.Get_Translation("NEWSPAPER SEARCH", Current_Mode.Language), Current_Mode, Downward_Tabs);

                case Item_Aggregation.CollectionViewsAndSearchesEnum.Admin_View:
                    return String.Empty; // HTML_Helper(Skin_Code, SobekCM.Library.Navigation.Search_Type_Enum.Admin_View, Translations.Get_Translation("ADMIN", Current_Mode.Language), Current_Mode, Downward_Tabs);

                case Item_Aggregation.CollectionViewsAndSearchesEnum.dLOC_FullText_Search:
                    return HTML_Helper(skinCode, Search_Type_Enum.dLOC_Full_Text, Translations.Get_Translation("TEXT SEARCH", Current_Mode.Language), Current_Mode, Downward_Tabs);

                case Item_Aggregation.CollectionViewsAndSearchesEnum.FullText_Search:
                    return HTML_Helper(skinCode, Search_Type_Enum.Full_Text, Translations.Get_Translation("TEXT SEARCH", Current_Mode.Language), Current_Mode, Downward_Tabs);
            }

            return String.Empty;
        }

        private static string HTML_Helper(string SkinCode, Search_Type_Enum Search_Type, string Display_Text, SobekCM_Navigation_Object Current_Mode, bool Downward_Tabs)
        {
            if (Downward_Tabs)
            {
                if ((Current_Mode.Mode == Display_Mode_Enum.Search) && (Current_Mode.Search_Type == Search_Type))
                {
                    return  "<img src=\"" + Current_Mode.Base_URL + "design/skins/" + SkinCode + "/tabs/cLD_s.gif\" border=\"0\" class=\"tab_image\" alt=\"\" /><span class=\"tab_s\"> " + Display_Text + " </span><img src=\"" + Current_Mode.Base_URL + "design/skins/" + SkinCode + "/tabs/cRD_s.gif\" border=\"0\" class=\"tab_image\" alt=\"\" />" + Environment.NewLine ;
                }
                Search_Type_Enum currentSearchType = Current_Mode.Search_Type;
                Display_Mode_Enum currentMode = Current_Mode.Mode;
                Current_Mode.Search_Type = Search_Type;
                Current_Mode.Mode = Display_Mode_Enum.Search;
                string toReturn = "<a href=\"" + Current_Mode.Redirect_URL() + "\"> " + "<img src=\"" + Current_Mode.Base_URL + "design/skins/" + SkinCode + "/tabs/cLD.gif\" border=\"0\" class=\"tab_image\" alt=\"\" /><span class=\"tab\"> "+ Display_Text + " </span><img src=\"" + Current_Mode.Base_URL + "design/skins/" + SkinCode + "/tabs/cRD.gif\" border=\"0\" class=\"tab_image\" alt=\"\" /></a>" + Environment.NewLine ;
                Current_Mode.Search_Type = currentSearchType;
                Current_Mode.Mode = currentMode;
                return toReturn;
            }

            if ((Current_Mode.Mode == Display_Mode_Enum.Search) && (Current_Mode.Search_Type == Search_Type))
            {
                return "<img src=\"" + Current_Mode.Base_URL + "design/skins/" + SkinCode + "/tabs/cL_s.gif\" border=\"0\" class=\"tab_image\" alt=\"\" /><span class=\"tab_s\"> " + Display_Text + " </span><img src=\"" + Current_Mode.Base_URL + "design/skins/" + SkinCode + "/tabs/cR_s.gif\" border=\"0\" class=\"tab_image\" alt=\"\" />" + Environment.NewLine ;
            }

			// else...
	        Search_Type_Enum currentSearchType2 = Current_Mode.Search_Type;
	        Display_Mode_Enum currentMode2 = Current_Mode.Mode;
	        Current_Mode.Search_Type = Search_Type;
	        Current_Mode.Mode = Display_Mode_Enum.Search;
	        string toReturn2 = "<a href=\"" + Current_Mode.Redirect_URL() + "\"> <img src=\"" + Current_Mode.Base_URL + "design/skins/" + SkinCode + "/tabs/cL.gif\" border=\"0\" class=\"tab_image\" alt=\"\" /><span class=\"tab\"> " + Display_Text + " </span><img src=\"" + Current_Mode.Base_URL + "design/skins/" + SkinCode + "/tabs/cR.gif\" border=\"0\" class=\"tab_image\" alt=\"\" /> </a>" + Environment.NewLine ;
	        Current_Mode.Search_Type = currentSearchType2;
	        Current_Mode.Mode = currentMode2;
	        return toReturn2;
        }
    }
}
