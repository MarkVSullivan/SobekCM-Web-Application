#region Using directives

using System.Drawing;
using System.IO;
using SobekCM.Resource_Object.Divisions;

#endregion

namespace SobekCM.Builder_Library.Modules.Items
{
    public class UpdateJpegAttributesModule : abstractSubmissionPackageModule
    {
        public override bool DoWork(Incoming_Digital_Resource Resource)
        {
            // Now, just look for the data being present in each file
            if (Directory.Exists(Resource.Resource_Folder))
            {
                foreach (SobekCM_File_Info thisFile in Resource.Metadata.Divisions.Files)
                {
                    // Is this a jpeg?
                    if (thisFile.System_Name.ToUpper().IndexOf(".JPG") > 0)
                    {
                        if (thisFile.System_Name.ToUpper().IndexOf("THM.JPG") < 0)
                            Compute_Jpeg_Attributes(thisFile, Resource.Resource_Folder);
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
            // If the width and height are already determined, done!
            if ((JPEG_File.Width > 0) && (JPEG_File.Height > 0))
                return true;

            // Does this file exist?
            if (File.Exists(File_Location + "/" + JPEG_File.System_Name))
            {
                try
                {
                    // Get the height and width of this JPEG file
                    Bitmap image = (Bitmap)Image.FromFile(File_Location + "/" + JPEG_File.System_Name);
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
                int[] previousValues = new[] { 0, 0, 0, 0 };
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
