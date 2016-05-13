#region Using directives

using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using SobekCM.Core.Navigation;
using SobekCM.Core.Results;
using SobekCM.Core.Search;
using SobekCM.Library.Settings;
using SobekCM.Library.UI;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.ResultsViewer
{
    /// <summary> Results viewer shows just the thumbnails for each item in a large grid.  </summary>
    /// <remarks> This class extends the abstract class <see cref="abstract_ResultsViewer"/> and implements the 
    /// <see cref="iResultsViewer" /> interface. </remarks>
    public class Thumbnail_ResultsViewer : abstract_ResultsViewer
    {

        /// <summary> Constructor for a new instance of the Thumbnail_ResultsViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        public Thumbnail_ResultsViewer(RequestCache RequestSpecificValues ) : base(RequestSpecificValues)
        {
            // Do nothing
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
            if ((RequestSpecificValues.Paged_Results == null) || (RequestSpecificValues.Results_Statistics == null) || (RequestSpecificValues.Results_Statistics.Total_Items <= 0))
                return;

            // Get the text search redirect stem and (writer-adjusted) base url 
            string textRedirectStem = Text_Redirect_Stem;
            string base_url = RequestSpecificValues.Current_Mode.Base_URL;
            if (RequestSpecificValues.Current_Mode.Writer_Type == Writer_Type_Enum.HTML_LoggedIn)
                base_url = RequestSpecificValues.Current_Mode.Base_URL + "l/";

            // Should the publication date be shown?
            bool showDate = false;
            if (RequestSpecificValues.Current_Mode.Sort >= 10)
            {
                showDate = true;
            }

            // Start this table
            StringBuilder resultsBldr = new StringBuilder(5000);

            //Add the necessary JavaScript, CSS files
            resultsBldr.AppendLine("  <script type=\"text/javascript\" src=\"" + Static_Resources.Sobekcm_Thumb_Results_Js + "\"></script>");
            
            // Start this table
            resultsBldr.AppendLine("<table align=\"center\" width=\"100%\" cellspacing=\"15px\">");
            resultsBldr.AppendLine("\t<tr>");
            resultsBldr.AppendLine("\t\t<td width=\"25%\">&nbsp;</td>");
            resultsBldr.AppendLine("\t\t<td width=\"25%\">&nbsp;</td>");
            resultsBldr.AppendLine("\t\t<td width=\"25%\">&nbsp;</td>");
            resultsBldr.AppendLine("\t\t<td width=\"25%\">&nbsp;</td>");
            resultsBldr.AppendLine("\t</tr>");
            resultsBldr.AppendLine("\t<tr valign=\"top\">");

            // For UFDC IR Elsevier, first loop through all result firstItem bibs, setting up information 
            // for each Elsevier bibID (starts with "LS"). Note. All Elsevier bibs use only vid 00001.
            Elsevier_Entitlements_Cache e_cache = new Elsevier_Entitlements_Cache("LS", "");
            foreach (iSearch_Title_Result titleResult in RequestSpecificValues.Paged_Results)
            {
                iSearch_Item_Result firstItemResult = titleResult.Get_Item(0);
                e_cache.Add_Article(titleResult.BibID, firstItemResult.Link);
            }
            // Until CORS is tested OK, 
            // Call e_cache.update_from_entitlements_api()
            // after the foreach to more quickly query all entitlements info at once.
            // This saves several valuable seconds versus doing one entitlement api 
            // call per Elsevier article. 
            // e_cache.update_from_entitlements_api();

            Elsevier_Article elsevier_article;
            if (e_cache.d_bib_article.Count > 0)
            {   // Add html or javascript for Elsevier results
                resultsBldr.AppendLine(Elsevier_Entitlements_Cache.javascript_cors_entitlement);
            } 
            // end Elsevier setup for 'preview' results loop.

            // Step through all the results
            int col = 0;
            int title_count = 0;

            foreach (iSearch_Title_Result titleResult in RequestSpecificValues.Paged_Results)
            {
                title_count++;
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
                string title_link = "";
                string internal_citation_link = "";

                // Set Elsevier indicators to manage html construction.
                bool isElsevier = e_cache.d_bib_article.TryGetValue(titleResult.BibID, out elsevier_article);
                // Initialize Elsevier indicators for this bibid
                bool isOpenAccess = false;
                bool entitlement = true;

                if (isElsevier)
                {
                    // Workaround for builder error failures to delete obsolete Elsevier articles. Do not show results for them.
                    if (elsevier_article.is_obsolete == true)
                    {
                        continue;
                    }

                    entitlement = elsevier_article.is_entitled;
                    isOpenAccess = elsevier_article.is_open_access;
                    title_link = elsevier_article.title_url;
                    internal_citation_link = base_url + titleResult.BibID + textRedirectStem;
                }
                else
                {
                    // This not an Elsevier article. Determine the internal link to the first (possibly only) item
                    internal_citation_link = base_url + titleResult.BibID + "/" + firstItemResult.VID + textRedirectStem;

                    // For browses, just point to the title
                    if ((RequestSpecificValues.Current_Mode.Mode == Display_Mode_Enum.Aggregation)
                       && (RequestSpecificValues.Current_Mode.Aggregation_Type == Aggregation_Type_Enum.Browse_Info))
                        internal_citation_link = base_url + titleResult.BibID + textRedirectStem;

                    title_link = internal_citation_link;
                }

                // Set href to the title link 
                resultsBldr.AppendLine("\t\t<td align=\"center\" onmouseover=\"this.className='tableRowHighlight'\" "
                    + "onmouseout=\"this.className='tableRowNormal'\" onclick=\"window.location.href='"
                    + title_link + "';\" >");

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
                resultsBldr.AppendLine("<table >");

                //// Is this restricted?
                bool restricted_by_ip = false;
                if ((titleResult.Item_Count == 1) && (firstItemResult.IP_Restriction_Mask > 0))
                {
                    int comparison = firstItemResult.IP_Restriction_Mask & CurrentUserMask;
                    if (comparison == 0)
                    {
                        restricted_by_ip = true;
                    }
                }

                // Add the thumbnail
                if (isElsevier) 
                {
                    // Elsevier Published Journal Article Generic Image
                   resultsBldr.AppendLine("<tr><td>" 
                       + "<a href=\"" 
                       + title_link  + "\"\n><img " 
                       + " id=\"sbkThumbnailImg" + title_count + "\""
                       + " src=\"http://ufdcimages.uflib.ufl.edu/LS/00/00/00/00/thumb150_189.png\"\n"
                       + " alt=\"Elsevier Journal Article\" />"
                       + "</a>" 
                       + "</td></tr>");
                   if (!elsevier_article.is_open_access)
                   {
                       // Use image supplied by Elsevier for Message about user access to this article.
                       // CSS Class 'elsevier_access' and attribute pii are used by e_cache's javascript 
                       // to adjust access messages by client-requested entitlements via CORS feedback 
                       // on user article access permission.

                       resultsBldr.AppendLine("<tr><td>"
                           + " <img width='150' height='60'"
                           + " src='http://ufdcimages.uflib.ufl.edu/LS/00/00/00/00/access_check.png'"
                           + " alt=\"Elsevier Check Access\""
                           + " id='" + elsevier_article.pii + "' class='elsevier_access'"
                           + "/>"
                           + "</td></tr>");
                   } else { // Open Access - we must not use CSS class elsevier_access here.
                       resultsBldr.AppendLine("<tr><td>"
                           + " <img width='160' height='60'"
                           + " src='http://ufdcimages.uflib.ufl.edu/LS/00/00/00/00/PublisherVersion_OpenAccess.png'"
                           + " alt='Elsevier Open Access'"
                           + " id='" + elsevier_article.pii + "'"
                           + "/>"
                           + "</td></tr>" 
                           );
                   }
                } // end if isElsevier
                else if ((firstItemResult.MainThumbnail.ToUpper().IndexOf(".JPG") < 0)
                   && (firstItemResult.MainThumbnail.ToUpper().IndexOf(".GIF") < 0))
                {
                    resultsBldr.AppendLine("<tr><td><span id=\"sbkThumbnailSpan" + title_count 
                        + "\"><a href=\"" + title_link
                        + "\"><img id=\"sbkThumbnailImg" + title_count + "\" src=\"" 
                        + Static_Resources.Nothumb_Jpg
                        + "\" alt=\"MISSING THUMBNAIL\" /></a></span></td></tr>");
                }
                else
                {
                    string thumb = UI_ApplicationCache_Gateway.Settings.Image_URL 
                        + titleResult.BibID.Substring(0, 2) + "/"
                        + titleResult.BibID.Substring(2, 2) + "/" + titleResult.BibID.Substring(4, 2) + "/"
                        + titleResult.BibID.Substring(6, 2) + "/" + titleResult.BibID.Substring(8) + "/" + firstItemResult.VID + "/"
                        + (firstItemResult.MainThumbnail).Replace("\\", "/").Replace("//", "/");
                    resultsBldr.AppendLine("<tr><td><span id=\"sbkThumbnailSpan" + title_count + "\"><a href=\"" + internal_citation_link
                        + "\"><img id=\"sbkThumbnailImg" + title_count + "\"src=\"" + thumb + "\" alt=\"" + title.Replace("\"", "")
                        + "\" /></a></span></td></tr>");
                }

                #region Add the div displayed as a tooltip for this thumbnail on hover

                const string VARIES_STRING = "<span style=\"color:Gray\">( varies )</span>";
                //Add the hidden item values for display in the tooltip
                resultsBldr.AppendLine("<tr style=\"display:none;\"><td colspan=\"100%\"><div  id=\"descThumbnail" + title_count + "\" >");
                // Add each element to this table
                resultsBldr.AppendLine("\t\t\t<table cellspacing=\"0px\">");

                if (multiple_title)
                {
                    //<a href=\"" + internal_citation_link + "\">
                    resultsBldr.AppendLine("\t\t\t\t<tr style=\"height:40px;\" valign=\"middle\"><td colspan=\"3\"><span class=\"qtip_BriefTitle\" style=\"color: #a5a5a5;font-weight: bold;font-size:13px;\">" + titleResult.GroupTitle.Replace("<", "&lt;").Replace(">", "&gt;") + "</span> &nbsp; </td></tr>");
                    resultsBldr.AppendLine("<tr><td colspan=\"100%\"><br/></td></tr>");
                }
                else
                {
                    resultsBldr.AppendLine(
                        "\t\t\t\t<tr style=\"height:40px;\" valign=\"middle\"><td colspan=\"3\"><span class=\"qtip_BriefTitle\" style=\"color: #a5a5a5;font-weight: bold;font-size:13px;\">" + firstItemResult.Title.Replace("<", "&lt;").Replace(">", "&gt;") +
                        "</span> &nbsp; </td></tr><br/>");
                    resultsBldr.AppendLine("<tr><td colspan=\"100%\"><br/></td></tr>");
                }

                if ((titleResult.Primary_Identifier_Type.Length > 0) && (titleResult.Primary_Identifier.Length > 0))
                {
                    resultsBldr.AppendLine("\t\t\t\t<tr><td>" + UI_ApplicationCache_Gateway.Translation.Get_Translation(titleResult.Primary_Identifier_Type, RequestSpecificValues.Current_Mode.Language) + ":</td><td>&nbsp;</td><td>" + HttpUtility.HtmlDecode(titleResult.Primary_Identifier) + "</td></tr>");
                }

                if ((RequestSpecificValues.Current_User != null) && (RequestSpecificValues.Current_User.LoggedOn) && (RequestSpecificValues.Current_User.Is_Internal_User))
                {
                    resultsBldr.AppendLine("\t\t\t\t<tr><td>BibID:</td><td>&nbsp;</td><td>" + titleResult.BibID + "</td></tr>");

                    if (titleResult.OPAC_Number > 1)
                    {
                        resultsBldr.AppendLine("\t\t\t\t<tr><td>OPAC:</td><td>&nbsp;</td><td>" + titleResult.OPAC_Number + "</td></tr>");
                    }

                    if (titleResult.OCLC_Number > 1)
                    {
                        resultsBldr.AppendLine("\t\t\t\t<tr><td>OCLC:</td><td>&nbsp;</td><td>" + titleResult.OCLC_Number + "</td></tr>");
                    }
                }

                for (int i = 0; i < RequestSpecificValues.Results_Statistics.Metadata_Labels.Count; i++)
                {
                    string field = RequestSpecificValues.Results_Statistics.Metadata_Labels[i];

                    // Somehow the metadata for this item did not fully save in the database.  Break out, rather than
                    // throw the exception
                    if ((titleResult.Metadata_Display_Values == null) || (titleResult.Metadata_Display_Values.Length <= i))
                        break;

                    string value = titleResult.Metadata_Display_Values[i];
                    Metadata_Search_Field thisField = UI_ApplicationCache_Gateway.Settings.Metadata_Search_Field_By_Name(field);
                    string display_field = string.Empty;
                    if (thisField != null)
                        display_field = thisField.Display_Term;
                    if (display_field.Length == 0)
                        display_field = field.Replace("_", " ");

                    if (value == "*")
                    {
                        resultsBldr.AppendLine("\t\t\t\t<tr><td>" + UI_ApplicationCache_Gateway.Translation.Get_Translation(display_field, RequestSpecificValues.Current_Mode.Language) + ":</td><td>&nbsp;</td><td>" + HttpUtility.HtmlDecode(VARIES_STRING) + "</td></tr>");
                    }
                    else if (value.Trim().Length > 0)
                    {
                        if (value.IndexOf("|") > 0)
                        {
                            bool value_found = false;
                            string[] value_split = value.Split("|".ToCharArray());

                            foreach (string thisValue in value_split)
                            {
                                if (thisValue.Trim().Trim().Length > 0)
                                {
                                    if (!value_found)
                                    {
                                        resultsBldr.AppendLine("\t\t\t\t<tr valign=\"top\"><td>" + UI_ApplicationCache_Gateway.Translation.Get_Translation(display_field, RequestSpecificValues.Current_Mode.Language) + ":</td><td>&nbsp;</td><td>");
                                        value_found = true;
                                    }
                                    resultsBldr.Append(HttpUtility.HtmlDecode(thisValue) + "<br />");
                                }
                            }

                            if (value_found)
                            {
                                resultsBldr.AppendLine("</td></tr>");
                            }
                        }
                        else
                        {
                            resultsBldr.AppendLine("\t\t\t\t<tr><td>" + UI_ApplicationCache_Gateway.Translation.Get_Translation(display_field, RequestSpecificValues.Current_Mode.Language) + ":</td><td>&nbsp;</td><td>" + HttpUtility.HtmlDecode(value) + "</td></tr>");
                        }
                    }
                }

                if (titleResult.Snippet.Length > 0)
                {
                    resultsBldr.AppendLine("\t\t\t\t<tr><td colspan=\"3\"><br />&ldquo;..." + titleResult.Snippet.Replace("<em>", "<span class=\"texthighlight\">").Replace("</em>", "</span>") + "...&rdquo;</td></tr>");
                }

                resultsBldr.AppendLine("\t\t\t</table>");
                resultsBldr.AppendLine("</div></td></tr>");

                #endregion

                // Add the title
                resultsBldr.AppendLine("<tr><td align=\"center\"><span class=\"SobekThumbnailText\">" + title);
                // End the title's cell and row
                resultsBldr.AppendLine("</span></td></tr>");

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
