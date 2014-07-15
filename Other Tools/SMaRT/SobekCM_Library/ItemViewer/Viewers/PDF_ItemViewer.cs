#region Using directives

using System.Text;
using System.Web.UI.WebControls;

#endregion

namespace SobekCM.Library.ItemViewer.Viewers
{
	/// <summary> Item viewer displays the a PDF related to this digital resource embedded into the SobekCM window for viewing. </summary>
	/// <remarks> This class extends the abstract class <see cref="abstractItemViewer"/> and implements the 
	/// <see cref="iItemViewer" /> interface. </remarks>
	public class PDF_ItemViewer : abstractItemViewer
	{
		/// <summary> Constructor for a new instance of the PDF_ItemViewer class </summary>
		/// <param name="FileName"> Name of the PDF file to display </param>
		public PDF_ItemViewer( string FileName )
		{
			this.FileName = FileName;
		}

		/// <summary> Gets the type of item viewer this object represents </summary>
		/// <value> This property always returns the enumerational value <see cref="ItemViewer_Type_Enum.PDF"/>. </value>
		public override ItemViewer_Type_Enum ItemViewer_Type
		{
			get { return ItemViewer_Type_Enum.PDF; }
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
		/// <value> This always returns the value -100 </value>
		public override int Viewer_Width
		{
			get
			{
				return 800;
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

		/// <summary> Adds the main view section to the page turner </summary>
		/// <param name="placeHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the the bulk of the item viewer's output is displayed</param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		public override void Add_Main_Viewer_Section(PlaceHolder placeHolder, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("PDF_ItemViewer.Add_Main_Viewer_Section", "Adds one literal with all the html");
			}

			// Build the value
			StringBuilder builder = new StringBuilder(1500);

			// Save the current viewer code
			string current_view_code = CurrentMode.ViewerCode;

			// Find the PDF download
			string displayFileName = FileName;
			if (displayFileName.IndexOf("http") < 0)
			{
				displayFileName = CurrentItem.Web.Source_URL + "/" + displayFileName;
			}
			displayFileName = displayFileName.Replace("\\", "/").Replace("//", "/").Replace("http:/", "http://");

            // MAKE THIS USE THE FILES.ASPX WEB PAGE if this is restricted (or dark)
            if ((CurrentItem.Behaviors.Dark_Flag) || (CurrentItem.Behaviors.IP_Restriction_Membership > 0))
                displayFileName = CurrentMode.Base_URL + "files/" + CurrentItem.BibID + "/" + CurrentItem.VID + "/" + FileName;


			// Start the citation table
			builder.AppendLine("\t\t<!-- PDF ITEM VIEWER OUTPUT -->" );
            builder.AppendLine("\t\t<td align=\"left\">" );
            builder.AppendLine("<table width=\"95%\"><tr>" );
            builder.AppendLine("<td align=\"left\"> &nbsp; &nbsp; <a href=\"" + displayFileName + "\">Download this PDF</a></td>");
            builder.AppendLine("<td align=\"right\"><a href=\"http://get.adobe.com/reader/\"><img src=\"" + CurrentMode.Base_URL + "default/images/get_adobe_reader.png\" /></a></td>" );
            builder.AppendLine("</tr></table>");
            builder.AppendLine("</td></tr>");
            builder.AppendLine("\t\t<tr><td>");

			if (CurrentMode.Text_Search.Length > 0)
			{
				displayFileName = displayFileName + "#search=&quot;" + CurrentMode.Text_Search.Replace("\"", "").Replace("+", " ").Replace("-", " ") + "&quot;";
			}

            builder.AppendLine("                  <embed src=\"" + displayFileName + "\" width=\"100%\" height=\"700px\" href=\"" + FileName + "\"></embed>");
            
			// Finish the table
            builder.AppendLine("\t\t</td>");
            builder.AppendLine("\t\t<!-- END PDF VIEWER OUTPUT -->" );

			// Restore the mode
			CurrentMode.ViewerCode = current_view_code;

			// Add the HTML for the image
		    Literal mainLiteral = new Literal {Text = builder.ToString()};
		    placeHolder.Controls.Add(mainLiteral);
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
