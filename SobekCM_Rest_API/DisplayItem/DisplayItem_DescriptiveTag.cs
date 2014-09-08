#region Using directives

using System;
using System.Runtime.Serialization;

#endregion

namespace SobekCM_Rest_API.DisplayItem
{
    /// <summary> Class holds the information about a single user-entered descriptive tag in relation to this digital object </summary>
    [DataContract]
    public class DisplayItem_DescriptiveTag
    {
        /// <summary> Primary key for this tag from the database </summary>
        [DataMember]
        public int TagID { get; set; }

        /// <summary> Date the tag was added or last modified </summary>
        [DataMember]
        public DateTime Date_Added { get; set; }

        /// <summary> Primary key to the user who entered this tag </summary>
        [DataMember]
        public int UserID { get; internal set; }

        /// <summary> Name of the user who entered this tag in the format 'Last Name, First Name' </summary>
        [DataMember]
        public string UserName { get; internal set; }

        /// <summary> Text of the user-entered descriptive tag </summary>
        [DataMember]
        public string Description_Tag { get; internal set; }
    }
}