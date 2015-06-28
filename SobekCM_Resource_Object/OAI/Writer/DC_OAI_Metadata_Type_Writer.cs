#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SobekCM.Resource_Object.METS_Sec_ReaderWriters;

#endregion

namespace SobekCM.Resource_Object.OAI.Writer
{
    /// <summary> Returns the OAI-PMH formatted dublin core record for a single digital resource / item </summary>
    public class DC_OAI_Metadata_Type_Writer : abstract_OAI_PMH_Metadata_Type_Writer
    {
        /// <summary> Returns the OAI-PMH metadata in dublin core (OAI-flavored) for this item </summary>
        /// <param name="Item_To_Save"> Package with all the metadata to save </param>
        /// <param name="Options"> Dictionary of any options which this metadata reader/writer may utilize </param>
        /// <param name="Error_Message">[OUTPUT] Explanation of the error, if an error occurs during write </param>
        /// <returns> Metadata for a OAI-PMH record of a particular metadata format/type </returns>
        public override string Create_OAI_PMH_Metadata( SobekCM_Item Item_To_Save, Dictionary<string, object> Options, out string Error_Message)
        {
            // Set default error outpt message
            Error_Message = String.Empty;

            StringBuilder results = new StringBuilder();
            StringWriter writer = new StringWriter(results);

            writer.WriteLine("<oai_dc:dc xmlns:oai_dc=\"http://www.openarchives.org/OAI/2.0/oai_dc/\" ");
            writer.WriteLine("xmlns:dc=\"http://purl.org/dc/elements/1.1/\" ");
            writer.WriteLine("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" ");
            writer.WriteLine("xsi:schemaLocation=\"http://www.openarchives.org/OAI/2.0/oai_dc/ ");
            writer.WriteLine("http://www.openarchives.org/OAI/2.0/oai_dc.xsd\">");

            // Add the dublin core
            DC_METS_dmdSec_ReaderWriter.Write_Simple_Dublin_Core(writer, Item_To_Save.Bib_Info);

            // Add the URL as the identifier
            if (Item_To_Save.Bib_Info.Location.PURL.Length > 0)
            {
                writer.WriteLine("<dc:identifier>" + Item_To_Save.Bib_Info.Location.PURL + "</dc:identifier>");
            }
            else if (Item_To_Save.Web.Service_URL.Length > 0)
            {
                writer.WriteLine("<dc:identifier>" + Item_To_Save.Web.Service_URL + "</dc:identifier>");
            }

            // Finish this OAI
            writer.WriteLine("</oai_dc:dc>");

            string resultsString = results.ToString();
            writer.Close();

            return resultsString;
        }
    }
}
