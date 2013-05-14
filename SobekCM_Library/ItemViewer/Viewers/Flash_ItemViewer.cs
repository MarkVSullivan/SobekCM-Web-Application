#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using SobekCM.Resource_Object.Divisions;

#endregion

namespace SobekCM.Library.ItemViewer.Viewers
{
    /// <summary> Item viewer displays the  a flash (SWF) file associated with a digital resource within the SobekCM window. </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractItemViewer"/> and implements the 
    /// <see cref="iItemViewer" /> interface. </remarks>
    public class Flash_ItemViewer : abstractItemViewer
    {
        private readonly int flashIndex;
        private readonly string label;

        /// <summary> Constructor for a new instance of the Flash_ItemViewer class </summary>
        /// <param name="Label"> Label to display for this flash file (appears in viewer tab) as well</param>
        /// <param name="Index"> If there are more than one flash files associated with this resourcee, this indicates the index of the file referenced </param>
        public Flash_ItemViewer( string Label, int Index )
        {
            label = Label;
            flashIndex = Index;
        }

        /// <summary> Gets the type of item viewer this object represents </summary>
        /// <value> This property always returns the enumerational value <see cref="ItemViewer_Type_Enum.Flash"/>. </value>
        public override ItemViewer_Type_Enum ItemViewer_Type
        {
            get { return ItemViewer_Type_Enum.Flash; }
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

        /// <summary> Adds any viewer_specific information to the Navigation Bar Menu Section </summary>
        /// <param name="placeHolder"> Additional place holder ( &quot;navigationPlaceHolder&quot; ) in the itemNavForm form allows item-viewer-specific controls to be added to the left navigation bar</param>
        /// <param name="Internet_Explorer"> Flag indicates if the current browser is internet explorer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Returns FALSE since nothing was added to the left navigational bar </returns>
        /// <remarks> For this item viewer, this method does nothing except return FALSE </remarks>
        public override bool Add_Nav_Bar_Menu_Section(PlaceHolder placeHolder, bool Internet_Explorer, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Flash_ItemViewer.Add_Nav_Bar_Menu_Section", "Nothing added to placeholder");
            }

            return false;
        }

        /// <summary> Adds the main view section to the page turner </summary>
        /// <param name="placeHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the the bulk of the item viewer's output is displayed</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
         public override void Add_Main_Viewer_Section(PlaceHolder placeHolder, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Flash_ItemViewer.Add_Main_Viewer_Section", "Adds one literal with all the html");
            }

            //Determine the name of the FLASH file
            string flash_file = String.Empty;
            int current_flash_index = 0;
            List<abstract_TreeNode> downloadPages = CurrentItem.Divisions.Download_Tree.Pages_PreOrder;
            foreach (Page_TreeNode thisPage in downloadPages)
            {
                // Look for a flash file on each page
                foreach (SobekCM_File_Info thisFile in thisPage.Files.Where(thisFile => thisFile.File_Extension == "SWF"))
                {
                    if (current_flash_index == flashIndex)
                    {
                        flash_file = thisFile.System_Name;
                        break;
                    }
                    current_flash_index++;
                }
            }

            if (flash_file.IndexOf("http:") < 0)
            {
                flash_file = CurrentItem.Web.Source_URL + "/" + flash_file;
            }

            // Add the HTML for the image
            Literal mainLiteral = new Literal();

            StringBuilder result = new StringBuilder(500);
            result.AppendLine("        <!-- FLASH VIEWER OUTPUT -->" );
            result.AppendLine("          <td align=\"left\"><span class=\"SobekViewerTitle\"><b>" + label +"</b></span></td>");
            result.AppendLine("        </tr>") ;
            result.AppendLine("        <tr>");
            result.AppendLine("          <td class=\"SobekCitationDisplay\">");
            result.AppendLine("            <object width=\"630\" height=\"630\">" );
            result.AppendLine("            <param name=\"allowScriptAccess\" value=\"always\" />" );
            result.AppendLine("            <param name=\"movie\" value=\"" + flash_file + "\">");
            result.AppendLine("            <embed src=\"" + flash_file + "\" AllowScriptAccess=\"always\" width=\"630\" height=\"630\">" );
            result.AppendLine("            </embed>" );
            result.AppendLine("            </object>");
            result.AppendLine("          </td>" );
            result.AppendLine("        <!-- END FLASH VIEWER OUTPUT -->" );

            mainLiteral.Text = result.ToString();
            placeHolder.Controls.Add(mainLiteral);
        }
    }
}
