using System;
using System.Collections.Specialized;
using System.Linq;
using SobekCM.Core.Navigation;

namespace SobekCM.Core.Results
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
        public uint ResultsPerPage { get; set; }

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

        /// <summary> Flag indicates if this request should look in the cache, and subsequently store the
        /// results in the cache </summary>
        public bool Use_Cache { get; set; }

        /// <summary> Constructor for a new instance of this Results_Arguments class </summary>
        public Results_Arguments()
        {
            // Set some first defaults
            Sort = 0;
            Page = 1;
            ResultsPerPage = 20;
            Search_Precision = Search_Precision_Type_Enum.Inflectional_Form;
            Search_Type = Search_Type_Enum.Basic;
            Aggregation = "all";
            Use_Cache = true;
        }

        /// <summary> Constructor for a new instance of this Results_Arguments class </summary>
        /// <param name="QueryString"></param>
        public Results_Arguments(NameValueCollection QueryString)
        {
            // Set some first defaults
            Sort = 0;
            Page = 1;
            ResultsPerPage = 20;
            Search_Precision = Search_Precision_Type_Enum.Inflectional_Form;
            Search_Type = Search_Type_Enum.Basic;
            Aggregation = "all";
            Use_Cache = true;

            // Get the search field 
            if (!String.IsNullOrEmpty(QueryString["f"]))
                Search_Fields = QueryString["f"];
            else if (!String.IsNullOrEmpty(QueryString["fields"]))
                Search_Fields = QueryString["fields"];

            // Get the search string
            if (!String.IsNullOrEmpty(QueryString["t"]))
                Search_String = QueryString["t"];
            else if (!String.IsNullOrEmpty(QueryString["terms"]))
                Search_String = QueryString["terms"];

            // Get the precision
            if (!String.IsNullOrEmpty(QueryString["precision"]))
            {
                switch (QueryString["precision"].ToLower())
                {
                    case "resultslike":
                    case "like":
                        Search_Precision = Search_Precision_Type_Enum.Synonmic_Form;
                        break;

                    case "contains":
                        Search_Precision = Search_Precision_Type_Enum.Contains;
                        break;

                    case "exact":
                        Search_Precision = Search_Precision_Type_Enum.Exact_Match;
                        break;
                }
            }

            // Collect the coordinate information from the URL query string
            if ( !String.IsNullOrEmpty(QueryString["coord"]))
            {
                string coordinates_test = QueryString["coord"].Trim();
                string[] terms = coordinates_test.Split(",".ToCharArray());
                if (terms.Length >= 2)
                {
                    double lat1 = 1000;
                    double long1 = 1000;
                    double lat2 = 1000;
                    double long2 = 1000;
                    if (terms.Length < 4)
                    {
                        lat1 = Convert.ToDouble(terms[0]);
                        lat2 = lat1;
                        long1 = Convert.ToDouble(terms[1]);
                        long2 = long1;
                    }
                    if (terms.Length >= 4)
                    {
                        if (terms[0].Length > 0)
                            lat1 = Convert.ToDouble(terms[0]);
                        if (terms[1].Length > 0)
                            long1 = Convert.ToDouble(terms[1]);
                        if (terms[2].Length > 0)
                            lat2 = Convert.ToDouble(terms[2]);
                        if (terms[3].Length > 0)
                            long2 = Convert.ToDouble(terms[3]);
                    }

                    // If either point is valid, continue
                    if (((lat1 != 1000) && (long1 != 1000)) || ((lat2 != 1000) && (long2 != 1000)))
                    {
                        // If just the first point is valid, use that
                        if ((lat2 == 1000) || (long2 == 1000))
                        {
                            lat2 = lat1;
                            long2 = long1;
                        }

                        // If just the second point is valid, use that
                        if ((lat1 == 1000) || (long1 == 1000))
                        {
                            lat1 = lat2;
                            long1 = long2;
                        }

                        if ((lat1 == lat2) && (long1 == long2))
                            Coordinates = lat1 + "," + long1;
                        else
                            Coordinates = lat1 + "," + long1 + "," + lat2 + "," + long2;

                        Search_Type = Search_Type_Enum.Map;
                        Search_Fields = String.Empty;
                        Search_String = String.Empty;
                    }
                }
            }

            // Check for any page value
            if ((!String.IsNullOrEmpty(QueryString["p"])) || (!String.IsNullOrEmpty(QueryString["page"])))
            {
                string page_test = !String.IsNullOrEmpty(QueryString["p"]) ? QueryString["p"] : QueryString["page"];
                if (is_String_Number(page_test))
                {
                    ushort page_result;
                    if (UInt16.TryParse(page_test, out page_result))
                    {
                        Page = Math.Max(page_result, ((ushort) 1));
                    }
                }
            }

            // Check for any sort value
            if (( !String.IsNullOrEmpty(QueryString["o"])) || ( !String.IsNullOrEmpty(QueryString["sort"])))
            {
                string sort_test = !String.IsNullOrEmpty(QueryString["o"]) ? QueryString["o"] : QueryString["sort"];
                if (is_String_Number(sort_test))
                {
                    short sort_result;
                    if ( Int16.TryParse(sort_test, out sort_result))
                        Sort = sort_result;
                }
            }

            // Get the number of results per page
            if (!String.IsNullOrEmpty(QueryString["pageSize"]))
            {
                string pageSize_test = QueryString["pageSize"].Trim();
                if (is_String_Number(pageSize_test))
                {
                    uint pageSize_result;
                    if (UInt32.TryParse(pageSize_test, out pageSize_result))
                        ResultsPerPage = pageSize_result;
                }
            }

            // Collect any date range that may have existed
            if (QueryString["yr1"] != null)
            {
                short year1;
                if (Int16.TryParse(QueryString["yr1"], out year1))
                    DateRange_Year1 = year1;
            }
            if (QueryString["yr2"] != null)
            {
                short year2;
                if (Int16.TryParse(QueryString["yr2"], out year2))
                    DateRange_Year2 = year2;
            }
            if ((DateRange_Year1.HasValue) && (DateRange_Year2.HasValue) && (DateRange_Year1.Value > DateRange_Year2.Value))
            {
                short temp = DateRange_Year1.Value;
                DateRange_Year1 = DateRange_Year2.Value;
                DateRange_Year2 = temp;
            }
            if (QueryString["da1"] != null)
            {
                long date1;
                if (Int64.TryParse(QueryString["da1"], out date1))
                    DateRange_Date1 = date1;
            }
            if (QueryString["da2"] != null)
            {
                long date2;
                if (Int64.TryParse(QueryString["da2"], out date2))
                    DateRange_Date2 = date2;
            }

            // Was a search string and fields included?
            if ((Search_String.Length > 0) && (Search_Fields.Length > 0))
            {
                Search_Type = Search_Type_Enum.Advanced;
            }
            else
            {
                Search_Fields = "ZZ";
            }

            // If no search term, look foor the TEXT-specific term
            if (Search_String.Length == 0)
            {
                if ( !String.IsNullOrEmpty(QueryString["text"]))
                {
                    Search_String = QueryString["text"].Trim();
                    Search_Type = Search_Type_Enum.Full_Text;
                }
            }     

        }

        /// <summary> Method checks to see if this string contains only numbers </summary>
        /// <param name="TestString"> string to check for all numerals </param>
        /// <returns> TRUE if the string is made of all numerals </returns>
        /// <remarks> This just steps through each character in the string and tests with the Char.IsNumber method</remarks>
        private static bool is_String_Number(string TestString)
        {
            // Step through each character and return false if not a number
            return TestString.All(Char.IsNumber);
        }
    }
}
