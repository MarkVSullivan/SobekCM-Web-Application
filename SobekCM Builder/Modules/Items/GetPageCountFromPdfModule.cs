using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SobekCM.Library.Builder;

namespace SobekCM.Builder.Modules.Items
{
    public class GetPageCountFromPdfModule : abstractSubmissionPackageModule
    {
        public override void DoWork(Incoming_Digital_Resource Resource)
        {
            // If there are no pages, look for a PDF we can use to get a page count
            if (Resource.Metadata.Divisions.Physical_Tree.Pages_PreOrder.Count <= 0)
            {
                string[] pdf_files = Directory.GetFiles(Resource.Resource_Folder, "*.pdf");
                if (pdf_files.Length > 0)
                {
                    int pdf_page_count = PDF_Tools.Page_Count(pdf_files[0]);
                    if (pdf_page_count > 0)
                        Resource.Metadata.Divisions.Page_Count = pdf_page_count;
                }
            }
        }
    }
}
