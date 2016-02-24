using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.UI_Configuration.Viewers
{
    /// <summary> Configuration information for the special aggregation HTML writer, including
    /// the viewers configuration </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("AggregationWriterConfig")]
    public class AggregationWriterConfig
    {
        /// <summary> Fully qualified (including namespace) name of the main class used
        /// as the aggregation HTML writer </summary>
        /// <remarks> By default, this would be 'SobekCM.Library.HTML.Aggregation_HtmlSubwriter' </remarks>
        [DataMember(Name = "class")]
        [XmlAttribute("class")]
        [ProtoMember(1)]
        public string Class { get; set; }

        /// <summary> Name of the assembly within which this class resides, unless this
        /// is one of the default subviewers included in the core code </summary>
        /// <remarks> By default, this would be blank </remarks>
        [DataMember(Name = "assembly", EmitDefaultValue = false)]
        [XmlAttribute("assembly")]
        [ProtoMember(2)]
        public string Assembly { get; set; }

        /// <summary> Collection of aggregation viewers mapped to viewer codes </summary>
        [DataMember(Name = "viewers")]
        [XmlArray("viewers")]
        [XmlArrayItem("viewer", typeof(SingleSubViewerConfig))]
        [ProtoMember(3)]
        public List<SingleSubViewerConfig> Viewers { get; set; }

        /// <summary> Constructor for a new instance of the <see cref="AggregationWriterConfig"/> class </summary>
        public AggregationWriterConfig()
        {
            Class = "SobekCM.Library.HTML.Aggregation_HtmlSubwriter";
            Viewers = new List<SingleSubViewerConfig>();
        }
    }
}
