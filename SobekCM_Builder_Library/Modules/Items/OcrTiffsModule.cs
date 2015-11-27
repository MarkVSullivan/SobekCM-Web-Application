#region Using directives

using System;
using System.Diagnostics;
using System.IO;

#endregion

namespace SobekCM.Builder_Library.Modules.Items
{
    /// <summary> Item-level submission package module performs OCR on the incoming TIFF files to create indexable text </summary>
    /// <remarks> This class implements the <see cref="abstractSubmissionPackageModule" /> abstract class and implements the <see cref="iSubmissionPackageModule" /> interface. </remarks>
    public class OcrTiffsModule : abstractSubmissionPackageModule
    {
        /// <summary> Performs OCR on the incoming TIFF files to create indexable text </summary>
        /// <param name="Resource"> Incoming digital resource object </param>
        /// <returns> TRUE if processing can continue, FALSE if a critical error occurred which should stop all processing </returns>
        public override bool DoWork(Incoming_Digital_Resource Resource)
        {
            string resourceFolder = Resource.Resource_Folder;

            // Run OCR for any TIFF files that do not have any corresponding TXT files
            if (Settings.Builder.OCR_Command_Prompt.Length > 0)
            {
                string[] ocr_tiff_files = Directory.GetFiles(resourceFolder, "*.tif");
                foreach (string thisTiffFile in ocr_tiff_files)
                {
                    FileInfo thisTiffFileInfo = new FileInfo(thisTiffFile);
                    string text_file = resourceFolder + "\\" + thisTiffFileInfo.Name.Replace(thisTiffFileInfo.Extension, "") + ".txt";
                    if (!File.Exists(text_file))
                    {
                        try
                        {
                            string command = String.Format(Settings.Builder.OCR_Command_Prompt, thisTiffFile, text_file);
                            Process ocrProcess = new Process { StartInfo = { FileName = command } };
                            ocrProcess.Start();
                            ocrProcess.WaitForExit();
                        }
                        catch
                        {
                            OnError("Error launching OCR on (" + thisTiffFileInfo.Name + ")", Resource.BibID + ":" + Resource.VID, Resource.METS_Type_String, Resource.BuilderLogId);
                        }
                    }
                }
            }

            return true;
        }
    }
}
