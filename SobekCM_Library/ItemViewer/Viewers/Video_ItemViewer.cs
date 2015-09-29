using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Tools;

namespace SobekCM.Library.ItemViewer.Viewers
{
    /// <summary> Item viewer displays the video files associated with a digital resource within the SobekCM window. </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractItemViewer"/> and implements the 
    /// <see cref="iItemViewer" /> interface. </remarks>
    public class Video_ItemViewer : abstractItemViewer
    {
        /// <summary> Constructor for a new instance of the Flash_ItemViewer class </summary>
        public Video_ItemViewer()
        {
        }

        /// <summary> Gets the type of item viewer this object represents </summary>
        /// <value> This property always returns the enumerational value <see cref="ItemViewer_Type_Enum.Video"/>. </value>
        public override ItemViewer_Type_Enum ItemViewer_Type
        {
            get { return ItemViewer_Type_Enum.Video; }
        }

        /// <summary> Flag indicates if this view should be overriden if the item is checked out by another user </summary>
        /// <remarks> This always returns the value TRUE for this viewer </remarks>
        public override bool Override_On_Checked_Out
        {
            get
            {
                return true;
            }
        }

        /// <summary> Gets the number of pages for this viewer </summary>
        /// <value> This is a single page viewer, so this property always returns the value 1</value>
        public override int PageCount
        {
            get
            {
                return 1;
            }
        }

        /// <summary> Gets the flag that indicates if the page selector should be shown </summary>
        /// <value> This is a single page viewer, so this property always returns NONE</value>
        public override ItemViewer_PageSelector_Type_Enum Page_Selector
        {
            get
            {
                return ItemViewer_PageSelector_Type_Enum.NONE;
            }
        }

        /// <summary> Width for the main viewer section to adjusted to accomodate this viewer</summary>
        /// <value> This always returns the value 650 </value>
        public override int Viewer_Width
        {
            get
            {
                return 650;
            }
        }

        /// <summary> Stream to which to write the HTML for this subwriter  </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Video_ItemViewer.Add_Main_Viewer_Section", "");
            }

            ////Determine the name of the FLASH file
            //string flash_file = String.Empty;
            //int current_flash_index = 0;
            //List<abstract_TreeNode> downloadPages = CurrentItem.Divisions.Download_Tree.Pages_PreOrder;
            //foreach (Page_TreeNode thisPage in downloadPages)
            //{
            //    // Look for a flash file on each page
            //    foreach (SobekCM_File_Info thisFile in thisPage.Files.Where(thisFile => thisFile.File_Extension == "SWF"))
            //    {
            //        if (current_flash_index == flashIndex)
            //        {
            //            flash_file = thisFile.System_Name;
            //            break;
            //        }
            //        current_flash_index++;
            //    }
            //}

            //if (flash_file.IndexOf("http:") < 0)
            //{
            //    flash_file = CurrentItem.Web.Source_URL + "/" + flash_file;
            //}

            // Add the HTML for the image
            Output.WriteLine("        <!-- VIDEO VIEWER OUTPUT -->" );
            Output.WriteLine("          <td><div id=\"sbkFiv_ViewerTitle\">LABEL HERE</div></td>");
            Output.WriteLine("        </tr>") ;
            Output.WriteLine("        <tr>");
            Output.WriteLine("          <td id=\"sbkFiv_MainArea\">SHOW THE VIDEO HERE</td>" );
            Output.WriteLine("        <!-- END VIDEO VIEWER OUTPUT -->" );
        }
    }
}
