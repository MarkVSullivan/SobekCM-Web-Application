#region Using directives

using System;
using System.IO;

#endregion

namespace SobekCM.Resource_Object.Utilities
{
    /// <summary> Utility is used to move or copy a SobekCM item (digital resource) to a new location </summary>
    public static class SobekCM_Item_Mover
    {
        /// <summary> Enumeration tells whether to MOVE or COPY the files </summary>
        public enum SobekCM_Item_Move_Type_Enum : byte
        {
            /// <summary> Move the files to the new location </summary>
            MOVE = 1,

            /// <summary> Copy the files to the new location </summary>
            COPY
        }

        /// <summary> Moves all the applicable resource files from the current location to under the destination parent provided </summary>
        /// <param name="Source_Folder"> Folder for the resource item, within which the METS file should exist </param>
        /// <param name="Destination_Parent"> Folder under which to create the new digital resource folder for the new files </param>
        /// <param name="OverWrite"> Flag indicates if any existing files should be overwritten </param>
        /// <param name="Include_TIFFs"> Flag indicates if TIFFs should be moved as well </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> This does not read the MET to decide which files to move, it is just slightly smarter than a normal move </remarks>
        public static bool Move(string Source_Folder, string Destination_Parent, bool OverWrite, bool Include_TIFFs)
        {
            string error_message;

            return MOve_Or_Copy(SobekCM_Item_Move_Type_Enum.MOVE, Source_Folder, Destination_Parent, OverWrite, Include_TIFFs, out error_message );
        }

        /// <summary> Moves all the applicable resource files from the current location to under the destination parent provided </summary>
        /// <param name="MoveOrCopy"> Flag indicates wheter to MOVE or COPY the files </param>
        /// <param name="Source_Folder"> Folder for the resource item, within which the METS file should exist </param>
        /// <param name="Destination_Parent"> Folder under which to create the new digital resource folder for the new files </param>
        /// <param name="OverWrite"> Flag indicates if any existing files should be overwritten </param>
        /// <param name="Include_TIFFs"> Flag indicates if TIFFs should be moved as well </param>
        /// <param name="ErrorMessage"> Error message for errors that occur </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> This does not read the MET to decide which files to move, it is just slightly smarter than a normal move </remarks>
        public static bool MOve_Or_Copy(SobekCM_Item_Move_Type_Enum MoveOrCopy, string Source_Folder, string Destination_Parent, bool OverWrite, bool Include_TIFFs, out string ErrorMessage )
        {
            ErrorMessage = String.Empty;
            bool returnValue = true;
            int errors = 0;

            // Ensure the source exists
            if (!Directory.Exists(Source_Folder))
            {
                ErrorMessage = "Source folder (" + Source_Folder + ") does not exist";
                return false;
            }

            // Ensure the destination exists
            if (!Directory.Exists(Destination_Parent))
            {
                ErrorMessage = "Destination (" + Destination_Parent + ") does not exist";
                return false;
            }

            // Get the current folder name and destination name
            string folder_name = (new DirectoryInfo(Source_Folder)).Name;
            string destination_name = Destination_Parent + "\\" + folder_name;

            try
            {
                if (!Directory.Exists(destination_name))
                    Directory.CreateDirectory(destination_name);
            }
            catch ( Exception ee )
            {
                ErrorMessage = "Unable to create destination folder ( " + ee.Message + " )";
                return false;
            }

            // Get the list of all files under the current folder
            string[] files = Directory.GetDirectories(Source_Folder);
            foreach (string thisFile in files)
            {
                FileInfo fileInfo = new FileInfo(thisFile);
                string filename = fileInfo.Name;
                string destination_file = destination_name + "\\" + filename;

                string extension = fileInfo.Extension.ToUpper();
                if ((extension.IndexOf(".TIF") < 0) || (Include_TIFFs))
                {
                    try
                    {
                        if ((!File.Exists(destination_file)) || (OverWrite))
                        {
                            if (MoveOrCopy == SobekCM_Item_Move_Type_Enum.MOVE)
                            {
                                File.Delete(destination_file);
                                File.Move(thisFile, destination_file);
                            }
                            else
                            {
                                File.Copy(thisFile, destination_file, true);
                            }
                        }
                    }
                    catch
                    {
                        errors++;
                        returnValue = false;
                    }
                }

                // If up to five errors, abort the whole thing
                if (errors >= 5)
                {
                    ErrorMessage = "Five or more errors occurred during the file move/copy";
                    return false;
                }
            }

            return returnValue;
        }
    }
}
