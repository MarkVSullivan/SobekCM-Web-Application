#region Using directives

using System.Web.UI.WebControls;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.ResultsViewer
{
    /// <summary> Interface which all results viewer objects must implement </summary>
    public interface iResultsViewer
    {
        /// <summary> Flag indicates if this result view is sortable </summary>
        bool Sortable { get; }

        /// <summary> Gets the total number of results to display </summary>
        int Total_Results { get; }

        /// <summary> Adds the controls for this result viewer to the place holder on the main form </summary>
        /// <param name="MainPlaceHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the the bulk of the result viewer's output is displayed</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Sorted tree with the results in hierarchical structure with volumes and issues under the titles and sorted by serial hierarchy </returns>
        void Add_HTML(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer);
    }
}
