using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;


namespace SobekCM.Core.Items
{
    /// <summary> Class represents a single worklog history / progress entry for a digital resource </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("trackingEvent")]
    public class Item_Tracking_Event
    {
        /// <summary> Name of the workflow for this single worklog history / progress entry </summary>
        [DataMember(Name = "name")]
        [XmlAttribute("name")]
        [ProtoMember(1)]
        public string WorkflowName { get; set; }

        /// <summary> Date that this worklog history / progress was completed </summary>
        [DataMember(Name = "date")]
        [XmlAttribute("date")]
        [ProtoMember(2)]
        public DateTime CompletedDate { get; set; }
        
        /// <summary> Personal name, username, or vendor name for the party that performed this work </summary>
        [DataMember(EmitDefaultValue = false, Name = "worker")]
        [XmlAttribute("worker")]
        [ProtoMember(3)]
        public string WorkPerformedBy { get; set; }

        /// <summary> Any associated note for this single worklog history / progress entry </summary>
        [DataMember(EmitDefaultValue = false, Name = "notes")]
        [XmlText]
        [ProtoMember(4)]
        public string Notes { get; set; }
    }
}
