namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Usage_Statistics_AggregationViewer class </summary>
    public class Usage_Statistics_AggregationViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Usage_Statistics_AggregationViewer_Localization class </summary>
        public Usage_Statistics_AggregationViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Usage_Statistics_AggregationViewer";
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
                case "History Of Collectionlevel Usage":
                    HistoryOfCollectionlevelUsage = Value;
                    break;

                case "History Of Item Usage":
                    HistoryOfItemUsage = Value;
                    break;

                case "Most Accessed Titles":
                    MostAccessedTitles = Value;
                    break;

                case "Most Accessed Items":
                    MostAccessedItems = Value;
                    break;

                case "Definitions Of Terms Used":
                    DefinitionsOfTermsUsed = Value;
                    break;

                case "Collection Views":
                    CollectionViews = Value;
                    break;

                case "Item Views":
                    ItemViews = Value;
                    break;

                case "Top Titles":
                    TopTitles = Value;
                    break;

                case "Top Items":
                    TopItems = Value;
                    break;

                case "Definitions":
                    Definitions = Value;
                    break;

                case "Usage History For This Collection Is Displayed Below This History Includes Just The Toplevel Views Of The Collection":
                    UsageHistoryForThisCollectionIsDisplayedBel = Value;
                    break;

                case "Definitions Page Provides More Details About The Statistics And Words Used Below":
                    DefinitionsPageProvidesMoreDetailsAboutTheS = Value;
                    break;

                case "Date":
                    Date = Value;
                    break;

                case "Total Views":
                    TotalViews = Value;
                    break;

                case "Visits":
                    Visits = Value;
                    break;

                case "Main Pages":
                    MainPages = Value;
                    break;

                case "Browses":
                    Browses = Value;
                    break;

                case "Search Results":
                    SearchResults = Value;
                    break;

                case "Title Views":
                    TitleViews = Value;
                    break;

                case "Usage History For The Items Within This Collection Are Displayed Below":
                    UsageHistoryForTheItemsWithinThisCollection = Value;
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

                case "The Most Commonly Utilized Items For This Collection Appear Below":
                    TheMostCommonlyUtilizedItemsForThisCollecti = Value;
                    break;

                case "Title":
                    Title = Value;
                    break;

                case "Views":
                    Views = Value;
                    break;

                case "The Most Commonly Utilized Titles By Collection Appear Below":
                    TheMostCommonlyUtilizedTitlesByCollectionAp = Value;
                    break;

                case "XXX Statistics":
                    XXXStatistics = Value;
                    break;

                case "Collection Hierarchy":
                    CollectionHierarchy = Value;
                    break;

                case "Collection Groups":
                    CollectionGroups = Value;
                    break;

                case "Collections":
                    Collections = Value;
                    break;

                case "Subcollections":
                    Subcollections = Value;
                    break;

                case "Titles And Items":
                    TitlesAndItems = Value;
                    break;

                case "Bibid":
                    Bibid = Value;
                    break;

                case "VID":
                    VID = Value;
                    break;

            }
        }
        /// <remarks> 'History of Collection-Level Usage' localization string </remarks>
        public string HistoryOfCollectionlevelUsage { get; private set; }

        /// <remarks> 'History of Item Usage' localization string </remarks>
        public string HistoryOfItemUsage { get; private set; }

        /// <remarks> 'Most Accessed Titles' localization string </remarks>
        public string MostAccessedTitles { get; private set; }

        /// <remarks> 'Most Accessed Items' localization string </remarks>
        public string MostAccessedItems { get; private set; }

        /// <remarks> 'Definitions of Terms Used' localization string </remarks>
        public string DefinitionsOfTermsUsed { get; private set; }

        /// <remarks> 'Collection Views' localization string </remarks>
        public string CollectionViews { get; private set; }

        /// <remarks> 'Item Views' localization string </remarks>
        public string ItemViews { get; private set; }

        /// <remarks> 'Top Titles' localization string </remarks>
        public string TopTitles { get; private set; }

        /// <remarks> 'Top Items' localization string </remarks>
        public string TopItems { get; private set; }

        /// <remarks> 'Definitions' localization string </remarks>
        public string Definitions { get; private set; }

        /// <remarks> 'Usage history for this collection is displayed below. This history includes just the top-level views of the collection.' localization string </remarks>
        public string UsageHistoryForThisCollectionIsDisplayedBel { get; private set; }

        /// <remarks> 'Definitions page provides more details about the statistics and words used below.' localization string </remarks>
        public string DefinitionsPageProvidesMoreDetailsAboutTheS { get; private set; }

        /// <remarks> 'Date' localization string </remarks>
        public string Date { get; private set; }

        /// <remarks> 'Total Views' localization string </remarks>
        public string TotalViews { get; private set; }

        /// <remarks> 'Visits' localization string </remarks>
        public string Visits { get; private set; }

        /// <remarks> 'Main Pages' localization string </remarks>
        public string MainPages { get; private set; }

        /// <remarks> 'Browses' localization string </remarks>
        public string Browses { get; private set; }

        /// <remarks> 'Search Results' localization string </remarks>
        public string SearchResults { get; private set; }

        /// <remarks> 'Title Views' localization string </remarks>
        public string TitleViews { get; private set; }

        /// <remarks> 'Usage history for the items within this collection are displayed below.' localization string </remarks>
        public string UsageHistoryForTheItemsWithinThisCollection { get; private set; }

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

        /// <remarks> 'The most commonly utilized items for this collection appear below.' localization string </remarks>
        public string TheMostCommonlyUtilizedItemsForThisCollecti { get; private set; }

        /// <remarks> 'Title' localization string </remarks>
        public string Title { get; private set; }

        /// <remarks> 'Views' localization string </remarks>
        public string Views { get; private set; }

        /// <remarks> 'The most commonly utilized titles by collection appear below.' localization string </remarks>
        public string TheMostCommonlyUtilizedTitlesByCollectionAp { get; private set; }

        /// <remarks> '%1 Statistics' localization string </remarks>
        public string XXXStatistics { get; private set; }

        /// <remarks> 'Collection Hierarchy' localization string </remarks>
        public string CollectionHierarchy { get; private set; }

        /// <remarks> 'Collection Groups' localization string </remarks>
        public string CollectionGroups { get; private set; }

        /// <remarks> 'Collections' localization string </remarks>
        public string Collections { get; private set; }

        /// <remarks> 'SubCollections' localization string </remarks>
        public string Subcollections { get; private set; }

        /// <remarks> 'Titles and Items' localization string </remarks>
        public string TitlesAndItems { get; private set; }

        /// <remarks> 'BibID' localization string </remarks>
        public string Bibid { get; private set; }

        /// <remarks> 'VID' localization string </remarks>
        public string VID { get; private set; }

    }
}
