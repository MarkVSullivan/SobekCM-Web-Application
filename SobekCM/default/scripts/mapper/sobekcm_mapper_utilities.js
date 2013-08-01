﻿//utilities

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
}

//open a specific tab
function openToolboxTab(id) {
    ////START WORKAROUND
    //if (id == 1) {
    //    setTimeout(function () { document.getElementById("content_toolbox_searchButton").style.display = "block"; }, 100);
    //} else {
    //    document.getElementById("content_toolbox_searchButton").style.display = "none";
    //}
    ////END WORKAROUND
    $("#mapper_container_toolboxTabs").accordion({ active: id });
}

//facilitates button sticky effect
function buttonActive(id) {
    switch (id) {
        case "mapControls":
            if (mapControlsDisplayed == false) { //not present
                document.getElementById("content_toolbar_button_toggleMapControls").className = document.getElementById("content_toolbar_button_toggleMapControls").className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                document.getElementById("content_toolbox_button_toggleMapControls").className = document.getElementById("content_toolbox_button_toggleMapControls").className.replace(/(?:^|\s)isActive(?!\S)/g, '');
            } else { //present
                document.getElementById("content_toolbar_button_toggleMapControls").className += " isActive";
                document.getElementById("content_toolbox_button_toggleMapControls").className += " isActive";
            }
            break;
        case "toolbox":
            if (toolboxDisplayed == false) { //not present
                document.getElementById("content_toolbar_button_toggleToolbox").className = document.getElementById("content_toolbar_button_toggleToolbox").className.replace(/(?:^|\s)isActive(?!\S)/g, '');
            } else { //present
                document.getElementById("content_toolbar_button_toggleToolbox").className += " isActive";
            }
            break;
        case "layer":
            if (prevMapLayerActive != null) {
                document.getElementById("content_toolbar_button_layer" + prevMapLayerActive).className = document.getElementById("content_toolbar_button_layer" + prevMapLayerActive).className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                document.getElementById("content_toolbox_button_layer" + prevMapLayerActive).className = document.getElementById("content_toolbox_button_layer" + prevMapLayerActive).className.replace(/(?:^|\s)isActive(?!\S)/g, '');
            }
            document.getElementById("content_toolbar_button_layer" + mapLayerActive).className += " isActive";
            document.getElementById("content_toolbox_button_layer" + mapLayerActive).className += " isActive";
            prevMapLayerActive = mapLayerActive; //set and hold the previous map layer active
            break;
        case "kml":
            if (kmlDisplayed == false) { //not present
                document.getElementById("content_toolbar_button_layerCustom").className = document.getElementById("content_toolbar_button_layerCustom").className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                document.getElementById("content_toolbox_button_layerCustom").className = document.getElementById("content_toolbox_button_layerCustom").className.replace(/(?:^|\s)isActive(?!\S)/g, '');
            } else { //present
                document.getElementById("content_toolbar_button_layerCustom").className += " isActive";
                document.getElementById("content_toolbox_button_layerCustom").className += " isActive";
            }
            break;
        case "action":
            de("aa: " + actionActive + "<br>" + "paa: " + prevActionActive);
            if (actionActive == "Other") {
                document.getElementById("content_toolbar_button_manage" + prevActionActive).className = document.getElementById("content_toolbar_button_manage" + prevActionActive).className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                document.getElementById("content_toolbox_button_manage" + prevActionActive).className = document.getElementById("content_toolbox_button_manage" + prevActionActive).className.replace(/(?:^|\s)isActive(?!\S)/g, '');
            } else {
                if (prevActionActive != null) {
                    document.getElementById("content_toolbar_button_manage" + prevActionActive).className = document.getElementById("content_toolbar_button_manage" + prevActionActive).className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                    document.getElementById("content_toolbox_button_manage" + prevActionActive).className = document.getElementById("content_toolbox_button_manage" + prevActionActive).className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                }
                document.getElementById("content_toolbar_button_manage" + actionActive).className += " isActive";
                document.getElementById("content_toolbox_button_manage" + actionActive).className += " isActive";
                prevActionActive = actionActive; //set and hold the previous map layer active
            }
            break;
    }
}

//display an inline message
function displayMessage(message) {

    de(message); //send to debugger for logging

    //create message
    //var messageText = "<p class=\"message\">";
    //messageText += message; //assign incoming message to text
    //messageText += "</p>";
    document.getElementById("content_message").innerHTML = message; //assign to element

    //show message
    document.getElementById("mapper_container_message").style.display = "block"; //display element

    //fade message out
    setTimeout(function () {
        $("#mapper_container_message").fadeOut("slow", function () {
            $("#mapper_container_message").hide();
        });
    }, 3000); //after 3 sec
}

//create a package to send to server to save item location
function createSavedItem(coordinates) {
    var messageType = "item"; //define what message type it is
    //assign data
    var data = messageType + "|" + coordinates + "|";
    var dataPackage = data + "~";
    de("saving item: " + dataPackage); //temp
    //document.getElementById("saveTest").value = dataPackage; //not used
    //document.location.reload(); //refresh page //not used
    //CallServer(dataPackage); //not used (for callbacks)
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
    var dataPackage = "";
    //cycle through all pois
    for (var i = 0; i < poiObj.length; i++) {
        //get specific geometry 
        switch (poiType[i]) {
            case "marker":
                poiKML[i] = poiObj[i].getPosition().toString();
                break;
            case "circle":
                poiKML[i] = poiObj[i].getCenter() + "|";
                poiKML[i] += poiObj[i].getRadius();
                break;
            case "rectangle":
                poiKML[i] = poiObj[i].getBounds().toString();
                break;
            case "polygon":
                poiObj[i].getPath().forEach(function (latLng) {
                    poiKML[i] += "|";
                    poiKML[i] += latLng; //NOTE: this has a non standard coordinate separator    
                });
                break;
            case "polyline":
                poiObj[i].getPath().forEach(function (latLng) {
                    poiKML[i] += "|";
                    poiKML[i] += latLng; //NOTE: this has a non standard coordinate separator    
                });
                break;
            case "deleted":
                //nothing to do here
                break;
        }
        //compile message
        var data = "poi|" + poiType[i] + "|" + poiDesc[i] + "|" + poiKML[i] + "|";
        dataPackage += data + "~";
    }
    de("saving overlay set: " + dataPackage); //temp  
    toServer(dataPackage); //send to server to save
}

//sends save dataPackages to the server via json
function toServer(dataPackage) {
    var scriptURL = "default/scripts/serverside/Scripts.aspx";
    $.ajax({
        type: "POST",
        url: baseURL + scriptURL + "/SaveItem",
        data: JSON.stringify({ sendData: dataPackage }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            de("server result:" + result);
            displayMessage("Saved");
        }
    });
}

//toggles overlay for editing
function overlayEditMe(id) {
    var ghostIndex = id;
    pageMode = "edit";
    if (currentlyEditing == "yes") {                                                            //if editing is being done, save
        cacheSaveOverlay(ghostIndex);                                                           //trigger a cache of current working overlay
        ghostOverlayRectangle[workingOverlayIndex].setOptions(ghosting);                        //set rectangle to ghosting
        currentlyEditing = "no";                                                                //reset editing marker
        preservedRotation = 0;                                                                  //reset preserved rotation
    }
    if (currentlyEditing == "no") {
        currentlyEditing = "yes"; //enable editing marker
        workingOverlayIndex = ghostIndex; //set this overay as the one being e
        ghostOverlayRectangle[ghostIndex].setOptions(editable); //show ghost
        currentTopZindex++; //iterate top z index
        document.getElementById("overlay" + ghostIndex).style.zIndex = currentTopZindex; //bring overlay to front
        ghostOverlayRectangle[ghostIndex].setOptions({ zIndex: currentTopZindex }); //bring ghost to front
        for (var i = 0; i < savingOverlayIndex.length; i++) { //set rotation if the overlay was previously saved
            if (ghostIndex == savingOverlayIndex[i]) {
                preservedRotation = savingOverlayRotation[i];
            }
        }
        //panMap(ghostOverlayRectangle[ghostIndex].bounds.getCenter()); //attempt to pan to center of overlay
    }
    displayMessage("Editing Overlay " + id);
}

//hide poi on map
function overlayHideMe(id) {
    overlaysOnMap[id].setMap(null);
    ghostOverlayRectangle[id].setMap(null);
    document.getElementById("overlayToggle" + id).innerHTML = "<img src=\"" + baseURL + baseImagesDirURL + "add.png\" onclick=\"overlayShowMe(" + id + ");\" />";
    displayMessage("Hiding Overlay " + id);
}

//show poi on map
function overlayShowMe(id) {
    overlaysOnMap[id].setMap(map);
    ghostOverlayRectangle[id].setMap(map);
    document.getElementById("overlayToggle" + id).innerHTML = "<img src=\"" + baseURL + baseImagesDirURL + "sub.png\" onclick=\"overlayHideMe(" + id + ");\" />";
    displayMessage("Showing Overlay " + id);
}

//delete poi from map and list
function overlayDeleteMe(id) {
    overlaysOnMap[id].setMap(null);
    ghostOverlayRectangle[id].setMap(null);
    var strg = "#overlay" + id; //create <li> poi string
    $(strg).remove(); //remove <li>
    displayMessage("Overlay " + id + " Removed");
}

//open the infowindow of a poi
function poiEditMe(id) {
    poiObj[id].setMap(map);
    if (poiType[id] == "marker") {
        infowindow[id].open(map, poiObj[id]);
    } else {
        infowindow[id].setMap(map);
    }
}

//hide poi on map
function poiHideMe(id) {
    poiObj[id].setMap(null);
    infowindow[id].setMap(null);
    label[id].setMap(null);
    document.getElementById("poiToggle" + id).innerHTML = "<img src=\"" + baseURL + baseImagesDirURL + "add.png\" onclick=\"poiShowMe(" + id + ");\" />";
}

//show poi on map
function poiShowMe(id) {
    poiObj[id].setMap(map);
    infowindow[id].setMap(map);
    label[id].setMap(map);
    document.getElementById("poiToggle" + id).innerHTML = "<img src=\"" + baseURL + baseImagesDirURL + "sub.png\" onclick=\"poiHideMe(" + id + ");\" />";
}

//delete poi from map and list
function poiDeleteMe(id) {
    poiObj[id].setMap(null);
    poiObj[id] = null;
    poiType[id] = "deleted";
    var strg = "#poi" + id; //create <li> poi string
    $(strg).remove(); //remove <li>
    infowindow[id].setMap(null);
    label[id].setMap(null);
}

//get the poi desc
function poiGetDesc(id) {

    var temp = document.getElementById("poiDesc" + id).value;
    if (temp.contains("~") || temp.contains("|")) {
        displayMessage(L8);
    } else {
        
        //replace the list item title 
        var tempHTMLHolder1 = document.getElementById("poiList").innerHTML.replace(poiDesc[id], document.getElementById("poiDesc" + id).value);
        document.getElementById("poiList").innerHTML = tempHTMLHolder1;
        //now replace the title (order is important)
        var tempHTMLHolder2 = document.getElementById("poiList").innerHTML.replace(poiDesc[id].substring(0, 20), document.getElementById("poiDesc" + id).value.substring(0, 20));
        //now post all this back to the listbox
        document.getElementById("poiList").innerHTML = tempHTMLHolder2;
        
        //replace the obeject label
        label[id].set(content,document.getElementById("poiDesc" + id).value.substring(0, 20));
       

        //assign full description to the poi object
        poiDesc[id] = document.getElementById("poiDesc" + id).value;
        
        //close the poi desc box

    }
}                       

//delete search results from map and list
function searchResultDeleteMe() {
    //remove visually
    searchResult.setMap(null); //remove from map
    $("#searchResult_1").remove(); //remove the first result div from result list box in toolbox
    document.getElementById("content_toolbar_searchField").value = ""; //clear searchbar
    document.getElementById("content_toolbox_searchField").value = ""; //clear searchbox

    //remove references to 
    searchResult = null; //reset search result map item
    searchCount = 0; //reset search count
}


//used for lat/long tool
function DisplayCursorCoords(arg) {
    cCoord.innerHTML = arg;
}             

//check the zoom level
function checkZoomLevel() {
    var currentZoomLevel = map.getZoom();
    var currentMapType = map.getMapTypeId();
    if (currentZoomLevel == maxZoomLevel) {
        displayMessage(L16);
    } else {
        switch (currentMapType) {
            case "roadmap": //roadmap and default
                if (currentZoomLevel == minZoomLevel_Roadmap) {
                    displayMessage(L17);
                }
                break;
            case "satellite": //sat
                if (currentZoomLevel == minZoomLevel_Satellite) {
                    displayMessage(L17);
                }
                break;
            case "hybrid": //sat
                if (currentZoomLevel == minZoomLevel_Satellite) {
                    displayMessage(L17);
                }
                break;
            case "terrain": //terrain only
                if (currentZoomLevel == minZoomLevel_Terrain) {
                    displayMessage(L17);
                }
                break;
        }
        if (isCustomOverlay == true) {
            if (currentZoomLevel == minZoomLevel_BlockLot) {
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
        value: preserveOpacity,
        orientation: "vertical",
        min: 0.00,
        max: 1.00,
        step: 0.01,
        slide: function (event, ui) {
            var selection = $("#overlayTransparencySlider").slider("value");
            var div = document.getElementById("overlay" + workingOverlayIndex);
            div.style.opacity = selection;
            preserveOpacity = selection;
        }
    });
});

//jquery rotation knob
$(function ($) {
    $(".knob").knob({
        change: function (value) {
            knobRotationValue = value; //assign knob value
            if (value > 180) {
                knobRotationValue = ((knobRotationValue - 360) * (1)); //used to correct for visual effect of knob error
                //knobRotationValue = ((knobRotationValue-180)*(-1));
            }
            preservedRotation = knobRotationValue; //reassign
            keepRotate(preservedRotation); //send to display fcn of rotation

        }
    });

});

//used to maintain specific rotation of overlay
function keepRotate(degreeIn) {
    currentlyEditing = "yes"; //used to signify we are editing this overlay
    $(function () {
        $("#overlay" + workingOverlayIndex).rotate(degreeIn);
    });
}

//used to specify a variable rotation
function rotate(degreeIn) {
    currentlyEditing = "yes"; //used to signify we are editing this overlay
    degree = preservedRotation;
    degree += degreeIn;
    if (degreeIn != 0) {
        $(function () {
            $("#overlay" + workingOverlayIndex).rotate(degree); //assign overlay to defined rotation
        });
    } else { //if rotation is 0, reset rotation
        $(function () {
            degree = 0;
            $("#overlay" + workingOverlayIndex).rotate(degree);
        });
    }
    preservedRotation = degree; //preserve rotation value
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
    if (strictBounds != null) {
        if (strictBounds.contains(map.getCenter())) {
            mapInBounds = "yes";
        } else {
            mapInBounds = "no";
            map.panTo(mapCenter); //recenter
            displayMessage(L5);
        }
    }
}

//search the gmaps for a location (lat/long or address)
function finder(stuff) {
    if (stuff.length > 0) {
        codeAddress("lookup", stuff); //find the thing
        document.getElementById("content_toolbar_searchField").value = stuff; //sync toolbar
        document.getElementById("content_toolbox_searchField").value = stuff; //sync toolbox
        openToolboxTab(1); //open the actions tab
    } else {
        //do nothing and keep quiet
    }
}

//placeholder to search the collection [currently not used]
function searcher(stuff) {

    if (stuff.length > 0) {
        displayMessage("No collection to search"); //temp
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
            if (mapInBounds == "yes") { //if it is inside bounds
                if (searchCount > 0) { //check to see if this is the first time searched, if not
                    searchResult.setMap(null); //set old search result to not display on map
                } else { //if it is the first time
                    searchCount++; //just interate
                }
                searchResult = new google.maps.Marker({ //create a new marker
                    map: map, //add to current map
                    position: results[0].geometry.location //set position to search results
                });
                var searchResult_i = 1; //temp, placeholder for later multi search result support
                document.getElementById("searchResults_list").innerHTML = "<div id=\"searchResult_" + searchResult_i + "\" class=\"searchResults_listItem\">" + geo + " <div class=\"searchResults_actionButton\"> <a href=\"#\" onclick=\"searchResultDeleteMe();\"><img src=\"" + baseURL + baseImagesDirURL + "delete.png\"/></a></div></div><br\>"; //add list div
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
    placerType = "item";                        //this tells listeners what to do
    itemMarker = new google.maps.Marker({
        map: map,
        position: searchResult.getPosition(),   //assign to prev search result location
        draggable: true
    });

    firstMarker++;                              //prevent redraw
    
    searchResultDeleteMe(); //delete search result
    
    itemMarker.setMap(map);                     //set itemMarker location icon to map
    document.getElementById('content_toolbox_posItem').value = itemMarker.getPosition(); //get the lat/long of item marker and put it in the item location tab
    codeLatLng(itemMarker.getPosition());       //get the reverse geo address for item location and put in location tab
    savingMarkerCenter = itemMarker.getPosition(); //store coords to save

    //add listener for new item marker (can only add once the itemMarker is created)
    google.maps.event.addListener(itemMarker, 'dragend', function () {
        document.getElementById('content_toolbox_posItem').value = itemMarker.getPosition(); //get lat/long
        codeLatLng(itemMarker.getPosition());   //get address
        savingMarkerCenter = itemMarker.getPosition(); //store coords to save
    });

}

//used to display list of overlays in the toolbox container
function initOverlayList() {
    de("initOverlayList(); started...");
    document.getElementById("overlayList").innerHTML = "";
    if (incomingOverlayLabel.length > 0) {
        de("There are " + incomingOverlayLabel.length + " Incoming Overlays");
        for (var i = 0; i < incomingOverlayLabel.length; i++) {
            de("Adding Overlay List Item");
            document.getElementById("overlayList").innerHTML += writeHTML("overlayListItem", i, incomingOverlayLabel[i], "");
        }  
    }   
}

//used to write html content to page via js
function writeHTML(type, param1, param2, param3) {
    var htmlString = "";
    switch (type) {
        case "poiListItem":
            de("Creating html String");
            htmlString = "<div id=\"poi" + param1 + "\" class=\"poiListItem\" title=\" New" + param3 + param2 + " \"> New" + param3 + param2 + " <div class=\"poiActionButton\"><a href=\"#\" onclick=\"poiEditMe(" + param1 + ");\"><img src=\"" + baseURL + baseImagesDirURL + "edit.png\"/></a> <a id=\"poiToggle" + param1 + "\" href=\"#\"><img src=\"" + baseURL + baseImagesDirURL + "sub.png\" onclick=\"poiHideMe(" + param1 + ");\" /></a> <a href=\"#\" onclick=\"poiDeleteMe(" + param1 + ");\"><img src=\"" + baseURL + baseImagesDirURL + "delete.png\"/></a></div></div>";
            poiDesc[param1] = "New" + param3 + param2;
            break;
        case "poiDesc":
            de("Creating html String");
            htmlString = "<div class=\"poiDescContainer\"> <textarea id=\"poiDesc" + param1 + "\" class=\"descPOI\" placeholder=\"" + L3 + "\"></textarea> <br/> <div class=\"buttonPOIDesc\" id=\"poiGetDesc\" onClick=\"poiGetDesc(" + param1 + ");\">Save</div> </div>";
            break;
        case "overlayListItem":
            de("Creating html String");
            htmlString = "<div id=\"overlayListItem" + param1 + "\" class=\"overlayListItem\" title=\"" + param2 + "\"> " + param2.substring(0, 20) + " <div class=\"overlayActionButton\"><a href=\"#\" onclick=\"overlayEditMe(" + param1 + ");\"><img src=\"" + baseURL + baseImagesDirURL + "edit.png\"/></a> <a id=\"overlayToggle" + param1 + "\" href=\"#\"><img src=\"" + baseURL + baseImagesDirURL + "sub.png\" onclick=\"overlayHideMe(" + param1 + ");\" /></a> <a href=\"#\" onclick=\"overlayDeleteMe(" + param1 + ");\"><img src=\"" + baseURL + baseImagesDirURL + "delete.png\"/></a></div></div>";
            break;
    }
    return htmlString;
}

//keypress shortcuts/actions
window.onkeypress = keypress;
function keypress(e) {
    var keycode = null;
    if (window.event) {
        keycode = window.event.keyCode;
    } else if (e) {
        keycode = e.which;
    }
    de("keycode: " + keycode);
    switch (keycode) {
        case 70: //F
            if (navigator.appName == "Microsoft Internet Explorer") {
                var copyString = cLat.innerHTML;
                copyString += ", " + cLong.innerHTML;
                window.clipboardData.setData("Text", copyString);
                displayMessage(L19);
            } else {
                if (cCoordsFrozen == "no") {
                    //freeze
                    cCoordsFrozen = "yes";
                    displayMessage(L20);
                } else {
                    //unfreeze
                    cCoordsFrozen = "no";
                    displayMessage(L21);
                }
            }
            break;
        case 79: //O
            if (overlaysCurrentlyDisplayed == true) {
                displayMessage(L22);
                for (var i = 0; i < incomingOverlayBounds.length; i++) {    //go through and display overlays as long as there is an overlay to display
                    overlaysOnMap[i].setMap(null);                          //hide the overlay from the map
                    ghostOverlayRectangle[i].setMap(null);                  //hide ghost from map
                    overlaysCurrentlyDisplayed = false;                     //mark that overlays are not on the map
                }
            } else {
                displayMessage(L23);
                for (var i = 0; i < incomingOverlayBounds.length; i++) {   //go through and display overlays as long as there is an overlay to display
                    overlaysOnMap[i].setMap(map);                          //set the overlay to the map
                    ghostOverlayRectangle[i].setMap(map);                  //set to map
                    overlaysCurrentlyDisplayed = true;                     //mark that overlays are on the map
                }
            }

            break;
        case 13: //enter
            if ( document.getElementById("content_toolbox_searchField").value != null) {
                var stuff = document.getElementById("content_toolbox_searchField").value;
                finder(stuff);
            }
            if (document.getElementById("content_toolbar_searchField").value != null) {
                var stuff = document.getElementById("content_toolbar_searchField").value;
                finder(stuff);
            }

            break;
        case 68: //D (for debuggin)
            debugs++;
            if (debugs % 2 == 0) {
                document.getElementById("debugs").style.display = "none";
            } else {
                document.getElementById("debugs").style.display = "block";
            }
            break;
    }
}

//resizes container based on the viewport
function resizeView() {
    //de(document.documentElement.clientHeight);

    var totalPX = document.documentElement.clientHeight;
    var headerPX = $("#mapper_container").offset().top;
    var bodyPX = totalPX - headerPX;
    var percentOfHeight = Math.round((bodyPX / totalPX) * 100);
    document.getElementById("mapper_container").style.height = percentOfHeight + "%";

    //de(percentOfHeight);
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