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
            if (actionActive == "other") {
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
    //create message
    var messageText = "<p class=\"message\">";
    messageText += message; //assign incoming message to text
    messageText += "</p>";
    document.getElementById("content_message").innerHTML = messageText; //assign to element

    //show message
    document.getElementById("container_message").style.display = "block"; //display element

    //fade message out
    setTimeout(function () {
        $("#container_message").fadeOut("slow", function () {
            $("#container_message").hide();
        });
    }, 3000); //after 3 sec
}

//create a package to send to server to save item location
function createSavedItem(coordinates) {
    var messageType = "item"; //define what message type it is
    //assign data
    var data = messageType + "|" + coordinates + "|";
    var dataPackage = data + "~";
    alert("saving item: " + dataPackage);
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
    alert("saving overlay set: " + dataPackage);
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
    alert("saving poi set: " + dataPackage);
    //CallServer(dataPackage);
}

//non changed fcns

function poiEditMe(id) {
    poiObj[id].setMap(map);
    if (poiType[id] == "marker") {
        infowindow[id].open(map, poiObj[id]);
    } else {
        infowindow[id].setMap(map);
    }
}                        //open the infowindow of a poi
function poiHideMe(id) {
    poiObj[id].setMap(null);
    infowindow[id].setMap(null);
    label[id].setMap(null);
    document.getElementById("poiToggle" + id).innerHTML = "<img src=\"" + baseURL + "default/images/mapper/add.png\" onclick=\"poiShowMe(" + id + ");\" />";
}                        //hide poi on map
function poiShowMe(id) {
    poiObj[id].setMap(map);
    infowindow[id].setMap(map);
    label[id].setMap(map);
    document.getElementById("poiToggle" + id).innerHTML = "<img src=\"" + baseURL + "default/images/mapper/sub.png\" onclick=\"poiHideMe(" + id + ");\" />";
}                        //show poi on map
function poiDeleteMe(id) {
    poiObj[id].setMap(null);
    poiObj[id] = null;
    poiType[id] = "deleted";
    var strg = "#poi" + id; //create <li> poi string
    $(strg).remove(); //remove <li>
    infowindow[id].setMap(null);
    label[id].setMap(null);
}                      //delete poi from map and list
function poiGetDesc(id) {

    var temp = document.getElementById("poiDesc" + id).value;
    if (temp.contains("~") || temp.contains("|")) {
        displayMessage(L8);
    } else {
        poiDesc[id] = document.getElementById("poiDesc" + id).value;
    }
}                       //get the poi desc
function searchResultDeleteMe() {
    searchResult.setMap(null);
    $("#searchResult").remove();
}               //delete search results from map and list
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
}                    //callback message board
function DisplayCursorCoords(arg) {
    cCoord.innerHTML = arg;
}             //used for lat/long tool

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
}                     //check the zoom level
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
});                               //jquery transparency slider
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

});                              //for rotation knob
function keepRotate(degreeIn) {
    currentlyEditing = "yes"; //used to signify we are editing this overlay
    $(function () {
        $("#overlay" + workingOverlayIndex).rotate(degreeIn);
    });
}                 //maintain specific rotation
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
}                     //variable rotation fcn
function hide(what) {
    $(what).hide();
}                           //hide somethign using jquery
function show(what) {
    $(what).show();
}                           //display something using jquery

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
}                  //get the center lat/long of a polygon
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
}                         //test the map bounds
function finder(stuff) {

    if (stuff.length > 0) {
        codeAddress("lookup", stuff);
    } else {
        //do nothing and keep quiet
    }
}                        //search the gmaps for a location (lat/long or address)
function searcher(stuff) {

    if (stuff.length > 0) {
        displayMessage("No collection to search");
    } else {
        //do nothing and keep quiet
    }
}                      //search the collection for something

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
                document.getElementById("content_toolbox_search_results").innerHTML = "<div id=\"searchResult\">" + geo + " <a href=\"#\" onclick=\"searchResultDeleteMe();\"><img src=\"" + baseURL + "default/images/mapper/delete.png\"/></a></div><br\>"; //add list div
            } else { //if location found was outside strict map bounds...
                displayMessage("Could not find within bounds."); //say so
            }

        } else { //if the entire geocode did not work
            alert(L6); //localization...
        }
    });
}               //get the location of a lat/long or address
function codeLatLng(latlng) {
    if (geocoder) {
        geocoder.geocode({ 'latLng': latlng }, function (results, status) {
            if (status == google.maps.GeocoderStatus.OK) {
                if (results[1]) {
                    document.getElementById("content_toolbox_rgItem").innerHTML = results[0].formatted_address;
                }
                else {
                    document.getElementById("content_toolbox_rgItem").innerHTML = "Geocoder failed due to: " + status;
                    document.getElementById("content_toolbox_rgItem").innerHTML = "";
                }
            }
        });
    }
}                   //get the nearest human reabable location from lat/long
function useSearchAsItemLocation() {
    placerType = "item";                        //this tells listeners what to do
    itemMarker = new google.maps.Marker({
        map: map,
        position: searchResult.getPosition(),   //assign to prev search result location
        draggable: true
    });

    firstMarker++;                              //prevent redraw
    searchResult.setMap(null);                  //delete search result from map
    $("#content_toolbox_searchResult").remove();                //delete search result form list
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

}            //assign search location pin to item location

//keypress shortcuts/actions
window.onkeypress = keypress;
function keypress(e) {
    var keycode = null;
    if (window.event) {
        keycode = window.event.keyCode;
    } else if (e) {
        keycode = e.which;
    }
    displayMessage("keycode: " + keycode);
    switch (keycode) {
        case 70: //F
            if (navigator.appName == "Microsoft Internet Explorer") {
                var copyString = cLat.innerHTML;
                copyString += ", " + cLong.innerHTML;
                window.clipboardData.setData("Text", copyString);
                displayMessage("Coordinates Copied To Clipboard");
            } else {
                if (cCoordsFrozen == "no") {
                    //freeze
                    cCoordsFrozen = "yes";
                    displayMessage("Coordinates Viewer Frozen");
                } else {
                    //unfreeze
                    cCoordsFrozen = "no";
                    displayMessage("Coordinates Viewer UnFrozen");
                }
            }
            break;
        case 79: //O
            if (overlaysCurrentlyDisplayed == true) {
                displayMessage("Hiding Overlays");
                for (var i = 0; i < incomingOverlayBounds.length; i++) {    //go through and display overlays as long as there is an overlay to display
                    overlaysOnMap[i].setMap(null);                          //hide the overlay from the map
                    ghostOverlayRectangle[i].setMap(null);                  //hide ghost from map
                    overlaysCurrentlyDisplayed = false;                     //mark that overlays are not on the map
                }
            } else {
                displayMessage("Showing Overlays");
                for (var i = 0; i < incomingOverlayBounds.length; i++) {   //go through and display overlays as long as there is an overlay to display
                    overlaysOnMap[i].setMap(map);                          //set the overlay to the map
                    ghostOverlayRectangle[i].setMap(map);                  //set to map
                    overlaysCurrentlyDisplayed = true;                     //mark that overlays are on the map
                }
            }

            break;
    }
}