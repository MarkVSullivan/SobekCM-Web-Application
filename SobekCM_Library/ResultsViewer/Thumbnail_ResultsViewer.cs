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

        protected string iconOpenAccess = @"
<svg xmlns='http://www.w3.org/2000/svg' 
x='0px' y='0px' viewbox='0 0 1000 1000'  
width='80px' height='80px'
xmlns:rdf='http://www.w3.org/1999/02/22-rdf-syntax-ns#' xmlns:cc='http://creativecommons.org/ns#' xmlns:dc='http://purl.org/dc/elements/1.1/'>
  <metadata><rdf:RDF><cc:Work rdf:about=''>
    <dc:format>image/svg+xml</dc:format>
    <dc:type rdf:resource='http://purl.org/dc/dcmitype/StillImage'/>
    <dc:creator>art designer at PLoS, modified by Wikipedia users Nina, Beao, JakobVoss, and AnonMoos</dc:creator>
    <dc:description>Open Access logo, converted into svg, designed by PLoS. This version with transparent background.</dc:description>
    <dc:source>http://commons.wikimedia.org/wiki/File:Open_Access_logo_PLoS_white.svg</dc:source>
    <dc:license rdf:resource='http://creativecommons.org/publicdomain/zero/1.0/'/>
    <cc:license rdf:resource='http://creativecommons.org/publicdomain/zero/1.0/'/>
    <cc:attributionName>art designer at PLoS, modified by Wikipedia users Nina, Beao, JakobVoss, and AnonMoos</cc:attributionName>
    <cc:attributionURL>http://www.plos.org/</cc:attributionURL>
  </cc:Work></rdf:RDF></metadata>
  <rect width='640' height='1000' fill='#ffffff'/>
  <g stroke='#f68212' stroke-width='104.764' fill='none' transform='scale(0.5)'>
    <path d='M111.387,308.135V272.408A209.21,209.214 0 0,1 529.807,272.408V530.834'/>
    <circle cx='320.004' cy='680.729' r='256.083'/>
  </g>
  <circle fill='#f68212' cx='321.01' cy='681.659' r='86.4287'/>
</svg>
";
        /// <summary> String literal for html svg for External (guest paygate) icon</summary>

        protected string iconGuestPays = @"
<svg id='icon-external' xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' 
width='60px' height='60px'
x='0px' y='0px' viewBox='0 0 130 130' xml:space='preserve'  transform='scale(0.5)'>
<polygon fill='#008000' points='120.088,16.696 60.256,16.697 60.257,0.095 120.092,0.091 '/>
<rect x='55.91' y='24.562' 
 transform='matrix(0.7071 -0.7071 0.7071 0.7071 1.0877 70.8061)' 
 fill='#008000' width='60.209' height='19.056'/>
<polygon fill='#008000' points='119.975,0.107 119.996,59.938 103.408,59.95 103.393,0.104 '/>
<rect x='3' y='23.5' fill='#008000' width='17' height='87'/>
<rect x='86.49' y='76.059' fill='#008000' width='17' height='36.941'/>
<rect x='3' y='16.692' fill='#008000' width='40.655' height='17'/>
<rect x='3' y='96' fill='#008000' width='100.49' height='17'/>
</svg>
";

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

            // For UFDC IR Elsevier, pre-step through all results, caching entitlement information for each Elsevier 
            // bibID (starts with "LS") keyed by pii value. Compose a string of all Elsevier pii values to save time 
            // by only using at most one entitlement query for this results page.
            string elsevier_pii_string = "";
            Elsevier_Entitlements_Cache e_cache = new Elsevier_Entitlements_Cache();
            foreach (iSearch_Title_Result titleResult in RequestSpecificValues.Paged_Results)
            {
                iSearch_Item_Result firstItemResult = titleResult.Get_Item(0);
                // Display special feature icons for Elsevier Articles, if any
                if (titleResult.BibID.IndexOf("LS") == 0)
                {
                    string[] parts = firstItemResult.Link.Split("/".ToCharArray());
                    string pii = "";
                    if (parts.Length > 2)
                        pii = parts[parts.Length - 1];
                    if (elsevier_pii_string.Length > 0)
                    {
                        elsevier_pii_string = elsevier_pii_string + ",";
                    }
                    elsevier_pii_string += pii;
                }
            }
            // If we found any elsevier pii values, update the entitlement cache with them.
            if (elsevier_pii_string.Length > 0)
            {
                e_cache.cache_pii_csv_string(elsevier_pii_string);
            }

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
                string internal_link = "";

                // Initialize and perhaps change Elsevier info for this bibid
                bool isElsevier = false;
                bool isOpenAccess = false;
                bool entitlement = true;
                if (titleResult.BibID.IndexOf("LS") == 0)
                {
                    // This is an Elsevier article, so we must check for openAccess and for Elsevier 
                    // entitlement to decide to show guest icon.
                    //
                    // Parse data members for this Elsevier Title. They are used to display special feature icons for 
                    // Elsevier Articles, if any, and make the Title link go to the external resource, as planned for the
                    // Elsevier Pilot.

                    // We have stored the article key PII id value in the last part of the link value,
                    // and an openaccess indicator in the penultimate part
                    // 
                    isElsevier = true;
                    string[] parts = firstItemResult.Link.Split("/".ToCharArray());
                    string pii = "";

                    if (parts.Length > 2)
                        pii = parts[parts.Length - 1].Replace("(", "").Replace(")", "").Replace("-", "");

                    //alternate settings: entitlement = e_cache.d_pii_entitlement.TryGetValue(pii, out isEntitled) ? isEntitled : false;
                    entitlement = e_cache.piis_entitled.Contains(pii) ? true : false;
                    // Extract the open access indicator
                    isOpenAccess = (parts[parts.Length - 2] == "oa_true");

                    // Create the 'real' external link from the pii value part
                    title_link = "http://www.sciencedirect.com/science/article/pii/" + pii;
                    internal_link = base_url + titleResult.BibID + textRedirectStem;
                } // if "LS" Bibid for Elsevier Article
                else 
                {
                    // This not an Elsevier article. Determine the internal link to the first (possibly only) item
                    internal_link = base_url + titleResult.BibID + "/" + firstItemResult.VID + textRedirectStem;

                    // For browses, just point to the title
                    if ((RequestSpecificValues.Current_Mode.Mode == Display_Mode_Enum.Aggregation)
                       && (RequestSpecificValues.Current_Mode.Aggregation_Type == Aggregation_Type_Enum.Browse_Info))
                        internal_link = base_url + titleResult.BibID + textRedirectStem;

                    title_link = internal_link;
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
                resultsBldr.AppendLine("<table width=\"150px\">");

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
                if ((firstItemResult.MainThumbnail.ToUpper().IndexOf(".JPG") < 0)
                   && (firstItemResult.MainThumbnail.ToUpper().IndexOf(".GIF") < 0))
                {
                    resultsBldr.AppendLine("<tr><td><span id=\"sbkThumbnailSpan" + title_count + "\"><a href=\"" + title_link
                        + "\"><img id=\"sbkThumbnailImg" + title_count + "\" src=\"" + Static_Resources.Nothumb_Jpg
                        + "\" alt=\"MISSING THUMBNAIL\" /></a></span></td></tr>");
                }
                else
                {
                    string thumb = UI_ApplicationCache_Gateway.Settings.Image_URL + titleResult.BibID.Substring(0, 2) + "/"
                        + titleResult.BibID.Substring(2, 2) + "/" + titleResult.BibID.Substring(4, 2) + "/"
                        + titleResult.BibID.Substring(6, 2) + "/" + titleResult.BibID.Substring(8) + "/" + firstItemResult.VID + "/"
                        + (firstItemResult.MainThumbnail).Replace("\\", "/").Replace("//", "/");
                    resultsBldr.AppendLine("<tr><td><span id=\"sbkThumbnailSpan" + title_count + "\"><a href=\"" + internal_link
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
                    //<a href=\"" + internal_link + "\">
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

                // End this row
                //           resultsBldr.AppendLine("\t\t<br />");

                //// Add children, if there are some
                //if (multiple_title)
                //{
                //    // Add this to the place holder
                //    Literal thisLiteral = new Literal
                //                              { Text = resultsBldr.ToString().Replace("&lt;role&gt;", "<i>").Replace( "&lt;/role&gt;", "</i>") };
                //    MainPlaceHolder.Controls.Add(thisLiteral);
                //    resultsBldr.Remove(0, resultsBldr.Length);

                //    Add_Issue_Tree(MainPlaceHolder, titleResult, current_row, textRedirectStem, base_url);
                //}

                //resultsBldr.AppendLine("\t\t</td>");
                //resultsBldr.AppendLine("\t</tr>");

                // Add a horizontal line
                //       resultsBldr.AppendLine("\t<tr><td bgcolor=\"#e7e7e7\" colspan=\"3\"></td></tr>");



                // End this table
                //           resultsBldr.AppendLine("</table>");
                resultsBldr.AppendLine("</div></td></tr>");


                #endregion
                // Add the title
                resultsBldr.AppendLine("<tr><td align=\"center\"><span class=\"SobekThumbnailText\">" + title);
                // End the title's cell and row
                resultsBldr.AppendLine("</span></td></tr>");

                // Add an Elsevier-Pilot-inspired icon if applicable
                if (isElsevier)
                {
                    if (isOpenAccess)
                    {
                        resultsBldr.AppendLine("<tr><td>" + iconOpenAccess + "</td></tr>");
                    }
                    else if (!entitlement)
                    {
                        // This is an Elsevier article to which user's IP is a 'guest' so not entitled to 
                        // full-text access, so show special icon.
                        resultsBldr.AppendLine("<tr><td>" + iconGuestPays + "</td></tr>");
                    }
                }


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
