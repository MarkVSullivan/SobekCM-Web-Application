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
        #region Private variable definitions

        private readonly List<Container_Info> containers;
        private Descriptive_Identification did;
        private string head;
        private string type;

        #endregion

        #region Constructor(s)

        /// <summary> Constructor for a new instance of the Description_of_Subordinate_Components class </summary>
        public Description_of_Subordinate_Components()
        {
            containers = new List<Container_Info>();
            did = new Descriptive_Identification();
        }

        #endregion

        #region Public Properties

        /// <summary> Gets the type value</summary>
        public string Type
        {
            get { return type ?? String.Empty; }
            set { type = value; }
        }

        /// <summary> Gets the list of container objects </summary>
        public List<Container_Info> Containers
        {
            get { return containers; }
        }

        /// <summary> Gets the did information object </summary>
        public Descriptive_Identification Did_Info
        {
            get { return did; }
        }

        #endregion

        /// <summary> Clear the contents of child componets and the descriptive identification </summary>
        public void Clear()
        {
            did.Clear();
            containers.Clear();
        }

        /// <summary> Reads the information about this container in the container list from the EAD XML Reader</summary>
        /// <param name="reader"> EAD XML Text Reader </param>
        public void Read(XmlTextReader reader)
        {
            String tagname = reader.Name;
            for (int i = 0; i < reader.AttributeCount; i++)
            {
                reader.MoveToAttribute(i);
                if (reader.Name.Equals("type"))
                    type = reader.Value;
            }
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "head")
                    {
                        reader.Read();
                        head = reader.Value;
                    }
                    else if (reader.Name == "did")
                    {
                        did = new Descriptive_Identification();
                        did.Read(reader);
                    }
                    else if (reader.Name == "c01")
                    {
                        Container_Info c_tag = new Container_Info();
                        c_tag.Read(reader);
                        containers.Add(c_tag);
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (reader.Name.Equals(tagname))
                        break;
                }
            }
        }

        /// <summary> Returns the recursively built container information for this class as a string for debug purposes </summary>
        /// <returns> Child container information, returned as a string for debug purposes </returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (Container_Info component in containers)
            {
                component.recursively_add_container_information(builder);
            }

            return builder.ToString();
        }
    }
}