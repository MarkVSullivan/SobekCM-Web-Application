#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using SobekCM.Builder_Library.Modules;
using SobekCM.Builder_Library.Modules.Folders;
using SobekCM.Builder_Library.Modules.Items;
using SobekCM.Builder_Library.Modules.PostProcess;
using SobekCM.Builder_Library.Modules.PreProcess;
using SobekCM.Builder_Library.Modules.Schedulable;
using SobekCM.Builder_Library.Settings;
using SobekCM.Core.Builder;
using SobekCM.Core.Client;
using SobekCM.Core.Configuration.Extensions;
using SobekCM.Core.MemoryMgmt;
using SobekCM.Core.MicroservicesClient;
using SobekCM.Core.Settings;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Engine_Library.Database;
using SobekCM.Engine_Library.Settings;
using SobekCM.Resource_Object.Configuration;
using SobekCM.Tools;
using SobekCM.Tools.Logs;
using SobekCM_Resource_Database;

#endregion

namespace SobekCM.Builder_Library
{
    /// <summary> Class is the worker thread for the main bulk loader processor </summary>
    public class Worker_BulkLoader
    {

        private Builder_Settings builderSettings;
        private Builder_Modules builderModules;
        private readonly Single_Instance_Configuration instanceInfo;
        private InstanceWide_Settings settings;


        private readonly LogFileXhtml logger;
        private readonly string logFileDirectory;
        private readonly string pluginRootDirectory;
        
        private bool aborted;
        private bool verbose;
        private bool refreshed;
        private bool firstrun;
        
	    private readonly bool multiInstanceBuilder;

	    private string finalmessage;
        
        private readonly List<string> aggregationsToRefresh;
        private readonly List<BibVidStruct> processedItems;
        private readonly List<BibVidStruct> deletedItems;

        private readonly int newItemLimit;
        private bool stillPendingItems;

        private bool noFoldersReported;




        /// <summary> Constructor for a new instance of the Worker_BulkLoader class </summary>
        /// <param name="Logger"> Log file object for logging progress </param>
        /// <param name="Verbose"> Flag indicates if the builder is in verbose mode, where it should log alot more information </param>
        /// <param name="InstanceInfo"> Information for the instance of SobekCM to be processed by this bulk loader </param>
        /// <param name="LogFileDirectory"> Directory where any log files would be written </param>
        /// <param name="PluginRootDirectory"> Root directory where all the plug-ins are stored locally for the builder </param>
        public Worker_BulkLoader(LogFileXhtml Logger, Single_Instance_Configuration InstanceInfo, bool Verbose, string LogFileDirectory, string PluginRootDirectory )
        {
            // Save the log file and verbose flag
            logger = Logger;
            verbose = Verbose;
	        multiInstanceBuilder = ( MultiInstance_Builder_Settings.Instances.Count > 1);
            logFileDirectory = LogFileDirectory;
            pluginRootDirectory = PluginRootDirectory;
            instanceInfo = InstanceInfo;

            // If this is processing multiple instances, limit the numbe of packages that should be processed
            // before allowing the builder to move to the next instance and poll
	        if (multiInstanceBuilder)
	            newItemLimit = 100;
	        else
	            newItemLimit = -1;
 
            Add_NonError_To_Log("Worker_BulkLoader.Constructor: Start", verbose, String.Empty, String.Empty, -1);

            // Create new list of collections to build
            aggregationsToRefresh = new List<string>();
	        processedItems = new List<BibVidStruct>();
	        deletedItems = new List<BibVidStruct>();

            // Set some defaults
            aborted = false;
            refreshed = false;
            noFoldersReported = false;
            firstrun = true;

            Add_NonError_To_Log("Worker_BulkLoader.Constructor: Done", verbose, String.Empty, String.Empty, -1);
        }

        #region Main Method that steps through each package and performs work

        /// <summary> Performs the bulk loader process and handles any incoming digital resources </summary>
        /// <returns> TRUE if there are still pending items to be processed, otherwise FALSE </returns>
        public bool Perform_BulkLoader( bool Verbose )
        {

            // Run the usage stats module first
            //CalculateUsageStatisticsModule statsModule2 = new CalculateUsageStatisticsModule();
            //statsModule2.Process += module_Process;
            //statsModule2.Error += module_Error;
            //statsModule2.DoWork(settings);

            verbose = Verbose;
            finalmessage = String.Empty;
            stillPendingItems = false;

            Add_NonError_To_Log("Starting to perform bulk load", verbose, String.Empty, String.Empty, -1);

            // Refresh any settings and item lists 
            if (!Refresh_Settings_And_Item_List())
            {
                Add_Error_To_Log("Error refreshing settings and item list", String.Empty, String.Empty, -1);
                finalmessage = "Error refreshing settings and item list";
                ReportLastRun();
                return false;
            }

            // If not already verbose, check settings
            if (!verbose)
            {
                verbose = settings.Builder.Verbose_Flag;
            }

            Add_NonError_To_Log("Refreshed settings and item list", verbose, String.Empty, String.Empty, -1);

            // Check for abort
            if (CheckForAbort()) 
            {
                Add_NonError_To_Log("Aborted (Worker_BulkLoader line 137)", verbose, String.Empty, String.Empty, -1);
                finalmessage = "Aborted per database request";
                ReportLastRun();
                return false; 
            }


	        // Set to standard operation then
			Abort_Database_Mechanism.Builder_Operation_Flag = Builder_Operation_Flag_Enum.STANDARD_OPERATION;

            // Run the usage stats module first
            if (firstrun)
            {
                CalculateUsageStatisticsModule statsModule = new CalculateUsageStatisticsModule();
                statsModule.Process += module_Process;
                statsModule.Error += module_Error;
                statsModule.DoWork(settings);
                firstrun = false;
            }

            // RUN ANY PRE-PROCESSING MODULES HERE 
            if (builderModules.PreProcessModules.Count > 0)
            {
                Add_NonError_To_Log("Running all pre-processing steps", verbose, String.Empty, String.Empty, -1);
                foreach (iPreProcessModule thisModule in builderModules.PreProcessModules)
                {
                    // Check for abort
                    if (CheckForAbort())
                    {
                        Abort_Database_Mechanism.Builder_Operation_Flag = Builder_Operation_Flag_Enum.ABORTING;
                        break;
                    }

                    thisModule.DoWork(settings);
                }
            }

            // Load the settings into thall the item and folder processors
            foreach (iSubmissionPackageModule thisModule in builderModules.ItemProcessModules)
                thisModule.Settings = settings;
            foreach (iSubmissionPackageModule thisModule in builderModules.DeleteItemModules)
                thisModule.Settings = settings;
            foreach (iFolderModule thisModule in builderModules.AllFolderModules)
                thisModule.Settings = settings;


            Add_NonError_To_Log("Begin completing any recent loads requiring additional work", verbose, String.Empty, String.Empty, -1);

            // Handle all packages already on the web server which are flagged for additional work required
            Complete_Any_Recent_Loads_Requiring_Additional_Work();

            Add_NonError_To_Log("Finished completing any recent loads requiring additional work", verbose, String.Empty, String.Empty, -1);

            // Check for abort
            if (CheckForAbort())
            {
                Add_NonError_To_Log("Aborted (Worker_BulkLoader line 151)", verbose, String.Empty, String.Empty, -1);
                finalmessage = "Aborted per database request";
                ReleaseResources();
                ReportLastRun();
                return false;
            }

            // Create the seperate queues for each type of incoming digital resource files
            List<Incoming_Digital_Resource> incoming_packages = new List<Incoming_Digital_Resource>();
            List<Incoming_Digital_Resource> deletes = new List<Incoming_Digital_Resource>();

            // Step through all the incoming folders, and run the folder modules
            if (builderSettings.IncomingFolders.Count == 0)
            {
                if (!noFoldersReported)
                {
                    Add_NonError_To_Log("There are no incoming folders set in the database", "Standard", String.Empty, String.Empty, -1);
                    noFoldersReported = true;
                }
            }
            else
            {
                Add_NonError_To_Log("Begin processing builder folders", verbose, String.Empty, String.Empty, -1);

                foreach (Builder_Source_Folder folder in builderSettings.IncomingFolders)
                {
                    Actionable_Builder_Source_Folder actionFolder = new Actionable_Builder_Source_Folder(folder, builderModules);

                    foreach (iFolderModule thisModule in actionFolder.BuilderModules)
                    {
                        // Check for abort
                        if (CheckForAbort())
                        {
                            Add_NonError_To_Log("Aborted (Worker_BulkLoader line 151)", verbose, String.Empty, String.Empty, -1);
                            finalmessage = "Aborted per database request";
                            ReleaseResources();
                            ReportLastRun();
                            return false;
                        }

                        thisModule.DoWork(actionFolder, incoming_packages, deletes);
                    }
                }

                // Since all folder processing is complete, release resources
                foreach (iFolderModule thisModule in builderModules.AllFolderModules)
                    thisModule.ReleaseResources();
            }
            

            // Check for abort
            if (CheckForAbort())
            {
                Add_NonError_To_Log("Aborted (Worker_BulkLoader line 179)", verbose, String.Empty, String.Empty, -1);
                finalmessage = "Aborted per database request";
                ReleaseResources();
                ReportLastRun();
                return false;
            }

            // If there were no packages to process further stop here
            if ((incoming_packages.Count == 0) && (deletes.Count == 0))
            {
	            Add_Complete_To_Log("No New Packages - Process Complete", "No Work", String.Empty, String.Empty, -1);
                if (finalmessage.Length == 0)
                    finalmessage = "No New Packages - Process Complete";
                ReleaseResources();
                ReportLastRun();
                return false;
            }

            // Iterate through all non-delete resources ready for processing
            Add_NonError_To_Log("Process any incoming packages", verbose, String.Empty, String.Empty, -1);
            Process_All_Incoming_Packages(incoming_packages);

            // Can now release these resources
            foreach (iSubmissionPackageModule thisModule in builderModules.ItemProcessModules)
            {
                thisModule.ReleaseResources();
            }

            // Process any delete requests ( iterate through all deletes )
            Add_NonError_To_Log("Process any deletes", verbose, String.Empty, String.Empty, -1);
            Process_All_Deletes(deletes);

            // Can now release these resources
            foreach (iSubmissionPackageModule thisModule in builderModules.DeleteItemModules)
            {
                thisModule.ReleaseResources();
            }


            // RUN ANY POST-PROCESSING MODULES HERE 
            if (builderModules.PostProcessModules.Count > 0)
            {
                Add_NonError_To_Log("Running all post-processing steps", verbose, String.Empty, String.Empty, -1);
                foreach (iPostProcessModule thisModule in builderModules.PostProcessModules)
                {
                    // Check for abort
                    if (CheckForAbort())
                    {
                        Abort_Database_Mechanism.Builder_Operation_Flag = Builder_Operation_Flag_Enum.ABORTING;
                        break;
                    }

                    thisModule.DoWork(aggregationsToRefresh, processedItems, deletedItems, settings);
                }
            }

            // Add the complete entry for the log
            if (!CheckForAbort())
            {
                Add_Complete_To_Log("Process Complete", "Complete", String.Empty, String.Empty, -1);
                if (finalmessage.Length == 0)
                    finalmessage = "Process completed successfully";
            }
            else
            {
                finalmessage = "Aborted per database request";
                Add_Complete_To_Log("Process Aborted Cleanly", "Complete", String.Empty, String.Empty, -1);
            }

            // Save information about this last run
            ReportLastRun();

            // Clear lots of collections and such from memory, since we are done processing
            ReleaseResources();


            Add_NonError_To_Log("Process Done", verbose, String.Empty, String.Empty, -1);



            return stillPendingItems;

        }

        private void ReportLastRun()
        {
            // Save information about this last run
            Library.Database.SobekCM_Database.Set_Setting("Builder Version", Engine_ApplicationCache_Gateway.Settings.Static.Current_Builder_Version);
            Library.Database.SobekCM_Database.Set_Setting("Builder Last Run Finished", DateTime.Now.ToString());
            Library.Database.SobekCM_Database.Set_Setting("Builder Last Message", finalmessage);
        }

        /// <summary> Release all the resources held in this class and all the related builder modules </summary>
        public void ReleaseResources()
        {
            // Set some things to NULL
            aggregationsToRefresh.Clear();
            processedItems.Clear();
            deletedItems.Clear();

            // release all modules
            foreach (iFolderModule thisModule in builderModules.AllFolderModules)
            {
                thisModule.ReleaseResources();
            }
            foreach (iSubmissionPackageModule thisModule in builderModules.DeleteItemModules)
            {
                thisModule.ReleaseResources();
            }
            foreach (iSubmissionPackageModule thisModule in builderModules.ItemProcessModules)
            {
                thisModule.ReleaseResources();
            }
        }

        #endregion

        #region Refresh any settings and item lists and clear the item list

		/// <summary> Refresh the settings and item list from the database </summary>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
        public bool Refresh_Settings_And_Item_List()
        {
            // Create the tracer for this
		    Custom_Tracer tracer = new Custom_Tracer();

            // Disable the cache
            CachedDataManager.Settings.Disabled = true;

            // Set all the database strings appropriately
            Engine_Database.Connection_String = instanceInfo.DatabaseConnection.Connection_String;
            SobekCM_Item_Database.Connection_String = instanceInfo.DatabaseConnection.Connection_String;
            Library.Database.SobekCM_Database.Connection_String = instanceInfo.DatabaseConnection.Connection_String;

            // Get the settings values directly from the database
            settings = InstanceWide_Settings_Builder.Build_Settings(instanceInfo.DatabaseConnection);
		    if (settings == null)
		    {
	            Add_Error_To_Log("Unable to pull the newest settings from the database", String.Empty, String.Empty, -1);
                return false;
		    }

            // If this was not refreshed yet, ensure [BASEURL] is replaced
		    if (!refreshed)
		    {
                // Determine the base url
                string baseUrl = String.IsNullOrWhiteSpace(settings.Servers.Base_URL) ? settings.Servers.Application_Server_URL : settings.Servers.Base_URL;
		        List<MicroservicesClient_Endpoint> endpoints = instanceInfo.Microservices.Endpoints;
		        foreach (MicroservicesClient_Endpoint thisEndpoint in endpoints)
		        {
		            if (thisEndpoint.URL.IndexOf("[BASEURL]") > 0)
                        thisEndpoint.URL = thisEndpoint.URL.Replace("[BASEURL]", baseUrl).Replace("//", "/").Replace("http:/", "http://").Replace("https:/", "https://");
                    else if (( thisEndpoint.URL.IndexOf("http:/") < 0 ) && ( thisEndpoint.URL.IndexOf("https:/") < 0 ))
                        thisEndpoint.URL = ( baseUrl + thisEndpoint.URL).Replace("//", "/").Replace("http:/", "http://").Replace("https:/", "https://");
		        }
                refreshed = true;
		    }

            // Set the microservice endpoints
		    SobekEngineClient.Set_Endpoints(instanceInfo.Microservices);

            // Load the necessary configuration objects into the engine application cache gateway
		    try
		    {
		        Engine_ApplicationCache_Gateway.Configuration.OAI_PMH = SobekEngineClient.Admin.Get_OAI_PMH_Configuration(tracer);
		    }
		    catch (Exception ee)
		    {
                Add_Error_To_Log("Unable to pull the OAI-PMH settings from the engine", String.Empty, String.Empty, -1);
                Add_Error_To_Log(ee.Message, String.Empty, String.Empty, -1);
                return false;
		    }

            try
            {
                Engine_ApplicationCache_Gateway.Configuration.Metadata = SobekEngineClient.Admin.Get_Metadata_Configuration(tracer);
            }
            catch (Exception ee)
            {
                Add_Error_To_Log("Unable to pull the metadata settings from the engine", String.Empty, String.Empty, -1);
                Add_Error_To_Log(ee.Message, String.Empty, String.Empty, -1);
                return false;
            }

            try
            {
                Engine_ApplicationCache_Gateway.Configuration.Extensions = SobekEngineClient.Admin.Get_Extensions_Configuration(tracer);
            }
            catch (Exception ee)
            {
                Add_Error_To_Log("Unable to pull the extension settings from the engine", String.Empty, String.Empty, -1);
                Add_Error_To_Log(ee.Message, String.Empty, String.Empty, -1);
                return false;
            }


            // Check for any enabled extensions with assemblies
		    ResourceObjectSettings.Clear_Assemblies();
		    try
		    {
		        if ((Engine_ApplicationCache_Gateway.Configuration.Extensions.Extensions != null) && (Engine_ApplicationCache_Gateway.Configuration.Extensions.Extensions.Count > 0))
		        {
		            // Step through each extension
		            foreach (ExtensionInfo extensionInfo in Engine_ApplicationCache_Gateway.Configuration.Extensions.Extensions)
		            {
		                // If not enabled, skip it
		                if (!extensionInfo.Enabled) continue;

		                // Look for assemblies
		                if ((extensionInfo.Assemblies != null) && (extensionInfo.Assemblies.Count > 0))
		                {
		                    // Step through each assembly
		                    foreach (ExtensionAssembly assembly in extensionInfo.Assemblies)
		                    {
		                        // Find the relative file name
		                        if (assembly.FilePath.IndexOf("plugins", StringComparison.OrdinalIgnoreCase) > 0)
		                        {
		                            // Determine the network way to get there
		                            string from_plugins = assembly.FilePath.Substring(assembly.FilePath.IndexOf("plugins", StringComparison.OrdinalIgnoreCase));
		                            string network_plugin_file = Path.Combine(settings.Servers.Application_Server_Network, from_plugins);

		                            // Get the plugin filename
		                            string plugin_filename = Path.GetFileName(assembly.FilePath);

		                            // Does this local plugin directory exist for this extension?
		                            string local_path = Path.Combine(pluginRootDirectory, instanceInfo.Name, extensionInfo.Code);
		                            if (!Directory.Exists(local_path))
		                            {
		                                try
		                                {
		                                    Directory.CreateDirectory(local_path);
		                                }
		                                catch (Exception ee)
		                                {
		                                    Add_Error_To_Log("Error creating the necessary plug-in subdirectory", String.Empty, String.Empty, -1);
		                                    Add_Error_To_Log(ee.Message, String.Empty, String.Empty, -1);
		                                    return false;
		                                }
		                            }

		                            // Determine if the assembly is here
		                            string local_file = Path.Combine(local_path, plugin_filename);
		                            if (!File.Exists(local_file))
		                            {
		                                File.Copy(network_plugin_file, local_file);
		                            }
		                            else
		                            {
		                                // Do a date check
		                                DateTime webFileDate = File.GetLastWriteTime(network_plugin_file);
		                                DateTime localFileDate = File.GetLastWriteTime(local_file);

		                                if (webFileDate.CompareTo(localFileDate) > 0)
		                                {
		                                    File.Copy(network_plugin_file, local_file, true);
		                                }
		                            }

		                            // Also, point the assembly to use the local file
		                            assembly.FilePath = local_file;
		                        }
		                    }
		                }
		            }

		            // Now, also set this all in the metadata portion
		            // Copy over all the extension information
		            foreach (ExtensionInfo thisExtension in Engine_ApplicationCache_Gateway.Configuration.Extensions.Extensions)
		            {
		                if ((thisExtension.Enabled) && (thisExtension.Assemblies != null))
		                {
		                    foreach (ExtensionAssembly thisAssembly in thisExtension.Assemblies)
		                    {
		                        ResourceObjectSettings.Add_Assembly(thisAssembly.ID, thisAssembly.FilePath);
		                    }
		                }
		            }
		        }
		    }
		    catch (Exception ee)
		    {
		        Add_Error_To_Log("Unable to copy the extension files from the web", String.Empty, String.Empty, -1);
		        Add_Error_To_Log(ee.Message, String.Empty, String.Empty, -1);
		        return false;
		    }
            
            // Finalize the metadata config
            Engine_ApplicationCache_Gateway.Configuration.Metadata.Finalize_Metadata_Configuration();

            // Set the metadata preferences for writing
            ResourceObjectSettings.MetadataConfig = Engine_ApplicationCache_Gateway.Configuration.Metadata;

		    // Also, load the builder configuration this way
            try
            {
                builderSettings = SobekEngineClient.Builder.Get_Builder_Settings(false, tracer);
            }
            catch (Exception ee)
            {
                Add_Error_To_Log("Unable to pull the builder settings from the engine", String.Empty, String.Empty, -1);
                Add_Error_To_Log(ee.Message, String.Empty, String.Empty, -1);
                return false;
            }


            // Build the modules
		    builderModules = new Builder_Modules(builderSettings);
            List<string> errors = builderModules.Builder_Modules_From_Settings();

            if (( errors != null ) && ( errors.Count > 0 ))
            {
                long logId = Add_Error_To_Log("Error(s) builder the modules from the settings", String.Empty, String.Empty, -1);
                foreach (string thisError in errors)
                {
                    Add_Error_To_Log(thisError, String.Empty, String.Empty, logId);
                }
                return false;
            }

            // Add the event listeners 
            foreach (iPreProcessModule thisModule in builderModules.PreProcessModules)
            {
                thisModule.Error += module_Error;
                thisModule.Process += module_Process;
            }
            foreach (iSubmissionPackageModule thisModule in builderModules.DeleteItemModules)
            {
                thisModule.Error += module_Error;
                thisModule.Process += module_Process;
            }
            foreach (iSubmissionPackageModule thisModule in builderModules.ItemProcessModules)
            {
                thisModule.Error += module_Error;
                thisModule.Process += module_Process;
            }
            foreach (iPostProcessModule thisModule in builderModules.PostProcessModules)
            {
                thisModule.Error += module_Error;
                thisModule.Process += module_Process;
            }
            foreach (iFolderModule thisModule in builderModules.AllFolderModules)
            {
                thisModule.Error += module_Error;
                thisModule.Process += module_Process;
            }

            return true;
        }

        #endregion

        #region Process any recent loads which require additonal work

        private void Complete_Any_Recent_Loads_Requiring_Additional_Work()
        {
            // Get the list of recent loads requiring additional work
            DataTable additionalWorkRequired = Library.Database.SobekCM_Database.Items_Needing_Aditional_Work;
            if ((additionalWorkRequired != null) && (additionalWorkRequired.Rows.Count > 0))
            {
	            Add_NonError_To_Log("Processing recently loaded items needing additional work", "Standard", String.Empty, String.Empty, -1);

                // Create the incoming digital folder object which will be used for all these
                Actionable_Builder_Source_Folder sourceFolder = new Actionable_Builder_Source_Folder();

                // Step through each one
                foreach (DataRow thisRow in additionalWorkRequired.Rows)
                {
                    // Get the information about this item
                    string bibID = thisRow["BibID"].ToString();
                    string vid = thisRow["VID"].ToString();

	                // Determine the file root for this
                    string file_root = bibID.Substring(0, 2) + "\\" + bibID.Substring(2, 2) + "\\" + bibID.Substring(4, 2) + "\\" + bibID.Substring(6, 2) + "\\" + bibID.Substring(8, 2);

                    // Determine the source folder for this resource
                    string resource_folder = settings.Servers.Image_Server_Network + file_root + "\\" + vid;

                    // Determine the METS file name
                    string mets_file = resource_folder + "\\" + bibID + "_" + vid + ".mets.xml";

                    // Ensure these both exist
                    if ((Directory.Exists(resource_folder)) && (File.Exists(mets_file)))
                    {
                        // Create the incoming digital resource object
                        Incoming_Digital_Resource additionalWorkResource = new Incoming_Digital_Resource(resource_folder, sourceFolder) 
							{BibID = bibID, VID = vid, File_Root = bibID.Substring(0, 2) + "\\" + bibID.Substring(2, 2) + "\\" + bibID.Substring(4, 2) + "\\" + bibID.Substring(6, 2) + "\\" + bibID.Substring(8, 2)};

	                    Complete_Single_Recent_Load_Requiring_Additional_Work( additionalWorkResource);
                    }
                    else
                    {
	                    Add_Error_To_Log("Unable to find valid resource files for reprocessing " + bibID + ":" + vid, bibID + ":" + vid, "Reprocess", -1);

	                    int itemID = Library.Database.SobekCM_Database.Get_ItemID_From_Bib_VID(bibID, vid);

						Library.Database.SobekCM_Database.Update_Additional_Work_Needed_Flag(itemID, false, null);
                    }
                }
            }
        }

        private void Complete_Single_Recent_Load_Requiring_Additional_Work( Incoming_Digital_Resource AdditionalWorkResource)
        {
	        AdditionalWorkResource.METS_Type_String = "Reprocess";
            AdditionalWorkResource.BuilderLogId = Add_NonError_To_Log("Reprocessing '" + AdditionalWorkResource.BibID + ":" + AdditionalWorkResource.VID + "'", "Standard",  AdditionalWorkResource.BibID + ":" + AdditionalWorkResource.VID, AdditionalWorkResource.METS_Type_String, -1);

            try
            {
                // Load the METS file
                if ((!AdditionalWorkResource.Load_METS()) || (AdditionalWorkResource.BibID.Length == 0))
                {
                    Add_Error_To_Log("Error reading METS file from " + AdditionalWorkResource.Folder_Name.Replace("_", ":"), AdditionalWorkResource.Folder_Name.Replace("_", ":"), "Reprocess", AdditionalWorkResource.BuilderLogId);
                    return;
                }

                AdditionalWorkResource.METS_Type_String = "Reprocess";

                // Add thumbnail and aggregation informaiton from the database 
                Library.Database.SobekCM_Database.Add_Minimum_Builder_Information(AdditionalWorkResource.Metadata);

                // Do all the item processing per instance config
                foreach (iSubmissionPackageModule thisModule in builderModules.ItemProcessModules)
                {
                    if (verbose)
                    {
                        Add_NonError_To_Log("Running module " + thisModule.GetType(), true, AdditionalWorkResource.BibID + ":" + AdditionalWorkResource.VID, String.Empty, AdditionalWorkResource.BuilderLogId);
                    }
                    if (!thisModule.DoWork(AdditionalWorkResource))
                    {
                        Add_Error_To_Log("Unable to complete additional work for " + AdditionalWorkResource.BibID + ":" + AdditionalWorkResource.VID, AdditionalWorkResource.BibID + ":" + AdditionalWorkResource.VID, String.Empty, AdditionalWorkResource.BuilderLogId);

                        return;
                    }
                }

                // Save these collections to mark them for refreshing the RSS feeds, etc..
                Add_Process_Info_To_PostProcess_Lists(AdditionalWorkResource.BibID, AdditionalWorkResource.VID, AdditionalWorkResource.Metadata.Behaviors.Aggregation_Code_List);

                // Finally, clear the memory a little bit
                AdditionalWorkResource.Clear_METS();
            }
            catch (Exception ee)
            {
                Add_Error_To_Log("Unable to complete additional work for " + AdditionalWorkResource.BibID + ":" + AdditionalWorkResource.VID, AdditionalWorkResource.BibID + ":" + AdditionalWorkResource.VID, AdditionalWorkResource.METS_Type_String, AdditionalWorkResource.BuilderLogId, ee);
            }
        }

        #endregion

        #region Process any complete packages, whether a new resource or a replacement

        private void Process_All_Incoming_Packages(List<Incoming_Digital_Resource> IncomingPackages )
        {
            if (IncomingPackages.Count == 0)
                return;

            try
            {
                int number_packages_processed = 0;

                // Step through each package and handle all the files and metadata
                Add_NonError_To_Log("....Processing incoming packages", "Standard", String.Empty, String.Empty, -1);
                IncomingPackages.Sort();
                foreach (Incoming_Digital_Resource resourcePackage in IncomingPackages)
                {
                    // Check for abort
                    if (CheckForAbort())
                    {
                        Abort_Database_Mechanism.Builder_Operation_Flag = Builder_Operation_Flag_Enum.ABORTING;
                        return;
                    }

                    Process_Single_Incoming_Package(resourcePackage);

                    if (newItemLimit > 0)
                    {
                        number_packages_processed++;
                        if (number_packages_processed >= MultiInstance_Builder_Settings.Instance_Package_Limit)
                        {
                            stillPendingItems = true;
                            Add_NonError_To_Log("....Still pending packages, but moving to next instances and will return for these", "Standard", String.Empty, String.Empty, -1);
                            return;
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                StreamWriter errorWriter = new StreamWriter(logFileDirectory + "\\error.log", true);
                errorWriter.WriteLine("Message: " + ee.Message);
                errorWriter.WriteLine("Stack Trace: " + ee.StackTrace);
                errorWriter.Flush();
                errorWriter.Close();

                Add_Error_To_Log("Unable to process all of the NEW and REPLACEMENT packages.", String.Empty, String.Empty, -1, ee);
            }
        }

        private void Process_Single_Incoming_Package(Incoming_Digital_Resource ResourcePackage)
        {

            ResourcePackage.BuilderLogId = Add_NonError_To_Log("........Processing '" + ResourcePackage.Folder_Name + "'", "Standard", ResourcePackage.BibID + ":" + ResourcePackage.VID, ResourcePackage.METS_Type_String, -1);

            // Clear any existing error linked to this item
			Library.Database.SobekCM_Database.Builder_Clear_Item_Error_Log(ResourcePackage.BibID, ResourcePackage.VID, "SobekCM Builder");

            // Before we save this or anything, let's see if this is truly a new resource
            ResourcePackage.NewPackage = (Engine_Database.Get_Item_Information(ResourcePackage.BibID, ResourcePackage.VID, null ) == null );
            ResourcePackage.Package_Time = DateTime.Now;

            try
            {
                // Do all the item processing per instance config
                foreach (iSubmissionPackageModule thisModule in builderModules.ItemProcessModules)
                {
                    //if ( superverbose)
                    //{
                    //    Add_NonError_To_Log("Running module " + thisModule.GetType().ToString(), true, ResourcePackage.BibID + ":" + ResourcePackage.VID, String.Empty, ResourcePackage.BuilderLogId);
                    //}
                    if (!thisModule.DoWork(ResourcePackage))
                    {
                        Add_Error_To_Log("Unable to complete new/replacement for " + ResourcePackage.BibID + ":" + ResourcePackage.VID, ResourcePackage.BibID + ":" + ResourcePackage.VID, String.Empty, ResourcePackage.BuilderLogId);

                        // Try to move the whole package to the failures folder
                        string final_failures_folder = Path.Combine(ResourcePackage.Source_Folder.Failures_Folder, ResourcePackage.BibID + "_" + ResourcePackage.VID);
                        if (Directory.Exists(final_failures_folder))
                        {
                            final_failures_folder = final_failures_folder + "_" + DateTime.Now.Year + "_" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "_" + DateTime.Now.Day.ToString().PadLeft(2, '0') + "_" + DateTime.Now.Hour.ToString().PadLeft(2, '0') + "_" + DateTime.Now.Minute.ToString().PadLeft(2, '0') + "_" + DateTime.Now.Second.ToString().PadLeft(2, '0');
                        }

                        try
                        {
                            Directory.Move(ResourcePackage.Resource_Folder, final_failures_folder);
                        }
                        catch
                        {
                            
                        }
                        return;
                    }
                }

                // Save these collections to mark them for refreshing the RSS feeds, etc..
                Add_Process_Info_To_PostProcess_Lists(ResourcePackage.BibID, ResourcePackage.VID, ResourcePackage.Metadata.Behaviors.Aggregation_Code_List);

                // Finally, clear the memory a little bit
                ResourcePackage.Clear_METS();
            }
            catch (Exception ee)
            {
                StreamWriter errorWriter = new StreamWriter(logFileDirectory + "\\error.log", true);
                errorWriter.WriteLine("Message: " + ee.Message);
                errorWriter.WriteLine("Stack Trace: " + ee.StackTrace);
                errorWriter.Flush();
                errorWriter.Close();

                Add_Error_To_Log("Unable to complete new/replacement for " + ResourcePackage.BibID + ":" + ResourcePackage.VID, ResourcePackage.BibID + ":" + ResourcePackage.VID, String.Empty, ResourcePackage.BuilderLogId, ee);
            }
        }


        long module_Error(string LogStatement, string BibID_VID, string MetsType, long RelatedLogID)
        {
            return Add_Error_To_Log(LogStatement, BibID_VID, MetsType, RelatedLogID);
        }

        long module_Process(string LogStatement, string DbLogType, string BibID_VID, string MetsType, long RelatedLogID)
        {
            return Add_NonError_To_Log(LogStatement, DbLogType, BibID_VID, MetsType, RelatedLogID);
        }

        #endregion

        #region Process any delete requests

        private void Process_All_Deletes(List<Incoming_Digital_Resource> Deletes)
        {
            if (Deletes.Count == 0)
                return;

            Add_NonError_To_Log("....Processing delete packages", "Standard", String.Empty, String.Empty, -1);
            Deletes.Sort();
            foreach (Incoming_Digital_Resource deleteResource in Deletes)
            {
                // Check for abort
                if (CheckForAbort())
                {
                    Abort_Database_Mechanism.Builder_Operation_Flag = Builder_Operation_Flag_Enum.ABORTING;
                    return;
                }

                // Save these collections to mark them for search index building
                Add_Delete_Info_To_PostProcess_Lists(deleteResource.BibID, deleteResource.VID, deleteResource.Metadata.Behaviors.Aggregation_Code_List);

                // Only continue if this bibid/vid exists
                if (Engine_Database.Get_Item_Information(deleteResource.BibID, deleteResource.VID, null) != null)
                {
                    // Do all the item processing per instance config
                    foreach (iSubmissionPackageModule thisModule in builderModules.DeleteItemModules)
                    {
                        if (!thisModule.DoWork(deleteResource))
                        {
                            Add_Error_To_Log("Unable to complete delete for " + deleteResource.BibID + ":" + deleteResource.VID, deleteResource.BibID + ":" + deleteResource.VID, String.Empty, deleteResource.BuilderLogId);

                            // Try to move the whole package to the failures folder
                            string final_failures_folder = Path.Combine(deleteResource.Source_Folder.Failures_Folder, deleteResource.BibID + "_" + deleteResource.VID);
                            if (Directory.Exists(final_failures_folder))
                            {
                                final_failures_folder = final_failures_folder + "_" + DateTime.Now.Year + "_" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "_" + DateTime.Now.Day.ToString().PadLeft(2, '0') + "_" + DateTime.Now.Hour.ToString().PadLeft(2, '0') + "_" + DateTime.Now.Minute.ToString().PadLeft(2, '0') + "_" + DateTime.Now.Second.ToString().PadLeft(2, '0');
                            }

                            try
                            {
                                Directory.Move(deleteResource.Resource_Folder, final_failures_folder);
                            }
                            catch
                            {
                                // Do nothing if not able to move?
                            }
                            return;
                        }
                    }
                }
                else
                {
                    Add_Error_To_Log("Delete ( " + deleteResource.BibID + ":" + deleteResource.VID + " ) invalid... no pre-existing resource", deleteResource.BibID + ":" + deleteResource.VID, deleteResource.METS_Type_String, deleteResource.BuilderLogId);

                    // Finally, clear the memory a little bit
                    deleteResource.Clear_METS();

                    // Delete the handled METS file and package
                    deleteResource.Delete();
                }
            }
        }

        #endregion

        #region Log-supporting methods

		private long Add_NonError_To_Log(string LogStatement, string DbLogType, string BibID_VID, string MetsType, long RelatedLogID )
        {
			if (multiInstanceBuilder)
			{
				Console.WriteLine( instanceInfo.Name + " - " + LogStatement);
                logger.AddNonError(instanceInfo.Name + " - " + LogStatement.Replace("\t", "....."));
			}
			else
			{
				Console.WriteLine( LogStatement);
				logger.AddNonError( LogStatement.Replace("\t", "....."));
			}
			return Library.Database.SobekCM_Database.Builder_Add_Log_Entry(RelatedLogID, BibID_VID, DbLogType, LogStatement.Replace("\t", ""), MetsType);
        }

		private long Add_NonError_To_Log(string LogStatement, bool IsVerbose, string BibID_VID, string MetsType, long RelatedLogID)
        {
            if (IsVerbose)
            {
	            if (multiInstanceBuilder)
	            {
                    Console.WriteLine(instanceInfo.Name + " - " + LogStatement);
                    logger.AddNonError(instanceInfo.Name + " - " + LogStatement.Replace("\t", "....."));
	            }
	            else
				{
					Console.WriteLine( LogStatement);
					logger.AddNonError( LogStatement.Replace("\t", "....."));
				}
	            return Library.Database.SobekCM_Database.Builder_Add_Log_Entry(RelatedLogID, BibID_VID, "Verbose", LogStatement.Replace("\t", ""), MetsType);
            }
			return -1;
        }

		private long Add_Error_To_Log(string LogStatement, string BibID_VID, string MetsType, long RelatedLogID)
        {
			if (multiInstanceBuilder)
			{
                Console.WriteLine(instanceInfo.Name + " - " + LogStatement);
                logger.AddError(instanceInfo.Name + " - " + LogStatement.Replace("\t", "....."));
			}
			else
			{
				Console.WriteLine( LogStatement);
				logger.AddError( LogStatement.Replace("\t", "....."));
			}
			return Library.Database.SobekCM_Database.Builder_Add_Log_Entry(RelatedLogID, BibID_VID, "Error", LogStatement.Replace("\t", ""), MetsType);
        }

        private void Add_Error_To_Log(string LogStatement, string BibID_VID, string MetsType, long RelatedLogID, Exception Ee)
        {
            if (multiInstanceBuilder)
            {
                Console.WriteLine(instanceInfo.Name + " - " + LogStatement);
                logger.AddError(instanceInfo.Name + " - " + LogStatement.Replace("\t", "....."));
            }
            else
            {
                Console.WriteLine(LogStatement);
                logger.AddError(LogStatement.Replace("\t", "....."));
            }
            long mainErrorId = Library.Database.SobekCM_Database.Builder_Add_Log_Entry(RelatedLogID, BibID_VID, "Error", LogStatement.Replace("\t", ""), MetsType);


            string[] split = Ee.ToString().Split("\n".ToCharArray());
            foreach (string thisSplit in split)
            {
                Library.Database.SobekCM_Database.Builder_Add_Log_Entry(mainErrorId, BibID_VID, "Error", thisSplit, MetsType);
            }


            // Determine, and create the local work space
            string localLogArea = Path.Combine(MultiInstance_Builder_Settings.Builder_Executable_Directory, "logs");

            if (!Directory.Exists(localLogArea))
                Directory.CreateDirectory(localLogArea);


            // Save the exception to an exception file
            StreamWriter exception_writer = new StreamWriter(Path.Combine(localLogArea, "exceptions_log.txt"), true);
            exception_writer.WriteLine(String.Empty);
            exception_writer.WriteLine(String.Empty);
            exception_writer.WriteLine("----------------------------------------------------------");
            exception_writer.WriteLine("EXCEPTION CAUGHT " + DateTime.Now.ToString() + " BY PRELOADER");
            exception_writer.WriteLine(LogStatement.ToUpper().Replace("\t", "").Trim());
            exception_writer.WriteLine(Ee.ToString());
            exception_writer.Flush();
            exception_writer.Close();
        }

        private void Add_Complete_To_Log(string LogStatement, string DbLogType, string BibID_VID, string MetsType, long RelatedLogID )
        {
	        if (multiInstanceBuilder)
	        {
                Console.WriteLine(instanceInfo.Name + " - " + LogStatement);
                logger.AddComplete(instanceInfo.Name + " - " + LogStatement.Replace("\t", "....."));
	        }
	        else
			{
				Console.WriteLine( LogStatement);
				logger.AddComplete( LogStatement.Replace("\t", "....."));
			}
	        Library.Database.SobekCM_Database.Builder_Add_Log_Entry(RelatedLogID, BibID_VID, DbLogType, LogStatement.Replace("\t", ""), MetsType);
        }

        #endregion

        #region Methods used to get the list of collections to mark in db for build

        private void Add_Aggregation_To_Refresh_List(string Code)
        {
            // Only continue if there is length
            if (Code.Length > 1)
            {
                // This aggregation should be refreshed
                if (!aggregationsToRefresh.Contains(Code.ToUpper()))
                    aggregationsToRefresh.Add(Code.ToUpper());
            }
        }

        private void Add_Process_Info_To_PostProcess_Lists(string BibID, string VID, IEnumerable<string> Codes)
        {
            foreach (string collectionCode in Codes)
            {
                Add_Aggregation_To_Refresh_List(collectionCode);
            }
            processedItems.Add(new BibVidStruct(BibID, VID));
        }

        private void Add_Delete_Info_To_PostProcess_Lists(string BibID, string VID, IEnumerable<string> Codes)
        {
            foreach (string collectionCode in Codes)
            {
                Add_Aggregation_To_Refresh_List(collectionCode);
            }
            deletedItems.Add(new BibVidStruct(BibID, VID));
        }

        #endregion

        #region Methods to handle checking for abort requests

		/// <summary> Flag indicates if the last run of the bulk loader was ABORTED </summary>
        public bool Aborted
        {
            get { return aborted; }
        }

        private bool CheckForAbort()
        {

            if (aborted)
                return true;

            bool returnValue = Abort_Database_Mechanism.Abort_Requested();
            if (returnValue )
            {
                aborted = true;
                
                logger.AddError("ABORT REQUEST RECEIVED VIA DATABASE KEY");
            }
            return returnValue;
        }

        #endregion
        
		/// <summary> Gets the message to sum up this execution  </summary>
        public string Final_Message
        {
            get { return finalmessage; }
        }
    }
}
