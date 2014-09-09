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
    /// <summary> Institution ( Holding location or source ) information </summary>
    [DataContract]
    public class DisplayItem_Institution
    {
        /// <summary> System code for this holding location </summary>
        [DataMember(EmitDefaultValue = false)]
        public string code;

        /// <summary> Name of the holding location institution ( may contain physical location information as well ) </summary>
        [DataMember(EmitDefaultValue = false)]
        public string name; 
    }
}