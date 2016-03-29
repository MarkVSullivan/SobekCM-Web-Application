using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.Items
{
    /// <summary> Class stores all of the tracking-specific data about a digital resource </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("trackingInfo")]
    public class Item_Tracking_Details
    {
        /// <summary> List of all the workflow events that occurred against this digital resource </summary>
        [DataMember(Name="events")]
        [XmlArray("events")]
        [XmlArrayItem("event", typeof(Item_Tracking_Event))]
        [ProtoMember(1)]
        public List<Item_Tracking_Event> WorkEvents { get; set; }

        /// <summary> Date this item was added to the SobekCM database </summary>
        [DataMember(Name = "createDate")]
        [XmlAttribute("createDate")]
        [ProtoMember(2)]
        public DateTime CreateDate { get; set; }

        /// <summary> Date this item reached the first milestone - digital acquisition </summary>
        [DataMember(Name = "digitalAcquisition")]
        [XmlIgnore]
        [ProtoMember(3)]
        public DateTime? Milestone_DigitalAcquisition { get; set; }

        /// <summary> Date this item reached the first milestone - digital acquisition (for XML serialization)</summary>
        /// <remarks> This property is only exposed to allow for XML serialization of the nullable datetime </remarks>
        [IgnoreDataMember]
        [XmlAttribute("digitalAcquisition")]
        public DateTime Milestone_DigitalAcquisition_XML
        {
            get { return Milestone_DigitalAcquisition.HasValue ? Milestone_DigitalAcquisition.Value : DateTime.MinValue; } 
            set { if (value != DateTime.MinValue) Milestone_DigitalAcquisition = value; }
        }

        /// <summary> Property controls if the associated property is serialized during XML serialization </summary>
        public bool ShouldSerializeMilestone_DigitalAcquisition_XML
        {
            get { return Milestone_DigitalAcquisition.HasValue;  }
        }

        /// <summary> Date this item reached the second milestone - image processing </summary>
        [DataMember(Name = "imageProcessing")]
        [XmlIgnore]
        [ProtoMember(4)]
        public DateTime? Milestone_ImageProcessing { get; set; }

        /// <summary> Date this item reached the second milestone - image processing (for XML serialization)</summary>
        /// <remarks> This property is only exposed to allow for XML serialization of the nullable datetime </remarks>
        [IgnoreDataMember]
        [XmlAttribute("imageProcessing")]
        public DateTime Milestone_ImageProcessing_XML
        {
            get { return Milestone_ImageProcessing.HasValue ? Milestone_ImageProcessing.Value : DateTime.MinValue; }
            set { if (value != DateTime.MinValue) Milestone_ImageProcessing = value; }
        }

        /// <summary> Property controls if the associated property is serialized during XML serialization </summary>
        public bool ShouldSerializeMilestone_ImageProcessing_XML
        {
            get { return Milestone_ImageProcessing.HasValue; }
        }

        /// <summary> Date this item reached the third milestone - quality control </summary>
        [DataMember(Name = "qualityControl")]
        [XmlIgnore]
        [ProtoMember(5)]
        public DateTime? Milestone_QualityControl { get; set; }

        /// <summary> Date this item reached the third milestone - quality control (for XML serialization)</summary>
        /// <remarks> This property is only exposed to allow for XML serialization of the nullable datetime </remarks>
        [IgnoreDataMember]
        [XmlAttribute("qualityControl")]
        public DateTime Milestone_QualityControl_XML
        {
            get { return Milestone_QualityControl.HasValue ? Milestone_QualityControl.Value : DateTime.MinValue; }
            set { if (value != DateTime.MinValue) Milestone_QualityControl = value; }
        }

        /// <summary> Property controls if the associated property is serialized during XML serialization </summary>
        public bool ShouldSerializeMilestone_QualityControl_XML
        {
            get { return Milestone_QualityControl.HasValue; }
        }

        /// <summary> Date this item reached the fourth milestone - online complete </summary>
        [DataMember(Name = "onlineComplete")]
        [XmlIgnore]
        [ProtoMember(6)]
        public DateTime? Milestone_OnlineComplete { get; set; }

        /// <summary> Date this item reached the fourth milestone - online complete (for XML serialization)</summary>
        /// <remarks> This property is only exposed to allow for XML serialization of the nullable datetime </remarks>
        [IgnoreDataMember]
        [XmlAttribute("onlineComplete")]
        public DateTime Milestone_OnlineComplete_XML
        {
            get { return Milestone_OnlineComplete.HasValue ? Milestone_OnlineComplete.Value : DateTime.MinValue; }
            set { if (value != DateTime.MinValue) Milestone_OnlineComplete = value; }
        }

        /// <summary> Property controls if the associated property is serialized during XML serialization </summary>
        public bool ShouldSerializeMilestone_OnlineComplete_XML
        {
            get { return Milestone_OnlineComplete.HasValue; }
        }

        /// <summary> Date the physical version of this material was received in house for digitization </summary>
        [DataMember(Name = "receivedDate")]
        [XmlIgnore]
        [ProtoMember(7)]
        public DateTime? Material_ReceivedDate { get; set; }

        /// <summary> Date the physical version of this material was received in house for digitization (for XML serialization)</summary>
        /// <remarks> This property is only exposed to allow for XML serialization of the nullable datetime </remarks>
        [IgnoreDataMember]
        [XmlAttribute("receivedDate")]
        public DateTime Material_ReceivedDate_XML
        {
            get { return Material_ReceivedDate.HasValue ? Material_ReceivedDate.Value : DateTime.MinValue; }
            set { if (value != DateTime.MinValue) Material_ReceivedDate = value; }
        }

        /// <summary> Property controls if the associated property is serialized during XML serialization </summary>
        public bool ShouldSerializeMaterial_ReceivedDate_XML
        {
            get { return Material_ReceivedDate.HasValue; }
        }

        /// <summary> Date the physical version of this material was disposed of, post-digitization </summary>
        [DataMember(Name = "dispositionDate")]
        [XmlIgnore]
        [ProtoMember(8)]
        public DateTime? Disposition_Date { get; set; }

        /// <summary> Date the physical version of this material was disposed of, post-digitization (for XML serialization)</summary>
        /// <remarks> This property is only exposed to allow for XML serialization of the nullable datetime </remarks>
        [IgnoreDataMember]
        [XmlAttribute("dispositionDate")]
        public DateTime Disposition_Date_XML
        {
            get { return Disposition_Date.HasValue ? Disposition_Date.Value : DateTime.MinValue; }
            set { if (value != DateTime.MinValue) Disposition_Date = value; }
        }

        /// <summary> Property controls if the associated property is serialized during XML serialization </summary>
        public bool ShouldSerializeDisposition_Date_XML
        {
            get { return Disposition_Date.HasValue; }
        }

        /// <summary> Constructor for a new instance of the Item_Tracking_Details class </summary>
        public Item_Tracking_Details()
        {
            WorkEvents = new List<Item_Tracking_Event>();
        }
    }
}
