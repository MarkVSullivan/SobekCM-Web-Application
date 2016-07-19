using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.Configuration.Extensions
{
    /// <summary> Information about a single assembly included within an extension </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("ExtensionAssembly")]
    public class ExtensionAssembly
    {
        /// <summary> ID for this assembly, which is used throughout the configuration files to reference this assembly </summary>
        [DataMember(Name = "id")]
        [XmlAttribute("id")]
        [ProtoMember(1)]
        public string ID { get; set; }

        /// <summary> Absolute path and filename for this assembly DLL file </summary>
        [DataMember(Name = "filePath")]
        [XmlText]
        [ProtoMember(2)]
        public string FilePath { get; set; }

        /// <summary> Constructor for a new instance of the ExtensionAssembly class </summary>
        /// <param name="ID"> ID for this assembly, which is used throughout the configuration files to reference this assembly </param>
        /// <param name="FilePath"> Absolute path and filename for this assembly DLL file </param>
        public ExtensionAssembly(string ID, string FilePath)
        {
            this.ID = ID;
            this.FilePath = FilePath;
        }

        /// <summary> Constructor for a new instance of the ExtensionAssembly class </summary>
        public ExtensionAssembly()
        {
            // Do nothing.. primarily for serialization purposes 
        }
    }
}
