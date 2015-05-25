#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI.WebControls;
using SobekCM.Core.Navigation;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.HTML
{
    /// <summary> Search results html subwriter renders a browse of search results  </summary>
    /// <remarks> This class extends the <see cref="abstractHtmlSubwriter"/> abstract class. </remarks>
    public class Search_Results_HtmlSubwriter : abstractHtmlSubwriter
    {
        private PagedResults_HtmlSubwriter writeResult;

        /// <summary> Constructor for a new instance of the Search_Results_HtmlSubwriter class </summary>
        /// <param name="RequestSpecificValues"> All the neFcessary, non-global data specific to the current request </param>
        public Search_Results_HtmlSubwriter(RequestCache RequestSpecificValues) : base(RequestSpecificValues) 
        {
            // Do nothing
         }

        /// <summary> Adds controls to the main navigational page </summary>
        /// <param name="MainPlaceHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form, widely used throughout the application</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <param name="PopulateNodeEvent"> Event is used to populate the a tree node without doing a full refresh of the page </param>
        /// <returns> Sorted tree with the results in hierarchical structure with volumes and issues under the titles </returns>
        /// <remarks> This uses a <see cref="PagedResults_HtmlSubwriter"/> instance to render the items  </remarks>
        public void Add_Controls(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer, TreeNodeEventHandler PopulateNodeEvent )
        {
            if (RequestSpecificValues.Results_Statistics == null) return;

            if (writeResult == null)
            {
                Tracer.Add_Trace("Search_Results_HtmlSubwriter.Add_Controls", "Building Result DataSet Writer");

                writeResult = new PagedResults_HtmlSubwriter(RequestSpecificValues);
            }

            Tracer.Add_Trace("Search_Results_HtmlSubwriter.Add_Controls", "Add controls");
            writeResult.Add_Controls(MainPlaceHolder, Tracer);
        }

        /// <summary> Writes the final output to close this search page results, including the results page navigation buttons </summary>
        /// <param name="Output"> Stream to which to write the HTML for this subwriter </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> TRUE is always returned </returns>
        /// <remarks> This calls the <see cref="PagedResults_HtmlSubwriter.Write_Final_HTML"/> method in the <see cref="PagedResults_HtmlSubwriter"/> object. </remarks>
		public override void Write_Final_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("browse_info_html_subwriter.Write_Final_Html", "Rendering HTML ( finish the main viewer section )");

            if (writeResult != null)
            {
                writeResult.Write_Final_HTML(Output, Tracer);
            }
        }

        /// <summary> Writes the HTML generated to browse the results of a search directly to the response stream </summary>
        /// <param name="Output"> Stream to which to write the HTML for this subwriter </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> TRUE -- Value indicating if html writer should finish the page immediately after this, or if there are other controls or routines which need to be called first </returns>
        public override bool Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Search_Results_HtmlSubwriter.Write_HTML", "Rendering HTML");

            // If this skin has top-level navigation suppressed, skip the top tabs
            if ((RequestSpecificValues.HTML_Skin.Suppress_Top_Navigation.HasValue) && (RequestSpecificValues.HTML_Skin.Suppress_Top_Navigation.Value))
            {
                Output.WriteLine("<br />");
            }
            else
            {
				// Add the main aggrgeation menu here
				MainMenus_Helper_HtmlSubWriter.Add_Aggregation_Search_Results_Menu(Output, RequestSpecificValues, false);
            }
           
            if ( RequestSpecificValues.Results_Statistics != null )
            {
                if (writeResult == null)
                {
                    Tracer.Add_Trace("Search_Results_HtmlSubwriter.Write_HTML", "Building Result DataSet Writer");
                    writeResult = new PagedResults_HtmlSubwriter(RequestSpecificValues);
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
                if (RequestSpecificValues.Current_Mode.Result_Display_Type == Result_Display_Type_Enum.Map)
                {
                    List<Tuple<string, string>> returnValue = new List<Tuple<string, string>> {new Tuple<string, string>("onload", "load();")};

                    return returnValue;
                }
                if (RequestSpecificValues.Current_Mode.Result_Display_Type == Result_Display_Type_Enum.Map_Beta)
                {
                    List<Tuple<string, string>> returnValue = new List<Tuple<string, string>> {new Tuple<string, string>("onload", "load();")};

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
                if (RequestSpecificValues.Hierarchy_Object != null)
                {
                    return "{0} Search Results - " + RequestSpecificValues.Hierarchy_Object.Name;
                }
                return "{0} Search Results";
            }
        }

        /// <summary> Gets the collection of special behaviors which this subwriter
        /// requests from the main HTML subwriter. </summary>
        /// <remarks> By default, this returns an empty list </remarks>
        public override List<HtmlSubwriter_Behaviors_Enum> Subwriter_Behaviors
        {
            get
            {
                return new List<HtmlSubwriter_Behaviors_Enum> { HtmlSubwriter_Behaviors_Enum.Include_Skip_To_Main_Content_Link };
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

		/// <summary> Gets the CSS class of the container that the page is wrapped within </summary>
		public override string Container_CssClass
		{
			get
			{
                return RequestSpecificValues.Paged_Results != null ? "container-facets" : base.Container_CssClass;
			}
		}
    }
}
