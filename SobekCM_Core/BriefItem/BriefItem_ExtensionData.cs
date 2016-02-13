using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ProtoBuf;
using SobekCM.Tools;

namespace SobekCM.Core.BriefItem
{
    /// <summary> Information passed from an extension (or non-standard metadata module) </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("extension")]
    public class BriefItem_ExtensionData
    {
        /// <summary> Name of this extension, used as a key for retrieval </summary>
        [DataMember(EmitDefaultValue = false, Name = "name")]
        [XmlAttribute("name")]
        [ProtoMember(1)]
        public string Extension_Name { get; set; }

        /// <summary> Set of key/value pairs of data associated with this extension </summary>
        [DataMember(EmitDefaultValue = false, Name = "data")]
        [XmlArray("data")]
        [XmlArrayItem("keyValuePair", typeof(StringKeyValuePair))]
        [ProtoMember(2)]
        public List<StringKeyValuePair> Data { get; set; }

        /// <summary> Constructor for a new instance of the BriefItem_ExtensionData class </summary>
        /// <param name="Extension_Name"> Name of this extension, used as a key for retrieval  </param>
        public BriefItem_ExtensionData( string Extension_Name )
        {
            this.Extension_Name = Extension_Name;
            Data = new List<StringKeyValuePair>();
        }

        /// <summary> Constructor for a new instance of the BriefItem_ExtensionData class </summary>
        /// <remarks> Empty constructor for serialization and deserialization purposes </remarks>
        public BriefItem_ExtensionData()
        {
            Data = new List<StringKeyValuePair>();
        }
    }
}
