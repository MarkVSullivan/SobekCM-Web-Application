#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace SobekCM.Tools.FDA
{
    /// <summary> Class stores all the important data from a FDA Ingest Report </summary>
    /// <example>
    /// <i><em>Example #1</em></i> - Simple code to read a FDA report, save it to the database, and write it to another location
    /// <code>
    ///    // Read the report and save it in another location
    ///    public void Read_And_Save_Report(string Source, string Destination)
    ///    {
    ///        // Try to read the report
    ///        FDA_Report_Data reportData = FDA_Report_Reader.Read(Source);
    ///
    ///        // If this appears valid, save to the database
    ///        if (reportData.Report_Type != FDA_Report_Type.INVALID)
    ///        {
    ///            // Try to save to the database
    ///            if ( reportData.Save_To_Database())
    ///            {
    ///                 // Since this was successful, delete the old and save the briefer version
    ///                 File.Delete( Source );
    ///                 FDA_Report_Writer.Write( reportData, Destination );    /// 
    ///            }
    ///        }      
    ///    }
    /// </code>
    /// <i><em>Example #2</em></i> - Below is the actual implemenation used by the FDA_Report_Processor.Process method of the FDA_Report_Processor class:
    /// <code>
    ///        public void Process()
    ///        {
    ///           // Get list of XML files
    ///            string[] xml_files;
    ///            if (recurse)
    ///            {
    ///                xml_files = get_reports_recursively(source_directory);
    ///            }
    ///            else
    ///            {
    ///                xml_files = Directory.GetFiles(source_directory, "*.xml");
    ///            }
    ///
    ///            // Loop through each file
    ///            int count = 0;
    ///            bool database_successful = true;
    ///            bool rewrite_successful = true;
    ///            foreach (string thisXML in xml_files)
    ///            {
    ///                if ((thisXML.IndexOf(".brief.xml") &lt; 0) || (include_brief))
    ///                {
    ///                    // Read the XML report
    ///                    FDA_Report_Data data = UF.FDA.Core.FDA_Report_Reader.Read(thisXML);
    ///
    ///                    // If this is a valid report, save it to the collection
    ///                    if (data.Report_Type != FDA_Report_Type.INVALID)
    ///                    {
    ///                        // Show status for this part
    ///                        OnNewProgress(++count, 2 * (xml_files.Length + 2));
    ///
    ///                        // Set the flags for this item
    ///                        database_successful = true;
    ///                        rewrite_successful = true;
    ///
    ///                        // Rewrite this if it is INGEST or DISSEMINATION and user asked to
    ///                        if (((write_brief_always) || ((write_brief_on_warning) &amp;&amp; (data.Warnings &gt; 0)))
    ///                            &amp;&amp; ((data.Report_Type == FDA_Report_Type.INGEST) || (data.Report_Type == FDA_Report_Type.DISSEMINATION)))
    ///                        {
    ///                            // Write the brief report
    ///                            if (!FDA_Report_Writer.Write(data, data.FileName.Replace(".xml", ".brief.xml")))
    ///                            {
    ///                                // If unsuccessful, set unsuccessful flag
    ///                                rewrite_successful = false;
    ///                            }
    ///                        }
    ///
    ///                        // Did the user ask to save to the database?
    ///                        if (save_to_db)
    ///                        {
    ///                            // Save to the database
    ///                            if (!data.Save_To_Database())
    ///                            {
    ///                                // If unsuccessful, set unsuccessful flag
    ///                                database_successful = false;
    ///                            }
    ///                        }
    ///
    ///                        // If the user asked to delete the file and all work was successul,
    ///                        // and this was not an error, delete the original report
    ///                        if ((data.Report_Type != FDA_Report_Type.ERROR) &amp;&amp; (delete) &amp;&amp; (database_successful) &amp;&amp; (rewrite_successful))
    ///                        {
    ///                            try
    ///                            {
    ///                                File.Delete(data.FileName);
    ///                            }
    ///                            catch
    ///                            {
    ///                                System.Windows.Forms.MessageBox.Show("Unable to delete '" + data.FileName + "'");
    ///                            }
    ///                        }
    ///
    ///                        // Add the data to the results table
    ///                        results_form.Add_To_ResultTable(data.Package, data.IEID, data.Report_Type_String, data.Date.ToShortDateString(), data.Warnings, data.Files.Count, data.Message_Note);
    ///
    ///                        // Add the complete object
    ///                        // NOTE: FOR LARGE BATCHES, THIS SHOULD BE EXCLUDED 
    ///                        results_form.Add_Complete_Report_Object(data);
    ///                    }
    ///                }
    ///
    ///                // Show status for this part
    ///                OnNewProgress(++count, 2 * (xml_files.Length + 2));
    ///            }
    ///
    ///            // If there were no valid reports found, stop
    ///            if (xml_files.Length == 0)
    ///            {
    ///                System.Windows.Forms.MessageBox.Show("No valid FDA reports found!      ", "No Reports", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
    ///                return;
    ///            }
    ///
    ///            // Show status for this part
    ///            OnNewProgress(xml_files.Length + 2, xml_files.Length + 2);
    ///        }
    /// </code>
    /// </example>
    public class FDA_Report_Data
    {
        private string filename;
        private readonly List<FDA_File> files;

        /// <summary> Constructor creates a new instance of the FDA_Report_Data class </summary>
        public FDA_Report_Data()
        {
            // Initialize all values
            IEID = String.Empty;
            Package = String.Empty;
            Account = String.Empty;
            Project = String.Empty;
            Message_Note = String.Empty;
            filename = String.Empty;
            files = new List<FDA_File>();
            Report_Type = FDA_Report_Type.INVALID;
            Warnings = 0;
        }

        /// <summary> Gets or sets the type of report which generated this data </summary>
        public FDA_Report_Type Report_Type { get; set; }

        /// <summary> Gets the collection of files associated with this IEID </summary>
        /// <remarks>Returned as a generic list of FDA_File objects</remarks>
        public List<FDA_File> Files
        {
            get { return files; }
        }

        /// <summary> Gets the IEID (Intellectual Entity ID) for this FDA report </summary>
        public string IEID { get; set; }

        /// <summary> Gets the submitted package name for this IEID </summary>
        public string Package { get; set; }

        /// <summary> Gets the account information submitted with this package </summary>
        public string Account { get; set; }

        /// <summary> Gets the project information submitted with this package </summary>
        public string Project { get; set; }

        /// <summary> Gets the message or note returned with the report </summary>
        public string Message_Note { get; set; }

        /// <summary> Gets or sets the name of the file read for this report </summary>
        public string FileName
        {
            get { return filename; }
            set { filename = value; }
        }

        /// <summary> Gets the date this report was created </summary>
        public DateTime Date { get; set; }

        /// <summary> Gets or sets the number of warnings in this package </summary>
        public int Warnings { get; set; }

        /// <summary> Gets the report type as a string </summary>
        public string Report_Type_String
        {
            get
            {
                switch (Report_Type)
                {
                    case FDA_Report_Type.DISSEMINATION:
                        return "Dissemination";

                    case FDA_Report_Type.ERROR:
                        return "Error";

                    case FDA_Report_Type.INGEST:
                        return "Ingest";

                    case FDA_Report_Type.WITHDRAWAL:
                        return "Withdrawal";

                    default:
                        return "INVALID REPORT TYPE";
                }
            }
        }

        /// <summary> Returns the basic information about this report </summary>
        /// <returns>Report information as text </returns>
        public override string ToString()
        {
            // Use a string builder
            StringBuilder writer = new StringBuilder();

            // Write the basic data
            writer.Append("------------------------------------------\r\n");
            writer.Append("REPORT:\t\t" + filename + "\r\n");
            writer.Append("TYPE:\t\t" + Report_Type_String + "\r\n" );

            if ( IEID.Length > 0)
            {
                writer.Append("IEID:\t\t" + IEID + "\r\n");
            }

            if (Package.Length > 0)
            {
                writer.Append("PACKAGE:\t" + Package + "\r\n");
            }

           writer.Append("DATE:\t\t" + Date + "\r\n");

            if (Account.Length > 0)
            {
                writer.Append("ACCOUNT:\t" + Account + "\r\n");
            }

            if (Project.Length > 0)
            {
                writer.Append("PROJECT:\t" + Project + "\r\n");
            }

            if (Message_Note.Length > 0)
            {
                writer.Append("NOTE:\t\t" + Message_Note + "\r\n");
            }

            // Write the files
            foreach (FDA_File file in Files)
            {
                writer.Append("\r\n");
                writer.Append("\tFILE ID:\t" + file.ID + "\r\n");
                writer.Append("\tNAME:\t\t" + file.Name + "\r\n");
                writer.Append("\tPRESERVATION:\t" + file.Preservation + "\r\n");
                writer.Append("\tSIZE:\t\t" + file.Size + "\r\n");
            }
            return writer.ToString();
        }
    }
}
