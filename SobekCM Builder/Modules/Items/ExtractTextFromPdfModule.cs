using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SobekCM.Library.Builder;

namespace SobekCM.Builder.Modules.Items
{
    public class ExtractTextFromPdfModule : abstractSubmissionPackageModule
    {
        public override void DoWork(Incoming_Digital_Resource Resource)
        {
            string resourceFolder = Resource.Resource_Folder;

            // Preprocess each PDF
            string[] pdfs = Directory.GetFiles(resourceFolder, "*.pdf");
            foreach (string thisPdf in pdfs)
            {
                // Get the fileinfo and the name
                FileInfo thisPdfInfo = new FileInfo(thisPdf);
                string fileName = thisPdfInfo.Name.Replace(thisPdfInfo.Extension, "");

                // Does the full text exist for this item?
                if (!File.Exists(resourceFolder + "\\" + fileName + "_pdf.txt"))
                {
                    PDF_Tools.Extract_Text(thisPdf, resourceFolder + "\\" + fileName + "_pdf.txt");
                }
            }
        }
    }
}
