//listeners

try {
    document.getElementById("content_toolbar_button_reset").addEventListener("click", function () {
        resetAll();
    }, false);
    document.getElementById("content_toolbar_button_toggleMapControls").addEventListener("click", function() {
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
    document.getElementById("content_toolbar_button_manageItem").addEventListener("click", function() {
        action("manageItem");
    }, false);
    document.getElementById("content_toolbar_button_manageOverlay").addEventListener("click", function () {
        action("manageOverlay");
    }, false);
    document.getElementById("content_toolbar_button_managePOI").addEventListener("click", function () {
        action("managePOI");
    }, false);
    document.getElementById("content_toolbar_searchField").addEventListener("click", function () { 
        //currently in js
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
        action("other");
        openToolboxTab(0);
    }, false);
    document.getElementById("content_toolbox_tab2_header").addEventListener("click", function () {
        action("other");
        openToolboxTab(1);
    }, false);
    document.getElementById("content_toolbox_tab3_header").addEventListener("click", function () {
        action("manageItem");
        //openToolboxTab(2); //called in action
    }, false);
    document.getElementById("content_toolbox_tab4_header").addEventListener("click", function () {
        action("manageOverlay");
        //openToolboxTab(3); //called in action
    }, false);
    document.getElementById("content_toolbox_tab5_header").addEventListener("click", function () {
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
    document.getElementById("content_toolbox_button_placeOverlay").addEventListener("click", function () {
        place("overlay");
    }, false);
    document.getElementById("content_toolbox_button_overlayGetUserLocation").addEventListener("click", function () {
        geolocate("overlay");
    }, false);
    document.getElementById("rotationKnob").addEventListener("click", function () { 
        //do nothing, (possible just mapper_container)
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
        //nothing (possible just mapper_container)
     }, false);
    document.getElementById("overlayTransparencySlider").addEventListener("click", function () { 
        //nothing (possible just mapper_container)
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
} catch(err) {
    alert(Lerror1);
}

