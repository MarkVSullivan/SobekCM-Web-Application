#region Using directives

using System.Collections.Generic;
using System.Web.UI.WebControls;
using SobekCM.Library.Aggregations;
using SobekCM.Library.Application_State;
using SobekCM.Library.Navigation;
using SobekCM.Library.Results;

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

        /// <summary> Set the current mode / navigation information for the current request </summary>
        SobekCM_Navigation_Object CurrentMode	{ set; }

        /// <summary> Sets the single page of results for a search or browse, within the entire set </summary>
        List<iSearch_Title_Result> Paged_Results { set; }

        /// <summary> Sets the information about the entire set of results for a search or browse</summary>
        Search_Results_Statistics Results_Statistics { set; }

        /// <summary> Sets the current item aggregation under which these results are displayed </summary>
        Item_Aggregation HierarchyObject	{ set; }

        /// <summary> Sets the language support object which handles simple translational duties </summary>
        Language_Support_Info Translator		{	set;	}

        /// <summary> Sets the lookup object used to pull basic information about any item loaded into this library </summary>
        Item_Lookup_Object All_Items_Lookup { set; }

        /// <summary> Adds the controls for this result viewer to the place holder on the main form </summary>
        /// <param name="MainPlaceHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the the bulk of the result viewer's output is displayed</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Sorted tree with the results in hierarchical structure with volumes and issues under the titles and sorted by serial hierarchy </returns>
        void Add_HTML(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer);
    }
}
