namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Internal_HtmlSubwriter class </summary>
    public class Internal_HtmlSubwriter_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Internal_HtmlSubwriter_Localization class </summary>
        public Internal_HtmlSubwriter_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Internal_HtmlSubwriter";
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
                case "Collection Hierarchy":
                    CollectionHierarchy = Value;
                    break;

                case "Active And Inactive Collections":
                    ActiveAndInactiveCollections = Value;
                    break;

                case "New Items":
                    NewItems = Value;
                    break;

                case "Newly Added Or Modified Items":
                    NewlyAddedOrModifiedItems = Value;
                    break;

                case "Memory Management":
                    MemoryManagement = Value;
                    break;

                case "Current Memory Profile":
                    CurrentMemoryProfile = Value;
                    break;

                case "Wordmarks":
                    Wordmarks = Value;
                    break;

                case "Build Failures":
                    BuildFailures = Value;
                    break;

                case "Build Failure Logs":
                    BuildFailureLogs = Value;
                    break;

                case "Internal Users Only":
                    InternalUsersOnly = Value;
                    break;

                case "You Are Not Authorized To Access This View":
                    YouAreNotAuthorizedToAccessThisView = Value;
                    break;

                case "Click Here To Return To The Digital Library Home Page":
                    ClickHereToReturnToTheDigitalLibraryHomeP = Value;
                    break;

                case "The Data Below Shows Errors Which Occurred While Loading New Items Through The Builder These Can Be Displayed By Month And Year Below By Selecting The Start And End Month These Failures Will Continue To Display Until They Are Manually Cleared Or Until The Item Successfully Loads After The Failure Or Warning":
                    TheDataBelowShowsErrorsWhichOccurredWhileL = Value;
                    break;

                case "Selected Date Range":
                    SelectedDateRange = Value;
                    break;

                case "The Failures And Warnings Which Were Encountered During Build Are Searchable Below By Month":
                    TheFailuresAndWarningsWhichWereEncounteredD = Value;
                    break;

                case "From":
                    From = Value;
                    break;

                case "To":
                    To = Value;
                    break;

                case "To Change The Date Shown Choose Your Dates Above And Hit The GO Button":
                    ToChangeTheDateShownChooseYourDatesAboveA = Value;
                    break;

                case "Build Failures And Warnings":
                    BuildFailuresAndWarnings = Value;
                    break;

                case "No Uncleared Warnings Or Failures For The Selected Date Range":
                    NoUnclearedWarningsOrFailuresForTheSelected = Value;
                    break;

                case "METS Type":
                    METSType = Value;
                    break;

                case "Description":
                    Description = Value;
                    break;

                case "GLOBAL VALUES":
                    GLOBALVALUES = Value;
                    break;

                case "APPLICATION STATE VALUES":
                    APPLICATIONSTATEVALUES = Value;
                    break;

                case "LOCALLY CACHED OBJECTS":
                    LOCALLYCACHEDOBJECTS = Value;
                    break;

                case "REMOTELY CACHED OBJECTS":
                    REMOTELYCACHEDOBJECTS = Value;
                    break;

                case "SESSION STATE VALUES":
                    SESSIONSTATEVALUES = Value;
                    break;

                case "INSTANCE NAME":
                    INSTANCENAME = Value;
                    break;

                case "KEY":
                    KEY = Value;
                    break;

                case "OBJECT":
                    OBJECT = Value;
                    break;

                case "UNKNOWN OBJECT TYPE":
                    UNKNOWNOBJECTTYPE = Value;
                    break;

                case "None":
                    None = Value;
                    break;

                case "Select One Of The Aggregation Types Below To View Information About All Aggregations Of That Type":
                    SelectOneOfTheAggregationTypesBelowToView = Value;
                    break;

                case "All Aggregation Types":
                    AllAggregationTypes = Value;
                    break;

                case "Parent Aggregations":
                    ParentAggregations = Value;
                    break;

                case "Child Aggregations":
                    ChildAggregations = Value;
                    break;

                case "Active Aggregations":
                    ActiveAggregations = Value;
                    break;

                case "Inactive Aggregations":
                    InactiveAggregations = Value;
                    break;

                case "Below Is The Complete Master List Of All Aggregations Within This Library This Includes All Active Aggregations As Well As All Hidden Or Inactive Collections":
                    BelowIsTheCompleteMasterListOfAllAggregati = Value;
                    break;

                case "Click Here To Sort By DATE ADDED":
                    ClickHereToSortByDATEADDED = Value;
                    break;

                case "Click Here To Sort By CODE":
                    ClickHereToSortByCODE = Value;
                    break;

                case "Sobekcm Code":
                    SobekcmCode = Value;
                    break;

                case "Type":
                    Type = Value;
                    break;

                case "Name":
                    Name = Value;
                    break;

                case "Date Added":
                    DateAdded = Value;
                    break;

                case "METS TYPE":
                    METSTYPE = Value;
                    break;

                case "USER":
                    USER = Value;
                    break;

                case "ALL":
                    ALL = Value;
                    break;

                case "ONLINE EDITS":
                    ONLINEEDITS = Value;
                    break;

                case "ONLINE SUBMITS":
                    ONLINESUBMITS = Value;
                    break;

                case "VISIBILITY CHANGES":
                    VISIBILITYCHANGES = Value;
                    break;

                case "BULK LOADED":
                    BULKLOADED = Value;
                    break;

                case "POSTPROCESSED":
                    POSTPROCESSED = Value;
                    break;

                case "NO NEW ITEMS":
                    NONEWITEMS = Value;
                    break;

                case "There Have Been An Unusually Large Number Of Updates Over The Last Week":
                    ThereHaveBeenAnUnusuallyLargeNumberOfUpdat = Value;
                    break;

                case "Select The Update Type Tab Above To View The Details":
                    SelectTheUpdateTypeTabAboveToViewTheDetai = Value;
                    break;

                case "NO INFORMATION FOR YOUR SELECTION":
                    NOINFORMATIONFORYOURSELECTION = Value;
                    break;

            }
        }
        /// <remarks> 'Collection Hierarchy' localization string </remarks>
        public string CollectionHierarchy { get; private set; }

        /// <remarks> 'Active and Inactive Collections' localization string </remarks>
        public string ActiveAndInactiveCollections { get; private set; }

        /// <remarks> 'New Items' localization string </remarks>
        public string NewItems { get; private set; }

        /// <remarks> 'Newly Added or Modified Items' localization string </remarks>
        public string NewlyAddedOrModifiedItems { get; private set; }

        /// <remarks> 'Memory Management' localization string </remarks>
        public string MemoryManagement { get; private set; }

        /// <remarks> 'Current Memory Profile' localization string </remarks>
        public string CurrentMemoryProfile { get; private set; }

        /// <remarks> 'Wordmarks' localization string </remarks>
        public string Wordmarks { get; private set; }

        /// <remarks> 'Build Failures' localization string </remarks>
        public string BuildFailures { get; private set; }

        /// <remarks> 'Build Failure Logs' localization string </remarks>
        public string BuildFailureLogs { get; private set; }

        /// <remarks> 'Internal Users Only' localization string </remarks>
        public string InternalUsersOnly { get; private set; }

        /// <remarks> 'You are not authorized to access this view.' localization string </remarks>
        public string YouAreNotAuthorizedToAccessThisView { get; private set; }

        /// <remarks> 'Click here to return to the digital library home page.' localization string </remarks>
        public string ClickHereToReturnToTheDigitalLibraryHomeP { get; private set; }

        /// <remarks> 'The data below shows errors which occurred while loading new items through the builder.  These can be displayed by month and year below by selecting the start and end month.  These failures will continue to display until they are manually cleared or until the item successfully loads after the failure or warning.' localization string </remarks>
        public string TheDataBelowShowsErrorsWhichOccurredWhileL { get; private set; }

        /// <remarks> 'Selected Date Range' localization string </remarks>
        public string SelectedDateRange { get; private set; }

        /// <remarks> '"The failures and warnings which were encountered during build are searchable below, by month:"' localization string </remarks>
        public string TheFailuresAndWarningsWhichWereEncounteredD { get; private set; }

        /// <remarks> 'From:' localization string </remarks>
        public string From { get; private set; }

        /// <remarks> 'To:' localization string </remarks>
        public string To { get; private set; }

        /// <remarks> '"To change the date shown, choose your dates above and hit the GO button."' localization string </remarks>
        public string ToChangeTheDateShownChooseYourDatesAboveA { get; private set; }

        /// <remarks> 'Build Failures and Warnings' localization string </remarks>
        public string BuildFailuresAndWarnings { get; private set; }

        /// <remarks> 'No uncleared warnings or failures for the selected date range.' localization string </remarks>
        public string NoUnclearedWarningsOrFailuresForTheSelected { get; private set; }

        /// <remarks> 'METS Type' localization string </remarks>
        public string METSType { get; private set; }

        /// <remarks> 'Description' localization string </remarks>
        public string Description { get; private set; }

        /// <remarks> 'GLOBAL VALUES' localization string </remarks>
        public string GLOBALVALUES { get; private set; }

        /// <remarks> 'APPLICATION STATE VALUES' localization string </remarks>
        public string APPLICATIONSTATEVALUES { get; private set; }

        /// <remarks> 'LOCALLY CACHED OBJECTS' localization string </remarks>
        public string LOCALLYCACHEDOBJECTS { get; private set; }

        /// <remarks> 'REMOTELY CACHED OBJECTS' localization string </remarks>
        public string REMOTELYCACHEDOBJECTS { get; private set; }

        /// <remarks> 'SESSION STATE VALUES' localization string </remarks>
        public string SESSIONSTATEVALUES { get; private set; }

        /// <remarks> 'INSTANCE NAME' localization string </remarks>
        public string INSTANCENAME { get; private set; }

        /// <remarks> 'KEY' localization string </remarks>
        public string KEY { get; private set; }

        /// <remarks> 'OBJECT' localization string </remarks>
        public string OBJECT { get; private set; }

        /// <remarks> 'UNKNOWN OBJECT TYPE' localization string </remarks>
        public string UNKNOWNOBJECTTYPE { get; private set; }

        /// <remarks> '( none )' localization string </remarks>
        public string None { get; private set; }

        /// <remarks> 'Select one of the aggregation types below to view information about all aggregations of that type.' localization string </remarks>
        public string SelectOneOfTheAggregationTypesBelowToView { get; private set; }

        /// <remarks> 'All Aggregation Types' localization string </remarks>
        public string AllAggregationTypes { get; private set; }

        /// <remarks> 'Parent Aggregations' localization string </remarks>
        public string ParentAggregations { get; private set; }

        /// <remarks> 'Child Aggregations' localization string </remarks>
        public string ChildAggregations { get; private set; }

        /// <remarks> 'Active Aggregations' localization string </remarks>
        public string ActiveAggregations { get; private set; }

        /// <remarks> 'Inactive Aggregations' localization string </remarks>
        public string InactiveAggregations { get; private set; }

        /// <remarks> '"Below is the complete master list of all aggregations within this library.  This includes all active aggregations, as well as all hidden or inactive collections."' localization string </remarks>
        public string BelowIsTheCompleteMasterListOfAllAggregati { get; private set; }

        /// <remarks> 'Click here to sort by DATE ADDED' localization string </remarks>
        public string ClickHereToSortByDATEADDED { get; private set; }

        /// <remarks> 'Click here to sort by CODE' localization string </remarks>
        public string ClickHereToSortByCODE { get; private set; }

        /// <remarks> 'SobekCM Code' localization string </remarks>
        public string SobekcmCode { get; private set; }

        /// <remarks> 'Type' localization string </remarks>
        public string Type { get; private set; }

        /// <remarks> 'Name' localization string </remarks>
        public string Name { get; private set; }

        /// <remarks> 'Date Added' localization string </remarks>
        public string DateAdded { get; private set; }

        /// <remarks> 'METS TYPE' localization string </remarks>
        public string METSTYPE { get; private set; }

        /// <remarks> 'USER' localization string </remarks>
        public string USER { get; private set; }

        /// <remarks> 'ALL' localization string </remarks>
        public string ALL { get; private set; }

        /// <remarks> 'ONLINE EDITS' localization string </remarks>
        public string ONLINEEDITS { get; private set; }

        /// <remarks> 'ONLINE SUBMITS' localization string </remarks>
        public string ONLINESUBMITS { get; private set; }

        /// <remarks> 'VISIBILITY CHANGES' localization string </remarks>
        public string VISIBILITYCHANGES { get; private set; }

        /// <remarks> 'BULK LOADED' localization string </remarks>
        public string BULKLOADED { get; private set; }

        /// <remarks> 'POST-PROCESSED' localization string </remarks>
        public string POSTPROCESSED { get; private set; }

        /// <remarks> 'NO NEW ITEMS' localization string </remarks>
        public string NONEWITEMS { get; private set; }

        /// <remarks> 'There have been an unusually large number of updates over the last week.' localization string </remarks>
        public string ThereHaveBeenAnUnusuallyLargeNumberOfUpdat { get; private set; }

        /// <remarks> 'Select the update type tab above to view the details.' localization string </remarks>
        public string SelectTheUpdateTypeTabAboveToViewTheDetai { get; private set; }

        /// <remarks> 'NO INFORMATION FOR YOUR SELECTION' localization string </remarks>
        public string NOINFORMATIONFORYOURSELECTION { get; private set; }

    }
}
