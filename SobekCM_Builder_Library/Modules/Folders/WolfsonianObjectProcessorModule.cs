#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using SobekCM.Engine_Library.Email;
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
    public class WolfsonianObjectProcessorModule : abstractFolderModule
    {
        private const string archived_files_link = "http://resources.wolfsonian.org/";
        private const string log_email_address = "Mark.V.Sullivan@sobekdigital.com;labs@thewolf.fiu.edu";

        /// <summary> Constructor for a new instance of the WolfsonianObjectProcessorModule class </summary>
        public WolfsonianObjectProcessorModule()
        {
            // Do nothing
        }

        #region Method that cleans all the metadata, like splitting some fields, etc..

        private void Clean_METS(SobekCM_Item thisItem)
        {
            // Change the subtitle to be lower case (may later make it more appropriately "camel case")
            if ((thisItem.Bib_Info.Main_Title.Subtitle.Length > 1) && (Char.IsUpper(thisItem.Bib_Info.Main_Title.Subtitle[1])))
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
            if ((thisItem.Bib_Info.Main_Title.Subtitle.Length > 1) && (thisItem.Bib_Info.Main_Title.Subtitle[0] != '['))
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
            VRACore_Info vraInfo = thisItem.Get_Metadata_Module(GlobalVar.VRACORE_METADATA_MODULE_KEY) as VRACore_Info;
            if (vraInfo == null)
            {
                vraInfo = new VRACore_Info();
                thisItem.Add_Metadata_Module(GlobalVar.VRACORE_METADATA_MODULE_KEY, vraInfo);
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
                            string newmeasurement = thisSplit.Replace("Inches __", "").Replace("Centimeters __", "").Replace("inches __", "").Replace("centimeters __", ""); ;

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

        #endregion

        /// <summary>  </summary>
        /// <param name="BuilderFolder"> Builder folder upon which to perform all work </param>
        /// <param name="IncomingPackages"> List of valid incoming packages, which may be modified by this process </param>
        /// <param name="Deletes"> List of valid deletes, which may be modifyed by this process </param>
        public override void DoWork(Actionable_Builder_Source_Folder BuilderFolder, List<Incoming_Digital_Resource> IncomingPackages, List<Incoming_Digital_Resource> Deletes)
        {
            // Determine if this is library or not
            bool isLibrary = true;
            if ((Arguments != null) && (Arguments.Count > 0))
            {
                if (String.Compare(Arguments[0], "OBJECTS", StringComparison.OrdinalIgnoreCase) == 0)
                    isLibrary = false;
            }

            // Only continue if the processing folder exists and has subdirectories
            if (!Directory.Exists(BuilderFolder.Processing_Folder))
                return;

            // Create the log
            List<string> error_log_for_email = new List<string>();
            List<string> new_log_for_email = new List<string>();
            List<string> existing_log_for_email = new List<string>();

            // Used for the next bib id
            string next_bibid = null;
            int next_bibid_int = -1;

            // Determine generic flag used on error
            string errorItemDesc = "Wolfsonian Library XML";
            if (!isLibrary)
                errorItemDesc = "Wolfsonian Object XML";

            // Handle any loose XML files first (although these really should have been put in folders during the 
            // previous process running in MoveAgedPackageesToProcessModule )
            string[] xml_files = Directory.GetFiles(BuilderFolder.Processing_Folder, "*.xml");
            if (xml_files.Length > 0)
            {
                // Get the next BibID
                next_bibid = Get_Next_BibID("WOLF");
                if ((String.IsNullOrEmpty(next_bibid)) || (next_bibid.IndexOf("ERROR") == 0))
                {
                    OnError(next_bibid, "WolfsonianObjectProcessorModule", errorItemDesc, -1);
                    return;
                }

                next_bibid_int = Convert.ToInt32(next_bibid.Substring(4));

                // Step through each XML file
                foreach (string this_xml_file in xml_files)
                {
                    // Look to see if this exists
                    string identifier_from_name = Path.GetFileNameWithoutExtension(this_xml_file);
                    string existing_bib_vid = null;

                    if (isLibrary)
                        existing_bib_vid = Get_BibID_VID_From_Library_Identifier(identifier_from_name);
                    else
                    {
                        string object_bib_lookup_message = null;
                        string object_bib_lookup_bib = null;
                        string object_bib_lookup_vid = null;

                        if (Wolfsonian_Find_BibID_From_Accession_Number(identifier_from_name, out object_bib_lookup_bib, out object_bib_lookup_vid, out object_bib_lookup_message))
                        {
                            existing_bib_vid = object_bib_lookup_bib + "_" + object_bib_lookup_vid;
                        }
                    }

                    // Was this an error
                    if ((!String.IsNullOrEmpty(existing_bib_vid)) && (existing_bib_vid.IndexOf("ERROR") == 0))
                    {
                        OnError(existing_bib_vid, identifier_from_name, errorItemDesc, -1);
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

                        // Process this XML file
                        Process_Xml_File(this_xml_file, new_folder, bib_vid, isLibrary);

                    }
                }
            }

            // Look for non-bib id folders
            string[] incoming_folders = Directory.GetDirectories(BuilderFolder.Processing_Folder);
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
                    if (SobekCM_Item.is_bibid_format(folder_name.Substring(0, 10)))
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
                // Step through each one
                foreach (string this_non_bib_folder in non_bib_folders)
                {
                    // Get any existing bib/vid from the identifier
                    string identifier_from_name = Path.GetFileName(this_non_bib_folder);
                    string existing_bib_vid = null;
                    if (isLibrary)
                        existing_bib_vid = Get_BibID_VID_From_Library_Identifier(identifier_from_name);
                    else
                    {
                        string object_bib_lookup_message = null;
                        string object_bib_lookup_bib = null;
                        string object_bib_lookup_vid = null;

                        if (Wolfsonian_Find_BibID_From_Accession_Number(identifier_from_name, out object_bib_lookup_bib, out object_bib_lookup_vid, out object_bib_lookup_message))
                        {
                            existing_bib_vid = object_bib_lookup_bib + "_" + object_bib_lookup_vid;
                        }

                        if (object_bib_lookup_bib.Length == 0)
                            existing_bib_vid = null;
                    }

                    // Did this exist?
                    if (!String.IsNullOrEmpty(existing_bib_vid))
                    {
                        // Is there an XML file?
                        string[] xml_files_non_bib_folder = Directory.GetFiles(this_non_bib_folder, "*.xml");

                        // If there are multiple XML files, do no more processing here
                        if (xml_files_non_bib_folder.Length >= 2)
                        {
                            OnError(errorItemDesc + " : Processing skipped due to multiple XML files present in " + identifier_from_name + " folder", existing_bib_vid.Replace("_", ":"), errorItemDesc, -1);
                            error_log_for_email.Add("Processing skipped due to multiple XML files present in " + identifier_from_name + " folder ( " + existing_bib_vid.Replace("_", ":") + ")");

                            continue;
                        }

                        // Log this
                        OnProcess(errorItemDesc + " : Found mapping between " + identifier_from_name + " and existing bib/vid", errorItemDesc, existing_bib_vid.Replace("_", ":"), "Incoming", -1);
                        existing_log_for_email.Add("Found mapping between " + identifier_from_name + " and existing bib/vid " + existing_bib_vid.Replace("_", ":"));

                        // Found a mapping to an existing bib_vid
                        try
                        {
                            // Create the bibid/vid folder 
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

                                string new_file = Path.Combine(new_folder, filename.Replace(".", "_") + extension);
                                File.Move(thisFile, new_file);
                            }

                            // Look again for the XML files
                            if (xml_files_non_bib_folder.Length == 1)
                            {
                                string xml_file_name = Path.Combine(new_folder, Path.GetFileNameWithoutExtension(xml_files_non_bib_folder[0]).Replace(".", "_") + ".xml");
                                if (File.Exists(xml_file_name))
                                {
                                    Process_Xml_File(xml_file_name, new_folder, existing_bib_vid, isLibrary);
                                }
                            }

                            // Try to delete the folder
                            Directory.Delete(this_non_bib_folder);
                        }
                        catch (Exception ee)
                        {
                            // Exception here
                            OnError(errorItemDesc + " : Unable to move the non-bib folder " + identifier_from_name + " into bib format ( " + existing_bib_vid + ")." + ee.Message, existing_bib_vid, errorItemDesc, -1);
                            error_log_for_email.Add("UNEXPECTED ERROR: Unable to move the non-bib folder " + identifier_from_name + " into bib format ( " + existing_bib_vid + ")." + ee.Message);
                        }
                    }
                    else
                    {
                        // No mapping was found, so we will create a new item
                        // Is there an XML file?
                        string[] xml_files_non_bib_folder = Directory.GetFiles(this_non_bib_folder, "*.xml");

                        // If there are multiple XML files, do no more processing here
                        if (xml_files_non_bib_folder.Length >= 2)
                        {
                            OnError(errorItemDesc + " : Processing skipped due to multiple XML files present in " + identifier_from_name + " folder", identifier_from_name, errorItemDesc, -1);
                            error_log_for_email.Add("ERROR: Processing skipped due to multiple XML files present in " + identifier_from_name + " folder");
                            continue;
                        }

                        // If there are multiple XML files, do no more processing here
                        if (xml_files_non_bib_folder.Length == 0)
                        {
                            OnError(errorItemDesc + " : Processing skipped due to no XML file present in new item " + identifier_from_name + " folder", identifier_from_name, errorItemDesc, -1);
                            error_log_for_email.Add("ERROR: Processing skipped due to no XML file present in new item " + identifier_from_name + " folder");
                            continue;
                        }

                        // Did we already get the next bib id?
                        if (next_bibid_int < 0)
                        {
                            next_bibid = Get_Next_BibID("WOLF");
                            if ((String.IsNullOrEmpty(next_bibid)) || (next_bibid.IndexOf("ERROR") == 0))
                            {
                                OnError(next_bibid, "WolfsonianObjectProcessorModule", errorItemDesc, -1);
                                return;
                            }

                            next_bibid_int = Convert.ToInt32(next_bibid.Substring(4));
                        }

                        // Get the next bibid
                        string new_bibid = "WOLF" + next_bibid_int.ToString().PadLeft(6, '0');
                        string bib_vid = new_bibid + "_00001";
                        next_bibid_int++;

                        // Log this
                        OnProcess(errorItemDesc + " : Creating new item " + bib_vid.Replace("_", ":") + " for incoming item " + identifier_from_name, errorItemDesc, bib_vid.Replace("_", ":"), "Incoming", -1);
                        new_log_for_email.Add("Creating new item " + bib_vid.Replace("_", ":") + " for incoming item " + identifier_from_name);

                        try
                        {

                            // Create the bibid/vid folder 
                            string new_folder = Path.Combine(BuilderFolder.Processing_Folder, bib_vid);
                            if (!Directory.Exists(new_folder))
                                Directory.CreateDirectory(new_folder);

                            // Copy over all the files 
                            string[] files = Directory.GetFiles(this_non_bib_folder);
                            foreach (string thisFile in files)
                            {
                                // Also remove any periods here
                                string extension = Path.GetExtension(thisFile);
                                string filename = Path.GetFileNameWithoutExtension(thisFile);

                                string new_file = Path.Combine(new_folder, filename.Replace(".", "_") + extension);
                                File.Move(thisFile, new_file);
                            }

                            // Look again for the XML files
                            if (xml_files_non_bib_folder.Length == 1)
                            {
                                string xml_file_name = Path.Combine(new_folder, Path.GetFileNameWithoutExtension(xml_files_non_bib_folder[0]).Replace(".", "_") + ".xml");
                                if (File.Exists(xml_file_name))
                                {
                                    Process_Xml_File(xml_file_name, new_folder, bib_vid, isLibrary);
                                }
                            }

                            // Try to delete the folder
                            Directory.Delete(this_non_bib_folder);
                        }
                        catch (Exception ee)
                        {
                            // Exception here
                            OnError(errorItemDesc + " : Unable to move the non-bib folder " + identifier_from_name + " into new bib format ( " + bib_vid + ")." + ee.Message, bib_vid, errorItemDesc, -1);
                            error_log_for_email.Add("UNEXPECTED ERROR: Unable to move the non-bib folder " + identifier_from_name + " into new bib format ( " + bib_vid + ")." + ee.Message);
                        }
                    }

                }
            }

            // Email the log?
            if (((error_log_for_email.Count > 0) || ( new_log_for_email.Count > 0 ) || ( existing_log_for_email.Count > 0)) && ( !String.IsNullOrEmpty(log_email_address)))
            {
                // Build the body
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("<p>The " + errorItemDesc + " module in the builder found new incoming folders/files to process.</p>");

                // Add any errors
                if (error_log_for_email.Count > 0)
                {
                    builder.AppendLine("<p style=\"font-weight:bold;color:red\">The following errors were detected during processing:</p>");
                    builder.AppendLine("<ul style=\"list-style:none;padding-bottom:20px;\">");
                    foreach (string thisLogLine in error_log_for_email)
                    {
                        builder.AppendLine("<li>" + thisLogLine + "</li>");
                    }
                    builder.AppendLine("</ul>");
                }

                // Add any new items
                if (new_log_for_email.Count > 0)
                {
                    builder.AppendLine("<p>The following new items are in the process of being loaded:</p>");
                    builder.AppendLine("<ul style=\"list-style:none;padding-bottom:20px;\">");
                    foreach (string thisLogLine in new_log_for_email)
                    {
                        builder.AppendLine("<li>" + thisLogLine + "</li>");
                    }
                    builder.AppendLine("</ul>");
                }


                // Add any new items
                if (existing_log_for_email.Count > 0)
                {
                    builder.AppendLine("<p>The following existing items are in the process of being updated:</p>");
                    builder.AppendLine("<ul style=\"list-style:none;padding-bottom:20px;\">");
                    foreach (string thisLogLine in existing_log_for_email)
                    {
                        builder.AppendLine("<li>" + thisLogLine + "</li>");
                    }
                    builder.AppendLine("</ul>");
                }

                // Send the email
                Email_Helper.SendEmail(log_email_address, errorItemDesc + " Processing Log", builder.ToString(), true, "Wolfsonian-FIU Digital Repository");
            }
        }

        private bool Process_Xml_File(string this_xml_file, string new_folder, string bib_vid, bool isLibrary )
        {
            // Create the subfolder
            if (!Directory.Exists(Path.Combine(new_folder, "sobek_files")))
                Directory.CreateDirectory(Path.Combine(new_folder, "sobek_files"));

            // Move the XML file over there
            string new_xml_file = Path.Combine(new_folder, "sobek_files", Path.GetFileNameWithoutExtension(this_xml_file) + "_recd_" + DateTime.Now.Year + "_" + DateTime.Now.Month.ToString().PadLeft(2,'0') + "_" + DateTime.Now.Day.ToString().PadLeft(2,'0') + ".xml");
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
            newItem.Bib_Info.Location.Holding_Code = "iWOLF";

            // We are now removing all Genres
            newItem.Bib_Info.Clear_Genres();

            if (isLibrary)
            {
                if (newItem.Bib_Info.Location.Holding_Name.Length > 0)
                {
                    newItem.Bib_Info.Location.Holding_Name = "The Wolfsonian FIU Library Collection ( " + newItem.Bib_Info.Location.Holding_Name + " )";
                }
                else
                {
                    newItem.Bib_Info.Location.Holding_Name = "The Wolfsonian FIU Library Collection";
                }
                newItem.Behaviors.Add_Aggregation("LIBRARY");
            }
            else
            {
                if (newItem.Bib_Info.Location.Holding_Name.Length > 0)
                {
                    newItem.Bib_Info.Location.Holding_Name = "The Wolfsonian FIU Object Collection ( " + newItem.Bib_Info.Location.Holding_Name + " )";
                }
                else
                {
                    newItem.Bib_Info.Location.Holding_Name = "The Wolfsonian FIU Object Collection";
                }
                newItem.Behaviors.Add_Aggregation("OBJECTS");
            }

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

            return true;
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

        private string Get_BibID_VID_From_Library_Identifier(string Identifier)
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
                catch (Exception ee)
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

        private bool Wolfsonian_Find_BibID_From_Accession_Number(string AccessionNumber, out string BibiD, out string VID, out string Message)
        {
            string connectionString = @"data source=SOB-SQL01\SOBEK2;initial catalog=wolfsonian;integrated security=Yes;";
            BibiD = String.Empty;
            VID = String.Empty;
            Message = String.Empty;

            try
            {
                // Create the connection
                SqlConnection connect = new SqlConnection(connectionString);

                // Create the command 
                SqlCommand executeCommand = new SqlCommand("Wolfsonian_Find_BibID_From_Accession_Number", connect);
                executeCommand.CommandType = CommandType.StoredProcedure;
                executeCommand.Parameters.AddWithValue("@AccessionNumber", AccessionNumber);
                SqlParameter bibParam = executeCommand.Parameters.AddWithValue("@BibID", String.Empty.PadLeft(10, ' '));
                bibParam.Direction = ParameterDirection.InputOutput;

                SqlParameter vidParam = executeCommand.Parameters.AddWithValue("@VID", String.Empty.PadLeft(10, ' '));
                vidParam.Direction = ParameterDirection.InputOutput;

                SqlParameter msgParam = executeCommand.Parameters.AddWithValue("@Message", String.Empty.PadLeft(200, ' '));
                msgParam.Direction = ParameterDirection.InputOutput;

                // Create the adapter
                connect.Open();
                executeCommand.ExecuteNonQuery();
                connect.Close();

                // Get the output params
                BibiD = bibParam.Value.ToString();
                VID = vidParam.Value.ToString();
                Message = msgParam.Value.ToString();

                return true;
            }
            catch (Exception ee)
            {
                return false;
            }

        }
    }
}
