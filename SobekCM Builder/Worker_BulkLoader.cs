using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using SobekCM.Library.Settings;
using SobekCM.Resource_Object.Behaviors;
using SobekCM.Resource_Object.Metadata_Modules;
using SobekCM.Resource_Object.Utilities;
using SobekCM.Tools.Logs;
using SobekCM.Resource_Object;
using SobekCM.Library;
using SobekCM.Library.Application_State;
using SobekCM.Library.Database;
using SobekCM.Library.Builder;


namespace SobekCM.Builder
{
    /// <summary> Class is the worker thread for the main bulk loader processor </summary>
    public class Worker_BulkLoader
    {
        private DataSet incomingFileInstructions;
        private readonly Aggregation_Code_Manager codeManager;
        private readonly LogFileXHTML logger;
        private readonly SobekCM_METS_Validator thisMetsValidator;
        private readonly METS_Validator_Object metsSchemeValidator;
        private DataTable itemTable;
        private readonly List<string> aggregations_to_refresh;
        private bool aborted;
        private bool verbose;
        private readonly Static_Pages_Builder staticBuilder;
        private readonly Dictionary<string, List<string>> items_by_aggregation;

        private readonly string ghostscript_executable;
        private readonly string imagemagick_executable;
	    private string finalmessage;

        /// <summary> List of possible page image extensions </summary>
        private readonly List<string> PAGE_IMAGE_EXTENSIONS = new List<string>(new string[] { "JPG", "JP2", "JPX", "GIF", "PNG", "BMP", "JPEG" });

        /// <summary> Constructor for a new instance of the Worker_BulkLoader class </summary>
        /// <param name="Logger"> Log file object for logging progress </param>
        /// <param name="Verbose"> Flag indicates if the builder is in verbose mode, where it should log alot more information </param>
        public Worker_BulkLoader(LogFileXHTML Logger, bool Verbose )
        {
            // Save the log file and verbose flag
            logger = Logger;
            verbose = Verbose;

            Add_NonError_To_Log("Worker_BulkLoader.Constructor: Start", verbose);

            // Assign the database connection string
            SobekCM_Database.Connection_String = SobekCM_Library_Settings.Database_Connections[0].Connection_String;

            // Create the METS validation objects
            thisMetsValidator = new SobekCM_METS_Validator( String.Empty );
            metsSchemeValidator = new METS_Validator_Object(false);

            Add_NonError_To_Log("Worker_BulkLoader.Constructor: Created Validators", verbose);
 

            // Create new list of collections to build
            aggregations_to_refresh = new List<string>();
            items_by_aggregation = new Dictionary<string, List<string>>();

            // Create the new statics page builder
            staticBuilder = new Static_Pages_Builder(SobekCM_Library_Settings.Application_Server_URL, SobekCM_Library_Settings.Static_Pages_Location);

            Add_NonError_To_Log("Worker_BulkLoader.Constructor: Created Static Pages Builder", verbose);

            // Get the list of collection codes
            codeManager = new Aggregation_Code_Manager();
            SobekCM_Database.Populate_Code_Manager(codeManager, null);

            Add_NonError_To_Log("Worker_BulkLoader.Constructor: Populated code manager with " + codeManager.All_Aggregations.Count + " aggregations.", verbose);

            // Set some defaults
            aborted = false;

            // Get the executable path/file for ghostscript and imagemagick
            ghostscript_executable = SobekCM_Library_Settings.Ghostscript_Executable;
            imagemagick_executable = SobekCM_Library_Settings.ImageMagick_Executable;

            // Get the emails, which are used very rarely

	        Add_NonError_To_Log("Worker_BulkLoader.Constructor: Done", verbose);

        }

        /// <summary> Build any RSS or XML feeds for recently processed items </summary>
        public void Build_Feeds()
        {
            // Only build the feed if some aggregatios have recently been affected
            if (aggregations_to_refresh.Count > 0)
            {
                // Recreate any aggregation level XML or RSS feeds
                Recreate_Aggregation_XML_and_RSS();

                // Clear the list
                aggregations_to_refresh.Clear();

                // Update the library-wide XML and RSS feeds
                Recreate_Library_XML_and_RSS();
            }
        }

        #region Main Method that steps through each package and performs work

        /// <summary> Performs the bulk loader process and handles any incoming digital resources </summary>
        public void Perform_BulkLoader( bool Verbose )
        {
            verbose = Verbose;
            finalmessage = String.Empty;

            Add_NonError_To_Log("Worker_BulkLoader.Perform_BulkLoader: Start", verbose);

            // Refresh any settings and item lists 
            if (!Refresh_Settings_And_Item_List())
            {
                Add_Error_To_Log("Worker_BulkLoader.Perform_BulkLoader: Error refreshing settings and item list");
                finalmessage = "Error refreshing settings and item list";
                return;
            }

            // If not already verbose, check settings
            if (!verbose)
            {
                verbose = SobekCM_Library_Settings.Builder_Verbose_Flag;
            }

            Add_NonError_To_Log("Worker_BulkLoader.Perform_BulkLoader: Refreshed settings and item list", verbose);

            // Check for abort
            if (CheckForAbort()) 
            {
                Add_NonError_To_Log("Worker_BulkLoader.Perform_BulkLoader: Aborted (line 137)", verbose );
                finalmessage = "Aborted per database request";
                return; 
            }

            Add_NonError_To_Log("Worker_BulkLoader.Perform_BulkLoader: Begin completing any recent loads requiring additional work", verbose );

            // Handle all packages already on the web server which are flagged for additional work required
            Complete_Any_Recent_Loads_Requiring_Additional_Work();

            Add_NonError_To_Log("Worker_BulkLoader.Perform_BulkLoader: Finished completing any recent loads requiring additional work", verbose );

            // Check for abort
            if (CheckForAbort())
            {
                Add_NonError_To_Log("Worker_BulkLoader.Perform_BulkLoader: Aborted (line 151)", verbose );
                finalmessage = "Aborted per database request";
                return;
            }

            // Check for any new appropriately-aged packages in any inbound folders and move to processing
            Move_Appropriate_Inbound_Packages_To_Processing();

            // Check for abort
            if (CheckForAbort())
            {
                Add_NonError_To_Log("Worker_BulkLoader.Perform_BulkLoader: Aborted (line 161)", verbose);
                finalmessage = "Aborted per database request";
                return;
            }

            // Create the seperate queues for each type of incoming digital resource files
            List<Incoming_Digital_Resource> incoming_packages = new List<Incoming_Digital_Resource>();
            List<Incoming_Digital_Resource> deletes = new List<Incoming_Digital_Resource>();

            Add_NonError_To_Log("Worker_BulkLoader.Perform_BulkLoader: Begin validating and classifying packages in incoming/process folders", verbose);

            // Validate and classify all incoming digital resource folders/files
            Validate_And_Classify_Packages_In_Process_Folders(incoming_packages, deletes);

            Add_NonError_To_Log("Worker_BulkLoader.Perform_BulkLoader: Finished validating and classifying packages in incoming/process folders", verbose );

            // Check for abort
            if (CheckForAbort())
            {
                Add_NonError_To_Log("Worker_BulkLoader.Perform_BulkLoader: Aborted (line 179)", verbose);
                finalmessage = "Aborted per database request";
                return;
            }

            // If there were no packages to process further stop here
            if ((incoming_packages.Count == 0) && (deletes.Count == 0))
            {
                Add_Complete_To_Log("No New Packages - Process Complete");
                if (finalmessage.Length == 0)
                    finalmessage = "No New Packages - Process Complete";

                // Publish the log file
                File.Copy(logger.FileName, SobekCM_Library_Settings.Log_Files_Directory + "\\" + (new FileInfo(logger.FileName)).Name, true);

                return;
            }


            // Iterate through all non-delete resources ready for processing
            Add_NonError_To_Log("Worker_BulkLoader.Perform_BulkLoader: Process any incoming packages", verbose);
            Process_All_Incoming_Packages(incoming_packages);

            // Process any delete requests ( iterate through all deletes )
            Add_NonError_To_Log("Worker_BulkLoader.Perform_BulkLoader: Process any deletes", verbose);
            Process_All_Deletes(deletes);

            // Send emails to each location ( CURRENTLY SUSPENDED )
            send_emails_for_new_items();

            // Add the complete entry for the log
            if (!CheckForAbort())
            {
                Add_Complete_To_Log("Process Complete");
                if (finalmessage.Length == 0)
                    finalmessage = "Process completed successfully";
            }
            else
            {
                finalmessage = "Aborted per database request";
                Add_Complete_To_Log("Process Aborted Cleanly");
            }

            // Set some things to NULL
            itemTable = null;
            incomingFileInstructions = null;

            // Publish the log file
            Add_NonError_To_Log("Worker_BulkLoader.Perform_BulkLoader: Copying log to web directory", verbose);
            try
            {
                File.Copy(logger.FileName, SobekCM_Library_Settings.Log_Files_Directory + "\\" + (new FileInfo(logger.FileName)).Name, true);
            }
            catch (Exception)
            {
                finalmessage = "Error copying the log to " + SobekCM_Library_Settings.Log_Files_Directory + "\\" + (new FileInfo(logger.FileName)).Name;
            }

            Add_NonError_To_Log("Worker_BulkLoader.Perform_BulkLoader: Done", verbose );
        }

        #endregion

        #region Method to read and validate the METS file

	    /// <summary> Validates and reads the data from the METS file associated with this incoming digital resource </summary>
	    /// <param name="Resource"></param>
	    /// <param name="ThisMetsValidator"></param>
	    /// <param name="MetsSchemeValidator"></param>
	    /// <returns></returns>
	    public string Validate_and_Read_METS(Incoming_Digital_Resource Resource, SobekCM_METS_Validator ThisMetsValidator, METS_Validator_Object MetsSchemeValidator)
	    {
		    Add_NonError_To_Log("Worker_BulkLoader.Validate_and_Read_METS: Start ( " + Resource.Folder_Name + " )", verbose);

		    // Determine invalid bib id and vid for any errors
		    string invalid_bibid = String.Empty;
		    string invalid_vid = String.Empty;
		    if ((Resource.Folder_Name.Length == 16) && (Resource.Folder_Name[10] == '_'))
		    {
			    invalid_bibid = Resource.Folder_Name.Substring(0, 10);
			    invalid_vid = Resource.Folder_Name.Substring(11, 5);
		    }
		    else if (Resource.Folder_Name.Length == 10)
		    {
			    invalid_bibid = Resource.Folder_Name;
			    invalid_vid = "none";
		    }

		    // Verify that a METS file exists
		    Add_NonError_To_Log("Worker_BulkLoader.Validate_and_Read_METS: Check for METS existence", verbose);
		    if (ThisMetsValidator.METS_Existence_Check(Resource.Resource_Folder) == false)
		    {
			    // Save this error log and return the error
			    Create_Error_Log(Resource.Resource_Folder, Resource.Folder_Name, ThisMetsValidator.ValidationError);
			    Save_Validation_Errors_To_Database(invalid_bibid, invalid_vid, "UNKNOWN", ThisMetsValidator.ValidationError);
			    return ThisMetsValidator.ValidationError;
		    }

		    // Get the name of this METS file
		    string[] mets_files = Directory.GetFiles(Resource.Resource_Folder, invalid_bibid + "_" + invalid_vid + ".mets*");
		    string mets_file = String.Empty;
		    try
		    {
			    if (mets_files.Length == 0)
			    {
				    mets_file = Directory.GetFiles(Resource.Resource_Folder, "*mets*")[0];
			    }
			    else
			    {
				    mets_file = mets_files[0];
			    }
		    }
		    catch (Exception)
		    {
			    Create_Error_Log(Resource.Resource_Folder, Resource.Folder_Name, "Unable to locate correct METS file");
			    Save_Validation_Errors_To_Database(invalid_bibid, invalid_vid, "UNKNOWN", "Unable to locate correct METS file");
		    }
		    if (mets_file == String.Empty)
		    {
			    Create_Error_Log(Resource.Resource_Folder, Resource.Folder_Name, "Unable to locate correct METS file");
			    Save_Validation_Errors_To_Database(invalid_bibid, invalid_vid, "UNKNOWN", "Unable to locate correct METS file");
		    }


		    // check the mets file against the scheme
		    FileInfo metsFileInfo = new FileInfo(mets_file);
		    Add_NonError_To_Log("Worker_BulkLoader.Validate_and_Read_METS: Validate against " + metsFileInfo.Name + " against the schema", verbose);
		    if (MetsSchemeValidator.Validate_Against_Schema(mets_file) == false)
		    {
			    // Save this error log and return the error
			    Create_Error_Log(Resource.Resource_Folder, Resource.Folder_Name, MetsSchemeValidator.Errors);
			    Save_Validation_Errors_To_Database(invalid_bibid, invalid_vid, "UNKNOWN", "METS Scheme Validation Error");
			    return MetsSchemeValidator.Errors;
		    }

		    SobekCM_Item returnValue;
		    try
		    {
			    Add_NonError_To_Log("Worker_BulkLoader.Validate_and_Read_METS: Read validated METS file", verbose);

			    returnValue = SobekCM_Item.Read_METS(mets_file);
		    }
		    catch
		    {
			    // Save this error log and return the error
			    Create_Error_Log(Resource.Resource_Folder, Resource.Folder_Name, "Error encountered while reading the METS file '" + mets_file);
			    Save_Validation_Errors_To_Database(invalid_bibid, invalid_vid, "UNKNOWN", "Error encountered while reading the METS file '" + mets_file);
			    return "Error encountered while reading the METS file '" + mets_file;
		    }

		    // If the METS file was read, determine if this is valid by the METS type
		    if (returnValue != null)
		    {
			    // If there is a bibid and no vid, check to see what's going on here
			    if ((returnValue.BibID.Length == 10) && (returnValue.VID.Length == 0))
			    {
				    DataRow[] matches = itemTable.Select("BibID='" + returnValue.BibID + "'");
				    if (matches.Length == 0)
				    {
					    returnValue.VID = "00001";
				    }
				    else
				    {
					    if ((matches.Length == 1) && (matches[0]["VID"].ToString() == "00001"))
						    returnValue.VID = "00001";
					    else
					    {
						    // Save this error log and return the error
						    Create_Error_Log(Resource.Resource_Folder, Resource.Folder_Name, "METS file does not have a VID and belongs to a multi-volume title");
						    Save_Validation_Errors_To_Database(invalid_bibid, invalid_vid, "UNKNOWN", "METS file does not have a VID and belongs to a multi-volume title");
						    return "METS file does not have a VID and belongs to a multi-volume title";
					    }
				    }
			    }


			    // Do the basic check first
			    Add_NonError_To_Log("Worker_BulkLoader.Validate_and_Read_METS: Perform basic check", verbose);
			    if (!ThisMetsValidator.SobekCM_Standard_Check(returnValue, Resource.Resource_Folder))
			    {
				    // Save this error log and return null
				    Create_Error_Log(Resource.Resource_Folder, Resource.Folder_Name, ThisMetsValidator.ValidationError);
				    Save_Validation_Errors_To_Database(invalid_bibid, invalid_vid, "UNKNOWN", ThisMetsValidator.ValidationError);
				    return ThisMetsValidator.ValidationError;
			    }

			    // If this is a COMPLETE package, check files
			    if (returnValue.METS_Header.RecordStatus_Enum == METS_Record_Status.COMPLETE)
			    {
				    Add_NonError_To_Log("Worker_BulkLoader.Validate_and_Read_METS: Check resource files (existence and checksum)", verbose);

				    // check if all files exist in the package and the MD5 checksum if the checksum flag is true		
				    if (!ThisMetsValidator.Check_Files(Resource.Resource_Folder, SobekCM_Library_Settings.VerifyCheckSum))
				    {
					    // Save this error log and return null
					    Create_Error_Log(Resource.Resource_Folder, Resource.Folder_Name, ThisMetsValidator.ValidationError);
					    Save_Validation_Errors_To_Database(invalid_bibid, invalid_vid, "UNKNOWN", ThisMetsValidator.ValidationError);
					    return ThisMetsValidator.ValidationError;
				    }
			    }

			    // This is apparently valid, so do some final checks and copying into the resource wrapper
			    Resource.BibID = returnValue.BibID;
			    Resource.VID = returnValue.VID;

			    switch (returnValue.METS_Header.RecordStatus_Enum)
			    {
				    case METS_Record_Status.METADATA_UPDATE:
					    Resource.Resource_Type = Incoming_Digital_Resource.Incoming_Digital_Resource_Type.METADATA_UPDATE;
					    break;

				    case METS_Record_Status.COMPLETE:
					    Resource.Resource_Type = Incoming_Digital_Resource.Incoming_Digital_Resource_Type.COMPLETE_PACKAGE;
					    break;

				    case METS_Record_Status.PARTIAL:
					    Resource.Resource_Type = Incoming_Digital_Resource.Incoming_Digital_Resource_Type.PARTIAL_PACKAGE;
					    break;

				    case METS_Record_Status.DELETE:
					    Resource.Resource_Type = Incoming_Digital_Resource.Incoming_Digital_Resource_Type.DELETE;
					    break;

				    case METS_Record_Status.BIB_LEVEL:
					    Resource.Resource_Type = Incoming_Digital_Resource.Incoming_Digital_Resource_Type.BIB_LEVEL;
					    break;
			    }

			    Add_NonError_To_Log("Worker_BulkLoader.Validate_and_Read_METS: Complete - validated", verbose);
			    return String.Empty;
		    }
		    

		    // Save this error log and return the error
		    Create_Error_Log(Resource.Resource_Folder, Resource.Folder_Name, "Error encountered while reading the METS file '" + mets_file);
		    Save_Validation_Errors_To_Database(invalid_bibid, invalid_vid, "UNKNOWN", "Error encountered while reading the METS file '" + mets_file);
		    return "Error encountered while reading the METS file '" + mets_file;
	    }

	    private void Save_Validation_Errors_To_Database(string BIBID, string VID, string METSType, string ValidationErrors)
        {
            if ((String.IsNullOrEmpty(BIBID)) || (String.IsNullOrEmpty(VID)))
                return;

            string[] split_validation_errors = ValidationErrors.Split(new char[] { '\n' });
            int error_count = 0;
            foreach (string thisError in split_validation_errors)
            {
                if (thisError.Trim().Length > 0)
                {
                    SobekCM_Database.Add_Item_Error_Log(BIBID, VID, METSType, thisError.Trim());
                    error_count++;
                    if (error_count == 5)
                    {
                        SobekCM_Database.Add_Item_Error_Log(BIBID, VID, METSType, "(more errors");
                        break;
                    }
                }
            }
        }

	    /// <summary>Private method used to generate the error log for the packages</summary>
	    /// <param name="Folder_Name"></param>
	    /// <param name="ErrorMessage"></param>
	    /// <param name="Resource_Folder"></param>
	    private void Create_Error_Log( string Resource_Folder, string Folder_Name, string ErrorMessage)
        {
            // Split the message into seperate lines
            string[] errors = ErrorMessage.Split(new char[] { '\n' });

            try
            {
                LogFileXHTML errorLog = new LogFileXHTML(Resource_Folder + "\\" + Folder_Name + ".log.html", "Package Processing Log", "UFDC Pre-Loader");
                errorLog.New();
                errorLog.AddComplete("Error Log for " + Folder_Name + " processed at: " + DateTime.Now.ToString());
                errorLog.AddComplete("");

                foreach (string thisError in errors)
                {
                    errorLog.AddError(thisError);
                    Add_Error_To_Log(thisError);
                }
                errorLog.Close();
            }
            catch
            {

            }
        }

        #endregion

        #region Refresh any settings and item lists 

        private bool Refresh_Settings_And_Item_List()
        {
            // Do some preparation work
            items_by_aggregation.Clear();

            // Pull the settings again
            incomingFileInstructions = SobekCM_Database.Get_Builder_Settings_Complete(null);

            // Reload the settings
            if ((incomingFileInstructions == null) || (!SobekCM_Library_Settings.Refresh(incomingFileInstructions)))
            {
                Add_Error_To_Log("Unable to pull the newest settings from the database");
                return false;
            }

            // Save the item table
            itemTable = incomingFileInstructions.Tables[2];

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
                Add_NonError_To_Log("\tProcessing recently loaded items needing additional work");

                // Create the incoming digital folder object which will be used for all these
                Builder_Source_Folder sourceFolder = new Builder_Source_Folder();

                // Step through each one
                foreach (DataRow thisRow in additionalWorkRequired.Rows)
                {
                    // Get the information about this item
                    string bibID = thisRow["BibID"].ToString();
                    string vid = thisRow["VID"].ToString();

	                // Determine the file root for this
                    string file_root = bibID.Substring(0, 2) + "\\" + bibID.Substring(2, 2) + "\\" + bibID.Substring(4, 2) + "\\" + bibID.Substring(6, 2) + "\\" + bibID.Substring(8, 2);

                    // Determine the source folder for this resource
                    string resource_folder = SobekCM_Library_Settings.Image_Server_Network + file_root + "\\" + vid;

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
                }
            }
        }

        private void Complete_Single_Recent_Load_Requiring_Additional_Work(string Resource_Folder, Incoming_Digital_Resource AdditionalWorkResource)
        {
            Add_NonError_To_Log("\t\tProcessing '" + AdditionalWorkResource.BibID + ":" + AdditionalWorkResource.VID + "'");

            try
            {
                // Pre-Process any resource files
                List<string> new_image_files = new List<string>();
                PreProcess_Any_Resource_Files(AdditionalWorkResource, new_image_files);

                // Delete any pre-archive deletes
                if (SobekCM_Library_Settings.PreArchive_Files_To_Delete.Length > 0)
                {
                    // Get the list of files again
                    string[] files = Directory.GetFiles(Resource_Folder);
                    foreach (string thisFile in files)
                    {
                        FileInfo thisFileInfo = new FileInfo(thisFile);
                        if (System.Text.RegularExpressions.Regex.Match(thisFileInfo.Name, SobekCM_Library_Settings.PreArchive_Files_To_Delete, System.Text.RegularExpressions.RegexOptions.IgnoreCase).Success)
                        {
                            File.Delete(thisFile);
                        }
                    }
                }

                // Archive any files, per the folder instruction
                Archive_Any_Files(AdditionalWorkResource);

                // Delete any remaining post-archive deletes
                if (SobekCM_Library_Settings.PostArchive_Files_To_Delete.Length > 0)
                {
                    // Get the list of files again
                    string[] files = Directory.GetFiles(Resource_Folder);
                    foreach (string thisFile in files)
                    {
                        FileInfo thisFileInfo = new FileInfo(thisFile);
                        if (System.Text.RegularExpressions.Regex.Match(thisFileInfo.Name, SobekCM_Library_Settings.PostArchive_Files_To_Delete, System.Text.RegularExpressions.RegexOptions.IgnoreCase).Success)
                        {
                            File.Delete(thisFile);
                        }
                    }
                }

                // Load the METS file
                if ((!AdditionalWorkResource.Load_METS()) || ( AdditionalWorkResource.BibID.Length == 0 ))
                {
                    Add_Error_To_Log("Error reading METS file from " + AdditionalWorkResource.BibID + ":" + AdditionalWorkResource.VID);
                    SobekCM_Database.Add_Item_Error_Log(AdditionalWorkResource.BibID, AdditionalWorkResource.VID, AdditionalWorkResource.METS_Type_String, "Error reading METS file");
	                return;
                }

                // Add thumbnail and aggregation informaiton from the database 
                SobekCM_Database.Add_Minimum_Builder_Information(AdditionalWorkResource.Metadata);

                // Perform any final file updates
                Resource_File_Updates(AdditionalWorkResource, new_image_files, false );

                // Save all the metadata files again for any possible changes which may have occurred
                Save_All_Updated_Metadata_Files(AdditionalWorkResource);

                // Determine total size on the disk
                string[] all_files_final = Directory.GetFiles(AdditionalWorkResource.Resource_Folder);
                double size = all_files_final.Sum(ThisFile => (double) (((new FileInfo(ThisFile)).Length)/1024));
	            AdditionalWorkResource.DiskSpace_MB = size;

                // Save this package to the database
                if (!AdditionalWorkResource.Save_to_Database(itemTable, false))
                {
                    Add_Error_To_Log("Error saving data to SobekCM database.  The database may not reflect the most recent data in the METS.");
                }

                // Save this to the Solr/Lucene database
                if (SobekCM_Library_Settings.Document_Solr_Index_URL.Length > 0)
                {
                    try
                    {
                        Library.Solr.Solr_Controller.Update_Index(SobekCM_Library_Settings.Document_Solr_Index_URL, SobekCM_Library_Settings.Page_Solr_Index_URL, AdditionalWorkResource.Metadata, true);
                    }
                    catch (Exception ee)
                    {
                        Add_Error_To_Log("Error saving data to the Solr/Lucene index.  The index may not reflect the most recent data in the METS.");
						Add_Error_To_Log("Solr Error: " + ee.Message );

                    }
                }

                // Save the static page and then copy to all the image servers
                string static_file = AdditionalWorkResource.Save_Static_HTML(staticBuilder);
                if ((static_file.Length == 0) || (!File.Exists(static_file)))
                {
                    Add_Error_To_Log("Error creating static page for this resource");
                }
                else
                {
                    // Also copy to the static page location server
                    string web_server_file_version = SobekCM_Library_Settings.Static_Pages_Location + AdditionalWorkResource.File_Root + "\\" + AdditionalWorkResource.BibID + "_" + AdditionalWorkResource.VID + ".html";
                    if (!Directory.Exists(SobekCM_Library_Settings.Static_Pages_Location + AdditionalWorkResource.File_Root))
                        Directory.CreateDirectory(SobekCM_Library_Settings.Static_Pages_Location + AdditionalWorkResource.File_Root);
                    File.Copy(static_file, web_server_file_version, true);
                }

                // Save these collections to mark them for refreshing the RSS feeds, etc..
                Add_Aggregation_To_Refresh_List(AdditionalWorkResource.Metadata.Behaviors.Aggregation_Code_List);

                // Clear the flag for additional work
                SobekCM_Database.Update_Additional_Work_Needed_Flag(AdditionalWorkResource.Metadata.Web.ItemID, false, null);
    
                // Mark a log in the database that this was handled as well
                Resource_Object.Database.SobekCM_Database.Add_Workflow(AdditionalWorkResource.Metadata.Web.ItemID, "Post-Processed", String.Empty, "SobekCM Bulk Loader", String.Empty);

                // If the item is born digital, has files, and is currently public, close out the digitization milestones completely
                if ((!AdditionalWorkResource.Metadata.Tracking.Born_Digital_Is_Null) && (AdditionalWorkResource.Metadata.Tracking.Born_Digital) && (AdditionalWorkResource.Metadata.Behaviors.IP_Restriction_Membership >= 0) && (AdditionalWorkResource.Metadata.Divisions.Download_Tree.Has_Files))
                {
                    Resource_Object.Database.SobekCM_Database.Update_Digitization_Milestone(AdditionalWorkResource.Metadata.Web.ItemID, 4, DateTime.Now);
                }

                // Finally, clear the memory a little bit
                AdditionalWorkResource.Clear_METS();
            }
            catch (Exception ee)
            {
                Add_Error_To_Log("Unable to complete additional work for " + AdditionalWorkResource.BibID + ":" + AdditionalWorkResource.VID, ee);
            }
        }

        #endregion

        #region Process any pending FDA reports from the FDA Report DropBox

        /// <summary> Check for any FDA/DAITSS reports which were dropped into a FDA/DAITSS report drop box </summary>
        public void Process_Any_Pending_FDA_Reports()
        {
            // Step through each incoming folder and look for FDA reports
	        if ((SobekCM_Library_Settings.FDA_Report_DropBox.Length > 0) && (Directory.Exists(SobekCM_Library_Settings.FDA_Report_DropBox)))
            {
                // Create the FDA process
                FDA_Report_Processor fdaProcessor = new FDA_Report_Processor();

                // Process all pending FDA reports
                fdaProcessor.Process(SobekCM_Library_Settings.FDA_Report_DropBox);

                // Log successes and failures
                if ((fdaProcessor.Error_Count > 0) || (fdaProcessor.Success_Count > 0))
                {
	                // Clear any previous report
                    SobekCM_Database.Clear_Item_Error_Log("FDA REPORT", "", "SobekCM Builder");

                    if (fdaProcessor.Error_Count > 0)
                    {
                        Add_Error_To_Log("Processed " + fdaProcessor.Success_Count + " FDA reports with " + fdaProcessor.Error_Count + " errors");
                        SobekCM_Database.Add_Item_Error_Log("FDA Report", "", "N/A", fdaProcessor.Error_Count + " errors encountered");
                    }
                    else
                    {
                        Add_NonError_To_Log("Processed " + fdaProcessor.Success_Count + " FDA reports");
                    }
                }
            }
        }

        #endregion

        #region Check for any new appropriately-aged packages in any inbound folders and move to processing

        private void Move_Appropriate_Inbound_Packages_To_Processing()
        {
            // What if there are no folder?
            if (SobekCM_Library_Settings.Incoming_Folders.Count == 0)
            {
                Add_NonError_To_Log("Worker_BulkLoader.Move_Appropriate_Inbound_Packages_To_Processing: There are no incoming folders set in the database!" );
                finalmessage = "There are no incoming folders set in the database!";
            }

            try
            {
                // Move all eligible packages from the FTP folders to the processing folders
                bool anyInboundFilesExist = false;
                foreach (Builder_Source_Folder folder in SobekCM_Library_Settings.Incoming_Folders)
                {
                    Add_NonError_To_Log("Worker_BulkLoader.Move_Appropriate_Inbound_Packages_To_Processing: Checking incoming folder " +  folder.Inbound_Folder, verbose );

                    if (folder.Items_Exist_In_Inbound)
                    {
                        Add_NonError_To_Log("Worker_BulkLoader.Move_Appropriate_Inbound_Packages_To_Processing: Found either files or subdirectories in " + folder.Inbound_Folder, verbose);

                        anyInboundFilesExist = true;
                        break;
                    }
	                
					Add_NonError_To_Log("Worker_BulkLoader.Move_Appropriate_Inbound_Packages_To_Processing: No subdirectories or files found in incoming folder " + folder.Inbound_Folder, verbose);
                }

                if (anyInboundFilesExist)
                {
                    Add_NonError_To_Log("Checking inbound packages for aging and possibly moving to processing", verbose);

                    bool errorMoving = false;
                    foreach (Builder_Source_Folder folder in SobekCM_Library_Settings.Incoming_Folders)
                    {
                        String outMessage;
                        if (!folder.Move_From_Inbound_To_Processing(out outMessage))
                        {
                            if ( outMessage.Length > 0 ) Add_Error_To_Log(outMessage);
                            errorMoving = true;
                        }
                        else
                        {
                            if (outMessage.Length > 0) Add_NonError_To_Log(outMessage, verbose);
                        }

                    }
                    if (errorMoving)
                        Add_Error_To_Log("Unspecified error moving files from inbound to processing");
                }
            }
            catch (Exception ee)
            {
                Add_Error_To_Log("Error in harvesting packages from inbound folders to processing", ee);
            }
        }

        #endregion

        #region Validate and classify all incoming digital resource folders/files

        private void Validate_And_Classify_Packages_In_Process_Folders(List<Incoming_Digital_Resource> IncomingPackages, List<Incoming_Digital_Resource> Deletes)
        {
            try
            {
                // Step through each incomig folder
	            foreach (Builder_Source_Folder ftpFolder in SobekCM_Library_Settings.Incoming_Folders)
                {
                    // Only continue if the processing folder exists and has subdirectories
                    if ((Directory.Exists(ftpFolder.Processing_Folder)) && (Directory.GetDirectories(ftpFolder.Processing_Folder).Length > 0))
                    {
                        // Get the list of all packages in the processing folder
                        Add_NonError_To_Log("\tValidate packages for " + ftpFolder.Folder_Name, verbose );
                        List<Incoming_Digital_Resource> packages = ftpFolder.Packages_For_Processing;

                        // Step through each package
                        foreach (Incoming_Digital_Resource resource in packages)
                        {
                            // Validate the categorize the package
                            Add_NonError_To_Log("\t\tChecking '" + resource.Folder_Name + "'", verbose );

                            // If there is no METS file, use special code to check this
                            if (Directory.GetFiles(resource.Resource_Folder, "*.mets*").Length == 0)
                            {
                                DirectoryInfo noMetsDirInfo = new DirectoryInfo(resource.Resource_Folder);
                                string vid = noMetsDirInfo.Name;
                                string bibid = noMetsDirInfo.Parent.Name;
                                if ((vid.Length == 16) && (vid[10] == '_'))
                                {
                                    bibid = vid.Substring(0, 10);
                                    vid = vid.Substring(11, 5);
                                }

                                // Is this allowed in this incomnig folder?
                                if (ftpFolder.Allow_Folders_No_Metadata)
                                {
                                    DataRow[] selected = itemTable.Select("BibID='" + bibid + "' and VID='" + vid + "'");
                                    if (selected.Length > 0)
                                    {
                                        resource.BibID = bibid;
                                        resource.VID = vid;
                                        resource.Resource_Type = Incoming_Digital_Resource.Incoming_Digital_Resource_Type.FOLDER_OF_FILES;
                                        IncomingPackages.Add(resource);
                                    }
                                    else
                                    {
                                        SobekCM_Database.Add_Item_Error_Log(bibid, vid, "MISSING", "METS-less folder is not a valid BibID/VID combination");
                                        // Move this resource
                                        if (!resource.Move(ftpFolder.Failures_Folder))
                                        {
                                            Add_Error_To_Log("Unable to move folder " + resource.Folder_Name);
                                            SobekCM_Database.Add_Item_Error_Log(resource.BibID, resource.VID, resource.METS_Type_String, "Unable to move folder during load");
                                        }
                                    }
                                }
                                else
                                {
                                    SobekCM_Database.Add_Item_Error_Log(bibid, vid, "MISSING", "METS-less folder is not allowed");

                                    // Move this resource
                                    if (!resource.Move(ftpFolder.Failures_Folder))
                                    {
                                        Add_Error_To_Log("Unable to move folder " + resource.Folder_Name);
                                        SobekCM_Database.Add_Item_Error_Log(resource.BibID, resource.VID, resource.METS_Type_String, "Unable to move folder during load");
                                    }
                                }
                            }
                            else
                            {
                                Add_NonError_To_Log("\tValidating METS file for " + resource.Source_Folder, verbose);
                                string validation_errors = Validate_and_Read_METS( resource, thisMetsValidator, metsSchemeValidator);

                                // Save any errors to the main log
                                if (validation_errors.Length > 0)
                                {
                                    // Save the validation errors to the main log
                                    Save_Validation_Errors_To_Log(validation_errors);

                                    // Move this resource
                                    if (!resource.Move(ftpFolder.Failures_Folder))
                                    {
                                        Add_Error_To_Log("Unable to move folder " + resource.Folder_Name);
                                        SobekCM_Database.Add_Item_Error_Log(resource.BibID, resource.VID, resource.METS_Type_String, "Unable to move folder during load");
                                    }
                                }
                                else
                                {
                                    // Categorize remaining packages by type
                                    switch (resource.Resource_Type)
                                    {
                                        case Incoming_Digital_Resource.Incoming_Digital_Resource_Type.PARTIAL_PACKAGE:
                                        case Incoming_Digital_Resource.Incoming_Digital_Resource_Type.COMPLETE_PACKAGE:
                                            IncomingPackages.Add(resource);
                                            break;

                                        case Incoming_Digital_Resource.Incoming_Digital_Resource_Type.METADATA_UPDATE:
                                            if (ftpFolder.Allow_Metadata_Updates)
                                            {
                                                IncomingPackages.Add(resource);
                                            }
                                            else
                                            {
                                                SobekCM_Database.Add_Item_Error_Log(resource.BibID, resource.VID, "METADATA UPDATE", "Metadata update is not allowed");

                                                // Move this resource
                                                if (!resource.Move(ftpFolder.Failures_Folder))
                                                {
                                                    Add_Error_To_Log("Unable to move folder " + resource.Folder_Name);
                                                    SobekCM_Database.Add_Item_Error_Log(resource.BibID, resource.VID, resource.METS_Type_String, "Unable to move folder during load");
                                                }
                                            }
                                            break;

                                        case Incoming_Digital_Resource.Incoming_Digital_Resource_Type.DELETE:
                                            if (ftpFolder.Allow_Deletes)
                                            {
                                                Deletes.Add(resource);
                                            }
                                            else
                                            {
                                                SobekCM_Database.Add_Item_Error_Log(resource.BibID, resource.VID, resource.METS_Type_String, "Delete is not allowed");

                                                // Move this resource
                                                if (!resource.Move(ftpFolder.Failures_Folder))
                                                {
                                                    Add_Error_To_Log("Unable to move folder " + resource.Folder_Name);
                                                    SobekCM_Database.Add_Item_Error_Log(resource.BibID, resource.VID, resource.METS_Type_String, "Unable to move folder during load");
                                                }
                                            }
                                            break;
                                    }
                                }
                            }
                        }
                    }

                    // Check for abort
                    if (CheckForAbort())
                    {
                        return;
                    }
                }
            }
            catch (Exception ee)
            {
                Add_Error_To_Log("Error in harvesting packages from processing", ee);
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
                // Step through each package and handle all the files and metadata
                Add_NonError_To_Log("\tProcessing incoming packages");
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

                }
            }
            catch (Exception ee)
            {
                StreamWriter errorWriter = new StreamWriter(System.Windows.Forms.Application.StartupPath + "\\Logs\\error.log", true);
                errorWriter.WriteLine("Message: " + ee.Message);
                errorWriter.WriteLine("Stack Trace: " + ee.StackTrace);
                errorWriter.Flush();
                errorWriter.Close();

                Add_Error_To_Log("Unable to process all of the NEW and REPLACEMENT packages.", ee);
            }
        }

        private void Process_Single_Incoming_Package(Incoming_Digital_Resource ResourcePackage)
        {

            Add_NonError_To_Log("\t\tProcessing '" + ResourcePackage.Folder_Name + "'");

            // Clear any existing error linked to this item
            SobekCM_Database.Clear_Item_Error_Log(ResourcePackage.BibID, ResourcePackage.VID, "SobekCM Builder");

            try
            {
                // Pre-Process any resource files
                List<string> new_image_files = new List<string>();
                PreProcess_Any_Resource_Files(ResourcePackage, new_image_files);

                // Rename the received METS files
                Rename_Any_Received_METS_File(ResourcePackage);

                // Delete any pre-archive deletes
                if (SobekCM_Library_Settings.PreArchive_Files_To_Delete.Length > 0)
                {
                    // Get the list of files again
                    string[] files = Directory.GetFiles(ResourcePackage.Resource_Folder);
                    foreach (string thisFile in files)
                    {
                        FileInfo thisFileInfo = new FileInfo(thisFile);
                        if (System.Text.RegularExpressions.Regex.Match(thisFileInfo.Name, SobekCM_Library_Settings.PreArchive_Files_To_Delete, System.Text.RegularExpressions.RegexOptions.IgnoreCase).Success)
                        {
                            File.Delete(thisFile);
                        }
                    }
                }

                // Archive any files, per the folder instruction
                Archive_Any_Files(ResourcePackage);

                // Delete any remaining post-archive deletes
                if (SobekCM_Library_Settings.PostArchive_Files_To_Delete.Length > 0)
                {
                    // Get the list of files again
                    string[] files = Directory.GetFiles(ResourcePackage.Resource_Folder);
                    foreach (string thisFile in files)
                    {
                        FileInfo thisFileInfo = new FileInfo(thisFile);
                        if (System.Text.RegularExpressions.Regex.Match(thisFileInfo.Name, SobekCM_Library_Settings.PostArchive_Files_To_Delete, System.Text.RegularExpressions.RegexOptions.IgnoreCase).Success)
                        {
                            File.Delete(thisFile);
                        }
                    }
                }

                // Clear the list of new images files here, since moving the package will recalculate this
                new_image_files.Clear();

                // Move all files to the image server
                if (!Move_All_Files_To_Image_Server(ResourcePackage, new_image_files))
                {
                    Add_Error_To_Log("Error moving some files to the image server for " + ResourcePackage.BibID + ":" + ResourcePackage.VID);
                    SobekCM_Database.Add_Item_Error_Log(ResourcePackage.BibID, ResourcePackage.VID, ResourcePackage.METS_Type_String, "Error encountered moving some files to image server");
                }

                // Before we save this or anything, let's see if this is truly a new resource
                bool truly_new_bib = !(itemTable.Select("BibID='" + ResourcePackage.BibID + "' and VID='" + ResourcePackage.VID + "'").Length > 0);
                ResourcePackage.Package_Time = DateTime.Now;

                // Load the METS file
                if (!ResourcePackage.Load_METS())
                {
                    Add_Error_To_Log("Error reading METS file from " + ResourcePackage.BibID + ":" + ResourcePackage.VID);
                    SobekCM_Database.Add_Item_Error_Log(ResourcePackage.BibID, ResourcePackage.VID, ResourcePackage.METS_Type_String, "Error reading METS file");
	                return;
                }

                // Add thumbnail, aggregation informaiton, and dark/access information from the database 
                if (!truly_new_bib)
                {
                    SobekCM_Database.Add_Minimum_Builder_Information(ResourcePackage.Metadata);
                }
                else
                {
                    // Check for any access/restriction/embargo date in the RightsMD section
                    RightsMD_Info rightsInfo = ResourcePackage.Metadata.Get_Metadata_Module(GlobalVar.PALMM_RIGHTSMD_METADATA_MODULE_KEY) as RightsMD_Info;
                    if (( rightsInfo != null ) && ( rightsInfo.hasData ))
                    {
                        switch (rightsInfo.Access_Code)
                        {
                            case RightsMD_Info.AccessCode_Enum.Campus:
                                // Was there an embargo date?
                                if (rightsInfo.Has_Embargo_End)
                                {
                                    if (DateTime.Compare(DateTime.Now, rightsInfo.Embargo_End) < 0)
                                    {
                                        ResourcePackage.Metadata.Behaviors.IP_Restriction_Membership = 1;
                                    }
                                }
                                else
                                {
                                    ResourcePackage.Metadata.Behaviors.IP_Restriction_Membership = 1;
                                }
                                break;

                            case RightsMD_Info.AccessCode_Enum.Private:
                                // Was there an embargo date?
                                if (rightsInfo.Has_Embargo_End)
                                {
                                    if (DateTime.Compare(DateTime.Now, rightsInfo.Embargo_End) < 0)
                                    {
                                        ResourcePackage.Metadata.Behaviors.Dark_Flag = true;
                                    }
                                }
                                else
                                {
                                    ResourcePackage.Metadata.Behaviors.Dark_Flag = true;
                                }
                                break;
                        }
                    }
                }

                // Perform any final file updates
                Resource_File_Updates(ResourcePackage, new_image_files, truly_new_bib);

                // Save all the metadata files again for any possible changes which may have occurred
                Save_All_Updated_Metadata_Files(ResourcePackage);

                // Determine total size on the disk
                string[] all_files_final = Directory.GetFiles(ResourcePackage.Resource_Folder);
                double size = all_files_final.Sum(ThisFile => (double) (((new FileInfo(ThisFile)).Length)/1024));
	            ResourcePackage.DiskSpace_MB = size;

                // Save this package to the database
                if (!ResourcePackage.Save_to_Database(itemTable, truly_new_bib))
                {
                    Add_Error_To_Log("Error saving data to SobekCM database.  The database may not reflect the most recent data in the METS.");
                    SobekCM_Database.Add_Item_Error_Log(ResourcePackage.BibID, ResourcePackage.VID, ResourcePackage.METS_Type_String, "Error saving to SobekCM database");
                }

                // Save this to the Solr/Lucene database
                if (SobekCM_Library_Settings.Document_Solr_Index_URL.Length > 0)
                {
                    try
                    {
                        Library.Solr.Solr_Controller.Update_Index(SobekCM_Library_Settings.Document_Solr_Index_URL, SobekCM_Library_Settings.Page_Solr_Index_URL, ResourcePackage.Metadata, true);
                    }
                    catch (Exception ee)
                    {
                        Add_Error_To_Log("Error saving data to the Solr/Lucene index.  The index may not reflect the most recent data in the METS.");
	                    Add_Error_To_Log("Solr Error: " + ee.Message);
 
                        SobekCM_Database.Add_Item_Error_Log(ResourcePackage.BibID, ResourcePackage.VID, ResourcePackage.METS_Type_String, "Error saving to Solr/Lucene index");
                    }
                }

                // Save the static page and then copy to all the image servers
                string static_file = ResourcePackage.Save_Static_HTML(staticBuilder);
                if ((static_file.Length == 0) || (!File.Exists(static_file)))
                {
                    Add_Error_To_Log("Warning: error encountered creating static page for this resource");
                    SobekCM_Database.Add_Item_Error_Log(ResourcePackage.BibID, ResourcePackage.VID, ResourcePackage.METS_Type_String, "Warning: error creating static page");
                }
                else
                {
                    // Also copy to the static page location server
                    string web_server_file_version = SobekCM_Library_Settings.Static_Pages_Location + ResourcePackage.File_Root + "\\" + ResourcePackage.BibID + "_" + ResourcePackage.VID + ".html";
                    if (!Directory.Exists(SobekCM_Library_Settings.Static_Pages_Location + ResourcePackage.File_Root))
                        Directory.CreateDirectory(SobekCM_Library_Settings.Static_Pages_Location + ResourcePackage.File_Root);
                    File.Copy(static_file, web_server_file_version, true);
                }

                // Save these collections to mark them for refreshing the RSS feeds, etc..
                Add_Aggregation_To_Refresh_List(ResourcePackage.Metadata.Behaviors.Aggregation_Code_List);

                // Mark a log in the database that this was handled as well
                Resource_Object.Database.SobekCM_Database.Add_Workflow(ResourcePackage.Metadata.Web.ItemID, "Bulk Loaded", ResourcePackage.METS_Type_String, "SobekCM Bulk Loader", String.Empty);

                // If the item is born digital, has files, and is currently public, close out the digitization milestones completely
                if ((!ResourcePackage.Metadata.Tracking.Born_Digital_Is_Null) && (ResourcePackage.Metadata.Tracking.Born_Digital) && (ResourcePackage.Metadata.Behaviors.IP_Restriction_Membership >= 0) && (ResourcePackage.Metadata.Divisions.Download_Tree.Has_Files))
                {
                    Resource_Object.Database.SobekCM_Database.Update_Digitization_Milestone(ResourcePackage.Metadata.Web.ItemID, 4, DateTime.Now);
                }

                // Call the post-process custom actions
                foreach (iBuilder_PostBuild_Process processor in SobekCM_Builder_Configuration_Details.PostBuild_Processes)
                {
                    processor.PostProcess(ResourcePackage.Metadata, ResourcePackage.Resource_Folder);
                }

                // Finally, clear the memory a little bit
                ResourcePackage.Clear_METS();
            }
            catch (Exception ee)
            {
                StreamWriter errorWriter = new StreamWriter(System.Windows.Forms.Application.StartupPath + "\\Logs\\error.log", true);
                errorWriter.WriteLine("Message: " + ee.Message);
                errorWriter.WriteLine("Stack Trace: " + ee.StackTrace);
                errorWriter.Flush();
                errorWriter.Close();

                Add_Error_To_Log("Unable to complete new/replacement for " + ResourcePackage.BibID + ":" + ResourcePackage.VID, ee);
                SobekCM_Database.Add_Item_Error_Log(ResourcePackage.BibID, ResourcePackage.VID, ResourcePackage.METS_Type_String, "Unable to complete load");
            }
        }

		/// <summary> Pre-process any resource files in the incoming folder </summary>
		/// <param name="Resource"></param>
		/// <param name="NewImageFiles"></param>
        private void PreProcess_Any_Resource_Files( Incoming_Digital_Resource Resource, List<string> NewImageFiles )
        {         
            string resourceFolder = Resource.Resource_Folder;
            string bibID = Resource.BibID;
            string vid = Resource.VID;

            // Should we try to convert office files?
            if (SobekCM_Library_Settings.Convert_Office_Files_To_PDF)
            {
                try
                {
                    // Preprocess each Powerpoint document to PDF
                    string[] ppt_files = Directory.GetFiles(resourceFolder, "*.ppt*");
                    foreach (string thisPowerpoint in ppt_files)
                    {
                        // Get the fileinfo and the name
                        FileInfo thisPowerpointInfo = new FileInfo(thisPowerpoint);
                        string filename = thisPowerpointInfo.Name.Replace(thisPowerpointInfo.Extension, "");

                        // Does a PDF version exist for this item?
                        string pdf_version = resourceFolder + "\\" + filename + ".pdf";
                        if (!File.Exists(pdf_version))
                        {
                            int conversion_error = Word_Powerpoint_to_PDF_Converter.Powerpoint_To_PDF(thisPowerpoint, pdf_version);
                            switch (conversion_error)
                            {
                                case 1:
                                    SobekCM_Database.Add_Item_Error_Log(Resource.BibID, Resource.VID, Resource.METS_Type_String, "Error converting PPT to PDF: Can't open input file");
                                    break;

                                case 2:
                                    SobekCM_Database.Add_Item_Error_Log(Resource.BibID, Resource.VID, Resource.METS_Type_String, "Error converting PPT to PDF: Can't create output file");
                                    break;

                                case 3:
                                    SobekCM_Database.Add_Item_Error_Log(Resource.BibID, Resource.VID, Resource.METS_Type_String, "Error converting PPT to PDF: Converting failed");
                                    break;

                                case 4:
                                    SobekCM_Database.Add_Item_Error_Log(Resource.BibID, Resource.VID, Resource.METS_Type_String, "Error converting PPT to PDF: MS Office not installed");
                                    break;
                            }
                        }
                    }

                    // Preprocess each Word document to PDF
                    string[] doc_files = Directory.GetFiles(resourceFolder, "*.doc*");
                    foreach (string thisWordDoc in doc_files)
                    {
                        // Get the fileinfo and the name
                        FileInfo thisWordDocInfo = new FileInfo(thisWordDoc);
                        string filename = thisWordDocInfo.Name.Replace(thisWordDocInfo.Extension, "");

                        // Does a PDF version exist for this item?
                        string pdf_version = resourceFolder + "\\" + filename + ".pdf";
                        if (!File.Exists(pdf_version))
                        {
                            int conversion_error = Word_Powerpoint_to_PDF_Converter.Word_To_PDF(thisWordDoc, pdf_version);
                            switch (conversion_error)
                            {
                                case 1:
                                    SobekCM_Database.Add_Item_Error_Log(Resource.BibID, Resource.VID, Resource.METS_Type_String, "Error converting Word DOC to PDF: Can't open input file");
                                    break;

                                case 2:
                                    SobekCM_Database.Add_Item_Error_Log(Resource.BibID, Resource.VID, Resource.METS_Type_String, "Error converting Word DOC to PDF: Can't create output file");
                                    break;

                                case 3:
                                    SobekCM_Database.Add_Item_Error_Log(Resource.BibID, Resource.VID, Resource.METS_Type_String, "Error converting Word DOC to PDF: Converting failed");
                                    break;

                                case 4:
                                    SobekCM_Database.Add_Item_Error_Log(Resource.BibID, Resource.VID, Resource.METS_Type_String, "Error converting Word DOC to PDF: MS Office not installed");
                                    break;
                            }
                        }
                    }
                }
                catch (Exception ee)
                {
                    StreamWriter errorWriter = new StreamWriter(System.Windows.Forms.Application.StartupPath + "\\Logs\\error.log", true);
                    errorWriter.WriteLine("Message: " + ee.Message);
                    errorWriter.WriteLine("Stack Trace: " + ee.StackTrace);
                    errorWriter.Flush();
                    errorWriter.Close();

                    SobekCM_Database.Add_Item_Error_Log(Resource.BibID, Resource.VID, Resource.METS_Type_String, "Unknown error converting office files to PDF");
                    SobekCM_Database.Add_Item_Error_Log(Resource.BibID, Resource.VID, Resource.METS_Type_String, ee.Message);
                }
            }

            // Preprocess each PDF
            string[] pdfs = Directory.GetFiles(resourceFolder, "*.pdf");
            foreach (string thisPdf in pdfs)
            {
                // Get the fileinfo and the name
                FileInfo thisPdfInfo = new FileInfo(thisPdf);
                string fileName = thisPdfInfo.Name.Replace(thisPdfInfo.Extension, "");

                // Does the full text exist for this item?
                if (!File.Exists(resourceFolder + "\\" + fileName + "_pdf.txt"))
                {
                    PDF_Tools.Extract_Text(thisPdf, resourceFolder + "\\" + fileName + "_pdf.txt");
                }

                // Does the thumbnail exist for this item?
                if ((ghostscript_executable.Length > 0) && (imagemagick_executable.Length > 0))
                {
                    if (!File.Exists(resourceFolder + "\\" + fileName + "thm.jpg"))
                    {
                        PDF_Tools.Create_Thumbnail( resourceFolder, thisPdf, resourceFolder + "\\" + fileName + "thm.jpg", ghostscript_executable, imagemagick_executable);
                    }
                }
            }

            // Preprocess each HTML file for the text
            string[] html_files = Directory.GetFiles(resourceFolder, "*.htm*");
            foreach (string thisHtml in html_files)
            {
                // Get the fileinfo and the name
                FileInfo thisHtmlInfo = new FileInfo(thisHtml);

                // Exclude QC_Error.html
                if (thisHtmlInfo.Name.ToUpper() != "QC_ERROR.HTML")
                {
                    // Just don't pull text for the static page
                    if (thisHtmlInfo.Name.ToUpper() != bibID.ToUpper() + "_" + vid.ToUpper() + ".HTML")
                    {
                        string text_fileName = thisHtmlInfo.Name.Replace(".", "_") + ".txt";

                        // Does the full text exist for this item?
                        if (!File.Exists(resourceFolder + "\\" + text_fileName))
                        {
                            HTML_XML_Text_Extractor.Extract_Text(thisHtml, resourceFolder + "\\" + text_fileName);
                        }
                    }
                }
            }

            // Preprocess each XML file for the text
            string[] xml_files = Directory.GetFiles(resourceFolder, "*.xml");
            foreach (string thisXml in xml_files)
            {
                // Get the fileinfo and the name
                FileInfo thisXmlInfo = new FileInfo(thisXml);

                // Just don't pull text for the static page
                string xml_upper = thisXmlInfo.Name.ToUpper();
                if (( xml_upper.IndexOf(".METS") < 0 ) && ( xml_upper != "DOC.XML" ) && ( xml_upper != "CITATION_METS.XML") && ( xml_upper != "MARC.XML" ))
                {
                    string text_fileName = thisXmlInfo.Name.Replace(".", "_") + ".txt";

                    // Does the full text exist for this item?
                    if (!File.Exists(resourceFolder + "\\" + text_fileName))
                    {
                        HTML_XML_Text_Extractor.Extract_Text(thisXml, resourceFolder + "\\" + text_fileName);
                    }
                }
            }

            // Run OCR for any TIFF files that do not have any corresponding TXT files
            if (SobekCM_Library_Settings.OCR_Command_Prompt.Length > 0)
            {
                string[] ocr_tiff_files = Directory.GetFiles(resourceFolder, "*.tif");
                foreach (string thisTiffFile in ocr_tiff_files)
                {
                    FileInfo thisTiffFileInfo = new FileInfo(thisTiffFile);
                    string text_file = resourceFolder + "\\" + thisTiffFileInfo.Name.Replace(thisTiffFileInfo.Extension,"") + ".txt";
                    if ( !File.Exists( text_file ))
                    {
                        try
                        {
                            string command = String.Format( SobekCM_Library_Settings.OCR_Command_Prompt, thisTiffFile, text_file );
                            System.Diagnostics.Process ocrProcess = new System.Diagnostics.Process {StartInfo = {FileName = command}};
	                        ocrProcess.Start();
                            ocrProcess.WaitForExit();
                        }
                        catch
                        {
                            SobekCM_Database.Add_Item_Error_Log(bibID, vid, String.Empty, "Error launching OCR on (" + thisTiffFileInfo.Name + ")");
                        }
                    }
                }
            }

            // Clean any incoming text files first and look for SSN in text
            string ssn_text_file_name = String.Empty;
            string ssn_match = String.Empty;
            try
            {
                // Get the list of all text files here
                string[] text_files = Directory.GetFiles(resourceFolder, "*.txt");
                if (text_files.Length > 0)
                {
                    // Step through each text file
                    foreach (string textFile in text_files)
                    {
                        // Clean the text file first
                        Text_Cleaner.Clean_Text_File(textFile);

                        // If no SSN possibly found, look for one
                        if (ssn_match.Length == 0)
                        {
                            ssn_match = Text_Cleaner.Has_SSN(textFile);
                            if ( ssn_match.Length > 0 )
                                ssn_text_file_name = (new FileInfo(textFile)).Name;
                        }
                    }
                }
            }
            catch
            {

            }

            // Send a database email if there appears to have been a SSN
            if (ssn_match.Length > 0 )
            {
                if (SobekCM_Library_Settings.Privacy_Email_Address.Length > 0)
                {
                    SobekCM_Database.Send_Database_Email(SobekCM_Library_Settings.Privacy_Email_Address, "Possible Social Security Number Located", "A string which appeared to be a possible social security number was found while bulk loading or post-processing an item.\n\nThe SSN was found in package " + bibID + ":" + vid + " in file '" + ssn_text_file_name + "'.\n\nThe text which may be a SSN is '" + ssn_match + "'.\n\nPlease review this item and remove any private information which should not be on the web server.", false, false, -1, -1);
                }
                SobekCM_Database.Add_Item_Error_Log(bibID, vid, String.Empty, "Possible SSN Located (" + ssn_text_file_name + ")");
            }

            // Are there images that need to be processed here?
            if ( !String.IsNullOrEmpty(imagemagick_executable))
            {
                // Get the list of jpeg and tiff files
                string[] jpeg_files = Directory.GetFiles(resourceFolder, "*.jpg");
                string[] tiff_files = Directory.GetFiles(resourceFolder, "*.tif");

                // Only continue if some exist
                if ((jpeg_files.Length > 0) || (tiff_files.Length > 0))
                {
                    // Create the image process object for creating 
                    Image_Derivative_Creation_Processor imageProcessor = new Image_Derivative_Creation_Processor(imagemagick_executable, System.Windows.Forms.Application.StartupPath + "\\Kakadu", true, true, SobekCM_Library_Settings.JPEG_Width, SobekCM_Library_Settings.JPEG_Height, false, SobekCM_Library_Settings.Thumbnail_Width, SobekCM_Library_Settings.Thumbnail_Height);
                    imageProcessor.New_Task_String += imageProcessor_New_Task_String;
                    imageProcessor.Error_Encountered += imageProcessor_Error_Encountered;


                    // Step through the JPEGS and ensure they have thumbnails (TIFF generation below makes them as well)
                    if (jpeg_files.Length > 0)
                    {
                        foreach (string jpegFile in jpeg_files)
                        {
                            FileInfo jpegFileInfo = new FileInfo(jpegFile);
                            string name = jpegFileInfo.Name.ToUpper();
                            if ((name.IndexOf("THM.JPG") < 0) && (name.IndexOf(".QC.JPG") < 0))
                            {
                                string name_sans_extension = jpegFileInfo.Name.Replace(jpegFileInfo.Extension, "");
                                if (!File.Exists(resourceFolder + "\\" + name_sans_extension + "thm.jpg"))
                                {
                                    imageProcessor.ImageMagick_Create_JPEG(jpegFile, resourceFolder + "\\" + name_sans_extension + "thm.jpg", SobekCM_Library_Settings.Thumbnail_Width, SobekCM_Library_Settings.Thumbnail_Height);
                                }
                            }
                        }
                    }


                    // Step through any TIFFs as well
                    if (tiff_files.Length > 0)
                    {
                        // Do a complete image derivative creation process on these TIFF files
                        imageProcessor.Process(resourceFolder, bibID, vid, tiff_files);

                        // Since we are actually creating page images here (most likely) try to add
                        // them to the package as well
                        foreach (string thisTiffFile in tiff_files)
                        {
                            // Get the name of the tiff file
                            FileInfo thisTiffFileInfo = new FileInfo(thisTiffFile);
                            string tiffFileName = thisTiffFileInfo.Name.Replace(thisTiffFileInfo.Extension, "");

                            // Get matching files
                            string[] matching_files = Directory.GetFiles(resourceFolder, tiffFileName + ".*");

                            // Now, step through all these files
                            foreach (string derivativeFile in matching_files)
                            {
                                // If this is a page image type file, add it
                                FileInfo derivativeFileInfo = new FileInfo(derivativeFile);
                                if (PAGE_IMAGE_EXTENSIONS.Contains(derivativeFileInfo.Extension.ToUpper().Replace(".", "")))
                                    NewImageFiles.Add(derivativeFileInfo.Name);
                            }
                        }
                    }
                }
            }
        }

        private void Rename_Any_Received_METS_File(Incoming_Digital_Resource ResourcePackage)
        {
            string recd_filename = "recd_" + DateTime.Now.Year + "_" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "_" + DateTime.Now.Day.ToString().PadLeft(2, '0') + ".mets.bak";

            // If a renamed file already exists for this year, delete the incoming with that name (shouldn't exist)
            if (File.Exists(ResourcePackage.Resource_Folder + "\\" + recd_filename))
                File.Delete(ResourcePackage.Resource_Folder + "\\" + recd_filename);

            if (File.Exists(ResourcePackage.Resource_Folder + "\\" + ResourcePackage.BibID + "_" + ResourcePackage.VID + ".mets"))
            {
                if (File.Exists(ResourcePackage.Resource_Folder + "\\" + recd_filename ))
                    File.Delete(ResourcePackage.Resource_Folder + "\\" + recd_filename );
                File.Move(ResourcePackage.Resource_Folder + "\\" + ResourcePackage.BibID + "_" + ResourcePackage.VID + ".mets", ResourcePackage.Resource_Folder + "\\" + recd_filename );
                ResourcePackage.METS_File = recd_filename;
                return;
            }
            if (File.Exists(ResourcePackage.Resource_Folder + "\\" + ResourcePackage.BibID + "_" + ResourcePackage.VID + ".mets.xml"))
            {
                if (File.Exists(ResourcePackage.Resource_Folder + "\\" + recd_filename ))
                    File.Delete(ResourcePackage.Resource_Folder + "\\" + recd_filename );
                File.Move(ResourcePackage.Resource_Folder + "\\" + ResourcePackage.BibID + "_" + ResourcePackage.VID + ".mets.xml", ResourcePackage.Resource_Folder + "\\" + recd_filename );
                ResourcePackage.METS_File = recd_filename;
                return;
            }
            if (File.Exists(ResourcePackage.Resource_Folder + "\\" + ResourcePackage.BibID + ".mets"))
            {
                if (File.Exists(ResourcePackage.Resource_Folder + "\\" + recd_filename))
                    File.Delete(ResourcePackage.Resource_Folder + "\\" + recd_filename);
                File.Move(ResourcePackage.Resource_Folder + "\\" + ResourcePackage.BibID + ".mets", ResourcePackage.Resource_Folder + "\\" + recd_filename);
                ResourcePackage.METS_File = recd_filename;
                return;
            }
            if (File.Exists(ResourcePackage.Resource_Folder + "\\" + ResourcePackage.BibID + ".mets.xml"))
            {
                if (File.Exists(ResourcePackage.Resource_Folder + "\\" + recd_filename))
                    File.Delete(ResourcePackage.Resource_Folder + "\\" + recd_filename);
                File.Move(ResourcePackage.Resource_Folder + "\\" + ResourcePackage.BibID + ".mets.xml", ResourcePackage.Resource_Folder + "\\" + recd_filename);
                ResourcePackage.METS_File = recd_filename;
            }
        }

        private void Archive_Any_Files(Incoming_Digital_Resource ResourcePackage)
        {
            // First see if this folder is even eligible for archiving and an archive drop box exists
            if ((SobekCM_Library_Settings.Archive_DropBox.Length > 0) && (( ResourcePackage.Source_Folder.Archive_All_Files) || (ResourcePackage.Source_Folder.Archive_TIFFs)))
            {
                // Get the list of TIFFs
                string[] tiff_files = Directory.GetFiles(ResourcePackage.Resource_Folder, "*.tif");

                // Now, see if we should archive THIS folder, based on upper level folder properties
                if ((ResourcePackage.Source_Folder.Archive_All_Files) || ((tiff_files.Length > 0) && (ResourcePackage.Source_Folder.Archive_TIFFs)))
                {
                    try
                    {
                        // Calculate and create the archive directory
                        string archiveDirectory = SobekCM_Library_Settings.Archive_DropBox + "\\" + ResourcePackage.BibID + "\\" + ResourcePackage.VID;
                        if (!Directory.Exists(archiveDirectory))
                            Directory.CreateDirectory(archiveDirectory);

                        // Copy ALL the files over?
                        if (ResourcePackage.Source_Folder.Archive_All_Files)
                        {
                            string[] archive_files = Directory.GetFiles(ResourcePackage.Resource_Folder);
                            foreach (string thisFile in archive_files)
                            {
                                File.Copy(thisFile, archiveDirectory + "\\" + (new FileInfo(thisFile)).Name, true);
                            }
                        }
                        else
                        {
                            string[] archive_tiff_files = Directory.GetFiles(ResourcePackage.Resource_Folder, "*.tif");
                            foreach (string thisFile in archive_tiff_files)
                            {
                                File.Copy(thisFile, archiveDirectory + "\\" + (new FileInfo(thisFile)).Name, true);
                            }
                        }
                    }
                    catch (Exception ee)
                    {
                        Add_Error_To_Log("Copy to archive failed for " + ResourcePackage.BibID + ":" + ResourcePackage.VID, ee);
                        SobekCM_Database.Add_Item_Error_Log(ResourcePackage.BibID, ResourcePackage.VID, ResourcePackage.METS_Type_String, "Unable to complete copy to archive");

                    }
                }
            }
        }

        private bool Move_All_Files_To_Image_Server(Incoming_Digital_Resource ResourcePackage, List<string> NewImageFiles )
        {
	        try
            {
                // Determine the file root for this
                ResourcePackage.File_Root = ResourcePackage.BibID.Substring(0, 2) + "\\" + ResourcePackage.BibID.Substring(2, 2) + "\\" + ResourcePackage.BibID.Substring(4, 2) + "\\" + ResourcePackage.BibID.Substring(6, 2) + "\\" + ResourcePackage.BibID.Substring(8, 2);

                // Determine the destination folder for this resource
                string serverPackageFolder = SobekCM_Library_Settings.Image_Server_Network + ResourcePackage.File_Root + "\\" + ResourcePackage.VID;

                // Make sure a directory exists here
                if (!Directory.Exists(serverPackageFolder))
                {
                    Directory.CreateDirectory(serverPackageFolder);
                }
                else
                {
                    // COpy any existing mets file to keep what the METS looked like before this change
                    if (File.Exists(serverPackageFolder + "\\" + ResourcePackage.BibID + "_" + ResourcePackage.VID + ".mets.xml"))
                    {
                        File.Copy(serverPackageFolder + "\\" + ResourcePackage.BibID + "_" + ResourcePackage.VID + ".mets.xml", serverPackageFolder + "\\" + ResourcePackage.BibID + "_" + ResourcePackage.VID + "_" + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + ".mets.bak", true);
                    }
                }

                // Move all the files to the digital resource file server
                string[] all_files = Directory.GetFiles(ResourcePackage.Resource_Folder);
                foreach (string thisFile in all_files)
                {
                    FileInfo thisFileInfo = new FileInfo(thisFile);
                    string new_file = serverPackageFolder + "/" + thisFileInfo.Name;

                    // Keep the list of new image files being copied, which may be used later
                    if (PAGE_IMAGE_EXTENSIONS.Contains(thisFileInfo.Extension.ToUpper().Replace(".", "")))
                        NewImageFiles.Add(thisFileInfo.Name);

                    // If the file exists, delete it, 
                    if (File.Exists(new_file))
                    {
                        File.Delete(new_file);
                    }

	                // Move the file over
                    File.Move(thisFile, new_file);
                }

                // Remove the directory and any files which somehow remain
                ResourcePackage.Delete();

                // Since the package has been moved, repoint the resource
                ResourcePackage.Resource_Folder = serverPackageFolder;

                return true;
            }
            catch (Exception ee)
            {
                return false;
            }
        }


        private void Resource_File_Updates(Incoming_Digital_Resource ResourcePackage, List<string> NewImageFiles, bool New_Pacakge )
        {
            // Update the JPEG2000 and JPEG attributes now
            ResourcePackage.Load_File_Attributes(String.Empty, ResourcePackage.Resource_Folder);

            // Ensure all non-image files are linked to the METS file
            string[] all_files = Directory.GetFiles(ResourcePackage.Resource_Folder);
            foreach (string thisFile in all_files)
            {
                FileInfo thisFileInfo = new FileInfo(thisFile );

                if ((!System.Text.RegularExpressions.Regex.Match(thisFileInfo.Name, SobekCM_Library_Settings.Files_To_Exclude_From_Downloads, System.Text.RegularExpressions.RegexOptions.IgnoreCase).Success) && (String.Compare(thisFileInfo.Name, ResourcePackage.BibID + "_" + ResourcePackage.VID + ".html", StringComparison.OrdinalIgnoreCase) != 0))
                {
					// Some last checks here
	                if ((thisFileInfo.Name.IndexOf("marc.xml", StringComparison.OrdinalIgnoreCase) != 0) && (thisFileInfo.Name.IndexOf("doc.xml", StringComparison.OrdinalIgnoreCase) != 0) && (thisFileInfo.Name.IndexOf(".mets", StringComparison.OrdinalIgnoreCase) < 0) && (thisFileInfo.Name.IndexOf("citation_mets.xml", StringComparison.OrdinalIgnoreCase) < 0) &&
						(thisFileInfo.Name.IndexOf("ufdc_mets.xml", StringComparison.OrdinalIgnoreCase) < 0) && (thisFileInfo.Name.IndexOf("agreement.txt", StringComparison.OrdinalIgnoreCase) < 0) &&
						((thisFileInfo.Name.IndexOf(".xml", StringComparison.OrdinalIgnoreCase) < 0) || (thisFileInfo.Name.IndexOf(ResourcePackage.BibID, StringComparison.OrdinalIgnoreCase) < 0)))
	                {
		                ResourcePackage.Metadata.Divisions.Download_Tree.Add_File(thisFileInfo.Name);
	                }
                }
            }

            // Ensure all new image files are linked to the METS file
            bool jpeg_added = false;
            bool jpeg2000_added = false;
            foreach (string thisFile in NewImageFiles)
            {
                // Leave out the legacy QC images
                if ((thisFile.ToUpper().IndexOf(".QC.JPG") < 0) && ( thisFile.ToUpper().IndexOf("THM.JPG") < 0 ))
                {
                    // Add this file
                    FileInfo thisFileInfo = new FileInfo(thisFile);
                    ResourcePackage.Metadata.Divisions.Physical_Tree.Add_File(thisFileInfo.Name);

                    // Also, check to see if this is a jpeg or jpeg2000
                    if (thisFileInfo.Extension.ToUpper() == ".JP2")
                        jpeg2000_added = true;
                    if (thisFileInfo.Extension.ToUpper() == ".JPG")
                        jpeg_added = true;
                }
            }

            // Ensure proper views are attached to this item
            if ((jpeg2000_added) || (jpeg_added))
            {
	            ResourcePackage.Metadata.Behaviors.Add_View(View_Enum.JPEG);
                int view1;
                int view2 = -1;
                int view3 = -1;
                if (jpeg_added)
                {
					ResourcePackage.Metadata.Behaviors.Add_View(View_Enum.JPEG);
					ResourcePackage.Metadata.Behaviors.Add_View(View_Enum.RELATED_IMAGES);
                    if (jpeg2000_added)
						ResourcePackage.Metadata.Behaviors.Add_View(View_Enum.JPEG2000);
                }
                else
                {
					ResourcePackage.Metadata.Behaviors.Add_View(View_Enum.JPEG2000);
                }
            }

            // Ensure a thumbnail is attached
            if ((ResourcePackage.Metadata.Behaviors.Main_Thumbnail.Length == 0) ||
                ((ResourcePackage.Metadata.Behaviors.Main_Thumbnail.IndexOf("http:") < 0) && (!File.Exists(ResourcePackage.Resource_Folder + "\\" + ResourcePackage.Metadata.Behaviors.Main_Thumbnail))))
            {
                // Look for a valid thumbnail
                if (File.Exists(ResourcePackage.Resource_Folder + "\\mainthm.jpg"))
                    ResourcePackage.Metadata.Behaviors.Main_Thumbnail = "mainthm.jpg";
                else
                {
                    string[] jpeg_files = Directory.GetFiles(ResourcePackage.Resource_Folder, "*thm.jpg");
                    if (jpeg_files.Length > 0)
                    {
                        ResourcePackage.Metadata.Behaviors.Main_Thumbnail = (new FileInfo(jpeg_files[0])).Name;
                    }
                    else
                    {
                        if ( ResourcePackage.Metadata.Divisions.Page_Count == 0 )
                        {
                            List<Resource_Object.Divisions.SobekCM_File_Info> downloads = ResourcePackage.Metadata.Divisions.Download_Other_Files;
                            foreach (Resource_Object.Divisions.SobekCM_File_Info thisDownloadFile in downloads)
                            {
                                string mimetype = thisDownloadFile.MIME_Type( thisDownloadFile.File_Extension ).ToUpper();
                                if ((mimetype.IndexOf("AUDIO") >= 0) || (mimetype.IndexOf("VIDEO") >= 0))
                                {
                                    if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\images\\multimedia.jpg"))
                                    {
                                        File.Copy(System.Windows.Forms.Application.StartupPath + "\\images\\multimedia.jpg", ResourcePackage.Resource_Folder + "\\multimediathm.jpg", true);
                                        ResourcePackage.Metadata.Behaviors.Main_Thumbnail = "multimediathm.jpg";
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }

                // Should this be saved?
                if ((ResourcePackage.Metadata.Web.ItemID > 0) && (ResourcePackage.Metadata.Behaviors.Main_Thumbnail.Length > 0))
                {
                    SobekCM_Database.Set_Item_Main_Thumbnail(ResourcePackage.BibID, ResourcePackage.VID, ResourcePackage.Metadata.Behaviors.Main_Thumbnail);
                }
            }

            // If there are no pages, look for a PDF we can use to get a page count
            if (ResourcePackage.Metadata.Divisions.Physical_Tree.Pages_PreOrder.Count <= 0)
            {
                string[] pdf_files = Directory.GetFiles(ResourcePackage.Resource_Folder, "*.pdf");
                if (pdf_files.Length > 0)
                {
                    int pdf_page_count = PDF_Tools.Page_Count(pdf_files[0]);
                    if (pdf_page_count > 0)
                        ResourcePackage.Metadata.Divisions.Page_Count = pdf_page_count;
                }
            }

            // Delete any existing web.config file and write is as necessary
            try
            {
                string web_config = ResourcePackage.Resource_Folder + "\\web.config";
                if (File.Exists(web_config))
                    File.Delete(web_config);
                if ((ResourcePackage.Metadata.Behaviors.Dark_Flag) || (ResourcePackage.Metadata.Behaviors.IP_Restriction_Membership > 0))
                {
                    StreamWriter writer = new StreamWriter(web_config, false);
                    writer.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                    writer.WriteLine("<configuration>");
                    writer.WriteLine("    <system.webServer>");
                    writer.WriteLine("        <security>");
                    writer.WriteLine("            <ipSecurity allowUnlisted=\"false\">");
                    writer.WriteLine("                 <clear />");
                    writer.WriteLine("                 <add ipAddress=\"127.0.0.1\" allowed=\"true\" />");
                    if (SobekCM_Library_Settings.SobekCM_Web_Server_IP.Length > 0)
                        writer.WriteLine("                 <add ipAddress=\"" + SobekCM_Library_Settings.SobekCM_Web_Server_IP.Trim() + "\" allowed=\"true\" />");
                    writer.WriteLine("            </ipSecurity>");
                    writer.WriteLine("        </security>");
                    writer.WriteLine("        <modules runAllManagedModulesForAllRequests=\"true\" />");
                    writer.WriteLine("    </system.webServer>");

                    // Is there now a main thumbnail?
                    if ((ResourcePackage.Metadata.Behaviors.Main_Thumbnail.Length > 0) && (ResourcePackage.Metadata.Behaviors.Main_Thumbnail.IndexOf("http:") < 0))
                    {
                        writer.WriteLine("    <location path=\"" + ResourcePackage.Metadata.Behaviors.Main_Thumbnail + "\">");
                        writer.WriteLine("        <system.webServer>");
                        writer.WriteLine("            <security>");
                        writer.WriteLine("                <ipSecurity allowUnlisted=\"true\" />");
                        writer.WriteLine("            </security>");
                        writer.WriteLine("        </system.webServer>");
                        writer.WriteLine("    </location>");
                    }

                    writer.WriteLine("</configuration>");
                    writer.Flush();
                    writer.Close();
                }
            }
            catch (Exception)
            {
                Add_Error_To_Log("Unable to update the resource web.config file");
            }
        }

        private void Save_All_Updated_Metadata_Files(Incoming_Digital_Resource ResourcePackage)
        {
            // Save the UFDC service mets
            ResourcePackage.Save_SobekCM_Service_METS();

            // Save the citation METS
            ResourcePackage.Save_Citation_METS();

            // Save the MARC XML
            ResourcePackage.Save_MARC_XML(null);
        }

        void imageProcessor_New_Task_String(string NewMessage)
        {
            Add_NonError_To_Log(NewMessage);
        }

        void imageProcessor_Error_Encountered(string NewMessage)
        {
            Add_Error_To_Log(NewMessage);
        }

        #endregion

        #region Process any delete requests

        private void Process_All_Deletes(List<Incoming_Digital_Resource> Deletes)
        {
            if (Deletes.Count == 0)
                return;

            Add_NonError_To_Log("\tProcessing delete packages");
            Deletes.Sort();
            foreach (Incoming_Digital_Resource deleteResource in Deletes)
            {
                // Check for abort
                if (CheckForAbort())
                {
                    Abort_Database_Mechanism.Builder_Operation_Flag = Builder_Operation_Flag_Enum.ABORTING;
                    return;
                }

                Add_NonError_To_Log("\t\tProcessing '" + deleteResource.Folder_Name + "'");

                SobekCM_Database.Clear_Item_Error_Log(deleteResource.BibID, deleteResource.VID, "SobekCM Builder");

                if (itemTable.Select("BibID='" + deleteResource.BibID + "' and VID='" + deleteResource.VID + "'").Length > 0)
                {
                    deleteResource.File_Root = deleteResource.BibID.Substring(0, 2) + "\\" + deleteResource.BibID.Substring(2, 2) + "\\" + deleteResource.BibID.Substring(4, 2) + "\\" + deleteResource.BibID.Substring(6, 2) + "\\" + deleteResource.BibID.Substring(8);
                    string existing_folder = SobekCM_Library_Settings.Image_Server_Network + deleteResource.File_Root + "\\" + deleteResource.VID;

                    // Remove from the primary collection area
                    try
                    {
                        if (Directory.Exists(existing_folder))
                        {
                            // Make sure the delete folder exists
                            if (!Directory.Exists(SobekCM_Library_Settings.Image_Server_Network + "\\DELETED"))
                            {
                                Directory.CreateDirectory(SobekCM_Library_Settings.Image_Server_Network + "\\DELETED");
                            }

                            // Create the final directory
                            string final_folder = SobekCM_Library_Settings.Image_Server_Network + "\\DELETED\\" + deleteResource.File_Root + "\\" + deleteResource.VID;
                            if (!Directory.Exists(final_folder))
                            {
                                Directory.CreateDirectory(final_folder);
                            }

                            // Move each file
                            string[] delete_files = Directory.GetFiles(existing_folder);
                            foreach (string thisDeleteFile in delete_files)
                            {
                                string destination_file = final_folder + "\\" + ((new FileInfo(thisDeleteFile)).Name);
                                if (File.Exists(destination_file))
                                    File.Delete(destination_file);
                                File.Move(thisDeleteFile, destination_file);
                            }
                        }
                    }
                    catch (Exception ee)
                    {
                        Add_Error_To_Log("Unable to move resource ( " + deleteResource.BibID + ":" + deleteResource.VID + " ) to deletes", ee);
                        SobekCM_Database.Add_Item_Error_Log(deleteResource.BibID, deleteResource.VID, deleteResource.METS_Type_String, "Unable to move resource to deletes");
                    }

                    // Delete the static page
                    if (File.Exists(SobekCM_Library_Settings.Static_Pages_Location + deleteResource.BibID + "_" + deleteResource.VID + ".html"))
                    {
                        File.Delete(SobekCM_Library_Settings.Static_Pages_Location + deleteResource.BibID + "_" + deleteResource.VID + ".html");
                    }

                    // Delete the file from the database
                    SobekCM_Database.Delete_SobekCM_Item(deleteResource.BibID, deleteResource.VID, true, "Deleted upon request by builder");

                    // Delete from the solr/lucene indexes
                    if (SobekCM_Library_Settings.Document_Solr_Index_URL.Length > 0)
                    {
                        try
                        {
                            Library.Solr.Solr_Controller.Delete_Resource_From_Index(SobekCM_Library_Settings.Document_Solr_Index_URL, SobekCM_Library_Settings.Page_Solr_Index_URL, deleteResource.BibID, deleteResource.VID);
                        }
                        catch (Exception ee)
                        {
                            Add_Error_To_Log("Error deleting item from the Solr/Lucene index.  The index may not reflect this delete.");
							Add_Error_To_Log("Solr Error: " + ee.Message);
 
                            SobekCM_Database.Add_Item_Error_Log(deleteResource.BibID, deleteResource.VID, "DELETE", "Error deleting from Solr/Lucene index");
                        }
                    }
                    

                    // Save these collections to mark them for search index building
                    Add_Aggregation_To_Refresh_List(deleteResource.Metadata.Behaviors.Aggregation_Code_List);
                }
                else
                {
                    Add_Error_To_Log("Delete ( " + deleteResource.BibID + ":" + deleteResource.VID + " ) invalid... no pre-existing resource");

                    // Finally, clear the memory a little bit
                    deleteResource.Clear_METS();
                }

                // Delete the handled METS file and package
                deleteResource.Delete();
            }
        }

        #endregion

        #region Recreate any aggregation level XML or RSS feeds

        private void Recreate_Aggregation_XML_and_RSS()
        {
            // Any affected aggregation should be refreshed as well
            if (aggregations_to_refresh.Count > 0)
            {
                Add_NonError_To_Log("\tPerforming some aggregation update functions");

                // Step through each aggregation with new items
	            foreach (string thisAggrCode in aggregations_to_refresh)
                {
                    // Some aggregations can be excluded
                    if ((thisAggrCode != "IUF") && (thisAggrCode != "ALL") && (thisAggrCode.Length > 1))
                    {
                        // Get the display aggregation code (lower leading 'i')
                        string display_code = thisAggrCode;
                        if (display_code[0] == 'I')
                            display_code = 'i' + display_code.Substring(1);

                        // Get this item aggregations
                        Library.Aggregations.Item_Aggregation aggregationObj = SobekCM_Database.Get_Item_Aggregation(thisAggrCode, false, false, null);

                        // Get the list of items for this aggregation
                        DataSet aggregation_items = SobekCM_Database.Simple_Item_List(thisAggrCode, null);

                        // Create the XML list for this aggregation
                        Add_NonError_To_Log("\t\tBuilding XML item list for " + display_code);
                        try
                        {
                            string aggregation_list_file = SobekCM_Library_Settings.Static_Pages_Location + "\\" + thisAggrCode.ToLower() + ".xml";
                            if (File.Exists(aggregation_list_file))
                                File.Delete(aggregation_list_file);
                            aggregation_items.WriteXml(aggregation_list_file, XmlWriteMode.WriteSchema);
                        }
                        catch (Exception ee)
                        {
                            Add_Error_To_Log("\t\tError in building XML list for " + display_code + " on " + SobekCM_Library_Settings.Static_Pages_Location, ee);
                        }

                        Add_NonError_To_Log("\t\tBuilding RSS feed for " + display_code);
                        try
                        {
                            staticBuilder.Create_RSS_Feed(thisAggrCode.ToLower(), SobekCM_Library_Settings.Local_Log_Directory, aggregationObj.Name, aggregation_items);
                            try
                            {
                                File.Copy(SobekCM_Library_Settings.Local_Log_Directory + thisAggrCode.ToLower() + "_rss.xml", SobekCM_Library_Settings.Static_Pages_Location + "\\rss\\" + thisAggrCode.ToLower() + "_rss.xml", true);
                                File.Copy(SobekCM_Library_Settings.Local_Log_Directory + thisAggrCode.ToLower() + "_short_rss.xml", SobekCM_Library_Settings.Static_Pages_Location + "\\rss\\" + thisAggrCode.ToLower() + "_short_rss.xml", true);
                            }
                            catch (Exception ee)
                            {
                                Add_Error_To_Log("\t\tError in copying RSS feed for " + display_code + " to " + SobekCM_Library_Settings.Static_Pages_Location, ee);
                            }
                        }
                        catch (Exception ee)
                        {
                            Add_Error_To_Log("\t\tError in building RSS feed for " + display_code, ee);
                        }
                    }
                }
            }
        }

        #endregion

        #region Update the library-wide XML and RSS feeds

        private void Recreate_Library_XML_and_RSS()
        {
            // Update the RSS Feeds and Item Lists for ALL 
            // Build the simple XML result for this build
            Add_NonError_To_Log("\t\tBuilding XML list for all digital resources");
            try
            {
                DataSet simple_list = SobekCM_Database.Simple_Item_List(String.Empty, null);
                if (simple_list != null)
                {
                    try
                    {
                        string aggregation_list_file = SobekCM_Library_Settings.Static_Pages_Location + "\\all.xml";
                        if (File.Exists(aggregation_list_file))
                            File.Delete(aggregation_list_file);
                        simple_list.WriteXml(aggregation_list_file, XmlWriteMode.WriteSchema);
                    }
                    catch (Exception ee)
                    {
                        Add_Error_To_Log("\t\tError in building XML list for all digital resources on " + SobekCM_Library_Settings.Static_Pages_Location, ee);
                    }
                }
            }
            catch (Exception ee)
            {
                Add_Error_To_Log("\t\tError in building XML list for all digital resources", ee);
            }

            // Create the RSS feed for all ufdc items
            try
            {
                Add_NonError_To_Log("\t\tBuilding RSS feed for all digital resources");
                DataSet complete_list = SobekCM_Database.Simple_Item_List(String.Empty, null);

                staticBuilder.Create_RSS_Feed("all", SobekCM_Library_Settings.Local_Log_Directory, "All Items", complete_list);
                try
                {
                    File.Copy(SobekCM_Library_Settings.Local_Log_Directory + "all_rss.xml", SobekCM_Library_Settings.Static_Pages_Location + "\\rss\\all_rss.xml", true);
                    File.Copy(SobekCM_Library_Settings.Local_Log_Directory + "all_short_rss.xml", SobekCM_Library_Settings.Static_Pages_Location + "\\rss\\all_short_rss.xml", true);
                }
                catch (Exception ee)
                {
                    Add_Error_To_Log("\t\tError in copying RSS feed for all digital resources to " + SobekCM_Library_Settings.Static_Pages_Location, ee);
                }
            }
            catch (Exception ee)
            {
                Add_Error_To_Log("\t\tError in building RSS feed for all digital resources", ee);
            }
        }

        #endregion

        #region Method for sending emails to partners for new items

        public void send_emails_for_new_items()
        {
            //Add_NonError_To_Log("\t\tSending emails for new items");

            //// Get the list of institutions
            //DataTable institutions = SobekCM.Library.Database.SobekCM_Database.Get_All_Institutions();

            //// Step through each institution in the item list
            //foreach (string thisInstitution in items_by_location.Keys)
            //{
            //    // Determine the email address
            //    string email = String.Empty;
            //    string institution = String.Empty;

            //    // Determine if these are dLOC
            //    string software = "dLOC";
            //    string help_email = "dloc@fiu.edu";
            //    foreach (string html in items_by_location[thisInstitution])
            //    {
            //        if (html.IndexOf("dloc") < 0)
            //        {
            //            software = "UFDC";
            //            help_email = "ufdc@uflib.ufl.edu";
            //            break;
            //        }
            //    }

            //    if (thisInstitution.IndexOf("@") > 0)
            //    {
            //        email = thisInstitution;
            //        institution = "New " + software + " items";
            //    }
            //    else
            //    {
            //        // Search the list of institutions by the code
            //        DataRow[] selected = institutions.Select("InstitutionCode = '" + thisInstitution + "' and Load_Email_Notification=1");
            //        if (selected.Length > 0)
            //        {
            //            // Get the email and name of institution from the database
            //            email = selected[0]["Load_Email"].ToString();
            //            institution = "New " + software + " items for " + selected[0]["InstitutionName"].ToString();
            //        }
            //    }

            //    if (email.Length > 0)
            //    {
            //        // Get the name of the institution

            //        // Build the text for the email
            //        StringBuilder bodyBuilder = new StringBuilder();
            //        bodyBuilder.AppendLine("The following items are now available online in " + software + ".<br />");
            //        bodyBuilder.AppendLine("Los siguientes artículos, que usted mandó, están disponibles en línea a través de " + software + ".<br />");
            //        bodyBuilder.AppendLine("Les fichiers suivants ont été téléchargés sur le serveur " + software + ".<br />");
            //        bodyBuilder.AppendLine("<br />");
            //        bodyBuilder.AppendLine("<blockquote>");

            //        foreach (string html in items_by_location[thisInstitution])
            //        {
            //            bodyBuilder.AppendLine(html + "<br />");
            //        }

            //        bodyBuilder.AppendLine("</blockquote>");
            //        bodyBuilder.AppendLine("<br />");
            //        bodyBuilder.AppendLine("This is an automatic email.  Please do not respond to this email.  For any issues please contact " + help_email + ".<br />");
            //        bodyBuilder.AppendLine("Este correo se mandó automáticamente. Por favor no conteste este correo. Si usted tiene alguna pregunta o problema por favor mande un correo a " + help_email + ".<br />");
            //        bodyBuilder.AppendLine("Ceci est une réponse automatique. Veuillez ne pas répondre à ce message.  Envoyez-vos enquêtes directement à " + help_email + ".<br />");

            //        // Send this email
            //        try
            //        {
            //            System.Net.Mail.MailMessage myMail = new System.Net.Mail.MailMessage("ufdc_load@uflib.ufl.edu", email);
            //            myMail.Subject = institution;
            //            myMail.IsBodyHtml = true;
            //            myMail.Body = bodyBuilder.ToString();
            //            myMail.IsBodyHtml = true;

            //            // Mail this
            //            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient("smtp.ufl.edu");
            //            client.Send(myMail);
            //        }
            //        catch (Exception ee)
            //        {
            //            string valu = "rerror";
            //        }
            //    }
            //}
        }

        #endregion

        #region Log-supporting methods

        private void Add_NonError_To_Log(string LogStatement)
        {
            Console.WriteLine(LogStatement);
            logger.AddNonError(LogStatement.Replace("\t", "....."));
        }

        private void Add_NonError_To_Log(string LogStatement, bool IsVerbose )
        {
            if (IsVerbose)
            {
                Console.WriteLine(LogStatement);
                logger.AddNonError(LogStatement.Replace("\t", ".....")); 
            }
        }

        private void Add_Error_To_Log(string LogStatement)
        {
            Console.WriteLine(LogStatement);
            logger.AddError(LogStatement.Replace("\t", "....."));
        }

        private void Add_Error_To_Log(string LogStatement, Exception Ee)
        {
            Console.WriteLine(LogStatement);
            logger.AddError(LogStatement.Replace("\t", "....."));

            // Save the exception to an exception file
            StreamWriter exception_writer = new StreamWriter(SobekCM_Library_Settings.Log_Files_Directory + "\\exceptions_log.txt", true);
            exception_writer.WriteLine(String.Empty);
            exception_writer.WriteLine(String.Empty);
            exception_writer.WriteLine("----------------------------------------------------------");
            exception_writer.WriteLine("EXCEPTION CAUGHT " + DateTime.Now.ToString() + " BY PRELOADER");
            exception_writer.WriteLine(LogStatement.ToUpper().Replace("\t", "").Trim());
            exception_writer.WriteLine(Ee.ToString());
            exception_writer.Flush();
            exception_writer.Close();
        }

        private void Add_Complete_To_Log(string LogStatement)
        {
            Console.WriteLine(LogStatement);
            logger.AddComplete(LogStatement.Replace("\t", "....."));
        }

        private void Save_Validation_Errors_To_Log(string ValidationErrors)
        {
            string[] split_validation_errors = ValidationErrors.Split(new char[] { '\n' });
            int error_count = 0;
            foreach (string thisError in split_validation_errors)
            {
                if (thisError.Trim().Length > 0)
                {
                    Add_Error_To_Log("\t\t\t" + thisError);
                    error_count++;
                    if (error_count == 5)
                    {
                        Add_Error_To_Log("\t\t\t(more errors)");
                        break;
                    }
                }
            }
        }

        #endregion

        #region Methods used to get the list of collections to mark in db for build

        private void Add_Aggregation_To_Refresh_List( string Code )
        {
            // Only continue if there is length
            if (Code.Length > 1)
            {
                // This aggregation should be refreshed
                if (!aggregations_to_refresh.Contains(Code.ToUpper()))
                    aggregations_to_refresh.Add(Code.ToUpper());
            }
        }

        private void Add_Aggregation_To_Refresh_List(ReadOnlyCollection<string> Codes)
        {
            foreach (string collectionCode in Codes)
            {
                Add_Aggregation_To_Refresh_List(collectionCode);
            }
        }

        #endregion

        #region Methods to handle checking for abort requests

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
