//utilities

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
        //check to see if we are in debug mode
        if (globalVars.debugMode != true) {
            var message = L47,
                e = e || window.event;
            // For IE and Firefox
            if (e) {
                e.returnValue = message;
            }
            // For Safari
            return message;
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
                    document.getElementById("content_toolbox_button_manage" + globalVars.prevActionActive).className = document.getElementById("content_toolbox_button_manage" + globalVars.prevActionActive).className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                }
                document.getElementById("content_menubar_manage" + globalVars.actionActive).className += " isActive2";
                document.getElementById("content_toolbar_button_manage" + globalVars.actionActive).className += " isActive";
                document.getElementById("content_toolbox_button_manage" + globalVars.actionActive).className += " isActive";
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
    globalVars.ghostOverlayRectangle[id].setMap(null);
    var strg = "#overlayListItem" + id; //create <li> poi string
    $(strg).remove(); //remove <li>
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
        $("#overlayTransparencySlider").slider({value:globalVars.preservedOpacity});
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
        globalVars.incomingOverlayLabel[0] = globalVars.incomingPointLabel[0];
        globalVars.incomingOverlaySourceURL[0] = globalVars.incomingPointSourceURL[0];
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

    //detect and handle different widths
    if (widthPX <= 800) {
        de("tablet viewing width detected...");
        //toolbar
        //menubar
        //toolbox -min
    }
    if (widthPX <= 250) {
        de("phone viewing width detected...");
        //toolbar
        //menubar
        //toolbox -close
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

