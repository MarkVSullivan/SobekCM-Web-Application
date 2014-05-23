/*
This file loads all of the custom javascript libraries needed to run the mapsearch portion of sobek.
*/

//does nothing just here to appease the firebug todo: find where this is being called...
function load() {

}

var MAPSEARCHER; //TEMP must remain at top for now (for C# js vars)

//must remain outside fcns at top level (for now)
var map;                //holds gmap
var baseURL;            //holds base url
var markers = [];       //holds all the markers on the map
var infowindows = [];   //holds all the infowindows on the map
var markerCluster;      //holds the map clusterer object
var DSR;                //holds the display search results object
var mcOptions = {
    styles: [{
        height: 29,
        url: "http://hlmatt.com/uf/clusters/lvl3.png",
        width: 29
    }]
};

//add start listener
google.maps.event.addDomListener(window, 'load', initAll);

//init all
function initAll() {
    console.log("Initializing MapSearcher..."); //this fixesd a console issue
    //init server vars
    initServerToClientVars();
    //init display search result (DSR)
    initDSR();
    //init declarations (local/js)
    initDeclarations();
    //init map elements
    initMapElements();
    //show DSR
    try {
        showDSR();
    } catch (e) {
        console.error("Could not show DSR " + e);
    }
    //fit map
    map.fitBounds(MAPSEARCHER.GLOBAL.DEFINES.defaultSearchBounds); //this could trigger a zoom changed event (redraw of markers) if the zoom changes from default
    //resize view (currently just the map)
    resizeView('init');

    //switch map type if zoom max reached
    google.maps.event.addListener(map, 'zoom_changed', function () {
        if ((map.getZoom() > 14)) {
            map.setOptions({ mapTypeId: google.maps.MapTypeId.ROADMAP });
        } else {
            map.setOptions({ mapTypeId: google.maps.MapTypeId.TERRAIN });
        }
    });

    google.maps.event.addListener(MAPSEARCHER.GLOBAL.DEFINES.drawingManager, 'rectanglecomplete', function (rectangle) {
        //close the drawing manager bounds selector
        MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setDrawingMode(null);
        console.info("searchAreaBounds: " + rectangle.getBounds());
        searchWithinBounds(rectangle.getBounds());

        var dragging = false;

        google.maps.event.addListener(rectangle, 'dragstart', function () {
            dragging = true;
        });

        google.maps.event.addListener(rectangle, 'dragend', function () {
            dragging = false;
            console.info("searchAreaBounds: " + this.getBounds());
            searchWithinBounds(this.getBounds());
        });

        google.maps.event.addListener(rectangle, 'bounds_changed', function () {
            if (!dragging) {
                console.info("searchAreaBounds: " + this.getBounds());
                searchWithinBounds(this.getBounds());
            }
        });

    });

    //if the bounds change by panning (dragging)
    //google.maps.event.addListener(map, 'dragend', function () {
    //    if (MAPSEARCHER.GLOBAL.DEFINES.markersVisible == true) {
    //        drawMarkersInBounds(map.getBounds());
    //    }
    //});
    //if the bounds change by zoom
    //google.maps.event.addListener(map, 'zoom_changed', function () {
    //    if (MAPSEARCHER.GLOBAL.DEFINES.markersVisible == true) {
    //        drawMarkersInBounds(map.getBounds());
    //    }
    //});
}

//testing/holder
function searchWithinBounds(bounds) {
    //toServer("search|bounds|" + bounds.getSouthWest().lat() + "|" + bounds.getSouthWest().lng() + "|" + bounds.getNorthEast().lat() + "|" + bounds.getNorthEast().lng() + "|" + MSRKey + "|");
    toServer("search|bounds|" + bounds.getSouthWest().lat() + "|" + bounds.getSouthWest().lng() + "|" + bounds.getNorthEast().lat() + "|" + bounds.getNorthEast().lng() + "|");
}

//#region inits

function initDSR() {
    try {
        initJSON();
        DSR = JSON.parse(DSR);
    } catch(e) {
        console.error("Could not show DSR " + e);
    } 
}

function initDeclarations() {

    MAPSEARCHER = function () {
        return {
            GLOBAL: function () {
                return {
                    DEFINES: function () {
                        return {
                            defaultSearchBounds: null,
                            incomingPointFeatureType: [],
                            incomingPointLabel: [],
                            incomingPointCenter: [],
                            isActive_searchControl: false,
                            isActive_filterControl: false,
                            pageViewMode: "geo", //2do, mv2config
                            defaultMapZoom: 2,
                            markersVisible: true,
                            searchBoundsDrawn: false,
                            drawingManager: null
                        };
                    }()
                };
            }()
        };
    }();

    //init dependent vars
    
    //get search results bounds (these are always the last two josn points)
    try {
        var swx = DSR[DSR.length - 2].Point_Latitude;
        var swy = DSR[DSR.length - 2].Point_Longitude;
        var nex = DSR[DSR.length - 1].Point_Latitude;
        var ney = DSR[DSR.length - 1].Point_Longitude;
        MAPSEARCHER.GLOBAL.DEFINES.defaultSearchBounds = new google.maps.LatLngBounds(new google.maps.LatLng(swx, swy), new google.maps.LatLng(nex, ney));
    } catch (e) {
        console.error("Could not get bounds from DSR " + e);
        MAPSEARCHER.GLOBAL.DEFINES.defaultSearchBounds = new google.maps.LatLngBounds(
            new google.maps.LatLng(85, -180),           // top left corner of map
            new google.maps.LatLng(-85, 180)            // bottom right corner
        );
    }
    
    //alert("LOADING (this is here to force firebug to start)");
    
}

function initMapElements() {

    //define map and options
    map = new google.maps.Map(document.getElementById('container_SearchMap'), {
        zoom: MAPSEARCHER.GLOBAL.DEFINES.defaultMapZoom,
        center: MAPSEARCHER.GLOBAL.DEFINES.defaultSearchBounds.getCenter(),
        streetViewControl: false, //is streetview active?
        scaleControl: true,
        //rotateControl: true,
        tilt: 0, //set to 0 to disable 45 degree tilt
        mapTypeId: google.maps.MapTypeId.TERRAIN,
        mapTypeControlOptions: {
            style: google.maps.MapTypeControlStyle.DROPDOWN_MENU, //map layer control style
            position: google.maps.ControlPosition.TOP_LEFT //map layer control position
        },
        zoomControlOptions: {
            style: google.maps.ZoomControlStyle.SMALL, //zoom control style
            position: google.maps.ControlPosition.LEFT_TOP //zoom control position
        },
        panControlOptions: {
            position: google.maps.ControlPosition.LEFT_TOP //pan control position
        },
        overviewMapControl: true,
        overviewMapControlOptions: {
            opened: false
        },
        styles: //turn off all poi stylers (supporting url: https://developers.google.com/maps/documentation/javascript/reference#MapTypeStyleFeatureType)
            [
            {
                featureType: "poi", //poi
                elementType: "all", //or labels
                stylers: [{ visibility: "off" }]
            },
            {
                featureType: "transit", //poi
                elementType: "labels", //labels
                stylers: [{ visibility: "off" }]
            }
            ]
    });

    //visual refresh option
    google.maps.visualRefresh = true;

    //create and set toggle DSRs control
    var toggleDSRControlDiv = document.createElement('div');
    var toggleDSRControl = new ToggleDSRControl(toggleDSRControlDiv, map);
    toggleDSRControlDiv.index = 1;
    map.controls[google.maps.ControlPosition.TOP_RIGHT].push(toggleDSRControlDiv);

    //create and set toggle DSRs control
    var boundsControlDiv = document.createElement('div');
    var boundsControl = new BoundsControl(boundsControlDiv, map);
    boundsControlDiv.index = 1;
    map.controls[google.maps.ControlPosition.TOP_RIGHT].push(boundsControlDiv);

    //define dm and options
    MAPSEARCHER.GLOBAL.DEFINES.drawingManager = new google.maps.drawing.DrawingManager({
        drawingControl: false,
        drawingControlOptions: {
            position: google.maps.ControlPosition.RIGHT_TOP,
            drawingModes: [
              google.maps.drawing.OverlayType.RECTANGLE
            ]
        },
        rectangleOptions: {
            //fillColor: '#ffff00',
            editable: true,
            draggable: true,
            zIndex: 5,
            strokeOpacity: 1.0,
            strokeWeight: 0.5,
            fillOpacity: 0.25
        }
    });
    MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setMap(map);

    //init SRFH1
    //init SRFH2
    //init SRTShowing
    //init SRTShowing

}

//#endregion

//#region GMap Controls

//this sis the toggle control
function ToggleDSRControl(controlDiv, map) {

    // Set CSS styles for the DIV containing the control
    // Setting padding to 5 px will offset the control
    // from the edge of the map
    controlDiv.style.padding = '5px';

    // Set CSS for the control border
    var controlUI = document.createElement('div');
    controlUI.style.backgroundColor = '#FFFFFF';
    controlUI.style.borderStyle = 'solid';
    controlUI.style.borderWidth = '0';
    controlUI.style.cursor = 'pointer';
    controlUI.style.textAlign = 'center';
    controlUI.title = 'Click to Toggle Search Results';
    controlDiv.appendChild(controlUI);

    // Set CSS for the control interior
    var controlText = document.createElement('div');
    controlText.className = "gmapControl";
    controlText.style.fontFamily = 'Arial,sans-serif';
    controlText.style.fontSize = '10px';
    controlText.style.paddingTop = '2px';
    controlText.style.paddingBottom = '2px';
    controlText.style.paddingLeft = '4px';
    controlText.style.paddingRight = '4px';
    //controlText.innerHTML = '<b>Toggle Results</b>';
    controlText.innerHTML = '<b>Select Layer</b>';
    controlUI.appendChild(controlText);

    //handle the clicked button state
    google.maps.event.addDomListener(controlUI, 'click', function () {
        toggleMarkers();
    });

}

//this sis the toggle control
function BoundsControl(controlDiv, map) {

    // Set CSS styles for the DIV containing the control
    // Setting padding to 5 px will offset the control
    // from the edge of the map
    controlDiv.style.padding = '5px';

    // Set CSS for the control border
    var controlUI = document.createElement('div');
    controlUI.style.backgroundColor = '#FFFFFF';
    controlUI.style.borderStyle = 'solid';
    controlUI.style.borderWidth = '0';
    controlUI.style.cursor = 'pointer';
    controlUI.style.textAlign = 'center';
    controlUI.title = 'Click to Define a Search Bounding Box';
    controlDiv.appendChild(controlUI);

    // Set CSS for the control interior
    var controlText = document.createElement('div');
    controlText.className = "gmapControl";
    controlText.style.fontFamily = 'Arial,sans-serif';
    controlText.style.fontSize = '10px';
    controlText.style.paddingTop = '2px';
    controlText.style.paddingBottom = '2px';
    controlText.style.paddingLeft = '4px';
    controlText.style.paddingRight = '4px';
    controlText.innerHTML = '<b>Define Bounds</b>';
    controlUI.appendChild(controlText);

    //handle the clicked button state
    google.maps.event.addDomListener(controlUI, 'click', function () {
        if (!MAPSEARCHER.GLOBAL.DEFINES.searchBoundsDrawn) {
            MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setDrawingMode(google.maps.drawing.OverlayType.RECTANGLE);
            MAPSEARCHER.GLOBAL.DEFINES.searchBoundsDrawn = true;
        }
    });

}

//#endregion

function showDSR() {
    //draw the markers (if visible)
    if (MAPSEARCHER.GLOBAL.DEFINES.markersVisible == true) {
        drawMarkers();
    }
}

function drawMarkers() {
    clearMarkers();
    console.info("total incoming markers: " + DSR.length);

    ////setup objects
    //var points = [];
    //var bounds = map.getBounds();
    //var southWest = bounds.getSouthWest();
    //var northEast = bounds.getNorthEast();
    //var lngSpan = northEast.lng() - southWest.lng();
    //var latSpan = northEast.lat() - southWest.lat();
    ////create the random points
    //for (var i = 0; i < DSR.length; i++) {
    //    var point = new google.maps.LatLng(southWest.lat() + latSpan * Math.random(), southWest.lng() + lngSpan * Math.random());
    //    points.push(point);
    //}

    //go through and now add the points to the map
    for (var i = 0; i < DSR.length ; i++) {
        if (DSR[i].ItemID.indexOf("Bounds") == -1) {
            var marker;
            marker = new google.maps.Marker({
                position: new google.maps.LatLng(DSR[i].Point_Latitude, DSR[i].Point_Longitude)
                //, position: points
                , title: DSR[i].ItemID
                //, icon: "http://hlmatt.com/uf/iconActive.png"
            });
            markers.push(marker);
        } else {
            //these are the bounds sw/ne
        }
    }
    markerCluster = new MarkerClusterer(map, markers, mcOptions);
    markerCluster.setGridSize(25);

    //change the showing //todo: localize
    document.getElementById("hook_showing_coord_range_text").innerHTML = "showing " + markers.length + " titles";

    console.info("Total Markers on map: " + markers.length);
}

function toggleMarkers() {
    if (MAPSEARCHER.GLOBAL.DEFINES.markersVisible == true) {
        clearMarkers();
        MAPSEARCHER.GLOBAL.DEFINES.markersVisible = false;
    } else {
        drawMarkers();
        MAPSEARCHER.GLOBAL.DEFINES.markersVisible = true;
    }
}

function clearMarkers() {
    console.info("Clearing " + markers.length + " markers.");
    if (markers.length > 0) {
        //console.info("Clearing " + markers.length + " markers.");
        //for (var i = 0; i < markers.length; i++) {
        //    markers[i].setMap(null);
        //}
        markers = [];
        markerCluster.clearMarkers();
        //console.info("Markers cleared.");
    } else {
        console.warn("No markers found to clear.");
    }
}

//sends datapackage to the server
function toServer(data) {
    var dataPackage = data + "~";
    console.debug(dataPackage);
    //document.getElementById('payload').value = JSON.stringify({ sendData: dataPackage });
    $.ajax({
        type: "POST",
        url: baseURL + "default/scripts/Callbacks.aspx/MapSearch",
        data: JSON.stringify({ sendData: dataPackage }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        dataFilter: function (data) {
            // This boils the response string down into a proper JavaScript Object()
            var msg = eval('(' + data + ')');
            // If the response has a ".d" top-level property,
            if (msg.hasOwnProperty('d'))
                return msg.d;
            else
                return msg;
        },
        success: function (response) {
            DSR = response;
            showDSR();
        },
        failure: function (response) {
            alert("There was an error, please try again later...");
        }
    });
}

//resizes container based on the viewport
function resizeView(param) {
    try {
        //get sizes of elements already drawn
        var totalPX = document.documentElement.clientHeight;
        var headerPX = document.getElementById("container_SearchMap").offsetHeight;
        //var toolbar1PX = document.getElementById("sbkAgm_MenuBar").offsetHeight;
        //var toolbar2PX = document.getElementById("container_toolbar1").offsetHeight;
        //var toolbar3PX = document.getElementById("container_toolbar2").offsetHeight;
        var widthPX = document.documentElement.clientWidth;
        var bodyPX = 0; //init

        //calculate and set sizes (the header and toolbar are taken into account as 'ghosts' in all but IE)
        if (param == 'init') {
            //bodyPX = totalPX - ((headerPX * 2) + ((toolbar1PX + toolbar2PX + toolbar3PX))); //this accounts for toolbar not being loaded
            //bodyPX = totalPX - (265 * 2); //TEMP OVERRIDE
        } else {
            //bodyPX = totalPX - ((headerPX * 2) + toolbar1PX + toolbar2PX + toolbar3PX);
            //bodyPX = totalPX - ((265 * 2) - 40); //TEMP OVERRIDE
        }
        if (navigator.appName == 'Microsoft Internet Explorer') {
            //bodyPX = totalPX - (headerPX + toolbar1PX + toolbar2PX + toolbar3PX); //for IE, no ghosts
            //bodyPX = totalPX - 265; //TEMP OVERRIDE
        }

        document.getElementById("container_SearchMap").style.height = (totalPX - 283) + "px";
        document.getElementById("container_SearchMap").style.height = (totalPX - 283) + "px";

    } catch (e) {
        console.error(e);
    }
}

//serach with this facet
function add_facet_callback2(code, new_value) {
    alert(code);
    alert(new_value);


    var aggregations = "usach";
    var data = "search|filter|" + aggregations + "|" + map.getBounds().getSouthWest().lat() + "|" + map.getBounds().getSouthWest().lng() + "|" + map.getBounds().getNorthEast().lat() + "|" + map.getBounds().getNorthEast().lng() + "|";
    //toServer(data);
}

//init filterbox onload
$(function () {
    
    //define icons object
    var icons = {
        header: "ui-icon-circle-arrow-e",
        activeHeader: "ui-icon-circle-arrow-s"
    };

    //disable icons
    //$( "#filterBox" ).accordion( "option", "icons", null );

    //define filterBox
    $('#filterBox').accordion({
        header: "div.sbkPrsw_FacetBoxTitle",
        //icons: icons,
        active: false,
        collapsible: true,
        heightStyle: "content",
        beforeActivate: function (event, ui) {
            // The accordion believes a panel is being opened
            if (ui.newHeader[0]) {
                var currHeader = ui.newHeader;
                var currContent = currHeader.next('.ui-accordion-content');
                // The accordion believes a panel is being closed
            } else {
                var currHeader = ui.oldHeader;
                var currContent = currHeader.next('.ui-accordion-content');
            }
            // Since we've changed the default behavior, this detects the actual status
            var isPanelSelected = currHeader.attr('aria-selected') == 'true';
            // Toggle the panel's header
            currHeader.toggleClass('ui-corner-all', isPanelSelected).toggleClass('accordion-header-active ui-state-active ui-corner-top', !isPanelSelected).attr('aria-selected', ((!isPanelSelected).toString()));
            // Toggle the panel's icon
            currHeader.children('.ui-icon').toggleClass('ui-icon-triangle-1-e', isPanelSelected).toggleClass('ui-icon-triangle-1-s', !isPanelSelected);
            // Toggle the panel's content
            currContent.toggleClass('accordion-content-active', !isPanelSelected);
            if (isPanelSelected) { currContent.slideUp(); } else { currContent.slideDown(); }
            // Cancels the default action
            return false;
        }
    });

    //set currently active boxes //2do, read from config
    //$( "#filterBox" ).accordion( "option", "active", 5 );

});