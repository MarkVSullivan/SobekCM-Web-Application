#region Using directives

using System.IO;
using SobekCM.Builder_Library.Tools;

#endregion

namespace SobekCM.Builder_Library.Modules.Items
{
    /// <summary> Item-level submission package module extracts indexable (i.e, without the tags) text from a HTML file </summary>
    /// <remarks> This class implements the <see cref="abstractSubmissionPackageModule" /> abstract class and implements the <see cref="iSubmissionPackageModule" /> interface. </remarks>
    public class ExtractTextFromHtmlModule : abstractSubmissionPackageModule
    {
        /// <summary> Extracts indexable (i.e, without the tags) text from a HTML file </summary>
        /// <param name="Resource"> Incoming digital resource object </param>
        /// <returns> TRUE if processing can continue, FALSE if a critical error occurred which should stop all processing </returns>
        public override bool DoWork(Incoming_Digital_Resource Resource)
        {
            string resourceFolder = Resource.Resource_Folder;
            string bibID = Resource.BibID;
            string vid = Resource.VID;

            // Preprocess each HTML file for the text
            string[] html_files = Directory.GetFiles(resourceFolder, "*.htm*");
            foreach (string thisHtml in html_files)
            {
                // Get the fileinfo and the name
                FileInfo thisHtmlInfo = new FileInfo(thisHtml);

                // Exclude QC_Error.html
                if (thisHtmlInfo.Name.ToUpper() != "QC_ERROR.HTML")
                {
                    // Just don't pull text for the static page
                    if (thisHtmlInfo.Name.ToUpper() != bibID.ToUpper() + "_" + vid.ToUpper() + ".HTML")
                    {
                        string text_fileName = thisHtmlInfo.Name.Replace(".", "_") + ".txt";

                        // Does the full text exist for this item?
                        if (!File.Exists(resourceFolder + "\\" + text_fileName))
                        {
                            HTML_XML_Text_Extractor.Extract_Text(thisHtml, resourceFolder + "\\" + text_fileName);
                        }
                    }
                }
            }

            return true;
        }
    }
}
