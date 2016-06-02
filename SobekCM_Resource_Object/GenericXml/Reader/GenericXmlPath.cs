using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Resource_Object.GenericXml.Reader
{
    /// <summary> A single path (similar to XPath) from either an existing XML file, or from a
    /// generic XML mapping set </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("genericXmlPath")]
    public class GenericXmlPath
    {
        /// <summary> The name of this attribute, if this path represents a path to a single attribute 
        /// within an XML element </summary>
        [DataMember(EmitDefaultValue = false, Name = "attributeName")]
        [XmlAttribute("attributeName")]
        [ProtoMember(1)]
        public string AttributeName { get; set; }

        /// <summary> Hierarchical list of the path of the nodes through the XML hierarchy to this node </summary>
        [DataMember(EmitDefaultValue = false, Name = "nodes")]
        [XmlArray("nodes")]
        [XmlArrayItem("node", typeof(GenericXmlNode))]
        [ProtoMember(2)]
        public List<GenericXmlNode> PathNodes { get; set; }

        /// <summary> Constructor for a new instance of the <see cref="GenericXmlPath"/> class </summary>
        public GenericXmlPath()
        {
            PathNodes = new List<GenericXmlNode>();
        }

        /// <summary> Overrides the default method and returns a <see cref="System.String" /> that represents this instance </summary>
        public override string ToString()
        {
            // Build the string for this
            StringBuilder builder = new StringBuilder();

            // Add each path node
            bool isFirst = true;
            foreach (GenericXmlNode pathNode in PathNodes)
            {
                if (isFirst)
                {
                    builder.Append(pathNode);
                    isFirst = false;
                }
                else
                {
                    builder.Append(" > " + pathNode);
                }
            }

            // If this includes an attribute, include that as well here at the end
            if (!String.IsNullOrEmpty(AttributeName))
                builder.Append("." + AttributeName);

            // Return the built string
            return builder.ToString();
        }
    }
}
