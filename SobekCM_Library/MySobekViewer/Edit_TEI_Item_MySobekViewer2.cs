using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SobekCM.Core.Aggregations;
using SobekCM.Core.BriefItem;
using SobekCM.Core.Client;
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
// STAGE 2: Edit TEI (have FINISH from here)
// STAGE 3: Edit Metadata



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

        private readonly string tei_file;
        private readonly string mapping_file;
        private readonly string xslt_file;
        private readonly string css_file;

        private readonly string error_message;


        private readonly SobekCM_Item currentItem;
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

            RequestSpecificValues.Tracer.Add_Trace("Edit_Item_Metadata_MySobekViewer.Constructor", "Try to pull this sobek complete item");
            currentItem = SobekEngineClient.Items.Get_Sobek_Item(bibid, vid, RequestSpecificValues.Current_User.UserID, RequestSpecificValues.Tracer);
            if (currentItem == null)
            {
                RequestSpecificValues.Tracer.Add_Trace("Edit_Item_Metadata_MySobekViewer.Constructor", "Unable to build complete item");
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Error;
                RequestSpecificValues.Current_Mode.Error_Message = "Invalid Request : Unable to build complete item";
                return;
            }

            // Determine the in process directory for this
            if (RequestSpecificValues.Current_User.ShibbID.Trim().Length > 0)
                userInProcessDirectory = Path.Combine(UI_ApplicationCache_Gateway.Settings.Servers.In_Process_Submission_Location, RequestSpecificValues.Current_User.ShibbID, "teiedit", bibid + "_" + vid);
            else
                userInProcessDirectory = Path.Combine(UI_ApplicationCache_Gateway.Settings.Servers.In_Process_Submission_Location, RequestSpecificValues.Current_User.UserName.Replace(".", "").Replace("@", ""), "teiedit", bibid + "_" + vid);

            // Load the CompleteTemplate
            completeTemplate = Template_MemoryMgmt_Utility.Retrieve_Template("tei", RequestSpecificValues.Tracer);
            if (completeTemplate != null)
            {
                RequestSpecificValues.Tracer.Add_Trace("Edit_TEI_Item_MySobekViewer.Constructor", "Found template in cache");
            }
            else
            {
                RequestSpecificValues.Tracer.Add_Trace("Edit_TEI_Item_MySobekViewer.Constructor", "Reading template");

                string user_template = Path.Combine(UI_ApplicationCache_Gateway.Settings.Servers.Application_Server_Network, "plugins\\tei\\templates\\user\\template.xml");
                if (!File.Exists(user_template))
                    user_template = Path.Combine(UI_ApplicationCache_Gateway.Settings.Servers.Application_Server_Network, "plugins\\tei\\templates\\default\\template.xml");


                // Read this CompleteTemplate
                Template_XML_Reader reader = new Template_XML_Reader();
                completeTemplate = new CompleteTemplate();
                reader.Read_XML(user_template, completeTemplate, true);

                // Save this into the cache
                Template_MemoryMgmt_Utility.Store_Template("tei", completeTemplate, RequestSpecificValues.Tracer);
            }

            // Determine the number of total CompleteTemplate pages
            totalTemplatePages = completeTemplate.InputPages_Count + 2;

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


            // If there is a boundary infraction here, go back to step 1
            if (currentProcessStep < 0)
                currentProcessStep = 1;
            if ((currentProcessStep > completeTemplate.InputPages.Count + 4) && (currentProcessStep != 8) && (currentProcessStep != 9))
                currentProcessStep = 1;

            // Look for the item in the session, then directory, then just create a new one
            if (HttpContext.Current.Session["TEI_Item"] == null)
            {
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
                        if (DateTime.Now.Subtract(modifiedDate).TotalHours > (24 * 7))
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

                // First, look for an existing METS file
                string[] existing_mets = Directory.GetFiles(userInProcessDirectory, "*.mets*");
                if (existing_mets.Length > 0)
                {
                    RequestSpecificValues.Tracer.Add_Trace("Edit_TEI_Item_MySobekViewer.Constructor", "Reading existing METS file<br />(" + existing_mets[0] + ")");
                    item = SobekCM_Item.Read_METS(existing_mets[0]);

                    // Set the visibility information from the CompleteTemplate
                    item.Behaviors.IP_Restriction_Membership = completeTemplate.Default_Visibility;
                }

                // If there is still no item, just create a new one
                if (item == null)
                {
                    // Build a new empty METS file
                    new_item(RequestSpecificValues.Tracer);
                }

                // Save this to the session state now
                HttpContext.Current.Session["Item"] = item;
            }
            else
            {
                RequestSpecificValues.Tracer.Add_Trace("Edit_TEI_Item_MySobekViewer.Constructor", "Item found in session cache");
                item = (SobekCM_Item)HttpContext.Current.Session["Item"];
            }

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
                        if (Path.GetFileName(thisTeiFile).ToLower().IndexOf("marc.xml") >= 0)
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
                            if (Path.GetFileName(thisTeiFile).ToLower().IndexOf("marc.xml") >= 0)
                                continue;

                            // Was this the latest file?
                            if (String.Compare(thisTeiFile, latest_tei_file, StringComparison.OrdinalIgnoreCase) == 0)
                                continue;

                            try
                            {
                                File.Delete(thisTeiFile);
                            }
                            catch { }

                        }
                    }

                    tei_file = latest_tei_file;
                }
                else if (tei_files.Length == 1)
                {
                    tei_file = Path.GetFileName(tei_files[0]);
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
                    string file_name_from_keys = String.Empty;
                    string label_from_keys = String.Empty;
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
                        tei_file = ee.Message;
                        // Unable to delete existing file in the RequestSpecificValues.Current_User's folder.
                        // This is an error, but how to report it?
                    }

                    // Clear all the information in memory
                    HttpContext.Current.Session["item"] = null;

                    HttpContext.Current.Session["Edit_TEI_Item_MySobekViewer." + bibid + "_" + vid + ".Mapping_File"] = null;
                    HttpContext.Current.Session["Edit_TEI_Item_MySobekViewer." + bibid + "_" + vid + ".XSLT_File"] = null;
                    HttpContext.Current.Session["Edit_TEI_Item_MySobekViewer." + bibid + "_" + vid + ".CSS_File"] = null;


                    // Clear any temporarily assigned current project and CompleteTemplate
                    RequestSpecificValues.Current_User.Current_Default_Metadata = null;
                    RequestSpecificValues.Current_User.Current_Template = null;

                    // Forward back to my Sobek home
                    RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                    UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                }

                if (action == "delete")
                {
                    string filename = HttpContext.Current.Request.Form["phase"];
                    try
                    {
                        if (File.Exists(userInProcessDirectory + "\\" + filename))
                            File.Delete(userInProcessDirectory + "\\" + filename);

                        // Forward
                        UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                        return;
                    }
                    catch (Exception)
                    {
                        // Unable to delete existing file in the RequestSpecificValues.Current_User's folder.
                        // This is an error, but how to report it?
                    }
                }

                if (action == "next_phase")
                {
                    string next_phase = HttpContext.Current.Request.Form["phase"];
                

                    // If this is going from a step that includes the metadata entry portion, save this to the item
                    if ((currentProcessStep > 4) && (currentProcessStep < 8))
                    {
                        // Save to the item
                        completeTemplate.Save_To_Bib(item, RequestSpecificValues.Current_User, currentProcessStep - 4);
                        item.Save_METS();
                        HttpContext.Current.Session["Item"] = item;

                        // Save the pertinent data to the METS file package
                        item.METS_Header.Create_Date = DateTime.Now;
                        if ((HttpContext.Current.Session["agreement_date"] != null) && (HttpContext.Current.Session["agreement_date"].ToString().Length > 0))
                        {
                            DateTime asDateTime;
                            if (DateTime.TryParse(HttpContext.Current.Session["agreement_date"].ToString(), out asDateTime))
                                item.METS_Header.Create_Date = asDateTime;
                        }
                        HttpContext.Current.Session["Item"] = item;

                        // Save this item, just in case it gets lost somehow
                        item.Source_Directory = userInProcessDirectory;
                        string acquisition_append = "Submitted by " + RequestSpecificValues.Current_User.Full_Name + ".";
                        if (item.Bib_Info.Notes_Count > 0)
                        {
                            foreach (Note_Info thisNote in item.Bib_Info.Notes.Where(ThisNote => ThisNote.Note_Type == Note_Type_Enum.Acquisition))
                            {
                                if (thisNote.Note.IndexOf(acquisition_append) < 0)
                                    thisNote.Note = thisNote.Note.Trim() + "  " + acquisition_append;
                                break;
                            }
                        }

                        // Also, check all the authors to add the current users attribution information
                        if (RequestSpecificValues.Current_User.Organization.Length > 0)
                        {
                            if ((item.Bib_Info.Main_Entity_Name.Full_Name.IndexOf(RequestSpecificValues.Current_User.Family_Name) >= 0) && ((item.Bib_Info.Main_Entity_Name.Full_Name.IndexOf(RequestSpecificValues.Current_User.Given_Name) >= 0) || ((RequestSpecificValues.Current_User.Nickname.Length > 2) && (item.Bib_Info.Main_Entity_Name.Full_Name.IndexOf(RequestSpecificValues.Current_User.Nickname) > 0))))
                            {
                                item.Bib_Info.Main_Entity_Name.Affiliation = RequestSpecificValues.Current_User.Organization;
                                if (RequestSpecificValues.Current_User.College.Length > 0)
                                    item.Bib_Info.Main_Entity_Name.Affiliation = item.Bib_Info.Main_Entity_Name.Affiliation + " -- " + RequestSpecificValues.Current_User.College;
                                if (RequestSpecificValues.Current_User.Department.Length > 0)
                                    item.Bib_Info.Main_Entity_Name.Affiliation = item.Bib_Info.Main_Entity_Name.Affiliation + " -- " + RequestSpecificValues.Current_User.Department;
                                if (RequestSpecificValues.Current_User.Unit.Length > 0)
                                    item.Bib_Info.Main_Entity_Name.Affiliation = item.Bib_Info.Main_Entity_Name.Affiliation + " -- " + RequestSpecificValues.Current_User.Unit;
                            }
                            if (item.Bib_Info.Names_Count > 0)
                            {
                                foreach (Name_Info thisName in item.Bib_Info.Names)
                                {
                                    if ((thisName.Full_Name.IndexOf(RequestSpecificValues.Current_User.Family_Name) >= 0) && ((thisName.Full_Name.IndexOf(RequestSpecificValues.Current_User.Given_Name) >= 0) || ((RequestSpecificValues.Current_User.Nickname.Length > 2) && (thisName.Full_Name.IndexOf(RequestSpecificValues.Current_User.Nickname) > 0))))
                                    {
                                        thisName.Affiliation = RequestSpecificValues.Current_User.Organization;
                                        if (RequestSpecificValues.Current_User.College.Length > 0)
                                            thisName.Affiliation = thisName.Affiliation + " -- " + RequestSpecificValues.Current_User.College;
                                        if (RequestSpecificValues.Current_User.Department.Length > 0)
                                            thisName.Affiliation = thisName.Affiliation + " -- " + RequestSpecificValues.Current_User.Department;
                                        if (RequestSpecificValues.Current_User.Unit.Length > 0)
                                            thisName.Affiliation = thisName.Affiliation + " -- " + RequestSpecificValues.Current_User.Unit;

                                    }
                                }
                            }
                        }
                        item.Save_METS();
                        HttpContext.Current.Session["Item"] = item;
                    }

                    // For now, just forward to the next phase
                    RequestSpecificValues.Current_Mode.My_Sobek_SubMode = next_phase;
                    UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                    return;
                }
            }

            #endregion

            #region Perform some validation to determine if the RequestSpecificValues.Current_User should be at this step

            // If this is past the step to upload a TEI file, ensure a TEI file exists
            if ((currentProcessStep > 1) && (String.IsNullOrEmpty(tei_file)))
            {
                RequestSpecificValues.Current_Mode.My_Sobek_SubMode = "1";
                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                return;
            }


            // If this is past the step to upload a TEI file, ensure a TEI file exists
            if ((currentProcessStep > 1) && ((String.IsNullOrEmpty(mapping_file)) || (String.IsNullOrEmpty(xslt_file)) || (String.IsNullOrEmpty(css_file))))
            {
                RequestSpecificValues.Current_Mode.My_Sobek_SubMode = "3";
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
                if (validationErrors.Count == 0)
                    item.Save_METS();
                else
                {
                    item.Web.Show_Validation_Errors = true;
                    RequestSpecificValues.Current_Mode.My_Sobek_SubMode = "2";
                    UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                    return;
                }
            }

            // If this is for step 8, ensure that this even takes this information, or go to step 9
            if ((currentProcessStep == 8) && (completeTemplate.Upload_Types == CompleteTemplate.Template_Upload_Types.None))
            {
                RequestSpecificValues.Current_Mode.My_Sobek_SubMode = "9";
                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                return;
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
            // Set an initial flag 
            criticalErrorEncountered = false;
            bool xml_found = false;

            string[] all_files = Directory.GetFiles(userInProcessDirectory);
            SortedList<string, List<string>> image_files = new SortedList<string, List<string>>();
            SortedList<string, List<string>> download_files = new SortedList<string, List<string>>();
            foreach (string thisFile in all_files)
            {
                FileInfo thisFileInfo = new FileInfo(thisFile);

                if ((thisFileInfo.Name.IndexOf("agreement.txt") != 0) && (thisFileInfo.Name.IndexOf("TEMP000001_00001.mets") != 0) && (thisFileInfo.Name.IndexOf("doc.xml") != 0) && (thisFileInfo.Name.IndexOf("ufdc_mets.xml") != 0) && (thisFileInfo.Name.IndexOf("marc.xml") != 0))
                {
                    // Get information about this files name and extension
                    string extension_upper = thisFileInfo.Extension.ToUpper();
                    string filename_sans_extension = thisFileInfo.Name.Replace(thisFileInfo.Extension, "");
                    string name_upper = thisFileInfo.Name.ToUpper();

                    // Was this the TEI file?
                    if (String.Compare(tei_file, name_upper, StringComparison.OrdinalIgnoreCase) == 0)
                        continue;

                    // Is this a page image?
                    if ((extension_upper == ".JPG") || (extension_upper == ".TIF") || (extension_upper == ".JP2") || (extension_upper == ".JPX"))
                    {
                        // Exclude .QC.jpg files
                        if (name_upper.IndexOf(".QC.JPG") < 0)
                        {
                            // If this is a thumbnail, trim off the THM part on the file name
                            if (name_upper.IndexOf("THM.JPG") > 0)
                            {
                                filename_sans_extension = filename_sans_extension.Substring(0, filename_sans_extension.Length - 3);
                            }

                            // Is this the first image file with this name?
                            if (image_files.ContainsKey(filename_sans_extension.ToLower()))
                            {
                                image_files[filename_sans_extension.ToLower()].Add(thisFileInfo.Name);
                            }
                            else
                            {
                                List<string> newImageGrouping = new List<string> { thisFileInfo.Name };
                                image_files[filename_sans_extension.ToLower()] = newImageGrouping;
                            }
                        }
                    }
                    else
                    {
                        // If this does not match the exclusion regular expression, than add this
                        if (!Regex.Match(thisFileInfo.Name, UI_ApplicationCache_Gateway.Settings.Resources.Files_To_Exclude_From_Downloads, RegexOptions.IgnoreCase).Success)
                        {
                            // Also, exclude files that are .XML and marc.xml, or doc.xml, or have the bibid in the name
                            if ((thisFileInfo.Name.IndexOf("marc.xml", StringComparison.OrdinalIgnoreCase) != 0) && (thisFileInfo.Name.IndexOf("marc.xml", StringComparison.OrdinalIgnoreCase) != 0) && (thisFileInfo.Name.IndexOf(".mets", StringComparison.OrdinalIgnoreCase) < 0))
                            {
                                // Is this the first image file with this name?
                                if (download_files.ContainsKey(filename_sans_extension.ToLower()))
                                {
                                    download_files[filename_sans_extension.ToLower()].Add(thisFileInfo.Name);
                                }
                                else
                                {
                                    List<string> newDownloadGrouping = new List<string> { thisFileInfo.Name };
                                    download_files[filename_sans_extension.ToLower()] = newDownloadGrouping;
                                }
                            }
                        }
                    }
                }
            }

            // This package is good to go, so build it, save, etc...
            try
            {
                // Save the METS file to the database and back to the directory
                Item_To_Complete.Source_Directory = userInProcessDirectory;

                // Step through and add each file 
                Item_To_Complete.Divisions.Download_Tree.Clear();
                if ((completeTemplate.Upload_Types == CompleteTemplate.Template_Upload_Types.File_or_URL) || (completeTemplate.Upload_Types == CompleteTemplate.Template_Upload_Types.File))
                {
                    // Step through each file

                    bool error_reading_file_occurred = false;

                    // Add the image files first
                    bool jpeg_added = false;
                    bool jp2_added = false;
                    foreach (string thisFileKey in image_files.Keys)
                    {
                        // Get the list of files
                        List<string> theseFiles = image_files[thisFileKey];

                        // Add each file
                        foreach (string thisFile in theseFiles)
                        {
                            // Create the new file object and compute a label
                            FileInfo fileInfo = new FileInfo(thisFile);
                            SobekCM_File_Info newFile = new SobekCM_File_Info(fileInfo.Name);
                            string label = fileInfo.Name.Replace(fileInfo.Extension, "");
                            if (HttpContext.Current.Session["file_" + thisFileKey] != null)
                            {
                                string possible_label = HttpContext.Current.Session["file_" + thisFileKey].ToString();
                                if (possible_label.Length > 0)
                                    label = possible_label;
                            }

                            // Add this file
                            Item_To_Complete.Divisions.Physical_Tree.Add_File(newFile, label);

                            // Seperate code for JP2 and JPEG type files
                            string extension = fileInfo.Extension.ToUpper();
                            if (extension.IndexOf("JP2") >= 0)
                            {
                                if (!error_reading_file_occurred)
                                {
                                    if (!newFile.Compute_Jpeg2000_Attributes(userInProcessDirectory))
                                        error_reading_file_occurred = true;
                                }
                                jp2_added = true;
                            }
                            else
                            {
                                if (!error_reading_file_occurred)
                                {
                                    if (!newFile.Compute_Jpeg_Attributes(userInProcessDirectory))
                                        error_reading_file_occurred = true;
                                }
                                jpeg_added = true;
                            }
                        }
                    }

                    // Add the download files next
                    foreach (string thisFileKey in download_files.Keys)
                    {
                        // Get the list of files
                        List<string> theseFiles = download_files[thisFileKey];

                        // Add each file
                        foreach (string thisFile in theseFiles)
                        {
                            // Create the new file object and compute a label
                            FileInfo fileInfo = new FileInfo(thisFile);
                            SobekCM_File_Info newFile = new SobekCM_File_Info(fileInfo.Name);
                            string label = fileInfo.Name.Replace(fileInfo.Extension, "");
                            if (HttpContext.Current.Session["file_" + thisFileKey] != null)
                            {
                                string possible_label = HttpContext.Current.Session["file_" + thisFileKey].ToString();
                                if (possible_label.Length > 0)
                                    label = possible_label;
                            }

                            // Add this file
                            Item_To_Complete.Divisions.Download_Tree.Add_File(newFile, label);
                        }
                    }

                    // Now, add the TEI file
                    SobekCM_File_Info tei_newFile = new SobekCM_File_Info(tei_file);
                    string tei_label = tei_file + " (TEI)";
                    Item_To_Complete.Divisions.Download_Tree.Add_File(tei_newFile, tei_label);
                }

                // Determine the total size of the package before saving
                string[] all_files_final = Directory.GetFiles(userInProcessDirectory);
                double size = all_files_final.Aggregate<string, double>(0, (Current, ThisFile) => Current + (((new FileInfo(ThisFile)).Length) / 1024));
                Item_To_Complete.DiskSize_KB = size;

                // BibID and VID will be automatically assigned
                Item_To_Complete.BibID = completeTemplate.BibID_Root;
                Item_To_Complete.VID = String.Empty;

                // Set some values in the tracking portion
                if (Item_To_Complete.Divisions.Files.Count > 0)
                {
                    Item_To_Complete.Tracking.Born_Digital = true;
                }
                Item_To_Complete.Tracking.VID_Source = "SobekCM:" + templateCode;

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
                SobekCM_Item_Database.Set_Item_Setting_Value(Item_To_Complete.Web.ItemID, "TEI.Source_File", tei_file);
                SobekCM_Item_Database.Set_Item_Setting_Value(Item_To_Complete.Web.ItemID, "TEI.CSS", css_file);
                SobekCM_Item_Database.Set_Item_Setting_Value(Item_To_Complete.Web.ItemID, "TEI.Mapping", mapping_file);

                // Find the actual XSLT file
                string xslt_directory = Path.Combine(UI_ApplicationCache_Gateway.Settings.Servers.Application_Server_Network, "plugins", "tei", "xslt");
                string[] xslt_files = Directory.GetFiles(xslt_directory, xslt_file + ".xsl*");
                SobekCM_Item_Database.Set_Item_Setting_Value(Item_To_Complete.Web.ItemID, "TEI.XSLT", Path.GetFileName(xslt_files[0]));

                // Add the TEI viewer
                SobekCM_Item_Database.Save_Item_Add_Viewer(Item_To_Complete.Web.ItemID, "TEI", tei_file.Replace(".xml", "").Replace(".XML", "") + " (TEI)", tei_file);

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

                // Add this to the cache
                UI_ApplicationCache_Gateway.Items.Add_SobekCM_Item(Item_To_Complete);

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

                // Incrememnt the count of number of items submitted by this RequestSpecificValues.Current_User
                RequestSpecificValues.Current_User.Items_Submitted_Count++;
                if (!RequestSpecificValues.Current_User.BibIDs.Contains(Item_To_Complete.BibID))
                    RequestSpecificValues.Current_User.Add_BibID(Item_To_Complete.BibID);


                // Now, delete all the files here
                all_files = Directory.GetFiles(userInProcessDirectory);
                foreach (string thisFile in all_files)
                {
                    File.Delete(thisFile);
                }

                // Always set the additional work needed flag, to give the builder a  chance to look at it
                SobekCM_Database.Update_Additional_Work_Needed_Flag(Item_To_Complete.Web.ItemID, true, Tracer);

                // Clear any temporarily assigned current project and CompleteTemplate
                RequestSpecificValues.Current_User.Current_Default_Metadata = null;
                RequestSpecificValues.Current_User.Current_Template = null;

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
            Write_Item_Type_Top(Output, currentItem);

            Output.WriteLine("<div id=\"container-inner1000\">");
            Output.WriteLine("<div id=\"pagecontainer\">");

            Output.WriteLine("<div class=\"sbkMySobek_HomeText\">");
            Output.WriteLine("  <br />");

            if (currentProcessStep == 1)
            {
                Output.WriteLine("<script src=\"" + Static_Resources_Gateway.Sobekcm_Metadata_Js + "\" type=\"text/javascript\"></script>");
                Output.WriteLine("<div class=\"sbkMySobek_HomeText\">");
                Output.Write("<h2>Step 1 of " + totalTemplatePages + ": Upload TEI and Confirm Mapping, XSLT, and CSS </h2>");

                Output.WriteLine("<table class=\"sbkMySobek_TemplateTbl\" cellpadding=\"4px\" >");
                Output.WriteLine("  <tr>");
                Output.WriteLine("    <td colspan=\"3\" class=\"sbkMySobek_TemplateTblTitle_first\">New TEI File</td>");
                Output.WriteLine("  </tr>");
                Output.WriteLine("  <tr>");

                string explanation = "You can upload a new TEI file to replace the current TEI file.  Use the button below to upload a new TEI file.";
                if (!String.IsNullOrEmpty(tei_file))
                {
                    string tei_filename = Path.GetFileName(tei_file);
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
                Output.WriteLine("<br />");
                Output.WriteLine("    </td>");
                Output.WriteLine("  </tr>");
                Output.WriteLine("  <tr>");
                Output.WriteLine("    <td colspan=\"3\" class=\"sbkMySobek_TemplateTblTitle_first\">Metadata Mapping</td>");
                Output.WriteLine("  </tr>");
                Output.WriteLine("  <tr>");
                Output.WriteLine("    <td colspan=\"3\" style=\"padding-left:30px;font-style:italic; color:#333; font-size:0.9em;\">Select the metadata mapping file below.  This mapping file will read the header information from your TEI file into the system, to facilitate searching and discovery of this resource.</td>");
                Output.WriteLine("  </tr>");

                Output.WriteLine("  <tr>");
                Output.WriteLine("    <td style=\"width:15px\" > &nbsp;</td>");
                Output.WriteLine("    <td class=\"metadata_label\">Metadata Mapping:</a></td>");
                Output.WriteLine("    <td>");
                Output.WriteLine("      <table>");
                Output.WriteLine("        <tr>");
                Output.WriteLine("          <td>");
                Output.WriteLine("            <div id=\"mapping_div\">");
                Output.WriteLine("              <select class=\"type_select\" name=\"mapping_select\" id=\"mapping_select\" >");

                foreach (string thisSettingKey in RequestSpecificValues.Current_User.Settings.Keys)
                {
                    if (thisSettingKey.IndexOf("TEI.MAPPING.") == 0)
                    {
                        string enabled = RequestSpecificValues.Current_User.Get_Setting(thisSettingKey, "false");
                        if (String.Compare(enabled, "true", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            string file = thisSettingKey.Replace("TEI.MAPPING.", "");
                            if (String.Compare(file, mapping_file, StringComparison.OrdinalIgnoreCase) == 0)
                                Output.WriteLine("              <option value=\"" + file + "\" selected=\"selected\">" + file + "</option>");
                            else
                                Output.WriteLine("              <option value=\"" + file + "\">" + file + "</option>");
                        }
                    }
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

                Output.WriteLine("  <tr>");
                Output.WriteLine("    <td colspan=\"3\" class=\"sbkMySobek_TemplateTblTitle\" style=\"padding-top:25px\">Display Parameters (XSLT and CSS)</td>");
                Output.WriteLine("  </tr>");
                Output.WriteLine("  <tr>");
                Output.WriteLine("    <td colspan=\"3\" style=\"padding-left:30px;font-style:italic; color:#333; font-size:0.9em;\">The values below determine how the TEI will display within this system.  The XSLT will transform your TEI into HTML for display and the CSS file can add additional style to the resulting display.</td>");
                Output.WriteLine("  </tr>");

                Output.WriteLine("  <tr>");
                Output.WriteLine("    <td style=\"width:15px\" > &nbsp;</td>");
                Output.WriteLine("    <td class=\"metadata_label\">XSLT File:</a></td>");
                Output.WriteLine("    <td>");
                Output.WriteLine("      <table>");
                Output.WriteLine("        <tr>");
                Output.WriteLine("          <td>");
                Output.WriteLine("            <div id=\"xslt_div\">");
                Output.WriteLine("              <select class=\"type_select\" name=\"xslt_select\" id=\"xslt_select\" >");

                foreach (string thisSettingKey in RequestSpecificValues.Current_User.Settings.Keys)
                {
                    if (thisSettingKey.IndexOf("TEI.XSLT.") == 0)
                    {
                        string enabled = RequestSpecificValues.Current_User.Get_Setting(thisSettingKey, "false");
                        if (String.Compare(enabled, "true", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            string file = thisSettingKey.Replace("TEI.XSLT.", "");
                            if (String.Compare(file, xslt_file, StringComparison.OrdinalIgnoreCase) == 0)
                                Output.WriteLine("              <option value=\"" + file + "\" selected=\"selected\">" + file + "</option>");
                            else
                                Output.WriteLine("              <option value=\"" + file + "\">" + file + "</option>");
                        }
                    }
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


                Output.WriteLine("  <tr>");
                Output.WriteLine("    <td style=\"width:15px\" > &nbsp;</td>");
                Output.WriteLine("    <td class=\"metadata_label\">CSS File:</a></td>");
                Output.WriteLine("    <td>");
                Output.WriteLine("      <table>");
                Output.WriteLine("        <tr>");
                Output.WriteLine("          <td>");
                Output.WriteLine("            <div id=\"css_div\">");
                Output.WriteLine("              <select class=\"type_select\" name=\"css_select\" id=\"css_select\" >");
                foreach (string thisSettingKey in RequestSpecificValues.Current_User.Settings.Keys)
                {
                    if (thisSettingKey.IndexOf("TEI.CSS.") == 0)
                    {
                        string enabled = RequestSpecificValues.Current_User.Get_Setting(thisSettingKey, "false");
                        if (String.Compare(enabled, "true", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            string file = thisSettingKey.Replace("TEI.CSS.", "");
                            if (String.Compare(file, css_file, StringComparison.OrdinalIgnoreCase) == 0)
                                Output.WriteLine("              <option value=\"" + file + "\" selected=\"selected\">" + file + "</option>");
                            else
                                Output.WriteLine("              <option value=\"" + file + "\">" + file + "</option>");
                        }
                    }
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


            #region Add the CompleteTemplate and surrounding HTML for the CompleteTemplate page step(s)

            if ((currentProcessStep >= 5) && (currentProcessStep <= (completeTemplate.InputPages_Count + 4)))
            {
                Output.WriteLine("<script type=\"text/javascript\" src=\"" + Static_Resources_Gateway.Jquery_Ui_1_10_3_Custom_Js + "\"></script>");

                Output.WriteLine("<div class=\"sbkMySobek_HomeText\">");
                Output.WriteLine("<br />");
                string template_page_title = completeTemplate.InputPages[currentProcessStep - 5].Title;
                if (template_page_title.Length == 0)
                    template_page_title = "Additional Item Description";
                string template_page_instructions = completeTemplate.InputPages[currentProcessStep - 5].Instructions;
                if (template_page_instructions.Length == 0)
                    template_page_instructions = "Enter additional basic information for your new item.";

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
                if (currentProcessStep == completeTemplate.InputPages_Count + 4)
                {
                    next_step = completeTemplate.Upload_Types == CompleteTemplate.Template_Upload_Types.None ? 9 : 8;
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

                string popup_forms = completeTemplate.Render_Template_HTML(Output, item, RequestSpecificValues.Current_Mode.Skin, isMozilla, RequestSpecificValues.Current_User, RequestSpecificValues.Current_Mode.Language, UI_ApplicationCache_Gateway.Translation, RequestSpecificValues.Current_Mode.Base_URL, currentProcessStep - 4);


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

        #region Step 3: Upload Related Files

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
                uploadControl.AllowedFileExtensions = AllowedFileExtensions;
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

        #region Private method for creating a new digital resource

        private void new_item(Custom_Tracer Tracer)
        {
            // Build a new empty METS file
            item = new SobekCM_Item();
            item.Bib_Info.SobekCM_Type_String = String.Empty;

            // Set the source directory first
            item.Source_Directory = userInProcessDirectory;
            string orgcode = RequestSpecificValues.Current_User.Organization_Code;
            if ((orgcode.Length > 0) && (orgcode.ToLower()[0] != 'i'))
            {
                orgcode = "i" + orgcode;
            }

            // Determine if there are multiple source and holdings for this institution
            bool multipleInstitutionsSelectable = false;
            string institutionCode = String.Empty;
            string institutionName = String.Empty;

            foreach (string thisType in UI_ApplicationCache_Gateway.Aggregations.All_Types)
            {
                if (thisType.IndexOf("Institution", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    ReadOnlyCollection<Item_Aggregation_Related_Aggregations> matchingAggr = UI_ApplicationCache_Gateway.Aggregations.Aggregations_By_Type(thisType);
                    foreach (Item_Aggregation_Related_Aggregations thisAggr in matchingAggr)
                    {
                        if (thisAggr.Code.Length > 1)
                        {
                            if (institutionCode.Length > 0)
                            {
                                multipleInstitutionsSelectable = true;
                                break;
                            }
                            if ((thisAggr.Code[0] == 'i') || (thisAggr.Code[0] == 'I'))
                            {
                                institutionCode = thisAggr.Code.Substring(1).ToUpper();
                                institutionName = thisAggr.Name;
                            }
                            else
                            {
                                institutionCode = thisAggr.Code.ToUpper();
                                institutionName = thisAggr.Name;
                            }
                        }
                    }
                }
            }

            // If there is no source code, use the users's code or the only option available
            if (item.Bib_Info.Source.Code.Length == 0)
            {
                // If only one option for the user, assign that
                if ((!multipleInstitutionsSelectable) && (institutionCode.Length > 0))
                {
                    item.Bib_Info.Source.Code = institutionCode;
                    item.Bib_Info.Source.Statement = institutionName;
                }
                else
                {
                    item.Bib_Info.Source.Code = orgcode;
                    item.Bib_Info.Source.Statement = RequestSpecificValues.Current_User.Organization;
                }
            }

            // If there is no holding code, use the user's code or the only option avaiable
            if (item.Bib_Info.Location.Holding_Code.Length == 0)
            {
                if ((!multipleInstitutionsSelectable) && (institutionCode.Length > 0))
                {
                    item.Bib_Info.Location.Holding_Code = institutionCode;
                    item.Bib_Info.Location.Holding_Name = institutionName;
                }
                else
                {
                    item.Bib_Info.Location.Holding_Code = orgcode;
                    item.Bib_Info.Location.Holding_Name = RequestSpecificValues.Current_User.Organization;
                }
            }

            // For testing
            //if (item.Bib_Info.Source.Code.Length == 0)
            //{
            //    item.Bib_Info.Source.Code = "iSD";
            //    item.Bib_Info.Source.Statement = "Testing purposes only";
            //}
            //if (item.Bib_Info.Location.Holding_Code.Length == 0)
            //{
            //    item.Bib_Info.Location.Holding_Code = "iSD";
            //    item.Bib_Info.Location.Holding_Name = "Testing purposes only";
            //}
            item.Bib_Info.Main_Title.Title = "TEI Item";
            item.Bib_Info.Type.MODS_Type = TypeOfResource_MODS_Enum.Mixed_Material;

            // Set some values from the CompleteTemplate
            if (completeTemplate.Include_User_As_Author)
            {
                item.Bib_Info.Main_Entity_Name.Full_Name = RequestSpecificValues.Current_User.Family_Name + ", " + RequestSpecificValues.Current_User.Given_Name;
            }
            item.Behaviors.IP_Restriction_Membership = completeTemplate.Default_Visibility;

            item.VID = "00001";
            item.BibID = "TEMP000001";
            item.METS_Header.Create_Date = DateTime.Now;
            item.METS_Header.Modify_Date = item.METS_Header.Create_Date;
            item.METS_Header.Creator_Individual = RequestSpecificValues.Current_User.Full_Name;
            item.METS_Header.Add_Creator_Org_Notes("Created using online TEI submission form");

            if (RequestSpecificValues.Current_User.Default_Rights.Length > 0)
                item.Bib_Info.Access_Condition.Text = RequestSpecificValues.Current_User.Default_Rights.Replace("[name]", RequestSpecificValues.Current_User.Full_Name).Replace("[year]", DateTime.Now.Year.ToString());
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




