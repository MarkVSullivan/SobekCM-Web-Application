#region Using directives

using System.Runtime.Serialization;

#endregion

namespace SobekCM_Rest_API.DisplayItem
{
    /// <summary> Contains information about a single origination location (publication place) for an item </summary>
    [DataContract]
    public class DisplayItem_OriginPlace
    {
        /// <summary> Text of the publication place </summary>
        [DataMember(EmitDefaultValue = false)]
        public string text { get; internal set; }

        /// <summary> ISO-3166 code for the publication place </summary>
        [DataMember(EmitDefaultValue = false)]
        public string isoCode { get; internal set; }
    }
}
