#region Using directives

using System.IO;

#endregion

namespace SobekCM.Builder_Library.Modules.Items
{
    public class ExtractTextFromHtmlModule : abstractSubmissionPackageModule
    {
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
