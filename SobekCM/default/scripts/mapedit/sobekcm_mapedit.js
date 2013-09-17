/*
This file loads all of the custom javascript libraries needed to run the mapedit portion of sobek.
*/


//Not Used (old load)

//#region old load fcns

//$(document).ready(function () {
//    // Add the page method call as an onclick handler for the div.
//    var scriptURL = "default/scripts/serverside/Scripts.aspx";
//    de("Debug ID: 1");
//    $("#Result").click(function () {
//        $.ajax({
//            type: "POST",
//            url: baseURL+scriptURL+"/GetDate",
//            data: "{}",
//            contentType: "application/json; charset=utf-8",
//            dataType: "json",
//            success: function (msg) {
//                // Replace the div's content with the page method's return.
//                $("#Result").text(msg.d);
//            }
//        });
//    });
//});

//#endregion


//Declarations

//#region Declarations

//must remain outside fcns at top level
var globalVars; //holds global vars
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
    globalVars = function () {
        //private

        //public
        return {
            //public vars

            //init global vars
            //global defines (do not change here)
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
            incomingPointCenter: [],                    //defined in c# to js on page
            incomingPointLabel: [],                     //defined in c# to js on page
            incomingPointSourceURL: [],                 //defined in c# to js on page
            incomingOverlayBounds: [],                  //defined in c# to js on page
            incomingOverlayLabel: [],                   //defined in c# to js on page
            incomingOverlaySourceURL: [],               //defined in c# to js on page
            incomingOverlayRotation: [],                //defined in c# to js on page
            ghostOverlayRectangle: [],                  //holds ghost overlay rectangles (IE overlay hotspots)
            workingOverlayIndex: null,                  //holds the index of the overlay we are working with (and saving)
            currentlyEditing: "no",                     //tells us if we are editing anything
            currentTopZIndex: 5,                        //current top zindex (used in displaying overlays over overlays)
            savingOverlayIndex: [],                     //holds index of the overlay we are saving
            savingOverlayLabel: [],                     //holds label of the overlay we are saving
            savingOverlaySourceURL: [],                 //hold the source url of the overlay to save
            savingOverlayBounds: [],                    //holds bounds of the overlay we are saving
            savingOverlayRotation: [],                  //holds rotation of the overlay we are saving
            strictBounds: null,                         //holds the strict bounds
            ghosting: {                                 //define options for globalVars.ghosting (IE being invisible)
                strokeOpacity: 0.0,                     //make border invisible
                fillOpacity: 0.0,                       //make fill transparent
                editable: false,                        //sobek standard
                draggable: false,                       //sobek standard
                zindex: 5                               //perhaps higher?
            },
            editable: {                                 //define options for visible and globalVars.editable
                editable: true,                         //sobek standard
                draggable: true,                        //sobek standard
                strokeOpacity: 0.2,                     //sobek standard
                strokeWeight: 1,                        //sobek standard
                fillOpacity: 0.0,                       //sobek standard 
                zindex: 5                               //sobek standard
            }//,
            ////public methods
            //addListeners: function () {
            //    //alert(globalVars.test1);
            //},
            //add: function () {
            //    //globalVars.test2++;
            //    //alert(globalVars.test2);
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
var L_NotSaved = "Nothing To Save";
var L_NotCleared = "Nothing to Reset";
var L_Save = "Save";
var L_Apply = "Apply";
var L_Editing = "Editing";
var L_Removed = "Removed";
var L_Showing = "Showing";
var L_Hiding = "Hiding";
var L1 = "SobekCM Plugin <a href=\"#\">Report a Sobek Error</a>"; //copyright node
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
                    document.getElementById("content_toolbox_button_placeItem").title = "Edit Location";
                    document.getElementById("content_toolbox_button_itemGetUserLocation").title = "Center On Your Current Position";
                    document.getElementById("content_toolbox_button_useSearchAsLocation").title = "Use Search Result As Location";
                    document.getElementById("content_toolbox_button_convertToOverlay").title = "Convert This To a Map Overlay";
                    document.getElementById("content_toolbox_posItem").title = "Coordinates: This is the selected Latitude and Longitude of the point you selected.";
                    document.getElementById("content_toolbox_rgItem").title = "Address: This is the nearest address of the point you selected.";
                    document.getElementById("content_toolbox_button_saveItem").title = "Save Location Changes";
                    document.getElementById("content_toolbox_button_clearItem").title = "Reset Location Changes";
                    //tab
                    document.getElementById("content_toolbox_button_overlayEdit").title = "Toggle Overlay Editing";
                    document.getElementById("content_toolbox_button_overlayPlace").title = "Place A New Overlay";
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
                    document.getElementById("content_toolbox_button_placePOI").title = "Toggle Point Of Interest Editing";
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
                    document.getElementById("content_menubar_overlayEdit").innerHTML = "Toggle Overlay Editing";
                    document.getElementById("content_menubar_overlayPlace").innerHTML = "Place A New Overlay";
                    document.getElementById("content_menubar_overlayToggle").innerHTML = "Toggle All Map Overlays";
                    document.getElementById("content_menubar_rotationClockwise").innerHTML = "Clockwise";
                    document.getElementById("content_menubar_rotationCounterClockwise").innerHTML = "Counter-Clockwise";
                    document.getElementById("content_menubar_rotationReset").innerHTML = "Reset";
                    document.getElementById("content_menubar_transparencyDarker").innerHTML = "Darker";
                    document.getElementById("content_menubar_transparencyLighter").innerHTML = "Lighter";
                    document.getElementById("content_menubar_transparencyReset").innerHTML = "Reset";
                    document.getElementById("content_menubar_overlayReset").innerHTML = "Reset Overlays";
                    document.getElementById("content_menubar_poiGetUserLocation").innerHTML = "Center On Current Location";
                    document.getElementById("content_menubar_poiPlace").innerHTML = "Toggle POI Editing";
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
            //attempt to save all three
            save("item");
            save("overlay");
            save("poi");
        }, false);
        document.getElementById("content_menubar_cancel").addEventListener("click", function () {
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
            openToolboxTab("item");
            convertToOverlay();
            place("overlay");
        }, false);
        document.getElementById("content_menubar_itemReset").addEventListener("click", function () {
            openToolboxTab("item");
            clear("item");
        }, false);
        document.getElementById("content_menubar_manageOverlay").addEventListener("click", function () {
            action("manageOverlay");
        }, false);
        document.getElementById("content_menubar_overlayPlace").addEventListener("click", function () {
            openToolboxTab("overlay");
            place("overlay");
        }, false);
        document.getElementById("content_menubar_overlayEdit").addEventListener("click", function () {
            openToolboxTab("overlay");
            place("overlay");
        }, false);
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
        document.getElementById("content_menubar_poiPlace").addEventListener("click", function () {
            openToolboxTab("poi");
            place("poi");
        }, false);
        document.getElementById("content_menubar_poiGetUserLocation").addEventListener("click", function () {
            openToolboxTab("poi");
            geolocate("poi");
        }, false);
        document.getElementById("content_menubar_poiToggle").addEventListener("click", function () {
            openToolboxTab("poi");
            toggleVis("pois");
        }, false);
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
        document.getElementById("content_toolbox_button_placeItem").addEventListener("click", function () {
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
            //now trigger place overlay
            place("overlay");
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
        document.getElementById("content_toolbox_button_overlayPlace").addEventListener("click", function () {
            place("overlay");
        }, false);
        document.getElementById("content_toolbox_button_overlayEdit").addEventListener("click", function () {
            place("overlay");
        }, false);
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
        document.getElementById("content_toolbox_button_placePOI").addEventListener("click", function () {
            place("poi");
        }, false);
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

    //if (globalVars.userMayLoseData) {
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
            if (globalVars.mapControlsDisplayed == true) { //not present
                map.setOptions({
                    zoomControl: false,
                    panControl: false,
                    mapTypeControl: false
                });
                globalVars.mapControlsDisplayed = false;
            } else { //present
                map.setOptions({
                    zoomControl: true,
                    zoomControlOptions: { style: google.maps.ZoomControlStyle.SMALL, position: google.maps.ControlPosition.LEFT_TOP },
                    panControl: true,
                    panControlOptions: { position: google.maps.ControlPosition.LEFT_TOP },
                    mapTypeControl: true,
                    mapTypeControlOptions: { style: google.maps.MapTypeControlStyle.DROPDOWN_MENU, position: google.maps.ControlPosition.RIGHT_TOP }
                });
                globalVars.mapControlsDisplayed = true;
            }
            buttonActive("mapControls"); //set the is active glow for button
            break;

        case "toolbox":
            if (globalVars.toolboxDisplayed == true) {
                document.getElementById("mapedit_container_toolbox").style.display = "none";
                document.getElementById("mapedit_container_toolboxTabs").style.display = "none";
                //$("#mapedit_container_toolbox").effect("slide", 500);
                globalVars.toolboxDisplayed = false;
            } else {
                document.getElementById("mapedit_container_toolbox").style.display = "block";
                document.getElementById("mapedit_container_toolboxTabs").style.display = "block";
                document.getElementById("mapedit_container_toolbox").style.height = "auto";
                globalVars.toolboxDisplayed = true;
            }
            buttonActive("toolbox"); //set the is active glow for button
            break;

        case "toolbar":
            if (globalVars.toolbarDisplayed == true) {
                $("#mapedit_container_pane_1").hide();
                document.getElementById("mapedit_container_toolbarGrabber").style.marginTop = "0";
                globalVars.toolbarDisplayed = false;
            } else {
                $("#mapedit_container_pane_1").show();
                document.getElementById("mapedit_container_toolbarGrabber").style.marginTop = "48px";
                globalVars.toolbarDisplayed = true;
            }
            buttonActive("toolbar"); //set the is active glow for button
            break;

        case "kml":
            if (globalVars.kmlDisplayed == true) {
                KmlLayer.setMap(null);
                globalVars.kmlDisplayed = false;
            } else {
                KmlLayer.setMap(map);
                globalVars.kmlDisplayed = true;
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
            if (globalVars.mapDrawingManagerDisplayed == true) {
                drawingManager.setMap(null);
                globalVars.mapDrawingManagerDisplayed = false;
            } else {
                drawingManager.setMap(map);
                globalVars.mapDrawingManagerDisplayed = true;
            }
            //buttonActive("mapDrawingManager"); 
            break;

        case "overlays":
            if (globalVars.overlaysOnMap.length) {
                if (globalVars.overlaysCurrentlyDisplayed == true) {
                    displayMessage(L22);
                    
                    for (var i = 0; i < globalVars.incomingOverlayBounds.length; i++) { //go through and display overlays as long as there is an overlay to display
                        de("overlay count " + globalVars.overlayCount);
                        globalVars.RIBMode = true;
                        if (document.getElementById("overlayToggle" + i)) {
                            de("found: overlayToggle" + i);
                            overlayHideMe(i);
                        } else {
                            de("did not find: overlayToggle" + i);
                        }
                        
                        globalVars.RIBMode = false;
                        //globalVars.overlaysOnMap[i].setMap(null); //hide the overlay from the map
                        //globalVars.ghostOverlayRectangle[i].setMap(null); //hide ghost from map
                        globalVars.overlaysCurrentlyDisplayed = false; //mark that overlays are not on the map
                    }
                } else {
                    displayMessage(L23);
                    for (var i = 0; i < globalVars.incomingOverlayBounds.length; i++) { //go through and display overlays as long as there is an overlay to display
                        de("oom " + globalVars.overlaysOnMap.length);
                        globalVars.RIBMode = true;
                        if (document.getElementById("overlayToggle" + i)) {
                            de("found: overlayToggle" + i);
                            overlayShowMe(i);
                        } else {
                            de("did not find: overlayToggle" + i);
                        }
                        globalVars.RIBMode = false;
                        //globalVars.overlaysOnMap[i].setMap(map); //set the overlay to the map
                        //globalVars.ghostOverlayRectangle[i].setMap(map); //set to map
                        globalVars.overlaysCurrentlyDisplayed = true; //mark that overlays are on the map
                    }
                }
            } else {
                //nothing to toggle
                displayMessage(L45);
            }
            break;

        case "pois":
            if (globalVars.poiCount) {
                for (var i = 0; i < globalVars.poiCount; i++) {
                    if (globalVars.poiToggleState == "displayed") {
                        poiHideMe(i);
                        globalVars.poiToggleState = "hidden";
                        displayMessage(L42);
                    } else {
                        poiShowMe(i);
                        globalVars.poiToggleState = "displayed";
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
            globalVars.mapLayerActive = "Roadmap";
            break;
        case "terrain":
            map.setMapTypeId(google.maps.MapTypeId.TERRAIN);
            globalVars.mapLayerActive = "Terrain";
            break;
        case "satellite":
            map.setMapTypeId(google.maps.MapTypeId.SATELLITE);
            globalVars.mapLayerActive = "Satellite";
            break;
        case "hybrid":
            map.setMapTypeId(google.maps.MapTypeId.HYBRID);
            globalVars.mapLayerActive = "Hybrid";
            break;
        case "custom":
            toggleVis("kml");
            break;
        case "reset":
            map.setMapTypeId(google.maps.MapTypeId.ROADMAP);
            //2do make this set to default
            globalVars.mapLayerActive = "Roadmap";
            if (globalVars.kmlDisplayed == true) {
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
            map.panTo(globalVars.mapCenter);
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
            map.setZoom(globalVars.defaultZoomLevel);
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
            globalVars.userMayLoseData = true;
            globalVars.actionActive = "Item";  //note case (uppercase is tied to the actual div)
            buttonActive("action");
            if (globalVars.toolboxDisplayed != true) {
                toggleVis("toolbox");
            }
            openToolboxTab(2);
            //force a suppression dm
            if (globalVars.mapDrawingManagerDisplayed == true) {
                globalVars.mapDrawingManagerDisplayed = true;
                toggleVis("mapDrawingManager");
            }
            globalVars.placerType = "item";
            //save 
            globalVars.RIBMode = true;
            save("overlay");
            save("poi");
            globalVars.RIBMode = false;
            place("item");

            break;

        case "manageOverlay":
            globalVars.userMayLoseData = true;
            globalVars.actionActive = "Overlay"; //notice case (uppercase is tied to the actual div)
            buttonActive("action");
            if (globalVars.toolboxDisplayed != true) {
                toggleVis("toolbox");
            }
            openToolboxTab(3);
            //force a suppression dm
            if (globalVars.mapDrawingManagerDisplayed == true) {
                globalVars.mapDrawingManagerDisplayed = true;
                toggleVis("mapDrawingManager");
            }

            //save
            globalVars.RIBMode = true;
            save("item");
            save("poi");
            globalVars.RIBMode = false;

            //place
            globalVars.placerType = "overlay";
            place("overlay");

            break;

        case "managePOI":
            globalVars.userMayLoseData = true;
            globalVars.actionActive = "POI"; //notice case (uppercase is tied to the actual div)
            buttonActive("action");
            if (globalVars.toolboxDisplayed != true) {
                toggleVis("toolbox");
            }
            openToolboxTab(4);
            toggleVis("mapDrawingManager");

            //save
            globalVars.RIBMode = true;
            save("item");
            save("overlay");
            globalVars.RIBMode = false;

            //place
            globalVars.placerType = "poi";
            place("poi");
            break;

        case "other":
            de("action Other started...");
            globalVars.actionActive = "Other";
            buttonActive("action");
            //openToolboxTab(); //not called here, called in listerner
            //force a suppression dm
            if (globalVars.mapDrawingManagerDisplayed == true) {
                globalVars.mapDrawingManagerDisplayed = true;
                toggleVis("mapDrawingManager");
            }

            //save
            globalVars.RIBMode = true;
            save("item");
            save("overlay");
            save("poi");
            globalVars.RIBMode = false;

            globalVars.placerType = "none";

            de("action Other ended...");

            break;

        case "search":
            de("action search started...");
            globalVars.actionActive = "Search";
            buttonActive("action");
            globalVars.placerType = "none";

            //force a suppression dm (unknown if we need to?)
            if (globalVars.mapDrawingManagerDisplayed == true) {
                globalVars.mapDrawingManagerDisplayed = true;
                toggleVis("mapDrawingManager");
            }

            //open search tab
            de("globalVars.toolboxDisplayed: " + globalVars.toolboxDisplayed);
            if (globalVars.toolboxDisplayed == true) {
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
            globalVars.placerType = "item";
            if (globalVars.itemMarker != null) {
                displayMessage(L30);
            } else {
                if (globalVars.searchCount > 0 && globalVars.itemMarker == null) {
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
            globalVars.placerType = "overlay";
            if (globalVars.incomingOverlayBounds.length > 0) {
                if (globalVars.pageMode == "edit") {
                    globalVars.pageMode = "view";
                    if (globalVars.savingOverlayIndex.length > 0) {
                        for (var i = 0; i < globalVars.savingOverlayIndex.length; i++) {
                            globalVars.ghostOverlayRectangle[globalVars.savingOverlayIndex[i]].setOptions(globalVars.ghosting); //set globalVars.rectangle to globalVars.ghosting    
                        }
                    }
                    displayMessage(L26);
                } else {
                    globalVars.pageMode = "edit";
                    displayMessage(L27);
                }
                //toggleOverlayEditor(); 
            } else {
                //select the area to draw the overlay
                displayMessage(L41);

                //define drawing manager
                drawingManager.setOptions({ drawingControl: false, drawingControlOptions: { position: google.maps.ControlPosition.RIGHT_TOP, drawingModes: [google.maps.drawing.OverlayType.RECTANGLE] }, rectangleOptions: { strokeOpacity: 0.2, strokeWeight: 1, fillOpacity: 0.0 } });

                //set drawingmode to rectangle
                drawingManager.setDrawingMode(google.maps.drawing.OverlayType.RECTANGLE);

                //apply the changes
                drawingManager.setMap(map);

                //go ahead and auto switch to editing mode
                globalVars.pageMode = "edit";
            }

            break;

        case "poi":
            drawingManager.setOptions({ drawingControl: true, drawingControlOptions: { position: google.maps.ControlPosition.RIGHT_TOP, drawingModes: [google.maps.drawing.OverlayType.MARKER, google.maps.drawing.OverlayType.CIRCLE, google.maps.drawing.OverlayType.RECTANGLE, google.maps.drawing.OverlayType.POLYGON, google.maps.drawing.OverlayType.POLYLINE] } });
            globalVars.placerType = "poi";
            break;
    }
}

//poi object placer handler
function placePOI(type) {
    globalVars.placerType = "poi";
    switch (type) {
        case "marker":
            drawingManager.setDrawingMode(google.maps.drawing.OverlayType.MARKER);
            break;
        case "circle":
            drawingManager.setDrawingMode(google.maps.drawing.OverlayType.CIRCLE);
            break;
        case "rectangle":
            drawingManager.setDrawingMode(google.maps.drawing.OverlayType.RECTANGLE);
            break;
        case "polygon":
            drawingManager.setDrawingMode(google.maps.drawing.OverlayType.POLYGON);
            break;
        case "line":
            drawingManager.setDrawingMode(google.maps.drawing.OverlayType.POLYLINE);
            break;
    }
}

//geolocation handler
function geolocate(id) {
    displayMessage(L50);
    switch (id) {
        case "item":
            drawingManager.setDrawingMode(google.maps.drawing.OverlayType.MARKER);
            globalVars.placerType = "item";
            // Try W3C Geolocation
            if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(function (position) {
                    var userLocation = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);
                    map.setCenter(userLocation);
                    testBounds();
                    globalVars.markerCenter = userLocation;
                    globalVars.itemMarker = new google.maps.Marker({
                        position: globalVars.markerCenter,
                        map: map
                    });
                    globalVars.itemMarker.setMap(map);
                    document.getElementById('content_toolbox_posItem').value = globalVars.markerCenter;
                    globalVars.savingMarkerCenter = globalVars.itemMarker.getPosition(); //store coords to save
                });

            } else {
                alert(L4);
            }
            drawingManager.setDrawingMode(null);
            break;

        case "overlay":
            globalVars.placerType = "overlay";
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
            globalVars.placerType = "poi";
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
            if (globalVars.savingMarkerCenter != null) {
                //is this the first time saving a changed item?
                if (globalVars.firstSaveItem == true) {
                    de("saving location: " + globalVars.savingMarkerCenter);
                    //save to temp xml file
                    createSavedItem(globalVars.savingMarkerCenter);
                    //reset first save
                    globalVars.firstSaveItem = false;
                    //change save button to apply button
                    document.getElementById("content_toolbox_button_saveItem").value = L36;
                    //change save title to apply
                    document.getElementById("content_toolbox_button_saveItem").title = L35;
                } else {
                    //apply the changes
                    de("Applying Changes...");
                    //currently doesnt do anything

                    //reset apply button to save
                    document.getElementById("content_toolbox_button_saveItem").value = L37;
                    document.getElementById("content_toolbox_button_saveitem").title = L38;
                }
            } else {
                displayMessage(L_NotSaved);
            }

            break;

        case "overlay":
            //is this the first time saving a changed item?
            de("first save overlay? " + globalVars.firstSaveOverlay);
            if (globalVars.firstSaveOverlay == true) {
                //determine if there is something to save
                de("overlay length? " + globalVars.savingOverlayIndex.length);
                if (globalVars.savingOverlayIndex.length) {
                    for (var i = 0; i < globalVars.savingOverlayIndex.length; i++) {
                        //save to temp xml file
                        de("saving overlay: " + globalVars.savingOverlayLabel[i] + "\nsource: " + globalVars.savingOverlaySourceURL[i] + "\nbounds: " + globalVars.savingOverlayBounds[i] + "\nrotation: " + globalVars.savingOverlayRotation[i]);
                        createSavedOverlay(globalVars.savingOverlayLabel[i], globalVars.savingOverlaySourceURL[i], globalVars.savingOverlayBounds[i], globalVars.savingOverlayRotation[i]); //send overlay to the server
                    }
                    //reset first save
                    globalVars.firstSaveOverlay = false;
                    //change save button to apply button
                    document.getElementById("content_toolbox_button_saveOverlay").value = L36;
                    //change save title to apply
                    document.getElementById("content_toolbox_button_saveOverlay").title = L35;
                } else {
                    //tell that we did not save anything
                    displayMessage(L_NotSaved);
                }
            } else {

                //is there something to save?
                if (globalVars.savingOverlayIndex.length) {
                    //apply the changes
                    de("Applying Changes...");
                    //currently doesnt do anything
                } else {
                    displayMessage(L_NotSaved);
                }

                //reset apply button to save
                document.getElementById("content_toolbox_button_saveOverlay").value = L37;
                document.getElementById("content_toolbox_button_saveOverlay").title = L38;
            }
            break;

        case "poi":

            //save

            //save to temp xml file
            if (globalVars.poiObj.length > 0) {
                de("saving " + globalVars.poiObj.length + " POIs...");
                createSavedPOI();
                //displayMessage(L_Saved); //not used here
            } else {
                displayMessage(L_NotSaved);
            }

            //apply

            //is this the first time saving a changed item? (apply changes)
            if (globalVars.firstSavePOI == true) {
                //determine if there is something to save
                if (globalVars.poiObj.length > 0) {
                    //save to temp xml file
                    de("saving " + globalVars.poiObj.length + " POIs...");
                    createSavedPOI();
                    //reset first save
                    globalVars.firstSavePOI = false;
                    //change save button to apply button
                    document.getElementById("content_toolbox_button_savePOI").value = L36;
                    //change save title to apply
                    document.getElementById("content_toolbox_button_savePOI").title = L35;
                } else {
                    //tell that we did not save anything
                    displayMessage(L_NotSaved);
                }
            } else {
                if (globalVars.firstSavePOI == false) {
                    //is there something to save?
                    if (globalVars.poiObj.length > 0) {
                        //apply the changes
                        de("Applying Changes...");
                        //currently doesnt do anything
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
            if (globalVars.savingMarkerCenter != null) {
                //clear the current marker
                globalVars.itemMarker.setMap(null); //delete marker form map
                globalVars.itemMarker = null; //delete reference to marker
                globalVars.savingMarkerCenter = null; //reset stored coords to save
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
            if ((globalVars.workingOverlayIndex != null) || (globalVars.overlayCount != globalVars.overlaysOnMap)) {
                //delete all incoming overlays
                displayMessage(localize.L52);
                clearIncomingOverlays();
                //show all the incoming overlays
                displayIncomingOverlays();
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
            de("attempting to clear " + globalVars.poiObj.length + "POIs...");
            if (globalVars.poiObj.length > 0) {
                displayMessage(localize.L53);
                for (var i = 0; i < globalVars.poiObj.length; i++) {
                    if (globalVars.poiObj[i] != null) {
                        globalVars.poiObj[i].setMap(null);
                        globalVars.poiObj[i] = null;
                    }
                    if (globalVars.poiDesc[i] != null) {
                        globalVars.poiDesc[i] = null;
                    }
                    if (globalVars.poiKML[i] != null) {
                        globalVars.poiKML[i] = null;
                    }
                    infoWindow[i].setMap(null);
                    infoWindow[i] = null;
                    label[i].setMap(null);
                    label[i] = null;
                    var strg = "#poi" + i; //create <li> poi string
                    $(strg).remove(); //remove <li>
                }
                globalVars.poiObj = [];
                globalVars.poi_i = -1;

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
            globalVars.baseImageDirURL = "default/images/mapedit/";                            //the default directory to the image files
            globalVars.mapLayerActive = "Roadmap";                                             //what map layer is displayed
            globalVars.mapDrawingManagerDisplayed = false;                                     //by default, is the drawing manager displayed (true/false)
            globalVars.mapCenter = new google.maps.LatLng(29.6480, -82.3482);                  //used to center map on load
            globalVars.mapControlsDisplayed = true;                                            //by default, are map controls displayed (true/false)
            globalVars.defaultDisplayDrawingMangerTool = false;                                //by default, is the drawingmanger displayed (true/false)
            globalVars.toolboxDisplayed = true;                                                //by default, is the toolbox displayed (true/false)
            globalVars.toolbarDisplayed = true;                                                //by default, is the toolbar open (yes/no)
            globalVars.kmlDisplayed = false;                                                   //by default, is kml layer on (yes/no)
            KmlLayer = new google.maps.KmlLayer("http://ufdc.ufl.edu/design/mapedit/stAugParel_v6.kmz");  //must be pingable by google
            globalVars.defaultZoomLevel = 13;                                                  //zoom level, starting
            globalVars.maxZoomLevel = 2;                                                       //max zoom out, default (21=lowest level, 1=highest level)
            globalVars.minZoomLevel_Terrain = 15;                                              //max zoom in, terrain
            globalVars.minZoomLevel_Satellite = 20;                                            //max zoom in, sat + hybrid
            globalVars.minZoomLevel_Roadmap = 21;                                              //max zoom in, roadmap (default)
            globalVars.minZoomLevel_BlockLot = 19;                                             //max zoom in, used for special layers not having default of roadmap
            globalVars.isCustomOverlay = false;                                                //used to determine if other overlays (block/lot etc) //unknown
            globalVars.preservedRotation = 0;                                                  //rotation, default
            globalVars.knobRotationValue = 0;                                                  //rotation to display by default 
            globalVars.preservedOpacity = 0.75;                                                 //opacity, default value (0-1,1=opaque)
            globalVars.strictBounds = null;                                                    //set the bounds for this google map instance (set to null for no bounds)
            globalVars.hasCustomMapType = true;                                                //used to determine if there is a custom maptype layer
            break;
        case "stAugustine":
            globalVars.baseImageDirURL = "default/images/mapedit/";                            //the default directory to the image files
            globalVars.mapDrawingManagerDisplayed = false;                                     //by default, is the drawing manager displayed (true/false)
            globalVars.mapLayerActive = "Roadmap";                                             //what map layer is displayed
            globalVars.mapCenter = new google.maps.LatLng(29.8944, -81.3147);                  //used to center map on load
            globalVars.mapControlsDisplayed = true;                                            //by default, are map controls displayed (true/false)
            globalVars.defaultDisplayDrawingMangerTool = false;                                //by default, is the drawingmanger displayed (true/false)
            globalVars.toolboxDisplayed = true;                                                //by default, is the toolbox displayed (true/false)
            globalVars.toolbarDisplayed = true;                                                //by default, is the toolbar open (yes/no)
            globalVars.kmlDisplayed = false;                                                   //by default, is kml layer on (yes/no)
            //KmlLayer = new google.maps.KmlLayer("http://ufdc.ufl.edu/design/mapedit/stAugParcel_v6.kmz");  //must be pingable by google
            KmlLayer = new google.maps.KmlLayer("http://ufdc.ufl.edu/design/mapedit/stAugParcel_v6.kmz");  //must be pingable by google
            globalVars.defaultZoomLevel = 14;                                                  //zoom level, starting
            globalVars.maxZoomLevel = 10;                                                      //max zoom out, default (21=lowest level, 1=highest level)
            globalVars.minZoomLevel_Terrain = 15;                                              //max zoom in, terrain
            globalVars.minZoomLevel_Satellite = 20;                                            //max zoom in, sat + hybrid
            globalVars.minZoomLevel_Roadmap = 21;                                              //max zoom in, roadmap (default)
            globalVars.minZoomLevel_BlockLot = 19;                                             //max zoom in, used for special layers not having default of roadmap
            globalVars.isCustomOverlay = false;                                                //used to determine if other overlays (block/lot etc) //unknown
            globalVars.preservedRotation = 0;                                                  //rotation, default
            globalVars.knobRotationValue = 0;                                                  //rotation to display by default 
            globalVars.preservedOpacity = 0.35;                                                 //opacity, default value (0-1,1=opaque)
            globalVars.hasCustomMapType = true;                                                //used to determine if there is a custom maptype layer
            globalVars.strictBounds = new google.maps.LatLngBounds(                            //set the bounds for this google map instance
                new google.maps.LatLng(29.78225755812941, -81.4306640625),
                new google.maps.LatLng(29.99181288866604, -81.1917114257)
            );
            break;
        case "custom":
            globalVars.baseImageDirURL = "default/images/mapedit/";                            //the default directory to the image files
            globalVars.mapDrawingManagerDisplayed = false;                                     //by default, is the drawing manager displayed (true/false)
            globalVars.mapLayerActive = "Roadmap";                                             //what map layer is displayed
            globalVars.mapCenter = new google.maps.LatLng(29.6480, -82.3482);                  //used to center map on load
            globalVars.mapControlsDisplayed = true;                                            //by default, are map controls displayed (true/false)
            globalVars.defaultDisplayDrawingMangerTool = false;                                //by default, is the drawingmanger displayed (true/false)
            globalVars.toolboxDisplayed = true;                                                //by default, is the toolbox displayed (true/false)
            globalVars.toolbarDisplayed = true;                                                //by default, is the toolbar open (yes/no)
            globalVars.kmlDisplayed = false;                                                   //by default, is kml layer on (yes/no)
            KmlLayer = new google.maps.KmlLayer("http://ufdc.ufl.edu/design/mapedit/parcels_2012_kmz_fldor.kmz");  //must be pingable by google
            globalVars.defaultZoomLevel = 13;                                                  //zoom level, starting
            globalVars.maxZoomLevel = 10;                                                      //max zoom out, default (21=lowest level, 1=highest level)
            globalVars.minZoomLevel_Terrain = 15;                                              //max zoom in, terrain
            globalVars.minZoomLevel_Satellite = 20;                                            //max zoom in, sat + hybrid
            globalVars.minZoomLevel_Roadmap = 21;                                              //max zoom in, roadmap (default)
            globalVars.minZoomLevel_BlockLot = 19;                                             //max zoom in, used for special layers not having default of roadmap
            globalVars.isCustomOverlay = false;                                                //used to determine if other overlays (block/lot etc) //unknown
            globalVars.preservedRotation = 0;                                                  //rotation, default
            globalVars.knobRotationValue = 0;                                                  //rotation to display by default 
            globalVars.preservedOpacity = 0.75;                                                 //opacity, default value (0-1,1=opaque)
            globalVars.hasCustomMapType = true;                                                //used to determine if there is a custom maptype layer
            globalVars.strictBounds = new google.maps.LatLngBounds(                            //set the bounds for this google map instance
                new google.maps.LatLng(29.21570636285318, -82.87811279296875),
                new google.maps.LatLng(30.07978967039041, -81.76300048828125)
            );
            break;
        case "florida":
            globalVars.baseImageDirURL = "default/images/mapedit/";                            //the default directory to the image files
            globalVars.mapDrawingManagerDisplayed = false;                                     //by default, is the drawing manager displayed (true/false)
            globalVars.mapLayerActive = "Roadmap";                                             //what map layer is displayed
            globalVars.mapCenter = new google.maps.LatLng(29.6480, -82.3482);                  //used to center map on load
            globalVars.mapControlsDisplayed = true;                                            //by default, are map controls displayed (true/false)
            globalVars.defaultDisplayDrawingMangerTool = false;                                //by default, is the drawingmanger displayed (true/false)
            globalVars.toolboxDisplayed = true;                                                //by default, is the toolbox displayed (true/false)
            globalVars.toolbarDisplayed = true;                                                //by default, is the toolbar open (yes/no)
            globalVars.kmlDisplayed = false;                                                   //by default, is kml layer on (yes/no)
            KmlLayer = new google.maps.KmlLayer("http://hlmatt.com/uf/kml/10.kml"); //must be pingable by google
            globalVars.defaultZoomLevel = 13;                                                  //zoom level, starting
            globalVars.maxZoomLevel = 1;                                                       //max zoom out, default (21=lowest level, 1=highest level)
            globalVars.minZoomLevel_Terrain = 15;                                              //max zoom in, terrain
            globalVars.minZoomLevel_Satellite = 20;                                            //max zoom in, sat + hybrid
            globalVars.minZoomLevel_Roadmap = 21;                                              //max zoom in, roadmap (default)
            globalVars.minZoomLevel_BlockLot = 19;                                             //max zoom in, used for special layers not having default of roadmap
            globalVars.isCustomOverlay = false;                                                //used to determine if other overlays (block/lot etc) 
            globalVars.preservedRotation = 0;                                                  //rotation, default
            globalVars.knobRotationValue = 0;                                                  //rotation to display by default 
            globalVars.preservedOpacity = 0.75;                                                 //opacity, default value (0-1,1=opaque)
            globalVars.hasCustomMapType = true;                                                //used to determine if there is a custom maptype layer
            globalVars.strictBounds = new google.maps.LatLngBounds(                            //set the bounds for this google map instance
                //new google.maps.LatLng(30.69420636285318, -88.04311279296875), //fl nw
                //new google.maps.LatLng(25.06678967039041, -77.33330048828125) //fl se
                //new google.maps.LatLng(24.55531738915811, -81.78283295288095), //fl sw
                //new google.maps.LatLng(30.79109834517092, -81.53709923706058) //fl ne
                //new google.maps.LatLng(29.5862, -82.4146), //gville
                //new google.maps.LatLng(29.7490, -82.2106)
                new google.maps.LatLng(22.053908635225607, -86.18838838405613), //east coast
                new google.maps.LatLng(36.06512404320089, -76.72320000000003)
            );

            //globalVars.strictBounds = new google.maps.LatLngBounds(                            //set the bounds for this google map instance
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
        if (globalVars.placerType == "item") {
            globalVars.firstSaveItem = true;
            //used to prevent multi markers
            if (globalVars.firstMarker > 0) {
                drawingManager.setDrawingMode(null); //only place one at a time
            } else {
                globalVars.firstMarker++;
                drawingManager.setDrawingMode(null); //only place one at a time
            }
            globalVars.itemMarker = marker; //assign globally
            de("marker placed");
            document.getElementById('content_toolbox_posItem').value = globalVars.itemMarker.getPosition();
            globalVars.savingMarkerCenter = globalVars.itemMarker.getPosition(); //store coords to save
            codeLatLng(globalVars.itemMarker.getPosition());
        }

        if (globalVars.placerType == "poi") {
            globalVars.firstSavePOI = true;
            globalVars.poi_i++;

            label[globalVars.poi_i] = new MarkerWithLabel({
                position: marker.getPosition(), //position of real marker
                map: map,
                zIndex: 2,
                labelContent: globalVars.poi_i + 1, //the current user count
                labelAnchor: new google.maps.Point(15, 0),
                labelClass: "labels", // the CSS class for the label
                labelStyle: { opacity: 0.75 },
                icon: {} //initialize to nothing so no marker shows
            });

            globalVars.poiObj[globalVars.poi_i] = marker;
            globalVars.poiType[globalVars.poi_i] = "marker";
            var poiId = globalVars.poi_i + 1;
            var poiDescTemp = L_Marker;
            document.getElementById("poiList").innerHTML += writeHTML("poiListItem", globalVars.poi_i, poiId, poiDescTemp);
            var contentString = writeHTML("poiDesc", globalVars.poi_i, "", "");
            infoWindow[globalVars.poi_i] = new google.maps.InfoWindow({
                content: contentString,
                position: marker.getPosition(),
                pixelOffset: new google.maps.Size(0, -40)
            });
            //infoWindow[globalVars.poi_i].open(map, globalVars.poiObj[globalVars.poi_i]);
            if (globalVars.poiCount > 0) {
                infoWindow[globalVars.poi_i].open(map);
            } else {
                globalVars.poiCount++;
                infoWindow[globalVars.poi_i].setMap(map);
                infoWindow[globalVars.poi_i].setMap(null);
                infoWindow[globalVars.poi_i].setMap(map);
            }
        }

        google.maps.event.addListener(marker, 'dragstart', function () {

            if (globalVars.placerType == "poi") {
                globalVars.firstSavePOI = true;
                for (var i = 0; i < globalVars.poiObj.length; i++) {
                    if (globalVars.poiObj[i] == this) {
                        infoWindow[i].setMap(null);
                        label[i].setMap(null);
                    }
                }
            }
        });
        google.maps.event.addListener(marker, 'dragend', function () {
            if (globalVars.placerType == "item") {
                globalVars.firstSaveItem = true;
                document.getElementById('content_toolbox_posItem').value = marker.getPosition();
                codeLatLng(marker.getPosition());
            }
            if (globalVars.placerType == "poi") {
                globalVars.firstSavePOI = true;
                for (var i = 0; i < globalVars.poiObj.length; i++) {
                    if (globalVars.poiObj[i] == this) {
                        infoWindow[i].setOptions({ position: marker.getPosition(), pixelOffset: new google.maps.Size(0, -40) });
                        infoWindow[i].open(null);
                        label[i].setPosition(marker.getPosition());
                        label[i].setMap(map);
                    }
                }
            }
        });
        google.maps.event.addListener(marker, 'click', function () {
            if (globalVars.placerType == "poi") {
                globalVars.firstSavePOI = true;
                for (var i = 0; i < globalVars.poiObj.length; i++) {
                    if (globalVars.poiObj[i] == this) {
                        infoWindow[i].setOptions({ position: marker.getPosition(), pixelOffset: new google.maps.Size(0, -40) });
                        infoWindow[i].open(map);
                    }
                }
            }
        });
    });
    google.maps.event.addListener(drawingManager, 'circlecomplete', function (circle) {
        testBounds();
        if (globalVars.placerType == "poi") {
            globalVars.firstSavePOI = true;
            globalVars.poi_i++;

            label[globalVars.poi_i] = new MarkerWithLabel({
                position: circle.getCenter(), //position of real marker
                zIndex: 2,
                map: map,
                labelContent: globalVars.poi_i + 1, //the current user count
                labelAnchor: new google.maps.Point(15, 0),
                labelClass: "labels", // the CSS class for the label
                labelStyle: { opacity: 0.75 },
                icon: {} //initialize to nothing so no marker shows
            });

            var poiId = globalVars.poi_i + 1;
            globalVars.poiObj[globalVars.poi_i] = circle;
            globalVars.poiType[globalVars.poi_i] = "circle";
            var poiDescTemp = L_Circle;
            document.getElementById("poiList").innerHTML += writeHTML("poiListItem", globalVars.poi_i, poiId, poiDescTemp);
            var contentString = writeHTML("poiDesc", globalVars.poi_i, "", "");
            infoWindow[globalVars.poi_i] = new google.maps.InfoWindow({
                content: contentString
            });
            infoWindow[globalVars.poi_i].setPosition(circle.getCenter());
            if (globalVars.poiCount > 0) {
                infoWindow[globalVars.poi_i].open(map);
            } else {
                globalVars.poiCount++;
                infoWindow[globalVars.poi_i].setMap(map);
                infoWindow[globalVars.poi_i].setMap(null);
                infoWindow[globalVars.poi_i].setMap(map);
            }
        }

        google.maps.event.addListener(circle, 'dragstart', function () {

            if (globalVars.placerType == "poi") {
                globalVars.firstSavePOI = true;
                for (var i = 0; i < globalVars.poiObj.length; i++) {
                    if (globalVars.poiObj[i] == this) {
                        infoWindow[i].setMap(null);
                        label[i].setMap(null);
                    }
                }
            }
        });
        google.maps.event.addListener(circle, 'drag', function () {
            //used to get the center point for lat/long tool
            globalVars.circleCenter = circle.getCenter();
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
            if (globalVars.placerType == "poi") {
                globalVars.firstSavePOI = true;
                for (var i = 0; i < globalVars.poiObj.length; i++) {
                    if (globalVars.poiObj[i] == this) {
                        infoWindow[i].setPosition(circle.getCenter());
                        infoWindow[i].open(null);
                        label[i].setPosition(circle.getCenter());
                        label[i].setMap(map);
                    }
                }
            }
        });
        google.maps.event.addListener(circle, 'click', function () {
            if (globalVars.placerType == "poi") {
                globalVars.firstSavePOI = true;
                for (var i = 0; i < globalVars.poiObj.length; i++) {
                    if (globalVars.poiObj[i] == this) {
                        infoWindow[i].setPosition(circle.getCenter());
                        infoWindow[i].open(map);
                    }
                }
            }
        });
    });
    google.maps.event.addListener(drawingManager, 'rectanglecomplete', function (rectangle) {

        testBounds();                                   //check the bounds to make sure you havent strayed too far away
        if (globalVars.placerType == "overlay") {
            //mark that we are editing
            globalVars.firstSaveOverlay = true;

            //add the incoming overlay bounds
            globalVars.incomingOverlayBounds[0] = rectangle.getBounds();

            //redisplay overlays (the one we just made)
            displayIncomingOverlays();

            //relist the overlay we drew
            initOverlayList();

            //hide the rectangle we drew
            rectangle.setMap(null);

            //prevent redraw
            drawingManager.setDrawingMode(null);

            //create cache save overlay for the new converted overlay only
            if (globalVars.isConvertedOverlay == true) {
                cacheSaveOverlay(globalVars.workingOverlayIndex);
            }
        }


        if (globalVars.placerType == "poi") {
            globalVars.firstSavePOI = true;
            globalVars.poi_i++;

            label[globalVars.poi_i] = new MarkerWithLabel({
                position: rectangle.getBounds().getCenter(), //position of real marker
                zIndex: 2,
                map: map,
                labelContent: globalVars.poi_i + 1, //the current user count
                labelAnchor: new google.maps.Point(15, 0),
                labelClass: "labels", // the CSS class for the label
                labelStyle: { opacity: 0.75 },
                icon: {} //initialize to nothing so no marker shows
            });

            var poiId = globalVars.poi_i + 1;
            globalVars.poiObj[globalVars.poi_i] = rectangle;
            globalVars.poiType[globalVars.poi_i] = "rectangle";
            var poiDescTemp = L_Rectangle;
            document.getElementById("poiList").innerHTML += writeHTML("poiListItem", globalVars.poi_i, poiId, poiDescTemp);
            var contentString = writeHTML("poiDesc", globalVars.poi_i, "", "");
            infoWindow[globalVars.poi_i] = new google.maps.InfoWindow({
                content: contentString
            });
            infoWindow[globalVars.poi_i].setPosition(rectangle.getBounds().getCenter());
            if (globalVars.poiCount > 0) {
                infoWindow[globalVars.poi_i].open(map);
            } else {
                globalVars.poiCount++;
                infoWindow[globalVars.poi_i].setMap(map);
                infoWindow[globalVars.poi_i].setMap(null);
                infoWindow[globalVars.poi_i].setMap(map);
            }

        }

        google.maps.event.addListener(rectangle, 'bounds_changed', function () {
            if (globalVars.placerType == "poi") {
                globalVars.firstSavePOI = true;
                for (var i = 0; i < globalVars.poiObj.length; i++) {
                    if (globalVars.poiObj[i] == this) {
                        infoWindow[i].setPosition(rectangle.getBounds().getCenter());
                        infoWindow[i].setMap(null);
                        label[i].setPosition(rectangle.getBounds().getCenter());
                        label[i].setMap(map);
                    }
                }
            }
        });
        google.maps.event.addListener(rectangle, 'dragstart', function () {
            if (globalVars.placerType == "poi") {
                globalVars.firstSavePOI = true;
                for (var i = 0; i < globalVars.poiObj.length; i++) {
                    if (globalVars.poiObj[i] == this) {
                        infoWindow[i].setMap(null);
                        label[i].setMap(null);
                    }
                }
            }
        });
        google.maps.event.addListener(rectangle, 'drag', function () {
            if (globalVars.placerType == "poi") {
                globalVars.firstSavePOI = true;
                for (var i = 0; i < globalVars.poiObj.length; i++) {
                    if (globalVars.poiObj[i] == this) {
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
            if (globalVars.placerType == "poi") {
                globalVars.firstSavePOI = true;
                for (var i = 0; i < globalVars.poiObj.length; i++) {
                    if (globalVars.poiObj[i] == this) {
                        infoWindow[i].setPosition(rectangle.getBounds().getCenter());
                        infoWindow[i].open(null);
                        label[i].setPosition(rectangle.getBounds().getCenter());
                        label[i].setMap(map);
                    }
                }
            }
        });
        google.maps.event.addListener(rectangle, 'click', function () {
            if (globalVars.placerType == "poi") {
                globalVars.firstSavePOI = true;
                for (var i = 0; i < globalVars.poiObj.length; i++) {
                    if (globalVars.poiObj[i] == this) {
                        infoWindow[i].setPosition(rectangle.getBounds().getCenter());
                        infoWindow[i].open(map);
                    }
                }
            }
        });

    });
    google.maps.event.addListener(drawingManager, 'polygoncomplete', function (polygon) {
        testBounds();
        if (globalVars.placerType == "poi") {
            globalVars.firstSavePOI = true;
            globalVars.poi_i++;

            label[globalVars.poi_i] = new MarkerWithLabel({
                position: polygonCenter(polygon), //position of real marker
                zIndex: 2,
                map: map,
                labelContent: globalVars.poi_i + 1, //the current user count
                labelAnchor: new google.maps.Point(15, 0),
                labelClass: "labels", // the CSS class for the label
                labelStyle: { opacity: 0.75 },
                icon: {} //initialize to nothing so no marker shows
            });

            var poiId = globalVars.poi_i + 1;
            globalVars.poiObj[globalVars.poi_i] = polygon;
            globalVars.poiType[globalVars.poi_i] = "polygon";
            var poiDescTemp = L_Polygon;
            document.getElementById("poiList").innerHTML += writeHTML("poiListItem", globalVars.poi_i, poiId, poiDescTemp);
            var contentString = writeHTML("poiDesc", globalVars.poi_i, "", "");
            infoWindow[globalVars.poi_i] = new google.maps.InfoWindow({
                content: contentString
            });
            infoWindow[globalVars.poi_i].setPosition(polygonCenter(polygon));
            if (globalVars.poiCount > 0) {
                infoWindow[globalVars.poi_i].open(map);
            } else {
                globalVars.poiCount++;
                infoWindow[globalVars.poi_i].setMap(map);
                infoWindow[globalVars.poi_i].setMap(null);
                infoWindow[globalVars.poi_i].setMap(map);
            }
        }
        google.maps.event.addListener(polygon.getPath(), 'set_at', function () { //if bounds change
            if (globalVars.placerType == "poi") {
                globalVars.firstSavePOI = true;
                for (var i = 0; i < globalVars.poiObj.length; i++) {
                    if (globalVars.poiObj[i] == this) {
                        infoWindow[i].setPosition(polygonCenter(polygon));
                        infoWindow[i].setMap(null);
                        label[i].setPosition(polygonCenter(polygon));
                        label[i].setMap(map); //does not redisplay
                    }
                }
            }
        });
        google.maps.event.addListener(polygon, 'dragstart', function () {
            if (globalVars.placerType == "poi") {
                globalVars.firstSavePOI = true;
                for (var i = 0; i < globalVars.poiObj.length; i++) {
                    if (globalVars.poiObj[i] == this) {
                        infoWindow[i].setMap(null);
                        label[i].setMap(null);
                    }
                }
            }
        });
        google.maps.event.addListener(polygon, 'drag', function () {
            if (globalVars.placerType == "poi") {
                globalVars.firstSavePOI = true;
                for (var i = 0; i < globalVars.poiObj.length; i++) {
                    if (globalVars.poiObj[i] == this) {
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
            if (globalVars.placerType == "poi") {
                globalVars.firstSavePOI = true;
                for (var i = 0; i < globalVars.poiObj.length; i++) {
                    if (globalVars.poiObj[i] == this) {
                        infoWindow[i].setPosition(polygonCenter(polygon));
                        infoWindow[i].open(null);
                        label[i].setPosition(polygonCenter(polygon));
                        label[i].setMap(map);
                    }
                }
            }
        });
        google.maps.event.addListener(polygon, 'click', function () {
            if (globalVars.placerType == "poi") {
                globalVars.firstSavePOI = true;
                for (var i = 0; i < globalVars.poiObj.length; i++) {
                    if (globalVars.poiObj[i] == this) {
                        infoWindow[i].setPosition(polygonCenter(polygon));
                        infoWindow[i].open(map);
                    }
                }
            }
        });
    });
    google.maps.event.addListener(drawingManager, 'polylinecomplete', function (polyline) {
        testBounds();
        if (globalVars.placerType == "poi") {
            globalVars.firstSavePOI = true;
            globalVars.poi_i++;
            var poiId = globalVars.poi_i + 1;
            globalVars.poiObj[globalVars.poi_i] = polyline;
            globalVars.poiType[globalVars.poi_i] = "polyline";
            var poiDescTemp = L_Line;
            document.getElementById("poiList").innerHTML += writeHTML("poiListItem", globalVars.poi_i, poiId, poiDescTemp);
            var contentString = writeHTML("poiDesc", globalVars.poi_i, "", "");
            infoWindow[globalVars.poi_i] = new google.maps.InfoWindow({
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
            infoWindow[globalVars.poi_i].setPosition(polylineStartPoint);
            if (globalVars.poiCount > 0) {
                infoWindow[globalVars.poi_i].open(map);
            } else {
                globalVars.poiCount++;
                infoWindow[globalVars.poi_i].setMap(map);
                infoWindow[globalVars.poi_i].setMap(null);
                infoWindow[globalVars.poi_i].setMap(map);
            }

            label[globalVars.poi_i] = new MarkerWithLabel({
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
            if (globalVars.placerType == "poi") {
                globalVars.firstSavePOI = true;
                de("is poi");
                for (var i = 0; i < globalVars.poiObj.length; i++) {
                    de("inside loop1");
                    if (globalVars.poiObj[i].getPath() == this) {
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
                        infoWindow[globalVars.poi_i].setPosition(polylineStartPoint);
                        infoWindow[globalVars.poi_i].open(null);
                        label[i].setPosition(polylineStartPoint);
                        label[i].setMap(map);
                        de("here3");
                    }
                }

            }
        });
        google.maps.event.addListener(polyline, 'dragstart', function () {
            if (globalVars.placerType == "poi") {
                globalVars.firstSavePOI = true;
                for (var i = 0; i < globalVars.poiObj.length; i++) {
                    if (globalVars.poiObj[i] == this) {
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
            if (globalVars.placerType == "poi") {
                globalVars.firstSavePOI = true;
                //var bounds = new google.maps.LatLngBounds;
                //polyline.getPath().forEach(function (latLng) { bounds.extend(latLng); });
                //var polylineCenter = bounds.getCenter();
                //var bounds = new google.maps.LatLngBounds; //spatial center, bounds holder

                for (var i = 0; i < globalVars.poiObj.length; i++) {
                    if (globalVars.poiObj[i] == this) {
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
            if (globalVars.placerType == "poi") {
                globalVars.firstSavePOI = true;
                //var bounds = new google.maps.LatLngBounds;
                //polyline.getPath().forEach(function (latLng) { bounds.extend(latLng); });
                //var polylineCenter = bounds.getCenter();

                for (var i = 0; i < globalVars.poiObj.length; i++) {
                    if (globalVars.poiObj[i] == this) {
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

    google.maps.event.addDomListener(map, 'mousemove', function (point) {

        if (globalVars.cCoordsFrozen == "no") {
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

    });                                    //used to display cursor location via lat/long
    google.maps.event.addListener(map, 'dragend', function () {
        testBounds();
    });                                              //drag listener (for boundary test)
    google.maps.event.addListener(map, 'zoom_changed', function () {
        checkZoomLevel();
    });                                         //check the zoom level display message if out limits

    //when kml layer is clicked, get feature that was clicked
    google.maps.event.addListener(KmlLayer, 'click', function (kmlEvent) {
        var name = kmlEvent.featureData.name;
        displayMessage("ParcelID: " + name); //temp
    });

    //#endregion

    initGeoObjects(); //initialize all the incoming geo obejects (the fcn is written via c#)

    google.maps.event.addListenerOnce(map, 'tilesloaded', function () {
        //this part runs when the mapobject is created and rendered
        initOptions(); //setup the graphical user interface (enhances visual effect to do all of this after map loads)
        initOverlayList(); //list all the overlays in the list box"
    });
}

//Displays all the points sent from the C# code.
function displayIncomingPoints() {
    if (globalVars.incomingPointCenter) {
        //go through and display points as long as there is a point to display (note, currently only supports one point)
        for (var i = 0; i < globalVars.incomingPointCenter.length; i++) {
            globalVars.firstMarker++;
            globalVars.itemMarker = new google.maps.Marker({
                position: globalVars.incomingPointCenter[i],
                map: map,
                draggable: true,
                title: globalVars.incomingPointLabel[i]
            });
            document.getElementById('content_toolbox_posItem').value = globalVars.itemMarker.getPosition();
            codeLatLng(globalVars.itemMarker.getPosition());
            google.maps.event.addListener(globalVars.itemMarker, 'dragend', function() {
                globalVars.firstSaveItem = true;
                globalVars.savingMarkerCenter = globalVars.itemMarker.getPosition(); //store coords to save
                document.getElementById('content_toolbox_posItem').value = globalVars.itemMarker.getPosition();
                codeLatLng(globalVars.itemMarker.getPosition());
            });
        }
    } else {
        globalVars.firstMarker++;
        globalVars.itemMarker = new google.maps.Marker({
            position: map.getCenter(), //just get the center poin of the map
            map: null, //hide on load
            draggable: false,
            title: globalVars.incomingPointLabel[0]
        });
        //nothing to display because there is no geolocation of item
    }
    
}

//Displays all the overlays sent from the C# code. Also calls displayglobalVars.ghostOverlayRectangle.
function displayIncomingOverlays() {
    for (var i = 0; i < globalVars.incomingOverlayBounds.length; i++) {                                                                                //go through and display overlays as long as there is an overlay to display
        globalVars.overlaysOnMap[i] = new CustomOverlay(i, globalVars.incomingOverlayBounds[i], globalVars.incomingOverlaySourceURL[i], map, globalVars.incomingOverlayRotation[i]);    //create overlay with incoming
        globalVars.overlaysOnMap[i].setMap(map);                                                                                                       //set the overlay to the map

        setGhostOverlay(i, globalVars.incomingOverlayBounds[i]);                                                                                       //set hotspot on top of overlay
        de("I created ghost: " + i);
    }
    globalVars.overlaysCurrentlyDisplayed = true;
}

//clears all incoming overlays
function clearIncomingOverlays() {
    for (var i = 0; i < globalVars.incomingOverlayBounds.length; i++) {                                                                                //go through and display overlays as long as there is an overlay to display
        if (globalVars.overlaysOnMap[i] != null) {
            globalVars.overlaysOnMap[i].setMap(null);
            globalVars.overlaysOnMap[i] = null;
            globalVars.ghostOverlayRectangle[i].setMap(null);
            globalVars.ghostOverlayRectangle[i] = null;
        } else {
            //do nothing
        }
        
    }
    //globalVars.overlaysCurrentlyDisplayed = false;
    globalVars.preservedOpacity = globalVars.defaultOpacity;
}

//Creates and sets the ghost overlays (used to tie actions with actual overlay)
function setGhostOverlay(ghostIndex, ghostBounds) {

    //create ghost directly over an overlay
    globalVars.ghostOverlayRectangle[ghostIndex] = new google.maps.Rectangle();        //init ghost
    globalVars.ghostOverlayRectangle[ghostIndex].setOptions(globalVars.ghosting);                 //set globalVars.ghosting 
    globalVars.ghostOverlayRectangle[ghostIndex].setBounds(ghostBounds);               //set bounds
    globalVars.ghostOverlayRectangle[ghostIndex].setMap(map);                          //set to map

    //create listener for if clicked
    google.maps.event.addListener(globalVars.ghostOverlayRectangle[ghostIndex], 'click', function () {
        if (globalVars.pageMode == "edit") {
            if (globalVars.currentlyEditing == "yes") {                                                            //if editing is being done, save
                if (globalVars.workingOverlayIndex == null) {
                    globalVars.currentlyEditing = "no";
                } else {
                    cacheSaveOverlay(ghostIndex);                                                       //trigger a cache of current working overlay
                    globalVars.ghostOverlayRectangle[globalVars.workingOverlayIndex].setOptions(globalVars.ghosting);                    //set globalVars.rectangle to globalVars.ghosting
                    globalVars.currentlyEditing = "no";                                                            //reset editing marker
                    globalVars.preservedRotation = 0;                                                              //reset preserved rotation
                }
            }
            if (globalVars.currentlyEditing == "no") {                                                             //if editing is not being done, start editing
                $("#toolbox").show();                                                                   //show the toolbox
                globalVars.toolboxDisplayed = true;                                                                //mark that the toolbox is open
                $("#toolboxTabs").accordion({ active: 3 });                                             //open edit overlay tab in toolbox
                globalVars.currentlyEditing = "yes";                                                               //enable editing marker
                globalVars.workingOverlayIndex = ghostIndex;                                                       //set this overay as the one being e
                globalVars.ghostOverlayRectangle[ghostIndex].setOptions(globalVars.editable);                                 //show ghost
                globalVars.currentTopZIndex++;                                                                     //iterate top z index
                document.getElementById("overlay" + ghostIndex).style.zIndex = globalVars.currentTopZIndex;        //bring overlay to front
                globalVars.ghostOverlayRectangle[ghostIndex].setOptions({ zIndex: globalVars.currentTopZIndex });             //bring ghost to front
                for (var i = 0; i < globalVars.savingOverlayIndex.length; i++) {                                   //set rotation if the overlay was previously saved
                    if (ghostIndex == globalVars.savingOverlayIndex[i]) {
                        globalVars.preservedRotation = globalVars.savingOverlayRotation[i];
                    }
                }
            }
        }
    });

    //set listener for bounds changed
    google.maps.event.addListener(globalVars.ghostOverlayRectangle[ghostIndex], 'bounds_changed', function () {
        if (globalVars.pageMode == "edit") {
            globalVars.overlaysOnMap[ghostIndex].setMap(null);                                                                                                                                 //hide previous overlay
            globalVars.overlaysOnMap[ghostIndex] = null;                                                                                                                                       //delete previous overlay values
            globalVars.overlaysOnMap[ghostIndex] = new CustomOverlay(ghostIndex, globalVars.ghostOverlayRectangle[ghostIndex].getBounds(), globalVars.incomingOverlaySourceURL[ghostIndex], map, globalVars.preservedRotation); //redraw the overlay within the new bounds
            globalVars.overlaysOnMap[ghostIndex].setMap(map);                                                                                                                                  //set the overlay with new bounds to the map
            globalVars.currentlyEditing = "yes";                                                                                                                                               //enable editing marker
            cacheSaveOverlay(ghostIndex);                                                                                                                                           //trigger a cache of current working overlay
        }
    });

    //set listener for right click (fixes reset issue over overlays)
    google.maps.event.addListener(globalVars.ghostOverlayRectangle[ghostIndex], 'rightclick', function () {
        drawingManager.setDrawingMode(null); //reset drawing manager no matter what
    });

}

//Stores the overlays to save and their associated data
function cacheSaveOverlay(index) {
    de("caching save overlay");
    de("current save overlay index: " + globalVars.csoi);
    //is this the first save
    globalVars.firstSaveOverlay = true;
    de("firstSaveOvelay: " + globalVars.firstSaveOverlay);
    //set overlay index to save
    globalVars.savingOverlayIndex[globalVars.csoi] = globalVars.workingOverlayIndex;
    de("globalVars.savingOverlayIndex[globalVars.csoi]:" + globalVars.savingOverlayIndex[globalVars.csoi]);
    //set label to save
    globalVars.savingOverlayLabel[globalVars.csoi] = globalVars.incomingOverlayLabel[globalVars.workingOverlayIndex];
    de("globalVars.savingOverlayLabel[globalVars.csoi]:" + globalVars.savingOverlayLabel[globalVars.csoi]);
    //set source url to save
    globalVars.savingOverlaySourceURL[globalVars.csoi] = globalVars.incomingOverlaySourceURL[globalVars.workingOverlayIndex];
    de("globalVars.savingOverlaySourceURL[globalVars.csoi]:" + globalVars.savingOverlaySourceURL[globalVars.csoi]);
    //set bounds to save
    globalVars.savingOverlayBounds[globalVars.csoi] = globalVars.ghostOverlayRectangle[globalVars.workingOverlayIndex].getBounds();
    de("globalVars.savingOverlayBounds[globalVars.csoi]:" + globalVars.savingOverlayBounds[globalVars.csoi]);
    //set rotation to save
    globalVars.savingOverlayRotation[globalVars.csoi] = globalVars.preservedRotation;
    de("globalVars.savingOverlayRotation[globalVars.csoi]:" + globalVars.savingOverlayRotation[globalVars.csoi]);
    //check to see if we just recached or if it was a unique cache
    if (globalVars.savingOverlayIndex[globalVars.csoi] != index) {
        //iterate the current save overlay index   
        globalVars.csoi++;
    }
    de("save overlay cached");
}

//Starts the creation of a custom overlay div which contains a rectangular image.
//Supporting URL: https://developers.google.com/maps/documentation/javascript/overlays#CustomOverlays
function CustomOverlay(id, bounds, image, map, rotation) {
    globalVars.overlayCount++;                 //iterate how many overlays have been drawn
    this.bounds_ = bounds;          //set the bounds
    this.image_ = image;            //set source url
    this.map_ = map;                //set to map
    globalVars.preservedRotation = rotation;   //set the rotation
    this.div_ = null;               //defines a property to hold the image's div. We'll actually create this div upon receipt of the onAdd() method so we'll leave it null for now.
    this.index_ = id;               //set the index/id of this overlay
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
    div.style.opacity = globalVars.preservedOpacity;

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

    //for a preserved rotation
    if (globalVars.preservedRotation != 0) {
        keepRotate(globalVars.preservedRotation);
    }

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
    zoom: globalVars.defaultZoomLevel,                                     //starting zoom level
    minZoom: globalVars.maxZoomLevel,                                      //highest zoom out level
    center: globalVars.mapCenter,                                          //default center point
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

    //menubar
    de("[WARN]: #mapedit_container_pane_0 background color must be set manually if changed from default.");
    document.getElementById("mapedit_container_pane_0").style.display = "block";

    //determine ACL placer type
    if (globalVars.incomingPointCenter.length > 0) {
        //there is an item
        actionsACL("full", "item");
    } else {
        if (globalVars.incomingOverlayBounds.length > 0) {
            //there are overlays
            actionsACL("full", "overlay");
        } else {
            //actionsACL("full", "item");
            //actionsACL("full", "overlay");
            //actionsACL("full", "poi"); //not yet implemented
        }
    }

    //determine ACL maptype toggle
    if (globalVars.hasCustomMapType == true) {
        actionsACL("full", "customMapType");
    } else {
        actionsACL("none", "customMapType");
    }

    //set window offload fcn to remind to save
    window.onbeforeunload = function (e) {
        if (globalVars.userMayLoseData) {
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
            if (globalVars.mapControlsDisplayed == false) { //not present
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
            if (globalVars.toolboxDisplayed == false) { //not present
                document.getElementById("content_menubar_toggleToolbox").className = document.getElementById("content_menubar_toggleToolbox").className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
                document.getElementById("content_toolbar_button_toggleToolbox").className = document.getElementById("content_toolbar_button_toggleToolbox").className.replace(/(?:^|\s)isActive(?!\S)/g, '');
            } else { //present
                document.getElementById("content_menubar_toggleToolbox").className += " isActive2";
                document.getElementById("content_toolbar_button_toggleToolbox").className += " isActive";
            }
            break;
        case "toolbar":
            if (globalVars.toolbarDisplayed == false) { //not present
                document.getElementById("content_menubar_toggleToolbar").className = document.getElementById("content_menubar_toggleToolbar").className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
            } else { //present
                document.getElementById("content_menubar_toggleToolbar").className += " isActive2";
            }
            break;
        case "layer":
            if (globalVars.prevMapLayerActive != null) {
                document.getElementById("content_menubar_layer" + globalVars.prevMapLayerActive).className = document.getElementById("content_menubar_layer" + globalVars.prevMapLayerActive).className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
                document.getElementById("content_toolbar_button_layer" + globalVars.prevMapLayerActive).className = document.getElementById("content_toolbar_button_layer" + globalVars.prevMapLayerActive).className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                document.getElementById("content_toolbox_button_layer" + globalVars.prevMapLayerActive).className = document.getElementById("content_toolbox_button_layer" + globalVars.prevMapLayerActive).className.replace(/(?:^|\s)isActive(?!\S)/g, '');
            }
            document.getElementById("content_menubar_layer" + globalVars.mapLayerActive).className += " isActive2";
            document.getElementById("content_toolbar_button_layer" + globalVars.mapLayerActive).className += " isActive";
            document.getElementById("content_toolbox_button_layer" + globalVars.mapLayerActive).className += " isActive";
            globalVars.prevMapLayerActive = globalVars.mapLayerActive; //set and hold the previous map layer active
            break;
        case "kml":
            if (globalVars.kmlDisplayed == false) { //not present
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
            de("aa: " + globalVars.actionActive + "<br>" + "paa: " + globalVars.prevActionActive);
            if (globalVars.actionActive == "Other") {
                if (globalVars.prevActionActive != null) {
                    document.getElementById("content_menubar_manage" + globalVars.prevActionActive).className = document.getElementById("content_menubar_manage" + globalVars.prevActionActive).className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
                    document.getElementById("content_toolbar_button_manage" + globalVars.prevActionActive).className = document.getElementById("content_toolbar_button_manage" + globalVars.prevActionActive).className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                    document.getElementById("content_toolbox_button_manage" + globalVars.prevActionActive).className = document.getElementById("content_toolbox_button_manage" + globalVars.prevActionActive).className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                }
            } else {
                if (globalVars.prevActionActive != null) {
                    document.getElementById("content_menubar_manage" + globalVars.prevActionActive).className = document.getElementById("content_menubar_manage" + globalVars.prevActionActive).className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
                    document.getElementById("content_toolbar_button_manage" + globalVars.prevActionActive).className = document.getElementById("content_toolbar_button_manage" + globalVars.prevActionActive).className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                    if (document.getElementById("content_toolbox_button_manage" + globalVars.prevActionActive)) {
                        de("found " + globalVars.prevActionActive);
                        document.getElementById("content_toolbox_button_manage" + globalVars.prevActionActive).className = document.getElementById("content_toolbox_button_manage" + globalVars.prevActionActive).className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                    }
                    
                }
                document.getElementById("content_menubar_manage" + globalVars.actionActive).className += " isActive2";
                document.getElementById("content_toolbar_button_manage" + globalVars.actionActive).className += " isActive";
                if (document.getElementById("content_toolbox_button_manage" + globalVars.actionActive)) {
                    de("found " + globalVars.actionActive);
                    document.getElementById("content_toolbox_button_manage" + globalVars.actionActive).className += " isActive";
                }
                globalVars.prevActionActive = globalVars.actionActive; //set and hold the previous map layer active
            }
            break;
    }
    de("buttonAction() completed");
}

//display an inline message
function displayMessage(message) {

    //debug log this message
    de("message #" + globalVars.messageCount + ": " + message); //send to debugger for logging

    //keep a count of messages
    globalVars.messageCount++;

    //check to see if RIB is on
    if (globalVars.RIBMode == true) {
        de("RIB Mode: " + globalVars.RIBMode);
        return;
    } else {
        //display the message

        //debug
        de("RIB Mode: " + globalVars.RIBMode);

        //compile divID
        var currentDivId = "message" + globalVars.messageCount;

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
function createSavedItem(coordinates) {
    var messageType = "item"; //define what message type it is
    //assign data
    var data = messageType + "|" + coordinates + "|";
    var dataPackage = data + "~";
    de("saving item: " + dataPackage); //temp
    toServer(dataPackage);
}

//create a package to send to server to save overlay
function createSavedOverlay(label, source, bounds, rotation) {
    var temp = source;
    if (temp.contains("~") || temp.contains("|")) { //check to make sure reserve characters are not there
        displayMessage(L7);
    }
    //var formattedBounds = 
    var messageType = "overlay"; //define what message type it is
    var data = messageType + "|" + label + "|" + bounds + "|" + source + "|" + rotation + "|";
    var dataPackage = data + "~";
    de("saving overlay set: " + dataPackage); //temp
    toServer(dataPackage);
}

//create a package to send to the server to save poi
function createSavedPOI() {
    var dataPackage = null;
    //cycle through all pois
    de("poi length: " + globalVars.poiObj.length);
    for (var i = 0; i < globalVars.poiObj.length; i++) {
        //get specific geometry 
        switch (globalVars.poiType[i]) {
            case "marker":
                globalVars.poiKML[i] = globalVars.poiObj[i].getPosition().toString();
                break;
            case "circle":
                globalVars.poiKML[i] = globalVars.poiObj[i].getCenter() + "|";
                globalVars.poiKML[i] += globalVars.poiObj[i].getRadius();
                break;
            case "globalVars.rectangle":
                globalVars.poiKML[i] = globalVars.poiObj[i].getBounds().toString();
                break;
            case "polygon":
                globalVars.poiObj[i].getPath().forEach(function (latLng) {
                    globalVars.poiKML[i] += "|";
                    globalVars.poiKML[i] += latLng; //NOTE: this has a non standard coordinate separator    
                });
                break;
            case "polyline":
                globalVars.poiObj[i].getPath().forEach(function (latLng) {
                    globalVars.poiKML[i] += "|";
                    globalVars.poiKML[i] += latLng; //NOTE: this has a non standard coordinate separator    
                });
                break;
            case "deleted":
                //nothing to do here, just a placeholder
                break;
        }
        //filter out the deleted pois
        if (globalVars.poiType[i] != "deleted") {
            //compile data message
            var data = "poi|" + globalVars.poiType[i] + "|" + globalVars.poiDesc[i] + "|" + globalVars.poiKML[i] + "|";
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
    var scriptURL = "default/scripts/serverside/Scripts.aspx";
    $.ajax({
        type: "POST",
        url: globalVars.baseURL + scriptURL + "/SaveItem",
        data: JSON.stringify({ sendData: dataPackage }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            de("server result:" + result);
            displayMessage(L_Saved);
        }
    });
}

//centeres on an overlay
function overlayCenterOnMe(id) {
    //attempt to pan to center of overlay
    map.panTo(globalVars.ghostOverlayRectangle[id].getBounds().getCenter());
}

//toggles overlay for editing
function overlayEditMe(id) {
    var ghostIndex = id;
    globalVars.pageMode = "edit";
    if (globalVars.currentlyEditing == "yes" && globalVars.workingOverlayIndex != null) {                           //if editing is being done and there is something to save, save
        de("saving overlay " + ghostIndex);
        cacheSaveOverlay(ghostIndex);                                                           //trigger a cache of current working overlay
        globalVars.ghostOverlayRectangle[globalVars.workingOverlayIndex].setOptions(globalVars.ghosting);                        //set globalVars.rectangle to globalVars.ghosting
        globalVars.currentlyEditing = "no";                                                                //reset editing marker
        globalVars.preservedRotation = 0;                                                                  //reset preserved rotation
    }
    if (globalVars.currentlyEditing == "no" || globalVars.workingOverlayIndex == null) {
        de("editing overlay " + ghostIndex);
        globalVars.currentlyEditing = "yes"; //enable editing marker
        globalVars.workingOverlayIndex = ghostIndex; //set this overay as the one being e
        globalVars.ghostOverlayRectangle[ghostIndex].setOptions(globalVars.editable); //show ghost
        globalVars.currentTopZIndex++; //iterate top z index
        document.getElementById("overlay" + ghostIndex).style.zIndex = globalVars.currentTopZIndex;        //bring overlay to front
        globalVars.ghostOverlayRectangle[ghostIndex].setOptions({ zIndex: globalVars.currentTopZIndex });           //bring ghost to front
        for (var i = 0; i < globalVars.savingOverlayIndex.length; i++) { //set rotation if the overlay was previously saved
            if (ghostIndex == globalVars.savingOverlayIndex[i]) {
                globalVars.preservedRotation = globalVars.savingOverlayRotation[i];
            }
        }
        overlayCenterOnMe(id);
    }
    displayMessage(L34 + " " + globalVars.incomingOverlayLabel[id]);
}

//hide poi on map
function overlayHideMe(id) {
    globalVars.overlaysOnMap[id].setMap(null);
    globalVars.ghostOverlayRectangle[id].setMap(null);
    document.getElementById("overlayToggle" + id).innerHTML = "<img src=\"" + globalVars.baseURL + globalVars.baseImageDirURL + "add.png\" onclick=\"overlayShowMe(" + id + ");\" />";
    displayMessage(L31 + " " + globalVars.incomingOverlayLabel[id]);
}

//show poi on map
function overlayShowMe(id) {
    globalVars.overlaysOnMap[id].setMap(map);
    globalVars.ghostOverlayRectangle[id].setMap(map);
    document.getElementById("overlayToggle" + id).innerHTML = "<img src=\"" + globalVars.baseURL + globalVars.baseImageDirURL + "sub.png\" onclick=\"overlayHideMe(" + id + ");\" />";
    displayMessage(L32 + " " + globalVars.incomingOverlayLabel[id]);
}

//delete poi from map and list
function overlayDeleteMe(id) {
    globalVars.overlaysOnMap[id].setMap(null);
    globalVars.overlaysOnMap[id] = null;
    globalVars.ghostOverlayRectangle[id].setMap(null);
    globalVars.ghostOverlayRectangle[id] = null;
    var strg = "#overlayListItem" + id; //create <li> poi string
    $(strg).remove(); //remove <li>
    globalVars.overlayCount--;
    displayMessage(id + " " + L33);
}

//open the infoWindow of a poi
function poiEditMe(id) {
    globalVars.poiObj[id].setMap(map);
    infoWindow[id].setMap(map);
    //document.getElementById("overlayListItem" + id).style.backgroundColor = "red"; //not implemented yet
    document.getElementById("poiToggle" + id).innerHTML = "<img src=\"" + globalVars.baseURL + globalVars.baseImageDirURL + "sub.png\" onclick=\"poiHideMe(" + id + ");\" />";
}

//hide poi on map
function poiHideMe(id) {
    globalVars.poiObj[id].setMap(null);
    infoWindow[id].setMap(null);
    label[id].setMap(null);
    document.getElementById("poiToggle" + id).innerHTML = "<img src=\"" + globalVars.baseURL + globalVars.baseImageDirURL + "add.png\" onclick=\"poiShowMe(" + id + ");\" />";
}

//show poi on map
function poiShowMe(id) {
    globalVars.poiObj[id].setMap(map);
    infoWindow[id].setMap(map);
    label[id].setMap(map);
    document.getElementById("poiToggle" + id).innerHTML = "<img src=\"" + globalVars.baseURL + globalVars.baseImageDirURL + "sub.png\" onclick=\"poiHideMe(" + id + ");\" />";
}

//delete poi from map and list
function poiDeleteMe(id) {
    globalVars.poiObj[id].setMap(null);
    globalVars.poiObj[id] = null;
    globalVars.poiType[id] = "deleted";
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

            de("poiDesc[id]: " + globalVars.poiDesc[id]);
            de("temp: " + temp);

            //replace the list item title 
            var tempHTMLHolder1 = document.getElementById("poiList").innerHTML.replace(globalVars.poiDesc[id], temp);
            document.getElementById("poiList").innerHTML = tempHTMLHolder1;

            de("tempHTMLHolder1: " + tempHTMLHolder1);
            de("globalVars.poiDesc[id].substring(0, 20): " + globalVars.poiDesc[id].substring(0, 20));
            de("temp.substring(0, 20): " + temp.substring(0, 20));

            //now replace the title (order is important)
            var tempHTMLHolder2 = document.getElementById("poiList").innerHTML.replace("> " + globalVars.poiDesc[id].substring(0, 20), "> " + temp.substring(0, 20));
            //now post all this back to the listbox
            document.getElementById("poiList").innerHTML = tempHTMLHolder2;

            de("tempHTMLHolder2: " + tempHTMLHolder2);
            de("label[id]" + label[id]);
            de("temp.substring(0, 20): " + temp.substring(0, 20));

            //replace the object label
            label[id].set("labelContent", temp.substring(0, 20));

            de("globalVars.poiDesc[id]: " + globalVars.poiDesc[id]);
            de("temp: " + temp);

            //assign full description to the poi object
            globalVars.poiDesc[id] = temp;

            //close the poi desc box
            infoWindow[id].setMap(null);
        }
    }
    de("poiGetDesc(" + id + "); finished...");
}

//delete search results from map and list
function searchResultDeleteMe() {
    //remove visually
    globalVars.searchResult.setMap(null); //remove from map
    $("#searchResult_1").remove(); //remove the first result div from result list box in toolbox
    document.getElementById("content_toolbar_searchField").value = ""; //clear searchbar
    document.getElementById("content_toolbox_searchField").value = ""; //clear searchbox

    //remove references to 
    globalVars.searchResult = null; //reset search result map item
    globalVars.searchCount = 0; //reset search count
}

//used for lat/long tool
function DisplayCursorCoords(arg) {
    cCoord.innerHTML = arg;
}

//check the zoom level
function checkZoomLevel() {
    var currentZoomLevel = map.getZoom();
    var currentMapType = map.getMapTypeId();
    if (currentZoomLevel == globalVars.maxZoomLevel) {
        displayMessage(L16);
    } else {
        switch (currentMapType) {
            case "roadmap": //roadmap and default
                if (currentZoomLevel == globalVars.minZoomLevel_Roadmap) {
                    displayMessage(L17);
                }
                break;
            case "satellite": //sat
                if (currentZoomLevel == globalVars.minZoomLevel_Satellite) {
                    displayMessage(L17);
                }
                break;
            case "hybrid": //sat
                if (currentZoomLevel == globalVars.minZoomLevel_Satellite) {
                    displayMessage(L17);
                }
                break;
            case "terrain": //terrain only
                if (currentZoomLevel == globalVars.minZoomLevel_Terrain) {
                    displayMessage(L17);
                }
                break;
            case "blocklot":
                if (currentZoomLevel == globalVars.minZoomLevel_BlockLot) {
                    displayMessage(L17);
                }
                break;
        }
        if (globalVars.isCustomOverlay == true) {
            if (currentZoomLevel == globalVars.minZoomLevel_BlockLot) {
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
        value: globalVars.preservedOpacity,
        orientation: "vertical",
        min: 0.00,
        max: 1.00,
        step: 0.01,
        slide: function (event, ui) {
            var selection = $("#overlayTransparencySlider").slider("value");
            de("opacity selected: " + selection);
            keepOpacity(selection);
        }
    });
});

//keeps a specific opacity
function keepOpacity(opacityIn) {
    de("keepOpacity: " + opacityIn);
    var div = document.getElementById("overlay" + globalVars.workingOverlayIndex);
    div.style.opacity = opacityIn;
    globalVars.preservedOpacity = opacityIn;
}

//used to specify a variable opacity (IE adds value to existing)
function opacity(opacityIn) {

    if (globalVars.preservedOpacity <= 1 && globalVars.preservedOpacity >= 0) {
        de("add opacity: " + opacityIn + " to overlay" + globalVars.workingOverlayIndex);
        var div = document.getElementById("overlay" + globalVars.workingOverlayIndex);
        var newOpacity = globalVars.preservedOpacity + opacityIn;
        if (newOpacity > 1) {
            newOpacity = 1;
        }
        if (newOpacity < 0) {
            newOpacity = 0;
        }
        div.style.opacity = newOpacity;
        de("newOpacity: " + newOpacity);
        globalVars.preservedOpacity = newOpacity;
        de("globalVars.preservedOpacity: " + globalVars.preservedOpacity);
        $("#overlayTransparencySlider").slider({ value: globalVars.preservedOpacity });
    } else {
        //could not change the opacity    
    }


}

//jquery rotation knob
$(function ($) {
    $(".knob").knob({
        change: function (value) {
            globalVars.knobRotationValue = value; //assign knob value
            if (value > 180) {
                globalVars.knobRotationValue = ((globalVars.knobRotationValue - 360) * (1)); //used to correct for visual effect of knob error
                //globalVars.knobRotationValue = ((globalVars.knobRotationValue-180)*(-1));
            }
            //only do something if we are in pageEdit Mode and there is an overlay to apply these changes to
            if (globalVars.workingOverlayIndex != null) {
                globalVars.preservedRotation = globalVars.knobRotationValue; //reassign
                keepRotate(globalVars.preservedRotation); //send to display fcn of rotation
                de("globalVars.workingOverlayIndex: " + globalVars.workingOverlayIndex);
                globalVars.savingOverlayRotation[globalVars.workingOverlayIndex] = globalVars.preservedRotation; //just make sure it is prepping for save    
            }
        }
    });

});

//used to maintain specific rotation of overlay
function keepRotate(degreeIn) {
    globalVars.currentlyEditing = "yes"; //used to signify we are editing this overlay
    $(function () {
        $("#overlay" + globalVars.workingOverlayIndex).rotate(degreeIn);
    });
}

//used to specify a variable rotation
function rotate(degreeIn) {
    globalVars.currentlyEditing = "yes"; //used to signify we are editing this overlay
    globalVars.degree = globalVars.preservedRotation;
    globalVars.degree += degreeIn;
    if (degreeIn != 0) {
        $(function () {
            $("#overlay" + globalVars.workingOverlayIndex).rotate(globalVars.degree); //assign overlay to defined rotation
        });
    } else { //if rotation is 0, reset rotation
        $(function () {
            globalVars.degree = 0;
            $("#overlay" + globalVars.workingOverlayIndex).rotate(globalVars.degree);
        });
    }
    globalVars.preservedRotation = globalVars.degree; //preserve rotation value
    globalVars.savingOverlayRotation[globalVars.workingOverlayIndex] = globalVars.preservedRotation; //just make sure it is prepping for save
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
    if (globalVars.strictBounds != null) {
        if (globalVars.strictBounds.contains(map.getCenter())) {
            globalVars.mapInBounds = "yes";
        } else {
            globalVars.mapInBounds = "no";
            map.panTo(globalVars.mapCenter); //recenter
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
            if (globalVars.mapInBounds == "yes") { //if it is inside bounds
                if (globalVars.searchCount > 0) { //check to see if this is the first time searched, if not
                    globalVars.searchResult.setMap(null); //set old search result to not display on map
                } else { //if it is the first time
                    globalVars.searchCount++; //just interate
                }
                globalVars.searchResult = new google.maps.Marker({ //create a new marker
                    map: map, //add to current map
                    position: results[0].geometry.location //set position to search results
                });
                var searchResult_i = 1; //temp, placeholder for later multi search result support
                document.getElementById("searchResults_list").innerHTML = "<div id=\"searchResult_" + searchResult_i + "\" class=\"searchResults_listItem\">" + geo + " <div class=\"searchResults_actionButton\"> <a href=\"#\" onclick=\"searchResultDeleteMe();\"><img src=\"" + globalVars.baseURL + globalVars.baseImageDirURL + "delete.png\"/></a></div></div><br\>"; //add list div
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
    de("search result: " + globalVars.searchResult);
    //check to see if there is a search result
    if (globalVars.searchResult != null) {
        //this tells listeners what to do
        globalVars.placerType = "item";
        //assign new position of marker
        globalVars.itemMarker.setPosition(globalVars.searchResult.getPosition());
        //prevent redraw
        globalVars.firstMarker++;
        //delete search result
        globalVars.searchResultDeleteMe();
        //display new marker
        globalVars.itemMarker.setMap(map);
        //get the lat/long of item marker and put it in the item location tab
        document.getElementById('content_toolbox_posItem').value = globalVars.itemMarker.getPosition();
        //get the reverse geo address for item location and put in location tab
        codeLatLng(globalVars.itemMarker.getPosition());
        //store coords to save
        globalVars.savingMarkerCenter = globalVars.itemMarker.getPosition();
        //add listener for new item marker (can only add once the globalVars.itemMarker is created)
        google.maps.event.addListener(globalVars.itemMarker, 'dragend', function () {
            //get lat/long
            document.getElementById('content_toolbox_posItem').value = globalVars.itemMarker.getPosition();
            //get address
            codeLatLng(globalVars.itemMarker.getPosition());
            //store coords to save
            globalVars.savingMarkerCenter = globalVars.itemMarker.getPosition();
        });
    } else {
        //nothing in search
        displayMessage(L39);
    }
}

//used to convert an incoming point to an overlay
function convertToOverlay() {
    //is there an item to convert and is there a proper source?
    if (globalVars.itemMarker && globalVars.incomingPointSourceURL[0] != "") {
        //hide marker
        globalVars.itemMarker.setMap(null);
        //switch to overlay tab
        actionsACL("none", "item");
        actionsACL("full", "overlay");

        //a simple marker to fix a bug
        //isConverted = true;

        //explicitly open overlay tab (fixes bug)
        openToolboxTab(3);

        //add what we know already
        if (globalVars.incomingPointLabel[0]) {
            globalVars.incomingOverlayLabel[0] = globalVars.incomingPointLabel[0];
        } else {
            de("no incoming point label");
            //ask for it
        }
        if (globalVars.incomingPointSourceURL[0]) {
            globalVars.incomingOverlaySourceURL[0] = globalVars.incomingPointSourceURL[0];
        } else {
            de("no incoming point source url");
            //ask for it
        }
        globalVars.incomingOverlayRotation[0] = 0;

        //adds a working overlay index
        if (globalVars.workingOverlayIndex == null) {
            globalVars.workingOverlayIndex = 0;
        }

        //marks this overlay as converted
        globalVars.isConvertedOverlay = true;

        //converted
        displayMessage(L44);
    } else {
        //cannot convert
        displayMessage(L40);
    }

}

//used to display list of overlays in the toolbox container
function initOverlayList() {
    de("initOverlayList(); started...");
    document.getElementById("overlayList").innerHTML = "";
    if (globalVars.incomingOverlayLabel.length > 0) {
        de("There are " + globalVars.incomingOverlayLabel.length + " Incoming Overlays");
        for (var i = 0; i < globalVars.incomingOverlayLabel.length; i++) {
            de("Adding Overlay List Item");
            if (globalVars.incomingOverlayLabel[i] == "") {
                globalVars.incomingOverlayLabel[i] = "Overlay" + (i + 1);
            }
            document.getElementById("overlayList").innerHTML += writeHTML("overlayListItem", i, globalVars.incomingOverlayLabel[i], "");
        }
    }
}

//used to set acess control levels for the actions
function actionsACL(level, id) {
    //doesnt work
    //document.getElementById("mapedit_container_toolbar").style.width = "1170px";
    //document.getElementById("mapedit_container_toolbar").style["margin-left"] = "-535px";
    switch (id) {
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
            htmlString = "<div id=\"poi" + param1 + "\" class=\"poiListItem\" title=\" New" + param3 + param2 + " \"> New" + param3 + param2 + " <div class=\"poiActionButton\"><a href=\"#\" onclick=\"poiEditMe(" + param1 + ");\"><img src=\"" + globalVars.baseURL + globalVars.baseImageDirURL + "edit.png\"/></a> <a id=\"poiToggle" + param1 + "\" href=\"#\"><img src=\"" + globalVars.baseURL + globalVars.baseImageDirURL + "sub.png\" onclick=\"poiHideMe(" + param1 + ");\" /></a> <a href=\"#\" onclick=\"poiDeleteMe(" + param1 + ");\"><img src=\"" + globalVars.baseURL + globalVars.baseImageDirURL + "delete.png\"/></a></div></div>";
            globalVars.poiDesc[param1] = "New" + param3 + param2;
            break;
        case "poiDesc":
            de("Creating html String");
            htmlString = "<div class=\"poiDescContainer\"> <textarea id=\"poiDesc" + param1 + "\" class=\"descPOI\" placeholder=\"" + L3 + "\"></textarea> <br/> <div class=\"buttonPOIDesc\" id=\"poiGetDesc\" onClick=\"poiGetDesc(" + param1 + ");\">Save</div> </div>";
            break;
        case "overlayListItem":
            de("Creating html String");
            htmlString = "<div id=\"overlayListItem" + param1 + "\" class=\"overlayListItem\" title=\"" + param2 + "\"> " + param2.substring(0, 20) + " <div class=\"overlayActionButton\"><a href=\"#\" onclick=\"overlayEditMe(" + param1 + ");\"><img src=\"" + globalVars.baseURL + globalVars.baseImageDirURL + "edit.png\"/></a> <a id=\"overlayToggle" + param1 + "\" href=\"#\"><img src=\"" + globalVars.baseURL + globalVars.baseImageDirURL + "sub.png\" onclick=\"overlayHideMe(" + param1 + ");\" /></a> <a href=\"#\" onclick=\"overlayDeleteMe(" + param1 + ");\"><img src=\"" + globalVars.baseURL + globalVars.baseImageDirURL + "delete.png\"/></a></div></div>";
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
    var pane2PX = bodyPX * .9;
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
    if (globalVars.savingOverlayIndex.length > 0) {
        de("reseting cache data");
        globalVars.savingOverlayIndex = [];
        globalVars.savingOverlayLabel = [];
        globalVars.savingOverlaySourceURL = [];
        globalVars.savingOverlayBounds = [];
        globalVars.savingOverlayRotation = [];
        de("reseting cache save overlay index");
        globalVars.csoi = 0;
        globalVars.userMayLoseData = false;
        de("cache reset");
    } else {
        de("nothing in cache");
    }
    de("reseting working index");
    globalVars.workingOverlayIndex = null;
    de("reseting preserved rotation");
    globalVars.preservedRotation = 0;
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
                    if (globalVars.cCoordsFrozen == "no") {
                        //freeze
                        globalVars.cCoordsFrozen = "yes";
                        displayMessage(L20);
                    } else {
                        //unfreeze
                        globalVars.cCoordsFrozen = "no";
                        displayMessage(L21);
                    }
                }
            }
            break;
        case 79: //O
            if (isCntrlDown == true) {
                if (globalVars.overlaysCurrentlyDisplayed == true) {
                    displayMessage(L22);
                    for (var i = 0; i < globalVars.incomingOverlayBounds.length; i++) { //go through and display overlays as long as there is an overlay to display
                        globalVars.overlaysOnMap[i].setMap(null); //hide the overlay from the map
                        globalVars.ghostOverlayRectangle[i].setMap(null); //hide ghost from map
                        globalVars.overlaysCurrentlyDisplayed = false; //mark that overlays are not on the map
                    }
                } else {
                    displayMessage(L23);
                    for (var i = 0; i < globalVars.incomingOverlayBounds.length; i++) { //go through and display overlays as long as there is an overlay to display
                        globalVars.overlaysOnMap[i].setMap(map); //set the overlay to the map
                        globalVars.ghostOverlayRectangle[i].setMap(map); //set to map
                        globalVars.overlaysCurrentlyDisplayed = true; //mark that overlays are on the map
                    }
                }
            }
            break;
        case 68: //D (for debuggin)
            if (isCntrlDown == true) {
                debugs++;
                if (debugs % 2 == 0) {
                    document.getElementById("debugs").style.display = "none";
                    globalVars.debugMode = false;
                    isCntrlDown = false;
                    displayMessage("Debug Mode Off");
                } else {
                    document.getElementById("debugs").style.display = "block";
                    globalVars.debugMode = true;
                    displayMessage("Debug Mode On");
                    isCntrlDown = false;
                }
            }
            break;
    }
}

//debugging 
var debugStringDefault = "<strong>Debug Panel:</strong> <a onclick=\"debugClear()\">(clear)</a><br><br>"; //starting debug string
var debugString = debugStringDefault;
var debugs = 0; //used for keycode debugging
function de(message) {
    //create debug string
    var currentdate = new Date();
    var time = currentdate.getHours() + ":" + currentdate.getMinutes() + ":" + currentdate.getSeconds() + ":" + currentdate.getMilliseconds();
    debugString += "[" + time + "] " + message + "<br><hr>";
    document.getElementById("debugs").innerHTML = debugString;
}
function debugClear() {
    debugString = debugStringDefault;
    document.getElementById("debugs").innerHTML = debugString;

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
        $("#content_toolbox_button_placeItem").tooltip({ track: true });
        $("#content_toolbox_button_overlayPlace").tooltip({ track: true });
        $("#content_toolbox_button_placePOI").tooltip({ track: true });
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
        $("#content_toolbox_button_placeItem").tooltip({ track: true });
        $("#descItem").tooltip({ track: true });
        $("#content_toolbox_button_saveItem").tooltip({ track: true });
        $("#content_toolbox_button_overlayPlace").tooltip({ track: true });
        $("#content_toolbox_button_saveOverlay").tooltip({ track: true });
        $("#content_toolbox_button_placePOI").tooltip({ track: true });
        $("#descPOI").tooltip({ track: true });
        $("#content_toolbox_button_savePOI").tooltip({ track: true });
        $("#content_toolbox_button_itemGetUserLocation").tooltip({ track: true });
        $("#content_toolbox_button_overlayGetUserLocation").tooltip({ track: true });
        $("#content_toolbox_button_overlayEdit").tooltip({ track: true });
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