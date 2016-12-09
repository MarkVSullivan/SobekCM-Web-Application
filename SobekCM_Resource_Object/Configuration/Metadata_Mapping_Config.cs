using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;
using SobekCM.Resource_Object.Mapping;

namespace SobekCM.Resource_Object.Configuration
{
    /// <summary> Configuration information for a single metadata mapping object used to map
    /// field/value metadata pairs into a digital resource object </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("MetadataMappingConfig")]
    public class Metadata_Mapping_Config
    {
        /// <summary> Name used for identifying this metadata mapping configuration </summary>
        /// <remarks> This is just used for identifying which mapping configuration successfully mapped an field/value metadata pair</remarks>
        [DataMember(Name = "name")]
        [XmlAttribute("name")]
        [ProtoMember(1)]
        public string Name { get; set; }

        /// <summary> Namespace within which this metadata mapping object appears </summary>
        /// <remarks> For all standard reader/writers, this returns 'SobekCM.Resource_Object.Mapping'.<br /><br />
        /// This is used for instantiating the metadata mapping object. </remarks>
        [DataMember(Name = "namespace")]
        [XmlAttribute("namespace")]
        [ProtoMember(2)]
        public string Code_Namespace { get; set; }

        /// <summary> Class name for the metadata mapping object </summary>
        /// <remarks> This is used for instantiating the metadata mapping object. </remarks>
        [DataMember(Name = "class")]
        [XmlAttribute("class")]
        [ProtoMember(3)]
        public string Code_Class { get; set; }

        /// <summary> Assembly name of the DLL which holds this metadata mapping object </summary>
        /// <remarks> For all standard metadata mapping object, this is an empty string, since they are in this DLL. <br /><br />
        ///  This is used for instantiating the metadata mapping object. </remarks>
        [DataMember(Name = "assembly")]
        [XmlAttribute("assembly")]
        [ProtoMember(4)]
        public string Code_Assembly { get; set; }
        
        /// <summary> Constructor for a new instance of the Metadata_Mapping_Config class </summary>
        public Metadata_Mapping_Config()
        {
            Name = String.Empty;
            Code_Namespace = String.Empty;
            Code_Class = String.Empty;
            Code_Assembly = String.Empty;
        }

        /// <summary> Constructor for a new instance of the Metadata_Mapping_Config class </summary>
        /// <param name="Name"> Name used for identifying this metadata mapping configuration </param>
        /// <param name="Code_Namespace"> Namespace within which this metadata mapping object appears </param>
        /// <param name="Code_Class"> Class name for the metadata mapping object </param>
        /// <param name="Code_Assembly"> Assembly name of the DLL which holds this metadata mapping object </param>
        public Metadata_Mapping_Config(string Name, string Code_Namespace, string Code_Class, string Code_Assembly)
        {
            this.Name = Name;
            this.Code_Assembly = Code_Assembly;
            this.Code_Class = Code_Class;
            this.Code_Namespace = Code_Namespace;
        }

        /// <summary> Creates the bibliographic mapping object, from the provided code assembly, namespace, and class </summary>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public iBibliographicMapper Get_Module()
        {
            try
            {
                // Using reflection, create an object from the class namespace/name
                Assembly dllAssembly = Assembly.GetExecutingAssembly();
                if ( !String.IsNullOrWhiteSpace(Code_Assembly))
                {
                    // Try to find the file/path for this assembly then
                    string assemblyFilePath = ResourceObjectSettings.Get_Assembly(Code_Assembly);
                    if (assemblyFilePath != null)
                    {
                        dllAssembly = Assembly.LoadFrom(assemblyFilePath);
                    }
                }

                //  Assembly dllAssembly = Assembly..LoadFrom( Code_Assembly );
                Type readerWriterType = dllAssembly.GetType(Code_Namespace + "." + Code_Class);
                object possibleModule = Activator.CreateInstance(readerWriterType);
                iBibliographicMapper module = possibleModule as iBibliographicMapper;
                return module;
            }
            catch 
            {
                 return null;
            }
        }


    }
}
