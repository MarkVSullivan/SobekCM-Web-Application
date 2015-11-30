using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ProtoBuf;
using SobekCM.Core.MicroservicesClient;

namespace SobekCM.Core.Configuration
{
    /// <summary> Class provides access to all the configuration information loaded from the
    /// engine-side configuration files </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("Settings")]
    public class InstanceWide_Configuration
    {
        /// <summary> Constructor for a new instance of the InstanceWide_Configuration class </summary>
        public InstanceWide_Configuration()
        {
            // Create some of the configuration stuff
            Authentication = new Authentication_Configuration();
            ContactForm = new ContactForm_Configuration();
            QualityControlTool = new QualityControl_Configuration();
            MapEditor = new MapEditor_Configuration();
            OAI_PMH = new OAI_PMH_Configuration();
        }


        /// <summary> Configuration for authentication for this instance </summary>
        [DataMember(Name = "authentication", EmitDefaultValue = false)]
        [XmlElement("authentication")]
        [ProtoMember(2)]
        public Authentication_Configuration Authentication { get; set; }

        /// <summary> Configuration for the default contact form for this instance </summary>
        [DataMember(Name = "contactForm", EmitDefaultValue = false)]
        [XmlElement("contactForm")]
        [ProtoMember(3)]
        public ContactForm_Configuration ContactForm { get; set; }

        /// <summary> Configuration information for the map editor function for this instance </summary>
        [DataMember(Name = "mapEditor", EmitDefaultValue = false)]
        [XmlElement("mapEditor")]
        [ProtoMember(4)]
        public MapEditor_Configuration MapEditor { get; set; }

        /// <summary> Configuration information for the map editor function for this instance </summary>
        [DataMember(Name = "microservices", EmitDefaultValue = false)]
        [XmlElement("microservices")]
        [ProtoMember(5)]
        public MicroservicesClient_Configuration Microservices { get; set; }

        /// <summary> Configuration for instance-wide OAI-PMH settings for this instance </summary>
        [DataMember(Name = "oai-pmh", EmitDefaultValue = false)]
        [XmlElement("oai-pmh")]
        [ProtoMember(6)]
        public OAI_PMH_Configuration OAI_PMH { get; set; }

        /// <summary> Configuration for the quality control tool for this instance </summary>
        [DataMember(Name = "qcConfig", EmitDefaultValue = false)]
        [XmlElement("qcConfig")]
        [ProtoMember(7)]
        public QualityControl_Configuration QualityControlTool { get; set; }

    }
}
