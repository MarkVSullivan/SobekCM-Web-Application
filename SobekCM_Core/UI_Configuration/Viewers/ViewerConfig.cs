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
    [XmlRoot("ViewerConfig")]
    public class ViewerConfig
    {
        /// <summary> Collection of item subviewers mapped to viewer codes </summary>
        [DataMember(Name = "itemSubViewers")]
        [XmlArray("itemSubViewers")]
        [XmlArrayItem("viewer", typeof(SingleSubViewer))]
        [ProtoMember(1)]
        public List<SingleSubViewer> ItemViewers { get; set; }

        /// <summary> Collection of aggregation subviewers mapped to viewer codes </summary>
        [DataMember(Name = "aggregationsSubViewers")]
        [XmlArray("aggregationsSubViewers")]
        [XmlArrayItem("viewer", typeof(SingleSubViewer))]
        [ProtoMember(2)]
        public List<SingleSubViewer> AggregationViewers { get; set; }

        /// <summary> Collection of results subviewers mapped to viewer codes </summary>
        [DataMember(Name = "resultsSubViewers")]
        [XmlArray("resultsSubViewers")]
        [XmlArrayItem("viewer", typeof(SingleSubViewer))]
        [ProtoMember(3)]
        public List<SingleSubViewer> ResultsViewers { get; set; }

        /// <summary> Collection of mysobek subviewers mapped to viewer codes </summary>
        [DataMember(Name = "mySobekSubViewers")]
        [XmlArray("mySobekSubViewers")]
        [XmlArrayItem("viewer", typeof(SingleSubViewer))]
        [ProtoMember(4)]
        public List<SingleSubViewer> MySobekViewers { get; set; }

        /// <summary> Collection of admin subviewers mapped to viewer codes </summary>
        [DataMember(Name = "adminSubViewers")]
        [XmlArray("adminSubViewers")]
        [XmlArrayItem("viewer", typeof(SingleSubViewer))]
        [ProtoMember(5)]
        public List<SingleSubViewer> AdminViewers { get; set; }

        /// <summary> Constructor for a new instance of the <see cref="ViewerConfig"/> class </summary>
        public ViewerConfig()
        {
            ItemViewers = new List<SingleSubViewer>();
            AggregationViewers = new List<SingleSubViewer>();
            ResultsViewers = new List<SingleSubViewer>();
            MySobekViewers = new List<SingleSubViewer>();
            AdminViewers = new List<SingleSubViewer>();
        }
 

    }
}
