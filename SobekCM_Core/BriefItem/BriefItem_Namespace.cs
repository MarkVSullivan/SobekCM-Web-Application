using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.BriefItem
{
    /// <summary> Namespace definition used within the brief item (generally within the citation)  </summary>
    [Serializable, DataContract, ProtoContract]
    public class BriefItem_Namespace
    {
        /// <summary> Prefix used for this namespace throughout the object </summary>
        [DataMember(Name = "prefix")]
        [XmlAttribute("prefix")]
        [ProtoMember(1)]
        public string Prefix { get; set; }

        /// <summary> URI for the schema/namespace referred to by the prefix  </summary>
        [DataMember(Name = "uri")]
        [XmlText]
        [ProtoMember(2)]
        public string URI { get; set; }

        /// <summary> Constructor for a new instance of the BriefItem_Namespace class </summary>
        public BriefItem_Namespace()
        {
            // Do nothing - used for deserialization
        }

        /// <summary> Constructor for a new instance of the BriefItem_Namespace class </summary>
        /// <param name="Prefix"> Prefix used for this namespace throughout the object </param>
        /// <param name="URI"> URI for the schema/namespace referred to by the prefix </param>
        public BriefItem_Namespace(string Prefix, string URI)
        {
            this.Prefix = Prefix;
            this.URI = URI;
        }
    }
}
