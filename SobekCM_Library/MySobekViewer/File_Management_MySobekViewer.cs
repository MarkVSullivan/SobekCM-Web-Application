#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SobekCM.Core.MemoryMgmt;
using SobekCM.Core.Navigation;
using SobekCM.Engine_Library.Email;
using SobekCM.Engine_Library.Navigation;
using SobekCM.Library.HTML;
using SobekCM.Library.Settings;
using SobekCM.Library.UploadiFive;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Database;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Tools;
using SobekCM.UI_Library;

#endregion

namespace SobekCM.Library.MySobekViewer
{
    /// <summary> 
    /// 
    /// </summary>
    public class File_Management_MySobekViewer : abstract_MySobekViewer
    {
        private bool criticalErrorEncountered;
        private readonly string digitalResourceDirectory;


        #region Constructor

        /// <summary> Constructor for a new instance of the New_Group_And_Item_MySobekViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        public File_Management_MySobekViewer(RequestCache RequestSpecificValues) : base(RequestSpecificValues)
        {
            RequestSpecificValues.Tracer.Add_Trace("File_Management_MySobekViewer.Constructor", String.Empty);

            digitalResourceDirectory = RequestSpecificValues.Current_Item.Source_Directory;

            // If the user cannot edit this RequestSpecificValues.Current_Item, go back
            if (!RequestSpecificValues.Current_User.Can_Edit_This_Item(RequestSpecificValues.Current_Item.BibID, RequestSpecificValues.Current_Item.Bib_Info.SobekCM_Type_String, RequestSpecificValues.Current_Item.Bib_Info.Source.Code, RequestSpecificValues.Current_Item.Bib_Info.HoldingCode, RequestSpecificValues.Current_Item.Behaviors.Aggregation_Code_List))
            {
                RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                return;
            }

            // If this is post-back, handle it
            if (RequestSpecificValues.Current_Mode.isPostBack)
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
                        HttpContext.Current.Session["file_" + RequestSpecificValues.Current_Item.Web.ItemID + "_" + file_name_from_keys.Trim()] = label_from_keys.Trim();
                        file_name_from_keys = String.Empty;
                        label_from_keys = String.Empty;
                    }

                    if (thisKey == "url_input")
                    {
                        RequestSpecificValues.Current_Item.Bib_Info.Location.Other_URL = HttpContext.Current.Request.Form[thisKey];
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
                        UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
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
                            List<string> keys = HttpContext.Current.Session.Keys.Cast<string>().Where(ThisKey => ThisKey.IndexOf("file_" + RequestSpecificValues.Current_Item.Web.ItemID + "_") == 0).ToList();
		                    foreach (string thisKey in keys)
                            {
                                HttpContext.Current.Session.Remove(thisKey);
                            }

                            // Redirect to the RequestSpecificValues.Current_Item
                            RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Item_Display;
                            UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                            break;

                        case 9:
                            if (!complete_item_submission(RequestSpecificValues.Current_Item, null))
                            {
                                // Clear all the file keys in the session state
                                List<string> keys2 = HttpContext.Current.Session.Keys.Cast<string>().Where(ThisKey => ThisKey.IndexOf("file_" + RequestSpecificValues.Current_Item.Web.ItemID + "_") == 0).ToList();
	                            foreach (string thisKey in keys2)
                                {
                                    HttpContext.Current.Session.Remove(thisKey);
                                }

                                // Also clear the RequestSpecificValues.Current_Item from the cache
                                CachedDataManager.Remove_Digital_Resource_Object(RequestSpecificValues.Current_Item.BibID, RequestSpecificValues.Current_Item.VID, null);

                                // Redirect to the RequestSpecificValues.Current_Item
                                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Item_Display;
                                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
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

                if ((thisFileInfo.Name.IndexOf("agreement.txt") != 0) && (thisFileInfo.Name.IndexOf("TEMP000001_00001.mets") != 0) && (thisFileInfo.Name.IndexOf("doc.xml") != 0) && (thisFileInfo.Name.IndexOf("marc.xml") != 0))
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
                        if ((!Regex.Match(thisFileInfo.Name, UI_ApplicationCache_Gateway.Settings.Files_To_Exclude_From_Downloads, RegexOptions.IgnoreCase).Success) && ( String.Compare(thisFileInfo.Name, Item_To_Complete.BibID + "_" + Item_To_Complete.VID + ".html", StringComparison.OrdinalIgnoreCase) != 0 ))
                        {
							// Also, exclude files that are .XML and marc.xml, or doc.xml, or have the bibid in the name
							if ((thisFileInfo.Name.IndexOf("marc.xml", StringComparison.OrdinalIgnoreCase) != 0) && (thisFileInfo.Name.IndexOf("marc.xml", StringComparison.OrdinalIgnoreCase) != 0) && (thisFileInfo.Name.IndexOf(".mets", StringComparison.OrdinalIgnoreCase) < 0) && (thisFileInfo.Name.IndexOf("citation_mets.xml", StringComparison.OrdinalIgnoreCase) < 0) &&
								((thisFileInfo.Name.IndexOf(".xml", StringComparison.OrdinalIgnoreCase) < 0) || (thisFileInfo.Name.IndexOf( Item_To_Complete.BibID, StringComparison.OrdinalIgnoreCase) < 0)))
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
            }

            // This package is good to go, so build it, save, etc...
            try
            {
                // Save the METS file to the database and back to the directory
                Item_To_Complete.Source_Directory = digitalResourceDirectory;

                // Step through and add each file 
                Item_To_Complete.Divisions.Download_Tree.Clear();

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
                        if (HttpContext.Current.Session["file_" + RequestSpecificValues.Current_Item.Web.ItemID + "_" + thisFileKey] != null)
                        {
                            string possible_label = HttpContext.Current.Session["file_" + RequestSpecificValues.Current_Item.Web.ItemID + "_" + thisFileKey].ToString();
                            if (possible_label.Length > 0)
                                label = possible_label;
                        }

                        // Add this file
                        Item_To_Complete.Divisions.Download_Tree.Add_File(newFile, label);
                    }
                }

                // Determine the total size of the package before saving
                string[] all_files_final = Directory.GetFiles(digitalResourceDirectory);
                double size = all_files_final.Aggregate<string, double>(0, (Current, ThisFile) => Current + (((new FileInfo(ThisFile)).Length)/1024));
                Item_To_Complete.DiskSize_KB = size;

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
                options["MarcXML_File_ReaderWriter:System Name"] = UI_ApplicationCache_Gateway.Settings.System_Name;
                options["MarcXML_File_ReaderWriter:System Abbreviation"] = UI_ApplicationCache_Gateway.Settings.System_Abbreviation;


                // Save to the database
                try
                {
                    SobekCM_Database.Save_Digital_Resource( Item_To_Complete, options  );
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

                //// Create the static html pages
                //string base_url = RequestSpecificValues.Current_Mode.Base_URL;
                //try
                //{
                //    Static_Pages_Builder staticBuilder = new Static_Pages_Builder(UI_ApplicationCache_Gateway.Settings.System_Base_URL, UI_ApplicationCache_Gateway.Settings.Base_Data_Directory, RequestSpecificValues.HTML_Skin.Skin_Code);
                //    if (!Directory.Exists(digitalResourceDirectory + "\\" + UI_ApplicationCache_Gateway.Settings.Backup_Files_Folder_Name))
                //        Directory.CreateDirectory(digitalResourceDirectory + "\\" + UI_ApplicationCache_Gateway.Settings.Backup_Files_Folder_Name);
                //    string filename = digitalResourceDirectory + "\\" + UI_ApplicationCache_Gateway.Settings.Backup_Files_Folder_Name + "\\" + Item_To_Complete.BibID + "_" + Item_To_Complete.VID + ".html";
                //    staticBuilder.Create_Item_Citation_HTML(Item_To_Complete, filename, String.Empty);

                //    // Copy the static HTML file to the web server
                //    try
                //    {
                //        if (!Directory.Exists(UI_ApplicationCache_Gateway.Settings.Static_Pages_Location + RequestSpecificValues.Current_Item.BibID.Substring(0, 2) + "\\" + RequestSpecificValues.Current_Item.BibID.Substring(2, 2) + "\\" + RequestSpecificValues.Current_Item.BibID.Substring(4, 2) + "\\" + RequestSpecificValues.Current_Item.BibID.Substring(6, 2) + "\\" + RequestSpecificValues.Current_Item.BibID.Substring(8)))
                //            Directory.CreateDirectory(UI_ApplicationCache_Gateway.Settings.Static_Pages_Location + RequestSpecificValues.Current_Item.BibID.Substring(0, 2) + "\\" + RequestSpecificValues.Current_Item.BibID.Substring(2, 2) + "\\" + RequestSpecificValues.Current_Item.BibID.Substring(4, 2) + "\\" + RequestSpecificValues.Current_Item.BibID.Substring(6, 2) + "\\" + RequestSpecificValues.Current_Item.BibID.Substring(8));
                //        if (File.Exists(filename))
                //            File.Copy(filename, UI_ApplicationCache_Gateway.Settings.Static_Pages_Location + RequestSpecificValues.Current_Item.BibID.Substring(0, 2) + "\\" + RequestSpecificValues.Current_Item.BibID.Substring(2, 2) + "\\" + RequestSpecificValues.Current_Item.BibID.Substring(4, 2) + "\\" + RequestSpecificValues.Current_Item.BibID.Substring(6, 2) + "\\" + RequestSpecificValues.Current_Item.BibID.Substring(8) + "\\" + RequestSpecificValues.Current_Item.BibID + "_" + RequestSpecificValues.Current_Item.VID + ".html", true);
                //    }
                //    catch (Exception)
                //    {
                //        // This is not critical
                //    }
                //}
                //catch (Exception)
                //{
                //}

                //RequestSpecificValues.Current_Mode.Base_URL = base_url;

                // Save the rest of the metadata
                Item_To_Complete.Save_SobekCM_METS();

                // Finally, set the RequestSpecificValues.Current_Item for more processing if there were any files
                if (((image_files.Count > 0) || (download_files.Count > 0)) && ( Item_To_Complete.Web.ItemID > 0 ))
                {
                    Database.SobekCM_Database.Update_Additional_Work_Needed_Flag(Item_To_Complete.Web.ItemID, true, Tracer);
                }
            }
            catch (Exception ee)
            {
                // Set an initial flag 
                criticalErrorEncountered = true;

                string error_body = "<strong>ERROR ENCOUNTERED DURING ONLINE FILE MANAGEMENT</strong><br /><br /><blockquote>Title: " + Item_To_Complete.Bib_Info.Main_Title.Title + "<br />Permanent Link: <a href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "/" + Item_To_Complete.BibID + "/" + Item_To_Complete.VID + "\">" + RequestSpecificValues.Current_Mode.Base_URL + "/" + Item_To_Complete.BibID + "/" + Item_To_Complete.VID + "</a><br />User: " + RequestSpecificValues.Current_User.Full_Name + "<br /><br /></blockquote>" + ee.ToString().Replace("\n", "<br />");
                string error_subject = "Error during file management for '" + Item_To_Complete.Bib_Info.Main_Title.Title + "'";
                string email_to = UI_ApplicationCache_Gateway.Settings.System_Error_Email;
                if (email_to.Length == 0)
                    email_to = UI_ApplicationCache_Gateway.Settings.System_Email;
                Email_Helper.SendEmail(email_to, error_subject, error_body, true, String.Empty);
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

            Output.WriteLine("<script src=\"" + Static_Resources.Sobekcm_Metadata_Js + "\" type=\"text/javascript\"></script>");

			// Write the top RequestSpecificValues.Current_Item mimic html portion
			Write_Item_Type_Top(Output, RequestSpecificValues.Current_Item);

			Output.WriteLine("<div id=\"container-inner1000\">");
			Output.WriteLine("<div id=\"pagecontainer\">");

			Output.WriteLine("<div class=\"sbkMySobek_HomeText\">");
			Output.WriteLine("  <br />");

            Output.WriteLine("  <h2>Manage Downloads</h2>");
            Output.WriteLine("  <p>Upload the download files for your item.  You can also provide labels for each file, once they are uploaded.</p>");
            
            RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Page_Images_Management;
            Output.WriteLine("  <p><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Click here to upload page images instead.</a></p>");
            RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.File_Management;

			Output.WriteLine("  <br />");

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
                Tracer.Add_Trace("File_Managament_MySobekViewer.Write_ItemNavForm_Closing", "");
            }

            // Add the hidden fields first
            Output.WriteLine("<!-- Hidden field is used for postbacks to indicate what to save and reset -->");
            Output.WriteLine("<input type=\"hidden\" id=\"action\" name=\"action\" value=\"\" />");
            Output.WriteLine("<input type=\"hidden\" id=\"phase\" name=\"phase\" value=\"\" />");
            Output.WriteLine("<script type=\"text/javascript\" src=\"" + Static_Resources.Sobekcm_Metadata_Js + "\" ></script>");

            Output.WriteLine("<hr />");
            Output.WriteLine("<br />");

            #region Get the list of all files sans extensions to pages 

            Dictionary<string, string> image_files_to_labels = new Dictionary<string,string>();
            Dictionary<string, string> resource_files_to_labels = new Dictionary<string,string>();
            List<abstract_TreeNode> imagePages = RequestSpecificValues.Current_Item.Divisions.Physical_Tree.Pages_PreOrder;
            List<abstract_TreeNode> resourcePages = RequestSpecificValues.Current_Item.Divisions.Download_Tree.Pages_PreOrder;
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

                if ((thisFileInfo.Name.IndexOf("agreement.txt") != 0) && (thisFileInfo.Name.IndexOf("TEMP000001_00001.mets") != 0) && (thisFileInfo.Name.IndexOf("doc.xml") != 0) && (thisFileInfo.Name.IndexOf("marc.xml") != 0))
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
                        if ((!Regex.Match(thisFileInfo.Name, UI_ApplicationCache_Gateway.Settings.Files_To_Exclude_From_Downloads, RegexOptions.IgnoreCase).Success) && (String.Compare(thisFileInfo.Name, RequestSpecificValues.Current_Item.BibID + "_" + RequestSpecificValues.Current_Item.VID + ".html", StringComparison.OrdinalIgnoreCase) != 0))
                        {
							if ((thisFileInfo.Name.IndexOf("marc.xml", StringComparison.OrdinalIgnoreCase) != 0) && (thisFileInfo.Name.IndexOf("marc.xml", StringComparison.OrdinalIgnoreCase) != 0) && (thisFileInfo.Name.IndexOf(".mets", StringComparison.OrdinalIgnoreCase) < 0) && (thisFileInfo.Name.IndexOf("citation_mets.xml", StringComparison.OrdinalIgnoreCase) < 0) && (thisFileInfo.Name.IndexOf("_ingest.xml", StringComparison.OrdinalIgnoreCase) < 0) &&
	                            ((thisFileInfo.Name.IndexOf(".xml", StringComparison.OrdinalIgnoreCase) < 0) || (thisFileInfo.Name.IndexOf(RequestSpecificValues.Current_Item.BibID, StringComparison.OrdinalIgnoreCase) < 0)))
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
            }

            // Any page images?
            int file_counter = 0;

            // Any download files?
            if (download_files.Count > 0)
            {
                Output.WriteLine("The following files are already uploaded for this package and will be included as downloads:");
				Output.WriteLine("<table class=\"sbkMySobek_FileTable\">");
                Output.WriteLine("  <tr style=\"min-height:22px;\" >");
                Output.WriteLine("    <th style=\"width:350px;\">FILENAME</th>");
                Output.WriteLine("    <th style=\"width:90px\">SIZE</th>");
                Output.WriteLine("    <th style=\"width:170px\">DATE UPLOADED</th>");
                Output.WriteLine("    <th style=\"width:90px; text-align: center;\">ACTION</th>");
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
			            Output.WriteLine("  <tr style=\"min-height:22px; vertical-align: bottom\" >");
			            Output.WriteLine("    <td>" + fileInfo.Name + "</td>");
			            if (fileInfo.Length < 1024)
				            Output.WriteLine("    <td>" + fileInfo.Length + "</td>");
			            else
			            {
				            if (fileInfo.Length < (1024*1024))
					            Output.WriteLine("    <td>" + (fileInfo.Length/1024) + " KB</td>");
				            else
					            Output.WriteLine("    <td>" + (fileInfo.Length/(1024*1024)) + " MB</td>");
			            }

			            Output.WriteLine("    <td>" + fileInfo.LastWriteTime + "</td>");

                        //add by Keven:replace single & double quote with ascII characters
                        string strFileName = fileInfo.Name;
                        if (strFileName.Contains("'") || strFileName.Contains("\""))
                        {
                            strFileName = strFileName.Replace("'", "\\&#39;");
                            strFileName = strFileName.Replace("\"", "\\&#34;");
                        }
                        Output.WriteLine("    <td style=\"text-align:center\"> <span class=\"sbkMySobek_ActionLink\">( <a href=\"\" onclick=\"return file_delete('" + strFileName + "');\">delete</a> )</span></td>");

			            Output.WriteLine("  </tr>");
		            }

		            // Now add the row to include the label
		            string input_name = "upload_label" + file_counter.ToString();
		            Output.WriteLine("  <tr style=\"min-height: 30px;\">");
		            Output.WriteLine("    <td colspan=\"4\">");
		            Output.WriteLine("      <div style=\"padding-left: 90px;\">");
					Output.WriteLine("        <span style=\"color:gray\">Label:</span>");
		            Output.WriteLine("        <input type=\"hidden\" id=\"upload_file" + file_counter.ToString() + "\" name=\"upload_file" + file_counter.ToString() + "\" value=\"" + fileKey + "\" />");

		            if (HttpContext.Current.Session["file_" + RequestSpecificValues.Current_Item.Web.ItemID + "_" + fileKey] == null)
		            {
			            if (resource_files_to_labels.ContainsKey(fileKey))
			            {
				            HttpContext.Current.Session["file_" + RequestSpecificValues.Current_Item.Web.ItemID + "_" + fileKey] = resource_files_to_labels[fileKey];
							Output.WriteLine("        <input type=\"text\" class=\"upload_label_input sbk_Focusable\" id=\"" + input_name + "\" name=\"" + input_name + "\" value=\"" + HttpUtility.HtmlEncode(resource_files_to_labels[fileKey]) + "\" ></input>");
			            }
			            else
			            {
							Output.WriteLine("        <input type=\"text\" class=\"upload_label_input sbk_Focusable\" id=\"" + input_name + "\" name=\"" + input_name + "\" value=\"\" ></input>");
			            }
		            }
		            else
		            {
			            string label_from_session = HttpContext.Current.Session["file_" + RequestSpecificValues.Current_Item.Web.ItemID + "_" + fileKey].ToString();
						Output.WriteLine("        <input type=\"text\" class=\"upload_label_input sbk_Focusable\" id=\"" + input_name + "\" name=\"" + input_name + "\" value=\"" + HttpUtility.HtmlEncode(label_from_session) + "\" ></input>");
		            }

					Output.WriteLine("      </div>");
		            Output.WriteLine("    </td>");
					Output.WriteLine("  </tr>");
		            Output.WriteLine("  <tr><td class=\"sbkMySobek_FileTableRule\" colspan=\"4\"></td></tr>");
	            }
	            Output.WriteLine("</table>");
				Output.WriteLine();
            }

			Output.WriteLine("<div class=\"sbkMySobek_FileRightButtons\">");
			Output.WriteLine("  <button title=\"Cancel this and remove the recently uploaded files\" onclick=\"return new_upload_next_phase(2);\" class=\"sbkPiu_RoundButton\">CANCEL</button> &nbsp; ");
			Output.WriteLine("  <button title=\"Submit the recently uploaded files and complete the process\" onclick=\"return new_upload_next_phase(9);\" class=\"sbkPiu_RoundButton\">SUBMIT</button> &nbsp; ");
			Output.WriteLine("  <div id=\"circular_progress\" name=\"circular_progress\" class=\"hidden_progress\">&nbsp;</div>");
			Output.WriteLine("</div>");
			Output.WriteLine();

            const string COMPLETION_MESSAGE = "Once all files are uploaded, press SUBMIT to finish this item.";

            Output.WriteLine("<div class=\"sbkMySobek_FileCompletionMsg\">" + COMPLETION_MESSAGE + "</div>");
			Output.WriteLine();

            Output.WriteLine("<br /><br />");
            Output.WriteLine("<hr />");
            Output.WriteLine("<br />");
            Output.WriteLine("The following extensions are accepted:");
            Output.WriteLine("<blockquote>");
            Output.WriteLine(UI_ApplicationCache_Gateway.Settings.Upload_File_Types.Replace(",",", "));
            Output.WriteLine("</blockquote>");

            Output.WriteLine("</div>");
			Output.WriteLine("</div>");
			Output.WriteLine("</div>");

            #endregion

        }

        #endregion

		/// <summary> Add controls directly to the form in the main control area placeholder </summary>
        /// <param name="MainPlaceHolder"> Main place holder to which all main controls are added </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Add_Controls(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("File_Managament_MySobekViewer.Add_Controls", String.Empty);

            // Add the upload controls to the file place holder
            add_upload_controls(MainPlaceHolder, Tracer);
        }

        private void add_upload_controls(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("File_Managament_MySobekViewer.add_upload_controls", String.Empty);

            StringBuilder filesBuilder = new StringBuilder(2000);
            filesBuilder.AppendLine("<script src=\"" + Static_Resources.Sobekcm_Metadata_Js + "\" type=\"text/javascript\"></script>");
            filesBuilder.AppendLine("Add a new file for this package:");
            filesBuilder.AppendLine("<blockquote>");

            LiteralControl filesLiteral2 = new LiteralControl(filesBuilder.ToString());
            MainPlaceHolder.Controls.Add(filesLiteral2);
            filesBuilder.Remove(0, filesBuilder.Length);

			UploadiFiveControl uploadControl = new UploadiFiveControl();
			uploadControl.UploadPath = digitalResourceDirectory;
	        uploadControl.UploadScript = RequestSpecificValues.Current_Mode.Base_URL + "UploadiFiveFileHandler.ashx";
			uploadControl.SubmitWhenQueueCompletes = true;
	        uploadControl.RemoveCompleted = true;
            uploadControl.Swf = Static_Resources.Uploadify_Swf; 
			uploadControl.RevertToFlashVersion = true;
	        uploadControl.AllowedFileExtensions = UI_ApplicationCache_Gateway.Settings.Upload_File_Types;
			MainPlaceHolder.Controls.Add(uploadControl);


            filesBuilder.AppendLine("</blockquote><br />");

            LiteralControl literal1 = new LiteralControl(filesBuilder.ToString());
            MainPlaceHolder.Controls.Add(literal1);
        }

		/// <summary> Gets the collection of special behaviors which this admin or mySobek viewer
		/// requests from the main HTML subwriter. </summary>
		/// <value> This tells the HTML and mySobek writers to mimic the RequestSpecificValues.Current_Item viewer </value>
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
  


