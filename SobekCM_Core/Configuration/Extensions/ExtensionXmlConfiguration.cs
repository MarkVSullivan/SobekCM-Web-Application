using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.Configuration.Extensions
{
    /// <summary> A named section of configuration that is retained in XML format from the extension configuration </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("ExtensionXmlConfig")]
    public class ExtensionXmlConfiguration
    {
        /// <summary> Name of this configuration section in the extension config file </summary>
        [DataMember(Name = "name")]
        [XmlAttribute("name")]
        [ProtoMember(1)]
        public string SectionName { get; set; }

        /// <summary> InnerXML in string format </summary>
        [DataMember(Name = "innerXml")]
        [XmlElement("innerXml")]
        [ProtoMember(2)]
        public string InnerXml { get; set; }

        /// <summary> Constructor for a new instance of the <see cref="ExtensionXmlConfiguration"/> class </summary>
        public ExtensionXmlConfiguration()
        {
            // Do nothing
        }
    }
}
