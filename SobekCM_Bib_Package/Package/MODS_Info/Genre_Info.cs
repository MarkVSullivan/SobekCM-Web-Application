using System;
using System.Collections.Generic;
using System.Text;
using SobekCM.Bib_Package.MARC;

namespace SobekCM.Bib_Package.Bib_Info
{
    /// <summary>Stores the information about any genre associated with this digital resource. </summary>
    /// <remarks>This class extends the <see cref="XML_Node_Base_Type"/> class for writing to XML </remarks>
    [Serializable]
    public class Genre_Info : XML_Node_Base_Type, IEquatable<Genre_Info>
    {
        private string genre_term;
        private string authority;
        private string language;

        /// <summary> Constructor creates a new instance of the Genre_Info class </summary>
        public Genre_Info()
        {
            // Do nothing
        }

        /// <summary> Constructor creates a new instance of the Genre_Info class </summary>
        /// <param name="Genre_Term">Genre term for this item</param>
        /// <param name="Authority">Authority for this genre term</param>
        public Genre_Info( string Genre_Term, string Authority )
        {
            genre_term = Genre_Term;
            authority = Authority;
        }

        /// <summary> Gets or sets this genre term </summary>
        public string Genre_Term
        {
            get { return genre_term ?? String.Empty; }
            set { genre_term = value; }
        }

        /// <summary> Gets or sets this genre term </summary>
        /// <remarks> There is no controlled list for this.  In general <i>marcgt</i> is used to represent controlled genres related to leader and 006/008 fields from the MARC record</remarks>
        public string Authority
        {
            get { return authority ?? String.Empty; }
            set { authority = value; }
        }

        /// <summary> Gets or sets the language for this genre term </summary>
        public string Language
        {
            get { return language ?? String.Empty; }
            set { language = value; }
        }


        #region IEquatable<Genre_Info> Members

        /// <summary> Compares this object with another similarly typed object </summary>
        /// <param name="other">Similarly types object </param>
        /// <returns>TRUE if the two objects are sufficiently similar</returns>
        public bool Equals(Genre_Info other )
        {
            if (Genre_Term == other.Genre_Term)
                return true;
            else
                return false;
        }

        #endregion  

        
        /// <summary> Returns this genre as a general string for display purposes </summary>
        /// <returns> This object in string format </returns>
        public override string ToString()
        {
            if (String.IsNullOrEmpty(genre_term))
                return String.Empty;

            if ( !String.IsNullOrEmpty(authority) )
            {
                return genre_term + " ( <i>" + authority + "</i> )";
            }
            else
            {
                return genre_term;
            }
        }


        /// <summary> Writes this genre as MODS to a writer writing to a stream ( either a file or web response stream )</summary>
        /// <param name="returnValue"> Writer to the MODS building stream </param>
        internal void Add_MODS( System.IO.TextWriter returnValue)
        {
            if (String.IsNullOrEmpty(genre_term))
                return;

            returnValue.Write( "<mods:genre");
            base.Add_ID(returnValue);
            if (!String.IsNullOrEmpty(authority))
                returnValue.Write(" authority=\"" + authority + "\"");
            if (!String.IsNullOrEmpty(language))
                returnValue.Write(" language=\"" + language + "\"");
            returnValue.Write(">" + base.Convert_String_To_XML_Safe(genre_term) + "</mods:genre>\r\n");
        }

        internal MARC_Field to_MARC_HTML()
        {
            if ((authority == "marcgt") || ( genre_term.Trim().Length == 0 ))               
                return null;

            MARC_Field returnValue = new MARC_Field();
            returnValue.Tag = 655;

            string second_indicator = " ";
            string authority_builder = String.Empty;
            if (!String.IsNullOrEmpty(authority))
            {
                switch (authority.ToLower())
                {
                    case "lcsh":
                        second_indicator = "0";
                        break;

                    case "lcshac":
                        second_indicator = "1";
                        break;

                    case "mesh":
                        second_indicator = "2";
                        break;

                    case "nal":
                        second_indicator = "3";
                        break;

                    case "csh":
                        second_indicator = "5";
                        break;

                    case "rvm":
                        second_indicator = "6";
                        break;

                    case "local":
                        second_indicator = "9";
                        break;

                    case "":
                        second_indicator = "4";
                        break;

                    default:
                        authority_builder = " |2 " + authority;
                        second_indicator = "7";
                        break;
                }
            }
            returnValue.Indicators = " " + second_indicator;
            returnValue.Control_Field_Value = "|a " + genre_term + authority_builder;
            return returnValue;
        }
    }
}
