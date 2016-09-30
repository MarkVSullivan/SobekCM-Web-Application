using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
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
        [DataMember(EmitDefaultValue = false, Name = "displayTagsToIgnore")]
        [XmlArray("displayTagsToIgnore")]
        [XmlArrayItem("tag", typeof(string))]
        [ProtoMember(7)]
        public List<string> DisplayTagsToIgnore;

        /// <summary> Collection of all the mappings included in this mapping set </summary>
        [DataMember(EmitDefaultValue = false, Name = "mappings")]
        [XmlArray("mappings")]
        [XmlArrayItem("mapping", typeof(GenericXmlMappingPath))]
        [ProtoMember(8)]
        public List<GenericXmlMappingPath> Mappings;

        private Dictionary<string, GenericXmlMappingTreeNode> searchNodes;

        /// <summary> Constructor for a new instance of the <see cref="GenericXmlMappingSet"/> class </summary>
        public GenericXmlMappingSet()
        {
            Mappings = new List<GenericXmlMappingPath>();
            DisplayTagsToIgnore = new List<string>();
            searchNodes = new Dictionary<string, GenericXmlMappingTreeNode>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary> Constructor for a new instance of the <see cref="GenericXmlMappingSet"/> class </summary>
        /// <param name="MappingName"> The name of this mapping set </param>
        public GenericXmlMappingSet(string MappingName)
        {
            this.MappingName = MappingName;
            Mappings = new List<GenericXmlMappingPath>();
            DisplayTagsToIgnore = new List<string>();
            searchNodes = new Dictionary<string, GenericXmlMappingTreeNode>(StringComparer.OrdinalIgnoreCase);
        }

        public void Add_Display_Tag_To_Ignore(string Tag)
        {
            if (DisplayTagsToIgnore == null)
                DisplayTagsToIgnore = new List<string>();

            if (!DisplayTagsToIgnore.Contains(Tag))
                DisplayTagsToIgnore.Add(Tag);
        }

        public GenericXmlMappingPath Add_Path(GenericXmlPath Path)
        {
            GenericXmlMappingPath addPath = new GenericXmlMappingPath(Path);
            Mappings.Add(addPath);
            return addPath;
        }


        public static GenericXmlMappingSet Read(string MappingSetFile)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(GenericXmlMappingSet));

            // A FileStream is needed to read the XML document.
            FileStream fs = new FileStream(MappingSetFile, FileMode.Open);
            XmlReader reader = XmlReader.Create(fs);

            // Use the Deserialize method to restore the object's state.
            GenericXmlMappingSet i = (GenericXmlMappingSet)serializer.Deserialize(reader);
            fs.Close();

            return i;
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

        public PathMappingInstructions Get_Matching_Path_Instructions(GenericXmlPath Path)
        {
            if (searchNodes.Count == 0)
                build_search_tree();

            // Look for a match
            if (searchNodes.ContainsKey(Path.PathNodes[0].NodeName))
            {
                GenericXmlMappingTreeNode lastMatchingNode = searchNodes[Path.PathNodes[0].NodeName];
                int index = 1;
                while (index < Path.PathNodes.Count)
                {
                    string nextNodeName = Path.PathNodes[index].NodeName;
                    if (!lastMatchingNode.Children.ContainsKey(nextNodeName))
                        return null;

                    lastMatchingNode = lastMatchingNode.Children[nextNodeName];

                    index++;
                }

                // If there are instructions here, return them
                return lastMatchingNode.Instructions;
            }

            return null;
        }

        private void build_search_tree()
        {
            searchNodes.Clear();

            foreach (GenericXmlMappingPath mapping in Mappings)
            {
                // Handle the first node first
                if (( mapping.XmlPath != null ) && ( mapping.XmlPath.PathNodes != null ) && ( mapping.XmlPath.PathNodes.Count > 0 ))
                {
                    // Handle the root node
                    GenericXmlMappingTreeNode rootNode = null;
                    if (searchNodes.ContainsKey(mapping.XmlPath.PathNodes[0].NodeName))
                    {
                        rootNode = searchNodes[mapping.XmlPath.PathNodes[0].NodeName];
                    }
                    else
                    {
                        rootNode = new GenericXmlMappingTreeNode();
                        rootNode.Node = mapping.XmlPath.PathNodes[0];
                        searchNodes[mapping.XmlPath.PathNodes[0].NodeName] = rootNode;
                    }

                    // Was this the only node?
                    if (mapping.XmlPath.PathNodes.Count == 1)
                    {
                        rootNode.Instructions = mapping.Instructions;
                    }
                    else
                    {
                        int i = 0;
                        while ((i + 1) < mapping.XmlPath.PathNodes.Count)
                        {
                            i++;

                            if (rootNode.Children.ContainsKey(mapping.XmlPath.PathNodes[i].NodeName))
                            {
                                rootNode = rootNode.Children[mapping.XmlPath.PathNodes[i].NodeName];
                            }
                            else
                            {
                                GenericXmlMappingTreeNode newChildNode = new GenericXmlMappingTreeNode();
                                newChildNode.Node = mapping.XmlPath.PathNodes[i];
                                rootNode.Children[mapping.XmlPath.PathNodes[i].NodeName] = newChildNode;
                                rootNode = newChildNode;
                            }
                        }

                        // ASsign instructions here
                        rootNode.Instructions = mapping.Instructions;
                    }
                }
            }
        }
    }
}
