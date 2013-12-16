#region Using directives

using System.Collections.Generic;

#endregion

namespace SobekCM.Library.Results
{
    /// <summary> Arguments returned from a search or browse against the SobekCM database which will only include a single
    /// page of results, without any page lookahead </summary>
    public class Single_Paged_Results_Args
    {
        /// <summary> Constructor for a new instance of the Single_Paged_Results_Args class </summary>
        public Single_Paged_Results_Args()
        {
            // Do nothing
        }

        /// <summary> Constructor for a new instance of the Single_Paged_Results_Args class </summary>
        /// <param name="Statistics"> Statistics/information about the overall search or browse, including initial query time, complete results counts, and facets </param>
        /// <param name="Paged_Results"> Single  page of results, which is collection of search title results </param>
        public Single_Paged_Results_Args(Search_Results_Statistics Statistics, List<iSearch_Title_Result> Paged_Results)
        {
            this.Statistics = Statistics;
            this.Paged_Results = Paged_Results;
        }

        /// <summary> Statistics/information about the overall search or browse, including initial query time, complete results counts, and facets </summary>
        public Search_Results_Statistics Statistics { get; set; }

        /// <summary> Single  page of results, which is collection of search title results </summary>
        public List<iSearch_Title_Result> Paged_Results { get; set; }
    }
}
