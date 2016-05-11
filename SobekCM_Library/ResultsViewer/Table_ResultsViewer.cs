#region Using directives

using System;
using System.Text;
using System.Web.UI.WebControls;
using SobekCM.Core.Navigation;
using SobekCM.Core.Results;
using SobekCM.Library.UI;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.ResultsViewer
{
    
    /// <summary> Results viewer shows the results in a simple table view with each title on its own table row.  </summary>
    /// <remarks> This class extends the abstract class <see cref="abstract_ResultsViewer"/> and implements the 
    /// <see cref="iResultsViewer" /> interface. </remarks>
    public class Table_ResultsViewer : abstract_ResultsViewer
    {
        /// <summary> Constructor for a new instance of the Table_ResultsViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        public Table_ResultsViewer(RequestCache RequestSpecificValues) : base(RequestSpecificValues)
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
                Tracer.Add_Trace("Table_ResultsWriter.Add_HTML", "Rendering results in table view");
            }

            // If results are null, or no results, return empty string
            if (  (RequestSpecificValues.Paged_Results == null) 
               || (RequestSpecificValues.Results_Statistics == null) 
               || (RequestSpecificValues.Results_Statistics.Total_Items <= 0))
                return;

            // Get the text search redirect stem and (writer-adjusted) base url 
            string textRedirectStem = Text_Redirect_Stem;
            string base_url = RequestSpecificValues.Current_Mode.Base_URL;
            if (RequestSpecificValues.Current_Mode.Writer_Type == Writer_Type_Enum.HTML_LoggedIn)
                base_url = RequestSpecificValues.Current_Mode.Base_URL + "l/";

            // Start the results
            StringBuilder resultsBldr = new StringBuilder(5000);
            resultsBldr.AppendLine("<br />");
            resultsBldr.AppendLine("<table width=\"100%\">");

            // Start the header row and add the column for the 'No.' part
            short? currentOrder = RequestSpecificValues.Current_Mode.Sort;
            if (RequestSpecificValues.Current_Mode.Mode == Display_Mode_Enum.Results)
            {
                RequestSpecificValues.Current_Mode.Sort = 0;
                resultsBldr.AppendLine("\t<tr valign=\"bottom\" align=\"left\">");
                resultsBldr.AppendLine("\t\t<td width=\"30px\"><span class=\"SobekTableSortText\"><a href=\"" 
                    + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode).Replace("&", "&amp;") 
                    + "\"><strong>No.</strong></a></span></td>");
            }
            else
            {
                resultsBldr.AppendLine("\t<tr valign=\"bottom\" align=\"left\">\n\t\t<td>No.</td>");
            }

            // Add the 'Title' column
            RequestSpecificValues.Current_Mode.Sort = 1;
            resultsBldr.AppendLine("\t\t<td><span class=\"SobekTableSortText\"><a href=\"" 
                + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode).Replace("&", "&amp;") + "\"><strong>" 
                + UI_ApplicationCache_Gateway.Translation.Get_Translation("Title", RequestSpecificValues.Current_Mode.Language) 
                + "</strong></a></span></td>");

            // Add the unlabeled 'external feat' column
            resultsBldr.AppendLine("\t\t<td width=\"30px\"></td>"); 

            // Add the date column
            RequestSpecificValues.Current_Mode.Sort = 10;
            resultsBldr.AppendLine("\t\t<td><span class=\"SobekTableSortText\"><a href=\"" 
                + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode).Replace("&", "&amp;") + "\"><strong>" 
                + UI_ApplicationCache_Gateway.Translation.Get_Translation("Date", RequestSpecificValues.Current_Mode.Language) 
                + "</strong></a></span></td>");
            RequestSpecificValues.Current_Mode.Sort = currentOrder;
            resultsBldr.AppendLine("\t</tr>");

            resultsBldr.AppendLine("\t<tr><td bgcolor=\"#e7e7e7\" colspan=\"4\"></td></tr>");

            // For UFDC IR Elsevier, first loop through all result firstItem bibs, setting up information 
            // for each Elsevier bibID (starts with "LS"). Note. All Elsevier bibs use only vid 00001.
            Elsevier_Entitlements_Cache e_cache = new Elsevier_Entitlements_Cache("LS", "");
            foreach (iSearch_Title_Result titleResult in RequestSpecificValues.Paged_Results)
            {
                iSearch_Item_Result firstItemResult = titleResult.Get_Item(0);
                e_cache.Add_Article(titleResult.BibID, firstItemResult.Link);
            }
            // Call this after the foreach to more quickly query all entitlements info at once.
            // This saves several valuable seconds versus doing one entitlement api 
            // call per Elsevier article. 
            // e_cache.update_from_entitlements_api();
            Elsevier_Article elsevier_article;
            // Add html and javascript for Elsevier results
            resultsBldr.AppendLine(Elsevier_Entitlements_Cache.svg_access_symbols);
            resultsBldr.AppendLine(Elsevier_Entitlements_Cache.javascript_cors_entitlement);

            // end Elsevier setup for 'preview' results loop.
            // Set the counter for these results from the page 
            int page = RequestSpecificValues.Current_Mode.Page.HasValue ? Math.Max(RequestSpecificValues.Current_Mode.Page.Value, ((ushort)1)) : 1;
            int result_counter = ((page - 1) * Results_Per_Page) + 1;
            int current_row = 0;

            // Step through all the results
            foreach (iSearch_Title_Result titleResult in RequestSpecificValues.Paged_Results)
            {
                bool multiple_title = titleResult.Item_Count > 1;

                // Always get the first item for things like the main link and thumbnail
                iSearch_Item_Result firstItemResult = titleResult.Get_Item(0);
                
                //// Is this restricted?
                //bool restricted_by_ip = false;
                //if ((titleResult.Item_Count == 1) && (firstItemResult.IP_Restriction_Mask > 0))
                //{
                //    int comparison = dbItem.IP_Range_Membership & base.current_user_mask;
                //    if (comparison == 0)
                //    {
                //        restricted_by_ip = true;
                //    }
                //}

                // Determine the internal link to the first (possibly only) item for this bibID/title
                string internal_link = base_url + titleResult.BibID + "/" + firstItemResult.VID + textRedirectStem;

                // For browses, just point to the title
                if (  ( RequestSpecificValues.Current_Mode.Mode == Display_Mode_Enum.Aggregation ) 
                   && ( RequestSpecificValues.Current_Mode.Aggregation_Type == Aggregation_Type_Enum.Browse_Info ))
                    internal_link = base_url + titleResult.BibID + textRedirectStem;

                // Start to build html for this table row
                if ( multiple_title )
                    resultsBldr.AppendLine("\t<tr valign=\"top\" onmouseover=\"this.className='tableRowHighlight'\"" 
                        + " onmouseout=\"this.className='tableRowNormal'\" >");
                else
                    resultsBldr.AppendLine("\t<tr valign=\"top\" onmouseover=\"this.className='tableRowHighlight'\""  
                        + " onmouseout=\"this.className='tableRowNormal'\" onclick=\"window.location.href='" 
                        + internal_link  + "';\" >");

                // Add the counter as the first column
                resultsBldr.AppendLine("\t\t<td>" + result_counter + "</td>");

                // For some articles an external site requires some user feat (for example, a payment or a sign-up, or to
                // agree to a license provision, etc) to see the full-text content, so in cell_external_feat we set an html
                string cell_external_feat = "";                    

                // Add differently depending on the child row count
                if ( !multiple_title )
                {              
                    // Elsevier code region to detect and treat display of Elsevier articles
                    string pii = "";

                    // Always get the first item for things like the main link and thumbnail
                    // iSearch_Item_Result firstItemResult = titleResult.Get_Item(0);
                    string title_link = "";
                    //string internal_citation_link = "";

                    // Set Elsevier indicators to manage html construction.
                    bool isElsevier = e_cache.d_bib_article.TryGetValue(titleResult.BibID, out elsevier_article);
                    // Initialize Elsevier indicators for this bibid
                    bool isOpenAccess = false;
                    bool entitlement = true;

                    // Show Title Column 
                    if (isElsevier)
                    {
                        // Workaround for builder error failures to delete obsolete Elsevier articles. Do not show results for them.
                        if (elsevier_article.is_obsolete == true)
                        {
                            continue;
                        }

                        // Elsevier articles do not have multiple titles, so this clause treats all Elsevier 
                        // article results.
                        // The title_link is the external link with sciencedirect url base 
                        // and pii from the pii value part.
                        title_link = elsevier_article.title_url;
                        entitlement = elsevier_article.is_entitled;
                        isOpenAccess = elsevier_article.is_open_access;
                        title_link = elsevier_article.title_url;
                        // internal_citation_link = base_url + titleResult.BibID + textRedirectStem;
                        internal_link = base_url + titleResult.BibID + textRedirectStem;

                        // For Elsevier Article, title_link is the external resource, but for non-Elseveir title links to internal url.
                        // So for  Elsevier, also show separate internal citation link.
                        resultsBldr.AppendLine("\t\t<td><a href=\"" + title_link + "\">" + firstItemResult.Title + "</a>"
                            + " (<a href=\"" + title_link 
                            + "\">external resource</a> | <a href=\"" + internal_link + "\">internal citation</a>)</td>");
                        //resultsBldr.AppendLine("\t\t<td><a href=\"" + title_link + "\">" + firstItemResult.Title + "</a></td>");
                    } // this is an Elsevier BibID
                    else if (firstItemResult.Link.Length > 0)
                    {
                        // This is a non-Elsevier article that has a METS related url, aka an external link aka link in the database.
                        resultsBldr.AppendLine("\t\t<td>" + firstItemResult.Title + " (<a href=\"" + firstItemResult.Link 
                            + "\">external resource</a> | <a href=\"" + internal_link + "\">internal citation</a>)</td>");
                    }
                    else
                    {   // This is a non-Elsevier BibID without a link stored in the database
                        resultsBldr.AppendLine("\t\t<td>" 
                            + "<a href=\"" + internal_link + "\">" 
                            + firstItemResult.Title + "</a></td>");
                    }

                    // Show a value in the external_feat column value, if apt, else show nothing in it.
                    if (isOpenAccess) // Show OpenAccess Icon
                    {
                        // Show OpenAccess Icon. 
                        // resultsBldr.AppendLine("<td>" + e_cache.iconOpenAccess + "</td>");
                        resultsBldr.AppendLine("<td>"
                            + "<svg height='30px' width='20px'><use xlink:href=#access-open /></svg>"
                            + "</td>");

                    }
                    else if (!entitlement)
                    {
                        // Show 'check access' message because a non-entitled user may desire other access options
                        resultsBldr.AppendLine("<td " 
                           + "id='" + elsevier_article.pii + "' class='elsevier_access'" 
                           + " type='message' >" 
                           + "Check Access</td>");
                    }
                    else
                    {
                        // Default is OK access, so display no message.
                        resultsBldr.AppendLine("<td></td>");
                    }
                    // PubDate
                    resultsBldr.AppendLine("<td>" + firstItemResult.PubDate + "</td>");
                } // Not Multiple Title
                else // Multiple title
                {
                    resultsBldr.AppendLine("\t\t<td colspan=\"3\">");

                    // Add this to the place holder
                    Literal thisLiteral = new Literal
                        { Text = resultsBldr.ToString().Replace("&lt;role&gt;", "<i>").Replace( "&lt;/role&gt;", "</i>") };
                    MainPlaceHolder.Controls.Add(thisLiteral);
                    resultsBldr.Remove(0, resultsBldr.Length);

                    Add_Issue_Tree(MainPlaceHolder, titleResult, current_row, textRedirectStem, base_url);
                    resultsBldr.AppendLine("\t</td>" );
                }
                resultsBldr.AppendLine("</tr>");
                // Add a horizontal line
                resultsBldr.AppendLine("\t<tr><td bgcolor=\"#e7e7e7\" colspan=\"4\"></td></tr>");

                // Increment the result counters
                result_counter++;
                current_row++;
            }

            // End this table
            resultsBldr.AppendLine("</table>");

            // Add this to the html table
            Literal mainLiteral = new Literal
                {  Text = resultsBldr.ToString().Replace("&lt;role&gt;", "<i>").Replace( "&lt;/role&gt;", "</i>")  };
            MainPlaceHolder.Controls.Add(mainLiteral);
        }
    }
}
