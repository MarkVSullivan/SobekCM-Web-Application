#region Using directives

using System;
using System.IO;
using System.Text;
using System.Runtime.Serialization;

#endregion

namespace SobekCM_Rest_API.DisplayItem
{
    /// <summary>Stores the information about any 'affiliations' associated with this digital resource. </summary>
    [DataContract]
    public class DisplayItem_Affiliation
    {
        /// <summary> Gets or sets the affiliation term associated with this resource </summary>
        /// <remarks>This can be used rather than the entire hierarchy of the other members of this class.</remarks>
        [DataMember(EmitDefaultValue = false)]
        public string term { get; internal set; }

        /// <summary> Gets or sets the university listed in the affililation hierarchy for this resource </summary>
        [DataMember(EmitDefaultValue = false)]
        public string university { get; internal set; }

        /// <summary> Gets or sets the campus listed in the affililation hierarchy for this resource </summary>
        [DataMember(EmitDefaultValue = false)]
        public string campus { get; internal set; }

        /// <summary> Gets or sets the college listed in the affililation hierarchy for this resource </summary>
        [DataMember(EmitDefaultValue = false)]
        public string college { get; internal set; }

        /// <summary> Gets or sets the unit listed in the affililation hierarchy for this resource </summary>
        [DataMember(EmitDefaultValue = false)]
        public string unit { get; internal set; }

        /// <summary> Gets or sets the department listed in the affililation hierarchy for this resource </summary>
        [DataMember(EmitDefaultValue = false)]
        public string department { get; internal set; }

        /// <summary> Gets or sets the institute listed in the affililation hierarchy for this resource </summary>
        [DataMember(EmitDefaultValue = false)]
        public string institute { get; internal set; }

        /// <summary> Gets or sets the center listed in the affililation hierarchy for this resource </summary>
        [DataMember(EmitDefaultValue = false)]
        public string center { get; internal set; }

        /// <summary> Gets or sets the section listed in the affililation hierarchy for this resource </summary>
        [DataMember(EmitDefaultValue = false)]
        public string section { get; internal set; }

        /// <summary> Gets or sets the subsection listed in the affililation hierarchy for this resource </summary>
        [DataMember(EmitDefaultValue = false)]
        public string subSection { get; internal set; }

        /// <summary> Key for this affiliation within the local authority system </summary>
        [DataMember(EmitDefaultValue = false)]
        public string key;
    }
}