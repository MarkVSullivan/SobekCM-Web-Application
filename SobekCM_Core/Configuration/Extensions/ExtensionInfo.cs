using System;
using System.Collections.Generic;
using System.IO;
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

        /// <summary> Date this extension was enabled </summary>
        [DataMember(Name = "enabledDate")]
        [XmlIgnore]
        [ProtoMember(6)]
        public DateTime? EnabledDate { get; set; }

        /// <summary> Date this extension was enabled (for XML serialization)</summary>
        /// <remarks> This property is only exposed to allow for XML serialization of the nullable datetime </remarks>
        [IgnoreDataMember]
        [XmlAttribute("enableDate")]
        public DateTime EnabledDate_XML
        {
            get { return EnabledDate.HasValue ? EnabledDate.Value : DateTime.MinValue; }
            set { if (value != DateTime.MinValue) EnabledDate = value; }
        }

        /// <summary> Property controls if the associated property is serialized during XML serialization </summary>
        public bool ShouldSerializeEnabledDate_XML
        {
            get { return EnabledDate.HasValue; }
        }

        /// <summary> List of assemblies referenced in the extension configuration file </summary>
        [DataMember(Name = "assemblies", EmitDefaultValue = false)]
        [XmlArray("assemblies")]
        [XmlArrayItem("assembly", typeof(ExtensionAssembly))]
        [ProtoMember(7)]
        public List<ExtensionAssembly> Assemblies { get; set; }

        /// <summary> List of CSS files referenced by this extension in the extension configuration file </summary>
        [DataMember(Name = "cssFiles", EmitDefaultValue = false)]
        [XmlArray("cssFiles")]
        [XmlArrayItem("css", typeof(ExtensionCssInfo))]
        [ProtoMember(8)]
        public List<ExtensionCssInfo> CssFiles { get; set; }

        /// <summary> Simple key/value configurations from the extension configuration file </summary>
        [DataMember(Name = "keyValueConfigurations", EmitDefaultValue = false)]
        [XmlArray("keyValueConfigurations")]
        [XmlArrayItem("keyValueConfig", typeof(ExtensionKeyValueConfiguration))]
        [ProtoMember(9)]
        public List<ExtensionKeyValueConfiguration> KeyValueConfigurations { get; set; }

        /// <summary> XML configuration sections from the extension configuration file </summary>
        [DataMember(Name = "xmlConfigurations", EmitDefaultValue = false)]
        [XmlArray("xmlConfigurations")]
        [XmlArrayItem("xmlConfig", typeof(ExtensionXmlConfiguration))]
        [ProtoMember(10)]
        public List<ExtensionXmlConfiguration> XmlConfigurations { get; set; }

        /// <summary> List of any errors (or warnings) that occurred while reading the configuration file </summary>
        [DataMember(Name = "errors", EmitDefaultValue = false)]
        [XmlArray("errors")]
        [XmlArrayItem("error", typeof(string))]
        [ProtoMember(11)]
        public List<string> ConfigurationErrors { get; set; }


        /// <summary> Administrative information about an extension/plug-in, such as description,
        /// authors, permissions, etc..  </summary>
        [DataMember(Name = "adminInfo",EmitDefaultValue = false)]
        [XmlElement("adminInfo")]
        [ProtoMember(12)]
        public ExtensionAdminInfo AdminInfo { get; set; }

        /// <summary> Flag indicates if the highest rights are required to enable/disable this plug-in </summary>
        [DataMember(Name = "highestRightsRequired")]
        [XmlAttribute("highestRightsRequired")]
        [ProtoMember(13)]
        public bool HighestRightsRequired { get; set; }

        /// <summary> Constructor for a new instance of the <see cref="ExtensionInfo"/> class </summary>
        public ExtensionInfo()
        {
            // Do nothing?   
            HighestRightsRequired = false;
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

        /// <summary> Add information about an extension CSS file </summary>
        /// <param name="URL"> URL for this CSS </param>
        /// <param name="Condition"> Condition upon which this CSS file should be added </param>
        public void Add_CssFile(string URL, ExtensionCssInfoConditionEnum Condition)
        {
            if (CssFiles == null)
                CssFiles = new List<ExtensionCssInfo>();

            CssFiles.Add(new ExtensionCssInfo(URL, Condition));
        }

        /// <summary> Add information about any error encountered while reading the 
        /// plug-in configuration file extension subtree </summary>
        /// <param name="ErrorMessage"> Error message to add </param>
        public void Add_Error(string ErrorMessage)
        {
            if (ConfigurationErrors == null) ConfigurationErrors = new List<string>();

            ConfigurationErrors.Add(ErrorMessage);
        }

        /// <summary> Add information about a related assembly for this plug-in </summary>
        /// <param name="AssemblyPathFile"> Absolute path and filename for this assembly DLL file  </param>
        public void Add_Assembly(string AssemblyPathFile)
        {
            if (Assemblies == null) Assemblies = new List<ExtensionAssembly>();

            try
            {
                string filename = Path.GetFileNameWithoutExtension(AssemblyPathFile);
                Assemblies.Add(new ExtensionAssembly(filename, AssemblyPathFile));
            }
            catch 
            {
                Assemblies.Add(new ExtensionAssembly(String.Empty, AssemblyPathFile));
            }
        }

        /// <summary> Add information about a related assembly for this plug-in </summary>
        /// <param name="ID"> ID for this assembly, which is used throughout the configuration files to reference this assembly </param>
        /// <param name="AssemblyPathFile"> Absolute path and filename for this assembly DLL file  </param>
        public void Add_Assembly(string ID, string AssemblyPathFile)
        {
            if (Assemblies == null) Assemblies = new List<ExtensionAssembly>();

            Assemblies.Add(new ExtensionAssembly(ID, AssemblyPathFile));
        }
    }
}
