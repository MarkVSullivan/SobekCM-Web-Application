//Gmap Support

//#region main google functions

///<summary>Setups everything with user defined options</summary>
///<param name="collection" type="string">Specify the type of collection to load</param>
function setupInterface(collection) {
    ///<summary>Setups everything with user defined options</summary>
    ///<param name="collection" type="string">Specify the type of collection to load</param>
    //todo make this auto generated  

    google.maps.visualRefresh = true; //Enable the visual refresh (new gmaps)

    switch (collection) {
        case "default":
            globalVar.baseImageDirURL = "default/images/mapedit/";                            //the default directory to the image files
            globalVar.mapLayerActive = "Roadmap";                                             //what map layer is displayed
            globalVar.mapDrawingManagerDisplayed = false;                                     //by default, is the drawing manager displayed (true/false)
            globalVar.mapCenter = new google.maps.LatLng(29.6480, -82.3482);                  //used to center map on load
            globalVar.mapControlsDisplayed = true;                                            //by default, are map controls displayed (true/false)
            globalVar.defaultDisplayDrawingMangerTool = false;                                //by default, is the drawingmanger displayed (true/false)
            globalVar.toolboxDisplayed = true;                                                //by default, is the toolbox displayed (true/false)
            globalVar.toolbarDisplayed = true;                                                //by default, is the toolbar open (yes/no)
            globalVar.kmlDisplayed = false;                                                   //by default, is kml layer on (yes/no)
            KmlLayer = new google.maps.KmlLayer("http://ufdc.ufl.edu/design/mapedit/stAugParel_v6.kmz");  //must be pingable by google
            globalVar.defaultZoomLevel = 13;                                                  //zoom level, starting
            globalVar.maxZoomLevel = 2;                                                       //max zoom out, default (21=lowest level, 1=highest level)
            globalVar.minZoomLevel_Terrain = 15;                                              //max zoom in, terrain
            globalVar.minZoomLevel_Satellite = 20;                                            //max zoom in, sat + hybrid
            globalVar.minZoomLevel_Roadmap = 21;                                              //max zoom in, roadmap (default)
            globalVar.minZoomLevel_BlockLot = 19;                                             //max zoom in, used for special layers not having default of roadmap
            globalVar.isCustomOverlay = false;                                                //used to determine if other overlays (block/lot etc) //unknown
            globalVar.preservedRotation = 0;                                                  //rotation, default
            globalVar.knobRotationValue = 0;                                                  //rotation to display by default 
            globalVar.preservedOpacity = 0.75;                                                 //opacity, default value (0-1,1=opaque)
            globalVar.strictBounds = null;                                                    //set the bounds for this google map instance (set to null for no bounds)
            globalVar.hasCustomMapType = true;                                                //used to determine if there is a custom maptype layer
            break;
        case "stAugustine":
            globalVar.baseImageDirURL = "default/images/mapedit/";                            //the default directory to the image files
            globalVar.mapDrawingManagerDisplayed = false;                                     //by default, is the drawing manager displayed (true/false)
            globalVar.mapLayerActive = "Roadmap";                                             //what map layer is displayed
            globalVar.mapCenter = new google.maps.LatLng(29.8944, -81.3147);                  //used to center map on load
            globalVar.mapControlsDisplayed = true;                                            //by default, are map controls displayed (true/false)
            globalVar.defaultDisplayDrawingMangerTool = false;                                //by default, is the drawingmanger displayed (true/false)
            globalVar.toolboxDisplayed = true;                                                //by default, is the toolbox displayed (true/false)
            globalVar.toolbarDisplayed = true;                                                //by default, is the toolbar open (yes/no)
            globalVar.kmlDisplayed = false;                                                   //by default, is kml layer on (yes/no)
            //KmlLayer = new google.maps.KmlLayer("http://ufdc.ufl.edu/design/mapedit/stAugParcel_v6.kmz");  //must be pingable by google
            KmlLayer = new google.maps.KmlLayer("http://ufdc.ufl.edu/design/mapedit/stAugParcel_v6.kmz");  //must be pingable by google
            globalVar.defaultZoomLevel = 14;                                                  //zoom level, starting
            globalVar.maxZoomLevel = 10;                                                      //max zoom out, default (21=lowest level, 1=highest level)
            globalVar.minZoomLevel_Terrain = 15;                                              //max zoom in, terrain
            globalVar.minZoomLevel_Satellite = 20;                                            //max zoom in, sat + hybrid
            globalVar.minZoomLevel_Roadmap = 21;                                              //max zoom in, roadmap (default)
            globalVar.minZoomLevel_BlockLot = 19;                                             //max zoom in, used for special layers not having default of roadmap
            globalVar.isCustomOverlay = false;                                                //used to determine if other overlays (block/lot etc) //unknown
            globalVar.preservedRotation = 0;                                                  //rotation, default
            globalVar.knobRotationValue = 0;                                                  //rotation to display by default 
            globalVar.preservedOpacity = 0.35;                                                 //opacity, default value (0-1,1=opaque)
            globalVar.hasCustomMapType = true;                                                //used to determine if there is a custom maptype layer
            globalVar.strictBounds = new google.maps.LatLngBounds(                            //set the bounds for this google map instance
                new google.maps.LatLng(29.78225755812941, -81.4306640625),
                new google.maps.LatLng(29.99181288866604, -81.1917114257)
            );
            break;
        case "custom":
            globalVar.baseImageDirURL = "default/images/mapedit/";                            //the default directory to the image files
            globalVar.mapDrawingManagerDisplayed = false;                                     //by default, is the drawing manager displayed (true/false)
            globalVar.mapLayerActive = "Roadmap";                                             //what map layer is displayed
            globalVar.mapCenter = new google.maps.LatLng(29.6480, -82.3482);                  //used to center map on load
            globalVar.mapControlsDisplayed = true;                                            //by default, are map controls displayed (true/false)
            globalVar.defaultDisplayDrawingMangerTool = false;                                //by default, is the drawingmanger displayed (true/false)
            globalVar.toolboxDisplayed = true;                                                //by default, is the toolbox displayed (true/false)
            globalVar.toolbarDisplayed = true;                                                //by default, is the toolbar open (yes/no)
            globalVar.kmlDisplayed = false;                                                   //by default, is kml layer on (yes/no)
            KmlLayer = new google.maps.KmlLayer("http://ufdc.ufl.edu/design/mapedit/parcels_2012_kmz_fldor.kmz");  //must be pingable by google
            globalVar.defaultZoomLevel = 13;                                                  //zoom level, starting
            globalVar.maxZoomLevel = 10;                                                      //max zoom out, default (21=lowest level, 1=highest level)
            globalVar.minZoomLevel_Terrain = 15;                                              //max zoom in, terrain
            globalVar.minZoomLevel_Satellite = 20;                                            //max zoom in, sat + hybrid
            globalVar.minZoomLevel_Roadmap = 21;                                              //max zoom in, roadmap (default)
            globalVar.minZoomLevel_BlockLot = 19;                                             //max zoom in, used for special layers not having default of roadmap
            globalVar.isCustomOverlay = false;                                                //used to determine if other overlays (block/lot etc) //unknown
            globalVar.preservedRotation = 0;                                                  //rotation, default
            globalVar.knobRotationValue = 0;                                                  //rotation to display by default 
            globalVar.preservedOpacity = 0.75;                                                 //opacity, default value (0-1,1=opaque)
            globalVar.hasCustomMapType = true;                                                //used to determine if there is a custom maptype layer
            globalVar.strictBounds = new google.maps.LatLngBounds(                            //set the bounds for this google map instance
                new google.maps.LatLng(29.21570636285318, -82.87811279296875),
                new google.maps.LatLng(30.07978967039041, -81.76300048828125)
            );
            break;
        case "florida":
            globalVar.baseImageDirURL = "default/images/mapedit/";                            //the default directory to the image files
            globalVar.mapDrawingManagerDisplayed = false;                                     //by default, is the drawing manager displayed (true/false)
            globalVar.mapLayerActive = "Roadmap";                                             //what map layer is displayed
            globalVar.mapCenter = new google.maps.LatLng(29.6480, -82.3482);                  //used to center map on load
            globalVar.mapControlsDisplayed = true;                                            //by default, are map controls displayed (true/false)
            globalVar.defaultDisplayDrawingMangerTool = false;                                //by default, is the drawingmanger displayed (true/false)
            globalVar.toolboxDisplayed = true;                                                //by default, is the toolbox displayed (true/false)
            globalVar.toolbarDisplayed = true;                                                //by default, is the toolbar open (yes/no)
            globalVar.kmlDisplayed = false;                                                   //by default, is kml layer on (yes/no)
            KmlLayer = new google.maps.KmlLayer("http://hlmatt.com/uf/kml/10.kml"); //must be pingable by google
            globalVar.defaultZoomLevel = 13;                                                  //zoom level, starting
            globalVar.maxZoomLevel = 1;                                                       //max zoom out, default (21=lowest level, 1=highest level)
            globalVar.minZoomLevel_Terrain = 15;                                              //max zoom in, terrain
            globalVar.minZoomLevel_Satellite = 20;                                            //max zoom in, sat + hybrid
            globalVar.minZoomLevel_Roadmap = 21;                                              //max zoom in, roadmap (default)
            globalVar.minZoomLevel_BlockLot = 19;                                             //max zoom in, used for special layers not having default of roadmap
            globalVar.isCustomOverlay = false;                                                //used to determine if other overlays (block/lot etc) 
            globalVar.preservedRotation = 0;                                                  //rotation, default
            globalVar.knobRotationValue = 0;                                                  //rotation to display by default 
            globalVar.preservedOpacity = 0.75;                                                 //opacity, default value (0-1,1=opaque)
            globalVar.hasCustomMapType = true;                                                //used to determine if there is a custom maptype layer
            globalVar.strictBounds = new google.maps.LatLngBounds(                            //set the bounds for this google map instance
                //new google.maps.LatLng(30.69420636285318, -88.04311279296875), //fl nw
                //new google.maps.LatLng(25.06678967039041, -77.33330048828125) //fl se
                //new google.maps.LatLng(24.55531738915811, -81.78283295288095), //fl sw
                //new google.maps.LatLng(30.79109834517092, -81.53709923706058) //fl ne
                //new google.maps.LatLng(29.5862, -82.4146), //gville
                //new google.maps.LatLng(29.7490, -82.2106)
                new google.maps.LatLng(22.053908635225607, -86.18838838405613), //east coast
                new google.maps.LatLng(36.06512404320089, -76.72320000000003)
            );

            //globalVar.strictBounds = new google.maps.LatLngBounds(                            //set the bounds for this google map instance
            //    new google.maps.LatLng(30.69420636285318, -88.04311279296875),
            //    new google.maps.LatLng(25.06678967039041, -77.33330048828125)
            //);
            break;
    }
}

//on page load functions (mainly google map event listeners)
function initialize() {

    //as map is loading, fit to screen
    resizeView();

    //initialize google map objects
    map = new google.maps.Map(document.getElementById(gmapPageDivId), gmapOptions);                             //initialize map    
    map.controls[google.maps.ControlPosition.RIGHT_BOTTOM].push(copyrightNode);                                 //initialize custom copyright
    map.controls[google.maps.ControlPosition.BOTTOM_LEFT].push(cursorLatLongTool);                              //initialize cursor lat long tool
    map.controls[google.maps.ControlPosition.TOP_LEFT].push(toolbarBufferZone1);                                //initialize spacer
    map.controls[google.maps.ControlPosition.TOP_RIGHT].push(toolbarBufferZone2);                               //intialize spacer
    drawingManager.setMap(map);                                                                                 //initialize drawing manager
    drawingManager.setMap(null);                                                                                //initialize drawing manager (hide)
    geocoder = new google.maps.Geocoder();                                                                      //initialize geocoder

    //#region Google Specific Listeners  

    //initialize drawingmanger listeners
    google.maps.event.addListener(drawingManager, 'markercomplete', function (marker) {

        testBounds(); //are we still in the bounds 
        if (globalVar.placerType == "item") {
            globalVar.firstSaveItem = true;
            //used to prevent multi markers
            if (globalVar.firstMarker > 0) {
                drawingManager.setDrawingMode(null); //only place one at a time
            } else {
                globalVar.firstMarker++;
                drawingManager.setDrawingMode(null); //only place one at a time
            }
            globalVar.itemMarker = marker; //assign globally
            de("marker placed");
            document.getElementById('content_toolbox_posItem').value = globalVar.itemMarker.getPosition();
            globalVar.savingMarkerCenter = globalVar.itemMarker.getPosition(); //store coords to save
            codeLatLng(globalVar.itemMarker.getPosition());
        }

        if (globalVar.placerType == "poi") {
            globalVar.firstSavePOI = true;
            globalVar.poi_i++;

            label[globalVar.poi_i] = new MarkerWithLabel({
                position: marker.getPosition(), //position of real marker
                map: map,
                zIndex: 2,
                labelContent: globalVar.poi_i + 1, //the current user count
                labelAnchor: new google.maps.Point(15, 0),
                labelClass: "labels", // the CSS class for the label
                labelStyle: { opacity: 0.75 },
                icon: {} //initialize to nothing so no marker shows
            });

            globalVar.poiObj[globalVar.poi_i] = marker;
            globalVar.poiType[globalVar.poi_i] = "marker";
            var poiId = globalVar.poi_i + 1;
            var poiDescTemp = L_Marker;
            document.getElementById("poiList").innerHTML += writeHTML("poiListItem", globalVar.poi_i, poiId, poiDescTemp);
            var contentString = writeHTML("poiDesc", globalVar.poi_i, "", "");
            infoWindow[globalVar.poi_i] = new google.maps.InfoWindow({
                content: contentString,
                position: marker.getPosition()
                //pixelOffset: new google.maps.Size(0, -40)
            });

            infoWindow[globalVar.poi_i].setMap(map);

            infoWindow[globalVar.poi_i].open(map, globalVar.poiObj[globalVar.poi_i]);

            de("poiCount: " + globalVar.poiCount);

            //best fix so far
            if (globalVar.poiCount == 0) {
                setTimeout(function () {
                    infoWindow[0].setMap(null);
                    infoWindow[0].setOptions({ pixelOffset: new google.maps.Size(0, -40) });
                    infoWindow[0].setMap(map);
                }, 800);
            }

            //if (globalVar.poiCount > 0) {
            //    infoWindow[globalVar.poi_i].open(map);
            //} else {
            //    infoWindow[globalVar.poi_i].setMap(map);
            //    infoWindow[globalVar.poi_i].setMap(null);
            //    de("platform: " + navigator.platform);
            //    if (navigator.platform)
            //    var t2 = setTimeout(function () {
            //        infoWindow[globalVar.poi_i].setMap(map);
            //    }, 1500);
            //}

            globalVar.poiCount++;

            ////try to fix first poi infobox issue
            //de("poi count: " + globalVar.poiCount);
            //de("tempYo" + globalVar.tempYo);
            //if (globalVar.tempYo == false) {
            //    var t = setTimeout(function () {
            //        de("fire infowindow");

            //        var contentString = writeHTML("poiDesc", globalVar.poi_i, "", "");
            //        infoWindow[globalVar.poi_i] = new google.maps.InfoWindow({
            //            content: contentString,
            //            position: marker.getPosition(),
            //            pixelOffset: new google.maps.Size(0, -40)
            //        });

            //        infoWindow[globalVar.poi_i].setMap(map);

            //        poiHideMe(0);

            //        var t2 = setTimeout(function() {

            //            poiShowMe(0);

            //        }, 1500);

            //    }, 500);

            //    globalVar.tempYo = true;

            //} else {

            //    var contentString = writeHTML("poiDesc", globalVar.poi_i, "", "");
            //    infoWindow[globalVar.poi_i] = new google.maps.InfoWindow({
            //        content: contentString,
            //        position: marker.getPosition(),
            //        pixelOffset: new google.maps.Size(0, -40)
            //    });

            //    infoWindow[globalVar.poi_i].setMap(map);
            //}

        }

        google.maps.event.addListener(marker, 'dragstart', function () {

            if (globalVar.placerType == "poi") {
                globalVar.firstSavePOI = true;
                for (var i = 0; i < globalVar.poiObj.length; i++) {
                    if (globalVar.poiObj[i] == this) {
                        infoWindow[i].setMap(null);
                        label[i].setMap(null);
                    }
                }
            }
        });
        google.maps.event.addListener(marker, 'dragend', function () {
            if (globalVar.placerType == "item") {
                globalVar.firstSaveItem = true;
                document.getElementById('content_toolbox_posItem').value = marker.getPosition();
                codeLatLng(marker.getPosition());
            }
            if (globalVar.placerType == "poi") {
                globalVar.firstSavePOI = true;
                for (var i = 0; i < globalVar.poiObj.length; i++) {
                    if (globalVar.poiObj[i] == this) {
                        infoWindow[i].setOptions({ position: marker.getPosition(), pixelOffset: new google.maps.Size(0, -40) });
                        infoWindow[i].open(null);
                        label[i].setPosition(marker.getPosition());
                        label[i].setMap(map);
                    }
                }
            }
        });
        google.maps.event.addListener(marker, 'click', function () {
            if (globalVar.placerType == "poi") {
                globalVar.firstSavePOI = true;
                for (var i = 0; i < globalVar.poiObj.length; i++) {
                    if (globalVar.poiObj[i] == this) {
                        infoWindow[i].setOptions({ position: marker.getPosition(), pixelOffset: new google.maps.Size(0, -40) });
                        infoWindow[i].open(map);
                    }
                }
            }
        });
    });
    google.maps.event.addListener(drawingManager, 'circlecomplete', function (circle) {
        testBounds();
        if (globalVar.placerType == "poi") {
            globalVar.firstSavePOI = true;
            globalVar.poi_i++;

            label[globalVar.poi_i] = new MarkerWithLabel({
                position: circle.getCenter(), //position of real marker
                zIndex: 2,
                map: map,
                labelContent: globalVar.poi_i + 1, //the current user count
                labelAnchor: new google.maps.Point(15, 0),
                labelClass: "labels", // the CSS class for the label
                labelStyle: { opacity: 0.75 },
                icon: {} //initialize to nothing so no marker shows
            });

            var poiId = globalVar.poi_i + 1;
            globalVar.poiObj[globalVar.poi_i] = circle;
            globalVar.poiType[globalVar.poi_i] = "circle";
            var poiDescTemp = L_Circle;
            document.getElementById("poiList").innerHTML += writeHTML("poiListItem", globalVar.poi_i, poiId, poiDescTemp);
            var contentString = writeHTML("poiDesc", globalVar.poi_i, "", "");
            infoWindow[globalVar.poi_i] = new google.maps.InfoWindow({
                content: contentString
            });
            infoWindow[globalVar.poi_i].setPosition(circle.getCenter());
            infoWindow[globalVar.poi_i].open(map);
            //best fix so far
            if (globalVar.poiCount == 0) {
                setTimeout(function () {
                    infoWindow[0].setMap(null);
                    //infoWindow[0].setOptions({ pixelOffset: new google.maps.Size(0, -40) });
                    infoWindow[0].setMap(map);
                }, 800);
            }
            //if (globalVar.poiCount > 0) {
            //    infoWindow[globalVar.poi_i].open(map);
            //} else {
            //    infoWindow[globalVar.poi_i].setMap(map);
            //    infoWindow[globalVar.poi_i].setMap(null);
            //    infoWindow[globalVar.poi_i].setMap(map);
            //}
            globalVar.poiCount++;
        }
        google.maps.event.addListener(circle, 'dragstart', function () {

            if (globalVar.placerType == "poi") {
                globalVar.firstSavePOI = true;
                for (var i = 0; i < globalVar.poiObj.length; i++) {
                    if (globalVar.poiObj[i] == this) {
                        infoWindow[i].setMap(null);
                        label[i].setMap(null);
                    }
                }
            }
        });
        google.maps.event.addListener(circle, 'drag', function () {
            //used to get the center point for lat/long tool
            globalVar.circleCenter = circle.getCenter();
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
            cLat.innerHTML = cLatV + " (" + latH + ")";
            cLong.innerHTML = cLongV + " (" + longH + ")";
        });
        google.maps.event.addListener(circle, 'dragend', function () {
            if (globalVar.placerType == "poi") {
                globalVar.firstSavePOI = true;
                for (var i = 0; i < globalVar.poiObj.length; i++) {
                    if (globalVar.poiObj[i] == this) {
                        infoWindow[i].setPosition(circle.getCenter());
                        infoWindow[i].open(null);
                        label[i].setPosition(circle.getCenter());
                        label[i].setMap(map);
                    }
                }
            }
        });
        google.maps.event.addListener(circle, 'click', function () {
            if (globalVar.placerType == "poi") {
                globalVar.firstSavePOI = true;
                for (var i = 0; i < globalVar.poiObj.length; i++) {
                    if (globalVar.poiObj[i] == this) {
                        infoWindow[i].setPosition(circle.getCenter());
                        infoWindow[i].open(map);
                    }
                }
            }
        });
    });
    google.maps.event.addListener(drawingManager, 'rectanglecomplete', function (rectangle) {
        //check the bounds to make sure you havent strayed too far away
        testBounds();
        if (globalVar.placerType == "overlay") {
            de("placertype: overlay");
            //assing working overlay index
            globalVar.workingOverlayIndex = globalVar.convertedOverlayIndex;
            if (globalVar.overlayType == "drawn") {
                de("globalVar.overlayType: " + globalVar.overlayType);
                de("globalVar.convertedOverlayIndex: " + globalVar.convertedOverlayIndex);
                globalVar.incomingPolygonPath[globalVar.convertedOverlayIndex] = rectangle.getBounds();
                //create overlay with incoming
                globalVar.overlaysOnMap[globalVar.incomingPolygonPageId[globalVar.convertedOverlayIndex]] = new CustomOverlay(globalVar.incomingPolygonPageId[globalVar.convertedOverlayIndex], globalVar.incomingPolygonPath[globalVar.convertedOverlayIndex], globalVar.incomingPolygonSourceURL[globalVar.convertedOverlayIndex], map, globalVar.incomingPolygonRotation[globalVar.convertedOverlayIndex]);
                globalVar.currentlyEditing = "no";
                //set the overlay to the map
                globalVar.overlaysOnMap[globalVar.incomingPolygonPageId[globalVar.convertedOverlayIndex]].setMap(map);
                //set hotspot on top of overlay
                setGhostOverlay(globalVar.incomingPolygonPageId[globalVar.convertedOverlayIndex], globalVar.incomingPolygonPath[globalVar.convertedOverlayIndex]);
                de("I created ghost: " + globalVar.incomingPolygonPageId[globalVar.convertedOverlayIndex]);
                globalVar.mainCount++;
                globalVar.incomingACL = "overlay";
                //mark that we converted it
                globalVar.isConvertedOverlay = true;
                //hide the rectangle we drew
                rectangle.setMap(null);
                //relist the overlay we drew
                initOverlayList();
                //edit that overlay
                overlayEditMe(globalVar.incomingPolygonPageId[globalVar.convertedOverlayIndex]);
                //reset drawing manager no matter what
                drawingManager.setDrawingMode(null);
            } else {
                //mark that we are editing
                globalVar.firstSaveOverlay = true;

                //add the incoming overlay bounds
                globalVar.incomingPolygonPath[globalVar.convertedOverlayIndex] = rectangle.getBounds();

                //redisplay overlays (the one we just made)
                displayIncomingPolygons();

                //relist the overlay we drew
                initOverlayList();

                //hide the rectangle we drew
                rectangle.setMap(null);

                //prevent redraw
                drawingManager.setDrawingMode(null);
            }
            //create cache save overlay for the new converted overlay only
            if (globalVar.isConvertedOverlay == true) {
                cacheSaveOverlay(globalVar.workingOverlayIndex);
                globalVar.isConvertedOverlay = false;
            }
        }

        if (globalVar.placerType == "poi") {
            globalVar.firstSavePOI = true;
            globalVar.poi_i++;

            label[globalVar.poi_i] = new MarkerWithLabel({
                position: rectangle.getBounds().getCenter(), //position of real marker
                zIndex: 2,
                map: map,
                labelContent: globalVar.poi_i + 1, //the current user count
                labelAnchor: new google.maps.Point(15, 0),
                labelClass: "labels", // the CSS class for the label
                labelStyle: { opacity: 0.75 },
                icon: {} //initialize to nothing so no marker shows
            });

            var poiId = globalVar.poi_i + 1;
            globalVar.poiObj[globalVar.poi_i] = rectangle;
            globalVar.poiType[globalVar.poi_i] = "rectangle";
            var poiDescTemp = L_Rectangle;
            document.getElementById("poiList").innerHTML += writeHTML("poiListItem", globalVar.poi_i, poiId, poiDescTemp);
            var contentString = writeHTML("poiDesc", globalVar.poi_i, "", "");
            infoWindow[globalVar.poi_i] = new google.maps.InfoWindow({
                content: contentString
            });
            infoWindow[globalVar.poi_i].setPosition(rectangle.getBounds().getCenter());
            infoWindow[globalVar.poi_i].open(map);
            //best fix so far
            if (globalVar.poiCount == 0) {
                setTimeout(function () {
                    infoWindow[0].setMap(null);
                    //infoWindow[0].setOptions({ pixelOffset: new google.maps.Size(0, -40) });
                    infoWindow[0].setMap(map);
                }, 800);
            }
            //if (globalVar.poiCount > 0) {
            //    infoWindow[globalVar.poi_i].open(map);
            //} else {
            //    infoWindow[globalVar.poi_i].setMap(map);
            //    infoWindow[globalVar.poi_i].setMap(null);
            //    infoWindow[globalVar.poi_i].setMap(map);
            //}
            globalVar.poiCount++;
            de("completed overlay bounds getter");
        }

        google.maps.event.addListener(rectangle, 'bounds_changed', function () {
            if (globalVar.placerType == "poi") {
                globalVar.firstSavePOI = true;
                for (var i = 0; i < globalVar.poiObj.length; i++) {
                    if (globalVar.poiObj[i] == this) {
                        infoWindow[i].setPosition(rectangle.getBounds().getCenter());
                        infoWindow[i].setMap(null);
                        label[i].setPosition(rectangle.getBounds().getCenter());
                        label[i].setMap(map);
                    }
                }
            }
        });
        google.maps.event.addListener(rectangle, 'dragstart', function () {
            if (globalVar.placerType == "poi") {
                globalVar.firstSavePOI = true;
                for (var i = 0; i < globalVar.poiObj.length; i++) {
                    if (globalVar.poiObj[i] == this) {
                        infoWindow[i].setMap(null);
                        label[i].setMap(null);
                    }
                }
            }
        });
        google.maps.event.addListener(rectangle, 'drag', function () {
            if (globalVar.placerType == "poi") {
                globalVar.firstSavePOI = true;
                for (var i = 0; i < globalVar.poiObj.length; i++) {
                    if (globalVar.poiObj[i] == this) {
                        infoWindow[i].setMap(null);
                        label[i].setMap(null);
                    }
                }
            }
            //used to get center point for lat/long tool
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
            cLat.innerHTML = cLatV + " (" + latH + ")";
            cLong.innerHTML = cLongV + " (" + longH + ")";
        });
        google.maps.event.addListener(rectangle, 'dragend', function () {
            if (globalVar.placerType == "poi") {
                globalVar.firstSavePOI = true;
                for (var i = 0; i < globalVar.poiObj.length; i++) {
                    if (globalVar.poiObj[i] == this) {
                        infoWindow[i].setPosition(rectangle.getBounds().getCenter());
                        infoWindow[i].open(null);
                        label[i].setPosition(rectangle.getBounds().getCenter());
                        label[i].setMap(map);
                    }
                }
            }
        });
        google.maps.event.addListener(rectangle, 'click', function () {
            if (globalVar.placerType == "poi") {
                globalVar.firstSavePOI = true;
                for (var i = 0; i < globalVar.poiObj.length; i++) {
                    if (globalVar.poiObj[i] == this) {
                        infoWindow[i].setPosition(rectangle.getBounds().getCenter());
                        infoWindow[i].open(map);
                    }
                }
            }
        });
    });
    google.maps.event.addListener(drawingManager, 'polygoncomplete', function (polygon) {
        testBounds();
        if (globalVar.placerType == "poi") {
            globalVar.firstSavePOI = true;
            globalVar.poi_i++;

            label[globalVar.poi_i] = new MarkerWithLabel({
                position: polygonCenter(polygon), //position of real marker
                zIndex: 2,
                map: map,
                labelContent: globalVar.poi_i + 1, //the current user count
                labelAnchor: new google.maps.Point(15, 0),
                labelClass: "labels", // the CSS class for the label
                labelStyle: { opacity: 0.75 },
                icon: {} //initialize to nothing so no marker shows
            });

            var poiId = globalVar.poi_i + 1;
            globalVar.poiObj[globalVar.poi_i] = polygon;
            globalVar.poiType[globalVar.poi_i] = "polygon";
            var poiDescTemp = L_Polygon;
            document.getElementById("poiList").innerHTML += writeHTML("poiListItem", globalVar.poi_i, poiId, poiDescTemp);
            var contentString = writeHTML("poiDesc", globalVar.poi_i, "", "");
            infoWindow[globalVar.poi_i] = new google.maps.InfoWindow({
                content: contentString
            });
            infoWindow[globalVar.poi_i].setPosition(polygonCenter(polygon));
            infoWindow[globalVar.poi_i].open(map);
            //best fix so far
            if (globalVar.poiCount == 0) {
                setTimeout(function () {
                    infoWindow[0].setMap(null);
                    //infoWindow[0].setOptions({ pixelOffset: new google.maps.Size(0, -40) });
                    infoWindow[0].setMap(map);
                }, 800);
            }
            //if (globalVar.poiCount > 0) {
            //    infoWindow[globalVar.poi_i].open(map);
            //} else {
            //    infoWindow[globalVar.poi_i].setMap(map);
            //    infoWindow[globalVar.poi_i].setMap(null);
            //    infoWindow[globalVar.poi_i].setMap(map);
            //}
            globalVar.poiCount++;
        }
        google.maps.event.addListener(polygon.getPath(), 'set_at', function () { //if bounds change
            if (globalVar.placerType == "poi") {
                globalVar.firstSavePOI = true;
                for (var i = 0; i < globalVar.poiObj.length; i++) {
                    if (globalVar.poiObj[i] == this) {
                        infoWindow[i].setPosition(polygonCenter(polygon));
                        infoWindow[i].setMap(null);
                        label[i].setPosition(polygonCenter(polygon));
                        label[i].setMap(map); //does not redisplay
                    }
                }
            }
        });
        google.maps.event.addListener(polygon, 'dragstart', function () {
            if (globalVar.placerType == "poi") {
                globalVar.firstSavePOI = true;
                for (var i = 0; i < globalVar.poiObj.length; i++) {
                    if (globalVar.poiObj[i] == this) {
                        infoWindow[i].setMap(null);
                        label[i].setMap(null);
                    }
                }
            }
        });
        google.maps.event.addListener(polygon, 'drag', function () {
            if (globalVar.placerType == "poi") {
                globalVar.firstSavePOI = true;
                for (var i = 0; i < globalVar.poiObj.length; i++) {
                    if (globalVar.poiObj[i] == this) {
                        infoWindow[i].setMap(null);
                        label[i].setMap(null);
                    }
                }
            }
            //used for lat/long tool
            var str = polygonCenter(polygon).toString();
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
            cLat.innerHTML = cLatV + " (" + latH + ")";
            cLong.innerHTML = cLongV + " (" + longH + ")";
        });
        google.maps.event.addListener(polygon, 'dragend', function () {
            if (globalVar.placerType == "poi") {
                globalVar.firstSavePOI = true;
                for (var i = 0; i < globalVar.poiObj.length; i++) {
                    if (globalVar.poiObj[i] == this) {
                        infoWindow[i].setPosition(polygonCenter(polygon));
                        infoWindow[i].open(null);
                        label[i].setPosition(polygonCenter(polygon));
                        label[i].setMap(map);
                    }
                }
            }
        });
        google.maps.event.addListener(polygon, 'click', function () {
            if (globalVar.placerType == "poi") {
                globalVar.firstSavePOI = true;
                for (var i = 0; i < globalVar.poiObj.length; i++) {
                    if (globalVar.poiObj[i] == this) {
                        infoWindow[i].setPosition(polygonCenter(polygon));
                        infoWindow[i].open(map);
                    }
                }
            }
        });
    });
    google.maps.event.addListener(drawingManager, 'polylinecomplete', function (polyline) {
        testBounds();
        if (globalVar.placerType == "poi") {
            globalVar.firstSavePOI = true;
            globalVar.poi_i++;
            var poiId = globalVar.poi_i + 1;
            globalVar.poiObj[globalVar.poi_i] = polyline;
            globalVar.poiType[globalVar.poi_i] = "polyline";
            var poiDescTemp = L_Line;
            document.getElementById("poiList").innerHTML += writeHTML("poiListItem", globalVar.poi_i, poiId, poiDescTemp);
            var contentString = writeHTML("poiDesc", globalVar.poi_i, "", "");
            infoWindow[globalVar.poi_i] = new google.maps.InfoWindow({
                content: contentString
            });
            //var bounds = new google.maps.LatLngBounds;
            //polyline.getPath().forEach(function (latLng) { bounds.extend(latLng); });
            //var polylineCenter = bounds.getCenter();
            //var bounds = new google.maps.LatLngBounds; //spatial center, bounds holder
            var polylinePoints = [];
            var polylinePointCount = 0;
            polyline.getPath().forEach(function (latLng) {
                polylinePoints[polylinePointCount] = latLng;
                de("polylinePoints[" + polylinePointCount + "] = " + latLng);
                polylinePointCount++;
            });
            de("polylinePointCount: " + polylinePointCount);
            de("polylinePoints.length: " + polylinePoints.length);
            de("Math.round((polylinePoints.length / 2)): " + Math.round((polylinePoints.length / 2)));
            var polylineCenterPoint = polylinePoints[Math.round((polylinePoints.length / 2))];
            de("polylineCenterPoint: " + polylineCenterPoint);
            var polylineStartPoint = polylinePoints[0];
            de("polylineStartPoint: " + polylineStartPoint);
            infoWindow[globalVar.poi_i].setPosition(polylineStartPoint);
            infoWindow[globalVar.poi_i].open(map);
            //best fix so far
            if (globalVar.poiCount == 0) {
                setTimeout(function () {
                    infoWindow[0].setMap(null);
                    //infoWindow[0].setOptions({ pixelOffset: new google.maps.Size(0, -40) });
                    infoWindow[0].setMap(map);
                }, 800);
            }
            //if (globalVar.poiCount > 0) {
            //    infoWindow[globalVar.poi_i].open(map);
            //} else {
            //    infoWindow[globalVar.poi_i].setMap(map);
            //    infoWindow[globalVar.poi_i].setMap(null);
            //    infoWindow[globalVar.poi_i].setMap(map);
            //}
            globalVar.poiCount++;

            label[globalVar.poi_i] = new MarkerWithLabel({
                //position: polylineCenter, //position of real marker
                //position: polylineCenterPoint, //position of real marker
                position: polylineStartPoint, //position at start of polyline
                zIndex: 2,
                map: map,
                labelContent: poiId, //the current user count
                labelAnchor: new google.maps.Point(15, 0),
                labelClass: "labels", // the CSS class for the label
                labelStyle: { opacity: 0.75 },
                icon: {} //initialize to nothing so no marker shows
            });

        }
        google.maps.event.addListener(polyline.getPath(), 'set_at', function () { //what is path?
            if (globalVar.placerType == "poi") {
                globalVar.firstSavePOI = true;
                de("is poi");
                for (var i = 0; i < globalVar.poiObj.length; i++) {
                    de("inside loop1");
                    if (globalVar.poiObj[i].getPath() == this) {
                        //var bounds = new google.maps.LatLngBounds;
                        //polyline.getPath().forEach(function (latLng) { bounds.extend(latLng); });
                        //var polylineCenter = bounds.getCenter();
                        //var bounds = new google.maps.LatLngBounds; //spatial center, bounds holder
                        var polylinePoints = [];
                        var polylinePointCount = 0;
                        de("here1");
                        polyline.getPath().forEach(function (latLng) {
                            polylinePoints[polylinePointCount] = latLng;
                            polylinePointCount++;
                        });
                        de("here2");
                        var polylineCenterPoint = polylinePoints[(polylinePoints.length / 2)];
                        var polylineStartPoint = polylinePoints[0];
                        infoWindow[globalVar.poi_i].setPosition(polylineStartPoint);
                        infoWindow[globalVar.poi_i].open(null);
                        label[i].setPosition(polylineStartPoint);
                        label[i].setMap(map);
                        de("here3");
                    }
                }

            }
        });
        google.maps.event.addListener(polyline, 'dragstart', function () {
            if (globalVar.placerType == "poi") {
                globalVar.firstSavePOI = true;
                for (var i = 0; i < globalVar.poiObj.length; i++) {
                    if (globalVar.poiObj[i] == this) {
                        infoWindow[i].setMap(null);
                        label[i].setMap(null);
                    }
                }
            }
        });
        google.maps.event.addListener(polyline, 'drag', function () {
            //used for lat/long tooll
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
            cLat.innerHTML = cLatV + " (" + latH + ")";
            cLong.innerHTML = cLongV + " (" + longH + ")";
        });
        google.maps.event.addListener(polyline, 'dragend', function () {
            if (globalVar.placerType == "poi") {
                globalVar.firstSavePOI = true;
                //var bounds = new google.maps.LatLngBounds;
                //polyline.getPath().forEach(function (latLng) { bounds.extend(latLng); });
                //var polylineCenter = bounds.getCenter();
                //var bounds = new google.maps.LatLngBounds; //spatial center, bounds holder

                for (var i = 0; i < globalVar.poiObj.length; i++) {
                    if (globalVar.poiObj[i] == this) {
                        var polylinePoints = [];
                        var polylinePointCount = 0;
                        polyline.getPath().forEach(function (latLng) {
                            polylinePoints[polylinePointCount] = latLng;
                            polylinePointCount++;
                        });
                        var polylineCenterPoint = polylinePoints[(polylinePoints.length / 2)];
                        var polylineStartPoint = polylinePoints[0];
                        infoWindow[i].setPosition(polylineStartPoint);
                        infoWindow[i].open(null);
                        label[i].setPosition(polylineStartPoint);
                        label[i].setMap(map);
                    }
                }

            }
        });
        google.maps.event.addListener(polyline, 'click', function () {
            if (globalVar.placerType == "poi") {
                globalVar.firstSavePOI = true;
                //var bounds = new google.maps.LatLngBounds;
                //polyline.getPath().forEach(function (latLng) { bounds.extend(latLng); });
                //var polylineCenter = bounds.getCenter();

                for (var i = 0; i < globalVar.poiObj.length; i++) {
                    if (globalVar.poiObj[i] == this) {
                        var polylinePoints = [];
                        var polylinePointCount = 0;
                        polyline.getPath().forEach(function (latLng) {
                            polylinePoints[polylinePointCount] = latLng;
                            polylinePointCount++;
                        });
                        var polylineCenterPoint = polylinePoints[(polylinePoints.length / 2)];
                        var polylineStartPoint = polylinePoints[0];
                        infoWindow[i].setPosition(polylineStartPoint);
                        infoWindow[i].open(map);
                    }
                }
            }
        });
    });

    //initialize map specific listeners

    //on right click stop drawing thing
    google.maps.event.addListener(map, 'rightclick', function () {
        drawingManager.setDrawingMode(null); //reset drawing manager no matter what
    });
    //used to display cursor location via lat/long
    google.maps.event.addDomListener(map, 'mousemove', function (point) {

        if (globalVar.cCoordsFrozen == "no") {
            //cCoord.innerHTML = point.latLng.toString(); //directly inject into page
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

            cLat.innerHTML = cLatV + " (" + latH + ")";
            cLong.innerHTML = cLongV + " (" + longH + ")";
        }

    });
    //drag listener (for boundary test)
    google.maps.event.addListener(map, 'dragend', function () {
        testBounds();
    });
    //check the zoom level display message if out limits
    google.maps.event.addListener(map, 'zoom_changed', function () {
        checkZoomLevel();
    });
    //when kml layer is clicked, get feature that was clicked
    google.maps.event.addListener(KmlLayer, 'click', function (kmlEvent) {
        var name = kmlEvent.featureData.name;
        displayMessage("ParcelID: " + name); //temp
    });

    //#endregion

    //initialize all the incoming geo obejects (the fcn is written via c#)
    initGeoObjects();

    //this part runs when the mapobject is created and rendered
    google.maps.event.addListenerOnce(map, 'tilesloaded', function () {
        initOptions(); //setup the graphical user interface (enhances visual effect to do all of this after map loads)
        initOverlayList(); //list all the overlays in the list box"
    });
}

//Displays all the circles sent from the C# code.
function displayIncomingCircles() {
    if (globalVar.incomingCircleCenter.length > 0) {
        for (var i = 0; i < globalVar.incomingCircleCenter.length; i++) {
            switch (globalVar.incomingCircleFeatureType[i]) {
                case "":
                    break;
                case "main":
                    break;
                case "poi":
                    de("incoming poi: " + i + " " + globalVar.incomingCircleLabel[i]);
                    globalVar.placerType = "poi";
                    var circle = new google.maps.Circle({
                        center: globalVar.incomingCircleCenter[i],
                        map: map,
                        draggable: true,
                        editable: true,
                        title: globalVar.incomingCircleLabel[i],
                        radius: globalVar.incomingCircleRadius[i]
                    });
                    if (globalVar.placerType == "poi") {
                        globalVar.firstSavePOI = true;
                        globalVar.poi_i++;

                        label[globalVar.poi_i] = new MarkerWithLabel({
                            position: circle.getCenter(), //position of real marker
                            zIndex: 2,
                            map: map,
                            labelContent: globalVar.incomingCircleLabel[i],
                            labelAnchor: new google.maps.Point(15, 0),
                            labelClass: "labels", // the CSS class for the label
                            labelStyle: { opacity: 0.75 },
                            icon: {} //initialize to nothing so no marker shows
                        });

                        var poiId = globalVar.poi_i + 1;
                        globalVar.poiObj[globalVar.poi_i] = circle;
                        globalVar.poiType[globalVar.poi_i] = "circle";
                        var poiDescTemp = globalVar.incomingCircleLabel[i];
                        document.getElementById("poiList").innerHTML += writeHTML("poiListItemIncoming", globalVar.poi_i, poiId, poiDescTemp);
                        globalVar.poiDesc[globalVar.poi_i] = poiDescTemp;
                        var contentString = writeHTML("poiDescIncoming", globalVar.poi_i, poiDescTemp, "");
                        infoWindow[globalVar.poi_i] = new google.maps.InfoWindow({
                            content: contentString
                        });
                        infoWindow[globalVar.poi_i].setPosition(circle.getCenter());
                        infoWindow[globalVar.poi_i].open(map);
                        globalVar.poiCount++;
                    }
                    google.maps.event.addListener(circle, 'dragstart', function () {

                        if (globalVar.placerType == "poi") {
                            globalVar.firstSavePOI = true;
                            for (var i = 0; i < globalVar.poiObj.length; i++) {
                                if (globalVar.poiObj[i] == this) {
                                    infoWindow[i].setMap(null);
                                    label[i].setMap(null);
                                }
                            }
                        }
                    });
                    google.maps.event.addListener(circle, 'drag', function () {
                        //used to get the center point for lat/long tool
                        globalVar.circleCenter = this.getCenter();
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
                        cLat.innerHTML = cLatV + " (" + latH + ")";
                        cLong.innerHTML = cLongV + " (" + longH + ")";
                    });
                    google.maps.event.addListener(circle, 'dragend', function () {
                        if (globalVar.placerType == "poi") {
                            globalVar.firstSavePOI = true;
                            for (var i = 0; i < globalVar.poiObj.length; i++) {
                                if (globalVar.poiObj[i] == this) {
                                    infoWindow[i].setPosition(this.getCenter());
                                    infoWindow[i].open(null);
                                    label[i].setPosition(this.getCenter());
                                    label[i].setMap(map);
                                }
                            }
                        }
                    });
                    google.maps.event.addListener(circle, 'click', function () {
                        if (globalVar.placerType == "poi") {
                            globalVar.firstSavePOI = true;
                            for (var i = 0; i < globalVar.poiObj.length; i++) {
                                if (globalVar.poiObj[i] == this) {
                                    infoWindow[i].setPosition(this.getCenter());
                                    infoWindow[i].open(map);
                                }
                            }
                        }
                    });
                    break;
            }
        }
    } else {
        //nothing
    }
    //once everything is drawn, determine if there are pois
    if (globalVar.poiCount > 0) {
        //close and reopen pois (to fix firefox bug)
        setTimeout(function () {
            globalVar.RIBMode = true;
            toggleVis("pois");
            toggleVis("pois");
            globalVar.RIBMode = false;
            //this hides the infowindows at startup
            for (var j = 0; j < globalVar.poiCount; j++) {
                infoWindow[j].setMap(null);
            }
        }, 1000);
    }
}

//Displays all the points sent from the C# code.
function displayIncomingPoints() {
    if (globalVar.incomingPointCenter) {
        //go through and display points as long as there is a point to display
        for (var i = 0; i < globalVar.incomingPointCenter.length; i++) {
            switch (globalVar.incomingPointFeatureType[i]) {
                case "":
                    globalVar.placerType = "item";
                    globalVar.firstMarker++;
                    globalVar.itemMarker = new google.maps.Marker({
                        position: globalVar.incomingPointCenter[i],
                        map: map,
                        draggable: true,
                        title: globalVar.incomingPointLabel[i]
                    });
                    document.getElementById('content_toolbox_posItem').value = globalVar.itemMarker.getPosition();
                    codeLatLng(globalVar.itemMarker.getPosition());
                    google.maps.event.addListener(globalVar.itemMarker, 'dragend', function () {
                        globalVar.firstSaveItem = true;
                        globalVar.savingMarkerCenter = globalVar.itemMarker.getPosition(); //store coords to save
                        document.getElementById('content_toolbox_posItem').value = globalVar.itemMarker.getPosition();
                        codeLatLng(globalVar.itemMarker.getPosition());
                    });
                    globalVar.mainCount++;
                    globalVar.incomingACL = "item";
                    break;
                case "main":
                    globalVar.placerType = "item";
                    globalVar.firstMarker++;
                    globalVar.itemMarker = new google.maps.Marker({
                        position: globalVar.incomingPointCenter[i],
                        map: map,
                        draggable: true,
                        title: globalVar.incomingPointLabel[i]
                    });
                    document.getElementById('content_toolbox_posItem').value = globalVar.itemMarker.getPosition();
                    codeLatLng(globalVar.itemMarker.getPosition());
                    google.maps.event.addListener(globalVar.itemMarker, 'dragend', function () {
                        globalVar.firstSaveItem = true;
                        globalVar.savingMarkerCenter = globalVar.itemMarker.getPosition(); //store coords to save
                        document.getElementById('content_toolbox_posItem').value = globalVar.itemMarker.getPosition();
                        codeLatLng(globalVar.itemMarker.getPosition());
                    });
                    globalVar.mainCount++;
                    globalVar.incomingACL = "item";
                    break;
                case "poi":
                    de("incoming poi: " + i + " " + globalVar.incomingPointLabel[i]);
                    globalVar.placerType = "poi";
                    var marker = new google.maps.Marker({
                        position: globalVar.incomingPointCenter[i],
                        map: map,
                        draggable: true,
                        title: globalVar.incomingPointLabel[i]
                    });
                    de("incoming center: " + marker.getPosition());
                    if (globalVar.placerType == "poi") {
                        globalVar.firstSavePOI = true;
                        globalVar.poi_i++;
                        label[globalVar.poi_i] = new MarkerWithLabel({
                            position: marker.getPosition(), //position of real marker
                            map: map,
                            zIndex: 2,
                            labelContent: globalVar.incomingPointLabel[(i)],
                            labelAnchor: new google.maps.Point(15, 0),
                            labelClass: "labels", // the CSS class for the label
                            labelStyle: { opacity: 0.75 },
                            icon: {} //initialize to nothing so no marker shows
                        });
                        globalVar.poiObj[globalVar.poi_i] = marker;
                        globalVar.poiType[globalVar.poi_i] = "marker";
                        var poiId = globalVar.poi_i + 1;
                        var poiDescTemp = globalVar.incomingPointLabel[i];
                        document.getElementById("poiList").innerHTML += writeHTML("poiListItemIncoming", globalVar.poi_i, poiId, poiDescTemp);
                        globalVar.poiDesc[globalVar.poi_i] = poiDescTemp;
                        var contentString = writeHTML("poiDescIncoming", globalVar.poi_i, poiDescTemp, "");
                        infoWindow[globalVar.poi_i] = new google.maps.InfoWindow({
                            content: contentString,
                            position: marker.getPosition()
                            //pixelOffset: new google.maps.Size(0, -40)
                        });
                        infoWindow[globalVar.poi_i].setMap(map);
                        infoWindow[globalVar.poi_i].open(map, globalVar.poiObj[globalVar.poi_i]);
                        globalVar.poiCount++;
                    }
                    google.maps.event.addListener(marker, 'dragstart', function () {
                        if (globalVar.placerType == "poi") {
                            globalVar.firstSavePOI = true;
                            for (var i = 0; i < globalVar.poiObj.length; i++) {
                                if (globalVar.poiObj[i] == this) {
                                    infoWindow[i].setMap(null);
                                    label[i].setMap(null);
                                }
                            }
                        }
                    });
                    google.maps.event.addListener(marker, 'dragend', function () {
                        if (globalVar.placerType == "poi") {
                            globalVar.firstSavePOI = true;
                            for (var i = 0; i < globalVar.poiObj.length; i++) {
                                if (globalVar.poiObj[i] == this) {
                                    infoWindow[i].setOptions({ position: this.getPosition(), pixelOffset: new google.maps.Size(0, -40) });
                                    infoWindow[i].open(null);
                                    label[i].setPosition(this.getPosition());
                                    label[i].setMap(map);
                                }
                            }
                        }
                    });
                    google.maps.event.addListener(marker, 'click', function () {
                        if (globalVar.placerType == "poi") {
                            globalVar.firstSavePOI = true;
                            for (var i = 0; i < globalVar.poiObj.length; i++) {
                                if (globalVar.poiObj[i] == this) {
                                    infoWindow[i].setOptions({ position: this.getPosition(), pixelOffset: new google.maps.Size(0, -40) });
                                    infoWindow[i].open(map);
                                }
                            }
                        }
                    });
                    break;
            }
        }
    } else {
        globalVar.firstMarker++;
        globalVar.itemMarker = new google.maps.Marker({
            position: map.getCenter(), //just get the center poin of the map
            map: null, //hide on load
            draggable: false,
            title: globalVar.incomingPointLabel[0]
        });
        //nothing to display because there is no geolocation of item
    }
    //once everything is drawn, determine if there are pois
    if (globalVar.poiCount > 0) {
        //close and reopen pois (to fix firefox bug)
        setTimeout(function () {
            globalVar.RIBMode = true;
            toggleVis("pois");
            toggleVis("pois");
            globalVar.RIBMode = false;
            //this hides the infowindows at startup
            for (var j = 0; j < globalVar.poiCount; j++) {
                infoWindow[j].setMap(null);
            }
        }, 1000);
    }
}

//Displays all the lines sent from the C# code.
function displayIncomingLines() {
    if (globalVar.incomingLinePath.length > 0) {
        for (var i = 0; i < globalVar.incomingLinePath.length; i++) {
            switch (globalVar.incomingLineFeatureType[i]) {
                case "":
                    break;
                case "main":
                    break;
                case "poi":
                    de("incoming poi: " + i + " " + globalVar.incomingLineLabel[i]);
                    globalVar.placerType = "poi";
                    var polyline = new google.maps.Polyline({
                        path: globalVar.incomingLinePath[i],
                        map: map,
                        draggable: true,
                        editable: true,
                        title: globalVar.incomingLineLabel[i]
                    });
                    if (globalVar.placerType == "poi") {
                        globalVar.firstSavePOI = true;
                        globalVar.poi_i++;
                        var poiId = globalVar.poi_i + 1;
                        globalVar.poiObj[globalVar.poi_i] = polyline;
                        globalVar.poiType[globalVar.poi_i] = "polyline";
                        var poiDescTemp = globalVar.incomingLineLabel[i];
                        document.getElementById("poiList").innerHTML += writeHTML("poiListItemIncoming", globalVar.poi_i, poiId, poiDescTemp);
                        globalVar.poiDesc[globalVar.poi_i] = poiDescTemp;
                        var contentString = writeHTML("poiDescIncoming", globalVar.poi_i, poiDescTemp, "");
                        infoWindow[globalVar.poi_i] = new google.maps.InfoWindow({
                            content: contentString
                        });
                        var polylinePoints = [];
                        var polylinePointCount = 0;
                        polyline.getPath().forEach(function (latLng) {
                            polylinePoints[polylinePointCount] = latLng;
                            de("polylinePoints[" + polylinePointCount + "] = " + latLng);
                            polylinePointCount++;
                        });
                        de("polylinePointCount: " + polylinePointCount);
                        de("polylinePoints.length: " + polylinePoints.length);
                        de("Math.round((polylinePoints.length / 2)): " + Math.round((polylinePoints.length / 2)));
                        var polylineCenterPoint = polylinePoints[Math.round((polylinePoints.length / 2))];
                        de("polylineCenterPoint: " + polylineCenterPoint);
                        var polylineStartPoint = polylinePoints[0];
                        de("polylineStartPoint: " + polylineStartPoint);
                        infoWindow[globalVar.poi_i].setPosition(polylineStartPoint);
                        infoWindow[globalVar.poi_i].open(map);
                        //best fix so far
                        if (globalVar.poiCount == 0) {
                            setTimeout(function () {
                                infoWindow[0].setMap(null);
                                infoWindow[0].setMap(map);
                            }, 800);
                        }
                        globalVar.poiCount++;
                        label[globalVar.poi_i] = new MarkerWithLabel({
                            position: polylineStartPoint, //position at start of polyline
                            zIndex: 2,
                            map: map,
                            labelContent: globalVar.incomingLineLabel[i],
                            labelAnchor: new google.maps.Point(15, 0),
                            labelClass: "labels", // the CSS class for the label
                            labelStyle: { opacity: 0.75 },
                            icon: {} //initialize to nothing so no marker shows
                        });
                    }
                    google.maps.event.addListener(polyline, 'mouseout', function () {
                        if (globalVar.placerType == "poi") {
                            globalVar.firstSavePOI = true;
                            for (var i = 0; i < globalVar.poiObj.length; i++) {
                                if (globalVar.poiObj[i] == this) {
                                    var polylinePoints = [];
                                    var polylinePointCount = 0;
                                    de("here1");
                                    this.getPath().forEach(function (latLng) {
                                        polylinePoints[polylinePointCount] = latLng;
                                        polylinePointCount++;
                                    });
                                    de("here2");
                                    var polylineCenterPoint = polylinePoints[(polylinePoints.length / 2)];
                                    var polylineStartPoint = polylinePoints[0];
                                    infoWindow[i].setPosition(polylineStartPoint);
                                    label[i].setPosition(polylineStartPoint);
                                    de("here3");
                                }
                            }
                        }
                    });
                    google.maps.event.addListener(polyline, 'dragstart', function () {
                        if (globalVar.placerType == "poi") {
                            globalVar.firstSavePOI = true;
                            for (var i = 0; i < globalVar.poiObj.length; i++) {
                                if (globalVar.poiObj[i] == this) {
                                    infoWindow[i].setMap(null);
                                    label[i].setMap(null);
                                }
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
                        cLat.innerHTML = cLatV + " (" + latH + ")";
                        cLong.innerHTML = cLongV + " (" + longH + ")";
                    });
                    google.maps.event.addListener(polyline, 'dragend', function () {
                        if (globalVar.placerType == "poi") {
                            globalVar.firstSavePOI = true;
                            //var bounds = new google.maps.LatLngBounds;
                            //polyline.getPath().forEach(function (latLng) { bounds.extend(latLng); });
                            //var polylineCenter = bounds.getCenter();
                            //var bounds = new google.maps.LatLngBounds; //spatial center, bounds holder

                            for (var i = 0; i < globalVar.poiObj.length; i++) {
                                if (globalVar.poiObj[i] == this) {
                                    var polylinePoints = [];
                                    var polylinePointCount = 0;
                                    this.getPath().forEach(function (latLng) {
                                        polylinePoints[polylinePointCount] = latLng;
                                        polylinePointCount++;
                                    });
                                    var polylineCenterPoint = polylinePoints[(polylinePoints.length / 2)];
                                    var polylineStartPoint = polylinePoints[0];
                                    infoWindow[i].setPosition(polylineStartPoint);
                                    infoWindow[i].open(null);
                                    label[i].setPosition(polylineStartPoint);
                                    label[i].setMap(map);
                                }
                            }

                        }
                    });
                    google.maps.event.addListener(polyline, 'click', function () {
                        if (globalVar.placerType == "poi") {
                            globalVar.firstSavePOI = true;
                            //var bounds = new google.maps.LatLngBounds;
                            //polyline.getPath().forEach(function (latLng) { bounds.extend(latLng); });
                            //var polylineCenter = bounds.getCenter();

                            for (var i = 0; i < globalVar.poiObj.length; i++) {
                                if (globalVar.poiObj[i] == this) {
                                    var polylinePoints = [];
                                    var polylinePointCount = 0;
                                    this.getPath().forEach(function (latLng) {
                                        polylinePoints[polylinePointCount] = latLng;
                                        polylinePointCount++;
                                    });
                                    var polylineCenterPoint = polylinePoints[(polylinePoints.length / 2)];
                                    var polylineStartPoint = polylinePoints[0];
                                    infoWindow[i].setPosition(polylineStartPoint);
                                    infoWindow[i].open(map);
                                }
                            }
                        }
                    });
                    break;
            }
        }
    } else {
        //nothing
    }
    //once everything is drawn, determine if there are pois
    if (globalVar.poiCount > 0) {
        //close and reopen pois (to fix firefox bug)
        setTimeout(function () {
            globalVar.RIBMode = true;
            toggleVis("pois");
            toggleVis("pois");
            globalVar.RIBMode = false;
            //this hides the infowindows at startup
            for (var j = 0; j < globalVar.poiCount; j++) {
                infoWindow[j].setMap(null);
            }
        }, 1000);
    }
}

//Displays all the overlays sent from the C# code. Also calls displayglobalVar.ghostOverlayRectangle.
function displayIncomingPolygons() {
    //go through and display overlays as long as there is an overlay to display
    for (var i = 0; i < globalVar.incomingPolygonPath.length; i++) {
        switch (globalVar.incomingPolygonFeatureType[i]) {
            case "hidden":
                //hidden do nothing
                break;
            case "":
                //globalVar.workingOverlayIndex = globalVar.incomingPolygonPageId[i];
                //create overlay with incoming
                globalVar.overlaysOnMap[globalVar.incomingPolygonPageId[i]] = new CustomOverlay(globalVar.incomingPolygonPageId[i], globalVar.incomingPolygonPath[i], globalVar.incomingPolygonSourceURL[i], map, globalVar.incomingPolygonRotation[i]);
                globalVar.currentlyEditing = "no";
                //set the overlay to the map
                globalVar.overlaysOnMap[globalVar.incomingPolygonPageId[i]].setMap(map);
                //set hotspot on top of overlay
                setGhostOverlay(globalVar.incomingPolygonPageId[i], globalVar.incomingPolygonPath[i]);
                de("I created ghost: " + globalVar.incomingPolygonPageId[i]);
                globalVar.mainCount++;
                globalVar.incomingACL = "overlay";
                drawingManager.setDrawingMode(null); //reset drawing manager no matter what
                globalVar.overlaysCurrentlyDisplayed = true;
                break;
            case "main":
                globalVar.workingOverlayIndex = globalVar.incomingPolygonPageId[i];
                //create overlay with incoming
                globalVar.overlaysOnMap[globalVar.incomingPolygonPageId[i]] = new CustomOverlay(globalVar.incomingPolygonPageId[i], globalVar.incomingPolygonPath[i], globalVar.incomingPolygonSourceURL[i], map, globalVar.incomingPolygonRotation[i]);
                globalVar.currentlyEditing = "no";
                //set the overlay to the map
                globalVar.overlaysOnMap[globalVar.incomingPolygonPageId[i]].setMap(map);
                //set hotspot on top of overlay
                setGhostOverlay(globalVar.incomingPolygonPageId[i], globalVar.incomingPolygonPath[i]);
                de("I created ghost: " + globalVar.incomingPolygonPageId[i]);
                globalVar.mainCount++;
                globalVar.incomingACL = "overlay";
                drawingManager.setDrawingMode(null); //reset drawing manager no matter what
                globalVar.overlaysCurrentlyDisplayed = true;
                break;
            case "poi":
                //globalVar.placerType = "poi";
                if (globalVar.placerType == "poi") {
                    //determine polygon type
                    if (globalVar.incomingPolygonPolygonType[i] == "rectangle") {
                        de("incoming poi: " + i + " " + globalVar.incomingPolygonLabel[i]);
                        de("detected incoming rectangle");
                        //convert path to a rectangle bounds
                        var pathCount = 0;
                        var polygon = new google.maps.Polygon({
                            paths: globalVar.incomingPolygonPath[i]
                        });
                        polygon.getPath().forEach(function () { pathCount++; });
                        if (pathCount == 2) {
                            de("pathcount: " + pathCount);
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
                            globalVar.incomingPolygonPath[i] = new google.maps.LatLngBounds(new google.maps.LatLng(l[3], l[2]), new google.maps.LatLng(l[1], l[4]));
                            //rectangle.setBounds([new google.maps.LatLng(l[1], l[4]), new google.maps.LatLng(l[3], l[4]), new google.maps.LatLng(l[3], l[2]), new google.maps.LatLng(l[1], l[2])]);
                        }
                        var rectangle = new google.maps.Rectangle({
                            bounds: globalVar.incomingPolygonPath[i],
                            map: map,
                            draggable: true,
                            editable: true,
                            title: globalVar.incomingPolygonLabel[i]
                        });
                        if (globalVar.placerType == "poi") {
                            globalVar.firstSavePOI = true;
                            globalVar.poi_i++;
                            label[globalVar.poi_i] = new MarkerWithLabel({
                                position: rectangle.getBounds().getCenter(), //position of real marker
                                zIndex: 2,
                                map: map,
                                labelContent: globalVar.incomingPolygonLabel[i],
                                labelAnchor: new google.maps.Point(15, 0),
                                labelClass: "labels", // the CSS class for the label
                                labelStyle: { opacity: 0.75 },
                                icon: {} //initialize to nothing so no marker shows
                            });
                            var poiId = globalVar.poi_i + 1;
                            globalVar.poiObj[globalVar.poi_i] = rectangle;
                            globalVar.poiType[globalVar.poi_i] = "rectangle";
                            var poiDescTemp = globalVar.incomingPolygonLabel[i];
                            document.getElementById("poiList").innerHTML += writeHTML("poiListItemIncoming", globalVar.poi_i, poiId, poiDescTemp);
                            globalVar.poiDesc[globalVar.poi_i] = poiDescTemp;
                            var contentString = writeHTML("poiDescIncoming", globalVar.poi_i, poiDescTemp, "");
                            infoWindow[globalVar.poi_i] = new google.maps.InfoWindow({
                                content: contentString
                            });
                            infoWindow[globalVar.poi_i].setPosition(rectangle.getBounds().getCenter());
                            infoWindow[globalVar.poi_i].open(map);
                            //best fix so far
                            if (globalVar.poiCount == 0) {
                                setTimeout(function () {
                                    infoWindow[0].setMap(null);
                                    infoWindow[0].setMap(map);
                                }, 800);
                            }
                            globalVar.poiCount++;
                        }
                        google.maps.event.addListener(rectangle, 'bounds_changed', function () {
                            if (globalVar.placerType == "poi") {
                                globalVar.firstSavePOI = true;
                                for (var i = 0; i < globalVar.poiObj.length; i++) {
                                    if (globalVar.poiObj[i] == this) {
                                        infoWindow[i].setPosition(this.getBounds().getCenter());
                                        infoWindow[i].setMap(null);
                                        label[i].setPosition(this.getBounds().getCenter());
                                        label[i].setMap(map);
                                    }
                                }
                            }
                        });
                        google.maps.event.addListener(rectangle, 'dragstart', function () {
                            if (globalVar.placerType == "poi") {
                                globalVar.firstSavePOI = true;
                                for (var i = 0; i < globalVar.poiObj.length; i++) {
                                    if (globalVar.poiObj[i] == this) {
                                        infoWindow[i].setMap(null);
                                        label[i].setMap(null);
                                    }
                                }
                            }
                        });
                        google.maps.event.addListener(rectangle, 'drag', function () {
                            if (globalVar.placerType == "poi") {
                                globalVar.firstSavePOI = true;
                                for (var i = 0; i < globalVar.poiObj.length; i++) {
                                    if (globalVar.poiObj[i] == this) {
                                        infoWindow[i].setMap(null);
                                        label[i].setMap(null);
                                    }
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
                            cLat.innerHTML = cLatV + " (" + latH + ")";
                            cLong.innerHTML = cLongV + " (" + longH + ")";
                        });
                        google.maps.event.addListener(rectangle, 'dragend', function () {
                            if (globalVar.placerType == "poi") {
                                globalVar.firstSavePOI = true;
                                for (var i = 0; i < globalVar.poiObj.length; i++) {
                                    if (globalVar.poiObj[i] == this) {
                                        infoWindow[i].setPosition(this.getBounds().getCenter());
                                        infoWindow[i].open(null);
                                        label[i].setPosition(this.getBounds().getCenter());
                                        label[i].setMap(map);
                                    }
                                }
                            }
                        });
                        google.maps.event.addListener(rectangle, 'click', function () {
                            if (globalVar.placerType == "poi") {
                                globalVar.firstSavePOI = true;
                                for (var i = 0; i < globalVar.poiObj.length; i++) {
                                    if (globalVar.poiObj[i] == this) {
                                        infoWindow[i].setPosition(this.getBounds().getCenter());
                                        infoWindow[i].open(map);
                                    }
                                }
                            }
                        });
                    } else {
                        var polygon = new google.maps.Polygon({
                            paths: globalVar.incomingPolygonPath[i],
                            map: map,
                            draggable: true,
                            editable: true,
                            title: globalVar.incomingPolygonLabel[i]
                        });
                        if (globalVar.placerType == "poi") {
                            globalVar.firstSavePOI = true;
                            globalVar.poi_i++;
                            label[globalVar.poi_i] = new MarkerWithLabel({
                                position: polygonCenter(polygon), //position of real marker
                                zIndex: 2,
                                map: map,
                                labelContent: globalVar.incomingPolygonLabel[i], //the current user count
                                labelAnchor: new google.maps.Point(15, 0),
                                labelClass: "labels", // the CSS class for the label
                                labelStyle: { opacity: 0.75 },
                                icon: {} //initialize to nothing so no marker shows
                            });
                            var poiId = globalVar.poi_i + 1;
                            globalVar.poiObj[globalVar.poi_i] = polygon;
                            globalVar.poiType[globalVar.poi_i] = "polygon";
                            var poiDescTemp = globalVar.incomingPolygonLabel[i];
                            document.getElementById("poiList").innerHTML += writeHTML("poiListItemIncoming", globalVar.poi_i, poiId, poiDescTemp);
                            globalVar.poiDesc[globalVar.poi_i] = poiDescTemp;
                            var contentString = writeHTML("poiDescIncoming", globalVar.poi_i, poiDescTemp, "");
                            infoWindow[globalVar.poi_i] = new google.maps.InfoWindow({
                                content: contentString
                            });
                            infoWindow[globalVar.poi_i].setPosition(polygonCenter(polygon));
                            infoWindow[globalVar.poi_i].open(map);
                            //best fix so far
                            if (globalVar.poiCount == 0) {
                                setTimeout(function () {
                                    infoWindow[0].setMap(null);
                                    infoWindow[0].setMap(map);
                                }, 800);
                            }
                            globalVar.poiCount++;
                        }
                        google.maps.event.addListener(polygon, 'mouseout', function () { //if bounds change
                            if (globalVar.placerType == "poi") {
                                globalVar.firstSavePOI = true;
                                for (var i = 0; i < globalVar.poiObj.length; i++) {
                                    if (globalVar.poiObj[i] == this) {
                                        infoWindow[i].setPosition(polygonCenter(this));
                                        label[i].setPosition(polygonCenter(this));
                                    }
                                }
                            }
                        });
                        google.maps.event.addListener(polygon, 'dragstart', function () {
                            if (globalVar.placerType == "poi") {
                                globalVar.firstSavePOI = true;
                                for (var i = 0; i < globalVar.poiObj.length; i++) {
                                    if (globalVar.poiObj[i] == this) {
                                        infoWindow[i].setMap(null);
                                        label[i].setMap(null);
                                    }
                                }
                            }
                        });
                        google.maps.event.addListener(polygon, 'drag', function () {
                            if (globalVar.placerType == "poi") {
                                globalVar.firstSavePOI = true;
                                for (var i = 0; i < globalVar.poiObj.length; i++) {
                                    if (globalVar.poiObj[i] == this) {
                                        infoWindow[i].setMap(null);
                                        label[i].setMap(null);
                                    }
                                }
                            }
                            //used for lat/long tool
                            var str = polygonCenter(this).toString();
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
                            cLat.innerHTML = cLatV + " (" + latH + ")";
                            cLong.innerHTML = cLongV + " (" + longH + ")";
                        });
                        google.maps.event.addListener(polygon, 'dragend', function () {
                            if (globalVar.placerType == "poi") {
                                globalVar.firstSavePOI = true;
                                for (var i = 0; i < globalVar.poiObj.length; i++) {
                                    if (globalVar.poiObj[i] == this) {
                                        infoWindow[i].setPosition(polygonCenter(this));
                                        infoWindow[i].open(null);
                                        label[i].setPosition(polygonCenter(this));
                                        label[i].setMap(map);
                                    }
                                }
                            }
                        });
                        google.maps.event.addListener(polygon, 'click', function () {
                            if (globalVar.placerType == "poi") {
                                globalVar.firstSavePOI = true;
                                for (var i = 0; i < globalVar.poiObj.length; i++) {
                                    if (globalVar.poiObj[i] == this) {
                                        infoWindow[i].setPosition(polygonCenter(this));
                                        infoWindow[i].open(map);
                                    }
                                }
                            }
                        });
                    }
                    globalVar.overlaysCurrentlyDisplayed = true;
                    //once everything is drawn, determine if there are pois
                    if (globalVar.poiCount > 0) {
                        //close and reopen pois (to fix firefox bug)
                        setTimeout(function () {
                            globalVar.RIBMode = true;
                            toggleVis("pois");
                            toggleVis("pois");
                            globalVar.RIBMode = false;
                            //this hides the infowindows at startup
                            for (var j = 0; j < globalVar.poiCount; j++) {
                                infoWindow[j].setMap(null);
                            }
                        }, 1000);
                    }
                }
                break;
        }
    }
}

//clears all incoming overlays
function clearIncomingOverlays() {
    //go through and display overlays as long as there is an overlay to display
    for (var i = 0; i < globalVar.incomingPolygonPageId.length; i++) {
        if (globalVar.overlaysOnMap[globalVar.incomingPolygonPageId[i]] != null) {
            globalVar.overlaysOnMap[globalVar.incomingPolygonPageId[i]].setMap(null);
            globalVar.overlaysOnMap[globalVar.incomingPolygonPageId[i]] = null;
            globalVar.ghostOverlayRectangle[globalVar.incomingPolygonPageId[i]].setMap(null);
            globalVar.ghostOverlayRectangle[globalVar.incomingPolygonPageId[i]] = null;
        } else {
            //do nothing
        }
    }
    //globalVar.overlaysCurrentlyDisplayed = false;
    globalVar.preservedOpacity = globalVar.defaultOpacity;
}

//Creates and sets the ghost overlays (used to tie actions with actual overlay)
function setGhostOverlay(ghostIndex, ghostBounds) {

    //create ghost directly over an overlay
    globalVar.ghostOverlayRectangle[ghostIndex] = new google.maps.Rectangle();        //init ghost
    globalVar.ghostOverlayRectangle[ghostIndex].setOptions(globalVar.ghosting);                 //set globalVar.ghosting 
    globalVar.ghostOverlayRectangle[ghostIndex].setBounds(ghostBounds);               //set bounds
    globalVar.ghostOverlayRectangle[ghostIndex].setMap(map);                          //set to map

    //create listener for if clicked
    google.maps.event.addListener(globalVar.ghostOverlayRectangle[ghostIndex], 'click', function () {
        if (globalVar.pageMode == "edit") {
            if (globalVar.currentlyEditing == "yes") {                                                            //if editing is being done, save
                if (globalVar.workingOverlayIndex == null) {
                    globalVar.currentlyEditing = "no";
                } else {
                    cacheSaveOverlay(ghostIndex);                                                       //trigger a cache of current working overlay
                    globalVar.ghostOverlayRectangle[globalVar.workingOverlayIndex].setOptions(globalVar.ghosting);                    //set globalVar.rectangle to globalVar.ghosting
                    globalVar.currentlyEditing = "no";                                                            //reset editing marker
                    globalVar.preservedRotation = 0;                                                              //reset preserved rotation
                }
            }
            if (globalVar.currentlyEditing == "no") {                                                             //if editing is not being done, start editing
                $("#toolbox").show();                                                                   //show the toolbox
                globalVar.toolboxDisplayed = true;                                                                //mark that the toolbox is open
                $("#toolboxTabs").accordion({ active: 3 });                                             //open edit overlay tab in toolbox
                globalVar.currentlyEditing = "yes";                                                               //enable editing marker
                globalVar.workingOverlayIndex = ghostIndex;                                                       //set this overay as the one being e
                globalVar.ghostOverlayRectangle[ghostIndex].setOptions(globalVar.editable);                                 //show ghost
                globalVar.currentTopZIndex++;                                                                     //iterate top z index
                document.getElementById("overlay" + ghostIndex).style.zIndex = globalVar.currentTopZIndex;        //bring overlay to front
                globalVar.ghostOverlayRectangle[ghostIndex].setOptions({ zIndex: globalVar.currentTopZIndex });             //bring ghost to front
                //set rotation if the overlay was previously saved
                if (globalVar.incomingPolygonRotation[i] != globalVar.savingOverlayRotation[i]) {
                    alert(1);
                    globalVar.preservedRotation = globalVar.savingOverlayRotation[i];
                }
                //for (var i = 0; i < globalVar.savingOverlayIndex.length; i++) {
                //    if (ghostIndex == globalVar.savingOverlayIndex[i]) {
                //        globalVar.preservedRotation = globalVar.savingOverlayRotation[i];
                //    }
                //}
            }
        }
    });

    //set listener for bounds changed
    google.maps.event.addListener(globalVar.ghostOverlayRectangle[ghostIndex], 'bounds_changed', function () {
        de("ghost index: " + ghostIndex);
        if (globalVar.pageMode == "edit") {
            //hide previous overlay
            globalVar.overlaysOnMap[ghostIndex].setMap(null);
            //delete previous overlay values
            globalVar.overlaysOnMap[ghostIndex] = null;
            //redraw the overlay within the new bounds
            globalVar.overlaysOnMap[ghostIndex] = new CustomOverlay(ghostIndex, globalVar.ghostOverlayRectangle[ghostIndex].getBounds(), globalVar.incomingPolygonSourceURL[(ghostIndex - 1)], map, globalVar.preservedRotation);
            //set the overlay with new bounds to the map
            globalVar.overlaysOnMap[ghostIndex].setMap(map);
            //enable editing marker
            globalVar.currentlyEditing = "yes";
            //trigger a cache of current working overlay
            cacheSaveOverlay(ghostIndex);
        }
    });

    //set listener for right click (fixes reset issue over overlays)
    google.maps.event.addListener(globalVar.ghostOverlayRectangle[ghostIndex], 'rightclick', function () {
        drawingManager.setDrawingMode(null); //reset drawing manager no matter what
    });

}

//Stores the overlays to save and their associated data
function cacheSaveOverlay(index) {
    de("caching save overlay <hr/>");
    de("incoming index: " + index);
    de("current save overlay index: " + globalVar.csoi);
    de("current working overlay index: " + globalVar.workingOverlayIndex);
    globalVar.csoi = index - 1;
    //is this the first save
    globalVar.firstSaveOverlay = true;
    //de("firstSaveOvelay: " + globalVar.firstSaveOverlay);
    //set overlay index to save
    globalVar.savingOverlayIndex[globalVar.csoi] = globalVar.csoi; //globalVar.workingOverlayIndex;
    de("globalVar.savingOverlayIndex[globalVar.csoi]: " + globalVar.savingOverlayIndex[globalVar.csoi]);
    de("globalVar.csoi: " + globalVar.csoi);
    //set label to save
    globalVar.savingOverlayLabel[globalVar.csoi] = globalVar.incomingPolygonLabel[globalVar.csoi];
    de("globalVar.savingOverlayLabel[globalVar.csoi]: " + globalVar.savingOverlayLabel[globalVar.csoi]);
    de("globalVar.incomingPolygonLabel[globalVar.csoi]: " + globalVar.incomingPolygonLabel[globalVar.csoi]);
    //set source url to save
    globalVar.savingOverlaySourceURL[globalVar.csoi] = globalVar.incomingPolygonSourceURL[globalVar.csoi];
    de("globalVar.savingOverlaySourceURL[globalVar.csoi]: " + globalVar.incomingPolygonSourceURL[globalVar.csoi]);
    //set bounds to save
    globalVar.savingOverlayBounds[globalVar.csoi] = globalVar.ghostOverlayRectangle[globalVar.workingOverlayIndex].getBounds();
    de("globalVar.savingOverlayBounds[globalVar.csoi]: " + globalVar.ghostOverlayRectangle[globalVar.workingOverlayIndex].getBounds());
    //set rotation to save
    alert("caching... \nindex: " + index + "\nwoi: " + globalVar.workingOverlayIndex + "\ncsoi: " + globalVar.csoi + "\nsor: " + globalVar.savingOverlayRotation[globalVar.csoi] + "\nipr: " + globalVar.incomingPolygonRotation[globalVar.csoi] + "\npr: " + globalVar.preservedRotation);
    if (globalVar.savingOverlayRotation[globalVar.csoi] != globalVar.incomingPolygonRotation[globalVar.csoi]) {
        globalVar.savingOverlayRotation[globalVar.csoi] = globalVar.preservedRotation;
        //alert("no match" + globalVar.preservedRotation);
    } else {
        //alert("match " + globalVar.savingOverlayRotation[globalVar.csoi]);
    }
    globalVar.savingOverlayRotation[globalVar.csoi] = globalVar.preservedRotation;
    de("globalVar.savingOverlayRotation[globalVar.csoi]: " + globalVar.savingOverlayRotation[globalVar.csoi]);
    //set the pageId to save
    globalVar.savingOverlayPageId[globalVar.csoi] = globalVar.incomingPolygonPageId[globalVar.csoi];
    de("globalVar.savingOverlayPageId[globalVar.csoi]: " + globalVar.incomingPolygonPageId[globalVar.csoi]);
    ////check to see if we just recached or if it was a unique cache
    //if (globalVar.savingOverlayIndex[globalVar.csoi] != index) {
    //    //iterate the current save overlay index   
    //    globalVar.csoi++;
    //}
    de("save overlay cached");
}

//Starts the creation of a custom overlay div which contains a rectangular image.
//Supporting URL: https://developers.google.com/maps/documentation/javascript/overlays#CustomOverlays
function CustomOverlay(id, bounds, image, map, rotation) {
    globalVar.overlayCount++;                   //iterate how many overlays have been drawn
    this.bounds_ = bounds;                      //set the bounds
    this.image_ = image;                        //set source url
    this.map_ = map;                            //set to map
    globalVar.preservedRotation = rotation;     //set the rotation
    this.div_ = null;                           //defines a property to hold the image's div. We'll actually create this div upon receipt of the onAdd() method so we'll leave it null for now.
    this.index_ = id;                           //set the index/id of this overlay
}

//#endregion

//#region other google functions

//Continues support for adding an custom overlay
//Supporting URL: https://developers.google.com/maps/documentation/javascript/overlays#CustomOverlays
// Note: an overlay's receipt of onAdd() indicates that the map's panes are now available for attaching the overlay to the map via the DOM.
CustomOverlay.prototype.onAdd = function () {

    // Create the DIV and set some basic attributes.
    var div = document.createElement("div");
    div.id = "overlay" + this.index_;
    div.style.borderStyle = 'none';
    div.style.borderWidth = '0px';
    div.style.position = 'absolute';
    div.style.opacity = globalVar.preservedOpacity;

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
CustomOverlay.prototype.draw = function () {
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

    //for a preserved rotation
    if (globalVar.preservedRotation != 0) {
        keepRotate(globalVar.preservedRotation);
    }

};

//Not currently used
//Supporting URL: https://developers.google.com/maps/documentation/javascript/overlays#CustomOverlays
CustomOverlay.prototype.onRemove = function () {
    this.div_.parentNode.removeChild(this.div_);
    this.div_ = null;
};

//start this whole mess once the google map is loaded
google.maps.event.addDomListener(window, 'load', initialize);

//start the whole thing
//todo make this dynamic and read from the collection
setupInterface("stAugustine"); //defines interface

//#endregion

//#region Define google map objects

//set the google map instance options
//supporting url: https://developers.google.com/maps/documentation/javascript/controls
var gmapPageDivId = "googleMap"; //get page div where google maps is to reside
var gmapOptions = {
    disableDefaultUI: false,                                    //set to false to start from a clean slate of map controls
    zoom: globalVar.defaultZoomLevel,                                     //starting zoom level
    minZoom: globalVar.maxZoomLevel,                                      //highest zoom out level
    center: globalVar.mapCenter,                                          //default center point
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
        //{
        //    featureType: "all", //poi
        //    elementType: "all", //labels
        //    stylers: [{ invert_lightness: "true" }]
        //},
        {
            featureType: "transit", //poi
            elementType: "labels", //labels
            stylers: [{ visibility: "off" }]
        }
    ]

};

//define drawing manager for this google maps instance
//support url: https://developers.google.com/maps/documentation/javascript/3.exp/reference#DrawingManager
var drawingManager = new google.maps.drawing.DrawingManager({
    //drawingMode: google.maps.drawing.OverlayType.MARKER, //set default/start type
    drawingControl: false,
    drawingControlOptions: {
        position: google.maps.ControlPosition.TOP_CENTER,
        drawingModes: [
          google.maps.drawing.OverlayType.MARKER,
          google.maps.drawing.OverlayType.CIRCLE,
          google.maps.drawing.OverlayType.RECTANGLE,
          google.maps.drawing.OverlayType.POLYGON,
          google.maps.drawing.OverlayType.POLYLINE
        ]
    },
    markerOptions: {
        draggable: true,
        zIndex: 5
    },
    circleOptions: {
        editable: true,
        draggable: true,
        zIndex: 5
    },
    polygonOptions: {
        editable: true,
        draggable: true,
        zIndex: 5
    },
    polylineOptions: {
        editable: true,
        draggable: true,
        zIndex: 5
    },
    rectangleOptions: {
        editable: true,
        draggable: true,
        zIndex: 5
    }
});
drawingManager.setOptions({
    drawingControl: true, drawingControlOptions: {
        position: google.maps.ControlPosition.RIGHT_TOP,
        drawingModes: [
            google.maps.drawing.OverlayType.MARKER,
            google.maps.drawing.OverlayType.CIRCLE,
            google.maps.drawing.OverlayType.RECTANGLE,
            google.maps.drawing.OverlayType.POLYGON,
            google.maps.drawing.OverlayType.POLYLINE
        ]
    }
});

KmlLayer.setOptions({ suppressinfowindows: true });

//define custom copyright control
//supporting url: https://developers.google.com/maps/documentation/javascript/controls#CustomControls
var copyrightNode = document.createElement('div');
copyrightNode.id = 'copyright-control';
copyrightNode.style.fontSize = '10px';
copyrightNode.style.color = '#333333';
copyrightNode.style.fontFamily = 'Arial, sans-serif';
copyrightNode.style.margin = '0 2px 2px 0';
copyrightNode.style.whiteSpace = 'nowrap';
copyrightNode.index = 0;
copyrightNode.style.backgroundColor = '#FFFFFF';
copyrightNode.style.opacity = 0.75;
copyrightNode.innerHTML = L1; //localization copyright

//define cursor lat long tool custom control
//supporting url: https://developers.google.com/maps/documentation/javascript/controls#CustomControls
var cursorLatLongTool = document.createElement('div');
cursorLatLongTool.id = 'cursorLatLongTool';
cursorLatLongTool.style.fontSize = '10px';
cursorLatLongTool.style.color = '#333333';
cursorLatLongTool.style.fontFamily = 'Arial, sans-serif';
cursorLatLongTool.style.margin = '0 2px 2px 0';
cursorLatLongTool.style.whiteSpace = 'nowrap';
cursorLatLongTool.index = 0;
cursorLatLongTool.style.backgroundColor = '#FFFFFF';
cursorLatLongTool.style.opacity = 0.75;
cursorLatLongTool.innerHTML = L2; //localization cursor lat/long tool

//buffer zone top left (used to push map controls down)
//supporting url: https://developers.google.com/maps/documentation/javascript/controls#CustomControls
toolbarBufferZone1 = document.createElement('div');
toolbarBufferZone1.id = 'toolbarBufferZone1';
toolbarBufferZone1.style.height = '50px';

//buffer zone top right
//supporting url: https://developers.google.com/maps/documentation/javascript/controls#CustomControls
toolbarBufferZone2 = document.createElement('div');
toolbarBufferZone2.id = 'toolbarBufferZone2';
toolbarBufferZone2.style.height = '50px';

//#endregion