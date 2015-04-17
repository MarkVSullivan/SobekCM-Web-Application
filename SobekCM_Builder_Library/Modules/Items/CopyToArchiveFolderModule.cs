#region Using directives

using System;
using System.IO;
using System.Text.RegularExpressions;

#endregion

namespace SobekCM.Builder_Library.Modules.Items
{
    public class CopyToArchiveFolderModule : abstractSubmissionPackageModule
    {
        public override bool DoWork(Incoming_Digital_Resource Resource)
        {
            string resourceFolder = Resource.Resource_Folder;

            // Delete any pre-archive deletes
            if (Settings.PreArchive_Files_To_Delete.Length > 0)
            {
                // Get the list of files again
                string[] files = Directory.GetFiles(resourceFolder);
                foreach (string thisFile in files)
                {
                    FileInfo thisFileInfo = new FileInfo(thisFile);
                    if (Regex.Match(thisFileInfo.Name, Settings.PreArchive_Files_To_Delete, RegexOptions.IgnoreCase).Success)
                    {
                        File.Delete(thisFile);
                    }
                }
            }

            // Archive any files, per the folder instruction
            if (!Archive_Any_Files(Resource))
                return false;

            // Delete any remaining post-archive deletes
            if (Settings.PostArchive_Files_To_Delete.Length > 0)
            {
                // Get the list of files again
                string[] files = Directory.GetFiles(resourceFolder);
                foreach (string thisFile in files)
                {
                    FileInfo thisFileInfo = new FileInfo(thisFile);
                    if (Regex.Match(thisFileInfo.Name, Settings.PostArchive_Files_To_Delete, RegexOptions.IgnoreCase).Success)
                    {
                        File.Delete(thisFile);
                    }
                }
            }

            return true;
        }

        private bool Archive_Any_Files(Incoming_Digital_Resource ResourcePackage)
        {
            bool returnValue = true;

            // First see if this folder is even eligible for archiving and an archive drop box exists
            if ((Settings.Archive_DropBox.Length > 0) && ((ResourcePackage.Source_Folder.Archive_All_Files) || (ResourcePackage.Source_Folder.Archive_TIFFs)))
            {
                // Get the list of TIFFs
                string[] tiff_files = Directory.GetFiles(ResourcePackage.Resource_Folder, "*.tif");

                // Now, see if we should archive THIS folder, based on upper level folder properties
                if ((ResourcePackage.Source_Folder.Archive_All_Files) || ((tiff_files.Length > 0) && (ResourcePackage.Source_Folder.Archive_TIFFs)))
                {
                    try
                    {
                        // Calculate and create the archive directory
                        string archiveDirectory = Settings.Archive_DropBox + "\\" + ResourcePackage.BibID + "\\" + ResourcePackage.VID;
                        if (!Directory.Exists(archiveDirectory))
                            Directory.CreateDirectory(archiveDirectory);

                        // Copy ALL the files over?
                        if (ResourcePackage.Source_Folder.Archive_All_Files)
                        {
                            string[] archive_files = Directory.GetFiles(ResourcePackage.Resource_Folder);
                            foreach (string thisFile in archive_files)
                            {
                                string filename = Path.GetFileName(thisFile);
                                if ( String.Compare(filename, "thumbs.db", StringComparison.OrdinalIgnoreCase) != 0 )
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
                        returnValue = false;
                    }
                }
            }

            return returnValue;
        }

    }
}
