#region Using directives

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.ResultTitle
{
    /// <summary> Title information from a result set  </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("resultTitle")]
    public class ResultTitleInfo
    {
        /// <summary> Bibliographic identifier for this title </summary>
        [DataMember(EmitDefaultValue = false, Name = "bibid")]
        [XmlAttribute("bibid")]
        [ProtoMember(1)]
        public string BibID { get; set; }

        /// <summary> Group title for this title </summary>
        [DataMember(EmitDefaultValue = false, Name = "title")]
        [XmlElement("title")]
        [ProtoMember(2)]
        public string Title { get; set; }

        /// <summary> Main thumbnail for this title </summary>
        [DataMember(EmitDefaultValue = false, Name = "mainThumbnail")]
        [XmlElement("mainThumbnail")]
        [ProtoMember(3)]
        public string MainThumbnail { get; set; }

        /// <summary> Description/Citation elements for this item </summary>
        [DataMember(EmitDefaultValue = false, Name = "description")]
        [XmlArray("description")]
        [XmlArrayItem("descriptiveTerm", typeof(ResultTitle_DescriptiveTerm))]
        [ProtoMember(4)]
        public List<ResultTitle_DescriptiveTerm> Description { get; set; }

        /// <summary> List of items </summary>
        [DataMember(EmitDefaultValue = false, Name = "items")]
        [XmlArray("items")]
        [XmlArrayItem("item", typeof(ResultItemInfo))]
        [ProtoMember(4)]
        public List<ResultItemInfo> Items { get; set; }

        /// <summary> Constructor for a new instance of the <see cref="ResultTitleInfo"/> class.  </summary>
        public ResultTitleInfo()
        {
            Items = new List<ResultItemInfo>();
            Description = new List<ResultTitle_DescriptiveTerm>();
        }
    }
}
