using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;
using SobekCM.Resource_Object.GenericXml.Reader;

namespace SobekCM.Resource_Object.GenericXml.Mapping
{
    /// <summary> A single path from a generic xml mapping set, which includes the full XML path 
    /// and instructions for a single mapping from the generic XML file to the SobekCM fields </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("genericXmlMappingPath")]
    public class GenericXmlMappingPath
    {
        /// <summary> Full XML path for the node data that should be mapped from a generic XML file to the SobekCM fields </summary>
        [DataMember(EmitDefaultValue = false, Name = "path")]
        [XmlElement("path")]
        [ProtoMember(1)]
        public GenericXmlPath XmlPath { get; set; }

        /// <summary> Instructions on how the data in the full XML path should be mapped into SobekCM fields </summary>
        [DataMember(EmitDefaultValue = false, Name = "instructions")]
        [XmlElement("instructions")]
        [ProtoMember(2)]
        public PathMappingInstructions Instructions { get; set; }

        /// <summary> Constructor for a new instance of the <see cref="GenericXmlMappingPath"/> class </summary>
        public GenericXmlMappingPath()
        {
            // Empty constructor
        }

        /// <summary> Constructor for a new instance of the <see cref="GenericXmlMappingPath"/> class </summary>
        public GenericXmlMappingPath(GenericXmlPath XmlPath)
        {
            this.XmlPath = XmlPath;
            Instructions = new PathMappingInstructions();
        }
    }
}
