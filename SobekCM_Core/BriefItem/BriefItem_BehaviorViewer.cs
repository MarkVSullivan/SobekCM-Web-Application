using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.BriefItem
{
    /// <summary> Viewer tied to this item </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("behaviors")]
    public class BriefItem_BehaviorViewer
    {
        /// <summary> Name of this viewer type, from the database </summary>
        [DataMember(Name = "type")]
        [XmlAttribute("type")]
        [ProtoMember(1)]
        public string ViewerType { get; set; }

        /// <summary> Order of this viewer within the context of other viewers </summary>
        [DataMember(Name = "order")]
        [XmlAttribute("order")]
        [ProtoMember(2)]
        public int Order { get; set; }

        /// <summary> Flag indicates if this viewer is explicitly excluded from this digital resource </summary>
        [DataMember(Name = "excluded")]
        [XmlAttribute("excluded")]
        [ProtoMember(3)]
        public bool Excluded { get; set; }
    }
}
