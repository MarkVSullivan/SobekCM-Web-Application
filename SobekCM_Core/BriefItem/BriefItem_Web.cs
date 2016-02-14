using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.BriefItem
{
    /// <summary> Specification of how this item should behave within this library </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("web")]
    public class BriefItem_Web
    { 
        /// <summary> Base source URL for this item </summary>
        [DataMember(EmitDefaultValue = false, Name = "pageFileExtensionsForDownload")]
        [XmlElement("pageFileExtensionsForDownload")]
        [ProtoMember(1)]
        public string Source_URL { get; set; }
    }
}
