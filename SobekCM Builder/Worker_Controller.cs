using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.IO;
using System.Threading;
using SobekCM.Library;
using SobekCM.Library.Builder;
using SobekCM.Library.Settings;

namespace SobekCM.Builder
{
    /// <summary> Class controls the execution of all tasks, whether being immediately executed
    /// or running continuously in a background thread </summary>
    public class Worker_Controller
    {
        private bool aborted;
        private bool complete_static_rebuild;
        private DateTime controllerStarted;
        private const int BULK_LOADER_END_HOUR = 23;
        private DateTime feedNextBuildTime;
        private bool verbose;

        /// <summary> Constructor for a new instance of the Worker_Controller class </summary>
        /// <param name="Verbose"> Flag indicates if this should be verbose in the log file and console </param>
        public Worker_Controller( bool Verbose )
        {
            verbose = Verbose;
            controllerStarted = DateTime.Now;
            aborted = false;
            complete_static_rebuild = false;

            // Assign the database connection strings
            SobekCM.Resource_Object.Database.SobekCM_Database.Connection_String = SobekCM_Library_Settings.Database_Connections[0].Connection_String;
            SobekCM.Library.Database.SobekCM_Database.Connection_String = SobekCM_Library_Settings.Database_Connections[0].Connection_String;

            // Pull the values from the database and assign other setting values
            SobekCM_Library_Settings.Local_Log_Directory = System.Windows.Forms.Application.StartupPath + "\\Logs\\";
            DataSet settings = SobekCM.Library.Database.SobekCM_Database.Get_Builder_Settings_Complete(null);
            if (settings == null)
            {
                Console.WriteLine("FATAL ERROR pulling latest settings from the database: " + SobekCM.Library.Database.SobekCM_Database.Last_Exception.Message);
                return;
            }
            if (!SobekCM_Library_Settings.Refresh(settings))
            {
                Console.WriteLine("Error using database settings to refresh SobekCM_Library_Settings in Worker_Controller constructor");
            }

            // Clear any LOCAL logs older than XX days old
            if ( Directory.Exists( SobekCM_Library_Settings.Local_Log_Directory ))
            {
                string[] existing_log_files = Directory.GetFiles(SobekCM_Library_Settings.Local_Log_Directory, "incoming_*.html");
                foreach (string thisFile in existing_log_files)
                {
                    try
                    {
                        string fileName = (new FileInfo(thisFile)).Name.ToUpper().Replace(".HTML", "").Replace("INCOMING_", "");
                        if (fileName.Length == 10)
                        {
                            int year = Convert.ToInt32(fileName.Substring(0, 4));
                            int month = Convert.ToInt32(fileName.Substring(5, 2));
                            int day = Convert.ToInt32(fileName.Substring(8, 2));

                            DateTime logDate = new DateTime(year, month, day);
                            TimeSpan logAge = DateTime.Now.Subtract(logDate);
                            if (logAge.TotalDays > SobekCM_Library_Settings.Builder_Log_Expiration_Days )
                                File.Delete(thisFile);
                        }
                    }
                    catch
                    {

                    }
                }
            }

            // Clear any WEB logs older than XX days old
            if (Directory.Exists(SobekCM_Library_Settings.Log_Files_Directory))
            {
                string[] existing_log_files = Directory.GetFiles(SobekCM_Library_Settings.Log_Files_Directory, "incoming_*.html");
                foreach (string thisFile in existing_log_files)
                {
                    try
                    {
                        string fileName = (new FileInfo(thisFile)).Name.ToUpper().Replace(".HTML", "").Replace("INCOMING_", "");
                        if (fileName.Length == 10)
                        {
                            int year = Convert.ToInt32(fileName.Substring(0, 4));
                            int month = Convert.ToInt32(fileName.Substring(5, 2));
                            int day = Convert.ToInt32(fileName.Substring(8, 2));

                            DateTime logDate = new DateTime(year, month, day);
                            TimeSpan logAge = DateTime.Now.Subtract(logDate);
                            if (logAge.TotalDays > SobekCM_Library_Settings.Builder_Log_Expiration_Days )
                                File.Delete(thisFile);
                        }
                    }
                    catch
                    {

                    }
                }
            }

            // If this starts in an ABORTED mode, set to standard
            Builder_Operation_Flag_Enum operationFlag = Abort_Database_Mechanism.Builder_Operation_Flag;
            if ((operationFlag == Builder_Operation_Flag_Enum.ABORTING) || ( operationFlag == Builder_Operation_Flag_Enum.ABORT_REQUESTED ) || ( operationFlag == Builder_Operation_Flag_Enum.LAST_EXECUTION_ABORTED ))
                Abort_Database_Mechanism.Builder_Operation_Flag = Builder_Operation_Flag_Enum.STANDARD_OPERATION;
        }

        #region Method to execute processes in background

        /// <summary> Continuously execute the processes in a recurring background thread </summary>
        public void Execute_In_Background()
        {
            // Check that this should not be skipped or aborted
            Builder_Operation_Flag_Enum operationFlag = Abort_Database_Mechanism.Builder_Operation_Flag;
            switch (operationFlag)
            {
                case Builder_Operation_Flag_Enum.NO_BUILDING_REQUESTED:
                case Builder_Operation_Flag_Enum.ABORT_REQUESTED:
                case Builder_Operation_Flag_Enum.ABORTING:
                    return;
            }

            //// Always do a complete static rebuild of all the aggregation browses and RSS feeds first
            //if (operationFlag != Builder_Operation_Flag_Enum.PAUSE_REQUESTED)
            //{
            //    SobekCM.Tools.Logs.LogFileXHTML staticRebuildLog = new SobekCM.Tools.Logs.LogFileXHTML(System.Windows.Forms.Application.StartupPath + "/Logs/static_rebuild.html");
            //    staticRebuildLog.New();
            //    SobekCM.Library.Static_Pages_Builder builder = new SobekCM.Library.Static_Pages_Builder(SobekCM_Library_Settings.Application_Server_URL, SobekCM_Library_Settings.Static_Pages_Location);
            //    int errors = builder.Rebuild_All_Static_Pages(staticRebuildLog, false, SobekCM_Library_Settings.Local_Log_Directory);

            //    // Always build an endeca feed first (so it occurs once a day)
            //    if (SobekCM_Library_Settings.Build_MARC_Feed_By_Default)
            //    {
            //        Create_Complete_MarcXML_Feed(false);
            //    }
            //}

            // Set the variable which will control background execution
            int time_between_polls = Convert.ToInt32(SobekCM_Library_Settings.Builder_Seconds_Between_Polls);

            // Create the local log directories
            if (!Directory.Exists(SobekCM_Library_Settings.Local_Log_Directory))
            {
                Console.WriteLine("Creating local log directory: " + SobekCM_Library_Settings.Local_Log_Directory);
                Directory.CreateDirectory(SobekCM_Library_Settings.Local_Log_Directory);
            }

            // Create the web log directory 
            string log_web_location = SobekCM_Library_Settings.Log_Files_Directory;
            if ( SobekCM_Library_Settings.Log_Files_Directory.Length > 0 )
            {
                if (!Directory.Exists(log_web_location))
                    Directory.CreateDirectory(log_web_location);
            }

            // Determine the new log name
            string log_name = "incoming_" + controllerStarted.Year + "_" + controllerStarted.Month.ToString().PadLeft(2, '0') + "_" + controllerStarted.Day.ToString().PadLeft(2, '0') + ".html";
            string local_log_name = SobekCM_Library_Settings.Local_Log_Directory + "\\" + log_name;

            // Create the new log file
            SobekCM.Tools.Logs.LogFileXHTML preloader_logger = new SobekCM.Tools.Logs.LogFileXHTML(local_log_name, "SobekCM Incoming Packages Log", "UFDC_Builder.exe", true);

            // Create the prebuilder object
            Worker_BulkLoader bulkLoader = new Worker_BulkLoader(preloader_logger, verbose);

            // Process any pending FDA reports from the FDA Report DropBox
            if (operationFlag != Builder_Operation_Flag_Enum.PAUSE_REQUESTED)
                Process_Any_Pending_FDA_Reports(bulkLoader);

            // Set the time for the next feed building event to 10 minutes from now
            feedNextBuildTime = DateTime.Now.Add(new TimeSpan(0, 10, 0));

            // Loop continually until the end hour is achieved
            do
            {

                // Check for abort
                if (checkForAbort())
                {
                    Abort_Database_Mechanism.Builder_Operation_Flag = Builder_Operation_Flag_Enum.ABORTING;
                    return;
                }

                // Pull the abort/pause flag
                Builder_Operation_Flag_Enum currentPauseFlag = Abort_Database_Mechanism.Builder_Operation_Flag;

                // Run the PREBUILDER
                if (currentPauseFlag != Builder_Operation_Flag_Enum.PAUSE_REQUESTED)
                {
                    Run_BulkLoader(bulkLoader, verbose);

                    if (aborted)
                    {
                        Abort_Database_Mechanism.Builder_Operation_Flag = Builder_Operation_Flag_Enum.ABORTING;
                        return;
                    }

                    // Is it time to build any RSS/XML feeds?
                    if (DateTime.Compare(DateTime.Now, feedNextBuildTime) >= 0)
                    {
                        bulkLoader.Build_Feeds();
                        feedNextBuildTime = DateTime.Now.Add(new TimeSpan(0, 10, 0));
                    }
                }
                else
                {
                    preloader_logger.AddNonError("Building paused");

                    try
                    {
                        // Publish the log file
                        File.Copy(local_log_name, SobekCM_Library_Settings.Log_Files_Directory + "\\" + log_name, true);
                    }
                    catch (Exception ee)
                    {
                    }
                }

                // Sleep for coorect number of milliseconds
                Thread.Sleep(1000 * time_between_polls);
            } while (DateTime.Now.Hour < BULK_LOADER_END_HOUR);


            // Pull the abort/pause flag
            Builder_Operation_Flag_Enum currentPauseFlag2 = Abort_Database_Mechanism.Builder_Operation_Flag;
            if (currentPauseFlag2 == Builder_Operation_Flag_Enum.PAUSE_REQUESTED)
                return;

            // Initiate the recreation of the links between metadata and collections
            SobekCM.Library.Database.SobekCM_Database.Admin_Update_Cached_Aggregation_Metadata_Links();

            // Initiate a solr/lucene index optimization since we are done loading for a while
            if (DateTime.Now.Day % 2 == 0)
            {
                if (SobekCM_Library_Settings.Document_Solr_Index_URL.Length > 0)
                {
                    Console.WriteLine("Initiating Solr/Lucene document index optimization");
                    SobekCM.Library.Solr.Solr_Controller.Optimize_Document_Index(SobekCM_Library_Settings.Document_Solr_Index_URL);
                }
            }
            else
            {
                if (SobekCM_Library_Settings.Page_Solr_Index_URL.Length > 0)
                {
                    Console.WriteLine("Initiating Solr/Lucene page index optimization");
                    SobekCM.Library.Solr.Solr_Controller.Optimize_Page_Index(SobekCM_Library_Settings.Page_Solr_Index_URL);
                }
            }

            // Sleep for ten minutes to end this
            Thread.Sleep(1000 * 10 * 60);
        }

        private void Process_Any_Pending_FDA_Reports( Worker_BulkLoader prebuilder )
        {

            try
            {
                prebuilder.Process_Any_Pending_FDA_Reports();

                if (prebuilder.Aborted)
                    aborted = true;
            }
            catch (Exception ee)
            {
                int error = 1;
            }
        }

        private void Run_BulkLoader(Worker_BulkLoader prebuilder, bool Verbose)
        {
            try
            {
                prebuilder.Perform_BulkLoader( Verbose );

                // Save information about this last run
                Library.Database.SobekCM_Database.Set_Setting("Builder Version", SobekCM_Library_Settings.CURRENT_BUILDER_VERSION);
                Library.Database.SobekCM_Database.Set_Setting("Builder Last Run Finished", DateTime.Now.ToString());
                Library.Database.SobekCM_Database.Set_Setting("Builder Last Message", prebuilder.Final_Message);

                if (prebuilder.Aborted)
                    aborted = true;
            }
            catch (Exception ee)
            {
                bool error = true;
            }
        }
  
        private void Run_BulkLoader( bool Verbose )
        {
            // Create the local log directories
            if (!Directory.Exists(SobekCM_Library_Settings.Local_Log_Directory))
            {
                Console.WriteLine("Creating local log directory: " + SobekCM_Library_Settings.Local_Log_Directory);
                Directory.CreateDirectory(SobekCM_Library_Settings.Local_Log_Directory);
            }

            // Create the web log directory 
            string log_web_location = SobekCM_Library_Settings.Log_Files_Directory;
            if (!Directory.Exists(log_web_location))
            {
                Console.WriteLine("Creating log files directory: " + SobekCM_Library_Settings.Log_Files_Directory);
                Directory.CreateDirectory(log_web_location);
            }

            // Determine the new log name
            string log_name = "incoming_" + controllerStarted.Year + "_" + controllerStarted.Month.ToString().PadLeft(2, '0') + "_" + controllerStarted.Day.ToString().PadLeft(2, '0') + ".html";
            string local_log_name = SobekCM_Library_Settings.Local_Log_Directory + "\\" + log_name;

            // Create the new log file
            SobekCM.Tools.Logs.LogFileXHTML preloader_logger = new SobekCM.Tools.Logs.LogFileXHTML(local_log_name, "SobekCM Incoming Packages Log", "UFDC_Builder.exe", true);

            try
            {

                Worker_BulkLoader prebuilder = new Worker_BulkLoader(preloader_logger, Verbose );
                prebuilder.Perform_BulkLoader( Verbose );

                // Save information about this last run
                Library.Database.SobekCM_Database.Set_Setting("Builder Version", SobekCM_Library_Settings.CURRENT_BUILDER_VERSION);
                Library.Database.SobekCM_Database.Set_Setting("Builder Last Run Finished", DateTime.Now.ToString());
                Library.Database.SobekCM_Database.Set_Setting("Builder Last Message", prebuilder.Final_Message );


                if (prebuilder.Aborted)
                    aborted = true;
            }
            catch (Exception ee)
            {
                int error = 1;
            }

            try
            {
                // Publish the log file
                File.Copy(local_log_name, SobekCM_Library_Settings.Log_Files_Directory + "\\" + log_name, true);
            }
            catch (Exception ee)
            {
            }


        }

         #endregion

        #region Method to immediately execute all requested processes

        /// <summary> Immediately perform all requested tasks </summary>
        /// <param name="build_production_marcxml_feed"> Flag indicates if the MarcXML feed for Mango should be produced </param>
        /// <param name="run_bulkloader"> Flag indicates if the preload </param>
        /// <param name="complete_static_rebuild"> Flag indicates whether to rebuild all the item static pages </param>
        /// <param name="marcrebuild"> Flag indicates if all the MarcXML files for each resource should be rewritten from the METS/MODS metadata files </param>
        public void Execute_Immediately(bool build_production_marcxml_feed, bool build_test_marcxml_feed, bool run_bulkloader, bool complete_static_rebuild, bool marcrebuild )
        {
            // Save the flag for complete static rebuild
            this.complete_static_rebuild = complete_static_rebuild;

            if (complete_static_rebuild)
            {
                SobekCM.Tools.Logs.LogFileXHTML staticRebuildLog = new SobekCM.Tools.Logs.LogFileXHTML(System.Windows.Forms.Application.StartupPath + "/Logs/static_rebuild.html");
                SobekCM.Library.Static_Pages_Builder builder = new SobekCM.Library.Static_Pages_Builder(SobekCM_Library_Settings.Application_Server_URL, SobekCM_Library_Settings.Static_Pages_Location);
                int errors = builder.Rebuild_All_Static_Pages(staticRebuildLog, false, SobekCM_Library_Settings.Local_Log_Directory);
            }
            
            if ( marcrebuild )
            {
                SobekCM.Library.Static_Pages_Builder builder = new SobekCM.Library.Static_Pages_Builder(SobekCM_Library_Settings.Application_Server_URL, SobekCM_Library_Settings.Static_Pages_Location);
                int errors = builder.Rebuild_All_MARC_Files(@"\\cns-uflib-ufdc\UFDC\RESOURCES\" );
            }

            if (build_production_marcxml_feed)
            {
                Create_Complete_MarcXML_Feed(false);
            }

            if (build_test_marcxml_feed)
            {
                Create_Complete_MarcXML_Feed(true);
            }

            // Create the log
            string directory = SobekCM_Library_Settings.Local_Log_Directory;
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            // Run the PRELOADER
            if (run_bulkloader)
            {
                Run_BulkLoader( verbose );
            }
            else
            {
                Console.WriteLine("PreLoader skipped per command line arguments");
            }

            if (checkForAbort())
            {
                return;
            }
        }

        #endregion

        #region Methods to handle checking for abort requests

        private bool checkForAbort()
        {
            if (aborted)
                return true;

            if (Abort_Database_Mechanism.Abort_Requested())
            {
                aborted = true;
                Abort_Database_Mechanism.Builder_Operation_Flag = Builder_Operation_Flag_Enum.ABORTING;
            }
            return aborted;
        }

        #endregion

        #region Method to create the mango/sus feed

        private void Create_Complete_MarcXML_Feed( bool Test_Feed_Flag )
        {
            // Determine some values based on whether this is for thr test feed or something else
            string feed_name = "Production MarcXML Feed";
            string file_name = "complete_marc.xml";
            string error_file_name = "complete_marc_last_error.html";
            if (Test_Feed_Flag)
            {
                feed_name = "Test MarcXML Feed";
                file_name = "test_marc.xml";
                error_file_name = "test_marc_last_error.html";
            }

            // Before doing this, create the Mango load
            try
            {
                // Create the Mango load stuff
                Console.WriteLine("Building " + feed_name);
                MarcXML_Load_Creator createEndeca = new MarcXML_Load_Creator();
                bool reportSuccess = createEndeca.Create_MarcXML_Data_File( Test_Feed_Flag, SobekCM_Library_Settings.Local_Log_Directory + file_name);

                // Publish this feed
                if (reportSuccess)
                {
                    SobekCM.Library.Database.SobekCM_Database.Clear_Item_Error_Log(feed_name.ToUpper(), "", "UFDC Builder");
                    File.Copy(SobekCM_Library_Settings.Local_Log_Directory + file_name, SobekCM_Library_Settings.MarcXML_Feed_Location + file_name, true);
                }
                else
                {
                    string errors = createEndeca.Errors;
                    if (errors.Length > 0)
                    {
                        StreamWriter writer = new StreamWriter(SobekCM_Library_Settings.MarcXML_Feed_Location + error_file_name, false);
                        writer.WriteLine("<html><head><title>" + feed_name + " Errors</title></head><body><h1>" + feed_name + " Errors</h1>");
                        writer.Write(errors.Replace("\r\n","<br />").Replace("\n","<br />").Replace("<br />", "<br />\r\n"));
                        writer.Write("</body></html>");
                        writer.Flush();
                        writer.Close();

                        SobekCM.Library.Database.SobekCM_Database.Add_Item_Error_Log(feed_name.ToUpper(), "", "N/A", "Resulting file failed validation");

                        File.Copy(SobekCM_Library_Settings.Local_Log_Directory + file_name, SobekCM_Library_Settings.MarcXML_Feed_Location + file_name.Replace(".xml", "_error.xml"), true);
                    }
                }
            }
            catch (Exception ee)
            {
                SobekCM.Library.Database.SobekCM_Database.Add_Item_Error_Log(feed_name.ToUpper(), "", "N/A", "Unknown exception caught");
                Console.WriteLine("ERROR BUILDING THE " + feed_name.ToUpper());
            }
        }

        #endregion
    }
}
