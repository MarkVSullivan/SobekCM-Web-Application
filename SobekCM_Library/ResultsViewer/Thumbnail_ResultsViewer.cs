#region Using directives

using System.Text;
using System.Web.UI.WebControls;
using SobekCM.Library.Application_State;
using SobekCM.Library.Navigation;
using SobekCM.Library.Results;
using SobekCM.Library.Settings;

#endregion

namespace SobekCM.Library.ResultsViewer
{
    /// <summary> Results viewer shows just the thumbnails for each item in a large grid.  </summary>
    /// <remarks> This class extends the abstract class <see cref="abstract_ResultsViewer"/> and implements the 
    /// <see cref="iResultsViewer" /> interface. </remarks>
    public class Thumbnail_ResultsViewer : abstract_ResultsViewer
    {
        /// <summary> Constructor for a new instance of the Thumbnail_ResultsViewer class </summary>
        /// <param name="All_Items_Lookup"> Lookup object used to pull basic information about any item loaded into this library </param>
        public Thumbnail_ResultsViewer(Item_Lookup_Object All_Items_Lookup)
        {
            base.All_Items_Lookup = All_Items_Lookup;
        }

        /// <summary> Adds the controls for this result viewer to the place holder on the main form </summary>
        /// <param name="MainPlaceHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the the bulk of the result viewer's output is displayed</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Sorted tree with the results in hierarchical structure with volumes and issues under the titles and sorted by serial hierarchy </returns>
        public override void Add_HTML(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Thumbnail_ResultsWriter.Add_HTML", "Rendering results in thumbnail view");
            }

            // If results are null, or no results, return empty string
            if ((Paged_Results == null) || (Results_Statistics == null) || (Results_Statistics.Total_Items <= 0))
                return;

            // Get the text search redirect stem and (writer-adjusted) base url 
            string textRedirectStem = Text_Redirect_Stem;
            string base_url = CurrentMode.Base_URL;
            if (CurrentMode.Writer_Type == Writer_Type_Enum.HTML_LoggedIn)
                base_url = CurrentMode.Base_URL + "l/";

            // Should the publication date be shown?
            bool showDate = false;
            if (CurrentMode.Sort >= 10)
            {
                showDate = true;
            }

            // Start this table
            StringBuilder resultsBldr = new StringBuilder(5000);

            // Start this table
            resultsBldr.AppendLine("<table align=\"center\" width=\"100%\" cellspacing=\"15px\">");
            resultsBldr.AppendLine("\t<tr>");
            resultsBldr.AppendLine("\t\t<td width=\"25%\">&nbsp;</td>");
            resultsBldr.AppendLine("\t\t<td width=\"25%\">&nbsp;</td>");
            resultsBldr.AppendLine("\t\t<td width=\"25%\">&nbsp;</td>");
            resultsBldr.AppendLine("\t\t<td width=\"25%\">&nbsp;</td>");
            resultsBldr.AppendLine("\t</tr>");
            resultsBldr.AppendLine("\t<tr valign=\"top\">");

            // Step through all the results
            int col = 0;
            foreach (iSearch_Title_Result titleResult in Paged_Results)
            {
                // Should a new row be started
                if (col == 4)
                {
                    col = 0;
                    resultsBldr.AppendLine("\t</tr>");
                    // Horizontal Line
                    resultsBldr.AppendLine("\t<tr><td bgcolor=\"#e7e7e7\" colspan=\"4\"></td></tr>");
                    resultsBldr.AppendLine("\t<tr valign=\"top\">");
                }

                bool multiple_title = titleResult.Item_Count > 1;

                // Always get the first item for things like the main link and thumbnail
                iSearch_Item_Result firstItemResult = titleResult.Get_Item(0);

                // Determine the internal link to the first (possibly only) item
                string internal_link = base_url + titleResult.BibID + "/" + firstItemResult.VID + textRedirectStem;

                // For browses, just point to the title
                if ((CurrentMode.Mode == Display_Mode_Enum.Aggregation) && ( CurrentMode.Aggregation_Type == Aggregation_Type_Enum.Browse_Info ))
                    internal_link = base_url + titleResult.BibID + textRedirectStem;

                resultsBldr.AppendLine("\t\t<td align=\"center\" onmouseover=\"this.className='tableRowHighlight'\" onmouseout=\"this.className='tableRowNormal'\" onclick=\"window.location.href='" + internal_link + "';\" >");

                string title;
                if (multiple_title)
                {
                    // Determine term to use
                    string multi_term = "volume";
                    if (titleResult.MaterialType.ToUpper() == "NEWSPAPER")
                    {
                        multi_term = titleResult.Item_Count > 1 ? "issues" : "issue";
                    }
                    else
                    {
                        if (titleResult.Item_Count > 1)
                            multi_term = "volumes";
                    }

                    if ((showDate))
                    {
                        if (firstItemResult.PubDate.Length > 0)
                        {
                            title = "[" + firstItemResult.PubDate + "] " + titleResult.GroupTitle;
                        }
                        else
                        {
                            title = titleResult.GroupTitle;
                        }
                    }
                    else
                    {
                        title = titleResult.GroupTitle + "<br />( " + titleResult.Item_Count + " " + multi_term + " )";
                    }
                }
                else
                {
                    if (showDate)
                    {
                        if (firstItemResult.PubDate.Length > 0)
                        {
                            title = "[" + firstItemResult.PubDate + "] " + firstItemResult.Title;
                        }
                        else
                        {
                            title = firstItemResult.Title;
                        }
                    }
                    else
                    {
                        title = firstItemResult.Title;
                    }
                }

                // Start the HTML for this item
                resultsBldr.AppendLine("<table width=\"150px\">");

                //// Is this restricted?
                bool restricted_by_ip = false;
                if ((titleResult.Item_Count == 1) && (firstItemResult.IP_Restriction_Mask > 0))
                {
                    int comparison = firstItemResult.IP_Restriction_Mask & current_user_mask;
                    if (comparison == 0)
                    {
                        restricted_by_ip = true;
                    }
                }

                // Calculate the thumbnail

                // Add the thumbnail
                if ((firstItemResult.MainThumbnail.ToUpper().IndexOf(".JPG") < 0) && (firstItemResult.MainThumbnail.ToUpper().IndexOf(".GIF") < 0))
                {
                    resultsBldr.AppendLine("<tr><td><a href=\"" + internal_link + "\"><img src=\"" + CurrentMode.Default_Images_URL + "NoThumb.jpg\" /></a></td></tr>");
                }
                else
                {
                    string thumb = SobekCM_Library_Settings.Image_URL + titleResult.BibID.Substring(0, 2) + "/" + titleResult.BibID.Substring(2, 2) + "/" + titleResult.BibID.Substring(4, 2) + "/" + titleResult.BibID.Substring(6, 2) + "/" + titleResult.BibID.Substring(8) + "/" + firstItemResult.VID + "/" + (firstItemResult.MainThumbnail).Replace("\\", "/").Replace("//", "/");           
                    resultsBldr.AppendLine("<tr><td><a href=\"" + internal_link + "\"><img src=\"" +  thumb + "\" alt=\"MISSING THUMBNAIL\" /></a></td></tr>");
                }

                // Add the title
                resultsBldr.AppendLine("<tr><td align=\"center\"><span class=\"SobekThumbnailText\">" + title + "</span></td></tr>");

                // If this was access restricted, add that
                if (restricted_by_ip)
                {
                    resultsBldr.AppendLine("<tr><td align=\"center\"><span class=\"RestrictedItemText\">Access Restricted</span></td></tr>");
                }

                // Finish this one thumbnail
                resultsBldr.AppendLine("</table></td>");
                col++;
            }

            // Finish this row out
            while (col < 4)
            {
                resultsBldr.AppendLine("\t\t<td>&nbsp;</td>");
                col++;
            }

            // End this table
            resultsBldr.AppendLine("\t</tr>");
            resultsBldr.AppendLine("</table>");

            // Add this to the html table
            Literal mainLiteral = new Literal {Text = resultsBldr.ToString()};
            MainPlaceHolder.Controls.Add(mainLiteral);
        }
    }
}
