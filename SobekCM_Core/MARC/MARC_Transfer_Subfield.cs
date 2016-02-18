using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.MARC
{
    /// <summary> Holds the data about a single subfield in a <see cref="MARC_Transfer_Field"/>. <br /> <br /> </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("marcSubfield")]
    public class MARC_Transfer_Subfield
    {
        /// <summary> Constructor for a new instance the MARC_Transfer_Subfield class </summary>
        /// <param name="Subfield_Code"> Code for this subfield in the MARC record </param>
        /// <param name="Data"> Data stored for this subfield </param>
        public MARC_Transfer_Subfield(char Subfield_Code, string Data)
        {
            // Save the parameters
            this.Subfield_Code = Subfield_Code;
            this.Data = Data;
        }

        /// <summary> Constructor for a new instance the MARC_Subfield class </summary>
        /// <remarks> This constructor does not initialize any of the values and is primarily
        /// added for serialization/deserialization purposes. </remarks>
        public MARC_Transfer_Subfield()
        {
            // Constructor does nothing - for serialization and deserialization purposes only
        }

        /// <summary> Gets the MARC subfield code associated with this data  </summary>
        [DataMember(EmitDefaultValue = false, Name = "code")]
        [XmlAttribute("code")]
        [ProtoMember(1)]
        public char Subfield_Code { get; set; }

        /// <summary> Gets the data associated with this MARC subfield  </summary>
        [DataMember(EmitDefaultValue = false, Name = "data")]
        [XmlAttribute("data")]
        [ProtoMember(2)]
        public string Data { get; set; }

        /// <summary> Returns this MARC Subfield as a string </summary>
        /// <returns> Subfield in format '|x data'.</returns>
        public override string ToString()
        {
            return "|" + Subfield_Code + " " + Data;
        }
    }
}