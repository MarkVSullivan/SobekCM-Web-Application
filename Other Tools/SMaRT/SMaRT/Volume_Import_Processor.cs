#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Security.Principal;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Database;
using SobekCM.Resource_Object.Metadata_Modules;
using SobekCM.Management_Tool.Importer;
using SobekCM.Management_Tool.Settings;

#endregion

namespace SobekCM.Management_Tool
{
    /// <summary>Processor class imports a number of items (volumes) into an existing title  </summary>
    public class Volume_Import_Processor
    {
        private readonly bool dark;
        private readonly int restrictionMask;
        private readonly Dictionary<string, string> sourceMets;
        private readonly DataTable tableToImport;
        private readonly string workingFolder;
        private string bibid;

        /// <summary> Processor is used to import a number of items into an existing title  </summary>
        /// <param name="tableToImport"> Table of items to import </param>
        /// <param name="bibid"> Bibliographic identifier for the title to add all the new volumes to </param>
        /// <param name="sourceMets"> Source METS file to be used for all newly added volumes </param>
        /// <param name="WorkingFolder"> Working folder where these will be prepped </param>
        /// <param name="IP_Restriction_Mask"> IP restriction mask to set for each new item </param>
        /// <param name="Dark"> Dark flag to apply to each new item </param>
        public Volume_Import_Processor(DataTable tableToImport, string bibid, string sourceMets, string WorkingFolder, int IP_Restriction_Mask, bool Dark )
        {
             this.tableToImport = tableToImport;
            workingFolder = WorkingFolder;
            dark = Dark;
            restrictionMask = IP_Restriction_Mask;

            this.sourceMets = new Dictionary<string, string>();
            if (( bibid.Length > 0 ) && ( sourceMets.Length > 0 ))
                this.sourceMets[bibid.ToUpper()] = sourceMets;

            this.bibid = bibid;
        }

        /// <summary> Event is fired during processing to update the progress form  </summary>
        public event New_Importer_Progress_Delegate New_Progress;

        /// <summary> Event is fired when the import process is complete  </summary>
        public event New_Importer_Progress_Delegate Complete;

        /// <summary> Perform the importing process </summary>
        public void Go()
        {
            int currentRow = 0;

            string username = String.Empty;
                var windowsIdentity = WindowsIdentity.GetCurrent();
                if (windowsIdentity != null) username = windowsIdentity.Name;


            // Get column references
            DataColumn bibCol = tableToImport.Columns[0];
            DataColumn firstLevelTextCol = tableToImport.Columns[3];
            DataColumn firstLevelIndexCol = tableToImport.Columns[4];
            DataColumn secondLevelTextCol = tableToImport.Columns[5];
            DataColumn secondLevelIndexCol = tableToImport.Columns[6];
            DataColumn thirdLevelTextCol = tableToImport.Columns[7];
            DataColumn thirdLevelIndexCol = tableToImport.Columns[8];
            DataColumn pubDateCol = tableToImport.Columns[9];
            DataColumn trackingBoxCol = tableToImport.Columns[10];
            DataColumn receivedDateCol = tableToImport.Columns[11];
            DataColumn dispositionAdviceCol = tableToImport.Columns[12];

            foreach (DataRow thisRow in tableToImport.Rows)
            {
                // Get the bibid from the table
                if ( bibid.Length == 0 )
                    bibid = thisRow[bibCol].ToString().ToUpper();

                // Find the source mets
                string thisSourceMets = String.Empty;
                if (sourceMets.ContainsKey(bibid))
                    thisSourceMets = sourceMets[bibid];
                else
                {
                    // Try to find the source mets file
                    string assocFilePath = bibid.Substring(0, 2) + "\\" + bibid.Substring(2, 2) + "\\" + bibid.Substring(4, 2) + "\\" + bibid.Substring(6, 2) + "\\" + bibid.Substring(8, 2);
                    string server_location = Library.SobekCM_Library_Settings.Image_Server_Network + "\\" + assocFilePath;
                    if (Directory.Exists(server_location))
                    {
                        string[] vidFolders = Directory.GetDirectories(server_location);
                        if (vidFolders.Length > 0)
                        {
                            string[] possibleMets = Directory.GetFiles(vidFolders[0], bibid + "_*.mets.xml");
                            if (possibleMets.Length > 0)
                            {
                                thisSourceMets = possibleMets[0];
                                sourceMets[bibid] = thisSourceMets;
                            }
                        }
                    }
                }

                // Load this METS and clear some values
                if (thisSourceMets.Length > 0)
                {
                    SobekCM_Item newItem = SobekCM_Item.Read_METS(thisSourceMets);
                    newItem.VID = String.Empty;
                    newItem.Divisions.Clear();
                    newItem.Bib_Info.Origin_Info.Date_Copyrighted = String.Empty;
                    newItem.Bib_Info.Origin_Info.Date_Issued = String.Empty;
                    newItem.Bib_Info.Origin_Info.Date_Reprinted = String.Empty;

                    // Assign the hierarchy values
                    newItem.Bib_Info.Origin_Info.Date_Issued = thisRow[pubDateCol].ToString();

                    newItem.Behaviors.Serial_Info.Clear();
                    if (thisRow[firstLevelTextCol].ToString().Length > 0)
                    {
                        newItem.Behaviors.Serial_Info.Add_Hierarchy(1, Convert.ToInt32(thisRow[firstLevelIndexCol]), thisRow[firstLevelTextCol].ToString());
                        if (thisRow[secondLevelTextCol].ToString().Length > 0)
                        {
                            newItem.Behaviors.Serial_Info.Add_Hierarchy(2, Convert.ToInt32(thisRow[secondLevelIndexCol]), thisRow[secondLevelTextCol].ToString());
                            if (thisRow[thirdLevelTextCol].ToString().Length > 0)
                            {
                                newItem.Behaviors.Serial_Info.Add_Hierarchy(3, Convert.ToInt32(thisRow[thirdLevelIndexCol]), thisRow[thirdLevelTextCol].ToString());
                            }
                        }
                    }

                    // Assign some tracking values as well, if present
                    if ((thisRow[trackingBoxCol] != null) && (thisRow[trackingBoxCol].ToString().Length > 0))
                    {
                        newItem.Tracking.Tracking_Box = thisRow[trackingBoxCol].ToString();
                    }
                    if ((thisRow[receivedDateCol] != null) && (thisRow[receivedDateCol].ToString().Length > 0))
                    {
                        try
                        {
                            newItem.Tracking.Material_Received_Date = Convert.ToDateTime(thisRow[receivedDateCol]);
                            newItem.Tracking.Material_Rec_Date_Estimated = false;
                        }
                        catch
                        {

                        }
                    }
                    if ((thisRow[dispositionAdviceCol] != null) && (thisRow[dispositionAdviceCol].ToString().Length > 0))
                    {
                        newItem.Tracking.Disposition_Advice_Notes = thisRow[dispositionAdviceCol].ToString();
                        string advice_notes_as_caps = newItem.Tracking.Disposition_Advice_Notes.ToUpper();
                        if ((advice_notes_as_caps.IndexOf("RETURN") >= 0) || (advice_notes_as_caps.IndexOf("RETAIN") >= 0))
                        {
                            newItem.Tracking.Disposition_Advice = 1;
                        }
                        else
                        {
                            if (advice_notes_as_caps.IndexOf("WITHDRAW") >= 0)
                            {
                                newItem.Tracking.Disposition_Advice = 2;
                            }
                            else if (advice_notes_as_caps.IndexOf("DISCARD") >= 0)
                            {
                                newItem.Tracking.Disposition_Advice = 3;
                            }
                        }
                    }

                    // Save this to the database
                    newItem.Tracking.VID_Source = "SMaRT Volume Import:" + username;
                    newItem.Behaviors.Dark_Flag = dark;
                    newItem.Behaviors.IP_Restriction_Membership = (short)restrictionMask;
                    SobekCM_Database.Save_New_Digital_Resource(newItem, false, false, username, String.Empty, -1);

                    // Save this VID to the table
                    thisRow[0] = newItem.VID;

                    // Save this METS
                    newItem.METS_Header.RecordStatus_Enum = METS_Record_Status.METADATA_UPDATE;
                    newItem.Source_Directory = workingFolder;
                    newItem.Save_SobekCM_METS();

                    // Copy to the INBOUND and DATA FOLDERS
                    string inbound_folder = Library.SobekCM_Library_Settings.Main_Builder_Input_Folder + "\\" + newItem.BibID + "_" + newItem.VID;
                    if (!Directory.Exists(inbound_folder))
                        Directory.CreateDirectory(inbound_folder);
                    string newbibid = newItem.BibID;
                    File.Copy(workingFolder + "\\" + newbibid + "_" + newItem.VID + ".mets.xml", inbound_folder + "\\" + newbibid + "_" + newItem.VID + ".mets.xml", true);

                }


                // Update progress
                currentRow++;
                OnNewProgress(currentRow);
            }

            OnComplete(currentRow);
        }

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
    }
}
