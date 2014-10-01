using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SobekCM.Library.Builder;
using SobekCM.Library.Settings;

namespace SobekCM.Builder.Modules.Items
{
    public class AttachAllNonImageFilesModule : abstractSubmissionPackageModule
    {
        public override void DoWork(Incoming_Digital_Resource Resource)
        {
            // Ensure all non-image files are linked to the METS file
            string[] all_files = Directory.GetFiles(Resource.Resource_Folder);
            foreach (string thisFile in all_files)
            {
                FileInfo thisFileInfo = new FileInfo(thisFile);

                if ((!Regex.Match(thisFileInfo.Name, SobekCM_Library_Settings.Files_To_Exclude_From_Downloads, RegexOptions.IgnoreCase).Success) && (String.Compare(thisFileInfo.Name, Resource.BibID + "_" + Resource.VID + ".html", StringComparison.OrdinalIgnoreCase) != 0))
                {
                    // Some last checks here
                    if ((thisFileInfo.Name.IndexOf("marc.xml", StringComparison.OrdinalIgnoreCase) != 0) && (thisFileInfo.Name.IndexOf("doc.xml", StringComparison.OrdinalIgnoreCase) != 0) && (thisFileInfo.Name.IndexOf(".mets", StringComparison.OrdinalIgnoreCase) < 0) && (thisFileInfo.Name.IndexOf("citation_mets.xml", StringComparison.OrdinalIgnoreCase) < 0) &&
                        (thisFileInfo.Name.IndexOf("ufdc_mets.xml", StringComparison.OrdinalIgnoreCase) < 0) && (thisFileInfo.Name.IndexOf("agreement.txt", StringComparison.OrdinalIgnoreCase) < 0) &&
                        ((thisFileInfo.Name.IndexOf(".xml", StringComparison.OrdinalIgnoreCase) < 0) || (thisFileInfo.Name.IndexOf(Resource.BibID, StringComparison.OrdinalIgnoreCase) < 0)))
                    {
                        Resource.Metadata.Divisions.Download_Tree.Add_File(thisFileInfo.Name);
                    }
                }
            }
        }
    }
}
