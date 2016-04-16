#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI.WebControls;
using SobekCM.Core.Aggregations;
using SobekCM.Core.Navigation;
using SobekCM.Core.UI_Configuration;
using SobekCM.Core.UI_Configuration.StaticResources;
using SobekCM.Engine_Library.Configuration;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.HTML
{
    /// <summary> Search results html subwriter renders a browse of search results  </summary>
    /// <remarks> This class extends the <see cref="abstractHtmlSubwriter"/> abstract class. </remarks>
    public class Search_Results_HtmlSubwriter : abstractHtmlSubwriter
    {
        private PagedResults_HtmlSubwriter writeResult;
        private readonly Item_Aggregation hierarchyObject;

        /// <summary> Constructor for a new instance of the Search_Results_HtmlSubwriter class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        public Search_Results_HtmlSubwriter(RequestCache RequestSpecificValues) : base(RequestSpecificValues) 
        {
            // Use the method in the base class to actually pull the entire hierarchy
            if (!Get_Collection(RequestSpecificValues.Current_Mode, RequestSpecificValues.Tracer, out hierarchyObject))
            {
                RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Error;
            }
        }


        /// <summary> Write any additional values within the HTML Head of the
        /// final served page </summary>
        /// <param name="Output"> Output stream currently within the HTML head tags </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Within_HTML_Head(TextWriter Output, Custom_Tracer Tracer)
        {
            Output.WriteLine("  <meta name=\"robots\" content=\"index, nofollow\" />");

            // If this is the thumbnails results, add the QTIP script and css
            if ((RequestSpecificValues.Results_Statistics != null) &&
                (RequestSpecificValues.Results_Statistics.Total_Items > 0) &&
                (RequestSpecificValues.Current_Mode.Result_Display_Type == Result_Display_Type_Enum.Thumbnails))
            {
                Output.WriteLine("  <script type=\"text/javascript\" src=\"" + Static_Resources_Gateway.Jquery_Qtip_Js + "\"></script>");
                Output.WriteLine("  <link rel=\"stylesheet\" type=\"text/css\" href=\"" + Static_Resources_Gateway.Jquery_Qtip_Css + "\" /> ");
            }
        }

        /// <summary> Chance for a final, final CSS which can override anything else, including the web skin </summary>
        public override string Final_CSS
        {
            get
            {
                // Finally add the aggregation-level CSS if it exists
                if ((hierarchyObject != null) && (!String.IsNullOrEmpty(hierarchyObject.CSS_File)))
                {
                    return "  <link href=\"" + RequestSpecificValues.Current_Mode.Base_Design_URL + "aggregations/" + hierarchyObject.Code + "/" + hierarchyObject.CSS_File + "\" rel=\"stylesheet\" type=\"text/css\" />";
                }
                return String.Empty;
            }
        }

        /// <summary> Gets the collection of body attributes to be included 
        /// within the HTML body tag (usually to add events to the body) </summary>
        public override List<Tuple<string, string>> Body_Attributes
        {
            get
            {
                if (RequestSpecificValues.Current_Mode.Result_Display_Type == Result_Display_Type_Enum.Map)
                {
                    List<Tuple<string, string>> returnValue = new List<Tuple<string, string>> { new Tuple<string, string>("onload", "load();") };

                    return returnValue;
                }
                if (RequestSpecificValues.Current_Mode.Result_Display_Type == Result_Display_Type_Enum.Map_Beta)
                {
                    List<Tuple<string, string>> returnValue = new List<Tuple<string, string>> { new Tuple<string, string>("onload", "load();") };

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
                if (hierarchyObject != null)
                {
                    return "{0} Search Results - " + hierarchyObject.Name;
                }
                return "{0} Search Results";
            }
        }

        /// <summary> Add the header to the output </summary>
        /// <param name="Output"> Stream to which to write the HTML for this header </param>
        public override void Add_Header(TextWriter Output)
        {
            HeaderFooter_Helper_HtmlSubWriter.Add_Header(Output, RequestSpecificValues, Container_CssClass, WebPage_Title, Subwriter_Behaviors, hierarchyObject, null);
        }

        /// <summary> Flag indicates if the internal header should included </summary>
        /// <remarks> By default this return TRUE if the user is internal, or a portal/system admin, but can be 
        /// overwritten by all the individual html subwriters </remarks>
        public override bool Include_Internal_Header
        {
            get
            {
                // If no user, do not show
                if ((RequestSpecificValues.Current_User == null) || (!RequestSpecificValues.Current_User.LoggedOn))
                    return false;

                // Always show for admins
                if ((RequestSpecificValues.Current_User.Is_System_Admin) || (RequestSpecificValues.Current_User.Is_Portal_Admin))
                    return true;

                if ((RequestSpecificValues.Current_User.Is_Aggregation_Curator(RequestSpecificValues.Current_Mode.Aggregation)) || (RequestSpecificValues.Current_User.Can_Edit_All_Items(RequestSpecificValues.Current_Mode.Aggregation)))
                    return true;

                // Otherwise, do not show
                return false;
            }
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

                writeResult = new PagedResults_HtmlSubwriter(RequestSpecificValues, RequestSpecificValues.Results_Statistics, RequestSpecificValues.Paged_Results);
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
				MainMenus_Helper_HtmlSubWriter.Add_Aggregation_Search_Results_Menu(Output, RequestSpecificValues, hierarchyObject, false);
            }
           
            if ( RequestSpecificValues.Results_Statistics != null )
            {
                if (writeResult == null)
                {
                    Tracer.Add_Trace("Search_Results_HtmlSubwriter.Write_HTML", "Building Result DataSet Writer");
                    writeResult = new PagedResults_HtmlSubwriter(RequestSpecificValues, RequestSpecificValues.Results_Statistics, RequestSpecificValues.Paged_Results);
                }
                writeResult.Write_HTML(Output, Tracer);
            }

            return true;
        }

        /// <summary> Add the footer to the output </summary>
        /// <param name="Output"> Stream to which to write the HTML for this footer </param>
        public override void Add_Footer(TextWriter Output)
        {
            HeaderFooter_Helper_HtmlSubWriter.Add_Footer(Output, RequestSpecificValues, Subwriter_Behaviors, hierarchyObject, null);
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
