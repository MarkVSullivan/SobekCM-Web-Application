namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the MainMenus_Helper_HtmlSubWriter class </summary>
    public class MainMenus_Helper_HtmlSubWriter_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the MainMenus_Helper_HtmlSubWriter_Localization class </summary>
        public MainMenus_Helper_HtmlSubWriter_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "MainMenus_Helper_HtmlSubWriter";
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
                case "Added Automatically":
                    AddedAutomatically = Value;
                    break;

                case "Advanced Search":
                    AdvancedSearch = Value;
                    break;

                case "BOOKSHELF VIEW":
                    BOOKSHELFVIEW = Value;
                    break;

                case "Brief View":
                    BriefView = Value;
                    break;

                case "BRIEF VIEW":
                    BRIEFVIEW = Value;
                    break;

                case "BROWSE BY":
                    BROWSEBY = Value;
                    break;

                case "BROWSE PARTNERS":
                    BROWSEPARTNERS = Value;
                    break;

                case "Browse Partners":
                    BrowsePartners = Value;
                    break;

                case "Build Failures":
                    BuildFailures = Value;
                    break;

                case "Collection Hierarchy":
                    CollectionHierarchy = Value;
                    break;

                case "EXPORT":
                    EXPORT = Value;
                    break;

                case "Home":
                    Home = Value;
                    break;

                case "Internal":
                    Internal = Value;
                    break;

                case "List View":
                    ListView = Value;
                    break;

                case "MAP BROWSE":
                    MAPBROWSE = Value;
                    break;

                case "MAP VIEW":
                    MAPVIEW = Value;
                    break;

                case "Memory Management":
                    MemoryManagement = Value;
                    break;

                case "My Account":
                    MyAccount = Value;
                    break;

                case "MY COLLECTIONS":
                    MYCOLLECTIONS = Value;
                    break;

                case "My Library":
                    MyLibrary = Value;
                    break;

                case "New Items":
                    NewItems = Value;
                    break;

                case "Portal Admin":
                    PortalAdmin = Value;
                    break;

                case "Print":
                    Print = Value;
                    break;

                case "Save":
                    Save = Value;
                    break;

                case "SEARCH OPTIONS":
                    SEARCHOPTIONS = Value;
                    break;

                case "Send":
                    Send = Value;
                    break;

                case "System Admin":
                    SystemAdmin = Value;
                    break;

                case "TABLE VIEW":
                    TABLEVIEW = Value;
                    break;

                case "THUMBNAIL VIEW":
                    THUMBNAILVIEW = Value;
                    break;

                case "Tree View":
                    TreeView = Value;
                    break;

                case "View All Items":
                    ViewAllItems = Value;
                    break;

                case "View Items":
                    ViewItems = Value;
                    break;

                case "View Recently Added Items":
                    ViewRecentlyAddedItems = Value;
                    break;

                case "Wordmarks":
                    Wordmarks = Value;
                    break;

            }
        }
        /// <remarks> ' ( Added Automatically )' localization string </remarks>
        public string AddedAutomatically { get; private set; }

        /// <remarks> 'Advanced Search' localization string </remarks>
        public string AdvancedSearch { get; private set; }

        /// <remarks> 'BOOKSHELF VIEW' localization string </remarks>
        public string BOOKSHELFVIEW { get; private set; }

        /// <remarks> 'Brief View' localization string </remarks>
        public string BriefView { get; private set; }

        /// <remarks> 'BRIEF VIEW' localization string </remarks>
        public string BRIEFVIEW { get; private set; }

        /// <remarks> 'BROWSE BY' localization string </remarks>
        public string BROWSEBY { get; private set; }

        /// <remarks> 'BROWSE PARTNERS' localization string </remarks>
        public string BROWSEPARTNERS { get; private set; }

        /// <remarks> 'Browse Partners' localization string </remarks>
        public string BrowsePartners { get; private set; }

        /// <remarks> 'Build Failures' localization string </remarks>
        public string BuildFailures { get; private set; }

        /// <remarks> 'Collection Hierarchy' localization string </remarks>
        public string CollectionHierarchy { get; private set; }

        /// <remarks> 'EXPORT' localization string </remarks>
        public string EXPORT { get; private set; }

        /// <remarks> 'Home' localization string </remarks>
        public string Home { get; private set; }

        /// <remarks> 'Internal' localization string </remarks>
        public string Internal { get; private set; }

        /// <remarks> 'List View' localization string </remarks>
        public string ListView { get; private set; }

        /// <remarks> 'MAP BROWSE' localization string </remarks>
        public string MAPBROWSE { get; private set; }

        /// <remarks> 'MAP VIEW' localization string </remarks>
        public string MAPVIEW { get; private set; }

        /// <remarks> 'Memory Management' localization string </remarks>
        public string MemoryManagement { get; private set; }

        /// <remarks> 'My Account' localization string </remarks>
        public string MyAccount { get; private set; }

        /// <remarks> 'MY COLLECTIONS' localization string </remarks>
        public string MYCOLLECTIONS { get; private set; }

        /// <remarks> 'My Library' localization string </remarks>
        public string MyLibrary { get; private set; }

        /// <remarks> 'New Items' localization string </remarks>
        public string NewItems { get; private set; }

        /// <remarks> 'Portal Admin' localization string </remarks>
        public string PortalAdmin { get; private set; }

        /// <remarks> 'Print' localization string </remarks>
        public string Print { get; private set; }

        /// <remarks> 'Save' localization string </remarks>
        public string Save { get; private set; }

        /// <remarks> 'SEARCH OPTIONS' localization string </remarks>
        public string SEARCHOPTIONS { get; private set; }

        /// <remarks> 'Send' localization string </remarks>
        public string Send { get; private set; }

        /// <remarks> 'System Admin' localization string </remarks>
        public string SystemAdmin { get; private set; }

        /// <remarks> 'TABLE VIEW' localization string </remarks>
        public string TABLEVIEW { get; private set; }

        /// <remarks> 'THUMBNAIL VIEW' localization string </remarks>
        public string THUMBNAILVIEW { get; private set; }

        /// <remarks> 'Tree View' localization string </remarks>
        public string TreeView { get; private set; }

        /// <remarks> 'View All Items' localization string </remarks>
        public string ViewAllItems { get; private set; }

        /// <remarks> 'View Items' localization string </remarks>
        public string ViewItems { get; private set; }

        /// <remarks> 'View Recently Added Items' localization string </remarks>
        public string ViewRecentlyAddedItems { get; private set; }

        /// <remarks> 'Wordmarks' localization string </remarks>
        public string Wordmarks { get; private set; }

    }
}
