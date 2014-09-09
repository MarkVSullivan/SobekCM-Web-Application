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
    [DataContract]
    public class DisplayItem_CreationInfo
    {
        /// <summary> Date this item was first added to the SobekCM library </summary>
        [DataMember(EmitDefaultValue = false)]
        public DateTime? creationDate { get; set;  }

        /// <summary> Name of the individual who first created this resource  </summary>
        [DataMember(EmitDefaultValue = false)]
        public string creatorName { get; set; }

        /// <summary> Name of the organization which was responsible for the original creation of this resource </summary>
        [DataMember(EmitDefaultValue = false)]
        public string creatorOrganization { get; set; }

        /// <summary> Name of the software which originally created the metadata files </summary>
        [DataMember(EmitDefaultValue = false)]
        public string creatorSoftware { get; set; }

        /// <summary> Date this item was last modified within this SobekCM library </summary>
        [DataMember(EmitDefaultValue = false)]
        public DateTime? lastModifyDate;      
    }
}