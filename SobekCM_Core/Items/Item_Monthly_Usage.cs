using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.Items
{
    /// <summary> Monthly usage for an individual item for an individual month </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("itemUsage")]
    public class Item_Monthly_Usage
    {
        /// <summary> Year for which these statistics apply </summary>
        [DataMember(Name = "year")]
        [XmlAttribute("year")]
        [ProtoMember(1)]
        public int Year { get; set; }

        /// <summary> Month for which these statistics apply </summary>
        [DataMember(Name = "month")]
        [XmlAttribute("month")]
        [ProtoMember(2)]
        public int Month { get; set; }

        /// <summary> Number of views at the title level for this year/month </summary>
        [DataMember(Name = "titleViews")]
        [XmlAttribute("titleViews")]
        [ProtoMember(3)]
        public int Title_Views { get; set; }

        /// <summary> Number of visitors at the title level for this year/month </summary>
        [DataMember(Name = "titleVisitors")]
        [XmlAttribute("titleVisitors")]
        [ProtoMember(4)]
        public int Title_Visitors { get; set; }

        /// <summary> Number of views at the item level for this year/month </summary>
        [DataMember(Name = "views")]
        [XmlAttribute("views")]
        [ProtoMember(5)]
        public int Views { get; set; }

        /// <summary> Number of visitors at the item level for this year/month </summary>
        [DataMember(Name = "visitors")]
        [XmlAttribute("visitors")]
        [ProtoMember(6)]
        public int Visitors { get; set; }

        /// <summary> Constructor for a new instance of the Item_Monthly_Usage class </summary>
        /// <remarks> This constructor is mostly empty, and is primarily to be used for serialization and deserialization </remarks>
        public Item_Monthly_Usage()
        {
            Title_Views = 0;
            Title_Visitors = 0;
            Views = 0;
            Visitors = 0;
        }

        /// <summary> Constructor for a new instance of the Item_Monthly_Usage class </summary>
        /// <param name="Year"> Year for which these statistics apply </param>
        /// <param name="Month"> Month for which these statistics apply </param>
        public Item_Monthly_Usage( int Year, int Month )
        {
            this.Year = Year;
            this.Month = Month;
            Title_Views = 0;
            Title_Visitors = 0;
            Views = 0;
            Visitors = 0;
        }
    }
}
