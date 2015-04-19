namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the PagedResults_HtmlSubwriter class </summary>
    public class PagedResults_HtmlSubwriter_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the PagedResults_HtmlSubwriter_Localization class </summary>
        public PagedResults_HtmlSubwriter_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "PagedResults_HtmlSubwriter";
        }

        /// <summary> Adds a localization string ( with key and value ) to this localization class </summary>
        /// <param name="Key"> Key for the new localization string being saved </param>
        /// <param name="Value"> Value for this localization string </param>
        /// <remarks> This overrides the base class's implementation </remarks>
        public override void Add_Localization_String(string Key, string Value)
        {
            // First, add to the localization string dictionary
            base.Add_Localization_String(Key, Value);

            // Assign to custom properties depending on the key
            switch (Key)
            {
                case "Search Has Been Saved To Your Saved Searches":
                    SearchHasBeenSavedToYourSavedSearches = Value;
                    break;

                case "ERROR Encountered While Saving Your Search":
                    ERROREncounteredWhileSavingYourSearch = Value;
                    break;

                case "Your Email Has Been Sent":
                    YourEmailHasBeenSent = Value;
                    break;

                case "ERROR Encountered While Saving":
                    ERROREncounteredWhileSaving = Value;
                    break;

                case "Title":
                    Title = Value;
                    break;

                case "Rank":
                    Rank = Value;
                    break;

                case "Bibid Ascending":
                    BibidAscending = Value;
                    break;

                case "Bibid Descending":
                    BibidDescending = Value;
                    break;

                case "Date Ascending":
                    DateAscending = Value;
                    break;

                case "Date Descending":
                    DateDescending = Value;
                    break;

                case "Date Added":
                    DateAdded = Value;
                    break;

                case "Bookshelf View":
                    BookshelfView = Value;
                    break;

                case "Brief View":
                    BriefView = Value;
                    break;

                case "Map View":
                    MapView = Value;
                    break;

                case "Table View":
                    TableView = Value;
                    break;

                case "Thumbnail View":
                    ThumbnailView = Value;
                    break;

                case "Export":
                    Export = Value;
                    break;

                case "XXX XXX Of XXX Matching Titles":
                    XXXXXXOfXXXMatchingTitles = Value;
                    break;

                case "XXX XXX Of XXX Matching Coordinates":
                    XXXXXXOfXXXMatchingCoordinates = Value;
                    break;

                case "XXX XXX Of XXX Matching Flights":
                    XXXXXXOfXXXMatchingFlights = Value;
                    break;

                case "Public Bookshelf":
                    PublicBookshelf = Value;
                    break;

                case "Browse":
                    Browse = Value;
                    break;

                case "Search":
                    Search = Value;
                    break;

                case "XXX Items For Export":
                    XXXItemsForExport = Value;
                    break;

                case "Add This To Your Saved Searches":
                    AddThisToYourSavedSearches = Value;
                    break;

                case "Enter Notes For This Public Bookshelf":
                    EnterNotesForThisPublicBookshelf = Value;
                    break;

                case "Enter Notes For This Browse":
                    EnterNotesForThisBrowse = Value;
                    break;

                case "Enter Notes For This Search":
                    EnterNotesForThisSearch = Value;
                    break;

                case "Description":
                    Description = Value;
                    break;

                case "And":
                    And = Value;
                    break;

                case "Or":
                    Or = Value;
                    break;

                case "Not":
                    Not = Value;
                    break;

                case "Resulted In No Matching Records":
                    ResultedInNoMatchingRecords = Value;
                    break;

                case "Resulted In One Matching Record":
                    ResultedInOneMatchingRecord = Value;
                    break;

                case "Resulted In XXX Matching Records":
                    ResultedInXXXMatchingRecords = Value;
                    break;

                case "Resulted In One Item In":
                    ResultedInOneItemIn = Value;
                    break;

                case "Resulted In XXX Items In":
                    ResultedInXXXItemsIn = Value;
                    break;

                case "One Title":
                    OneTitle = Value;
                    break;

                case "Titles":
                    Titles = Value;
                    break;

                case "Resulted In No Matching Flights":
                    ResultedInNoMatchingFlights = Value;
                    break;

                case "Resulted In One Matching Flight":
                    ResultedInOneMatchingFlight = Value;
                    break;

                case "Resulted In XXX Matching Flights":
                    ResultedInXXXMatchingFlights = Value;
                    break;

                case "Resulted In One Flight In":
                    ResultedInOneFlightIn = Value;
                    break;

                case "Resulted In XXX Flights In":
                    ResultedInXXXFlightsIn = Value;
                    break;

                case "One County":
                    OneCounty = Value;
                    break;

                case "Counties":
                    Counties = Value;
                    break;

                case "Your Search Of XXX For":
                    YourSearchOfXXXFor = Value;
                    break;

                case "Your Geographic Search Of XXX":
                    YourGeographicSearchOfXXX = Value;
                    break;

                case "In":
                    In = Value;
                    break;

                case "NARROW RESULTS BY":
                    NARROWRESULTSBY = Value;
                    break;

                case "Show More":
                    ShowMore = Value;
                    break;

                case "Show Less":
                    ShowLess = Value;
                    break;

                case "Sort Alphabetically":
                    SortAlphabetically = Value;
                    break;

                case "Sort By Frequency":
                    SortByFrequency = Value;
                    break;

                case "Sort By":
                    SortBy = Value;
                    break;

                case "BRIEF":
                    BRIEF = Value;
                    break;

                case "THUMB":
                    THUMB = Value;
                    break;

                case "Enter The Email Inforamtion Below":
                    EnterTheEmailInforamtionBelow = Value;
                    break;

                case "To":
                    To = Value;
                    break;

                case "Comments":
                    Comments = Value;
                    break;

                case "HTML":
                    HTML = Value;
                    break;

                case "Plain Text":
                    PlainText = Value;
                    break;

                case "UNRECOGNIZED SEARCH":
                    UNRECOGNIZEDSEARCH = Value;
                    break;

            }
        }
        /// <remarks> 'Search has been saved to your saved searches.' localization string </remarks>
        public string SearchHasBeenSavedToYourSavedSearches { get; private set; }

        /// <remarks> 'ERROR encountered while saving your search!' localization string </remarks>
        public string ERROREncounteredWhileSavingYourSearch { get; private set; }

        /// <remarks> 'Your email has been sent' localization string </remarks>
        public string YourEmailHasBeenSent { get; private set; }

        /// <remarks> 'ERROR encountered while saving!' localization string </remarks>
        public string ERROREncounteredWhileSaving { get; private set; }

        /// <remarks> 'Title' localization string </remarks>
        public string Title { get; private set; }

        /// <remarks> 'Rank' localization string </remarks>
        public string Rank { get; private set; }

        /// <remarks> 'BibID Ascending' localization string </remarks>
        public string BibidAscending { get; private set; }

        /// <remarks> 'BibID Descending' localization string </remarks>
        public string BibidDescending { get; private set; }

        /// <remarks> 'Date Ascending' localization string </remarks>
        public string DateAscending { get; private set; }

        /// <remarks> 'Date Descending' localization string </remarks>
        public string DateDescending { get; private set; }

        /// <remarks> 'Date Added' localization string </remarks>
        public string DateAdded { get; private set; }

        /// <remarks> 'Bookshelf View' localization string </remarks>
        public string BookshelfView { get; private set; }

        /// <remarks> 'Brief View' localization string </remarks>
        public string BriefView { get; private set; }

        /// <remarks> 'Map View' localization string </remarks>
        public string MapView { get; private set; }

        /// <remarks> 'Table View' localization string </remarks>
        public string TableView { get; private set; }

        /// <remarks> 'Thumbnail View' localization string </remarks>
        public string ThumbnailView { get; private set; }

        /// <remarks> 'Export' localization string </remarks>
        public string Export { get; private set; }

        /// <remarks> '{0} - {1} of {2} matching titles' localization string </remarks>
        public string XXXXXXOfXXXMatchingTitles { get; private set; }

        /// <remarks> '{0} - {1} of {2} matching coordinates' localization string </remarks>
        public string XXXXXXOfXXXMatchingCoordinates { get; private set; }

        /// <remarks> '{0} - {1} of {2} matching flights' localization string </remarks>
        public string XXXXXXOfXXXMatchingFlights { get; private set; }

        /// <remarks> 'Public Bookshelf' localization string </remarks>
        public string PublicBookshelf { get; private set; }

        /// <remarks> 'Browse' localization string </remarks>
        public string Browse { get; private set; }

        /// <remarks> 'Search' localization string </remarks>
        public string Search { get; private set; }

        /// <remarks> '{1} items for export' localization string </remarks>
        public string XXXItemsForExport { get; private set; }

        /// <remarks> 'Add this to your Saved Searches' localization string </remarks>
        public string AddThisToYourSavedSearches { get; private set; }

        /// <remarks> 'Enter notes for this public bookshelf' localization string </remarks>
        public string EnterNotesForThisPublicBookshelf { get; private set; }

        /// <remarks> 'Enter notes for this browse' localization string </remarks>
        public string EnterNotesForThisBrowse { get; private set; }

        /// <remarks> 'Enter notes for this search' localization string </remarks>
        public string EnterNotesForThisSearch { get; private set; }

        /// <remarks> 'Description:' localization string </remarks>
        public string Description { get; private set; }

        /// <remarks> 'and' localization string </remarks>
        public string And { get; private set; }

        /// <remarks> 'or' localization string </remarks>
        public string Or { get; private set; }

        /// <remarks> 'not' localization string </remarks>
        public string Not { get; private set; }

        /// <remarks> 'resulted in no matching records.' localization string </remarks>
        public string ResultedInNoMatchingRecords { get; private set; }

        /// <remarks> 'resulted in one matching record.' localization string </remarks>
        public string ResultedInOneMatchingRecord { get; private set; }

        /// <remarks> 'resulted in {0} matching records.' localization string </remarks>
        public string ResultedInXXXMatchingRecords { get; private set; }

        /// <remarks> 'resulted in one item in ' localization string </remarks>
        public string ResultedInOneItemIn { get; private set; }

        /// <remarks> 'resulted in {0} items in' localization string </remarks>
        public string ResultedInXXXItemsIn { get; private set; }

        /// <remarks> 'one title' localization string </remarks>
        public string OneTitle { get; private set; }

        /// <remarks> 'titles' localization string </remarks>
        public string Titles { get; private set; }

        /// <remarks> 'resulted in no matching flights.' localization string </remarks>
        public string ResultedInNoMatchingFlights { get; private set; }

        /// <remarks> 'resulted in one matching flight.' localization string </remarks>
        public string ResultedInOneMatchingFlight { get; private set; }

        /// <remarks> 'resulted in {0} matching flights.' localization string </remarks>
        public string ResultedInXXXMatchingFlights { get; private set; }

        /// <remarks> 'resulted in one flight in ' localization string </remarks>
        public string ResultedInOneFlightIn { get; private set; }

        /// <remarks> 'resulted in {0} flights in ' localization string </remarks>
        public string ResultedInXXXFlightsIn { get; private set; }

        /// <remarks> 'one county' localization string </remarks>
        public string OneCounty { get; private set; }

        /// <remarks> 'counties' localization string </remarks>
        public string Counties { get; private set; }

        /// <remarks> 'Your search of {0} for ' localization string </remarks>
        public string YourSearchOfXXXFor { get; private set; }

        /// <remarks> 'Your geographic search of {0}' localization string </remarks>
        public string YourGeographicSearchOfXXX { get; private set; }

        /// <remarks> 'in ' localization string </remarks>
        public string In { get; private set; }

        /// <remarks> 'NARROW RESULTS BY' localization string </remarks>
        public string NARROWRESULTSBY { get; private set; }

        /// <remarks> 'Show More' localization string </remarks>
        public string ShowMore { get; private set; }

        /// <remarks> 'Show Less' localization string </remarks>
        public string ShowLess { get; private set; }

        /// <remarks> 'Sort alphabetically' localization string </remarks>
        public string SortAlphabetically { get; private set; }

        /// <remarks> 'Sort by frequency' localization string </remarks>
        public string SortByFrequency { get; private set; }

        /// <remarks> 'Sort by' localization string </remarks>
        public string SortBy { get; private set; }

        /// <remarks> 'BRIEF' localization string </remarks>
        public string BRIEF { get; private set; }

        /// <remarks> 'THUMB' localization string </remarks>
        public string THUMB { get; private set; }

        /// <remarks> 'Enter the email inforamtion below' localization string </remarks>
        public string EnterTheEmailInforamtionBelow { get; private set; }

        /// <remarks> 'To:' localization string </remarks>
        public string To { get; private set; }

        /// <remarks> 'Comments:' localization string </remarks>
        public string Comments { get; private set; }

        /// <remarks> 'HTML' localization string </remarks>
        public string HTML { get; private set; }

        /// <remarks> 'Plain Text' localization string </remarks>
        public string PlainText { get; private set; }

        /// <remarks> 'UNRECOGNIZED SEARCH' localization string </remarks>
        public string UNRECOGNIZEDSEARCH { get; private set; }

    }
}
