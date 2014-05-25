namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Internal_HtmlSubwriter class </summary>
    public class Internal_HtmlSubwriter_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Internal_HtmlSubwriter_Localization class </summary>
        public Internal_HtmlSubwriter_LocalizationInfo() : base()
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
                case "None":
                    None = Value;
                    break;

                case "Active Aggregations":
                    ActiveAggregations = Value;
                    break;

                case "Active And Inactive Collections":
                    ActiveAndInactiveCollections = Value;
                    break;

                case "ALL":
                    ALL = Value;
                    break;

                case "All Aggregation Types":
                    AllAggregationTypes = Value;
                    break;

                case "APPLICATION STATE VALUES":
                    APPLICATIONSTATEVALUES = Value;
                    break;

                case "Below Is The Complete Master List Of All Aggregations Within This Library This Includes All Active Aggregations As Well As All Hidden Or Inactive Collections":
                    BelowIsTheCompleteMasterListOfAllAggregationsWithinThisLibraryThisIncludesAllActiveAggregationsAsWellAsAllHiddenOrInactiveCollections = Value;
                    break;

                case "Build Failure Logs":
                    BuildFailureLogs = Value;
                    break;

                case "Build Failures":
                    BuildFailures = Value;
                    break;

                case "Build Failures And Warnings":
                    BuildFailuresAndWarnings = Value;
                    break;

                case "BULK LOADED":
                    BULKLOADED = Value;
                    break;

                case "Child Aggregations":
                    ChildAggregations = Value;
                    break;

                case "Click Here To Return To The Digital Library Home Page":
                    ClickHereToReturnToTheDigitalLibraryHomePage = Value;
                    break;

                case "Click Here To Sort By CODE":
                    ClickHereToSortByCODE = Value;
                    break;

                case "Click Here To Sort By DATE ADDED":
                    ClickHereToSortByDATEADDED = Value;
                    break;

                case "Collection Hierarchy":
                    CollectionHierarchy = Value;
                    break;

                case "Current Memory Profile":
                    CurrentMemoryProfile = Value;
                    break;

                case "Date Added":
                    DateAdded = Value;
                    break;

                case "Description":
                    Description = Value;
                    break;

                case "From":
                    From = Value;
                    break;

                case "GLOBAL VALUES":
                    GLOBALVALUES = Value;
                    break;

                case "Inactive Aggregations":
                    InactiveAggregations = Value;
                    break;

                case "INSTANCE NAME":
                    INSTANCENAME = Value;
                    break;

                case "Internal Users Only":
                    InternalUsersOnly = Value;
                    break;

                case "KEY":
                    KEY = Value;
                    break;

                case "LOCALLY CACHED OBJECTS":
                    LOCALLYCACHEDOBJECTS = Value;
                    break;

                case "Memory Management":
                    MemoryManagement = Value;
                    break;

                case "METS Type":
                    METSType = Value;
                    break;

                case "METS TYPE":
                    METSTYPE = Value;
                    break;

                case "Name":
                    Name = Value;
                    break;

                case "New Items":
                    NewItems = Value;
                    break;

                case "Newly Added Or Modified Items":
                    NewlyAddedOrModifiedItems = Value;
                    break;

                case "NO INFORMATION FOR YOUR SELECTION":
                    NOINFORMATIONFORYOURSELECTION = Value;
                    break;

                case "NO NEW ITEMS":
                    NONEWITEMS = Value;
                    break;

                case "No Uncleared Warnings Or Failures For The Selected Date Range":
                    NoUnclearedWarningsOrFailuresForTheSelectedDateRange = Value;
                    break;

                case "OBJECT":
                    OBJECT = Value;
                    break;

                case "ONLINE EDITS":
                    ONLINEEDITS = Value;
                    break;

                case "ONLINE SUBMITS":
                    ONLINESUBMITS = Value;
                    break;

                case "Parent Aggregations":
                    ParentAggregations = Value;
                    break;

                case "POSTPROCESSED":
                    POSTPROCESSED = Value;
                    break;

                case "REMOTELY CACHED OBJECTS":
                    REMOTELYCACHEDOBJECTS = Value;
                    break;

                case "Select One Of The Aggregation Types Below To View Information About All Aggregations Of That Type":
                    SelectOneOfTheAggregationTypesBelowToViewInformationAboutAllAggregationsOfThatType = Value;
                    break;

                case "Select The Update Type Tab Above To View The Details":
                    SelectTheUpdateTypeTabAboveToViewTheDetails = Value;
                    break;

                case "Selected Date Range":
                    SelectedDateRange = Value;
                    break;

                case "SESSION STATE VALUES":
                    SESSIONSTATEVALUES = Value;
                    break;

                case "Sobekcm Code":
                    SobekcmCode = Value;
                    break;

                case "The Data Below Shows Errors Which Occurred While Loading New Items Through The Builder These Can Be Displayed By Month And Year Below By Selecting The Start And End Month These Failures Will Continue To Display Until They Are Manually Cleared Or Until The Item Successfully Loads After The Failure Or Warning":
                    TheDataBelowShowsErrorsWhichOccurredWhileLoadingNewItemsThroughTheBuilderTheseCanBeDisplayedByMonthAndYearBelowBySelectingTheStartAndEndMonthTheseFailuresWillContinueToDisplayUntilTheyAreManuallyClearedOrUntilTheItemSuccessfullyLoadsAfterTheFailureOrWarning = Value;
                    break;

                case "The Failures And Warnings Which Were Encountered During Build Are Searchable Below By Month":
                    TheFailuresAndWarningsWhichWereEncounteredDuringBuildAreSearchableBelowByMonth = Value;
                    break;

                case "There Have Been An Unusually Large Number Of Updates Over The Last Week":
                    ThereHaveBeenAnUnusuallyLargeNumberOfUpdatesOverTheLastWeek = Value;
                    break;

                case "To Change The Date Shown Choose Your Dates Above And Hit The GO Button":
                    ToChangeTheDateShownChooseYourDatesAboveAndHitTheGOButton = Value;
                    break;

                case "To":
                    To = Value;
                    break;

                case "Type":
                    Type = Value;
                    break;

                case "UNKNOWN OBJECT TYPE":
                    UNKNOWNOBJECTTYPE = Value;
                    break;

                case "USER":
                    USER = Value;
                    break;

                case "VISIBILITY CHANGES":
                    VISIBILITYCHANGES = Value;
                    break;

                case "Wordmarks":
                    Wordmarks = Value;
                    break;

                case "You Are Not Authorized To Access This View":
                    YouAreNotAuthorizedToAccessThisView = Value;
                    break;

            }
        }
        /// <remarks> Used when no matching value in the cache </remarks>
        public string None { get; private set; }

        /// <remarks> For dividing list between active and inactive aggregations </remarks>
        public string ActiveAggregations { get; private set; }

        /// <remarks> "Internal view, must be logged on as internal user." </remarks>
        public string ActiveAndInactiveCollections { get; private set; }

        /// <remarks> List of all recently added or changed items in system </remarks>
        public string ALL { get; private set; }

        /// <remarks> Currently this view is not linked to? </remarks>
        public string AllAggregationTypes { get; private set; }

        /// <remarks> When displaying the list of items in the local web server cache </remarks>
        public string APPLICATIONSTATEVALUES { get; private set; }

        /// <remarks> For listing all aggregations in the system </remarks>
        public string BelowIsTheCompleteMasterListOfAllAggregationsWithinThisLibraryThisIncludesAllActiveAggregationsAsWellAsAllHiddenOrInactiveCollections { get; private set; }

        /// <remarks> "Internal view, must be logged on as internal user." </remarks>
        public string BuildFailureLogs { get; private set; }

        /// <remarks> "Internal view, must be logged on as internal user." </remarks>
        public string BuildFailures { get; private set; }

        /// <remarks> When listing build failures within the internal view </remarks>
        public string BuildFailuresAndWarnings { get; private set; }

        /// <remarks> List of all recently added or changed items in system </remarks>
        public string BULKLOADED { get; private set; }

        /// <remarks> Header for listing all child aggregations </remarks>
        public string ChildAggregations { get; private set; }

        /// <remarks> 'Click here to return to the digital library home page.' localization string </remarks>
        public string ClickHereToReturnToTheDigitalLibraryHomePage { get; private set; }

        /// <remarks> For listing all aggregations in the system </remarks>
        public string ClickHereToSortByCODE { get; private set; }

        /// <remarks> For listing all aggregations in the system </remarks>
        public string ClickHereToSortByDATEADDED { get; private set; }

        /// <remarks> "Internal view, must be logged on as internal user." </remarks>
        public string CollectionHierarchy { get; private set; }

        /// <remarks> "Internal view, must be logged on as internal user." </remarks>
        public string CurrentMemoryProfile { get; private set; }

        /// <remarks> For listing all aggregations in the system </remarks>
        public string DateAdded { get; private set; }

        /// <remarks> Header for the column describing the failure </remarks>
        public string Description { get; private set; }

        /// <remarks> When listing build failures within the internal view </remarks>
        public string From { get; private set; }

        /// <remarks> When displaying the list of items in the local web server cache </remarks>
        public string GLOBALVALUES { get; private set; }

        /// <remarks> For dividing list between active and inactive aggregations </remarks>
        public string InactiveAggregations { get; private set; }

        /// <remarks> When displaying the list of items in the local web server cache </remarks>
        public string INSTANCENAME { get; private set; }

        /// <remarks> If unauthorized for this view </remarks>
        public string InternalUsersOnly { get; private set; }

        /// <remarks> When displaying the list of items in the local web server cache </remarks>
        public string KEY { get; private set; }

        /// <remarks> When displaying the list of items in the local web server cache </remarks>
        public string LOCALLYCACHEDOBJECTS { get; private set; }

        /// <remarks> "Internal view, must be logged on as internal user." </remarks>
        public string MemoryManagement { get; private set; }

        /// <remarks> "Type of METS file ( i.e., New, Delete, etc..)" </remarks>
        public string METSType { get; private set; }

        /// <remarks> "Type of METS file ( i.e., New, Delete, etc..) - List of all recently added or changed items in system" </remarks>
        public string METSTYPE { get; private set; }

        /// <remarks> For listing all aggregations in the system </remarks>
        public string Name { get; private set; }

        /// <remarks> "Internal view, must be logged on as internal user." </remarks>
        public string NewItems { get; private set; }

        /// <remarks> "Internal view, must be logged on as internal user." </remarks>
        public string NewlyAddedOrModifiedItems { get; private set; }

        /// <remarks> 'NO INFORMATION FOR YOUR SELECTION' localization string </remarks>
        public string NOINFORMATIONFORYOURSELECTION { get; private set; }

        /// <remarks> List of all recently added or changed items in system </remarks>
        public string NONEWITEMS { get; private set; }

        /// <remarks> When listing build failures within the internal view </remarks>
        public string NoUnclearedWarningsOrFailuresForTheSelectedDateRange { get; private set; }

        /// <remarks> When displaying the list of items in the local web server cache </remarks>
        public string OBJECT { get; private set; }

        /// <remarks> List of all recently added or changed items in system </remarks>
        public string ONLINEEDITS { get; private set; }

        /// <remarks> List of all recently added or changed items in system </remarks>
        public string ONLINESUBMITS { get; private set; }

        /// <remarks> Header for listing parents of an aggregation </remarks>
        public string ParentAggregations { get; private set; }

        /// <remarks> List of all recently added or changed items in system </remarks>
        public string POSTPROCESSED { get; private set; }

        /// <remarks> When displaying the list of items in the local web server cache </remarks>
        public string REMOTELYCACHEDOBJECTS { get; private set; }

        /// <remarks> Currently this view is not linked to? </remarks>
        public string SelectOneOfTheAggregationTypesBelowToViewInformationAboutAllAggregationsOfThatType { get; private set; }

        /// <remarks> List of all recently added or changed items in system </remarks>
        public string SelectTheUpdateTypeTabAboveToViewTheDetails { get; private set; }

        /// <remarks> When listing build failures within the internal view </remarks>
        public string SelectedDateRange { get; private set; }

        /// <remarks> When displaying the list of items in the local web server cache </remarks>
        public string SESSIONSTATEVALUES { get; private set; }

        /// <remarks> For listing all aggregations in the system </remarks>
        public string SobekcmCode { get; private set; }

        /// <remarks> When listing build failures within the internal view </remarks>
        public string TheDataBelowShowsErrorsWhichOccurredWhileLoadingNewItemsThroughTheBuilderTheseCanBeDisplayedByMonthAndYearBelowBySelectingTheStartAndEndMonthTheseFailuresWillContinueToDisplayUntilTheyAreManuallyClearedOrUntilTheItemSuccessfullyLoadsAfterTheFailureOrWarning { get; private set; }

        /// <remarks> When listing build failures within the internal view </remarks>
        public string TheFailuresAndWarningsWhichWereEncounteredDuringBuildAreSearchableBelowByMonth { get; private set; }

        /// <remarks> List of all recently added or changed items in system </remarks>
        public string ThereHaveBeenAnUnusuallyLargeNumberOfUpdatesOverTheLastWeek { get; private set; }

        /// <remarks> When listing build failures within the internal view </remarks>
        public string ToChangeTheDateShownChooseYourDatesAboveAndHitTheGOButton { get; private set; }

        /// <remarks> When listing build failures within the internal view </remarks>
        public string To { get; private set; }

        /// <remarks> For listing all aggregations in the system </remarks>
        public string Type { get; private set; }

        /// <remarks> When displaying the list of items in the local web server cache </remarks>
        public string UNKNOWNOBJECTTYPE { get; private set; }

        /// <remarks> List of all recently added or changed items in system </remarks>
        public string USER { get; private set; }

        /// <remarks> List of all recently added or changed items in system </remarks>
        public string VISIBILITYCHANGES { get; private set; }

        /// <remarks> "Internal view, must be logged on as internal user." </remarks>
        public string Wordmarks { get; private set; }

        /// <remarks> "Internal view, must be logged on as internal user." </remarks>
        public string YouAreNotAuthorizedToAccessThisView { get; private set; }

    }
}
