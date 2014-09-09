#region Using directives

using System;
using System.IO;
using System.Runtime.Serialization;

#endregion

namespace SobekCM_Rest_API.DisplayItem
{
    /// <summary>Stores the information about any 'abstracts' associated with this digital resource. </summary>
    [DataContract]
    public class DisplayItem_Abstract
    {
        /// <summary> Gets or sets the language of the abstract </summary>
        /// <remarks>This is an uncontrolled field</remarks>
        [DataMember(EmitDefaultValue = false)]
        public string language { get; internal set; }

        /// <summary> Gets or sets the display label for the abstract </summary>
        /// <remarks>This attribute is intended to be used when additional text associated with the abstract is necessary for display. </remarks>
        [DataMember(EmitDefaultValue = false)]
        public string displayLabel { get; internal set; }

        /// <summary> Gets or sets the uncontrolled type for the abstract </summary>
        /// <remarks>There is no controlled list of abstract types.   Suggested values are: subject, review, scope and content, content advice</remarks>
        [DataMember(EmitDefaultValue = false)]
        public string type { get; internal set; }

        /// <summary> Text of this abstract </summary>
        [DataMember(EmitDefaultValue = false)]
        public string text { get; internal set; }
    }
}