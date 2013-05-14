#region Using directives

using System;
using System.Collections.Generic;

#endregion

namespace SobekCM.Resource_Object.Metadata_Modules
{
    /// <summary> Stores information about PALMM associated with this resource </summary>
    /// <remarks> Object created by Mark V Sullivan (2006) for University of Florida's Digital Library Center.</remarks>
    [Serializable]
    public class PALMM_Info : iMetadata_Module
    {
        private string palmm_project;
        private string palmm_server;
        private string palmm_type;
        private bool topalmm;

        /// <summary> Constructor creates a new instance of the PALMM_Info class </summary>
        public PALMM_Info()
        {
            topalmm = false;
        }

        /// <summary> Gets and sets the flag which indicates this resource should be loaded on PALMM </summary>
        public bool toPALMM
        {
            get { return topalmm; }
            set { topalmm = value; }
        }

        /// <summary> Gets and sets the PALMM project associated with this resource </summary>
        public string PALMM_Project
        {
            get { return palmm_project ?? String.Empty; }
            set { palmm_project = value; }
        }

        /// <summary> Gets and sets the PALMM-compliant type for this resource </summary>
        public string PALMM_Type
        {
            get { return palmm_type ?? String.Empty; }
            set { palmm_type = value; }
        }

        /// <summary> Gets and sets the PALMM target server (i.e. IC, TC, etc...)</summary>
        public string PALMM_Server
        {
            get { return palmm_server ?? String.Empty; }
            set { palmm_server = value; }
        }

        #region Methods/Properties to implement the iMetadata_Module interface

        /// <summary> Name for this metadata module </summary>
        /// <value> This always returns 'PALMM'</value>
        public string Module_Name
        {
            get { return "PALMM"; }
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

        /// <summary> Sets many of the PALMM values, based on the type of material </summary>
        /// <param name="type"></param>
        public void Set_Values(string type)
        {
            string temp_palmm_type = "monograph";
            string temp_palmm_server = "TC";

            if (type.Length > 0)
            {
                switch (type.Trim().ToUpper())
                {
                    case "TEXT":
                    case "BOOK":
                        temp_palmm_type = "monograph";
                        temp_palmm_server = "TC";
                        break;

                    case "IMAGE":
                        temp_palmm_type = "photo";
                        temp_palmm_server = "IC";
                        break;

                    case "SERIAL":
                    case "NEWSPAPER":
                        temp_palmm_type = "serial";
                        temp_palmm_server = "IC";
                        break;
                }
            }

            if (String.IsNullOrEmpty(palmm_type))
                palmm_type = temp_palmm_type;
            if (String.IsNullOrEmpty(palmm_server))
                palmm_server = temp_palmm_server;
        }
    }
}