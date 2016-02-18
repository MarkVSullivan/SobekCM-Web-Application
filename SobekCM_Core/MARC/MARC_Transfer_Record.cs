#region License and Copyright

//          SobekCM MARC Library ( Version 1.2 )
//          
//          Copyright (2005-2012) Mark Sullivan. ( Mark.V.Sullivan@gmail.com )
//          
//          This file is part of SobekCM MARC Library.
//          
//          SobekCM MARC Library is free software: you can redistribute it and/or modify
//          it under the terms of the GNU Lesser Public License as published by
//          the Free Software Foundation, either version 3 of the License, or
//          (at your option) any later version.
//            
//          SobekCM MARC Library is distributed in the hope that it will be useful,
//          but WITHOUT ANY WARRANTY; without even the implied warranty of
//          MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//          GNU Lesser Public License for more details.
//            
//          You should have received a copy of the GNU Lesser Public License
//          along with SobekCM MARC Library.  If not, see <http://www.gnu.org/licenses/>.

#endregion

#region Using directives

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.MARC
{
    /// <summary> Stores all the information from a MARC21 record </summary>
    /// <remarks>Object created by Mark V Sullivan (2006) for University of Florida's Digital Library Center.</remarks>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("marcRecords")]
    public class MARC_Transfer_Record
    {
        private string controlNumber;
        private string leader;
        private Dictionary<int, List<MARC_Transfer_Field>> dictionary;

        /// <summary> Constructor for a new instance of the MARC_Transfer_Record class </summary>
        public MARC_Transfer_Record()
        {
            leader = String.Empty;
            Fields = new List<MARC_Transfer_Field>();
        }

        #region Public properties

        /// <summary> Control number for this record from the 001 field </summary>
        /// <remarks> This is used when importing directly from MARC records into the SobekCM library </remarks>
        [DataMember(EmitDefaultValue = false, Name = "id")]
        [XmlAttribute("id")]
        [ProtoMember(1)]
        public string Control_Number
        {
            get
            {
                if (controlNumber != null)
                    return controlNumber;

                // Is the dictionary built?
                if (dictionary == null)
                    dictionary = new Dictionary<int, List<MARC_Transfer_Field>>();
                if (dictionary.Count != Fields.Count)
                {
                    foreach (MARC_Transfer_Field field in Fields)
                    {
                        if ( dictionary.ContainsKey(field.Tag ))
                            dictionary[field.Tag].Add(field);
                        else
                            dictionary[field.Tag] = new List<MARC_Transfer_Field> {field};
                    }
                }

                // Does tag 1 exist?
                if (dictionary.ContainsKey(1))
                    controlNumber = dictionary[1][0].Control_Field_Value;
                else
                    controlNumber = String.Empty;

                return controlNumber;
            }
            set
            {
                controlNumber = value;
            }
        }

        /// <summary> Gets or sets the leader portion of this MARC21 Record </summary>
        [DataMember(EmitDefaultValue = false, Name = "leader")]
        [XmlAttribute("leader")]
        [ProtoMember(2)]
        public string Leader
        {
            get
            {
                // First, compute the overall length of this record
                int total_length = 0;
                int directory_length = 25;
                foreach (MARC_Transfer_Field thisTag in Fields)
                {
                    total_length = total_length + 5 + thisTag.Control_Field_Value.Length;
                    directory_length += 12;
                }

                string total_length_string = (total_length.ToString()).PadLeft(5, '0');
                string total_directory_string = (directory_length.ToString()).PadLeft(5, '0');

                if (leader.Length == 0)
                {
                    return total_length_string + "nam  22" + total_directory_string + "3a 4500";
                }
                return total_length_string + leader.Substring(5, 7) + total_directory_string + leader.Substring(17);
            }
            set { leader = value; }
        }

        /// <summary> Returns a list of all the MARC tags, sorted by tag number and
        /// ready to display as a complete MARC record </summary>
        [DataMember(EmitDefaultValue = false, Name = "fields")]
        [XmlArray("fields")]
        [XmlArrayItem("field", typeof(MARC_Transfer_Field))]
        [ProtoMember(4)]
        public List<MARC_Transfer_Field> Fields { get; set; }

        #endregion

        /// <summary> Adds a new field to this record </summary>
        /// <param name="New_Field"> New field to add </param>
        public void Add_Field(MARC_Transfer_Field New_Field)
        {
            if (New_Field == null)
                return;

            if (Fields == null)
                Fields = new List<MARC_Transfer_Field>();

            Fields.Add(New_Field);
        }

        #region Method overrides the ToString() method 

        /// <summary> Outputs this record as a string </summary>
        /// <returns></returns>
        public override string ToString()
        {
            // Create the StringBuilder
            StringBuilder returnVal = new StringBuilder(2000);

            // Add the leader
            returnVal.Append("LDR " + Leader + "\r\n");

            // Step through each field in the collection
            foreach (MARC_Transfer_Field thisField in Fields)
            {
                if (thisField.Subfield_Count == 0)
                {
                    if (thisField.Control_Field_Value.Length > 0)
                    {
                        returnVal.Append(thisField.Tag.ToString().PadLeft(3, '0') + " " + thisField.Control_Field_Value + "\r\n");
                    }
                }
                else
                {
                    returnVal.Append(thisField.Tag.ToString().PadLeft(3, '0') + " " + thisField.Indicators);

                    // Build the complete line
                    foreach (MARC_Transfer_Subfield thisSubfield in thisField.Subfields)
                    {
                        if (thisSubfield.Subfield_Code == ' ')
                        {
                            returnVal.Append(" " + thisSubfield.Data);
                        }
                        else
                        {
                            returnVal.Append(" |" + thisSubfield.Subfield_Code + " " + thisSubfield.Data);
                        }
                    }

                    returnVal.Append("\r\n");
                }
            }

            // Return the built string
            return returnVal.ToString();
        }

        #endregion

        #region Metho to return as HTML

        /// <summary> Returns this MARC record as HTML  </summary>
        /// <param name="Width"> Width of the resulting HTML-formatted MARC record </param>
        /// <returns> This MARC record formatted for HTML, returned as a string </returns>
        public string ToHTML(string Width)
        {
            // Start to build the HTML result
            StringBuilder results = new StringBuilder();
            results.Append("<table style=\"border:none; text-align:left; width:" + Width + ";\">\n");

            // Add the LEADER
            results.Append("  <tr class=\"trGenContent\">\n");
            results.Append("    <td style=\"width:33px;vertical-align:top;\">LDR</td>\n");
            results.Append("    <td style=\"width:26px;vertical-align:top;\">&nbsp;</td>\n");
            results.Append("    <td>" + Leader.Replace(" ", "^") + "</td>\n");
            results.Append("  </tr>");


            // Add all the FIELDS
            foreach (MARC_Transfer_Field thisTag in Fields)
            {
                results.Append("  <tr class=\"trGenContent\">\n");
                results.Append("    <td>" + thisTag.Tag.ToString().PadLeft(3, '0') + "</td>\n");
                results.Append("    <td style=\"color: green;\">" + thisTag.Indicators.Replace(" ", "&nbsp;&nbsp;&nbsp;") + "</td>\n");
                results.Append("    <td>");
                if ((thisTag.Tag == 8) || (thisTag.Tag == 7) || (thisTag.Tag == 6))
                {
                    results.Append(Convert_String_To_XML_Safe(thisTag.Control_Field_Value.Replace(" ", "^")));
                }
                else
                {
                    results.Append(Convert_String_To_XML_Safe(thisTag.Control_Field_Value).Replace("|a", "<span style=\"color:blue;\">|a</span>").
                                       Replace("|b", "<span style=\"color:blue;\">|b</span>").
                                       Replace("|c", "<span style=\"color:blue;\">|c</span>").
                                       Replace("|d", "<span style=\"color:blue;\">|d</span>").
                                       Replace("|e", "<span style=\"color:blue;\">|e</span>").
                                       Replace("|g", "<span style=\"color:blue;\">|g</span>").
                                       Replace("|x", "<span style=\"color:blue;\">|x</span>").
                                       Replace("|y", "<span style=\"color:blue;\">|y</span>").
                                       Replace("|z", "<span style=\"color:blue;\">|z</span>").
                                       Replace("|v", "<span style=\"color:blue;\">|v</span>").
                                       Replace("|h", "<span style=\"color:blue;\">|h</span>").
                                       Replace("|u", "<span style=\"color:blue;\">|u</span>").
                                       Replace("|f", "<span style=\"color:blue;\">|f</span>").
                                       Replace("|n", "<span style=\"color:blue;\">|n</span>").
                                       Replace("|2", "<span style=\"color:blue;\">|2</span>").
                                       Replace("|3", "<span style=\"color:blue;\">|3</span>").
                                       Replace("|w", "<span style=\"color:blue;\">|w</span>").
                                       Replace("|t", "<span style=\"color:blue;\">|t</span>").
                                       Replace("|q", "<span style=\"color:blue;\">|q</span>").
                                       Replace("|o", "<span style=\"color:blue;\">|o</span>").
                                       Replace("|i", "<span style=\"color:blue;\">|i</span>").
                                       Replace("|4", "<span style=\"color:blue;\">|4</span>"));
                }
                results.Append("</td>\n");
                results.Append("  </tr>");
            }
            results.Append("</table>\n");
            return results.ToString().Replace("&amp;bar;", "|");
        }

        /// <summary> Converts a basic string into an XML-safe string </summary>
        /// <param name="Element"> Element data to convert </param>
        /// <returns> Data converted into an XML-safe string</returns>
        public static string Convert_String_To_XML_Safe(string Element)
        {
            if (Element == null)
                return string.Empty;

            string xml_safe = Element;
            int i = xml_safe.IndexOf("&");
            while (i >= 0)
            {
                if ((i != xml_safe.IndexOf("&amp;", i, StringComparison.OrdinalIgnoreCase)) && 
                    (i != xml_safe.IndexOf("&quot;", i, StringComparison.OrdinalIgnoreCase)) &&
                    (i != xml_safe.IndexOf("&gt;", i, StringComparison.OrdinalIgnoreCase)) && 
                    (i != xml_safe.IndexOf("&lt;", i, StringComparison.OrdinalIgnoreCase)))
                {
                    xml_safe = xml_safe.Substring(0, i + 1) + "amp;" + xml_safe.Substring(i + 1);
                }

                i = xml_safe.IndexOf("&", i + 1, StringComparison.OrdinalIgnoreCase);
            }
            return xml_safe.Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
        }

        #endregion
    }
}