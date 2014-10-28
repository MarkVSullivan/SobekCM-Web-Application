#region Using directives

using System.Collections.Generic;
using System.Runtime.Serialization;

#endregion

namespace SobekCM_Rest_API.DisplayItem
{
    /// <summary>The name of a person, organization, or event (conference, meeting, etc.) associated in some way with the resource. </summary>
    [DataContract]
    public class DisplayItem_Name
    {
        /// <summary> Dates associated with this named entity </summary>
        [DataMember(EmitDefaultValue = false)]
        public string dates;

        /// <summary> Actual name for this named entity </summary>
        [DataMember(EmitDefaultValue = false)]
        public string name;

        /// <summary> Type of the name ( personal, corporate, conference ) </summary>
        [DataMember(EmitDefaultValue = false)]
        public string type;

        /// <summary> List of textual roles </summary>
        [DataMember(EmitDefaultValue = false)]
        public List<string> roles;

        /// <summary> Key for this named entity within the local authority system </summary>
        [DataMember(EmitDefaultValue = false)]
        public string key;
    }
}