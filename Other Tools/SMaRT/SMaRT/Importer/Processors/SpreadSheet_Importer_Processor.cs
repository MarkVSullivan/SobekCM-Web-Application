using System;
using System.Configuration;
using System.Windows.Forms;
using System.IO;
using System.Data;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Bib_Info;
using System.Threading;

namespace SobekCM.Management_Tool.Importer
{
    /// <summary> Processor object steps through the MARC file, and does all the necessary work. <br /> <br /> </summary>
    /// <remarks> This runs in a seperate thread than the main form class. <br /> <br /> </remarks>
    public class SpreadSheet_Importer_Processor : baseImporter_Processor
    {
        private DataTable inputDataTbl;
        private List<Mapped_Fields> mapping;
        private DataRow currentRow;
        private bool stopThread = false;
       // private bool create_mets = false;
        private bool preview_mode;

        private string matching_message = "Found a row in the spreadsheet ( {0} ) \nthat matches an existing Bib record ( {1} ).          \n\nSelect an option below on how to process this matching row.";

        /// <summary> Constructor for a new instance of this class </summary>
        /// <param name="InputDataTable">Table from the Excel spreadsheet being processed</param>
        /// <param name="Mapping">Arraylist of 'enum Mapped_Fields' members.</param>
        public SpreadSheet_Importer_Processor(DataTable InputDataTable, List<Mapped_Fields> Mapping, Constant_Fields constantCollection, bool PreView_Mode)
            : base(constantCollection )
        {
            // Save the parameters
            this.inputDataTbl = InputDataTable;
            this.mapping = Mapping;
            this.preview_mode = PreView_Mode;

            // If this is preview mode, get ready to continue by cleaning any preview mets files
            if (preview_mode)
            {
                string temp_folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\SMaRT\\Temporary";

                try
                {
                    if (Directory.Exists(temp_folder))
                    {
                        string[] preview_mets = System.IO.Directory.GetFiles(temp_folder, "*_PREVIEW.mets");
                        int mets_cleanup_errors = 0;
                        foreach (string thisPreviewMets in preview_mets)
                        {
                            try
                            {
                                File.Delete(thisPreviewMets);
                            }
                            catch
                            {
                                mets_cleanup_errors++;
                            }

                            if (mets_cleanup_errors > 5)
                                break;
                        }

                        if (mets_cleanup_errors > 5)
                        {
                            MessageBox.Show("Error cleaning old preview METS from temporary folder.\n\n" + temp_folder);
                        }
                    }
                    else
                    {
                        Directory.CreateDirectory(temp_folder);
                    }
                }
                catch
                {
                    MessageBox.Show("Error cleaning old preview METS from temporary folder.\n\n" + temp_folder);
                }
            }
        }

        public List<SobekCM.Resource_Object.Mapped_Fields> Mapping
        {
            get { return mapping; }
            set { mapping = value; }
        }

        public bool StopThread
        {
            get { return stopThread; }
            set { stopThread = value; }
        }

        /// <summary> Do the bulk of the work of stepping through the input file and
        /// copying the data from the Bib Package to the Tracking Database and creating METS files.</summary>
        public void Do_Work()
        {
            string username = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

            try
            {

                // check for empty rows in the input table
                foreach (DataRow row in inputDataTbl.Rows)
                {
                    // Save this row in case there is an exception caught
                    currentRow = row;

                    // Load the data and constant into a SobekCM_Item object
                    SobekCM_Item newItem = Load_Data_From_DataRow_And_Constants(row);

                    // If that was successful, continnue
                    if (newItem != null)
                    {
                        // If there is a series, bib title but no bib id, use that title as the bib id as well
                        if ((newItem.Behaviors.GroupTitle.Length > 0) && (newItem.BibID.Length == 0))
                            newItem.BibID = newItem.Behaviors.GroupTitle;

                        // Save the original bib id
                        string original_bibid = newItem.BibID;

                        // Save to the tracking database
                        bool success = true;
                        success = base.Check_For_Existence_And_Save(newItem, row, String.Empty, matching_message, "Spreadsheet Importer GUI", preview_mode);

                        // If that was successful, save a new METS file and add to item list
                        if (success)
                        {
                            // Save the METS
                            newItem.METS_Header.Creator_Software = "Spreadsheet Importer";
                            newItem.METS_Header.Creator_Individual = username;
                            save_to_mets(newItem, preview_mode);

                            // Also, save this bib id into the lookup
                            if ((original_bibid.Length > 0) && (original_bibid != newItem.BibID) && ( !base.Provided_Bib_To_New_Bib.ContainsKey( original_bibid )))
                            {
                                base.Provided_Bib_To_New_Bib[original_bibid] = newItem.BibID.ToUpper().Trim();
                                base.Provided_Bib_To_New_Receiving[original_bibid] = -1; // newItem.Tracking.ReceivingID;
                            }
                        }
                    }

                    // check stopThread flag to see if processing should contine                  
                    if (stopThread)
                    {
                        // write message to indicate where processing stopped
                        row["Messages"] += " Processing stopped by user.  This is the last row processed.";
                        item_import_comments += " Processing stopped by user.  This is the last row processed.";

                        // Add this to the result table being built
                        report.Add_Item(newItem, String.Empty, item_import_comments.Trim());
                        return;
                    }

                    // Fire the event that one item is complete
                    OnNewProgress(base.recordsProcessed);

                    errors.Clear();

                    // check stopThread flag to see if processing should contine                  
                    if (stopThread)
                    {
                        break;
                    }
                } // end of processing loop 


                if (stopThread)
                {

                    // Fire the event that the entire work has been stopped
                    OnComplete(999999);

                }
                else
                {

                    // display messagebox that import is complete
                    if (preview_mode)
                    {
                        MessageBox.Show("records processed:\t\t[ " + recordsProcessed.ToString("#,##0;") + " ]" +
                                        "\n\n records saved:\t\t[ " + recordsSavedToDB.ToString("#,##0;") + " ]" +
                                        "\n\n records skipped:\t\t[ " + recordsSkipped.ToString("#,##0;") + " ]" +
                                        "\n\n records with errors:\t\t[ " + errorCnt.ToString("#,##0;") + " ]", "Preview Complete!");
                    }
                    else
                    {
                        MessageBox.Show("records processed:\t\t[ " + recordsProcessed.ToString("#,##0;") + " ]" +
                "\n\n records saved:\t\t[ " + recordsSavedToDB.ToString("#,##0;") + " ]" +
                "\n\n records skipped:\t\t[ " + recordsSkipped.ToString("#,##0;") + " ]" +
                "\n\n records with errors:\t\t[ " + errorCnt.ToString("#,##0;") + " ]", "Import Complete!");
                    }

                    recordsSavedToDB = 0;

                    // Fire the event that the entire work is complete
                    OnComplete(999999);
                }


            }
            catch (System.Threading.ThreadAbortException)
            {
                // A ThreadAbortException has been invoked on the
                // Processor thread.  This exception will be caught here
                // in addition to being caught in the delegate method
                // 'processor_Volume_Processed', where processing of this 
                // exception will take place.                
            }
            catch (Exception e)
            {
                // display the error message
                DLC.Tools.Forms.ErrorMessageBox.Show("Error encountered while processing!\n\n" + e.Message, "DLC Importer Error", e);
                try
                {
                    // write message to indicate where processing stopped
                    currentRow["Messages"] = "Processing stopped by application.  Error Message: " + e.Message + "\n\nThis is the last row processed.";
                    item_import_comments = "Processing stopped by application.  Error Message: " + e.Message + "\n\nThis is the last row processed.";

                    // Add this to the result table being built
                    report.Add_Item(null, String.Empty, item_import_comments.Trim());

                    // set the flag to stop the Processor thread
                    stopThread = true;

                    // Fire the event that the entire work has been stopped
                    OnComplete(999999);
                }
                catch { }
            }
        }

        private SobekCM_Item Load_Data_From_DataRow_And_Constants(DataRow currentRow)
        {
            // Check to see if this is a completely empty row
            bool empty_row = true;
            for (int i = 0; i < currentRow.ItemArray.Length; i++)
            {
                if (currentRow[i].ToString().Trim().Length > 0)
                {
                    empty_row = false;
                    break;
                }
            }

            // If this is empty, skip it
            if (empty_row)
            {
                return null;
            }

            // Create the bibliographic package
            SobekCM_Item bibPackage = new SobekCM_Item();

            // reset the static variables in the mappings class
            Bibliographic_Mapping.clear_static_variables();

            // Step through each column in the data row and add the data into the bib package
            for (int i = 0; (i < inputDataTbl.Columns.Count) && (i < mapping.Count); i++)
            {
                if (currentRow[i].ToString().Length > 0)
                {
                    Bibliographic_Mapping.Add_Data(bibPackage, currentRow[i].ToString(), (Mapped_Fields)mapping[i]);
                }
            }

            // Copy all user settings to this package
            base.Copy_User_Settings_To_Package(bibPackage);

            // Return the built object
            return bibPackage;
        }

        private bool isLocationValid(string location)
        {
            DataRow[] instRows = allInstitutions.Select("itemcode = '" + location.Replace("'", "''") + "'");

            if (instRows.Length > 0)
                return true;
            else
                return false;
        }

        public override Importer_Type_Enum Importer_Type
        {
            get { return Importer_Type_Enum.Spreadsheet; }
        }
    }
}