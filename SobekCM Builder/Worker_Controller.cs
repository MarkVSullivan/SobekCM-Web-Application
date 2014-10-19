#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using SobekCM.Core.Configuration;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Engine_Library.Database;
using SobekCM.Library;
using SobekCM.Resource_Object.Database;
using SobekCM.Tools.Logs;
using SobekCM.Builder_Library;

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

        private string imageMagickProgram;
        private string ghostscriptProgram;
        private List<Database_Instance_Configuration> instances;
        private List<Worker_BulkLoader> loaders;

        /// <summary> Constructor for a new instance of the Worker_Controller class </summary>
        /// <param name="Verbose"> Flag indicates if this should be verbose in the log file and console </param>
        public Worker_Controller( bool Verbose )
        {
            verbose = Verbose;
            controllerStarted = DateTime.Now;
            aborted = false;

            // Assign the database connection strings
            SobekCM_Database.Connection_String = Engine_ApplicationCache_Gateway.Settings.Database_Connections[0].Connection_String;
            Library.Database.SobekCM_Database.Connection_String = Engine_ApplicationCache_Gateway.Settings.Database_Connections[0].Connection_String;

            // Save the list of instances
            instances = new List<Database_Instance_Configuration>();
            foreach (Database_Instance_Configuration dbInfo in Engine_ApplicationCache_Gateway.Settings.Database_Connections)
            {
                instances.Add(dbInfo);
            }

            // Pull the values from the database and assign other setting values
            Engine_ApplicationCache_Gateway.Settings.Local_Log_Directory = Application.StartupPath + "\\Logs\\";
            DataSet settings = Engine_Database.Get_Settings_Complete(null);
            if (settings == null)
            {
                Console.WriteLine("FATAL ERROR pulling latest settings from the database: " + Library.Database.SobekCM_Database.Last_Exception.Message);
                return;
            }
            if (!Engine_ApplicationCache_Gateway.RefreshSettings())
            {
                Console.WriteLine("Error using database settings to refresh SobekCM_Library_Settings in Worker_Controller constructor");
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
			// Load all the settings
            Engine_ApplicationCache_Gateway.RefreshSettings();

            // Set the variable which will control background execution
	        int time_between_polls = Engine_ApplicationCache_Gateway.Settings.Builder_Override_Seconds_Between_Polls;
			if (( time_between_polls < 0 ) || ( Engine_ApplicationCache_Gateway.Settings.Database_Connections.Count == 1 ))
				time_between_polls = Convert.ToInt32(Engine_ApplicationCache_Gateway.Settings.Builder_Seconds_Between_Polls);

            // Determine the new log name
            string log_name = "incoming_" + controllerStarted.Year + "_" + controllerStarted.Month.ToString().PadLeft(2, '0') + "_" + controllerStarted.Day.ToString().PadLeft(2, '0') + ".html";
            string local_log_name = Engine_ApplicationCache_Gateway.Settings.Local_Log_Directory + "\\" + log_name;

            // Create the new log file
            LogFileXHTML preloader_logger = new LogFileXHTML(local_log_name, "SobekCM Incoming Packages Log", "UFDC_Builder.exe", true);

            // start with warnings on imagemagick and ghostscript not being installed
            if ((String.IsNullOrEmpty(Engine_ApplicationCache_Gateway.Settings.ImageMagick_Executable)) || (!File.Exists(Engine_ApplicationCache_Gateway.Settings.ImageMagick_Executable)))
            {
                string possible_imagemagick = Look_For_Variable_Registry_Key("SOFTWARE\\ImageMagick", "BinPath");
                if ((!String.IsNullOrEmpty(possible_imagemagick)) && ( Directory.Exists(possible_imagemagick)) && ( File.Exists( Path.Combine(possible_imagemagick, "convert.exe"))))
                {
                    imageMagickProgram = Path.Combine(possible_imagemagick, "convert.exe");
                }
                else
                {
                    Console.WriteLine("WARNING: Could not find ImageMagick installed.  Some image processing will be unavailable.");
                    preloader_logger.AddNonError("WARNING: Could not find ImageMagick installed.  Some image processing will be unavailable.");
                }


            }
            else
            {
                imageMagickProgram = Engine_ApplicationCache_Gateway.Settings.ImageMagick_Executable;
            }

            if ((String.IsNullOrEmpty(Engine_ApplicationCache_Gateway.Settings.Ghostscript_Executable)) || (!File.Exists(Engine_ApplicationCache_Gateway.Settings.Ghostscript_Executable)))
            {
                string possible_ghost = Look_For_Variable_Registry_Key("SOFTWARE\\GPL Ghostscript", "GS_DLL");
                if (!String.IsNullOrEmpty(possible_ghost))
                {
                    string gsPath = Path.GetDirectoryName( possible_ghost );
                    if ((Directory.Exists(gsPath)) && ((File.Exists(Path.Combine(gsPath, "gswin32c.exe"))) || (File.Exists(Path.Combine(gsPath, "gswin64c.exe")))))
                    {
                        if (File.Exists(Path.Combine(gsPath, "gswin64c.exe")))
                            ghostscriptProgram = Path.Combine(gsPath, "gswin64c.exe");
                        else
                            ghostscriptProgram = Path.Combine(gsPath, "gswin32c.exe");
                    }
                    else
                    {
                        Console.WriteLine("WARNING: Could not find GhostScript installed.  Some PDF processing will be unavailable.");
                        preloader_logger.AddNonError("WARNING: Could not find GhostScript installed.  Some PDF processing will be unavailable.");
                    }
                }
                else
                {
                    Console.WriteLine("WARNING: Could not find GhostScript installed.  Some PDF processing will be unavailable.");
                    preloader_logger.AddNonError("WARNING: Could not find GhostScript installed.  Some PDF processing will be unavailable.");
                }
            }
            else
            {
                ghostscriptProgram = Engine_ApplicationCache_Gateway.Settings.Ghostscript_Executable;
            }

			// Set the time for the next feed building event to 10 minutes from now
			feedNextBuildTime = DateTime.Now.Add(new TimeSpan(0, 10, 0));

			// First, step through each active configuration and see if building is currently aborted 
			// while doing very minimal processes
			aborted = false;
			Console.WriteLine("Checking for initial abort condition");
			preloader_logger.AddNonError("Checking for initial abort condition");
	        string abort_message = String.Empty;
			Builder_Operation_Flag_Enum abort_flag = Builder_Operation_Flag_Enum.STANDARD_OPERATION;
	        foreach (Database_Instance_Configuration dbConfig in instances)
	        {
		        if ((!aborted) && (dbConfig.Is_Active) && (dbConfig.Can_Abort))
		        {
			        Resource_Object.Database.SobekCM_Database.Connection_String = dbConfig.Connection_String;
			        Library.Database.SobekCM_Database.Connection_String = dbConfig.Connection_String;

			        // Check that this should not be skipped or aborted
			        Builder_Operation_Flag_Enum operationFlag = Abort_Database_Mechanism.Builder_Operation_Flag;
			        switch (operationFlag)
			        {
				        case Builder_Operation_Flag_Enum.ABORT_REQUESTED:
				        case Builder_Operation_Flag_Enum.ABORTING:
					        abort_message = "PREVIOUS ABORT flag found in " + dbConfig.Name;
							abort_flag = Builder_Operation_Flag_Enum.LAST_EXECUTION_ABORTED;
							Console.WriteLine(abort_message);
							preloader_logger.AddNonError(abort_message);
					        aborted = true;
							Abort_Database_Mechanism.Builder_Operation_Flag = Builder_Operation_Flag_Enum.LAST_EXECUTION_ABORTED;
					        break;

						case Builder_Operation_Flag_Enum.NO_BUILDING_REQUESTED:
					        abort_message = "PREVIOUS NO BUILDING flag found in " + dbConfig.Name;
							Console.WriteLine(abort_message);
							preloader_logger.AddNonError(abort_message);
							aborted = true;
							break;

			        }
		        }
	        }

			// If initially aborted, step through each instance and set a message
			if (aborted)
			{
				// Add messages in each active instance
				foreach (Database_Instance_Configuration dbConfig in instances)
				{
					if (dbConfig.Is_Active) 
					{
						Console.WriteLine("Setting previous abort flag message in " + dbConfig.Name);
						preloader_logger.AddNonError("Setting previous abort flag message in " + dbConfig.Name);
						SobekCM_Database.Connection_String = dbConfig.Connection_String;
						Library.Database.SobekCM_Database.Connection_String = dbConfig.Connection_String;
						Library.Database.SobekCM_Database.Builder_Add_Log_Entry(-1, String.Empty, "Standard", abort_message, String.Empty);

						// Save information about this last run
                        Library.Database.SobekCM_Database.Set_Setting("Builder Version", Engine_ApplicationCache_Gateway.Settings.Current_Builder_Version);
						Library.Database.SobekCM_Database.Set_Setting("Builder Last Run Finished", DateTime.Now.ToString());
						Library.Database.SobekCM_Database.Set_Setting("Builder Last Message", abort_message);

						// Finally, set the builder flag appropriately
						if (abort_flag != Builder_Operation_Flag_Enum.STANDARD_OPERATION)
							Abort_Database_Mechanism.Builder_Operation_Flag = abort_flag;
					}
				}

				// Do nothing else
				return;
			}

	        // Build all the bulk loader objects
	        List<Worker_BulkLoader> loaders = new List<Worker_BulkLoader>();
	        bool activeInstanceFound = false;
			foreach (Database_Instance_Configuration dbConfig in instances)
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


                    // At this point warn on mossing the Ghostscript and ImageMagick
                    if ( String.IsNullOrEmpty(imageMagickProgram))
                    {
                        Library.Database.SobekCM_Database.Builder_Add_Log_Entry(-1, String.Empty, "Standard", "WARNING: Could not find ImageMagick installed.  Some image processing will be unavailable.", String.Empty);
                    }
                    if ( String.IsNullOrEmpty(ghostscriptProgram))
                    {
                        Library.Database.SobekCM_Database.Builder_Add_Log_Entry(-1, String.Empty, "Standard", "WARNING: Could not find GhostScript installed.  Some PDF processing will be unavailable.", String.Empty);
                    }

					Console.WriteLine(dbConfig.Name + " - Preparing to begin polling");
					preloader_logger.AddNonError(dbConfig.Name + " - Preparing to begin polling");
					Library.Database.SobekCM_Database.Builder_Add_Log_Entry(-1, String.Empty, "Standard", "Preparing to begin polling", String.Empty);

					Worker_BulkLoader newLoader = new Worker_BulkLoader(preloader_logger, verbose, dbConfig, (instances.Count > 1 ), imageMagickProgram, ghostscriptProgram );
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
				// Is it time to build any RSS/XML feeds?
	            bool rebuildRssFeeds = false;
				if (DateTime.Compare(DateTime.Now, feedNextBuildTime) >= 0)
				{
					rebuildRssFeeds = true;
					feedNextBuildTime = DateTime.Now.Add(new TimeSpan(0, 10, 0));
				}

				// Step through each instance
				for (int i = 0; i < instances.Count; i++)
				{
					if (loaders[i] != null)
					{
						// Get the instance
						Database_Instance_Configuration dbInstance = instances[i];

						// Set the database connection strings
						SobekCM_Database.Connection_String = dbInstance.Connection_String;
						Library.Database.SobekCM_Database.Connection_String = dbInstance.Connection_String;

						// Look for abort
						if ((dbInstance.Can_Abort) && (CheckForAbort()))
						{
							aborted = true;
							if (Abort_Database_Mechanism.Builder_Operation_Flag != Builder_Operation_Flag_Enum.NO_BUILDING_REQUESTED)
							{
								abort_flag = Builder_Operation_Flag_Enum.LAST_EXECUTION_ABORTED;
								Abort_Database_Mechanism.Builder_Operation_Flag = Builder_Operation_Flag_Enum.ABORTING;
							}
							break;
						}



						// Refresh all settings, etc..
						loaders[i].Refresh_Settings_And_Item_List();

						// Pull the abort/pause flag
						Builder_Operation_Flag_Enum currentPauseFlag = Abort_Database_Mechanism.Builder_Operation_Flag;

						// If not paused, run the prebuilder
						if (currentPauseFlag != Builder_Operation_Flag_Enum.PAUSE_REQUESTED)
						{
							if (firstRun)
							{

								//    // Always build an endeca feed first (so it occurs once a day)
								//    if (Engine_ApplicationCache_Gateway.Settings.Build_MARC_Feed_By_Default)
								//    {
								//        Create_Complete_MarcXML_Feed(false);
								//    }
								//}
								
								// CLear the old logs
								Console.WriteLine(dbInstance.Name + " - Expiring old log entries");
								preloader_logger.AddNonError(dbInstance.Name + " - Expiring old log entries");
								Library.Database.SobekCM_Database.Builder_Add_Log_Entry(-1, String.Empty, "Standard", "Expiring old log entries", String.Empty);
								Library.Database.SobekCM_Database.Builder_Expire_Log_Entries(Engine_ApplicationCache_Gateway.Settings.Builder_Log_Expiration_Days);



								// Rebuild all the static pages
								Console.WriteLine(dbInstance.Name + " - Rebuilding all static pages");
								preloader_logger.AddNonError(dbInstance.Name + " - Rebuilding all static pages");
								long staticRebuildLogId = Library.Database.SobekCM_Database.Builder_Add_Log_Entry(-1, String.Empty, "Standard", "Rebuilding all static pages", String.Empty);

								Static_Pages_Builder builder = new Static_Pages_Builder(Engine_ApplicationCache_Gateway.Settings.Application_Server_URL, Engine_ApplicationCache_Gateway.Settings.Static_Pages_Location, Engine_ApplicationCache_Gateway.Settings.Application_Server_Network);
								builder.Rebuild_All_Static_Pages(preloader_logger, false, Engine_ApplicationCache_Gateway.Settings.Local_Log_Directory, dbInstance.Name, staticRebuildLogId);

							}

							Run_BulkLoader(loaders[i], verbose);

							// Look for abort
							if ((!aborted) && (dbInstance.Can_Abort) && (CheckForAbort()))
							{
								aborted = true;
								if (Abort_Database_Mechanism.Builder_Operation_Flag != Builder_Operation_Flag_Enum.NO_BUILDING_REQUESTED)
								{
									abort_flag = Builder_Operation_Flag_Enum.LAST_EXECUTION_ABORTED;
									Abort_Database_Mechanism.Builder_Operation_Flag = Builder_Operation_Flag_Enum.ABORTING;
								}
								break;
							}
						}
						else
						{
							preloader_logger.AddNonError( dbInstance.Name +  " - Building paused");
							Library.Database.SobekCM_Database.Builder_Add_Log_Entry(-1, String.Empty, "Standard", "Building temporarily PAUSED", String.Empty);
						}
					}
				}

	            if (aborted)
		            break;


				// No longer the first run
				firstRun = false;

				// Publish the log
	            publish_log_file(local_log_name);

                // Sleep for correct number of milliseconds
                Thread.Sleep(1000 * time_between_polls);


            } while (DateTime.Now.Hour < BULK_LOADER_END_HOUR);

			// Do the final work for all of the different dbInstances
	        if (!aborted)
	        {
		        for (int i = 0; i < instances.Count; i++)
		        {
			        if (loaders[i] != null)
			        {
				        // Get the instance
				        Database_Instance_Configuration dbInstance = instances[i];

				        // Set the database flag
				        Resource_Object.Database.SobekCM_Database.Connection_String = dbInstance.Connection_String;
			            Library.Database.SobekCM_Database.Connection_String = dbInstance.Connection_String;

				        // Pull the abort/pause flag
				        Builder_Operation_Flag_Enum currentPauseFlag2 = Abort_Database_Mechanism.Builder_Operation_Flag;

				        // If not paused, run the prebuilder
				        if (currentPauseFlag2 != Builder_Operation_Flag_Enum.PAUSE_REQUESTED)
				        {
					        // Initiate the recreation of the links between metadata and collections
					        Library.Database.SobekCM_Database.Admin_Update_Cached_Aggregation_Metadata_Links();
				        }

				        // Clear the memory
				        loaders[i].ReleaseResources();
			        }
		        }
	        }
	        else
	        {
		        // Mark the aborted in each instance
		        foreach (Database_Instance_Configuration dbConfig in instances )
		        {
					if (dbConfig.Is_Active)
					{
						Console.WriteLine("Setting abort flag message in " + dbConfig.Name);
						preloader_logger.AddNonError("Setting abort flag message in " + dbConfig.Name);
						Resource_Object.Database.SobekCM_Database.Connection_String = dbConfig.Connection_String;
						Library.Database.SobekCM_Database.Connection_String = dbConfig.Connection_String;
						Library.Database.SobekCM_Database.Builder_Add_Log_Entry(-1, String.Empty, "Standard", "Building ABORTED per request from database key", String.Empty);

						// Save information about this last run
                        Library.Database.SobekCM_Database.Set_Setting("Builder Version", Engine_ApplicationCache_Gateway.Settings.Current_Builder_Version);
						Library.Database.SobekCM_Database.Set_Setting("Builder Last Run Finished", DateTime.Now.ToString());
						Library.Database.SobekCM_Database.Set_Setting("Builder Last Message", "Building ABORTED per request");

						// Finally, set the builder flag appropriately
						if ( abort_flag == Builder_Operation_Flag_Enum.LAST_EXECUTION_ABORTED )
							Abort_Database_Mechanism.Builder_Operation_Flag = Builder_Operation_Flag_Enum.LAST_EXECUTION_ABORTED;
					}
		        }
	        }


	        // Publish the log
			publish_log_file(local_log_name);


			//// Initiate a solr/lucene index optimization since we are done loading for a while
			//if (DateTime.Now.Day % 2 == 0)
			//{
			//	if (Engine_ApplicationCache_Gateway.Settings.Document_Solr_Index_URL.Length > 0)
			//	{
			//		Console.WriteLine("Initiating Solr/Lucene document index optimization");
			//		Solr_Controller.Optimize_Document_Index(Engine_ApplicationCache_Gateway.Settings.Document_Solr_Index_URL);
			//	}
			//}
			//else
			//{
			//	if (Engine_ApplicationCache_Gateway.Settings.Page_Solr_Index_URL.Length > 0)
			//	{
			//		Console.WriteLine("Initiating Solr/Lucene page index optimization");
			//		Solr_Controller.Optimize_Page_Index(Engine_ApplicationCache_Gateway.Settings.Page_Solr_Index_URL);
			//	}
			//}
			//// Sleep for twenty minutes to end this (the index rebuild might take some time)
            //Thread.Sleep(1000 * 20 * 60);
        }

		private void publish_log_file(string LocalLogName)
		{
			try
			{
				if ((Engine_ApplicationCache_Gateway.Settings.Builder_Logs_Publish_Directory.Length > 0) && (Directory.Exists(Engine_ApplicationCache_Gateway.Settings.Builder_Logs_Publish_Directory)))
				{
					if ( File.Exists(LocalLogName))
						File.Copy(LocalLogName, Engine_ApplicationCache_Gateway.Settings.Builder_Logs_Publish_Directory + "\\" + Path.GetFileName(LocalLogName), true );
				}
			}
			catch
			{
				// Not critical error
			}
		}

        private void Run_BulkLoader(Worker_BulkLoader Prebuilder, bool Verbose)
        {
            try
            {
                Prebuilder.Perform_BulkLoader( Verbose );

                // Save information about this last run
                Library.Database.SobekCM_Database.Set_Setting("Builder Version", Engine_ApplicationCache_Gateway.Settings.Current_Builder_Version);
                Library.Database.SobekCM_Database.Set_Setting("Builder Last Run Finished", DateTime.Now.ToString());
                Library.Database.SobekCM_Database.Set_Setting("Builder Last Message", Prebuilder.Final_Message);

                if (Prebuilder.Aborted)
                    aborted = true;
            }
            catch ( Exception ee)
            {

            }
        }
  
        private void Run_BulkLoader( bool Verbose )
        {
            // Create the local log directories
            if (!Directory.Exists(Engine_ApplicationCache_Gateway.Settings.Local_Log_Directory))
            {
                Console.WriteLine("Creating local log directory: " + Engine_ApplicationCache_Gateway.Settings.Local_Log_Directory);
                Directory.CreateDirectory(Engine_ApplicationCache_Gateway.Settings.Local_Log_Directory);
            }

            // Determine the new log name
            string log_name = "incoming_" + controllerStarted.Year + "_" + controllerStarted.Month.ToString().PadLeft(2, '0') + "_" + controllerStarted.Day.ToString().PadLeft(2, '0') + ".html";
            string local_log_name = Engine_ApplicationCache_Gateway.Settings.Local_Log_Directory + "\\" + log_name;

            // Create the new log file
            LogFileXHTML preloader_logger = new LogFileXHTML(local_log_name, "SobekCM Incoming Packages Log", "UFDC_Builder.exe", true);

			// Step through each database instance
	        foreach (Database_Instance_Configuration dbConfig in Engine_ApplicationCache_Gateway.Settings.Database_Connections)
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
				        SobekCM.Resource_Object.Database.SobekCM_Database.Connection_String = dbConfig.Connection_String;
			            SobekCM.Library.Database.SobekCM_Database.Connection_String = dbConfig.Connection_String;
                        Worker_BulkLoader newLoader = new Worker_BulkLoader(preloader_logger, verbose, dbConfig, (instances.Count > 1), imageMagickProgram, ghostscriptProgram);
						newLoader.Perform_BulkLoader(Verbose);

						// Save information about this last run
                        Library.Database.SobekCM_Database.Set_Setting("Builder Version", Engine_ApplicationCache_Gateway.Settings.Current_Builder_Version);
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
            // start with warnings on imagemagick and ghostscript not being installed
            if (Engine_ApplicationCache_Gateway.Settings.ImageMagick_Executable.Length == 0)
            {
                Console.WriteLine("WARNING: Could not find ImageMagick installed.  Some image processing will be unavailable.");
            }
            if (Engine_ApplicationCache_Gateway.Settings.Ghostscript_Executable.Length == 0)
            {
                Console.WriteLine("WARNING: Could not find GhostScript installed.  Some PDF processing will be unavailable.");
            }

            if (CompleteStaticRebuild)
            {
				Console.WriteLine("Beginning static rebuild");
                LogFileXHTML staticRebuildLog = new LogFileXHTML(Application.StartupPath + "/Logs/static_rebuild.html");
				Static_Pages_Builder builder = new Static_Pages_Builder(Engine_ApplicationCache_Gateway.Settings.Application_Server_URL, Engine_ApplicationCache_Gateway.Settings.Static_Pages_Location, Engine_ApplicationCache_Gateway.Settings.Application_Server_Network);
                builder.Rebuild_All_Static_Pages(staticRebuildLog, true, Engine_ApplicationCache_Gateway.Settings.Local_Log_Directory, String.Empty, -1);
            }
            
            if ( MarcRebuild )
            {
				Static_Pages_Builder builder = new Static_Pages_Builder(Engine_ApplicationCache_Gateway.Settings.Application_Server_URL, Engine_ApplicationCache_Gateway.Settings.Static_Pages_Location, Engine_ApplicationCache_Gateway.Settings.Application_Server_Network);
                builder.Rebuild_All_MARC_Files( Engine_ApplicationCache_Gateway.Settings.Image_Server_Network );
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
            string directory = Engine_ApplicationCache_Gateway.Settings.Local_Log_Directory;
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
                bool reportSuccess = createEndeca.Create_MarcXML_Data_File( Test_Feed_Flag, Engine_ApplicationCache_Gateway.Settings.Local_Log_Directory + file_name);

                // Publish this feed
                if (reportSuccess)
                {
                    Library.Database.SobekCM_Database.Builder_Clear_Item_Error_Log(feed_name.ToUpper(), "", "UFDC Builder");
                    File.Copy(Engine_ApplicationCache_Gateway.Settings.Local_Log_Directory + file_name, Engine_ApplicationCache_Gateway.Settings.MarcXML_Feed_Location + file_name, true);
                }
                else
                {
                    string errors = createEndeca.Errors;
                    if (errors.Length > 0)
                    {
                        StreamWriter writer = new StreamWriter(Engine_ApplicationCache_Gateway.Settings.MarcXML_Feed_Location + error_file_name, false);
                        writer.WriteLine("<html><head><title>" + feed_name + " Errors</title></head><body><h1>" + feed_name + " Errors</h1>");
                        writer.Write(errors.Replace("\r\n","<br />").Replace("\n","<br />").Replace("<br />", "<br />\r\n"));
                        writer.Write("</body></html>");
                        writer.Flush();
                        writer.Close();

                        Library.Database.SobekCM_Database.Builder_Add_Log_Entry(-1, feed_name.ToUpper(), "Error", "Resulting file failed validation", "");

                        File.Copy(Engine_ApplicationCache_Gateway.Settings.Local_Log_Directory + file_name, Engine_ApplicationCache_Gateway.Settings.MarcXML_Feed_Location + file_name.Replace(".xml", "_error.xml"), true);
                    }
                }
            }
            catch 
            {
				Library.Database.SobekCM_Database.Builder_Add_Log_Entry(-1, feed_name.ToUpper(), "Error", "Unknown exception caught", "");

                Console.WriteLine("ERROR BUILDING THE " + feed_name.ToUpper());
            }
        }

        #endregion


        #region Code to read registry values

        private static string Look_For_Variable_Registry_Key(string Manufacturer, string KeyName)
        {
            RegistryKey localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            localKey = localKey.OpenSubKey(Manufacturer);
            if (localKey != null)
            {
                string[] subkeys = localKey.GetSubKeyNames();
                foreach (string thisSubKey in subkeys)
                {
                    RegistryKey subKey = localKey.OpenSubKey(thisSubKey);
                    string value64 = subKey.GetValue(KeyName) as string;
                    if (!String.IsNullOrEmpty(value64))
                        return value64;
                }
            }
            RegistryKey localKey32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            localKey32 = localKey32.OpenSubKey(Manufacturer);
            if (localKey32 != null)
            {
                string[] subkeys = localKey32.GetSubKeyNames();
                foreach (string thisSubKey in subkeys)
                {
                    RegistryKey subKey = localKey32.OpenSubKey(thisSubKey);
                    string value32 = subKey.GetValue(KeyName) as string;
                    if (!String.IsNullOrEmpty(value32))
                        return value32;
                }
            }
            return null;
        }

        private static string Get_Registry_Value(string KeyPath, string KeyName)
        {
            RegistryKey localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            localKey = localKey.OpenSubKey(KeyPath);
            if (localKey != null)
            {
                string tomcat6_value64 = localKey.GetValue(KeyName) as string;
                if (tomcat6_value64 != null)
                {
                    return tomcat6_value64;
                }
            }
            RegistryKey localKey32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            localKey32 = localKey32.OpenSubKey(KeyPath);
            if (localKey32 != null)
            {
                string tomcat6_value32 = localKey32.GetValue(KeyName) as string;
                if (tomcat6_value32 != null)
                {
                    return tomcat6_value32;
                }
            }

            return null;
        }

        #endregion

    }
}
