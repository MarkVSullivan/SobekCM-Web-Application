#region Using directives

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

#endregion

namespace SobekCM.Resource_Object.Utilities
{

	#region Definitions of delegates

	/// <summary> Delegate for the custom event which is fired when the status
	/// string on the main form needs to change </summary>
	public delegate void Image_Creation_New_Status_String_Delegate(string new_message);

	/// <summary> Delegate for the custom event which is fired when the progress
	/// bar should change. </summary>
	public delegate void Image_Creation_New_Progress_Delegate(int Value, int Max);

	/// <summary> Delegate for the custom event which is fired when all the processing is complete </summary>
	public delegate void Image_Creation_Process_Complete_Delegate(int Packages_Processed, int JPEG2000_Warnings);

	#endregion

	/// <summary> Proceessor class is used for creatig image derivatives for a single package 
	/// destined for a SobekCM library, including thumbnails, jpeg images, and a service jpeg2000 file </summary>
	public class Image_Derivative_Creation_Processor
	{
		private const string SOBEKCM_IMAGE_LOCATION = @"\\cns-uflib-ufdc\UFDC\RESOURCES\";
		private const string SOBEKCM_DROPBOX_LOCATION = @"\\cns-uflib-ufdc\UFDC\INCOMING\";
		private const string SOBEKCM_DATA_LOCATION = @"\\cns-uflib-ufdc\UFDC\DATA\";

		private bool catastrophic_failure_detected = false;
		private int consecutive_image_creation_error;
		private bool create_jpeg2000_images;
		private bool create_jpeg_images;
		private bool create_qc_images;
		private bool errorEncountered;
		private bool first_jpeg_successfully_created = false;

		private string image_magick_path;
		private string imagemagick_executable;
		private int jpeg_height;
		private int jpeg_width;
		private int kakadu_error_count;
		private string kakadu_path;
		private string temp_folder;
		private int thumbnail_height;
		private int thumbnail_width;

		/// <summary> Constructor for a new instance of this class </summary>
		public Image_Derivative_Creation_Processor(string Image_Magick_Path, string Kakadu_Path, bool Create_JPEGs, bool Create_JPEG2000s, int JPEG_Width, int JPEG_Height, bool Create_QC_Images, int Thumbnail_Width, int Thumbnail_Height)
		{
			// Save all the parameters
			image_magick_path = Image_Magick_Path;
			kakadu_path = Kakadu_Path;
			create_jpeg_images = Create_JPEGs;
			create_jpeg2000_images = Create_JPEG2000s;
			create_qc_images = Create_QC_Images;
			jpeg_width = JPEG_Width;
			jpeg_height = JPEG_Height;
			thumbnail_height = Thumbnail_Height;
			thumbnail_width = Thumbnail_Width;

			// Save the location for temporary files
			temp_folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			temp_folder = temp_folder + "\\DLC Toolbox\\Temporary";
			imagemagick_executable = String.Empty;
		}

		/// <summary> Gets the count of the number of failures Kakadu experienced while creating JPEG2000 derivatives </summary>
		public int Kakadu_Failures
		{
			get { return kakadu_error_count; }
		}

		/// <summary> Custom event is fired when the task string on the 
		/// main form needs to change. </summary>
		public event Image_Creation_New_Status_String_Delegate New_Task_String;

		/// <summary> Custom event is fired when an error is encountered during image creation. </summary>
		public event Image_Creation_New_Status_String_Delegate Error_Encountered;

		/// <summary> Custom event is fired when the progress bar
		/// on the main form needs to change.  </summary>
		public event Image_Creation_New_Progress_Delegate New_Progress;

		/// <summary> Custom event is fired when all processing is complete </summary>
		public event Image_Creation_Process_Complete_Delegate Process_Complete;

		/// <summary> Processes a single package </summary>
		public bool Process(string Package_Directory, string BibID, string VID, string[] TIF_Files)
		{
			// Create the temp folder
			if (!Directory.Exists(temp_folder))
			{
				Directory.CreateDirectory(temp_folder);
			}

			kakadu_error_count = 0;
			consecutive_image_creation_error = 0;
			catastrophic_failure_detected = false;

			errorEncountered = false;
			bool kakaduErrorEncountered = false;

			// Perform imaging work on all the files, in a try/catch
			try
			{
				if (Create_Derivative_Files(Package_Directory, TIF_Files, BibID + ":" + VID))
				{
					kakaduErrorEncountered = true;
				}

				// Were there three consecutive errors caught?
				if (consecutive_image_creation_error > 3)
				{
					errorEncountered = true;
				}
			}
			catch (Exception ee)
			{
				// Add this error to the log
				OnErrorEncountered("Error encountered during image work on " + BibID + ":" + VID);
				OnErrorEncountered(ee.Message);
				errorEncountered = true;
			}

			// Add a warning if there were kakdu error
			if (kakaduErrorEncountered)
			{
				kakadu_error_count++;
				OnErrorEncountered("Warning: Error during JPEG2000 creation for " + BibID + ":" + VID);
			}

			// Delete the temporary folder
			if (Directory.Exists(temp_folder))
			{
				Directory.Delete(temp_folder, true);
			}

			// Fire the process complete event
			if (!catastrophic_failure_detected)
			{
				OnProcessComplete();
				Thread.Sleep(2000);
			}
			else
			{
				OnErrorEncountered("ImageMagick Error Detected - Process Aborted");

				OnProcessComplete();
				Thread.Sleep(2000);
			}

			return errorEncountered;
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

		private void OnNewTask(string newMessage)
		{
			if (New_Task_String != null)
				New_Task_String(newMessage);
		}

		private void OnErrorEncountered(string newMessage)
		{
			if (Error_Encountered != null)
				Error_Encountered(newMessage);
		}

		private void OnProcessComplete()
		{
			if (Process_Complete != null)
				Process_Complete(1, kakadu_error_count);
		}

		#region Create Derivative Files

		private bool Create_Derivative_Files(string directory, string[] tif_files, string package_name)
		{
			bool kakadu_error = false;

			// Ensure the directory does not end in a '\' for later work
			if (directory[directory.Length - 1] == '\\')
				directory = directory.Substring(0, directory.Length - 1);

			// Itereate through all the files in this volume
			int fileCtr = 1;
			int totalCount = tif_files.Length;
			if (totalCount > 0)
			{
				OnNewTask("\t\tProcessing Images for " + package_name);

				// Step through each TIF file
				foreach (string tifFile in tif_files)
				{
					// Get the basic file information
					FileInfo tifFileInfo = new FileInfo(tifFile);
					string fileName = tifFileInfo.Name;
					string fileNameUpper = fileName.ToUpper();
					if (fileNameUpper.IndexOf("_ARCHIVE") < 0)
					{
						string tiffNameSansExtension = tifFileInfo.Name.Replace(tifFileInfo.Extension, "");
						string rootName = directory + "\\" + tiffNameSansExtension;

						// Add to log
						OnNewTask("\t\t\tProcessing Image '" + fileName + "'");
						OnNewProgress(fileCtr++, (totalCount + 1));


						// Get the date for this file and the date for the QC files, if they exist
						if (((!File.Exists(rootName + ".QC.jpg")) || (!File.Exists(rootName + ".jpg")) || (!File.Exists(rootName + "thm.jpg")))
							|| (File.GetLastWriteTime(rootName + ".QC.jpg").CompareTo(File.GetLastWriteTime(tifFile)) < 0)
							|| (File.GetLastWriteTime(rootName + ".jpg").CompareTo(File.GetLastWriteTime(tifFile)) < 0))
						{
							// We'll do the processing on our local machine to avoid pulling the data from the SAN multiple times
							string localTempFile = temp_folder + "\\TEMP.tif";
							try
							{
								if (File.Exists(localTempFile))
								{
									File.Delete(localTempFile);
								}
								File.Copy(tifFile, localTempFile, true);
							}
							catch
							{
								Thread.Sleep(2000);

								try
								{
									if (File.Exists(localTempFile))
									{
										File.Delete(localTempFile);
									}
									File.Copy(tifFile, localTempFile, true);
								}
								catch
								{
									OnErrorEncountered("ERROR DELETING EXISTING TEMP.TIF FILE");
									consecutive_image_creation_error++;
								}
							}

							// Process this file, as necessary
							Image_Magick_Process_TIFF_File(localTempFile, tiffNameSansExtension, directory, true, jpeg_width, jpeg_height);

							// Create the JPEG2000
							kakadu_error = !Create_JPEG2000(localTempFile, tiffNameSansExtension, directory);
						}
					}
				}
			}

			return kakadu_error;
		}

		#endregion

		#region ImageMagick all files

		/// <summary> Sets the executable location for the ImageMagick installation on the local machine </summary>
		public string ImageMagick_Executable
		{
			set { imagemagick_executable = value; }
		}

		/// <summary> Process this image via ImageMagick, to create the needed jpeg derivative(s) </summary>
		/// <param name="localTempFile"> Complete name (including directory) of the TIFF file to actually process, which is often in a temporary location </param>
		/// <param name="thisTiffFile"> Name (excluding extension and directory) for the original TIFF file, to be used for naming of the derivatives </param>
		/// <param name="volumeDirectory"> Directory where the derivatives should be created </param>
		/// <param name="make_thumbnail"> Flag indicates whether a thumbnail image should be generated for this TIFF </param>
		/// <param name="width"> Requested width limit of the resulting full-size jpeg image to create </param>
		/// <param name="height"> Requested height limit of the resulting full-size jpeg image to create </param>
		public void Image_Magick_Process_TIFF_File(string localTempFile, string thisTiffFile, string volumeDirectory, bool make_thumbnail, int width, int height)
		{
			// Get the full file name
			string fullFileName = volumeDirectory + "\\" + thisTiffFile + ".tif";
			string rootName = volumeDirectory + "\\" + thisTiffFile;

			try
			{
				// Make a thumbnail image, if necessary
				if ((make_thumbnail) && (thumbnail_height > 0) && (thumbnail_width > 0))
				{
					// Save a 150 pixel wide JPEG
					ImageMagick_Create_JPEG(localTempFile, rootName + "thm.jpg", thumbnail_width, thumbnail_height);
				}

				// Save a 630 pixel wide JPEG
				ImageMagick_Create_JPEG(localTempFile, rootName + ".jpg", width, height);

				if (catastrophic_failure_detected)
				{
					return;
				}

				// Save a 315 pixel wide JPEG
				if (create_qc_images)
				{
					ImageMagick_Create_JPEG(localTempFile, rootName + ".QC.jpg", 315, 500);
				}
			}
			catch (Exception ee)
			{
				OnErrorEncountered("Error caught during image derivative creation: " + ee.Message);
				consecutive_image_creation_error++;
			}
		}

		/// <summary> Use ImageMagick to create a JPEG derivative file </summary>
		/// <param name="sourcefile"> Source file </param>
		/// <param name="finalfile"> Final file</param>
		/// <param name="width"> Width restriction for the resulting jpeg </param>
		/// <param name="height"> Height restriction for the resulting jpeg</param>
		public void ImageMagick_Create_JPEG(string sourcefile, string finalfile, int width, int height)
		{
			try
			{
				// Start this process
				Process convert = new Process();
				convert.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
				convert.StartInfo.CreateNoWindow = true;
				convert.StartInfo.ErrorDialog = true;
				convert.StartInfo.RedirectStandardError = true;
				convert.StartInfo.UseShellExecute = false;
				if (image_magick_path.ToUpper().IndexOf("CONVERT.EXE") > 0)
					convert.StartInfo.FileName = image_magick_path;
				else
					convert.StartInfo.FileName = image_magick_path + "\\convert.exe";
				convert.StartInfo.Arguments = "-geometry " + width + "x" + height + " \"" + sourcefile + "\" \"" + finalfile + "\"";
				string command = convert.StartInfo.FileName + " " + convert.StartInfo.Arguments;

				// Start
				convert.Start();

				// Check for any error
				StreamReader readError = convert.StandardError;
				string error = readError.ReadToEnd();

				// Make sure it is complete
				convert.WaitForExit();
				convert.Dispose();

				// If the final file did not appear, there was a problem
				if (!File.Exists(finalfile))
				{
					if (error.Length > 0)
					{
						// Image Magick did not function!
						//      Tools.Forms.ErrorMessageBox.Show("Error encountered during ImageMagick execution; no JPEG created!\n\n" + error , MessageProvider_Gateway.ImageMagick_Error_Title, new ApplicationException("Attempted to execute command:\n\t" + command));
						if (!first_jpeg_successfully_created)
						{
							catastrophic_failure_detected = true;
						}
						OnErrorEncountered("Error encountered during ImageMagick execution; no JPEG created!");
						OnErrorEncountered(error);
						consecutive_image_creation_error++;
						errorEncountered = true;
						kakadu_error_count++;
					}
					else
					{
						if (!first_jpeg_successfully_created)
						{
							catastrophic_failure_detected = true;
						}
						OnErrorEncountered("No error discovered during ImageMagick execution, but no JPEG was created either!");

						consecutive_image_creation_error++;
						errorEncountered = true;
						kakadu_error_count++;
					}
				}
				else
				{
					if (!first_jpeg_successfully_created)
						first_jpeg_successfully_created = true;
					consecutive_image_creation_error = 0;
				}
			}
			catch
			{
				consecutive_image_creation_error++;
				errorEncountered = true;
				kakadu_error_count++;
			}
		}

		#endregion

		#region Create JPEG2000 Files

		/// <summary> Creates a JPEG2000 derivative service file, according to NDNP specs, for display 
		/// within a SobekCM library </summary>
		/// <param name="localTempFile"> Complete name (including directory) of the TIFF file to actually process, which is often in a temporary location </param>
		/// <param name="filename"> Name of the resulting JPEG2000 file </param>
		/// <param name="directory"> Directory where the resulting JPEG2000 should be created ( usually the directory for the volume )</param>
		/// <returns>TRUE if successful, otherwise FALSE</returns>
		public bool Create_JPEG2000(string localTempFile, string filename, string directory)
		{
			bool returnVal = true;

			// Create the JPEG2000
			string rootFile = directory + "\\" + filename;
			string fullFileName = rootFile + ".tif";

			if ((!File.Exists(rootFile + ".jp2")) || (File.GetLastWriteTime(rootFile + ".jp2").CompareTo(File.GetLastWriteTime(fullFileName)) < 0))
			{
				// Save the JPEG2000
				// IF there is a special temporary TIFF to use, use that one instead
				string source_file = fullFileName;
				string temp_file = temp_folder + "\\TEMP.tif";
				if (File.Exists(temp_file))
				{
					source_file = temp_file;
				}
				if (File.Exists(rootFile + ".tempjp2.tif"))
				{
					// Use the temporary file instead
					source_file = rootFile + ".tempjp2.tif";
				}

				// Save the JPEG2000
				returnVal = Kakadu_Create_JPEG2000(localTempFile, rootFile + ".jp2");
			}

			try
			{
				File.Delete(localTempFile);
			}
			catch
			{
			}

			// Perform some final cleanup
			if (File.Exists(rootFile + ".tempjp2.tif"))
			{
				try
				{
					File.Delete(rootFile + ".tempjp2.tif");
				}
				catch
				{
				}
			}

			return returnVal;
		}

		private bool Kakadu_Create_JPEG2000(string sourcefile, string finalfile)
		{
			bool returnVal = true;

			// Start this process
			Process convert = new Process();
			convert.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
			convert.StartInfo.CreateNoWindow = true;
			convert.StartInfo.ErrorDialog = true;
			convert.StartInfo.RedirectStandardError = true;
			convert.StartInfo.UseShellExecute = false;
			convert.StartInfo.FileName = kakadu_path + "\\kdu_compress_libtiff.exe";
			convert.StartInfo.Arguments = " -i \"" + sourcefile + "\" -o \"" + finalfile + "\" -rate 1.0,0.84,0.7,0.6,0.5,0.4,0.35,0.3,0.25,0.21,0.18,0.15,0.125,0.1,0.088,0.075,0.0625,0.05,0.04419,0.03716,0.03125,0.025,0.0221,0.01858,0.015625 Clevels=6 Stiles={1024,1024} Corder=RLCP Cblk={64,64} Sprofile=PROFILE1";


			////			StreamWriter writer = new StreamWriter( Application.StartupPath + "\\Logs\\kakadu.log", true );
			////			writer.WriteLine( convert.StartInfo.Arguments );
			////			writer.Flush();
			////			writer.Close();

			convert.Start();

			// Check for any error
			StreamReader readError = convert.StandardError;
			string error = readError.ReadToEnd();
			if (error.Length > 0)
			{
				returnVal = false;
				OnErrorEncountered("......." + error);
			}

			// Make sure it is complete
			convert.WaitForExit();
			convert.Dispose();

			return returnVal;
		}

		#endregion

		#region Code for moving necessary files to the TIVOLI drop box

		//public void check_tivoli_backup_and_archive_necessary_files(string BibID, string VID, string Source_Directory, string Tivoli_Directory)
		//{
		//    // Create the destination folder
		//    string destination_bib_folder = Tivoli_Directory + BibID;
		//    string destination_item_folder = destination_bib_folder + "\\" + VID;

		//    // Possible old destination folder
		//    string possible_old_destination_folder = BibID.Substring(0, 2) + "\\" + BibID.Substring(2, 2) + "\\" + BibID.Substring(4, 2) + "\\" + BibID.Substring(6, 2) + "\\" + BibID.Substring(8) + "\\" + VID;

		//    // Get the list of files in the database currently
		//    DataTable archived_files = SobekCM.Resource_Object.Database.SobekCM_Database.Tivoli_Get_Archived_Files(BibID, VID);

		//    // Get the file count
		//    int total_file_count = get_count_recurse_through_item_folder(Source_Directory, BibID + "\\" + VID, possible_old_destination_folder, archived_files);

		//    // Recurse through all the folders
		//    recurse_through_item_folder(total_file_count, 0, Source_Directory, BibID + "\\" + VID, possible_old_destination_folder, archived_files);
		//}

		//private int get_count_recurse_through_item_folder(string item_source_folder, string item_destination_folder, string possible_old_destination_folder, DataTable archived_files)
		//{
		//    int return_value = 0;
		//    string[] files = Directory.GetFiles(item_source_folder);
		//    string complete_destination_folder = DLC.Tools.Settings.DLC_UserSettings.Tivoli_Dropbox_Directory + item_destination_folder;
		//    foreach (string thisFile in files)
		//    {
		//        FileInfo thisFileInfo = new FileInfo(thisFile);
		//        bool previously_archived_identical = false;
		//        bool previously_archived = false;
		//        if (archived_files != null)
		//        {
		//            DataRow[] select = archived_files.Select("FileName='" + thisFileInfo.Name + "' and Folder='" + item_destination_folder + "'");
		//            DataRow[] old_select = archived_files.Select("FileName='" + thisFileInfo.Name + "' and Folder='" + possible_old_destination_folder + "'");
		//            if ((select.Length > 0) || (old_select.Length > 0))
		//            {
		//                previously_archived = true;
		//                if ((select.Length > 0) && (select[0]["Size"].ToString() == thisFileInfo.Length.ToString()) && (select[0]["LastWriteDate"].ToString() == thisFileInfo.LastWriteTime.ToString()))
		//                {
		//                    previously_archived_identical = true;
		//                }
		//                if ((old_select.Length > 0) && (old_select[0]["Size"].ToString() == thisFileInfo.Length.ToString()) && (old_select[0]["LastWriteDate"].ToString() == thisFileInfo.LastWriteTime.ToString()))
		//                {
		//                    previously_archived_identical = true;
		//                }
		//            }
		//        }

		//        if (!previously_archived_identical)
		//        {
		//            return_value++;
		//        }
		//    }

		//    string[] dirs = Directory.GetDirectories(item_source_folder);
		//    foreach (string subDir in dirs)
		//    {
		//        DirectoryInfo thisDirInfo = new DirectoryInfo(subDir);
		//        return_value += get_count_recurse_through_item_folder(item_source_folder + "\\" + thisDirInfo.Name, item_destination_folder + "\\" + thisDirInfo.Name, possible_old_destination_folder + "\\" + thisDirInfo.Name, archived_files);
		//    }

		//    return return_value;
		//}

		//private int recurse_through_item_folder(int total_file_count, int current_file_count, string item_source_folder, string item_destination_folder, string possible_old_destination_folder, DataTable archived_files)
		//{
		//    string[] files = Directory.GetFiles(item_source_folder);
		//    string complete_destination_folder = DLC.Tools.Settings.DLC_UserSettings.Tivoli_Dropbox_Directory + item_destination_folder;
		//    foreach (string thisFile in files)
		//    {
		//        FileInfo thisFileInfo = new FileInfo(thisFile);
		//        bool previously_archived_identical = false;
		//        bool previously_archived = false;
		//        if (archived_files != null)
		//        {
		//            DataRow[] select = archived_files.Select("FileName='" + thisFileInfo.Name + "' and Folder='" + item_destination_folder + "'");
		//            DataRow[] old_select = archived_files.Select("FileName='" + thisFileInfo.Name + "' and Folder='" + possible_old_destination_folder + "'");
		//            if ((select.Length > 0) || (old_select.Length > 0))
		//            {
		//                previously_archived = true;
		//                if ((select.Length > 0) && (select[0]["Size"].ToString() == thisFileInfo.Length.ToString()) && (select[0]["LastWriteDate"].ToString() == thisFileInfo.LastWriteTime.ToString()))
		//                {
		//                    previously_archived_identical = true;
		//                }
		//                if ((old_select.Length > 0) && (old_select[0]["Size"].ToString() == thisFileInfo.Length.ToString()) && (old_select[0]["LastWriteDate"].ToString() == thisFileInfo.LastWriteTime.ToString()))
		//                {
		//                    previously_archived_identical = true;
		//                }
		//            }
		//        }

		//        if (!previously_archived_identical)
		//        {
		//            // Determine the name to archive under?
		//            string new_name = complete_destination_folder + "\\" + thisFileInfo.Name;


		//            if (!Directory.Exists(complete_destination_folder))
		//            {
		//                Directory.CreateDirectory(complete_destination_folder);
		//            }

		//            OnNewProgress(current_file_count++, total_file_count);
		//            OnNewTask(String.Format(tivoli_message, thisFileInfo.Name), false);

		//            if (!File.Exists(new_name))
		//            {
		//                File.Copy(thisFile, new_name, true);
		//            }
		//            else
		//            {
		//                FileInfo previousFileInfo = new FileInfo(new_name);
		//                if ((previousFileInfo.Length != thisFileInfo.Length) || (previousFileInfo.LastWriteTime != thisFileInfo.LastWriteTime))
		//                {
		//                    File.Copy(thisFile, new_name, true);
		//                }
		//            }
		//        }
		//    }

		//    string[] dirs = Directory.GetDirectories(item_source_folder);
		//    foreach (string subDir in dirs)
		//    {
		//        DirectoryInfo thisDirInfo = new DirectoryInfo(subDir);
		//        current_file_count = recurse_through_item_folder(total_file_count, current_file_count, item_source_folder + "\\" + thisDirInfo.Name, item_destination_folder + "\\" + thisDirInfo.Name, possible_old_destination_folder + "\\" + thisDirInfo.Name, archived_files);
		//    }

		//    return current_file_count;
		//}

		#endregion
	}
}