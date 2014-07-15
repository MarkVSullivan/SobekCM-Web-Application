using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using SobekCM.Resource_Object;
using System.Text.RegularExpressions;

namespace SobekCM.Management_Tool.Importer
{
    public delegate void New_Importer_Progress_Delegate(int New_Progress);

    public enum Matching_Record_Choice_Enum
    {
        Undefined = -1,
        Overlay_Bib_Record = 1,
        Create_New_Record,
        Skip
    }

    public abstract class baseImporter_Processor : iImporter_Process
    {
        public static string Default_Institution_Code;
        public static string Default_Institution_Statement;

        protected List<string> default_projects;
        protected string default_material_type;

        public event New_Importer_Progress_Delegate New_Progress;
        public event New_Importer_Progress_Delegate Complete;

        protected Importer_Report report;
        protected List<string> errors;
        protected List<string> warnings;
        protected string item_import_comments;

        protected Constant_Fields constantCollection;

        protected int recordsSavedToDB;
        protected int recordsProcessed;
        protected int recordsSkipped;
        protected int errorCnt;
        protected int preview_counter;

        protected DataTable allInstitutions;

        protected Dictionary<string, string> Provided_Bib_To_New_Bib;
        protected Dictionary<string, int> Provided_Bib_To_New_Receiving;
        protected List<string> New_Bib_IDs;

        protected Matching_Record_Choice_Enum matching_record_dialog_form_always_use_value = Matching_Record_Choice_Enum.Undefined;
        protected bool allow_overlay;

        protected baseImporter_Processor(Constant_Fields constantCollection)
        {
            // Create the objects to keep track of errors and items processed
            report = new Importer_Report();
            errors = new List<string>();
            warnings = new List<string>();

            // Save the parameters
            this.constantCollection = constantCollection;

            // Set some constants;
            recordsSavedToDB = 0;
            recordsProcessed = 0;
            recordsSkipped = 0;
            errorCnt = 0;
            preview_counter = 1;
            allow_overlay = false;

            // Load the data tables needed
            //     TrackingDB.CS_TrackingDatabase.Refresh_Bib_Table();
            allInstitutions = SobekCM.Library.Database.SobekCM_Database.Get_Codes_Item_Aggregations(null);

            // Declare the lists which will hold the new bib id and receiving id information
            Provided_Bib_To_New_Bib = new Dictionary<string, string>();
            Provided_Bib_To_New_Receiving = new Dictionary<string, int>();
            New_Bib_IDs = new List<string>();

            // Set some defaults
            default_projects = new List<string>();
            default_material_type = String.Empty;
        }

        /// <summary>Gets the formatted messages for errors that prevent saving</summary>
        public string Error_Message
        {
            get
            {
                if (errors.Count > 0)
                {
                    StringBuilder errors_builder = new StringBuilder();
                    foreach (string thisError in errors)
                    {
                        errors_builder.Append( " " + thisError);
                    }
                    return errors_builder.ToString().Trim();
                }

                return String.Empty;
            }
        }

        /// <summary>Gets the formatted messages for warningd that do not prevent saving</summary>
        public string Tracking_Warnings
        {
            get
            {
                if (warnings.Count > 0)
                {
                    StringBuilder warnings_builder = new StringBuilder();
                    foreach (string thisError in warnings)
                    {
                        warnings_builder.Append(" " + thisError);
                    }
                    return warnings_builder.ToString().Trim();
                }

                return String.Empty;
            }
        }

        public DataTable Report_Data
        {
            get { return report.Data; }
        }

        public abstract Importer_Type_Enum Importer_Type { get; }

        protected void OnNewProgress(int Progress)
        {
            if (New_Progress != null)
                New_Progress(Progress);
        }

        protected void OnComplete(int Progress)
        {
            if (Complete != null)
                Complete(Progress);
        }


        protected void Copy_User_Settings_To_Package(SobekCM_Item bibPackage)
        {
            // Add constant data from each mapped column into the bib package
            constantCollection.Add_To_Package(bibPackage);

            // If there is no source included, add it 
            if (bibPackage.Bib_Info.Source.Code.Length == 0)
                bibPackage.Bib_Info.Source.Code = baseImporter_Processor.Default_Institution_Code;
            if (bibPackage.Bib_Info.Source.Statement.Length == 0)
                bibPackage.Bib_Info.Source.Statement = baseImporter_Processor.Default_Institution_Statement;
        }

        protected bool Check_For_Existence_And_Save(SobekCM_Item bibPackage, DataRow currentRow, string related_file, string message, string importer_bib_source, bool preview_mode )
        {
            // reset local variables
            Matching_Record_Choice_Enum matching_record_dialog_form_selected_value = Matching_Record_Choice_Enum.Undefined;
            item_import_comments = String.Empty;
            bool try_to_fetch_existing_division_information = false;

            // clear the message collection
            if (currentRow != null)
            {
                currentRow["Messages"] = String.Empty;
            }

            // Does this Bibliographic record already exist in the tracking database?
            DataRow[] selected = null;
            DataTable matchingRows = SobekCM.Resource_Object.Database.SobekCM_Database.Check_For_Record_Existence(bibPackage.BibID, bibPackage.VID, bibPackage.Bib_Info.OCLC_Record, bibPackage.Bib_Info.ALEPH_Record);
            if ((matchingRows != null) && (matchingRows.Rows.Count > 0))
            {
                // check if the BibID already exists
                if (bibPackage.BibID.Trim().Length > 0)
                {
                    if (Provided_Bib_To_New_Bib.ContainsKey(bibPackage.BibID))
                    {
                        bibPackage.BibID = Provided_Bib_To_New_Bib[bibPackage.BibID];
                        matching_record_dialog_form_selected_value = Matching_Record_Choice_Enum.Create_New_Record;
                    }
                    else
                    {
                        string vid = bibPackage.VID.PadLeft(5, '0');
                        selected = matchingRows.Select("BibID ='" + bibPackage.BibID.Replace("'", "''") + "' and vid = '" + vid.Replace("'", "") + "'");

                        // Search for an existing record that matches the provided BibID.
                        if ((bibPackage.VID.Length > 0) && (selected.Length > 0))
                        {
                            matching_record_dialog_form_selected_value = Matching_Record_Choice_Enum.Skip;
                        }
                    }
                }

                // Don't bother if there was already a BibID:VID match
                if ((matching_record_dialog_form_selected_value == Matching_Record_Choice_Enum.Undefined) && (matchingRows.Rows.Count > 0))
                {
                    // Check for both existing ALEPH and existing OCLC here in one piece of logic
                    if (!New_Bib_IDs.Contains(matchingRows.Rows[0]["bibid"].ToString().ToUpper()))
                    {
                        matching_record_dialog_form_selected_value = get_matching_record_choice(message, "Aleph # " + bibPackage.Bib_Info.ALEPH_Record, matchingRows.Rows[0]["bibid"].ToString());
                        if (matching_record_dialog_form_selected_value == Matching_Record_Choice_Enum.Skip)
                        {
                            bibPackage.BibID = "(" + matchingRows.Rows[0]["bibid"].ToString() + ")";
                        }
                        else if ( matching_record_dialog_form_selected_value == Matching_Record_Choice_Enum.Overlay_Bib_Record )
                        {
                            if (bibPackage.BibID.Length == 0)
                                bibPackage.BibID = matchingRows.Rows[0]["bibid"].ToString().ToUpper();
                            if (bibPackage.VID.Length == 0)
                                bibPackage.VID = matchingRows.Rows[0]["vid"].ToString().ToUpper();
                            try_to_fetch_existing_division_information = true;
                        }
                    }
                    else if (Importer_Type == Importer_Type_Enum.MARC)
                    {
                        if (bibPackage.BibID.Length == 0)
                            bibPackage.BibID = matchingRows.Rows[0]["bibid"].ToString().ToUpper();
                        if (bibPackage.VID.Length == 0)
                            bibPackage.VID = matchingRows.Rows[0]["vid"].ToString().ToUpper();
                        try_to_fetch_existing_division_information = true;
                    }
                }
            }

            // Clear the errors
            this.errors.Clear();
            this.warnings.Clear();

            // determine how to process this row from the dialog result
            switch (matching_record_dialog_form_selected_value)
            {
                case Matching_Record_Choice_Enum.Overlay_Bib_Record:
                    if ((try_to_fetch_existing_division_information))
                    {
                        Resource_Object.SobekCM_Item fromTracking = SobekCM_METS_Finder.Find_UFDC_METS(bibPackage.BibID, bibPackage.VID, String.Empty, null);
                        if (fromTracking != null)
                        {
                            // Copy serial hierarchy information
                            if ((fromTracking.Bib_Info.Series_Part_Info.Enum1.Length > 0) && (bibPackage.Bib_Info.Series_Part_Info.Enum1.Length == 0))
                                bibPackage.Bib_Info.Series_Part_Info.Enum1 = fromTracking.Bib_Info.Series_Part_Info.Enum1;
                            if ((fromTracking.Bib_Info.Series_Part_Info.Enum2.Length > 0) && (bibPackage.Bib_Info.Series_Part_Info.Enum2.Length == 0))
                                bibPackage.Bib_Info.Series_Part_Info.Enum2 = fromTracking.Bib_Info.Series_Part_Info.Enum2;
                            if ((fromTracking.Bib_Info.Series_Part_Info.Enum3.Length > 0) && (bibPackage.Bib_Info.Series_Part_Info.Enum3.Length == 0))
                                bibPackage.Bib_Info.Series_Part_Info.Enum3 = fromTracking.Bib_Info.Series_Part_Info.Enum3;
                            if ((fromTracking.Bib_Info.Series_Part_Info.Year.Length > 0) && (bibPackage.Bib_Info.Series_Part_Info.Year.Length == 0))
                                bibPackage.Bib_Info.Series_Part_Info.Year = fromTracking.Bib_Info.Series_Part_Info.Year;
                            if ((fromTracking.Bib_Info.Series_Part_Info.Month.Length > 0) && (bibPackage.Bib_Info.Series_Part_Info.Month.Length == 0))
                                bibPackage.Bib_Info.Series_Part_Info.Month = fromTracking.Bib_Info.Series_Part_Info.Month;
                            if ((fromTracking.Bib_Info.Series_Part_Info.Day.Length > 0) && (bibPackage.Bib_Info.Series_Part_Info.Day.Length == 0))
                                bibPackage.Bib_Info.Series_Part_Info.Day = fromTracking.Bib_Info.Series_Part_Info.Day;

                            // Copy origin date
                            if ((fromTracking.Bib_Info.Origin_Info.Date_Issued.Length > 0) && (bibPackage.Bib_Info.Origin_Info.Date_Issued.Length == 0))
                                bibPackage.Bib_Info.Origin_Info.Date_Issued = fromTracking.Bib_Info.Origin_Info.Date_Issued;

                            // Check for donor
                            if ((bibPackage.Bib_Info.Donor.ToString().ToUpper().Replace("UNKNOWN", "").Length == 0) && (fromTracking.Bib_Info.Donor.Full_Name.ToUpper().Replace("UNKNOWN", "").Length > 0))
                            {
                                bibPackage.Bib_Info.Donor.Full_Name = fromTracking.Bib_Info.Donor.Full_Name;
                            }

                            // Check for ALEPH number
                            if ((bibPackage.Bib_Info.ALEPH_Record.Length == 0) && (fromTracking.Bib_Info.ALEPH_Record.Length > 0))
                            {
                                bibPackage.Bib_Info.Add_Identifier(fromTracking.Bib_Info.ALEPH_Record, "Aleph");
                            }

                            // Copy the source and holding information
                            bibPackage.Bib_Info.Source.Code = fromTracking.Bib_Info.Source.Code;
                            bibPackage.Bib_Info.Source.Statement = fromTracking.Bib_Info.Source.Statement;
                            bibPackage.Bib_Info.Location.Holding_Code = fromTracking.Bib_Info.Location.Holding_Code;
                            bibPackage.Bib_Info.Location.Holding_Name = fromTracking.Bib_Info.Location.Holding_Name;

                            // Copy any EAD information
                            bibPackage.Bib_Info.Location.EAD_Name = fromTracking.Bib_Info.Location.EAD_Name;
                            bibPackage.Bib_Info.Location.EAD_URL = fromTracking.Bib_Info.Location.EAD_URL;

                            // Finally copy over the divisions, which also includes the files
                            bibPackage.Divisions = fromTracking.Divisions;
                        }
                    }

                    bibPackage.METS_Header.RecordStatus_Enum = METS_Record_Status.METADATA_UPDATE;
                        add_to_database(bibPackage, importer_bib_source, preview_mode, true);

                        if (this.errors.Count == 0)
                        {
                            if (preview_mode)
                            {
                                item_import_comments += "Existing record would have been overlayed. " + Tracking_Warnings;
                            }
                            else
                            {
                                item_import_comments += "Existing record overlayed. " + Tracking_Warnings;
                            }
                            report.Add_Item(bibPackage, related_file, item_import_comments.Trim());

                            if (currentRow != null)
                            {
                                currentRow["Messages"] = item_import_comments;
                                currentRow["New Bib ID"] = bibPackage.BibID;
                                currentRow["New VID ID"] = bibPackage.VID;
                            }

                            return true;
                        }
                        else
                        {
                            item_import_comments = Error_Message;
                            report.Add_Item(bibPackage, related_file, item_import_comments.Trim());

                            if (currentRow != null)
                            {
                                currentRow["Messages"] = Error_Message;
                                bibPackage.VID = String.Empty;
                                bibPackage.BibID = String.Empty;
                            }

                            return false;
                        }

                    
                    break;

                case Matching_Record_Choice_Enum.Create_New_Record:
                    // add new bib record
                    add_to_database(bibPackage, importer_bib_source, preview_mode, false );

                    if (this.errors.Count == 0)
                    {
                        if (preview_mode)
                        {
                            item_import_comments += "New record would have been created. " + Tracking_Warnings;
                        }
                        else
                        {
                            item_import_comments += "New record created. " + Tracking_Warnings;
                        }
                        report.Add_Item(bibPackage, related_file, item_import_comments.Trim());

                        if (currentRow != null)
                        {
                            currentRow["Messages"] = item_import_comments;
                            currentRow["New Bib ID"] = bibPackage.BibID;
                            currentRow["New VID ID"] = bibPackage.VID;
                        }

                        return true;
                    }
                    else
                    {
                        item_import_comments = Error_Message;
                        report.Add_Item(bibPackage, related_file, item_import_comments.Trim());

                        if (currentRow != null)
                        {
                            currentRow["Messages"] = Error_Message;
                            bibPackage.VID = String.Empty;
                            bibPackage.BibID = String.Empty;
                        }

                        return false;
                    }
                    break;

                case Matching_Record_Choice_Enum.Skip:
                    // stop processing this record and continue to the next record
                    if (errors.Count == 0)
                    {
                            item_import_comments += " Record skipped.";

                            if (currentRow != null)
                            {
                                currentRow["Messages"] = "Record skipped.";
                            }
                    }
                    else
                    {
                        item_import_comments = Error_Message;

                        if (currentRow != null)
                        {
                            currentRow["Messages"] = Error_Message;
                        }
                    }

                    // Add this to the result table being built
                    report.Add_Item(bibPackage, related_file, item_import_comments.Trim());
                    recordsProcessed++;
                    recordsSkipped++;
                    return false;

                case Matching_Record_Choice_Enum.Undefined:
                    // add this record to the database since it did not match any existing records
                    if (!add_to_database(bibPackage, importer_bib_source, preview_mode, false))
                    {
                        this.errors.Add("Unable to save to tracking");
                    }

                    if (this.errors.Count == 0)
                    {
                        if (preview_mode)
                        {
                            item_import_comments += "New record would have been created. " + Tracking_Warnings;
                        }
                        else
                        {
                            item_import_comments += "New record created. " + Tracking_Warnings;
                        }
                        report.Add_Item(bibPackage, related_file, item_import_comments.Trim());

                        if (currentRow != null)
                        {
                            currentRow["Messages"] = item_import_comments;
                            currentRow["New Bib ID"] = bibPackage.BibID;
                            currentRow["New VID ID"] = bibPackage.VID;
                        }

                        return true;
                    }
                    else
                    {
                        item_import_comments = Error_Message;
                        report.Add_Item(bibPackage, related_file, item_import_comments.Trim());

                        if (currentRow != null)
                        {
                            currentRow["Messages"] = Error_Message;
                            bibPackage.VID = String.Empty;
                            bibPackage.BibID = String.Empty;
                        }

                        return false;
                    }
                    break;
            }

            return false;
        }

        private Matching_Record_Choice_Enum get_matching_record_choice(string matching_message, string field, string matching_record)
        {
            // Display matching row dialog form
            if (matching_record_dialog_form_always_use_value == Matching_Record_Choice_Enum.Undefined)
            {
                // Get users input
                Forms.Matching_Record_Dialog_Form matching_record_dialog_form = new Forms.Matching_Record_Dialog_Form(String.Format( matching_message, field, matching_record ), allow_overlay );
                matching_record_dialog_form.ShowDialog();

                // Did user select always answer this way?
                if (matching_record_dialog_form.Always_Use_Option_Flag)
                    matching_record_dialog_form_always_use_value = matching_record_dialog_form.Selected_Option_Value;

                // Return the value
                return matching_record_dialog_form.Selected_Option_Value;
            }
            else
            {
                return matching_record_dialog_form_always_use_value;
            }
        }

        /// <summary> Adds this bib id to the SobekCM database. </summary>
        /// <param name="bibPackage"> Digital resource object to save to teh database </param>
        /// <param name="source"> String indicates the source of this new item </param>
        /// <param name="preview_mode"> Flag indicates this is preview mode </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> If the save is successful, the new BibID, VID, ItemID, and GroupID are 
        /// added to the digital resource object. </remarks>
        protected bool add_to_database(SobekCM_Item bibPackage, string source, bool preview_mode, bool existing )
        {
            bool new_item_flag = false;

            try
            {

                // Is there a valid material type?
                if ((bibPackage.Bib_Info.SobekCM_Type_String == "UNDETERMINED") || (bibPackage.Bib_Info.SobekCM_Type_String.Length == 0))
                {
                    if (default_material_type.Length > 0)
                    {
                        bibPackage.Bib_Info.SobekCM_Type_String = default_material_type;
                    }
                    else
                    {
                        Forms.Select_Material_Type_Form selectMaterialType = new Forms.Select_Material_Type_Form(bibPackage.Bib_Info.OCLC_Record, bibPackage.Bib_Info.ALEPH_Record, bibPackage.Bib_Info.Main_Title.ToString(), bibPackage.BibID);
                        if ( selectMaterialType.ShowDialog() == System.Windows.Forms.DialogResult.OK )
                        {
                            bibPackage.Bib_Info.SobekCM_Type_String = selectMaterialType.Material_Type;

                            if ( selectMaterialType.Always_Use_This_Answer )
                            {
                                default_material_type = selectMaterialType.Material_Type;
                            }
                        }
                    }
                }

                // Set a value for the VID_Source and Last_Modified_User variables
                string username = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                bibPackage.Tracking.VID_Source = source + ":" + System.Security.Principal.WindowsIdentity.GetCurrent().Name;

                // If the bibid is not in bibid format, then clear it and save it now
                string original_bibid = bibPackage.BibID;
                if (!SobekCM.Resource_Object.Database.SobekCM_Database.is_bibid_format(original_bibid))
                {
                    if (Provided_Bib_To_New_Bib.ContainsKey(bibPackage.BibID))
                    {
                        bibPackage.BibID = Provided_Bib_To_New_Bib[bibPackage.BibID];
                    }
                    else
                    {
                        bibPackage.BibID = "AA";
                    }
                }
                else
                {
                    original_bibid = String.Empty;
                }

                // Set some values based on the TYPE
                if (bibPackage.Bib_Info.SobekCM_Type_String.ToUpper().IndexOf("NEWSPAPER") >= 0)
                {
                    bibPackage.Tracking.Large_Format = true;
                    bibPackage.Tracking.Track_By_Month = true;
                }

                // Save this Bib oject to the database
                warnings.Clear();
                if (!preview_mode)
                {
                    if (!existing)
                    {
                        if (!SobekCM.Resource_Object.Database.SobekCM_Database.Save_New_Digital_Resource(bibPackage, false, false, username, source, -1))
                        {
                            //this.errors.Add("Error saving " + bibPackage.BibID + ":" + bibPackage.VID + " to the database.");
                            errorCnt++;
                            recordsProcessed++;

                            // exit without saving the Bib item
                            return false;
                        }
                        else
                        {
                            string original_id = bibPackage.BibID + ":" + bibPackage.VID;

                            // increment the recordsSavedToDB counter
                            recordsSavedToDB++;

                            // Save this as a new bib id
                            if (!New_Bib_IDs.Contains(bibPackage.BibID.ToUpper()))
                                New_Bib_IDs.Add(bibPackage.BibID.ToUpper());

                        }
                    }
                    else
                    {
                        if (SobekCM.Resource_Object.Database.SobekCM_Database.Save_Digital_Resource(bibPackage) < 0)
                        {
                            //this.errors.Add("Error saving " + bibPackage.BibID + ":" + bibPackage.VID + " to the database.");
                            errorCnt++;
                            recordsProcessed++;

                            // exit without saving the Bib item
                            return false;
                        }
                        else
                        {
                            string original_id = bibPackage.BibID + ":" + bibPackage.VID;

                            // increment the recordsSavedToDB counter
                            recordsSavedToDB++;

                            // Save this as a new bib id
                            if (!New_Bib_IDs.Contains(bibPackage.BibID.ToUpper()))
                                New_Bib_IDs.Add(bibPackage.BibID.ToUpper());

                        }
                    }
                }
                else
                {
                    if (!Preview_Duplicate_Save_To_Database(bibPackage, errors, warnings, true, new_item_flag))
                    {
                        errorCnt++;
                        recordsProcessed++;

                        // exit without saving the Bib item
                        return false;
                    }
                    else
                    {
                        string original_id = bibPackage.BibID + ":" + bibPackage.VID;

                        // increment the recordsSavedToDB counter
                        recordsSavedToDB++;

                        // Save this as a new bib id
                        if (!New_Bib_IDs.Contains(bibPackage.BibID.ToUpper()))
                            New_Bib_IDs.Add(bibPackage.BibID.ToUpper());
                    }
                }

                // If the original value for BibID was not a valid BibID include the mapping
                // from that term to the new BibID
                if ((original_bibid.Length > 0) && ( bibPackage.BibID.Length > 0 ) && ( !Provided_Bib_To_New_Bib.ContainsKey( original_bibid )))
                {
                    Provided_Bib_To_New_Bib[original_bibid] = bibPackage.BibID;
                }

            }
            catch (Exception e)
            {
                DLC.Tools.Forms.ErrorMessageBox.Show("Error encountered while processing!\n\n" + e.Message, "DLC Importer Error", e);
            }

            // increment the recordsProcessed counter
            recordsProcessed++;

            // Return this Bib item
            return true;
        }

        protected bool Preview_Duplicate_Save_To_Database(SobekCM_Item thisBib, List<string> errors, List<string> warnings, bool check_CopyrightPermissions, bool NewItemFlag)
        {
            try
            {
                // Clear the error list
                errors.Clear();
                warnings.Clear();

                // If there is now a receiving id (it was pre-existing) and VID number is specified, see 
                // if that VID already exists.
                if (thisBib.BibID.Length == 10)
                {
                    //DataRow[] selected_vid = allBibs.Select("VID='" + thisBib.VID.ToUpper().Replace("VID", "") + "' and BibID='" + thisBib.BibID + "'");
                    //if (selected_vid.Length > 0)
                    //{
                    //    thisBib.Tracking.VolumeID = Convert.ToInt32(selected_vid[0]["volumeid"]);
                    //}
                    //else
                    //{
                    //    int volume_number = allBibs.Select("BibID='" + thisBib.BibID + "'").Length;
                    //    thisBib.VID = (volume_number + 1).ToString().PadLeft(5, '0');
                    //}
                }

                // If there is still no receiving id or bib id, reserve one
                if (thisBib.BibID.Length != 10)
                {
                    thisBib.BibID = "PRE" + preview_counter.ToString().PadLeft(7, '0');
                    thisBib.VID = "00001";
                    thisBib.Web.GroupID = preview_counter;
                    thisBib.Web.ItemID = preview_counter;
                    preview_counter++;
                }

                return true;
            }
            catch (Exception ee)
            {
                return false;
            }
        }

        /// <summary> Gets a flag indicating if the provided string appears to be in bib id format </summary>
        /// <param name="test_string"> string to check for bib id format </param>
        /// <returns> TRUE if this string appears to be in bib id format, otherwise FALSE </returns>
        protected bool is_bibid_format(string test_string)
        {
            // Must be 10 characters long to start with
            if (test_string.Length != 10)
                return false;

            // Use regular expressions to check format
            Regex myReg = new Regex("[A-Z]{2}[A-Z|0-9]{4}[0-9]{4}");
            return myReg.IsMatch(test_string.ToUpper());
        }


        protected bool save_to_mets(SobekCM_Item bibPackage, bool preview_mode)
        {
            bibPackage.METS_Header.RecordStatus_Enum = METS_Record_Status.METADATA_UPDATE;

            // Saves the data members in the SobekCM.Bib_Package to a METS file
            try
            {
                // check if the mets file is needed
                if (bibPackage.VID.Length > 0)
                {
                    // Set the directory where the METS file will be saved
                    if (!preview_mode)
                    {
                        string inbound_folder = Library.SobekCM_Library_Settings.Main_Builder_Input_Folder + "\\" + bibPackage.BibID + "_" + bibPackage.VID;
                        bibPackage.Source_Directory = inbound_folder;
                        if (!Directory.Exists(inbound_folder))
                        {
                            Directory.CreateDirectory(inbound_folder);
                        }

                        // create the METS file
                        bibPackage.Save_METS();
                    }
                    else
                    {
                        bibPackage.Source_Directory = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\SMaRT\\Temporary";

                        // create the METS file
                        bibPackage.Save_METS();

                        if (File.Exists(bibPackage.Source_Directory + "\\" + bibPackage.BibID + "_" + bibPackage.VID + ".mets"))
                        {
                            if (File.Exists(bibPackage.Source_Directory + "\\" + bibPackage.BibID + "_" + bibPackage.VID + "_PREVIEW.mets"))
                                File.Delete(bibPackage.Source_Directory + "\\" + bibPackage.BibID + "_" + bibPackage.VID + "_PREVIEW.mets");
                            File.Move(bibPackage.Source_Directory + "\\" + bibPackage.BibID + "_" + bibPackage.VID + ".mets", bibPackage.Source_Directory + "\\" + bibPackage.BibID + "_" + bibPackage.VID + "_PREVIEW.mets");
                        }
                    }




                }

                return true;
            }
            catch (Exception e)
            {
                DLC.Tools.Forms.ErrorMessageBox.Show("Error encountered while creating METS file!\n\n" + e.Message, "DLC Importer Error", e);
                return false;
            }
        }

    }
}
