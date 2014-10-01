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
    public class CreatePdfThumbnailModule : abstractSubmissionPackageModule
    {
        public override void DoWork(Incoming_Digital_Resource Resource)
        {
            string resourceFolder = Resource.Resource_Folder;

            // Get the executable path/file for ghostscript and imagemagick
            string ghostscript_executable = SobekCM_Library_Settings.Ghostscript_Executable;
            string imagemagick_executable = SobekCM_Library_Settings.ImageMagick_Executable;

            // Preprocess each PDF
            string[] pdfs = Directory.GetFiles(resourceFolder, "*.pdf");
            foreach (string thisPdf in pdfs)
            {
                // Get the fileinfo and the name
                FileInfo thisPdfInfo = new FileInfo(thisPdf);
                string fileName = thisPdfInfo.Name.Replace(thisPdfInfo.Extension, "");

                // Does the thumbnail exist for this item?
                if ((ghostscript_executable.Length > 0) && (imagemagick_executable.Length > 0))
                {
                    if (!File.Exists(resourceFolder + "\\" + fileName + "thm.jpg"))
                    {
                        PDF_Tools.Create_Thumbnail(resourceFolder, thisPdf, resourceFolder + "\\" + fileName + "thm.jpg", ghostscript_executable, imagemagick_executable);
                    }
                }
            }
        }
    }
}
