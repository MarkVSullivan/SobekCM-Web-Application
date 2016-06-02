using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Resource_Object.GenericXml.Mapping
{
    /// <summary> Instructions on how a related path should be handled when mapped into a SobekCM field </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("pathMappingInstructions")]
    public class PathMappingInstructions
    {
        /// <summary> Flag indicates if the inner XML tags should be retained, or stripped out </summary>
        [DataMember(EmitDefaultValue = false, Name = "retainInnerXmlTags")]
        [XmlAttribute("retainInnerXmlTags")]
        [ProtoMember(1)]
        public bool RetainInnerXmlTags { get; set; }

        /// <summary> Name of the metadata field (or subfield) to which the data within this path
        /// shouold be mapped within the world of SobekCM metadata </summary>
        [DataMember(EmitDefaultValue = false, Name = "mapping")]
        [XmlAttribute("mapping")]
        [ProtoMember(2)]
        public string SobekMapping { get; set; }

        /// <summary> Flag indicates to completely ignore all of the subtree, usually used when
        /// the entire contents of this element is mapped into a single field, with no additional 
        /// analysis required, like mapping the text/body field into full text from a TEI file </summary>
        [DataMember(EmitDefaultValue = false, Name = "ignoreSubTree")]
        [XmlAttribute("ignoreSubTree")]
        [ProtoMember(3)]
        public bool IgnoreSubTree { get; set; }

        /// <summary> Constructor for a new instance of the <see cref="PathMappingInstructions"/> class </summary>
        public PathMappingInstructions()
        {
            RetainInnerXmlTags = false;
            SobekMapping = "Unknown";
            IgnoreSubTree = false;
        }
    }
}
