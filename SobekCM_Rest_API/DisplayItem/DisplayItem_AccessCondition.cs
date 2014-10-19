#region Using directives

using System.Runtime.Serialization;

#endregion

namespace SobekCM_Rest_API.DisplayItem
{
    /// <summary>Stores the information about any access conditions or rights associated with this digital resource. </summary>
    [DataContract]
    public class DisplayItem_AccessCondition 
    {
        /// <summary>Gets or set information about restrictions imposed on access to a resource</summary>
        [DataMember(EmitDefaultValue = false)]
        public string Text { get; set; }

        /// <summary>Gets or sets the uncontrolled type of access condition.</summary>
        /// <remarks>There is no controlled list of types for accessCondition defined.  Suggested values are: restriction on access, and use and reproduction.</remarks>
        [DataMember(EmitDefaultValue = false)]
        public string Type { get; set; }

        /// <summary>Gets or sets the additional text associated with the access conditions which is necessary for display.</summary>
        [DataMember(EmitDefaultValue = false)]
        public string Display_Label { get; set; }

        /// <summary> Gets or sets the language of this accessCondition text </summary>
        [DataMember(EmitDefaultValue = false)]
        public string Language { get; set; }
    }
}