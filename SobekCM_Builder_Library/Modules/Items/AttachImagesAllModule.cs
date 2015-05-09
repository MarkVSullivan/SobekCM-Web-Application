using System.IO;
using SobekCM.Resource_Object.Behaviors;

namespace SobekCM.Builder_Library.Modules.Items
{
    class AttachImagesAllModule : abstractSubmissionPackageModule
    {
        public override bool DoWork(Incoming_Digital_Resource Resource)
        {
            bool jpeg_added = false;
            bool jpeg2000_added = false;

            // Ensure all non-image files are linked to the METS file
            string[] all_files = SobekCM.Tools.SobekCM_File_Utilities.GetFiles(Resource.Resource_Folder, "*.jp2|*.jpg");
            foreach (string thisFile in all_files)
            {
                string filename = Path.GetFileName(thisFile);
                string filename_sans_extension = Path.GetFileNameWithoutExtension(thisFile);
                string extension = Path.GetExtension(thisFile).ToLower().Replace(".", "");

                // Try to just always add it
                Resource.Metadata.Divisions.Physical_Tree.Add_File(filename);

                // Also, check to see if this is a jpeg or jpeg2000
                if (extension == "jp2")
                    jpeg2000_added = true;
                if (extension == "jpg")
                    jpeg_added = true;
            }

            // Ensure proper views are attached to this item
            if ((jpeg2000_added) || (jpeg_added))
            {
                Resource.Metadata.Behaviors.Add_View(View_Enum.JPEG);
                if (jpeg_added)
                {
                    Resource.Metadata.Behaviors.Add_View(View_Enum.JPEG);
                    Resource.Metadata.Behaviors.Add_View(View_Enum.RELATED_IMAGES);
                    if (jpeg2000_added)
                        Resource.Metadata.Behaviors.Add_View(View_Enum.JPEG2000);
                }
                else
                {
                    Resource.Metadata.Behaviors.Add_View(View_Enum.JPEG2000);
                }
            }

            return true;
        }
    }
}
