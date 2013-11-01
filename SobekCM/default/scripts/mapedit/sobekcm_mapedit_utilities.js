//Utilities

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

    //reset the visual rotation value on page load
    $('.knob').val(0).trigger('change');

    //menubar
    de("[WARN]: #mapedit_container_pane_0 background color must be set manually if changed from default.");
    document.getElementById("mapedit_container_pane_0").style.display = "block";

    //var mainCount = 0;
    ////determine ACL placer type
    //if (globalVar.incomingPointCenter.length > 0) {
    //    //determine if any points are "main"
    //    for (var i = 0; i < globalVar.incomingPointCenter.length; i++) {
    //        if (globalVar.incomingPointFeatureType[i] == "main" || globalVar.incomingPointFeatureType[i] == "") {
    //            mainCount++;
    //            //globalVar.incomingACL = "item";
    //            actionsACL("full", "item");
    //        }
    //    }
    //} else {
    //    if (globalVar.incomingPolygonBounds.length > 0) {
    //        for (var j = 0; j < globalVar.incomingPolygonBounds.length; j++) {
    //            if (globalVar.incomingPolygonFeatureType[j] == "main" || globalVar.incomingPolygonFeatureType[j] == "") {
    //                mainCount++;
    //                //globalVar.incomingACL = "overlay";
    //                actionsACL("full", "overlay");
    //            }
    //        }
    //    } else {
    //        //(if no geo detected, open item first [from there you can convert to overlay] this is just a command decision)
    //        actionsACL("full", "item"); 
    //        //actionsACL("full", "overlay");
    //        //actionsACL("full", "poi"); //not yet implemented
    //    }
    //}

    switch (globalVar.incomingACL) {
        case "item":
            actionsACL("full", "item");
            break;
        case "overlay":
            actionsACL("full", "overlay");
            break;
        case "poi":
            actionsACL("full", "poi");
            break;
        case "none":
            actionsACL("full", "actions");
            break;
    }
    de("main count: " + globalVar.mainCount);

    //determine ACL maptype toggle
    if (globalVar.hasCustomMapType == true) {
        actionsACL("full", "customMapType");
    } else {
        actionsACL("none", "customMapType");
    }

    //set window offload fcn to remind to save
    window.onbeforeunload = function (e) {
        if (globalVar.userMayLoseData) {
            var message = L47,
                e = e || window.event;
            // For IE and Firefox
            if (e) {
                e.returnValue = message;
            }
            // For Safari
            return message;
        } else {
            //do nothing
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
            if (globalVar.mapControlsDisplayed == false) { //not present
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
            if (globalVar.toolboxDisplayed == false) { //not present
                document.getElementById("content_menubar_toggleToolbox").className = document.getElementById("content_menubar_toggleToolbox").className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
                document.getElementById("content_toolbar_button_toggleToolbox").className = document.getElementById("content_toolbar_button_toggleToolbox").className.replace(/(?:^|\s)isActive(?!\S)/g, '');
            } else { //present
                document.getElementById("content_menubar_toggleToolbox").className += " isActive2";
                document.getElementById("content_toolbar_button_toggleToolbox").className += " isActive";
            }
            break;
        case "toolbar":
            if (globalVar.toolbarDisplayed == false) { //not present
                document.getElementById("content_menubar_toggleToolbar").className = document.getElementById("content_menubar_toggleToolbar").className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
            } else { //present
                document.getElementById("content_menubar_toggleToolbar").className += " isActive2";
            }
            break;
        case "layer":
            if (globalVar.prevMapLayerActive != null) {
                document.getElementById("content_menubar_layer" + globalVar.prevMapLayerActive).className = document.getElementById("content_menubar_layer" + globalVar.prevMapLayerActive).className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
                document.getElementById("content_toolbar_button_layer" + globalVar.prevMapLayerActive).className = document.getElementById("content_toolbar_button_layer" + globalVar.prevMapLayerActive).className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                document.getElementById("content_toolbox_button_layer" + globalVar.prevMapLayerActive).className = document.getElementById("content_toolbox_button_layer" + globalVar.prevMapLayerActive).className.replace(/(?:^|\s)isActive(?!\S)/g, '');
            }
            document.getElementById("content_menubar_layer" + globalVar.mapLayerActive).className += " isActive2";
            document.getElementById("content_toolbar_button_layer" + globalVar.mapLayerActive).className += " isActive";
            document.getElementById("content_toolbox_button_layer" + globalVar.mapLayerActive).className += " isActive";
            globalVar.prevMapLayerActive = globalVar.mapLayerActive; //set and hold the previous map layer active
            break;
        case "kml":
            if (globalVar.kmlDisplayed == false) { //not present
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
            de("aa: " + globalVar.actionActive + "<br>" + "paa: " + globalVar.prevActionActive);
            if (globalVar.actionActive == "Other") {
                if (globalVar.prevActionActive != null) {
                    document.getElementById("content_menubar_manage" + globalVar.prevActionActive).className = document.getElementById("content_menubar_manage" + globalVar.prevActionActive).className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
                    document.getElementById("content_toolbar_button_manage" + globalVar.prevActionActive).className = document.getElementById("content_toolbar_button_manage" + globalVar.prevActionActive).className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                    document.getElementById("content_toolbox_button_manage" + globalVar.prevActionActive).className = document.getElementById("content_toolbox_button_manage" + globalVar.prevActionActive).className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                }
            } else {
                if (globalVar.prevActionActive != null) {
                    document.getElementById("content_menubar_manage" + globalVar.prevActionActive).className = document.getElementById("content_menubar_manage" + globalVar.prevActionActive).className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
                    document.getElementById("content_toolbar_button_manage" + globalVar.prevActionActive).className = document.getElementById("content_toolbar_button_manage" + globalVar.prevActionActive).className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                    if (document.getElementById("content_toolbox_button_manage" + globalVar.prevActionActive)) {
                        de("found " + globalVar.prevActionActive);
                        document.getElementById("content_toolbox_button_manage" + globalVar.prevActionActive).className = document.getElementById("content_toolbox_button_manage" + globalVar.prevActionActive).className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                    }

                }
                document.getElementById("content_menubar_manage" + globalVar.actionActive).className += " isActive2";
                document.getElementById("content_toolbar_button_manage" + globalVar.actionActive).className += " isActive";
                if (document.getElementById("content_toolbox_button_manage" + globalVar.actionActive)) {
                    de("found " + globalVar.actionActive);
                    document.getElementById("content_toolbox_button_manage" + globalVar.actionActive).className += " isActive";
                }
                globalVar.prevActionActive = globalVar.actionActive; //set and hold the previous map layer active
            }
            break;
            //todo move these into a seperate fcn
            //case "itemPlace":
            //    try {
            //        document.getElementById("content_menubar_itemPlace").className = document.getElementById("content_menubar_itemPlace").className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
            //        document.getElementById("content_toolbox_button_itemPlace").className = document.getElementById("content_toolbox_button_itemPlace").className.replace(/(?:^|\s)isActive(?!\S)/g, '');
            //    }catch (e)
            //    {
            //        document.getElementById("content_menubar_itemPlace").className += " isActive2";
            //        document.getElementById("content_toolbox_button_itemPlace").className += " isActive";
            //    }
            //    break;
            //case "itemPlace2":
            //    if (globalVar.buttonActive_itemPlace == false) { //not present
            //        document.getElementById("content_menubar_itemPlace").className = document.getElementById("content_menubar_itemPlace").className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
            //        document.getElementById("content_toolbox_button_itemPlace").className = document.getElementById("content_toolbox_button_itemPlace").className.replace(/(?:^|\s)isActive(?!\S)/g, '');
            //        globalVar.buttonActive_itemPlace = true;
            //    } else { //present
            //        document.getElementById("content_menubar_itemPlace").className += " isActive2";
            //        document.getElementById("content_toolbox_button_itemPlace").className += " isActive";
            //        globalVar.buttonActive_itemPlace = false;
            //    }
            //    break;
            //case "overlayEdit":
            //    if (globalVar.buttonActive_overlayEdit == false) { //not present
            //        document.getElementById("content_menubar_overlayEdit").className = document.getElementById("content_menubar_overlayEdit").className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
            //        document.getElementById("content_toolbox_button_overlayEdit").className = document.getElementById("content_toolbox_button_overlayEdit").className.replace(/(?:^|\s)isActive(?!\S)/g, '');
            //        globalVar.buttonActive_overlayEdit = true;
            //    } else { //present
            //        document.getElementById("content_menubar_overlayEdit").className += " isActive2";
            //        document.getElementById("content_toolbox_button_overlayEdit").className += " isActive";
            //        globalVar.buttonActive_overlayEdit = false;
            //    }
            //    break;
            //case "overlayPlace":
            //    if (globalVar.buttonActive_overlayPlace == false) { //not present
            //        document.getElementById("content_menubar_overlayPlace").className = document.getElementById("content_menubar_overlayPlace").className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
            //        document.getElementById("content_toolbox_button_overlayPlace").className = document.getElementById("content_toolbox_button_overlayPlace").className.replace(/(?:^|\s)isActive(?!\S)/g, '');
            //        globalVar.buttonActive_overlayPlace = true;
            //    } else { //present
            //        document.getElementById("content_menubar_overlayPlace").className += " isActive2";
            //        document.getElementById("content_toolbox_button_overlayPlace").className += " isActive";
            //        globalVar.buttonActive_overlayPlace = false;
            //    }
            //    break;
        case "overlayToggle":
            if (globalVar.buttonActive_overlayToggle == false) { //not present
                document.getElementById("content_menubar_overlayToggle").className += " isActive2";
                document.getElementById("content_toolbox_button_overlayToggle").className += " isActive";
                globalVar.buttonActive_overlayToggle = true;
            } else { //present
                document.getElementById("content_menubar_overlayToggle").className = document.getElementById("content_menubar_overlayToggle").className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
                document.getElementById("content_toolbox_button_overlayToggle").className = document.getElementById("content_toolbox_button_overlayToggle").className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                globalVar.buttonActive_overlayToggle = false;
            }
            break;
            //case "poiPlace":
            //    if (globalVar.buttonActive_poiPlace == false) { //not present
            //        document.getElementById("content_menubar_overlayPlace").className = document.getElementById("content_menubar_poiPlace").className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
            //        document.getElementById("content_toolbox_button_poiPlace").className = document.getElementById("content_toolbox_button_poiPlace").className.replace(/(?:^|\s)isActive(?!\S)/g, '');
            //        globalVar.buttonActive_poiPlace = true;
            //    } else { //present
            //        document.getElementById("content_menubar_poiPlace").className += " isActive2";
            //        document.getElementById("content_toolbox_button_poiPlace").className += " isActive";
            //        globalVar.buttonActive_poiPlace = false;
            //    }
            //    break;
        case "poiToggle":
            if (globalVar.buttonActive_poiToggle == false) { //not present
                document.getElementById("content_menubar_poiToggle").className += " isActive2";
                document.getElementById("content_toolbox_button_poiToggle").className += " isActive";
                globalVar.buttonActive_poiToggle = true;
            } else { //present
                document.getElementById("content_menubar_poiToggle").className = document.getElementById("content_menubar_poiToggle").className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
                document.getElementById("content_toolbox_button_poiToggle").className = document.getElementById("content_toolbox_button_poiToggle").className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                globalVar.buttonActive_poiToggle = false;
            }
            break;
            //case "poiMarker":
            //    if (globalVar.buttonActive_poiMarker == false) { //not present
            //        document.getElementById("content_menubar_poiMarker").className = document.getElementById("content_menubar_poiMarker").className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
            //        document.getElementById("content_toolbox_button_poiMarker").className = document.getElementById("content_toolbox_button_poiMarker").className.replace(/(?:^|\s)isActive(?!\S)/g, '');
            //        globalVar.buttonActive_poiMarker = true;
            //    } else { //present
            //        document.getElementById("content_menubar_poiMarker").className += " isActive2";
            //        document.getElementById("content_toolbox_button_poiMarker").className += " isActive";
            //        globalVar.buttonActive_poiMarker = false;
            //    }
            //    break;
            //case "poiCircle":
            //    if (globalVar.buttonActive_poiCircle == false) { //not present
            //        document.getElementById("content_menubar_poiCircle").className = document.getElementById("content_menubar_poiCircle").className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
            //        document.getElementById("content_toolbox_button_poiCircle").className = document.getElementById("content_toolbox_button_poiCircle").className.replace(/(?:^|\s)isActive(?!\S)/g, '');
            //        globalVar.buttonActive_poiCircle = true;
            //    } else { //present
            //        document.getElementById("content_menubar_poiCircle").className += " isActive2";
            //        document.getElementById("content_toolbox_button_poiCircle").className += " isActive";
            //        globalVar.buttonActive_poiCircle = true;
            //    }
            //    break;
            //case "poiRectangle":
            //    if (globalVar.buttonActive_poiRectangle == false) { //not present
            //        document.getElementById("content_menubar_poiRectangle").className = document.getElementById("content_menubar_poiRectangle").className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
            //        document.getElementById("content_toolbox_button_poiRectangle").className = document.getElementById("content_toolbox_button_poiRectangle").className.replace(/(?:^|\s)isActive(?!\S)/g, '');
            //        globalVar.buttonActive_poiRectangle = true;
            //    } else { //present
            //        document.getElementById("content_menubar_poiRectangle").className += " isActive2";
            //        document.getElementById("content_toolbox_button_poiRectangle").className += " isActive";
            //        globalVar.buttonActive_poiRectangle = false;
            //    }
            //    break;
            //case "poiPolygon":
            //    if (globalVar.buttonActive_poiPolygon == false) { //not present
            //        document.getElementById("content_menubar_poiPolygon").className = document.getElementById("content_menubar_poiPolygon").className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
            //        document.getElementById("content_toolbox_button_poiPolygon").className = document.getElementById("content_toolbox_button_poiPolygon").className.replace(/(?:^|\s)isActive(?!\S)/g, '');
            //        globalVar.buttonActive_poiRectangle = true;
            //    } else { //present
            //        document.getElementById("content_menubar_poiPolygon").className += " isActive2";
            //        document.getElementById("content_toolbox_button_poiPolygon").className += " isActive";
            //        globalVar.buttonActive_poiRectangle = false;
            //    }
            //    break;
            //case "poiLine":
            //    if (globalVar.buttonActive_poiLine == false) { //not present
            //        document.getElementById("content_menubar_poiLine").className = document.getElementById("content_menubar_poiLine").className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
            //        document.getElementById("content_toolbox_button_poiLine").className = document.getElementById("content_toolbox_button_poiLine").className.replace(/(?:^|\s)isActive(?!\S)/g, '');
            //        globalVar.buttonActive_poiLine = true;
            //    } else { //present
            //        document.getElementById("content_menubar_poiLine").className += " isActive2";
            //        document.getElementById("content_toolbox_button_poiLine").className += " isActive";
            //        globalVar.buttonActive_poiLine = false;
            //    }
            //    break;
    }
    de("buttonAction() completed");
}

//display an inline message
function displayMessage(message) {

    //debug log this message
    de("message #" + globalVar.messageCount + ": " + message); //send to debugger for logging

    //keep a count of messages
    globalVar.messageCount++;

    //check to see if RIB is on
    if (globalVar.RIBMode == true) {
        de("RIB Mode: " + globalVar.RIBMode);
        return;
    } else {
        //display the message

        //debug
        de("RIB Mode: " + globalVar.RIBMode);

        //compile divID
        var currentDivId = "message" + globalVar.messageCount;

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
function createSavedItem(handle, coordinates) {
    var messageType = handle + "|" + "item"; //define what message type it is
    //assign data
    var data = messageType + "|" + coordinates + "|";
    var dataPackage = data + "~";
    de("saving item: " + dataPackage); //temp
    toServer(dataPackage);
}

//create a package to send to server to save overlay
function createSavedOverlay(handle, pageId, label, source, bounds, rotation) {
    var temp = source;
    if (temp.contains("~") || temp.contains("|")) { //check to make sure reserve characters are not there
        displayMessage(L7);
    }
    //var formattedBounds = 
    var messageType = handle + "|" + "overlay"; //define what message type it is
    var data = messageType + "|" + pageId + "|" + label + "|" + bounds + "|" + source + "|" + rotation + "|";
    var dataPackage = data + "~";
    de("saving overlay set: " + dataPackage); //temp
    toServer(dataPackage);
}

//create a package to send to the server to save poi
function createSavedPOI(handle) {
    var dataPackage = "";
    //cycle through all pois
    de("poi length: " + globalVar.poiObj.length);
    for (var i = 0; i < globalVar.poiObj.length; i++) {
        //get specific geometry 
        switch (globalVar.poiType[i]) {
            case "marker":
                globalVar.poiKML[i] = globalVar.poiObj[i].getPosition().toString();
                break;
            case "circle":
                globalVar.poiKML[i] = globalVar.poiObj[i].getCenter() + "|";
                globalVar.poiKML[i] += globalVar.poiObj[i].getRadius();
                break;
            case "rectangle":
                globalVar.poiKML[i] = globalVar.poiObj[i].getBounds().toString();
                break;
            case "polygon":
                globalVar.poiObj[i].getPath().forEach(function (latLng) {
                    globalVar.poiKML[i] += "|";
                    globalVar.poiKML[i] += latLng; //NOTE: this has a non standard coordinate separator    
                });
                break;
            case "polyline":
                globalVar.poiObj[i].getPath().forEach(function (latLng) {
                    globalVar.poiKML[i] += "|";
                    globalVar.poiKML[i] += latLng; //NOTE: this has a non standard coordinate separator    
                });
                break;
            case "deleted":
                //nothing to do here, just a placeholder
                break;
        }
        //filter out the deleted pois
        if (globalVar.poiType[i] != "deleted") {
            //compile data message
            var data = handle + "|" + "poi|" + globalVar.poiType[i] + "|" + globalVar.poiDesc[i] + "|" + globalVar.poiKML[i] + "|";
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

    alert(dataPackage);
    return;

    jQuery('form').each(function () {

        var payload = JSON.stringify({ sendData: dataPackage });
        var hiddenfield = document.getElementById('payload');
        hiddenfield.value = payload;
        var hiddenfield2 = document.getElementById('action');
        hiddenfield2.value = 'save';

        //reset success marker
        globalVar.toServerSuccess = false;
        $.ajax({
            type: "POST",
            async: true,
            url: window.location.href.toString(),
            data: jQuery(this).serialize(),
            success: function (result) {
                //de("server result:" + result);
                displayMessage(L_Saved);
                globalVar.toServerSuccess = true;
                globalVar.csoi = 0; //reset
            }
        });
    });
}

//centeres on an overlay
function overlayCenterOnMe(id) {
    //attempt to pan to center of overlay
    map.panTo(globalVar.ghostOverlayRectangle[id].getBounds().getCenter());
}

//toggles overlay for editing
function overlayEditMe(id) {
    de("editing overlay id: " + id);
    alert("editing... \noverlay id: " + id + "\nwoi: " + globalVar.workingOverlayIndex + "\nsor: " + globalVar.savingOverlayRotation[globalVar.workingOverlayIndex] + "\nipr: " + globalVar.incomingPolygonRotation[globalVar.workingOverlayIndex] + "\npr: " + globalVar.preservedRotation);
    //check to see if overlay is indeed a bonified overlay (on the map)
    try {
        //indicate we are editing
        globalVar.pageMode = "edit";
        //if editing is being done and there is something to save, save
        if (globalVar.currentlyEditing == "yes" && globalVar.workingOverlayIndex != null) {
            de("saving overlay " + globalVar.workingOverlayIndex);
            //trigger a cache of current working overlay
            cacheSaveOverlay(globalVar.workingOverlayIndex);
            //set globalVar.rectangle to globalVar.ghosting
            globalVar.ghostOverlayRectangle[globalVar.workingOverlayIndex].setOptions(globalVar.ghosting);
            //reset editing marker
            globalVar.currentlyEditing = "no";
            //set preserved rotation to the rotation of the current overlay
            //alert("setting preserved rotation to globalVar.savingOverlayRotation[" + globalVar.workingOverlayIndex + "] (" + globalVar.savingOverlayRotation[globalVar.workingOverlayIndex] + ")");
            //globalVar.preservedRotation = globalVar.savingOverlayRotation[globalVar.workingOverlayIndex];
            globalVar.preservedRotation = 0;
        }
        //if editing is not being done, make it so
        if (globalVar.currentlyEditing == "no" || globalVar.workingOverlayIndex == null) {
            globalVar.workingOverlayIndex = id;
            de("editing overlay " + globalVar.workingOverlayIndex);
            //reset preserved rotation value
            globalVar.preservedRotation = 0;
            //set visual rotation knob value
            try {
                if (globalVar.savingOverlayRotation[globalVar.workingOverlayIndex] < 0) {
                    $('.knob').val(((180 + globalVar.savingOverlayRotation[globalVar.workingOverlayIndex]) + 180)).trigger('change');
                    de("setting knob to: " + ((180 + globalVar.savingOverlayRotation[globalVar.workingOverlayIndex]) + 180));
                } else {
                    $('.knob').val(globalVar.savingOverlayRotation[globalVar.workingOverlayIndex]).trigger('change');
                    //alert("setting knob to: globalVar.savingOverlayRotation[" + globalVar.workingOverlayIndex + "] (" + globalVar.savingOverlayRotation[globalVar.workingOverlayIndex] + ")");
                    de("setting knob to: " + globalVar.savingOverlayRotation[globalVar.workingOverlayIndex]);
                }
            } catch (e) {
                de("rotation error catch: " + e);
            }
            //enable editing marker
            globalVar.currentlyEditing = "yes";
            //show ghost
            globalVar.ghostOverlayRectangle[globalVar.workingOverlayIndex].setOptions(globalVar.editable);
            //iterate top z index
            globalVar.currentTopZIndex++;
            //bring overlay to front
            document.getElementById("overlay" + globalVar.workingOverlayIndex).style.zIndex = globalVar.currentTopZIndex;
            //bring ghost to front
            globalVar.ghostOverlayRectangle[globalVar.workingOverlayIndex].setOptions({ zIndex: globalVar.currentTopZIndex });
            //recenter on the overlay
            overlayCenterOnMe(id);
        }
        //indicate to user we are editing a polygon
        displayMessage(L34 + " " + globalVar.incomingPolygonLabel[(id - 1)]);
    } catch (e) {
        //create the overlay
        createOverlayFromPage(id);
    }
}

//hide poi on map
function overlayHideMe(id) {
    try {
        globalVar.overlaysOnMap[id].setMap(null);
        globalVar.ghostOverlayRectangle[id].setMap(null);
        document.getElementById("overlayToggle" + id).innerHTML = "<img src=\"" + globalVar.baseURL + globalVar.baseImageDirURL + "add.png\" onclick=\"overlayShowMe(" + id + ");\" />";
        displayMessage(L31 + " " + globalVar.incomingPolygonLabel[id]);
    } catch (e) {
        displayMessage(localize.L56); //nothing to hide
    }
}

//show poi on map
function overlayShowMe(id) {
    globalVar.overlaysOnMap[id].setMap(map);
    globalVar.ghostOverlayRectangle[id].setMap(map);
    document.getElementById("overlayToggle" + id).innerHTML = "<img src=\"" + globalVar.baseURL + globalVar.baseImageDirURL + "sub.png\" onclick=\"overlayHideMe(" + id + ");\" />";
    displayMessage(L32 + " " + globalVar.incomingPolygonLabel[id]);
}

//delete poi from map and list
function overlayDeleteMe(id) {
    try {
        globalVar.overlaysOnMap[id].setMap(null);
        globalVar.overlaysOnMap[id] = null;
        globalVar.ghostOverlayRectangle[id].setMap(null);
        globalVar.ghostOverlayRectangle[id] = null;
        var strg = "#overlayListItem" + id; //create <li> poi string
        $(strg).remove(); //remove <li>
        globalVar.overlayCount--;
        displayMessage(id + " " + L33);
    } catch (e) {
        displayMessage(localize.L57); //nothing to delete
    }
}

//open the infoWindow of a poi
function poiEditMe(id) {
    globalVar.poiObj[id].setMap(map);
    infoWindow[id].setMap(map);
    label[id].setMap(map);
    //document.getElementById("overlayListItem" + id).style.backgroundColor = "red"; //not implemented yet
    document.getElementById("poiToggle" + id).innerHTML = "<img src=\"" + globalVar.baseURL + globalVar.baseImageDirURL + "sub.png\" onclick=\"poiHideMe(" + id + ");\" />";
}

//hide poi on map
function poiHideMe(id) {
    globalVar.poiObj[id].setMap(null);
    infoWindow[id].setMap(null);
    label[id].setMap(null);
    document.getElementById("poiToggle" + id).innerHTML = "<img src=\"" + globalVar.baseURL + globalVar.baseImageDirURL + "add.png\" onclick=\"poiShowMe(" + id + ");\" />";
}

//show poi on map
function poiShowMe(id) {
    globalVar.poiObj[id].setMap(map);
    infoWindow[id].setMap(map);
    label[id].setMap(map);
    document.getElementById("poiToggle" + id).innerHTML = "<img src=\"" + globalVar.baseURL + globalVar.baseImageDirURL + "sub.png\" onclick=\"poiHideMe(" + id + ");\" />";
}

//delete poi from map and list
function poiDeleteMe(id) {
    globalVar.poiObj[id].setMap(null);
    globalVar.poiObj[id] = null;
    globalVar.poiType[id] = "deleted";
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

            de("poiDesc[id]: " + globalVar.poiDesc[id]);
            de("temp: " + temp);

            //replace the list item title 
            var tempHTMLHolder1 = document.getElementById("poiList").innerHTML.replace(globalVar.poiDesc[id], temp);
            document.getElementById("poiList").innerHTML = tempHTMLHolder1;

            //de("tempHTMLHolder1: " + tempHTMLHolder1);
            de("globalVar.poiDesc[id].substring(0, 20): " + globalVar.poiDesc[id].substring(0, 20));
            de("temp.substring(0, 20): " + temp.substring(0, 20));

            //now replace the list item (order is important)
            var tempHTMLHolder2 = document.getElementById("poiList").innerHTML.replace(">" + globalVar.poiDesc[id].substring(0, 20), ">" + temp.substring(0, 20));
            //now post all this back to the listbox
            document.getElementById("poiList").innerHTML = tempHTMLHolder2;

            de("tempHTMLHolder2: " + tempHTMLHolder2);
            de("label[id]" + label[id]);
            de("temp.substring(0, 20): " + temp.substring(0, 20));

            //replace the object label
            label[id].set("labelContent", temp.substring(0, 20));

            de("globalVar.poiDesc[id]: " + globalVar.poiDesc[id]);
            de("temp: " + temp);

            //assign full description to the poi object
            globalVar.poiDesc[id] = temp;

            //visually set desc
            //infoWindow[id].setOptions({ content: writeHTML("poiDesc", id, "", "") });

            infoWindow[id] = new google.maps.InfoWindow({
                content: writeHTML("poiDescIncoming", id, temp, "")
            });

            //close the poi desc box
            infoWindow[id].setMap(null);
        }
    }
    de("poiGetDesc(" + id + "); finished...");
}

//hide search result from map and list
function searchResultHideMe(id) {
    globalVar.searchResult.setMap(null); //remove from map
    document.getElementById("searchResultToggle" + id).innerHTML = "<img src=\"" + globalVar.baseURL + globalVar.baseImageDirURL + "add.png\" onclick=\"searchResultShowMe(" + id + ");\" />";
}

//show search result on map and list
function searchResultShowMe(id) {
    globalVar.searchResult.setMap(map); //remove from map
    document.getElementById("searchResultToggle" + id).innerHTML = "<img src=\"" + globalVar.baseURL + globalVar.baseImageDirURL + "sub.png\" onclick=\"searchResultHideMe(" + id + ");\" />";
}

//delete search results from map and list
function searchResultDeleteMe(id) {
    //remove visually
    globalVar.searchResult.setMap(null); //remove from map
    $("#searchResultListItem" + id).remove(); //remove the first result div from result list box in toolbox
    document.getElementById("content_toolbar_searchField").value = ""; //clear searchbar
    document.getElementById("content_toolbox_searchField").value = ""; //clear searchbox

    //remove references to 
    globalVar.searchResult = null; //reset search result map item
    globalVar.searchCount = 0; //reset search count
}

//used for lat/long tool
function DisplayCursorCoords(arg) {
    cCoord.innerHTML = arg;
}

//check the zoom level
function checkZoomLevel() {
    var currentZoomLevel = map.getZoom();
    var currentMapType = map.getMapTypeId();
    if (currentZoomLevel == globalVar.maxZoomLevel) {
        displayMessage(L16);
    } else {
        switch (currentMapType) {
            case "roadmap": //roadmap and default
                if (currentZoomLevel == globalVar.minZoomLevel_Roadmap) {
                    displayMessage(L17);
                }
                break;
            case "satellite": //sat
                if (currentZoomLevel == globalVar.minZoomLevel_Satellite) {
                    displayMessage(L17);
                }
                break;
            case "hybrid": //sat
                if (currentZoomLevel == globalVar.minZoomLevel_Satellite) {
                    displayMessage(L17);
                }
                break;
            case "terrain": //terrain only
                if (currentZoomLevel == globalVar.minZoomLevel_Terrain) {
                    displayMessage(L17);
                }
                break;
            case "blocklot":
                if (currentZoomLevel == globalVar.minZoomLevel_BlockLot) {
                    displayMessage(L17);
                }
                break;
        }
        if (globalVar.isCustomOverlay == true) {
            if (currentZoomLevel == globalVar.minZoomLevel_BlockLot) {
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
        value: globalVar.preservedOpacity,
        orientation: "vertical",
        min: 0.00,
        max: 1.00,
        step: 0.01,
        slide: function (event, ui) {
            if (globalVar.currentlyEditing == "yes") {
                //if (globalVar.pageMode == "edit") {
                var selection = $("#overlayTransparencySlider").slider("value");
                de("opacity selected: " + selection);
                keepOpacity(selection);
            }
        }
    });
});

//keeps a specific opacity
function keepOpacity(opacityIn) {
    de("keepOpacity: " + opacityIn);
    var div = document.getElementById("overlay" + globalVar.workingOverlayIndex);
    div.style.opacity = opacityIn;
    globalVar.preservedOpacity = opacityIn;
}

//used to specify a variable opacity (IE adds value to existing)
function opacity(opacityIn) {

    if (globalVar.preservedOpacity <= 1 && globalVar.preservedOpacity >= 0) {
        de("add opacity: " + opacityIn + " to overlay" + globalVar.workingOverlayIndex);
        var div = document.getElementById("overlay" + globalVar.workingOverlayIndex);
        var newOpacity = globalVar.preservedOpacity + opacityIn;
        if (newOpacity > 1) {
            newOpacity = 1;
        }
        if (newOpacity < 0) {
            newOpacity = 0;
        }
        div.style.opacity = newOpacity;
        de("newOpacity: " + newOpacity);
        globalVar.preservedOpacity = newOpacity;
        de("globalVar.preservedOpacity: " + globalVar.preservedOpacity);
        $("#overlayTransparencySlider").slider({ value: globalVar.preservedOpacity });
    } else {
        //could not change the opacity    
    }


}

//jquery rotation knob
$(function ($) {
    $(".knob").knob({
        change: function (value) {
            if (globalVar.currentlyEditing == "yes") {
                //if (globalVar.pageMode == "edit") {
                globalVar.knobRotationValue = value; //assign knob value
                if (value > 180) {
                    globalVar.knobRotationValue = ((globalVar.knobRotationValue - 360) * (1)); //used to correct for visual effect of knob error
                    //globalVar.knobRotationValue = ((globalVar.knobRotationValue-180)*(-1));
                }
                //only do something if we are in pageEdit Mode and there is an overlay to apply these changes to
                if (globalVar.workingOverlayIndex != null) {
                    globalVar.preservedRotation = globalVar.knobRotationValue; //reassign
                    keepRotate(globalVar.preservedRotation); //send to display fcn of rotation
                    de("setting rotation from knob at wroking index: " + globalVar.workingOverlayIndex + "to value: " + globalVar.preservedRotation);
                    globalVar.savingOverlayRotation[globalVar.workingOverlayIndex] = globalVar.preservedRotation; //just make sure it is prepping for save    
                }
            }
        }
    });
});

//used to maintain specific rotation of overlay
function keepRotate(degreeIn) {
    globalVar.currentlyEditing = "yes"; //used to signify we are editing this overlay
    $(function () {
        $("#overlay" + globalVar.workingOverlayIndex).rotate(degreeIn);
        if (degreeIn > 180) {
            $('.knob').val(((degreeIn - 360) * (1))).trigger('change'); //used to correct for visual effect of knob error
        } else {
            $('.knob').val(degreeIn).trigger('change');
        }
    });
}

//used to specify a variable rotation
function rotate(degreeIn) {
    if (globalVar.currentlyEditing == "yes") {
        //if (globalVar.pageMode == "edit") {
        globalVar.currentlyEditing = "yes"; //used to signify we are editing this overlay
        globalVar.degree = globalVar.preservedRotation;
        globalVar.degree += degreeIn;
        if (degreeIn != 0) {
            $(function () {
                $("#overlay" + globalVar.workingOverlayIndex).rotate(globalVar.degree); //assign overlay to defined rotation
            });
        } else { //if rotation is 0, reset rotation
            $(function () {
                globalVar.degree = 0;
                $("#overlay" + globalVar.workingOverlayIndex).rotate(globalVar.degree);
            });
        }
        globalVar.preservedRotation = globalVar.degree; //preserve rotation value
        //globalVar.savingOverlayRotation[globalVar.workingOverlayIndex] = globalVar.preservedRotation; //just make sure it is prepping for save
    }
    if (globalVar.degree > 180) {
        $('.knob').val(((globalVar.degree - 360) * (1))).trigger('change'); //used to correct for visual effect of knob error

    } else {
        $('.knob').val(globalVar.degree).trigger('change');
    }
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
    if (globalVar.strictBounds != null) {
        if (globalVar.strictBounds.contains(map.getCenter())) {
            globalVar.mapInBounds = "yes";
        } else {
            globalVar.mapInBounds = "no";
            map.panTo(globalVar.mapCenter); //recenter
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
            if (globalVar.mapInBounds == "yes") { //if it is inside bounds
                if (globalVar.searchCount > 0) { //check to see if this is the first time searched, if not
                    globalVar.searchResult.setMap(null); //set old search result to not display on map
                } else { //if it is the first time
                    globalVar.searchCount++; //just interate
                }
                globalVar.searchResult = new google.maps.Marker({ //create a new marker
                    map: map, //add to current map
                    position: results[0].geometry.location //set position to search results
                });
                var searchResult_i = 1; //temp, placeholder for later multi search result support
                document.getElementById("searchResults_list").innerHTML = writeHTML("searchResultListItem", searchResult_i, geo, "", "");
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
    de("search result: " + globalVar.searchResult);
    //check to see if there is a search result
    if (globalVar.searchResult != null) {
        //this tells listeners what to do
        globalVar.placerType = "item";
        //assign new position of marker
        globalVar.itemMarker.setPosition(globalVar.searchResult.getPosition());
        //prevent redraw
        globalVar.firstMarker++;
        //delete search result
        globalVar.searchResultDeleteMe();
        //display new marker
        globalVar.itemMarker.setMap(map);
        //get the lat/long of item marker and put it in the item location tab
        document.getElementById('content_toolbox_posItem').value = globalVar.itemMarker.getPosition();
        //get the reverse geo address for item location and put in location tab
        codeLatLng(globalVar.itemMarker.getPosition());
        //store coords to save
        globalVar.savingMarkerCenter = globalVar.itemMarker.getPosition();
        //add listener for new item marker (can only add once the globalVar.itemMarker is created)
        google.maps.event.addListener(globalVar.itemMarker, 'dragend', function () {
            //get lat/long
            document.getElementById('content_toolbox_posItem').value = globalVar.itemMarker.getPosition();
            //get address
            codeLatLng(globalVar.itemMarker.getPosition());
            //store coords to save
            globalVar.savingMarkerCenter = globalVar.itemMarker.getPosition();
        });
    } else {
        //nothing in search
        displayMessage(L39);
    }
}

//used to create an overlay from a page
function createOverlayFromPage(pageId) {
    //assign convertedoverlay index
    de("previous globalVar.convertedOverlayIndex: " + globalVar.convertedOverlayIndex);
    globalVar.convertedOverlayIndex = pageId - 1;
    //select the area to draw the overlay
    displayMessage(L41);
    //define drawing manager
    drawingManager.setOptions({ drawingControl: false, drawingControlOptions: { position: google.maps.ControlPosition.RIGHT_TOP, drawingModes: [google.maps.drawing.OverlayType.RECTANGLE] }, rectangleOptions: { strokeOpacity: 0.2, strokeWeight: 1, fillOpacity: 0.0 } });
    //set drawingmode to rectangle
    drawingManager.setDrawingMode(google.maps.drawing.OverlayType.RECTANGLE);
    //apply the changes
    drawingManager.setMap(map);
    //go ahead and auto switch to editing mode
    globalVar.pageMode = "edit";
    //indicate overlay placer
    globalVar.placerType = "overlay";
    //indicate that we drew this overlay
    globalVar.overlayType = "drawn";
    //assign featuretype
    globalVar.incomingPolygonFeatureType[globalVar.convertedOverlayIndex] = "main";
    //assign polygon type
    globalVar.incomingPolygonPolygonType[globalVar.convertedOverlayIndex] = "rectangle";
    //add the rotation
    globalVar.incomingPolygonRotation[globalVar.convertedOverlayIndex] = 0;
    ////add the working overlay index
    //if (globalVar.workingOverlayIndex == null) {
    //    globalVar.workingOverlayIndex = 0;
    //} else {
    //    globalVar.workingOverlayIndex++;
    //}
}

//used to convert an incoming point to an overlay
function convertToOverlay() {
    //determine index of overlay 
    var totalPolygonCount = 0;
    var nonPoiCount = 0;
    try {
        for (var i = 0; i < globalVar.incomingPolygonFeatureType.length; i++) {
            totalPolygonCount++;
            if (globalVar.incomingPolygonFeatureType[i] != "poi") {
                nonPoiCount++;
            }
        }
        globalVar.convertedOverlayIndex = globalVar.incomingPolygonFeatureType.length - nonPoiCount;
        de("converted overlay index: " + globalVar.convertedOverlayIndex);
    } catch (e) {
        de("no overlays thus pages to convert to.");
    }


    //if (globalVar.itemMarker && globalVar.incomingPointSourceURL[0] != "") {
    if (nonPoiCount > 0) {
        if (globalVar.itemMarker) {
            //hide marker
            globalVar.itemMarker.setMap(null);
            //delete maker todo confirm this deletes
            globalVar.itemMarker = null;
            ////open first overlay to convert
            //createOverlayFromPage(globalVar.convertedOverlayIndex + 1);
        }
        //switch to overlay tab
        actionsACL("none", "item");
        actionsACL("full", "overlay");
        //(confirm 'main' if not already there) fixes a bug 
        globalVar.incomingPolygonFeatureType[globalVar.convertedOverlayIndex] = "main";
        globalVar.incomingPolygonPolygonType[globalVar.convertedOverlayIndex] = "rectangle";
        //explicitly open overlay tab (fixes bug)
        openToolboxTab(3);
        //converted
        displayMessage(L44);
        //explicitly disallow editing after converting
        drawingManager.setDrawingMode(null);
    } else {
        //cannot convert
        displayMessage(L40);
        //explicitly disallow editing after a failed convert
        drawingManager.setDrawingMode(null);
    }
}

//used to display list of overlays in the toolbox container
function initOverlayList() {
    de("initOverlayList(); started...");
    document.getElementById("overlayList").innerHTML = "";
    if (globalVar.incomingPolygonPageId.length > 0) {
        for (var i = 0; i < globalVar.incomingPolygonLabel.length; i++) {
            if (globalVar.incomingPolygonFeatureType[i] != "poi") {
                de("Adding Overlay List Item");
                //if (globalVar.incomingPolygonLabel[i] == "") {
                //    globalVar.incomingPolygonLabel[i] = "Overlay" + (i + 1);
                //}
                //de("label: " + globalVar.incomingPolygonLabel[i] + " at " + i);
                document.getElementById("overlayList").innerHTML += writeHTML("overlayListItem", globalVar.incomingPolygonPageId[i], globalVar.incomingPolygonLabel[i], "");
            }
        }
    }

    //if (globalVar.incomingPolygonPath.length > 0) {
    //    de("There are " + globalVar.incomingPolygonLabel.length + " Incoming Polygons");
    //    for (var i = 0; i < globalVar.incomingPolygonPath.length; i++) {
    //        if (globalVar.incomingPolygonFeatureType[i] != "poi") {
    //            de("Adding Overlay List Item");
    //            //if (globalVar.incomingPolygonLabel[i] == "") {
    //            //    globalVar.incomingPolygonLabel[i] = "Overlay" + (i + 1);
    //            //}
    //            //de("label: " + globalVar.incomingPolygonLabel[i] + " at " + i);
    //            document.getElementById("overlayList").innerHTML += writeHTML("overlayListItem", globalVar.incomingPolygonPageId[i], globalVar.incomingPolygonLabel[i], "");
    //        }
    //    }
    //}
}

//used to set acess control levels for the actions
function actionsACL(level, id) {
    //doesnt work
    //document.getElementById("mapedit_container_toolbar").style.width = "1170px";
    //document.getElementById("mapedit_container_toolbar").style["margin-left"] = "-535px";
    switch (id) {
        case "actions":
            $('#content_menubar_manageOverlay').hide();
            $('#content_toolbar_button_manageOverlay').hide();
            $('#content_toolbox_button_manageOverlay').hide();
            $('#content_toolbox_tab4_header').hide();
            $('#overlayACL').hide();
            $('#content_menubar_manageItem').hide();
            $('#content_toolbar_button_manageItem').hide();
            $('#content_toolbox_button_manageItem').hide();
            $('#content_toolbox_tab3_header').hide();
            $('#itemACL').hide();
            break;
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
            //if (globalVar.incomingPointLabel[param1] == "") {
            //    globalVar.poiDesc[param1] = "New" + param3 + param2;
            //} else {
            //    globalVar.poiDesc[param1] = globalVar.incomingPointLabel[param1];
            //}
            globalVar.poiDesc[param1] = "New" + param3 + param2;
            htmlString = "<div id=\"poi" + param1 + "\" class=\"poiListItem\" title=\"" + globalVar.poiDesc[param1] + " \">" + globalVar.poiDesc[param1] + " <div class=\"poiActionButton\"><a href=\"#\" onclick=\"poiEditMe(" + param1 + ");\"><img src=\"" + globalVar.baseURL + globalVar.baseImageDirURL + "edit.png\"/></a> <a id=\"poiToggle" + param1 + "\" href=\"#\"><img src=\"" + globalVar.baseURL + globalVar.baseImageDirURL + "sub.png\" onclick=\"poiHideMe(" + param1 + ");\" /></a> <a href=\"#\" onclick=\"poiDeleteMe(" + param1 + ");\"><img src=\"" + globalVar.baseURL + globalVar.baseImageDirURL + "delete.png\"/></a></div></div>";
            break;
        case "poiListItemIncoming":
            de("Creating html String");
            globalVar.poiDesc[param1] = param3;
            htmlString = "<div id=\"poi" + param1 + "\" class=\"poiListItem\" title=\"" + param3 + " \">" + param3 + " <div class=\"poiActionButton\"><a href=\"#\" onclick=\"poiEditMe(" + param1 + ");\"><img src=\"" + globalVar.baseURL + globalVar.baseImageDirURL + "edit.png\"/></a> <a id=\"poiToggle" + param1 + "\" href=\"#\"><img src=\"" + globalVar.baseURL + globalVar.baseImageDirURL + "sub.png\" onclick=\"poiHideMe(" + param1 + ");\" /></a> <a href=\"#\" onclick=\"poiDeleteMe(" + param1 + ");\"><img src=\"" + globalVar.baseURL + globalVar.baseImageDirURL + "delete.png\"/></a></div></div>";
            break;
        case "poiDesc":
            de("Creating html String");
            htmlString = "<div class=\"poiDescContainer\"> <textarea id=\"poiDesc" + param1 + "\" class=\"descPOI\" placeholder=\"" + L3 + "\"></textarea> <br/> <div class=\"buttonPOIDesc\" id=\"poiGetDesc\" onClick=\"poiGetDesc(" + param1 + ");\">Save</div> </div>";
            break;
        case "poiDescIncoming":
            de("Creating html String");
            htmlString = "<div class=\"poiDescContainer\"> <textarea id=\"poiDesc" + param1 + "\" class=\"descPOI\">" + param2 + "</textarea> <br/> <div class=\"buttonPOIDesc\" id=\"poiGetDesc\" onClick=\"poiGetDesc(" + param1 + ");\">Save</div> </div>";
            break;
        case "overlayListItem":
            de("Creating html String");
            htmlString = "<div id=\"overlayListItem" + param1 + "\" class=\"overlayListItem\" title=\"" + param2 + "\"> " + param2.substring(0, 20) + " <div class=\"overlayActionButton\"><a href=\"#\" onclick=\"overlayEditMe(" + param1 + ");\"><img src=\"" + globalVar.baseURL + globalVar.baseImageDirURL + "edit.png\"/></a> <a id=\"overlayToggle" + param1 + "\" href=\"#\"><img src=\"" + globalVar.baseURL + globalVar.baseImageDirURL + "sub.png\" onclick=\"overlayHideMe(" + param1 + ");\" /></a> <a href=\"#\" onclick=\"overlayDeleteMe(" + param1 + ");\"><img src=\"" + globalVar.baseURL + globalVar.baseImageDirURL + "delete.png\"/></a></div></div>";
            break;
        case "searchResultListItem":
            de("Creating search html String");
            htmlString = "<div id=\"searchResultListItem" + param1 + "\" class=\"searchResultListItem\" title=\"" + param2 + "\"> " + param2.substring(0, 20) + " <div class=\"searchResultActionButton\"><a id=\"searchResultToggle" + param1 + "\" href=\"#\"><img src=\"" + globalVar.baseURL + globalVar.baseImageDirURL + "sub.png\" onclick=\"searchResultHideMe(" + param1 + ");\" /></a> <a href=\"#\" onclick=\"searchResultDeleteMe(" + param1 + ");\"><img src=\"" + globalVar.baseURL + globalVar.baseImageDirURL + "delete.png\"/></a></div></div>";
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

    //load all toolbar buttons into an array
    //todo make dynamic
    //todo move to onload listener
    var toolbarButtonIds = [];
    toolbarButtonIds[0] = "content_toolbar_button_reset";
    toolbarButtonIds[1] = "content_toolbar_button_toggleMapControls";
    toolbarButtonIds[2] = "content_toolbar_button_toggleToolbox";
    toolbarButtonIds[3] = "content_toolbar_button_layerRoadmap";
    toolbarButtonIds[4] = "content_toolbar_button_layerTerrain";
    toolbarButtonIds[5] = "content_toolbar_button_layerSatellite";
    toolbarButtonIds[6] = "content_toolbar_button_layerHybrid";
    toolbarButtonIds[7] = "content_toolbar_button_layerCustom";
    toolbarButtonIds[8] = "content_toolbar_button_layerReset";
    toolbarButtonIds[9] = "content_toolbar_button_panUp";
    toolbarButtonIds[10] = "content_toolbar_button_panLeft";
    toolbarButtonIds[11] = "content_toolbar_button_panReset";
    toolbarButtonIds[12] = "content_toolbar_button_panRight";
    toolbarButtonIds[13] = "content_toolbar_button_panDown";
    toolbarButtonIds[14] = "content_toolbar_button_zoomIn";
    toolbarButtonIds[15] = "content_toolbar_button_zoomReset";
    toolbarButtonIds[16] = "content_toolbar_button_zoomOut";
    toolbarButtonIds[17] = "content_toolbar_button_manageItem";
    toolbarButtonIds[18] = "content_toolbar_button_manageOverlay";
    toolbarButtonIds[19] = "content_toolbar_button_managePOI";
    toolbarButtonIds[20] = "content_toolbar_button_search";

    if (widthPX < 1100) {
        //
    }

    //if view width < toolbar width
    //todo make 1190 dynamic (cannot simple getelementbyid because toolbar is closed at start)
    if (widthPX < 1100) {
        var temp2 = toolbarButtonIds.length * 45;
        temp2 = widthPX - temp2 - 60;
        temp2 = Math.round(temp2 / 45);
        //var buttonNonVisibleCount = Math.round(((toolbarButtonIds.length * 45) - (widthPX - 60)) / 45);
        var buttonNonVisibleCount = temp2;
        de("non vis button count: " + buttonNonVisibleCount);
        var buttonVisibleCount = toolbarButtonIds.length - buttonNonVisibleCount;
        de("vis button count: " + buttonVisibleCount);
        for (var i = 0; i < buttonVisibleCount; i++) {
            de("showing: " + toolbarButtonIds[i]);
            //document.getElementById(toolbarButtonIds[i]).style.visibility = "show";
            //document.getElementById(toolbarButtonIds[i]).style.display = "block";
        }
        for (var i = buttonVisibleCount; i < buttonNonVisibleCount; i++) {
            de("hiding: " + toolbarButtonIds[i]);
            //document.getElementById(toolbarButtonIds[i]).style.visibility = "hidden";
            //document.getElementById(toolbarButtonIds[i]).style.display = "none";
        }
    }
    //calculate how many buttons can be placed based on width
    //display said buttons with arrow to cycle through

    //detect and handle different widths
    //todo make the 800,250 dynamic
    if (widthPX <= 800) {
        de("tablet viewing width detected...");
        //toolbar
        //menubar
        //toolbox -min
    }
    if (widthPX <= 250) {
        de("phone viewing width detected...");
        //toolbar -convert to bottom button style
        //menubar -convert to sidemenu
        //toolbox -close/disable
    }

    //calculate and set sizes
    var bodyPX = totalPX - headerPX;
    document.getElementById("mapedit_container").style.height = bodyPX + "px";
    var pane0PX = bodyPX * .05;
    //document.getElementById("mapedit_container_pane_0").style.height = pane0PX + "px";
    var pane1PX = bodyPX * .05;
    //document.getElementById("mapedit_container_pane_1").style.height = pane1PX + "px";
    var pane2PX = bodyPX * .90;
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
    if (globalVar.savingOverlayIndex.length > 0) {
        de("reseting cache data");
        globalVar.savingOverlayIndex = [];
        globalVar.savingOverlayPageId = [];
        globalVar.savingOverlayLabel = [];
        globalVar.savingOverlaySourceURL = [];
        globalVar.savingOverlayBounds = [];
        globalVar.savingOverlayRotation = [];
        de("reseting cache save overlay index");
        globalVar.csoi = 0;
        globalVar.userMayLoseData = false;
        de("cache reset");
    } else {
        de("nothing in cache");
    }
    de("reseting working index");
    globalVar.workingOverlayIndex = null;
    de("reseting preserved rotation");
    globalVar.preservedRotation = 0;
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
                    if (globalVar.cCoordsFrozen == "no") {
                        //freeze
                        globalVar.cCoordsFrozen = "yes";
                        displayMessage(L20);
                    } else {
                        //unfreeze
                        globalVar.cCoordsFrozen = "no";
                        displayMessage(L21);
                    }
                }
            }
            break;
        case 79: //O
            if (isCntrlDown == true) {
                if (globalVar.overlaysCurrentlyDisplayed == true) {
                    displayMessage(L22);
                    for (var i = 0; i < globalVar.incomingPolygonPageId.length; i++) { //go through and display overlays as long as there is an overlay to display
                        globalVar.overlaysOnMap[globalVar.incomingPolygonPageId[i]].setMap(null); //hide the overlay from the map
                        globalVar.ghostOverlayRectangle[globalVar.incomingPolygonPageId[i]].setMap(null); //hide ghost from map
                        globalVar.overlaysCurrentlyDisplayed = false; //mark that overlays are not on the map
                    }
                } else {
                    displayMessage(L23);
                    for (var i = 0; i < globalVar.incomingPolygonPageId.length; i++) { //go through and display overlays as long as there is an overlay to display
                        globalVar.overlaysOnMap[globalVar.incomingPolygonPageId[i]].setMap(map); //set the overlay to the map
                        globalVar.ghostOverlayRectangle[globalVar.incomingPolygonPageId[i]].setMap(map); //set to map
                        globalVar.overlaysCurrentlyDisplayed = true; //mark that overlays are on the map
                    }
                }
            }
            break;
        case 68: //D (for debuggin)
            if (isCntrlDown == true) {
                debugs++;
                if (debugs % 2 == 0) {
                    document.getElementById("debugs").style.display = "none";
                    globalVar.debugMode = false;
                    isCntrlDown = false;
                    displayMessage("Debug Mode Off");
                } else {
                    document.getElementById("debugs").style.display = "block";
                    globalVar.debugMode = true;
                    displayMessage("Debug Mode On");
                    isCntrlDown = false;
                }
            }
            break;
    }
}

//debugging 
var debugStringBase = "<strong>Debug Panel:</strong> <a onclick=\"debugClear()\">(clear)</a><br><br>"; //starting debug string
var debugString; //holds debug messages
var debugs = 0; //used for keycode debugging
function de(message) {
    //create debug string
    var currentdate = new Date();
    var time = currentdate.getHours() + ":" + currentdate.getMinutes() + ":" + currentdate.getSeconds() + ":" + currentdate.getMilliseconds();
    var newDebugString = "[" + time + "] " + message + "<br><hr>";
    newDebugString += debugString;
    document.getElementById("debugs").innerHTML = debugStringBase + newDebugString;
    debugString = newDebugString;
}
function debugClear() {
    debugString = ""; //clear debug string
    document.getElementById("debugs").innerHTML = debugStringBase;
}

//#endregion