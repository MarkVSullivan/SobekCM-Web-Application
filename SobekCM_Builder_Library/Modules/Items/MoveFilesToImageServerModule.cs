#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using SobekCM.Library.Settings;

#endregion

namespace SobekCM.Builder_Library.Modules.Items
{
    public class MoveFilesToImageServerModule : abstractSubmissionPackageModule
    {
        public override void DoWork(Incoming_Digital_Resource Resource)
        {
            // Determine if this is actually already IN the final image server spot first
            // Determine the file root for this
            Resource.File_Root = Resource.BibID.Substring(0, 2) + "\\" + Resource.BibID.Substring(2, 2) + "\\" + Resource.BibID.Substring(4, 2) + "\\" + Resource.BibID.Substring(6, 2) + "\\" + Resource.BibID.Substring(8, 2);

            // Determine the destination folder for this resource
            string serverPackageFolder = InstanceWide_Settings_Singleton.Settings.Image_Server_Network + Resource.File_Root + "\\" + Resource.VID;

            throw new NotImplementedException();

            // Clear the list of new images files here, since moving the package will recalculate this
            Resource.NewImageFiles.Clear();

            // Move all files to the image server
            if (!Move_All_Files_To_Image_Server(Resource, Resource.NewImageFiles, serverPackageFolder))
            {
                OnError("Error moving some files to the image server for " + Resource.BibID + ":" + Resource.VID, Resource.BibID + ":" + Resource.VID, Resource.METS_Type_String, Resource.BuilderLogId);
            }
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
                    if (InstanceWide_Settings_Singleton.Settings.Page_Image_Extensions.Contains(thisFileInfo.Extension.ToUpper().Replace(".", "")))
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
