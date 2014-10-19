#region Using directives

using System.Runtime.Serialization;

#endregion

namespace SobekCM_Rest_API.DisplayItem
{
    /// <summary> A geographic name given in a hierarchical form relating to the resource. </summary>
    [DataContract]
    public class DisplayItem_Subject_HierarchicalGeographic
    {
        /// <summary> Language for this subject </summary>
        [DataMember(EmitDefaultValue = false)]
        public string language { get; internal set; }

        /// <summary> Uncontrolled authority term </summary>
        [DataMember(EmitDefaultValue = false)]
        public string authority { get; internal set; }

        /// <summary> Gets or sets the continent level of this hierarchical geographic subject </summary>
        [DataMember(EmitDefaultValue = false)]
        public string continent { get; internal set; }

        /// <summary> Gets or sets the country level of this hierarchical geographic subject </summary>
        [DataMember(EmitDefaultValue = false)]
        public string country { get; internal set; }

        /// <summary> Gets or sets the province level of this hierarchical geographic subject </summary>
        [DataMember(EmitDefaultValue = false)]
        public string province { get; internal set; }

        /// <summary> Gets or sets the region level of this hierarchical geographic subject </summary>
        [DataMember(EmitDefaultValue = false)]
        public string region { get; internal set; }

        /// <summary> Gets or sets the state level of this hierarchical geographic subject </summary>
        [DataMember(EmitDefaultValue = false)]
        public string state { get; internal set; }

        /// <summary> Gets or sets the territory level of this hierarchical geographic subject </summary>
        [DataMember(EmitDefaultValue = false)]
        public string territory { get; internal set; }

        /// <summary> Gets or sets the county level of this hierarchical geographic subject </summary>
        [DataMember(EmitDefaultValue = false)]
        public string county { get; internal set; }

        /// <summary> Gets or sets the city level of this hierarchical geographic subject </summary>
        [DataMember(EmitDefaultValue = false)]
        public string city { get; internal set; }

        /// <summary> Gets or sets the city section level of this hierarchical geographic subject </summary>
        [DataMember(EmitDefaultValue = false)]
        public string citySection { get; internal set; }

        /// <summary> Gets or sets the island level of this hierarchical geographic subject </summary>
        [DataMember(EmitDefaultValue = false)]
        public string island { get; internal set; }

        /// <summary> Gets or sets the area level of this hierarchical geographic subject </summary>
        [DataMember(EmitDefaultValue = false)]
        public string area { get; internal set; }
    }
}