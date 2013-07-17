#region Using directives

using System.Collections.Generic;
using System.IO;
using System.Web.UI.WebControls;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Library.Aggregations;
using SobekCM.Library.Navigation;
using SobekCM.Library.Results;
using SobekCM.Library.WebContent;

#endregion

namespace SobekCM.Library.MainWriters
{
    /// <summary> Abstract class which all main writer classes must extend </summary>
    public abstract class abstractMainWriter
    {
        /// <summary> Protected field contains the current item to display </summary>
        protected SobekCM_Item currentItem;

        /// <summary> Protected field contains the mode / navigation information for the current request </summary>
        protected SobekCM_Navigation_Object currentMode;

        /// <summary> Protected field contains the current page within the item </summary>
        protected Page_TreeNode currentPage;

        /// <summary> Protected field contains the current item aggregation object to display </summary>
        protected Item_Aggregation hierarchyObject;

        /// <summary> Content derived from static HTML pages, such as static info/browse pages or general web content </summary>
        protected HTML_Based_Content htmlBasedContent;

        /// <summary> Protected field contains the single page of results for a search or browse, within the entire set </summary>
        protected List<iSearch_Title_Result> paged_results;

        /// <summary> Protected field contains the information about the entire set of results for a search or browse</summary>
        protected Search_Results_Statistics results_statistics;

        /// <summary> Protected field contains the basic information about any browse or info display</summary>
        protected Item_Aggregation_Browse_Info thisBrowseObject;

        /// <summary> Constructor for a new instance of the abstractMainWriter abstract class </summary>
        /// <param name="Current_Mode"> Mode / navigation information for the current request</param>
        /// <param name="Hierarchy_Object"> Current item aggregation object to display </param>
        /// <param name="Results_Statistics"> Information about the entire set of results for a search or browse </param>
        /// <param name="Paged_Results"> Single page of results for a search or browse, within the entire set </param>
        /// <param name="Browse_Object"> Object contains all the basic information about any browse or info display </param>
        /// <param name="Current_Item"> Current item to display </param>
        /// <param name="Current_Page"> Current page within the item</param>
        /// <param name="Static_Web_Content"> HTML content-based browse, info, or imple CMS-style web content objects.  These are objects which are read from a static HTML file and much of the head information must be maintained </param>
        protected abstractMainWriter(SobekCM_Navigation_Object Current_Mode,
            Item_Aggregation Hierarchy_Object,
            Search_Results_Statistics Results_Statistics,
            List<iSearch_Title_Result> Paged_Results,
            Item_Aggregation_Browse_Info Browse_Object,
            SobekCM_Item Current_Item,
            Page_TreeNode Current_Page,
            HTML_Based_Content Static_Web_Content )
        {
            currentMode = Current_Mode;
            hierarchyObject = Hierarchy_Object;
            results_statistics = Results_Statistics;
            paged_results = Paged_Results;
            thisBrowseObject = Browse_Object;
            currentItem = Current_Item;
            currentPage = Current_Page;
            htmlBasedContent = Static_Web_Content;
        }

        /// <summary> Gets the enumeration of the type of main writer </summary>
        public abstract Writer_Type_Enum Writer_Type { get; }

        /// <summary> Returns a flag indicating whether the navigation form should be included in the page </summary>
        /// <value> This value can be override by child classes, but by default this returns FALSE </value>
        public virtual bool Include_Navigation_Form
        {
            get
            {
                return false;
            }
        }


        /// <summary> Returns a flag indicating whether the additional table of contents place holder ( &quot;tocPlaceHolder&quot; ) in the itemNavForm form will be utilized 
        /// for the current request, or if it can be hidden. </summary>
        /// <value> This value can be override by child classes, but by default this returns FALSE </value>
        public virtual bool Include_TOC_Place_Holder
        {
            get
            {
                return false;
            }
        }

        /// <summary> Returns a flag indicating whether the additional place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form will be utilized 
        /// for the current request, or if it can be hidden. </summary>
        /// <value> This value can be override by child classes, but by default this returns FALSE </value>
        public virtual bool Include_Main_Place_Holder
        {
            get
            {
                return false;
            }
        }

        /// <summary> Perform all the work of adding text directly to the response stream back to the web user </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public abstract void Write_Html(TextWriter Output, Custom_Tracer Tracer);

        /// <summary> Perform all the work of adding to the response stream back to the web user </summary>
        /// <param name="TOC_Place_Holder"> Place holder is used to add more complex server-side objects during execution</param>
        /// <param name="Main_Place_Holder"> Place holder is used to add more complex server-side objects during execution</param>
        /// <param name="myUfdcUploadPlaceHolder"> Place holder is used to add more complex server-side objects during execution </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        public virtual void Add_Controls(PlaceHolder TOC_Place_Holder, PlaceHolder Main_Place_Holder, PlaceHolder myUfdcUploadPlaceHolder, Custom_Tracer Tracer)
        {
            // Do nothing
        }
    }
}
