namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Private_Items_AggregationViewer class </summary>
    public class Private_Items_AggregationViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Private_Items_AggregationViewer_Localization class </summary>
        public Private_Items_AggregationViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Private_Items_AggregationViewer";
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
                case "Days Ago":
                    DaysAgo = Value;
                    break;

                case "XXX Volumes Out Of XXX Total Volumes":
                    XXXVolumesOutOfXXXTotalVolumes = Value;
                    break;

                case "Below Is The List Of All Items Linked To This Aggregation Which Are Either Private In Process Or Dark":
                    BelowIsTheListOfAllItemsLinkedToThisAggregationWhichAreEitherPrivateInProcessOrDark = Value;
                    break;

                case "Bibid VID":
                    BibidVID = Value;
                    break;

                case "Dated XXX":
                    DatedXXX = Value;
                    break;

                case "ERROR PULLING INFORMATION FROM DATABASE":
                    ERRORPULLINGINFORMATIONFROMDATABASE = Value;
                    break;

                case "First Page":
                    FirstPage = Value;
                    break;

                case "Last Activity":
                    LastActivity = Value;
                    break;

                case "Last Activity Date Most Recent First":
                    LastActivityDateMostRecentFirst = Value;
                    break;

                case "Last Activity Date Oldest First":
                    LastActivityDateOldestFirst = Value;
                    break;

                case "Last Milestone":
                    LastMilestone = Value;
                    break;

                case "Last Milestone Date Most Recent First":
                    LastMilestoneDateMostRecentFirst = Value;
                    break;

                case "Last Milestone Date Oldest First":
                    LastMilestoneDateOldestFirst = Value;
                    break;

                case "Last Page":
                    LastPage = Value;
                    break;

                case "Next Page":
                    NextPage = Value;
                    break;

                case "Online Completed":
                    OnlineCompleted = Value;
                    break;

                case "Previous Page":
                    PreviousPage = Value;
                    break;

                case "Private And Dark Items":
                    PrivateAndDarkItems = Value;
                    break;

                case "Processed":
                    Processed = Value;
                    break;

                case "Quality Control":
                    QualityControl = Value;
                    break;

                case "Record Created":
                    RecordCreated = Value;
                    break;

                case "Scanned":
                    Scanned = Value;
                    break;

                case "There Are A Total Of XXX Items In XXX Titles":
                    ThereAreATotalOfXXXItemsInXXXTitles = Value;
                    break;

                case "There Are A Total Of XXX Items In One Title":
                    ThereAreATotalOfXXXItemsInOneTitle = Value;
                    break;

                case "There Are A Total Of XXX Titles":
                    ThereAreATotalOfXXXTitles = Value;
                    break;

                case "There Is Only One Matching Item":
                    ThereIsOnlyOneMatchingItem = Value;
                    break;

                case "This Collection Does Not Include Any PRIVATE Or DARK Items":
                    ThisCollectionDoesNotIncludeAnyPRIVATEOrDARKItems = Value;
                    break;

                case "Title":
                    Title = Value;
                    break;

                case "Title VID":
                    TitleVID = Value;
                    break;

                case "Unknown Milestone":
                    UnknownMilestone = Value;
                    break;

            }
        }
        /// <remarks> "i.e., '32 days ago'. - time since last milestone or activity" </remarks>
        public string DaysAgo { get; private set; }

        /// <remarks> '%1 volumes out of %2 total volumes' localization string </remarks>
        public string XXXVolumesOutOfXXXTotalVolumes { get; private set; }

        /// <remarks> 'Below is the list of all items linked to this aggregation which are either private (in process) or dark.' localization string </remarks>
        public string BelowIsTheListOfAllItemsLinkedToThisAggregationWhichAreEitherPrivateInProcessOrDark { get; private set; }

        /// <remarks> "Sort by options for the administrative view on all non-public items, accessible from the internal header in aggregationPermissions" </remarks>
        public string BibidVID { get; private set; }

        /// <remarks> Used for publication date display ( to differentiate from milestone dates) </remarks>
        public string DatedXXX { get; private set; }

        /// <remarks> 'ERROR PULLING INFORMATION FROM DATABASE' localization string </remarks>
        public string ERRORPULLINGINFORMATIONFROMDATABASE { get; private set; }

        /// <remarks> 'First Page' localization string </remarks>
        public string FirstPage { get; private set; }

        /// <remarks> 'Last Activity' localization string </remarks>
        public string LastActivity { get; private set; }

        /// <remarks> "Sort by options for the administrative view on all non-public items, accessible from the internal header in aggregationPermissions" </remarks>
        public string LastActivityDateMostRecentFirst { get; private set; }

        /// <remarks> "Sort by options for the administrative view on all non-public items, accessible from the internal header in aggregationPermissions" </remarks>
        public string LastActivityDateOldestFirst { get; private set; }

        /// <remarks> 'Last Milestone' localization string </remarks>
        public string LastMilestone { get; private set; }

        /// <remarks> "Sort by options for the administrative view on all non-public items, accessible from the internal header in aggregationPermissions" </remarks>
        public string LastMilestoneDateMostRecentFirst { get; private set; }

        /// <remarks> "Sort by options for the administrative view on all non-public items, accessible from the internal header in aggregationPermissions" </remarks>
        public string LastMilestoneDateOldestFirst { get; private set; }

        /// <remarks> 'Last Page' localization string </remarks>
        public string LastPage { get; private set; }

        /// <remarks> 'Next Page' localization string </remarks>
        public string NextPage { get; private set; }

        /// <remarks> Name of the last milestons performed </remarks>
        public string OnlineCompleted { get; private set; }

        /// <remarks> 'Previous Page' localization string </remarks>
        public string PreviousPage { get; private set; }

        /// <remarks> Title for the administrative view on all non-public items </remarks>
        public string PrivateAndDarkItems { get; private set; }

        /// <remarks> Name of the last milestons performed </remarks>
        public string Processed { get; private set; }

        /// <remarks> Name of the last milestons performed </remarks>
        public string QualityControl { get; private set; }

        /// <remarks> Name of the last milestons performed </remarks>
        public string RecordCreated { get; private set; }

        /// <remarks> Name of the last milestons performed </remarks>
        public string Scanned { get; private set; }

        /// <remarks> 'There are a total of %1 items in %2 titles.' localization string </remarks>
        public string ThereAreATotalOfXXXItemsInXXXTitles { get; private set; }

        /// <remarks> 'There are a total of %1 items in one title.' localization string </remarks>
        public string ThereAreATotalOfXXXItemsInOneTitle { get; private set; }

        /// <remarks> 'There are a total of %1 titles.' localization string </remarks>
        public string ThereAreATotalOfXXXTitles { get; private set; }

        /// <remarks> 'There is only one matching item.' localization string </remarks>
        public string ThereIsOnlyOneMatchingItem { get; private set; }

        /// <remarks> 'This collection does not include any PRIVATE or DARK items.' localization string </remarks>
        public string ThisCollectionDoesNotIncludeAnyPRIVATEOrDARKItems { get; private set; }

        /// <remarks> 'Title' localization string </remarks>
        public string Title { get; private set; }

        /// <remarks> "Sort by options for the administrative view on all non-public items, accessible from the internal header in aggregationPermissions" </remarks>
        public string TitleVID { get; private set; }

        /// <remarks> Name of the last milestons performed </remarks>
        public string UnknownMilestone { get; private set; }

    }
}
