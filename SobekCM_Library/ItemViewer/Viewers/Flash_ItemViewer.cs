#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SobekCM.Core.BriefItem;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Tools;

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

        /// <summary> CSS ID for the viewer viewport for this particular viewer </summary>
        /// <value> This always returns the value 'sbkFliv_Viewer' </value>
        public override string Viewer_CSS
        {
            get { return "sbkFliv_Viewer"; }
        }

        /// <summary> Stream to which to write the HTML for this subwriter  </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Flash_ItemViewer.Add_Main_Viewer_Section", "");
            }

            //Determine the name of the FLASH file
            string flash_file = String.Empty;
            int current_flash_index = 0;
            foreach (BriefItem_FileGrouping thisPage in BriefItem.Downloads)
            {
                // Look for a flash file on each page
                foreach (BriefItem_File thisFile in thisPage.Files)
                {
                    if ( String.Compare(thisFile.File_Extension, ".SWF", StringComparison.OrdinalIgnoreCase) == 0 )
                    {
                        if (current_flash_index == flashIndex)
                        {
                            flash_file = thisFile.Name;
                            break;
                        }
                        current_flash_index++;
                    }
                }
            }

            if (flash_file.IndexOf("http:") < 0)
            {
                flash_file = BriefItem.Web.Source_URL + "/" + flash_file;
            }

            // Add the HTML for the image
            Output.WriteLine("        <!-- FLASH VIEWER OUTPUT -->" );
            Output.WriteLine("          <td><div id=\"sbkFiv_ViewerTitle\">" + label +"</div></td>");
            Output.WriteLine("        </tr>") ;
            Output.WriteLine("        <tr>");
            Output.WriteLine("          <td id=\"sbkFiv_MainArea\">");
            Output.WriteLine("            <object style=\"width:630px;height:630px;\">" );
            Output.WriteLine("            <param name=\"allowScriptAccess\" value=\"always\" />" );
            Output.WriteLine("            <param name=\"movie\" value=\"" + flash_file + "\">");
            Output.WriteLine("            <embed src=\"" + flash_file + "\" AllowScriptAccess=\"always\" style=\"width:630px;height:630px;\">");
            Output.WriteLine("            </embed>" );
            Output.WriteLine("            </object>");
            Output.WriteLine("          </td>" );
            Output.WriteLine("        <!-- END FLASH VIEWER OUTPUT -->" );
        }
    }
}
