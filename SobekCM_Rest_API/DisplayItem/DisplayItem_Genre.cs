#region Using directives

using System.Runtime.Serialization;

#endregion

namespace SobekCM_Rest_API.DisplayItem
{
    /// <summary>Stores the information about any genre associated with this digital resource. </summary>
    [DataContract]
    public class DisplayItem_Genre
    {
        /// <summary> Gets or sets this genre term </summary>
        [DataMember(EmitDefaultValue = false)]
        public string term { get; internal set; }

        /// <summary> Gets or sets this genre term </summary>
        /// <remarks> There is no controlled list for this.  In general <i>marcgt</i> is used to represent controlled genres related to leader and 006/008 fields from the MARC record</remarks>
        [DataMember(EmitDefaultValue = false)]
        public string authority { get; internal set; }

        /// <summary> Gets or sets the language for this genre term </summary>
        [DataMember(EmitDefaultValue = false)]
        public string language { get; internal set; }
    }
}