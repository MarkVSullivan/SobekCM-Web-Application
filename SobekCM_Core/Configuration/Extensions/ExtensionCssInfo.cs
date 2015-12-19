using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.Configuration.Extensions
{
    public enum ExtensionCssInfoConditionEnum : byte
    {
        // always|admin|item|metadata|mysobek

        always,

        admin,

        item,

        metadata,

        mysobek,

        aggregation,

        results
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("ExtensionCssInfo")]
    public class ExtensionCssInfo
    {
        /// <summary> Condition upon which this CSS file should be added </summary>
        [DataMember(Name = "condition")]
        [XmlAttribute("condition")]
        [ProtoMember(1)]
        public ExtensionCssInfoConditionEnum Condition { get; set; }

        /// <summary> URL for this CSS </summary>
        [DataMember(Name = "url")]
        [XmlAttribute("url")]
        [ProtoMember(2)]
        public string URL { get; set; }
    }
}
