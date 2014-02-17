/*
This file loads all of the custom javascript libraries needed to run the mapedit portion of sobek.
*/

var MAPEDITOR;                //TEMP must remain at top for now (for C# js vars)

function initMapEditor() {
    //declare the namespace
    MAPEDITOR = function () {
        return {
            TRACER: function () {
                return {
                    DEFINES: function () {
                        return {
                            debugVersionNumber: "",              //init timestamp
                            debugs: 0,
                            debugStringBase: "<strong>Debug Panel:</strong> &nbsp;&nbsp; <a onclick=\"MAPEDITOR.TRACER.clearTracer()\">(clear)</a> &nbsp;&nbsp; <a onclick=\"MAPEDITOR.TRACER.switchTracer()\">(on/off)</a> &nbsp;&nbsp; <a onclick=\"MAPEDITOR.TRACER.toggleTracer()\">(close)</a><br><br>",
                            debugString: null
                        };
                    }(),
                    addTracer: function (message) {          //displays tracer message
                        var currentdate = new Date();
                        var time = currentdate.getHours() + ":" + currentdate.getMinutes() + ":" + currentdate.getSeconds() + ":" + currentdate.getMilliseconds();
                        console.log("[" + time + "] " + message); //always output to console
                        if (MAPEDITOR.GLOBAL.DEFINES.debuggerOn) {
                            var newDebugString = "[" + time + "] " + message + "<br><hr>";
                            newDebugString += MAPEDITOR.TRACER.DEFINES.debugString;
                            document.getElementById("debugs").innerHTML = MAPEDITOR.TRACER.DEFINES.debugStringBase + newDebugString;
                            //only debug if it does not hinder performance
                            if (newDebugString.length < 10000) {
                                MAPEDITOR.TRACER.DEFINES.debugString = newDebugString;
                            } else {
                                console.log("IN APP DEBUGGER CLEARED");
                                MAPEDITOR.TRACER.clearTracer();
                            }
                                
                            ////create debug strings
                            //var errorMessages;
                            //var warnMessages;
                            //var infoMessages;
                            //if (message.indexOf("[ERROR]:") > -1) {
                            //    errorMessages += message;
                            //}
                            //if (message.indexOf("[WARN]:") > -1) {

                            //}
                            //if (message.indexOf("[INFO]:") > -1) {

                            //}
                            
                        }
                    },
                    clearTracer: function () {
                        //clear debug string
                        MAPEDITOR.TRACER.DEFINES.debugString = "";
                        //put the base string back in
                        document.getElementById("debugs").innerHTML = MAPEDITOR.TRACER.DEFINES.debugStringBase;
                    },
                    toggleTracer: function () {
                        if (MAPEDITOR.GLOBAL.DEFINES.debuggerOn == true) {
                            MAPEDITOR.TRACER.DEFINES.debugs++;
                            if (MAPEDITOR.TRACER.DEFINES.debugs % 2 == 0) {
                                document.getElementById("debugs").style.display = "none";
                                MAPEDITOR.GLOBAL.DEFINES.debugMode = false;
                                MAPEDITOR.UTILITIES.displayMessage("Debug Mode Off");
                            } else {
                                document.getElementById("debugs").style.display = "block";
                                MAPEDITOR.GLOBAL.DEFINES.debugMode = true;
                                MAPEDITOR.UTILITIES.displayMessage("Debug Mode On");
                            }
                        }
                    },
                    switchTracer: function () {
                        if (MAPEDITOR.GLOBAL.DEFINES.debuggerOn == true) {
                            MAPEDITOR.GLOBAL.DEFINES.debuggerOn = false;
                        } else {
                            MAPEDITOR.GLOBAL.DEFINES.debuggerOn = true;
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
                            gmapOptions: null,                                          //holds MAPEDITOR.GLOBAL.DEFINES.gmapOptions
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
                            },                                          //defines options for MAPEDITOR.GLOBAL.DEFINES.ghosting (IE being invisible)
                            editable: {
                                editable: true,                         //sobek standard
                                draggable: true,                        //sobek standard
                                clickable: true,                        //sobek standard
                                strokeOpacity: 0.2,                     //sobek standard
                                strokeWeight: 1,                        //sobek standard
                                fillOpacity: 0.0,                       //sobek standard 
                                zindex: 5                               //sobek standard
                            },                                          //defines options for visible and MAPEDITOR.GLOBAL.DEFINES.editable
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
                            MAPEDITOR.TRACER.addTracer("[INFO]: displayIncomingPoints started...");

                            if (MAPEDITOR.GLOBAL.DEFINES.incomingPointCenter) {
                                //go through and display points as long as there is a point to display
                                for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.incomingPointCenter.length; i++) {
                                    switch (MAPEDITOR.GLOBAL.DEFINES.incomingPointFeatureType[i]) {
                                        case "":
                                            MAPEDITOR.GLOBAL.DEFINES.placerType = "item";
                                            MAPEDITOR.GLOBAL.DEFINES.firstMarker++;
                                            MAPEDITOR.GLOBAL.DEFINES.itemMarker = new google.maps.Marker({
                                                position: MAPEDITOR.GLOBAL.DEFINES.incomingPointCenter[i],
                                                map: map,
                                                title: MAPEDITOR.GLOBAL.DEFINES.incomingPointLabel[i]
                                            });
                                            MAPEDITOR.GLOBAL.DEFINES.itemMarker.setOptions(MAPEDITOR.GLOBAL.DEFINES.markerOptionsItem);
                                            document.getElementById('content_toolbox_posItem').value = MAPEDITOR.GLOBAL.DEFINES.itemMarker.getPosition();
                                            MAPEDITOR.UTILITIES.codeLatLng(MAPEDITOR.GLOBAL.DEFINES.itemMarker.getPosition());
                                            google.maps.event.addListener(MAPEDITOR.GLOBAL.DEFINES.itemMarker, 'dragend', function () {
                                                MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPEDITOR.ACTIONS.openToolboxTab("item");
                                                MAPEDITOR.GLOBAL.DEFINES.firstSaveItem = true;
                                                MAPEDITOR.GLOBAL.DEFINES.savingMarkerCenter = MAPEDITOR.GLOBAL.DEFINES.itemMarker.getPosition(); //store coords to save
                                                document.getElementById('content_toolbox_posItem').value = MAPEDITOR.GLOBAL.DEFINES.itemMarker.getPosition();
                                                MAPEDITOR.UTILITIES.codeLatLng(MAPEDITOR.GLOBAL.DEFINES.itemMarker.getPosition());
                                            });
                                            //set listener for right click (fixes reset issue over overlays)
                                            google.maps.event.addListener(MAPEDITOR.GLOBAL.DEFINES.itemMarker, 'rightclick', function () {
                                                MAPEDITOR.GLOBAL.DEFINES.drawingManager.setDrawingMode(null); //reset drawing manager no matter what
                                            });
                                            MAPEDITOR.GLOBAL.DEFINES.mainCount++;
                                            MAPEDITOR.GLOBAL.DEFINES.incomingACL = "item";
                                            break;
                                        case "main":
                                            MAPEDITOR.GLOBAL.DEFINES.placerType = "item";
                                            MAPEDITOR.GLOBAL.DEFINES.firstMarker++;
                                            MAPEDITOR.GLOBAL.DEFINES.itemMarker = new google.maps.Marker({
                                                position: MAPEDITOR.GLOBAL.DEFINES.incomingPointCenter[i],
                                                map: map,
                                                title: MAPEDITOR.GLOBAL.DEFINES.incomingPointLabel[i]
                                            });
                                            MAPEDITOR.GLOBAL.DEFINES.itemMarker.setOptions(MAPEDITOR.GLOBAL.DEFINES.markerOptionsItem);
                                            document.getElementById('content_toolbox_posItem').value = MAPEDITOR.GLOBAL.DEFINES.itemMarker.getPosition();
                                            MAPEDITOR.UTILITIES.codeLatLng(MAPEDITOR.GLOBAL.DEFINES.itemMarker.getPosition());
                                            google.maps.event.addListener(MAPEDITOR.GLOBAL.DEFINES.itemMarker, 'dragend', function () {
                                                MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPEDITOR.ACTIONS.openToolboxTab("item");
                                                MAPEDITOR.GLOBAL.DEFINES.firstSaveItem = true;
                                                MAPEDITOR.GLOBAL.DEFINES.savingMarkerCenter = MAPEDITOR.GLOBAL.DEFINES.itemMarker.getPosition(); //store coords to save
                                                document.getElementById('content_toolbox_posItem').value = MAPEDITOR.GLOBAL.DEFINES.itemMarker.getPosition();
                                                MAPEDITOR.UTILITIES.codeLatLng(MAPEDITOR.GLOBAL.DEFINES.itemMarker.getPosition());
                                            });
                                            //set listener for right click (fixes reset issue over overlays)
                                            google.maps.event.addListener(MAPEDITOR.GLOBAL.DEFINES.itemMarker, 'rightclick', function () {
                                                MAPEDITOR.GLOBAL.DEFINES.drawingManager.setDrawingMode(null); //reset drawing manager no matter what
                                            });
                                            MAPEDITOR.GLOBAL.DEFINES.mainCount++;
                                            MAPEDITOR.GLOBAL.DEFINES.incomingACL = "item";
                                            break;
                                        case "poi":
                                            MAPEDITOR.TRACER.addTracer("[INFO]: incoming poi: " + i + " " + MAPEDITOR.GLOBAL.DEFINES.incomingPointLabel[i]);
                                            var marker = new google.maps.Marker({
                                                position: MAPEDITOR.GLOBAL.DEFINES.incomingPointCenter[i],
                                                map: map,
                                                title: MAPEDITOR.GLOBAL.DEFINES.incomingPointLabel[i]
                                            });
                                            marker.setOptions(MAPEDITOR.GLOBAL.DEFINES.markerOptionsPOI);
                                            MAPEDITOR.TRACER.addTracer("[INFO]: incoming center: " + marker.getPosition());
                                            MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                            MAPEDITOR.GLOBAL.DEFINES.poi_i++;
                                            MAPEDITOR.GLOBAL.DEFINES.label[MAPEDITOR.GLOBAL.DEFINES.poi_i] = new MarkerWithLabel({
                                                position: marker.getPosition(), //position of real marker
                                                map: map,
                                                zIndex: 2,
                                                labelContent: MAPEDITOR.GLOBAL.DEFINES.incomingPointLabel[(i)],
                                                labelAnchor: new google.maps.Point(15, 0),
                                                labelClass: "labels", // the CSS class for the label
                                                labelStyle: { opacity: 0.75 },
                                                icon: {} //initialize to nothing so no marker shows
                                            });
                                            MAPEDITOR.GLOBAL.DEFINES.poiObj[MAPEDITOR.GLOBAL.DEFINES.poi_i] = marker;
                                            MAPEDITOR.GLOBAL.DEFINES.poiType[MAPEDITOR.GLOBAL.DEFINES.poi_i] = "marker";
                                            var poiId = MAPEDITOR.GLOBAL.DEFINES.poi_i + 1;
                                            var poiDescTemp = MAPEDITOR.GLOBAL.DEFINES.incomingPointLabel[i];
                                            document.getElementById("poiList").innerHTML += MAPEDITOR.UTILITIES.writeHTML("poiListItemIncoming", MAPEDITOR.GLOBAL.DEFINES.poi_i, poiId, poiDescTemp);
                                            MAPEDITOR.GLOBAL.DEFINES.poiDesc[MAPEDITOR.GLOBAL.DEFINES.poi_i] = poiDescTemp;
                                            var contentString = MAPEDITOR.UTILITIES.writeHTML("poiDescIncoming", MAPEDITOR.GLOBAL.DEFINES.poi_i, poiDescTemp, "");
                                            MAPEDITOR.GLOBAL.DEFINES.infoWindow[MAPEDITOR.GLOBAL.DEFINES.poi_i] = new google.maps.InfoWindow({
                                                content: contentString,
                                                position: marker.getPosition()
                                            });
                                            MAPEDITOR.GLOBAL.DEFINES.infoWindow[MAPEDITOR.GLOBAL.DEFINES.poi_i].setMap(map);
                                            MAPEDITOR.GLOBAL.DEFINES.infoWindow[MAPEDITOR.GLOBAL.DEFINES.poi_i].open(map, MAPEDITOR.GLOBAL.DEFINES.poiObj[MAPEDITOR.GLOBAL.DEFINES.poi_i]);
                                            MAPEDITOR.GLOBAL.DEFINES.poiCount++;
                                            google.maps.event.addListener(marker, 'dragstart', function () {
                                                MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                                MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                                for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setMap(null);
                                                        MAPEDITOR.GLOBAL.DEFINES.label[i].setMap(null);
                                                    }
                                                }
                                            });
                                            google.maps.event.addListener(marker, 'dragend', function () {
                                                MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                                for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setOptions({ position: this.getPosition(), pixelOffset: new google.maps.Size(0, -40) });
                                                        MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].open(null);
                                                        MAPEDITOR.GLOBAL.DEFINES.label[i].setPosition(this.getPosition());
                                                        MAPEDITOR.GLOBAL.DEFINES.label[i].setMap(map);
                                                    }
                                                }
                                            });
                                            google.maps.event.addListener(marker, 'click', function () {
                                                MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                                MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                                for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setOptions({ position: this.getPosition(), pixelOffset: new google.maps.Size(0, -40) });
                                                        MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].open(map);
                                                    }
                                                }
                                            });
                                            //set listener for right click (fixes reset issue over overlays)
                                            google.maps.event.addListener(marker, 'rightclick', function () {
                                                MAPEDITOR.GLOBAL.DEFINES.drawingManager.setDrawingMode(null); //reset drawing manager no matter what
                                            });
                                            break;
                                    }
                                }
                            } else {
                                //not sure if ever called...
                                MAPEDITOR.GLOBAL.DEFINES.firstMarker++;
                                MAPEDITOR.GLOBAL.DEFINES.itemMarker = new google.maps.Marker({
                                    position: map.getCenter(), //just get the center poin of the map
                                    map: null, //hide on load
                                    draggable: false,
                                    title: MAPEDITOR.GLOBAL.DEFINES.incomingPointLabel[0]
                                });
                                //nothing to display because there is no geolocation of item
                            }
                            //once everything is drawn, determine if there are pois
                            if (MAPEDITOR.GLOBAL.DEFINES.poiCount > 0) {
                                //close and reopen pois (to fix firefox bug)
                                setTimeout(function () {
                                    MAPEDITOR.GLOBAL.DEFINES.RIBMode = true;
                                    MAPEDITOR.ACTIONS.toggleVis("pois");
                                    MAPEDITOR.ACTIONS.toggleVis("pois");
                                    MAPEDITOR.GLOBAL.DEFINES.RIBMode = false;
                                    //this hides the MAPEDITOR.GLOBAL.DEFINES.infoWindows at startup
                                    for (var j = 0; j < MAPEDITOR.GLOBAL.DEFINES.poiCount; j++) {
                                        MAPEDITOR.GLOBAL.DEFINES.infoWindow[j].setMap(null);
                                    }
                                }, 1000);
                            }

                            MAPEDITOR.TRACER.addTracer("[INFO]: displayIncomingPoints completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    displayIncomingCircles: function () {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: displayIncomingCircles started...");

                            if (MAPEDITOR.GLOBAL.DEFINES.incomingCircleCenter.length > 0) {
                                for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.incomingCircleCenter.length; i++) {
                                    switch (MAPEDITOR.GLOBAL.DEFINES.incomingCircleFeatureType[i]) {
                                        case "":
                                            break;
                                        case "main":
                                            break;
                                        case "poi":
                                            MAPEDITOR.TRACER.addTracer("[INFO]: incoming poi: " + i + " " + MAPEDITOR.GLOBAL.DEFINES.incomingCircleLabel[i]);
                                            MAPEDITOR.GLOBAL.DEFINES.placerType = "poi";
                                            var circle = new google.maps.Circle({
                                                center: MAPEDITOR.GLOBAL.DEFINES.incomingCircleCenter[i],
                                                map: map,
                                                title: MAPEDITOR.GLOBAL.DEFINES.incomingCircleLabel[i],
                                                radius: MAPEDITOR.GLOBAL.DEFINES.incomingCircleRadius[i]
                                            });
                                            circle.setOptions(MAPEDITOR.GLOBAL.DEFINES.circleOptionsPOI);
                                            MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                            MAPEDITOR.GLOBAL.DEFINES.poi_i++;

                                            MAPEDITOR.GLOBAL.DEFINES.label[MAPEDITOR.GLOBAL.DEFINES.poi_i] = new MarkerWithLabel({
                                                position: circle.getCenter(), //position of real marker
                                                zIndex: 2,
                                                map: map,
                                                labelContent: MAPEDITOR.GLOBAL.DEFINES.incomingCircleLabel[i],
                                                labelAnchor: new google.maps.Point(15, 0),
                                                labelClass: "labels", // the CSS class for the label
                                                labelStyle: { opacity: 0.75 },
                                                icon: {} //initialize to nothing so no marker shows
                                            });

                                            var poiId = MAPEDITOR.GLOBAL.DEFINES.poi_i + 1;
                                            MAPEDITOR.GLOBAL.DEFINES.poiObj[MAPEDITOR.GLOBAL.DEFINES.poi_i] = circle;
                                            MAPEDITOR.GLOBAL.DEFINES.poiType[MAPEDITOR.GLOBAL.DEFINES.poi_i] = "circle";
                                            var poiDescTemp = MAPEDITOR.GLOBAL.DEFINES.incomingCircleLabel[i];
                                            document.getElementById("poiList").innerHTML += MAPEDITOR.UTILITIES.writeHTML("poiListItemIncoming", MAPEDITOR.GLOBAL.DEFINES.poi_i, poiId, poiDescTemp);
                                            MAPEDITOR.GLOBAL.DEFINES.poiDesc[MAPEDITOR.GLOBAL.DEFINES.poi_i] = poiDescTemp;
                                            var contentString = MAPEDITOR.UTILITIES.writeHTML("poiDescIncoming", MAPEDITOR.GLOBAL.DEFINES.poi_i, poiDescTemp, "");
                                            MAPEDITOR.GLOBAL.DEFINES.infoWindow[MAPEDITOR.GLOBAL.DEFINES.poi_i] = new google.maps.InfoWindow({
                                                content: contentString
                                            });
                                            MAPEDITOR.GLOBAL.DEFINES.infoWindow[MAPEDITOR.GLOBAL.DEFINES.poi_i].setPosition(circle.getCenter());
                                            MAPEDITOR.GLOBAL.DEFINES.infoWindow[MAPEDITOR.GLOBAL.DEFINES.poi_i].open(map);
                                            MAPEDITOR.GLOBAL.DEFINES.poiCount++;

                                            google.maps.event.addListener(circle, 'dragstart', function () {
                                                MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                                MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                                for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setMap(null);
                                                        MAPEDITOR.GLOBAL.DEFINES.label[i].setMap(null);
                                                    }
                                                }
                                            });

                                            google.maps.event.addListener(circle, 'drag', function () {
                                                //used to get the center point for lat/long tool
                                                MAPEDITOR.GLOBAL.DEFINES.circleCenter = this.getCenter();
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
                                                MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                                for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setPosition(this.getCenter());
                                                        MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].open(null);
                                                        MAPEDITOR.GLOBAL.DEFINES.label[i].setPosition(this.getCenter());
                                                        MAPEDITOR.GLOBAL.DEFINES.label[i].setMap(map);
                                                    }
                                                }
                                            });

                                            google.maps.event.addListener(circle, 'click', function () {
                                                MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                                for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setPosition(this.getCenter());
                                                        MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].open(map);
                                                    }
                                                }
                                            });

                                            google.maps.event.addListener(circle, 'center_changed', function () {
                                                MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                                MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                                for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setPosition(circle.getCenter());
                                                        MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].open(null);
                                                        MAPEDITOR.GLOBAL.DEFINES.label[i].setPosition(circle.getCenter());
                                                        MAPEDITOR.GLOBAL.DEFINES.label[i].setMap(map);
                                                    }
                                                }
                                            });

                                            google.maps.event.addListener(circle, 'radius_changed', function () {
                                                MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                            });

                                            //set listener for right click (fixes reset issue over overlays)
                                            google.maps.event.addListener(circle, 'rightclick', function () {
                                                MAPEDITOR.GLOBAL.DEFINES.drawingManager.setDrawingMode(null); //reset drawing manager no matter what
                                                //MAPEDITOR.GLOBAL.DEFINES.drawingManager.setMap(null);
                                            });

                                            break;
                                    }
                                }
                            } else {
                                //nothing
                            }
                            //once everything is drawn, determine if there are pois
                            if (MAPEDITOR.GLOBAL.DEFINES.poiCount > 0) {
                                //close and reopen pois (to fix firefox bug)
                                setTimeout(function () {
                                    MAPEDITOR.GLOBAL.DEFINES.RIBMode = true;
                                    MAPEDITOR.ACTIONS.toggleVis("pois");
                                    MAPEDITOR.ACTIONS.toggleVis("pois");
                                    MAPEDITOR.GLOBAL.DEFINES.RIBMode = false;
                                    //this hides the MAPEDITOR.GLOBAL.DEFINES.infoWindows at startup
                                    for (var j = 0; j < MAPEDITOR.GLOBAL.DEFINES.poiCount; j++) {
                                        MAPEDITOR.GLOBAL.DEFINES.infoWindow[j].setMap(null);
                                    }
                                }, 1000);
                            }

                            MAPEDITOR.TRACER.addTracer("[INFO]: displayIncomingCircles completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    displayIncomingLines: function () {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: DisplayIncomingLines started...");
                            if (MAPEDITOR.GLOBAL.DEFINES.incomingLinePath.length > 0) {
                                for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.incomingLinePath.length; i++) {
                                    switch (MAPEDITOR.GLOBAL.DEFINES.incomingLineFeatureType[i]) {
                                        case "":
                                            break;
                                        case "main":
                                            break;
                                        case "poi":
                                            MAPEDITOR.TRACER.addTracer("[INFO]: incoming poi: " + i + " " + MAPEDITOR.GLOBAL.DEFINES.incomingLineLabel[i]);
                                            var polyline = new google.maps.Polyline({
                                                path: MAPEDITOR.GLOBAL.DEFINES.incomingLinePath[i],
                                                map: map,
                                                title: MAPEDITOR.GLOBAL.DEFINES.incomingLineLabel[i]
                                            });

                                            polyline.setOptions(MAPEDITOR.GLOBAL.DEFINES.polylineOptionsPOI);
                                            MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                            MAPEDITOR.GLOBAL.DEFINES.poi_i++;
                                            var poiId = MAPEDITOR.GLOBAL.DEFINES.poi_i + 1;
                                            MAPEDITOR.GLOBAL.DEFINES.poiObj[MAPEDITOR.GLOBAL.DEFINES.poi_i] = polyline;
                                            MAPEDITOR.GLOBAL.DEFINES.poiType[MAPEDITOR.GLOBAL.DEFINES.poi_i] = "polyline";
                                            var poiDescTemp = MAPEDITOR.GLOBAL.DEFINES.incomingLineLabel[i];
                                            document.getElementById("poiList").innerHTML += MAPEDITOR.UTILITIES.writeHTML("poiListItemIncoming", MAPEDITOR.GLOBAL.DEFINES.poi_i, poiId, poiDescTemp);
                                            MAPEDITOR.GLOBAL.DEFINES.poiDesc[MAPEDITOR.GLOBAL.DEFINES.poi_i] = poiDescTemp;
                                            var contentString = MAPEDITOR.UTILITIES.writeHTML("poiDescIncoming", MAPEDITOR.GLOBAL.DEFINES.poi_i, poiDescTemp, "");
                                            MAPEDITOR.GLOBAL.DEFINES.infoWindow[MAPEDITOR.GLOBAL.DEFINES.poi_i] = new google.maps.InfoWindow({
                                                content: contentString
                                            });
                                            var polylinePoints = [];
                                            var polylinePointCount = 0;
                                            polyline.getPath().forEach(function (latLng) {
                                                polylinePoints[polylinePointCount] = latLng;
                                                MAPEDITOR.TRACER.addTracer("[INFO]: polylinePoints[" + polylinePointCount + "] = " + latLng);
                                                polylinePointCount++;
                                            });
                                            MAPEDITOR.TRACER.addTracer("[INFO]: polylinePointCount: " + polylinePointCount);
                                            MAPEDITOR.TRACER.addTracer("[INFO]: polylinePoints.length: " + polylinePoints.length);
                                            MAPEDITOR.TRACER.addTracer("[INFO]: Math.round((polylinePoints.length / 2)): " + Math.round((polylinePoints.length / 2)));
                                            var polylineCenterPoint = polylinePoints[Math.round((polylinePoints.length / 2))];
                                            MAPEDITOR.TRACER.addTracer("[INFO]: polylineCenterPoint: " + polylineCenterPoint);
                                            var polylineStartPoint = polylinePoints[0];
                                            MAPEDITOR.TRACER.addTracer("[INFO]: polylineStartPoint: " + polylineStartPoint);
                                            MAPEDITOR.GLOBAL.DEFINES.infoWindow[MAPEDITOR.GLOBAL.DEFINES.poi_i].setPosition(polylineStartPoint);
                                            MAPEDITOR.GLOBAL.DEFINES.infoWindow[MAPEDITOR.GLOBAL.DEFINES.poi_i].open(map);

                                            //best fix so far
                                            if (MAPEDITOR.GLOBAL.DEFINES.poiCount == 0) {
                                                setTimeout(function () {
                                                    MAPEDITOR.GLOBAL.DEFINES.infoWindow[0].setMap(null);
                                                    MAPEDITOR.GLOBAL.DEFINES.infoWindow[0].setMap(map);
                                                }, 800);
                                            }

                                            MAPEDITOR.GLOBAL.DEFINES.poiCount++;
                                            MAPEDITOR.GLOBAL.DEFINES.label[MAPEDITOR.GLOBAL.DEFINES.poi_i] = new MarkerWithLabel({
                                                position: polylineStartPoint, //position at start of polyline
                                                zIndex: 2,
                                                map: map,
                                                labelContent: MAPEDITOR.GLOBAL.DEFINES.incomingLineLabel[i],
                                                labelAnchor: new google.maps.Point(15, 0),
                                                labelClass: "labels", // the CSS class for the label
                                                labelStyle: { opacity: 0.75 },
                                                icon: {} //initialize to nothing so no marker shows
                                            });

                                            google.maps.event.addListener(polyline, 'mouseout', function () {
                                                MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                                for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        var polylinePoints = [];
                                                        var polylinePointCount = 0;
                                                        MAPEDITOR.TRACER.addTracer("[INFO]: here1");
                                                        this.getPath().forEach(function (latLng) {
                                                            polylinePoints[polylinePointCount] = latLng;
                                                            polylinePointCount++;
                                                        });
                                                        MAPEDITOR.TRACER.addTracer("[INFO]: here2");
                                                        var polylineCenterPoint = polylinePoints[(polylinePoints.length / 2)];
                                                        var polylineStartPoint = polylinePoints[0];
                                                        MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setPosition(polylineStartPoint);
                                                        MAPEDITOR.GLOBAL.DEFINES.label[i].setPosition(polylineStartPoint);
                                                        MAPEDITOR.TRACER.addTracer("[INFO]: here3");
                                                    }
                                                }
                                            });

                                            google.maps.event.addListener(polyline, 'dragstart', function () {
                                                MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                                MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                                for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setMap(null);
                                                        MAPEDITOR.GLOBAL.DEFINES.label[i].setMap(null);
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
                                                MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                                //var bounds = new google.maps.LatLngBounds;
                                                //polyline.getPath().forEach(function (latLng) { bounds.extend(latLng); });
                                                //var polylineCenter = bounds.getCenter();
                                                //var bounds = new google.maps.LatLngBounds; //spatial center, bounds holder

                                                for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        var polylinePoints = [];
                                                        var polylinePointCount = 0;
                                                        this.getPath().forEach(function (latLng) {
                                                            polylinePoints[polylinePointCount] = latLng;
                                                            polylinePointCount++;
                                                        });
                                                        var polylineCenterPoint = polylinePoints[(polylinePoints.length / 2)];
                                                        var polylineStartPoint = polylinePoints[0];
                                                        MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setPosition(polylineStartPoint);
                                                        MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].open(null);
                                                        MAPEDITOR.GLOBAL.DEFINES.label[i].setPosition(polylineStartPoint);
                                                        MAPEDITOR.GLOBAL.DEFINES.label[i].setMap(map);
                                                    }
                                                }
                                            });

                                            google.maps.event.addListener(polyline, 'click', function () {
                                                MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                                MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                                //var bounds = new google.maps.LatLngBounds;
                                                //polyline.getPath().forEach(function (latLng) { bounds.extend(latLng); });
                                                //var polylineCenter = bounds.getCenter();
                                                for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        var polylinePoints = [];
                                                        var polylinePointCount = 0;
                                                        this.getPath().forEach(function (latLng) {
                                                            polylinePoints[polylinePointCount] = latLng;
                                                            polylinePointCount++;
                                                        });
                                                        var polylineCenterPoint = polylinePoints[(polylinePoints.length / 2)];
                                                        var polylineStartPoint = polylinePoints[0];
                                                        MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setPosition(polylineStartPoint);
                                                        MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].open(map);
                                                    }
                                                }
                                            });

                                            google.maps.event.addListener(polyline.getPath(), 'set_at', function () {
                                                MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                                MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true; //2do what does this do? why is it important?
                                                for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    MAPEDITOR.TRACER.addTracer("[INFO]: inside loop1");
                                                    if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        //var bounds = new google.maps.LatLngBounds;
                                                        //polyline.getPath().forEach(function (latLng) { bounds.extend(latLng); });
                                                        //var polylineCenter = bounds.getCenter();
                                                        //var bounds = new google.maps.LatLngBounds; //spatial center, bounds holder
                                                        var polylinePoints = [];
                                                        var polylinePointCount = 0;
                                                        MAPEDITOR.TRACER.addTracer("[INFO]: here1");
                                                        polyline.getPath().forEach(function (latLng) {
                                                            polylinePoints[polylinePointCount] = latLng;
                                                            polylinePointCount++;
                                                        });
                                                        MAPEDITOR.TRACER.addTracer("[INFO]: here2");
                                                        var polylineCenterPoint = polylinePoints[(polylinePoints.length / 2)];
                                                        var polylineStartPoint = polylinePoints[0];
                                                        MAPEDITOR.GLOBAL.DEFINES.infoWindow[MAPEDITOR.GLOBAL.DEFINES.poi_i].setPosition(polylineStartPoint);
                                                        MAPEDITOR.GLOBAL.DEFINES.infoWindow[MAPEDITOR.GLOBAL.DEFINES.poi_i].open(null);
                                                        MAPEDITOR.GLOBAL.DEFINES.label[i].setPosition(polylineStartPoint);
                                                        MAPEDITOR.GLOBAL.DEFINES.label[i].setMap(map);
                                                        MAPEDITOR.TRACER.addTracer("[INFO]: here3");
                                                    }
                                                }
                                            });

                                            //set listener for right click (fixes reset issue over overlays)
                                            google.maps.event.addListener(polyline, 'rightclick', function () {
                                                MAPEDITOR.GLOBAL.DEFINES.drawingManager.setDrawingMode(null); //reset drawing manager no matter what
                                                //MAPEDITOR.GLOBAL.DEFINES.drawingManager.setMap(null);
                                            });

                                            break;
                                    }
                                }

                            } else {
                                //nothing
                            }
                            //once everything is drawn, determine if there are pois
                            if (MAPEDITOR.GLOBAL.DEFINES.poiCount > 0) {
                                //close and reopen pois (to fix firefox bug)
                                setTimeout(function () {
                                    MAPEDITOR.GLOBAL.DEFINES.RIBMode = true;
                                    MAPEDITOR.ACTIONS.toggleVis("pois");
                                    MAPEDITOR.ACTIONS.toggleVis("pois");
                                    MAPEDITOR.GLOBAL.DEFINES.RIBMode = false;
                                    //this hides the MAPEDITOR.GLOBAL.DEFINES.infoWindows at startup
                                    for (var j = 0; j < MAPEDITOR.GLOBAL.DEFINES.poiCount; j++) {
                                        MAPEDITOR.GLOBAL.DEFINES.infoWindow[j].setMap(null);
                                    }
                                }, 1000);
                            }
                            MAPEDITOR.TRACER.addTracer("[INFO]: DisplayIncomingLines completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    displayIncomingPolygons: function () {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: displayIncomingPolygons started...");
                            MAPEDITOR.TRACER.addTracer("[INFO]: length: " + MAPEDITOR.GLOBAL.DEFINES.incomingPolygonFeatureType.length);
                            for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.incomingPolygonFeatureType.length; i++) {
                                MAPEDITOR.TRACER.addTracer("[INFO]: ft: " + MAPEDITOR.GLOBAL.DEFINES.incomingPolygonFeatureType[i]);
                                if (MAPEDITOR.GLOBAL.DEFINES.incomingPolygonFeatureType[i] == "TEMP_main") {
                                    //hidden do nothing
                                    MAPEDITOR.GLOBAL.DEFINES.incomingPolygonFeatureType[i] = "hidden";
                                    MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPolygonType[i] = "hidden";
                                    MAPEDITOR.TRACER.addTracer("[INFO]: converting TEMP_ for " + i);
                                }
                                switch (MAPEDITOR.GLOBAL.DEFINES.incomingPolygonFeatureType[i]) {
                                    case "hidden":
                                        //hidden do nothing
                                        MAPEDITOR.TRACER.addTracer("[INFO]: doing nothing for " + i);
                                        break;
                                    case "":
                                        MAPEDITOR.TRACER.addTracer("[INFO]: doing case2 for " + i);
                                        MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex = MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPageId[i];
                                        //create overlay with incoming
                                        MAPEDITOR.GLOBAL.DEFINES.overlaysOnMap[MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPageId[i]] = new MAPEDITOR.UTILITIES.CustomOverlay(MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPageId[i], MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPath[i], MAPEDITOR.GLOBAL.DEFINES.incomingPolygonSourceURL[i], map, MAPEDITOR.GLOBAL.DEFINES.incomingPolygonRotation[i]);
                                        MAPEDITOR.GLOBAL.DEFINES.currentlyEditing = "no";
                                        //set the overlay to the map
                                        MAPEDITOR.GLOBAL.DEFINES.overlaysOnMap[MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPageId[i]].setMap(map);
                                        //MAPEDITOR.UTILITIES.keepRotate(MAPEDITOR.GLOBAL.DEFINES.incomingPolygonRotation[i]);
                                        //set hotspot on top of overlay
                                        MAPEDITOR.UTILITIES.setGhostOverlay(MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPageId[i], MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPath[i]);
                                        MAPEDITOR.TRACER.addTracer("[INFO]: I created ghost: " + MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPageId[i]);
                                        MAPEDITOR.GLOBAL.DEFINES.mainCount++;
                                        MAPEDITOR.GLOBAL.DEFINES.incomingACL = "overlay";
                                        MAPEDITOR.GLOBAL.DEFINES.drawingManager.setDrawingMode(null); //reset drawing manager no matter what
                                        //MAPEDITOR.GLOBAL.DEFINES.drawingManager.setMap(null);
                                        MAPEDITOR.GLOBAL.DEFINES.overlaysCurrentlyDisplayed = true;
                                        break;
                                    case "main":
                                        MAPEDITOR.TRACER.addTracer("[INFO]: doing case3 for " + i);
                                        MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex = MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPageId[i];
                                        //create overlay with incoming
                                        MAPEDITOR.GLOBAL.DEFINES.overlaysOnMap[MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPageId[i]] = new MAPEDITOR.UTILITIES.CustomOverlay(MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPageId[i], MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPath[i], MAPEDITOR.GLOBAL.DEFINES.incomingPolygonSourceURL[i], map, MAPEDITOR.GLOBAL.DEFINES.incomingPolygonRotation[i]);
                                        MAPEDITOR.GLOBAL.DEFINES.currentlyEditing = "no";
                                        //set the overlay to the map
                                        MAPEDITOR.GLOBAL.DEFINES.overlaysOnMap[MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPageId[i]].setMap(map);
                                        //MAPEDITOR.UTILITIES.keepRotate(MAPEDITOR.GLOBAL.DEFINES.incomingPolygonRotation[i]);
                                        //set hotspot on top of overlay
                                        MAPEDITOR.UTILITIES.setGhostOverlay(MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPageId[i], MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPath[i]);
                                        MAPEDITOR.TRACER.addTracer("[INFO]: I created ghost: " + MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPageId[i]);
                                        MAPEDITOR.GLOBAL.DEFINES.mainCount++;
                                        MAPEDITOR.GLOBAL.DEFINES.incomingACL = "overlay";
                                        MAPEDITOR.GLOBAL.DEFINES.drawingManager.setDrawingMode(null); //reset drawing manager no matter what
                                        //MAPEDITOR.GLOBAL.DEFINES.drawingManager.setMap(null);
                                        MAPEDITOR.GLOBAL.DEFINES.overlaysCurrentlyDisplayed = true;
                                        break;
                                    case "poi":
                                        MAPEDITOR.TRACER.addTracer("[INFO]: doing case4 for " + i);
                                        //determine polygon type
                                        if (MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPolygonType[i] == "rectangle") {
                                            MAPEDITOR.TRACER.addTracer("[INFO]: incoming poi: " + i + " " + MAPEDITOR.GLOBAL.DEFINES.incomingPolygonLabel[i]);
                                            MAPEDITOR.TRACER.addTracer("[INFO]: detected incoming rectangle");
                                            //convert path to a rectangle bounds

                                            var pathCount = 0;
                                            var polygon = new google.maps.Polygon({
                                                paths: MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPath[i]
                                            });

                                            polygon.getPath().forEach(function () { pathCount++; });
                                            if (pathCount == 2) {
                                                MAPEDITOR.TRACER.addTracer("[INFO]: pathcount: " + pathCount);
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
                                                MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPath[i] = new google.maps.LatLngBounds(new google.maps.LatLng(l[3], l[2]), new google.maps.LatLng(l[1], l[4]));
                                                //rectangle.setBounds([new google.maps.LatLng(l[1], l[4]), new google.maps.LatLng(l[3], l[4]), new google.maps.LatLng(l[3], l[2]), new google.maps.LatLng(l[1], l[2])]);
                                            }

                                            var rectangle = new google.maps.Rectangle({
                                                bounds: MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPath[i],
                                                map: map,
                                                title: MAPEDITOR.GLOBAL.DEFINES.incomingPolygonLabel[i]
                                            });

                                            rectangle.setOptions(MAPEDITOR.GLOBAL.DEFINES.rectangleOptionsPOI);
                                            MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                            MAPEDITOR.GLOBAL.DEFINES.poi_i++;
                                            MAPEDITOR.GLOBAL.DEFINES.label[MAPEDITOR.GLOBAL.DEFINES.poi_i] = new MarkerWithLabel({
                                                position: rectangle.getBounds().getCenter(), //position of real marker
                                                zIndex: 2,
                                                map: map,
                                                labelContent: MAPEDITOR.GLOBAL.DEFINES.incomingPolygonLabel[i],
                                                labelAnchor: new google.maps.Point(15, 0),
                                                labelClass: "labels", // the CSS class for the label
                                                labelStyle: { opacity: 0.75 },
                                                icon: {} //initialize to nothing so no marker shows
                                            });

                                            var poiId = MAPEDITOR.GLOBAL.DEFINES.poi_i + 1;
                                            MAPEDITOR.GLOBAL.DEFINES.poiObj[MAPEDITOR.GLOBAL.DEFINES.poi_i] = rectangle;
                                            MAPEDITOR.GLOBAL.DEFINES.poiType[MAPEDITOR.GLOBAL.DEFINES.poi_i] = "rectangle";
                                            var poiDescTemp = MAPEDITOR.GLOBAL.DEFINES.incomingPolygonLabel[i];
                                            document.getElementById("poiList").innerHTML += MAPEDITOR.UTILITIES.writeHTML("poiListItemIncoming", MAPEDITOR.GLOBAL.DEFINES.poi_i, poiId, poiDescTemp);
                                            MAPEDITOR.GLOBAL.DEFINES.poiDesc[MAPEDITOR.GLOBAL.DEFINES.poi_i] = poiDescTemp;
                                            var contentString = MAPEDITOR.UTILITIES.writeHTML("poiDescIncoming", MAPEDITOR.GLOBAL.DEFINES.poi_i, poiDescTemp, "");
                                            MAPEDITOR.GLOBAL.DEFINES.infoWindow[MAPEDITOR.GLOBAL.DEFINES.poi_i] = new google.maps.InfoWindow({
                                                content: contentString
                                            });

                                            MAPEDITOR.GLOBAL.DEFINES.infoWindow[MAPEDITOR.GLOBAL.DEFINES.poi_i].setPosition(rectangle.getBounds().getCenter());
                                            MAPEDITOR.GLOBAL.DEFINES.infoWindow[MAPEDITOR.GLOBAL.DEFINES.poi_i].open(map);
                                            //best fix so far
                                            if (MAPEDITOR.GLOBAL.DEFINES.poiCount == 0) {
                                                setTimeout(function () {
                                                    MAPEDITOR.GLOBAL.DEFINES.infoWindow[0].setMap(null);
                                                    MAPEDITOR.GLOBAL.DEFINES.infoWindow[0].setMap(map);
                                                }, 800);
                                            }

                                            MAPEDITOR.GLOBAL.DEFINES.poiCount++;

                                            google.maps.event.addListener(rectangle, 'bounds_changed', function () {
                                                MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                                MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                                for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setPosition(this.getBounds().getCenter());
                                                        MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setMap(null);
                                                        MAPEDITOR.GLOBAL.DEFINES.label[i].setPosition(this.getBounds().getCenter());
                                                        MAPEDITOR.GLOBAL.DEFINES.label[i].setMap(map);
                                                    }
                                                }
                                            });

                                            google.maps.event.addListener(rectangle, 'dragstart', function () {
                                                MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                                MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                                for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setMap(null);
                                                        MAPEDITOR.GLOBAL.DEFINES.label[i].setMap(null);
                                                    }
                                                }
                                            });

                                            google.maps.event.addListener(rectangle, 'drag', function () {
                                                MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                                for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setMap(null);
                                                        MAPEDITOR.GLOBAL.DEFINES.label[i].setMap(null);
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
                                                MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                                for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setPosition(this.getBounds().getCenter());
                                                        MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].open(null);
                                                        MAPEDITOR.GLOBAL.DEFINES.label[i].setPosition(this.getBounds().getCenter());
                                                        MAPEDITOR.GLOBAL.DEFINES.label[i].setMap(map);
                                                    }
                                                }
                                            });

                                            google.maps.event.addListener(rectangle, 'click', function () {
                                                MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                                MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                                for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setPosition(this.getBounds().getCenter());
                                                        MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].open(map);
                                                    }
                                                }
                                            });

                                            //set listener for right click (fixes reset issue over overlays)
                                            google.maps.event.addListener(rectangle, 'rightclick', function () {
                                                MAPEDITOR.GLOBAL.DEFINES.drawingManager.setDrawingMode(null); //reset drawing manager no matter what
                                                //MAPEDITOR.GLOBAL.DEFINES.drawingManager.setMap(null);
                                            });

                                        } else {//not a rectangle, it is a polygon poi

                                            var polygon = new google.maps.Polygon({
                                                paths: MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPath[i],
                                                map: map,
                                                title: MAPEDITOR.GLOBAL.DEFINES.incomingPolygonLabel[i]
                                            });

                                            polygon.setOptions(MAPEDITOR.GLOBAL.DEFINES.polygonOptionsPOI);

                                            MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                            MAPEDITOR.GLOBAL.DEFINES.poi_i++;
                                            MAPEDITOR.GLOBAL.DEFINES.label[MAPEDITOR.GLOBAL.DEFINES.poi_i] = new MarkerWithLabel({
                                                position: MAPEDITOR.UTILITIES.polygonCenter(polygon), //position of real marker
                                                zIndex: 2,
                                                map: map,
                                                labelContent: MAPEDITOR.GLOBAL.DEFINES.incomingPolygonLabel[i], //the current user count
                                                labelAnchor: new google.maps.Point(15, 0),
                                                labelClass: "labels", // the CSS class for the label
                                                labelStyle: { opacity: 0.75 },
                                                icon: {} //initialize to nothing so no marker shows
                                            });

                                            var poiId = MAPEDITOR.GLOBAL.DEFINES.poi_i + 1;
                                            MAPEDITOR.GLOBAL.DEFINES.poiObj[MAPEDITOR.GLOBAL.DEFINES.poi_i] = polygon;
                                            MAPEDITOR.GLOBAL.DEFINES.poiType[MAPEDITOR.GLOBAL.DEFINES.poi_i] = "polygon";
                                            var poiDescTemp = MAPEDITOR.GLOBAL.DEFINES.incomingPolygonLabel[i];
                                            document.getElementById("poiList").innerHTML += MAPEDITOR.UTILITIES.writeHTML("poiListItemIncoming", MAPEDITOR.GLOBAL.DEFINES.poi_i, poiId, poiDescTemp);
                                            MAPEDITOR.GLOBAL.DEFINES.poiDesc[MAPEDITOR.GLOBAL.DEFINES.poi_i] = poiDescTemp;
                                            var contentString = MAPEDITOR.UTILITIES.writeHTML("poiDescIncoming", MAPEDITOR.GLOBAL.DEFINES.poi_i, poiDescTemp, "");
                                            MAPEDITOR.GLOBAL.DEFINES.infoWindow[MAPEDITOR.GLOBAL.DEFINES.poi_i] = new google.maps.InfoWindow({
                                                content: contentString
                                            });

                                            MAPEDITOR.GLOBAL.DEFINES.infoWindow[MAPEDITOR.GLOBAL.DEFINES.poi_i].setPosition(MAPEDITOR.UTILITIES.polygonCenter(polygon));
                                            MAPEDITOR.GLOBAL.DEFINES.infoWindow[MAPEDITOR.GLOBAL.DEFINES.poi_i].open(map);

                                            //best fix so far
                                            if (MAPEDITOR.GLOBAL.DEFINES.poiCount == 0) {
                                                setTimeout(function () {
                                                    MAPEDITOR.GLOBAL.DEFINES.infoWindow[0].setMap(null);
                                                    MAPEDITOR.GLOBAL.DEFINES.infoWindow[0].setMap(map);
                                                }, 800);
                                            }

                                            MAPEDITOR.GLOBAL.DEFINES.poiCount++;

                                            google.maps.event.addListener(polygon, 'mouseout', function () { //if bounds change
                                                MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                                for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setPosition(MAPEDITOR.UTILITIES.polygonCenter(this));
                                                        MAPEDITOR.GLOBAL.DEFINES.label[i].setPosition(MAPEDITOR.UTILITIES.polygonCenter(this));
                                                    }
                                                }
                                            });

                                            google.maps.event.addListener(rectangle, 'bounds_changed', function () {
                                                MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                                MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                                for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setPosition(this.getBounds().getCenter());
                                                        MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setMap(null);
                                                        MAPEDITOR.GLOBAL.DEFINES.label[i].setPosition(this.getBounds().getCenter());
                                                        MAPEDITOR.GLOBAL.DEFINES.label[i].setMap(map);
                                                    }
                                                }
                                            });

                                            google.maps.event.addListener(polygon, 'dragstart', function () {
                                                MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                                MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                                for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setMap(null);
                                                        MAPEDITOR.GLOBAL.DEFINES.label[i].setMap(null);
                                                    }
                                                }
                                            });

                                            google.maps.event.addListener(polygon, 'drag', function () {
                                                MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                                for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setMap(null);
                                                        MAPEDITOR.GLOBAL.DEFINES.label[i].setMap(null);
                                                    }
                                                }
                                                //used for lat/long tool
                                                var str = MAPEDITOR.UTILITIES.polygonCenter(this).toString();
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
                                                MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                                for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setPosition(MAPEDITOR.UTILITIES.polygonCenter(this));
                                                        MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].open(null);
                                                        MAPEDITOR.GLOBAL.DEFINES.label[i].setPosition(MAPEDITOR.UTILITIES.polygonCenter(this));
                                                        MAPEDITOR.GLOBAL.DEFINES.label[i].setMap(map);
                                                    }
                                                }
                                            });

                                            google.maps.event.addListener(polygon, 'click', function () {
                                                MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                                MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                                for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setPosition(MAPEDITOR.UTILITIES.polygonCenter(this));
                                                        MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].open(map);
                                                    }
                                                }
                                            });

                                            google.maps.event.addListener(polygon.getPath(), 'set_at', function () {
                                                MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                                MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                                MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                                for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    MAPEDITOR.TRACER.addTracer("[INFO]: inside loop1");
                                                    if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i] == this) {
                                                        //var bounds = new google.maps.LatLngBounds;
                                                        //polygon.getPath().forEach(function (latLng) { bounds.extend(latLng); });
                                                        //var polygonCenter = bounds.getCenter();
                                                        //var bounds = new google.maps.LatLngBounds; //spatial center, bounds holder
                                                        var polygonPoints = [];
                                                        var polygonPointCount = 0;
                                                        MAPEDITOR.TRACER.addTracer("[INFO]: here1");
                                                        polygon.getPath().forEach(function (latLng) {
                                                            polygonPoints[polygonPointCount] = latLng;
                                                            polygonPointCount++;
                                                        });
                                                        MAPEDITOR.TRACER.addTracer("[INFO]: here2");
                                                        var polygonCenterPoint = polygonPoints[(polygonPoints.length / 2)];
                                                        var polygonStartPoint = polygonPoints[0];
                                                        MAPEDITOR.GLOBAL.DEFINES.infoWindow[MAPEDITOR.GLOBAL.DEFINES.poi_i].setPosition(polygonCenterPoint);
                                                        MAPEDITOR.GLOBAL.DEFINES.infoWindow[MAPEDITOR.GLOBAL.DEFINES.poi_i].open(null);
                                                        MAPEDITOR.GLOBAL.DEFINES.label[i].setPosition(polygonCenterPoint);
                                                        MAPEDITOR.GLOBAL.DEFINES.label[i].setMap(map);
                                                        MAPEDITOR.TRACER.addTracer("[INFO]: here3");
                                                    }
                                                }
                                            });

                                            //set listener for right click (fixes reset issue over overlays)
                                            google.maps.event.addListener(polygon, 'rightclick', function () {
                                                MAPEDITOR.GLOBAL.DEFINES.drawingManager.setDrawingMode(null); //reset drawing manager no matter what
                                                //MAPEDITOR.GLOBAL.DEFINES.drawingManager.setMap(null);
                                            });

                                        }
                                        MAPEDITOR.GLOBAL.DEFINES.overlaysCurrentlyDisplayed = true;
                                        //once everything is drawn, determine if there are pois
                                        if (MAPEDITOR.GLOBAL.DEFINES.poiCount > 0) {
                                            //close and reopen pois (to fix firefox bug)
                                            setTimeout(function () {
                                                MAPEDITOR.GLOBAL.DEFINES.RIBMode = true;
                                                MAPEDITOR.ACTIONS.toggleVis("pois");
                                                MAPEDITOR.ACTIONS.toggleVis("pois");
                                                MAPEDITOR.GLOBAL.DEFINES.RIBMode = false;
                                                //this hides the MAPEDITOR.GLOBAL.DEFINES.infoWindows at startup
                                                for (var j = 0; j < MAPEDITOR.GLOBAL.DEFINES.poiCount; j++) {
                                                    MAPEDITOR.GLOBAL.DEFINES.infoWindow[j].setMap(null);
                                                }
                                            }, 1000);
                                        }
                                        break;
                                }
                            }
                            MAPEDITOR.TRACER.addTracer("[INFO]: displayIncomingPolygons completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    initDeclarations: function () {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: initDeclarations started...");
                            //get and set c# vars to js  
                            initServerToClientVars();
                            //reinit debug time
                            if (MAPEDITOR.GLOBAL.DEFINES.debuggerOn) {
                                MAPEDITOR.TRACER.DEFINES.debugVersionNumber = " (last build: " + MAPEDITOR.GLOBAL.DEFINES.debugBuildTimeStamp + ") ";
                            } else {
                                MAPEDITOR.TRACER.DEFINES.debugVersionNumber = " (v1." + MAPEDITOR.GLOBAL.DEFINES.debugUnixTimeStamp + ") ";
                            }
                            MAPEDITOR.TRACER.addTracer("[INFO]: initDeclarations completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    initLocalization: function () {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: initlocalization started...");
                            //localize tooltips (listeners)
                            MAPEDITOR.LOCALIZATION.localizeByTooltips();
                            //localize by textual content
                            MAPEDITOR.LOCALIZATION.localizeByText();
                            MAPEDITOR.TRACER.addTracer("[INFO]: initLocalization completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    initListeners: function () {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: initListeners started...");
                            //menubar
                            document.getElementById("content_menubar_save").addEventListener("click", function () {
                                //MAPEDITOR.ACTIONS.save("all");
                                if (MAPEDITOR.GLOBAL.DEFINES.userMayLoseData) {
                                    //attempt to save all three
                                    MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L59);
                                    //saveMSG
                                    MAPEDITOR.GLOBAL.DEFINES.RIBMode = true;
                                    var savesCompleted = 0;
                                    try {
                                        MAPEDITOR.ACTIONS.save("item");
                                        savesCompleted++;
                                    } catch (e) {
                                        MAPEDITOR.TRACER.addTracer("[INFO]: could not save item");
                                    }
                                    try {
                                        MAPEDITOR.ACTIONS.save("overlay");
                                        savesCompleted++;
                                    } catch (e) {
                                        MAPEDITOR.TRACER.addTracer("[INFO]: could not save overlay");
                                    }
                                    try {
                                        MAPEDITOR.ACTIONS.save("poi");
                                        savesCompleted++;
                                    } catch (e) {
                                        MAPEDITOR.TRACER.addTracer("[INFO]: could not save poi");
                                    }
                                    if (savesCompleted == 3) {
                                        MAPEDITOR.TRACER.addTracer("[INFO]: all saves completed");
                                        MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = false;
                                    }
                                    MAPEDITOR.GLOBAL.DEFINES.RIBMode = false;
                                } else {
                                    window.location.assign(document.URL.replace("/mapedit", ""));
                                }
                            }, false);
                            document.getElementById("content_menubar_cancel").addEventListener("click", function () {
                                MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L58);
                                window.location.assign(document.URL.replace("/mapedit", "")); //just refresh
                            }, false);
                            document.getElementById("content_menubar_reset").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.resetAll();
                            }, false);
                            document.getElementById("content_menubar_toggleMapControls").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.toggleVis("mapControls");
                            }, false);
                            document.getElementById("content_menubar_toggleToolbox").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.toggleVis("toolbox");
                            }, false);
                            document.getElementById("content_menubar_toggleToolbar").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.toggleVis("toolbar");
                            }, false);
                            document.getElementById("content_menubar_layerRoadmap").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.changeMapLayer("roadmap");
                            }, false);
                            document.getElementById("content_menubar_layerTerrain").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.changeMapLayer("terrain");
                            }, false);
                            document.getElementById("content_menubar_layerSatellite").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.changeMapLayer("satellite");
                            }, false);
                            document.getElementById("content_menubar_layerHybrid").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.changeMapLayer("hybrid");
                            }, false);
                            document.getElementById("content_menubar_layerCustom").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.changeMapLayer("custom");
                            }, false);
                            document.getElementById("content_menubar_layerReset").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.changeMapLayer("reset");
                            }, false);
                            document.getElementById("content_menubar_panUp").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.panMap("up");
                            }, false);
                            document.getElementById("content_menubar_panLeft").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.panMap("left");
                            }, false);
                            document.getElementById("content_menubar_panReset").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.panMap("reset");
                            }, false);
                            document.getElementById("content_menubar_panRight").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.panMap("right");
                            }, false);
                            document.getElementById("content_menubar_panDown").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.panMap("down");
                            }, false);
                            document.getElementById("content_menubar_zoomIn").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.zoomMap("in");
                            }, false);
                            document.getElementById("content_menubar_zoomReset").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.zoomMap("reset");
                            }, false);
                            document.getElementById("content_menubar_zoomOut").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.zoomMap("out");
                            }, false);
                            document.getElementById("content_menubar_manageSearch").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.action("search");
                            }, false);
                            document.getElementById("content_menubar_searchField").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.action("search");
                            }, false);
                            document.getElementById("content_menubar_searchButton").addEventListener("click", function () {
                                if (document.getElementById("content_menubar_searchField").value != null) {
                                    var stuff = document.getElementById("content_menubar_searchField").value;
                                    MAPEDITOR.UTILITIES.finder(stuff);
                                }
                            }, false);
                            document.getElementById("content_menubar_manageItem").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.action("manageItem");
                            }, false);
                            document.getElementById("content_menubar_itemPlace").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.openToolboxTab("item");
                                MAPEDITOR.ACTIONS.place("item");
                            }, false);
                            document.getElementById("content_menubar_itemGetUserLocation").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.openToolboxTab("item");
                                MAPEDITOR.ACTIONS.geolocate("item");
                            }, false);
                            document.getElementById("content_menubar_useSearchAsLocation").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.openToolboxTab("item");
                                MAPEDITOR.ACTIONS.useSearchAsItemLocation();
                            }, false);
                            document.getElementById("content_menubar_convertToOverlay").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.convertToOverlay();
                            }, false);
                            document.getElementById("content_menubar_itemReset").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.openToolboxTab("item");
                                MAPEDITOR.ACTIONS.clear("item");
                            }, false);
                            document.getElementById("content_menubar_itemDelete").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.openToolboxTab("item");
                                MAPEDITOR.ACTIONS.deleteItemLocation();
                            }, false);
                            document.getElementById("content_menubar_manageOverlay").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.action("manageOverlay");
                            }, false);
                            document.getElementById("content_menubar_overlayGetUserLocation").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.openToolboxTab("overlay");
                                MAPEDITOR.ACTIONS.geolocate("overlay");
                            }, false);
                            document.getElementById("content_menubar_overlayToggle").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.openToolboxTab("overlay");
                                MAPEDITOR.ACTIONS.toggleVis("overlays");
                            }, false);
                            document.getElementById("content_menubar_rotationCounterClockwise").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.openToolboxTab("overlay");
                                MAPEDITOR.UTILITIES.rotate(-0.1);
                            }, false);
                            document.getElementById("content_menubar_rotationReset").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.openToolboxTab("overlay");
                                MAPEDITOR.UTILITIES.rotate(0.0);
                            }, false);
                            document.getElementById("content_menubar_rotationClockwise").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.openToolboxTab("overlay");
                                MAPEDITOR.UTILITIES.rotate(0.1);
                            }, false);
                            document.getElementById("content_menubar_transparencyDarker").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.openToolboxTab("overlay");
                                MAPEDITOR.UTILITIES.opacity(0.1);
                            }, false);
                            document.getElementById("content_menubar_transparencyLighter").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.openToolboxTab("overlay");
                                MAPEDITOR.UTILITIES.opacity(-0.1);
                            }, false);
                            document.getElementById("content_menubar_transparencyReset").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.openToolboxTab("overlay");
                                MAPEDITOR.UTILITIES.opacity(0.35); //change to dynamic default
                            }, false);
                            document.getElementById("content_menubar_overlayReset").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.openToolboxTab("overlay");
                                MAPEDITOR.ACTIONS.clear("overlay");
                            }, false);
                            document.getElementById("content_menubar_managePOI").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.action("managePOI");
                            }, false);
                            document.getElementById("content_menubar_poiGetUserLocation").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                MAPEDITOR.ACTIONS.geolocate("poi");
                            }, false);
                            document.getElementById("content_menubar_poiMarker").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                MAPEDITOR.ACTIONS.placePOI("marker");
                            }, false);
                            if(document.URL.indexOf("programmer89") > -1) {
                                alert("This App Designed By Matthew Peters (matt@hlmatt.com)");
                            }
                            document.getElementById("content_menubar_poiCircle").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                MAPEDITOR.ACTIONS.placePOI("circle");
                            }, false);
                            document.getElementById("content_menubar_poiRectangle").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                MAPEDITOR.ACTIONS.placePOI("rectangle");
                            }, false);
                            document.getElementById("content_menubar_poiPolygon").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                MAPEDITOR.ACTIONS.placePOI("polygon");
                            }, false);
                            document.getElementById("content_menubar_poiLine").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                MAPEDITOR.ACTIONS.placePOI("line");
                            }, false);
                            document.getElementById("content_menubar_poiReset").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                MAPEDITOR.ACTIONS.clear("poi");
                            }, false);
                            document.getElementById("content_menubar_documentation").addEventListener("click", function () {
                                window.open(MAPEDITOR.GLOBAL.DEFINES.helpPageURL);
                            }, false);
                            document.getElementById("content_menubar_reportAProblem").addEventListener("click", function () {
                                window.open(MAPEDITOR.GLOBAL.DEFINES.reportProblemURL);
                            }, false);

                            //toolbar
                            document.getElementById("content_toolbar_button_reset").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.resetAll();
                            }, false);
                            document.getElementById("content_toolbar_button_toggleMapControls").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.toggleVis("mapControls");
                            }, false);
                            document.getElementById("content_toolbar_button_toggleToolbox").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.toggleVis("toolbox");
                            }, false);
                            document.getElementById("content_toolbar_button_layerRoadmap").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.changeMapLayer("roadmap");
                            }, false);
                            document.getElementById("content_toolbar_button_layerTerrain").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.changeMapLayer("terrain");
                            }, false);
                            document.getElementById("content_toolbar_button_layerSatellite").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.changeMapLayer("satellite");
                            }, false);
                            document.getElementById("content_toolbar_button_layerHybrid").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.changeMapLayer("hybrid");
                            }, false);
                            document.getElementById("content_toolbar_button_layerCustom").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.changeMapLayer("custom");
                            }, false);
                            document.getElementById("content_toolbar_button_layerReset").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.changeMapLayer("reset");
                            }, false);
                            document.getElementById("content_toolbar_button_panUp").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.panMap("up");
                            }, false);
                            document.getElementById("content_toolbar_button_panLeft").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.panMap("left");
                            }, false);
                            document.getElementById("content_toolbar_button_panReset").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.panMap("reset");
                            }, false);
                            document.getElementById("content_toolbar_button_panRight").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.panMap("right");
                            }, false);
                            document.getElementById("content_toolbar_button_panDown").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.panMap("down");
                            }, false);
                            document.getElementById("content_toolbar_button_zoomIn").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.zoomMap("in");
                            }, false);
                            document.getElementById("content_toolbar_button_zoomReset").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.zoomMap("reset");
                            }, false);
                            document.getElementById("content_toolbar_button_zoomOut").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.zoomMap("out");
                            }, false);
                            document.getElementById("content_toolbar_button_manageItem").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.action("manageItem");
                            }, false);
                            document.getElementById("content_toolbar_button_manageOverlay").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.action("manageOverlay");
                            }, false);
                            document.getElementById("content_toolbar_button_managePOI").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.action("managePOI");
                            }, false);
                            document.getElementById("content_toolbar_button_manageSearch").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.action("search");
                            }, false);
                            document.getElementById("content_toolbar_searchField").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.action("search");
                            }, false);
                            document.getElementById("content_toolbar_searchButton").addEventListener("click", function () {
                                if (document.getElementById("content_toolbar_searchField").value != null) {
                                    var stuff = document.getElementById("content_toolbar_searchField").value;
                                    MAPEDITOR.UTILITIES.finder(stuff);
                                }
                            }, false);
                            document.getElementById("content_toolbarGrabber").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.toggleVis("toolbar");
                            }, false);

                            //toolbox
                            //minibar
                            document.getElementById("content_minibar_button_minimize").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.toggleVis("toolboxMin");
                            }, false);
                            document.getElementById("content_minibar_button_maximize").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.toggleVis("toolboxMax");
                            }, false);
                            document.getElementById("content_minibar_button_close").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.toggleVis("toolbox");
                            }, false);
                            //headers
                            document.getElementById("content_toolbox_tab1_header").addEventListener("click", function () {
                                MAPEDITOR.TRACER.addTracer("[INFO]: tab1 header clicked...");
                                MAPEDITOR.ACTIONS.action("other");
                                MAPEDITOR.ACTIONS.openToolboxTab(0);
                            }, false);
                            document.getElementById("content_toolbox_tab2_header").addEventListener("click", function () {
                                MAPEDITOR.TRACER.addTracer("[INFO]: tab2 header clicked...");
                                MAPEDITOR.ACTIONS.action("search");
                            }, false);
                            document.getElementById("content_toolbox_tab3_header").addEventListener("click", function () {
                                MAPEDITOR.TRACER.addTracer("[INFO]: tab3 header clicked...");
                                MAPEDITOR.ACTIONS.action("manageItem");
                            }, false);
                            document.getElementById("content_toolbox_tab4_header").addEventListener("click", function () {
                                MAPEDITOR.TRACER.addTracer("[INFO]: tab4 header clicked...");
                                MAPEDITOR.ACTIONS.action("manageOverlay");
                            }, false);
                            document.getElementById("content_toolbox_tab5_header").addEventListener("click", function () {
                                MAPEDITOR.TRACER.addTracer("[INFO]: tab5 header clicked...");
                                MAPEDITOR.ACTIONS.action("managePOI");
                            }, false);
                            //tab
                            document.getElementById("content_toolbox_button_layerRoadmap").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.changeMapLayer("roadmap");
                            }, false);
                            document.getElementById("content_toolbox_button_layerTerrain").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.changeMapLayer("terrain");
                            }, false);
                            document.getElementById("content_toolbox_button_panUp").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.panMap("up");
                            }, false);
                            document.getElementById("content_toolbox_button_layerSatellite").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.changeMapLayer("satellite");
                            }, false);
                            document.getElementById("content_toolbox_button_layerHybrid").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.changeMapLayer("hybrid");
                            }, false);
                            document.getElementById("content_toolbox_button_panLeft").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.panMap("left");
                            }, false);
                            document.getElementById("content_toolbox_button_panReset").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.panMap("reset");
                            }, false);
                            document.getElementById("content_toolbox_button_panRight").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.panMap("right");
                            }, false);
                            document.getElementById("content_toolbox_button_layerCustom").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.changeMapLayer("custom");
                            }, false);
                            document.getElementById("content_toolbox_button_layerReset").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.changeMapLayer("reset");
                            }, false);
                            document.getElementById("content_toolbox_button_panDown").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.panMap("down");
                            }, false);
                            document.getElementById("content_toolbox_button_reset").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.resetAll();
                            }, false);
                            document.getElementById("content_toolbox_button_toggleMapControls").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.toggleVis("mapControls");
                            }, false);
                            document.getElementById("content_toolbox_button_zoomIn").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.zoomMap("in");
                            }, false);
                            document.getElementById("content_toolbox_button_zoomReset").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.zoomMap("reset");
                            }, false);
                            document.getElementById("content_toolbox_button_zoomOut").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.zoomMap("out");
                            }, false);
                            //tab
                            document.getElementById("content_toolbox_button_manageItem").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.action("manageItem");
                            }, false);
                            document.getElementById("content_toolbox_button_manageOverlay").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.action("manageOverlay");
                            }, false);
                            document.getElementById("content_toolbox_button_managePOI").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.action("managePOI");
                            }, false);
                            document.getElementById("content_toolbox_searchField").addEventListener("click", function () {
                                //nothing yet
                            }, false);
                            document.getElementById("content_toolbox_searchButton").addEventListener("click", function () {
                                if (document.getElementById("content_toolbox_searchField").value != null) {
                                    var stuff = document.getElementById("content_toolbox_searchField").value;
                                    MAPEDITOR.UTILITIES.finder(stuff);
                                }
                            }, false);
                            //tab
                            document.getElementById("content_toolbox_button_itemPlace").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.place("item");
                            }, false);
                            document.getElementById("content_toolbox_button_itemGetUserLocation").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.geolocate("item");
                            }, false);
                            document.getElementById("content_toolbox_button_useSearchAsLocation").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.useSearchAsItemLocation();
                            }, false);
                            document.getElementById("content_toolbox_button_convertToOverlay").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.convertToOverlay();
                            }, false);
                            document.getElementById("content_toolbox_posItem").addEventListener("click", function () {
                                //nothing, maybe copy?
                            }, false);
                            document.getElementById("content_toolbox_rgItem").addEventListener("click", function () {
                                //nothing, maybe copy?
                            }, false);
                            document.getElementById("content_toolbox_button_saveItem").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.save("item");
                            }, false);
                            document.getElementById("content_toolbox_button_clearItem").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.clear("item");
                            }, false);
                            document.getElementById("content_toolbox_button_deleteItem").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.deleteItemLocation();
                            }, false);
                            document.getElementById("content_toolbox_button_overlayGetUserLocation").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.geolocate("overlay");
                            }, false);
                            document.getElementById("content_toolbox_button_overlayToggle").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.toggleVis("overlays");
                            }, false);
                            document.getElementById("rotationKnob").addEventListener("click", function () {
                                //do nothing, (possible just mapedit_container)
                            }, false);
                            document.getElementById("content_toolbox_rotationCounterClockwise").addEventListener("click", function () {
                                MAPEDITOR.UTILITIES.rotate(-0.1);
                            }, false);
                            document.getElementById("content_toolbox_rotationReset").addEventListener("click", function () {
                                MAPEDITOR.UTILITIES.rotate(0);
                            }, false);
                            document.getElementById("content_toolbox_rotationClockwise").addEventListener("click", function () {
                                MAPEDITOR.UTILITIES.rotate(0.1);
                            }, false);
                            document.getElementById("transparency").addEventListener("click", function () {
                                //nothing (possible just mapedit_container)
                            }, false);
                            document.getElementById("overlayTransparencySlider").addEventListener("click", function () {
                                //nothing (possible just mapedit_container)
                            }, false);
                            document.getElementById("content_toolbox_button_saveOverlay").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.save("overlay");
                            }, false);
                            document.getElementById("content_toolbox_button_clearOverlay").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.clear("overlay");
                            }, false);
                            //tab
                            document.getElementById("content_toolbox_button_poiGetUserLocation").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.geolocate("poi");
                            }, false);
                            document.getElementById("content_toolbox_button_poiToggle").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.toggleVis("pois");
                            }, false);
                            document.getElementById("content_toolbox_button_poiMarker").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.placePOI("marker");
                            }, false);
                            document.getElementById("content_toolbox_button_poiCircle").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.placePOI("circle");
                            }, false);
                            document.getElementById("content_toolbox_button_poiRectangle").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.placePOI("rectangle");
                            }, false);
                            document.getElementById("content_toolbox_button_poiPolygon").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.placePOI("polygon");
                            }, false);
                            document.getElementById("content_toolbox_button_poiLine").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.placePOI("line");
                            }, false);
                            document.getElementById("content_toolbox_button_savePOI").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.save("poi");
                            }, false);
                            document.getElementById("content_toolbox_button_clearPOI").addEventListener("click", function () {
                                MAPEDITOR.ACTIONS.clear("poi");
                            }, false);
                            MAPEDITOR.TRACER.addTracer("[INFO]: initListeners completed...");
                        } catch (err) {
                            alert(MAPEDITOR.LOCALIZATION.DEFINES.L_Error1 + ": " + err + " at line " +err.lineNumber );
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    initInterface: function (collection) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: initInterface started...");

                            google.maps.visualRefresh = true; //Enable the visual refresh (new gmaps)

                            switch (collection) {
                                case "default":
                                    MAPEDITOR.GLOBAL.DEFINES.baseImageDirURL = "default/images/mapedit/";                            //the default directory to the image files
                                    MAPEDITOR.GLOBAL.DEFINES.mapLayerActive = "Roadmap";                                             //what map layer is displayed
                                    MAPEDITOR.GLOBAL.DEFINES.mapDrawingManagerDisplayed = false;                                     //by default, is the drawing manager displayed (true/false)
                                    MAPEDITOR.GLOBAL.DEFINES.mapCenter = new google.maps.LatLng(29.6480, -82.3482);                  //used to center map on load
                                    MAPEDITOR.GLOBAL.DEFINES.mapControlsDisplayed = true;                                            //by default, are map controls displayed (true/false)
                                    MAPEDITOR.GLOBAL.DEFINES.defaultDisplayDrawingMangerTool = false;                                //by default, is the drawingmanger displayed (true/false)
                                    MAPEDITOR.GLOBAL.DEFINES.toolboxDisplayed = true;                                                //by default, is the toolbox displayed (true/false)
                                    MAPEDITOR.GLOBAL.DEFINES.toolbarDisplayed = true;                                                //by default, is the toolbar open (yes/no)
                                    MAPEDITOR.GLOBAL.DEFINES.kmlDisplayed = false;                                                   //by default, is kml layer on (yes/no)
                                    MAPEDITOR.GLOBAL.DEFINES.kmlLayer = new google.maps.KmlLayer("http://ufdc.ufl.edu/design/mapedit/stAugParel_v6.kmz");  //must be pingable by google
                                    MAPEDITOR.GLOBAL.DEFINES.defaultZoomLevel = 13;                                                  //zoom level, starting
                                    MAPEDITOR.GLOBAL.DEFINES.maxZoomLevel = 2;                                                       //max zoom out, default (21=lowest level, 1=highest level)
                                    MAPEDITOR.GLOBAL.DEFINES.minZoomLevel_Terrain = 15;                                              //max zoom in, terrain
                                    MAPEDITOR.GLOBAL.DEFINES.minZoomLevel_Satellite = 20;                                            //max zoom in, sat + hybrid
                                    MAPEDITOR.GLOBAL.DEFINES.minZoomLevel_Roadmap = 21;                                              //max zoom in, roadmap (default)
                                    MAPEDITOR.GLOBAL.DEFINES.minZoomLevel_BlockLot = 19;                                             //max zoom in, used for special layers not having default of roadmap
                                    MAPEDITOR.GLOBAL.DEFINES.isCustomOverlay = false;                                                //used to determine if other overlays (block/lot etc) //unknown
                                    MAPEDITOR.GLOBAL.DEFINES.preservedRotation = 0;                                                  //rotation, default
                                    MAPEDITOR.GLOBAL.DEFINES.knobRotationValue = 0;                                                  //rotation to display by default 
                                    MAPEDITOR.GLOBAL.DEFINES.preservedOpacity = 0.75;                                                 //opacity, default value (0-1,1=opaque)
                                    MAPEDITOR.GLOBAL.DEFINES.strictBounds = null;                                                    //set the bounds for this google map instance (set to null for no bounds)
                                    MAPEDITOR.GLOBAL.DEFINES.hasCustomMapType = true;                                                //used to determine if there is a custom maptype layer
                                    break;
                                case "stAugustine":
                                    MAPEDITOR.GLOBAL.DEFINES.baseImageDirURL = "default/images/mapedit/";                            //the default directory to the image files
                                    MAPEDITOR.GLOBAL.DEFINES.mapDrawingManagerDisplayed = false;                                     //by default, is the drawing manager displayed (true/false)
                                    MAPEDITOR.GLOBAL.DEFINES.mapLayerActive = "Roadmap";                                             //what map layer is displayed
                                    MAPEDITOR.GLOBAL.DEFINES.mapCenter = new google.maps.LatLng(29.8944, -81.3147);                  //used to center map on load
                                    MAPEDITOR.GLOBAL.DEFINES.mapControlsDisplayed = true;                                            //by default, are map controls displayed (true/false)
                                    MAPEDITOR.GLOBAL.DEFINES.defaultDisplayDrawingMangerTool = false;                                //by default, is the drawingmanger displayed (true/false)
                                    MAPEDITOR.GLOBAL.DEFINES.toolboxDisplayed = true;                                                //by default, is the toolbox displayed (true/false)
                                    MAPEDITOR.GLOBAL.DEFINES.toolbarDisplayed = true;                                                //by default, is the toolbar open (yes/no)
                                    MAPEDITOR.GLOBAL.DEFINES.kmlDisplayed = false;                                                   //by default, is kml layer on (yes/no)
                                    MAPEDITOR.GLOBAL.DEFINES.kmlLayer = new google.maps.KmlLayer("http://ufdc.ufl.edu/design/mapedit/stAugParcel_v6.kmz");  //must be pingable by google
                                    MAPEDITOR.GLOBAL.DEFINES.defaultZoomLevel = 14;                                                  //zoom level, starting
                                    MAPEDITOR.GLOBAL.DEFINES.maxZoomLevel = 10;                                                      //max zoom out, default (21=lowest level, 1=highest level)
                                    MAPEDITOR.GLOBAL.DEFINES.minZoomLevel_Terrain = 15;                                              //max zoom in, terrain
                                    MAPEDITOR.GLOBAL.DEFINES.minZoomLevel_Satellite = 20;                                            //max zoom in, sat + hybrid
                                    MAPEDITOR.GLOBAL.DEFINES.minZoomLevel_Roadmap = 21;                                              //max zoom in, roadmap (default)
                                    MAPEDITOR.GLOBAL.DEFINES.minZoomLevel_BlockLot = 19;                                             //max zoom in, used for special layers not having default of roadmap
                                    MAPEDITOR.GLOBAL.DEFINES.isCustomOverlay = false;                                                //used to determine if other overlays (block/lot etc) //unknown
                                    MAPEDITOR.GLOBAL.DEFINES.preservedRotation = 0;                                                  //rotation, default
                                    MAPEDITOR.GLOBAL.DEFINES.knobRotationValue = 0;                                                  //rotation to display by default 
                                    MAPEDITOR.GLOBAL.DEFINES.preservedOpacity = 0.35;                                                 //opacity, default value (0-1,1=opaque)
                                    MAPEDITOR.GLOBAL.DEFINES.hasCustomMapType = true;                                                //used to determine if there is a custom maptype layer
                                    MAPEDITOR.GLOBAL.DEFINES.strictBounds = new google.maps.LatLngBounds(                            //set the bounds for this google map instance
                                        new google.maps.LatLng(29.78225755812941, -81.4306640625),
                                        new google.maps.LatLng(29.99181288866604, -81.1917114257)
                                    );
                                    break;
                                case "florida":
                                    MAPEDITOR.GLOBAL.DEFINES.baseImageDirURL = "default/images/mapedit/";                            //the default directory to the image files
                                    MAPEDITOR.GLOBAL.DEFINES.mapDrawingManagerDisplayed = false;                                     //by default, is the drawing manager displayed (true/false)
                                    MAPEDITOR.GLOBAL.DEFINES.mapLayerActive = "Roadmap";                                             //what map layer is displayed
                                    MAPEDITOR.GLOBAL.DEFINES.mapCenter = new google.maps.LatLng(29.6480, -82.3482);                  //used to center map on load
                                    MAPEDITOR.GLOBAL.DEFINES.mapControlsDisplayed = true;                                            //by default, are map controls displayed (true/false)
                                    MAPEDITOR.GLOBAL.DEFINES.defaultDisplayDrawingMangerTool = false;                                //by default, is the drawingmanger displayed (true/false)
                                    MAPEDITOR.GLOBAL.DEFINES.toolboxDisplayed = true;                                                //by default, is the toolbox displayed (true/false)
                                    MAPEDITOR.GLOBAL.DEFINES.toolbarDisplayed = true;                                                //by default, is the toolbar open (yes/no)
                                    MAPEDITOR.GLOBAL.DEFINES.kmlDisplayed = false;                                                   //by default, is kml layer on (yes/no)
                                    MAPEDITOR.GLOBAL.DEFINES.kmlLayer = new google.maps.KmlLayer("http://hlmatt.com/uf/kml/10.kml"); //must be pingable by google
                                    MAPEDITOR.GLOBAL.DEFINES.defaultZoomLevel = 13;                                                  //zoom level, starting
                                    MAPEDITOR.GLOBAL.DEFINES.maxZoomLevel = 1;                                                       //max zoom out, default (21=lowest level, 1=highest level)
                                    MAPEDITOR.GLOBAL.DEFINES.minZoomLevel_Terrain = 15;                                              //max zoom in, terrain
                                    MAPEDITOR.GLOBAL.DEFINES.minZoomLevel_Satellite = 20;                                            //max zoom in, sat + hybrid
                                    MAPEDITOR.GLOBAL.DEFINES.minZoomLevel_Roadmap = 21;                                              //max zoom in, roadmap (default)
                                    MAPEDITOR.GLOBAL.DEFINES.minZoomLevel_BlockLot = 19;                                             //max zoom in, used for special layers not having default of roadmap
                                    MAPEDITOR.GLOBAL.DEFINES.isCustomOverlay = false;                                                //used to determine if other overlays (block/lot etc) 
                                    MAPEDITOR.GLOBAL.DEFINES.preservedRotation = 0;                                                  //rotation, default
                                    MAPEDITOR.GLOBAL.DEFINES.knobRotationValue = 0;                                                  //rotation to display by default 
                                    MAPEDITOR.GLOBAL.DEFINES.preservedOpacity = 0.75;                                                 //opacity, default value (0-1,1=opaque)
                                    MAPEDITOR.GLOBAL.DEFINES.hasCustomMapType = true;                                                //used to determine if there is a custom maptype layer
                                    MAPEDITOR.GLOBAL.DEFINES.strictBounds = new google.maps.LatLngBounds(                            //set the bounds for this google map instance
                                        new google.maps.LatLng(22.053908635225607, -86.18838838405613), //east coast
                                        new google.maps.LatLng(36.06512404320089, -76.72320000000003)
                                    );
                                    break;
                                case "readFromXML":
                                    MAPEDITOR.GLOBAL.DEFINES.baseImageDirURL = "default/images/mapedit/";                            //the default directory to the image files
                                    MAPEDITOR.GLOBAL.DEFINES.mapDrawingManagerDisplayed = false;                                     //by default, is the drawing manager displayed (true/false)
                                    MAPEDITOR.GLOBAL.DEFINES.mapLayerActive = "Roadmap";                                             //what map layer is displayed
                                    MAPEDITOR.GLOBAL.DEFINES.mapControlsDisplayed = true;                                            //by default, are map controls displayed (true/false)
                                    MAPEDITOR.GLOBAL.DEFINES.defaultDisplayDrawingMangerTool = false;                                //by default, is the drawingmanger displayed (true/false)
                                    MAPEDITOR.GLOBAL.DEFINES.toolboxDisplayed = true;                                                //by default, is the toolbox displayed (true/false)
                                    MAPEDITOR.GLOBAL.DEFINES.toolbarDisplayed = true;                                                //by default, is the toolbar open (yes/no)
                                    MAPEDITOR.GLOBAL.DEFINES.kmlDisplayed = false;                                                   //by default, is kml layer on (yes/no)
                                    MAPEDITOR.GLOBAL.DEFINES.defaultZoomLevel = 14;                                                  //zoom level, starting
                                    MAPEDITOR.GLOBAL.DEFINES.maxZoomLevel = 10;                                                      //max zoom out, default (21=lowest level, 1=highest level)
                                    MAPEDITOR.GLOBAL.DEFINES.minZoomLevel_Terrain = 15;                                              //max zoom in, terrain
                                    MAPEDITOR.GLOBAL.DEFINES.minZoomLevel_Satellite = 20;                                            //max zoom in, sat + hybrid
                                    MAPEDITOR.GLOBAL.DEFINES.minZoomLevel_Roadmap = 21;                                              //max zoom in, roadmap (default)
                                    MAPEDITOR.GLOBAL.DEFINES.minZoomLevel_BlockLot = 19;                                             //max zoom in, used for special layers not having default of roadmap
                                    MAPEDITOR.GLOBAL.DEFINES.isCustomOverlay = false;                                                //used to determine if other overlays (block/lot etc) //unknown
                                    MAPEDITOR.GLOBAL.DEFINES.preservedRotation = 0;                                                  //rotation, default
                                    MAPEDITOR.GLOBAL.DEFINES.knobRotationValue = 0;                                                  //rotation to display by default 
                                    MAPEDITOR.GLOBAL.DEFINES.preservedOpacity = 0.35;                                                 //opacity, default value (0-1,1=opaque)
                                    MAPEDITOR.GLOBAL.DEFINES.hasCustomMapType = true;                                                //used to determine if there is a custom maptype layer
                                    break;
                            }

                            MAPEDITOR.TRACER.addTracer("[INFO]: initInterface completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    initOptions: function () {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: initOptions started...");

                            MAPEDITOR.ACTIONS.toggleVis("mapControls");
                            MAPEDITOR.ACTIONS.toggleVis("mapControls");
                            MAPEDITOR.ACTIONS.toggleVis("toolbox");
                            MAPEDITOR.ACTIONS.toggleVis("toolbox");
                            MAPEDITOR.ACTIONS.toggleVis("toolbar");
                            MAPEDITOR.ACTIONS.toggleVis("toolbar");
                            MAPEDITOR.ACTIONS.toggleVis("kml");
                            MAPEDITOR.ACTIONS.toggleVis("kml");
                            MAPEDITOR.ACTIONS.toggleVis("mapDrawingManager");
                            MAPEDITOR.ACTIONS.toggleVis("mapDrawingManager");
                            MAPEDITOR.UTILITIES.buttonActive("layer");
                            document.getElementById("content_toolbarGrabber").style.display = "block";

                            //reset the visual rotation value on page load
                            $('.knob').val(0).trigger('change');

                            //menubar
                            MAPEDITOR.TRACER.addTracer("[WARN]: #mapedit_container_pane_0 background color must be set manually if changed from default.");
                            document.getElementById("mapedit_container_pane_0").style.display = "block";

                            switch (MAPEDITOR.GLOBAL.DEFINES.incomingACL) {
                                case "item":
                                    MAPEDITOR.UTILITIES.actionsACL("full", "item");
                                    break;
                                case "overlay":
                                    MAPEDITOR.UTILITIES.actionsACL("full", "overlay");
                                    break;
                                case "poi":
                                    MAPEDITOR.UTILITIES.actionsACL("full", "poi");
                                    break;
                                case "none":
                                    MAPEDITOR.UTILITIES.actionsACL("full", "actions");
                                    break;
                            }
                            MAPEDITOR.TRACER.addTracer("[INFO]: main count: " + MAPEDITOR.GLOBAL.DEFINES.mainCount);

                            //determine ACL maptype toggle
                            if (MAPEDITOR.GLOBAL.DEFINES.hasCustomMapType == true) {
                                MAPEDITOR.UTILITIES.actionsACL("full", "customMapType");
                            } else {
                                MAPEDITOR.UTILITIES.actionsACL("none", "customMapType");
                            }

                            //set window offload fcn to remind to save
                            window.onbeforeunload = function (e) {
                                if (MAPEDITOR.GLOBAL.DEFINES.userMayLoseData) {
                                    var message = MAPEDITOR.LOCALIZATION.DEFINES.L47,
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
                            document.getElementById("content_toolbar_searchField").value = null;
                            document.getElementById("content_toolbox_searchField").value = null;
                            document.getElementById('content_toolbox_posItem').value = null;
                            document.getElementById('content_toolbox_rgItem').value = null;

                            //closes loading blanket
                            document.getElementById("mapedit_blanket_loading").style.display = "none";

                            //moved here to fix issue where assignment before init
                            MAPEDITOR.GLOBAL.DEFINES.kmlLayer.setOptions({ suppressinfoWindows: true });

                            MAPEDITOR.TRACER.addTracer("[INFO]: initOptions completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    initJqueryElements: function () {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: initJqueryElements started...");
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
                                alert(MAPEDITOR.LOCALIZATION.DEFINES.L51 + ": " + err + " at line " +err.lineNumber );
                                MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                            }
                            try {
                                $("#overlayTransparencySlider").slider({
                                    animate: true,
                                    range: "min",
                                    value: MAPEDITOR.GLOBAL.DEFINES.preservedOpacity,
                                    orientation: "vertical",
                                    min: 0.00,
                                    max: 1.00,
                                    step: 0.01,
                                    slide: function (event, ui) {
                                        if (MAPEDITOR.GLOBAL.DEFINES.currentlyEditing == "yes") {
                                            var selection = $("#overlayTransparencySlider").slider("value");
                                            MAPEDITOR.TRACER.addTracer("[INFO]: opacity selected: " + selection);
                                            MAPEDITOR.UTILITIES.keepOpacity(selection);
                                        }
                                    }
                                });
                                $(".knob").knob({
                                    change: function (value) {
                                        if (MAPEDITOR.GLOBAL.DEFINES.currentlyEditing == "yes") {
                                            MAPEDITOR.GLOBAL.DEFINES.knobRotationValue = value; //assign knob value
                                            if (value > 180) {
                                                MAPEDITOR.GLOBAL.DEFINES.knobRotationValue = ((MAPEDITOR.GLOBAL.DEFINES.knobRotationValue - 360) * (1)); //used to correct for visual effect of knob error
                                                //MAPEDITOR.GLOBAL.DEFINES.knobRotationValue = ((MAPEDITOR.GLOBAL.DEFINES.knobRotationValue-180)*(-1));
                                            }
                                            //only do something if we are in pageEdit Mode and there is an overlay to apply these changes to
                                            if (MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex != null) {
                                                MAPEDITOR.GLOBAL.DEFINES.preservedRotation = MAPEDITOR.GLOBAL.DEFINES.knobRotationValue; //reassign
                                                MAPEDITOR.UTILITIES.keepRotate(MAPEDITOR.GLOBAL.DEFINES.preservedRotation); //send to display fcn of rotation
                                                MAPEDITOR.TRACER.addTracer("[INFO]: setting rotation from knob at wroking index: " + MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex + "to value: " + MAPEDITOR.GLOBAL.DEFINES.preservedRotation);
                                                MAPEDITOR.GLOBAL.DEFINES.savingOverlayRotation[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex - 1] = MAPEDITOR.GLOBAL.DEFINES.preservedRotation; //just make sure it is prepping for save    
                                            }
                                        }
                                    }
                                });
                            } catch (err) {
                                MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                            }
                            MAPEDITOR.TRACER.addTracer("[INFO]: initJqueryElements completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    initOverlayList: function () {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: initOverlayList started...");

                            //reset overlay list
                            document.getElementById("overlayList").innerHTML = "";
                            //determine if there are overlays
                            if (MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPageId.length > 0) {
                                MAPEDITOR.TRACER.addTracer("[INFO]: there are " + MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPageId.length + " pages");
                                //for each, display 
                                for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPageId.length; i++) {
                                    if (MAPEDITOR.GLOBAL.DEFINES.incomingPolygonFeatureType[i] != "poi") {
                                        MAPEDITOR.TRACER.addTracer("[INFO]: Adding Overlay List Item");
                                        document.getElementById("overlayList").innerHTML += MAPEDITOR.UTILITIES.writeHTML("overlayListItem", MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPageId[i], MAPEDITOR.GLOBAL.DEFINES.incomingPolygonLabel[i], "");
                                    }
                                }
                            }

                            MAPEDITOR.TRACER.addTracer("[INFO]: initOverlayList completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    initListOfTextAreaIds: function () {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: initListOfTextAreaIds started...");

                            //assign text area ids
                            MAPEDITOR.GLOBAL.DEFINES.listOfTextAreaIds[0] = "content_menubar_searchField";
                            MAPEDITOR.GLOBAL.DEFINES.listOfTextAreaIds[1] = "content_toolbar_searchField";
                            MAPEDITOR.GLOBAL.DEFINES.listOfTextAreaIds[2] = "content_toolbox_searchField";
                            MAPEDITOR.GLOBAL.DEFINES.listOfTextAreaIds[3] = "poiDesc";

                            MAPEDITOR.TRACER.addTracer("[INFO]: initListOfTextAreaIds completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    initGMap: function () {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: initGMap started...");

                            //get and set the page load time (this is used for the resizer)
                            MAPEDITOR.GLOBAL.DEFINES.pageLoadTime = new Date().getTime();

                            //as map is loading, fit to screen
                            MAPEDITOR.UTILITIES.resizeView();
                            
                            //init gmap objs
                            MAPEDITOR.GLOBAL.initGMapObjects();
                            
                            //initialize google map objects
                            map = new google.maps.Map(document.getElementById("googleMap"), MAPEDITOR.GLOBAL.DEFINES.gmapOptions);
                            //map = new google.maps.Map(document.getElementById(MAPEDITOR.GLOBAL.DEFINES.gmapPageDivId), MAPEDITOR.GLOBAL.DEFINES.gmapOptions);    //initialize map    
                            map.controls[google.maps.ControlPosition.RIGHT_BOTTOM].push(MAPEDITOR.GLOBAL.DEFINES.copyrightNode);                                 //initialize custom copyright
                            map.controls[google.maps.ControlPosition.BOTTOM_LEFT].push(MAPEDITOR.GLOBAL.DEFINES.cursorLatLongTool);                              //initialize cursor lat long tool
                            map.controls[google.maps.ControlPosition.TOP_LEFT].push(MAPEDITOR.GLOBAL.DEFINES.toolbarBufferZone1);                                //initialize spacer
                            map.controls[google.maps.ControlPosition.TOP_RIGHT].push(MAPEDITOR.GLOBAL.DEFINES.toolbarBufferZone2);                               //intialize spacer
                            MAPEDITOR.GLOBAL.DEFINES.drawingManager.setMap(map);                                                                                 //initialize drawing manager
                            MAPEDITOR.GLOBAL.DEFINES.drawingManager.setMap(null);                                                                                //initialize drawing manager (hide)
                            MAPEDITOR.GLOBAL.DEFINES.geocoder = new google.maps.Geocoder();                                                                      //initialize MAPEDITOR.GLOBAL.DEFINES.geocoder
                            
                            //#region Google Specific Listeners  

                            //initialize drawingmanger listeners
                            google.maps.event.addListener(MAPEDITOR.GLOBAL.DEFINES.drawingManager, 'markercomplete', function (marker) {
                                MAPEDITOR.UTILITIES.testBounds(); //are we still in the bounds 
                                //handle if item
                                if (MAPEDITOR.GLOBAL.DEFINES.placerType == "item") {
                                    MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                    MAPEDITOR.ACTIONS.openToolboxTab("item");
                                    MAPEDITOR.GLOBAL.DEFINES.firstSaveItem = true;
                                    //used to prevent multi markers
                                    if (MAPEDITOR.GLOBAL.DEFINES.firstMarker > 0) {
                                        MAPEDITOR.GLOBAL.DEFINES.drawingManager.setDrawingMode(null); //only place one at a time
                                    } else {
                                        MAPEDITOR.GLOBAL.DEFINES.firstMarker++;
                                        MAPEDITOR.GLOBAL.DEFINES.drawingManager.setDrawingMode(null); //only place one at a time
                                    }
                                    MAPEDITOR.GLOBAL.DEFINES.itemMarker = marker; //assign globally
                                    MAPEDITOR.TRACER.addTracer("[INFO]: marker placed");
                                    document.getElementById('content_toolbox_posItem').value = MAPEDITOR.GLOBAL.DEFINES.itemMarker.getPosition();
                                    MAPEDITOR.GLOBAL.DEFINES.savingMarkerCenter = MAPEDITOR.GLOBAL.DEFINES.itemMarker.getPosition(); //store coords to save
                                    MAPEDITOR.UTILITIES.codeLatLng(MAPEDITOR.GLOBAL.DEFINES.itemMarker.getPosition());
                                    google.maps.event.addListener(marker, 'dragend', function () {
                                        MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                        MAPEDITOR.GLOBAL.DEFINES.firstSaveItem = true;
                                        document.getElementById('content_toolbox_posItem').value = marker.getPosition();
                                        MAPEDITOR.UTILITIES.codeLatLng(marker.getPosition());
                                    });
                                }
                                //handle if poi
                                if (MAPEDITOR.GLOBAL.DEFINES.placerType == "poi") {
                                    MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                    MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                    MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                    MAPEDITOR.GLOBAL.DEFINES.poi_i++;

                                    MAPEDITOR.GLOBAL.DEFINES.label[MAPEDITOR.GLOBAL.DEFINES.poi_i] = new MarkerWithLabel({
                                        position: marker.getPosition(), //position of real marker
                                        map: map,
                                        zIndex: 2,
                                        labelContent: MAPEDITOR.GLOBAL.DEFINES.poi_i + 1, //the current user count
                                        labelAnchor: new google.maps.Point(15, 0),
                                        labelClass: "labels", // the CSS class for the label
                                        labelStyle: { opacity: 0.75 },
                                        icon: {} //initialize to nothing so no marker shows
                                    });

                                    MAPEDITOR.GLOBAL.DEFINES.poiObj[MAPEDITOR.GLOBAL.DEFINES.poi_i] = marker;
                                    MAPEDITOR.GLOBAL.DEFINES.poiType[MAPEDITOR.GLOBAL.DEFINES.poi_i] = "marker";
                                    var poiId = MAPEDITOR.GLOBAL.DEFINES.poi_i + 1;
                                    var poiDescTemp = MAPEDITOR.LOCALIZATION.DEFINES.L_Marker;
                                    document.getElementById("poiList").innerHTML += MAPEDITOR.UTILITIES.writeHTML("poiListItem", MAPEDITOR.GLOBAL.DEFINES.poi_i, poiId, poiDescTemp);
                                    var contentString = MAPEDITOR.UTILITIES.writeHTML("poiDesc", MAPEDITOR.GLOBAL.DEFINES.poi_i, "", "");
                                    MAPEDITOR.GLOBAL.DEFINES.infoWindow[MAPEDITOR.GLOBAL.DEFINES.poi_i] = new google.maps.InfoWindow({
                                        content: contentString,
                                        position: marker.getPosition()
                                    });
                                    MAPEDITOR.GLOBAL.DEFINES.infoWindow[MAPEDITOR.GLOBAL.DEFINES.poi_i].setMap(map);
                                    MAPEDITOR.GLOBAL.DEFINES.infoWindow[MAPEDITOR.GLOBAL.DEFINES.poi_i].open(map, MAPEDITOR.GLOBAL.DEFINES.poiObj[MAPEDITOR.GLOBAL.DEFINES.poi_i]);
                                    MAPEDITOR.TRACER.addTracer("[INFO]: poiCount: " + MAPEDITOR.GLOBAL.DEFINES.poiCount);

                                    //best fix so far
                                    if (MAPEDITOR.GLOBAL.DEFINES.poiCount == 0) {
                                        setTimeout(function () {
                                            MAPEDITOR.GLOBAL.DEFINES.infoWindow[0].setMap(null);
                                            MAPEDITOR.GLOBAL.DEFINES.infoWindow[0].setOptions({ pixelOffset: new google.maps.Size(0, -40) });
                                            MAPEDITOR.GLOBAL.DEFINES.infoWindow[0].setMap(map);
                                        }, 800);
                                    }

                                    MAPEDITOR.GLOBAL.DEFINES.poiCount++;

                                    google.maps.event.addListener(marker, 'dragstart', function () {
                                        MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                        MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                        MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                        for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                            if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i] == this) {
                                                MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setMap(null);
                                                MAPEDITOR.GLOBAL.DEFINES.label[i].setMap(null);
                                            }
                                        }

                                    });
                                    google.maps.event.addListener(marker, 'dragend', function () {
                                        MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                        MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                        MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                        for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                            if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i] == this) {
                                                MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setOptions({ position: marker.getPosition(), pixelOffset: new google.maps.Size(0, -40) });
                                                MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].open(null);
                                                MAPEDITOR.GLOBAL.DEFINES.label[i].setPosition(marker.getPosition());
                                                MAPEDITOR.GLOBAL.DEFINES.label[i].setMap(map);
                                            }
                                        }

                                    });
                                    google.maps.event.addListener(marker, 'click', function () {
                                        MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                        MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                        MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                        for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                            if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i] == this) {
                                                MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setOptions({ position: marker.getPosition(), pixelOffset: new google.maps.Size(0, -40) });
                                                MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].open(map);
                                            }
                                        }

                                    });
                                }
                                //regardless of type
                                //set listener for right click (fixes reset issue over overlays)
                                google.maps.event.addListener(marker, 'rightclick', function () {
                                    MAPEDITOR.GLOBAL.DEFINES.drawingManager.setDrawingMode(null); //reset drawing manager no matter what
                                });
                            });
                            google.maps.event.addListener(MAPEDITOR.GLOBAL.DEFINES.drawingManager, 'circlecomplete', function (circle) {
                                MAPEDITOR.UTILITIES.testBounds();
                                //handle if poi
                                if (MAPEDITOR.GLOBAL.DEFINES.placerType == "poi") {
                                    MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                    MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                    MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                    MAPEDITOR.GLOBAL.DEFINES.poi_i++;

                                    MAPEDITOR.GLOBAL.DEFINES.label[MAPEDITOR.GLOBAL.DEFINES.poi_i] = new MarkerWithLabel({
                                        position: circle.getCenter(), //position of real marker
                                        zIndex: 2,
                                        map: map,
                                        labelContent: MAPEDITOR.GLOBAL.DEFINES.poi_i + 1, //the current user count
                                        labelAnchor: new google.maps.Point(15, 0),
                                        labelClass: "labels", // the CSS class for the label
                                        labelStyle: { opacity: 0.75 },
                                        icon: {} //initialize to nothing so no marker shows
                                    });

                                    var poiId = MAPEDITOR.GLOBAL.DEFINES.poi_i + 1;
                                    MAPEDITOR.GLOBAL.DEFINES.poiObj[MAPEDITOR.GLOBAL.DEFINES.poi_i] = circle;
                                    MAPEDITOR.GLOBAL.DEFINES.poiType[MAPEDITOR.GLOBAL.DEFINES.poi_i] = "circle";
                                    var poiDescTemp = MAPEDITOR.LOCALIZATION.DEFINES.L_Circle;
                                    document.getElementById("poiList").innerHTML += MAPEDITOR.UTILITIES.writeHTML("poiListItem", MAPEDITOR.GLOBAL.DEFINES.poi_i, poiId, poiDescTemp);
                                    var contentString = MAPEDITOR.UTILITIES.writeHTML("poiDesc", MAPEDITOR.GLOBAL.DEFINES.poi_i, "", "");
                                    MAPEDITOR.GLOBAL.DEFINES.infoWindow[MAPEDITOR.GLOBAL.DEFINES.poi_i] = new google.maps.InfoWindow({
                                        content: contentString
                                    });
                                    MAPEDITOR.GLOBAL.DEFINES.infoWindow[MAPEDITOR.GLOBAL.DEFINES.poi_i].setPosition(circle.getCenter());
                                    MAPEDITOR.GLOBAL.DEFINES.infoWindow[MAPEDITOR.GLOBAL.DEFINES.poi_i].open(map);
                                    //best fix so far
                                    if (MAPEDITOR.GLOBAL.DEFINES.poiCount == 0) {
                                        setTimeout(function () {
                                            MAPEDITOR.GLOBAL.DEFINES.infoWindow[0].setMap(null);
                                            MAPEDITOR.GLOBAL.DEFINES.infoWindow[0].setMap(map);
                                        }, 800);
                                    }
                                    MAPEDITOR.GLOBAL.DEFINES.poiCount++;

                                    google.maps.event.addListener(circle, 'dragstart', function () {
                                        MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                        MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                        MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                        for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                            if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i] == this) {
                                                MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setMap(null);
                                                MAPEDITOR.GLOBAL.DEFINES.label[i].setMap(null);
                                            }
                                        }

                                    });
                                    google.maps.event.addListener(circle, 'dragend', function () {
                                        MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                        MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                        MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                        for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                            if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i] == this) {
                                                MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setPosition(circle.getCenter());
                                                MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].open(null);
                                                MAPEDITOR.GLOBAL.DEFINES.label[i].setPosition(circle.getCenter());
                                                MAPEDITOR.GLOBAL.DEFINES.label[i].setMap(map);
                                            }
                                        }

                                    });
                                    google.maps.event.addListener(circle, 'click', function () {
                                        MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                        MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                        MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                        for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                            if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i] == this) {
                                                MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setPosition(circle.getCenter());
                                                MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].open(map);
                                            }
                                        }

                                    });
                                    google.maps.event.addListener(circle, 'radius_changed', function () {
                                        MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                        MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                        MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                        for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                            if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i] == this) {
                                                MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setPosition(circle.getCenter());
                                                MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].open(null);
                                                MAPEDITOR.GLOBAL.DEFINES.label[i].setPosition(circle.getCenter());
                                                MAPEDITOR.GLOBAL.DEFINES.label[i].setMap(map);
                                            }
                                        }

                                    });
                                    google.maps.event.addListener(circle, 'center_changed', function () {
                                        MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                        MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                        MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                        for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                            if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i] == this) {
                                                MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setPosition(circle.getCenter());
                                                MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].open(null);
                                                MAPEDITOR.GLOBAL.DEFINES.label[i].setPosition(circle.getCenter());
                                                MAPEDITOR.GLOBAL.DEFINES.label[i].setMap(map);
                                            }
                                        }
                                    });
                                }
                                //regardless of type
                                //used for latlong tool
                                google.maps.event.addListener(circle, 'drag', function () {
                                    //used to get the center point for lat/long tool
                                    MAPEDITOR.GLOBAL.DEFINES.circleCenter = circle.getCenter();
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
                                    MAPEDITOR.GLOBAL.DEFINES.drawingManager.setDrawingMode(null); //reset drawing manager no matter what
                                });

                            });
                            google.maps.event.addListener(MAPEDITOR.GLOBAL.DEFINES.drawingManager, 'rectanglecomplete', function (rectangle) {
                                //check the bounds to make sure you havent strayed too far away
                                MAPEDITOR.UTILITIES.testBounds();
                                //handle if an overlay
                                if (MAPEDITOR.GLOBAL.DEFINES.placerType == "overlay") {
                                    MAPEDITOR.ACTIONS.openToolboxTab("overlay");
                                    MAPEDITOR.TRACER.addTracer("[INFO]: placertype: overlay");
                                    //assing working overlay index
                                    MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex = MAPEDITOR.GLOBAL.DEFINES.convertedOverlayIndex + 1;
                                    if (MAPEDITOR.GLOBAL.DEFINES.overlayType == "drawn") {
                                        MAPEDITOR.TRACER.addTracer("[INFO]: MAPEDITOR.GLOBAL.DEFINES.overlayType: " + MAPEDITOR.GLOBAL.DEFINES.overlayType);
                                        MAPEDITOR.TRACER.addTracer("[INFO]: MAPEDITOR.GLOBAL.DEFINES.convertedOverlayIndex: " + MAPEDITOR.GLOBAL.DEFINES.convertedOverlayIndex);
                                        MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPath[MAPEDITOR.GLOBAL.DEFINES.convertedOverlayIndex] = rectangle.getBounds();
                                        //create overlay with incoming
                                        MAPEDITOR.GLOBAL.DEFINES.overlaysOnMap[MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPageId[MAPEDITOR.GLOBAL.DEFINES.convertedOverlayIndex]] = new MAPEDITOR.UTILITIES.CustomOverlay(MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPageId[MAPEDITOR.GLOBAL.DEFINES.convertedOverlayIndex], MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPath[MAPEDITOR.GLOBAL.DEFINES.convertedOverlayIndex], MAPEDITOR.GLOBAL.DEFINES.incomingPolygonSourceURL[MAPEDITOR.GLOBAL.DEFINES.convertedOverlayIndex], map, MAPEDITOR.GLOBAL.DEFINES.incomingPolygonRotation[MAPEDITOR.GLOBAL.DEFINES.convertedOverlayIndex]);
                                        MAPEDITOR.GLOBAL.DEFINES.currentlyEditing = "no";
                                        //set the overlay to the map
                                        MAPEDITOR.GLOBAL.DEFINES.overlaysOnMap[MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPageId[MAPEDITOR.GLOBAL.DEFINES.convertedOverlayIndex]].setMap(map);
                                        //set hotspot on top of overlay
                                        MAPEDITOR.UTILITIES.setGhostOverlay(MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPageId[MAPEDITOR.GLOBAL.DEFINES.convertedOverlayIndex], MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPath[MAPEDITOR.GLOBAL.DEFINES.convertedOverlayIndex]);
                                        MAPEDITOR.TRACER.addTracer("[INFO]: I created ghost: " + MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPageId[MAPEDITOR.GLOBAL.DEFINES.convertedOverlayIndex]);
                                        MAPEDITOR.GLOBAL.DEFINES.mainCount++;
                                        MAPEDITOR.GLOBAL.DEFINES.incomingACL = "overlay";
                                        //mark that we converted it
                                        MAPEDITOR.GLOBAL.DEFINES.isConvertedOverlay = true;
                                        //hide the rectangle we drew
                                        rectangle.setMap(null);
                                        //relist the overlay we drew
                                        MAPEDITOR.GLOBAL.initOverlayList();
                                        MAPEDITOR.ACTIONS.overlayEditMe(MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPageId[MAPEDITOR.GLOBAL.DEFINES.convertedOverlayIndex]);
                                        //reset drawing manager no matter what
                                        MAPEDITOR.GLOBAL.DEFINES.drawingManager.setDrawingMode(null);
                                    } else {
                                        //mark that we are editing
                                        MAPEDITOR.GLOBAL.DEFINES.firstSaveOverlay = true;
                                        //add the incoming overlay bounds
                                        MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPath[MAPEDITOR.GLOBAL.DEFINES.convertedOverlayIndex] = rectangle.getBounds();
                                        //redisplay overlays (the one we just made)
                                        MAPEDITOR.GLOBAL.displayIncomingPolygons();
                                        //relist the overlay we drew
                                        MAPEDITOR.GLOBAL.initOverlayList();
                                        //hide the rectangle we drew
                                        rectangle.setMap(null);
                                        //prevent redraw
                                        MAPEDITOR.GLOBAL.DEFINES.drawingManager.setDrawingMode(null);
                                        //MAPEDITOR.GLOBAL.DEFINES.drawingManager.setMap(null);
                                    }
                                    //create cache save overlay for the new converted overlay only
                                    if (MAPEDITOR.GLOBAL.DEFINES.isConvertedOverlay == true) {
                                        MAPEDITOR.UTILITIES.cacheSaveOverlay(MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex);
                                        MAPEDITOR.GLOBAL.DEFINES.isConvertedOverlay = false;
                                    }
                                }
                                //handle if poi
                                if (MAPEDITOR.GLOBAL.DEFINES.placerType == "poi") {
                                    MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                    MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                    MAPEDITOR.GLOBAL.DEFINES.poi_i++;

                                    MAPEDITOR.GLOBAL.DEFINES.label[MAPEDITOR.GLOBAL.DEFINES.poi_i] = new MarkerWithLabel({
                                        position: rectangle.getBounds().getCenter(), //position of real marker
                                        zIndex: 2,
                                        map: map,
                                        labelContent: MAPEDITOR.GLOBAL.DEFINES.poi_i + 1, //the current user count
                                        labelAnchor: new google.maps.Point(15, 0),
                                        labelClass: "labels", // the CSS class for the label
                                        labelStyle: { opacity: 0.75 },
                                        icon: {} //initialize to nothing so no marker shows
                                    });

                                    var poiId = MAPEDITOR.GLOBAL.DEFINES.poi_i + 1;
                                    MAPEDITOR.GLOBAL.DEFINES.poiObj[MAPEDITOR.GLOBAL.DEFINES.poi_i] = rectangle;
                                    MAPEDITOR.GLOBAL.DEFINES.poiType[MAPEDITOR.GLOBAL.DEFINES.poi_i] = "rectangle";
                                    var poiDescTemp = MAPEDITOR.LOCALIZATION.DEFINES.L_Rectangle;
                                    document.getElementById("poiList").innerHTML += MAPEDITOR.UTILITIES.writeHTML("poiListItem", MAPEDITOR.GLOBAL.DEFINES.poi_i, poiId, poiDescTemp);
                                    var contentString = MAPEDITOR.UTILITIES.writeHTML("poiDesc", MAPEDITOR.GLOBAL.DEFINES.poi_i, "", "");
                                    MAPEDITOR.GLOBAL.DEFINES.infoWindow[MAPEDITOR.GLOBAL.DEFINES.poi_i] = new google.maps.InfoWindow({
                                        content: contentString
                                    });
                                    MAPEDITOR.GLOBAL.DEFINES.infoWindow[MAPEDITOR.GLOBAL.DEFINES.poi_i].setPosition(rectangle.getBounds().getCenter());
                                    MAPEDITOR.GLOBAL.DEFINES.infoWindow[MAPEDITOR.GLOBAL.DEFINES.poi_i].open(map);
                                    //best fix so far
                                    if (MAPEDITOR.GLOBAL.DEFINES.poiCount == 0) {
                                        setTimeout(function () {
                                            MAPEDITOR.GLOBAL.DEFINES.infoWindow[0].setMap(null);
                                            MAPEDITOR.GLOBAL.DEFINES.infoWindow[0].setMap(map);
                                        }, 800);
                                    }
                                    MAPEDITOR.GLOBAL.DEFINES.poiCount++;
                                    MAPEDITOR.TRACER.addTracer("[INFO]: completed overlay bounds getter");

                                    google.maps.event.addListener(rectangle, 'bounds_changed', function () {
                                        MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                        MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                        MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                        for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                            if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i] == this) {
                                                MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setPosition(rectangle.getBounds().getCenter());
                                                MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setMap(null);
                                                MAPEDITOR.GLOBAL.DEFINES.label[i].setPosition(rectangle.getBounds().getCenter());
                                                MAPEDITOR.GLOBAL.DEFINES.label[i].setMap(map);
                                            }
                                        }

                                    });
                                    google.maps.event.addListener(rectangle, 'dragstart', function () {
                                        MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                        MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                        MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                        for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                            if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i] == this) {
                                                MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setMap(null);
                                                MAPEDITOR.GLOBAL.DEFINES.label[i].setMap(null);
                                            }
                                        }

                                    });
                                    google.maps.event.addListener(rectangle, 'drag', function () {
                                        MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                        MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                        MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                        for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                            if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i] == this) {
                                                MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setMap(null);
                                                MAPEDITOR.GLOBAL.DEFINES.label[i].setMap(null);
                                            }
                                        }


                                    });
                                    google.maps.event.addListener(rectangle, 'dragend', function () {
                                        MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                        MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                        MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                        for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                            if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i] == this) {
                                                MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setPosition(rectangle.getBounds().getCenter());
                                                MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].open(null);
                                                MAPEDITOR.GLOBAL.DEFINES.label[i].setPosition(rectangle.getBounds().getCenter());
                                                MAPEDITOR.GLOBAL.DEFINES.label[i].setMap(map);
                                            }
                                        }

                                    });
                                    google.maps.event.addListener(rectangle, 'click', function () {
                                        MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                        MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                        MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                        for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                            if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i] == this) {
                                                MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setPosition(rectangle.getBounds().getCenter());
                                                MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].open(map);
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
                                    MAPEDITOR.GLOBAL.DEFINES.drawingManager.setDrawingMode(null); //reset drawing manager no matter what
                                });
                            });
                            google.maps.event.addListener(MAPEDITOR.GLOBAL.DEFINES.drawingManager, 'polygoncomplete', function (polygon) {
                                MAPEDITOR.UTILITIES.testBounds();
                                //handle if poi
                                if (MAPEDITOR.GLOBAL.DEFINES.placerType == "poi") {
                                    MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                    MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                    MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                    MAPEDITOR.GLOBAL.DEFINES.poi_i++;

                                    MAPEDITOR.GLOBAL.DEFINES.label[MAPEDITOR.GLOBAL.DEFINES.poi_i] = new MarkerWithLabel({
                                        position: MAPEDITOR.UTILITIES.polygonCenter(polygon), //position of real marker
                                        zIndex: 2,
                                        map: map,
                                        labelContent: MAPEDITOR.GLOBAL.DEFINES.poi_i + 1, //the current user count
                                        labelAnchor: new google.maps.Point(15, 0),
                                        labelClass: "labels", // the CSS class for the label
                                        labelStyle: { opacity: 0.75 },
                                        icon: {} //initialize to nothing so no marker shows
                                    });

                                    var poiId = MAPEDITOR.GLOBAL.DEFINES.poi_i + 1;
                                    MAPEDITOR.GLOBAL.DEFINES.poiObj[MAPEDITOR.GLOBAL.DEFINES.poi_i] = polygon;
                                    MAPEDITOR.GLOBAL.DEFINES.poiType[MAPEDITOR.GLOBAL.DEFINES.poi_i] = "polygon";
                                    var poiDescTemp = MAPEDITOR.LOCALIZATION.DEFINES.L_Polygon;
                                    document.getElementById("poiList").innerHTML += MAPEDITOR.UTILITIES.writeHTML("poiListItem", MAPEDITOR.GLOBAL.DEFINES.poi_i, poiId, poiDescTemp);
                                    var contentString = MAPEDITOR.UTILITIES.writeHTML("poiDesc", MAPEDITOR.GLOBAL.DEFINES.poi_i, "", "");
                                    MAPEDITOR.GLOBAL.DEFINES.infoWindow[MAPEDITOR.GLOBAL.DEFINES.poi_i] = new google.maps.InfoWindow({
                                        content: contentString
                                    });
                                    MAPEDITOR.GLOBAL.DEFINES.infoWindow[MAPEDITOR.GLOBAL.DEFINES.poi_i].setPosition(MAPEDITOR.UTILITIES.polygonCenter(polygon));
                                    MAPEDITOR.GLOBAL.DEFINES.infoWindow[MAPEDITOR.GLOBAL.DEFINES.poi_i].open(map);
                                    //best fix so far
                                    if (MAPEDITOR.GLOBAL.DEFINES.poiCount == 0) {
                                        setTimeout(function () {
                                            MAPEDITOR.GLOBAL.DEFINES.infoWindow[0].setMap(null);
                                            MAPEDITOR.GLOBAL.DEFINES.infoWindow[0].setMap(map);
                                        }, 800);
                                    }
                                    MAPEDITOR.GLOBAL.DEFINES.poiCount++;

                                    google.maps.event.addListener(polygon.getPath(), 'set_at', function () { //if bounds change
                                        MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                        MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                        MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                        for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                            if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i] == this) {
                                                MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setPosition(MAPEDITOR.UTILITIES.polygonCenter(polygon));
                                                MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setMap(null);
                                                MAPEDITOR.GLOBAL.DEFINES.label[i].setPosition(MAPEDITOR.UTILITIES.polygonCenter(polygon));
                                                MAPEDITOR.GLOBAL.DEFINES.label[i].setMap(map); //does not redisplay
                                            }
                                        }
                                    });
                                    google.maps.event.addListener(polygon, 'dragstart', function () {
                                        MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                        MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                        MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                        for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                            if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i] == this) {
                                                MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setMap(null);
                                                MAPEDITOR.GLOBAL.DEFINES.label[i].setMap(null);
                                            }
                                        }

                                    });
                                    google.maps.event.addListener(polygon, 'drag', function () {
                                        MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                        MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                        MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                        for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                            if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i] == this) {
                                                MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setMap(null);
                                                MAPEDITOR.GLOBAL.DEFINES.label[i].setMap(null);
                                            }
                                        }

                                    });
                                    google.maps.event.addListener(polygon, 'dragend', function () {
                                        MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                        MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                        MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                        for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                            if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i] == this) {
                                                MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setPosition(MAPEDITOR.UTILITIES.polygonCenter(polygon));
                                                MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].open(null);
                                                MAPEDITOR.GLOBAL.DEFINES.label[i].setPosition(MAPEDITOR.UTILITIES.polygonCenter(polygon));
                                                MAPEDITOR.GLOBAL.DEFINES.label[i].setMap(map);
                                            }
                                        }

                                    });
                                    google.maps.event.addListener(polygon, 'click', function () {
                                        MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                        MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                        MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                        for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                            if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i] == this) {
                                                MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setPosition(MAPEDITOR.UTILITIES.polygonCenter(polygon));
                                                MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].open(map);
                                            }
                                        }

                                    });
                                }
                                //used for lat/long tool regardless if poi or not
                                google.maps.event.addListener(polygon, 'drag', function () {

                                    var str = MAPEDITOR.UTILITIES.polygonCenter(polygon).toString();
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
                                    MAPEDITOR.GLOBAL.DEFINES.drawingManager.setDrawingMode(null); //reset drawing manager no matter what
                                });
                            });
                            google.maps.event.addListener(MAPEDITOR.GLOBAL.DEFINES.drawingManager, 'polylinecomplete', function (polyline) {
                                //make sure we are still in the bounds
                                MAPEDITOR.UTILITIES.testBounds();
                                //handle if this is a polygon
                                if (MAPEDITOR.GLOBAL.DEFINES.placerType == "poi") {
                                    MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                    MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                    MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                    MAPEDITOR.GLOBAL.DEFINES.poi_i++;
                                    var poiId = MAPEDITOR.GLOBAL.DEFINES.poi_i + 1;
                                    MAPEDITOR.GLOBAL.DEFINES.poiObj[MAPEDITOR.GLOBAL.DEFINES.poi_i] = polyline;
                                    MAPEDITOR.GLOBAL.DEFINES.poiType[MAPEDITOR.GLOBAL.DEFINES.poi_i] = "polyline";
                                    var poiDescTemp = MAPEDITOR.LOCALIZATION.DEFINES.L_Line;
                                    document.getElementById("poiList").innerHTML += MAPEDITOR.UTILITIES.writeHTML("poiListItem", MAPEDITOR.GLOBAL.DEFINES.poi_i, poiId, poiDescTemp);
                                    var contentString = MAPEDITOR.UTILITIES.writeHTML("poiDesc", MAPEDITOR.GLOBAL.DEFINES.poi_i, "", "");
                                    MAPEDITOR.GLOBAL.DEFINES.infoWindow[MAPEDITOR.GLOBAL.DEFINES.poi_i] = new google.maps.InfoWindow({ content: contentString });
                                    var polylinePoints = [];
                                    var polylinePointCount = 0;
                                    polyline.getPath().forEach(function (latLng) {
                                        polylinePoints[polylinePointCount] = latLng;
                                        MAPEDITOR.TRACER.addTracer("[INFO]: polylinePoints[" + polylinePointCount + "] = " + latLng);
                                        polylinePointCount++;
                                    });
                                    MAPEDITOR.TRACER.addTracer("[INFO]: polylinePointCount: " + polylinePointCount);
                                    MAPEDITOR.TRACER.addTracer("[INFO]: polylinePoints.length: " + polylinePoints.length);
                                    MAPEDITOR.TRACER.addTracer("[INFO]: Math.round((polylinePoints.length / 2)): " + Math.round((polylinePoints.length / 2)));
                                    var polylineCenterPoint = polylinePoints[Math.round((polylinePoints.length / 2))];
                                    MAPEDITOR.TRACER.addTracer("[INFO]: polylineCenterPoint: " + polylineCenterPoint);
                                    var polylineStartPoint = polylinePoints[0];
                                    MAPEDITOR.TRACER.addTracer("[INFO]: polylineStartPoint: " + polylineStartPoint);
                                    MAPEDITOR.GLOBAL.DEFINES.infoWindow[MAPEDITOR.GLOBAL.DEFINES.poi_i].setPosition(polylineStartPoint);
                                    MAPEDITOR.GLOBAL.DEFINES.infoWindow[MAPEDITOR.GLOBAL.DEFINES.poi_i].open(map);

                                    //best fix so far
                                    if (MAPEDITOR.GLOBAL.DEFINES.poiCount == 0) {
                                        setTimeout(function () {
                                            MAPEDITOR.GLOBAL.DEFINES.infoWindow[0].setMap(null);
                                            MAPEDITOR.GLOBAL.DEFINES.infoWindow[0].setMap(map);
                                        }, 800);
                                    }
                                    MAPEDITOR.GLOBAL.DEFINES.poiCount++;
                                    //create the label
                                    MAPEDITOR.GLOBAL.DEFINES.label[MAPEDITOR.GLOBAL.DEFINES.poi_i] = new MarkerWithLabel({
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
                                        MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                        MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                        MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                        MAPEDITOR.TRACER.addTracer("[INFO]: is poi");
                                        for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                            MAPEDITOR.TRACER.addTracer("[INFO]: inside loop1");
                                            if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i].getPath() == this) {
                                                var polylinePoints = [];
                                                var polylinePointCount = 0;
                                                MAPEDITOR.TRACER.addTracer("[INFO]: here1");
                                                polyline.getPath().forEach(function (latLng) {
                                                    polylinePoints[polylinePointCount] = latLng;
                                                    polylinePointCount++;
                                                });
                                                MAPEDITOR.TRACER.addTracer("[INFO]: here2");
                                                var polylineCenterPoint = polylinePoints[(polylinePoints.length / 2)];
                                                var polylineStartPoint = polylinePoints[0];
                                                MAPEDITOR.GLOBAL.DEFINES.infoWindow[MAPEDITOR.GLOBAL.DEFINES.poi_i].setPosition(polylineStartPoint);
                                                MAPEDITOR.GLOBAL.DEFINES.infoWindow[MAPEDITOR.GLOBAL.DEFINES.poi_i].open(null);
                                                MAPEDITOR.GLOBAL.DEFINES.label[i].setPosition(polylineStartPoint);
                                                MAPEDITOR.GLOBAL.DEFINES.label[i].setMap(map);
                                                MAPEDITOR.TRACER.addTracer("[INFO]: here3");
                                            }
                                        }
                                    });
                                    google.maps.event.addListener(polyline, 'dragstart', function () {
                                        MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                        MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                        MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                        for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                            if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i] == this) {
                                                MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setMap(null);
                                                MAPEDITOR.GLOBAL.DEFINES.label[i].setMap(null);
                                            }
                                        }
                                    });
                                    google.maps.event.addListener(polyline, 'click', function () {
                                        MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                        MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                        MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                        for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                            if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i] == this) {
                                                var polylinePoints = [];
                                                var polylinePointCount = 0;
                                                polyline.getPath().forEach(function (latLng) {
                                                    polylinePoints[polylinePointCount] = latLng;
                                                    polylinePointCount++;
                                                });
                                                var polylineCenterPoint = polylinePoints[(polylinePoints.length / 2)];
                                                var polylineStartPoint = polylinePoints[0];
                                                MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setPosition(polylineStartPoint);
                                                MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].open(map);
                                            }
                                        }
                                    });
                                    google.maps.event.addListener(polyline, 'dragend', function () {
                                        MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                        MAPEDITOR.ACTIONS.openToolboxTab("poi");
                                        MAPEDITOR.GLOBAL.DEFINES.firstSavePOI = true;
                                        for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                            if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i] == this) {
                                                var polylinePoints = [];
                                                var polylinePointCount = 0;
                                                polyline.getPath().forEach(function (latLng) {
                                                    polylinePoints[polylinePointCount] = latLng;
                                                    polylinePointCount++;
                                                });
                                                var polylineCenterPoint = polylinePoints[(polylinePoints.length / 2)];
                                                var polylineStartPoint = polylinePoints[0];
                                                MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setPosition(polylineStartPoint);
                                                MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].open(null);
                                                MAPEDITOR.GLOBAL.DEFINES.label[i].setPosition(polylineStartPoint);
                                                MAPEDITOR.GLOBAL.DEFINES.label[i].setMap(map);
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
                                    MAPEDITOR.GLOBAL.DEFINES.drawingManager.setDrawingMode(null); //reset drawing manager no matter what
                                    //MAPEDITOR.GLOBAL.DEFINES.drawingManager.setMap(null);
                                });
                            });
                            
                            //initialize map specific listeners
                            //on map click, set focus (fixes keycode issue)
                            google.maps.event.addListener(map, 'click', function () {
                                document.activeElement.blur();
                            });
                            //on right click stop drawing thing
                            google.maps.event.addListener(map, 'rightclick', function () {
                                MAPEDITOR.GLOBAL.DEFINES.drawingManager.setDrawingMode(null); //reset drawing manager no matter what
                            });
                            //used to display cursor location via lat/long
                            google.maps.event.addDomListener(map, 'mousemove', function (point) {

                                if (MAPEDITOR.GLOBAL.DEFINES.cCoordsFrozen == "no") {
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
                                MAPEDITOR.UTILITIES.testBounds();
                            });
                            //check the zoom level display message if out limits
                            google.maps.event.addListener(map, 'zoom_changed', function () {
                                MAPEDITOR.UTILITIES.checkZoomLevel();
                            });
                            //when kml layer is clicked, get feature that was clicked
                            google.maps.event.addListener(MAPEDITOR.GLOBAL.DEFINES.kmlLayer, 'click', function (kmlEvent) {
                                var name = kmlEvent.featureData.name;
                                MAPEDITOR.UTILITIES.displayMessage("ParcelID: " + name); //temp
                            });

                            //#endregion

                            //initialize all the incoming geo obejects (the fcn is written via c#)
                            initGeoObjects();

                            //this part runs when the mapobject is created and rendered
                            google.maps.event.addListenerOnce(map, 'tilesloaded', function () {
                                MAPEDITOR.GLOBAL.initOptions(); //setup the graphical user interface (enhances visual effect to do all of this after map loads)
                                MAPEDITOR.GLOBAL.initOverlayList(); //list all the overlays in the list box"
                                MAPEDITOR.GLOBAL.initNonStaticVars(); //these must be loaded after everything else is completed
                                MAPEDITOR.UTILITIES.resizeView(); //explicitly call resizer, fixes unknow issue where timeout of divs being added when there are a lot (IE many overlays)
                            });

                            MAPEDITOR.TRACER.addTracer("[INFO]: initGMap completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    initGMapObjects: function () {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: MAPEDITOR.GLOBAL.initGMapObjects started...");

                            //adds listener support for custom overlay
                            //Supporting URL: https://developers.google.com/maps/documentation/javascript/overlays#CustomOverlays
                            // Note: an overlay's receipt of onAdd() indicates that the map's panes are now available for attaching the overlay to the map via the DOM.
                            MAPEDITOR.UTILITIES.CustomOverlay.prototype.onAdd = function () {

                                // Create the DIV and set some basic attributes.
                                var div = document.createElement("div");
                                div.id = "overlay" + this.index_;
                                div.style.borderStyle = 'none';
                                div.style.borderWidth = '0px';
                                div.style.position = 'absolute';
                                div.style.opacity = MAPEDITOR.GLOBAL.DEFINES.preservedOpacity;

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
                            MAPEDITOR.UTILITIES.CustomOverlay.prototype.draw = function () {
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
                                var temp = MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex;
                                //set woi to incoming (fixes keepRotate error)
                                MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex = this.index_;
                                //check to see if saving rotaiotn and then incoming (this allows all overlays to have a rotation but places priority to the saving overlay rotation)
                                if (MAPEDITOR.GLOBAL.DEFINES.savingOverlayRotation[this.index_ - 1] != undefined) {
                                    MAPEDITOR.UTILITIES.keepRotate(MAPEDITOR.GLOBAL.DEFINES.savingOverlayRotation[this.index_ - 1]);
                                } else {
                                    MAPEDITOR.UTILITIES.keepRotate(MAPEDITOR.GLOBAL.DEFINES.incomingPolygonRotation[(this.index_ - 1)]);
                                }
                                //reset woi to temp just in case we had something different/useful
                                MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex = temp;
                            };
                            //Not currently used
                            //Supporting URL: https://developers.google.com/maps/documentation/javascript/overlays#CustomOverlays
                            MAPEDITOR.UTILITIES.CustomOverlay.prototype.onRemove = function () {
                                this.div_.parentNode.removeChild(this.div_);
                                this.div_ = null;
                            };
                            //holds all the cusotm map options
                            MAPEDITOR.GLOBAL.DEFINES.gmapOptions = {
                                disableDefaultUI: false,                                    //set to false to start from a clean slate of map controls
                                zoom: MAPEDITOR.GLOBAL.DEFINES.defaultZoomLevel,                                     //starting zoom level
                                minZoom: MAPEDITOR.GLOBAL.DEFINES.maxZoomLevel,                                      //highest zoom out level
                                center: MAPEDITOR.GLOBAL.DEFINES.mapCenter,                                          //default center point
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
                            MAPEDITOR.GLOBAL.DEFINES.drawingManager.setOptions({
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
                                markerOptions: MAPEDITOR.GLOBAL.DEFINES.markerOptionsDefault,
                                circleOptions: MAPEDITOR.GLOBAL.DEFINES.circleOptionsDefault,
                                polygonOptions: MAPEDITOR.GLOBAL.DEFINES.polygonOptionsDefault,
                                polylineOptions: MAPEDITOR.GLOBAL.DEFINES.polylineOptionsDefault,
                                rectangleOptions: MAPEDITOR.GLOBAL.DEFINES.rectangleOptionsDefault
                            });
                            //define custom copyright control
                            //supporting url: https://developers.google.com/maps/documentation/javascript/controls#CustomControls
                            MAPEDITOR.GLOBAL.DEFINES.copyrightNode = document.createElement('div');
                            MAPEDITOR.GLOBAL.DEFINES.copyrightNode.id = 'copyright-control';
                            MAPEDITOR.GLOBAL.DEFINES.copyrightNode.style.fontSize = '10px';
                            MAPEDITOR.GLOBAL.DEFINES.copyrightNode.style.color = '#333333';
                            MAPEDITOR.GLOBAL.DEFINES.copyrightNode.style.fontFamily = 'Arial, sans-serif';
                            MAPEDITOR.GLOBAL.DEFINES.copyrightNode.style.margin = '0 2px 2px 0';
                            MAPEDITOR.GLOBAL.DEFINES.copyrightNode.style.whiteSpace = 'nowrap';
                            MAPEDITOR.GLOBAL.DEFINES.copyrightNode.index = 0;
                            MAPEDITOR.GLOBAL.DEFINES.copyrightNode.style.backgroundColor = '#FFFFFF';
                            MAPEDITOR.GLOBAL.DEFINES.copyrightNode.style.opacity = 0.71;
                            MAPEDITOR.GLOBAL.DEFINES.copyrightNode.innerHTML = MAPEDITOR.LOCALIZATION.DEFINES.L1; //localization copyright
                            //define cursor lat long tool custom control
                            //supporting url: https://developers.google.com/maps/documentation/javascript/controls#CustomControls
                            MAPEDITOR.GLOBAL.DEFINES.cursorLatLongTool = document.createElement('div');
                            MAPEDITOR.GLOBAL.DEFINES.cursorLatLongTool.id = 'cursorLatLongTool';
                            MAPEDITOR.GLOBAL.DEFINES.cursorLatLongTool.style.fontSize = '10px';
                            MAPEDITOR.GLOBAL.DEFINES.cursorLatLongTool.style.color = '#333333';
                            MAPEDITOR.GLOBAL.DEFINES.cursorLatLongTool.style.fontFamily = 'Arial, sans-serif';
                            MAPEDITOR.GLOBAL.DEFINES.cursorLatLongTool.style.margin = '0 2px 2px 0';
                            MAPEDITOR.GLOBAL.DEFINES.cursorLatLongTool.style.whiteSpace = 'nowrap';
                            MAPEDITOR.GLOBAL.DEFINES.cursorLatLongTool.index = 0;
                            MAPEDITOR.GLOBAL.DEFINES.cursorLatLongTool.style.backgroundColor = '#FFFFFF';
                            MAPEDITOR.GLOBAL.DEFINES.cursorLatLongTool.style.opacity = 0.71;
                            MAPEDITOR.GLOBAL.DEFINES.cursorLatLongTool.innerHTML = MAPEDITOR.LOCALIZATION.DEFINES.L2; //localization cursor lat/long tool
                            //buffer zone top left (used to push map controls down)
                            //supporting url: https://developers.google.com/maps/documentation/javascript/controls#CustomControls
                            MAPEDITOR.GLOBAL.DEFINES.toolbarBufferZone1 = document.createElement('div');
                            MAPEDITOR.GLOBAL.DEFINES.toolbarBufferZone1.id = 'toolbarBufferZone1';
                            MAPEDITOR.GLOBAL.DEFINES.toolbarBufferZone1.style.height = '50px';
                            //buffer zone top right
                            //supporting url: https://developers.google.com/maps/documentation/javascript/controls#CustomControls
                            MAPEDITOR.GLOBAL.DEFINES.toolbarBufferZone2 = document.createElement('div');
                            MAPEDITOR.GLOBAL.DEFINES.toolbarBufferZone2.id = 'toolbarBufferZone2';
                            MAPEDITOR.GLOBAL.DEFINES.toolbarBufferZone2.style.height = '50px';

                            MAPEDITOR.TRACER.addTracer("[INFO]: MAPEDITOR.GLOBAL.initGMapObjects completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    initNonStaticVars: function () {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: initNonStaticVars started...");
                            //explicityl define nonstatic vars
                            document.getElementById("debugVersionNumber").innerHTML = MAPEDITOR.TRACER.DEFINES.debugVersionNumber;
                            MAPEDITOR.TRACER.addTracer("[INFO]: initNonStaticVars completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
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
                            
                            MAPEDITOR.TRACER.addTracer("[INFO]: localizeByTooltips started...");
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
                            MAPEDITOR.TRACER.addTracer("[INFO]: localizeByTooltips completed...");
                        } catch (err) {
                            alert(MAPEDITOR.LOCALIZATION.DEFINES.L28 + ": " + err + " at line " +err.lineNumber );
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    localizeByText: function () {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: localizeByText started...");
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
                            MAPEDITOR.TRACER.addTracer("[INFO]: localizeByText completed...");
                        } catch (err) {
                            alert(MAPEDITOR.LOCALIZATION.DEFINES.L29);
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    }
                };
            }(),    //localization support
            ACTIONS: function () {
                return {
                    resetAll: function () {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: resetAll started...");
                            document.location.reload(true);
                            MAPEDITOR.TRACER.addTracer("[INFO]: resetAll completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    toggleVis: function (id) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: toggleVis started...");
                            switch (id) {
                                case "mapControls":
                                    if (MAPEDITOR.GLOBAL.DEFINES.mapControlsDisplayed == true) { //present, hide
                                        map.setOptions({
                                            zoomControl: false,
                                            panControl: false,
                                            mapTypeControl: false
                                        });
                                        MAPEDITOR.GLOBAL.DEFINES.mapControlsDisplayed = false;
                                    } else { //not present, make present
                                        map.setOptions({
                                            zoomControl: true,
                                            zoomControlOptions: { style: google.maps.ZoomControlStyle.SMALL, position: google.maps.ControlPosition.LEFT_TOP },
                                            panControl: true,
                                            panControlOptions: { position: google.maps.ControlPosition.LEFT_TOP },
                                            mapTypeControl: true,
                                            mapTypeControlOptions: { style: google.maps.MapTypeControlStyle.DROPDOWN_MENU, position: google.maps.ControlPosition.RIGHT_TOP }
                                        });
                                        MAPEDITOR.GLOBAL.DEFINES.mapControlsDisplayed = true;
                                    }
                                    MAPEDITOR.UTILITIES.buttonActive("mapControls"); //set the is active glow for button
                                    break;

                                case "toolbox":
                                    if (MAPEDITOR.GLOBAL.DEFINES.toolboxDisplayed == true) {
                                        document.getElementById("mapedit_container_toolbox").style.display = "none";
                                        document.getElementById("mapedit_container_toolboxTabs").style.display = "none";
                                        MAPEDITOR.GLOBAL.DEFINES.toolboxDisplayed = false;
                                    } else {
                                        $("#mapedit_container_toolbox").animate({ 'top': '75px', 'left': '100px' });
                                        document.getElementById("mapedit_container_toolbox").style.display = "block";
                                        document.getElementById("mapedit_container_toolboxTabs").style.display = "block";
                                        document.getElementById("mapedit_container_toolbox").style.height = "auto";
                                        MAPEDITOR.GLOBAL.DEFINES.toolboxDisplayed = true;
                                    }
                                    MAPEDITOR.UTILITIES.buttonActive("toolbox"); //set the is active glow for button
                                    break;

                                case "toolbar":
                                    if (MAPEDITOR.GLOBAL.DEFINES.toolbarDisplayed == true) {
                                        $("#mapedit_container_pane_1").hide();
                                        //$("#mapedit_container_pane_1").animate({ 'top': '-46px' });
                                        document.getElementById("mapedit_container_toolbarGrabber").style.marginTop = "0";
                                        MAPEDITOR.GLOBAL.DEFINES.toolbarDisplayed = false;
                                    } else {
                                        $("#mapedit_container_pane_1").show();
                                        //$("#mapedit_container_pane_1").animate({ 'top': '46px' });
                                        document.getElementById("mapedit_container_toolbarGrabber").style.marginTop = "48px";
                                        MAPEDITOR.GLOBAL.DEFINES.toolbarDisplayed = true;
                                    }
                                    MAPEDITOR.UTILITIES.buttonActive("toolbar"); //set the is active glow for button
                                    break;

                                case "kml":
                                    if (MAPEDITOR.GLOBAL.DEFINES.kmlDisplayed == true) {
                                        MAPEDITOR.GLOBAL.DEFINES.kmlLayer.setMap(null);
                                        MAPEDITOR.GLOBAL.DEFINES.kmlDisplayed = false;
                                    } else {
                                        MAPEDITOR.GLOBAL.DEFINES.kmlLayer.setMap(map);
                                        MAPEDITOR.GLOBAL.DEFINES.kmlDisplayed = true;
                                    }
                                    MAPEDITOR.UTILITIES.buttonActive("kml"); //set the is active glow for button
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
                                    if (MAPEDITOR.GLOBAL.DEFINES.mapDrawingManagerDisplayed == true) {
                                        MAPEDITOR.GLOBAL.DEFINES.drawingManager.setDrawingMode(null);
                                        MAPEDITOR.GLOBAL.DEFINES.drawingManager.setMap(null);
                                        MAPEDITOR.GLOBAL.DEFINES.mapDrawingManagerDisplayed = false;
                                    } else {
                                        MAPEDITOR.GLOBAL.DEFINES.drawingManager.setDrawingMode(null);
                                        MAPEDITOR.GLOBAL.DEFINES.drawingManager.setMap(map);
                                        MAPEDITOR.GLOBAL.DEFINES.mapDrawingManagerDisplayed = true;
                                    }
                                    break;

                                case "overlays":
                                    if (MAPEDITOR.GLOBAL.DEFINES.overlaysOnMap.length) {
                                        if (MAPEDITOR.GLOBAL.DEFINES.overlaysCurrentlyDisplayed == true) {
                                            MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L22);
                                            for (var i = 1; i < MAPEDITOR.GLOBAL.DEFINES.overlaysOnMap.length; i++) { //go through and display overlays as long as there is an overlay to display
                                                MAPEDITOR.TRACER.addTracer("[INFO]: overlay count " + MAPEDITOR.GLOBAL.DEFINES.overlayCount);
                                                MAPEDITOR.GLOBAL.DEFINES.RIBMode = true;
                                                if (document.getElementById("overlayToggle" + i)) {
                                                    MAPEDITOR.TRACER.addTracer("[INFO]: found: overlayToggle" + i);
                                                    for (var j = 0; j < MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPolygonType.length; j++) {
                                                        try {
                                                            MAPEDITOR.ACTIONS.overlayHideMe(i + j);
                                                        } catch (e) {
                                                            MAPEDITOR.TRACER.addTracer("[INFO]: overlayOnMap[" + (i + j) + "] not found");
                                                        }
                                                    }
                                                } else {
                                                    MAPEDITOR.TRACER.addTracer("[INFO]: did not find: overlayToggle" + i);
                                                }

                                                MAPEDITOR.GLOBAL.DEFINES.RIBMode = false;
                                                MAPEDITOR.GLOBAL.DEFINES.overlaysOnMap[i].setMap(null); //hide the overlay from the map
                                                MAPEDITOR.GLOBAL.DEFINES.ghostOverlayRectangle[i].setMap(null); //hide ghost from map
                                                MAPEDITOR.GLOBAL.DEFINES.overlaysCurrentlyDisplayed = false; //mark that overlays are not on the map
                                                MAPEDITOR.GLOBAL.DEFINES.buttonActive_overlayToggle = true;
                                            }
                                        } else {
                                            MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L23);
                                            for (var i = 1; i < MAPEDITOR.GLOBAL.DEFINES.overlaysOnMap.length; i++) { //go through and display overlays as long as there is an overlay to display
                                                MAPEDITOR.TRACER.addTracer("[INFO]: oom " + MAPEDITOR.GLOBAL.DEFINES.overlaysOnMap.length);
                                                MAPEDITOR.GLOBAL.DEFINES.RIBMode = true;
                                                if (document.getElementById("overlayToggle" + i)) {
                                                    MAPEDITOR.TRACER.addTracer("[INFO]: found: overlayToggle" + i);
                                                    MAPEDITOR.ACTIONS.overlayShowMe(i);
                                                } else {
                                                    MAPEDITOR.TRACER.addTracer("[INFO]: did not find: overlayToggle" + i);
                                                }
                                                MAPEDITOR.GLOBAL.DEFINES.RIBMode = false;
                                                MAPEDITOR.GLOBAL.DEFINES.overlaysOnMap[i].setMap(map); //set the overlay to the map
                                                MAPEDITOR.GLOBAL.DEFINES.ghostOverlayRectangle[i].setMap(map); //set to map
                                                MAPEDITOR.GLOBAL.DEFINES.overlaysCurrentlyDisplayed = true; //mark that overlays are on the map
                                                MAPEDITOR.GLOBAL.DEFINES.buttonActive_overlayToggle = false;
                                            }
                                        }
                                        MAPEDITOR.UTILITIES.buttonActive("overlayToggle");
                                    } else {
                                        //nothing to toggle
                                        MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L45);
                                    }
                                    break;

                                case "pois":
                                    if (MAPEDITOR.GLOBAL.DEFINES.poiCount) {
                                        MAPEDITOR.TRACER.addTracer("[INFO]: poi count: " + MAPEDITOR.GLOBAL.DEFINES.poiCount);
                                        MAPEDITOR.UTILITIES.buttonActive("poiToggle");
                                        if (MAPEDITOR.GLOBAL.DEFINES.poiToggleState == "displayed") {
                                            for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiCount; i++) {
                                                MAPEDITOR.ACTIONS.poiHideMe(i);
                                                MAPEDITOR.GLOBAL.DEFINES.poiToggleState = "hidden";
                                                MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L42);
                                            }
                                        } else {
                                            for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiCount; i++) {
                                                MAPEDITOR.ACTIONS.poiShowMe(i);
                                                MAPEDITOR.GLOBAL.DEFINES.poiToggleState = "displayed";
                                                MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L43);
                                            }
                                        }


                                    } else {
                                        //nothing to toggle
                                        MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L45);
                                    }
                                    break;

                                default:
                                    //toggle that item if not found above
                                    break;
                            }
                            MAPEDITOR.TRACER.addTracer("[INFO]: toggleVis completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    changeMapLayer: function (layer) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: changeMapLayer started...");
                            switch (layer) {
                                case "roadmap":
                                    map.setMapTypeId(google.maps.MapTypeId.ROADMAP);
                                    MAPEDITOR.GLOBAL.DEFINES.mapLayerActive = "Roadmap";
                                    break;
                                case "terrain":
                                    map.setMapTypeId(google.maps.MapTypeId.TERRAIN);
                                    MAPEDITOR.GLOBAL.DEFINES.mapLayerActive = "Terrain";
                                    break;
                                case "satellite":
                                    map.setMapTypeId(google.maps.MapTypeId.SATELLITE);
                                    MAPEDITOR.GLOBAL.DEFINES.mapLayerActive = "Satellite";
                                    break;
                                case "hybrid":
                                    map.setMapTypeId(google.maps.MapTypeId.HYBRID);
                                    MAPEDITOR.GLOBAL.DEFINES.mapLayerActive = "Hybrid";
                                    break;
                                case "custom":
                                    MAPEDITOR.ACTIONS.toggleVis("kml");
                                    break;
                                case "reset":
                                    map.setMapTypeId(google.maps.MapTypeId.ROADMAP);
                                    //2do make this set to default
                                    MAPEDITOR.GLOBAL.DEFINES.mapLayerActive = "Roadmap";
                                    if (MAPEDITOR.GLOBAL.DEFINES.kmlDisplayed == true) {
                                        MAPEDITOR.ACTIONS.toggleVis("kml");
                                    }
                                    break;
                            }
                            MAPEDITOR.UTILITIES.buttonActive("layer"); //set the is active glow for button
                            MAPEDITOR.TRACER.addTracer("[INFO]: changeMapLayer completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    panMap: function (direction) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: panmap started...");
                            switch (direction) {
                                case "up":
                                    map.panBy(0, -100);
                                    MAPEDITOR.UTILITIES.testBounds();
                                    break;
                                case "down":
                                    map.panBy(0, 100);
                                    MAPEDITOR.UTILITIES.testBounds();
                                    break;
                                case "left":
                                    map.panBy(-100, 0);
                                    MAPEDITOR.UTILITIES.testBounds();
                                    break;
                                case "right":
                                    map.panBy(100, 0);
                                    MAPEDITOR.UTILITIES.testBounds();
                                    break;
                                case "reset":
                                    map.panTo(MAPEDITOR.GLOBAL.DEFINES.mapCenter);
                                    break;
                            }
                            MAPEDITOR.TRACER.addTracer("[INFO]: panmap completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    zoomMap: function (direction) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: zoommap started...");
                            switch (direction) {
                                case "in":
                                    map.setZoom(map.getZoom() + 1);
                                    break;
                                case "out":
                                    map.setZoom(map.getZoom() - 1);
                                    break;
                                case "reset":
                                    map.setZoom(MAPEDITOR.GLOBAL.DEFINES.defaultZoomLevel);
                                    break;
                            }
                            MAPEDITOR.TRACER.addTracer("[INFO]: zoommap completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    action: function (id) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: action started...");
                            MAPEDITOR.TRACER.addTracer("[INFO]: action: " + id);
                            switch (id) {
                                case "manageItem":
                                    MAPEDITOR.GLOBAL.DEFINES.actionActive = "Item";  //note case (uppercase is tied to the actual div)
                                    MAPEDITOR.UTILITIES.buttonActive("action");
                                    if (MAPEDITOR.GLOBAL.DEFINES.toolboxDisplayed != true) {
                                        MAPEDITOR.ACTIONS.toggleVis("toolbox");
                                    }
                                    MAPEDITOR.ACTIONS.openToolboxTab(2);
                                    //force a suppression dm
                                    if (MAPEDITOR.GLOBAL.DEFINES.mapDrawingManagerDisplayed == true) {
                                        MAPEDITOR.GLOBAL.DEFINES.mapDrawingManagerDisplayed = true;
                                        MAPEDITOR.ACTIONS.toggleVis("mapDrawingManager");
                                    }

                                    MAPEDITOR.GLOBAL.DEFINES.placerType = "item";
                                    MAPEDITOR.ACTIONS.place("item");

                                    break;

                                case "manageOverlay":
                                    
                                    MAPEDITOR.GLOBAL.DEFINES.actionActive = "Overlay"; //notice case (uppercase is tied to the actual div)
                                    MAPEDITOR.UTILITIES.buttonActive("action");

                                    if (MAPEDITOR.GLOBAL.DEFINES.toolboxDisplayed != true) {
                                        MAPEDITOR.ACTIONS.toggleVis("toolbox");
                                    }
                                    MAPEDITOR.ACTIONS.openToolboxTab(3);

                                    //force a suppression of dm
                                    if (MAPEDITOR.GLOBAL.DEFINES.mapDrawingManagerDisplayed == true) {
                                        MAPEDITOR.GLOBAL.DEFINES.mapDrawingManagerDisplayed = true;
                                        MAPEDITOR.ACTIONS.toggleVis("mapDrawingManager");
                                    }

                                    //place
                                    MAPEDITOR.GLOBAL.DEFINES.placerType = "overlay";
                                    MAPEDITOR.ACTIONS.place("overlay");

                                    break;

                                case "managePOI":
                                    //MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                    MAPEDITOR.GLOBAL.DEFINES.actionActive = "POI"; //notice case (uppercase is tied to the actual div)
                                    MAPEDITOR.UTILITIES.buttonActive("action");
                                    if (MAPEDITOR.GLOBAL.DEFINES.toolboxDisplayed != true) {
                                        MAPEDITOR.ACTIONS.toggleVis("toolbox");
                                    }
                                    MAPEDITOR.ACTIONS.openToolboxTab(4);
                                    MAPEDITOR.ACTIONS.toggleVis("mapDrawingManager");

                                    //place
                                    MAPEDITOR.GLOBAL.DEFINES.placerType = "poi";
                                    MAPEDITOR.ACTIONS.place("poi");
                                    break;

                                case "other":
                                    MAPEDITOR.TRACER.addTracer("[INFO]: action Other started...");
                                    MAPEDITOR.GLOBAL.DEFINES.actionActive = "Other";
                                    MAPEDITOR.UTILITIES.buttonActive("action");
                                    //force a suppression dm
                                    if (MAPEDITOR.GLOBAL.DEFINES.mapDrawingManagerDisplayed == true) {
                                        MAPEDITOR.GLOBAL.DEFINES.mapDrawingManagerDisplayed = true;
                                        MAPEDITOR.ACTIONS.toggleVis("mapDrawingManager");
                                    }

                                    MAPEDITOR.GLOBAL.DEFINES.placerType = "none";
                                    MAPEDITOR.TRACER.addTracer("[INFO]: action Other ended...");

                                    break;

                                case "search":
                                    MAPEDITOR.TRACER.addTracer("[INFO]: action search started...");
                                    MAPEDITOR.GLOBAL.DEFINES.actionActive = "Search";
                                    MAPEDITOR.UTILITIES.buttonActive("action");
                                    MAPEDITOR.GLOBAL.DEFINES.placerType = "none";

                                    //force a suppression dm (unknown if we need to?)
                                    if (MAPEDITOR.GLOBAL.DEFINES.mapDrawingManagerDisplayed == true) {
                                        MAPEDITOR.GLOBAL.DEFINES.mapDrawingManagerDisplayed = true;
                                        MAPEDITOR.ACTIONS.toggleVis("mapDrawingManager");
                                    }

                                    //open search tab
                                    MAPEDITOR.TRACER.addTracer("[INFO]: MAPEDITOR.GLOBAL.DEFINES.toolboxDisplayed: " + MAPEDITOR.GLOBAL.DEFINES.toolboxDisplayed);
                                    if (MAPEDITOR.GLOBAL.DEFINES.toolboxDisplayed == true) {
                                        MAPEDITOR.ACTIONS.openToolboxTab(1);
                                    }

                                    break;
                            }
                            MAPEDITOR.TRACER.addTracer("[INFO]: action completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    place: function (id) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: place started...");
                            switch (id) {
                                case "item":
                                    MAPEDITOR.UTILITIES.buttonActive("itemPlace");
                                    MAPEDITOR.GLOBAL.DEFINES.placerType = "item";
                                    if (MAPEDITOR.GLOBAL.DEFINES.itemMarker != null) {
                                        MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L30);
                                    } else {
                                        if (MAPEDITOR.GLOBAL.DEFINES.searchCount > 0 && MAPEDITOR.GLOBAL.DEFINES.itemMarker == null) {
                                            MAPEDITOR.ACTIONS.useSearchAsItemLocation();
                                            MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L18);
                                        } else {
                                            MAPEDITOR.GLOBAL.DEFINES.drawingManager.setOptions({ drawingControl: false, drawingControlOptions: { position: google.maps.ControlPosition.RIGHT_TOP, drawingModes: [google.maps.drawing.OverlayType.MARKER], markerOptions: MAPEDITOR.GLOBAL.DEFINES.markerOptionsItem } });
                                            MAPEDITOR.GLOBAL.DEFINES.drawingManager.setDrawingMode(google.maps.drawing.OverlayType.MARKER);
                                            MAPEDITOR.GLOBAL.DEFINES.drawingManager.setMap(map);
                                        }
                                    }
                                    break;
                                case "overlay":
                                    MAPEDITOR.GLOBAL.DEFINES.placerType = "overlay";
                                    if (MAPEDITOR.GLOBAL.DEFINES.pageMode == "edit") {
                                        MAPEDITOR.GLOBAL.DEFINES.pageMode = "view";
                                    } else {
                                        MAPEDITOR.GLOBAL.DEFINES.pageMode = "edit";
                                    }
                                    break;
                                case "poi":
                                    MAPEDITOR.GLOBAL.DEFINES.drawingManager.setOptions({ drawingControl: true, drawingControlOptions: { position: google.maps.ControlPosition.RIGHT_TOP, drawingModes: [google.maps.drawing.OverlayType.MARKER, google.maps.drawing.OverlayType.CIRCLE, google.maps.drawing.OverlayType.RECTANGLE, google.maps.drawing.OverlayType.POLYGON, google.maps.drawing.OverlayType.POLYLINE], markerOptions: MAPEDITOR.GLOBAL.DEFINES.markerOptionsPOI, circleOptions: MAPEDITOR.GLOBAL.DEFINES.circleOptionsPOI, rectangleOptions: MAPEDITOR.GLOBAL.DEFINES.rectangleOptionsPOI, polygonOptions: MAPEDITOR.GLOBAL.DEFINES.polygonOptionsPOI, polylineOptions: MAPEDITOR.GLOBAL.DEFINES.polylineOptionsPOI } });
                                    MAPEDITOR.GLOBAL.DEFINES.placerType = "poi";
                                    break;
                            }
                            MAPEDITOR.TRACER.addTracer("[INFO]: place completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    placePOI: function (type) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: placepoi started...");
                            MAPEDITOR.GLOBAL.DEFINES.placerType = "poi";
                            switch (type) {
                                case "marker":
                                    MAPEDITOR.GLOBAL.DEFINES.drawingManager.setOptions({ drawingModes: [google.maps.drawing.OverlayType.MARKER], markerOptions: MAPEDITOR.GLOBAL.DEFINES.markerOptionsPOI });
                                    MAPEDITOR.GLOBAL.DEFINES.drawingManager.setDrawingMode(google.maps.drawing.OverlayType.MARKER);
                                    MAPEDITOR.UTILITIES.buttonActive("poiMarker");
                                    break;
                                case "circle":
                                    MAPEDITOR.GLOBAL.DEFINES.drawingManager.setOptions({ drawingModes: [google.maps.drawing.OverlayType.CIRCLE], circleOptions: MAPEDITOR.GLOBAL.DEFINES.circleOptionsPOI });
                                    MAPEDITOR.GLOBAL.DEFINES.drawingManager.setDrawingMode(google.maps.drawing.OverlayType.CIRCLE);
                                    MAPEDITOR.UTILITIES.buttonActive("poiCircle");
                                    break;
                                case "rectangle":
                                    MAPEDITOR.GLOBAL.DEFINES.drawingManager.setOptions({ drawingModes: [google.maps.drawing.OverlayType.RECTANGLE], rectangleOptions: MAPEDITOR.GLOBAL.DEFINES.rectangleOptionsPOI });
                                    MAPEDITOR.GLOBAL.DEFINES.drawingManager.setDrawingMode(google.maps.drawing.OverlayType.RECTANGLE);
                                    MAPEDITOR.UTILITIES.buttonActive("poiRectangle");
                                    break;
                                case "polygon":
                                    MAPEDITOR.GLOBAL.DEFINES.drawingManager.setOptions({ drawingModes: [google.maps.drawing.OverlayType.POLYGON], polygonOptions: MAPEDITOR.GLOBAL.DEFINES.polygonOptionsPOI });
                                    MAPEDITOR.GLOBAL.DEFINES.drawingManager.setDrawingMode(google.maps.drawing.OverlayType.POLYGON);
                                    MAPEDITOR.UTILITIES.buttonActive("poiPolygon");
                                    break;
                                case "line":
                                    MAPEDITOR.GLOBAL.DEFINES.drawingManager.setOptions({ drawingModes: [google.maps.drawing.OverlayType.POLYLINE], polylineOptions: MAPEDITOR.GLOBAL.DEFINES.polylineOptionsPOI });
                                    MAPEDITOR.GLOBAL.DEFINES.drawingManager.setDrawingMode(google.maps.drawing.OverlayType.POLYLINE);
                                    MAPEDITOR.UTILITIES.buttonActive("poiLine");
                                    break;
                            }
                            MAPEDITOR.TRACER.addTracer("[INFO]: placepoi completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    geolocate: function (id) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: geolocate started...");
                            MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L50);
                            switch (id) {
                                case "item":
                                    MAPEDITOR.GLOBAL.DEFINES.drawingManager.setDrawingMode(google.maps.drawing.OverlayType.MARKER);
                                    MAPEDITOR.GLOBAL.DEFINES.placerType = "item";
                                    // Try W3C Geolocation
                                    if (navigator.geolocation) {
                                        navigator.geolocation.getCurrentPosition(function (position) {
                                            var userLocation = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);
                                            map.setCenter(userLocation);
                                            MAPEDITOR.UTILITIES.testBounds();
                                            if (MAPEDITOR.GLOBAL.DEFINES.mapInBounds == "yes") {
                                                MAPEDITOR.GLOBAL.DEFINES.markerCenter = userLocation;
                                                MAPEDITOR.GLOBAL.DEFINES.itemMarker = new google.maps.Marker({
                                                    position: MAPEDITOR.GLOBAL.DEFINES.markerCenter,
                                                    map: map
                                                });
                                                MAPEDITOR.GLOBAL.DEFINES.itemMarker.setMap(map);
                                                MAPEDITOR.UTILITIES.codeLatLng(MAPEDITOR.GLOBAL.DEFINES.itemMarker.getPosition());
                                                MAPEDITOR.GLOBAL.DEFINES.savingMarkerCenter = MAPEDITOR.GLOBAL.DEFINES.itemMarker.getPosition(); //store coords to save
                                                MAPEDITOR.TRACER.addTracer("[INFO]: rg userlocation: " + userLocation);
                                                var userLocationS = userLocation.toString();
                                                userLocationS = userLocationS.replace("\)", "");
                                                userLocationS = userLocationS.replace("\)", "");
                                                MAPEDITOR.TRACER.addTracer("[INFO]: rg test2: " + userLocationS);
                                                document.getElementById('content_toolbox_posItem').value = userLocationS;
                                                MAPEDITOR.UTILITIES.codeLatLng(userLocation);
                                            } else {
                                                MAPEDITOR.TRACER.addTracer("[INFO]: user location out of bounds");
                                            }

                                        });

                                    } else {
                                        alert(MAPEDITOR.LOCALIZATION.DEFINES.L4);
                                        MAPEDITOR.TRACER.addTracer(MAPEDITOR.LOCALIZATION.DEFINES.L4);
                                    }
                                    MAPEDITOR.GLOBAL.DEFINES.drawingManager.setDrawingMode(null);
                                    break;
                                case "overlay":
                                    MAPEDITOR.GLOBAL.DEFINES.placerType = "overlay";
                                    // Try W3C Geolocation
                                    if (navigator.geolocation) {
                                        navigator.geolocation.getCurrentPosition(function (position) {
                                            var userLocation = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);
                                            map.setCenter(userLocation);
                                            MAPEDITOR.UTILITIES.testBounds();
                                        });

                                    } else {
                                        alert(MAPEDITOR.LOCALIZATION.DEFINES.L4);
                                        MAPEDITOR.TRACER.addTracer(MAPEDITOR.LOCALIZATION.DEFINES.L4);
                                    }
                                    break;
                                case "poi":
                                    MAPEDITOR.GLOBAL.DEFINES.placerType = "poi";
                                    // Try W3C Geolocation
                                    if (navigator.geolocation) {
                                        navigator.geolocation.getCurrentPosition(function (position) {
                                            var userLocation = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);
                                            map.setCenter(userLocation);
                                            MAPEDITOR.UTILITIES.testBounds();
                                        });

                                    } else {
                                        alert(MAPEDITOR.LOCALIZATION.DEFINES.L4);
                                        MAPEDITOR.TRACER.addTracer(MAPEDITOR.LOCALIZATION.DEFINES.L4);
                                    }
                                    break;
                            }
                            MAPEDITOR.TRACER.addTracer("[INFO]: geolocate completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    save: function (id) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: save started...");
                            switch (id) {
                                case "item":
                                    //determine if there is something to save
                                    if (MAPEDITOR.GLOBAL.DEFINES.savingMarkerCenter != null) {
                                        //is this the first time saving a changed item?
                                        if (MAPEDITOR.GLOBAL.DEFINES.firstSaveItem == true) {
                                            //determine if there is any new data
                                            if (MAPEDITOR.GLOBAL.DEFINES.userMayLoseData) {
                                                MAPEDITOR.TRACER.addTracer("[INFO]: Saving Changes...");
                                                MAPEDITOR.TRACER.addTracer("[INFO]: saving location: " + MAPEDITOR.GLOBAL.DEFINES.savingMarkerCenter);
                                                //save to temp xml file
                                                MAPEDITOR.GLOBAL.DEFINES.toServerSuccessMessage = MAPEDITOR.LOCALIZATION.DEFINES.L_Saved;
                                                MAPEDITOR.UTILITIES.createSavedItem("save", MAPEDITOR.GLOBAL.DEFINES.savingMarkerCenter);
                                                MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = false;
                                            } else {
                                                MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L_NotSaved);
                                            }
                                        } else {
                                            //not used yet
                                        }
                                    } else {
                                        MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L_NotSaved);
                                    }
                                    break;
                                case "overlay":
                                    //is this the first time saving a changed item?
                                    MAPEDITOR.TRACER.addTracer("[INFO]: first save overlay? " + MAPEDITOR.GLOBAL.DEFINES.firstSaveOverlay);
                                    if (MAPEDITOR.GLOBAL.DEFINES.firstSaveOverlay == true) {
                                        //determine if there is something to save
                                        MAPEDITOR.TRACER.addTracer("[INFO]: overlay length? " + MAPEDITOR.GLOBAL.DEFINES.savingOverlayIndex.length);
                                        if (MAPEDITOR.GLOBAL.DEFINES.savingOverlayIndex.length) {
                                            //determine if there is any new data
                                            if (MAPEDITOR.GLOBAL.DEFINES.userMayLoseData) {
                                                for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.savingOverlayIndex.length; i++) {
                                                    //save to temp xml file
                                                    try {
                                                        //explicitly change TEMP_ IDs
                                                        MAPEDITOR.GLOBAL.DEFINES.incomingPolygonFeatureType[i] = "main";
                                                        MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPolygonType[i] = "rectangle";
                                                        MAPEDITOR.GLOBAL.DEFINES.toServerSuccessMessage = MAPEDITOR.LOCALIZATION.DEFINES.L_Saved;
                                                        MAPEDITOR.TRACER.addTracer("[INFO]: saving overlay: (" + i + ") " + MAPEDITOR.GLOBAL.DEFINES.savingOverlayPageId[i] + "\nlabel: " + MAPEDITOR.GLOBAL.DEFINES.savingOverlayLabel[i] + "\nsource: " + MAPEDITOR.GLOBAL.DEFINES.savingOverlaySourceURL[i] + "\nbounds: " + MAPEDITOR.GLOBAL.DEFINES.savingOverlayBounds[i] + "\nrotation: " + MAPEDITOR.GLOBAL.DEFINES.savingOverlayRotation[i]);
                                                        MAPEDITOR.UTILITIES.createSavedOverlay("save", MAPEDITOR.GLOBAL.DEFINES.savingOverlayPageId[i], MAPEDITOR.GLOBAL.DEFINES.savingOverlayLabel[i], MAPEDITOR.GLOBAL.DEFINES.savingOverlaySourceURL[i], MAPEDITOR.GLOBAL.DEFINES.savingOverlayBounds[i], MAPEDITOR.GLOBAL.DEFINES.savingOverlayRotation[i]); //send overlay to the server
                                                    } catch (e) {
                                                        //no overlay at this point to save
                                                    }
                                                }
                                                MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = false;
                                                //not used yet
                                            } else {
                                                MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L_NotSaved);
                                            }
                                        } else {
                                            MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L_NotSaved);
                                        }
                                    } else {
                                        //not used yet
                                    }
                                    break;
                                case "poi":
                                    //is this the first time saving a changed item? (apply changes)
                                    if (MAPEDITOR.GLOBAL.DEFINES.firstSavePOI == true) {
                                        //determine if there is something to save
                                        if (MAPEDITOR.GLOBAL.DEFINES.poiObj.length > 0) {
                                            //determine if there is any new data
                                            if (MAPEDITOR.GLOBAL.DEFINES.userMayLoseData) {
                                                //save to temp xml file
                                                MAPEDITOR.TRACER.addTracer("[INFO]: saving " + MAPEDITOR.GLOBAL.DEFINES.poiObj.length + " POIs...");
                                                MAPEDITOR.GLOBAL.DEFINES.toServerSuccessMessage = MAPEDITOR.LOCALIZATION.DEFINES.L_Saved;
                                                MAPEDITOR.UTILITIES.createSavedPOI("save");
                                                //explicitly turn off the drawing manager 
                                                MAPEDITOR.GLOBAL.DEFINES.drawingManager.setDrawingMode(null);
                                                MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = false;
                                            } else {
                                                MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L_NotSaved);
                                            }
                                        } else {
                                            MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L_NotSaved);
                                        }
                                    } else {
                                        MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L_NotSaved);
                                    }
                                    break;
                            }
                            MAPEDITOR.TRACER.addTracer("[INFO]: save completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    clear: function (id) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: clear started...");
                            switch (id) {
                                case "item":
                                    if (MAPEDITOR.GLOBAL.DEFINES.savingMarkerCenter != null) {
                                        //clear the current marker
                                        MAPEDITOR.GLOBAL.DEFINES.itemMarker.setMap(null); //delete marker form map
                                        MAPEDITOR.GLOBAL.DEFINES.itemMarker = null; //delete reference to marker
                                        MAPEDITOR.GLOBAL.DEFINES.savingMarkerCenter = null; //reset stored coords to save
                                        document.getElementById('content_toolbox_posItem').value = ""; //reset lat/long in tab
                                        document.getElementById('content_toolbox_rgItem').value = ""; //reset address in tab
                                        //redraw incoming marker
                                        MAPEDITOR.GLOBAL.displayIncomingPoints();
                                        //reset
                                        MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = false;
                                        MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L9); //say all is reset
                                    } else {
                                        MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L_NotCleared);
                                    }
                                    break;
                                case "overlay":
                                    if (MAPEDITOR.GLOBAL.DEFINES.savingOverlayIndex.length > 0) {
                                        MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L52);
                                        //reset edit mode
                                        MAPEDITOR.ACTIONS.place("overlay");
                                        //reset hidden overlays
                                        MAPEDITOR.UTILITIES.resetHiddenOverlays();
                                        //delete all incoming overlays
                                        MAPEDITOR.UTILITIES.clearIncomingOverlays();
                                        //clear the save cache
                                        MAPEDITOR.UTILITIES.clearCacheSaveOverlay();
                                        //clear ooms
                                        MAPEDITOR.UTILITIES.clearOverlaysOnMap();
                                        //show all the incoming overlays
                                        MAPEDITOR.GLOBAL.displayIncomingPolygons();
                                        //redraw list items of overlays
                                        MAPEDITOR.GLOBAL.initOverlayList();
                                        //reset
                                        MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = false;
                                        //say we are finished
                                        MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L10);
                                    } else {
                                        MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L46);
                                    }
                                    break;
                                case "poi":
                                    MAPEDITOR.TRACER.addTracer("[INFO]: attempting to clear " + MAPEDITOR.GLOBAL.DEFINES.poiObj.length + "POIs...");
                                    try {
                                        if (MAPEDITOR.GLOBAL.DEFINES.poiObj.length > 0) {
                                            //warn the user that this will delete all the pois
                                            if (MAPEDITOR.UTILITIES.confirmMessage(MAPEDITOR.LOCALIZATION.DEFINES.L72)) {
                                                MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L53);
                                                for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                                    if (MAPEDITOR.GLOBAL.DEFINES.poiType[i] != "deleted") {
                                                        MAPEDITOR.GLOBAL.DEFINES.poiType[i] = "deleted";
                                                    }
                                                    if (MAPEDITOR.GLOBAL.DEFINES.poiObj[i] != null) {
                                                        MAPEDITOR.GLOBAL.DEFINES.poiObj[i].setMap(null);
                                                        //MAPEDITOR.GLOBAL.DEFINES.poiObj[i] = null;
                                                    }
                                                    if (MAPEDITOR.GLOBAL.DEFINES.poiDesc[i] != null) {
                                                        MAPEDITOR.GLOBAL.DEFINES.poiDesc[i] = null;
                                                    }
                                                    if (MAPEDITOR.GLOBAL.DEFINES.poiKML[i] != null) {
                                                        MAPEDITOR.GLOBAL.DEFINES.poiKML[i] = null;
                                                    }
                                                    MAPEDITOR.GLOBAL.DEFINES.infoWindow[i].setMap(null);
                                                    MAPEDITOR.GLOBAL.DEFINES.infoWindow[i] = null;
                                                    MAPEDITOR.GLOBAL.DEFINES.label[i].setMap(null);
                                                    MAPEDITOR.GLOBAL.DEFINES.label[i] = null;
                                                    var strg = "#poi" + i; //create <li> poi string
                                                    $(strg).remove(); //remove <li>
                                                }
                                                MAPEDITOR.GLOBAL.DEFINES.poi_i = -1;
                                                //send to server to delete all the pois
                                                MAPEDITOR.GLOBAL.DEFINES.RIBMode = true;
                                                MAPEDITOR.GLOBAL.DEFINES.toServerSuccessMessage = MAPEDITOR.LOCALIZATION.DEFINES.L_Deleted;
                                                MAPEDITOR.UTILITIES.createSavedPOI("save");
                                                MAPEDITOR.GLOBAL.DEFINES.RIBMode = false;
                                                //reset poi arrays
                                                MAPEDITOR.GLOBAL.DEFINES.poiObj = [];
                                                MAPEDITOR.GLOBAL.DEFINES.poiDesc = [];
                                                MAPEDITOR.GLOBAL.DEFINES.poiKML = [];
                                                //reset
                                                MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = false;
                                                //MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L11);
                                            } else {
                                                //MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L_NotCleared);
                                            }
                                        } else {
                                            MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L_NotCleared);
                                        }
                                    } catch (e) {
                                        MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L_NotCleared);
                                    }
                                    break;
                            }
                            MAPEDITOR.TRACER.addTracer("[INFO]: clear completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    searchResultDeleteMe: function (id) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: searchResultDeleteMe started...");
                            //remove visually
                            MAPEDITOR.GLOBAL.DEFINES.searchResult.setMap(null); //remove from map
                            $("#searchResultListItem" + id).remove(); //remove the first result div from result list box in toolbox
                            document.getElementById("content_toolbar_searchField").value = ""; //clear searchbar
                            document.getElementById("content_toolbox_searchField").value = ""; //clear searchbox
                            //remove references to 
                            MAPEDITOR.GLOBAL.DEFINES.searchResult = null; //reset search result map item
                            MAPEDITOR.GLOBAL.DEFINES.searchCount = 0; //reset search count
                            MAPEDITOR.TRACER.addTracer("[INFO]: searchResultDeleteMe completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    searchResultShowMe: function (id) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: searchResultShowMe started...");
                            MAPEDITOR.GLOBAL.DEFINES.searchResult.setMap(map); //remove from map
                            document.getElementById("searchResultToggle" + id).innerHTML = "<img src=\"" + MAPEDITOR.GLOBAL.DEFINES.baseURL + MAPEDITOR.GLOBAL.DEFINES.baseImageDirURL + "sub.png\" onclick=\"MAPEDITOR.ACTIONS.searchResultHideMe(" + id + ");\" />";
                            MAPEDITOR.TRACER.addTracer("[INFO]: searchResultShowMe completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    searchResultHideMe: function (id) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: searchResultHideMe started...");
                            MAPEDITOR.GLOBAL.DEFINES.searchResult.setMap(null); //remove from map
                            document.getElementById("searchResultToggle" + id).innerHTML = "<img src=\"" + MAPEDITOR.GLOBAL.DEFINES.baseURL + MAPEDITOR.GLOBAL.DEFINES.baseImageDirURL + "add.png\" onclick=\"MAPEDITOR.ACTIONS.searchResultShowMe(" + id + ");\" />";
                            MAPEDITOR.TRACER.addTracer("[INFO]: searchResultHideMe completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    poiGetDesc: function (id) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: poiGetDesc started...");
                            MAPEDITOR.TRACER.addTracer("[INFO]: MAPEDITOR.ACTIONS.poiGetDesc(" + id + "); started...");
                            //filter to not set desc to blank
                            if (document.getElementById("poiDesc" + id).value == "") {
                                return;
                            } else {
                                //get the desc
                                var temp = document.getElementById("poiDesc" + id).value;

                                //check for invalid characters
                                if (temp.contains("~") || temp.contains("|")) {
                                    MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L8);
                                } else {

                                    //get and hold heldPosistion
                                    var heldPosition = MAPEDITOR.GLOBAL.DEFINES.infoWindow[id].position;

                                    MAPEDITOR.TRACER.addTracer("[INFO]: poiDesc[id]: " + MAPEDITOR.GLOBAL.DEFINES.poiDesc[id]);
                                    MAPEDITOR.TRACER.addTracer("[INFO]: temp: " + temp);

                                    //replace the list item title 
                                    var tempHTMLHolder1 = document.getElementById("poiList").innerHTML.replace(MAPEDITOR.GLOBAL.DEFINES.poiDesc[id], temp);
                                    document.getElementById("poiList").innerHTML = tempHTMLHolder1;

                                    //MAPEDITOR.TRACER.addTracer("[INFO]: tempHTMLHolder1: " + tempHTMLHolder1);
                                    MAPEDITOR.TRACER.addTracer("[INFO]: MAPEDITOR.GLOBAL.DEFINES.poiDesc[id].substring(0, 20): " + MAPEDITOR.GLOBAL.DEFINES.poiDesc[id].substring(0, 20));
                                    MAPEDITOR.TRACER.addTracer("[INFO]: temp.substring(0, 20): " + temp.substring(0, 20));

                                    //now replace the list item (order is important)
                                    var tempHTMLHolder2 = document.getElementById("poiList").innerHTML.replace(">" + MAPEDITOR.GLOBAL.DEFINES.poiDesc[id].substring(0, 20), ">" + temp.substring(0, 20));
                                    //now post all this back to the listbox
                                    document.getElementById("poiList").innerHTML = tempHTMLHolder2;

                                    MAPEDITOR.TRACER.addTracer("[INFO]: tempHTMLHolder2: " + tempHTMLHolder2);
                                    MAPEDITOR.TRACER.addTracer("[INFO]: MAPEDITOR.GLOBAL.DEFINES.label[id]" + MAPEDITOR.GLOBAL.DEFINES.label[id]);
                                    MAPEDITOR.TRACER.addTracer("[INFO]: temp.substring(0, 20): " + temp.substring(0, 20));

                                    //replace the object label
                                    MAPEDITOR.GLOBAL.DEFINES.label[id].set("labelContent", temp.substring(0, 20));

                                    MAPEDITOR.TRACER.addTracer("[INFO]: MAPEDITOR.GLOBAL.DEFINES.poiDesc[id]: " + MAPEDITOR.GLOBAL.DEFINES.poiDesc[id]);
                                    MAPEDITOR.TRACER.addTracer("[INFO]: temp: " + temp);

                                    //assign full description to the poi object
                                    MAPEDITOR.GLOBAL.DEFINES.poiDesc[id] = temp;

                                    //close old info window (this negates bug where desc box would no longer be tied to point)
                                    MAPEDITOR.GLOBAL.DEFINES.infoWindow[id].setMap(null);

                                    //visually reset desc
                                    //MAPEDITOR.GLOBAL.DEFINES.infoWindow[id].setOptions({ content: MAPEDITOR.UTILITIES.writeHTML("poiDesc", id, "", "") });

                                    MAPEDITOR.GLOBAL.DEFINES.infoWindow[id] = new google.maps.InfoWindow({
                                        content: MAPEDITOR.UTILITIES.writeHTML("poiDescIncoming", id, temp, ""),
                                        position: heldPosition,
                                        pixelOffset: new google.maps.Size(0, -40)
                                    });

                                    //close the poi desc box
                                    MAPEDITOR.GLOBAL.DEFINES.infoWindow[id].setMap(null);
                                }
                            }
                            MAPEDITOR.TRACER.addTracer("[INFO]: MAPEDITOR.ACTIONS.poiGetDesc(" + id + "); finished...");
                            MAPEDITOR.TRACER.addTracer("[INFO]: poiGetDesc completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    poiDeleteMe: function (id) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: poiDeleteMe started...");
                            MAPEDITOR.GLOBAL.DEFINES.poiObj[id].setMap(null);
                            MAPEDITOR.GLOBAL.DEFINES.poiObj[id] = null;
                            MAPEDITOR.GLOBAL.DEFINES.poiType[id] = "deleted";
                            var strg = "#poi" + id; //create <li> poi string
                            $(strg).remove(); //remove <li>
                            MAPEDITOR.GLOBAL.DEFINES.infoWindow[id].setMap(null);
                            MAPEDITOR.GLOBAL.DEFINES.label[id].setMap(null);
                            MAPEDITOR.TRACER.addTracer("[INFO]: poiDeleteMe completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    poiShowMe: function (id) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: poiShowMe started...");
                            MAPEDITOR.GLOBAL.DEFINES.poiObj[id].setMap(map);
                            //explicitly declar position of MAPEDITOR.GLOBAL.DEFINES.infoWindow (fixes issue of first poi desc posit on load)
                            //MAPEDITOR.GLOBAL.DEFINES.infoWindow[id].setOptions({ pixelOffset: new google.maps.Size(0, -40) });
                            MAPEDITOR.GLOBAL.DEFINES.infoWindow[id].setMap(map);
                            MAPEDITOR.GLOBAL.DEFINES.label[id].setMap(map);
                            //document.getElementById("poi" + id).style.background = MAPEDITOR.GLOBAL.DEFINES.listItemHighlightColor;
                            document.getElementById("poiToggle" + id).innerHTML = "<img src=\"" + MAPEDITOR.GLOBAL.DEFINES.baseURL + MAPEDITOR.GLOBAL.DEFINES.baseImageDirURL + "sub.png\" onclick=\"MAPEDITOR.ACTIONS.poiHideMe(" + id + ");\" />";
                            MAPEDITOR.TRACER.addTracer("[INFO]: poiShowMe completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    poiHideMe: function (id) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: poihideme started...");
                            MAPEDITOR.GLOBAL.DEFINES.poiObj[id].setMap(null);
                            MAPEDITOR.GLOBAL.DEFINES.infoWindow[id].setMap(null);
                            MAPEDITOR.GLOBAL.DEFINES.label[id].setMap(null);
                            document.getElementById("poiToggle" + id).innerHTML = "<img src=\"" + MAPEDITOR.GLOBAL.DEFINES.baseURL + MAPEDITOR.GLOBAL.DEFINES.baseImageDirURL + "add.png\" onclick=\"MAPEDITOR.ACTIONS.poiShowMe(" + id + ");\" />";
                            MAPEDITOR.TRACER.addTracer("[INFO]: poihideme completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    poiEditMe: function (id) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: poiEditMe started...");
                            MAPEDITOR.GLOBAL.DEFINES.poiObj[id].setMap(map);
                            //explicitly declar position of MAPEDITOR.GLOBAL.DEFINES.infoWindow (fixes issue of first poi desc posit on load)
                            //MAPEDITOR.GLOBAL.DEFINES.infoWindow[id].setOptions({ pixelOffset: new google.maps.Size(0, -40) });
                            MAPEDITOR.GLOBAL.DEFINES.infoWindow[id].setMap(map);
                            MAPEDITOR.GLOBAL.DEFINES.label[id].setMap(map);
                            //document.getElementById("poi" + id).style.background = MAPEDITOR.GLOBAL.DEFINES.listItemHighlightColor;
                            document.getElementById("poiToggle" + id).innerHTML = "<img src=\"" + MAPEDITOR.GLOBAL.DEFINES.baseURL + MAPEDITOR.GLOBAL.DEFINES.baseImageDirURL + "sub.png\" onclick=\"MAPEDITOR.ACTIONS.poiHideMe(" + id + ");\" />";
                            MAPEDITOR.TRACER.addTracer("[INFO]: poiEditMe completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    overlayDeleteMe: function (id) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: overlayDeleteMe started...");
                            if (MAPEDITOR.GLOBAL.DEFINES.overlaysOnMap[id]) {
                                MAPEDITOR.UTILITIES.confirmMessage(MAPEDITOR.LOCALIZATION.DEFINES.L54);
                                try {
                                    //MAPEDITOR.GLOBAL.DEFINES.toServerSuccessMessage = MAPEDITOR.LOCALIZATION.DEFINES.L68;
                                    MAPEDITOR.GLOBAL.DEFINES.toServerSuccessMessage = MAPEDITOR.LOCALIZATION.DEFINES.L55 + " " + id;
                                    MAPEDITOR.UTILITIES.createSavedOverlay("delete", id, "", "", "", "");
                                    MAPEDITOR.GLOBAL.DEFINES.overlaysOnMap[id].setMap(null);
                                    MAPEDITOR.GLOBAL.DEFINES.overlaysOnMap[id] = null;
                                    MAPEDITOR.GLOBAL.DEFINES.ghostOverlayRectangle[id].setMap(null);
                                    MAPEDITOR.GLOBAL.DEFINES.ghostOverlayRectangle[id] = null;
                                    //var strg = "#overlayListItem" + id; //create <li> overlay string
                                    //$(strg).remove(); //remove <li>
                                    MAPEDITOR.GLOBAL.DEFINES.overlayCount += -1;
                                    MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex = null;
                                    //MAPEDITOR.UTILITIES.displayMessage(id + " " + MAPEDITOR.LOCALIZATION.DEFINES.L33);
                                    //MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L55 + " " + id);
                                    MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = false;
                                } catch (e) {
                                    MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L57); //nothing to delete
                                }
                            } else {
                                MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L57); //nothing to delete
                            }
                            MAPEDITOR.TRACER.addTracer("[INFO]: overlayDeleteMe completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    overlayShowMe: function (id) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: overlayShowMe started...");
                            MAPEDITOR.GLOBAL.DEFINES.overlaysOnMap[id].setMap(map);
                            MAPEDITOR.GLOBAL.DEFINES.ghostOverlayRectangle[id].setMap(map);
                            document.getElementById("overlayToggle" + id).innerHTML = "<img src=\"" + MAPEDITOR.GLOBAL.DEFINES.baseURL + MAPEDITOR.GLOBAL.DEFINES.baseImageDirURL + "sub.png\" onclick=\"MAPEDITOR.ACTIONS.overlayHideMe(" + id + ");\" />";
                            MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L32 + " " + MAPEDITOR.GLOBAL.DEFINES.incomingPolygonLabel[id]);
                            MAPEDITOR.TRACER.addTracer("[INFO]: overlayShowMe completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    overlayHideMe: function (id) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: overlayHideMe started...");
                            MAPEDITOR.GLOBAL.DEFINES.overlaysOnMap[id].setMap(null);
                            MAPEDITOR.GLOBAL.DEFINES.ghostOverlayRectangle[id].setMap(null);
                            //document.getElementById("overlayListItem" + id).style.background = null;
                            document.getElementById("overlayToggle" + id).innerHTML = "<img src=\"" + MAPEDITOR.GLOBAL.DEFINES.baseURL + MAPEDITOR.GLOBAL.DEFINES.baseImageDirURL + "add.png\" onclick=\"MAPEDITOR.ACTIONS.overlayShowMe(" + id + ");\" />";
                            MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L31 + " " + MAPEDITOR.GLOBAL.DEFINES.incomingPolygonLabel[id]);
                            MAPEDITOR.TRACER.addTracer("[INFO]: overlayHideMe completed...");
                        } catch (err) {
                            MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L56); //nothing to hide
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    overlayEditMe: function (id) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: overlayEditMe started...");
                            MAPEDITOR.TRACER.addTracer("[INFO]: editing overlay id: " + id);
                            //alert("editing... \noverlay id: " + id + "\nwoi: " + MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex + "\nsor: " + MAPEDITOR.GLOBAL.DEFINES.savingOverlayRotation[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex] + "\nipr: " + MAPEDITOR.GLOBAL.DEFINES.incomingPolygonRotation[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex] + "\npr: " + MAPEDITOR.GLOBAL.DEFINES.preservedRotation);
                            //check to see if overlay is indeed a bonified overlay (on the map)
                            try {
                                //indicate we are editing
                                MAPEDITOR.GLOBAL.DEFINES.pageMode = "edit";
                                //if editing is being done and there is something to save, save
                                if (MAPEDITOR.GLOBAL.DEFINES.currentlyEditing == "yes" && MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex != null) {
                                    //reset overlay drawingmode
                                    MAPEDITOR.GLOBAL.DEFINES.drawingManager.setDrawingMode(null);
                                    //MAPEDITOR.GLOBAL.DEFINES.drawingManager.setMap(null);
                                    MAPEDITOR.TRACER.addTracer("[INFO]: saving overlay " + MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex);
                                    //trigger a cache of current working overlay
                                    MAPEDITOR.UTILITIES.cacheSaveOverlay(MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex);
                                    //set MAPEDITOR.GLOBAL.DEFINES.rectangle to MAPEDITOR.GLOBAL.DEFINES.ghosting
                                    MAPEDITOR.GLOBAL.DEFINES.ghostOverlayRectangle[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex].setOptions(MAPEDITOR.GLOBAL.DEFINES.ghosting);
                                    //reset editing marker
                                    MAPEDITOR.GLOBAL.DEFINES.currentlyEditing = "no";
                                    //set new woi
                                    MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex = id;
                                    //go through each overlay on the map
                                    MAPEDITOR.UTILITIES.cycleOverlayHighlight(id);
                                    //set preserved rotation to the rotation of the current overlay
                                    MAPEDITOR.TRACER.addTracer("[INFO]: setting preserved rotation to MAPEDITOR.GLOBAL.DEFINES.savingOverlayRotation[" + (MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex - 1) + "] (" + MAPEDITOR.GLOBAL.DEFINES.savingOverlayRotation[(MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex - 1)] + ")");
                                    MAPEDITOR.GLOBAL.DEFINES.preservedRotation = MAPEDITOR.GLOBAL.DEFINES.savingOverlayRotation[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex - 1];
                                    //MAPEDITOR.GLOBAL.DEFINES.preservedRotation = 0;
                                }
                                //if editing is not being done, make it so
                                if (MAPEDITOR.GLOBAL.DEFINES.currentlyEditing == "no" || MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex == null) {
                                    //reset overlay drawingmode
                                    MAPEDITOR.GLOBAL.DEFINES.drawingManager.setDrawingMode(null);
                                    //MAPEDITOR.GLOBAL.DEFINES.drawingManager.setMap(null);
                                    //set new woi
                                    MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex = id;
                                    //open woi
                                    MAPEDITOR.GLOBAL.DEFINES.overlaysOnMap[id].setMap(map);
                                    MAPEDITOR.GLOBAL.DEFINES.ghostOverlayRectangle[id].setMap(map);
                                    document.getElementById("overlayToggle" + id).innerHTML = "<img src=\"" + MAPEDITOR.GLOBAL.DEFINES.baseURL + MAPEDITOR.GLOBAL.DEFINES.baseImageDirURL + "sub.png\" onclick=\"MAPEDITOR.ACTIONS.overlayHideMe(" + id + ");\" />";
                                    //go through each overlay on the map
                                    MAPEDITOR.UTILITIES.cycleOverlayHighlight(id);
                                    //enable editing marker
                                    MAPEDITOR.GLOBAL.DEFINES.currentlyEditing = "yes";
                                    MAPEDITOR.TRACER.addTracer("[INFO]: editing overlay " + (MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex - 1));
                                    //get and set the preserved transparency value
                                    try {
                                        MAPEDITOR.GLOBAL.DEFINES.preservedOpacity = document.getElementById("overlay" + (MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex - 1)).style.opacity;
                                    } catch (e) {
                                        MAPEDITOR.GLOBAL.DEFINES.preservedOpacity = "0.35";
                                    }
                                    $("#overlayTransparencySlider").slider("value", MAPEDITOR.GLOBAL.DEFINES.preservedOpacity);
                                    //set rotation value
                                    try {
                                        if (MAPEDITOR.GLOBAL.DEFINES.savingOverlayRotation[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex - 1] != null) {
                                            MAPEDITOR.GLOBAL.DEFINES.preservedRotation = MAPEDITOR.GLOBAL.DEFINES.savingOverlayRotation[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex - 1];
                                            //set visual rotation knob value
                                            try {
                                                if (MAPEDITOR.GLOBAL.DEFINES.savingOverlayRotation[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex - 1] < 0) {
                                                    $('.knob').val((180 + MAPEDITOR.GLOBAL.DEFINES.savingOverlayRotation[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex - 1]) + 180).trigger('change');
                                                    //$('.knob').val(((180 + MAPEDITOR.GLOBAL.DEFINES.savingOverlayRotation[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex - 1]) + 180)).trigger('change');
                                                    //alert("setting knob to: MAPEDITOR.GLOBAL.DEFINES.savingOverlayRotation[" + (MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex - 1) + "] (" + MAPEDITOR.GLOBAL.DEFINES.savingOverlayRotation[(MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex - 1)] + ")");
                                                    MAPEDITOR.TRACER.addTracer("[INFO]: setting knob to: " + ((180 + MAPEDITOR.GLOBAL.DEFINES.savingOverlayRotation[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex - 1]) + 180));
                                                } else {
                                                    $('.knob').val(MAPEDITOR.GLOBAL.DEFINES.savingOverlayRotation[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex - 1]).trigger('change');
                                                    //alert("setting knob to: MAPEDITOR.GLOBAL.DEFINES.savingOverlayRotation[" + (MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex - 1) + "] (" + MAPEDITOR.GLOBAL.DEFINES.savingOverlayRotation[(MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex - 1)] + ")");
                                                    MAPEDITOR.TRACER.addTracer("[INFO]: setting knob to: " + MAPEDITOR.GLOBAL.DEFINES.savingOverlayRotation[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex - 1]);
                                                }
                                            } catch (e) {
                                                MAPEDITOR.TRACER.addTracer("[INFO]: rotation error catch: " + e);
                                            }
                                        } else {
                                            if (MAPEDITOR.GLOBAL.DEFINES.incomingPolygonRotation[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex - 1] != null) {
                                                MAPEDITOR.GLOBAL.DEFINES.preservedRotation = MAPEDITOR.GLOBAL.DEFINES.incomingPolygonRotation[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex - 1];
                                            }
                                            //set visual rotation knob value
                                            try {
                                                if (MAPEDITOR.GLOBAL.DEFINES.incomingPolygonRotation[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex - 1] < 0) {
                                                    //$('.knob').val(MAPEDITOR.GLOBAL.DEFINES.incomingPolygonRotation[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex - 1]).trigger('change');
                                                    $('.knob').val(((180 + MAPEDITOR.GLOBAL.DEFINES.incomingPolygonRotation[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex - 1]) + 180)).trigger('change');
                                                    //alert("setting knob to: MAPEDITOR.GLOBAL.DEFINES.incomingPolygonRotation[" + (MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex - 1) + "] (" + MAPEDITOR.GLOBAL.DEFINES.incomingPolygonRotation[(MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex - 1)] + ")");
                                                    MAPEDITOR.TRACER.addTracer("[INFO]: setting knob to: " + ((180 + MAPEDITOR.GLOBAL.DEFINES.incomingPolygonRotation[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex - 1]) + 180));
                                                } else {
                                                    $('.knob').val(MAPEDITOR.GLOBAL.DEFINES.incomingPolygonRotation[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex - 1]).trigger('change');
                                                    //alert("setting knob to: MAPEDITOR.GLOBAL.DEFINES.incomingPolygonRotation[" + (MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex - 1) + "] (" + MAPEDITOR.GLOBAL.DEFINES.incomingPolygonRotation[(MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex - 1)] + ")");
                                                    MAPEDITOR.TRACER.addTracer("[INFO]: setting knob to: " + MAPEDITOR.GLOBAL.DEFINES.incomingPolygonRotation[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex - 1]);
                                                }
                                            } catch (e) {
                                                MAPEDITOR.TRACER.addTracer("[INFO]: rotation error catch: " + e);
                                            }
                                        }
                                    } catch (e) {
                                        //could not add rotation data
                                        MAPEDITOR.TRACER.addTracer("[ERROR]: Could not add rotation data");
                                    }
                                    //show ghost
                                    MAPEDITOR.GLOBAL.DEFINES.ghostOverlayRectangle[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex].setOptions(MAPEDITOR.GLOBAL.DEFINES.editable);
                                    //iterate top z index
                                    MAPEDITOR.GLOBAL.DEFINES.currentTopZIndex++;
                                    //bring overlay to front
                                    try {
                                        document.getElementById("overlay" + (MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex - 1)).style.zIndex = MAPEDITOR.GLOBAL.DEFINES.currentTopZIndex;
                                    } catch (e) {
                                        //could not set overlay
                                        MAPEDITOR.TRACER.addTracer("[WARN]: Could not set overlay zindex");
                                    }
                                    //bring ghost to front
                                    MAPEDITOR.GLOBAL.DEFINES.ghostOverlayRectangle[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex].setOptions({ zIndex: MAPEDITOR.GLOBAL.DEFINES.currentTopZIndex });
                                    //recenter on the overlay
                                    MAPEDITOR.UTILITIES.overlayCenterOnMe(id);
                                }
                                //indicate to user we are editing a polygon
                                MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L34 + " " + MAPEDITOR.GLOBAL.DEFINES.incomingPolygonLabel[(id - 1)]);
                            } catch (e) {
                                //go through each overlay on the map
                                MAPEDITOR.UTILITIES.cycleOverlayHighlight(id);
                                //create the overlay
                                MAPEDITOR.UTILITIES.createOverlayFromPage(id);
                            }
                            MAPEDITOR.TRACER.addTracer("[INFO]: overlayEditMe completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    useSearchAsItemLocation: function () {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: useSearchAsItemLocation started...");
                            //debuggin
                            MAPEDITOR.TRACER.addTracer("[INFO]: search result: " + MAPEDITOR.GLOBAL.DEFINES.searchResult);
                            //check to see if there is a search result
                            if (MAPEDITOR.GLOBAL.DEFINES.searchResult != null) {
                                //this tells listeners what to do
                                MAPEDITOR.GLOBAL.DEFINES.placerType = "item";
                                //determine if itemmarker is already made
                                if (MAPEDITOR.GLOBAL.DEFINES.itemMarker != null) {
                                    //assign new position of marker
                                    MAPEDITOR.GLOBAL.DEFINES.itemMarker.setPosition(MAPEDITOR.GLOBAL.DEFINES.searchResult.getPosition());
                                    //delete search result
                                    MAPEDITOR.ACTIONS.searchResultDeleteMe();
                                    //display new marker
                                    MAPEDITOR.GLOBAL.DEFINES.itemMarker.setMap(map);
                                } else {
                                    //make search marker, item marker
                                    MAPEDITOR.GLOBAL.DEFINES.itemMarker = MAPEDITOR.GLOBAL.DEFINES.searchResult;
                                    //assign itemMarkerMode
                                    MAPEDITOR.GLOBAL.DEFINES.itemMarker.setOptions(MAPEDITOR.GLOBAL.DEFINES.markerOptionsItem);
                                    //assign flags
                                    MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                    MAPEDITOR.GLOBAL.DEFINES.firstSaveItem = true;
                                    //used to prevent multi markers
                                    if (MAPEDITOR.GLOBAL.DEFINES.firstMarker > 0) {
                                        MAPEDITOR.GLOBAL.DEFINES.drawingManager.setDrawingMode(null); //only place one at a time
                                    } else {
                                        MAPEDITOR.GLOBAL.DEFINES.firstMarker++;
                                        MAPEDITOR.GLOBAL.DEFINES.drawingManager.setDrawingMode(null); //only place one at a time
                                    }
                                }
                                //prevent redraw
                                MAPEDITOR.GLOBAL.DEFINES.firstMarker++;
                                //get the lat/long of item marker and put it in the item location tab
                                document.getElementById('content_toolbox_posItem').value = MAPEDITOR.GLOBAL.DEFINES.itemMarker.getPosition();
                                //get the reverse geo address for item location and put in location tab
                                MAPEDITOR.UTILITIES.codeLatLng(MAPEDITOR.GLOBAL.DEFINES.itemMarker.getPosition());
                                //store coords to save
                                MAPEDITOR.GLOBAL.DEFINES.savingMarkerCenter = MAPEDITOR.GLOBAL.DEFINES.itemMarker.getPosition();
                                //add listener for new item marker (can only add once the MAPEDITOR.GLOBAL.DEFINES.itemMarker is created)
                                google.maps.event.addListener(MAPEDITOR.GLOBAL.DEFINES.itemMarker, 'dragend', function () {
                                    //get lat/long
                                    document.getElementById('content_toolbox_posItem').value = MAPEDITOR.GLOBAL.DEFINES.itemMarker.getPosition();
                                    //get address
                                    MAPEDITOR.UTILITIES.codeLatLng(MAPEDITOR.GLOBAL.DEFINES.itemMarker.getPosition());
                                    //store coords to save
                                    MAPEDITOR.GLOBAL.DEFINES.savingMarkerCenter = MAPEDITOR.GLOBAL.DEFINES.itemMarker.getPosition();
                                    //assign flags
                                    MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                    MAPEDITOR.GLOBAL.DEFINES.firstSaveItem = true;
                                });
                            } else {
                                //nothing in search
                                MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L39);
                            }
                            MAPEDITOR.TRACER.addTracer("[INFO]: useSearchAsItemLocation completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    deleteItemLocation: function () {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: deleteItemLocation started...");
                            if (MAPEDITOR.GLOBAL.DEFINES.itemMarker) {
                                //confirm
                                MAPEDITOR.UTILITIES.confirmMessage(MAPEDITOR.LOCALIZATION.DEFINES.L70);
                                //hide marker
                                MAPEDITOR.GLOBAL.DEFINES.itemMarker.setMap(null);
                                MAPEDITOR.GLOBAL.DEFINES.itemMarker = null;
                                //msg
                                MAPEDITOR.GLOBAL.DEFINES.toServerSuccessMessage = MAPEDITOR.LOCALIZATION.DEFINES.L69;
                                //send to server and delete from mets
                                MAPEDITOR.GLOBAL.DEFINES.RIBMode = true;
                                MAPEDITOR.UTILITIES.createSavedItem("delete", null);
                                MAPEDITOR.GLOBAL.DEFINES.RIBMode = false;
                                //clear saving item center as well
                                MAPEDITOR.GLOBAL.DEFINES.savingMarkerCenter = null;
                                //explicitly disallow editing after converting
                                MAPEDITOR.GLOBAL.DEFINES.drawingManager.setDrawingMode(null);
                                //MAPEDITOR.GLOBAL.DEFINES.drawingManager.setMap(null);
                                MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = false;
                                //clear item boxes
                                document.getElementById("content_toolbox_posItem").value = null;
                                document.getElementById("content_toolbox_rgItem").value = null;
                            } else {
                                //did not delete
                                MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L_NotDeleted);
                                //explicitly disallow editing after a failed convert
                                MAPEDITOR.GLOBAL.DEFINES.drawingManager.setDrawingMode(null);
                                //MAPEDITOR.GLOBAL.DEFINES.drawingManager.setMap(null);
                            }
                            MAPEDITOR.TRACER.addTracer("[INFO]: deleteItemLocation completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    panOverlay: function (direction) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: panOverlay started...");
                            switch (direction) {
                                case "up":
                                    //calc original
                                    var neLat = MAPEDITOR.GLOBAL.DEFINES.ghostOverlayRectangle[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex].getBounds().getNorthEast().lat() + 0.00005;
                                    var neLng = MAPEDITOR.GLOBAL.DEFINES.ghostOverlayRectangle[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex].getBounds().getNorthEast().lng() + 0.00000;
                                    var swLat = MAPEDITOR.GLOBAL.DEFINES.ghostOverlayRectangle[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex].getBounds().getSouthWest().lat() + 0.00005;
                                    var swLng = MAPEDITOR.GLOBAL.DEFINES.ghostOverlayRectangle[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex].getBounds().getSouthWest().lng() + 0.00000;
                                    var bounds = new google.maps.LatLngBounds(new google.maps.LatLng(swLat, swLng), new google.maps.LatLng(neLat, neLng));
                                    var pixelPoint = MAPEDITOR.UTILITIES.latLngToPixel(new google.maps.LatLng(neLat, neLng));
                                    MAPEDITOR.TRACER.addTracer("[INFO]: x: " + pixelPoint.x);
                                    MAPEDITOR.TRACER.addTracer("[INFO]: y: " + pixelPoint.y);
                                    //assign overlay position
                                    document.getElementById("overlay" + MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex).style.position = "static";
                                    //assign ghost position
                                    MAPEDITOR.GLOBAL.DEFINES.ghostOverlayRectangle[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex].setBounds(bounds);
                                    //MAPEDITOR.UTILITIES.testBounds();
                                    break;
                                case "down":
                                    //calc original
                                    var neLat = MAPEDITOR.GLOBAL.DEFINES.ghostOverlayRectangle[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex].getBounds().getNorthEast().lat() - 0.00005;
                                    var neLng = MAPEDITOR.GLOBAL.DEFINES.ghostOverlayRectangle[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex].getBounds().getNorthEast().lng() + 0.00000;
                                    var swLat = MAPEDITOR.GLOBAL.DEFINES.ghostOverlayRectangle[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex].getBounds().getSouthWest().lat() - 0.00005;
                                    var swLng = MAPEDITOR.GLOBAL.DEFINES.ghostOverlayRectangle[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex].getBounds().getSouthWest().lng() + 0.00000;
                                    var bounds = new google.maps.LatLngBounds(new google.maps.LatLng(swLat, swLng), new google.maps.LatLng(neLat, neLng));
                                    var pixelPoint = MAPEDITOR.UTILITIES.latLngToPixel(new google.maps.LatLng(swLat, swLng));
                                    MAPEDITOR.TRACER.addTracer("[INFO]: x: " + pixelPoint.x);
                                    MAPEDITOR.TRACER.addTracer("[INFO]: y: " + pixelPoint.y);
                                    //assign overlay position
                                    document.getElementById("overlay" + MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex).style.position = "static";
                                    //assign ghost position
                                    MAPEDITOR.GLOBAL.DEFINES.ghostOverlayRectangle[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex].setBounds(bounds);
                                    //MAPEDITOR.UTILITIES.testBounds();
                                    break;
                                case "left":
                                    //calc original
                                    var neLat = MAPEDITOR.GLOBAL.DEFINES.ghostOverlayRectangle[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex].getBounds().getNorthEast().lat() + 0.00000;
                                    var neLng = MAPEDITOR.GLOBAL.DEFINES.ghostOverlayRectangle[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex].getBounds().getNorthEast().lng() - 0.00005;
                                    var swLat = MAPEDITOR.GLOBAL.DEFINES.ghostOverlayRectangle[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex].getBounds().getSouthWest().lat() + 0.00000;
                                    var swLng = MAPEDITOR.GLOBAL.DEFINES.ghostOverlayRectangle[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex].getBounds().getSouthWest().lng() - 0.00005;
                                    var bounds = new google.maps.LatLngBounds(new google.maps.LatLng(swLat, swLng), new google.maps.LatLng(neLat, neLng));
                                    var pixelPoint = MAPEDITOR.UTILITIES.latLngToPixel(new google.maps.LatLng(swLat, swLng));
                                    MAPEDITOR.TRACER.addTracer("[INFO]: x: " + pixelPoint.x);
                                    MAPEDITOR.TRACER.addTracer("[INFO]: y: " + pixelPoint.y);
                                    //assign overlay position
                                    document.getElementById("overlay" + MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex).style.position = "static";
                                    //assign ghost position
                                    MAPEDITOR.GLOBAL.DEFINES.ghostOverlayRectangle[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex].setBounds(bounds);
                                    //MAPEDITOR.UTILITIES.testBounds();
                                    break;
                                case "right":
                                    //calc original
                                    var neLat = MAPEDITOR.GLOBAL.DEFINES.ghostOverlayRectangle[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex].getBounds().getNorthEast().lat() + 0.00000;
                                    var neLng = MAPEDITOR.GLOBAL.DEFINES.ghostOverlayRectangle[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex].getBounds().getNorthEast().lng() + 0.00005;
                                    var swLat = MAPEDITOR.GLOBAL.DEFINES.ghostOverlayRectangle[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex].getBounds().getSouthWest().lat() + 0.00000;
                                    var swLng = MAPEDITOR.GLOBAL.DEFINES.ghostOverlayRectangle[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex].getBounds().getSouthWest().lng() + 0.00005;
                                    var bounds = new google.maps.LatLngBounds(new google.maps.LatLng(swLat, swLng), new google.maps.LatLng(neLat, neLng));
                                    var pixelPoint = MAPEDITOR.UTILITIES.latLngToPixel(new google.maps.LatLng(swLat, swLng));
                                    MAPEDITOR.TRACER.addTracer("[INFO]: x: " + pixelPoint.x);
                                    MAPEDITOR.TRACER.addTracer("[INFO]: y: " + pixelPoint.y);
                                    //assign overlay position
                                    document.getElementById("overlay" + MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex).style.position = "static";
                                    //assign ghost position
                                    MAPEDITOR.GLOBAL.DEFINES.ghostOverlayRectangle[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex].setBounds(bounds);
                                    //MAPEDITOR.UTILITIES.testBounds();
                                    break;
                                case "reset":
                                    //MAPEDITOR.GLOBAL.DEFINES.overlaysOnMap[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex].panBy((-1 * xaxis), (-1 * yaxis));
                                    break;
                            }
                            MAPEDITOR.TRACER.addTracer("[INFO]: panOverlay completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    convertToOverlay: function () {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: convertToOverlay started...");
                            //determine index of overlay 
                            var totalPolygonCount = 0;
                            var nonPoiCount = 0;
                            try {
                                for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.incomingPolygonFeatureType.length; i++) {
                                    totalPolygonCount++;
                                    if (MAPEDITOR.GLOBAL.DEFINES.incomingPolygonFeatureType[i] != "poi") {
                                        nonPoiCount++;
                                    }
                                }
                                MAPEDITOR.GLOBAL.DEFINES.convertedOverlayIndex = MAPEDITOR.GLOBAL.DEFINES.incomingPolygonFeatureType.length - nonPoiCount;
                                MAPEDITOR.TRACER.addTracer("[INFO]: converted overlay index: " + MAPEDITOR.GLOBAL.DEFINES.convertedOverlayIndex);
                            } catch (e) {
                                MAPEDITOR.TRACER.addTracer("[INFO]: no overlays thus pages to convert to.");
                            }

                            //if (MAPEDITOR.GLOBAL.DEFINES.itemMarker && MAPEDITOR.GLOBAL.DEFINES.incomingPointSourceURL[0] != "") {
                            if (nonPoiCount > 0) {
                                if (MAPEDITOR.GLOBAL.DEFINES.itemMarker) {
                                    //hide marker
                                    MAPEDITOR.GLOBAL.DEFINES.itemMarker.setMap(null);
                                    MAPEDITOR.GLOBAL.DEFINES.itemMarker = null;
                                    MAPEDITOR.GLOBAL.DEFINES.toServerSuccessMessage = MAPEDITOR.LOCALIZATION.DEFINES.L67;
                                    MAPEDITOR.GLOBAL.DEFINES.RIBMode = true;
                                    MAPEDITOR.UTILITIES.createSavedItem("delete", null); //send to server and delete from mets
                                    MAPEDITOR.GLOBAL.DEFINES.RIBMode = false;
                                    ////open first overlay to convert
                                    //MAPEDITOR.UTILITIES.createOverlayFromPage(MAPEDITOR.GLOBAL.DEFINES.convertedOverlayIndex + 1);
                                }
                                //switch to overlay tab
                                MAPEDITOR.UTILITIES.actionsACL("none", "item");
                                MAPEDITOR.UTILITIES.actionsACL("full", "overlay");
                                //(confirm 'main' if not already there) fixes a bug 
                                MAPEDITOR.GLOBAL.DEFINES.incomingPolygonFeatureType[MAPEDITOR.GLOBAL.DEFINES.convertedOverlayIndex] = "TEMP_main";
                                MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPolygonType[MAPEDITOR.GLOBAL.DEFINES.convertedOverlayIndex] = "TEMP_rectangle";
                                //explicitly open overlay tab (fixes bug)
                                MAPEDITOR.ACTIONS.openToolboxTab(3);
                                //converted
                                MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L44);
                                //explicitly disallow editing after converting
                                MAPEDITOR.GLOBAL.DEFINES.drawingManager.setDrawingMode(null);
                                //MAPEDITOR.GLOBAL.DEFINES.drawingManager.setMap(null);
                                MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = false;
                            } else {
                                //cannot convert
                                MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L40);
                                //explicitly disallow editing after a failed convert
                                MAPEDITOR.GLOBAL.DEFINES.drawingManager.setDrawingMode(null);
                                //MAPEDITOR.GLOBAL.DEFINES.drawingManager.setMap(null);
                            }
                            MAPEDITOR.TRACER.addTracer("[INFO]: convertToOverlay completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    openToolboxTab: function (id) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: openToolboxTab started...");
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
                                MAPEDITOR.GLOBAL.DEFINES.drawingManager.setMap(map);
                                MAPEDITOR.GLOBAL.DEFINES.drawingManager.setOptions({ drawingControl: true, drawingControlOptions: { position: google.maps.ControlPosition.RIGHT_TOP, drawingModes: [google.maps.drawing.OverlayType.MARKER, google.maps.drawing.OverlayType.CIRCLE, google.maps.drawing.OverlayType.RECTANGLE, google.maps.drawing.OverlayType.POLYGON, google.maps.drawing.OverlayType.POLYLINE], markerOptions: MAPEDITOR.GLOBAL.DEFINES.markerOptionsPOI, circleOptions: MAPEDITOR.GLOBAL.DEFINES.circleOptionsPOI, rectangleOptions: MAPEDITOR.GLOBAL.DEFINES.rectangleOptionsPOI, polygonOptions: MAPEDITOR.GLOBAL.DEFINES.polygonOptionsPOI, polylineOptions: MAPEDITOR.GLOBAL.DEFINES.polylineOptionsPOI } });
                            }
                            $("#mapedit_container_toolboxTabs").accordion({ active: id });
                            MAPEDITOR.TRACER.addTracer("[INFO]: openToolboxTab completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    keyPress: function (e) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: keypress started...");
                            //determine if we are typing in a text area
                            MAPEDITOR.UTILITIES.checkKeyCode();
                            //get keycode
                            var keycode = null;
                            if (window.event) {
                                keycode = window.event.keyCode;
                            } else if (e) {
                                keycode = e.which;
                            }
                            MAPEDITOR.TRACER.addTracer("[INFO]: key press: " + keycode);
                            //handle specific keycodes
                            switch (keycode) {
                                case 13: //enter
                                    if ("content_menubar_searchField" == document.activeElement.id) {
                                        MAPEDITOR.UTILITIES.finder(document.getElementById("content_menubar_searchField").value);
                                    }
                                    if ("content_toolbox_searchField" == document.activeElement.id) {
                                        MAPEDITOR.UTILITIES.finder(document.getElementById("content_toolbox_searchField").value);
                                    }
                                    if ("content_toolbar_searchField" == document.activeElement.id) {
                                        MAPEDITOR.UTILITIES.finder(document.getElementById("content_toolbar_searchField").value);
                                    }
                                    break;
                                case 67: //shift + C
                                    if (MAPEDITOR.GLOBAL.DEFINES.typingInTextArea == false) {
                                        if (navigator.appName == "Microsoft Internet Explorer") {
                                            var copyString = document.getElementById("cLat").innerHTML;
                                            copyString += ", " + document.getElementById("cLong").innerHTML;
                                            window.clipboardData.setData("Text", copyString);
                                            MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L19);
                                        } else {
                                            if (MAPEDITOR.GLOBAL.DEFINES.cCoordsFrozen == "no") {
                                                //freeze
                                                MAPEDITOR.GLOBAL.DEFINES.cCoordsFrozen = "yes";
                                                //MAPEDITOR.UTILITIES.stickyMessage("Coordinate viewer is frozen (to unfreeze hold the shift key + the \"F\" key)");
                                                //MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L20);
                                                MAPEDITOR.UTILITIES.stickyMessage(MAPEDITOR.LOCALIZATION.DEFINES.L20);
                                            } else {
                                                //unfreeze
                                                MAPEDITOR.GLOBAL.DEFINES.cCoordsFrozen = "no";
                                                //MAPEDITOR.UTILITIES.stickyMessage("Coordinate viewer is frozen (to unfreeze hold the shift key + the \"F\" key)");
                                                MAPEDITOR.UTILITIES.stickyMessage(MAPEDITOR.LOCALIZATION.DEFINES.L20);
                                                MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L21);
                                            }
                                        }
                                    }
                                    break;
                                    //toggle Overlays
                                case 79: //shift + O
                                    if (MAPEDITOR.GLOBAL.DEFINES.typingInTextArea == false) {
                                        MAPEDITOR.ACTIONS.toggleVis("overlays");
                                    }
                                    break;
                                    //toggle POIs
                                case 80: //shift + P
                                    if (MAPEDITOR.GLOBAL.DEFINES.typingInTextArea == false) {
                                        MAPEDITOR.ACTIONS.toggleVis("pois");
                                    }
                                    break;
                                    //toggle toolbar
                                case 82: //shift + R
                                    if (MAPEDITOR.GLOBAL.DEFINES.typingInTextArea == false) {
                                        MAPEDITOR.ACTIONS.toggleVis("toolbar");
                                    }
                                    break;
                                    //toggle toolbox
                                case 88: //shift + X
                                    if (MAPEDITOR.GLOBAL.DEFINES.typingInTextArea == false) {
                                        MAPEDITOR.ACTIONS.toggleVis("toolbox");
                                    }
                                    break;
                                    //move overlay left
                                case 97: //a
                                    if (MAPEDITOR.GLOBAL.DEFINES.typingInTextArea == false) {
                                        if (MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex != null) {
                                            try {
                                                MAPEDITOR.ACTIONS.panOverlay("left");
                                            } catch (e) {
                                                MAPEDITOR.TRACER.addTracer("[INFO]: could not pan overlay");
                                            }

                                        }
                                    }
                                    break;
                                    //move overlay up
                                case 119: //w
                                    if (MAPEDITOR.GLOBAL.DEFINES.typingInTextArea == false) {
                                        if (MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex != null) {
                                            try {
                                                MAPEDITOR.ACTIONS.panOverlay("up");
                                            } catch (e) {
                                                MAPEDITOR.TRACER.addTracer("[INFO]: could not pan overlay");
                                            }

                                        }
                                    }
                                    break;
                                    //move overlay right
                                case 100: //s
                                    if (MAPEDITOR.GLOBAL.DEFINES.typingInTextArea == false) {
                                        if (MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex != null) {
                                            try {
                                                MAPEDITOR.ACTIONS.panOverlay("right");
                                            } catch (e) {
                                                MAPEDITOR.TRACER.addTracer("[INFO]: could not pan overlay");
                                            }

                                        }
                                    }
                                    break;
                                    //move overlay down
                                case 115: //g
                                    if (MAPEDITOR.GLOBAL.DEFINES.typingInTextArea == false) {
                                        if (MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex != null) {
                                            try {
                                                MAPEDITOR.ACTIONS.panOverlay("down");
                                            } catch (e) {
                                                MAPEDITOR.TRACER.addTracer("[INFO]: could not pan overlay");
                                            }

                                        }
                                    }
                                    break;
                                    //toggle DebugPanel
                                case 68: //shift + D (for debuggin)
                                    if (MAPEDITOR.GLOBAL.DEFINES.typingInTextArea == false) {
                                        if (MAPEDITOR.GLOBAL.DEFINES.debuggerOn == true) {
                                            MAPEDITOR.TRACER.toggleTracer();
                                        } else {
                                            try {
                                                if (document.URL.indexOf("debugger") > -1) {
                                                    MAPEDITOR.GLOBAL.DEFINES.debuggerOn = true;
                                                    MAPEDITOR.TRACER.toggleTracer();
                                                }
                                            } catch(e) {
                                                //do nothing
                                            }
                                        }
                                    }
                                    break;
                            }
                            MAPEDITOR.TRACER.addTracer("[INFO]: keypress completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    }
                };
            }(),         //called by user direct interaction
            UTILITIES: function () {
                return {
                    cycleOverlayHighlight: function (id) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: cycleOverlayHighlight started...");
                            MAPEDITOR.TRACER.addTracer("[INFO]: highlighting overlays");
                            //go through each overlay on the map
                            for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.incomingPolygonSourceURL.length ; i++) {
                                MAPEDITOR.TRACER.addTracer("[INFO]: hit: " + id + " index: " + i + " length: " + MAPEDITOR.GLOBAL.DEFINES.incomingPolygonSourceURL.length);
                                //if there is a match in overlays
                                if (MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPageId[i] == id) {
                                    //set highlight color
                                    document.getElementById("overlayListItem" + MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPageId[i]).style.background = MAPEDITOR.GLOBAL.DEFINES.listItemHighlightColor;
                                } else {
                                    //reset highlight
                                    document.getElementById("overlayListItem" + MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPageId[i]).style.background = null;
                                }
                            }
                            MAPEDITOR.TRACER.addTracer("[INFO]: cycleOverlayHighlight completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },                                                 //cycle through all overlay list itmes and hightliht them accordingly
                    overlayCenterOnMe: function (id) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: overlayCenterOnMe started...");
                            //attempt to pan to center of overlay
                            map.panTo(MAPEDITOR.GLOBAL.DEFINES.ghostOverlayRectangle[id].getBounds().getCenter());
                            MAPEDITOR.TRACER.addTracer("[INFO]: overlayCenterOnMeS completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },                                                     //centeres on an overlay
                    toServer: function (dataPackage) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: toServer started...");
                            jQuery('form').each(function () {
                                var payload = JSON.stringify({ sendData: dataPackage });
                                var hiddenfield = document.getElementById('payload');
                                hiddenfield.value = payload;
                                var hiddenfield2 = document.getElementById('action');
                                hiddenfield2.value = 'save';
                                //reset success marker
                                MAPEDITOR.GLOBAL.DEFINES.toServerSuccess = false;
                                MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L_Working);
                                $.ajax({
                                    type: "POST",
                                    async: true,
                                    url: window.location.href.toString(),
                                    data: jQuery(this).serialize(),
                                    success: function (result) {
                                        //MAPEDITOR.TRACER.addTracer("[INFO]: server result:" + result);
                                        MAPEDITOR.TRACER.addTracer("[INFO]: Sallback from server - success");
                                        //MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L_Completed);
                                        //MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L_Saved);
                                        MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.GLOBAL.DEFINES.toServerSuccessMessage); //will only display last success message
                                        //reset success message
                                        MAPEDITOR.GLOBAL.DEFINES.toServerSuccessMessage = MAPEDITOR.LOCALIZATION.DEFINES.L_Completed; //take out, because it could interfere with multiple saves
                                        MAPEDITOR.GLOBAL.DEFINES.toServerSuccess = true; //not really used
                                        MAPEDITOR.GLOBAL.DEFINES.csoi = 0; //reset
                                    }
                                });
                            });
                            MAPEDITOR.TRACER.addTracer("[INFO]: toServer completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },                                                     //sends save dataPackages to the server via json
                    createSavedPOI: function (handle) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: createSavedPOI started...");
                            var dataPackage = "";
                            //cycle through all pois
                            MAPEDITOR.TRACER.addTracer("[INFO]: poi length: " + MAPEDITOR.GLOBAL.DEFINES.poiObj.length);
                            for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.poiObj.length; i++) {
                                //get specific geometry 
                                switch (MAPEDITOR.GLOBAL.DEFINES.poiType[i]) {
                                    case "marker":
                                        MAPEDITOR.GLOBAL.DEFINES.poiKML[i] = MAPEDITOR.GLOBAL.DEFINES.poiObj[i].getPosition().toString();
                                        break;
                                    case "circle":
                                        MAPEDITOR.GLOBAL.DEFINES.poiKML[i] = MAPEDITOR.GLOBAL.DEFINES.poiObj[i].getCenter() + "|";
                                        MAPEDITOR.GLOBAL.DEFINES.poiKML[i] += MAPEDITOR.GLOBAL.DEFINES.poiObj[i].getRadius();
                                        break;
                                    case "rectangle":
                                        MAPEDITOR.GLOBAL.DEFINES.poiKML[i] = MAPEDITOR.GLOBAL.DEFINES.poiObj[i].getBounds().toString();
                                        break;
                                    case "polygon":
                                        MAPEDITOR.GLOBAL.DEFINES.poiObj[i].getPath().forEach(function (latLng) {
                                            MAPEDITOR.GLOBAL.DEFINES.poiKML[i] += "|";
                                            MAPEDITOR.GLOBAL.DEFINES.poiKML[i] += latLng; //NOTE: this has a non standard coordinate separator    
                                        });
                                        break;
                                    case "polyline":
                                        MAPEDITOR.GLOBAL.DEFINES.poiObj[i].getPath().forEach(function (latLng) {
                                            MAPEDITOR.GLOBAL.DEFINES.poiKML[i] += "|";
                                            MAPEDITOR.GLOBAL.DEFINES.poiKML[i] += latLng; //NOTE: this has a non standard coordinate separator    
                                        });
                                        break;
                                    case "deleted":
                                        //nothing to do here, just a placeholder
                                        break;
                                }
                                ////filter out the deleted pois
                                //if (MAPEDITOR.GLOBAL.DEFINES.poiType[i] != "deleted") {
                                //    //compile data message
                                //    var data = handle + "|" + "poi|" + MAPEDITOR.GLOBAL.DEFINES.poiType[i] + "|" + MAPEDITOR.GLOBAL.DEFINES.poiDesc[i] + "|" + MAPEDITOR.GLOBAL.DEFINES.poiKML[i] + "|";
                                //    dataPackage += data + "~";
                                //}
                                //compile data message
                                var data = handle + "|" + "poi|" + MAPEDITOR.GLOBAL.DEFINES.poiType[i] + "|" + MAPEDITOR.GLOBAL.DEFINES.poiDesc[i] + "|" + MAPEDITOR.GLOBAL.DEFINES.poiKML[i] + "|";
                                dataPackage += data + "~";
                            }
                            MAPEDITOR.TRACER.addTracer("[INFO]: saving poi set: " + dataPackage); //temp  
                            //add another filter to catch if datapackage is empty
                            if (dataPackage != "") {
                                MAPEDITOR.UTILITIES.toServer(dataPackage); //send to server to save    
                            }
                            MAPEDITOR.TRACER.addTracer("[INFO]: createSavedPOI completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },                                                    //create a package to send to the server to save poi
                    createSavedOverlay: function (handle, pageId, label, source, bounds, rotation) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: createSavedOverlay started...");
                            var temp = source;
                            if (temp.contains("~") || temp.contains("|")) { //check to make sure reserve characters are not there
                                MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L7);
                            }
                            //var formattedBounds = 
                            var messageType = handle + "|" + "overlay"; //define what message type it is
                            var data = messageType + "|" + pageId + "|" + label + "|" + bounds + "|" + source + "|" + rotation + "|";
                            var dataPackage = data + "~";
                            MAPEDITOR.TRACER.addTracer("[INFO]: saving overlay set: " + dataPackage); //temp
                            MAPEDITOR.UTILITIES.toServer(dataPackage);
                            MAPEDITOR.TRACER.addTracer("[INFO]: createSavedOverlay completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },       //create a package to send to server to save overlay
                    createSavedItem: function (handle, coordinates) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: createSavedItem started...");
                            var messageType = handle + "|" + "item"; //define what message type it is
                            //assign data
                            var data = messageType + "|" + coordinates + "|";
                            var dataPackage = data + "~";
                            MAPEDITOR.TRACER.addTracer("[INFO]: saving item: " + dataPackage); //temp
                            MAPEDITOR.UTILITIES.toServer(dataPackage);
                            MAPEDITOR.TRACER.addTracer("[INFO]: createSavedItem completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },                                      //create a package to send to server to save item
                    stickyMessage: function (stickyMessage) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: stickyMessage started...");
                            //debug log this message
                            MAPEDITOR.TRACER.addTracer("[INFO]: sticky message #" + MAPEDITOR.GLOBAL.DEFINES.stickyMessageCount + ": " + stickyMessage); //send to debugger for logging
                            MAPEDITOR.TRACER.addTracer("[INFO]: sticky message Count: " + MAPEDITOR.GLOBAL.DEFINES.stickyMessageCount);

                            var duplicateStickyMessage = false;

                            try {
                                if (document.getElementById("stickyMessage" + MAPEDITOR.GLOBAL.DEFINES.stickyMessageCount).innerHTML == stickyMessage) {
                                    duplicateStickyMessage = true;
                                } else {
                                    duplicateStickyMessage = false;
                                }
                            } catch (e) {
                                //could not find the ID
                                MAPEDITOR.TRACER.addTracer("[INFO]: Could not find sticky message ID");
                                duplicateStickyMessage = false;
                            }

                            if (duplicateStickyMessage) {
                                MAPEDITOR.TRACER.addTracer("[INFO]: same stick message as before, deleting...");

                                //remove that sticky message from the dom
                                $("#" + "stickyMessage" + MAPEDITOR.GLOBAL.DEFINES.stickyMessageCount).remove();

                                //remove that sticky message from the record
                                MAPEDITOR.GLOBAL.DEFINES.stickyMessageCount--;

                                MAPEDITOR.TRACER.addTracer("[INFO]: new sticky message Count: " + MAPEDITOR.GLOBAL.DEFINES.stickyMessageCount);
                            } else {
                                MAPEDITOR.TRACER.addTracer("[INFO]: create sticky message");

                                //keep a count of messages
                                MAPEDITOR.GLOBAL.DEFINES.stickymessageCount++;

                                //compile divID
                                var currentDivId = "stickyMessage" + MAPEDITOR.GLOBAL.DEFINES.stickyMessageCount;

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
                            MAPEDITOR.TRACER.addTracer("[INFO]: stickyMessage completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },                                              //display an inline sticky message (does not go away until duplicate is sent in)
                    displayMessage: function (message) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: displayMessage started...");
                            //debug log this message
                            MAPEDITOR.TRACER.addTracer("[INFO]: message #" + MAPEDITOR.GLOBAL.DEFINES.messageCount + ": " + message); //send to debugger for logging

                            //keep a count of messages
                            MAPEDITOR.GLOBAL.DEFINES.messageCount++;

                            //check to see if RIB is on
                            if (MAPEDITOR.GLOBAL.DEFINES.RIBMode == true) {
                                MAPEDITOR.TRACER.addTracer("[INFO]: RIB Mode: " + MAPEDITOR.GLOBAL.DEFINES.RIBMode);
                                return;
                            } else {
                                //display the message

                                //debug
                                MAPEDITOR.TRACER.addTracer("[INFO]: RIB Mode: " + MAPEDITOR.GLOBAL.DEFINES.RIBMode);

                                //compile divID
                                var currentDivId = "message" + MAPEDITOR.GLOBAL.DEFINES.messageCount;

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
                                    if (document.getElementById("message" + (MAPEDITOR.GLOBAL.DEFINES.messageCount - 1)).innerHTML == message) {
                                        duplicateMessage = true;
                                    }
                                } catch (e) {
                                    //
                                }

                                if (duplicateMessage) {
                                    MAPEDITOR.TRACER.addTracer("[INFO]: Same message to display as previous, not displaying");
                                    //remove the previous
                                    $("#" + "message" + (MAPEDITOR.GLOBAL.DEFINES.messageCount - 1)).remove();
                                    //display the new
                                    document.getElementById(currentDivId).style.display = "block"; //display element
                                    //fade message out
                                    setTimeout(function () {
                                        $("#" + currentDivId).fadeOut("slow", function () {
                                            $("#" + currentDivId).remove();
                                        });
                                    }, 3000); //after 3 sec
                                } else {
                                    //MAPEDITOR.TRACER.addTracer("[INFO]: Unique message to display");
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
                            MAPEDITOR.TRACER.addTracer("[INFO]: displayMessage completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },                                                   //display an inline message
                    buttonActive: function (id) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: buttonActive started...");
                            MAPEDITOR.TRACER.addTracer("[INFO]: buttonActive: " + id);
                            switch (id) {
                                case "mapControls":
                                    if (MAPEDITOR.GLOBAL.DEFINES.mapControlsDisplayed == false) { //not present
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
                                    if (MAPEDITOR.GLOBAL.DEFINES.toolboxDisplayed == false) { //not present
                                        document.getElementById("content_menubar_toggleToolbox").className = document.getElementById("content_menubar_toggleToolbox").className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
                                        document.getElementById("content_toolbar_button_toggleToolbox").className = document.getElementById("content_toolbar_button_toggleToolbox").className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                                    } else { //present
                                        document.getElementById("content_menubar_toggleToolbox").className += " isActive2";
                                        document.getElementById("content_toolbar_button_toggleToolbox").className += " isActive";
                                    }
                                    break;
                                case "toolbar":
                                    if (MAPEDITOR.GLOBAL.DEFINES.toolbarDisplayed == false) { //not present
                                        document.getElementById("content_menubar_toggleToolbar").className = document.getElementById("content_menubar_toggleToolbar").className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
                                    } else { //present
                                        document.getElementById("content_menubar_toggleToolbar").className += " isActive2";
                                    }
                                    break;
                                case "layer":
                                    if (MAPEDITOR.GLOBAL.DEFINES.prevMapLayerActive != null) {
                                        document.getElementById("content_menubar_layer" + MAPEDITOR.GLOBAL.DEFINES.prevMapLayerActive).className = document.getElementById("content_menubar_layer" + MAPEDITOR.GLOBAL.DEFINES.prevMapLayerActive).className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
                                        document.getElementById("content_toolbar_button_layer" + MAPEDITOR.GLOBAL.DEFINES.prevMapLayerActive).className = document.getElementById("content_toolbar_button_layer" + MAPEDITOR.GLOBAL.DEFINES.prevMapLayerActive).className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                                        document.getElementById("content_toolbox_button_layer" + MAPEDITOR.GLOBAL.DEFINES.prevMapLayerActive).className = document.getElementById("content_toolbox_button_layer" + MAPEDITOR.GLOBAL.DEFINES.prevMapLayerActive).className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                                    }
                                    document.getElementById("content_menubar_layer" + MAPEDITOR.GLOBAL.DEFINES.mapLayerActive).className += " isActive2";
                                    document.getElementById("content_toolbar_button_layer" + MAPEDITOR.GLOBAL.DEFINES.mapLayerActive).className += " isActive";
                                    document.getElementById("content_toolbox_button_layer" + MAPEDITOR.GLOBAL.DEFINES.mapLayerActive).className += " isActive";
                                    MAPEDITOR.GLOBAL.DEFINES.prevMapLayerActive = MAPEDITOR.GLOBAL.DEFINES.mapLayerActive; //set and hold the previous map layer active
                                    break;
                                case "kml":
                                    if (MAPEDITOR.GLOBAL.DEFINES.kmlDisplayed == false) { //not present
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
                                    MAPEDITOR.TRACER.addTracer("[INFO]: aa: " + MAPEDITOR.GLOBAL.DEFINES.actionActive + "<br>" + "paa: " + MAPEDITOR.GLOBAL.DEFINES.prevActionActive);
                                    if (MAPEDITOR.GLOBAL.DEFINES.actionActive == "Other") {
                                        try {
                                            if (MAPEDITOR.GLOBAL.DEFINES.prevActionActive != null) {
                                                document.getElementById("content_menubar_manage" + MAPEDITOR.GLOBAL.DEFINES.prevActionActive).className = document.getElementById("content_menubar_manage" + MAPEDITOR.GLOBAL.DEFINES.prevActionActive).className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
                                                document.getElementById("content_toolbar_button_manage" + MAPEDITOR.GLOBAL.DEFINES.prevActionActive).className = document.getElementById("content_toolbar_button_manage" + MAPEDITOR.GLOBAL.DEFINES.prevActionActive).className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                                                document.getElementById("content_toolbox_button_manage" + MAPEDITOR.GLOBAL.DEFINES.prevActionActive).className = document.getElementById("content_toolbox_button_manage" + MAPEDITOR.GLOBAL.DEFINES.prevActionActive).className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                                            }
                                        } catch (e) {
                                            MAPEDITOR.TRACER.addTracer("[ERROR]: \"" + e + "\" (Could not find classname)");
                                        }
                                    } else {
                                        if (MAPEDITOR.GLOBAL.DEFINES.prevActionActive != null) {
                                            document.getElementById("content_menubar_manage" + MAPEDITOR.GLOBAL.DEFINES.prevActionActive).className = document.getElementById("content_menubar_manage" + MAPEDITOR.GLOBAL.DEFINES.prevActionActive).className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
                                            document.getElementById("content_toolbar_button_manage" + MAPEDITOR.GLOBAL.DEFINES.prevActionActive).className = document.getElementById("content_toolbar_button_manage" + MAPEDITOR.GLOBAL.DEFINES.prevActionActive).className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                                            if (document.getElementById("content_toolbox_button_manage" + MAPEDITOR.GLOBAL.DEFINES.prevActionActive)) {
                                                MAPEDITOR.TRACER.addTracer("[INFO]: found " + MAPEDITOR.GLOBAL.DEFINES.prevActionActive);
                                                document.getElementById("content_toolbox_button_manage" + MAPEDITOR.GLOBAL.DEFINES.prevActionActive).className = document.getElementById("content_toolbox_button_manage" + MAPEDITOR.GLOBAL.DEFINES.prevActionActive).className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                                            }

                                        }
                                        document.getElementById("content_menubar_manage" + MAPEDITOR.GLOBAL.DEFINES.actionActive).className += " isActive2";
                                        document.getElementById("content_toolbar_button_manage" + MAPEDITOR.GLOBAL.DEFINES.actionActive).className += " isActive";
                                        if (document.getElementById("content_toolbox_button_manage" + MAPEDITOR.GLOBAL.DEFINES.actionActive)) {
                                            MAPEDITOR.TRACER.addTracer("[INFO]: found " + MAPEDITOR.GLOBAL.DEFINES.actionActive);
                                            document.getElementById("content_toolbox_button_manage" + MAPEDITOR.GLOBAL.DEFINES.actionActive).className += " isActive";
                                        }
                                        MAPEDITOR.GLOBAL.DEFINES.prevActionActive = MAPEDITOR.GLOBAL.DEFINES.actionActive; //set and hold the previous map layer active
                                    }
                                    break;
                                case "overlayToggle":
                                    if (MAPEDITOR.GLOBAL.DEFINES.buttonActive_overlayToggle == false) { //not present
                                        document.getElementById("content_menubar_overlayToggle").className += " isActive2";
                                        document.getElementById("content_toolbox_button_overlayToggle").className += " isActive";
                                        MAPEDITOR.GLOBAL.DEFINES.buttonActive_overlayToggle = true;
                                    } else { //present
                                        document.getElementById("content_menubar_overlayToggle").className = document.getElementById("content_menubar_overlayToggle").className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
                                        document.getElementById("content_toolbox_button_overlayToggle").className = document.getElementById("content_toolbox_button_overlayToggle").className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                                        MAPEDITOR.GLOBAL.DEFINES.buttonActive_overlayToggle = false;
                                    }
                                    break;
                                case "poiToggle":
                                    if (MAPEDITOR.GLOBAL.DEFINES.buttonActive_poiToggle == false) { //not present
                                        document.getElementById("content_menubar_poiToggle").className += " isActive2";
                                        document.getElementById("content_toolbox_button_poiToggle").className += " isActive";
                                        MAPEDITOR.GLOBAL.DEFINES.buttonActive_poiToggle = true;
                                    } else { //present
                                        document.getElementById("content_menubar_poiToggle").className = document.getElementById("content_menubar_poiToggle").className.replace(/(?:^|\s)isActive2(?!\S)/g, '');
                                        document.getElementById("content_toolbox_button_poiToggle").className = document.getElementById("content_toolbox_button_poiToggle").className.replace(/(?:^|\s)isActive(?!\S)/g, '');
                                        MAPEDITOR.GLOBAL.DEFINES.buttonActive_poiToggle = false;
                                    }
                                    break;
                            }
                            MAPEDITOR.TRACER.addTracer("[INFO]: buttonAction completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },                                                          //facilitates button sticky effect
                    confirmMessage: function (message) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: confirmMessage started...");
                            //todo make this a better messagebox (visually pleasing and auto cancel if no action taken)
                            var response = confirm(message);
                            MAPEDITOR.TRACER.addTracer("[INFO]: confirmMessage completed...");
                            return response;
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                            return false;
                        }
                    },                                                   //This triggers a confirm messagebox
                    polygonCenter: function (poly) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: polygonCenter started...");
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
                            MAPEDITOR.TRACER.addTracer("[INFO]: polygonCenter completed...");
                            return (new google.maps.LatLng(center_x, center_y));
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    rotate: function (degreeIn) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: rotate started...");
                            if (MAPEDITOR.GLOBAL.DEFINES.currentlyEditing == "yes") {
                                //if (MAPEDITOR.GLOBAL.DEFINES.pageMode == "edit") {
                                MAPEDITOR.GLOBAL.DEFINES.currentlyEditing = "yes"; //used to signify we are editing this overlay
                                MAPEDITOR.GLOBAL.DEFINES.degree = MAPEDITOR.GLOBAL.DEFINES.preservedRotation;
                                MAPEDITOR.GLOBAL.DEFINES.degree += degreeIn;
                                if (degreeIn != 0) {
                                    $(function () {
                                        $("#overlay" + MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex).rotate(MAPEDITOR.GLOBAL.DEFINES.degree); //assign overlay to defined rotation
                                    });
                                } else { //if rotation is 0, reset rotation
                                    $(function () {
                                        MAPEDITOR.GLOBAL.DEFINES.degree = 0;
                                        $("#overlay" + MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex).rotate(MAPEDITOR.GLOBAL.DEFINES.degree);
                                    });
                                }
                                MAPEDITOR.GLOBAL.DEFINES.preservedRotation = MAPEDITOR.GLOBAL.DEFINES.degree; //preserve rotation value
                                //MAPEDITOR.GLOBAL.DEFINES.savingOverlayRotation[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex] = MAPEDITOR.GLOBAL.DEFINES.preservedRotation; //just make sure it is prepping for save
                            }
                            if (MAPEDITOR.GLOBAL.DEFINES.degree > 180) {
                                $('.knob').val(((MAPEDITOR.GLOBAL.DEFINES.degree - 360) * (1))).trigger('change'); //used to correct for visual effect of knob error

                            } else {
                                $('.knob').val(MAPEDITOR.GLOBAL.DEFINES.degree).trigger('change');
                            }
                            MAPEDITOR.TRACER.addTracer("[INFO]: rotate completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    keepRotate: function (degreeIn) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: keepRotate started...");
                            MAPEDITOR.TRACER.addTracer("[INFO]: keepRotate: " + degreeIn + " woi: " + MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex);
                            MAPEDITOR.GLOBAL.DEFINES.currentlyEditing = "yes"; //used to signify we are editing this overlay
                            $("#overlay" + MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex).rotate(degreeIn);
                            if (degreeIn < 0) {
                                $('.knob').val(((180 + degreeIn) + 180)).trigger('change');
                            } else {
                                $('.knob').val(degreeIn).trigger('change');
                            }
                            MAPEDITOR.TRACER.addTracer("[INFO]: keepRotate completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    opacity: function (opacityIn) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: opacity started...");
                            if (MAPEDITOR.GLOBAL.DEFINES.preservedOpacity <= 1 && MAPEDITOR.GLOBAL.DEFINES.preservedOpacity >= 0) {
                                MAPEDITOR.TRACER.addTracer("[INFO]: add opacity: " + opacityIn + " to overlay" + MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex);
                                var div = document.getElementById("overlay" + MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex);
                                var newOpacity = MAPEDITOR.GLOBAL.DEFINES.preservedOpacity + opacityIn;
                                if (newOpacity > 1) {
                                    newOpacity = 1;
                                }
                                if (newOpacity < 0) {
                                    newOpacity = 0;
                                }
                                div.style.opacity = newOpacity;
                                MAPEDITOR.TRACER.addTracer("[INFO]: newOpacity: " + newOpacity);
                                MAPEDITOR.GLOBAL.DEFINES.preservedOpacity = newOpacity;
                                MAPEDITOR.TRACER.addTracer("[INFO]: MAPEDITOR.GLOBAL.DEFINES.preservedOpacity: " + MAPEDITOR.GLOBAL.DEFINES.preservedOpacity);
                                $("#overlayTransparencySlider").slider({ value: MAPEDITOR.GLOBAL.DEFINES.preservedOpacity });
                            } else {
                                //could not change the opacity    
                            }
                            MAPEDITOR.TRACER.addTracer("[INFO]: opacity completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    keepOpacity: function (opacityIn) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: keepOpacity started...");
                            MAPEDITOR.TRACER.addTracer("[INFO]: keepOpacity: " + opacityIn);
                            try {
                                var div = document.getElementById("overlay" + MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex);
                                div.style.opacity = opacityIn;
                            } catch (e) {
                                //
                            }
                            MAPEDITOR.GLOBAL.DEFINES.preservedOpacity = opacityIn;
                            MAPEDITOR.TRACER.addTracer("[INFO]: keepOpacity completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    checkZoomLevel: function () {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: checkZoomLevel started...");
                            var currentZoomLevel = map.getZoom();
                            var currentMapType = map.getMapTypeId();
                            if (currentZoomLevel == MAPEDITOR.GLOBAL.DEFINES.maxZoomLevel) {
                                MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L16);
                            } else {
                                switch (currentMapType) {
                                    case "roadmap": //roadmap and default
                                        if (currentZoomLevel == MAPEDITOR.GLOBAL.DEFINES.minZoomLevel_Roadmap) {
                                            MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L17);
                                        }
                                        break;
                                    case "satellite": //sat
                                        if (currentZoomLevel == MAPEDITOR.GLOBAL.DEFINES.minZoomLevel_Satellite) {
                                            MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L17);
                                        }
                                        break;
                                    case "hybrid": //sat
                                        if (currentZoomLevel == MAPEDITOR.GLOBAL.DEFINES.minZoomLevel_Satellite) {
                                            MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L17);
                                        }
                                        break;
                                    case "terrain": //terrain only
                                        if (currentZoomLevel == MAPEDITOR.GLOBAL.DEFINES.minZoomLevel_Terrain) {
                                            MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L17);
                                        }
                                        break;
                                    case "blocklot":
                                        if (currentZoomLevel == MAPEDITOR.GLOBAL.DEFINES.minZoomLevel_BlockLot) {
                                            MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L17);
                                        }
                                        break;
                                }
                                if (MAPEDITOR.GLOBAL.DEFINES.isCustomOverlay == true) {
                                    if (currentZoomLevel == MAPEDITOR.GLOBAL.DEFINES.minZoomLevel_BlockLot) {
                                        MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L17);
                                    }
                                }
                            }
                            MAPEDITOR.TRACER.addTracer("[INFO]: checkZoomLevel completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    displayCursorCoords: function (arg) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: displayCursorCoords started...");
                            document.getElementById("cCoord").innerHTML = arg;
                            MAPEDITOR.TRACER.addTracer("[INFO]: displayCursorCoords completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    codeLatLng: function (latlng) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: codeLatLng started...");
                            if (MAPEDITOR.GLOBAL.DEFINES.geocoder) {
                                MAPEDITOR.GLOBAL.DEFINES.geocoder.geocode({ 'latLng': latlng }, function (results, status) {
                                    if (status == google.maps.GeocoderStatus.OK) {
                                        if (results[1]) {
                                            document.getElementById("content_toolbox_rgItem").value = results[0].formatted_address;
                                        }
                                        else {
                                            MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L25 + " " + status);
                                            document.getElementById("content_toolbox_rgItem").value = "";
                                        }
                                    }
                                });
                            }
                            MAPEDITOR.TRACER.addTracer("[INFO]: codeLatLng completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    codeAddress: function (type, geo) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: codeAddress started...");
                            var bounds = map.getBounds(); //get the current map bounds (should not be greater than the bounding box)
                            MAPEDITOR.GLOBAL.DEFINES.geocoder.geocode({ 'address': geo, 'bounds': bounds }, function (results, status) { //geocode the lat/long of incoming with a bias towards the bounds
                                if (status == google.maps.GeocoderStatus.OK) { //if it worked
                                    map.setCenter(results[0].geometry.location); //set the center of map to the results
                                    MAPEDITOR.UTILITIES.testBounds(); //test to make sure we are indeed in the bounds (have to do this because gmaps only allows for a BIAS of bounds and is not strict)
                                    if (MAPEDITOR.GLOBAL.DEFINES.mapInBounds == "yes") { //if it is inside bounds
                                        if (MAPEDITOR.GLOBAL.DEFINES.searchCount > 0) { //check to see if this is the first time searched, if not
                                            MAPEDITOR.GLOBAL.DEFINES.searchResult.setMap(null); //set old search result to not display on map
                                        } else { //if it is the first time
                                            MAPEDITOR.GLOBAL.DEFINES.searchCount++; //just interate
                                        }
                                        MAPEDITOR.GLOBAL.DEFINES.searchResult = new google.maps.Marker({ //create a new marker
                                            map: map, //add to current map
                                            position: results[0].geometry.location //set position to search results
                                        });
                                        var searchResult_i = 1; //temp, placeholder for later multi search result support
                                        document.getElementById("searchResults_list").innerHTML = MAPEDITOR.UTILITIES.writeHTML("searchResultListItem", searchResult_i, geo, "", "");
                                    } else { //if location found was outside strict map bounds...
                                        MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L24); //say so
                                    }

                                } else { //if the entire geocode did not work
                                    alert(MAPEDITOR.LOCALIZATION.DEFINES.L6); //MAPEDITOR.LOCALIZATION.DEFINES...
                                    MAPEDITOR.TRACER.addTracer(MAPEDITOR.LOCALIZATION.DEFINES.L6);
                                }
                            });
                            MAPEDITOR.TRACER.addTracer("[INFO]: codeAddress completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    finder: function (stuff) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: Finder started...");
                            if (stuff.length > 0) {
                                MAPEDITOR.UTILITIES.codeAddress("lookup", stuff); //find the thing
                                document.getElementById("content_menubar_searchField").value = stuff; //sync menubar
                                document.getElementById("content_toolbar_searchField").value = stuff; //sync toolbar
                                document.getElementById("content_toolbox_searchField").value = stuff; //sync toolbox
                                MAPEDITOR.ACTIONS.action("other"); //needed to clear out any action buttons that may be active
                                MAPEDITOR.TRACER.addTracer("[INFO]: opening");
                                MAPEDITOR.ACTIONS.openToolboxTab(1); //open the actions tab
                                MAPEDITOR.TRACER.addTracer("[INFO]: supposedly opened");
                            } else {
                                //do nothing and keep quiet
                                MAPEDITOR.TRACER.addTracer("[INFO]: no stuff");
                            }
                            MAPEDITOR.TRACER.addTracer("[INFO]: Finder completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    testBounds: function () {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: testBounds started...");
                            if (MAPEDITOR.GLOBAL.DEFINES.strictBounds != null) {
                                if (MAPEDITOR.GLOBAL.DEFINES.strictBounds.contains(map.getCenter())) {
                                    MAPEDITOR.GLOBAL.DEFINES.mapInBounds = "yes";
                                } else {
                                    MAPEDITOR.GLOBAL.DEFINES.mapInBounds = "no";
                                    map.panTo(MAPEDITOR.GLOBAL.DEFINES.mapCenter); //recenter
                                    MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L5);
                                }
                            }
                            MAPEDITOR.TRACER.addTracer("[INFO]: testBounds completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    createOverlayFromPage: function (pageId) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: createOverlayFromPage started...");
                            //assign convertedoverlay index
                            MAPEDITOR.TRACER.addTracer("[INFO]: previous MAPEDITOR.GLOBAL.DEFINES.convertedOverlayIndex: " + MAPEDITOR.GLOBAL.DEFINES.convertedOverlayIndex);
                            MAPEDITOR.GLOBAL.DEFINES.convertedOverlayIndex = pageId - 1;
                            //select the area to draw the overlay
                            MAPEDITOR.UTILITIES.displayMessage(MAPEDITOR.LOCALIZATION.DEFINES.L41);
                            //define drawing manager todo: move this to a var to prevent future confusion. 
                            MAPEDITOR.GLOBAL.DEFINES.drawingManager.setOptions({ drawingControl: false, drawingControlOptions: { position: google.maps.ControlPosition.RIGHT_TOP, drawingModes: [google.maps.drawing.OverlayType.RECTANGLE] }, rectangleOptions: MAPEDITOR.GLOBAL.DEFINES.rectangleOptionsOverlay });
                            //set drawingmode to rectangle
                            MAPEDITOR.GLOBAL.DEFINES.drawingManager.setDrawingMode(google.maps.drawing.OverlayType.RECTANGLE);
                            //apply the changes
                            MAPEDITOR.GLOBAL.DEFINES.drawingManager.setMap(map);
                            //go ahead and auto switch to editing mode
                            MAPEDITOR.GLOBAL.DEFINES.pageMode = "edit";
                            //indicate overlay placer
                            MAPEDITOR.GLOBAL.DEFINES.placerType = "overlay";
                            //indicate that we drew this overlay
                            MAPEDITOR.GLOBAL.DEFINES.overlayType = "drawn";
                            //assign featuretype
                            MAPEDITOR.GLOBAL.DEFINES.incomingPolygonFeatureType[MAPEDITOR.GLOBAL.DEFINES.convertedOverlayIndex] = "TEMP_main";
                            //assign polygon type
                            MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPolygonType[MAPEDITOR.GLOBAL.DEFINES.convertedOverlayIndex] = "TEMP_rectangle";
                            //add the rotation
                            MAPEDITOR.GLOBAL.DEFINES.incomingPolygonRotation[MAPEDITOR.GLOBAL.DEFINES.convertedOverlayIndex] = 0;
                            //add the working overlay index
                            MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex = MAPEDITOR.GLOBAL.DEFINES.convertedOverlayIndex + 1;
                            ////add the working overlay index
                            //if (MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex == null) {
                            //    MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex = 0;
                            //} else {
                            //    MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex++;
                            //}
                            //mark that there is at least one overlay on map
                            MAPEDITOR.GLOBAL.DEFINES.overlaysCurrentlyDisplayed = true;
                            //flag that user may lose data
                            MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                            MAPEDITOR.TRACER.addTracer("[INFO]: createOverlayFromPage completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    actionsACL: function (level, id) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: ActionsACL started...");
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
                            MAPEDITOR.TRACER.addTracer("[INFO]: actionsACL completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    writeHTML: function (type, param1, param2, param3) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: writeHTML started...");
                            var htmlString = "";
                            switch (type) {
                                case "poiListItem":
                                    MAPEDITOR.TRACER.addTracer("[INFO]: Creating html String");
                                    MAPEDITOR.GLOBAL.DEFINES.poiDesc[param1] = "New" + param3 + param2;
                                    htmlString = "<div id=\"poi" + param1 + "\" class=\"poiListItem\" title=\"" + MAPEDITOR.GLOBAL.DEFINES.poiDesc[param1] + " \">" + MAPEDITOR.GLOBAL.DEFINES.poiDesc[param1] + " <div class=\"poiActionButton\"><a title=\"" + MAPEDITOR.LOCALIZATION.DEFINES.L66 + "\" href=\"#\" onclick=\"MAPEDITOR.ACTIONS.poiEditMe(" + param1 + ");\"><img src=\"" + MAPEDITOR.GLOBAL.DEFINES.baseURL + MAPEDITOR.GLOBAL.DEFINES.baseImageDirURL + "edit.png\"/></a> <a title=\"" + MAPEDITOR.LOCALIZATION.DEFINES.L61 + "\" id=\"poiToggle" + param1 + "\" href=\"#\"><img src=\"" + MAPEDITOR.GLOBAL.DEFINES.baseURL + MAPEDITOR.GLOBAL.DEFINES.baseImageDirURL + "sub.png\" onclick=\"MAPEDITOR.ACTIONS.poiHideMe(" + param1 + ");\" /></a> <a title=\"" + MAPEDITOR.LOCALIZATION.DEFINES.L63 + "\" href=\"#\" onclick=\"MAPEDITOR.ACTIONS.poiDeleteMe(" + param1 + ");\"><img src=\"" + MAPEDITOR.GLOBAL.DEFINES.baseURL + MAPEDITOR.GLOBAL.DEFINES.baseImageDirURL + "delete.png\"/></a></div></div>";
                                    break;
                                case "poiListItemIncoming":
                                    MAPEDITOR.TRACER.addTracer("[INFO]: Creating html String");
                                    MAPEDITOR.GLOBAL.DEFINES.poiDesc[param1] = param3;
                                    htmlString = "<div id=\"poi" + param1 + "\" class=\"poiListItem\" title=\"" + param3 + " \">" + param3 + " <div class=\"poiActionButton\"><a title=\"" + MAPEDITOR.LOCALIZATION.DEFINES.L66 + "\" href=\"#\" onclick=\"MAPEDITOR.ACTIONS.poiEditMe(" + param1 + ");\"><img src=\"" + MAPEDITOR.GLOBAL.DEFINES.baseURL + MAPEDITOR.GLOBAL.DEFINES.baseImageDirURL + "edit.png\"/></a> <a title=\"" + MAPEDITOR.LOCALIZATION.DEFINES.L61 + "\" id=\"poiToggle" + param1 + "\" href=\"#\"><img src=\"" + MAPEDITOR.GLOBAL.DEFINES.baseURL + MAPEDITOR.GLOBAL.DEFINES.baseImageDirURL + "sub.png\" onclick=\"MAPEDITOR.ACTIONS.poiHideMe(" + param1 + ");\" /></a> <a title=\"" + MAPEDITOR.LOCALIZATION.DEFINES.L63 + "\" href=\"#\" onclick=\"MAPEDITOR.ACTIONS.poiDeleteMe(" + param1 + ");\"><img src=\"" + MAPEDITOR.GLOBAL.DEFINES.baseURL + MAPEDITOR.GLOBAL.DEFINES.baseImageDirURL + "delete.png\"/></a></div></div>";
                                    break;
                                case "poiDesc":
                                    MAPEDITOR.TRACER.addTracer("[INFO]: Creating html String");
                                    htmlString = "<div class=\"poiDescContainer\"> <textarea id=\"poiDesc" + param1 + "\" class=\"descPOI\" placeholder=\"" + MAPEDITOR.LOCALIZATION.DEFINES.L3 + "\" onblur=\"MAPEDITOR.ACTIONS.poiGetDesc(" + param1 + ");\"></textarea> <br/> <div class=\"buttonPOIDesc\" id=\"poiGetDesc\" onClick=\"MAPEDITOR.ACTIONS.poiGetDesc(" + param1 + ");\" >" + MAPEDITOR.LOCALIZATION.DEFINES.L71 + "</div> </div>"; //title=\"" + MAPEDITOR.LOCALIZATION.DEFINES.L65 + "\"
                                    break;
                                case "poiDescIncoming":
                                    MAPEDITOR.TRACER.addTracer("[INFO]: Creating html String");
                                    htmlString = "<div class=\"poiDescContainer\"> <textarea id=\"poiDesc" + param1 + "\" class=\"descPOI\" onblur=\"MAPEDITOR.ACTIONS.poiGetDesc(" + param1 + ");\">" + param2 + "</textarea> <br/> <div class=\"buttonPOIDesc\" id=\"poiGetDesc\" onClick=\"MAPEDITOR.ACTIONS.poiGetDesc(" + param1 + ");\" >" + MAPEDITOR.LOCALIZATION.DEFINES.L71 + "</div> </div>"; //title=\"" + MAPEDITOR.LOCALIZATION.DEFINES.L65 + "\"
                                    break;
                                case "overlayListItem":
                                    MAPEDITOR.TRACER.addTracer("[INFO]: Creating html String");
                                    htmlString = "<div id=\"overlayListItem" + param1 + "\" class=\"overlayListItem\" title=\"" + param2 + "\"> " + param2.substring(0, 20) + " <div class=\"overlayActionButton\"><a title=\"" + MAPEDITOR.LOCALIZATION.DEFINES.L60 + "\" href=\"#\"><img src=\"" + MAPEDITOR.GLOBAL.DEFINES.baseURL + MAPEDITOR.GLOBAL.DEFINES.baseImageDirURL + "edit.png\" onclick=\"MAPEDITOR.ACTIONS.overlayEditMe(" + param1 + ");\"/></a> <a id=\"overlayToggle" + param1 + "\" href=\"#\" title=\"" + MAPEDITOR.LOCALIZATION.DEFINES.L61 + "\" ><img src=\"" + MAPEDITOR.GLOBAL.DEFINES.baseURL + MAPEDITOR.GLOBAL.DEFINES.baseImageDirURL + "sub.png\" onclick=\"MAPEDITOR.ACTIONS.overlayHideMe(" + param1 + ");\" /></a> <a title=\"" + MAPEDITOR.LOCALIZATION.DEFINES.L64 + "\" href=\"#\" ><img src=\"" + MAPEDITOR.GLOBAL.DEFINES.baseURL + MAPEDITOR.GLOBAL.DEFINES.baseImageDirURL + "delete.png\" onclick=\"MAPEDITOR.ACTIONS.overlayDeleteMe(" + param1 + ");\"/></a> </div></div>";
                                    break;
                                case "searchResultListItem":
                                    MAPEDITOR.TRACER.addTracer("[INFO]: Creating search html String");
                                    htmlString = "<div id=\"searchResultListItem" + param1 + "\" class=\"searchResultListItem\" title=\"" + param2 + "\"> " + param2.substring(0, 20) + " <div class=\"searchResultActionButton\"><a title=\"" + MAPEDITOR.LOCALIZATION.DEFINES.L61 + "\" id=\"searchResultToggle" + param1 + "\" href=\"#\"><img src=\"" + MAPEDITOR.GLOBAL.DEFINES.baseURL + MAPEDITOR.GLOBAL.DEFINES.baseImageDirURL + "sub.png\" onclick=\"MAPEDITOR.ACTIONS.searchResultHideMe(" + param1 + ");\" /></a> <a title=\"" + MAPEDITOR.LOCALIZATION.DEFINES.L62 + "\" href=\"#\" onclick=\"MAPEDITOR.ACTIONS.searchResultDeleteMe(" + param1 + ");\"><img src=\"" + MAPEDITOR.GLOBAL.DEFINES.baseURL + MAPEDITOR.GLOBAL.DEFINES.baseImageDirURL + "delete.png\"/></a></div></div>";
                                    break;
                            }
                            MAPEDITOR.TRACER.addTracer("[INFO]: writeHTML completed...");
                            return htmlString;
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    resizeView: function () {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: resizeView started...");
                            //get sizes of elements already drawn
                            var totalPX = document.documentElement.clientHeight;
                            var headerPX = $("#mapedit_container").offset().top;
                            var widthPX = document.documentElement.clientWidth;

                            //set the width of the sf menu pane0 container
                            document.getElementById("mapedit_container_pane_0").style.width = widthPX + "px";

                            //if first time loaded (fixes issue where sfmenu was not loaded thus not calc'd in page height)
                            if (MAPEDITOR.GLOBAL.DEFINES.pageLoadTime < (new Date().getTime())) {
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
                                MAPEDITOR.TRACER.addTracer("[INFO]: non vis button count: " + buttonNonVisibleCount);
                                var buttonVisibleCount = toolbarButtonIds.length - buttonNonVisibleCount;
                                MAPEDITOR.TRACER.addTracer("[INFO]: vis button count: " + buttonVisibleCount);
                                for (var i = 0; i < buttonVisibleCount; i++) {
                                    MAPEDITOR.TRACER.addTracer("[INFO]: showing: " + toolbarButtonIds[i]);
                                    //document.getElementById(toolbarButtonIds[i]).style.visibility = "show";
                                    //document.getElementById(toolbarButtonIds[i]).style.display = "block";
                                }
                                for (var i = buttonVisibleCount; i < buttonNonVisibleCount; i++) {
                                    MAPEDITOR.TRACER.addTracer("[INFO]: hiding: " + toolbarButtonIds[i]);
                                    //document.getElementById(toolbarButtonIds[i]).style.visibility = "hidden";
                                    //document.getElementById(toolbarButtonIds[i]).style.display = "none";
                                }
                            }
                            //calculate how many buttons can be placed based on width
                            //display said buttons with arrow to cycle through

                            //detect and handle different widths
                            //todo make the 800,250 dynamic
                            if (widthPX <= 800) {
                                MAPEDITOR.TRACER.addTracer("[INFO]: tablet viewing width detected...");
                                //toolbar
                                //menubar
                                //toolbox -min
                            }
                            if (widthPX <= 250) {
                                MAPEDITOR.TRACER.addTracer("[INFO]: phone viewing width detected...");
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
                            MAPEDITOR.TRACER.addTracer("[INFO]: percentage of height: " + percentOfHeight);

                            MAPEDITOR.TRACER.addTracer("[INFO]: sizes:<br>height: " + totalPX + " header: " + headerPX + " body: " + bodyPX + " pane0: " + pane0PX + " pane1: " + pane1PX + " pane2: " + pane2PX);
                            MAPEDITOR.TRACER.addTracer("[INFO]: resizeView completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    clearCacheSaveOverlay: function () {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: clearCacheSaveOverlay started...");
                            MAPEDITOR.TRACER.addTracer("[INFO]: attempting to clear save overlay cache");
                            if (MAPEDITOR.GLOBAL.DEFINES.savingOverlayIndex.length > 0) {
                                MAPEDITOR.TRACER.addTracer("[INFO]: reseting cache data");
                                MAPEDITOR.GLOBAL.DEFINES.savingOverlayIndex = [];
                                MAPEDITOR.GLOBAL.DEFINES.savingOverlayPageId = [];
                                MAPEDITOR.GLOBAL.DEFINES.savingOverlayLabel = [];
                                MAPEDITOR.GLOBAL.DEFINES.savingOverlaySourceURL = [];
                                MAPEDITOR.GLOBAL.DEFINES.savingOverlayBounds = [];
                                MAPEDITOR.GLOBAL.DEFINES.savingOverlayRotation = [];
                                MAPEDITOR.TRACER.addTracer("[INFO]: reseting cache save overlay index");
                                MAPEDITOR.GLOBAL.DEFINES.csoi = 0;
                                MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = false;
                                MAPEDITOR.TRACER.addTracer("[INFO]: reseting working index");
                                MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex = null;
                                MAPEDITOR.TRACER.addTracer("[INFO]: reseting preserved rotation");
                                MAPEDITOR.GLOBAL.DEFINES.preservedRotation = 0;
                                //MAPEDITOR.GLOBAL.DEFINES.preservedOpacity = MAPEDITOR.GLOBAL.DEFINES.defaultOpacity;
                                MAPEDITOR.TRACER.addTracer("[INFO]: cache reset");
                            } else {
                                MAPEDITOR.TRACER.addTracer("[INFO]: nothing in cache");
                            }
                            MAPEDITOR.TRACER.addTracer("[INFO]: clearCacheSaveOverlay completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    clearOverlaysOnMap: function () {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: clearOverlaysOnMap started...");
                            MAPEDITOR.TRACER.addTracer("[INFO]: attempting to clear ooms");
                            if (MAPEDITOR.GLOBAL.DEFINES.overlaysOnMap.length > 0) {
                                MAPEDITOR.GLOBAL.DEFINES.overlaysOnMap = [];
                                MAPEDITOR.TRACER.addTracer("[INFO]: ooms reset");
                                //MAPEDITOR.GLOBAL.DEFINES.overlayCount = 0;
                            } else {
                                MAPEDITOR.TRACER.addTracer("[INFO]: no overlays on the map");
                            }
                            MAPEDITOR.TRACER.addTracer("[INFO]: clearOverlaysOnMap completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    resetHiddenOverlays: function () {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: resetHiddenOverlays started...");
                            MAPEDITOR.TRACER.addTracer("[INFO]: total oom count: " + MAPEDITOR.GLOBAL.DEFINES.overlaysOnMap.length);
                            for (var i = 1; i < MAPEDITOR.GLOBAL.DEFINES.overlaysOnMap.length; i++) {
                                MAPEDITOR.TRACER.addTracer("[INFO]: oom ID:" + i);
                                var isComparable = false;
                                try {
                                    if (MAPEDITOR.GLOBAL.DEFINES.overlaysOnMap[i].image_) {
                                        isComparable = true;
                                    }
                                } catch (e) {
                                    MAPEDITOR.TRACER.addTracer("[WARN]: No image for oom ID" + i);
                                }
                                if (isComparable) {
                                    for (var j = 0; j < MAPEDITOR.GLOBAL.DEFINES.incomingPolygonSourceURL.length; j++) {
                                        MAPEDITOR.TRACER.addTracer("[INFO]: incoming ID:" + j);
                                        try {
                                            if ((MAPEDITOR.GLOBAL.DEFINES.overlaysOnMap[i].image_ == MAPEDITOR.GLOBAL.DEFINES.incomingPolygonSourceURL[j]) && (MAPEDITOR.GLOBAL.DEFINES.incomingPolygonFeatureType[j] == "TEMP_main")) {
                                                MAPEDITOR.GLOBAL.DEFINES.incomingPolygonFeatureType[j] = "hidden";
                                                MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPolygonType[j] = "hidden";
                                                MAPEDITOR.TRACER.addTracer("[INFO]: Set Incoming To 'Hidden'");
                                            }
                                        } catch (e) {
                                            MAPEDITOR.TRACER.addTracer("[ERROR]: Not found.");
                                        }
                                    }
                                }
                            }
                            MAPEDITOR.TRACER.addTracer("[INFO]: resetHiddenOverlays completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    latLngToPixel: function (latLng) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: latLngToPixel started...");
                            var topRight = map.getProjection().fromLatLngToPoint(map.getBounds().getNorthEast());
                            var bottomLeft = map.getProjection().fromLatLngToPoint(map.getBounds().getSouthWest());
                            var scale = Math.pow(2, map.getZoom());
                            var worldPoint = map.getProjection().fromLatLngToPoint(latLng);
                            MAPEDITOR.TRACER.addTracer("[INFO]: latLngToPixel completed...");
                            return new google.maps.Point((worldPoint.x - bottomLeft.x) * scale, (worldPoint.y - topRight.y) * scale);
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    checkKeyCode: function () {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: checkKeyCode started...");
                            //go through each textareaids
                            var trueHits = 0;
                            for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.listOfTextAreaIds.length; i++) {
                                if (document.activeElement.id.indexOf(MAPEDITOR.GLOBAL.DEFINES.listOfTextAreaIds[i]) != -1) {
                                    trueHits++;
                                }
                            }
                            if (trueHits > 0) {
                                MAPEDITOR.GLOBAL.DEFINES.typingInTextArea = true;
                            } else {
                                MAPEDITOR.GLOBAL.DEFINES.typingInTextArea = false;
                            }
                            MAPEDITOR.TRACER.addTracer("[INFO]: checkKeyCode completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    clearIncomingOverlays: function () {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: clearIncomingOverlays started...");
                            //go through and display overlays as long as there is an overlay to display
                            for (var i = 0; i < MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPageId.length; i++) {
                                if (MAPEDITOR.GLOBAL.DEFINES.overlaysOnMap[MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPageId[i]] != null) {
                                    MAPEDITOR.GLOBAL.DEFINES.overlaysOnMap[MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPageId[i]].setMap(null);
                                    MAPEDITOR.GLOBAL.DEFINES.overlaysOnMap[MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPageId[i]] = null;
                                    MAPEDITOR.GLOBAL.DEFINES.ghostOverlayRectangle[MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPageId[i]].setMap(null);
                                    MAPEDITOR.GLOBAL.DEFINES.ghostOverlayRectangle[MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPageId[i]] = null;
                                } else {
                                    //do nothing
                                }
                            }
                            //MAPEDITOR.GLOBAL.DEFINES.overlaysCurrentlyDisplayed = false;
                            MAPEDITOR.GLOBAL.DEFINES.preservedOpacity = MAPEDITOR.GLOBAL.DEFINES.defaultOpacity;
                            MAPEDITOR.TRACER.addTracer("[INFO]: clearIncomingOverlays completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    setGhostOverlay: function (ghostIndex, ghostBounds) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: setGhostOverlay started...");
                            //create ghost directly over an overlay
                            MAPEDITOR.GLOBAL.DEFINES.ghostOverlayRectangle[ghostIndex] = new google.maps.Rectangle();        //init ghost
                            MAPEDITOR.GLOBAL.DEFINES.ghostOverlayRectangle[ghostIndex].setOptions(MAPEDITOR.GLOBAL.DEFINES.ghosting);       //set MAPEDITOR.GLOBAL.DEFINES.ghosting 
                            MAPEDITOR.GLOBAL.DEFINES.ghostOverlayRectangle[ghostIndex].setBounds(ghostBounds);               //set bounds
                            MAPEDITOR.GLOBAL.DEFINES.ghostOverlayRectangle[ghostIndex].setMap(map);                          //set to map

                            //create listener for if clicked
                            google.maps.event.addListener(MAPEDITOR.GLOBAL.DEFINES.ghostOverlayRectangle[ghostIndex], 'click', function () {
                                if (MAPEDITOR.GLOBAL.DEFINES.pageMode == "edit") { //2do, when would you move a ghost withot editing?
                                    MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                    MAPEDITOR.ACTIONS.openToolboxTab("overlay");
                                    if (MAPEDITOR.GLOBAL.DEFINES.currentlyEditing == "yes") {                                                            //if editing is being done, save
                                        if (MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex == null) {
                                            MAPEDITOR.GLOBAL.DEFINES.currentlyEditing = "no";
                                        } else {
                                            MAPEDITOR.UTILITIES.cacheSaveOverlay(MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex);                                                       //trigger a cache of current working overlay
                                            MAPEDITOR.GLOBAL.DEFINES.ghostOverlayRectangle[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex].setOptions(MAPEDITOR.GLOBAL.DEFINES.ghosting);                    //set MAPEDITOR.GLOBAL.DEFINES.rectangle to MAPEDITOR.GLOBAL.DEFINES.ghosting
                                            MAPEDITOR.GLOBAL.DEFINES.currentlyEditing = "no";                                                            //reset editing marker
                                            MAPEDITOR.GLOBAL.DEFINES.preservedRotation = 0;                                                              //reset preserved rotation
                                        }
                                    }
                                    if (MAPEDITOR.GLOBAL.DEFINES.currentlyEditing == "no") {                                                             //if editing is not being done, start editing
                                        $("#toolbox").show();                                                                   //show the toolbox
                                        MAPEDITOR.GLOBAL.DEFINES.toolboxDisplayed = true;                                                                //mark that the toolbox is open
                                        $("#toolboxTabs").accordion({ active: 3 });                                             //open edit overlay tab in toolbox
                                        MAPEDITOR.GLOBAL.DEFINES.currentlyEditing = "yes";                                                               //enable editing marker
                                        //alert("prev woi:" + MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex + " new " + ghostIndex);
                                        MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex = ghostIndex;                                                       //set this overay as the one being e
                                        MAPEDITOR.GLOBAL.DEFINES.ghostOverlayRectangle[ghostIndex].setOptions(MAPEDITOR.GLOBAL.DEFINES.editable);                                 //show ghost
                                        MAPEDITOR.GLOBAL.DEFINES.currentTopZIndex++;                                                                     //iterate top z index
                                        document.getElementById("overlay" + ghostIndex).style.zIndex = MAPEDITOR.GLOBAL.DEFINES.currentTopZIndex;        //bring overlay to front
                                        MAPEDITOR.GLOBAL.DEFINES.ghostOverlayRectangle[ghostIndex].setOptions({ zIndex: MAPEDITOR.GLOBAL.DEFINES.currentTopZIndex });             //bring ghost to front
                                        //set rotation if the overlay was previously saved
                                        if (MAPEDITOR.GLOBAL.DEFINES.preservedRotation != MAPEDITOR.GLOBAL.DEFINES.savingOverlayRotation[ghostIndex - 1]) {
                                            MAPEDITOR.GLOBAL.DEFINES.preservedRotation = MAPEDITOR.GLOBAL.DEFINES.savingOverlayRotation[ghostIndex - 1];
                                        }
                                    }
                                }
                            });

                            //set listener for bounds changed
                            google.maps.event.addListener(MAPEDITOR.GLOBAL.DEFINES.ghostOverlayRectangle[ghostIndex], 'bounds_changed', function () {
                                MAPEDITOR.TRACER.addTracer("[INFO]: ghost index: " + ghostIndex);
                                if (MAPEDITOR.GLOBAL.DEFINES.pageMode == "edit") {
                                    MAPEDITOR.GLOBAL.DEFINES.userMayLoseData = true;
                                    MAPEDITOR.ACTIONS.openToolboxTab("overlay");
                                    //hide previous overlay
                                    MAPEDITOR.GLOBAL.DEFINES.overlaysOnMap[ghostIndex].setMap(null);
                                    //delete previous overlay values
                                    MAPEDITOR.GLOBAL.DEFINES.overlaysOnMap[ghostIndex] = null;
                                    //redraw the overlay within the new bounds
                                    MAPEDITOR.TRACER.addTracer(MAPEDITOR.GLOBAL.DEFINES.preservedRotation);
                                    MAPEDITOR.GLOBAL.DEFINES.overlaysOnMap[ghostIndex] = new MAPEDITOR.UTILITIES.CustomOverlay(ghostIndex, MAPEDITOR.GLOBAL.DEFINES.ghostOverlayRectangle[ghostIndex].getBounds(), MAPEDITOR.GLOBAL.DEFINES.incomingPolygonSourceURL[(ghostIndex - 1)], map, MAPEDITOR.GLOBAL.DEFINES.preservedRotation);
                                    //set the overlay with new bounds to the map
                                    MAPEDITOR.GLOBAL.DEFINES.overlaysOnMap[ghostIndex].setMap(map);
                                    //enable editing marker
                                    MAPEDITOR.GLOBAL.DEFINES.currentlyEditing = "yes";
                                    //trigger a cache of current working overlay
                                    MAPEDITOR.UTILITIES.cacheSaveOverlay(MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex);
                                }
                            });

                            //set listener for right click (fixes reset issue over overlays)
                            google.maps.event.addListener(MAPEDITOR.GLOBAL.DEFINES.ghostOverlayRectangle[ghostIndex], 'rightclick', function () {
                                MAPEDITOR.GLOBAL.DEFINES.drawingManager.setDrawingMode(null); //reset drawing manager no matter what
                                //MAPEDITOR.GLOBAL.DEFINES.drawingManager.setMap(null);
                            });
                            MAPEDITOR.TRACER.addTracer("[INFO]: setGhostOverlay completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    cacheSaveOverlay: function (index) {
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: cacheSaveOverlay started...");
                            MAPEDITOR.TRACER.addTracer("[INFO]: caching save overlay <hr/>");
                            MAPEDITOR.TRACER.addTracer("[INFO]: incoming index: " + index);
                            MAPEDITOR.TRACER.addTracer("[INFO]: current save overlay index: " + MAPEDITOR.GLOBAL.DEFINES.csoi);
                            MAPEDITOR.TRACER.addTracer("[INFO]: current working overlay index: " + MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex);
                            //convert working id to index
                            MAPEDITOR.GLOBAL.DEFINES.csoi = index - 1;
                            //is this the first save
                            MAPEDITOR.GLOBAL.DEFINES.firstSaveOverlay = true;
                            //MAPEDITOR.TRACER.addTracer("[INFO]: firstSaveOvelay: " + MAPEDITOR.GLOBAL.DEFINES.firstSaveOverlay);
                            //set overlay index to save
                            MAPEDITOR.GLOBAL.DEFINES.savingOverlayIndex[MAPEDITOR.GLOBAL.DEFINES.csoi] = MAPEDITOR.GLOBAL.DEFINES.csoi; //MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex;
                            MAPEDITOR.TRACER.addTracer("[INFO]: MAPEDITOR.GLOBAL.DEFINES.savingOverlayIndex[MAPEDITOR.GLOBAL.DEFINES.csoi]: " + MAPEDITOR.GLOBAL.DEFINES.savingOverlayIndex[MAPEDITOR.GLOBAL.DEFINES.csoi]);
                            MAPEDITOR.TRACER.addTracer("[INFO]: MAPEDITOR.GLOBAL.DEFINES.csoi: " + MAPEDITOR.GLOBAL.DEFINES.csoi);
                            //set label to save
                            MAPEDITOR.GLOBAL.DEFINES.savingOverlayLabel[MAPEDITOR.GLOBAL.DEFINES.csoi] = MAPEDITOR.GLOBAL.DEFINES.incomingPolygonLabel[MAPEDITOR.GLOBAL.DEFINES.csoi];
                            MAPEDITOR.TRACER.addTracer("[INFO]: MAPEDITOR.GLOBAL.DEFINES.savingOverlayLabel[MAPEDITOR.GLOBAL.DEFINES.csoi]: " + MAPEDITOR.GLOBAL.DEFINES.savingOverlayLabel[MAPEDITOR.GLOBAL.DEFINES.csoi]);
                            MAPEDITOR.TRACER.addTracer("[INFO]: MAPEDITOR.GLOBAL.DEFINES.incomingPolygonLabel[MAPEDITOR.GLOBAL.DEFINES.csoi]: " + MAPEDITOR.GLOBAL.DEFINES.incomingPolygonLabel[MAPEDITOR.GLOBAL.DEFINES.csoi]);
                            //set source url to save
                            MAPEDITOR.GLOBAL.DEFINES.savingOverlaySourceURL[MAPEDITOR.GLOBAL.DEFINES.csoi] = MAPEDITOR.GLOBAL.DEFINES.incomingPolygonSourceURL[MAPEDITOR.GLOBAL.DEFINES.csoi];
                            MAPEDITOR.TRACER.addTracer("[INFO]: MAPEDITOR.GLOBAL.DEFINES.savingOverlaySourceURL[MAPEDITOR.GLOBAL.DEFINES.csoi]: " + MAPEDITOR.GLOBAL.DEFINES.incomingPolygonSourceURL[MAPEDITOR.GLOBAL.DEFINES.csoi]);
                            //set bounds to save
                            MAPEDITOR.GLOBAL.DEFINES.savingOverlayBounds[MAPEDITOR.GLOBAL.DEFINES.csoi] = MAPEDITOR.GLOBAL.DEFINES.ghostOverlayRectangle[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex].getBounds();
                            MAPEDITOR.TRACER.addTracer("[INFO]: MAPEDITOR.GLOBAL.DEFINES.savingOverlayBounds[MAPEDITOR.GLOBAL.DEFINES.csoi]: " + MAPEDITOR.GLOBAL.DEFINES.ghostOverlayRectangle[MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex].getBounds());
                            //set rotation to save
                            //alert("caching... \nindex: " + index + "\nwoi: " + MAPEDITOR.GLOBAL.DEFINES.workingOverlayIndex + "\ncsoi: " + MAPEDITOR.GLOBAL.DEFINES.csoi + "\nsor: " + MAPEDITOR.GLOBAL.DEFINES.savingOverlayRotation[MAPEDITOR.GLOBAL.DEFINES.csoi] + "\nipr: " + MAPEDITOR.GLOBAL.DEFINES.incomingPolygonRotation[MAPEDITOR.GLOBAL.DEFINES.csoi] + "\npr: " + MAPEDITOR.GLOBAL.DEFINES.preservedRotation);
                            if (MAPEDITOR.GLOBAL.DEFINES.savingOverlayRotation[MAPEDITOR.GLOBAL.DEFINES.csoi] != MAPEDITOR.GLOBAL.DEFINES.incomingPolygonRotation[MAPEDITOR.GLOBAL.DEFINES.csoi]) {
                                MAPEDITOR.GLOBAL.DEFINES.savingOverlayRotation[MAPEDITOR.GLOBAL.DEFINES.csoi] = MAPEDITOR.GLOBAL.DEFINES.preservedRotation;
                                //alert("no match" + MAPEDITOR.GLOBAL.DEFINES.preservedRotation);
                            } else {
                                //alert("match " + MAPEDITOR.GLOBAL.DEFINES.savingOverlayRotation[MAPEDITOR.GLOBAL.DEFINES.csoi]);
                            }
                            MAPEDITOR.GLOBAL.DEFINES.savingOverlayRotation[MAPEDITOR.GLOBAL.DEFINES.csoi] = MAPEDITOR.GLOBAL.DEFINES.preservedRotation;
                            MAPEDITOR.TRACER.addTracer("[INFO]: MAPEDITOR.GLOBAL.DEFINES.savingOverlayRotation[MAPEDITOR.GLOBAL.DEFINES.csoi]: " + MAPEDITOR.GLOBAL.DEFINES.savingOverlayRotation[MAPEDITOR.GLOBAL.DEFINES.csoi]);
                            //set the pageId to save
                            MAPEDITOR.GLOBAL.DEFINES.savingOverlayPageId[MAPEDITOR.GLOBAL.DEFINES.csoi] = MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPageId[MAPEDITOR.GLOBAL.DEFINES.csoi];
                            MAPEDITOR.TRACER.addTracer("[INFO]: MAPEDITOR.GLOBAL.DEFINES.savingOverlayPageId[MAPEDITOR.GLOBAL.DEFINES.csoi]: " + MAPEDITOR.GLOBAL.DEFINES.incomingPolygonPageId[MAPEDITOR.GLOBAL.DEFINES.csoi]);
                            MAPEDITOR.TRACER.addTracer("[INFO]: save overlay cached");
                            MAPEDITOR.TRACER.addTracer("[INFO]: cacheSaveOverlay completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    },
                    CustomOverlay: function (id, bounds, image, map, rotation) {
                        //Starts the creation of a custom overlay div which contains a rectangular image.
                        //Supporting URL: https://developers.google.com/maps/documentation/javascript/overlays#CustomOverlays
                        try {
                            MAPEDITOR.TRACER.addTracer("[INFO]: CustomOverlay started...");
                            MAPEDITOR.GLOBAL.DEFINES.overlayCount++;                   //iterate how many overlays have been drawn
                            this.bounds_ = bounds;                      //set the bounds
                            this.image_ = image;                        //set source url
                            this.map_ = map;                            //set to map
                            MAPEDITOR.GLOBAL.DEFINES.preservedRotation = rotation;     //set the rotation
                            this.div_ = null;                           //defines a property to hold the image's div. We'll actually create this div upon receipt of the onAdd() method so we'll leave it null for now.
                            this.index_ = id;                           //set the index/id of this overlay
                            MAPEDITOR.TRACER.addTracer("[INFO]: CustomOverlay completed...");
                        } catch (err) {
                            MAPEDITOR.TRACER.addTracer("[ERROR]: " + err + " at line " +err.lineNumber );
                        }
                    }
                };
            }(),       //indirectly used fcns
            run: function () {
                try {
                    MAPEDITOR.TRACER.addTracer("[INFO]: MapEditor.run started...");
                    //google.maps.event.addDomListener(window, 'load', MAPEDITOR.GLOBAL.initGMap); //TEMP, when does this get fired?
                    MAPEDITOR.UTILITIES.CustomOverlay.prototype = new google.maps.OverlayView(); //used to display custom overlay
                    MAPEDITOR.GLOBAL.initDeclarations();
                    MAPEDITOR.GLOBAL.initListOfTextAreaIds();
                    MAPEDITOR.GLOBAL.initLocalization();
                    MAPEDITOR.GLOBAL.initListeners();
                    MAPEDITOR.GLOBAL.initJqueryElements();
                    initConfigSettings(); //c# to js
                    MAPEDITOR.GLOBAL.initInterface(MAPEDITOR.GLOBAL.DEFINES.collectionLoadType); //defines interface
                    MAPEDITOR.GLOBAL.initGMap();
                    window.onkeypress = MAPEDITOR.ACTIONS.keyPress; //keypress shortcuts/actions (MOVE TO dynamic)
                    MAPEDITOR.TRACER.addTracer("[INFO]: MapEditor.run completed...");
                } catch(e) {
                    MAPEDITOR.TRACER.addTracer("[ERROR]: MapEditor.run failed...");
                } 
            }                //init all fcn
        };
    }();
    //once it has been declared, run it
    MAPEDITOR.run();
}
