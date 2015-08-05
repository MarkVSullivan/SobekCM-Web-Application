using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.WebContent.Single
{
    /// <summary> List of all the milestones / changes affecting a single web content page </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("recentChanges")]
    public class Single_WebContent_Change_Report
    {
        /// <summary> Primary key of the web content page milestone report </summary>
        [DataMember(EmitDefaultValue = false, Name = "id")]
        [XmlAttribute("id")]
        [ProtoMember(1)]
        public int WebContentID { get; set; }

        /// <summary> Returns the orderd list of milestones for this page </summary>
        [DataMember(Name = "changes")]
        [XmlArray("changes")]
        [XmlArrayItem("recentChange", typeof(Milestone_Entry))]
        [ProtoMember(2)]
        public List<Milestone_Entry> Changes { get; set; }

        /// <summary> Constructor for a new instance of the Single_WebContent_Change_Report class </summary>
        /// <remarks> This class wraps the returned milestone report in a return wrapper </remarks>
        public Single_WebContent_Change_Report()
        {
            Changes = new List<Milestone_Entry>();
        }
    }
}
