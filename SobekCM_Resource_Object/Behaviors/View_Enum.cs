namespace SobekCM.Resource_Object.Behaviors
{
    /// <summary> Enumeration of the possible 'views' for a digital resource or page </summary>
    /// <remarks> Object created by Mark V Sullivan (2006) for University of Florida's Digital Library Center. </remarks>
    public enum View_Enum
    {
        /// <summary> No view (this is generally not used </summary>
        None = -1,

        /// <summary> List of all volumes related to the current volume's title (i.e., all VIDs in the same BibID )</summary>
        ALL_VOLUMES = 1,

        /// <summary> Citation viewer shows the citation in various formats </summary>
        CITATION,

        /// <summary> Downloads viewer shows the list of possible downloads for a digital resource </summary>
        DOWNLOADS,

		/// <summary> Dataset viewer is used to dispay codebook information present in the XSD accompanying a XML dataset </summary>
		DATASET_CODEBOOK,

		/// <summary> Dataset viewer is used to view saved reports or create new custom reports </summary>
		DATASET_REPORTS,

		/// <summary> Dataset viewer shows the paged data from the dataset and allows simple searching/filtering </summary>
		DATASET_VIEWDATA,

        /// <summary> Container list viewer for an EAD/Finding guide item with a container list </summary>
        EAD_CONTAINER_LIST,

        /// <summary> Descirption viewer for an EAD/Finding guide item </summary>
        EAD_DESCRIPTION,

        /// <summary> Ephemeral Cities (project-specific) resource viewer </summary>
        EPHEMERAL_CITIES,

        /// <summary> Resource viewer shows features from the authority
        /// database portion which are linked to this digital resource </summary>
        FEATURES,

        /// <summary> Flash viewer is used to display a flash file for a digital resource </summary>
        FLASH,

        /// <summary> Google coordinate edit view (restricted to item editors) </summary>
        GOOGLE_COORDINATE_ENTRY,
            
        /// <summary> Google map view shows the coverage of a digital resource graphically </summary>
        GOOGLE_MAP,

        /// <summary> Resource views shows a static HTML page </summary>
        HTML,

        /// <summary> Static JPEG page viewer </summary>
        JPEG,

		/// <summary> Viewer is used to view the jpeg image and view (and possibly edit) the 
		/// full text derived from that page image </summary>
		JPEG_TEXT_TWO_UP,

        /// <summary> Zoomable JPEG2000 page viewer </summary>
        JPEG2000,

        /// <summary> Page turner which allows the user to step through each jpeg in a format that resembles a traditional book </summary>
        PAGE_TURNER,

        /// <summary> PDF viewer is used to view an embedded Adobe PDF file for a digital resource </summary>
        PDF,

        /// <summary> Related Images resource viewer </summary>
        RELATED_IMAGES,

        /// <summary> Restricted viewer is displayed when the item is restricted by IP address </summary>
        RESTRICTED,

        /// <summary> Quality control viewer (restricted to item editors)  </summary>
        QUALITY_CONTROL,

        /// <summary> Sanborn (project-specific) resource viewer </summary>
        SANBORN,

        /// <summary> Search page to view search options and/or search results at the single item level </summary>
        SEARCH,

        /// <summary> This creates a tab which looks like a view tab, but is really just a link out to a seperate html page </summary>
        SIMPLE_HTML_LINK,

        /// <summary> Resource viewer shows streets from the authority
        /// database portion which are linked to this digital resource </summary>
        STREETS,

        /// <summary> Text Encoding Initiative display </summary>
        TEI,

        /// <summary> Test viewer used during development for different things </summary>
        TEST,

        /// <summary> Text page viewer </summary>
        TEXT,

        /// <summary> Table of contents view lists the table of contents with links to the beginning of each division </summary>
        TOC,

        /// <summary> Displays the tracking information for a single digital resource </summary>
        TRACKING,

        /// <summary>Displays the tracking sheet for the object being digitized</summary>
        TRACKING_SHEET,

        /// <summary> Displays a you tube video embedded within the digital resource </summary>
        YOUTUBE_VIDEO,

		/// <summary> Generic embedded video displayer is used for displaying any streaming video in an iFrame </summary>
        EMBEDDED_VIDEO
    }
}