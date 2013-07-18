//utilities

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
    var messageText = "<p class=\"message\">";
    messageText += message; //assign incoming message to text
    messageText += "</p>";
    document.getElementById("content_message").innerHTML = messageText; //assign to element

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
    document.getElementById("saveTest").value = dataPackage;
    //document.location.reload(); //refresh page
    //CallServer(dataPackage);
}

//create a package to send to server to save overlay
function createSavedOverlay(index, source, bounds, rotation) {
    var temp = source;
    if (temp.contains("~") || temp.contains("|")) { //check to make sure reserve characters are not there
        displayMessage(L7);
    }

    var messageType = "overlay"; //define what message type it is
    var data = messageType + "|" + index + "|" + bounds + "|" + source + "|" + rotation + "|";

    var dataPackage = data + "~";
    de("saving overlay set: " + dataPackage); //temp
    document.getElementById("saveTest").value = dataPackage;
    //CallServer(dataPackage); //not yet working
}

//create a package to send to the server to save poi
function createSavedPOI() {
    var dataPackage = "";
    for (var i = 0; i < poiObj.length; i++) {
        switch (poiType[i]) {
            case "marker":
                poiKML[i] = poiObj[i].getPosition().toString();
                break;
            case "circle":
                poiKML[i] = poiObj[i].getCenter() + ",";
                poiKML[i] += poiObj[i].getRadius();
                break;
            case "rectangle":
                poiKML[i] = poiObj[i].getBounds().toString();
                break;
            case "polygon":
                poiObj[i].getPath().forEach(function (latLng) {
                    poiKML[i] += latLng; //NOTE: this has a non standard coordinate separator
                });
                break;
            case "polyline":
                poiObj[i].getPath().forEach(function (latLng) {
                    poiKML[i] += latLng; //NOTE: this has a non standard coordinate separator
                });
                break;
            case "deleted":
                //nothing to do here
                break;
        }
        var data = "poi|" + poiType[i] + "|" + poiDesc[i] + "|" + poiKML[i] + "|";
        dataPackage += data + "~";
    }
    de("saving poi set: " + dataPackage); //temp
    //CallServer(dataPackage);
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
        poiDesc[id] = document.getElementById("poiDesc" + id).value;
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

//callback message board [currently not used]
function HandleResult(arg) {
    switch (arg) {
        case "0":
            displayMessage(L12);
            break;
        case "1":
            displayMessage(L13);
            break;
        case "2":
            displayMessage(L14);
            break;
        case "3":
            displayMessage(L15);
            break;
    }
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
    
    searchResult.setMap(null);                  //delete search result from map
    $("#searchResult_1").remove();              //delete search result from list
    
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
    //alert(document.documentElement.clientHeight);

    var totalPX = document.documentElement.clientHeight;
    var headerPX = $("#mapper_container").offset().top;
    var bodyPX = totalPX - headerPX;
    var percentOfHeight = Math.round((bodyPX / totalPX) * 100);
    document.getElementById("mapper_container").style.height = percentOfHeight + "%";

    //alert(percentOfHeight);
}

//debugging 
var debugString = "<strong>Debug Panel:</strong><br><br>";
var debugs = 0; //used for keycode debugging
function de(message) {
    //create debug string
    var currentdate = new Date();
    var time = currentdate.getHours() + ":" + currentdate.getMinutes() + ":" + currentdate.getSeconds() + ":" + currentdate.getMilliseconds();
    debugString += "[" + time + "] " + message + "<br><hr>";
    document.getElementById("debugs").innerHTML = debugString;
}