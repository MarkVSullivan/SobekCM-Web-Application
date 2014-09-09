#region Using directives

using System;
using System.IO;
using System.Runtime.Serialization;

#endregion

namespace SobekCM_Rest_API.DisplayItem
{
    /// <summary>Stores information about a single identifier associated with a digital resource </summary>
    /// <remarks>"identifier" contains a unique standard number or code that distinctively identifies a resource. 
    /// It includes manifestation, expression and work level identifiers. This should be repeated for each applicable identifier recorded, including invalid and canceled identifiers. </remarks>
    [DataContract]
    public class DisplayItem_Identifier
    {
        /// <summary> Gets or sets the identifier term for this identifier </summary>
        [DataMember(EmitDefaultValue = false)]
        public string identifier { get; internal set; }

        /// <summary> Gets or sets the uncontrolled type of this identifier </summary>
        /// <remarks>There is no controlled list of identifier types</remarks>
        [DataMember(EmitDefaultValue = false)]
        public string type { get; internal set; }

        /// <summary> Gets or sets the additional text associated with the identifier which is necessary for display. </summary>
        [DataMember(EmitDefaultValue = false)]
        public string displayLabel { get; internal set; }
    }
}