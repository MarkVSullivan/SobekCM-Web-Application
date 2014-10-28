#region Using directives

using System;

#endregion

namespace SobekCM.Core.Results
{
    /// <summary> Class holds the specialized aggregation facets, which also includes the aggregation code</summary>
    [Serializable]
    public class Search_Facet_Aggregation : Search_Facet
    {
         /// <summary> Aggregation code associated with this facet </summary>
        public readonly string Code;

        /// <summary> Constructor for a new instance of the Search_Facet_Aggregation class </summary>
        /// <param name="Facet"> Text of this facet </param>
        /// <param name="Frequency"> Frequency of this facet ( number of occurances )</param>
        /// <param name="Code"> Aggregation code associated with this facet </param>
        public Search_Facet_Aggregation(string Facet, int Frequency, string Code ) : base ( Facet, Frequency )
        {
            this.Code = Code;
        }
    }
}
