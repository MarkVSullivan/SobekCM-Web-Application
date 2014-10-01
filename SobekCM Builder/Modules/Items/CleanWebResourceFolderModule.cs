using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SobekCM.Library.Builder;
using SobekCM.Library.Settings;

namespace SobekCM.Builder.Modules.Items
{
    public class CleanWebResourceFolderModule : abstractSubmissionPackageModule
    {
        public override void DoWork(Incoming_Digital_Resource Resource)
        {
            try
            {
                // Insure subfolder exists
                string backup_dir = Resource.Resource_Folder + "\\" + SobekCM_Library_Settings.BACKUP_FILES_FOLDER_NAME;
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
        }
    }
}
