using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.WebContent
{
    /// <summary> Collection of pages hit, along with the number of hits, for a date-range
    /// query of usage statistics across many top-level static web content pages </summary>
    public class WebContent_Usage_Report
    {
        /// <summary> Number of pages with hits during this period </summary>
        [DataMember(Name = "total")]
        [XmlAttribute("total")]
        [ProtoMember(1)]
        public int Total { get; set; }

        /// <summary> Number of rows returned per page of usage hist </summary>
        [DataMember(Name = "rowsPerPage")]
        [XmlAttribute("rowsPerPage")]
        [ProtoMember(2)]
        public int RowsPerPage { get; set; }

        /// <summary> Page of usage hits requested, within the larger framework </summary>
        [DataMember(Name = "page")]
        [XmlAttribute("page")]
        [ProtoMember(3)]
        public int Page { get; set; }

        /// <summary> Start year-month for the reported date range </summary>
        [DataMember(Name = "rangeStart", EmitDefaultValue = false)]
        [XmlAttribute("rangeStart")]
        [ProtoMember(4)]
        public string RangeStart { get; set; }

        /// <summary> Ending year-month for the reported date range </summary>
        [DataMember(Name = "rangeEnd", EmitDefaultValue = false)]
        [XmlAttribute("rangeEnd")]
        [ProtoMember(5)]
        public string RangeEnd { get; set; }

        /// <summary> Returns the list of pages with their number of hits from this range </summary>
        [DataMember(Name = "usageCollection")]
        [XmlArray("usageCollection")]
        [XmlArrayItem("pageUsage", typeof(WebContent_Page_Usage))]
        [ProtoMember(6)]
        public List<WebContent_Page_Usage> Pages { get; set; }

        /// <summary> Constructor for a new instance of the WebContent_Usage_Report class </summary>
        /// <remarks> This class wraps the returned recent changes in a return wrapper </remarks>
        public WebContent_Usage_Report()
        {
            Pages = new List<WebContent_Page_Usage>();
        }
    }
}
