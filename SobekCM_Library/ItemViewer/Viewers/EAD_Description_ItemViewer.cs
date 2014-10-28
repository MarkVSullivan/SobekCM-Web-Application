#region Using directives

using System.Collections.Generic;
using System.IO;
using System.Linq;
using SobekCM.Resource_Object.Metadata_Modules;
using SobekCM.Resource_Object.Metadata_Modules.EAD;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.ItemViewer.Viewers
{
    /// <summary> Item viewer displays the description associated with an archival EAD/Finding guide item. </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractItemViewer"/> and implements the 
    /// <see cref="iItemViewer" /> interface. </remarks>
    public class EAD_Description_ItemViewer : abstractItemViewer
    {
        /// <summary> Gets the type of item viewer this object represents </summary>
        /// <value> This property always returns the enumerational value <see cref="ItemViewer_Type_Enum.EAD_Description"/>. </value>
        public override ItemViewer_Type_Enum ItemViewer_Type
        {
            get { return ItemViewer_Type_Enum.EAD_Description; }
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
                return 750;
            }
        }

        /// <summary> Stream to which to write the HTML for this subwriter  </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("EAD_Description_ItemViewer.Write_Main_Viewer_Section", "");
            }

            // Get the metadata module for EADs
            EAD_Info eadInfo = (EAD_Info)CurrentItem.Get_Metadata_Module(GlobalVar.EAD_METADATA_MODULE_KEY);

            // Build the value
            Output.WriteLine("          <td>");
            Output.WriteLine("            <div id=\"sbkEad_MainArea\">");

            if (CurrentMode.Text_Search.Length > 0)
            {
                // Get any search terms
                List<string> terms = new List<string>();
                if (CurrentMode.Text_Search.Trim().Length > 0)
                {
                    string[] splitter = CurrentMode.Text_Search.Replace("\"", "").Split(" ".ToCharArray());
                    terms.AddRange(from thisSplit in splitter where thisSplit.Trim().Length > 0 select thisSplit.Trim());
                }

                Output.Write(Text_Search_Term_Highlighter.Hightlight_Term_In_HTML(eadInfo.Full_Description, terms));
            }
            else
            {
                Output.Write(eadInfo.Full_Description);
            }
            
            Output.WriteLine("            </div>");
            Output.WriteLine("          </td>");
 
        }
    }
}
