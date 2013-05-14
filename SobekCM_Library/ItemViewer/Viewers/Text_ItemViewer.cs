#region Using directives

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web.UI.WebControls;

#endregion

namespace SobekCM.Library.ItemViewer.Viewers
{
    /// <summary> Item viewer shows plain text view of any text file associated with this digital resource, including OCR'd texte </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractItemViewer"/> and implements the 
    /// <see cref="iItemViewer" /> interface. </remarks>
    public class Text_ItemViewer : abstractItemViewer
    {
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

        /// <summary> Adds the main view section to the page turner </summary>
        /// <param name="placeHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the the bulk of the item viewer's output is displayed</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Add_Main_Viewer_Section(PlaceHolder placeHolder, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Text_ItemViewer.Add_Main_Viewer_Section", "Adds one literal with all the html");
            }

            // Build the value
            StringBuilder returnVal = new StringBuilder(5000);
            returnVal.AppendLine("\t\t<td align=\"left\" colspan=\"3\">"  );
            returnVal.AppendLine("\t\t\t<table class=\"SobekDocumentText\">" );
            returnVal.AppendLine("\t\t\t\t<tr>" );
            returnVal.AppendLine("\t\t\t\t\t<td width=\"15\"> </td>" );
            returnVal.AppendLine("\t\t\t\t\t<td>");
            returnVal.AppendLine("\t\t\t\t\t\t<pre>" );	
            
            if ( FileName.Length > 0 )
            {
                string filesource = CurrentItem.Web.Source_URL + "/" + FileName;
                string text = Get_Html_Page( filesource, Tracer );

                // If there was a term search here, highlight it
                if (CurrentMode.Text_Search.Length > 0)
                {
                    string upper_text = text.ToUpper();
                    string upper_search = CurrentMode.Text_Search.ToUpper();
                    StringBuilder text_builder = new StringBuilder(text);

                    int start_point = 0;
                    int adjust = 0;
                    int this_point = upper_text.IndexOf(upper_search, start_point);
                    while (this_point >= 0)
                    {
                        if ((this_point + adjust) < text_builder.Length)
                        {
                            text_builder.Insert(this_point + adjust, "<span style=\"background-color: #FFFF00\">");
                        }
                        if (this_point + 40 + upper_search.Length + adjust < text_builder.Length)
                        {
                            text_builder.Insert(this_point + 40 + upper_search.Length + adjust, "</span>");
                        }
                        else
                        {
                            text_builder.Append("</span>");
                        }

                        adjust += 47;
                        start_point = this_point + upper_search.Length;
                        if (start_point < upper_text.Length)
                        {
                            this_point = upper_text.IndexOf(upper_search, start_point);
                        }
                        else
                        {
                            this_point = -1;
                        }
                    }

                    returnVal.Append(text_builder.ToString());
                }
                else
                {
                    returnVal.Append(text);
                }
            }

            returnVal.AppendLine("\t\t\t\t\t\t</pre>" );
            returnVal.AppendLine("\t\t\t\t\t</td>");
            returnVal.AppendLine("\t\t\t\t\t<td width=\"15\"> </td>" );
            returnVal.AppendLine("\t\t\t\t</TR>" );
            returnVal.AppendLine("\t\t\t</TABLE>" );
            returnVal.AppendLine("\t\t</td>" );

            // Add the HTML for the image
            Literal mainLiteral = new Literal {Text = returnVal.ToString()};
            placeHolder.Controls.Add( mainLiteral );
        }


        /// <summary> Adds any viewer_specific information to the Navigation Bar Menu Section </summary>
        /// <param name="placeHolder"> Additional place holder ( &quot;navigationPlaceHolder&quot; ) in the itemNavForm form allows item-viewer-specific controls to be added to the left navigation bar</param>
        /// <param name="Internet_Explorer"> Flag indicates if the current browser is internet explorer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Returns FALSE since nothing was added to the left navigational bar </returns>
        /// <remarks> For this item viewer, this method does nothing except return FALSE </remarks>
        public override bool Add_Nav_Bar_Menu_Section( PlaceHolder placeHolder, bool Internet_Explorer, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Text_ItemViewer.Add_Nav_Bar_Menu_Section", "Nothing added to placeholder");
            }

            return false;
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
                throw new ApplicationException("Error pulling html data '" + strURL + "'", ee);
            }
        }
    }
}
