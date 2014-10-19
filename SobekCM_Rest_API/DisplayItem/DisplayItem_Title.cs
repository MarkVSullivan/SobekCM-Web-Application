#region Using directives

using System.Collections.Generic;
using System.Runtime.Serialization;

#endregion

namespace SobekCM_Rest_API.DisplayItem
{

    /// <summary>A word, phrase, character, or group of characters, normally appearing in a resource, that names it or the work contained in it.</summary>
    [DataContract]
    public class DisplayItem_Title
    {
        /// <summary> Display label for this title </summary>
        [DataMember(EmitDefaultValue = false)]
        public string displayLabel { get; internal set;  }

        /// <summary> Language for this title </summary>
        [DataMember(EmitDefaultValue = false)]
        public string language { get; internal set; }

        /// <summary> Title text for this title ( includes nonsort, title, subtitle, part numbers and part names ) </summary>
        [DataMember(EmitDefaultValue = false)]
        public string title { get; internal set; }

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