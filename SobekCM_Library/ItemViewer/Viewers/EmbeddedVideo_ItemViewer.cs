#region Using directives

using System.Text;
using System.Web.UI.WebControls;

#endregion

namespace SobekCM.Library.ItemViewer.Viewers
{
    public class EmbeddedVideo_ItemViewer : abstractItemViewer
    {
        /// <summary> Gets the type of item viewer this object represents </summary>
        /// <value> This property always returns the enumerational value <see cref="ItemViewer_Type_Enum.Embedded_Video"/>. </value>
        public override ItemViewer_Type_Enum ItemViewer_Type
        {
            get { return ItemViewer_Type_Enum.Embedded_Video; }
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
                Tracer.Add_Trace("EmbeddedVideo_ItemViewer.Add_Nav_Bar_Menu_Section", "Nothing added to placeholder");
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
                Tracer.Add_Trace("EmbeddedVideo_ItemViewer.Add_Main_Viewer_Section", "Adds one literal with all the html");
            }

            //Determine the name of the FLASH file
            string youtube_url = CurrentItem.Bib_Info.Location.Other_URL;
            if (youtube_url.IndexOf("watch") > 0)
                youtube_url = youtube_url.Replace("watch?v=", "v/") + "?fs=1&amp;hl=en_US";
            const int width = 600;
            const int height = 480;

            // Add the HTML for the image
            StringBuilder result = new StringBuilder(500);
            result.AppendLine("        <!-- EMBEDDED VIDEO VIEWER OUTPUT -->");
            result.AppendLine("          <td align=\"left\"><span class=\"SobekViewerTitle\"><b>Streaming Video</b></span></td>");
            result.AppendLine("        </tr>");
            result.AppendLine("        <tr>");
            result.AppendLine("          <td class=\"SobekCitationDisplay\">");

            result.AppendLine(CurrentItem.Behaviors.Embedded_Video);

            result.AppendLine("          </td>");
            result.AppendLine("        <!-- END EMBEDDED VIDEO VIEWER OUTPUT -->");

            Literal mainLiteral = new Literal { Text = result.ToString() };
            placeHolder.Controls.Add(mainLiteral);
        }
    }
}
