#region Using directives

using System;
using System.IO;
using System.Text;
using System.Runtime.Serialization;

#endregion

namespace SobekCM_Rest_API.DisplayItem
{
    /// <summary> Subject information which includes cartographic data indicating spatial coverage </summary>
    [DataContract]
    public class DisplayItem_Subject_Cartographic
    {
        /// <summary> Language for this subject </summary>
        [DataMember(EmitDefaultValue = false)]
        public string language { get; internal set; }

        /// <summary> Uncontrolled authority term </summary>
        [DataMember(EmitDefaultValue = false)]
        public string authority { get; internal set; }

        /// <summary> Scale for this cartographic material </summary>
        [DataMember(EmitDefaultValue = false)]
        public string scale { get; internal set; }

        /// <summary> Simple coordinates for this cartographic material </summary>
        [DataMember(EmitDefaultValue = false)]
        public string coordinates { get; internal set; }

        /// <summary> Projection for this cartographic material </summary>
        [DataMember(EmitDefaultValue = false)]
        public string projection { get; internal set; }
    }
}