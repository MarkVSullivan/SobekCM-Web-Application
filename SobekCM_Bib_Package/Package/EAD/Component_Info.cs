using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;

namespace SobekCM.Bib_Package.EAD
{
    /// <summary> Contains all of the information about a single component (or container) including
    /// the list of child components (or containers)  </summary>
    [Serializable]
    public class Component_Info
    {
        #region Private variable definitions

        private string level;
        private string biogHist;
        private string scope;
        private List<Component_Info> c_tags;
        private Descriptive_Identification did;
        private bool has_complex_child;


        #endregion

        #region Constructor(s)

        /// <summary> Constructor for a new instance of the C_Info class </summary>
        public Component_Info()
        {
            did = new Descriptive_Identification();
        }

        #endregion

        /// <summary> Recursively adds the child component's information to the StringBuilder, in HTML format </summary>
        /// <param name="builder"> Builder of all the HTML-formatted componenet information for this component</param>
        public void recursively_add_container_information(StringBuilder builder)
        {
            // Write the information for this tage
            builder.AppendLine(did.Unit_Title + "<br />");
            if (c_tags.Count > 0)
            {
                builder.AppendLine("<blockquote>");
                foreach (Component_Info component in c_tags)
                {
                    component.recursively_add_container_information(builder);
                }
                builder.AppendLine("</blockquote>");
            }
        }

        /// <summary> Flag indicates if this component has complex data, or any children have complex data </summary>
        /// <remarks> A compex component is defined as one that has scope, biogHist, or extend information included, or has a child which has this type of data</remarks>
        public bool is_Complex_Or_Has_Complex_Children
        {
            get
            {
                // If this has complex children, it is complex
                if (has_complex_child)
                    return true;

                // Is this complex?
                if ((!String.IsNullOrEmpty(scope)) || (!String.IsNullOrEmpty(biogHist)) || (did.Extent.Length > 0))
                    return true;

                // Default return false
                return false;
            }
        }

        

        #region Public Properties

        /// <summary> Gets the number of child component tags to this component </summary>
        public int Children_Count
        {
            get
            {
                if (c_tags == null)
                    return 0;
                else
                    return c_tags.Count;
            }
        }

        /// <summary> Gets the collection of child components </summary>
        public List<Component_Info> Children
        {
            get
            {
                return c_tags;
            }
        }

        /// <summary> Gets the level value associated with this container in the container list </summary>
        public string Level
        {
            get { return level ?? String.Empty;  }
            set { level = value;  }
        }

        /// <summary> Gets the biogrpahical history value associated with this container in the container list </summary>
        public string Biographical_History
        {
            get { return biogHist ?? String.Empty; }
            set { biogHist = value; }
        }

        /// <summary> Gets the scope and content value associated with this container in the container list </summary>
        public string Scope_And_Content
        {
            get { return scope ?? String.Empty; }
            set { scope = value; }
        }

        /// <summary> Gets the number of container information objects included in the descriptive portion of this component </summary>
        public int Container_Count
        {
            get   {     return did.Container_Count;     }
        }

        /// <summary> Gets the number of container information objects in the descriptive portion of this component </summary>
        public List<Parent_Container_Info> Containers
        {
            get
            {
                return did.Containers;
            }
        }

        /// <summary> Gets the unit title value associated with this  </summary>
        public string Unit_Title
        {
            get { return did.Unit_Title; }
            set { did.Unit_Title = value; }
        }

        /// <summary> Gets the unit date value associated with this  </summary>
        public string Unit_Date
        {
            get { return did.Unit_Date; }
            set { did.Unit_Date = value; }
        }

        /// <summary> Gets the link to the digital object  </summary>
        public string DAO_Link
        {
            get { return did.DAO_Link; }
            set { did.DAO_Link = value; }
        }

        /// <summary> Gets the title of the digital object  </summary>
        public string DAO_Title
        {
            get { return did.DAO_Title; }
            set { did.DAO_Title = value; }
        }

        /// <summary> Gets the dao information of the digital object  </summary>
        public string DAO
        {
            get { return did.DAO; }
            set { did.DAO = value; }
        }

        /// <summary> Gets the extent information </summary>
        public string Extent
        {
            get { return did.Extent; }
            set { did.Extent = value; }
        }

        #endregion

        /// <summary> Reads the information about this container in the container list from the EAD XML Reader</summary>
        /// <param name="reader"> EAD XML Text Reader </param>
        public void Read(XmlTextReader reader )
        {
            Regex rgx1 = new Regex("xmlns=\"[^\"]*\"");
            Regex rgx2 = new Regex("[ ]*>");
            Regex rgx3 = new Regex("[ ]*/>");

            string tagname = reader.Name;
            Regex ctagPattern = new Regex("c[0-9][0-9]");
            if ( reader.MoveToAttribute("level"))
                level = reader.Value;

            // Read all the information under this component
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (ctagPattern.IsMatch(reader.Name))
                    {
                        // Read this child component
                        Component_Info c_tag = new Component_Info();
                        c_tag.Read(reader);
                        if (c_tags == null)
                            c_tags = new List<Component_Info>();
                        c_tags.Add(c_tag);

                        // Set this flag to determine if this has a complex child
                        if (c_tag.is_Complex_Or_Has_Complex_Children)
                            has_complex_child = true;
                    }
                    else
                    {
                        switch (reader.Name)
                        {
                            case "did":
                                did = new Descriptive_Identification();
                                did.Read(reader);
                                break;

                            case "bioghist":
                                string InnerXml_bioghist = reader.ReadInnerXml();
                                string InnerXml_bioghist2 = rgx1.Replace(InnerXml_bioghist, "");
                                string InnerXml_bioghist3 = rgx2.Replace(InnerXml_bioghist2, ">");
                                biogHist = rgx3.Replace(InnerXml_bioghist3, "/>");
                                break;

                            case "scopecontent":
                                string InnerXml_scopecontent = reader.ReadInnerXml();
                                string InnerXml_scopecontent2 = rgx1.Replace(InnerXml_scopecontent, "");
                                string InnerXml_scopecontent3 = rgx2.Replace(InnerXml_scopecontent2, ">");
                                scope = rgx3.Replace(InnerXml_scopecontent3, "/>");
                                break;
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement && reader.Name.Equals(tagname))
                    break;
            }
        }
    }
}
