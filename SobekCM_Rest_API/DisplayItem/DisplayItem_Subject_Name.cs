#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.Serialization;

#endregion

namespace SobekCM_Rest_API.DisplayItem
{
    /// <summary> Name used as a subject for a digital resource </summary>
    [DataContract]
    public class DisplayItem_Subject_Name
    {
        /// <summary> Language for this subject </summary>
        [DataMember(EmitDefaultValue = false)]
        public string language { get; internal set; }

        /// <summary> Uncontrolled authority term </summary>
        [DataMember(EmitDefaultValue = false)]
        public string authority { get; internal set; }

        /// <summary> Dates associated with this named entity </summary>
        [DataMember(EmitDefaultValue = false)]
        public string dates;

        /// <summary> Actual name for this named entity </summary>
        [DataMember(EmitDefaultValue = false)]
        public string name;

        /// <summary> Type of the name ( personal, corporate, conference ) </summary>
        [DataMember(EmitDefaultValue = false)]
        public string type;

        /// <summary> List of textual roles </summary>
        [DataMember(EmitDefaultValue = false)]
        public List<string> roles;

        /// <summary> Key for this named entity within the local authority system </summary>
        [DataMember(EmitDefaultValue = false)]
        public string key;

        /// <summary> List of all the genre subject keywords in this complex subject object </summary>
        [DataMember(EmitDefaultValue = false)]
        public List<string> genres { get; internal set; }

        /// <summary> List of all the geographic subject keywords in this complex subject object </summary>
        [DataMember(EmitDefaultValue = false)]
        public List<string> geographics { get; internal set; }

        /// <summary> List of all the temporal subject keywords in this complex subject object </summary>
        [DataMember(EmitDefaultValue = false)]
        public List<string> temporals { get; internal set; }

        /// <summary> List of all the topical subject keywords in this complex subject object </summary>
        [DataMember(EmitDefaultValue = false)]
        public List<string> topics { get; internal set; }

    }
}