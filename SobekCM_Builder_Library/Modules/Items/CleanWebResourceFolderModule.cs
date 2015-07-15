#region Using directives

using System.IO;

#endregion

namespace SobekCM.Builder_Library.Modules.Items
{
    /// <summary> Item-level submission package module performs some cleanup on digital resource folders
    /// from previous versions that had some extraneous files and didn't store the backup files in a subfolder </summary>
    /// <remarks> This class implements the <see cref="abstractSubmissionPackageModule" /> abstract class and implements the <see cref="iSubmissionPackageModule" /> interface. </remarks>
    public class CleanWebResourceFolderModule : abstractSubmissionPackageModule
    {
        /// <summary> Performs some cleanup on digital resource folders from previous versions that had some 
        /// extraneous files and didn't store the backup files in a subfolder </summary>
        /// <param name="Resource"> Incoming digital resource object </param>
        /// <returns> TRUE if processing can continue, FALSE if a critical error occurred which should stop all processing </returns>
        public override bool DoWork(Incoming_Digital_Resource Resource)
        {
            try
            {
                // Insure subfolder exists
                string backup_dir = Resource.Resource_Folder + "\\" + Settings.Backup_Files_Folder_Name;
                if (!Directory.Exists(backup_dir))
                {
                    Directory.CreateDirectory(backup_dir);
                }

                // Look for backup mets
                string[] backup_files = Directory.GetFiles(Resource.Resource_Folder, "*.mets.bak");
                foreach (string thisBackUpFile in backup_files)
                {
                    string name = Path.GetFileName(thisBackUpFile);
                    if (File.Exists(backup_dir + "\\" + name))
                        File.Delete(backup_dir + "\\" + name);
                    File.Move(thisBackUpFile, backup_dir + "\\" + name);
                }

                // Look for the original mets
                if (File.Exists(Resource.Resource_Folder + "\\original.mets.xml"))
                {
                    if (File.Exists(backup_dir + "\\original.mets.xml"))
                        File.Delete(backup_dir + "\\original.mets.xml");
                    File.Move(Resource.Resource_Folder + "\\original.mets.xml", backup_dir + "\\original.mets.xml");
                }

                // If the citation_mets.xml file exists, delete that
                if (File.Exists(Resource.Resource_Folder + "\\citation_mets.xml"))
                {
                    File.Delete(Resource.Resource_Folder + "\\citation_mets.xml");
                }
            }
            catch
            {
                // Log as a warning
                OnProcess("WARNING: Unable to perform final cleanup on web folder", "Warning", Resource.BibID + ":" + Resource.VID, Resource.METS_Type_String, Resource.BuilderLogId);
            }

            return true;
        }
    }
}
