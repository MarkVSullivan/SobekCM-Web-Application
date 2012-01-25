using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;

namespace SobekCM.Bib_Package.EAD
{

    /// <summary> Contains all the information for a single descriptive identification block within an EAD's container list </summary>
    [Serializable]
    public class Descriptive_Identification
    {
        #region Private variable definitions
        
        private string unittitle;
        private string unitdate;
        private string daoHref;
        private string daoTitle;
        private string dao;
        private string extent;
        private List<Parent_Container_Info> containers;

        #endregion

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
            if (containers == null)
                containers = new List<Parent_Container_Info>();

            containers.Add(new Parent_Container_Info(Container_Type, Container_Title));
        }

        #region Public Properties

        /// <summary> Gets the number of container information objects associated with this descriptive identification information </summary>
        public int Container_Count
        {
            get
            {
                if (containers == null)
                    return 0;
                else
                    return containers.Count;
            }
        }

        /// <summary> Gets the collection of container information objects associated with this descriptive 
        /// identifiation object </summary>
        public List<Parent_Container_Info> Containers
        {
            get
            {
                return containers;
            }
        }

        /// <summary> Gets the unit title value associated with this  </summary>
        public string Unit_Title
        {
            get { return unittitle ?? String.Empty; }
            set { unittitle = value; }
        }

        /// <summary> Gets the unit date value associated with this  </summary>
        public string Unit_Date
        {
            get { return unitdate ?? String.Empty; }
            set { unitdate = value; }
        }

        /// <summary> Gets the link to the digital object  </summary>
        public string DAO_Link
        {
            get { return daoHref ?? String.Empty; }
            set { daoHref = value; }
        }

        /// <summary> Gets the title of the digital object  </summary>
        public string DAO_Title
        {
            get { return daoTitle ?? String.Empty; }
            set { daoTitle = value; }
        }

        /// <summary> Gets the dao information of the digital object  </summary>
        public string DAO
        {
            get { return dao ?? String.Empty; }
            set { dao = value; }
        }

        /// <summary> Gets the extent information </summary>
        public string Extent
        {
            get { return extent ?? String.Empty; }
            set { extent = value; }
        }

        #endregion

        /// <summary> Clears all the information in this descriptive identification object </summary>
        public void Clear()
        {
            unittitle = null;
            unitdate = null;
            daoHref = null;
            daoTitle = null;
            dao = null;
            extent = null;
            containers.Clear();
        }


        /// <summary> Reads the information about this container in the container list from the EAD XML Reader</summary>
        /// <param name="reader"> EAD XML Text Reader </param>
        public void Read(XmlTextReader reader)
        {
            String tagname = reader.Name;
            while (reader.Read())
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "container":
                            string type = "";
                            if ( reader.MoveToAttribute("type"))
                                type = reader.Value;
                            reader.Read();
                            if (reader.NodeType != XmlNodeType.EndElement)
                            {
                                Add_Container(type, reader.Value);
                            }
                            break;

                        case "unittitle":
                            while (reader.Read())
                            {
                                if (reader.NodeType == XmlNodeType.Text)
                                    unittitle = reader.Value;
                                else if (reader.NodeType == XmlNodeType.EndElement && reader.Name.Equals("unittitle"))
                                    break;
                            }
                            break;

                        case "unitdate":
                            while (reader.Read())
                            {
                                if (reader.NodeType == XmlNodeType.Text)
                                    unitdate = reader.Value;
                                else if (reader.NodeType == XmlNodeType.EndElement && reader.Name.Equals("unitdate"))
                                    break;
                            }
                            break;

                        case "physdesc":
                        case "extent":
                            while (reader.Read())
                            {
                                if (reader.NodeType == XmlNodeType.Text)
                                {
                                    extent = reader.Value;
                                }
                                else if (reader.NodeType == XmlNodeType.EndElement)
                                {
                                    if ((reader.Name.Equals("extent")) || (reader.Name.Equals("physdesc")))
                                        break;
                                }
                            }
                            break;

                        case "dao":
                            for (int i = 0; i < reader.AttributeCount; i++)
                            {
                                reader.MoveToAttribute(i);
                                if (reader.Name.Equals("href"))
                                    daoHref = reader.Value;
                                else if (reader.Name.Equals("title"))
                                    daoTitle = reader.Value;
                            }
                            while (reader.Read())
                            {
                                if (reader.NodeType == XmlNodeType.Text)
                                    dao = reader.Value;
                                else if (reader.NodeType == XmlNodeType.EndElement && reader.Name.Equals("dao"))
                                    break;
                            }
                            break;
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement && reader.Name.Equals(tagname))
                    break;
        }
    }
}
