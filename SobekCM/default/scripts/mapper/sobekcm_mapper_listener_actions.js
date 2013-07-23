//listener actions

//reset handler
function resetAll() {
    document.location.reload(true); //refresh page
}

//toggle items handler
function toggleVis(id) {
    switch (id) {
        case "mapControls":
            if (mapControlsDisplayed == true) { //not present
                map.setOptions({
                    zoomControl: false,
                    panControl: false,
                    mapTypeControl: false
                });
                mapControlsDisplayed = false;
            } else { //present
                map.setOptions({
                    zoomControl: true,
                    zoomControlOptions: { style: google.maps.ZoomControlStyle.SMALL, position: google.maps.ControlPosition.LEFT_TOP },
                    panControl: true,
                    panControlOptions: { position: google.maps.ControlPosition.LEFT_TOP },
                    mapTypeControl: true,
                    mapTypeControlOptions: { style: google.maps.MapTypeControlStyle.DROPDOWN_MENU, position: google.maps.ControlPosition.RIGHT_TOP }
                });               
                mapControlsDisplayed = true;
            }
            buttonActive("mapControls"); //set the is active glow for button
            break;
            
        case "toolbox":
            if (toolboxDisplayed == true) {
                document.getElementById("mapper_container_toolbox").style.display = "none";
                document.getElementById("mapper_container_toolboxTabs").style.display = "none";
                //$("#mapper_container_toolbox").effect("slide", 500);
                toolboxDisplayed = false;
            } else {
                document.getElementById("mapper_container_toolbox").style.display = "block";
                document.getElementById("mapper_container_toolboxTabs").style.display = "block";
                document.getElementById("mapper_container_toolbox").style.height = "auto";
                toolboxDisplayed = true;
            }
            buttonActive("toolbox"); //set the is active glow for button
            break;
            
        case "toolbar":
            if (toolbarDisplayed == true) {
                $("#mapper_container_pane_1").hide();
                document.getElementById("mapper_container_toolbarGrabber").style.marginTop = "0";
                toolbarDisplayed = false;
            } else {
                $("#mapper_container_pane_1").show();
                document.getElementById("mapper_container_toolbarGrabber").style.marginTop = "48px";
                toolbarDisplayed = true;
            }
            buttonActive("toolbar"); //set the is active glow for button
            break;
            
        case "kml":
            if (kmlDisplayed == true) {
                kmlLayer.setMap(null);
                kmlDisplayed = false;
            } else {
                kmlLayer.setMap(map);
                kmlDisplayed = true;
            }
            buttonActive("kml"); //set the is active glow for button
            break;
            
        case "toolboxMin":
            $("#mapper_container_toolboxTabs").hide();
            document.getElementById("mapper_container_toolbox").style.height = "17px";
            break;
            
        case "toolboxMax":
            $("#mapper_container_toolboxTabs").show();
            document.getElementById("mapper_container_toolbox").style.height = "auto";
            break;
            
        case "mapDrawingManager":
            if (mapDrawingManagerDisplayed == true) {
                drawingManager.setMap(null);
                mapDrawingManagerDisplayed = false;
            } else {
                drawingManager.setMap(map);
                mapDrawingManagerDisplayed = true;
            }
            //buttonActive("mapDrawingManager"); 
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
            mapLayerActive = "Roadmap";
            break;
        case "terrain":
            map.setMapTypeId(google.maps.MapTypeId.TERRAIN);
            mapLayerActive = "Terrain";
            break;
        case "satellite":
            map.setMapTypeId(google.maps.MapTypeId.SATELLITE);
            mapLayerActive = "Satellite";
            break;
        case "hybrid":
            map.setMapTypeId(google.maps.MapTypeId.HYBRID);
            mapLayerActive = "Hybrid";
            break;
        case "custom":
            toggleVis("kml");
            break;
        case "reset":
            map.setMapTypeId(google.maps.MapTypeId.ROADMAP);
            //2do make this set to default
            mapLayerActive = "Roadmap";
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
            map.panTo(mapCenter);
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
            map.setZoom(defaultZoomLevel);
            break;
    }
}

//user initiated action handler
function action(id) {
    switch (id) {
        case "manageItem":
            actionActive = "Item"; //notice case (uppercase is tied to the actual div)
            buttonActive("action");
            if (toolboxDisplayed != true) {
                toggleVis("toolbox");
            }
            openToolboxTab(2);
            //force a suppression dm
            if (mapDrawingManagerDisplayed == true) {
                mapDrawingManagerDisplayed = true;
                toggleVis("mapDrawingManager");
            }
            placerType = "item";
            break;
            
        case "manageOverlay":
            actionActive = "Overlay"; //notice case (uppercase is tied to the actual div)
            buttonActive("action");
            if (toolboxDisplayed != true) {
                toggleVis("toolbox");
            }
            openToolboxTab(3);
            //force a suppression dm
            if (mapDrawingManagerDisplayed == true) {
                mapDrawingManagerDisplayed = true;
                toggleVis("mapDrawingManager");
            }
            placerType = "overlay";
            break;
            
        case "managePOI":
            actionActive = "POI"; //notice case (uppercase is tied to the actual div)
            buttonActive("action");
            if (toolboxDisplayed != true) {
                toggleVis("toolbox");
            }
            openToolboxTab(4);
            toggleVis("mapDrawingManager");
            placerType = "poi";
            break;
            
        case "other":
            actionActive = "Other";
            buttonActive("action");
            //openToolboxTab(); //not called here, called in listerner
            //force a suppression dm
            if (mapDrawingManagerDisplayed == true) {
                mapDrawingManagerDisplayed = true;
                toggleVis("mapDrawingManager");
            }
            placerType = "none";
            break;
    }
}

//placer button handler
function place(id) {
    switch (id) {
        case "item":
            placerType = "item";
            if (itemMarker != null) {
                displayMessage(L30);
            } else {
                if (searchCount > 0 && itemMarker == null) {
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
            placerType = "overlay";
            if (pageMode == "edit") {
                pageMode = "view";
                if (savingOverlayIndex.length > 0) {
                    for (var i = 0; i < savingOverlayIndex.length; i++) {
                        ghostOverlayRectangle[savingOverlayIndex[i]].setOptions(ghosting); //set rectangle to ghosting    
                    }
                }
                displayMessage(L26);
            } else {
                pageMode = "edit";
                displayMessage(L27);
            }
            //toggleOverlayEditor(); 
            break;
            
        case "poi":
            drawingManager.setOptions({ drawingControl: true, drawingControlOptions: { position: google.maps.ControlPosition.RIGHT_TOP, drawingModes: [google.maps.drawing.OverlayType.MARKER, google.maps.drawing.OverlayType.CIRCLE, google.maps.drawing.OverlayType.RECTANGLE, google.maps.drawing.OverlayType.POLYGON, google.maps.drawing.OverlayType.POLYLINE] } });
            placerType = "poi";
            break;          
    }
}

//poi object placer handler
function placePOI(type) {
    placerType = "poi";
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
    switch (id) {
        case "item":
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
            break;

        case "overlay":
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
            break;

        case "poi":
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
            break;
    }
}

//save handler
function save(id) {
    switch (id) {
        case "item":
            if (savingMarkerCenter != null) {
                de("saving location: " + savingMarkerCenter); //grab coords from gmaps js
                createSavedItem(savingMarkerCenter);
                //displayMessage(L_Saved); //not used here
            } else {
                displayMessage(L_NotSaved);
            }
            break;

        case "overlay":
            if (savingOverlayIndex.length) {
                for (var i = 0; i < savingOverlayIndex.length; i++) {
                    de("saving overlay: " + savingOverlayIndex[i] + "\nsource: " + savingOverlaySourceURL[i] + "\nbounds: " + savingOverlayBounds[i] + "\nrotation: " + savingOverlayRotation[i]);
                    createSavedOverlay(savingOverlayIndex[i], savingOverlaySourceURL[i], savingOverlayBounds[i], savingOverlayRotation[i]); //send overlay to the server
                    //ghostOverlayRectangle[savingOverlayIndex[i]].setOptions(ghosting); //set rectangle to ghosting
                }
                //displayMessage(L_Saved); //not used here
            } else {
                displayMessage(L_NotSaved);
            }
            break;

        case "poi":
            if (poiObj.length > 0) {
                de("saving " + poiObj.length + " POIs...");
                createSavedPOI();
                //displayMessage(L_Saved); //not used here
            } else {
                displayMessage(L_NotSaved);
            }
            break;
    }
}

//clear handler
function clear(id) {
    switch (id) {
        case "item":
            itemMarker.setMap(null); //delete marker form map
            itemMarker = null; //delete reference to marker
            savingMarkerCenter = null; //reset stored coords to save
            document.getElementById('content_toolbox_posItem').value = ""; //reset lat/long in tab
            document.getElementById('content_toolbox_rgItem').value = ""; //reset address in tab
            displayMessage(L9); //say all is reset
            break;

        case "overlay":
            //does nothing
            displayMessage("Nothing to clear"); //temp
            //displayMessage(L10);
            break;

        case "poi":
            for (var i = 0; i < poiObj.length; i++) {
                if (poiObj[i] != null) {
                    poiObj[i].setMap(null);
                    poiObj[i] = null;
                }
                if (poiDesc[i] != null) {
                    poiDesc[i] = null;
                }
                if (poiKML[i] != null) {
                    poiKML[i] = null;
                }
                infowindow[i].setMap(null);
                infowindow[i] = null;
                label[i].setMap(null);
                label[i] = null;
                var strg = "#poi" + i; //create <li> poi string
                $(strg).remove(); //remove <li>
            }
            poiObj = [];
            poi_i = -1;
            displayMessage(L11);
            break;
    }
}