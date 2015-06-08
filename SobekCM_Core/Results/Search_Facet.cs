#region Using directives

using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.Results
{
    /// <summary> Class holds the basic information about a single facet -- the facet and the frequency.</summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("facet")]
    public class Search_Facet
    {
        /// <summary> Text of this facet </summary>
        [DataMember(Name = "term")]
        [XmlText]
        [ProtoMember(1)]
        public string Facet { get; set; }

        /// <summary> Frequency of this facet ( number of occurances ) </summary>
        [DataMember(Name = "frequency")]
        [XmlAttribute("frequency")]
        [ProtoMember(2)]
        public int Frequency { get; set; }

        /// <summary> Constructor for a new instance of the Search_Facet class </summary>
        public Search_Facet( )
        {
            // Empty constructor for serialization purposes
        }

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
