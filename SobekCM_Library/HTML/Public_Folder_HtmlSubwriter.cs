#region Using directives

using System.Collections.Generic;
using System.IO;
using System.Web.UI.WebControls;
using SobekCM.Core.Configuration;
using SobekCM.Library.Application_State;
using SobekCM.Library.Configuration;
using SobekCM.Library.Navigation;
using SobekCM.Library.Results;
using SobekCM.Core.Users;
using SobekCM.Tools;
using SobekCM_UI_Library.Navigation;

#endregion

namespace SobekCM.Library.HTML
{
    /// <summary> Public folder html subwriter renders a browse of a public bookshelf folder  </summary>
    /// <remarks> This class extends the <see cref="abstractHtmlSubwriter"/> abstract class. </remarks>
    public class Public_Folder_HtmlSubwriter : abstractHtmlSubwriter
    {
        private readonly Item_Lookup_Object allItemsTable;
        private readonly Aggregation_Code_Manager codeManager;
        private readonly User_Object currentUser;
        private readonly List<iSearch_Title_Result> pagedResults;
        private readonly Public_User_Folder publicFolder;
        private readonly Search_Results_Statistics resultsStatistics;
        private readonly Language_Support_Info translations;
        private PagedResults_HtmlSubwriter writeResult;

        /// <summary> Constructor for a new instance of the Public_Folder_HtmlSubwriter class </summary>
        /// <param name="Results_Statistics"> Information about the entire set of results for a browse of a user's bookshelf folder</param>
        /// <param name="Paged_Results"> Single page of results for a browse of a user's bookshelf folder, within the entire set </param>
        /// <param name="Code_Manager"> List of valid collection codes, including mapping from the Sobek collections to Greenstone collections</param>
        /// <param name="Translator"> Language support object which handles simple translational duties </param>
        /// <param name="All_Items_Lookup"> Lookup object used to pull basic information about any item loaded into this library </param>
        /// <param name="Current_User"> Currently logged on user </param>
        /// <param name="Public_Folder"> Object contains the information about the public folder to display </param>
        public Public_Folder_HtmlSubwriter(Search_Results_Statistics Results_Statistics,
            List<iSearch_Title_Result> Paged_Results,
            Aggregation_Code_Manager Code_Manager, Language_Support_Info Translator,
            Item_Lookup_Object All_Items_Lookup, 
            User_Object Current_User, Public_User_Folder Public_Folder )
        {
            publicFolder = Public_Folder;
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
        /// <remarks> This uses a <see cref="PagedResults_HtmlSubwriter"/> instance to render the browse  </remarks>
        public void Add_Controls(PlaceHolder placeHolder, Custom_Tracer Tracer, TreeNodeEventHandler populate_node_event)
        {
            if ((pagedResults != null) && (resultsStatistics != null))
            {
                if (writeResult == null)
                {
                    Tracer.Add_Trace("Public_Folder_HtmlSubwriter.Add_Controls", "Building Result DataSet Writer");

                    writeResult = new PagedResults_HtmlSubwriter(resultsStatistics, pagedResults, codeManager, translations, allItemsTable, currentUser, Mode, Search_Stop_Words, Tracer)
                                      {
                                          Current_Aggregation = Current_Aggregation,
                                          Skin = Skin,
                                          Mode = Mode,
                                          Browse_Title = publicFolder.FolderName,
                                          Folder_Owner_Name = publicFolder.Name,
                                          Folder_Owner_Email = publicFolder.Email
                                      };
                }

                Tracer.Add_Trace("Public_Folder_HtmlSubwriter.Add_Controls", "Add controls");
                writeResult.Add_Controls(placeHolder, Tracer);
            }
        }

        /// <summary> Writes the final output to close this public folder browse, including the results page navigation buttons </summary>
        /// <param name="Output"> Stream to which to write the HTML for this subwriter </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <remarks> This calls the <see cref="PagedResults_HtmlSubwriter.Write_Final_HTML"/> method in the <see cref="PagedResults_HtmlSubwriter"/> object. </remarks>
        public override void Write_Final_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Public_Folder_HtmlSubwriter.Write_Final_Html", "Rendering HTML ( finish the main viewer section )");

            if (writeResult != null)
            {
                writeResult.Write_Final_HTML(Output, Tracer);
            }

        }

        /// <summary> Writes the HTML generated to browse a public folder  directly to the response stream </summary>
        /// <param name="Output"> Stream to which to write the HTML for this subwriter </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> TRUE -- Value indicating if html writer should finish the page immediately after this, or if there are other controls or routines which need to be called first </returns>
        public override bool Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Public_Folder_HtmlSubwriter.Write_HTML", "Rendering HTML");

            const string publicFolderText = "PUBLIC BOOKSHELF";
            string homeText = "HOME";

            if (Mode.Language == Web_Language_Enum.French)
            {
                homeText = "PAGE D'ACCUEIL";
            }

            if (Mode.Language == Web_Language_Enum.Spanish)
            {
                homeText = "INICIO";
            }

			// Add the item views
			Output.WriteLine("<!-- Add the menu -->");
			Output.WriteLine("<div id=\"sbkPfm_MenuBar\" class=\"sbkMenu_Bar\">");
			Output.WriteLine("<ul class=\"sf-menu\">");

			// Get ready to draw the tabs
			string sobek_home_text = Mode.SobekCM_Instance_Abbreviation + " Home";

			// Add the 'SOBEK HOME' first menu option and suboptions
			Mode.Mode = Display_Mode_Enum.Aggregation;
			Mode.Aggregation_Type = Aggregation_Type_Enum.Home;
			Mode.Home_Type = Home_Type_Enum.List;
			Output.WriteLine("\t\t<li class=\"sbkMenu_Home\"><a href=\"" + Mode.Redirect_URL() + "\" class=\"sbkMenu_NoPadding\"><img src=\"" + Mode.Default_Images_URL + "home.png\" /> <div class=\"sbkMenu_HomeText\">" + sobek_home_text + "</div></a></li>");

			Mode.Mode = Display_Mode_Enum.Public_Folder;
			Output.WriteLine("\t\t<li class=\"selected-sf-menu-item-link\"><a href=\"" + Mode.Redirect_URL() + "\">" + publicFolderText + "</a></li>");
			Output.WriteLine("\t</ul></div>");

			Output.WriteLine("<!-- Initialize the main user menu -->");
			Output.WriteLine("<script>");
			Output.WriteLine("  jQuery(document).ready(function () {");
			Output.WriteLine("     jQuery('ul.sf-menu').superfish();");
			Output.WriteLine("  });");
			Output.WriteLine("</script>");
			Output.WriteLine();

			Output.WriteLine("<br />");
			Output.WriteLine();

            if (( pagedResults != null ) && ( resultsStatistics != null ))
            {
                if (writeResult == null)
                {
                    Tracer.Add_Trace("Public_Folder_HtmlSubwriter.Write_HTML", "Building Result DataSet Writer");
                    writeResult = new PagedResults_HtmlSubwriter(null, null, codeManager, translations, allItemsTable, currentUser, Mode, Search_Stop_Words, Tracer)
                                      {
                                          Current_Aggregation = Current_Aggregation,
                                          Skin = Skin,
                                          Mode = Mode,
                                          Browse_Title = publicFolder.FolderName,
                                          Folder_Owner_Name = publicFolder.Name,
                                          Folder_Owner_Email = publicFolder.Email
                                      };
                }
                writeResult.Write_HTML(Output, Tracer);
            }

            return true;
        }


        /// <summary> Title for this web page </summary>
        public override string WebPage_Title
        {
            get {
                return publicFolder != null ? publicFolder.FolderName : "{0} Folder";
            }
        }

        /// <summary> Write any additional values within the HTML Head of the
        /// final served page </summary>
        /// <param name="Output"> Output stream currently within the HTML head tags </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Within_HTML_Head(TextWriter Output, Custom_Tracer Tracer)
        {
            Output.WriteLine("  <meta name=\"robots\" content=\"index, follow\" />");
        }
    }
}
