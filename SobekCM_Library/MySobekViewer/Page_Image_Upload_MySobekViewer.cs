#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SobekCM.Library.HTML;
using SobekCM.Library.MemoryMgmt;
using SobekCM.Library.Settings;
using SobekCM.Library.UploadiFive;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Behaviors;
using SobekCM.Resource_Object.Database;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Library.Application_State;
using SobekCM.Library.Navigation;
using SobekCM.Library.Skins;
using SobekCM.Library.Users;

#endregion

namespace SobekCM.Library.MySobekViewer
{


    public class Page_Image_Upload_MySobekViewer : abstract_MySobekViewer
    {
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
        public Page_Image_Upload_MySobekViewer(User_Object User,
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
            Tracer.Add_Trace("Page_Image_Upload_MySobekViewer.Constructor", String.Empty);

            // Save the parameters
            codeManager = Code_Manager;
            itemList = Item_List;
            iconList = Icon_Table;
            currentMode = Current_Mode;
            webSkin = HTML_Skin;
            this.validationErrors = validationErrors;
            base.Translator = Translator;
            item = Current_Item;


            // If the user cannot edit this item, go back
            if (!user.Can_Edit_This_Item(item))
            {
                currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                currentMode.Redirect();
                return;
            }

            // Determine the in process directory for this
            digitalResourceDirectory = SobekCM_Library_Settings.In_Process_Submission_Location + "\\" + User.UserName.Replace(".", "").Replace("@", "") + "\\uploadimages\\" + Current_Item.METS_Header.ObjectID;
            if (User.ShibbID.Trim().Length > 0)
                digitalResourceDirectory = SobekCM_Library_Settings.In_Process_Submission_Location + "\\" + User.ShibbID + "\\uploadimages\\" + Current_Item.METS_Header.ObjectID;

            // Make the folder for the user in process directory
            if (!Directory.Exists(digitalResourceDirectory))
                Directory.CreateDirectory(digitalResourceDirectory);
            else
            {
				// Any post-processing to do?
				string[] files = Directory.GetFiles(digitalResourceDirectory);
				foreach (string thisFile in files)
				{
					FileInfo thisFileInfo = new FileInfo(thisFile);
					if ((thisFileInfo.Extension.ToUpper() == ".TIF") || (thisFileInfo.Extension.ToUpper() == ".TIFF"))
					{
						// Is there a JPEG and/or thumbnail?
						string jpeg = digitalResourceDirectory + "\\" + thisFileInfo.Name.Replace(thisFileInfo.Extension, "") + ".jpg";
						string jpeg_thumbnail = digitalResourceDirectory + "\\" + thisFileInfo.Name.Replace(thisFileInfo.Extension, "") + "thm.jpg";

						// Is one missing?
						if ((!File.Exists(jpeg)) || (!File.Exists(jpeg_thumbnail)))
						{
							try
							{
								var tiffImg = System.Drawing.Image.FromFile(thisFile);
								var mainImg = ScaleImage(tiffImg, SobekCM_Library_Settings.JPEG_Width, SobekCM_Library_Settings.JPEG_Height);
								mainImg.Save(jpeg, ImageFormat.Jpeg);
								var thumbnailImg = ScaleImage(tiffImg, 150, 400);
								thumbnailImg.Save(jpeg_thumbnail, ImageFormat.Jpeg);

							}
							catch (Exception)
							{
								bool error = true;
							}
						}
					}
				}
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
                            // Clear all the file keys in the temporary folder
                            string[] allFiles = Directory.GetFiles(digitalResourceDirectory);
                            foreach (string thisFile in allFiles)
                            {
                                try
                                {
                                    File.Delete(thisFile);
                                }
                                catch
                                {
                                    // Do nothing - not a fatal problem
                                }
                            }

                            try
                            {
                                Directory.Delete(digitalResourceDirectory);
                            }
                            catch
                            {
                                // Do nothing - not a fatal problem
                            }

                            // Redirect to the item
                            currentMode.Mode = Display_Mode_Enum.Item_Display;
                            currentMode.Redirect();
                            break;

                        case 9:
                            if (!complete_item_submission(item, null))
                            {
                                // Also clear the item from the cache
                                Cached_Data_Manager.Remove_Digital_Resource_Object(item.BibID, item.VID, null);

                                // Redirect to the item
                                currentMode.Mode = Display_Mode_Enum.Item_Display;
                                currentMode.ViewerCode = "qc";
                                currentMode.Redirect();
                            }
                            break;                         
                    }
                }
            }
        } 

		/// <summary> Scales an existing SourceImage to a new max width / max height </summary>
		/// <param name="SourceImage"> Source image </param>
		/// <param name="MaxWidth"> Maximum width for the new image </param>
		/// <param name="maxHeight"> Maximum height for the new image </param>
		/// <returns> Newly scaled image, without changing the original source image </returns>
		public static System.Drawing.Image ScaleImage(System.Drawing.Image SourceImage, int MaxWidth, int maxHeight)
		{
			var ratioX = (double)MaxWidth / SourceImage.Width;
			var ratioY = (double)maxHeight / SourceImage.Height;
			var ratio = Math.Min(ratioX, ratioY);

			var newWidth = (int)(SourceImage.Width * ratio);
			var newHeight = (int)(SourceImage.Height * ratio);

			var newImage = new Bitmap(newWidth, newHeight);
			Graphics.FromImage(newImage).DrawImage(SourceImage, 0, 0, newWidth, newHeight);
			return newImage;
		}

        #endregion

        #region Method commpletes the item submission on the way to the congratulations screen

        private bool complete_item_submission(SobekCM_Item Item_To_Complete, Custom_Tracer Tracer )
        {
            // Set an initial flag 
            criticalErrorEncountered = false;

            string final_destination = Item_To_Complete.Source_Directory;


            string[] image_files = Directory.GetFiles(digitalResourceDirectory);
            

            // This package is good to go, so build it, save, etc...
            try
            {
                // Step through each file
                bool error_reading_file_occurred = false;

                // Add the SourceImage files first
                bool jpeg_added = false;
                bool jp2_added = false;
                foreach (string thisFile in image_files)
                {
                    // Create the new file object and compute a label
                    FileInfo fileInfo = new System.IO.FileInfo(thisFile);
                    SobekCM_File_Info newFile = new SobekCM_File_Info(fileInfo.Name);

                    // Copy this file
                    if (File.Exists(final_destination + "\\" + fileInfo.Name))
                    {
                        File.Copy(thisFile, final_destination + "\\" + fileInfo.Name, true);
                    }
                    else
                    {
                        File.Copy(thisFile, final_destination + "\\" + fileInfo.Name, true);
                        item.Divisions.Physical_Tree.Add_File(newFile, "New Page");


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
                        else if (extension.IndexOf("JPG") >= 0)
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

				// Add the JPEG2000 and JPEG-specific viewers
				//item.Behaviors.Clear_Views();
				if (jpeg_added) 
				{
					// Is a JPEG view already existing?
					bool jpeg_viewer_already_exists = false;
					foreach (View_Object thisViewer in item.Behaviors.Views)
					{
						if (thisViewer.View_Type == View_Enum.JPEG)
						{
							jpeg_viewer_already_exists = true;
							break;
						}
					}

					// Add the JPEG view if it did not already exists
					if ( !jpeg_viewer_already_exists )
						item.Behaviors.Add_View(View_Enum.JPEG);
				}

				// If a JPEG2000 file was just added, ensure it exists as a view for this item
				if (jp2_added)
				{
					// Is a JPEG view already existing?
					bool jpg2000_viewer_already_exists = false;
					foreach (View_Object thisViewer in item.Behaviors.Views)
					{
						if (thisViewer.View_Type == View_Enum.JPEG2000 )
						{
							jpg2000_viewer_already_exists = true;
							break;
						}
					}

					// Add the JPEG2000 view if it did not already exists
					if (!jpg2000_viewer_already_exists)
						item.Behaviors.Add_View(View_Enum.JPEG2000);
				}

                // Determine the total size of the package before saving
                string[] all_files_final = Directory.GetFiles(final_destination);
                double size = all_files_final.Aggregate<string, double>(0, (current, thisFile) => current + (((new FileInfo(thisFile)).Length)/1024));
                Item_To_Complete.DiskSize_MB = size;

                // Save to the database
                try
                {
                    SobekCM_Database.Save_Digital_Resource( Item_To_Complete );
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

                // Save the rest of the metadata
                Item_To_Complete.Save_SobekCM_METS();

                // Finally, set the item for more processing if there were any files
                if ((image_files.Length > 0) && ( Item_To_Complete.Web.ItemID > 0 ))
                {
                    Database.SobekCM_Database.Update_Additional_Work_Needed_Flag(Item_To_Complete.Web.ItemID, true, Tracer);
                }

                foreach (string thisFile in image_files)
                {
                    try
                    {
                        File.Delete(thisFile);
                    }
                    catch
                    {
                        // Do nothing - not a fatal problem
                    }
                }

                try
                {
                    Directory.Delete(digitalResourceDirectory);
                }
                catch
                {
                    // Do nothing - not a fatal problem
                }

                // This may be called from QC, so check on that as well
                string userInProcessDirectory = SobekCM_Library_Settings.In_Process_Submission_Location + "\\" + user.UserName.Replace(".", "").Replace("@", "") + "\\qcwork\\" + Item_To_Complete.METS_Header.ObjectID;
                if (user.ShibbID.Trim().Length > 0)
                    userInProcessDirectory = SobekCM_Library_Settings.In_Process_Submission_Location + "\\" + user.ShibbID + "\\qcwork\\" + Item_To_Complete.METS_Header.ObjectID;

                // Make the folder for the user in process directory
                if (Directory.Exists(userInProcessDirectory))
                {
                    foreach (string thisFile in Directory.GetFiles(userInProcessDirectory))
                    {
                        try
                        {
                            File.Delete(thisFile);
                        }
                        catch
                        {
                            // Do nothing - not a fatal problem
                        }
                    }
                }
                HttpContext.Current.Session[Item_To_Complete.BibID + "_" + Item_To_Complete.VID + " QC Work"] = null;

            }
            catch (Exception ee)
            {
                validationErrors.Add("Error encountered during item save!");
                validationErrors.Add(ee.ToString().Replace("\r", "<br />"));

                // Set an initial flag 
                criticalErrorEncountered = true;

                string error_body = "<strong>ERROR ENCOUNTERED DURING ONLINE PAGE IMAGE UPLOAD</strong><br /><br /><blockquote>Title: " + Item_To_Complete.Bib_Info.Main_Title.Title + "<br />Permanent Link: <a href=\"" + base.currentMode.Base_URL + "/" + Item_To_Complete.BibID + "/" + Item_To_Complete.VID + "\">" + base.currentMode.Base_URL + "/" + Item_To_Complete.BibID + "/" + Item_To_Complete.VID + "</a><br />User: " + user.Full_Name + "<br /><br /></blockquote>" + ee.ToString().Replace("\n", "<br />");
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
                return "Page Image Upload";
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

			// Write the top item mimic html portion
			Write_Item_Type_Top(Output, item);

			Output.WriteLine("<div id=\"container-inner1000\">");
			Output.WriteLine("<div id=\"pagecontainer\">");

			Output.WriteLine("<div class=\"sbkMySobek_HomeText\">");
			Output.WriteLine("  <br />");

            Output.WriteLine("  <h2>Upload Page Images</h2>");
            Output.WriteLine("  <p>Upload the page images for your item.  You will then be directed to manage the pages and divisions to ensure the new page images appear in the correct order and are reflected in the table of contents.</p>");

            currentMode.My_Sobek_Type = My_Sobek_Type_Enum.File_Management;
            Output.WriteLine("  <p><a href=\"" + currentMode.Redirect_URL() + "\">Click here to add download files instead.</a></p>");
            currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Page_Images_Management;

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
                Tracer.Add_Trace("File_Management_MySobekViewer.Write_ItemNavForm_Closing", "");
            }

            // Add the hidden fields first
            Output.WriteLine("<!-- Hidden field is used for postbacks to indicate what to save and reset -->");
            Output.WriteLine("<input type=\"hidden\" id=\"action\" name=\"action\" value=\"\" />");
            Output.WriteLine("<input type=\"hidden\" id=\"phase\" name=\"phase\" value=\"\" />");
            Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_metadata.js\" ></script>");

            Output.WriteLine("<hr />");
            Output.WriteLine("<br />");

            // Any download files?
            string[] files = Directory.GetFiles(digitalResourceDirectory);
            if (files.Length > 0)
            {
                Output.WriteLine("The following new page images will be added to the item once you click SUBMIT:");
				Output.WriteLine("<table class=\"sbkMySobek_FileTable\">");
				Output.WriteLine("  <tr style=\"min-height:22px;\" >");
				Output.WriteLine("    <th style=\"width:350px;\">FILENAME</th>");
				Output.WriteLine("    <th style=\"width:90px\">SIZE</th>");
				Output.WriteLine("    <th style=\"width:170px\">DATE UPLOADED</th>");
				Output.WriteLine("    <th style=\"width:90px; text-align: center;\">ACTION</th>");
				Output.WriteLine("  </tr>");

                // Collect the page files we are uploading into groups
                SortedDictionary<string, List<string>> file_groups = new SortedDictionary<string, List<string>>();
                foreach (string thisFile in files)
                {
                    FileInfo newFileInfo = new FileInfo(thisFile);
                    string name = newFileInfo.Name;
                    string extension = newFileInfo.Extension;
                    string name_sans_extension = name.Replace(extension, "").ToUpper();
                    if (name.ToUpper().IndexOf("THM.JPG") > 0)
                    {
                        name_sans_extension = name.ToUpper().Replace("THM.JPG","");
                    }

                    if ( file_groups.ContainsKey(name_sans_extension))
                        file_groups[name_sans_extension].Add(thisFile);
                    else
                    {
                        List<string> newGroup = new List<string> {thisFile};
                        file_groups.Add(name_sans_extension, newGroup);
                    }
                }


                // Step through all the page image file groups
                int file_counter = 0;
                for (int i = 0; i < file_groups.Count; i++)
                {
                    List<string> groupFiles = file_groups.ElementAt(i).Value;

                    foreach (string thisFile in groupFiles)
                    {
                        file_counter++;

                        // Add the file name literal
                        FileInfo fileInfo = new FileInfo(thisFile);  
						Output.WriteLine("  <tr style=\"min-height:22px;\">");
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
						Output.WriteLine("    <td align=\"center\"> <span class=\"sbkMySobek_ActionLink\">( <a href=\"\" onclick=\"return file_delete('" + fileInfo.Name + "');\">delete</a> )</span></td>");
	                    Output.WriteLine("  </tr>");
                    }
					Output.WriteLine("  <tr><td class=\"sbkMySobek_FileTableRule\" colspan=\"4\"></td></tr>");
                }
                Output.WriteLine("</table>");
				Output.WriteLine();
            }

			Output.WriteLine("<div class=\"sbkMySobek_FileRightButtons\">");
			Output.WriteLine("  <button title=\"Cancel this and remove the recentely uploaded images\" onclick=\"return new_upload_next_phase(2);\" class=\"sbkPiu_RoundButton\">CANCEL</button> &nbsp; ");
			Output.WriteLine("  <button title=\"Submit the recently uploaded page images and complete the process\" onclick=\"return new_upload_next_phase(9);\" class=\"sbkPiu_RoundButton\">SUBMIT</button> &nbsp; ");
			Output.WriteLine("  <div id=\"circular_progress\" name=\"circular_progress\" class=\"hidden_progress\">&nbsp;</div>");
			Output.WriteLine("</div>");
			Output.WriteLine();

			const string COMPLETION_MESSAGE = "Once all images are uploaded, press SUBMIT to finish this item.";

			Output.WriteLine("<div class=\"sbkMySobek_FileCompletionMsg\">" + COMPLETION_MESSAGE + "</div>");
			Output.WriteLine();

            Output.WriteLine("<br /><br />");
            Output.WriteLine("<hr />");
            Output.WriteLine("<br />");
            Output.WriteLine("The following extensions are accepted:");
            Output.WriteLine("<blockquote>");
            Output.WriteLine(SobekCM_Library_Settings.Upload_Image_Types.Replace(",", ", "));
            Output.WriteLine("</blockquote>");
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
            Tracer.Add_Trace("New_Group_And_Item_MySobekViewer.Add_Controls", String.Empty);

            // Add the upload controls to the file place holder
            add_upload_controls(MainPlaceHolder, Tracer);
        }

        private void add_upload_controls(PlaceHolder placeHolder, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("New_Group_And_Item_MySobekViewer.add_upload_controls", String.Empty);

            StringBuilder filesBuilder = new StringBuilder(2000);
            filesBuilder.AppendLine("<script src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_metadata.js\" type=\"text/javascript\"></script>");
            filesBuilder.AppendLine("Add a new page image for this package:");
            filesBuilder.AppendLine("<blockquote>");

            LiteralControl filesLiteral2 = new LiteralControl(filesBuilder.ToString());
            placeHolder.Controls.Add(filesLiteral2);
            filesBuilder.Remove(0, filesBuilder.Length);

			UploadiFiveControl uploadControl = new UploadiFiveControl();
			uploadControl.UploadPath = digitalResourceDirectory;
			uploadControl.UploadScript = currentMode.Base_URL + "UploadiFiveFileHandler.ashx";
			uploadControl.AllowedFileExtensions = SobekCM_Library_Settings.Upload_Image_Types;
			uploadControl.SubmitWhenQueueCompletes = true;
	        uploadControl.RemoveCompleted = true;
			uploadControl.Swf = currentMode.Base_URL + "default/scripts/uploadify/uploadify.swf";
			uploadControl.RevertToFlashVersion = true;
			placeHolder.Controls.Add(uploadControl);

			filesBuilder.AppendLine("</blockquote><br />");

            LiteralControl literal1 = new LiteralControl(filesBuilder.ToString());
            placeHolder.Controls.Add(literal1);
        }

		/// <summary> Gets the collection of special behaviors which this admin or mySobek viewer
		/// requests from the main HTML subwriter. </summary>
		/// <value> This tells the HTML and mySobek writers to mimic the item viewer </value>
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
  

