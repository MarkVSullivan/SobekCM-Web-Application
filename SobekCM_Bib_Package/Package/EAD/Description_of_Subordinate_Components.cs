using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;

namespace SobekCM.Bib_Package.EAD
{
    /// <summary> Description of subordinate components (or container list) for an encoded archival description (EAD) </summary>
    [Serializable]
    public class  Description_of_Subordinate_Components
    {
        #region Private variable definitions

        private string head;
        private string type;
        private List<Component_Info> c_tags;
        private Descriptive_Identification did;

        #endregion
                
        #region Constructor(s)

        /// <summary> Constructor for a new instance of the Description_of_Subordinate_Components class </summary>
        public Description_of_Subordinate_Components()
        {
            c_tags = new List<Component_Info>();
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
        public List<Component_Info> C_Tags
        {
            get { return c_tags; }
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
            c_tags.Clear();
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
                        Component_Info c_tag = new Component_Info();
                        c_tag.Read(reader);
                        c_tags.Add(c_tag);
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
            foreach (Component_Info component in c_tags)
            {
                component.recursively_add_container_information(builder);
            }

            return builder.ToString();
        }
    }
}
