#region Using directives

using System;

#endregion

namespace SobekCM.Library.Results
{
    /// <summary> Class holds the basic information about a single facet -- the facet and the frequency.</summary>
    [Serializable]
    public class Search_Facet
    {
        /// <summary> Text of this facet </summary>
        public readonly string Facet;

        /// <summary> Frequency of this facet ( number of occurances ) </summary>
        public readonly int Frequency;

        /// <summary> Constructor for a new instance of the Search_Facet class </summary>
        /// <param name="Facet"> Text of this facet </param>
        /// <param name="Frequency"> Frequency of this facet ( number of occurances )</param>
        public Search_Facet(string Facet, int Frequency)
        {
            this.Facet = Facet;
            this.Frequency = Frequency;
        }
    }
}
