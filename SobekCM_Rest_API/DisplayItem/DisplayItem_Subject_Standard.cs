#region Using directives

using System.Collections.Generic;
using System.Runtime.Serialization;

#endregion

namespace SobekCM_Rest_API.DisplayItem
{
    /// <summary> Standard subject keywords for this item </summary>
    [DataContract]
    public class DisplayItem_Subject_Standard
    {
        /// <summary> Language for this subject </summary>
        [DataMember(EmitDefaultValue = false)]
        public string language { get; internal set; }

        /// <summary> Uncontrolled authority term </summary>
        [DataMember(EmitDefaultValue = false)]
        public string authority { get; internal set; }

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

        /// <summary> List of all the occupational subject keywords in this complex subject object </summary>
        [DataMember(EmitDefaultValue = false)]
        public List<string> occupations { get; internal set; }
    }
}