using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;
using SobekCM.Core.Configuration.Authentication;
using SobekCM.Core.Configuration.Engine;
using SobekCM.Core.Configuration.OAIPMH;
using SobekCM.Resource_Object.Configuration;

namespace SobekCM.Core.Configuration
{
    /// <summary> Class provides access to all the configuration information loaded from the
    /// engine-side configuration files </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("Configurations")]
    public class InstanceWide_Configuration
    {
        /// <summary> Constructor for a new instance of the InstanceWide_Configuration class </summary>
        public InstanceWide_Configuration()
        {
            // Create some of the configuration stuff
            Authentication = new Authentication_Configuration();
            BriefItemMapping = new BriefItemMapping_Configuration();
            ContactForm = new ContactForm_Configuration();
            Engine = new Engine_Server_Configuration();
            QualityControlTool = new QualityControl_Configuration();
            MapEditor = new MapEditor_Configuration();
            Metadata = new Metadata_Configuration();
            OAI_PMH = new OAI_PMH_Configuration();
        }


        /// <summary> Configuration for authentication for this instance </summary>
        [DataMember(Name = "authentication", EmitDefaultValue = false)]
        [XmlElement("authentication")]
        [ProtoMember(2)]
        public Authentication_Configuration Authentication { get; set; }

        /// <summary> Configuration for the mapping between the main SobekCM_Item object
        /// and the BriefItem object used by the item viewers and passed over the REST API </summary>
        [DataMember(Name = "briefItemMapping", EmitDefaultValue = false)]
        [XmlElement("briefItemMapping")]
        [ProtoMember(3)]
        public BriefItemMapping_Configuration BriefItemMapping { get; set; }

        /// <summary> Configuration for the default contact form for this instance </summary>
        [DataMember(Name = "contactForm", EmitDefaultValue = false)]
        [XmlElement("contactForm")]
        [ProtoMember(4)]
        public ContactForm_Configuration ContactForm { get; set; }

        /// <summary> Configuration for the engine endpoints exposed for this instance </summary>
        [DataMember(Name = "engine", EmitDefaultValue = false)]
        [XmlElement("engine")]
        [ProtoMember(5)]
        public Engine_Server_Configuration Engine { get; set; }

        /// <summary> Configuration information for the map editor function for this instance </summary>
        [DataMember(Name = "mapEditor", EmitDefaultValue = false)]
        [XmlElement("mapEditor")]
        [ProtoMember(6)]
        public MapEditor_Configuration MapEditor { get; set; }

        /// <summary> Configuration information regarding how to read and write metadata files in the system </summary>
        [DataMember(Name = "metadata", EmitDefaultValue = false)]
        [XmlElement("metadata")]
        [ProtoMember(7)]
        public Metadata_Configuration Metadata { get; set; }

        /// <summary> Configuration for instance-wide OAI-PMH settings for this instance </summary>
        [DataMember(Name = "oai-pmh", EmitDefaultValue = false)]
        [XmlElement("oai-pmh")]
        [ProtoMember(8)]
        public OAI_PMH_Configuration OAI_PMH { get; set; }

        /// <summary> Configuration for the quality control tool for this instance </summary>
        [DataMember(Name = "qcConfig", EmitDefaultValue = false)]
        [XmlElement("qcConfig")]
        [ProtoMember(9)]
        public QualityControl_Configuration QualityControlTool { get; set; }



    }
}
