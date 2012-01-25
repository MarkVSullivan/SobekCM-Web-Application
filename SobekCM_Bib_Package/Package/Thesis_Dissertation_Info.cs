using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SobekCM.Bib_Package
{
    /// <summary> Class stores all the Electronic Thesis or Disseration (ETD) specific 
    /// information, such as graduation date, degree, and members of the Thesis committee  </summary>
    [Serializable]
    public class Thesis_Dissertation_Info : XML_Writing_Base_Type
    {
        /// <summary> Enumeration holds the level of the degree ( masters or doctorate ) 
        /// pursued along with this ETD work </summary>
        public enum Thesis_Degree_Level_Enum : byte
        {
            /// <summary> Level of this ETD work is unknown </summary>
            Unknown,

            /// <summary> This is a Masters-level thesis </summary>
            Masters,

            /// <summary> This is a Doctorate-level dissertation </summary>
            Doctorate
        }

        private string committeeChair;
        private string committeeCoChair;
        private List<string> committeeMember;
        private Nullable<DateTime> graduationDate;
        private string degree;
        private string degreeDiscipline;
        private string degreeGrantor;
        private Thesis_Degree_Level_Enum degreeLevel;

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
            if ( !committeeMember.Contains( Name ))
                committeeMember.Add(Name);
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
                    ( degreeLevel != Thesis_Degree_Level_Enum.Unknown ) || ( graduationDate.HasValue ) ||
                    (( committeeMember != null ) && ( committeeMember.Count > 0 )))
                    return true;
                else
                    return false;
            }
        }

        public string To_Metadata_Section
        {
            get
            {
                StringBuilder resultBuilder = new StringBuilder();
                resultBuilder.AppendLine("<palmm:thesis>");
                if (!String.IsNullOrEmpty(committeeChair))
                    resultBuilder.AppendLine("<palmm:committeeChair>" + base.Convert_String_To_XML_Safe( committeeChair ) + "</palmm:committeeChair>");
                if (!String.IsNullOrEmpty(committeeCoChair))
                    resultBuilder.AppendLine("<palmm:committeeCoChair>" + base.Convert_String_To_XML_Safe(committeeCoChair) + "</palmm:committeeCoChair>");
                if (committeeMember != null)
                {
                    foreach (string thisCommitteeMember in committeeMember)
                    {
                        resultBuilder.AppendLine("<palmm:committeeMember>" + base.Convert_String_To_XML_Safe(thisCommitteeMember) + "</palmm:committeeMember>");
                    }
                }
                if (graduationDate.HasValue)
                {
                    string encoded_date = graduationDate.Value.Year + "-" + graduationDate.Value.Month.ToString().PadLeft(2, '0') + "-" + graduationDate.Value.Day.ToString().PadLeft(2, '0');
                    resultBuilder.AppendLine("<palmm:graduationDate>" + encoded_date + "</palmm:graduationDate>");
                }
                if (!String.IsNullOrEmpty(degree))
                    resultBuilder.AppendLine("<palmm:degree>" + base.Convert_String_To_XML_Safe(degree) + "</palmm:degree>");
                if (!String.IsNullOrEmpty(degreeDiscipline))
                    resultBuilder.AppendLine("<palmm:degreeDiscipline>" + base.Convert_String_To_XML_Safe(degreeDiscipline) + "</palmm:degreeDiscipline>");
                if (!String.IsNullOrEmpty(degreeGrantor))
                    resultBuilder.AppendLine("<palmm:degreeGrantor>" + base.Convert_String_To_XML_Safe(degreeGrantor) + "</palmm:degreeGrantor>");
                if ( degreeLevel == Thesis_Degree_Level_Enum.Masters )
                    resultBuilder.AppendLine("<palmm:degreeLevel>Masters</palmm:degreeLevel>");
                if (degreeLevel == Thesis_Degree_Level_Enum.Doctorate)
                    resultBuilder.AppendLine("<palmm:degreeLevel>Doctorate</palmm:degreeLevel>");
                resultBuilder.AppendLine("</palmm:thesis>");
                return resultBuilder.ToString();
            }
        }

    }
}
