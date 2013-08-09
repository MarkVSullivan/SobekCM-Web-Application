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
            
        case "overlays":
            if (overlaysOnMap.length) {
                if (overlaysCurrentlyDisplayed == true) {
                    displayMessage(L22);
                    for (var i = 0; i < incomingOverlayBounds.length; i++) { //go through and display overlays as long as there is an overlay to display
                        overlaysOnMap[i].setMap(null); //hide the overlay from the map
                        ghostOverlayRectangle[i].setMap(null); //hide ghost from map
                        overlaysCurrentlyDisplayed = false; //mark that overlays are not on the map
                    }
                } else {
                    displayMessage(L23);
                    for (var i = 0; i < incomingOverlayBounds.length; i++) { //go through and display overlays as long as there is an overlay to display
                        overlaysOnMap[i].setMap(map); //set the overlay to the map
                        ghostOverlayRectangle[i].setMap(map); //set to map
                        overlaysCurrentlyDisplayed = true; //mark that overlays are on the map
                    }
                }
            } else {
                //nothing to toggle
                displayMessage(L45);
            }
            break;

        case "pois":
            if (poiCount) {
                for (var i = 0; i < poiCount; i++) {
                    if (poiToggleState == "displayed") {
                        poiHideMe(i);
                        poiToggleState = "hidden";
                        displayMessage(L42);
                    } else {
                        poiShowMe(i);
                        poiToggleState = "displayed";
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
            if (incomingOverlayBounds.length > 0) {
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
                pageMode = "edit";
            }

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
            //is this the first time saving a changed item?
            if (firstSaveItem == true) {
                //determine if there is something to save
                if (savingMarkerCenter != null) {
                    de("saving location: " + savingMarkerCenter);
                    //save to temp xml file
                    createSavedItem(savingMarkerCenter);
                    //reset first save
                    firstSaveItem = false;
                    //change save button to apply button
                    document.getElementById("content_toolbox_button_saveItem").value = L36;
                    //change save title to apply
                    document.getElementById("content_toolbox_button_saveItem").title = L35;
                } else {
                    displayMessage(L_NotSaved);
                }
            } else {
                //apply the changes
                de("Applying Changes...");
                //currently doesnt do anything

                //reset apply button to save
                document.getElementById("content_toolbox_button_saveItem").value = L37;
                document.getElementById("content_toolbox_button_saveitem").title = L38;
            }
            break;

        case "overlay":
            //is this the first time saving a changed item?
            de("first save overlay? " + firstSaveOverlay);
            if (firstSaveOverlay == true) {
                //determine if there is something to save
                de("overlay length? " + savingOverlayIndex.length);
                if (savingOverlayIndex.length) {
                    for (var i = 0; i < savingOverlayIndex.length; i++) {
                        //save to temp xml file
                        de("saving overlay: " + savingOverlayLabel[i] + "\nsource: " + savingOverlaySourceURL[i] + "\nbounds: " + savingOverlayBounds[i] + "\nrotation: " + savingOverlayRotation[i]);
                        createSavedOverlay(savingOverlayLabel[i], savingOverlaySourceURL[i], savingOverlayBounds[i], savingOverlayRotation[i]); //send overlay to the server
                    }
                    //reset first save
                    firstSaveOverlay = false;
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
                if (savingOverlayIndex.length) {
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
            //save to temp xml file
            if (poiObj.length > 0) {
                de("saving " + poiObj.length + " POIs...");
                createSavedPOI();
                //displayMessage(L_Saved); //not used here
            } else {
                displayMessage(L_NotSaved);
            }
            
            //is this the first time saving a changed item?
            if (firstSavePOI == true) {
                //determine if there is something to save
                if (poiObj.length > 0) {
                    //save to temp xml file
                    de("saving " + poiObj.length + " POIs...");
                    createSavedPOI();
                    //reset first save
                    firstSavePOI = false;
                    //change save button to apply button
                    document.getElementById("content_toolbox_button_savePOI").value = L36;
                    //change save title to apply
                    document.getElementById("content_toolbox_button_savePOI").title = L35;
                } else {
                    //tell that we did not save anything
                    displayMessage(L_NotSaved);
                }
            } else {
                //is there something to save?
                if (poiObj.length > 0) {
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
            break;
    }
}

//clear handler
function clear(id) {
    switch (id) {
        case "item":
            //clear the current marker
            itemMarker.setMap(null); //delete marker form map
            itemMarker = null; //delete reference to marker
            savingMarkerCenter = null; //reset stored coords to save
            document.getElementById('content_toolbox_posItem').value = ""; //reset lat/long in tab
            document.getElementById('content_toolbox_rgItem').value = ""; //reset address in tab
            //redraw incoming marker
            displayIncomingPoints();
            displayMessage(L9); //say all is reset
            break;

        case "overlay":
            if (workingOverlayIndex != null) {
                //delete all incoming overlays
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