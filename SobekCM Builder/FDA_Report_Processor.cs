using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SobekCM.Library.Settings;
using SobekCM.Tools.FDA;
using SobekCM.Library;
using SobekCM.Library.Builder;

namespace SobekCM.Builder
{
    /// <summary> Processor class steps through and processes all the <a href="http://fclaweb.fcla.edu/FDA_landing_page">Florida Digital Archive</a> reports under a given directory, 
    /// reading the FDA reports and saving the pertinent information into the SobekCM database </summary>
    public class FDA_Report_Processor
    {
        private bool save_to_db, delete, include_brief, recurse, write_brief_always, write_brief_on_warning;
        private int success_count, error_count;

        /// <summary> Constructor for a new instance of the FDA_Report_Processor class </summary>
        /// <param name="Save_to_Database"> Flag indicates whether to save the information to the database </param>
        /// <param name="Delete"> Flag indicates whether to delete the report once processed </param>
        /// <param name="Include_Briefs"> Flag indicates whether to exclude all "brief" rewritten versions of the FDA reports</param>
        /// <param name="Recurse"> Flag indicates whether to recurse through all subfolders looking for reports to process </param>
        /// <param name="Write_Brief_Always"> Flag indicates whether a "brief" version should be written for all reports</param>
        /// <param name="Write_Brief_On_Warning"> Flag indicates whether a "brief" version should be written for reports containing warnings </param>
        public FDA_Report_Processor( bool Save_to_Database, bool Delete, bool Include_Briefs, bool Recurse, bool Write_Brief_Always, bool Write_Brief_On_Warning )
        {
            save_to_db = Save_to_Database;
            delete = Delete;
            include_brief = Include_Briefs;
            recurse = Recurse;
            write_brief_always = Write_Brief_Always;
            write_brief_on_warning = Write_Brief_On_Warning;
        }

        /// <summary> Constructor for a new instance of the FDA_Report_Processor class </summary>
        public FDA_Report_Processor()
        {
            save_to_db = true;
            delete = true;
            include_brief = false;
            recurse = true;
            write_brief_always = true;
            write_brief_on_warning = true;
        }

        /// <summary> Process all reports under the provided directory according to pre-established flags  </summary>
        /// <param name="source_directory"> Directory under which to look for FDA reports to process </param>
        public void Process(string source_directory )
        {
            // Get list of XML files
            string[] xml_files;
            if (recurse)
            {
                xml_files = get_reports_recursively(source_directory);
            }
            else
            {
                xml_files = Directory.GetFiles(source_directory, "*.xml");
            }

            // Loop through each file
            success_count = 0;
            error_count = 0;
            bool database_successful = true;
            foreach (string thisXML in xml_files)
            {
                if ((thisXML.IndexOf(".brief.xml") < 0) || (include_brief))
                {
                    // Ensure the report still exists
                    if (File.Exists(thisXML))
                    {
                        try
                        {
                            // Read the XML report
                            FDA_Report_Data data = FDA_Report_Reader.Read(thisXML);

                            if (data == null)
                            {
                                error_count++;
                            }
                            else
                            {

                                // If this is a valid report, save it to the collection
                                if (data.Report_Type != FDA_Report_Type.INVALID)
                                {
                                    // Increment success count
                                    success_count++;
                                    //OnNewProgress(++success_count, 2 * (xml_files.Length + 2));

                                    // Set the flags for this item
                                    database_successful = true;

                                    // Rewrite this if it is INGEST or DISSEMINATION and user asked to
                                    if (((write_brief_always) || ((write_brief_on_warning) && (data.Warnings > 0)))
                                        && ((data.Report_Type == FDA_Report_Type.INGEST) || (data.Report_Type == FDA_Report_Type.DISSEMINATION)))
                                    {
                                        // Write the brief report
                                        FDA_Report_Writer.Write(data, data.FileName);
                                    }

                                    // Did the user ask to save to the database?
                                    if (save_to_db)
                                    {
                                        // Save to the database
                                        if (!SobekCM.Library.Database.SobekCM_Database.FDA_Report_Save( data ))
                                        {
                                            // If unsuccessful, set unsuccessful flag
                                            database_successful = false;
                                            error_count++;
                                        }
                                    }

                                    // Move to the web space
                                    string possible_bib_vid = data.Package;
                                    if ((possible_bib_vid.Length == 16) && (possible_bib_vid[10] == '_'))
                                    {
                                        string bibid = possible_bib_vid.Substring(0, 10);
                                        string vid = possible_bib_vid.Substring(11, 5);
                                        string assocFilePath = bibid.Substring(0, 2) + "\\" + bibid.Substring(2, 2) + "\\" + bibid.Substring(4, 2) + "\\" + bibid.Substring(6, 2) + "\\" + bibid.Substring(8) + "\\" + vid;

                                        // Determine the destination folder for this resource
                                        string serverPackageFolder = SobekCM_Library_Settings.Image_Server_Network + assocFilePath;

                                        // Make sure a directory exists here
                                        if (!Directory.Exists(serverPackageFolder))
                                        {
                                            Directory.CreateDirectory(serverPackageFolder);
                                        }

                                        // Copy the file
                                        string fileName = (new FileInfo(data.FileName)).Name;
                                        File.Copy(data.FileName, serverPackageFolder + "\\" + fileName.Replace(".brief", ""), true);
                                    }

                                    // If the user asked to delete the file and all work was successul,
                                    // and this was not an error, delete the original report
                                    if ((delete) && (database_successful))
                                    {
                                        try
                                        {
                                            File.Delete(data.FileName);
                                        }
                                        catch (Exception ee)
                                        {
                                            error_count++;
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ee)
                        {
                            error_count++;
                        }
                    }
                }

                // Show status for this part
                //OnNewProgress(success_count++, 2 * (xml_files.Length + 2));
            }

            // Show status for this part
            OnNewProgress(xml_files.Length + 2, xml_files.Length + 2);
        }

        /// <summary> Gets the number of successfully processed FDA reports </summary>
        public int Success_Count
        {
            get { return success_count; }
        }

        /// <summary> Gets the number of errors encountered during processing  </summary>
        public int Error_Count
        {
            get { return error_count; }
        }


        private void OnNewProgress(int current, int maximum)
        {
            // This part is disabled
        }

        private string[] get_reports_recursively(string directory)
        {
            List<string> reports = new List<string>();
            reports_recurse(directory, reports);

            string[] returnVal = new string[reports.Count];
            for (int i = 0; i < reports.Count; i++)
                returnVal[i] = reports[i].ToString();

            return returnVal;
        }

        private void reports_recurse(string directory, List<string> reports)
        {
            // Add all the files under the current directory
            reports.AddRange(Directory.GetFiles(directory, "*.xml"));

            // Step through all subdirectories
            string[] subdirs = Directory.GetDirectories(directory);
            foreach (string subdir in subdirs)
                reports_recurse(subdir, reports);
        }
    }
}
