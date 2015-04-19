namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Statistics_HtmlSubwriter class </summary>
    public class Statistics_HtmlSubwriter_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Statistics_HtmlSubwriter_Localization class </summary>
        public Statistics_HtmlSubwriter_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Statistics_HtmlSubwriter";
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
                case "Item Count":
                    ItemCount = Value;
                    break;

                case "Usage Statistics":
                    UsageStatistics = Value;
                    break;

                case "Recent Searches":
                    RecentSearches = Value;
                    break;

                case "Resource Count In XXX":
                    ResourceCountInXXX = Value;
                    break;

                case "Recent Searches In XXX":
                    RecentSearchesInXXX = Value;
                    break;

                case "Usage Statistics For XXX":
                    UsageStatisticsForXXX = Value;
                    break;

                case "Standard View":
                    StandardView = Value;
                    break;

                case "FYTD Growth View":
                    FYTDGrowthView = Value;
                    break;

                case "Arbitrary Dates":
                    ArbitraryDates = Value;
                    break;

                case "Overall Stats":
                    OverallStats = Value;
                    break;

                case "Collections By Date":
                    CollectionsByDate = Value;
                    break;

                case "Item Views By Date":
                    ItemViewsByDate = Value;
                    break;

                case "Collection History":
                    CollectionHistory = Value;
                    break;

                case "Top Titles":
                    TopTitles = Value;
                    break;

                case "Definitions":
                    Definitions = Value;
                    break;

                case "Browse Partners":
                    BrowsePartners = Value;
                    break;

                case "Advanced Search":
                    AdvancedSearch = Value;
                    break;

                case "The Most Commonly Accessed Items By Collection Appear Below":
                    TheMostCommonlyAccessedItemsByCollectionApp = Value;
                    break;

                case "The Definitions Page Provides More Details About The Statistics And Words Used Below":
                    TheDefinitionsPageProvidesMoreDetailsAboutT = Value;
                    break;

                case "Selected Collection":
                    SelectedCollection = Value;
                    break;

                case "The Most Commonly Accessed Items Below Are Displayed For The Following Collection":
                    TheMostCommonlyAccessedItemsBelowAreDisplay = Value;
                    break;

                case "From":
                    From = Value;
                    break;

                case "To Change The Collection Shown Choose The Collection Above And Hit The GO Button":
                    ToChangeTheCollectionShownChooseTheCollecti = Value;
                    break;

                case "Most Accessed Items":
                    MostAccessedItems = Value;
                    break;

                case "The Data Below Shows The Most Commonly Accessed Items Within The Collection Above":
                    TheDataBelowShowsTheMostCommonlyAccessedIt = Value;
                    break;

                case "Click Here To View The Most Commonly Accessed TITLES":
                    ClickHereToViewTheMostCommonlyAccessedTITL = Value;
                    break;

                case "Views":
                    Views = Value;
                    break;

                case "The Most Commonly Utilized Titles By Collection Appear Below":
                    TheMostCommonlyUtilizedTitlesByCollectionAp = Value;
                    break;

                case "The Most Commonly Accessed Titles Below Are Displayed Is For The Following Collection":
                    TheMostCommonlyAccessedTitlesBelowAreDispla = Value;
                    break;

                case "Most Accessed Titles":
                    MostAccessedTitles = Value;
                    break;

                case "The Data Below Shows The Most Commonly Accessed Titles Within The Collection Above":
                    TheDataBelowShowsTheMostCommonlyAccessedTi = Value;
                    break;

                case "Click Here To View The Most Commonly Accessed ITEMS":
                    ClickHereToViewTheMostCommonlyAccessedITEM = Value;
                    break;

                case "Below Is The Collection And Itemlevel Details For Your Provided Date Range In Commaseperated Value Form To Use The Data Below Cut And Paste It Into A CSV Or Text File The Resulting File Can Be Opened In A Variety Of Applications Including Openoffice And Microsoft Excel":
                    BelowIsTheCollectionAndItemlevelDetailsFor = Value;
                    break;

                case "Select Collection":
                    SelectCollection = Value;
                    break;

                case "By Name":
                    ByName = Value;
                    break;

                case "Below Are The Details About The Specialized Views For Items Aggregated At The Collection Levels":
                    BelowAreTheDetailsAboutTheSpecializedViews = Value;
                    break;

                case "For The Number Of Times Collections Were Viewed Or Searched See Collections By Date":
                    ForTheNumberOfTimesCollectionsWereViewedOr = Value;
                    break;

                case "Selected Date Range":
                    SelectedDateRange = Value;
                    break;

                case "The Usage For All Items Appears Below For The Following Date Range":
                    TheUsageForAllItemsAppearsBelowForTheFoll = Value;
                    break;

                case "To":
                    To = Value;
                    break;

                case "To Change The Date Shown Choose Your Dates Above And Hit The GO Button":
                    ToChangeTheDateShownChooseYourDatesAboveA = Value;
                    break;

                case "Summary By Collection":
                    SummaryByCollection = Value;
                    break;

                case "NO USAGE STATISTICS EXIST FOR YOUR SELECTION":
                    NOUSAGESTATISTICSEXISTFORYOURSELECTION = Value;
                    break;

                case "Export As CSV":
                    ExportAsCSV = Value;
                    break;

                case "JPEG Views":
                    JPEGViews = Value;
                    break;

                case "Zoomable Views":
                    ZoomableViews = Value;
                    break;

                case "Citation Views":
                    CitationViews = Value;
                    break;

                case "Thumbnail Views":
                    ThumbnailViews = Value;
                    break;

                case "Text Searches":
                    TextSearches = Value;
                    break;

                case "Flash Views":
                    FlashViews = Value;
                    break;

                case "Map Views":
                    MapViews = Value;
                    break;

                case "Download Views":
                    DownloadViews = Value;
                    break;

                case "Static Views":
                    StaticViews = Value;
                    break;

                case "Collections":
                    Collections = Value;
                    break;

                case "Type":
                    Type = Value;
                    break;

                case "Terms":
                    Terms = Value;
                    break;

                case "Time":
                    Time = Value;
                    break;

                case "Title":
                    Title = Value;
                    break;

                case "NUMBER":
                    NUMBER = Value;
                    break;

                case "The Data Below Shows Details For The Views That Occurred At Each Level Of The Collection Hierarchy Collection Groups Collections Subcollections":
                    TheDataBelowShowsDetailsForTheViewsThatOc = Value;
                    break;

                case "These Statistics Show The Number Of Times Users":
                    TheseStatisticsShowTheNumberOfTimesUsers = Value;
                    break;

                case "Navigated To The Collection Main Pages":
                    NavigatedToTheCollectionMainPages = Value;
                    break;

                case "Browsed The Items Or Information About The Collection":
                    BrowsedTheItemsOrInformationAboutTheCollect = Value;
                    break;

                case "Performed Searches Against The Items Contained In The Collection":
                    PerformedSearchesAgainstTheItemsContainedIn = Value;
                    break;

                case "Viewed Titles And Items Contained Within The Collection":
                    ViewedTitlesAndItemsContainedWithinTheColle = Value;
                    break;

                case "For The Specialized Itemlevel View Details See Item Views By Date":
                    ForTheSpecializedItemlevelViewDetailsSeeIte = Value;
                    break;

                case "The Usage For All The Collections Appears Below For The Following Data Range":
                    TheUsageForAllTheCollectionsAppearsBelowFo = Value;
                    break;

                case "GROUP CODE":
                    GROUPCODE = Value;
                    break;

                case "COLL CODE":
                    COLLCODE = Value;
                    break;

                case "SUB CODE":
                    SUBCODE = Value;
                    break;

                case "TOTAL VIEWS":
                    TOTALVIEWS = Value;
                    break;

                case "VISITS":
                    VISITS = Value;
                    break;

                case "MAIN PAGES":
                    MAINPAGES = Value;
                    break;

                case "BROWSES":
                    BROWSES = Value;
                    break;

                case "SEARCH RESULTS":
                    SEARCHRESULTS = Value;
                    break;

                case "TITLE VIEWS":
                    TITLEVIEWS = Value;
                    break;

                case "ITEM VIEWS":
                    ITEMVIEWS = Value;
                    break;

                case "ALL COLLECTION GROUPS":
                    ALLCOLLECTIONGROUPS = Value;
                    break;

                case "Usage History For A Single Collection Is Displayed Below This History Includes Views Of The Collection And Views Of The Items Within The Collections":
                    UsageHistoryForASingleCollectionIsDisplayed = Value;
                    break;

                case "The Usage Displayed Is For The Following Collection":
                    TheUsageDisplayedIsForTheFollowingCollectio = Value;
                    break;

                case "The Data Below Shows The Collection History For The Selected Collection The First Table Shows The Summary Of All Views Of This Collection And Items Contained In The Collection The Second Table Includes The Details For Specialized Itemlevel Views":
                    TheDataBelowShowsTheCollectionHistoryForTh = Value;
                    break;

                case "Below Is The History Collection Information Included In Commaseperated Value Form To Use The Data Below Cut And Paste It Into A CSV Or Text File The Resulting File Can Be Opened In A Variety Of Applications Including Openoffice And Microsoft Excel":
                    BelowIsTheHistoryCollectionInformationInclud = Value;
                    break;

                case "MONTH":
                    MONTH = Value;
                    break;

                case "Titles And Items":
                    TitlesAndItems = Value;
                    break;

                case "Below Are The Most Up To Date Numbers For Overall Utilization Of XXX The First Table Shows The Summary Of Views Against All Collections The Second Table Includes The Details For Specialized Itemlevel Views":
                    BelowAreTheMostUpToDateNumbersForOverall = Value;
                    break;

                case "NO ITEM COUNT AVAILABLE":
                    NOITEMCOUNTAVAILABLE = Value;
                    break;

                case "Below Are The Number Of Items In Each Collection And Subcollection":
                    BelowAreTheNumberOfItemsInEachCollectionA = Value;
                    break;

                case "TITLES":
                    TITLES = Value;
                    break;

                case "ITEMS":
                    ITEMS = Value;
                    break;

                case "PAGES":
                    PAGES = Value;
                    break;

                case "FYTD TITLES":
                    FYTDTITLES = Value;
                    break;

                case "FYTD ITEMS":
                    FYTDITEMS = Value;
                    break;

                case "FYTD PAGES":
                    FYTDPAGES = Value;
                    break;

                case "List View":
                    ListView = Value;
                    break;

                case "Brief View":
                    BriefView = Value;
                    break;

                case "Tree View":
                    TreeView = Value;
                    break;

                case "Bibid":
                    Bibid = Value;
                    break;

                case "VID":
                    VID = Value;
                    break;

                case "The Following Terms Are Defined Below":
                    TheFollowingTermsAreDefinedBelow = Value;
                    break;

                case "Collection Hierarchy":
                    CollectionHierarchy = Value;
                    break;

                case "Collection Groups":
                    CollectionGroups = Value;
                    break;

                case "Subcollections":
                    Subcollections = Value;
                    break;

                case "NO STATISTICS FOUND FOR THAT DATE RANGE":
                    NOSTATISTICSFOUNDFORTHATDATERANGE = Value;
                    break;

                case "Below Is The Item Count With Fiscal Year To Date Information Included In Commaseperated Value Form To Use The Data Below Cut And Paste It Into A CSV Or Text File The Resulting File Can Be Opened In A Variety Of Applications Including Openoffice And Microsoft Excel":
                    BelowIsTheItemCountWithFiscalYearToDateI = Value;
                    break;

                case "Defined Terms":
                    DefinedTerms = Value;
                    break;

                case "CODES":
                    CODES = Value;
                    break;

                case "NAME":
                    NAME = Value;
                    break;

                case "TOTAL":
                    TOTAL = Value;
                    break;

                case "This Option Allows The Complete Title Count Item Count And Page Count To Be Viewed For A Previous Time And To Additionally See The Growth Between Two Arbitrary Dates":
                    ThisOptionAllowsTheCompleteTitleCountItemC = Value;
                    break;

                case "The Title Count Item Count And Page Count Appear Below For The Following Two Arbitrary Dates":
                    TheTitleCountItemCountAndPageCountAppearB = Value;
                    break;

                case "To Change The Dates Shown Choose Your Dates Above And Hit The GO Button":
                    ToChangeTheDatesShownChooseYourDatesAbove = Value;
                    break;

                case "Item Count And Growth":
                    ItemCountAndGrowth = Value;
                    break;

                case "Below Are The Number Of Titles Items And Pages In Each Collection And Subcollection The Quotinitialquot Values Are The Values That Were Present At Midnight On XXX Before Any New Items Were Processed That Day":
                    BelowAreTheNumberOfTitlesItemsAndPagesIn = Value;
                    break;

                case "The Quotaddedquot Values Are The Number Of Titles Items And Pages That Were Added Between XXX And XXX Inclusive":
                    TheQuotaddedquotValuesAreTheNumberOfTitles = Value;
                    break;

                case "INITIAL TITLES":
                    INITIALTITLES = Value;
                    break;

                case "INITIAL ITEMS":
                    INITIALITEMS = Value;
                    break;

                case "INITIAL PAGES":
                    INITIALPAGES = Value;
                    break;

            }
        }
        /// <remarks> 'Item Count' localization string </remarks>
        public string ItemCount { get; private set; }

        /// <remarks> 'Usage Statistics' localization string </remarks>
        public string UsageStatistics { get; private set; }

        /// <remarks> 'Recent Searches' localization string </remarks>
        public string RecentSearches { get; private set; }

        /// <remarks> 'Resource count in %1' localization string </remarks>
        public string ResourceCountInXXX { get; private set; }

        /// <remarks> 'Recent searches in %1' localization string </remarks>
        public string RecentSearchesInXXX { get; private set; }

        /// <remarks> 'Usage statistics for %1' localization string </remarks>
        public string UsageStatisticsForXXX { get; private set; }

        /// <remarks> 'Standard View' localization string </remarks>
        public string StandardView { get; private set; }

        /// <remarks> 'FYTD Growth View' localization string </remarks>
        public string FYTDGrowthView { get; private set; }

        /// <remarks> 'Arbitrary Dates' localization string </remarks>
        public string ArbitraryDates { get; private set; }

        /// <remarks> 'Overall Stats' localization string </remarks>
        public string OverallStats { get; private set; }

        /// <remarks> 'Collections by Date' localization string </remarks>
        public string CollectionsByDate { get; private set; }

        /// <remarks> 'Item Views by Date' localization string </remarks>
        public string ItemViewsByDate { get; private set; }

        /// <remarks> 'Collection History' localization string </remarks>
        public string CollectionHistory { get; private set; }

        /// <remarks> 'Top Titles' localization string </remarks>
        public string TopTitles { get; private set; }

        /// <remarks> 'Definitions' localization string </remarks>
        public string Definitions { get; private set; }

        /// <remarks> 'Browse Partners' localization string </remarks>
        public string BrowsePartners { get; private set; }

        /// <remarks> 'Advanced Search' localization string </remarks>
        public string AdvancedSearch { get; private set; }

        /// <remarks> 'The most commonly accessed items by collection appear below.' localization string </remarks>
        public string TheMostCommonlyAccessedItemsByCollectionApp { get; private set; }

        /// <remarks> 'The Definitions page provides more details about the statistics and words used below.' localization string </remarks>
        public string TheDefinitionsPageProvidesMoreDetailsAboutT { get; private set; }

        /// <remarks> 'Selected Collection' localization string </remarks>
        public string SelectedCollection { get; private set; }

        /// <remarks> 'The most commonly accessed items below are displayed for the following collection:' localization string </remarks>
        public string TheMostCommonlyAccessedItemsBelowAreDisplay { get; private set; }

        /// <remarks> 'From:' localization string </remarks>
        public string From { get; private set; }

        /// <remarks> '"To change the collection shown, choose the collection above and hit the GO button."' localization string </remarks>
        public string ToChangeTheCollectionShownChooseTheCollecti { get; private set; }

        /// <remarks> 'Most Accessed Items' localization string </remarks>
        public string MostAccessedItems { get; private set; }

        /// <remarks> 'The data below shows the most commonly accessed items within the collection above.' localization string </remarks>
        public string TheDataBelowShowsTheMostCommonlyAccessedIt { get; private set; }

        /// <remarks> 'Click here to view the most commonly accessed TITLES' localization string </remarks>
        public string ClickHereToViewTheMostCommonlyAccessedTITL { get; private set; }

        /// <remarks> 'Views' localization string </remarks>
        public string Views { get; private set; }

        /// <remarks> 'The most commonly utilized titles by collection appear below.' localization string </remarks>
        public string TheMostCommonlyUtilizedTitlesByCollectionAp { get; private set; }

        /// <remarks> 'The most commonly accessed titles below are displayed is for the following collection:' localization string </remarks>
        public string TheMostCommonlyAccessedTitlesBelowAreDispla { get; private set; }

        /// <remarks> 'Most Accessed Titles' localization string </remarks>
        public string MostAccessedTitles { get; private set; }

        /// <remarks> 'The data below shows the most commonly accessed titles within the collection above.' localization string </remarks>
        public string TheDataBelowShowsTheMostCommonlyAccessedTi { get; private set; }

        /// <remarks> 'Click here to view the most commonly accessed ITEMS' localization string </remarks>
        public string ClickHereToViewTheMostCommonlyAccessedITEM { get; private set; }

        /// <remarks> '"Below is the collection and item-level details for your provided date range in comma-seperated value form.  To use the data below, cut and paste it into a CSV or text file.  The resulting file can be opened in a variety of applications, including OpenOffice and Microsoft Excel."' localization string </remarks>
        public string BelowIsTheCollectionAndItemlevelDetailsFor { get; private set; }

        /// <remarks> 'Select Collection' localization string </remarks>
        public string SelectCollection { get; private set; }

        /// <remarks> 'By Name' localization string </remarks>
        public string ByName { get; private set; }

        /// <remarks> 'Below are the details about the specialized views for items aggregated at the collection levels.' localization string </remarks>
        public string BelowAreTheDetailsAboutTheSpecializedViews { get; private set; }

        /// <remarks> '"For the number of times collections were viewed or searched, see Collections by Date."' localization string </remarks>
        public string ForTheNumberOfTimesCollectionsWereViewedOr { get; private set; }

        /// <remarks> 'Selected Date Range' localization string </remarks>
        public string SelectedDateRange { get; private set; }

        /// <remarks> 'The usage for all items appears below for the following date range:' localization string </remarks>
        public string TheUsageForAllItemsAppearsBelowForTheFoll { get; private set; }

        /// <remarks> 'To:' localization string </remarks>
        public string To { get; private set; }

        /// <remarks> '"To change the date shown, choose your dates above and hit the GO button."' localization string </remarks>
        public string ToChangeTheDateShownChooseYourDatesAboveA { get; private set; }

        /// <remarks> 'Summary by Collection' localization string </remarks>
        public string SummaryByCollection { get; private set; }

        /// <remarks> 'NO USAGE STATISTICS EXIST FOR YOUR SELECTION' localization string </remarks>
        public string NOUSAGESTATISTICSEXISTFORYOURSELECTION { get; private set; }

        /// <remarks> 'Export as CSV' localization string </remarks>
        public string ExportAsCSV { get; private set; }

        /// <remarks> 'JPEG Views' localization string </remarks>
        public string JPEGViews { get; private set; }

        /// <remarks> 'Zoomable Views' localization string </remarks>
        public string ZoomableViews { get; private set; }

        /// <remarks> 'Citation Views' localization string </remarks>
        public string CitationViews { get; private set; }

        /// <remarks> 'Thumbnail Views' localization string </remarks>
        public string ThumbnailViews { get; private set; }

        /// <remarks> 'Text Searches' localization string </remarks>
        public string TextSearches { get; private set; }

        /// <remarks> 'Flash Views' localization string </remarks>
        public string FlashViews { get; private set; }

        /// <remarks> 'Map Views' localization string </remarks>
        public string MapViews { get; private set; }

        /// <remarks> 'Download Views' localization string </remarks>
        public string DownloadViews { get; private set; }

        /// <remarks> 'Static Views' localization string </remarks>
        public string StaticViews { get; private set; }

        /// <remarks> 'Collections' localization string </remarks>
        public string Collections { get; private set; }

        /// <remarks> 'Type' localization string </remarks>
        public string Type { get; private set; }

        /// <remarks> 'Terms' localization string </remarks>
        public string Terms { get; private set; }

        /// <remarks> 'Time' localization string </remarks>
        public string Time { get; private set; }

        /// <remarks> 'Title' localization string </remarks>
        public string Title { get; private set; }

        /// <remarks> 'NUMBER' localization string </remarks>
        public string NUMBER { get; private set; }

        /// <remarks> '"The data below shows details for the views that occurred at each level of the collection hierarchy (Collection Groups, Collections, SubCollections)."' localization string </remarks>
        public string TheDataBelowShowsDetailsForTheViewsThatOc { get; private set; }

        /// <remarks> 'These statistics show the number of times users:' localization string </remarks>
        public string TheseStatisticsShowTheNumberOfTimesUsers { get; private set; }

        /// <remarks> 'navigated to the collection main pages' localization string </remarks>
        public string NavigatedToTheCollectionMainPages { get; private set; }

        /// <remarks> 'browsed the items or information about the collection' localization string </remarks>
        public string BrowsedTheItemsOrInformationAboutTheCollect { get; private set; }

        /// <remarks> 'performed searches against the items contained in the collection' localization string </remarks>
        public string PerformedSearchesAgainstTheItemsContainedIn { get; private set; }

        /// <remarks> 'viewed titles and items contained within the collection' localization string </remarks>
        public string ViewedTitlesAndItemsContainedWithinTheColle { get; private set; }

        /// <remarks> '"For the specialized item-level view details, see Item Views by Date."' localization string </remarks>
        public string ForTheSpecializedItemlevelViewDetailsSeeIte { get; private set; }

        /// <remarks> 'The usage for all the collections appears below for the following data range:' localization string </remarks>
        public string TheUsageForAllTheCollectionsAppearsBelowFo { get; private set; }

        /// <remarks> 'GROUP CODE' localization string </remarks>
        public string GROUPCODE { get; private set; }

        /// <remarks> 'COLL CODE' localization string </remarks>
        public string COLLCODE { get; private set; }

        /// <remarks> 'SUB CODE' localization string </remarks>
        public string SUBCODE { get; private set; }

        /// <remarks> 'TOTAL VIEWS' localization string </remarks>
        public string TOTALVIEWS { get; private set; }

        /// <remarks> 'VISITS' localization string </remarks>
        public string VISITS { get; private set; }

        /// <remarks> 'MAIN PAGES' localization string </remarks>
        public string MAINPAGES { get; private set; }

        /// <remarks> 'BROWSES' localization string </remarks>
        public string BROWSES { get; private set; }

        /// <remarks> 'SEARCH RESULTS' localization string </remarks>
        public string SEARCHRESULTS { get; private set; }

        /// <remarks> 'TITLE VIEWS' localization string </remarks>
        public string TITLEVIEWS { get; private set; }

        /// <remarks> 'ITEM VIEWS' localization string </remarks>
        public string ITEMVIEWS { get; private set; }

        /// <remarks> 'ALL COLLECTION GROUPS' localization string </remarks>
        public string ALLCOLLECTIONGROUPS { get; private set; }

        /// <remarks> 'Usage history for a single collection is displayed below. This history includes views of the collection and views of the items within the collections.' localization string </remarks>
        public string UsageHistoryForASingleCollectionIsDisplayed { get; private set; }

        /// <remarks> 'The usage displayed is for the following collection:' localization string </remarks>
        public string TheUsageDisplayedIsForTheFollowingCollectio { get; private set; }

        /// <remarks> 'The data below shows the collection history for the selected collection.  The first table shows the summary of all views of this collection and items contained in the collection.  The second table includes the details for specialized item-level views.' localization string </remarks>
        public string TheDataBelowShowsTheCollectionHistoryForTh { get; private set; }

        /// <remarks> '"Below is the history collection information included in comma-seperated value form.  To use the data below, cut and paste it into a CSV or text file.  The resulting file can be opened in a variety of applications, including OpenOffice and Microsoft Excel."' localization string </remarks>
        public string BelowIsTheHistoryCollectionInformationInclud { get; private set; }

        /// <remarks> 'MONTH' localization string </remarks>
        public string MONTH { get; private set; }

        /// <remarks> 'Titles and Items' localization string </remarks>
        public string TitlesAndItems { get; private set; }

        /// <remarks> 'Below are the most up to date numbers for overall utilization of %1.  The first table shows the summary of views against all collections.  The second table includes the details for specialized item-level views.' localization string </remarks>
        public string BelowAreTheMostUpToDateNumbersForOverall { get; private set; }

        /// <remarks> 'NO ITEM COUNT AVAILABLE' localization string </remarks>
        public string NOITEMCOUNTAVAILABLE { get; private set; }

        /// <remarks> 'Below are the number of items in each collection and subcollection.' localization string </remarks>
        public string BelowAreTheNumberOfItemsInEachCollectionA { get; private set; }

        /// <remarks> 'TITLES' localization string </remarks>
        public string TITLES { get; private set; }

        /// <remarks> 'ITEMS' localization string </remarks>
        public string ITEMS { get; private set; }

        /// <remarks> 'PAGES' localization string </remarks>
        public string PAGES { get; private set; }

        /// <remarks> 'FYTD TITLES' localization string </remarks>
        public string FYTDTITLES { get; private set; }

        /// <remarks> 'FYTD ITEMS' localization string </remarks>
        public string FYTDITEMS { get; private set; }

        /// <remarks> 'FYTD PAGES' localization string </remarks>
        public string FYTDPAGES { get; private set; }

        /// <remarks> 'List View' localization string </remarks>
        public string ListView { get; private set; }

        /// <remarks> 'Brief View' localization string </remarks>
        public string BriefView { get; private set; }

        /// <remarks> 'Tree View' localization string </remarks>
        public string TreeView { get; private set; }

        /// <remarks> 'BibID' localization string </remarks>
        public string Bibid { get; private set; }

        /// <remarks> 'VID' localization string </remarks>
        public string VID { get; private set; }

        /// <remarks> 'The following terms are defined below:' localization string </remarks>
        public string TheFollowingTermsAreDefinedBelow { get; private set; }

        /// <remarks> 'Collection Hierarchy' localization string </remarks>
        public string CollectionHierarchy { get; private set; }

        /// <remarks> 'Collection Groups' localization string </remarks>
        public string CollectionGroups { get; private set; }

        /// <remarks> 'SubCollections' localization string </remarks>
        public string Subcollections { get; private set; }

        /// <remarks> 'NO STATISTICS FOUND FOR THAT DATE RANGE' localization string </remarks>
        public string NOSTATISTICSFOUNDFORTHATDATERANGE { get; private set; }

        /// <remarks> '"Below is the item count with fiscal year to date information included in comma-seperated value form.  To use the data below, cut and paste it into a CSV or text file.  The resulting file can be opened in a variety of applications, including OpenOffice and Microsoft Excel."' localization string </remarks>
        public string BelowIsTheItemCountWithFiscalYearToDateI { get; private set; }

        /// <remarks> 'Defined Terms' localization string </remarks>
        public string DefinedTerms { get; private set; }

        /// <remarks> 'CODES' localization string </remarks>
        public string CODES { get; private set; }

        /// <remarks> 'NAME' localization string </remarks>
        public string NAME { get; private set; }

        /// <remarks> 'TOTAL' localization string </remarks>
        public string TOTAL { get; private set; }

        /// <remarks> '"This option allows the complete title count, item count, and page count to be viewed for a previous time and to additionally see the growth between two arbitrary dates."' localization string </remarks>
        public string ThisOptionAllowsTheCompleteTitleCountItemC { get; private set; }

        /// <remarks> '"The title count, item count, and page count appear below for the following two arbitrary dates:"' localization string </remarks>
        public string TheTitleCountItemCountAndPageCountAppearB { get; private set; }

        /// <remarks> '"To change the dates shown, choose your dates above and hit the GO button."' localization string </remarks>
        public string ToChangeTheDatesShownChooseYourDatesAbove { get; private set; }

        /// <remarks> 'Item Count and Growth' localization string </remarks>
        public string ItemCountAndGrowth { get; private set; }

        /// <remarks> '"Below are the number of titles, items, and pages in each collection and subcollection.  The &quot;INITIAL&quot; values are the values that were present at midnight on %1 before any new items were processed that day."' localization string </remarks>
        public string BelowAreTheNumberOfTitlesItemsAndPagesIn { get; private set; }

        /// <remarks> '" The &quot;ADDED&quot; values are the number of titles, items, and pages that were added between %1 and %2 inclusive."' localization string </remarks>
        public string TheQuotaddedquotValuesAreTheNumberOfTitles { get; private set; }

        /// <remarks> 'INITIAL TITLES' localization string </remarks>
        public string INITIALTITLES { get; private set; }

        /// <remarks> 'INITIAL ITEMS' localization string </remarks>
        public string INITIALITEMS { get; private set; }

        /// <remarks> 'INITIAL PAGES' localization string </remarks>
        public string INITIALPAGES { get; private set; }

    }
}
