#region Using directives

using System.Collections.Generic;
using SobekCM.Core.Aggregations;
using SobekCM.Core.Items;
using SobekCM.Core.Navigation;
using SobekCM.Core.Results;
using SobekCM.Core.SiteMap;
using SobekCM.Core.Skins;
using SobekCM.Core.Users;
using SobekCM.Core.WebContent;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library
{
    /// <summary> Class holds all the information about one specific request and the non-global data that is specifically
    /// needed to resolve the request. </summary>
    public class RequestCache
    {
        /// <summary> Constructor for a new instance of the RequestCache class </summary>
        /// <param name="Current_Mode"> Mode / navigation information for the current request</param>
        /// <param name="Hierarchy_Object"> Current item aggregation object to display </param>
        /// <param name="Results_Statistics"> Information about the entire set of results for a search or browse </param>
        /// <param name="Paged_Results"> Single page of results for a search or browse, within the entire set </param>
        /// <param name="Browse_Object"> Object contains all the basic information about any browse or info display </param>
        /// <param name="Current_Item"> Current item to display </param>
        /// <param name="Current_Page"> Current page within the item</param>
        /// <param name="HTML_Skin"> HTML Web skin which controls the overall appearance of this digital library </param>
        /// <param name="Current_User"> Currently logged on user </param>
        /// <param name="Public_Folder"> Object contains the information about the public folder to display </param>
        /// <param name="Site_Map"> Optional site map object used to render a navigational tree-view on left side of static web content pages </param>
        /// <param name="Items_In_Title"> List of items within the current title ( used for the Item Group display )</param>
        /// <param name="Static_Web_Content"> HTML content-based browse, info, or imple CMS-style web content objects.  These are objects which are read from a static HTML file and much of the head information must be maintained </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public RequestCache(SobekCM_Navigation_Object Current_Mode,
            Item_Aggregation Hierarchy_Object,
            Search_Results_Statistics Results_Statistics,
            List<iSearch_Title_Result> Paged_Results,
            Item_Aggregation_Child_Page Browse_Object,
            SobekCM_Item Current_Item,
            Page_TreeNode Current_Page,
            SobekCM_Skin_Object HTML_Skin,
            User_Object Current_User,
            Public_User_Folder Public_Folder,
            SobekCM_SiteMap Site_Map,
            SobekCM_Items_In_Title Items_In_Title,
            HTML_Based_Content Static_Web_Content,
            Custom_Tracer Tracer)
        {
            this.Current_Mode = Current_Mode;
            this.Hierarchy_Object = Hierarchy_Object;
            this.Results_Statistics = Results_Statistics;
            this.Paged_Results = Paged_Results;
            this.Browse_Object = Browse_Object;
            this.Current_Item = Current_Item;
            this.Current_Page = Current_Page;
            this.HTML_Skin = HTML_Skin;
            this.Current_User = Current_User;
            this.Public_Folder = Public_Folder;
            this.Site_Map = Site_Map;
            this.Items_In_Title = Items_In_Title;
            this.Static_Web_Content = Static_Web_Content;
            this.Tracer = Tracer;
        }

        /// <summary> Mode / navigation information for the current request </summary>
        public readonly SobekCM_Navigation_Object Current_Mode;

        /// <summary>  Current item aggregation object to display  </summary>
        public readonly Item_Aggregation Hierarchy_Object;

        /// <summary> Information about the entire set of results for a search or browse </summary>
        public readonly Search_Results_Statistics Results_Statistics;

        /// <summary> Single page of results for a search or browse, within the entire set </summary>
        public readonly List<iSearch_Title_Result> Paged_Results;

        /// <summary> Object contains all the basic information about any browse or info display </summary>
        public readonly Item_Aggregation_Child_Page Browse_Object;

        /// <summary> Current item to display </summary>
        public readonly SobekCM_Item Current_Item;

        /// <summary> Current page within the item </summary>
        public Page_TreeNode Current_Page { get; set; }
    

        /// <summary> HTML Web skin which controls the overall appearance of this digital library </summary>
        public readonly SobekCM_Skin_Object HTML_Skin;

        /// <summary> Currently logged on user </summary>
        public readonly User_Object Current_User;

        /// <summary> Object contains the information about the public folder to display </summary>
        public readonly Public_User_Folder Public_Folder;

        /// <summary> Optional site map object used to render a navigational tree-view on left side of static web content pages </summary>
        public readonly SobekCM_SiteMap Site_Map;

        /// <summary> List of items within the current title ( used for the Item Group display ) </summary>
        public SobekCM_Items_In_Title Items_In_Title { get; set; }

        /// <summary> HTML content-based browse, info, or imple CMS-style web content objects.  These are objects which are read from a static HTML file and much of the head information must be maintained  </summary>
        public readonly HTML_Based_Content Static_Web_Content;

        /// <summary>  Trace object keeps a list of each method executed and important milestones in rendering  </summary>
        public readonly Custom_Tracer Tracer;
    }
}
