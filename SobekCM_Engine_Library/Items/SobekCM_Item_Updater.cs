using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SobekCM.Core.MemoryMgmt;
using SobekCM.Core.Users;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Engine_Library.Database;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Database;
using SobekCM.Resource_Object.Metadata_File_ReaderWriters;

namespace SobekCM.Engine_Library.Items
{
    public static class SobekCM_Item_Updater
    {
        public static bool Update_Item(SobekCM_Item Item, User_Object User, out string Error_Message )
        {
            Error_Message = String.Empty;

            // Determine the in process directory for this
            string user_bib_vid_process_directory = Path.Combine(Engine_ApplicationCache_Gateway.Settings.In_Process_Submission_Location, User.ShibbID + "\\metadata_updates\\" + Item.BibID + "_" + Item.VID);
            if (User.ShibbID.Trim().Length == 0)
                user_bib_vid_process_directory = Path.Combine(Engine_ApplicationCache_Gateway.Settings.In_Process_Submission_Location, User.UserName.Replace(".", "").Replace("@", "") + "\\metadata_updates\\" + Item.BibID + "_" + Item.VID);

            // Ensure the folder exists and is empty to start with
            if (!Directory.Exists(user_bib_vid_process_directory))
                Directory.CreateDirectory(user_bib_vid_process_directory);
            else
            {
                // Anything older than a day should be deleted
                string[] files = Directory.GetFiles(user_bib_vid_process_directory);
                foreach (string thisFile in files)
                {
                    try
                    {
                        File.Delete(thisFile);
                    }
                    catch (Exception)
                    {
                        // Not much to do here
                    }
                }
            }

            // Update the METS file with METS note and name
            Item.METS_Header.Creator_Individual = User.UserName;
            Item.METS_Header.Modify_Date = DateTime.Now;
            Item.METS_Header.RecordStatus_Enum = METS_Record_Status.METADATA_UPDATE;

            // Create the options dictionary used when saving information to the database, or writing MarcXML
            Dictionary<string, object> options = new Dictionary<string, object>();
            if (Engine_ApplicationCache_Gateway.Settings.MarcGeneration != null)
            {
                options["MarcXML_File_ReaderWriter:MARC Cataloging Source Code"] = Engine_ApplicationCache_Gateway.Settings.MarcGeneration.Cataloging_Source_Code;
                options["MarcXML_File_ReaderWriter:MARC Location Code"] = Engine_ApplicationCache_Gateway.Settings.MarcGeneration.Location_Code;
                options["MarcXML_File_ReaderWriter:MARC Reproduction Agency"] = Engine_ApplicationCache_Gateway.Settings.MarcGeneration.Reproduction_Agency;
                options["MarcXML_File_ReaderWriter:MARC Reproduction Place"] = Engine_ApplicationCache_Gateway.Settings.MarcGeneration.Reproduction_Place;
                options["MarcXML_File_ReaderWriter:MARC XSLT File"] = Engine_ApplicationCache_Gateway.Settings.MarcGeneration.XSLT_File;
            }
            options["MarcXML_File_ReaderWriter:System Name"] = Engine_ApplicationCache_Gateway.Settings.System_Name;
            options["MarcXML_File_ReaderWriter:System Abbreviation"] = Engine_ApplicationCache_Gateway.Settings.System_Abbreviation;
            //  options["MarcXML_File_ReaderWriter:Additional_Tags"] = Item.MARC_Sobek_Standard_Tags(true, Engine_ApplicationCache_Gateway.Settings.System_Name, Engine_ApplicationCache_Gateway.Settings.System_Abbreviation);
 

            // Save the METS file and related Items
            bool successful_save = true;
            try
            {
                SobekCM_Database.Save_Digital_Resource(Item, options, DateTime.Now, true);
            }
            catch
            {
                successful_save = false;
            }

            //// Create the static html pages
            //string base_url = RequestSpecificValues.Current_Mode.Base_URL;
            //try
            //{
            //    Static_Pages_Builder staticBuilder = new Static_Pages_Builder(Engine_AppliationCache_Gateway.Settings.System_Base_URL, Engine_AppliationCache_Gateway.Settings.Base_Data_Directory, RequestSpecificValues.HTML_Skin.Skin_Code);
            //    string filename = user_bib_vid_process_directory + "\\" + Item.BibID + "_" + Item.VID + ".html";
            //    staticBuilder.Create_Item_Citation_HTML(Item, filename, Engine_AppliationCache_Gateway.Settings.Image_Server_Network + Item.Web.AssocFilePath);

            //    // Copy the static HTML file to the web server
            //    try
            //    {
            //        if (!Directory.Exists(Engine_AppliationCache_Gateway.Settings.Static_Pages_Location + Item.BibID.Substring(0, 2) + "\\" + Item.BibID.Substring(2, 2) + "\\" + Item.BibID.Substring(4, 2) + "\\" + Item.BibID.Substring(6, 2) + "\\" + Item.BibID.Substring(8)))
            //            Directory.CreateDirectory(Engine_AppliationCache_Gateway.Settings.Static_Pages_Location + Item.BibID.Substring(0, 2) + "\\" + Item.BibID.Substring(2, 2) + "\\" + Item.BibID.Substring(4, 2) + "\\" + Item.BibID.Substring(6, 2) + "\\" + Item.BibID.Substring(8));
            //        if (File.Exists(user_bib_vid_process_directory + "\\" + Item.BibID + "_" + Item.VID + ".html"))
            //            File.Copy(user_bib_vid_process_directory + "\\" + Item.BibID + "_" + Item.VID + ".html", Engine_AppliationCache_Gateway.Settings.Static_Pages_Location + Item.BibID.Substring(0, 2) + "\\" + Item.BibID.Substring(2, 2) + "\\" + Item.BibID.Substring(4, 2) + "\\" + Item.BibID.Substring(6, 2) + "\\" + Item.BibID.Substring(8) + "\\" + Item.BibID + "_" + Item.VID + ".html", true);
            //    }
            //    catch
            //    {
            //        // This is not critical
            //    }
            //}
            //catch
            //{
            //    // Failing to make the static page is not the worst thing in the world...
            //}
            //RequestSpecificValues.Current_Mode.Base_URL = base_url;

            Item.Source_Directory = user_bib_vid_process_directory;
            Item.Save_SobekCM_METS();

            // If this was not able to be saved in the database, try it again
            if (!successful_save)
            {
                SobekCM_Database.Save_Digital_Resource(Item, options, DateTime.Now, false);
            }

            // Make sure the progress has been added to this Item's work log
            try
            {
                Engine_Database.Tracking_Online_Edit_Complete(Item.Web.ItemID, User.Full_Name, String.Empty);
            }
            catch (Exception)
            {
                // This is not critical
            }

            // Save the MARC file
            MarcXML_File_ReaderWriter marcWriter = new MarcXML_File_ReaderWriter();
            string errorMessage;
            marcWriter.Write_Metadata(Item.Source_Directory + "\\marc.xml", Item, options, out errorMessage);

            // Determine the server folder
            string serverNetworkFolder = Engine_ApplicationCache_Gateway.Settings.Image_Server_Network + Item.Web.AssocFilePath;

            // Create the folder
            if (!Directory.Exists(serverNetworkFolder))
            {
                Directory.CreateDirectory(serverNetworkFolder);
                if (!Directory.Exists(serverNetworkFolder + "\\" + Engine_ApplicationCache_Gateway.Settings.Backup_Files_Folder_Name))
                    Directory.CreateDirectory(serverNetworkFolder + "\\" + Engine_ApplicationCache_Gateway.Settings.Backup_Files_Folder_Name);
            }
            else
            {
                if (!Directory.Exists(serverNetworkFolder + "\\" + Engine_ApplicationCache_Gateway.Settings.Backup_Files_Folder_Name))
                    Directory.CreateDirectory(serverNetworkFolder + "\\" + Engine_ApplicationCache_Gateway.Settings.Backup_Files_Folder_Name);

                // Rename any existing standard mets to keep a backup
                if (File.Exists(serverNetworkFolder + "\\" + Item.BibID + "_" + Item.VID + ".mets.xml"))
                {
                    FileInfo currentMetsFileInfo = new FileInfo(serverNetworkFolder + "\\" + Item.BibID + "_" + Item.VID + ".mets.xml");
                    DateTime lastModDate = currentMetsFileInfo.LastWriteTime;
                    File.Copy(serverNetworkFolder + "\\" + Item.BibID + "_" + Item.VID + ".mets.xml", serverNetworkFolder + "\\" + Engine_ApplicationCache_Gateway.Settings.Backup_Files_Folder_Name + "\\" + Item.BibID + "_" + Item.VID + "_" + lastModDate.Year + "_" + lastModDate.Month + "_" + lastModDate.Day + ".mets.bak", true);
                }
            }

            // Copy the static HTML page over first
            if (File.Exists(user_bib_vid_process_directory + "\\" + Item.BibID + "_" + Item.VID + ".html"))
            {
                File.Copy(user_bib_vid_process_directory + "\\" + Item.BibID + "_" + Item.VID + ".html", serverNetworkFolder + "\\" + Engine_ApplicationCache_Gateway.Settings.Backup_Files_Folder_Name + "\\" + Item.BibID + "_" + Item.VID + ".html", true);
                File.Delete(user_bib_vid_process_directory + "\\" + Item.BibID + "_" + Item.VID + ".html");
            }

            // Copy all the files 
            string[] allFiles = Directory.GetFiles(user_bib_vid_process_directory);
            foreach (string thisFile in allFiles)
            {
                string destination_file = serverNetworkFolder + "\\" + (new FileInfo(thisFile)).Name;
                File.Copy(thisFile, destination_file, true);
            }

            // Add this to the cache
            Engine_ApplicationCache_Gateway.Items.Add_SobekCM_Item(Item, false);

            // Now, delete all the files here
            string[] all_files = Directory.GetFiles(user_bib_vid_process_directory);
            foreach (string thisFile in all_files)
            {
                try
                {
                    File.Delete(thisFile);
                }
                catch
                {

                }
            }

            // Clear the User-specific and global cache of this Item 
            CachedDataManager.Remove_Digital_Resource_Object(User.UserID, Item.BibID, Item.VID, null);
            CachedDataManager.Remove_Digital_Resource_Object(Item.BibID, Item.VID, null);
            CachedDataManager.Remove_Items_In_Title(Item.BibID, null);

            // Also clear any searches or browses ( in the future could refine this to only remove those
            // that are impacted by this save... but this is good enough for now )
            CachedDataManager.Clear_Search_Results_Browses();

            return true;
        }
    }
}
