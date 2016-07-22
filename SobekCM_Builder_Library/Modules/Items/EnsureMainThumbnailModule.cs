#region Using directives

using System.Collections.Generic;
using System.IO;
using System.Reflection;
using SobekCM.Builder_Library.Settings;
using SobekCM.Library.Database;
using SobekCM.Resource_Object.Divisions;

#endregion

namespace SobekCM.Builder_Library.Modules.Items
{
    /// <summary> Item-level submission package module ensures a main thumbnail has been selected for this digital resource </summary>
    /// <remarks> This class implements the <see cref="abstractSubmissionPackageModule" /> abstract class and implements the <see cref="iSubmissionPackageModule" /> interface. </remarks>
    public class EnsureMainThumbnailModule : abstractSubmissionPackageModule
    {
        /// <summary> Ensures a main thumbnail has been selected for this digital resource </summary>
        /// <param name="Resource"> Incoming digital resource object </param>
        /// <returns> TRUE if processing can continue, FALSE if a critical error occurred which should stop all processing </returns>
        public override bool DoWork(Incoming_Digital_Resource Resource)
        {
            // Ensure a thumbnail is attached
            if ((Resource.Metadata.Behaviors.Main_Thumbnail.Length == 0) ||
                ((Resource.Metadata.Behaviors.Main_Thumbnail.IndexOf("http:") < 0) && (!File.Exists(Path.Combine(Resource.Resource_Folder, Resource.Metadata.Behaviors.Main_Thumbnail)))))
            {
                // Look for a valid thumbnail
                if (File.Exists(Path.Combine(Resource.Resource_Folder, "mainthm.jpg")))
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
                                    if (File.Exists(Path.Combine(MultiInstance_Builder_Settings.Builder_Executable_Directory, "images\\multimedia.jpg")))
                                    {
                                        File.Copy(Path.Combine(MultiInstance_Builder_Settings.Builder_Executable_Directory, "images\\multimedia.jpg"), Path.Combine(Resource.Resource_Folder, "multimediathm.jpg"), true);
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

            return true;
        }
    }
}
