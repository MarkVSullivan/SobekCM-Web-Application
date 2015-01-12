#region Using directives

using SobekCM.Core.Aggregations;
using SobekCM.Core.Navigation;
using SobekCM.Library.AggregationViewer.Viewers;
using SobekCM.Library.HTML;

#endregion

namespace SobekCM.Library.AggregationViewer
{    
    /// <summary> Factory class that generate and returns the requested view for an item aggregation </summary>
    /// <remarks> This is used by <see cref="Aggregation_HtmlSubwriter"/> class </remarks>
    public class AggregationViewer_Factory
    {
        /// <summary> Returns a built collection viewer matching request </summary>
        /// <param name="ViewType"> Aggregation view type </param>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
         /// <returns> Collection viewer that extends the <see cref="abstractAggregationViewer"/> class. </returns>
        public static abstractAggregationViewer Get_Viewer(Item_Aggregation.CollectionViewsAndSearchesEnum ViewType, RequestCache RequestSpecificValues )
        {
            switch (ViewType)
            {
                case Item_Aggregation.CollectionViewsAndSearchesEnum.Advanced_Search:
                    return new Advanced_Search_AggregationViewer(RequestSpecificValues);

                case Item_Aggregation.CollectionViewsAndSearchesEnum.Advanced_Search_MimeType:
                    return new Advanced_Search_MimeType_AggregationViewer(RequestSpecificValues);

				case Item_Aggregation.CollectionViewsAndSearchesEnum.Advanced_Search_YearRange:
                    return new Advanced_Search_YearRange_AggregationViewer(RequestSpecificValues);


                case Item_Aggregation.CollectionViewsAndSearchesEnum.Basic_Search:
                    Item_Aggregation_Front_Banner frontBannerImage = RequestSpecificValues.Hierarchy_Object.Front_Banner_Image(RequestSpecificValues.Current_Mode.Language);
                    if ((frontBannerImage != null) && (RequestSpecificValues.Hierarchy_Object.Highlights != null ) && (RequestSpecificValues.Hierarchy_Object.Highlights.Count > 0))
                    {
                        return new Rotating_Highlight_Search_AggregationViewer(RequestSpecificValues);
                    }
                    return new Basic_Search_AggregationViewer(RequestSpecificValues);

				case Item_Aggregation.CollectionViewsAndSearchesEnum.Basic_Search_YearRange:
                    return new Basic_Search_YearRange_AggregationViewer(RequestSpecificValues);

                case Item_Aggregation.CollectionViewsAndSearchesEnum.Basic_Search_MimeType:
                    Item_Aggregation_Front_Banner frontBannerImage2 = RequestSpecificValues.Hierarchy_Object.Front_Banner_Image(RequestSpecificValues.Current_Mode.Language);
                    if ((frontBannerImage2 != null) && (RequestSpecificValues.Hierarchy_Object.Highlights != null) && (RequestSpecificValues.Hierarchy_Object.Highlights.Count > 0))
                    {
                        return new Rotating_Highlight_MimeType_AggregationViewer(RequestSpecificValues);
                    }
                    return new Basic_Search_MimeType_AggregationViewer(RequestSpecificValues);

                case Item_Aggregation.CollectionViewsAndSearchesEnum.FullText_Search:
                    return new Full_Text_Search_AggregationViewer(RequestSpecificValues);

                case Item_Aggregation.CollectionViewsAndSearchesEnum.No_Home_Search:
                    return new No_Search_AggregationViewer(RequestSpecificValues);

                case Item_Aggregation.CollectionViewsAndSearchesEnum.Newspaper_Search:
                    return new Newspaper_Search_AggregationViewer(RequestSpecificValues);

                case Item_Aggregation.CollectionViewsAndSearchesEnum.Map_Search:
                    return new Map_Search_AggregationViewer(RequestSpecificValues);

                case Item_Aggregation.CollectionViewsAndSearchesEnum.Map_Search_Beta:
                    return new Map_Search_AggregationViewer_Beta(RequestSpecificValues);

                case Item_Aggregation.CollectionViewsAndSearchesEnum.DLOC_FullText_Search:
                    return new dLOC_Search_AggregationViewer(RequestSpecificValues);

                default:
                    return null;
            }
        }

        /// <summary> Returns a built collection viewer matching request </summary>
        /// <param name="SearchType"> Type of search from the current http request </param>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        /// <returns> Collection viewer that extends the <see cref="abstractAggregationViewer"/> class. </returns>
        public static abstractAggregationViewer Get_Viewer(Search_Type_Enum SearchType, RequestCache RequestSpecificValues )
        {
            switch (SearchType)
            {
                case Search_Type_Enum.Advanced:
                    if (RequestSpecificValues.Hierarchy_Object.Views_And_Searches.Contains(Item_Aggregation.CollectionViewsAndSearchesEnum.Advanced_Search_YearRange))
                        return new Advanced_Search_YearRange_AggregationViewer(RequestSpecificValues);
                    if (RequestSpecificValues.Hierarchy_Object.Views_And_Searches.Contains(Item_Aggregation.CollectionViewsAndSearchesEnum.Advanced_Search_MimeType))
                        return new Advanced_Search_MimeType_AggregationViewer(RequestSpecificValues);
                    return new Advanced_Search_AggregationViewer(RequestSpecificValues);

                case Search_Type_Enum.Basic:
                    Item_Aggregation_Front_Banner frontBannerImage = RequestSpecificValues.Hierarchy_Object.Front_Banner_Image(RequestSpecificValues.Current_Mode.Language);
                    if ((frontBannerImage != null) && (RequestSpecificValues.Hierarchy_Object.Highlights != null) && (RequestSpecificValues.Hierarchy_Object.Highlights.Count > 0))
                    {
                        return new Rotating_Highlight_Search_AggregationViewer(RequestSpecificValues);
                    }
                    return new Basic_Search_AggregationViewer(RequestSpecificValues);

                case Search_Type_Enum.Full_Text:
                    return new Full_Text_Search_AggregationViewer(RequestSpecificValues);

                case Search_Type_Enum.Newspaper:
                    return new Newspaper_Search_AggregationViewer(RequestSpecificValues);

                case Search_Type_Enum.Map:
                    return new Map_Search_AggregationViewer(RequestSpecificValues);

                case Search_Type_Enum.Map_Beta:
                    return new Map_Search_AggregationViewer_Beta(RequestSpecificValues);

                case Search_Type_Enum.dLOC_Full_Text:
                    return new dLOC_Search_AggregationViewer(RequestSpecificValues);

                default:
                    return null;
            }
        }
    }
}
