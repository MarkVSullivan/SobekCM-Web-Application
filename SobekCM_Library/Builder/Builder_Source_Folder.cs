#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using SobekCM.Library.Settings;

#endregion

namespace SobekCM.Library.Builder
{
    /// <summary> Class which contains all the specifications for an source folder which may contain 
    /// packages destined to be bulk loaded into a SobekCM library incoming FTP folder, including 
    /// network locations, archiving instructions, and what type of packages are permissable </summary>
    public class Builder_Source_Folder
    {
        /// <summary> Constructor for a new instance of the Builder_Source_Folder class </summary>
        /// <param name="Source_Data_Row"> Data row from the builder settings procedure which contains all the specifications 
        /// for this incoming source folder, including network locations, archiving instructions, and what type of packages are permissable </param>
        public Builder_Source_Folder( DataRow Source_Data_Row )
        {
            Folder_Name = Source_Data_Row["FolderName"].ToString();
            Inbound_Folder = Source_Data_Row["NetworkFolder"].ToString();
            Failures_Folder = Source_Data_Row["ErrorFolder"].ToString();
            Processing_Folder = Source_Data_Row["ProcessingFolder"].ToString();
            Perform_Checksum = Convert.ToBoolean(Source_Data_Row["Perform_Checksum_Validation"]);
            Archive_TIFFs = Convert.ToBoolean(Source_Data_Row["Archive_TIFF"]);
            Archive_All_Files = Convert.ToBoolean(Source_Data_Row["Archive_All_Files"]);
            Allow_Deletes = Convert.ToBoolean(Source_Data_Row["Allow_Deletes"]);
            Allow_Folders_No_Metadata = Convert.ToBoolean(Source_Data_Row["Allow_Folders_No_Metadata"]);
            Allow_Metadata_Updates = Convert.ToBoolean(Source_Data_Row["Allow_Metadata_Updates"]);
			BibID_Roots_Restrictions = Source_Data_Row["BibID_Roots_Restrictions"].ToString();
	        if (Source_Data_Row["Can_Move_To_Content_Folder"] == DBNull.Value)
		        Can_Move_To_Content_Folder = null;
	        else
				Can_Move_To_Content_Folder = Convert.ToBoolean(Source_Data_Row["Can_Move_To_Content_Folder"]);
        }

        /// <summary> Constructor for a new instance of the Builder_Source_Folder class </summary>
        public Builder_Source_Folder()
        {
            Folder_Name = String.Empty;
            Inbound_Folder = String.Empty;
            Failures_Folder = String.Empty;
            Processing_Folder = String.Empty;
            Perform_Checksum = false;
            Archive_TIFFs = true;
            Archive_All_Files = true;
            Allow_Deletes = false;
            Allow_Folders_No_Metadata = false;
            Allow_Metadata_Updates = false;
	        Can_Move_To_Content_Folder = true;
	        BibID_Roots_Restrictions = String.Empty;
        }

        /// <summary> Human readable label for this folder </summary>
        public string Folder_Name { get; private set; }

        /// <summary> Network folder where packages which fail validation and other checks are placed </summary>
        public string Failures_Folder { get; private set; }

        /// <summary> Network inbound folder which is checked for new incoming packages or metadata files </summary>
        public string Inbound_Folder { get; private set; }

        /// <summary> Network processing folder where packages are moved while being loaded into the library </summary>
        public string Processing_Folder { get; private set; }

        /// <summary> Flag indicates if incoming packages should be subjected to a checksum validation if
        /// checksums exist within the METS file </summary>
        public bool Perform_Checksum { get; private set; }

        /// <summary> Flag indicates if any incoming, unarchived TIFF files should be copied to the archiving directory </summary>
        public bool Archive_TIFFs { get; private set; }

        /// <summary> Flag indicates if all incoming, unarchived files (regardless of type) should be copied to the archiving directory </summary>
        public bool Archive_All_Files { get; private set; }

        /// <summary> Flag indicates if this folder accepts DELETE metadata files, or if these should be rejected </summary>
        public bool Allow_Deletes { get; private set; }

        /// <summary> Flag indicates if this folder accepts folders with resource files but lacking metadata, or if these should be rejected </summary>
        public bool Allow_Folders_No_Metadata { get; private set; }

        /// <summary> Flag indicates if this folder accepts METADATA UPDATE files, or if these should be rejected </summary>
        public bool Allow_Metadata_Updates { get; private set; }

        /// <summary> If there are any BibID root restrictions related to this incoming folder, they are 
        /// listed here, with 'pipes' between them.  i.e., 'UF|UCF|CA001' </summary>
		public string BibID_Roots_Restrictions { get; private set; }

		/// <summary> Flag indicates if content can be moved, or must be copied, from this incoming
		/// folder to the content folder.  Essentially, set to TRUE if on the same system </summary>
		public Nullable<bool> Can_Move_To_Content_Folder { get; set; }

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
                    if ((Directory.GetDirectories(inboundFolder).Length > 0) || ( Directory.GetFiles(inboundFolder,"*.mets*").Length > 0 ))
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
        private IEnumerable<string> Get_Terminal_SubDirectories(string InitialDir)
        {
            List<string> returnVal = new List<string>();
            foreach (string thisDir in Directory.GetDirectories(InitialDir))
            {
                Collect_Terminal_Dirs(returnVal, thisDir);
            }
            return returnVal;
        }


        /// <summary>Private recursive method used to get the full path of each package's mets files</summary>
        /// <param name="dirCollection"></param>
        /// <param name="currentDir"></param>
        private void Collect_Terminal_Dirs(List<string> dirCollection, string currentDir)
        {
            if (Directory.GetDirectories(currentDir).Length == 0)
            {
                if (Directory.GetFiles(currentDir).Length > 0)
                    dirCollection.Add(currentDir);
                else
                {
                    try
                    {
                        Directory.Delete(currentDir);
                    }
                    catch
                    {

                    }
                }
            }
            else
            {
                foreach (String thisDir in Directory.GetDirectories(currentDir))
                    Collect_Terminal_Dirs(dirCollection, thisDir);
            }
        }

        #endregion

        /// <summary> Moves all packages from the inbound folder into the processing folder
        /// to queue them for loading into the library  </summary>
        /// <param name="Message"> Message to be passed out if something occurred during this attempted move </param>
        /// <returns> TRUE if successful, or FALSE if an error occurs </returns>
        public bool Move_From_Inbound_To_Processing( out string Message )
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
                long resource_age = resource.Age_in_Ticks;
                if ((resource_age > SobekCM_Library_Settings.Complete_Package_Required_Aging) || ((resource_age > SobekCM_Library_Settings.METS_Only_Package_Required_Aging) && (resource.METS_Only_Package)))
                {
                    if (!resource.Move(Processing_Folder))
                        returnVal = false;
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
