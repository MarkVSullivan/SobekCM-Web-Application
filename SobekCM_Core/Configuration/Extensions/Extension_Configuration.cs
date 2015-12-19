using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.Configuration.Extensions
{
    /// <summary> Collection of all the extension information </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("ExtensionConfig")]
    public class Extension_Configuration
    {
        /// <summary> Collection of information about each extension </summary>
        [DataMember(Name = "extensions", EmitDefaultValue = false)]
        [XmlArray("extensions")]
        [XmlArrayItem("extension", typeof(ExtensionInfo))]
        [ProtoMember(1)]
        public List<ExtensionInfo> Extensions { get; set; } 
    }
}
