#region Using directives

using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.Aggregations
{
    /// <summary> Basic statistical information for an item aggregation, including basic counts of items, titles, pages, etc..  </summary>
    [Serializable, DataContract, ProtoContract]
    public class Item_Aggregation_Statistics
    {
        /// <summary> Number of pages for all the items within this item aggregation </summary>
        [DataMember(EmitDefaultValue = false, Name = "pageCount")]
        [XmlAttribute("pageCount")]
        [ProtoMember(1)]
        public int Page_Count { get; set; }

        /// <summary> Number of titles within this item aggregation </summary>
        [DataMember(EmitDefaultValue = false, Name = "titleCount")]
        [XmlAttribute("titleCount")]
        [ProtoMember(2)]
        public int Title_Count { get; set; }

        /// <summary> Number of items within this item aggregation </summary>
        [DataMember(EmitDefaultValue = false, Name = "itemCount")]
        [XmlAttribute("itemCount")]
        [ProtoMember(3)]
        public int Item_Count { get; set; }

    }
}
