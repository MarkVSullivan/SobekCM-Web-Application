#region Using directives

using System;
using System.IO;
using System.Runtime.Serialization;

#endregion

namespace SobekCM_Rest_API.DisplayItem
{
    /// <summary>A designation of the language in which the content of a resource is expressed</summary>
    [DataContract]
    public class DisplayItem_Language 
    {
        /// <summary> Gets or sets the Iso639-2b code for this language </summary>
        [DataMember(EmitDefaultValue = false)]
        public string isoCode { get; set; }

        /// <summary> Gets or sets the Rfc3066 code for this language  </summary>
        [DataMember(EmitDefaultValue = false)]
        public string rfcCode { get; set; }

        /// <summary> Gets or sets the Language term for this language  </summary>
        [DataMember(EmitDefaultValue = false)]
        public string language { get; set; }
    }
}