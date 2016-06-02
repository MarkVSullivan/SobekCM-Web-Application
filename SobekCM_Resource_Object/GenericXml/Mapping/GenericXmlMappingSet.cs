using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;
using SobekCM.Resource_Object.GenericXml.Reader;

namespace SobekCM.Resource_Object.GenericXml.Mapping
{
    /// <summary> A single mapping set with mappings from a generic XML file to the related
    /// SobekCM fields, including basic information about the mapping set itself </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("XmlMappingSet")]
    public class GenericXmlMappingSet
    {
        /// <summary> The name of this mapping set </summary>
        [DataMember(EmitDefaultValue = false, Name = "name")]
        [XmlAttribute("name")]
        [ProtoMember(1)]
        public string MappingName { get; set; }

        /// <summary> The creator of this mapping set </summary>
        [DataMember(EmitDefaultValue = false, Name = "creator")]
        [XmlAttribute("creator")]
        [ProtoMember(2)]
        public string Creator { get; set; }

        /// <summary> The version of this mapping set </summary>
        [DataMember(EmitDefaultValue = false, Name = "version")]
        [XmlAttribute("version")]
        [ProtoMember(3)]
        public string Version { get; set; }

        /// <summary> The date this mapping set was last modified  </summary>
        [DataMember(EmitDefaultValue = false, Name = "createDate")]
        [XmlAttribute("createDate")]
        [ProtoMember(4)]
        public string CreateDate { get; set; }

        /// <summary> The date this mapping set was last modified  </summary>
        [DataMember(EmitDefaultValue = false, Name = "lastModified")]
        [XmlAttribute("lastModified")]
        [ProtoMember(5)]
        public string LastModified { get; set; }

        /// <summary> Description of this mapping set </summary>
        [DataMember(EmitDefaultValue = false, Name = "description")]
        [XmlElement("description")]
        [ProtoMember(6)]
        public string Description { get; set; }

        /// <summary> Collection of all the mappings included in this mapping set </summary>
        [DataMember(EmitDefaultValue = false, Name = "mappings")]
        [XmlArray("mappings")]
        [XmlArrayItem("mapping", typeof(GenericXmlMappingPath))]
        [ProtoMember(7)]
        public List<GenericXmlMappingPath> Mappings;

        /// <summary> Constructor for a new instance of the <see cref="GenericXmlMappingSet"/> class </summary>
        public GenericXmlMappingSet()
        {
            Mappings = new List<GenericXmlMappingPath>();
        }

        /// <summary> Constructor for a new instance of the <see cref="GenericXmlMappingSet"/> class </summary>
        /// <param name="MappingName"> The name of this mapping set </param>
        public GenericXmlMappingSet(string MappingName)
        {
            this.MappingName = MappingName;
            Mappings = new List<GenericXmlMappingPath>();
        }

        public GenericXmlMappingPath Add_Path(GenericXmlPath Path)
        {
            GenericXmlMappingPath addPath = new GenericXmlMappingPath(Path);
            Mappings.Add(addPath);
            return addPath;
        }


        public static GenericXmlMappingSet Read(string MappingSetFile)
        {
            return new GenericXmlMappingSet();    
        }

        public bool Save(string MappingSetFile )
        {
            try
            {
                // Open a stream to the file
                StreamWriter outputFile = new StreamWriter(MappingSetFile, false );

                // Create the XML serializer
                XmlSerializer x = new XmlSerializer(this.GetType());

                // Serialize the mapping object
                x.Serialize(outputFile, this);

                outputFile.Flush();
                outputFile.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
