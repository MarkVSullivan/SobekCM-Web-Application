using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            throw new NotImplementedException();
        }
    }

    public static class Tesseract_Processor
    {
        public static string Tesseract_Executable { get; set; }

        public static string Last_Exception { get; private set; }

        public static bool Process_TIFF(string SourceFileName, string TextFileName)
        {
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
