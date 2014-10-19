#region Using directives

using System.Runtime.Serialization;

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