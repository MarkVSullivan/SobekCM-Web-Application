#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using SobekCM.Builder_Library.Modules.Schedulable;
using SobekCM.Builder_Library.Settings;
using SobekCM.Core.Client;
using SobekCM.Core.Configuration;
using SobekCM.Core.MemoryMgmt;
using SobekCM.Core.Settings;
using SobekCM.Builder_Library.Modules;
using SobekCM.Builder_Library.Modules.Folders;
using SobekCM.Builder_Library.Modules.Items;
using SobekCM.Builder_Library.Modules.PostProcess;
using SobekCM.Builder_Library.Modules.PreProcess;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Engine_Library.Database;
using SobekCM.Engine_Library.Settings;
using SobekCM.Library.Database;
using SobekCM.Tools.Logs;
using SobekCM.Builder_Library;

#endregion

namespace SobekCM.Builder_Library
{
    /// <summary> Class is the worker thread for the main bulk loader processor </summary>
    public class Worker_BulkLoader
    {
        private readonly Database_Instance_Configuration dbInstance;
        private InstanceWide_Settings settings;
        public Builder_Modules BuilderSettings;
        private DataTable itemTable;

        private readonly LogFileXHTML logger;
        private readonly string logFileDirectory;
        
	    private readonly bool canAbort;
        private bool aborted;
        private bool verbose;
        
	    private readonly bool multiInstanceBuilder;

	    private readonly string instanceName;
	    private string finalmessage;
        
        private readonly List<string> aggregations_to_refresh;
        private readonly List<BibVidStruct> processed_items;
        private readonly List<BibVidStruct> deleted_items;

        private readonly int new_item_limit;
        private bool still_pending_items;


        /// <summary> Constructor for a new instance of the Worker_BulkLoader class </summary>
        /// <param name="Logger"> Log file object for logging progress </param>
        /// <param name="Verbose"> Flag indicates if the builder is in verbose mode, where it should log alot more information </param>
        /// <param name="DbInstance"> This database instance </param>
        /// <param name="MultiInstanceBuilder"></param>
        public Worker_BulkLoader(LogFileXHTML Logger, bool Verbose, Database_Instance_Configuration DbInstance, bool MultiInstanceBuilder, string LogFileDirectory )
        {
            // Save the log file and verbose flag
            logger = Logger;
            verbose = Verbose;
	        instanceName = DbInstance.Name;
		    canAbort = DbInstance.Can_Abort;
	        multiInstanceBuilder = MultiInstanceBuilder;
	        dbInstance = DbInstance;
            logFileDirectory = LogFileDirectory;

	        if (multiInstanceBuilder)
	            new_item_limit = 100;
	        else
	            new_item_limit = -1;
 
            Add_NonError_To_Log("Worker_BulkLoader.Constructor: Start", verbose, String.Empty, String.Empty, -1);


            // Create new list of collections to build
            aggregations_to_refresh = new List<string>();
	        processed_items = new List<BibVidStruct>();
	        deleted_items = new List<BibVidStruct>();

			// get all the info
	        settings = InstanceWide_Settings_Builder.Build_Settings(dbInstance);
	        Refresh_Settings_And_Item_List();
            
			// Ensure there is SOME instance name
	        if (instanceName.Length == 0)
		        instanceName = settings.System_Name;
            if (verbose)
                settings.Builder_Verbose_Flag = true;


            Add_NonError_To_Log("Worker_BulkLoader.Constructor: Created Static Pages Builder", verbose, String.Empty, String.Empty, -1);

            // Set some defaults
            aborted = false;


            Add_NonError_To_Log("Worker_BulkLoader.Constructor: Building modules for pre, post, and item processing", verbose, String.Empty, String.Empty, -1);


	        Add_NonError_To_Log("Worker_BulkLoader.Constructor: Done", verbose, String.Empty, String.Empty, -1);
        }

        #region Main Method that steps through each package and performs work

        /// <summary> Performs the bulk loader process and handles any incoming digital resources </summary>
        /// <returns> TRUE if there are still pending items to be processed, otherwise FALSE </returns>
        public bool Perform_BulkLoader( bool Verbose )
        {

            verbose = Verbose;
            finalmessage = String.Empty;
            still_pending_items = false;

            Add_NonError_To_Log("Worker_BulkLoader.Perform_BulkLoader: Start", verbose, String.Empty, String.Empty, -1);

            // Refresh any settings and item lists 
            if (!Refresh_Settings_And_Item_List())
            {
                Add_Error_To_Log("Worker_BulkLoader.Perform_BulkLoader: Error refreshing settings and item list", String.Empty, String.Empty, -1);
                finalmessage = "Error refreshing settings and item list";
                return false;
            }

            // If not already verbose, check settings
            if (!verbose)
            {
                verbose = settings.Builder_Verbose_Flag;
            }

            Add_NonError_To_Log("Worker_BulkLoader.Perform_BulkLoader: Refreshed settings and item list", verbose, String.Empty, String.Empty, -1);

            // Check for abort
            if (CheckForAbort()) 
            {
                Add_NonError_To_Log("Worker_BulkLoader.Perform_BulkLoader: Aborted (line 137)", verbose, String.Empty, String.Empty, -1);
                finalmessage = "Aborted per database request";
                return false; 
            }


	        // Set to standard operation then
			Abort_Database_Mechanism.Builder_Operation_Flag = Builder_Operation_Flag_Enum.STANDARD_OPERATION;

            // Run the usage stats module first
            CalculateUsageStatisticsModule statsModule = new CalculateUsageStatisticsModule();
            statsModule.Process += module_Process;
            statsModule.Error += module_Error;
            statsModule.DoWork(settings);
            
            // RUN ANY PRE-PROCESSING MODULES HERE 
            if (BuilderSettings.PreProcessModules.Count > 0)
            {
                Add_NonError_To_Log("Running all pre-processing steps", verbose, String.Empty, String.Empty, -1);
                foreach (iPreProcessModule thisModule in BuilderSettings.PreProcessModules)
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
            foreach (iSubmissionPackageModule thisModule in BuilderSettings.ItemProcessModules)
                thisModule.Settings = settings;
            foreach (iSubmissionPackageModule thisModule in BuilderSettings.DeleteItemModules)
                thisModule.Settings = settings;
            foreach (iFolderModule thisModule in BuilderSettings.AllFolderModules)
                thisModule.Settings = settings;


            Add_NonError_To_Log("Worker_BulkLoader.Perform_BulkLoader: Begin completing any recent loads requiring additional work", verbose, String.Empty, String.Empty, -1);

            // Handle all packages already on the web server which are flagged for additional work required
            Complete_Any_Recent_Loads_Requiring_Additional_Work();

            Add_NonError_To_Log("Worker_BulkLoader.Perform_BulkLoader: Finished completing any recent loads requiring additional work", verbose, String.Empty, String.Empty, -1);

            // Check for abort
            if (CheckForAbort())
            {
                Add_NonError_To_Log("Worker_BulkLoader.Perform_BulkLoader: Aborted (line 151)", verbose, String.Empty, String.Empty, -1);
                finalmessage = "Aborted per database request";
                ReleaseResources();
                return false;
            }

            // Create the seperate queues for each type of incoming digital resource files
            List<Incoming_Digital_Resource> incoming_packages = new List<Incoming_Digital_Resource>();
            List<Incoming_Digital_Resource> deletes = new List<Incoming_Digital_Resource>();

            // Step through all the incoming folders, and run the folder modules
            if (BuilderSettings.IncomingFolders.Count == 0)
            {
                Add_NonError_To_Log("Worker_BulkLoader.Move_Appropriate_Inbound_Packages_To_Processing: There are no incoming folders set in the database", "Standard", String.Empty, String.Empty, -1);
            }
            else
            {
                Add_NonError_To_Log("Worker_BulkLoader.Perform_BulkLoader: Begin processing builder folders", verbose, String.Empty, String.Empty, -1);

                foreach (Builder_Source_Folder folder in BuilderSettings.IncomingFolders)
                {
                    Actionable_Builder_Source_Folder actionFolder = new Actionable_Builder_Source_Folder(folder, BuilderSettings);

                    foreach (iFolderModule thisModule in actionFolder.BuilderModules)
                    {
                        // Check for abort
                        if (CheckForAbort())
                        {
                            Add_NonError_To_Log("Worker_BulkLoader.Perform_BulkLoader: Aborted (line 151)", verbose, String.Empty, String.Empty, -1);
                            finalmessage = "Aborted per database request";
                            ReleaseResources();
                            return false;
                        }

                        thisModule.DoWork(actionFolder, incoming_packages, deletes);
                    }
                }

                // Since all folder processing is complete, release resources
                foreach (iFolderModule thisModule in BuilderSettings.AllFolderModules)
                    thisModule.ReleaseResources();
            }
            

            // Check for abort
            if (CheckForAbort())
            {
                Add_NonError_To_Log("Worker_BulkLoader.Perform_BulkLoader: Aborted (line 179)", verbose, String.Empty, String.Empty, -1);
                finalmessage = "Aborted per database request";
                ReleaseResources();
                return false;
            }

            // If there were no packages to process further stop here
            if ((incoming_packages.Count == 0) && (deletes.Count == 0))
            {
	            Add_Complete_To_Log("No New Packages - Process Complete", "No Work", String.Empty, String.Empty, -1);
                if (finalmessage.Length == 0)
                    finalmessage = "No New Packages - Process Complete";
                ReleaseResources();
                return false;
            }

            // Iterate through all non-delete resources ready for processing
            Add_NonError_To_Log("Worker_BulkLoader.Perform_BulkLoader: Process any incoming packages", verbose, String.Empty, String.Empty, -1);
            Process_All_Incoming_Packages(incoming_packages);

            // Can now release these resources
            foreach (iSubmissionPackageModule thisModule in BuilderSettings.ItemProcessModules)
            {
                thisModule.ReleaseResources();
            }

            // Process any delete requests ( iterate through all deletes )
            Add_NonError_To_Log("Worker_BulkLoader.Perform_BulkLoader: Process any deletes", verbose, String.Empty, String.Empty, -1);
            Process_All_Deletes(deletes);

            // Can now release these resources
            foreach (iSubmissionPackageModule thisModule in BuilderSettings.DeleteItemModules)
            {
                thisModule.ReleaseResources();
            }


            // RUN ANY POST-PROCESSING MODULES HERE 
            if (BuilderSettings.PostProcessModules.Count > 0)
            {
                Add_NonError_To_Log("Running all post-processing steps", verbose, String.Empty, String.Empty, -1);
                foreach (iPostProcessModule thisModule in BuilderSettings.PostProcessModules)
                {
                    // Check for abort
                    if (CheckForAbort())
                    {
                        Abort_Database_Mechanism.Builder_Operation_Flag = Builder_Operation_Flag_Enum.ABORTING;
                        break;
                    }

                    thisModule.DoWork(aggregations_to_refresh, processed_items, deleted_items, settings);
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

            // Clear lots of collections and such from memory, since we are done processing
            ReleaseResources();


            Add_NonError_To_Log("Worker_BulkLoader.Perform_BulkLoader: Done", verbose, String.Empty, String.Empty, -1);

            return still_pending_items;

        }

        public void ReleaseResources()
        {
            // Set some things to NULL
            itemTable = null;
            aggregations_to_refresh.Clear();
            processed_items.Clear();
            deleted_items.Clear();

            // release all modules
            foreach (iFolderModule thisModule in BuilderSettings.AllFolderModules)
            {
                thisModule.ReleaseResources();
            }
            foreach (iSubmissionPackageModule thisModule in BuilderSettings.DeleteItemModules)
            {
                thisModule.ReleaseResources();
            }
            foreach (iSubmissionPackageModule thisModule in BuilderSettings.ItemProcessModules)
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
            // Disable the cache
            CachedDataManager.Settings.Disabled = true;

            Engine_Database.Connection_String = dbInstance.Connection_String;
            Resource_Object.Database.SobekCM_Database.Connection_String = dbInstance.Connection_String;
            Library.Database.SobekCM_Database.Connection_String = dbInstance.Connection_String;

            // Reload all the other data
            Engine_ApplicationCache_Gateway.RefreshAll(dbInstance);

            // Also, pull the engine configuration
            // Try to read the OAI-PMH configuration file
            if (File.Exists(Engine_ApplicationCache_Gateway.Settings.Base_Directory + "\\config\\user\\sobekcm_microservices.config"))
            {
                SobekEngineClient.Read_Config_File(Engine_ApplicationCache_Gateway.Settings.Base_Directory + "\\config\\user\\sobekcm_microservices.config", Engine_ApplicationCache_Gateway.Settings.System_Base_URL);
            }
            else if (File.Exists(Engine_ApplicationCache_Gateway.Settings.Base_Directory + "\\config\\default\\sobekcm_microservices.config"))
            {
                SobekEngineClient.Read_Config_File(Engine_ApplicationCache_Gateway.Settings.Base_Directory + "\\config\\default\\sobekcm_microservices.config", Engine_ApplicationCache_Gateway.Settings.System_Base_URL);
            }

		    if (settings == null)
		    {
	            Add_Error_To_Log("Unable to pull the newest settings from the database", String.Empty, String.Empty, -1);
                return false;
		    }

            // Save the item table
		    itemTable = SobekCM_Database.Get_Item_List(true, null).Tables[0];

            // get all the info
            settings = InstanceWide_Settings_Builder.Build_Settings(dbInstance);
            BuilderSettings = new Builder_Modules();
		    DataSet builderSettingsTbl = Engine_Database.Get_Builder_Settings(false, null);
		    if (builderSettingsTbl == null)
		    {
                Add_Error_To_Log("Unable to pull the newest BUILDER settings from the database", String.Empty, String.Empty, -1);
                return false;
		    }
		    if (!Builder_Settings_Builder.Refresh(BuilderSettings, builderSettingsTbl))
		    {
                Add_Error_To_Log("Error building the builder settings from the dataset", String.Empty, String.Empty, -1);
                return false; 
		    }
            List<string> errors = BuilderSettings.Builder_Modules_From_Settings();

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
            foreach (iPreProcessModule thisModule in BuilderSettings.PreProcessModules)
            {
                thisModule.Error += module_Error;
                thisModule.Process += module_Process;
            }
            foreach (iSubmissionPackageModule thisModule in BuilderSettings.DeleteItemModules)
            {
                thisModule.Error += module_Error;
                thisModule.Process += module_Process;
            }
            foreach (iSubmissionPackageModule thisModule in BuilderSettings.ItemProcessModules)
            {
                thisModule.Error += module_Error;
                thisModule.Process += module_Process;
            }
            foreach (iPostProcessModule thisModule in BuilderSettings.PostProcessModules)
            {
                thisModule.Error += module_Error;
                thisModule.Process += module_Process;
            }
            foreach (iFolderModule thisModule in BuilderSettings.AllFolderModules)
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
            DataTable additionalWorkRequired = SobekCM_Database.Items_Needing_Aditional_Work;
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
                    string resource_folder = settings.Image_Server_Network + file_root + "\\" + vid;

                    // Determine the METS file name
                    string mets_file = resource_folder + "\\" + bibID + "_" + vid + ".mets.xml";

                    // Ensure these both exist
                    if ((Directory.Exists(resource_folder)) && (File.Exists(mets_file)))
                    {
                        // Create the incoming digital resource object
                        Incoming_Digital_Resource additionalWorkResource = new Incoming_Digital_Resource(resource_folder, sourceFolder) 
							{BibID = bibID, VID = vid, File_Root = bibID.Substring(0, 2) + "\\" + bibID.Substring(2, 2) + "\\" + bibID.Substring(4, 2) + "\\" + bibID.Substring(6, 2) + "\\" + bibID.Substring(8, 2)};

	                    Complete_Single_Recent_Load_Requiring_Additional_Work( resource_folder, additionalWorkResource);
                    }
                    else
                    {
	                    Add_Error_To_Log("Unable to find valid resource files for reprocessing " + bibID + ":" + vid, bibID + ":" + vid, "Reprocess", -1);

	                    int itemID = SobekCM_Database.Get_ItemID_From_Bib_VID(bibID, vid);

						SobekCM_Database.Update_Additional_Work_Needed_Flag(itemID, false, null);
                    }
                }
            }
        }

        private void Complete_Single_Recent_Load_Requiring_Additional_Work(string Resource_Folder, Incoming_Digital_Resource AdditionalWorkResource)
        {
	        AdditionalWorkResource.METS_Type_String = "Reprocess";
            AdditionalWorkResource.BuilderLogId = Add_NonError_To_Log("........Reprocessing '" + AdditionalWorkResource.BibID + ":" + AdditionalWorkResource.VID + "'", "Standard",  AdditionalWorkResource.BibID + ":" + AdditionalWorkResource.VID, AdditionalWorkResource.METS_Type_String, -1);

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
                SobekCM_Database.Add_Minimum_Builder_Information(AdditionalWorkResource.Metadata);

                // Do all the item processing per instance config
                foreach (iSubmissionPackageModule thisModule in BuilderSettings.ItemProcessModules)
                {
                    if (verbose)
                    {
                        Add_NonError_To_Log("Running module " + thisModule.GetType().ToString(), true, AdditionalWorkResource.BibID + ":" + AdditionalWorkResource.VID, String.Empty, AdditionalWorkResource.BuilderLogId);
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

                    if (new_item_limit > 0)
                    {
                        number_packages_processed++;
                        if (number_packages_processed >= MultiInstance_Builder_Settings.Instance_Package_Limit)
                        {
                            still_pending_items = true;
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
			SobekCM_Database.Builder_Clear_Item_Error_Log(ResourcePackage.BibID, ResourcePackage.VID, "SobekCM Builder");

            // Before we save this or anything, let's see if this is truly a new resource
            ResourcePackage.NewPackage = !(itemTable.Select("BibID='" + ResourcePackage.BibID + "' and VID='" + ResourcePackage.VID + "'").Length > 0);
            ResourcePackage.Package_Time = DateTime.Now;

            try
            {
                // Do all the item processing per instance config
                foreach (iSubmissionPackageModule thisModule in BuilderSettings.ItemProcessModules)
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

                if (itemTable.Select("BibID='" + deleteResource.BibID + "' and VID='" + deleteResource.VID + "'").Length > 0)
                {
                    // Do all the item processing per instance config
                    foreach (iSubmissionPackageModule thisModule in BuilderSettings.DeleteItemModules)
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
				Console.WriteLine(instanceName + " - " + LogStatement);
				logger.AddNonError(instanceName + " - " + LogStatement.Replace("\t", "....."));
			}
			else
			{
				Console.WriteLine( LogStatement);
				logger.AddNonError( LogStatement.Replace("\t", "....."));
			}
			return SobekCM_Database.Builder_Add_Log_Entry(RelatedLogID, BibID_VID, DbLogType, LogStatement.Replace("\t", ""), MetsType);
        }

		private long Add_NonError_To_Log(string LogStatement, bool IsVerbose, string BibID_VID, string MetsType, long RelatedLogID)
        {
            if (IsVerbose)
            {
	            if (multiInstanceBuilder)
	            {
		            Console.WriteLine(instanceName + " - " + LogStatement);
		            logger.AddNonError(instanceName + " - " + LogStatement.Replace("\t", "....."));
	            }
	            else
				{
					Console.WriteLine( LogStatement);
					logger.AddNonError( LogStatement.Replace("\t", "....."));
				}
	            return SobekCM_Database.Builder_Add_Log_Entry(RelatedLogID, BibID_VID, "Verbose", LogStatement.Replace("\t", ""), MetsType);
            }
			return -1;
        }

		private long Add_Error_To_Log(string LogStatement, string BibID_VID, string MetsType, long RelatedLogID)
        {
			if (multiInstanceBuilder)
			{
				Console.WriteLine(instanceName + " - " + LogStatement);
				logger.AddError(instanceName + " - " + LogStatement.Replace("\t", "....."));
			}
			else
			{
				Console.WriteLine( LogStatement);
				logger.AddError( LogStatement.Replace("\t", "....."));
			}
			return SobekCM_Database.Builder_Add_Log_Entry(RelatedLogID, BibID_VID, "Error", LogStatement.Replace("\t", ""), MetsType);
        }

        private void Add_Error_To_Log(string LogStatement, string BibID_VID, string MetsType, long RelatedLogID, Exception Ee)
        {
            if (multiInstanceBuilder)
            {
                Console.WriteLine(instanceName + " - " + LogStatement);
                logger.AddError(instanceName + " - " + LogStatement.Replace("\t", "....."));
            }
            else
            {
                Console.WriteLine(LogStatement);
                logger.AddError(LogStatement.Replace("\t", "....."));
            }
            long mainErrorId = SobekCM_Database.Builder_Add_Log_Entry(RelatedLogID, BibID_VID, "Error", LogStatement.Replace("\t", ""), MetsType);


            string[] split = Ee.ToString().Split("\n".ToCharArray());
            foreach (string thisSplit in split)
            {
                SobekCM_Database.Builder_Add_Log_Entry(mainErrorId, BibID_VID, "Error", thisSplit, MetsType);
            }


            // Save the exception to an exception file
            StreamWriter exception_writer = new StreamWriter(settings.Local_Log_Directory + "\\exceptions_log.txt", true);
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
		        Console.WriteLine(instanceName + " - " + LogStatement);
		        logger.AddComplete(instanceName + " - " + LogStatement.Replace("\t", "....."));
	        }
	        else
			{
				Console.WriteLine( LogStatement);
				logger.AddComplete( LogStatement.Replace("\t", "....."));
			}
	        SobekCM_Database.Builder_Add_Log_Entry(RelatedLogID, BibID_VID, DbLogType, LogStatement.Replace("\t", ""), MetsType);
        }

        #endregion

        #region Methods used to get the list of collections to mark in db for build

        private void Add_Aggregation_To_Refresh_List(string Code)
        {
            // Only continue if there is length
            if (Code.Length > 1)
            {
                // This aggregation should be refreshed
                if (!aggregations_to_refresh.Contains(Code.ToUpper()))
                    aggregations_to_refresh.Add(Code.ToUpper());
            }
        }

        private void Add_Process_Info_To_PostProcess_Lists(string BibID, string VID, IEnumerable<string> Codes)
        {
            foreach (string collectionCode in Codes)
            {
                Add_Aggregation_To_Refresh_List(collectionCode);
            }
            processed_items.Add(new BibVidStruct(BibID, VID));
        }

        private void Add_Delete_Info_To_PostProcess_Lists(string BibID, string VID, IEnumerable<string> Codes)
        {
            foreach (string collectionCode in Codes)
            {
                Add_Aggregation_To_Refresh_List(collectionCode);
            }
            deleted_items.Add(new BibVidStruct(BibID, VID));
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
	        if (!canAbort)
		        return false;

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
