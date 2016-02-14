using System;
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
    }
}
