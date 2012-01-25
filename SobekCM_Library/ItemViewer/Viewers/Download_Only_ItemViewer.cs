#region Using directives

using System;
using System.Text;
using System.Web.UI.WebControls;

#endregion

namespace SobekCM.Library.ItemViewer.Viewers
{

    /// <summary> [DEPRECATED?] Item viewer displays the list of links to the downloads associated with a digital resource in the case the item is ONLY available as a download </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractItemViewer"/> and implements the 
    /// <see cref="iItemViewer" /> interface. </remarks>
    public class Download_Only_ItemViewer : abstractItemViewer
    {
        private readonly string title;

        /// <summary> Constructor for a new instance of the Download_Only_ItemViewer class </summary>
        public Download_Only_ItemViewer()
        {
            title = String.Empty;
        }

        /// <summary> Gets the type of item viewer this object represents </summary>
        /// <value> This property always returns the enumerational value <see cref="ItemViewer_Type_Enum.Download_Only"/>. </value>
        public override ItemViewer_Type_Enum ItemViewer_Type
        {
            get { return ItemViewer_Type_Enum.Download_Only; }
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

        /// <summary> Width for the main viewer section to adjusted to accomodate this viewer</summary>
        /// <value> This always returns the value 600 </value>
        public override int Viewer_Width
		{
			get
			{
				return 600;
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
        /// <value> This is a single page viewer, so this property always returns FALSE</value>
        public override bool Show_Page_Selector
        {
            get
            {
                return false;
            }
        }

        /// <summary> Adds the main view section to the page turner </summary>
        /// <param name="placeHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the the bulk of the item viewer's output is displayed</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Add_Main_Viewer_Section(PlaceHolder placeHolder, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Download_Only_ItemViewer.Add_Main_Viewer_Section", "Adds one literal with all the html");
            }

			// Build the value
			StringBuilder builder = new StringBuilder(1500);

			// Save the current viewer code
			string current_view_code = CurrentMode.ViewerCode;

			// Start the citation table
            builder.AppendLine("\t\t<!-- DOWNLOAD ONLY VIEWER OUTPUT -->" );
            builder.AppendLine("\t\t<td align=\"left\"><span class=\"SobekViewerTitle\"><b>" + title + "</b></span></td></tr>");
            builder.AppendLine("\t\t<tr><td class=\"SobekDocumentDisplay\">" );
            builder.AppendLine("\t\t\t<div class=\"SobekCitation\">" );


            builder.AppendLine("\t\t\t</div>" );

			// Finish the table
			builder.AppendLine( "\t\t</td>" );
            builder.AppendLine("\t\t<!-- END DOWNLOAD ONLY VIEWER OUTPUT -->"  );

			// Restore the mode
			CurrentMode.ViewerCode = current_view_code;

			// Add the HTML for the image
            Literal mainLiteral = new Literal {Text = builder.ToString()};
            placeHolder.Controls.Add( mainLiteral );
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
                Tracer.Add_Trace("Download_Only_ItemViewer.Add_Nav_Bar_Menu_Section", "Nothing added to placeholder");
            }

            return false;
		}
    }
}
