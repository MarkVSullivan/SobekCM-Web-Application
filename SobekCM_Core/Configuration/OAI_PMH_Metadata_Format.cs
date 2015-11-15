using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.Configuration
{
    /// <summary> Class represents one possible metadata format that can be served
    /// via OAI-PMH, and indicates the class that should be used to create the metadata
    /// for storing in the databaase  and subsequently serving via OAI-PMH </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("OaiPmhMetadataFormat")]
    public class OAI_PMH_Metadata_Format
    {
        /// <summary> Constructor for a new instance of the OAI_PMH_Metadata_Format class </summary>
        public OAI_PMH_Metadata_Format()
        {
            Enabled = true;
        }

        /// <summary> Metadata prefix dispayed publicly and used by harvesters to indicate 
        /// this type of metadata is requested </summary>
        [DataMember(Name = "prefix", EmitDefaultValue = false)]
        [XmlAttribute("prefix")]
        [ProtoMember(1)]
        public string Prefix { get; set; }

        /// <summary> Metadata schema associated with this metadata format </summary>
        [DataMember(Name = "schema", EmitDefaultValue = false)]
        [XmlAttribute("schema")]
        [ProtoMember(2)]
        public string Schema { get; set; }

        /// <summary> Metadata namespace associated with this metadata format </summary>
        [DataMember(Name = "metadataNamespace", EmitDefaultValue = false)]
        [XmlAttribute("metadataNamespace")]
        [ProtoMember(3)]
        public string MetadataNamespace { get; set;  }

        /// <summary> Assembly in which this class resdes, if not a standard, included metadata format </summary>
        /// <remarks> If this is empty or null, an object of this type will be instantiated via reflection
        /// from the curently executing assemblies. </remarks>
        [DataMember(Name = "assembly", EmitDefaultValue = false)]
        [XmlAttribute("assembly")]
        [ProtoMember(4)]
        public string Assembly { get; set;  }

        /// <summary> Namspace in which the class used to create the metadata for this format is stored </summary>
        [DataMember(Name = "namespace", EmitDefaultValue = false)]
        [XmlAttribute("namespace")]
        [ProtoMember(5)]
        public string Namespace { get; set;  }

        /// <summary> Class which does the actual metadata format writing </summary>
        [DataMember(Name = "class", EmitDefaultValue = false)]
        [XmlAttribute("class")]
        [ProtoMember(6)]
        public string Class { get; set; }

        /// <summary> Flag indicates if OAI-PMH is allowed to be enabled with this format of metadata </summary>
        /// <remarks> The default value is TRUE, but can be overidden in the XML  </remarks>
        [DataMember(Name = "enabled")]
        [XmlAttribute("enabled")]
        [ProtoMember(7)]
        public bool Enabled { get; set; }
    }
}
