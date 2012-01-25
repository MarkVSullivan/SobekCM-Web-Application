using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SobekCM.Bib_Package.EAD
{
    /// <summary> Contains all the information about an Encoded Archival Description (EAD) object, including the container hierarchy </summary>
    [Serializable]
    public class EAD_Info
    {
        private Description_of_Subordinate_Components containerHierachy;
        private string findingGuideDescription;
        private List<EAD_TOC_Included_Section> toc_included_sections;

        /// <summary> Constructor for a new instance of the EAD_Info class </summary>
        public EAD_Info()
        {
            containerHierachy = new Description_of_Subordinate_Components();
            toc_included_sections = new List<EAD_TOC_Included_Section>();
        }

        /// <summary> Gets any container hierarchy for this Encoded Archival Description </summary>
        public Description_of_Subordinate_Components Container_Hierarchy
        {
            get
            {
                return containerHierachy;
            }
        }

        /// <summary> Gets the list of included sections in this EAD-type object to be included in 
        /// the table of contents </summary>
        public List<EAD_TOC_Included_Section> TOC_Included_Sections
        {
            get
            {
                return toc_included_sections;
            }
        }

        /// <summary> Add a TOC section for this EAD Finding Guide for display within a
        /// SobekCM digital library </summary>
        /// <param name="Internal_Link_Name"> Name of the internal link with the EAD Finding Guide which is used to allow a
        /// user to move to a section within the complete EAD Finding Guide HTML </param>
        /// <param name="Section_Title"> Title of this EAD Finding Guide section to be displayed in the table of contents </param>
        public void Add_TOC_Included_Section(string Internal_Link_Name, string Section_Title)
        {
            toc_included_sections.Add(new EAD_TOC_Included_Section(Internal_Link_Name, Section_Title));
        }

        /// <summary> Gets and sets the Archival description chunk of HTML or XML for this EAD-type object </summary>
        public string Description
        {
            get { return findingGuideDescription ?? String.Empty; }
            set { findingGuideDescription = value; }
        }
    }
}
