namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Aggregation_Single_AdminViewer class </summary>
    public class Aggregation_Single_AdminViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Aggregation_Single_AdminViewer_Localization class </summary>
        public Aggregation_Single_AdminViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Aggregation_Single_AdminViewer";
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
                case "NO DEFAULT":
                    NODEFAULT = Value;
                    break;

                case "None Top Level":
                    NoneTopLevel = Value;
                    break;

                case "ACTION":
                    ACTION = Value;
                    break;

                case "ACTIONS":
                    ACTIONS = Value;
                    break;

                case "Active":
                    Active = Value;
                    break;

                case "Add":
                    Add = Value;
                    break;

                case "Add New Banner":
                    AddNewBanner = Value;
                    break;

                case "Add New Home Page":
                    AddNewHomePage = Value;
                    break;

                case "Additional Dublin Core Metadata To Include In OAIPMH Set List":
                    AdditionalDublinCoreMetadataToIncludeInOAIPMHSetList = Value;
                    break;

                case "Advanced Search":
                    AdvancedSearch = Value;
                    break;

                case "Advanced Search With Year Range":
                    AdvancedSearchWithYearRange = Value;
                    break;

                case "Africa":
                    Africa = Value;
                    break;

                case "Aggregationlevel Custom Stylesheet CSS":
                    AggregationlevelCustomStylesheetCSS = Value;
                    break;

                case "Appearance":
                    Appearance = Value;
                    break;

                case "Appearance Options":
                    AppearanceOptions = Value;
                    break;

                case "Asia":
                    Asia = Value;
                    break;

                case "BACK":
                    BACK = Value;
                    break;

                case "Banner Type":
                    BannerType = Value;
                    break;

                case "Banners":
                    Banners = Value;
                    break;

                case "Basic Information":
                    BasicInformation = Value;
                    break;

                case "Basic Search With Year Range":
                    BasicSearchWithYearRange = Value;
                    break;

                case "Behavior":
                    Behavior = Value;
                    break;

                case "Brief View":
                    BriefView = Value;
                    break;

                case "Browse By":
                    BrowseBy = Value;
                    break;

                case "CANCEL":
                    CANCEL = Value;
                    break;

                case "Caribbean":
                    Caribbean = Value;
                    break;

                case "Child Pages":
                    ChildPages = Value;
                    break;

                case "Child Pages Are Pages Related To The Aggregation And Allow Additional Information To Be Presented Within The Same Aggregational Branding These Can Appear In The Aggregation Main Menu With Any Metadata Browses Pulled From The Database Or You Can Set Them To For No Automatic Visibility In Which Case They Are Only Accessible By Links In The Home Page Or Other Child Pages":
                    ChildPagesArePagesRelatedToTheAggregationAndAllowAdditionalInformationToBePresentedWithinTheSameAggregationalBrandingTheseCanAppearInTheAggregationMainMenuWithAnyMetadataBrowsesPulledFromTheDatabaseOrYouCanSetThemToForNoAutomaticVisibilityInWhichCaseTheyAreOnlyAccessibleByLinksInTheHomePageOrOtherChildPages = Value;
                    break;

                case "Click Here To View The Help Page":
                    ClickHereToViewTheHelpPage = Value;
                    break;

                case "Click To Delete This Subcollection":
                    ClickToDeleteThisSubcollection = Value;
                    break;

                case "Close This Child Page Details And Return To Main Admin Pages":
                    CloseThisChildPageDetailsAndReturnToMainAdminPages = Value;
                    break;

                case "CODE":
                    CODE = Value;
                    break;

                case "Collection":
                    Collection = Value;
                    break;

                case "Collection Button":
                    CollectionButton = Value;
                    break;

                case "Collection Visibility":
                    CollectionVisibility = Value;
                    break;

                case "Contact Email":
                    ContactEmail = Value;
                    break;

                case "Copy From Existing":
                    CopyFromExisting = Value;
                    break;

                case "Copy From Existing Home":
                    CopyFromExistingHome = Value;
                    break;

                case "Custom Home Page":
                    CustomHomePage = Value;
                    break;

                case "Custom Stylesheet":
                    CustomStylesheet = Value;
                    break;

                case "Default":
                    Default = Value;
                    break;

                case "Default Browse":
                    DefaultBrowse = Value;
                    break;

                case "Default Result View":
                    DefaultResultView = Value;
                    break;

                case "Delete":
                    Delete = Value;
                    break;

                case "Delete This XXX Version":
                    DeleteThisXXXVersion = Value;
                    break;

                case "Delete This Banner":
                    DeleteThisBanner = Value;
                    break;

                case "DELETE THIS HIGHLIGHT":
                    DELETETHISHIGHLIGHT = Value;
                    break;

                case "Deleted XXX Subcollection":
                    DeletedXXXSubcollection = Value;
                    break;

                case "Description":
                    Description = Value;
                    break;

                case "Disable This Aggregationlevel Stylesheet":
                    DisableThisAggregationlevelStylesheet = Value;
                    break;

                case "Dloc Full Text Search":
                    DlocFullTextSearch = Value;
                    break;

                case "Do Not Apply Changes":
                    DoNotApplyChanges = Value;
                    break;

                case "Edit":
                    Edit = Value;
                    break;

                case "Edit Child Page Detail":
                    EditChildPageDetail = Value;
                    break;

                case "Edit This Aggregationlevel Stylesheet":
                    EditThisAggregationlevelStylesheet = Value;
                    break;

                case "Edit This Child Page":
                    EditThisChildPage = Value;
                    break;

                case "Edit This Child Page In XXX":
                    EditThisChildPageInXXX = Value;
                    break;

                case "Edit This Home Page":
                    EditThisHomePage = Value;
                    break;

                case "Edit This Home Page In XXX":
                    EditThisHomePageInXXX = Value;
                    break;

                case "Edit This Subcollection":
                    EditThisSubcollection = Value;
                    break;

                case "ENABLE":
                    ENABLE = Value;
                    break;

                case "Enable An Aggregationlevel Stylesheet":
                    EnableAnAggregationlevelStylesheet = Value;
                    break;

                case "Enabled Flag":
                    EnabledFlag = Value;
                    break;

                case "Error Adding New Version To This Child Page":
                    ErrorAddingNewVersionToThisChildPage = Value;
                    break;

                case "Error Saving Aggregation Information":
                    ErrorSavingAggregationInformation = Value;
                    break;

                case "ERROR Saving The New Item Aggregation To The Database":
                    ERRORSavingTheNewItemAggregationToTheDatabase = Value;
                    break;

                case "ERROR Invalid Entry For New Item Aggregation":
                    ERRORInvalidEntryForNewItemAggregation = Value;
                    break;

                case "ERROR Invalid Entry For New Item Child Page":
                    ERRORInvalidEntryForNewItemChildPage = Value;
                    break;

                case "Europe":
                    Europe = Value;
                    break;

                case "Exhibit":
                    Exhibit = Value;
                    break;

                case "Existing Banners":
                    ExistingBanners = Value;
                    break;

                case "Existing Child Pages":
                    ExistingChildPages = Value;
                    break;

                case "Existing Home Pages":
                    ExistingHomePages = Value;
                    break;

                case "Existing Subcollections":
                    ExistingSubcollections = Value;
                    break;

                case "Existing Versions":
                    ExistingVersions = Value;
                    break;

                case "External Link":
                    ExternalLink = Value;
                    break;

                case "Florida":
                    Florida = Value;
                    break;

                case "For More Information About The Settings On This Tab":
                    ForMoreInformationAboutTheSettingsOnThisTab = Value;
                    break;

                case "Full Text Search":
                    FullTextSearch = Value;
                    break;

                case "Full View":
                    FullView = Value;
                    break;

                case "General":
                    General = Value;
                    break;

                case "Highlights":
                    Highlights = Value;
                    break;

                case "Highlights Type":
                    HighlightsType = Value;
                    break;

                case "Home Page":
                    HomePage = Value;
                    break;

                case "Home Page Left":
                    HomePageLeft = Value;
                    break;

                case "Home Page Right":
                    HomePageRight = Value;
                    break;

                case "Home Page Text":
                    HomePageText = Value;
                    break;

                case "Image":
                    Image = Value;
                    break;

                case "Include All New Item Browses":
                    IncludeAllNewItemBrowses = Value;
                    break;

                case "Include In OAIPMH As A Set":
                    IncludeInOAIPMHAsASet = Value;
                    break;

                case "Include Map Browse":
                    IncludeMapBrowse = Value;
                    break;

                case "Institutional Division":
                    InstitutionalDivision = Value;
                    break;

                case "LANGUAGE":
                    LANGUAGE = Value;
                    break;

                case "LANGUAGES":
                    LANGUAGES = Value;
                    break;

                case "Language":
                    Language = Value;
                    break;

                case "Link":
                    Link = Value;
                    break;

                case "Main Menu":
                    MainMenu = Value;
                    break;

                case "Map Search Beta":
                    MapSearchBeta = Value;
                    break;

                case "Map Search Legacy":
                    MapSearchLegacy = Value;
                    break;

                case "Map Search Default Area":
                    MapSearchDefaultArea = Value;
                    break;

                case "Metadata":
                    Metadata = Value;
                    break;

                case "Metadata Browses":
                    MetadataBrowses = Value;
                    break;

                case "Middle East":
                    MiddleEast = Value;
                    break;

                case "Name Full":
                    NameFull = Value;
                    break;

                case "Name Short":
                    NameShort = Value;
                    break;

                case "New Aggregation Code Must Be Twenty Characters Long Or Less":
                    NewAggregationCodeMustBeTwentyCharactersLongOrLess = Value;
                    break;

                case "New Banner":
                    NewBanner = Value;
                    break;

                case "New Child Page Code Must Be Twenty Characters Long Or Less":
                    NewChildPageCodeMustBeTwentyCharactersLongOrLess = Value;
                    break;

                case "New Child Page":
                    NewChildPage = Value;
                    break;

                case "New Code Must Be Unique XXX Already Exists":
                    NewCodeMustBeUniqueXXXAlreadyExists = Value;
                    break;

                case "New Collection Home Page Text Goes Here":
                    NewCollectionHomePageTextGoesHere = Value;
                    break;

                case "New Home Page Text In XXX Goes Here":
                    NewHomePageTextInXXXGoesHere = Value;
                    break;

                case "New Home Page":
                    NewHomePage = Value;
                    break;

                case "New Item Aggregation XXX Saved Successfully":
                    NewItemAggregationXXXSavedSuccessfully = Value;
                    break;

                case "New Subcollection":
                    NewSubcollection = Value;
                    break;

                case "No Custom Aggregationlevel Stylesheet":
                    NoCustomAggregationlevelStylesheet = Value;
                    break;

                case "No Html Source Files":
                    NoHtmlSourceFiles = Value;
                    break;

                case "NONE":
                    NONE = Value;
                    break;

                case "North America":
                    NorthAmerica = Value;
                    break;

                case "NOTE You May Need To Refresh Your Browser For Your Changes To Take Affect":
                    NOTEYouMayNeedToRefreshYourBrowserForYourChangesToTakeAffect = Value;
                    break;

                case "OAIPMH Settings":
                    OAIPMHSettings = Value;
                    break;

                case "Other Display Types":
                    OtherDisplayTypes = Value;
                    break;

                case "PARENT":
                    PARENT = Value;
                    break;

                case "Parent Codes":
                    ParentCodes = Value;
                    break;

                case "Parent":
                    Parent = Value;
                    break;

                case "Result Views":
                    ResultViews = Value;
                    break;

                case "Results":
                    Results = Value;
                    break;

                case "Results Options":
                    ResultsOptions = Value;
                    break;

                case "Rotating":
                    Rotating = Value;
                    break;

                case "SAVE":
                    SAVE = Value;
                    break;

                case "Save Changes To This Item Aggregation":
                    SaveChangesToThisItemAggregation = Value;
                    break;

                case "Save New Child Page":
                    SaveNewChildPage = Value;
                    break;

                case "Save New Version Of This Child Page":
                    SaveNewVersionOfThisChildPage = Value;
                    break;

                case "Search":
                    Search = Value;
                    break;

                case "Search Options":
                    SearchOptions = Value;
                    break;

                case "Search Types":
                    SearchTypes = Value;
                    break;

                case "Selected Image File":
                    SelectedImageFile = Value;
                    break;

                case "Show In Parent Collection Home Page":
                    ShowInParentCollectionHomePage = Value;
                    break;

                case "Some Of The Files May Be In Use":
                    SomeOfTheFilesMayBeInUse = Value;
                    break;

                case "SOURCE FILE":
                    SOURCEFILE = Value;
                    break;

                case "South America":
                    SouthAmerica = Value;
                    break;

                case "Standard":
                    Standard = Value;
                    break;

                case "Static":
                    Static = Value;
                    break;

                case "Subcollections":
                    Subcollections = Value;
                    break;

                case "Table View":
                    TableView = Value;
                    break;

                case "Text":
                    Text = Value;
                    break;

                case "That Code Is A Systemreserved Keyword Try A Different Code":
                    ThatCodeIsASystemreservedKeywordTryADifferentCode = Value;
                    break;

                case "The Code For This Browse Is XXX":
                    TheCodeForThisBrowseIsXXX = Value;
                    break;

                case "The Information In This Section Controls How Search Results Or Item Browses Appears The Facet Options Control Which Metadata Values Appear To The Left Of The Results To Allow Users To Narrow Their Results The Search Results Values Determine Which Options Are Available For Viewing The Results And What Are The Aggregation Defaults Finally The Result Fields Determines Which Values Are Displayed With Each Individual Result In The Result Set":
                    TheInformationInThisSectionControlsHowSearchResultsOrItemBrowsesAppearsTheFacetOptionsControlWhichMetadataValuesAppearToTheLeftOfTheResultsToAllowUsersToNarrowTheirResultsTheSearchResultsValuesDetermineWhichOptionsAreAvailableForViewingTheResultsAndWhatAreTheAggregationDefaultsFinallyTheResultFieldsDeterminesWhichValuesAreDisplayedWithEachIndividualResultInTheResultSet = Value;
                    break;

                case "The Information In This Section Is The Basic Information About The Aggregation Such As The Full Name The Shortened Name Used For Breadcrumbs The Description And The Email Contact":
                    TheInformationInThisSectionIsTheBasicInformationAboutTheAggregationSuchAsTheFullNameTheShortenedNameUsedForBreadcrumbsTheDescriptionAndTheEmailContact = Value;
                    break;

                case "The Metadata Browses Can Be Used To Expose All The Metadata Of The Resources Within This Aggregation For Public Browsing Select The Metadata Fields You Would Like Have Available Below":
                    TheMetadataBrowsesCanBeUsedToExposeAllTheMetadataOfTheResourcesWithinThisAggregationForPublicBrowsingSelectTheMetadataFieldsYouWouldLikeHaveAvailableBelow = Value;
                    break;

                case "The Values In This Section Determine If The Collection Is Currently Visible At All Whether It Is Eligible To Appear On The Collection List At The Bottom Of The Parent Page And The Collection Button Used In That Case Thematic Headings Are Used To Place This Collection On The Main Home Page":
                    TheValuesInThisSectionDetermineIfTheCollectionIsCurrentlyVisibleAtAllWhetherItIsEligibleToAppearOnTheCollectionListAtTheBottomOfTheParentPageAndTheCollectionButtonUsedInThatCaseThematicHeadingsAreUsedToPlaceThisCollectionOnTheMainHomePage = Value;
                    break;

                case "Thematic Heading":
                    ThematicHeading = Value;
                    break;

                case "These Options Control How Searching Works Within This Aggregation Such As Which Search Options Are Made Publicly Available":
                    TheseOptionsControlHowSearchingWorksWithinThisAggregationSuchAsWhichSearchOptionsAreMadePubliclyAvailable = Value;
                    break;

                case "These Three Settings Have The Most Profound Affects On The Appearance Of This Aggregation By Forcing It To Appear Under A Particular Web Skin Allowing A Custom Aggregationlevel Stylesheet Or Completely Overriding The Systemgenerated Home Page For A Custom Home Page HTML Source File":
                    TheseThreeSettingsHaveTheMostProfoundAffectsOnTheAppearanceOfThisAggregationByForcingItToAppearUnderAParticularWebSkinAllowingACustomAggregationlevelStylesheetOrCompletelyOverridingTheSystemgeneratedHomePageForACustomHomePageHTMLSourceFile = Value;
                    break;

                case "This Aggregation Currently Has No Child Pages":
                    ThisAggregationCurrentlyHasNoChildPages = Value;
                    break;

                case "This Aggregation Currently Has No Subcollections":
                    ThisAggregationCurrentlyHasNoSubcollections = Value;
                    break;

                case "THIS BANNER IMAGE IS MISSING":
                    THISBANNERIMAGEISMISSING = Value;
                    break;

                case "This Page Allows You To Edit The Basic Information About A Single Child Page And Add The Ability To Display This Child Page In Alternate Languages":
                    ThisPageAllowsYouToEditTheBasicInformationAboutASingleChildPageAndAddTheAbilityToDisplayThisChildPageInAlternateLanguages = Value;
                    break;

                case "This Section Controls All The Languagespecific And Default Text Which Appears On The Home Page":
                    ThisSectionControlsAllTheLanguagespecificAndDefaultTextWhichAppearsOnTheHomePage = Value;
                    break;

                case "This Section Shows All The Existing Languagespecific Banners For This Aggregation And Allows You Upload New Banners For This Aggregation":
                    ThisSectionShowsAllTheExistingLanguagespecificBannersForThisAggregationAndAllowsYouUploadNewBannersForThisAggregation = Value;
                    break;

                case "Thumbnail View":
                    ThumbnailView = Value;
                    break;

                case "TITLE":
                    TITLE = Value;
                    break;

                case "Title Default":
                    TitleDefault = Value;
                    break;

                case "To Change Browse To A 50X50 Pixel GIF File And Then Select UPLOAD":
                    ToChangeBrowseToA50X50PixelGIFFileAndThenSelectUPLOAD = Value;
                    break;

                case "To Edit This Log On As The Aggregation Admin And Hover Over This Text To Edit It":
                    ToEditThisLogOnAsTheAggregationAdminAndHoverOverThisTextToEditIt = Value;
                    break;

                case "To Upload Browse To A GIF PNG JPEG Or BMP File And Then Select UPLOAD":
                    ToUploadBrowseToAGIFPNGJPEGOrBMPFileAndThenSelectUPLOAD = Value;
                    break;

                case "Tooltip":
                    Tooltip = Value;
                    break;

                case "TYPE":
                    TYPE = Value;
                    break;

                case "Unable To Remove Subcollection Directory":
                    UnableToRemoveSubcollectionDirectory = Value;
                    break;

                case "United States":
                    UnitedStates = Value;
                    break;

                case "Upload New Banner Image":
                    UploadNewBannerImage = Value;
                    break;

                case "View":
                    View = Value;
                    break;

                case "View Banner Image File":
                    ViewBannerImageFile = Value;
                    break;

                case "View CSS File":
                    ViewCSSFile = Value;
                    break;

                case "View Source File":
                    ViewSourceFile = Value;
                    break;

                case "View This Child Page":
                    ViewThisChildPage = Value;
                    break;

                case "View This Child Page In XXX":
                    ViewThisChildPageInXXX = Value;
                    break;

                case "View This Home Page":
                    ViewThisHomePage = Value;
                    break;

                case "View This Home Page In XXX":
                    ViewThisHomePageInXXX = Value;
                    break;

                case "View This Subcollection":
                    ViewThisSubcollection = Value;
                    break;

                case "VISIBILITY":
                    VISIBILITY = Value;
                    break;

                case "Visibility":
                    Visibility = Value;
                    break;

                case "Web Skin":
                    WebSkin = Value;
                    break;

                case "World":
                    World = Value;
                    break;

                case "You Can Edit The Contents Of The Aggregationlevel Custom Stylesheet Css File Here Click SAVE When Complete To Return To The Main Aggregation Administration Screen":
                    YouCanEditTheContentsOfTheAggregationlevelCustomStylesheetCssFileHereClickSAVEWhenCompleteToReturnToTheMainAggregationAdministrationScreen = Value;
                    break;

                case "You Can Use OAIPMH To Expose All The Metadata Of The Resources Within This Aggregation For Automatic Harvesting By Other Repositories Additionally You Can Choose To Attach Metadata To The Collectionlevel OAIPMH Record This Should Be Coded As Dublin Core Tags":
                    YouCanUseOAIPMHToExposeAllTheMetadataOfTheResourcesWithinThisAggregationForAutomaticHarvestingByOtherRepositoriesAdditionallyYouCanChooseToAttachMetadataToTheCollectionlevelOAIPMHRecordThisShouldBeCodedAsDublinCoreTags = Value;
                    break;

                case "You Can View Existing Subcollections Or Add New Subcollections To This Aggregation From This Tab You Will Have Full Curatorial Rights Over Any New Subcollections You Add Currently Only System Administrators Can DELETE Subcollections":
                    YouCanViewExistingSubcollectionsOrAddNewSubcollectionsToThisAggregationFromThisTabYouWillHaveFullCuratorialRightsOverAnyNewSubcollectionsYouAddCurrentlyOnlySystemAdministratorsCanDELETESubcollections = Value;
                    break;

                case "You Must Enter A CODE For This Child Page":
                    YouMustEnterACODEForThisChildPage = Value;
                    break;

                case "You Must Enter A CODE For This Item Aggregation":
                    YouMustEnterACODEForThisItemAggregation = Value;
                    break;

                case "You Must Enter A DESCRIPTION For This New Aggregation":
                    YouMustEnterADESCRIPTIONForThisNewAggregation = Value;
                    break;

                case "You Must Enter A LABEL For This Child Page":
                    YouMustEnterALABELForThisChildPage = Value;
                    break;

                case "You Must Enter A NAME For This New Aggregation":
                    YouMustEnterANAMEForThisNewAggregation = Value;
                    break;

                case "You Must SAVE Your Changes Before You Can View Or Edit Newly Added Child Page Versions":
                    YouMustSAVEYourChangesBeforeYouCanViewOrEditNewlyAddedChildPageVersions = Value;
                    break;

                case "You Must SAVE Your Changes Before You Can View Or Edit Newly Added Home Pages":
                    YouMustSAVEYourChangesBeforeYouCanViewOrEditNewlyAddedHomePages = Value;
                    break;

                case "You Must Select A TYPE For This New Aggregation":
                    YouMustSelectATYPEForThisNewAggregation = Value;
                    break;

                case "You Must Select A VISIBILITY For This Child Page":
                    YouMustSelectAVISIBILITYForThisChildPage = Value;
                    break;

            }
        }
        /// <remarks> '( NO DEFAULT)' localization string </remarks>
        public string NODEFAULT { get; private set; }

        /// <remarks> '(none - top level)' localization string </remarks>
        public string NoneTopLevel { get; private set; }

        /// <remarks> 'ACTION' localization string </remarks>
        public string ACTION { get; private set; }

        /// <remarks> Table headers </remarks>
        public string ACTIONS { get; private set; }

        /// <remarks> Checkbox label for : Is aggregation admin? </remarks>
        public string Active { get; private set; }

        /// <remarks> 'Add' localization string </remarks>
        public string Add { get; private set; }

        /// <remarks> 'Add new banner' localization string </remarks>
        public string AddNewBanner { get; private set; }

        /// <remarks> 'Add new home page' localization string </remarks>
        public string AddNewHomePage { get; private set; }

        /// <remarks> 'Additional dublin core metadata to include in OAI-PMH set list:' localization string </remarks>
        public string AdditionalDublinCoreMetadataToIncludeInOAIPMHSetList { get; private set; }

        /// <remarks> 'Advanced Search' localization string </remarks>
        public string AdvancedSearch { get; private set; }

        /// <remarks> 'Advanced Search (with Year Range)' localization string </remarks>
        public string AdvancedSearchWithYearRange { get; private set; }

        /// <remarks> One of the geographic search location options for map searching </remarks>
        public string Africa { get; private set; }

        /// <remarks> 'Aggregation-level Custom Stylesheet (CSS)' localization string </remarks>
        public string AggregationlevelCustomStylesheetCSS { get; private set; }

        /// <remarks> Tab header </remarks>
        public string Appearance { get; private set; }

        /// <remarks> 'Appearance Options' localization string </remarks>
        public string AppearanceOptions { get; private set; }

        /// <remarks> One of the geographic search location options for map searching </remarks>
        public string Asia { get; private set; }

        /// <remarks> Button text </remarks>
        public string BACK { get; private set; }

        /// <remarks> 'Banner Type:' localization string </remarks>
        public string BannerType { get; private set; }

        /// <remarks> 'Banners' localization string </remarks>
        public string Banners { get; private set; }

        /// <remarks> Section Heading for collecting basic information about the user </remarks>
        public string BasicInformation { get; private set; }

        /// <remarks> 'Basic Search (with Year Range)' localization string </remarks>
        public string BasicSearchWithYearRange { get; private set; }

        /// <remarks> 'Behavior:' localization string </remarks>
        public string Behavior { get; private set; }

        /// <remarks> 'Brief View' localization string </remarks>
        public string BriefView { get; private set; }

        /// <remarks> 'Browse By' localization string </remarks>
        public string BrowseBy { get; private set; }

        /// <remarks> Button text </remarks>
        public string CANCEL { get; private set; }

        /// <remarks> One of the geographic search location options for map searching </remarks>
        public string Caribbean { get; private set; }

        /// <remarks> Tab header </remarks>
        public string ChildPages { get; private set; }

        /// <remarks> '"Child pages are pages related to the aggregation and allow additional information to be presented within the same aggregational branding.  These can appear in the aggregation main menu, with any metadata browses pulled from the database, or you can set them to for no automatic visibility, in which case they are only accessible by links in the home page or other child pages."' localization string </remarks>
        public string ChildPagesArePagesRelatedToTheAggregationAndAllowAdditionalInformationToBePresentedWithinTheSameAggregationalBrandingTheseCanAppearInTheAggregationMainMenuWithAnyMetadataBrowsesPulledFromTheDatabaseOrYouCanSetThemToForNoAutomaticVisibilityInWhichCaseTheyAreOnlyAccessibleByLinksInTheHomePageOrOtherChildPages { get; private set; }

        /// <remarks> 'click here to view the help page' localization string </remarks>
        public string ClickHereToViewTheHelpPage { get; private set; }

        /// <remarks> 'Click to delete this subcollection' localization string </remarks>
        public string ClickToDeleteThisSubcollection { get; private set; }

        /// <remarks> 'Close this child page details and return to main admin pages' localization string </remarks>
        public string CloseThisChildPageDetailsAndReturnToMainAdminPages { get; private set; }

        /// <remarks> 'CODE' localization string </remarks>
        public string CODE { get; private set; }

        /// <remarks> 'Collection' localization string </remarks>
        public string Collection { get; private set; }

        /// <remarks> 'Collection Button:' localization string </remarks>
        public string CollectionButton { get; private set; }

        /// <remarks> 'Collection Visibility' localization string </remarks>
        public string CollectionVisibility { get; private set; }

        /// <remarks> 'Contact Email:' localization string </remarks>
        public string ContactEmail { get; private set; }

        /// <remarks> 'Copy from existing' localization string </remarks>
        public string CopyFromExisting { get; private set; }

        /// <remarks> 'Copy from existing home:' localization string </remarks>
        public string CopyFromExistingHome { get; private set; }

        /// <remarks> 'Custom Home Page:' localization string </remarks>
        public string CustomHomePage { get; private set; }

        /// <remarks> 'Custom Stylesheet:' localization string </remarks>
        public string CustomStylesheet { get; private set; }

        /// <remarks> 'default' localization string </remarks>
        public string Default { get; private set; }

        /// <remarks> 'Default Browse:' localization string </remarks>
        public string DefaultBrowse { get; private set; }

        /// <remarks> 'Default Result View:' localization string </remarks>
        public string DefaultResultView { get; private set; }

        /// <remarks> Button text </remarks>
        public string Delete { get; private set; }

        /// <remarks> 'Delete this %1 version' localization string </remarks>
        public string DeleteThisXXXVersion { get; private set; }

        /// <remarks> 'Delete this banner' localization string </remarks>
        public string DeleteThisBanner { get; private set; }

        /// <remarks> 'DELETE THIS HIGHLIGHT' localization string </remarks>
        public string DELETETHISHIGHLIGHT { get; private set; }

        /// <remarks> 'Deleted %1 subcollection' localization string </remarks>
        public string DeletedXXXSubcollection { get; private set; }

        /// <remarks> 'Description:' localization string </remarks>
        public string Description { get; private set; }

        /// <remarks> Button hover-over text </remarks>
        public string DisableThisAggregationlevelStylesheet { get; private set; }

        /// <remarks> 'dLOC Full Text Search' localization string </remarks>
        public string DlocFullTextSearch { get; private set; }

        /// <remarks> Message explaining cancel button on hover </remarks>
        public string DoNotApplyChanges { get; private set; }

        /// <remarks> Button text </remarks>
        public string Edit { get; private set; }

        /// <remarks> 'Edit Child Page Detail' localization string </remarks>
        public string EditChildPageDetail { get; private set; }

        /// <remarks> Button hover-over text </remarks>
        public string EditThisAggregationlevelStylesheet { get; private set; }

        /// <remarks> 'Edit this child page' localization string </remarks>
        public string EditThisChildPage { get; private set; }

        /// <remarks> 'Edit this child page in %1' localization string </remarks>
        public string EditThisChildPageInXXX { get; private set; }

        /// <remarks> 'Edit this home page' localization string </remarks>
        public string EditThisHomePage { get; private set; }

        /// <remarks> 'Edit this home page in %1' localization string </remarks>
        public string EditThisHomePageInXXX { get; private set; }

        /// <remarks> 'Edit this subcollection' localization string </remarks>
        public string EditThisSubcollection { get; private set; }

        /// <remarks> Button text to enable the new style-sheet </remarks>
        public string ENABLE { get; private set; }

        /// <remarks> 'Enable an aggregation-level stylesheet' localization string </remarks>
        public string EnableAnAggregationlevelStylesheet { get; private set; }

        /// <remarks> Label for checkbox indicating if the flag is set </remarks>
        public string EnabledFlag { get; private set; }

        /// <remarks> 'Error adding new version to this child page' localization string </remarks>
        public string ErrorAddingNewVersionToThisChildPage { get; private set; }

        /// <remarks> Error message displayed on the screen on encountering a save error </remarks>
        public string ErrorSavingAggregationInformation { get; private set; }

        /// <remarks> 'ERROR saving the new item aggregation to the database' localization string </remarks>
        public string ERRORSavingTheNewItemAggregationToTheDatabase { get; private set; }

        /// <remarks> 'ERROR: Invalid entry for new item aggregation' localization string </remarks>
        public string ERRORInvalidEntryForNewItemAggregation { get; private set; }

        /// <remarks> 'ERROR: Invalid entry for new item child page' localization string </remarks>
        public string ERRORInvalidEntryForNewItemChildPage { get; private set; }

        /// <remarks> One of the geographic search location options for map searching </remarks>
        public string Europe { get; private set; }

        /// <remarks> 'Exhibit' localization string </remarks>
        public string Exhibit { get; private set; }

        /// <remarks> 'Existing Banners:' localization string </remarks>
        public string ExistingBanners { get; private set; }

        /// <remarks> 'Existing Child Pages:' localization string </remarks>
        public string ExistingChildPages { get; private set; }

        /// <remarks> 'Existing Home Pages:' localization string </remarks>
        public string ExistingHomePages { get; private set; }

        /// <remarks> 'Existing Subcollections:' localization string </remarks>
        public string ExistingSubcollections { get; private set; }

        /// <remarks> 'Existing Versions:' localization string </remarks>
        public string ExistingVersions { get; private set; }

        /// <remarks> 'External Link:' localization string </remarks>
        public string ExternalLink { get; private set; }

        /// <remarks> One of the geographic search location options for map searching </remarks>
        public string Florida { get; private set; }

        /// <remarks> '"For more information about the settings on this tab,"' localization string </remarks>
        public string ForMoreInformationAboutTheSettingsOnThisTab { get; private set; }

        /// <remarks> 'Full Text Search' localization string </remarks>
        public string FullTextSearch { get; private set; }

        /// <remarks> 'Full View' localization string </remarks>
        public string FullView { get; private set; }

        /// <remarks> Tab header </remarks>
        public string General { get; private set; }

        /// <remarks> Tab header </remarks>
        public string Highlights { get; private set; }

        /// <remarks> 'Highlights Type:' localization string </remarks>
        public string HighlightsType { get; private set; }

        /// <remarks> 'Home Page' localization string </remarks>
        public string HomePage { get; private set; }

        /// <remarks> 'Home Page - Left' localization string </remarks>
        public string HomePageLeft { get; private set; }

        /// <remarks> 'Home Page - Right' localization string </remarks>
        public string HomePageRight { get; private set; }

        /// <remarks> 'Home Page Text' localization string </remarks>
        public string HomePageText { get; private set; }

        /// <remarks> 'Image' localization string </remarks>
        public string Image { get; private set; }

        /// <remarks> 'Include All / New Item Browses' localization string </remarks>
        public string IncludeAllNewItemBrowses { get; private set; }

        /// <remarks> 'Include in OAI-PMH as a set?' localization string </remarks>
        public string IncludeInOAIPMHAsASet { get; private set; }

        /// <remarks> 'Include Map Browse' localization string </remarks>
        public string IncludeMapBrowse { get; private set; }

        /// <remarks> 'Institutional Division' localization string </remarks>
        public string InstitutionalDivision { get; private set; }

        /// <remarks> 'LANGUAGE' localization string </remarks>
        public string LANGUAGE { get; private set; }

        /// <remarks> 'LANGUAGE(S)' localization string </remarks>
        public string LANGUAGES { get; private set; }

        /// <remarks> 'Language:' localization string </remarks>
        public string Language { get; private set; }

        /// <remarks> 'Link' localization string </remarks>
        public string Link { get; private set; }

        /// <remarks> 'Main Menu' localization string </remarks>
        public string MainMenu { get; private set; }

        /// <remarks> 'Map Search (Beta)' localization string </remarks>
        public string MapSearchBeta { get; private set; }

        /// <remarks> 'Map Search (Legacy)' localization string </remarks>
        public string MapSearchLegacy { get; private set; }

        /// <remarks> 'Map Search Default Area:' localization string </remarks>
        public string MapSearchDefaultArea { get; private set; }

        /// <remarks> Tab header </remarks>
        public string Metadata { get; private set; }

        /// <remarks> 'Metadata Browses' localization string </remarks>
        public string MetadataBrowses { get; private set; }

        /// <remarks> One of the geographic search location options for map searching </remarks>
        public string MiddleEast { get; private set; }

        /// <remarks> 'Name (full)' localization string </remarks>
        public string NameFull { get; private set; }

        /// <remarks> 'Name (short):' localization string </remarks>
        public string NameShort { get; private set; }

        /// <remarks> 'New aggregation code must be twenty characters long or less' localization string </remarks>
        public string NewAggregationCodeMustBeTwentyCharactersLongOrLess { get; private set; }

        /// <remarks> 'New Banner:' localization string </remarks>
        public string NewBanner { get; private set; }

        /// <remarks> Error message </remarks>
        public string NewChildPageCodeMustBeTwentyCharactersLongOrLess { get; private set; }

        /// <remarks> 'New Child Page:' localization string </remarks>
        public string NewChildPage { get; private set; }

        /// <remarks> 'New code must be unique... %1 already exists' localization string </remarks>
        public string NewCodeMustBeUniqueXXXAlreadyExists { get; private set; }

        /// <remarks> 'New collection home page text goes here.' localization string </remarks>
        public string NewCollectionHomePageTextGoesHere { get; private set; }

        /// <remarks> 'New home page text in %1 goes here' localization string </remarks>
        public string NewHomePageTextInXXXGoesHere { get; private set; }

        /// <remarks> 'New Home Page:' localization string </remarks>
        public string NewHomePage { get; private set; }

        /// <remarks> 'New item aggregation %1 saved successfully' localization string </remarks>
        public string NewItemAggregationXXXSavedSuccessfully { get; private set; }

        /// <remarks> 'New Subcollection:' localization string </remarks>
        public string NewSubcollection { get; private set; }

        /// <remarks> 'No custom aggregation-level stylesheet' localization string </remarks>
        public string NoCustomAggregationlevelStylesheet { get; private set; }

        /// <remarks> 'No html source files' localization string </remarks>
        public string NoHtmlSourceFiles { get; private set; }

        /// <remarks> Alt text for No image available </remarks>
        public string NONE { get; private set; }

        /// <remarks> One of the geographic search location options for map searching </remarks>
        public string NorthAmerica { get; private set; }

        /// <remarks> 'NOTE: You may need to refresh your browser for your changes to take affect.' localization string </remarks>
        public string NOTEYouMayNeedToRefreshYourBrowserForYourChangesToTakeAffect { get; private set; }

        /// <remarks> 'OAI-PMH Settings' localization string </remarks>
        public string OAIPMHSettings { get; private set; }

        /// <remarks> 'Other Display Types:' localization string </remarks>
        public string OtherDisplayTypes { get; private set; }

        /// <remarks> 'PARENT' localization string </remarks>
        public string PARENT { get; private set; }

        /// <remarks> 'Parent Code(s)' localization string </remarks>
        public string ParentCodes { get; private set; }

        /// <remarks> 'Parent:' localization string </remarks>
        public string Parent { get; private set; }

        /// <remarks> 'Result Views:' localization string </remarks>
        public string ResultViews { get; private set; }

        /// <remarks> Tab header </remarks>
        public string Results { get; private set; }

        /// <remarks> Facets header </remarks>
        public string ResultsOptions { get; private set; }

        /// <remarks> 'Rotating' localization string </remarks>
        public string Rotating { get; private set; }

        /// <remarks> Button text </remarks>
        public string SAVE { get; private set; }

        /// <remarks> 'Save changes to this item Aggregation' localization string </remarks>
        public string SaveChangesToThisItemAggregation { get; private set; }

        /// <remarks> 'Save new child page' localization string </remarks>
        public string SaveNewChildPage { get; private set; }

        /// <remarks> 'Save new version of this child page' localization string </remarks>
        public string SaveNewVersionOfThisChildPage { get; private set; }

        /// <remarks> Tab header </remarks>
        public string Search { get; private set; }

        /// <remarks> 'Search Options' localization string </remarks>
        public string SearchOptions { get; private set; }

        /// <remarks> Heading label text </remarks>
        public string SearchTypes { get; private set; }

        /// <remarks> Text displayed when hovering over a selected image </remarks>
        public string SelectedImageFile { get; private set; }

        /// <remarks> 'Show in parent collection home page?' localization string </remarks>
        public string ShowInParentCollectionHomePage { get; private set; }

        /// <remarks> 'Some of the files may be in use' localization string </remarks>
        public string SomeOfTheFilesMayBeInUse { get; private set; }

        /// <remarks> Table headers </remarks>
        public string SOURCEFILE { get; private set; }

        /// <remarks> One of the geographic search location options for map searching </remarks>
        public string SouthAmerica { get; private set; }

        /// <remarks> Title for Standard aggregations table </remarks>
        public string Standard { get; private set; }

        /// <remarks> 'Static' localization string </remarks>
        public string Static { get; private set; }

        /// <remarks> Tab header </remarks>
        public string Subcollections { get; private set; }

        /// <remarks> 'Table View' localization string </remarks>
        public string TableView { get; private set; }

        /// <remarks> 'Text:' localization string </remarks>
        public string Text { get; private set; }

        /// <remarks> 'That code is a system-reserved keyword.  Try a different code.' localization string </remarks>
        public string ThatCodeIsASystemreservedKeywordTryADifferentCode { get; private set; }

        /// <remarks> 'The code for this browse is: %1' localization string </remarks>
        public string TheCodeForThisBrowseIsXXX { get; private set; }

        /// <remarks> '"The information in this section controls how search results or item browses appears.  The facet options control which metadata values appear to the left of the results, to allow users to narrow their results.  The search results values determine which options are available for viewing the results and what are the aggregation defaults.  Finally, the result fields determines which values are displayed with each individual result in the result set."' localization string </remarks>
        public string TheInformationInThisSectionControlsHowSearchResultsOrItemBrowsesAppearsTheFacetOptionsControlWhichMetadataValuesAppearToTheLeftOfTheResultsToAllowUsersToNarrowTheirResultsTheSearchResultsValuesDetermineWhichOptionsAreAvailableForViewingTheResultsAndWhatAreTheAggregationDefaultsFinallyTheResultFieldsDeterminesWhichValuesAreDisplayedWithEachIndividualResultInTheResultSet { get; private set; }

        /// <remarks> '"The information in this section is the basic information about the aggregation, such as the full name, the shortened name used for breadcrumbs, the description, and the email contact. "' localization string </remarks>
        public string TheInformationInThisSectionIsTheBasicInformationAboutTheAggregationSuchAsTheFullNameTheShortenedNameUsedForBreadcrumbsTheDescriptionAndTheEmailContact { get; private set; }

        /// <remarks> 'The metadata browses can be used to expose all the metadata of the resources within this aggregation for public browsing.  Select the metadata fields you would like have available below.' localization string </remarks>
        public string TheMetadataBrowsesCanBeUsedToExposeAllTheMetadataOfTheResourcesWithinThisAggregationForPublicBrowsingSelectTheMetadataFieldsYouWouldLikeHaveAvailableBelow { get; private set; }

        /// <remarks> '"The values in this section determine if the collection is currently visible at all, whether it is eligible to appear on the collection list at the bottom of the parent page, and the collection button used in that case.  Thematic headings are used to place this collection on the main home page."' localization string </remarks>
        public string TheValuesInThisSectionDetermineIfTheCollectionIsCurrentlyVisibleAtAllWhetherItIsEligibleToAppearOnTheCollectionListAtTheBottomOfTheParentPageAndTheCollectionButtonUsedInThatCaseThematicHeadingsAreUsedToPlaceThisCollectionOnTheMainHomePage { get; private set; }

        /// <remarks> Heading label text </remarks>
        public string ThematicHeading { get; private set; }

        /// <remarks> '"These options control how searching works within this aggregation, such as which search options are made publicly available."' localization string </remarks>
        public string TheseOptionsControlHowSearchingWorksWithinThisAggregationSuchAsWhichSearchOptionsAreMadePubliclyAvailable { get; private set; }

        /// <remarks> '"These three settings have the most profound affects on the appearance of this aggregation, by forcing it to appear under a particular web skin, allowing a custom aggregation-level stylesheet, or completely overriding the system-generated home page for a custom home page HTML source file."' localization string </remarks>
        public string TheseThreeSettingsHaveTheMostProfoundAffectsOnTheAppearanceOfThisAggregationByForcingItToAppearUnderAParticularWebSkinAllowingACustomAggregationlevelStylesheetOrCompletelyOverridingTheSystemgeneratedHomePageForACustomHomePageHTMLSourceFile { get; private set; }

        /// <remarks> 'This aggregation currently has no child pages' localization string </remarks>
        public string ThisAggregationCurrentlyHasNoChildPages { get; private set; }

        /// <remarks> 'This aggregation currently has no subcollections' localization string </remarks>
        public string ThisAggregationCurrentlyHasNoSubcollections { get; private set; }

        /// <remarks> Alt text for a missing image </remarks>
        public string THISBANNERIMAGEISMISSING { get; private set; }

        /// <remarks> 'This page allows you to edit the basic information about a single child page and add the ability to display this child page in alternate languages.' localization string </remarks>
        public string ThisPageAllowsYouToEditTheBasicInformationAboutASingleChildPageAndAddTheAbilityToDisplayThisChildPageInAlternateLanguages { get; private set; }

        /// <remarks> 'This section controls all the language-specific (and default) text which appears on the home page.' localization string </remarks>
        public string ThisSectionControlsAllTheLanguagespecificAndDefaultTextWhichAppearsOnTheHomePage { get; private set; }

        /// <remarks> 'This section shows all the existing language-specific banners for this aggregation and allows you upload new banners for this aggregation.' localization string </remarks>
        public string ThisSectionShowsAllTheExistingLanguagespecificBannersForThisAggregationAndAllowsYouUploadNewBannersForThisAggregation { get; private set; }

        /// <remarks> 'Thumbnail View' localization string </remarks>
        public string ThumbnailView { get; private set; }

        /// <remarks> 'TITLE' localization string </remarks>
        public string TITLE { get; private set; }

        /// <remarks> 'Title (default):' localization string </remarks>
        public string TitleDefault { get; private set; }

        /// <remarks> Upload instructions </remarks>
        public string ToChangeBrowseToA50X50PixelGIFFileAndThenSelectUPLOAD { get; private set; }

        /// <remarks> '"To edit this, log on as the aggregation admin and hover over this text to edit it."' localization string </remarks>
        public string ToEditThisLogOnAsTheAggregationAdminAndHoverOverThisTextToEditIt { get; private set; }

        /// <remarks> '"To upload, browse to a GIF, PNG, JPEG, or BMP file, and then select UPLOAD"' localization string </remarks>
        public string ToUploadBrowseToAGIFPNGJPEGOrBMPFileAndThenSelectUPLOAD { get; private set; }

        /// <remarks> 'Tooltip:' localization string </remarks>
        public string Tooltip { get; private set; }

        /// <remarks> 'TYPE' localization string </remarks>
        public string TYPE { get; private set; }

        /// <remarks> 'Unable to remove subcollection directory' localization string </remarks>
        public string UnableToRemoveSubcollectionDirectory { get; private set; }

        /// <remarks> One of the geographic search location options for map searching </remarks>
        public string UnitedStates { get; private set; }

        /// <remarks> 'Upload New Banner Image:' localization string </remarks>
        public string UploadNewBannerImage { get; private set; }

        /// <remarks> Button text </remarks>
        public string View { get; private set; }

        /// <remarks> 'View banner image file' localization string </remarks>
        public string ViewBannerImageFile { get; private set; }

        /// <remarks> Button hover-over text </remarks>
        public string ViewCSSFile { get; private set; }

        /// <remarks> 'View source file' localization string </remarks>
        public string ViewSourceFile { get; private set; }

        /// <remarks> 'View this child page' localization string </remarks>
        public string ViewThisChildPage { get; private set; }

        /// <remarks> 'View this child page in %1' localization string </remarks>
        public string ViewThisChildPageInXXX { get; private set; }

        /// <remarks> 'View this home page' localization string </remarks>
        public string ViewThisHomePage { get; private set; }

        /// <remarks> 'View this home page in %1' localization string </remarks>
        public string ViewThisHomePageInXXX { get; private set; }

        /// <remarks> 'View this subcollection' localization string </remarks>
        public string ViewThisSubcollection { get; private set; }

        /// <remarks> 'VISIBILITY' localization string </remarks>
        public string VISIBILITY { get; private set; }

        /// <remarks> 'Visibility:' localization string </remarks>
        public string Visibility { get; private set; }

        /// <remarks> 'Web Skin:' localization string </remarks>
        public string WebSkin { get; private set; }

        /// <remarks> One of the geographic search location options for map searching </remarks>
        public string World { get; private set; }

        /// <remarks> 'You can edit the contents of the aggregation-level custom stylesheet (css) file here.  Click SAVE when complete to return to the main aggregation administration screen.' localization string </remarks>
        public string YouCanEditTheContentsOfTheAggregationlevelCustomStylesheetCssFileHereClickSAVEWhenCompleteToReturnToTheMainAggregationAdministrationScreen { get; private set; }

        /// <remarks> '"You can use OAI-PMH to expose all the metadata of the resources within this aggregation for automatic harvesting by other repositories.  Additionally, you can choose to attach metadata to the collection-level OAI-PMH record.  This should be coded as dublin core tags."' localization string </remarks>
        public string YouCanUseOAIPMHToExposeAllTheMetadataOfTheResourcesWithinThisAggregationForAutomaticHarvestingByOtherRepositoriesAdditionallyYouCanChooseToAttachMetadataToTheCollectionlevelOAIPMHRecordThisShouldBeCodedAsDublinCoreTags { get; private set; }

        /// <remarks> '"You can view existing subcollections or add new subcollections to this aggregation from this tab.  You will have full curatorial rights over any new subcollections you add.  Currently, only system administrators can DELETE subcollections."' localization string </remarks>
        public string YouCanViewExistingSubcollectionsOrAddNewSubcollectionsToThisAggregationFromThisTabYouWillHaveFullCuratorialRightsOverAnyNewSubcollectionsYouAddCurrentlyOnlySystemAdministratorsCanDELETESubcollections { get; private set; }

        /// <remarks> 'You must enter a CODE for this child page' localization string </remarks>
        public string YouMustEnterACODEForThisChildPage { get; private set; }

        /// <remarks> 'You must enter a CODE for this item aggregation' localization string </remarks>
        public string YouMustEnterACODEForThisItemAggregation { get; private set; }

        /// <remarks> 'You must enter a DESCRIPTION for this new aggregation' localization string </remarks>
        public string YouMustEnterADESCRIPTIONForThisNewAggregation { get; private set; }

        /// <remarks> 'You must enter a LABEL for this child page' localization string </remarks>
        public string YouMustEnterALABELForThisChildPage { get; private set; }

        /// <remarks> 'You must enter a NAME for this new aggregation' localization string </remarks>
        public string YouMustEnterANAMEForThisNewAggregation { get; private set; }

        /// <remarks> 'You must SAVE your changes before you can view or edit newly added child page versions' localization string </remarks>
        public string YouMustSAVEYourChangesBeforeYouCanViewOrEditNewlyAddedChildPageVersions { get; private set; }

        /// <remarks> 'You must SAVE your changes before you can view or edit newly added home pages.' localization string </remarks>
        public string YouMustSAVEYourChangesBeforeYouCanViewOrEditNewlyAddedHomePages { get; private set; }

        /// <remarks> 'You must select a TYPE for this new aggregation' localization string </remarks>
        public string YouMustSelectATYPEForThisNewAggregation { get; private set; }

        /// <remarks> 'You must select a VISIBILITY for this child page' localization string </remarks>
        public string YouMustSelectAVISIBILITYForThisChildPage { get; private set; }

    }
}
