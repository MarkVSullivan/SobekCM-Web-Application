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
    /// <summary> Results viewer shows the thumbnail and a small citation block for each item in the result set.  </summary>
    /// <remarks> This class extends the abstract class <see cref="abstract_ResultsViewer"/> and implements the 
    /// <see cref="iResultsViewer" /> interface. </remarks>
    public class Brief_ResultsViewer : abstract_ResultsViewer
    {
        /// <summary> Constructor for a new instance of the Brief_ResultsViewer class </summary>
        /// <param name="All_Items_Lookup"> Lookup object used to pull basic information about any item loaded into this library </param>
        public Brief_ResultsViewer(Item_Lookup_Object All_Items_Lookup)
        {
            base.All_Items_Lookup = All_Items_Lookup;
        }

        /// <summary> Adds the controls for this result viewer to the place holder on the main form </summary>
        /// <param name="placeHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the the bulk of the result viewer's output is displayed</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Sorted tree with the results in hierarchical structure with volumes and issues under the titles and sorted by serial hierarchy </returns>
        public override void Add_HTML(PlaceHolder placeHolder, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Brief_ResultsWriter.Add_HTML", "Rendering results in brief view");
            }

            // If results are null, or no results, return empty string
            if ((Paged_Results == null) || (Results_Statistics == null) || (Results_Statistics.Total_Items <= 0))
                return;

            const string VARIES_STRING = "<span style=\"color:Gray\">( varies )</span>";

            // Get the text search redirect stem and (writer-adjusted) base url 
            string textRedirectStem = Text_Redirect_Stem;
            string base_url = CurrentMode.Base_URL;
            if (CurrentMode.Writer_Type == Writer_Type_Enum.HTML_LoggedIn)
                base_url = CurrentMode.Base_URL + "l/";

            // Start the results
            StringBuilder resultsBldr = new StringBuilder(2000);
            resultsBldr.AppendLine("<br />");
            resultsBldr.AppendLine("<table>");

            // Set the counter for these results from the page 
            int result_counter = ((CurrentMode.Page - 1)*Results_Per_Page) + 1;

            // Step through all the results
            int current_row = 0;
            foreach (iSearch_Title_Result titleResult in Paged_Results)
            {
                bool multiple_title = false;
                if (titleResult.Item_Count > 1)
                    multiple_title = true;

                // Always get the first item for things like the main link and thumbnail
                iSearch_Item_Result firstItemResult = titleResult.Get_Item(0);

                // Determine the internal link to the first (possibly only) item
                string internal_link = base_url + titleResult.BibID + "/" + firstItemResult.VID + textRedirectStem;

                // For browses, just point to the title
                if (CurrentMode.Mode == Display_Mode_Enum.Aggregation_Browse_Info)
                    internal_link = base_url + titleResult.BibID + textRedirectStem;

                // Start this row
                if (multiple_title)
                    resultsBldr.AppendLine( "\t<tr valign=\"top\" onmouseover=\"this.className='tableRowHighlight'\" onmouseout=\"this.className='tableRowNormal'\" >");
                else
                    resultsBldr.AppendLine( "\t<tr valign=\"top\" onmouseover=\"this.className='tableRowHighlight'\" onmouseout=\"this.className='tableRowNormal'\" onclick=\"window.location.href='" + internal_link + "';\" >");

                // Add the counter as the first column
                resultsBldr.AppendLine("\t\t<td><br /><b>" + result_counter + "</b></td>\t\t<td valign=\"top\" width=\"150\">");

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
                string thumb = titleResult.BibID.Substring(0, 2) + "/" + titleResult.BibID.Substring(2, 2) + "/" +titleResult.BibID.Substring(4, 2) + "/" + titleResult.BibID.Substring(6, 2) + "/" + titleResult.BibID.Substring(8) + "/" + firstItemResult.VID + "/" + (firstItemResult.MainThumbnail).Replace("\\", "/").Replace("//", "/");

                // Draw the thumbnail 
                if ((thumb.ToUpper().IndexOf(".JPG") < 0) && (thumb.ToUpper().IndexOf(".GIF") < 0))
                {
                    resultsBldr.AppendLine("<a href=\"" + internal_link + "\"><img src=\"" + CurrentMode.Default_Images_URL + "NoThumb.jpg\" border=\"0px\" class=\"resultsThumbnail\" alt=\"MISSING THUMBNAIL\" /></a></td>");
                }
                else
                {
                    resultsBldr.AppendLine("<a href=\"" + internal_link + "\"><img src=\"" +SobekCM_Library_Settings.Image_URL + thumb + "\" class=\"resultsThumbnail\" alt=\"MISSING THUMBNAIL\" /></a></td>");
                }
                resultsBldr.AppendLine("\t\t<td>");

                // If this was access restricted, add that
                if (restricted_by_ip)
                {
                    resultsBldr.AppendLine("<span class=\"RestrictedItemText\">" + Translator.Get_Translation("Access Restricted", CurrentMode.Language) + "</span>");
                }

                // Add each element to this table
                resultsBldr.AppendLine("\t\t\t<table cellspacing=\"0px\">");

                if (multiple_title)
                {
                    resultsBldr.AppendLine( "\t\t\t\t<tr style=\"height:40px;\" valign=\"middle\"><td colspan=\"3\"><span class=\"briefResultsTitle\"><a href=\"" + internal_link + "\">" + titleResult.GroupTitle.Replace("<", "&lt;").Replace(">", "&gt;") + "</a></span> &nbsp; </td></tr>");
                }
                else
                {
                    resultsBldr.AppendLine(
                        "\t\t\t\t<tr style=\"height:40px;\" valign=\"middle\"><td colspan=\"3\"><span class=\"briefResultsTitle\"><a href=\"" +
                        internal_link + "\">" + firstItemResult.Title.Replace("<", "&lt;").Replace(">", "&gt;") +
                        "</a></span> &nbsp; </td></tr>");
                }

                if ((titleResult.Primary_Identifier_Type.Length > 0) && (titleResult.Primary_Identifier.Length > 0))
                {
                    resultsBldr.AppendLine("\t\t\t\t<tr><td>" + Translator.Get_Translation(titleResult.Primary_Identifier_Type, CurrentMode.Language) + ":</td><td>&nbsp;</td><td>" + titleResult.Primary_Identifier + "</td></tr>");
                }

                if (CurrentMode.Internal_User)
                {
                    resultsBldr.AppendLine("\t\t\t\t<tr><td>BibID:</td><td>&nbsp;</td><td>" + titleResult.BibID + "</td></tr>");

                    if (titleResult.ALEPH_Number > 1)
                    {
                        resultsBldr.AppendLine("\t\t\t\t<tr><td>ALEPH:</td><td>&nbsp;</td><td>" +titleResult.ALEPH_Number + "</td></tr>");
                    }

                    if (titleResult.OCLC_Number > 1)
                    {
                        resultsBldr.AppendLine("\t\t\t\t<tr><td>OCLC:</td><td>&nbsp;</td><td>" + titleResult.OCLC_Number + "</td></tr>");
                    }
                }

                if ((!multiple_title) && (firstItemResult.PubDate.Length > 0) && (firstItemResult.PubDate != "-1"))
                {
                    resultsBldr.AppendLine("\t\t\t\t<tr><td>" + Translator.Get_Translation("Date", CurrentMode.Language) + ":</td><td>&nbsp;</td><td>" + firstItemResult.PubDate + "</td></tr>");
                }

                if (titleResult.Author.Length > 0)
                {
                    string creatorString = "Author";
                    if (titleResult.MaterialType.ToUpper().IndexOf("ARTIFACT") == 0)
                    {
                        creatorString = "Creator";
                    }

                    if (titleResult.Author == "*")
                    {
                        resultsBldr.AppendLine("\t\t\t\t<tr><td>" + Translator.Get_Translation(creatorString, CurrentMode.Language) + ":</td><td>&nbsp;</td><td>" + VARIES_STRING + "</td></tr>");
                    }
                    else
                    {
                        bool author_found = false;
                        string[] author_split = titleResult.Author.Split("|".ToCharArray());

                        foreach (string thisAuthor in author_split)
                        {
                            if (thisAuthor.ToUpper().IndexOf("PUBLISHER") < 0)
                            {
                                if (!author_found)
                                {
                                    resultsBldr.AppendLine("\t\t\t\t<tr valign=\"top\"><td>" +Translator.Get_Translation(creatorString, CurrentMode.Language) + ":</td><td>&nbsp;</td><td>");
                                    author_found = true;
                                }
                                resultsBldr.Append(thisAuthor + "<br />");
                            }
                        }

                        if (author_found)
                        {
                            resultsBldr.AppendLine("</td></tr>");
                        }
                    }
                }
                if (titleResult.Publisher.Length > 0)
                {
                    if (titleResult.Publisher == "*")
                    {
                        resultsBldr.AppendLine("\t\t\t\t<tr><td>" + Translator.Get_Translation("Publisher", CurrentMode.Language) + ":</td><td>&nbsp;</td><td>" + VARIES_STRING + "</td></tr>");
                    }
                    else
                    {
                        string[] publisher_split = titleResult.Publisher.Split("|".ToCharArray());
                        resultsBldr.AppendLine("\t\t\t\t<tr valign=\"top\"><td>" + Translator.Get_Translation("Publisher", CurrentMode.Language) + ":</td><td>&nbsp;</td><td>");
                        foreach (string thisPublisher in publisher_split)
                        {
                            resultsBldr.Append(thisPublisher + "<br />");
                        }
                        resultsBldr.AppendLine("</td></tr>");
                    }
                }
                if (titleResult.Format.Length > 0)
                {
                    if (titleResult.Format == "*")
                    {
                        resultsBldr.AppendLine("\t\t\t\t<tr><td>" + Translator.Get_Translation("Format", CurrentMode.Language) + ":</td><td>&nbsp;</td><td>" + VARIES_STRING + "</td></tr>");
                    }
                    else
                    {
                        resultsBldr.AppendLine("\t\t\t\t<tr><td>" +
                                               Translator.Get_Translation("Format", CurrentMode.Language) +
                                               ":</td><td>&nbsp;</td><td>" +
                                               titleResult.Format.Replace("[", " ").Replace("]", " ") + "</td></tr>");
                    }
                }
                if (titleResult.Edition.Length > 0)
                {
                    if (titleResult.Edition == "*")
                    {
                        resultsBldr.AppendLine("\t\t\t\t<tr><td>" + Translator.Get_Translation("Edition", CurrentMode.Language) + ":</td><td>&nbsp;</td><td>" + VARIES_STRING + "</td></tr>");
                    }
                    else
                    {
                        resultsBldr.AppendLine("\t\t\t\t<tr height=\"10px\"><td>" +Translator.Get_Translation("Edition", CurrentMode.Language) + ":</td><td>&nbsp;</td><td>" + titleResult.Edition + "</td></tr>");
                    }
                }
                if (titleResult.Material.Length > 0)
                {
                    if (titleResult.Material == "*")
                    {
                        resultsBldr.AppendLine("\t\t\t\t<tr><td>" +Translator.Get_Translation("Material", CurrentMode.Language) + ":</td><td>&nbsp;</td><td>" + VARIES_STRING + "</td></tr>");
                    }
                    else
                    {
                        resultsBldr.AppendLine("\t\t\t\t<tr><td>" +Translator.Get_Translation("Material", CurrentMode.Language) + ":</td><td>&nbsp;</td><td>" + titleResult.Material + "</td></tr>");
                    }
                }

                if (titleResult.Measurement.Length > 0)
                {
                    if (titleResult.Measurement == "*")
                    {
                        resultsBldr.AppendLine("\t\t\t\t<tr><td>" + Translator.Get_Translation("Measurement", CurrentMode.Language) + ":</td><td>&nbsp;</td><td>" + VARIES_STRING + "</td></tr>");
                    }
                    else
                    {
                        string[] measurement_split = titleResult.Measurement.Split("|".ToCharArray());
                        resultsBldr.AppendLine("\t\t\t\t<tr valign=\"top\"><td>" + Translator.Get_Translation("Measurement", CurrentMode.Language) + ":</td><td>&nbsp;</td><td>");
                        resultsBldr.AppendLine(measurement_split[0] + "<br />");
                        //foreach (string thisMeasurement in measurement_split)
                        //{
                        //    resultsBldr.Append(thisMeasurement + "<br />");
                        //}
                        resultsBldr.AppendLine("</td></tr>");
                    }
                }
                if (titleResult.Style_Period.Length > 0)
                {
                    if (titleResult.Style_Period == "*")
                    {
                        resultsBldr.AppendLine("\t\t\t\t<tr><td>" +Translator.Get_Translation("Style/Period", CurrentMode.Language) + ":</td><td>&nbsp;</td><td>" + VARIES_STRING + "</td></tr>");
                    }
                    else
                    {
                        resultsBldr.AppendLine("\t\t\t\t<tr><td>" + Translator.Get_Translation("Style/Period", CurrentMode.Language) + ":</td><td>&nbsp;</td><td>" + titleResult.Style_Period + "</td></tr>");
                    }
                }
                if (titleResult.Technique.Length > 0)
                {
                    if (titleResult.Technique == "*")
                    {
                        resultsBldr.AppendLine("\t\t\t\t<tr><td>" +Translator.Get_Translation("Technique", CurrentMode.Language) + ":</td><td>&nbsp;</td><td>" + VARIES_STRING + "</td></tr>");
                    }
                    else
                    {
                        resultsBldr.AppendLine("\t\t\t\t<tr><td>" + Translator.Get_Translation("Technique", CurrentMode.Language) + ":</td><td>&nbsp;</td><td>" + titleResult.Technique + "</td></tr>");
                    }
                }
                if (titleResult.Subjects.Length > 0)
                {
                    if (titleResult.Subjects == "*")
                    {
                        resultsBldr.AppendLine("\t\t\t\t<tr><td>" + Translator.Get_Translation("Subject", CurrentMode.Language) + ":</td><td>&nbsp;</td><td>" + VARIES_STRING + "</td></tr>");
                    }
                    else
                    {
                        string[] subject_split = titleResult.Subjects.Split("|".ToCharArray());
                        resultsBldr.AppendLine("\t\t\t\t<tr valign=\"top\"><td>" +Translator.Get_Translation("Subject", CurrentMode.Language) + ":</td><td>&nbsp;</td><td>");
                        foreach (string thisSubject in subject_split)
                        {
                            resultsBldr.Append(thisSubject + "<br />");
                        }
                        resultsBldr.AppendLine("</td></tr>");
                    }
                }
                if (titleResult.Institution.Length > 0)
                {
                    if (titleResult.Institution == "*")
                    {
                        resultsBldr.AppendLine("\t\t\t\t<tr><td>" + Translator.Get_Translation("Institution", CurrentMode.Language) + ":</td><td>&nbsp;</td><td>" + VARIES_STRING + "</td></tr>");
                    }
                    else
                    {
                        resultsBldr.AppendLine("\t\t\t\t<tr><td>" + Translator.Get_Translation("Institution", CurrentMode.Language) + ":</td><td>&nbsp;</td><td>" + titleResult.Institution + "</td></tr>");
                    }
                }
                if (titleResult.Donor.Length > 0)
                {
                    if (titleResult.Donor == "*")
                    {
                        resultsBldr.AppendLine("\t\t\t\t<tr><td>" + Translator.Get_Translation("Donor", CurrentMode.Language) + ":</td><td>&nbsp;</td><td>" + VARIES_STRING + "</td></tr>"); 
                    }
                    else
                    {
                        resultsBldr.AppendLine("\t\t\t\t<tr><td>" +Translator.Get_Translation("Donor", CurrentMode.Language) + ":</td><td>&nbsp;</td><td>" + titleResult.Donor + "</td></tr>");
                    }
                }

                if (titleResult.Snippet.Length > 0)
                {
                    resultsBldr.AppendLine("\t\t\t\t<tr><td colspan=\"3\"><br />&ldquo;..." + titleResult.Snippet.Replace("<em>", "<span class=\"texthighlight\">").Replace ("</em>", "</span>") + "...&rdquo;</td></tr>");
                }

                resultsBldr.AppendLine("\t\t\t</table>");

                // End this row
                resultsBldr.AppendLine("\t\t<br />");

                // Add children, if there are some
                if (multiple_title)
                {
                    // Add this to the place holder
                    Literal thisLiteral = new Literal
                                              { Text = resultsBldr.ToString().Replace("&lt;role&gt;", "<i>").Replace( "&lt;/role&gt;", "</i>") };
                    placeHolder.Controls.Add(thisLiteral);
                    resultsBldr.Remove(0, resultsBldr.Length);

                    Add_Issue_Tree(placeHolder, titleResult, current_row, textRedirectStem, base_url);
                }

                resultsBldr.AppendLine("\t\t</td>");
                resultsBldr.AppendLine("\t</tr>");

                // Add a horizontal line
                resultsBldr.AppendLine("\t<tr><td bgcolor=\"#e7e7e7\" colspan=\"3\"></td></tr>");

                // Increment the result counters
                result_counter++;
                current_row++;
            }

            // End this table
            resultsBldr.AppendLine("</table>");

            // Add this to the HTML page
            Literal mainLiteral = new Literal
                                      { Text = resultsBldr.ToString().Replace("&lt;role&gt;", "<i>").Replace( "&lt;/role&gt;", "</i>") };
            placeHolder.Controls.Add(mainLiteral);
        }
    }
}
