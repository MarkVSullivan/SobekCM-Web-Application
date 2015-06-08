using SobekCM.Core.Navigation;

namespace SobekCM.Engine_Library.Results
{
    /// <summary> Arguments used to indicate the currently requested search result information </summary>
    public class Results_Arguments
    {
        /// <summary> Current aggregation code </summary>
        public string Aggregation { get; set; }

        /// <summary> Coordinate search string for a geographic search </summary>
        public string Coordinates { get; set; }

        /// <summary> Beginning of a date range, if the search includes
        /// a date range between two arbitrary dates </summary>
        /// <value>-1 if no year range</value>
        public long? DateRange_Date1 { get; set; }

        /// <summary> End of a date range, if the search includes
        /// a date range between two arbitrary dates </summary>
        /// <value>-1 if no year range</value>
        public long? DateRange_Date2 { get; set; }

        /// <summary> Beginning of the year range, if the search includes
        /// a date range between two years </summary>
        /// <value>-1 if no year range</value>
        public short? DateRange_Year1 { get; set; }

        /// <summary> End of the year range, if the search includes
        /// a date range between two years </summary>
        /// <value>-1 if no year range</value>
        public short? DateRange_Year2 { get; set; }

        /// <summary> Which page of the results to retrieve and return </summary>
        public ushort Page { get; set; }

        /// <summary> Number of results to dispay per page </summary>
        public int ResultsPerPage { get; set; }

        /// <summary> Search fields </summary>
        public string Search_Fields { get; set; }

        /// <summary> Precision to be used while performing a metadata search in the database </summary>
        public Search_Precision_Type_Enum Search_Precision { get; set; }

        /// <summary> Search string </summary>
        public string Search_String { get; set; }

        /// <summary> Submode for searching </summary>
        public Search_Type_Enum Search_Type { get; set; }

        /// <summary> Sort type employed for displaying result sets </summary>
        public short? Sort { get; set; }
    }
}
