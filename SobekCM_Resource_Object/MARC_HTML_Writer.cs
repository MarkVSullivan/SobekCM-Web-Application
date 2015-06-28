#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using SobekCM.Resource_Object.MARC;

#endregion

namespace SobekCM.Resource_Object
{
    /// <summary> Class is used to write the information about a digital resource in MARC21 format </summary>
    public class MARC_HTML_Writer : XML_Writing_Base_Type
    {
        /// <summary> Constructor for a new instance of the MARC_Writer class </summary>
        public MARC_HTML_Writer()
        {
            // Do nothing
        }


        /// <summary> Writes a digital resource in MARC format, with additional considerations for displaying within a HTML page </summary>
        /// <param name="Package"> Digital resource to write in MARC format </param>
        /// <param name="Options"> Dictionary of any options which this metadata reader/writer may utilize </param>
        /// <returns> MARC record formatted for HTML, returned as a string </returns>
        public string MARC_HTML(SobekCM_Item Package, Dictionary<string, object> Options)
        {
            return MARC_HTML(Package, "95%", Options);
        }

        /// <summary> Writes a digital resource in MARC format, with additional considerations for displaying within a HTML page </summary>
        /// <param name="Package"> Digital resource to write in MARC format </param>
        /// <param name="Width"> Width of the resulting HTML-formatted MARC record </param>
        /// <param name="Options"> Dictionary of any options which this metadata reader/writer may utilize </param>
        /// <returns> MARC record formatted for HTML, returned as a string </returns>
        public string MARC_HTML(SobekCM_Item Package, string Width, Dictionary<string, object> Options)
        {
            // Try to pull some values from the options
            string cataloging_source_code = String.Empty;
            if ((Options.ContainsKey("MarcXML_File_ReaderWriter:MARC Cataloging Source Code")) && (Options["MarcXML_File_ReaderWriter:MARC Cataloging Source Code"] != null ))
                cataloging_source_code = Options["MarcXML_File_ReaderWriter:MARC Cataloging Source Code"].ToString();

            string location_code = String.Empty;
            if ((Options.ContainsKey("MarcXML_File_ReaderWriter:MARC Location Code")) && (Options["MarcXML_File_ReaderWriter:MARC Location Code"] != null))
                location_code = Options["MarcXML_File_ReaderWriter:MARC Location Code"].ToString();

            string reproduction_agency = String.Empty;
            if ((Options.ContainsKey("MarcXML_File_ReaderWriter:MARC Reproduction Agency")) && (Options["MarcXML_File_ReaderWriter:MARC Reproduction Agency"] != null))
                reproduction_agency = Options["MarcXML_File_ReaderWriter:MARC Reproduction Agency"].ToString();

            string reproduction_place = String.Empty;
            if ((Options.ContainsKey("MarcXML_File_ReaderWriter:MARC Reproduction Place")) && (Options["MarcXML_File_ReaderWriter:MARC Reproduction Place"] != null))
                reproduction_place = Options["MarcXML_File_ReaderWriter:MARC Reproduction Place"].ToString();

            string system_name = String.Empty;
            if ((Options.ContainsKey("MarcXML_File_ReaderWriter:System Name")) && (Options["MarcXML_File_ReaderWriter:System Name"] != null))
                system_name = Options["MarcXML_File_ReaderWriter:System Name"].ToString();

            string system_abbreviation = String.Empty;
            if ((Options.ContainsKey("MarcXML_File_ReaderWriter:System Abbreviation")) && (Options["MarcXML_File_ReaderWriter:System Abbreviation"] != null))
                system_abbreviation = Options["MarcXML_File_ReaderWriter:System Abbreviation"].ToString();

            string thumbnail_base = String.Empty;
            if ((Options.ContainsKey("MarcXML_File_ReaderWriter:Image_Base")) && (Options["MarcXML_File_ReaderWriter:Image_Base"] != null))
                thumbnail_base = Options["MarcXML_File_ReaderWriter:Image_Base"].ToString();
            
            // Get all the standard tags
            MARC_Record tags = Package.To_MARC_Record(cataloging_source_code, location_code, reproduction_agency, reproduction_place, system_name, system_abbreviation, thumbnail_base);

            // Look for extra tags to add in the OPTIONS
            if (Options.ContainsKey("MarcXML_File_ReaderWriter:Additional_Tags"))
            {
                object add_tags_obj = Options["MarcXML_File_ReaderWriter:Additional_Tags"];
                if (add_tags_obj != null)
                {
                    try
                    {
                        List<MARC_Field> add_tags = (List<MARC_Field>)add_tags_obj;
                        foreach (MARC_Field thisTag in add_tags)
                        {
                            tags.Add_Field(thisTag);
                        }
                    }
                    catch
                    {
                        // Do nothing in this case
                    }
                }
            }

            // Start to build the HTML result
            StringBuilder results = new StringBuilder();
            results.Append("<table style=\"border:none; text-align:left; width:" + Width + ";\">\n");

            // Add the LEADER
            results.Append("  <tr class=\"trGenContent\">\n");
            results.Append("    <td style=\"width:33px;vertical-align:top;\">LDR</td>\n");
            results.Append("    <td style=\"width:26px;vertical-align:top;\">&nbsp;</td>\n");
            results.Append("    <td>" + tags.Leader.Replace(" ", "^") + "</td>\n");
            results.Append("  </tr>");


            // Add all the FIELDS
            foreach (MARC_Field thisTag in tags.Sorted_MARC_Tag_List)
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

        /// <summary> Writes a digital resource in MARC format, with additional considerations for displaying within a HTML page </summary>
        /// <param name="Package"> Digital resource to write in MARC format </param>
        /// <returns> MARC record formatted for HTML, returned as a string </returns>
        public string MARC_HTML(SobekCM_Item Package)
        {
            return MARC_HTML(Package, "95%", null);
        }

        /// <summary> Writes a digital resource in MARC format, with additional considerations for displaying within a HTML page </summary>
        /// <param name="Package"> Digital resource to write in MARC format </param>
        /// <param name="Width"> Width of the resulting HTML-formatted MARC record </param>
        /// <returns> MARC record formatted for HTML, returned as a string </returns>
        public string MARC_HTML(SobekCM_Item Package, string Width)
        {
            return MARC_HTML(Package, Width, null);
        }
    }
}