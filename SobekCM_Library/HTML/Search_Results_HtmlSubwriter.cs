#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI.WebControls;
using SobekCM.Library.Application_State;
using SobekCM.Library.Configuration;
using SobekCM.Library.Navigation;
using SobekCM.Library.Results;
using SobekCM.Library.Users;

#endregion

namespace SobekCM.Library.HTML
{
    /// <summary> Search results html subwriter renders a browse of search results  </summary>
    /// <remarks> This class extends the <see cref="abstractHtmlSubwriter"/> abstract class. </remarks>
    public class Search_Results_HtmlSubwriter : abstractHtmlSubwriter
    {
        private readonly Item_Lookup_Object allItemsTable;
        private readonly Aggregation_Code_Manager codeManager;
        private readonly User_Object currentUser;
        private readonly List<iSearch_Title_Result> pagedResults;
        private readonly Search_Results_Statistics resultsStatistics;
        private readonly Language_Support_Info translations;
        private PagedResults_HtmlSubwriter writeResult;

        /// <summary> Constructor for a new instance of the Search_Results_HtmlSubwriter class </summary>
        /// <param name="Results_Statistics"> Information about the entire set of results for a search or browse </param>
        /// <param name="Paged_Results"> Single page of results for a search or browse, within the entire set </param>
        /// <param name="Code_Manager"> List of valid collection codes, including mapping from the Sobek collections to Greenstone collections</param>
        /// <param name="Translator"> Language support object which handles simple translational duties </param>
        /// <param name="All_Items_Lookup"> Lookup object used to pull basic information about any item loaded into this library </param>
        /// <param name="Current_User"> Currently logged on user </param>
        public Search_Results_HtmlSubwriter(Search_Results_Statistics Results_Statistics,
            List<iSearch_Title_Result> Paged_Results,
            Aggregation_Code_Manager Code_Manager, Language_Support_Info Translator,
            Item_Lookup_Object All_Items_Lookup, 
            User_Object Current_User)
        {
            currentUser = Current_User;
            pagedResults = Paged_Results;
            resultsStatistics = Results_Statistics;
            translations = Translator;
            codeManager = Code_Manager;
            allItemsTable = All_Items_Lookup;
         }

        /// <summary> Adds controls to the main navigational page </summary>
        /// <param name="placeHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form, widely used throughout the application</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <param name="populate_node_event"> Event is used to populate the a tree node without doing a full refresh of the page </param>
        /// <returns> Sorted tree with the results in hierarchical structure with volumes and issues under the titles </returns>
        /// <remarks> This uses a <see cref="PagedResults_HtmlSubwriter"/> instance to render the items  </remarks>
        public void Add_Controls(PlaceHolder placeHolder, Custom_Tracer Tracer, TreeNodeEventHandler populate_node_event )
        {
            if (resultsStatistics == null) return;

            if (writeResult == null)
            {
                Tracer.Add_Trace("Search_Results_HtmlSubwriter.Add_Controls", "Building Result DataSet Writer");

                writeResult = new PagedResults_HtmlSubwriter(resultsStatistics, pagedResults, codeManager, translations, allItemsTable, currentUser, currentMode, Tracer)
                                  {Hierarchy_Object = Hierarchy_Object, Skin = htmlSkin, Mode = currentMode};
            }

            Tracer.Add_Trace("Search_Results_HtmlSubwriter.Add_Controls", "Add controls");
            writeResult.Add_Controls(placeHolder, Tracer);
        }

        /// <summary> Writes the final output to close this search page results, including the results page navigation buttons </summary>
        /// <param name="Output"> Stream to which to write the HTML for this subwriter </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> TRUE is always returned </returns>
        /// <remarks> This calls the <see cref="PagedResults_HtmlSubwriter.Write_Final_HTML"/> method in the <see cref="PagedResults_HtmlSubwriter"/> object. </remarks>
        public bool Write_Final_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("browse_info_html_subwriter.Write_Final_Html", "Rendering HTML ( finish the main viewer section )");

            if (writeResult != null)
            {
                writeResult.Write_Final_HTML(Output, Tracer);
            }
            return true;
        }

        /// <summary> Writes the HTML generated to browse the results of a search directly to the response stream </summary>
        /// <param name="Output"> Stream to which to write the HTML for this subwriter </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> TRUE -- Value indicating if html writer should finish the page immediately after this, or if there are other controls or routines which need to be called first </returns>
        public override bool Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Search_Results_HtmlSubwriter.Write_HTML", "Rendering HTML");

            // If this skin has top-level navigation suppressed, skip the top tabs
            if (htmlSkin.Suppress_Top_Navigation)
            {
                Output.WriteLine("<br />");
            }
            else
            {
                string refine_search = "MODIFY YOUR SEARCH";
                string new_search = "NEW SEARCH";
                string home_text = "HOME";

                if (currentMode.Language == Web_Language_Enum.French)
                {
                    refine_search = "MODIFIER LA RECHERCHE";
                    new_search = "RELANCEZ LA RECHERCHE";
                    home_text = "PAGE D'ACCUEIL";
                }

                if (currentMode.Language == Web_Language_Enum.Spanish)
                {
                    refine_search = "REDUZCA SU BÚSQUEDA";
                    new_search = "BÚSQUEDA NUEVA";
                    home_text = "INICIO";
                }

                // Add the reference to the script for sorting
                Output.WriteLine("<div class=\"ViewsBrowsesRow\">");

                string fields = currentMode.Search_Fields;
                string search_string = currentMode.Search_String;
                Search_Type_Enum currentSearchType = currentMode.Search_Type;
	            Aggregation_Type_Enum aggrType = currentMode.Aggregation_Type;
                currentMode.Mode = Display_Mode_Enum.Search;
                string refine_redirect_url;
                if ((currentMode.Search_Type == Search_Type_Enum.Basic) || (currentMode.Search_Type == Search_Type_Enum.Newspaper)|| (currentMode.Search_Type == Search_Type_Enum.Advanced))
                {
                    currentMode.Search_Type = Search_Type_Enum.Advanced;
                    refine_redirect_url = currentMode.Redirect_URL();
                    if ( refine_redirect_url.IndexOf("?") > 0 )
                        refine_redirect_url = refine_redirect_url + "&t=" + System.Web.HttpUtility.UrlEncode(currentMode.Search_String).Replace("%2c", ",") + "&f=" + currentMode.Search_Fields;
                    else
                        refine_redirect_url = refine_redirect_url + "?t=" + System.Web.HttpUtility.UrlEncode(currentMode.Search_String).Replace("%2c", ",") + "&f=" + currentMode.Search_Fields;

                    currentMode.Search_Type = currentSearchType;
                }
                else
                {
                    refine_redirect_url = currentMode.Redirect_URL();
                }

                if (currentMode.Search_Type == Search_Type_Enum.Map)
                    currentMode.Search_String = String.Empty;

	            currentMode.Mode = Display_Mode_Enum.Aggregation;
				currentMode.Aggregation_Type = Aggregation_Type_Enum.Home;
                Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\">" + Unselected_Tab_Start + home_text + Unselected_Tab_End + "</a>");
                currentMode.Mode = Display_Mode_Enum.Search;



                Output.WriteLine("  <a href=\"" + refine_redirect_url + "\">" + Unselected_Tab_Start + refine_search + Unselected_Tab_End + "</a>");


                currentMode.Search_String = String.Empty;
                currentMode.Search_Fields = String.Empty;
                if (currentMode.Aggregation == ".all")
                {
                    currentMode.Mode = Display_Mode_Enum.Aggregation;
					currentMode.Aggregation_Type = Aggregation_Type_Enum.Home;
                    currentMode.Aggregation = "";
                    Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\">" + Unselected_Tab_Start + new_search + Unselected_Tab_End + "</a>");
                    currentMode.Aggregation = ".all";
                }
                else
                {
                    currentMode.Mode = Display_Mode_Enum.Aggregation;
					currentMode.Aggregation_Type = Aggregation_Type_Enum.Home;
                    Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\">" + Unselected_Tab_Start + new_search + Unselected_Tab_End + "</a>");
                }
                currentMode.Mode = Display_Mode_Enum.Results;
                currentMode.Search_String = search_string;
                currentMode.Search_Fields = fields;
                Output.WriteLine("</div>");
                Output.WriteLine();
            }
           
            if ( resultsStatistics != null )
            {
                if (writeResult == null)
                {
                    Tracer.Add_Trace("Search_Results_HtmlSubwriter.Write_HTML", "Building Result DataSet Writer");
                    writeResult = new PagedResults_HtmlSubwriter(resultsStatistics, pagedResults, codeManager, translations, allItemsTable, currentUser, currentMode, Tracer)
                                      {Hierarchy_Object = Hierarchy_Object, Skin = htmlSkin, Mode = currentMode};
                }
                writeResult.Write_HTML(Output, Tracer);
            }

            return true;
        }

        /// <summary> Gets the collection of body attributes to be included 
        /// within the HTML body tag (usually to add events to the body) </summary>
        public override List<Tuple<string, string>> Body_Attributes
        {
            get
            {
                if (currentMode.Result_Display_Type == Result_Display_Type_Enum.Map)
                {
                    List<Tuple<string, string>> returnValue = new List<Tuple<string, string>>();

                    returnValue.Add(new Tuple<string, string>("onload", "load();"));

                    return returnValue;
                }
                return null;
            }
        }

        /// <summary> Title for this web page </summary>
        public override string WebPage_Title
        {
            get
            {
                if (Hierarchy_Object != null)
                {
                    return "{0} Search Results - " + Hierarchy_Object.Name;
                }
                else
                {
                    return "{0} Search Results";
                }
            }
        }

        /// <summary> Write any additional values within the HTML Head of the
        /// final served page </summary>
        /// <param name="Output"> Output stream currently within the HTML head tags </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Within_HTML_Head(TextWriter Output, Custom_Tracer Tracer)
        {
            Output.WriteLine("  <meta name=\"robots\" content=\"index, nofollow\" />");
        }
    }
}
