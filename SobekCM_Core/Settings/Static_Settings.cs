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
    /// <summary> Top-level settings that are fairly consistent, and don't really load from 
    /// any database or configuration value </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("StaticSettings")]
    public class Static_Settings
    {
        /// <summary> Current version number associated with this SobekCM builder application </summary>
        [DataMember(Name = "builderVersion", EmitDefaultValue = false)]
        [XmlAttribute("builderVersion")]
        [ProtoMember(1)]
        public string Current_Builder_Version { get; set; }

        /// <summary> Current version number associated with this SobekCM digital repository web application </summary>
        [DataMember(Name = "webVersion", EmitDefaultValue = false)]
        [XmlAttribute("webVersion")]
        [ProtoMember(2)]
        public string Current_Web_Version { get; set; }

        /// <summary> Gets the list of reserved keywords that cannot be used
        /// for aggregation codes or aggregation aliases </summary>
        /// <remarks> These are all lower case </remarks>
        [DataMember(Name = "reservedKeywords", EmitDefaultValue = false)]
        [XmlArray("reservedKeywords")]
        [XmlArrayItem("keyword", typeof(string))]
        [ProtoMember(3)]
        public List<string> Reserved_Keywords { get; set; }
    }
}
