#region Using directives

using System;
using System.IO;
using System.Text.RegularExpressions;

#endregion

namespace SobekCM.Builder_Library.Modules.Items
{
    /// <summary> Item-level submission package module copies all incoming files into an archive
    /// folder, where an archiving process can pickup the new files  </summary>
    /// <remarks> This class implements the <see cref="abstractSubmissionPackageModule" /> abstract class and implements the <see cref="iSubmissionPackageModule" /> interface. </remarks>
    public class CopyToArchiveFolderModule : abstractSubmissionPackageModule
    {
        /// <summary> Copies all incoming files into an archive folder, where an archiving process can pickup the new files  </summary>
        /// <param name="Resource"> Incoming digital resource object </param>
        /// <returns> TRUE if processing can continue, FALSE if a critical error occurred which should stop all processing </returns>
        public override bool DoWork(Incoming_Digital_Resource Resource)
        {
            string resourceFolder = Resource.Resource_Folder;

            // Delete any pre-archive deletes
            if ( !String.IsNullOrEmpty(Settings.Archive.PreArchive_Files_To_Delete))
            {
                // Get the list of files again
                string[] files = Directory.GetFiles(resourceFolder);
                foreach (string thisFile in files)
                {
                    FileInfo thisFileInfo = new FileInfo(thisFile);
                    if (Regex.Match(thisFileInfo.Name, Settings.Archive.PreArchive_Files_To_Delete, RegexOptions.IgnoreCase).Success)
                    {
                        File.Delete(thisFile);
                    }
                }
            }

            // Archive any files, per the folder instruction
            if (!Archive_Any_Files(Resource))
                return false;

            // Delete any remaining post-archive deletes
            if (!String.IsNullOrEmpty(Settings.Archive.PostArchive_Files_To_Delete))
            {
                // Get the list of files again
                string[] files = Directory.GetFiles(resourceFolder);
                foreach (string thisFile in files)
                {
                    FileInfo thisFileInfo = new FileInfo(thisFile);
                    if (Regex.Match(thisFileInfo.Name, Settings.Archive.PostArchive_Files_To_Delete, RegexOptions.IgnoreCase).Success)
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
            if (( !String.IsNullOrEmpty(Settings.Archive.Archive_DropBox)) && ((ResourcePackage.Source_Folder.Archive_All_Files) || (ResourcePackage.Source_Folder.Archive_TIFFs)))
            {

               // OnProcess("\t\tCopying files to the archive", "Copy To Archive", ResourcePackage.BibID + ":" + ResourcePackage.VID, String.Empty, -1);

                // Get the list of TIFFs
                string[] tiff_files = Directory.GetFiles(ResourcePackage.Resource_Folder, "*.tif");

                // Now, see if we should archive THIS folder, based on upper level folder properties
                if ((ResourcePackage.Source_Folder.Archive_All_Files) || ((tiff_files.Length > 0) && (ResourcePackage.Source_Folder.Archive_TIFFs)))
                {
                    try
                    {
                        // Calculate and create the archive directory
                        string archiveDirectory = Settings.Archive.Archive_DropBox + "\\" + ResourcePackage.BibID + "\\" + ResourcePackage.VID;
                        if (!Directory.Exists(archiveDirectory))
                            Directory.CreateDirectory(archiveDirectory);

                        // Copy ALL the files over?
                        if (ResourcePackage.Source_Folder.Archive_All_Files)
                        {
                            string[] archive_files = Directory.GetFiles(ResourcePackage.Resource_Folder);
                            foreach (string thisFile in archive_files)
                            {
                                string filename = Path.GetFileName(thisFile);
                                if (String.Compare(filename, "thumbs.db", StringComparison.OrdinalIgnoreCase) != 0)
                                {
                                    string newFile = archiveDirectory + "\\" + filename;
                                  //  OnProcess("\t\tCopying file ( " + thisFile + " -->" + newFile + ")", "Copy To Archive", ResourcePackage.BibID + ":" + ResourcePackage.VID, String.Empty, -1);

                                    File.Copy(thisFile, newFile, true);
                                }
                            }
                        }
                        else
                        {
                            string[] archive_tiff_files = Directory.GetFiles(ResourcePackage.Resource_Folder, "*.tif");
                            foreach (string thisFile in archive_tiff_files)
                            {
                                string filename = Path.GetFileName(thisFile);
                                string newFile = archiveDirectory + "\\" + filename;
                              //  OnProcess("\t\tCopying file ( " + thisFile + " -->" + newFile + ")", "Copy To Archive", ResourcePackage.BibID + ":" + ResourcePackage.VID, String.Empty, -1);

                                File.Copy(thisFile, newFile, true);
                            }
                        }
                    }
                    catch (Exception ee)
                    {
                        OnError("Copy to archive failed for " + ResourcePackage.BibID + ":" + ResourcePackage.VID + "\n" + ee.Message, ResourcePackage.BibID + ":" + ResourcePackage.VID, ResourcePackage.METS_Type_String, ResourcePackage.BuilderLogId);
                        OnError(ee.StackTrace, ResourcePackage.BibID + ":" + ResourcePackage.VID, ResourcePackage.METS_Type_String, ResourcePackage.BuilderLogId);

                        returnValue = false;
                    }
                }
            }

            return returnValue;
        }

    }
}
