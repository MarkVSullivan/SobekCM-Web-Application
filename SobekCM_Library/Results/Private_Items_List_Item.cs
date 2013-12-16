#region Using directives

using System;

#endregion

namespace SobekCM.Library.Results
{
    /// <summary> A single private item, used for display of all private items within 
    /// an item aggreation, from the internal header </summary>
    public class Private_Items_List_Item
    {
        /// <summary> Volume identifier (VID) for this private item </summary>
        public string VID { get; set; }

        /// <summary>Title for this private item </summary>
        public string Title { get; set; }

        /// <summary>Internal comments for this private item </summary>
        public string Internal_Comments { get; set; }

        /// <summary>Publication date string for this private item </summary>
        public string PubDate { get; set; }

        /// <summary>Flag indicates if this private item has been locally archived </summary>
        public bool Locally_Archived { get; set; }

        /// <summary>Flag indicates if this private item has been remotely archived </summary>
        public bool Remotely_Archived { get; set; }

        /// <summary>Aggregation codes for this private item ( i.e., 'dloc,cndl,fdnl') </summary>
        public string Aggregation_Codes { get; set; }

        /// <summary>Last date any activity occurred for this private item </summary>
        public DateTime Last_Activity_Date { get; set; }

        /// <summary> Last activity type which occurred for this item </summary>
        public string Last_Activity_Type { get; set; }

        /// <summary>Last milestone date for this private item </summary>
        public DateTime Last_Milestone_Date { get; set; }

        /// <summary> Last milestone number which occurred for this item </summary>
        public int Last_Milestone { get; set; }

        /// <summary> Last milestone which occurred for this item, as a string </summary>
        public string Last_Milestone_String
        {
            get
            {
                switch (Last_Milestone)
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
