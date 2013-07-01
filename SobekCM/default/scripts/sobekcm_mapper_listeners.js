    //Add Listeners
try{
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
        //currently not used
     }, false);
    document.getElementById("content_toolbarGrabber").addEventListener("click", function () {
        toggleVis("toolbar");
    }, false);
    //toolbox
    document.getElementById("content_minibar_button_minimize").addEventListener("click", function () {
        toggleVis("toolboxMin");
    }, false);
    document.getElementById("content_minibar_button_maximize").addEventListener("click", function () { 
        toggleVis("toolboxMax");
     }, false);
    document.getElementById("content_minibar_button_close").addEventListener("click", function () { 
        toggleVis("toolbox");
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
        //nothing yet
     }, false);
    document.getElementById("content_toolbox_searchResults").addEventListener("click", function () { 
        //nothing yet
     }, false);
    //tab
    document.getElementById("content_toolbox_button_placeItem").addEventListener("click", function () { 
        if (searchCount > 0 && itemMarker == null) {
            useSearchAsItemLocation();
            displayMessage(L18);
        } else {
            drawingManager.setDrawingMode(google.maps.drawing.OverlayType.MARKER);
            placerType = "item";
        }
     }, false);
    document.getElementById("content_toolbox_button_itemGetUserLocation").addEventListener("click", function () { 
        drawingManager.setDrawingMode(google.maps.drawing.OverlayType.MARKER);
        placerType = "item";
        // Try W3C Geolocation
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(function (position) {
                var userLocation = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);
                map.setCenter(userLocation);
                testBounds();
                markerCenter = userLocation;
                itemMarker = new google.maps.Marker({
                    position: markerCenter,
                    map: map
                });
                itemMarker.setMap(map);
                document.getElementById('content_toolbox_posItem').value = markerCenter;
                savingMarkerCenter = itemMarker.getPosition(); //store coords to save
            });

        } else {
            alert(L4);
        }
        drawingManager.setDrawingMode(null);
     }, false);
    document.getElementById("content_toolbox_posItem").addEventListener("click", function () { 
        //nothing, maybe copy?
     }, false);
    document.getElementById("content_toolbox_rgItem").addEventListener("click", function () { 
        //nothing, maybe copy?
    }, false);
    document.getElementById("content_toolbox_button_saveItem").addEventListener("click", function () { 
        buttonSaveItem();
     }, false);
    document.getElementById("content_toolbox_button_clearItem").addEventListener("click", function () { 
        buttonClearItem();
     }, false);
    //tab
    document.getElementById("content_toolbox_button_placeOverlay").addEventListener("click", function () { 
        placerType = "overlay";
        if (pageMode == "edit") {
            pageMode = "view";
            if (savingOverlayIndex.length > 0) {
                for (var i = 0; i < savingOverlayIndex.length; i++) {
                    ghostOverlayRectangle[savingOverlayIndex[i]].setOptions(ghosting); //set rectangle to ghosting    
                }
            }
            displayMessage("Overlay Editting Turned Off");
        } else {
            pageMode = "edit";
            displayMessage("Overlay Editting Turned On");
        }
        //toggleOverlayEditor(); 
     }, false);
    document.getElementById("content_toolbox_button_overlayGetUserLocation").addEventListener("click", function () { 
        placerType = "overlay";
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
     }, false);
    document.getElementById("rotationKnob").addEventListener("click", function () { 
        //do nothing, (possible just container)
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
        //nothing (possible just container)
     }, false);
    document.getElementById("overlayTransparencySlider").addEventListener("click", function () { 
        //nothing (possible just container)
     }, false);
    document.getElementById("content_toolbox_button_saveOverlay").addEventListener("click", function () { 

     }, false);
    document.getElementById("content_toolbox_button_clearOverlay").addEventListener("click", function () { 

     }, false);
    //tab
    document.getElementById("content_toolbox_button_placePOI").addEventListener("click", function () { 
        drawingManager.setOptions({ drawingControl: true, drawingControlOptions: { position: google.maps.ControlPosition.RIGHT_TOP, drawingModes: [google.maps.drawing.OverlayType.MARKER, google.maps.drawing.OverlayType.CIRCLE, google.maps.drawing.OverlayType.RECTANGLE, google.maps.drawing.OverlayType.POLYGON, google.maps.drawing.OverlayType.POLYLINE] } });
        placerType = "poi";
     }, false);
    document.getElementById("content_toolbox_button_poiGetUserLocation").addEventListener("click", function () { 
        //drawingManager.setDrawingMode(google.maps.drawing.OverlayType.MARKER);
        placerType = "poi";
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
     }, false);
    document.getElementById("content_toolbox_button_poiMarker").addEventListener("click", function () { 
        placerType = "poi";
        drawingManager.setDrawingMode(google.maps.drawing.OverlayType.MARKER);
     }, false);
    document.getElementById("content_toolbox_button_poiCircle").addEventListener("click", function () { 
        placerType = "poi";
        drawingManager.setDrawingMode(google.maps.drawing.OverlayType.CIRCLE);
     }, false);
    document.getElementById("content_toolbox_button_poiRectangle").addEventListener("click", function () { 
        placerType = "poi";
        drawingManager.setDrawingMode(google.maps.drawing.OverlayType.RECTANGLE);
     }, false);
    document.getElementById("content_toolbox_button_poiPolygon").addEventListener("click", function () { 
        placerType = "poi";
        drawingManager.setDrawingMode(google.maps.drawing.OverlayType.POLYGON);
     }, false);
    document.getElementById("content_toolbox_button_poiLine").addEventListener("click", function () { 
        placerType = "poi";
        drawingManager.setDrawingMode(google.maps.drawing.OverlayType.POLYLINE);
     }, false);
    document.getElementById("content_toolbox_button_savePOI").addEventListener("click", function () { 
        //this is done in the other js file
     }, false);
    document.getElementById("content_toolbox_button_clearPOI").addEventListener("click", function () { 
        
    }, false);
} catch(err) {
    alert("ERROR: Failed Adding Listeners");
}

