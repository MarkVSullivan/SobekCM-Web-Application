#region Using directives

using System.Runtime.Serialization;

#endregion

namespace SobekCM_Rest_API.DisplayItem
{
    /// <summary>Designation of physical parts of a resource in a detailed form 
    /// (particularly in relation to the original host document or larger body of work)</summary>
    [DataContract]
    public class DisplayItem_SerialInfo
    {
        /// <summary> Text for the first level of enumeration </summary>
        [DataMember(EmitDefaultValue = false)]
        public string Enum1 { get; internal set; }

        /// <summary> Text for the second level of enumeration </summary>
        [DataMember(EmitDefaultValue = false)]
        public string Enum2 { get; internal set; }

        /// <summary> Text for the third level of enumeration </summary>
        [DataMember(EmitDefaultValue = false)]
        public string Enum3 { get; internal set; }

        /// <summary> Text for the fourth level of enumeration </summary>
        [DataMember(EmitDefaultValue = false)]
        public string Enum4 { get; internal set; }

        /// <summary> Text for the year/first level of chronology </summary>
        [DataMember(EmitDefaultValue = false)]
        public string Year { get; internal set; }

        /// <summary> Text for the month/second level of chronology </summary>
        [DataMember(EmitDefaultValue = false)]
        public string Month { get; internal set; }

        /// <summary> Text for the day/third level of chronology </summary>
        [DataMember(EmitDefaultValue = false)]
        public string Day { get; internal set; }

    }
}