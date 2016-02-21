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
        /// <param name="ViewBag"> View bag holds specific data that was pulled for an aggregation request </param>
        /// <returns> Collection viewer that extends the <see cref="abstractAggregationViewer"/> class. </returns>
        public static abstractAggregationViewer Get_Viewer(Item_Aggregation_Views_Searches_Enum ViewType, RequestCache RequestSpecificValues, AggregationViewBag ViewBag)
        {
            switch (ViewType)
            {
                case Item_Aggregation_Views_Searches_Enum.Advanced_Search:
                    return new Advanced_Search_AggregationViewer(RequestSpecificValues, ViewBag);

                case Item_Aggregation_Views_Searches_Enum.Advanced_Search_MimeType:
                    return new Advanced_Search_MimeType_AggregationViewer(RequestSpecificValues, ViewBag);

				case Item_Aggregation_Views_Searches_Enum.Advanced_Search_YearRange:
                    return new Advanced_Search_YearRange_AggregationViewer(RequestSpecificValues, ViewBag);


                case Item_Aggregation_Views_Searches_Enum.Basic_Search:
                    Item_Aggregation_Front_Banner frontBannerImage = ViewBag.Hierarchy_Object.FrontBannerObj;
                    if ((frontBannerImage != null) && (ViewBag.Hierarchy_Object.Highlights != null ) && (ViewBag.Hierarchy_Object.Highlights.Count > 0))
                    {
                        return new Rotating_Highlight_Search_AggregationViewer(RequestSpecificValues, ViewBag);
                    }
                    return new Basic_Search_AggregationViewer(RequestSpecificValues, ViewBag);

				case Item_Aggregation_Views_Searches_Enum.Basic_Search_YearRange:
                    return new Basic_Search_YearRange_AggregationViewer(RequestSpecificValues, ViewBag);

                case Item_Aggregation_Views_Searches_Enum.Basic_Search_FullTextOption:
                    return new Basic_Text_Search_Combined_AggregationViewer(RequestSpecificValues, ViewBag);

                case Item_Aggregation_Views_Searches_Enum.Basic_Search_MimeType:
                    Item_Aggregation_Front_Banner frontBannerImage2 = ViewBag.Hierarchy_Object.FrontBannerObj;
                    if ((frontBannerImage2 != null) && (ViewBag.Hierarchy_Object.Highlights != null) && (ViewBag.Hierarchy_Object.Highlights.Count > 0))
                    {
                        return new Rotating_Highlight_MimeType_AggregationViewer(RequestSpecificValues, ViewBag);
                    }
                    return new Basic_Search_MimeType_AggregationViewer(RequestSpecificValues, ViewBag);

                case Item_Aggregation_Views_Searches_Enum.FullText_Search:
                    return new Full_Text_Search_AggregationViewer(RequestSpecificValues, ViewBag);

                case Item_Aggregation_Views_Searches_Enum.No_Home_Search:
                    return new No_Search_AggregationViewer(RequestSpecificValues, ViewBag);

                case Item_Aggregation_Views_Searches_Enum.Newspaper_Search:
                    return new Newspaper_Search_AggregationViewer(RequestSpecificValues, ViewBag);

                case Item_Aggregation_Views_Searches_Enum.Map_Search:
                    return new Map_Search_AggregationViewer(RequestSpecificValues, ViewBag);

                case Item_Aggregation_Views_Searches_Enum.Map_Search_Beta:
                    return new Map_Search_AggregationViewer_Beta(RequestSpecificValues, ViewBag);

                case Item_Aggregation_Views_Searches_Enum.DLOC_FullText_Search:
                    return new dLOC_Search_AggregationViewer(RequestSpecificValues, ViewBag);

                default:
                    return null;
            }
        }

        /// <summary> Returns a built collection viewer matching request </summary>
        /// <param name="SearchType"> Type of search from the current http request </param>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        /// <param name="ViewBag"> View bag holds specific data that was pulled for an aggregation request </param>
        /// <returns> Collection viewer that extends the <see cref="abstractAggregationViewer"/> class. </returns>
        public static abstractAggregationViewer Get_Viewer(Search_Type_Enum SearchType, RequestCache RequestSpecificValues, AggregationViewBag ViewBag)
        {
            switch (SearchType)
            {
                case Search_Type_Enum.Advanced:
                    if (ViewBag.Hierarchy_Object.Views_And_Searches.Contains(Item_Aggregation_Views_Searches_Enum.Advanced_Search_YearRange))
                        return new Advanced_Search_YearRange_AggregationViewer(RequestSpecificValues, ViewBag);
                    if (ViewBag.Hierarchy_Object.Views_And_Searches.Contains(Item_Aggregation_Views_Searches_Enum.Advanced_Search_MimeType))
                        return new Advanced_Search_MimeType_AggregationViewer(RequestSpecificValues, ViewBag);
                    return new Advanced_Search_AggregationViewer(RequestSpecificValues, ViewBag);

                case Search_Type_Enum.Basic:
                    Item_Aggregation_Front_Banner frontBannerImage = ViewBag.Hierarchy_Object.FrontBannerObj;
                    if ((frontBannerImage != null) && (ViewBag.Hierarchy_Object.Highlights != null) && (ViewBag.Hierarchy_Object.Highlights.Count > 0))
                    {
                        return new Rotating_Highlight_Search_AggregationViewer(RequestSpecificValues, ViewBag);
                    }
                    return new Basic_Search_AggregationViewer(RequestSpecificValues, ViewBag);

                case Search_Type_Enum.Full_Text:
                    return new Full_Text_Search_AggregationViewer(RequestSpecificValues, ViewBag);

                case Search_Type_Enum.Newspaper:
                    return new Newspaper_Search_AggregationViewer(RequestSpecificValues, ViewBag);

                case Search_Type_Enum.Map:
                    return new Map_Search_AggregationViewer(RequestSpecificValues, ViewBag);

                case Search_Type_Enum.Map_Beta:
                    return new Map_Search_AggregationViewer_Beta(RequestSpecificValues, ViewBag);

                case Search_Type_Enum.dLOC_Full_Text:
                    return new dLOC_Search_AggregationViewer(RequestSpecificValues, ViewBag);

                default:
                    return null;
            }
        }
    }
}
