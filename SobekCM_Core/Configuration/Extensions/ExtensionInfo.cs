using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.Configuration.Extensions
{
    /// <summary> Contains the basic information about an extension from the configuration
    /// file primarily, but also including a flag if this extension is enabled </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("ExtensionInfo")]
    public class ExtensionInfo
    {
        /// <summary> Flag indicates if this extension is enabled in the database </summary>
        [DataMember(Name = "enabled")]
        [XmlAttribute("enabled")]
        [ProtoMember(1)]
        public bool Enabled { get; set; }

        /// <summary> Base directory for this extension </summary>
        [DataMember(Name = "directory", EmitDefaultValue = false)]
        [XmlAttribute("directory")]
        [ProtoMember(2)]
        public string BaseDirectory { get; set; }
         
        /// <summary> Code that uniquely identifiers this extension </summary>
        [DataMember(Name = "code")]
        [XmlAttribute("code")]
        [ProtoMember(3)]
        public string Code { get; set; }

        /// <summary> Full name of this extension </summary>
        [DataMember(Name = "name")]
        [XmlAttribute("name")]
        [ProtoMember(4)]
        public string Name { get; set; }

        /// <summary> Version of this extension </summary>
        [DataMember(Name = "version")]
        [XmlAttribute("version")]
        [ProtoMember(5)]
        public string Version { get; set; }

        /// <summary> List of assemblies referenced in the extension configuration file </summary>
        [DataMember(Name = "assemblies", EmitDefaultValue = false)]
        [XmlArray("assemblies")]
        [XmlArrayItem("assembly", typeof(string))]
        [ProtoMember(6)]
        public List<string> Assemblies { get; set; }

        /// <summary> List of CSS files referenced by this extension in the extension configuration file </summary>
        [DataMember(Name = "cssFiles", EmitDefaultValue = false)]
        [XmlArray("cssFiles")]
        [XmlArrayItem("css", typeof(ExtensionCssInfo))]
        [ProtoMember(7)]
        public List<ExtensionCssInfo> CssFiles { get; set; }

        /// <summary> Simple key/value configurations from the extension configuration file </summary>
        [DataMember(Name = "keyValueConfigurations", EmitDefaultValue = false)]
        [XmlArray("keyValueConfigurations")]
        [XmlArrayItem("keyValueConfig", typeof(ExtensionKeyValueConfiguration))]
        [ProtoMember(8)]
        public List<ExtensionKeyValueConfiguration> KeyValueConfigurations { get; set; }

        /// <summary> XML configuration sections from the extension configuration file </summary>
        [DataMember(Name = "xmlConfigurations", EmitDefaultValue = false)]
        [XmlArray("xmlConfigurations")]
        [XmlArrayItem("xmlConfig", typeof(ExtensionXmlConfiguration))]
        [ProtoMember(9)]
        public List<ExtensionXmlConfiguration> XmlConfigurations { get; set; }

        /// <summary> Constructor for a new instance of the <see cref="ExtensionInfo"/> class </summary>
        public ExtensionInfo()
        {
            // Do nothing?   
        }

        #region Methods that controls XML serialization

        /// <summary> Method suppresses XML Serialization of the Assemblies property if it is empty </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeAssemblies()
        {
            return ((Assemblies != null ) && ( Assemblies.Count > 0 ));
        }

        /// <summary> Method suppresses XML Serialization of the CssFiles property if it is empty </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeCssFiles()
        {
            return ((CssFiles != null) && (CssFiles.Count > 0));
        }

        /// <summary> Method suppresses XML Serialization of the KeyValueConfigurations property if it is empty </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeKeyValueConfigurations()
        {
            return ((KeyValueConfigurations != null) && (KeyValueConfigurations.Count > 0));
        }

        /// <summary> Method suppresses XML Serialization of the XmlConfigurations property if it is empty </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeXmlConfigurations()
        {
            return ((XmlConfigurations != null) && (XmlConfigurations.Count > 0));
        }


        #endregion


    }
}
