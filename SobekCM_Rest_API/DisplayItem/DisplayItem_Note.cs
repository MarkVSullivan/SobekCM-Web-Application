#region Using directives

using System;
using System.IO;
using System.Runtime.Serialization;

#endregion

namespace SobekCM_Rest_API.DisplayItem
{
    /// <summary> General textual information relating to a resource</summary>
    [DataContract]
    public class DisplayItem_Note
    {
        /// <summary> Gets or sets the text of the note </summary>
        [DataMember(EmitDefaultValue = false)]
        public string note { get; internal set; }

        /// <summary> Gets or sets the type of the note </summary>
        [DataMember(EmitDefaultValue = false)]
        public string type { get; internal set; }

        /// <summary> Gets or sets the any additional information needed for display of the note </summary>
        [DataMember(EmitDefaultValue = false)]
        public string displayLabel  { get; internal set; }
    }
}