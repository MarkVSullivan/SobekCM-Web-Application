#region Using directives

using System;
using System.Runtime.Serialization;

#endregion

namespace SobekCM.Core.Results
{
    /// <summary> A single private item, used for display of all private items within 
    /// an item aggreation, from the internal header </summary>
    [DataContract]
    public class Private_Items_List_Item
    {
        /// <summary> Volume identifier (VID) for this private item </summary>
        [DataMember]
        public string VID { get; set; }

        /// <summary>Title for this private item </summary>
        [DataMember]
        public string Title { get; set; }

        /// <summary>Creator for this private item </summary>
        [DataMember]
        public string Creator { get; set; }

        /// <summary>Internal comments for this private item </summary>
        [DataMember(EmitDefaultValue = false)]
        public string Internal_Comments { get; set; }

        /// <summary>Publication date string for this private item </summary>
        [DataMember(EmitDefaultValue = false)]
        public string PubDate { get; set; }

        /// <summary>Flag indicates if this private item has been locally archived </summary>
        [DataMember]
        public bool LocallyArchived { get; set; }

        /// <summary>Flag indicates if this private item has been remotely archived </summary>
        [DataMember]
        public bool RemotelyArchived { get; set; }

        /// <summary>Aggregation codes for this private item ( i.e., 'dloc,cndl,fdnl') </summary>
        [DataMember]
        public string AggregationCodes { get; set; }

        /// <summary>Last date any activity occurred for this private item </summary>
        [DataMember]
        public DateTime LastActivityDate { get; set; }

        /// <summary> Last activity type which occurred for this item </summary>
        [DataMember]
        public string LastActivityType { get; set; }

        /// <summary>Last milestone date for this private item </summary>
        [DataMember]
        public DateTime LastMilestoneDate { get; set; }

        /// <summary> Last milestone number which occurred for this item </summary>
        [DataMember]
        public int LastMilestone { get; set; }

        /// <summary> Embargo date (if one exists) for this item </summary>
        [DataMember(EmitDefaultValue = false)]
        public DateTime? EmbargoDate { get; set; }

        /// <summary> Last milestone which occurred for this item, as a string </summary>
        public string Last_Milestone_String
        {
            get
            {
                switch (LastMilestone)
                {
                    case 0:
                        return "record created";

                    case 1:
                        return "scanned";

                    case 2:
                        return "processed";

                    case 3:
                        return "quality control";

                    case 4:
                        return "online completed";

                    default:
                        return "unknown milestone";                
                }
            }
        }
    }
}
