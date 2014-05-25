namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Map_Search_AggregationViewer class </summary>
    public class Map_Search_AggregationViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Map_Search_AggregationViewer_Localization class </summary>
        public Map_Search_AggregationViewer_LocalizationInfo() : base()
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
                case "Use The I Select Area I Button And Click To Select Opposite Corners To Draw A Search Box On The Map":
                    UseTheSelectAreaButtonAndClickToSelectOppositeCornersToDrawASearchBoxOnTheMap = Value;
                    break;

                case "Use One Of The Methods Below To Define Your Geographic Search":
                    UseOneOfTheMethodsBelowToDefineYourGeographicSearch = Value;
                    break;

                case "Press The I Search I Button To See Results":
                    PressTheSearchButtonToSeeResults = Value;
                    break;

                case "A Enter An Address And Press I Find Address I To Locate I Or I":
                    AEnterAnAddressAndPressFindAddressToLocateOr = Value;
                    break;

                case "Actions":
                    Actions = Value;
                    break;

                case "Address":
                    Address = Value;
                    break;

                case "Address This Is The Nearest Address Of The Point You Selected":
                    AddressThisIsTheNearestAddressOfThePointYouSelected = Value;
                    break;

                case "Applied":
                    Applied = Value;
                    break;

                case "Apply":
                    Apply = Value;
                    break;

                case "Apply Changes Make Changes Public":
                    ApplyChangesMakeChangesPublic = Value;
                    break;

                case "B Press The I Select Area I Button And Click To Select Two Opposite Corners I Or I":
                    BPressTheSelectAreaButtonAndClickToSelectTwoOppositeCornersOr = Value;
                    break;

                case "Blocklot Toggle Blocklot Map Layer":
                    BlocklotToggleBlocklotMapLayer = Value;
                    break;

                case "C Press The I Select Point I Button And Click To Select A Single Point":
                    CPressTheSelectPointButtonAndClickToSelectASinglePoint = Value;
                    break;

                case "Cancel Editing":
                    CancelEditing = Value;
                    break;

                case "Canceling":
                    Canceling = Value;
                    break;

                case "Cannot Convert":
                    CannotConvert = Value;
                    break;

                case "Cannot Zoom In Further":
                    CannotZoomInFurther = Value;
                    break;

                case "Cannot Zoom Out Further":
                    CannotZoomOutFurther = Value;
                    break;

                case "Center On Current Location":
                    CenterOnCurrentLocation = Value;
                    break;

                case "Center On Your Current Position":
                    CenterOnYourCurrentPosition = Value;
                    break;

                case "Circle":
                    Circle = Value;
                    break;

                case "Circle Place A Circle":
                    CirclePlaceACircle = Value;
                    break;

                case "Clear Point Of Interest Set":
                    ClearPointOfInterestSet = Value;
                    break;

                case "Click Here To View Instructions For This Search Interface":
                    ClickHereToViewInstructionsForThisSearchInterface = Value;
                    break;

                case "Clockwise":
                    Clockwise = Value;
                    break;

                case "Complete Editing":
                    CompleteEditing = Value;
                    break;

                case "Completed":
                    Completed = Value;
                    break;

                case "Controls":
                    Controls = Value;
                    break;

                case "Controls Toggle Map Controls":
                    ControlsToggleMapControls = Value;
                    break;

                case "Convert This To A Map Overlay":
                    ConvertThisToAMapOverlay = Value;
                    break;

                case "Convert To Overlay":
                    ConvertToOverlay = Value;
                    break;

                case "Converted Item To Overlay":
                    ConvertedItemToOverlay = Value;
                    break;

                case "Coordinate Data Removed For":
                    CoordinateDataRemovedFor = Value;
                    break;

                case "Coordinates Copied To Clipboard":
                    CoordinatesCopiedToClipboard = Value;
                    break;

                case "Coordinates Viewer Frozen":
                    CoordinatesViewerFrozen = Value;
                    break;

                case "Coordinates Viewer Unfrozen":
                    CoordinatesViewerUnfrozen = Value;
                    break;

                case "Coordinates This Is The Selected Latitude And Longitude Of The Point You Selected":
                    CoordinatesThisIsTheSelectedLatitudeAndLongitudeOfThePointYouSelected = Value;
                    break;

                case "Could Not Find Location Either The Format You Entered Is Invalid Or The Location Is Outside Of The Map Bounds":
                    CouldNotFindLocationEitherTheFormatYouEnteredIsInvalidOrTheLocationIsOutsideOfTheMapBounds = Value;
                    break;

                case "Could Not Find Within Bounds":
                    CouldNotFindWithinBounds = Value;
                    break;

                case "Counterclockwise":
                    Counterclockwise = Value;
                    break;

                case "Custom":
                    Custom = Value;
                    break;

                case "Default Pan Map To Default":
                    DefaultPanMapToDefault = Value;
                    break;

                case "Delete":
                    Delete = Value;
                    break;

                case "Delete Coordinate Data For Overlay":
                    DeleteCoordinateDataForOverlay = Value;
                    break;

                case "Delete Geographic Location":
                    DeleteGeographicLocation = Value;
                    break;

                case "Delete POI":
                    DeletePOI = Value;
                    break;

                case "Delete Search Result":
                    DeleteSearchResult = Value;
                    break;

                case "Deleted":
                    Deleted = Value;
                    break;

                case "Description Optional":
                    DescriptionOptional = Value;
                    break;

                case "Did Not Reset Page":
                    DidNotResetPage = Value;
                    break;

                case "Documentation":
                    Documentation = Value;
                    break;

                case "Down Pan Map Down":
                    DownPanMapDown = Value;
                    break;

                case "Edit Location":
                    EditLocation = Value;
                    break;

                case "Edit Location By Dragging Existing Marker":
                    EditLocationByDraggingExistingMarker = Value;
                    break;

                case "Edit This Overlay":
                    EditThisOverlay = Value;
                    break;

                case "Edit This POI":
                    EditThisPOI = Value;
                    break;

                case "Editing":
                    Editing = Value;
                    break;

                case "Enter Address Ie 12 Main Street Gainesville Florida":
                    EnterAddressIe12MainStreetGainesvilleFlorida = Value;
                    break;

                case "Error Addign Other Listeners":
                    ErrorAddignOtherListeners = Value;
                    break;

                case "ERROR Failed Adding Textual Content":
                    ERRORFailedAddingTextualContent = Value;
                    break;

                case "ERROR Failed Adding Titles":
                    ERRORFailedAddingTitles = Value;
                    break;

                case "Error Description Cannot Contain A Or":
                    ErrorDescriptionCannotContainAOr = Value;
                    break;

                case "Error Overlay Image Source Cannot Contain A Or":
                    ErrorOverlayImageSourceCannotContainAOr = Value;
                    break;

                case "File":
                    File = Value;
                    break;

                case "Find A Location":
                    FindALocation = Value;
                    break;

                case "Find Address":
                    FindAddress = Value;
                    break;

                case "Find Location":
                    FindLocation = Value;
                    break;

                case "Finding Your Location":
                    FindingYourLocation = Value;
                    break;

                case "Geocoder Failed Due To":
                    GeocoderFailedDueTo = Value;
                    break;

                case "Geolocation Service Failed":
                    GeolocationServiceFailed = Value;
                    break;

                case "Help":
                    Help = Value;
                    break;

                case "Hide Coordinates":
                    HideCoordinates = Value;
                    break;

                case "Hiding":
                    Hiding = Value;
                    break;

                case "Hiding Overlays":
                    HidingOverlays = Value;
                    break;

                case "Hiding Pois":
                    HidingPois = Value;
                    break;

                case "Hybrid":
                    Hybrid = Value;
                    break;

                case "Hybrid Toggle Hybrid Map Layer":
                    HybridToggleHybridMapLayer = Value;
                    break;

                case "In Zoom Map In":
                    InZoomMapIn = Value;
                    break;

                case "Item Geographic Location Deleted":
                    ItemGeographicLocationDeleted = Value;
                    break;

                case "Item Location Converted To Listing Overlays":
                    ItemLocationConvertedToListingOverlays = Value;
                    break;

                case "Item Relocation Reset":
                    ItemRelocationReset = Value;
                    break;

                case "Item Saved":
                    ItemSaved = Value;
                    break;

                case "Latitude":
                    Latitude = Value;
                    break;

                case "Layers":
                    Layers = Value;
                    break;

                case "Left Pan Map Left":
                    LeftPanMapLeft = Value;
                    break;

                case "Line":
                    Line = Value;
                    break;

                case "Line Place A Line":
                    LinePlaceALine = Value;
                    break;

                case "Locate":
                    Locate = Value;
                    break;

                case "Locate Find A Location On The Map":
                    LocateFindALocationOnTheMap = Value;
                    break;

                case "Longitude":
                    Longitude = Value;
                    break;

                case "Manage Location":
                    ManageLocation = Value;
                    break;

                case "Manage Location Details":
                    ManageLocationDetails = Value;
                    break;

                case "Manage Map Coverage":
                    ManageMapCoverage = Value;
                    break;

                case "Manage Overlay":
                    ManageOverlay = Value;
                    break;

                case "Manage Overlays":
                    ManageOverlays = Value;
                    break;

                case "Manage POI":
                    ManagePOI = Value;
                    break;

                case "Manage Points Of Interest":
                    ManagePointsOfInterest = Value;
                    break;

                case "Manage Pois":
                    ManagePois = Value;
                    break;

                case "Map Controls":
                    MapControls = Value;
                    break;

                case "Marker":
                    Marker = Value;
                    break;

                case "Marker Place A Point":
                    MarkerPlaceAPoint = Value;
                    break;

                case "More Help":
                    MoreHelp = Value;
                    break;

                case "Nearest Address":
                    NearestAddress = Value;
                    break;

                case "Not Cleared":
                    NotCleared = Value;
                    break;

                case "Nothing Happened":
                    NothingHappened = Value;
                    break;

                case "Nothing To Delete":
                    NothingToDelete = Value;
                    break;

                case "Nothing To Hide":
                    NothingToHide = Value;
                    break;

                case "Nothing To Reset":
                    NothingToReset = Value;
                    break;

                case "Nothing To Save":
                    NothingToSave = Value;
                    break;

                case "Nothing To Search":
                    NothingToSearch = Value;
                    break;

                case "Nothing To Toggle":
                    NothingToToggle = Value;
                    break;

                case "Out Zoom Map Out":
                    OutZoomMapOut = Value;
                    break;

                case "Overall Geographic Data Deleted":
                    OverallGeographicDataDeleted = Value;
                    break;

                case "Overlay Editing Turned Off":
                    OverlayEditingTurnedOff = Value;
                    break;

                case "Overlay Editing Turned On":
                    OverlayEditingTurnedOn = Value;
                    break;

                case "Overlay Saved":
                    OverlaySaved = Value;
                    break;

                case "Overlays Reset":
                    OverlaysReset = Value;
                    break;

                case "Pan":
                    Pan = Value;
                    break;

                case "Place":
                    Place = Value;
                    break;

                case "POI Set Cleared":
                    POISetCleared = Value;
                    break;

                case "POI Set Saved":
                    POISetSaved = Value;
                    break;

                case "Point 1":
                    Point1 = Value;
                    break;

                case "Point 2":
                    Point2 = Value;
                    break;

                case "Polygon":
                    Polygon = Value;
                    break;

                case "Polygon Place A Polygon":
                    PolygonPlaceAPolygon = Value;
                    break;

                case "Rectangle":
                    Rectangle = Value;
                    break;

                case "Rectangle Place A Rectangle":
                    RectanglePlaceARectangle = Value;
                    break;

                case "Removed":
                    Removed = Value;
                    break;

                case "Report A Problem":
                    ReportAProblem = Value;
                    break;

                case "Report A Sobek Error":
                    ReportASobekError = Value;
                    break;

                case "Reset":
                    Reset = Value;
                    break;

                case "Reset All Changes":
                    ResetAllChanges = Value;
                    break;

                case "Reset All Overlay Changes":
                    ResetAllOverlayChanges = Value;
                    break;

                case "Reset Location":
                    ResetLocation = Value;
                    break;

                case "Reset Location Changes":
                    ResetLocationChanges = Value;
                    break;

                case "Reset Me":
                    ResetMe = Value;
                    break;

                case "Reset Overlays":
                    ResetOverlays = Value;
                    break;

                case "Reset Pois":
                    ResetPois = Value;
                    break;

                case "Reset Click To Reset Rotation":
                    ResetClickToResetRotation = Value;
                    break;

                case "Reset Reset Map To Defaults":
                    ResetResetMapToDefaults = Value;
                    break;

                case "Reset Reset Map Type":
                    ResetResetMapType = Value;
                    break;

                case "Reset Reset Zoom Level":
                    ResetResetZoomLevel = Value;
                    break;

                case "Reseting Page":
                    ResetingPage = Value;
                    break;

                case "Resetting Overlays":
                    ResettingOverlays = Value;
                    break;

                case "Resetting Pois":
                    ResettingPois = Value;
                    break;

                case "Returned To Bounds":
                    ReturnedToBounds = Value;
                    break;

                case "Right Pan Map Right":
                    RightPanMapRight = Value;
                    break;

                case "Roadmap":
                    Roadmap = Value;
                    break;

                case "Roadmap Toggle Road Map Layer":
                    RoadmapToggleRoadMapLayer = Value;
                    break;

                case "Rotate":
                    Rotate = Value;
                    break;

                case "Rotate Edit The Rotation Value":
                    RotateEditTheRotationValue = Value;
                    break;

                case "Satellite":
                    Satellite = Value;
                    break;

                case "Satellite Toggle Satellite Map Layer":
                    SatelliteToggleSatelliteMapLayer = Value;
                    break;

                case "Save":
                    Save = Value;
                    break;

                case "Save Description":
                    SaveDescription = Value;
                    break;

                case "Save Location Changes":
                    SaveLocationChanges = Value;
                    break;

                case "Save Overlay Changes":
                    SaveOverlayChanges = Value;
                    break;

                case "Save Point Of Interest Set":
                    SavePointOfInterestSet = Value;
                    break;

                case "Save This Description":
                    SaveThisDescription = Value;
                    break;

                case "Save To Temporary File":
                    SaveToTemporaryFile = Value;
                    break;

                case "Saved":
                    Saved = Value;
                    break;

                case "Saving":
                    Saving = Value;
                    break;

                case "Search Coordinates":
                    SearchCoordinates = Value;
                    break;

                case "Select Area":
                    SelectArea = Value;
                    break;

                case "Select Point":
                    SelectPoint = Value;
                    break;

                case "Select The Area To Draw The Overlay":
                    SelectTheAreaToDrawTheOverlay = Value;
                    break;

                case "Selected Latlong":
                    SelectedLatlong = Value;
                    break;

                case "Show Coordinates":
                    ShowCoordinates = Value;
                    break;

                case "Showing":
                    Showing = Value;
                    break;

                case "Showing Overlays":
                    ShowingOverlays = Value;
                    break;

                case "Showing Pois":
                    ShowingPois = Value;
                    break;

                case "Tenth Degree Left Click To Rotate A Tenth Degree Counterclockwise":
                    TenthDegreeLeftClickToRotateATenthDegreeCounterclockwise = Value;
                    break;

                case "Tenth Degree Right Click To Rotate A Tenth Degree Clockwise":
                    TenthDegreeRightClickToRotateATenthDegreeClockwise = Value;
                    break;

                case "Terrain":
                    Terrain = Value;
                    break;

                case "Terrain Toggle Terrain Map Layer":
                    TerrainToggleTerrainMapLayer = Value;
                    break;

                case "This Will Delete All Of The Pois Are You Sure":
                    ThisWillDeleteAllOfThePoisAreYouSure = Value;
                    break;

                case "This Will Delete The Geogarphic Coodinate Data For This Item Are You Sure":
                    ThisWillDeleteTheGeogarphicCoodinateDataForThisItemAreYouSure = Value;
                    break;

                case "This Will Delete The Geographic Coordinate Data For This Overlay Are You Sure":
                    ThisWillDeleteTheGeographicCoordinateDataForThisOverlayAreYouSure = Value;
                    break;

                case "Toggle All Map Overlays":
                    ToggleAllMapOverlays = Value;
                    break;

                case "Toggle All Overlays On Map":
                    ToggleAllOverlaysOnMap = Value;
                    break;

                case "Toggle All Pois On Map":
                    ToggleAllPoisOnMap = Value;
                    break;

                case "Toggle On Map":
                    ToggleOnMap = Value;
                    break;

                case "Toggle Pois On Map":
                    TogglePoisOnMap = Value;
                    break;

                case "Toolbar":
                    Toolbar = Value;
                    break;

                case "Toolbar Toggle The Toolbar":
                    ToolbarToggleTheToolbar = Value;
                    break;

                case "Toolbox":
                    Toolbox = Value;
                    break;

                case "Toolbox Toggle Toolbox":
                    ToolboxToggleToolbox = Value;
                    break;

                case "Transparency":
                    Transparency = Value;
                    break;

                case "Transparency Set The Transparency Of This Overlay":
                    TransparencySetTheTransparencyOfThisOverlay = Value;
                    break;

                case "Up Pan Map Up":
                    UpPanMapUp = Value;
                    break;

                case "Use Search Result As Location":
                    UseSearchResultAsLocation = Value;
                    break;

                case "Using Search Results As Location":
                    UsingSearchResultsAsLocation = Value;
                    break;

                case "View":
                    View = Value;
                    break;

                case "Warning This Will Erase Any Changes You Have Made Do You Still Want To Proceed":
                    WarningThisWillEraseAnyChangesYouHaveMadeDoYouStillWantToProceed = Value;
                    break;

                case "Working":
                    Working = Value;
                    break;

                case "Zoom":
                    Zoom = Value;
                    break;

                case "Zoom In":
                    ZoomIn = Value;
                    break;

                case "Zoom Out":
                    ZoomOut = Value;
                    break;

            }
        }
        /// <remarks> Instructions for map searching when only SELECT AREA is available </remarks>
        public string UseTheSelectAreaButtonAndClickToSelectOppositeCornersToDrawASearchBoxOnTheMap { get; private set; }

        /// <remarks> Instructions for map searching with area and point searching </remarks>
        public string UseOneOfTheMethodsBelowToDefineYourGeographicSearch { get; private set; }

        /// <remarks> Instructions for map searching when only SELECT AREA is available </remarks>
        public string PressTheSearchButtonToSeeResults { get; private set; }

        /// <remarks> Instructions for map searching with area and point searching </remarks>
        public string AEnterAnAddressAndPressFindAddressToLocateOr { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string Actions { get; private set; }

        /// <remarks> For map searching </remarks>
        public string Address { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string AddressThisIsTheNearestAddressOfThePointYouSelected { get; private set; }

        /// <remarks> 'Applied' localization string </remarks>
        public string Applied { get; private set; }

        /// <remarks> 'Apply' localization string </remarks>
        public string Apply { get; private set; }

        /// <remarks> 'Apply Changes (Make Changes Public)' localization string </remarks>
        public string ApplyChangesMakeChangesPublic { get; private set; }

        /// <remarks> Instructions for map searching with area and point searching </remarks>
        public string BPressTheSelectAreaButtonAndClickToSelectTwoOppositeCornersOr { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string BlocklotToggleBlocklotMapLayer { get; private set; }

        /// <remarks> Instructions for map searching with area and point searching </remarks>
        public string CPressTheSelectPointButtonAndClickToSelectASinglePoint { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string CancelEditing { get; private set; }

        /// <remarks> 'Canceling...' localization string </remarks>
        public string Canceling { get; private set; }

        /// <remarks> 'Cannot Convert' localization string </remarks>
        public string CannotConvert { get; private set; }

        /// <remarks> 'Cannot Zoom in further' localization string </remarks>
        public string CannotZoomInFurther { get; private set; }

        /// <remarks> 'Cannot Zoom out Further' localization string </remarks>
        public string CannotZoomOutFurther { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string CenterOnCurrentLocation { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string CenterOnYourCurrentPosition { get; private set; }

        /// <remarks> 'Circle' localization string </remarks>
        public string Circle { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string CirclePlaceACircle { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string ClearPointOfInterestSet { get; private set; }

        /// <remarks> Reveals instructions for map searching </remarks>
        public string ClickHereToViewInstructionsForThisSearchInterface { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string Clockwise { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string CompleteEditing { get; private set; }

        /// <remarks> 'Completed' localization string </remarks>
        public string Completed { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string Controls { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string ControlsToggleMapControls { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string ConvertThisToAMapOverlay { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string ConvertToOverlay { get; private set; }

        /// <remarks> 'Converted Item to Overlay' localization string </remarks>
        public string ConvertedItemToOverlay { get; private set; }

        /// <remarks> 'Coordinate Data removed for' localization string </remarks>
        public string CoordinateDataRemovedFor { get; private set; }

        /// <remarks> 'Coordinates Copied to Clipboard' localization string </remarks>
        public string CoordinatesCopiedToClipboard { get; private set; }

        /// <remarks> 'Coordinates Viewer Frozen' localization string </remarks>
        public string CoordinatesViewerFrozen { get; private set; }

        /// <remarks> 'Coordinates Viewer Unfrozen' localization string </remarks>
        public string CoordinatesViewerUnfrozen { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string CoordinatesThisIsTheSelectedLatitudeAndLongitudeOfThePointYouSelected { get; private set; }

        /// <remarks> 'Could not find location. Either the format you entered is invalid or the location is outside of the map bounds.' localization string </remarks>
        public string CouldNotFindLocationEitherTheFormatYouEnteredIsInvalidOrTheLocationIsOutsideOfTheMapBounds { get; private set; }

        /// <remarks> 'Could not find within bounds' localization string </remarks>
        public string CouldNotFindWithinBounds { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string Counterclockwise { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string Custom { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string DefaultPanMapToDefault { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string Delete { get; private set; }

        /// <remarks> 'Delete Coordinate Data for Overlay' localization string </remarks>
        public string DeleteCoordinateDataForOverlay { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string DeleteGeographicLocation { get; private set; }

        /// <remarks> 'Delete POI' localization string </remarks>
        public string DeletePOI { get; private set; }

        /// <remarks> 'Delete Search Result' localization string </remarks>
        public string DeleteSearchResult { get; private set; }

        /// <remarks> 'Deleted' localization string </remarks>
        public string Deleted { get; private set; }

        /// <remarks> 'Description (Optional)' localization string </remarks>
        public string DescriptionOptional { get; private set; }

        /// <remarks> 'Did not Reset Page' localization string </remarks>
        public string DidNotResetPage { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string Documentation { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string DownPanMapDown { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string EditLocation { get; private set; }

        /// <remarks> 'Edit Location By Dragging Existing Marker' localization string </remarks>
        public string EditLocationByDraggingExistingMarker { get; private set; }

        /// <remarks> 'Edit this Overlay' localization string </remarks>
        public string EditThisOverlay { get; private set; }

        /// <remarks> 'Edit this POI' localization string </remarks>
        public string EditThisPOI { get; private set; }

        /// <remarks> 'Editing' localization string </remarks>
        public string Editing { get; private set; }

        /// <remarks> Prompt for finding by address. </remarks>
        public string EnterAddressIe12MainStreetGainesvilleFlorida { get; private set; }

        /// <remarks> 'Error Addign other Listeners' localization string </remarks>
        public string ErrorAddignOtherListeners { get; private set; }

        /// <remarks> 'ERROR Failed Adding Textual Content' localization string </remarks>
        public string ERRORFailedAddingTextualContent { get; private set; }

        /// <remarks> 'ERROR Failed Adding Titles' localization string </remarks>
        public string ERRORFailedAddingTitles { get; private set; }

        /// <remarks> 'Error: Description cannot contain a ~ or |' localization string </remarks>
        public string ErrorDescriptionCannotContainAOr { get; private set; }

        /// <remarks> 'Error: Overlay image source cannot contain a ~ or |' localization string </remarks>
        public string ErrorOverlayImageSourceCannotContainAOr { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string File { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string FindALocation { get; private set; }

        /// <remarks> For map searching </remarks>
        public string FindAddress { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string FindLocation { get; private set; }

        /// <remarks> 'Finding your location' localization string </remarks>
        public string FindingYourLocation { get; private set; }

        /// <remarks> 'geocoder failed due to:' localization string </remarks>
        public string GeocoderFailedDueTo { get; private set; }

        /// <remarks> 'Geolocation Service Failed.' localization string </remarks>
        public string GeolocationServiceFailed { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string Help { get; private set; }

        /// <remarks> 'Hide Coordinates' localization string </remarks>
        public string HideCoordinates { get; private set; }

        /// <remarks> 'Hiding' localization string </remarks>
        public string Hiding { get; private set; }

        /// <remarks> 'Hiding Overlays' localization string </remarks>
        public string HidingOverlays { get; private set; }

        /// <remarks> 'Hiding POIs' localization string </remarks>
        public string HidingPois { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string Hybrid { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string HybridToggleHybridMapLayer { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string InZoomMapIn { get; private set; }

        /// <remarks> 'Item Geographic Location Deleted' localization string </remarks>
        public string ItemGeographicLocationDeleted { get; private set; }

        /// <remarks> 'Item Location Converted to Listing Overlays' localization string </remarks>
        public string ItemLocationConvertedToListingOverlays { get; private set; }

        /// <remarks> 'Item Relocation Reset!' localization string </remarks>
        public string ItemRelocationReset { get; private set; }

        /// <remarks> 'Item Saved!' localization string </remarks>
        public string ItemSaved { get; private set; }

        /// <remarks> 'Latitude' localization string </remarks>
        public string Latitude { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string Layers { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string LeftPanMapLeft { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string Line { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string LinePlaceALine { get; private set; }

        /// <remarks> For map searching </remarks>
        public string Locate { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string LocateFindALocationOnTheMap { get; private set; }

        /// <remarks> 'Longitude' localization string </remarks>
        public string Longitude { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string ManageLocation { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string ManageLocationDetails { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string ManageMapCoverage { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string ManageOverlay { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string ManageOverlays { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string ManagePOI { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string ManagePointsOfInterest { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string ManagePois { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string MapControls { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string Marker { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string MarkerPlaceAPoint { get; private set; }

        /// <remarks> For more instructions </remarks>
        public string MoreHelp { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string NearestAddress { get; private set; }

        /// <remarks> 'Not Cleared' localization string </remarks>
        public string NotCleared { get; private set; }

        /// <remarks> 'Nothing Happened!' localization string </remarks>
        public string NothingHappened { get; private set; }

        /// <remarks> 'Nothing to Delete' localization string </remarks>
        public string NothingToDelete { get; private set; }

        /// <remarks> 'Nothing to Hide' localization string </remarks>
        public string NothingToHide { get; private set; }

        /// <remarks> 'Nothing to Reset' localization string </remarks>
        public string NothingToReset { get; private set; }

        /// <remarks> 'Nothing to Save' localization string </remarks>
        public string NothingToSave { get; private set; }

        /// <remarks> 'Nothing to Search' localization string </remarks>
        public string NothingToSearch { get; private set; }

        /// <remarks> 'Nothing to Toggle' localization string </remarks>
        public string NothingToToggle { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string OutZoomMapOut { get; private set; }

        /// <remarks> 'Overall Geographic Data Deleted' localization string </remarks>
        public string OverallGeographicDataDeleted { get; private set; }

        /// <remarks> 'Overlay Editing Turned Off' localization string </remarks>
        public string OverlayEditingTurnedOff { get; private set; }

        /// <remarks> 'Overlay Editing Turned On' localization string </remarks>
        public string OverlayEditingTurnedOn { get; private set; }

        /// <remarks> 'Overlay Saved!' localization string </remarks>
        public string OverlaySaved { get; private set; }

        /// <remarks> 'Overlays Reset!' localization string </remarks>
        public string OverlaysReset { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string Pan { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string Place { get; private set; }

        /// <remarks> 'POI Set Cleared!' localization string </remarks>
        public string POISetCleared { get; private set; }

        /// <remarks> 'POI Set Saved!' localization string </remarks>
        public string POISetSaved { get; private set; }

        /// <remarks> 'Point 1' localization string </remarks>
        public string Point1 { get; private set; }

        /// <remarks> 'Point 2' localization string </remarks>
        public string Point2 { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string Polygon { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string PolygonPlaceAPolygon { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string Rectangle { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string RectanglePlaceARectangle { get; private set; }

        /// <remarks> 'Removed' localization string </remarks>
        public string Removed { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string ReportAProblem { get; private set; }

        /// <remarks> 'Report a Sobek Error' localization string </remarks>
        public string ReportASobekError { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string Reset { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string ResetAllChanges { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string ResetAllOverlayChanges { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string ResetLocation { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string ResetLocationChanges { get; private set; }

        /// <remarks> 'Reset Me' localization string </remarks>
        public string ResetMe { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string ResetOverlays { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string ResetPois { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string ResetClickToResetRotation { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string ResetResetMapToDefaults { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string ResetResetMapType { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string ResetResetZoomLevel { get; private set; }

        /// <remarks> 'Reseting Page' localization string </remarks>
        public string ResetingPage { get; private set; }

        /// <remarks> 'Resetting Overlays' localization string </remarks>
        public string ResettingOverlays { get; private set; }

        /// <remarks> 'Resetting POIs' localization string </remarks>
        public string ResettingPois { get; private set; }

        /// <remarks> 'Returned to Bounds!' localization string </remarks>
        public string ReturnedToBounds { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string RightPanMapRight { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string Roadmap { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string RoadmapToggleRoadMapLayer { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string Rotate { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string RotateEditTheRotationValue { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string Satellite { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string SatelliteToggleSatelliteMapLayer { get; private set; }

        /// <remarks> 'Save' localization string </remarks>
        public string Save { get; private set; }

        /// <remarks> 'Save description' localization string </remarks>
        public string SaveDescription { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string SaveLocationChanges { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string SaveOverlayChanges { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string SavePointOfInterestSet { get; private set; }

        /// <remarks> 'save this Description' localization string </remarks>
        public string SaveThisDescription { get; private set; }

        /// <remarks> 'Save to Temporary File' localization string </remarks>
        public string SaveToTemporaryFile { get; private set; }

        /// <remarks> 'Saved' localization string </remarks>
        public string Saved { get; private set; }

        /// <remarks> 'Saving...' localization string </remarks>
        public string Saving { get; private set; }

        /// <remarks> When user has selected to show the coordinates selected </remarks>
        public string SearchCoordinates { get; private set; }

        /// <remarks> For map searching </remarks>
        public string SelectArea { get; private set; }

        /// <remarks> For map searching </remarks>
        public string SelectPoint { get; private set; }

        /// <remarks> 'Select the area to Draw the Overlay' localization string </remarks>
        public string SelectTheAreaToDrawTheOverlay { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string SelectedLatlong { get; private set; }

        /// <remarks> 'Show Coordinates' localization string </remarks>
        public string ShowCoordinates { get; private set; }

        /// <remarks> 'Showing' localization string </remarks>
        public string Showing { get; private set; }

        /// <remarks> 'Showing Overlays' localization string </remarks>
        public string ShowingOverlays { get; private set; }

        /// <remarks> 'Showing POIs' localization string </remarks>
        public string ShowingPois { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string TenthDegreeLeftClickToRotateATenthDegreeCounterclockwise { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string TenthDegreeRightClickToRotateATenthDegreeClockwise { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string Terrain { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string TerrainToggleTerrainMapLayer { get; private set; }

        /// <remarks> '"This will delete all of the POIs, are you sure?"' localization string </remarks>
        public string ThisWillDeleteAllOfThePoisAreYouSure { get; private set; }

        /// <remarks> '"This will delete the geogarphic coodinate data for this item, are you sure?"' localization string </remarks>
        public string ThisWillDeleteTheGeogarphicCoodinateDataForThisItemAreYouSure { get; private set; }

        /// <remarks> '"This will delete the geographic coordinate data for this overlay, are you sure?"' localization string </remarks>
        public string ThisWillDeleteTheGeographicCoordinateDataForThisOverlayAreYouSure { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string ToggleAllMapOverlays { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string ToggleAllOverlaysOnMap { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string ToggleAllPoisOnMap { get; private set; }

        /// <remarks> 'Toggle on Map' localization string </remarks>
        public string ToggleOnMap { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string TogglePoisOnMap { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string Toolbar { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string ToolbarToggleTheToolbar { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string Toolbox { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string ToolboxToggleToolbox { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string Transparency { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string TransparencySetTheTransparencyOfThisOverlay { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string UpPanMapUp { get; private set; }

        /// <remarks> Tooltips set from the sobekcm_map_editor.js file </remarks>
        public string UseSearchResultAsLocation { get; private set; }

        /// <remarks> 'Using Search Results as Location' localization string </remarks>
        public string UsingSearchResultsAsLocation { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string View { get; private set; }

        /// <remarks> 'Warning! This will erase any changes you have made. Do you still want to proceed?' localization string </remarks>
        public string WarningThisWillEraseAnyChangesYouHaveMadeDoYouStillWantToProceed { get; private set; }

        /// <remarks> 'Working...' localization string </remarks>
        public string Working { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string Zoom { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string ZoomIn { get; private set; }

        /// <remarks> Content menubar innerHTML added from the JS file </remarks>
        public string ZoomOut { get; private set; }

    }
}
