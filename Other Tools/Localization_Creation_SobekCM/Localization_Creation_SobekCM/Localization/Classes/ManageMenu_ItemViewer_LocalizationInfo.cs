namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the ManageMenu_ItemViewer class </summary>
    public class ManageMenu_ItemViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the ManageMenu_ItemViewer_Localization class </summary>
        public ManageMenu_ItemViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "ManageMenu_ItemViewer";
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
                case "Beta":
                    Beta = Value;
                    break;

                case "Add A New Related Volume To This Item Group":
                    AddANewRelatedVolumeToThisItemGroup = Value;
                    break;

                case "Add Geospatial Information For This Item This Can Be As Simple As A Location For A Photograph Or Can Be An Overlay For A Map Points Lines And Polygons Of Interest Can Also Be Drawn":
                    AddGeospatialInformationForThisItemThisCanBeAsSimpleAsALocationForAPhotographOrCanBeAnOverlayForAMapPointsLinesAndPolygonsOfInterestCanAlsoBeDrawn = Value;
                    break;

                case "Add New Volume":
                    AddNewVolume = Value;
                    break;

                case "Change The Way This Item Behaves In This Library Including Which Aggregations It Appears Under The Wordmarks To The Left And Which Viewer Types Are Publicly Accessible":
                    ChangeTheWayThisItemBehavesInThisLibraryIncludingWhichAggregationsItAppearsUnderTheWordmarksToTheLeftAndWhichViewerTypesArePubliclyAccessible = Value;
                    break;

                case "Edit Item Behaviors":
                    EditItemBehaviors = Value;
                    break;

                case "Edit Item Group Behaviors":
                    EditItemGroupBehaviors = Value;
                    break;

                case "Edit Item Metadata":
                    EditItemMetadata = Value;
                    break;

                case "Edit The Information About This Item Which Appears In The Citationdescription This Is Basic Information About The Original Item And This Digital Manifestation":
                    EditTheInformationAboutThisItemWhichAppearsInTheCitationdescriptionThisIsBasicInformationAboutTheOriginalItemAndThisDigitalManifestation = Value;
                    break;

                case "How Would You Like To Manage This Item Group Today":
                    HowWouldYouLikeToManageThisItemGroupToday = Value;
                    break;

                case "How Would You Like To Manage This Item Today":
                    HowWouldYouLikeToManageThisItemToday = Value;
                    break;

                case "In Addition The Following Changes Can Be Made At The Item Group Level":
                    InAdditionTheFollowingChangesCanBeMadeAtTheItemGroupLevel = Value;
                    break;

                case "Manage Download Files":
                    ManageDownloadFiles = Value;
                    break;

                case "Manage Geospatial Data":
                    ManageGeospatialData = Value;
                    break;

                case "Manage Pages And Divisions Quality Control":
                    ManagePagesAndDivisionsQualityControl = Value;
                    break;

                case "Manage This Item":
                    ManageThisItem = Value;
                    break;

                case "Manage This Item Group":
                    ManageThisItemGroup = Value;
                    break;

                case "Mass Update Item Behaviors":
                    MassUpdateItemBehaviors = Value;
                    break;

                case "Reorder Page Images Name Pages Assign Divisions And Delete And Add New Page Images To This Item":
                    ReorderPageImagesNamePagesAssignDivisionsAndDeleteAndAddNewPageImagesToThisItem = Value;
                    break;

                case "Set The Title Under Which All Of These Items Appear In Search Results And Set The Web Skins Under Which All These Items Should Appear":
                    SetTheTitleUnderWhichAllOfTheseItemsAppearInSearchResultsAndSetTheWebSkinsUnderWhichAllTheseItemsShouldAppear = Value;
                    break;

                case "This Allows Itemlevel Behaviors To Be Set For All Items Within This Item Group Including Which Aggregations It Appears Under The Wordmarks To The Left And Which Viewer Types Are Publicly Accessible":
                    ThisAllowsItemlevelBehaviorsToBeSetForAllItemsWithinThisItemGroupIncludingWhichAggregationsItAppearsUnderTheWordmarksToTheLeftAndWhichViewerTypesArePubliclyAccessible = Value;
                    break;

                case "This Can Be Used For Printing The Tracking Sheet For This Item Which Can Be Used As Part Of The Builtin Digitization Workflow":
                    ThisCanBeUsedForPrintingTheTrackingSheetForThisItemWhichCanBeUsedAsPartOfTheBuiltinDigitizationWorkflow = Value;
                    break;

                case "Upload New Files For Download Or Remove Existing Files That Are Attached To This Item For Download This Generally Includes Everything Except For The Page Images":
                    UploadNewFilesForDownloadOrRemoveExistingFilesThatAreAttachedToThisItemForDownloadThisGenerallyIncludesEverythingExceptForThePageImages = Value;
                    break;

                case "View The History Of All Work Performed On This Item From This View You Can Also See Any Digitization Milestones And Digital Resource File Information":
                    ViewTheHistoryOfAllWorkPerformedOnThisItemFromThisViewYouCanAlsoSeeAnyDigitizationMilestonesAndDigitalResourceFileInformation = Value;
                    break;

                case "View Tracking Sheet":
                    ViewTrackingSheet = Value;
                    break;

                case "View Work History":
                    ViewWorkHistory = Value;
                    break;

            }
        }
        /// <remarks> '(beta)' localization string </remarks>
        public string Beta { get; private set; }

        /// <remarks> '"Add a new, related volume to this item group."' localization string </remarks>
        public string AddANewRelatedVolumeToThisItemGroup { get; private set; }

        /// <remarks> '"Add geo-spatial information for this item.  This can be as simple as a location for a photograph or can be an overlay for a map.  Points, lines, and polygons of interest can also be drawn."' localization string </remarks>
        public string AddGeospatialInformationForThisItemThisCanBeAsSimpleAsALocationForAPhotographOrCanBeAnOverlayForAMapPointsLinesAndPolygonsOfInterestCanAlsoBeDrawn { get; private set; }

        /// <remarks> 'Add New Volume' localization string </remarks>
        public string AddNewVolume { get; private set; }

        /// <remarks> '"Change the way this item behaves in this library, including which aggregations it appears under, the wordmarks to the left, and which viewer types are publicly accessible."' localization string </remarks>
        public string ChangeTheWayThisItemBehavesInThisLibraryIncludingWhichAggregationsItAppearsUnderTheWordmarksToTheLeftAndWhichViewerTypesArePubliclyAccessible { get; private set; }

        /// <remarks> 'Edit Item Behaviors' localization string </remarks>
        public string EditItemBehaviors { get; private set; }

        /// <remarks> 'Edit Item Group Behaviors' localization string </remarks>
        public string EditItemGroupBehaviors { get; private set; }

        /// <remarks> 'Edit Item Metadata' localization string </remarks>
        public string EditItemMetadata { get; private set; }

        /// <remarks> 'Edit the information about this item which appears in the citation/description.  This is basic information about the original item and this digital manifestation.' localization string </remarks>
        public string EditTheInformationAboutThisItemWhichAppearsInTheCitationdescriptionThisIsBasicInformationAboutTheOriginalItemAndThisDigitalManifestation { get; private set; }

        /// <remarks> 'How would you like to manage this item group today?' localization string </remarks>
        public string HowWouldYouLikeToManageThisItemGroupToday { get; private set; }

        /// <remarks> 'How would you like to manage this item today?' localization string </remarks>
        public string HowWouldYouLikeToManageThisItemToday { get; private set; }

        /// <remarks> '"In addition, the following changes can be made at the item group level:"' localization string </remarks>
        public string InAdditionTheFollowingChangesCanBeMadeAtTheItemGroupLevel { get; private set; }

        /// <remarks> 'Manage Download Files' localization string </remarks>
        public string ManageDownloadFiles { get; private set; }

        /// <remarks> 'Manage Geo-Spatial Data' localization string </remarks>
        public string ManageGeospatialData { get; private set; }

        /// <remarks> 'Manage Pages and Divisions (Quality Control)' localization string </remarks>
        public string ManagePagesAndDivisionsQualityControl { get; private set; }

        /// <remarks> 'Manage this Item' localization string </remarks>
        public string ManageThisItem { get; private set; }

        /// <remarks> 'Manage this Item Group' localization string </remarks>
        public string ManageThisItemGroup { get; private set; }

        /// <remarks> 'Mass Update Item Behaviors' localization string </remarks>
        public string MassUpdateItemBehaviors { get; private set; }

        /// <remarks> '"Reorder page images, name pages, assign divisions, and delete and add new page images to this item."' localization string </remarks>
        public string ReorderPageImagesNamePagesAssignDivisionsAndDeleteAndAddNewPageImagesToThisItem { get; private set; }

        /// <remarks> 'Set the title under which all of these items appear in search results and set the web skins under which all these items should appear.' localization string </remarks>
        public string SetTheTitleUnderWhichAllOfTheseItemsAppearInSearchResultsAndSetTheWebSkinsUnderWhichAllTheseItemsShouldAppear { get; private set; }

        /// <remarks> '"This allows item-level behaviors to be set for all items within this item group, including which aggregations it appears under, the wordmarks to the left, and which viewer types are publicly accessible."' localization string </remarks>
        public string ThisAllowsItemlevelBehaviorsToBeSetForAllItemsWithinThisItemGroupIncludingWhichAggregationsItAppearsUnderTheWordmarksToTheLeftAndWhichViewerTypesArePubliclyAccessible { get; private set; }

        /// <remarks> '"This can be used for printing the tracking sheet for this item, which can be used as part of the built-in digitization workflow."' localization string </remarks>
        public string ThisCanBeUsedForPrintingTheTrackingSheetForThisItemWhichCanBeUsedAsPartOfTheBuiltinDigitizationWorkflow { get; private set; }

        /// <remarks> 'Upload new files for download or remove existing files that are attached to this item for download.  This generally includes everything except for the page images.' localization string </remarks>
        public string UploadNewFilesForDownloadOrRemoveExistingFilesThatAreAttachedToThisItemForDownloadThisGenerallyIncludesEverythingExceptForThePageImages { get; private set; }

        /// <remarks> '"View the history of all work performed on this item.  From this view, you can also see any digitization milestones and digital resource file information."' localization string </remarks>
        public string ViewTheHistoryOfAllWorkPerformedOnThisItemFromThisViewYouCanAlsoSeeAnyDigitizationMilestonesAndDigitalResourceFileInformation { get; private set; }

        /// <remarks> 'View Tracking Sheet' localization string </remarks>
        public string ViewTrackingSheet { get; private set; }

        /// <remarks> 'View Work History' localization string </remarks>
        public string ViewWorkHistory { get; private set; }

    }
}
