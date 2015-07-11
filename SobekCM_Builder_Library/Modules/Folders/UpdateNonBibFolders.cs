using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;


namespace SobekCM.Builder_Library.Modules.Folders
{
    /// <summary> Folder-level builder module checks for folders that are not named in BibID or BibiD/VID format and do not have metadata.  For 
    /// these items, creates a BibID/VID folder with minimal metadata from the folder name. </summary>
    /// <remarks> This class implements the <see cref="abstractFolderModule" /> abstract class and implements the <see cref="iFolderModule" /> interface. </remarks>
    public class UpdateNonBibFolders : abstractFolderModule
    {
        /// <summary> Check for folders that are not named in BibID or BibiD/VID format and do not have metadata.  For 
        /// these items, creates a BibID/VID folder with minimal metadata from the folder name.  </summary>
        /// <param name="BuilderFolder"> Builder folder upon which to perform all work </param>
        /// <param name="IncomingPackages"> List of valid incoming packages, which may be modified by this process </param>
        /// <param name="Deletes"> List of valid deletes, which may be modifyed by this process </param>
        public override void DoWork(Actionable_Builder_Source_Folder BuilderFolder, List<Incoming_Digital_Resource> IncomingPackages, List<Incoming_Digital_Resource> Deletes)
        {
            string[] subdirs = Directory.GetDirectories(BuilderFolder.Inbound_Folder);
            foreach ( string thisSubDir in subdirs)
            {
                try
                {
                    string thisSubDirName = (new DirectoryInfo(thisSubDir)).Name;

                    // Must have some files to continue
                    if (Directory.GetFiles(thisSubDir).Length == 0)
                        continue;

                    // Need to check if this MAY be a valid BibID. 
                    // Need to make this a bit more specific in the future, as it will skip ANY folders
                    // that are ten digits long right now.
                    if ((thisSubDir.Length == 10) || ((thisSubDir.Length == 16) && (thisSubDirName[0] == '_')))
                        continue;

                    // Look for a METS file or any source of metadata in the folder
                    if ((Directory.GetFiles(thisSubDir, "*.mets").Length > 0) || (Directory.GetFiles(thisSubDir, "*.xml").Length > 0))
                        continue;

                    // Clean any additional periods in the filenames first
                    string[] allFiles = Directory.GetFiles(thisSubDir);
                    foreach (string thisFile in allFiles)
                    {
                        string fileName = Path.GetFileName(thisFile);
                        if (Regex.Matches(fileName, "\\.").Count > 1)
                        {
                            string newFileName = fileName;
                            while (Regex.Matches(newFileName, "\\.").Count > 1)
                            {
                                char[] charArr = newFileName.ToCharArray();
                                charArr[newFileName.IndexOf(".")] = '_'; // freely modify the array
                                newFileName = new string(charArr);
                            }

                            File.Move(thisFile, Path.Combine(thisSubDir, newFileName));
                        }
                    }

                    // Create the new object
                    Resource_Object.SobekCM_Item newItem = new Resource_Object.SobekCM_Item();
                    newItem.Bib_Info.SobekCM_Type = Resource_Object.Bib_Info.TypeOfResource_SobekCM_Enum.Archival;
                    newItem.Bib_Info.Main_Title.Title = thisSubDirName;
                    newItem.Bib_Info.Add_Identifier(thisSubDirName);
                    newItem.Bib_Info.Source.Code = Arguments[1];
                    newItem.Bib_Info.Source.Statement = Arguments[2];
                    newItem.BibID = Arguments[0];
                    newItem.VID = "00001";

                    // Save this item, for the necessary bibid
                    Resource_Object.Database.SobekCM_Database.Save_New_Digital_Resource(newItem, false, false, "Builder", "Created BibID folder from '" + thisSubDirName + "'", -1);

                    string newFolderName = newItem.BibID + "_" + newItem.VID;
                    string newFolder = Path.Combine(BuilderFolder.Inbound_Folder, newFolderName);
                    Directory.Move(thisSubDir, newFolder);

                    newItem.Source_Directory = newFolder;
                    newItem.Save_METS();
                }
                catch (Exception ee)
                {
                    Console.WriteLine("Error moving directory " + ee.Message);
                }
            }
        }
    }
}
