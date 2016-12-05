using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SobekCM.Core.Aggregations;
using SobekCM.Core.BriefItem;
using SobekCM.Core.Client;
using SobekCM.Core.FileSystems;
using SobekCM.Core.MemoryMgmt;
using SobekCM.Core.Navigation;
using SobekCM.Core.UI_Configuration.Citation;
using SobekCM.Engine_Library.Configuration;
using SobekCM.Engine_Library.Email;
using SobekCM.Engine_Library.Items.BriefItems;
using SobekCM.Library.AdminViewer;
using SobekCM.Library.Citation;
using SobekCM.Library.Citation.SectionWriter;
using SobekCM.Library.Citation.Template;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.UI;
using SobekCM.Library.UploadiFive;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Resource_Object.GenericXml.Reader;
using SobekCM.Resource_Object.GenericXml.Results;
using SobekCM.Resource_Object.Mapping;
using SobekCM.Resource_Object.Metadata_File_ReaderWriters;
using SobekCM.Resource_Object.Utilities;
using SobekCM.Tools;
using SobekCM_Resource_Database;
using Image = System.Drawing.Image;

// STAGE 1: Upload new TEI or set mapping
// STAGE 2: Edit TEI
// STAGE 3: Preview Changes (have FINISH from here)
// STAGE 4: Edit Metadata



namespace SobekCM.Library.MySobekViewer
{
    public class Edit_TEI_Item_MySobekViewer2 : abstract_MySobekViewer
    {
        private bool criticalErrorEncountered;
        private readonly int currentProcessStep;
        private SobekCM_Item item;
        private readonly CompleteTemplate completeTemplate;
        private readonly string templateCode = "ir";
        private readonly string toolTitle;
        private readonly int totalTemplatePages;
        private readonly string userInProcessDirectory;
        private readonly List<string> validationErrors;

        private readonly string original_tei_file;
        private readonly string new_tei_file;
        private readonly string mapping_file;
        private readonly string xslt_file;
        private readonly string css_file;

        private string error_message;
        private readonly string success_message;

        private readonly SobekCM_Item editingItem;
        private readonly string bibid;
        private readonly string vid;



        #region Constructor

        /// <summary> Constructor for a new instance of the Edit_TEI_Item_MySobekViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        public Edit_TEI_Item_MySobekViewer2(RequestCache RequestSpecificValues) : base(RequestSpecificValues)
        {
            RequestSpecificValues.Tracer.Add_Trace("Edit_TEI_Item_MySobekViewer.Constructor", String.Empty);


            // If the RequestSpecificValues.Current_User cannot submit items, go back
            if (!RequestSpecificValues.Current_User.Can_Submit)
            {
                RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                return;
            }

            // Ensure the TEI plug-in is enabled
            if ((UI_ApplicationCache_Gateway.Configuration.Extensions == null) ||
                (UI_ApplicationCache_Gateway.Configuration.Extensions.Get_Extension("TEI") == null) ||
                (!UI_ApplicationCache_Gateway.Configuration.Extensions.Get_Extension("TEI").Enabled))
            {
                RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                return;
            }

            // Ensure BibID and VID provided
            RequestSpecificValues.Tracer.Add_Trace("Edit_Item_Metadata_MySobekViewer.Constructor", "Validate provided bibid / vid");
            if ((String.IsNullOrEmpty(RequestSpecificValues.Current_Mode.BibID)) || (String.IsNullOrEmpty(RequestSpecificValues.Current_Mode.VID)))
            {
                RequestSpecificValues.Tracer.Add_Trace("Edit_Item_Metadata_MySobekViewer.Constructor", "BibID or VID was not provided!");
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Error;
                RequestSpecificValues.Current_Mode.Error_Message = "Invalid Request : BibID/VID missing in item metadata edit request";
                return;
            }

            bibid = RequestSpecificValues.Current_Mode.BibID;
            vid = RequestSpecificValues.Current_Mode.VID;

            // Ensure the item is valid
            RequestSpecificValues.Tracer.Add_Trace("Edit_Item_Metadata_MySobekViewer.Constructor", "Validate bibid/vid exists");
            if (!UI_ApplicationCache_Gateway.Items.Contains_BibID_VID(bibid, vid))
            {
                RequestSpecificValues.Tracer.Add_Trace("Edit_Item_Metadata_MySobekViewer.Constructor", "BibID/VID indicated is not valid", Custom_Trace_Type_Enum.Error);
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Error;
                RequestSpecificValues.Current_Mode.Error_Message = "Invalid Request : BibID/VID indicated is not valid";
                return;
            }

            // Determine the in process directory for this
            if (RequestSpecificValues.Current_User.ShibbID.Trim().Length > 0)
                userInProcessDirectory = Path.Combine(UI_ApplicationCache_Gateway.Settings.Servers.In_Process_Submission_Location, RequestSpecificValues.Current_User.ShibbID, "teiedit", bibid + "_" + vid);
            else
                userInProcessDirectory = Path.Combine(UI_ApplicationCache_Gateway.Settings.Servers.In_Process_Submission_Location, RequestSpecificValues.Current_User.UserName.Replace(".", "").Replace("@", ""), "teiedit", bibid + "_" + vid);

            // Clear any old files (older than 24 hours) that are in the directory
            if (!Directory.Exists(userInProcessDirectory))
                Directory.CreateDirectory(userInProcessDirectory);
            else
            {
                // Anything older than a day should be deleted
                string[] files = Directory.GetFiles(userInProcessDirectory);
                foreach (string thisFile in files)
                {
                    DateTime modifiedDate = ((new FileInfo(thisFile)).LastWriteTime);
                    if (DateTime.Now.Subtract(modifiedDate).TotalHours > (24*7))
                    {
                        try
                        {
                            File.Delete(thisFile);
                        }
                        catch (Exception)
                        {
                            // Unable to delete existing file in the RequestSpecificValues.Current_User's folder.
                            // This is an error, but how to report it?
                        }
                    }
                }
            }

            // Load the CompleteTemplate
            completeTemplate = Template_MemoryMgmt_Utility.Retrieve_Template("tei_edit", RequestSpecificValues.Tracer);
            if (completeTemplate != null)
            {
                RequestSpecificValues.Tracer.Add_Trace("Edit_TEI_Item_MySobekViewer.Constructor", "Found template in cache");
            }
            else
            {
                RequestSpecificValues.Tracer.Add_Trace("Edit_TEI_Item_MySobekViewer.Constructor", "Reading template");

                string user_template = Path.Combine(UI_ApplicationCache_Gateway.Settings.Servers.Application_Server_Network, "plugins\\tei\\templates\\user\\edit.xml");
                if (!File.Exists(user_template))
                    user_template = Path.Combine(UI_ApplicationCache_Gateway.Settings.Servers.Application_Server_Network, "plugins\\tei\\templates\\default\\edit.xml");


                // Read this CompleteTemplate
                Template_XML_Reader reader = new Template_XML_Reader();
                completeTemplate = new CompleteTemplate();
                reader.Read_XML(user_template, completeTemplate, true);

                // Save this into the cache
                Template_MemoryMgmt_Utility.Store_Template("tei_edit", completeTemplate, RequestSpecificValues.Tracer);
            }

            // Determine the number of total CompleteTemplate pages
            totalTemplatePages = completeTemplate.InputPages_Count + 3;

            // Determine the title for this CompleteTemplate, or use a default
            toolTitle = completeTemplate.Title;
            if (toolTitle.Length == 0)
                toolTitle = "TEI Editing Tool";

            // Determine the current phase
            currentProcessStep = 1;
            if ((RequestSpecificValues.Current_Mode.My_Sobek_SubMode.Length > 0) && (Char.IsNumber(RequestSpecificValues.Current_Mode.My_Sobek_SubMode[0])))
            {
                Int32.TryParse(RequestSpecificValues.Current_Mode.My_Sobek_SubMode.Substring(0), out currentProcessStep);
            }

            // Load some information from the session
            if (HttpContext.Current.Session["Edit_TEI_mySobekViewer." + bibid + "_" + vid + ".Mapping_File"] != null)
                mapping_file = HttpContext.Current.Session["Edit_TEI_mySobekViewer." + bibid + "_" + vid + ".Mapping_File"] as string;
            if (HttpContext.Current.Session["Edit_TEI_mySobekViewer." + bibid + "_" + vid + ".XSLT_File"] != null)
                xslt_file = HttpContext.Current.Session["Edit_TEI_mySobekViewer." + bibid + "_" + vid + ".XSLT_File"] as string;
            if (HttpContext.Current.Session["Edit_TEI_mySobekViewer." + bibid + "_" + vid + ".CSS_File"] != null)
                css_file = HttpContext.Current.Session["Edit_TEI_Item_MySobekViewer." + bibid + "_" + vid + ".CSS_File"] as string;
            if (HttpContext.Current.Session["Edit_TEI_mySobekViewer." + bibid + "_" + vid + ".Original_TEI_File"] != null)
                original_tei_file = HttpContext.Current.Session["Edit_TEI_Item_MySobekViewer." + bibid + "_" + vid + ".Original_TEI_File"] as string;

            // Pull the current item beig edited
            editingItem = null;
            if (HttpContext.Current.Session["Edit_TEI_mySobekViewer." + bibid + "_" + vid + ".New_Item"] != null)
                editingItem = HttpContext.Current.Session["Edit_TEI_Item_MySobekViewer." + bibid + "_" + vid + ".New_Item"] as SobekCM_Item;
            
            // If any are null or blank, pull from the item
            if ((String.IsNullOrEmpty(mapping_file)) || (String.IsNullOrEmpty(xslt_file)) || (String.IsNullOrEmpty(original_tei_file)) || (editingItem == null))
            {
                // Pull the original item
                RequestSpecificValues.Tracer.Add_Trace("Edit_Item_Metadata_MySobekViewer.Constructor", "Try to pull this sobek complete item");
                SobekCM_Item currentItem = SobekEngineClient.Items.Get_Sobek_Item(bibid, vid, RequestSpecificValues.Current_User.UserID, RequestSpecificValues.Tracer);
                if (currentItem == null)
                {
                    RequestSpecificValues.Tracer.Add_Trace("Edit_Item_Metadata_MySobekViewer.Constructor", "Unable to build complete item");
                    RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Error;
                    RequestSpecificValues.Current_Mode.Error_Message = "Invalid Request : Unable to build complete item";
                    return;
                }

                // If the editing item was NULL, assign it
                if (editingItem == null)
                {
                    editingItem = currentItem;
                    editingItem.Source_Directory = userInProcessDirectory;
                    HttpContext.Current.Session["Edit_TEI_mySobekViewer." + bibid + "_" + vid + ".New_Item"] = editingItem;
                }

                // Settings SHOULD not be null
                if (currentItem.Behaviors.Settings != null)
                {
                    // Build the setting dictionary
                    Dictionary<string, string> settingDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    foreach (Tuple<string, string> setting in currentItem.Behaviors.Settings)
                    {
                        settingDictionary[setting.Item1] = setting.Item2;
                    }

                    // Now, use to assign to each value
                    if ((String.IsNullOrEmpty(mapping_file)) && (settingDictionary.ContainsKey("TEI.Mapping")))
                        mapping_file = settingDictionary["TEI.Mapping"];
                    if ((String.IsNullOrEmpty(css_file)) && (settingDictionary.ContainsKey("TEI.CSS")))
                        css_file = settingDictionary["TEI.CSS"];
                    if ((String.IsNullOrEmpty(xslt_file)) && (settingDictionary.ContainsKey("TEI.XSLT")))
                        xslt_file = settingDictionary["TEI.XSLT"];
                    if ((String.IsNullOrEmpty(original_tei_file)) && (settingDictionary.ContainsKey("TEI.Source_File")))
                        original_tei_file = settingDictionary["TEI.Source_File"];

                    // Now, save all these
                    HttpContext.Current.Session["Edit_TEI_mySobekViewer." + bibid + "_" + vid + ".Mapping_File"] = mapping_file;
                    HttpContext.Current.Session["Edit_TEI_mySobekViewer." + bibid + "_" + vid + ".XSLT_File"] = xslt_file;
                    HttpContext.Current.Session["Edit_TEI_mySobekViewer." + bibid + "_" + vid + ".CSS_File"] = css_file;
                    HttpContext.Current.Session["Edit_TEI_mySobekViewer." + bibid + "_" + vid + ".Original_TEI_File"] = original_tei_file;
                }
            }

            // If there is a boundary infraction here, go back to step 1
            if (currentProcessStep < 0)
                currentProcessStep = 1;
            if ((currentProcessStep > completeTemplate.InputPages.Count + 4) && (currentProcessStep != 8) && (currentProcessStep != 9))
                currentProcessStep = 1;

            // Find the TEI file
            if (Directory.Exists(userInProcessDirectory))
            {
                string[] tei_files = Directory.GetFiles(userInProcessDirectory, "*.xml");
                if (tei_files.Length > 1)
                {
                    // Two XML files, so delete all but the latest
                    string latest_tei_file = String.Empty;
                    DateTime latest_timestamp = DateTime.MinValue;

                    // Find the latest TEI file
                    foreach (string thisTeiFile in tei_files)
                    {
                        // If this is marc.xml, skip it
                        if ((Path.GetFileName(thisTeiFile).ToLower().IndexOf("marc.xml") >= 0) || (Path.GetFileName(thisTeiFile).ToLower().IndexOf("mets.xml") >= 0))
                            continue;

                        DateTime file_timestamp = File.GetLastWriteTime(thisTeiFile);

                        if (DateTime.Compare(latest_timestamp, file_timestamp) < 0)
                        {
                            latest_tei_file = thisTeiFile;
                            latest_timestamp = file_timestamp;
                        }
                    }

                    // If a latest file as found, delete the others
                    if (!String.IsNullOrEmpty(latest_tei_file))
                    {
                        foreach (string thisTeiFile in tei_files)
                        {
                            // If this is marc.xml, skip it
                            if ((Path.GetFileName(thisTeiFile).ToLower().IndexOf("marc.xml") >= 0) || (Path.GetFileName(thisTeiFile).ToLower().IndexOf("mets.xml") >= 0))
                                continue;

                            // Was this the latest file?
                            if (String.Compare(thisTeiFile, latest_tei_file, StringComparison.OrdinalIgnoreCase) == 0)
                                continue;

                            try
                            {
                                File.Delete(thisTeiFile);
                            }
                            catch
                            {
                            }

                        }
                    }

                    new_tei_file = latest_tei_file;
                }
                else if (tei_files.Length == 1)
                {
                    new_tei_file = Path.GetFileName(tei_files[0]);
                }
            }

            #region Handle any other post back requests

            // If this is post-back, handle it
            if (RequestSpecificValues.Current_Mode.isPostBack)
            {
                // Was this where the mapping, xslt, and css is set?
                if (currentProcessStep == 1)
                {
                    string[] getKeys = HttpContext.Current.Request.Form.AllKeys;
                    foreach (string thisKey in getKeys)
                    {
                        if (thisKey.IndexOf("mapping_select") == 0)
                        {
                            mapping_file = HttpContext.Current.Request.Form[thisKey];
                            HttpContext.Current.Session["Edit_TEI_Item_MySobekViewer." + bibid + "_" + vid + ".Mapping_File"] = mapping_file;
                        }
                        if (thisKey.IndexOf("xslt_select") == 0)
                        {
                            xslt_file = HttpContext.Current.Request.Form[thisKey];
                            HttpContext.Current.Session["Edit_TEI_Item_MySobekViewer." + bibid + "_" + vid + ".XSLT_File"] = xslt_file;
                        }
                        if (thisKey.IndexOf("css_select") == 0)
                        {
                            css_file = HttpContext.Current.Request.Form[thisKey];
                            HttpContext.Current.Session["Edit_TEI_Item_MySobekViewer." + bibid + "_" + vid + ".CSS_File"] = css_file;
                        }
                    }
                }

                // If this is a post back from editing the XML, save this
                if (currentProcessStep == 2)
                {
                    // Get the content of the edited source
                    string new_output = HttpContext.Current.Request.Form["tei_source_content"];

                    // If the user is editing the current TEI file, make a new version under the
                    // user process folder now
                    string save_tei_file = Path.Combine(userInProcessDirectory, original_tei_file);
                    if (!String.IsNullOrEmpty(new_tei_file))
                    {
                        // Determine new filename
                        save_tei_file = Path.Combine(userInProcessDirectory, new_tei_file);
                    }
                    else
                    {
                        new_tei_file = original_tei_file;
                    }

                    // Save new source file
                    try
                    {
                        StreamWriter writer = new StreamWriter(save_tei_file);
                        writer.Write(new_output);
                        writer.Flush();
                        writer.Close();

                        success_message = "Saved TEI changes";
                    }
                    catch (Exception ee)
                    {
                        error_message = "Error saving the new XML source.<br /><br />" + ee.Message + "<br /><br />";

                    }

                }

                string action = HttpContext.Current.Request.Form["action"];
                if (action == "cancel")
                {
                    // Clear all files in the RequestSpecificValues.Current_User process folder
                    try
                    {
                        string[] all_files = Directory.GetFiles(userInProcessDirectory);
                        foreach (string thisFile in all_files)
                            File.Delete(thisFile);
                        Directory.Delete(userInProcessDirectory);
                    }
                    catch (Exception ee)
                    {
                        new_tei_file = ee.Message;
                        // Unable to delete existing file in the RequestSpecificValues.Current_User's folder.
                        // This is an error, but how to report it?
                    }

                    // Clear all the information in memory
                    HttpContext.Current.Session["Edit_TEI_mySobekViewer." + bibid + "_" + vid + ".New_Item"] = null;

                    HttpContext.Current.Session["Edit_TEI_Item_MySobekViewer." + bibid + "_" + vid + ".Mapping_File"] = null;
                    HttpContext.Current.Session["Edit_TEI_Item_MySobekViewer." + bibid + "_" + vid + ".XSLT_File"] = null;
                    HttpContext.Current.Session["Edit_TEI_Item_MySobekViewer." + bibid + "_" + vid + ".CSS_File"] = null;
                    HttpContext.Current.Session["Edit_TEI_Item_MySobekViewer." + bibid + "_" + vid + ".Original_TEI_File"] = null;


                    // Clear any temporarily assigned current project and CompleteTemplate
                    RequestSpecificValues.Current_User.Current_Default_Metadata = null;
                    RequestSpecificValues.Current_User.Current_Template = null;

                    // Forward back to my Sobek home
                    RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                    UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                }

                if (action == "next_phase")
                {
                    string next_phase = HttpContext.Current.Request.Form["phase"];

                    if (currentProcessStep == 1)
                    {
                        // Should be a TEI file to continue
                        if (!String.IsNullOrEmpty(new_tei_file))
                        {
                            XmlValidator validator = new XmlValidator();
                            string tei_filepath = Path.Combine(userInProcessDirectory, new_tei_file);
                            bool isValid = validator.IsValid(tei_filepath);
                            if (!isValid)
                            {
                                string validatorErrors = validator.Errors.Replace("\n", "<br />\n");
                                error_message = "Uploaded TEI file is not a valid XML source file.<br /><br />\n" + validatorErrors;
                                next_phase = "1";
                            }
                        }
                    }

                    // Was this from editing the XMl file?
                    if (currentProcessStep == 2)
                    {
                        // Was there an error?
                        if (!String.IsNullOrEmpty(error_message))
                            next_phase = "2";
                        else if (!String.IsNullOrEmpty(new_tei_file))
                        {
                            XmlValidator validator = new XmlValidator();
                            string tei_filepath = Path.Combine(userInProcessDirectory, new_tei_file);
                            bool isValid = validator.IsValid(tei_filepath);
                            if (!isValid)
                            {
                                string validatorErrors = validator.Errors.Replace("\n", "<br />\n");
                                error_message = "TEI is not valid XML.<br /><br />\n" + validatorErrors;
                                next_phase = "2";
                            }
                        }
                    }


                    // If this is going from a step that includes the metadata entry portion, save this to the item
                    if ((currentProcessStep > 4) && (currentProcessStep < 8))
                    {
                        // Save to the item
                        completeTemplate.Save_To_Bib(editingItem, RequestSpecificValues.Current_User, currentProcessStep - 4);
                        editingItem.Save_METS();
                        HttpContext.Current.Session["Edit_TEI_mySobekViewer." + bibid + "_" + vid + ".New_Item"] = editingItem;
                    }

                    // For now, just forward to the next phase
                    if (currentProcessStep.ToString() != next_phase)
                    {
                        RequestSpecificValues.Current_Mode.My_Sobek_SubMode = next_phase;
                        UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                        return;
                    }
                }
            }

            #endregion

            #region Perform some validation to determine if the RequestSpecificValues.Current_User should be at this step

            // If this is past the step to upload a TEI file, ensure a TEI file exists
            if ((currentProcessStep > 1) && ((String.IsNullOrEmpty(mapping_file)) || (String.IsNullOrEmpty(xslt_file))))
            {
                RequestSpecificValues.Current_Mode.My_Sobek_SubMode = "1";
                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                return;
            }

            // If this is to put up items or complete the item, validate the METS
            if (currentProcessStep >= 8)
            {
                // Validate that a METS file exists
                if (Directory.GetFiles(userInProcessDirectory, "*.mets*").Length == 0)
                {
                    RequestSpecificValues.Current_Mode.My_Sobek_SubMode = "2";
                    UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                    return;
                }

                // Get the validation errors
                validationErrors = new List<string>();
                SobekCM_Item_Validator.Validate_SobekCM_Item(editingItem, validationErrors);

                // Get the validation errors
                if ((validationErrors == null ) || ( validationErrors.Count == 0))
                    editingItem.Save_METS();
                else
                {
                    editingItem.Web.Show_Validation_Errors = true;
                    
                    RequestSpecificValues.Current_Mode.My_Sobek_SubMode = "4";
                    UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                    return;
                }
            }

            // If this is going into the last process step, check that any mandatory info (file, url, .. ) 
            // from the last step is present
            if (currentProcessStep == 9)
            {
                // Complete the item submission
                complete_item_submission(item, RequestSpecificValues.Tracer);
            }

            #endregion
        }

        #endregion

        #region Code to re-scale an image

        /// <summary> Scales an existing SourceImage to a new max width / max height </summary>
        /// <param name="SourceImage"> Source image </param>
        /// <param name="MaxWidth"> Maximum width for the new image </param>
        /// <param name="MaxHeight"> Maximum height for the new image </param>
        /// <returns> Newly scaled image, without changing the original source image </returns>
        public static Image ScaleImage(Image SourceImage, int MaxWidth, int MaxHeight)
        {
            var ratioX = (double)MaxWidth / SourceImage.Width;
            var ratioY = (double)MaxHeight / SourceImage.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(SourceImage.Width * ratio);
            var newHeight = (int)(SourceImage.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);
            Graphics.FromImage(newImage).DrawImage(SourceImage, 0, 0, newWidth, newHeight);
            return newImage;
        }

        #endregion

        #region Method commpletes the item submission on the way to the congratulations screen

        private bool complete_item_submission(SobekCM_Item Item_To_Complete, Custom_Tracer Tracer)
        {
            return false;

            // Set an initial flag 
            criticalErrorEncountered = false;
            bool xml_found = false;

            // This package is good to go, so build it, save, etc...
            try
            {
                // Save the METS file to the database and back to the directory
                Item_To_Complete.Source_Directory = userInProcessDirectory;


                    // Now, add the TEI file
                    SobekCM_File_Info tei_newFile = new SobekCM_File_Info(new_tei_file);
                    string tei_label = new_tei_file + " (TEI)";
                    Item_To_Complete.Divisions.Download_Tree.Add_File(tei_newFile, tei_label);


                // Save to the database
                try
                {
                    SobekCM_Item_Database.Save_New_Digital_Resource(Item_To_Complete, false, true, RequestSpecificValues.Current_User.UserName, String.Empty, RequestSpecificValues.Current_User.UserID);
                }
                catch (Exception ee)
                {
                    StreamWriter writer = new StreamWriter(userInProcessDirectory + "\\exception.txt", false);
                    writer.WriteLine("ERROR CAUGHT WHILE SAVING NEW DIGITAL RESOURCE");
                    writer.WriteLine(DateTime.Now.ToString());
                    writer.WriteLine();
                    writer.WriteLine(ee.Message);
                    writer.WriteLine(ee.StackTrace);
                    writer.Flush();
                    writer.Close();
                    throw;
                }


                // Assign the file root and assoc file path
                Item_To_Complete.Web.File_Root = Item_To_Complete.BibID.Substring(0, 2) + "\\" + Item_To_Complete.BibID.Substring(2, 2) + "\\" + Item_To_Complete.BibID.Substring(4, 2) + "\\" + Item_To_Complete.BibID.Substring(6, 2) + "\\" + Item_To_Complete.BibID.Substring(8, 2);
                Item_To_Complete.Web.AssocFilePath = Item_To_Complete.Web.File_Root + "\\" + Item_To_Complete.VID + "\\";

                // Save the item settings
                SobekCM_Item_Database.Set_Item_Setting_Value(Item_To_Complete.Web.ItemID, "TEI.Source_File", new_tei_file);
                SobekCM_Item_Database.Set_Item_Setting_Value(Item_To_Complete.Web.ItemID, "TEI.CSS", css_file);
                SobekCM_Item_Database.Set_Item_Setting_Value(Item_To_Complete.Web.ItemID, "TEI.Mapping", mapping_file);

                // Find the actual XSLT file
                string xslt_directory = Path.Combine(UI_ApplicationCache_Gateway.Settings.Servers.Application_Server_Network, "plugins", "tei", "xslt");
                string[] xslt_files = Directory.GetFiles(xslt_directory, xslt_file + ".xsl*");
                SobekCM_Item_Database.Set_Item_Setting_Value(Item_To_Complete.Web.ItemID, "TEI.XSLT", Path.GetFileName(xslt_files[0]));

                // Add the TEI viewer
                SobekCM_Item_Database.Save_Item_Add_Viewer(Item_To_Complete.Web.ItemID, "TEI", new_tei_file.Replace(".xml", "").Replace(".XML", "") + " (TEI)", new_tei_file);

                // Create the static html pages
                string base_url = RequestSpecificValues.Current_Mode.Base_URL;
                try
                {
                    Static_Pages_Builder staticBuilder = new Static_Pages_Builder(UI_ApplicationCache_Gateway.Settings.Servers.System_Base_URL, UI_ApplicationCache_Gateway.Settings.Servers.Base_Data_Directory, RequestSpecificValues.HTML_Skin.Skin_Code);
                    string filename = userInProcessDirectory + "\\" + Item_To_Complete.BibID + "_" + Item_To_Complete.VID + ".html";
                    staticBuilder.Create_Item_Citation_HTML(Item_To_Complete, filename, String.Empty);

                    // Copy the static HTML file to the web server
                    try
                    {
                        if (!Directory.Exists(UI_ApplicationCache_Gateway.Settings.Servers.Static_Pages_Location + item.BibID.Substring(0, 2) + "\\" + item.BibID.Substring(2, 2) + "\\" + item.BibID.Substring(4, 2) + "\\" + item.BibID.Substring(6, 2) + "\\" + item.BibID.Substring(8)))
                            Directory.CreateDirectory(UI_ApplicationCache_Gateway.Settings.Servers.Static_Pages_Location + item.BibID.Substring(0, 2) + "\\" + item.BibID.Substring(2, 2) + "\\" + item.BibID.Substring(4, 2) + "\\" + item.BibID.Substring(6, 2) + "\\" + item.BibID.Substring(8));
                        if (File.Exists(userInProcessDirectory + "\\" + item.BibID + "_" + item.VID + ".html"))
                            File.Copy(userInProcessDirectory + "\\" + item.BibID + "_" + item.VID + ".html", UI_ApplicationCache_Gateway.Settings.Servers.Static_Pages_Location + item.BibID.Substring(0, 2) + "\\" + item.BibID.Substring(2, 2) + "\\" + item.BibID.Substring(4, 2) + "\\" + item.BibID.Substring(6, 2) + "\\" + item.BibID.Substring(8) + "\\" + item.BibID + "_" + item.VID + ".html", true);
                    }
                    catch (Exception)
                    {
                        // This is not critical
                    }
                }
                catch (Exception)
                {
                    // An error here is not catastrophic
                }

                RequestSpecificValues.Current_Mode.Base_URL = base_url;

                // Save the rest of the metadata
                Item_To_Complete.Save_SobekCM_METS();

                // Create the options dictionary used when saving information to the database, or writing MarcXML
                Dictionary<string, object> options = new Dictionary<string, object>();
                if (UI_ApplicationCache_Gateway.Settings.MarcGeneration != null)
                {
                    options["MarcXML_File_ReaderWriter:MARC Cataloging Source Code"] = UI_ApplicationCache_Gateway.Settings.MarcGeneration.Cataloging_Source_Code;
                    options["MarcXML_File_ReaderWriter:MARC Location Code"] = UI_ApplicationCache_Gateway.Settings.MarcGeneration.Location_Code;
                    options["MarcXML_File_ReaderWriter:MARC Reproduction Agency"] = UI_ApplicationCache_Gateway.Settings.MarcGeneration.Reproduction_Agency;
                    options["MarcXML_File_ReaderWriter:MARC Reproduction Place"] = UI_ApplicationCache_Gateway.Settings.MarcGeneration.Reproduction_Place;
                    options["MarcXML_File_ReaderWriter:MARC XSLT File"] = UI_ApplicationCache_Gateway.Settings.MarcGeneration.XSLT_File;
                }
                options["MarcXML_File_ReaderWriter:System Name"] = UI_ApplicationCache_Gateway.Settings.System.System_Name;
                options["MarcXML_File_ReaderWriter:System Abbreviation"] = UI_ApplicationCache_Gateway.Settings.System.System_Abbreviation;

                // Save the marc xml file
                MarcXML_File_ReaderWriter marcWriter = new MarcXML_File_ReaderWriter();
                string errorMessage;
                marcWriter.Write_Metadata(Item_To_Complete.Source_Directory + "\\marc.xml", Item_To_Complete, options, out errorMessage);

                // Delete the TEMP mets file
                if (File.Exists(userInProcessDirectory + "\\TEMP000001_00001.mets"))
                    File.Delete(userInProcessDirectory + "\\TEMP000001_00001.mets");

                // Rename the METS file to the XML file                
                if ((!File.Exists(userInProcessDirectory + "\\" + Item_To_Complete.BibID + "_" + Item_To_Complete.VID + ".mets.xml")) &&
                    (File.Exists(userInProcessDirectory + "\\" + Item_To_Complete.BibID + "_" + Item_To_Complete.VID + ".mets")))
                {
                    File.Move(userInProcessDirectory + "\\" + Item_To_Complete.BibID + "_" + Item_To_Complete.VID + ".mets", userInProcessDirectory + "\\" + Item_To_Complete.BibID + "_" + Item_To_Complete.VID + ".mets.xml");
                }

                string serverNetworkFolder = UI_ApplicationCache_Gateway.Settings.Servers.Image_Server_Network + Item_To_Complete.Web.AssocFilePath;

                // Create the folder
                if (!Directory.Exists(serverNetworkFolder))
                    Directory.CreateDirectory(serverNetworkFolder);
                if (!Directory.Exists(serverNetworkFolder + "\\" + UI_ApplicationCache_Gateway.Settings.Resources.Backup_Files_Folder_Name))
                    Directory.CreateDirectory(serverNetworkFolder + "\\" + UI_ApplicationCache_Gateway.Settings.Resources.Backup_Files_Folder_Name);

                // Copy the static HTML page over first
                if (File.Exists(userInProcessDirectory + "\\" + item.BibID + "_" + item.VID + ".html"))
                {
                    File.Copy(userInProcessDirectory + "\\" + item.BibID + "_" + item.VID + ".html", serverNetworkFolder + "\\" + UI_ApplicationCache_Gateway.Settings.Resources.Backup_Files_Folder_Name + "\\" + item.BibID + "_" + item.VID + ".html", true);
                    File.Delete(userInProcessDirectory + "\\" + item.BibID + "_" + item.VID + ".html");
                }

                // Copy all the files
                string[] allFiles = Directory.GetFiles(userInProcessDirectory);
                foreach (string thisFile in allFiles)
                {
                    string destination_file = serverNetworkFolder + "\\" + (new FileInfo(thisFile)).Name;
                    File.Copy(thisFile, destination_file, true);
                }

                // Add this to the cache
                UI_ApplicationCache_Gateway.Items.Add_SobekCM_Item(Item_To_Complete);

                // Now, delete all the files here
                string[] all_files = Directory.GetFiles(userInProcessDirectory);
                foreach (string thisFile in all_files)
                {
                    File.Delete(thisFile);
                }

                // Always set the additional work needed flag, to give the builder a  chance to look at it
                SobekCM_Database.Update_Additional_Work_Needed_Flag(Item_To_Complete.Web.ItemID, true, Tracer);

                // Also clear some values from the session
                HttpContext.Current.Session["Edit_TEI_Item_MySobekViewer." + bibid + "_" + vid + ".Mapping_File"] = null;
                HttpContext.Current.Session["Edit_TEI_Item_MySobekViewer." + bibid + "_" + vid + ".XSLT_File"] = null;
                HttpContext.Current.Session["Edit_TEI_Item_MySobekViewer." + bibid + "_" + vid + ".CSS_File"] = null;
                HttpContext.Current.Session["Edit_TEI_Item_MySobekViewer." + bibid + "_" + vid + ".Original_TEI_File"] = null;
                HttpContext.Current.Session["Edit_TEI_mySobekViewer." + bibid + "_" + vid + ".New_Item"] = null;

            }
            catch (Exception ee)
            {
                validationErrors.Add("Error encountered during item save!");
                validationErrors.Add(ee.ToString().Replace("\r", "<br />"));

                // Set an initial flag 
                criticalErrorEncountered = true;

                string error_body = "<strong>ERROR ENCOUNTERED DURING ONLINE SUBMITTAL PROCESS</strong><br /><br /><blockquote>Title: " + Item_To_Complete.Bib_Info.Main_Title.Title + "<br />Permanent Link: <a href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "/" + Item_To_Complete.BibID + "/" + Item_To_Complete.VID + "\">" + RequestSpecificValues.Current_Mode.Base_URL + "/" + Item_To_Complete.BibID + "/" + Item_To_Complete.VID + "</a><br />User: " + RequestSpecificValues.Current_User.Full_Name + "<br /><br /></blockquote>" + ee.ToString().Replace("\n", "<br />");
                string error_subject = "Error during submission for '" + Item_To_Complete.Bib_Info.Main_Title.Title + "'";
                string email_to = UI_ApplicationCache_Gateway.Settings.Email.System_Error_Email;
                if (email_to.Length == 0)
                    email_to = UI_ApplicationCache_Gateway.Settings.Email.System_Email;
                Email_Helper.SendEmail(email_to, error_subject, error_body, true, RequestSpecificValues.Current_Mode.Instance_Name);
            }

            if (!criticalErrorEncountered)
            {
                // Send email to the email from the CompleteTemplate, if one was provided
                if (completeTemplate.Email_Upon_Receipt.Length > 0)
                {
                    string body = "New item submission complete!<br /><br /><blockquote>Title: " + Item_To_Complete.Bib_Info.Main_Title.Title + "<br />Submittor: " + RequestSpecificValues.Current_User.Full_Name + " ( " + RequestSpecificValues.Current_User.Email + " )<br />Link: <a href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "/" + Item_To_Complete.BibID + "/" + Item_To_Complete.VID + "\">" + Item_To_Complete.BibID + ":" + Item_To_Complete.VID + "</a></blockquote>";
                    string subject = "Item submission complete for '" + Item_To_Complete.Bib_Info.Main_Title.Title + "'";
                    Email_Helper.SendEmail(completeTemplate.Email_Upon_Receipt, subject, body, true, RequestSpecificValues.Current_Mode.Instance_Name);
                }

                // If the RequestSpecificValues.Current_User wants to have a message sent, send one
                if (RequestSpecificValues.Current_User.Send_Email_On_Submission)
                {
                    // Create the mail message
                    string body2 = "<strong>CONGRATULATIONS!</strong><br /><br />Your item has been successfully added to the digital library and will appear immediately.  Search indexes may take a couple minutes to build, at which time this item will be discoverable through the search interface. <br /><br /><blockquote>Title: " + Item_To_Complete.Bib_Info.Main_Title.Title + "<br />Permanent Link: <a href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "/" + Item_To_Complete.BibID + "/" + Item_To_Complete.VID + "\">" + RequestSpecificValues.Current_Mode.Base_URL + "/" + Item_To_Complete.BibID + "/" + Item_To_Complete.VID + "</a></blockquote>";
                    string subject2 = "Item submission complete for '" + Item_To_Complete.Bib_Info.Main_Title.Title + "'";
                    Email_Helper.SendEmail(RequestSpecificValues.Current_User.Email, subject2, body2, true, RequestSpecificValues.Current_Mode.Instance_Name);
                }

                // Also clear any searches or browses ( in the future could refine this to only remove those
                // that are impacted by this save... but this is good enough for now )
                CachedDataManager.Clear_Search_Results_Browses();
            }

            return criticalErrorEncountered;
        }

        #endregion

        /// <summary> Gets the banner from the current CompleteTemplate, if there is one </summary>
        public string Current_Template_Banner
        {
            get
            {
                return completeTemplate != null ? completeTemplate.Banner : String.Empty;
            }
        }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <value> This returns the title of the current <see cref="CompleteTemplate"/> object </value>
        public override string Web_Title
        {
            get
            {
                return toolTitle;
            }
        }

        #region Methods to write the HTML directly to the output stream

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This adds the CompleteTemplate HTML for step 2 and the congratulations text for step 4 </remarks>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Edit_TEI_Item_MySobekViewer.Write_HTML", "Do nothing");

            Output.WriteLine("<script src=\"" + Static_Resources_Gateway.Sobekcm_Metadata_Js + "\" type=\"text/javascript\"></script>");

            // Write the top currentItem mimic html portion
            Write_Item_Type_Top(Output, editingItem);

            if ( currentProcessStep == 2 )
                Output.WriteLine("<div id=\"container-innerfull\">");
            else
                Output.WriteLine("<div id=\"container-inner1000\">");

            Output.WriteLine("<div id=\"pagecontainer\">");

            Output.WriteLine("<div class=\"sbkMySobek_HomeText\">");
            Output.WriteLine("  <br />");

            if (currentProcessStep == 1)
            {
                Output.WriteLine("<script src=\"" + Static_Resources_Gateway.Sobekcm_Metadata_Js + "\" type=\"text/javascript\"></script>");
                Output.WriteLine("<div class=\"sbkMySobek_HomeText\">");
                Output.Write("<h2>Step 1 of " + totalTemplatePages + ": Upload TEI and Confirm Mapping, XSLT, and CSS </h2>");

                // Was there a basic XML validation error?
                if (!String.IsNullOrEmpty(error_message))
                {
                    Output.WriteLine("<div style=\"padding-left:30px;font-weight:bold; color:Red; font-size:1.1em; padding-bottom: 20px;\">");
                    Output.WriteLine(error_message);
                    Output.WriteLine("</div>");
                }


                Output.WriteLine("<table class=\"sbkMySobek_TemplateTbl\" cellpadding=\"4px\" >");
                Output.WriteLine("  <tr>");
                Output.WriteLine("    <td colspan=\"3\" class=\"sbkMySobek_TemplateTblTitle_first\">New TEI File</td>");
                Output.WriteLine("  </tr>");
                Output.WriteLine("  <tr>");

                string explanation = "You can upload a new TEI file to replace the current TEI file.  Use the button below to upload a new TEI file.";
                if (!String.IsNullOrEmpty(new_tei_file))
                {
                    string tei_filename = Path.GetFileName(new_tei_file);
                    explanation = "You will replace the current TEI file with the newly uploaded file <i>" + tei_filename + "</i>.  You can change the file to upload by pressing the button below.";
                }
                Output.WriteLine("    <td colspan=\"3\" style=\"padding-left:30px;font-style:italic; color:#333; font-size:0.9em;\">" + explanation + "</td>");
                Output.WriteLine("  </tr>");
                Output.WriteLine("  <tr>");
                Output.WriteLine("    <td colspan=\"3\">");
            }

 

        }

        /// <summary> This is an opportunity to write HTML directly into the main form, without
        /// using the pop-up html form architecture </summary>
        /// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This text will appear within the ItemNavForm form tags </remarks>
        public override void Write_ItemNavForm_Closing(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Edit_TEI_Item_MySobekViewer.Write_ItemNavForm_Closing", "");
            }

            // Add the hidden fields first
            Output.WriteLine("<!-- Hidden field is used for postbacks to indicate what to save and reset -->");
            Output.WriteLine("<input type=\"hidden\" id=\"action\" name=\"action\" value=\"\" />");
            Output.WriteLine("<input type=\"hidden\" id=\"phase\" name=\"phase\" value=\"\" />");
            Output.WriteLine("<script type=\"text/javascript\" src=\"" + Static_Resources_Gateway.Sobekcm_Metadata_Js + "\" ></script>");

            #region Add the HTML to select the mapping, css, and xslt

            if (currentProcessStep == 1)
            {
                Output.WriteLine("      <br />");
                Output.WriteLine("    </td>");
                Output.WriteLine("  </tr>");
                Output.WriteLine("  <tr>");
                Output.WriteLine("    <td colspan=\"3\" class=\"sbkMySobek_TemplateTblTitle_first\">Metadata Mapping</td>");
                Output.WriteLine("  </tr>");

                // Get the list of Mapping files that exist and this user is enabled for
                List<string> mapping_files = new List<string>();
                foreach (string thisSettingKey in RequestSpecificValues.Current_User.Settings.Keys)
                {
                    if (thisSettingKey.IndexOf("TEI.MAPPING.") == 0)
                    {
                        // Only show enabled options
                        string enabled = RequestSpecificValues.Current_User.Get_Setting(thisSettingKey, "false");
                        if (String.Compare(enabled, "true", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            // Get this file name
                            string file = thisSettingKey.Replace("TEI.MAPPING.", "");

                            // Also verify this mapping file exists
                            string filepath = Path.Combine(UI_ApplicationCache_Gateway.Settings.Servers.Application_Server_Network, "plugins", "tei", "mapping", file + ".xml");
                            if (!File.Exists(filepath))
                                continue;

                            // Since this exists, add to the css file list
                            mapping_files.Add(file);
                        }
                    }
                }

                // Show an error message if no mapping file exists
                if (mapping_files.Count == 0)
                {
                    Output.WriteLine("  <tr>");
                    Output.WriteLine("    <td colspan=\"3\" style=\"padding-left:30px;font-weight:bold; color:Red; font-size:1.1em;\">You are not approved for any TEI mapping file.  Please let your system administrator know so they can approve you for an existing TEI mapping file.</td>");
                    Output.WriteLine("  </tr>");
                }
                else
                {

                    Output.WriteLine("  <tr>");
                    if (mapping_files.Count == 1)
                        Output.WriteLine("    <td colspan=\"3\" style=\"padding-left:30px;font-style:italic; color:#333; font-size:0.9em;\">The mapping file below will read the header information from your TEI file into the system, to facilitate searching and discovery of this resource.</td>");
                    else
                        Output.WriteLine("    <td colspan=\"3\" style=\"padding-left:30px;font-style:italic; color:#333; font-size:0.9em;\">Select the metadata mapping file below.  This mapping file will read the header information from your TEI file into the system, to facilitate searching and discovery of this resource.</td>");
                    Output.WriteLine("  </tr>");

                    Output.WriteLine("  <tr>");
                    Output.WriteLine("    <td style=\"width:15px\" > &nbsp;</td>");
                    Output.WriteLine("    <td class=\"metadata_label\">Metadata Mapping:</a></td>");


                    // If they are approved for only one mapping file, just show that one as text, not a select box
                    if (mapping_files.Count == 1)
                    {
                        Output.WriteLine("    <td>");
                        Output.WriteLine("      " + mapping_files[0]);
                        Output.WriteLine("      <input type=\"hidden\" id=\"mapping_select\" name=\"mapping_select\" value=\"" + mapping_files[0] + "\" />");
                        Output.WriteLine("    </td>");
                    }
                    else
                    {

                        Output.WriteLine("    <td>");
                        Output.WriteLine("      <table>");
                        Output.WriteLine("        <tr>");
                        Output.WriteLine("          <td>");
                        Output.WriteLine("            <div id=\"mapping_div\">");
                        Output.WriteLine("              <select class=\"type_select\" name=\"mapping_select\" id=\"mapping_select\" >");

                        foreach (string file in mapping_files)
                        {
                            // Add this mapping information
                            if (String.Compare(file, mapping_file, StringComparison.OrdinalIgnoreCase) == 0)
                                Output.WriteLine("              <option value=\"" + file + "\" selected=\"selected\">" + file + "</option>");
                            else
                                Output.WriteLine("              <option value=\"" + file + "\">" + file + "</option>");
                        }

                        Output.WriteLine("              </select>");
                        Output.WriteLine("            </div>");
                        Output.WriteLine("          </td>");

                        //Output.WriteLine("          <td style=\"vertical-align:bottom\">");
                        //Output.WriteLine("            <a target=\"_TYPE\"  title=\"Get help.\" href=\"http://sobekrepository.org/help/typesimple\"><img class=\"help_button\" src=\"http://cdn.sobekrepository.org/images/misc/help_button.jpg\" /></a>");
                        //Output.WriteLine("          </td>");

                        Output.WriteLine("        </tr>");
                        Output.WriteLine("      </table>");
                        Output.WriteLine("    </td>");
                    }
                    Output.WriteLine("  </tr>");
                }

                Output.WriteLine("  <tr>");
                Output.WriteLine("    <td colspan=\"3\" class=\"sbkMySobek_TemplateTblTitle\" style=\"padding-top:25px\">Display Parameters (XSLT and CSS)</td>");
                Output.WriteLine("  </tr>");

                // Get the list of XSLT files that exist and this user is enabled for
                List<string> xslt_files = new List<string>();
                foreach (string thisSettingKey in RequestSpecificValues.Current_User.Settings.Keys)
                {
                    if (thisSettingKey.IndexOf("TEI.XSLT.") == 0)
                    {
                        // Only show enabled options
                        string enabled = RequestSpecificValues.Current_User.Get_Setting(thisSettingKey, "false");
                        if (String.Compare(enabled, "true", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            // Get this file name
                            string file = thisSettingKey.Replace("TEI.XSLT.", "");

                            // Also verify this mapping file exists
                            string filepath = Path.Combine(UI_ApplicationCache_Gateway.Settings.Servers.Application_Server_Network, "plugins", "tei", "xslt", file );
                            if ((!File.Exists(filepath + ".xslt")) && ( !File.Exists(filepath + ".xsl")))
                                continue;

                            // Since this exists, add to the xslt file list
                            xslt_files.Add(file);
                        }
                    }
                }

                // Show an error message if no XSLT file exists
                if (xslt_files.Count == 0)
                {
                    Output.WriteLine("  <tr>");
                    Output.WriteLine("    <td colspan=\"3\" style=\"padding-left:30px;font-weight:bold; color:Red; font-size:1.1em;\">You are not approved for any TEI XSLT file.  Please let your system administrator know so they can approve you for an existing TEI XSLT file.</td>");
                    Output.WriteLine("  </tr>");
                }
                else
                {
                    Output.WriteLine("  <tr>");
                    Output.WriteLine("    <td colspan=\"3\" style=\"padding-left:30px;font-style:italic; color:#333; font-size:0.9em;\">The values below determine how the TEI will display within this system.  The XSLT will transform your TEI into HTML for display and the CSS file can add additional style to the resulting display.</td>");
                    Output.WriteLine("  </tr>");

                    Output.WriteLine("  <tr>");
                    Output.WriteLine("    <td style=\"width:15px\" > &nbsp;</td>");
                    Output.WriteLine("    <td class=\"metadata_label\">XSLT File:</a></td>");

                    // If they are approved for only one XSLT file, just show that one as text, not a select box
                    if (xslt_files.Count == 1)
                    {
                        Output.WriteLine("    <td>");
                        Output.WriteLine("      " + xslt_files[0]);
                        Output.WriteLine("      <input type=\"hidden\" id=\"xslt_select\" name=\"xslt_select\" value=\"" + xslt_files[0] + "\" />");
                        Output.WriteLine("    </td>");
                    }
                    else
                    {

                        Output.WriteLine("    <td>");
                        Output.WriteLine("      <table>");
                        Output.WriteLine("        <tr>");
                        Output.WriteLine("          <td>");
                        Output.WriteLine("            <div id=\"xslt_div\">");
                        Output.WriteLine("              <select class=\"type_select\" name=\"xslt_select\" id=\"xslt_select\" >");

                        foreach (string file in xslt_files)
                        {

                            // Add this XSLT option
                            if (String.Compare(file, xslt_file, StringComparison.OrdinalIgnoreCase) == 0)
                                Output.WriteLine("              <option value=\"" + file + "\" selected=\"selected\">" + file + "</option>");
                            else
                                Output.WriteLine("              <option value=\"" + file + "\">" + file + "</option>");
                        }

                        Output.WriteLine("              </select>");
                        Output.WriteLine("            </div>");
                        Output.WriteLine("          </td>");

                        //Output.WriteLine("          <td style=\"vertical-align:bottom\">");
                        //Output.WriteLine("            <a target=\"_TYPE\"  title=\"Get help.\" href=\"http://sobekrepository.org/help/typesimple\"><img class=\"help_button\" src=\"http://cdn.sobekrepository.org/images/misc/help_button.jpg\" /></a>");
                        //Output.WriteLine("          </td>");

                        Output.WriteLine("        </tr>");
                        Output.WriteLine("      </table>");
                        Output.WriteLine("    </td>");
                    }
                    Output.WriteLine("  </tr>");
                }

                // CSS is not required, so check to see if any enable CSS's exist
                List<string> css_files = new List<string>();
                foreach (string thisSettingKey in RequestSpecificValues.Current_User.Settings.Keys)
                {
                    if (thisSettingKey.IndexOf("TEI.CSS.") == 0)
                    {
                        // Only show enabled options
                        string enabled = RequestSpecificValues.Current_User.Get_Setting(thisSettingKey, "false");
                        if (String.Compare(enabled, "true", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            // Get this file name
                            string file = thisSettingKey.Replace("TEI.CSS.", "");

                            // Also verify this mapping file exists
                            string filepath = Path.Combine(UI_ApplicationCache_Gateway.Settings.Servers.Application_Server_Network, "plugins", "tei", "css", file + ".css");
                            if (!File.Exists(filepath))
                                continue;

                            // Since this exists, add to the css file list
                            css_files.Add(file);
                        }
                    }
                }

                // Only show the CSS options, if there are CSS options
                if (css_files.Count > 0)
                {
                    Output.WriteLine("  <tr>");
                    Output.WriteLine("    <td style=\"width:15px\" > &nbsp;</td>");
                    Output.WriteLine("    <td class=\"metadata_label\">CSS File:</a></td>");

                    Output.WriteLine("    <td>");
                    Output.WriteLine("      <table>");
                    Output.WriteLine("        <tr>");
                    Output.WriteLine("          <td>");
                    Output.WriteLine("            <div id=\"css_div\">");
                    Output.WriteLine("              <select class=\"type_select\" name=\"css_select\" id=\"css_select\" >");
                    Output.WriteLine("                <option value=\"\">(none)</option>");
                    foreach (string file in css_files)
                    {
                        if (String.Compare(file, css_file, StringComparison.OrdinalIgnoreCase) == 0)
                            Output.WriteLine("                <option value=\"" + file + "\" selected=\"selected\">" + file + "</option>");
                        else
                            Output.WriteLine("                <option value=\"" + file + "\">" + file + "</option>");
                    }

                    Output.WriteLine("              </select>");
                    Output.WriteLine("            </div>");
                    Output.WriteLine("          </td>");

                    //Output.WriteLine("          <td style=\"vertical-align:bottom\">");
                    //Output.WriteLine("            <a target=\"_TYPE\"  title=\"Get help.\" href=\"http://sobekrepository.org/help/typesimple\"><img class=\"help_button\" src=\"http://cdn.sobekrepository.org/images/misc/help_button.jpg\" /></a>");
                    //Output.WriteLine("          </td>");

                    Output.WriteLine("        </tr>");
                    Output.WriteLine("      </table>");
                    Output.WriteLine("    </td>");


                    Output.WriteLine("  </tr>");
                }

                Output.WriteLine("</table>");


                // Add the bottom buttons
                Output.WriteLine("      <div class=\"sbkMySobek_RightButtons\">");
                Output.WriteLine("        <button onclick=\"return new_item_cancel();\" class=\"sbkMySobek_BigButton\"><img src=\"" + Static_Resources_Gateway.Button_Previous_Arrow_Png + "\" class=\"sbkMySobek_RoundButton_LeftImg\" alt=\"\" /> CANCEL </button> &nbsp; &nbsp; ");
                Output.WriteLine("        <button onclick=\"return new_item_next_phase(2);\" class=\"sbkMySobek_BigButton\"> NEXT <img src=\"" + Static_Resources_Gateway.Button_Next_Arrow_Png + "\" class=\"sbkMySobek_RoundButton_RightImg\" alt=\"\" /></button>");
                Output.WriteLine("      </div>");

                Output.WriteLine("<br /><br /><br />");

                Output.WriteLine("</div>");
            }

            #endregion

            #region Add the HTML to allow the source of the TEI file to be edited

            // Add code to edit the TEI 
            if (currentProcessStep == 2)
            {
                Output.WriteLine("<script src=\"" + Static_Resources_Gateway.Sobekcm_Metadata_Js + "\" type=\"text/javascript\"></script>");
                Output.WriteLine("<h2>Step 2 of " + totalTemplatePages + ": Edit TEI</h2>");

                // Was there a basic XML validation error?
                if (!String.IsNullOrEmpty(error_message))
                {
                    Output.WriteLine("<div style=\"padding-left:30px;font-weight:bold; color:Red; font-size:1.1em; padding-bottom: 20px;\">");
                    Output.WriteLine(error_message);
                    Output.WriteLine("</div>");
                }
                else if (!String.IsNullOrEmpty(success_message))
                {
                    Output.WriteLine("<div style=\"padding-left:30px;font-weight:bold; color:Blue; font-size:1.1em; padding-bottom: 20px;\">");
                    Output.WriteLine(success_message);
                    Output.WriteLine("</div>");
                }

                Output.WriteLine("</div>");

                // Find the TEI source file (may be newly uploaded file)
                string current_tei_file = SobekFileSystem.Resource_Network_Uri(bibid, vid, original_tei_file);
                if (!String.IsNullOrEmpty(new_tei_file))
                {
                    current_tei_file = Path.Combine(userInProcessDirectory, new_tei_file);
                }

                // Get the TEI source 
                string tei_source_content = null;
                try
                {
                    tei_source_content = File.ReadAllText(current_tei_file);
                }
                catch (Exception)
                {
                    tei_source_content = "ERROR READING TEI SOURCE.  ( " + current_tei_file + " )";
                    throw;
                }

                Output.WriteLine("<input type=\"hidden\" id=\"tei_source_content\" name=\"tei_source_content\" value=\"\" />");

                Output.WriteLine("<div id=\"sbkEtmv_TeiEditorDiv\">");
                Output.WriteLine("<pre id=\"sbkEtmv_TeiEditor\">");
                Output.WriteLine(HttpUtility.HtmlEncode(tei_source_content));
                Output.WriteLine("</pre>  ");
                Output.WriteLine("</div>");

                Output.WriteLine("<script src=\"http://localhost:52468/default/includes/ace/1.2.5/ace.js\" type=\"text/javascript\" charset=\"utf-8\"></script>  ");
                Output.WriteLine("<script>  ");
                Output.WriteLine("    var editor = ace.edit(\"sbkEtmv_TeiEditor\");  ");
                Output.WriteLine("    editor.setTheme(\"ace/theme/chrome\");  ");
                Output.WriteLine("    editor.session.setMode(\"ace/mode/xml\");  ");
                Output.WriteLine("    editor.setOptions({ enableBasicAutocompletion: true, enableSnippets: true, enableLiveAutocompletion: false });");
                Output.WriteLine("</script>  ");

                // Add the bottom buttons
                Output.WriteLine("<div class=\"sbkMySobek_HomeText\">");
                Output.WriteLine("      <div class=\"sbkMySobek_RightButtons\">");
                Output.WriteLine("        <button onclick=\"return new_item_cancel();\" class=\"sbkMySobek_BigButton\"><img src=\"" + Static_Resources_Gateway.Button_Previous_Arrow_Png + "\" class=\"sbkMySobek_RoundButton_LeftImg\" alt=\"\" /> CANCEL </button> &nbsp; &nbsp; ");
                Output.WriteLine("        <button onclick=\"return new_item_next_phase_ace_editor(2);\" class=\"sbkMySobek_BigButton\"> SAVE </button> &nbsp; &nbsp; ");
                Output.WriteLine("        <button onclick=\"return new_item_next_phase_ace_editor(3);\" class=\"sbkMySobek_BigButton\"> NEXT <img src=\"" + Static_Resources_Gateway.Button_Next_Arrow_Png + "\" class=\"sbkMySobek_RoundButton_RightImg\" alt=\"\" /></button>");
                Output.WriteLine("      </div>");

                Output.WriteLine("<br /><br /><br />");

                Output.WriteLine("</div>");
            }

            #endregion

            #region Convert TEI to METS and add the metadata preview for this item

            if (currentProcessStep == 3)
            {
                // Get the mapping file
                string complete_mapping_file = Path.Combine(UI.UI_ApplicationCache_Gateway.Settings.Servers.Application_Server_Network, "plugins\\tei\\mapping", mapping_file + ".xml");
                string complete_tei_file = original_tei_file;
                if (!String.IsNullOrEmpty(new_tei_file))
                    complete_tei_file = Path.Combine(userInProcessDirectory, new_tei_file);
                bool error = false;

                try
                {
                    // Save some data
                    string holding_code = editingItem.Bib_Info.Location.Holding_Code;
                    string source_code = editingItem.Bib_Info.Source.Code;
                    TypeOfResource_SobekCM_Enum original_type = editingItem.Bib_Info.SobekCM_Type;
                    string original_title = editingItem.Bib_Info.Main_Title.Title;

                    // Clear the existing bibliographic information
                    editingItem.Bib_Info.Clear();

                    // Use the mapper and pull the results
                    GenericXmlReader testMapper = new GenericXmlReader();
                    GenericXmlReaderResults returnValue = testMapper.ProcessFile(complete_tei_file, complete_mapping_file);

                    // Was there an error converting using the selected mapping?
                    if ((returnValue == null) || (!String.IsNullOrEmpty(returnValue.ErrorMessage)))
                    {
                        error = true;
                        if (returnValue != null)
                            error_message = "Error mapping the TEI XML file into the SobekCM item.<br /><br />" + returnValue.ErrorMessage + "<br /><br />Try a different mapping or contact your system administrator.<br /><br />";
                        else
                            error_message = "Error mapping the TEI XML file into the SobekCM item.<br /><br />Try a different mapping or contact your system administrator.<br /><br />";
                    }
                    else
                    {
                        // Create the mapper to map these values into the SobekCM object
                        Standard_Bibliographic_Mapper mappingObject = new Standard_Bibliographic_Mapper();

                        // Add all this information
                        foreach (MappedValue mappedValue in returnValue.MappedValues)
                        {
                            // If NONE mapping, just go on
                            if ((String.IsNullOrEmpty(mappedValue.Mapping)) || (String.Compare(mappedValue.Mapping, "None", StringComparison.OrdinalIgnoreCase) == 0))
                                continue;

                            if (!String.IsNullOrEmpty(mappedValue.Value))
                            {
                                // One mappig that is NOT bibliographic in nature is the full text
                                if ((String.Compare(mappedValue.Mapping, "FullText", StringComparison.OrdinalIgnoreCase) == 0) ||
                                    (String.Compare(mappedValue.Mapping, "Text", StringComparison.OrdinalIgnoreCase) == 0) ||
                                    (String.Compare(mappedValue.Mapping, "Full Text", StringComparison.OrdinalIgnoreCase) == 0))
                                {
                                    // Ensure no other TEXT file exists here ( in case a different file was uploaded )
                                    try
                                    {
                                        string text_file = Path.Combine(userInProcessDirectory, "fulltext.txt");
                                        StreamWriter writer = new StreamWriter(text_file);
                                        writer.Write(mappedValue.Value);
                                        writer.Flush();
                                        writer.Close();
                                    }
                                    catch
                                    {

                                    }
                                }
                                else
                                {
                                    mappingObject.Add_Data(editingItem, mappedValue.Value, mappedValue.Mapping);
                                }
                            }
                        }

                        // If there is no title, assign a default
                        if (String.IsNullOrEmpty(editingItem.Bib_Info.Main_Title.Title))
                            editingItem.Bib_Info.Main_Title.Title = "TEI Item";

                        // Ensure source and holding codes remain
                        if (String.IsNullOrEmpty(editingItem.Bib_Info.Location.Holding_Code)) editingItem.Bib_Info.Location.Holding_Code = holding_code;
                        if (String.IsNullOrEmpty(editingItem.Bib_Info.Source.Code)) editingItem.Bib_Info.Source.Code = source_code;
                        if ( editingItem.Bib_Info.SobekCM_Type == TypeOfResource_SobekCM_Enum.UNKNOWN )
                            editingItem.Bib_Info.SobekCM_Type = original_type;
                        if (String.IsNullOrEmpty(editingItem.Bib_Info.Main_Title.Title))
                            editingItem.Bib_Info.Main_Title.Title = original_title;

                        editingItem.Save_METS();
                        HttpContext.Current.Session["Edit_TEI_mySobekViewer." + bibid + "_" + vid + ".New_Item"] = editingItem;

                    }

                }
                catch (Exception ee)
                {
                    error_message = ee.Message;
                }

                // Save this as the editing item
                HttpContext.Current.Session["Edit_TEI_mySobekViewer." + bibid + "_" + vid + ".New_Item"] = editingItem;


                Output.WriteLine("<div class=\"sbkMySobek_HomeText\" >");
                Output.WriteLine("<br />");

                Output.WriteLine("<h2>Step 3 of " + totalTemplatePages + ": Metadata Preview</h2>");

                // Was there a basic XML validation error?
                if ((error) && (!String.IsNullOrEmpty(error_message)))
                {
                    Output.WriteLine("<div style=\"padding-left:30px;font-weight:bold; color:Red; font-size:1.1em;\">");
                    Output.WriteLine(error_message);
                    Output.WriteLine("</div>");
                }

                Output.WriteLine("<blockquote>Below is a preview of the metadata extracted from your TEI file.<br /><br />");


                string citation = Standard_Citation_String(false, Tracer);
                Output.WriteLine(citation);

                // Add the bottom buttons
                Output.WriteLine("      <div class=\"sbkMySobek_RightButtons\">");
                Output.WriteLine("        <button onclick=\"return new_item_next_phase(2);\" class=\"sbkMySobek_BigButton\"><img src=\"" + Static_Resources_Gateway.Button_Previous_Arrow_Png + "\" class=\"sbkMySobek_RoundButton_LeftImg\" alt=\"\" /> BACK </button> &nbsp; &nbsp; ");

                if (!error)
                    Output.WriteLine("        <button onclick=\"return new_item_next_phase(4);\" class=\"sbkMySobek_BigButton\"> NEXT <img src=\"" + Static_Resources_Gateway.Button_Next_Arrow_Png + "\" class=\"sbkMySobek_RoundButton_RightImg\" alt=\"\" /></button>");

                Output.WriteLine("      </div>");

                Output.WriteLine("</blockquote><br />");

                Output.WriteLine("<br />");
                Output.WriteLine("</div>");
            }

            #endregion

            #region Add the CompleteTemplate and surrounding HTML for the CompleteTemplate page step(s)

            if ((currentProcessStep >= 4) && (currentProcessStep <= (completeTemplate.InputPages_Count + 3)))
            {
                Output.WriteLine("<script type=\"text/javascript\" src=\"" + Static_Resources_Gateway.Jquery_Ui_1_10_3_Custom_Js + "\"></script>");

                Output.WriteLine("<div class=\"sbkMySobek_HomeText\">");
                Output.WriteLine("<br />");
                string template_page_title = completeTemplate.InputPages[currentProcessStep - 4].Title;
                if (template_page_title.Length == 0)
                    template_page_title = "Additional Item Description";
                string template_page_instructions = completeTemplate.InputPages[currentProcessStep - 4].Instructions;
                if (template_page_instructions.Length == 0)
                    template_page_instructions = "Enter additional basic information for your item.";

                // Get the adjusted process step number ( for skipping permissions, usual step1 )
                int adjusted_process_step = currentProcessStep;
                if (completeTemplate.Permissions_Agreement.Length == 0)
                    adjusted_process_step--;

                Output.WriteLine("<h2>Step " + adjusted_process_step + " of " + totalTemplatePages + ": " + template_page_title + "</h2>");
                Output.WriteLine("<blockquote>" + template_page_instructions + "</blockquote>");
                if ((validationErrors != null) && (validationErrors.Count > 0) && (item.Web.Show_Validation_Errors))
                {
                    Output.WriteLine("<span style=\"color: red;\"><b>The following errors were detected:</b>");
                    Output.WriteLine("<blockquote>");
                    foreach (string validation_error in validationErrors)
                    {
                        Output.WriteLine(validation_error + "<br />");
                    }
                    Output.WriteLine("</blockquote>");
                    Output.WriteLine("</span>");
                    Output.WriteLine("<br />");
                    Output.WriteLine();
                }

                int next_step = currentProcessStep + 1;
                if (currentProcessStep == completeTemplate.InputPages_Count + 3)
                {
                    next_step = 9;
                }
                Output.WriteLine("<div id=\"tabContainer\" class=\"fulltabs\">");
                Output.WriteLine("  <div class=\"graytabscontent\">");
                Output.WriteLine("    <div class=\"tabpage\" id=\"tabpage_1\">");

                // Add the top buttons
                Output.WriteLine("      <div class=\"sbkMySobek_RightButtons\">");
                Output.WriteLine("        <button onclick=\"return new_item_next_phase(" + (currentProcessStep - 1) + ");\" class=\"sbkMySobek_BigButton\"><img src=\"" + Static_Resources_Gateway.Button_Previous_Arrow_Png + "\" class=\"sbkMySobek_RoundButton_LeftImg\" alt=\"\" /> BACK </button> &nbsp; &nbsp; ");
                Output.WriteLine("        <button onclick=\"return new_item_next_phase(" + next_step + ");\" class=\"sbkMySobek_BigButton\"> NEXT <img src=\"" + Static_Resources_Gateway.Button_Next_Arrow_Png + "\" class=\"sbkMySobek_RoundButton_RightImg\" alt=\"\" /></button>");
                Output.WriteLine("      </div>");
                Output.WriteLine("      <br /><br />");
                Output.WriteLine();

                bool isMozilla = ((!String.IsNullOrEmpty(RequestSpecificValues.Current_Mode.Browser_Type)) && (RequestSpecificValues.Current_Mode.Browser_Type.ToUpper().IndexOf("FIREFOX") >= 0));

                string popup_forms = completeTemplate.Render_Template_HTML(Output, editingItem, RequestSpecificValues.Current_Mode.Skin, isMozilla, RequestSpecificValues.Current_User, RequestSpecificValues.Current_Mode.Language, UI_ApplicationCache_Gateway.Translation, RequestSpecificValues.Current_Mode.Base_URL, currentProcessStep - 3);


                // Add the bottom buttons
                Output.WriteLine("      <div class=\"sbkMySobek_RightButtons\">");
                Output.WriteLine("        <button onclick=\"return new_item_next_phase(" + (currentProcessStep - 1) + ");\" class=\"sbkMySobek_BigButton\"><img src=\"" + Static_Resources_Gateway.Button_Previous_Arrow_Png + "\" class=\"sbkMySobek_RoundButton_LeftImg\" alt=\"\" /> BACK </button> &nbsp; &nbsp; ");
                Output.WriteLine("        <button onclick=\"return new_item_next_phase(" + next_step + ");\" class=\"sbkMySobek_BigButton\"> NEXT <img src=\"" + Static_Resources_Gateway.Button_Next_Arrow_Png + "\" class=\"sbkMySobek_RoundButton_RightImg\" alt=\"\" /></button>");
                Output.WriteLine("      </div>");
                Output.WriteLine("      <br />");
                Output.WriteLine();

                Output.WriteLine("    </div>");
                Output.WriteLine("  </div>");
                Output.WriteLine("</div>");
                Output.WriteLine("<br /><br />");


                if (popup_forms.Length > 0)
                    Output.WriteLine(popup_forms);

                Output.WriteLine("</div>");
            }

            #endregion

            Output.WriteLine("</div>");
            Output.WriteLine("</div>");
            Output.WriteLine("</div>");

        }

        #endregion

        /// <summary> Add controls directly to the form in the main control area placeholder </summary>
        /// <param name="MainPlaceHolder"> Main place holder to which all main controls are added </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Add_Controls(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Edit_TEI_Item_MySobekViewer.Add_Controls", String.Empty);

            // Do nothing if this is the very last step
            if (currentProcessStep == 1)
            {
                // Add the upload controls to the file place holder
                add_upload_controls_tei(MainPlaceHolder, "", ".xml", Tracer);
            }
        }

        #region Step 1: Upload TEI uploadify code

        private void add_upload_controls_tei(PlaceHolder MainPlaceholder, string Prompt, string AllowedFileExtensions, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Edit_TEI_Item_MySobekViewer.add_upload_controls", String.Empty);

            StringBuilder filesBuilder = new StringBuilder(2000);
            filesBuilder.AppendLine("<script src=\"" + Static_Resources_Gateway.Sobekcm_Metadata_Js + "\" type=\"text/javascript\"></script>");

            if (currentProcessStep == 1) 
            {
                filesBuilder.AppendLine(Prompt);
                filesBuilder.AppendLine("<blockquote>");

                LiteralControl filesLiteral2 = new LiteralControl(filesBuilder.ToString());
                MainPlaceholder.Controls.Add(filesLiteral2);
                filesBuilder.Remove(0, filesBuilder.Length);


                UploadiFiveControl uploadControl = new UploadiFiveControl();
                uploadControl.UploadPath = userInProcessDirectory;
                uploadControl.UploadScript = RequestSpecificValues.Current_Mode.Base_URL + "UploadiFiveFileHandler.ashx";
                uploadControl.AllowedFileExtensions = ".xml";
                uploadControl.SubmitWhenQueueCompletes = true;
                uploadControl.RemoveCompleted = true;
                uploadControl.Swf = Static_Resources_Gateway.Uploadify_Swf;
                uploadControl.RevertToFlashVersion = true;
                uploadControl.QueueSizeLimit = 1;
                uploadControl.ButtonText = "Select TEI File";
                uploadControl.ButtonWidth = 175;
                MainPlaceholder.Controls.Add(uploadControl);

                filesBuilder.AppendLine("</blockquote><br />");
            }

            LiteralControl literal1 = new LiteralControl(filesBuilder.ToString());
            MainPlaceholder.Controls.Add(literal1);
        }

        #endregion

        #region Code to create the regular citation string

        /// <summary> Returns the basic information about this digital resource in standard format </summary>
        /// <param name="Include_Links"> Flag tells whether to include the search links from this citation view </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> HTML string with the basic information about this digital resource for display </returns>
        public string Standard_Citation_String(bool Include_Links, Custom_Tracer Tracer)
        {
            Navigation_Object CurrentRequest = RequestSpecificValues.Current_Mode;

            // Compute the URL to use for all searches from the citation
            Display_Mode_Enum lastMode = CurrentRequest.Mode;
            CurrentRequest.Mode = Display_Mode_Enum.Results;
            CurrentRequest.Search_Type = Search_Type_Enum.Advanced;
            CurrentRequest.Search_String = "<%VALUE%>";
            CurrentRequest.Search_Fields = "<%CODE%>";
            string search_link = "<a href=\"" + UrlWriterHelper.Redirect_URL(CurrentRequest).Replace("&", "&amp;").Replace("%3c%25", "<%").Replace("%25%3e", "%>").Replace("<%VALUE%>", "&quot;<%VALUE%>&quot;") + "\" target=\"_BLANK\">";
            string search_link_end = "</a>";
            CurrentRequest.Aggregation = String.Empty;
            CurrentRequest.Search_String = String.Empty;
            CurrentRequest.Search_Fields = String.Empty;
            CurrentRequest.Mode = lastMode;

            // If no search links should should be included, clear the search strings
            if (!Include_Links)
            {
                search_link = String.Empty;
                search_link_end = String.Empty;
            }


            if (Tracer != null)
            {
                Tracer.Add_Trace("Citation_Standard_ItemViewer.Standard_Citation_String", "Configuring brief item data into standard citation format");
            }

            // Use string builder to build this
            const string INDENT = "    ";
            StringBuilder result = new StringBuilder();

            // Need to convert this current item to a brief item
            BriefItemInfo BriefItem = BriefItem_Factory.Create(editingItem, Tracer);

            // Now, try to add the thumbnail from any page images here
            if (BriefItem.Behaviors.Dark_Flag != true)
            {
                string name_for_image = HttpUtility.HtmlEncode(BriefItem.Title);

                if (!String.IsNullOrEmpty(BriefItem.Behaviors.Main_Thumbnail))
                {

                    result.AppendLine();
                    result.AppendLine(INDENT + "<div id=\"Sbk_CivThumbnailDiv\"><a href=\"" + CurrentRequest.Base_URL + BriefItem.BibID + "/" + BriefItem.VID + "\" ><img src=\"" + BriefItem.Web.Source_URL + "/" + BriefItem.Behaviors.Main_Thumbnail + "\" alt=\"" + name_for_image + "\" id=\"Sbk_CivThumbnailImg\" itemprop=\"primaryImageOfPage\" /></a></div>");
                    result.AppendLine();
                }
                else if ((BriefItem.Images != null) && (BriefItem.Images.Count > 0))
                {
                    if (BriefItem.Images[0].Files.Count > 0)
                    {
                        string jpeg = String.Empty;
                        foreach (BriefItem_File thisFileInfo in BriefItem.Images[0].Files)
                        {
                            if (thisFileInfo.Name.ToLower().IndexOf(".jpg") > 0)
                            {
                                if (jpeg.Length == 0)
                                    jpeg = thisFileInfo.Name;
                                else if (thisFileInfo.Name.ToLower().IndexOf("thm.jpg") < 0)
                                    jpeg = thisFileInfo.Name;
                            }
                        }

                        string name_of_page = BriefItem.Images[0].Label;
                        name_for_image = name_for_image + " - " + HttpUtility.HtmlEncode(name_of_page);


                        // If a jpeg was found, show it
                        if (jpeg.Length > 0)
                        {
                            result.AppendLine();
                            result.AppendLine(INDENT + "<div id=\"Sbk_CivThumbnailDiv\"><a href=\"" + CurrentRequest.Base_URL + BriefItem.BibID + "/" + BriefItem.VID + "\" ><img src=\"" + BriefItem.Web.Source_URL + "/" + jpeg + "\" alt=\"" + name_for_image + "\" id=\"Sbk_CivThumbnailImg\" itemprop=\"primaryImageOfPage\" /></a></div>");
                            result.AppendLine();
                        }
                    }
                }
            }

            // Step through the citation configuration here
            CitationSet citationSet = UI_ApplicationCache_Gateway.Configuration.UI.CitationViewer.Get_CitationSet();
            foreach (CitationFieldSet fieldsSet in citationSet.FieldSets)
            {
                // Check to see if any of the values indicated in this field set exist
                bool foundExistingData = false;
                foreach (CitationElement thisField in fieldsSet.Elements)
                {
                    // Was this a custom writer?
                    if ((thisField.SectionWriter != null) && (!String.IsNullOrWhiteSpace(thisField.SectionWriter.Class_Name)))
                    {
                        // Try to get the section writer
                        iCitationSectionWriter sectionWriter = SectionWriter_Factory.GetSectionWriter(thisField.SectionWriter.Assembly, thisField.SectionWriter.Class_Name);

                        // If it was found and there is data, then we found some
                        if ((sectionWriter != null) && (sectionWriter.Has_Data_To_Write(thisField, BriefItem)))
                        {
                            foundExistingData = true;
                            break;
                        }
                    }
                    else // Not a custom writer
                    {
                        // Look for a match in the item description
                        BriefItem_DescriptiveTerm briefTerm = BriefItem.Get_Description(thisField.MetadataTerm);

                        // If no match, just continue
                        if ((briefTerm != null) && (briefTerm.Values.Count > 0))
                        {
                            foundExistingData = true;
                            break;
                        }
                    }
                }

                // If no data was found to put in this field set, skip it
                if (!foundExistingData)
                    continue;

                // Start this section
                result.AppendLine(INDENT + "<div class=\"sbkCiv_CitationSection\" id=\"sbkCiv_" + fieldsSet.ID.Replace(" ", "_") + "Section\" >");
                if (!String.IsNullOrEmpty(fieldsSet.Heading))
                {
                    result.AppendLine(INDENT + "<h2>" + UI_ApplicationCache_Gateway.Translation.Get_Translation(fieldsSet.Heading, CurrentRequest.Language) + "</h2>");
                }
                result.AppendLine(INDENT + "  <dl>");

                // Step through all the fields in this field set and write them
                foreach (CitationElement thisField in fieldsSet.Elements)
                {
                    // Was this a custom writer?
                    if ((thisField.SectionWriter != null) && (!String.IsNullOrWhiteSpace(thisField.SectionWriter.Class_Name)))
                    {
                        // Try to get the section writer
                        iCitationSectionWriter sectionWriter = SectionWriter_Factory.GetSectionWriter(thisField.SectionWriter.Assembly, thisField.SectionWriter.Class_Name);

                        // If it was found and there is data, then we found some
                        if ((sectionWriter != null) && (sectionWriter.Has_Data_To_Write(thisField, BriefItem)))
                        {
                            sectionWriter.Write_Citation_Section(thisField, result, BriefItem, 180, search_link, search_link_end, Tracer);
                        }
                    }
                    else // Not a custom writer
                    {

                        // Look for a match in the item description
                        BriefItem_DescriptiveTerm briefTerm = BriefItem.Get_Description(thisField.MetadataTerm);

                        // If no match, just continue
                        if ((briefTerm == null) || (briefTerm.Values.Count == 0))
                            continue;

                        // If they can all be listed one after the other do so now
                        if (!thisField.IndividualFields)
                        {
                            List<string> valueArray = new List<string>();
                            foreach (BriefItem_DescTermValue thisValue in briefTerm.Values)
                            {
                                if (!String.IsNullOrEmpty(thisField.SearchCode))
                                {
                                    // It is possible a different search term is valid for this item, so check it
                                    string searchTerm = (!String.IsNullOrWhiteSpace(thisValue.SearchTerm)) ? thisValue.SearchTerm : thisValue.Value;

                                    if (String.IsNullOrEmpty(thisField.ItemProp))
                                    {
                                        if (String.IsNullOrEmpty(thisValue.Authority))
                                        {
                                            if (String.IsNullOrEmpty(thisValue.Language))
                                            {
                                                valueArray.Add(search_link.Replace("<%VALUE%>", search_link_from_value(searchTerm)).Replace("<%CODE%>", thisField.SearchCode) + display_text_from_value(thisValue.Value) + search_link_end);
                                            }
                                            else
                                            {
                                                valueArray.Add(search_link.Replace("<%VALUE%>", search_link_from_value(searchTerm)).Replace("<%CODE%>", thisField.SearchCode) + display_text_from_value(thisValue.Value) + search_link_end + " ( " + thisValue.Language + " )");
                                            }
                                        }
                                        else
                                        {
                                            if (String.IsNullOrEmpty(thisValue.Language))
                                            {
                                                valueArray.Add(search_link.Replace("<%VALUE%>", search_link_from_value(searchTerm)).Replace("<%CODE%>", thisField.SearchCode) + display_text_from_value(thisValue.Value) + search_link_end + " ( " + thisValue.Authority + " )");
                                            }
                                            else
                                            {
                                                valueArray.Add(search_link.Replace("<%VALUE%>", search_link_from_value(searchTerm)).Replace("<%CODE%>", thisField.SearchCode) + display_text_from_value(thisValue.Value) + search_link_end + " ( " + thisValue.Authority + ", " + thisValue.Language + " )");
                                            }
                                        }

                                    }
                                    else
                                    {
                                        if (String.IsNullOrEmpty(thisValue.Authority))
                                        {
                                            if (String.IsNullOrEmpty(thisValue.Language))
                                            {
                                                valueArray.Add("<span itemprop=\"" + thisField.ItemProp + "\">" + search_link.Replace("<%VALUE%>", search_link_from_value(searchTerm)).Replace("<%CODE%>", thisField.SearchCode) + display_text_from_value(thisValue.Value) + search_link_end + "</span>");
                                            }
                                            else
                                            {
                                                valueArray.Add("<span itemprop=\"" + thisField.ItemProp + "\">" + search_link.Replace("<%VALUE%>", search_link_from_value(searchTerm)).Replace("<%CODE%>", thisField.SearchCode) + display_text_from_value(thisValue.Value) + search_link_end + " ( " + thisValue.Language + " )" + "</span>");
                                            }
                                        }
                                        else
                                        {
                                            if (String.IsNullOrEmpty(thisValue.Language))
                                            {
                                                valueArray.Add("<span itemprop=\"" + thisField.ItemProp + "\">" + search_link.Replace("<%VALUE%>", search_link_from_value(searchTerm)).Replace("<%CODE%>", thisField.SearchCode) + display_text_from_value(thisValue.Value) + search_link_end + " ( " + thisValue.Authority + " )" + "</span>");
                                            }
                                            else
                                            {
                                                valueArray.Add("<span itemprop=\"" + thisField.ItemProp + "\">" + search_link.Replace("<%VALUE%>", search_link_from_value(searchTerm)).Replace("<%CODE%>", thisField.SearchCode) + display_text_from_value(thisValue.Value) + search_link_end + " ( " + thisValue.Authority + ", " + thisValue.Language + " )" + "</span>");
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (String.IsNullOrEmpty(thisField.ItemProp))
                                    {
                                        if (String.IsNullOrEmpty(thisValue.Authority))
                                        {
                                            if (String.IsNullOrEmpty(thisValue.Language))
                                            {
                                                valueArray.Add(display_text_from_value(thisValue.Value));
                                            }
                                            else
                                            {
                                                valueArray.Add(display_text_from_value(thisValue.Value) + " ( " + thisValue.Language + " )");
                                            }
                                        }
                                        else
                                        {
                                            if (String.IsNullOrEmpty(thisValue.Language))
                                            {
                                                valueArray.Add(display_text_from_value(thisValue.Value) + " ( " + thisValue.Authority + " )");
                                            }
                                            else
                                            {
                                                valueArray.Add(display_text_from_value(thisValue.Value) + " ( " + thisValue.Authority + ", " + thisValue.Language + " )");
                                            }
                                        }

                                    }
                                    else
                                    {
                                        if (String.IsNullOrEmpty(thisValue.Authority))
                                        {
                                            if (String.IsNullOrEmpty(thisValue.Language))
                                            {
                                                valueArray.Add("<span itemprop=\"" + thisField.ItemProp + "\">" + display_text_from_value(thisValue.Value) + "</span>");
                                            }
                                            else
                                            {
                                                valueArray.Add("<span itemprop=\"" + thisField.ItemProp + "\">" + display_text_from_value(thisValue.Value) + " ( " + thisValue.Language + " )" + "</span>");
                                            }
                                        }
                                        else
                                        {
                                            if (String.IsNullOrEmpty(thisValue.Language))
                                            {
                                                valueArray.Add("<span itemprop=\"" + thisField.ItemProp + "\">" + display_text_from_value(thisValue.Value) + " ( " + thisValue.Authority + " )" + "</span>");
                                            }
                                            else
                                            {
                                                valueArray.Add("<span itemprop=\"" + thisField.ItemProp + "\">" + display_text_from_value(thisValue.Value) + " ( " + thisValue.Authority + ", " + thisValue.Language + " )" + "</span>");
                                            }
                                        }
                                    }
                                }
                            }

                            // Now, add this to the citation HTML
                            Add_Citation_HTML_Rows(thisField.DisplayTerm, valueArray, INDENT, result);
                        }
                        else
                        {
                            // In this case, each individual value gets its own citation html row
                            foreach (BriefItem_DescTermValue thisValue in briefTerm.Values)
                            {
                                // Determine the label
                                string label = thisField.DisplayTerm;
                                if (thisField.OverrideDisplayTerm == CitationElement_OverrideDispayTerm_Enum.subterm)
                                {
                                    if (!String.IsNullOrEmpty(thisValue.SubTerm))
                                        label = thisValue.SubTerm;
                                }

                                // It is possible a different search term is valid for this item, so check it
                                string searchTerm = (!String.IsNullOrWhiteSpace(thisValue.SearchTerm)) ? thisValue.SearchTerm : thisValue.Value;

                                if (!String.IsNullOrEmpty(thisField.SearchCode))
                                {
                                    if (String.IsNullOrEmpty(thisField.ItemProp))
                                    {
                                        if (String.IsNullOrEmpty(thisValue.Authority))
                                        {
                                            if (String.IsNullOrEmpty(thisValue.Language))
                                            {
                                                result.Append(Single_Citation_HTML_Row(label, search_link.Replace("<%VALUE%>", search_link_from_value(searchTerm)).Replace("<%CODE%>", thisField.SearchCode) + display_text_from_value(thisValue.Value) + search_link_end, INDENT));
                                            }
                                            else
                                            {
                                                result.Append(Single_Citation_HTML_Row(label, search_link.Replace("<%VALUE%>", search_link_from_value(searchTerm)).Replace("<%CODE%>", thisField.SearchCode) + display_text_from_value(thisValue.Value) + search_link_end + " ( " + thisValue.Language + " )", INDENT));
                                            }
                                        }
                                        else
                                        {
                                            if (String.IsNullOrEmpty(thisValue.Language))
                                            {
                                                result.Append(Single_Citation_HTML_Row(label, search_link.Replace("<%VALUE%>", search_link_from_value(searchTerm)).Replace("<%CODE%>", thisField.SearchCode) + display_text_from_value(thisValue.Value) + search_link_end + " ( " + thisValue.Authority + " )", INDENT));
                                            }
                                            else
                                            {
                                                result.Append(Single_Citation_HTML_Row(label, search_link.Replace("<%VALUE%>", search_link_from_value(searchTerm)).Replace("<%CODE%>", thisField.SearchCode) + display_text_from_value(thisValue.Value) + search_link_end + " ( " + thisValue.Authority + ", " + thisValue.Language + " )", INDENT));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (String.IsNullOrEmpty(thisValue.Authority))
                                        {
                                            if (String.IsNullOrEmpty(thisValue.Language))
                                            {
                                                result.Append(Single_Citation_HTML_Row(label, "<span itemprop=\"" + thisField.ItemProp + "\">" + search_link.Replace("<%VALUE%>", search_link_from_value(searchTerm)).Replace("<%CODE%>", thisField.SearchCode) + display_text_from_value(thisValue.Value) + search_link_end + "</span>", INDENT));
                                            }
                                            else
                                            {
                                                result.Append(Single_Citation_HTML_Row(label, "<span itemprop=\"" + thisField.ItemProp + "\">" + search_link.Replace("<%VALUE%>", search_link_from_value(searchTerm)).Replace("<%CODE%>", thisField.SearchCode) + display_text_from_value(thisValue.Value) + search_link_end + " ( " + thisValue.Language + " )" + "</span>", INDENT));
                                            }
                                        }
                                        else
                                        {
                                            if (String.IsNullOrEmpty(thisValue.Language))
                                            {
                                                result.Append(Single_Citation_HTML_Row(label, "<span itemprop=\"" + thisField.ItemProp + "\">" + search_link.Replace("<%VALUE%>", search_link_from_value(searchTerm)).Replace("<%CODE%>", thisField.SearchCode) + display_text_from_value(thisValue.Value) + search_link_end + " ( " + thisValue.Authority + " )" + "</span>", INDENT));
                                            }
                                            else
                                            {
                                                result.Append(Single_Citation_HTML_Row(label, "<span itemprop=\"" + thisField.ItemProp + "\">" + search_link.Replace("<%VALUE%>", search_link_from_value(searchTerm)).Replace("<%CODE%>", thisField.SearchCode) + display_text_from_value(thisValue.Value) + search_link_end + " ( " + thisValue.Authority + ", " + thisValue.Language + " )" + "</span>", INDENT));
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    // Since this isn't tied to a search code, we won't build a URL.  But the
                                    // data could still HAVE a URL associated with it.
                                    if ((thisValue.URIs == null) || (thisValue.URIs.Count == 0))
                                    {
                                        if (String.IsNullOrEmpty(thisField.ItemProp))
                                        {
                                            if (String.IsNullOrEmpty(thisValue.Authority))
                                            {
                                                if (String.IsNullOrEmpty(thisValue.Language))
                                                {
                                                    result.Append(Single_Citation_HTML_Row(label, display_text_from_value(thisValue.Value), INDENT));
                                                }
                                                else
                                                {
                                                    result.Append(Single_Citation_HTML_Row(label, display_text_from_value(thisValue.Value) + " ( " + thisValue.Language + " )", INDENT));
                                                }
                                            }
                                            else
                                            {
                                                if (String.IsNullOrEmpty(thisValue.Language))
                                                {
                                                    result.Append(Single_Citation_HTML_Row(label, display_text_from_value(thisValue.Value) + " ( " + thisValue.Authority + " )", INDENT));
                                                }
                                                else
                                                {
                                                    result.Append(Single_Citation_HTML_Row(label, display_text_from_value(thisValue.Value) + " ( " + thisValue.Authority + ", " + thisValue.Language + " )", INDENT));
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (String.IsNullOrEmpty(thisValue.Authority))
                                            {
                                                if (String.IsNullOrEmpty(thisValue.Language))
                                                {
                                                    result.Append(Single_Citation_HTML_Row(label, "<span itemprop=\"" + thisField.ItemProp + "\">" + display_text_from_value(thisValue.Value) + "</span>", INDENT));
                                                }
                                                else
                                                {
                                                    result.Append(Single_Citation_HTML_Row(label, "<span itemprop=\"" + thisField.ItemProp + "\">" + display_text_from_value(thisValue.Value) + " ( " + thisValue.Language + " )" + "</span>", INDENT));
                                                }
                                            }
                                            else
                                            {
                                                if (String.IsNullOrEmpty(thisValue.Language))
                                                {
                                                    result.Append(Single_Citation_HTML_Row(label, "<span itemprop=\"" + thisField.ItemProp + "\">" + display_text_from_value(thisValue.Value) + " ( " + thisValue.Authority + " )" + "</span>", INDENT));
                                                }
                                                else
                                                {
                                                    result.Append(Single_Citation_HTML_Row(label, "<span itemprop=\"" + thisField.ItemProp + "\">" + display_text_from_value(thisValue.Value) + " ( " + thisValue.Authority + ", " + thisValue.Language + " )" + "</span>", INDENT));
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // This has a URI
                                        if (String.IsNullOrEmpty(thisField.ItemProp))
                                        {
                                            if (String.IsNullOrEmpty(thisValue.Authority))
                                            {
                                                if (String.IsNullOrEmpty(thisValue.Language))
                                                {
                                                    result.Append(Single_Citation_HTML_Row(label, "<a href=\"" + thisValue.URIs[0] + "\">" + display_text_from_value(thisValue.Value) + "</a>", INDENT));
                                                }
                                                else
                                                {
                                                    result.Append(Single_Citation_HTML_Row(label, "<a href=\"" + thisValue.URIs[0] + "\">" + display_text_from_value(thisValue.Value) + "</a>" + " ( " + thisValue.Language + " )", INDENT));
                                                }
                                            }
                                            else
                                            {
                                                if (String.IsNullOrEmpty(thisValue.Language))
                                                {
                                                    result.Append(Single_Citation_HTML_Row(label, "<a href=\"" + thisValue.URIs[0] + "\">" + display_text_from_value(thisValue.Value) + "</a>" + " ( " + thisValue.Authority + " )", INDENT));
                                                }
                                                else
                                                {
                                                    result.Append(Single_Citation_HTML_Row(label, "<a href=\"" + thisValue.URIs[0] + "\">" + display_text_from_value(thisValue.Value) + "</a>" + " ( " + thisValue.Authority + ", " + thisValue.Language + " )", INDENT));
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (String.IsNullOrEmpty(thisValue.Authority))
                                            {
                                                if (String.IsNullOrEmpty(thisValue.Language))
                                                {
                                                    result.Append(Single_Citation_HTML_Row(label, "<span itemprop=\"" + thisField.ItemProp + "\">" + "<a href=\"" + thisValue.URIs[0] + "\">" + display_text_from_value(thisValue.Value) + "</a>" + "</span>", INDENT));
                                                }
                                                else
                                                {
                                                    result.Append(Single_Citation_HTML_Row(label, "<span itemprop=\"" + thisField.ItemProp + "\">" + "<a href=\"" + thisValue.URIs[0] + "\">" + display_text_from_value(thisValue.Value) + "</a>" + " ( " + thisValue.Language + " )" + "</span>", INDENT));
                                                }
                                            }
                                            else
                                            {
                                                if (String.IsNullOrEmpty(thisValue.Language))
                                                {
                                                    result.Append(Single_Citation_HTML_Row(label, "<span itemprop=\"" + thisField.ItemProp + "\">" + "<a href=\"" + thisValue.URIs[0] + "\">" + display_text_from_value(thisValue.Value) + "</a>" + " ( " + thisValue.Authority + " )" + "</span>", INDENT));
                                                }
                                                else
                                                {
                                                    result.Append(Single_Citation_HTML_Row(label, "<span itemprop=\"" + thisField.ItemProp + "\">" + "<a href=\"" + thisValue.URIs[0] + "\">" + display_text_from_value(thisValue.Value) + "</a>" + " ( " + thisValue.Authority + ", " + thisValue.Language + " )" + "</span>", INDENT));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // End this division
                result.AppendLine(INDENT + "  </dl>");
                result.AppendLine(INDENT + "</div>");
            }

            result.AppendLine(INDENT + "<br />");
            result.AppendLine("</div>");

            // Return the built string
            return result.ToString();
        }

        private static string display_text_from_value(string Value)
        {
            return HttpUtility.HtmlEncode(Value).Replace("&lt;i&gt;", "<i>").Replace("&lt;/i&gt;", "</i>");
        }

        private static string search_link_from_value(string Value)
        {
            string replacedValue = Value.Replace("&amp;", "&").Replace("&", "").Replace("  ", " ");
            string urlEncode = HttpUtility.UrlEncode(replacedValue);
            return urlEncode != null ? urlEncode.Replace(",", "").Replace("&amp;", "&").Replace("&", "").Replace(" ", "+") : String.Empty;
        }

        private void Add_Citation_HTML_Rows(string Row_Name, List<string> Values, string Indent, StringBuilder Results)
        {
            // Only add if there is a value
            if (Values.Count <= 0) return;

            Results.Append(Indent + "    <dt class=\"sbk_Civ" + Row_Name.ToUpper().Replace(" ", "_") + "_Element\" style=\"width:180px;\" >");

            // Add with proper language
            Results.Append(UI_ApplicationCache_Gateway.Translation.Get_Translation(Row_Name, RequestSpecificValues.Current_Mode.Language));

            Results.AppendLine(": </dt>");
            Results.Append(Indent + "    <dd class=\"sbk_Civ" + Row_Name.ToUpper().Replace(" ", "_") + "_Element\" style=\"margin-left:180px;\">");
            bool first = true;
            foreach (string thisValue in Values.Where(ThisValue => ThisValue.Length > 0))
            {
                if (first)
                {
                    Results.Append(thisValue);
                    first = false;
                }
                else
                {
                    Results.Append("<br />" + thisValue);
                }
            }
            Results.AppendLine("</dd>");
            Results.AppendLine();
        }

        private string Single_Citation_HTML_Row(string Row_Name, string Value, string Indent)
        {
            // Only add if there is a value
            if (Value.Length > 0)
            {
                return Indent + "    <dt class=\"sbk_Civ" + Row_Name.ToUpper().Replace(" ", "_") + "_Element\" style=\"width:180px;\">" + UI_ApplicationCache_Gateway.Translation.Get_Translation(Row_Name, RequestSpecificValues.Current_Mode.Language) + ": </dt>" + Environment.NewLine + Indent + "    <dd class=\"sbk_Civ" + Row_Name.ToUpper().Replace(" ", "_") + "_Element\" style=\"margin-left:180px;\">" + Value + "</dd>" + Environment.NewLine + Environment.NewLine;
            }
            return String.Empty;
        }

        #endregion


        /// <summary> Navigation type to be displayed (mostly used by the mySobek viewers) </summary>
        /// <value> This returns none since this viewer writes all the necessary navigational elements </value>
        /// <remarks> This is set to NONE if the viewer will write its own navigation and ADMIN if the standard
        /// administrative tabs should be included as well.  </remarks>
        public override MySobek_Admin_Included_Navigation_Enum Standard_Navigation_Type { get { return MySobek_Admin_Included_Navigation_Enum.NONE; } }


        /// <summary> Returns a flag indicating whether the file upload specific holder in the itemNavForm form will be utilized 
        /// for the current request, or if it can be hidden/omitted. </summary>
        /// <value> Sometimes returns TRUE when files can be be uploaded through this viewer </value>
        public override bool Upload_File_Possible
        {
            get { return (!String.IsNullOrEmpty(RequestSpecificValues.Current_Mode.My_Sobek_SubMode)) && ((RequestSpecificValues.Current_Mode.My_Sobek_SubMode[0] == '1')); }
        }

        /// <summary> Write any additional values within the HTML Head of the final served page </summary>
        /// <param name="Output"> Output stream currently within the HTML head tags </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> TRUE if this should completely override the default added by the admin or mySobek viewer </returns>
        public override bool Write_Within_HTML_Head(TextWriter Output, Custom_Tracer Tracer)
        {
            Output.WriteLine("  <link href=\"" + Static_Resources_Gateway.Sobekcm_Metadata_Css + "\" rel=\"stylesheet\" type=\"text/css\" />");

            if (currentProcessStep == 4)
            {
                Output.WriteLine("  <link href=\"" + Static_Resources_Gateway.Sobekcm_Item_Css + "\" rel=\"stylesheet\" type=\"text/css\" />");
            }

            return false;
        }

        /// <summary> Gets the collection of special behaviors which this admin or mySobek viewer
        /// requests from the main HTML subwriter. </summary>
        /// <value> This tells the HTML and mySobek writers to mimic the currentItem viewer </value>
        public override List<HtmlSubwriter_Behaviors_Enum> Viewer_Behaviors
        {
            get
            {
                return new List<HtmlSubwriter_Behaviors_Enum>
                {
                    HtmlSubwriter_Behaviors_Enum.MySobek_Subwriter_Mimic_Item_Subwriter,
                    HtmlSubwriter_Behaviors_Enum.Suppress_Banner
                };
            }
        }
    }
}




