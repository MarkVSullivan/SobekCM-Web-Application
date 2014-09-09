#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization;

#endregion

namespace SobekCM_Rest_API.DisplayItem
{
    /// <summary>physicalDescription is a wrapper element that contains all subelements relating to 
    /// physical description information of the resource described. Data is input only within each 
    /// subelement. </summary>
    [DataContract]
    public class DisplayItem_PhysicalDescription
    {
        /// <summary> A statement of the number and specific material of the units of the resource that express physical extent </summary>
        [DataMember(EmitDefaultValue = false)]
        public string extent { get; internal set; }

        /// <summary> A designation of a particular physical presentation of a resource, including the physical 
        /// form or medium of material for a resource </summary>
        [DataMember(EmitDefaultValue = false)]
        public List<DisplayItem_PhysicalForm> form { get; internal set; }

        /// <summary> Any notes related to the physical description of the item  </summary>
        [DataMember(EmitDefaultValue = false)]
        public List<string> notes { get; internal set; }
    }
}