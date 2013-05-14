#region Using directives

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using SobekCM.Resource_Object.Metadata_Modules;
using SobekCM.Resource_Object.Metadata_Modules.EAD;

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
                Tracer.Add_Trace("EAD_Description_ItemViewer.Add_Nav_Bar_Menu_Section", "Nothing added to placeholder");
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
                Tracer.Add_Trace("EAD_Description_ItemViewer.Add_Main_Viewer_Section", "Adds one literal with all the html");
            }

            // Get the metadata module for EADs
            EAD_Info eadInfo = (EAD_Info)CurrentItem.Get_Metadata_Module(GlobalVar.EAD_METADATA_MODULE_KEY);

            // Build the value
            StringBuilder builder = new StringBuilder(15000);
            builder.AppendLine("          <td align=\"left\"><span class=\"SobekViewerTitle\">Archival Description</span></td>");
            builder.AppendLine("        </tr>" );
            builder.AppendLine("        <tr>");
            builder.AppendLine("          <td>");
            builder.AppendLine("            <div class=\"SobekCitation\">");
            builder.AppendLine("              <br />");
            builder.AppendLine("              <blockquote>" );

            if (CurrentMode.Text_Search.Length > 0)
            {
                // Get any search terms
                List<string> terms = new List<string>();
                if (CurrentMode.Text_Search.Trim().Length > 0)
                {
                    string[] splitter = CurrentMode.Text_Search.Replace("\"", "").Split(" ".ToCharArray());
                    terms.AddRange(from thisSplit in splitter where thisSplit.Trim().Length > 0 select thisSplit.Trim());
                }

                builder.Append(Text_Search_Term_Highlighter.Hightlight_Term_In_HTML(eadInfo.Full_Description, terms));
            }
            else
            {
                builder.Append(eadInfo.Full_Description);
            }
            
            builder.AppendLine("              </blockquote>" );
            builder.AppendLine("              <br />" );
            builder.AppendLine("            </div>");

            // Add the HTML for the image
            Literal mainLiteral = new Literal {Text = builder.ToString()};
            placeHolder.Controls.Add(mainLiteral);
  
        }
    }
}
