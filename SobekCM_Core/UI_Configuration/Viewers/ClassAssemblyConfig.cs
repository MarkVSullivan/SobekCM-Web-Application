using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.UI_Configuration.Viewers
{
    /// <summary> Represents a generic class / assembly for configuration purposes </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("classAssembly")]
    public class ClassAssemblyConfig
    {
        /// <summary> Fully qualified (including namespace) name of the class used 
        /// for this class/assembly information </summary>
        [DataMember(Name = "class")]
        [XmlAttribute("class")]
        [ProtoMember(1)]
        public string Class { get; set; }

        /// <summary> Name of the assembly within which this class resides, unless this
        /// is one of the default class/assembly included in the core code </summary>
        [DataMember(Name = "assembly", EmitDefaultValue = false)]
        [XmlAttribute("assembly")]
        [ProtoMember(2)]
        public string Assembly { get; set; }
    }
}
