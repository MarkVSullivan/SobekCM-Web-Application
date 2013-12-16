#region Using directives

using System;
using System.IO;
using System.Text;
using SobekCM.Resource_Object.MARC;

#endregion

namespace SobekCM.Resource_Object.Bib_Info
{
    /// <summary> Subject type enumeration tells the subclass </summary>
    public enum Subject_Info_Type
    {
        /// <summary> Undefined or unknown type of subject </summary>
        UNKNOWN = -1,

        /// <summary> Standard type of complex subject term  </summary>
        Standard = 1,

        /// <summary> Hierarchical spatial subject with continent, country, state, city, etc.. </summary>
        Hierarchical_Spatial,

        /// <summary> Cartographic information for a map, inluding scale and projection information </summary>
        Cartographics,

        /// <summary> A name used as a subject, i.e., if an entire book is about a person, that person would be the subject </summary>
        Name,

        /// <summary> A title used as a subject, i.e., if a book is written about a movie that movie title is the subject </summary>
        TitleInfo
    }

    /// <summary> A term or phrase representing the primary topic(s) on which a work is focused.  </summary>
    [Serializable]
    public abstract class Subject_Info : XML_Node_Base_Type
    {
        /// <summary> Protected field stores the authority or source for this subject </summary>
        protected string authority;

        /// <summary> Protected field stores the language of this complex subject </summary>
        protected string language;

        /// <summary> Constructor for the abstract class Subject_Info </summary>
        public Subject_Info()
        {
            id = String.Empty;
        }

        /// <summary> Gets and sets the language for this subject </summary>
        public string Language
        {
            get { return language ?? String.Empty; }
            set { language = value; }
        }

        /// <summary> Gets and sets the uncontrolled authority term </summary>
        public string Authority
        {
            get { return authority ?? String.Empty; }
            set { authority = value; }
        }

        /// <summary> Gets the subject type </summary>
        public abstract Subject_Info_Type Class_Type { get; }

        /// <summary> Analyzes this subject to determine the indicator based on the authority/source 
        /// when converting this subject into the equivalent MARC </summary>
        /// <param name="returnValue"> MARC Tag to add the indicator to </param>
        /// <param name="fieldBuilder"> Field builder to which the |2 source may be added if the source is not represented in the indicator </param>
        protected void Add_Source_Indicator(MARC_Field returnValue, StringBuilder fieldBuilder)
        {
            string second_indicator = " ";
            if (!String.IsNullOrEmpty(authority))
            {
                switch (authority.ToLower())
                {
                    case "lcnaf":
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
                        fieldBuilder.Append("|2 " + authority + " ");
                        second_indicator = "7";
                        break;
                }
            }

            returnValue.Indicators = returnValue.Indicators.Substring(0, 1) + second_indicator;
        }

        /// <summary> Writes this subject as a simple string </summary>
        /// <param name="Include_Scheme">Flag indicates if the scheme should be included</param>
        /// <returns> This subject returned as a simple string </returns>
        public abstract string ToString(bool Include_Scheme);

        internal abstract void Add_MODS(TextWriter results);

        internal abstract MARC_Field to_MARC_HTML();
    }
}