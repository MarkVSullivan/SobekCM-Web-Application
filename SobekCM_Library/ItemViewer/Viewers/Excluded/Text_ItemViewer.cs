#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.ItemViewer.Viewers
{
    /// <summary> Item viewer shows plain text view of any text file associated with this digital resource, including OCR'd texte </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractItemViewer_OLD"/> and implements the 
    /// <see cref="iItemViewer" /> interface. </remarks>
    public class Text_ItemViewer : abstractItemViewer_OLD
    {
        private string text_from_file;
        private bool file_does_not_exist;
        private bool error_occurred;
        private int width;

        /// <summary> This provides an opportunity for the viewer to perform any pre-display work
        /// which is necessary before entering any of the rendering portions </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <remarks> This methods pulls the text to display and determines the width </remarks>
        public override void Perform_PreDisplay_Work(Custom_Tracer Tracer)
        {
            // Set some defaults
            text_from_file = String.Empty;
            file_does_not_exist = false;
            error_occurred = false;
            width = -1;

            if (FileName.Length > 0)
            {
                string filesource = CurrentItem.Web.Source_URL + "/" + FileName;
                text_from_file = Get_Html_Page(filesource, Tracer);

                // Did this work?
                if (text_from_file.Length > 0)
                {
                    string[] splitter = text_from_file.Split("\n".ToCharArray());
                    foreach (string thisString in splitter)
                    {
                        width = Math.Max(width, thisString.Length*9);
                    }
                   // width = Math.Min(width, 800);
                }
            }
            else
            {
                file_does_not_exist = true;
            }
        }

        /// <summary> Gets the type of item viewer this object represents </summary>
        /// <value> This property always returns the enumerational value <see cref="ItemViewer_Type_Enum.Text"/>. </value>
        public override ItemViewer_Type_Enum ItemViewer_Type
        {
            get { return ItemViewer_Type_Enum.Text; }
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
        /// <value> This returns the width of the jpeg file to display or 500, whichever is larger </value>
        public override int Viewer_Width
        {
            get
            {
                return width < 600 ? 600 : width;
            }
        }

        /// <summary> Stream to which to write the HTML for this subwriter  </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Text_ItemViewer.Write_Main_Viewer_Section", "");
            }

            // Build the value
            

            if ((error_occurred) || (file_does_not_exist) || (text_from_file.Trim().Length == 0 ))
            {
                Output.WriteLine("\t\t<td id=\"sbkTiv_ErrorArea\">");
                if (error_occurred)
                {
                    Output.WriteLine("Unknown error while retrieving text");
                }
                else if (file_does_not_exist)
                {
                    Output.WriteLine("No text file exists for this page");
                }
                else
                {
                    Output.WriteLine("No text is recorded for this page");
                }
            }
            else
            {
                Output.WriteLine("\t\t<td id=\"sbkTiv_MainArea\">");
                // If there was a term search here, highlight it
                if ( !String.IsNullOrWhiteSpace(CurrentMode.Text_Search))
                {
                    // Get any search terms
                    List<string> terms = new List<string>();
                    string[] splitter = CurrentMode.Text_Search.Replace("\"", "").Split(" ".ToCharArray());
                    terms.AddRange(from thisSplit in splitter where thisSplit.Trim().Length > 0 select thisSplit.Trim());

                    Output.WriteLine(Text_Search_Term_Highlighter.Hightlight_Term_In_HTML(text_from_file, terms, "<span class=\"sbkTiv_TextHighlight\">", "</span>"));
                }
                else
                {
                    Output.Write(text_from_file);
                }
            }
            Output.WriteLine("\t\t</td>" );
        }

        private string Get_Html_Page(string strURL, Custom_Tracer tracer )
        {
            tracer.Add_Trace("Text_ItemViewer.Get_Html_Page", "Pull full text from related text file");

            try
            {
                // the html retrieved from the page
                string strResult;
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
                if (ee.Message.IndexOf("404") >= 0)
                    file_does_not_exist = true;
                else
                    error_occurred = true;
                return String.Empty;
            }
        }
    }
}
