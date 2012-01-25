using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SobekCM.Bib_Package.MARC;

namespace SobekCM.Bib_Package.Writers.SubWriters
{
    /// <summary> Subwriter writes MARC21 slim XML for a given digital resource </summary>
    public class MarcXML_SubWriter
    {
        /// <summary> Add the bibliographic information as MARC21 slim XML to the output stream for a given digital resource </summary>
        /// <param name="Output"> Output stream for this metadata to be written to </param>
        /// <param name="thisBib"> Source digital resource </param>
        /// <param name="Include_Schema"> Flag indicates whether the schema information should be included in the record tag </param>
        public static void Add_MarcXML(System.IO.TextWriter Output, SobekCM_Item thisBib, bool Include_Schema )
        {
            // Get all the standard tags
            MARC_Record tags = thisBib.To_MARC_Record();

            // Start to build the XML result
            if (Include_Schema)
            {
                Output.Write("<record xmlns=\"http://www.loc.gov/MARC21/slim\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.loc.gov/MARC21/slim http://www.loc.gov/standards/marcxml/schema/MARC21slim.xsd\">\r\n");
            }
            else
            {
                Output.Write("<record>\r\n");
            }

            // Add the leader
            Output.Write("<leader>" + tags.Leader + "</leader>\r\n");

            foreach (MARC_Field thisTag in tags.Sorted_MARC_Tag_List)
            {
                if ((thisTag.Tag == 1) || (thisTag.Tag == 3) || (thisTag.Tag == 5) || (thisTag.Tag == 6) || (thisTag.Tag == 7) || (thisTag.Tag == 8))
                {
                    Output.Write("<controlfield tag=\"" + thisTag.Tag.ToString().PadLeft(3,'0') + "\">" + XML_Writing_Base_Type.Convert_String_To_XML_Safe_Static(thisTag.Control_Field_Value) + "</controlfield>\r\n");
                }
                else
                {
                    Output.Write("<datafield tag=\"" + thisTag.Tag.ToString().PadLeft(3, '0') + "\" ind1=\"" + thisTag.Indicators[0] + "\" ind2=\"" + thisTag.Indicators[1] + "\">\r\n");

                    string[] splitter = thisTag.Control_Field_Value.Split("|".ToCharArray());
                    foreach (string subfield in splitter)
                    {
                        if ((subfield.Length > 2) && (( Char.IsLetter( subfield[0] )) || ( Char.IsNumber( subfield[0] ))))
                        {
                            Output.Write("<subfield code=\"" + subfield[0] + "\">" + XML_Writing_Base_Type.Convert_String_To_XML_Safe_Static(subfield.Substring(2).Trim()) + "</subfield>\r\n");
                        }
                    }

                    Output.Write("</datafield>\r\n");
                }
            }

            Output.Write("</record>\r\n");
        }
    }
}
