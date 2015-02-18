using System;
using System.IO;
using SobekCM.Engine_Library.Solr;
using SobekCM.Library.Database;

namespace SobekCM.Builder_Library.Modules.Items
{
    public class DeleteItemModule : abstractSubmissionPackageModule
    {
        public override bool DoWork(Incoming_Digital_Resource Resource)
        {
            // Read the METS and load the basic information before continuing
            Resource.Load_METS();
            SobekCM_Database.Add_Minimum_Builder_Information(Resource.Metadata);

            Resource.BuilderLogId = OnProcess("........Processing '" + Resource.Folder_Name + "'", "Standard", Resource.BibID + ":" + Resource.VID, Resource.METS_Type_String, -1);

            SobekCM_Database.Builder_Clear_Item_Error_Log(Resource.BibID, Resource.VID, "SobekCM Builder");

            Resource.File_Root = Resource.BibID.Substring(0, 2) + "\\" + Resource.BibID.Substring(2, 2) + "\\" + Resource.BibID.Substring(4, 2) + "\\" + Resource.BibID.Substring(6, 2) + "\\" + Resource.BibID.Substring(8);
            string existing_folder = Settings.Image_Server_Network + Resource.File_Root + "\\" + Resource.VID;

            // Remove from the primary collection area
            try
            {
                if (Directory.Exists(existing_folder))
                {
                    // Make sure the delete folder exists
                    if (!Directory.Exists(Settings.Image_Server_Network + "\\RECYCLE BIN"))
                    {
                        Directory.CreateDirectory(Settings.Image_Server_Network + "\\RECYCLE BIN");
                    }

                    // Create the final directory
                    string final_folder = Settings.Image_Server_Network + "\\RECYCLE BIN\\" + Resource.File_Root + "\\" + Resource.VID;
                    if (!Directory.Exists(final_folder))
                    {
                        Directory.CreateDirectory(final_folder);
                    }

                    // Move each file
                    string[] delete_files = Directory.GetFiles(existing_folder);
                    foreach (string thisDeleteFile in delete_files)
                    {
                        string destination_file = final_folder + "\\" + Path.GetFileName(thisDeleteFile);
                        if (File.Exists(destination_file))
                            File.Delete(destination_file);
                        File.Move(thisDeleteFile, destination_file);
                    }
                }
            }
            catch (Exception ee)
            {
                OnError("Unable to move resource ( " + Resource.BibID + ":" + Resource.VID + " ) to deletes", Resource.BibID + ":" + Resource.VID, Resource.METS_Type_String, Resource.BuilderLogId);
                OnError(ee.Message, Resource.BibID + ":" + Resource.VID, Resource.METS_Type_String, Resource.BuilderLogId);
                return false;
            }

            // Delete the static page
            string static_page1 = Settings.Static_Pages_Location + Resource.BibID.Substring(0, 2) + "\\" + Resource.BibID.Substring(2, 2) + "\\" + Resource.BibID.Substring(4, 2) + "\\" + Resource.BibID.Substring(6, 2) + "\\" + Resource.BibID.Substring(8) + "\\" + Resource.VID + "\\" + Resource.BibID + "_" + Resource.VID + ".html";
            if (File.Exists(static_page1))
            {
                File.Delete(static_page1);
            }
            string static_page2 = Settings.Static_Pages_Location + Resource.BibID.Substring(0, 2) + "\\" + Resource.BibID.Substring(2, 2) + "\\" + Resource.BibID.Substring(4, 2) + "\\" + Resource.BibID.Substring(6, 2) + "\\" + Resource.BibID.Substring(8) + "\\" + Resource.BibID + "_" + Resource.VID + ".html";
            if (File.Exists(static_page2))
            {
                File.Delete(static_page2);
            }

            // Delete the file from the database
            SobekCM_Database.Delete_SobekCM_Item(Resource.BibID, Resource.VID, true, "Deleted upon request by builder");

            // Delete from the solr/lucene indexes
            if (Settings.Document_Solr_Index_URL.Length > 0)
            {
                try
                {
                    Solr_Controller.Delete_Resource_From_Index(Settings.Document_Solr_Index_URL, Settings.Page_Solr_Index_URL, Resource.BibID, Resource.VID);
                }
                catch (Exception ee)
                {
                    OnError("Error deleting item from the Solr/Lucene index.  The index may not reflect this delete.", Resource.BibID + ":" + Resource.VID, Resource.METS_Type_String, Resource.BuilderLogId);
                    OnError("Solr Error: " + ee.Message, Resource.BibID + ":" + Resource.VID, Resource.METS_Type_String, Resource.BuilderLogId);
                    return false;
                }
            }

            // Delete the handled METS file and package
            Resource.Delete();
            return true;
        }
    }
}
