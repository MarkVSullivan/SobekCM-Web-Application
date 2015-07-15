#region Using directives

using System.IO;
using SobekCM.Builder_Library.Tools;

#endregion

namespace SobekCM.Builder_Library.Modules.Items
{
    /// <summary> Item-level submission package module attempts to clean dirty OCR files that may somehow
    /// contain unprintable characters and other flaws </summary>
    /// <remarks> This class implements the <see cref="abstractSubmissionPackageModule" /> abstract class and implements the <see cref="iSubmissionPackageModule" /> interface. </remarks>
    public class CleanDirtyOcrModule : abstractSubmissionPackageModule
    {
        /// <summary> Attempts to clean dirty OCR files that may somehow
        /// contain unprintable characters and other flaws </summary>
        /// <param name="Resource"> Incoming digital resource object </param>
        /// <returns> TRUE if processing can continue, FALSE if a critical error occurred which should stop all processing </returns>
        public override bool DoWork(Incoming_Digital_Resource Resource)
        {
            string resourceFolder = Resource.Resource_Folder;

            // Clean any incoming text files first
            try
            {
                // Get the list of all text files here
                string[] text_files = Directory.GetFiles(resourceFolder, "*.txt");
                if (text_files.Length > 0)
                {
                    // Step through each text file
                    foreach (string textFile in text_files)
                    {
                        // Clean the text file first
                        Text_Cleaner.Clean_Text_File(textFile);
                    }
                }

                return true;
            }
            catch 
            {

            }

            return true;
        }
    }
}
