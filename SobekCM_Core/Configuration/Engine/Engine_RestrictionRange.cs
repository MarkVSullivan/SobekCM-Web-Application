#region Using directives

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.Configuration.Engine
{
    /// <summary> Collection of IP ranges which can be used to limit access to a microservice endpoint </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("EngineIpRestrictionSet")]
    public class Engine_RestrictionRange
    {
        /// <summary> Constructor for a new instance of the Engine_RestrictionRange class </summary>
        public Engine_RestrictionRange()
        {
            IpRanges = new List<Engine_IpRange>();
        }

        /// <summary> Identifier for this component, which is referenced within the configuration file to specify this component </summary>
        [DataMember(Name = "id", EmitDefaultValue = false)]
        [XmlAttribute("id")]
        [ProtoMember(1)]
        public string ID { get; set; }

        /// <summary> Descriptive label for this collection of IP addresses </summary>
        [DataMember(Name = "label", EmitDefaultValue = false)]
        [XmlAttribute("label")]
        [ProtoMember(2)]
        public string Label { get; internal set; }

        /// <summary> Collection of individual IP addresses or individual IP ranges </summary>
        [DataMember(Name = "ipRanges", EmitDefaultValue = false)]
        [XmlArray("ipRanges")]
        [XmlArrayItem("ipRange", typeof(Engine_IpRange))]
        [ProtoMember(3)]
        public List<Engine_IpRange> IpRanges { get; private set; }
    }
}