using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SobekCM.Library.Builder;
using SobekCM.Library.Database;
using SobekCM.Library.Settings;

namespace SobekCM.Builder.Modules.Items
{
    public class CleanDirtyOcrModule : abstractSubmissionPackageModule
    {
        public override void DoWork(Incoming_Digital_Resource Resource)
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
            }
            catch
            {

            }
        }
    }
}
