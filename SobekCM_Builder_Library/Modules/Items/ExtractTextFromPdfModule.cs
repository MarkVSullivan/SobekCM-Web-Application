#region Using directives

using System.IO;
using SobekCM.Builder_Library.Tools;

#endregion

namespace SobekCM.Builder_Library.Modules.Items
{
    /// <summary> Item-level submission package module extracts indexable text from a PDF file </summary>
    /// <remarks> This class implements the <see cref="abstractSubmissionPackageModule" /> abstract class and implements the <see cref="iSubmissionPackageModule" /> interface. </remarks>
    public class ExtractTextFromPdfModule : abstractSubmissionPackageModule
    {
        /// <summary> Extracts indexable text from a PDF file </summary>
        /// <param name="Resource"> Incoming digital resource object </param>
        /// <returns> TRUE if processing can continue, FALSE if a critical error occurred which should stop all processing </returns>
        public override bool DoWork(Incoming_Digital_Resource Resource)
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

            return true;
        }
    }
}
