#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SobekCM.Builder_Library.Modules.Folders;
using SobekCM.Builder_Library.Settings;
using SobekCM.Core.Settings;
using SobekCM.Engine_Library.ApplicationState;

#endregion

namespace SobekCM.Builder_Library
{
    /// <summary> Builder source folder that adds the ability to perform some basic processing to the
    /// base <see cref="Builder_Source_Folder" /> class </summary>
    public class Actionable_Builder_Source_Folder : Builder_Source_Folder
    {
        /// <summary> Constructor for a new instance of the Actionable_Builder_Source_Folder class </summary>
        /// <remarks> This extends the core class <see cref="Builder_Source_Folder"/> and adds some methods to perform work </remarks>
        public Actionable_Builder_Source_Folder() 
        {
            // Do nothing
        }

        /// <summary> Constructor for a new instance of the Actionable_Builder_Source_Folder class </summary>
        /// <param name="ExistingBaseInstance"> An existing base instance used to populate this class with data </param>
        /// <param name="BuilderModulesConfig"> Builder module configuration to use to process this incoming folder structure </param>
        /// <remarks> This extends the core class <see cref="Builder_Source_Folder"/> and adds some methods to perform work </remarks>
        public Actionable_Builder_Source_Folder(Builder_Source_Folder ExistingBaseInstance, Builder_Modules BuilderModulesConfig )
        {
            Allow_Deletes = ExistingBaseInstance.Allow_Deletes;
            Allow_Folders_No_Metadata = ExistingBaseInstance.Allow_Folders_No_Metadata;
            Allow_Metadata_Updates = ExistingBaseInstance.Allow_Metadata_Updates;
            Archive_All_Files = ExistingBaseInstance.Archive_All_Files;
            Archive_TIFFs = ExistingBaseInstance.Archive_TIFFs;
            BibID_Roots_Restrictions = ExistingBaseInstance.BibID_Roots_Restrictions;
            Can_Move_To_Content_Folder = ExistingBaseInstance.Can_Move_To_Content_Folder;
            Failures_Folder = ExistingBaseInstance.Failures_Folder;
            Folder_Name = ExistingBaseInstance.Folder_Name;
            Inbound_Folder = ExistingBaseInstance.Inbound_Folder;
            Perform_Checksum = ExistingBaseInstance.Perform_Checksum;
            Processing_Folder = ExistingBaseInstance.Processing_Folder;

            BuilderModules = new List<iFolderModule>();

            // Copy over the folder modules
            foreach (Builder_Module_Setting settings in ExistingBaseInstance.Builder_Module_Settings)
            {
                iFolderModule module = BuilderModulesConfig.Get_Folder_Module_By_Key(settings.Key);
                if (module != null)
                    BuilderModules.Add(module);
            }
        }

        /// <summary> Collection of the builder modules to be run against this folder </summary>
        public List<iFolderModule> BuilderModules { get; private set;  }

        /// <summary> Gets flag indicating there are packages in the inbound folder </summary>
        public bool Items_Exist_In_Inbound
        {
            get
            {
                // Get the directories
                string inboundFolder = Inbound_Folder;

                // Make sure the inbound directory exists
                if (Directory.Exists(inboundFolder))
                {
                    if ((Directory.GetDirectories(inboundFolder).Length > 0) || (Directory.GetFiles(inboundFolder, "*.mets*").Length > 0))
                        return true;
                }

                return false;
            }
        }

        /// <summary> Gets the list of all packages for processing from the processing folder </summary>
        public List<Incoming_Digital_Resource> Packages_For_Processing
        {
            get
            {
                // Get the list of all terminal directories
                IEnumerable<string> terminalDirectories = Get_Terminal_SubDirectories(Processing_Folder);

                // Create a digital resource object for each directory
                return terminalDirectories.Select(ThisDirectory => new Incoming_Digital_Resource(ThisDirectory, this)).ToList();
            }
        }

        #region Methods used to get the terminal subdirectories

        /// <summary>Private method used to find the direct parent directory name for the item packages
        /// because the directory structure for the package is pretty flexible, it can be for instance UF00000001_VID00001 or 
        /// UF00000001/VID00001, or JUV/UFSpecial/UF00000001/VID00001 etc. 
        /// the searching criterial is that there are only files in the directory but not sub directories </summary>
        /// <param name="InitialDir"></param>
        /// <returns></returns>
        private static IEnumerable<string> Get_Terminal_SubDirectories(string InitialDir)
        {
            List<string> returnVal = new List<string>();
            foreach (string thisDir in Directory.GetDirectories(InitialDir))
            {
                Collect_Terminal_Dirs(returnVal, thisDir);
            }
            return returnVal;
        }


        /// <summary>Private recursive method used to get the full path of each package's mets files</summary>
        /// <param name="DirCollection"></param>
        /// <param name="CurrentDir"></param>
        private static void Collect_Terminal_Dirs(List<string> DirCollection, string CurrentDir)
        {
            if ((Directory.GetDirectories(CurrentDir).Length == 0) || (Directory.GetFiles(CurrentDir, "*.mets").Length > 0) || (Directory.GetFiles(CurrentDir, "*.mets.xml").Length > 0))
            {
                if (Directory.GetFiles(CurrentDir).Length > 0)
                    DirCollection.Add(CurrentDir);
                else
                {
                    try
                    {
                        Directory.Delete(CurrentDir);
                    }
                    catch
                    {
                        // Do not throw this error.. not necessary
                    }
                }
            }
            else
            {
                foreach (String thisDir in Directory.GetDirectories(CurrentDir))
                    Collect_Terminal_Dirs(DirCollection, thisDir);
            }
        }

        #endregion

        /// <summary> Moves all packages from the inbound folder into the processing folder
        /// to queue them for loading into the library  </summary>
        /// <param name="Message"> Message to be passed out if something occurred during this attempted move </param>
        /// <returns> TRUE if successful, or FALSE if an error occurs </returns>
        public bool Move_From_Inbound_To_Processing(out string Message)
        {
            Message = String.Empty;

            // Get the directories
            string inboundFolder = Inbound_Folder;

            // Make sure the inbound directory exists
            if (!Directory.Exists(inboundFolder))
            {
                try
                {
                    Directory.CreateDirectory(inboundFolder);
                }
                catch
                {
                    Message = "Unable to create the non-existent inbound folder " + inboundFolder;
                    return false;
                }
            }

            // Make sure the processing directory exists
            if (!Directory.Exists(Processing_Folder))
            {
                try
                {
                    Directory.CreateDirectory(Processing_Folder);
                }
                catch
                {
                    Message = "Unable to create the non-existent processing folder " + Processing_Folder;
                    return false;
                }
            }

            // Make sure the failures directory exists
            if (!Directory.Exists(Failures_Folder))
            {
                try
                {
                    Directory.CreateDirectory(Failures_Folder);
                }
                catch
                {
                    Message = "Unable to create the non-existent failures folder " + Failures_Folder;
                    return false;
                }
            }

            // If there are loose METS here, move them into flat folders of the same name
            try
            {
                string[] looseMets = Directory.GetFiles(inboundFolder, "*.mets*");
                foreach (string thisLooseMets in looseMets)
                {
                    string filename = (new FileInfo(thisLooseMets)).Name;
                    string[] filenameSplitter = filename.Split(".".ToCharArray());
                    if (!Directory.Exists(inboundFolder + "\\" + filenameSplitter[0]))
                        Directory.CreateDirectory(inboundFolder + "\\" + filenameSplitter[0]);
                    if (File.Exists(inboundFolder + "\\" + filenameSplitter[0] + "\\" + filename))
                        File.Delete(thisLooseMets);
                    else
                        File.Move(thisLooseMets, inboundFolder + "\\" + filenameSplitter[0] + "\\" + filename);
                }
            }
            catch (Exception ee)
            {
                Message = "Error moving the package from " + inboundFolder + " to " + Processing_Folder + ":" + ee.Message;
                return false;
            }


            // Get the list of all terminal directories
            IEnumerable<string> terminalDirectories = Get_Terminal_SubDirectories(inboundFolder);

            // Create a digital resource object for each directory
            List<Incoming_Digital_Resource> inboundResources = terminalDirectories.Select(ThisDirectory => new Incoming_Digital_Resource(ThisDirectory, this)).ToList();

            // Step through each resource which came in
            bool returnVal = true;
            foreach (Incoming_Digital_Resource resource in inboundResources)
            {
                // Is this resource a candidate to move for continued processing?
                long resource_age = resource.AgeInTicks;
                if ((resource_age > Engine_ApplicationCache_Gateway.Settings.Complete_Package_Required_Aging) || ((resource_age > Engine_ApplicationCache_Gateway.Settings.METS_Only_Package_Required_Aging) && (resource.METS_Only_Package)))
                {
                    if (!resource.Move(Processing_Folder))
                    {
                        returnVal = false;
                    }
                }
                else
                {
                    Message = "Resource ( " + resource.Resource_Folder + " ) needs to age more before it will be processed";
                }
            }

            return returnVal;
        }
    }
}
