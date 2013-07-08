//localization

//by variables (used in fcns)
var L_Marker = "Marker";
var L_Circle = "Circle";
var L_Rectangle = "Rectangle";
var L_Polygon = "Polygon";
var L_Line = "Line";
var L1 = "SobekCM Plugin <a href=\"#\">Report a Sobek Error</a>"; //copyright node
var L2 = "lat: <a id=\"cLat\"></a><br/>long: <a id=\"cLong\"></a>"; //lat long of cursor position tool
var L3 = "Description (Optional)"; //describe poi box
var L4 = "Geolocation Service Failed."; //geolocation buttons error message
var L5 = "Returned to Bounds!"; //tesbounds();
var L6 = "Could not find location. Either the format you entered is invalid or the location is outside of the map bounds."; //codeAddress();
var L7 = "Error: Overlay image source cannot contain a ~ or |"; //createSavedOverlay();
var L8 = "Error: Description cannot contain a ~ or |"; //poiGetDesc(id);
var L9 = "Item Cleared!"; //buttonClearItem();
var L10 = "Overlay Cleared!"; //buttonClearOverlay();
var L11 = "POI Set Cleared!"; //buttonClearPOI();
var L12 = "Nothing Happened!"; //HandleResult(arg);
var L13 = "Item Saved!"; //HandleResult(arg);
var L14 = "Overlay Saved!"; //HandleResult(arg);
var L15 = "POI Set Saved!"; //HandleResult(arg);
var L16 = "Cannot Zoom Out Further"; //checkZoomLevel();
var L17 = "Cannot Zoom In Further"; //checkZoomLevel();
var L18 = "Using Search Results as Location"; //marker complete listener

//by listeners
try {
    //toolbar
    document.getElementById("content_toolbar_button_reset").title = "Reset: Reset Map To Defaults";
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
    document.getElementById("content_toolbox_searchResults").title = "Locate: Find A Location On The Map";
    //tab
    document.getElementById("content_toolbox_button_placeItem").title = "Place a Point";
    document.getElementById("content_toolbox_button_itemGetUserLocation").title = "Center On Your Current Position";
    document.getElementById("content_toolbox_posItem").title = "Coordinates: This is the selected Latitude and Longitude of the point you selected.";
    document.getElementById("content_toolbox_rgItem").title = "Address: This is the nearest address of the point you selected.";
    document.getElementById("content_toolbox_button_saveItem").title = "Save Location";
    document.getElementById("content_toolbox_button_clearItem").title = "Clear Item";
    //tab
    document.getElementById("content_toolbox_button_placeOverlay").title = "Toggle Overlay Editing";
    document.getElementById("content_toolbox_button_overlayGetUserLocation").title = "Center On Your Current Position";
    document.getElementById("rotation").title = "Rotate: Edit the rotation value";
    document.getElementById("rotationKnob").title = "Rotate: Edit the rotation value";
    document.getElementById("content_toolbox_rotationCounterClockwise").title = ".1&deg Left: Click to Rotate .1&deg Counter-Clockwise";
    document.getElementById("content_toolbox_rotationReset").title = "Reset: Click to Reset Rotation";
    document.getElementById("content_toolbox_rotationClockwise").title = ".1&deg Right: Click to Rotate .1&deg Clockwise";
    document.getElementById("transparency").title = "Transparency: Set the transparency of this Overlay";
    document.getElementById("content_toolbox_button_saveOverlay").title = "Save Overlay";
    document.getElementById("content_toolbox_button_clearOverlay").title = "Clear Overlay";
    //tab
    document.getElementById("content_toolbox_button_placePOI").title = "Place Point Of Interest";
    document.getElementById("content_toolbox_button_poiGetUserLocation").title = "Center On Your Current Position";
    document.getElementById("content_toolbox_button_poiMarker").title = "Marker: Place a Point";
    document.getElementById("content_toolbox_button_poiCircle").title = "Circle: Place a Circle";
    document.getElementById("content_toolbox_button_poiRectangle").title = "Rectangle: Place a Rectangle";
    document.getElementById("content_toolbox_button_poiPolygon").title = "Polygon: Place a Polygon";
    document.getElementById("content_toolbox_button_poiLine").title = "Line: Place a Line";
    document.getElementById("content_toolbox_button_savePOI").title = "Save Point Of Interest Set";
    document.getElementById("content_toolbox_button_clearPOI").title = "Clear Point Of Interest Set";
    
} catch(err) {
    alert("ERROR: Failed Adding Titles");
}

//by textual content
try {
    document.getElementById("content_minibar_header").innerHTML = "Toolbox";
    document.getElementById("content_toolbox_button_saveItem").innerHTML = "Save";
    document.getElementById("content_toolbox_button_clearItem").innerHTML = "Clear";
    document.getElementById("content_toolbox_button_saveOverlay").innerHTML = "Save";
    document.getElementById("content_toolbox_button_clearOverlay").innerHTML = "Clear";
    document.getElementById("content_toolbox_button_savePOI").innerHTML = "Save";
    document.getElementById("content_toolbox_button_clearPOI").innerHTML = "Clear";
    document.getElementById("content_toolbox_tab1_header").innerHTML = "Map Controls";
    document.getElementById("content_toolbox_tab2_header").innerHTML = "Actions";
    document.getElementById("content_toolbox_tab3_header").innerHTML = "Manage Location";
    document.getElementById("content_toolbox_tab4_header").innerHTML = "Manage Overlay";
    document.getElementById("content_toolbox_tab5_header").innerHTML = "Manage POI";
    document.getElementById("content_toolbar_searchField").setAttribute('placeholder', "Find On Map");
    document.getElementById("content_toolbox_searchField").setAttribute('placeholder', "Find On Map");
    document.getElementById("content_toolbox_posItem").setAttribute('placeholder', "Selected Lat/Long");
    document.getElementById("content_toolbox_rgItem").setAttribute('placeholder', "Nearest Address");
    
} catch (err) {
    alert("ERROR: Failed Adding Textual Content");
}
