#region Using directives

using System;
using System.Drawing;
using System.IO;
using SobekCM.Resource_Object.Divisions;

#endregion

namespace SobekCM.Builder_Library.Modules.Items
{
    /// <summary> Item-level submission package module updates the basic dimensional information stored for all of the JPEG files 
    /// within the service METS file </summary>
    /// <remarks> This class implements the <see cref="abstractSubmissionPackageModule" /> abstract class and implements the <see cref="iSubmissionPackageModule" /> interface. </remarks>
    public class UpdateJpegAttributesModule : abstractSubmissionPackageModule
    {
        /// <summary> Updates the basic dimensional information stored for all of the JPEG files 
        /// within the service METS file </summary>
        /// <param name="Resource"> Incoming digital resource object </param>
        /// <returns> TRUE if processing can continue, FALSE if a critical error occurred which should stop all processing </returns>
        public override bool DoWork(Incoming_Digital_Resource Resource)
        {
            // Now, just look for the data being present in each file
            if (Directory.Exists(Resource.Resource_Folder))
            {
                // Now, step through each file
                foreach (SobekCM_File_Info thisFile in Resource.Metadata.Divisions.Files)
                {
                    // Does this exist?
                    string file_in_resource_folder = Path.Combine(Resource.Resource_Folder, thisFile.System_Name);
                    if (!File.Exists(file_in_resource_folder))
                        continue;

                    // Is this a jpeg?
                    if (thisFile.System_Name.ToUpper().IndexOf(".JPG") > 0)
                    {
                        if (thisFile.System_Name.ToUpper().IndexOf("THM.JPG") < 0)
                        {
                            // JPEG attributes are ALWAYS re-calculated
                            Compute_Jpeg_Attributes(thisFile, Resource.Resource_Folder);
                        }
                    }

                    // Is this a jpeg2000?
                    if (thisFile.System_Name.ToUpper().IndexOf("JP2") > 0)
                    {
                        Compute_Jpeg2000_Attributes(thisFile, Resource.Resource_Folder);
                    }
                }
            }

            return true;
        }

        /// <summary> Computes the attributes (width, height) for a JPEG file </summary>
        /// <param name="JPEG_File"> METS SobekCM_File_Info object for this jpeg file </param>
        /// <param name="File_Location"> Location where this file exists </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> The attribute information is computed and then stored in the provided METS SobekCM_File_Info object </remarks>
        private bool Compute_Jpeg_Attributes(SobekCM_File_Info JPEG_File, string File_Location)
        {
            // Does this file exist?
            string file_in_place = Path.Combine(File_Location, JPEG_File.System_Name);
            if (File.Exists(file_in_place))
            {
                try
                {
                    // Get the height and width of this JPEG file
                    Bitmap image = (Bitmap)Image.FromFile(file_in_place);
                    JPEG_File.Width = (ushort)image.Width;
                    JPEG_File.Height = (ushort)image.Height;
                    image.Dispose();
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary> Computes the attributes (width, height) for a JPEG2000 file </summary>
        /// <param name="JPEG2000_File"> METS SobekCM_File_Info object for this jpeg2000 file </param>
        /// <param name="File_Location"> Location where this file exists </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> The attribute information is computed and then stored in the provided METS SobekCM_File_Info object </remarks>
        private bool Compute_Jpeg2000_Attributes(SobekCM_File_Info JPEG2000_File, string File_Location)
        {
            // If the width and height are already determined, done!
            if ((JPEG2000_File.Width > 0) && (JPEG2000_File.Height > 0) && (JPEG2000_File.System_Name.Length > 0))
                return true;

            // Does this file exist?
            if (File.Exists(File_Location + "/" + JPEG2000_File.System_Name))
            {
                return get_attributes_from_jpeg2000(JPEG2000_File, File_Location + "/" + JPEG2000_File.System_Name);
            }

            if ((JPEG2000_File.System_Name.Length > 0) && (File.Exists(JPEG2000_File.System_Name)))
            {
                return get_attributes_from_jpeg2000(JPEG2000_File, JPEG2000_File.System_Name);
            }

            return false;
        }

        private bool get_attributes_from_jpeg2000(SobekCM_File_Info JPEG2000_File, string File)
        {
            try
            {
                // Get the height and width of this JPEG file
                FileStream reader = new FileStream(File, FileMode.Open, FileAccess.Read);
                int[] previousValues = { 0, 0, 0, 0 };
                int bytevalue = reader.ReadByte();
                int count = 1;
                while (bytevalue != -1)
                {
                    // Move this value into the array
                    previousValues[0] = previousValues[1];
                    previousValues[1] = previousValues[2];
                    previousValues[2] = previousValues[3];
                    previousValues[3] = bytevalue;

                    // Is this IHDR?
                    if ((previousValues[0] == 105) && (previousValues[1] == 104) &&
                        (previousValues[2] == 100) && (previousValues[3] == 114))
                    {
                        break;
                    }

                    // Is this the first four bytes and does it match the output from Kakadu 3-2?
                    if ((count == 4) && (previousValues[0] == 255) && (previousValues[1] == 79) &&
                        (previousValues[2] == 255) && (previousValues[3] == 81))
                    {
                        reader.ReadByte();
                        reader.ReadByte();
                        reader.ReadByte();
                        reader.ReadByte();
                        break;
                    }

                    // Read the next byte
                    bytevalue = reader.ReadByte();
                    count++;
                }

                // Now, read ahead for the height and width
                JPEG2000_File.Height = (ushort)((((((reader.ReadByte() * 256) + reader.ReadByte()) * 256) + reader.ReadByte()) * 256) + reader.ReadByte());
                JPEG2000_File.Width = (ushort)((((((reader.ReadByte() * 256) + reader.ReadByte()) * 256) + reader.ReadByte()) * 256) + reader.ReadByte());
                reader.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
