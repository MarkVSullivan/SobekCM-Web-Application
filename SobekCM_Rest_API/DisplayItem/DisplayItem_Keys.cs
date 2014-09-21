#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;


#endregion

namespace SobekCM_Rest_API.DisplayItem
{
    /// <summary> Primary keys to this item/group within this SobekCM database </summary>
    [DataContract]
    public class DisplayItem_Keys
    {
        /// <summary> Primary key for this group from the SobekCM Web database </summary>
        [DataMember(EmitDefaultValue = false)]
        public int? groupId { get; internal set; }

        /// <summary> Primary key for this item from the SobekCM Web database </summary>
        [DataMember(EmitDefaultValue = false)]
        public int? itemId { get; internal set; } 
    }
}