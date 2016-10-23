﻿#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Resource_Object.Metadata_File_ReaderWriters;
using SobekCM.Resource_Object.Metadata_Modules;
using SobekCM.Resource_Object.Metadata_Modules.VRACore;

#endregion

namespace SobekCM.Builder_Library.Modules.Folders
{
    /// <summary> TEST Folder-level builder module for custom Wolfsonian processing </summary>
    /// <remarks> This class implements the <see cref="abstractFolderModule" /> abstract class and implements the <see cref="iFolderModule" /> interface. </remarks>
    public class WolfsonianLibraryProcessorModule : abstractFolderModule
    {
        private string source_folder, destination_folder;
        private int bibid;
        private bool isLibrary;

        private const string archived_files_link = "http://images.wolfsonian.org/resources/";

        /// <summary> Delegate for the custom event which is fired when the status
        /// string on the main form needs to change </summary>
        public delegate void MFP_New_Status_String_Delegate(string new_message);

        /// <summary> Delegate for the custom event which is fired when the progress
        /// bar should change. </summary>
        public delegate void MFP_New_Progress_Delegate(int Value, int Max);

        /// <summary> Delegate for the custom event which is fired when all the processing is complete </summary>
        public delegate void MFP_Process_Complete_Delegate( bool Aborted, int Packages_Processed, string Next_BibID );

        /// <summary> Custom event is fired when the volume string on the 
        /// main form needs to change. </summary>
        public event MFP_New_Status_String_Delegate New_Volume_String;

        /// <summary> Custom event is fired when the progress bar
        /// on the main form needs to change.  </summary>
        public event MFP_New_Progress_Delegate New_Progress;

        /// <summary> Custom event is fired when all processing is complete </summary>
        public event MFP_Process_Complete_Delegate Process_Complete;

        /// <summary> Constructor for a new instance of the WolfsonianProcessorModule class </summary>
        public WolfsonianLibraryProcessorModule()
        {
            // Do nothing
        }

        /// <summary> Constructor for a new instance of the WolfsonianProcessorModule class </summary>
        /// <param name="bibid"></param>
        /// <param name="source_folder"></param>
        /// <param name="destination_folder"></param>
        /// <param name="isLibrary"></param>
        /// <param name="archived_files_link"></param>
        public WolfsonianLibraryProcessorModule(int bibid, string source_folder, string destination_folder, bool isLibrary, string archived_files_link)
        {
            this.bibid = bibid;
            this.source_folder = source_folder;
            this.destination_folder = destination_folder;
            this.isLibrary = isLibrary;
        }

        //public void Process()
        //{

        //    // Is the source and destination directories the same?
        //    bool inplace = false;
        //    DirectoryInfo sourceInfo = new DirectoryInfo(source_folder.ToLower());
        //    DirectoryInfo destinationInfo = new DirectoryInfo(destination_folder.ToLower());
        //    if (String.Compare(sourceInfo.FullName, destinationInfo.FullName, StringComparison.InvariantCultureIgnoreCase) == 0)
        //    {
        //        inplace = true;
        //    }

        //    int processed = 0;
        //    int package_counter = 1;
        //    string[] subdirs = System.IO.Directory.GetDirectories(source_folder);
        //    foreach (string thisSubDir in subdirs)
        //    {
        //        string currentDirectory = thisSubDir;
        //        string foldername = (new DirectoryInfo(thisSubDir)).Name;
        //        OnNewProgress(package_counter++, subdirs.Length);
        //        OnNewVolume(foldername);

        //        try
        //        {
        //            string[] allXmlFiles = System.IO.Directory.GetFiles(thisSubDir, "*.xml");
        //            if (allXmlFiles.Length > 0)
        //            {
        //                // Create a new empty item
        //                SobekCM_Item newItem = new SobekCM_Item();

        //                // Read the MDOS information from the XML file
        //                string xmlFile = allXmlFiles[0];
        //                Stream reader = new FileStream(allXmlFiles[0], FileMode.Open, FileAccess.Read);
        //                MODS_File_ReaderWriter modsReader = new MODS_File_ReaderWriter();
        //                String error = String.Empty;
        //                modsReader.Read_Metadata(reader, newItem, null, out error);

        //                // Is the name of the XML file look like a WOLF bibid?
        //                string filename = new FileInfo(xmlFile).Name.ToUpper().Replace(".XML", "");
        //                int discard;
        //                string newBib;
        //                if (( filename.Length == 10 ) && ( filename.IndexOf("WOLF") == 0 ) && ( Int32.TryParse( filename.Substring(4), out discard )))
        //                    newBib = filename;
        //                else
        //                {
        //                    // Compute this bibid and increment
        //                    newBib = "WOLF" + bibid.ToString().PadLeft(6, '0');
        //                    bibid++;
        //                }

        //                // Check for existence and create the bibid folder
        //                string newDirectory = destination_folder + "\\" + newBib;
        //                if (inplace)
        //                {
        //                    // Create the vid folder
        //                    if (Directory.Exists(newDirectory))
        //                    {
        //                        ErrorMessageBox_Internal showError = new ErrorMessageBox_Internal("Directory already exists for bib id '" + newBib + "'.\n\nStop processing or continue and have the existing folder deleted?", "Existing folder detected");
        //                        DialogResult showErrorResult = showError.ShowDialog();
        //                        if (showErrorResult == DialogResult.Abort)
        //                        {
        //                            logWriter.Flush();
        //                            logWriter.Close();
        //                            OnProcessComplete(false, processed);
        //                        }

        //                        Directory.Delete(newDirectory);
        //                    }

        //                    Directory.CreateDirectory(newDirectory);


        //                     newDirectory = newDirectory + "\\00001";
                             
        //                    Directory.Move(thisSubDir, newDirectory);
        //                    currentDirectory = newDirectory;
        //                }
        //                else
        //                {
        //                    if (Directory.Exists(newDirectory))
        //                    {
        //                        ErrorMessageBox_Internal showError = new ErrorMessageBox_Internal("Directory already exists for bib id '" + newBib + "'.\n\nStop processing or continue and overwrite existing folder?", "Existing folder detected");
        //                        DialogResult showErrorResult = showError.ShowDialog();
        //                        if (showErrorResult == DialogResult.Abort)
        //                        {
        //                            logWriter.Flush();
        //                            logWriter.Close();
        //                            OnProcessComplete(false, processed);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        Directory.CreateDirectory(newDirectory);
        //                    }

        //                    // Create the vid folder
        //                    newDirectory = newDirectory + "\\00001";
        //                    if (!Directory.Exists(newDirectory))
        //                        Directory.CreateDirectory(newDirectory);
        //                }

        //                // Set some values
        //                newItem.BibID = newBib;
        //                newItem.VID = "00001";
        //                newItem.Bib_Info.Source.Code = "iWOLF";
        //                newItem.Bib_Info.Source.Statement = "The Wolfsonian-Florida International University";
        //                if (newItem.Bib_Info.Type.MODS_Type == TypeOfResource_MODS_Enum.UNKNOWN)
        //                    newItem.Bib_Info.Type.MODS_Type = TypeOfResource_MODS_Enum.Three_Dimensional_Object;
        //                if (newItem.Bib_Info.Main_Title.Title.Length == 0)
        //                    newItem.Bib_Info.Main_Title.Title = "NO TITLE";
        //                newItem.Source_Directory = newDirectory;
        //                newItem.Behaviors.Add_View(SobekCM.Resource_Object.Behaviors.View_Enum.RELATED_IMAGES);
        //                newItem.Behaviors.Add_View(SobekCM.Resource_Object.Behaviors.View_Enum.JPEG);

        //                // We are now removing all Genres
        //                newItem.Bib_Info.Clear_Genres();

        //                if (isLibrary)
        //                {
        //                    newItem.Bib_Info.Location.Holding_Code = "iWOLF";
        //                    if (newItem.Bib_Info.Location.Holding_Name.Length > 0)
        //                    {
        //                        newItem.Bib_Info.Location.Holding_Name = "The Wolfsonian FIU Library Collection ( " + newItem.Bib_Info.Location.Holding_Name + " )";
        //                    }
        //                    else
        //                    {
        //                        newItem.Bib_Info.Location.Holding_Name = "The Wolfsonian FIU Library Collection";
        //                    }
        //                    newItem.Behaviors.Add_Aggregation("LIBRARY");
        //                }
        //                else
        //                {
        //                    newItem.Bib_Info.Location.Holding_Code = "iWOLF";
        //                    if (newItem.Bib_Info.Location.Holding_Name.Length > 0)
        //                    {
        //                        newItem.Bib_Info.Location.Holding_Name = "The Wolfsonian FIU Object Collection ( " + newItem.Bib_Info.Location.Holding_Name + " )";
        //                    }
        //                    else
        //                    {
        //                        newItem.Bib_Info.Location.Holding_Name = "The Wolfsonian FIU Object Collection";
        //                    }
        //                    newItem.Behaviors.Add_Aggregation("OBJECTS");
        //                }

        //                // Split and clean all values that need it
        //                Clean_METS(newItem);

        //                // Get the list of all files
        //                string[] allFiles = System.IO.Directory.GetFiles(currentDirectory, "*.jpg");
        //                int imagecounter = 1;
        //                foreach (string thisJpegFile in allFiles)
        //                {
        //                    // Get this file object
        //                    FileInfo thisJpegFileInfo = new FileInfo(thisJpegFile);
        //                    string currentFileName = thisJpegFileInfo.Name;

        //                    // Create a default file page label
        //                    string file_page_label = "Image " + imagecounter;
        //                    bool proper_label_found = false;

        //                    // Get the new name
        //                    string newFileName = thisJpegFileInfo.Name.Replace(thisJpegFileInfo.Extension, "").Replace(".", "_").Replace(",", "_").Replace(" ", "_");

        //                    // First, look for this in related items
        //                    if (newItem.Bib_Info.RelatedItems_Count > 0)
        //                    {
        //                        Related_Item_Info relatedItemToDelete = null;
        //                        foreach (Related_Item_Info relatedItem in newItem.Bib_Info.RelatedItems)
        //                        {
        //                            if ((relatedItem.Identifiers_Count > 0) && ( relatedItem.Identifiers[0].Type == "uri" ))
        //                            {
        //                                string identifier = relatedItem.Identifiers[0].Identifier + "  ";
        //                                if (identifier.IndexOf(currentFileName) > 0)
        //                                {
        //                                    // THIS IS FOR THE OLD FORMAT WITH TITLE AFTER THE IDENTIFIER
        //                                    //string possible_label = identifier.Substring(identifier.IndexOf(currentFileName) + currentFileName.Length).Trim();
        //                                    //if (possible_label.Length > 0)
        //                                    //{
        //                                    //    proper_label_found = true;
        //                                    //    file_page_label = possible_label;
        //                                    //    relatedItemToDelete = relatedItem;
        //                                    //    break;
        //                                    //}

        //                                    // Get the possible title from the title field in the related item
        //                                    if (relatedItem.hasMainTitle)
        //                                    {
        //                                        string possible_label = relatedItem.Main_Title.ToString();
        //                                        if (possible_label.Length > 0)
        //                                        {
        //                                            proper_label_found = true;
        //                                            file_page_label = possible_label;
        //                                            relatedItemToDelete = relatedItem;
        //                                            break;
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        if (relatedItemToDelete != null)
        //                            newItem.Bib_Info.Remove_Related_Item(relatedItemToDelete);
        //                    }

        //                    // Try pulling the name from the file then
        //                    if (!proper_label_found)
        //                    {
        //                        string left_over_label = thisJpegFileInfo.Name.Replace(foldername, "").Replace(".jpg", "").Replace(".JPG", "");
        //                        if (left_over_label.Length > 1)
        //                        {
        //                            file_page_label = left_over_label;
        //                        }
        //                    }

        //                    // Copy or rename the file
        //                    if (inplace)
        //                    {
        //                        File.Move(thisJpegFile, newDirectory + "\\" + newFileName + ".jpg");
        //                    }
        //                    else
        //                    {
        //                        File.Copy(thisJpegFile, newDirectory + "\\" + newFileName + ".jpg", true);
        //                    }

        //                    // Add this file
        //                    newItem.Divisions.Physical_Tree.Add_File(newFileName + ".jpg", file_page_label.Trim());

        //                    // Now, try to make and add the thumbnail
        //                    if (imagemagick_executable.Length > 0)
        //                    {
        //                        if ( ImageMagick_Create_JPEG( newDirectory + "\\" + newFileName + ".jpg", newDirectory + "\\" + newFileName + "thm.jpg", 150, 300))
        //                        {
        //                            newItem.Divisions.Physical_Tree.Add_File(newFileName + "thm.jpg");
        //                        }
        //                    }

        //                    // Increment image counter
        //                    imagecounter++;
        //                }

        //                // Look for the first thumbnail
        //                string[] thumbnails = Directory.GetFiles(newDirectory, "*thm.jpg");
        //                if (thumbnails.Length > 0)
        //                {
        //                    newItem.Behaviors.Main_Thumbnail = (new FileInfo(thumbnails[0])).Name;
        //                }


        //                // Change the name of the identifier 'accn' 
        //                foreach (Identifier_Info thisIdentifier in newItem.Bib_Info.Identifiers)
        //                {
        //                    if (thisIdentifier.Type == "accn")
        //                        thisIdentifier.Type = "accession number";
        //                }

        //                // Try to find the accession number.. may seem repetitive of work above, but
        //                // this METS/MODS may already have had the identifier converted to 'accession number'
        //                // from a previous iteration.  So, this search is done seperately, and more
        //                // generally
        //                string accession_number = String.Empty;
        //                foreach (Identifier_Info thisIdentifier in newItem.Bib_Info.Identifiers)
        //                {
        //                    if ((thisIdentifier.Type.IndexOf("accn", StringComparison.InvariantCultureIgnoreCase) >= 0) || (thisIdentifier.Type.IndexOf("accession", StringComparison.InvariantCultureIgnoreCase) >= 0))
        //                    {
        //                        accession_number = thisIdentifier.Identifier.Trim();
        //                        break;
        //                    }
        //                }

        //                // If an accession number was found and the related link URL was included in this
        //                // run, add this as a related link (assuming it doesn't already exist)
        //                if ((accession_number.Length > 0) && (archived_files_link.Length > 0))
        //                {
        //                    // Create the link for this item
        //                    string item_link = archived_files_link + accession_number;

        //                    // Make sure it does not already exist
        //                    bool preexisting = false;
        //                    if (newItem.Bib_Info.RelatedItems_Count > 0)
        //                    {
        //                        foreach (Related_Item_Info relatedItem in newItem.Bib_Info.RelatedItems)
        //                        {
        //                            if (relatedItem.URL.Length > 0)
        //                            {
        //                                if (String.Compare(relatedItem.URL, item_link, StringComparison.InvariantCultureIgnoreCase) == 0)
        //                                {
        //                                    preexisting = true;
        //                                    break;
        //                                }
        //                            }
        //                        }
        //                    }

        //                    // Add this if not already existing
        //                    if (!preexisting)
        //                    {
        //                        Related_Item_Info newRelatedItem = new Related_Item_Info();
        //                        newRelatedItem.URL = item_link;
        //                        newRelatedItem.Main_Title.Title = "Full-Resolution Files – Internal Access Only";
        //                        newItem.Bib_Info.Add_Related_Item(newRelatedItem);
        //                    }
        //                }


        //                // Save this METS then
        //                newItem.Save_METS();

        //                // Finally, save this new bib id and accession number to the log
        //                logWriter.WriteLine(foldername + "\t" + newBib + "\t" + DateTime.Now.ToString());
        //                logWriter.Flush();

        //                processed++;
        //            }
        //        }
        //        catch (Exception ee)
        //        {
        //            ErrorMessageBox_Internal showError = new ErrorMessageBox_Internal("Error caught while processing '" + foldername + "'.\n\n" + ee.Message + "\n\nStop processing or continue?", "Error caught");
        //            DialogResult showErrorResult = showError.ShowDialog();
        //            if (showErrorResult == DialogResult.Abort)
        //            {
        //                logWriter.Flush();
        //                logWriter.Close();
        //                OnProcessComplete(true, processed);
        //            }
        //        }
        //    }

        //    logWriter.Flush();
        //    logWriter.Close();
        //    OnProcessComplete(false, processed);
        //}

        private void Clean_METS(SobekCM_Item thisItem)
        {
            // Change the subtitle to be lower case (may later make it more appropriately "camel case")
            if ((thisItem.Bib_Info.Main_Title.Subtitle.Length > 1) && ( Char.IsUpper( thisItem.Bib_Info.Main_Title.Subtitle[1] )))
            {
                thisItem.Bib_Info.Main_Title.Subtitle = thisItem.Bib_Info.Main_Title.Subtitle.ToLower();
            }

            // Copy the subtitle over to genres, if no matching genre exists
            if (thisItem.Bib_Info.Main_Title.Subtitle.Length > 0)
            {
                bool subtitle_found_as_genre = false;
                foreach (Genre_Info thisGenre in thisItem.Bib_Info.Genres)
                {
                    if (String.Compare(thisGenre.Genre_Term, thisItem.Bib_Info.Main_Title.Subtitle, true) == 0)
                    {
                        subtitle_found_as_genre = true;
                        break;
                    }
                }
                if (!subtitle_found_as_genre)
                {
                    thisItem.Bib_Info.Add_Genre(thisItem.Bib_Info.Main_Title.Subtitle.ToLower());
                }
            }

            // If there are no brackets around the subtitle, add them now
            if (( thisItem.Bib_Info.Main_Title.Subtitle.Length > 1 ) && ( thisItem.Bib_Info.Main_Title.Subtitle[0] != '[' ))
            {
                thisItem.Bib_Info.Main_Title.Subtitle = '[' + thisItem.Bib_Info.Main_Title.Subtitle + ']';
            }

            // Clean up the punctuation in the place terms
            if (thisItem.Bib_Info.Origin_Info.Places_Count > 0)
            {
                foreach (Origin_Info_Place thisPlace in thisItem.Bib_Info.Origin_Info.Places)
                {
                    thisPlace.Place_Text = thisPlace.Place_Text.Replace(" __", ", ");
                }
            }


            // If there is only one name, may need to split it
            if (thisItem.Bib_Info.Names_Count == 1)
            {
                // Look at the only named entity to see if splitting is necessary
                Name_Info namedEntity = thisItem.Bib_Info.Names[0];
                string completeName = namedEntity.Full_Name;
                if ((completeName.IndexOf("__") > 0) || (completeName.IndexOf("||") > 0))
                {
                    // First split into the seperate possible names
                    string[] splitter = completeName.Split("|".ToCharArray());
                    foreach (string thisSplit in splitter)
                    {
                        // Does this have length? ( Since there are two pipes side by side some will have no length)
                        if (thisSplit.Length > 0)
                        {
                            if (thisSplit.IndexOf("__") > 0)
                            {
                                string name = thisSplit.Substring(0, thisSplit.IndexOf("__"));
                                string roles = thisSplit.Substring(thisSplit.IndexOf("__") + 2).Trim();
                                while (roles[roles.Length - 1] == '_')
                                    roles = roles.Substring(0, roles.Length - 1);
                                roles = roles.Replace("__", ", ");
                                thisItem.Bib_Info.Add_Named_Entity(name.Trim(), roles.Trim());
                            }
                            else
                            {
                                thisItem.Bib_Info.Add_Named_Entity(thisSplit.Trim());
                            }
                        }
                    }

                    // Remove the original name
                    thisItem.Bib_Info.Remove_Name(namedEntity);
                }
            }

            // Split subjects into seperate topics
            if (thisItem.Bib_Info.Subjects_Count > 0)
            {
                foreach (Subject_Info thisSubject in thisItem.Bib_Info.Subjects)
                {
                    // Is this a standard subject type (vs. name subject, hierarchical geo, or title subject)
                    if (thisSubject.Class_Type == Subject_Info_Type.Standard)
                    {
                        // Cast to a standard subject
                        Subject_Info_Standard standardSubject = (Subject_Info_Standard)thisSubject;

                        // Look at the topics, if there are some
                        if (standardSubject.Topics_Count > 0)
                        {
                            // Step through each existing topic
                            List<string> newTopics = new List<string>();
                            foreach (string existingTopic in standardSubject.Topics)
                            {
                                // Split this topic up at the '--' string
                                string topic_chopper = existingTopic;
                                while (topic_chopper.IndexOf("--") > 0)
                                {
                                    newTopics.Add(topic_chopper.Substring(0, topic_chopper.IndexOf("--")));
                                    topic_chopper = topic_chopper.Substring(topic_chopper.IndexOf("--") + 2);
                                }
                                if (topic_chopper.Length > 0)
                                {
                                    newTopics.Add(topic_chopper.Trim());
                                }
                            }

                            // Now clear existing topics and copy over the ones
                            standardSubject.Clear_Topics();
                            foreach (string newTopic in newTopics)
                                standardSubject.Add_Topic(newTopic.Trim());
                        }
                    }
                }
            }

            // Get the VRA Core object
            VRACore_Info vraInfo = thisItem.Get_Metadata_Module( GlobalVar.VRACORE_METADATA_MODULE_KEY) as VRACore_Info;
            if (vraInfo == null)
            {
                vraInfo = new VRACore_Info();
                thisItem.Add_Metadata_Module(GlobalVar.VRACORE_METADATA_MODULE_KEY, vraInfo );
            }

            // Split materials into seperate fields
            if (vraInfo.Material_Count > 0)
            {
                List<VRACore_Materials_Info> newMaterials = new List<VRACore_Materials_Info>();
                foreach (VRACore_Materials_Info thisMaterial in vraInfo.Materials)
                {
                    if (thisMaterial.Materials.IndexOf("--") < 0)
                    {
                        newMaterials.Add(thisMaterial);
                    }
                    else
                    {
                        // Split this material up at the '--' string
                        string material_chopper = thisMaterial.Materials;
                        while (material_chopper.IndexOf("--") > 0)
                        {
                            newMaterials.Add(new VRACore_Materials_Info(material_chopper.Substring(0, material_chopper.IndexOf("--")).Trim(), thisMaterial.Type));
                            material_chopper = material_chopper.Substring(material_chopper.IndexOf("--") + 2);
                        }
                        if (material_chopper.Length > 0)
                        {
                            newMaterials.Add(new VRACore_Materials_Info(material_chopper.Trim(), thisMaterial.Type));
                        }
                    }
                }

                // Now clear existing topics and copy over the ones
                vraInfo.Clear_Materials();
                foreach (VRACore_Materials_Info newMaterial in newMaterials)
                    vraInfo.Add_Material(newMaterial);
            }

            // Clean up the measurements
            if (vraInfo.Measurement_Count > 0)
            {
                List<VRACore_Measurement_Info> newMeasurements = new List<VRACore_Measurement_Info>();

                foreach (VRACore_Measurement_Info thisMeasurement in vraInfo.Measurements)
                {
                    string[] measurement_splitter = thisMeasurement.Measurements.Split("|".ToCharArray());
                    foreach (string thisSplit in measurement_splitter)
                    {
                        if (thisSplit.Length > 0)
                        {
                            string newmeasurement = thisSplit.Replace("Inches __", "").Replace("Centimeters __", "");

                            while (newmeasurement.IndexOf("__ __") > 0)
                                newmeasurement = newmeasurement.Replace("__ __", "__");

                            List<string> measurement_portions = new List<string>();
                            string measurement_chopper = newmeasurement;
                            while (measurement_chopper.IndexOf("__") > 0)
                            {
                                measurement_portions.Add(measurement_chopper.Substring(0, measurement_chopper.IndexOf("__")).Trim());

                                measurement_chopper = measurement_chopper.Substring(measurement_chopper.IndexOf("__") + 2);
                            }
                            if (measurement_chopper.Length > 0)
                            {
                                measurement_portions.Add(measurement_chopper.Trim());
                            }

                            StringBuilder newMeasurementBuilder = new StringBuilder(100);
                            bool first = true;
                            foreach (string thisPortion in measurement_portions)
                            {
                                if (thisPortion.Length > 0)
                                {
                                    if (Char.IsNumber(thisPortion[0]))
                                    {
                                        if (!first)
                                            newMeasurementBuilder.Append(" x ");
                                        else
                                            first = false;

                                        newMeasurementBuilder.Append(thisPortion + " " + thisMeasurement.Units);
                                    }
                                    else
                                    {
                                        newMeasurementBuilder.Append(" ( " + thisPortion + " )");
                                    }
                                }
                            }
                            newMeasurements.Add(new VRACore_Measurement_Info(newMeasurementBuilder.ToString(), thisMeasurement.Units));
                        }
                    }
                }

                // Clear the old measurements and use the new one
                vraInfo.Clear_Measurements();
                foreach (VRACore_Measurement_Info thisMeasurements in newMeasurements)
                    vraInfo.Add_Measurement(thisMeasurements);
            }

            // Remove any trailing periods on genres
            if (thisItem.Bib_Info.Genres_Count > 0)
            {
                foreach (Genre_Info thisGenre in thisItem.Bib_Info.Genres)
                {
                    if (thisGenre.Genre_Term.Length > 1)
                    {
                        if (thisGenre.Genre_Term[thisGenre.Genre_Term.Length - 1] == '.')
                        {
                            thisGenre.Genre_Term = thisGenre.Genre_Term.Substring(0, thisGenre.Genre_Term.Length - 1);
                        }
                    }
                }
            }

            // Remove any trailing periods on roles for named entities
            if (thisItem.Bib_Info.Names_Count > 0)
            {
                foreach (Name_Info thisName in thisItem.Bib_Info.Names)
                {
                    foreach (Name_Info_Role thisRole in thisName.Roles)
                    {
                        if (thisRole.Role.Length > 1)
                        {
                            if (thisRole.Role[thisRole.Role.Length - 1] == '.')
                            {
                                thisRole.Role = thisRole.Role.Substring(0, thisRole.Role.Length - 1);
                            }
                        }
                    }
                }
            }

            // Remove any trailing bracket (or starting bracket) from date created
            if (thisItem.Bib_Info.Origin_Info.Date_Issued.Length > 0)
            {
                thisItem.Bib_Info.Origin_Info.Date_Issued = thisItem.Bib_Info.Origin_Info.Date_Issued.Replace("[", "").Replace("]", "").Replace("_", "-").Replace("--", "-").Replace("--", "-").Replace("--", "-").Trim();
            }

            // Remove any invalid date issued
            if (thisItem.Bib_Info.Origin_Info.Date_Issued.Length > 0)
            {
                if ((thisItem.Bib_Info.Origin_Info.Date_Issued.IndexOf("s.d") == 0) || (thisItem.Bib_Info.Origin_Info.Date_Issued.IndexOf("n.d") == 0))
                {
                    thisItem.Bib_Info.Origin_Info.Date_Issued = String.Empty;
                }
            }

            // Remove any invalid locations
            if (thisItem.Bib_Info.Origin_Info.Places_Count > 0)
            {
                List<Origin_Info_Place> placesToRemove = new List<Origin_Info_Place>();
                foreach (Origin_Info_Place thisPlace in thisItem.Bib_Info.Origin_Info.Places)
                {
                    if (thisPlace.Place_Text.IndexOf("s.l") == 0)
                        placesToRemove.Add(thisPlace);
                }
                foreach (Origin_Info_Place thisPlace in placesToRemove)
                {
                    thisItem.Bib_Info.Origin_Info.Remove_Place(thisPlace);
                }
            }

            // Remove any invalid publishers
            if (thisItem.Bib_Info.Publishers_Count > 0)
            {
                List<Publisher_Info> publishersToRemove = new List<Publisher_Info>();
                foreach (Publisher_Info thisPublisher in thisItem.Bib_Info.Publishers)
                {
                    if (thisPublisher.Name.IndexOf("s.n") == 0)
                    {
                        publishersToRemove.Add(thisPublisher);
                    }
                }
                foreach (Publisher_Info thisPublisher in publishersToRemove)
                {
                    thisItem.Bib_Info.Remove_Publisher(thisPublisher);
                }
            }        
        }

        private void OnNewProgress(int Value, int Max)
        {
            if (New_Progress != null)
            {
                if (Value > Max)
                {
                    New_Progress(Value, Value);
                }
                else
                {
                    New_Progress(Value, Max);
                }
            }
        }

        private void OnNewVolume(string newMessage)
        {
            if (New_Volume_String != null)
                New_Volume_String(newMessage);
            // myLog.AddComplete(newMessage);
        }

        private void OnProcessComplete( bool Aborted, int packages_processed_count )
        {
            if (Process_Complete != null)
                Process_Complete(Aborted, packages_processed_count, "WOLF" + bibid.ToString().PadLeft(6, '0'));
        }

        /// <summary>  </summary>
        /// <param name="BuilderFolder"> Builder folder upon which to perform all work </param>
        /// <param name="IncomingPackages"> List of valid incoming packages, which may be modified by this process </param>
        /// <param name="Deletes"> List of valid deletes, which may be modifyed by this process </param>
        public override void DoWork(Actionable_Builder_Source_Folder BuilderFolder, List<Incoming_Digital_Resource> IncomingPackages, List<Incoming_Digital_Resource> Deletes)
        {
            // Only continue if the processing folder exists and has subdirectories
            if (!Directory.Exists(BuilderFolder.Inbound_Folder))
                return;

            // Look for top-level XML files
            string[] xml_files = Directory.GetFiles(BuilderFolder.Inbound_Folder, "*.xml");

            // Used for the next bib id
            string next_bibid;
            int next_bibid_int = -1;

            // Handle any loose XML files forst
            if (xml_files.Length > 0)
            {
                // Get the next BibID
                next_bibid = Get_Next_BibID("WOLF");
                if ((String.IsNullOrEmpty(next_bibid)) || (next_bibid.IndexOf("ERROR") == 0))
                {
                    OnError(next_bibid, "WolfsonianLibraryProcessorModule", "Wolfsonian Library XML", -1);
                    return;
                }

                next_bibid_int = Convert.ToInt32(next_bibid.Substring(4));

                // Step through each XML file
                foreach (string this_xml_file in xml_files)
                {
                    // Look to see if this exists
                    string identifier_from_name = Path.GetFileNameWithoutExtension(this_xml_file);
                    string existing_bib_vid = Get_BibID_VID_From_Identifier(identifier_from_name);

                    // Was this an error
                    if ((!String.IsNullOrEmpty(existing_bib_vid)) && (existing_bib_vid.IndexOf("ERROR") == 0))
                    {
                        OnError(existing_bib_vid, identifier_from_name, "Wolfsonian Library XML", -1);
                        return;
                    }

                    // Do we need to set a BibID/VID?
                    string bib_vid = existing_bib_vid;
                    if (String.IsNullOrEmpty(existing_bib_vid))
                    {
                        string new_bibid = "WOLF" + next_bibid_int.ToString().PadLeft(6, '0');
                        bib_vid = new_bibid + "_00001";
                        next_bibid_int++;
                    }

                    // Did this exist?
                    if (!String.IsNullOrEmpty(bib_vid))
                    {
                        // Create the bibid/vid folder directly in processing to save time
                        string new_folder = Path.Combine(BuilderFolder.Processing_Folder, bib_vid);
                        if (!Directory.Exists(new_folder))
                            Directory.CreateDirectory(new_folder);

                        // Create the subfolder
                        if (!Directory.Exists(Path.Combine(new_folder, "sobek_files")))
                            Directory.CreateDirectory(Path.Combine(new_folder, "sobek_files"));

                        // Move the XML file over there
                        string new_xml_file = Path.Combine(new_folder, "sobek_files", Path.GetFileName(this_xml_file));
                        File.Move(this_xml_file, new_xml_file);

                        // Create a new empty item
                        SobekCM_Item newItem = new SobekCM_Item();

                        // Read the MDOS information from the XML file
                        Stream reader = new FileStream(new_xml_file, FileMode.Open, FileAccess.Read);
                        MODS_File_ReaderWriter modsReader = new MODS_File_ReaderWriter();
                        String error = String.Empty;
                        modsReader.Read_Metadata(reader, newItem, null, out error);

                        // Get the BibID and VID
                        string bibid = bib_vid.Substring(0, 10);
                        string vid = bib_vid.Substring(11);

                        // Set some values
                        newItem.BibID = bibid;
                        newItem.VID = vid;
                        newItem.Bib_Info.Source.Code = "iWOLF";
                        newItem.Bib_Info.Source.Statement = "The Wolfsonian-Florida International University";
                        if (newItem.Bib_Info.Type.MODS_Type == TypeOfResource_MODS_Enum.UNKNOWN)
                            newItem.Bib_Info.Type.MODS_Type = TypeOfResource_MODS_Enum.Three_Dimensional_Object;
                        if (newItem.Bib_Info.Main_Title.Title.Length == 0)
                            newItem.Bib_Info.Main_Title.Title = "NO TITLE";
                        newItem.Source_Directory = new_folder;


                        // We are now removing all Genres
                        newItem.Bib_Info.Clear_Genres();


                        newItem.Bib_Info.Location.Holding_Code = "iWOLF";
                        if (newItem.Bib_Info.Location.Holding_Name.Length > 0)
                        {
                            newItem.Bib_Info.Location.Holding_Name = "The Wolfsonian FIU Library Collection ( " + newItem.Bib_Info.Location.Holding_Name + " )";
                        }
                        else
                        {
                            newItem.Bib_Info.Location.Holding_Name = "The Wolfsonian FIU Library Collection";
                        }
                        newItem.Behaviors.Add_Aggregation("LIBRARY");

                        Clean_METS(newItem);

                        // Change the name of the identifier 'accn' 
                        foreach (Identifier_Info thisIdentifier in newItem.Bib_Info.Identifiers)
                        {
                            if (thisIdentifier.Type == "accn")
                                thisIdentifier.Type = "accession number";
                        }

                        // Try to find the accession number.. may seem repetitive of work above, but
                        // this METS/MODS may already have had the identifier converted to 'accession number'
                        // from a previous iteration.  So, this search is done seperately, and more
                        // generally
                        string accession_number = String.Empty;
                        foreach (Identifier_Info thisIdentifier in newItem.Bib_Info.Identifiers)
                        {
                            if ((thisIdentifier.Type.IndexOf("accn", StringComparison.InvariantCultureIgnoreCase) >= 0) || (thisIdentifier.Type.IndexOf("accession", StringComparison.InvariantCultureIgnoreCase) >= 0))
                            {
                                accession_number = thisIdentifier.Identifier.Trim();
                                break;
                            }
                        }

                        // If an accession number was found and the related link URL was included in this
                        // run, add this as a related link (assuming it doesn't already exist)
                        if ((accession_number.Length > 0) && (archived_files_link.Length > 0))
                        {
                            // Create the link for this item
                            string item_link = archived_files_link + accession_number;

                            // Make sure it does not already exist
                            bool preexisting = false;
                            if (newItem.Bib_Info.RelatedItems_Count > 0)
                            {
                                foreach (Related_Item_Info relatedItem in newItem.Bib_Info.RelatedItems)
                                {
                                    if (relatedItem.URL.Length > 0)
                                    {
                                        if (String.Compare(relatedItem.URL, item_link, StringComparison.InvariantCultureIgnoreCase) == 0)
                                        {
                                            preexisting = true;
                                            break;
                                        }
                                    }
                                }
                            }

                            // Add this if not already existing
                            if (!preexisting)
                            {
                                Related_Item_Info newRelatedItem = new Related_Item_Info();
                                newRelatedItem.URL = item_link;
                                newRelatedItem.Main_Title.Title = "Full-Resolution Files – Internal Access Only";
                                newItem.Bib_Info.Add_Related_Item(newRelatedItem);
                            }
                        }


                        // Save this METS then
                        newItem.Save_METS();

                    }
                }
            }

            // Look for non-bib id folders
            string[] incoming_folders = Directory.GetDirectories(BuilderFolder.Inbound_Folder);
            List<string> non_bib_folders = new List<string>();
            foreach (string this_incoming_folder in incoming_folders)
            {
                string folder_name = Path.GetFileName(this_incoming_folder);
                bool is_bib_folder = false;

                // Is this possibly the bibid?
                if (folder_name.Length == 10)
                {
                    if (SobekCM_Item.is_bibid_format(folder_name))
                        is_bib_folder = true;
                }

                // Is this possible the bibid_vid?
                if ((folder_name.Length == 16) && (folder_name[10] == '_'))
                {
                    if (SobekCM_Item.is_bibid_format(folder_name.Substring(0,10)))
                        is_bib_folder = true;
                }

                // If not bib format, collect it to be pre-processed
                if (!is_bib_folder)
                {
                    non_bib_folders.Add(this_incoming_folder);
                }
            }

            // Handle any non-BibID folders
            if (non_bib_folders.Count > 0)
            {
                //// Did we already get the next bib id?
                //if (next_bibid_int < 0)
                //{
                //    next_bibid = Get_Next_BibID("WOLF");
                //    if ((String.IsNullOrEmpty(next_bibid)) || (next_bibid.IndexOf("ERROR") == 0))
                //    {
                //        OnError(next_bibid, "WolfsonianLibraryProcessorModule", "Wolfsonian Library XML", -1);
                //        return;
                //    }

                //    next_bibid_int = Convert.ToInt32(next_bibid.Substring(4));
                //}

                // Step through each one
                foreach (string this_non_bib_folder in non_bib_folders)
                {
                    string identifier_from_name = Path.GetFileName(this_non_bib_folder);
                    string existing_bib_vid = Get_BibID_VID_From_Identifier(identifier_from_name);

                    // Did this exist?
                    if (!String.IsNullOrEmpty(existing_bib_vid))
                    {
                        try
                        {
                            // Create the bibid/vid folder directly in processing to save time
                            string new_folder = Path.Combine(BuilderFolder.Processing_Folder, existing_bib_vid);
                            if (!Directory.Exists(new_folder))
                                Directory.CreateDirectory(new_folder);

                            // Copy over all the files 
                            string[] files = Directory.GetFiles(this_non_bib_folder);
                            foreach (string thisFile in files)
                            {
                                // Also remove any periods here
                                string extension = Path.GetExtension(thisFile);
                                string filename = Path.GetFileNameWithoutExtension(thisFile);

                                string new_file = Path.Combine(new_folder, filename.Replace(".","_") + extension );
                                File.Move(thisFile, new_file);
                            }

                            // Try to delete the folder
                            Directory.Delete(this_non_bib_folder);
                        }
                        catch (Exception ee)
                        {
                            // Exception here
                            OnError("Unable to move the non-bib folder into bib format.  " + ee.Message, existing_bib_vid, "INCOMING", -1);
                        }
                    }

                }
            }
        }

        private string Get_Next_BibID(string BibIdStart)
        {
            DataSet resultSet = new DataSet();

            // Create the SQL connection
            using (SqlConnection sqlConnect = new SqlConnection("data source=SOB-SQL01\\SOBEK2;initial catalog=wolfsonian;integrated security=Yes;"))
            {
                try
                {
                    sqlConnect.Open();
                }
                catch
                {
                    return "ERROR : Unable to open SQL connection";
                }

                // Create the SQL command
                SqlCommand sqlCommand = new SqlCommand("SobekCM_Get_Next_BibID", sqlConnect)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Add the parameters
                sqlCommand.Parameters.AddWithValue("BibIdStart", BibIdStart);

                // Run the command itself
                try
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);

                    adapter.Fill(resultSet);
                }
                catch (Exception ee)
                {
                    return "ERROR : Exception caught while executing SobekCM_Get_Next_BibID ( " + ee.Message + " )";
                }

                // Close the connection (not technical necessary since we put the connection in the
                // scope of the using brackets.. it would dispose itself anyway)
                try
                {
                    sqlConnect.Close();
                }
                catch
                {
                    return "ERROR : Unable to close SQL connection";
                }
            }

            if ((resultSet.Tables.Count == 0) || (resultSet.Tables[0].Rows.Count == 0))
                return String.Empty;

            return resultSet.Tables[0].Rows[0]["NextBibId"].ToString();
        }

        private string Get_BibID_VID_From_Identifier(string Identifier)
        {
            // Create the SQL connection
            using (SqlConnection sqlConnect = new SqlConnection("data source=SOB-SQL01\\SOBEK2;initial catalog=wolfsonian;integrated security=Yes;"))
            {
                try
                {
                    sqlConnect.Open();
                }
                catch
                {
                    return "ERROR : Unable to open SQL connection";
                }

                // Create the SQL command
                SqlCommand sqlCommand = new SqlCommand("Wolfsonian_Library_Item_By_Identifier", sqlConnect)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Add the parameters
                sqlCommand.Parameters.AddWithValue("Identifier", Identifier);

                // Run the command itself
                try
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);

                    DataSet resultSet = new DataSet();
                    adapter.Fill(resultSet);

                    if ((resultSet.Tables.Count == 0) || (resultSet.Tables[0].Rows.Count == 0))
                        return String.Empty;

                    return resultSet.Tables[0].Rows[0]["BibID"] + "_" + resultSet.Tables[0].Rows[0]["VID"];

                }
                catch ( Exception ee )
                {
                    return "ERROR : Exception caught while executing Wolfsonian_Library_Item_By_Identifier ( " + ee.Message + " )";
                }

                // Close the connection (not technical necessary since we put the connection in the
                // scope of the using brackets.. it would dispose itself anyway)
                try
                {
                    sqlConnect.Close();
                }
                catch
                {
                    return "ERROR : Unable to close SQL connection";
                }
            }
        }
    }
}
