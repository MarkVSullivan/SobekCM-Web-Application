#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Database;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Library.Application_State;
using SobekCM.Library.Navigation;
using SobekCM.Library.Skins;
using SobekCM.Library.Users;
using darrenjohnstone.net.FileUpload;

#endregion

namespace SobekCM.Library.MySobekViewer
{
    /// <summary> 
    /// 
    /// </summary>
    public class File_Management_MySobekViewer : abstract_MySobekViewer
    {
        private DJAccessibleProgressBar DJAccessibleProgrssBar1;
        private DJFileUpload DJFileUpload1;
        private DJUploadController DJUploadController1;
        private readonly Aggregation_Code_Manager codeManager;
        private bool criticalErrorEncountered;
        private readonly string digitalResourceDirectory;
        private readonly Dictionary<string, Wordmark_Icon> iconList;
        private readonly SobekCM_Item item;
        private readonly Item_Lookup_Object itemList;
        private readonly List<string> validationErrors;
        private readonly SobekCM_Skin_Object webSkin;

        #region Constructor

        /// <summary> Constructor for a new instance of the New_Group_And_Item_MySobekViewer class </summary>
        /// <param name="User"> Authenticated user information </param>
        /// <param name="Current_Mode"> Mode / navigation information for the current request</param>
        /// <param name="Current_Item"> Digital resource selected for file management </param>
        /// <param name="Item_List"> Allows individual items to be retrieved by various methods as <see cref="Single_Item"/> objects.</param>
        /// <param name="Code_Manager"> Code manager contains the list of all valid aggregation codes </param>
        /// <param name="HTML_Skin"> HTML Web skin which controls the overall appearance of this digital library </param>
        /// <param name="Icon_Table"> Dictionary of all the wordmark/icons which can be tagged to the items </param>
        /// <param name="Translator"> Language support object which handles simple translational duties </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public File_Management_MySobekViewer(User_Object User,
                                             SobekCM_Navigation_Object Current_Mode,
                                             SobekCM_Item Current_Item,
                                             Item_Lookup_Object Item_List,
                                             Aggregation_Code_Manager Code_Manager,
                                             Dictionary<string, Wordmark_Icon> Icon_Table,
                                             SobekCM_Skin_Object HTML_Skin,
                                             Language_Support_Info Translator,
                                             Custom_Tracer Tracer)
            : base(User)
        {
            Tracer.Add_Trace("File_Management_MySobekViewer.Constructor", String.Empty);


            // Save the parameters
            codeManager = Code_Manager;
            itemList = Item_List;
            iconList = Icon_Table;
            currentMode = Current_Mode;
            webSkin = HTML_Skin;
            this.validationErrors = validationErrors;
            base.Translator = Translator;
            item = Current_Item;
            digitalResourceDirectory = Current_Item.Source_Directory;

            // If the user cannot edit this item, go back
            if (!user.Can_Edit_This_Item(item))
            {
                currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                currentMode.Redirect();
                return;
            }

            // If this is post-back, handle it
            if (currentMode.isPostBack)
            {
                string[] getKeys = HttpContext.Current.Request.Form.AllKeys;
                string file_name_from_keys = String.Empty;
                string label_from_keys = String.Empty;
                foreach (string thisKey in getKeys)
                {
                    if (thisKey.IndexOf("upload_file") == 0)
                    {
                        file_name_from_keys = HttpContext.Current.Request.Form[thisKey];
                    }
                    if (thisKey.IndexOf("upload_label") == 0)
                    {
                        label_from_keys = HttpContext.Current.Request.Form[thisKey];
                    }
                    if ((file_name_from_keys.Length > 0) && (label_from_keys.Length > 0))
                    {
                        HttpContext.Current.Session["file_" + item.Web.ItemID + "_" + file_name_from_keys.Trim()] = label_from_keys.Trim();
                        file_name_from_keys = String.Empty;
                        label_from_keys = String.Empty;
                    }

                    if (thisKey == "url_input")
                    {
                        item.Bib_Info.Location.Other_URL = HttpContext.Current.Request.Form[thisKey];
                    }
                }

                string action = HttpContext.Current.Request.Form["action"];
                if (action == "delete")
                {
                    string filename = HttpContext.Current.Request.Form["phase"];
                    try
                    {
                        if (File.Exists(digitalResourceDirectory + "\\" + filename))
                            File.Delete(digitalResourceDirectory + "\\" + filename);

                        // Forward
                        currentMode.Redirect();
                        return;
                    }
                    catch
                    {
                        // Error was caught during attempted delete
                    }
                }

                if ( action == "next_phase")
                {
                    int phase = Convert.ToInt32(HttpContext.Current.Request.Form["phase"]);
                    switch( phase )
                    {
                        case 2:
                            // Clear all the file keys in the session state
                            List<string> keys = new List<string>();
                            foreach (string thisKey in HttpContext.Current.Session.Keys)
                            {
                                if (thisKey.IndexOf("file_" + item.Web.ItemID + "_") == 0)
                                    keys.Add(thisKey);
                            }
                            foreach (string thisKey in keys)
                            {
                                HttpContext.Current.Session.Remove(thisKey);
                            }

                            // Redirect to the item
                            currentMode.Mode = Display_Mode_Enum.Item_Display;
                            currentMode.Redirect();
                            break;

                        case 9:
                            if (!complete_item_submission(item, null))
                            {
                                // Clear all the file keys in the session state
                                List<string> keys2 = new List<string>();
                                foreach (string thisKey in HttpContext.Current.Session.Keys)
                                {
                                    if (thisKey.IndexOf("file_" + item.Web.ItemID + "_") == 0)
                                        keys2.Add(thisKey);
                                }
                                foreach (string thisKey in keys2)
                                {
                                    HttpContext.Current.Session.Remove(thisKey);
                                }

                                // Also clear the item from the cache
                                MemoryMgmt.Cached_Data_Manager.Remove_Digital_Resource_Object(item.BibID, item.VID, null);

                                // Redirect to the item
                                currentMode.Mode = Display_Mode_Enum.Item_Display;
                                currentMode.Redirect();
                            }
                            break;                         
                    }
                }
            }
        }

        #endregion

        #region Method commpletes the item submission on the way to the congratulations screen

        private bool complete_item_submission(SobekCM_Item Item_To_Complete, Custom_Tracer Tracer )
        {
            // Set an initial flag 
            criticalErrorEncountered = false;

            string[] all_files = Directory.GetFiles(digitalResourceDirectory);
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
                                List<string> newImageGrouping = new List<string> {thisFileInfo.Name};
                                image_files[filename_sans_extension.ToLower()] = newImageGrouping;
                            }
                        }
                    }
                    else
                    {
                        // If this does not match the exclusion regular expression, than add this
                        if ((!Regex.Match(thisFileInfo.Name, SobekCM_Library_Settings.Files_To_Exclude_From_Downloads, RegexOptions.IgnoreCase).Success) && ( String.Compare(thisFileInfo.Name, Item_To_Complete.BibID + "_" + Item_To_Complete.VID + ".html", true ) != 0 ))
                        {
                            // Is this the first image file with this name?
                            if (download_files.ContainsKey(filename_sans_extension.ToLower()))
                            {
                                download_files[filename_sans_extension.ToLower()].Add(thisFileInfo.Name);
                            }
                            else
                            {
                                List<string> newDownloadGrouping = new List<string> {thisFileInfo.Name};
                                download_files[filename_sans_extension.ToLower()] = newDownloadGrouping;
                            }
                        }
                    }
                }
            }

            // This package is good to go, so build it, save, etc...
            try
            {
                // Save the METS file to the database and back to the directory
                Item_To_Complete.Source_Directory = digitalResourceDirectory;

                // Step through and add each file 
                Item_To_Complete.Divisions.Download_Tree.Clear();

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
                        System.IO.FileInfo fileInfo = new System.IO.FileInfo(thisFile);
                        SobekCM.Resource_Object.Divisions.SobekCM_File_Info newFile = new SobekCM.Resource_Object.Divisions.SobekCM_File_Info(fileInfo.Name);
                        string label = fileInfo.Name.Replace(fileInfo.Extension, "");
                        if (System.Web.HttpContext.Current.Session["file_" + item.Web.ItemID + "_" + thisFileKey] != null)
                        {
                            string possible_label = System.Web.HttpContext.Current.Session["file_" + item.Web.ItemID + "_" + thisFileKey].ToString();
                            if (possible_label.Length > 0)
                                label = possible_label;
                        }

                        // Add this file
                        item.Divisions.Physical_Tree.Add_File(newFile, label);

                        // Seperate code for JP2 and JPEG type files
                        string extension = fileInfo.Extension.ToUpper();
                        if (extension.IndexOf("JP2") >= 0)
                        {
                            if (!error_reading_file_occurred)
                            {
                                if (!newFile.Compute_Jpeg2000_Attributes(item.Source_Directory))
                                    error_reading_file_occurred = true;
                            }
                            jp2_added = true;
                        }
                        else
                        {
                            if (!error_reading_file_occurred)
                            {
                                if (!newFile.Compute_Jpeg_Attributes(item.Source_Directory))
                                    error_reading_file_occurred = true;
                            }
                            jpeg_added = true;
                        }
                    }
                }

                // Add the download files next
                foreach(string thisFileKey in download_files.Keys )
                {
                    // Get the list of files
                    List<string> theseFiles = download_files[thisFileKey];

                    // Add each file
                    foreach (string thisFile in theseFiles)
                    {
                        // Create the new file object and compute a label
                        FileInfo fileInfo = new FileInfo(thisFile);
                        SobekCM_File_Info newFile = new SobekCM_File_Info(fileInfo.Name);
                        string label = fileInfo.Name.Replace( fileInfo.Extension, "");
                        if (HttpContext.Current.Session["file_" + item.Web.ItemID + "_" + thisFileKey] != null)
                        {
                            string possible_label = HttpContext.Current.Session["file_" + item.Web.ItemID + "_" + thisFileKey].ToString();
                            if (possible_label.Length > 0)
                                label = possible_label;
                        }

                        // Add this file
                        Item_To_Complete.Divisions.Download_Tree.Add_File(newFile, label);
                    }
                }

                // Add the JPEG2000 and JPEG-specific viewers
                item.Behaviors.Clear_Views();
                if (jpeg_added)
                {
                    item.Behaviors.Add_View(SobekCM.Resource_Object.Behaviors.View_Enum.JPEG);
                }
                if (jp2_added)
                {
                    item.Behaviors.Add_View(SobekCM.Resource_Object.Behaviors.View_Enum.JPEG2000);
                }
                

                // Determine the total size of the package before saving
                string[] all_files_final = Directory.GetFiles(digitalResourceDirectory);
                double size = all_files_final.Aggregate<string, double>(0, (current, thisFile) => current + (((new FileInfo(thisFile)).Length)/1024));
                Item_To_Complete.DiskSize_MB = size;

                // Save to the database
                try
                {
                    SobekCM_Database.Save_Digital_Resource( Item_To_Complete );
                    SobekCM_Database.Save_Behaviors(Item_To_Complete, Item_To_Complete.Behaviors.Text_Searchable, false);
                }
                catch (Exception ee)
                {
                    StreamWriter writer = new StreamWriter(digitalResourceDirectory + "\\exception.txt", false);
                    writer.WriteLine( "ERROR CAUGHT WHILE SAVING DIGITAL RESOURCE");
                    writer.WriteLine( DateTime.Now.ToString());
                    writer.WriteLine();
                    writer.WriteLine( ee.Message );
                    writer.WriteLine( ee.StackTrace );
                    writer.Flush();
                    writer.Close();
                    throw;
                }


                // Assign the file root and assoc file path
                Item_To_Complete.Web.File_Root = Item_To_Complete.BibID.Substring(0, 2) + "\\" + Item_To_Complete.BibID.Substring(2, 2) + "\\" + Item_To_Complete.BibID.Substring(4, 2) + "\\" + Item_To_Complete.BibID.Substring(6, 2) + "\\" + Item_To_Complete.BibID.Substring(8, 2);
                Item_To_Complete.Web.AssocFilePath = Item_To_Complete.Web.File_Root + "\\" + Item_To_Complete.VID + "\\";

                // Create the static html pages
                string base_url = currentMode.Base_URL;
                try
                {
                    Static_Pages_Builder staticBuilder = new Static_Pages_Builder(SobekCM_Library_Settings.System_Base_URL, SobekCM_Library_Settings.Base_Data_Directory, Translator, codeManager, itemList, iconList, webSkin);
                    string filename = digitalResourceDirectory + "\\" + Item_To_Complete.BibID + "_" + Item_To_Complete.VID + ".html";
                    staticBuilder.Create_Item_Citation_HTML(Item_To_Complete, filename, String.Empty);
                }
                catch (Exception ee)
                {
                    string error = ee.Message;
                }

                currentMode.Base_URL = base_url;

                // Save the rest of the metadata
                Item_To_Complete.Save_SobekCM_METS();

                // Finally, set the item for more processing if there were any files
                if (((image_files.Count > 0) || (download_files.Count > 0)) && ( Item_To_Complete.Web.ItemID > 0 ))
                {
                    Database.SobekCM_Database.Update_Additional_Work_Needed_Flag(Item_To_Complete.Web.ItemID, true, Tracer);
                }
            }
            catch (Exception ee)
            {
                validationErrors.Add("Error encountered during item save!");
                validationErrors.Add(ee.ToString().Replace("\r", "<br />"));

                // Set an initial flag 
                criticalErrorEncountered = true;

                string error_body = "<strong>ERROR ENCOUNTERED DURING ONLINE FILE MANAGEMENT</strong><br /><br /><blockquote>Title: " + Item_To_Complete.Bib_Info.Main_Title.Title + "<br />Permanent Link: <a href=\"" + base.currentMode.Base_URL + "/" + Item_To_Complete.BibID + "/" + Item_To_Complete.VID + "\">" + base.currentMode.Base_URL + "/" + Item_To_Complete.BibID + "/" + Item_To_Complete.VID + "</a><br />User: " + user.Full_Name + "<br /><br /></blockquote>" + ee.ToString().Replace("\n", "<br />");
                string error_subject = "Error during file management for '" + Item_To_Complete.Bib_Info.Main_Title.Title + "'";
                string email_to = SobekCM_Library_Settings.System_Error_Email;
                if (email_to.Length == 0)
                    email_to = SobekCM_Library_Settings.System_Email;
                Database.SobekCM_Database.Send_Database_Email(email_to, error_subject, error_body, true, false, -1, -1);
            }


            return criticalErrorEncountered;
        }

        #endregion

        /// <summary> Property indicates the standard navigation to be included at the top of the page by the
        /// main MySobek html subwriter. </summary>
        /// <value> This returns none since this viewer writes all the necessary navigational elements </value>
        /// <remarks> This is set to NONE if the viewer will write its own navigation and ADMIN if the standard
        /// administrative tabs should be included as well.  </remarks>
        public override MySobek_Included_Navigation_Enum Standard_Navigation_Type
        {
            get
            {
                return MySobek_Included_Navigation_Enum.NONE;
            }
        }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <value> This returns the value 'File Management' </value>
        public override string Web_Title
        {
            get
            {
                return "File Management";
            }
        }

        #region Methods to write the HTML directly to the output stream

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This adds the template HTML for step 2 and the congratulations text for step 4 </remarks>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("File_Management_MySobekViewer.Write_HTML", "Add instructions");

            Output.WriteLine("<script src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_metadata.js\" type=\"text/javascript\"></script>");
            Output.WriteLine("<div class=\"SobekHomeText\">");
            Output.WriteLine("<br />");
            Output.WriteLine("<blockquote>Upload the related files for your new item.  You can also provide labels for each file, once they are uploaded.</blockquote><br />"); 
        }

        /// <summary> This is an opportunity to write HTML directly into the main form, without
        /// using the pop-up html form architecture </summary>
        /// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This text will appear within the ItemNavForm form tags </remarks>
        public override void Add_HTML_In_Main_Form(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("File_Management_MySobekViewer.Add_HTML_In_Main_Form", "");
            }

            // Add the hidden fields first
            Output.WriteLine("<!-- Hidden field is used for postbacks to indicate what to save and reset -->");
            Output.WriteLine("<input type=\"hidden\" id=\"action\" name=\"action\" value=\"\" />");
            Output.WriteLine("<input type=\"hidden\" id=\"phase\" name=\"phase\" value=\"\" />");
            Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_metadata.js\" ></script>");
            Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_form.js\" ></script>");

            #region Get the list of all files sans extensions to pages 

            Dictionary<string, string> image_files_to_labels = new Dictionary<string,string>();
            Dictionary<string, string> resource_files_to_labels = new Dictionary<string,string>();
            List<abstract_TreeNode> imagePages = item.Divisions.Physical_Tree.Pages_PreOrder;
            List<abstract_TreeNode> resourcePages = item.Divisions.Download_Tree.Pages_PreOrder;
            foreach( Page_TreeNode thisPage in imagePages )
            {
                if ( thisPage.Files.Count > 0 )
                    image_files_to_labels[thisPage.Files[0].File_Name_Sans_Extension.ToLower()] = thisPage.Label;
            }
            foreach( Page_TreeNode thisPage in resourcePages )
            {
                if ( thisPage.Files.Count > 0 )
                    resource_files_to_labels[thisPage.Files[0].File_Name_Sans_Extension.ToLower()] = thisPage.Label;
            }

            #endregion

            #region Add the list of all existing files and the URL box for the upload file/enter URL step

            string[] all_files = Directory.GetFiles(digitalResourceDirectory);
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

                    // Is this a page image?
                    if ((extension_upper == ".JPG") || (extension_upper == ".TIF") || (extension_upper == ".JP2") || (extension_upper == ".JPX") || ( extension_upper == ".PNG") || ( extension_upper == ".GIF"))
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
                                List<string> newImageGrouping = new List<string> {thisFileInfo.Name};
                                image_files[filename_sans_extension.ToLower()] = newImageGrouping;
                            }
                        }
                    }
                    else
                    {
                        // If this does not match the exclusion regular expression, than add this
                        if ((!Regex.Match(thisFileInfo.Name, SobekCM_Library_Settings.Files_To_Exclude_From_Downloads, RegexOptions.IgnoreCase).Success) && (String.Compare(thisFileInfo.Name, item.BibID + "_" + item.VID + ".html", true) != 0))
                        {
                            // Is this the first image file with this name?
                            if (download_files.ContainsKey(filename_sans_extension.ToLower()))
                            {
                                download_files[filename_sans_extension.ToLower()].Add(thisFileInfo.Name);
                            }
                            else
                            {
                                List<string> newDownloadGrouping = new List<string> {thisFileInfo.Name};
                                download_files[filename_sans_extension.ToLower()] = newDownloadGrouping;
                            }
                        }
                    }
                }
            }

            // Any page images?
            int file_counter = 0;
            if (SobekCM_Library_Settings.Allow_Page_Image_File_Management)
            {
                if (image_files.Count > 0)
                {
                    Output.WriteLine("The following page images are already uploaded for this package:");
                    Output.WriteLine("<blockquote>");
                    Output.WriteLine("<table border=\"0px\" cellspacing=\"0px\">");
                    Output.WriteLine("  <tr align=\"left\" bgcolor=\"#0022a7\" height=\"22px\" >");
                    Output.WriteLine("    <th width=\"100px\" align=\"left\"><span style=\"color: White\">FILENAME</span></th>");
                    Output.WriteLine("    <th width=\"150px\" align=\"left\">&nbsp;</th>");
                    Output.WriteLine("    <th width=\"90px\"><span style=\"color: White\">SIZE</span></th>");
                    Output.WriteLine("    <th width=\"170px\"><span style=\"color: White\">DATE UPLOADED</span></th>");
                    Output.WriteLine("    <th width=\"90px\"><span style=\"color: White\">ACTION</span></th>");
                    Output.WriteLine("  </tr>");

                    // Step through all the page image file groups
                    foreach (string fileKey in image_files.Keys)
                    {
                        // Get this group of files
                        List<string> fileGroup = image_files[fileKey];

                        // Add each individual file
                        foreach (string thisFile in fileGroup)
                        {
                            file_counter++;

                            // Add the file name literal
                            FileInfo fileInfo = new FileInfo(digitalResourceDirectory + "\\" + thisFile);
                            Output.WriteLine("<tr align=\"left\" >");
                            Output.WriteLine("<td colspan=\"2\">" + fileInfo.Name + "</td>");
                            if (fileInfo.Length < 1024)
                                Output.WriteLine("<td>" + fileInfo.Length + "</td>");
                            else
                            {
                                if (fileInfo.Length < (1024 * 1024))
                                    Output.WriteLine("<td>" + (fileInfo.Length / 1024) + " KB</td>");
                                else
                                    Output.WriteLine("<td>" + (fileInfo.Length / (1024 * 1024)) + " MB</td>");
                            }

                            Output.WriteLine("<td>" + fileInfo.LastWriteTime + "</td>");
                            Output.WriteLine("<td></td>");
                        //    Output.WriteLine("<td align=\"center\"> <span class=\"SobekFolderActionLink\">( <a href=\"\" onclick=\"return file_delete('" + fileInfo.Name + "');\">delete</a> )</span></td></tr>");
                        }

                        // Now add the row to include the label
                        string input_name = "upload_label" + file_counter.ToString();
                        Output.WriteLine("<tr><td width=\"120px\" align=\"right\"><span style=\"color:gray\">Label:</span></td><td colspan=\"4\">");
                        Output.WriteLine("<input type=\"hidden\" id=\"upload_file" + file_counter.ToString() + "\" name=\"upload_file" + file_counter.ToString() + "\" value=\"" + fileKey + "\" />");
                        if (HttpContext.Current.Session["file_" + item.Web.ItemID + "_" + fileKey] == null)
                        {
                            if ( image_files_to_labels.ContainsKey( fileKey ))
                            {
                                HttpContext.Current.Session["file_" + item.Web.ItemID + "_" + fileKey] = image_files_to_labels[fileKey];
                                Output.WriteLine("<input type=\"text\" class=\"upload_label_input\" id=\"" + input_name + "\" name=\"" + input_name + "\" value=\"" + HttpUtility.HtmlEncode( image_files_to_labels[fileKey] ) + "\" onfocus=\"javascript:textbox_enter('" + input_name + "', 'upload_label_input_focused')\" onblur=\"javascript:textbox_leave('" + input_name + "', 'upload_label_input')\" ></input>");
                            }
                            else
                            {
                                Output.WriteLine("<input type=\"text\" class=\"upload_label_input\" id=\"" + input_name + "\" name=\"" + input_name + "\" value=\"\" onfocus=\"javascript:textbox_enter('" + input_name + "', 'upload_label_input_focused')\" onblur=\"javascript:textbox_leave('" + input_name + "', 'upload_label_input')\" ></input>");                            
                            }
                        }
                        else
                        {
                            string label_from_session = HttpContext.Current.Session["file_" + item.Web.ItemID + "_" + fileKey].ToString();
                            Output.WriteLine("<input type=\"text\" class=\"upload_label_input\" id=\"" + input_name + "\" name=\"" + input_name + "\" value=\"" + HttpUtility.HtmlEncode( label_from_session ) + "\" onfocus=\"javascript:textbox_enter('" + input_name + "', 'upload_label_input_focused')\" onblur=\"javascript:textbox_leave('" + input_name + "', 'upload_label_input')\" ></input>");
                        }
                        Output.WriteLine("</td></tr>");
                        Output.WriteLine("<tr><td bgcolor=\"#0022a7\" colspan=\"5\"></td></tr>");
                        Output.WriteLine("<tr height=\"6px\"><td colspan=\"5\"></td></tr>");
                    }
                    Output.WriteLine("</table></blockquote><br />");
                }
            }

            // Any download files?
            if (download_files.Count > 0)
            {
                Output.WriteLine("The following files are already uploaded for this package and will be included as downloads:");
                Output.WriteLine("<blockquote>");
                Output.WriteLine("<table border=\"0px\" cellspacing=\"0px\">");
                Output.WriteLine("  <tr align=\"left\" bgcolor=\"#0022a7\" height=\"22px\" >");
                Output.WriteLine("    <th width=\"100px\" align=\"left\"><span style=\"color: White\">FILENAME</span></th>");
                Output.WriteLine("    <th width=\"150px\" align=\"left\">&nbsp;</th>");
                Output.WriteLine("    <th width=\"90px\"><span style=\"color: White\">SIZE</span></th>");
                Output.WriteLine("    <th width=\"170px\"><span style=\"color: White\">DATE UPLOADED</span></th>");
                Output.WriteLine("    <th width=\"90px\"><span style=\"color: White\">ACTION</span></th>");
                Output.WriteLine("  </tr>");

                // Step through all the download file groups
                foreach (string fileKey in download_files.Keys)
                {
                    // Get this group of files
                    List<string> fileGroup = download_files[fileKey];

                    // Add each individual file
                    foreach (string thisFile in fileGroup)
                    {
                        file_counter++;

                        // Add the file name literal
                        FileInfo fileInfo = new FileInfo(digitalResourceDirectory + "\\" + thisFile);
                        Output.WriteLine("<tr align=\"left\" >");
                        Output.WriteLine("<td colspan=\"2\">" + fileInfo.Name + "</td>");
                        if (fileInfo.Length < 1024)
                            Output.WriteLine("<td>" + fileInfo.Length + "</td>");
                        else
                        {
                            if (fileInfo.Length < (1024 * 1024))
                                Output.WriteLine("<td>" + (fileInfo.Length / 1024) + " KB</td>");
                            else
                                Output.WriteLine("<td>" + (fileInfo.Length / (1024 * 1024)) + " MB</td>");
                        }

                        Output.WriteLine("<td>" + fileInfo.LastWriteTime + "</td>");
                        Output.WriteLine("<td align=\"center\"> <span class=\"SobekFolderActionLink\">( <a href=\"\" onclick=\"return file_delete('" + fileInfo.Name + "');\">delete</a> )</span></td></tr>");
                    }

                    // Now add the row to include the label
                    string input_name = "upload_label" + file_counter.ToString();
                    Output.WriteLine("<tr><td width=\"120px\" align=\"right\"><span style=\"color:gray\">Label:</span></td><td colspan=\"4\">");
                    Output.WriteLine("<input type=\"hidden\" id=\"upload_file" + file_counter.ToString() + "\" name=\"upload_file" + file_counter.ToString() + "\" value=\"" + fileKey + "\" />");

                        if (HttpContext.Current.Session["file_" + item.Web.ItemID + "_" + fileKey] == null)
                        {
                            if (  resource_files_to_labels.ContainsKey( fileKey ))
                            {
                                HttpContext.Current.Session["file_" + item.Web.ItemID + "_" + fileKey] = resource_files_to_labels[fileKey];
                                Output.WriteLine("<input type=\"text\" class=\"upload_label_input\" id=\"" + input_name + "\" name=\"" + input_name + "\" value=\"" + HttpUtility.HtmlEncode( resource_files_to_labels[fileKey] ) + "\" onfocus=\"javascript:textbox_enter('" + input_name + "', 'upload_label_input_focused')\" onblur=\"javascript:textbox_leave('" + input_name + "', 'upload_label_input')\" ></input>");
                            }
                            else
                            {
                                Output.WriteLine("<input type=\"text\" class=\"upload_label_input\" id=\"" + input_name + "\" name=\"" + input_name + "\" value=\"\" onfocus=\"javascript:textbox_enter('" + input_name + "', 'upload_label_input_focused')\" onblur=\"javascript:textbox_leave('" + input_name + "', 'upload_label_input')\" ></input>");                            
                            }
                        }
                        else
                        {
                            string label_from_session = HttpContext.Current.Session["file_" + item.Web.ItemID + "_" + fileKey].ToString();
                            Output.WriteLine("<input type=\"text\" class=\"upload_label_input\" id=\"" + input_name + "\" name=\"" + input_name + "\" value=\"" + HttpUtility.HtmlEncode( label_from_session ) + "\" onfocus=\"javascript:textbox_enter('" + input_name + "', 'upload_label_input_focused')\" onblur=\"javascript:textbox_leave('" + input_name + "', 'upload_label_input')\" ></input>");
                        }

                    Output.WriteLine("</td></tr>");
                    Output.WriteLine("<tr><td bgcolor=\"#0022a7\" colspan=\"5\"></td></tr>");
                    Output.WriteLine("<tr height=\"6px\"><td colspan=\"5\"></td></tr>");
                }
                Output.WriteLine("</table></blockquote><br />");
            }

            const string completionMessage = "Once all files are uploaded, press SUBMIT to finish this item.";

            Output.WriteLine("<table width=\"750\">");
            Output.WriteLine("  <tr height=\"40px\" align=\"left\" valign=\"middle\" >");
            Output.WriteLine("    <td height=\"40px\" width=\"450\">" + completionMessage + "</td>");
            Output.WriteLine("    <td height=\"40px\" align=\"right\">");
            Output.WriteLine("      <a href=\"\" onclick=\"return new_upload_next_phase(2);\"><img border=\"0\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/back_button_22.gif\" alt=\"BACK\" /></a>");
            Output.WriteLine("      &nbsp; ");
            Output.WriteLine("      <a href=\"\" onclick=\"return new_upload_next_phase(9);\"><img border=\"0\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/submit_button_22.gif\" alt=\"SUBMIT\" /></a> &nbsp; ");
            Output.WriteLine("    </td>");
            Output.WriteLine("    <td height=\"40px\" width=\"65px\"><div id=\"circular_progress\" name=\"circular_progress\" class=\"hidden_progress\">&nbsp;</div></td>");
            Output.WriteLine("  </tr>");
            Output.WriteLine("</table>");
            Output.WriteLine("<br /><br />");
            Output.WriteLine("</div>");

            #endregion

        }

        #endregion

        /// <summary> Add controls directly to the form, either to the main control area or to the file upload placeholder </summary>
        /// <param name="placeHolder"> Main place holder to which all main controls are added </param>
        /// <param name="uploadFilesPlaceHolder"> Place holder is used for uploading file </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Add_Controls(PlaceHolder placeHolder, PlaceHolder uploadFilesPlaceHolder, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("New_Group_And_Item_MySobekViewer.Add_Controls", String.Empty);

            // Add the upload controls to the file place holder
            add_upload_controls(uploadFilesPlaceHolder, Tracer);
        }

        private void add_upload_controls(PlaceHolder placeHolder, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("New_Group_And_Item_MySobekViewer.add_upload_controls", String.Empty);

            StringBuilder filesBuilder = new StringBuilder(2000);
            filesBuilder.AppendLine("<script src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_metadata.js\" type=\"text/javascript\"></script>");
            filesBuilder.AppendLine("Add a new file for this package:");
            filesBuilder.AppendLine("<blockquote>");

            LiteralControl filesLiteral2 = new LiteralControl(filesBuilder.ToString());
            placeHolder.Controls.Add(filesLiteral2);
            filesBuilder.Remove(0, filesBuilder.Length);

            DJUploadController1 = new DJUploadController
                                      {
                                          CSSPath = currentMode.Base_URL + "default/scripts/upload_styles",
                                          ImagePath = currentMode.Base_URL + "default/scripts/upload_images",
                                          ScriptPath = currentMode.Base_URL + "default/scripts/upload_scripts"
                                      };
            placeHolder.Controls.Add(DJUploadController1);

            DJAccessibleProgrssBar1 = new DJAccessibleProgressBar();
            placeHolder.Controls.Add(DJAccessibleProgrssBar1);

            DJFileUpload1 = new DJFileUpload {ShowAddButton = false, ShowUploadButton = true, MaxFileUploads = 1};
            placeHolder.Controls.Add(DJFileUpload1);

            // Set the default processor
            FileSystemProcessor fs = new FileSystemProcessor {OutputPath = digitalResourceDirectory};
            DJUploadController1.DefaultFileProcessor = fs;

            // Change the file processor and set it's properties.
            FieldTestProcessor fsd = new FieldTestProcessor {OutputPath = digitalResourceDirectory};
            DJFileUpload1.FileProcessor = fsd;

            filesBuilder.AppendLine("</blockquote><br />");

            LiteralControl literal1 = new LiteralControl(filesBuilder.ToString());
            placeHolder.Controls.Add(literal1);
        }
    }
}
  


