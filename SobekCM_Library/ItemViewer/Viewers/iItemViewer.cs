#region Using directives

using System.Web.UI.WebControls;
using SobekCM.Bib_Package;
using SobekCM.Library.Navigation;

#endregion

namespace SobekCM.Library.ItemViewer.Viewers
{
    /// <summary> Delegate defines a signature for an event used by item viewers for requesting
    /// an immediate redirect of the user's browser. </summary>
    /// <param name="new_url"> URL to forward the user to </param>
	public delegate void Redirect_Requested( string new_url );

    /// <summary> Enumeration of the different item viewer types for displaying an item in HTML </summary>
    public enum ItemViewer_Type_Enum : byte
    {
        /// <summary> Item is currently checked out and can not be viewed </summary>
        Checked_Out = 1,

        /// <summary> Citation of an item includes the basic metadata in standard and MARC format, as well as
        /// links to the metadata  </summary>
        Citation,

        /// <summary> List of downloads associated with this digital resource </summary>
        Download,

        /// <summary> [DEPRECATED?] Item is available as download ONLY </summary>
        Download_Only,

        /// <summary> Displays the container list for an archival EAD/Finding Guide type of item </summary>
        EAD_Container_List,

        /// <summary> Displays the description for an archival EAD/Finding Guide type of item </summary>
        EAD_Description,

        /// <summary> List of features associated with this resource, from the authority portion of the database</summary>
        Features,

        /// <summary> Displays a flash file within the SobekCM window </summary>
        Flash,

        /// <summary> Displays the item in a full-screen implementation of GnuBooks page turner </summary>
        GnuBooks_PageTurner,

        /// <summary> Google map display allows user to view the geographic coverage of this digital resource</summary>
        Google_Map,

        /// <summary> Displays a HTML file related to the digital resource, within the SobekCM window </summary>
        HTML,

        /// <summary> Displays a HTML file which also has a HTML Map element, for controlling the link out according to location on an image </summary>
        HTML_Map,

        /// <summary> JPEG2000 file allows the user to zoom in and pan on a JPEG2000 file related to this digital resource </summary>
        JPEG2000,

        /// <summary> Displays a static jpeg file related to this digital resource </summary>
        JPEG,

        /// <summary> Displays other issues related to the current digital resource by title / bib id </summary>
        MultiVolume,

        /// <summary> Embeds the PDF viewer into the SobekCM window for viewing a PDF related to this digital resource </summary>
        PDF,

        /// <summary> Shows thumbnails of all the images related to this digital resource  </summary>
        Related_Images,

        /// <summary> Item is restricted by IP address and is currently not accessible </summary>
        Restricted,

        /// <summary> Search options and/or results for the full-text of any pages within this document </summary>
        Search,

        /// <summary> List of streets associated with this resource, from the authority portion of the database</summary>
        Streets,

        /// <summary> Plain text view shows any text file associated with this digital resource, including OCR'd text </summary>
        Text,

        /// <summary> [DEPRECATED] Table of contents view shows the entire table of contents in the main item viewer area </summary>
        TOC,

        /// <summary> Displays the tracking information for a single digital resource </summary>
        Tracking,

        /// <summary> Displays a you tube video embedded within the digital resource  </summary>
        YouTube_Video
    }

	/// <summary> Interface which all item viewer objects must implement </summary>
	public interface iItemViewer
	{
		/// <summary> Sets the current mode which (may) tell how to display this item </summary>
		SobekCM_Navigation_Object CurrentMode { set; }

		/// <summary> Sets the current item for this viewer to display </summary>
		SobekCM_Item CurrentItem { set; }

		/// <summary> Sets the filename for this viewer (if this is a simple viewer)</summary>
		string FileName { set; }

		/// <summary> Sets the attributes for this viewer (from the database) </summary>
		string Attributes { set; }

	    /// <summary> Gets any HTML for a Navigation Row above the image or text </summary>
		string NavigationRow { get; }

	    /// <summary> Gets the number of pages for this viewer </summary>
		int PageCount { get; }

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

		/// <summary> Flag indicates if the header (with the title, group title, etc..) should be displayed </summary>
		bool Show_Header { get; }

        /// <summary> Width for the main viewer section to adjusted to accomodate this viewer</summary>
		int Viewer_Width { get; }

        /// <summary> Gets the current page for paging purposes </summary>
        int Current_Page { get; }

        /// <summary> Gets the flag that indicates if the page selector should be shown </summary>
        bool Show_Page_Selector { get; }

        /// <summary> Gets the type of item viewer </summary>
        ItemViewer_Type_Enum ItemViewer_Type { get; }

	    /// <summary> Adds any viewer_specific information to the Navigation Bar Menu Section </summary>
	    /// <param name="placeHolder"> Additional place holder ( &quot;navigationPlaceHolder&quot; ) in the itemNavForm form allows item-viewer-specific controls to be added to the left navigation bar</param>
	    /// <param name="Internet_Explorer"> Flag indicates if the current browser is internet explorer </param>
	    /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
	    /// <returns> TRUE if this viewer added something to the left navigational bar, otherwise FALSE</returns>
	    bool Add_Nav_Bar_Menu_Section( PlaceHolder placeHolder, bool Internet_Explorer, Custom_Tracer Tracer );

	    /// <summary> Adds the main view section to the page turner </summary>
	    /// <param name="placeHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the the bulk of the item viewer's output is displayed</param>
	    /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
	    void Add_Main_Viewer_Section(PlaceHolder placeHolder, Custom_Tracer Tracer);

	    /// <summary> This provides an opportunity for the viewer to perform any pre-display work
        /// which is necessary before entering any of the rendering portions </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        void Perform_PreDisplay_Work(Custom_Tracer Tracer);
	}
}
