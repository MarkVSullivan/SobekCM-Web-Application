#region Using directives

using System.IO;
using SobekCM.Library;
using SobekCM.Library.Settings;

#endregion

namespace SobekCM.Builder_Library.Modules.Items
{
    public class CreateStaticVersionModule : abstractSubmissionPackageModule
    {
        private Static_Pages_Builder staticBuilder;

        public override void ReleaseResources()
        {
            staticBuilder = null;
        }

        public override void DoWork(Incoming_Digital_Resource Resource)
        {
            // Only build the statyic builder when needed 
            if (staticBuilder == null)
            {
                // Create the new statics page builder
                staticBuilder = new Static_Pages_Builder(SobekCM_Library_Settings.Application_Server_URL, SobekCM_Library_Settings.Static_Pages_Location, SobekCM_Library_Settings.Application_Server_Network);
            }

            // Save the static page and then copy to all the image servers
            try
            {
                if (!Directory.Exists(Resource.Resource_Folder + "\\" + SobekCM_Library_Settings.BACKUP_FILES_FOLDER_NAME))
                    Directory.CreateDirectory(Resource.Resource_Folder + "\\" + SobekCM_Library_Settings.BACKUP_FILES_FOLDER_NAME);

                string static_file = Resource.Resource_Folder + "\\" + SobekCM_Library_Settings.BACKUP_FILES_FOLDER_NAME + "\\" + Resource.Metadata.BibID + "_" + Resource.Metadata.VID + ".html";
                staticBuilder.Create_Item_Citation_HTML(Resource.Metadata, static_file, Resource.Resource_Folder);

                if (!File.Exists(static_file))
                {
                    OnError("Error creating static page for this resource", Resource.BibID + ":" + Resource.VID, Resource.METS_Type_String, Resource.BuilderLogId);
                }
                else
                {
                    // Also copy to the static page location server
                    string web_server_file_version = SobekCM_Library_Settings.Static_Pages_Location + Resource.File_Root + "\\" + Resource.BibID + "_" + Resource.VID + ".html";
                    if (!Directory.Exists(SobekCM_Library_Settings.Static_Pages_Location + Resource.File_Root))
                        Directory.CreateDirectory(SobekCM_Library_Settings.Static_Pages_Location + Resource.File_Root);
                    File.Copy(static_file, web_server_file_version, true);
                }
            }
            catch
            {
                OnError("Error creating static page for this resource", Resource.BibID + ":" + Resource.VID, Resource.METS_Type_String, Resource.BuilderLogId);
            }
        }
    }
}
