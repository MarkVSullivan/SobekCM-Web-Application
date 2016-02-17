using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.BriefItem
{
    /// <summary> Specification of how this item should behave within this library </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("behaviors")]
    public class BriefItem_Behaviors
    {
        /// <summary> List of page file extenions that should be listed in the downloads tab </summary>
        [DataMember(EmitDefaultValue = false, Name = "pageFileExtensionsForDownload")]
        [XmlElement("pageFileExtensionsForDownload")]
        [ProtoMember(1)]
        public string[] Page_File_Extensions_For_Download { get; set; }

        /// <summary> Complete embed html tag for an embedded video ( i.e., from YouTube for example ) </summary>
        [DataMember(EmitDefaultValue = false, Name = "embeddedVideo")]
        [XmlElement("embeddedVideo")]
        [ProtoMember(2)]
        public string Embedded_Video { get; set; }

        /// <summary> List of viewers attached to this item </summary>
        [DataMember(EmitDefaultValue = false, Name = "viewers")]
        [XmlArray("viewers")]
        [XmlArrayItem("viewer", typeof(BriefItem_BehaviorViewer))]
        [ProtoMember(3)]
        public List<BriefItem_BehaviorViewer> Viewers { get; set; }

        /// <summary> Flag indicates which IP ranges have access to this item, or 0 if this is public, or -1 if private </summary>
        [DataMember(EmitDefaultValue = false, Name = "ipRestriction")]
        [XmlAttribute("ipRestriction")]
        [ProtoMember(4)]
        public short IP_Restriction_Membership { get; set; }

        /// <summary> Flag indicates which IP ranges have access to this item, or 0 if this is public, or -1 if private </summary>
        [DataMember(EmitDefaultValue = false, Name = "dark")]
        [XmlAttribute("dark")]
        [ProtoMember(5)]
        public bool Dark_Flag { get; set; }

        /// <summary> List of all the aggregation codes associated with this item </summary>
        [DataMember(EmitDefaultValue = false, Name = "aggregations")]
        [XmlArray("aggregations")]
        [XmlArrayItem("aggregation", typeof(string))]
        [ProtoMember(6)]
        public List<string> Aggregation_Code_List { get; set; }

        /// <summary> Code for the source institution aggregation </summary>
        [DataMember(EmitDefaultValue = false, Name = "sourceAggregation")]
        [XmlElement("sourceAggregation")]
        [ProtoMember(7)]
        public string Source_Institution_Aggregation { get; set; }

        /// <summary> Code for the holding location aggregation </summary>
        [DataMember(EmitDefaultValue = false, Name = "holdingAggregation")]
        [XmlElement("holdingAggregation")]
        [ProtoMember(8)]
        public string Holding_Location_Aggregation { get; set; }

        /// <summary> Type for the overall item group (title) </summary>
        [DataMember(EmitDefaultValue = false, Name = "groupType")]
        [XmlAttribute("groupType")]
        [ProtoMember(9)] 
        public string GroupType { get; set; }


        /// <summary> Constructor for a new instance of the BriefItem_Behaviors class </summary>
        public BriefItem_Behaviors()
        {
            Viewers = new List<BriefItem_BehaviorViewer>();
        }
    }
}
