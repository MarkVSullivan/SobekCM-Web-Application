#region Using directives

using System;
using System.Collections;
using System.Xml;

#endregion

namespace SobekCM.Tools.FDA
{
    /// <summary> Class is used to read the XML report from FDA </summary>
    /// <remarks> This uses an XML reader to iterate through each node in a FDA report and build the associated <see cref="FDA_Report_Data" /> object.<br /><br />
    /// This does not save any GLOBAL files or any files not included in the package from the DEPOSITOR.  When saving the individual file information in 
    /// each package, this greatly ballooned the size of each FDA report data.  </remarks>
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
    /// </example>
    public class FDA_Report_Reader
    {
        private static string lastError;

        /// <summary> Get the last exception which occurred  </summary>
        public string Last_Exception
        {
            get { return lastError; }
        }

        /// <summary>Reads the FDA report and creates the associated data object </summary>
        /// <param name="FileName">Name (including path) of the report to read</param>
        /// <returns>All the important data from an ingest report</returns>
        /// <remarks>If an exception occurred during this process, the last exception is accessible in the <see cref="Last_Exception" /> property. </remarks>
        public static FDA_Report_Data Read(string FileName)
        {
            // Clear the last error
            lastError = String.Empty;

            // Load the XML Document
            XmlDocument report_xml = new XmlDocument();
            report_xml.Load(FileName);

            // Create the data repository
            FDA_Report_Data report_data = new FDA_Report_Data {FileName = FileName};

            // Step through
            try
            {
                // Find the REPORT node
                foreach (XmlNode reportNode in report_xml.ChildNodes)
                {
                    if (reportNode.Name == "REPORT")
                    {
                        // Find the relevant node
                        foreach (XmlNode ingestNode in reportNode.ChildNodes)
                        {
                            // Is this the INGEST or DISSEMINATION information?
                            if ((ingestNode.Name == "INGEST") || ( ingestNode.Name == "DISSEMINATION" ))
                            {
                                // Set the report type
                                report_data.Report_Type = ingestNode.Name == "INGEST" ? FDA_Report_Type.INGEST : FDA_Report_Type.WITHDRAWAL;

                                // Read the attribute information
                                if (ingestNode.Attributes != null)
                                {
                                    foreach (XmlAttribute thisAttribute in ingestNode.Attributes)
                                    {
                                        switch (thisAttribute.Name)
                                        {
                                            case "IEID":
                                                report_data.IEID = thisAttribute.Value;
                                                break;

                                            case "INGEST_TIME":
                                                string date_string_value =
                                                    thisAttribute.Value.Replace("-0400 ", "").Replace("-0500 ", "");
                                                string[] split = date_string_value.Split(" ".ToCharArray());
                                                if (split.Length == 5)
                                                {
                                                    string new_date_string = split[1] + " " + split[2] + " " + split[4] +" " + split[3];
                                                    DateTime report_date;
                                                    if (DateTime.TryParse(new_date_string, out report_date))
                                                        report_data.Date = report_date;
                                                }
                                                else
                                                {
                                                    // Just try to convert it as it is
                                                    DateTime report_date;
                                                    if (DateTime.TryParse(thisAttribute.Value, out report_date))
                                                        report_data.Date = report_date;
                                                }
                                                break;

                                            case "PACKAGE":
                                                report_data.Package = thisAttribute.Value;
                                                break;
                                        }
                                    }
                                }

                                // Find the AGREEMENT and FILES information
                                foreach (XmlNode childNode in ingestNode.ChildNodes)
                                {
                                    switch (childNode.Name)
                                    {
                                        case "AGREEMENT_INFO":
                                            if (childNode.Attributes != null)
                                            {
                                                foreach (XmlAttribute thisAttribute in childNode.Attributes)
                                                {
                                                    switch (thisAttribute.Name)
                                                    {
                                                        case "ACCOUNT":
                                                            report_data.Account = thisAttribute.Value;
                                                            break;

                                                        case "PROJECT":
                                                            report_data.Project = thisAttribute.Value;
                                                            break;
                                                    }
                                                }
                                            }
                                            break;

                                        case "FILES":
                                            read_file_info(childNode, report_data);
                                            break;
                                    }
                                }

                                // No need to continue through this report anymore
                                break;

                            } // End INGEST or DISSEMINATION node

                            // Is this WITHDRAWAL information?
                            if (ingestNode.Name == "WITHDRAWAL")
                            {
                                // Set the report type
                                report_data.Report_Type = FDA_Report_Type.WITHDRAWAL;

                                // Read the attribute information
                                if (ingestNode.Attributes != null)
                                {
                                    foreach (XmlAttribute thisAttribute in ingestNode.Attributes)
                                    {
                                        switch (thisAttribute.Name)
                                        {
                                            case "IEID":
                                                report_data.IEID = thisAttribute.Value;
                                                break;

                                            case "WITHDRAWAL_TIME":
                                                    DateTime report_date;
                                                    if (DateTime.TryParse(thisAttribute.Value, out report_date))
                                                        report_data.Date = report_date;
                                                break;

                                            case "PACKAGE_NAME":
                                                report_data.Package = thisAttribute.Value;
                                                break;

                                            case "NOTE":
                                                report_data.Message_Note = thisAttribute.Value;
                                                break;
                                        }
                                    }
                                }

                                // No need to continue through this report anymore
                                break;

                            } // End WITHDRAWAL node

                            // Is this ERROR information?
                            if (ingestNode.Name == "ERROR")
                            {
                                // Set the report type
                                report_data.Report_Type = FDA_Report_Type.ERROR;

                                // Read the attribute information
                                if (ingestNode.Attributes != null)
                                {
                                    foreach (XmlAttribute thisAttribute in ingestNode.Attributes)
                                    {
                                        switch (thisAttribute.Name)
                                        {
                                            case "REJECT_TIME":
                                                DateTime report_date;
                                                if (DateTime.TryParse(thisAttribute.Value, out report_date))
                                                    report_data.Date = report_date;
                                                break;
                                        }
                                    }
                                }

                                // Step through the children nodes
                                foreach (XmlNode childNode in ingestNode.ChildNodes)
                                {
                                    // Is this the MESSAGE?
                                    if (childNode.Name == "MESSAGE")
                                    {
                                        // Remove alot of empty space, if it exists
                                        string message = childNode.InnerText.Replace("\n", ". ").Replace("\r", "");
                                        while (message.IndexOf("  ") >= 0)
                                        {
                                            message = message.Replace("  ", " ");
                                        }

                                        // Save the cleaned up message
                                        report_data.Message_Note = message;                                        
                                    }

                                    // Is this the PACKAGE name?
                                    if (childNode.Name == "PACKAGE")
                                    {
                                        report_data.Package = childNode.InnerText;
                                    }
                                }

                                // No need to continue through this report anymore
                                break;

                            } // End ERROR node

                        } // End stepping through subchildren under REPORT

                    } // End REPORT node

                } // End stepping through all the nodes in the XML document
            }
            catch (Exception ee)
            {
                lastError = ee.ToString();
                return null;
            }

            // Return the built object
            return report_data;
        }


        private static void read_file_info(XmlNode FilesNode, FDA_Report_Data ReportData)
        {
            // Declare some variables for all the files
            ArrayList storage_nodes = new ArrayList();

            // Step through all the individual files
            foreach (XmlNode fileNode in FilesNode)
            {
                // Clear the values
                string dfid = String.Empty;
                string global = String.Empty;
                string origin = String.Empty;
                string path = String.Empty;
                string preservation = String.Empty;
                string size = String.Empty;
                storage_nodes.Clear();

                // Parse the attributes associated with this file
                if (fileNode.Attributes != null)
                {
                    foreach (XmlAttribute fileAttribute in fileNode.Attributes)
                    {
                        switch (fileAttribute.Name)
                        {
                            case "DFID":
                                dfid = fileAttribute.Value;
                                break;

                            case "GLOBAL":
                                global = fileAttribute.Value;
                                break;

                            case "ORIGIN":
                                origin = fileAttribute.Value;
                                break;

                            case "PATH":
                                path = fileAttribute.Value;
                                break;

                            case "PRESERVATION":
                                preservation = fileAttribute.Value;
                                break;

                            case "SIZE":
                                size = fileAttribute.Value;
                                break;
                        }
                    }
                }

                // Is this a NON-GLOBAL and DEPOSITOR originated file?
                if ((global.ToLower() == "false") && (origin.ToUpper() == "DEPOSITOR"))
                {
                    // This is a valid file to save
                    FDA_File file = new FDA_File
                                        {ID = dfid, Name = path, Preservation = preservation, XML_Node = fileNode};
                    ReportData.Files.Add(file);

                    // Get the size
                    long size_numeric;
                    if (Int64.TryParse(size, out size_numeric))
                        file.Size = size_numeric;

                    // Step through the subnodes associated with this file
                    foreach (XmlNode subNode in fileNode)
                    {
                        // Collect the checksums for this file
                        if (subNode.Name == "MESSAGE_DIGEST")
                        {
                            if (subNode.Attributes != null)
                            {
                                if ((subNode.Attributes.Count > 0) && (subNode.Attributes[0].Value == "MD5"))
                                {
                                    file.MD5_Checksum = subNode.InnerText.Trim();
                                }
                                if ((subNode.Attributes.Count > 0) && (subNode.Attributes[0].Value == "SHA-1"))
                                {
                                    file.SHA1_Checksum = subNode.InnerText.Trim();
                                }
                            }
                        }

                        // Collect the STORAGE subnodes to remove later
                        if ( subNode.Name == "STORAGE" )
                        {
                            storage_nodes.Add( subNode );
                        }

                        // Count the number of warnings, and save them at the file level
                        if (subNode.Name == "WARNING")
                        {
                            // Increment the warning count
                            ReportData.Warnings++;

                            // Get the information about this warning
                            if (( subNode.Attributes != null ) && (subNode.Attributes.Count > 0) && (subNode.Attributes[0].Name == "CODE"))
                            {
                                // Add this warning to the file
                                file.Add_Warning(subNode.Attributes[0].Value, subNode.InnerText);
                            }
                        }

                        // Get the NOTE from any EVENT listed
                        if (subNode.Name == "EVENT")
                        {
                            // Look for the NOTE subnode
                            foreach (XmlNode eventNode in subNode.ChildNodes)
                            {
                                if (eventNode.Name == "NOTE")
                                {
                                    file.Event = file.Event + eventNode.InnerText + ". ";
                                }
                            }
                        }
                    }

                    // Remove all the storage nodes
                    foreach (XmlNode deleteNode in storage_nodes)
                    {
                        fileNode.RemoveChild(deleteNode);
                    }
                }
            }
        }
    }
}
