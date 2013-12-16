#region Using directives

using System;
using System.Collections.Generic;

#endregion

namespace SobekCM.Resource_Object.Metadata_Modules
{
    /// <summary> Stores information about the Digital Archive [DAITSS] associated with this resource </summary>
    /// <remarks> Object created by Mark V Sullivan (2006) for University of Florida's Digital Library Center.</remarks>
    [Serializable]
    public class DAITSS_Info : iMetadata_Module
    {
        private string account;
        private bool archive;
        private string project;
        private string subaccount;

        /// <summary> Constructor for a new instance of the DAITSS_Info class </summary>
        public DAITSS_Info()
        {
            archive = true;
        }

        /// <summary> Gets and sets the flag which indicates this material will be sent to DAITSS </summary>
        public bool toArchive
        {
            get { return archive; }
            set { archive = value; }
        }

        /// <summary> Gets and sets the DAITSS account from the signed contract </summary>
        /// <value> This defaults to 'UF' if there is no value</value>
        public string Account
        {
            get { return account ?? "UF"; }
            set { account = value; }
        }

        /// <summary> Gets and sets the DAITSS sub-account, used for internal book-keeping </summary>
        public string SubAccount
        {
            get { return subaccount ?? String.Empty; }
            set { subaccount = value; }
        }

        /// <summary> Gets and sets the DAITSS project </summary>
        public string Project
        {
            get { return project ?? String.Empty; }
            set { project = value; }
        }

        public bool hasData
        {
            get { return (( !String.IsNullOrEmpty(project)) && ( !String.IsNullOrEmpty(account))); }
        }
        
        #region Methods/Properties to implement the iMetadata_Module interface

        /// <summary> Name for this metadata module </summary>
        /// <value> This always returns 'DAITSS'</value>
        public string Module_Name
        {
            get { return GlobalVar.DAITSS_METADATA_MODULE_KEY; }
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
    }
}