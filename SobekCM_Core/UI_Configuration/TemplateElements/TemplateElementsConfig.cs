using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.UI_Configuration.TemplateElements
{
    /// <summary> Configuration class handles the mapping between all of the
    /// type/subtype attributes in each individual template cofiguration file
    /// and the actual classes to render that element in the online metadata forms </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("TemplateElementsConfig")]
    public class TemplateElementsConfig
    {
        /// <summary> Constructor for a new instance of the <see cref="TemplateElementsConfig"/> class </summary>
        public TemplateElementsConfig()
        {
            Elements = new List<TemplateElement>();
        }

        /// <summary> Collection of all the template elements </summary>
        [DataMember(Name = "elements", EmitDefaultValue = false)]
        [XmlArray("elements")]
        [XmlArrayItem("element", typeof(TemplateElement))]
        [ProtoMember(1)]
        public List<TemplateElement> Elements { get; set; } 
    }
}
