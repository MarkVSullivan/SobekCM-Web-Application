using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.Settings
{
    /// <summary> Settings for specific to the Florida SUS schools </summary>
    [Serializable, DataContract, ProtoContract]
    public class Florida_Settings
    {
        /// <summary> Dropbox to check for any <a href="http://fclaweb.fcla.edu/FDA_landing_page">Florida Digital Archive</a> ingest reports </summary>
        [DataMember(Name = "fdaReportDropBox", EmitDefaultValue = false)]
        [XmlElement("fdaReportDropBox")]
        [ProtoMember(1)]
        public string FDA_Report_DropBox { get; set; }

        /// <summary> Gets the Mango Union search base URL, in support of Florida SUS's</summary>
        [DataMember(Name = "mangoUnionSearchUrl", EmitDefaultValue = false)]
        [XmlElement("mangoUnionSearchUrl")]
        [ProtoMember(2)]
        public string Mango_Union_Search_Base_URL { get; set; }

        /// <summary> Gets the Mango Union search text to be displayed, in support of Florida SUS's</summary>
        [DataMember(Name = "mangoUnionSearchText", EmitDefaultValue = false)]
        [XmlElement("mangoUnionSearchText")]
        [ProtoMember(3)]
        public string Mango_Union_Search_Text { get; set; }
    }
}
