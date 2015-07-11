#region Using directives

using System.IO;
using SobekCM.Builder_Library.Tools;

#endregion

namespace SobekCM.Builder_Library.Modules.Items
{
    /// <summary> Item-level submission package module checks ... </summary>
    /// <remarks> This class implements the <see cref="abstractSubmissionPackageModule" /> abstract class and implements the <see cref="iSubmissionPackageModule" /> interface. </remarks>
    public class ExtractTextFromXmlModule : abstractSubmissionPackageModule
    {
        /// <summary>  </summary>
        /// <param name="Resource"> Incoming digital resource object </param>
        /// <returns> TRUE if processing can continue, FALSE if a critical error occurred which should stop all processing </returns>
        public override bool DoWork(Incoming_Digital_Resource Resource)
        {
            string resourceFolder = Resource.Resource_Folder;

            // Preprocess each XML file for the text
            string[] xml_files = Directory.GetFiles(resourceFolder, "*.xml");
            foreach (string thisXml in xml_files)
            {
                // Get the fileinfo and the name
                FileInfo thisXmlInfo = new FileInfo(thisXml);

                // Just don't pull text for the static page
                string xml_upper = thisXmlInfo.Name.ToUpper();
                if ((xml_upper.IndexOf(".METS") < 0) && (xml_upper != "DOC.XML") && (xml_upper != "CITATION_METS.XML") && (xml_upper != "MARC.XML"))
                {
                    string text_fileName = thisXmlInfo.Name.Replace(".", "_") + ".txt";

                    // Does the full text exist for this item?
                    if (!File.Exists(resourceFolder + "\\" + text_fileName))
                    {
                        HTML_XML_Text_Extractor.Extract_Text(thisXml, resourceFolder + "\\" + text_fileName);
                    }
                }
            }

            return true;
        }
    }
}
