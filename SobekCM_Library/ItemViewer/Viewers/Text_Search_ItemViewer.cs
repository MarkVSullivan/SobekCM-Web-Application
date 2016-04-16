using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using SobekCM.Core.BriefItem;
using SobekCM.Core.Configuration.Localization;
using SobekCM.Core.Navigation;
using SobekCM.Core.UI_Configuration;
using SobekCM.Core.UI_Configuration.StaticResources;
using SobekCM.Core.Users;
using SobekCM.Engine_Library.Configuration;
using SobekCM.Engine_Library.Solr;
using SobekCM.Library.ItemViewer.Menu;
using SobekCM.Tools;

namespace SobekCM.Library.ItemViewer.Viewers
{
    /// <summary> Text search item viewer prototyper, which is used to check to see if an item can be full text searched
    /// and to create the viewer itself if the user selects that option </summary>
    public class Text_Search_ItemViewer_Prototyper : iItemViewerPrototyper
    {
        /// <summary> Constructor for a new instance of the Text_Search_ItemViewer_Prototyper class </summary>
        public Text_Search_ItemViewer_Prototyper()
        {
            ViewerType = "SEARCH";
            ViewerCode = "search";
        }

        /// <summary> Name of this viewer, which matches the viewer name from the database and 
        /// in the configuration files as well.  This is actually populate by the configuration information </summary>
        public string ViewerType { get; set; }

        /// <summary> Code for this viewer, which can also be set from the configuration information </summary>
        public string ViewerCode { get; set; }

        /// <summary> If this viewer is tied to certain files existing in the digital resource, this lists all the 
        /// possible file extensions this supports (from the configuration file usually) </summary>
        public string[] FileExtensions { get; set; }

        /// <summary> Indicates if the specified item matches the basic requirements for this viewer, or
        /// if this viewer should be ignored for this item </summary>
        /// <param name="CurrentItem"> Digital resource to examine to see if this viewer really should be included </param>
        /// <returns> TRUE if this viewer should generally be included with this item, otherwise FALSE </returns>
        public bool Include_Viewer(BriefItemInfo CurrentItem)
        {
            // This should always be included (although it won't be accessible or shown to everyone)
            return CurrentItem.Behaviors.Full_Text_Searchable;
        }

        /// <summary> Flag indicates if this viewer should be override on checkout </summary>
        /// <param name="CurrentItem"> Digital resource to examine to see if this viewer should really be overriden </param>
        /// <returns> TRUE always, since PDFs should never be shown if an item is checked out </returns>
        public bool Override_On_Checkout(BriefItemInfo CurrentItem)
        {
            return false;
        }

        /// <summary> Flag indicates if the current user has access to this viewer for the item </summary>
        /// <param name="CurrentItem"> Digital resource to see if the current user has correct permissions to use this viewer </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="IpRestricted"> Flag indicates if this item is IP restricted AND if the current user is outside the ranges </param>
        /// <returns> TRUE if the user has access to use this viewer, otherwise FALSE </returns>
        public bool Has_Access(BriefItemInfo CurrentItem, User_Object CurrentUser, bool IpRestricted)
        {
            return !IpRestricted;
        }

        /// <summary> Gets the menu items related to this viewer that should be included on the main item (digital resource) menu </summary>
        /// <param name="CurrentItem"> Digital resource object, which can be used to ensure if and how this viewer should appear 
        /// in the main item (digital resource) menu </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        /// <param name="MenuItems"> List of menu items, to which this method may add one or more menu items </param>
        public void Add_Menu_items(BriefItemInfo CurrentItem, User_Object CurrentUser, Navigation_Object CurrentRequest, List<Item_MenuItem> MenuItems)
        {
            // Get the URL for this
            string previous_code = CurrentRequest.ViewerCode;
            CurrentRequest.ViewerCode = ViewerCode;
            string url = UrlWriterHelper.Redirect_URL(CurrentRequest);
            CurrentRequest.ViewerCode = previous_code;

            // Add the item menu information
            Item_MenuItem menuItem = new Item_MenuItem("Search", null, null, url, ViewerCode);
            MenuItems.Add(menuItem);
        }

        /// <summary> Creates and returns the an instance of the <see cref="Text_Search_ItemViewer"/> class for performing full text
        /// search against a single digital resource during execution of a single HTTP request. </summary>
        /// <param name="CurrentItem"> Digital resource object </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Fully built and initialized <see cref="Text_Search_ItemViewer"/> object </returns>
        /// <remarks> This method is called whenever a request requires the actual viewer to be created to render the HTML for
        /// the digital resource requested.  The created viewer is then destroyed at the end of the request </remarks>
        public iItemViewer Create_Viewer(BriefItemInfo CurrentItem, User_Object CurrentUser, Navigation_Object CurrentRequest, Custom_Tracer Tracer)
        {
            return new Text_Search_ItemViewer(CurrentItem, CurrentUser, CurrentRequest, Tracer );
        }
    }

    /// <summary> Text search item viewer allows the full text of an individual resource to be searched and individual matching pages
    /// are displayed with page thumbnails </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractNoPaginationItemViewer"/> and implements the 
    /// <see cref="iItemViewer" /> interface. </remarks>
    public class Text_Search_ItemViewer : abstractNoPaginationItemViewer
    {
        private Solr_Page_Results results;

        /// <summary> Constructor for a new instance of the Text_Search_ItemViewer class, which allows the full text of an 
        /// individual resource to be searched and individual matching pages are displayed with page thumbnails </summary>
        /// <param name="BriefItem"> Digital resource object </param>
        /// <param name="CurrentUser"> Current user, who may or may not be logged on </param>
        /// <param name="CurrentRequest"> Information about the current request </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public Text_Search_ItemViewer(BriefItemInfo BriefItem, User_Object CurrentUser, Navigation_Object CurrentRequest, Custom_Tracer Tracer )
        {
            Tracer.Add_Trace("Text_Search_ItemViewer.Constructor");

            // Save the arguments for use later
            this.BriefItem = BriefItem;
            this.CurrentUser = CurrentUser;
            this.CurrentRequest = CurrentRequest;

            // Set the behavior properties to the empy behaviors ( in the base class )
            Behaviors = EmptyBehaviors;

            if (!String.IsNullOrWhiteSpace(CurrentRequest.Text_Search))
            {
                List<string> terms = new List<string>();
                List<string> web_fields = new List<string>();

                // Split the terms correctly
                SobekCM_Assistant.Split_Clean_Search_Terms_Fields(CurrentRequest.Text_Search, "ZZ", Search_Type_Enum.Basic, terms, web_fields, null, Search_Precision_Type_Enum.Contains, '|');

                Tracer.Add_Trace("Text_Search_ItemViewer.Constructor", "Performing Solr/Lucene search");

                int page = CurrentRequest.SubPage.HasValue ? Math.Max(CurrentRequest.SubPage.Value, ((ushort)1)) : 1;
                results = Solr_Page_Results.Search(BriefItem.BibID, BriefItem.VID, terms, 20, page, false);

                Tracer.Add_Trace("Text_Search_ItemViewer.Constructor", "Completed Solr/Lucene search in " + results.QueryTime + "ms");
            }
        }

        /// <summary> CSS ID for the viewer viewport for this particular viewer </summary>
        /// <value> This always returns the value 'sbkTsiv_Viewer' </value>
        public override string ViewerBox_CssId
        {
            get { return "sbkTsiv_Viewer"; }
        }

        /// <summary> Write the item viewer main section as HTML directly to the HTTP output stream </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("Text_Search_ItemViewer.Write_Main_Viewer_Section", "");
            }

            string search_this_document = "Search this document";

            if (CurrentRequest.Language == Web_Language_Enum.French)
            {
                search_this_document = "Rechercher sur ce Document";
            }

            if (CurrentRequest.Language == Web_Language_Enum.Spanish)
            {
                search_this_document = "Buscar en este Objeto";
            }

            // Save the original search string
            string originalSearchString = CurrentRequest.Text_Search;

            Output.WriteLine("       <!-- TEXT SEARCH ITEM VIEWER OUTPUT -->");

            // Determine the value without any search
            string currentSearch = CurrentRequest.Text_Search;
            CurrentRequest.Text_Search = String.Empty;
            string redirect_url = UrlWriterHelper.Redirect_URL(CurrentRequest);
            CurrentRequest.Text_Search = currentSearch;
            string button_text = String.Empty;

            // Makee sure the search is not null
            if (String.IsNullOrWhiteSpace(currentSearch))
                currentSearch = String.Empty;

            // Add the search this document portion
            Output.WriteLine("    <td style=\"text-align:center;\">");
            Output.WriteLine("      <div style=\"padding:10px 0 10px 0;\" >");
            Output.WriteLine("        <label for=\"searchTextBox\">" + search_this_document + ":</label> &nbsp;");
            Output.WriteLine("        <input class=\"sbkTsv_SearchBox sbkIsw_Focusable\" id=\"searchTextBox\" name=\"searchTextBox\" type=\"text\" value=\"" + currentSearch.Replace(" =", " or ") + "\" onkeydown=\"item_search_keytrap(event, '" + redirect_url + "');\" /> &nbsp; ");
            Output.WriteLine("        <button title=\"" + search_this_document + "\" class=\"sbkIsw_RoundButton\" onclick=\"item_search_sobekcm('" + redirect_url + "'); return false;\">GO<img src=\"" + Static_Resources_Gateway.Button_Next_Arrow_Png + "\" class=\"roundbutton_img_right\" alt=\"\" /></button>");
            Output.WriteLine("      </div>");
            if (results != null)
            {
                // Display the explanation string, and possibly paging options if there are more results
                Output.WriteLine("      <hr id=\"sbkTsv_HorizontalLine\">");
                Output.WriteLine("    </td>");
                Output.WriteLine("  </tr>");
                Output.WriteLine("  <tr>");
                Output.WriteLine("    <td style=\"text-align:center;\">");
                Output.WriteLine("      <div id=\"sbkTsv_CurrentSearch\">");
                Output.WriteLine(Compute_Search_Explanation());
                Output.WriteLine("</div>");

                if (results.TotalResults > 20)
                {
                    int current_page = CurrentRequest.SubPage.HasValue ? Math.Max(CurrentRequest.SubPage.Value, ((ushort)1)) : 1;

                    string first_page = "First Page";
                    string previous_page = "Previous Page";
                    string next_page = "Next Page";
                    string last_page = "Last Page";
                    string first_page_text = "First";
                    string previous_page_text = "Previous";
                    string next_page_text = "Next";
                    string last_page_text = "Last";

                    if (CurrentRequest.Language == Web_Language_Enum.Spanish)
                    {
                        first_page = "Primera Página";
                        previous_page = "Página Anterior";
                        next_page = "Página Siguiente";
                        last_page = "Última Página";
                        first_page_text = "Primero";
                        previous_page_text = "Anterior";
                        next_page_text = "Proximo";
                        last_page_text = "Último";
                    }

                    if (CurrentRequest.Language == Web_Language_Enum.French)
                    {
                        first_page = "Première Page";
                        previous_page = "Page Précédente";
                        next_page = "Page Suivante";
                        last_page = "Dernière Page";
                        first_page_text = "Première";
                        previous_page_text = "Précédente";
                        next_page_text = "Suivante";
                        last_page_text = "Derniere";
                    }

                    // Use a stringbuilder here
                    StringBuilder buttonWriter = new StringBuilder(2000);

                    buttonWriter.AppendLine("            <div class=\"sbkIsw_PageNavBar\">");

                    // Should the first and previous buttons be shown?
                    if (current_page > 1)
                    {
                        // Get the URL for the first and previous buttons
                        CurrentRequest.SubPage = 1;
                        string firstButtonURL = UrlWriterHelper.Redirect_URL(CurrentRequest);
                        CurrentRequest.SubPage = (ushort)(current_page - 1);
                        string prevButtonURL = UrlWriterHelper.Redirect_URL(CurrentRequest);

                        buttonWriter.AppendLine("              <span class=\"sbkIsw_LeftPaginationButtons\">");
                        buttonWriter.AppendLine("                <button title=\"" + first_page + "\" class=\"sbkIsw_RoundButton\" onclick=\"window.location='" + firstButtonURL + "'; return false;\"><img src=\"" + Static_Resources_Gateway.Button_First_Arrow_Png + "\" class=\"roundbutton_img_left\" alt=\"\" />" + first_page_text + "</button>&nbsp;");
                        buttonWriter.AppendLine("                <button title=\"" + previous_page + "\" class=\"sbkIsw_RoundButton\" onclick=\"window.location='" + prevButtonURL + "'; return false;\"><img src=\"" + Static_Resources_Gateway.Button_Previous_Arrow_Png + "\" class=\"roundbutton_img_left\" alt=\"\" />" + previous_page_text + "</button>");
                        buttonWriter.AppendLine("              </span>");
                    }



                    // Only continue if there is an item and mode, and there is previous pages to go to
                    int total_pages = (int)Math.Ceiling((((decimal)results.TotalResults) / 20));
                    if (current_page < total_pages)
                    {
                        // Get the URL for the first and previous buttons
                        CurrentRequest.SubPage = (ushort)total_pages;
                        string lastButtonURL = UrlWriterHelper.Redirect_URL(CurrentRequest);
                        CurrentRequest.SubPage = (ushort)(current_page + 1);
                        string nextButtonURL = UrlWriterHelper.Redirect_URL(CurrentRequest);

                        buttonWriter.AppendLine("              <span class=\"sbkIsw_RightPaginationButtons\">");
                        buttonWriter.AppendLine("                <button title=\"" + next_page + "\" class=\"sbkIsw_RoundButton\" onclick=\"window.location='" + nextButtonURL + "'; return false;\">" + next_page_text + "<img src=\"" + Static_Resources_Gateway.Button_Next_Arrow_Png + "\" class=\"roundbutton_img_right\" alt=\"\" /></button>&nbsp;");
                        buttonWriter.AppendLine("                <button title=\"" + last_page + "\" class=\"sbkIsw_RoundButton\" onclick=\"window.location='" + lastButtonURL + "'; return false;\">" + last_page_text + "<img src=\"" + Static_Resources_Gateway.Button_Last_Arrow_Png + "\" class=\"roundbutton_img_right\" alt=\"\" /></button>");
                        buttonWriter.AppendLine("              </span>");
                    }

                    buttonWriter.AppendLine("            </div>");

                    button_text = buttonWriter.ToString();
                    Output.WriteLine(button_text);
                    CurrentRequest.SubPage = (ushort)current_page;
                }
            }
            Output.WriteLine("    </td>");
            Output.WriteLine("  </tr>");

            if ((results != null) && (results.TotalResults > 0))
            {
                // Look that some of these have thumbnails
                char columns = '2';
                bool hasThumbs = false;
                if (results.Results.Any(Result => Result.Thumbnail.Length > 0))
                {
                    columns = '3';
                    hasThumbs = true;
                }

                Output.WriteLine("  <tr>");
                Output.WriteLine("    <td id=\"sbkTsv_ResultsArea\">");
                Output.WriteLine("        <table id=\"sbkTsv_ResultsTable\">");

                string thumbnail_root = BriefItem.Web.Source_URL;
                string url_options = UrlWriterHelper.URL_Options(CurrentRequest);
                if (url_options.Length > 0)
                {
                    url_options = url_options + "&search=" + HttpUtility.UrlEncode(originalSearchString);
                }
                else
                {
                    url_options = "?search=" + HttpUtility.UrlEncode(originalSearchString);
                }
                int current_displayed_result = ((results.Page_Number - 1) * 20) + 1;
                bool first = true;
                foreach (Solr_Page_Result result in results.Results)
                {
                    // If this is not the first results drawn, add a seperating line
                    if (!first)
                    {
                        Output.WriteLine("          <tr><td colspan=\"" + columns + "\"></td></tr>");
                    }
                    else
                    {
                        first = false;
                    }

                    Output.WriteLine("          <tr style=\"vertical-align:middle;\">");
                    Output.WriteLine("            <td class=\"sbkTsv_ResultNumber\">" + current_displayed_result + "</td>");

                    // Only include the thumbnail column if some exist
                    if (hasThumbs)
                    {
                        if (result.Thumbnail.Length > 0)
                        {
                            Output.WriteLine("            <td style=\"text-align:left; width: 150px;\"><a href=\"" + CurrentRequest.Base_URL + BriefItem.BibID + "/" + BriefItem.VID + "/" + result.PageOrder + url_options + "\"><img src=\"" + thumbnail_root + "/" + result.Thumbnail + "\" class=\"sbkTsv_Thumbnail\" /></a></td>");
                        }
                        else
                        {
                            Output.WriteLine("            <td style=\"text-align:left; width: 150px;\"><a href=\"" + CurrentRequest.Base_URL + BriefItem.BibID + "/" + BriefItem.VID + "/" + result.PageOrder + url_options + "\"><img src=\"" + Static_Resources_Gateway.Nothumb_Jpg + "\" class=\"sbkTsv_Thumbnail\" /></a></td>");
                        }
                    }

                    Output.WriteLine("            <td style=\"text-align:left;\">");
                    Output.WriteLine("              <a class=\"sbkTsv_ResultsLink\" href=\"" + CurrentRequest.Base_URL + BriefItem.BibID + "/" + BriefItem.VID + "/" + result.PageOrder + url_options + "\">" + result.PageName + "</a>");
                    if (result.Snippet.Length > 0)
                    {
                        Output.WriteLine("              <br /><br />");
                        Output.WriteLine("              &ldquo;..." + result.Snippet.Replace("<em>", "<span class=\"sbkTsv_HighlightText\">").Replace("</em>", "</span>") + "...&rdquo;");
                    }
                    Output.WriteLine("            </td>");
                    Output.WriteLine("          </tr>");

                    current_displayed_result++;
                }

                Output.WriteLine("        </table>");

                Output.WriteLine(button_text);
                Output.WriteLine("    </td>");
            }
            else
            {
                Output.WriteLine("  <tr>");
                Output.WriteLine("    <td class=\"sbkTsv_ResultsArea\">");
                Output.WriteLine("        <br />");
                Output.WriteLine("        <h2>Quick Tips</h2>");
                Output.WriteLine("        <div id=\"sbkTsv_QuickTips\">");
                Output.WriteLine("          <ul>");
                Output.WriteLine("            <li><h3>Document Searching</h3>");
                Output.WriteLine("              <p> This option searches the full-text of the document and returns any pages which match<br />");
                Output.WriteLine("              the conditions of your search.</p>");
                Output.WriteLine("            </li>");
                Output.WriteLine("            <li><h3>Boolean Searches</h3>");
                Output.WriteLine("              <p> Use <span class=\"sbkTsv_Bold\">+</span> or <span class=\"sbkTsv_BoldItalic\">and</span> between terms to find records with <span class=\"sbkTsv_Bold\">all</span> the terms.<br />");
                Output.WriteLine("              Use <span class=\"sbkTsv_Bold\">-</span> or <span class=\"sbkTsv_BoldItalic\">or</span> between terms to find records with <span class=\"sbkTsv_Bold\">any</span> of the terms.<br />");
                Output.WriteLine("              Use <span class=\"sbkTsv_Bold\">!</span> or <span class=\"sbkTsv_BoldItalic\">and not</span> between terms to exclude records with terms.<br />");
                Output.WriteLine("              If nothing is indicated, <span class=\"sbkTsv_BoldItalic\">and</span> is the default.<br />");
                Output.WriteLine("              EXAMPLE: natural and not history");
                Output.WriteLine("              </p>");
                Output.WriteLine("            </li>");
                Output.WriteLine("            <li><h3>Phrase Searching</h3>");
                Output.WriteLine("              <p> Placing quotes around a phrase will search for the exact phrase.<br />");
                Output.WriteLine("              EXAMPLE: &quot;natural history&quot;</p>");
                Output.WriteLine("            </li>");
                Output.WriteLine("            <li><h3>Capitalization</h3>");
                Output.WriteLine("              <p> Searches are not capitalization sensitive.<br />");
                Output.WriteLine("              EXAMPLE: Searching for <span class=\"sbkTsv_Italic\">NATURAL</span> will return the same results as searching for <span class=\"sbkTsv_Italic\">natural</span></p>");
                Output.WriteLine("            </li>");
                Output.WriteLine("            <li><h3>Diacritics</h3>");
                Output.WriteLine("              <p> To search for words with diacritics, the character must be entered into the search box.<br />");
                Output.WriteLine("              EXAMPLE: Searching <span class=\"sbkTsv_Italic\">Précédent</span> is a different search than <span class=\"sbkTsv_Italic\">Precedent</span></p>");
                Output.WriteLine("            </li>");
                Output.WriteLine("          </ul>");
                Output.WriteLine("        </div>");
                Output.WriteLine("        <br />");
                Output.WriteLine("    </td>");
            }
        }

        /// <summary> Allows controls to be added directory to a place holder, rather than just writing to the output HTML stream </summary>
        /// <param name="MainPlaceHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the bulk of the item viewer's output is displayed</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <remarks> This method does nothing, since nothing is added to the place holder as a control for this item viewer </remarks>
        public override void Add_Main_Viewer_Section(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
        {
            // Do nothing
        }

        /// <summary> Returns the textual explanation of the item-level search </summary>
        protected string Compute_Search_Explanation()
        {
            StringBuilder output = new StringBuilder();

            // Split the parts
            List<string> terms = new List<string>();
            List<string> fields = new List<string>();

            // If this is basic, do some other preparation
            string complete_search = CurrentRequest.Text_Search;
            ushort subpage = CurrentRequest.SubPage.HasValue ? Math.Max(CurrentRequest.SubPage.Value, ((ushort)1)) : ((ushort)1);
            CurrentRequest.SubPage = 1;
            Solr_Documents_Searcher.Split_Multi_Terms(CurrentRequest.Text_Search, "ZZ", terms, fields);

            string your_search_language = "Your search within this document for ";
            string and_not_language = " AND NOT ";
            string and_language = " AND ";
            string or_language = " OR ";
            string not_language = "not ";
            string resulted_in_language = " resulted in ";
            string matching_pages_language = " matching pages";
            string no_matches_language = "no matching pages";
            string expand_language = "You can expand your results by searching for";
            string restrict_language = "You can restrict your results by searching for";


            if (CurrentRequest.Language == Web_Language_Enum.French)
            {
                your_search_language = "Votre recherche dans les textes intégrals pour les pages contenant ";
                and_not_language = " ET PAS ";
                and_language = " ET ";
                or_language = " OU ";
                not_language = "pas ";
                resulted_in_language = " corresponde a ";
                matching_pages_language = " pages de résultats";
                no_matches_language = "pas pages de résultats";
                expand_language = "Vous pouvez elaborer votre rechereche en cherchant par";
                restrict_language = "Vous pouvez limiter votre rechereche en cherchant par";
            }

            if (CurrentRequest.Language == Web_Language_Enum.Spanish)
            {
                your_search_language = "Su búsqueda dentro de el texto completo por paginas conteniendo ";
                and_not_language = " Y NO ";
                and_language = " Y ";
                or_language = " O ";
                not_language = "no ";
                resulted_in_language = " resulto en ";
                matching_pages_language = " paginas correspondientes";
                no_matches_language = "no paginas correspondientes";
                expand_language = "Usted puede ampliar sus resultados buscando por";
                restrict_language = "Usted puede disminuir sus resultados buscando por";
            }

            output.Append(your_search_language);
            bool first = true;
            bool allOr = true;
            bool allAnd = true;
            StringBuilder allAndBldr = new StringBuilder(1000);
            StringBuilder allOrBldr = new StringBuilder(1000);
            StringBuilder allAndURL = new StringBuilder(1000);
            StringBuilder allOrURL = new StringBuilder(1000);
            for (int i = 0; i < terms.Count; i++)
            {
                string thisTerm = terms[i];
                if (!first)
                {
                    switch (fields[i][0])
                    {
                        case '-':
                            allOr = false;
                            allAnd = false;
                            output.Append(and_not_language);
                            allAndBldr.Append(and_not_language);
                            allAndURL.Append("+-");
                            break;

                        case '=':
                            output.Append(or_language);
                            allAndBldr.Append(and_language);
                            allOrBldr.Append(or_language);
                            allAnd = false;
                            allAndURL.Append("+");
                            allOrURL.Append("+=");
                            break;

                        default:
                            output.Append(and_language);
                            allAndBldr.Append(and_language);
                            allOrBldr.Append(or_language);
                            allOr = false;
                            allAndURL.Append("+");
                            allOrURL.Append("+=");
                            break;
                    }
                }
                else
                {
                    first = false;
                    if (fields[i][0] == '-')
                    {
                        output.Append(not_language);
                        allAndURL.Append("-");
                    }
                }

                // Write the term
                if (thisTerm[0] == '"')
                {
                    CurrentRequest.Text_Search = thisTerm;
                    output.Append("<a href=\"" + UrlWriterHelper.Redirect_URL(CurrentRequest) + "\">" + thisTerm.Replace("+", " ") + "</a>");

                    if (fields[i][0] == '-')
                    {
                        allAndBldr.Append(thisTerm.Replace("+", " "));
                        allAndURL.Append(thisTerm.Replace("\"", "%22"));
                    }
                    else
                    {
                        allAndBldr.Append(thisTerm.Replace("+", " "));
                        allAndURL.Append(thisTerm.Replace("\"", "%22"));
                        allOrBldr.Append(thisTerm.Replace("+", " "));
                        allOrURL.Append(thisTerm.Replace("\"", "%22"));
                    }
                }
                else
                {
                    CurrentRequest.Text_Search = thisTerm;
                    output.Append("<a href=\"" + UrlWriterHelper.Redirect_URL(CurrentRequest) + "\">'" + thisTerm + "'</a>");

                    if (fields[i][0] == '-')
                    {
                        allAndBldr.Append(thisTerm.Replace("+", " "));
                        allAndURL.Append(thisTerm.Replace("\"", "%22"));
                    }
                    else
                    {
                        allAndBldr.Append(thisTerm.Replace("+", " "));
                        allAndURL.Append(thisTerm.Replace("\"", "%22"));
                        allOrBldr.Append(thisTerm.Replace("+", " "));
                        allOrURL.Append(thisTerm.Replace("\"", "%22"));
                    }
                }
            }
            output.Append(resulted_in_language);

            if (results.TotalResults > 0)
            {
                output.AppendLine(number_to_string(results.TotalResults) + matching_pages_language + ".");
            }
            else
            {
                output.AppendLine("<b>" + no_matches_language + "</b>.");
            }

            if (!allOr)
            {
                CurrentRequest.Text_Search = allOrURL.ToString();
                output.AppendLine("<br /><br />" + expand_language + " <a href=\"" + UrlWriterHelper.Redirect_URL(CurrentRequest) + "\">" + allOrBldr + "</a>.");
            }

            if ((!allAnd) && (results.TotalResults > 0))
            {
                CurrentRequest.Text_Search = allAndURL.ToString();
                output.AppendLine("<br /><br />" + restrict_language + " <a href=\"" + UrlWriterHelper.Redirect_URL(CurrentRequest) + "\">" + allAndBldr + "</a>.");
            }

            // Restore the original values
            CurrentRequest.Text_Search = complete_search;
            CurrentRequest.SubPage = subpage;

            return output.ToString();
        }


        private static string number_to_string(int Number)
        {
            switch (Number)
            {
                case 1: return "one";
                case 2: return "two";
                case 3: return "three";
                case 4: return "four";
                case 5: return "five";
                case 6: return "six";
                case 7: return "seven";
                case 8: return "eight";
                case 9: return "nine";
                case 10: return "ten";
                case 11: return "eleven";
                case 12: return "twelve";
                default: return Number.ToString();
            }
        }
    }
}
