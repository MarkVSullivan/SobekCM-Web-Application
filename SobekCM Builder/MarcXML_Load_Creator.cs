using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace SobekCM.Builder
{
    /// <summary> Class is used to create the <a href="http://fclaweb.fcla.edu/mango">Mango</a> MarcXML feed of all digital resources
    /// in this SobekCM library </summary>
    public class MarcXML_Load_Creator
    {
        private string server_root1;
        private bool isValid;

        private List<string> error_messages;
        private List<int> error_lines;
        private string errors;
        
        /// <summary> Constructor for a new instance of the Mango_Load_Creator class </summary>
        public MarcXML_Load_Creator()
        {
            
            server_root1 = @"\\cns-uflib-ufdc\UFDC\RESOURCES\";
        }

        /// <summary> Create the MarcXML feed file for all files within this SobekCM library for loading into
        /// <a href="http://fclaweb.fcla.edu/mango">Mango</a> </summary>
        /// <param name="XML_File"> Name of the resulting file </param>
        /// <returns> TRUE if succesful, otherwise FALSE </returns>
        /// <remarks> This first creates the MarcXML file, and then validates it against the schema </remarks>
        public bool Create_MarcXML_Data_File(bool Test_Feed_Flag, string XML_File)
        {
            DataTable endecaItemList = null;
            if (Test_Feed_Flag)
            {
                endecaItemList = SobekCM.Library.Database.SobekCM_Database.MarcXML_Test_Feed_Records;
            }
            else
            {
                endecaItemList = SobekCM.Library.Database.SobekCM_Database.MarcXML_Production_Feed_Records;
            }

            if (endecaItemList == null)
            {
                if (SobekCM.Library.Database.SobekCM_Database.Last_Exception != null)
                    errors = "Error pulling list for the feed: " + SobekCM.Library.Database.SobekCM_Database.Last_Exception.Message;
                else
                    errors = "Error pulling list for the feed, NULL was returned";
                return false;
            }

            if (!System.IO.Directory.Exists(server_root1))
            {
                errors = "Server root ( " + server_root1 + " ) does not exist!  Configuration incorrect.";
                return false;
            }

            string last_bibid = String.Empty;

            System.IO.StreamWriter writer = new System.IO.StreamWriter(XML_File, false);

            writer.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
            writer.WriteLine("<collection xmlns=\"http://www.loc.gov/MARC21/slim\">");

            int total_added = 0;
            System.IO.StreamReader reader;
            foreach (DataRow thisRow in endecaItemList.Rows)
            {

                string this_bibid = thisRow["BibID"].ToString();
                string this_vid = thisRow["VID"].ToString();

                try
                {
                    string institution_code = this_bibid.Substring(0, 2);
                    if (last_bibid != this_bibid)
                    {
                        last_bibid = this_bibid;
                        total_added++;

                        string marc_xml = server_root1 + thisRow["File_Location"].ToString().Replace("/", "\\") + "\\" + this_vid + "\\marc.xml";
                        if (System.IO.File.Exists(marc_xml))
                        {
                            reader = new System.IO.StreamReader(marc_xml);
                            reader.ReadLine();
                            reader.ReadLine();

                            string marc_xml_content = reader.ReadToEnd();

                            if (this_bibid.IndexOf("AM") == 0)
                            {
                                writer.Write(marc_xml_content.Replace("</collection>", "").Replace("UFDC", "AMDC").Replace("University of Florida Digital Collections", "Florida A&amp;M University Digital Collections"));
                            }
                            else
                            {
                                writer.Write(marc_xml_content.Replace("</collection>", ""));
                            }

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
            errors = String.Empty;
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
                marcXmlSettings.ValidationEventHandler += new ValidationEventHandler(MyValidationEventHandler);

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
                int record_line_number = 0;
                int line_number = 1;
                int next_error_number = 0;
                System.IO.StreamReader rereader = new System.IO.StreamReader(XML_File);
                string line = rereader.ReadLine();
                while ((line != null) && (error_lines.Count > next_error_number))
                {
                    // Get the record number
                    if (line.IndexOf("<controlfield tag=\"001\">") >= 0)
                    {
                        record_number = line.Replace("<controlfield tag=\"001\">", "").Replace("</controlfield>", "");
                        record_line_number = line_number;
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
                errors = errorBuilder.ToString();
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
                    errors = exceptionErrorBuilder.ToString();
                }
            }

            // Return the valid flag
            return isValid;
        }

        /// <summary> Gets the error string for the last Mango report creation and validation process </summary>
        public string Errors
        {
            get { return errors; }
        }

        /// <summary> EventHandler is called when there is an error during validation </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void MyValidationEventHandler(object sender, ValidationEventArgs args)
        {
            // Set the flag
            isValid = false;

            // Add this error message to the building list of errors
            error_messages.Add("Line " + args.Exception.LineNumber + " : " + args.Exception.LinePosition + " " + args.Message + "\r\n");

            // Also save the error message line to insert the record into the error as well
            error_lines.Add(args.Exception.LineNumber);
        }
    }
}
