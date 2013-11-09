#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI.WebControls;
using SobekCM.Library.Application_State;
using SobekCM.Library.Configuration;
using SobekCM.Library.Navigation;
using SobekCM.Library.Results;
using SobekCM.Library.Settings;
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
                                  {Current_Aggregation = Current_Aggregation, Skin = htmlSkin, Mode = currentMode};
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
				Output.WriteLine("<!-- Add the results menu -->");
				Output.WriteLine("<div id=\"sf-menubar\">");
				Output.WriteLine("<ul class=\"sf-menu\" id=\"sbkSrm_Menu\">");


                string refine_search = "MODIFY YOUR SEARCH";
                string new_search = "NEW SEARCH";
                string home_text = "HOME";

				// Get ready to draw the tabs
				string home = "Home";
				string collection_home = translations.Get_Translation(Current_Aggregation.ShortName, Mode.Language) + " Home";
				string sobek_home_text = Mode.SobekCM_Instance_Abbreviation + " Home";
				string myCollections = "MY COLLECTIONS";
				const string list_view_text = "List View";
				const string brief_view_text = "Brief View";
				const string tree_view_text = "Tree View";
				const string partners_text = "Browse Partners";

                if (currentMode.Language == Web_Language_Enum.French)
                {
                    refine_search = "MODIFIER LA RECHERCHE";
                    new_search = "RELANCEZ LA RECHERCHE";
                    home_text = "PAGE D'ACCUEIL";
					sobek_home_text = "PAGE D'ACCUEIL";
                }

                if (currentMode.Language == Web_Language_Enum.Spanish)
                {
					home = "INICIO";
					collection_home = "INICIO " + translations.Get_Translation(Current_Aggregation.ShortName, Mode.Language);
					sobek_home_text = "INICIO " + Mode.SobekCM_Instance_Abbreviation.ToUpper();
					myCollections = "MIS COLECCIONES";
                    refine_search = "REDUZCA SU BÚSQUEDA";
                    new_search = "BÚSQUEDA NUEVA";
                    home_text = "INICIO";
                }

				string fields = currentMode.Search_Fields;
				string search_string = currentMode.Search_String;
				Search_Type_Enum currentSearchType = currentMode.Search_Type;
				Aggregation_Type_Enum aggrType = currentMode.Aggregation_Type;
				currentMode.Mode = Display_Mode_Enum.Search;

				// Add the 'SOBEK HOME' first menu option and suboptions
				Mode.Mode = Display_Mode_Enum.Aggregation;
				Mode.Aggregation_Type = Aggregation_Type_Enum.Home;
				Mode.Home_Type = Home_Type_Enum.List;


				// Add the HOME tab
				if (Current_Aggregation.Code == "all")
				{
					// Add the 'SOBEK HOME' first menu option and suboptions
					Mode.Mode = Display_Mode_Enum.Aggregation;
					Mode.Aggregation_Type = Aggregation_Type_Enum.Home;
					Mode.Home_Type = Home_Type_Enum.List;
					Output.WriteLine("\t\t<li id=\"sbkAgm_Home\"><a href=\"" + Mode.Redirect_URL() + "\" class=\"sbkAgm_NoPadding\"><img src=\"" + Mode.Default_Images_URL + "home.png\" /> <div class=\"sbkAgm_HomeText\">" + sobek_home_text + "</div></a><ul id=\"sbkAgm_HomeSubMenu\">");
					Output.WriteLine("\t\t\t<li id=\"sbkAgm_HomeListView\"><a href=\"" + Mode.Redirect_URL() + "\">" + list_view_text + "</a></li>");
					Mode.Home_Type = Home_Type_Enum.Descriptions;
					Output.WriteLine("\t\t\t<li id=\"sbkAgm_HomeBriefView\"><a href=\"" + Mode.Redirect_URL() + "\">" + brief_view_text + "</a></li>");
					if (SobekCM_Library_Settings.Include_TreeView_On_System_Home)
					{
						Mode.Home_Type = Home_Type_Enum.Tree_Collapsed;
						Output.WriteLine("\t\t\t<li id=\"sbkAgm_HomeTreeView\"><a href=\"" + Mode.Redirect_URL() + "\">" + tree_view_text + "</a></li>");
					}
					if ((currentUser != null) && (currentUser.LoggedOn))
					{
						Mode.Home_Type = Home_Type_Enum.Personalized;
						Output.WriteLine("\t\t\t<li id=\"sbkAgm_HomePersonalized\"><a href=\"" + Mode.Redirect_URL() + "\">" + myCollections + "</a></li>");
					}
					if (SobekCM_Library_Settings.Include_Partners_On_System_Home)
					{
						Mode.Home_Type = Home_Type_Enum.Partners_List;
						Output.WriteLine("\t\t\t<li id=\"sbkAgm_HomePartners\"><a href=\"" + Mode.Redirect_URL() + "\">" + partners_text + "</a></li>");
					}
					Output.WriteLine("\t\t</ul></li>");
				}
				else
				{

					// Add the 'SOBEK HOME' first menu option and suboptions
					Mode.Mode = Display_Mode_Enum.Aggregation;
					Mode.Aggregation_Type = Aggregation_Type_Enum.Home;
					Mode.Home_Type = Home_Type_Enum.List;
					Output.WriteLine("\t\t<li id=\"sbkAgm_Home\"><a href=\"" + Mode.Redirect_URL() + "\" class=\"sbkAgm_NoPadding\"><img src=\"" + Mode.Default_Images_URL + "home.png\" /> <div class=\"sbkAgm_HomeText\">" + home + "</div></a><ul id=\"sbkAgm_HomeSubMenu\">");
					Output.WriteLine("\t\t\t<li id=\"sbkAgm_AggrHome\"><a href=\"" + Mode.Redirect_URL() + "\">" + collection_home + "</a></li>");

					Mode.Aggregation = String.Empty;
					Output.WriteLine("\t\t\t<li id=\"sbkAgm_InstanceHome\"><a href=\"" + Mode.Redirect_URL() + "\">" + sobek_home_text + "</a><ul id=\"sbkAgm_InstanceHomeSubMenu\">");
					Output.WriteLine("\t\t\t\t<li id=\"sbkAgm_HomeListView\"><a href=\"" + Mode.Redirect_URL() + "\">" + list_view_text + "</a></li>");
					Mode.Home_Type = Home_Type_Enum.Descriptions;
					Output.WriteLine("\t\t\t\t<li id=\"sbkAgm_HomeBriefView\"><a href=\"" + Mode.Redirect_URL() + "\">" + brief_view_text + "</a></li>");
					if (SobekCM_Library_Settings.Include_TreeView_On_System_Home)
					{
						Mode.Home_Type = Home_Type_Enum.Tree_Collapsed;
						Output.WriteLine("\t\t\t\t<li id=\"sbkAgm_HomeTreeView\"><a href=\"" + Mode.Redirect_URL() + "\">" + tree_view_text + "</a></li>");
					}
					if ((currentUser != null) && (currentUser.LoggedOn))
					{
						Mode.Home_Type = Home_Type_Enum.Personalized;
						Output.WriteLine("\t\t\t\t<li id=\"sbkAgm_HomePersonalized\"><a href=\"" + Mode.Redirect_URL() + "\">" + myCollections + "</a></li>");
					}
					if (SobekCM_Library_Settings.Include_Partners_On_System_Home)
					{
						Mode.Home_Type = Home_Type_Enum.Partners_List;
						Output.WriteLine("\t\t\t\t<li id=\"sbkAgm_HomePartners\"><a href=\"" + Mode.Redirect_URL() + "\">" + partners_text + "</a></li>");
					}
					Output.WriteLine("\t\t\t</ul></li>");
					Output.WriteLine("\t\t</ul></li>");

					Mode.Aggregation = Current_Aggregation.Code;
				}

				// Get the refine URL
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

                currentMode.Mode = Display_Mode_Enum.Search;

                Output.WriteLine("\t\t<li><a href=\"" + refine_redirect_url + "\">" + refine_search + "</a></li>");


                currentMode.Search_String = String.Empty;
                currentMode.Search_Fields = String.Empty;
                if (currentMode.Aggregation == ".all")
                {
                    currentMode.Mode = Display_Mode_Enum.Aggregation;
					currentMode.Aggregation_Type = Aggregation_Type_Enum.Home;
                    currentMode.Aggregation = "";
                    Output.WriteLine("\t\t<li><a href=\"" + currentMode.Redirect_URL() + "\">" + new_search + "</a></li>");
                    currentMode.Aggregation = ".all";
                }
                else
                {
                    currentMode.Mode = Display_Mode_Enum.Aggregation;
					currentMode.Aggregation_Type = Aggregation_Type_Enum.Home;
                    Output.WriteLine("\t\t<li><a href=\"" + currentMode.Redirect_URL() + "\">" + new_search + "</a></li>");
                }

				Output.WriteLine("\t</ul></div>");
				Output.WriteLine();

				// Add the scripts needed
				Output.WriteLine("<!-- Add references to the superfish and hoverintent libraries for the main user menu -->");
				Output.WriteLine("<script type=\"text/javascript\" src=\"" + Mode.Base_URL + "default/scripts/superfish/hoverIntent.js\" ></script>");
				Output.WriteLine("<script type=\"text/javascript\" src=\"" + Mode.Base_URL + "default/scripts/superfish/superfish.js\" ></script>");
				Output.WriteLine();

				Output.WriteLine("<!-- Initialize the main user menu -->");
				Output.WriteLine("<script>");
				Output.WriteLine();
				Output.WriteLine("jQuery(document).ready(function () {");
				Output.WriteLine("     jQuery('ul.sf-menu').superfish();");
				Output.WriteLine("  });");
				Output.WriteLine();
				Output.WriteLine("</script>");
				Output.WriteLine();

				Output.WriteLine("<br /><br />");

                currentMode.Mode = Display_Mode_Enum.Results;
                currentMode.Search_String = search_string;
                currentMode.Search_Fields = fields;


            }
           
            if ( resultsStatistics != null )
            {
                if (writeResult == null)
                {
                    Tracer.Add_Trace("Search_Results_HtmlSubwriter.Write_HTML", "Building Result DataSet Writer");
                    writeResult = new PagedResults_HtmlSubwriter(resultsStatistics, pagedResults, codeManager, translations, allItemsTable, currentUser, currentMode, Tracer)
                                      {Current_Aggregation = Current_Aggregation, Skin = htmlSkin, Mode = currentMode};
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
                if (Current_Aggregation != null)
                {
                    return "{0} Search Results - " + Current_Aggregation.Name;
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

			Output.WriteLine("  <link href=\"" + currentMode.Base_URL + "default/SobekCM_UserMenu.css\" rel=\"stylesheet\" type=\"text/css\" title=\"standard\" />");
        }
    }
}
