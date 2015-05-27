#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
			Bachelors,

			/// <summary> This is a Post-Doctorate-level dissertation </summary>
            PostDoctorate
        }

        #endregion

        private string committeeChair;
        private string committeeCoChair;
        private List<string> committeeMember;
        private string degree;
		private List<string> degreeDiscipline;
		private List<string> degreeDivision;
        private string degreeGrantor;
        private Thesis_Degree_Level_Enum degreeLevel;
        private Nullable<DateTime> graduationDate;
		private string graduationSemester;

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

		#region Committee member collection properties and methods

		/// <summary> Returns the number of committee members </summary>
        /// <remarks>This should be used rather than the Count property of the <see cref="Committee_Members"/> property.  Even if 
        /// there are no commitee members, the Committee_Members property creates a readonly collection to pass back out.</remarks>
        public int Committee_Members_Count
        {
            get {
	            return committeeMember == null ? 0 : committeeMember.Count;
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

		/// <summary> Clears the list of committee members </summary>
		public void Clear_Committee_Members()
		{
            if ( committeeMember != null )
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

		#endregion

		/// <summary> The type of degree granted. For mapping to MARC, catalogers prefer the 
        /// abbreviated form, that is "M.A." rather than "Master of Arts". </summary>
        public string Degree
        {
            get { return degree ?? String.Empty; }
            set { degree = value; }
        }

		#region Degree discipline collection properties and methods

		/// <summary> Returns the number of disciplines which best describes
		/// the degree/materials's content </summary>
		/// <remarks>This should be used rather than the Count property of the <see cref="Degree_Disciplines"/> property.  Even if 
		/// there are no degree disciplines, the Degree_Disciplines property creates a readonly collection to pass back out.</remarks>
		public int Degree_Disciplines_Count
		{
			get
			{
				return degreeDiscipline == null ? 0 : degreeDiscipline.Count;
			}
		}

		/// <summary> Gets the collection of disciplines which best describes
		/// the degree/materials's content </summary>
		/// <remarks> You should check the count of degree disciplines first using the <see cref="Degree_Disciplines_Count"/> property before using this property.
		/// Even if there are no degree disciplines, this property creates a readonly collection to pass back out.</remarks>
		public ReadOnlyCollection<string> Degree_Disciplines
		{
			get
			{
				if (degreeDiscipline == null)
					degreeDiscipline = new List<string>();
				return new ReadOnlyCollection<string>(degreeDiscipline);
			}
		}

		/// <summary> Adds a new degree discipline which best describes
		/// the degree/materials's content </summary>
		/// <param name="NewDiscipline"> New discipline to add to the collection </param>
		public void Add_Degree_Discipline(string NewDiscipline)
		{
			// If empty, do nothing
			if (String.IsNullOrEmpty(NewDiscipline))
				return;

			// Ensure the collection was already built
			if (degreeDiscipline == null)
				degreeDiscipline = new List<string>();

			// If it does not exist, add it
			if (!degreeDiscipline.Contains(NewDiscipline))
				degreeDiscipline.Add(NewDiscipline);
		}

		/// <summary> Clears the list of associated degree disciplines which best
		/// describes the degree/materials's content </summary>
		public void Clear_Degree_Disciplines()
		{
			if (degreeDiscipline != null)
				degreeDiscipline.Clear();
		}

		#endregion

		#region Degree divisions collection properties and methods

		/// <summary> Returns the number of divisions within the college which granted 
		/// the degree or worked on the material </summary>
		/// <remarks>This should be used rather than the Count property of the <see cref="Degree_Divisions"/> property.  Even if 
		/// there are no degree divisions, the Degree_Divisions property creates a readonly collection to pass back out.</remarks>
		public int Degree_Divisions_Count
		{
			get
			{
				return degreeDivision == null ? 0 : degreeDivision.Count;
			}
		}

		/// <summary> Gets the collection of the divisions within the college which granted
		/// the degree or worked on the material </summary>
		/// <remarks> You should check the count of degree divisions first using the <see cref="Degree_Divisions_Count"/> property before using this property.
		/// Even if there are no degree divisions, this property creates a readonly collection to pass back out.</remarks>
		public ReadOnlyCollection<string> Degree_Divisions
		{
			get
			{
				if (degreeDivision == null)
					degreeDivision = new List<string>();
				return new ReadOnlyCollection<string>(degreeDivision);
			}
		}

		/// <summary> Adds a new degree division within the college which granted the
		/// degre or worked on the material in some way </summary>
		/// <param name="NewDivision"> New division to add to the collection </param>
		public void Add_Degree_Division(string NewDivision)
		{
			// If empty, do nothing
			if (String.IsNullOrEmpty(NewDivision))
				return;

			// Ensure the collection was already built
			if (degreeDivision == null)
				degreeDivision = new List<string>();

			// If it does not exist, add it
			if ( !degreeDivision.Contains(NewDivision))
				degreeDivision.Add(NewDivision);
		}

		/// <summary> Clears the list of associated degree divisions within the college
		/// which granted the degree or worked on the material in some way  </summary>
		public void Clear_Degree_Divisions()
		{
			if (degreeDivision != null)
				degreeDivision.Clear();
		}

		#endregion

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

		/// <summary> Semster in which this student graduated </summary>
	    public string Graduation_Semester
		{
			get { return graduationSemester ?? String.Empty; }
			set { graduationSemester = value; }
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
	            return (!String.IsNullOrEmpty(committeeChair)) || (!String.IsNullOrEmpty(committeeCoChair)) ||
	                   (!String.IsNullOrEmpty(degree)) || (!String.IsNullOrEmpty(degreeGrantor)) ||
	                   (degreeLevel != Thesis_Degree_Level_Enum.Unknown) || (graduationDate.HasValue) ||
	                   (!String.IsNullOrEmpty(graduationSemester)) ||
	                   ((committeeMember != null) && (committeeMember.Count > 0)) ||
	                   ((degreeDiscipline != null) && (degreeDiscipline.Count > 0)) ||
	                   ((degreeDivision != null) && (degreeDivision.Count > 0));
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
	                metadataTerms.AddRange(committeeMember.Select(ThisCommitteeMember => new KeyValuePair<string, string>("ETD Committee", ThisCommitteeMember)));
                }

                // Add the degree
                if (!String.IsNullOrEmpty(degree))
                {
                    metadataTerms.Add(new KeyValuePair<string, string>("ETD Degree", degree));
                }

                // Add the degree disciplines
				if (degreeDiscipline != null)
				{
					metadataTerms.AddRange(degreeDiscipline.Select(ThisSplit => new KeyValuePair<string, string>("ETD Degree Discipline", ThisSplit.Trim())));
				}

				// Add the degree divisions
				if (degreeDivision != null)
				{
					metadataTerms.AddRange(degreeDivision.Select(ThisSplit => new KeyValuePair<string, string>("ETD Degree Division", ThisSplit.Trim())));
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
						metadataTerms.Add(new KeyValuePair<string, string>("ETD Degree Level", "Bachelors"));
						break;

					case Thesis_Degree_Level_Enum.PostDoctorate:
						metadataTerms.Add(new KeyValuePair<string, string>("ETD Degree Level", "Post-Doctorate"));
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


    }
}