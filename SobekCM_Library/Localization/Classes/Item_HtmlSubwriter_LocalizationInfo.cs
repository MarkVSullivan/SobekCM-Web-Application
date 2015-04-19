namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Item_HtmlSubwriter class </summary>
    public class Item_HtmlSubwriter_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Item_HtmlSubwriter_Localization class </summary>
        public Item_HtmlSubwriter_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Item_HtmlSubwriter";
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
                case "Error Encountered While Sending Email":
                    ErrorEncounteredWhileSendingEmail = Value;
                    break;

                case "Error Encountered While Trying To Save To Your Bookshelf":
                    ErrorEncounteredWhileTryingToSaveToYourBoo = Value;
                    break;

                case "Your Email Has Been Sent":
                    YourEmailHasBeenSent = Value;
                    break;

                case "Item Was Saved To Your Bookshelf":
                    ItemWasSavedToYourBookshelf = Value;
                    break;

                case "ERROR Encountered While Trying To Save To Your Bookshelf":
                    ERROREncounteredWhileTryingToSaveToYourBoo = Value;
                    break;

                case "Item Was Removed From Your Bookshelves":
                    ItemWasRemovedFromYourBookshelves = Value;
                    break;

                case "ERROR Encountered While Trying To Remove Item From Your Bookshelves":
                    ERROREncounteredWhileTryingToRemoveItemFrom = Value;
                    break;

                case "MATCHING PAGES":
                    MATCHINGPAGES = Value;
                    break;

                case "MATCHING TILES":
                    MATCHINGTILES = Value;
                    break;

                case "SHOW TABLE OF CONTENTS":
                    SHOWTABLEOFCONTENTS = Value;
                    break;

                case "HIDE":
                    HIDE = Value;
                    break;

                case "TABLE OF CONTENTS":
                    TABLEOFCONTENTS = Value;
                    break;

                case "HIDE TABLE OF CONTENTS":
                    HIDETABLEOFCONTENTS = Value;
                    break;

                case "View Work Log":
                    ViewWorkLog = Value;
                    break;

                case "Private Resource":
                    PrivateResource = Value;
                    break;

                case "Public Resource":
                    PublicResource = Value;
                    break;

                case "Accession Number":
                    AccessionNumber = Value;
                    break;

                case "Usage Statistics":
                    UsageStatistics = Value;
                    break;

                case "MARC View":
                    MARCView = Value;
                    break;

                case "Metadata":
                    Metadata = Value;
                    break;

                case "Tree View":
                    TreeView = Value;
                    break;

                case "Thumbnails":
                    Thumbnails = Value;
                    break;

                case "List View":
                    ListView = Value;
                    break;

                case "Manage Download Files":
                    ManageDownloadFiles = Value;
                    break;

                case "Edit Item Behaviors":
                    EditItemBehaviors = Value;
                    break;

                case "Manage Pages And Divisions":
                    ManagePagesAndDivisions = Value;
                    break;

                case "Manage Geospatial Data":
                    ManageGeospatialData = Value;
                    break;

                case "Beta":
                    Beta = Value;
                    break;

                case "View Tracking Sheet":
                    ViewTrackingSheet = Value;
                    break;

                case "Mass Update Item Behaviors":
                    MassUpdateItemBehaviors = Value;
                    break;

                case "First Page":
                    FirstPage = Value;
                    break;

                case "Edit Item Group Behaviors":
                    EditItemGroupBehaviors = Value;
                    break;

                case "Previous Page":
                    PreviousPage = Value;
                    break;

                case "Next Page":
                    NextPage = Value;
                    break;

                case "Last Page":
                    LastPage = Value;
                    break;

                case "Next":
                    Next = Value;
                    break;

                case "Last":
                    Last = Value;
                    break;

                case "First":
                    First = Value;
                    break;

                case "Previous":
                    Previous = Value;
                    break;

                case "Edit Metadata":
                    EditMetadata = Value;
                    break;

                case "Edit Behaviors":
                    EditBehaviors = Value;
                    break;

                case "Change Access Restriction":
                    ChangeAccessRestriction = Value;
                    break;

                case "Public Item":
                    PublicItem = Value;
                    break;

                case "Private Item":
                    PrivateItem = Value;
                    break;

                case "Dark Item":
                    DarkItem = Value;
                    break;

                case "Restricted Item":
                    RestrictedItem = Value;
                    break;

                case "View Work History":
                    ViewWorkHistory = Value;
                    break;

                case "Manage Files":
                    ManageFiles = Value;
                    break;

                case "Perform QC":
                    PerformQC = Value;
                    break;

                case "Perform Quality Control":
                    PerformQualityControl = Value;
                    break;

                case "Comments":
                    Comments = Value;
                    break;

                case "Save New Internal Comments":
                    SaveNewInternalComments = Value;
                    break;

                case "SET ACCESS RESTRICTIONS":
                    SETACCESSRESTRICTIONS = Value;
                    break;

                case "Make Item Public":
                    MakeItemPublic = Value;
                    break;

                case "Add IP Restriction To This Item":
                    AddIPRestrictionToThisItem = Value;
                    break;

                case "Make Item Private":
                    MakeItemPrivate = Value;
                    break;

                case "Delete This Item":
                    DeleteThisItem = Value;
                    break;

                case "Cancel Changes":
                    CancelChanges = Value;
                    break;

                case "Add Volume":
                    AddVolume = Value;
                    break;

                case "Autofill Volumes":
                    AutofillVolumes = Value;
                    break;

                case "Edit Serial Hierarchy":
                    EditSerialHierarchy = Value;
                    break;

                case "Mass Update Volumes":
                    MassUpdateVolumes = Value;
                    break;

                case "Hide Internal Header":
                    HideInternalHeader = Value;
                    break;

                case "Show Internal Header":
                    ShowInternalHeader = Value;
                    break;

                case "Container List":
                    ContainerList = Value;
                    break;

                case "DARK ITEM":
                    DARKITEM = Value;
                    break;

                case "PRIVATE ITEM":
                    PRIVATEITEM = Value;
                    break;

                case "Digitization Of This Item Is Currently In Progress":
                    DigitizationOfThisItemIsCurrentlyInProgress = Value;
                    break;

                case "Go To":
                    GoTo = Value;
                    break;

            }
        }
        /// <remarks> 'Error encountered while sending email' localization string </remarks>
        public string ErrorEncounteredWhileSendingEmail { get; private set; }

        /// <remarks> 'Error encountered while trying to save to your bookshelf' localization string </remarks>
        public string ErrorEncounteredWhileTryingToSaveToYourBoo { get; private set; }

        /// <remarks> 'Your email has been sent' localization string </remarks>
        public string YourEmailHasBeenSent { get; private set; }

        /// <remarks> 'Item was saved to your bookshelf.' localization string </remarks>
        public string ItemWasSavedToYourBookshelf { get; private set; }

        /// <remarks> 'ERROR encountered while trying to save to your bookshelf.' localization string </remarks>
        public string ERROREncounteredWhileTryingToSaveToYourBoo { get; private set; }

        /// <remarks> 'Item was removed from your bookshelves.' localization string </remarks>
        public string ItemWasRemovedFromYourBookshelves { get; private set; }

        /// <remarks> 'ERROR encountered while trying to remove item from your bookshelves.' localization string </remarks>
        public string ERROREncounteredWhileTryingToRemoveItemFrom { get; private set; }

        /// <remarks> 'MATCHING PAGES' localization string </remarks>
        public string MATCHINGPAGES { get; private set; }

        /// <remarks> 'MATCHING TILES' localization string </remarks>
        public string MATCHINGTILES { get; private set; }

        /// <remarks> 'SHOW TABLE OF CONTENTS' localization string </remarks>
        public string SHOWTABLEOFCONTENTS { get; private set; }

        /// <remarks> 'HIDE' localization string </remarks>
        public string HIDE { get; private set; }

        /// <remarks> 'TABLE OF CONTENTS' localization string </remarks>
        public string TABLEOFCONTENTS { get; private set; }

        /// <remarks> 'HIDE TABLE OF CONTENTS' localization string </remarks>
        public string HIDETABLEOFCONTENTS { get; private set; }

        /// <remarks> 'View Work Log' localization string </remarks>
        public string ViewWorkLog { get; private set; }

        /// <remarks> 'Private Resource' localization string </remarks>
        public string PrivateResource { get; private set; }

        /// <remarks> 'Public Resource' localization string </remarks>
        public string PublicResource { get; private set; }

        /// <remarks> 'Accession number' localization string </remarks>
        public string AccessionNumber { get; private set; }

        /// <remarks> 'Usage Statistics' localization string </remarks>
        public string UsageStatistics { get; private set; }

        /// <remarks> 'MARC View' localization string </remarks>
        public string MARCView { get; private set; }

        /// <remarks> 'Metadata' localization string </remarks>
        public string Metadata { get; private set; }

        /// <remarks> 'Tree View' localization string </remarks>
        public string TreeView { get; private set; }

        /// <remarks> 'Thumbnails' localization string </remarks>
        public string Thumbnails { get; private set; }

        /// <remarks> 'List View' localization string </remarks>
        public string ListView { get; private set; }

        /// <remarks> 'Manage Download Files' localization string </remarks>
        public string ManageDownloadFiles { get; private set; }

        /// <remarks> 'Edit Item Behaviors' localization string </remarks>
        public string EditItemBehaviors { get; private set; }

        /// <remarks> 'Manage Pages and Divisions' localization string </remarks>
        public string ManagePagesAndDivisions { get; private set; }

        /// <remarks> 'Manage Geo-Spatial Data' localization string </remarks>
        public string ManageGeospatialData { get; private set; }

        /// <remarks> 'beta' localization string </remarks>
        public string Beta { get; private set; }

        /// <remarks> 'View Tracking Sheet' localization string </remarks>
        public string ViewTrackingSheet { get; private set; }

        /// <remarks> 'Mass Update Item Behaviors' localization string </remarks>
        public string MassUpdateItemBehaviors { get; private set; }

        /// <remarks> 'First Page' localization string </remarks>
        public string FirstPage { get; private set; }

        /// <remarks> 'Edit Item Group Behaviors' localization string </remarks>
        public string EditItemGroupBehaviors { get; private set; }

        /// <remarks> 'Previous Page' localization string </remarks>
        public string PreviousPage { get; private set; }

        /// <remarks> 'Next Page' localization string </remarks>
        public string NextPage { get; private set; }

        /// <remarks> 'Last Page' localization string </remarks>
        public string LastPage { get; private set; }

        /// <remarks> 'Next' localization string </remarks>
        public string Next { get; private set; }

        /// <remarks> 'Last' localization string </remarks>
        public string Last { get; private set; }

        /// <remarks> 'First' localization string </remarks>
        public string First { get; private set; }

        /// <remarks> 'Previous' localization string </remarks>
        public string Previous { get; private set; }

        /// <remarks> 'Edit Metadata' localization string </remarks>
        public string EditMetadata { get; private set; }

        /// <remarks> 'Edit Behaviors' localization string </remarks>
        public string EditBehaviors { get; private set; }

        /// <remarks> 'Change Access Restriction' localization string </remarks>
        public string ChangeAccessRestriction { get; private set; }

        /// <remarks> 'Public Item' localization string </remarks>
        public string PublicItem { get; private set; }

        /// <remarks> 'Private Item' localization string </remarks>
        public string PrivateItem { get; private set; }

        /// <remarks> 'Dark Item' localization string </remarks>
        public string DarkItem { get; private set; }

        /// <remarks> 'Restricted Item' localization string </remarks>
        public string RestrictedItem { get; private set; }

        /// <remarks> 'View Work History' localization string </remarks>
        public string ViewWorkHistory { get; private set; }

        /// <remarks> 'Manage Files' localization string </remarks>
        public string ManageFiles { get; private set; }

        /// <remarks> 'Perform QC' localization string </remarks>
        public string PerformQC { get; private set; }

        /// <remarks> 'Perform Quality Control' localization string </remarks>
        public string PerformQualityControl { get; private set; }

        /// <remarks> 'Comments' localization string </remarks>
        public string Comments { get; private set; }

        /// <remarks> 'Save new internal comments' localization string </remarks>
        public string SaveNewInternalComments { get; private set; }

        /// <remarks> 'SET ACCESS RESTRICTIONS' localization string </remarks>
        public string SETACCESSRESTRICTIONS { get; private set; }

        /// <remarks> 'Make item public' localization string </remarks>
        public string MakeItemPublic { get; private set; }

        /// <remarks> 'Add IP restriction to this item' localization string </remarks>
        public string AddIPRestrictionToThisItem { get; private set; }

        /// <remarks> 'Make item private' localization string </remarks>
        public string MakeItemPrivate { get; private set; }

        /// <remarks> 'Delete this item' localization string </remarks>
        public string DeleteThisItem { get; private set; }

        /// <remarks> 'Cancel changes' localization string </remarks>
        public string CancelChanges { get; private set; }

        /// <remarks> 'Add Volume' localization string </remarks>
        public string AddVolume { get; private set; }

        /// <remarks> 'Auto-Fill Volumes' localization string </remarks>
        public string AutofillVolumes { get; private set; }

        /// <remarks> 'Edit Serial Hierarchy' localization string </remarks>
        public string EditSerialHierarchy { get; private set; }

        /// <remarks> 'Mass Update Volumes' localization string </remarks>
        public string MassUpdateVolumes { get; private set; }

        /// <remarks> 'Hide Internal Header' localization string </remarks>
        public string HideInternalHeader { get; private set; }

        /// <remarks> 'Show Internal Header' localization string </remarks>
        public string ShowInternalHeader { get; private set; }

        /// <remarks> 'Container List' localization string </remarks>
        public string ContainerList { get; private set; }

        /// <remarks> 'DARK ITEM' localization string </remarks>
        public string DARKITEM { get; private set; }

        /// <remarks> 'PRIVATE ITEM' localization string </remarks>
        public string PRIVATEITEM { get; private set; }

        /// <remarks> 'Digitization of this item is currently in progress.' localization string </remarks>
        public string DigitizationOfThisItemIsCurrentlyInProgress { get; private set; }

        /// <remarks> 'Go To:' localization string </remarks>
        public string GoTo { get; private set; }

    }
}
