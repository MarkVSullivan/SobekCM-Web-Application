using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core
{
    /// <summary> A single milestone entry in the history of an entity </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("milestone")]
    public class Milestone_Entry
    {
        /// <summary> Date this milestone entry was added </summary>
        [DataMember(Name = "date")]
        [XmlAttribute("date")]
        [ProtoMember(1)]
        public DateTime MilestoneDate { get; set; }

        /// <summary> User associated with this milestone </summary>
        [DataMember(EmitDefaultValue = false, Name = "user")]
        [XmlAttribute("user")]
        [ProtoMember(2)]
        public string User { get; set; }

        /// <summary> Notes (and milestone type) for this miletsone entry </summary>
        [DataMember(Name = "notes")]
        [XmlText]
        [ProtoMember(3)]
        public string Notes;

        /// <summary> Workflow name (if any) associated with this milestone </summary>
        /// <remarks> This is primarily used for item-related milestones </remarks>
        [DataMember(EmitDefaultValue = false, Name = "workflow")]
        [XmlAttribute("workflow")]
        [ProtoMember(4)]
        public string Workflow;

        /// <summary> Constructor for a new instance of the Milestone_Entry class </summary>
        public Milestone_Entry()
        {
            // Empty constructor for deserialization
        }

        /// <summary> Constructor for a new instance of the Milestone_Entry class </summary>
        /// <param name="MilestoneDate"> Date this milestone entry was added </param>
        /// <param name="User"> User associated with this milestone </param>
        /// <param name="Notes"> Notes (and milestone type) for this miletsone entry </param>
        public Milestone_Entry(DateTime MilestoneDate, string User, string Notes)
        {
            this.MilestoneDate = MilestoneDate;
            this.User = User;
            this.Notes = Notes;
        }

        /// <summary> Constructor for a new instance of the Milestone_Entry class </summary>
        /// <param name="MilestoneDate"> Date this milestone entry was added </param>
        /// <param name="User"> User associated with this milestone </param>
        /// <param name="Notes"> Notes (and milestone type) for this miletsone entry </param>
        /// <param name="Workflow">Constructor for a new instance of the Milestone_Entry class </param>
        public Milestone_Entry(DateTime MilestoneDate, string User, string Notes, string Workflow )
        {
            this.MilestoneDate = MilestoneDate;
            this.User = User;
            this.Notes = Notes;
            this.Workflow = Workflow;
        }
    }
}
