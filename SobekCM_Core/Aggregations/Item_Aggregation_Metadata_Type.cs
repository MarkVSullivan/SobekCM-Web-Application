#region Using directives

using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.Aggregations
{
    /// <summary> Information about a single metadata type, related to an item aggregation </summary>
    [Serializable, DataContract, ProtoContract]
    public class Item_Aggregation_Metadata_Type
    {
        /// <summary> Constructor for a new instance of the Item_Aggregation_Metadata_Type class </summary>
        public Item_Aggregation_Metadata_Type() { }

        /// <summary> Constructor for a new instance of the Item_Aggregation_Metadata_Type class </summary>
        /// <param name="DisplayTerm"> Display term for this metadata type </param>
        /// <param name="SobekCode"> Code related to this metadata type, used for searching for example </param>
        public Item_Aggregation_Metadata_Type( string DisplayTerm, string SobekCode)
        {
            this.DisplayTerm = DisplayTerm;
            this.SobekCode = SobekCode;
        }

        /// <summary> Display term for this metadata type </summary>
        [DataMember(Name = "term")]
        [XmlText]
        [ProtoMember(1)]
        public string DisplayTerm { get; set; }

        /// <summary> Code related to this metadata type, used for searching for example </summary>
        [DataMember(Name = "code")]
        [XmlAttribute("code")]
        [ProtoMember(2)]
        public string SobekCode { get; set; }
    }
}