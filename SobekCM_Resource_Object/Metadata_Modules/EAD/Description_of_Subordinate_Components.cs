#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

#endregion

namespace SobekCM.Resource_Object.Metadata_Modules.EAD
{
    /// <summary> Description of subordinate Containers (or container list) for an encoded archival description (EAD) </summary>
    [Serializable]
    public class Description_of_Subordinate_Components
    {
        #region Constructor(s)

        /// <summary> Constructor for a new instance of the Description_of_Subordinate_Components class </summary>
        public Description_of_Subordinate_Components()
        {
            Containers = new List<Container_Info>();
            Did = new Descriptive_Identification();
        }

        #endregion

        #region Public Properties

        /// <summary> Gets the type value</summary>
        public string Type { get; set; }

        /// <summary> Gets the list of container objects </summary>
        public List<Container_Info> Containers { get; private set; }

        /// <summary> Gets the did information object </summary>
        public Descriptive_Identification Did { get; private set; }

        /// <summary> Holds basic head information about the containers, from the EAD </summary>
        public string Head { get; set; }

        #endregion

        /// <summary> Clear the contents of child componets and the descriptive identification </summary>
        public void Clear()
        {
            Did.Clear();
            Containers.Clear();
        }

        /// <summary> Reads the information about this container in the container list from the EAD XML Reader</summary>
        /// <param name="Reader"> EAD XML Text Reader </param>
        public void Read(XmlTextReader Reader)
        {
            String tagname = Reader.Name;
            for (int i = 0; i < Reader.AttributeCount; i++)
            {
                Reader.MoveToAttribute(i);
                if (Reader.Name.Equals("type"))
                    Type = Reader.Value;
            }
            while (Reader.Read())
            {
                if (Reader.NodeType == XmlNodeType.Element)
                {
                    if (Reader.Name == "head")
                    {
                        Reader.Read();
                        Head = Reader.Value;
                    }
                    else if (Reader.Name == "did")
                    {
                        Did = new Descriptive_Identification();
                        Did.Read(Reader);
                    }
                    else if (Reader.Name == "c01")
                    {
                        Container_Info c_tag = new Container_Info();
                        c_tag.Read(Reader);
                        Containers.Add(c_tag);
                    }
                }
                else if (Reader.NodeType == XmlNodeType.EndElement)
                {
                    if (Reader.Name.Equals(tagname))
                        break;
                }
            }
        }

        /// <summary> Returns the recursively built container information for this class as a string for debug purposes </summary>
        /// <returns> Child container information, returned as a string for debug purposes </returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (Container_Info component in Containers)
            {
                component.recursively_add_container_information(builder);
            }

            return builder.ToString();
        }
    }
}