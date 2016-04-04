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
        /// <summary> String literal for html svg for OpenAccess icon</summary>
        /// The size is different among the current ResultsViewers, so each has its own icon versions, so they can be 
        /// tweaked individually in the code for each concrete ResultsViewer as development iterates.
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
        /// TODO: might try using a clipPath later to crop off a too-wide bottom margin on this image... 
        protected string iconGuestPays = @"
<svg id='icon-external' xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' 
width='60px' height='60px'
x='0px' y='0px' viewBox='0 0 130 130' xml:space='preserve' transform='scale(0.5)'>
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
            e_cache.update_from_entitlements_api();
            Elsevier_Article elsevier_article;

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
                        resultsBldr.AppendLine("<td>" + iconOpenAccess + "</td>");
                    }
                    else if (!entitlement)
                    {
                        // Show external icon
                        resultsBldr.AppendLine("<td>" + iconGuestPays + "</td>");
                    }
                    else
                    {
                        //No icon 
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
