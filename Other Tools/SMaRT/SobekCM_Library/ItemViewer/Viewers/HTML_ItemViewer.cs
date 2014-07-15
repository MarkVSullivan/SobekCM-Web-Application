#region Using directives

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web.UI.WebControls;

#endregion

namespace SobekCM.Library.ItemViewer.Viewers
{
    /// <summary> Item viewer displays a HTML file related to the digital resource, within the SobekCM window. </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractItemViewer"/> and implements the 
    /// <see cref="iItemViewer" /> interface. </remarks>
	public class HTML_ItemViewer : abstractItemViewer
	{
		private readonly string htmlFile;
		private readonly string title;

        /// <summary> Constructor for a new instance of the HTML_ItemViewer class </summary>
        /// <param name="HTML_File"> Static html file to display </param>
        /// <param name="Title"> Title to display for this html file (appears in viewer tab) as well </param>
		public HTML_ItemViewer( string HTML_File, string Title )
		{
			htmlFile = HTML_File;
			title = Title;
		}

        /// <summary> Gets the type of item viewer this object represents </summary>
        /// <value> This property always returns the enumerational value <see cref="ItemViewer_Type_Enum.HTML"/>. </value>
        public override ItemViewer_Type_Enum ItemViewer_Type
        {
            get { return ItemViewer_Type_Enum.HTML; }
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

        /// <summary> Flag indicates if the header (with the title, group title, etc..) should be displayed </summary>
        /// <value> This always returns the value FALSE, to suppress the standard header information </value>
        public override bool Show_Header
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
                Tracer.Add_Trace("HTML_ItemViewer.Add_Main_Viewer_Section", "Adds one literal with all the html");
            }

			// Build the value
			StringBuilder builder = new StringBuilder(2000);

			// Save the current viewer code
			string current_view_code = CurrentMode.ViewerCode;

			// Start the citation table
            builder.AppendLine("\t\t<!-- HTML VIEWER OUTPUT -->" );
            builder.AppendLine("\t\t<td align=\"left\"><span class=\"SobekViewerTitle\"><b>" + title + "</b></span></td></tr>");
            builder.AppendLine("\t\t<tr><td class=\"SobekDocumentDisplay\">" );
            builder.AppendLine("\t\t\t<div class=\"SobekCitation\">");

            // Determine the string for the item URL
            string itemURL = SobekCM_Library_Settings.Image_URL + CurrentItem.Web.File_Root + "/";

			// Try to get the HTML for this
            string map = Get_Html_Page(CurrentItem.Web.Source_URL + "/" + htmlFile, Tracer);

            string url_options = CurrentMode.URL_Options();
            string urlOptions1 = String.Empty;
            string urlOptions2 = String.Empty;
            if (url_options.Length > 0)
            {
                urlOptions1 = "?" + url_options;
                urlOptions2 = "&" + url_options;
            }
            builder.AppendLine(map.Replace("<%URLOPTS%>", url_options).Replace("<%?URLOPTS%>", urlOptions1).Replace("<%&URLOPTS%>", urlOptions2).Replace("<%ITEMURL%>", itemURL));

            builder.AppendLine("\t\t\t</div>" );

			// Finish the table
			builder.AppendLine( "\t\t</td>"  );
            builder.AppendLine("\t\t<!-- END HTML VIEWER OUTPUT -->" );

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
                Tracer.Add_Trace("HTML_ItemViewer.Add_Nav_Bar_Menu_Section", "Nothing added to placeholder");
            }

            return false;
		}

		
		private String Get_Html_Page(string strURL, Custom_Tracer Tracer )
		{
            if (Tracer != null)
            {
                Tracer.Add_Trace("HTML_ItemViewer.Get_Html_Page", "Reading html for this view from static page");
            }

            try
            {
                // the html retrieved from the page
                String strResult;
                WebRequest objRequest = WebRequest.Create(strURL);
                WebResponse objResponse = objRequest.GetResponse();

                // the using keyword will automatically dispose the object // once complete
                using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
                {
                    strResult = sr.ReadToEnd();
                    // Close and clean up the StreamReader
                    sr.Close();
                }
                return strResult;
            }
            catch 
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("<div class=\"SobekHomeText\">");
                builder.AppendLine("<br /><br />");
                builder.AppendLine("<table width=\"630\" border=\"0\" align=\"center\">");
                builder.AppendLine("  <tr>");
                builder.AppendLine("    <td align=\"center\" >");

                builder.AppendLine("      <b><h4>Unable to pull html view for item</h4></b>");
                builder.AppendLine("      <h5>We apologize for the inconvenience.</h5>");
                string error_message = "Unable to pull html view for item " + CurrentItem.Web.ItemID;
                string returnurl = "?m=hc&em=" + error_message.Replace(" ", "%20");
                builder.AppendLine("      <h5>Click <a href=\"" + returnurl + "\">here</a> to report the problem.</h5>");
                builder.AppendLine("    </td>");
                builder.AppendLine("  </tr>");
                builder.AppendLine("</table>");
                builder.AppendLine("<br /><br />");
                builder.AppendLine("</div>");
                return builder.ToString();
            }
		}
	}
}
