#region Using directives

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

                    writeResult = new PagedResults_HtmlSubwriter(resultsStatistics, pagedResults, codeManager, translations, allItemsTable, currentUser, currentMode, Tracer)
                                      {
                                          Hierarchy_Object = Hierarchy_Object,
                                          Skin = htmlSkin,
                                          Mode = currentMode,
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
        /// <returns> TRUE is always returned </returns>
        /// <remarks> This calls the <see cref="PagedResults_HtmlSubwriter.Write_Final_HTML"/> method in the <see cref="PagedResults_HtmlSubwriter"/> object. </remarks>
        public bool Write_Final_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Public_Folder_HtmlSubwriter.Write_Final_Html", "Rendering HTML ( finish the main viewer section )");

            if (writeResult != null)
            {
                writeResult.Write_Final_HTML(Output, Tracer);
            }
            return true;
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

            if (currentMode.Language == Web_Language_Enum.French)
            {
                homeText = "PAGE D'ACCUEIL";
            }

            if (currentMode.Language == Web_Language_Enum.Spanish)
            {
                homeText = "INICIO";
            }

            Output.WriteLine("<div class=\"ViewsBrowsesRow\">");

            currentMode.Mode = Display_Mode_Enum.Aggregation_Home;
            Output.WriteLine("  <a href=\"" + currentMode.Redirect_URL() + "\">" + Unselected_Tab_Start + homeText + Unselected_Tab_End + "</a>");
            currentMode.Mode = Display_Mode_Enum.Public_Folder;
            
            Output.WriteLine("  " + Selected_Tab_Start + publicFolderText + Selected_Tab_End);
            Output.WriteLine("</div>");
            Output.WriteLine();

            if (( pagedResults != null ) && ( resultsStatistics != null ))
            {
                if (writeResult == null)
                {
                    Tracer.Add_Trace("Public_Folder_HtmlSubwriter.Write_HTML", "Building Result DataSet Writer");
                    writeResult = new PagedResults_HtmlSubwriter(null, null, codeManager, translations, allItemsTable, currentUser, currentMode, Tracer)
                                      {
                                          Hierarchy_Object = Hierarchy_Object,
                                          Skin = htmlSkin,
                                          Mode = currentMode,
                                          Browse_Title = publicFolder.FolderName,
                                          Folder_Owner_Name = publicFolder.Name,
                                          Folder_Owner_Email = publicFolder.Email
                                      };
                }
                writeResult.Write_HTML(Output, Tracer);
            }

            return true;
        }
    }
}
