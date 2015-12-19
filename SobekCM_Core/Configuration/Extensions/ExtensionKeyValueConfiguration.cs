using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;
using SobekCM.Tools;

namespace SobekCM.Core.Configuration.Extensions
{
    /// <summary> A named collection of key value pairs from the extension configuration </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("ExtensionKeyValuePairConfig")]
    public class ExtensionKeyValueConfiguration
    {
        /// <summary> Name of this configuration section in the extension config file </summary>
        [DataMember(Name = "name")]
        [XmlAttribute("name")]
        [ProtoMember(1)]
        public string SectionName { get; set; }

        /// <summary> Collection of key value pairs </summary>
        [DataMember(Name = "keyValuePairs", EmitDefaultValue = false)]
        [XmlArray("keyValuePairs")]
        [XmlArrayItem("keyValuePair", typeof(StringKeyValuePair))]
        [ProtoMember(2)]
        public List<StringKeyValuePair> KeyValuePairs { get; set; }

        /// <summary> Constructor for a new instance of the <see cref="ExtensionKeyValueConfiguration"/> class </summary>
        public ExtensionKeyValueConfiguration()
        {
            KeyValuePairs = new List<StringKeyValuePair>();
        }
    }
}
