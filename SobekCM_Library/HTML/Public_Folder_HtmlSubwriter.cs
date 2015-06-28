#region Using directives

using System.IO;
using System.Web.UI.WebControls;
using SobekCM.Core.Configuration;
using SobekCM.Core.Navigation;
using SobekCM.Library.Settings;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.HTML
{
    /// <summary> Public folder html subwriter renders a browse of a public bookshelf folder  </summary>
    /// <remarks> This class extends the <see cref="abstractHtmlSubwriter"/> abstract class. </remarks>
    public class Public_Folder_HtmlSubwriter : abstractHtmlSubwriter
    {
        private PagedResults_HtmlSubwriter writeResult;

        /// <summary> Constructor for a new instance of the Public_Folder_HtmlSubwriter class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        public Public_Folder_HtmlSubwriter(RequestCache RequestSpecificValues) : base(RequestSpecificValues) 
        {
            // Do nothing
        }

        /// <summary> Adds controls to the main navigational page </summary>
        /// <param name="placeHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form, widely used throughout the application</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <param name="populate_node_event"> Event is used to populate the a tree node without doing a full refresh of the page </param>
        /// <returns> Sorted tree with the results in hierarchical structure with volumes and issues under the titles </returns>
        /// <remarks> This uses a <see cref="PagedResults_HtmlSubwriter"/> instance to render the browse  </remarks>
        public void Add_Controls(PlaceHolder placeHolder, Custom_Tracer Tracer, TreeNodeEventHandler populate_node_event)
        {
            if ((RequestSpecificValues.Paged_Results != null) && (RequestSpecificValues.Results_Statistics != null))
            {
                if (writeResult == null)
                {
                    Tracer.Add_Trace("Public_Folder_HtmlSubwriter.Add_Controls", "Building Result DataSet Writer");

                    writeResult = new PagedResults_HtmlSubwriter(RequestSpecificValues)
                                      {
                                          Browse_Title = RequestSpecificValues.Public_Folder.FolderName,
                                          Folder_Owner_Name = RequestSpecificValues.Public_Folder.Name,
                                          Folder_Owner_Email = RequestSpecificValues.Public_Folder.Email
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

            if (RequestSpecificValues.Current_Mode.Language == Web_Language_Enum.French)
            {
            }

            if (RequestSpecificValues.Current_Mode.Language == Web_Language_Enum.Spanish)
            {
            }

			// Add the item views
			Output.WriteLine("<!-- Add the menu -->");
			Output.WriteLine("<div id=\"sbkPfm_MenuBar\" class=\"sbkMenu_Bar\">");
			Output.WriteLine("<ul class=\"sf-menu\">");

			// Get ready to draw the tabs
			string sobek_home_text = RequestSpecificValues.Current_Mode.Instance_Abbreviation + " Home";

			// Add the 'SOBEK HOME' first menu option and suboptions
			RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Aggregation;
			RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Home;
			RequestSpecificValues.Current_Mode.Home_Type = Home_Type_Enum.List;
            Output.WriteLine("\t\t<li class=\"sbkMenu_Home\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\" class=\"sbkMenu_NoPadding\"><img src=\"" + Static_Resources.Home_Png + "\" /> <div class=\"sbkMenu_HomeText\">" + sobek_home_text + "</div></a></li>");

			RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Public_Folder;
			Output.WriteLine("\t\t<li class=\"selected-sf-menu-item-link\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\">" + publicFolderText + "</a></li>");
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

            if (( RequestSpecificValues.Paged_Results != null ) && ( RequestSpecificValues.Results_Statistics != null ))
            {
                if (writeResult == null)
                {
                    Tracer.Add_Trace("Public_Folder_HtmlSubwriter.Write_HTML", "Building Result DataSet Writer");
                    writeResult = new PagedResults_HtmlSubwriter(RequestSpecificValues)
                                      {
                                          Browse_Title = RequestSpecificValues.Public_Folder.FolderName,
                                          Folder_Owner_Name = RequestSpecificValues.Public_Folder.Name,
                                          Folder_Owner_Email = RequestSpecificValues.Public_Folder.Email
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
                return RequestSpecificValues.Public_Folder != null ? RequestSpecificValues.Public_Folder.FolderName : "{0} Folder";
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
