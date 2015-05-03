#region Using directives

using System;
using System.IO;
using System.Net;
using System.Text;
using SobekCM.Core.Navigation;
using SobekCM.Engine_Library.Navigation;
using SobekCM.Library.UI;
using SobekCM.Tools;

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

        /// <summary> Stream to which to write the HTML for this subwriter  </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("HTML_ItemViewer.Write_Main_Viewer_Section", "");
            }

			// Save the current viewer code
			string current_view_code = CurrentMode.ViewerCode;

			// Start the citation table
            Output.WriteLine("\t\t<!-- HTML VIEWER OUTPUT -->" );
            Output.WriteLine("\t\t<td style=\"align:left;\">");

            // Determine some replacement strings here
            string itemURL = UI_ApplicationCache_Gateway.Settings.Image_URL + CurrentItem.Web.File_Root + "/";
            string itemLink = CurrentMode.Base_URL + "/" + CurrentItem.BibID + "/" + CurrentItem.VID;

            // Determine the source string
            string sourceString = CurrentItem.Web.Source_URL + "/" + htmlFile;
            if ((htmlFile.IndexOf("http://") == 0) || (htmlFile.IndexOf("https://") == 0) || (htmlFile.IndexOf("[%BASEURL%]") == 0) || (htmlFile.IndexOf("<%BASEURL%>") == 0))
            {
                sourceString = htmlFile.Replace("[%BASEURL%]", CurrentMode.Base_URL).Replace("<%BASEURL%>", CurrentMode.Base_URL);
            }

			// Try to get the HTML for this
            string map = Get_Html_Page(sourceString, Tracer);

            // Write the HTML 
            string url_options = UrlWriterHelper.URL_Options(CurrentMode);
            string urlOptions1 = String.Empty;
            string urlOptions2 = String.Empty;
            if (url_options.Length > 0)
            {
                urlOptions1 = "?" + url_options;
                urlOptions2 = "&" + url_options;
            }
            Output.WriteLine(map.Replace("<%URLOPTS%>", url_options).Replace("<%?URLOPTS%>", urlOptions1).Replace("<%&URLOPTS%>", urlOptions2).Replace("<%ITEMURL%>", itemURL).Replace("<%ITEM_LINK%>", itemLink));

			// Finish the table
			Output.WriteLine( "\t\t</td>"  );
            Output.WriteLine("\t\t<!-- END HTML VIEWER OUTPUT -->" );

			// Restore the mode
			CurrentMode.ViewerCode = current_view_code;
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
                builder.AppendLine("<div style=\"background-color: White; color: black;text-align:center; width:630px;\">");
                builder.AppendLine("  <br /><br />");
                builder.AppendLine("  <span style=\"font-weight:bold;font-size:1.4em\">Unable to pull html view for item ( <a href=\"" + strURL + "\">source</a> )</span><br /><br />");
                builder.AppendLine("  <span style=\"font-size:1.2em\">We apologize for the inconvenience.</span><br /><br />");
                //string error_message = "Unable to pull html view for item " + CurrentItem.Web.ItemID;
    
                string returnurl = CurrentMode.Base_URL + "/contact";
                builder.AppendLine("  <span style=\"font-size:1.2em\">Click <a href=\"" + returnurl + "\">here</a> to report the problem.</span>");
                builder.AppendLine("  <br /><br />");
                builder.AppendLine("</div>");
                return builder.ToString();
            }
		}
	}
}
