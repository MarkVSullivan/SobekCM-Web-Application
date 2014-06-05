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
                case "The Quotaddedquot Values Are The Number Of Titles Items And Pages That Were Added Between XXX And XXX Inclusive":
                    TheQuotaddedquotValuesAreTheNumberOfTitlesItemsAndPagesThatWereAddedBetweenXXXAndXXXInclusive = Value;
                    break;

                case "Advanced Search":
                    AdvancedSearch = Value;
                    break;

                case "ALL COLLECTION GROUPS":
                    ALLCOLLECTIONGROUPS = Value;
                    break;

                case "Arbitrary Dates":
                    ArbitraryDates = Value;
                    break;

                case "Below Are The Details About The Specialized Views For Items Aggregated At The Collection Levels":
                    BelowAreTheDetailsAboutTheSpecializedViewsForItemsAggregatedAtTheCollectionLevels = Value;
                    break;

                case "Below Are The Most Up To Date Numbers For Overall Utilization Of XXX The First Table Shows The Summary Of Views Against All Collections The Second Table Includes The Details For Specialized Itemlevel Views":
                    BelowAreTheMostUpToDateNumbersForOverallUtilizationOfXXXTheFirstTableShowsTheSummaryOfViewsAgainstAllCollectionsTheSecondTableIncludesTheDetailsForSpecializedItemlevelViews = Value;
                    break;

                case "Below Are The Number Of Items In Each Collection And Subcollection":
                    BelowAreTheNumberOfItemsInEachCollectionAndSubcollection = Value;
                    break;

                case "Below Are The Number Of Titles Items And Pages In Each Collection And Subcollection The Quotinitialquot Values Are The Values That Were Present At Midnight On XXX Before Any New Items Were Processed That Day":
                    BelowAreTheNumberOfTitlesItemsAndPagesInEachCollectionAndSubcollectionTheQuotinitialquotValuesAreTheValuesThatWerePresentAtMidnightOnXXXBeforeAnyNewItemsWereProcessedThatDay = Value;
                    break;

                case "Below Is The Collection And Itemlevel Details For Your Provided Date Range In Commaseperated Value Form To Use The Data Below Cut And Paste It Into A CSV Or Text File The Resulting File Can Be Opened In A Variety Of Applications Including Openoffice And Microsoft Excel":
                    BelowIsTheCollectionAndItemlevelDetailsForYourProvidedDateRangeInCommaseperatedValueFormToUseTheDataBelowCutAndPasteItIntoACSVOrTextFileTheResultingFileCanBeOpenedInAVarietyOfApplicationsIncludingOpenofficeAndMicrosoftExcel = Value;
                    break;

                case "Below Is The History Collection Information Included In Commaseperated Value Form To Use The Data Below Cut And Paste It Into A CSV Or Text File The Resulting File Can Be Opened In A Variety Of Applications Including Openoffice And Microsoft Excel":
                    BelowIsTheHistoryCollectionInformationIncludedInCommaseperatedValueFormToUseTheDataBelowCutAndPasteItIntoACSVOrTextFileTheResultingFileCanBeOpenedInAVarietyOfApplicationsIncludingOpenofficeAndMicrosoftExcel = Value;
                    break;

                case "Below Is The Item Count With Fiscal Year To Date Information Included In Commaseperated Value Form To Use The Data Below Cut And Paste It Into A CSV Or Text File The Resulting File Can Be Opened In A Variety Of Applications Including Openoffice And Microsoft Excel":
                    BelowIsTheItemCountWithFiscalYearToDateInformationIncludedInCommaseperatedValueFormToUseTheDataBelowCutAndPasteItIntoACSVOrTextFileTheResultingFileCanBeOpenedInAVarietyOfApplicationsIncludingOpenofficeAndMicrosoftExcel = Value;
                    break;

                case "Bibid":
                    Bibid = Value;
                    break;

                case "Brief View":
                    BriefView = Value;
                    break;

                case "Browse Partners":
                    BrowsePartners = Value;
                    break;

                case "Browsed The Items Or Information About The Collection":
                    BrowsedTheItemsOrInformationAboutTheCollection = Value;
                    break;

                case "BROWSES":
                    BROWSES = Value;
                    break;

                case "By Name":
                    ByName = Value;
                    break;

                case "Citation Views":
                    CitationViews = Value;
                    break;

                case "Click Here To View The Most Commonly Accessed ITEMS":
                    ClickHereToViewTheMostCommonlyAccessedITEMS = Value;
                    break;

                case "Click Here To View The Most Commonly Accessed TITLES":
                    ClickHereToViewTheMostCommonlyAccessedTITLES = Value;
                    break;

                case "CODES":
                    CODES = Value;
                    break;

                case "COLL CODE":
                    COLLCODE = Value;
                    break;

                case "Collection Groups":
                    CollectionGroups = Value;
                    break;

                case "Collection Hierarchy":
                    CollectionHierarchy = Value;
                    break;

                case "Collection History":
                    CollectionHistory = Value;
                    break;

                case "Collections":
                    Collections = Value;
                    break;

                case "Collections By Date":
                    CollectionsByDate = Value;
                    break;

                case "Defined Terms":
                    DefinedTerms = Value;
                    break;

                case "Definitions":
                    Definitions = Value;
                    break;

                case "Download Views":
                    DownloadViews = Value;
                    break;

                case "Export As CSV":
                    ExportAsCSV = Value;
                    break;

                case "Flash Views":
                    FlashViews = Value;
                    break;

                case "For The Number Of Times Collections Were Viewed Or Searched See Collections By Date":
                    ForTheNumberOfTimesCollectionsWereViewedOrSearchedSeeCollectionsByDate = Value;
                    break;

                case "For The Specialized Itemlevel View Details See Item Views By Date":
                    ForTheSpecializedItemlevelViewDetailsSeeItemViewsByDate = Value;
                    break;

                case "From":
                    From = Value;
                    break;

                case "FYTD Growth View":
                    FYTDGrowthView = Value;
                    break;

                case "FYTD ITEMS":
                    FYTDITEMS = Value;
                    break;

                case "FYTD PAGES":
                    FYTDPAGES = Value;
                    break;

                case "FYTD TITLES":
                    FYTDTITLES = Value;
                    break;

                case "GROUP CODE":
                    GROUPCODE = Value;
                    break;

                case "INITIAL ITEMS":
                    INITIALITEMS = Value;
                    break;

                case "INITIAL PAGES":
                    INITIALPAGES = Value;
                    break;

                case "INITIAL TITLES":
                    INITIALTITLES = Value;
                    break;

                case "Item Count":
                    ItemCount = Value;
                    break;

                case "Item Count And Growth":
                    ItemCountAndGrowth = Value;
                    break;

                case "ITEM VIEWS":
                    ITEMVIEWS = Value;
                    break;

                case "Item Views By Date":
                    ItemViewsByDate = Value;
                    break;

                case "ITEMS":
                    ITEMS = Value;
                    break;

                case "JPEG Views":
                    JPEGViews = Value;
                    break;

                case "List View":
                    ListView = Value;
                    break;

                case "MAIN PAGES":
                    MAINPAGES = Value;
                    break;

                case "Map Views":
                    MapViews = Value;
                    break;

                case "MONTH":
                    MONTH = Value;
                    break;

                case "Most Accessed Items":
                    MostAccessedItems = Value;
                    break;

                case "Most Accessed Titles":
                    MostAccessedTitles = Value;
                    break;

                case "NAME":
                    NAME = Value;
                    break;

                case "Navigated To The Collection Main Pages":
                    NavigatedToTheCollectionMainPages = Value;
                    break;

                case "NO ITEM COUNT AVAILABLE":
                    NOITEMCOUNTAVAILABLE = Value;
                    break;

                case "NO STATISTICS FOUND FOR THAT DATE RANGE":
                    NOSTATISTICSFOUNDFORTHATDATERANGE = Value;
                    break;

                case "NO USAGE STATISTICS EXIST FOR YOUR SELECTION":
                    NOUSAGESTATISTICSEXISTFORYOURSELECTION = Value;
                    break;

                case "NUMBER":
                    NUMBER = Value;
                    break;

                case "Overall Stats":
                    OverallStats = Value;
                    break;

                case "PAGES":
                    PAGES = Value;
                    break;

                case "Performed Searches Against The Items Contained In The Collection":
                    PerformedSearchesAgainstTheItemsContainedInTheCollection = Value;
                    break;

                case "Recent Searches":
                    RecentSearches = Value;
                    break;

                case "Recent Searches In XXX":
                    RecentSearchesInXXX = Value;
                    break;

                case "Resource Count In XXX":
                    ResourceCountInXXX = Value;
                    break;

                case "SEARCH RESULTS":
                    SEARCHRESULTS = Value;
                    break;

                case "Select Collection":
                    SelectCollection = Value;
                    break;

                case "Selected Collection":
                    SelectedCollection = Value;
                    break;

                case "Selected Date Range":
                    SelectedDateRange = Value;
                    break;

                case "Standard View":
                    StandardView = Value;
                    break;

                case "Static Views":
                    StaticViews = Value;
                    break;

                case "SUB CODE":
                    SUBCODE = Value;
                    break;

                case "Subcollections":
                    Subcollections = Value;
                    break;

                case "Summary By Collection":
                    SummaryByCollection = Value;
                    break;

                case "Terms":
                    Terms = Value;
                    break;

                case "Text Searches":
                    TextSearches = Value;
                    break;

                case "The Data Below Shows Details For The Views That Occurred At Each Level Of The Collection Hierarchy Collection Groups Collections Subcollections":
                    TheDataBelowShowsDetailsForTheViewsThatOccurredAtEachLevelOfTheCollectionHierarchyCollectionGroupsCollectionsSubcollections = Value;
                    break;

                case "The Data Below Shows The Collection History For The Selected Collection The First Table Shows The Summary Of All Views Of This Collection And Items Contained In The Collection The Second Table Includes The Details For Specialized Itemlevel Views":
                    TheDataBelowShowsTheCollectionHistoryForTheSelectedCollectionTheFirstTableShowsTheSummaryOfAllViewsOfThisCollectionAndItemsContainedInTheCollectionTheSecondTableIncludesTheDetailsForSpecializedItemlevelViews = Value;
                    break;

                case "The Data Below Shows The Most Commonly Accessed Items Within The Collection Above":
                    TheDataBelowShowsTheMostCommonlyAccessedItemsWithinTheCollectionAbove = Value;
                    break;

                case "The Data Below Shows The Most Commonly Accessed Titles Within The Collection Above":
                    TheDataBelowShowsTheMostCommonlyAccessedTitlesWithinTheCollectionAbove = Value;
                    break;

                case "The Definitions Page Provides More Details About The Statistics And Words Used Below":
                    TheDefinitionsPageProvidesMoreDetailsAboutTheStatisticsAndWordsUsedBelow = Value;
                    break;

                case "The Following Terms Are Defined Below":
                    TheFollowingTermsAreDefinedBelow = Value;
                    break;

                case "The Most Commonly Accessed Items Below Are Displayed For The Following Collection":
                    TheMostCommonlyAccessedItemsBelowAreDisplayedForTheFollowingCollection = Value;
                    break;

                case "The Most Commonly Accessed Items By Collection Appear Below":
                    TheMostCommonlyAccessedItemsByCollectionAppearBelow = Value;
                    break;

                case "The Most Commonly Accessed Titles Below Are Displayed Is For The Following Collection":
                    TheMostCommonlyAccessedTitlesBelowAreDisplayedIsForTheFollowingCollection = Value;
                    break;

                case "The Most Commonly Utilized Titles By Collection Appear Below":
                    TheMostCommonlyUtilizedTitlesByCollectionAppearBelow = Value;
                    break;

                case "The Title Count Item Count And Page Count Appear Below For The Following Two Arbitrary Dates":
                    TheTitleCountItemCountAndPageCountAppearBelowForTheFollowingTwoArbitraryDates = Value;
                    break;

                case "The Usage Displayed Is For The Following Collection":
                    TheUsageDisplayedIsForTheFollowingCollection = Value;
                    break;

                case "The Usage For All Items Appears Below For The Following Date Range":
                    TheUsageForAllItemsAppearsBelowForTheFollowingDateRange = Value;
                    break;

                case "The Usage For All The Collections Appears Below For The Following Data Range":
                    TheUsageForAllTheCollectionsAppearsBelowForTheFollowingDataRange = Value;
                    break;

                case "These Statistics Show The Number Of Times Users":
                    TheseStatisticsShowTheNumberOfTimesUsers = Value;
                    break;

                case "This Option Allows The Complete Title Count Item Count And Page Count To Be Viewed For A Previous Time And To Additionally See The Growth Between Two Arbitrary Dates":
                    ThisOptionAllowsTheCompleteTitleCountItemCountAndPageCountToBeViewedForAPreviousTimeAndToAdditionallySeeTheGrowthBetweenTwoArbitraryDates = Value;
                    break;

                case "Thumbnail Views":
                    ThumbnailViews = Value;
                    break;

                case "Time":
                    Time = Value;
                    break;

                case "Title":
                    Title = Value;
                    break;

                case "TITLE VIEWS":
                    TITLEVIEWS = Value;
                    break;

                case "TITLES":
                    TITLES = Value;
                    break;

                case "Titles And Items":
                    TitlesAndItems = Value;
                    break;

                case "To Change The Collection Shown Choose The Collection Above And Hit The GO Button":
                    ToChangeTheCollectionShownChooseTheCollectionAboveAndHitTheGOButton = Value;
                    break;

                case "To Change The Date Shown Choose Your Dates Above And Hit The GO Button":
                    ToChangeTheDateShownChooseYourDatesAboveAndHitTheGOButton = Value;
                    break;

                case "To Change The Dates Shown Choose Your Dates Above And Hit The GO Button":
                    ToChangeTheDatesShownChooseYourDatesAboveAndHitTheGOButton = Value;
                    break;

                case "To":
                    To = Value;
                    break;

                case "Top Titles":
                    TopTitles = Value;
                    break;

                case "TOTAL":
                    TOTAL = Value;
                    break;

                case "TOTAL VIEWS":
                    TOTALVIEWS = Value;
                    break;

                case "Tree View":
                    TreeView = Value;
                    break;

                case "Type":
                    Type = Value;
                    break;

                case "Usage History For A Single Collection Is Displayed Below This History Includes Views Of The Collection And Views Of The Items Within The Collections":
                    UsageHistoryForASingleCollectionIsDisplayedBelowThisHistoryIncludesViewsOfTheCollectionAndViewsOfTheItemsWithinTheCollections = Value;
                    break;

                case "Usage Statistics":
                    UsageStatistics = Value;
                    break;

                case "Usage Statistics For XXX":
                    UsageStatisticsForXXX = Value;
                    break;

                case "VID":
                    VID = Value;
                    break;

                case "Viewed Titles And Items Contained Within The Collection":
                    ViewedTitlesAndItemsContainedWithinTheCollection = Value;
                    break;

                case "Views":
                    Views = Value;
                    break;

                case "VISITS":
                    VISITS = Value;
                    break;

                case "Zoomable Views":
                    ZoomableViews = Value;
                    break;

            }
        }
        /// <remarks> %2 = second date selected </remarks>
        public string TheQuotaddedquotValuesAreTheNumberOfTitlesItemsAndPagesThatWereAddedBetweenXXXAndXXXInclusive { get; private set; }

        /// <remarks> 'Advanced Search' localization string </remarks>
        public string AdvancedSearch { get; private set; }

        /// <remarks> Row header in table </remarks>
        public string ALLCOLLECTIONGROUPS { get; private set; }

        /// <remarks> Allows admin to select any arbitrary dates for stats </remarks>
        public string ArbitraryDates { get; private set; }

        /// <remarks> 'Below are the details about the specialized views for items aggregated at the collection levels.' localization string </remarks>
        public string BelowAreTheDetailsAboutTheSpecializedViewsForItemsAggregatedAtTheCollectionLevels { get; private set; }

        /// <remarks> %1 = abbreviation for system.  For overall usage page </remarks>
        public string BelowAreTheMostUpToDateNumbersForOverallUtilizationOfXXXTheFirstTableShowsTheSummaryOfViewsAgainstAllCollectionsTheSecondTableIncludesTheDetailsForSpecializedItemlevelViews { get; private set; }

        /// <remarks> 'Below are the number of items in each collection and subcollection.' localization string </remarks>
        public string BelowAreTheNumberOfItemsInEachCollectionAndSubcollection { get; private set; }

        /// <remarks> %1 = first date selected </remarks>
        public string BelowAreTheNumberOfTitlesItemsAndPagesInEachCollectionAndSubcollectionTheQuotinitialquotValuesAreTheValuesThatWerePresentAtMidnightOnXXXBeforeAnyNewItemsWereProcessedThatDay { get; private set; }

        /// <remarks> '"Below is the collection and item-level details for your provided date range in comma-seperated value form.  To use the data below, cut and paste it into a CSV or text file.  The resulting file can be opened in a variety of applications, including OpenOffice and Microsoft Excel."' localization string </remarks>
        public string BelowIsTheCollectionAndItemlevelDetailsForYourProvidedDateRangeInCommaseperatedValueFormToUseTheDataBelowCutAndPasteItIntoACSVOrTextFileTheResultingFileCanBeOpenedInAVarietyOfApplicationsIncludingOpenofficeAndMicrosoftExcel { get; private set; }

        /// <remarks> '"Below is the history collection information included in comma-seperated value form.  To use the data below, cut and paste it into a CSV or text file.  The resulting file can be opened in a variety of applications, including OpenOffice and Microsoft Excel."' localization string </remarks>
        public string BelowIsTheHistoryCollectionInformationIncludedInCommaseperatedValueFormToUseTheDataBelowCutAndPasteItIntoACSVOrTextFileTheResultingFileCanBeOpenedInAVarietyOfApplicationsIncludingOpenofficeAndMicrosoftExcel { get; private set; }

        /// <remarks> For exporting the item count information as CSV </remarks>
        public string BelowIsTheItemCountWithFiscalYearToDateInformationIncludedInCommaseperatedValueFormToUseTheDataBelowCutAndPasteItIntoACSVOrTextFileTheResultingFileCanBeOpenedInAVarietyOfApplicationsIncludingOpenofficeAndMicrosoftExcel { get; private set; }

        /// <remarks> 'BibID' localization string </remarks>
        public string Bibid { get; private set; }

        /// <remarks> 'Brief View' localization string </remarks>
        public string BriefView { get; private set; }

        /// <remarks> 'Browse Partners' localization string </remarks>
        public string BrowsePartners { get; private set; }

        /// <remarks> 'browsed the items or information about the collection' localization string </remarks>
        public string BrowsedTheItemsOrInformationAboutTheCollection { get; private set; }

        /// <remarks> Header for table </remarks>
        public string BROWSES { get; private set; }

        /// <remarks> 'By Name' localization string </remarks>
        public string ByName { get; private set; }

        /// <remarks> 'Citation Views' localization string </remarks>
        public string CitationViews { get; private set; }

        /// <remarks> 'Click here to view the most commonly accessed ITEMS' localization string </remarks>
        public string ClickHereToViewTheMostCommonlyAccessedITEMS { get; private set; }

        /// <remarks> 'Click here to view the most commonly accessed TITLES' localization string </remarks>
        public string ClickHereToViewTheMostCommonlyAccessedTITLES { get; private set; }

        /// <remarks> 'CODES' localization string </remarks>
        public string CODES { get; private set; }

        /// <remarks> Short for Collection Code - Header for table </remarks>
        public string COLLCODE { get; private set; }

        /// <remarks> 'Collection Groups' localization string </remarks>
        public string CollectionGroups { get; private set; }

        /// <remarks> 'Collection Hierarchy' localization string </remarks>
        public string CollectionHierarchy { get; private set; }

        /// <remarks> Tabs for different views on usage  </remarks>
        public string CollectionHistory { get; private set; }

        /// <remarks> 'Collections' localization string </remarks>
        public string Collections { get; private set; }

        /// <remarks> Tabs for different views on usage  </remarks>
        public string CollectionsByDate { get; private set; }

        /// <remarks> 'Defined Terms' localization string </remarks>
        public string DefinedTerms { get; private set; }

        /// <remarks> Tabs for different views on usage  </remarks>
        public string Definitions { get; private set; }

        /// <remarks> Views on the page which lists the downloads </remarks>
        public string DownloadViews { get; private set; }

        /// <remarks> 'Export as CSV' localization string </remarks>
        public string ExportAsCSV { get; private set; }

        /// <remarks> Items which are utilizing Adobe Flash files </remarks>
        public string FlashViews { get; private set; }

        /// <remarks> '"For the number of times collections were viewed or searched, see Collections by Date."' localization string </remarks>
        public string ForTheNumberOfTimesCollectionsWereViewedOrSearchedSeeCollectionsByDate { get; private set; }

        /// <remarks> '"For the specialized item-level view details, see Item Views by Date."' localization string </remarks>
        public string ForTheSpecializedItemlevelViewDetailsSeeItemViewsByDate { get; private set; }

        /// <remarks> Prompt for the dropdown list of all colections </remarks>
        public string From { get; private set; }

        /// <remarks> FYTD = Fiscal Year To Date </remarks>
        public string FYTDGrowthView { get; private set; }

        /// <remarks> "If in Fiscal Year to Date mode, header for table" </remarks>
        public string FYTDITEMS { get; private set; }

        /// <remarks> "If in Fiscal Year to Date mode, header for table" </remarks>
        public string FYTDPAGES { get; private set; }

        /// <remarks> "If in Fiscal Year to Date mode, header for table" </remarks>
        public string FYTDTITLES { get; private set; }

        /// <remarks> Header for table </remarks>
        public string GROUPCODE { get; private set; }

        /// <remarks> 'INITIAL ITEMS' localization string </remarks>
        public string INITIALITEMS { get; private set; }

        /// <remarks> 'INITIAL PAGES' localization string </remarks>
        public string INITIALPAGES { get; private set; }

        /// <remarks> 'INITIAL TITLES' localization string </remarks>
        public string INITIALTITLES { get; private set; }

        /// <remarks> Allows usage stats and item counts to be displayed </remarks>
        public string ItemCount { get; private set; }

        /// <remarks> 'Item Count and Growth' localization string </remarks>
        public string ItemCountAndGrowth { get; private set; }

        /// <remarks> Header for table </remarks>
        public string ITEMVIEWS { get; private set; }

        /// <remarks> Tabs for different views on usage  </remarks>
        public string ItemViewsByDate { get; private set; }

        /// <remarks> Header for table </remarks>
        public string ITEMS { get; private set; }

        /// <remarks> 'JPEG Views' localization string </remarks>
        public string JPEGViews { get; private set; }

        /// <remarks> 'List View' localization string </remarks>
        public string ListView { get; private set; }

        /// <remarks> Header for table </remarks>
        public string MAINPAGES { get; private set; }

        /// <remarks> 'Map Views' localization string </remarks>
        public string MapViews { get; private set; }

        /// <remarks> 'MONTH' localization string </remarks>
        public string MONTH { get; private set; }

        /// <remarks> 'Most Accessed Items' localization string </remarks>
        public string MostAccessedItems { get; private set; }

        /// <remarks> 'Most Accessed Titles' localization string </remarks>
        public string MostAccessedTitles { get; private set; }

        /// <remarks> 'NAME' localization string </remarks>
        public string NAME { get; private set; }

        /// <remarks> 'navigated to the collection main pages' localization string </remarks>
        public string NavigatedToTheCollectionMainPages { get; private set; }

        /// <remarks> 'NO ITEM COUNT AVAILABLE' localization string </remarks>
        public string NOITEMCOUNTAVAILABLE { get; private set; }

        /// <remarks> 'NO STATISTICS FOUND FOR THAT DATE RANGE' localization string </remarks>
        public string NOSTATISTICSFOUNDFORTHATDATERANGE { get; private set; }

        /// <remarks> 'NO USAGE STATISTICS EXIST FOR YOUR SELECTION' localization string </remarks>
        public string NOUSAGESTATISTICSEXISTFORYOURSELECTION { get; private set; }

        /// <remarks> 'NUMBER' localization string </remarks>
        public string NUMBER { get; private set; }

        /// <remarks> Tabs for different views on usage  </remarks>
        public string OverallStats { get; private set; }

        /// <remarks> Header for table </remarks>
        public string PAGES { get; private set; }

        /// <remarks> 'performed searches against the items contained in the collection' localization string </remarks>
        public string PerformedSearchesAgainstTheItemsContainedInTheCollection { get; private set; }

        /// <remarks> Allows usage stats and item counts to be displayed </remarks>
        public string RecentSearches { get; private set; }

        /// <remarks> %1 = code for SobekCM instance.  Allows usage stats and item counts to be displayed </remarks>
        public string RecentSearchesInXXX { get; private set; }

        /// <remarks> %1 = code for SobekCM instance.  Allows usage stats and item counts to be displayed </remarks>
        public string ResourceCountInXXX { get; private set; }

        /// <remarks> Header for table </remarks>
        public string SEARCHRESULTS { get; private set; }

        /// <remarks> 'Select Collection' localization string </remarks>
        public string SelectCollection { get; private set; }

        /// <remarks> 'Selected Collection' localization string </remarks>
        public string SelectedCollection { get; private set; }

        /// <remarks> 'Selected Date Range' localization string </remarks>
        public string SelectedDateRange { get; private set; }

        /// <remarks> 'Standard View' localization string </remarks>
        public string StandardView { get; private set; }

        /// <remarks> "Views on the static pages, which is how search engines index system" </remarks>
        public string StaticViews { get; private set; }

        /// <remarks> Short for Subcollection Code - Header for table </remarks>
        public string SUBCODE { get; private set; }

        /// <remarks> 'SubCollections' localization string </remarks>
        public string Subcollections { get; private set; }

        /// <remarks> 'Summary by Collection' localization string </remarks>
        public string SummaryByCollection { get; private set; }

        /// <remarks> 'Terms' localization string </remarks>
        public string Terms { get; private set; }

        /// <remarks> 'Text Searches' localization string </remarks>
        public string TextSearches { get; private set; }

        /// <remarks> '"The data below shows details for the views that occurred at each level of the collection hierarchy (Collection Groups, Collections, SubCollections)."' localization string </remarks>
        public string TheDataBelowShowsDetailsForTheViewsThatOccurredAtEachLevelOfTheCollectionHierarchyCollectionGroupsCollectionsSubcollections { get; private set; }

        /// <remarks> 'The data below shows the collection history for the selected collection.  The first table shows the summary of all views of this collection and items contained in the collection.  The second table includes the details for specialized item-level views.' localization string </remarks>
        public string TheDataBelowShowsTheCollectionHistoryForTheSelectedCollectionTheFirstTableShowsTheSummaryOfAllViewsOfThisCollectionAndItemsContainedInTheCollectionTheSecondTableIncludesTheDetailsForSpecializedItemlevelViews { get; private set; }

        /// <remarks> 'The data below shows the most commonly accessed items within the collection above.' localization string </remarks>
        public string TheDataBelowShowsTheMostCommonlyAccessedItemsWithinTheCollectionAbove { get; private set; }

        /// <remarks> 'The data below shows the most commonly accessed titles within the collection above.' localization string </remarks>
        public string TheDataBelowShowsTheMostCommonlyAccessedTitlesWithinTheCollectionAbove { get; private set; }

        /// <remarks> 'The Definitions page provides more details about the statistics and words used below.' localization string </remarks>
        public string TheDefinitionsPageProvidesMoreDetailsAboutTheStatisticsAndWordsUsedBelow { get; private set; }

        /// <remarks> 'The following terms are defined below:' localization string </remarks>
        public string TheFollowingTermsAreDefinedBelow { get; private set; }

        /// <remarks> 'The most commonly accessed items below are displayed for the following collection:' localization string </remarks>
        public string TheMostCommonlyAccessedItemsBelowAreDisplayedForTheFollowingCollection { get; private set; }

        /// <remarks> 'The most commonly accessed items by collection appear below.' localization string </remarks>
        public string TheMostCommonlyAccessedItemsByCollectionAppearBelow { get; private set; }

        /// <remarks> 'The most commonly accessed titles below are displayed is for the following collection:' localization string </remarks>
        public string TheMostCommonlyAccessedTitlesBelowAreDisplayedIsForTheFollowingCollection { get; private set; }

        /// <remarks> 'The most commonly utilized titles by collection appear below.' localization string </remarks>
        public string TheMostCommonlyUtilizedTitlesByCollectionAppearBelow { get; private set; }

        /// <remarks> '"The title count, item count, and page count appear below for the following two arbitrary dates:"' localization string </remarks>
        public string TheTitleCountItemCountAndPageCountAppearBelowForTheFollowingTwoArbitraryDates { get; private set; }

        /// <remarks> 'The usage displayed is for the following collection:' localization string </remarks>
        public string TheUsageDisplayedIsForTheFollowingCollection { get; private set; }

        /// <remarks> 'The usage for all items appears below for the following date range:' localization string </remarks>
        public string TheUsageForAllItemsAppearsBelowForTheFollowingDateRange { get; private set; }

        /// <remarks> 'The usage for all the collections appears below for the following data range:' localization string </remarks>
        public string TheUsageForAllTheCollectionsAppearsBelowForTheFollowingDataRange { get; private set; }

        /// <remarks> 'These statistics show the number of times users:' localization string </remarks>
        public string TheseStatisticsShowTheNumberOfTimesUsers { get; private set; }

        /// <remarks> '"This option allows the complete title count, item count, and page count to be viewed for a previous time and to additionally see the growth between two arbitrary dates."' localization string </remarks>
        public string ThisOptionAllowsTheCompleteTitleCountItemCountAndPageCountToBeViewedForAPreviousTimeAndToAdditionallySeeTheGrowthBetweenTwoArbitraryDates { get; private set; }

        /// <remarks> 'Thumbnail Views' localization string </remarks>
        public string ThumbnailViews { get; private set; }

        /// <remarks> 'Time' localization string </remarks>
        public string Time { get; private set; }

        /// <remarks> 'Title' localization string </remarks>
        public string Title { get; private set; }

        /// <remarks> Header for table </remarks>
        public string TITLEVIEWS { get; private set; }

        /// <remarks> Header for table </remarks>
        public string TITLES { get; private set; }

        /// <remarks> 'Titles and Items' localization string </remarks>
        public string TitlesAndItems { get; private set; }

        /// <remarks> '"To change the collection shown, choose the collection above and hit the GO button."' localization string </remarks>
        public string ToChangeTheCollectionShownChooseTheCollectionAboveAndHitTheGOButton { get; private set; }

        /// <remarks> '"To change the date shown, choose your dates above and hit the GO button."' localization string </remarks>
        public string ToChangeTheDateShownChooseYourDatesAboveAndHitTheGOButton { get; private set; }

        /// <remarks> '"To change the dates shown, choose your dates above and hit the GO button."' localization string </remarks>
        public string ToChangeTheDatesShownChooseYourDatesAboveAndHitTheGOButton { get; private set; }

        /// <remarks> 'To:' localization string </remarks>
        public string To { get; private set; }

        /// <remarks> Tabs for different views on usage  </remarks>
        public string TopTitles { get; private set; }

        /// <remarks> 'TOTAL' localization string </remarks>
        public string TOTAL { get; private set; }

        /// <remarks> Header for table </remarks>
        public string TOTALVIEWS { get; private set; }

        /// <remarks> 'Tree View' localization string </remarks>
        public string TreeView { get; private set; }

        /// <remarks> 'Type' localization string </remarks>
        public string Type { get; private set; }

        /// <remarks> 'Usage history for a single collection is displayed below. This history includes views of the collection and views of the items within the collections.' localization string </remarks>
        public string UsageHistoryForASingleCollectionIsDisplayedBelowThisHistoryIncludesViewsOfTheCollectionAndViewsOfTheItemsWithinTheCollections { get; private set; }

        /// <remarks> Allows usage stats and item counts to be displayed </remarks>
        public string UsageStatistics { get; private set; }

        /// <remarks> %1 = code for SobekCM instance.  Allows usage stats and item counts to be displayed </remarks>
        public string UsageStatisticsForXXX { get; private set; }

        /// <remarks> 'VID' localization string </remarks>
        public string VID { get; private set; }

        /// <remarks> 'viewed titles and items contained within the collection' localization string </remarks>
        public string ViewedTitlesAndItemsContainedWithinTheCollection { get; private set; }

        /// <remarks> 'Views' localization string </remarks>
        public string Views { get; private set; }

        /// <remarks> Header for table </remarks>
        public string VISITS { get; private set; }

        /// <remarks> 'Zoomable Views' localization string </remarks>
        public string ZoomableViews { get; private set; }

    }
}
