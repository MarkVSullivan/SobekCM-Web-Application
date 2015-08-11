using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.WebContent.Admin
{
    /// <summary> Represents a collection recently changed web pages, the user who made the changes, and 
    /// the dates and types of the changes </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("globalRecentChanges")]
    public class WebContent_Recent_Changes
    {
        /// <summary> Number of recent changes matching query </summary>
        [DataMember(Name = "total")]
        [XmlAttribute("total")]
        [ProtoMember(1)]
        public int Total { get; set; }

        /// <summary> If there was a user filter applied for this result set, included here </summary>
        [DataMember(Name = "userFilter",EmitDefaultValue=false, IsRequired=false)]
        [XmlAttribute("userFilter")]
        [ProtoMember(2)]
        public string UserFilter { get; set; }

        /// <summary> Number of rows returned per page of recent updates </summary>
        [DataMember(Name = "rowsPerPage")]
        [XmlAttribute("rowsPerPage")]
        [ProtoMember(3)]
        public int RowsPerPage { get; set; }

        /// <summary> Page of recent updates requested, within the larger framework </summary>
        [DataMember(Name = "page")]
        [XmlAttribute("page")]
        [ProtoMember(4)]
        public int Page { get; set; }

        /// <summary> Returns the list of changes matching the requested page, filter, etc.. </summary>
        [DataMember(Name = "changes")]
        [XmlArray("changes")]
        [XmlArrayItem("recentChange", typeof(WebContent_Recent_Changed_Page))]
        [ProtoMember(5)]
        public List<WebContent_Recent_Changed_Page> Changes { get; set; }

        /// <summary> Constructor for a new instance of the WebContent_Recent_Changes class </summary>
        /// <remarks> This class wraps the returned recent changes in a return wrapper </remarks>
        public WebContent_Recent_Changes()
        {
            Changes = new List<WebContent_Recent_Changed_Page>();
        }
    }
}
