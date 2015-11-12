#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using SobekCM.Builder_Library.Settings;
using SobekCM.Library.Database;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Utilities;
using SobekCM.Tools.Logs;

#endregion

namespace SobekCM.Builder_Library.Modules.Folders
{
    /// <summary> Folder-level builder module validates the metadata for each folder and classified as package either adding 
    /// a new item or updating an existing item versus a package requesting a delete </summary>
    /// <remarks> This class implements the <see cref="abstractFolderModule" /> abstract class and implements the <see cref="iFolderModule" /> interface. </remarks>
    public class ValidateAndClassifyModule : abstractFolderModule
    {
        private SobekCM_METS_Validator thisMetsValidator;
        private METS_Validator_Object metsSchemeValidator;
        private DataTable itemTable;

        /// <summary> Validates the metadata for each folder and classified as package either adding 
        /// a new item or updating an existing item versus a package requesting a delete </summary>
        /// <param name="BuilderFolder"> Builder folder upon which to perform all work </param>
        /// <param name="IncomingPackages"> List of valid incoming packages, which may be modified by this process </param>
        /// <param name="Deletes"> List of valid deletes, which may be modifyed by this process </param>
        public override void DoWork(Actionable_Builder_Source_Folder BuilderFolder, List<Incoming_Digital_Resource> IncomingPackages, List<Incoming_Digital_Resource> Deletes)
        {
            if (Settings.Builder.Verbose_Flag)
                OnProcess("ValidateAndClassifyModule.Perform_BulkLoader: Begin validating and classifying packages in incoming/process folders", "Verbose", String.Empty, String.Empty, -1);

            // If the maximum number of (incoming, non-delete) packages have already been set aside to process, no need to continue on this folder
            if ((MultiInstance_Builder_Settings.Instance_Package_Limit > 0) && (IncomingPackages.Count >= MultiInstance_Builder_Settings.Instance_Package_Limit))
            {
                OnProcess("...Package validation aborted - maximum number of packages ( " + MultiInstance_Builder_Settings.Instance_Package_Limit + " ) reached", "Verbose", String.Empty, String.Empty, -1);
                return;
            }
                

            try
            {
                // Only continue if the processing folder exists and has subdirectories
                if ((Directory.Exists(BuilderFolder.Processing_Folder)) && (Directory.GetDirectories(BuilderFolder.Processing_Folder).Length > 0))
                {
                    // Get the list of all packages in the processing folder
                    if (Settings.Builder.Verbose_Flag)
                        OnProcess("....Validate packages for " + BuilderFolder.Folder_Name, "Verbose", String.Empty, String.Empty, -1);

                    List<Incoming_Digital_Resource> packages = BuilderFolder.Packages_For_Processing;
                    if (packages.Count > 0)
                    {
                        // Create the METS validation objects
                        if (thisMetsValidator == null)
                        {
                            if (Settings.Builder.Verbose_Flag)
                                OnProcess("ValidateAndClassifyModule.Constructor: Created Validators", "Verbose", String.Empty, String.Empty, -1);

                            thisMetsValidator = new SobekCM_METS_Validator(String.Empty);
                            metsSchemeValidator = new METS_Validator_Object(false);
                        }

                        // Step through each package
                        foreach (Incoming_Digital_Resource resource in packages)
                        {
                            // Validate the categorize the package
                            if (Settings.Builder.Verbose_Flag)
                                OnProcess("........Checking '" + resource.Folder_Name + "'", "Verbose", resource.Folder_Name.Replace("_", ":"), String.Empty, -1);

                            // If there is no METS file, use special code to check this
                            if (Directory.GetFiles(resource.Resource_Folder, "*.mets*").Length == 0)
                            {
                                DirectoryInfo noMetsDirInfo = new DirectoryInfo(resource.Resource_Folder);
                                string vid = noMetsDirInfo.Name;
                                if (noMetsDirInfo.Parent != null) // Should never be null
                                {
                                    string bibid = noMetsDirInfo.Parent.Name;
                                    if ((vid.Length == 16) && (vid[10] == '_'))
                                    {
                                        bibid = vid.Substring(0, 10);
                                        vid = vid.Substring(11, 5);
                                    }

                                    // Is this allowed in this incomnig folder?
                                    if (BuilderFolder.Allow_Folders_No_Metadata)
                                    {
                                        if (itemTable == null)
                                        {
                                            DataSet itemListFromDb = SobekCM_Database.Get_Item_List(true, null);

                                            // Reload the settings
                                            if (itemListFromDb == null)
                                            {
                                                OnError("ValidateAndClassifyModule : Unable to pull the item table from the database", String.Empty, String.Empty, -1);
                                                return;
                                            }

                                            // Save the item table
                                            itemTable = itemListFromDb.Tables[0];
                                        }


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
                                            OnError("METS-less folder is not a valid BibID/VID combination", resource.Folder_Name.Replace("_", ":"), "NONE", -1);

                                            // Move this resource
                                            if (!resource.Move(BuilderFolder.Failures_Folder))
                                            {
                                                OnError("Unable to move folder " + resource.Folder_Name + " in " + BuilderFolder.Folder_Name + " to the failures folder", resource.Folder_Name.Replace("_", ":"), "NONE", -1);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        OnError("METS-less folders are not allowed in " + BuilderFolder.Folder_Name, bibid + ":" + vid, "NONE", -1);

                                        // Move this resource
                                        if (!resource.Move(BuilderFolder.Failures_Folder))
                                        {
                                            OnError("Unable to move folder " + resource.Folder_Name + " in " + BuilderFolder.Folder_Name + " to the failures folder", resource.Folder_Name.Replace("_", ":"), "NONE", -1);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                long validateId = OnProcess("....Validating METS file for " + resource.Folder_Name, "Folder Process", resource.Folder_Name.Replace("_", ":"), "UNKNOWN", -1);
                                string validation_errors = Validate_and_Read_METS(resource, thisMetsValidator, metsSchemeValidator);

                                // Save any errors to the main log
                                if (validation_errors.Length > 0)
                                {
                                    // Save the validation errors to the main log
                                    Save_Validation_Errors_To_Log(validation_errors, resource.Source_Folder.Folder_Name.Replace("_", ":"), validateId);

                                    // Move this resource
                                    if (!resource.Move(BuilderFolder.Failures_Folder))
                                    {
                                        OnError("Unable to move folder " + resource.Folder_Name + " in " + BuilderFolder.Folder_Name + " to the failures folder", resource.Folder_Name.Replace("_", ":"), "NONE", -1);
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
                                            if (BuilderFolder.Allow_Metadata_Updates)
                                            {
                                                IncomingPackages.Add(resource);
                                            }
                                            else
                                            {
                                                OnError("Metadata update is not allowed in " + BuilderFolder.Folder_Name, resource.Folder_Name.Replace("_", ":"), "METADATA UPDATE", -1);

                                                // Move this resource
                                                if (!resource.Move(BuilderFolder.Failures_Folder))
                                                {
                                                    OnError("Unable to move folder " + resource.Folder_Name + " to failures", resource.Folder_Name.Replace("_", ":"), String.Empty, -1);
                                                }
                                            }
                                            break;

                                        case Incoming_Digital_Resource.Incoming_Digital_Resource_Type.DELETE:
                                            if (BuilderFolder.Allow_Deletes)
                                            {
                                                Deletes.Add(resource);
                                            }
                                            else
                                            {
                                                OnError("Delete is not allowed in " + BuilderFolder.Folder_Name, resource.Folder_Name.Replace("_", ":"), "DELETE", -1);

                                                // Move this resource
                                                if (!resource.Move(BuilderFolder.Failures_Folder))
                                                {
                                                    OnError("Unable to move folder " + resource.Folder_Name + " to failures", resource.Folder_Name.Replace("_", ":"), String.Empty, -1);
                                                }
                                            }
                                            break;
                                    }
                                }
                            }

                            // If the maximum number of (incoming, non-delete) packages has now been met, no need to classify anymore
                            if ((MultiInstance_Builder_Settings.Instance_Package_Limit > 0) && (IncomingPackages.Count >= MultiInstance_Builder_Settings.Instance_Package_Limit))
                            {
                                OnProcess("...Package validation aborted - maximum number of packages ( " + MultiInstance_Builder_Settings.Instance_Package_Limit + " ) reached", "Verbose", String.Empty, String.Empty, -1);
                                return;
                            }
                        }
                    }
                }

            }
            catch (Exception ee)
            {
                OnError("Error in harvesting packages from processing : " + ee.Message + "\n" + ee.StackTrace , String.Empty, String.Empty, -1);
            }
        }

        /// <summary> Method releases all resources </summary>
        /// <remarks> Method is overridden to release XML/METS validator objects </remarks>
        public override void ReleaseResources()
        {
            thisMetsValidator = null;
            metsSchemeValidator = null;
            itemTable = null;
        }


        /// <summary> Validates and reads the data from the METS file associated with this incoming digital resource </summary>
        /// <param name="Resource"></param>
        /// <param name="ThisMetsValidator"></param>
        /// <param name="MetsSchemeValidator"></param>
        /// <returns></returns>
        public string Validate_and_Read_METS(Incoming_Digital_Resource Resource, SobekCM_METS_Validator ThisMetsValidator, METS_Validator_Object MetsSchemeValidator)
        {
            if (Settings.Builder.Verbose_Flag)
                OnProcess("ValidateAndClassifyModule.Validate_and_Read_METS: Start ( " + Resource.Folder_Name + " )", "Verbose", String.Empty, String.Empty, -1);

            // Determine invalid bib id and vid for any errors
            string invalid_bibid = String.Empty;
            string invalid_vid = String.Empty;
            string bib_vid = String.Empty;
            if ((Resource.Folder_Name.Length == 16) && (Resource.Folder_Name[10] == '_'))
            {
                invalid_bibid = Resource.Folder_Name.Substring(0, 10);
                invalid_vid = Resource.Folder_Name.Substring(11, 5);
                bib_vid = invalid_bibid + ":" + invalid_vid;
            }
            else if (Resource.Folder_Name.Length == 10)
            {
                invalid_bibid = Resource.Folder_Name;
                invalid_vid = "none";
                bib_vid = invalid_bibid + ":" + invalid_vid;
            }

            // Verify that a METS file exists
            if (Settings.Builder.Verbose_Flag)
                OnProcess("ValidateAndClassifyModule.Validate_and_Read_METS: Check for METS existence", "Verbose", bib_vid, String.Empty, -1);

            if (ThisMetsValidator.METS_Existence_Check(Resource.Resource_Folder) == false)
            {
                // Save this error log and return the error
                Create_Error_Log(Resource.Resource_Folder, Resource.Folder_Name, ThisMetsValidator.ValidationError, "Missing");
                return ThisMetsValidator.ValidationError;
            }

            // Get the name of this METS file
            string[] mets_files = Directory.GetFiles(Resource.Resource_Folder, invalid_bibid + "_" + invalid_vid + ".mets*");
            string mets_file = String.Empty;
            try
            {
                mets_file = mets_files.Length == 0 ? Directory.GetFiles(Resource.Resource_Folder, "*mets*")[0] : mets_files[0];
            }
            catch (Exception)
            {
                Create_Error_Log(Resource.Resource_Folder, Resource.Folder_Name, "Unable to locate correct METS file", "UNKNOWN");
            }
            if (mets_file == String.Empty)
            {
                Create_Error_Log(Resource.Resource_Folder, Resource.Folder_Name, "Unable to locate correct METS file", "UNKNOWN");
            }


            // check the mets file against the scheme
            FileInfo metsFileInfo = new FileInfo(mets_file);
            if (Settings.Builder.Verbose_Flag)
                OnProcess("ValidateAndClassifyModule.Validate_and_Read_METS: Validate against " + metsFileInfo.Name + " against the schema", "Verbose", bib_vid, String.Empty, -1);

            if (MetsSchemeValidator.Validate_Against_Schema(mets_file) == false)
            {
                // Save this error log and return the error
                Create_Error_Log(Resource.Resource_Folder, Resource.Folder_Name, MetsSchemeValidator.Errors, "UNKNOWN", "METS Scheme Validation Error");
                return MetsSchemeValidator.Errors;
            }

            SobekCM_Item returnValue;
            try
            {
                if (Settings.Builder.Verbose_Flag)
                    OnProcess("ValidateAndClassifyModule.Validate_and_Read_METS: Read validated METS file", "Verbose", Resource.Folder_Name.Replace("_", ":"), "UNKNOWN", -1);
                returnValue = SobekCM_Item.Read_METS(mets_file);
            }
            catch
            {
                // Save this error log and return the error
                Create_Error_Log(Resource.Resource_Folder, Resource.Folder_Name, "Error encountered while reading the METS file '" + mets_file, "UNKNOWN");
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
                            Create_Error_Log(Resource.Resource_Folder, Resource.Folder_Name, "METS file does not have a VID and belongs to a multi-volume title", Resource.METS_Type_String);
                            return "METS file does not have a VID and belongs to a multi-volume title";
                        }
                    }
                }


                // Do the basic check first
                if (Settings.Builder.Verbose_Flag)
                    OnProcess("ValidateAndClassifyModule.Validate_and_Read_METS: Perform basic check", "Verbose", Resource.Resource_Folder.Replace("_", ":"), Resource.METS_Type_String, -1);

                if (!ThisMetsValidator.SobekCM_Standard_Check(returnValue, Resource.Resource_Folder))
                {
                    // Save this error log and return null
                    Create_Error_Log(Resource.Resource_Folder, Resource.Folder_Name, ThisMetsValidator.ValidationError, Resource.METS_Type_String);
                    return ThisMetsValidator.ValidationError;
                }

                // If this is a COMPLETE package, check files
                if (returnValue.METS_Header.RecordStatus_Enum == METS_Record_Status.COMPLETE)
                {
                    if (Settings.Builder.Verbose_Flag)
                        OnProcess("ValidateAndClassifyModule.Validate_and_Read_METS: Check resource files (existence and checksum)", "Verbose", Resource.Resource_Folder.Replace("_", ":"), Resource.METS_Type_String, -1);

                    // check if all files exist in the package and the MD5 checksum if the checksum flag is true		
                    if (!ThisMetsValidator.Check_Files(Resource.Resource_Folder, Settings.Builder.VerifyCheckSum))
                    {
                        // Save this error log and return null
                        Create_Error_Log(Resource.Resource_Folder, Resource.Folder_Name, ThisMetsValidator.ValidationError, Resource.METS_Type_String);
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

                if (Settings.Builder.Verbose_Flag)
                    OnProcess("ValidateAndClassifyModule.Validate_and_Read_METS: Complete - validated", "Verbose", Resource.Resource_Folder.Replace("_", ":"), Resource.METS_Type_String, -1);
                return String.Empty;
            }


            // Save this error log and return the error
            Create_Error_Log(Resource.Resource_Folder, Resource.Folder_Name, "Error encountered while reading the METS file '" + mets_file, "UNKNOWN");
            return "Error encountered while reading the METS file '" + mets_file;
        }


        /// <summary>Private method used to generate the error log for the packages</summary>
        /// <param name="Folder_Name"></param>
        /// <param name="ErrorMessage"></param>
        /// <param name="Resource_Folder"></param>
        /// <param name="MetsType"></param>
        private void Create_Error_Log(string Resource_Folder, string Folder_Name, string ErrorMessage, string MetsType)
        {
            Create_Error_Log(Resource_Folder, Folder_Name, ErrorMessage, MetsType, "Error encountered while processing " + Folder_Name);
        }

        /// <summary>Private method used to generate the error log for the packages</summary>
        /// <param name="Folder_Name"></param>
        /// <param name="ErrorMessage"></param>
        /// <param name="Resource_Folder"></param>
        /// <param name="MetsType"></param>
        /// <param name="BaseErrorMessage"></param>
        private void Create_Error_Log(string Resource_Folder, string Folder_Name, string ErrorMessage, string MetsType, string BaseErrorMessage)
        {
            // Split the message into seperate lines
            string[] errors = ErrorMessage.Split(new char[] { '\n' });

            long errorLogId = OnError(BaseErrorMessage, Folder_Name.Replace("_", ":"), MetsType, -1);

            try
            {
                LogFileXhtml errorLog = new LogFileXhtml(Resource_Folder + "\\" + Folder_Name + ".log.html", "Package Processing Log", "SobekCM Builder Errors");
                errorLog.New();
                errorLog.AddComplete("Error Log for " + Folder_Name + " processed at: " + DateTime.Now.ToString());
                errorLog.AddComplete("");

                foreach (string thisError in errors)
                {
                    errorLog.AddError(thisError);
                    OnError(thisError, Folder_Name.Replace("_", ":"), MetsType, errorLogId);
                }
                errorLog.Close();
            }
            catch
            {

            }
        }

        private void Save_Validation_Errors_To_Log(string ValidationErrors, string BibID_VID, long BuilderID)
        {
            string[] split_validation_errors = ValidationErrors.Split(new char[] { '\n' });
            int error_count = 0;
            foreach (string thisError in split_validation_errors)
            {
                if (thisError.Trim().Length > 0)
                {
                    OnError("............" + thisError, BibID_VID, String.Empty, BuilderID);
                    error_count++;
                    if (error_count == 5)
                    {
                        OnError("............(more errors)", BibID_VID, String.Empty, BuilderID);
                        break;
                    }
                }
            }
        }
    }
}
