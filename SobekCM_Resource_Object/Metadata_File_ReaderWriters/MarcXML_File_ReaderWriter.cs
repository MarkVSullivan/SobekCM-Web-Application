#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using SobekCM.Resource_Object.MARC;
using SobekCM.Resource_Object.METS_Sec_ReaderWriters;

#endregion

namespace SobekCM.Resource_Object.Metadata_File_ReaderWriters
{
    /// <summary> MarcXML metadat reader/writer utilized the MarcXML schema to write a stand-alone
    /// MarcXML file/stream or read from a stand-alone MarcXML file/stream </summary>
    public class MarcXML_File_ReaderWriter : XML_Writing_Base_Type, iMetadata_File_ReaderWriter
    {
        #region iMetadata_File_ReaderWriter Members

        /// <summary> Flag indicates if this reader/writer can read from files and streams </summary>
        /// <value> This property always returns TRUE </value>
        public bool canRead
        {
            get { return true; }
        }

        /// <summary> Flag indicates if this reader/writer can write to files and streams </summary>
        /// <value> This property always return TRUE </value>
        public bool canWrite
        {
            get { return true; }
        }

        /// <summary> Full name which best describes the metadata format this reader/writer utilizes (i.e. Dublin Core, Greenstone file, etc.. ) </summary>
        /// <value>This property always returns 'MarcXML'</value>
        public string Metadata_Type_Name
        {
            get { return "MarcXML"; }
        }

        /// <summary> Abbreviation for the metadata format utilized by this reader/writer (i.e., DC, MODS, GSA, etc.. ) </summary>
        /// <value> This property always returns 'MarcXML'</value>
        public string Metadata_Type_Abbreviation
        {
            get { return "MarcXML"; }
        }

        /// <summary> Reads metadata from an existing metadata file and saves to the provided item/package </summary>
        /// <param name="MetadataFilePathName"> Path and name of the metadata file to read </param>
        /// <param name="Return_Package"> Package into which to read the metadata </param>
        /// <param name="Options"> Dictionary of any options which this metadata reader/writer may utilize </param>
        /// <param name="Error_Message">[OUTPUT] Explanation of the error, if an error occurs during reading </param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        public bool Read_Metadata(string MetadataFilePathName, SobekCM_Item Return_Package, Dictionary<string, object> Options, out string Error_Message)
        {
            // Set default error outpt message
            Error_Message = String.Empty;

            // Create a stream and XML reader and read the metadata
            Stream reader = null;
            XmlTextReader nodeReader = null;
            bool returnValue = true;

            try
            {
                reader = new FileStream(MetadataFilePathName, FileMode.Open, FileAccess.Read);

                // create the node reader
                nodeReader = new XmlTextReader(reader);

                MarcXML_METS_dmdSec_ReaderWriter.Read_MarcXML_Info(nodeReader, Return_Package.Bib_Info, Return_Package, true, Options );
            }
            catch (Exception ee)
            {
                Error_Message = "Error reading MarcXML from stream: " + ee.Message;
                returnValue = false;
            }
            finally
            {
                if (nodeReader != null)
                    nodeReader.Close();
                if (reader != null)
                    reader.Close();
            }

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

                MarcXML_METS_dmdSec_ReaderWriter.Read_MarcXML_Info(nodeReader, Return_Package.Bib_Info, Return_Package, true);
            }
            catch (Exception ee)
            {
                Error_Message = "Error reading MarcXML from stream: " + ee.Message;
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
        ///  <remarks>This writer accepts one option value.  'MarcXML_File_ReaderWriter:Additional_Tags' is a List of additional 
        ///  <see cref="MARC_Field"/> tags which should be added to the standard tags written (Default is NULL).</remarks>
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
                Error_Message = "Error writing MarcXML metadata to file '" + MetadataFilePathName + ": " + ee.Message;
                return false;
            }
        }

        /// <summary> Writes the formatted metadata from the provided item to a TextWriter (usually to an output stream) </summary>
        /// <param name="Output_Stream"></param>
        /// <param name="Item_To_Save"> Package with all the metadata to save </param>
        /// <param name="Options"> Dictionary of any options which this metadata reader/writer may utilize </param>
        /// <param name="Error_Message">[OUTPUT] Explanation of the error, if an error occurs during write </param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        ///  <remarks>This writer accepts one option value.  'MarcXML_File_ReaderWriter:Additional_Tags' is a List of additional 
        ///  <see cref="MARC_Field"/> tags which should be added to the standard tags written (Default is NULL).</remarks>
        public bool Write_Metadata(TextWriter Output_Stream, SobekCM_Item Item_To_Save, Dictionary<string, object> Options, out string Error_Message)
        {
            // Set default error outpt message
            Error_Message = String.Empty;

            // Get all the standard tags
            MARC_Record tags = Item_To_Save.To_MARC_Record();

            // Look for extra tags to add in the OPTIONS
            if (Options.ContainsKey("MarcXML_File_ReaderWriter:Additional_Tags"))
            {
                object add_tags_obj = Options["MarcXML_File_ReaderWriter:Additional_Tags"];
                if (add_tags_obj != null)
                {
                    try
                    {
                        List<MARC_Field> add_tags = (List<MARC_Field>) add_tags_obj;
                        foreach (MARC_Field thisTag in add_tags)
                        {
                            tags.Add_Field(thisTag);
                        }
                    }
                    catch
                    {
                        Error_Message = "Unable to cast provided option ( MarcXML_File_ReaderWriter:Additional_Tags ) to List<MARC_Field> object";
                        return false;
                    }
                }
            }

            // Start to build the XML result
            Output_Stream.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
            Output_Stream.WriteLine("<collection xmlns=\"http://www.loc.gov/MARC21/slim\">");
            Output_Stream.WriteLine("  <record>");

            // Add the leader
            Output_Stream.WriteLine("    <leader>" + tags.Leader + "</leader>");

            foreach (MARC_Field thisTag in tags.Sorted_MARC_Tag_List)
            {
                if ((thisTag.Tag >= 1) && (thisTag.Tag <= 8))
                {
                    Output_Stream.WriteLine("    <controlfield tag=\"" + thisTag.Tag.ToString().PadLeft(3, '0') + "\">" + base.Convert_String_To_XML_Safe(thisTag.Control_Field_Value).Replace("&amp;bar;", "|") + "</controlfield>");
                }
                else
                {
                    Output_Stream.WriteLine("    <datafield tag=\"" + thisTag.Tag.ToString().PadLeft(3, '0') + "\" ind1=\"" + thisTag.Indicators[0] + "\" ind2=\"" + thisTag.Indicators[1] + "\">");

                    string[] splitter = thisTag.Control_Field_Value.Split("|".ToCharArray());
                    foreach (string subfield in splitter)
                    {
                        if (subfield.Length > 2)
                        {
                            Output_Stream.WriteLine("      <subfield code=\"" + subfield[0] + "\">" + base.Convert_String_To_XML_Safe(subfield.Substring(2).Trim()).Replace("&amp;bar;", "|") + "</subfield>");
                        }
                    }

                    Output_Stream.WriteLine("    </datafield>");
                }
            }

            Output_Stream.WriteLine("  </record>");
            Output_Stream.WriteLine("</collection>");

            return true;
        }

        #endregion
    }
}