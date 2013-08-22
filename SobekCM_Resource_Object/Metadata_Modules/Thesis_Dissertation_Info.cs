#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#endregion

namespace SobekCM.Resource_Object.Metadata_Modules
{
    /// <summary> Class stores all the Electronic Thesis or Disseration (ETD) specific 
    /// information, such as graduation date, degree, and members of the Thesis committee  </summary>
    [Serializable]
    public class Thesis_Dissertation_Info : iMetadata_Module
    {
        #region Thesis_Degree_Level_Enum enum

        /// <summary> Enumeration holds the level of the degree ( masters or doctorate ) 
        /// pursued along with this ETD work </summary>
        public enum Thesis_Degree_Level_Enum : byte
        {
            /// <summary> Level of this ETD work is unknown </summary>
            Unknown,

            /// <summary> This is a Masters-level thesis </summary>
            Masters,

            /// <summary> This is a Doctorate-level dissertation </summary>
            Doctorate,

			/// <summary> This is a Bachelors-level thesis </summary>
			Bachelors
        }

        #endregion

        private string committeeChair;
        private string committeeCoChair;
        private List<string> committeeMember;
        private string degree;
        private string degreeDiscipline;
        private string degreeGrantor;
        private Thesis_Degree_Level_Enum degreeLevel;
        private Nullable<DateTime> graduationDate;

        /// <summary> Constructor for a new instance of the Thesis_Dissertation_Info class  </summary>
        public Thesis_Dissertation_Info()
        {
            degreeLevel = Thesis_Degree_Level_Enum.Unknown;
        }

        /// <summary> The name of the committee chair, surname first.  </summary>
        public string Committee_Chair
        {
            get { return committeeChair ?? String.Empty; }
            set { committeeChair = value; }
        }

        /// <summary> The name of the committee co-chair, surname first. </summary>
        public string Committee_Co_Chair
        {
            get { return committeeCoChair ?? String.Empty; }
            set { committeeCoChair = value; }
        }

        /// <summary> Returns the number of committee members </summary>
        /// <remarks>This should be used rather than the Count property of the <see cref="Committee_Members"/> property.  Even if 
        /// there are no commitee members, the Committee_Members property creates a readonly collection to pass back out.</remarks>
        public int Committee_Members_Count
        {
            get
            {
                if (committeeMember == null)
                    return 0;
                else
                    return committeeMember.Count;
            }
        }

        /// <summary> Gets the collection of the names of the committee members, surname first. </summary>
        /// <remarks> You should check the count of commitee members first using the <see cref="Committee_Members_Count"/> property before using this property.
        /// Even if there are no commitee members, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<string> Committee_Members
        {
            get
            {
                if (committeeMember == null)
                    committeeMember = new List<string>();
                return new ReadOnlyCollection<string>(committeeMember);
            }
        }


        /// <summary> The type of degree granted. For mapping to MARC, catalogers prefer the 
        /// abbreviated form, that is "M.A." rather than "Master of Arts". </summary>
        public string Degree
        {
            get { return degree ?? String.Empty; }
            set { degree = value; }
        }

        /// <summary> Name of the college or department. This is used to create two fields 
        /// in the cataloging record, a non-standard subject heading and a note. </summary>
        public string Degree_Discipline
        {
            get { return degreeDiscipline ?? String.Empty; }
            set { degreeDiscipline = value; }
        }

        /// <summary> The name of the institution in standard form </summary>
        public string Degree_Grantor
        {
            get { return degreeGrantor ?? String.Empty; }
            set { degreeGrantor = value; }
        }

        /// <summary> The student’s graduation date. This is 
        /// optional unless you want a note created in the cataloging record with this information. </summary>
        public Nullable<DateTime> Graduation_Date
        {
            get { return graduationDate; }
            set { graduationDate = value; }
        }

        /// <summary> Gets the level of this ETD work ( either 'Masters' or 'Doctorate' )  </summary>
        public Thesis_Degree_Level_Enum Degree_Level
        {
            get { return degreeLevel; }
            set { degreeLevel = value; }
        }

        /// <summary> Flag indicates if this object holds any data in the subfields </summary>
        public bool hasData
        {
            get
            {
                if ((!String.IsNullOrEmpty(committeeChair)) || (!String.IsNullOrEmpty(committeeCoChair)) ||
                    (!String.IsNullOrEmpty(degree)) || (!String.IsNullOrEmpty(degreeDiscipline)) || (!String.IsNullOrEmpty(degreeGrantor)) ||
                    (degreeLevel != Thesis_Degree_Level_Enum.Unknown) || (graduationDate.HasValue) ||
                    ((committeeMember != null) && (committeeMember.Count > 0)))
                    return true;
                else
                    return false;
            }
        }

        #region Methods/Properties to implement the iMetadata_Module interface

        /// <summary> Name for this metadata module </summary>
        /// <value> This always returns 'ThesisDissertation'</value>
        public string Module_Name
        {
            get { return GlobalVar.THESIS_METADATA_MODULE_KEY; }
        }

        /// <summary> Gets the metadata search terms and values to be saved to the database
        /// to allow searching to occur over the data in this metadata module </summary>
        public List<KeyValuePair<string, string>> Metadata_Search_Terms
        {
            get
            {
                List<KeyValuePair<string, string>> metadataTerms = new List<KeyValuePair<string, string>>();

                // Add the committeeChair
                if (!String.IsNullOrEmpty(committeeChair))
                {
                    metadataTerms.Add(new KeyValuePair<string, string>("ETD Committee", committeeChair));
                }

                // Add the committeeCoChair
                if (!String.IsNullOrEmpty(committeeCoChair))
                {
                    metadataTerms.Add(new KeyValuePair<string, string>("ETD Committee", committeeCoChair));
                }

                // Add the rest of the committee
                if ( committeeMember != null )
                {
                    foreach( string thisCommitteeMember in committeeMember )
                        metadataTerms.Add(new KeyValuePair<string, string>("ETD Committee", thisCommitteeMember));
                }

                // Add the degree
                if (!String.IsNullOrEmpty(degree))
                {
                    metadataTerms.Add(new KeyValuePair<string, string>("ETD Degree", degree));
                }

                // Add the degree discipline
                if (!String.IsNullOrEmpty(degreeDiscipline))
                {
                    if (degreeDiscipline.IndexOf(";") > 0)
                    {
                        string[] splitter = degreeDiscipline.Split(";".ToCharArray());
                        foreach( string thisSplit in splitter )
                            metadataTerms.Add(new KeyValuePair<string, string>("ETD Degree Discipline", thisSplit.Trim()));
                    }
                    else
                    {
                        metadataTerms.Add(new KeyValuePair<string, string>("ETD Degree Discipline", degreeDiscipline));
                    }
                    
                }

                // Add the degree grantor
                if (!String.IsNullOrEmpty(degreeGrantor))
                {
                    metadataTerms.Add(new KeyValuePair<string, string>("ETD Degree Grantor", degreeGrantor));
                }

                // Add the degree level
                switch( degreeLevel )
                {
                    case Thesis_Degree_Level_Enum.Masters:
                        metadataTerms.Add(new KeyValuePair<string, string>("ETD Degree Level", "Masters"));
                        break;

                    case Thesis_Degree_Level_Enum.Doctorate:
                        metadataTerms.Add(new KeyValuePair<string, string>("ETD Degree Level", "Doctorate"));
                        break;

					case Thesis_Degree_Level_Enum.Bachelors:
						metadataTerms.Add(new KeyValuePair<string, string>("ETD Degree Level", "Bechelors"));
						break;
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

        /// <summary> Clears the list of committee members </summary>
        public void Clear_Committee_Members()
        {
            committeeMember.Clear();
        }

        /// <summary> Adds the name for a single committee member </summary>
        /// <param name="Name"> Name of the committee member to add, surname first </param>
        public void Add_Committee_Member(string Name)
        {
            if (committeeMember == null)
                committeeMember = new List<string>();
            if (!committeeMember.Contains(Name))
                committeeMember.Add(Name);
        }
    }
}