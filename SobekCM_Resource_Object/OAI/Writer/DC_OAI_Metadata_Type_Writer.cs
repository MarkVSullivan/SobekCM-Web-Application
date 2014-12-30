using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SobekCM.Resource_Object.OAI.Writer
{
    public class DC_OAI_Metadata_Type_Writer : abstract_OAI_PMH_Metadata_Type_Writer
    {
        /// <summary> Writes the formatted metadata from the provided item to a TextWriter (usually to an output stream) </summary>
        /// <param name="Output_Stream">Stream to which the formatted text is written </param>
        /// <param name="Item_To_Save"> Package with all the metadata to save </param>
        /// <param name="Options"> Dictionary of any options which this metadata reader/writer may utilize </param>
        /// <param name="Error_Message">[OUTPUT] Explanation of the error, if an error occurs during write </param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        public override bool Write_Metadata(TextWriter Output_Stream, SobekCM_Item Item_To_Save, Dictionary<string, object> Options, out string Error_Message)
        {
            // Set default error outpt message
            Error_Message = String.Empty;

            //// Check for OAI set and OAI data in the options
            //string OAI_Set = String.Empty;
            //DateTime OAI_Date = DateTime.Now;
            //if (Options.ContainsKey("OAI_File_ReaderWriter:OAI_Set"))
            //{
            //    OAI_Set = Options["OAI_File_ReaderWriter:OAI_Set"].ToString();
            //}
            //if (Options.ContainsKey("OAI_File_ReaderWriter:OAI_Date"))
            //{
            //    DateTime.TryParse(Options["OAI_File_ReaderWriter:OAI_Date"].ToString(), out OAI_Date);
            //}

            //StringBuilder returnValue = new StringBuilder();

            //// Add the header for this OAI
            //Output_Stream.WriteLine("<xml>");

            //Output_Stream.WriteLine("<record><header><identifier>oai:www.uflib.ufl.edu.ufdc:" + Item_To_Save.BibID + "</identifier>");
            //Output_Stream.WriteLine("<datestamp>" + OAI_Date.Year + "-" + OAI_Date.Month.ToString().PadLeft(2, '0') + "-" + OAI_Date.Day.ToString().PadLeft(2, '0') + "</datestamp>");
            //Output_Stream.WriteLine("<setSpec>" + OAI_Set + "</setSpec></header>");

            //// Start the metadata section with the namespace references
            //Output_Stream.WriteLine("<metadata>");
            //Output_Stream.WriteLine("<oai_dc:dc xmlns:oai_dc=\"http://www.openarchives.org/OAI/2.0/oai_dc/\" ");
            //Output_Stream.WriteLine("xmlns:dc=\"http://purl.org/dc/elements/1.1/\" ");
            //Output_Stream.WriteLine("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" ");
            //Output_Stream.WriteLine("xsi:schemaLocation=\"http://www.openarchives.org/OAI/2.0/oai_dc/ ");
            //Output_Stream.WriteLine("http://www.openarchives.org/OAI/2.0/oai_dc.xsd\">");

            //// Add the dublin core
            //SobekCM.Resource_Object.METS_Sec_ReaderWriters.DC_METS_dmdSec_ReaderWriter.Write_Simple_Dublin_Core(Output_Stream, Item_To_Save.Bib_Info);

            //// Add the URL as the identifier
            //if (Item_To_Save.Bib_Info.Location.PURL.Length > 0)
            //{
            //    Output_Stream.WriteLine("<dc:identifer>" + Item_To_Save.Bib_Info.Location.PURL + "</dc:identifier>");
            //}
            //else if (Item_To_Save.Web.Service_URL.Length > 0)
            //{
            //    Output_Stream.WriteLine("<dc:identifer>" + Item_To_Save.Web.Service_URL + "</dc:identifier>");
            //}

            //// Finish this OAI
            //Output_Stream.WriteLine("</oai_dc:dc>");
            //Output_Stream.WriteLine("</metadata>");
            //Output_Stream.WriteLine("</record>");

            //Output_Stream.WriteLine("</xml>");

            //// Return the built OAI string
            return true;
        }
    }
}
