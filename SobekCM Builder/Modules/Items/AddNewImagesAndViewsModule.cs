using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SobekCM.Library.Builder;
using SobekCM.Resource_Object.Behaviors;

namespace SobekCM.Builder.Modules.Items
{
    public class AddNewImagesAndViewsModule : abstractSubmissionPackageModule
    {
        public override void DoWork(Incoming_Digital_Resource Resource)
        {
            // Ensure all new image files are linked to the METS file
            bool jpeg_added = false;
            bool jpeg2000_added = false;
            foreach (string thisFile in Resource.NewImageFiles)
            {
                // Leave out the legacy QC images
                if ((thisFile.ToUpper().IndexOf(".QC.JPG") < 0) && (thisFile.ToUpper().IndexOf("THM.JPG") < 0))
                {
                    // Add this file
                    FileInfo thisFileInfo = new FileInfo(thisFile);
                    Resource.Metadata.Divisions.Physical_Tree.Add_File(thisFileInfo.Name);

                    // Also, check to see if this is a jpeg or jpeg2000
                    if (thisFileInfo.Extension.ToUpper() == ".JP2")
                        jpeg2000_added = true;
                    if (thisFileInfo.Extension.ToUpper() == ".JPG")
                        jpeg_added = true;
                }
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
        }
    }
}
