#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using SobekCM.Library;
using SobekCM.Library.Configuration;
using SobekCM.Library.Settings;
using SobekCM.Library.Solr;
using SobekCM.Resource_Object.Database;
using SobekCM.Tools.Logs;

#endregion

namespace SobekCM.Builder
{
    /// <summary> Class controls the execution of all tasks, whether being immediately executed
    /// or running continuously in a background thread </summary>
    public class Worker_Controller
    {
        private bool aborted;
        private DateTime controllerStarted;
        private const int BULK_LOADER_END_HOUR = 23;
        private DateTime feedNextBuildTime;
        private readonly bool verbose;

        /// <summary> Constructor for a new instance of the Worker_Controller class </summary>
        /// <param name="Verbose"> Flag indicates if this should be verbose in the log file and console </param>
        public Worker_Controller( bool Verbose )
        {
            verbose = Verbose;
            controllerStarted = DateTime.Now;
            aborted = false;

            // Assign the database connection strings
            SobekCM_Database.Connection_String = SobekCM_Library_Settings.Database_Connections[0].Connection_String;
            Library.Database.SobekCM_Database.Connection_String = SobekCM_Library_Settings.Database_Connections[0].Connection_String;

            // Pull the values from the database and assign other setting values
            SobekCM_Library_Settings.Local_Log_Directory = Application.StartupPath + "\\Logs\\";
            DataSet settings = Library.Database.SobekCM_Database.Get_Builder_Settings_Complete(null);
            if (settings == null)
            {
                Console.WriteLine("FATAL ERROR pulling latest settings from the database: " + Library.Database.SobekCM_Database.Last_Exception.Message);
                return;
            }
            if (!SobekCM_Library_Settings.Refresh(settings))
            {
                Console.WriteLine("Error using database settings to refresh SobekCM_Library_Settings in Worker_Controller constructor");
            }


            // Clear any WEB logs older than XX days old


            // If this starts in an ABORTED mode, set to standard
            Builder_Operation_Flag_Enum operationFlag = Abort_Database_Mechanism.Builder_Operation_Flag;
            if ((operationFlag == Builder_Operation_Flag_Enum.ABORTING) || ( operationFlag == Builder_Operation_Flag_Enum.ABORT_REQUESTED ) || ( operationFlag == Builder_Operation_Flag_Enum.LAST_EXECUTION_ABORTED ))
                Abort_Database_Mechanism.Builder_Operation_Flag = Builder_Operation_Flag_Enum.STANDARD_OPERATION;
        }

        #region Method to execute processes in background

        /// <summary> Continuously execute the processes in a recurring background thread </summary>
        public void Execute_In_Background()
        {
			// Load all the settings
			SobekCM_Library_Settings.Refresh(Library.Database.SobekCM_Database.Get_Settings_Complete(null));


            // Check that this should not be skipped or aborted
            Builder_Operation_Flag_Enum operationFlag = Abort_Database_Mechanism.Builder_Operation_Flag;
            switch (operationFlag)
            {
                case Builder_Operation_Flag_Enum.NO_BUILDING_REQUESTED:
                case Builder_Operation_Flag_Enum.ABORT_REQUESTED:
                case Builder_Operation_Flag_Enum.ABORTING:
                    return;
            }

            // Set the variable which will control background execution
	        int time_between_polls = SobekCM_Library_Settings.Builder_Override_Seconds_Between_Polls;
			if (( time_between_polls < 0 ) || ( SobekCM_Library_Settings.Database_Connections.Count == 1 ))
				time_between_polls = Convert.ToInt32(SobekCM_Library_Settings.Builder_Seconds_Between_Polls);

            // Determine the new log name
            string log_name = "incoming_" + controllerStarted.Year + "_" + controllerStarted.Month.ToString().PadLeft(2, '0') + "_" + controllerStarted.Day.ToString().PadLeft(2, '0') + ".html";
            string local_log_name = SobekCM_Library_Settings.Local_Log_Directory + "\\" + log_name;

            // Create the new log file
            LogFileXHTML preloader_logger = new LogFileXHTML(local_log_name, "SobekCM Incoming Packages Log", "UFDC_Builder.exe", true);

			// Set the time for the next feed building event to 10 minutes from now
			feedNextBuildTime = DateTime.Now.Add(new TimeSpan(0, 10, 0));

			// Build all the bulk loader objects
	        List<Worker_BulkLoader> loaders = new List<Worker_BulkLoader>();
	        bool activeInstanceFound = false;
			foreach (Database_Instance_Configuration dbConfig in SobekCM_Library_Settings.Database_Connections)
			{
				if (!dbConfig.Is_Active)
				{
					loaders.Add(null);
					Console.WriteLine(dbConfig.Name + " is set to INACTIVE");
					preloader_logger.AddNonError(dbConfig.Name + " is set to INACTIVE");
				}
				else
				{
					activeInstanceFound = true;
					SobekCM_Database.Connection_String = dbConfig.Connection_String;
					Library.Database.SobekCM_Database.Connection_String = dbConfig.Connection_String;
					Console.WriteLine(dbConfig.Name + " - Preparing to begin polling");
					preloader_logger.AddNonError(dbConfig.Name + " - Preparing to begin polling");
					Library.Database.SobekCM_Database.Builder_Add_Log_Entry(-1, String.Empty, "Standard", "Preparing to begin polling");

					Worker_BulkLoader newLoader = new Worker_BulkLoader(preloader_logger, verbose, dbConfig.Name);
					loaders.Add(newLoader);
				}
			}

			// If no active instances, just exit
			if (!activeInstanceFound)
			{
				Console.WriteLine("No active databases in the config file");
				preloader_logger.AddError("No active databases in config file... Aborting");
				return;
			}

	        bool firstRun = true;

            // Loop continually until the end hour is achieved
            do
            {
                // Check for abort
                if (CheckForAbort())
                {
                    Abort_Database_Mechanism.Builder_Operation_Flag = Builder_Operation_Flag_Enum.ABORTING;
                    return;
                }

				// Is it time to build any RSS/XML feeds?
	            bool rebuildRssFeeds = false;
				if (DateTime.Compare(DateTime.Now, feedNextBuildTime) >= 0)
				{
					rebuildRssFeeds = true;
					feedNextBuildTime = DateTime.Now.Add(new TimeSpan(0, 10, 0));
				}

				// Step through each instance
				for (int i = 0; i < SobekCM_Library_Settings.Database_Connections.Count; i++)
				{
					if (loaders[i] != null)
					{
						// Get the instance
						Database_Instance_Configuration dbInstance = SobekCM_Library_Settings.Database_Connections[i];

						// Set the database connection strings
						SobekCM_Database.Connection_String = dbInstance.Connection_String;
						Library.Database.SobekCM_Database.Connection_String = dbInstance.Connection_String;

						// Refresh all settings, etc..
						loaders[i].Refresh_Settings_And_Item_List();

						// Pull the abort/pause flag
						Builder_Operation_Flag_Enum currentPauseFlag = Abort_Database_Mechanism.Builder_Operation_Flag;

						// If not paused, run the prebuilder
						if (currentPauseFlag != Builder_Operation_Flag_Enum.PAUSE_REQUESTED)
						{
							if (firstRun)
							{
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

								// Process any pending FDA reports from the FDA Report DropBox
								if (operationFlag != Builder_Operation_Flag_Enum.PAUSE_REQUESTED)
									Process_Any_Pending_FDA_Reports(loaders[i]);

								// CLear the old logs
								Console.WriteLine(dbInstance.Name + " - Expiring old log entries");
								preloader_logger.AddNonError(dbInstance.Name + " - Expiring old log entries");
								Library.Database.SobekCM_Database.Builder_Add_Log_Entry(-1, String.Empty, "Standard", "Expiring old log entries");
								Library.Database.SobekCM_Database.Builder_Expire_Log_Entries(SobekCM_Library_Settings.Builder_Log_Expiration_Days);
							}

							Run_BulkLoader(loaders[i], verbose);

							//if (aborted)
							//{
							//	Abort_Database_Mechanism.Builder_Operation_Flag = Builder_Operation_Flag_Enum.ABORTING;
							//	return;
							//}

							if (rebuildRssFeeds)
								loaders[i].Build_Feeds();
						}
						else
						{
							preloader_logger.AddNonError( dbInstance.Name +  " - Building paused");
						}
					}
				}

                // Sleep for correct number of milliseconds
                Thread.Sleep(1000 * time_between_polls);

	            firstRun = false;
            } while (DateTime.Now.Hour < BULK_LOADER_END_HOUR);

			// Do the final work for all of the different dbInstances
	        for (int i = 0; i < SobekCM_Library_Settings.Database_Connections.Count; i++)
	        {
		        if (loaders[i] != null)
		        {
			        // Get the instance
			        Database_Instance_Configuration dbInstance = SobekCM_Library_Settings.Database_Connections[i];

			        // Set the database flag
			        SobekCM_Database.Connection_String = dbInstance.Connection_String;

			        // Refresh all settings, etc..
			        loaders[i].Refresh_Settings_And_Item_List();

			        // Pull the abort/pause flag
			        Builder_Operation_Flag_Enum currentPauseFlag2 = Abort_Database_Mechanism.Builder_Operation_Flag;

			        // If not paused, run the prebuilder
			        if (currentPauseFlag2 != Builder_Operation_Flag_Enum.PAUSE_REQUESTED)
			        {
						// Initiate the recreation of the links between metadata and collections
						Library.Database.SobekCM_Database.Admin_Update_Cached_Aggregation_Metadata_Links();

			        }
		        }
	        }


			//// Initiate a solr/lucene index optimization since we are done loading for a while
			//if (DateTime.Now.Day % 2 == 0)
			//{
			//	if (SobekCM_Library_Settings.Document_Solr_Index_URL.Length > 0)
			//	{
			//		Console.WriteLine("Initiating Solr/Lucene document index optimization");
			//		Solr_Controller.Optimize_Document_Index(SobekCM_Library_Settings.Document_Solr_Index_URL);
			//	}
			//}
			//else
			//{
			//	if (SobekCM_Library_Settings.Page_Solr_Index_URL.Length > 0)
			//	{
			//		Console.WriteLine("Initiating Solr/Lucene page index optimization");
			//		Solr_Controller.Optimize_Page_Index(SobekCM_Library_Settings.Page_Solr_Index_URL);
			//	}
			//}
			//// Sleep for twenty minutes to end this (the index rebuild might take some time)
            //Thread.Sleep(1000 * 20 * 60);
        }


        private void Process_Any_Pending_FDA_Reports( Worker_BulkLoader Prebuilder )
        {

            try
            {
                Prebuilder.Process_Any_Pending_FDA_Reports();

                if (Prebuilder.Aborted)
                    aborted = true;
            }
            catch 
            {

            }
        }

        private void Run_BulkLoader(Worker_BulkLoader Prebuilder, bool Verbose)
        {
            try
            {
                Prebuilder.Perform_BulkLoader( Verbose );

                // Save information about this last run
                Library.Database.SobekCM_Database.Set_Setting("Builder Version", SobekCM_Library_Settings.CURRENT_BUILDER_VERSION);
                Library.Database.SobekCM_Database.Set_Setting("Builder Last Run Finished", DateTime.Now.ToString());
                Library.Database.SobekCM_Database.Set_Setting("Builder Last Message", Prebuilder.Final_Message);

                if (Prebuilder.Aborted)
                    aborted = true;
            }
            catch
            {

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

            // Determine the new log name
            string log_name = "incoming_" + controllerStarted.Year + "_" + controllerStarted.Month.ToString().PadLeft(2, '0') + "_" + controllerStarted.Day.ToString().PadLeft(2, '0') + ".html";
            string local_log_name = SobekCM_Library_Settings.Local_Log_Directory + "\\" + log_name;

            // Create the new log file
            LogFileXHTML preloader_logger = new LogFileXHTML(local_log_name, "SobekCM Incoming Packages Log", "UFDC_Builder.exe", true);

			// Step through each database instance
	        foreach (Database_Instance_Configuration dbConfig in SobekCM_Library_Settings.Database_Connections)
	        {
		        try
		        {
			        if (!dbConfig.Is_Active)
			        {
				        Console.WriteLine( dbConfig.Name + " is set to INACTIVE");
				        preloader_logger.AddNonError(dbConfig.Name + " is set to INACTIVE");
			        }
			        else
			        {
				        SobekCM_Database.Connection_String = dbConfig.Connection_String;
				        Worker_BulkLoader newLoader = new Worker_BulkLoader(preloader_logger, verbose, dbConfig.Name);
						newLoader.Perform_BulkLoader(Verbose);

						// Save information about this last run
						Library.Database.SobekCM_Database.Set_Setting("Builder Version", SobekCM_Library_Settings.CURRENT_BUILDER_VERSION);
						Library.Database.SobekCM_Database.Set_Setting("Builder Last Run Finished", DateTime.Now.ToString());
						Library.Database.SobekCM_Database.Set_Setting("Builder Last Message", newLoader.Final_Message);
			        }
		        }
		        catch
		        {

		        }
	        }
        }

         #endregion

        #region Method to immediately execute all requested processes

	    /// <summary> Immediately perform all requested tasks </summary>
	    /// <param name="BuildProductionMarcxmlFeed"> Flag indicates if the MarcXML feed for OPACs should be produced </param>
	    /// <param name="BuildTestMarcxmlFeed"> Flag indicates if the items set to be put in a TEST feed should have their MarcXML feed produced</param>
	    /// <param name="RunBulkloader"> Flag indicates if the preload </param>
	    /// <param name="CompleteStaticRebuild"> Flag indicates whether to rebuild all the item static pages </param>
	    /// <param name="MarcRebuild"> Flag indicates if all the MarcXML files for each resource should be rewritten from the METS/MODS metadata files </param>
	    public void Execute_Immediately(bool BuildProductionMarcxmlFeed, bool BuildTestMarcxmlFeed, bool RunBulkloader, bool CompleteStaticRebuild, bool MarcRebuild )
        {
            if (CompleteStaticRebuild)
            {
                LogFileXHTML staticRebuildLog = new LogFileXHTML(Application.StartupPath + "/Logs/static_rebuild.html");
                Static_Pages_Builder builder = new Static_Pages_Builder(SobekCM_Library_Settings.Application_Server_URL, SobekCM_Library_Settings.Static_Pages_Location);
                builder.Rebuild_All_Static_Pages(staticRebuildLog, false, SobekCM_Library_Settings.Local_Log_Directory);
            }
            
            if ( MarcRebuild )
            {
                Static_Pages_Builder builder = new Static_Pages_Builder(SobekCM_Library_Settings.Application_Server_URL, SobekCM_Library_Settings.Static_Pages_Location);
                builder.Rebuild_All_MARC_Files( SobekCM_Library_Settings.Image_Server_Network );
            }

            if (BuildProductionMarcxmlFeed)
            {
                Create_Complete_MarcXML_Feed(false);
            }

            if (BuildTestMarcxmlFeed)
            {
                Create_Complete_MarcXML_Feed(true);
            }

            // Create the log
            string directory = SobekCM_Library_Settings.Local_Log_Directory;
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            // Run the PRELOADER
            if (RunBulkloader)
            {
                Run_BulkLoader( verbose );
            }
            else
            {
                Console.WriteLine("PreLoader skipped per command line arguments");
            }
        }

        #endregion

        #region Methods to handle checking for abort requests

        private bool CheckForAbort()
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
                    Library.Database.SobekCM_Database.Builder_Clear_Item_Error_Log(feed_name.ToUpper(), "", "UFDC Builder");
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

                        Library.Database.SobekCM_Database.Builder_Add_Item_Error_Log(feed_name.ToUpper(), "", "N/A", "Resulting file failed validation");

                        File.Copy(SobekCM_Library_Settings.Local_Log_Directory + file_name, SobekCM_Library_Settings.MarcXML_Feed_Location + file_name.Replace(".xml", "_error.xml"), true);
                    }
                }
            }
            catch 
            {
                Library.Database.SobekCM_Database.Builder_Add_Item_Error_Log(feed_name.ToUpper(), "", "N/A", "Unknown exception caught");
                Console.WriteLine("ERROR BUILDING THE " + feed_name.ToUpper());
            }
        }

        #endregion
    }
}
