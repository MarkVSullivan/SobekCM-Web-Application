namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Usage_Statistics_AggregationViewer class </summary>
    public class Usage_Statistics_AggregationViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Usage_Statistics_AggregationViewer_Localization class </summary>
        public Usage_Statistics_AggregationViewer_LocalizationInfo() : base()
        {
            // Do nothing
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
                case "XXX Statistics":
                    XXXStatistics = Value;
                    break;

                case "Bibid":
                    Bibid = Value;
                    break;

                case "Browses":
                    Browses = Value;
                    break;

                case "Citation Views":
                    CitationViews = Value;
                    break;

                case "Collection Groups":
                    CollectionGroups = Value;
                    break;

                case "Collection Hierarchy":
                    CollectionHierarchy = Value;
                    break;

                case "Collection Views":
                    CollectionViews = Value;
                    break;

                case "Collections":
                    Collections = Value;
                    break;

                case "Date":
                    Date = Value;
                    break;

                case "Definitions":
                    Definitions = Value;
                    break;

                case "Definitions Of Terms Used":
                    DefinitionsOfTermsUsed = Value;
                    break;

                case "Definitions Page Provides More Details About The Statistics And Words Used Below":
                    DefinitionsPageProvidesMoreDetailsAboutTheStatisticsAndWordsUsedBelow = Value;
                    break;

                case "Download Views":
                    DownloadViews = Value;
                    break;

                case "Flash Views":
                    FlashViews = Value;
                    break;

                case "History Of Collectionlevel Usage":
                    HistoryOfCollectionlevelUsage = Value;
                    break;

                case "History Of Item Usage":
                    HistoryOfItemUsage = Value;
                    break;

                case "Item Views":
                    ItemViews = Value;
                    break;

                case "JPEG Views":
                    JPEGViews = Value;
                    break;

                case "Main Pages":
                    MainPages = Value;
                    break;

                case "Map Views":
                    MapViews = Value;
                    break;

                case "Most Accessed Items":
                    MostAccessedItems = Value;
                    break;

                case "Most Accessed Titles":
                    MostAccessedTitles = Value;
                    break;

                case "Search Results":
                    SearchResults = Value;
                    break;

                case "Static Views":
                    StaticViews = Value;
                    break;

                case "Subcollections":
                    Subcollections = Value;
                    break;

                case "Text Searches":
                    TextSearches = Value;
                    break;

                case "The Most Commonly Utilized Items For This Collection Appear Below":
                    TheMostCommonlyUtilizedItemsForThisCollectionAppearBelow = Value;
                    break;

                case "The Most Commonly Utilized Titles By Collection Appear Below":
                    TheMostCommonlyUtilizedTitlesByCollectionAppearBelow = Value;
                    break;

                case "Thumbnail Views":
                    ThumbnailViews = Value;
                    break;

                case "Title":
                    Title = Value;
                    break;

                case "Title Views":
                    TitleViews = Value;
                    break;

                case "Titles And Items":
                    TitlesAndItems = Value;
                    break;

                case "Top Items":
                    TopItems = Value;
                    break;

                case "Top Titles":
                    TopTitles = Value;
                    break;

                case "Total Views":
                    TotalViews = Value;
                    break;

                case "Usage History For The Items Within This Collection Are Displayed Below":
                    UsageHistoryForTheItemsWithinThisCollectionAreDisplayedBelow = Value;
                    break;

                case "Usage History For This Collection Is Displayed Below This History Includes Just The Toplevel Views Of The Collection":
                    UsageHistoryForThisCollectionIsDisplayedBelowThisHistoryIncludesJustTheToplevelViewsOfTheCollection = Value;
                    break;

                case "VID":
                    VID = Value;
                    break;

                case "Views":
                    Views = Value;
                    break;

                case "Visits":
                    Visits = Value;
                    break;

                case "Zoomable Views":
                    ZoomableViews = Value;
                    break;

            }
        }
        /// <remarks> '%1 Statistics' localization string </remarks>
        public string XXXStatistics { get; private set; }

        /// <remarks> 'BibID' localization string </remarks>
        public string Bibid { get; private set; }

        /// <remarks> 'Browses' localization string </remarks>
        public string Browses { get; private set; }

        /// <remarks> 'Citation Views' localization string </remarks>
        public string CitationViews { get; private set; }

        /// <remarks> 'Collection Groups' localization string </remarks>
        public string CollectionGroups { get; private set; }

        /// <remarks> 'Collection Hierarchy' localization string </remarks>
        public string CollectionHierarchy { get; private set; }

        /// <remarks> Admin view on usage statistics for aggregation from internal header </remarks>
        public string CollectionViews { get; private set; }

        /// <remarks> 'Collections' localization string </remarks>
        public string Collections { get; private set; }

        /// <remarks> Admin view on usage statistics for aggregation from internal header </remarks>
        public string Date { get; private set; }

        /// <remarks> Admin view on usage statistics for aggregation from internal header </remarks>
        public string Definitions { get; private set; }

        /// <remarks> Admin view on usage statistics for aggregation from internal header </remarks>
        public string DefinitionsOfTermsUsed { get; private set; }

        /// <remarks> Admin view on usage statistics for aggregation from internal header </remarks>
        public string DefinitionsPageProvidesMoreDetailsAboutTheStatisticsAndWordsUsedBelow { get; private set; }

        /// <remarks> Views on the page which lists the downloads </remarks>
        public string DownloadViews { get; private set; }

        /// <remarks> Items which are utilizing Adobe Flash files </remarks>
        public string FlashViews { get; private set; }

        /// <remarks> Admin view on usage statistics for aggregation from internal header </remarks>
        public string HistoryOfCollectionlevelUsage { get; private set; }

        /// <remarks> Admin view on usage statistics for aggregation from internal header </remarks>
        public string HistoryOfItemUsage { get; private set; }

        /// <remarks> Admin view on usage statistics for aggregation from internal header </remarks>
        public string ItemViews { get; private set; }

        /// <remarks> 'JPEG Views' localization string </remarks>
        public string JPEGViews { get; private set; }

        /// <remarks> Admin view on usage statistics for aggregation from internal header </remarks>
        public string MainPages { get; private set; }

        /// <remarks> 'Map Views' localization string </remarks>
        public string MapViews { get; private set; }

        /// <remarks> Admin view on usage statistics for aggregation from internal header </remarks>
        public string MostAccessedItems { get; private set; }

        /// <remarks> Admin view on usage statistics for aggregation from internal header </remarks>
        public string MostAccessedTitles { get; private set; }

        /// <remarks> 'Search Results' localization string </remarks>
        public string SearchResults { get; private set; }

        /// <remarks> "Views on the static pages, which is how search engines index system" </remarks>
        public string StaticViews { get; private set; }

        /// <remarks> 'SubCollections' localization string </remarks>
        public string Subcollections { get; private set; }

        /// <remarks> 'Text Searches' localization string </remarks>
        public string TextSearches { get; private set; }

        /// <remarks> 'The most commonly utilized items for this collection appear below.' localization string </remarks>
        public string TheMostCommonlyUtilizedItemsForThisCollectionAppearBelow { get; private set; }

        /// <remarks> 'The most commonly utilized titles by collection appear below.' localization string </remarks>
        public string TheMostCommonlyUtilizedTitlesByCollectionAppearBelow { get; private set; }

        /// <remarks> 'Thumbnail Views' localization string </remarks>
        public string ThumbnailViews { get; private set; }

        /// <remarks> 'Title' localization string </remarks>
        public string Title { get; private set; }

        /// <remarks> 'Title Views' localization string </remarks>
        public string TitleViews { get; private set; }

        /// <remarks> 'Titles and Items' localization string </remarks>
        public string TitlesAndItems { get; private set; }

        /// <remarks> Admin view on usage statistics for aggregation from internal header </remarks>
        public string TopItems { get; private set; }

        /// <remarks> Admin view on usage statistics for aggregation from internal header </remarks>
        public string TopTitles { get; private set; }

        /// <remarks> Admin view on usage statistics for aggregation from internal header </remarks>
        public string TotalViews { get; private set; }

        /// <remarks> 'Usage history for the items within this collection are displayed below.' localization string </remarks>
        public string UsageHistoryForTheItemsWithinThisCollectionAreDisplayedBelow { get; private set; }

        /// <remarks> Admin view on usage statistics for aggregation from internal header </remarks>
        public string UsageHistoryForThisCollectionIsDisplayedBelowThisHistoryIncludesJustTheToplevelViewsOfTheCollection { get; private set; }

        /// <remarks> 'VID' localization string </remarks>
        public string VID { get; private set; }

        /// <remarks> 'Views' localization string </remarks>
        public string Views { get; private set; }

        /// <remarks> Admin view on usage statistics for aggregation from internal header </remarks>
        public string Visits { get; private set; }

        /// <remarks> 'Zoomable Views' localization string </remarks>
        public string ZoomableViews { get; private set; }

    }
}
