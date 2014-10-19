#region Using directives

using System.Runtime.Serialization;

#endregion

namespace SobekCM_Rest_API.DisplayItem
{
    /// <summary> URL information </summary>
    [DataContract]
    public class DisplayItem_URL
    {
        /// <summary> Gets or sets the an additional link </summary>
        [DataMember(EmitDefaultValue = false)]
        public string url;

        /// <summary> Gets or sets the display information for the additional link </summary>
        [DataMember(EmitDefaultValue = false)]
        public string label;

        /// <summary> Gets or sets the note for the additional link </summary>
        [DataMember(EmitDefaultValue = false)]
        public string notes;
    }
}