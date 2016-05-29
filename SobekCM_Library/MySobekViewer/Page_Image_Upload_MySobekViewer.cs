#region Using directives

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SobekCM.Core.Client;
using SobekCM.Core.MemoryMgmt;
using SobekCM.Core.Navigation;
using SobekCM.Core.UI_Configuration;
using SobekCM.Core.UI_Configuration.StaticResources;
using SobekCM.Engine_Library.Configuration;
using SobekCM.Engine_Library.Email;
using SobekCM.Library.AdminViewer;
using SobekCM.Library.HTML;
using SobekCM.Library.UI;
using SobekCM.Library.UploadiFive;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Behaviors;
using SobekCM.Resource_Object.Database;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Tools;
using SobekCM_Resource_Database;
using Image = System.Drawing.Image;

#endregion

namespace SobekCM.Library.MySobekViewer
{
    /// <summary> MySobek viewer for uploading new page images to an existing digital resource </summary>
    public class Page_Image_Upload_MySobekViewer : abstract_MySobekViewer
    {
        private bool criticalErrorEncountered;
        private readonly string digitalResourceDirectory;
        private readonly SobekCM_Item currentItem;

        #region Constructor

        /// <summary> Constructor for a new instance of the Page_Image_Upload_MySobekViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        public Page_Image_Upload_MySobekViewer(RequestCache RequestSpecificValues)  : base(RequestSpecificValues)
        {
            RequestSpecificValues.Tracer.Add_Trace("Page_Image_Upload_MySobekViewer.Constructor", String.Empty);

            // If no user then that is an error
            if ((RequestSpecificValues.Current_User == null) || (!RequestSpecificValues.Current_User.LoggedOn))
            {
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Aggregation;
                RequestSpecificValues.Current_Mode.Aggregation = String.Empty;
                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                return;
            }

            // Ensure BibID and VID provided
            RequestSpecificValues.Tracer.Add_Trace("Page_Image_Upload_MySobekViewer.Constructor", "Validate provided bibid / vid");
            if ((String.IsNullOrEmpty(RequestSpecificValues.Current_Mode.BibID)) || (String.IsNullOrEmpty(RequestSpecificValues.Current_Mode.VID)))
            {
                RequestSpecificValues.Tracer.Add_Trace("Page_Image_Upload_MySobekViewer.Constructor", "BibID or VID was not provided!");
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Error;
                RequestSpecificValues.Current_Mode.Error_Message = "Invalid Request : BibID/VID missing in item page upload request";
                return;
            }

            // Ensure the item is valid
            RequestSpecificValues.Tracer.Add_Trace("Page_Image_Upload_MySobekViewer.Constructor", "Validate bibid/vid exists");
            if (!UI_ApplicationCache_Gateway.Items.Contains_BibID_VID(RequestSpecificValues.Current_Mode.BibID, RequestSpecificValues.Current_Mode.VID))
            {
                RequestSpecificValues.Tracer.Add_Trace("Page_Image_Upload_MySobekViewer.Constructor", "BibID/VID indicated is not valid", Custom_Trace_Type_Enum.Error);
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Error;
                RequestSpecificValues.Current_Mode.Error_Message = "Invalid Request : BibID/VID indicated is not valid";
                return;
            }

            RequestSpecificValues.Tracer.Add_Trace("Page_Image_Upload_MySobekViewer.Constructor", "Try to pull this sobek complete item");
            currentItem = SobekEngineClient.Items.Get_Sobek_Item(RequestSpecificValues.Current_Mode.BibID, RequestSpecificValues.Current_Mode.VID, RequestSpecificValues.Current_User.UserID, RequestSpecificValues.Tracer);
            if (currentItem == null)
            {
                RequestSpecificValues.Tracer.Add_Trace("Page_Image_Upload_MySobekViewer.Constructor", "Unable to build complete item");
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Error;
                RequestSpecificValues.Current_Mode.Error_Message = "Invalid Request : Unable to build complete item";
                return;
            }


            // If the current user cannot edit this currentItem, go back
            if (!RequestSpecificValues.Current_User.Can_Edit_This_Item( currentItem.BibID, currentItem.Bib_Info.SobekCM_Type_String, currentItem.Bib_Info.Source.Code, currentItem.Bib_Info.HoldingCode, currentItem.Behaviors.Aggregation_Code_List ))
            {
                RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                return;
            }

            // Determine the in process directory for this
            digitalResourceDirectory = UI_ApplicationCache_Gateway.Settings.Servers.In_Process_Submission_Location + "\\" + RequestSpecificValues.Current_User.UserName.Replace(".", "").Replace("@", "") + "\\uploadimages\\" + currentItem.METS_Header.ObjectID;
            if (RequestSpecificValues.Current_User.ShibbID.Trim().Length > 0)
                digitalResourceDirectory = UI_ApplicationCache_Gateway.Settings.Servers.In_Process_Submission_Location + "\\" + RequestSpecificValues.Current_User.ShibbID + "\\uploadimages\\" + currentItem.METS_Header.ObjectID;

            // Make the folder for the RequestSpecificValues.Current_User in process directory
            if (!Directory.Exists(digitalResourceDirectory))
                Directory.CreateDirectory(digitalResourceDirectory);
            else
            {
				// Any post-processing to do?
				string[] files = Directory.GetFiles(digitalResourceDirectory);

                // Make a dictionary of the included files
                Dictionary<string, string> filesByName = new Dictionary<string, string>();
                foreach (string thisFile in files)
                {
                    string fileName = Path.GetFileName(thisFile);
                    if (!String.IsNullOrEmpty(fileName))
                        filesByName[fileName.ToUpper()] = thisFile;
                }

                // Now, step through each TIFF
				foreach (string thisFile in files)
				{
                    string extension = Path.GetExtension(thisFile);
                    string name = Path.GetFileName(thisFile);

                    // Should never happen
                    if ((extension == null) || (name == null )) continue;

                    if ((String.Compare(extension, ".TIF", StringComparison.OrdinalIgnoreCase) == 0) || (String.Compare(extension, ".TIFF", StringComparison.OrdinalIgnoreCase) == 0))
					{
						// Is there a JPEG and/or thumbnail?
                        string jpeg = digitalResourceDirectory + "\\" + name.Replace(extension, "") + ".jpg";
                        string jpeg_thumbnail = digitalResourceDirectory + "\\" + name.Replace(extension, "") + "thm.jpg";

						// Is one missing?
						if ((!File.Exists(jpeg)) || (!File.Exists(jpeg_thumbnail)))
						{
							try
							{
								var tiffImg = Image.FromFile(thisFile);
								var mainImg = ScaleImage(tiffImg, UI_ApplicationCache_Gateway.Settings.Resources.JPEG_Width, UI_ApplicationCache_Gateway.Settings.Resources.JPEG_Height);
								mainImg.Save(jpeg, ImageFormat.Jpeg);
								var thumbnailImg = ScaleImage(tiffImg, 150, 400);
								thumbnailImg.Save(jpeg_thumbnail, ImageFormat.Jpeg);

							}
							catch 
							{
                                // Do nothing
							}
						}
					}
				}

                // Now, check one more time for JPEGs that do not have thumbnails
                FileStream reuseStream = null;
                foreach (string thisFile in files)
                {
                    string extension = Path.GetExtension(thisFile);
                    string name = Path.GetFileName(thisFile);

                    // Should never happen
                    if ((extension == null) || (name == null)) continue;

                    if ((String.Compare(extension, ".JPG", StringComparison.OrdinalIgnoreCase) == 0) && ( name.IndexOf("THM.JPG", StringComparison.OrdinalIgnoreCase) < 0 ))
                    {
                        // Is there a JPEG and/or thumbnail?
                        string jpeg_thumbnail = digitalResourceDirectory + "\\" + name.Replace(extension, "") + "thm.jpg";

                        // Is one missing?
                        if (!File.Exists(jpeg_thumbnail))
                        {
                            try
                            {
                                // Load the JPEG
                                Image jpegSourceImg = SafeImageFromFile(thisFile, ref reuseStream);
                                if ((jpegSourceImg.Width > UI_ApplicationCache_Gateway.Settings.Resources.JPEG_Maximum_Width) || (jpegSourceImg.Height > UI_ApplicationCache_Gateway.Settings.Resources.JPEG_Maximum_Height))
                                {
                                    // Copy the JPEG
                                    string final_destination = currentItem.Source_Directory + "\\" + UI_ApplicationCache_Gateway.Settings.Resources.Backup_Files_Folder_Name;
                                    if (Directory.Exists(final_destination))
                                        Directory.CreateDirectory(final_destination);
                                    string copy_file = final_destination + "\\" + name.Replace(extension, "") + "_ORIG.jpg";
                                    File.Copy(thisFile, copy_file, true);

                                    // Create the TIFF
                                    string tiff_file = digitalResourceDirectory + "\\" + name.Replace(extension, "") + ".tif";
                                    jpegSourceImg.Save(tiff_file, ImageFormat.Tiff);

                                    // Delete the original JPEG file
                                    File.Delete(thisFile);

                                    // Now, create the smaller JPEG and JPEG thumbnail
                                    string jpeg = digitalResourceDirectory + "\\" + name.Replace(extension, "") + ".jpg";
                                    var mainImg = ScaleImage(jpegSourceImg, UI_ApplicationCache_Gateway.Settings.Resources.JPEG_Width, UI_ApplicationCache_Gateway.Settings.Resources.JPEG_Height);
                                    mainImg.Save(jpeg, ImageFormat.Jpeg);

                                    // And save the thumbnasil as well
                                    var thumbnailImg = ScaleImage(jpegSourceImg, 150, 400);
                                    thumbnailImg.Save(jpeg_thumbnail, ImageFormat.Jpeg);
                                }
                                else
                                {
                                    // The JPEG is good to show AS IS, so just create the thumbnail
                                    var thumbnailImg = ScaleImage(jpegSourceImg, 150, 400);
                                    thumbnailImg.Save(jpeg_thumbnail, ImageFormat.Jpeg);
                                }
                            }
                            catch ( Exception ee)
                            {
                                RequestSpecificValues.Current_Mode.Error_Message = ee.Message;
                                // Do nothing
                            }
                        }
                    }
                }
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
                        HttpContext.Current.Session["file_" + currentItem.Web.ItemID + "_" + file_name_from_keys.Trim()] = label_from_keys.Trim();
                        file_name_from_keys = String.Empty;
                        label_from_keys = String.Empty;
                    }

                    if (thisKey == "url_input")
                    {
                        currentItem.Bib_Info.Location.Other_URL = HttpContext.Current.Request.Form[thisKey];
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

                            // Redirect to the currentItem
                            RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Item_Display;
                            UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                            break;

                        case 9:
                            if (!complete_item_submission(currentItem, null))
                            {
                                // Also clear the currentItem from the cache
                                CachedDataManager.Items.Remove_Digital_Resource_Object(currentItem.BibID, currentItem.VID, null);

                                // Redirect to the currentItem
                                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Item_Display;
                                RequestSpecificValues.Current_Mode.ViewerCode = "qc";
                                UrlWriterHelper.Redirect(RequestSpecificValues.Current_Mode);
                            }
                            break;                         
                    }
                }
            }
        } 

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
                    FileInfo fileInfo = new FileInfo(thisFile);
                    SobekCM_File_Info newFile = new SobekCM_File_Info(fileInfo.Name);

                    // Copy this file
                    File.Copy(thisFile, final_destination + "\\" + fileInfo.Name, true);
                    currentItem.Divisions.Physical_Tree.Add_File(newFile, "New Page");

                    // Seperate code for JP2 and JPEG type files
                    string extension = fileInfo.Extension.ToUpper();
                    if (extension.IndexOf("JP2") >= 0)
                    {
                        if (!error_reading_file_occurred)
                        {
                            if (!newFile.Compute_Jpeg2000_Attributes(currentItem.Source_Directory))
                                error_reading_file_occurred = true;
                        }
                        jp2_added = true;
                    }
                    else if (extension.IndexOf("JPG") >= 0)
                    {
                        if (!error_reading_file_occurred)
                        {
                            if (!newFile.Compute_Jpeg_Attributes(currentItem.Source_Directory))
                                error_reading_file_occurred = true;
                        }
                        jpeg_added = true;
                    }
                }

                // Add the JPEG2000 and JPEG-specific viewers
				//currentItem.Behaviors.Clear_Views();
				if (jpeg_added) 
				{
					// Is a JPEG view already existing?
					bool jpeg_viewer_already_exists = false;
                    foreach (View_Object thisViewer in currentItem.Behaviors.Views)
					{
						if (thisViewer.View_Type == "JPEG")
						{
							jpeg_viewer_already_exists = true;
							break;
						}
					}

					// Add the JPEG view if it did not already exists
					if ( !jpeg_viewer_already_exists )
						currentItem.Behaviors.Add_View("JPEG");
				}

				// If a JPEG2000 file was just added, ensure it exists as a view for this currentItem
				if (jp2_added)
				{
					// Is a JPEG view already existing?
					bool jpg2000_viewer_already_exists = false;
					foreach (View_Object thisViewer in currentItem.Behaviors.Views)
					{
						if (thisViewer.View_Type == "JPEG2000" )
						{
							jpg2000_viewer_already_exists = true;
							break;
						}
					}

					// Add the JPEG2000 view if it did not already exists
					if (!jpg2000_viewer_already_exists)
                        currentItem.Behaviors.Add_View("JPEG2000");
				}

                // Determine the total size of the package before saving
                string[] all_files_final = Directory.GetFiles(final_destination);
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
                options["MarcXML_File_ReaderWriter:System Name"] = UI_ApplicationCache_Gateway.Settings.System.System_Name;
                options["MarcXML_File_ReaderWriter:System Abbreviation"] = UI_ApplicationCache_Gateway.Settings.System.System_Abbreviation;


                // Save to the database
                try
                {
                    SobekCM_Item_Database.Save_Digital_Resource(Item_To_Complete, options);
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

                // Finally, set the currentItem for more processing if there were any files
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
                string userInProcessDirectory = UI_ApplicationCache_Gateway.Settings.Servers.In_Process_Submission_Location + "\\" + RequestSpecificValues.Current_User.UserName.Replace(".", "").Replace("@", "") + "\\qcwork\\" + Item_To_Complete.METS_Header.ObjectID;
                if (RequestSpecificValues.Current_User.ShibbID.Trim().Length > 0)
                    userInProcessDirectory = UI_ApplicationCache_Gateway.Settings.Servers.In_Process_Submission_Location + "\\" + RequestSpecificValues.Current_User.ShibbID + "\\qcwork\\" + Item_To_Complete.METS_Header.ObjectID;

                // Make the folder for the RequestSpecificValues.Current_User in process directory
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
                // Set an initial flag 
                criticalErrorEncountered = true;

                string error_body = "<strong>ERROR ENCOUNTERED DURING ONLINE PAGE IMAGE UPLOAD</strong><br /><br /><blockquote>Title: " + Item_To_Complete.Bib_Info.Main_Title.Title + "<br />Permanent Link: <a href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "/" + Item_To_Complete.BibID + "/" + Item_To_Complete.VID + "\">" + RequestSpecificValues.Current_Mode.Base_URL + "/" + Item_To_Complete.BibID + "/" + Item_To_Complete.VID + "</a><br />RequestSpecificValues.Current_User: " + RequestSpecificValues.Current_User.Full_Name + "<br /><br /></blockquote>" + ee.ToString().Replace("\n", "<br />");
                string error_subject = "Error during file management for '" + Item_To_Complete.Bib_Info.Main_Title.Title + "'";
                string email_to = UI_ApplicationCache_Gateway.Settings.Email.System_Error_Email;
                if (email_to.Length == 0)
                    email_to = UI_ApplicationCache_Gateway.Settings.Email.System_Email;
                Email_Helper.SendEmail(email_to, error_subject, error_body, true, String.Empty);
            }


            return criticalErrorEncountered;
        }

        #endregion

        #region Method to return an image after closing connectio to the file

        private static Image SafeImageFromFile(string FilePath, ref FileStream ReuseStream)
        {
            // http://stackoverflow.com/questions/18250848/how-to-prevent-the-image-fromfile-method-to-lock-the-file

            Bitmap img;
            ReuseStream = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
            using (Bitmap b = new Bitmap(ReuseStream))
            {
                img = new Bitmap(b.Width, b.Height, b.PixelFormat);
                using (Graphics g = Graphics.FromImage(img))
                {
                    g.DrawImage(b, 0, 0, img.Width, img.Height);
                    g.Flush();
                }
            }
            ReuseStream.Close();
            return img;
        }

        #endregion


        /// <summary> Navigation type to be displayed (mostly used by the mySobek viewers) </summary>
        /// <value> This returns none since this viewer writes all the necessary navigational elements </value>
        /// <remarks> This is set to NONE if the viewer will write its own navigation and ADMIN if the standard
        /// administrative tabs should be included as well.  </remarks>
        public override MySobek_Admin_Included_Navigation_Enum Standard_Navigation_Type { get { return MySobek_Admin_Included_Navigation_Enum.NONE; } }

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

            Output.WriteLine("<script src=\"" + Static_Resources_Gateway.Sobekcm_Metadata_Js + "\" type=\"text/javascript\"></script>");

			// Write the top currentItem mimic html portion
			Write_Item_Type_Top(Output, currentItem);

			Output.WriteLine("<div id=\"container-inner1000\">");
			Output.WriteLine("<div id=\"pagecontainer\">");

			Output.WriteLine("<div class=\"sbkMySobek_HomeText\">");
			Output.WriteLine("  <br />");

            Output.WriteLine("  <h2>Upload Page Images</h2>");
            Output.WriteLine("  <p>Upload the page images for your item.  You will then be directed to manage the pages and divisions to ensure the new page images appear in the correct order and are reflected in the table of contents.</p>");

            RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.File_Management;
            Output.WriteLine("  <p><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">Click here to add download files instead.</a></p>");
            RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Page_Images_Management;

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
            Output.WriteLine("<script type=\"text/javascript\" src=\"" + Static_Resources_Gateway.Sobekcm_Metadata_Js + "\" ></script>");

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
                for (int i = 0; i < file_groups.Count; i++)
                {
                    List<string> groupFiles = file_groups.ElementAt(i).Value;

                    foreach (string thisFile in groupFiles)
                    {
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

                        //add by Keven:replace single & double quote with ascII characters
                        string strFileName = fileInfo.Name;
                        if (strFileName.Contains("'") || strFileName.Contains("\""))
                        {
                            strFileName = strFileName.Replace("'", "\\&#39;");
                            strFileName = strFileName.Replace("\"", "\\&#34;");
                        }
                        Output.WriteLine("    <td align=\"center\"> <span class=\"sbkMySobek_ActionLink\">( <a href=\"\" onclick=\"return file_delete('" + strFileName + "');\">delete</a> )</span></td>");

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
            Output.WriteLine(UI_ApplicationCache_Gateway.Settings.Resources.Upload_Image_Types.Replace(",", ", "));
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

        private void add_upload_controls(PlaceHolder PlaceHolder, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("New_Group_And_Item_MySobekViewer.add_upload_controls", String.Empty);

            StringBuilder filesBuilder = new StringBuilder(2000);
            filesBuilder.AppendLine("<script src=\"" + Static_Resources_Gateway.Sobekcm_Metadata_Js + "\" type=\"text/javascript\"></script>");
            filesBuilder.AppendLine("Add a new page image for this package:");
            filesBuilder.AppendLine("<blockquote>");

            LiteralControl filesLiteral2 = new LiteralControl(filesBuilder.ToString());
            PlaceHolder.Controls.Add(filesLiteral2);
            filesBuilder.Remove(0, filesBuilder.Length);

			UploadiFiveControl uploadControl = new UploadiFiveControl
			{
			    UploadPath = digitalResourceDirectory, 
                UploadScript = RequestSpecificValues.Current_Mode.Base_URL + "UploadiFiveFileHandler.ashx", 
                AllowedFileExtensions = UI_ApplicationCache_Gateway.Settings.Resources.Upload_Image_Types, 
                SubmitWhenQueueCompletes = true, 
                RemoveCompleted = true, 
                Swf = Static_Resources_Gateway.Uploadify_Swf, 
                RevertToFlashVersion = true
			};
            PlaceHolder.Controls.Add(uploadControl);

			filesBuilder.AppendLine("</blockquote><br />");

            LiteralControl literal1 = new LiteralControl(filesBuilder.ToString());
            PlaceHolder.Controls.Add(literal1);
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

        /// <summary> Returns a flag indicating whether the file upload specific holder in the itemNavForm form will be utilized 
        /// for the current request, or if it can be hidden/omitted. </summary>
        /// <value> Returns TRUE since files can be be uploaded through this viewer </value>
        public override bool Upload_File_Possible { get { return true; } }
    }
}
  

