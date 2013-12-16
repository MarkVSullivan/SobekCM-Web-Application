#region Using directives

using System;
using System.Collections.Generic;

#endregion

namespace SobekCM.Resource_Object.Metadata_Modules
{
    /// <summary> Stores oral interview specific information for this resource </summary>
    /// <remarks> Object created by Mark V Sullivan (2006) for University of Florida's Digital Library Center.</remarks>
    [Serializable]
    public class Oral_Interview_Info : iMetadata_Module
    {
        private string interviewDate;
        private string interviewee;
        private string interviewer;

        /// <summary> Constructor for a new instance of the Oral_Interview_Info class </summary>
        public Oral_Interview_Info()
        {
            // Do nothing by default
        }

        /// <summary> Gets the flag which indicates that there is data in this object that needs to be written to the METS file </summary>
        internal bool hasData
        {
            get
            {
                if (((interviewee != null) && (interviewee.Length > 0)) ||
                    ((interviewer != null) && (interviewer.Length > 0)) ||
                    ((interviewDate != null) && (interviewDate.Length > 0)))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary> Gets and sets the interviewee associated with this resource </summary>
        public string Interviewee
        {
            get { return interviewee ?? String.Empty; }
            set { interviewee = value; }
        }

        /// <summary> Gets and sets the interviewer associated with this resource </summary>
        public string Interviewer
        {
            get { return interviewer ?? String.Empty; }
            set { interviewer = value; }
        }

        /// <summary> Gets and sets the interview date associated with this resource </summary>
        public string Interview_Date
        {
            get { return interviewDate ?? String.Empty; }
            set { interviewDate = value; }
        }

        #region Methods/Properties to implement the iMetadata_Module interface

        /// <summary> Name for this metadata module </summary>
        /// <value> This always returns 'OralInterview'</value>
        public string Module_Name
        {
            get { return "OralInterview"; }
        }

        /// <summary> Gets the metadata search terms and values to be saved to the database
        /// to allow searching to occur over the data in this metadata module </summary>
        public List<KeyValuePair<string, string>> Metadata_Search_Terms
        {
            get
            {
                List<KeyValuePair<string, string>> metadataTerms = new List<KeyValuePair<string, string>>();

                // Add the interviewee
                if (!String.IsNullOrEmpty(interviewee))
                {
                    metadataTerms.Add(new KeyValuePair<string, string>("Interviewee", interviewee));
                }

                // Add the interviewer
                if (!String.IsNullOrEmpty(interviewer))
                {
                    metadataTerms.Add(new KeyValuePair<string, string>("Interviewer", interviewer));
                }

                return metadataTerms;
            }
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