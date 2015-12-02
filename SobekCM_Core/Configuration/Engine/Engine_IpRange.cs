
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.Configuration.Engine
{
    /// <summary> A single IP address (or one continuous range of IP addresses) which make up one 
    /// part of a restriction range for restricting access to microservices </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("EngineIpRange")]
    public class Engine_IpRange
    {
        /// <summary> Descriptive label for this particular IP address(es) </summary>
        [DataMember(Name = "label", EmitDefaultValue = false)]
        [XmlAttribute("label")]
        [ProtoMember(1)]
        public string Label { get; set; }

        /// <summary> IP address, or the beginning of a range of IP addresses </summary>
        [DataMember(Name = "start")]
        [XmlAttribute("start")]
        [ProtoMember(2)]
        public string StartIp { get; set;  }

        /// <summary> Ending IP address, in the case this is a range of IP addresses </summary>
        [DataMember(Name = "end", EmitDefaultValue = false)]
        [XmlAttribute("end")]
        [ProtoMember(3)]
        public string EndIp { get; set; }
    }
}