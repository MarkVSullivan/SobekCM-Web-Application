#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using SobekCM.Library.Application_State;
using SobekCM.Library.Configuration;
using SobekCM.Library.Database;
using SobekCM.Library.Email;
using SobekCM.Library.Navigation;
using SobekCM.Library.Results;
using SobekCM.Library.ResultsViewer;
using SobekCM.Library.Search;
using SobekCM.Library.Users;

#endregion

namespace SobekCM.Library.HTML
{
    /// <summary> Accepts a result set of titles and items and renders the correct page of results in the result view the user has requested </summary>
    /// <remarks> This class extends the <see cref="abstractHtmlSubwriter"/> abstract class. </remarks>
    public class PagedResults_HtmlSubwriter : abstractHtmlSubwriter
    {
        private const int MINIMIZED_FACET_COUNT = 10;
        private const int MAXIMIZED_FACET_COUNT = 100;
        private const int RESULTS_PER_PAGE = 20;

        private readonly Item_Lookup_Object allItems;
        private string buttons;
        private readonly Aggregation_Code_Manager codeManager;
        private readonly User_Object currentUser;
        private readonly string facetInformation;
        private readonly List<iSearch_Title_Result> pagedResults;
        private iResultsViewer resultWriter;
        private readonly Search_Results_Statistics resultsStatistics;
        private string sortOptions;
        private readonly Language_Support_Info translations;

        /// <summary> Constructor for a new instance of the paged_result_html_subwriter class </summary>
        /// <param name="Results_Statistics"> Information about the entire set of results for a search or browse </param>
        /// <param name="Paged_Results"> Single page of results for a search or browse, within the entire set </param>
        /// <param name="Code_Manager"> List of valid collection codes, including mapping from the Sobek collections to Greenstone collections</param>
        /// <param name="Translator"> Language support object which handles simple translational duties </param>
        /// <param name="All_Items_Lookup"> Lookup object used to pull basic information about any item loaded into this library </param>
        /// <param name="Current_User"> Currently logged on user </param>
        /// <param name="Current_Mode"> Mode / navigation information for the current request</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public PagedResults_HtmlSubwriter(Search_Results_Statistics Results_Statistics,
            List<iSearch_Title_Result> Paged_Results,
            Aggregation_Code_Manager Code_Manager,
            Language_Support_Info Translator, Item_Lookup_Object All_Items_Lookup, 
            User_Object Current_User,
            SobekCM_Navigation_Object Current_Mode,
            Custom_Tracer Tracer )
        {
            currentUser = Current_User;
            pagedResults = Paged_Results;
            resultsStatistics = Results_Statistics;
            codeManager = Code_Manager;
            translations = Translator;
            Browse_Title = String.Empty;
            allItems = All_Items_Lookup;
            sortOptions = String.Empty;
            buttons = String.Empty;
            Showing_Text = String.Empty;
            Include_Bookshelf_View = false;
            Outer_Form_Name = String.Empty;
            currentMode= Current_Mode;
            Folder_Owner_Name = String.Empty;
            Folder_Owner_Email = String.Empty;

            // Try to get the facet configuration information
            facetInformation = "0000000";
            if (HttpContext.Current.Request.Form["facet"] != null)
                facetInformation = HttpContext.Current.Request.Form["facet"].PadRight(7, '0');

            if ( true ) // if (currentMode.isPostBack)
            {
                // Pull the standard values
                NameValueCollection form = HttpContext.Current.Request.Form;

                if (form["item_action"] != null)
                {
                    string action = form["item_action"].ToLower().Trim();
                    string url_description = form["url_description"].Trim();
                    
                    if (action == "email")
                    {
                        string address = form["email_address"].Replace(";", ",").Trim();
                        string comments = form["email_comments"].Trim();
                        string format = form["email_format"].Trim().ToUpper();
                        string list_type = form["list_type"].Trim();

                        if (address.Length > 0)
                        {
                            // Determine the email format
                            bool is_html_format = true;
                            if (format == "TEXT")
                                is_html_format = false;

                            // CC: the user, unless they are already on the list
                            string cc_list = currentUser.Email;
                            if (address.ToUpper().IndexOf(currentUser.Email.ToUpper()) >= 0)
                                cc_list = String.Empty;

                            // Send the email
                            string any_error = URL_Email_Helper.Send_Email(address, cc_list, comments, currentUser.Full_Name, currentMode.SobekCM_Instance_Abbreviation, is_html_format, HttpContext.Current.Items["Original_URL"].ToString(), url_description, list_type);
                            HttpContext.Current.Session.Add("ON_LOAD_MESSAGE", any_error.Length > 0 ? any_error : "Your email has been sent");

                            currentMode.isPostBack = true;

                            // Do this to force a return trip (cirumnavigate cacheing)
                            string original_url = HttpContext.Current.Items["Original_URL"].ToString();
                            if ( original_url.IndexOf("?") < 0 )
                                HttpContext.Current.Response.Redirect(original_url + "?p=" + DateTime.Now.Millisecond, false);
                            else
                                HttpContext.Current.Response.Redirect(original_url + "&p=" + DateTime.Now.Millisecond, false);
                        }
                    }

                    if (action == "save_search")
                    {
                        string usernotes = HttpContext.Current.Request.Form["add_notes"].Trim();
                        bool open_searches = false;
                        if (HttpContext.Current.Request.Form["open_searches"] != null)
                            open_searches = true;

                        string original_url = HttpContext.Current.Items["Original_URL"].ToString();
                        if (SobekCM_Database.Save_User_Search(currentUser.UserID, original_url , url_description, 0, usernotes, Tracer) != -1000)
                        {
                            if (open_searches)
                            {
                                HttpContext.Current.Session.Add("ON_LOAD_WINDOW", "?m=lms");
                            }

                            HttpContext.Current.Session.Add("ON_LOAD_MESSAGE", "Search has been saved to your saved searches.");
                        }
                        else
                        {
                            HttpContext.Current.Session.Add("ON_LOAD_MESSAGE", "ERROR encountered while saving!");
                        }

                        // Do this to force a return trip (cirumnavigate cacheing)
                        currentMode.isPostBack = true;
                        if (original_url.IndexOf("?") > 0)
                        {
                            HttpContext.Current.Response.Redirect(original_url + "&p=" + DateTime.Now.Millisecond, false);
                        }
                        else
                        {
                            HttpContext.Current.Response.Redirect(original_url + "?p=" + DateTime.Now.Millisecond, false);

                        }
                    }
                }
            }
        }

        /// <summary> If the results dataset should be displayed in the context of an outer form (such as in
        /// the case that this is part of the myUFDC bookshelf functionality) then the form name should go here.  If 
        /// no outer form name is provided, this will create its own sort form  </summary>
        public string Outer_Form_Name { get; set; }

        /// <summary> Flag indicates if the bookshelf view should be included in the list of possible views  </summary>
        public bool Include_Bookshelf_View { get; set; }

        /// <summary> Name of the owner of this folder </summary>
        public string Folder_Owner_Name { private get; set; }

        /// <summary> Email of the owner of this folder </summary>
        public string Folder_Owner_Email { private get; set; }

        /// <summary> Text which indicates which values of the current result or browse are being shown</summary>
        public string Showing_Text { get; private set; }

        /// <summary> Title for the current view, which is used rather than the search explanation </summary>
        public string Browse_Title { get; set; }

        /// <summary> HTML for the navigation row which links to previous and later pages from the result set </summary>
        public string Buttons
        {
            get
            {
                if (buttons.Length == 0)
                {
                    string first_page = "First Page";
                    string previous_page = "Previous Page";
                    string next_page = "Next Page";
                    string last_page = "Last Page";

                    if (currentMode.Language == Web_Language_Enum.Spanish)
                    {
                        first_page = "Primera Página";
                        previous_page = "Página Anterior";
                        next_page = "Página Siguiente";
                        last_page = "Última Página";
                    }

                    if (currentMode.Language == Web_Language_Enum.French)
                    {
                        first_page = "Première Page";
                        previous_page = "Page Précédente";
                        next_page = "Page Suivante";
                        last_page = "Dernière Page";
                    }

                    // Make sure the result writer has been created
                    if (resultWriter == null)
                        create_resultwriter();
                    Debug.Assert(resultWriter != null, "resultWriter != null");

                    StringBuilder buttons_builder = new StringBuilder(1000);

                    string inter_page_navigation = String.Empty;
                    if ((currentMode.Result_Display_Type == Result_Display_Type_Enum.Full_Citation) || (currentMode.Result_Display_Type == Result_Display_Type_Enum.Full_Image))
                        inter_page_navigation = "#image";

                    ushort current_page = currentMode.Page;

                    if (RESULTS_PER_PAGE < resultWriter.Total_Results)
                    {
                        string language_suffix = currentMode.Language_Code;
                        if (language_suffix.Length > 0)
                            language_suffix = "_" + language_suffix;

                        buttons_builder.Append("  <span class=\"leftButtons\">" + Environment.NewLine );
                        // Should the previous and first buttons be enabled?
                        if (current_page > 1)
                        {
                            currentMode.Page = 1;
                            buttons_builder.Append("    <a href=\"" + currentMode.Redirect_URL().Replace("&", "&amp;") + inter_page_navigation + "\"><img src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/first_button" + language_suffix + ".gif\" border=\"0\" alt=\"" + first_page + "\" /></a>&nbsp;" + Environment.NewLine );
                            currentMode.Page = (ushort)(current_page - 1);
                            buttons_builder.Append("    <a href=\"" + currentMode.Redirect_URL().Replace("&", "&amp;") + inter_page_navigation + "\"><img src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/previous_button" + language_suffix + ".gif\" border=\"0\" alt=\"" + previous_page + "\" /></a>" + Environment.NewLine );
                        }
                        else
                        {
                            buttons_builder.Append("    <img src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/no_button_spacer.gif\" alt=\"\" />" + Environment.NewLine );
                        }
                        buttons_builder.Append("  </span>" + Environment.NewLine );
                        buttons_builder.Append("  <span class=\"rightButtons\">" + Environment.NewLine );

                        // Should the next and last buttons be enabled?
                        if ((current_page * RESULTS_PER_PAGE) < resultWriter.Total_Results)
                        {
                            currentMode.Page = (ushort)(current_page + 1);
                            buttons_builder.Append("    <a href=\"" + currentMode.Redirect_URL().Replace("&", "&amp;") + inter_page_navigation + "\"><img src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/next_button" + language_suffix + ".gif\" border=\"0\" alt=\"" + next_page + "\" /></a>&nbsp;" + Environment.NewLine );
                            currentMode.Page = (ushort)(resultWriter.Total_Results / RESULTS_PER_PAGE);
                            if (resultWriter.Total_Results % RESULTS_PER_PAGE > 0)
                                currentMode.Page = (ushort)(currentMode.Page + 1);
                            buttons_builder.Append("    <a href=\"" + currentMode.Redirect_URL().Replace("&", "&amp;") + inter_page_navigation + "\"><img src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/last_button" + language_suffix + ".gif\" border=\"0\" alt=\"" + last_page + "\" /></a>" + Environment.NewLine );
                        }
                        else
                        {
                            buttons_builder.Append("    <img src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/no_button_spacer.gif\" alt=\"\" />" + Environment.NewLine );
                        }
                        buttons_builder.Append("  </span>" + Environment.NewLine );
                        currentMode.Page = current_page;
                    }

                    buttons = buttons_builder.ToString();
                }
                return buttons;
            }
        }

        /// <summary> Creates the specific results viewer according the user's preferences in the current request mode </summary>
        private void create_resultwriter()
        {
            if ( resultsStatistics.Total_Items == 0)
            {
                resultWriter = new No_Results_ResultsViewer
                                   {CurrentMode = currentMode, Results_Statistics = resultsStatistics};
                return;
            }

            // If this is default, determine the type from the aggregation (currently) or user
            if (currentMode.Result_Display_Type == Result_Display_Type_Enum.Default)
            {
                if (currentMode.Coordinates.Length > 0)
                    currentMode.Result_Display_Type = Result_Display_Type_Enum.Map;
                else
                {
                    string user_view = "default";
                    if (HttpContext.Current.Session["User_Default_View"] != null)
                        user_view = HttpContext.Current.Session["User_Default_View"].ToString();
                    currentMode.Result_Display_Type = Hierarchy_Object.Default_Result_View;
                    switch (user_view)
                    {
                        case "brief":
                            if (Hierarchy_Object.Result_Views.Contains(Result_Display_Type_Enum.Brief))
                                currentMode.Result_Display_Type = Result_Display_Type_Enum.Brief;
                            break;

                        case "thumb":
                            if (Hierarchy_Object.Result_Views.Contains(Result_Display_Type_Enum.Thumbnails))
                                currentMode.Result_Display_Type = Result_Display_Type_Enum.Thumbnails;
                            break;

                        case "table":
                            if (Hierarchy_Object.Result_Views.Contains(Result_Display_Type_Enum.Table))
                                currentMode.Result_Display_Type = Result_Display_Type_Enum.Table;
                            break;

                    }

                }
            }

            // Create the bookshelf view
            if (currentMode.Result_Display_Type == Result_Display_Type_Enum.Bookshelf)
            {
                if (currentMode.Mode == Display_Mode_Enum.My_Sobek)
                {
                    resultWriter = new Bookshelf_View_ResultsViewer(allItems, currentUser);
                }
                else
                {
                    resultWriter = new Brief_ResultsViewer(allItems);
                }
            }

            // Create the result writer and populate the sort list for BRIEF view
            if (currentMode.Result_Display_Type == Result_Display_Type_Enum.Brief)
            {
                resultWriter = new Brief_ResultsViewer(allItems);
            }

            // Create the result writer and populate the sort list for THUMBNAIL view
            if (currentMode.Result_Display_Type == Result_Display_Type_Enum.Thumbnails)
            {
                resultWriter = new Thumbnail_ResultsViewer(allItems);
                ((Thumbnail_ResultsViewer)resultWriter).Code_Manager = codeManager;
            }

            // Create the result writer and populate the sort list for TABLE view
            if (currentMode.Result_Display_Type == Result_Display_Type_Enum.Table)
            {
                resultWriter = new Table_ResultsViewer(allItems);
            }

            // Create the result writer and populate the sort list for FULL view
            if ((currentMode.Result_Display_Type == Result_Display_Type_Enum.Full_Citation) || (currentMode.Result_Display_Type == Result_Display_Type_Enum.Full_Image))
            {
                resultWriter = new Full_ResultsViewer(allItems);
            }

            // Create the result writer and populate the sort list for MAP view
            if (currentMode.Result_Display_Type == Result_Display_Type_Enum.Map)
            {
                resultWriter = new Map_ResultsViewer(allItems);
            }

            // Create the result writer and populate the sort list for TEXT view
            if (currentMode.Result_Display_Type == Result_Display_Type_Enum.Export)
            {
                resultWriter = new Export_File_ResultsViewer(allItems, currentUser);
            }

            resultWriter.CurrentMode = currentMode;
            resultWriter.Results_Statistics = resultsStatistics;
            resultWriter.Paged_Results = pagedResults;
            resultWriter.HierarchyObject = Hierarchy_Object;
            resultWriter.Translator = translations;
            
            // Populate the sort list and sort the result set
            sortOptions = String.Empty;
            StringBuilder sort_options_builder = new StringBuilder(1000);
            if ((resultWriter.Sortable) && (!currentMode.Is_Robot))
            {
                // Add the special sorts for browses
                if (currentMode.Mode == Display_Mode_Enum.Aggregation_Browse_Info)
                {
                    if (currentMode.Info_Browse_Mode.ToUpper().IndexOf("NEW") >= 0)
                    {
                        if (currentMode.Sort == 0)
                        {
                            sort_options_builder.Append("      <option value=\"" + 0 + "\" selected=\"selected\">" + translations.Get_Translation("Date Added", currentMode.Language) + "</option>" + Environment.NewLine );
                        }
                        else
                        {
                            sort_options_builder.Append("      <option value=\"" + 0 + "\">" + translations.Get_Translation("Date Added", currentMode.Language) + "</option>" + Environment.NewLine );
                        }

                        if (currentMode.Sort == 1)
                        {
                            sort_options_builder.Append("      <option value=\"" + 1 + "\" selected=\"selected\">" + translations.Get_Translation("Title", currentMode.Language) + "</option>" + Environment.NewLine );
                        }
                        else
                        {
                            sort_options_builder.Append("      <option value=\"" + 1 + "\">" + translations.Get_Translation("Title", currentMode.Language) + "</option>" + Environment.NewLine );
                        }
                    }
                    else
                    {
                        if ((currentMode.Sort == 0) || (currentMode.Sort == 1))
                        {
                            sort_options_builder.Append("      <option value=\"" + 0 + "\" selected=\"selected\">" + translations.Get_Translation("Title", currentMode.Language) + "</option>" + Environment.NewLine );
                        }
                        else
                        {
                            sort_options_builder.Append("      <option value=\"" + 0 + "\">" + translations.Get_Translation("Title", currentMode.Language) + "</option>" + Environment.NewLine );
                        }
                    }
                }

                // Add the special sorts for searches
                if (currentMode.Mode == Display_Mode_Enum.Results)
                {
                    if (currentMode.Sort == 0)
                    {
                        sort_options_builder.Append("      <option value=\"" + 0 + "\" selected=\"selected\">" + translations.Get_Translation("Rank", currentMode.Language) + "</option>" + Environment.NewLine );
                    }
                    else
                    {
                        sort_options_builder.Append("      <option value=\"" + 0 + "\">" + translations.Get_Translation("Rank", currentMode.Language) + "</option>" + Environment.NewLine );
                    }

                    if (currentMode.Sort == 1)
                    {
                        sort_options_builder.Append("      <option value=\"" + 1 + "\" selected=\"selected\">" + translations.Get_Translation("Title", currentMode.Language) + "</option>" + Environment.NewLine );
                    }
                    else
                    {
                        sort_options_builder.Append("      <option value=\"" + 1 + "\">" + translations.Get_Translation("Title", currentMode.Language) + "</option>" + Environment.NewLine );
                    }
                }

                // Add the bibid sorts if this is an internal user
                if (currentMode.Internal_User)
                {
                    if (currentMode.Sort == 2)
                    {
                        sort_options_builder.Append("      <option value=\"" + 2 + "\" selected=\"selected\">" + translations.Get_Translation("BibID Ascending", currentMode.Language) + "</option>" + Environment.NewLine );
                    }
                    else
                    {
                        sort_options_builder.Append("      <option value=\"" + 2 + "\">" + translations.Get_Translation("BibID Ascending", currentMode.Language) + "</option>" + Environment.NewLine );
                    }

                    if (currentMode.Sort == 3)
                    {
                        sort_options_builder.Append("      <option value=\"" + 3 + "\" selected=\"selected\">" + translations.Get_Translation("BibID Descending", currentMode.Language) + "</option>" + Environment.NewLine );
                    }
                    else
                    {
                        sort_options_builder.Append("      <option value=\"" + 3 + "\">" + translations.Get_Translation("BibID Descending", currentMode.Language) + "</option>" + Environment.NewLine );
                    }
                }

                // Add the publication date sorts
                if (currentMode.Sort == 10)
                {
                    sort_options_builder.Append("      <option value=\"" + 10 + "\" selected=\"selected\">" + translations.Get_Translation("Date Ascending", currentMode.Language) + "</option>" + Environment.NewLine );
                }
                else
                {
                    sort_options_builder.Append("      <option value=\"" + 10 + "\">" + translations.Get_Translation("Date Ascending", currentMode.Language) + "</option>" + Environment.NewLine );
                }

                if (currentMode.Sort == 11)
                {
                    sort_options_builder.Append("      <option value=\"" + 11 + "\" selected=\"selected\">" + translations.Get_Translation("Date Descending", currentMode.Language) + "</option>" + Environment.NewLine );
                }
                else
                {
                    sort_options_builder.Append("      <option value=\"" + 11 + "\">" + translations.Get_Translation("Date Descending", currentMode.Language) + "</option>" + Environment.NewLine );
                }

                sortOptions = sort_options_builder.ToString();
            }
        }

        /// <summary> Adds controls to the main navigational page </summary>
        /// <param name="placeHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form, widely used throughout the application</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Sorted tree with the results in hierarchical structure with volumes and issues under the titles and sorted by serial hierarchy </returns>
        public void Add_Controls(PlaceHolder placeHolder, Custom_Tracer Tracer )
        {
            Tracer.Add_Trace("paged_result_html_subwriter.Add_Controls", "Adding controls for the result set");

            // If the results have facets, this should be rendered in a table with the facets to the left
            if ((resultsStatistics.Has_Facet_Info) && (resultsStatistics.Total_Items > 1) && (currentMode.Result_Display_Type != Result_Display_Type_Enum.Export) && (currentMode.Result_Display_Type != Result_Display_Type_Enum.Map))
            {
                // Start this table, write the facets, and start the next TD section for the results
                Literal startFacetTable = new Literal
                                              { Text = string.Format("<table>" + Environment.NewLine + "<tr valign=\"top\">" + Environment.NewLine + "<td width=\"240px\" valign=\"top\" height=\"100%\">" + Environment.NewLine + "{0}" + Environment.NewLine + "</td>" + Environment.NewLine + "<td>" + Environment.NewLine , Add_Facet_Information(Tracer)) };
                placeHolder.Controls.Add(startFacetTable);
            }
            else
            {
                Literal startFacetTable = new Literal
                                              { Text =  "<table width=\"100%\">" + Environment.NewLine + "<tr valign=\"top\">" + Environment.NewLine + "<td align=\"center\">" + Environment.NewLine  };
                placeHolder.Controls.Add(startFacetTable);
            }

            // Make sure the result writer has been created
            if (resultWriter == null)
                create_resultwriter();
            Debug.Assert(resultWriter != null, "resultWriter != null");

            if (resultsStatistics.Total_Items == 0)
            {
                resultWriter.Add_HTML(placeHolder, Tracer);
                return;
            }

            Literal startingLiteral = new Literal
                                          { Text = currentMode.Result_Display_Type == Result_Display_Type_Enum.Map ? "</div>" + Environment.NewLine + "<div class=\"SobekResultsPanel\" align=\"center\">" + Environment.NewLine  : "<div class=\"SobekResultsPanel\" align=\"center\">" + Environment.NewLine  };
            placeHolder.Controls.Add(startingLiteral);

            resultWriter.Add_HTML(placeHolder, Tracer );

            Literal endingLiteral = new Literal
                                        { Text = currentMode.Result_Display_Type == Result_Display_Type_Enum.Map ? "</div>" + Environment.NewLine + "<div id=\"pagecontainer_resumed\">" + Environment.NewLine  : "</div>" + Environment.NewLine  };
            placeHolder.Controls.Add(endingLiteral);

            // If the results have facets, end the result table
            Literal endResultTable = new Literal {Text = "</td>" + Environment.NewLine + "</tr>" + Environment.NewLine + "</table>" + Environment.NewLine };
            placeHolder.Controls.Add(endResultTable);
        }

        /// <summary> Writes the final output to close this result view, including the results page navigation buttons </summary>
        /// <param name="Output"> Stream to which to write the HTML for this subwriter </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Final_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("paged_result_html_subwriter.Write_Final_Html", "Rendering HTML ( finish the main viewer section )");

            if ( resultsStatistics.Total_Items > 0 )
            {
                Output.WriteLine("<div class=\"SobekResultsNavBar\">");
                Output.Write(buttons);
                Output.WriteLine("  " + Showing_Text);
                Output.WriteLine("</div>" + Environment.NewLine + "<br />" + Environment.NewLine );
            }
        }

        /// <summary> Writes the HTML generated to browse the list of titles/itemsr  directly to the response stream </summary>
        /// <param name="Output"> Stream to which to write the HTML for this subwriter </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> TRUE -- Value indicating if html writer should finish the page immediately after this, or if there are other controls or routines which need to be called first </returns>
        public override bool Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("paged_result_html_subwriter.Write_HTML", "Rendering HTML");

            string bookshelf_view = "BOOKSHELF VIEW";
            string brief_view = "BRIEF VIEW";
            string map_view = "MAP VIEW";
            string table_view = "TABLE VIEW";
            string thumbnail_view = "THUMBNAIL VIEW";
            const string exportView = "EXPORT";
            string sort_by = "Sort By";
            string showing_range_text = "{0} - {1} of {2} matching titles";
            string showing_coord_range_text = "{0} - {1} of {2} matching coordinates";

            if (currentMode.Aggregation == "aerials")
            {
                showing_coord_range_text = "{0} - {1} of {2} matching flights";
            }

            if (currentMode.Language == Web_Language_Enum.Spanish)
            {
                bookshelf_view = "VISTA BIBLIOTECA";
                map_view = "VISTA MAPA";
                brief_view = "VISTA BREVE";
                table_view = "VISTA TABLERA";
                thumbnail_view = "VISTA MINIATURA";
                sort_by = "Organizar";
                showing_range_text = "{0} - {1} de {2} títulos correspondientes";
            }

            if (currentMode.Language == Web_Language_Enum.French)
            {
                bookshelf_view = "MODE MA BIBLIOTHEQUE";
                map_view = "MODE CARTE";
                brief_view = "MODE SIMPLE";
                table_view = "MODE DE TABLE";
                thumbnail_view = "MODE IMAGETTE";
                sort_by = "Limiter";
                showing_range_text = "{0} - {1} de {2} titres correspondants";
            }

            if (currentMode.Result_Display_Type == Result_Display_Type_Enum.Map)
                showing_range_text = showing_coord_range_text;

            Display_Mode_Enum initialMode = currentMode.Mode;

            Tracer.Add_Trace("paged_result_html_subwriter.Write_HTML", "Building appropriate ResultsWriter");

            currentMode.Mode = initialMode;
            if (currentMode.Mode == Display_Mode_Enum.Search)
                currentMode.Mode = Display_Mode_Enum.Results;

            // If no results, display different information here
            if ((currentMode.Mode == Display_Mode_Enum.Results) && ( resultsStatistics.Total_Items == 0))
            {
                Output.WriteLine("<div class=\"SobekResultsDescPanel\">");
                Output.WriteLine("  <br /><br />");
                Show_Search_Info(Output);
                Output.WriteLine("</div>");
                Output.WriteLine("<div class=\"SobekResultsNavBarImbed\">&nbsp;</div>");
                return true;
            }

            // Make sure the result writer has been created
            if (resultWriter == null)
                create_resultwriter();
           Debug.Assert(resultWriter != null, "resultWriter != null");

            // Determine which rows are being displayed
            int lastRow = currentMode.Page * RESULTS_PER_PAGE;
            int startRow = lastRow - 19;

            // Start the form for this, unless we are already in an appropriate form
            string form_name = Outer_Form_Name;
            if (form_name.Length == 0)
                form_name = "sort_form";
            if (Outer_Form_Name.Length == 0)
            {
                string post_url = HttpUtility.HtmlEncode(HttpContext.Current.Items["Original_URL"].ToString());
                Output.WriteLine("<form name=\"sort_form\" id=\"addedForm\" method=\"post\" action=\"" + post_url + "\" >");
            }

            // Get the name of this
            string currentName = "browse";
            string currentTitle = "B<span class=\"smaller\">ROWSE</span>";
            if (currentMode.Mode == Display_Mode_Enum.Results)
            {
                currentName = "search";
                currentTitle = "S<span class=\"smaller\">EARCH</span>";
            }
            if (currentMode.Mode == Display_Mode_Enum.Public_Folder)
            {
                currentName = "public bookshelf";
                currentTitle = "P<span class=\"smaller\">UBLIC</span> B<span class=\"smaller\">OOKSHELF</span>";
            }
            
            // Write the HTML for the sort and then description and buttons, etc..
            switch (currentMode.Mode)
            {
                case Display_Mode_Enum.Public_Folder:
                    Output.WriteLine("<div class=\"SobekFolderDescPanel\">");
                    break;

                case Display_Mode_Enum.Aggregation_Browse_Info:
                    Output.WriteLine("<div class=\"SobekBrowseDescPanel\">");
                    break;

                default:
                    Output.WriteLine("<div class=\"SobekResultsDescPanel\">");
                    break;
            }

            short current_order = currentMode.Sort;
            currentMode.Sort = 0;
            string url = currentMode.Redirect_URL();
            currentMode.Sort = current_order;

            Output.WriteLine("  <span class=\"SobekResultsSort\">");
            Output.Write("<a href=\"\" onmouseover=\"document." + form_name + ".print_button.src='" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/print_rect_button_h.gif'\" onmouseout=\"document." + form_name + ".print_button.src='" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/print_rect_button.gif'\" onclick=\"window.print(); return false;\"><img class=\"ResultSavePrintButtons\" border=\"0px\" name=\"print_button\" id=\"print_button\" src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/print_rect_button.gif\" title=\"Print this " + currentName + "\" alt=\"PRINT\" /></a>");

            if (currentUser != null)
            {
                Output.Write("<a href=\"\" onmouseover=\"document." + form_name + ".send_button.src='" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/send_rect_button_h.gif'\" onmouseout=\"document." + form_name + ".send_button.src='" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/send_rect_button.gif'\" onclick=\"return email_form_open2('send_button','');\"><img class=\"ResultSavePrintButtons\" border=\"0px\" name=\"send_button\" id=\"send_button\" src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/send_rect_button.gif\" title=\"Send this " + currentName + " to someone\" alt=\"SEND\" /></a>");
                if (currentMode.Mode != Display_Mode_Enum.My_Sobek)
                {
                    Output.Write("<a href=\"\" onmouseover=\"document." + form_name + ".save_button.src='" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/save_rect_button_h.gif'\" onmouseout=\"document." + form_name + ".save_button.src='" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/save_rect_button.gif'\" onclick=\"return save_search_form_open('save_button','');\"><img class=\"ResultSavePrintButtons\" border=\"0px\" name=\"save_button\" id=\"save_button\" src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/save_rect_button.gif\" title=\"Save this " + currentName + "\" alt=\"SAVE\" /></a>");
                }
            }
            else
            {
                Output.Write("<a href=\"?m=hmh\" onmouseover=\"document." + form_name + ".send_button.src='" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/send_rect_button_h.gif'\" onmouseout=\"document." + form_name + ".send_button.src='" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/send_rect_button.gif'\"><img class=\"ResultSavePrintButtons\" border=\"0px\" name=\"send_button\" id=\"send_button\" src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/send_rect_button.gif\" title=\"Send this " + currentName + " to someone\" alt=\"SEND\" /></a>");
                Output.Write("<a href=\"?m=hmh\" onmouseover=\"document." + form_name + ".save_button.src='" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/save_rect_button_h.gif'\" onmouseout=\"document." + form_name + ".save_button.src='" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/save_rect_button.gif'\"><img class=\"ResultSavePrintButtons\" border=\"0px\" name=\"save_button\" id=\"save_button\" src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/save_rect_button.gif\" title=\"Save this " + currentName + "\" alt=\"SAVE\"  /></a>");
            }

            if (currentMode.Mode != Display_Mode_Enum.My_Sobek)
            {
                Output.Write("<a href=\"\" onmouseover=\"document." + form_name + ".share_button.src='" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/share_rect_button_h.gif'\" onmouseout=\"document." + form_name + ".share_button.src='" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/share_rect_button.gif'\" onclick=\"return toggle_share_form2('share_button');\"><img class=\"ResultSavePrintButtons\" border=\"0px\" name=\"share_button\" id=\"share_button\" src=\"" + currentMode.Base_URL + "design/skins/" + htmlSkin.Base_Skin_Code + "/buttons/share_rect_button.gif\" title=\"Share this " + currentName + "\" alt=\"SHARE\" /></a>");
            }

            string summation;
            if ((currentMode.Mode == Display_Mode_Enum.Aggregation_Browse_Info) || ( currentMode.Mode == Display_Mode_Enum.Public_Folder ) || ((currentMode.Mode == Display_Mode_Enum.My_Sobek) && (currentMode.My_Sobek_Type == My_Sobek_Type_Enum.Folder_Management)))
            {
                if ((resultWriter.Sortable) && (!currentMode.Is_Robot) && (currentMode.Mode != Display_Mode_Enum.My_Sobek) && ( currentMode.Mode != Display_Mode_Enum.Public_Folder ))
                {
                    Output.WriteLine("    <br />");
                    Output.WriteLine("    " + sort_by + ": &nbsp;");
                    Output.WriteLine("    <select name=\"sorter_input\" onchange=\"javascript:sort_results('" + url.Replace("&","&amp;") + "')\" id=\"sorter_input\">");
                    Output.WriteLine(sortOptions);
                    Output.WriteLine("    </select>");
                }
                Output.WriteLine("  </span>");
                if (currentMode.Mode == Display_Mode_Enum.Public_Folder)
                {
                    Output.WriteLine("  <h1>&quot;" + translations.Get_Translation(Browse_Title, currentMode.Language) + "&quot;</h1>");
                    Output.WriteLine("  <span class=\"publicFolderAuthor\">This is a publicly shared bookshelf of <a href=\"mailto:" + Folder_Owner_Email + "\">" + Folder_Owner_Name + "</a>.</span>");

                    summation = translations.Get_Translation(Browse_Title, currentMode.Language) + " (publicly shared folder)";
                }
                else
                {
                    Output.WriteLine("  <h1>" + translations.Get_Translation(Browse_Title, currentMode.Language) + "</h1>");
                    summation = translations.Get_Translation(Browse_Title, currentMode.Language) + " browse in " + Hierarchy_Object.Name; 
                }                   
            }
            else
            {

                if ((resultWriter.Sortable) && (!currentMode.Is_Robot) && (currentMode.Mode != Display_Mode_Enum.My_Sobek))
                {
                    Output.WriteLine("    <br />");
                    Output.WriteLine("    " + sort_by + ": &nbsp;");
                    Output.WriteLine("    <select name=\"sorter_input\" onchange=\"javascript:sort_results('" + url.Replace("&","&amp;") + "')\" id=\"sorter_input\">");
                    Output.WriteLine(sortOptions);
                    Output.WriteLine("    </select>");
                }
                Output.WriteLine("  </span>");
                Output.WriteLine(sortOptions.Length == 0 ? "  <div class=\"ResultsExplanation_NoSort\">" : "  <div class=\"ResultsExplanation\">");
                Output.Write("    ");
                StringBuilder searchInfoBuilder = new StringBuilder();
                StringWriter writer = new StringWriter(searchInfoBuilder);
                Show_Search_Info(writer);
                Output.Write(searchInfoBuilder.ToString());
                summation = searchInfoBuilder.ToString().Replace("<i>", "").Replace("</i>", "").Replace("\"", "").Replace("'", "").Replace("\n", "").Replace("\r", "").Replace("&", "%26").Replace("</td>","");
                Output.WriteLine("  </div>");
                Output.WriteLine("  <br />");
            }
            Output.WriteLine("</div>");
            Output.WriteLine();

            ushort current_page = currentMode.Page;
            if ((currentMode.Result_Display_Type == Result_Display_Type_Enum.Full_Citation) || (currentMode.Result_Display_Type == Result_Display_Type_Enum.Full_Image))
            {
                Output.WriteLine("<a name=\"#image\"></a>");
            }

            if (currentMode.Result_Display_Type == Result_Display_Type_Enum.Export)
            {
                Output.WriteLine("<div class=\"SobekResultsNavBarImbed\">");
                Output.WriteLine("<br />");
                Output.WriteLine("  " + resultsStatistics.Total_Items + "");
                Output.WriteLine("</div>");
            }
            else
            {
                Output.WriteLine("<div class=\"SobekResultsNavBarImbed\">");
                Output.WriteLine("  <br />");
                Output.Write(Buttons);
                Showing_Text = String.Format(showing_range_text, startRow, Math.Min(lastRow, resultsStatistics.Total_Titles), resultWriter.Total_Results);
                if (startRow == lastRow)
                {
                    Showing_Text = Showing_Text.Replace(startRow + " - " + startRow, startRow + " ");
                }

                Output.WriteLine("  " + Showing_Text);
                Output.WriteLine("</div>");
            }

 
            Output.WriteLine();

            // Now add the tabs for the view type
            if (( Include_Bookshelf_View ) || (Hierarchy_Object.Result_Views.Count > 1))
            {
                Output.WriteLine("<div class=\"ResultViewSelectRow\">");
                Result_Display_Type_Enum resultView = currentMode.Result_Display_Type;
                if (Include_Bookshelf_View)
                {
                    if (resultView == Result_Display_Type_Enum.Bookshelf)
                    {
                        Output.WriteLine("  " + Down_Selected_Tab_Start + bookshelf_view + Down_Selected_Tab_End);
                    }
                    else
                    {
                        currentMode.Result_Display_Type = Result_Display_Type_Enum.Bookshelf;
                        Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL().Replace("&","&amp;") + "\">" + Down_Tab_Start + bookshelf_view + Down_Tab_End + "</a>");
                    }
                }

                if ((currentMode.Coordinates.Length > 0) || (Hierarchy_Object.Result_Views.Contains(Result_Display_Type_Enum.Map)))
                {
                    if (resultView == Result_Display_Type_Enum.Map)
                    {
                        Output.WriteLine("  " + Down_Selected_Tab_Start + map_view + Down_Selected_Tab_End);
                    }
                    else
                    {
                        currentMode.Result_Display_Type = Result_Display_Type_Enum.Map;
                        Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL().Replace("&", "&amp;") + "\">" + Down_Tab_Start + map_view + Down_Tab_End + "</a>");
                    }
                }

                if (Hierarchy_Object.Result_Views.Contains(Result_Display_Type_Enum.Brief))
                {
                    if (resultView == Result_Display_Type_Enum.Brief)
                    {
                        Output.WriteLine("  " + Down_Selected_Tab_Start + brief_view + Down_Selected_Tab_End);
                    }
                    else
                    {
                        currentMode.Result_Display_Type = Result_Display_Type_Enum.Brief;
                        Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL().Replace("&", "&amp;") + "\">" + Down_Tab_Start + brief_view + Down_Tab_End + "</a>");
                    }
                }

                if (Hierarchy_Object.Result_Views.Contains(Result_Display_Type_Enum.Table))
                {
                    if (resultView == Result_Display_Type_Enum.Table)
                    {
                        Output.WriteLine("  " + Down_Selected_Tab_Start + table_view + Down_Selected_Tab_End);
                    }
                    else
                    {
                        currentMode.Result_Display_Type = Result_Display_Type_Enum.Table;
                        Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL().Replace("&", "&amp;") + "\">" + Down_Tab_Start + table_view + Down_Tab_End + "</a>");
                    }
                }

                if (Hierarchy_Object.Result_Views.Contains(Result_Display_Type_Enum.Thumbnails))
                {
                    if (resultView == Result_Display_Type_Enum.Thumbnails)
                    {
                        Output.WriteLine("  " + Down_Selected_Tab_Start + thumbnail_view + Down_Selected_Tab_End);
                    }
                    else
                    {
                        currentMode.Result_Display_Type = Result_Display_Type_Enum.Thumbnails;
                        Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL().Replace("&", "&amp;") + "\">" + Down_Tab_Start + thumbnail_view + Down_Tab_End + "</a>");
                    }
                }

                //if (!currentMode.Is_Robot)
                //{
                //    currentMode.Page = 1;
                //    if (hierarchyObject.Result_Views.Contains(SobekCM.Library.Navigation.Result_Display_Type_Enum.Full_Citation))
                //    {
                //        if ((resultView == SobekCM.Library.Navigation.Result_Display_Type_Enum.Full_Citation) || (resultView == SobekCM.Library.Navigation.Result_Display_Type_Enum.Full_Image))
                //        {
                //            Output.WriteLine("  " + base.Down_Selected_Tab_Start + full_view + base.Down_Selected_Tab_End);
                //        }
                //        else
                //        {
                //            currentMode.Result_Display_Type = SobekCM.Library.Navigation.Result_Display_Type_Enum.Full_Image;
                //            Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\">" + base.Down_Tab_Start + full_view + base.Down_Tab_End + "</a>");
                //        }
                //    }


                if (( Include_Bookshelf_View ) && ((resultView == Result_Display_Type_Enum.Export) || (currentMode.Writer_Type == Writer_Type_Enum.HTML_LoggedIn )))
                {
                    currentMode.Page = 1;
                    if (resultView == Result_Display_Type_Enum.Export)
                    {
                        Output.WriteLine("  " + Down_Selected_Tab_Start + exportView + Down_Selected_Tab_End);
                    }
                    else
                    {
                        currentMode.Result_Display_Type = Result_Display_Type_Enum.Export;
                        Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL().Replace("&", "&amp;") + "\">" + Down_Tab_Start + exportView + Down_Tab_End + "</a>");
                    }
                }

                currentMode.Page = current_page;
                currentMode.Result_Display_Type = resultView;
                Output.WriteLine("</div>");
                Output.WriteLine();
            }

            // Determine the number of columns for text areas, depending on browser
            int actual_cols = 50;
            if (currentMode.Browser_Type.ToUpper().IndexOf("FIREFOX") >= 0)
                actual_cols = 45;

            // Add the hidden field
            Output.WriteLine("<!-- Hidden field is used for postbacks to indicate what to save and reset -->");
            if (currentMode.Mode != Display_Mode_Enum.My_Sobek)
            {
                Output.WriteLine("<input type=\"hidden\" id=\"item_action\" name=\"item_action\" value=\"\" />");
            }
            Output.WriteLine("<input type=\"hidden\" id=\"url_description\" name=\"url_description\" value=\"" + HttpUtility.HtmlEncode(summation) + "\" />");
            Output.WriteLine("<input type=\"hidden\" id=\"list_type\" name=\"list_type\" value=\"" + currentName + "\" />");
            Output.WriteLine();

            // Add the scripts needed
            if (currentMode.Mode != Display_Mode_Enum.My_Sobek)
            {
                Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/jquery/jquery-ui-1.10.1.js\"></script>");
                Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_form.js\" ></script>");
                Output.WriteLine();
            }

            #region Email form

            if (currentUser != null)
            {
                Output.WriteLine("<!-- Email form -->");
                Output.WriteLine("<div class=\"email_popup_div\" id=\"form_email\" style=\"display:none;\">");
                Output.WriteLine("  <div class=\"popup_title\"><table width=\"100%\"><tr><td align=\"left\">S<span class=\"smaller\">END THIS</span> " + currentTitle + "<span class=\"smaller\"> TO A</span> F<span class=\"smaller\">RIEND</span></td><td align=\"right\"> <a href=\"#template\" alt=\"CLOSE\" onclick=\"email_form_close()\">X</a> &nbsp; </td></tr></table></div>");
                Output.WriteLine("  <br />");
                Output.WriteLine("  <fieldset><legend>Enter the email information below &nbsp; </legend>");
                Output.WriteLine("    <br />");
                Output.WriteLine("    <table class=\"popup_table\">");


                // Add email address line
                Output.Write("      <tr align=\"left\"><td width=\"80px\"><label for=\"email_address\">To:</label></td>");
                Output.WriteLine("<td><input class=\"email_input\" name=\"email_address\" id=\"email_address\" type=\"text\" value=\"" + currentUser.Email + "\" onfocus=\"javascript:textbox_enter('email_address', 'email_input_focused')\" onblur=\"javascript:textbox_leave('email_address', 'email_input')\" /></td></tr>");

                // Add comments area
                Output.Write("      <tr align=\"left\" valign=\"top\"><td><br /><label for=\"email_comments\">Comments:</label></td>");
                Output.WriteLine("<td><textarea rows=\"6\" cols=\"" + actual_cols + "\" name=\"email_comments\" id=\"email_comments\" class=\"email_textarea\" onfocus=\"javascript:textbox_enter('email_comments','email_textarea_focused')\" onblur=\"javascript:textbox_leave('email_comments','email_textarea')\"></textarea></td></tr>");

                // Add format area
                Output.Write("      <tr align=\"left\" valign=\"top\"><td>Format:</td>");
                Output.Write("<td><input type=\"radio\" name=\"email_format\" id=\"email_format_html\" value=\"html\" checked=\"checked\" /> <label for=\"email_format_html\">HTML</label> &nbsp; &nbsp; ");
                Output.WriteLine("<input type=\"radio\" name=\"email_format\" id=\"email_format_text\" value=\"text\" /> <label for=\"email_format_text\">Plain Text</label></td></tr>");


                Output.WriteLine("    </table>");
                Output.WriteLine("    <br />");
                Output.WriteLine("  </fieldset><br />");
                Output.WriteLine("  <center><a href=\"\" onclick=\"return email_form_close();\"><img border=\"0\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/cancel_button_g.gif\" alt=\"CLOSE\" /></a> &nbsp; &nbsp; <input type=\"image\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/send_button_g.gif\" value=\"Submit\" alt=\"Submit\"></center><br />");
                Output.WriteLine("</div>");
                Output.WriteLine();
            }

            #endregion

            if (currentMode.Mode != Display_Mode_Enum.My_Sobek)
            {

                #region Save search/browse form

                if (currentUser != null)
                {
                    Output.WriteLine("<!-- Save search/browse -->");
                    Output.WriteLine("<div class=\"add_popup_div\" id=\"add_item_form\" style=\"display:none;\">");
                    Output.WriteLine("  <div class=\"popup_title\"><table width=\"100%\"><tr><td align=\"left\">A<span class=\"smaller\">DD THIS</span> " + currentTitle + "<span class=\"smaller\"> TO YOUR</span> S<span class=\"smaller\">AVED</span> S<span class=\"smaller\">EARCHES</span></td><td align=\"right\"> <a href=\"#template\" alt=\"CLOSE\" onclick=\"add_item_form_close()\">X</a> &nbsp; </td></tr></table></div>");
                    Output.WriteLine("  <br />");
                    Output.WriteLine("  <fieldset><legend>Enter notes for this " + currentName + " &nbsp; </legend>");
                    Output.WriteLine("    <br />");
                    Output.WriteLine("    <table class=\"popup_table\">");

                    // Add comments area
                    Output.Write("      <tr align=\"left\" valign=\"top\"><td><br /><label for=\"add_notes\">Description:</label></td>");
                    Output.WriteLine("<td><textarea rows=\"8\" cols=\"" + actual_cols + "\" name=\"add_notes\" id=\"add_notes\" class=\"add_notes_textarea\" onfocus=\"javascript:textbox_enter('add_notes','add_notes_textarea_focused')\" onblur=\"javascript:textbox_leave('add_notes','add_notes_textarea')\">" + summation + "</textarea></td></tr>");
                    Output.WriteLine("      <tr align=\"left\" valign=\"top\"><td>&nbsp;</td><td><input type=\"checkbox\" id=\"open_searches\" name=\"open_searches\" value=\"open\" /> <label for=\"open_searches\">Open saved searches in new window</label></td></tr>");
                    Output.WriteLine("    </table>");
                    Output.WriteLine("    <br />");
                    Output.WriteLine("  </fieldset><br />");
                    Output.WriteLine("  <center><a href=\"\" onclick=\"return add_item_form_close();\"><img border=\"0\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/cancel_button_g.gif\" alt=\"CLOSE\" /></a> &nbsp; &nbsp; <input type=\"image\" src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/save_button_g.gif\" value=\"Submit\" alt=\"Submit\"></center><br />");
                    Output.WriteLine("</div>");
                    Output.WriteLine();
                }

                #endregion

                #region Share form

                // Calculate the title and url
                string title = HttpUtility.HtmlEncode(summation);
                string share_url = HttpContext.Current.Items["Original_URL"].ToString().Replace("&", "%26").Replace("?", "%3F").Replace("http://", "").Replace("=", "%3D").Replace("\"","&quot;");

                Output.WriteLine("<!-- Share form -->");
                Output.WriteLine("<div class=\"share_popup_div\" id=\"share_form\" style=\"display:none;\">");

                Output.WriteLine("<a href=\"http://www.facebook.com/share.php?u=" + share_url + "&amp;t=" + title + "\" target=\"FACEBOOK_WINDOW\" onmouseover=\"facebook_share.src='" + currentMode.Base_URL + "default/images/facebook_share_h.gif'\" onmouseout=\"facebook_share.src='" + currentMode.Base_URL + "default/images/facebook_share.gif'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"facebook_share\" name=\"facebook_share\" src=\"" + currentMode.Base_URL + "default/images/facebook_share.gif\" alt=\"FACEBOOK\" /></a>");
                Output.WriteLine("<a href=\"http://buzz.yahoo.com/buzz?targetUrl=" + share_url + "&amp;headline=" + title + "\" target=\"YAHOOBUZZ_WINDOW\" onmouseover=\"yahoobuzz_share.src='" + currentMode.Base_URL + "default/images/yahoobuzz_share_h.gif'\" onmouseout=\"yahoobuzz_share.src='" + currentMode.Base_URL + "default/images/yahoobuzz_share.gif'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"yahoobuzz_share\" name=\"yahoobuzz_share\" src=\"" + currentMode.Base_URL + "default/images/yahoobuzz_share.gif\" alt=\"YAHOO BUZZ\" /></a>");
                Output.WriteLine("<br />");

                Output.WriteLine("<a href=\"http://twitter.com/home?status=Currently reading " + share_url + "\" target=\"TWITTER_WINDOW\" onmouseover=\"twitter_share.src='" + currentMode.Base_URL + "default/images/twitter_share_h.gif'\" onmouseout=\"twitter_share.src='" + currentMode.Base_URL + "default/images/twitter_share.gif'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"twitter_share\" name=\"twitter_share\" src=\"" + currentMode.Base_URL + "default/images/twitter_share.gif\" alt=\"TWITTER\" /></a>");
                Output.WriteLine("<a href=\"http://www.google.com/bookmarks/mark?op=add&amp;bkmk=" + share_url + "&amp;title=" + title + "\" target=\"GOOGLE_WINDOW\" onmouseover=\"google_share.src='" + currentMode.Base_URL + "default/images/google_share_h.gif'\" onmouseout=\"google_share.src='" + currentMode.Base_URL + "default/images/google_share.gif'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"google_share\" name=\"google_share\" src=\"" + currentMode.Base_URL + "default/images/google_share.gif\" alt=\"GOOGLE SHARE\" /></a>");
                Output.WriteLine("<br />");

                Output.WriteLine("<a href=\"http://www.stumbleupon.com/submit?url=" + share_url + "&amp;title=" + title + "\" target=\"STUMBLEUPON_WINDOW\" onmouseover=\"stumbleupon_share.src='" + currentMode.Base_URL + "default/images/stumbleupon_share_h.gif'\" onmouseout=\"stumbleupon_share.src='" + currentMode.Base_URL + "default/images/stumbleupon_share.gif'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"stumbleupon_share\" name=\"stumbleupon_share\" src=\"" + currentMode.Base_URL + "default/images/stumbleupon_share.gif\" alt=\"STUMBLEUPON\" /></a>");
                Output.WriteLine("<a href=\"http://myweb.yahoo.com/myresults/bookmarklet?t=" + title + "&amp;u=" + share_url + "\" target=\"YAHOO_WINDOW\" onmouseover=\"yahoo_share.src='" + currentMode.Base_URL + "default/images/yahoo_share_h.gif'\" onmouseout=\"yahoo_share.src='" + currentMode.Base_URL + "default/images/yahoo_share.gif'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"yahoo_share\" name=\"yahoo_share\" src=\"" + currentMode.Base_URL + "default/images/yahoo_share.gif\" alt=\"YAHOO SHARE\" /></a>");
                Output.WriteLine("<br />");

                Output.WriteLine("<a href=\"http://digg.com/submit?phase=2&amp;url=" + share_url + "&amp;title=" + title + "\" target=\"DIGG_WINDOW\" onmouseover=\"digg_share.src='" + currentMode.Base_URL + "default/images/digg_share_h.gif'\" onmouseout=\"digg_share.src='" + currentMode.Base_URL + "default/images/digg_share.gif'\" onclick=\"\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"digg_share\" name=\"digg_share\" src=\"" + currentMode.Base_URL + "default/images/digg_share.gif\" alt=\"DIGG\" /></a>");
                Output.WriteLine("<a onmouseover=\"favorites_share.src='" + currentMode.Base_URL + "default/images/favorites_share_h.gif'\" onmouseout=\"favorites_share.src='" + currentMode.Base_URL + "default/images/favorites_share.gif'\" onclick=\"javascript:add_to_favorites();\"><img class=\"ResultSavePrintButtons\" border=\"0px\" id=\"favorites_share\" name=\"favorites_share\" src=\"" + currentMode.Base_URL + "default/images/favorites_share.gif\" alt=\"MY FAVORITES\" /></a>");
                Output.WriteLine("<br />");

                Output.WriteLine("</div>");
                Output.WriteLine();

                #endregion

            }

            if (Outer_Form_Name.Length == 0)
                Output.WriteLine("</form>");

            return true;
        }

        /// <summary> Renders the text about this search (i.e., &quot;Your search for ... resulted in ...&quot; )
        /// directly to the output stream </summary>
        /// <param name="Output"> Stream to which to write the HTML for this subwriter </param>
        protected void Show_Search_Info(TextWriter Output)
        {
            string and_language = "and ";
            string or_language = "or ";
            string and_not_language = "not ";
            string no_matches_language = "resulted in no matching records.";
            string one_match_language = "resulted in one matching record.";
            string multiple_records_language = "resulted in {0} matching records.";
            string one_item_language = "resulted in one item in ";
            string multiple_items_language = "resulted in {0} items in ";
            string one_title_language = "one title.";
            string multiple_titles_language = " titles.";

            // Set special language for aerials
            if (currentMode.Aggregation == "aerials")
            {
                no_matches_language = "resulted in no matching flights.";
                one_match_language = "resulted in one matching flight.";
                multiple_records_language = "resulted in {0} matching flights.";
                one_item_language = "resulted in one flight in ";
                multiple_items_language = "resulted in {0} flights in ";
                one_title_language = "one county.";
                multiple_titles_language = " counties.";
            }
            
            switch (currentMode.Language)
            {
                case Web_Language_Enum.French:
                    Output.Write("Votre recherche de <i>" + Hierarchy_Object.Name + "</i> en ");
                    and_language = "et ";
                    or_language = "ou ";
                    and_not_language = "non ";

                    no_matches_language = "aucun des documents correspondants.";
                    one_match_language = ", correpsonde à 1 document.";
                    multiple_records_language = ", correpsonde à {0} documents";
                    one_item_language = ", correpsonde à 1 document en ";
                    multiple_items_language = ", correpsonde à {0} documents en ";
                    one_title_language = "1 titre.";
                    multiple_titles_language = " titres.";
                    break;

                case Web_Language_Enum.Spanish:
                    Output.Write("Su búsqueda de <i>" + Hierarchy_Object.Name + "</i> en ");
                    and_language = "y ";
                    or_language = "o ";
                    and_not_language = "no ";

                    no_matches_language = "no dio lugar a los objetos.";
                    one_match_language = ", resultó en 1 objeto.";
                    multiple_records_language = ", resultó en {0} objetos.";
                    one_item_language = ", resultó en 1 objeto en ";
                    multiple_items_language = ", resultó en {0} objetos en ";
                    one_title_language = "1 título.";
                    multiple_titles_language = " títulos.";
                    break;

                default:
                    if (currentMode.Search_Type == Search_Type_Enum.Map)
                        Output.Write("Your geographic search of <i>" + Hierarchy_Object.Name + "</i> ");
                    else
                        Output.Write("Your search of <i>" + Hierarchy_Object.Name + "</i> for ");
                    break;
            }

            // Split the parts
            if (currentMode.Search_Type != Search_Type_Enum.Map)
            {
                int length_of_explanation = 0;
                List<string> terms = new List<string>();
                List<string> fields = new List<string>();

                // Split the terms correctly
                SobekCM_Assistant.Split_Clean_Search_Terms_Fields(currentMode.Search_String, currentMode.Search_Fields, currentMode.Search_Type, terms, fields, SobekCM_Library_Settings.Search_Stop_Words, currentMode.Search_Precision, ',');

                try
                {
                    for (int i = 0; (i < terms.Count) && (i < fields.Count); i++)
                    {
                        if ((terms[i].Length > 0) && (fields[i].Length > 0))
                        {
                            // Remove the leading + sign
                            if (fields[i][0] == '+')
                                fields[i] = fields[i].Substring(1);
                            if (fields[i][0] == ' ')
                                fields[i] = fields[i].Substring(1);

                            // Add the 'AND' value
                            if (i > 0)
                            {
                                if (fields[i][0] == '=')
                                {
                                    Output.Write(or_language);
                                    length_of_explanation += or_language.Length;
                                    fields[i] = fields[i].Substring(1);
                                }
                                else
                                {
                                    Output.Write(and_language);
                                    length_of_explanation += and_language.Length;
                                }
                            }

                            // This explanataion need to be capped
                            if (length_of_explanation >= 160)
                            {
                                Output.Write("... ");
                                break;
                            }

                            // Add the term
                            if (terms[i].Contains(" "))
                            {
                                Output.Write("\"" + terms[i].Replace("''''", "'").Replace("''", "'") + "\" ");
                                length_of_explanation += terms[i].Length + 1;
                            }
                            else
                            {
                                Output.Write("'" + terms[i].Replace("''''","'").Replace("''","'") + "' ");
                                length_of_explanation += terms[i].Length + 3;
                            }

                            // Does the field start with a negative?
                            if (fields[i][0] == '-')
                            {
                                Output.Write(and_not_language);
                                length_of_explanation += and_not_language.Length;
                                fields[i] = fields[i].Substring(1);
                            }

                            string write_value = Search_Label_from_UFDC_Code(fields[i]).ToLower() + " ";
                            Output.Write(write_value);
                            length_of_explanation += write_value.Length;
                        }
                    }
                }
                catch
                {
                    Output.Write("UNRECOGNIZED SEARCH ");
                }
            }

            if ((resultsStatistics == null) || (resultsStatistics.Total_Titles == 0))
            {
                Output.WriteLine(no_matches_language );
            }
            else
            {
                if (resultsStatistics.Total_Titles == resultsStatistics.Total_Items)
                {
                    Output.WriteLine(resultsStatistics.Total_Titles == 1 ? one_match_language : String.Format(multiple_records_language, resultsStatistics.Total_Titles));
                }
                else
                {
                    Output.Write(resultsStatistics.Total_Items == 1 ? one_item_language : String.Format(multiple_items_language, resultsStatistics.Total_Items.ToString()));

                    if (resultsStatistics.Total_Titles == 1)
                    {
                        Output.WriteLine(one_title_language);
                    }
                    else
                    {
                        Output.WriteLine(resultsStatistics.Total_Titles + multiple_titles_language);
                    }
                }
            }
        }

        private string Search_Label_from_UFDC_Code(string code)
        {
            string in_language = "in ";
            if (currentMode.Language == Web_Language_Enum.French)
            {
                in_language = "en ";
            }
            if (currentMode.Language == Web_Language_Enum.Spanish)
            {
                in_language = "en ";
            }

            if (code == "ZZ")
                return translations.Get_Translation("anywhere", currentMode.Language);

            Metadata_Search_Field field = SobekCM_Library_Settings.Metadata_Search_Field_By_Code(code);
            return (field != null) ? in_language + translations.Get_Translation(field.Display_Term, currentMode.Language) : in_language + code;
        }

        #region Methods to create the facets on the left side of the results

        /// <summary> Returns the facets for this result/browse as HTML to be added into the form </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Formatted facet information in HTML table format </returns>
        protected string Add_Facet_Information(Custom_Tracer Tracer)
        {

            string collection = translations.Get_Translation("Collection", currentMode.Language );
            string show_more = translations.Get_Translation("Show More", currentMode.Language); 
            string show_less = translations.Get_Translation("Show Less", currentMode.Language); 
            string sort_by_frequency = translations.Get_Translation("Sort these facets by frequency", currentMode.Language);
            string sort_alphabetically = translations.Get_Translation("Sort these facets alphabetically", currentMode.Language);

            StringBuilder builder = new StringBuilder();

            builder.AppendLine("<input type=\"hidden\" id=\"facet\" name=\"facet\" value=\"" + HttpUtility.HtmlEncode(facetInformation) + "\" />");

            builder.AppendLine("<script type=\"text/javascript\">");
            builder.AppendLine("  //<![CDATA[");
            builder.AppendLine("    function add_facet(code, new_value) {");

            string url = String.Empty;
            string aggregation_url = String.Empty;

            if (currentMode.Mode == Display_Mode_Enum.Aggregation_Browse_Info) 
            {
                Display_Mode_Enum displayMode = currentMode.Mode;
                currentMode.Mode = Display_Mode_Enum.Results;
                currentMode.Search_Type = Search_Type_Enum.Advanced;
                currentMode.Search_Fields = "<%CODE%>";
                currentMode.Search_String = "<%VALUE%>";
                ushort page = currentMode.Page;
                currentMode.Page = 1;
                url = currentMode.Redirect_URL().Replace("%3c%25", "<%").Replace("%25%3e", "%>").Replace("<%VALUE%>","\"<%VALUE%>\"");
                currentMode.Mode = displayMode;
                currentMode.Page = page;
                currentMode.Search_Fields = String.Empty;
                currentMode.Search_String = String.Empty;

                if ((currentMode.Aggregation.Length == 0) || (currentMode.Aggregation == "all"))
                {
                    currentMode.Aggregation = "<%AGGREGATION%>";
                    aggregation_url = currentMode.Redirect_URL();
                    currentMode.Aggregation = String.Empty;
                }
            }
            else
            {
                if ((currentMode.Aggregation.Length == 0) || ( currentMode.Aggregation == "all" ))
                {
                    currentMode.Aggregation = "<%AGGREGATION%>";
                    aggregation_url = currentMode.Redirect_URL();
                    currentMode.Aggregation = String.Empty;
                }

                if (currentMode.Search_Type == Search_Type_Enum.Advanced)
                {
                    string orig_field = currentMode.Search_Fields;
                    string orig_terms = currentMode.Search_String;
                    currentMode.Search_Fields = currentMode.Search_Fields + ",<%CODE%>";
                    currentMode.Search_String = currentMode.Search_String + ",<%VALUE%>";
                    ushort page = currentMode.Page;
                    currentMode.Page = 1;
                    url = currentMode.Redirect_URL().Replace("%3c%25", "<%").Replace("%25%3e", "%>").Replace("<%VALUE%>", "\"<%VALUE%>\"");
                    currentMode.Page = page;
                    currentMode.Search_Fields = orig_field;
                    currentMode.Search_String = orig_terms;
                }
                if ( currentMode.Search_Type == Search_Type_Enum.Basic )
                {
                    List<string> output_terms = new List<string>();
                    List<string> output_fields = new List<string>();
                    SobekCM_Assistant.Split_Clean_Search_Terms_Fields(currentMode.Search_String, currentMode.Search_Fields, currentMode.Search_Type, output_terms, output_fields, SobekCM_Library_Settings.Search_Stop_Words, currentMode.Search_Precision, ',');

                    string original_search = currentMode.Search_String;
                    currentMode.Search_Type = Search_Type_Enum.Advanced;
                    StringBuilder term_builder = new StringBuilder();
                    foreach (string thisTerm in output_terms)
                    {
                        if (term_builder.Length > 0)
                            term_builder.Append(",");
                        term_builder.Append(thisTerm);
                    }
                    StringBuilder field_builder = new StringBuilder();
                    foreach (string thisField in output_fields)
                    {
                        if (field_builder.Length > 0)
                            field_builder.Append(",");
                        field_builder.Append(thisField);
                    }
                    currentMode.Search_Fields = field_builder.ToString();
                    currentMode.Search_String = term_builder.ToString();

                    currentMode.Search_Fields = currentMode.Search_Fields + ",<%CODE%>";
                    currentMode.Search_String = currentMode.Search_String + ",<%VALUE%>";
                    url = currentMode.Redirect_URL().Replace("%3c%25", "<%").Replace("%25%3e", "%>").Replace("<%VALUE%>", "\"<%VALUE%>\""); 

                    currentMode.Search_Type = Search_Type_Enum.Basic;
                    currentMode.Search_Fields = String.Empty;
                    currentMode.Search_String = original_search;

                }
            }
            builder.AppendLine("      var stem_url = '" + url + "';");
            builder.AppendLine("      var new_url = stem_url.replace('<%CODE%>', code).replace('<%VALUE%>', new_value);");
            builder.AppendLine("      window.location.href = new_url;");
            builder.AppendLine("      return false;");
            builder.AppendLine("    }");
            builder.AppendLine("  //]]>");
            builder.AppendLine("</script>");
            builder.AppendLine();
           
            builder.AppendLine("<div class=\"SobekFacetColumn\">");
            builder.AppendLine("<div class=\"SobekFacetColumnTitle\">" + translations.Get_Translation("NARROW RESULTS BY", currentMode.Language ) + ":</div>");


            // Add the aggregation information first
            if ((( currentMode.Aggregation.Length == 0 ) || ( currentMode.Aggregation == "all")) && (resultsStatistics.Aggregation_Facets_Count > 0))
            {
                string title = collection;
                const int facetIndex = 0;
                int facet_count = 0;
                int total_facets_to_show = MINIMIZED_FACET_COUNT;
                char other_sort_type = '2';
                char other_show_type = '1';
                if ((facetInformation[facetIndex] == '1') || (facetInformation[facetIndex] == '3'))
                {
                    total_facets_to_show = MAXIMIZED_FACET_COUNT;
                }

                string resort_image = "2_to_1.gif";
                string sort_instructions = sort_by_frequency;
                switch (facetInformation[facetIndex])
                {
                    case '0':
                        other_sort_type = '2';
                        other_show_type = '1';
                        resort_image = "a_to_z.gif";
                        sort_instructions = sort_alphabetically;
                        break;

                    case '1':
                        other_sort_type = '3';
                        other_show_type = '0';
                        resort_image = "a_to_z.gif";
                        sort_instructions = sort_alphabetically;
                        break;

                    case '2':
                        other_sort_type = '0';
                        other_show_type = '3';
                        break;

                    case '3':
                        other_sort_type = '1';
                        other_show_type = '2';
                        break;
                }

                builder.AppendLine("<br /><span style=\"float:right; padding-right: 3px\"><a href=\"\" onclick=\"return set_facet(" + facetIndex + ",'" + other_sort_type + "');\" title=\"" + sort_instructions + "\"><img src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/" + resort_image + "\" /></a></span>");
                builder.AppendLine("<b> &nbsp;" + title + "</b><br />");
                builder.AppendLine("<div class=\"SobekFacetBox\">");
                if ((facetInformation[facetIndex] == '2') || (facetInformation[facetIndex] == '3'))
                {
                    SortedList<string, string> order_facets = new SortedList<string, string>();
                    while ((facet_count < total_facets_to_show) && (facet_count < resultsStatistics.Aggregation_Facets.Count))
                    {
                        if (resultsStatistics.Aggregation_Facets[facet_count].Code.ToLower() != "iuf")
                        {
                            order_facets[resultsStatistics.Aggregation_Facets[facet_count].Facet.ToUpper()] = "<a href=\"" + aggregation_url.Replace("<%AGGREGATION%>", resultsStatistics.Aggregation_Facets[facet_count].Code.ToLower()) + "\">" + resultsStatistics.Aggregation_Facets[facet_count].Facet + "</a> ( " + resultsStatistics.Aggregation_Facets[facet_count].Frequency + " ) <br />";
                        }
                        facet_count++;
                    }
                    foreach (string html in order_facets.Values)
                    {
                        builder.AppendLine(html);
                    }
                }
                else
                {
                    while ((facet_count < total_facets_to_show) && (facet_count < resultsStatistics.Aggregation_Facets.Count))
                    {
                        if (resultsStatistics.Aggregation_Facets[facet_count].Code.ToLower() != "iuf")
                        {
                            builder.AppendLine("<a href=\"" + aggregation_url.Replace("<%AGGREGATION%>", resultsStatistics.Aggregation_Facets[facet_count].Code.ToLower()) + "\">" + resultsStatistics.Aggregation_Facets[facet_count].Facet + "</a> ( " + resultsStatistics.Aggregation_Facets[facet_count].Frequency + " ) <br />");
                        }
                        facet_count++;
                    }
                }
                if (facet_count > MINIMIZED_FACET_COUNT)
                {
                    builder.AppendLine("<div class=\"SobekShowHideFacets\"><a href=\"\" onclick=\"return set_facet(" + facetIndex + ",'" + other_show_type + "');\">&lt;&lt; " + show_less + " &nbsp; &nbsp;</a></div>");
                }
                else
                {
                    if (facet_count < resultsStatistics.Aggregation_Facets.Count)
                    {
                        builder.AppendLine("<div class=\"SobekShowHideFacets\"><a href=\"\" onclick=\"return set_facet(" + facetIndex + ",'" + other_show_type + "');\">" + show_more + " &gt;&gt; &nbsp;</a></div>");
                    }
                }
                builder.AppendLine("</div>");
            }

            // Add the first facet information 
            if (resultsStatistics.First_Facets_Count > 0)
            {
                Metadata_Search_Field field = SobekCM_Library_Settings.Metadata_Search_Field_By_ID(resultsStatistics.First_Facets_MetadataTypeID);
                if (field != null)
                {
                    Add_Single_Facet(builder,  translations.Get_Translation(field.Facet_Term, currentMode.Language ), field.Web_Code, show_less, show_more, 1, sort_by_frequency, sort_alphabetically, resultsStatistics.First_Facets);
                }
            }

            // Add the second facet information 
            if (resultsStatistics.Second_Facets_Count > 0)
            {
                Metadata_Search_Field field = SobekCM_Library_Settings.Metadata_Search_Field_By_ID(resultsStatistics.Second_Facets_MetadataTypeID);
                if (field != null)
                {
                    Add_Single_Facet(builder, translations.Get_Translation(field.Facet_Term, currentMode.Language), field.Web_Code, show_less, show_more, 2, sort_by_frequency, sort_alphabetically, resultsStatistics.Second_Facets);
                }
            }

            // Add the third facet information 
            if (resultsStatistics.Third_Facets_Count > 0)
            {
                Metadata_Search_Field field = SobekCM_Library_Settings.Metadata_Search_Field_By_ID(resultsStatistics.Third_Facets_MetadataTypeID);
                if (field != null)
                {
                    Add_Single_Facet(builder, translations.Get_Translation(field.Facet_Term, currentMode.Language), field.Web_Code, show_less, show_more, 3, sort_by_frequency, sort_alphabetically, resultsStatistics.Third_Facets);
                }
            }

            // Add the fourth facet information 
            if (resultsStatistics.Fourth_Facets_Count > 0)
            {
                Metadata_Search_Field field = SobekCM_Library_Settings.Metadata_Search_Field_By_ID(resultsStatistics.Fourth_Facets_MetadataTypeID);
                if (field != null)
                {
                    Add_Single_Facet(builder, translations.Get_Translation(field.Facet_Term, currentMode.Language), field.Web_Code, show_less, show_more, 4, sort_by_frequency, sort_alphabetically, resultsStatistics.Fourth_Facets);
                }
            }

            // Add the fifth facet information 
            if (resultsStatistics.Fifth_Facets_Count > 0)
            {
                Metadata_Search_Field field = SobekCM_Library_Settings.Metadata_Search_Field_By_ID(resultsStatistics.Fifth_Facets_MetadataTypeID);
                if (field != null)
                {
                    Add_Single_Facet(builder, translations.Get_Translation(field.Facet_Term, currentMode.Language), field.Web_Code, show_less, show_more, 5, sort_by_frequency, sort_alphabetically, resultsStatistics.Fifth_Facets);
                }
            }

            // Add the sixth facet information 
            if (resultsStatistics.Sixth_Facets_Count > 0)
            {
                Metadata_Search_Field field = SobekCM_Library_Settings.Metadata_Search_Field_By_ID(resultsStatistics.Sixth_Facets_MetadataTypeID);
                if (field != null)
                {
                    Add_Single_Facet(builder, translations.Get_Translation(field.Facet_Term, currentMode.Language), field.Web_Code, show_less, show_more, 6, sort_by_frequency, sort_alphabetically, resultsStatistics.Sixth_Facets);
                }
            }

            // Add the seventh facet information 
            if (resultsStatistics.Seventh_Facets_Count > 0)
            {
                Metadata_Search_Field field = SobekCM_Library_Settings.Metadata_Search_Field_By_ID(resultsStatistics.Seventh_Facets_MetadataTypeID);
                if (field != null)
                {
                    Add_Single_Facet(builder, translations.Get_Translation(field.Facet_Term, currentMode.Language), field.Web_Code, show_less, show_more, 7, sort_by_frequency, sort_alphabetically, resultsStatistics.Seventh_Facets);
                }
            }

            // Add the eighth facet information 
            if (resultsStatistics.Eighth_Facets_Count > 0)
            {
                Metadata_Search_Field field = SobekCM_Library_Settings.Metadata_Search_Field_By_ID(resultsStatistics.Eighth_Facets_MetadataTypeID);
                if (field != null)
                {
                    Add_Single_Facet(builder, translations.Get_Translation(field.Facet_Term, currentMode.Language), field.Web_Code, show_less, show_more, 8, sort_by_frequency, sort_alphabetically, resultsStatistics.Eighth_Facets);
                }
            }

            builder.AppendLine("</div>");
            return builder.ToString();
        }


        private void Add_Single_Facet(StringBuilder builder, string title, string search_code, string show_less, string show_more, int facet_index, string sort_by_frequency, string sort_alphabetically, List<Search_Facet> collection)
        {
            int facet_count = 0;
            int total_facets_to_show = MINIMIZED_FACET_COUNT;
            char other_sort_type = '2';
            char other_show_type = '1';
            if ((facetInformation[facet_index - 1] == '1') || ( facetInformation[facet_index - 1 ] == '3' ))
            {
                total_facets_to_show = MAXIMIZED_FACET_COUNT;
            }

            string resort_image = "2_to_1.gif";
            string sort_instructions = sort_by_frequency;
            switch ( facetInformation[facet_index - 1])
            {
                case '0':
                    other_sort_type = '2';
                    other_show_type = '1';
                    resort_image = "a_to_z.gif";
                    sort_instructions = sort_alphabetically;
                    break;

                case '1':
                    other_sort_type = '3';
                    other_show_type = '0';
                    resort_image = "a_to_z.gif";
                    sort_instructions = sort_alphabetically;
                    break;

                case '2':
                    other_sort_type = '0';
                    other_show_type = '3';
                    break;

                case '3':
                    other_sort_type = '1';
                    other_show_type = '2';
                    break;
            }

            builder.AppendLine("<br /><span style=\"float:right; padding-right: 3px\"><a href=\"\" onclick=\"return set_facet(" + (facet_index - 1) + ",'" + other_sort_type + "');\" title=\"" + sort_instructions + "\"><img src=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/buttons/" + resort_image + "\" alt=\"RESORT\" /></a></span>");
            builder.AppendLine("<b> &nbsp;" + title + "</b><br />");
            builder.AppendLine("<div class=\"SobekFacetBox\">");
            if ((facetInformation[facet_index - 1] == '2') || (facetInformation[facet_index - 1] == '3'))
            {
                SortedList<string, string> order_facets = new SortedList<string, string>();
                while ((facet_count < total_facets_to_show) && (facet_count < collection.Count))
                {
                    order_facets[collection[facet_count].Facet.ToUpper()] = "<a href=\"\" onclick=\"return add_facet('" + search_code + "','" + HttpUtility.HtmlEncode(collection[facet_count].Facet.Replace("&", "")).Replace("'", "\\'").Replace(",", "").Replace("&", "") + "');\">" + collection[facet_count].Facet.Replace("&", "&amp;") + "</a> ( " + collection[facet_count].Frequency + " ) <br />";
                    facet_count++;
                }
                foreach (string html in order_facets.Values)
                {
                    builder.AppendLine(html);
                }
            }
            else
            {
                while ((facet_count < total_facets_to_show) && (facet_count < collection.Count))
                {
                    builder.AppendLine("<a href=\"\" onclick=\"return add_facet('" + search_code + "','" + HttpUtility.HtmlEncode(collection[facet_count].Facet.Replace("&", "")).Replace("'", "\\'").Replace(",", "").Replace("&", "") + "');\">" + collection[facet_count].Facet.Replace("&", "&amp;" ) + "</a> ( " + collection[facet_count].Frequency + " ) <br />");
                    facet_count++;
                }
            }
            if (facet_count > MINIMIZED_FACET_COUNT)
            {
                builder.AppendLine("<div class=\"SobekShowHideFacets\"><a href=\"\" onclick=\"return set_facet(" + (facet_index - 1) + ",'" + other_show_type + "');\">&lt;&lt; " + show_less + " &nbsp; &nbsp;</a></div>");
            }
            else
            {
                if (facet_count < collection.Count)
                {
                    builder.AppendLine("<div class=\"SobekShowHideFacets\"><a href=\"\" onclick=\"return set_facet(" + ( facet_index - 1 ) + ",'" + other_show_type + "');\">" + show_more + " &gt;&gt; &nbsp;</a></div>");
                }
            }
            builder.AppendLine("</div>");
        }
        #endregion
    }
}
