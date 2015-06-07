using System;


namespace SobekCM.Core
{
    /// <summary> A single milestone entry in the history of an entity </summary>
    public class Milestone_Entry
    {
        /// <summary> Date this milestone entry was added </summary>
        public DateTime MilestoneDate { get; set; }

        /// <summary> User associated with this milestone </summary>
        public string User { get; set; }

        /// <summary> Notes (and milestone type) for this miletsone entry </summary>
        public string Notes;

        /// <summary> Workflow name (if any) associated with this milestone </summary>
        /// <remarks> This is primarily used for item-related milestones </remarks>
        public string Workflow;
    }
}
