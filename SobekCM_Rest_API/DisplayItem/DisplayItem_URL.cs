#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

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