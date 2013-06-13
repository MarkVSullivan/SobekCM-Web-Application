#region Using directives

using System;
using System.IO;
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
        /// <value> This is a single page viewer, so this property always returns NONE</value>
        public override ItemViewer_PageSelector_Type_Enum Page_Selector
        {
            get
            {
                return ItemViewer_PageSelector_Type_Enum.NONE;
            }
        }

        /// <summary> Stream to which to write the HTML for this subwriter  </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Download_Only_ItemViewer.Write_Main_Viewer_Section", "");
            }

			// Save the current viewer code
			string current_view_code = CurrentMode.ViewerCode;

			// Start the citation table
            Output.WriteLine("\t\t<!-- DOWNLOAD ONLY VIEWER OUTPUT -->" );
            Output.WriteLine("\t\t<td class=\"SobekDocumentDisplay\">" );
            Output.WriteLine("\t\t\t<div class=\"SobekCitation\">" );


            Output.WriteLine("\t\t\t</div>" );

			// Finish the table
			Output.WriteLine( "\t\t</td>" );
            Output.WriteLine("\t\t<!-- END DOWNLOAD ONLY VIEWER OUTPUT -->"  );

			// Restore the mode
			CurrentMode.ViewerCode = current_view_code;
		}
    }
}
