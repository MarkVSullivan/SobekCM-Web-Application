#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#endregion

namespace SobekCM.Builder_Library.Modules.Folders
{
    public class ApplyBibIdRestrictionModule : abstractFolderModule
    {
        public override void DoWork(Actionable_Builder_Source_Folder BuilderFolder, List<Incoming_Digital_Resource> IncomingPackages, List<Incoming_Digital_Resource> Deletes)
        {
            // If this folder is limited on what BibID roots it accepts, check that now
            if (BuilderFolder.BibID_Roots_Restrictions.Length > 0)
            {
                if ((Directory.Exists(BuilderFolder.Processing_Folder)) && (Directory.GetDirectories(BuilderFolder.Processing_Folder).Length > 0))
                {
                    // Get the list of all packages in the processing folder
                    List<Incoming_Digital_Resource> packages = BuilderFolder.Packages_For_Processing;

                    // Step through each package
                    foreach (Incoming_Digital_Resource resource in packages)
                    {
                        string[] starts = BuilderFolder.BibID_Roots_Restrictions.Split("|,;".ToCharArray());
                        bool okay = starts.Any(ThisStart => resource.Folder_Name.IndexOf(ThisStart, StringComparison.OrdinalIgnoreCase) == 0);

                        // If not okay.. it is a failure
                        if (!okay)
                        {
                            OnError("Package " + resource.Folder_Name + " has invalid BibID for " + BuilderFolder.Folder_Name + " incoming folder ( " + BuilderFolder.Folder_Name + " )", resource.BibID + ":" + resource.VID, "INCOMING", -1);

                            // Move this resource
                            if (!resource.Move(BuilderFolder.Failures_Folder))
                            {
                                OnError("Unable to move folder " + resource.Folder_Name + " in " + BuilderFolder.Folder_Name + " to the failures folder", resource.BibID + ":" + resource.VID, resource.METS_Type_String, -1);
                            }
                        }
                    }
                }
            }
        }
    }
}
