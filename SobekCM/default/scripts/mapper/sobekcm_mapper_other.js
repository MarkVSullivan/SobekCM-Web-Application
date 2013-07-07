//other 

//#region Supporting JS

//jquery elements
$(function () {
    //draggable
    $("#container_toolbox").draggable({ handle: "#container_toolboxMinibar" });
    //accordian
    $("#container_toolboxTabs").accordion({ active: 0, icons: false, heightStyle: "content" });
    //tooltips
    $("#content_toolbarGrabber").tooltip();
    $("#content_toolbar_button_reset").tooltip();
    $("#content_toolbar_button_toggleMapControls").tooltip();
    $("#content_toolbar_button_toggleToolbox").tooltip();
    $("#content_toolbar_button_layerRoadmap").tooltip();
    $("#content_toolbar_button_layerSatellite").tooltip();
    $("#content_toolbar_button_layerTerrain").tooltip();
    $("#content_toolbar_button_layerControls").tooltip();
    $("#content_toolbar_button_layerHybrid").tooltip();
    $("#content_toolbar_button_layerCustom").tooltip();
    $("#content_toolbar_button_layerReset").tooltip();
    $("#content_toolbar_button_zoomIn").tooltip();
    $("#content_toolbar_button_zoomReset").tooltip();
    $("#content_toolbar_button_zoomOut").tooltip();
    $("#content_toolbar_button_panUp").tooltip();
    $("#content_toolbar_button_panDown").tooltip();
    $("#content_toolbar_button_panReset").tooltip();
    $("#content_toolbar_button_panLeft").tooltip();
    $("#content_toolbar_button_panRight").tooltip();
    $("#content_toolbar_button_manageItem").tooltip();
    $("#content_toolbar_button_manageOverlay").tooltip();
    $("#content_toolbar_button_managePOI").tooltip();
    $("#content_toolbox_button_reset").tooltip();
    $("#content_toolbox_button_toggleMapControls").tooltip();
    $("#content_toolbox_button_layerRoadmap").tooltip();
    $("#content_toolbox_button_layerSatellite").tooltip();
    $("#content_toolbox_button_layerTerrain").tooltip();
    $("#content_toolbox_button_layerControls").tooltip();
    $("#content_toolbox_button_layerHybrid").tooltip();
    $("#content_toolbox_button_layerCustom").tooltip();
    $("#content_toolbox_button_layerReset").tooltip();
    $("#content_toolbox_button_zoomIn").tooltip();
    $("#content_toolbox_button_zoomReset").tooltip();
    $("#content_toolbox_button_zoomOut").tooltip();
    $("#content_toolbox_button_panUp").tooltip();
    $("#content_toolbox_button_panDown").tooltip();
    $("#content_toolbox_button_panReset").tooltip();
    $("#content_toolbox_button_panLeft").tooltip();
    $("#content_toolbox_button_panRight").tooltip();
    $("#content_toolbox_button_manageItem").tooltip();
    $("#content_toolbox_button_manageOverlay").tooltip();
    $("#content_toolbox_button_managePOI").tooltip();
    $("#content_toolbox_button_placeItem").tooltip();
    $("#content_toolbox_button_placeOverlay").tooltip();
    $("#content_toolbox_button_placePOI").tooltip();
    $("#content_toolbox_button_poiMarker").tooltip();
    $("#content_toolbox_button_poiCircle").tooltip();
    $("#content_toolbox_button_poiRectangle").tooltip();
    $("#content_toolbox_button_poiPolygon").tooltip();
    $("#content_toolbox_button_poiLine").tooltip();
    $("#rotationKnob").tooltip();
    $("#content_toolbox_rotationClockwise").tooltip();
    $("#content_toolbox_rotationReset").tooltip();
    $("#content_toolbox_rotationCounterClockwise").tooltip();
    $("#transparency").tooltip();
    $("#content_toolbox_rgItem").tooltip();
    $("#content_toolbox_posItem").tooltip();
    $("#content_toolbox_button_placeItem").tooltip();
    $("#descItem").tooltip();
    $("#content_toolbox_button_saveItem").tooltip();
    $("#content_toolbox_button_placeOverlay").tooltip();
    $("#content_toolbox_button_saveOverlay").tooltip();
    $("#content_toolbox_button_placePOI").tooltip();
    $("#descPOI").tooltip();
    $("#content_toolbox_button_savePOI").tooltip();
    $("#content_toolbox_button_itemGetUserLocation").tooltip();
    $("#content_toolbox_button_overlayGetUserLocation").tooltip();
    $("#content_toolbox_button_poiGetUserLocation").tooltip();
    $("#content_toolbox_button_clearItem").tooltip();
    $("#content_toolbox_button_clearOverlay").tooltip();
    $("#content_toolbox_button_clearPOI").tooltip();
    $("#content_toolbar_searchField").tooltip();
    $("#content_toolbar_searchButton").tooltip();
    $("#content_toolbox_searchField").tooltip();
    $("#content_toolbox_searchResults").tooltip();
    //$(".selector").tooltip({ content: "Awesome title!" });

});                               

//#endregion