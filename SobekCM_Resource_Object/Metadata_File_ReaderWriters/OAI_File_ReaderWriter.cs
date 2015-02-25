#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SobekCM.Resource_Object.METS_Sec_ReaderWriters;

#endregion

namespace SobekCM.Resource_Object.Metadata_File_ReaderWriters
{
    public class OAI_File_ReaderWriter : iMetadata_File_ReaderWriter
    {
        #region iMetadata_File_ReaderWriter Members

        /// <summary> Flag indicates if this reader/writer can read from files and streams </summary>
        /// <value> This property always returns FALSE </value>
        public bool canRead
        {
            get { return false; }
        }

        /// <summary> Flag indicates if this reader/writer can write to files and streams </summary>
        /// <value> This property always return TRUE </value>
        public bool canWrite
        {
            get { return true; }
        }

        /// <summary> Full name which best describes the metadata format this reader/writer utilizes (i.e. Dublin Core, Greenstone file, etc.. ) </summary>
        /// <value>This property always returns 'Open-Archives Initiative : Protocol for Metadata Harvesting'</value>
        public string Metadata_Type_Name
        {
            get { return "Open-Archives Initiative : Protocol for Metadata Harvesting"; }
        }

        /// <summary> Abbreviation for the metadata format utilized by this reader/writer (i.e., DC, MODS, GSA, etc.. ) </summary>
        /// <value> This property always returns 'OAI-PMH'</value>
        public string Metadata_Type_Abbreviation
        {
            get { return "OAI-PMH"; }
        }

        /// <summary> Reads metadata from an existing metadata file and saves to the provided item/package </summary>
        /// <param name="MetadataFilePathName"> Path and name of the metadata file to read </param>
        /// <param name="Return_Package"> Package into which to read the metadata </param>
        /// <param name="Options"> Dictionary of any options which this metadata reader/writer may utilize </param>
        /// <param name="Error_Message">[OUTPUT] Explanation of the error, if an error occurs during reading </param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        public bool Read_Metadata(string MetadataFilePathName, SobekCM_Item Return_Package, Dictionary<string, object> Options, out string Error_Message)
        {
            throw new NotImplementedException();
        }

        /// <summary> Reads metadata from an open stream and saves to the provided item/package </summary>
        /// <param name="Input_Stream"> Open stream to read metadata from </param>
        /// <param name="Return_Package"> Package into which to read the metadata </param>
        /// <param name="Options"> Dictionary of any options which this metadata reader/writer may utilize </param>
        /// <param name="Error_Message">[OUTPUT] Explanation of the error, if an error occurs during reading </param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        public bool Read_Metadata(Stream Input_Stream, SobekCM_Item Return_Package, Dictionary<string, object> Options, out string Error_Message)
        {
            throw new NotImplementedException();
        }

        /// <summary> Writes the formatted metadata from the provided item to a file </summary>
        /// <param name="MetadataFilePathName"> Path and name of the metadata file to write</param>
        /// <param name="Item_To_Save"> Package with all the metadata to save </param>
        /// <param name="Options"> Dictionary of any options which this metadata reader/writer may utilize </param>
        /// <param name="Error_Message">[OUTPUT] Explanation of the error, if an error occurs during write </param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        public bool Write_Metadata(string MetadataFilePathName, SobekCM_Item Item_To_Save, Dictionary<string, object> Options, out string Error_Message)
        {
            try
            {
                StreamWriter results = new StreamWriter(MetadataFilePathName, false, Encoding.UTF8);
                bool returnValue = Write_Metadata(results, Item_To_Save, Options, out Error_Message);
                results.Flush();
                results.Close();

                return returnValue;
            }
            catch (Exception ee)
            {
                Error_Message = "Error writing OAI-PMH metadata to file '" + MetadataFilePathName + ": " + ee.Message;
                return false;
            }
        }

        /// <summary> Writes the formatted metadata from the provided item to a TextWriter (usually to an output stream) </summary>
        /// <param name="Output_Stream"></param>
        /// <param name="Item_To_Save"> Package with all the metadata to save </param>
        /// <param name="Options"> Dictionary of any options which this metadata reader/writer may utilize </param>
        /// <param name="Error_Message">[OUTPUT] Explanation of the error, if an error occurs during write </param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        public bool Write_Metadata(TextWriter Output_Stream, SobekCM_Item Item_To_Save, Dictionary<string, object> Options, out string Error_Message)
        {
            // Set default error outpt message
            Error_Message = String.Empty;

            // Check for OAI set and OAI data in the options
            string OAI_Set = String.Empty;
            DateTime OAI_Date = DateTime.Now;
            if (Options.ContainsKey("OAI_File_ReaderWriter:OAI_Set"))
            {
                OAI_Set = Options["OAI_File_ReaderWriter:OAI_Set"].ToString();
            }
            if (Options.ContainsKey("OAI_File_ReaderWriter:OAI_Date"))
            {
                DateTime.TryParse(Options["OAI_File_ReaderWriter:OAI_Date"].ToString(), out OAI_Date);
            }

            StringBuilder returnValue = new StringBuilder();

            // Add the header for this OAI
            Output_Stream.WriteLine("<xml>");

            Output_Stream.WriteLine("<record><header><identifier>oai:www.uflib.ufl.edu.ufdc:" + Item_To_Save.BibID + "</identifier>");
            Output_Stream.WriteLine("<datestamp>" + OAI_Date.Year + "-" + OAI_Date.Month.ToString().PadLeft(2, '0') + "-" + OAI_Date.Day.ToString().PadLeft(2, '0') + "</datestamp>");
            Output_Stream.WriteLine("<setSpec>" + OAI_Set + "</setSpec></header>");

            // Start the metadata section with the namespace references
            Output_Stream.WriteLine("<metadata>");
            Output_Stream.WriteLine("<oai_dc:dc xmlns:oai_dc=\"http://www.openarchives.org/OAI/2.0/oai_dc/\" ");
            Output_Stream.WriteLine("xmlns:dc=\"http://purl.org/dc/elements/1.1/\" ");
            Output_Stream.WriteLine("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" ");
            Output_Stream.WriteLine("xsi:schemaLocation=\"http://www.openarchives.org/OAI/2.0/oai_dc/ ");
            Output_Stream.WriteLine("http://www.openarchives.org/OAI/2.0/oai_dc.xsd\">");

            // Add the dublin core
            DC_METS_dmdSec_ReaderWriter.Write_Simple_Dublin_Core(Output_Stream, Item_To_Save.Bib_Info);

            // Add the URL as the identifier
            if (Item_To_Save.Bib_Info.Location.PURL.Length > 0)
            {
                Output_Stream.WriteLine("<dc:identifier>" + Item_To_Save.Bib_Info.Location.PURL + "</dc:identifier>");
            }
            else if (Item_To_Save.Web.Service_URL.Length > 0)
            {
                Output_Stream.WriteLine("<dc:identifier>" + Item_To_Save.Web.Service_URL + "</dc:identifier>");
            }

            // Finish this OAI
            Output_Stream.WriteLine("</oai_dc:dc>");
            Output_Stream.WriteLine("</metadata>");
            Output_Stream.WriteLine("</record>");

            Output_Stream.WriteLine("</xml>");

            // Return the built OAI string
            return true;
        }

        #endregion
    }
}