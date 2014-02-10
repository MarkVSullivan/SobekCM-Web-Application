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
                    if (globalVar.mapInBounds == "yes") {
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
                        de("saving overlay: (" + i + ") " + globalVar.savingOverlayPageId[i] + "\nlabel: " + globalVar.savingOverlayLabel[i] + "\nsource: " + globalVar.savingOverlaySourceURL[i] + "\nbounds: " + globalVar.savingOverlayBounds[i] + "\nrotation: " + globalVar.savingOverlayRotation[i]);
                        createSavedOverlay("save", globalVar.savingOverlayPageId[i], globalVar.savingOverlayLabel[i], globalVar.savingOverlaySourceURL[i], globalVar.savingOverlayBounds[i], globalVar.savingOverlayRotation[i]); //send overlay to the server
                        if (globalVar.toServerSuccess == true) {
                            displayMessage(L_Saved);
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