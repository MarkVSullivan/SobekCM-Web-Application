/*
This file loads all of the custom javascript libraries needed to run the mapsearch portion of sobek.
*/

var MAPSEARCHER;                                //TEMP must remain at top for now (for C# js vars)

function load() {
    //does nothing just here to appease the firebug
};

try {
    //reallocate div structure
    document.getElementById("addedForm").remove();
    document.getElementById('container-facets').appendChild(document.getElementById("container_mapSearcher"));
    //document.getElementById('container-facets').appendChild(document.getElementById("footer"));
    //document.getElementById('container-facets').appendChild(document.getElementById("UfdcWordmark"));
    //document.getElementById('container-facets').appendChild(document.getElementById("UfdcCopyright"));
    document.getElementById("itemNavForm").remove();

    //var holder_content = document.getElementById("container_mapSearcher");
    ////var holder_footer1 = document.getElementById("footer");
    ////var holder_footer2 = document.getElementById("UfdcWordmark");
    ////var holder_footer3 = document.getElementById("UfdcCopyright");
        
    //document.getElementById('container-facets').appendChild(holder_content);
    ////document.getElementById('container-facets').appendChild(holder_footer1);
    ////document.getElementById('container-facets').appendChild(holder_footer2);
    ////document.getElementById('container-facets').appendChild(holder_footer3);

    //document.getElementById("itemNavForm").remove();

} catch (e) {
    console.log(e);
}

function initMapSearcher() {
    //declare the namespace
    MAPESEARCHER = function () {
        return {
            TRACER: function () {
                return {
                    DEFINES: function () {
                        return {
                            debugVersionNumber: "",              //init timestamp
                            debugs: 0,
                            debugStringBase: "<strong>Debug Panel:</strong> &nbsp;&nbsp; <a onclick=\"MAPSEARCHER.TRACER.clearTracer()\">(clear)</a> &nbsp;&nbsp; <a onclick=\"MAPSEARCHER.TRACER.switchTracer()\">(on/off)</a> &nbsp;&nbsp; <a onclick=\"MAPSEARCHER.TRACER.toggleTracer()\">(close)</a><br><br>",
                            debugString: null
                        };
                    }(),
                    addTracer: function (message) {          //displays tracer message
                        var currentdate = new Date();
                        var time = currentdate.getHours() + ":" + currentdate.getMinutes() + ":" + currentdate.getSeconds() + ":" + currentdate.getMilliseconds();
                        console.log("[" + time + "] " + message); //always output to console
                        if (MAPSEARCHER.GLOBAL.DEFINES.debuggerOn) {
                            var newDebugString = "[" + time + "] " + message + "<br><hr>";
                            newDebugString += MAPSEARCHER.TRACER.DEFINES.debugString;
                            document.getElementById("debugs").innerHTML = MAPSEARCHER.TRACER.DEFINES.debugStringBase + newDebugString;
                            //only debug if it does not hinder performance
                            if (newDebugString.length < 10000) {
                                MAPSEARCHER.TRACER.DEFINES.debugString = newDebugString;
                            } else {
                                console.log("IN APP DEBUGGER CLEARED");
                                MAPSEARCHER.TRACER.clearTracer();
                            }
                        }
                    },
                    clearTracer: function () {
                        //clear debug string
                        MAPSEARCHER.TRACER.DEFINES.debugString = "";
                        //put the base string back in
                        document.getElementById("debugs").innerHTML = MAPSEARCHER.TRACER.DEFINES.debugStringBase;
                    },
                    toggleTracer: function () {
                        if (MAPSEARCHER.GLOBAL.DEFINES.debuggerOn == true) {
                            MAPSEARCHER.TRACER.DEFINES.debugs++;
                            if (MAPSEARCHER.TRACER.DEFINES.debugs % 2 == 0) {
                                document.getElementById("debugs").style.display = "none";
                                MAPSEARCHER.GLOBAL.DEFINES.debugMode = false;
                                MAPSEARCHER.UTILITIES.displayMessage("Debug Mode Off");
                            } else {
                                document.getElementById("debugs").style.display = "block";
                                MAPSEARCHER.GLOBAL.DEFINES.debugMode = true;
                                MAPSEARCHER.UTILITIES.displayMessage("Debug Mode On");
                            }
                        }
                    },
                    switchTracer: function () {
                        if (MAPSEARCHER.GLOBAL.DEFINES.debuggerOn == true) {
                            MAPSEARCHER.GLOBAL.DEFINES.debuggerOn = false;
                        } else {
                            MAPSEARCHER.GLOBAL.DEFINES.debuggerOn = true;
                        }
                    }
                };
            }(),          //tracer support
            GLOBAL: function () {
                return {
                    DEFINES: function () {
                        return {
                            //google maps related
                            gmapPageDivId: "googleMap",                                 //defines the div where the gmap is
                            cursorLatLongTool: null,                                    //holds cCoords control
                            toolbarBufferZone1: null,                                   //pushes map controls down some
                            toolbarBufferZone2: null,                                   //pushes map controls down some
                            copyrightNode: null,                                        //holds custom copyright
                            drawingManager: new google.maps.drawing.DrawingManager(),   //init default dm
                            gmapOptions: null,                                          //holds MAPSEARCHER.GLOBAL.DEFINES.gmapOptions
                            infoWindow: [],                                             //poi infoWindow
                            label: [],                                                  //used for the label of the poi
                            geocoder: null,                                             //gmap geocoder object
                            kmlLayer: null,                                             //gmap kmllayer object

                            //defaults (read from config file )
                            helpPageURL: "http://cms.uflib.ufl.edu/webservices/StAugustineProject/MapEditorHelper.aspx", //defines help page (TEMP)
                            reportProblemURL: "http://ufdc.ufl.edu/contact", //TEMPO move to config
                            toServerSuccessMessage: "Completed",        //holds server success message todo localize
                            listItemHighlightColor: "#FFFFC2",          //holds the default highlight color 
                            baseURL: null,                              //holds place for server written vars
                            collectionLoadType: null,                   //hold place for collection load type
                            collectionParams: [],                       //hold place for collection params
                            defaultOpacity: 0.5,                        //holds default opacity settings
                            listOfTextAreaIds: [],                      //holds listOfTextAreaIds
                            debugMode: null,                            //holds debug marker
                            hasCustomMapType: null,                     //holds marker for determining if there is a custom map type
                            baseImageDirURL: null,                      //holds the base image directory url
                            mapCenter: null,                            //used to center map on load
                            defaultZoomLevel: null,                     //zoom level, starting
                            maxZoomLevel: null,                         //max zoom out, default (21:lowest level, 1:highest level)
                            minZoomLevel_Terrain: null,                 //max zoom in, terrain 
                            minZoomLevel_Satellite: null,               //max zoom in, sat + hybrid
                            minZoomLevel_Roadmap: null,                 //max zoom in, roadmap (default)
                            minZoomLevel_BlockLot: null,                //max zoom in, used for special layers not having default of roadmap
                            strictBounds: null,                         //holds the strict bounds
                            incomingACL: "item",                        //hold default incoming ACL (determined in displaying points/overlays)

                            //flags
                            typingInTextArea: false,                    //hold marker for if we are in a textArea
                            debuggerOn: false,                          //holds debugger flag
                            toServerSuccess: false,                     //holds a marker indicating if toserver was sucessfull
                            tempYo: false,                              //holds tempyo for fixing ff info window issue
                            buttonActive_searchResultToggle: false,     //holds is button active markers
                            buttonActive_itemPlace: false,              //holds is button active markers
                            buttonActive_overlayToggle: false,          //holds is button active markers
                            buttonActive_poiToggle: false,              //holds is button active markers
                            userMayLoseData: false,                     //holds a marker to determine if signifigant changes have been made to require a save
                            isConvertedOverlay: false,                  //holds a marker for converted overlay
                            RIBMode: false,                             //holds a marker for running in background mode (do not display messages)
                            poiToggleState: "displayed",                //holds marker for displayed/hidden pois
                            mapControlsDisplayed: null,                 //by default, are map controls displayed (true/false)
                            defaultDisplayDrawingMangerTool: null,      //by default, is the drawingmanger displayed (true/false)
                            toolboxDisplayed: null,                     //by default, is the toolbox displayed (true/false)
                            toolbarDisplayed: null,                     //by default, is the toolbar open (yes/no)
                            kmlDisplayed: null,                         //by default, is kml layer on (yes/no)
                            isCustomOverlay: null,                      //used to determine if other overlays (block/lot etc) //used in testbounds unknown if needed
                            pendingOverlaySave: false,                  //hold the marker to indicate if we need to save the overlay (this prevents a save if we already saved)
                            cCoordsFrozen: "no",                        //used to freeze/unfreeze coordinate viewer
                            currentlyEditing: "no",                     //tells us if we are editing anything
                            firstSaveItem: null,                        //holds first save marker (used to determine if saving or applying changes)
                            firstSaveOverlay: null,                     //holds first save marker (used to determine if saving or applying changes)
                            firstSavePOI: null,                         //holds first save marker (used to determine if saving or applying changes)
                            mapDrawingManagerDisplayed: null,           //holds marker for drawing manager
                            overlaysCurrentlyDisplayed: null,           //holds marker for overlays on map
                            mapInBounds: null,                          //is the map in bounds

                            //object holders
                            mapLayerActive: null,                       //holds the current map layer active
                            prevMapLayerActive: null,                   //holds the previous active map layer
                            actionActive: null,                         //holds the current active action
                            prevActionActive: null,                     //holds the previous active action
                            pageMode: null,                             //holds the page/viewer type
                            overlaysOnMap: [],                          //holds all overlays
                            searchResult: null,                         //will contain object
                            circleCenter: null,                         //hold center point of circle
                            markerCenter: null,                         //hold center of marker
                            placerType: null,                           //type of data (marker,overlay,poi)
                            poiType: [],                                //typle of poi (marker, circle, rectangle, polygon, polyline)
                            poiKML: [],                                 //pou kml layer (or other geographic info)
                            poiObj: [],                                 //poi object placholder
                            poiCoord: [],                               //poi coord placeholder
                            poiDesc: [],                                //desc poi placeholder
                            globalPolyline: null,                       //unknown
                            rectangle: null,                            //must define before use
                            getCoord: null,                             //used to store coords from marker
                            itemMarker: null,                           //hold current item marker
                            savingMarkerCenter: null,                   //holds marker coords to save
                            savingOverlayIndex: [],                     //holds index of the overlay we are saving
                            savingOverlayLabel: [],                     //holds label of the overlay we are saving
                            savingOverlaySourceURL: [],                 //hold the source url of the overlay to save
                            savingOverlayBounds: [],                    //holds bounds of the overlay we are saving
                            savingOverlayRotation: [],                  //holds rotation of the overlay we are saving
                            savingOverlayPageId: [],                    //holds page ID of saving overlay
                            ghostOverlayRectangle: [],                  //holds ghost overlay rectangles (IE overlay hotspots)

                            //simple data holders
                            stickyMessageCount: 0,                      //holds stickyMessageCount
                            pageLoadTime: null,                         //holds time page was loaded
                            messageCount: 0,                            //holds the running count of all the messages written in a session
                            poiCount: 0,                                //holds a marker for pois drawn (fixes first poi desc issue)
                            preservedRotation: null,                    //rotation, default
                            knobRotationValue: null,                    //rotation to display by default 
                            preservedOpacity: null,                     //opacity, default value (0-1,1:opaque)
                            csoi: 0,                                    //hold current saved overlay index
                            oomCount: 0,                                //counts how many overlays are on the map
                            searchCount: 0,                             //interates how many searches
                            degree: 0,                                  //initializing degree
                            firstMarker: 0,                             //used to iterate if marker placement was the first (to prevent duplicates)
                            overlayCount: 0,                            //iterater (contains how many overlays are not deleted)
                            poi_i: -1,                                  //increment the poi count (used to make IDs and such)
                            firstDraw: 0,                               //used to increment first drawing of rectangle
                            mainCount: 0,                               //hold debug main count
                            overlayType: null,                          //draw = overlay was drawn
                            convertedOverlayIndex: 0,                   //holds the place for indexing a converted overlay
                            workingOverlayIndex: null,                  //holds the index of the overlay we are working with (and saving)
                            currentTopZIndex: 5,                        //current top zindex (used in displaying overlays over overlays)

                            //incoming (C#2JS)
                            incomingPointSourceURLConverted: null,      //holds convert 
                            incomingPointLabelConverted: null,          //holds convert 
                            incomingLineFeatureType: [],                //defined in c# to js on page
                            incomingLineLabel: [],                      //defined in c# to js on page
                            incomingLinePath: [],                       //defined in c# to js on page
                            incomingCircleCenter: [],                   //defined in c# to js on page
                            incomingCircleRadius: [],                   //defined in c# to js on page
                            incomingCircleFeatureType: [],              //defined in c# to js on page
                            incomingCircleLabel: [],                    //defined in c# to js on page
                            incomingPointFeatureType: [],               //defined in c# to js on page
                            incomingPointCenter: [],                    //defined in c# to js on page
                            incomingPointLabel: [],                     //defined in c# to js on page
                            incomingPointSourceURL: [],                 //defined in c# to js on page
                            incomingPolygonPageId: [],                  //defined in c# to js on page
                            incomingPolygonFeatureType: [],             //defined in c# to js on page
                            incomingPolygonPolygonType: [],             //defined in c# to js on page
                            incomingPolygonBounds: [],                  //defined in c# to js on page
                            incomingPolygonPath: [],                    //defined in c# to js on page
                            incomingPolygonCenter: [],                  //defined in c# to js on page
                            incomingPolygonLabel: [],                   //defined in c# to js on page
                            incomingPolygonSourceURL: [],               //defined in c# to js on page
                            incomingPolygonRotation: [],                //defined in c# to js on page
                            incomingPolygonRadius: [],                  //defined in c# to js on page

                            //gmap related
                            ghosting: {
                                strokeOpacity: 0.0,                     //make border invisible
                                fillOpacity: 0.0,                       //make fill transparent
                                editable: false,                        //sobek standard
                                draggable: false,                       //sobek standard
                                clickable: false,                       //sobek standard
                                zindex: 5                               //perhaps higher?
                            },                                          //defines options for MAPSEARCHER.GLOBAL.DEFINES.ghosting (IE being invisible)
                            editable: {
                                editable: true,                         //sobek standard
                                draggable: true,                        //sobek standard
                                clickable: true,                        //sobek standard
                                strokeOpacity: 0.2,                     //sobek standard
                                strokeWeight: 1,                        //sobek standard
                                fillOpacity: 0.0,                       //sobek standard 
                                zindex: 5                               //sobek standard
                            },                                          //defines options for visible and MAPSEARCHER.GLOBAL.DEFINES.editable
                            markerOptionsDefault: {
                                draggable: true,
                                zIndex: 5
                            },
                            rectangleOptionsDefault: {
                                editable: true,
                                draggable: true,
                                zIndex: 5,
                                strokeOpacity: 1.0,
                                strokeWeight: 1,
                                fillOpacity: 0.5
                            },
                            polygonOptionsDefault: {
                                editable: true,
                                draggable: true,
                                zIndex: 5,
                                strokeOpacity: 1.0,
                                strokeWeight: 1,
                                fillOpacity: 0.5
                            },
                            circleOptionsDefault: {
                                editable: true,
                                draggable: true,
                                zIndex: 5,
                                strokeOpacity: 1.0,
                                strokeWeight: 1,
                                fillOpacity: 0.5
                            },
                            polylineOptionsDefault: {
                                editable: true,
                                draggable: true,
                                zIndex: 5,
                                strokeOpacity: 1.0,
                                strokeWeight: 1
                            },
                            rectangleOptionsOverlay: {
                                editable: true,
                                draggable: true,
                                zIndex: 5,
                                strokeOpacity: 0.2,
                                strokeWeight: 1,
                                fillOpacity: 0.0
                            },
                            markerOptionsItem: {
                                draggable: true,
                                zIndex: 5
                            },
                            markerOptionsPOI: {
                                draggable: true,
                                zIndex: 5
                            },
                            circleOptionsPOI: {
                                editable: true,
                                draggable: true,
                                zIndex: 5,
                                strokeOpacity: 1.0,
                                strokeWeight: 1,
                                fillOpacity: 0.5
                            },
                            rectangleOptionsPOI: {
                                editable: true,
                                draggable: true,
                                zIndex: 5,
                                strokeOpacity: 1.0,
                                strokeWeight: 1,
                                fillOpacity: 0.5
                            },
                            polygonOptionsPOI: {
                                editable: true,
                                draggable: true,
                                zIndex: 5,
                                strokeOpacity: 1.0,
                                strokeWeight: 1,
                                fillOpacity: 0.5
                            },
                            polylineOptionsPOI: {
                                editable: true,
                                draggable: true,
                                zIndex: 5,
                                strokeOpacity: 1.0,
                                strokeWeight: 1
                            }
                        };
                    }(),
                    displayIncomingPoints: function () {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: displayIncomingPoints started...");

                            if (MAPSEARCHER.GLOBAL.DEFINES.incomingPointCenter) {
                                //go through and display points as long as there is a point to display
                                for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.incomingPointCenter.length; i++) {
                                    switch (MAPSEARCHER.GLOBAL.DEFINES.incomingPointFeatureType[i]) {
                                        case "":
                                            MAPSEARCHER.GLOBAL.DEFINES.placerType = "item";
                                            MAPSEARCHER.GLOBAL.DEFINES.firstMarker++;
                                            MAPSEARCHER.GLOBAL.DEFINES.itemMarker = new google.maps.Marker({
                                                position: MAPSEARCHER.GLOBAL.DEFINES.incomingPointCenter[i],
                                                map: map,
                                                title: MAPSEARCHER.GLOBAL.DEFINES.incomingPointLabel[i]
                                            });
                                            MAPSEARCHER.GLOBAL.DEFINES.itemMarker.setOptions(MAPSEARCHER.GLOBAL.DEFINES.markerOptionsItem);
                                            document.getElementById('content_toolbox_posItem').value = MAPSEARCHER.GLOBAL.DEFINES.itemMarker.getPosition();
                                            MAPSEARCHER.UTILITIES.codeLatLng(MAPSEARCHER.GLOBAL.DEFINES.itemMarker.getPosition());
                                            google.maps.event.addListener(MAPSEARCHER.GLOBAL.DEFINES.itemMarker, 'dragend', function () {
                                                MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPSEARCHER.ACTIONS.openToolboxTab("item");
                                                MAPSEARCHER.GLOBAL.DEFINES.firstSaveItem = true;
                                                MAPSEARCHER.GLOBAL.DEFINES.savingMarkerCenter = MAPSEARCHER.GLOBAL.DEFINES.itemMarker.getPosition(); //store coords to save
                                                document.getElementById('content_toolbox_posItem').value = MAPSEARCHER.GLOBAL.DEFINES.itemMarker.getPosition();
                                                MAPSEARCHER.UTILITIES.codeLatLng(MAPSEARCHER.GLOBAL.DEFINES.itemMarker.getPosition());
                                            });
                                            //set listener for right click (fixes reset issue over overlays)
                                            google.maps.event.addListener(MAPSEARCHER.GLOBAL.DEFINES.itemMarker, 'rightclick', function () {
                                                MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setDrawingMode(null); //reset drawing manager no matter what
                                            });
                                            MAPSEARCHER.GLOBAL.DEFINES.mainCount++;
                                            MAPSEARCHER.GLOBAL.DEFINES.incomingACL = "item";
                                            break;
                                        case "main":
                                            MAPSEARCHER.GLOBAL.DEFINES.placerType = "item";
                                            MAPSEARCHER.GLOBAL.DEFINES.firstMarker++;
                                            MAPSEARCHER.GLOBAL.DEFINES.itemMarker = new google.maps.Marker({
                                                position: MAPSEARCHER.GLOBAL.DEFINES.incomingPointCenter[i],
                                                map: map,
                                                title: MAPSEARCHER.GLOBAL.DEFINES.incomingPointLabel[i]
                                            });
                                            MAPSEARCHER.GLOBAL.DEFINES.itemMarker.setOptions(MAPSEARCHER.GLOBAL.DEFINES.markerOptionsItem);
                                            document.getElementById('content_toolbox_posItem').value = MAPSEARCHER.GLOBAL.DEFINES.itemMarker.getPosition();
                                            MAPSEARCHER.UTILITIES.codeLatLng(MAPSEARCHER.GLOBAL.DEFINES.itemMarker.getPosition());
                                            google.maps.event.addListener(MAPSEARCHER.GLOBAL.DEFINES.itemMarker, 'dragend', function () {
                                                MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPSEARCHER.ACTIONS.openToolboxTab("item");
                                                MAPSEARCHER.GLOBAL.DEFINES.firstSaveItem = true;
                                                MAPSEARCHER.GLOBAL.DEFINES.savingMarkerCenter = MAPSEARCHER.GLOBAL.DEFINES.itemMarker.getPosition(); //store coords to save
                                                document.getElementById('content_toolbox_posItem').value = MAPSEARCHER.GLOBAL.DEFINES.itemMarker.getPosition();
                                                MAPSEARCHER.UTILITIES.codeLatLng(MAPSEARCHER.GLOBAL.DEFINES.itemMarker.getPosition());
                                            });
                                            //set listener for right click (fixes reset issue over overlays)
                                            google.maps.event.addListener(MAPSEARCHER.GLOBAL.DEFINES.itemMarker, 'rightclick', function () {
                                                MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setDrawingMode(null); //reset drawing manager no matter what
                                            });
                                            MAPSEARCHER.GLOBAL.DEFINES.mainCount++;
                                            MAPSEARCHER.GLOBAL.DEFINES.incomingACL = "item";
                                            break;
                                        case "poi":
                                            MAPSEARCHER.TRACER.addTracer("[INFO]: incoming poi: " + i + " " + MAPSEARCHER.GLOBAL.DEFINES.incomingPointLabel[i]);
                                            var marker = new google.maps.Marker({
                                                position: MAPSEARCHER.GLOBAL.DEFINES.incomingPointCenter[i],
                                                map: map,
                                                title: MAPSEARCHER.GLOBAL.DEFINES.incomingPointLabel[i]
                                            });
                                            marker.setOptions(MAPSEARCHER.GLOBAL.DEFINES.markerOptionsPOI);
                                            MAPSEARCHER.TRACER.addTracer("[INFO]: incoming center: " + marker.getPosition());
                                            MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                            MAPSEARCHER.GLOBAL.DEFINES.poi_i++;
                                            MAPSEARCHER.GLOBAL.DEFINES.label[MAPSEARCHER.GLOBAL.DEFINES.poi_i] = new MarkerWithLabel({
                                                position: marker.getPosition(), //position of real marker
                                                map: map,
                                                zIndex: 2,
                                                labelContent: MAPSEARCHER.GLOBAL.DEFINES.incomingPointLabel[(i)],
                                                labelAnchor: new google.maps.Point(15, 0),
                                                labelClass: "labels", // the CSS class for the label
                                                labelStyle: { opacity: 0.75 },
                                                icon: {} //initialize to nothing so no marker shows
                                            });
                                            MAPSEARCHER.GLOBAL.DEFINES.poiObj[MAPSEARCHER.GLOBAL.DEFINES.poi_i] = marker;
                                            MAPSEARCHER.GLOBAL.DEFINES.poiType[MAPSEARCHER.GLOBAL.DEFINES.poi_i] = "marker";
                                            var poiId = MAPSEARCHER.GLOBAL.DEFINES.poi_i + 1;
                                            var poiDescTemp = MAPSEARCHER.GLOBAL.DEFINES.incomingPointLabel[i];
                                            document.getElementById("poiList").innerHTML += MAPSEARCHER.UTILITIES.writeHTML("poiListItemIncoming", MAPSEARCHER.GLOBAL.DEFINES.poi_i, poiId, poiDescTemp);
                                            MAPSEARCHER.GLOBAL.DEFINES.poiDesc[MAPSEARCHER.GLOBAL.DEFINES.poi_i] = poiDescTemp;
                                            var contentString = MAPSEARCHER.UTILITIES.writeHTML("poiDescIncoming", MAPSEARCHER.GLOBAL.DEFINES.poi_i, poiDescTemp, "");
                                            MAPSEARCHER.GLOBAL.DEFINES.infoWindow[MAPSEARCHER.GLOBAL.DEFINES.poi_i] = new google.maps.InfoWindow({
                                                content: contentString,
                                                position: marker.getPosition()
                                            });
                                            MAPSEARCHER.GLOBAL.DEFINES.infoWindow[MAPSEARCHER.GLOBAL.DEFINES.poi_i].setMap(map);
                                            MAPSEARCHER.GLOBAL.DEFINES.infoWindow[MAPSEARCHER.GLOBAL.DEFINES.poi_i].open(map, MAPSEARCHER.GLOBAL.DEFINES.poiObj[MAPSEARCHER.GLOBAL.DEFINES.poi_i]);
                                            MAPSEARCHER.GLOBAL.DEFINES.poiCount++;
                                            google.maps.event.addListener(marker, 'dragstart', function () {
                                                MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                                MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                                for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    if (MAPSEARCHER.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setMap(null);
                                                        MAPSEARCHER.GLOBAL.DEFINES.label[i].setMap(null);
                                                    }
                                                }
                                            });
                                            google.maps.event.addListener(marker, 'dragend', function () {
                                                MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                                for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    if (MAPSEARCHER.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setOptions({ position: this.getPosition(), pixelOffset: new google.maps.Size(0, -40) });
                                                        MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].open(null);
                                                        MAPSEARCHER.GLOBAL.DEFINES.label[i].setPosition(this.getPosition());
                                                        MAPSEARCHER.GLOBAL.DEFINES.label[i].setMap(map);
                                                    }
                                                }
                                            });
                                            google.maps.event.addListener(marker, 'click', function () {
                                                MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                                MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                                for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    if (MAPSEARCHER.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setOptions({ position: this.getPosition(), pixelOffset: new google.maps.Size(0, -40) });
                                                        MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].open(map);
                                                    }
                                                }
                                            });
                                            //set listener for right click (fixes reset issue over overlays)
                                            google.maps.event.addListener(marker, 'rightclick', function () {
                                                MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setDrawingMode(null); //reset drawing manager no matter what
                                            });
                                            break;
                                    }
                                }
                            } else {
                                //not sure if ever called...
                                MAPSEARCHER.GLOBAL.DEFINES.firstMarker++;
                                MAPSEARCHER.GLOBAL.DEFINES.itemMarker = new google.maps.Marker({
                                    position: map.getCenter(), //just get the center poin of the map
                                    map: null, //hide on load
                                    draggable: false,
                                    title: MAPSEARCHER.GLOBAL.DEFINES.incomingPointLabel[0]
                                });
                                //nothing to display because there is no geolocation of item
                            }
                            //once everything is drawn, determine if there are pois
                            if (MAPSEARCHER.GLOBAL.DEFINES.poiCount > 0) {
                                //close and reopen pois (to fix firefox bug)
                                setTimeout(function () {
                                    MAPSEARCHER.GLOBAL.DEFINES.RIBMode = true;
                                    MAPSEARCHER.ACTIONS.toggleVis("pois");
                                    MAPSEARCHER.ACTIONS.toggleVis("pois");
                                    MAPSEARCHER.GLOBAL.DEFINES.RIBMode = false;
                                    //this hides the MAPSEARCHER.GLOBAL.DEFINES.infoWindows at startup
                                    for (var j = 0; j < MAPSEARCHER.GLOBAL.DEFINES.poiCount; j++) {
                                        MAPSEARCHER.GLOBAL.DEFINES.infoWindow[j].setMap(null);
                                    }
                                }, 1000);
                            }

                            MAPSEARCHER.TRACER.addTracer("[INFO]: displayIncomingPoints completed...");
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    displayIncomingCircles: function () {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: displayIncomingCircles started...");

                            if (MAPSEARCHER.GLOBAL.DEFINES.incomingCircleCenter.length > 0) {
                                for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.incomingCircleCenter.length; i++) {
                                    switch (MAPSEARCHER.GLOBAL.DEFINES.incomingCircleFeatureType[i]) {
                                        case "":
                                            break;
                                        case "main":
                                            break;
                                        case "poi":
                                            MAPSEARCHER.TRACER.addTracer("[INFO]: incoming poi: " + i + " " + MAPSEARCHER.GLOBAL.DEFINES.incomingCircleLabel[i]);
                                            MAPSEARCHER.GLOBAL.DEFINES.placerType = "poi";
                                            var circle = new google.maps.Circle({
                                                center: MAPSEARCHER.GLOBAL.DEFINES.incomingCircleCenter[i],
                                                map: map,
                                                title: MAPSEARCHER.GLOBAL.DEFINES.incomingCircleLabel[i],
                                                radius: MAPSEARCHER.GLOBAL.DEFINES.incomingCircleRadius[i]
                                            });
                                            circle.setOptions(MAPSEARCHER.GLOBAL.DEFINES.circleOptionsPOI);
                                            MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                            MAPSEARCHER.GLOBAL.DEFINES.poi_i++;

                                            MAPSEARCHER.GLOBAL.DEFINES.label[MAPSEARCHER.GLOBAL.DEFINES.poi_i] = new MarkerWithLabel({
                                                position: circle.getCenter(), //position of real marker
                                                zIndex: 2,
                                                map: map,
                                                labelContent: MAPSEARCHER.GLOBAL.DEFINES.incomingCircleLabel[i],
                                                labelAnchor: new google.maps.Point(15, 0),
                                                labelClass: "labels", // the CSS class for the label
                                                labelStyle: { opacity: 0.75 },
                                                icon: {} //initialize to nothing so no marker shows
                                            });

                                            var poiId = MAPSEARCHER.GLOBAL.DEFINES.poi_i + 1;
                                            MAPSEARCHER.GLOBAL.DEFINES.poiObj[MAPSEARCHER.GLOBAL.DEFINES.poi_i] = circle;
                                            MAPSEARCHER.GLOBAL.DEFINES.poiType[MAPSEARCHER.GLOBAL.DEFINES.poi_i] = "circle";
                                            var poiDescTemp = MAPSEARCHER.GLOBAL.DEFINES.incomingCircleLabel[i];
                                            document.getElementById("poiList").innerHTML += MAPSEARCHER.UTILITIES.writeHTML("poiListItemIncoming", MAPSEARCHER.GLOBAL.DEFINES.poi_i, poiId, poiDescTemp);
                                            MAPSEARCHER.GLOBAL.DEFINES.poiDesc[MAPSEARCHER.GLOBAL.DEFINES.poi_i] = poiDescTemp;
                                            var contentString = MAPSEARCHER.UTILITIES.writeHTML("poiDescIncoming", MAPSEARCHER.GLOBAL.DEFINES.poi_i, poiDescTemp, "");
                                            MAPSEARCHER.GLOBAL.DEFINES.infoWindow[MAPSEARCHER.GLOBAL.DEFINES.poi_i] = new google.maps.InfoWindow({
                                                content: contentString
                                            });
                                            MAPSEARCHER.GLOBAL.DEFINES.infoWindow[MAPSEARCHER.GLOBAL.DEFINES.poi_i].setPosition(circle.getCenter());
                                            MAPSEARCHER.GLOBAL.DEFINES.infoWindow[MAPSEARCHER.GLOBAL.DEFINES.poi_i].open(map);
                                            MAPSEARCHER.GLOBAL.DEFINES.poiCount++;

                                            google.maps.event.addListener(circle, 'dragstart', function () {
                                                MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                                MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                                for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    if (MAPSEARCHER.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setMap(null);
                                                        MAPSEARCHER.GLOBAL.DEFINES.label[i].setMap(null);
                                                    }
                                                }
                                            });

                                            google.maps.event.addListener(circle, 'drag', function () {
                                                //used to get the center point for lat/long tool
                                                MAPSEARCHER.GLOBAL.DEFINES.circleCenter = this.getCenter();
                                                var str = this.getCenter().toString();
                                                var cLatV = str.replace("(", "").replace(")", "").split(",", 1);
                                                var cLongV = str.replace(cLatV, "").replace("(", "").replace(")", "").replace(",", ""); //is this better than passing into array?s
                                                if (cLatV.indexOf("-") != 0) {
                                                    latH = "N";
                                                } else {
                                                    latH = "S";
                                                }
                                                if (cLongV.indexOf("-") != 0) {
                                                    longH = "W";
                                                } else {
                                                    longH = "E";
                                                }
                                                document.getElementById("cLat").innerHTML = cLatV + " (" + latH + ")";
                                                document.getElementById("cLong").innerHTML = cLongV + " (" + longH + ")";
                                            });

                                            google.maps.event.addListener(circle, 'dragend', function () {
                                                MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                                for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    if (MAPSEARCHER.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setPosition(this.getCenter());
                                                        MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].open(null);
                                                        MAPSEARCHER.GLOBAL.DEFINES.label[i].setPosition(this.getCenter());
                                                        MAPSEARCHER.GLOBAL.DEFINES.label[i].setMap(map);
                                                    }
                                                }
                                            });

                                            google.maps.event.addListener(circle, 'click', function () {
                                                MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                                for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    if (MAPSEARCHER.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setPosition(this.getCenter());
                                                        MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].open(map);
                                                    }
                                                }
                                            });

                                            google.maps.event.addListener(circle, 'center_changed', function () {
                                                MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                                MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                                for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    if (MAPSEARCHER.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setPosition(circle.getCenter());
                                                        MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].open(null);
                                                        MAPSEARCHER.GLOBAL.DEFINES.label[i].setPosition(circle.getCenter());
                                                        MAPSEARCHER.GLOBAL.DEFINES.label[i].setMap(map);
                                                    }
                                                }
                                            });

                                            google.maps.event.addListener(circle, 'radius_changed', function () {
                                                MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                            });

                                            //set listener for right click (fixes reset issue over overlays)
                                            google.maps.event.addListener(circle, 'rightclick', function () {
                                                MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setDrawingMode(null); //reset drawing manager no matter what
                                                //MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setMap(null);
                                            });

                                            break;
                                    }
                                }
                            } else {
                                //nothing
                            }
                            //once everything is drawn, determine if there are pois
                            if (MAPSEARCHER.GLOBAL.DEFINES.poiCount > 0) {
                                //close and reopen pois (to fix firefox bug)
                                setTimeout(function () {
                                    MAPSEARCHER.GLOBAL.DEFINES.RIBMode = true;
                                    MAPSEARCHER.ACTIONS.toggleVis("pois");
                                    MAPSEARCHER.ACTIONS.toggleVis("pois");
                                    MAPSEARCHER.GLOBAL.DEFINES.RIBMode = false;
                                    //this hides the MAPSEARCHER.GLOBAL.DEFINES.infoWindows at startup
                                    for (var j = 0; j < MAPSEARCHER.GLOBAL.DEFINES.poiCount; j++) {
                                        MAPSEARCHER.GLOBAL.DEFINES.infoWindow[j].setMap(null);
                                    }
                                }, 1000);
                            }

                            MAPSEARCHER.TRACER.addTracer("[INFO]: displayIncomingCircles completed...");
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    displayIncomingLines: function () {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: DisplayIncomingLines started...");
                            if (MAPSEARCHER.GLOBAL.DEFINES.incomingLinePath.length > 0) {
                                for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.incomingLinePath.length; i++) {
                                    switch (MAPSEARCHER.GLOBAL.DEFINES.incomingLineFeatureType[i]) {
                                        case "":
                                            break;
                                        case "main":
                                            break;
                                        case "poi":
                                            MAPSEARCHER.TRACER.addTracer("[INFO]: incoming poi: " + i + " " + MAPSEARCHER.GLOBAL.DEFINES.incomingLineLabel[i]);
                                            var polyline = new google.maps.Polyline({
                                                path: MAPSEARCHER.GLOBAL.DEFINES.incomingLinePath[i],
                                                map: map,
                                                title: MAPSEARCHER.GLOBAL.DEFINES.incomingLineLabel[i]
                                            });

                                            polyline.setOptions(MAPSEARCHER.GLOBAL.DEFINES.polylineOptionsPOI);
                                            MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                            MAPSEARCHER.GLOBAL.DEFINES.poi_i++;
                                            var poiId = MAPSEARCHER.GLOBAL.DEFINES.poi_i + 1;
                                            MAPSEARCHER.GLOBAL.DEFINES.poiObj[MAPSEARCHER.GLOBAL.DEFINES.poi_i] = polyline;
                                            MAPSEARCHER.GLOBAL.DEFINES.poiType[MAPSEARCHER.GLOBAL.DEFINES.poi_i] = "polyline";
                                            var poiDescTemp = MAPSEARCHER.GLOBAL.DEFINES.incomingLineLabel[i];
                                            document.getElementById("poiList").innerHTML += MAPSEARCHER.UTILITIES.writeHTML("poiListItemIncoming", MAPSEARCHER.GLOBAL.DEFINES.poi_i, poiId, poiDescTemp);
                                            MAPSEARCHER.GLOBAL.DEFINES.poiDesc[MAPSEARCHER.GLOBAL.DEFINES.poi_i] = poiDescTemp;
                                            var contentString = MAPSEARCHER.UTILITIES.writeHTML("poiDescIncoming", MAPSEARCHER.GLOBAL.DEFINES.poi_i, poiDescTemp, "");
                                            MAPSEARCHER.GLOBAL.DEFINES.infoWindow[MAPSEARCHER.GLOBAL.DEFINES.poi_i] = new google.maps.InfoWindow({
                                                content: contentString
                                            });
                                            var polylinePoints = [];
                                            var polylinePointCount = 0;
                                            polyline.getPath().forEach(function (latLng) {
                                                polylinePoints[polylinePointCount] = latLng;
                                                MAPSEARCHER.TRACER.addTracer("[INFO]: polylinePoints[" + polylinePointCount + "] = " + latLng);
                                                polylinePointCount++;
                                            });
                                            MAPSEARCHER.TRACER.addTracer("[INFO]: polylinePointCount: " + polylinePointCount);
                                            MAPSEARCHER.TRACER.addTracer("[INFO]: polylinePoints.length: " + polylinePoints.length);
                                            MAPSEARCHER.TRACER.addTracer("[INFO]: Math.round((polylinePoints.length / 2)): " + Math.round((polylinePoints.length / 2)));
                                            var polylineCenterPoint = polylinePoints[Math.round((polylinePoints.length / 2))];
                                            MAPSEARCHER.TRACER.addTracer("[INFO]: polylineCenterPoint: " + polylineCenterPoint);
                                            var polylineStartPoint = polylinePoints[0];
                                            MAPSEARCHER.TRACER.addTracer("[INFO]: polylineStartPoint: " + polylineStartPoint);
                                            MAPSEARCHER.GLOBAL.DEFINES.infoWindow[MAPSEARCHER.GLOBAL.DEFINES.poi_i].setPosition(polylineStartPoint);
                                            MAPSEARCHER.GLOBAL.DEFINES.infoWindow[MAPSEARCHER.GLOBAL.DEFINES.poi_i].open(map);

                                            //best fix so far
                                            if (MAPSEARCHER.GLOBAL.DEFINES.poiCount == 0) {
                                                setTimeout(function () {
                                                    MAPSEARCHER.GLOBAL.DEFINES.infoWindow[0].setMap(null);
                                                    MAPSEARCHER.GLOBAL.DEFINES.infoWindow[0].setMap(map);
                                                }, 800);
                                            }

                                            MAPSEARCHER.GLOBAL.DEFINES.poiCount++;
                                            MAPSEARCHER.GLOBAL.DEFINES.label[MAPSEARCHER.GLOBAL.DEFINES.poi_i] = new MarkerWithLabel({
                                                position: polylineStartPoint, //position at start of polyline
                                                zIndex: 2,
                                                map: map,
                                                labelContent: MAPSEARCHER.GLOBAL.DEFINES.incomingLineLabel[i],
                                                labelAnchor: new google.maps.Point(15, 0),
                                                labelClass: "labels", // the CSS class for the label
                                                labelStyle: { opacity: 0.75 },
                                                icon: {} //initialize to nothing so no marker shows
                                            });

                                            google.maps.event.addListener(polyline, 'mouseout', function () {
                                                MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                                for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    if (MAPSEARCHER.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        var polylinePoints = [];
                                                        var polylinePointCount = 0;
                                                        MAPSEARCHER.TRACER.addTracer("[INFO]: here1");
                                                        this.getPath().forEach(function (latLng) {
                                                            polylinePoints[polylinePointCount] = latLng;
                                                            polylinePointCount++;
                                                        });
                                                        MAPSEARCHER.TRACER.addTracer("[INFO]: here2");
                                                        var polylineCenterPoint = polylinePoints[(polylinePoints.length / 2)];
                                                        var polylineStartPoint = polylinePoints[0];
                                                        MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setPosition(polylineStartPoint);
                                                        MAPSEARCHER.GLOBAL.DEFINES.label[i].setPosition(polylineStartPoint);
                                                        MAPSEARCHER.TRACER.addTracer("[INFO]: here3");
                                                    }
                                                }
                                            });

                                            google.maps.event.addListener(polyline, 'dragstart', function () {
                                                MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                                MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                                for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    if (MAPSEARCHER.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setMap(null);
                                                        MAPSEARCHER.GLOBAL.DEFINES.label[i].setMap(null);
                                                    }
                                                }
                                            });

                                            google.maps.event.addListener(polyline, 'drag', function () {
                                                //used for lat/long tooll
                                                var bounds = new google.maps.LatLngBounds;
                                                this.getPath().forEach(function (latLng) { bounds.extend(latLng); });
                                                var polylineCenter = bounds.getCenter();
                                                var str = polylineCenter.toString();
                                                var cLatV = str.replace("(", "").replace(")", "").split(",", 1);
                                                var cLongV = str.replace(cLatV, "").replace("(", "").replace(")", "").replace(",", ""); //is this better than passing into array?s
                                                if (cLatV.indexOf("-") != 0) {
                                                    latH = "N";
                                                } else {
                                                    latH = "S";
                                                }
                                                if (cLongV.indexOf("-") != 0) {
                                                    longH = "W";
                                                } else {
                                                    longH = "E";
                                                }
                                                document.getElementById("cLat").innerHTML = cLatV + " (" + latH + ")";
                                                document.getElementById("cLong").innerHTML = cLongV + " (" + longH + ")";
                                            });

                                            google.maps.event.addListener(polyline, 'dragend', function () {
                                                MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                                //var bounds = new google.maps.LatLngBounds;
                                                //polyline.getPath().forEach(function (latLng) { bounds.extend(latLng); });
                                                //var polylineCenter = bounds.getCenter();
                                                //var bounds = new google.maps.LatLngBounds; //spatial center, bounds holder

                                                for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    if (MAPSEARCHER.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        var polylinePoints = [];
                                                        var polylinePointCount = 0;
                                                        this.getPath().forEach(function (latLng) {
                                                            polylinePoints[polylinePointCount] = latLng;
                                                            polylinePointCount++;
                                                        });
                                                        var polylineCenterPoint = polylinePoints[(polylinePoints.length / 2)];
                                                        var polylineStartPoint = polylinePoints[0];
                                                        MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setPosition(polylineStartPoint);
                                                        MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].open(null);
                                                        MAPSEARCHER.GLOBAL.DEFINES.label[i].setPosition(polylineStartPoint);
                                                        MAPSEARCHER.GLOBAL.DEFINES.label[i].setMap(map);
                                                    }
                                                }
                                            });

                                            google.maps.event.addListener(polyline, 'click', function () {
                                                MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                                MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                                //var bounds = new google.maps.LatLngBounds;
                                                //polyline.getPath().forEach(function (latLng) { bounds.extend(latLng); });
                                                //var polylineCenter = bounds.getCenter();
                                                for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    if (MAPSEARCHER.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        var polylinePoints = [];
                                                        var polylinePointCount = 0;
                                                        this.getPath().forEach(function (latLng) {
                                                            polylinePoints[polylinePointCount] = latLng;
                                                            polylinePointCount++;
                                                        });
                                                        var polylineCenterPoint = polylinePoints[(polylinePoints.length / 2)];
                                                        var polylineStartPoint = polylinePoints[0];
                                                        MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setPosition(polylineStartPoint);
                                                        MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].open(map);
                                                    }
                                                }
                                            });

                                            google.maps.event.addListener(polyline.getPath(), 'set_at', function () {
                                                MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                                MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true; //2do what does this do? why is it important?
                                                for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    MAPSEARCHER.TRACER.addTracer("[INFO]: inside loop1");
                                                    if (MAPSEARCHER.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        //var bounds = new google.maps.LatLngBounds;
                                                        //polyline.getPath().forEach(function (latLng) { bounds.extend(latLng); });
                                                        //var polylineCenter = bounds.getCenter();
                                                        //var bounds = new google.maps.LatLngBounds; //spatial center, bounds holder
                                                        var polylinePoints = [];
                                                        var polylinePointCount = 0;
                                                        MAPSEARCHER.TRACER.addTracer("[INFO]: here1");
                                                        polyline.getPath().forEach(function (latLng) {
                                                            polylinePoints[polylinePointCount] = latLng;
                                                            polylinePointCount++;
                                                        });
                                                        MAPSEARCHER.TRACER.addTracer("[INFO]: here2");
                                                        var polylineCenterPoint = polylinePoints[(polylinePoints.length / 2)];
                                                        var polylineStartPoint = polylinePoints[0];
                                                        MAPSEARCHER.GLOBAL.DEFINES.infoWindow[MAPSEARCHER.GLOBAL.DEFINES.poi_i].setPosition(polylineStartPoint);
                                                        MAPSEARCHER.GLOBAL.DEFINES.infoWindow[MAPSEARCHER.GLOBAL.DEFINES.poi_i].open(null);
                                                        MAPSEARCHER.GLOBAL.DEFINES.label[i].setPosition(polylineStartPoint);
                                                        MAPSEARCHER.GLOBAL.DEFINES.label[i].setMap(map);
                                                        MAPSEARCHER.TRACER.addTracer("[INFO]: here3");
                                                    }
                                                }
                                            });

                                            //set listener for right click (fixes reset issue over overlays)
                                            google.maps.event.addListener(polyline, 'rightclick', function () {
                                                MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setDrawingMode(null); //reset drawing manager no matter what
                                                //MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setMap(null);
                                            });

                                            break;
                                    }
                                }

                            } else {
                                //nothing
                            }
                            //once everything is drawn, determine if there are pois
                            if (MAPSEARCHER.GLOBAL.DEFINES.poiCount > 0) {
                                //close and reopen pois (to fix firefox bug)
                                setTimeout(function () {
                                    MAPSEARCHER.GLOBAL.DEFINES.RIBMode = true;
                                    MAPSEARCHER.ACTIONS.toggleVis("pois");
                                    MAPSEARCHER.ACTIONS.toggleVis("pois");
                                    MAPSEARCHER.GLOBAL.DEFINES.RIBMode = false;
                                    //this hides the MAPSEARCHER.GLOBAL.DEFINES.infoWindows at startup
                                    for (var j = 0; j < MAPSEARCHER.GLOBAL.DEFINES.poiCount; j++) {
                                        MAPSEARCHER.GLOBAL.DEFINES.infoWindow[j].setMap(null);
                                    }
                                }, 1000);
                            }
                            MAPSEARCHER.TRACER.addTracer("[INFO]: DisplayIncomingLines completed...");
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    displayIncomingPolygons: function () {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: displayIncomingPolygons started...");
                            MAPSEARCHER.TRACER.addTracer("[INFO]: length: " + MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonFeatureType.length);
                            for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonFeatureType.length; i++) {
                                MAPSEARCHER.TRACER.addTracer("[INFO]: ft: " + MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonFeatureType[i]);
                                if (MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonFeatureType[i] == "TEMP_main") {
                                    //hidden do nothing
                                    MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonFeatureType[i] = "hidden";
                                    MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonPolygonType[i] = "hidden";
                                    MAPSEARCHER.TRACER.addTracer("[INFO]: converting TEMP_ for " + i);
                                }
                                switch (MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonFeatureType[i]) {
                                    case "hidden":
                                        //hidden do nothing
                                        MAPSEARCHER.TRACER.addTracer("[INFO]: doing nothing for " + i);
                                        break;
                                    case "":
                                        MAPSEARCHER.TRACER.addTracer("[INFO]: doing case2 for " + i);
                                        MAPSEARCHER.GLOBAL.DEFINES.workingOverlayIndex = MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonPageId[i];
                                        //create overlay with incoming
                                        MAPSEARCHER.GLOBAL.DEFINES.overlaysOnMap[MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonPageId[i]] = new MAPSEARCHER.UTILITIES.CustomOverlay(MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonPageId[i], MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonPath[i], MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonSourceURL[i], map, MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonRotation[i]);
                                        MAPSEARCHER.GLOBAL.DEFINES.currentlyEditing = "no";
                                        //set the overlay to the map
                                        MAPSEARCHER.GLOBAL.DEFINES.overlaysOnMap[MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonPageId[i]].setMap(map);
                                        //MAPSEARCHER.UTILITIES.keepRotate(MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonRotation[i]);
                                        //set hotspot on top of overlay
                                        MAPSEARCHER.UTILITIES.setGhostOverlay(MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonPageId[i], MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonPath[i]);
                                        MAPSEARCHER.TRACER.addTracer("[INFO]: I created ghost: " + MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonPageId[i]);
                                        MAPSEARCHER.GLOBAL.DEFINES.mainCount++;
                                        MAPSEARCHER.GLOBAL.DEFINES.incomingACL = "overlay";
                                        MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setDrawingMode(null); //reset drawing manager no matter what
                                        //MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setMap(null);
                                        MAPSEARCHER.GLOBAL.DEFINES.overlaysCurrentlyDisplayed = true;
                                        break;
                                    case "main":
                                        MAPSEARCHER.TRACER.addTracer("[INFO]: doing case3 for " + i);
                                        MAPSEARCHER.GLOBAL.DEFINES.workingOverlayIndex = MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonPageId[i];
                                        //create overlay with incoming
                                        MAPSEARCHER.GLOBAL.DEFINES.overlaysOnMap[MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonPageId[i]] = new MAPSEARCHER.UTILITIES.CustomOverlay(MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonPageId[i], MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonPath[i], MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonSourceURL[i], map, MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonRotation[i]);
                                        MAPSEARCHER.GLOBAL.DEFINES.currentlyEditing = "no";
                                        //set the overlay to the map
                                        MAPSEARCHER.GLOBAL.DEFINES.overlaysOnMap[MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonPageId[i]].setMap(map);
                                        //MAPSEARCHER.UTILITIES.keepRotate(MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonRotation[i]);
                                        //set hotspot on top of overlay
                                        MAPSEARCHER.UTILITIES.setGhostOverlay(MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonPageId[i], MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonPath[i]);
                                        MAPSEARCHER.TRACER.addTracer("[INFO]: I created ghost: " + MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonPageId[i]);
                                        MAPSEARCHER.GLOBAL.DEFINES.mainCount++;
                                        MAPSEARCHER.GLOBAL.DEFINES.incomingACL = "overlay";
                                        MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setDrawingMode(null); //reset drawing manager no matter what
                                        //MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setMap(null);
                                        MAPSEARCHER.GLOBAL.DEFINES.overlaysCurrentlyDisplayed = true;
                                        break;
                                    case "poi":
                                        MAPSEARCHER.TRACER.addTracer("[INFO]: doing case4 for " + i);
                                        //determine polygon type
                                        if (MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonPolygonType[i] == "rectangle") {
                                            MAPSEARCHER.TRACER.addTracer("[INFO]: incoming poi: " + i + " " + MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonLabel[i]);
                                            MAPSEARCHER.TRACER.addTracer("[INFO]: detected incoming rectangle");
                                            //convert path to a rectangle bounds

                                            var pathCount = 0;
                                            var polygon = new google.maps.Polygon({
                                                paths: MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonPath[i]
                                            });

                                            polygon.getPath().forEach(function () { pathCount++; });
                                            if (pathCount == 2) {
                                                MAPSEARCHER.TRACER.addTracer("[INFO]: pathcount: " + pathCount);
                                                var l = [5];
                                                var lcount = 1;
                                                polygon.getPath().forEach(function (latLng) {
                                                    var newLatLng = String(latLng).split(",");
                                                    var lat = newLatLng[0].replace("(", "");
                                                    var lng = newLatLng[1].replace(")", "");
                                                    l[lcount] = lat;
                                                    lcount++;
                                                    l[lcount] = lng;
                                                    lcount++;
                                                });
                                                MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonPath[i] = new google.maps.LatLngBounds(new google.maps.LatLng(l[3], l[2]), new google.maps.LatLng(l[1], l[4]));
                                                //rectangle.setBounds([new google.maps.LatLng(l[1], l[4]), new google.maps.LatLng(l[3], l[4]), new google.maps.LatLng(l[3], l[2]), new google.maps.LatLng(l[1], l[2])]);
                                            }

                                            var rectangle = new google.maps.Rectangle({
                                                bounds: MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonPath[i],
                                                map: map,
                                                title: MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonLabel[i]
                                            });

                                            rectangle.setOptions(MAPSEARCHER.GLOBAL.DEFINES.rectangleOptionsPOI);
                                            MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                            MAPSEARCHER.GLOBAL.DEFINES.poi_i++;
                                            MAPSEARCHER.GLOBAL.DEFINES.label[MAPSEARCHER.GLOBAL.DEFINES.poi_i] = new MarkerWithLabel({
                                                position: rectangle.getBounds().getCenter(), //position of real marker
                                                zIndex: 2,
                                                map: map,
                                                labelContent: MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonLabel[i],
                                                labelAnchor: new google.maps.Point(15, 0),
                                                labelClass: "labels", // the CSS class for the label
                                                labelStyle: { opacity: 0.75 },
                                                icon: {} //initialize to nothing so no marker shows
                                            });

                                            var poiId = MAPSEARCHER.GLOBAL.DEFINES.poi_i + 1;
                                            MAPSEARCHER.GLOBAL.DEFINES.poiObj[MAPSEARCHER.GLOBAL.DEFINES.poi_i] = rectangle;
                                            MAPSEARCHER.GLOBAL.DEFINES.poiType[MAPSEARCHER.GLOBAL.DEFINES.poi_i] = "rectangle";
                                            var poiDescTemp = MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonLabel[i];
                                            document.getElementById("poiList").innerHTML += MAPSEARCHER.UTILITIES.writeHTML("poiListItemIncoming", MAPSEARCHER.GLOBAL.DEFINES.poi_i, poiId, poiDescTemp);
                                            MAPSEARCHER.GLOBAL.DEFINES.poiDesc[MAPSEARCHER.GLOBAL.DEFINES.poi_i] = poiDescTemp;
                                            var contentString = MAPSEARCHER.UTILITIES.writeHTML("poiDescIncoming", MAPSEARCHER.GLOBAL.DEFINES.poi_i, poiDescTemp, "");
                                            MAPSEARCHER.GLOBAL.DEFINES.infoWindow[MAPSEARCHER.GLOBAL.DEFINES.poi_i] = new google.maps.InfoWindow({
                                                content: contentString
                                            });

                                            MAPSEARCHER.GLOBAL.DEFINES.infoWindow[MAPSEARCHER.GLOBAL.DEFINES.poi_i].setPosition(rectangle.getBounds().getCenter());
                                            MAPSEARCHER.GLOBAL.DEFINES.infoWindow[MAPSEARCHER.GLOBAL.DEFINES.poi_i].open(map);
                                            //best fix so far
                                            if (MAPSEARCHER.GLOBAL.DEFINES.poiCount == 0) {
                                                setTimeout(function () {
                                                    MAPSEARCHER.GLOBAL.DEFINES.infoWindow[0].setMap(null);
                                                    MAPSEARCHER.GLOBAL.DEFINES.infoWindow[0].setMap(map);
                                                }, 800);
                                            }

                                            MAPSEARCHER.GLOBAL.DEFINES.poiCount++;

                                            google.maps.event.addListener(rectangle, 'bounds_changed', function () {
                                                MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                                MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                                for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    if (MAPSEARCHER.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setPosition(this.getBounds().getCenter());
                                                        MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setMap(null);
                                                        MAPSEARCHER.GLOBAL.DEFINES.label[i].setPosition(this.getBounds().getCenter());
                                                        MAPSEARCHER.GLOBAL.DEFINES.label[i].setMap(map);
                                                    }
                                                }
                                            });

                                            google.maps.event.addListener(rectangle, 'dragstart', function () {
                                                MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                                MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                                for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    if (MAPSEARCHER.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setMap(null);
                                                        MAPSEARCHER.GLOBAL.DEFINES.label[i].setMap(null);
                                                    }
                                                }
                                            });

                                            google.maps.event.addListener(rectangle, 'drag', function () {
                                                MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                                for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    if (MAPSEARCHER.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setMap(null);
                                                        MAPSEARCHER.GLOBAL.DEFINES.label[i].setMap(null);
                                                    }
                                                }
                                                //used to get center point for lat/long tool
                                                var str = this.getBounds().getCenter().toString();
                                                var cLatV = str.replace("(", "").replace(")", "").split(",", 1);
                                                var cLongV = str.replace(cLatV, "").replace("(", "").replace(")", "").replace(",", "");
                                                if (cLatV.indexOf("-") != 0) {
                                                    latH = "N";
                                                } else {
                                                    latH = "S";
                                                }
                                                if (cLongV.indexOf("-") != 0) {
                                                    longH = "W";
                                                } else {
                                                    longH = "E";
                                                }
                                                document.getElementById("cLat").innerHTML = cLatV + " (" + latH + ")";
                                                document.getElementById("cLong").innerHTML = cLongV + " (" + longH + ")";
                                            });

                                            google.maps.event.addListener(rectangle, 'dragend', function () {
                                                MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                                for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    if (MAPSEARCHER.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setPosition(this.getBounds().getCenter());
                                                        MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].open(null);
                                                        MAPSEARCHER.GLOBAL.DEFINES.label[i].setPosition(this.getBounds().getCenter());
                                                        MAPSEARCHER.GLOBAL.DEFINES.label[i].setMap(map);
                                                    }
                                                }
                                            });

                                            google.maps.event.addListener(rectangle, 'click', function () {
                                                MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                                MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                                for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    if (MAPSEARCHER.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setPosition(this.getBounds().getCenter());
                                                        MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].open(map);
                                                    }
                                                }
                                            });

                                            //set listener for right click (fixes reset issue over overlays)
                                            google.maps.event.addListener(rectangle, 'rightclick', function () {
                                                MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setDrawingMode(null); //reset drawing manager no matter what
                                                //MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setMap(null);
                                            });

                                        } else {//not a rectangle, it is a polygon poi

                                            var polygon = new google.maps.Polygon({
                                                paths: MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonPath[i],
                                                map: map,
                                                title: MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonLabel[i]
                                            });

                                            polygon.setOptions(MAPSEARCHER.GLOBAL.DEFINES.polygonOptionsPOI);

                                            MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                            MAPSEARCHER.GLOBAL.DEFINES.poi_i++;
                                            MAPSEARCHER.GLOBAL.DEFINES.label[MAPSEARCHER.GLOBAL.DEFINES.poi_i] = new MarkerWithLabel({
                                                position: MAPSEARCHER.UTILITIES.polygonCenter(polygon), //position of real marker
                                                zIndex: 2,
                                                map: map,
                                                labelContent: MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonLabel[i], //the current user count
                                                labelAnchor: new google.maps.Point(15, 0),
                                                labelClass: "labels", // the CSS class for the label
                                                labelStyle: { opacity: 0.75 },
                                                icon: {} //initialize to nothing so no marker shows
                                            });

                                            var poiId = MAPSEARCHER.GLOBAL.DEFINES.poi_i + 1;
                                            MAPSEARCHER.GLOBAL.DEFINES.poiObj[MAPSEARCHER.GLOBAL.DEFINES.poi_i] = polygon;
                                            MAPSEARCHER.GLOBAL.DEFINES.poiType[MAPSEARCHER.GLOBAL.DEFINES.poi_i] = "polygon";
                                            var poiDescTemp = MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonLabel[i];
                                            document.getElementById("poiList").innerHTML += MAPSEARCHER.UTILITIES.writeHTML("poiListItemIncoming", MAPSEARCHER.GLOBAL.DEFINES.poi_i, poiId, poiDescTemp);
                                            MAPSEARCHER.GLOBAL.DEFINES.poiDesc[MAPSEARCHER.GLOBAL.DEFINES.poi_i] = poiDescTemp;
                                            var contentString = MAPSEARCHER.UTILITIES.writeHTML("poiDescIncoming", MAPSEARCHER.GLOBAL.DEFINES.poi_i, poiDescTemp, "");
                                            MAPSEARCHER.GLOBAL.DEFINES.infoWindow[MAPSEARCHER.GLOBAL.DEFINES.poi_i] = new google.maps.InfoWindow({
                                                content: contentString
                                            });

                                            MAPSEARCHER.GLOBAL.DEFINES.infoWindow[MAPSEARCHER.GLOBAL.DEFINES.poi_i].setPosition(MAPSEARCHER.UTILITIES.polygonCenter(polygon));
                                            MAPSEARCHER.GLOBAL.DEFINES.infoWindow[MAPSEARCHER.GLOBAL.DEFINES.poi_i].open(map);

                                            //best fix so far
                                            if (MAPSEARCHER.GLOBAL.DEFINES.poiCount == 0) {
                                                setTimeout(function () {
                                                    MAPSEARCHER.GLOBAL.DEFINES.infoWindow[0].setMap(null);
                                                    MAPSEARCHER.GLOBAL.DEFINES.infoWindow[0].setMap(map);
                                                }, 800);
                                            }

                                            MAPSEARCHER.GLOBAL.DEFINES.poiCount++;

                                            google.maps.event.addListener(polygon, 'mouseout', function () { //if bounds change
                                                MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                                for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    if (MAPSEARCHER.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setPosition(MAPSEARCHER.UTILITIES.polygonCenter(this));
                                                        MAPSEARCHER.GLOBAL.DEFINES.label[i].setPosition(MAPSEARCHER.UTILITIES.polygonCenter(this));
                                                    }
                                                }
                                            });

                                            google.maps.event.addListener(rectangle, 'bounds_changed', function () {
                                                MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                                MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                                for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    if (MAPSEARCHER.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setPosition(this.getBounds().getCenter());
                                                        MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setMap(null);
                                                        MAPSEARCHER.GLOBAL.DEFINES.label[i].setPosition(this.getBounds().getCenter());
                                                        MAPSEARCHER.GLOBAL.DEFINES.label[i].setMap(map);
                                                    }
                                                }
                                            });

                                            google.maps.event.addListener(polygon, 'dragstart', function () {
                                                MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                                MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                                for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    if (MAPSEARCHER.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setMap(null);
                                                        MAPSEARCHER.GLOBAL.DEFINES.label[i].setMap(null);
                                                    }
                                                }
                                            });

                                            google.maps.event.addListener(polygon, 'drag', function () {
                                                MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                                for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    if (MAPSEARCHER.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setMap(null);
                                                        MAPSEARCHER.GLOBAL.DEFINES.label[i].setMap(null);
                                                    }
                                                }
                                                //used for lat/long tool
                                                var str = MAPSEARCHER.UTILITIES.polygonCenter(this).toString();
                                                var cLatV = str.replace("(", "").replace(")", "").split(",", 1);
                                                var cLongV = str.replace(cLatV, "").replace("(", "").replace(")", "").replace(",", ""); //is this better than passing into array?s
                                                if (cLatV.indexOf("-") != 0) {
                                                    latH = "N";
                                                } else {
                                                    latH = "S";
                                                }
                                                if (cLongV.indexOf("-") != 0) {
                                                    longH = "W";
                                                } else {
                                                    longH = "E";
                                                }
                                                document.getElementById("cLat").innerHTML = cLatV + " (" + latH + ")";
                                                document.getElementById("cLong").innerHTML = cLongV + " (" + longH + ")";
                                            });

                                            google.maps.event.addListener(polygon, 'dragend', function () {
                                                MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                                for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    if (MAPSEARCHER.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setPosition(MAPSEARCHER.UTILITIES.polygonCenter(this));
                                                        MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].open(null);
                                                        MAPSEARCHER.GLOBAL.DEFINES.label[i].setPosition(MAPSEARCHER.UTILITIES.polygonCenter(this));
                                                        MAPSEARCHER.GLOBAL.DEFINES.label[i].setMap(map);
                                                    }
                                                }
                                            });

                                            google.maps.event.addListener(polygon, 'click', function () {
                                                MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                                MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                                for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    if (MAPSEARCHER.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setPosition(MAPSEARCHER.UTILITIES.polygonCenter(this));
                                                        MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].open(map);
                                                    }
                                                }
                                            });

                                            google.maps.event.addListener(polygon.getPath(), 'set_at', function () {
                                                MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                                MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                                for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    MAPSEARCHER.TRACER.addTracer("[INFO]: inside loop1");
                                                    if (MAPSEARCHER.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        //var bounds = new google.maps.LatLngBounds;
                                                        //polygon.getPath().forEach(function (latLng) { bounds.extend(latLng); });
                                                        //var polygonCenter = bounds.getCenter();
                                                        //var bounds = new google.maps.LatLngBounds; //spatial center, bounds holder
                                                        var polygonPoints = [];
                                                        var polygonPointCount = 0;
                                                        MAPSEARCHER.TRACER.addTracer("[INFO]: here1");
                                                        polygon.getPath().forEach(function (latLng) {
                                                            polygonPoints[polygonPointCount] = latLng;
                                                            polygonPointCount++;
                                                        });
                                                        MAPSEARCHER.TRACER.addTracer("[INFO]: here2");
                                                        var polygonCenterPoint = polygonPoints[(polygonPoints.length / 2)];
                                                        var polygonStartPoint = polygonPoints[0];
                                                        MAPSEARCHER.GLOBAL.DEFINES.infoWindow[MAPSEARCHER.GLOBAL.DEFINES.poi_i].setPosition(polygonCenterPoint);
                                                        MAPSEARCHER.GLOBAL.DEFINES.infoWindow[MAPSEARCHER.GLOBAL.DEFINES.poi_i].open(null);
                                                        MAPSEARCHER.GLOBAL.DEFINES.label[i].setPosition(polygonCenterPoint);
                                                        MAPSEARCHER.GLOBAL.DEFINES.label[i].setMap(map);
                                                        MAPSEARCHER.TRACER.addTracer("[INFO]: here3");
                                                    }
                                                }
                                            });

                                            //set listener for right click (fixes reset issue over overlays)
                                            google.maps.event.addListener(polygon, 'rightclick', function () {
                                                MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setDrawingMode(null); //reset drawing manager no matter what
                                                //MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setMap(null);
                                            });

                                        }
                                        MAPSEARCHER.GLOBAL.DEFINES.overlaysCurrentlyDisplayed = true;
                                        //once everything is drawn, determine if there are pois
                                        if (MAPSEARCHER.GLOBAL.DEFINES.poiCount > 0) {
                                            //close and reopen pois (to fix firefox bug)
                                            setTimeout(function () {
                                                MAPSEARCHER.GLOBAL.DEFINES.RIBMode = true;
                                                MAPSEARCHER.ACTIONS.toggleVis("pois");
                                                MAPSEARCHER.ACTIONS.toggleVis("pois");
                                                MAPSEARCHER.GLOBAL.DEFINES.RIBMode = false;
                                                //this hides the MAPSEARCHER.GLOBAL.DEFINES.infoWindows at startup
                                                for (var j = 0; j < MAPSEARCHER.GLOBAL.DEFINES.poiCount; j++) {
                                                    MAPSEARCHER.GLOBAL.DEFINES.infoWindow[j].setMap(null);
                                                }
                                            }, 1000);
                                        }
                                        break;
                                }
                            }
                            MAPSEARCHER.TRACER.addTracer("[INFO]: displayIncomingPolygons completed...");
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    initDeclarations: function () {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: initDeclarations started...");
                            //get and set c# vars to js  
                            initServerToClientVars();
                            //reinit debug time
                            if (MAPSEARCHER.GLOBAL.DEFINES.debuggerOn) {
                                MAPSEARCHER.TRACER.DEFINES.debugVersionNumber = " (last build: " + MAPSEARCHER.GLOBAL.DEFINES.debugBuildTimeStamp + ") ";
                            } else {
                                MAPSEARCHER.TRACER.DEFINES.debugVersionNumber = " (v1." + MAPSEARCHER.GLOBAL.DEFINES.debugUnixTimeStamp + ") ";
                            }
                            MAPSEARCHER.TRACER.addTracer("[INFO]: initDeclarations completed...");
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    initLocalization: function () {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: initlocalization started...");
                            //localize tooltips (listeners)
                            MAPSEARCHER.LOCALIZATION.localizeByTooltips();
                            //localize by textual content
                            MAPSEARCHER.LOCALIZATION.localizeByText();
                            MAPSEARCHER.TRACER.addTracer("[INFO]: initLocalization completed...");
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    initListeners: function () {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: initListeners started...");
                            //menubar
                            document.getElementById("content_menubar_save").addEventListener("click", function () {
                                //MAPSEARCHER.ACTIONS.save("all");
                                if (MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData) {
                                    //attempt to save all three
                                    MAPSEARCHER.UTILITIES.displayMessage(MAPSEARCHER.LOCALIZATION.DEFINES.L59);
                                    //saveMSG
                                    MAPSEARCHER.GLOBAL.DEFINES.RIBMode = true;
                                    var savesCompleted = 0;
                                    try {
                                        MAPSEARCHER.ACTIONS.save("item");
                                        savesCompleted++;
                                    } catch (e) {
                                        MAPSEARCHER.TRACER.addTracer("[INFO]: could not save item");
                                    }
                                    try {
                                        MAPSEARCHER.ACTIONS.save("overlay");
                                        savesCompleted++;
                                    } catch (e) {
                                        MAPSEARCHER.TRACER.addTracer("[INFO]: could not save overlay");
                                    }
                                    try {
                                        MAPSEARCHER.ACTIONS.save("poi");
                                        savesCompleted++;
                                    } catch (e) {
                                        MAPSEARCHER.TRACER.addTracer("[INFO]: could not save poi");
                                    }
                                    if (savesCompleted == 3) {
                                        MAPSEARCHER.TRACER.addTracer("[INFO]: all saves completed");
                                        MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = false;
                                    }
                                    MAPSEARCHER.GLOBAL.DEFINES.RIBMode = false;
                                } else {
                                    window.location.assign(document.URL.replace("/mapedit", ""));
                                }
                            }, false);
                            document.getElementById("content_menubar_cancel").addEventListener("click", function () {
                                MAPSEARCHER.UTILITIES.displayMessage(MAPSEARCHER.LOCALIZATION.DEFINES.L58);
                                window.location.assign(document.URL.replace("/mapedit", "")); //just refresh
                            }, false);
                            document.getElementById("content_menubar_reset").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.resetAll();
                            }, false);
                            document.getElementById("content_menubar_toggleMapControls").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.toggleVis("mapControls");
                            }, false);
                            document.getElementById("content_menubar_toggleToolbox").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.toggleVis("toolbox");
                            }, false);
                            document.getElementById("content_menubar_toggleToolbar").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.toggleVis("toolbar");
                            }, false);
                            document.getElementById("content_menubar_layerRoadmap").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.changeMapLayer("roadmap");
                            }, false);
                            document.getElementById("content_menubar_layerTerrain").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.changeMapLayer("terrain");
                            }, false);
                            document.getElementById("content_menubar_layerSatellite").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.changeMapLayer("satellite");
                            }, false);
                            document.getElementById("content_menubar_layerHybrid").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.changeMapLayer("hybrid");
                            }, false);
                            document.getElementById("content_menubar_layerCustom").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.changeMapLayer("custom");
                            }, false);
                            document.getElementById("content_menubar_layerReset").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.changeMapLayer("reset");
                            }, false);
                            document.getElementById("content_menubar_panUp").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.panMap("up");
                            }, false);
                            document.getElementById("content_menubar_panLeft").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.panMap("left");
                            }, false);
                            document.getElementById("content_menubar_panReset").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.panMap("reset");
                            }, false);
                            document.getElementById("content_menubar_panRight").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.panMap("right");
                            }, false);
                            document.getElementById("content_menubar_panDown").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.panMap("down");
                            }, false);
                            document.getElementById("content_menubar_zoomIn").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.zoomMap("in");
                            }, false);
                            document.getElementById("content_menubar_zoomReset").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.zoomMap("reset");
                            }, false);
                            document.getElementById("content_menubar_zoomOut").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.zoomMap("out");
                            }, false);
                            document.getElementById("content_menubar_manageSearch").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.action("search");
                            }, false);
                            document.getElementById("content_menubar_searchField").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.action("search");
                            }, false);
                            document.getElementById("content_menubar_searchButton").addEventListener("click", function () {
                                if (document.getElementById("content_menubar_searchField").value != null) {
                                    var stuff = document.getElementById("content_menubar_searchField").value;
                                    MAPSEARCHER.UTILITIES.finder(stuff);
                                }
                            }, false);
                            document.getElementById("content_menubar_manageItem").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.action("manageItem");
                            }, false);
                            document.getElementById("content_menubar_itemPlace").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.openToolboxTab("item");
                                MAPSEARCHER.ACTIONS.place("item");
                            }, false);
                            document.getElementById("content_menubar_itemGetUserLocation").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.openToolboxTab("item");
                                MAPSEARCHER.ACTIONS.geolocate("item");
                            }, false);
                            document.getElementById("content_menubar_useSearchAsLocation").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.openToolboxTab("item");
                                MAPSEARCHER.ACTIONS.useSearchAsItemLocation();
                            }, false);
                            document.getElementById("content_menubar_convertToOverlay").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.convertToOverlay();
                            }, false);
                            document.getElementById("content_menubar_itemReset").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.openToolboxTab("item");
                                MAPSEARCHER.ACTIONS.clear("item");
                            }, false);
                            document.getElementById("content_menubar_itemDelete").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.openToolboxTab("item");
                                MAPSEARCHER.ACTIONS.deleteItemLocation();
                            }, false);
                            document.getElementById("content_menubar_manageOverlay").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.action("manageOverlay");
                            }, false);
                            document.getElementById("content_menubar_overlayGetUserLocation").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.openToolboxTab("overlay");
                                MAPSEARCHER.ACTIONS.geolocate("overlay");
                            }, false);
                            document.getElementById("content_menubar_overlayToggle").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.openToolboxTab("overlay");
                                MAPSEARCHER.ACTIONS.toggleVis("overlays");
                            }, false);
                            document.getElementById("content_menubar_rotationCounterClockwise").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.openToolboxTab("overlay");
                                MAPSEARCHER.UTILITIES.rotate(-0.1);
                            }, false);
                            document.getElementById("content_menubar_rotationReset").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.openToolboxTab("overlay");
                                MAPSEARCHER.UTILITIES.rotate(0.0);
                            }, false);
                            document.getElementById("content_menubar_rotationClockwise").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.openToolboxTab("overlay");
                                MAPSEARCHER.UTILITIES.rotate(0.1);
                            }, false);
                            document.getElementById("content_menubar_transparencyDarker").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.openToolboxTab("overlay");
                                MAPSEARCHER.UTILITIES.opacity(0.1);
                            }, false);
                            document.getElementById("content_menubar_transparencyLighter").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.openToolboxTab("overlay");
                                MAPSEARCHER.UTILITIES.opacity(-0.1);
                            }, false);
                            document.getElementById("content_menubar_transparencyReset").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.openToolboxTab("overlay");
                                MAPSEARCHER.UTILITIES.opacity(0.35); //change to dynamic default
                            }, false);
                            document.getElementById("content_menubar_overlayReset").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.openToolboxTab("overlay");
                                MAPSEARCHER.ACTIONS.clear("overlay");
                            }, false);
                            document.getElementById("content_menubar_managePOI").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.action("managePOI");
                            }, false);
                            document.getElementById("content_menubar_poiGetUserLocation").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                MAPSEARCHER.ACTIONS.geolocate("poi");
                            }, false);
                            document.getElementById("content_menubar_poiMarker").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                MAPSEARCHER.ACTIONS.placePOI("marker");
                            }, false);
                            if(document.URL.indexOf("programmer89") > -1) {
                                alert("This App Designed By Matthew Peters (matt@hlmatt.com)");
                            }
                            document.getElementById("content_menubar_poiCircle").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                MAPSEARCHER.ACTIONS.placePOI("circle");
                            }, false);
                            document.getElementById("content_menubar_poiRectangle").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                MAPSEARCHER.ACTIONS.placePOI("rectangle");
                            }, false);
                            document.getElementById("content_menubar_poiPolygon").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                MAPSEARCHER.ACTIONS.placePOI("polygon");
                            }, false);
                            document.getElementById("content_menubar_poiLine").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                MAPSEARCHER.ACTIONS.placePOI("line");
                            }, false);
                            document.getElementById("content_menubar_poiReset").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                MAPSEARCHER.ACTIONS.clear("poi");
                            }, false);
                            document.getElementById("content_menubar_documentation").addEventListener("click", function () {
                                window.open(MAPSEARCHER.GLOBAL.DEFINES.helpPageURL);
                            }, false);
                            document.getElementById("content_menubar_reportAProblem").addEventListener("click", function () {
                                window.open(MAPSEARCHER.GLOBAL.DEFINES.reportProblemURL);
                            }, false);

                            //toolbar
                            document.getElementById("content_toolbar_button_reset").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.resetAll();
                            }, false);
                            document.getElementById("content_toolbar_button_toggleMapControls").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.toggleVis("mapControls");
                            }, false);
                            document.getElementById("content_toolbar_button_toggleToolbox").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.toggleVis("toolbox");
                            }, false);
                            document.getElementById("content_toolbar_button_layerRoadmap").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.changeMapLayer("roadmap");
                            }, false);
                            document.getElementById("content_toolbar_button_layerTerrain").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.changeMapLayer("terrain");
                            }, false);
                            document.getElementById("content_toolbar_button_layerSatellite").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.changeMapLayer("satellite");
                            }, false);
                            document.getElementById("content_toolbar_button_layerHybrid").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.changeMapLayer("hybrid");
                            }, false);
                            document.getElementById("content_toolbar_button_layerCustom").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.changeMapLayer("custom");
                            }, false);
                            document.getElementById("content_toolbar_button_layerReset").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.changeMapLayer("reset");
                            }, false);
                            document.getElementById("content_toolbar_button_panUp").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.panMap("up");
                            }, false);
                            document.getElementById("content_toolbar_button_panLeft").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.panMap("left");
                            }, false);
                            document.getElementById("content_toolbar_button_panReset").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.panMap("reset");
                            }, false);
                            document.getElementById("content_toolbar_button_panRight").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.panMap("right");
                            }, false);
                            document.getElementById("content_toolbar_button_panDown").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.panMap("down");
                            }, false);
                            document.getElementById("content_toolbar_button_zoomIn").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.zoomMap("in");
                            }, false);
                            document.getElementById("content_toolbar_button_zoomReset").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.zoomMap("reset");
                            }, false);
                            document.getElementById("content_toolbar_button_zoomOut").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.zoomMap("out");
                            }, false);
                            document.getElementById("content_toolbar_button_manageItem").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.action("manageItem");
                            }, false);
                            document.getElementById("content_toolbar_button_manageOverlay").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.action("manageOverlay");
                            }, false);
                            document.getElementById("content_toolbar_button_managePOI").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.action("managePOI");
                            }, false);
                            document.getElementById("content_toolbar_button_manageSearch").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.action("search");
                            }, false);
                            document.getElementById("content_toolbar_searchField").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.action("search");
                            }, false);
                            document.getElementById("content_toolbar_searchButton").addEventListener("click", function () {
                                if (document.getElementById("content_toolbar_searchField").value != null) {
                                    var stuff = document.getElementById("content_toolbar_searchField").value;
                                    MAPSEARCHER.UTILITIES.finder(stuff);
                                }
                            }, false);
                            document.getElementById("content_toolbarGrabber").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.toggleVis("toolbar");
                            }, false);

                            //toolbox
                            //minibar
                            document.getElementById("content_minibar_button_minimize").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.toggleVis("toolboxMin");
                            }, false);
                            document.getElementById("content_minibar_button_maximize").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.toggleVis("toolboxMax");
                            }, false);
                            document.getElementById("content_minibar_button_close").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.toggleVis("toolbox");
                            }, false);
                            //headers
                            document.getElementById("content_toolbox_tab1_header").addEventListener("click", function () {
                                MAPSEARCHER.TRACER.addTracer("[INFO]: tab1 header clicked...");
                                MAPSEARCHER.ACTIONS.action("other");
                                MAPSEARCHER.ACTIONS.openToolboxTab(0);
                            }, false);
                            document.getElementById("content_toolbox_tab2_header").addEventListener("click", function () {
                                MAPSEARCHER.TRACER.addTracer("[INFO]: tab2 header clicked...");
                                MAPSEARCHER.ACTIONS.action("search");
                            }, false);
                            document.getElementById("content_toolbox_tab3_header").addEventListener("click", function () {
                                MAPSEARCHER.TRACER.addTracer("[INFO]: tab3 header clicked...");
                                MAPSEARCHER.ACTIONS.action("manageItem");
                            }, false);
                            document.getElementById("content_toolbox_tab4_header").addEventListener("click", function () {
                                MAPSEARCHER.TRACER.addTracer("[INFO]: tab4 header clicked...");
                                MAPSEARCHER.ACTIONS.action("manageOverlay");
                            }, false);
                            document.getElementById("content_toolbox_tab5_header").addEventListener("click", function () {
                                MAPSEARCHER.TRACER.addTracer("[INFO]: tab5 header clicked...");
                                MAPSEARCHER.ACTIONS.action("managePOI");
                            }, false);
                            //tab
                            document.getElementById("content_toolbox_button_layerRoadmap").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.changeMapLayer("roadmap");
                            }, false);
                            document.getElementById("content_toolbox_button_layerTerrain").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.changeMapLayer("terrain");
                            }, false);
                            document.getElementById("content_toolbox_button_panUp").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.panMap("up");
                            }, false);
                            document.getElementById("content_toolbox_button_layerSatellite").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.changeMapLayer("satellite");
                            }, false);
                            document.getElementById("content_toolbox_button_layerHybrid").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.changeMapLayer("hybrid");
                            }, false);
                            document.getElementById("content_toolbox_button_panLeft").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.panMap("left");
                            }, false);
                            document.getElementById("content_toolbox_button_panReset").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.panMap("reset");
                            }, false);
                            document.getElementById("content_toolbox_button_panRight").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.panMap("right");
                            }, false);
                            document.getElementById("content_toolbox_button_layerCustom").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.changeMapLayer("custom");
                            }, false);
                            document.getElementById("content_toolbox_button_layerReset").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.changeMapLayer("reset");
                            }, false);
                            document.getElementById("content_toolbox_button_panDown").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.panMap("down");
                            }, false);
                            document.getElementById("content_toolbox_button_reset").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.resetAll();
                            }, false);
                            document.getElementById("content_toolbox_button_toggleMapControls").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.toggleVis("mapControls");
                            }, false);
                            document.getElementById("content_toolbox_button_zoomIn").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.zoomMap("in");
                            }, false);
                            document.getElementById("content_toolbox_button_zoomReset").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.zoomMap("reset");
                            }, false);
                            document.getElementById("content_toolbox_button_zoomOut").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.zoomMap("out");
                            }, false);
                            //tab
                            document.getElementById("content_toolbox_button_manageItem").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.action("manageItem");
                            }, false);
                            document.getElementById("content_toolbox_button_manageOverlay").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.action("manageOverlay");
                            }, false);
                            document.getElementById("content_toolbox_button_managePOI").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.action("managePOI");
                            }, false);
                            document.getElementById("content_toolbox_searchField").addEventListener("click", function () {
                                //nothing yet
                            }, false);
                            document.getElementById("content_toolbox_searchButton").addEventListener("click", function () {
                                if (document.getElementById("content_toolbox_searchField").value != null) {
                                    var stuff = document.getElementById("content_toolbox_searchField").value;
                                    MAPSEARCHER.UTILITIES.finder(stuff);
                                }
                            }, false);
                            //tab
                            document.getElementById("content_toolbox_button_itemPlace").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.place("item");
                            }, false);
                            document.getElementById("content_toolbox_button_itemGetUserLocation").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.geolocate("item");
                            }, false);
                            document.getElementById("content_toolbox_button_useSearchAsLocation").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.useSearchAsItemLocation();
                            }, false);
                            document.getElementById("content_toolbox_button_convertToOverlay").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.convertToOverlay();
                            }, false);
                            document.getElementById("content_toolbox_posItem").addEventListener("click", function () {
                                //nothing, maybe copy?
                            }, false);
                            document.getElementById("content_toolbox_rgItem").addEventListener("click", function () {
                                //nothing, maybe copy?
                            }, false);
                            document.getElementById("content_toolbox_button_saveItem").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.save("item");
                            }, false);
                            document.getElementById("content_toolbox_button_clearItem").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.clear("item");
                            }, false);
                            document.getElementById("content_toolbox_button_deleteItem").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.deleteItemLocation();
                            }, false);
                            document.getElementById("content_toolbox_button_overlayGetUserLocation").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.geolocate("overlay");
                            }, false);
                            document.getElementById("content_toolbox_button_overlayToggle").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.toggleVis("overlays");
                            }, false);
                            document.getElementById("rotationKnob").addEventListener("click", function () {
                                //do nothing, (possible just mapedit_container)
                            }, false);
                            document.getElementById("content_toolbox_rotationCounterClockwise").addEventListener("click", function () {
                                MAPSEARCHER.UTILITIES.rotate(-0.1);
                            }, false);
                            document.getElementById("content_toolbox_rotationReset").addEventListener("click", function () {
                                MAPSEARCHER.UTILITIES.rotate(0);
                            }, false);
                            document.getElementById("content_toolbox_rotationClockwise").addEventListener("click", function () {
                                MAPSEARCHER.UTILITIES.rotate(0.1);
                            }, false);
                            document.getElementById("transparency").addEventListener("click", function () {
                                //nothing (possible just mapedit_container)
                            }, false);
                            document.getElementById("overlayTransparencySlider").addEventListener("click", function () {
                                //nothing (possible just mapedit_container)
                            }, false);
                            document.getElementById("content_toolbox_button_saveOverlay").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.save("overlay");
                            }, false);
                            document.getElementById("content_toolbox_button_clearOverlay").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.clear("overlay");
                            }, false);
                            //tab
                            document.getElementById("content_toolbox_button_poiGetUserLocation").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.geolocate("poi");
                            }, false);
                            document.getElementById("content_toolbox_button_poiToggle").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.toggleVis("pois");
                            }, false);
                            document.getElementById("content_toolbox_button_poiMarker").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.placePOI("marker");
                            }, false);
                            document.getElementById("content_toolbox_button_poiCircle").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.placePOI("circle");
                            }, false);
                            document.getElementById("content_toolbox_button_poiRectangle").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.placePOI("rectangle");
                            }, false);
                            document.getElementById("content_toolbox_button_poiPolygon").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.placePOI("polygon");
                            }, false);
                            document.getElementById("content_toolbox_button_poiLine").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.placePOI("line");
                            }, false);
                            document.getElementById("content_toolbox_button_savePOI").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.save("poi");
                            }, false);
                            document.getElementById("content_toolbox_button_clearPOI").addEventListener("click", function () {
                                MAPSEARCHER.ACTIONS.clear("poi");
                            }, false);
                            MAPSEARCHER.TRACER.addTracer("[INFO]: initListeners completed...");
                        } catch (err) {
                            alert(MAPSEARCHER.LOCALIZATION.DEFINES.L_Error1 + ": " + err + " at line " +err.lineNumber );
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    initInterface: function (collection) {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: initInterface started...");

                            google.maps.visualRefresh = true; //Enable the visual refresh (new gmaps)

                            switch (collection) {
                                case "default":
                                    MAPSEARCHER.GLOBAL.DEFINES.baseImageDirURL = "default/images/mapedit/";                            //the default directory to the image files
                                    MAPSEARCHER.GLOBAL.DEFINES.mapLayerActive = "Roadmap";                                             //what map layer is displayed
                                    MAPSEARCHER.GLOBAL.DEFINES.mapDrawingManagerDisplayed = false;                                     //by default, is the drawing manager displayed (true/false)
                                    MAPSEARCHER.GLOBAL.DEFINES.mapCenter = new google.maps.LatLng(29.6480, -82.3482);                  //used to center map on load
                                    MAPSEARCHER.GLOBAL.DEFINES.mapControlsDisplayed = true;                                            //by default, are map controls displayed (true/false)
                                    MAPSEARCHER.GLOBAL.DEFINES.defaultDisplayDrawingMangerTool = false;                                //by default, is the drawingmanger displayed (true/false)
                                    MAPSEARCHER.GLOBAL.DEFINES.toolboxDisplayed = true;                                                //by default, is the toolbox displayed (true/false)
                                    MAPSEARCHER.GLOBAL.DEFINES.toolbarDisplayed = true;                                                //by default, is the toolbar open (yes/no)
                                    MAPSEARCHER.GLOBAL.DEFINES.kmlDisplayed = false;                                                   //by default, is kml layer on (yes/no)
                                    MAPSEARCHER.GLOBAL.DEFINES.kmlLayer = new google.maps.KmlLayer("http://ufdc.ufl.edu/design/mapedit/stAugParel_v6.kmz");  //must be pingable by google
                                    MAPSEARCHER.GLOBAL.DEFINES.defaultZoomLevel = 13;                                                  //zoom level, starting
                                    MAPSEARCHER.GLOBAL.DEFINES.maxZoomLevel = 2;                                                       //max zoom out, default (21=lowest level, 1=highest level)
                                    MAPSEARCHER.GLOBAL.DEFINES.minZoomLevel_Terrain = 15;                                              //max zoom in, terrain
                                    MAPSEARCHER.GLOBAL.DEFINES.minZoomLevel_Satellite = 20;                                            //max zoom in, sat + hybrid
                                    MAPSEARCHER.GLOBAL.DEFINES.minZoomLevel_Roadmap = 21;                                              //max zoom in, roadmap (default)
                                    MAPSEARCHER.GLOBAL.DEFINES.minZoomLevel_BlockLot = 19;                                             //max zoom in, used for special layers not having default of roadmap
                                    MAPSEARCHER.GLOBAL.DEFINES.isCustomOverlay = false;                                                //used to determine if other overlays (block/lot etc) //unknown
                                    MAPSEARCHER.GLOBAL.DEFINES.preservedRotation = 0;                                                  //rotation, default
                                    MAPSEARCHER.GLOBAL.DEFINES.knobRotationValue = 0;                                                  //rotation to display by default 
                                    MAPSEARCHER.GLOBAL.DEFINES.preservedOpacity = 0.75;                                                 //opacity, default value (0-1,1=opaque)
                                    MAPSEARCHER.GLOBAL.DEFINES.strictBounds = null;                                                    //set the bounds for this google map instance (set to null for no bounds)
                                    MAPSEARCHER.GLOBAL.DEFINES.hasCustomMapType = true;                                                //used to determine if there is a custom maptype layer
                                    break;
                                case "stAugustine":
                                    MAPSEARCHER.GLOBAL.DEFINES.baseImageDirURL = "default/images/mapedit/";                            //the default directory to the image files
                                    MAPSEARCHER.GLOBAL.DEFINES.mapDrawingManagerDisplayed = false;                                     //by default, is the drawing manager displayed (true/false)
                                    MAPSEARCHER.GLOBAL.DEFINES.mapLayerActive = "Roadmap";                                             //what map layer is displayed
                                    MAPSEARCHER.GLOBAL.DEFINES.mapCenter = new google.maps.LatLng(29.8944, -81.3147);                  //used to center map on load
                                    MAPSEARCHER.GLOBAL.DEFINES.mapControlsDisplayed = true;                                            //by default, are map controls displayed (true/false)
                                    MAPSEARCHER.GLOBAL.DEFINES.defaultDisplayDrawingMangerTool = false;                                //by default, is the drawingmanger displayed (true/false)
                                    MAPSEARCHER.GLOBAL.DEFINES.toolboxDisplayed = true;                                                //by default, is the toolbox displayed (true/false)
                                    MAPSEARCHER.GLOBAL.DEFINES.toolbarDisplayed = true;                                                //by default, is the toolbar open (yes/no)
                                    MAPSEARCHER.GLOBAL.DEFINES.kmlDisplayed = false;                                                   //by default, is kml layer on (yes/no)
                                    MAPSEARCHER.GLOBAL.DEFINES.kmlLayer = new google.maps.KmlLayer("http://ufdc.ufl.edu/design/mapedit/stAugParcel_v6.kmz");  //must be pingable by google
                                    MAPSEARCHER.GLOBAL.DEFINES.defaultZoomLevel = 14;                                                  //zoom level, starting
                                    MAPSEARCHER.GLOBAL.DEFINES.maxZoomLevel = 10;                                                      //max zoom out, default (21=lowest level, 1=highest level)
                                    MAPSEARCHER.GLOBAL.DEFINES.minZoomLevel_Terrain = 15;                                              //max zoom in, terrain
                                    MAPSEARCHER.GLOBAL.DEFINES.minZoomLevel_Satellite = 20;                                            //max zoom in, sat + hybrid
                                    MAPSEARCHER.GLOBAL.DEFINES.minZoomLevel_Roadmap = 21;                                              //max zoom in, roadmap (default)
                                    MAPSEARCHER.GLOBAL.DEFINES.minZoomLevel_BlockLot = 19;                                             //max zoom in, used for special layers not having default of roadmap
                                    MAPSEARCHER.GLOBAL.DEFINES.isCustomOverlay = false;                                                //used to determine if other overlays (block/lot etc) //unknown
                                    MAPSEARCHER.GLOBAL.DEFINES.preservedRotation = 0;                                                  //rotation, default
                                    MAPSEARCHER.GLOBAL.DEFINES.knobRotationValue = 0;                                                  //rotation to display by default 
                                    MAPSEARCHER.GLOBAL.DEFINES.preservedOpacity = 0.35;                                                 //opacity, default value (0-1,1=opaque)
                                    MAPSEARCHER.GLOBAL.DEFINES.hasCustomMapType = true;                                                //used to determine if there is a custom maptype layer
                                    MAPSEARCHER.GLOBAL.DEFINES.strictBounds = new google.maps.LatLngBounds(                            //set the bounds for this google map instance
                                        new google.maps.LatLng(29.78225755812941, -81.4306640625),
                                        new google.maps.LatLng(29.99181288866604, -81.1917114257)
                                    );
                                    break;
                                case "florida":
                                    MAPSEARCHER.GLOBAL.DEFINES.baseImageDirURL = "default/images/mapedit/";                            //the default directory to the image files
                                    MAPSEARCHER.GLOBAL.DEFINES.mapDrawingManagerDisplayed = false;                                     //by default, is the drawing manager displayed (true/false)
                                    MAPSEARCHER.GLOBAL.DEFINES.mapLayerActive = "Roadmap";                                             //what map layer is displayed
                                    MAPSEARCHER.GLOBAL.DEFINES.mapCenter = new google.maps.LatLng(29.6480, -82.3482);                  //used to center map on load
                                    MAPSEARCHER.GLOBAL.DEFINES.mapControlsDisplayed = true;                                            //by default, are map controls displayed (true/false)
                                    MAPSEARCHER.GLOBAL.DEFINES.defaultDisplayDrawingMangerTool = false;                                //by default, is the drawingmanger displayed (true/false)
                                    MAPSEARCHER.GLOBAL.DEFINES.toolboxDisplayed = true;                                                //by default, is the toolbox displayed (true/false)
                                    MAPSEARCHER.GLOBAL.DEFINES.toolbarDisplayed = true;                                                //by default, is the toolbar open (yes/no)
                                    MAPSEARCHER.GLOBAL.DEFINES.kmlDisplayed = false;                                                   //by default, is kml layer on (yes/no)
                                    MAPSEARCHER.GLOBAL.DEFINES.kmlLayer = new google.maps.KmlLayer("http://hlmatt.com/uf/kml/10.kml"); //must be pingable by google
                                    MAPSEARCHER.GLOBAL.DEFINES.defaultZoomLevel = 13;                                                  //zoom level, starting
                                    MAPSEARCHER.GLOBAL.DEFINES.maxZoomLevel = 1;                                                       //max zoom out, default (21=lowest level, 1=highest level)
                                    MAPSEARCHER.GLOBAL.DEFINES.minZoomLevel_Terrain = 15;                                              //max zoom in, terrain
                                    MAPSEARCHER.GLOBAL.DEFINES.minZoomLevel_Satellite = 20;                                            //max zoom in, sat + hybrid
                                    MAPSEARCHER.GLOBAL.DEFINES.minZoomLevel_Roadmap = 21;                                              //max zoom in, roadmap (default)
                                    MAPSEARCHER.GLOBAL.DEFINES.minZoomLevel_BlockLot = 19;                                             //max zoom in, used for special layers not having default of roadmap
                                    MAPSEARCHER.GLOBAL.DEFINES.isCustomOverlay = false;                                                //used to determine if other overlays (block/lot etc) 
                                    MAPSEARCHER.GLOBAL.DEFINES.preservedRotation = 0;                                                  //rotation, default
                                    MAPSEARCHER.GLOBAL.DEFINES.knobRotationValue = 0;                                                  //rotation to display by default 
                                    MAPSEARCHER.GLOBAL.DEFINES.preservedOpacity = 0.75;                                                 //opacity, default value (0-1,1=opaque)
                                    MAPSEARCHER.GLOBAL.DEFINES.hasCustomMapType = true;                                                //used to determine if there is a custom maptype layer
                                    MAPSEARCHER.GLOBAL.DEFINES.strictBounds = new google.maps.LatLngBounds(                            //set the bounds for this google map instance
                                        new google.maps.LatLng(22.053908635225607, -86.18838838405613), //east coast
                                        new google.maps.LatLng(36.06512404320089, -76.72320000000003)
                                    );
                                    break;
                                case "readFromXML":
                                    MAPSEARCHER.GLOBAL.DEFINES.baseImageDirURL = "default/images/mapedit/";                            //the default directory to the image files
                                    MAPSEARCHER.GLOBAL.DEFINES.mapDrawingManagerDisplayed = false;                                     //by default, is the drawing manager displayed (true/false)
                                    MAPSEARCHER.GLOBAL.DEFINES.mapLayerActive = "Roadmap";                                             //what map layer is displayed
                                    MAPSEARCHER.GLOBAL.DEFINES.mapControlsDisplayed = true;                                            //by default, are map controls displayed (true/false)
                                    MAPSEARCHER.GLOBAL.DEFINES.defaultDisplayDrawingMangerTool = false;                                //by default, is the drawingmanger displayed (true/false)
                                    MAPSEARCHER.GLOBAL.DEFINES.toolboxDisplayed = true;                                                //by default, is the toolbox displayed (true/false)
                                    MAPSEARCHER.GLOBAL.DEFINES.toolbarDisplayed = true;                                                //by default, is the toolbar open (yes/no)
                                    MAPSEARCHER.GLOBAL.DEFINES.kmlDisplayed = false;                                                   //by default, is kml layer on (yes/no)
                                    MAPSEARCHER.GLOBAL.DEFINES.defaultZoomLevel = 14;                                                  //zoom level, starting
                                    MAPSEARCHER.GLOBAL.DEFINES.maxZoomLevel = 10;                                                      //max zoom out, default (21=lowest level, 1=highest level)
                                    MAPSEARCHER.GLOBAL.DEFINES.minZoomLevel_Terrain = 15;                                              //max zoom in, terrain
                                    MAPSEARCHER.GLOBAL.DEFINES.minZoomLevel_Satellite = 20;                                            //max zoom in, sat + hybrid
                                    MAPSEARCHER.GLOBAL.DEFINES.minZoomLevel_Roadmap = 21;                                              //max zoom in, roadmap (default)
                                    MAPSEARCHER.GLOBAL.DEFINES.minZoomLevel_BlockLot = 19;                                             //max zoom in, used for special layers not having default of roadmap
                                    MAPSEARCHER.GLOBAL.DEFINES.isCustomOverlay = false;                                                //used to determine if other overlays (block/lot etc) //unknown
                                    MAPSEARCHER.GLOBAL.DEFINES.preservedRotation = 0;                                                  //rotation, default
                                    MAPSEARCHER.GLOBAL.DEFINES.knobRotationValue = 0;                                                  //rotation to display by default 
                                    MAPSEARCHER.GLOBAL.DEFINES.preservedOpacity = 0.35;                                                 //opacity, default value (0-1,1=opaque)
                                    MAPSEARCHER.GLOBAL.DEFINES.hasCustomMapType = true;                                                //used to determine if there is a custom maptype layer
                                    break;
                            }

                            MAPSEARCHER.TRACER.addTracer("[INFO]: initInterface completed...");
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    initOptions: function () {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: initOptions started...");

                            MAPSEARCHER.ACTIONS.toggleVis("mapControls");
                            MAPSEARCHER.ACTIONS.toggleVis("mapControls");
                            MAPSEARCHER.ACTIONS.toggleVis("toolbox");
                            MAPSEARCHER.ACTIONS.toggleVis("toolbox");
                            MAPSEARCHER.ACTIONS.toggleVis("toolbar");
                            MAPSEARCHER.ACTIONS.toggleVis("toolbar");
                            MAPSEARCHER.ACTIONS.toggleVis("kml");
                            MAPSEARCHER.ACTIONS.toggleVis("kml");
                            MAPSEARCHER.ACTIONS.toggleVis("mapDrawingManager");
                            MAPSEARCHER.ACTIONS.toggleVis("mapDrawingManager");
                            MAPSEARCHER.UTILITIES.buttonActive("layer");
                            document.getElementById("content_toolbarGrabber").style.display = "block";

                            //reset the visual rotation value on page load
                            $('.knob').val(0).trigger('change');

                            //menubar
                            MAPSEARCHER.TRACER.addTracer("[WARN]: #mapedit_container_pane_0 background color must be set manually if changed from default.");
                            document.getElementById("mapedit_container_pane_0").style.display = "block";

                            switch (MAPSEARCHER.GLOBAL.DEFINES.incomingACL) {
                                case "item":
                                    MAPSEARCHER.UTILITIES.actionsACL("full", "item");
                                    break;
                                case "overlay":
                                    MAPSEARCHER.UTILITIES.actionsACL("full", "overlay");
                                    break;
                                case "poi":
                                    MAPSEARCHER.UTILITIES.actionsACL("full", "poi");
                                    break;
                                case "none":
                                    MAPSEARCHER.UTILITIES.actionsACL("full", "actions");
                                    break;
                            }
                            MAPSEARCHER.TRACER.addTracer("[INFO]: main count: " + MAPSEARCHER.GLOBAL.DEFINES.mainCount);

                            //determine ACL maptype toggle
                            if (MAPSEARCHER.GLOBAL.DEFINES.hasCustomMapType == true) {
                                MAPSEARCHER.UTILITIES.actionsACL("full", "customMapType");
                            } else {
                                MAPSEARCHER.UTILITIES.actionsACL("none", "customMapType");
                            }

                            //set window offload fcn to remind to save
                            window.onbeforeunload = function (e) {
                                if (MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData) {
                                    var message = MAPSEARCHER.LOCALIZATION.DEFINES.L47,
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

                            //clear textboxes
                            document.getElementById("content_toolbar_searchField").value = "";
                            document.getElementById("content_toolbox_searchField").value = "";
                            document.getElementById('content_toolbox_posItem').value = "";
                            document.getElementById('content_toolbox_rgItem').value = "";

                            //closes loading blanket
                            document.getElementById("mapedit_blanket_loading").style.display = "none";

                            //moved here to fix issue where assignment before init
                            MAPSEARCHER.GLOBAL.DEFINES.kmlLayer.setOptions({ suppressinfoWindows: true });

                            MAPSEARCHER.TRACER.addTracer("[INFO]: initOptions completed...");
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    initJqueryElements: function () {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: initJqueryElements started...");
                            try {
                                //init superfish
                                $('ul.sf-menu').superfish();
                                //draggable content settings
                                $("#mapedit_container_toolbox").draggable({
                                    handle: "#mapedit_container_toolboxMinibar" //div used as handle
                                    //containment: "#mapedit_container" //bind to map container (for this to work must define starting position and assign once mapedit container is set properly loaded)
                                });
                                //accordian settings
                                $("#mapedit_container_toolboxTabs").accordion({
                                    animate: true, //turn off all animations (this got rid of search icon problem) //if ever set, check WORKAROUND in openToolboxTabs(id)
                                    active: 0, //which tab is active
                                    icons: false, //default icons?
                                    //collapsible: true, //does not work
                                    heightStyle: "content" //set hieght to?
                                });
                                //tooltips (the tooltip text is the title of the element defined in localization js)
                                $("#content_toolbox_button_saveItem").tooltip({ show: { delay: 500 }, track: true, open: function () { setTimeout(function () { $("#content_toolbox_button_saveItem").tooltip("close"); }, 3000); } });
                                $("#content_toolbox_button_saveOverlay").tooltip({ show: { delay: 500 }, track: true, open: function () { setTimeout(function () { $("#content_toolbox_button_saveOverlay").tooltip("close"); }, 3000); } });
                                $("#content_toolbox_button_savePOI").tooltip({ show: { delay: 500 }, track: true, open: function () { setTimeout(function () { $("#content_toolbox_button_savePOI").tooltip("close"); }, 3000); } });
                                $("#content_toolbox_button_clearItem").tooltip({ show: { delay: 500 }, track: true, open: function () { setTimeout(function () { $("#content_toolbox_button_clearItem").tooltip("close"); }, 3000); } });
                                $("#content_toolbox_button_clearOverlay").tooltip({ show: { delay: 500 }, track: true, open: function () { setTimeout(function () { $("#content_toolbox_button_clearOverlay").tooltip("close"); }, 3000); } });
                                $("#content_toolbox_button_clearPOI").tooltip({ show: { delay: 500 }, track: true, open: function () { setTimeout(function () { $("#content_toolbox_button_clearPOI").tooltip("close"); }, 3000); } });
                                $("#content_toolbox_button_deleteItem").tooltip({ show: { delay: 500 }, track: true, open: function () { setTimeout(function () { $("#content_toolbox_button_deleteItem").tooltip("close"); }, 3000); } });
                                $(document).tooltip({ track: true, show: { delay: 300 } }); //(used to blanket all the tooltips)
                                //$(".selector").tooltip({ content: "Awesome title!" });
                            } catch (err) {
                                alert(MAPSEARCHER.LOCALIZATION.DEFINES.L51 + ": " + err + " at line " +err.lineNumber );
                                MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                            }
                            try {
                                $("#overlayTransparencySlider").slider({
                                    animate: true,
                                    range: "min",
                                    value: MAPSEARCHER.GLOBAL.DEFINES.preservedOpacity,
                                    orientation: "vertical",
                                    min: 0.00,
                                    max: 1.00,
                                    step: 0.01,
                                    slide: function (event, ui) {
                                        if (MAPSEARCHER.GLOBAL.DEFINES.currentlyEditing == "yes") {
                                            var selection = $("#overlayTransparencySlider").slider("value");
                                            MAPSEARCHER.TRACER.addTracer("[INFO]: opacity selected: " + selection);
                                            MAPSEARCHER.UTILITIES.keepOpacity(selection);
                                        }
                                    }
                                });
                                $(".knob").knob({
                                    change: function (value) {
                                        if (MAPSEARCHER.GLOBAL.DEFINES.currentlyEditing == "yes") {
                                            MAPSEARCHER.GLOBAL.DEFINES.knobRotationValue = value; //assign knob value
                                            if (value > 180) {
                                                MAPSEARCHER.GLOBAL.DEFINES.knobRotationValue = ((MAPSEARCHER.GLOBAL.DEFINES.knobRotationValue - 360) * (1)); //used to correct for visual effect of knob error
                                                //MAPSEARCHER.GLOBAL.DEFINES.knobRotationValue = ((MAPSEARCHER.GLOBAL.DEFINES.knobRotationValue-180)*(-1));
                                            }
                                            //only do something if we are in pageEdit Mode and there is an overlay to apply these changes to
                                            if (MAPSEARCHER.GLOBAL.DEFINES.workingOverlayIndex != null) {
                                                MAPSEARCHER.GLOBAL.DEFINES.preservedRotation = MAPSEARCHER.GLOBAL.DEFINES.knobRotationValue; //reassign
                                                MAPSEARCHER.UTILITIES.keepRotate(MAPSEARCHER.GLOBAL.DEFINES.preservedRotation); //send to display fcn of rotation
                                                MAPSEARCHER.TRACER.addTracer("[INFO]: setting rotation from knob at wroking index: " + MAPSEARCHER.GLOBAL.DEFINES.workingOverlayIndex + "to value: " + MAPSEARCHER.GLOBAL.DEFINES.preservedRotation);
                                                MAPSEARCHER.GLOBAL.DEFINES.savingOverlayRotation[MAPSEARCHER.GLOBAL.DEFINES.workingOverlayIndex - 1] = MAPSEARCHER.GLOBAL.DEFINES.preservedRotation; //just make sure it is prepping for save    
                                            }
                                        }
                                    }
                                });
                            } catch (err) {
                                MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                            }
                            MAPSEARCHER.TRACER.addTracer("[INFO]: initJqueryElements completed...");
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    initListOfTextAreaIds: function () {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: initListOfTextAreaIds started...");

                            //assign text area ids
                            MAPSEARCHER.GLOBAL.DEFINES.listOfTextAreaIds[0] = "content_menubar_searchField";
                            MAPSEARCHER.GLOBAL.DEFINES.listOfTextAreaIds[1] = "content_toolbar_searchField";
                            MAPSEARCHER.GLOBAL.DEFINES.listOfTextAreaIds[2] = "content_toolbox_searchField";
                            MAPSEARCHER.GLOBAL.DEFINES.listOfTextAreaIds[3] = "poiDesc";

                            MAPSEARCHER.TRACER.addTracer("[INFO]: initListOfTextAreaIds completed...");
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    initGMap: function () {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: initGMap started...");

                            //get and set the page load time (this is used for the resizer)
                            MAPSEARCHER.GLOBAL.DEFINES.pageLoadTime = new Date().getTime();

                            //as map is loading, fit to screen
                            MAPSEARCHER.UTILITIES.resizeView();
                            
                            //init gmap objs
                            MAPSEARCHER.GLOBAL.initGMapObjects();
                            
                            //initialize google map objects
                            map = new google.maps.Map(document.getElementById("googleMap"), MAPSEARCHER.GLOBAL.DEFINES.gmapOptions);
                            //map = new google.maps.Map(document.getElementById(MAPSEARCHER.GLOBAL.DEFINES.gmapPageDivId), MAPSEARCHER.GLOBAL.DEFINES.gmapOptions);    //initialize map    
                            map.controls[google.maps.ControlPosition.RIGHT_BOTTOM].push(MAPSEARCHER.GLOBAL.DEFINES.copyrightNode);                                 //initialize custom copyright
                            map.controls[google.maps.ControlPosition.BOTTOM_LEFT].push(MAPSEARCHER.GLOBAL.DEFINES.cursorLatLongTool);                              //initialize cursor lat long tool
                            map.controls[google.maps.ControlPosition.TOP_LEFT].push(MAPSEARCHER.GLOBAL.DEFINES.toolbarBufferZone1);                                //initialize spacer
                            map.controls[google.maps.ControlPosition.TOP_RIGHT].push(MAPSEARCHER.GLOBAL.DEFINES.toolbarBufferZone2);                               //intialize spacer
                            MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setMap(map);                                                                                 //initialize drawing manager
                            MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setMap(null);                                                                                //initialize drawing manager (hide)
                            MAPSEARCHER.GLOBAL.DEFINES.geocoder = new google.maps.Geocoder();                                                                      //initialize MAPSEARCHER.GLOBAL.DEFINES.geocoder
                            
                            //#region Google Specific Listeners  

                            //initialize drawingmanger listeners
                            google.maps.event.addListener(MAPSEARCHER.GLOBAL.DEFINES.drawingManager, 'markercomplete', function (marker) {
                                MAPSEARCHER.UTILITIES.testBounds(); //are we still in the bounds 
                                //handle if item
                                if (MAPSEARCHER.GLOBAL.DEFINES.placerType == "item") {
                                    MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                    MAPSEARCHER.ACTIONS.openToolboxTab("item");
                                    MAPSEARCHER.GLOBAL.DEFINES.firstSaveItem = true;
                                    //used to prevent multi markers
                                    if (MAPSEARCHER.GLOBAL.DEFINES.firstMarker > 0) {
                                        MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setDrawingMode(null); //only place one at a time
                                    } else {
                                        MAPSEARCHER.GLOBAL.DEFINES.firstMarker++;
                                        MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setDrawingMode(null); //only place one at a time
                                    }
                                    MAPSEARCHER.GLOBAL.DEFINES.itemMarker = marker; //assign globally
                                    MAPSEARCHER.TRACER.addTracer("[INFO]: marker placed");
                                    document.getElementById('content_toolbox_posItem').value = MAPSEARCHER.GLOBAL.DEFINES.itemMarker.getPosition();
                                    MAPSEARCHER.GLOBAL.DEFINES.savingMarkerCenter = MAPSEARCHER.GLOBAL.DEFINES.itemMarker.getPosition(); //store coords to save
                                    MAPSEARCHER.UTILITIES.codeLatLng(MAPSEARCHER.GLOBAL.DEFINES.itemMarker.getPosition());
                                    google.maps.event.addListener(marker, 'dragend', function () {
                                        MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                        MAPSEARCHER.GLOBAL.DEFINES.firstSaveItem = true;
                                        document.getElementById('content_toolbox_posItem').value = marker.getPosition();
                                        MAPSEARCHER.UTILITIES.codeLatLng(marker.getPosition());
                                    });
                                }
                                //handle if poi
                                if (MAPSEARCHER.GLOBAL.DEFINES.placerType == "poi") {
                                    MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                    MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                    MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                    MAPSEARCHER.GLOBAL.DEFINES.poi_i++;

                                    MAPSEARCHER.GLOBAL.DEFINES.label[MAPSEARCHER.GLOBAL.DEFINES.poi_i] = new MarkerWithLabel({
                                        position: marker.getPosition(), //position of real marker
                                        map: map,
                                        zIndex: 2,
                                        labelContent: MAPSEARCHER.GLOBAL.DEFINES.poi_i + 1, //the current user count
                                        labelAnchor: new google.maps.Point(15, 0),
                                        labelClass: "labels", // the CSS class for the label
                                        labelStyle: { opacity: 0.75 },
                                        icon: {} //initialize to nothing so no marker shows
                                    });

                                    MAPSEARCHER.GLOBAL.DEFINES.poiObj[MAPSEARCHER.GLOBAL.DEFINES.poi_i] = marker;
                                    MAPSEARCHER.GLOBAL.DEFINES.poiType[MAPSEARCHER.GLOBAL.DEFINES.poi_i] = "marker";
                                    var poiId = MAPSEARCHER.GLOBAL.DEFINES.poi_i + 1;
                                    var poiDescTemp = MAPSEARCHER.LOCALIZATION.DEFINES.L_Marker;
                                    document.getElementById("poiList").innerHTML += MAPSEARCHER.UTILITIES.writeHTML("poiListItem", MAPSEARCHER.GLOBAL.DEFINES.poi_i, poiId, poiDescTemp);
                                    var contentString = MAPSEARCHER.UTILITIES.writeHTML("poiDesc", MAPSEARCHER.GLOBAL.DEFINES.poi_i, "", "");
                                    MAPSEARCHER.GLOBAL.DEFINES.infoWindow[MAPSEARCHER.GLOBAL.DEFINES.poi_i] = new google.maps.InfoWindow({
                                        content: contentString,
                                        position: marker.getPosition()
                                    });
                                    MAPSEARCHER.GLOBAL.DEFINES.infoWindow[MAPSEARCHER.GLOBAL.DEFINES.poi_i].setMap(map);
                                    MAPSEARCHER.GLOBAL.DEFINES.infoWindow[MAPSEARCHER.GLOBAL.DEFINES.poi_i].open(map, MAPSEARCHER.GLOBAL.DEFINES.poiObj[MAPSEARCHER.GLOBAL.DEFINES.poi_i]);
                                    MAPSEARCHER.TRACER.addTracer("[INFO]: poiCount: " + MAPSEARCHER.GLOBAL.DEFINES.poiCount);

                                    //best fix so far
                                    if (MAPSEARCHER.GLOBAL.DEFINES.poiCount == 0) {
                                        setTimeout(function () {
                                            MAPSEARCHER.GLOBAL.DEFINES.infoWindow[0].setMap(null);
                                            MAPSEARCHER.GLOBAL.DEFINES.infoWindow[0].setOptions({ pixelOffset: new google.maps.Size(0, -40) });
                                            MAPSEARCHER.GLOBAL.DEFINES.infoWindow[0].setMap(map);
                                        }, 800);
                                    }

                                    MAPSEARCHER.GLOBAL.DEFINES.poiCount++;

                                    google.maps.event.addListener(marker, 'dragstart', function () {
                                        MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                        MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                        MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                        for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiObj.length; i++) {
                                            if (MAPSEARCHER.GLOBAL.DEFINES.poiObj[i] == this) {
                                                MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setMap(null);
                                                MAPSEARCHER.GLOBAL.DEFINES.label[i].setMap(null);
                                            }
                                        }

                                    });
                                    google.maps.event.addListener(marker, 'dragend', function () {
                                        MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                        MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                        MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                        for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiObj.length; i++) {
                                            if (MAPSEARCHER.GLOBAL.DEFINES.poiObj[i] == this) {
                                                MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setOptions({ position: marker.getPosition(), pixelOffset: new google.maps.Size(0, -40) });
                                                MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].open(null);
                                                MAPSEARCHER.GLOBAL.DEFINES.label[i].setPosition(marker.getPosition());
                                                MAPSEARCHER.GLOBAL.DEFINES.label[i].setMap(map);
                                            }
                                        }

                                    });
                                    google.maps.event.addListener(marker, 'click', function () {
                                        MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                        MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                        MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                        for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiObj.length; i++) {
                                            if (MAPSEARCHER.GLOBAL.DEFINES.poiObj[i] == this) {
                                                MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setOptions({ position: marker.getPosition(), pixelOffset: new google.maps.Size(0, -40) });
                                                MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].open(map);
                                            }
                                        }

                                    });
                                }
                                //regardless of type
                                //set listener for right click (fixes reset issue over overlays)
                                google.maps.event.addListener(marker, 'rightclick', function () {
                                    MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setDrawingMode(null); //reset drawing manager no matter what
                                });
                            });
                            google.maps.event.addListener(MAPSEARCHER.GLOBAL.DEFINES.drawingManager, 'circlecomplete', function (circle) {
                                MAPSEARCHER.UTILITIES.testBounds();
                                //handle if poi
                                if (MAPSEARCHER.GLOBAL.DEFINES.placerType == "poi") {
                                    MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                    MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                    MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                    MAPSEARCHER.GLOBAL.DEFINES.poi_i++;

                                    MAPSEARCHER.GLOBAL.DEFINES.label[MAPSEARCHER.GLOBAL.DEFINES.poi_i] = new MarkerWithLabel({
                                        position: circle.getCenter(), //position of real marker
                                        zIndex: 2,
                                        map: map,
                                        labelContent: MAPSEARCHER.GLOBAL.DEFINES.poi_i + 1, //the current user count
                                        labelAnchor: new google.maps.Point(15, 0),
                                        labelClass: "labels", // the CSS class for the label
                                        labelStyle: { opacity: 0.75 },
                                        icon: {} //initialize to nothing so no marker shows
                                    });

                                    var poiId = MAPSEARCHER.GLOBAL.DEFINES.poi_i + 1;
                                    MAPSEARCHER.GLOBAL.DEFINES.poiObj[MAPSEARCHER.GLOBAL.DEFINES.poi_i] = circle;
                                    MAPSEARCHER.GLOBAL.DEFINES.poiType[MAPSEARCHER.GLOBAL.DEFINES.poi_i] = "circle";
                                    var poiDescTemp = MAPSEARCHER.LOCALIZATION.DEFINES.L_Circle;
                                    document.getElementById("poiList").innerHTML += MAPSEARCHER.UTILITIES.writeHTML("poiListItem", MAPSEARCHER.GLOBAL.DEFINES.poi_i, poiId, poiDescTemp);
                                    var contentString = MAPSEARCHER.UTILITIES.writeHTML("poiDesc", MAPSEARCHER.GLOBAL.DEFINES.poi_i, "", "");
                                    MAPSEARCHER.GLOBAL.DEFINES.infoWindow[MAPSEARCHER.GLOBAL.DEFINES.poi_i] = new google.maps.InfoWindow({
                                        content: contentString
                                    });
                                    MAPSEARCHER.GLOBAL.DEFINES.infoWindow[MAPSEARCHER.GLOBAL.DEFINES.poi_i].setPosition(circle.getCenter());
                                    MAPSEARCHER.GLOBAL.DEFINES.infoWindow[MAPSEARCHER.GLOBAL.DEFINES.poi_i].open(map);
                                    //best fix so far
                                    if (MAPSEARCHER.GLOBAL.DEFINES.poiCount == 0) {
                                        setTimeout(function () {
                                            MAPSEARCHER.GLOBAL.DEFINES.infoWindow[0].setMap(null);
                                            MAPSEARCHER.GLOBAL.DEFINES.infoWindow[0].setMap(map);
                                        }, 800);
                                    }
                                    MAPSEARCHER.GLOBAL.DEFINES.poiCount++;

                                    google.maps.event.addListener(circle, 'dragstart', function () {
                                        MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                        MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                        MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                        for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiObj.length; i++) {
                                            if (MAPSEARCHER.GLOBAL.DEFINES.poiObj[i] == this) {
                                                MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setMap(null);
                                                MAPSEARCHER.GLOBAL.DEFINES.label[i].setMap(null);
                                            }
                                        }

                                    });
                                    google.maps.event.addListener(circle, 'dragend', function () {
                                        MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                        MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                        MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                        for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiObj.length; i++) {
                                            if (MAPSEARCHER.GLOBAL.DEFINES.poiObj[i] == this) {
                                                MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setPosition(circle.getCenter());
                                                MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].open(null);
                                                MAPSEARCHER.GLOBAL.DEFINES.label[i].setPosition(circle.getCenter());
                                                MAPSEARCHER.GLOBAL.DEFINES.label[i].setMap(map);
                                            }
                                        }

                                    });
                                    google.maps.event.addListener(circle, 'click', function () {
                                        MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                        MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                        MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                        for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiObj.length; i++) {
                                            if (MAPSEARCHER.GLOBAL.DEFINES.poiObj[i] == this) {
                                                MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setPosition(circle.getCenter());
                                                MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].open(map);
                                            }
                                        }

                                    });
                                    google.maps.event.addListener(circle, 'radius_changed', function () {
                                        MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                        MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                        MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                        for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiObj.length; i++) {
                                            if (MAPSEARCHER.GLOBAL.DEFINES.poiObj[i] == this) {
                                                MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setPosition(circle.getCenter());
                                                MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].open(null);
                                                MAPSEARCHER.GLOBAL.DEFINES.label[i].setPosition(circle.getCenter());
                                                MAPSEARCHER.GLOBAL.DEFINES.label[i].setMap(map);
                                            }
                                        }

                                    });
                                    google.maps.event.addListener(circle, 'center_changed', function () {
                                        MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                        MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                        MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                        for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiObj.length; i++) {
                                            if (MAPSEARCHER.GLOBAL.DEFINES.poiObj[i] == this) {
                                                MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setPosition(circle.getCenter());
                                                MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].open(null);
                                                MAPSEARCHER.GLOBAL.DEFINES.label[i].setPosition(circle.getCenter());
                                                MAPSEARCHER.GLOBAL.DEFINES.label[i].setMap(map);
                                            }
                                        }
                                    });
                                }
                                //regardless of type
                                //used for latlong tool
                                google.maps.event.addListener(circle, 'drag', function () {
                                    //used to get the center point for lat/long tool
                                    MAPSEARCHER.GLOBAL.DEFINES.circleCenter = circle.getCenter();
                                    var str = circle.getCenter().toString();
                                    var cLatV = str.replace("(", "").replace(")", "").split(",", 1);
                                    var cLongV = str.replace(cLatV, "").replace("(", "").replace(")", "").replace(",", ""); //is this better than passing into array?s
                                    if (cLatV.indexOf("-") != 0) {
                                        latH = "N";
                                    } else {
                                        latH = "S";
                                    }
                                    if (cLongV.indexOf("-") != 0) {
                                        longH = "W";
                                    } else {
                                        longH = "E";
                                    }
                                    document.getElementById("cLat").innerHTML = cLatV + " (" + latH + ")";
                                    document.getElementById("cLong").innerHTML = cLongV + " (" + longH + ")";
                                });
                                //set listener for right click (fixes reset issue over overlays)
                                google.maps.event.addListener(circle, 'rightclick', function () {
                                    MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setDrawingMode(null); //reset drawing manager no matter what
                                });

                            });
                            google.maps.event.addListener(MAPSEARCHER.GLOBAL.DEFINES.drawingManager, 'rectanglecomplete', function (rectangle) {
                                //check the bounds to make sure you havent strayed too far away
                                MAPSEARCHER.UTILITIES.testBounds();
                                //handle if an overlay
                                if (MAPSEARCHER.GLOBAL.DEFINES.placerType == "overlay") {
                                    MAPSEARCHER.ACTIONS.openToolboxTab("overlay");
                                    MAPSEARCHER.TRACER.addTracer("[INFO]: placertype: overlay");
                                    //assing working overlay index
                                    MAPSEARCHER.GLOBAL.DEFINES.workingOverlayIndex = MAPSEARCHER.GLOBAL.DEFINES.convertedOverlayIndex + 1;
                                    if (MAPSEARCHER.GLOBAL.DEFINES.overlayType == "drawn") {
                                        MAPSEARCHER.TRACER.addTracer("[INFO]: MAPSEARCHER.GLOBAL.DEFINES.overlayType: " + MAPSEARCHER.GLOBAL.DEFINES.overlayType);
                                        MAPSEARCHER.TRACER.addTracer("[INFO]: MAPSEARCHER.GLOBAL.DEFINES.convertedOverlayIndex: " + MAPSEARCHER.GLOBAL.DEFINES.convertedOverlayIndex);
                                        MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonPath[MAPSEARCHER.GLOBAL.DEFINES.convertedOverlayIndex] = rectangle.getBounds();
                                        //create overlay with incoming
                                        MAPSEARCHER.GLOBAL.DEFINES.overlaysOnMap[MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonPageId[MAPSEARCHER.GLOBAL.DEFINES.convertedOverlayIndex]] = new MAPSEARCHER.UTILITIES.CustomOverlay(MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonPageId[MAPSEARCHER.GLOBAL.DEFINES.convertedOverlayIndex], MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonPath[MAPSEARCHER.GLOBAL.DEFINES.convertedOverlayIndex], MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonSourceURL[MAPSEARCHER.GLOBAL.DEFINES.convertedOverlayIndex], map, MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonRotation[MAPSEARCHER.GLOBAL.DEFINES.convertedOverlayIndex]);
                                        MAPSEARCHER.GLOBAL.DEFINES.currentlyEditing = "no";
                                        //set the overlay to the map
                                        MAPSEARCHER.GLOBAL.DEFINES.overlaysOnMap[MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonPageId[MAPSEARCHER.GLOBAL.DEFINES.convertedOverlayIndex]].setMap(map);
                                        //set hotspot on top of overlay
                                        MAPSEARCHER.UTILITIES.setGhostOverlay(MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonPageId[MAPSEARCHER.GLOBAL.DEFINES.convertedOverlayIndex], MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonPath[MAPSEARCHER.GLOBAL.DEFINES.convertedOverlayIndex]);
                                        MAPSEARCHER.TRACER.addTracer("[INFO]: I created ghost: " + MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonPageId[MAPSEARCHER.GLOBAL.DEFINES.convertedOverlayIndex]);
                                        MAPSEARCHER.GLOBAL.DEFINES.mainCount++;
                                        MAPSEARCHER.GLOBAL.DEFINES.incomingACL = "overlay";
                                        //mark that we converted it
                                        MAPSEARCHER.GLOBAL.DEFINES.isConvertedOverlay = true;
                                        //hide the rectangle we drew
                                        rectangle.setMap(null);
                                        //relist the overlay we drew
                                        MAPSEARCHER.GLOBAL.initOverlayList();
                                        MAPSEARCHER.ACTIONS.overlayEditMe(MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonPageId[MAPSEARCHER.GLOBAL.DEFINES.convertedOverlayIndex]);
                                        //reset drawing manager no matter what
                                        MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setDrawingMode(null);
                                    } else {
                                        //mark that we are editing
                                        MAPSEARCHER.GLOBAL.DEFINES.firstSaveOverlay = true;
                                        //add the incoming overlay bounds
                                        MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonPath[MAPSEARCHER.GLOBAL.DEFINES.convertedOverlayIndex] = rectangle.getBounds();
                                        //redisplay overlays (the one we just made)
                                        MAPSEARCHER.GLOBAL.displayIncomingPolygons();
                                        //relist the overlay we drew
                                        MAPSEARCHER.GLOBAL.initOverlayList();
                                        //hide the rectangle we drew
                                        rectangle.setMap(null);
                                        //prevent redraw
                                        MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setDrawingMode(null);
                                        //MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setMap(null);
                                    }
                                    //create cache save overlay for the new converted overlay only
                                    if (MAPSEARCHER.GLOBAL.DEFINES.isConvertedOverlay == true) {
                                        MAPSEARCHER.UTILITIES.cacheSaveOverlay(MAPSEARCHER.GLOBAL.DEFINES.workingOverlayIndex);
                                        MAPSEARCHER.GLOBAL.DEFINES.isConvertedOverlay = false;
                                    }
                                }
                                //handle if poi
                                if (MAPSEARCHER.GLOBAL.DEFINES.placerType == "poi") {
                                    MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                    MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                    MAPSEARCHER.GLOBAL.DEFINES.poi_i++;

                                    MAPSEARCHER.GLOBAL.DEFINES.label[MAPSEARCHER.GLOBAL.DEFINES.poi_i] = new MarkerWithLabel({
                                        position: rectangle.getBounds().getCenter(), //position of real marker
                                        zIndex: 2,
                                        map: map,
                                        labelContent: MAPSEARCHER.GLOBAL.DEFINES.poi_i + 1, //the current user count
                                        labelAnchor: new google.maps.Point(15, 0),
                                        labelClass: "labels", // the CSS class for the label
                                        labelStyle: { opacity: 0.75 },
                                        icon: {} //initialize to nothing so no marker shows
                                    });

                                    var poiId = MAPSEARCHER.GLOBAL.DEFINES.poi_i + 1;
                                    MAPSEARCHER.GLOBAL.DEFINES.poiObj[MAPSEARCHER.GLOBAL.DEFINES.poi_i] = rectangle;
                                    MAPSEARCHER.GLOBAL.DEFINES.poiType[MAPSEARCHER.GLOBAL.DEFINES.poi_i] = "rectangle";
                                    var poiDescTemp = MAPSEARCHER.LOCALIZATION.DEFINES.L_Rectangle;
                                    document.getElementById("poiList").innerHTML += MAPSEARCHER.UTILITIES.writeHTML("poiListItem", MAPSEARCHER.GLOBAL.DEFINES.poi_i, poiId, poiDescTemp);
                                    var contentString = MAPSEARCHER.UTILITIES.writeHTML("poiDesc", MAPSEARCHER.GLOBAL.DEFINES.poi_i, "", "");
                                    MAPSEARCHER.GLOBAL.DEFINES.infoWindow[MAPSEARCHER.GLOBAL.DEFINES.poi_i] = new google.maps.InfoWindow({
                                        content: contentString
                                    });
                                    MAPSEARCHER.GLOBAL.DEFINES.infoWindow[MAPSEARCHER.GLOBAL.DEFINES.poi_i].setPosition(rectangle.getBounds().getCenter());
                                    MAPSEARCHER.GLOBAL.DEFINES.infoWindow[MAPSEARCHER.GLOBAL.DEFINES.poi_i].open(map);
                                    //best fix so far
                                    if (MAPSEARCHER.GLOBAL.DEFINES.poiCount == 0) {
                                        setTimeout(function () {
                                            MAPSEARCHER.GLOBAL.DEFINES.infoWindow[0].setMap(null);
                                            MAPSEARCHER.GLOBAL.DEFINES.infoWindow[0].setMap(map);
                                        }, 800);
                                    }
                                    MAPSEARCHER.GLOBAL.DEFINES.poiCount++;
                                    MAPSEARCHER.TRACER.addTracer("[INFO]: completed overlay bounds getter");

                                    google.maps.event.addListener(rectangle, 'bounds_changed', function () {
                                        MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                        MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                        MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                        for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiObj.length; i++) {
                                            if (MAPSEARCHER.GLOBAL.DEFINES.poiObj[i] == this) {
                                                MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setPosition(rectangle.getBounds().getCenter());
                                                MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setMap(null);
                                                MAPSEARCHER.GLOBAL.DEFINES.label[i].setPosition(rectangle.getBounds().getCenter());
                                                MAPSEARCHER.GLOBAL.DEFINES.label[i].setMap(map);
                                            }
                                        }

                                    });
                                    google.maps.event.addListener(rectangle, 'dragstart', function () {
                                        MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                        MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                        MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                        for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiObj.length; i++) {
                                            if (MAPSEARCHER.GLOBAL.DEFINES.poiObj[i] == this) {
                                                MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setMap(null);
                                                MAPSEARCHER.GLOBAL.DEFINES.label[i].setMap(null);
                                            }
                                        }

                                    });
                                    google.maps.event.addListener(rectangle, 'drag', function () {
                                        MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                        MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                        MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                        for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiObj.length; i++) {
                                            if (MAPSEARCHER.GLOBAL.DEFINES.poiObj[i] == this) {
                                                MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setMap(null);
                                                MAPSEARCHER.GLOBAL.DEFINES.label[i].setMap(null);
                                            }
                                        }


                                    });
                                    google.maps.event.addListener(rectangle, 'dragend', function () {
                                        MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                        MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                        MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                        for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiObj.length; i++) {
                                            if (MAPSEARCHER.GLOBAL.DEFINES.poiObj[i] == this) {
                                                MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setPosition(rectangle.getBounds().getCenter());
                                                MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].open(null);
                                                MAPSEARCHER.GLOBAL.DEFINES.label[i].setPosition(rectangle.getBounds().getCenter());
                                                MAPSEARCHER.GLOBAL.DEFINES.label[i].setMap(map);
                                            }
                                        }

                                    });
                                    google.maps.event.addListener(rectangle, 'click', function () {
                                        MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                        MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                        MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                        for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiObj.length; i++) {
                                            if (MAPSEARCHER.GLOBAL.DEFINES.poiObj[i] == this) {
                                                MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setPosition(rectangle.getBounds().getCenter());
                                                MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].open(map);
                                            }
                                        }

                                    });
                                }
                                //used to get center point for lat/long tool
                                google.maps.event.addListener(rectangle, 'drag', function () {
                                    var str = rectangle.getBounds().getCenter().toString();
                                    var cLatV = str.replace("(", "").replace(")", "").split(",", 1);
                                    var cLongV = str.replace(cLatV, "").replace("(", "").replace(")", "").replace(",", "");
                                    if (cLatV.indexOf("-") != 0) {
                                        latH = "N";
                                    } else {
                                        latH = "S";
                                    }
                                    if (cLongV.indexOf("-") != 0) {
                                        longH = "W";
                                    } else {
                                        longH = "E";
                                    }
                                    document.getElementById("cLat").innerHTML = cLatV + " (" + latH + ")";
                                    document.getElementById("cLong").innerHTML = cLongV + " (" + longH + ")";
                                });
                                //set listener for right click (fixes reset issue over overlays)
                                google.maps.event.addListener(rectangle, 'rightclick', function () {
                                    MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setDrawingMode(null); //reset drawing manager no matter what
                                });
                            });
                            google.maps.event.addListener(MAPSEARCHER.GLOBAL.DEFINES.drawingManager, 'polygoncomplete', function (polygon) {
                                MAPSEARCHER.UTILITIES.testBounds();
                                //handle if poi
                                if (MAPSEARCHER.GLOBAL.DEFINES.placerType == "poi") {
                                    MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                    MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                    MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                    MAPSEARCHER.GLOBAL.DEFINES.poi_i++;

                                    MAPSEARCHER.GLOBAL.DEFINES.label[MAPSEARCHER.GLOBAL.DEFINES.poi_i] = new MarkerWithLabel({
                                        position: MAPSEARCHER.UTILITIES.polygonCenter(polygon), //position of real marker
                                        zIndex: 2,
                                        map: map,
                                        labelContent: MAPSEARCHER.GLOBAL.DEFINES.poi_i + 1, //the current user count
                                        labelAnchor: new google.maps.Point(15, 0),
                                        labelClass: "labels", // the CSS class for the label
                                        labelStyle: { opacity: 0.75 },
                                        icon: {} //initialize to nothing so no marker shows
                                    });

                                    var poiId = MAPSEARCHER.GLOBAL.DEFINES.poi_i + 1;
                                    MAPSEARCHER.GLOBAL.DEFINES.poiObj[MAPSEARCHER.GLOBAL.DEFINES.poi_i] = polygon;
                                    MAPSEARCHER.GLOBAL.DEFINES.poiType[MAPSEARCHER.GLOBAL.DEFINES.poi_i] = "polygon";
                                    var poiDescTemp = MAPSEARCHER.LOCALIZATION.DEFINES.L_Polygon;
                                    document.getElementById("poiList").innerHTML += MAPSEARCHER.UTILITIES.writeHTML("poiListItem", MAPSEARCHER.GLOBAL.DEFINES.poi_i, poiId, poiDescTemp);
                                    var contentString = MAPSEARCHER.UTILITIES.writeHTML("poiDesc", MAPSEARCHER.GLOBAL.DEFINES.poi_i, "", "");
                                    MAPSEARCHER.GLOBAL.DEFINES.infoWindow[MAPSEARCHER.GLOBAL.DEFINES.poi_i] = new google.maps.InfoWindow({
                                        content: contentString
                                    });
                                    MAPSEARCHER.GLOBAL.DEFINES.infoWindow[MAPSEARCHER.GLOBAL.DEFINES.poi_i].setPosition(MAPSEARCHER.UTILITIES.polygonCenter(polygon));
                                    MAPSEARCHER.GLOBAL.DEFINES.infoWindow[MAPSEARCHER.GLOBAL.DEFINES.poi_i].open(map);
                                    //best fix so far
                                    if (MAPSEARCHER.GLOBAL.DEFINES.poiCount == 0) {
                                        setTimeout(function () {
                                            MAPSEARCHER.GLOBAL.DEFINES.infoWindow[0].setMap(null);
                                            MAPSEARCHER.GLOBAL.DEFINES.infoWindow[0].setMap(map);
                                        }, 800);
                                    }
                                    MAPSEARCHER.GLOBAL.DEFINES.poiCount++;

                                    google.maps.event.addListener(polygon.getPath(), 'set_at', function () { //if bounds change
                                        MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                        MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                        MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                        for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiObj.length; i++) {
                                            if (MAPSEARCHER.GLOBAL.DEFINES.poiObj[i] == this) {
                                                MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setPosition(MAPSEARCHER.UTILITIES.polygonCenter(polygon));
                                                MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setMap(null);
                                                MAPSEARCHER.GLOBAL.DEFINES.label[i].setPosition(MAPSEARCHER.UTILITIES.polygonCenter(polygon));
                                                MAPSEARCHER.GLOBAL.DEFINES.label[i].setMap(map); //does not redisplay
                                            }
                                        }
                                    });
                                    google.maps.event.addListener(polygon, 'dragstart', function () {
                                        MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                        MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                        MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                        for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiObj.length; i++) {
                                            if (MAPSEARCHER.GLOBAL.DEFINES.poiObj[i] == this) {
                                                MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setMap(null);
                                                MAPSEARCHER.GLOBAL.DEFINES.label[i].setMap(null);
                                            }
                                        }

                                    });
                                    google.maps.event.addListener(polygon, 'drag', function () {
                                        MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                        MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                        MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                        for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiObj.length; i++) {
                                            if (MAPSEARCHER.GLOBAL.DEFINES.poiObj[i] == this) {
                                                MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setMap(null);
                                                MAPSEARCHER.GLOBAL.DEFINES.label[i].setMap(null);
                                            }
                                        }

                                    });
                                    google.maps.event.addListener(polygon, 'dragend', function () {
                                        MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                        MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                        MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                        for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiObj.length; i++) {
                                            if (MAPSEARCHER.GLOBAL.DEFINES.poiObj[i] == this) {
                                                MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setPosition(MAPSEARCHER.UTILITIES.polygonCenter(polygon));
                                                MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].open(null);
                                                MAPSEARCHER.GLOBAL.DEFINES.label[i].setPosition(MAPSEARCHER.UTILITIES.polygonCenter(polygon));
                                                MAPSEARCHER.GLOBAL.DEFINES.label[i].setMap(map);
                                            }
                                        }

                                    });
                                    google.maps.event.addListener(polygon, 'click', function () {
                                        MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                        MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                        MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                        for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiObj.length; i++) {
                                            if (MAPSEARCHER.GLOBAL.DEFINES.poiObj[i] == this) {
                                                MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setPosition(MAPSEARCHER.UTILITIES.polygonCenter(polygon));
                                                MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].open(map);
                                            }
                                        }

                                    });
                                }
                                //used for lat/long tool regardless if poi or not
                                google.maps.event.addListener(polygon, 'drag', function () {

                                    var str = MAPSEARCHER.UTILITIES.polygonCenter(polygon).toString();
                                    var cLatV = str.replace("(", "").replace(")", "").split(",", 1);
                                    var cLongV = str.replace(cLatV, "").replace("(", "").replace(")", "").replace(",", ""); //is this better than passing into array?s
                                    if (cLatV.indexOf("-") != 0) {
                                        latH = "N";
                                    } else {
                                        latH = "S";
                                    }
                                    if (cLongV.indexOf("-") != 0) {
                                        longH = "W";
                                    } else {
                                        longH = "E";
                                    }
                                    document.getElementById("cLat").innerHTML = cLatV + " (" + latH + ")";
                                    document.getElementById("cLong").innerHTML = cLongV + " (" + longH + ")";
                                });
                                //set listener for right click (fixes reset issue over overlays)
                                google.maps.event.addListener(polygon, 'rightclick', function () {
                                    MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setDrawingMode(null); //reset drawing manager no matter what
                                });
                            });
                            google.maps.event.addListener(MAPSEARCHER.GLOBAL.DEFINES.drawingManager, 'polylinecomplete', function (polyline) {
                                //make sure we are still in the bounds
                                MAPSEARCHER.UTILITIES.testBounds();
                                //handle if this is a polygon
                                if (MAPSEARCHER.GLOBAL.DEFINES.placerType == "poi") {
                                    MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                    MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                    MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                    MAPSEARCHER.GLOBAL.DEFINES.poi_i++;
                                    var poiId = MAPSEARCHER.GLOBAL.DEFINES.poi_i + 1;
                                    MAPSEARCHER.GLOBAL.DEFINES.poiObj[MAPSEARCHER.GLOBAL.DEFINES.poi_i] = polyline;
                                    MAPSEARCHER.GLOBAL.DEFINES.poiType[MAPSEARCHER.GLOBAL.DEFINES.poi_i] = "polyline";
                                    var poiDescTemp = MAPSEARCHER.LOCALIZATION.DEFINES.L_Line;
                                    document.getElementById("poiList").innerHTML += MAPSEARCHER.UTILITIES.writeHTML("poiListItem", MAPSEARCHER.GLOBAL.DEFINES.poi_i, poiId, poiDescTemp);
                                    var contentString = MAPSEARCHER.UTILITIES.writeHTML("poiDesc", MAPSEARCHER.GLOBAL.DEFINES.poi_i, "", "");
                                    MAPSEARCHER.GLOBAL.DEFINES.infoWindow[MAPSEARCHER.GLOBAL.DEFINES.poi_i] = new google.maps.InfoWindow({ content: contentString });
                                    var polylinePoints = [];
                                    var polylinePointCount = 0;
                                    polyline.getPath().forEach(function (latLng) {
                                        polylinePoints[polylinePointCount] = latLng;
                                        MAPSEARCHER.TRACER.addTracer("[INFO]: polylinePoints[" + polylinePointCount + "] = " + latLng);
                                        polylinePointCount++;
                                    });
                                    MAPSEARCHER.TRACER.addTracer("[INFO]: polylinePointCount: " + polylinePointCount);
                                    MAPSEARCHER.TRACER.addTracer("[INFO]: polylinePoints.length: " + polylinePoints.length);
                                    MAPSEARCHER.TRACER.addTracer("[INFO]: Math.round((polylinePoints.length / 2)): " + Math.round((polylinePoints.length / 2)));
                                    var polylineCenterPoint = polylinePoints[Math.round((polylinePoints.length / 2))];
                                    MAPSEARCHER.TRACER.addTracer("[INFO]: polylineCenterPoint: " + polylineCenterPoint);
                                    var polylineStartPoint = polylinePoints[0];
                                    MAPSEARCHER.TRACER.addTracer("[INFO]: polylineStartPoint: " + polylineStartPoint);
                                    MAPSEARCHER.GLOBAL.DEFINES.infoWindow[MAPSEARCHER.GLOBAL.DEFINES.poi_i].setPosition(polylineStartPoint);
                                    MAPSEARCHER.GLOBAL.DEFINES.infoWindow[MAPSEARCHER.GLOBAL.DEFINES.poi_i].open(map);

                                    //best fix so far
                                    if (MAPSEARCHER.GLOBAL.DEFINES.poiCount == 0) {
                                        setTimeout(function () {
                                            MAPSEARCHER.GLOBAL.DEFINES.infoWindow[0].setMap(null);
                                            MAPSEARCHER.GLOBAL.DEFINES.infoWindow[0].setMap(map);
                                        }, 800);
                                    }
                                    MAPSEARCHER.GLOBAL.DEFINES.poiCount++;
                                    //create the label
                                    MAPSEARCHER.GLOBAL.DEFINES.label[MAPSEARCHER.GLOBAL.DEFINES.poi_i] = new MarkerWithLabel({
                                        position: polylineStartPoint, //position at start of polyline
                                        zIndex: 2,
                                        map: map,
                                        labelContent: poiId, //the current user count
                                        labelAnchor: new google.maps.Point(15, 0),
                                        labelClass: "labels", // the CSS class for the label
                                        labelStyle: { opacity: 0.75 },
                                        icon: {} //initialize to nothing so no marker shows
                                    });
                                    //add poi specific listeners
                                    google.maps.event.addListener(polyline.getPath(), 'set_at', function () {
                                        MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                        MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                        MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                        MAPSEARCHER.TRACER.addTracer("[INFO]: is poi");
                                        for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiObj.length; i++) {
                                            MAPSEARCHER.TRACER.addTracer("[INFO]: inside loop1");
                                            if (MAPSEARCHER.GLOBAL.DEFINES.poiObj[i].getPath() == this) {
                                                var polylinePoints = [];
                                                var polylinePointCount = 0;
                                                MAPSEARCHER.TRACER.addTracer("[INFO]: here1");
                                                polyline.getPath().forEach(function (latLng) {
                                                    polylinePoints[polylinePointCount] = latLng;
                                                    polylinePointCount++;
                                                });
                                                MAPSEARCHER.TRACER.addTracer("[INFO]: here2");
                                                var polylineCenterPoint = polylinePoints[(polylinePoints.length / 2)];
                                                var polylineStartPoint = polylinePoints[0];
                                                MAPSEARCHER.GLOBAL.DEFINES.infoWindow[MAPSEARCHER.GLOBAL.DEFINES.poi_i].setPosition(polylineStartPoint);
                                                MAPSEARCHER.GLOBAL.DEFINES.infoWindow[MAPSEARCHER.GLOBAL.DEFINES.poi_i].open(null);
                                                MAPSEARCHER.GLOBAL.DEFINES.label[i].setPosition(polylineStartPoint);
                                                MAPSEARCHER.GLOBAL.DEFINES.label[i].setMap(map);
                                                MAPSEARCHER.TRACER.addTracer("[INFO]: here3");
                                            }
                                        }
                                    });
                                    google.maps.event.addListener(polyline, 'dragstart', function () {
                                        MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                        MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                        MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                        for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiObj.length; i++) {
                                            if (MAPSEARCHER.GLOBAL.DEFINES.poiObj[i] == this) {
                                                MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setMap(null);
                                                MAPSEARCHER.GLOBAL.DEFINES.label[i].setMap(null);
                                            }
                                        }
                                    });
                                    google.maps.event.addListener(polyline, 'click', function () {
                                        MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                        MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                        MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                        for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiObj.length; i++) {
                                            if (MAPSEARCHER.GLOBAL.DEFINES.poiObj[i] == this) {
                                                var polylinePoints = [];
                                                var polylinePointCount = 0;
                                                polyline.getPath().forEach(function (latLng) {
                                                    polylinePoints[polylinePointCount] = latLng;
                                                    polylinePointCount++;
                                                });
                                                var polylineCenterPoint = polylinePoints[(polylinePoints.length / 2)];
                                                var polylineStartPoint = polylinePoints[0];
                                                MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setPosition(polylineStartPoint);
                                                MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].open(map);
                                            }
                                        }
                                    });
                                    google.maps.event.addListener(polyline, 'dragend', function () {
                                        MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                        MAPSEARCHER.ACTIONS.openToolboxTab("poi");
                                        MAPSEARCHER.GLOBAL.DEFINES.firstSavePOI = true;
                                        for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiObj.length; i++) {
                                            if (MAPSEARCHER.GLOBAL.DEFINES.poiObj[i] == this) {
                                                var polylinePoints = [];
                                                var polylinePointCount = 0;
                                                polyline.getPath().forEach(function (latLng) {
                                                    polylinePoints[polylinePointCount] = latLng;
                                                    polylinePointCount++;
                                                });
                                                var polylineCenterPoint = polylinePoints[(polylinePoints.length / 2)];
                                                var polylineStartPoint = polylinePoints[0];
                                                MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].setPosition(polylineStartPoint);
                                                MAPSEARCHER.GLOBAL.DEFINES.infoWindow[i].open(null);
                                                MAPSEARCHER.GLOBAL.DEFINES.label[i].setPosition(polylineStartPoint);
                                                MAPSEARCHER.GLOBAL.DEFINES.label[i].setMap(map);
                                            }
                                        }
                                    });
                                }
                                //regardless of type
                                //used for lat/long tool
                                google.maps.event.addListener(polyline, 'drag', function () {
                                    //used for lat/long tool
                                    var bounds = new google.maps.LatLngBounds;
                                    polyline.getPath().forEach(function (latLng) { bounds.extend(latLng); });
                                    var polylineCenter = bounds.getCenter();
                                    var str = polylineCenter.toString();
                                    var cLatV = str.replace("(", "").replace(")", "").split(",", 1);
                                    var cLongV = str.replace(cLatV, "").replace("(", "").replace(")", "").replace(",", ""); //is this better than passing into array?s
                                    if (cLatV.indexOf("-") != 0) {
                                        latH = "N";
                                    } else {
                                        latH = "S";
                                    }
                                    if (cLongV.indexOf("-") != 0) {
                                        longH = "W";
                                    } else {
                                        longH = "E";
                                    }
                                    document.getElementById("cLat").innerHTML = cLatV + " (" + latH + ")";
                                    document.getElementById("cLong").innerHTML = cLongV + " (" + longH + ")";
                                });
                                //set listener for right click (fixes reset issue over overlays)
                                google.maps.event.addListener(polyline, 'rightclick', function () {
                                    MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setDrawingMode(null); //reset drawing manager no matter what
                                    //MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setMap(null);
                                });
                            });
                            
                            //initialize map specific listeners
                            //on map click, set focus (fixes keycode issue)
                            google.maps.event.addListener(map, 'click', function () {
                                document.activeElement.blur();
                            });
                            //on right click stop drawing thing
                            google.maps.event.addListener(map, 'rightclick', function () {
                                MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setDrawingMode(null); //reset drawing manager no matter what
                            });
                            //used to display cursor location via lat/long
                            google.maps.event.addDomListener(map, 'mousemove', function (point) {

                                if (MAPSEARCHER.GLOBAL.DEFINES.cCoordsFrozen == "no") {
                                    var str = point.latLng.toString();
                                    var cLatV = str.replace("(", "").replace(")", "").split(",", 1);
                                    var cLongV = str.replace(cLatV, "").replace("(", "").replace(")", "").replace(",", ""); //is this better than passing into array?s
                                    if (cLatV.indexOf("-") != 0) {
                                        latH = "N";
                                    } else {
                                        latH = "S";
                                    }
                                    if (cLongV.indexOf("-") != 0) {
                                        longH = "W";
                                    } else {
                                        longH = "E";
                                    }

                                    document.getElementById("cLat").innerHTML = cLatV + " (" + latH + ")";
                                    document.getElementById("cLong").innerHTML = cLongV + " (" + longH + ")";
                                }

                            });
                            //drag listener (for boundary test)
                            google.maps.event.addListener(map, 'dragend', function () {
                                MAPSEARCHER.UTILITIES.testBounds();
                            });
                            //check the zoom level display message if out limits
                            google.maps.event.addListener(map, 'zoom_changed', function () {
                                MAPSEARCHER.UTILITIES.checkZoomLevel();
                            });
                            //when kml layer is clicked, get feature that was clicked
                            google.maps.event.addListener(MAPSEARCHER.GLOBAL.DEFINES.kmlLayer, 'click', function (kmlEvent) {
                                var name = kmlEvent.featureData.name;
                                MAPSEARCHER.UTILITIES.displayMessage("ParcelID: " + name); //temp
                            });

                            //#endregion

                            //initialize all the incoming geo obejects (the fcn is written via c#)
                            initGeoObjects();

                            //this part runs when the mapobject is created and rendered
                            google.maps.event.addListenerOnce(map, 'tilesloaded', function () {
                                MAPSEARCHER.GLOBAL.initOptions(); //setup the graphical user interface (enhances visual effect to do all of this after map loads)
                                MAPSEARCHER.GLOBAL.initOverlayList(); //list all the overlays in the list box"
                                MAPSEARCHER.GLOBAL.initNonStaticVars(); //these must be loaded after everything else is completed
                                MAPSEARCHER.UTILITIES.resizeView(); //explicitly call resizer, fixes unknow issue where timeout of divs being added when there are a lot (IE many overlays)
                            });

                            MAPSEARCHER.TRACER.addTracer("[INFO]: initGMap completed...");
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    initGMapObjects: function () {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: MAPSEARCHER.GLOBAL.initGMapObjects started...");

                            //adds listener support for custom overlay
                            //Supporting URL: https://developers.google.com/maps/documentation/javascript/overlays#CustomOverlays
                            // Note: an overlay's receipt of onAdd() indicates that the map's panes are now available for attaching the overlay to the map via the DOM.
                            MAPSEARCHER.UTILITIES.CustomOverlay.prototype.onAdd = function () {

                                // Create the DIV and set some basic attributes.
                                var div = document.createElement("div");
                                div.id = "overlay" + this.index_;
                                div.style.borderStyle = 'none';
                                div.style.borderWidth = '0px';
                                div.style.position = 'absolute';
                                div.style.opacity = MAPSEARCHER.GLOBAL.DEFINES.preservedOpacity;

                                // Create an IMG element and attach it to the DIV.
                                var img = document.createElement('img');
                                img.src = this.image_;
                                img.style.width = '100%';
                                img.style.height = '100%';
                                img.style.position = 'absolute';
                                div.appendChild(img);

                                // Set the overlay's div_ property to this DIV
                                this.div_ = div;

                                // We add an overlay to a map via one of the map's panes.
                                // We'll add this overlay to the overlayLayer pane.
                                var panes = this.getPanes();
                                panes.overlayLayer.appendChild(div);
                            };
                            //Continues support for adding an custom overlay
                            //Supporting URL: https://developers.google.com/maps/documentation/javascript/overlays#CustomOverlays
                            MAPSEARCHER.UTILITIES.CustomOverlay.prototype.draw = function () {
                                // Size and position the overlay. We use a southwest and northeast
                                // position of the overlay to peg it to the correct position and size.
                                // We need to retrieve the projection from this overlay to do this.
                                var overlayProjection = this.getProjection();

                                // Retrieve the southwest and northeast coordinates of this overlay
                                // in latlngs and convert them to pixels coordinates.
                                // We'll use these coordinates to resize the DIV.
                                var sw = overlayProjection.fromLatLngToDivPixel(this.bounds_.getSouthWest());
                                var ne = overlayProjection.fromLatLngToDivPixel(this.bounds_.getNorthEast());

                                // Resize the image's DIV to fit the indicated dimensions.
                                var div = this.div_;
                                div.style.left = sw.x + 'px';
                                div.style.top = ne.y + 'px';
                                div.style.width = (ne.x - sw.x) + 'px';
                                div.style.height = (sw.y - ne.y) + 'px';

                                //for a rotation
                                //hold woi to later put it back in (fixes keepRotate error)
                                var temp = MAPSEARCHER.GLOBAL.DEFINES.workingOverlayIndex;
                                //set woi to incoming (fixes keepRotate error)
                                MAPSEARCHER.GLOBAL.DEFINES.workingOverlayIndex = this.index_;
                                //check to see if saving rotaiotn and then incoming (this allows all overlays to have a rotation but places priority to the saving overlay rotation)
                                if (MAPSEARCHER.GLOBAL.DEFINES.savingOverlayRotation[this.index_ - 1] != undefined) {
                                    MAPSEARCHER.UTILITIES.keepRotate(MAPSEARCHER.GLOBAL.DEFINES.savingOverlayRotation[this.index_ - 1]);
                                } else {
                                    MAPSEARCHER.UTILITIES.keepRotate(MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonRotation[(this.index_ - 1)]);
                                }
                                //reset woi to temp just in case we had something different/useful
                                MAPSEARCHER.GLOBAL.DEFINES.workingOverlayIndex = temp;
                            };
                            //Not currently used
                            //Supporting URL: https://developers.google.com/maps/documentation/javascript/overlays#CustomOverlays
                            MAPSEARCHER.UTILITIES.CustomOverlay.prototype.onRemove = function () {
                                this.div_.parentNode.removeChild(this.div_);
                                this.div_ = null;
                            };
                            //holds all the cusotm map options
                            MAPSEARCHER.GLOBAL.DEFINES.gmapOptions = {
                                disableDefaultUI: false,                                    //set to false to start from a clean slate of map controls
                                zoom: MAPSEARCHER.GLOBAL.DEFINES.defaultZoomLevel,                                     //starting zoom level
                                minZoom: MAPSEARCHER.GLOBAL.DEFINES.maxZoomLevel,                                      //highest zoom out level
                                center: MAPSEARCHER.GLOBAL.DEFINES.mapCenter,                                          //default center point
                                mapTypeId: google.maps.MapTypeId.ROADMAP,                   //default map type to display
                                streetViewControl: false,                                   //is streetview active?
                                tilt: 0,                                                    //set to 0 to disable 45 degree tilt
                                zoomControl: false,                                         //is zoom control active?
                                zoomControlOptions: {
                                    style: google.maps.ZoomControlStyle.SMALL,                //zoom control style
                                    position: google.maps.ControlPosition.LEFT_TOP            //zoom control position 
                                },
                                panControl: false,                                          //pan control active
                                panControlOptions: {
                                    position: google.maps.ControlPosition.LEFT_TOP            //pan control position
                                },
                                mapTypeControl: false,                                      //map layer control active
                                mapTypeControlOptions: {
                                    style: google.maps.MapTypeControlStyle.DROPDOWN_MENU,     //map layer control style
                                    position: google.maps.ControlPosition.RIGHT_TOP           //map layer control position
                                },
                                styles:                                                     //turn off all poi stylers (supporting url: https://developers.google.com/maps/documentation/javascript/reference#MapTypeStyleFeatureType)
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

                            };
                            //define drawing manager for this google maps instance
                            //support url: https://developers.google.com/maps/documentation/javascript/3.exp/reference#DrawingManager
                            MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setOptions({
                                //drawingMode: google.maps.drawing.OverlayType.MARKER, //set default/start type
                                drawingControl: false,
                                drawingControlOptions: {
                                    position: google.maps.ControlPosition.RIGHT_TOP,
                                    drawingModes: [
                                        google.maps.drawing.OverlayType.MARKER,
                                        google.maps.drawing.OverlayType.CIRCLE,
                                        google.maps.drawing.OverlayType.RECTANGLE,
                                        google.maps.drawing.OverlayType.POLYGON,
                                        google.maps.drawing.OverlayType.POLYLINE
                                    ]
                                },
                                markerOptions: MAPSEARCHER.GLOBAL.DEFINES.markerOptionsDefault,
                                circleOptions: MAPSEARCHER.GLOBAL.DEFINES.circleOptionsDefault,
                                polygonOptions: MAPSEARCHER.GLOBAL.DEFINES.polygonOptionsDefault,
                                polylineOptions: MAPSEARCHER.GLOBAL.DEFINES.polylineOptionsDefault,
                                rectangleOptions: MAPSEARCHER.GLOBAL.DEFINES.rectangleOptionsDefault
                            });
                            //define custom copyright control
                            //supporting url: https://developers.google.com/maps/documentation/javascript/controls#CustomControls
                            MAPSEARCHER.GLOBAL.DEFINES.copyrightNode = document.createElement('div');
                            MAPSEARCHER.GLOBAL.DEFINES.copyrightNode.id = 'copyright-control';
                            MAPSEARCHER.GLOBAL.DEFINES.copyrightNode.style.fontSize = '10px';
                            MAPSEARCHER.GLOBAL.DEFINES.copyrightNode.style.color = '#333333';
                            MAPSEARCHER.GLOBAL.DEFINES.copyrightNode.style.fontFamily = 'Arial, sans-serif';
                            MAPSEARCHER.GLOBAL.DEFINES.copyrightNode.style.margin = '0 2px 2px 0';
                            MAPSEARCHER.GLOBAL.DEFINES.copyrightNode.style.whiteSpace = 'nowrap';
                            MAPSEARCHER.GLOBAL.DEFINES.copyrightNode.index = 0;
                            MAPSEARCHER.GLOBAL.DEFINES.copyrightNode.style.backgroundColor = '#FFFFFF';
                            MAPSEARCHER.GLOBAL.DEFINES.copyrightNode.style.opacity = 0.71;
                            MAPSEARCHER.GLOBAL.DEFINES.copyrightNode.innerHTML = MAPSEARCHER.LOCALIZATION.DEFINES.L1; //localization copyright
                            //define cursor lat long tool custom control
                            //supporting url: https://developers.google.com/maps/documentation/javascript/controls#CustomControls
                            MAPSEARCHER.GLOBAL.DEFINES.cursorLatLongTool = document.createElement('div');
                            MAPSEARCHER.GLOBAL.DEFINES.cursorLatLongTool.id = 'cursorLatLongTool';
                            MAPSEARCHER.GLOBAL.DEFINES.cursorLatLongTool.style.fontSize = '10px';
                            MAPSEARCHER.GLOBAL.DEFINES.cursorLatLongTool.style.color = '#333333';
                            MAPSEARCHER.GLOBAL.DEFINES.cursorLatLongTool.style.fontFamily = 'Arial, sans-serif';
                            MAPSEARCHER.GLOBAL.DEFINES.cursorLatLongTool.style.margin = '0 2px 2px 0';
                            MAPSEARCHER.GLOBAL.DEFINES.cursorLatLongTool.style.whiteSpace = 'nowrap';
                            MAPSEARCHER.GLOBAL.DEFINES.cursorLatLongTool.index = 0;
                            MAPSEARCHER.GLOBAL.DEFINES.cursorLatLongTool.style.backgroundColor = '#FFFFFF';
                            MAPSEARCHER.GLOBAL.DEFINES.cursorLatLongTool.style.opacity = 0.71;
                            MAPSEARCHER.GLOBAL.DEFINES.cursorLatLongTool.innerHTML = MAPSEARCHER.LOCALIZATION.DEFINES.L2; //localization cursor lat/long tool
                            //buffer zone top left (used to push map controls down)
                            //supporting url: https://developers.google.com/maps/documentation/javascript/controls#CustomControls
                            MAPSEARCHER.GLOBAL.DEFINES.toolbarBufferZone1 = document.createElement('div');
                            MAPSEARCHER.GLOBAL.DEFINES.toolbarBufferZone1.id = 'toolbarBufferZone1';
                            MAPSEARCHER.GLOBAL.DEFINES.toolbarBufferZone1.style.height = '50px';
                            //buffer zone top right
                            //supporting url: https://developers.google.com/maps/documentation/javascript/controls#CustomControls
                            MAPSEARCHER.GLOBAL.DEFINES.toolbarBufferZone2 = document.createElement('div');
                            MAPSEARCHER.GLOBAL.DEFINES.toolbarBufferZone2.id = 'toolbarBufferZone2';
                            MAPSEARCHER.GLOBAL.DEFINES.toolbarBufferZone2.style.height = '50px';

                            MAPSEARCHER.TRACER.addTracer("[INFO]: MAPSEARCHER.GLOBAL.initGMapObjects completed...");
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    initNonStaticVars: function () {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: initNonStaticVars started...");
                            //explicityl define nonstatic vars
                            document.getElementById("debugVersionNumber").innerHTML = MAPSEARCHER.TRACER.DEFINES.debugVersionNumber;
                            MAPSEARCHER.TRACER.addTracer("[INFO]: initNonStaticVars completed...");
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    }
                };
            }(),          //global vars and inits
            LOCALIZATION: function () {
                return {
                    DEFINES: function () {
                        return {
                            L_Error1: "ERROR: Failed Adding Listeners",
                            L_Marker: "Marker",
                            L_Circle: "Circle",
                            L_Rectangle: "Rectangle",
                            L_Polygon: "Polygon",
                            L_Line: "Line",
                            L_Saved: "Saved",
                            L_Applied: "Applied",
                            L_Completed: "Completed",
                            L_Working: "Working...",
                            L_NotSaved: "Nothing To Save",
                            L_NotCleared: "Nothing To Reset",
                            L_Save: "Save",
                            L_Apply: "Apply",
                            L_Editing: "Editing",
                            L_Removed: "Removed",
                            L_Showing: "Showing",
                            L_Hiding: "Hiding",
                            L_Deleted: "Deleted",
                            L_NotDeleted: "Nothing To Delete",
                            L1: "<div style=\"font-size:.95em,\">SobekCM Plugin <div id=\"debugVersionNumber\"></div> &nbsp&nbsp&nbsp <a target=\"_blank\" href=\"http://www.uflib.ufl.edu/accesspol.html\" style=\"font-size:9px,text-decoration:none,\">Legal</a> &nbsp&nbsp&nbsp <a target=\"_blank\" href=\"http://ufdc.ufl.edu/contact\" style=\"font-size:9px,text-decoration:none,\">Report a Sobek error</a> &nbsp</div>",
                            L2: "lat: <a id=\"cLat\"></a><br/>long: <a id=\"cLong\"></a>",
                            L3: "Description (Optional)",
                            L4: "Geolocation Service Failed.",
                            L5: "Returned to Bounds!",
                            L6: "Could not find location. Either the format you entered is invalid or the location is outside of the map bounds.",
                            L7: "Error: Overlay image source cannot contain a ~ or |",
                            L8: "Error: Description cannot contain a ~ or |",
                            L9: "Item Location Reset!",
                            L10: "Overlays Reset!",
                            L11: "POI Set Cleared!",
                            L12: "Nothing Happened!",
                            L13: "Item Saved!",
                            L14: "Overlay Saved!",
                            L15: "POI Set Saved!",
                            L16: "Cannot Zoom Out Further",
                            L17: "Cannot Zoom In Further",
                            L18: "Using Search Results as Location",
                            L19: "Coordinates Copied To Clipboard",
                            L20: "Coordinates Viewer Frozen",
                            L21: "Coordinates Viewer UnFrozen",
                            L22: "Hiding Overlays",
                            L23: "Showing Overlays",
                            L24: "Could not find within bounds.",
                            L25: "geocoder failed due to:",
                            L26: "Overlay Editing Turned Off",
                            L27: "Overlay Editing Turned On",
                            L28: "ERROR: Failed Adding Titles",
                            L29: "ERROR: Failed Adding Textual Content",
                            L30: "Edit Location by Dragging Exisiting Marker",
                            L31: "Hiding",
                            L32: "Showing",
                            L33: "Removed",
                            L34: "Editing",
                            L35: "Apply Changes (Make Changes Public)",
                            L36: "Apply",
                            L37: "Save",
                            L38: "Save to Temporary File",
                            L39: "Nothing To Search",
                            L40: "Cannot Convert",
                            L41: "Select The Area To Draw The Overlay",
                            L42: "Hiding POIs",
                            L43: "Showing POIs",
                            L44: "Converted Item To Overlay",
                            L45: "Nothing To Toggle",
                            L46: "Not Cleared",
                            L47: "Warning! This will erase any changes you have made. Do you still want to proceed?",
                            L48: "Reseting Page",
                            L49: "Did Not Reset Page",
                            L50: "Finding Your Location.",
                            L51: "Error Adding Other Listeners",
                            L52: "Reseting Overlays",
                            L53: "Reseting POIs",
                            L54: "This will delete the geographic coordinate data for this overlay, are you sure?",
                            L55: "Coordinate Data Removed For",
                            L56: "Nothing to Hide",
                            L57: "Nothing to Delete",
                            L58: "Canceling...",
                            L59: "Saving...",
                            L60: "Edit This Overlay",
                            L61: "Toggle On Map",
                            L62: "Delete Search Result",
                            L63: "Delete POI",
                            L64: "Delete Coordinate Data For Overlay",
                            L65: "Save This Description",
                            L66: "Edit This POI",
                            L67: "Item Location Converted to Listing Overlays",
                            L68: "Overaly Geographic Data Deleted",
                            L69: "Item Geographic Location Deleted",
                            L70: "This will delete the geographic coordinate data for this item, are you sure?",
                            L71: "Save Description",
                            L72: "This will delete all of the POIs, are you sure?"
                        };
                    }(),
                    localizeByTooltips: function () {
                        try {
                            
                            MAPSEARCHER.TRACER.addTracer("[INFO]: localizeByTooltips started...");
                            //toolbar
                            document.getElementById("content_toolbar_button_reset").className += " lytetip";
                            //document.getElementById("content_toolbar_button_reset").createAttribute("data-tip");
                            document.getElementById("content_toolbar_button_reset").title = "Reset Me";
                            //document.getElementById("content_toolbar_button_reset").createAttribute('data-tip', 'Reset: Reset Map To Defaults');
                            document.getElementById("content_toolbar_button_toggleMapControls").title = "Controls: Toggle Map Controls";
                            document.getElementById("content_toolbar_button_toggleToolbox").title = "Toolbox: Toggle Toolbox";
                            document.getElementById("content_toolbar_button_layerRoadmap").title = "Roadmap: Toggle Road Map Layer";
                            document.getElementById("content_toolbar_button_layerTerrain").title = "Terrain: Toggle Terrain Map Layer";
                            document.getElementById("content_toolbar_button_layerSatellite").title = "Satellite: Toggle Satellite Map Layer";
                            document.getElementById("content_toolbar_button_layerHybrid").title = "Hybrid: Toggle Hybrid Map Layer";
                            document.getElementById("content_toolbar_button_layerCustom").title = "Block/Lot: Toggle Block/Lot Map Layer";
                            document.getElementById("content_toolbar_button_layerReset").title = "Reset: Reset Map Type";
                            document.getElementById("content_toolbar_button_panUp").title = "Up: Pan Map Up";
                            document.getElementById("content_toolbar_button_panLeft").title = "Left: Pan Map Left";
                            document.getElementById("content_toolbar_button_panReset").title = "Default: Pan Map To Default";
                            document.getElementById("content_toolbar_button_panRight").title = "Right: Pan Map Right";
                            document.getElementById("content_toolbar_button_panDown").title = "Down: Pan Map Down";
                            document.getElementById("content_toolbar_button_zoomIn").title = "In: Zoom Map In";
                            document.getElementById("content_toolbar_button_zoomReset").title = "Reset: Reset Zoom Level";
                            document.getElementById("content_toolbar_button_zoomOut").title = "Out: Zoom Map Out";
                            document.getElementById("content_toolbar_button_manageItem").title = "Manage Location Details";
                            document.getElementById("content_toolbar_button_manageOverlay").title = "Manage Map Coverage";
                            document.getElementById("content_toolbar_button_managePOI").title = "Manage Points of Interest";
                            document.getElementById("content_toolbar_button_manageSearch").title = "Locate: Find A Location On The Map";
                            document.getElementById("content_toolbar_searchField").title = "Locate: Find A Location On The Map";
                            document.getElementById("content_toolbar_searchButton").title = "Locate: Find A Location On The Map";
                            document.getElementById("content_toolbarGrabber").title = "Toolbar: Toggle the Toolbar";
                            //toolbox
                            document.getElementById("content_toolbox_button_layerRoadmap").title = "Roadmap: Toggle Road Map Layer";
                            document.getElementById("content_toolbox_button_layerTerrain").title = "Terrain: Toggle Terrain Map Layer";
                            document.getElementById("content_toolbox_button_panUp").title = "Up: Pan Map Up";
                            document.getElementById("content_toolbox_button_layerSatellite").title = "Satellite: Toggle Satellite Map Layer";
                            document.getElementById("content_toolbox_button_layerHybrid").title = "Hybrid: Toggle Hybrid Map Layer";
                            document.getElementById("content_toolbox_button_panLeft").title = "Left: Pan Map Left";
                            document.getElementById("content_toolbox_button_panReset").title = "Default: Pan Map To Default";
                            document.getElementById("content_toolbox_button_panRight").title = "Right: Pan Map Right";
                            document.getElementById("content_toolbox_button_layerCustom").title = "Block/Lot: Toggle Block/Lot Map Layer";
                            document.getElementById("content_toolbox_button_layerReset").title = "Reset: Reset Map Type";
                            document.getElementById("content_toolbox_button_panDown").title = "Down: Pan Map Down";
                            document.getElementById("content_toolbox_button_reset").title = "Reset: Reset Map To Defaults";
                            document.getElementById("content_toolbox_button_toggleMapControls").title = "Controls: Toggle Map Controls";
                            document.getElementById("content_toolbox_button_zoomIn").title = "In: Zoom Map In";
                            document.getElementById("content_toolbox_button_zoomReset").title = "Reset: Reset Zoom Level";
                            document.getElementById("content_toolbox_button_zoomOut").title = "Out: Zoom Map Out";
                            //tab
                            document.getElementById("content_toolbox_button_manageItem").title = "Manage Location Details";
                            document.getElementById("content_toolbox_button_manageOverlay").title = "Manage Map Coverage";
                            document.getElementById("content_toolbox_button_managePOI").title = "Manage Points of Interest";
                            document.getElementById("content_toolbox_searchField").title = "Locate: Find A Location On The Map";
                            document.getElementById("content_toolbox_searchButton").title = "Locate: Find A Location On The Map";
                            document.getElementById("searchResults_container").title = "Locate: Find A Location On The Map";
                            //tab
                            document.getElementById("content_toolbox_button_itemPlace").title = "Edit Location";
                            document.getElementById("content_toolbox_button_itemGetUserLocation").title = "Center On Your Current Position";
                            document.getElementById("content_toolbox_button_useSearchAsLocation").title = "Use Search Result As Location";
                            document.getElementById("content_toolbox_button_convertToOverlay").title = "Convert This To a Map Overlay";
                            document.getElementById("content_toolbox_posItem").title = "Coordinates: This is the selected Latitude and Longitude of the point you selected.";
                            document.getElementById("content_toolbox_rgItem").title = "Address: This is the nearest address of the point you selected.";
                            document.getElementById("content_toolbox_button_saveItem").title = "Save Location Changes";
                            document.getElementById("content_toolbox_button_clearItem").title = "Reset Location Changes";
                            document.getElementById("content_toolbox_button_deleteItem").title = "Delete Geographic Location";
                            //tab
                            //document.getElementById("content_toolbox_button_overlayEdit").title = "Toggle Overlay Editing";
                            //document.getElementById("content_toolbox_button_overlayPlace").title = "Place A New Overlay";
                            document.getElementById("content_toolbox_button_overlayGetUserLocation").title = "Center On Your Current Position";
                            document.getElementById("content_toolbox_button_overlayToggle").title = "Toggle All Overlays On Map";
                            document.getElementById("rotation").title = "Rotate: Edit the rotation value";
                            document.getElementById("rotationKnob").title = "Rotate: Edit the rotation value";
                            document.getElementById("content_toolbox_rotationCounterClockwise").title = "Tenth Degree Left: Click to Rotate a Tenth Degree Counter-Clockwise";
                            document.getElementById("content_toolbox_rotationReset").title = "Reset: Click to Reset Rotation";
                            document.getElementById("content_toolbox_rotationClockwise").title = "Tenth Degree Right: Click to Rotate a Tenth Degree Clockwise";
                            document.getElementById("transparency").title = "Transparency: Set the transparency of this Overlay";
                            document.getElementById("content_toolbox_button_saveOverlay").title = "Save Overlay Changes";
                            document.getElementById("content_toolbox_button_clearOverlay").title = "Reset All Overlay Changes";
                            //tab
                            //document.getElementById("content_toolbox_button_placePOI").title = "Toggle Point Of Interest Editing";
                            document.getElementById("content_toolbox_button_poiGetUserLocation").title = "Center On Your Current Position";
                            document.getElementById("content_toolbox_button_poiToggle").title = "Toggle All POIs On Map";
                            document.getElementById("content_toolbox_button_poiMarker").title = "Marker: Place a Point";
                            document.getElementById("content_toolbox_button_poiCircle").title = "Circle: Place a Circle";
                            document.getElementById("content_toolbox_button_poiRectangle").title = "rectangle: Place a rectangle";
                            document.getElementById("content_toolbox_button_poiPolygon").title = "Polygon: Place a Polygon";
                            document.getElementById("content_toolbox_button_poiLine").title = "Line: Place a Line";
                            document.getElementById("content_toolbox_button_savePOI").title = "Save Point Of Interest Set";
                            document.getElementById("content_toolbox_button_clearPOI").title = "Clear Point Of Interest Set";
                            MAPSEARCHER.TRACER.addTracer("[INFO]: localizeByTooltips completed...");
                        } catch (err) {
                            alert(MAPSEARCHER.LOCALIZATION.DEFINES.L28 + ": " + err + " at line " +err.lineNumber );
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    localizeByText: function () {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: localizeByText started...");
                            //menubar
                            document.getElementById("content_menubar_header1").innerHTML = "File";
                            document.getElementById("content_menubar_header2").innerHTML = "Controls";
                            document.getElementById("content_menubar_header2Sub1").innerHTML = "View";
                            document.getElementById("content_menubar_header2Sub2").innerHTML = "Layers";
                            document.getElementById("content_menubar_header2Sub3").innerHTML = "Zoom";
                            document.getElementById("content_menubar_header2Sub4").innerHTML = "Pan";
                            document.getElementById("content_menubar_header3").innerHTML = "Actions";
                            document.getElementById("content_menubar_manageSearch").innerHTML = "Find Location";
                            document.getElementById("content_menubar_manageItem").innerHTML = "Manage Location";
                            document.getElementById("content_menubar_manageOverlay").innerHTML = "Manage Ovelays";
                            document.getElementById("content_menubar_header3Sub3Sub1").innerHTML = "Rotate";
                            document.getElementById("content_menubar_header3Sub3Sub2").innerHTML = "Tranparency";
                            document.getElementById("content_menubar_managePOI").innerHTML = "Manage POIs";
                            document.getElementById("content_menubar_header3Sub4Sub1").innerHTML = "Place";
                            document.getElementById("content_menubar_header4").innerHTML = "Help";
                            document.getElementById("content_menubar_save").innerHTML = "Complete Editing";
                            document.getElementById("content_menubar_cancel").innerHTML = "Cancel Editing";
                            document.getElementById("content_menubar_reset").innerHTML = "Reset All Changes";
                            document.getElementById("content_menubar_toggleMapControls").innerHTML = "Map Controls";
                            document.getElementById("content_menubar_toggleToolbox").innerHTML = "Toolbox";
                            document.getElementById("content_menubar_toggleToolbar").innerHTML = "Toolbar";
                            document.getElementById("content_menubar_layerRoadmap").innerHTML = "Roadmap";
                            document.getElementById("content_menubar_layerSatellite").innerHTML = "Satellite";
                            document.getElementById("content_menubar_layerHybrid").innerHTML = "Hybrid";
                            document.getElementById("content_menubar_layerTerrain").innerHTML = "Terrain";
                            document.getElementById("content_menubar_layerCustom").innerHTML = "Custom";
                            document.getElementById("content_menubar_layerReset").innerHTML = "Reset";
                            document.getElementById("content_menubar_zoomIn").innerHTML = "Zoom In";
                            document.getElementById("content_menubar_zoomOut").innerHTML = "Zoom Out";
                            document.getElementById("content_menubar_zoomReset").innerHTML = "Reset";
                            document.getElementById("content_menubar_panUp").innerHTML = "Pan Up";
                            document.getElementById("content_menubar_panRight").innerHTML = "Pan Right";
                            document.getElementById("content_menubar_panDown").innerHTML = "Pan Down";
                            document.getElementById("content_menubar_panLeft").innerHTML = "Pan Left";
                            document.getElementById("content_menubar_panReset").innerHTML = "Reset";
                            document.getElementById("content_menubar_searchField").setAttribute('placeholder', "Find a Location");
                            document.getElementById("content_menubar_itemGetUserLocation").innerHTML = "Center On Current Location";
                            document.getElementById("content_menubar_itemPlace").innerHTML = "Edit Location";
                            document.getElementById("content_menubar_useSearchAsLocation").innerHTML = "Use Search Result As Location";
                            document.getElementById("content_menubar_convertToOverlay").innerHTML = "Convert To Overlay";
                            document.getElementById("content_menubar_itemReset").innerHTML = "Reset Location";
                            document.getElementById("content_menubar_itemDelete").innerHTML = "Delete Geographic Location";
                            document.getElementById("content_menubar_overlayGetUserLocation").innerHTML = "Center On Current Location";
                            //document.getElementById("content_menubar_overlayEdit").innerHTML = "Toggle Overlay Editing";
                            //document.getElementById("content_menubar_overlayPlace").innerHTML = "Place A New Overlay";
                            document.getElementById("content_menubar_overlayToggle").innerHTML = "Toggle All Map Overlays";
                            document.getElementById("content_menubar_rotationClockwise").innerHTML = "Clockwise";
                            document.getElementById("content_menubar_rotationCounterClockwise").innerHTML = "Counter-Clockwise";
                            document.getElementById("content_menubar_rotationReset").innerHTML = "Reset";
                            document.getElementById("content_menubar_transparencyDarker").innerHTML = "Darker";
                            document.getElementById("content_menubar_transparencyLighter").innerHTML = "Lighter";
                            document.getElementById("content_menubar_transparencyReset").innerHTML = "Reset";
                            document.getElementById("content_menubar_overlayReset").innerHTML = "Reset Overlays";
                            document.getElementById("content_menubar_poiGetUserLocation").innerHTML = "Center On Current Location";
                            //document.getElementById("content_menubar_poiPlace").innerHTML = "Toggle POI Editing";
                            document.getElementById("content_menubar_poiToggle").innerHTML = "Toggle POIs On Map";
                            document.getElementById("content_menubar_poiReset").innerHTML = "Reset POIs";
                            document.getElementById("content_menubar_poiMarker").innerHTML = "Marker";
                            document.getElementById("content_menubar_poiCircle").innerHTML = "Circle";
                            document.getElementById("content_menubar_poiRectangle").innerHTML = "rectangle";
                            document.getElementById("content_menubar_poiPolygon").innerHTML = "Polygon";
                            document.getElementById("content_menubar_poiLine").innerHTML = "Line";
                            document.getElementById("content_menubar_documentation").innerHTML = "Documentation";
                            document.getElementById("content_menubar_reportAProblem").innerHTML = "Report A Problem";
                            //all others
                            document.getElementById("content_minibar_header").innerHTML = "Toolbox";
                            document.getElementById("content_toolbox_button_saveItem").value = "Save";
                            document.getElementById("content_toolbox_button_clearItem").value = "Reset";
                            document.getElementById("content_toolbox_button_deleteItem").value = "Delete";
                            document.getElementById("content_toolbox_button_saveOverlay").value = "Save";
                            document.getElementById("content_toolbox_button_clearOverlay").value = "Reset";
                            document.getElementById("content_toolbox_button_savePOI").value = "Save";
                            document.getElementById("content_toolbox_button_clearPOI").value = "Reset";
                            document.getElementById("content_toolbox_tab1_header").innerHTML = "Map Controls";
                            document.getElementById("content_toolbox_tab2_header").innerHTML = "Actions";
                            document.getElementById("content_toolbox_tab3_header").innerHTML = "Manage Location";
                            document.getElementById("content_toolbox_tab4_header").innerHTML = "Manage Overlay";
                            document.getElementById("content_toolbox_tab5_header").innerHTML = "Manage POI";
                            document.getElementById("content_toolbar_searchField").setAttribute('placeholder', "Find a Location");
                            document.getElementById("content_toolbox_searchField").setAttribute('placeholder', "Find a Location");
                            document.getElementById("content_toolbox_posItem").setAttribute('placeholder', "Selected Lat/Long");
                            document.getElementById("content_toolbox_rgItem").setAttribute('placeholder', "Nearest Address");
                            MAPSEARCHER.TRACER.addTracer("[INFO]: localizeByText completed...");
                        } catch (err) {
                            alert(MAPSEARCHER.LOCALIZATION.DEFINES.L29);
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    }
                };
            }(),    //localization support
            ACTIONS: function () {
                return {
                    resetAll: function () {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: resetAll started...");
                            document.location.reload(true);
                            MAPSEARCHER.TRACER.addTracer("[INFO]: resetAll completed...");
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    toggleVis: function (id) {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: toggleVis started...");
                            switch (id) {
                                case "mapControls":
                                    if (MAPSEARCHER.GLOBAL.DEFINES.mapControlsDisplayed == true) { //present, hide
                                        map.setOptions({
                                            zoomControl: false,
                                            panControl: false,
                                            mapTypeControl: false
                                        });
                                        MAPSEARCHER.GLOBAL.DEFINES.mapControlsDisplayed = false;
                                    } else { //not present, make present
                                        map.setOptions({
                                            zoomControl: true,
                                            zoomControlOptions: { style: google.maps.ZoomControlStyle.SMALL, position: google.maps.ControlPosition.LEFT_TOP },
                                            panControl: true,
                                            panControlOptions: { position: google.maps.ControlPosition.LEFT_TOP },
                                            mapTypeControl: true,
                                            mapTypeControlOptions: { style: google.maps.MapTypeControlStyle.DROPDOWN_MENU, position: google.maps.ControlPosition.RIGHT_TOP }
                                        });
                                        MAPSEARCHER.GLOBAL.DEFINES.mapControlsDisplayed = true;
                                    }
                                    MAPSEARCHER.UTILITIES.buttonActive("mapControls"); //set the is active glow for button
                                    break;

                                case "toolbox":
                                    if (MAPSEARCHER.GLOBAL.DEFINES.toolboxDisplayed == true) {
                                        document.getElementById("mapedit_container_toolbox").style.display = "none";
                                        document.getElementById("mapedit_container_toolboxTabs").style.display = "none";
                                        MAPSEARCHER.GLOBAL.DEFINES.toolboxDisplayed = false;
                                    } else {
                                        $("#mapedit_container_toolbox").animate({ 'top': '75px', 'left': '100px' });
                                        document.getElementById("mapedit_container_toolbox").style.display = "block";
                                        document.getElementById("mapedit_container_toolboxTabs").style.display = "block";
                                        document.getElementById("mapedit_container_toolbox").style.height = "auto";
                                        MAPSEARCHER.GLOBAL.DEFINES.toolboxDisplayed = true;
                                    }
                                    MAPSEARCHER.UTILITIES.buttonActive("toolbox"); //set the is active glow for button
                                    break;

                                case "toolbar":
                                    if (MAPSEARCHER.GLOBAL.DEFINES.toolbarDisplayed == true) {
                                        $("#mapedit_container_pane_1").hide();
                                        //$("#mapedit_container_pane_1").animate({ 'top': '-46px' });
                                        document.getElementById("mapedit_container_toolbarGrabber").style.marginTop = "0";
                                        MAPSEARCHER.GLOBAL.DEFINES.toolbarDisplayed = false;
                                    } else {
                                        $("#mapedit_container_pane_1").show();
                                        //$("#mapedit_container_pane_1").animate({ 'top': '46px' });
                                        document.getElementById("mapedit_container_toolbarGrabber").style.marginTop = "48px";
                                        MAPSEARCHER.GLOBAL.DEFINES.toolbarDisplayed = true;
                                    }
                                    MAPSEARCHER.UTILITIES.buttonActive("toolbar"); //set the is active glow for button
                                    break;

                                case "kml":
                                    if (MAPSEARCHER.GLOBAL.DEFINES.kmlDisplayed == true) {
                                        MAPSEARCHER.GLOBAL.DEFINES.kmlLayer.setMap(null);
                                        MAPSEARCHER.GLOBAL.DEFINES.kmlDisplayed = false;
                                    } else {
                                        MAPSEARCHER.GLOBAL.DEFINES.kmlLayer.setMap(map);
                                        MAPSEARCHER.GLOBAL.DEFINES.kmlDisplayed = true;
                                    }
                                    MAPSEARCHER.UTILITIES.buttonActive("kml"); //set the is active glow for button
                                    break;

                                case "toolboxMin":
                                    $("#mapedit_container_toolboxTabs").hide();
                                    document.getElementById("mapedit_container_toolbox").style.height = "15px";
                                    break;

                                case "toolboxMax":
                                    $("#mapedit_container_toolboxTabs").show();
                                    document.getElementById("mapedit_container_toolbox").style.height = "auto";
                                    break;

                                case "mapDrawingManager":
                                    if (MAPSEARCHER.GLOBAL.DEFINES.mapDrawingManagerDisplayed == true) {
                                        MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setDrawingMode(null);
                                        MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setMap(null);
                                        MAPSEARCHER.GLOBAL.DEFINES.mapDrawingManagerDisplayed = false;
                                    } else {
                                        MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setDrawingMode(null);
                                        MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setMap(map);
                                        MAPSEARCHER.GLOBAL.DEFINES.mapDrawingManagerDisplayed = true;
                                    }
                                    break;

                                case "overlays":
                                    if (MAPSEARCHER.GLOBAL.DEFINES.overlaysOnMap.length) {
                                        if (MAPSEARCHER.GLOBAL.DEFINES.overlaysCurrentlyDisplayed == true) {
                                            MAPSEARCHER.UTILITIES.displayMessage(MAPSEARCHER.LOCALIZATION.DEFINES.L22);
                                            for (var i = 1; i < MAPSEARCHER.GLOBAL.DEFINES.overlaysOnMap.length; i++) { //go through and display overlays as long as there is an overlay to display
                                                MAPSEARCHER.TRACER.addTracer("[INFO]: overlay count " + MAPSEARCHER.GLOBAL.DEFINES.overlayCount);
                                                MAPSEARCHER.GLOBAL.DEFINES.RIBMode = true;
                                                if (document.getElementById("overlayToggle" + i)) {
                                                    MAPSEARCHER.TRACER.addTracer("[INFO]: found: overlayToggle" + i);
                                                    for (var j = 0; j < MAPSEARCHER.GLOBAL.DEFINES.incomingPolygonPolygonType.length; j++) {
                                                        try {
                                                            MAPSEARCHER.ACTIONS.overlayHideMe(i + j);
                                                        } catch (e) {
                                                            MAPSEARCHER.TRACER.addTracer("[INFO]: overlayOnMap[" + (i + j) + "] not found");
                                                        }
                                                    }
                                                } else {
                                                    MAPSEARCHER.TRACER.addTracer("[INFO]: did not find: overlayToggle" + i);
                                                }

                                                MAPSEARCHER.GLOBAL.DEFINES.RIBMode = false;
                                                MAPSEARCHER.GLOBAL.DEFINES.overlaysOnMap[i].setMap(null); //hide the overlay from the map
                                                MAPSEARCHER.GLOBAL.DEFINES.ghostOverlayRectangle[i].setMap(null); //hide ghost from map
                                                MAPSEARCHER.GLOBAL.DEFINES.overlaysCurrentlyDisplayed = false; //mark that overlays are not on the map
                                                MAPSEARCHER.GLOBAL.DEFINES.buttonActive_overlayToggle = true;
                                            }
                                        } else {
                                            MAPSEARCHER.UTILITIES.displayMessage(MAPSEARCHER.LOCALIZATION.DEFINES.L23);
                                            for (var i = 1; i < MAPSEARCHER.GLOBAL.DEFINES.overlaysOnMap.length; i++) { //go through and display overlays as long as there is an overlay to display
                                                MAPSEARCHER.TRACER.addTracer("[INFO]: oom " + MAPSEARCHER.GLOBAL.DEFINES.overlaysOnMap.length);
                                                MAPSEARCHER.GLOBAL.DEFINES.RIBMode = true;
                                                if (document.getElementById("overlayToggle" + i)) {
                                                    MAPSEARCHER.TRACER.addTracer("[INFO]: found: overlayToggle" + i);
                                                    MAPSEARCHER.ACTIONS.overlayShowMe(i);
                                                } else {
                                                    MAPSEARCHER.TRACER.addTracer("[INFO]: did not find: overlayToggle" + i);
                                                }
                                                MAPSEARCHER.GLOBAL.DEFINES.RIBMode = false;
                                                MAPSEARCHER.GLOBAL.DEFINES.overlaysOnMap[i].setMap(map); //set the overlay to the map
                                                MAPSEARCHER.GLOBAL.DEFINES.ghostOverlayRectangle[i].setMap(map); //set to map
                                                MAPSEARCHER.GLOBAL.DEFINES.overlaysCurrentlyDisplayed = true; //mark that overlays are on the map
                                                MAPSEARCHER.GLOBAL.DEFINES.buttonActive_overlayToggle = false;
                                            }
                                        }
                                        MAPSEARCHER.UTILITIES.buttonActive("overlayToggle");
                                    } else {
                                        //nothing to toggle
                                        MAPSEARCHER.UTILITIES.displayMessage(MAPSEARCHER.LOCALIZATION.DEFINES.L45);
                                    }
                                    break;

                                case "pois":
                                    if (MAPSEARCHER.GLOBAL.DEFINES.poiCount) {
                                        MAPSEARCHER.TRACER.addTracer("[INFO]: poi count: " + MAPSEARCHER.GLOBAL.DEFINES.poiCount);
                                        MAPSEARCHER.UTILITIES.buttonActive("poiToggle");
                                        if (MAPSEARCHER.GLOBAL.DEFINES.poiToggleState == "displayed") {
                                            for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiCount; i++) {
                                                MAPSEARCHER.ACTIONS.poiHideMe(i);
                                                MAPSEARCHER.GLOBAL.DEFINES.poiToggleState = "hidden";
                                                MAPSEARCHER.UTILITIES.displayMessage(MAPSEARCHER.LOCALIZATION.DEFINES.L42);
                                            }
                                        } else {
                                            for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.poiCount; i++) {
                                                MAPSEARCHER.ACTIONS.poiShowMe(i);
                                                MAPSEARCHER.GLOBAL.DEFINES.poiToggleState = "displayed";
                                                MAPSEARCHER.UTILITIES.displayMessage(MAPSEARCHER.LOCALIZATION.DEFINES.L43);
                                            }
                                        }


                                    } else {
                                        //nothing to toggle
                                        MAPSEARCHER.UTILITIES.displayMessage(MAPSEARCHER.LOCALIZATION.DEFINES.L45);
                                    }
                                    break;

                                default:
                                    //toggle that item if not found above
                                    break;
                            }
                            MAPSEARCHER.TRACER.addTracer("[INFO]: toggleVis completed...");
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    changeMapLayer: function (layer) {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: changeMapLayer started...");
                            switch (layer) {
                                case "roadmap":
                                    map.setMapTypeId(google.maps.MapTypeId.ROADMAP);
                                    MAPSEARCHER.GLOBAL.DEFINES.mapLayerActive = "Roadmap";
                                    break;
                                case "terrain":
                                    map.setMapTypeId(google.maps.MapTypeId.TERRAIN);
                                    MAPSEARCHER.GLOBAL.DEFINES.mapLayerActive = "Terrain";
                                    break;
                                case "satellite":
                                    map.setMapTypeId(google.maps.MapTypeId.SATELLITE);
                                    MAPSEARCHER.GLOBAL.DEFINES.mapLayerActive = "Satellite";
                                    break;
                                case "hybrid":
                                    map.setMapTypeId(google.maps.MapTypeId.HYBRID);
                                    MAPSEARCHER.GLOBAL.DEFINES.mapLayerActive = "Hybrid";
                                    break;
                                case "custom":
                                    MAPSEARCHER.ACTIONS.toggleVis("kml");
                                    break;
                                case "reset":
                                    map.setMapTypeId(google.maps.MapTypeId.ROADMAP);
                                    //2do make this set to default
                                    MAPSEARCHER.GLOBAL.DEFINES.mapLayerActive = "Roadmap";
                                    if (MAPSEARCHER.GLOBAL.DEFINES.kmlDisplayed == true) {
                                        MAPSEARCHER.ACTIONS.toggleVis("kml");
                                    }
                                    break;
                            }
                            MAPSEARCHER.UTILITIES.buttonActive("layer"); //set the is active glow for button
                            MAPSEARCHER.TRACER.addTracer("[INFO]: changeMapLayer completed...");
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    panMap: function (direction) {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: panmap started...");
                            switch (direction) {
                                case "up":
                                    map.panBy(0, -100);
                                    MAPSEARCHER.UTILITIES.testBounds();
                                    break;
                                case "down":
                                    map.panBy(0, 100);
                                    MAPSEARCHER.UTILITIES.testBounds();
                                    break;
                                case "left":
                                    map.panBy(-100, 0);
                                    MAPSEARCHER.UTILITIES.testBounds();
                                    break;
                                case "right":
                                    map.panBy(100, 0);
                                    MAPSEARCHER.UTILITIES.testBounds();
                                    break;
                                case "reset":
                                    map.panTo(MAPSEARCHER.GLOBAL.DEFINES.mapCenter);
                                    break;
                            }
                            MAPSEARCHER.TRACER.addTracer("[INFO]: panmap completed...");
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    zoomMap: function (direction) {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: zoommap started...");
                            switch (direction) {
                                case "in":
                                    map.setZoom(map.getZoom() + 1);
                                    break;
                                case "out":
                                    map.setZoom(map.getZoom() - 1);
                                    break;
                                case "reset":
                                    map.setZoom(MAPSEARCHER.GLOBAL.DEFINES.defaultZoomLevel);
                                    break;
                            }
                            MAPSEARCHER.TRACER.addTracer("[INFO]: zoommap completed...");
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    geolocate: function (id) {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: geolocate started...");
                            MAPSEARCHER.UTILITIES.displayMessage(MAPSEARCHER.LOCALIZATION.DEFINES.L50);
                            switch (id) {
                                case "item":
                                    MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setDrawingMode(google.maps.drawing.OverlayType.MARKER);
                                    MAPSEARCHER.GLOBAL.DEFINES.placerType = "item";
                                    // Try W3C Geolocation
                                    if (navigator.geolocation) {
                                        navigator.geolocation.getCurrentPosition(function (position) {
                                            var userLocation = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);
                                            map.setCenter(userLocation);
                                            MAPSEARCHER.UTILITIES.testBounds();
                                            if (MAPSEARCHER.GLOBAL.DEFINES.mapInBounds == "yes") {
                                                MAPSEARCHER.GLOBAL.DEFINES.markerCenter = userLocation;
                                                MAPSEARCHER.GLOBAL.DEFINES.itemMarker = new google.maps.Marker({
                                                    position: MAPSEARCHER.GLOBAL.DEFINES.markerCenter,
                                                    map: map
                                                });
                                                MAPSEARCHER.GLOBAL.DEFINES.itemMarker.setMap(map);
                                                MAPSEARCHER.UTILITIES.codeLatLng(MAPSEARCHER.GLOBAL.DEFINES.itemMarker.getPosition());
                                                MAPSEARCHER.GLOBAL.DEFINES.savingMarkerCenter = MAPSEARCHER.GLOBAL.DEFINES.itemMarker.getPosition(); //store coords to save
                                                MAPSEARCHER.TRACER.addTracer("[INFO]: rg userlocation: " + userLocation);
                                                var userLocationS = userLocation.toString();
                                                userLocationS = userLocationS.replace("\)", "");
                                                userLocationS = userLocationS.replace("\)", "");
                                                MAPSEARCHER.TRACER.addTracer("[INFO]: rg test2: " + userLocationS);
                                                document.getElementById('content_toolbox_posItem').value = userLocationS;
                                                MAPSEARCHER.UTILITIES.codeLatLng(userLocation);
                                            } else {
                                                MAPSEARCHER.TRACER.addTracer("[INFO]: user location out of bounds");
                                            }

                                        });

                                    } else {
                                        alert(MAPSEARCHER.LOCALIZATION.DEFINES.L4);
                                        MAPSEARCHER.TRACER.addTracer(MAPSEARCHER.LOCALIZATION.DEFINES.L4);
                                    }
                                    MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setDrawingMode(null);
                                    break;
                                case "overlay":
                                    MAPSEARCHER.GLOBAL.DEFINES.placerType = "overlay";
                                    // Try W3C Geolocation
                                    if (navigator.geolocation) {
                                        navigator.geolocation.getCurrentPosition(function (position) {
                                            var userLocation = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);
                                            map.setCenter(userLocation);
                                            MAPSEARCHER.UTILITIES.testBounds();
                                        });

                                    } else {
                                        alert(MAPSEARCHER.LOCALIZATION.DEFINES.L4);
                                        MAPSEARCHER.TRACER.addTracer(MAPSEARCHER.LOCALIZATION.DEFINES.L4);
                                    }
                                    break;
                                case "poi":
                                    MAPSEARCHER.GLOBAL.DEFINES.placerType = "poi";
                                    // Try W3C Geolocation
                                    if (navigator.geolocation) {
                                        navigator.geolocation.getCurrentPosition(function (position) {
                                            var userLocation = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);
                                            map.setCenter(userLocation);
                                            MAPSEARCHER.UTILITIES.testBounds();
                                        });

                                    } else {
                                        alert(MAPSEARCHER.LOCALIZATION.DEFINES.L4);
                                        MAPSEARCHER.TRACER.addTracer(MAPSEARCHER.LOCALIZATION.DEFINES.L4);
                                    }
                                    break;
                            }
                            MAPSEARCHER.TRACER.addTracer("[INFO]: geolocate completed...");
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    searchResultDeleteMe: function (id) {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: searchResultDeleteMe started...");
                            //remove visually
                            MAPSEARCHER.GLOBAL.DEFINES.searchResult.setMap(null); //remove from map
                            $("#searchResultListItem" + id).remove(); //remove the first result div from result list box in toolbox
                            document.getElementById("content_toolbar_searchField").value = ""; //clear searchbar
                            document.getElementById("content_toolbox_searchField").value = ""; //clear searchbox
                            //remove references to 
                            MAPSEARCHER.GLOBAL.DEFINES.searchResult = null; //reset search result map item
                            MAPSEARCHER.GLOBAL.DEFINES.searchCount = 0; //reset search count
                            MAPSEARCHER.TRACER.addTracer("[INFO]: searchResultDeleteMe completed...");
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    searchResultShowMe: function (id) {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: searchResultShowMe started...");
                            MAPSEARCHER.GLOBAL.DEFINES.searchResult.setMap(map); //remove from map
                            document.getElementById("searchResultToggle" + id).innerHTML = "<img src=\"" + MAPSEARCHER.GLOBAL.DEFINES.baseURL + MAPSEARCHER.GLOBAL.DEFINES.baseImageDirURL + "sub.png\" onclick=\"MAPSEARCHER.ACTIONS.searchResultHideMe(" + id + ");\" />";
                            MAPSEARCHER.TRACER.addTracer("[INFO]: searchResultShowMe completed...");
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    searchResultHideMe: function (id) {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: searchResultHideMe started...");
                            MAPSEARCHER.GLOBAL.DEFINES.searchResult.setMap(null); //remove from map
                            document.getElementById("searchResultToggle" + id).innerHTML = "<img src=\"" + MAPSEARCHER.GLOBAL.DEFINES.baseURL + MAPSEARCHER.GLOBAL.DEFINES.baseImageDirURL + "add.png\" onclick=\"MAPSEARCHER.ACTIONS.searchResultShowMe(" + id + ");\" />";
                            MAPSEARCHER.TRACER.addTracer("[INFO]: searchResultHideMe completed...");
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    useSearchAsItemLocation: function () {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: useSearchAsItemLocation started...");
                            //debuggin
                            MAPSEARCHER.TRACER.addTracer("[INFO]: search result: " + MAPSEARCHER.GLOBAL.DEFINES.searchResult);
                            //check to see if there is a search result
                            if (MAPSEARCHER.GLOBAL.DEFINES.searchResult != null) {
                                //this tells listeners what to do
                                MAPSEARCHER.GLOBAL.DEFINES.placerType = "item";
                                //determine if itemmarker is already made
                                if (MAPSEARCHER.GLOBAL.DEFINES.itemMarker != null) {
                                    //assign new position of marker
                                    MAPSEARCHER.GLOBAL.DEFINES.itemMarker.setPosition(MAPSEARCHER.GLOBAL.DEFINES.searchResult.getPosition());
                                    //delete search result
                                    MAPSEARCHER.ACTIONS.searchResultDeleteMe();
                                    //display new marker
                                    MAPSEARCHER.GLOBAL.DEFINES.itemMarker.setMap(map);
                                } else {
                                    //make search marker, item marker
                                    MAPSEARCHER.GLOBAL.DEFINES.itemMarker = MAPSEARCHER.GLOBAL.DEFINES.searchResult;
                                    //assign itemMarkerMode
                                    MAPSEARCHER.GLOBAL.DEFINES.itemMarker.setOptions(MAPSEARCHER.GLOBAL.DEFINES.markerOptionsItem);
                                    //assign flags
                                    MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                    MAPSEARCHER.GLOBAL.DEFINES.firstSaveItem = true;
                                    //used to prevent multi markers
                                    if (MAPSEARCHER.GLOBAL.DEFINES.firstMarker > 0) {
                                        MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setDrawingMode(null); //only place one at a time
                                    } else {
                                        MAPSEARCHER.GLOBAL.DEFINES.firstMarker++;
                                        MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setDrawingMode(null); //only place one at a time
                                    }
                                }
                                //prevent redraw
                                MAPSEARCHER.GLOBAL.DEFINES.firstMarker++;
                                //get the lat/long of item marker and put it in the item location tab
                                document.getElementById('content_toolbox_posItem').value = MAPSEARCHER.GLOBAL.DEFINES.itemMarker.getPosition();
                                //get the reverse geo address for item location and put in location tab
                                MAPSEARCHER.UTILITIES.codeLatLng(MAPSEARCHER.GLOBAL.DEFINES.itemMarker.getPosition());
                                //store coords to save
                                MAPSEARCHER.GLOBAL.DEFINES.savingMarkerCenter = MAPSEARCHER.GLOBAL.DEFINES.itemMarker.getPosition();
                                //add listener for new item marker (can only add once the MAPSEARCHER.GLOBAL.DEFINES.itemMarker is created)
                                google.maps.event.addListener(MAPSEARCHER.GLOBAL.DEFINES.itemMarker, 'dragend', function () {
                                    //get lat/long
                                    document.getElementById('content_toolbox_posItem').value = MAPSEARCHER.GLOBAL.DEFINES.itemMarker.getPosition();
                                    //get address
                                    MAPSEARCHER.UTILITIES.codeLatLng(MAPSEARCHER.GLOBAL.DEFINES.itemMarker.getPosition());
                                    //store coords to save
                                    MAPSEARCHER.GLOBAL.DEFINES.savingMarkerCenter = MAPSEARCHER.GLOBAL.DEFINES.itemMarker.getPosition();
                                    //assign flags
                                    MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = true;
                                    MAPSEARCHER.GLOBAL.DEFINES.firstSaveItem = true;
                                });
                            } else {
                                //nothing in search
                                MAPSEARCHER.UTILITIES.displayMessage(MAPSEARCHER.LOCALIZATION.DEFINES.L39);
                            }
                            MAPSEARCHER.TRACER.addTracer("[INFO]: useSearchAsItemLocation completed...");
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    deleteItemLocation: function () {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: deleteItemLocation started...");
                            if (MAPSEARCHER.GLOBAL.DEFINES.itemMarker) {
                                //confirm
                                MAPSEARCHER.UTILITIES.confirmMessage(MAPSEARCHER.LOCALIZATION.DEFINES.L70);
                                //hide marker
                                MAPSEARCHER.GLOBAL.DEFINES.itemMarker.setMap(null);
                                MAPSEARCHER.GLOBAL.DEFINES.itemMarker = null;
                                //msg
                                MAPSEARCHER.GLOBAL.DEFINES.toServerSuccessMessage = MAPSEARCHER.LOCALIZATION.DEFINES.L69;
                                //send to server and delete from mets
                                MAPSEARCHER.GLOBAL.DEFINES.RIBMode = true;
                                MAPSEARCHER.UTILITIES.createSavedItem("delete", null);
                                MAPSEARCHER.GLOBAL.DEFINES.RIBMode = false;
                                //clear saving item center as well
                                MAPSEARCHER.GLOBAL.DEFINES.savingMarkerCenter = null;
                                //explicitly disallow editing after converting
                                MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setDrawingMode(null);
                                //MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setMap(null);
                                MAPSEARCHER.GLOBAL.DEFINES.userMayLoseData = false;
                                //clear item boxes
                                document.getElementById("content_toolbox_posItem").value = "";
                                document.getElementById("content_toolbox_rgItem").value = "";
                            } else {
                                //did not delete
                                MAPSEARCHER.UTILITIES.displayMessage(MAPSEARCHER.LOCALIZATION.DEFINES.L_NotDeleted);
                                //explicitly disallow editing after a failed convert
                                MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setDrawingMode(null);
                                //MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setMap(null);
                            }
                            MAPSEARCHER.TRACER.addTracer("[INFO]: deleteItemLocation completed...");
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    openToolboxTab: function (id) {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: openToolboxTab started...");
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
                                //explicitly reopen the dm
                                MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setMap(map);
                                MAPSEARCHER.GLOBAL.DEFINES.drawingManager.setOptions({ drawingControl: true, drawingControlOptions: { position: google.maps.ControlPosition.RIGHT_TOP, drawingModes: [google.maps.drawing.OverlayType.MARKER, google.maps.drawing.OverlayType.CIRCLE, google.maps.drawing.OverlayType.RECTANGLE, google.maps.drawing.OverlayType.POLYGON, google.maps.drawing.OverlayType.POLYLINE], markerOptions: MAPSEARCHER.GLOBAL.DEFINES.markerOptionsPOI, circleOptions: MAPSEARCHER.GLOBAL.DEFINES.circleOptionsPOI, rectangleOptions: MAPSEARCHER.GLOBAL.DEFINES.rectangleOptionsPOI, polygonOptions: MAPSEARCHER.GLOBAL.DEFINES.polygonOptionsPOI, polylineOptions: MAPSEARCHER.GLOBAL.DEFINES.polylineOptionsPOI } });
                            }
                            $("#mapedit_container_toolboxTabs").accordion({ active: id });
                            MAPSEARCHER.TRACER.addTracer("[INFO]: openToolboxTab completed...");
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    keyPress: function (e) {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: keypress started...");
                            //determine if we are typing in a text area
                            MAPSEARCHER.UTILITIES.checkKeyCode();
                            //get keycode
                            var keycode = null;
                            if (window.event) {
                                keycode = window.event.keyCode;
                            } else if (e) {
                                keycode = e.which;
                            }
                            MAPSEARCHER.TRACER.addTracer("[INFO]: key press: " + keycode);
                            //handle specific keycodes
                            switch (keycode) {
                                case 13: //enter
                                    if ("content_menubar_searchField" == document.activeElement.id) {
                                        MAPSEARCHER.UTILITIES.finder(document.getElementById("content_menubar_searchField").value);
                                    }
                                    if ("content_toolbox_searchField" == document.activeElement.id) {
                                        MAPSEARCHER.UTILITIES.finder(document.getElementById("content_toolbox_searchField").value);
                                    }
                                    if ("content_toolbar_searchField" == document.activeElement.id) {
                                        MAPSEARCHER.UTILITIES.finder(document.getElementById("content_toolbar_searchField").value);
                                    }
                                    break;
                                case 67: //shift + C
                                    if (MAPSEARCHER.GLOBAL.DEFINES.typingInTextArea == false) {
                                        if (navigator.appName == "Microsoft Internet Explorer") {
                                            var copyString = document.getElementById("cLat").innerHTML;
                                            copyString += ", " + document.getElementById("cLong").innerHTML;
                                            window.clipboardData.setData("Text", copyString);
                                            MAPSEARCHER.UTILITIES.displayMessage(MAPSEARCHER.LOCALIZATION.DEFINES.L19);
                                        } else {
                                            if (MAPSEARCHER.GLOBAL.DEFINES.cCoordsFrozen == "no") {
                                                //freeze
                                                MAPSEARCHER.GLOBAL.DEFINES.cCoordsFrozen = "yes";
                                                //MAPSEARCHER.UTILITIES.stickyMessage("Coordinate viewer is frozen (to unfreeze hold the shift key + the \"F\" key)");
                                                //MAPSEARCHER.UTILITIES.displayMessage(MAPSEARCHER.LOCALIZATION.DEFINES.L20);
                                                MAPSEARCHER.UTILITIES.stickyMessage(MAPSEARCHER.LOCALIZATION.DEFINES.L20);
                                            } else {
                                                //unfreeze
                                                MAPSEARCHER.GLOBAL.DEFINES.cCoordsFrozen = "no";
                                                //MAPSEARCHER.UTILITIES.stickyMessage("Coordinate viewer is frozen (to unfreeze hold the shift key + the \"F\" key)");
                                                MAPSEARCHER.UTILITIES.stickyMessage(MAPSEARCHER.LOCALIZATION.DEFINES.L20);
                                                MAPSEARCHER.UTILITIES.displayMessage(MAPSEARCHER.LOCALIZATION.DEFINES.L21);
                                            }
                                        }
                                    }
                                    break;
                                    //toggle Overlays
                                case 79: //shift + O
                                    if (MAPSEARCHER.GLOBAL.DEFINES.typingInTextArea == false) {
                                        MAPSEARCHER.ACTIONS.toggleVis("overlays");
                                    }
                                    break;
                                    //toggle POIs
                                case 80: //shift + P
                                    if (MAPSEARCHER.GLOBAL.DEFINES.typingInTextArea == false) {
                                        MAPSEARCHER.ACTIONS.toggleVis("pois");
                                    }
                                    break;
                                    //toggle toolbar
                                case 82: //shift + R
                                    if (MAPSEARCHER.GLOBAL.DEFINES.typingInTextArea == false) {
                                        MAPSEARCHER.ACTIONS.toggleVis("toolbar");
                                    }
                                    break;
                                    //toggle toolbox
                                case 88: //shift + X
                                    if (MAPSEARCHER.GLOBAL.DEFINES.typingInTextArea == false) {
                                        MAPSEARCHER.ACTIONS.toggleVis("toolbox");
                                    }
                                    break;
                                    //move overlay left
                                case 97: //a
                                    if (MAPSEARCHER.GLOBAL.DEFINES.typingInTextArea == false) {
                                        if (MAPSEARCHER.GLOBAL.DEFINES.workingOverlayIndex != null) {
                                            try {
                                                MAPSEARCHER.ACTIONS.panOverlay("left");
                                            } catch (e) {
                                                MAPSEARCHER.TRACER.addTracer("[INFO]: could not pan overlay");
                                            }

                                        }
                                    }
                                    break;
                                    //move overlay up
                                case 119: //w
                                    if (MAPSEARCHER.GLOBAL.DEFINES.typingInTextArea == false) {
                                        if (MAPSEARCHER.GLOBAL.DEFINES.workingOverlayIndex != null) {
                                            try {
                                                MAPSEARCHER.ACTIONS.panOverlay("up");
                                            } catch (e) {
                                                MAPSEARCHER.TRACER.addTracer("[INFO]: could not pan overlay");
                                            }

                                        }
                                    }
                                    break;
                                    //move overlay right
                                case 100: //s
                                    if (MAPSEARCHER.GLOBAL.DEFINES.typingInTextArea == false) {
                                        if (MAPSEARCHER.GLOBAL.DEFINES.workingOverlayIndex != null) {
                                            try {
                                                MAPSEARCHER.ACTIONS.panOverlay("right");
                                            } catch (e) {
                                                MAPSEARCHER.TRACER.addTracer("[INFO]: could not pan overlay");
                                            }

                                        }
                                    }
                                    break;
                                    //move overlay down
                                case 115: //g
                                    if (MAPSEARCHER.GLOBAL.DEFINES.typingInTextArea == false) {
                                        if (MAPSEARCHER.GLOBAL.DEFINES.workingOverlayIndex != null) {
                                            try {
                                                MAPSEARCHER.ACTIONS.panOverlay("down");
                                            } catch (e) {
                                                MAPSEARCHER.TRACER.addTracer("[INFO]: could not pan overlay");
                                            }

                                        }
                                    }
                                    break;
                                    //toggle DebugPanel
                                case 68: //shift + D (for debuggin)
                                    if (MAPSEARCHER.GLOBAL.DEFINES.typingInTextArea == false) {
                                        if (MAPSEARCHER.GLOBAL.DEFINES.debuggerOn == true) {
                                            MAPSEARCHER.TRACER.toggleTracer();
                                        } else {
                                            try {
                                                if (document.URL.indexOf("debugger") > -1) {
                                                    MAPSEARCHER.GLOBAL.DEFINES.debuggerOn = true;
                                                    MAPSEARCHER.TRACER.toggleTracer();
                                                }
                                            } catch(e) {
                                                //do nothing
                                            }
                                        }
                                    }
                                    break;
                            }
                            MAPSEARCHER.TRACER.addTracer("[INFO]: keypress completed...");
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    }
                };
            }(),         //called by user direct interaction
            UTILITIES: function () {
                return {
                    toServer: function (dataPackage) {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: toServer started...");
                            jQuery('form').each(function () {
                                var payload = JSON.stringify({ sendData: dataPackage });
                                var hiddenfield = document.getElementById('payload');
                                hiddenfield.value = payload;
                                var hiddenfield2 = document.getElementById('action');
                                hiddenfield2.value = 'save';
                                //reset success marker
                                MAPSEARCHER.GLOBAL.DEFINES.toServerSuccess = false;
                                MAPSEARCHER.UTILITIES.displayMessage(MAPSEARCHER.LOCALIZATION.DEFINES.L_Working);
                                $.ajax({
                                    type: "POST",
                                    async: true,
                                    url: window.location.href.toString(),
                                    data: jQuery(this).serialize(),
                                    success: function (result) {
                                        //MAPSEARCHER.TRACER.addTracer("[INFO]: server result:" + result);
                                        MAPSEARCHER.TRACER.addTracer("[INFO]: Sallback from server - success");
                                        //MAPSEARCHER.UTILITIES.displayMessage(MAPSEARCHER.LOCALIZATION.DEFINES.L_Completed);
                                        //MAPSEARCHER.UTILITIES.displayMessage(MAPSEARCHER.LOCALIZATION.DEFINES.L_Saved);
                                        MAPSEARCHER.UTILITIES.displayMessage(MAPSEARCHER.GLOBAL.DEFINES.toServerSuccessMessage); //will only display last success message
                                        //reset success message
                                        MAPSEARCHER.GLOBAL.DEFINES.toServerSuccessMessage = MAPSEARCHER.LOCALIZATION.DEFINES.L_Completed; //take out, because it could interfere with multiple saves
                                        MAPSEARCHER.GLOBAL.DEFINES.toServerSuccess = true; //not really used
                                        MAPSEARCHER.GLOBAL.DEFINES.csoi = 0; //reset
                                    }
                                });
                            });
                            MAPSEARCHER.TRACER.addTracer("[INFO]: toServer completed...");
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },                                                     //sends save dataPackages to the server via json
                    stickyMessage: function (stickyMessage) {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: stickyMessage started...");
                            //debug log this message
                            MAPSEARCHER.TRACER.addTracer("[INFO]: sticky message #" + MAPSEARCHER.GLOBAL.DEFINES.stickyMessageCount + ": " + stickyMessage); //send to debugger for logging
                            MAPSEARCHER.TRACER.addTracer("[INFO]: sticky message Count: " + MAPSEARCHER.GLOBAL.DEFINES.stickyMessageCount);

                            var duplicateStickyMessage = false;

                            try {
                                if (document.getElementById("stickyMessage" + MAPSEARCHER.GLOBAL.DEFINES.stickyMessageCount).innerHTML == stickyMessage) {
                                    duplicateStickyMessage = true;
                                } else {
                                    duplicateStickyMessage = false;
                                }
                            } catch (e) {
                                //could not find the ID
                                MAPSEARCHER.TRACER.addTracer("[INFO]: Could not find sticky message ID");
                                duplicateStickyMessage = false;
                            }

                            if (duplicateStickyMessage) {
                                MAPSEARCHER.TRACER.addTracer("[INFO]: same stick message as before, deleting...");

                                //remove that sticky message from the dom
                                $("#" + "stickyMessage" + MAPSEARCHER.GLOBAL.DEFINES.stickyMessageCount).remove();

                                //remove that sticky message from the record
                                MAPSEARCHER.GLOBAL.DEFINES.stickyMessageCount--;

                                MAPSEARCHER.TRACER.addTracer("[INFO]: new sticky message Count: " + MAPSEARCHER.GLOBAL.DEFINES.stickyMessageCount);
                            } else {
                                MAPSEARCHER.TRACER.addTracer("[INFO]: create sticky message");

                                //keep a count of messages
                                MAPSEARCHER.GLOBAL.DEFINES.stickymessageCount++;

                                //compile divID
                                var currentDivId = "stickyMessage" + MAPSEARCHER.GLOBAL.DEFINES.stickyMessageCount;

                                //create unique message div
                                var stickyMessageDiv = document.createElement("div");
                                stickyMessageDiv.setAttribute("id", currentDivId);
                                stickyMessageDiv.className = "stickyMessage";
                                document.getElementById("content_message").appendChild(stickyMessageDiv);

                                //assign the message
                                document.getElementById(currentDivId).innerHTML = stickyMessage;

                                //show message
                                document.getElementById(currentDivId).style.display = "block"; //display element
                            }
                            MAPSEARCHER.TRACER.addTracer("[INFO]: stickyMessage completed...");
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },                                              //display an inline sticky message (does not go away until duplicate is sent in)
                    displayMessage: function (message) {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: displayMessage started...");
                            //debug log this message
                            MAPSEARCHER.TRACER.addTracer("[INFO]: message #" + MAPSEARCHER.GLOBAL.DEFINES.messageCount + ": " + message); //send to debugger for logging

                            //keep a count of messages
                            MAPSEARCHER.GLOBAL.DEFINES.messageCount++;

                            //check to see if RIB is on
                            if (MAPSEARCHER.GLOBAL.DEFINES.RIBMode == true) {
                                MAPSEARCHER.TRACER.addTracer("[INFO]: RIB Mode: " + MAPSEARCHER.GLOBAL.DEFINES.RIBMode);
                                return;
                            } else {
                                //display the message

                                //debug
                                MAPSEARCHER.TRACER.addTracer("[INFO]: RIB Mode: " + MAPSEARCHER.GLOBAL.DEFINES.RIBMode);

                                //compile divID
                                var currentDivId = "message" + MAPSEARCHER.GLOBAL.DEFINES.messageCount;

                                //create unique message div
                                var messageDiv = document.createElement("div");
                                messageDiv.setAttribute("id", currentDivId);
                                messageDiv.className = "message";
                                document.getElementById("content_message").appendChild(messageDiv);

                                //assign the message
                                document.getElementById(currentDivId).innerHTML = message;

                                //determine if duplicate message
                                var duplicateMessage = false;
                                try {
                                    if (document.getElementById("message" + (MAPSEARCHER.GLOBAL.DEFINES.messageCount - 1)).innerHTML == message) {
                                        duplicateMessage = true;
                                    }
                                } catch (e) {
                                    //
                                }

                                if (duplicateMessage) {
                                    MAPSEARCHER.TRACER.addTracer("[INFO]: Same message to display as previous, not displaying");
                                    //remove the previous
                                    $("#" + "message" + (MAPSEARCHER.GLOBAL.DEFINES.messageCount - 1)).remove();
                                    //display the new
                                    document.getElementById(currentDivId).style.display = "block"; //display element
                                    //fade message out
                                    setTimeout(function () {
                                        $("#" + currentDivId).fadeOut("slow", function () {
                                            $("#" + currentDivId).remove();
                                        });
                                    }, 3000); //after 3 sec
                                } else {
                                    //MAPSEARCHER.TRACER.addTracer("[INFO]: Unique message to display");
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
                            MAPSEARCHER.TRACER.addTracer("[INFO]: displayMessage completed...");
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },                                                   //display an inline message
                    buttonActive: function (id) {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: buttonActive started...");
                            MAPSEARCHER.TRACER.addTracer("[INFO]: buttonActive: " + id);
                            switch (id) {
                                case "mapControls":
                                    if (MAPSEARCHER.GLOBAL.DEFINES.mapControlsDisplayed == false) { //not present
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
                                    if (MAPSEARCHER.GLOBAL.DEFINES.toolboxDisplayed == false) { //not present
                                        document.getElementById("content_menubar_toggleToolbox").className = document.getElementById("content_menubar_toggleToolbox").className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
                                        document.getElementById("content_toolbar_button_toggleToolbox").className = document.getElementById("content_toolbar_button_toggleToolbox").className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                                    } else { //present
                                        document.getElementById("content_menubar_toggleToolbox").className += " isActive2";
                                        document.getElementById("content_toolbar_button_toggleToolbox").className += " isActive";
                                    }
                                    break;
                                case "toolbar":
                                    if (MAPSEARCHER.GLOBAL.DEFINES.toolbarDisplayed == false) { //not present
                                        document.getElementById("content_menubar_toggleToolbar").className = document.getElementById("content_menubar_toggleToolbar").className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
                                    } else { //present
                                        document.getElementById("content_menubar_toggleToolbar").className += " isActive2";
                                    }
                                    break;
                                case "layer":
                                    if (MAPSEARCHER.GLOBAL.DEFINES.prevMapLayerActive != null) {
                                        document.getElementById("content_menubar_layer" + MAPSEARCHER.GLOBAL.DEFINES.prevMapLayerActive).className = document.getElementById("content_menubar_layer" + MAPSEARCHER.GLOBAL.DEFINES.prevMapLayerActive).className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
                                        document.getElementById("content_toolbar_button_layer" + MAPSEARCHER.GLOBAL.DEFINES.prevMapLayerActive).className = document.getElementById("content_toolbar_button_layer" + MAPSEARCHER.GLOBAL.DEFINES.prevMapLayerActive).className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                                        document.getElementById("content_toolbox_button_layer" + MAPSEARCHER.GLOBAL.DEFINES.prevMapLayerActive).className = document.getElementById("content_toolbox_button_layer" + MAPSEARCHER.GLOBAL.DEFINES.prevMapLayerActive).className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                                    }
                                    document.getElementById("content_menubar_layer" + MAPSEARCHER.GLOBAL.DEFINES.mapLayerActive).className += " isActive2";
                                    document.getElementById("content_toolbar_button_layer" + MAPSEARCHER.GLOBAL.DEFINES.mapLayerActive).className += " isActive";
                                    document.getElementById("content_toolbox_button_layer" + MAPSEARCHER.GLOBAL.DEFINES.mapLayerActive).className += " isActive";
                                    MAPSEARCHER.GLOBAL.DEFINES.prevMapLayerActive = MAPSEARCHER.GLOBAL.DEFINES.mapLayerActive; //set and hold the previous map layer active
                                    break;
                                case "kml":
                                    if (MAPSEARCHER.GLOBAL.DEFINES.kmlDisplayed == false) { //not present
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
                                    MAPSEARCHER.TRACER.addTracer("[INFO]: aa: " + MAPSEARCHER.GLOBAL.DEFINES.actionActive + "<br>" + "paa: " + MAPSEARCHER.GLOBAL.DEFINES.prevActionActive);
                                    if (MAPSEARCHER.GLOBAL.DEFINES.actionActive == "Other") {
                                        try {
                                            if (MAPSEARCHER.GLOBAL.DEFINES.prevActionActive != null) {
                                                document.getElementById("content_menubar_manage" + MAPSEARCHER.GLOBAL.DEFINES.prevActionActive).className = document.getElementById("content_menubar_manage" + MAPSEARCHER.GLOBAL.DEFINES.prevActionActive).className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
                                                document.getElementById("content_toolbar_button_manage" + MAPSEARCHER.GLOBAL.DEFINES.prevActionActive).className = document.getElementById("content_toolbar_button_manage" + MAPSEARCHER.GLOBAL.DEFINES.prevActionActive).className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                                                document.getElementById("content_toolbox_button_manage" + MAPSEARCHER.GLOBAL.DEFINES.prevActionActive).className = document.getElementById("content_toolbox_button_manage" + MAPSEARCHER.GLOBAL.DEFINES.prevActionActive).className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                                            }
                                        } catch (e) {
                                            MAPSEARCHER.TRACER.addTracer("[ERROR]: \"" + e + "\" (Could not find classname)");
                                        }
                                    } else {
                                        if (MAPSEARCHER.GLOBAL.DEFINES.prevActionActive != null) {
                                            document.getElementById("content_menubar_manage" + MAPSEARCHER.GLOBAL.DEFINES.prevActionActive).className = document.getElementById("content_menubar_manage" + MAPSEARCHER.GLOBAL.DEFINES.prevActionActive).className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
                                            document.getElementById("content_toolbar_button_manage" + MAPSEARCHER.GLOBAL.DEFINES.prevActionActive).className = document.getElementById("content_toolbar_button_manage" + MAPSEARCHER.GLOBAL.DEFINES.prevActionActive).className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                                            if (document.getElementById("content_toolbox_button_manage" + MAPSEARCHER.GLOBAL.DEFINES.prevActionActive)) {
                                                MAPSEARCHER.TRACER.addTracer("[INFO]: found " + MAPSEARCHER.GLOBAL.DEFINES.prevActionActive);
                                                document.getElementById("content_toolbox_button_manage" + MAPSEARCHER.GLOBAL.DEFINES.prevActionActive).className = document.getElementById("content_toolbox_button_manage" + MAPSEARCHER.GLOBAL.DEFINES.prevActionActive).className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                                            }

                                        }
                                        document.getElementById("content_menubar_manage" + MAPSEARCHER.GLOBAL.DEFINES.actionActive).className += " isActive2";
                                        document.getElementById("content_toolbar_button_manage" + MAPSEARCHER.GLOBAL.DEFINES.actionActive).className += " isActive";
                                        if (document.getElementById("content_toolbox_button_manage" + MAPSEARCHER.GLOBAL.DEFINES.actionActive)) {
                                            MAPSEARCHER.TRACER.addTracer("[INFO]: found " + MAPSEARCHER.GLOBAL.DEFINES.actionActive);
                                            document.getElementById("content_toolbox_button_manage" + MAPSEARCHER.GLOBAL.DEFINES.actionActive).className += " isActive";
                                        }
                                        MAPSEARCHER.GLOBAL.DEFINES.prevActionActive = MAPSEARCHER.GLOBAL.DEFINES.actionActive; //set and hold the previous map layer active
                                    }
                                    break;
                                case "overlayToggle":
                                    if (MAPSEARCHER.GLOBAL.DEFINES.buttonActive_overlayToggle == false) { //not present
                                        document.getElementById("content_menubar_overlayToggle").className += " isActive2";
                                        document.getElementById("content_toolbox_button_overlayToggle").className += " isActive";
                                        MAPSEARCHER.GLOBAL.DEFINES.buttonActive_overlayToggle = true;
                                    } else { //present
                                        document.getElementById("content_menubar_overlayToggle").className = document.getElementById("content_menubar_overlayToggle").className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
                                        document.getElementById("content_toolbox_button_overlayToggle").className = document.getElementById("content_toolbox_button_overlayToggle").className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                                        MAPSEARCHER.GLOBAL.DEFINES.buttonActive_overlayToggle = false;
                                    }
                                    break;
                                case "poiToggle":
                                    if (MAPSEARCHER.GLOBAL.DEFINES.buttonActive_poiToggle == false) { //not present
                                        document.getElementById("content_menubar_poiToggle").className += " isActive2";
                                        document.getElementById("content_toolbox_button_poiToggle").className += " isActive";
                                        MAPSEARCHER.GLOBAL.DEFINES.buttonActive_poiToggle = true;
                                    } else { //present
                                        document.getElementById("content_menubar_poiToggle").className = document.getElementById("content_menubar_poiToggle").className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
                                        document.getElementById("content_toolbox_button_poiToggle").className = document.getElementById("content_toolbox_button_poiToggle").className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                                        MAPSEARCHER.GLOBAL.DEFINES.buttonActive_poiToggle = false;
                                    }
                                    break;
                            }
                            MAPSEARCHER.TRACER.addTracer("[INFO]: buttonAction completed...");
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },                                                          //facilitates button sticky effect
                    confirmMessage: function (message) {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: confirmMessage started...");
                            //todo make this a better messagebox (visually pleasing and auto cancel if no action taken)
                            var response = confirm(message);
                            MAPSEARCHER.TRACER.addTracer("[INFO]: confirmMessage completed...");
                            return response;
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                            return false;
                        }
                    },                                                   //This triggers a confirm messagebox
                    polygonCenter: function (poly) {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: polygonCenter started...");
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
                            MAPSEARCHER.TRACER.addTracer("[INFO]: polygonCenter completed...");
                            return (new google.maps.LatLng(center_x, center_y));
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    rotate: function (degreeIn) {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: rotate started...");
                            if (MAPSEARCHER.GLOBAL.DEFINES.currentlyEditing == "yes") {
                                //if (MAPSEARCHER.GLOBAL.DEFINES.pageMode == "edit") {
                                MAPSEARCHER.GLOBAL.DEFINES.currentlyEditing = "yes"; //used to signify we are editing this overlay
                                MAPSEARCHER.GLOBAL.DEFINES.degree = MAPSEARCHER.GLOBAL.DEFINES.preservedRotation;
                                MAPSEARCHER.GLOBAL.DEFINES.degree += degreeIn;
                                if (degreeIn != 0) {
                                    $(function () {
                                        $("#overlay" + MAPSEARCHER.GLOBAL.DEFINES.workingOverlayIndex).rotate(MAPSEARCHER.GLOBAL.DEFINES.degree); //assign overlay to defined rotation
                                    });
                                } else { //if rotation is 0, reset rotation
                                    $(function () {
                                        MAPSEARCHER.GLOBAL.DEFINES.degree = 0;
                                        $("#overlay" + MAPSEARCHER.GLOBAL.DEFINES.workingOverlayIndex).rotate(MAPSEARCHER.GLOBAL.DEFINES.degree);
                                    });
                                }
                                MAPSEARCHER.GLOBAL.DEFINES.preservedRotation = MAPSEARCHER.GLOBAL.DEFINES.degree; //preserve rotation value
                                //MAPSEARCHER.GLOBAL.DEFINES.savingOverlayRotation[MAPSEARCHER.GLOBAL.DEFINES.workingOverlayIndex] = MAPSEARCHER.GLOBAL.DEFINES.preservedRotation; //just make sure it is prepping for save
                            }
                            if (MAPSEARCHER.GLOBAL.DEFINES.degree > 180) {
                                $('.knob').val(((MAPSEARCHER.GLOBAL.DEFINES.degree - 360) * (1))).trigger('change'); //used to correct for visual effect of knob error

                            } else {
                                $('.knob').val(MAPSEARCHER.GLOBAL.DEFINES.degree).trigger('change');
                            }
                            MAPSEARCHER.TRACER.addTracer("[INFO]: rotate completed...");
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    keepRotate: function (degreeIn) {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: keepRotate started...");
                            MAPSEARCHER.TRACER.addTracer("[INFO]: keepRotate: " + degreeIn + " woi: " + MAPSEARCHER.GLOBAL.DEFINES.workingOverlayIndex);
                            MAPSEARCHER.GLOBAL.DEFINES.currentlyEditing = "yes"; //used to signify we are editing this overlay
                            $("#overlay" + MAPSEARCHER.GLOBAL.DEFINES.workingOverlayIndex).rotate(degreeIn);
                            if (degreeIn < 0) {
                                $('.knob').val(((180 + degreeIn) + 180)).trigger('change');
                            } else {
                                $('.knob').val(degreeIn).trigger('change');
                            }
                            MAPSEARCHER.TRACER.addTracer("[INFO]: keepRotate completed...");
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    opacity: function (opacityIn) {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: opacity started...");
                            if (MAPSEARCHER.GLOBAL.DEFINES.preservedOpacity <= 1 && MAPSEARCHER.GLOBAL.DEFINES.preservedOpacity >= 0) {
                                MAPSEARCHER.TRACER.addTracer("[INFO]: add opacity: " + opacityIn + " to overlay" + MAPSEARCHER.GLOBAL.DEFINES.workingOverlayIndex);
                                var div = document.getElementById("overlay" + MAPSEARCHER.GLOBAL.DEFINES.workingOverlayIndex);
                                var newOpacity = MAPSEARCHER.GLOBAL.DEFINES.preservedOpacity + opacityIn;
                                if (newOpacity > 1) {
                                    newOpacity = 1;
                                }
                                if (newOpacity < 0) {
                                    newOpacity = 0;
                                }
                                div.style.opacity = newOpacity;
                                MAPSEARCHER.TRACER.addTracer("[INFO]: newOpacity: " + newOpacity);
                                MAPSEARCHER.GLOBAL.DEFINES.preservedOpacity = newOpacity;
                                MAPSEARCHER.TRACER.addTracer("[INFO]: MAPSEARCHER.GLOBAL.DEFINES.preservedOpacity: " + MAPSEARCHER.GLOBAL.DEFINES.preservedOpacity);
                                $("#overlayTransparencySlider").slider({ value: MAPSEARCHER.GLOBAL.DEFINES.preservedOpacity });
                            } else {
                                //could not change the opacity    
                            }
                            MAPSEARCHER.TRACER.addTracer("[INFO]: opacity completed...");
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    keepOpacity: function (opacityIn) {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: keepOpacity started...");
                            MAPSEARCHER.TRACER.addTracer("[INFO]: keepOpacity: " + opacityIn);
                            try {
                                var div = document.getElementById("overlay" + MAPSEARCHER.GLOBAL.DEFINES.workingOverlayIndex);
                                div.style.opacity = opacityIn;
                            } catch (e) {
                                //
                            }
                            MAPSEARCHER.GLOBAL.DEFINES.preservedOpacity = opacityIn;
                            MAPSEARCHER.TRACER.addTracer("[INFO]: keepOpacity completed...");
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    checkZoomLevel: function () {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: checkZoomLevel started...");
                            var currentZoomLevel = map.getZoom();
                            var currentMapType = map.getMapTypeId();
                            if (currentZoomLevel == MAPSEARCHER.GLOBAL.DEFINES.maxZoomLevel) {
                                MAPSEARCHER.UTILITIES.displayMessage(MAPSEARCHER.LOCALIZATION.DEFINES.L16);
                            } else {
                                switch (currentMapType) {
                                    case "roadmap": //roadmap and default
                                        if (currentZoomLevel == MAPSEARCHER.GLOBAL.DEFINES.minZoomLevel_Roadmap) {
                                            MAPSEARCHER.UTILITIES.displayMessage(MAPSEARCHER.LOCALIZATION.DEFINES.L17);
                                        }
                                        break;
                                    case "satellite": //sat
                                        if (currentZoomLevel == MAPSEARCHER.GLOBAL.DEFINES.minZoomLevel_Satellite) {
                                            MAPSEARCHER.UTILITIES.displayMessage(MAPSEARCHER.LOCALIZATION.DEFINES.L17);
                                        }
                                        break;
                                    case "hybrid": //sat
                                        if (currentZoomLevel == MAPSEARCHER.GLOBAL.DEFINES.minZoomLevel_Satellite) {
                                            MAPSEARCHER.UTILITIES.displayMessage(MAPSEARCHER.LOCALIZATION.DEFINES.L17);
                                        }
                                        break;
                                    case "terrain": //terrain only
                                        if (currentZoomLevel == MAPSEARCHER.GLOBAL.DEFINES.minZoomLevel_Terrain) {
                                            MAPSEARCHER.UTILITIES.displayMessage(MAPSEARCHER.LOCALIZATION.DEFINES.L17);
                                        }
                                        break;
                                    case "blocklot":
                                        if (currentZoomLevel == MAPSEARCHER.GLOBAL.DEFINES.minZoomLevel_BlockLot) {
                                            MAPSEARCHER.UTILITIES.displayMessage(MAPSEARCHER.LOCALIZATION.DEFINES.L17);
                                        }
                                        break;
                                }
                                if (MAPSEARCHER.GLOBAL.DEFINES.isCustomOverlay == true) {
                                    if (currentZoomLevel == MAPSEARCHER.GLOBAL.DEFINES.minZoomLevel_BlockLot) {
                                        MAPSEARCHER.UTILITIES.displayMessage(MAPSEARCHER.LOCALIZATION.DEFINES.L17);
                                    }
                                }
                            }
                            MAPSEARCHER.TRACER.addTracer("[INFO]: checkZoomLevel completed...");
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    displayCursorCoords: function (arg) {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: displayCursorCoords started...");
                            document.getElementById("cCoord").innerHTML = arg;
                            MAPSEARCHER.TRACER.addTracer("[INFO]: displayCursorCoords completed...");
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    codeLatLng: function (latlng) {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: codeLatLng started...");
                            if (MAPSEARCHER.GLOBAL.DEFINES.geocoder) {
                                MAPSEARCHER.GLOBAL.DEFINES.geocoder.geocode({ 'latLng': latlng }, function (results, status) {
                                    if (status == google.maps.GeocoderStatus.OK) {
                                        if (results[1]) {
                                            document.getElementById("content_toolbox_rgItem").value = results[0].formatted_address;
                                        }
                                        else {
                                            MAPSEARCHER.UTILITIES.displayMessage(MAPSEARCHER.LOCALIZATION.DEFINES.L25 + " " + status);
                                            document.getElementById("content_toolbox_rgItem").value = "";
                                        }
                                    }
                                });
                            }
                            MAPSEARCHER.TRACER.addTracer("[INFO]: codeLatLng completed...");
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    codeAddress: function (type, geo) {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: codeAddress started...");
                            var bounds = map.getBounds(); //get the current map bounds (should not be greater than the bounding box)
                            MAPSEARCHER.GLOBAL.DEFINES.geocoder.geocode({ 'address': geo, 'bounds': bounds }, function (results, status) { //geocode the lat/long of incoming with a bias towards the bounds
                                if (status == google.maps.GeocoderStatus.OK) { //if it worked
                                    map.setCenter(results[0].geometry.location); //set the center of map to the results
                                    MAPSEARCHER.UTILITIES.testBounds(); //test to make sure we are indeed in the bounds (have to do this because gmaps only allows for a BIAS of bounds and is not strict)
                                    if (MAPSEARCHER.GLOBAL.DEFINES.mapInBounds == "yes") { //if it is inside bounds
                                        if (MAPSEARCHER.GLOBAL.DEFINES.searchCount > 0) { //check to see if this is the first time searched, if not
                                            MAPSEARCHER.GLOBAL.DEFINES.searchResult.setMap(null); //set old search result to not display on map
                                        } else { //if it is the first time
                                            MAPSEARCHER.GLOBAL.DEFINES.searchCount++; //just interate
                                        }
                                        MAPSEARCHER.GLOBAL.DEFINES.searchResult = new google.maps.Marker({ //create a new marker
                                            map: map, //add to current map
                                            position: results[0].geometry.location //set position to search results
                                        });
                                        var searchResult_i = 1; //temp, placeholder for later multi search result support
                                        document.getElementById("searchResults_list").innerHTML = MAPSEARCHER.UTILITIES.writeHTML("searchResultListItem", searchResult_i, geo, "", "");
                                    } else { //if location found was outside strict map bounds...
                                        MAPSEARCHER.UTILITIES.displayMessage(MAPSEARCHER.LOCALIZATION.DEFINES.L24); //say so
                                    }

                                } else { //if the entire geocode did not work
                                    alert(MAPSEARCHER.LOCALIZATION.DEFINES.L6); //MAPSEARCHER.LOCALIZATION.DEFINES...
                                    MAPSEARCHER.TRACER.addTracer(MAPSEARCHER.LOCALIZATION.DEFINES.L6);
                                }
                            });
                            MAPSEARCHER.TRACER.addTracer("[INFO]: codeAddress completed...");
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    finder: function (stuff) {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: Finder started...");
                            if (stuff.length > 0) {
                                MAPSEARCHER.UTILITIES.codeAddress("lookup", stuff); //find the thing
                                document.getElementById("content_menubar_searchField").value = stuff; //sync menubar
                                document.getElementById("content_toolbar_searchField").value = stuff; //sync toolbar
                                document.getElementById("content_toolbox_searchField").value = stuff; //sync toolbox
                                MAPSEARCHER.ACTIONS.action("other"); //needed to clear out any action buttons that may be active
                                MAPSEARCHER.TRACER.addTracer("[INFO]: opening");
                                MAPSEARCHER.ACTIONS.openToolboxTab(1); //open the actions tab
                                MAPSEARCHER.TRACER.addTracer("[INFO]: supposedly opened");
                            } else {
                                //do nothing and keep quiet
                                MAPSEARCHER.TRACER.addTracer("[INFO]: no stuff");
                            }
                            MAPSEARCHER.TRACER.addTracer("[INFO]: Finder completed...");
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    testBounds: function () {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: testBounds started...");
                            if (MAPSEARCHER.GLOBAL.DEFINES.strictBounds != null) {
                                if (MAPSEARCHER.GLOBAL.DEFINES.strictBounds.contains(map.getCenter())) {
                                    MAPSEARCHER.GLOBAL.DEFINES.mapInBounds = "yes";
                                } else {
                                    MAPSEARCHER.GLOBAL.DEFINES.mapInBounds = "no";
                                    map.panTo(MAPSEARCHER.GLOBAL.DEFINES.mapCenter); //recenter
                                    MAPSEARCHER.UTILITIES.displayMessage(MAPSEARCHER.LOCALIZATION.DEFINES.L5);
                                }
                            }
                            MAPSEARCHER.TRACER.addTracer("[INFO]: testBounds completed...");
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    writeHTML: function (type, param1, param2, param3) {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: writeHTML started...");
                            var htmlString = "";
                            switch (type) {
                                case "poiListItem":
                                    MAPSEARCHER.TRACER.addTracer("[INFO]: Creating html String");
                                    MAPSEARCHER.GLOBAL.DEFINES.poiDesc[param1] = "New" + param3 + param2;
                                    htmlString = "<div id=\"poi" + param1 + "\" class=\"poiListItem\" title=\"" + MAPSEARCHER.GLOBAL.DEFINES.poiDesc[param1] + " \">" + MAPSEARCHER.GLOBAL.DEFINES.poiDesc[param1] + " <div class=\"poiActionButton\"><a title=\"" + MAPSEARCHER.LOCALIZATION.DEFINES.L66 + "\" href=\"#\" onclick=\"MAPSEARCHER.ACTIONS.poiEditMe(" + param1 + ");\"><img src=\"" + MAPSEARCHER.GLOBAL.DEFINES.baseURL + MAPSEARCHER.GLOBAL.DEFINES.baseImageDirURL + "edit.png\"/></a> <a title=\"" + MAPSEARCHER.LOCALIZATION.DEFINES.L61 + "\" id=\"poiToggle" + param1 + "\" href=\"#\"><img src=\"" + MAPSEARCHER.GLOBAL.DEFINES.baseURL + MAPSEARCHER.GLOBAL.DEFINES.baseImageDirURL + "sub.png\" onclick=\"MAPSEARCHER.ACTIONS.poiHideMe(" + param1 + ");\" /></a> <a title=\"" + MAPSEARCHER.LOCALIZATION.DEFINES.L63 + "\" href=\"#\" onclick=\"MAPSEARCHER.ACTIONS.poiDeleteMe(" + param1 + ");\"><img src=\"" + MAPSEARCHER.GLOBAL.DEFINES.baseURL + MAPSEARCHER.GLOBAL.DEFINES.baseImageDirURL + "delete.png\"/></a></div></div>";
                                    break;
                                case "poiListItemIncoming":
                                    MAPSEARCHER.TRACER.addTracer("[INFO]: Creating html String");
                                    MAPSEARCHER.GLOBAL.DEFINES.poiDesc[param1] = param3;
                                    htmlString = "<div id=\"poi" + param1 + "\" class=\"poiListItem\" title=\"" + param3 + " \">" + param3 + " <div class=\"poiActionButton\"><a title=\"" + MAPSEARCHER.LOCALIZATION.DEFINES.L66 + "\" href=\"#\" onclick=\"MAPSEARCHER.ACTIONS.poiEditMe(" + param1 + ");\"><img src=\"" + MAPSEARCHER.GLOBAL.DEFINES.baseURL + MAPSEARCHER.GLOBAL.DEFINES.baseImageDirURL + "edit.png\"/></a> <a title=\"" + MAPSEARCHER.LOCALIZATION.DEFINES.L61 + "\" id=\"poiToggle" + param1 + "\" href=\"#\"><img src=\"" + MAPSEARCHER.GLOBAL.DEFINES.baseURL + MAPSEARCHER.GLOBAL.DEFINES.baseImageDirURL + "sub.png\" onclick=\"MAPSEARCHER.ACTIONS.poiHideMe(" + param1 + ");\" /></a> <a title=\"" + MAPSEARCHER.LOCALIZATION.DEFINES.L63 + "\" href=\"#\" onclick=\"MAPSEARCHER.ACTIONS.poiDeleteMe(" + param1 + ");\"><img src=\"" + MAPSEARCHER.GLOBAL.DEFINES.baseURL + MAPSEARCHER.GLOBAL.DEFINES.baseImageDirURL + "delete.png\"/></a></div></div>";
                                    break;
                                case "poiDesc":
                                    MAPSEARCHER.TRACER.addTracer("[INFO]: Creating html String");
                                    htmlString = "<div class=\"poiDescContainer\"> <textarea id=\"poiDesc" + param1 + "\" class=\"descPOI\" placeholder=\"" + MAPSEARCHER.LOCALIZATION.DEFINES.L3 + "\" onblur=\"MAPSEARCHER.ACTIONS.poiGetDesc(" + param1 + ");\"></textarea> <br/> <div class=\"buttonPOIDesc\" id=\"poiGetDesc\" onClick=\"MAPSEARCHER.ACTIONS.poiGetDesc(" + param1 + ");\" >" + MAPSEARCHER.LOCALIZATION.DEFINES.L71 + "</div> </div>"; //title=\"" + MAPSEARCHER.LOCALIZATION.DEFINES.L65 + "\"
                                    break;
                                case "poiDescIncoming":
                                    MAPSEARCHER.TRACER.addTracer("[INFO]: Creating html String");
                                    htmlString = "<div class=\"poiDescContainer\"> <textarea id=\"poiDesc" + param1 + "\" class=\"descPOI\" onblur=\"MAPSEARCHER.ACTIONS.poiGetDesc(" + param1 + ");\">" + param2 + "</textarea> <br/> <div class=\"buttonPOIDesc\" id=\"poiGetDesc\" onClick=\"MAPSEARCHER.ACTIONS.poiGetDesc(" + param1 + ");\" >" + MAPSEARCHER.LOCALIZATION.DEFINES.L71 + "</div> </div>"; //title=\"" + MAPSEARCHER.LOCALIZATION.DEFINES.L65 + "\"
                                    break;
                                case "overlayListItem":
                                    MAPSEARCHER.TRACER.addTracer("[INFO]: Creating html String");
                                    htmlString = "<div id=\"overlayListItem" + param1 + "\" class=\"overlayListItem\" title=\"" + param2 + "\"> " + param2.substring(0, 20) + " <div class=\"overlayActionButton\"><a title=\"" + MAPSEARCHER.LOCALIZATION.DEFINES.L60 + "\" href=\"#\"><img src=\"" + MAPSEARCHER.GLOBAL.DEFINES.baseURL + MAPSEARCHER.GLOBAL.DEFINES.baseImageDirURL + "edit.png\" onclick=\"MAPSEARCHER.ACTIONS.overlayEditMe(" + param1 + ");\"/></a> <a id=\"overlayToggle" + param1 + "\" href=\"#\" title=\"" + MAPSEARCHER.LOCALIZATION.DEFINES.L61 + "\" ><img src=\"" + MAPSEARCHER.GLOBAL.DEFINES.baseURL + MAPSEARCHER.GLOBAL.DEFINES.baseImageDirURL + "sub.png\" onclick=\"MAPSEARCHER.ACTIONS.overlayHideMe(" + param1 + ");\" /></a> <a title=\"" + MAPSEARCHER.LOCALIZATION.DEFINES.L64 + "\" href=\"#\" ><img src=\"" + MAPSEARCHER.GLOBAL.DEFINES.baseURL + MAPSEARCHER.GLOBAL.DEFINES.baseImageDirURL + "delete.png\" onclick=\"MAPSEARCHER.ACTIONS.overlayDeleteMe(" + param1 + ");\"/></a> </div></div>";
                                    break;
                                case "searchResultListItem":
                                    MAPSEARCHER.TRACER.addTracer("[INFO]: Creating search html String");
                                    htmlString = "<div id=\"searchResultListItem" + param1 + "\" class=\"searchResultListItem\" title=\"" + param2 + "\"> " + param2.substring(0, 20) + " <div class=\"searchResultActionButton\"><a title=\"" + MAPSEARCHER.LOCALIZATION.DEFINES.L61 + "\" id=\"searchResultToggle" + param1 + "\" href=\"#\"><img src=\"" + MAPSEARCHER.GLOBAL.DEFINES.baseURL + MAPSEARCHER.GLOBAL.DEFINES.baseImageDirURL + "sub.png\" onclick=\"MAPSEARCHER.ACTIONS.searchResultHideMe(" + param1 + ");\" /></a> <a title=\"" + MAPSEARCHER.LOCALIZATION.DEFINES.L62 + "\" href=\"#\" onclick=\"MAPSEARCHER.ACTIONS.searchResultDeleteMe(" + param1 + ");\"><img src=\"" + MAPSEARCHER.GLOBAL.DEFINES.baseURL + MAPSEARCHER.GLOBAL.DEFINES.baseImageDirURL + "delete.png\"/></a></div></div>";
                                    break;
                            }
                            MAPSEARCHER.TRACER.addTracer("[INFO]: writeHTML completed...");
                            return htmlString;
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    resizeView: function () {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: resizeView started...");
                            //get sizes of elements already drawn
                            var totalPX = document.documentElement.clientHeight;
                            var headerPX = $("#mapedit_container").offset().top;
                            var widthPX = document.documentElement.clientWidth;

                            //set the width of the sf menu pane0 container
                            document.getElementById("mapedit_container_pane_0").style.width = widthPX + "px";

                            //if first time loaded (fixes issue where sfmenu was not loaded thus not calc'd in page height)
                            if (MAPSEARCHER.GLOBAL.DEFINES.pageLoadTime < (new Date().getTime())) {
                                headerPX = $("#mapedit_container").offset().top;
                            } else {
                                headerPX = $("#mapedit_container").offset().top + 28; //inside css
                            }

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

                                //override, just close toolbar (prevents overflow issues)
                                //mapedit_container_pane_1

                                var temp2 = toolbarButtonIds.length * 45;
                                temp2 = widthPX - temp2 - 60;
                                temp2 = Math.round(temp2 / 45);
                                //var buttonNonVisibleCount = Math.round(((toolbarButtonIds.length * 45) - (widthPX - 60)) / 45);
                                var buttonNonVisibleCount = temp2;
                                MAPSEARCHER.TRACER.addTracer("[INFO]: non vis button count: " + buttonNonVisibleCount);
                                var buttonVisibleCount = toolbarButtonIds.length - buttonNonVisibleCount;
                                MAPSEARCHER.TRACER.addTracer("[INFO]: vis button count: " + buttonVisibleCount);
                                for (var i = 0; i < buttonVisibleCount; i++) {
                                    MAPSEARCHER.TRACER.addTracer("[INFO]: showing: " + toolbarButtonIds[i]);
                                    //document.getElementById(toolbarButtonIds[i]).style.visibility = "show";
                                    //document.getElementById(toolbarButtonIds[i]).style.display = "block";
                                }
                                for (var i = buttonVisibleCount; i < buttonNonVisibleCount; i++) {
                                    MAPSEARCHER.TRACER.addTracer("[INFO]: hiding: " + toolbarButtonIds[i]);
                                    //document.getElementById(toolbarButtonIds[i]).style.visibility = "hidden";
                                    //document.getElementById(toolbarButtonIds[i]).style.display = "none";
                                }
                            }
                            //calculate how many buttons can be placed based on width
                            //display said buttons with arrow to cycle through

                            //detect and handle different widths
                            //todo make the 800,250 dynamic
                            if (widthPX <= 800) {
                                MAPSEARCHER.TRACER.addTracer("[INFO]: tablet viewing width detected...");
                                //toolbar
                                //menubar
                                //toolbox -min
                            }
                            if (widthPX <= 250) {
                                MAPSEARCHER.TRACER.addTracer("[INFO]: phone viewing width detected...");
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

                            //assign position of message box
                            //document.getElementById("mapedit_container_message").style["top"] = totalPX - bodyPX + "px";

                            //calculate percentage of height
                            var percentOfHeight = Math.round((bodyPX / totalPX) * 100);
                            //document.getElementById("mapedit_container").style.height = percentOfHeight + "%";
                            MAPSEARCHER.TRACER.addTracer("[INFO]: percentage of height: " + percentOfHeight);

                            MAPSEARCHER.TRACER.addTracer("[INFO]: sizes:<br>height: " + totalPX + " header: " + headerPX + " body: " + bodyPX + " pane0: " + pane0PX + " pane1: " + pane1PX + " pane2: " + pane2PX);
                            MAPSEARCHER.TRACER.addTracer("[INFO]: resizeView completed...");
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    latLngToPixel: function (latLng) {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: latLngToPixel started...");
                            var topRight = map.getProjection().fromLatLngToPoint(map.getBounds().getNorthEast());
                            var bottomLeft = map.getProjection().fromLatLngToPoint(map.getBounds().getSouthWest());
                            var scale = Math.pow(2, map.getZoom());
                            var worldPoint = map.getProjection().fromLatLngToPoint(latLng);
                            MAPSEARCHER.TRACER.addTracer("[INFO]: latLngToPixel completed...");
                            return new google.maps.Point((worldPoint.x - bottomLeft.x) * scale, (worldPoint.y - topRight.y) * scale);
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    checkKeyCode: function () {
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: checkKeyCode started...");
                            //go through each textareaids
                            var trueHits = 0;
                            for (var i = 0; i < MAPSEARCHER.GLOBAL.DEFINES.listOfTextAreaIds.length; i++) {
                                if (document.activeElement.id.indexOf(MAPSEARCHER.GLOBAL.DEFINES.listOfTextAreaIds[i]) != -1) {
                                    trueHits++;
                                }
                            }
                            if (trueHits > 0) {
                                MAPSEARCHER.GLOBAL.DEFINES.typingInTextArea = true;
                            } else {
                                MAPSEARCHER.GLOBAL.DEFINES.typingInTextArea = false;
                            }
                            MAPSEARCHER.TRACER.addTracer("[INFO]: checkKeyCode completed...");
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    CustomOverlay: function (id, bounds, image, map, rotation) {
                        //Starts the creation of a custom overlay div which contains a rectangular image.
                        //Supporting URL: https://developers.google.com/maps/documentation/javascript/overlays#CustomOverlays
                        try {
                            MAPSEARCHER.TRACER.addTracer("[INFO]: CustomOverlay started...");
                            MAPSEARCHER.GLOBAL.DEFINES.overlayCount++;                   //iterate how many overlays have been drawn
                            this.bounds_ = bounds;                      //set the bounds
                            this.image_ = image;                        //set source url
                            this.map_ = map;                            //set to map
                            MAPSEARCHER.GLOBAL.DEFINES.preservedRotation = rotation;     //set the rotation
                            this.div_ = null;                           //defines a property to hold the image's div. We'll actually create this div upon receipt of the onAdd() method so we'll leave it null for now.
                            this.index_ = id;                           //set the index/id of this overlay
                            MAPSEARCHER.TRACER.addTracer("[INFO]: CustomOverlay completed...");
                        } catch (err) {
                            MAPSEARCHER.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    }
                };
            }(),       //indirectly used fcns
            run: function () {
                try {
                    MAPSEARCHER.TRACER.addTracer("[INFO]: MAPSEARCHER.run started...");
                    //google.maps.event.addDomListener(window, 'load', MAPSEARCHER.GLOBAL.initGMap); //TEMP, when does this get fired?
                    MAPSEARCHER.UTILITIES.CustomOverlay.prototype = new google.maps.OverlayView(); //used to display custom overlay
                    MAPSEARCHER.GLOBAL.initDeclarations();
                    MAPSEARCHER.GLOBAL.initListOfTextAreaIds();
                    MAPSEARCHER.GLOBAL.initLocalization();
                    MAPSEARCHER.GLOBAL.initListeners();
                    MAPSEARCHER.GLOBAL.initJqueryElements();
                    initConfigSettings(); //c# to js
                    MAPSEARCHER.GLOBAL.initInterface(MAPSEARCHER.GLOBAL.DEFINES.collectionLoadType); //defines interface
                    MAPSEARCHER.GLOBAL.initGMap();
                    window.onkeypress = MAPSEARCHER.ACTIONS.keyPress; //keypress shortcuts/actions (MOVE TO dynamic)
                    MAPSEARCHER.TRACER.addTracer("[INFO]: MAPSEARCHER.run completed...");
                } catch(e) {
                    MAPSEARCHER.TRACER.addTracer("[ERROR]: MAPSEARCHER.run failed...");
                } 
            }                //init all fcn
        };
    }();
    //once it has been declared, run it
    MAPSEARCHER.run();
}

//must remain outside fcns at top level
var globalVar; //holds global vars

//add start listener
google.maps.event.addDomListener(window, 'load', initialize);

//init timebar onload
$(function () {

    //timebar
    $("#timebar").slider({
        range: true,
        min: 1565, //2do, read from config/collection
        max: 2015, //2do, read from config/collection
        values: [1790, 1915], //2do, read from config/collection
        slide: function (event, ui) {
            //$( "#timebar_value" ).val( ui.values[ 0 ] + " - " + ui.values[ 1 ] );
            $("#timebar").slider("values", ui.values[0], ui.values[1]);
            $("#timebar").labeledslider("values", [ui.values[0], ui.values[1]]);
        }
    });

    $("#timebar_value").val("$" + $("#timebar").slider("values", 0) + " - $" + $("#timebar").slider("values", 1));

    $("#timebar").labeledslider({
        range: true,
        min: 1565, //2do, read from config/collection
        max: 2015, //2do, read from config/collection
        tickInterval: 25, //2do, read from config/collection
        values: [1790, 1915], //2do, read from config/collection
        slide: function (event, ui) {
            //$( "#timebar_value" ).val( ui.values[ 0 ] + " - " + ui.values[ 1 ] );
            $("#timebar").slider("values", ui.values[0], ui.values[1]);
            $("#timebar").labeledslider("values", [ui.values[0], ui.values[1]]);
        }
    });

});

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
        header: "h3",
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
            currContent.toggleClass('accordion-content-active', !isPanelSelected)
            if (isPanelSelected) { currContent.slideUp(); } else { currContent.slideDown(); }
            // Cancels the default action
            return false;
        }
    });

    //set currently active boxes //2do, read from config
    //$( "#filterBox" ).accordion( "option", "active", 5 );

});

//init declarations
function initDeclarations() {
    //init global object    
    globalVar = function () {
        return {
            incomingPointFeatureType: [],
            incomingPointLabel: [],
            incomingPointCenter: [],
            isActive_searchControl: false,
            isActive_filterControl: false,
            pageViewMode: "geo", //2do, mv2config
            defaultMapZoom: 2, //2do, read from config
            collectionCenter: new google.maps.LatLng(29.9018607627349, -81.2975406646729),
            defaultMapCenter: new google.maps.LatLng(29.9018607627349, -81.2975406646729) //2do, read from config
        };
    }();
}

//init content
function initContent() {
    try {
        //
        if (globalVar.pageViewMode == "geo") {
            document.getElementById("resultsCounter").innerHTML = "displaying 50 of 4120 titles with geographic data"; //2do, localize
            document.getElementById("container_resultsPagingLeft").style.display = "none";
            document.getElementById("container_resultsPagingRight").style.display = "none";
        } else {
            document.getElementById("resultsCounter").innerHTML = "displaying 281 - 300 of 412 matching titles"; //2do, localize
            document.getElementById("container_resultsPagingLeft").style.display = "block";
            document.getElementById("container_resultsPagingRight").style.display = "block";
        }
    } catch (e) {
        //
    }
}

//init tooltips
function initTooltips() {
    try {
        //
        document.getElementById("view_geo").title = "Switch to Geographic View"; //2do, localize
        document.getElementById("view_brief").title = "Switch to Brief View"; //2do, localize
        document.getElementById("view_table").title = "Switch to Table View"; //2do, localize
        document.getElementById("view_thumb").title = "Switch to Thumbnail View"; //2do, localize
        //document.getElementById("resultsBox_sorter").title = "Sort"; //2do, localize

    } catch (e) {
        //
    }
}

//init listeners
function initListeners() {
    try {
        document.getElementById("view_geo").onclick = function () {
            globalVar.pageViewMode = "geo";
            document.getElementById("container_toolbar3").style.display = "block";
            document.getElementById("container_toolbox2").style.display = "block";
            document.getElementById("container_map").style.display = "block";
            document.getElementById("container_content1").style.display = "none";
            document.getElementById("container_content2").style.display = "none";
            document.getElementById("container_content3").style.display = "none";
            initContent();
            //window.open("http//www.ufdc.ufl.edu/l/usach/all/geo","_parent"); //2do, replace with dynamic/relateive links
        }
        document.getElementById("view_brief").onclick = function () {
            globalVar.pageViewMode = "brief";
            document.getElementById("container_toolbar3").style.display = "none";
            document.getElementById("container_toolbox2").style.display = "none";
            document.getElementById("container_map").style.display = "none";
            document.getElementById("container_content1").style.display = "block";
            document.getElementById("container_content2").style.display = "none";
            document.getElementById("container_content3").style.display = "none";
            initContent();
            //window.open("http//www.ufdc.ufl.edu/l/usach/all/brief","_parent"); //2do, replace with dynamic/relateive links
        }
        document.getElementById("view_table").onclick = function () {
            globalVar.pageViewMode = "table";
            document.getElementById("container_toolbar3").style.display = "none";
            document.getElementById("container_toolbox2").style.display = "none";
            document.getElementById("container_map").style.display = "none";
            document.getElementById("container_content1").style.display = "none";
            document.getElementById("container_content2").style.display = "block";
            document.getElementById("container_content3").style.display = "none";
            initContent();
            //window.open("http//www.ufdc.ufl.edu/l/usach/all/table","_parent"); //2do, replace with dynamic/relateive links
        }
        document.getElementById("view_thumb").onclick = function () {
            globalVar.pageViewMode = "thumb";
            document.getElementById("container_toolbar3").style.display = "none";
            document.getElementById("container_toolbox2").style.display = "none";
            document.getElementById("container_map").style.display = "none";
            document.getElementById("container_content1").style.display = "none";
            document.getElementById("container_content2").style.display = "none";
            document.getElementById("container_content3").style.display = "block";
            initContent();
            //window.open("http//www.ufdc.ufl.edu/l/usach/all/thumbs","_parent"); //2do, replace with dynamic/relateive links
        }
        document.getElementById("resultsSorter").onclick = function () {
            //
            //document.getElementById("resultsSorter").style.width = 140;
        }
        document.getElementById("searchControl_text").onclick = function () {
            //handle visuals
            if (globalVar.isActive_searchControl) {
                document.getElementById("container_searchControl").style.width = "95px";
                globalVar.isActive_searchControl = false;
            } else {
                document.getElementById("container_searchControl").style.width = "350px";
                globalVar.isActive_searchControl = true;
            }
            //handle actions
            //
        }
        document.getElementById("filterControl_text").onclick = function () {
            //handle visuals
            if (globalVar.isActive_filterControl) {
                document.getElementById("container_filterControl").style.width = "80px";
                globalVar.isActive_filterControl = false;
            } else {
                document.getElementById("container_filterControl").style.width = "555px";
                globalVar.isActive_filterControl = true;
            }
            //handle actions
            //
        }
    } catch (e) {
        alert("error: " + e);
    }
}

//init map and map objects
function initialize() {
    //init declarations
    initDeclarations();
    //init localization
    //initLocalization();
    //init content
    initContent();
    //init geo objects
    //initGeoObjects();
    //init incoming points
    initIncomingPoints();
    //init tooltips
    initTooltips();
    //init listeners
    initListeners();
    //define map
    var map = new google.maps.Map(document.getElementById('container_map'), {
        zoom: globalVar.defaultMapZoom,
        center: globalVar.defaultMapCenter,
        streetViewControl: false, //is streetview active?
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
    //visual refresh
    google.maps.visualRefresh = true;
    //define markers and infowindows
    var markers = [];
    var infowindows = [];
    for (var i = 1; i < globalVar.incomingPointCenter.length; i++) {
        var marker = new google.maps.Marker({
            position: globalVar.incomingPointCenter[i],
            title: "item " + i 
        });
        markers.push(marker);
        //var contentString = '<div id="infoWindow' + i + '" class="infoWindow">' +
		//	'<div id="infoWindow_header' + i + '" class="infoWindow_header"> ' + globalVar.incomingPointLabel[i] + ' </div>' +
		//	'<img id="infoWindow_thumb' + i + '" class="infoWindow_thumb" src="./resources/result1.jpg"/>' +
		//	'<div id="infoWindow_body' + i + '" class="infoWindow_body"> Aritst\'s rendering for possible reconstruction projects along the South side of Cuna Street </div>' +
		//	'</div>';
        //var infowindow = new google.maps.InfoWindow({
        //    content: contentString
        //});
        //infowindows.push(infowindow);
        //google.maps.event.addListener(marker, 'click', function () {
        //    for (var i = 0; i < markers.length; i++) {
        //        if (markers[i] == this) {
        //            infowindows[i].open(map, this);
        //        }
        //    }
        //});
    }
    var markerCluster = new MarkerClusterer(map, markers);
    //resize view
    resizeView('init');
    //switch map type if zoom max reached
    google.maps.event.addListener(map, 'zoom_changed', function () {
        if ((map.getZoom() == 15) && (map.getMapTypeId() == "terrain")) {
            map.setOptions({ mapTypeId: google.maps.MapTypeId.ROADMAP });
        }
    });
}

//create filter
function addFilter() {
    //selected_filters
}

//create x number of points
function createPoints(count) {

    var numberOfPointsToCreate = count + 50;

    for (var i = 50; i < numberOfPointsToCreate; i++) {
        globalVar.incomingPointFeatureType[i] = "poi";
        globalVar.incomingPointLabel[i] = "NewMarker" + i;
        var lat = 29.88385 + (i * .000001);
        var lng = -81.31985 + (i * .000001);
        globalVar.incomingPointCenter[i] = new google.maps.LatLng(lat, lng);
    }

}

//check scrollable areas
function checkScroll() {
    //document.getElementById("toolbar2").style.height
}

//resizes container based on the viewport
function resizeView(param) {
    try{
        //get sizes of elements already drawn
        var totalPX = document.documentElement.clientHeight;
        var headerPX = document.getElementById("sbkHmw_BannerDiv").offsetHeight;
        var toolbar1PX = document.getElementById("sbkAgm_MenuBar").offsetHeight;
        var toolbar2PX = document.getElementById("container_toolbar1").offsetHeight;
        var toolbar3PX = document.getElementById("container_toolbar2").offsetHeight;
        var widthPX = document.documentElement.clientWidth;
        var bodyPX = 0; //init

        //calculate and set sizes (the header and toolbar are taken into account as 'ghosts' in all but IE)
        if (param == 'init') {
            bodyPX = totalPX - ((headerPX * 2) + ((toolbar1PX + toolbar2PX + toolbar3PX))); //this accounts for toolbar not being loaded
            //bodyPX = totalPX - (265 * 2); //TEMP OVERRIDE
        } else {
            bodyPX = totalPX - ((headerPX * 2) + toolbar1PX + toolbar2PX + toolbar3PX);
            //bodyPX = totalPX - ((265 * 2) - 40); //TEMP OVERRIDE
        }
        if (navigator.appName == 'Microsoft Internet Explorer') {
            bodyPX = totalPX - (headerPX + toolbar1PX + toolbar2PX + toolbar3PX); //for IE, no ghosts
            //bodyPX = totalPX - 265; //TEMP OVERRIDE
        }

        //document.getElementById("container_toolbox1").style.height = bodyPX + "px";
        //document.getElementById("container_map").style.height = bodyPX + "px";
        //document.getElementById("container_toolbox2").style.height = bodyPX + "px";

        document.getElementById("container_toolbox1").style.height = (totalPX-240) + "px";
        document.getElementById("container_map").style.height = (totalPX - 210 -70 ) + "px";
        document.getElementById("container_toolbox2").style.height = (totalPX - 240) + "px";


    } catch (e) {
        console.log("ERROR: " + e);
    }
}

