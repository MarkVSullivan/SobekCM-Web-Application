#region Using directives

using System.IO;
using System.Web.UI.WebControls;
using SobekCM.Library.Aggregations;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.Navigation;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.AggregationViewer.Viewers
{
    /// <summary> Enumeration specifies if the subaggregation selection panel is displayed for a particular collection viewer </summary>
    public enum Selection_Panel_Display_Enum : byte
    {
        /// <summary> This collection viewer never shows the subaggregation selection panel </summary>
        Never = 0,

        /// <summary> This collection viewer allows the user to select to display or hide the subaggregation selection panel </summary>
        Selectable = 1,

        /// <summary> This collection viewer always displays the subaggregation selection panel </summary>
        Always = 2
    }

    /// <summary> Interface which all collection viewers must implement </summary>
    /// <remarks> Collection viewers are used when displaying collection home pages, searches, browses, and information pages.<br /><br />
    /// During a valid html request, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="SobekCM_Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  For a collection-level request, an instance of the  <see cref="Aggregation_HtmlSubwriter"/> class is created. </li>
    /// <li>To display the requested collection view, the collection subwriter will create one or more collection viewers ( implementing this class )</li>
    /// </ul></remarks>
    public interface iAggregationViewer
    {
        /// <summary> Gets the type of collection view or search </summary>
        Item_Aggregation.CollectionViewsAndSearchesEnum Type { get; }

        /// <summary> Gets the reference to the javascript file to be included in the HTML </summary>
        string Search_Script_Reference { get; }

        /// <summary> Gets the reference to the javascript method to be called </summary>
        string Search_Script_Action { get; }

        /// <summary> Gets flag which indicates whether to always use the home text as the secondary text </summary>
        bool Always_Display_Home_Text { get; }

        /// <summary> Gets flag which indicates whether the secondary text requires controls </summary>
        bool Secondary_Text_Requires_Controls { get; }

        /// <summary> Gets flag which indicates whether the selection panel should be displayed </summary>
        Selection_Panel_Display_Enum Selection_Panel_Display { get; }

        /// <summary> Add the HTML to be displayed in the search box </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        void Add_Search_Box_HTML(TextWriter Output, Custom_Tracer Tracer);

        /// <summary> Add the HTML to be displayed below the search box </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        void Add_Secondary_HTML(TextWriter Output, Custom_Tracer Tracer);

        /// <summary> Add the HTML and controls to the section below the search box </summary>
        /// <param name="MainPlaceHolder">Place holder to add html and controls to</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        void Add_Secondary_Controls(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer);
    }
}
