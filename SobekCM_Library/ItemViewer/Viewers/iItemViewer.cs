#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI.WebControls;
using SobekCM.Core.Navigation;
using SobekCM.Library.HTML;
using SobekCM.Resource_Object;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.ItemViewer.Viewers
{
    /// <summary> Page selector type enumeration </summary>
    public enum ItemViewer_PageSelector_Type_Enum : byte
    {
        /// <summary> No page selector </summary>
        NONE,

        /// <summary> Drop down selection list to choose a particular page </summary>
        DropDownList,

        /// <summary> Links for pages of results </summary>
        PageLinks
    }

	/// <summary> Interface which all item viewer objects must implement </summary>
	public interface iItemViewer
	{
        /// <summary> CSS ID for the viewer viewport for this particular viewer </summary>
        string ViewerBox_CssId { get; }

        /// <summary> Any additional inline style for this viewer that affects the main box around this</summary>
        string ViewerBox_InlineStyle { get; }

        /// <summary> Gets the collection of special behaviors which this item viewer
        /// requests from the main HTML subwriter. </summary>
        List<HtmlSubwriter_Behaviors_Enum> ItemViewer_Behaviors { get; }

        #region Methods used to write HTML directly to the output stream in various locations

        /// <summary> Write any additional values within the HTML Head of the final served page </summary>
        /// <param name="Output"> Output stream currently within the HTML head tags </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <remarks> By default this does nothing, but can be overwritten by all the individual item viewers </remarks>
        void Write_Within_HTML_Head(TextWriter Output, Custom_Tracer Tracer);

        /// <summary> Gets the collection of body attributes to be included 
        /// within the HTML body tag (usually to add events to the body) </summary>
        /// <param name="Body_Attributes"> List of body attributes to be included </param>
        void Add_ViewerSpecific_Body_Attributes(List<Tuple<string, string>> Body_Attributes);

        /// <summary> Adds any viewer_specific information to the left Navigation Bar Menu Section  </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        void Write_Left_Nav_Menu_Section(TextWriter Output, Custom_Tracer Tracer);

        /// <summary> Adds any viewer_specific information to the item viewer above the standard pagination buttons </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        void Write_Top_Additional_Navigation_Row(TextWriter Output, Custom_Tracer Tracer);

        /// <summary> Write the item viewer main section as HTML directly to the HTTP output stream </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer);

        #endregion

        #region Method used to add directly to the place holder within the item viewer (currently used by EAD containers )

        /// <summary> Allows controls to be added directory to a place holder, rather than just writing to the output HTML stream </summary>
        /// <param name="MainPlaceHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the the bulk of the item viewer's output is displayed</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        void Add_Main_Viewer_Section(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer);

        #endregion

        #region Properties used if the viewer has multiple pages and wishes to use the built-in page turning options

        /// <summary> Gets the flag that indicates if the page selector should be shown, and which page selector </summary>
        ItemViewer_PageSelector_Type_Enum Page_Selector { get; }

	    /// <summary> Gets the number of pages for this viewer </summary>
		int PageCount { get; }

        /// <summary> Gets the current page for paging purposes </summary>
        int Current_Page { get; }

		/// <summary> Gets the url to go to the first page </summary>
		string First_Page_URL { get; }

		/// <summary> Gets the url to go to the previous page </summary>
		string Previous_Page_URL { get; }

		/// <summary> Gets the url to go to the next page </summary>
		string Next_Page_URL { get; }

		/// <summary> Gets the url to go to the last page </summary>
		string Last_Page_URL { get; }

        /// <summary> Gets the names to show in the Go To combo box </summary>
		string[] Go_To_Names { get; }

        #endregion

    }
}
