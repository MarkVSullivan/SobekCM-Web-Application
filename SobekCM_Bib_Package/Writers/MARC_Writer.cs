using System;
using System.Collections.Generic;
using System.Text;
using SobekCM.Bib_Package;
using SobekCM.Bib_Package.Bib_Info;
using SobekCM.Bib_Package.MARC;

namespace SobekCM.Bib_Package.Writers
{
    /// <summary> Class is used to write the information about a digital resource in MARC21 format </summary>
    public class MARC_Writer : XML_Writing_Base_Type
    {
        /// <summary> Constructor for a new instance of the MARC_Writer class </summary>
        public MARC_Writer()
        {
            // Do nothing
        }
        
        /// <summary> Write the digital resource information as MARC XML to a file </summary>
        /// <param name="fileName"> Complete name for the file </param>
        /// <param name="package"> Digital resource object </param>
        /// <param name="add_tags"> Any additional MARC tags which should be included besides the standard MARC tags </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public bool Save_As_MARC_XML(string fileName, SobekCM_Item package, List<MARC_Field> add_tags)
        {
            try
            {
                // Start to build the XML result
                StringBuilder results = new StringBuilder();
                results.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n");
                results.Append("<collection xmlns=\"http://www.loc.gov/MARC21/slim\">\r\n");

                results.Append(MARC_XML_Slim(package, add_tags));

                results.Append("</collection>\r\n");

                System.IO.StreamWriter writer = new System.IO.StreamWriter(fileName, false);
                writer.Write(results.ToString());
                writer.Flush();
                writer.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary> Writes a digital resource in MARC XML slim format </summary>
        /// <param name="package"> Digital resource to write in MARC format </param>
        /// <param name="add_tags"> Any additional MARC tags which should be included besides the standard MARC tags </param>
        /// <returns> MARC record formatted as MARC XML Slim, returned as a string </returns>
        public string MARC_XML_Slim(SobekCM_Item package, List<MARC_Field> add_tags)
        {
            // Get all the standard tags
            MARC_Record tags = package.To_MARC_Record();

            // Add any additional tags included here
            if (add_tags != null)
            {
                foreach (MARC_Field thisTag in add_tags)
                {
                    tags.Add_Field(thisTag);
                }
            }

            // Start to build the XML result
            StringBuilder results = new StringBuilder();
            results.Append("  <record>\r\n");

            // Add the leader
            results.Append("    <leader>" + tags.Leader + "</leader>\r\n");

            foreach (MARC_Field thisTag in tags.Sorted_MARC_Tag_List)
            {
                if ((thisTag.Tag >= 1) && (thisTag.Tag <= 8 ))
                {
                    results.Append("    <controlfield tag=\"" + thisTag.Tag.ToString().PadLeft(3, '0') + "\">" + base.Convert_String_To_XML_Safe(thisTag.Control_Field_Value) + "</controlfield>\r\n");
                }
                else
                {
                    results.Append("    <datafield tag=\"" + thisTag.Tag.ToString().PadLeft(3, '0') + "\" ind1=\"" + thisTag.Indicators[0] + "\" ind2=\"" + thisTag.Indicators[1] + "\">\r\n");

                    string[] splitter = thisTag.Control_Field_Value.Split("|".ToCharArray());
                    foreach (string subfield in splitter)
                    {
                        if (subfield.Length > 2)
                        {
                            results.Append("      <subfield code=\"" + subfield[0] + "\">" + base.Convert_String_To_XML_Safe(subfield.Substring(2).Trim()) + "</subfield>\r\n");
                        }
                    }

                    results.Append("    </datafield>\r\n");
                }
            }

            results.Append("  </record>\r\n");
            return results.ToString().Replace("&amp;bar;","|");
        }

        /// <summary> Writes a digital resource in MARC format, with additional considerations for displaying within a HTML page </summary>
        /// <param name="package"> Digital resource to write in MARC format </param>
        /// <param name="add_tags"> Any additional MARC tags which should be included besides the standard MARC tags </param>
        /// <returns> MARC record formatted for HTML, returned as a string </returns>
        public string MARC_HTML(SobekCM_Item package, List<MARC_Field> add_tags)
        {
            return MARC_HTML(package, "95%", add_tags);
        }

        /// <summary> Writes a digital resource in MARC format, with additional considerations for displaying within a HTML page </summary>
        /// <param name="package"> Digital resource to write in MARC format </param>
        /// <param name="Width"> Width of the resulting HTML-formatted MARC record </param>
        /// <param name="add_tags"> Any additional MARC tags which should be included besides the standard MARC tags </param>
        /// <returns> MARC record formatted for HTML, returned as a string </returns>
        public string MARC_HTML(SobekCM_Item package, string Width, List<MARC_Field> add_tags)
        {
            // Get all the standard tags
            MARC_Record tags = package.To_MARC_Record();

            // Add any additional tags included here
            if (add_tags != null)
            {
                foreach (MARC_Field thisTag in add_tags)
                {
                    tags.Add_Field(thisTag);
                }
            }

            // Start to build the HTML result
            StringBuilder results = new StringBuilder();
            results.Append("<table border=\"0\" align=\"center\" width=\"" + Width + "\">\n");

            // Add the LEADER
            results.Append("  <tr class=\"trGenContent\">\n");
            results.Append("    <td width=\"33\" valign=\"top\">LDR</td>\n");
            results.Append("    <td width=\"26\" valign=\"top\">&nbsp;</td>\n");
            results.Append("    <td>" + tags.Leader.Replace(" ","^") + "</td>\n");
            results.Append("  </tr>");


            // Add all the FIELDS
            foreach (MARC_Field thisTag in tags.Sorted_MARC_Tag_List)
            {
                results.Append("  <tr class=\"trGenContent\">\n");
                results.Append("    <td width=\"33\" valign=\"top\">" + thisTag.Tag.ToString().PadLeft(3,'0') + "</td>\n");
                results.Append("    <td width=\"26\" valign=\"top\"><font color=\"green\">" + thisTag.Indicators.Replace(" ", "&nbsp;&nbsp;&nbsp;") + "</font></td>\n");
                results.Append("    <td>");
                if ((thisTag.Tag == 8) || ( thisTag.Tag == 7 ) || ( thisTag.Tag == 6 ))
                {
                    results.Append(base.Convert_String_To_XML_Safe(thisTag.Control_Field_Value.Replace(" ", "^")));
                }
                else
                {
                    results.Append(base.Convert_String_To_XML_Safe(thisTag.Control_Field_Value).Replace("|a", "<font color=\"blue\">|a</font>").
                        Replace("|b", "<font color=\"blue\">|b</font>").
                        Replace("|c", "<font color=\"blue\">|c</font>").
                        Replace("|d", "<font color=\"blue\">|d</font>").
                        Replace("|e", "<font color=\"blue\">|e</font>").
                        Replace("|g", "<font color=\"blue\">|g</font>").
                        Replace("|x", "<font color=\"blue\">|x</font>").
                        Replace("|y", "<font color=\"blue\">|y</font>").
                        Replace("|z", "<font color=\"blue\">|z</font>").
                        Replace("|v", "<font color=\"blue\">|v</font>").
                        Replace("|h", "<font color=\"blue\">|h</font>").
                        Replace("|u", "<font color=\"blue\">|u</font>").
                        Replace("|f", "<font color=\"blue\">|f</font>").
                        Replace("|n", "<font color=\"blue\">|n</font>").
                        Replace("|2", "<font color=\"blue\">|2</font>").
                        Replace("|3", "<font color=\"blue\">|3</font>").
                        Replace("|w", "<font color=\"blue\">|w</font>").
                        Replace("|t", "<font color=\"blue\">|t</font>").
                        Replace("|q", "<font color=\"blue\">|q</font>").
                        Replace("|o", "<font color=\"blue\">|o</font>").
                        Replace("|i", "<font color=\"blue\">|i</font>").
                        Replace("|4", "<font color=\"blue\">|4</font>"));
                }
                results.Append("</td>\n");
                results.Append("  </tr>");
            }
            results.Append("</table>\n");
            return results.ToString().Replace("&amp;bar;", "|");
        }

        /// <summary> Writes a digital resource in MARC format, with additional considerations for displaying within a HTML page </summary>
        /// <param name="package"> Digital resource to write in MARC format </param>
        /// <returns> MARC record formatted for HTML, returned as a string </returns>
        public string MARC_HTML(SobekCM_Item package )
        {
            return MARC_HTML(package, "95%", null);
        }

        /// <summary> Writes a digital resource in MARC format, with additional considerations for displaying within a HTML page </summary>
        /// <param name="package"> Digital resource to write in MARC format </param>
        /// <param name="Width"> Width of the resulting HTML-formatted MARC record </param>
        /// <returns> MARC record formatted for HTML, returned as a string </returns>
        public string MARC_HTML( SobekCM_Item package, string Width )
        {
            return MARC_HTML(package, Width, null);
        }
    }
}
