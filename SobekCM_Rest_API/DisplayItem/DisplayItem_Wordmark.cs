#region Using directives

using System.Runtime.Serialization;

#endregion

namespace SobekCM_Rest_API.DisplayItem
{
    /// <summary> Class represents the information about a single wordmark </summary>
    [DataContract]
    public class DisplayItem_Wordmark
    {
        /// <summary> Name of the image file for this wordmark, from the database </summary>
        [DataMember(EmitDefaultValue = false)]
        public string image { get; internal set; }

        /// <summary> Link related to this wordmark </summary>
        [DataMember(EmitDefaultValue = false)]
        public string link { get; internal set; }

        /// <summary> Title of this wordmark, can be displayed as hover-over text </summary>
        [DataMember(EmitDefaultValue = false)]
        public string title { get; internal set; }
    }
}