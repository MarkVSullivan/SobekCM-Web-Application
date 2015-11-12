#region Using directives

using System;
using System.Collections.Generic;
using System.IO;

#endregion

namespace SobekCM.Builder_Library.Modules.Items
{
    /// <summary> Item-level submission package module moves all the incoming files and images to the image server,
    /// while keeping track of which images were newly added </summary>
    /// <remarks> This class implements the <see cref="abstractSubmissionPackageModule" /> abstract class and implements the <see cref="iSubmissionPackageModule" /> interface. </remarks>
    public class MoveFilesToImageServerModule : abstractSubmissionPackageModule
    {
        /// <summary>  </summary>
        /// <param name="Resource"> Incoming digital resource object </param>
        /// <returns> TRUE if processing can continue, FALSE if a critical error occurred which should stop all processing </returns>
        public override bool DoWork(Incoming_Digital_Resource Resource)
        {
            Rename_Any_Received_METS_File(Resource);

            // Determine if this is actually already IN the final image server spot first
            // Determine the file root for this
            Resource.File_Root = Resource.BibID.Substring(0, 2) + "\\" + Resource.BibID.Substring(2, 2) + "\\" + Resource.BibID.Substring(4, 2) + "\\" + Resource.BibID.Substring(6, 2) + "\\" + Resource.BibID.Substring(8, 2);

            // Determine the destination folder for this resource
            string serverPackageFolder = Settings.Servers.Image_Server_Network + Resource.File_Root + "\\" + Resource.VID;

            // If this is re-processing the resource in situ, then just return.. nothing to move
            if (NormalizePath(Resource.Resource_Folder) == NormalizePath(serverPackageFolder))
                return true;

            // Clear the list of new images files here, since moving the package will recalculate this
            Resource.NewImageFiles.Clear();

            // Move all files to the image server
            if (!Move_All_Files_To_Image_Server(Resource, Resource.NewImageFiles, serverPackageFolder))
            {
                OnError("Error moving some files to the image server for " + Resource.BibID + ":" + Resource.VID, Resource.BibID + ":" + Resource.VID, Resource.METS_Type_String, Resource.BuilderLogId);
                return false;
            }

            return true;
        }

        private void Rename_Any_Received_METS_File(Incoming_Digital_Resource ResourcePackage)
        {
            string recd_filename = "recd_" + DateTime.Now.Year + "_" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "_" + DateTime.Now.Day.ToString().PadLeft(2, '0') + ".mets.bak";

            // If a renamed file already exists for this year, delete the incoming with that name (shouldn't exist)
            if (File.Exists(ResourcePackage.Resource_Folder + "\\" + recd_filename))
                File.Delete(ResourcePackage.Resource_Folder + "\\" + recd_filename);

            if (File.Exists(ResourcePackage.Resource_Folder + "\\" + ResourcePackage.BibID + "_" + ResourcePackage.VID + ".mets"))
            {
                File.Move(ResourcePackage.Resource_Folder + "\\" + ResourcePackage.BibID + "_" + ResourcePackage.VID + ".mets", ResourcePackage.Resource_Folder + "\\" + recd_filename);
                ResourcePackage.METS_File = recd_filename;
                return;
            }
            if (File.Exists(ResourcePackage.Resource_Folder + "\\" + ResourcePackage.BibID + "_" + ResourcePackage.VID + ".mets.xml"))
            {
                File.Move(ResourcePackage.Resource_Folder + "\\" + ResourcePackage.BibID + "_" + ResourcePackage.VID + ".mets.xml", ResourcePackage.Resource_Folder + "\\" + recd_filename);
                ResourcePackage.METS_File = recd_filename;
                return;
            }
            if (File.Exists(ResourcePackage.Resource_Folder + "\\" + ResourcePackage.BibID + ".mets"))
            {
                File.Move(ResourcePackage.Resource_Folder + "\\" + ResourcePackage.BibID + ".mets", ResourcePackage.Resource_Folder + "\\" + recd_filename);
                ResourcePackage.METS_File = recd_filename;
                return;
            }
            if (File.Exists(ResourcePackage.Resource_Folder + "\\" + ResourcePackage.BibID + ".mets.xml"))
            {
                File.Move(ResourcePackage.Resource_Folder + "\\" + ResourcePackage.BibID + ".mets.xml", ResourcePackage.Resource_Folder + "\\" + recd_filename);
                ResourcePackage.METS_File = recd_filename;
            }
        }

        /// <summary> Normalized this path </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        private static string NormalizePath(string Path)
        {
            return System.IO.Path.GetFullPath(new Uri(Path).LocalPath)
                       .TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar)
                       .ToUpperInvariant();
        }


        private bool Move_All_Files_To_Image_Server(Incoming_Digital_Resource ResourcePackage, List<string> NewImageFiles, string ServerPackageFolder)
        {
            try
            {

                // Make sure a directory exists here
                if (!Directory.Exists(ServerPackageFolder))
                {
                    Directory.CreateDirectory(ServerPackageFolder);
                }
                else
                {
                    // COpy any existing mets file to keep what the METS looked like before this change
                    if (File.Exists(ServerPackageFolder + "\\" + ResourcePackage.BibID + "_" + ResourcePackage.VID + ".mets.xml"))
                    {
                        File.Copy(ServerPackageFolder + "\\" + ResourcePackage.BibID + "_" + ResourcePackage.VID + ".mets.xml", ServerPackageFolder + "\\" + ResourcePackage.BibID + "_" + ResourcePackage.VID + "_" + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + ".mets.bak", true);
                    }
                }

                // Move all the files to the digital resource file server
                string[] all_files = Directory.GetFiles(ResourcePackage.Resource_Folder);
                foreach (string thisFile in all_files)
                {
                    FileInfo thisFileInfo = new FileInfo(thisFile);
                    string new_file = ServerPackageFolder + "/" + thisFileInfo.Name;

                    // Keep the list of new image files being copied, which may be used later
                    if (Settings.Page_Image_Extensions.Contains(thisFileInfo.Extension.ToUpper().Replace(".", "")))
                        NewImageFiles.Add(thisFileInfo.Name);

                    // If the file exists, delete it, 
                    if (File.Exists(new_file))
                    {
                        File.Delete(new_file);
                    }

                    // Move the file over
                    File.Move(thisFile, new_file);
                }

                // Remove the directory and any files which somehow remain
                ResourcePackage.Delete();

                // Since the package has been moved, repoint the resource
                ResourcePackage.Resource_Folder = ServerPackageFolder;

                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
