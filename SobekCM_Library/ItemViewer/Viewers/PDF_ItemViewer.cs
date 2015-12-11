
#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using SobekCM.Core.Navigation;
using SobekCM.Core.UI_Configuration;
using SobekCM.Library.HTML;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.ItemViewer.Viewers
{
	/// <summary> Item viewer displays the a PDF related to this digital resource embedded into the SobekCM window for viewing. </summary>
	/// <remarks> This class extends the abstract class <see cref="abstractItemViewer"/> and implements the 
	/// <see cref="iItemViewer" /> interface. </remarks>
	public class PDF_ItemViewer : abstractItemViewer
	{
	    private readonly bool writeAsIframe;

	    /// <summary> Constructor for a new instance of the PDF_ItemViewer class </summary>
	    /// <param name="FileName"> Name of the PDF file to display </param>
	    /// <param name="Current_Mode"> Current navigation information for this request </param>
	    public PDF_ItemViewer(string FileName, Navigation_Object Current_Mode)
		{
            // Determine if this should be written as an iFrame
		    writeAsIframe = ((!String.IsNullOrEmpty(Current_Mode.Browser_Type)) && (Current_Mode.Browser_Type.IndexOf("CHROME") == 0));

		    // Save the filename
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

			// Save the current viewer code
			string current_view_code = CurrentMode.ViewerCode;

			// Find the PDF download
			string displayFileName = FileName;
			if (displayFileName.IndexOf("http") < 0)
			{
				displayFileName = CurrentItem.Web.Source_URL + "/" + displayFileName;
			}
			displayFileName = displayFileName.Replace("\\", "/").Replace("//", "/").Replace("http:/", "http://");

            //replace single & double quote with ascII characters
            if (displayFileName.Contains("'") || displayFileName.Contains("\""))
            {
                displayFileName = displayFileName.Replace("'", "&#39;");
                displayFileName = displayFileName.Replace("\"", "&#34;");
            }

            // MAKE THIS USE THE FILES.ASPX WEB PAGE if this is restricted (or dark)
            if ((CurrentItem.Behaviors.Dark_Flag) || (CurrentItem.Behaviors.IP_Restriction_Membership > 0))
                displayFileName = CurrentMode.Base_URL + "files/" + CurrentItem.BibID + "/" + CurrentItem.VID + "/" + FileName;


			// Start the citation table
			Output.WriteLine("\t\t<!-- PDF ITEM VIEWER OUTPUT -->" );
            Output.WriteLine("\t\t<td id=\"sbkPdf_MainArea\">" );
            Output.WriteLine("<table style=\"width:95%;\"><tr>" );
            Output.WriteLine("<td style=\"text-align:left\"><a id=\"sbkPdf_DownloadFileLink\" href=\"" + displayFileName + "\">Download this PDF</a></td>");
            Output.WriteLine("<td style=\"text-align:right\"><a id=\"sbkPdf_DownloadAdobeReaderLink\" href=\"http://get.adobe.com/reader/\"><img src=\"" + Static_Resources.Get_Adobe_Reader_Png + "\" alt=\"Download Adobe Reader\" /></a></td>");
            Output.WriteLine("</tr></table><br />");
            //Output.WriteLine("</td></tr>");
            //Output.WriteLine("\t\t<tr><td>");
            

            // Write as an iFrame, or as embed
            if (writeAsIframe)
            {
                Output.WriteLine("                  <iframe id=\"sbkPdf_Container\" src='" + displayFileName + "' href='" + FileName + "' style=\"width:100%;\"></iframe>");
            }
            else
            {
                if (!String.IsNullOrWhiteSpace(CurrentMode.Text_Search))
                {
                    displayFileName = displayFileName + "#search=\"" + CurrentMode.Text_Search.Replace("\"", "").Replace("+", " ").Replace("-", " ") + "\"";
                }
               
                Output.WriteLine("                  <embed id=\"sbkPdf_Container\" src='" + displayFileName + "' href='" + FileName + "' style=\"width:100%;\"></embed>");
            }

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
            Body_Attributes.Add(new Tuple<string, string>("onload", "pdf_set_fullscreen(" + writeAsIframe.ToString().ToLower() + ");"));
            Body_Attributes.Add(new Tuple<string, string>("onresize", "pdf_set_fullscreen(" + writeAsIframe.ToString().ToLower() + ");"));
        }

        /// <summary> Gets the collection of special behaviors which this item viewer
        /// requests from the main HTML subwriter. </summary>
        public override List<HtmlSubwriter_Behaviors_Enum> ItemViewer_Behaviors
        {
            get
            {
                return new List<HtmlSubwriter_Behaviors_Enum> { HtmlSubwriter_Behaviors_Enum.Suppress_Footer };
            }
        }
	}
}
