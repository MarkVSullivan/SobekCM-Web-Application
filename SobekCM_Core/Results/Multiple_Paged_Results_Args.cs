#region Using directives

using System.Collections.Generic;

#endregion

namespace SobekCM.Core.Results
{
    /// <summary> Arguments returned from a search or browse against the SobekCM database which can potentially
    /// return both the requested page of results, as well as a number of pages ahead for caching and quick retrieval
    /// should the user desire to move forward in the set of results </summary>
    public class Multiple_Paged_Results_Args
    {
        /// <summary> Constructor for a new instance of the Multiple_Paged_Results_Args class </summary>
        public Multiple_Paged_Results_Args()
        {
            // Do nothing
        }

        /// <summary> Constructor for a new instance of the Multiple_Paged_Results_Args class </summary>
        /// <param name="Statistics"> Statistics/information about the overall search or browse, including initial query time, complete results counts, and facets </param>
        /// <param name="Paged_Results"> Collection of paged results, which are themselves a collection of search title results </param>
        public Multiple_Paged_Results_Args(Search_Results_Statistics Statistics, List<List<iSearch_Title_Result>> Paged_Results)
        {
            this.Statistics = Statistics;
            this.Paged_Results = Paged_Results;
        }

        /// <summary> Statistics/information about the overall search or browse, including initial query time, complete results counts, and facets </summary>
        public Search_Results_Statistics Statistics { get; set; }

        /// <summary> Collection of paged results, which are themselves a collection of search title results </summary>
        public List<List<iSearch_Title_Result>> Paged_Results { get; set; }
    }
}
