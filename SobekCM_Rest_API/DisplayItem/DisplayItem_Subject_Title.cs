#region Using directives

using System.Collections.Generic;
using System.Runtime.Serialization;

#endregion

namespace SobekCM_Rest_API.DisplayItem
{
    /// <summary> Title used as a subject for a digital resource </summary>
    [DataContract]
    public class DisplayItem_Subject_Title
    {
        /// <summary> Language for this subject </summary>
        [DataMember(EmitDefaultValue = false)]
        public string language { get; internal set; }

        /// <summary> Uncontrolled authority term </summary>
        [DataMember(EmitDefaultValue = false)]
        public string authority { get; internal set; }

        /// <summary> Title text for this title ( includes nonsort, title, subtitle, part numbers and part names ) </summary>
        [DataMember(EmitDefaultValue = false)]
        public string title { get; internal set; }

        /// <summary> Type of this title ( abbreviated, alternative, translated, uniform, serial ) </summary>
        [DataMember(EmitDefaultValue = false)]
        public string type { get; internal set; }

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