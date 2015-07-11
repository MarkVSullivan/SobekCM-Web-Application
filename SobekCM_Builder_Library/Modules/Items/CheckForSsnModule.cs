#region Using directives

using System;
using System.IO;
using SobekCM.Builder_Library.Tools;
using SobekCM.Engine_Library.Email;
using SobekCM.Library.Database;

#endregion

namespace SobekCM.Builder_Library.Modules.Items
{
    /// <summary> Item-level submission package module checks ... </summary>
    /// <remarks> This class implements the <see cref="abstractSubmissionPackageModule" /> abstract class and implements the <see cref="iSubmissionPackageModule" /> interface. </remarks>
    public class CheckForSsnModule : abstractSubmissionPackageModule
    {
        /// <summary>  </summary>
        /// <param name="Resource"> Incoming digital resource object </param>
        /// <returns> TRUE if processing can continue, FALSE if a critical error occurred which should stop all processing </returns>
        public override bool DoWork(Incoming_Digital_Resource Resource)
        {
            string resourceFolder = Resource.Resource_Folder;
            string bibID = Resource.BibID;
            string vid = Resource.VID;

            // Look for SSN in text
            string ssn_text_file_name = String.Empty;
            string ssn_match = String.Empty;
            try
            {
                // Get the list of all text files here
                string[] text_files = Directory.GetFiles(resourceFolder, "*.txt");
                if (text_files.Length > 0)
                {
                    // Step through each text file
                    foreach (string textFile in text_files)
                    {
                        // If no SSN possibly found, look for one
                        if (ssn_match.Length == 0)
                        {
                            ssn_match = Text_Cleaner.Has_SSN(textFile);
                            if (ssn_match.Length > 0)
                                ssn_text_file_name = (new FileInfo(textFile)).Name;
                        }
                    }
                }
            }
            catch
            {

            }

            // Send a database email if there appears to have been a SSN
            if (ssn_match.Length > 0)
            {
                if (Settings.Privacy_Email_Address.Length > 0)
                {
                    Email_Helper.SendEmail(Settings.Privacy_Email_Address, "Possible Social Security Number Located", "A string which appeared to be a possible social security number was found while bulk loading or post-processing an item.\n\nThe SSN was found in package " + bibID + ":" + vid + " in file '" + ssn_text_file_name + "'.\n\nThe text which may be a SSN is '" + ssn_match + "'.\n\nPlease review this item and remove any private information which should not be on the web server.", false, Settings.System_Name);
                }
                OnProcess("Possible SSN Located (" + ssn_text_file_name + ")", "Privacy Checking", Resource.BibID + ":" + Resource.VID, Resource.METS_Type_String, Resource.BuilderLogId);
            }

            return true;
        }
    }
}
