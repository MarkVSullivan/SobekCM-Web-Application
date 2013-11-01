/*
This file loads all of the custom javascript libraries needed to run the mapedit portion of sobek.
*/

//Declarations

//#region Declarations

//must remain outside fcns at top level
var globalVar; //holds global vars
var localize; //holds localization stuff

//other global vars
//todo move into init object (it gets dicey)
CustomOverlay.prototype = new google.maps.OverlayView(); //used to display custom overlay
var KmlLayer;               //must be pingable by google
var geocoder;               //must define before use
var label = [];             //used as label of poi
var infoWindow = [];        //poi infowindow

//call declarations init fcn
//todo move into a document onload listener (TEMP)
initDeclarations();

//init declarations
function initDeclarations() {

    //init global object    
    globalVar = function () {
        //private

        //public
        return {
            //public vars

            //init global vars
            //global defines (do not change here)
            toServerSuccess: false,                     //holds a marker indicating if toserver was sucessfull
            tempYo: false,                              //holds tempyo for fixing ff info window issue
            buttonActive_searchResultToggle: false,     //holds is button active markers
            buttonActive_itemPlace: false,              //holds is button active markers
            //buttonActive_overlayEdit: false,            //not currently used
            //buttonActive_overlayPlace: false,           //not currently used
            buttonActive_overlayToggle: false,          //holds is button active markers
            buttonActive_poiPlace: false,               //not currently used
            buttonActive_poiToggle: false,              //holds is button active markers
            buttonActive_poiMarker: false,              //not currently used
            buttonActive_poiCircle: false,              //not currently used
            buttonActive_poiRectangle: false,           //not currently used
            buttonActive_poiPolygon: false,             //not currently used
            buttonActive_poiLine: false,                //not currently used
            userMayLoseData: false,                     //holds a marker to determine if signifigant changes have been made to require a save
            baseURL: null,                              //holds place for server written vars
            defaultOpacity: 0.5,                        //holds default opacity settings
            isConvertedOverlay: false,                  //holds a marker for converted overlay
            RIBMode: false,                             //holds a marker for running in background mode (do not display messages)
            debugMode: null,                            //holds debug marker
            hasCustomMapType: null,                     //holds marker for determining if there is a custom map type
            messageCount: 0,                            //holds the running count of all the messages written in a session
            poiToggleState: "displayed",                //holds marker for displayed/hidden pois
            poiCount: 0,                                //holds a marker for pois drawn (fixes first poi desc issue)
            firstSaveItem: null,                        //holds first save marker (used to determine if saving or applying changes)
            firstSaveOverlay: null,                     //holds first save marker (used to determine if saving or applying changes)
            firstSavePOI: null,                         //holds first save marker (used to determine if saving or applying changes)
            baseImageDirURL: null,                      //holds the base image directory url
            mapDrawingManagerDisplayed: null,           //holds marker for drawing manager
            mapLayerActive: null,                       //holds the current map layer active
            prevMapLayerActive: null,                   //holds the previous active map layer
            actionActive: null,                         //holds the current active action
            prevActionActive: null,                     //holds the previous active action
            overlaysCurrentlyDisplayed: null,           //holds marker for overlays on map
            pageMode: null,                             //holds the page/viewer type
            mapCenter: null,                            //used to center map on load
            mapControlsDisplayed: null,                 //by default, are map controls displayed (true/false)
            defaultDisplayDrawingMangerTool: null,      //by default, is the drawingmanger displayed (true/false)
            toolboxDisplayed: null,                     //by default, is the toolbox displayed (true/false)
            toolbarDisplayed: null,                     //by default, is the toolbar open (yes/no)
            kmlDisplayed: null,                         //by default, is kml layer on (yes/no)
            defaultZoomLevel: null,                     //zoom level, starting
            maxZoomLevel: null,                         //max zoom out, default (21:lowest level, 1:highest level)
            minZoomLevel_Terrain: null,                 //max zoom in, terrain 
            minZoomLevel_Satellite: null,               //max zoom in, sat + hybrid
            minZoomLevel_Roadmap: null,                 //max zoom in, roadmap (default)
            minZoomLevel_BlockLot: null,                //max zoom in, used for special layers not having default of roadmap
            isCustomOverlay: null,                      //used to determine if other overlays (block/lot etc) //used in testbounds unknown if needed
            preservedRotation: null,                    //rotation, default
            knobRotationValue: null,                    //rotation to display by default 
            preservedOpacity: null,                     //opacity, default value (0-1,1:opaque)
            overlaysOnMap: [],                          //holds all overlays
            csoi: 0,                                    //hold current saved overlay index
            pendingOverlaySave: false,                  //hold the marker to indicate if we need to save the overlay (this prevents a save if we already saved)
            oomCount: 0,                                //counts how many overlays are on the map
            searchCount: 0,                             //interates how many searches
            degree: 0,                                  //initializing degree
            firstMarker: 0,                             //used to iterate if marker placement was the first (to prevent duplicates)
            overlayCount: 0,                            //iterater (contains how many overlays are not deleted)
            mapInBounds: null,                          //is the map in bounds
            searchResult: null,                         //will contain object
            circleCenter: null,                         //hold center point of circle
            markerCenter: null,                         //hold center of marker
            placerType: null,                           //type of data (marker,overlay,poi)
            poiType: [],                                //typle of poi (marker, circle, rectangle, polygon, polyline)
            poiKML: [],                                 //pou kml layer (or other geographic info)
            poi_i: -1,                                  //increment the poi count (used to make IDs and such)
            poiObj: [],                                 //poi object placholder
            poiCoord: [],                               //poi coord placeholder
            poiDesc: [],                                //desc poi placeholder
            globalPolyline: null,                       //unknown
            rectangle: null,                            //must define before use
            firstDraw: 0,                               //used to increment first drawing of rectangle
            getCoord: null,                             //used to store coords from marker
            itemMarker: null,                           //hold current item marker
            savingMarkerCenter: null,                   //holds marker coords to save
            CustomOverlay: null,                        //does nothing
            cCoordsFrozen: "no",                        //used to freeze/unfreeze coordinate viewer
            mainCount: 0,                               //hold debug main count
            incomingACL: "item",                        //hold default incoming ACL (determined in displaying points/overlays)
            overlayType: null,                          //draw = overlay was drawn
            incomingPointSourceURLConverted: null,      //holds convert 
            incomingPointLabelConverted: null,          //holds convert 
            incomingLineFeatureType: [],                //defined in c# to js on page
            incomingLineLabel: [],                      //defined in c# to js on page
            incomingLinePath: [],                       //defined in c# to js on page
            incomingCircleCenter: [],                   //defined in c# to js on page
            incomingCircleRadius: [],                   //defined in c# to js on page
            incomingCircleFeatureType: [],              //defined in c# to js on page
            incomingCircleLabel: [],                    //defined in c# to js on page
            incomingPointFeatureType: [],               //defined in c# to js on page
            incomingPointCenter: [],                    //defined in c# to js on page
            incomingPointLabel: [],                     //defined in c# to js on page
            incomingPointSourceURL: [],                 //defined in c# to js on page
            incomingPolygonPageId: [],                  //defined in c# to js on page
            incomingPolygonFeatureType: [],             //defined in c# to js on page
            incomingPolygonPolygonType: [],             //defined in c# to js on page
            incomingPolygonBounds: [],                  //defined in c# to js on page
            incomingPolygonPath: [],                    //defined in c# to js on page
            incomingPolygonCenter: [],                  //defined in c# to js on page
            incomingPolygonLabel: [],                   //defined in c# to js on page
            incomingPolygonSourceURL: [],               //defined in c# to js on page
            incomingPolygonRotation: [],                //defined in c# to js on page
            incomingPolygonRadius: [],                  //defined in c# to js on page
            ghostOverlayRectangle: [],                  //holds ghost overlay rectangles (IE overlay hotspots)
            convertedOverlayIndex: 0,                   //holds the place for indexing a converted overlay
            workingOverlayIndex: null,                  //holds the index of the overlay we are working with (and saving)
            currentlyEditing: "no",                     //tells us if we are editing anything
            currentTopZIndex: 5,                        //current top zindex (used in displaying overlays over overlays)
            savingOverlayIndex: [],                     //holds index of the overlay we are saving
            savingOverlayLabel: [],                     //holds label of the overlay we are saving
            savingOverlaySourceURL: [],                 //hold the source url of the overlay to save
            savingOverlayBounds: [],                    //holds bounds of the overlay we are saving
            savingOverlayRotation: [],                  //holds rotation of the overlay we are saving
            savingOverlayPageId: [],                    //holds page ID of saving overlay
            strictBounds: null,                         //holds the strict bounds
            ghosting: {                                 //define options for globalVar.ghosting (IE being invisible)
                strokeOpacity: 0.0,                     //make border invisible
                fillOpacity: 0.0,                       //make fill transparent
                editable: false,                        //sobek standard
                draggable: false,                       //sobek standard
                clickable: false,                       //sobek standard
                zindex: 5                               //perhaps higher?
            },
            editable: {                                 //define options for visible and globalVar.editable
                editable: true,                         //sobek standard
                draggable: true,                        //sobek standard
                clickable: true,                        //sobek standard
                strokeOpacity: 0.2,                     //sobek standard
                strokeWeight: 1,                        //sobek standard
                fillOpacity: 0.0,                       //sobek standard 
                zindex: 5                               //sobek standard
            }//,
            ////public methods
            //addListeners: function () {
            //    //alert(globalVar.test1);
            //},
            //add: function () {
            //    //globalVar.test2++;
            //    //alert(globalVar.test2);
            //}
        };
    }();
    //get and set c# vars to js  
    initServerToClientVars();
}

//#endregion


//Localization Support

//#region localization support

//#region localization by variables (used in fcns)

//todo move to localize object
var Lerror1 = "ERROR: Failed Adding Listeners";
var L_Marker = "Marker";
var L_Circle = "Circle";
var L_Rectangle = "Rectangle";
var L_Polygon = "Polygon";
var L_Line = "Line";
var L_Saved = "Saved";
var L_Applied = "Applied";
var L_NotSaved = "Nothing To Save";
var L_NotCleared = "Nothing to Reset";
var L_Save = "Save";
var L_Apply = "Apply";
var L_Editing = "Editing";
var L_Removed = "Removed";
var L_Showing = "Showing";
var L_Hiding = "Hiding";
var L1 = "<div style=\"font-size:.95em;\">SobekCM Plugin &nbsp&nbsp&nbsp <a href=\"#\" style=\"font-size:9px;text-decoration:none;\">Legal</a> &nbsp&nbsp&nbsp <a href=\"#\" style=\"font-size:9px;text-decoration:none;\">Report a Sobek error</a> &nbsp</div>"; //copyright node
var L2 = "lat: <a id=\"cLat\"></a><br/>long: <a id=\"cLong\"></a>"; //lat long of cursor position tool
var L3 = "Description (Optional)"; //describe poi box
var L4 = "Geolocation Service Failed."; //geolocation buttons error message
var L5 = "Returned to Bounds!"; //tesbounds();
var L6 = "Could not find location. Either the format you entered is invalid or the location is outside of the map bounds."; //codeAddress();
var L7 = "Error: Overlay image source cannot contain a ~ or |"; //createSavedOverlay();
var L8 = "Error: Description cannot contain a ~ or |"; //poiGetDesc(id);
var L9 = "Item Location Reset!"; //buttonClearItem();
var L10 = "Overlays Reset!"; //buttonClearOverlay();
var L11 = "POI Set Cleared!"; //buttonClearPOI();
var L12 = "Nothing Happened!"; //HandleResult(arg);
var L13 = "Item Saved!"; //HandleResult(arg);
var L14 = "Overlay Saved!"; //HandleResult(arg);
var L15 = "POI Set Saved!"; //HandleResult(arg);
var L16 = "Cannot Zoom Out Further"; //checkZoomLevel();
var L17 = "Cannot Zoom In Further"; //checkZoomLevel();
var L18 = "Using Search Results as Location"; //marker complete listener
var L19 = "Coordinates Copied To Clipboard"; //keypress(e);
var L20 = "Coordinates Viewer Frozen"; //keypress(e);
var L21 = "Coordinates Viewer UnFrozen"; //keypress(e);
var L22 = "Hiding Overlays"; //keypress(e);
var L23 = "Showing Overlays"; //keypress(e);
var L24 = "Could not find within bounds.";
var L25 = "geocoder failed due to:";
var L26 = "Overlay Editing Turned Off";
var L27 = "Overlay Editing Turned On";
var L28 = "ERROR: Failed Adding Titles";
var L29 = "ERROR: Failed Adding Textual Content";
var L30 = "Edit Location by Dragging Exisiting Marker";
var L31 = L_Hiding;
var L32 = L_Showing;
var L33 = L_Removed;
var L34 = L_Editing;
var L35 = "Apply Changes (Make Changes Public)";
var L36 = L_Apply;
var L37 = L_Save;
var L38 = "Save to Temporary File";
var L39 = "Nothing To Search";
var L40 = "Cannot Convert";
var L41 = "Select The Area To Draw The Overlay";
var L42 = "Hiding POIs"; //toogle poi
var L43 = "Showing POIs"; //toggle poi
var L44 = "Converted Item To Overlay";
var L45 = "Nothing To Toggle";
var L46 = L_NotCleared;
var L47 = "Warning! This will erase any changes you have made. Do you still want to proceed?";
var L48 = "Reseting Page";
var L49 = "Did Not Reset Page";
var L50 = "Finding Your Location.";
var L51 = "Error Adding Other Listeners";

//#endregion

//call localize init fcn
//todo move into a document onload listener (TEMP)
initLocalization();

function initLocalization() {

    //create localize object    
    localize = function () {
        return {
            //vars
            L52: "Reseting Overlays",
            L53: "Reseting POIs",
            L54: "",
            L55: "",
            L56: "Nothing to Hide",
            L57: "Nothing to Delete",
            //tooltips
            byTooltips: function () {
                //#region localization by listeners
                try {
                    //toolbar
                    document.getElementById("content_toolbar_button_reset").className += " lytetip";
                    //document.getElementById("content_toolbar_button_reset").createAttribute("data-tip");
                    document.getElementById("content_toolbar_button_reset").title = "Reset Me";
                    //document.getElementById("content_toolbar_button_reset").createAttribute('data-tip', 'Reset: Reset Map To Defaults');
                    document.getElementById("content_toolbar_button_toggleMapControls").title = "Controls: Toggle Map Controls";
                    document.getElementById("content_toolbar_button_toggleToolbox").title = "Toolbox: Toggle Toolbox";
                    document.getElementById("content_toolbar_button_layerRoadmap").title = "Roadmap: Toggle Road Map Layer";
                    document.getElementById("content_toolbar_button_layerTerrain").title = "Terrain: Toggle Terrain Map Layer";
                    document.getElementById("content_toolbar_button_layerSatellite").title = "Satellite: Toggle Satellite Map Layer";
                    document.getElementById("content_toolbar_button_layerHybrid").title = "Hybrid: Toggle Hybrid Map Layer";
                    document.getElementById("content_toolbar_button_layerCustom").title = "Block/Lot: Toggle Block/Lot Map Layer";
                    document.getElementById("content_toolbar_button_layerReset").title = "Reset: Reset Map Type";
                    document.getElementById("content_toolbar_button_panUp").title = "Up: Pan Map Up";
                    document.getElementById("content_toolbar_button_panLeft").title = "Left: Pan Map Left";
                    document.getElementById("content_toolbar_button_panReset").title = "Default: Pan Map To Default";
                    document.getElementById("content_toolbar_button_panRight").title = "Right: Pan Map Right";
                    document.getElementById("content_toolbar_button_panDown").title = "Down: Pan Map Down";
                    document.getElementById("content_toolbar_button_zoomIn").title = "In: Zoom Map In";
                    document.getElementById("content_toolbar_button_zoomReset").title = "Reset: Reset Zoom Level";
                    document.getElementById("content_toolbar_button_zoomOut").title = "Out: Zoom Map Out";
                    document.getElementById("content_toolbar_button_manageItem").title = "Manage Location Details";
                    document.getElementById("content_toolbar_button_manageOverlay").title = "Manage Map Coverage";
                    document.getElementById("content_toolbar_button_managePOI").title = "Manage Points of Interest";
                    document.getElementById("content_toolbar_button_manageSearch").title = "Locate: Find A Location On The Map";
                    document.getElementById("content_toolbar_searchField").title = "Locate: Find A Location On The Map";
                    document.getElementById("content_toolbar_searchButton").title = "Locate: Find A Location On The Map";
                    document.getElementById("content_toolbarGrabber").title = "Toolbar: Toggle the Toolbar";
                    //toolbox
                    document.getElementById("content_toolbox_button_layerRoadmap").title = "Roadmap: Toggle Road Map Layer";
                    document.getElementById("content_toolbox_button_layerTerrain").title = "Terrain: Toggle Terrain Map Layer";
                    document.getElementById("content_toolbox_button_panUp").title = "Up: Pan Map Up";
                    document.getElementById("content_toolbox_button_layerSatellite").title = "Satellite: Toggle Satellite Map Layer";
                    document.getElementById("content_toolbox_button_layerHybrid").title = "Hybrid: Toggle Hybrid Map Layer";
                    document.getElementById("content_toolbox_button_panLeft").title = "Left: Pan Map Left";
                    document.getElementById("content_toolbox_button_panReset").title = "Default: Pan Map To Default";
                    document.getElementById("content_toolbox_button_panRight").title = "Right: Pan Map Right";
                    document.getElementById("content_toolbox_button_layerCustom").title = "Block/Lot: Toggle Block/Lot Map Layer";
                    document.getElementById("content_toolbox_button_layerReset").title = "Reset: Reset Map Type";
                    document.getElementById("content_toolbox_button_panDown").title = "Down: Pan Map Down";
                    document.getElementById("content_toolbox_button_reset").title = "Reset: Reset Map To Defaults";
                    document.getElementById("content_toolbox_button_toggleMapControls").title = "Controls: Toggle Map Controls";
                    document.getElementById("content_toolbox_button_zoomIn").title = "In: Zoom Map In";
                    document.getElementById("content_toolbox_button_zoomReset").title = "Reset: Reset Zoom Level";
                    document.getElementById("content_toolbox_button_zoomOut").title = "Out: Zoom Map Out";
                    //tab
                    document.getElementById("content_toolbox_button_manageItem").title = "Manage Location Details";
                    document.getElementById("content_toolbox_button_manageOverlay").title = "Manage Map Coverage";
                    document.getElementById("content_toolbox_button_managePOI").title = "Manage Points of Interest";
                    document.getElementById("content_toolbox_searchField").title = "Locate: Find A Location On The Map";
                    document.getElementById("content_toolbox_searchButton").title = "Locate: Find A Location On The Map";
                    document.getElementById("searchResults_container").title = "Locate: Find A Location On The Map";
                    //tab
                    document.getElementById("content_toolbox_button_itemPlace").title = "Edit Location";
                    document.getElementById("content_toolbox_button_itemGetUserLocation").title = "Center On Your Current Position";
                    document.getElementById("content_toolbox_button_useSearchAsLocation").title = "Use Search Result As Location";
                    document.getElementById("content_toolbox_button_convertToOverlay").title = "Convert This To a Map Overlay";
                    document.getElementById("content_toolbox_posItem").title = "Coordinates: This is the selected Latitude and Longitude of the point you selected.";
                    document.getElementById("content_toolbox_rgItem").title = "Address: This is the nearest address of the point you selected.";
                    document.getElementById("content_toolbox_button_saveItem").title = "Save Location Changes";
                    document.getElementById("content_toolbox_button_clearItem").title = "Reset Location Changes";
                    //tab
                    //document.getElementById("content_toolbox_button_overlayEdit").title = "Toggle Overlay Editing";
                    //document.getElementById("content_toolbox_button_overlayPlace").title = "Place A New Overlay";
                    document.getElementById("content_toolbox_button_overlayGetUserLocation").title = "Center On Your Current Position";
                    document.getElementById("content_toolbox_button_overlayToggle").title = "Toggle All Overlays On Map";
                    document.getElementById("rotation").title = "Rotate: Edit the rotation value";
                    document.getElementById("rotationKnob").title = "Rotate: Edit the rotation value";
                    document.getElementById("content_toolbox_rotationCounterClockwise").title = "Tenth Degree Left: Click to Rotate a Tenth Degree Counter-Clockwise";
                    document.getElementById("content_toolbox_rotationReset").title = "Reset: Click to Reset Rotation";
                    document.getElementById("content_toolbox_rotationClockwise").title = "Tenth Degree Right: Click to Rotate a Tenth Degree Clockwise";
                    document.getElementById("transparency").title = "Transparency: Set the transparency of this Overlay";
                    document.getElementById("content_toolbox_button_saveOverlay").title = "Save Overlay Changes";
                    document.getElementById("content_toolbox_button_clearOverlay").title = "Reset All Overlay Changes";
                    //tab
                    //document.getElementById("content_toolbox_button_placePOI").title = "Toggle Point Of Interest Editing";
                    document.getElementById("content_toolbox_button_poiGetUserLocation").title = "Center On Your Current Position";
                    document.getElementById("content_toolbox_button_poiToggle").title = "Toggle All POIs On Map";
                    document.getElementById("content_toolbox_button_poiMarker").title = "Marker: Place a Point";
                    document.getElementById("content_toolbox_button_poiCircle").title = "Circle: Place a Circle";
                    document.getElementById("content_toolbox_button_poiRectangle").title = "rectangle: Place a rectangle";
                    document.getElementById("content_toolbox_button_poiPolygon").title = "Polygon: Place a Polygon";
                    document.getElementById("content_toolbox_button_poiLine").title = "Line: Place a Line";
                    document.getElementById("content_toolbox_button_savePOI").title = "Save Point Of Interest Set";
                    document.getElementById("content_toolbox_button_clearPOI").title = "Clear Point Of Interest Set";
                } catch (err) {
                    alert(L28 + ": " + err);
                }
                //#endregion
            },
            //text
            byText: function () {
                //#region localization by textual content
                try {
                    //menubar
                    document.getElementById("content_menubar_header1").innerHTML = "File";
                    document.getElementById("content_menubar_header2").innerHTML = "Controls";
                    document.getElementById("content_menubar_header2Sub1").innerHTML = "View";
                    document.getElementById("content_menubar_header2Sub2").innerHTML = "Layers";
                    document.getElementById("content_menubar_header2Sub3").innerHTML = "Zoom";
                    document.getElementById("content_menubar_header2Sub4").innerHTML = "Pan";
                    document.getElementById("content_menubar_header3").innerHTML = "Actions";
                    document.getElementById("content_menubar_manageSearch").innerHTML = "Find Location";
                    document.getElementById("content_menubar_manageItem").innerHTML = "Manage Location";
                    document.getElementById("content_menubar_manageOverlay").innerHTML = "Manage Ovelays";
                    document.getElementById("content_menubar_header3Sub3Sub1").innerHTML = "Rotate";
                    document.getElementById("content_menubar_header3Sub3Sub2").innerHTML = "Tranparency";
                    document.getElementById("content_menubar_managePOI").innerHTML = "Manage POIs";
                    document.getElementById("content_menubar_header3Sub4Sub1").innerHTML = "Place";
                    document.getElementById("content_menubar_header4").innerHTML = "Help";
                    document.getElementById("content_menubar_save").innerHTML = "Save Changes";
                    document.getElementById("content_menubar_cancel").innerHTML = "Cancel Changes";
                    document.getElementById("content_menubar_reset").innerHTML = "Reset Changes";
                    document.getElementById("content_menubar_toggleMapControls").innerHTML = "Map Controls";
                    document.getElementById("content_menubar_toggleToolbox").innerHTML = "Toolbox";
                    document.getElementById("content_menubar_toggleToolbar").innerHTML = "Toolbar";
                    document.getElementById("content_menubar_layerRoadmap").innerHTML = "Roadmap";
                    document.getElementById("content_menubar_layerSatellite").innerHTML = "Satellite";
                    document.getElementById("content_menubar_layerHybrid").innerHTML = "Hybrid";
                    document.getElementById("content_menubar_layerTerrain").innerHTML = "Terrain";
                    document.getElementById("content_menubar_layerCustom").innerHTML = "Custom";
                    document.getElementById("content_menubar_layerReset").innerHTML = "Reset";
                    document.getElementById("content_menubar_zoomIn").innerHTML = "Zoom In";
                    document.getElementById("content_menubar_zoomOut").innerHTML = "Zoom Out";
                    document.getElementById("content_menubar_zoomReset").innerHTML = "Reset";
                    document.getElementById("content_menubar_panUp").innerHTML = "Pan Up";
                    document.getElementById("content_menubar_panRight").innerHTML = "Pan Right";
                    document.getElementById("content_menubar_panDown").innerHTML = "Pan Down";
                    document.getElementById("content_menubar_panLeft").innerHTML = "Pan Left";
                    document.getElementById("content_menubar_panReset").innerHTML = "Reset";
                    document.getElementById("content_menubar_searchField").setAttribute('placeholder', "Find a Location");
                    document.getElementById("content_menubar_itemGetUserLocation").innerHTML = "Center On Current Location";
                    document.getElementById("content_menubar_itemPlace").innerHTML = "Edit Location";
                    document.getElementById("content_menubar_useSearchAsLocation").innerHTML = "Use Search Result As Location";
                    document.getElementById("content_menubar_convertToOverlay").innerHTML = "Convert To Overlay";
                    document.getElementById("content_menubar_itemReset").innerHTML = "Reset Location";
                    document.getElementById("content_menubar_overlayGetUserLocation").innerHTML = "Center On Current Location";
                    //document.getElementById("content_menubar_overlayEdit").innerHTML = "Toggle Overlay Editing";
                    //document.getElementById("content_menubar_overlayPlace").innerHTML = "Place A New Overlay";
                    document.getElementById("content_menubar_overlayToggle").innerHTML = "Toggle All Map Overlays";
                    document.getElementById("content_menubar_rotationClockwise").innerHTML = "Clockwise";
                    document.getElementById("content_menubar_rotationCounterClockwise").innerHTML = "Counter-Clockwise";
                    document.getElementById("content_menubar_rotationReset").innerHTML = "Reset";
                    document.getElementById("content_menubar_transparencyDarker").innerHTML = "Darker";
                    document.getElementById("content_menubar_transparencyLighter").innerHTML = "Lighter";
                    document.getElementById("content_menubar_transparencyReset").innerHTML = "Reset";
                    document.getElementById("content_menubar_overlayReset").innerHTML = "Reset Overlays";
                    document.getElementById("content_menubar_poiGetUserLocation").innerHTML = "Center On Current Location";
                    //document.getElementById("content_menubar_poiPlace").innerHTML = "Toggle POI Editing";
                    document.getElementById("content_menubar_poiToggle").innerHTML = "Toggle POIs On Map";
                    document.getElementById("content_menubar_poiReset").innerHTML = "Reset POIs";
                    document.getElementById("content_menubar_poiMarker").innerHTML = "Marker";
                    document.getElementById("content_menubar_poiCircle").innerHTML = "Circle";
                    document.getElementById("content_menubar_poiRectangle").innerHTML = "rectangle";
                    document.getElementById("content_menubar_poiPolygon").innerHTML = "Polygon";
                    document.getElementById("content_menubar_poiLine").innerHTML = "Line";
                    document.getElementById("content_menubar_documentation").innerHTML = "Documentation";
                    document.getElementById("content_menubar_reportAProblem").innerHTML = "Report A Problem";

                    //all others
                    document.getElementById("content_minibar_header").innerHTML = "Toolbox";
                    document.getElementById("content_toolbox_button_saveItem").value = "Save";
                    document.getElementById("content_toolbox_button_clearItem").value = "Reset";
                    document.getElementById("content_toolbox_button_saveOverlay").value = "Save";
                    document.getElementById("content_toolbox_button_clearOverlay").value = "Reset";
                    document.getElementById("content_toolbox_button_savePOI").value = "Save";
                    document.getElementById("content_toolbox_button_clearPOI").value = "Reset";
                    document.getElementById("content_toolbox_tab1_header").innerHTML = "Map Controls";
                    document.getElementById("content_toolbox_tab2_header").innerHTML = "Actions";
                    document.getElementById("content_toolbox_tab3_header").innerHTML = "Manage Location";
                    document.getElementById("content_toolbox_tab4_header").innerHTML = "Manage Overlay";
                    document.getElementById("content_toolbox_tab5_header").innerHTML = "Manage POI";
                    document.getElementById("content_toolbar_searchField").setAttribute('placeholder', "Find a Location");
                    document.getElementById("content_toolbox_searchField").setAttribute('placeholder', "Find a Location");
                    document.getElementById("content_toolbox_posItem").setAttribute('placeholder', "Selected Lat/Long");
                    document.getElementById("content_toolbox_rgItem").setAttribute('placeholder', "Nearest Address");

                } catch (err) {
                    alert(L29);
                }
                //#endregion
            }
        };
    }();

    //localize tooltips (listeners)
    localize.byTooltips();

    //localize by textual content
    localize.byText();

}

//#endregion


//Listeners

//#region Onclick Listeners

//call listeners init fcn
//todo move into a document onload listener (TEMP)
initListeners();

function initListeners() {
    try {

        //menubarf
        document.getElementById("content_menubar_save").addEventListener("click", function () {
            //save("all");
            //attempt to save all three
            save("item");
            save("overlay");
            save("poi");
        }, false);
        document.getElementById("content_menubar_cancel").addEventListener("click", function () {
            //clear("all");
            //attempt to cancel all three
            clear("item");
            clear("overlay");
            clear("poi");
        }, false);
        document.getElementById("content_menubar_reset").addEventListener("click", function () {
            resetAll();
        }, false);
        document.getElementById("content_menubar_toggleMapControls").addEventListener("click", function () {
            toggleVis("mapControls");
        }, false);
        document.getElementById("content_menubar_toggleToolbox").addEventListener("click", function () {
            toggleVis("toolbox");
        }, false);
        document.getElementById("content_menubar_toggleToolbar").addEventListener("click", function () {
            toggleVis("toolbar");
        }, false);
        document.getElementById("content_menubar_layerRoadmap").addEventListener("click", function () {
            changeMapLayer("roadmap");
        }, false);
        document.getElementById("content_menubar_layerTerrain").addEventListener("click", function () {
            changeMapLayer("terrain");
        }, false);
        document.getElementById("content_menubar_layerSatellite").addEventListener("click", function () {
            changeMapLayer("satellite");
        }, false);
        document.getElementById("content_menubar_layerHybrid").addEventListener("click", function () {
            changeMapLayer("hybrid");
        }, false);
        document.getElementById("content_menubar_layerCustom").addEventListener("click", function () {
            changeMapLayer("custom");
        }, false);
        document.getElementById("content_menubar_layerReset").addEventListener("click", function () {
            changeMapLayer("reset");
        }, false);
        document.getElementById("content_menubar_panUp").addEventListener("click", function () {
            panMap("up");
        }, false);
        document.getElementById("content_menubar_panLeft").addEventListener("click", function () {
            panMap("left");
        }, false);
        document.getElementById("content_menubar_panReset").addEventListener("click", function () {
            panMap("reset");
        }, false);
        document.getElementById("content_menubar_panRight").addEventListener("click", function () {
            panMap("right");
        }, false);
        document.getElementById("content_menubar_panDown").addEventListener("click", function () {
            panMap("down");
        }, false);
        document.getElementById("content_menubar_zoomIn").addEventListener("click", function () {
            zoomMap("in");
        }, false);
        document.getElementById("content_menubar_zoomReset").addEventListener("click", function () {
            zoomMap("reset");
        }, false);
        document.getElementById("content_menubar_zoomOut").addEventListener("click", function () {
            zoomMap("out");
        }, false);
        document.getElementById("content_menubar_manageSearch").addEventListener("click", function () {
            action("search");
            //openToolboxTab("search");
        }, false);
        document.getElementById("content_menubar_searchField").addEventListener("click", function () {
            action("search");
        }, false);
        document.getElementById("content_menubar_searchButton").addEventListener("click", function () {
            if (document.getElementById("content_menubar_searchField").value != null) {
                var stuff = document.getElementById("content_menubar_searchField").value;
                finder(stuff);
            }
        }, false);
        document.getElementById("content_menubar_manageItem").addEventListener("click", function () {
            action("manageItem");
        }, false);
        document.getElementById("content_menubar_itemPlace").addEventListener("click", function () {
            openToolboxTab("item");
            place("item");
        }, false);
        document.getElementById("content_menubar_itemGetUserLocation").addEventListener("click", function () {
            openToolboxTab("item");
            geolocate("item");
        }, false);
        document.getElementById("content_menubar_useSearchAsLocation").addEventListener("click", function () {
            openToolboxTab("item");
            useSearchAsItemLocation();
        }, false);
        document.getElementById("content_menubar_convertToOverlay").addEventListener("click", function () {
            //openToolboxTab("item");
            convertToOverlay();
            
        }, false);
        document.getElementById("content_menubar_itemReset").addEventListener("click", function () {
            openToolboxTab("item");
            clear("item");
        }, false);
        document.getElementById("content_menubar_manageOverlay").addEventListener("click", function () {
            action("manageOverlay");
        }, false);
        //document.getElementById("content_menubar_overlayPlace").addEventListener("click", function () {
        //    openToolboxTab("overlay");
        //    place("overlay");
        //}, false);
        //document.getElementById("content_menubar_overlayEdit").addEventListener("click", function () {
        //    openToolboxTab("overlay");
        //    place("overlay");
        //}, false);
        document.getElementById("content_menubar_overlayGetUserLocation").addEventListener("click", function () {
            openToolboxTab("overlay");
            geolocate("overlay");
        }, false);
        document.getElementById("content_menubar_overlayToggle").addEventListener("click", function () {
            openToolboxTab("overlay");
            toggleVis("overlays");
        }, false);
        document.getElementById("content_menubar_rotationCounterClockwise").addEventListener("click", function () {
            openToolboxTab("overlay");
            rotate(-0.1);
        }, false);
        document.getElementById("content_menubar_rotationReset").addEventListener("click", function () {
            openToolboxTab("overlay");
            rotate(0.0);
        }, false);
        document.getElementById("content_menubar_rotationClockwise").addEventListener("click", function () {
            openToolboxTab("overlay");
            rotate(0.1);
        }, false);
        document.getElementById("content_menubar_transparencyDarker").addEventListener("click", function () {
            openToolboxTab("overlay");
            opacity(0.1);
        }, false);
        document.getElementById("content_menubar_transparencyLighter").addEventListener("click", function () {
            openToolboxTab("overlay");
            opacity(-0.1);
        }, false);
        document.getElementById("content_menubar_transparencyReset").addEventListener("click", function () {
            openToolboxTab("overlay");
            opacity(0.35); //change to dynamic default
        }, false);
        document.getElementById("content_menubar_overlayReset").addEventListener("click", function () {
            openToolboxTab("overlay");
            clear("overlay");
        }, false);
        document.getElementById("content_menubar_managePOI").addEventListener("click", function () {
            action("managePOI");
        }, false);
        //document.getElementById("content_menubar_poiPlace").addEventListener("click", function () {
        //    openToolboxTab("poi");
        //    place("poi");
        //}, false);
        document.getElementById("content_menubar_poiGetUserLocation").addEventListener("click", function () {
            openToolboxTab("poi");
            geolocate("poi");
        }, false);
        //document.getElementById("content_menubar_poiToggle").addEventListener("click", function () {
        //    openToolboxTab("poi");
        //    toggleVis("pois");
        //}, false);
        document.getElementById("content_menubar_poiMarker").addEventListener("click", function () {
            openToolboxTab("poi");
            placePOI("marker");
        }, false);
        document.getElementById("content_menubar_poiCircle").addEventListener("click", function () {
            openToolboxTab("poi");
            placePOI("circle");
        }, false);
        document.getElementById("content_menubar_poiRectangle").addEventListener("click", function () {
            openToolboxTab("poi");
            placePOI("rectangle");
        }, false);
        document.getElementById("content_menubar_poiPolygon").addEventListener("click", function () {
            openToolboxTab("poi");
            placePOI("polygon");
        }, false);
        document.getElementById("content_menubar_poiLine").addEventListener("click", function () {
            openToolboxTab("poi");
            placePOI("line");
        }, false);
        document.getElementById("content_menubar_poiReset").addEventListener("click", function () {
            openToolboxTab("poi");
            clear("poi");
        }, false);
        document.getElementById("content_menubar_documentation").addEventListener("click", function () {
            displayMessage("No Documentation Yet.");
        }, false);
        document.getElementById("content_menubar_reportAProblem").addEventListener("click", function () {
            displayMessage("No Method To Report Errors Yet.");
        }, false);

        //toolbar
        document.getElementById("content_toolbar_button_reset").addEventListener("click", function () {
            resetAll();
        }, false);
        document.getElementById("content_toolbar_button_toggleMapControls").addEventListener("click", function () {
            toggleVis("mapControls");
        }, false);
        document.getElementById("content_toolbar_button_toggleToolbox").addEventListener("click", function () {
            toggleVis("toolbox");
        }, false);
        document.getElementById("content_toolbar_button_layerRoadmap").addEventListener("click", function () {
            changeMapLayer("roadmap");
        }, false);
        document.getElementById("content_toolbar_button_layerTerrain").addEventListener("click", function () {
            changeMapLayer("terrain");
        }, false);
        document.getElementById("content_toolbar_button_layerSatellite").addEventListener("click", function () {
            changeMapLayer("satellite");
        }, false);
        document.getElementById("content_toolbar_button_layerHybrid").addEventListener("click", function () {
            changeMapLayer("hybrid");
        }, false);
        document.getElementById("content_toolbar_button_layerCustom").addEventListener("click", function () {
            changeMapLayer("custom");
        }, false);
        document.getElementById("content_toolbar_button_layerReset").addEventListener("click", function () {
            changeMapLayer("reset");
        }, false);
        document.getElementById("content_toolbar_button_panUp").addEventListener("click", function () {
            panMap("up");
        }, false);
        document.getElementById("content_toolbar_button_panLeft").addEventListener("click", function () {
            panMap("left");
        }, false);
        document.getElementById("content_toolbar_button_panReset").addEventListener("click", function () {
            panMap("reset");
        }, false);
        document.getElementById("content_toolbar_button_panRight").addEventListener("click", function () {
            panMap("right");
        }, false);
        document.getElementById("content_toolbar_button_panDown").addEventListener("click", function () {
            panMap("down");
        }, false);
        document.getElementById("content_toolbar_button_zoomIn").addEventListener("click", function () {
            zoomMap("in");
        }, false);
        document.getElementById("content_toolbar_button_zoomReset").addEventListener("click", function () {
            zoomMap("reset");
        }, false);
        document.getElementById("content_toolbar_button_zoomOut").addEventListener("click", function () {
            zoomMap("out");
        }, false);
        document.getElementById("content_toolbar_button_manageItem").addEventListener("click", function () {
            action("manageItem");
        }, false);
        document.getElementById("content_toolbar_button_manageOverlay").addEventListener("click", function () {
            action("manageOverlay");
        }, false);
        document.getElementById("content_toolbar_button_managePOI").addEventListener("click", function () {
            action("managePOI");
        }, false);
        document.getElementById("content_toolbar_button_manageSearch").addEventListener("click", function () {
            action("search");
        }, false);
        document.getElementById("content_toolbar_searchField").addEventListener("click", function () {
            action("search");
        }, false);
        document.getElementById("content_toolbar_searchButton").addEventListener("click", function () {
            if (document.getElementById("content_toolbar_searchField").value != null) {
                var stuff = document.getElementById("content_toolbar_searchField").value;
                finder(stuff);
            }
        }, false);
        document.getElementById("content_toolbarGrabber").addEventListener("click", function () {
            toggleVis("toolbar");
        }, false);

        //toolbox
        //minibar
        document.getElementById("content_minibar_button_minimize").addEventListener("click", function () {
            toggleVis("toolboxMin");
        }, false);
        document.getElementById("content_minibar_button_maximize").addEventListener("click", function () {
            toggleVis("toolboxMax");
        }, false);
        document.getElementById("content_minibar_button_close").addEventListener("click", function () {
            toggleVis("toolbox");
        }, false);
        //headers
        document.getElementById("content_toolbox_tab1_header").addEventListener("click", function () {
            de("tab1 header clicked...");
            action("other");
            openToolboxTab(0);
        }, false);
        document.getElementById("content_toolbox_tab2_header").addEventListener("click", function () {
            de("tab2 header clicked...");
            action("search");
            //action("other");
            //openToolboxTab(1);
        }, false);
        document.getElementById("content_toolbox_tab3_header").addEventListener("click", function () {
            de("tab3 header clicked...");
            action("manageItem");
            //openToolboxTab(2); //called in action
        }, false);
        document.getElementById("content_toolbox_tab4_header").addEventListener("click", function () {
            de("tab4 header clicked...");
            action("manageOverlay");
            //openToolboxTab(3); //called in action
        }, false);
        document.getElementById("content_toolbox_tab5_header").addEventListener("click", function () {
            de("tab5 header clicked...");
            action("managePOI");
            //openToolboxTab(4); //called in action
        }, false);
        //tab
        document.getElementById("content_toolbox_button_layerRoadmap").addEventListener("click", function () {
            changeMapLayer("roadmap");
        }, false);
        document.getElementById("content_toolbox_button_layerTerrain").addEventListener("click", function () {
            changeMapLayer("terrain");
        }, false);
        document.getElementById("content_toolbox_button_panUp").addEventListener("click", function () {
            panMap("up");
        }, false);
        document.getElementById("content_toolbox_button_layerSatellite").addEventListener("click", function () {
            changeMapLayer("satellite");
        }, false);
        document.getElementById("content_toolbox_button_layerHybrid").addEventListener("click", function () {
            changeMapLayer("hybrid");
        }, false);
        document.getElementById("content_toolbox_button_panLeft").addEventListener("click", function () {
            panMap("left");
        }, false);
        document.getElementById("content_toolbox_button_panReset").addEventListener("click", function () {
            panMap("reset");
        }, false);
        document.getElementById("content_toolbox_button_panRight").addEventListener("click", function () {
            panMap("right");
        }, false);
        document.getElementById("content_toolbox_button_layerCustom").addEventListener("click", function () {
            changeMapLayer("custom");
        }, false);
        document.getElementById("content_toolbox_button_layerReset").addEventListener("click", function () {
            changeMapLayer("reset");
        }, false);
        document.getElementById("content_toolbox_button_panDown").addEventListener("click", function () {
            panMap("down");
        }, false);
        document.getElementById("content_toolbox_button_reset").addEventListener("click", function () {
            resetAll();
        }, false);
        document.getElementById("content_toolbox_button_toggleMapControls").addEventListener("click", function () {
            toggleVis("mapControls");
        }, false);
        document.getElementById("content_toolbox_button_zoomIn").addEventListener("click", function () {
            zoomMap("in");
        }, false);
        document.getElementById("content_toolbox_button_zoomReset").addEventListener("click", function () {
            zoomMap("reset");
        }, false);
        document.getElementById("content_toolbox_button_zoomOut").addEventListener("click", function () {
            zoomMap("out");
        }, false);
        //tab
        document.getElementById("content_toolbox_button_manageItem").addEventListener("click", function () {
            action("manageItem");
        }, false);
        document.getElementById("content_toolbox_button_manageOverlay").addEventListener("click", function () {
            action("manageOverlay");
        }, false);
        document.getElementById("content_toolbox_button_managePOI").addEventListener("click", function () {
            action("managePOI");
        }, false);
        document.getElementById("content_toolbox_searchField").addEventListener("click", function () {
            //nothing yet
        }, false);
        document.getElementById("content_toolbox_searchButton").addEventListener("click", function () {
            if (document.getElementById("content_toolbox_searchField").value != null) {
                var stuff = document.getElementById("content_toolbox_searchField").value;
                finder(stuff);
            }
        }, false);
        //tab
        document.getElementById("content_toolbox_button_itemPlace").addEventListener("click", function () {
            place("item");
        }, false);
        document.getElementById("content_toolbox_button_itemGetUserLocation").addEventListener("click", function () {
            geolocate("item");
        }, false);
        document.getElementById("content_toolbox_button_useSearchAsLocation").addEventListener("click", function () {
            useSearchAsItemLocation();
        }, false);
        document.getElementById("content_toolbox_button_convertToOverlay").addEventListener("click", function () {
            //convert it
            convertToOverlay();
            //now trigger place overlay (we doe this inside the cinverttooverlay fcn)
            //place("overlay");
            //now enable editing of overlay
            //edit("overlay");
        }, false);
        document.getElementById("content_toolbox_posItem").addEventListener("click", function () {
            //nothing, maybe copy?
        }, false);
        document.getElementById("content_toolbox_rgItem").addEventListener("click", function () {
            //nothing, maybe copy?
        }, false);
        document.getElementById("content_toolbox_button_saveItem").addEventListener("click", function () {
            save("item");
        }, false);
        document.getElementById("content_toolbox_button_clearItem").addEventListener("click", function () {
            clear("item");
        }, false);
        //tab
        //document.getElementById("content_toolbox_button_overlayPlace").addEventListener("click", function () {
        //    place("overlay");
        //}, false);
        //document.getElementById("content_toolbox_button_overlayEdit").addEventListener("click", function () {
        //    place("overlay");
        //}, false);
        document.getElementById("content_toolbox_button_overlayGetUserLocation").addEventListener("click", function () {
            geolocate("overlay");
        }, false);
        document.getElementById("content_toolbox_button_overlayToggle").addEventListener("click", function () {
            toggleVis("overlays");
        }, false);
        document.getElementById("rotationKnob").addEventListener("click", function () {
            //do nothing, (possible just mapedit_container)
        }, false);
        document.getElementById("content_toolbox_rotationCounterClockwise").addEventListener("click", function () {
            rotate(-0.1);
        }, false);
        document.getElementById("content_toolbox_rotationReset").addEventListener("click", function () {
            rotate(0);
        }, false);
        document.getElementById("content_toolbox_rotationClockwise").addEventListener("click", function () {
            rotate(0.1);
        }, false);
        document.getElementById("transparency").addEventListener("click", function () {
            //nothing (possible just mapedit_container)
        }, false);
        document.getElementById("overlayTransparencySlider").addEventListener("click", function () {
            //nothing (possible just mapedit_container)
        }, false);
        document.getElementById("content_toolbox_button_saveOverlay").addEventListener("click", function () {
            save("overlay");
        }, false);
        document.getElementById("content_toolbox_button_clearOverlay").addEventListener("click", function () {
            clear("overlay");
        }, false);
        //tab
        //document.getElementById("content_toolbox_button_placePOI").addEventListener("click", function () {
        //    place("poi");
        //}, false);
        document.getElementById("content_toolbox_button_poiGetUserLocation").addEventListener("click", function () {
            geolocate("poi");
        }, false);
        document.getElementById("content_toolbox_button_poiToggle").addEventListener("click", function () {
            toggleVis("pois");
        }, false);
        document.getElementById("content_toolbox_button_poiMarker").addEventListener("click", function () {
            placePOI("marker");
        }, false);
        document.getElementById("content_toolbox_button_poiCircle").addEventListener("click", function () {
            placePOI("circle");
        }, false);
        document.getElementById("content_toolbox_button_poiRectangle").addEventListener("click", function () {
            placePOI("rectangle");
        }, false);
        document.getElementById("content_toolbox_button_poiPolygon").addEventListener("click", function () {
            placePOI("polygon");
        }, false);
        document.getElementById("content_toolbox_button_poiLine").addEventListener("click", function () {
            placePOI("line");
        }, false);
        document.getElementById("content_toolbox_button_savePOI").addEventListener("click", function () {
            save("poi");
        }, false);
        document.getElementById("content_toolbox_button_clearPOI").addEventListener("click", function () {
            clear("poi");
        }, false);
    } catch (err) {
        alert(Lerror1 + ": " + err);
    }
}

//#endregion


//Listener Actions

//#region Listener Actions

//reset handler
function resetAll() {

    document.location.reload(true); //refresh page

    //if (globalVar.userMayLoseData) {
    //    //warn the user
    //    var consent = confirmMessage(L47);
    //    if (consent == true) {
    //        displayMessage(L48);
    //        document.location.reload(true); //refresh page
    //    } else {
    //        displayMessage(L49);
    //    }
    //} else {
    //    displayMessage(L48);
    //    document.location.reload(true); //refresh page
    //}

}

//toggle items handler
function toggleVis(id) {

    /// <summary>This Will Toggle The Visual Effects Of An Item</summary>
    /// <param name="id" type="string">The ID To Toggle</param>

    switch (id) {
        case "mapControls":
            if (globalVar.mapControlsDisplayed == true) { //present, hide
                map.setOptions({
                    zoomControl: false,
                    panControl: false,
                    mapTypeControl: false
                });
                //drawingManager.setMap(null);
                //globalVar.mapDrawingManagerDisplayed = false;
                globalVar.mapControlsDisplayed = false;
            } else { //not present, make present
                map.setOptions({
                    zoomControl: true,
                    zoomControlOptions: { style: google.maps.ZoomControlStyle.SMALL, position: google.maps.ControlPosition.LEFT_TOP },
                    panControl: true,
                    panControlOptions: { position: google.maps.ControlPosition.LEFT_TOP },
                    mapTypeControl: true,
                    mapTypeControlOptions: { style: google.maps.MapTypeControlStyle.DROPDOWN_MENU, position: google.maps.ControlPosition.RIGHT_TOP }
                });
                globalVar.mapControlsDisplayed = true;
            }
            buttonActive("mapControls"); //set the is active glow for button
            break;

        case "toolbox":
            if (globalVar.toolboxDisplayed == true) {
                document.getElementById("mapedit_container_toolbox").style.display = "none";
                document.getElementById("mapedit_container_toolboxTabs").style.display = "none";
                //$("#mapedit_container_toolbox").effect("slide", 500);
                globalVar.toolboxDisplayed = false;
            } else {
                document.getElementById("mapedit_container_toolbox").style.display = "block";
                document.getElementById("mapedit_container_toolboxTabs").style.display = "block";
                document.getElementById("mapedit_container_toolbox").style.height = "auto";
                globalVar.toolboxDisplayed = true;
            }
            buttonActive("toolbox"); //set the is active glow for button
            break;

        case "toolbar":
            if (globalVar.toolbarDisplayed == true) {
                $("#mapedit_container_pane_1").hide();
                document.getElementById("mapedit_container_toolbarGrabber").style.marginTop = "0";
                globalVar.toolbarDisplayed = false;
            } else {
                $("#mapedit_container_pane_1").show();
                document.getElementById("mapedit_container_toolbarGrabber").style.marginTop = "48px";
                globalVar.toolbarDisplayed = true;
            }
            buttonActive("toolbar"); //set the is active glow for button
            break;

        case "kml":
            if (globalVar.kmlDisplayed == true) {
                KmlLayer.setMap(null);
                globalVar.kmlDisplayed = false;
            } else {
                KmlLayer.setMap(map);
                globalVar.kmlDisplayed = true;
            }
            buttonActive("kml"); //set the is active glow for button
            break;

        case "toolboxMin":
            $("#mapedit_container_toolboxTabs").hide();
            document.getElementById("mapedit_container_toolbox").style.height = "17px";
            break;

        case "toolboxMax":
            $("#mapedit_container_toolboxTabs").show();
            document.getElementById("mapedit_container_toolbox").style.height = "auto";
            break;

        case "mapDrawingManager":
            if (globalVar.mapDrawingManagerDisplayed == true) {
                drawingManager.setMap(null);
                globalVar.mapDrawingManagerDisplayed = false;
            } else {
                drawingManager.setMap(map);
                globalVar.mapDrawingManagerDisplayed = true;
            }
            //buttonActive("mapDrawingManager"); 
            break;

        case "overlays":
            if (globalVar.overlaysOnMap.length) {
                if (globalVar.overlaysCurrentlyDisplayed == true) {
                    displayMessage(L22);
                    for (var i = 1; i < globalVar.overlaysOnMap.length; i++) { //go through and display overlays as long as there is an overlay to display
                        de("overlay count " + globalVar.overlayCount);
                        globalVar.RIBMode = true;
                        if (document.getElementById("overlayToggle" + i)) {
                            de("found: overlayToggle" + i);
                            overlayHideMe(i);
                        } else {
                            de("did not find: overlayToggle" + i);
                        }

                        globalVar.RIBMode = false;
                        globalVar.overlaysOnMap[i].setMap(null); //hide the overlay from the map
                        globalVar.ghostOverlayRectangle[i].setMap(null); //hide ghost from map
                        globalVar.overlaysCurrentlyDisplayed = false; //mark that overlays are not on the map
                        globalVar.buttonActive_overlayToggle = true;
                        buttonActive("overlayToggle");
                    }
                } else {
                    displayMessage(L23);
                    for (var i = 1; i < globalVar.overlaysOnMap.length; i++) { //go through and display overlays as long as there is an overlay to display
                        de("oom " + globalVar.overlaysOnMap.length);
                        globalVar.RIBMode = true;
                        if (document.getElementById("overlayToggle" + i)) {
                            de("found: overlayToggle" + i);
                            overlayShowMe(i);
                        } else {
                            de("did not find: overlayToggle" + i);
                        }
                        globalVar.RIBMode = false;
                        globalVar.overlaysOnMap[i].setMap(map); //set the overlay to the map
                        globalVar.ghostOverlayRectangle[i].setMap(map); //set to map
                        globalVar.overlaysCurrentlyDisplayed = true; //mark that overlays are on the map
                        globalVar.buttonActive_overlayToggle = false;
                        buttonActive("overlayToggle");
                    }
                }
            } else {
                //nothing to toggle
                displayMessage(L45);
            }
            break;

        case "pois":
            if (globalVar.poiCount) {
                de("poi count: " + globalVar.poiCount);
                buttonActive("poiToggle");
                if (globalVar.poiToggleState == "displayed") {
                    for (var i = 0; i < globalVar.poiCount; i++) {
                        poiHideMe(i);
                        globalVar.poiToggleState = "hidden";
                        displayMessage(L42);
                    }
                } else {
                    for (var i = 0; i < globalVar.poiCount; i++) {
                        poiShowMe(i);
                        globalVar.poiToggleState = "displayed";
                        displayMessage(L43);
                    }
                }
                
                
            } else {
                //nothing to toggle
                displayMessage(L45);
            }
            break;

        default:
            //toggle that item if not found above
            break;
    }
}

//map layer type handler
function changeMapLayer(layer) {
    switch (layer) {
        case "roadmap":
            map.setMapTypeId(google.maps.MapTypeId.ROADMAP);
            globalVar.mapLayerActive = "Roadmap";
            break;
        case "terrain":
            map.setMapTypeId(google.maps.MapTypeId.TERRAIN);
            globalVar.mapLayerActive = "Terrain";
            break;
        case "satellite":
            map.setMapTypeId(google.maps.MapTypeId.SATELLITE);
            globalVar.mapLayerActive = "Satellite";
            break;
        case "hybrid":
            map.setMapTypeId(google.maps.MapTypeId.HYBRID);
            globalVar.mapLayerActive = "Hybrid";
            break;
        case "custom":
            toggleVis("kml");
            break;
        case "reset":
            map.setMapTypeId(google.maps.MapTypeId.ROADMAP);
            //2do make this set to default
            globalVar.mapLayerActive = "Roadmap";
            if (globalVar.kmlDisplayed == true) {
                toggleVis("kml");
            }
            break;
    }
    buttonActive("layer"); //set the is active glow for button
}

//pan map handler
function panMap(direction) {
    switch (direction) {
        case "up":
            map.panBy(0, -100);
            testBounds();
            break;
        case "down":
            map.panBy(0, 100);
            testBounds();
            break;
        case "left":
            map.panBy(-100, 0);
            testBounds();
            break;
        case "right":
            map.panBy(100, 0);
            testBounds();
            break;
        case "reset":
            map.panTo(globalVar.mapCenter);
            break;
    }
}

//zoom map handler
function zoomMap(direction) {
    switch (direction) {
        case "in":
            map.setZoom(map.getZoom() + 1);
            break;
        case "out":
            map.setZoom(map.getZoom() - 1);
            break;
        case "reset":
            map.setZoom(globalVar.defaultZoomLevel);
            break;
    }
}

//user initiated action handler
function action(id) {
    ///<summary>User initiated action handler. This triggers events based on user choice</summary>
    ///<param name="id">String, Action string identifier</param>

    de("action: " + id);
    switch (id) {
        case "manageItem":
            globalVar.userMayLoseData = true;
            globalVar.actionActive = "Item";  //note case (uppercase is tied to the actual div)
            buttonActive("action");
            if (globalVar.toolboxDisplayed != true) {
                toggleVis("toolbox");
            }
            openToolboxTab(2);
            //force a suppression dm
            if (globalVar.mapDrawingManagerDisplayed == true) {
                globalVar.mapDrawingManagerDisplayed = true;
                toggleVis("mapDrawingManager");
            }
            
            //save 
            //globalVar.RIBMode = true;
            //save("overlay");
            //save("poi");
            //globalVar.RIBMode = false;
            
            globalVar.placerType = "item";
            place("item");

            break;

        case "manageOverlay":
            globalVar.userMayLoseData = true; //mark that we may lose data if we exit page
            
            globalVar.actionActive = "Overlay"; //notice case (uppercase is tied to the actual div)
            buttonActive("action"); 
            
            if (globalVar.toolboxDisplayed != true) {
                toggleVis("toolbox");
            }
            openToolboxTab(3);
            
            //force a suppression of dm
            if (globalVar.mapDrawingManagerDisplayed == true) {
                globalVar.mapDrawingManagerDisplayed = true;
                toggleVis("mapDrawingManager");
            }

            //save
            //globalVar.RIBMode = true;
            //save("item");
            //save("poi");
            //globalVar.RIBMode = false;

            //place
            globalVar.placerType = "overlay";
            place("overlay");

            break;

        case "managePOI":
            globalVar.userMayLoseData = true;
            globalVar.actionActive = "POI"; //notice case (uppercase is tied to the actual div)
            buttonActive("action");
            if (globalVar.toolboxDisplayed != true) {
                toggleVis("toolbox");
            }
            openToolboxTab(4);
            toggleVis("mapDrawingManager");

            //save
            //globalVar.RIBMode = true;
            //save("item");
            //save("overlay");
            //globalVar.RIBMode = false;

            //place
            globalVar.placerType = "poi";
            place("poi");
            break;

        case "other":
            de("action Other started...");
            globalVar.actionActive = "Other";
            buttonActive("action");
            //openToolboxTab(); //not called here, called in listerner
            //force a suppression dm
            if (globalVar.mapDrawingManagerDisplayed == true) {
                globalVar.mapDrawingManagerDisplayed = true;
                toggleVis("mapDrawingManager");
            }

            //save
            //globalVar.RIBMode = true;
            //save("item");
            //save("overlay");
            //save("poi");
            //globalVar.RIBMode = false;

            globalVar.placerType = "none";

            de("action Other ended...");

            break;

        case "search":
            de("action search started...");
            globalVar.actionActive = "Search";
            buttonActive("action");
            globalVar.placerType = "none";

            //force a suppression dm (unknown if we need to?)
            if (globalVar.mapDrawingManagerDisplayed == true) {
                globalVar.mapDrawingManagerDisplayed = true;
                toggleVis("mapDrawingManager");
            }

            //open search tab
            de("globalVar.toolboxDisplayed: " + globalVar.toolboxDisplayed);
            if (globalVar.toolboxDisplayed == true) {
                openToolboxTab(1);
            }

            break;
    }
    de("action() completed");
}

//placer button handler
function place(id) {
    switch (id) {
        case "item":
            buttonActive("itemPlace");
            globalVar.placerType = "item";
            if (globalVar.itemMarker != null) {
                displayMessage(L30);
            } else {
                if (globalVar.searchCount > 0 && globalVar.itemMarker == null) {
                    useSearchAsItemLocation();
                    displayMessage(L18);
                } else {
                    drawingManager.setDrawingMode(google.maps.drawing.OverlayType.MARKER);
                    drawingManager.setOptions({ drawingControl: false, drawingControlOptions: { position: google.maps.ControlPosition.RIGHT_TOP, drawingModes: [google.maps.drawing.OverlayType.MARKER] } });
                    drawingManager.setMap(map);
                }
            }
            break;

        case "overlay":
            
            //if (globalVar.currentlyEditingOverlays == true) {

            //    document.getElementById("content_menubar_overlayEdit").className += " isActive2";
            //    document.getElementById("content_toolbox_button_overlayEdit").className += " isActive";

            //    document.getElementById("content_menubar_overlayPlace").className += " isActive2";
            //    document.getElementById("content_toolbox_button_overlayPlace").className += " isActive";
                
            //} else {

            //    document.getElementById("content_menubar_overlayEdit").className = document.getElementById("content_menubar_overlayEdit").className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
            //    document.getElementById("content_toolbox_button_overlayEdit").className = document.getElementById("content_toolbox_button_overlayEdit").className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                
            //    document.getElementById("content_menubar_overlayPlace").className = document.getElementById("content_menubar_overlayPlace").className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
            //    document.getElementById("content_toolbox_button_overlayPlace").className = document.getElementById("content_toolbox_button_overlayPlace").className.replace(/(?:^|\s)isActive(?!\S)/g, '');

            //}
            
            //buttonActive("overlayEdit");
            //buttonActive("overlayPlace");

            globalVar.placerType = "overlay";
            //if (globalVar.savingOverlayIndex.length > 0) {
                if (globalVar.pageMode == "edit") {
                    globalVar.pageMode = "view";
                    //if (globalVar.overlaysOnMap.length > 0) {
                    //    for (var i = 0; i < globalVar.overlaysOnMap.length; i++) {
                    //        if (globalVar.ghostOverlayRectangle[i]) {
                    //            globalVar.ghostOverlayRectangle[i].setOptions(globalVar.ghosting); //set globalVar.rectangle to globalVar.ghosting    
                    //        }
                    //    }
                    //}
                    displayMessage(L26);
                    //document.getElementById("content_menubar_overlayEdit").className = document.getElementById("content_menubar_overlayEdit").className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
                    //document.getElementById("content_toolbox_button_overlayEdit").className = document.getElementById("content_toolbox_button_overlayEdit").className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                    //document.getElementById("content_menubar_overlayPlace").className = document.getElementById("content_menubar_overlayPlace").className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
                    //document.getElementById("content_toolbox_button_overlayPlace").className = document.getElementById("content_toolbox_button_overlayPlace").className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                    
                } else {
                    globalVar.pageMode = "edit";
                    //if (globalVar.overlaysOnMap.length > 0) {
                    //    for (var i = 0; i < globalVar.overlaysOnMap.length; i++) {
                    //        if (globalVar.ghostOverlayRectangle[i]) {
                    //            globalVar.ghostOverlayRectangle[i].setOptions(globalVar.editable); //set globalVar.rectangle to globalVar.editable
                    //        }
                    //    }
                    //}
                    displayMessage(L27);
                    //document.getElementById("content_menubar_overlayEdit").className += " isActive2";
                    //document.getElementById("content_toolbox_button_overlayEdit").className += " isActive";
                    //document.getElementById("content_menubar_overlayPlace").className += " isActive2";
                    //document.getElementById("content_toolbox_button_overlayPlace").className += " isActive";
                }
                //toggleOverlayEditor(); 
            //} else {
            //    //select the area to draw the overlay
            //    displayMessage(L41);

            //    //define drawing manager
            //    drawingManager.setOptions({ drawingControl: false, drawingControlOptions: { position: google.maps.ControlPosition.RIGHT_TOP, drawingModes: [google.maps.drawing.OverlayType.RECTANGLE] }, rectangleOptions: { strokeOpacity: 0.2, strokeWeight: 1, fillOpacity: 0.0 } });

            //    //set drawingmode to rectangle
            //    drawingManager.setDrawingMode(google.maps.drawing.OverlayType.RECTANGLE);

            //    //apply the changes
            //    drawingManager.setMap(map);

            //    //go ahead and auto switch to editing mode
            //    globalVar.pageMode = "edit";
            //}
            break;

        case "poi":
            //buttonActive("poiPlace");
            drawingManager.setOptions({ drawingControl: true, drawingControlOptions: { position: google.maps.ControlPosition.RIGHT_TOP, drawingModes: [google.maps.drawing.OverlayType.MARKER, google.maps.drawing.OverlayType.CIRCLE, google.maps.drawing.OverlayType.RECTANGLE, google.maps.drawing.OverlayType.POLYGON, google.maps.drawing.OverlayType.POLYLINE] } });
            globalVar.placerType = "poi";
            break;
    }
}

//poi object placer handler
function placePOI(type) {
    globalVar.placerType = "poi";
    switch (type) {
        case "marker":
            drawingManager.setDrawingMode(google.maps.drawing.OverlayType.MARKER);
            buttonActive("poiMarker");
            break;
        case "circle":
            drawingManager.setDrawingMode(google.maps.drawing.OverlayType.CIRCLE);
            buttonActive("poiCircle");
            break;
        case "rectangle":
            drawingManager.setDrawingMode(google.maps.drawing.OverlayType.RECTANGLE);
            buttonActive("poiRectangle");
            break;
        case "polygon":
            drawingManager.setDrawingMode(google.maps.drawing.OverlayType.POLYGON);
            buttonActive("poiPolygon");
            break;
        case "line":
            drawingManager.setDrawingMode(google.maps.drawing.OverlayType.POLYLINE);
            buttonActive("poiLine");
            break;
    }
}

//geolocation handler
function geolocate(id) {
    displayMessage(L50);
    switch (id) {
        case "item":
            drawingManager.setDrawingMode(google.maps.drawing.OverlayType.MARKER);
            globalVar.placerType = "item";
            // Try W3C Geolocation
            if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(function (position) {
                    var userLocation = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);
                    map.setCenter(userLocation);
                    testBounds();
                    if (globalVar.mapInBounds=="yes") {
                        globalVar.markerCenter = userLocation;
                        globalVar.itemMarker = new google.maps.Marker({
                            position: globalVar.markerCenter,
                            map: map
                        });
                        globalVar.itemMarker.setMap(map);
                        document.getElementById('content_toolbox_posItem').value = globalVar.itemMarker.getPosition();
                        codeLatLng(globalVar.itemMarker.getPosition());
                        globalVar.savingMarkerCenter = globalVar.itemMarker.getPosition(); //store coords to save
                    } else {
                        //do nothing
                        de("rg userlocation: " + userLocation);
                        //globalVar.markerCenter = userLocation;
                        //globalVar.itemMarker = new google.maps.Marker({
                        //    position: userLocation,
                        //    map: map
                        //});
                        //de("rg test1: " + globalVar.markerCenter.getPosition());
                        var userLocationS = userLocation;
                        userLocationS = userLocationS.replace(")", "");
                        userLocationS = userLocationS.replace("(", "");
                        de("rg test2: " + userLocationS);
                        document.getElementById('content_toolbox_posItem').value = userLocationS;
                        codeLatLng(userLocation);
                    }
                    
                });

            } else {
                alert(L4);
            }
            drawingManager.setDrawingMode(null);
            break;

        case "overlay":
            globalVar.placerType = "overlay";
            // Try W3C Geolocation
            if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(function (position) {
                    var userLocation = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);
                    map.setCenter(userLocation);
                    testBounds();
                });

            } else {
                alert(L4);
            }
            break;

        case "poi":
            //drawingManager.setDrawingMode(google.maps.drawing.OverlayType.MARKER);
            globalVar.placerType = "poi";
            // Try W3C Geolocation
            if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(function (position) {
                    var userLocation = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);
                    map.setCenter(userLocation);
                    testBounds();
                    //var marker = new google.maps.Marker({
                    //    position: userLocation,
                    //    map: map
                    //});
                    //marker.setMap(map);
                });

            } else {
                alert(L4);
            }
            //drawingManager.setDrawingMode(null);
            break;
    }
}

//save handler
function save(id) {
    switch (id) {
        case "item":
            //determine if there is something to save
            if (globalVar.savingMarkerCenter != null) {
                //is this the first time saving a changed item?
                if (globalVar.firstSaveItem == true) {
                    de("saving location: " + globalVar.savingMarkerCenter);
                    //save to temp xml file
                    createSavedItem("save", globalVar.savingMarkerCenter);
                    if (globalVar.toServerSuccess == true) {
                        displayMessage(L_Saved);
                    }
                    //reset first save
                    //globalVar.firstSaveItem = false;
                    //change save button to apply button
                    //document.getElementById("content_toolbox_button_saveItem").value = L36;
                    //change save title to apply
                    //document.getElementById("content_toolbox_button_saveItem").title = L35;
                } else {
                    //apply the changes
                    de("Applying Changes...");
                    de("applying location: " + globalVar.savingMarkerCenter);
                    //save to live areas
                    createSavedItem("apply", globalVar.savingMarkerCenter);
                    if (globalVar.toServerSuccess == true) {
                        displayMessage(L_Applied);
                    }
                    //reset first save
                    globalVar.firstSaveItem = true;
                    //reset apply button to save
                    document.getElementById("content_toolbox_button_saveItem").value = L37;
                    document.getElementById("content_toolbox_button_saveItem").title = L38;
                }
            } else {
                displayMessage(L_NotSaved);
            }

            break;

        case "overlay":
            //is this the first time saving a changed item?
            de("first save overlay? " + globalVar.firstSaveOverlay);
            if (globalVar.firstSaveOverlay == true) {
                //determine if there is something to save
                de("overlay length? " + globalVar.savingOverlayIndex.length);
                if (globalVar.savingOverlayIndex.length) {
                    for (var i = 0; i < globalVar.savingOverlayIndex.length; i++) {
                        //save to temp xml file
                        try {
                            de("saving overlay: (" + i + ") " + globalVar.savingOverlayPageId[i] + "\nlabel: " + globalVar.savingOverlayLabel[i] + "\nsource: " + globalVar.savingOverlaySourceURL[i] + "\nbounds: " + globalVar.savingOverlayBounds[i] + "\nrotation: " + globalVar.savingOverlayRotation[i]);
                            createSavedOverlay("save", globalVar.savingOverlayPageId[i], globalVar.savingOverlayLabel[i], globalVar.savingOverlaySourceURL[i], globalVar.savingOverlayBounds[i], globalVar.savingOverlayRotation[i]); //send overlay to the server
                            if (globalVar.toServerSuccess == true) {
                                displayMessage(L_Saved);
                            }
                        } catch(e) {
                            //no overlay at this point to save
                        } 
                    }
                    //reset first save
                    //globalVar.firstSaveOverlay = false;
                    //change save button to apply button
                    //document.getElementById("content_toolbox_button_saveOverlay").value = L36;
                    //change save title to apply
                    //document.getElementById("content_toolbox_button_saveOverlay").title = L35;
                } else {
                    //tell that we did not save anything
                    displayMessage(L_NotSaved);
                }
            } else {

                //is there something to apply?
                if (globalVar.savingOverlayIndex.length) {
                    //apply the changes
                    de("Applying Changes...");
                    for (var i = 0; i < globalVar.savingOverlayIndex.length; i++) {
                        //save to temp xml file
                        de("applying overlay: " + globalVar.savingOverlayPageId[i] + globalVar.savingOverlayLabel[i] + "\nsource: " + globalVar.savingOverlaySourceURL[i] + "\nbounds: " + globalVar.savingOverlayBounds[i] + "\nrotation: " + globalVar.savingOverlayRotation[i]);
                        createSavedOverlay("apply", globalVar.savingOverlayPageId[i], globalVar.savingOverlayLabel[i], globalVar.savingOverlaySourceURL[i], globalVar.savingOverlayBounds[i], globalVar.savingOverlayRotation[i]); //send overlay to the server
                        if (globalVar.toServerSuccess == true) {
                            displayMessage(L_Applied);
                        }
                    }
                    //reset first save
                    globalVar.firstSaveOverlay = true;
                } else {
                    displayMessage(L_NotSaved);
                }

                //reset apply button to save
                document.getElementById("content_toolbox_button_saveOverlay").value = L37;
                document.getElementById("content_toolbox_button_saveOverlay").title = L38;
            }
            break;

        case "poi":
            //is this the first time saving a changed item? (apply changes)
            if (globalVar.firstSavePOI == true) {
                //determine if there is something to save
                if (globalVar.poiObj.length > 0) {
                    //save to temp xml file
                    de("saving " + globalVar.poiObj.length + " POIs...");
                    createSavedPOI("save");
                    if (globalVar.toServerSuccess == true) {
                        displayMessage(L_Saved);
                    }
                    //explicitly turn off the drawing manager 
                    google.map.drawingManager.setDrawingMode(null);
                    google.map.drawingManager.setMap(null);
                    //reset first save
                    //globalVar.firstSavePOI = false;
                    //change save button to apply button
                    //document.getElementById("content_toolbox_button_savePOI").value = L36;
                    //change save title to apply
                    //document.getElementById("content_toolbox_button_savePOI").title = L35;
                } else {
                    //tell that we did not save anything
                    displayMessage(L_NotSaved);
                }
            } else {
                if (globalVar.firstSavePOI == false) {
                    //is there something to save?
                    if (globalVar.poiObj.length > 0) {
                        //apply the changes
                        de("Applying Changes...");
                        de("applying " + globalVar.poiObj.length + " POIs...");
                        //apply changes
                        createSavedPOI("apply");
                        if (globalVar.toServerSuccess == true) {
                            displayMessage(L_Applied);
                        }
                        //reset first save
                        globalVar.firstSavePOI = true;
                    } else {
                        displayMessage(L_NotSaved);
                    }
                    //reset apply button to save
                    document.getElementById("content_toolbox_button_savePOI").value = L37;
                    document.getElementById("content_toolbox_button_savePOI").title = L38;
                }
            }
            break;
    }
}

//clear handler
function clear(id) {
    switch (id) {
        case "item":
            if (globalVar.savingMarkerCenter != null) {
                //clear the current marker
                globalVar.itemMarker.setMap(null); //delete marker form map
                globalVar.itemMarker = null; //delete reference to marker
                globalVar.savingMarkerCenter = null; //reset stored coords to save
                document.getElementById('content_toolbox_posItem').value = ""; //reset lat/long in tab
                document.getElementById('content_toolbox_rgItem').value = ""; //reset address in tab
                //redraw incoming marker
                displayIncomingPoints();
                displayMessage(L9); //say all is reset
            } else {
                displayMessage(L_NotCleared);
            }

            break;

        case "overlay":
            if ((globalVar.workingOverlayIndex != null) || (globalVar.overlayCount != globalVar.overlaysOnMap.length)) {
                //delete all incoming overlays
                displayMessage(localize.L52);
                clearIncomingOverlays();
                //show all the incoming overlays
                displayIncomingPolygons();
                //redraw list items of overlays
                initOverlayList();
                //clear the save cache
                clearCacheSaveOverlay();
                //reset edit mode
                place("overlay");
                //say we are finished
                displayMessage(L10);
            } else {
                displayMessage(L46);
            }

            break;

        case "poi":
            de("attempting to clear " + globalVar.poiObj.length + "POIs...");
            if (globalVar.poiObj.length > 0) {
                displayMessage(localize.L53);
                for (var i = 0; i < globalVar.poiObj.length; i++) {
                    if (globalVar.poiObj[i] != null) {
                        globalVar.poiObj[i].setMap(null);
                        globalVar.poiObj[i] = null;
                    }
                    if (globalVar.poiDesc[i] != null) {
                        globalVar.poiDesc[i] = null;
                    }
                    if (globalVar.poiKML[i] != null) {
                        globalVar.poiKML[i] = null;
                    }
                    infoWindow[i].setMap(null);
                    infoWindow[i] = null;
                    label[i].setMap(null);
                    label[i] = null;
                    var strg = "#poi" + i; //create <li> poi string
                    $(strg).remove(); //remove <li>
                }
                globalVar.poiObj = [];
                globalVar.poi_i = -1;

                displayMessage(L11);
            } else {
                displayMessage(L_NotCleared);
            }

            break;
    }
}

//#endregion


//Gmap Support

//#region main google functions

///<summary>Setups everything with user defined options</summary>
///<param name="collection" type="string">Specify the type of collection to load</param>
function setupInterface(collection) {
    ///<summary>Setups everything with user defined options</summary>
    ///<param name="collection" type="string">Specify the type of collection to load</param>
    //todo make this auto generated  

    google.maps.visualRefresh = true; //Enable the visual refresh (new gmaps)

    switch (collection) {
        case "default":
            globalVar.baseImageDirURL = "default/images/mapedit/";                            //the default directory to the image files
            globalVar.mapLayerActive = "Roadmap";                                             //what map layer is displayed
            globalVar.mapDrawingManagerDisplayed = false;                                     //by default, is the drawing manager displayed (true/false)
            globalVar.mapCenter = new google.maps.LatLng(29.6480, -82.3482);                  //used to center map on load
            globalVar.mapControlsDisplayed = true;                                            //by default, are map controls displayed (true/false)
            globalVar.defaultDisplayDrawingMangerTool = false;                                //by default, is the drawingmanger displayed (true/false)
            globalVar.toolboxDisplayed = true;                                                //by default, is the toolbox displayed (true/false)
            globalVar.toolbarDisplayed = true;                                                //by default, is the toolbar open (yes/no)
            globalVar.kmlDisplayed = false;                                                   //by default, is kml layer on (yes/no)
            KmlLayer = new google.maps.KmlLayer("http://ufdc.ufl.edu/design/mapedit/stAugParel_v6.kmz");  //must be pingable by google
            globalVar.defaultZoomLevel = 13;                                                  //zoom level, starting
            globalVar.maxZoomLevel = 2;                                                       //max zoom out, default (21=lowest level, 1=highest level)
            globalVar.minZoomLevel_Terrain = 15;                                              //max zoom in, terrain
            globalVar.minZoomLevel_Satellite = 20;                                            //max zoom in, sat + hybrid
            globalVar.minZoomLevel_Roadmap = 21;                                              //max zoom in, roadmap (default)
            globalVar.minZoomLevel_BlockLot = 19;                                             //max zoom in, used for special layers not having default of roadmap
            globalVar.isCustomOverlay = false;                                                //used to determine if other overlays (block/lot etc) //unknown
            globalVar.preservedRotation = 0;                                                  //rotation, default
            globalVar.knobRotationValue = 0;                                                  //rotation to display by default 
            globalVar.preservedOpacity = 0.75;                                                 //opacity, default value (0-1,1=opaque)
            globalVar.strictBounds = null;                                                    //set the bounds for this google map instance (set to null for no bounds)
            globalVar.hasCustomMapType = true;                                                //used to determine if there is a custom maptype layer
            break;
        case "stAugustine":
            globalVar.baseImageDirURL = "default/images/mapedit/";                            //the default directory to the image files
            globalVar.mapDrawingManagerDisplayed = false;                                     //by default, is the drawing manager displayed (true/false)
            globalVar.mapLayerActive = "Roadmap";                                             //what map layer is displayed
            globalVar.mapCenter = new google.maps.LatLng(29.8944, -81.3147);                  //used to center map on load
            globalVar.mapControlsDisplayed = true;                                            //by default, are map controls displayed (true/false)
            globalVar.defaultDisplayDrawingMangerTool = false;                                //by default, is the drawingmanger displayed (true/false)
            globalVar.toolboxDisplayed = true;                                                //by default, is the toolbox displayed (true/false)
            globalVar.toolbarDisplayed = true;                                                //by default, is the toolbar open (yes/no)
            globalVar.kmlDisplayed = false;                                                   //by default, is kml layer on (yes/no)
            //KmlLayer = new google.maps.KmlLayer("http://ufdc.ufl.edu/design/mapedit/stAugParcel_v6.kmz");  //must be pingable by google
            KmlLayer = new google.maps.KmlLayer("http://ufdc.ufl.edu/design/mapedit/stAugParcel_v6.kmz");  //must be pingable by google
            globalVar.defaultZoomLevel = 14;                                                  //zoom level, starting
            globalVar.maxZoomLevel = 10;                                                      //max zoom out, default (21=lowest level, 1=highest level)
            globalVar.minZoomLevel_Terrain = 15;                                              //max zoom in, terrain
            globalVar.minZoomLevel_Satellite = 20;                                            //max zoom in, sat + hybrid
            globalVar.minZoomLevel_Roadmap = 21;                                              //max zoom in, roadmap (default)
            globalVar.minZoomLevel_BlockLot = 19;                                             //max zoom in, used for special layers not having default of roadmap
            globalVar.isCustomOverlay = false;                                                //used to determine if other overlays (block/lot etc) //unknown
            globalVar.preservedRotation = 0;                                                  //rotation, default
            globalVar.knobRotationValue = 0;                                                  //rotation to display by default 
            globalVar.preservedOpacity = 0.35;                                                 //opacity, default value (0-1,1=opaque)
            globalVar.hasCustomMapType = true;                                                //used to determine if there is a custom maptype layer
            globalVar.strictBounds = new google.maps.LatLngBounds(                            //set the bounds for this google map instance
                new google.maps.LatLng(29.78225755812941, -81.4306640625),
                new google.maps.LatLng(29.99181288866604, -81.1917114257)
            );
            break;
        case "custom":
            globalVar.baseImageDirURL = "default/images/mapedit/";                            //the default directory to the image files
            globalVar.mapDrawingManagerDisplayed = false;                                     //by default, is the drawing manager displayed (true/false)
            globalVar.mapLayerActive = "Roadmap";                                             //what map layer is displayed
            globalVar.mapCenter = new google.maps.LatLng(29.6480, -82.3482);                  //used to center map on load
            globalVar.mapControlsDisplayed = true;                                            //by default, are map controls displayed (true/false)
            globalVar.defaultDisplayDrawingMangerTool = false;                                //by default, is the drawingmanger displayed (true/false)
            globalVar.toolboxDisplayed = true;                                                //by default, is the toolbox displayed (true/false)
            globalVar.toolbarDisplayed = true;                                                //by default, is the toolbar open (yes/no)
            globalVar.kmlDisplayed = false;                                                   //by default, is kml layer on (yes/no)
            KmlLayer = new google.maps.KmlLayer("http://ufdc.ufl.edu/design/mapedit/parcels_2012_kmz_fldor.kmz");  //must be pingable by google
            globalVar.defaultZoomLevel = 13;                                                  //zoom level, starting
            globalVar.maxZoomLevel = 10;                                                      //max zoom out, default (21=lowest level, 1=highest level)
            globalVar.minZoomLevel_Terrain = 15;                                              //max zoom in, terrain
            globalVar.minZoomLevel_Satellite = 20;                                            //max zoom in, sat + hybrid
            globalVar.minZoomLevel_Roadmap = 21;                                              //max zoom in, roadmap (default)
            globalVar.minZoomLevel_BlockLot = 19;                                             //max zoom in, used for special layers not having default of roadmap
            globalVar.isCustomOverlay = false;                                                //used to determine if other overlays (block/lot etc) //unknown
            globalVar.preservedRotation = 0;                                                  //rotation, default
            globalVar.knobRotationValue = 0;                                                  //rotation to display by default 
            globalVar.preservedOpacity = 0.75;                                                 //opacity, default value (0-1,1=opaque)
            globalVar.hasCustomMapType = true;                                                //used to determine if there is a custom maptype layer
            globalVar.strictBounds = new google.maps.LatLngBounds(                            //set the bounds for this google map instance
                new google.maps.LatLng(29.21570636285318, -82.87811279296875),
                new google.maps.LatLng(30.07978967039041, -81.76300048828125)
            );
            break;
        case "florida":
            globalVar.baseImageDirURL = "default/images/mapedit/";                            //the default directory to the image files
            globalVar.mapDrawingManagerDisplayed = false;                                     //by default, is the drawing manager displayed (true/false)
            globalVar.mapLayerActive = "Roadmap";                                             //what map layer is displayed
            globalVar.mapCenter = new google.maps.LatLng(29.6480, -82.3482);                  //used to center map on load
            globalVar.mapControlsDisplayed = true;                                            //by default, are map controls displayed (true/false)
            globalVar.defaultDisplayDrawingMangerTool = false;                                //by default, is the drawingmanger displayed (true/false)
            globalVar.toolboxDisplayed = true;                                                //by default, is the toolbox displayed (true/false)
            globalVar.toolbarDisplayed = true;                                                //by default, is the toolbar open (yes/no)
            globalVar.kmlDisplayed = false;                                                   //by default, is kml layer on (yes/no)
            KmlLayer = new google.maps.KmlLayer("http://hlmatt.com/uf/kml/10.kml"); //must be pingable by google
            globalVar.defaultZoomLevel = 13;                                                  //zoom level, starting
            globalVar.maxZoomLevel = 1;                                                       //max zoom out, default (21=lowest level, 1=highest level)
            globalVar.minZoomLevel_Terrain = 15;                                              //max zoom in, terrain
            globalVar.minZoomLevel_Satellite = 20;                                            //max zoom in, sat + hybrid
            globalVar.minZoomLevel_Roadmap = 21;                                              //max zoom in, roadmap (default)
            globalVar.minZoomLevel_BlockLot = 19;                                             //max zoom in, used for special layers not having default of roadmap
            globalVar.isCustomOverlay = false;                                                //used to determine if other overlays (block/lot etc) 
            globalVar.preservedRotation = 0;                                                  //rotation, default
            globalVar.knobRotationValue = 0;                                                  //rotation to display by default 
            globalVar.preservedOpacity = 0.75;                                                 //opacity, default value (0-1,1=opaque)
            globalVar.hasCustomMapType = true;                                                //used to determine if there is a custom maptype layer
            globalVar.strictBounds = new google.maps.LatLngBounds(                            //set the bounds for this google map instance
                //new google.maps.LatLng(30.69420636285318, -88.04311279296875), //fl nw
                //new google.maps.LatLng(25.06678967039041, -77.33330048828125) //fl se
                //new google.maps.LatLng(24.55531738915811, -81.78283295288095), //fl sw
                //new google.maps.LatLng(30.79109834517092, -81.53709923706058) //fl ne
                //new google.maps.LatLng(29.5862, -82.4146), //gville
                //new google.maps.LatLng(29.7490, -82.2106)
                new google.maps.LatLng(22.053908635225607, -86.18838838405613), //east coast
                new google.maps.LatLng(36.06512404320089, -76.72320000000003)
            );

            //globalVar.strictBounds = new google.maps.LatLngBounds(                            //set the bounds for this google map instance
            //    new google.maps.LatLng(30.69420636285318, -88.04311279296875),
            //    new google.maps.LatLng(25.06678967039041, -77.33330048828125)
            //);
            break;
    }
}

//on page load functions (mainly google map event listeners)
function initialize() {

    //as map is loading, fit to screen
    resizeView();

    //initialize google map objects
    map = new google.maps.Map(document.getElementById(gmapPageDivId), gmapOptions);                             //initialize map    
    map.controls[google.maps.ControlPosition.RIGHT_BOTTOM].push(copyrightNode);                                 //initialize custom copyright
    map.controls[google.maps.ControlPosition.BOTTOM_LEFT].push(cursorLatLongTool);                              //initialize cursor lat long tool
    map.controls[google.maps.ControlPosition.TOP_LEFT].push(toolbarBufferZone1);                                //initialize spacer
    map.controls[google.maps.ControlPosition.TOP_RIGHT].push(toolbarBufferZone2);                               //intialize spacer
    drawingManager.setMap(map);                                                                                 //initialize drawing manager
    drawingManager.setMap(null);                                                                                //initialize drawing manager (hide)
    geocoder = new google.maps.Geocoder();                                                                      //initialize geocoder

    //#region Google Specific Listeners  

    //initialize drawingmanger listeners
    google.maps.event.addListener(drawingManager, 'markercomplete', function (marker) {
        
        testBounds(); //are we still in the bounds 
        if (globalVar.placerType == "item") {
            globalVar.firstSaveItem = true;
            //used to prevent multi markers
            if (globalVar.firstMarker > 0) {
                drawingManager.setDrawingMode(null); //only place one at a time
            } else {
                globalVar.firstMarker++;
                drawingManager.setDrawingMode(null); //only place one at a time
            }
            globalVar.itemMarker = marker; //assign globally
            de("marker placed");
            document.getElementById('content_toolbox_posItem').value = globalVar.itemMarker.getPosition();
            globalVar.savingMarkerCenter = globalVar.itemMarker.getPosition(); //store coords to save
            codeLatLng(globalVar.itemMarker.getPosition());
        }

        if (globalVar.placerType == "poi") {
            globalVar.firstSavePOI = true;
            globalVar.poi_i++;

            label[globalVar.poi_i] = new MarkerWithLabel({
                position: marker.getPosition(), //position of real marker
                map: map,
                zIndex: 2,
                labelContent: globalVar.poi_i + 1, //the current user count
                labelAnchor: new google.maps.Point(15, 0),
                labelClass: "labels", // the CSS class for the label
                labelStyle: { opacity: 0.75 },
                icon: {} //initialize to nothing so no marker shows
            });

            globalVar.poiObj[globalVar.poi_i] = marker;
            globalVar.poiType[globalVar.poi_i] = "marker";
            var poiId = globalVar.poi_i + 1;
            var poiDescTemp = L_Marker;
            document.getElementById("poiList").innerHTML += writeHTML("poiListItem", globalVar.poi_i, poiId, poiDescTemp);
            var contentString = writeHTML("poiDesc", globalVar.poi_i, "", "");
            infoWindow[globalVar.poi_i] = new google.maps.InfoWindow({
                content: contentString,
                position: marker.getPosition()
                //pixelOffset: new google.maps.Size(0, -40)
            });

            infoWindow[globalVar.poi_i].setMap(map);
            
            infoWindow[globalVar.poi_i].open(map, globalVar.poiObj[globalVar.poi_i]);

            de("poiCount: " + globalVar.poiCount);

            //best fix so far
            if (globalVar.poiCount == 0) {
                setTimeout(function () {
                    infoWindow[0].setMap(null);
                    infoWindow[0].setOptions({ pixelOffset: new google.maps.Size(0, -40) });
                    infoWindow[0].setMap(map);
                }, 800);
            }

            //if (globalVar.poiCount > 0) {
            //    infoWindow[globalVar.poi_i].open(map);
            //} else {
            //    infoWindow[globalVar.poi_i].setMap(map);
            //    infoWindow[globalVar.poi_i].setMap(null);
            //    de("platform: " + navigator.platform);
            //    if (navigator.platform)
            //    var t2 = setTimeout(function () {
            //        infoWindow[globalVar.poi_i].setMap(map);
            //    }, 1500);
            //}
            
            globalVar.poiCount++;
            
            ////try to fix first poi infobox issue
            //de("poi count: " + globalVar.poiCount);
            //de("tempYo" + globalVar.tempYo);
            //if (globalVar.tempYo == false) {
            //    var t = setTimeout(function () {
            //        de("fire infowindow");
                    
            //        var contentString = writeHTML("poiDesc", globalVar.poi_i, "", "");
            //        infoWindow[globalVar.poi_i] = new google.maps.InfoWindow({
            //            content: contentString,
            //            position: marker.getPosition(),
            //            pixelOffset: new google.maps.Size(0, -40)
            //        });

            //        infoWindow[globalVar.poi_i].setMap(map);
                    
            //        poiHideMe(0);
                    
            //        var t2 = setTimeout(function() {

            //            poiShowMe(0);
                        
            //        }, 1500);

            //    }, 500);
                
            //    globalVar.tempYo = true;
                
            //} else {
                
            //    var contentString = writeHTML("poiDesc", globalVar.poi_i, "", "");
            //    infoWindow[globalVar.poi_i] = new google.maps.InfoWindow({
            //        content: contentString,
            //        position: marker.getPosition(),
            //        pixelOffset: new google.maps.Size(0, -40)
            //    });

            //    infoWindow[globalVar.poi_i].setMap(map);
            //}
            
        }

        google.maps.event.addListener(marker, 'dragstart', function () {

            if (globalVar.placerType == "poi") {
                globalVar.firstSavePOI = true;
                for (var i = 0; i < globalVar.poiObj.length; i++) {
                    if (globalVar.poiObj[i] == this) {
                        infoWindow[i].setMap(null);
                        label[i].setMap(null);
                    }
                }
            }
        });
        google.maps.event.addListener(marker, 'dragend', function () {
            if (globalVar.placerType == "item") {
                globalVar.firstSaveItem = true;
                document.getElementById('content_toolbox_posItem').value = marker.getPosition();
                codeLatLng(marker.getPosition());
            }
            if (globalVar.placerType == "poi") {
                globalVar.firstSavePOI = true;
                for (var i = 0; i < globalVar.poiObj.length; i++) {
                    if (globalVar.poiObj[i] == this) {
                        infoWindow[i].setOptions({ position: marker.getPosition(), pixelOffset: new google.maps.Size(0, -40) });
                        infoWindow[i].open(null);
                        label[i].setPosition(marker.getPosition());
                        label[i].setMap(map);
                    }
                }
            }
        });
        google.maps.event.addListener(marker, 'click', function () {
            if (globalVar.placerType == "poi") {
                globalVar.firstSavePOI = true;
                for (var i = 0; i < globalVar.poiObj.length; i++) {
                    if (globalVar.poiObj[i] == this) {
                        infoWindow[i].setOptions({ position: marker.getPosition(), pixelOffset: new google.maps.Size(0, -40) });
                        infoWindow[i].open(map);
                    }
                }
            }
        });
    });
    google.maps.event.addListener(drawingManager, 'circlecomplete', function (circle) {
        testBounds();
        if (globalVar.placerType == "poi") {
            globalVar.firstSavePOI = true;
            globalVar.poi_i++;

            label[globalVar.poi_i] = new MarkerWithLabel({
                position: circle.getCenter(), //position of real marker
                zIndex: 2,
                map: map,
                labelContent: globalVar.poi_i + 1, //the current user count
                labelAnchor: new google.maps.Point(15, 0),
                labelClass: "labels", // the CSS class for the label
                labelStyle: { opacity: 0.75 },
                icon: {} //initialize to nothing so no marker shows
            });

            var poiId = globalVar.poi_i + 1;
            globalVar.poiObj[globalVar.poi_i] = circle;
            globalVar.poiType[globalVar.poi_i] = "circle";
            var poiDescTemp = L_Circle;
            document.getElementById("poiList").innerHTML += writeHTML("poiListItem", globalVar.poi_i, poiId, poiDescTemp);
            var contentString = writeHTML("poiDesc", globalVar.poi_i, "", "");
            infoWindow[globalVar.poi_i] = new google.maps.InfoWindow({
                content: contentString
            });
            infoWindow[globalVar.poi_i].setPosition(circle.getCenter());
            infoWindow[globalVar.poi_i].open(map);
            //best fix so far
            if (globalVar.poiCount == 0) {
                setTimeout(function () {
                    infoWindow[0].setMap(null);
                    //infoWindow[0].setOptions({ pixelOffset: new google.maps.Size(0, -40) });
                    infoWindow[0].setMap(map);
                }, 800);
            }
            //if (globalVar.poiCount > 0) {
            //    infoWindow[globalVar.poi_i].open(map);
            //} else {
            //    infoWindow[globalVar.poi_i].setMap(map);
            //    infoWindow[globalVar.poi_i].setMap(null);
            //    infoWindow[globalVar.poi_i].setMap(map);
            //}
            globalVar.poiCount++;
        }
        google.maps.event.addListener(circle, 'dragstart', function () {

            if (globalVar.placerType == "poi") {
                globalVar.firstSavePOI = true;
                for (var i = 0; i < globalVar.poiObj.length; i++) {
                    if (globalVar.poiObj[i] == this) {
                        infoWindow[i].setMap(null);
                        label[i].setMap(null);
                    }
                }
            }
        });
        google.maps.event.addListener(circle, 'drag', function () {
            //used to get the center point for lat/long tool
            globalVar.circleCenter = circle.getCenter();
            var str = circle.getCenter().toString();
            var cLatV = str.replace("(", "").replace(")", "").split(",", 1);
            var cLongV = str.replace(cLatV, "").replace("(", "").replace(")", "").replace(",", ""); //is this better than passing into array?s
            if (cLatV.indexOf("-") != 0) {
                latH = "N";
            } else {
                latH = "S";
            }
            if (cLongV.indexOf("-") != 0) {
                longH = "W";
            } else {
                longH = "E";
            }
            cLat.innerHTML = cLatV + " (" + latH + ")";
            cLong.innerHTML = cLongV + " (" + longH + ")";
        });
        google.maps.event.addListener(circle, 'dragend', function () {
            if (globalVar.placerType == "poi") {
                globalVar.firstSavePOI = true;
                for (var i = 0; i < globalVar.poiObj.length; i++) {
                    if (globalVar.poiObj[i] == this) {
                        infoWindow[i].setPosition(circle.getCenter());
                        infoWindow[i].open(null);
                        label[i].setPosition(circle.getCenter());
                        label[i].setMap(map);
                    }
                }
            }
        });
        google.maps.event.addListener(circle, 'click', function () {
            if (globalVar.placerType == "poi") {
                globalVar.firstSavePOI = true;
                for (var i = 0; i < globalVar.poiObj.length; i++) {
                    if (globalVar.poiObj[i] == this) {
                        infoWindow[i].setPosition(circle.getCenter());
                        infoWindow[i].open(map);
                    }
                }
            }
        });
    });
    google.maps.event.addListener(drawingManager, 'rectanglecomplete', function (rectangle) {
        //check the bounds to make sure you havent strayed too far away
        testBounds();
        if (globalVar.placerType == "overlay") {
            de("placertype: overlay");
            //assing working overlay index
            globalVar.workingOverlayIndex = globalVar.convertedOverlayIndex + 1;
            if (globalVar.overlayType == "drawn") {
                de("globalVar.overlayType: " + globalVar.overlayType);
                de("globalVar.convertedOverlayIndex: " + globalVar.convertedOverlayIndex);
                globalVar.incomingPolygonPath[globalVar.convertedOverlayIndex] = rectangle.getBounds();
                //create overlay with incoming
                globalVar.overlaysOnMap[globalVar.incomingPolygonPageId[globalVar.convertedOverlayIndex]] = new CustomOverlay(globalVar.incomingPolygonPageId[globalVar.convertedOverlayIndex], globalVar.incomingPolygonPath[globalVar.convertedOverlayIndex], globalVar.incomingPolygonSourceURL[globalVar.convertedOverlayIndex], map, globalVar.incomingPolygonRotation[globalVar.convertedOverlayIndex]);
                globalVar.currentlyEditing = "no";
                //set the overlay to the map
                globalVar.overlaysOnMap[globalVar.incomingPolygonPageId[globalVar.convertedOverlayIndex]].setMap(map);
                //set hotspot on top of overlay
                setGhostOverlay(globalVar.incomingPolygonPageId[globalVar.convertedOverlayIndex], globalVar.incomingPolygonPath[globalVar.convertedOverlayIndex]);
                de("I created ghost: " + globalVar.incomingPolygonPageId[globalVar.convertedOverlayIndex]);
                globalVar.mainCount++;
                globalVar.incomingACL = "overlay";
                //mark that we converted it
                globalVar.isConvertedOverlay = true;
                //hide the rectangle we drew
                rectangle.setMap(null);
                //relist the overlay we drew
                initOverlayList();
                overlayEditMe(globalVar.incomingPolygonPageId[globalVar.convertedOverlayIndex]);
                ////show ghost
                //globalVar.ghostOverlayRectangle[globalVar.convertedOverlayIndex].setOptions(globalVar.editable);
                ////recenter on the overlay
                //overlayCenterOnMe(globalVar.convertedOverlayIndex);
                //reset drawing manager no matter what
                drawingManager.setDrawingMode(null);
            } else {
                //mark that we are editing
                globalVar.firstSaveOverlay = true;

                //add the incoming overlay bounds
                globalVar.incomingPolygonPath[globalVar.convertedOverlayIndex] = rectangle.getBounds();

                //redisplay overlays (the one we just made)
                displayIncomingPolygons();

                //relist the overlay we drew
                initOverlayList();

                //hide the rectangle we drew
                rectangle.setMap(null);

                //prevent redraw
                drawingManager.setDrawingMode(null);
            }
            //create cache save overlay for the new converted overlay only
            if (globalVar.isConvertedOverlay == true) {
                cacheSaveOverlay(globalVar.workingOverlayIndex);
                globalVar.isConvertedOverlay = false;
            }
        }
        if (globalVar.placerType == "poi") {
            globalVar.firstSavePOI = true;
            globalVar.poi_i++;

            label[globalVar.poi_i] = new MarkerWithLabel({
                position: rectangle.getBounds().getCenter(), //position of real marker
                zIndex: 2,
                map: map,
                labelContent: globalVar.poi_i + 1, //the current user count
                labelAnchor: new google.maps.Point(15, 0),
                labelClass: "labels", // the CSS class for the label
                labelStyle: { opacity: 0.75 },
                icon: {} //initialize to nothing so no marker shows
            });

            var poiId = globalVar.poi_i + 1;
            globalVar.poiObj[globalVar.poi_i] = rectangle;
            globalVar.poiType[globalVar.poi_i] = "rectangle";
            var poiDescTemp = L_Rectangle;
            document.getElementById("poiList").innerHTML += writeHTML("poiListItem", globalVar.poi_i, poiId, poiDescTemp);
            var contentString = writeHTML("poiDesc", globalVar.poi_i, "", "");
            infoWindow[globalVar.poi_i] = new google.maps.InfoWindow({
                content: contentString
            });
            infoWindow[globalVar.poi_i].setPosition(rectangle.getBounds().getCenter());
            infoWindow[globalVar.poi_i].open(map);
            //best fix so far
            if (globalVar.poiCount == 0) {
                setTimeout(function () {
                    infoWindow[0].setMap(null);
                    //infoWindow[0].setOptions({ pixelOffset: new google.maps.Size(0, -40) });
                    infoWindow[0].setMap(map);
                }, 800);
            }
            //if (globalVar.poiCount > 0) {
            //    infoWindow[globalVar.poi_i].open(map);
            //} else {
            //    infoWindow[globalVar.poi_i].setMap(map);
            //    infoWindow[globalVar.poi_i].setMap(null);
            //    infoWindow[globalVar.poi_i].setMap(map);
            //}
            globalVar.poiCount++;
            de("completed overlay bounds getter");
        }

        google.maps.event.addListener(rectangle, 'bounds_changed', function () {
            if (globalVar.placerType == "poi") {
                globalVar.firstSavePOI = true;
                for (var i = 0; i < globalVar.poiObj.length; i++) {
                    if (globalVar.poiObj[i] == this) {
                        infoWindow[i].setPosition(rectangle.getBounds().getCenter());
                        infoWindow[i].setMap(null);
                        label[i].setPosition(rectangle.getBounds().getCenter());
                        label[i].setMap(map);
                    }
                }
            }
        });
        google.maps.event.addListener(rectangle, 'dragstart', function () {
            if (globalVar.placerType == "poi") {
                globalVar.firstSavePOI = true;
                for (var i = 0; i < globalVar.poiObj.length; i++) {
                    if (globalVar.poiObj[i] == this) {
                        infoWindow[i].setMap(null);
                        label[i].setMap(null);
                    }
                }
            }
        });
        google.maps.event.addListener(rectangle, 'drag', function () {
            if (globalVar.placerType == "poi") {
                globalVar.firstSavePOI = true;
                for (var i = 0; i < globalVar.poiObj.length; i++) {
                    if (globalVar.poiObj[i] == this) {
                        infoWindow[i].setMap(null);
                        label[i].setMap(null);
                    }
                }
            }
            //used to get center point for lat/long tool
            var str = rectangle.getBounds().getCenter().toString();
            var cLatV = str.replace("(", "").replace(")", "").split(",", 1);
            var cLongV = str.replace(cLatV, "").replace("(", "").replace(")", "").replace(",", "");
            if (cLatV.indexOf("-") != 0) {
                latH = "N";
            } else {
                latH = "S";
            }
            if (cLongV.indexOf("-") != 0) {
                longH = "W";
            } else {
                longH = "E";
            }
            cLat.innerHTML = cLatV + " (" + latH + ")";
            cLong.innerHTML = cLongV + " (" + longH + ")";
        });
        google.maps.event.addListener(rectangle, 'dragend', function () {
            if (globalVar.placerType == "poi") {
                globalVar.firstSavePOI = true;
                for (var i = 0; i < globalVar.poiObj.length; i++) {
                    if (globalVar.poiObj[i] == this) {
                        infoWindow[i].setPosition(rectangle.getBounds().getCenter());
                        infoWindow[i].open(null);
                        label[i].setPosition(rectangle.getBounds().getCenter());
                        label[i].setMap(map);
                    }
                }
            }
        });
        google.maps.event.addListener(rectangle, 'click', function () {
            if (globalVar.placerType == "poi") {
                globalVar.firstSavePOI = true;
                for (var i = 0; i < globalVar.poiObj.length; i++) {
                    if (globalVar.poiObj[i] == this) {
                        infoWindow[i].setPosition(rectangle.getBounds().getCenter());
                        infoWindow[i].open(map);
                    }
                }
            }
        });
    });
    google.maps.event.addListener(drawingManager, 'polygoncomplete', function (polygon) {
        testBounds();
        if (globalVar.placerType == "poi") {
            globalVar.firstSavePOI = true;
            globalVar.poi_i++;

            label[globalVar.poi_i] = new MarkerWithLabel({
                position: polygonCenter(polygon), //position of real marker
                zIndex: 2,
                map: map,
                labelContent: globalVar.poi_i + 1, //the current user count
                labelAnchor: new google.maps.Point(15, 0),
                labelClass: "labels", // the CSS class for the label
                labelStyle: { opacity: 0.75 },
                icon: {} //initialize to nothing so no marker shows
            });

            var poiId = globalVar.poi_i + 1;
            globalVar.poiObj[globalVar.poi_i] = polygon;
            globalVar.poiType[globalVar.poi_i] = "polygon";
            var poiDescTemp = L_Polygon;
            document.getElementById("poiList").innerHTML += writeHTML("poiListItem", globalVar.poi_i, poiId, poiDescTemp);
            var contentString = writeHTML("poiDesc", globalVar.poi_i, "", "");
            infoWindow[globalVar.poi_i] = new google.maps.InfoWindow({
                content: contentString
            });
            infoWindow[globalVar.poi_i].setPosition(polygonCenter(polygon));
            infoWindow[globalVar.poi_i].open(map);
            //best fix so far
            if (globalVar.poiCount == 0) {
                setTimeout(function () {
                    infoWindow[0].setMap(null);
                    //infoWindow[0].setOptions({ pixelOffset: new google.maps.Size(0, -40) });
                    infoWindow[0].setMap(map);
                }, 800);
            }
            //if (globalVar.poiCount > 0) {
            //    infoWindow[globalVar.poi_i].open(map);
            //} else {
            //    infoWindow[globalVar.poi_i].setMap(map);
            //    infoWindow[globalVar.poi_i].setMap(null);
            //    infoWindow[globalVar.poi_i].setMap(map);
            //}
            globalVar.poiCount++;
        }
        google.maps.event.addListener(polygon.getPath(), 'set_at', function () { //if bounds change
            if (globalVar.placerType == "poi") {
                globalVar.firstSavePOI = true;
                for (var i = 0; i < globalVar.poiObj.length; i++) {
                    if (globalVar.poiObj[i] == this) {
                        infoWindow[i].setPosition(polygonCenter(polygon));
                        infoWindow[i].setMap(null);
                        label[i].setPosition(polygonCenter(polygon));
                        label[i].setMap(map); //does not redisplay
                    }
                }
            }
        });
        google.maps.event.addListener(polygon, 'dragstart', function () {
            if (globalVar.placerType == "poi") {
                globalVar.firstSavePOI = true;
                for (var i = 0; i < globalVar.poiObj.length; i++) {
                    if (globalVar.poiObj[i] == this) {
                        infoWindow[i].setMap(null);
                        label[i].setMap(null);
                    }
                }
            }
        });
        google.maps.event.addListener(polygon, 'drag', function () {
            if (globalVar.placerType == "poi") {
                globalVar.firstSavePOI = true;
                for (var i = 0; i < globalVar.poiObj.length; i++) {
                    if (globalVar.poiObj[i] == this) {
                        infoWindow[i].setMap(null);
                        label[i].setMap(null);
                    }
                }
            }
            //used for lat/long tool
            var str = polygonCenter(polygon).toString();
            var cLatV = str.replace("(", "").replace(")", "").split(",", 1);
            var cLongV = str.replace(cLatV, "").replace("(", "").replace(")", "").replace(",", ""); //is this better than passing into array?s
            if (cLatV.indexOf("-") != 0) {
                latH = "N";
            } else {
                latH = "S";
            }
            if (cLongV.indexOf("-") != 0) {
                longH = "W";
            } else {
                longH = "E";
            }
            cLat.innerHTML = cLatV + " (" + latH + ")";
            cLong.innerHTML = cLongV + " (" + longH + ")";
        });
        google.maps.event.addListener(polygon, 'dragend', function () {
            if (globalVar.placerType == "poi") {
                globalVar.firstSavePOI = true;
                for (var i = 0; i < globalVar.poiObj.length; i++) {
                    if (globalVar.poiObj[i] == this) {
                        infoWindow[i].setPosition(polygonCenter(polygon));
                        infoWindow[i].open(null);
                        label[i].setPosition(polygonCenter(polygon));
                        label[i].setMap(map);
                    }
                }
            }
        });
        google.maps.event.addListener(polygon, 'click', function () {
            if (globalVar.placerType == "poi") {
                globalVar.firstSavePOI = true;
                for (var i = 0; i < globalVar.poiObj.length; i++) {
                    if (globalVar.poiObj[i] == this) {
                        infoWindow[i].setPosition(polygonCenter(polygon));
                        infoWindow[i].open(map);
                    }
                }
            }
        });
    });
    google.maps.event.addListener(drawingManager, 'polylinecomplete', function (polyline) {
        testBounds();
        if (globalVar.placerType == "poi") {
            globalVar.firstSavePOI = true;
            globalVar.poi_i++;
            var poiId = globalVar.poi_i + 1;
            globalVar.poiObj[globalVar.poi_i] = polyline;
            globalVar.poiType[globalVar.poi_i] = "polyline";
            var poiDescTemp = L_Line;
            document.getElementById("poiList").innerHTML += writeHTML("poiListItem", globalVar.poi_i, poiId, poiDescTemp);
            var contentString = writeHTML("poiDesc", globalVar.poi_i, "", "");
            infoWindow[globalVar.poi_i] = new google.maps.InfoWindow({
                content: contentString
            });
            //var bounds = new google.maps.LatLngBounds;
            //polyline.getPath().forEach(function (latLng) { bounds.extend(latLng); });
            //var polylineCenter = bounds.getCenter();
            //var bounds = new google.maps.LatLngBounds; //spatial center, bounds holder
            var polylinePoints = [];
            var polylinePointCount = 0;
            polyline.getPath().forEach(function (latLng) {
                polylinePoints[polylinePointCount] = latLng;
                de("polylinePoints[" + polylinePointCount + "] = " + latLng);
                polylinePointCount++;
            });
            de("polylinePointCount: " + polylinePointCount);
            de("polylinePoints.length: " + polylinePoints.length);
            de("Math.round((polylinePoints.length / 2)): " + Math.round((polylinePoints.length / 2)));
            var polylineCenterPoint = polylinePoints[Math.round((polylinePoints.length / 2))];
            de("polylineCenterPoint: " + polylineCenterPoint);
            var polylineStartPoint = polylinePoints[0];
            de("polylineStartPoint: " + polylineStartPoint);
            infoWindow[globalVar.poi_i].setPosition(polylineStartPoint);
            infoWindow[globalVar.poi_i].open(map);
            //best fix so far
            if (globalVar.poiCount == 0) {
                setTimeout(function () {
                    infoWindow[0].setMap(null);
                    //infoWindow[0].setOptions({ pixelOffset: new google.maps.Size(0, -40) });
                    infoWindow[0].setMap(map);
                }, 800);
            }
            //if (globalVar.poiCount > 0) {
            //    infoWindow[globalVar.poi_i].open(map);
            //} else {
            //    infoWindow[globalVar.poi_i].setMap(map);
            //    infoWindow[globalVar.poi_i].setMap(null);
            //    infoWindow[globalVar.poi_i].setMap(map);
            //}
            globalVar.poiCount++;

            label[globalVar.poi_i] = new MarkerWithLabel({
                //position: polylineCenter, //position of real marker
                //position: polylineCenterPoint, //position of real marker
                position: polylineStartPoint, //position at start of polyline
                zIndex: 2,
                map: map,
                labelContent: poiId, //the current user count
                labelAnchor: new google.maps.Point(15, 0),
                labelClass: "labels", // the CSS class for the label
                labelStyle: { opacity: 0.75 },
                icon: {} //initialize to nothing so no marker shows
            });

        }
        google.maps.event.addListener(polyline.getPath(), 'set_at', function () { //what is path?
            if (globalVar.placerType == "poi") {
                globalVar.firstSavePOI = true;
                de("is poi");
                for (var i = 0; i < globalVar.poiObj.length; i++) {
                    de("inside loop1");
                    if (globalVar.poiObj[i].getPath() == this) {
                        //var bounds = new google.maps.LatLngBounds;
                        //polyline.getPath().forEach(function (latLng) { bounds.extend(latLng); });
                        //var polylineCenter = bounds.getCenter();
                        //var bounds = new google.maps.LatLngBounds; //spatial center, bounds holder
                        var polylinePoints = [];
                        var polylinePointCount = 0;
                        de("here1");
                        polyline.getPath().forEach(function (latLng) {
                            polylinePoints[polylinePointCount] = latLng;
                            polylinePointCount++;
                        });
                        de("here2");
                        var polylineCenterPoint = polylinePoints[(polylinePoints.length / 2)];
                        var polylineStartPoint = polylinePoints[0];
                        infoWindow[globalVar.poi_i].setPosition(polylineStartPoint);
                        infoWindow[globalVar.poi_i].open(null);
                        label[i].setPosition(polylineStartPoint);
                        label[i].setMap(map);
                        de("here3");
                    }
                }

            }
        });
        google.maps.event.addListener(polyline, 'dragstart', function () {
            if (globalVar.placerType == "poi") {
                globalVar.firstSavePOI = true;
                for (var i = 0; i < globalVar.poiObj.length; i++) {
                    if (globalVar.poiObj[i] == this) {
                        infoWindow[i].setMap(null);
                        label[i].setMap(null);
                    }
                }
            }
        });
        google.maps.event.addListener(polyline, 'drag', function () {
            //used for lat/long tooll
            var bounds = new google.maps.LatLngBounds;
            polyline.getPath().forEach(function (latLng) { bounds.extend(latLng); });
            var polylineCenter = bounds.getCenter();
            var str = polylineCenter.toString();
            var cLatV = str.replace("(", "").replace(")", "").split(",", 1);
            var cLongV = str.replace(cLatV, "").replace("(", "").replace(")", "").replace(",", ""); //is this better than passing into array?s
            if (cLatV.indexOf("-") != 0) {
                latH = "N";
            } else {
                latH = "S";
            }
            if (cLongV.indexOf("-") != 0) {
                longH = "W";
            } else {
                longH = "E";
            }
            cLat.innerHTML = cLatV + " (" + latH + ")";
            cLong.innerHTML = cLongV + " (" + longH + ")";
        });
        google.maps.event.addListener(polyline, 'dragend', function () {
            if (globalVar.placerType == "poi") {
                globalVar.firstSavePOI = true;
                //var bounds = new google.maps.LatLngBounds;
                //polyline.getPath().forEach(function (latLng) { bounds.extend(latLng); });
                //var polylineCenter = bounds.getCenter();
                //var bounds = new google.maps.LatLngBounds; //spatial center, bounds holder

                for (var i = 0; i < globalVar.poiObj.length; i++) {
                    if (globalVar.poiObj[i] == this) {
                        var polylinePoints = [];
                        var polylinePointCount = 0;
                        polyline.getPath().forEach(function (latLng) {
                            polylinePoints[polylinePointCount] = latLng;
                            polylinePointCount++;
                        });
                        var polylineCenterPoint = polylinePoints[(polylinePoints.length / 2)];
                        var polylineStartPoint = polylinePoints[0];
                        infoWindow[i].setPosition(polylineStartPoint);
                        infoWindow[i].open(null);
                        label[i].setPosition(polylineStartPoint);
                        label[i].setMap(map);
                    }
                }

            }
        });
        google.maps.event.addListener(polyline, 'click', function () {
            if (globalVar.placerType == "poi") {
                globalVar.firstSavePOI = true;
                //var bounds = new google.maps.LatLngBounds;
                //polyline.getPath().forEach(function (latLng) { bounds.extend(latLng); });
                //var polylineCenter = bounds.getCenter();

                for (var i = 0; i < globalVar.poiObj.length; i++) {
                    if (globalVar.poiObj[i] == this) {
                        var polylinePoints = [];
                        var polylinePointCount = 0;
                        polyline.getPath().forEach(function (latLng) {
                            polylinePoints[polylinePointCount] = latLng;
                            polylinePointCount++;
                        });
                        var polylineCenterPoint = polylinePoints[(polylinePoints.length / 2)];
                        var polylineStartPoint = polylinePoints[0];
                        infoWindow[i].setPosition(polylineStartPoint);
                        infoWindow[i].open(map);
                    }
                }
            }
        });
    });

    //initialize map specific listeners

    //on right click stop drawing thing
    google.maps.event.addListener(map, 'rightclick', function () {
        drawingManager.setDrawingMode(null); //reset drawing manager no matter what
    });
    //used to display cursor location via lat/long
    google.maps.event.addDomListener(map, 'mousemove', function (point) {

        if (globalVar.cCoordsFrozen == "no") {
            //cCoord.innerHTML = point.latLng.toString(); //directly inject into page
            var str = point.latLng.toString();
            var cLatV = str.replace("(", "").replace(")", "").split(",", 1);
            var cLongV = str.replace(cLatV, "").replace("(", "").replace(")", "").replace(",", ""); //is this better than passing into array?s
            if (cLatV.indexOf("-") != 0) {
                latH = "N";
            } else {
                latH = "S";
            }
            if (cLongV.indexOf("-") != 0) {
                longH = "W";
            } else {
                longH = "E";
            }

            cLat.innerHTML = cLatV + " (" + latH + ")";
            cLong.innerHTML = cLongV + " (" + longH + ")";
        }

    });
    //drag listener (for boundary test)
    google.maps.event.addListener(map, 'dragend', function () {
        testBounds();
    });
    //check the zoom level display message if out limits
    google.maps.event.addListener(map, 'zoom_changed', function () {
        checkZoomLevel();
    });
    //when kml layer is clicked, get feature that was clicked
    google.maps.event.addListener(KmlLayer, 'click', function (kmlEvent) {
        var name = kmlEvent.featureData.name;
        displayMessage("ParcelID: " + name); //temp
    });

    //#endregion

    //initialize all the incoming geo obejects (the fcn is written via c#)
    initGeoObjects(); 

    //this part runs when the mapobject is created and rendered
    google.maps.event.addListenerOnce(map, 'tilesloaded', function () {
        initOptions(); //setup the graphical user interface (enhances visual effect to do all of this after map loads)
        initOverlayList(); //list all the overlays in the list box"
    });
}

//Displays all the circles sent from the C# code.
function displayIncomingCircles() {
    if (globalVar.incomingCircleCenter.length > 0) {
        for (var i = 0; i < globalVar.incomingCircleCenter.length; i++) {
            switch (globalVar.incomingCircleFeatureType[i]) {
                case "":
                    break;
                case "main":
                    break;
                case "poi":
                    de("incoming poi: " + i + " " + globalVar.incomingCircleLabel[i]);
                    globalVar.placerType = "poi";
                    var circle = new google.maps.Circle({
                        center: globalVar.incomingCircleCenter[i],
                        map: map,
                        draggable: true,
                        editable: true,
                        title: globalVar.incomingCircleLabel[i],
                        radius: globalVar.incomingCircleRadius[i]
                    });
                    if (globalVar.placerType == "poi") {
                        globalVar.firstSavePOI = true;
                        globalVar.poi_i++;

                        label[globalVar.poi_i] = new MarkerWithLabel({
                            position: circle.getCenter(), //position of real marker
                            zIndex: 2,
                            map: map,
                            labelContent: globalVar.incomingCircleLabel[i],
                            labelAnchor: new google.maps.Point(15, 0),
                            labelClass: "labels", // the CSS class for the label
                            labelStyle: { opacity: 0.75 },
                            icon: {} //initialize to nothing so no marker shows
                        });

                        var poiId = globalVar.poi_i + 1;
                        globalVar.poiObj[globalVar.poi_i] = circle;
                        globalVar.poiType[globalVar.poi_i] = "circle";
                        var poiDescTemp = globalVar.incomingCircleLabel[i];
                        document.getElementById("poiList").innerHTML += writeHTML("poiListItemIncoming", globalVar.poi_i, poiId, poiDescTemp);
                        globalVar.poiDesc[globalVar.poi_i] = poiDescTemp;
                        var contentString = writeHTML("poiDescIncoming", globalVar.poi_i, poiDescTemp, "");
                        infoWindow[globalVar.poi_i] = new google.maps.InfoWindow({
                            content: contentString
                        });
                        infoWindow[globalVar.poi_i].setPosition(circle.getCenter());
                        infoWindow[globalVar.poi_i].open(map);
                        globalVar.poiCount++;
                    }
                    google.maps.event.addListener(circle, 'dragstart', function() {

                        if (globalVar.placerType == "poi") {
                            globalVar.firstSavePOI = true;
                            for (var i = 0; i < globalVar.poiObj.length; i++) {
                                if (globalVar.poiObj[i] == this) {
                                    infoWindow[i].setMap(null);
                                    label[i].setMap(null);
                                }
                            }
                        }
                    });
                    google.maps.event.addListener(circle, 'drag', function() {
                        //used to get the center point for lat/long tool
                        globalVar.circleCenter = this.getCenter();
                        var str = this.getCenter().toString();
                        var cLatV = str.replace("(", "").replace(")", "").split(",", 1);
                        var cLongV = str.replace(cLatV, "").replace("(", "").replace(")", "").replace(",", ""); //is this better than passing into array?s
                        if (cLatV.indexOf("-") != 0) {
                            latH = "N";
                        } else {
                            latH = "S";
                        }
                        if (cLongV.indexOf("-") != 0) {
                            longH = "W";
                        } else {
                            longH = "E";
                        }
                        cLat.innerHTML = cLatV + " (" + latH + ")";
                        cLong.innerHTML = cLongV + " (" + longH + ")";
                    });
                    google.maps.event.addListener(circle, 'dragend', function() {
                        if (globalVar.placerType == "poi") {
                            globalVar.firstSavePOI = true;
                            for (var i = 0; i < globalVar.poiObj.length; i++) {
                                if (globalVar.poiObj[i] == this) {
                                    infoWindow[i].setPosition(this.getCenter());
                                    infoWindow[i].open(null);
                                    label[i].setPosition(this.getCenter());
                                    label[i].setMap(map);
                                }
                            }
                        }
                    });
                    google.maps.event.addListener(circle, 'click', function() {
                        if (globalVar.placerType == "poi") {
                            globalVar.firstSavePOI = true;
                            for (var i = 0; i < globalVar.poiObj.length; i++) {
                                if (globalVar.poiObj[i] == this) {
                                    infoWindow[i].setPosition(this.getCenter());
                                    infoWindow[i].open(map);
                                }
                            }
                        }
                    });
                    break;
            }
        }
    } else {
        //nothing
    }
    //once everything is drawn, determine if there are pois
    if (globalVar.poiCount > 0) {
        //close and reopen pois (to fix firefox bug)
        setTimeout(function () {
            globalVar.RIBMode = true;
            toggleVis("pois");
            toggleVis("pois");
            globalVar.RIBMode = false;
            //this hides the infowindows at startup
            for (var j = 0; j < globalVar.poiCount; j++) {
                infoWindow[j].setMap(null);
            }
        }, 1000);
    }
}

//Displays all the points sent from the C# code.
function displayIncomingPoints() {
    if (globalVar.incomingPointCenter) {
        //go through and display points as long as there is a point to display
        for (var i = 0; i < globalVar.incomingPointCenter.length; i++) {
            switch (globalVar.incomingPointFeatureType[i]) {
                case "":
                    globalVar.placerType = "item";
                    globalVar.firstMarker++;
                    globalVar.itemMarker = new google.maps.Marker({
                        position: globalVar.incomingPointCenter[i],
                        map: map,
                        draggable: true,
                        title: globalVar.incomingPointLabel[i]
                    });
                    document.getElementById('content_toolbox_posItem').value = globalVar.itemMarker.getPosition();
                    codeLatLng(globalVar.itemMarker.getPosition());
                    google.maps.event.addListener(globalVar.itemMarker, 'dragend', function () {
                        globalVar.firstSaveItem = true;
                        globalVar.savingMarkerCenter = globalVar.itemMarker.getPosition(); //store coords to save
                        document.getElementById('content_toolbox_posItem').value = globalVar.itemMarker.getPosition();
                        codeLatLng(globalVar.itemMarker.getPosition());
                    });
                    globalVar.mainCount++;
                    globalVar.incomingACL = "item";
                    break;
                case "main":
                    globalVar.placerType = "item";
                    globalVar.firstMarker++;
                    globalVar.itemMarker = new google.maps.Marker({
                        position: globalVar.incomingPointCenter[i],
                        map: map,
                        draggable: true,
                        title: globalVar.incomingPointLabel[i]
                    });
                    document.getElementById('content_toolbox_posItem').value = globalVar.itemMarker.getPosition();
                    codeLatLng(globalVar.itemMarker.getPosition());
                    google.maps.event.addListener(globalVar.itemMarker, 'dragend', function () {
                        globalVar.firstSaveItem = true;
                        globalVar.savingMarkerCenter = globalVar.itemMarker.getPosition(); //store coords to save
                        document.getElementById('content_toolbox_posItem').value = globalVar.itemMarker.getPosition();
                        codeLatLng(globalVar.itemMarker.getPosition());
                    });
                    globalVar.mainCount++;
                    globalVar.incomingACL = "item";
                    break;
                case "poi":
                    de("incoming poi: " + i + " " + globalVar.incomingPointLabel[i]);
                    globalVar.placerType = "poi";
                    var marker = new google.maps.Marker({
                        position: globalVar.incomingPointCenter[i],
                        map: map,
                        draggable: true,
                        title: globalVar.incomingPointLabel[i]
                    });
                    de("incoming center: " + marker.getPosition());
                    if (globalVar.placerType == "poi") {
                        globalVar.firstSavePOI = true;
                        globalVar.poi_i++;
                        label[globalVar.poi_i] = new MarkerWithLabel({
                            position: marker.getPosition(), //position of real marker
                            map: map,
                            zIndex: 2,
                            labelContent: globalVar.incomingPointLabel[(i)],
                            labelAnchor: new google.maps.Point(15, 0),
                            labelClass: "labels", // the CSS class for the label
                            labelStyle: { opacity: 0.75 },
                            icon: {} //initialize to nothing so no marker shows
                        });
                        globalVar.poiObj[globalVar.poi_i] = marker;
                        globalVar.poiType[globalVar.poi_i] = "marker";
                        var poiId = globalVar.poi_i + 1;
                        var poiDescTemp = globalVar.incomingPointLabel[i];
                        document.getElementById("poiList").innerHTML += writeHTML("poiListItemIncoming", globalVar.poi_i, poiId, poiDescTemp);
                        globalVar.poiDesc[globalVar.poi_i] = poiDescTemp;
                        var contentString = writeHTML("poiDescIncoming", globalVar.poi_i, poiDescTemp,"");
                        infoWindow[globalVar.poi_i] = new google.maps.InfoWindow({
                            content: contentString,
                            position: marker.getPosition()
                            //pixelOffset: new google.maps.Size(0, -40)
                        });
                        infoWindow[globalVar.poi_i].setMap(map);
                        infoWindow[globalVar.poi_i].open(map, globalVar.poiObj[globalVar.poi_i]);
                        globalVar.poiCount++;
                    }
                    google.maps.event.addListener(marker, 'dragstart', function () {
                        if (globalVar.placerType == "poi") {
                            globalVar.firstSavePOI = true;
                            for (var i = 0; i < globalVar.poiObj.length; i++) {
                                if (globalVar.poiObj[i] == this) {
                                    infoWindow[i].setMap(null);
                                    label[i].setMap(null);
                                }
                            }
                        }
                    });
                    google.maps.event.addListener(marker, 'dragend', function () {
                        if (globalVar.placerType == "poi") {
                            globalVar.firstSavePOI = true;
                            for (var i = 0; i < globalVar.poiObj.length; i++) {
                                if (globalVar.poiObj[i] == this) {
                                    infoWindow[i].setOptions({ position: this.getPosition(), pixelOffset: new google.maps.Size(0, -40) });
                                    infoWindow[i].open(null);
                                    label[i].setPosition(this.getPosition());
                                    label[i].setMap(map);
                                }
                            }
                        }
                    });
                    google.maps.event.addListener(marker, 'click', function () {
                        if (globalVar.placerType == "poi") {
                            globalVar.firstSavePOI = true;
                            for (var i = 0; i < globalVar.poiObj.length; i++) {
                                if (globalVar.poiObj[i] == this) {
                                    infoWindow[i].setOptions({ position: this.getPosition(), pixelOffset: new google.maps.Size(0, -40) });
                                    infoWindow[i].open(map);
                                }
                            }
                        }
                    });
                    break;
            }
        }
    } else {
        globalVar.firstMarker++;
        globalVar.itemMarker = new google.maps.Marker({
            position: map.getCenter(), //just get the center poin of the map
            map: null, //hide on load
            draggable: false,
            title: globalVar.incomingPointLabel[0]
        });
        //nothing to display because there is no geolocation of item
    }
    //once everything is drawn, determine if there are pois
    if (globalVar.poiCount > 0) {
        //close and reopen pois (to fix firefox bug)
        setTimeout(function () {
            globalVar.RIBMode = true;
            toggleVis("pois");
            toggleVis("pois");
            globalVar.RIBMode = false;
            //this hides the infowindows at startup
            for (var j = 0; j < globalVar.poiCount; j++) {
                infoWindow[j].setMap(null);
            }
        }, 1000);
    }
}

//Displays all the lines sent from the C# code.
function displayIncomingLines() {
    if (globalVar.incomingLinePath.length > 0) {
        for (var i = 0; i < globalVar.incomingLinePath.length; i++) {
            switch (globalVar.incomingLineFeatureType[i]) {
                case "":
                    break;
                case "main":
                    break;
                case "poi":
                    de("incoming poi: " + i + " " + globalVar.incomingLineLabel[i]);
                    globalVar.placerType = "poi";
                    var polyline = new google.maps.Polyline({
                        path: globalVar.incomingLinePath[i],
                        map: map,
                        draggable: true,
                        editable: true,
                        title: globalVar.incomingLineLabel[i]
                    });
                    if (globalVar.placerType == "poi") {
                        globalVar.firstSavePOI = true;
                        globalVar.poi_i++;
                        var poiId = globalVar.poi_i + 1;
                        globalVar.poiObj[globalVar.poi_i] = polyline;
                        globalVar.poiType[globalVar.poi_i] = "polyline";
                        var poiDescTemp = globalVar.incomingLineLabel[i];
                        document.getElementById("poiList").innerHTML += writeHTML("poiListItemIncoming", globalVar.poi_i, poiId, poiDescTemp);
                        globalVar.poiDesc[globalVar.poi_i] = poiDescTemp;
                        var contentString = writeHTML("poiDescIncoming", globalVar.poi_i, poiDescTemp, "");
                        infoWindow[globalVar.poi_i] = new google.maps.InfoWindow({
                            content: contentString
                        });
                        var polylinePoints = [];
                        var polylinePointCount = 0;
                        polyline.getPath().forEach(function (latLng) {
                            polylinePoints[polylinePointCount] = latLng;
                            de("polylinePoints[" + polylinePointCount + "] = " + latLng);
                            polylinePointCount++;
                        });
                        de("polylinePointCount: " + polylinePointCount);
                        de("polylinePoints.length: " + polylinePoints.length);
                        de("Math.round((polylinePoints.length / 2)): " + Math.round((polylinePoints.length / 2)));
                        var polylineCenterPoint = polylinePoints[Math.round((polylinePoints.length / 2))];
                        de("polylineCenterPoint: " + polylineCenterPoint);
                        var polylineStartPoint = polylinePoints[0];
                        de("polylineStartPoint: " + polylineStartPoint);
                        infoWindow[globalVar.poi_i].setPosition(polylineStartPoint);
                        infoWindow[globalVar.poi_i].open(map);
                        //best fix so far
                        if (globalVar.poiCount == 0) {
                            setTimeout(function () {
                                infoWindow[0].setMap(null);
                                infoWindow[0].setMap(map);
                            }, 800);
                        }
                        globalVar.poiCount++;
                        label[globalVar.poi_i] = new MarkerWithLabel({
                            position: polylineStartPoint, //position at start of polyline
                            zIndex: 2,
                            map: map,
                            labelContent: globalVar.incomingLineLabel[i], 
                            labelAnchor: new google.maps.Point(15, 0),
                            labelClass: "labels", // the CSS class for the label
                            labelStyle: { opacity: 0.75 },
                            icon: {} //initialize to nothing so no marker shows
                        });
                    }
                    google.maps.event.addListener(polyline, 'mouseout', function () {
                        if (globalVar.placerType == "poi") {
                            globalVar.firstSavePOI = true;
                            for (var i = 0; i < globalVar.poiObj.length; i++) {
                                if (globalVar.poiObj[i] == this) {
                                    var polylinePoints = [];
                                    var polylinePointCount = 0;
                                    de("here1");
                                    this.getPath().forEach(function (latLng) {
                                        polylinePoints[polylinePointCount] = latLng;
                                        polylinePointCount++;
                                    });
                                    de("here2");
                                    var polylineCenterPoint = polylinePoints[(polylinePoints.length / 2)];
                                    var polylineStartPoint = polylinePoints[0];
                                    infoWindow[i].setPosition(polylineStartPoint);
                                    label[i].setPosition(polylineStartPoint);
                                    de("here3");
                                }
                            }
                        }
                    });
                    google.maps.event.addListener(polyline, 'dragstart', function () {
                        if (globalVar.placerType == "poi") {
                            globalVar.firstSavePOI = true;
                            for (var i = 0; i < globalVar.poiObj.length; i++) {
                                if (globalVar.poiObj[i] == this) {
                                    infoWindow[i].setMap(null);
                                    label[i].setMap(null);
                                }
                            }
                        }
                    });
                    google.maps.event.addListener(polyline, 'drag', function () {
                        //used for lat/long tooll
                        var bounds = new google.maps.LatLngBounds;
                        this.getPath().forEach(function (latLng) { bounds.extend(latLng); });
                        var polylineCenter = bounds.getCenter();
                        var str = polylineCenter.toString();
                        var cLatV = str.replace("(", "").replace(")", "").split(",", 1);
                        var cLongV = str.replace(cLatV, "").replace("(", "").replace(")", "").replace(",", ""); //is this better than passing into array?s
                        if (cLatV.indexOf("-") != 0) {
                            latH = "N";
                        } else {
                            latH = "S";
                        }
                        if (cLongV.indexOf("-") != 0) {
                            longH = "W";
                        } else {
                            longH = "E";
                        }
                        cLat.innerHTML = cLatV + " (" + latH + ")";
                        cLong.innerHTML = cLongV + " (" + longH + ")";
                    });
                    google.maps.event.addListener(polyline, 'dragend', function () {
                        if (globalVar.placerType == "poi") {
                            globalVar.firstSavePOI = true;
                            //var bounds = new google.maps.LatLngBounds;
                            //polyline.getPath().forEach(function (latLng) { bounds.extend(latLng); });
                            //var polylineCenter = bounds.getCenter();
                            //var bounds = new google.maps.LatLngBounds; //spatial center, bounds holder

                            for (var i = 0; i < globalVar.poiObj.length; i++) {
                                if (globalVar.poiObj[i] == this) {
                                    var polylinePoints = [];
                                    var polylinePointCount = 0;
                                    this.getPath().forEach(function (latLng) {
                                        polylinePoints[polylinePointCount] = latLng;
                                        polylinePointCount++;
                                    });
                                    var polylineCenterPoint = polylinePoints[(polylinePoints.length / 2)];
                                    var polylineStartPoint = polylinePoints[0];
                                    infoWindow[i].setPosition(polylineStartPoint);
                                    infoWindow[i].open(null);
                                    label[i].setPosition(polylineStartPoint);
                                    label[i].setMap(map);
                                }
                            }

                        }
                    });
                    google.maps.event.addListener(polyline, 'click', function () {
                        if (globalVar.placerType == "poi") {
                            globalVar.firstSavePOI = true;
                            //var bounds = new google.maps.LatLngBounds;
                            //polyline.getPath().forEach(function (latLng) { bounds.extend(latLng); });
                            //var polylineCenter = bounds.getCenter();

                            for (var i = 0; i < globalVar.poiObj.length; i++) {
                                if (globalVar.poiObj[i] == this) {
                                    var polylinePoints = [];
                                    var polylinePointCount = 0;
                                    this.getPath().forEach(function (latLng) {
                                        polylinePoints[polylinePointCount] = latLng;
                                        polylinePointCount++;
                                    });
                                    var polylineCenterPoint = polylinePoints[(polylinePoints.length / 2)];
                                    var polylineStartPoint = polylinePoints[0];
                                    infoWindow[i].setPosition(polylineStartPoint);
                                    infoWindow[i].open(map);
                                }
                            }
                        }
                    });
                    break;
            }
        }
    } else {
        //nothing
    }
    //once everything is drawn, determine if there are pois
    if (globalVar.poiCount > 0) {
        //close and reopen pois (to fix firefox bug)
        setTimeout(function () {
            globalVar.RIBMode = true;
            toggleVis("pois");
            toggleVis("pois");
            globalVar.RIBMode = false;
            //this hides the infowindows at startup
            for (var j = 0; j < globalVar.poiCount; j++) {
                infoWindow[j].setMap(null);
            }
        }, 1000);
    }
}

//Displays all the overlays sent from the C# code. Also calls displayglobalVar.ghostOverlayRectangle.
function displayIncomingPolygons() {
    //go through and display overlays as long as there is an overlay to display
    for (var i = 0; i < globalVar.incomingPolygonPath.length; i++) {
        switch (globalVar.incomingPolygonFeatureType[i]) {
        case "hidden":
            //hidden do nothing
            break;
        case "":
            //globalVar.workingOverlayIndex = globalVar.incomingPolygonPageId[i];
            //create overlay with incoming
            globalVar.overlaysOnMap[globalVar.incomingPolygonPageId[i]] = new CustomOverlay(globalVar.incomingPolygonPageId[i], globalVar.incomingPolygonPath[i], globalVar.incomingPolygonSourceURL[i], map, globalVar.incomingPolygonRotation[i]);
            globalVar.currentlyEditing = "no";
            //set the overlay to the map
            globalVar.overlaysOnMap[globalVar.incomingPolygonPageId[i]].setMap(map);
            //set hotspot on top of overlay
            setGhostOverlay(globalVar.incomingPolygonPageId[i], globalVar.incomingPolygonPath[i]);
            de("I created ghost: " + globalVar.incomingPolygonPageId[i]);
            globalVar.mainCount++;
            globalVar.incomingACL = "overlay";
            drawingManager.setDrawingMode(null); //reset drawing manager no matter what
            globalVar.overlaysCurrentlyDisplayed = true;
            break;
        case "main":
            globalVar.workingOverlayIndex = globalVar.incomingPolygonPageId[i];
            //create overlay with incoming
            //alert(globalVar.incomingPolygonPageId[i] + ", " + globalVar.incomingPolygonPath[i] + ", " + globalVar.incomingPolygonSourceURL[i] + ", " + globalVar.incomingPolygonRotation[i]);
            globalVar.overlaysOnMap[globalVar.incomingPolygonPageId[i]] = new CustomOverlay(globalVar.incomingPolygonPageId[i], globalVar.incomingPolygonPath[i], globalVar.incomingPolygonSourceURL[i], map, globalVar.incomingPolygonRotation[i]);
            globalVar.currentlyEditing = "no";
            //set the overlay to the map
            globalVar.overlaysOnMap[globalVar.incomingPolygonPageId[i]].setMap(map);
            //keepRotate(globalVar.incomingPolygonRotation[i]);
            //set hotspot on top of overlay
            setGhostOverlay(globalVar.incomingPolygonPageId[i], globalVar.incomingPolygonPath[i]);
            de("I created ghost: " + globalVar.incomingPolygonPageId[i]);
            globalVar.mainCount++;
            globalVar.incomingACL = "overlay";
            drawingManager.setDrawingMode(null); //reset drawing manager no matter what
            globalVar.overlaysCurrentlyDisplayed = true;
            break;
        case "poi":
            //globalVar.placerType = "poi";
            if (globalVar.placerType == "poi") {
                //determine polygon type
                if (globalVar.incomingPolygonPolygonType[i] == "rectangle") {
                    de("incoming poi: " + i + " " + globalVar.incomingPolygonLabel[i]);
                    de("detected incoming rectangle");
                    //convert path to a rectangle bounds
                    var pathCount = 0;
                    var polygon = new google.maps.Polygon({
                        paths: globalVar.incomingPolygonPath[i]
                    });
                    polygon.getPath().forEach(function () { pathCount++; });
                    if (pathCount == 2) {
                        de("pathcount: " + pathCount);
                        var l = [5];
                        var lcount = 1;
                        polygon.getPath().forEach(function (latLng) {
                            var newLatLng = String(latLng).split(",");
                            var lat = newLatLng[0].replace("(", "");
                            var lng = newLatLng[1].replace(")", "");
                            l[lcount] = lat;
                            lcount++;
                            l[lcount] = lng;
                            lcount++;
                        });
                        globalVar.incomingPolygonPath[i] = new google.maps.LatLngBounds(new google.maps.LatLng(l[3], l[2]), new google.maps.LatLng(l[1], l[4]));
                        //rectangle.setBounds([new google.maps.LatLng(l[1], l[4]), new google.maps.LatLng(l[3], l[4]), new google.maps.LatLng(l[3], l[2]), new google.maps.LatLng(l[1], l[2])]);
                    }
                    var rectangle = new google.maps.Rectangle({
                        bounds: globalVar.incomingPolygonPath[i],
                        map: map,
                        draggable: true,
                        editable: true,
                        title: globalVar.incomingPolygonLabel[i]
                    });
                    if (globalVar.placerType == "poi") {
                        globalVar.firstSavePOI = true;
                        globalVar.poi_i++;
                        label[globalVar.poi_i] = new MarkerWithLabel({
                            position: rectangle.getBounds().getCenter(), //position of real marker
                            zIndex: 2,
                            map: map,
                            labelContent: globalVar.incomingPolygonLabel[i],
                            labelAnchor: new google.maps.Point(15, 0),
                            labelClass: "labels", // the CSS class for the label
                            labelStyle: { opacity: 0.75 },
                            icon: {} //initialize to nothing so no marker shows
                        });
                        var poiId = globalVar.poi_i + 1;
                        globalVar.poiObj[globalVar.poi_i] = rectangle;
                        globalVar.poiType[globalVar.poi_i] = "rectangle";
                        var poiDescTemp = globalVar.incomingPolygonLabel[i];
                        document.getElementById("poiList").innerHTML += writeHTML("poiListItemIncoming", globalVar.poi_i, poiId, poiDescTemp);
                        globalVar.poiDesc[globalVar.poi_i] = poiDescTemp;
                        var contentString = writeHTML("poiDescIncoming", globalVar.poi_i, poiDescTemp, "");
                        infoWindow[globalVar.poi_i] = new google.maps.InfoWindow({
                            content: contentString
                        });
                        infoWindow[globalVar.poi_i].setPosition(rectangle.getBounds().getCenter());
                        infoWindow[globalVar.poi_i].open(map);
                        //best fix so far
                        if (globalVar.poiCount == 0) {
                            setTimeout(function () {
                                infoWindow[0].setMap(null);
                                infoWindow[0].setMap(map);
                            }, 800);
                        }
                        globalVar.poiCount++;
                    }
                    google.maps.event.addListener(rectangle, 'bounds_changed', function () {
                        if (globalVar.placerType == "poi") {
                            globalVar.firstSavePOI = true;
                            for (var i = 0; i < globalVar.poiObj.length; i++) {
                                if (globalVar.poiObj[i] == this) {
                                    infoWindow[i].setPosition(this.getBounds().getCenter());
                                    infoWindow[i].setMap(null);
                                    label[i].setPosition(this.getBounds().getCenter());
                                    label[i].setMap(map);
                                }
                            }
                        }
                    });
                    google.maps.event.addListener(rectangle, 'dragstart', function () {
                        if (globalVar.placerType == "poi") {
                            globalVar.firstSavePOI = true;
                            for (var i = 0; i < globalVar.poiObj.length; i++) {
                                if (globalVar.poiObj[i] == this) {
                                    infoWindow[i].setMap(null);
                                    label[i].setMap(null);
                                }
                            }
                        }
                    });
                    google.maps.event.addListener(rectangle, 'drag', function () {
                        if (globalVar.placerType == "poi") {
                            globalVar.firstSavePOI = true;
                            for (var i = 0; i < globalVar.poiObj.length; i++) {
                                if (globalVar.poiObj[i] == this) {
                                    infoWindow[i].setMap(null);
                                    label[i].setMap(null);
                                }
                            }
                        }
                        //used to get center point for lat/long tool
                        var str = this.getBounds().getCenter().toString();
                        var cLatV = str.replace("(", "").replace(")", "").split(",", 1);
                        var cLongV = str.replace(cLatV, "").replace("(", "").replace(")", "").replace(",", "");
                        if (cLatV.indexOf("-") != 0) {
                            latH = "N";
                        } else {
                            latH = "S";
                        }
                        if (cLongV.indexOf("-") != 0) {
                            longH = "W";
                        } else {
                            longH = "E";
                        }
                        cLat.innerHTML = cLatV + " (" + latH + ")";
                        cLong.innerHTML = cLongV + " (" + longH + ")";
                    });
                    google.maps.event.addListener(rectangle, 'dragend', function () {
                        if (globalVar.placerType == "poi") {
                            globalVar.firstSavePOI = true;
                            for (var i = 0; i < globalVar.poiObj.length; i++) {
                                if (globalVar.poiObj[i] == this) {
                                    infoWindow[i].setPosition(this.getBounds().getCenter());
                                    infoWindow[i].open(null);
                                    label[i].setPosition(this.getBounds().getCenter());
                                    label[i].setMap(map);
                                }
                            }
                        }
                    });
                    google.maps.event.addListener(rectangle, 'click', function () {
                        if (globalVar.placerType == "poi") {
                            globalVar.firstSavePOI = true;
                            for (var i = 0; i < globalVar.poiObj.length; i++) {
                                if (globalVar.poiObj[i] == this) {
                                    infoWindow[i].setPosition(this.getBounds().getCenter());
                                    infoWindow[i].open(map);
                                }
                            }
                        }
                    });
                } else {
                    var polygon = new google.maps.Polygon({
                        paths: globalVar.incomingPolygonPath[i],
                        map: map,
                        draggable: true,
                        editable: true,
                        title: globalVar.incomingPolygonLabel[i]
                    });
                    if (globalVar.placerType == "poi") {
                        globalVar.firstSavePOI = true;
                        globalVar.poi_i++;
                        label[globalVar.poi_i] = new MarkerWithLabel({
                            position: polygonCenter(polygon), //position of real marker
                            zIndex: 2,
                            map: map,
                            labelContent: globalVar.incomingPolygonLabel[i], //the current user count
                            labelAnchor: new google.maps.Point(15, 0),
                            labelClass: "labels", // the CSS class for the label
                            labelStyle: { opacity: 0.75 },
                            icon: {} //initialize to nothing so no marker shows
                        });
                        var poiId = globalVar.poi_i + 1;
                        globalVar.poiObj[globalVar.poi_i] = polygon;
                        globalVar.poiType[globalVar.poi_i] = "polygon";
                        var poiDescTemp = globalVar.incomingPolygonLabel[i];
                        document.getElementById("poiList").innerHTML += writeHTML("poiListItemIncoming", globalVar.poi_i, poiId, poiDescTemp);
                        globalVar.poiDesc[globalVar.poi_i] = poiDescTemp;
                        var contentString = writeHTML("poiDescIncoming", globalVar.poi_i, poiDescTemp, "");
                        infoWindow[globalVar.poi_i] = new google.maps.InfoWindow({
                            content: contentString
                        });
                        infoWindow[globalVar.poi_i].setPosition(polygonCenter(polygon));
                        infoWindow[globalVar.poi_i].open(map);
                        //best fix so far
                        if (globalVar.poiCount == 0) {
                            setTimeout(function () {
                                infoWindow[0].setMap(null);
                                infoWindow[0].setMap(map);
                            }, 800);
                        }
                        globalVar.poiCount++;
                    }
                    google.maps.event.addListener(polygon, 'mouseout', function () { //if bounds change
                        if (globalVar.placerType == "poi") {
                            globalVar.firstSavePOI = true;
                            for (var i = 0; i < globalVar.poiObj.length; i++) {
                                if (globalVar.poiObj[i] == this) {
                                    infoWindow[i].setPosition(polygonCenter(this));
                                    label[i].setPosition(polygonCenter(this));
                                }
                            }
                        }
                    });
                    google.maps.event.addListener(polygon, 'dragstart', function () {
                        if (globalVar.placerType == "poi") {
                            globalVar.firstSavePOI = true;
                            for (var i = 0; i < globalVar.poiObj.length; i++) {
                                if (globalVar.poiObj[i] == this) {
                                    infoWindow[i].setMap(null);
                                    label[i].setMap(null);
                                }
                            }
                        }
                    });
                    google.maps.event.addListener(polygon, 'drag', function () {
                        if (globalVar.placerType == "poi") {
                            globalVar.firstSavePOI = true;
                            for (var i = 0; i < globalVar.poiObj.length; i++) {
                                if (globalVar.poiObj[i] == this) {
                                    infoWindow[i].setMap(null);
                                    label[i].setMap(null);
                                }
                            }
                        }
                        //used for lat/long tool
                        var str = polygonCenter(this).toString();
                        var cLatV = str.replace("(", "").replace(")", "").split(",", 1);
                        var cLongV = str.replace(cLatV, "").replace("(", "").replace(")", "").replace(",", ""); //is this better than passing into array?s
                        if (cLatV.indexOf("-") != 0) {
                            latH = "N";
                        } else {
                            latH = "S";
                        }
                        if (cLongV.indexOf("-") != 0) {
                            longH = "W";
                        } else {
                            longH = "E";
                        }
                        cLat.innerHTML = cLatV + " (" + latH + ")";
                        cLong.innerHTML = cLongV + " (" + longH + ")";
                    });
                    google.maps.event.addListener(polygon, 'dragend', function () {
                        if (globalVar.placerType == "poi") {
                            globalVar.firstSavePOI = true;
                            for (var i = 0; i < globalVar.poiObj.length; i++) {
                                if (globalVar.poiObj[i] == this) {
                                    infoWindow[i].setPosition(polygonCenter(this));
                                    infoWindow[i].open(null);
                                    label[i].setPosition(polygonCenter(this));
                                    label[i].setMap(map);
                                }
                            }
                        }
                    });
                    google.maps.event.addListener(polygon, 'click', function () {
                        if (globalVar.placerType == "poi") {
                            globalVar.firstSavePOI = true;
                            for (var i = 0; i < globalVar.poiObj.length; i++) {
                                if (globalVar.poiObj[i] == this) {
                                    infoWindow[i].setPosition(polygonCenter(this));
                                    infoWindow[i].open(map);
                                }
                            }
                        }
                    });
                }
                globalVar.overlaysCurrentlyDisplayed = true;
                //once everything is drawn, determine if there are pois
                if (globalVar.poiCount > 0) {
                    //close and reopen pois (to fix firefox bug)
                    setTimeout(function () {
                        globalVar.RIBMode = true;
                        toggleVis("pois");
                        toggleVis("pois");
                        globalVar.RIBMode = false;
                        //this hides the infowindows at startup
                        for (var j = 0; j < globalVar.poiCount; j++) {
                            infoWindow[j].setMap(null);
                        }
                    }, 1000);
                }
            }
            break;
        }
    }
}

//clears all incoming overlays
function clearIncomingOverlays() {
    //go through and display overlays as long as there is an overlay to display
    for (var i = 0; i < globalVar.incomingPolygonPageId.length; i++) {
        if (globalVar.overlaysOnMap[globalVar.incomingPolygonPageId[i]] != null) {
            globalVar.overlaysOnMap[globalVar.incomingPolygonPageId[i]].setMap(null);
            globalVar.overlaysOnMap[globalVar.incomingPolygonPageId[i]] = null;
            globalVar.ghostOverlayRectangle[globalVar.incomingPolygonPageId[i]].setMap(null);
            globalVar.ghostOverlayRectangle[globalVar.incomingPolygonPageId[i]] = null;
        } else {
            //do nothing
        }
    }
    //globalVar.overlaysCurrentlyDisplayed = false;
    globalVar.preservedOpacity = globalVar.defaultOpacity;
}

//Creates and sets the ghost overlays (used to tie actions with actual overlay)
function setGhostOverlay(ghostIndex, ghostBounds) {

    //create ghost directly over an overlay
    globalVar.ghostOverlayRectangle[ghostIndex] = new google.maps.Rectangle();        //init ghost
    globalVar.ghostOverlayRectangle[ghostIndex].setOptions(globalVar.ghosting);       //set globalVar.ghosting 
    globalVar.ghostOverlayRectangle[ghostIndex].setBounds(ghostBounds);               //set bounds
    globalVar.ghostOverlayRectangle[ghostIndex].setMap(map);                          //set to map

    //create listener for if clicked
    google.maps.event.addListener(globalVar.ghostOverlayRectangle[ghostIndex], 'click', function () {
        if (globalVar.pageMode == "edit") {
            if (globalVar.currentlyEditing == "yes") {                                                            //if editing is being done, save
                if (globalVar.workingOverlayIndex == null) {
                    globalVar.currentlyEditing = "no";
                } else {
                    cacheSaveOverlay(globalVar.workingOverlayIndex);                                                       //trigger a cache of current working overlay
                    globalVar.ghostOverlayRectangle[globalVar.workingOverlayIndex].setOptions(globalVar.ghosting);                    //set globalVar.rectangle to globalVar.ghosting
                    globalVar.currentlyEditing = "no";                                                            //reset editing marker
                    globalVar.preservedRotation = 0;                                                              //reset preserved rotation
                }
            }
            if (globalVar.currentlyEditing == "no") {                                                             //if editing is not being done, start editing
                $("#toolbox").show();                                                                   //show the toolbox
                globalVar.toolboxDisplayed = true;                                                                //mark that the toolbox is open
                $("#toolboxTabs").accordion({ active: 3 });                                             //open edit overlay tab in toolbox
                globalVar.currentlyEditing = "yes";                                                               //enable editing marker
                globalVar.workingOverlayIndex = ghostIndex;                                                       //set this overay as the one being e
                globalVar.ghostOverlayRectangle[ghostIndex].setOptions(globalVar.editable);                                 //show ghost
                globalVar.currentTopZIndex++;                                                                     //iterate top z index
                document.getElementById("overlay" + ghostIndex).style.zIndex = globalVar.currentTopZIndex;        //bring overlay to front
                globalVar.ghostOverlayRectangle[ghostIndex].setOptions({ zIndex: globalVar.currentTopZIndex });             //bring ghost to front
                //set rotation if the overlay was previously saved
                if (globalVar.preservedRotation != globalVar.savingOverlayRotation[ghostIndex - 1]) {
                    globalVar.preservedRotation = globalVar.savingOverlayRotation[ghostIndex-1];
                }
                //for (var i = 0; i < globalVar.savingOverlayIndex.length; i++) {
                //    if (ghostIndex == globalVar.savingOverlayIndex[i]) {
                //        globalVar.preservedRotation = globalVar.savingOverlayRotation[i];
                //    }
                //}
            }
        }
    });

    //set listener for bounds changed
    google.maps.event.addListener(globalVar.ghostOverlayRectangle[ghostIndex], 'bounds_changed', function () {
        de("ghost index: " + ghostIndex);
        if (globalVar.pageMode == "edit") {
            //hide previous overlay
            globalVar.overlaysOnMap[ghostIndex].setMap(null);
            //delete previous overlay values
            globalVar.overlaysOnMap[ghostIndex] = null;
            //redraw the overlay within the new bounds
            de(globalVar.preservedRotation);
            globalVar.overlaysOnMap[ghostIndex] = new CustomOverlay(ghostIndex, globalVar.ghostOverlayRectangle[ghostIndex].getBounds(), globalVar.incomingPolygonSourceURL[(ghostIndex-1)], map, globalVar.preservedRotation);
            //set the overlay with new bounds to the map
            globalVar.overlaysOnMap[ghostIndex].setMap(map);
            //enable editing marker
            globalVar.currentlyEditing = "yes";
            //trigger a cache of current working overlay
            cacheSaveOverlay(globalVar.workingOverlayIndex);
        }
    });

    //set listener for right click (fixes reset issue over overlays)
    google.maps.event.addListener(globalVar.ghostOverlayRectangle[ghostIndex], 'rightclick', function () {
        drawingManager.setDrawingMode(null); //reset drawing manager no matter what
    });

}

//Stores the overlays to save and their associated data
function cacheSaveOverlay(index) {
    de("caching save overlay <hr/>");
    de("incoming index: " + index);
    de("current save overlay index: " + globalVar.csoi);
    de("current working overlay index: " + globalVar.workingOverlayIndex);
    //convert working id to index
    globalVar.csoi = index - 1;
    //is this the first save
    globalVar.firstSaveOverlay = true;
    //de("firstSaveOvelay: " + globalVar.firstSaveOverlay);
    //set overlay index to save
    globalVar.savingOverlayIndex[globalVar.csoi] = globalVar.csoi; //globalVar.workingOverlayIndex;
    de("globalVar.savingOverlayIndex[globalVar.csoi]: " + globalVar.savingOverlayIndex[globalVar.csoi]);
    de("globalVar.csoi: " + globalVar.csoi);
    //set label to save
    globalVar.savingOverlayLabel[globalVar.csoi] = globalVar.incomingPolygonLabel[globalVar.csoi];
    de("globalVar.savingOverlayLabel[globalVar.csoi]: " + globalVar.savingOverlayLabel[globalVar.csoi]);
    de("globalVar.incomingPolygonLabel[globalVar.csoi]: " + globalVar.incomingPolygonLabel[globalVar.csoi]);
    //set source url to save
    globalVar.savingOverlaySourceURL[globalVar.csoi] = globalVar.incomingPolygonSourceURL[globalVar.csoi];
    de("globalVar.savingOverlaySourceURL[globalVar.csoi]: " + globalVar.incomingPolygonSourceURL[globalVar.csoi]);
    //set bounds to save
    globalVar.savingOverlayBounds[globalVar.csoi] = globalVar.ghostOverlayRectangle[globalVar.workingOverlayIndex].getBounds();
    de("globalVar.savingOverlayBounds[globalVar.csoi]: " + globalVar.ghostOverlayRectangle[globalVar.workingOverlayIndex].getBounds());
    //set rotation to save
    //alert("caching... \nindex: " + index + "\nwoi: " + globalVar.workingOverlayIndex + "\ncsoi: " + globalVar.csoi + "\nsor: " + globalVar.savingOverlayRotation[globalVar.csoi] + "\nipr: " + globalVar.incomingPolygonRotation[globalVar.csoi] + "\npr: " + globalVar.preservedRotation);
    if (globalVar.savingOverlayRotation[globalVar.csoi] != globalVar.incomingPolygonRotation[globalVar.csoi]) {
        globalVar.savingOverlayRotation[globalVar.csoi] = globalVar.preservedRotation;
        //alert("no match" + globalVar.preservedRotation);
    } else {
        //alert("match " + globalVar.savingOverlayRotation[globalVar.csoi]);
    }
    globalVar.savingOverlayRotation[globalVar.csoi] = globalVar.preservedRotation;
    de("globalVar.savingOverlayRotation[globalVar.csoi]: " + globalVar.savingOverlayRotation[globalVar.csoi]);
    //set the pageId to save
    globalVar.savingOverlayPageId[globalVar.csoi] = globalVar.incomingPolygonPageId[globalVar.csoi];
    de("globalVar.savingOverlayPageId[globalVar.csoi]: " + globalVar.incomingPolygonPageId[globalVar.csoi]);
    ////check to see if we just recached or if it was a unique cache
    //if (globalVar.savingOverlayIndex[globalVar.csoi] != index) {
    //    //iterate the current save overlay index   
    //    globalVar.csoi++;
    //}
    de("save overlay cached");
}

//Starts the creation of a custom overlay div which contains a rectangular image.
//Supporting URL: https://developers.google.com/maps/documentation/javascript/overlays#CustomOverlays
function CustomOverlay(id, bounds, image, map, rotation) {
    globalVar.overlayCount++;                   //iterate how many overlays have been drawn
    this.bounds_ = bounds;                      //set the bounds
    this.image_ = image;                        //set source url
    this.map_ = map;                            //set to map
    globalVar.preservedRotation = rotation;     //set the rotation
    this.div_ = null;                           //defines a property to hold the image's div. We'll actually create this div upon receipt of the onAdd() method so we'll leave it null for now.
    this.index_ = id;                           //set the index/id of this overlay
}

//#endregion

//#region other google functions

//Continues support for adding an custom overlay
//Supporting URL: https://developers.google.com/maps/documentation/javascript/overlays#CustomOverlays
// Note: an overlay's receipt of onAdd() indicates that the map's panes are now available for attaching the overlay to the map via the DOM.
CustomOverlay.prototype.onAdd = function () {

    // Create the DIV and set some basic attributes.
    var div = document.createElement("div");
    div.id = "overlay" + this.index_;
    div.style.borderStyle = 'none';
    div.style.borderWidth = '0px';
    div.style.position = 'absolute';
    div.style.opacity = globalVar.preservedOpacity;

    // Create an IMG element and attach it to the DIV.
    var img = document.createElement('img');
    img.src = this.image_;
    img.style.width = '100%';
    img.style.height = '100%';
    img.style.position = 'absolute';
    div.appendChild(img);

    // Set the overlay's div_ property to this DIV
    this.div_ = div;

    // We add an overlay to a map via one of the map's panes.
    // We'll add this overlay to the overlayLayer pane.
    var panes = this.getPanes();
    panes.overlayLayer.appendChild(div);
};

//Continues support for adding an custom overlay
//Supporting URL: https://developers.google.com/maps/documentation/javascript/overlays#CustomOverlays
CustomOverlay.prototype.draw = function () {
    // Size and position the overlay. We use a southwest and northeast
    // position of the overlay to peg it to the correct position and size.
    // We need to retrieve the projection from this overlay to do this.
    var overlayProjection = this.getProjection();

    // Retrieve the southwest and northeast coordinates of this overlay
    // in latlngs and convert them to pixels coordinates.
    // We'll use these coordinates to resize the DIV.
    var sw = overlayProjection.fromLatLngToDivPixel(this.bounds_.getSouthWest());
    var ne = overlayProjection.fromLatLngToDivPixel(this.bounds_.getNorthEast());

    // Resize the image's DIV to fit the indicated dimensions.
    var div = this.div_;
    div.style.left = sw.x + 'px';
    div.style.top = ne.y + 'px';
    div.style.width = (ne.x - sw.x) + 'px';
    div.style.height = (sw.y - ne.y) + 'px';
    
    //for a rotation
    //hold woi to later put it back in (fixes keepRotate error)
    var temp = globalVar.workingOverlayIndex;
    //set woi to incoming (fixes keepRotate error)
    globalVar.workingOverlayIndex = this.index_;
    //check to see if saving rotaiotn and then incoming (this allows all overlays to have a rotation but places priority to the saving overlay rotation)
    if(globalVar.savingOverlayRotation[this.index_-1]!=undefined){
        keepRotate(globalVar.savingOverlayRotation[this.index_-1]);
    } else {
        keepRotate(globalVar.incomingPolygonRotation[(this.index_ - 1)]);
    }
    //reset woi to temp just in case we had something different/useful
    globalVar.workingOverlayIndex = temp;
};

//Not currently used
//Supporting URL: https://developers.google.com/maps/documentation/javascript/overlays#CustomOverlays
CustomOverlay.prototype.onRemove = function () {
    this.div_.parentNode.removeChild(this.div_);
    this.div_ = null;
};

//start this whole mess once the google map is loaded
google.maps.event.addDomListener(window, 'load', initialize);

//start the whole thing
//todo make this dynamic and read from the collection
setupInterface("stAugustine"); //defines interface

//#endregion

//#region Define google map objects

//set the google map instance options
//supporting url: https://developers.google.com/maps/documentation/javascript/controls
var gmapPageDivId = "googleMap"; //get page div where google maps is to reside
var gmapOptions = {
    disableDefaultUI: false,                                    //set to false to start from a clean slate of map controls
    zoom: globalVar.defaultZoomLevel,                                     //starting zoom level
    minZoom: globalVar.maxZoomLevel,                                      //highest zoom out level
    center: globalVar.mapCenter,                                          //default center point
    mapTypeId: google.maps.MapTypeId.ROADMAP,                   //default map type to display
    streetViewControl: false,                                   //is streetview active?
    tilt: 0,                                                    //set to 0 to disable 45 degree tilt
    zoomControl: false,                                         //is zoom control active?
    zoomControlOptions: {
        style: google.maps.ZoomControlStyle.SMALL,                //zoom control style
        position: google.maps.ControlPosition.LEFT_TOP            //zoom control position 
    },
    panControl: false,                                          //pan control active
    panControlOptions: {
        position: google.maps.ControlPosition.LEFT_TOP            //pan control position
    },
    mapTypeControl: false,                                      //map layer control active
    mapTypeControlOptions: {
        style: google.maps.MapTypeControlStyle.DROPDOWN_MENU,     //map layer control style
        position: google.maps.ControlPosition.RIGHT_TOP           //map layer control position
    },
    styles:                                                     //turn off all poi stylers (supporting url: https://developers.google.com/maps/documentation/javascript/reference#MapTypeStyleFeatureType)
    [
        {
            featureType: "poi", //poi
            elementType: "all", //or labels
            stylers: [{ visibility: "off" }]
        },
        //{
        //    featureType: "all", //poi
        //    elementType: "all", //labels
        //    stylers: [{ invert_lightness: "true" }]
        //},
        {
            featureType: "transit", //poi
            elementType: "labels", //labels
            stylers: [{ visibility: "off" }]
        }
    ]

};

//define drawing manager for this google maps instance
//support url: https://developers.google.com/maps/documentation/javascript/3.exp/reference#DrawingManager
var drawingManager = new google.maps.drawing.DrawingManager({
    //drawingMode: google.maps.drawing.OverlayType.MARKER, //set default/start type
    drawingControl: false,
    drawingControlOptions: {
        position: google.maps.ControlPosition.TOP_CENTER,
        drawingModes: [
          google.maps.drawing.OverlayType.MARKER,
          google.maps.drawing.OverlayType.CIRCLE,
          google.maps.drawing.OverlayType.RECTANGLE,
          google.maps.drawing.OverlayType.POLYGON,
          google.maps.drawing.OverlayType.POLYLINE
        ]
    },
    markerOptions: {
        draggable: true,
        zIndex: 5
    },
    circleOptions: {
        editable: true,
        draggable: true,
        zIndex: 5
    },
    polygonOptions: {
        editable: true,
        draggable: true,
        zIndex: 5
    },
    polylineOptions: {
        editable: true,
        draggable: true,
        zIndex: 5
    },
    rectangleOptions: {
        editable: true,
        draggable: true,
        zIndex: 5
    }
});
drawingManager.setOptions({
    drawingControl: true, drawingControlOptions: {
        position: google.maps.ControlPosition.RIGHT_TOP,
        drawingModes: [
            google.maps.drawing.OverlayType.MARKER,
            google.maps.drawing.OverlayType.CIRCLE,
            google.maps.drawing.OverlayType.RECTANGLE,
            google.maps.drawing.OverlayType.POLYGON,
            google.maps.drawing.OverlayType.POLYLINE
        ]
    }
});

KmlLayer.setOptions({ suppressinfowindows: true });

//define custom copyright control
//supporting url: https://developers.google.com/maps/documentation/javascript/controls#CustomControls
var copyrightNode = document.createElement('div');
copyrightNode.id = 'copyright-control';
copyrightNode.style.fontSize = '10px';
copyrightNode.style.color = '#333333';
copyrightNode.style.fontFamily = 'Arial, sans-serif';
copyrightNode.style.margin = '0 2px 2px 0';
copyrightNode.style.whiteSpace = 'nowrap';
copyrightNode.index = 0;
copyrightNode.style.backgroundColor = '#FFFFFF';
copyrightNode.style.opacity = 0.75;
copyrightNode.innerHTML = L1; //localization copyright

//define cursor lat long tool custom control
//supporting url: https://developers.google.com/maps/documentation/javascript/controls#CustomControls
var cursorLatLongTool = document.createElement('div');
cursorLatLongTool.id = 'cursorLatLongTool';
cursorLatLongTool.style.fontSize = '10px';
cursorLatLongTool.style.color = '#333333';
cursorLatLongTool.style.fontFamily = 'Arial, sans-serif';
cursorLatLongTool.style.margin = '0 2px 2px 0';
cursorLatLongTool.style.whiteSpace = 'nowrap';
cursorLatLongTool.index = 0;
cursorLatLongTool.style.backgroundColor = '#FFFFFF';
cursorLatLongTool.style.opacity = 0.75;
cursorLatLongTool.innerHTML = L2; //localization cursor lat/long tool

//buffer zone top left (used to push map controls down)
//supporting url: https://developers.google.com/maps/documentation/javascript/controls#CustomControls
toolbarBufferZone1 = document.createElement('div');
toolbarBufferZone1.id = 'toolbarBufferZone1';
toolbarBufferZone1.style.height = '50px';

//buffer zone top right
//supporting url: https://developers.google.com/maps/documentation/javascript/controls#CustomControls
toolbarBufferZone2 = document.createElement('div');
toolbarBufferZone2.id = 'toolbarBufferZone2';
toolbarBufferZone2.style.height = '50px';

//#endregion


//Utilities

//#region Utility Functions

//inits user defined options
function initOptions() {

    toggleVis("mapControls");
    toggleVis("mapControls");
    toggleVis("toolbox");
    toggleVis("toolbox");
    toggleVis("toolbar");
    toggleVis("toolbar");
    toggleVis("kml");
    toggleVis("kml");
    toggleVis("mapDrawingManager");
    toggleVis("mapDrawingManager");
    buttonActive("layer");
    document.getElementById("content_toolbarGrabber").style.display = "block";

    //reset the visual rotation value on page load
    $('.knob').val(0).trigger('change');

    //menubar
    de("[WARN]: #mapedit_container_pane_0 background color must be set manually if changed from default.");
    document.getElementById("mapedit_container_pane_0").style.display = "block";

    //var mainCount = 0;
    ////determine ACL placer type
    //if (globalVar.incomingPointCenter.length > 0) {
    //    //determine if any points are "main"
    //    for (var i = 0; i < globalVar.incomingPointCenter.length; i++) {
    //        if (globalVar.incomingPointFeatureType[i] == "main" || globalVar.incomingPointFeatureType[i] == "") {
    //            mainCount++;
    //            //globalVar.incomingACL = "item";
    //            actionsACL("full", "item");
    //        }
    //    }
    //} else {
    //    if (globalVar.incomingPolygonBounds.length > 0) {
    //        for (var j = 0; j < globalVar.incomingPolygonBounds.length; j++) {
    //            if (globalVar.incomingPolygonFeatureType[j] == "main" || globalVar.incomingPolygonFeatureType[j] == "") {
    //                mainCount++;
    //                //globalVar.incomingACL = "overlay";
    //                actionsACL("full", "overlay");
    //            }
    //        }
    //    } else {
    //        //(if no geo detected, open item first [from there you can convert to overlay] this is just a command decision)
    //        actionsACL("full", "item"); 
    //        //actionsACL("full", "overlay");
    //        //actionsACL("full", "poi"); //not yet implemented
    //    }
    //}

    switch (globalVar.incomingACL) {
        case "item":
            actionsACL("full", "item");
            break;
        case "overlay":
            actionsACL("full", "overlay");
            break;
        case "poi":
            actionsACL("full", "poi");
            break;
        case "none":
            actionsACL("full", "actions");
            break;
    }
    de("main count: " + globalVar.mainCount);

    //determine ACL maptype toggle
    if (globalVar.hasCustomMapType == true) {
        actionsACL("full", "customMapType");
    } else {
        actionsACL("none", "customMapType");
    }

    //set window offload fcn to remind to save
    window.onbeforeunload = function (e) {
        if (globalVar.userMayLoseData) {
            var message = L47,
                e = e || window.event;
            // For IE and Firefox
            if (e) {
                e.returnValue = message;
            }
            // For Safari
            return message;
        } else {
            //do nothing
        }
    };

    //closes loading blanket
    document.getElementById("mapedit_blanket_loading").style.display = "none";

}

//open a specific tab
function openToolboxTab(id) {
    ///<summary>Opens a specific accordian tab</summary>

    //assign numerics to text
    if (id == "search") {
        id = 1;
    }
    if (id == "item") {
        id = 2;
    }
    if (id == "overlay") {
        id = 3;
    }
    if (id == "poi") {
        id = 4;
    }

    $("#mapedit_container_toolboxTabs").accordion({ active: id });
}

//confirm box
function confirmMessage(message) {
    ///<summary>This triggers a confirm messagebox</summary>
    //todo make this a better messagebox (visually pleasing and auto cancel if no action taken)

    var response = confirm(message);
    return response;
}

//facilitates button sticky effect
function buttonActive(id) {
    de("buttonActive: " + id);
    switch (id) {
        case "mapControls":
            if (globalVar.mapControlsDisplayed == false) { //not present
                document.getElementById("content_menubar_toggleMapControls").className = document.getElementById("content_menubar_toggleMapControls").className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
                document.getElementById("content_toolbar_button_toggleMapControls").className = document.getElementById("content_toolbar_button_toggleMapControls").className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                document.getElementById("content_toolbox_button_toggleMapControls").className = document.getElementById("content_toolbox_button_toggleMapControls").className.replace(/(?:^|\s)isActive(?!\S)/g, '');
            } else { //present
                document.getElementById("content_menubar_toggleMapControls").className += " isActive2";
                document.getElementById("content_toolbar_button_toggleMapControls").className += " isActive";
                document.getElementById("content_toolbox_button_toggleMapControls").className += " isActive";
            }
            break;
        case "toolbox":
            if (globalVar.toolboxDisplayed == false) { //not present
                document.getElementById("content_menubar_toggleToolbox").className = document.getElementById("content_menubar_toggleToolbox").className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
                document.getElementById("content_toolbar_button_toggleToolbox").className = document.getElementById("content_toolbar_button_toggleToolbox").className.replace(/(?:^|\s)isActive(?!\S)/g, '');
            } else { //present
                document.getElementById("content_menubar_toggleToolbox").className += " isActive2";
                document.getElementById("content_toolbar_button_toggleToolbox").className += " isActive";
            }
            break;
        case "toolbar":
            if (globalVar.toolbarDisplayed == false) { //not present
                document.getElementById("content_menubar_toggleToolbar").className = document.getElementById("content_menubar_toggleToolbar").className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
            } else { //present
                document.getElementById("content_menubar_toggleToolbar").className += " isActive2";
            }
            break;
        case "layer":
            if (globalVar.prevMapLayerActive != null) {
                document.getElementById("content_menubar_layer" + globalVar.prevMapLayerActive).className = document.getElementById("content_menubar_layer" + globalVar.prevMapLayerActive).className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
                document.getElementById("content_toolbar_button_layer" + globalVar.prevMapLayerActive).className = document.getElementById("content_toolbar_button_layer" + globalVar.prevMapLayerActive).className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                document.getElementById("content_toolbox_button_layer" + globalVar.prevMapLayerActive).className = document.getElementById("content_toolbox_button_layer" + globalVar.prevMapLayerActive).className.replace(/(?:^|\s)isActive(?!\S)/g, '');
            }
            document.getElementById("content_menubar_layer" + globalVar.mapLayerActive).className += " isActive2";
            document.getElementById("content_toolbar_button_layer" + globalVar.mapLayerActive).className += " isActive";
            document.getElementById("content_toolbox_button_layer" + globalVar.mapLayerActive).className += " isActive";
            globalVar.prevMapLayerActive = globalVar.mapLayerActive; //set and hold the previous map layer active
            break;
        case "kml":
            if (globalVar.kmlDisplayed == false) { //not present
                document.getElementById("content_menubar_layerCustom").className = document.getElementById("content_menubar_layerCustom").className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
                document.getElementById("content_toolbar_button_layerCustom").className = document.getElementById("content_toolbar_button_layerCustom").className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                document.getElementById("content_toolbox_button_layerCustom").className = document.getElementById("content_toolbox_button_layerCustom").className.replace(/(?:^|\s)isActive(?!\S)/g, '');
            } else { //present
                document.getElementById("content_menubar_layerCustom").className += " isActive2";
                document.getElementById("content_toolbar_button_layerCustom").className += " isActive";
                document.getElementById("content_toolbox_button_layerCustom").className += " isActive";
            }
            break;
        case "action":
            de("aa: " + globalVar.actionActive + "<br>" + "paa: " + globalVar.prevActionActive);
            if (globalVar.actionActive == "Other") {
                if (globalVar.prevActionActive != null) {
                    document.getElementById("content_menubar_manage" + globalVar.prevActionActive).className = document.getElementById("content_menubar_manage" + globalVar.prevActionActive).className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
                    document.getElementById("content_toolbar_button_manage" + globalVar.prevActionActive).className = document.getElementById("content_toolbar_button_manage" + globalVar.prevActionActive).className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                    document.getElementById("content_toolbox_button_manage" + globalVar.prevActionActive).className = document.getElementById("content_toolbox_button_manage" + globalVar.prevActionActive).className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                }
            } else {
                if (globalVar.prevActionActive != null) {
                    document.getElementById("content_menubar_manage" + globalVar.prevActionActive).className = document.getElementById("content_menubar_manage" + globalVar.prevActionActive).className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
                    document.getElementById("content_toolbar_button_manage" + globalVar.prevActionActive).className = document.getElementById("content_toolbar_button_manage" + globalVar.prevActionActive).className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                    if (document.getElementById("content_toolbox_button_manage" + globalVar.prevActionActive)) {
                        de("found " + globalVar.prevActionActive);
                        document.getElementById("content_toolbox_button_manage" + globalVar.prevActionActive).className = document.getElementById("content_toolbox_button_manage" + globalVar.prevActionActive).className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                    }

                }
                document.getElementById("content_menubar_manage" + globalVar.actionActive).className += " isActive2";
                document.getElementById("content_toolbar_button_manage" + globalVar.actionActive).className += " isActive";
                if (document.getElementById("content_toolbox_button_manage" + globalVar.actionActive)) {
                    de("found " + globalVar.actionActive);
                    document.getElementById("content_toolbox_button_manage" + globalVar.actionActive).className += " isActive";
                }
                globalVar.prevActionActive = globalVar.actionActive; //set and hold the previous map layer active
            }
            break;
            //todo move these into a seperate fcn
        //case "itemPlace":
        //    try {
        //        document.getElementById("content_menubar_itemPlace").className = document.getElementById("content_menubar_itemPlace").className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
        //        document.getElementById("content_toolbox_button_itemPlace").className = document.getElementById("content_toolbox_button_itemPlace").className.replace(/(?:^|\s)isActive(?!\S)/g, '');
        //    }catch (e)
        //    {
        //        document.getElementById("content_menubar_itemPlace").className += " isActive2";
        //        document.getElementById("content_toolbox_button_itemPlace").className += " isActive";
        //    }
        //    break;
        //case "itemPlace2":
        //    if (globalVar.buttonActive_itemPlace == false) { //not present
        //        document.getElementById("content_menubar_itemPlace").className = document.getElementById("content_menubar_itemPlace").className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
        //        document.getElementById("content_toolbox_button_itemPlace").className = document.getElementById("content_toolbox_button_itemPlace").className.replace(/(?:^|\s)isActive(?!\S)/g, '');
        //        globalVar.buttonActive_itemPlace = true;
        //    } else { //present
        //        document.getElementById("content_menubar_itemPlace").className += " isActive2";
        //        document.getElementById("content_toolbox_button_itemPlace").className += " isActive";
        //        globalVar.buttonActive_itemPlace = false;
        //    }
        //    break;
        //case "overlayEdit":
        //    if (globalVar.buttonActive_overlayEdit == false) { //not present
        //        document.getElementById("content_menubar_overlayEdit").className = document.getElementById("content_menubar_overlayEdit").className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
        //        document.getElementById("content_toolbox_button_overlayEdit").className = document.getElementById("content_toolbox_button_overlayEdit").className.replace(/(?:^|\s)isActive(?!\S)/g, '');
        //        globalVar.buttonActive_overlayEdit = true;
        //    } else { //present
        //        document.getElementById("content_menubar_overlayEdit").className += " isActive2";
        //        document.getElementById("content_toolbox_button_overlayEdit").className += " isActive";
        //        globalVar.buttonActive_overlayEdit = false;
        //    }
        //    break;
        //case "overlayPlace":
        //    if (globalVar.buttonActive_overlayPlace == false) { //not present
        //        document.getElementById("content_menubar_overlayPlace").className = document.getElementById("content_menubar_overlayPlace").className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
        //        document.getElementById("content_toolbox_button_overlayPlace").className = document.getElementById("content_toolbox_button_overlayPlace").className.replace(/(?:^|\s)isActive(?!\S)/g, '');
        //        globalVar.buttonActive_overlayPlace = true;
        //    } else { //present
        //        document.getElementById("content_menubar_overlayPlace").className += " isActive2";
        //        document.getElementById("content_toolbox_button_overlayPlace").className += " isActive";
        //        globalVar.buttonActive_overlayPlace = false;
        //    }
        //    break;
        case "overlayToggle":
            if (globalVar.buttonActive_overlayToggle == false) { //not present
                document.getElementById("content_menubar_overlayToggle").className += " isActive2";
                document.getElementById("content_toolbox_button_overlayToggle").className += " isActive";
                globalVar.buttonActive_overlayToggle = true;
            } else { //present
                document.getElementById("content_menubar_overlayToggle").className = document.getElementById("content_menubar_overlayToggle").className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
                document.getElementById("content_toolbox_button_overlayToggle").className = document.getElementById("content_toolbox_button_overlayToggle").className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                globalVar.buttonActive_overlayToggle = false;
            }
            break;
        //case "poiPlace":
        //    if (globalVar.buttonActive_poiPlace == false) { //not present
        //        document.getElementById("content_menubar_overlayPlace").className = document.getElementById("content_menubar_poiPlace").className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
        //        document.getElementById("content_toolbox_button_poiPlace").className = document.getElementById("content_toolbox_button_poiPlace").className.replace(/(?:^|\s)isActive(?!\S)/g, '');
        //        globalVar.buttonActive_poiPlace = true;
        //    } else { //present
        //        document.getElementById("content_menubar_poiPlace").className += " isActive2";
        //        document.getElementById("content_toolbox_button_poiPlace").className += " isActive";
        //        globalVar.buttonActive_poiPlace = false;
        //    }
        //    break;
        case "poiToggle":
            if (globalVar.buttonActive_poiToggle == false) { //not present
                document.getElementById("content_menubar_poiToggle").className += " isActive2";
                document.getElementById("content_toolbox_button_poiToggle").className += " isActive";
                globalVar.buttonActive_poiToggle = true;
            } else { //present
                document.getElementById("content_menubar_poiToggle").className = document.getElementById("content_menubar_poiToggle").className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
                document.getElementById("content_toolbox_button_poiToggle").className = document.getElementById("content_toolbox_button_poiToggle").className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                globalVar.buttonActive_poiToggle = false;
            }
            break;
        //case "poiMarker":
        //    if (globalVar.buttonActive_poiMarker == false) { //not present
        //        document.getElementById("content_menubar_poiMarker").className = document.getElementById("content_menubar_poiMarker").className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
        //        document.getElementById("content_toolbox_button_poiMarker").className = document.getElementById("content_toolbox_button_poiMarker").className.replace(/(?:^|\s)isActive(?!\S)/g, '');
        //        globalVar.buttonActive_poiMarker = true;
        //    } else { //present
        //        document.getElementById("content_menubar_poiMarker").className += " isActive2";
        //        document.getElementById("content_toolbox_button_poiMarker").className += " isActive";
        //        globalVar.buttonActive_poiMarker = false;
        //    }
        //    break;
        //case "poiCircle":
        //    if (globalVar.buttonActive_poiCircle == false) { //not present
        //        document.getElementById("content_menubar_poiCircle").className = document.getElementById("content_menubar_poiCircle").className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
        //        document.getElementById("content_toolbox_button_poiCircle").className = document.getElementById("content_toolbox_button_poiCircle").className.replace(/(?:^|\s)isActive(?!\S)/g, '');
        //        globalVar.buttonActive_poiCircle = true;
        //    } else { //present
        //        document.getElementById("content_menubar_poiCircle").className += " isActive2";
        //        document.getElementById("content_toolbox_button_poiCircle").className += " isActive";
        //        globalVar.buttonActive_poiCircle = true;
        //    }
        //    break;
        //case "poiRectangle":
        //    if (globalVar.buttonActive_poiRectangle == false) { //not present
        //        document.getElementById("content_menubar_poiRectangle").className = document.getElementById("content_menubar_poiRectangle").className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
        //        document.getElementById("content_toolbox_button_poiRectangle").className = document.getElementById("content_toolbox_button_poiRectangle").className.replace(/(?:^|\s)isActive(?!\S)/g, '');
        //        globalVar.buttonActive_poiRectangle = true;
        //    } else { //present
        //        document.getElementById("content_menubar_poiRectangle").className += " isActive2";
        //        document.getElementById("content_toolbox_button_poiRectangle").className += " isActive";
        //        globalVar.buttonActive_poiRectangle = false;
        //    }
        //    break;
        //case "poiPolygon":
        //    if (globalVar.buttonActive_poiPolygon == false) { //not present
        //        document.getElementById("content_menubar_poiPolygon").className = document.getElementById("content_menubar_poiPolygon").className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
        //        document.getElementById("content_toolbox_button_poiPolygon").className = document.getElementById("content_toolbox_button_poiPolygon").className.replace(/(?:^|\s)isActive(?!\S)/g, '');
        //        globalVar.buttonActive_poiRectangle = true;
        //    } else { //present
        //        document.getElementById("content_menubar_poiPolygon").className += " isActive2";
        //        document.getElementById("content_toolbox_button_poiPolygon").className += " isActive";
        //        globalVar.buttonActive_poiRectangle = false;
        //    }
        //    break;
        //case "poiLine":
        //    if (globalVar.buttonActive_poiLine == false) { //not present
        //        document.getElementById("content_menubar_poiLine").className = document.getElementById("content_menubar_poiLine").className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
        //        document.getElementById("content_toolbox_button_poiLine").className = document.getElementById("content_toolbox_button_poiLine").className.replace(/(?:^|\s)isActive(?!\S)/g, '');
        //        globalVar.buttonActive_poiLine = true;
        //    } else { //present
        //        document.getElementById("content_menubar_poiLine").className += " isActive2";
        //        document.getElementById("content_toolbox_button_poiLine").className += " isActive";
        //        globalVar.buttonActive_poiLine = false;
        //    }
        //    break;
    }
    de("buttonAction() completed");
}

//display an inline message
function displayMessage(message) {

    //debug log this message
    de("message #" + globalVar.messageCount + ": " + message); //send to debugger for logging

    //keep a count of messages
    globalVar.messageCount++;

    //check to see if RIB is on
    if (globalVar.RIBMode == true) {
        de("RIB Mode: " + globalVar.RIBMode);
        return;
    } else {
        //display the message

        //debug
        de("RIB Mode: " + globalVar.RIBMode);

        //compile divID
        var currentDivId = "message" + globalVar.messageCount;

        //create unique message div
        var messageDiv = document.createElement("div");
        messageDiv.setAttribute("id", currentDivId);
        messageDiv.className = "message";
        document.getElementById("content_message").appendChild(messageDiv);

        //assign the message
        document.getElementById(currentDivId).innerHTML = message;

        //show message
        document.getElementById(currentDivId).style.display = "block"; //display element

        //fade message out
        setTimeout(function () {
            $("#" + currentDivId).fadeOut("slow", function () {
                $("#" + currentDivId).remove();
            });
        }, 3000); //after 3 sec
    }
}

//create a package to send to server to save item location
function createSavedItem(handle, coordinates) {
    var messageType = handle + "|" + "item"; //define what message type it is
    //assign data
    var data = messageType + "|" + coordinates + "|";
    var dataPackage = data + "~";
    de("saving item: " + dataPackage); //temp
    toServer(dataPackage);
}

//create a package to send to server to save overlay
function createSavedOverlay(handle, pageId, label, source, bounds, rotation) {
    var temp = source;
    if (temp.contains("~") || temp.contains("|")) { //check to make sure reserve characters are not there
        displayMessage(L7);
    }
    //var formattedBounds = 
    var messageType = handle + "|" + "overlay"; //define what message type it is
    var data = messageType + "|" + pageId + "|" + label + "|" + bounds + "|" + source + "|" + rotation + "|";
    var dataPackage = data + "~";
    de("saving overlay set: " + dataPackage); //temp
    toServer(dataPackage);
}

//create a package to send to the server to save poi
function createSavedPOI(handle) {
    var dataPackage = "";
    //cycle through all pois
    de("poi length: " + globalVar.poiObj.length);
    for (var i = 0; i < globalVar.poiObj.length; i++) {
        //get specific geometry 
        switch (globalVar.poiType[i]) {
            case "marker":
                globalVar.poiKML[i] = globalVar.poiObj[i].getPosition().toString();
                break;
            case "circle":
                globalVar.poiKML[i] = globalVar.poiObj[i].getCenter() + "|";
                globalVar.poiKML[i] += globalVar.poiObj[i].getRadius();
                break;
            case "rectangle":
                globalVar.poiKML[i] = globalVar.poiObj[i].getBounds().toString();
                break;
            case "polygon":
                globalVar.poiObj[i].getPath().forEach(function (latLng) {
                    globalVar.poiKML[i] += "|";
                    globalVar.poiKML[i] += latLng; //NOTE: this has a non standard coordinate separator    
                });
                break;
            case "polyline":
                globalVar.poiObj[i].getPath().forEach(function (latLng) {
                    globalVar.poiKML[i] += "|";
                    globalVar.poiKML[i] += latLng; //NOTE: this has a non standard coordinate separator    
                });
                break;
            case "deleted":
                //nothing to do here, just a placeholder
                break;
        }
        //filter out the deleted pois
        if (globalVar.poiType[i] != "deleted") {
            //compile data message
            var data = handle + "|" + "poi|" + globalVar.poiType[i] + "|" + globalVar.poiDesc[i] + "|" + globalVar.poiKML[i] + "|";
            dataPackage += data + "~";
        }
    }
    de("saving overlay set: " + dataPackage); //temp  
    //add another filter to catch if datapackage is null
    if (dataPackage != null) {
        toServer(dataPackage); //send to server to save    
    }

}

//sends save dataPackages to the server via json
function toServer(dataPackage) {
    jQuery('form').each(function() {

        var payload = JSON.stringify({ sendData: dataPackage });
        var hiddenfield = document.getElementById('payload');
        hiddenfield.value = payload;
        var hiddenfield2 = document.getElementById('action');
        hiddenfield2.value = 'save';

        //reset success marker
        globalVar.toServerSuccess = false;
        $.ajax({
            type: "POST",
            async: true,
            url: window.location.href.toString(),
            data: jQuery(this).serialize(),
            success: function(result) {
                //de("server result:" + result);
                displayMessage(L_Saved);
                globalVar.toServerSuccess = true;
                globalVar.csoi = 0; //reset
            }
        });
    });
}

//centeres on an overlay
function overlayCenterOnMe(id) {
    //attempt to pan to center of overlay
    map.panTo(globalVar.ghostOverlayRectangle[id].getBounds().getCenter());
}

//toggles overlay for editing
function overlayEditMe(id) {
    de("editing overlay id: " + id);
    //alert("editing... \noverlay id: " + id + "\nwoi: " + globalVar.workingOverlayIndex + "\nsor: " + globalVar.savingOverlayRotation[globalVar.workingOverlayIndex] + "\nipr: " + globalVar.incomingPolygonRotation[globalVar.workingOverlayIndex] + "\npr: " + globalVar.preservedRotation);
    //check to see if overlay is indeed a bonified overlay (on the map)
    try {
        //indicate we are editing
        globalVar.pageMode = "edit";
        //if editing is being done and there is something to save, save
        if (globalVar.currentlyEditing == "yes" && globalVar.workingOverlayIndex != null) {
            de("saving overlay " + globalVar.workingOverlayIndex);
            //trigger a cache of current working overlay
            cacheSaveOverlay(globalVar.workingOverlayIndex);
            //set globalVar.rectangle to globalVar.ghosting
            globalVar.ghostOverlayRectangle[globalVar.workingOverlayIndex].setOptions(globalVar.ghosting);
            //reset editing marker
            globalVar.currentlyEditing = "no";
            //set preserved rotation to the rotation of the current overlay
            //alert("setting preserved rotation to globalVar.savingOverlayRotation[" + (globalVar.workingOverlayIndex-1) + "] (" + globalVar.savingOverlayRotation[(globalVar.workingOverlayIndex-1)] + ")");
            globalVar.workingOverlayIndex = id;
            globalVar.preservedRotation = globalVar.savingOverlayRotation[globalVar.workingOverlayIndex - 1];
            //globalVar.preservedRotation = 0;
        }
        //if editing is not being done, make it so
        if (globalVar.currentlyEditing == "no" || globalVar.workingOverlayIndex == null) {
            //enable editing marker
            globalVar.currentlyEditing = "yes";
            de("editing overlay " + globalVar.workingOverlayIndex);
            //set preserved rotation value
            //globalVar.preservedRotation = globalVar.savingOverlayRotation[globalVar.workingOverlayIndex - 1];
            //set visual rotation knob value
            //$('.knob').val(globalVar.savingOverlayRotation[globalVar.workingOverlayIndex - 1]).trigger('change');
            //alert("setting knob to: globalVar.savingOverlayRotation[" + (globalVar.workingOverlayIndex-1) + "] (" + globalVar.savingOverlayRotation[(globalVar.workingOverlayIndex-1)] + ")");
            //de("setting knob to: " + globalVar.savingOverlayRotation[globalVar.workingOverlayIndex - 1]);
            
            //try {
            //    if (globalVar.savingOverlayRotation[globalVar.workingOverlayIndex - 1] < 0) {
            //        $('.knob').val(((180 + globalVar.savingOverlayRotation[globalVar.workingOverlayIndex - 1]) + 180)).trigger('change');
            //        de("setting knob to: " + ((180 + globalVar.savingOverlayRotation[globalVar.workingOverlayIndex - 1]) + 180));
            //    } else {
            //        $('.knob').val(globalVar.savingOverlayRotation[globalVar.workingOverlayIndex - 1]).trigger('change');
            //        //alert("setting knob to: globalVar.savingOverlayRotation[" + globalVar.workingOverlayIndex + "] (" + globalVar.savingOverlayRotation[globalVar.workingOverlayIndex] + ")");
            //        de("setting knob to: " + globalVar.savingOverlayRotation[globalVar.workingOverlayIndex - 1]);
            //    }
            //} catch (e) {
            //    de("rotation error catch: " + e);
            //}

            if (globalVar.savingOverlayRotation[globalVar.workingOverlayIndex - 1] != null) {
                globalVar.preservedRotation = globalVar.savingOverlayRotation[globalVar.workingOverlayIndex - 1];
                //set visual rotation knob value
                try {
                    if (globalVar.savingOverlayRotation[globalVar.workingOverlayIndex - 1] < 0) {
                        $('.knob').val((180 + globalVar.savingOverlayRotation[globalVar.workingOverlayIndex - 1]) + 180).trigger('change');
                        //$('.knob').val(((180 + globalVar.savingOverlayRotation[globalVar.workingOverlayIndex - 1]) + 180)).trigger('change');
                        //alert("setting knob to: globalVar.savingOverlayRotation[" + (globalVar.workingOverlayIndex - 1) + "] (" + globalVar.savingOverlayRotation[(globalVar.workingOverlayIndex - 1)] + ")");
                        de("setting knob to: " + ((180 + globalVar.savingOverlayRotation[globalVar.workingOverlayIndex - 1]) + 180));
                    } else {
                        $('.knob').val(globalVar.savingOverlayRotation[globalVar.workingOverlayIndex - 1]).trigger('change');
                        //alert("setting knob to: globalVar.savingOverlayRotation[" + (globalVar.workingOverlayIndex - 1) + "] (" + globalVar.savingOverlayRotation[(globalVar.workingOverlayIndex - 1)] + ")");
                        de("setting knob to: " + globalVar.savingOverlayRotation[globalVar.workingOverlayIndex - 1]);
                    }
                } catch (e) {
                    de("rotation error catch: " + e);
                }
            } else {
                if (globalVar.incomingPolygonRotation[globalVar.workingOverlayIndex - 1] != null) {
                    globalVar.preservedRotation = globalVar.incomingPolygonRotation[globalVar.workingOverlayIndex - 1];
                }
                //set visual rotation knob value
                try {
                    if (globalVar.incomingPolygonRotation[globalVar.workingOverlayIndex - 1] < 0) {
                        //$('.knob').val(globalVar.incomingPolygonRotation[globalVar.workingOverlayIndex - 1]).trigger('change');
                        $('.knob').val(((180 + globalVar.incomingPolygonRotation[globalVar.workingOverlayIndex - 1]) + 180)).trigger('change');
                        //alert("setting knob to: globalVar.incomingPolygonRotation[" + (globalVar.workingOverlayIndex - 1) + "] (" + globalVar.incomingPolygonRotation[(globalVar.workingOverlayIndex - 1)] + ")");
                        de("setting knob to: " + ((180 + globalVar.incomingPolygonRotation[globalVar.workingOverlayIndex - 1]) + 180));
                    } else {
                        $('.knob').val(globalVar.incomingPolygonRotation[globalVar.workingOverlayIndex - 1]).trigger('change');
                        //alert("setting knob to: globalVar.incomingPolygonRotation[" + (globalVar.workingOverlayIndex - 1) + "] (" + globalVar.incomingPolygonRotation[(globalVar.workingOverlayIndex - 1)] + ")");
                        de("setting knob to: " + globalVar.incomingPolygonRotation[globalVar.workingOverlayIndex - 1]);
                    }
                } catch (e) {
                    de("rotation error catch: " + e);
                }
            }
            
            //show ghost
            globalVar.ghostOverlayRectangle[globalVar.workingOverlayIndex].setOptions(globalVar.editable);
            //iterate top z index
            globalVar.currentTopZIndex++;
            //bring overlay to front
            document.getElementById("overlay" + globalVar.workingOverlayIndex).style.zIndex = globalVar.currentTopZIndex;
            //bring ghost to front
            globalVar.ghostOverlayRectangle[globalVar.workingOverlayIndex].setOptions({ zIndex: globalVar.currentTopZIndex });
            //recenter on the overlay
            overlayCenterOnMe(id);
        }
        //indicate to user we are editing a polygon
        displayMessage(L34 + " " + globalVar.incomingPolygonLabel[(id-1)]);
    } catch(e) {
        //create the overlay
        createOverlayFromPage(id);
    }
}

//hide poi on map
function overlayHideMe(id) {
    try {
        globalVar.overlaysOnMap[id].setMap(null);
        globalVar.ghostOverlayRectangle[id].setMap(null);
        document.getElementById("overlayToggle" + id).innerHTML = "<img src=\"" + globalVar.baseURL + globalVar.baseImageDirURL + "add.png\" onclick=\"overlayShowMe(" + id + ");\" />";
        displayMessage(L31 + " " + globalVar.incomingPolygonLabel[id]);
    } catch(e) {
        displayMessage(localize.L56); //nothing to hide
    } 
}

//show poi on map
function overlayShowMe(id) {
    globalVar.overlaysOnMap[id].setMap(map);
    globalVar.ghostOverlayRectangle[id].setMap(map);
    document.getElementById("overlayToggle" + id).innerHTML = "<img src=\"" + globalVar.baseURL + globalVar.baseImageDirURL + "sub.png\" onclick=\"overlayHideMe(" + id + ");\" />";
    displayMessage(L32 + " " + globalVar.incomingPolygonLabel[id]);
}

//delete poi from map and list
function overlayDeleteMe(id) {
    try {
        globalVar.overlaysOnMap[id].setMap(null);
        globalVar.overlaysOnMap[id] = null;
        globalVar.ghostOverlayRectangle[id].setMap(null);
        globalVar.ghostOverlayRectangle[id] = null;
        var strg = "#overlayListItem" + id; //create <li> poi string
        $(strg).remove(); //remove <li>
        globalVar.overlayCount--;
        displayMessage(id + " " + L33);
    } catch(e) {
        displayMessage(localize.L57); //nothing to delete
    } 
}

//open the infoWindow of a poi
function poiEditMe(id) {
    globalVar.poiObj[id].setMap(map);
    infoWindow[id].setMap(map);
    label[id].setMap(map);
    //document.getElementById("overlayListItem" + id).style.backgroundColor = "red"; //not implemented yet
    document.getElementById("poiToggle" + id).innerHTML = "<img src=\"" + globalVar.baseURL + globalVar.baseImageDirURL + "sub.png\" onclick=\"poiHideMe(" + id + ");\" />";
}

//hide poi on map
function poiHideMe(id) {
    globalVar.poiObj[id].setMap(null);
    infoWindow[id].setMap(null);
    label[id].setMap(null);
    document.getElementById("poiToggle" + id).innerHTML = "<img src=\"" + globalVar.baseURL + globalVar.baseImageDirURL + "add.png\" onclick=\"poiShowMe(" + id + ");\" />";
}

//show poi on map
function poiShowMe(id) {
    globalVar.poiObj[id].setMap(map);
    infoWindow[id].setMap(map);
    label[id].setMap(map);
    document.getElementById("poiToggle" + id).innerHTML = "<img src=\"" + globalVar.baseURL + globalVar.baseImageDirURL + "sub.png\" onclick=\"poiHideMe(" + id + ");\" />";
}

//delete poi from map and list
function poiDeleteMe(id) {
    globalVar.poiObj[id].setMap(null);
    globalVar.poiObj[id] = null;
    globalVar.poiType[id] = "deleted";
    var strg = "#poi" + id; //create <li> poi string
    $(strg).remove(); //remove <li>
    infoWindow[id].setMap(null);
    label[id].setMap(null);
}

//get the poi desc
function poiGetDesc(id) {
    de("poiGetDesc(" + id + "); started...");
    //filter to not set desc to blank
    if (document.getElementById("poiDesc" + id).value == "") {
        return;
    } else {
        //get the desc
        var temp = document.getElementById("poiDesc" + id).value;

        //check for invalid characters
        if (temp.contains("~") || temp.contains("|")) {
            displayMessage(L8);
        } else {

            de("poiDesc[id]: " + globalVar.poiDesc[id]);
            de("temp: " + temp);

            //replace the list item title 
            var tempHTMLHolder1 = document.getElementById("poiList").innerHTML.replace(globalVar.poiDesc[id], temp);
            document.getElementById("poiList").innerHTML = tempHTMLHolder1;

            //de("tempHTMLHolder1: " + tempHTMLHolder1);
            de("globalVar.poiDesc[id].substring(0, 20): " + globalVar.poiDesc[id].substring(0, 20));
            de("temp.substring(0, 20): " + temp.substring(0, 20));

            //now replace the list item (order is important)
            var tempHTMLHolder2 = document.getElementById("poiList").innerHTML.replace(">" + globalVar.poiDesc[id].substring(0, 20), ">" + temp.substring(0, 20));
            //now post all this back to the listbox
            document.getElementById("poiList").innerHTML = tempHTMLHolder2;

            de("tempHTMLHolder2: " + tempHTMLHolder2);
            de("label[id]" + label[id]);
            de("temp.substring(0, 20): " + temp.substring(0, 20));

            //replace the object label
            label[id].set("labelContent", temp.substring(0, 20));

            de("globalVar.poiDesc[id]: " + globalVar.poiDesc[id]);
            de("temp: " + temp);

            //assign full description to the poi object
            globalVar.poiDesc[id] = temp;

            //visually set desc
            //infoWindow[id].setOptions({ content: writeHTML("poiDesc", id, "", "") });

            infoWindow[id] = new google.maps.InfoWindow({
                content: writeHTML("poiDescIncoming", id, temp, "")
            });

            //close the poi desc box
            infoWindow[id].setMap(null);
        }
    }
    de("poiGetDesc(" + id + "); finished...");
}

//hide search result from map and list
function searchResultHideMe(id) {
    globalVar.searchResult.setMap(null); //remove from map
    document.getElementById("searchResultToggle" + id).innerHTML = "<img src=\"" + globalVar.baseURL + globalVar.baseImageDirURL + "add.png\" onclick=\"searchResultShowMe(" + id + ");\" />";
}

//show search result on map and list
function searchResultShowMe(id) {
    globalVar.searchResult.setMap(map); //remove from map
    document.getElementById("searchResultToggle" + id).innerHTML = "<img src=\"" + globalVar.baseURL + globalVar.baseImageDirURL + "sub.png\" onclick=\"searchResultHideMe(" + id + ");\" />";
}

//delete search results from map and list
function searchResultDeleteMe(id) {
    //remove visually
    globalVar.searchResult.setMap(null); //remove from map
    $("#searchResultListItem" + id).remove(); //remove the first result div from result list box in toolbox
    document.getElementById("content_toolbar_searchField").value = ""; //clear searchbar
    document.getElementById("content_toolbox_searchField").value = ""; //clear searchbox

    //remove references to 
    globalVar.searchResult = null; //reset search result map item
    globalVar.searchCount = 0; //reset search count
}

//used for lat/long tool
function DisplayCursorCoords(arg) {
    cCoord.innerHTML = arg;
}

//check the zoom level
function checkZoomLevel() {
    var currentZoomLevel = map.getZoom();
    var currentMapType = map.getMapTypeId();
    if (currentZoomLevel == globalVar.maxZoomLevel) {
        displayMessage(L16);
    } else {
        switch (currentMapType) {
            case "roadmap": //roadmap and default
                if (currentZoomLevel == globalVar.minZoomLevel_Roadmap) {
                    displayMessage(L17);
                }
                break;
            case "satellite": //sat
                if (currentZoomLevel == globalVar.minZoomLevel_Satellite) {
                    displayMessage(L17);
                }
                break;
            case "hybrid": //sat
                if (currentZoomLevel == globalVar.minZoomLevel_Satellite) {
                    displayMessage(L17);
                }
                break;
            case "terrain": //terrain only
                if (currentZoomLevel == globalVar.minZoomLevel_Terrain) {
                    displayMessage(L17);
                }
                break;
            case "blocklot":
                if (currentZoomLevel == globalVar.minZoomLevel_BlockLot) {
                    displayMessage(L17);
                }
                break;
        }
        if (globalVar.isCustomOverlay == true) {
            if (currentZoomLevel == globalVar.minZoomLevel_BlockLot) {
                displayMessage(L17);
            }
        }
    }
}

//jquery transparency slider
$(function () {
    $("#overlayTransparencySlider").slider({
        animate: true,
        range: "min",
        value: globalVar.preservedOpacity,
        orientation: "vertical",
        min: 0.00,
        max: 1.00,
        step: 0.01,
        slide: function (event, ui) {
            if (globalVar.currentlyEditing == "yes") {
                //if (globalVar.pageMode == "edit") {
                var selection = $("#overlayTransparencySlider").slider("value");
                de("opacity selected: " + selection);
                keepOpacity(selection);
            }
        }
    });
});

//keeps a specific opacity
function keepOpacity(opacityIn) {
    de("keepOpacity: " + opacityIn);
    var div = document.getElementById("overlay" + globalVar.workingOverlayIndex);
    div.style.opacity = opacityIn;
    globalVar.preservedOpacity = opacityIn;
}

//used to specify a variable opacity (IE adds value to existing)
function opacity(opacityIn) {

    if (globalVar.preservedOpacity <= 1 && globalVar.preservedOpacity >= 0) {
        de("add opacity: " + opacityIn + " to overlay" + globalVar.workingOverlayIndex);
        var div = document.getElementById("overlay" + globalVar.workingOverlayIndex);
        var newOpacity = globalVar.preservedOpacity + opacityIn;
        if (newOpacity > 1) {
            newOpacity = 1;
        }
        if (newOpacity < 0) {
            newOpacity = 0;
        }
        div.style.opacity = newOpacity;
        de("newOpacity: " + newOpacity);
        globalVar.preservedOpacity = newOpacity;
        de("globalVar.preservedOpacity: " + globalVar.preservedOpacity);
        $("#overlayTransparencySlider").slider({ value: globalVar.preservedOpacity });
    } else {
        //could not change the opacity    
    }


}

//jquery rotation knob
$(function ($) {
    $(".knob").knob({
        change: function (value) {
            if (globalVar.currentlyEditing == "yes") {
                //if (globalVar.pageMode == "edit") {
                globalVar.knobRotationValue = value; //assign knob value
                if (value > 180) {
                    globalVar.knobRotationValue = ((globalVar.knobRotationValue - 360) * (1)); //used to correct for visual effect of knob error
                    //globalVar.knobRotationValue = ((globalVar.knobRotationValue-180)*(-1));
                }
                //only do something if we are in pageEdit Mode and there is an overlay to apply these changes to
                if (globalVar.workingOverlayIndex != null) {
                    globalVar.preservedRotation = globalVar.knobRotationValue; //reassign
                    keepRotate(globalVar.preservedRotation); //send to display fcn of rotation
                    de("setting rotation from knob at wroking index: " + globalVar.workingOverlayIndex + "to value: " + globalVar.preservedRotation);
                    globalVar.savingOverlayRotation[globalVar.workingOverlayIndex-1] = globalVar.preservedRotation; //just make sure it is prepping for save    
                }
            }
        }
    });
});

//used to maintain specific rotation of overlay
function keepRotate(degreeIn) {
    //alert("keepRotate: " + degreeIn + " woi: " + globalVar.workingOverlayIndex);
    globalVar.currentlyEditing = "yes"; //used to signify we are editing this overlay
    $(function () {
        $("#overlay" + globalVar.workingOverlayIndex).rotate(degreeIn);
        
        if (degreeIn < 0) {
            $('.knob').val(((180 + degreeIn) + 180)).trigger('change');
        } else {
            $('.knob').val(degreeIn).trigger('change');
        }

        //if (degreeIn > 180) {
        //    $('.knob').val(((degreeIn - 360) * (1))).trigger('change'); //used to correct for visual effect of knob error
        //} else {
        //    $('.knob').val(degreeIn).trigger('change');
        //}
    });
}

//used to specify a variable rotation
function rotate(degreeIn) {
    if (globalVar.currentlyEditing == "yes") {
    //if (globalVar.pageMode == "edit") {
        globalVar.currentlyEditing = "yes"; //used to signify we are editing this overlay
        globalVar.degree = globalVar.preservedRotation;
        globalVar.degree += degreeIn;
        if (degreeIn != 0) {
            $(function() {
                $("#overlay" + globalVar.workingOverlayIndex).rotate(globalVar.degree); //assign overlay to defined rotation
            });
        } else { //if rotation is 0, reset rotation
            $(function() {
                globalVar.degree = 0;
                $("#overlay" + globalVar.workingOverlayIndex).rotate(globalVar.degree);
            });
        }
        globalVar.preservedRotation = globalVar.degree; //preserve rotation value
        //globalVar.savingOverlayRotation[globalVar.workingOverlayIndex] = globalVar.preservedRotation; //just make sure it is prepping for save
    }
    if (globalVar.degree > 180) {
        $('.knob').val(((globalVar.degree - 360) * (1))).trigger('change'); //used to correct for visual effect of knob error

    } else {
        $('.knob').val(globalVar.degree).trigger('change');
    }
}

//get the center lat/long of a polygon
function polygonCenter(poly) {
    var lowx,
        highx,
        lowy,
        highy,
        lats = [],
        lngs = [],
        vertices = poly.getPath();

    for (var i = 0; i < vertices.length; i++) {
        lngs.push(vertices.getAt(i).lng());
        lats.push(vertices.getAt(i).lat());
    }

    lats.sort();
    lngs.sort();
    lowx = lats[0];
    highx = lats[vertices.length - 1];
    lowy = lngs[0];
    highy = lngs[vertices.length - 1];
    center_x = lowx + ((highx - lowx) / 2);
    center_y = lowy + ((highy - lowy) / 2);
    return (new google.maps.LatLng(center_x, center_y));
}

//test the map bounds
function testBounds() {
    if (globalVar.strictBounds != null) {
        if (globalVar.strictBounds.contains(map.getCenter())) {
            globalVar.mapInBounds = "yes";
        } else {
            globalVar.mapInBounds = "no";
            map.panTo(globalVar.mapCenter); //recenter
            displayMessage(L5);
        }
    }
}

//search the gmaps for a location (lat/long or address)
function finder(stuff) {
    if (stuff.length > 0) {
        codeAddress("lookup", stuff); //find the thing
        document.getElementById("content_menubar_searchField").value = stuff; //sync menubar
        document.getElementById("content_toolbar_searchField").value = stuff; //sync toolbar
        document.getElementById("content_toolbox_searchField").value = stuff; //sync toolbox
        action("other"); //needed to clear out any action buttons that may be active
        de("opening");
        openToolboxTab(1); //open the actions tab
        de("supposedly opened");
    } else {
        //do nothing and keep quiet
    }
}

//get the location of a lat/long or address
function codeAddress(type, geo) {
    var bounds = map.getBounds(); //get the current map bounds (should not be greater than the bounding box)
    geocoder.geocode({ 'address': geo, 'bounds': bounds }, function (results, status) { //geocode the lat/long of incoming with a bias towards the bounds
        if (status == google.maps.GeocoderStatus.OK) { //if it worked
            map.setCenter(results[0].geometry.location); //set the center of map to the results
            testBounds(); //test to make sure we are indeed in the bounds (have to do this because gmaps only allows for a BIAS of bounds and is not strict)
            if (globalVar.mapInBounds == "yes") { //if it is inside bounds
                if (globalVar.searchCount > 0) { //check to see if this is the first time searched, if not
                    globalVar.searchResult.setMap(null); //set old search result to not display on map
                } else { //if it is the first time
                    globalVar.searchCount++; //just interate
                }
                globalVar.searchResult = new google.maps.Marker({ //create a new marker
                    map: map, //add to current map
                    position: results[0].geometry.location //set position to search results
                });
                var searchResult_i = 1; //temp, placeholder for later multi search result support
                document.getElementById("searchResults_list").innerHTML = writeHTML("searchResultListItem",searchResult_i, geo, "", "");
            } else { //if location found was outside strict map bounds...
                displayMessage(L24); //say so
            }

        } else { //if the entire geocode did not work
            alert(L6); //localization...
        }
    });

}

//get the nearest human reabable location from lat/long
function codeLatLng(latlng) {
    if (geocoder) {
        geocoder.geocode({ 'latLng': latlng }, function (results, status) {
            if (status == google.maps.GeocoderStatus.OK) {
                if (results[1]) {
                    document.getElementById("content_toolbox_rgItem").value = results[0].formatted_address;
                }
                else {
                    displayMessage(L25 + " " + status);
                    document.getElementById("content_toolbox_rgItem").value = "";
                }
            }
        });
    }
}

//assign search location pin to item location
function useSearchAsItemLocation() {
    //debuggin
    de("search result: " + globalVar.searchResult);
    //check to see if there is a search result
    if (globalVar.searchResult != null) {
        //this tells listeners what to do
        globalVar.placerType = "item";
        //assign new position of marker
        globalVar.itemMarker.setPosition(globalVar.searchResult.getPosition());
        //prevent redraw
        globalVar.firstMarker++;
        //delete search result
        globalVar.searchResultDeleteMe();
        //display new marker
        globalVar.itemMarker.setMap(map);
        //get the lat/long of item marker and put it in the item location tab
        document.getElementById('content_toolbox_posItem').value = globalVar.itemMarker.getPosition();
        //get the reverse geo address for item location and put in location tab
        codeLatLng(globalVar.itemMarker.getPosition());
        //store coords to save
        globalVar.savingMarkerCenter = globalVar.itemMarker.getPosition();
        //add listener for new item marker (can only add once the globalVar.itemMarker is created)
        google.maps.event.addListener(globalVar.itemMarker, 'dragend', function () {
            //get lat/long
            document.getElementById('content_toolbox_posItem').value = globalVar.itemMarker.getPosition();
            //get address
            codeLatLng(globalVar.itemMarker.getPosition());
            //store coords to save
            globalVar.savingMarkerCenter = globalVar.itemMarker.getPosition();
        });
    } else {
        //nothing in search
        displayMessage(L39);
    }
}

//used to create an overlay from a page
function createOverlayFromPage(pageId) {
    //assign convertedoverlay index
    de("previous globalVar.convertedOverlayIndex: " + globalVar.convertedOverlayIndex);
    globalVar.convertedOverlayIndex = pageId-1;
    //select the area to draw the overlay
    displayMessage(L41);
    //define drawing manager
    drawingManager.setOptions({ drawingControl: false, drawingControlOptions: { position: google.maps.ControlPosition.RIGHT_TOP, drawingModes: [google.maps.drawing.OverlayType.RECTANGLE] }, rectangleOptions: { strokeOpacity: 0.2, strokeWeight: 1, fillOpacity: 0.0 } });
    //set drawingmode to rectangle
    drawingManager.setDrawingMode(google.maps.drawing.OverlayType.RECTANGLE);
    //apply the changes
    drawingManager.setMap(map);
    //go ahead and auto switch to editing mode
    globalVar.pageMode = "edit";
    //indicate overlay placer
    globalVar.placerType = "overlay";
    //indicate that we drew this overlay
    globalVar.overlayType = "drawn";
    //assign featuretype
    globalVar.incomingPolygonFeatureType[globalVar.convertedOverlayIndex] = "main";
    //assign polygon type
    globalVar.incomingPolygonPolygonType[globalVar.convertedOverlayIndex] = "rectangle";
    //add the rotation
    globalVar.incomingPolygonRotation[globalVar.convertedOverlayIndex] = 0;
    //add the working overlay index
    globalVar.workingOverlayIndex = globalVar.convertedOverlayIndex +1;
    ////add the working overlay index
    //if (globalVar.workingOverlayIndex == null) {
    //    globalVar.workingOverlayIndex = 0;
    //} else {
    //    globalVar.workingOverlayIndex++;
    //}
}

//used to convert an incoming point to an overlay
function convertToOverlay() {
    //determine index of overlay 
    var totalPolygonCount = 0;
    var nonPoiCount = 0;
    try {
        for (var i = 0; i < globalVar.incomingPolygonFeatureType.length; i++) {
            totalPolygonCount++;
            if (globalVar.incomingPolygonFeatureType[i] != "poi") {
                nonPoiCount++;
            }
        }
        globalVar.convertedOverlayIndex = globalVar.incomingPolygonFeatureType.length - nonPoiCount;
        de("converted overlay index: " + globalVar.convertedOverlayIndex);
    } catch(e) {
        de("no overlays thus pages to convert to.");
    }
    

    //if (globalVar.itemMarker && globalVar.incomingPointSourceURL[0] != "") {
    if (nonPoiCount>0) {
        if (globalVar.itemMarker) {
            //hide marker
            globalVar.itemMarker.setMap(null);
            //delete maker todo confirm this deletes
            globalVar.itemMarker = null;
            ////open first overlay to convert
            //createOverlayFromPage(globalVar.convertedOverlayIndex + 1);
        }
        //switch to overlay tab
        actionsACL("none", "item");
        actionsACL("full", "overlay");
        //(confirm 'main' if not already there) fixes a bug 
        globalVar.incomingPolygonFeatureType[globalVar.convertedOverlayIndex] = "main";
        globalVar.incomingPolygonPolygonType[globalVar.convertedOverlayIndex] = "rectangle";
        //explicitly open overlay tab (fixes bug)
        openToolboxTab(3);
        //converted
        displayMessage(L44);
        //explicitly disallow editing after converting
        drawingManager.setDrawingMode(null);
    } else {
        //cannot convert
        displayMessage(L40);
        //explicitly disallow editing after a failed convert
        drawingManager.setDrawingMode(null);
    }
}

//used to display list of overlays in the toolbox container
function initOverlayList() {
    de("initOverlayList(); started...");
    document.getElementById("overlayList").innerHTML = "";
    if (globalVar.incomingPolygonPageId.length > 0) {
        for (var i = 0; i < globalVar.incomingPolygonLabel.length; i++) {
            if (globalVar.incomingPolygonFeatureType[i] != "poi") {
                de("Adding Overlay List Item");
                //if (globalVar.incomingPolygonLabel[i] == "") {
                //    globalVar.incomingPolygonLabel[i] = "Overlay" + (i + 1);
                //}
                //de("label: " + globalVar.incomingPolygonLabel[i] + " at " + i);
                document.getElementById("overlayList").innerHTML += writeHTML("overlayListItem", globalVar.incomingPolygonPageId[i], globalVar.incomingPolygonLabel[i], "");
            }
        }
    }

    //if (globalVar.incomingPolygonPath.length > 0) {
    //    de("There are " + globalVar.incomingPolygonLabel.length + " Incoming Polygons");
    //    for (var i = 0; i < globalVar.incomingPolygonPath.length; i++) {
    //        if (globalVar.incomingPolygonFeatureType[i] != "poi") {
    //            de("Adding Overlay List Item");
    //            //if (globalVar.incomingPolygonLabel[i] == "") {
    //            //    globalVar.incomingPolygonLabel[i] = "Overlay" + (i + 1);
    //            //}
    //            //de("label: " + globalVar.incomingPolygonLabel[i] + " at " + i);
    //            document.getElementById("overlayList").innerHTML += writeHTML("overlayListItem", globalVar.incomingPolygonPageId[i], globalVar.incomingPolygonLabel[i], "");
    //        }
    //    }
    //}
}

//used to set acess control levels for the actions
function actionsACL(level, id) {
    //doesnt work
    //document.getElementById("mapedit_container_toolbar").style.width = "1170px";
    //document.getElementById("mapedit_container_toolbar").style["margin-left"] = "-535px";
    switch (id) {
        case "actions":
            $('#content_menubar_manageOverlay').hide();
            $('#content_toolbar_button_manageOverlay').hide();
            $('#content_toolbox_button_manageOverlay').hide();
            $('#content_toolbox_tab4_header').hide();
            $('#overlayACL').hide();
            $('#content_menubar_manageItem').hide();
            $('#content_toolbar_button_manageItem').hide();
            $('#content_toolbox_button_manageItem').hide();
            $('#content_toolbox_tab3_header').hide();
            $('#itemACL').hide();
            break;
        case "item":
            switch (level) {
                case "full":
                    $('#content_menubar_manageOverlay').hide();
                    $('#content_toolbar_button_manageOverlay').hide();
                    $('#content_toolbox_button_manageOverlay').hide();
                    $('#content_toolbox_tab4_header').hide();
                    $('#overlayACL').hide();
                    break;
                case "read":
                    //nothing yet
                    break;
                case "none":
                    $('#content_menubar_manageOverlay').show();
                    $('#content_toolbar_button_manageOverlay').show();
                    $('#content_toolbox_button_manageOverlay').show();
                    $('#content_toolbox_tab4_header').show();
                    $('#overlayACL').show();
                    break;
            }
            break;
        case "overlay":
            switch (level) {
                case "full":
                    $('#content_menubar_manageItem').hide();
                    $('#content_toolbar_button_manageItem').hide();
                    $('#content_toolbox_button_manageItem').hide();
                    $('#content_toolbox_tab3_header').hide();
                    $('#itemACL').hide();
                    break;
                case "read":
                    //nothing yet
                    break;
                case "none":
                    $('#content_menubar_manageItem').show();
                    $('#content_toolbar_button_manageItem').show();
                    $('#content_toolbox_button_manageItem').show();
                    $('#content_toolbox_tab3_header').show();
                    $('#itemACL').show();
                    break;
            }
            break;
        case "customMapType":
            switch (level) {
                case "full":
                    $('#content_menubar_layerCustom').show();
                    $('#content_toolbar_button_layerCustom').show();
                    break;
                case "none":
                    $('#content_menubar_layerCustom').hide();
                    $('#content_toolbar_button_layerCustom').hide();
                    break;
            }
            break;
    }
}

//used to write html content to page via js
function writeHTML(type, param1, param2, param3) {
    de("writeHTML(); started...");
    var htmlString = "";
    switch (type) {
        case "poiListItem":
            de("Creating html String");
            //if (globalVar.incomingPointLabel[param1] == "") {
            //    globalVar.poiDesc[param1] = "New" + param3 + param2;
            //} else {
            //    globalVar.poiDesc[param1] = globalVar.incomingPointLabel[param1];
            //}
            globalVar.poiDesc[param1] = "New" + param3 + param2;
            htmlString = "<div id=\"poi" + param1 + "\" class=\"poiListItem\" title=\"" + globalVar.poiDesc[param1] + " \">" + globalVar.poiDesc[param1] + " <div class=\"poiActionButton\"><a href=\"#\" onclick=\"poiEditMe(" + param1 + ");\"><img src=\"" + globalVar.baseURL + globalVar.baseImageDirURL + "edit.png\"/></a> <a id=\"poiToggle" + param1 + "\" href=\"#\"><img src=\"" + globalVar.baseURL + globalVar.baseImageDirURL + "sub.png\" onclick=\"poiHideMe(" + param1 + ");\" /></a> <a href=\"#\" onclick=\"poiDeleteMe(" + param1 + ");\"><img src=\"" + globalVar.baseURL + globalVar.baseImageDirURL + "delete.png\"/></a></div></div>";
            break;
        case "poiListItemIncoming":
            de("Creating html String");
            globalVar.poiDesc[param1] = param3;
            htmlString = "<div id=\"poi" + param1 + "\" class=\"poiListItem\" title=\"" + param3 + " \">" + param3 + " <div class=\"poiActionButton\"><a href=\"#\" onclick=\"poiEditMe(" + param1 + ");\"><img src=\"" + globalVar.baseURL + globalVar.baseImageDirURL + "edit.png\"/></a> <a id=\"poiToggle" + param1 + "\" href=\"#\"><img src=\"" + globalVar.baseURL + globalVar.baseImageDirURL + "sub.png\" onclick=\"poiHideMe(" + param1 + ");\" /></a> <a href=\"#\" onclick=\"poiDeleteMe(" + param1 + ");\"><img src=\"" + globalVar.baseURL + globalVar.baseImageDirURL + "delete.png\"/></a></div></div>";
            break;
        case "poiDesc":
            de("Creating html String");
            htmlString = "<div class=\"poiDescContainer\"> <textarea id=\"poiDesc" + param1 + "\" class=\"descPOI\" placeholder=\"" + L3 + "\"></textarea> <br/> <div class=\"buttonPOIDesc\" id=\"poiGetDesc\" onClick=\"poiGetDesc(" + param1 + ");\">Save</div> </div>";
            break;
        case "poiDescIncoming":
            de("Creating html String");
            htmlString = "<div class=\"poiDescContainer\"> <textarea id=\"poiDesc" + param1 + "\" class=\"descPOI\">" + param2 + "</textarea> <br/> <div class=\"buttonPOIDesc\" id=\"poiGetDesc\" onClick=\"poiGetDesc(" + param1 + ");\">Save</div> </div>";
            break;
        case "overlayListItem":
            de("Creating html String");
            htmlString = "<div id=\"overlayListItem" + param1 + "\" class=\"overlayListItem\" title=\"" + param2 + "\"> " + param2.substring(0, 20) + " <div class=\"overlayActionButton\"><a href=\"#\" onclick=\"overlayEditMe(" + param1 + ");\"><img src=\"" + globalVar.baseURL + globalVar.baseImageDirURL + "edit.png\"/></a> <a id=\"overlayToggle" + param1 + "\" href=\"#\"><img src=\"" + globalVar.baseURL + globalVar.baseImageDirURL + "sub.png\" onclick=\"overlayHideMe(" + param1 + ");\" /></a> <a href=\"#\" onclick=\"overlayDeleteMe(" + param1 + ");\"><img src=\"" + globalVar.baseURL + globalVar.baseImageDirURL + "delete.png\"/></a></div></div>";
            break;
        case "searchResultListItem":
            de("Creating search html String");
            htmlString = "<div id=\"searchResultListItem" + param1 + "\" class=\"searchResultListItem\" title=\"" + param2 + "\"> " + param2.substring(0, 20) + " <div class=\"searchResultActionButton\"><a id=\"searchResultToggle" + param1 + "\" href=\"#\"><img src=\"" + globalVar.baseURL + globalVar.baseImageDirURL + "sub.png\" onclick=\"searchResultHideMe(" + param1 + ");\" /></a> <a href=\"#\" onclick=\"searchResultDeleteMe(" + param1 + ");\"><img src=\"" + globalVar.baseURL + globalVar.baseImageDirURL + "delete.png\"/></a></div></div>";
            break;
    }
    return htmlString;
}

//resizes container based on the viewport
function resizeView() {
    
    //get sizes of elements already drawn
    var totalPX = document.documentElement.clientHeight;
    var headerPX = $("#mapedit_container").offset().top;
    var widthPX = document.documentElement.clientWidth;

    //set the width of the sf menu pane0 container
    document.getElementById("mapedit_container_pane_0").style.width = widthPX + "px";

    //load all toolbar buttons into an array
    //todo make dynamic
    //todo move to onload listener
    var toolbarButtonIds = [];
    toolbarButtonIds[0] = "content_toolbar_button_reset";
    toolbarButtonIds[1] = "content_toolbar_button_toggleMapControls";
    toolbarButtonIds[2] = "content_toolbar_button_toggleToolbox";
    toolbarButtonIds[3] = "content_toolbar_button_layerRoadmap";
    toolbarButtonIds[4] = "content_toolbar_button_layerTerrain";
    toolbarButtonIds[5] = "content_toolbar_button_layerSatellite";
    toolbarButtonIds[6] = "content_toolbar_button_layerHybrid";
    toolbarButtonIds[7] = "content_toolbar_button_layerCustom";
    toolbarButtonIds[8] = "content_toolbar_button_layerReset";
    toolbarButtonIds[9] = "content_toolbar_button_panUp";
    toolbarButtonIds[10] = "content_toolbar_button_panLeft";
    toolbarButtonIds[11] = "content_toolbar_button_panReset";
    toolbarButtonIds[12] = "content_toolbar_button_panRight";
    toolbarButtonIds[13] = "content_toolbar_button_panDown";
    toolbarButtonIds[14] = "content_toolbar_button_zoomIn";
    toolbarButtonIds[15] = "content_toolbar_button_zoomReset";
    toolbarButtonIds[16] = "content_toolbar_button_zoomOut";
    toolbarButtonIds[17] = "content_toolbar_button_manageItem";
    toolbarButtonIds[18] = "content_toolbar_button_manageOverlay";
    toolbarButtonIds[19] = "content_toolbar_button_managePOI";
    toolbarButtonIds[20] = "content_toolbar_button_search";

    if (widthPX < 1100) {
        //
    }

    //if view width < toolbar width
    //todo make 1190 dynamic (cannot simple getelementbyid because toolbar is closed at start)
    if (widthPX < 1100) {
        var temp2 = toolbarButtonIds.length * 45;
        temp2 = widthPX - temp2 - 60;
        temp2 = Math.round(temp2 / 45);
        //var buttonNonVisibleCount = Math.round(((toolbarButtonIds.length * 45) - (widthPX - 60)) / 45);
        var buttonNonVisibleCount = temp2;
        de("non vis button count: " + buttonNonVisibleCount);
        var buttonVisibleCount = toolbarButtonIds.length - buttonNonVisibleCount;
        de("vis button count: " + buttonVisibleCount);
        for (var i = 0; i < buttonVisibleCount; i++) {
            de("showing: " + toolbarButtonIds[i]);
            //document.getElementById(toolbarButtonIds[i]).style.visibility = "show";
            //document.getElementById(toolbarButtonIds[i]).style.display = "block";
        }
        for (var i = buttonVisibleCount; i < buttonNonVisibleCount; i++) {
            de("hiding: " + toolbarButtonIds[i]);
            //document.getElementById(toolbarButtonIds[i]).style.visibility = "hidden";
            //document.getElementById(toolbarButtonIds[i]).style.display = "none";
        }
    }
    //calculate how many buttons can be placed based on width
    //display said buttons with arrow to cycle through

    //detect and handle different widths
    //todo make the 800,250 dynamic
    if (widthPX <= 800) {
        de("tablet viewing width detected...");
        //toolbar
        //menubar
        //toolbox -min
    }
    if (widthPX <= 250) {
        de("phone viewing width detected...");
        //toolbar -convert to bottom button style
        //menubar -convert to sidemenu
        //toolbox -close/disable
    }

    //calculate and set sizes
    var bodyPX = totalPX - headerPX;
    document.getElementById("mapedit_container").style.height = bodyPX + "px";
    var pane0PX = bodyPX * .05;
    //document.getElementById("mapedit_container_pane_0").style.height = pane0PX + "px";
    var pane1PX = bodyPX * .05;
    //document.getElementById("mapedit_container_pane_1").style.height = pane1PX + "px";
    var pane2PX = bodyPX * .90;
    //document.getElementById("mapedit_container_pane_2").style.height = pane2PX + "px";

    //calculate percentage of height
    var percentOfHeight = Math.round((bodyPX / totalPX) * 100);
    //document.getElementById("mapedit_container").style.height = percentOfHeight + "%";
    de("percentage of height: " + percentOfHeight);

    de("sizes:<br>height: " + totalPX + " header: " + headerPX + " body: " + bodyPX + " pane0: " + pane0PX + " pane1: " + pane1PX + " pane2: " + pane2PX);
}

//clear the save overlay cache
function clearCacheSaveOverlay() {
    de("attempting to clear save overlay cache");
    if (globalVar.savingOverlayIndex.length > 0) {
        de("reseting cache data");
        globalVar.savingOverlayIndex = [];
        globalVar.savingOverlayPageId = [];
        globalVar.savingOverlayLabel = [];
        globalVar.savingOverlaySourceURL = [];
        globalVar.savingOverlayBounds = [];
        globalVar.savingOverlayRotation = [];
        de("reseting cache save overlay index");
        globalVar.csoi = 0;
        globalVar.userMayLoseData = false;
        de("cache reset");
    } else {
        de("nothing in cache");
    }
    de("reseting working index");
    globalVar.workingOverlayIndex = null;
    de("reseting preserved rotation");
    globalVar.preservedRotation = 0;
}

//keypress shortcuts/actions
//window.onkeypress = keypress;
//function keypress(e) {
window.onkeyup = keyup;
var isCntrlDown = false; //used for debug currently
function keyup(e) {
    var keycode = null;
    if (window.event) {
        keycode = window.event.keyCode;
    } else if (e) {
        keycode = e.which;
    }
    de("key pressed: " + keycode);
    switch (keycode) {
        case 13: //enter
            if (document.getElementById("content_toolbox_searchField").value != null) {
                var stuff = document.getElementById("content_toolbox_searchField").value;
                finder(stuff);
            }
            if (document.getElementById("content_toolbar_searchField").value != null) {
                var stuff = document.getElementById("content_toolbar_searchField").value;
                finder(stuff);
            }

            break;
        case 17: //ctrl
            if (isCntrlDown == false) {
                isCntrlDown = true;
            } else {
                isCntrlDown = false;
            }
            de("CntrlDown: " + isCntrlDown);
            break;

        case 70: //F
            if (isCntrlDown == true) {
                if (navigator.appName == "Microsoft Internet Explorer") {
                    var copyString = cLat.innerHTML;
                    copyString += ", " + cLong.innerHTML;
                    window.clipboardData.setData("Text", copyString);
                    displayMessage(L19);
                } else {
                    if (globalVar.cCoordsFrozen == "no") {
                        //freeze
                        globalVar.cCoordsFrozen = "yes";
                        displayMessage(L20);
                    } else {
                        //unfreeze
                        globalVar.cCoordsFrozen = "no";
                        displayMessage(L21);
                    }
                }
            }
            break;
        case 79: //O
            if (isCntrlDown == true) {
                if (globalVar.overlaysCurrentlyDisplayed == true) {
                    displayMessage(L22);
                    for (var i = 0; i < globalVar.incomingPolygonPageId.length; i++) { //go through and display overlays as long as there is an overlay to display
                        globalVar.overlaysOnMap[globalVar.incomingPolygonPageId[i]].setMap(null); //hide the overlay from the map
                        globalVar.ghostOverlayRectangle[globalVar.incomingPolygonPageId[i]].setMap(null); //hide ghost from map
                        globalVar.overlaysCurrentlyDisplayed = false; //mark that overlays are not on the map
                    }
                } else {
                    displayMessage(L23);
                    for (var i = 0; i < globalVar.incomingPolygonPageId.length; i++) { //go through and display overlays as long as there is an overlay to display
                        globalVar.overlaysOnMap[globalVar.incomingPolygonPageId[i]].setMap(map); //set the overlay to the map
                        globalVar.ghostOverlayRectangle[globalVar.incomingPolygonPageId[i]].setMap(map); //set to map
                        globalVar.overlaysCurrentlyDisplayed = true; //mark that overlays are on the map
                    }
                }
            }
            break;
        case 68: //D (for debuggin)
            if (isCntrlDown == true) {
                debugs++;
                if (debugs % 2 == 0) {
                    document.getElementById("debugs").style.display = "none";
                    globalVar.debugMode = false;
                    isCntrlDown = false;
                    displayMessage("Debug Mode Off");
                } else {
                    document.getElementById("debugs").style.display = "block";
                    globalVar.debugMode = true;
                    displayMessage("Debug Mode On");
                    isCntrlDown = false;
                }
            }
            break;
    }
}

//debugging 
var debugStringBase = "<strong>Debug Panel:</strong> <a onclick=\"debugClear()\">(clear)</a><br><br>"; //starting debug string
var debugString; //holds debug messages
var debugs = 0; //used for keycode debugging
function de(message) {
    //create debug string
    var currentdate = new Date();
    var time = currentdate.getHours() + ":" + currentdate.getMinutes() + ":" + currentdate.getSeconds() + ":" + currentdate.getMilliseconds();
    var newDebugString = "[" + time + "] " + message + "<br><hr>";
    newDebugString += debugString;
    document.getElementById("debugs").innerHTML = debugStringBase + newDebugString;
    debugString = newDebugString;
}
function debugClear() {
    debugString = ""; //clear debug string
    document.getElementById("debugs").innerHTML = debugStringBase;
}

//#endregion


//other 

//#region jquery UI elements

//jquery UI elements
$(function () {
    try {
        $('ul.sf-menu').superfish();

        //draggable content settings
        $("#mapedit_container_toolbox").draggable({
            handle: "#mapedit_container_toolboxMinibar"//, //div used as handle
            //containment: "#mapedit_container" //bind to map container 
        });
        //accordian settings
        $("#mapedit_container_toolboxTabs").accordion({
            animate: true, //turn off all animations (this got rid of search icon problem) //if ever set, check WORKAROUND in openToolboxTabs(id)
            active: 0, //which tab is active
            icons: false, //default icons?
            //collapsible: true, //does not work
            heightStyle: "content" //set hieght to?
        });
        //tooltips (the tooltip text is the title of the element defined in localization js)
        $("#content_toolbarGrabber").tooltip({ track: true });
        $("#content_toolbar_button_reset").tooltip({ track: true });
        $("#content_toolbar_button_toggleMapControls").tooltip({ track: true });
        $("#content_toolbar_button_toggleToolbox").tooltip({ track: true });
        $("#content_toolbar_button_layerRoadmap").tooltip({ track: true });
        $("#content_toolbar_button_layerSatellite").tooltip({ track: true });
        $("#content_toolbar_button_layerTerrain").tooltip({ track: true });
        $("#content_toolbar_button_layerControls").tooltip({ track: true });
        $("#content_toolbar_button_layerHybrid").tooltip({ track: true });
        $("#content_toolbar_button_layerCustom").tooltip({ track: true });
        $("#content_toolbar_button_layerReset").tooltip({ track: true });
        $("#content_toolbar_button_zoomIn").tooltip({ track: true });
        $("#content_toolbar_button_zoomReset").tooltip({ track: true });
        $("#content_toolbar_button_zoomOut").tooltip({ track: true });
        $("#content_toolbar_button_panUp").tooltip({ track: true });
        $("#content_toolbar_button_panDown").tooltip({ track: true });
        $("#content_toolbar_button_panReset").tooltip({ track: true });
        $("#content_toolbar_button_panLeft").tooltip({ track: true });
        $("#content_toolbar_button_panRight").tooltip({ track: true });
        $("#content_toolbar_button_manageItem").tooltip({ track: true });
        $("#content_toolbar_button_manageOverlay").tooltip({ track: true });
        $("#content_toolbar_button_managePOI").tooltip({ track: true });
        $("#content_toolbar_button_manageSearch").tooltip({ track: true });
        $("#content_toolbox_button_reset").tooltip({ track: true });
        $("#content_toolbox_button_toggleMapControls").tooltip({ track: true });
        $("#content_toolbox_button_layerRoadmap").tooltip({ track: true });
        $("#content_toolbox_button_layerSatellite").tooltip({ track: true });
        $("#content_toolbox_button_layerTerrain").tooltip({ track: true });
        $("#content_toolbox_button_layerControls").tooltip({ track: true });
        $("#content_toolbox_button_layerHybrid").tooltip({ track: true });
        $("#content_toolbox_button_layerCustom").tooltip({ track: true });
        $("#content_toolbox_button_layerReset").tooltip({ track: true });
        $("#content_toolbox_button_zoomIn").tooltip({ track: true });
        $("#content_toolbox_button_zoomReset").tooltip({ track: true });
        $("#content_toolbox_button_zoomOut").tooltip({ track: true });
        $("#content_toolbox_button_panUp").tooltip({ track: true });
        $("#content_toolbox_button_panDown").tooltip({ track: true });
        $("#content_toolbox_button_panReset").tooltip({ track: true });
        $("#content_toolbox_button_panLeft").tooltip({ track: true });
        $("#content_toolbox_button_panRight").tooltip({ track: true });
        $("#content_toolbox_button_manageItem").tooltip({ track: true });
        $("#content_toolbox_button_manageOverlay").tooltip({ track: true });
        $("#content_toolbox_button_managePOI").tooltip({ track: true });
        $("#content_toolbox_button_itemPlace").tooltip({ track: true });
        //$("#content_toolbox_button_overlayPlace").tooltip({ track: true });
        //$("#content_toolbox_button_placePOI").tooltip({ track: true });
        $("#content_toolbox_button_poiMarker").tooltip({ track: true });
        $("#content_toolbox_button_poiCircle").tooltip({ track: true });
        $("#content_toolbox_button_poirectangle").tooltip({ track: true });
        $("#content_toolbox_button_poiPolygon").tooltip({ track: true });
        $("#content_toolbox_button_poiLine").tooltip({ track: true });
        $("#rotationKnob").tooltip({ track: true });
        $("#content_toolbox_rotationClockwise").tooltip({ track: true });
        $("#content_toolbox_rotationReset").tooltip({ track: true });
        $("#content_toolbox_rotationCounterClockwise").tooltip({ track: true });
        $("#transparency").tooltip({ track: true });
        $("#content_toolbox_rgItem").tooltip({ track: true });
        $("#content_toolbox_posItem").tooltip({ track: true });
        $("#content_toolbox_button_itemPlace").tooltip({ track: true });
        $("#descItem").tooltip({ track: true });
        $("#content_toolbox_button_saveItem").tooltip({ track: true });
        //$("#content_toolbox_button_overlayPlace").tooltip({ track: true });
        $("#content_toolbox_button_saveOverlay").tooltip({ track: true });
        //$("#content_toolbox_button_placePOI").tooltip({ track: true });
        $("#descPOI").tooltip({ track: true });
        $("#content_toolbox_button_savePOI").tooltip({ track: true });
        $("#content_toolbox_button_itemGetUserLocation").tooltip({ track: true });
        $("#content_toolbox_button_overlayGetUserLocation").tooltip({ track: true });
        //$("#content_toolbox_button_overlayEdit").tooltip({ track: true });
        $("#content_toolbox_button_overlayToggle").tooltip({ track: true });
        $("#content_toolbox_button_useSearchAsLocation").tooltip({ track: true });
        $("#content_toolbox_button_convertToOverlay").tooltip({ track: true });
        $("#content_toolbox_button_poiGetUserLocation").tooltip({ track: true });
        $("#content_toolbox_button_poiToggle").tooltip({ track: true });
        $("#content_toolbox_button_clearItem").tooltip({ track: true });
        $("#content_toolbox_button_clearOverlay").tooltip({ track: true });
        $("#content_toolbox_button_clearPOI").tooltip({ track: true });
        $("#content_toolbar_searchField").tooltip({ track: true });
        $("#content_toolbar_searchButton").tooltip({ track: true });
        $("#content_toolbox_searchField").tooltip({ track: true });
        $("#content_toolbox_searchButton").tooltip({ track: true });
        $("#searchResults_container").tooltip({ track: true });
        //$(".selector").tooltip({ content: "Awesome title!" });
    } catch (err) {
        alert(L51 + ": " + err);
    }
});

//#endregion


//start whole mess here (doesnt work yet because of inline calls)
function initAll() {
    initDeclarations();
    initLocalization();
    initListeners();
}

//initAll(); //TEMP ENABLE WHEN READY