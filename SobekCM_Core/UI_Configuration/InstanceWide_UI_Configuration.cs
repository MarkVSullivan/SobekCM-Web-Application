using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;
using SobekCM.Core.UI_Configuration.Citation;
using SobekCM.Core.UI_Configuration.TemplateElements;
using SobekCM.Core.UI_Configuration.Viewers;

namespace SobekCM.Core.UI_Configuration
{
    /// <summary> User-Interface specific instance-wide configuration information,
    /// read from the configuration files mostly </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("UiConfig")]
    public class InstanceWide_UI_Configuration
    {
       // [DataMember(Name = "staticResources", EmitDefaultValue = false)]
       // [XmlElement("staticResources")]
       // [ProtoMember(1)]
       // public Static_Resources StaticResources { get; set; }

        /// <summary> Configuration for the citation within SobekCM, including the elements, group of element,
        /// order, and other details for rendering the citation within SobekCM  </summary>
        [DataMember(Name = "citationViewer", EmitDefaultValue = false)]
        [XmlElement("citationViewer")]
        [ProtoMember(2)]
        public CitationConfig CitationViewer { get; set; }

        /// <summary> Configuration class handles the mapping between all of the
        /// type/subtype attributes in each individual template cofiguration file
        /// and the actual classes to render that element in the online metadata forms </summary>
        [DataMember(Name = "templateElements", EmitDefaultValue = false)]
        [XmlElement("templateElements")]
        [ProtoMember(3)]
        public TemplateElementsConfig TemplateElements { get; set; }

        /// <summary> Configuration maps between all viewer codes used for
        /// most functionality in-system to th specific subviewer used </summary>
        [DataMember(Name = "viewers", EmitDefaultValue = false)]
        [XmlElement("viewers")]
        [ProtoMember(4)]
        public ViewerConfig Viewers { get; set; }
    }
}
