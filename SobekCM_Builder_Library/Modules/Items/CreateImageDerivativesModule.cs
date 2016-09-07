#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using SobekCM.Builder_Library.Settings;
using SobekCM.Engine_Library.Email;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Resource_Object.Utilities;

#endregion

namespace SobekCM.Builder_Library.Modules.Items
{
    /// <summary> Item-level submission package module creates all the image derivative files from original jpeg and tiff files </summary>
    /// <remarks> This class implements the <see cref="abstractSubmissionPackageModule" /> abstract class and implements the <see cref="iSubmissionPackageModule" /> interface. </remarks>
    public class CreateImageDerivativesModule : abstractSubmissionPackageModule
    {
        private bool returnValue;
        private string[] image_extensions = {".jpg", ".tif", ".png", ".gif", ".jp2"};

        /// <summary> Creates all the image derivative files from original jpeg and tiff files </summary>
        /// <param name="Resource"> Incoming digital resource object </param>
        /// <returns> TRUE if processing can continue, FALSE if a critical error occurred which should stop all processing </returns>
        public override bool DoWork(Incoming_Digital_Resource Resource)
        {
            returnValue = true;

            string resourceFolder = Resource.Resource_Folder;
            string imagemagick_executable = MultiInstance_Builder_Settings.ImageMagick_Executable;



            // Are there images that need to be processed here?
            if (!String.IsNullOrEmpty(imagemagick_executable))
            {
                // Get the list of image files first
                List<string> imageFiles = new List<string>();
                foreach (string imageExtension in image_extensions)
                {
                    imageFiles.AddRange(Directory.GetFiles(resourceFolder, "*" + imageExtension));
                }

                // Only continue if some exist
                if (imageFiles.Count > 0)
                {
                    // Build the list of files listed in the metadata
                    Dictionary<string, SobekCM_File_Info> names_to_mets_file = new Dictionary<string, SobekCM_File_Info>(StringComparer.OrdinalIgnoreCase);
                    List<SobekCM_File_Info> files = Resource.Metadata.Divisions.Physical_Tree.All_Files;
                    foreach (SobekCM_File_Info thisFile in files)
                    {
                        names_to_mets_file[thisFile.System_Name] = thisFile;
                    }

                    // Step through all the image files and find the collection of page images
                    Dictionary<string, List<string>> imageRootFiles = new Dictionary<string, List<string>>( StringComparer.OrdinalIgnoreCase );
                    List<string> possibleThumbnails = new List<string>();
                    foreach (string thisImageFile in imageFiles)
                    {
                        // Skip .QC.JPG files
                        if (thisImageFile.IndexOf(".qc.jpg", StringComparison.OrdinalIgnoreCase) > 0)
                            continue;

                        // If this might be a thumbnail image, save it for the very end for analysis
                        if ((thisImageFile.IndexOf("thm.jpg", StringComparison.OrdinalIgnoreCase) > 0) && ( Path.GetFileNameWithoutExtension(thisImageFile).Length > 3 ))
                        {
                            // Save for final analysis
                            possibleThumbnails.Add(thisImageFile);
                        }
                        else
                        {
                            // Get this filename without the extension
                            string filename_sans_extension = Path.GetFileNameWithoutExtension(thisImageFile);

                            // Has this root, or image grouping, already been analyzed?
                            if (imageRootFiles.ContainsKey(filename_sans_extension))
                            {
                                imageRootFiles[filename_sans_extension].Add(thisImageFile);
                            }
                            else
                            {
                                imageRootFiles.Add(filename_sans_extension, new List<string> {thisImageFile});
                            }
                        }
                    }

                    // Now, re-analyze those files that could have potentially been a thumbnail jpeg
                    foreach (string thisPossibleThumbnail in possibleThumbnails)
                    {
                        // Get this filename without the extension
                        string filename_sans_extension = Path.GetFileNameWithoutExtension(thisPossibleThumbnail);

                        // Remove the final 'thm' from the name first and look for a match
                        string filename_sans_thumb_extension = filename_sans_extension.Substring(0, filename_sans_extension.Length - 3);

                        // Has this root, or image grouping, already been analyzed?
                        if (imageRootFiles.ContainsKey(filename_sans_thumb_extension))
                        {
                            imageRootFiles[filename_sans_thumb_extension].Add(thisPossibleThumbnail);
                        }
                        else
                        {
                            imageRootFiles.Add(filename_sans_extension, new List<string> { thisPossibleThumbnail });
                        }
                    }

                    // Create the image process object for creating 
                    Image_Derivative_Creation_Processor imageProcessor = new Image_Derivative_Creation_Processor(imagemagick_executable, null, true, true, Settings.Resources.JPEG_Width, Settings.Resources.JPEG_Height, false, Settings.Resources.Thumbnail_Width, Settings.Resources.Thumbnail_Height, null);
                    imageProcessor.New_Task_String += imageProcessor_New_Task_String;
                    imageProcessor.Error_Encountered += imageProcessor_Error_Encountered;

                    // Step through each file grouping and look for the newest file and jpeg and thumbnail dates
                    string jpeg_file;
                    string jpeg_thumb_file;
                    string jpeg2000_file;
                    foreach (string thisImageRoot in imageRootFiles.Keys)
                    {
                        // Ready for the next set of images
                        jpeg_file = String.Empty;
                        jpeg_thumb_file = String.Empty;
                        jpeg2000_file = String.Empty;

                        // Get the list of all related files
                        List<string> theseImageFiles = imageRootFiles[thisImageRoot];

                        // Look for the jpeg and thumbnail derivatives
                        int image_index = 0;
                        while (image_index < theseImageFiles.Count)
                        {
                            // Get the extenxstion of this file
                            string extension = Path.GetExtension(theseImageFiles[image_index]).ToUpper();

                            // Was this a special image file type (i.e., jpeg or jpeg2000?)
                            if ((extension == ".JPG") || (extension == ".JP2"))
                            {
                                // If JPEG, does this appear to be the thumbnail?
                                if (extension == ".JPG")
                                {
                                    if (String.Compare(Path.GetFileNameWithoutExtension(theseImageFiles[image_index]), thisImageRoot + "thm", StringComparison.OrdinalIgnoreCase) == 0)
                                    {
                                        jpeg_thumb_file = theseImageFiles[image_index];
                                    }
                                    else
                                    {
                                        jpeg_file = theseImageFiles[image_index];
                                    }
                                }
                                else
                                {
                                    jpeg2000_file = theseImageFiles[image_index];
                                }

                                // Since this was a standard derivative file, remove it from the list (and don't icrement image_index)
                                theseImageFiles.RemoveAt(image_index);
                            }
                            else
                            {
                                // Since this looks like source image (and not a standard derivative)
                                // just keep it in the list and move to the next one
                                image_index++;
                            }
                        }


                        // Having separated the derivatives from the possible source files, let's determine if derivatives should be created
                        // based on the dates for the files
                        DateTime? jpeg_file_lastModTime = null;
                        if (!String.IsNullOrEmpty(jpeg_file))
                            jpeg_file_lastModTime = File.GetLastWriteTime(jpeg_file);

                        DateTime? jpeg_thumb_file_lastModTime = null;
                        if ( !String.IsNullOrEmpty(jpeg_thumb_file))
                            jpeg_thumb_file_lastModTime = File.GetLastWriteTime(jpeg_thumb_file);

                        DateTime? jpeg2000_file_lastModTime = null;
                        if ( !String.IsNullOrEmpty(jpeg2000_file))
                            jpeg2000_file_lastModTime = File.GetLastWriteTime(jpeg2000_file);

                        // Were there some ordinary source files left, that may need to be analyzed?
                        if (theseImageFiles.Count > 0)
                        {
                            // Keep track of newest source file and date
                            string newest_source_file = String.Empty;
                            DateTime newest_source_file_date = new DateTime(1900, 1, 1);

                            // Find the newest source file
                            foreach (string thisSourceFile in theseImageFiles)
                            {
                                DateTime lastModTime = File.GetLastWriteTime(thisSourceFile);
                                if (lastModTime.CompareTo(newest_source_file_date) > 0)
                                {
                                    newest_source_file_date = lastModTime;
                                    newest_source_file = thisSourceFile;
                                }
                            }

                            // Now, see if some of the basic derivatives are missing or too old
                            if (((!jpeg_file_lastModTime.HasValue) || (jpeg_file_lastModTime.Value.CompareTo(newest_source_file_date) < 0)) ||
                                ((!jpeg_thumb_file_lastModTime.HasValue) || (jpeg_thumb_file_lastModTime.Value.CompareTo(newest_source_file_date) < 0)) ||
                                ((!jpeg2000_file_lastModTime.HasValue) || (jpeg2000_file_lastModTime.Value.CompareTo(newest_source_file_date) < 0)))
                            {
                                // Create all the derivatives
                                string name_sans_extension = Path.GetFileNameWithoutExtension(newest_source_file);

                                // Create the JPEG derivatives from the JPEG2000
                                imageProcessor.ImageMagick_Create_JPEG(newest_source_file, resourceFolder + "\\" + name_sans_extension + "thm.jpg", Settings.Resources.Thumbnail_Width, Settings.Resources.Thumbnail_Height, Resource.BuilderLogId, Resource.BibID + ":" + Resource.VID);
                                imageProcessor.ImageMagick_Create_JPEG(newest_source_file, resourceFolder + "\\" + name_sans_extension + ".jpg", Settings.Resources.JPEG_Width, Settings.Resources.JPEG_Height, Resource.BuilderLogId, Resource.BibID + ":" + Resource.VID);

                                // If the JPEG exists with width/height information clear the information
                                if (names_to_mets_file.ContainsKey(name_sans_extension + ".jpg"))
                                {
                                    names_to_mets_file[name_sans_extension + ".jpg"].Height = 0;
                                    names_to_mets_file[name_sans_extension + ".jpg"].Width = 0;
                                }
                            }
                        }
                        else
                        {
                            // No derivate source files found, but we may build the derivatives from the JPEG2000 file
                            if (!String.IsNullOrEmpty(jpeg2000_file))
                            {
                                //if (( jpeg_file_lastModTime.HasValue ) && ( jpeg_file_lastModTime.Value.Month == 9 ) && ( jpeg_file_lastModTime.Value.Day == 6 ))

                                // Now, see if the other derivatives are missing or too old
                                if (((!jpeg_file_lastModTime.HasValue) || (jpeg_file_lastModTime.Value.CompareTo(jpeg2000_file_lastModTime) < 0)) ||
                                    ((!jpeg_thumb_file_lastModTime.HasValue) || (jpeg_thumb_file_lastModTime.Value.CompareTo(jpeg2000_file_lastModTime) < 0)))
                                {
                                    string name_sans_extension = Path.GetFileNameWithoutExtension(jpeg2000_file);

                                    //// Create a temporary, full-size file
                                    //string temp_file = resourceFolder + "\\" + name_sans_extension + "_sobektemp.tif";
                                    //imageProcessor.ImageMagick_Create_JPEG(jpeg2000_file, temp_file, -1, -1, Resource.BuilderLogId, Resource.BibID + ":" + Resource.VID);


                                    // Create the JPEG derivatives from the JPEG2000
                                    imageProcessor.ImageMagick_Create_JPEG(jpeg2000_file, resourceFolder + "\\" + name_sans_extension + "thm.jpg", Settings.Resources.Thumbnail_Width, Settings.Resources.Thumbnail_Height, Resource.BuilderLogId, Resource.BibID + ":" + Resource.VID);
                                    imageProcessor.ImageMagick_Create_JPEG(jpeg2000_file, resourceFolder + "\\" + name_sans_extension + ".jpg", Settings.Resources.JPEG_Width, Settings.Resources.JPEG_Height, Resource.BuilderLogId, Resource.BibID + ":" + Resource.VID);

                                    // If the JPEG exists with width/height information clear the information
                                    if (names_to_mets_file.ContainsKey(name_sans_extension + ".jpg"))
                                    {
                                        names_to_mets_file[name_sans_extension + ".jpg"].Height = 0;
                                        names_to_mets_file[name_sans_extension + ".jpg"].Width = 0;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return returnValue;
        }

        void imageProcessor_New_Task_String(string NewMessage, long ParentLogID, string BibID_VID)
        {
            OnProcess(NewMessage, "Image Processing", BibID_VID, String.Empty, ParentLogID);
        }

        void imageProcessor_Error_Encountered(string NewMessage, long ParentLogID, string BibID_VID)
        {
            if (NewMessage.IndexOf("WARNING: ") == 0)
            {
                OnProcess(NewMessage, "Image Processing", BibID_VID, String.Empty, ParentLogID);
            }
            else
            {
                // Put this in the builder logs
                OnError(NewMessage, BibID_VID, String.Empty, ParentLogID);

                // Email a message
                string email_address = Settings.Email.System_Error_Email;
                if (String.IsNullOrWhiteSpace(email_address))
                    email_address = Settings.Email.System_Email;
                if (!String.IsNullOrEmpty(email_address))
                {
                    Email_Helper.SendEmail(email_address, "Image Derivation Error : " + BibID_VID, "An error was encountered while creating images for the web from the provided files in the SobekCM Builder service.  Processing of this item will be incomplete.\n\n" + NewMessage + "\n\nPlease review this item and correct the issue, most likely by checking the TIFFs and reloading them.", false, Settings.System.System_Name);
                }

                // This will indicate a failure
                returnValue = false;
            }
        }
    }
}
