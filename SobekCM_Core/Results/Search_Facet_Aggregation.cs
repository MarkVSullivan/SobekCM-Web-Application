#region Using directives

using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.Results
{
    /// <summary> Class holds the specialized aggregation facets, which also includes the aggregation code</summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("aggregationFacet")]
    public class Search_Facet_Aggregation : Search_Facet
    {
        /// <summary> Aggregation code associated with this facet </summary>
        [DataMember(Name = "code")]
        [XmlAttribute("code")]
        [ProtoMember(3)]
        public string Code { get; set; }

        /// <summary> Constructor for a new instance of the Search_Facet_Aggregation class </summary>
        public Search_Facet_Aggregation()
        {
            // Parameterless constructor for deserialization
        }

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
