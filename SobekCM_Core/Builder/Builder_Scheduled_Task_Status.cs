
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.Builder
{
    /// <summary> Basic information about a scheduled task for the builder </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("scheduledTask")]
    public class Builder_Scheduled_Task_Status
    {

        /// <summary> Primary key for the related builder module set </summary>
        [DataMember(Name = "id", EmitDefaultValue = false)]
        [XmlAttribute("id")]
        [ProtoMember(1)]
        public int ModuleScheduleID { get; set; }

        /// <summary> Name of the related set of builder modules </summary>
        [DataMember(Name = "description", EmitDefaultValue = false)]
        [XmlAttribute("description")]
        [ProtoMember(2)]
        public string Description { get; set; }

        /// <summary> Flag indicates if this is enabled </summary>
        /// <remarks> This is a combination of the SET enabled flag and the scheduled TASK enabled flag </remarks>
        [DataMember(Name = "enabled", EmitDefaultValue = false)]
        [XmlAttribute("enabled")]
        [ProtoMember(3)]
        public bool Enabled { get; set; }

        /// <summary> Days of the week this process should run ( in the format 'MWF' for example )</summary>
        [DataMember(Name = "days", EmitDefaultValue = false)]
        [XmlAttribute("days")]
        [ProtoMember(4)]
        public string DaysOfWeek { get; set; }

        /// <summary> Time(s) of the day this process should run ( in the format '1400' for 2pm server time ) </summary>
        [DataMember(Name = "time", EmitDefaultValue = false)]
        [XmlAttribute("time")]
        [ProtoMember(5)]
        public string TimesOfDay { get; set; }

        /// <summary> Information about the set of builder modules </summary>
        [DataMember(Name = "moduleSet", EmitDefaultValue = false)]
        [XmlElement("moduleSet")]
        [ProtoMember(6)]
        public Builder_Module_Set_Info ModuleSet { get; set; }

        /// <summary> Information on the last time this scheduled task ran </summary>
        [DataMember(Name = "lastRun", EmitDefaultValue = false)]
        [XmlElement("lastRun")]
        [ProtoMember(7)]
        public Builder_Scheduled_Task_Execution_History LastRun { get; set; } 
    }
}
