using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Resource_Object.GenericXml.Reader
{
    /// <summary> A single node from a generic XML file, used while reading the file and also
    /// for indicating mapping from a generic XML file to the SobekCM metadata fields </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("genericXmlNode")]
    public class GenericXmlNode
    {
        /// <summary> Name (xml tag) of this node in a generic XML file </summary>
        [DataMember(EmitDefaultValue = false, Name = "name")]
        [XmlAttribute("name")]
        [ProtoMember(1)]
        public string NodeName { get; set; }

        /// <summary> Name of any key attribute which indicates mapping should be different for this node </summary>
        /// <remarks> This is utilized if the value of an attribute affects the way the overall node is mapped </remarks>
        [DataMember(EmitDefaultValue = false, Name = "keyAttributeName")]
        [XmlAttribute("keyAttributeName")]
        [ProtoMember(2)]
        public string KeyAttributeName { get; set; }

        /// <summary> Value of any key attribute which indicates mapping should be different for this node </summary>
        /// <remarks> This is utilized if the value of an attribute affects the way the overall node is mapped </remarks>
        [DataMember(EmitDefaultValue = false, Name = "keyAttributeValue")]
        [XmlAttribute("keyAttributeValue")]
        [ProtoMember(3)]
        public string KeyAttributeValue { get; set; }

        /// <summary> Constructor for a new instance of the GenericXmlNode class </summary>
        public GenericXmlNode()
        {
            // Do nothing currently
        }

        /// <summary> Overrides the default method and returns a <see cref="System.String" /> that represents this instance </summary>
        public override string ToString()
        {
            // If there is no key attribute indicated, just return the node name
            if (String.IsNullOrEmpty(KeyAttributeName))
                return NodeName;

            // If no value was indicates, but an attribute name was included, this is a wildcard
            if (String.IsNullOrEmpty(KeyAttributeValue))
                return NodeName + " (" + KeyAttributeValue + "=*)";

            // Return the key value and path
            return NodeName + " (" + KeyAttributeValue + "=" + KeyAttributeName + ")";

        }
    }
}
