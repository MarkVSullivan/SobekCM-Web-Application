using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SobekCM.Bib_Package.Tracking
{
    /// <summary> Class represents a single worklog history / progress entry for this digital resource </summary>
    [Serializable]
    public class Tracking_Progress
    {
        /// <summary> Name of the workflow for this single worklog history / progress entry </summary>
        public readonly string Workflow_Name;

        /// <summary> Personal name, username, or vendor name for the party that performed this work </summary>
        public readonly string Work_Performed_By;

        /// <summary> Filepath where this work occurred </summary>
        public readonly string FilePath;

        /// <summary> Any associated note for this single worklog history / progress entry </summary>
        public readonly string Note;

        /// <summary> Date that this worklog histroy / progress was completed </summary>
        public readonly Nullable<DateTime> Completed_Date;

        /// <summary> Constructor for a new instance of the Tracking_Progress class </summary>
        /// <param name="Workflow_Name">Name of the workflow for this single worklog history / progress entry</param>
        /// <param name="Work_Performed_By">Personal name, username, or vendor name for the party that performed this work</param>
        /// <param name="FilePath">Filepath where this work occurred</param>
        /// <param name="Note">Any associated note for this single worklog history / progress entry</param>
        /// <param name="Completed_Date">Date that this worklog histroy / progress was completed</param>
        public Tracking_Progress(string Workflow_Name, string Work_Performed_By, string FilePath, string Note, Nullable<DateTime> Completed_Date)
        {
            this.Workflow_Name = Workflow_Name;
            this.Work_Performed_By = Work_Performed_By;
            this.FilePath = FilePath;
            this.Note = Note;
            this.Completed_Date = Completed_Date;
        }
    }
}
