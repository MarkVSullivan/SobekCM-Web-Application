using System;
using System.Text;

namespace SobekCM.Bib_Package.Bib_Info
{
	/// <summary> Stores the temporal subject information about a digital resource </summary>
	/// <remarks>Object created by Mark V Sullivan (2006) for University of Florida's Digital Library Center.</remarks>
    [Serializable]
	public class Temporal_Info : XML_Writing_Base_Type, IEquatable<Temporal_Info>
	{
		private int start_year, end_year;
		private string timeperiod;

		/// <summary> Constructor for an empty instance of the Temporal_Info class </summary>
		public Temporal_Info()
		{
			start_year = -1;
			end_year = -1;
		}

		/// <summary> Constructor for a new instance of the Temporal_Info class </summary>
		/// <param name="Start_Year">Start year for the year range</param>
		/// <param name="End_Year">End year for the year range</param>
		/// <param name="TimePeriod">Description of the time period (i.e. 'Post-WWII')</param>
		public Temporal_Info( int Start_Year, int End_Year, string TimePeriod )
		{
			start_year = Start_Year;
			end_year = End_Year;
			timeperiod = TimePeriod;
		}

		/// <summary> Gets and sets the start year for the year range </summary>
		public int Start_Year
		{
			get		{		return start_year;			}
			set		{		start_year = value;			}
		}

		/// <summary> Gets and sets the end year for the year range </summary>
		public int End_Year
		{
			get		{		return end_year;			}
			set		{		end_year = value;			}
		}

		/// <summary> Gets and sets the description of the time period (i.e. 'Post-WWII') </summary>
		public string TimePeriod
		{
			get		{		return timeperiod ?? String.Empty;			}
			set		{		timeperiod = value;			}
		}

        #region IEquatable<Note_Info> Members

        /// <summary> Compares this object with another similarly typed object </summary>
        /// <param name="other">Similarly types object </param>
        /// <returns>TRUE if the two objects are sufficiently similar</returns>
        public bool Equals(Temporal_Info other)
        {
            if (( !String.IsNullOrEmpty(timeperiod)) && ( timeperiod == other.TimePeriod ))
                return true;

            if ((String.IsNullOrEmpty(timeperiod)) && (other.Start_Year == start_year) && (other.end_year == end_year))
                return true;
            
            return false;
        }

        #endregion  

        /// <summary> Writes this temporal subject as SobekCM-formatted XML </summary>
        /// <param name="sobekcm_namespace"> Namespace to use for the SobekCM custom schema ( usually 'sobekcm' )</param>
        /// <param name="results"> Stream to write this temporal subject as SobekCM-formatted XML</param>
        internal void Add_SobekCM_Metadata(string sobekcm_namespace, System.IO.TextWriter results)
        {
            results.Write("<" + sobekcm_namespace + ":period");

            // Add the start year?
            if ( start_year > 0)
            {
                results.Write(" start=\"" + start_year + "\"");
            }

            // Add the end year?
            if ( end_year > 0)
            {
                results.Write(" end=\"" + end_year + "\"");
            }

            // Go on to the period part
            results.Write(">" + base.Convert_String_To_XML_Safe(timeperiod) + "</" + sobekcm_namespace + ":period>\r\n");
        }
		
		/// <summary> Returns the temporal subject data in Greenstone Archival format </summary>
		/// <returns>XML of this temporal subject data in Greenstone Archival format </returns>
		internal string toGSA()
		{
			if ( !String.IsNullOrEmpty(timeperiod))
			{
				if (( start_year > 0 ) && ( end_year > 0 ))
				{
                    return "<Metadata name=\"dc.Coverage^temporal\">" + base.Convert_String_To_XML_Safe(timeperiod) + " ( " + start_year + "-" + end_year + " )</Metadata>\r\n";
				}

				if (( start_year > 0 ) && ( end_year < 0 ))
				{
                    return "<Metadata name=\"dc.Coverage^temporal\">" + base.Convert_String_To_XML_Safe(timeperiod) + " ( " + start_year + "- )</Metadata>\r\n";
				}

				if (( start_year < 0 ) && ( end_year > 0 ))
				{
                    return "<Metadata name=\"dc.Coverage^temporal\">" + base.Convert_String_To_XML_Safe(timeperiod) + " ( -" + end_year + " )</Metadata>\r\n";
				}

				if (( start_year < 0 ) && ( end_year < 0 ))
				{
                    return "<Metadata name=\"dc.Coverage^temporal\">" + base.Convert_String_To_XML_Safe(timeperiod) + "</Metadata>\r\n";
				}
			}
			else
			{
				if (( start_year > 0 ) && ( end_year > 0 ))
				{
                    return "<Metadata name=\"dc.Coverage^temporal\">" + start_year + "-" + end_year + "</Metadata>\r\n";
				}

				if (( start_year > 0 ) && ( end_year < 0 ))
				{
                    return "<Metadata name=\"dc.Coverage^temporal\">" + start_year + "-</Metadata>\r\n";
				}

				if (( start_year < 0 ) && ( end_year > 0 ))
				{
                    return "<Metadata name=\"dc.Coverage^temporal\">-" + end_year + "</Metadata>\r\n";
				}

				if (( start_year < 0 ) && ( end_year < 0 ))
				{
					return String.Empty;
				}
			}

			return String.Empty;
		}
	}
}
