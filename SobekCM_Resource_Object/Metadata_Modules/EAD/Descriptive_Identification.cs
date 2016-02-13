#region Using directives

using System;
using System.Collections.Generic;
using System.Xml;

#endregion

namespace SobekCM.Resource_Object.Metadata_Modules.EAD
{
    /// <summary> Contains all the information for a single descriptive identification block within an EAD's container list </summary>
    [Serializable]
    public class Descriptive_Identification
    {

        #region Constructor(s)

        /// <summary> Constructor for a new instance of the Descriptive_Identification class </summary>
        public Descriptive_Identification()
        {
            // Do nothing
        }

        #endregion

        /// <summary> Adds a new container information object to this descriptive information </summary>
        /// <param name="Container_Type"> General type of this container ( usually 'box', 'folder', etc.. )</param>
        /// <param name="Container_Title"> Title or label for this container</param>
        public void Add_Container(string Container_Type, string Container_Title)
        {
            if (Containers == null)
                Containers = new List<Parent_Container_Info>();

            Containers.Add(new Parent_Container_Info(Container_Type, Container_Title));
        }

        /// <summary> Clears all the information in this descriptive identification object </summary>
        public void Clear()
        {
            Unit_Title = null;
            Unit_Date = null;
            DAO_Link = null;
            DAO_Title = null;
            DAO = null;
            Extent = null;
            Containers.Clear();
        }

        #region Public Properties

        /// <summary> Gets the number of container information objects associated with this descriptive identification information </summary>
        public int Container_Count
        {
            get { return Containers == null ? 0 : Containers.Count; }
        }

        /// <summary> Gets the collection of container information objects associated with this descriptive 
        /// identifiation object </summary>
        public List<Parent_Container_Info> Containers { get; private set; }

        /// <summary> Gets the unit title value associated with this  </summary>
        public string Unit_Title { get; set; }

        /// <summary> Gets the unit date value associated with this  </summary>
        public string Unit_Date { get; set; }

        /// <summary> Gets the link to the digital object  </summary>
        public string DAO_Link { get; set; }

        /// <summary> Gets the title of the digital object  </summary>
        public string DAO_Title { get; set; }

        /// <summary> Gets the dao information of the digital object  </summary>
        public string DAO { get; set; }

        /// <summary> Gets the extent information </summary>
        public string Extent { get; set; }

        #endregion

        #region Method to read from the EAD XML file 

        /// <summary> Reads the information about this container in the container list from the EAD XML Reader</summary>
        /// <param name="Reader"> EAD XML Text Reader </param>
        public void Read(XmlTextReader Reader)
        {
            String tagname = Reader.Name;
            while (Reader.Read())
                if (Reader.NodeType == XmlNodeType.Element)
                {
                    switch (Reader.Name)
                    {
                        case "container":
                            string type = "";
                            if (Reader.MoveToAttribute("type"))
                                type = Reader.Value;
                            Reader.Read();
                            if (Reader.NodeType != XmlNodeType.EndElement)
                            {
                                Add_Container(type, Reader.Value);
                            }
                            break;

                        case "unittitle":
                            while (Reader.Read())
                            {
                                if (Reader.NodeType == XmlNodeType.Text)
                                    Unit_Title = Reader.Value;
                                else if (Reader.NodeType == XmlNodeType.EndElement && Reader.Name.Equals("unittitle"))
                                    break;
                            }
                            break;

                        case "unitdate":
                            while (Reader.Read())
                            {
                                if (Reader.NodeType == XmlNodeType.Text)
                                    Unit_Date = Reader.Value;
                                else if (Reader.NodeType == XmlNodeType.EndElement && Reader.Name.Equals("unitdate"))
                                    break;
                            }
                            break;

                        case "physdesc":
                        case "extent":
                            while (Reader.Read())
                            {
                                if (Reader.NodeType == XmlNodeType.Text)
                                {
                                    Extent = Reader.Value;
                                }
                                else if (Reader.NodeType == XmlNodeType.EndElement)
                                {
                                    if ((Reader.Name.Equals("extent")) || (Reader.Name.Equals("physdesc")))
                                        break;
                                }
                            }
                            break;

                        case "dao":
                            for (int i = 0; i < Reader.AttributeCount; i++)
                            {
                                Reader.MoveToAttribute(i);
                                if (Reader.Name.Equals("href"))
                                    DAO_Link = Reader.Value;
                                else if (Reader.Name.Equals("title"))
                                    DAO_Title = Reader.Value;
                            }
                            while (Reader.Read())
                            {
                                if (Reader.NodeType == XmlNodeType.Text)
                                    DAO = Reader.Value;
                                else if (Reader.NodeType == XmlNodeType.EndElement && Reader.Name.Equals("dao"))
                                    break;
                            }
                            break;
                    }
                }
                else if (Reader.NodeType == XmlNodeType.EndElement && Reader.Name.Equals(tagname))
                    break;
        }

        #endregion


    }
}