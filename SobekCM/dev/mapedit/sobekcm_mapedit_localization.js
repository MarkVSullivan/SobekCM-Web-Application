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