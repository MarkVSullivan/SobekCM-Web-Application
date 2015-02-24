#region Using directives

using System;
using System.IO;
using SobekCM.Builder_Library.Settings;

#endregion

namespace SobekCM.Builder_Library.Modules.Items
{
    public class CreatePdfThumbnailModule : abstractSubmissionPackageModule
    {
        public override bool DoWork(Incoming_Digital_Resource Resource)
        {
            string resourceFolder = Resource.Resource_Folder;

            // Get the executable path/file for ghostscript and imagemagick
            string ghostscript_executable = MultiInstance_Builder_Settings.Ghostscript_Executable;
            string imagemagick_executable = MultiInstance_Builder_Settings.ImageMagick_Executable;

            // Preprocess each PDF
            string[] pdfs = Directory.GetFiles(resourceFolder, "*.pdf");
            foreach (string thisPdf in pdfs)
            {
                // Get the fileinfo and the name
                FileInfo thisPdfInfo = new FileInfo(thisPdf);
                string fileName = thisPdfInfo.Name.Replace(thisPdfInfo.Extension, "");

                // Does the thumbnail exist for this item?
                if (( !String.IsNullOrEmpty(ghostscript_executable)) && (!String.IsNullOrEmpty(imagemagick_executable)))
                {
                    if (!File.Exists(resourceFolder + "\\" + fileName + "thm.jpg"))
                    {
                        PDF_Tools.Create_Thumbnail(resourceFolder, thisPdf, resourceFolder + "\\" + fileName + "thm.jpg", ghostscript_executable, imagemagick_executable);
                    }
                }
            }

            return true;
        }
    }
}
