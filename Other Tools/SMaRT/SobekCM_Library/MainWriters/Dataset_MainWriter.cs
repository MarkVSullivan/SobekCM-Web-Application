#region Using directives

using System.Collections.Generic;
using System.IO;
using System.Web.UI.WebControls;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Library.Aggregations;
using SobekCM.Library.Navigation;
using SobekCM.Library.Results;

#endregion

namespace SobekCM.Library.MainWriters
{
    /// <summary> Main writer writes search results and item browses as a dataset represented in
    /// XML format to the response stream.  This is the native Microsoft.NET format, easily read into
    /// a remote dataset by using DataSet.ReadXML() </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractMainWriter"/>. </remarks>
    public class Dataset_MainWriter : abstractMainWriter
    {
        /// <summary> Constructor for a new instance of the Dataset_MainWriter class </summary>
        /// <param name="Current_Mode"> Mode / navigation information for the current request</param>
        /// <param name="Hierarchy_Object"> Current item aggregation object to display </param>
        /// <param name="Results_Statistics"> Information about the entire set of results for a search or browse </param>
        /// <param name="Paged_Results"> Single page of results for a search or browse, within the entire set </param>
        /// <param name="Browse_Object"> Object contains all the basic information about any browse or info display </param>
         /// <param name="Current_Item"> Current item to display </param>
        /// <param name="Current_Page"> Current page within the item</param>
        public Dataset_MainWriter(SobekCM_Navigation_Object Current_Mode,
            Item_Aggregation Hierarchy_Object,
            Search_Results_Statistics Results_Statistics,
            List<iSearch_Title_Result> Paged_Results,
            Item_Aggregation_Browse_Info Browse_Object,
            SobekCM_Item Current_Item,
            Page_TreeNode Current_Page)
            : base(Current_Mode, Hierarchy_Object, Results_Statistics, Paged_Results, Browse_Object,  Current_Item, Current_Page, null)
   
        {
            // All work done in base class
        }

        /// <summary> Gets the enumeration of the type of main writer </summary>
        /// <value> This property always returns the enumerational value <see cref="SobekCM.Library.Navigation.Writer_Type_Enum.DataSet"/>. </value>
        public override Writer_Type_Enum Writer_Type { get { return Writer_Type_Enum.DataSet; } }

        /// <summary> Perform all the work of adding text directly to the response stream back to the web user </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Add_Text_To_Page(TextWriter Output, Custom_Tracer Tracer)
        {
            switch (currentMode.Mode)
            {
                case Display_Mode_Enum.Results:
                case Display_Mode_Enum.Aggregation_Browse_Info:
                    if (paged_results != null)
                        display_search_results(Output);
                     break;
                default:
                    Output.Write("DataSet Writer - Unknown Mode");
                    break;
            }
        }

        /// <summary> Perform all the work of adding to the response stream back to the web user </summary>
        /// <param name="Navigation_Place_Holder"> Place holder is used to add more complex server-side objects during execution</param>
        /// <param name="TOC_Place_Holder"> Place holder is used to add more complex server-side objects during execution</param>
        /// <param name="Main_Place_Holder"> Place holder is used to add more complex server-side objects during execution</param>
        /// <param name="myUfdcUploadPlaceHolder"> Place holder is used to add more complex server-side objects during execution </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> Since this class writes all the output directly to the response stream, this method simply returns, without doing anything</remarks>
        public override void Add_Controls(PlaceHolder Navigation_Place_Holder,
            PlaceHolder TOC_Place_Holder,
            PlaceHolder Main_Place_Holder,
            PlaceHolder myUfdcUploadPlaceHolder,
            Custom_Tracer Tracer)
        {
            return;
        }

        private void display_search_results(TextWriter Output)
        {
            // Write this information
            //search_results.WriteXml(Output, XmlWriteMode.WriteSchema);
        }
    }
}
