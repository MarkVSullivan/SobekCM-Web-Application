#region Using directives

using System;
using System.IO;
using System.Runtime.Serialization;

#endregion

namespace SobekCM_Rest_API.DisplayItem
{
    /// <summary>Stores the information about any classifications associated with this digital resource. </summary>
    [DataContract]
    public class DisplayItem_Classification
    {
        /// <summary> Gets or sets this classification term </summary>
        [DataMember(EmitDefaultValue = false)]
        public string Classification { get; internal set; }

        /// <summary> Gets or sets the display label for this classification term </summary>
        [DataMember(EmitDefaultValue = false)]
        public string Display_Label { get; internal set; }

        /// <summary> Gets or sets the authority for this classification term </summary>
        [DataMember(EmitDefaultValue = false)]
        public string Authority { get; internal set; }
    }
}