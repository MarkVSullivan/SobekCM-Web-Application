#region Using directives

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.Settings
{
    /// <summary> [DataContract] Class stores the more complex settings used by the builder </summary>
    [Serializable, DataContract, ProtoContract]
    public class Builder_Settings
    {
        /// <summary> Constructor for a new instance of the Builder_Settings class </summary>
        public Builder_Settings()
        {
            // Initialized the collections
            IncomingFolders = new List<Builder_Source_Folder>();
            PreProcessModulesSettings = new List<Builder_Module_Setting>();
            PostProcessModulesSettings = new List<Builder_Module_Setting>();
            ItemProcessModulesSettings = new List<Builder_Module_Setting>();
            ItemDeleteModulesSettings = new List<Builder_Module_Setting>();
        }
                

        /// <summary> [DataMember] List of all the incoming folders which should be checked for new resources </summary>
        [DataMember(Name = "folders")]
        [XmlArray("folders")]
        [XmlArrayItem("folder", typeof(Builder_Source_Folder))]
        [ProtoMember(1)]
        public List<Builder_Source_Folder> IncomingFolders { get; private set; }

        /// <summary> [DataMember] List of modules to run before doing any additional processing </summary>
        [DataMember(Name = "preProcessModules")]
        [XmlArray("preProcessModules")]
        [XmlArrayItem("module", typeof(Builder_Module_Setting))]
        [ProtoMember(2)]
        public List<Builder_Module_Setting> PreProcessModulesSettings { get; private set; }

        /// <summary> [DataMember] List of modules to run after doing any additional processing </summary>
        [DataMember(Name = "postProcessModules")]
        [XmlArray("postProcessModules")]
        [XmlArrayItem("module", typeof(Builder_Module_Setting))]
        [ProtoMember(3)]
        public List<Builder_Module_Setting> PostProcessModulesSettings { get; private set; }

        /// <summary> [DataMember] List of all the builder modules used for new packages or updates (processed by the builder) </summary>
        [DataMember(Name = "itemProcessModules")]
        [XmlArray("itemProcessModules")]
        [XmlArrayItem("module", typeof(Builder_Module_Setting))]
        [ProtoMember(4)]
        public List<Builder_Module_Setting> ItemProcessModulesSettings { get; private set; }

        /// <summary> [DataMember] List of all the builder modules to use for item deletes (processed by the builder) </summary>
        [DataMember(Name = "itemDeleteModules")]
        [XmlArray("itemDeleteModules")]
        [XmlArrayItem("module", typeof(Builder_Module_Setting))]
        [ProtoMember(5)]
        public List<Builder_Module_Setting> ItemDeleteModulesSettings { get; private set; }

        /// <summary> Clear all these settings </summary>
        public virtual void Clear()
        {
            IncomingFolders.Clear();
            PreProcessModulesSettings.Clear();
            PostProcessModulesSettings.Clear();
            ItemProcessModulesSettings.Clear();
            ItemDeleteModulesSettings.Clear();
        }

    }
}
