#region Using directives

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web.UI.WebControls;

#endregion

namespace SobekCM.Library.ItemViewer.Viewers
{
    /// <summary> Item viewer displays a HTML file which also has a HTML Map element, for controlling the link out according to location on an image. </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractItemViewer"/> and implements the 
    /// <see cref="iItemViewer" /> interface. </remarks>
    public class HTML_Map_ItemViewer : abstractItemViewer
    {
        private readonly string htmlFile;
        private readonly string imageFile;
        private readonly string title;

        /// <summary> Constructor for a new instance of the HTML_Map_ItemViewer class </summary>
        /// <param name="Image_File"> Static image file to display, associated with the HTML map</param>
        /// <param name="HTML_File"> Static html file to display </param>
        /// <param name="Title"> Title to display for this html file (appears in viewer tab) as well </param>
        public HTML_Map_ItemViewer( string Image_File, string HTML_File, string Title )
        {
            imageFile = Image_File;
            htmlFile = HTML_File;
            title = Title;
        }

        /// <summary> Gets the type of item viewer this object represents </summary>
        /// <value> This property always returns the enumerational value <see cref="ItemViewer_Type_Enum.HTML_Map"/>. </value>
        public override ItemViewer_Type_Enum ItemViewer_Type
        {
            get { return ItemViewer_Type_Enum.HTML_Map; }
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

        /// <summary> Stream to which to write the HTML for this subwriter  </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("HTML_Map_ItemViewer.Write_Main_Viewer_Section", "");
            }

            // Save the current viewer code
            string current_view_code = CurrentMode.ViewerCode;

            // Start the citation table
            Output.WriteLine( "\t\t<!-- HTML MAP VIEWER OUTPUT -->" );
            Output.WriteLine("\t\t<td align=\"left\" height=\"40px\" ><span class=\"SobekViewerTitle\"><b>" + title + "</b></span></td></tr>");
            Output.WriteLine("\t\t<tr><td align=\"center\">");

            Output.WriteLine("<b>CLICK ON A LINK BELOW TO VIEW A MAP SHEET</b>");
            Output.WriteLine("\t\t\t<img src=\"" + CurrentItem.Web.Source_URL + "/" + imageFile + "\" usemap=\"#Map\" alt=\"Click on a sheet in the map to view a sheet\" />");

            // Try to get the HTML for this
            string map = Get_Html_Page(CurrentItem.Web.Source_URL + "/" + htmlFile, Tracer);

            // Get the link for this item
            string itemLink = CurrentMode.Base_URL + "?b=" + CurrentItem.BibID + "&v=" + CurrentItem.VID;
            Output.WriteLine(map.Replace("<%ITEM_LINK%>", itemLink));			

            // Finish the citation table
            Output.WriteLine("\t\t</td>");
            Output.WriteLine("\t\t<!-- END HTML MAP VIEWER OUTPUT -->");

            // Restore the mode
            CurrentMode.ViewerCode = current_view_code;
        }
        
        private String Get_Html_Page(string strURL, Custom_Tracer Tracer )
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("HTML_Map_ItemViewer.Get_Html_Page", "Reading html for this map from static page");
            }

            try
            {
                // the html retrieved from the page
                String strResult;
                WebRequest objRequest = WebRequest.Create(strURL);
                WebResponse objResponse = objRequest.GetResponse();

                // the using keyword will automatically dispose the object once complete
                using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
                {
                    strResult = sr.ReadToEnd();
                    // Close and clean up the StreamReader
                    sr.Close();
                }
                return strResult;
            }
            catch (Exception ee)
            {
                throw new ApplicationException("Error pulling html data '" + strURL + "'", ee);
            }
        }
    }
}
