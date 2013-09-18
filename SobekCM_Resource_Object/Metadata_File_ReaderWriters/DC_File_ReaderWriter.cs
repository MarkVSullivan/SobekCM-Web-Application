#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using SobekCM.Resource_Object.METS_Sec_ReaderWriters;

#endregion

namespace SobekCM.Resource_Object.Metadata_File_ReaderWriters
{
    public class DC_File_ReaderWriter : iMetadata_File_ReaderWriter
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

        /// <summary> Full name which best describes the metadata format this reader/writer utilizes (i.e. Metadata Object Description Standard, Greenstone file, etc.. ) </summary>
        /// <value>This property always returns 'Dublin Core'</value>
        public string Metadata_Type_Name
        {
            get { return "Dublin Core"; }
        }

        /// <summary> Abbreviation for the metadata format utilized by this reader/writer (i.e., MODS, GSA, etc.. ) </summary>
        /// <value> This property always returns 'Dublin Core'</value>
        public string Metadata_Type_Abbreviation
        {
            get { return "Dublin Core"; }
        }

        /// <summary> Reads metadata from an existing metadata file and saves to the provided item/package </summary>
        /// <param name="MetadataFilePathName"> Path and name of the metadata file to read </param>
        /// <param name="Return_Package"> Package into which to read the metadata </param>
        /// <param name="Options"> Dictionary of any options which this metadata reader/writer may utilize </param>
        /// <param name="Error_Message">[OUTPUT] Explanation of the error, if an error occurs during reading </param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        public bool Read_Metadata(string MetadataFilePathName, SobekCM_Item Return_Package, Dictionary<string, object> Options, out string Error_Message)
        {
            Stream reader = new FileStream(MetadataFilePathName, FileMode.Open, FileAccess.Read);
            bool returnValue = Read_Metadata(reader, Return_Package, Options, out Error_Message);
            reader.Close();

            return returnValue;
        }

        /// <summary> Reads metadata from an open stream and saves to the provided item/package </summary>
        /// <param name="Input_Stream"> Open stream to read metadata from </param>
        /// <param name="Return_Package"> Package into which to read the metadata </param>
        /// <param name="Options"> Dictionary of any options which this metadata reader/writer may utilize </param>
        /// <param name="Error_Message">[OUTPUT] Explanation of the error, if an error occurs during reading </param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        public bool Read_Metadata(Stream Input_Stream, SobekCM_Item Return_Package, Dictionary<string, object> Options, out string Error_Message)
        {
            // Set default error outpt message
            Error_Message = String.Empty;

            // Create a XML reader and read the metadata
            XmlTextReader nodeReader = null;
            bool returnValue = true;
            try
            {
                // create the node reader
                nodeReader = new XmlTextReader(Input_Stream);

                DC_METS_dmdSec_ReaderWriter.Read_Simple_Dublin_Core_Info(nodeReader, Return_Package.Bib_Info, Return_Package);
            }
            catch (Exception ee)
            {
                Error_Message = "Error reading DC from stream: " + ee.Message;
                returnValue = false;
            }
            finally
            {
                if (nodeReader != null)
                    nodeReader.Close();
            }

            return returnValue;
        }

        /// <summary> Writes the formatted metadata from the provided item to a file </summary>
        /// <param name="MetadataFilePathName"> Path and name of the metadata file to write</param>
        /// <param name="Item_To_Save"> Package with all the metadata to save </param>
        /// <param name="Options"> Dictionary of any options which this metadata reader/writer may utilize </param>
        /// <param name="Error_Message">[OUTPUT] Explanation of the error, if an error occurs during write </param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        /// <remarks>This writer accepts one option value.  'DC_File_ReaderWriter:RDF_Style' is a boolean value which indicates
        /// if the Dublin Core metadata file should be written in RDF format.  (Default is FALSE).</remarks>
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
                Error_Message = "Error writing dublin core metadata to file '" + MetadataFilePathName + ": " + ee.Message;
                return false;
            }
        }

        /// <summary> Writes the formatted metadata from the provided item to a TextWriter (usually to an output stream) </summary>
        /// <param name="Output_Stream"></param>
        /// <param name="Item_To_Save"> Package with all the metadata to save </param>
        /// <param name="Options"> Dictionary of any options which this metadata reader/writer may utilize </param>
        /// <param name="Error_Message">[OUTPUT] Explanation of the error, if an error occurs during write </param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        /// <remarks>This writer accepts one option value.  'DC_File_ReaderWriter:RDF_Style' is a boolean value which indicates
        /// if the Dublin Core metadata file should be written in RDF format.  (Default is FALSE).</remarks>
        public bool Write_Metadata(TextWriter Output_Stream, SobekCM_Item Item_To_Save, Dictionary<string, object> Options, out string Error_Message)
        {
            // Set default error outpt message
            Error_Message = String.Empty;

            // Check for RDF style in the options
            bool RDF_Style = false;
            if (Options != null)
            {
                if (Options.ContainsKey("DC_File_ReaderWriter:RDF_Style"))
                {
                    bool.TryParse(Options["DC_File_ReaderWriter:RDF_Style"].ToString(), out RDF_Style);
                }
            }

            if (RDF_Style)
            {
                try
                {
                    // Start to build the XML result
                    Output_Stream.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
                    Output_Stream.WriteLine("<!DOCTYPE rdf:RDF SYSTEM \"http://purl.org/dc/schemas/dcmes-xml-20000714.dtd\">");
                    Output_Stream.WriteLine("<rdf:RDF xmlns:rdf=\"http://www.w3.org/1999/02/22-rdf-syntax-ns#\"");
                    Output_Stream.WriteLine("         xmlns:dc=\"http://purl.org/dc/elements/1.1/\">");
                    Output_Stream.WriteLine("<rdf:Description>");

                    DC_METS_dmdSec_ReaderWriter.Write_Simple_Dublin_Core(Output_Stream, Item_To_Save.Bib_Info);

                    Output_Stream.WriteLine("</rdf:Description>");
                    Output_Stream.WriteLine("</rdf:RDF>");

                    return true;
                }
                catch (Exception ee)
                {
                    Error_Message = "Error caught while saving RDF-style Dublin Core to stream: " + ee.Message;
                    return false;
                }
            }
            else
            {
                try
                {
                    // Start to build the XML result
                    StringBuilder results = new StringBuilder();
                    Output_Stream.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n");
                    Output_Stream.WriteLine("<records>\r\n");
                    Output_Stream.WriteLine("<record xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:dc=\"http://purl.org/dc/elements/1.1/\" xmlns:dcterms=\"http://purl.org/dc/terms/\">\r\n");

                    DC_METS_dmdSec_ReaderWriter.Write_Simple_Dublin_Core(Output_Stream, Item_To_Save.Bib_Info);

                    Output_Stream.WriteLine("</record>\r\n");
                    Output_Stream.WriteLine("</records>\r\n");

                    return true;
                }
                catch (Exception ee)
                {
                    Error_Message = "Error caught while saving Dublin Core to stream: " + ee.Message;
                    return false;
                }
            }
        }

        #endregion
    }
}