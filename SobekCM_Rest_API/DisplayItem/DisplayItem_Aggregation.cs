#region Using directives

using System;
using System.Runtime.Serialization;

#endregion

namespace SobekCM_Rest_API.DisplayItem
{
    /// <summary> Basic information (code and name) of aggregations linked to a digital object </summary>
    [DataContract]
    public class DisplayItem_Aggregation
    {
        /// <summary> Code associated with this aggregation </summary>
        [DataMember(EmitDefaultValue = false)]
        public string code { get; internal set; }

        /// <summary> Name associated with this aggregation </summary>
        [DataMember(EmitDefaultValue = false)]
        public string name { get; internal set; }
    }
}