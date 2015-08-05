using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.WebContent.Single
{
    /// <summary> Usage report for a single web content page </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("usageReport")]
    public class Single_WebContent_Usage_Report
    {
        /// <summary> Primary key of the web content page for which this the usage report applies </summary>
        [DataMember(EmitDefaultValue = false, Name = "id")]
        [XmlAttribute("id")]
        [ProtoMember(1)]
        public int WebContentID { get; set; }

        /// <summary> Returns the orderd list of milestones for this page </summary>
        [DataMember(Name = "usageCollection")]
        [XmlArray("usageCollection")]
        [XmlArrayItem("usage", typeof(Single_WebContent_Usage))]
        [ProtoMember(2)]
        public List<Single_WebContent_Usage> Usage { get; set; }

        /// <summary> Constructor for a new instance of the Single_WebContent_Usage_Report class </summary>
        /// <remarks> This class wraps the returned single page usage in a return wrapper </remarks>
        public Single_WebContent_Usage_Report()
        {
            Usage = new List<Single_WebContent_Usage>();
        }
    }
}
