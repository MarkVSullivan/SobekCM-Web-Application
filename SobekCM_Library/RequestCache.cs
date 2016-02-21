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
        /// <param name="Results_Statistics"> Information about the entire set of results for a search or browse </param>
        /// <param name="Paged_Results"> Single page of results for a search or browse, within the entire set </param>
        /// <param name="HTML_Skin"> HTML Web skin which controls the overall appearance of this digital library </param>
        /// <param name="Current_User"> Currently logged on user </param>
        /// <param name="Public_Folder"> Object contains the information about the public folder to display </param>
        /// <param name="Top_Collection"> Item aggregation for the top-level collection, which is used in a number of places, for example 
        /// showing the correct banner, even when it is not the "current" aggregation </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public RequestCache(Navigation_Object Current_Mode,
            Search_Results_Statistics Results_Statistics,
            List<iSearch_Title_Result> Paged_Results,
            Web_Skin_Object HTML_Skin,
            User_Object Current_User,
            Public_User_Folder Public_Folder,
            Item_Aggregation Top_Collection,
            Custom_Tracer Tracer)
        {
            this.Current_Mode = Current_Mode;
            this.Results_Statistics = Results_Statistics;
            this.Paged_Results = Paged_Results;
            this.HTML_Skin = HTML_Skin;
            this.Current_User = Current_User;
            this.Public_Folder = Public_Folder;
            this.Top_Collection = Top_Collection;
            this.Tracer = Tracer;
        }

        /// <summary> Constructor for a new instance of the RequestCache class </summary>
        /// <param name="Current_Mode"> Mode / navigation information for the current request</param>
        /// <param name="Results_Statistics"> Information about the entire set of results for a search or browse </param>
        /// <param name="Paged_Results"> Single page of results for a search or browse, within the entire set </param>
        /// <param name="Current_User"> Currently logged on user </param>
        /// <param name="Public_Folder"> Object contains the information about the public folder to display </param>
        /// <param name="Top_Collection"> Item aggregation for the top-level collection, which is used in a number of places, for example 
        /// showing the correct banner, even when it is not the "current" aggregation </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public RequestCache(Navigation_Object Current_Mode,
            Search_Results_Statistics Results_Statistics,
            List<iSearch_Title_Result> Paged_Results,
            User_Object Current_User,
            Public_User_Folder Public_Folder,
            Item_Aggregation Top_Collection,
            Custom_Tracer Tracer)
        {
            this.Current_Mode = Current_Mode;
            this.Results_Statistics = Results_Statistics;
            this.Paged_Results = Paged_Results;
            this.Current_User = Current_User;
            this.Public_Folder = Public_Folder;
            this.Top_Collection = Top_Collection;
            this.Tracer = Tracer;
        }

        /// <summary> Mode / navigation information for the current request </summary>
        public readonly Navigation_Object Current_Mode;

        /// <summary> Information about the entire set of results for a search or browse </summary>
        public readonly Search_Results_Statistics Results_Statistics;

        /// <summary> Single page of results for a search or browse, within the entire set </summary>
        public readonly List<iSearch_Title_Result> Paged_Results;

        /// <summary> HTML Web skin which controls the overall appearance of this digital library </summary>
        public Web_Skin_Object HTML_Skin { get; set; }

        /// <summary> Currently logged on user </summary>
        public readonly User_Object Current_User;

        /// <summary> Object contains the information about the public folder to display </summary>
        public readonly Public_User_Folder Public_Folder;

        /// <summary> Item aggregation for the top-level collection, which is used in a number of places, for example 
        /// showing the correct banner, even when it is not the "current" aggregation </summary>
        public readonly Item_Aggregation Top_Collection;

        /// <summary>  Trace object keeps a list of each method executed and important milestones in rendering  </summary>
        public readonly Custom_Tracer Tracer;
    }
}
