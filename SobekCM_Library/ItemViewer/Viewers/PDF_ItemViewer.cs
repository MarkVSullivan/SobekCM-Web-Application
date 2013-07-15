#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.UI.WebControls;
using SobekCM.Library.HTML;

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
				return -1;
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
				Tracer.Add_Trace("PDF_ItemViewer.Write_Main_Viewer_Section", "");
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
			Output.WriteLine("\t\t<!-- PDF ITEM VIEWER OUTPUT -->" );
            Output.WriteLine("\t\t<td align=\"left\">" );
            Output.WriteLine("<table width=\"95%\"><tr>" );
            Output.WriteLine("<td align=\"left\"> &nbsp; &nbsp; <a href=\"" + displayFileName + "\">Download this PDF</a></td>");
            Output.WriteLine("<td align=\"right\"><a href=\"http://get.adobe.com/reader/\"><img src=\"" + CurrentMode.Base_URL + "default/images/get_adobe_reader.png\" /></a></td>" );
            Output.WriteLine("</tr></table>");
            Output.WriteLine("</td></tr>");
            Output.WriteLine("\t\t<tr><td align=\"left\">");

			if (CurrentMode.Text_Search.Length > 0)
			{
				displayFileName = displayFileName + "#search=&quot;" + CurrentMode.Text_Search.Replace("\"", "").Replace("+", " ").Replace("-", " ") + "&quot;";
			}

            Output.WriteLine("                  <embed id=\"pdfdocument\" src=\"" + displayFileName + "\" width=\"600px\" height=\"700px\" href=\"" + FileName + "\"></embed>");
            
			// Finish the table
            Output.WriteLine("\t\t</td>");
            Output.WriteLine("\t\t<!-- END PDF VIEWER OUTPUT -->" );

			// Restore the mode
			CurrentMode.ViewerCode = current_view_code;
		}

        /// <summary> Gets the collection of body attributes to be included 
        /// within the HTML body tag (usually to add events to the body) </summary>
        /// <param name="Body_Attributes"> List of body attributes to be included </param>
        public override void Add_ViewerSpecific_Body_Attributes(List<Tuple<string, string>> Body_Attributes)
        {
            Body_Attributes.Add(new Tuple<string, string>("onload", "pdf_set_fullscreen();"));
            Body_Attributes.Add(new Tuple<string, string>("onresize", "pdf_set_fullscreen();"));
        }

        /// <summary> Gets the collection of special behaviors which this item viewer
        /// requests from the main HTML subwriter. </summary>
        public override List<HTML.HtmlSubwriter_Behaviors_Enum> ItemViewer_Behaviors
        {
            get
            {
                return new List<HtmlSubwriter_Behaviors_Enum> { HtmlSubwriter_Behaviors_Enum.Suppress_Footer };
            }
        }
	}
}
