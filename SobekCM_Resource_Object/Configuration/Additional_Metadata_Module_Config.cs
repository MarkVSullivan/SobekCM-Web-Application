#region Using directives

using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;
using SobekCM.Resource_Object.Metadata_Modules;

#endregion

namespace SobekCM.Resource_Object.Configuration
{
    /// <summary> Holds all the information about metadata modules which should be added
    /// to each new SobekCM_Item class </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("AdditionalMetadataModuleConfig")]
    public class Additional_Metadata_Module_Config
    {
        /// <summary> Key used for this metadata module when adding to a new package </summary>
        [DataMember(Name = "key")]
        [XmlAttribute("key")]
        [ProtoMember(1)]
        public string Key { get; set; }

        /// <summary> Namespace within which this metadata extension module appears </summary>
        /// <remarks> For all standard reader/writers, this returns 'SobekCM.Resource_Object.Metadata_Modules'.<br /><br />
        /// This is used for instantiating the metadata extension module. </remarks>
        [DataMember(Name = "namespace")]
        [XmlAttribute("namespace")]
        [ProtoMember(2)]
        public string Code_Namespace { get;  set; }

        /// <summary> Class name for the metadata extension module </summary>
        /// <remarks> This is used for instantiating the metadata extension module. </remarks>
        [DataMember(Name = "class")]
        [XmlAttribute("class")]
        [ProtoMember(3)]
        public string Code_Class { get; set; }

        /// <summary> Assembly name of the DLL which holds this metadata extension module </summary>
        /// <remarks> For all standard metadata extension modules, this is an empty string, since they are in this DLL. <br /><br />
        ///  This is used for instantiating the metadata extension module. </remarks>
        [DataMember(Name = "assembly")]
        [XmlAttribute("assembly")]
        [ProtoMember(4)]
        public string Code_Assembly { get; set; }

        /// <summary> Constructor for a new instance of the Additional_Metadata_Module_Config class </summary>
        public Additional_Metadata_Module_Config()
        {
            Key = String.Empty;
            Code_Namespace = String.Empty;
            Code_Class = String.Empty;
            Code_Assembly = String.Empty;
        }

        /// <summary> Constructor for a new instance of the Additional_Metadata_Module_Config class </summary>
        /// <param name="Key"> Key used for this metadata module when adding to a new package</param>
        /// <param name="Code_Namespace">  Namespace within which this metadata extension module appears </param>
        /// <param name="Code_Class">  Class name for the metadata extension module </param>
        /// <param name="Code_Assembly"> Assembly name of the DLL which holds this metadata extension module </param>
        public Additional_Metadata_Module_Config( string Key, string Code_Namespace, string Code_Class, string Code_Assembly )
        {
            this.Key = Key;
            this.Code_Assembly = Code_Assembly;
            this.Code_Class = Code_Class;
            this.Code_Namespace = Code_Namespace;
        }

        /// <summary> Creates the reader/writer object, from the provided code assembly, namespace, and class </summary>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public iMetadata_Module Get_Module()
        {
            try
            {
                // Using reflection, create an object from the class namespace/name
                //     System.Reflection.Assembly dllAssembly = System.Reflection.Assembly.LoadFrom("SobekCM_Bib_Package_3_0_5.dll");
                Assembly dllAssembly = Assembly.GetExecutingAssembly();
                //  Assembly dllAssembly = Assembly..LoadFrom( Code_Assembly );
                Type readerWriterType = dllAssembly.GetType(Code_Namespace + "." + Code_Class);
                object possibleModule = Activator.CreateInstance(readerWriterType);
                iMetadata_Module module = possibleModule as iMetadata_Module;
                return module;
            }
            catch 
            {
                 return null;
            }
        }
    }
}
