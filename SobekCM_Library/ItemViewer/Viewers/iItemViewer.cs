#region Using directives

using System.Collections.Generic;
using System.IO;
using System.Web.UI.WebControls;
using SobekCM.Library.HTML;
using SobekCM.Resource_Object;
using SobekCM.Library.Navigation;

#endregion

namespace SobekCM.Library.ItemViewer.Viewers
{
    /// <summary> Delegate defines a signature for an event used by item viewers for requesting
    /// an immediate redirect of the user's browser. </summary>
    /// <param name="NewURL"> URL to forward the user to </param>
	public delegate void Redirect_Requested( string NewURL );

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

    /// <summary> Enumeration of the different item viewer types for displaying an item in HTML </summary>
    public enum ItemViewer_Type_Enum : byte
    {
        /// <summary> Item is currently checked out and can not be viewed </summary>
        Checked_Out = 1,

        /// <summary> Citation of an item includes the basic metadata in standard and MARC format, as well as
        /// links to the metadata  </summary>
        Citation,

		/// <summary> Dataset viewer is used to dispay codebook information present in the XSD accompanying a XML dataset </summary>
		Dataset_Codebook,

		/// <summary> Dataset viewer is used to view saved reports or create new custom reports </summary>
		Dataset_Reports,

		/// <summary> Dataset viewer shows the paged data from the dataset and allows simple searching/filtering </summary>
		Dataset_ViewData,

        /// <summary> List of downloads associated with this digital resource </summary>
        Download,

        /// <summary> [DEPRECATED?] Item is available as download ONLY </summary>
        Download_Only,

        /// <summary> Displays the container list for an archival EAD/Finding Guide type of item </summary>
        EAD_Container_List,

        /// <summary> Displays the description for an archival EAD/Finding Guide type of item </summary>
        EAD_Description,

        /// <summary> Embedded video viewer </summary>
        Embedded_Video,

        /// <summary> List of features associated with this resource, from the authority portion of the database</summary>
        Features,

        /// <summary> Displays a flash file within the SobekCM window </summary>
        Flash,

        /// <summary> Fragment renders the add/remove form for adding/removing items from a bookshelf to be included on demand in an item view </summary>
        Fragment_AddForm,

        /// <summary> Fragment renders the describe form for adding/removing items from a bookshelf to be included on demand in an item view </summary>
        Fragment_DescribeForm,

        /// <summary> Fragment renders the print form to be included on demand in an item view </summary>
        Fragment_PrintForm,

        /// <summary> Fragment renders the send/email form to be included on demand in an item view </summary>
        Fragment_SendForm,

        /// <summary> Fragment renders the share (social media) form to be included on demand in an item view </summary>
        Fragment_ShareForm,

        /// <summary> Displays the item in a full-screen implementation of GnuBooks page turner </summary>
        GnuBooks_PageTurner,

        /// <summary> Allow a user to enter coordinate information ( i.e., map coverage, points of interest, etc.. ) </summary>
        Google_Coordinate_Entry,

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

		/// <summary> Viewer is used to view the jpeg image and view (and possibly edit) the 
		/// full text derived from that page image </summary>
		JPEG_Text_Two_Up,

		/// <summary> Viewers shows the manage menu which tells the user all the different options
		/// they have have to manage a single digital resource </summary>
		Manage,

        /// <summary> Displays other issues related to the current digital resource by title / bib id </summary>
        MultiVolume,

        /// <summary> Embeds the PDF viewer into the SobekCM window for viewing a PDF related to this digital resource </summary>
        PDF,

        /// <summary> Quality control online </summary>
        Quality_Control,

        /// <summary> Shows thumbnails of all the images related to this digital resource  </summary>
        Related_Images,

        /// <summary> Item is restricted by IP address and is currently not accessible </summary>
        Restricted,

        /// <summary> Search options and/or results for the full-text of any pages within this document </summary>
        Search,

        /// <summary> List of streets associated with this resource, from the authority portion of the database</summary>
        Streets,

        /// <summary> Used for special tests on item viewer dynamics </summary>
        Test,

        /// <summary> Plain text view shows any text file associated with this digital resource, including OCR'd text </summary>
        Text,

        /// <summary> [DEPRECATED] Table of contents view shows the entire table of contents in the main item viewer area </summary>
        TOC,

        /// <summary> Displays the tracking information for a single digital resource </summary>
        Tracking,

        /// <summary>Displays the tracking information for a single item (Bib/VID) </summary>
        Tracking_Sheet,

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

        /// <summary> Width for the main viewer section to adjusted to accomodate this viewer</summary>
		int Viewer_Width { get; }

        /// <summary> Height for the main viewer section to adjusted to accomodate this viewer</summary>
        int Viewer_Height { get; }

        /// <summary> Gets the current page for paging purposes </summary>
        int Current_Page { get; }

        /// <summary> Gets the collection of special behaviors which this item viewer
        /// requests from the main HTML subwriter. </summary>
        List<HtmlSubwriter_Behaviors_Enum> ItemViewer_Behaviors { get; }

        /// <summary> Gets the flag that indicates if the page selector should be shown </summary>
        ItemViewer_PageSelector_Type_Enum Page_Selector { get; }

        /// <summary> Gets the type of item viewer </summary>
        ItemViewer_Type_Enum ItemViewer_Type { get; }

        /// <summary> Adds any viewer_specific information to the left Navigation Bar Menu Section  </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        void Write_Left_Nav_Menu_Section(TextWriter Output, Custom_Tracer Tracer);

        /// <summary> Adds any viewer_specific information to the item viewer above the standard pagination buttons </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        void Write_Top_Additional_Navigation_Row(TextWriter Output, Custom_Tracer Tracer);

	    /// <summary> Adds the main view section to the page turner </summary>
	    /// <param name="MainPlaceHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the the bulk of the item viewer's output is displayed</param>
	    /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
	    void Add_Main_Viewer_Section(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer);

        /// <summary> Stream to which to write the HTML for this subwriter  </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer);

	    /// <summary> This provides an opportunity for the viewer to perform any pre-display work
        /// which is necessary before entering any of the rendering portions </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        void Perform_PreDisplay_Work(Custom_Tracer Tracer);
	}
}
