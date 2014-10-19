#region Using directives

using System.Collections.Generic;
using System.Runtime.Serialization;

#endregion

namespace SobekCM_Rest_API.DisplayItem
{
    /// <summary> Class contains the information about the publisher of a resource </summary>
    [DataContract]
    public class DisplayItem_Publisher
    {
        /// <summary> Name of this publisher-like entity  </summary>
        [DataMember(EmitDefaultValue = false)]
        public string name { get; internal set;  }

        /// <summary> List of places associated with this publisher </summary>
        [DataMember(EmitDefaultValue = false)]
        public List<DisplayItem_OriginPlace> places { get; internal set; }

        /// <summary> Key for this named entity within the local authority system </summary>
        [DataMember(EmitDefaultValue = false)]
        public string key { get; internal set; }

    }
}