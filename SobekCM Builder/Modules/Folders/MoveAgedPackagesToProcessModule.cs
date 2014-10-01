using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SobekCM.Library.Builder;
using SobekCM.Library.Settings;

namespace SobekCM.Builder.Modules.Folders
{
    public class MoveAgedPackagesToProcessModule : abstractFolderModule
    {
        public override void DoWork(Builder_Source_Folder BuilderFolder, List<Incoming_Digital_Resource> IncomingPackages, List<Incoming_Digital_Resource> Deletes)
        {

            try
            {
                // Move all eligible packages from the FTP folders to the processing folders
                if (SobekCM_Library_Settings.Builder_Verbose_Flag)
                    OnProcess("Worker_BulkLoader.Move_Appropriate_Inbound_Packages_To_Processing: Checking incoming folder " + BuilderFolder.Inbound_Folder, String.Empty, String.Empty, String.Empty, -1);

                if (BuilderFolder.Items_Exist_In_Inbound)
                {
                    if (SobekCM_Library_Settings.Builder_Verbose_Flag)
                        OnProcess("Worker_BulkLoader.Move_Appropriate_Inbound_Packages_To_Processing: Found either files or subdirectories in " + BuilderFolder.Inbound_Folder, String.Empty, String.Empty, String.Empty, -1);

                    if (SobekCM_Library_Settings.Builder_Verbose_Flag)
                        OnProcess("Checking inbound packages for aging and possibly moving to processing", String.Empty, String.Empty, String.Empty, -1);

                    String outMessage;
                    if (!BuilderFolder.Move_From_Inbound_To_Processing(out outMessage))
                    {
                        if (outMessage.Length > 0) OnError(outMessage, String.Empty, String.Empty, -1);
                        OnError("Unspecified error moving files from inbound to processing", String.Empty, String.Empty, -1);
                    }
                    else
                    {
                        if ((SobekCM_Library_Settings.Builder_Verbose_Flag) && (outMessage.Length > 0))
                            OnProcess(outMessage, String.Empty, String.Empty, String.Empty, -1);
                    }

                }
                else if (SobekCM_Library_Settings.Builder_Verbose_Flag)
                    OnProcess("Worker_BulkLoader.Move_Appropriate_Inbound_Packages_To_Processing: No subdirectories or files found in incoming folder " + BuilderFolder.Inbound_Folder, String.Empty, String.Empty, String.Empty, -1);
            }
            catch (Exception ee)
            {
                OnError("Error in harvesting packages from inbound folders to processing\n" + ee.Message, String.Empty, String.Empty, -1);
            }
        }
    }
}
