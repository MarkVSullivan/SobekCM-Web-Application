using System;
using System.Diagnostics;
using System.IO;
using SobekCM.Builder_Library.Settings;

namespace SobekCM.Builder_Library.Modules.Items
{
    /// <summary> Item-level submission package module looks for TIFF images without matching text files and
    /// uses Tesseract (if installed) to perform the OCR </summary>
    /// <remarks> This class implements the <see cref="abstractSubmissionPackageModule" /> abstract class 
    /// and implements the <see cref="iSubmissionPackageModule" /> interface. </remarks>
    public class TesseractOcrModule : abstractSubmissionPackageModule
    {
        /// <summary> Looks for TIFF images without matching text files and
        /// uses Tesseract (if installed) to perform the OCR </summary>
        /// <param name="Resource"> Incoming digital resource object </param>
        /// <returns> TRUE if processing can continue, FALSE if a critical error occurred which should stop all processing </returns>
        public override bool DoWork(Incoming_Digital_Resource Resource)
        {
            // Is Tesseract configured?
            if (String.IsNullOrEmpty(MultiInstance_Builder_Settings.Tesseract_Executable))
            {
                OnProcess("Tesseract OCR software not found", "Tesseract OCR Module", Resource.BibID + ":" + Resource.VID, Resource.METS_Type_String, Resource.BuilderLogId);
                return true;
            }

            // Ensure the executable exists
            string tesseract_executable = MultiInstance_Builder_Settings.Tesseract_Executable;
            try
            {
                if (!File.Exists(tesseract_executable))
                {
                    OnProcess("Tesseract OCR executable configured, but not present", "Tesseract OCR Module", Resource.BibID + ":" + Resource.VID, Resource.METS_Type_String, Resource.BuilderLogId);
                    return true;
                }
            }
            catch (Exception)
            {
                OnProcess("Exception thrown file checking for Tesseract OCR executable existance", "Tesseract OCR Module", Resource.BibID + ":" + Resource.VID, Resource.METS_Type_String, Resource.BuilderLogId);
                return true;
            }

            // Tesseract executable is configured and exists
            Tesseract_Processor.Tesseract_Executable = tesseract_executable;

            // Will only use the languag/type and directory information from the package
            string resourceFolder = Resource.Resource_Folder;
            string language = String.Empty;
            if ((Resource.Metadata.Bib_Info.Languages_Count > 0) && (!String.IsNullOrEmpty(Resource.Metadata.Bib_Info.Languages[0].Language_ISO_Code)))
            {
                language = Resource.Metadata.Bib_Info.Languages[0].Language_Text;
            }
            string type = Resource.Metadata.Bib_Info.SobekCM_Type_String;

            // Only certain TYPES should even be considered for OCR


            // Look through all the TIFFs
            string[] tiff_files = Directory.GetFiles(resourceFolder, "*.tif*");
            foreach (string thisTiffFile in tiff_files)
            {

                string textFileName = Path.GetFileNameWithoutExtension(thisTiffFile) + ".txt";
                string textFilePath = Path.Combine(resourceFolder, textFileName);

                // Should this TIFF be processed by Tesseract OCR?
                bool processTiff = false;
                if (!File.Exists(textFilePath))
                {
                    processTiff = true;
                }
                else
                {
                    DateTime textLastModifiedDate = (new FileInfo(textFilePath)).LastWriteTime;
                    DateTime tiffLastModifiedDate = (new FileInfo(thisTiffFile)).LastWriteTime;

                    if (textLastModifiedDate.CompareTo(tiffLastModifiedDate) < 0)
                    {
                        processTiff = true;
                    }
                }

                // Newer TIFF than text, so process
                if (processTiff)
                {
                    // Was this successful?
                    if (!Tesseract_Processor.Process_TIFF(thisTiffFile, textFilePath))
                    {
                        string exception_type = "Unknown Exception";
                        if (!String.IsNullOrEmpty(Tesseract_Processor.Last_Exception))
                            exception_type = Tesseract_Processor.Last_Exception;

                        OnProcess("Tesseract OCR exception on " + Path.GetFileName(thisTiffFile) + ": " + exception_type, "Tesseract OCR Module", Resource.BibID + ":" + Resource.VID, Resource.METS_Type_String, Resource.BuilderLogId);
                    }
                    else
                    {
                        OnProcess("Tesseract OCR successfy on " + Path.GetFileName(thisTiffFile), "Tesseract OCR Module", Resource.BibID + ":" + Resource.VID, Resource.METS_Type_String, Resource.BuilderLogId);
                    }
                }
            }

            return true;
        }
    }



    public static class Tesseract_Processor
    {
        public static string Tesseract_Executable { get; set; }

        public static string Last_Exception { get; private set; }

        public static bool Process_TIFF(string SourceFileName, string TextFileName)
        {
            Last_Exception = null;

            try
            {
                Process tessProcess = new Process();
                tessProcess.StartInfo.FileName = Tesseract_Executable;
                tessProcess.StartInfo.Arguments = SourceFileName + " " + TextFileName;

                // Stop the process from opening a new window
                //process.StartInfo.RedirectStandardOutput = true;
                //process.StartInfo.UseShellExecute = false;
                //process.StartInfo.CreateNoWindow = true;


                tessProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                tessProcess.Start();

                tessProcess.WaitForExit(5000);

                return true;
            }
            catch (Exception ee)
            {
                Last_Exception = ee.Message;
                return false;
            }

        }
    }
}
