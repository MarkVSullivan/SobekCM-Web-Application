#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Behaviors;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Resource_Object.Database;
using SobekCM.Resource_Object.Database.DataSets;
using SobekCM.Resource_Object.Divisions;

#endregion

namespace SobekCM_Resource_Database.Builder
{
    /// <summary> Creates a bibliographic package from a variety of locations </summary>
    /// <remarks> Object created by Mark V Sullivan (2006) for University of Florida's Digital Library Center.</remarks>
    public class Bib_Package_Builder
    {
        #region Delegates

        /// <summary> Delegate defines the method signature for any method which enriches the bibliographic package from a data source </summary>
        public delegate SobekCM_Item Load_From_Database_Delegate(string BibID, string VID);

        #endregion

        private static SobekCM_All_Items sobekItems;

        /// <summary> Static constructor for the Bib_Package_Builder class </summary>
        static Bib_Package_Builder()
        {
            // Set any defaults
            Error = String.Empty;
        }

        /// <summary> Gets or sets the Error string for each build </summary>
        public static string Error { get; set; }

        /// <summary> Load the list of SobekCM items in preparation for Bib package building </summary>
        /// <param name="SobekCM_Base_URL">Base URL for the SobekCM instance </param>
        /// <param name="Drive_Location"> Location on the local or network drive for the list of items to be checked and stored </param>
        public static SobekCM_All_Items Load_SobekCM_Item_List(string SobekCM_Base_URL, string Drive_Location)
        {
            // Pull the sobekcm list, if there is one from today
            if (File.Exists(Drive_Location))
            {
                FileInfo thisFileInfo = new FileInfo(Drive_Location);
                DateTime fileCreation = thisFileInfo.LastWriteTime;
                if ((fileCreation.Year == DateTime.Now.Year) && (fileCreation.Month == DateTime.Now.Month) && (fileCreation.Day == DateTime.Now.Day))
                {
                    try
                    {
                        sobekItems = new SobekCM_All_Items();
                        sobekItems.ReadXml(Drive_Location);
                    }
                    catch
                    {
                        sobekItems = null;
                    }
                }
            }

            if (sobekItems == null)
            {
                sobekItems = SobekCM_Item_Database.Current_SobekCM_Items(SobekCM_Base_URL);
                if (sobekItems != null)
                {
                    try
                    {
                        sobekItems.WriteXml(Drive_Location, XmlWriteMode.WriteSchema);
                    }
                    catch
                    {
                    }
                }
                else
                {
                    if (File.Exists(Drive_Location))
                    {
                        try
                        {
                            sobekItems = new SobekCM_All_Items();
                            sobekItems.ReadXml(Drive_Location);
                        }
                        catch
                        {
                            sobekItems = null;
                        }
                    }
                }
            }

            return sobekItems;
        }

        #region Method to build the bibliographic package, without divisions

        /// <summary> Builds a bib package using the database of choice for any additionally needed information </summary>
        /// <param name="BIBID"> Bib ID</param>
        /// <param name="Volumeid"> VID</param>
        /// <param name="DestinationDirectory">Directory in which to save the METS file and look for data</param>
        /// <param name="METSDirectory">Directory where pre-assembled METS files are stored</param>
        /// <param name="MARCDirectory">Directory where MARC XML files are stored</param>
        /// <param name="ProjectDirectory">Directory where PROJECT-LEVEL METS files are stored</param>
        /// <param name="XMLDirectory">Directory where necessary XML helper files are stored</param>
        /// <param name="AppName">Name of this application</param>
        /// <param name="UserName">Name of this user</param>
        /// <param name="DatabaseLoadMethod">Method to call to enrich this package from the database</param>
        /// <returns>Completely built bibliographic package, or NULL</returns>
        public static SobekCM_Item Get_Bibliographic_Data(string BIBID, string Volumeid, string DestinationDirectory, string METSDirectory, string MARCDirectory, string ProjectDirectory, string XMLDirectory, string AppName, string UserName, Load_From_Database_Delegate DatabaseLoadMethod)
        {
            // Now, create the Bibliographic package from the tracking and marc xml
            SobekCM_Item bibPackage = null;

            // See if there is a preexisting SobekCM METS file in the desitation_folder
            bool valid_mets_loaded = false;
            if ((File.Exists(DestinationDirectory + "/" + BIBID + "_" + Volumeid.Replace("VID", "") + ".mets")) ||
                (File.Exists(DestinationDirectory + "/" + BIBID + "_" + Volumeid.Replace("VID", "") + ".METS_Header.xml")))
            {
                try
                {
                    // Get the correct name for this
                    string name = DestinationDirectory + "/" + BIBID + "_" + Volumeid.Replace("VID", "") + ".mets";
                    if (!File.Exists(name))
                    {
                        name = DestinationDirectory + "/" + BIBID + "_" + Volumeid.Replace("VID", "") + ".METS_Header.xml";
                    }

                    // Try to read the METS file
                    try
                    {
                        bibPackage = SobekCM_Item.Read_METS(name);
                        valid_mets_loaded = true;
                    }
                    catch
                    {
                        File.Move(name, name + ".bak");
                    }
                }
                catch (Exception ee)
                {
                    Error = ee.ToString();
                }
            }

            // Also look for the old METS files which had 'VID' in the name
            if ((!valid_mets_loaded) && ((File.Exists(DestinationDirectory + "/" + BIBID + "_VID" + Volumeid.Replace("VID", "") + ".mets")) ||
                                         (File.Exists(DestinationDirectory + "/" + BIBID + "_VID" + Volumeid.Replace("VID", "") + ".METS_Header.xml"))))
            {
                try
                {
                    // Get the correct name for this
                    string name = DestinationDirectory + "/" + BIBID + "_VID" + Volumeid.Replace("VID", "") + ".mets";
                    if (!File.Exists(name))
                    {
                        name = DestinationDirectory + "/" + BIBID + "_VID" + Volumeid.Replace("VID", "") + ".METS_Header.xml";
                    }

                    // Try to read the METS file
                    try
                    {
                        bibPackage = SobekCM_Item.Read_METS(name);
                        valid_mets_loaded = true;

                        // Update this METS now
                        bibPackage.Divisions.Suppress_Checksum = true;
                        bibPackage.Save_METS();

                        // Delete the old one
                        File.Delete(name);
                    }
                    catch
                    {
                        File.Move(name, name + ".bak");
                    }
                }
                catch (Exception ee)
                {
                    Error = ee.ToString();
                }
            }

            // Is there a valid METS loaded on the SobekCM instance?
            if ((!valid_mets_loaded) && (sobekItems != null))
            {
                // Does this METS currently exist on the SobekCM instance?
                string package_resource_link = sobekItems.Package_Resource_Link(BIBID, Volumeid.Replace("VID", ""));
                if (package_resource_link.Length > 0)
                {
                    string mets = SobekCM_Item_Database.Download_METS(package_resource_link, BIBID, Volumeid.Replace("VID", ""));
                    if (mets.Length > 0)
                    {
                        // Get the new METS file name
                        string mets_file = DestinationDirectory + "/" + BIBID + "_" + Volumeid.Replace("VID", "") + ".mets";

                        // Save this mets information to the METS file
                        StreamWriter writer = new StreamWriter(mets_file, false);
                        writer.Write(mets);
                        writer.Flush();
                        writer.Close();

                        // Try to read this METS file back into the package
                        try
                        {
                            bibPackage = SobekCM_Item.Read_METS(mets_file);
                            valid_mets_loaded = true;
                        }
                        catch
                        {
                            File.Delete(mets_file);
                        }
                    }
                }
            }

            // Was there a METS for this on the network location?
            if ((!valid_mets_loaded) && (METSDirectory.Length > 0) && (Directory.Exists(METSDirectory)))
            {
                // Compute the folde by bib id
                string bib_mets_folder = METSDirectory;
                foreach (char thisChar in BIBID)
                {
                    bib_mets_folder = bib_mets_folder + thisChar + "\\";
                }
                bib_mets_folder = bib_mets_folder + Volumeid.Replace("VID", "") + "\\";

                // Check to see if a METS file exists there
                if ((File.Exists(bib_mets_folder + BIBID + "_" + Volumeid.Replace("VID", "") + ".mets")) ||
                    (File.Exists(bib_mets_folder + BIBID + "_" + Volumeid.Replace("VID", "") + ".METS_Header.xml")) ||
                    (File.Exists(METSDirectory + "\\" + BIBID + "_" + Volumeid.Replace("VID", "") + ".mets")) ||
                    (File.Exists(METSDirectory + "\\" + BIBID + "_" + Volumeid.Replace("VID", "") + ".METS_Header.xml")))
                {
                    try
                    {
                        // Get the correct name for this
                        string network_name = METSDirectory + "\\" + BIBID + "_" + Volumeid.Replace("VID", "") + ".mets";
                        if (!File.Exists(network_name))
                        {
                            network_name = METSDirectory + "\\" + BIBID + "_" + Volumeid.Replace("VID", "") + ".METS_Header.xml";
                        }
                        if (!File.Exists(network_name))
                        {
                            network_name = bib_mets_folder + BIBID + "_" + Volumeid.Replace("VID", "") + ".mets";
                        }
                        if (!File.Exists(network_name))
                        {
                            network_name = bib_mets_folder + BIBID + "_" + Volumeid.Replace("VID", "") + ".METS_Header.xml";
                        }


                        try
                        {
                            bibPackage = SobekCM_Item.Read_METS(network_name);
                            bibPackage.Source_Directory = DestinationDirectory;
                            valid_mets_loaded = true;

                            // Since this was loaded from the network, no need to add project 
                            // information
                            return bibPackage;
                        }
                        catch (Exception ee)
                        {
                            Error = ee.ToString();
                        }
                    }
                    catch (Exception ee)
                    {
                        Error = ee.ToString();
                    }
                }
            }


            // If no valid METS was loaded, do the rest of this
            if (!valid_mets_loaded)
            {
                // Try to enrich from MARC records
                // Load from any database
                if (DatabaseLoadMethod != null)
                    bibPackage = DatabaseLoadMethod(BIBID, Volumeid);

                // Create the new bib package
                if (bibPackage == null)
                {
                    bibPackage = new SobekCM_Item {BibID = BIBID, VID = Volumeid};
                }
                bibPackage.METS_Header.Creator_Software = AppName;
                bibPackage.METS_Header.ObjectID = bibPackage.BibID + "_" + bibPackage.VID;
                bibPackage.METS_Header.Creator_Organization = "UF";
                bibPackage.METS_Header.RecordStatus_Enum = METS_Record_Status.COMPLETE;
                bibPackage.Source_Directory = DestinationDirectory;

                bool enriched_from_marc = Add_MARC_Info(bibPackage, MARCDirectory, XMLDirectory);

                // If not enriched from MARC, load project-level information now
                if (!enriched_from_marc)
                {
                    // Try to load default XML data from the project
                    Add_Project_Info(bibPackage, ProjectDirectory);
                }
            }

            // Ensure the BIB ID is still the same (sometimes loaded from MARC records)
            bibPackage.BibID = BIBID;

            // Set the serial hierarchy, if possible
            bibPackage.Create_Serial_Hierarchy();

            // If this is image class, add each name to the list of pages without text
            if (bibPackage.Bib_Info.ImageClass)
            {
                if (!File.Exists(DestinationDirectory + "/processing.instr"))
                {
                    // Save the pages which should NOT be processed
                    try
                    {
                        StreamWriter processingInst = new StreamWriter(DestinationDirectory + "\\processing.instr", false);
                        string[] tiffFiles = Directory.GetFiles(DestinationDirectory, "*.tif");
                        foreach (string tiff in tiffFiles)
                        {
                            processingInst.WriteLine("-" + (new FileInfo(tiff)).Name.Replace(".TIF", "").Replace(".tif", ""));
                        }
                        processingInst.Flush();
                        processingInst.Close();
                    }
                    catch
                    {
                    }
                }
            }

            // Return the bib package
            return bibPackage;
        }

        #endregion

        #region Enrich from the MARC data

        /// <summary> Enrich a SobekCM_Item objet with information  </summary>
        /// <param name="BIBPackage"></param>
        /// <param name="MARCDirectory"></param>
        /// <param name="XMLDirectory"></param>
        /// <returns></returns>
        public static bool Add_MARC_Info(SobekCM_Item BIBPackage, string MARCDirectory, string XMLDirectory)
        {
            // Only look for MARC XML, if there is a storage spot listed
            if ((MARCDirectory.Length > 0) && (Directory.Exists(MARCDirectory)))
            {
                // See if there was a record listed for this
                List<string> alephRecords = new List<string>();
                string aleph = BIBPackage.Bib_Info.ALEPH_Record;
                if (aleph.Length > 0)
                {
                    alephRecords.Add(aleph);
                }
                string[] notis = BIBPackage.Bib_Info.NOTIS_Records;
                List<string> notisRecords = new List<string>();
                if (notis.Length > 0)
                {
                    foreach (string thisNotis in notis)
                        notisRecords.Add(thisNotis);
                }

                // Try to find MARC XML for any aleph records
                string aleph_record_number = String.Empty;
                DateTime aleph_record_date = new DateTime(1000, 1, 1);
                string oclc_record_number = String.Empty;
                string marc_folder;
                foreach (string thisRecord in alephRecords)
                {
                    // Compute the folder name
                    marc_folder = MARCDirectory;
                    foreach (char thisChar in thisRecord)
                    {
                        marc_folder = marc_folder + thisChar + "\\";
                    }

                    // Does this folder exist?
                    if ((Directory.Exists(marc_folder)) && (File.Exists(marc_folder + thisRecord + ".xml")))
                    {
                        DateTime thisDate = ((new FileInfo(marc_folder + thisRecord + ".xml")).LastWriteTime);
                        if ((aleph_record_number.Length == 0) || (thisDate.CompareTo(aleph_record_date) < 0))
                        {
                            aleph_record_number = thisRecord;
                            aleph_record_date = thisDate;
                        }
                    }
                }

                // Check for OCLC MARC xml file as well
                string oclc = BIBPackage.Bib_Info.OCLC_Record;
                if (oclc.Length > 0)
                {
                    marc_folder = MARCDirectory + "OCLC\\";
                    foreach (char thisChar in oclc)
                    {
                        marc_folder = marc_folder + thisChar + "\\";
                    }

                    // Does this folder exist?
                    if ((Directory.Exists(marc_folder)) && (File.Exists(marc_folder + oclc + ".xml")))
                    {
                        DateTime thisDate = ((new FileInfo(marc_folder + oclc + ".xml")).LastWriteTime);
                        if ((aleph_record_number.Length == 0) || (thisDate.CompareTo(aleph_record_date) < 0))
                        {
                            oclc_record_number = oclc;
                        }
                    }
                }

                // OCLC is newer
                if (oclc_record_number.Length > 0)
                {
                    // Try to pull OCLC first
                    if (oclc.Length > 0)
                    {
                        if (read_marc_file(BIBPackage, oclc, MARCDirectory + "OCLC\\"))
                        {
                            BIBPackage.Bib_Info.Record.Record_Origin = "Imported from (OCLC)" + oclc;
                            return true;
                        }
                    }

                    // Try to pull ALEPH next
                    if (aleph_record_number.Length > 0)
                    {
                        if (read_marc_file(BIBPackage, aleph_record_number, MARCDirectory))
                        {
                            BIBPackage.Bib_Info.Record.Record_Origin = "Imported from (ALEPH)" + aleph_record_number;
                            return true;
                        }
                    }
                }
                else // ALEPH is newer
                {
                    if (aleph_record_number.Length > 0)
                    {
                        // Try to pull ALEPH first
                        if (read_marc_file(BIBPackage, aleph_record_number, MARCDirectory))
                        {
                            BIBPackage.Bib_Info.Record.Record_Origin = "Imported from (ALEPH)" + aleph_record_number;
                            return true;
                        }
                    }

                    // Try to pull OCLC next
                    if (oclc.Length > 0)
                    {
                        if (read_marc_file(BIBPackage, oclc, MARCDirectory + "OCLC\\"))
                        {
                            BIBPackage.Bib_Info.Record.Record_Origin = "Imported from (OCLC)" + oclc;
                            return true;
                        }
                    }
                }

                // Try to find MARC XML for any notis records
                foreach (string thisRecord in notisRecords)
                {
                    if (read_marc_file(BIBPackage, thisRecord, MARCDirectory))
                    {
                        BIBPackage.Bib_Info.Record.Record_Origin = "Imported from (NOTIS)" + thisRecord;
                        return true;
                    }
                }
            }
            return false;
        }


        private static bool read_marc_file(SobekCM_Item BIBPackage, string ThisRecord, string MARCDirectory)
        {
            // Compute the folder name
            string folder = MARCDirectory;
            foreach (char thisChar in ThisRecord)
            {
                folder = folder + thisChar + "\\";
            }

            // Does this folder exist?
            if ((Directory.Exists(folder)) && (File.Exists(folder + ThisRecord + ".xml")))
            {
                try
                {
                    // The date could be rewritten by the load from MARC, so save it
                    string date = BIBPackage.Bib_Info.Origin_Info.Date_Issued;

                    // Read this in then
                    BIBPackage.Read_From_MARC_XML(folder + ThisRecord + ".xml");

                    // Save the date back, if there was one
                    if (date.Length > 0)
                        BIBPackage.Bib_Info.Origin_Info.Date_Issued = date;

                    // Set the flag and break out of this loop
                    return true;
                }
                catch (Exception ee)
                {
                    Error = "Error while reading '" + folder + ThisRecord + ".xml'. This is probably because the XML file indicated is not valid XML. To correct, go to the file and edit it to make it valid.";
                    throw new ApplicationException(Error, ee);
                }
            }
            return false;
        }

        #endregion

        #region Methods to enrich with project specific information

        /// <summary> Looks for project-level METS files and enriches the provided digital resource </summary>
        /// <param name="BIBPackage">Digital resource to enrich</param>
        /// <param name="ProjectDirectory">Directory where project-level METS can be found</param>
        public static void Add_Project_Info(SobekCM_Item BIBPackage, string ProjectDirectory)
        {
            // Build a collection of project information
            ArrayList pmets_projects = new ArrayList();
            ArrayList unenriched_collections = new ArrayList();

            // Check for aggregations' project files
            foreach (Aggregation_Info aggregation in BIBPackage.Behaviors.Aggregations)
            {
                string project = aggregation.Code;
                if (File.Exists(ProjectDirectory + "/" + project + ".pmets"))
                {
                    pmets_projects.Add(SobekCM_Item.Read_METS(ProjectDirectory + "/" + project + ".pmets"));
                }
                else
                {
                    unenriched_collections.Add(project);
                }
            }

            // If no data was found at all, just return
            if (pmets_projects.Count == 0)
                return;

            // Clear the existing information
            BIBPackage.Behaviors.Clear_Aggregations();

            // Loop through each project
            bool firstProject = true;
            foreach (SobekCM_Item projectMETS in pmets_projects)
            {
                if (projectMETS != null)
                {
                    if (firstProject)
                    {
                        // Move this data from the project list to the METS, where there is currently no data
                        if (projectMETS.Bib_Info.Notes_Count > 0)
                        {
                            foreach (Note_Info thisNote in projectMETS.Bib_Info.Notes)
                            {
                                BIBPackage.Bib_Info.Add_Note(thisNote.Clone());
                            }
                        }

                        // Only do this if the source institution matches
                        if ((projectMETS.Bib_Info.Source.Code.Length > 0) && ((BIBPackage.Bib_Info.Source.Code.Length == 0)) || (BIBPackage.Bib_Info.Source.Code.ToUpper() == projectMETS.Bib_Info.Source.Code.ToUpper()))
                        {
                            // Copy over the holding information as well
                            if ((projectMETS.Bib_Info.Location.Holding_Code.Length > 0) && (projectMETS.Bib_Info.Location.Holding_Name.Length > 0))
                            {
                                BIBPackage.Bib_Info.Location.Holding_Code = projectMETS.Bib_Info.Location.Holding_Code;
                                BIBPackage.Bib_Info.Location.Holding_Name = projectMETS.Bib_Info.Location.Holding_Name;
                            }

                            // Copy over the source information
                            if ((projectMETS.Bib_Info.Source.Code.Length > 0) && (projectMETS.Bib_Info.Source.Statement.Length > 0))
                            {
                                BIBPackage.Bib_Info.Source.Code = projectMETS.Bib_Info.Source.Code;
                                BIBPackage.Bib_Info.Source.Statement = projectMETS.Bib_Info.Source.Statement;
                            }
                        }

                        firstProject = false;
                    }

                    // Copy the rights over
                    if ((projectMETS.Bib_Info.Access_Condition.Text.Length > 0) && (BIBPackage.Bib_Info.Access_Condition.Text.Length == 0))
                    {
                        BIBPackage.Bib_Info.Access_Condition.Text = projectMETS.Bib_Info.Access_Condition.Text;
                    }

                    // Copy all the icon information over
                    if (projectMETS.Behaviors.Wordmark_Count > 0)
                    {
                        foreach (Wordmark_Info thisIcon in projectMETS.Behaviors.Wordmarks)
                        {
                            BIBPackage.Behaviors.Add_Wordmark(thisIcon.Code);
                        }
                    }

                    // Add the (primary) collection information
                    if (projectMETS.Behaviors.Aggregation_Count > 0)
                    {
                        foreach (Aggregation_Info aggregation in projectMETS.Behaviors.Aggregations)
                        {
                            string altCollection = aggregation.Code;
                            BIBPackage.Behaviors.Add_Aggregation(altCollection);
                        }
                    }

                    // Add the views
                    if ((projectMETS.Behaviors.Views != null) && (projectMETS.Behaviors.Views.Count > 0))
                    {
                        foreach (View_Object thisView in projectMETS.Behaviors.Views)
                        {
                            BIBPackage.Behaviors.Add_View(thisView);
                        }
                    }

                    // Add the interfaces as well
                    if (projectMETS.Behaviors.Web_Skin_Count > 0)
                    {
                        foreach (string thisInterface in projectMETS.Behaviors.Web_Skins)
                        {
                            if (!BIBPackage.Behaviors.Web_Skins.Contains(thisInterface))
                            {
                                BIBPackage.Behaviors.Add_Web_Skin(thisInterface);
                            }
                        }
                    }

                    // Add the Temporal locations, if they don't exist
                    if (projectMETS.Bib_Info.TemporalSubjects_Count > 0)
                    {
                        foreach (Temporal_Info myTemporal in projectMETS.Bib_Info.TemporalSubjects)
                        {
                            try
                            {
                                BIBPackage.Bib_Info.Add_Temporal_Subject(Convert.ToInt32(myTemporal.Start_Year), Convert.ToInt32(myTemporal.End_Year), myTemporal.TimePeriod);
                            }
                            catch
                            {
                            }
                        }
                    }

                    // Add the Subject keywords, if they don't exist
                    if (projectMETS.Bib_Info.Subjects_Count > 0)
                    {
                        foreach (Subject_Info thisSubject in projectMETS.Bib_Info.Subjects)
                        {
                            BIBPackage.Bib_Info.Add_Subject(thisSubject);
                        }
                    }

                    // Copy the abstract over, if there is one
                    if (projectMETS.Bib_Info.Abstracts_Count > 0)
                    {
                        foreach (Abstract_Info thisAbstract in projectMETS.Bib_Info.Abstracts)
                        {
                            if (!BIBPackage.Bib_Info.Contains_Abstract(thisAbstract))
                            {
                                BIBPackage.Bib_Info.Add_Abstract(thisAbstract.Abstract_Text, thisAbstract.Language);
                            }
                        }
                    }
                }
            }

            // Now, add any collection codes back in that did not have PMETS
            if (unenriched_collections.Count > 0)
            {
                foreach (string collection in unenriched_collections)
                {
                    BIBPackage.Behaviors.Add_Aggregation(collection);
                }
            }
        }

        #endregion

        #region Method to add all of the files and create necessary divisions

        /// <summary> Adds all of the file information to a digital resource package by analyzing the directory </summary>
        /// <param name="BIBPackage">Digital resource package to enrich</param>
        /// <param name="FilesFilter"> Files to be added as page image files ( such as "*.tif|*.jpg|*.jp2" )</param>
        /// <param name="RecursivelyIncludeSubfolders"> Flag indicates if all files in subfolders should also be added </param>
        /// <param name="PageImagesInSeperateFoldersCanBeSamePage"> If two images with the same root are found in subfolders, should </param>
        public static void Add_All_Files(SobekCM_Item BIBPackage, string FilesFilter, bool RecursivelyIncludeSubfolders, bool PageImagesInSeperateFoldersCanBeSamePage)
        {
            // Get the set of file filters within a list
            List<string> file_filters = new List<string>();
            if (FilesFilter.IndexOf("|") < 0)
            {
                file_filters.Add(FilesFilter.ToUpper());
            }
            else
            {
                string[] splitter = FilesFilter.Split("|".ToCharArray());
                foreach (string thisFilter in splitter)
                {
                    file_filters.Add(thisFilter.ToUpper());
                }
            }

            // Get the files from the current directory (or recursive directories)
            Builder_Page_File_Collection fileCollection = new Builder_Page_File_Collection();
            get_files_from_current_directory(fileCollection, file_filters, BIBPackage.Source_Directory, String.Empty, RecursivelyIncludeSubfolders);

            // Now, determine which files are already in the METS file.
            // Build a collection of file objects from the METS
            List<SobekCM_File_Info> metsFiles = new List<SobekCM_File_Info>();
            Builder_Page_File_Collection metsFileCollection = new Builder_Page_File_Collection();
            Dictionary<SobekCM_File_Info, Page_TreeNode> fileToPage = new Dictionary<SobekCM_File_Info, Page_TreeNode>();
            Dictionary<Page_TreeNode, Division_TreeNode> pageToDiv = new Dictionary<Page_TreeNode, Division_TreeNode>();

            foreach (abstract_TreeNode rootNode in BIBPackage.Divisions.Physical_Tree.Roots)
            {
                recursively_add_all_METS_files(rootNode, metsFiles, metsFileCollection, fileToPage, pageToDiv, file_filters);
            }

            // Determine which files to delete from the METS package
            List<SobekCM_File_Info> deletes = new List<SobekCM_File_Info>();
            foreach (SobekCM_File_Info thisFile in metsFiles)
            {
                if ((thisFile.METS_LocType == SobekCM_File_Info_Type_Enum.SYSTEM) && (!File.Exists(BIBPackage.Source_Directory + "//" + thisFile.System_Name)))
                {
                    deletes.Add(thisFile);
                }
            }

            // Delete the files, and related pages
            foreach (SobekCM_File_Info thisFile in deletes)
            {
                metsFiles.Remove(thisFile);

                Page_TreeNode thisPage = fileToPage[thisFile];
                if (thisPage != null)
                {
                    thisPage.Files.Remove(thisFile);
                    Division_TreeNode thisDiv = pageToDiv[thisPage];
                    if (thisDiv != null)
                    {
                        thisDiv.Nodes.Remove(thisPage);
                    }
                }

                // Remove this from the other mets list
                int index = 0;
                int deleteIndex = -1;
                foreach (Builder_Page_File thisPageFile in metsFileCollection)
                {
                    if (thisPageFile.FullName.ToUpper() == thisFile.System_Name.ToUpper())
                    {
                        deleteIndex = index;
                        break;
                    }
                    
                    index++;
                }
                if (deleteIndex >= 0)
                {
                    metsFileCollection.RemoveAt(deleteIndex);
                }
            }

            // Now, recursively check each division and remove empty divisions
            int rootNodeCounter = 0;
            while (rootNodeCounter < BIBPackage.Divisions.Physical_Tree.Roots.Count)
            {
                abstract_TreeNode rootNode = BIBPackage.Divisions.Physical_Tree.Roots[rootNodeCounter];
                if (recursively_remove_empty_divisions(rootNode))
                    BIBPackage.Divisions.Physical_Tree.Roots.Remove(rootNode);
                else
                    rootNodeCounter++;
            }

            // Build the list of all the remaining files 
            Hashtable filesPresent = new Hashtable();
            foreach (SobekCM_File_Info thisFile in metsFiles)
            {
                filesPresent[thisFile.System_Name] = thisFile;
            }

            // Determine which files need to be added
            Builder_Page_File_Collection addFiles = new Builder_Page_File_Collection();
            foreach (Builder_Page_File thisFile in fileCollection)
            {
                if (!filesPresent.Contains(thisFile.FullName_With_Relative_Directory))
                {
                    addFiles.Add(thisFile);
                }
            }

            // Add files that need to be added
            if (addFiles.Count > 0)
            {
                // Make sure there is at least one division
                if (BIBPackage.Divisions.Physical_Tree.Roots.Count == 0)
                {
                    Division_TreeNode newRootNode = new Division_TreeNode("Main", String.Empty);
                    BIBPackage.Divisions.Physical_Tree.Roots.Add(newRootNode);
                }

                // Create the map of file names to pages
                Dictionary<string, Page_TreeNode> file_to_page_hash = new Dictionary<string, Page_TreeNode>();
                List<abstract_TreeNode> pageNodes = BIBPackage.Divisions.Physical_Tree.Pages_PreOrder;
                foreach (Page_TreeNode pageNode in pageNodes)
                {
                    if (pageNode.Files.Count > 0)
                    {
                        string first_page_name = pageNode.Files[0].File_Name_Sans_Extension;

                        if (first_page_name.IndexOf(".") > 0)
                            first_page_name = first_page_name.Substring(0, first_page_name.IndexOf("."));

                        if ((PageImagesInSeperateFoldersCanBeSamePage) || (pageNode.Files[0].METS_LocType == SobekCM_File_Info_Type_Enum.URL))
                        {
                            if (first_page_name.IndexOf("\\") > 0)
                            {
                                string[] slash_splitter = first_page_name.Split("\\".ToCharArray());
                                first_page_name = slash_splitter[slash_splitter.Length - 1];
                            }
                        }

                        if (!file_to_page_hash.ContainsKey(first_page_name.ToUpper()))
                        {
                            file_to_page_hash[first_page_name.ToUpper()] = pageNode;
                        }
                    }
                }

                // If there are no existing pages, this can be easily assembled
                if (metsFiles.Count == 0)
                {
                    try
                    {
                        // Get the first division
                        Division_TreeNode firstDiv = (Division_TreeNode) BIBPackage.Divisions.Physical_Tree.Roots[0];

                        // Add each file
                        foreach (Builder_Page_File thisFile in addFiles)
                        {
                            // Create the new METS file object
                            SobekCM_File_Info newFileForMETS = new SobekCM_File_Info(thisFile.FullName_With_Relative_Directory);

                            // Get the root of this file, to put all files of the same root on the same page
                            string thisFileShort = newFileForMETS.File_Name_Sans_Extension;
                            if (PageImagesInSeperateFoldersCanBeSamePage)
                            {
                                if (thisFileShort.IndexOf("\\") > 0)
                                {
                                    string[] slash_splitter = thisFileShort.Split("\\".ToCharArray());
                                    thisFileShort = slash_splitter[slash_splitter.Length - 1];
                                }
                            }

                            // Is this a pre-existing root ( therefore pre-existing page )?
                            if (file_to_page_hash.ContainsKey(thisFileShort))
                            {
                                // Just add this file to the pre-existing page
                                file_to_page_hash[thisFileShort].Files.Add(newFileForMETS);
                            }
                            else
                            {
                                // This needs a new page then
                                Page_TreeNode newPage = new Page_TreeNode();
                                newPage.Files.Add(newFileForMETS);
                                firstDiv.Nodes.Add(newPage);

                                // Add this page to the hash, so it is not added again later
                                file_to_page_hash[thisFileShort] = newPage;
                            }
                        }
                    }
                    catch
                    {
                    }
                }
                else
                {
                    // Configure the initial pointers
                    Builder_Page_File previous_file = null;
                    Builder_Page_File next_file = metsFileCollection[0];
                    Builder_Page_File new_file = addFiles[0];
                    int new_file_counter = 1;
                    int next_file_counter = 1;

                    // Loop through each file to be added
                    while (new_file != null)
                    {
                        // Create the new METS file object
                        SobekCM_File_Info newFileForMETS = new SobekCM_File_Info(new_file.FullName_With_Relative_Directory);

                        // Get the root of this file, to put all files of the same root on the same page
                        string thisFileShort = newFileForMETS.File_Name_Sans_Extension;
                        if (PageImagesInSeperateFoldersCanBeSamePage)
                        {
                            if (thisFileShort.IndexOf("\\") > 0)
                            {
                                string[] slash_splitter = thisFileShort.Split("\\".ToCharArray());
                                thisFileShort = slash_splitter[slash_splitter.Length - 1];
                            }
                        }

                        // First, ensure that we have not already added a page for this
                        if (file_to_page_hash.ContainsKey(thisFileShort))
                        {
                            // Just add this file to the pre-existing page
                            file_to_page_hash[thisFileShort].Files.Add(newFileForMETS);
                        }
                        else
                        {
                            // Move to the right part of the existing files list
                            while ((new_file.CompareTo(next_file) > 0) && (next_file != null))
                            {
                                previous_file = next_file;
                                if (next_file_counter < metsFileCollection.Count)
                                {
                                    next_file = metsFileCollection[next_file_counter++];
                                }
                                else
                                {
                                    next_file = null;
                                }
                            }

                            // Add the page for this and link the new file
                            Page_TreeNode newPage = new Page_TreeNode();
                            newPage.Files.Add(newFileForMETS);
                            file_to_page_hash[thisFileShort] = newPage;

                            // Get the parent division and add this page in the right place
                            // Check there was a previous page, otherwise this inserts at the very beginning
                            if (previous_file == null)
                            {
                                abstract_TreeNode abstractNode = BIBPackage.Divisions.Physical_Tree.Roots[0];
                                Division_TreeNode lastDivNode = (Division_TreeNode) abstractNode;
                                while (!abstractNode.Page)
                                {
                                    lastDivNode = (Division_TreeNode) abstractNode;
                                    if (lastDivNode.Nodes.Count > 0)
                                    {
                                        abstractNode = lastDivNode.Nodes[0];
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                lastDivNode.Nodes.Insert(0, newPage);
                                metsFileCollection.Insert(new_file);
                                new_file.METS_Division = lastDivNode;
                                new_file.METS_Page = newPage;
                                next_file = metsFileCollection[0];
                            }
                            else
                            {
                                Division_TreeNode parentDivNode = previous_file.METS_Division;
                                Page_TreeNode previousPageNode = previous_file.METS_Page;
                                int previousFileIndex = parentDivNode.Nodes.IndexOf(previousPageNode);
                                if (previousFileIndex + 1 >= parentDivNode.Nodes.Count)
                                    parentDivNode.Nodes.Add(newPage);
                                else
                                    parentDivNode.Nodes.Insert(previousFileIndex + 1, newPage);
                                next_file = previous_file;
                                next_file_counter--;
                                new_file.METS_Division = parentDivNode;
                                new_file.METS_Page = newPage;
                                metsFileCollection.Insert(new_file);
                            }
                        }

                        // Move to the next new file
                        if (new_file_counter < addFiles.Count)
                        {
                            new_file = addFiles[new_file_counter++];
                        }
                        else
                        {
                            new_file = null;
                        }
                    }
                }
            }
        }

        private static void get_files_from_current_directory(Builder_Page_File_Collection FileList, List<string> FileFilters, string SourceDirectory, string RelativeDirectory, bool RecursivelyIncludeSubfolders)
        {
            // Get the files in this directory by using file filters ( a single filter may find the same 
            // file in this directory twice, so special code is added for this case )
            List<string> files_in_this_dir = new List<string>();
            foreach (string thisFilter in FileFilters)
            {
                string[] thisFilterFiles = Directory.GetFiles(SourceDirectory, thisFilter);
                foreach (string thisFilterFile in thisFilterFiles)
                {
                    if (!files_in_this_dir.Contains(thisFilterFile))
                        files_in_this_dir.Add(thisFilterFile);
                }
            }

            // All found files should be added to the builder page file collection
            foreach (string thisFile in files_in_this_dir)
            {
                // Exclude the archival files
                if ((thisFile.ToUpper().IndexOf("_ARCHIVE.") < 0) && (thisFile.ToUpper().IndexOf(".QC.JPG") < 0))
                {
                    // Create the new page_file object
                    Builder_Page_File newFile = new Builder_Page_File(thisFile, RelativeDirectory, true);

                    // Add into the page file collection, sorting appropriately
                    FileList.Insert(newFile);
                }
            }

            // Check subdirectories
            if (RecursivelyIncludeSubfolders)
            {
                string[] subdirs = Directory.GetDirectories(SourceDirectory);
                foreach (string thisSubDir in subdirs)
                {
                    DirectoryInfo thisSubDirInfo = new DirectoryInfo(thisSubDir);
                    string dir_name = thisSubDirInfo.Name;
                    if (RelativeDirectory.Length == 0)
                        get_files_from_current_directory(FileList, FileFilters, thisSubDir, dir_name, true);
                    else
                        get_files_from_current_directory(FileList, FileFilters, thisSubDir, RelativeDirectory + "\\" + dir_name, true);
                }
            }
        }

        private static void recursively_add_all_METS_files(abstract_TreeNode RootNode,
                                                           List<SobekCM_File_Info> METSFiles, Builder_Page_File_Collection METSFileCollection,
                                                           Dictionary<SobekCM_File_Info, Page_TreeNode> FileToPage, Dictionary<Page_TreeNode, Division_TreeNode> PageToDiv,
                                                           List<string> FileFilters)
        {
            if (RootNode.Page)
            {
                Page_TreeNode pageNode = (Page_TreeNode) RootNode;
                foreach (SobekCM_File_Info thisFile in pageNode.Files)
                {
                    bool add_file = false;
                    if (FileFilters.Count == 0)
                    {
                        add_file = true;
                    }
                    else
                    {
                        foreach (string file_filter in FileFilters)
                        {
                            if (thisFile.System_Name.ToUpper().IndexOf(file_filter) > 0)
                            {
                                add_file = true;
                                break;
                            }
                        }
                    }

                    if (add_file)
                    {
                        METSFiles.Add(thisFile);
                        Builder_Page_File newPageFile = new Builder_Page_File(thisFile.System_Name, true);
                        newPageFile.METS_Page = pageNode;
                        newPageFile.METS_Division = PageToDiv[pageNode];
                        METSFileCollection.Insert(newPageFile);
                        FileToPage.Add(thisFile, pageNode);
                    }
                }
            }
            else
            {
                Division_TreeNode divNode = (Division_TreeNode) RootNode;
                foreach (abstract_TreeNode thisNode in divNode.Nodes)
                {
                    if (thisNode.Page)
                    {
                        try
                        {
                            if (!PageToDiv.ContainsKey((Page_TreeNode) thisNode))
                            {
                                PageToDiv.Add((Page_TreeNode) thisNode, divNode);
                            }
                        }
                        catch
                        {
                        }
                    }

                    recursively_add_all_METS_files(thisNode, METSFiles, METSFileCollection, FileToPage, PageToDiv, FileFilters);
                }
            }
        }


        private static bool recursively_remove_empty_divisions(abstract_TreeNode ChildNode)
        {
            // This steps through all the children nodes and checks for emptiness
            // If the node is empty and should be removed by the parent, TRUE is returned,
            // otherwise FALSE is returned, in which case the node has some children and
            // should not be removed
            if (!ChildNode.Page)
            {
                Division_TreeNode divNode = (Division_TreeNode) ChildNode;
                if (divNode.Nodes.Count > 0)
                {
                    int nodeCounter = 0;
                    while (nodeCounter < divNode.Nodes.Count)
                    {
                        abstract_TreeNode thisNode = divNode.Nodes[nodeCounter];
                        if (recursively_remove_empty_divisions(thisNode))
                        {
                            divNode.Nodes.Remove(thisNode);
                        }
                        else
                        {
                            nodeCounter++;
                        }
                    }
                    return divNode.Nodes.Count <= 0;
                }
                
                return true;
            }
            
            Page_TreeNode pageNode = (Page_TreeNode) ChildNode;
            return pageNode.Files.Count == 0;
        }

        #endregion
    }
}