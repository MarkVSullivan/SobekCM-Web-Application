using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SobekCM.Library.Builder;
using SobekCM.Library.Database;
using SobekCM.Resource_Object.Divisions;

namespace SobekCM.Builder.Modules.Items
{
    public class EnsureMainThumbnailModule : abstractSubmissionPackageModule
    {
        public override void DoWork(Incoming_Digital_Resource Resource)
        {
            // Ensure a thumbnail is attached
            if ((Resource.Metadata.Behaviors.Main_Thumbnail.Length == 0) ||
                ((Resource.Metadata.Behaviors.Main_Thumbnail.IndexOf("http:") < 0) && (!File.Exists(Resource.Resource_Folder + "\\" + Resource.Metadata.Behaviors.Main_Thumbnail))))
            {
                // Look for a valid thumbnail
                if (File.Exists(Resource.Resource_Folder + "\\mainthm.jpg"))
                    Resource.Metadata.Behaviors.Main_Thumbnail = "mainthm.jpg";
                else
                {
                    string[] jpeg_files = Directory.GetFiles(Resource.Resource_Folder, "*thm.jpg");
                    if (jpeg_files.Length > 0)
                    {
                        Resource.Metadata.Behaviors.Main_Thumbnail = (new FileInfo(jpeg_files[0])).Name;
                    }
                    else
                    {
                        if (Resource.Metadata.Divisions.Page_Count == 0)
                        {
                            List<SobekCM_File_Info> downloads = Resource.Metadata.Divisions.Download_Other_Files;
                            foreach (SobekCM_File_Info thisDownloadFile in downloads)
                            {
                                string mimetype = thisDownloadFile.MIME_Type(thisDownloadFile.File_Extension).ToUpper();
                                if ((mimetype.IndexOf("AUDIO") >= 0) || (mimetype.IndexOf("VIDEO") >= 0))
                                {
                                    if (File.Exists(Application.StartupPath + "\\images\\multimedia.jpg"))
                                    {
                                        File.Copy(Application.StartupPath + "\\images\\multimedia.jpg", Resource.Resource_Folder + "\\multimediathm.jpg", true);
                                        Resource.Metadata.Behaviors.Main_Thumbnail = "multimediathm.jpg";
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }

                // Should this be saved?
                if ((Resource.Metadata.Web.ItemID > 0) && (Resource.Metadata.Behaviors.Main_Thumbnail.Length > 0))
                {
                    SobekCM_Database.Set_Item_Main_Thumbnail(Resource.BibID, Resource.VID, Resource.Metadata.Behaviors.Main_Thumbnail);
                }
            }
        }
    }
}
