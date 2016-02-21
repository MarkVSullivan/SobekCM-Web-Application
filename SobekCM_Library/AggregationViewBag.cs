using System.Collections.Generic;
using SobekCM.Core.Aggregations;
using SobekCM.Core.Results;
using SobekCM.Core.WebContent;

namespace SobekCM.Library
{
    /// <summary> Aggregation view bag is used to hold aggregation specific data that is passed down to
    /// each individual aggregation viewer </summary>
    public class AggregationViewBag
    {
        /// <summary> Constructor for a new instance of the AggregationViewBag class </summary>
        /// <param name="Hierarchy_Object"> Current item aggregation object to display </param>
        /// <param name="Results_Statistics"> Information about the entire set of results for a browse </param>
        /// <param name="Paged_Results"> Single page of results for a browse, within the entire set </param>
        /// <param name="Browse_Object"> Object contains all the basic information about any browse or info display </param>
        /// <param name="Static_Web_Content"> HTML content-based aggregation browse or info.  These are objects which are read from a static HTML file and much of the head information must be maintained </param>
        public AggregationViewBag(Item_Aggregation Hierarchy_Object,
            Search_Results_Statistics Results_Statistics,
            List<iSearch_Title_Result> Paged_Results,
            Item_Aggregation_Child_Page Browse_Object,
            HTML_Based_Content Static_Web_Content)
        {
            this.Hierarchy_Object = Hierarchy_Object;
            this.Results_Statistics = Results_Statistics;
            this.Paged_Results = Paged_Results;
            this.Browse_Object = Browse_Object;
            this.Static_Web_Content = Static_Web_Content;
        }

        /// <summary>  Current item aggregation object to display  </summary>
        public readonly Item_Aggregation Hierarchy_Object;

        /// <summary> Information about the entire set of results for a search or browse </summary>
        public readonly Search_Results_Statistics Results_Statistics;

        /// <summary> Single page of results for a search or browse, within the entire set </summary>
        public readonly List<iSearch_Title_Result> Paged_Results;

        /// <summary> Object contains all the basic information about any browse or info display </summary>
        public readonly Item_Aggregation_Child_Page Browse_Object;

        /// <summary> HTML content-based browse, info, or imple CMS-style web content objects.  These are objects which are read from a static HTML file and much of the head information must be maintained  </summary>
        public readonly HTML_Based_Content Static_Web_Content;
    }
}
