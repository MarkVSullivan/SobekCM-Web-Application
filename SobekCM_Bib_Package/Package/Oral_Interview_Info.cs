using System;
using System.Text;

namespace SobekCM.Bib_Package
{
	/// <summary> Stores oral interview specific information for this resource </summary>
	/// <remarks> Object created by Mark V Sullivan (2006) for University of Florida's Digital Library Center.</remarks>
    [Serializable]
	public class Oral_Interview_Info : XML_Writing_Base_Type
	{

		private string interviewee;
        private string interviewer;
        private string interviewDate;

		/// <summary> Constructor for a new instance of the Oral_Interview_Info class </summary>
		public Oral_Interview_Info()
		{
            // Do nothing by default
		}

		/// <summary> Gets the flag which indicates that there is data in this object that needs to be written to the METS file or GSA file</summary>
		internal bool hasData
		{
			get
			{
                if ((( interviewee != null ) && (interviewee.Length > 0)) || 
                    (( interviewer != null ) && (interviewer.Length > 0)) ||
                    (( interviewDate != null ) && (interviewDate.Length > 0)))
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
			get		{		return interviewee ?? String.Empty;		}
			set		{		interviewee = value;		}
		}

		/// <summary> Gets and sets the interviewer associated with this resource </summary>
		public string Interviewer
		{
            get { return interviewer ?? String.Empty; }
			set		{		interviewer =value;		}
		}

		/// <summary> Gets and sets the interview date associated with this resource </summary>
		public string Interview_Date
		{
            get { return interviewDate ?? String.Empty; }
			set		{		interviewDate= value;		}
		}


		/// <summary> Gets the Greenstone Archival format XML for this oral interview information associated with the resource </summary>
		internal string GSA_Interview_Metadata
		{
			get
			{
				StringBuilder results = new StringBuilder();

				string indent = "    ";

                // Add all the GSA metadata here
                results.Append( base.To_GSA(interviewee, "sobekcm.OralHistory^interviewee", indent));
                results.Append(base.To_GSA(interviewer, "sobekcm.OralHistory^interviewer", indent));
                results.Append(base.To_GSA(interviewDate, "sobekcm.OralHistory^interviewDate", indent));

				// Return the built information
				return results.ToString();

			}
		}

        internal void Add_METS( System.IO.TextWriter results)
        {
            if (!hasData)
                return;

            // Start the Administrative section
            results.Write( "<oral:interview>\r\n");

            // Add all the custom SobekCM specific data
            results.Write(toMETS( "oral:Interviewee", base.Convert_String_To_XML_Safe(interviewee)));
            results.Write(toMETS( "oral:Interviewer", base.Convert_String_To_XML_Safe(interviewer)));
            results.Write(toMETS( "oral:InterviewDate", base.Convert_String_To_XML_Safe(interviewDate)));

            // End the Administrative section
            results.Write( "</oral:interview>\r\n");
        }

        /// <summary> Helper method is used to create the METS from each individual data element </summary>
        /// <param name="mets_tag">Tag to use in the XML definition for this line</param>
        /// <param name="mets_value">Value to include in the METS tags</param>
        /// <returns>METS-compliant XML for this data</returns>
        internal static string toMETS( string mets_tag, string mets_value)
        {
            if ( !String.IsNullOrEmpty( mets_value ))
            {
                return "<" + mets_tag + ">" + mets_value + "</" + mets_tag + ">\r\n";
            }
            else
            {
                return String.Empty;
            }
        }
	}
}
