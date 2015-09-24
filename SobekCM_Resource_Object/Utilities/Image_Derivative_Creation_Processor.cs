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
	public delegate void Image_Creation_New_Status_String_Delegate(string NewMessage, long ParentLogID, string BibID_VID);

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
		private bool catastrophic_failure_detected;
		private int consecutive_image_creation_error;
		private readonly bool create_qc_images;
		private bool errorEncountered;
		private bool first_jpeg_successfully_created;

		private readonly string image_magick_path;

		private readonly int jpeg_height;
		private readonly int jpeg_width;
		private readonly string kakadu_path;
		private readonly string temp_folder;
		private readonly int thumbnail_height;
		private readonly int thumbnail_width;
		private readonly bool create_jpeg2000_images;
		private readonly bool create_jpeg_images;


		/// <summary> Constructor for a new instance of the Image_Derivative_Creation_Processor class </summary>
		/// <param name="Image_Magick_Path"> Path (and executable name) for the image magick files for processing JPEG images </param>
		/// <param name="Kakadu_Path"> Path to the Kakadu library for creating JPEG2000's </param>
		/// <param name="Create_JPEGs"> Flag indicates whether JPEGs should be created </param>
		/// <param name="Create_JPEG2000s"> Flag indicates whether JPEG2000s should be created </param>
		/// <param name="JPEG_Width"> Width for the JPEGs to be generated </param>
		/// <param name="JPEG_Height"> Height for the JPEGs to be generated </param>
		/// <param name="Create_QC_Images"> Flag indicates if medium size JPEGs should be created for the QC windows application </param>
		/// <param name="Thumbnail_Width"> Width of the bounding box for the thumbnail </param>
		/// <param name="Thumbnail_Height"> Height of the bounding box for the thumbnail </param>
		/// <param name="TempFolder"> Temporary folder, if a temporary folder should be used for processing </param>
		public Image_Derivative_Creation_Processor(string Image_Magick_Path, string Kakadu_Path, bool Create_JPEGs, bool Create_JPEG2000s, int JPEG_Width, int JPEG_Height, bool Create_QC_Images, int Thumbnail_Width, int Thumbnail_Height, string TempFolder )
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
		    temp_folder = TempFolder;

		    //// Save the location for temporary files
		    //temp_folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
		    //temp_folder = temp_folder + "\\SobekCM\\Temporary";
		}

		/// <summary> Gets the count of the number of failures Kakadu experienced while creating JPEG2000 derivatives </summary>
		public int Kakadu_Failures { get; private set; }

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
	    /// <param name="TifFiles"> List of TIFF files </param>
	    /// <param name="ParentLogId"> Primary key to the parent log entery if this is performed by the builder </param>
	    /// <param name="Package_Directory"> Directory for the package </param>
	    /// <param name="BibID"> Bibliographic id for the item to derive images for </param>
	    /// <param name="VID"> Volume id for the item to derive images for </param>
	    public bool Process(string Package_Directory, string BibID, string VID, string[] TifFiles, long ParentLogId )
		{
            // Ensure the directory does not end in a '\' for later work
            if (Package_Directory[Package_Directory.Length - 1] == '\\')
                Package_Directory = Package_Directory.Substring(0, Package_Directory.Length - 1);

            string processFolder = Package_Directory;

            // Was a temporary processing folder location given?  If so and it doesn't exist create it
	        if (!String.IsNullOrEmpty(temp_folder))
	        {
	            try
	            {
	                // Create the temp folder
	                if (!Directory.Exists(temp_folder))
	                {
	                    Directory.CreateDirectory(temp_folder);
	                }
	                processFolder = temp_folder;
	            }
	            catch
	            {
	                // Just don't use the temp folder if unable to create it for whatever reason
	            }
	        }

	        Kakadu_Failures = 0;
			consecutive_image_creation_error = 0;
			catastrophic_failure_detected = false;

			errorEncountered = false;
			bool kakaduErrorEncountered = false;

			// Perform imaging work on all the files, in a try/catch
			try
			{
				if (Create_Derivative_Files(Package_Directory, TifFiles, BibID + ":" + VID, ParentLogId, processFolder ))
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
				OnErrorEncountered("Error encountered during image work on " + BibID + ":" + VID, ParentLogId, BibID + ":" + VID);
				OnErrorEncountered(ee.Message, ParentLogId, BibID + ":" + VID);
				errorEncountered = true;
			}

			// Add a warning if there were kakdu error
			if (kakaduErrorEncountered)
			{
				Kakadu_Failures++;
				OnErrorEncountered("WARNING: Error during JPEG2000 creation for " + BibID + ":" + VID, ParentLogId, BibID + ":" + VID);
			}

			// Delete the temporary folder
            // Was a temporary processing folder location given?  If so and it doesn't exist create it
            if (!String.IsNullOrEmpty(temp_folder))
            {
                try
                {
                    // Delete the temp folder
                    if ((Directory.Exists(temp_folder)) && ( Directory.GetFiles(temp_folder).Length == 0 ))
                    {
                        
                        Directory.Delete(temp_folder, true);
                    }
                }
                catch
                {
                    // No big whoop
                }
            }

			// Fire the process complete event
			if (!catastrophic_failure_detected)
			{
				OnProcessComplete();
				Thread.Sleep(2000);
			}
			else
			{
				OnErrorEncountered("ImageMagick Error Detected - Process Aborted", ParentLogId, BibID + ":" + VID);

				OnProcessComplete();
				Thread.Sleep(2000);
			}

			return errorEncountered;
		}

		private void OnNewProgress(int Value, int Max)
		{
			if (New_Progress != null)
			{
				New_Progress(Value, Value > Max ? Value : Max);
			}
		}

		private void OnNewTask(string NewMessage, long ParentLogID, string BibID_VID)
		{
			if (New_Task_String != null)
				New_Task_String(NewMessage, ParentLogID, BibID_VID);
		}

		private void OnErrorEncountered(string NewMessage, long ParentLogID, string BibID_VID)
		{
			if (Error_Encountered != null)
				Error_Encountered(NewMessage, ParentLogID, BibID_VID);
		}

		private void OnProcessComplete()
		{
			if (Process_Complete != null)
				Process_Complete(1, Kakadu_Failures);
		}

		#region Create Derivative Files

		private bool Create_Derivative_Files(string Directory, string[] TifFiles, string PackageName, long ParentLogId, string ProcessFolder )
		{
			bool kakadu_error = false;

            // Is the processing folder the same as the directory?
            bool useTemp = !Path.Equals(Directory, ProcessFolder);

			// Itereate through all the files in this volume
			int fileCtr = 1;
			int totalCount = TifFiles.Length;
			if (totalCount > 0)
			{
				OnNewTask("\t\tProcessing Images for " + PackageName, ParentLogId, PackageName);

				// Step through each TIF file
				foreach (string tifFile in TifFiles)
				{
					// Get the basic file information
					FileInfo tifFileInfo = new FileInfo(tifFile);
					string fileName = tifFileInfo.Name;
					string fileNameUpper = fileName.ToUpper();
					if (fileNameUpper.IndexOf("_ARCHIVE") < 0)
					{
						string tiffNameSansExtension = tifFileInfo.Name.Replace(tifFileInfo.Extension, "");
						string rootName = Directory + "\\" + tiffNameSansExtension;

						// Add to log
						OnNewTask("\t\t\tProcessing Image '" + fileName + "'", ParentLogId, PackageName);
						OnNewProgress(fileCtr++, (totalCount + 1));


						// Get the date for this file and verify thumbnail, jpeg, and jp2 exist
						if ((((!File.Exists(rootName + ".jpg")) || (!File.Exists(rootName + "thm.jpg")))
							|| (File.GetLastWriteTime(rootName + ".jpg").CompareTo(File.GetLastWriteTime(tifFile)) < 0)) ||
                            ((!File.Exists(rootName + ".jp2")) || (File.GetLastWriteTime(rootName + ".jp2").CompareTo(File.GetLastWriteTime(tifFile)) < 0)))
						{
							// We'll do the processing on our local machine to avoid pulling the data from the SAN multiple times
                            string useFile = tifFile;

						    if (useTemp)
						    {
						        string localTempFile = temp_folder + "\\TEMP.tif";
						        try
						        {
						            if (File.Exists(localTempFile))
						            {
						                File.Delete(localTempFile);
						            }
						            File.Copy(tifFile, localTempFile, true);
                                    useFile = localTempFile;
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
                                        useFile = localTempFile;
						            }
						            catch
						            {
						                // Okay.. I guess we won't use the temp spot
                                        
						            }
						        }
						    }

						    // Process this file, as necessary
							if ( create_jpeg_images )
                                Image_Magick_Process_TIFF_File(useFile, tiffNameSansExtension, Directory, true, jpeg_width, jpeg_height, ParentLogId, PackageName);

							// Create the JPEG2000
							if ( create_jpeg2000_images )
                                kakadu_error = !Create_JPEG2000(useFile, tiffNameSansExtension, Directory, ParentLogId, PackageName);
						}
					}
				}
			}

			return kakadu_error;
		}

		#endregion

		#region ImageMagick all files

		/// <summary> Process this image via ImageMagick, to create the needed jpeg derivative(s) </summary>
		/// <param name="LocalTempFile"> Complete name (including directory) of the TIFF file to actually process, which is often in a temporary location </param>
		/// <param name="ThisTiffFile"> Name (excluding extension and directory) for the original TIFF file, to be used for naming of the derivatives </param>
		/// <param name="VolumeDirectory"> Directory where the derivatives should be created </param>
		/// <param name="MakeThumbnail"> Flag indicates whether a thumbnail image should be generated for this TIFF </param>
		/// <param name="Width"> Requested width limit of the resulting full-size jpeg image to create </param>
		/// <param name="Height"> Requested height limit of the resulting full-size jpeg image to create </param>
		/// <param name="ParentLogId"> Primary key to the parent log entery if this is performed by the builder </param>
		/// <param name="PackageName"> Name of the package this file belongs to ( BibID : VID )</param>
		public void Image_Magick_Process_TIFF_File(string LocalTempFile, string ThisTiffFile, string VolumeDirectory, bool MakeThumbnail, int Width, int Height, long ParentLogId, string PackageName)
		{
			// Get the full file name
			string rootName = VolumeDirectory + "\\" + ThisTiffFile;

			try
			{
				// Make a thumbnail image, if necessary
				if ((MakeThumbnail) && (thumbnail_height > 0) && (thumbnail_width > 0))
				{
					// Save a 150 pixel wide JPEG
					ImageMagick_Create_JPEG(LocalTempFile, rootName + "thm.jpg", thumbnail_width, thumbnail_height, ParentLogId, PackageName);
				}

				// Save a 630 pixel wide JPEG
				ImageMagick_Create_JPEG(LocalTempFile, rootName + ".jpg", Width, Height, ParentLogId, PackageName);

				if (catastrophic_failure_detected)
				{
					return;
				}

				// Save a 315 pixel wide JPEG
				if (create_qc_images)
				{
					ImageMagick_Create_JPEG(LocalTempFile, rootName + ".QC.jpg", 315, 500, ParentLogId, PackageName);
				}
			}
			catch (Exception ee)
			{
				OnErrorEncountered("Error caught during image derivative creation: " + ee.Message, ParentLogId, PackageName);
				consecutive_image_creation_error++;
			}
		}

		/// <summary> Use ImageMagick to create a JPEG derivative file </summary>
		/// <param name="Sourcefile"> Source file </param>
		/// <param name="Finalfile"> Final file</param>
		/// <param name="Width"> Width restriction for the resulting jpeg </param>
		/// <param name="Height"> Height restriction for the resulting jpeg</param>
		/// <param name="ParentLogId"> Primary key to the parent log entery if this is performed by the builder </param>
		/// <param name="PackageName"> Name of the package this file belongs to ( BibID : VID )</param>
		public void ImageMagick_Create_JPEG(string Sourcefile, string Finalfile, int Width, int Height, long ParentLogId, string PackageName)
		{
			try
			{
				// Start this process
				Process convert = new Process {StartInfo = {WindowStyle = ProcessWindowStyle.Minimized, CreateNoWindow = true, ErrorDialog = true, RedirectStandardError = true, UseShellExecute = false}};
				if (image_magick_path.ToUpper().IndexOf("CONVERT.EXE") > 0)
					convert.StartInfo.FileName = image_magick_path;
				else
					convert.StartInfo.FileName = image_magick_path + "\\convert.exe";
				convert.StartInfo.Arguments = "-geometry " + Width + "x" + Height + " \"" + Sourcefile + "\"[0] \"" + Finalfile + "\"";

				// Start
				convert.Start();

				// Check for any error
				StreamReader readError = convert.StandardError;
				string error = readError.ReadToEnd();

				// Make sure it is complete
				convert.WaitForExit();
				convert.Dispose();

				// If the final file did not appear, there was a problem
				if (!File.Exists(Finalfile))
				{
					if (error.Length > 0)
					{
						// Image Magick did not function!
						//      Tools.Forms.ErrorMessageBox.Show("Error encountered during ImageMagick execution; no JPEG created!\n\n" + error , MessageProvider_Gateway.ImageMagick_Error_Title, new ApplicationException("Attempted to execute command:\n\t" + command));
						if (!first_jpeg_successfully_created)
						{
							catastrophic_failure_detected = true;
						}
						OnErrorEncountered("Error encountered during ImageMagick execution; no JPEG created!", ParentLogId, PackageName);
						OnErrorEncountered(error, ParentLogId, PackageName);
						consecutive_image_creation_error++;
						errorEncountered = true;
						Kakadu_Failures++;
					}
					else
					{
						if (!first_jpeg_successfully_created)
						{
							catastrophic_failure_detected = true;
						}
						OnErrorEncountered("No error discovered during ImageMagick execution, but no JPEG was created either!", ParentLogId, PackageName);

						consecutive_image_creation_error++;
						errorEncountered = true;
						Kakadu_Failures++;
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
				Kakadu_Failures++;
			}
		}

		#endregion

		#region Create JPEG2000 Files

		/// <summary> Creates a JPEG2000 derivative service file, according to NDNP specs, for display 
		/// within a SobekCM library </summary>
		/// <param name="LocalTempFile"> Complete name (including directory) of the TIFF file to actually process, which is often in a temporary location </param>
		/// <param name="Filename"> Name of the resulting JPEG2000 file </param>
		/// <param name="Directory"> Directory where the resulting JPEG2000 should be created ( usually the directory for the volume )</param>
		/// <param name="ParentLogId"> Primary key to the parent log entery if this is performed by the builder </param>
		/// <param name="PackageName"> Name of the package this file belongs to ( BibID : VID )</param>
		/// <returns>TRUE if successful, otherwise FALSE</returns>
		public bool Create_JPEG2000(string LocalTempFile, string Filename, string Directory, long ParentLogId, string PackageName)
		{
			bool returnVal = true;

			// Create the JPEG2000
			string rootFile = Directory + "\\" + Filename;
			string fullFileName = rootFile + ".tif";

			if ((!File.Exists(rootFile + ".jp2")) || (File.GetLastWriteTime(rootFile + ".jp2").CompareTo(File.GetLastWriteTime(fullFileName)) < 0))
			{
				// Save the JPEG2000
				returnVal = Kakadu_Create_JPEG2000(LocalTempFile, rootFile + ".jp2", ParentLogId, PackageName);
			}

			try
			{
				File.Delete(LocalTempFile);
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


		private bool Kakadu_Create_JPEG2000(string Sourcefile, string Finalfile, long ParentLogId, string PackageName )
		{
			bool returnVal = true;

			// Start this process
			Process convert = new Process
				{
					StartInfo = {WindowStyle = ProcessWindowStyle.Minimized, CreateNoWindow = true, ErrorDialog = true, RedirectStandardError = true, UseShellExecute = false, FileName = kakadu_path + "\\kdu_compress_libtiff.exe", Arguments = " -i \"" + Sourcefile + "\" -o \"" + Finalfile + "\" -rate 1.0,0.84,0.7,0.6,0.5,0.4,0.35,0.3,0.25,0.21,0.18,0.15,0.125,0.1,0.088,0.075,0.0625,0.05,0.04419,0.03716,0.03125,0.025,0.0221,0.01858,0.015625 Clevels=6 Stiles={1024,1024} Corder=RLCP Cblk={64,64} Sprofile=PROFILE1"}
				};

			convert.Start();

			// Check for any error
			StreamReader readError = convert.StandardError;
			string error = readError.ReadToEnd();

			// Make sure it is complete
			convert.WaitForExit();
			convert.Dispose();

			if (error.Length > 0)
			{
				returnVal = false;
				if (File.Exists(Finalfile))
				{
					OnErrorEncountered("WARNING: " + error, ParentLogId, PackageName);
				}
				else
				{
					OnErrorEncountered("ERROR: " + error, ParentLogId, PackageName);
				}
			}

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