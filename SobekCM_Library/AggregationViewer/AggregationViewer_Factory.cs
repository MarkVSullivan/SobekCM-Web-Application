#region Using directives

using SobekCM.Library.AggregationViewer.Viewers;
using SobekCM.Library.Aggregations;
using SobekCM.Library.HTML;
using SobekCM.Library.Navigation;
using SobekCM.Library.Users;

#endregion

namespace SobekCM.Library.AggregationViewer
{    
    /// <summary> Factory class that generate and returns the requested view for an item aggregation </summary>
    /// <remarks> This is used by <see cref="Aggregation_HtmlSubwriter"/> class </remarks>
    public class AggregationViewer_Factory
    {
        /// <summary> Returns a built collection viewer matching request </summary>
        /// <param name="ViewType"> Aggregation view type </param>
        /// <param name="Current_Aggregation"> Current item aggregation object </param>
        /// <param name="Current_Mode"> Mode / navigation information for the current request </param>
         /// <returns> Collection viewer that extends the <see cref="abstractAggregationViewer"/> class. </returns>
        public static abstractAggregationViewer Get_Viewer(Item_Aggregation.CollectionViewsAndSearchesEnum ViewType, Item_Aggregation Current_Aggregation, SobekCM_Navigation_Object Current_Mode )
        {
            switch (ViewType)
            {
                case Item_Aggregation.CollectionViewsAndSearchesEnum.Advanced_Search:
                    return new Advanced_Search_AggregationViewer(Current_Aggregation, Current_Mode);

                case Item_Aggregation.CollectionViewsAndSearchesEnum.Basic_Search:
                    string frontBannerImage = Current_Aggregation.Front_Banner_Image(Current_Mode.Language);
                    if ((frontBannerImage.Length > 0) && (Current_Aggregation.Highlights.Count > 0))
                    {
                        return new Rotating_Highlight_Search_AggregationViewer(Current_Aggregation, Current_Mode);
                    }
                    return new Basic_Search_AggregationViewer(Current_Aggregation, Current_Mode);

                case Item_Aggregation.CollectionViewsAndSearchesEnum.FullText_Search:
                    return new Full_Text_Search_AggregationViewer(Current_Aggregation, Current_Mode );

                case Item_Aggregation.CollectionViewsAndSearchesEnum.No_Home_Search:
                    return new No_Search_AggregationViewer();

                case Item_Aggregation.CollectionViewsAndSearchesEnum.Newspaper_Search:
                    return new Newspaper_Search_AggregationViewer(Current_Aggregation, Current_Mode);

                case Item_Aggregation.CollectionViewsAndSearchesEnum.Map_Search:
                    return new Map_Search_AggregationViewer(Current_Aggregation, Current_Mode);

                case Item_Aggregation.CollectionViewsAndSearchesEnum.dLOC_FullText_Search:
                    return new dLOC_Search_AggregationViewer(Current_Aggregation, Current_Mode);

                default:
                    return null;
            }
        }

        /// <summary> Returns a built collection viewer matching request </summary>
        /// <param name="SearchType"> Type of search from the current http request </param>
        /// <param name="Current_Aggregation"> Current item aggregation object </param>
        /// <param name="Current_Mode"> Mode / navigation information for the current request </param>
        /// <param name="Current_User"> Currently logged on user, if there is one </param>
        /// <returns> Collection viewer that extends the <see cref="abstractAggregationViewer"/> class. </returns>
        public static abstractAggregationViewer Get_Viewer(Search_Type_Enum SearchType, Item_Aggregation Current_Aggregation, SobekCM_Navigation_Object Current_Mode, User_Object Current_User)
        {
            switch (SearchType)
            {
                case Search_Type_Enum.Advanced:
                    return new Advanced_Search_AggregationViewer(Current_Aggregation, Current_Mode);

                case Search_Type_Enum.Basic:
                    if ((Current_Aggregation.Front_Banner_Image(Current_Mode.Language ).Length > 0) && (Current_Aggregation.Highlights.Count > 0))
                    {
                        return new Rotating_Highlight_Search_AggregationViewer(Current_Aggregation, Current_Mode);
                    }
                    return new Basic_Search_AggregationViewer(Current_Aggregation, Current_Mode);

                case Search_Type_Enum.Full_Text:
                    return new Full_Text_Search_AggregationViewer(Current_Aggregation, Current_Mode);

                case Search_Type_Enum.Newspaper:
                    return new Newspaper_Search_AggregationViewer(Current_Aggregation, Current_Mode);

                case Search_Type_Enum.Map:
                    return new Map_Search_AggregationViewer(Current_Aggregation, Current_Mode);

                case Search_Type_Enum.dLOC_Full_Text:
                    return new dLOC_Search_AggregationViewer(Current_Aggregation, Current_Mode);

                default:
                    return null;
            }
        }
    }
}
