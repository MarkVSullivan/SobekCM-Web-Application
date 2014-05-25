namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Aggregation_HtmlSubwriter class </summary>
    public class Aggregation_HtmlSubwriter_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Aggregation_HtmlSubwriter_Localization class </summary>
        public Aggregation_HtmlSubwriter_LocalizationInfo() : base()
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
                case "ADD":
                    ADD = Value;
                    break;

                case "Add This To My Collections Home Page":
                    AddThisToMyCollectionsHomePage = Value;
                    break;

                case "Aggregation_Htmlsubwriter":
                    Aggregation_Htmlsubwriter = Value;
                    break;

                case "All Collections":
                    AllCollections = Value;
                    break;

                case "All Items":
                    AllItems = Value;
                    break;

                case "BRIEF VIEW":
                    BRIEFVIEW = Value;
                    break;

                case "Browse By":
                    BrowseBy = Value;
                    break;

                case "Browse Partners":
                    BrowsePartners = Value;
                    break;

                case "Cancel":
                    Cancel = Value;
                    break;

                case "Close":
                    Close = Value;
                    break;

                case "Collapse All":
                    CollapseAll = Value;
                    break;

                case "Comments":
                    Comments = Value;
                    break;

                case "Do Not Apply Changes":
                    DoNotApplyChanges = Value;
                    break;

                case "Edit Administrative Information":
                    EditAdministrativeInformation = Value;
                    break;

                case "Edit Content":
                    EditContent = Value;
                    break;

                case "Edit This Home Text":
                    EditThisHomeText = Value;
                    break;

                case "Enter The Email Information Below":
                    EnterTheEmailInformationBelow = Value;
                    break;

                case "Expand All":
                    ExpandAll = Value;
                    break;

                case "Format":
                    Format = Value;
                    break;

                case "Go To My XXX Home":
                    GoToMyXXXHome = Value;
                    break;

                case "Go To My Bookshelf":
                    GoToMyBookshelf = Value;
                    break;

                case "Go To My Saved Searches":
                    GoToMySavedSearches = Value;
                    break;

                case "Help Regarding This Header":
                    HelpRegardingThisHeader = Value;
                    break;

                case "Hide Collection Groups":
                    HideCollectionGroups = Value;
                    break;

                case "Hide Collections":
                    HideCollections = Value;
                    break;

                case "Hide Internal Header":
                    HideInternalHeader = Value;
                    break;

                case "Hide Subcollections":
                    HideSubcollections = Value;
                    break;

                case "Home":
                    Home = Value;
                    break;

                case "HTML":
                    HTML = Value;
                    break;

                case "LIST VIEW":
                    LISTVIEW = Value;
                    break;

                case "Make Private":
                    MakePrivate = Value;
                    break;

                case "Map Browse":
                    MapBrowse = Value;
                    break;

                case "My Collections":
                    MyCollections = Value;
                    break;

                case "My Library":
                    MyLibrary = Value;
                    break;

                case "My Links":
                    MyLinks = Value;
                    break;

                case "My Public Bookshelves":
                    MyPublicBookshelves = Value;
                    break;

                case "My Saved Searches":
                    MySavedSearches = Value;
                    break;

                case "New Items":
                    NewItems = Value;
                    break;

                case "Partners And Contributing Institutions":
                    PartnersAndContributingInstitutions = Value;
                    break;

                case "Partners Collaborating And Contributing To Digital Collections And Libraries Include":
                    PartnersCollaboratingAndContributingToDigitalCollectionsAndLibrariesInclude = Value;
                    break;

                case "Perform Search":
                    PerformSearch = Value;
                    break;

                case "Plain Text":
                    PlainText = Value;
                    break;

                case "PRINT":
                    PRINT = Value;
                    break;

                case "Print This Page":
                    PrintThisPage = Value;
                    break;

                case "REMOVE":
                    REMOVE = Value;
                    break;

                case "Remove This From My Collections Home Page":
                    RemoveThisFromMyCollectionsHomePage = Value;
                    break;

                case "Removed Aggregation From Your Home Page":
                    RemovedAggregationFromYourHomePage = Value;
                    break;

                case "Save":
                    Save = Value;
                    break;

                case "Save Changes To This Aggregation Home Page Text":
                    SaveChangesToThisAggregationHomePageText = Value;
                    break;

                case "Save This To My Collections Home Page":
                    SaveThisToMyCollectionsHomePage = Value;
                    break;

                case "Select Collection Groups To Include In Search":
                    SelectCollectionGroupsToIncludeInSearch = Value;
                    break;

                case "Select Collections To Include In Search":
                    SelectCollectionsToIncludeInSearch = Value;
                    break;

                case "Select Subcollections To Include In Search":
                    SelectSubcollectionsToIncludeInSearch = Value;
                    break;

                case "Send":
                    Send = Value;
                    break;

                case "SEND":
                    SEND = Value;
                    break;

                case "Send This Collection To A Friend":
                    SendThisCollectionToAFriend = Value;
                    break;

                case "Send This To Someone":
                    SendThisToSomeone = Value;
                    break;

                case "Share This Collection":
                    ShareThisCollection = Value;
                    break;

                case "Show Collection Groups":
                    ShowCollectionGroups = Value;
                    break;

                case "Show Collections":
                    ShowCollections = Value;
                    break;

                case "Show Internal Header":
                    ShowInternalHeader = Value;
                    break;

                case "Show Subcollections":
                    ShowSubcollections = Value;
                    break;

                case "THUMBNAIL VIEW":
                    THUMBNAILVIEW = Value;
                    break;

                case "To Add A Collection Use The XXX Button From That Collections Home Page":
                    ToAddACollectionUseTheXXXButtonFromThatCollectionsHomePage = Value;
                    break;

                case "To":
                    To = Value;
                    break;

                case "TREE VIEW":
                    TREEVIEW = Value;
                    break;

                case "View Administrative Information":
                    ViewAdministrativeInformation = Value;
                    break;

                case "View Item Count":
                    ViewItemCount = Value;
                    break;

                case "View Private Items":
                    ViewPrivateItems = Value;
                    break;

                case "View Usage Statistics":
                    ViewUsageStatistics = Value;
                    break;

                case "Welcome To Your Personalized XXX Home Page This Page Displays Any Collections You Have Added As Well As Any Of Your Bookshelves You Have Made Public":
                    WelcomeToYourPersonalizedXXXHomePageThisPageDisplaysAnyCollectionsYouHaveAddedAsWellAsAnyOfYourBookshelvesYouHaveMadePublic = Value;
                    break;

                case "You Do Not Have Any Collections Added To Your Home Page":
                    YouDoNotHaveAnyCollectionsAddedToYourHomePage = Value;
                    break;

                case "Your Email Has Been Sent":
                    YourEmailHasBeenSent = Value;
                    break;

            }
        }
        /// <remarks> 'ADD' localization string </remarks>
        public string ADD { get; private set; }

        /// <remarks> 'Add this to my collections home page' localization string </remarks>
        public string AddThisToMyCollectionsHomePage { get; private set; }

        /// <remarks> 'Aggregation_HtmlSubwriter' localization string </remarks>
        public string Aggregation_Htmlsubwriter { get; private set; }

        /// <remarks> 'All collections' localization string </remarks>
        public string AllCollections { get; private set; }

        /// <remarks> Tabs on aggregation home page - shows all items in the aggregation </remarks>
        public string AllItems { get; private set; }

        /// <remarks> 'BRIEF VIEW' localization string </remarks>
        public string BRIEFVIEW { get; private set; }

        /// <remarks> Tabs on aggregation home page - Metadata browse tab </remarks>
        public string BrowseBy { get; private set; }

        /// <remarks> Tabs on aggregation home page - Browse the list of partners </remarks>
        public string BrowsePartners { get; private set; }

        /// <remarks> 'Cancel' localization string </remarks>
        public string Cancel { get; private set; }

        /// <remarks> 'Close' localization string </remarks>
        public string Close { get; private set; }

        /// <remarks> 'Collapse All' localization string </remarks>
        public string CollapseAll { get; private set; }

        /// <remarks> Prompt for user comments </remarks>
        public string Comments { get; private set; }

        /// <remarks> 'Do not apply changes' localization string </remarks>
        public string DoNotApplyChanges { get; private set; }

        /// <remarks> "Used in the internal header, when an internal user is logged on" </remarks>
        public string EditAdministrativeInformation { get; private set; }

        /// <remarks> 'edit content' localization string </remarks>
        public string EditContent { get; private set; }

        /// <remarks> 'Edit this home text' localization string </remarks>
        public string EditThisHomeText { get; private set; }

        /// <remarks> Prompt for emailing </remarks>
        public string EnterTheEmailInformationBelow { get; private set; }

        /// <remarks> 'Expand All' localization string </remarks>
        public string ExpandAll { get; private set; }

        /// <remarks> Prompt for whether to send HTML or Plain text email </remarks>
        public string Format { get; private set; }

        /// <remarks> 'Go to my %1 home' localization string </remarks>
        public string GoToMyXXXHome { get; private set; }

        /// <remarks> 'Go to my bookshelf' localization string </remarks>
        public string GoToMyBookshelf { get; private set; }

        /// <remarks> 'Go to my saved searches' localization string </remarks>
        public string GoToMySavedSearches { get; private set; }

        /// <remarks> "Used in the internal header, when an internal user is logged on" </remarks>
        public string HelpRegardingThisHeader { get; private set; }

        /// <remarks> Part of deprecated code for the old menu </remarks>
        public string HideCollectionGroups { get; private set; }

        /// <remarks> Part of deprecated code for the old menu </remarks>
        public string HideCollections { get; private set; }

        /// <remarks> "Used in the internal header, when an internal user is logged on" </remarks>
        public string HideInternalHeader { get; private set; }

        /// <remarks> Part of deprecated code for the old menu </remarks>
        public string HideSubcollections { get; private set; }

        /// <remarks> Tabs on aggregation home page - return to aggregation home page </remarks>
        public string Home { get; private set; }

        /// <remarks> 'HTML' localization string </remarks>
        public string HTML { get; private set; }

        /// <remarks> 'LIST VIEW' localization string </remarks>
        public string LISTVIEW { get; private set; }

        /// <remarks> 'make private' localization string </remarks>
        public string MakePrivate { get; private set; }

        /// <remarks> Tabs on aggregation home page - Browse items in the collection on a map interface </remarks>
        public string MapBrowse { get; private set; }

        /// <remarks> Tabs on aggregation home page - Goes to a list of the collections user has added to their home page </remarks>
        public string MyCollections { get; private set; }

        /// <remarks> 'My Library' localization string </remarks>
        public string MyLibrary { get; private set; }

        /// <remarks> 'My Links' localization string </remarks>
        public string MyLinks { get; private set; }

        /// <remarks> 'My Public Bookshelves' localization string </remarks>
        public string MyPublicBookshelves { get; private set; }

        /// <remarks> 'My Saved Searches' localization string </remarks>
        public string MySavedSearches { get; private set; }

        /// <remarks> Tabs on aggregation home page - Shows items recently added to the aggregation </remarks>
        public string NewItems { get; private set; }

        /// <remarks> 'Partners and Contributing Institutions' localization string </remarks>
        public string PartnersAndContributingInstitutions { get; private set; }

        /// <remarks> Used for the automatic partner browse from the library home page.   </remarks>
        public string PartnersCollaboratingAndContributingToDigitalCollectionsAndLibrariesInclude { get; private set; }

        /// <remarks> "Used in the internal header, when an internal user is logged on" </remarks>
        public string PerformSearch { get; private set; }

        /// <remarks> 'Plain text' localization string </remarks>
        public string PlainText { get; private set; }

        /// <remarks> 'PRINT' localization string </remarks>
        public string PRINT { get; private set; }

        /// <remarks> 'Print this page' localization string </remarks>
        public string PrintThisPage { get; private set; }

        /// <remarks> 'REMOVE' localization string </remarks>
        public string REMOVE { get; private set; }

        /// <remarks> 'Remove this from my collections home page' localization string </remarks>
        public string RemoveThisFromMyCollectionsHomePage { get; private set; }

        /// <remarks> 'Removed aggregation from your home page' localization string </remarks>
        public string RemovedAggregationFromYourHomePage { get; private set; }

        /// <remarks> 'Save' localization string </remarks>
        public string Save { get; private set; }

        /// <remarks> 'Save changes to this aggregation home page text' localization string </remarks>
        public string SaveChangesToThisAggregationHomePageText { get; private set; }

        /// <remarks> 'Save this to my collections home page' localization string </remarks>
        public string SaveThisToMyCollectionsHomePage { get; private set; }

        /// <remarks> Part of deprecated code for the old menu </remarks>
        public string SelectCollectionGroupsToIncludeInSearch { get; private set; }

        /// <remarks> Part of deprecated code for the old menu </remarks>
        public string SelectCollectionsToIncludeInSearch { get; private set; }

        /// <remarks> Part of deprecated code for the old menu </remarks>
        public string SelectSubcollectionsToIncludeInSearch { get; private set; }

        /// <remarks> Button name for sending the email </remarks>
        public string Send { get; private set; }

        /// <remarks> 'SEND' localization string </remarks>
        public string SEND { get; private set; }

        /// <remarks> Used when a user chooses to send an email from the collection home page </remarks>
        public string SendThisCollectionToAFriend { get; private set; }

        /// <remarks> 'Send this to someone' localization string </remarks>
        public string SendThisToSomeone { get; private set; }

        /// <remarks> 'Share this collection' localization string </remarks>
        public string ShareThisCollection { get; private set; }

        /// <remarks> Part of deprecated code for the old menu </remarks>
        public string ShowCollectionGroups { get; private set; }

        /// <remarks> Part of deprecated code for the old menu </remarks>
        public string ShowCollections { get; private set; }

        /// <remarks> "Used in the internal header, when an internal user is logged on" </remarks>
        public string ShowInternalHeader { get; private set; }

        /// <remarks> Part of deprecated code for the old menu </remarks>
        public string ShowSubcollections { get; private set; }

        /// <remarks> 'THUMBNAIL VIEW' localization string </remarks>
        public string THUMBNAILVIEW { get; private set; }

        /// <remarks> '"To add a collection, use the %1 button from that collection's home page."' localization string </remarks>
        public string ToAddACollectionUseTheXXXButtonFromThatCollectionsHomePage { get; private set; }

        /// <remarks> Where user enters the email address(es) to send to </remarks>
        public string To { get; private set; }

        /// <remarks> 'TREE VIEW' localization string </remarks>
        public string TREEVIEW { get; private set; }

        /// <remarks> "Used in the internal header, when an internal user is logged on" </remarks>
        public string ViewAdministrativeInformation { get; private set; }

        /// <remarks> "Used in the internal header, when an internal user is logged on" </remarks>
        public string ViewItemCount { get; private set; }

        /// <remarks> "Used in the internal header, when an internal user is logged on" </remarks>
        public string ViewPrivateItems { get; private set; }

        /// <remarks> "Used in the internal header, when an internal user is logged on" </remarks>
        public string ViewUsageStatistics { get; private set; }

        /// <remarks> '"Welcome to your personalized %1 home page. This page displays any collections you have added, as well as any of your bookshelves you have made public."' localization string </remarks>
        public string WelcomeToYourPersonalizedXXXHomePageThisPageDisplaysAnyCollectionsYouHaveAddedAsWellAsAnyOfYourBookshelvesYouHaveMadePublic { get; private set; }

        /// <remarks> 'You do not have any collections added to your home page.' localization string </remarks>
        public string YouDoNotHaveAnyCollectionsAddedToYourHomePage { get; private set; }

        /// <remarks> 'Your email has been sent' localization string </remarks>
        public string YourEmailHasBeenSent { get; private set; }

    }
}
