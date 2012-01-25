using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using SobekCM.Bib_Package.MARC;

namespace SobekCM.Bib_Package.Bib_Info
{
    /// <summary> Standard subject keywords for this item </summary>
    [Serializable]
    public class Subject_Info_Standard : Subject_Standard_Base
    {
        private List<string> occupations;

        /// <summary> Constructor for a new instance of the standard subject class </summary>
        public Subject_Info_Standard()
        {
            // Do nothing
        }

        /// <summary> Constructor for a new instance of the standard subject class </summary>
        /// <param name="Topic">Topical subject</param>
        /// <param name="Authority">Authority for this topic subject keyword</param>
        public Subject_Info_Standard( string Topic, string Authority )
        {
            topics = new List<string>();
            topics.Add(Topic);
            authority = Authority;
        }

        /// <summary> Gets flag indicating if there is any data in this subject object </summary>
        public new bool hasData
        {
            get
            {
                if (( base.hasData ) || (( occupations != null ) && ( occupations.Count > 0 )))
                    return true;

                return false;
            }
        }

        /// <summary> Gets the number of topical subjects </summary>
        /// <remarks>This should be used rather than the Count property of the <see cref="Occupations"/> property.  Even if 
        /// there are no occupational subjects, the Occupations property creates a readonly collection to pass back out.</remarks>
        public int Occupations_Count
        {
            get
            {
                if (occupations == null)
                    return 0;
                else
                    return occupations.Count;
            }
        }

        /// <summary> Gets the list of occupational subjects </summary>
        /// <remarks> You should check the count of topical subjects first using the <see cref="Occupations_Count"/> before using this property.
        /// Even if there are no occupational subjects, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<string> Occupations
        {
            get
            {
                if (occupations == null)
                    return new ReadOnlyCollection<string>(new List<string>());
                else
                    return new ReadOnlyCollection<string>(occupations);
            }
        }

        /// <summary> Add a new occupational subject keyword to this subject </summary>
        /// <param name="NewTerm">New subject term to add</param>
        public void Add_Occupation(string NewTerm)
        {
            if (occupations == null)
                occupations = new List<string>();

            if (!occupations.Contains(NewTerm))
                occupations.Add(NewTerm);
        }

        /// <summary> Clear all the occupational subject keywords from this subject </summary>
        public void Clear_Occupations()
        {
            if (occupations != null)
                occupations.Clear();
        }

        /// <summary> Indicates this is the standard subclass of Subject_Info </summary>
        public override Subject_Info_Type Class_Type
        {
            get { return Subject_Info_Type.Standard; }
        }

        /// <summary> Write the subject information out to string format</summary>
        /// <returns> This subject expressed as a string</returns>
        /// <remarks> The scheme is included in this string</remarks>
        public override string ToString()
        {
            return ToString(true);
        }

        /// <summary> Write the subject information out to string format</summary>
        /// <param name="Include_Scheme"> Flag indicates whether the scheme should be included</param>
        /// <returns> This subject expressed as a string</returns>
        public override string ToString(bool Include_Scheme)
        {
            StringBuilder builder = new StringBuilder();

            if (occupations != null)
            {
                foreach (string thisOccupation in occupations)
                {
                    if (builder.Length > 0)
                    {
                        builder.Append(" -- " + thisOccupation);
                    }
                    else
                    {
                        builder.Append(thisOccupation);
                    }
                }
            }

            builder.Append(base.To_Base_String());

            if (Include_Scheme)
            {
                if ( !String.IsNullOrEmpty(authority))
                    builder.Append(" ( " + authority + " )");
            }
            
            return Convert_String_To_XML_Safe(builder.ToString());
        }

        internal override string To_GSA_DublinCore(string indent)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(To_GSA(ToString(), "dc.Subject", indent));
            if (geographics != null)
            {
                foreach (string thisGeographic in geographics)
                    builder.Append(To_GSA(thisGeographic, "dc.Coverage^spatial", indent));
            }
            return builder.ToString();
        }

        internal override string To_GSA_SobekCM(string indent)
        {
            return String.Empty;
        }

        internal override void Add_MODS(System.IO.TextWriter results)
        {
            if ((( occupations == null ) || (occupations.Count == 0)) && 
                ((genres == null ) || (genres.Count == 0)) && 
                ((temporals == null ) || (temporals.Count == 0)) && 
                (( topics == null ) || (topics.Count == 0)) && 
                (( geographics == null ) || (geographics.Count == 0)))
                return;

            results.Write( "<mods:subject");
            base.Add_ID(results);
            if (!String.IsNullOrEmpty(language))
                results.Write(" lang=\"" + language + "\"");
            if (!String.IsNullOrEmpty(authority))
                results.Write(" authority=\"" + authority + "\"");
            results.Write(">\r\n");

            base.Add_Base_MODS( results);

            if (occupations != null)
            {
                foreach (string thisElement in occupations)
                {
                    results.Write("<mods:occupation>" + base.Convert_String_To_XML_Safe(thisElement) + "</mods:occupation>\r\n");
                }
            }

            results.Write("</mods:subject>\r\n");
        }


        internal override MARC_Field to_MARC_HTML()
        {
            MARC_Field returnValue = new MARC_Field();

            // Set the tag            
            if ((id.IndexOf("SUBJ") == 0) && ( id.Length >= 7 ))
            {
                string possible_tag = id.Substring(4, 3);
                try
                {
                    int possible_tag_number = Convert.ToInt16(possible_tag);
                    returnValue.Tag = possible_tag_number;
                }
                catch
                {

                }                
            }

            // Try to guess the tag, if there was no tag
            if ((returnValue.Tag <= 0) && ( Topics_Count == 0 ))
            {
                if ((Temporals_Count > 0) && (Genres_Count == 0) && (Geographics_Count == 0))
                    returnValue.Tag = 648;

                if ((Temporals_Count == 0) && (Genres_Count > 0) && (Geographics_Count == 0))
                    returnValue.Tag = 655;

                if ((Temporals_Count == 0) && (Genres_Count == 0) && ( Geographics_Count > 0))
                    returnValue.Tag = 651;
            }

            // 650 is the default
            if (returnValue.Tag <= 0)
            {
                returnValue.Tag = 650;
            }

            // No indicators 
            returnValue.Indicators = "  ";            
            bool first_field_assigned = false;
            string scale = String.Empty;
            StringBuilder fieldBuilder = new StringBuilder();
            StringBuilder fieldBuilder2 = new StringBuilder();

            // Whenever there is an occupation, it must map into the 656
            if (( occupations != null ) && ( occupations.Count > 0))
            {
                returnValue.Tag = 656;
                fieldBuilder.Append("|a ");
                foreach (string occupation in occupations)
                {
                    if (!first_field_assigned)
                    {
                        fieldBuilder.Append( occupation);
                        first_field_assigned = true;
                    }
                    else
                    {
                        fieldBuilder.Append(" -- " + occupation);
                    }
                }
                fieldBuilder.Append(" ");
            }

            switch (returnValue.Tag)
            {
                case 690:
                    first_field_assigned = assign_genres(first_field_assigned, returnValue.Tag, fieldBuilder, fieldBuilder2);
                    first_field_assigned = assign_topics(first_field_assigned, returnValue.Tag, fieldBuilder, fieldBuilder2, ref scale);
                    first_field_assigned = assign_geographics(first_field_assigned, returnValue.Tag, fieldBuilder, fieldBuilder2);
                    first_field_assigned = assign_temporals(first_field_assigned, returnValue.Tag, fieldBuilder, fieldBuilder2);
                    break;

                case 691:
                    first_field_assigned = assign_genres(first_field_assigned, returnValue.Tag, fieldBuilder, fieldBuilder2);
                    first_field_assigned = assign_geographics(first_field_assigned, returnValue.Tag, fieldBuilder, fieldBuilder2);
                    first_field_assigned = assign_topics(first_field_assigned, returnValue.Tag, fieldBuilder, fieldBuilder2, ref scale);
                    first_field_assigned = assign_temporals(first_field_assigned, returnValue.Tag, fieldBuilder, fieldBuilder2);
                    break;

                default:
                    first_field_assigned = assign_geographics(first_field_assigned, returnValue.Tag, fieldBuilder, fieldBuilder2);
                    first_field_assigned = assign_temporals(first_field_assigned, returnValue.Tag, fieldBuilder, fieldBuilder2);
                    first_field_assigned = assign_topics(first_field_assigned, returnValue.Tag, fieldBuilder, fieldBuilder2, ref scale);
                    first_field_assigned = assign_genres(first_field_assigned, returnValue.Tag, fieldBuilder, fieldBuilder2);
                    break;
            }

            if ( !String.IsNullOrEmpty(scale))
            {
                fieldBuilder2.Append("|x " + scale + " ");
            }

            fieldBuilder.Append(fieldBuilder2.ToString());
            if (fieldBuilder.Length > 2)
            {
                fieldBuilder.Remove(fieldBuilder.Length - 1, 1);
                fieldBuilder.Append(". ");
            }

            base.Add_Source_Indicator(returnValue, fieldBuilder);
            returnValue.Control_Field_Value = fieldBuilder.ToString().Trim();

            return returnValue;
        }

        private bool assign_geographics(bool first_field_assigned, int tag, StringBuilder fieldBuilder, StringBuilder fieldBuilder2)
        {
            if (geographics != null)
            {
                foreach (string geo in geographics)
                {
                    if ((!first_field_assigned) && ((tag == 651) || (tag == 691)))
                    {
                        fieldBuilder.Append("|a " + geo + " ");
                        first_field_assigned = true;
                    }
                    else
                    {
                        fieldBuilder2.Append("|z " + geo + " ");
                    }
                }
            }
            return first_field_assigned;
        }

        private bool assign_temporals(bool first_field_assigned, int tag, StringBuilder fieldBuilder, StringBuilder fieldBuilder2 )
        {
            if (temporals != null)
            {
                foreach (string temporal in temporals)
                {
                    if ((!first_field_assigned) && (tag == 648))
                    {
                        fieldBuilder.Append("|a " + temporal + " ");
                        first_field_assigned = true;
                    }
                    else
                    {
                        fieldBuilder2.Append("|y " + temporal + " ");
                    }
                }
            }
            return first_field_assigned;
        }

        private bool assign_topics(bool first_field_assigned, int tag, StringBuilder fieldBuilder, StringBuilder fieldBuilder2, ref string scale )
        {
            if (topics != null)
            {
                foreach (string topic in topics)
                {
                    if ((topic.IndexOf("1:") == 0) || topic.IndexOf(" scale") >= 0)
                        scale = topic;
                    else
                    {
                        if ((!first_field_assigned) && ((tag == 690) || (tag == 650) || (tag == 654) || (tag == 657)))
                        {
                            fieldBuilder.Append("|a " + topic + " ");
                            first_field_assigned = true;
                        }
                        else
                        {
                            fieldBuilder2.Append("|x " + topic + " ");
                        }
                    }
                }
            }
            return first_field_assigned;
        }

        private bool assign_genres(bool first_field_assigned, int tag, StringBuilder fieldBuilder, StringBuilder fieldBuilder2)
        {
            if (genres != null)
            {
                foreach (string form in genres)
                {
                    if ((!first_field_assigned) && (tag == 655))
                    {
                        fieldBuilder.Append("|a " + form + " ");
                        first_field_assigned = true;
                    }
                    else
                    {
                        fieldBuilder2.Append("|v " + form + " ");
                    }
                }
            }
            return first_field_assigned;
        }
    }
}
