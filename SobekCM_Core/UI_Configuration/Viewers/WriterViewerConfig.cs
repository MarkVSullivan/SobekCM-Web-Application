using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.UI_Configuration.Viewers
{
    /// <summary> Configuration maps between all viewer codes used for
    /// most functionality in-system to th specific subviewer used </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("WriterViewerConfig")]
    public class WriterViewerConfig
    {
        /// <summary> Collection of item subviewers mapped to viewer codes </summary>
        [DataMember(Name = "itemSubViewers")]
        [XmlArray("itemSubViewers")]
        [XmlArrayItem("viewer", typeof(SingleSubViewerConfig))]
        [ProtoMember(1)]
        public List<SingleSubViewerConfig> ItemViewers { get; set; }

        /// <summary> Collection of aggregation subviewers mapped to viewer codes </summary>
        [DataMember(Name = "aggregationsSubViewers")]
        [XmlArray("aggregationsSubViewers")]
        [XmlArrayItem("viewer", typeof(SingleSubViewerConfig))]
        [ProtoMember(2)]
        public List<SingleSubViewerConfig> AggregationViewers { get; set; }

        /// <summary> Collection of results subviewers mapped to viewer codes </summary>
        [DataMember(Name = "resultsSubViewers")]
        [XmlArray("resultsSubViewers")]
        [XmlArrayItem("viewer", typeof(SingleSubViewerConfig))]
        [ProtoMember(3)]
        public List<SingleSubViewerConfig> ResultsViewers { get; set; }

        /// <summary> Collection of mysobek subviewers mapped to viewer codes </summary>
        [DataMember(Name = "mySobekSubViewers")]
        [XmlArray("mySobekSubViewers")]
        [XmlArrayItem("viewer", typeof(SingleSubViewerConfig))]
        [ProtoMember(4)]
        public List<SingleSubViewerConfig> MySobekViewers { get; set; }

        /// <summary> Collection of admin subviewers mapped to viewer codes </summary>
        [DataMember(Name = "adminSubViewers")]
        [XmlArray("adminSubViewers")]
        [XmlArrayItem("viewer", typeof(SingleSubViewerConfig))]
        [ProtoMember(5)]
        public List<SingleSubViewerConfig> AdminViewers { get; set; }

        /// <summary> Constructor for a new instance of the <see cref="WriterViewerConfig"/> class </summary>
        public WriterViewerConfig()
        {
            ItemViewers = new List<SingleSubViewerConfig>();
            AggregationViewers = new List<SingleSubViewerConfig>();
            ResultsViewers = new List<SingleSubViewerConfig>();
            MySobekViewers = new List<SingleSubViewerConfig>();
            AdminViewers = new List<SingleSubViewerConfig>();
        }
 

    }
}
