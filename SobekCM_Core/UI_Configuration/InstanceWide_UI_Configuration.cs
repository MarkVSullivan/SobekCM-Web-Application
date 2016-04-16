using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;
using SobekCM.Core.UI_Configuration.Citation;
using SobekCM.Core.UI_Configuration.StaticResources;
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
        public WriterViewerConfig WriterViewers { get; set; }

        /// <summary> Configuration information for the map editor function for this instance </summary>
        [DataMember(Name = "mapEditor", EmitDefaultValue = false)]
        [XmlElement("mapEditor")]
        [ProtoMember(5)]
        public MapEditor_Configuration MapEditor { get; set; }

        /// <summary> Static resource file information for this instance </summary>
        /// <remarks> This is set to be ignored since it is serialized separately </remarks>
        [IgnoreDataMember]
        public StaticResources_Configuration StaticResources { get; set; }


        /// <summary> Constructor for a new instance of the <see cref="InstanceWide_UI_Configuration"/> class </summary>
        public InstanceWide_UI_Configuration()
        {
            CitationViewer = new CitationConfig();
            TemplateElements = new TemplateElementsConfig();
            WriterViewers = new WriterViewerConfig();
            MapEditor = new MapEditor_Configuration();
            StaticResources = new StaticResources_Configuration();
        }
    }
}
