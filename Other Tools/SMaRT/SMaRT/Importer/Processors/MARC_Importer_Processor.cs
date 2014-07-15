using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.MARC;
using SobekCM.Resource_Object.MARC.Parsers;
using SobekCM.Resource_Object.Metadata_File_ReaderWriters;

namespace SobekCM.Management_Tool.Importer
{
	/// <summary> Processor object steps through the MARC file, and does all the necessary work. <br /> <br /> </summary>
	/// <remarks> This runs in a seperate thread than the main form class. <br /> <br /> Written by Mark Sullivan (2005) </remarks>
	public class MARC_Importer_Processor : baseImporter_Processor
	{
		private string inputFile;
        private bool JustSaveMarcXML;
		private string projectCode;
		private string marc_directory;            
        private bool preview_mode;
        private string error_folder;

		/// <summary> Object is used to actually step through the file and parse the
        /// information into <see cref="SobekCM.Bib_Package.MARC.MARC_Record"/> object. </summary>
		protected MARC21_Exchange_Format_Parser parser;

        private string matching_message = "Found a field in the MARC file ( {0} ) \nthat matches an existing Bib record ( {1} ).          \n\nSelect an option below on how to process this matching record.";


		/// <summary> Constructor for a new instance of this class </summary>
		/// <param name="fields"> Fields which will be added to each record created </param>
		/// <param name="inputFile"> Text of the input file </param>
		/// <param name="outputFile"> Text for the output file as well </param>
        public MARC_Importer_Processor(string inputFile, bool JustSaveMarcXML, string ProjectCode, Constant_Fields constantCollection, bool Preview_Mode, string Error_Folder ) : base ( constantCollection )
		{			
			// Save the parameters
			this.inputFile = inputFile;
            this.JustSaveMarcXML = JustSaveMarcXML;
			this.projectCode = ProjectCode;
            this.preview_mode = Preview_Mode;
            this.error_folder = Error_Folder;

            // Allow overlay from MARC records
            base.allow_overlay = true;

			// Create the parser
            parser = new MARC21_Exchange_Format_Parser();

            // Set the MARC directory and the error directory
		    marc_directory = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\SMaRT\\MarcXML\\";
            error_folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\SMaRT\\Temporary\\";

            // Ensure both folders exist
            if (!Directory.Exists(marc_directory))
                Directory.CreateDirectory(marc_directory);
            if (!Directory.Exists(error_folder))
                Directory.CreateDirectory(error_folder);
		}

        #region Method to save the MARC XML file for a particular MARC21 Record

        private string Save_MARC_XML( MARC_Record result )
		{            
			// Determine the directory to save the MARC XML into
			if ( marc_directory.Length > 0 )
			{
                // If this is an error, just save to a local error folder
                if ((result.Error_Flag) || ( preview_mode ))
                {
                    // Save the location for temporary files
                    string marc_xml_name = error_folder + DateTime.Now.Year + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0') + DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') + ".xml";
                    if (File.Exists(marc_xml_name))
                        File.Delete(marc_xml_name);
                    bool error_success = result.Save_MARC_XML(marc_xml_name);
                    if (error_success)
                        return marc_xml_name;
                    else
                        return String.Empty;
                }

				// Save by OCLC number first
                if (result.Control_Number.Length > 3)
                {
                    string recordNumber = result.Control_Number;
                    if (((recordNumber.IndexOf("ocn") == 0) || ( recordNumber.IndexOf("ocm") == 0 )) && (recordNumber.Length >= 4))
                    {
                        string oclc = recordNumber.Substring(3).Trim();
                        bool oclc_success = false;
                        string endDir = "OCLC/";
                        for (int i = 0; i < oclc.Length; i++)
                        {
                            endDir += oclc[i] + "/";
                        }
                        endDir = endDir.Remove(endDir.Length - 1);

                        try
                        {
                            if (!Directory.Exists(marc_directory + endDir))
                            {
                                Directory.CreateDirectory(marc_directory + endDir);
                            }

                            oclc_success = result.Save_MARC_XML(marc_directory + endDir + "/" + oclc + ".xml");
                        }
                        catch { }

                        if (oclc_success)
                        {
                            return marc_directory + endDir + "/" + oclc + ".xml";
                        }
                        else
                        {
                            try
                            {
                                if (!Directory.Exists(error_folder + "\\" + endDir))
                                {
                                    Directory.CreateDirectory(error_folder + "\\" + endDir);
                                }

                                // Create the output file
                                result.Save_MARC_XML(error_folder + "\\" + endDir + "\\" + oclc + ".xml");
                                return error_folder + "\\" + endDir + "\\" + oclc + ".xml";
                            }
                            catch { }
                        }
                    }
                    else
                    {
                        string aleph = recordNumber.Trim();
                        bool aleph_success = false;

                        string endDir = String.Empty;

                        for (int i = 0; i < aleph.Length; i++)
                        {
                            endDir += aleph[i] + "/";
                        }
                        endDir = endDir.Remove(endDir.Length - 1);

                        try
                        {
                            if (!Directory.Exists(marc_directory + endDir))
                            {
                                Directory.CreateDirectory(marc_directory + endDir);
                            }

                            aleph_success = result.Save_MARC_XML(marc_directory + endDir + "/" + aleph + ".xml");
                        }
                        catch { }

                        if (aleph_success)
                        {
                            return marc_directory + endDir + "/" + aleph + ".xml";
                        }
                        else
                        {
                            try
                            {
                                if (!Directory.Exists(error_folder + "\\" + endDir))
                                {
                                    Directory.CreateDirectory(error_folder + "\\" + endDir);
                                }

                                // Create the output file
                                result.Save_MARC_XML(error_folder + "\\" + endDir + "\\" + aleph + ".xml");
                                return error_folder + "\\" + endDir + "\\" + aleph + ".xml";
                            }
                            catch { }
                        }
                    }
                }
                else
                {
                    // Save the location for temporary files
                    string marc_xml_name = error_folder + DateTime.Now.Year + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0') + DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') + ".xml";
                    if (File.Exists(marc_xml_name))
                        File.Delete(marc_xml_name);
                    bool error_success = result.Save_MARC_XML(marc_xml_name);
                    if (error_success)
                        return marc_xml_name;
                    else
                        return String.Empty;
                }
            }

            return String.Empty;
        }

        #endregion

        /// <summary> Do the bulk of the work of stepping through the input file and
		/// copying the data from the MARC record to the Tracking data object. </summary>
		public void Do_Work()
		{
            string username = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            errors.Clear();
            errorCnt = 0;        
			//int error = 0;

            // Declare the marc xml reader
            MarcXML_File_ReaderWriter marcReader = new MarcXML_File_ReaderWriter();

			try
			{
				// Read the first record
                MARC_Record result = parser.Parse( inputFile );

                // Loop through all the records
                while (parser.EOF_Flag == false)                
				{
                    // Fire the event that one item is complete
                    OnNewProgress(recordsProcessed);

					try
					{
                        // check if the record is null or has errors
                        if (result == null)
                        {
                            // increment counters
                            errorCnt++;
                            recordsProcessed++;
                                
                            // get next record
                            result = parser.Next();
                            continue;
                        }

						// Save the MARC XML file
						string marc_xml_file = Save_MARC_XML( result );

                        // If we should only save these, move on
                        if (JustSaveMarcXML)
                        {
                            recordsProcessed++;

                            // get next record
                            result = parser.Next();
                            continue;
                        }

                        // If there was no marc xml file saved, skip this and call it an error
                        if ((marc_xml_file.Length == 0) || (!File.Exists(marc_xml_file)))
                        {
                            // increment counters
                            errorCnt++;
                            recordsProcessed++;

                            // get next record
                            result = parser.Next();
                            continue;
                        }

                        // Load the information from the MARC XML file
                        SobekCM_Item newItem = new SobekCM_Item();
					    string error_message = String.Empty;
                        marcReader.Read_Metadata( marc_xml_file, newItem, null, out error_message );


                        // FOR THE MARC IMPORTER ONLY.. THE MAIN TITLE SHOULD BE COPIED TO THE BIB TITLE
                        if (newItem != null)
                        {
                            newItem.Behaviors.GroupTitle = newItem.Bib_Info.Main_Title.Title.Trim();
                        }

                        // try adding some data if the error flag is true
                        if ((result.Error_Flag) || ( newItem == null ))
                        {
                            // Add this to the result table being built
                            base.report.Add_Item(newItem, marc_xml_file, "Error reading MARC record");

                            // increment counters
                            errorCnt++;
                            recordsProcessed++;
                        }
                        else
                        {
                            // Copy all user settings to this package
                            base.Copy_User_Settings_To_Package(newItem);

                            // Save this to the tracking database
                            bool success = base.Check_For_Existence_And_Save(newItem, null, marc_xml_file, matching_message, "MARC Importer GUI", preview_mode);

                            // If that was successful, save a new METS file and add to item list
                            if (success)
                            {
                                // Save the METS
                                newItem.METS_Header.Creator_Software = "Spreadsheet Importer";
                                newItem.METS_Header.Creator_Individual = username;
                                save_to_mets(newItem, preview_mode);
                            }
                        }                                          
					}
					catch (Exception ee)
					{
                        DLC.Tools.Forms.ErrorMessageBox.Show(ee.Message, "DLC Importer Error", ee);
                        errorCnt++;
					}			
					
					// Fire the event that one item is complete
                    OnNewProgress(recordsProcessed);

					// Get the next one
					result = parser.Next();                                      
				}
               
                // display messagebox that import is complete
                if (preview_mode)
                {
                    MessageBox.Show("records processed:\t\t[ " + recordsProcessed.ToString("#,##0;") + " ]" +
                                    "\n\n records saved:\t\t[ " + recordsSavedToDB.ToString("#,##0;") + " ]" +
                                    "\n\n records skipped:\t\t[ " + recordsSkipped.ToString("#,##0;") + " ]" +
                                    "\n\n records with errors:\t[ " + errorCnt.ToString("#,##0;") + " ]", "Preview Complete!");
                }
                else
                {
                    MessageBox.Show("records processed:\t\t[ " + recordsProcessed.ToString("#,##0;") + " ]" +
                                    "\n\n records saved:\t\t[ " + recordsSavedToDB.ToString("#,##0;") + " ]" +
                                    "\n\n records skipped:\t\t[ " + recordsSkipped.ToString("#,##0;") + " ]" +
                                    "\n\n records with errors:\t[ " + errorCnt.ToString("#,##0;") + " ]", "Import Complete!");
                }

                recordsSavedToDB = 0;
			}
			catch (Exception ee)
			{
				MessageBox.Show("Error encountered while processing!     \n\nOnly " + recordsProcessed + " records processed.\n\n" + ee.ToString(), "Did not reach EOF Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning );
			}

			// Fire the event that the entire work is complete
            OnComplete(999999);
		}

        //private void update_bib_list_table(SobekCM_Item thisBib)
        //{
        //    // Is this a new row?
        //    DataRow[] selected = this.allBibs.Select("receivingid = " + thisBib.ReceivingID);
        //    if ( selected.Length > 0 )
        //    {
        //        // Modify the existing row
        //        selected[0]["BibID"] = thisBib.BibID;
        //        selected[0]["LTQF"] = thisBib.Bib_Info.LTQF;
        //        selected[0]["LTUF"] = thisBib.LTUF;
        //        selected[0]["OCLC"] = thisBib.OCLC;               
        //        selected[0]["Aleph"] = thisBib.AlephBibNumber;
        //        selected[0]["Title"] = thisBib.Title;
        //        selected[0]["Author"] = thisBib.Author;
        //        selected[0]["Material_Type"] = Bibliographic_Type_Mill.ToSearchableString(thisBib.Type) ;
        //        selected[0]["Project_Code"] = " " + thisBib.ProjectCodes.ToString();
        //        selected[0]["MARC_Record_Flag"] = true;
        //    }
        //    else
        //    {
        //        // Add a new row
        //        DataRow newRow = allBibs.NewRow();
        //        newRow["receivingid"] = thisBib.ReceivingID;
        //        newRow["BibID"] = thisBib.BibID;
        //        newRow["LTQF"] = thisBib.LTQF;
        //        newRow["LTUF"] = thisBib.LTUF;
        //        newRow["OCLC"] = thisBib.OCLC;               
        //        newRow["Aleph"] = thisBib.AlephBibNumber;
        //        newRow["Title"] = thisBib.Title;
        //        newRow["Author"] = thisBib.Author;
        //        newRow["Material_Type"] = Bibliographic_Type_Mill.ToSearchableString(thisBib.Type) ;
        //        newRow["Project_Code"] = " " + thisBib.ProjectCodes.ToString();
        //        newRow["MARC_Record_Flag"] = true;

        //        allBibs.Rows.Add( newRow );
        //    } 
        //}
                                                                 
        public override Importer_Type_Enum Importer_Type
        {
            get { return Importer_Type_Enum.MARC; }
        }
    }
}
