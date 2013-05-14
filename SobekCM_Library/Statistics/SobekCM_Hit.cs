#region Using directives

using System;

#endregion

namespace SobekCM.Library.Statistics
{
    /// <summary> Information about a single hit against the SobekCM system </summary>
    /// <remarks> This class is used by the SobekCM Stats Reader app </remarks>
    public class SobekCM_Hit : IComparable<SobekCM_Hit>
    {
        /// <summary> Constructor for a new instance of the SobekCM_Hit class </summary>
        /// <param name="Time"> Date/Time of the hit against the web server </param>
        /// <param name="IP"> IP address from which the query was issued </param>
        /// <param name="Query_String"> Query string (includes the rewritten URL portion )</param>
        /// <param name="UFDC_URL"> Incoming URL which was queried </param>
        /// <param name="UserAgent"> All the information about the user's browser/settings </param>
        public SobekCM_Hit(DateTime Time, string IP, string Query_String, string UFDC_URL, string UserAgent)
        {
            this.Time = Time;
            this.IP = IP;
            this.Query_String = Query_String;
            this.UFDC_URL = UFDC_URL;
            this.UserAgent = UserAgent;
        }

        /// <summary> All the information about the user's browser/settings </summary>
        /// <remarks> This is used to determine if this is a search engine robot </remarks>
        public string UserAgent { get; private set; }

        /// <summary> Date/Time of the hit against the web server </summary>
        public DateTime Time { get; private set; }

        /// <summary> IP address from which the query was issued </summary>
        public string IP { get; private set; }

        /// <summary> Query string (includes the rewritten URL portion ) </summary>
        public string Query_String { get; private set; }

        /// <summary> Incoming URL which was queried </summary>
        public string UFDC_URL { get; set; }

        #region IComparable<SobekCM_Hit> Members

        /// <summary> Method allows this hit to be compared to another hit </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(SobekCM_Hit other)
        {
            int result = Time.CompareTo(other.Time);
            return ((result == 0)) ? 1 : result;
        }

        #endregion
    }
}