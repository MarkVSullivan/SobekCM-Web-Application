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
        public string Label { get; set; }

        /// <summary> Collection of individual IP addresses or individual IP ranges </summary>
        [DataMember(Name = "ipRanges", EmitDefaultValue = false)]
        [XmlArray("ipRanges")]
        [XmlArrayItem("ipRange", typeof(Engine_IpRange))]
        [ProtoMember(3)]
        public List<Engine_IpRange> IpRanges { get; set; }

        /// <summary> Add a new (single IP) ip value to this range </summary>
        /// <param name="Label"> Descriptive label for this particular IP address(es) </param>
        /// <param name="IpAddress"> IP address, or the beginning of a range of IP addresses </param>
        public void Add_IP_Range(string Label, string IpAddress )
        {
            Engine_IpRange singleIpRange = new Engine_IpRange {Label = Label, StartIp = IpAddress};
            IpRanges.Add(singleIpRange);
        }
    }
}