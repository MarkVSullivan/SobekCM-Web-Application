//listener actions

//#region Listener Actions

//reset handler
function resetAll() {
    //warn the user
    var consent = confirmMessage(L47);
    if (consent == true) {
        displayMessage(L48);
        document.location.reload(true); //refresh page
    } else {
        displayMessage(L49);
    }
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
            globalVars.actionActive = "Other";
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
            if (globalVars.workingOverlayIndex != null) {
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
            de("attempting to clear " + globalVars.poiObj.length + "POIs...");
            if (globalVars.poiObj.length > 0) {
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

