#region Using directives

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
                    results.Append(base.Convert_String_To_XML_Safe(thisTag.Control_Field_Value.Replace(" ", "^")));
                }
                else
                {
                    results.Append(base.Convert_String_To_XML_Safe(thisTag.Control_Field_Value).Replace("|a", "<span style=\"color:blue;\">|a</span>").
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
        /// <param name="package"> Digital resource to write in MARC format </param>
        /// <returns> MARC record formatted for HTML, returned as a string </returns>
        public string MARC_HTML(SobekCM_Item package)
        {
            return MARC_HTML(package, "95%", null);
        }

        /// <summary> Writes a digital resource in MARC format, with additional considerations for displaying within a HTML page </summary>
        /// <param name="package"> Digital resource to write in MARC format </param>
        /// <param name="Width"> Width of the resulting HTML-formatted MARC record </param>
        /// <returns> MARC record formatted for HTML, returned as a string </returns>
        public string MARC_HTML(SobekCM_Item package, string Width)
        {
            return MARC_HTML(package, Width, null);
        }
    }
}