#region Using directives

using System;
using System.IO;
using System.Text.RegularExpressions;
using SobekCM.Library.Settings;

#endregion

namespace SobekCM.Builder_Library.Modules.Items
{
    public class CopyToArchiveFolderModule : abstractSubmissionPackageModule
    {
        public override void DoWork(Incoming_Digital_Resource Resource)
        {
            string resourceFolder = Resource.Resource_Folder;

            // Delete any pre-archive deletes
            if (SobekCM_Library_Settings.PreArchive_Files_To_Delete.Length > 0)
            {
                // Get the list of files again
                string[] files = Directory.GetFiles(resourceFolder);
                foreach (string thisFile in files)
                {
                    FileInfo thisFileInfo = new FileInfo(thisFile);
                    if (Regex.Match(thisFileInfo.Name, SobekCM_Library_Settings.PreArchive_Files_To_Delete, RegexOptions.IgnoreCase).Success)
                    {
                        File.Delete(thisFile);
                    }
                }
            }

            // Archive any files, per the folder instruction
            Archive_Any_Files(Resource);

            // Delete any remaining post-archive deletes
            if (SobekCM_Library_Settings.PostArchive_Files_To_Delete.Length > 0)
            {
                // Get the list of files again
                string[] files = Directory.GetFiles(resourceFolder);
                foreach (string thisFile in files)
                {
                    FileInfo thisFileInfo = new FileInfo(thisFile);
                    if (Regex.Match(thisFileInfo.Name, SobekCM_Library_Settings.PostArchive_Files_To_Delete, RegexOptions.IgnoreCase).Success)
                    {
                        File.Delete(thisFile);
                    }
                }
            }
        }

        private void Archive_Any_Files(Incoming_Digital_Resource ResourcePackage)
        {
            // First see if this folder is even eligible for archiving and an archive drop box exists
            if ((SobekCM_Library_Settings.Archive_DropBox.Length > 0) && ((ResourcePackage.Source_Folder.Archive_All_Files) || (ResourcePackage.Source_Folder.Archive_TIFFs)))
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
                        OnError("Copy to archive failed for " + ResourcePackage.BibID + ":" + ResourcePackage.VID + "\n" + ee.Message, ResourcePackage.BibID + ":" + ResourcePackage.VID, ResourcePackage.METS_Type_String, ResourcePackage.BuilderLogId);
                    }
                }
            }
        }

    }
}
