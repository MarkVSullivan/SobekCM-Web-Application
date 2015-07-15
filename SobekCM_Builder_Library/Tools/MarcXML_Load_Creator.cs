#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using SobekCM.Library.Database;

#endregion

namespace SobekCM.Builder_Library.Tools
{
    /// <summary> Class is used to create the MarcXML feed of all digital resources
    /// in this SobekCM library </summary>
    public class MarcXML_Load_Creator
    {
        private readonly string server_root1;
        private bool isValid;

        private List<string> error_messages;
        private List<int> error_lines;

        /// <summary> Constructor for a new instance of the Mango_Load_Creator class </summary>
        public MarcXML_Load_Creator()
        {
            server_root1 = @"\\cns-uflib-ufdc\UFDC\RESOURCES\";
        }

        /// <summary> Create the MarcXML feed file for all files within this SobekCM library for loading into
        /// <a href="http://fclaweb.fcla.edu/mango">Mango</a> </summary>
        /// <param name="Test_Feed_Flag"> Flag indicates if this is to create the test feed</param>
        /// <param name="XML_File"> Name of the resulting file </param>
        /// <returns> TRUE if succesful, otherwise FALSE </returns>
        /// <remarks> This first creates the MarcXML file, and then validates it against the schema </remarks>
        public bool Create_MarcXML_Data_File(bool Test_Feed_Flag, string XML_File)
        {
            DataTable endecaItemList = Test_Feed_Flag ? SobekCM_Database.MarcXML_Test_Feed_Records : SobekCM_Database.MarcXML_Production_Feed_Records;

            if (endecaItemList == null)
            {
                if (SobekCM_Database.Last_Exception != null)
                    Errors = "Error pulling list for the feed: " + SobekCM_Database.Last_Exception.Message;
                else
                    Errors = "Error pulling list for the feed, NULL was returned";
                return false;
            }

            if (!Directory.Exists(server_root1))
            {
                Errors = "Server root ( " + server_root1 + " ) does not exist!  Configuration incorrect.";
                return false;
            }

            string last_bibid = String.Empty;

            StreamWriter writer = new StreamWriter(XML_File, false);

            writer.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
            writer.WriteLine("<collection xmlns=\"http://www.loc.gov/MARC21/slim\">");

            foreach (DataRow thisRow in endecaItemList.Rows)
            {

                string this_bibid = thisRow["BibID"].ToString();
                string this_vid = thisRow["VID"].ToString();

                try
                {
                    if (last_bibid != this_bibid)
                    {
                        last_bibid = this_bibid;

                        string marc_xml = server_root1 + thisRow["File_Location"].ToString().Replace("/", "\\") + "\\" + this_vid + "\\marc.xml";
                        if (File.Exists(marc_xml))
                        {
                            StreamReader reader = new StreamReader(marc_xml);
                            reader.ReadLine();
                            reader.ReadLine();

                            string marc_xml_content = reader.ReadToEnd();
                            writer.Write(marc_xml_content.Replace("</collection>", ""));

                            reader.Close();
                        }
                    }

                }
                catch 
                {
                    
                }
            }


            writer.WriteLine("</collection>");
            writer.Flush();
            writer.Close();

            // Set some initial values
            isValid = true;
            Errors = String.Empty;
            error_messages = new List<string>();
            error_lines = new List<int>();

            // Validate this XML file
            // Define the new XmlSChemeCollection
            XmlSchemaSet cache = new XmlSchemaSet();

            // Import the METS schema, which is the only schema needed.
            // METS schema governs the importing of other schemas.
            try
            {
                cache.Add("http://www.loc.gov/MARC21/slim", "http://www.loc.gov/standards/marcxml/schema/MARC21slim.xsd");
            }
            catch
            {
                error_messages.Add("UNABLE TO RETRIEVE THE LOC SCHEMA");
                return false;
            }

            try
            {

                // Create the reader and validator
                XmlReaderSettings marcXmlSettings = new XmlReaderSettings();
                marcXmlSettings.Schemas.Add(cache);
                marcXmlSettings.ValidationType = ValidationType.Schema;
                marcXmlSettings.ValidationEventHandler += MyValidationEventHandler;

                XmlReader validator = XmlReader.Create(XML_File, marcXmlSettings);

                // Step through the XML file
                while (validator.Read()) { /* Just reading through, looking for problems... */ }
                validator.Close();
            }
            catch ( Exception ee )
            {

                error_messages.Add("UNSPECIFIED ERROR CAUGHT DURING VALIDATION");
                error_messages.Add(ee.Message);
                error_lines.Add(-1);
                error_lines.Add(-1);
                isValid = false;
            }

            // Now, build the final error message
            if (error_lines.Count > 0)
            {
                StringBuilder errorBuilder = new StringBuilder();
                string record_number = String.Empty;
                int line_number = 1;
                int next_error_number = 0;
                StreamReader rereader = new StreamReader(XML_File);
                string line = rereader.ReadLine();
                while ((line != null) && (error_lines.Count > next_error_number))
                {
                    // Get the record number
                    if (line.IndexOf("<controlfield tag=\"001\">") >= 0)
                    {
                        record_number = line.Replace("<controlfield tag=\"001\">", "").Replace("</controlfield>", "");
                    }

                    // Does this match an error line?
                    if (line_number == error_lines[next_error_number])
                    {
                        errorBuilder.AppendLine(error_messages[next_error_number]);
                        errorBuilder.AppendLine(record_number);
                        errorBuilder.AppendLine();

                        next_error_number++;

                        while ((error_lines.Count > next_error_number) && (error_lines[next_error_number] < 0))
                        {
                            next_error_number++;
                        }

                    }

                    line = rereader.ReadLine();
                    line_number++;
                }
                rereader.Close();
                Errors = errorBuilder.ToString();
            }
            else
            {
                if (error_messages.Count > 0)
                {
                    StringBuilder exceptionErrorBuilder = new StringBuilder();
                    foreach (string thisError in error_messages)
                    {
                        exceptionErrorBuilder.AppendLine(thisError);
                        exceptionErrorBuilder.AppendLine();
                    }
                    Errors = exceptionErrorBuilder.ToString();
                }
            }

            // Return the valid flag
            return isValid;
        }

        /// <summary> Gets the error string for the last Mango report creation and validation process </summary>
        public string Errors { get; private set; }

        /// <summary> EventHandler is called when there is an error during validation </summary>
        /// <param name="Sender"></param>
        /// <param name="Args"></param>
        private void MyValidationEventHandler(object Sender, ValidationEventArgs Args)
        {
            // Set the flag
            isValid = false;

            // Add this error message to the building list of errors
            error_messages.Add("Line " + Args.Exception.LineNumber + " : " + Args.Exception.LinePosition + " " + Args.Message + "\r\n");

            // Also save the error message line to insert the record into the error as well
            error_lines.Add(Args.Exception.LineNumber);
        }
    }
}
