#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using SobekCM.Resource_Object.MARC;

#endregion

namespace SobekCM.Resource_Object.OAI.Writer
{
    /// <summary> Returns the OAI-PMH formatted MarcXML record for a single digital resource / item </summary>
    public class MarcXML_OAI_PMH_Metadata_Type_Writer : abstract_OAI_PMH_Metadata_Type_Writer
    {
        /// <summary> Returns the OAI-PMH metadata in MarcXML (OAI-flavored) for this item </summary>
        /// <param name="Item_To_Save"> Package with all the metadata to save </param>
        /// <param name="Options"> Dictionary of any options which this metadata reader/writer may utilize </param>
        /// <param name="Error_Message">[OUTPUT] Explanation of the error, if an error occurs during write </param>
        /// <returns> Metadata for a OAI-PMH record of a particular metadata format/type </returns>
        public override string Create_OAI_PMH_Metadata(SobekCM_Item Item_To_Save, Dictionary<string, object> Options, out string Error_Message)
        {
            // Look for options
            string CatalogingSourceCode = null;
            string LocationCode = null;
            string ReproductionAgency = null;
            string ReproductionPlace = null;
            string SystemName = null;
            string SystemAbbreviation = null;
            string ThumbnailBase = null;
            string XlstFile = null;


            if (Options != null)
            {
                if (Options.ContainsKey("MarcXML_File_ReaderWriter:MARC Cataloging Source Code")) CatalogingSourceCode = Options["MarcXML_File_ReaderWriter:MARC Cataloging Source Code"].ToString();
                if (Options.ContainsKey("MarcXML_File_ReaderWriter:MARC Location Code")) LocationCode = Options["MarcXML_File_ReaderWriter:MARC Location Code"].ToString();
                if (Options.ContainsKey("MarcXML_File_ReaderWriter:MARC Reproduction Agency")) ReproductionAgency = Options["MarcXML_File_ReaderWriter:MARC Reproduction Agency"].ToString();
                if (Options.ContainsKey("MarcXML_File_ReaderWriter:MARC Reproduction Place")) ReproductionPlace = Options["MarcXML_File_ReaderWriter:MARC Reproduction Place"].ToString();
                if (Options.ContainsKey("MarcXML_File_ReaderWriter:MARC XSLT File")) XlstFile = Options["MarcXML_File_ReaderWriter:MARC XSLT File"].ToString();
                if (Options.ContainsKey("MarcXML_File_ReaderWriter:System Name")) SystemName = Options["MarcXML_File_ReaderWriter:System Name"].ToString();
                if (Options.ContainsKey("MarcXML_File_ReaderWriter:System Abbreviation")) SystemAbbreviation = Options["MarcXML_File_ReaderWriter:System Abbreviation"].ToString();
                if (Options.ContainsKey("MarcXML_File_ReaderWriter:Image_Base")) ThumbnailBase = Options["MarcXML_File_ReaderWriter:Image_Base"].ToString();
            }




            // Set default error outpt message
            Error_Message = String.Empty;

            StringBuilder results = new StringBuilder();

            results.AppendLine("<marc:record xmlns:marc=\"http://www.loc.gov/MARC21/slim\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.loc.gov/MARC21/slim http://www.loc.gov/standards/marcxml/schema/MARC21slim.xsd\" type=\"Bibliographic\">");

            // Get all the standard tags
            MARC_Record tags = Item_To_Save.To_MARC_Record(CatalogingSourceCode, LocationCode, ReproductionAgency, ReproductionPlace, SystemName, SystemAbbreviation, ThumbnailBase);

            // St

            // Add the leader
            results.Append("<marc:leader>" + tags.Leader + "</marc:leader>\r\n");

            foreach (MARC_Field thisTag in tags.Sorted_MARC_Tag_List)
            {
                if ((thisTag.Tag == 1) || (thisTag.Tag == 3) || (thisTag.Tag == 5) || (thisTag.Tag == 6) || (thisTag.Tag == 7) || (thisTag.Tag == 8))
                {
                    results.Append("<marc:controlfield tag=\"" + thisTag.Tag.ToString().PadLeft(3, '0') + "\">" + Convert_String_To_XML_Safe_Static(thisTag.Control_Field_Value) + "</marc:controlfield>\r\n");
                }
                else
                {
                    results.Append("<marc:datafield tag=\"" + thisTag.Tag.ToString().PadLeft(3, '0') + "\" ind1=\"" + thisTag.Indicators[0] + "\" ind2=\"" + thisTag.Indicators[1] + "\">\r\n");

                    string[] splitter = thisTag.Control_Field_Value.Split("|".ToCharArray());
                    foreach (string subfield in splitter)
                    {
                        if ((subfield.Length > 2) && ((Char.IsLetter(subfield[0])) || (Char.IsNumber(subfield[0]))))
                        {
                            results.Append("<marc:subfield code=\"" + subfield[0] + "\">" + Convert_String_To_XML_Safe_Static(subfield.Substring(2).Trim()) + "</marc:subfield>\r\n");
                        }
                    }

                    results.Append("</marc:datafield>\r\n");
                }
            }

            // Finish this OAI
            results.AppendLine("</marc:record>");

            return results.ToString();
        }

        /// <summary> Converts a basic string into an XML-safe string </summary>
        /// <param name="element"> Element data to convert </param>
        /// <returns> Data converted into an XML-safe string</returns>
        public static string Convert_String_To_XML_Safe_Static(string element)
        {
            if (element == null)
                return string.Empty;

            string xml_safe = element;
            int i = xml_safe.IndexOf("&");
            while (i >= 0)
            {
                if ((i != xml_safe.IndexOf("&amp;", i)) && (i != xml_safe.IndexOf("&quot;", i)) &&
                    (i != xml_safe.IndexOf("&gt;", i)) && (i != xml_safe.IndexOf("&lt;", i)))
                {
                    xml_safe = xml_safe.Substring(0, i + 1) + "amp;" + xml_safe.Substring(i + 1);
                }

                i = xml_safe.IndexOf("&", i + 1);
            }
            return xml_safe.Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
        }
    }
}
