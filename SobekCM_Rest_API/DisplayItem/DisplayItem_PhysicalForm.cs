#region Using directives

using System;
using System.Runtime.Serialization;

#endregion

namespace SobekCM_Rest_API.DisplayItem
{
    /// <summary> Stores information about the Designation of a particular presentation of a resource, including the physical form or medium of material for a resource </summary>
    [DataContract]
    public class DisplayItem_PhysicalForm
    {
        /// <summary> Record the form of the digitized and analog original resources if such information will be useful</summary>
        /// <remarks> The form element should be used to characterize the resource itself rather than the content of the resource, which would be genre. </remarks>
        [DataMember(EmitDefaultValue = false)]
        public string form { get; internal set; }

        /// <summary> May be used to specify whether the form concerns materials or techniques ( i.e., 'materials', 'techniques' ) </summary>
        [DataMember(EmitDefaultValue = false)]
        public string type { get; internal set; }
    }
}