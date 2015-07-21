using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.WebContent
{
    /// <summary> Represents a collection of basic information about web content pages and redirects </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("webContentCollection")]
    public class WebContent_Basic_Pages
    {
        /// <summary> Number of pages or redirects </summary>
        [DataMember(Name = "total")]
        [XmlAttribute("total")]
        [ProtoMember(1)]
        public int Total { get; set; }

        /// <summary> Number of rows returned per page of web content information </summary>
        [DataMember(Name = "rowsPerPage")]
        [XmlAttribute("rowsPerPage")]
        [ProtoMember(2)]
        public int RowsPerPage { get; set; }

        /// <summary> Page of web content (or redirects) requested, within the larger framework </summary>
        [DataMember(Name = "page")]
        [XmlAttribute("page")]
        [ProtoMember(3)]
        public int Page { get; set; }

        /// <summary> Returns the list of web content matching the requested page, filter, etc.. </summary>
        [DataMember(Name = "contentCollection")]
        [XmlArray("contentCollection")]
        [XmlArrayItem("content", typeof(WebContent_Basic_Info))]
        [ProtoMember(4)]
        public List<WebContent_Basic_Info> ContentCollection { get; set; }

        /// <summary> Constructor for a new instance of the WebContent_Recent_Changes class </summary>
        /// <remarks> This class wraps the returned recent changes in a return wrapper </remarks>
        public WebContent_Basic_Pages()
        {
            ContentCollection = new List<WebContent_Basic_Info>();
        }
    }
}
