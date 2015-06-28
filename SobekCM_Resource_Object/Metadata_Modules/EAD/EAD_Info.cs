#region Using directives

using System;
using System.Collections.Generic;

#endregion

namespace SobekCM.Resource_Object.Metadata_Modules.EAD
{
    /// <summary> Contains all the information about an Encoded Archival Description (EAD) object, including the container hierarchy </summary>
    [Serializable]
    public class EAD_Info : iMetadata_Module
    {
        private readonly Description_of_Subordinate_Components containerHierachy;
        private string findingGuideDescription;
        private readonly List<EAD_TOC_Included_Section> toc_included_sections;

        /// <summary> Constructor for a new instance of the EAD_Info class </summary>
        public EAD_Info()
        {
            containerHierachy = new Description_of_Subordinate_Components();
            toc_included_sections = new List<EAD_TOC_Included_Section>();
        }

        /// <summary> Gets any container hierarchy for this Encoded Archival Description </summary>
        public Description_of_Subordinate_Components Container_Hierarchy
        {
            get { return containerHierachy; }
        }

        /// <summary> Gets the list of included sections in this EAD-type object to be included in 
        /// the table of contents </summary>
        public List<EAD_TOC_Included_Section> TOC_Included_Sections
        {
            get { return toc_included_sections; }
        }

        /// <summary> Gets and sets the Archival description chunk of HTML or XML for this EAD-type object </summary>
        public string Full_Description
        {
            get { return findingGuideDescription ?? String.Empty; }
            set { findingGuideDescription = value; }
        }

        /// <summary> Flag indicates if this EAD metadata module has data </summary>
        public bool hasData
        {
            get
            {
                return (Container_Hierarchy.Containers.Count > 0) || (Full_Description.Length > 0);
            }
        }

        #region Methods/Properties to implement the iMetadata_Module interface

        /// <summary> Name for this metadata module </summary>
        /// <value> This always returns 'EAD'</value>
        public string Module_Name
        {
            get { return GlobalVar.EAD_METADATA_MODULE_KEY; }
        }

        /// <summary> Gets the metadata search terms and values to be saved to the database
        /// to allow searching to occur over the data in this metadata module </summary>
        public List<KeyValuePair<string, string>> Metadata_Search_Terms
        {
            get { return null; }
        }

        /// <summary> Chance for this metadata module to perform any additional database work
        /// such as saving digital resource data into custom tables </summary>
        /// <param name="ItemID"> Primary key for this item within the SobekCM database </param>
        /// <param name="DB_ConnectionString"> Connection string for the current database </param>
        /// <param name="BibObject"> Entire resource, in case there are dependencies between this module and somethingt in the full resource </param>
        /// <param name="Error_Message"> In the case of an error, this contains text of the error </param>
        /// <returns> TRUE if no error occurred, otherwise FALSE </returns>
        /// <remarks> This module currently  does no additional processing in this method </remarks>
        public bool Save_Additional_Info_To_Database(int ItemID, string DB_ConnectionString, SobekCM_Item BibObject, out string Error_Message)
        {
            // Set the default error mesasge
            Error_Message = String.Empty;

            return true;
        }

        /// <summary> Chance for this metadata module to load any additional data from the 
        /// database when building this digital resource  in memory </summary>
        /// <param name="ItemID"> Primary key for this item within the SobekCM database </param>
        /// <param name="DB_ConnectionString">Connection string for the current database</param>
        /// <param name="BibObject"> Entire resource, in case there are dependencies between this module and somethingt in the full resource </param>
        /// <param name="Error_Message"> In the case of an error, this contains text of the error </param>
        /// <returns> TRUE if no error occurred, otherwise FALSE </returns>
        /// <remarks> This module currently  does no additional processing in this method </remarks>
        public bool Retrieve_Additional_Info_From_Database(int ItemID, string DB_ConnectionString, SobekCM_Item BibObject, out string Error_Message)
        {
            // Set the default error mesasge
            Error_Message = String.Empty;

            return true;
        }

        #endregion

        /// <summary> Add a TOC section for this EAD Finding Guide for display within a
        /// SobekCM digital library </summary>
        /// <param name="Internal_Link_Name"> Name of the internal link with the EAD Finding Guide which is used to allow a
        /// user to move to a section within the complete EAD Finding Guide HTML </param>
        /// <param name="Section_Title"> Title of this EAD Finding Guide section to be displayed in the table of contents </param>
        public void Add_TOC_Included_Section(string Internal_Link_Name, string Section_Title)
        {
            toc_included_sections.Add(new EAD_TOC_Included_Section(Internal_Link_Name, Section_Title));
        }
    }
}