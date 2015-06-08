using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.ResultTitle
{
    /// <summary> Title information from a result set  </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("resultItem")]
    public class ResultItemInfo
    {
        /// <summary> Volume identifier for this item </summary>
        [DataMember(Name = "vid")]
        [XmlAttribute("vid")]
        [ProtoMember(1)]
        public string VID { get; set; }

        /// <summary> Item title for this item </summary>
        [DataMember(Name = "title")]
        [XmlElement("title")]
        [ProtoMember(2)]
        public string Title { get; set; }

        /// <summary> Main thumbnail for this item </summary>
        [DataMember(EmitDefaultValue = false, Name = "mainThumbnail")]
        [XmlElement("mainThumbnail")]
        [ProtoMember(3)]
        public string MainThumbnail { get; set; }

        /// <summary> Highlighted snippet of text from this document </summary>
        [DataMember(EmitDefaultValue = false, Name = "snippet")]
        [XmlElement("snippet")]
        [ProtoMember(4)]
        public string Snippet { get; set; }

        /// <summary> Highlighted snippet of text from this document </summary>
        [DataMember(EmitDefaultValue = false, Name = "link")]
        [XmlElement("link")]
        [ProtoMember(5)]
        public string Link { get; set; }

        /// <summary> Constructor for a new instance of the ResultItemInfo class </summary>
        public ResultItemInfo()
        {
            // Parameterless constructor for deserialization
        }
    }
}
