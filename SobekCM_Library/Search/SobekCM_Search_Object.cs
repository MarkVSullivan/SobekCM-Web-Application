#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using SobekCM.Library.Database;
using SobekCM.Library.Navigation;

#endregion

namespace SobekCM.Library.Search
{
    /// <summary> Object encapsulates all the information about a single search against a SobekCM library </summary>
    public class SobekCM_Search_Object : IEquatable<SobekCM_Search_Object>
    {
        #region SobekCM_Link_Enum enum

        public enum SobekCM_Link_Enum 
        {
            AND = 0,
            OR,
            AND_NOT
        }

        #endregion

        #region SobekCM_Term_Enum enum

        public enum SobekCM_Term_Enum 
        {
            Affiliation = 0,
            ALEPH_Number,
            Anywhere,
            Attribution,
            Author,
            BibID,
            City,
            Country,
            County,
            Donor,
            Frequency,
            Genre,
            Identifier,
            Language,
            Notes,
            OCLC_Number,
            Place_of_Publication,
            Publisher,
            Spatial_Coverage,
            State,
            Subject_Keywords,
            Target_Audience,
            Tickler,
            Title,   
            Tracking_Box,
            Type 
        }

        #endregion

        public string Aggregation;
        public SobekCM_Link_Enum First_Link;
        public SobekCM_Term_Enum First_Term;
        public string First_Value;
        public SobekCM_Term_Enum Fourth_Term;
        public string Fourth_Value;
        public string Institution;
        public Search_Precision_Type_Enum Search_Precision;
        public SobekCM_Link_Enum Second_Link;
        public SobekCM_Term_Enum Second_Term;
        public string Second_Value;
        public SobekCM_Link_Enum Third_Link;
        public SobekCM_Term_Enum Third_Term;
        public string Third_Value;

        public SobekCM_Search_Object( )
        {
            // Set the default links
            First_Link = SobekCM_Link_Enum.AND;
            Second_Link = SobekCM_Link_Enum.AND;
            Third_Link = SobekCM_Link_Enum.AND;

            // Set the default terms
            First_Term = new SobekCM_Term_Enum(); //Settings.SMaRT_UserSettings.Discovery_Panel_Search_Term1;
            Second_Term = new SobekCM_Term_Enum(); //Settings.SMaRT_UserSettings.Discovery_Panel_Search_Term2;
            Third_Term = new SobekCM_Term_Enum(); //Settings.SMaRT_UserSettings.Discovery_Panel_Search_Term3;
            Fourth_Term = new SobekCM_Term_Enum(); //Settings.SMaRT_UserSettings.Discovery_Panel_Search_Term4;

            // Set the default values
            First_Value = String.Empty;
            Second_Value = String.Empty;
            Third_Value = String.Empty;
            Fourth_Value = String.Empty;

            // Set the default aggregation and institutions
            Aggregation = String.Empty;
            Institution = String.Empty;

            // Set default search precision
            Search_Precision = Search_Precision_Type_Enum.Inflectional_Form;
        }

        /// <summary> Constructor for a new instance of the SobekCM_Search_Object class </summary>
        /// <param name="Default_First_Term"></param>
        /// <param name="Default_Second_Term"></param>
        /// <param name="Default_Third_Term"></param>
        /// <param name="Default_Fourth_Term"></param>
        public SobekCM_Search_Object(SobekCM_Term_Enum Default_First_Term, SobekCM_Term_Enum Default_Second_Term, SobekCM_Term_Enum Default_Third_Term, SobekCM_Term_Enum Default_Fourth_Term)
        {
            // Set the default links
            First_Link = SobekCM_Link_Enum.AND;
            Second_Link = SobekCM_Link_Enum.AND;
            Third_Link = SobekCM_Link_Enum.AND;

            // Set the default terms
            First_Term = Default_First_Term;// Settings.SMaRT_UserSettings.Discovery_Panel_Search_Term1;
            Second_Term = Default_Second_Term; // Settings.SMaRT_UserSettings.Discovery_Panel_Search_Term2;
            Third_Term = Default_Third_Term; // Settings.SMaRT_UserSettings.Discovery_Panel_Search_Term3;
            Fourth_Term = Default_Fourth_Term; // Settings.SMaRT_UserSettings.Discovery_Panel_Search_Term4;

            // Set the default values
            First_Value = String.Empty;
            Second_Value = String.Empty;
            Third_Value = String.Empty;
            Fourth_Value = String.Empty;

            // Set the default aggregation and institutions
            Aggregation = String.Empty;
            Institution = String.Empty;

            // Set default search precision
            Search_Precision = Search_Precision_Type_Enum.Inflectional_Form;
        }

        #region Tracking searches employed by the SMaRT app

        /// <summary> Performs the search indicated by this search object, employing the procedures used for the tracking type functionality included in the SobekCM Management and Reporting Tool (SMaRT) windows application </summary>
        /// <returns> Dataset of all the results </returns>
        public DataSet Perform_Tracking_Search()
        {
            if ((First_Value.Length == 0) && (Second_Value.Length == 0) && ( Third_Value.Length == 0) && ( Fourth_Value.Length == 0))
            {
                string code = Aggregation;
                if (Aggregation.Length == 0)
                    code = Institution;

                if (code.Length > 0)
                {
                    try
                    {
                        DataSet dataSet = SobekCM_Database.Tracking_Get_Item_Aggregation_Browse(code);
                        return dataSet;
                    }
                    catch
                    {
                        return null;
                    }
                }
            }
            else
            {
                try
                {
                    DataSet dataSet = Perform_Database_Search();
                    return dataSet;
                }
                catch 
                {
                    return null;
                }
            }

            return null;
        }

        private DataSet Perform_Database_Search()
        {
            // Get the aggregation
            string code = Aggregation;
            if (Aggregation.Length == 0)
                code = Institution;

            // Build the search string and search fields string
            string searchString = First_Value + "|" + Second_Value + "|" + Third_Value + "|" + Fourth_Value;
            StringBuilder termStringBuilder = new StringBuilder();
            termStringBuilder.Append(term_to_code( First_Term ) + "|");
            termStringBuilder.Append(join_to_symbol( First_Link ) + term_to_code(Second_Term) + "|");
            termStringBuilder.Append(join_to_symbol( Second_Link ) + term_to_code(Third_Term) + "|");
            termStringBuilder.Append(join_to_symbol( Third_Link ) + term_to_code(Fourth_Term));
            string fieldString = termStringBuilder.ToString();


            List<string> terms = new List<string>();
            List<string> web_fields = new List<string>();
            List<int> db_fields = new List<int>();
            List<int> links = new List<int>();

            // Split the terms correctly
            SobekCM_Assistant.Split_Clean_Search_Terms_Fields(searchString, fieldString, Search_Type_Enum.Advanced, terms, web_fields, null, Search_Precision_Type_Enum.Contains, '|');

            // Get the count that will be used
            int actualCount = Math.Min(terms.Count, web_fields.Count);

            // If there are no terms, return an empty item collection
            if (terms.Count == 0)
                return null;

            // Special code for searching by bibid, oclc, or aleph
            if (actualCount == 1)
            {
                // Was this a OCLC search?
                if ((web_fields[0] == "OC") && (terms[0].Length > 0))
                {
                    bool is_number = terms[0].All(Char.IsNumber);

                    if (is_number)
                    {
                        long oclc = Convert.ToInt64(terms[0]);
                        return SobekCM_Database.Tracking_Items_By_OCLC_Number(oclc, null);
                    }
                }

                // Was this a ALEPH search?
                if ((web_fields[0] == "AL") && (terms[0].Length > 0))
                {
                    bool is_number = terms[0].All(Char.IsNumber);

                    if (is_number)
                    {
                        int aleph = Convert.ToInt32(terms[0]);
                        return SobekCM_Database.Tracking_Items_By_ALEPH_Number(aleph, null);
                    }
                }
            }

            // Step through all the web fields and convert to db fields
            for (int i = 0; i < actualCount; i++)
            {
                if (web_fields[i].Length > 1)
                {
                    // Find the joiner
                    if ((web_fields[i][0] == '+') || (web_fields[i][0] == '=') || (web_fields[i][0] == '-'))
                    {
                        if (i != 0)
                        {
                            if (web_fields[i][0] == '+')
                                links.Add(0);
                            if (web_fields[i][0] == '=')
                                links.Add(1);
                            if (web_fields[i][0] == '-')
                                links.Add(2);
                        }
                        web_fields[i] = web_fields[i].Substring(1);
                    }
                    else
                    {
                        if (i != 0)
                        {
                            links.Add(0);
                        }
                    }

                    // Find the db field number
                    //db_fields.Add( Settings.SMaRT_GlobalValues.Search_Fields.Metadata_Field_Number(web_fields[i]));
                }

                // Also add starting and ending quotes to all the valid searches
                if (terms[i].Length > 0)
                {
                    if ((terms[i].IndexOf("\"") < 0) && (terms[i].IndexOf(" ") < 0))
                    {
                        // Since this is a single word, see what type of special codes to include
                        switch (Search_Precision)
                        {
                            case Search_Precision_Type_Enum.Contains:
                                terms[i] = "\"" + terms[i] + "\"";
                                break;

                            case Search_Precision_Type_Enum.Inflectional_Form:
                                // If there are any non-characters, don't use inflectional for this term
                                bool inflectional = terms[i].All(Char.IsLetter);
                                if (inflectional)
                                {
                                    terms[i] = "FORMSOF(inflectional," + terms[i] + ")";
                                }
                                else
                                {
                                    terms[i] = "\"" + terms[i] + "\"";
                                }
                                break;

                            case Search_Precision_Type_Enum.Synonmic_Form:
                                terms[i] = "FORMSOF(thesaurus," + terms[i] + ")";
                                break;
                        }
                    }
                    else
                    {
                        if (Search_Precision != Search_Precision_Type_Enum.Exact_Match)
                        {
                            terms[i] = "\"" + terms[i] + "\"";
                        }
                    }
                }
            }

            // If this is an exact match, just do the search
            if (Search_Precision == Search_Precision_Type_Enum.Exact_Match)
            {
                return SobekCM_Database.Tracking_Metadata_Exact_Search(terms[0], db_fields[0], code);
            }

            // Finish filling up the fields and links
            while (links.Count < 9)
                links.Add(0);
            while (db_fields.Count < 10)
                db_fields.Add(-1);
            while (terms.Count < 10)
                terms.Add(String.Empty);

            // See if this is a simple search, which can use a more optimized search routine
            bool simplified_search = db_fields.All(field => field <= 0);

            // Perform either the simpler metadata search, or the more complex
            if (simplified_search)
            {
                StringBuilder searchBuilder = new StringBuilder();
                for (int i = 0; i < terms.Count; i++)
                {
                    if (terms[i].Length > 0)
                    {
                        if (i > 0)
                        {
                            if (i > links.Count)
                            {
                                searchBuilder.Append(" AND ");
                            }
                            else
                            {
                                switch (links[i - 1])
                                {
                                    case 0:
                                        searchBuilder.Append(" AND ");
                                        break;

                                    case 1:
                                        searchBuilder.Append(" OR ");
                                        break;

                                    case 2:
                                        searchBuilder.Append(" AND NOT ");
                                        break;
                                }
                            }
                        }

                        searchBuilder.Append(terms[i]);
                    }
                }

                return SobekCM_Database.Tracking_Metadata_Search(searchBuilder.ToString(), code);
                // OLD CODE WHEN USING THE SIMPLE METADATA SEARCH WHICH INCLUDES THE SAME LINK TYPE
                //return Database.SobekCM_Database.Perform_Metadata_Search(terms[0], terms[1], terms[2], terms[3],
                //  terms[4], terms[5], terms[6], terms[7], terms[8], terms[9], main_link, include_private, Current_Mode.Aggregation, Tracer);

            }
                
            return SobekCM_Database.Tracking_Metadata_Search(terms[0], db_fields[0], links[0], terms[1], db_fields[1], links[1], terms[2], db_fields[2], links[2], terms[3],
                                                             db_fields[3], links[3], terms[4], db_fields[4], links[4], terms[5], db_fields[5], links[5], terms[6], db_fields[6], links[6], terms[7], db_fields[7], links[7], terms[8], db_fields[8],
                                                             links[8], terms[9], db_fields[9], code);
        }

        private string join_to_symbol(SobekCM_Link_Enum join)
        {
            switch (join)
            {
                case SobekCM_Link_Enum.AND: return "+";
                case SobekCM_Link_Enum.OR: return "=";
                case SobekCM_Link_Enum.AND_NOT: return "-";
                default: return "+";
            }
        }

        private string term_to_code( SobekCM_Term_Enum term)
        {
            switch (term)
            {
                case SobekCM_Term_Enum.BibID: return "BI";
                case SobekCM_Term_Enum.OCLC_Number: return "OC";
                case SobekCM_Term_Enum.ALEPH_Number: return "AL";
                case SobekCM_Term_Enum.Anywhere: return "ZZ";
                case SobekCM_Term_Enum.Title: return "TI";
                case SobekCM_Term_Enum.Author: return "AU";
                case SobekCM_Term_Enum.Subject_Keywords: return "SU";
                case SobekCM_Term_Enum.Country: return "CO";
                case SobekCM_Term_Enum.State: return "ST";
                case SobekCM_Term_Enum.County: return "CT";
                case SobekCM_Term_Enum.City: return "CI";
                case SobekCM_Term_Enum.Place_of_Publication: return "PP";
                case SobekCM_Term_Enum.Spatial_Coverage: return "SP";
                case SobekCM_Term_Enum.Type: return "TY";
                case SobekCM_Term_Enum.Language: return "LA";
                case SobekCM_Term_Enum.Publisher: return "PU";
                case SobekCM_Term_Enum.Genre: return "GE";
                case SobekCM_Term_Enum.Target_Audience: return "TA";
                case SobekCM_Term_Enum.Donor: return "DO";
                case SobekCM_Term_Enum.Attribution: return "AT";
                case SobekCM_Term_Enum.Tickler: return "TL";
                case SobekCM_Term_Enum.Notes: return "NO";
                case SobekCM_Term_Enum.Identifier: return "ID";
                case SobekCM_Term_Enum.Affiliation: return "AF";
                case SobekCM_Term_Enum.Frequency: return "FR";
                case SobekCM_Term_Enum.Tracking_Box: return "TB";
                default: return "ZZ";
            }
        }

        #endregion

        #region IEquatable<SobekCM_Search_Object> Members

        /// <summary> Determines if this search is equal to another search </summary>
        /// <param name="other"> Another search object for comparison purposes  </param>
        /// <returns> TRUE if the searches are the same, otherwise FALSE </returns>
        public bool Equals(SobekCM_Search_Object other)
        {
            if (other == null)
                return false;

            if ((First_Value != other.First_Value) || (Second_Value != other.Second_Value) || (Third_Value != other.Third_Value) || (Fourth_Value != other.Fourth_Value))
                return false;
            if ((First_Term != other.First_Term) || (Second_Term != other.Second_Term) || (Third_Term != other.Third_Term) || (Fourth_Term != other.Fourth_Term))
                return false;
            if ((First_Link != other.First_Link) || (Second_Link != other.Second_Link) || (Third_Link != other.Third_Link))
                return false;
            if ((Aggregation != other.Aggregation) || (Institution != other.Institution))
                return false;
            if (Search_Precision != other.Search_Precision)
                return false;

            return true;
        }

        #endregion
    }
}
