//summary here

///<summary>Setups everything with user defined options</summary>
///<param name="collection" type="string">Specify the type of collection to load</param>
function setupInterface(collection) {
    ///<summary>Setups everything with user defined options</summary>
    ///<param name="collection" type="string">Specify the type of collection to load</param>
    //todo make this auto generated  

    switch (collection) {
        case "default":
            globalVars.baseImageDirURL = "default/images/mapedit/";                            //the default directory to the image files
            globalVars.mapLayerActive = "Roadmap";                                             //what map layer is displayed
            globalVars.mapDrawingManagerDisplayed = false;                                     //by default, is the drawing manager displayed (true/false)
            globalVars.mapCenter = new google.maps.LatLng(29.6480, -82.3482);                  //used to center map on load
            globalVars.mapControlsDisplayed = true;                                            //by default, are map controls displayed (true/false)
            globalVars.defaultDisplayDrawingMangerTool = false;                                //by default, is the drawingmanger displayed (true/false)
            globalVars.toolboxDisplayed = true;                                                //by default, is the toolbox displayed (true/false)
            globalVars.toolbarDisplayed = true;                                                //by default, is the toolbar open (yes/no)
            globalVars.kmlDisplayed = false;                                                   //by default, is kml layer on (yes/no)
            KmlLayer = new google.maps.KmlLayer("http://ufdc.ufl.edu/design/mapedit/stAugParel_v6.kmz");  //must be pingable by google
            globalVars.defaultZoomLevel = 13;                                                  //zoom level, starting
            globalVars.maxZoomLevel = 2;                                                       //max zoom out, default (21=lowest level, 1=highest level)
            globalVars.minZoomLevel_Terrain = 15;                                              //max zoom in, terrain
            globalVars.minZoomLevel_Satellite = 20;                                            //max zoom in, sat + hybrid
            globalVars.minZoomLevel_Roadmap = 21;                                              //max zoom in, roadmap (default)
            globalVars.minZoomLevel_BlockLot = 19;                                             //max zoom in, used for special layers not having default of roadmap
            globalVars.isCustomOverlay = false;                                                //used to determine if other overlays (block/lot etc) //unknown
            globalVars.preservedRotation = 0;                                                  //rotation, default
            globalVars.knobRotationValue = 0;                                                  //rotation to display by default 
            globalVars.preservedOpacity = 0.75;                                                 //opacity, default value (0-1,1=opaque)
            globalVars.strictBounds = null;                                                    //set the bounds for this google map instance (set to null for no bounds)
            globalVars.hasCustomMapType = true;                                                //used to determine if there is a custom maptype layer
            break;
        case "stAugustine":
            globalVars.baseImageDirURL = "default/images/mapedit/";                            //the default directory to the image files
            globalVars.mapDrawingManagerDisplayed = false;                                     //by default, is the drawing manager displayed (true/false)
            globalVars.mapLayerActive = "Roadmap";                                             //what map layer is displayed
            globalVars.mapCenter = new google.maps.LatLng(29.8944, -81.3147);                  //used to center map on load
            globalVars.mapControlsDisplayed = true;                                            //by default, are map controls displayed (true/false)
            globalVars.defaultDisplayDrawingMangerTool = false;                                //by default, is the drawingmanger displayed (true/false)
            globalVars.toolboxDisplayed = true;                                                //by default, is the toolbox displayed (true/false)
            globalVars.toolbarDisplayed = true;                                                //by default, is the toolbar open (yes/no)
            globalVars.kmlDisplayed = false;                                                   //by default, is kml layer on (yes/no)
            //KmlLayer = new google.maps.KmlLayer("http://ufdc.ufl.edu/design/mapedit/stAugParcel_v6.kmz");  //must be pingable by google
            KmlLayer = new google.maps.KmlLayer("http://ufdc.ufl.edu/design/mapedit/stAugParcel_v6.kmz");  //must be pingable by google
            globalVars.defaultZoomLevel = 14;                                                  //zoom level, starting
            globalVars.maxZoomLevel = 10;                                                      //max zoom out, default (21=lowest level, 1=highest level)
            globalVars.minZoomLevel_Terrain = 15;                                              //max zoom in, terrain
            globalVars.minZoomLevel_Satellite = 20;                                            //max zoom in, sat + hybrid
            globalVars.minZoomLevel_Roadmap = 21;                                              //max zoom in, roadmap (default)
            globalVars.minZoomLevel_BlockLot = 19;                                             //max zoom in, used for special layers not having default of roadmap
            globalVars.isCustomOverlay = false;                                                //used to determine if other overlays (block/lot etc) //unknown
            globalVars.preservedRotation = 0;                                                  //rotation, default
            globalVars.knobRotationValue = 0;                                                  //rotation to display by default 
            globalVars.preservedOpacity = 0.35;                                                 //opacity, default value (0-1,1=opaque)
            globalVars.hasCustomMapType = true;                                                //used to determine if there is a custom maptype layer
            globalVars.strictBounds = new google.maps.LatLngBounds(                            //set the bounds for this google map instance
                new google.maps.LatLng(29.78225755812941, -81.4306640625),
                new google.maps.LatLng(29.99181288866604, -81.1917114257)
            );
            break;
        case "custom":
            globalVars.baseImageDirURL = "default/images/mapedit/";                            //the default directory to the image files
            globalVars.mapDrawingManagerDisplayed = false;                                     //by default, is the drawing manager displayed (true/false)
            globalVars.mapLayerActive = "Roadmap";                                             //what map layer is displayed
            globalVars.mapCenter = new google.maps.LatLng(29.6480, -82.3482);                  //used to center map on load
            globalVars.mapControlsDisplayed = true;                                            //by default, are map controls displayed (true/false)
            globalVars.defaultDisplayDrawingMangerTool = false;                                //by default, is the drawingmanger displayed (true/false)
            globalVars.toolboxDisplayed = true;                                                //by default, is the toolbox displayed (true/false)
            globalVars.toolbarDisplayed = true;                                                //by default, is the toolbar open (yes/no)
            globalVars.kmlDisplayed = false;                                                   //by default, is kml layer on (yes/no)
            KmlLayer = new google.maps.KmlLayer("http://ufdc.ufl.edu/design/mapedit/parcels_2012_kmz_fldor.kmz");  //must be pingable by google
            globalVars.defaultZoomLevel = 13;                                                  //zoom level, starting
            globalVars.maxZoomLevel = 10;                                                      //max zoom out, default (21=lowest level, 1=highest level)
            globalVars.minZoomLevel_Terrain = 15;                                              //max zoom in, terrain
            globalVars.minZoomLevel_Satellite = 20;                                            //max zoom in, sat + hybrid
            globalVars.minZoomLevel_Roadmap = 21;                                              //max zoom in, roadmap (default)
            globalVars.minZoomLevel_BlockLot = 19;                                             //max zoom in, used for special layers not having default of roadmap
            globalVars.isCustomOverlay = false;                                                //used to determine if other overlays (block/lot etc) //unknown
            globalVars.preservedRotation = 0;                                                  //rotation, default
            globalVars.knobRotationValue = 0;                                                  //rotation to display by default 
            globalVars.preservedOpacity = 0.75;                                                 //opacity, default value (0-1,1=opaque)
            globalVars.hasCustomMapType = true;                                                //used to determine if there is a custom maptype layer
            globalVars.strictBounds = new google.maps.LatLngBounds(                            //set the bounds for this google map instance
                new google.maps.LatLng(29.21570636285318, -82.87811279296875),
                new google.maps.LatLng(30.07978967039041, -81.76300048828125)
            );
            break;
        case "florida":
            globalVars.baseImageDirURL = "default/images/mapedit/";                            //the default directory to the image files
            globalVars.mapDrawingManagerDisplayed = false;                                     //by default, is the drawing manager displayed (true/false)
            globalVars.mapLayerActive = "Roadmap";                                             //what map layer is displayed
            globalVars.mapCenter = new google.maps.LatLng(29.6480, -82.3482);                  //used to center map on load
            globalVars.mapControlsDisplayed = true;                                            //by default, are map controls displayed (true/false)
            globalVars.defaultDisplayDrawingMangerTool = false;                                //by default, is the drawingmanger displayed (true/false)
            globalVars.toolboxDisplayed = true;                                                //by default, is the toolbox displayed (true/false)
            globalVars.toolbarDisplayed = true;                                                //by default, is the toolbar open (yes/no)
            globalVars.kmlDisplayed = false;                                                   //by default, is kml layer on (yes/no)
            KmlLayer = new google.maps.KmlLayer("http://hlmatt.com/uf/kml/10.kml"); //must be pingable by google
            globalVars.defaultZoomLevel = 13;                                                  //zoom level, starting
            globalVars.maxZoomLevel = 1;                                                       //max zoom out, default (21=lowest level, 1=highest level)
            globalVars.minZoomLevel_Terrain = 15;                                              //max zoom in, terrain
            globalVars.minZoomLevel_Satellite = 20;                                            //max zoom in, sat + hybrid
            globalVars.minZoomLevel_Roadmap = 21;                                              //max zoom in, roadmap (default)
            globalVars.minZoomLevel_BlockLot = 19;                                             //max zoom in, used for special layers not having default of roadmap
            globalVars.isCustomOverlay = false;                                                //used to determine if other overlays (block/lot etc) 
            globalVars.preservedRotation = 0;                                                  //rotation, default
            globalVars.knobRotationValue = 0;                                                  //rotation to display by default 
            globalVars.preservedOpacity = 0.75;                                                 //opacity, default value (0-1,1=opaque)
            globalVars.hasCustomMapType = true;                                                //used to determine if there is a custom maptype layer
            globalVars.strictBounds = new google.maps.LatLngBounds(                            //set the bounds for this google map instance
                //new google.maps.LatLng(30.69420636285318, -88.04311279296875), //fl nw
                //new google.maps.LatLng(25.06678967039041, -77.33330048828125) //fl se
                //new google.maps.LatLng(24.55531738915811, -81.78283295288095), //fl sw
                //new google.maps.LatLng(30.79109834517092, -81.53709923706058) //fl ne
                //new google.maps.LatLng(29.5862, -82.4146), //gville
                //new google.maps.LatLng(29.7490, -82.2106)
                new google.maps.LatLng(22.053908635225607, -86.18838838405613), //east coast
                new google.maps.LatLng(36.06512404320089, -76.72320000000003)
            );

            //globalVars.strictBounds = new google.maps.LatLngBounds(                            //set the bounds for this google map instance
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
        if (globalVars.placerType == "item") {
            globalVars.firstSaveItem = true;
            //used to prevent multi markers
            if (globalVars.firstMarker > 0) {
                drawingManager.setDrawingMode(null); //only place one at a time
            } else {
                globalVars.firstMarker++;
                drawingManager.setDrawingMode(null); //only place one at a time
            }
            globalVars.itemMarker = marker; //assign globally
            de("marker placed");
            document.getElementById('content_toolbox_posItem').value = globalVars.itemMarker.getPosition();
            globalVars.savingMarkerCenter = globalVars.itemMarker.getPosition(); //store coords to save
            codeLatLng(globalVars.itemMarker.getPosition());
        }

        if (globalVars.placerType == "poi") {
            globalVars.firstSavePOI = true;
            globalVars.poi_i++;

            label[globalVars.poi_i] = new MarkerWithLabel({
                position: marker.getPosition(), //position of real marker
                map: map,
                zIndex: 2,
                labelContent: globalVars.poi_i + 1, //the current user count
                labelAnchor: new google.maps.Point(15, 0),
                labelClass: "labels", // the CSS class for the label
                labelStyle: { opacity: 0.75 },
                icon: {} //initialize to nothing so no marker shows
            });

            globalVars.poiObj[globalVars.poi_i] = marker;
            globalVars.poiType[globalVars.poi_i] = "marker";
            var poiId = globalVars.poi_i + 1;
            var poiDescTemp = L_Marker;
            document.getElementById("poiList").innerHTML += writeHTML("poiListItem", globalVars.poi_i, poiId, poiDescTemp);
            var contentString = writeHTML("poiDesc", globalVars.poi_i, "", "");
            infoWindow[globalVars.poi_i] = new google.maps.InfoWindow({
                content: contentString,
                position: marker.getPosition(),
                pixelOffset: new google.maps.Size(0, -40)
            });
            //infoWindow[globalVars.poi_i].open(map, globalVars.poiObj[globalVars.poi_i]);
            if (globalVars.poiCount > 0) {
                infoWindow[globalVars.poi_i].open(map);
            } else {
                globalVars.poiCount++;
                infoWindow[globalVars.poi_i].setMap(map);
                infoWindow[globalVars.poi_i].setMap(null);
                infoWindow[globalVars.poi_i].setMap(map);
            }
        }

        google.maps.event.addListener(marker, 'dragstart', function () {

            if (globalVars.placerType == "poi") {
                globalVars.firstSavePOI = true;
                for (var i = 0; i < globalVars.poiObj.length; i++) {
                    if (globalVars.poiObj[i] == this) {
                        infoWindow[i].setMap(null);
                        label[i].setMap(null);
                    }
                }
            }
        });
        google.maps.event.addListener(marker, 'dragend', function () {
            if (globalVars.placerType == "item") {
                globalVars.firstSaveItem = true;
                document.getElementById('content_toolbox_posItem').value = marker.getPosition();
                codeLatLng(marker.getPosition());
            }
            if (globalVars.placerType == "poi") {
                globalVars.firstSavePOI = true;
                for (var i = 0; i < globalVars.poiObj.length; i++) {
                    if (globalVars.poiObj[i] == this) {
                        infoWindow[i].setOptions({ position: marker.getPosition(), pixelOffset: new google.maps.Size(0, -40) });
                        infoWindow[i].open(null);
                        label[i].setPosition(marker.getPosition());
                        label[i].setMap(map);
                    }
                }
            }
        });
        google.maps.event.addListener(marker, 'click', function () {
            if (globalVars.placerType == "poi") {
                globalVars.firstSavePOI = true;
                for (var i = 0; i < globalVars.poiObj.length; i++) {
                    if (globalVars.poiObj[i] == this) {
                        infoWindow[i].setOptions({ position: marker.getPosition(), pixelOffset: new google.maps.Size(0, -40) });
                        infoWindow[i].open(map);
                    }
                }
            }
        });
    });
    google.maps.event.addListener(drawingManager, 'circlecomplete', function (circle) {
        testBounds();
        if (globalVars.placerType == "poi") {
            globalVars.firstSavePOI = true;
            globalVars.poi_i++;

            label[globalVars.poi_i] = new MarkerWithLabel({
                position: circle.getCenter(), //position of real marker
                zIndex: 2,
                map: map,
                labelContent: globalVars.poi_i + 1, //the current user count
                labelAnchor: new google.maps.Point(15, 0),
                labelClass: "labels", // the CSS class for the label
                labelStyle: { opacity: 0.75 },
                icon: {} //initialize to nothing so no marker shows
            });

            var poiId = globalVars.poi_i + 1;
            globalVars.poiObj[globalVars.poi_i] = circle;
            globalVars.poiType[globalVars.poi_i] = "circle";
            var poiDescTemp = L_Circle;
            document.getElementById("poiList").innerHTML += writeHTML("poiListItem", globalVars.poi_i, poiId, poiDescTemp);
            var contentString = writeHTML("poiDesc", globalVars.poi_i, "", "");
            infoWindow[globalVars.poi_i] = new google.maps.InfoWindow({
                content: contentString
            });
            infoWindow[globalVars.poi_i].setPosition(circle.getCenter());
            if (globalVars.poiCount > 0) {
                infoWindow[globalVars.poi_i].open(map);
            } else {
                globalVars.poiCount++;
                infoWindow[globalVars.poi_i].setMap(map);
                infoWindow[globalVars.poi_i].setMap(null);
                infoWindow[globalVars.poi_i].setMap(map);
            }
        }

        google.maps.event.addListener(circle, 'dragstart', function () {

            if (globalVars.placerType == "poi") {
                globalVars.firstSavePOI = true;
                for (var i = 0; i < globalVars.poiObj.length; i++) {
                    if (globalVars.poiObj[i] == this) {
                        infoWindow[i].setMap(null);
                        label[i].setMap(null);
                    }
                }
            }
        });
        google.maps.event.addListener(circle, 'drag', function () {
            //used to get the center point for lat/long tool
            globalVars.circleCenter = circle.getCenter();
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
            if (globalVars.placerType == "poi") {
                globalVars.firstSavePOI = true;
                for (var i = 0; i < globalVars.poiObj.length; i++) {
                    if (globalVars.poiObj[i] == this) {
                        infoWindow[i].setPosition(circle.getCenter());
                        infoWindow[i].open(null);
                        label[i].setPosition(circle.getCenter());
                        label[i].setMap(map);
                    }
                }
            }
        });
        google.maps.event.addListener(circle, 'click', function () {
            if (globalVars.placerType == "poi") {
                globalVars.firstSavePOI = true;
                for (var i = 0; i < globalVars.poiObj.length; i++) {
                    if (globalVars.poiObj[i] == this) {
                        infoWindow[i].setPosition(circle.getCenter());
                        infoWindow[i].open(map);
                    }
                }
            }
        });
    });
    google.maps.event.addListener(drawingManager, 'rectanglecomplete', function (rectangle) {

        testBounds();                                   //check the bounds to make sure you havent strayed too far away
        if (globalVars.placerType == "overlay") {
            //mark that we are editing
            globalVars.firstSaveOverlay = true;

            //add the incoming overlay bounds
            globalVars.incomingOverlayBounds[0] = rectangle.getBounds();

            //redisplay overlays (the one we just made)
            displayIncomingOverlays();

            //relist the overlay we drew
            initOverlayList();

            //hide the rectangle we drew
            rectangle.setMap(null);

            //prevent redraw
            drawingManager.setDrawingMode(null);

            //create cache save overlay for the new converted overlay only
            if (globalVars.isConvertedOverlay == true) {
                cacheSaveOverlay(globalVars.workingOverlayIndex);
            }
        }


        if (globalVars.placerType == "poi") {
            globalVars.firstSavePOI = true;
            globalVars.poi_i++;

            label[globalVars.poi_i] = new MarkerWithLabel({
                position: rectangle.getBounds().getCenter(), //position of real marker
                zIndex: 2,
                map: map,
                labelContent: globalVars.poi_i + 1, //the current user count
                labelAnchor: new google.maps.Point(15, 0),
                labelClass: "labels", // the CSS class for the label
                labelStyle: { opacity: 0.75 },
                icon: {} //initialize to nothing so no marker shows
            });

            var poiId = globalVars.poi_i + 1;
            globalVars.poiObj[globalVars.poi_i] = rectangle;
            globalVars.poiType[globalVars.poi_i] = "rectangle";
            var poiDescTemp = L_Rectangle;
            document.getElementById("poiList").innerHTML += writeHTML("poiListItem", globalVars.poi_i, poiId, poiDescTemp);
            var contentString = writeHTML("poiDesc", globalVars.poi_i, "", "");
            infoWindow[globalVars.poi_i] = new google.maps.InfoWindow({
                content: contentString
            });
            infoWindow[globalVars.poi_i].setPosition(rectangle.getBounds().getCenter());
            if (globalVars.poiCount > 0) {
                infoWindow[globalVars.poi_i].open(map);
            } else {
                globalVars.poiCount++;
                infoWindow[globalVars.poi_i].setMap(map);
                infoWindow[globalVars.poi_i].setMap(null);
                infoWindow[globalVars.poi_i].setMap(map);
            }

        }

        google.maps.event.addListener(rectangle, 'bounds_changed', function () {
            if (globalVars.placerType == "poi") {
                globalVars.firstSavePOI = true;
                for (var i = 0; i < globalVars.poiObj.length; i++) {
                    if (globalVars.poiObj[i] == this) {
                        infoWindow[i].setPosition(rectangle.getBounds().getCenter());
                        infoWindow[i].setMap(null);
                        label[i].setPosition(rectangle.getBounds().getCenter());
                        label[i].setMap(map);
                    }
                }
            }
        });
        google.maps.event.addListener(rectangle, 'dragstart', function () {
            if (globalVars.placerType == "poi") {
                globalVars.firstSavePOI = true;
                for (var i = 0; i < globalVars.poiObj.length; i++) {
                    if (globalVars.poiObj[i] == this) {
                        infoWindow[i].setMap(null);
                        label[i].setMap(null);
                    }
                }
            }
        });
        google.maps.event.addListener(rectangle, 'drag', function () {
            if (globalVars.placerType == "poi") {
                globalVars.firstSavePOI = true;
                for (var i = 0; i < globalVars.poiObj.length; i++) {
                    if (globalVars.poiObj[i] == this) {
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
            if (globalVars.placerType == "poi") {
                globalVars.firstSavePOI = true;
                for (var i = 0; i < globalVars.poiObj.length; i++) {
                    if (globalVars.poiObj[i] == this) {
                        infoWindow[i].setPosition(rectangle.getBounds().getCenter());
                        infoWindow[i].open(null);
                        label[i].setPosition(rectangle.getBounds().getCenter());
                        label[i].setMap(map);
                    }
                }
            }
        });
        google.maps.event.addListener(rectangle, 'click', function () {
            if (globalVars.placerType == "poi") {
                globalVars.firstSavePOI = true;
                for (var i = 0; i < globalVars.poiObj.length; i++) {
                    if (globalVars.poiObj[i] == this) {
                        infoWindow[i].setPosition(rectangle.getBounds().getCenter());
                        infoWindow[i].open(map);
                    }
                }
            }
        });

    });
    google.maps.event.addListener(drawingManager, 'polygoncomplete', function (polygon) {
        testBounds();
        if (globalVars.placerType == "poi") {
            globalVars.firstSavePOI = true;
            globalVars.poi_i++;

            label[globalVars.poi_i] = new MarkerWithLabel({
                position: polygonCenter(polygon), //position of real marker
                zIndex: 2,
                map: map,
                labelContent: globalVars.poi_i + 1, //the current user count
                labelAnchor: new google.maps.Point(15, 0),
                labelClass: "labels", // the CSS class for the label
                labelStyle: { opacity: 0.75 },
                icon: {} //initialize to nothing so no marker shows
            });

            var poiId = globalVars.poi_i + 1;
            globalVars.poiObj[globalVars.poi_i] = polygon;
            globalVars.poiType[globalVars.poi_i] = "polygon";
            var poiDescTemp = L_Polygon;
            document.getElementById("poiList").innerHTML += writeHTML("poiListItem", globalVars.poi_i, poiId, poiDescTemp);
            var contentString = writeHTML("poiDesc", globalVars.poi_i, "", "");
            infoWindow[globalVars.poi_i] = new google.maps.InfoWindow({
                content: contentString
            });
            infoWindow[globalVars.poi_i].setPosition(polygonCenter(polygon));
            if (globalVars.poiCount > 0) {
                infoWindow[globalVars.poi_i].open(map);
            } else {
                globalVars.poiCount++;
                infoWindow[globalVars.poi_i].setMap(map);
                infoWindow[globalVars.poi_i].setMap(null);
                infoWindow[globalVars.poi_i].setMap(map);
            }
        }
        google.maps.event.addListener(polygon.getPath(), 'set_at', function () { //if bounds change
            if (globalVars.placerType == "poi") {
                globalVars.firstSavePOI = true;
                for (var i = 0; i < globalVars.poiObj.length; i++) {
                    if (globalVars.poiObj[i] == this) {
                        infoWindow[i].setPosition(polygonCenter(polygon));
                        infoWindow[i].setMap(null);
                        label[i].setPosition(polygonCenter(polygon));
                        label[i].setMap(map); //does not redisplay
                    }
                }
            }
        });
        google.maps.event.addListener(polygon, 'dragstart', function () {
            if (globalVars.placerType == "poi") {
                globalVars.firstSavePOI = true;
                for (var i = 0; i < globalVars.poiObj.length; i++) {
                    if (globalVars.poiObj[i] == this) {
                        infoWindow[i].setMap(null);
                        label[i].setMap(null);
                    }
                }
            }
        });
        google.maps.event.addListener(polygon, 'drag', function () {
            if (globalVars.placerType == "poi") {
                globalVars.firstSavePOI = true;
                for (var i = 0; i < globalVars.poiObj.length; i++) {
                    if (globalVars.poiObj[i] == this) {
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
            if (globalVars.placerType == "poi") {
                globalVars.firstSavePOI = true;
                for (var i = 0; i < globalVars.poiObj.length; i++) {
                    if (globalVars.poiObj[i] == this) {
                        infoWindow[i].setPosition(polygonCenter(polygon));
                        infoWindow[i].open(null);
                        label[i].setPosition(polygonCenter(polygon));
                        label[i].setMap(map);
                    }
                }
            }
        });
        google.maps.event.addListener(polygon, 'click', function () {
            if (globalVars.placerType == "poi") {
                globalVars.firstSavePOI = true;
                for (var i = 0; i < globalVars.poiObj.length; i++) {
                    if (globalVars.poiObj[i] == this) {
                        infoWindow[i].setPosition(polygonCenter(polygon));
                        infoWindow[i].open(map);
                    }
                }
            }
        });
    });
    google.maps.event.addListener(drawingManager, 'polylinecomplete', function (polyline) {
        testBounds();
        if (globalVars.placerType == "poi") {
            globalVars.firstSavePOI = true;
            globalVars.poi_i++;
            var poiId = globalVars.poi_i + 1;
            globalVars.poiObj[globalVars.poi_i] = polyline;
            globalVars.poiType[globalVars.poi_i] = "polyline";
            var poiDescTemp = L_Line;
            document.getElementById("poiList").innerHTML += writeHTML("poiListItem", globalVars.poi_i, poiId, poiDescTemp);
            var contentString = writeHTML("poiDesc", globalVars.poi_i, "", "");
            infoWindow[globalVars.poi_i] = new google.maps.InfoWindow({
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
            infoWindow[globalVars.poi_i].setPosition(polylineStartPoint);
            if (globalVars.poiCount > 0) {
                infoWindow[globalVars.poi_i].open(map);
            } else {
                globalVars.poiCount++;
                infoWindow[globalVars.poi_i].setMap(map);
                infoWindow[globalVars.poi_i].setMap(null);
                infoWindow[globalVars.poi_i].setMap(map);
            }

            label[globalVars.poi_i] = new MarkerWithLabel({
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
            if (globalVars.placerType == "poi") {
                globalVars.firstSavePOI = true;
                de("is poi");
                for (var i = 0; i < globalVars.poiObj.length; i++) {
                    de("inside loop1");
                    if (globalVars.poiObj[i].getPath() == this) {
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
                        infoWindow[globalVars.poi_i].setPosition(polylineStartPoint);
                        infoWindow[globalVars.poi_i].open(null);
                        label[i].setPosition(polylineStartPoint);
                        label[i].setMap(map);
                        de("here3");
                    }
                }

            }
        });
        google.maps.event.addListener(polyline, 'dragstart', function () {
            if (globalVars.placerType == "poi") {
                globalVars.firstSavePOI = true;
                for (var i = 0; i < globalVars.poiObj.length; i++) {
                    if (globalVars.poiObj[i] == this) {
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
            if (globalVars.placerType == "poi") {
                globalVars.firstSavePOI = true;
                //var bounds = new google.maps.LatLngBounds;
                //polyline.getPath().forEach(function (latLng) { bounds.extend(latLng); });
                //var polylineCenter = bounds.getCenter();
                //var bounds = new google.maps.LatLngBounds; //spatial center, bounds holder

                for (var i = 0; i < globalVars.poiObj.length; i++) {
                    if (globalVars.poiObj[i] == this) {
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
            if (globalVars.placerType == "poi") {
                globalVars.firstSavePOI = true;
                //var bounds = new google.maps.LatLngBounds;
                //polyline.getPath().forEach(function (latLng) { bounds.extend(latLng); });
                //var polylineCenter = bounds.getCenter();

                for (var i = 0; i < globalVars.poiObj.length; i++) {
                    if (globalVars.poiObj[i] == this) {
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

    google.maps.event.addDomListener(map, 'mousemove', function (point) {

        if (globalVars.cCoordsFrozen == "no") {
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

    });                                    //used to display cursor location via lat/long
    google.maps.event.addListener(map, 'dragend', function () {
        testBounds();
    });                                              //drag listener (for boundary test)
    google.maps.event.addListener(map, 'zoom_changed', function () {
        checkZoomLevel();
    });                                         //check the zoom level display message if out limits

    //when kml layer is clicked, get feature that was clicked
    google.maps.event.addListener(KmlLayer, 'click', function (kmlEvent) {
        var name = kmlEvent.featureData.name;
        displayMessage("ParcelID: " + name); //temp
    });

    //#endregion

    initGeoObjects(); //initialize all the incoming geo obejects (the fcn is written via c#)

    google.maps.event.addListenerOnce(map, 'tilesloaded', function () {
        //this part runs when the mapobject is created and rendered
        initOptions(); //setup the graphical user interface (enhances visual effect to do all of this after map loads)
        initOverlayList(); //list all the overlays in the list box"
    });
}

//Displays all the points sent from the C# code.
function displayIncomingPoints() {
    //go through and display points as long as there is a point to display (note, currently only supports one point)
    for (var i = 0; i < globalVars.incomingPointCenter.length; i++) {
        globalVars.firstMarker++;
        globalVars.itemMarker = new google.maps.Marker({
            position: globalVars.incomingPointCenter[i],
            map: map,
            draggable: true,
            title: globalVars.incomingPointLabel[i]
        });
        document.getElementById('content_toolbox_posItem').value = globalVars.itemMarker.getPosition();
        codeLatLng(globalVars.itemMarker.getPosition());
        google.maps.event.addListener(globalVars.itemMarker, 'dragend', function () {
            globalVars.firstSaveItem = true;
            globalVars.savingMarkerCenter = globalVars.itemMarker.getPosition(); //store coords to save
            document.getElementById('content_toolbox_posItem').value = globalVars.itemMarker.getPosition();
            codeLatLng(globalVars.itemMarker.getPosition());
        });
    }
}

//Displays all the overlays sent from the C# code. Also calls displayglobalVars.ghostOverlayRectangle.
function displayIncomingOverlays() {
    for (var i = 0; i < globalVars.incomingOverlayBounds.length; i++) {                                                                                //go through and display overlays as long as there is an overlay to display
        globalVars.overlaysOnMap[i] = new CustomOverlay(i, globalVars.incomingOverlayBounds[i], globalVars.incomingOverlaySourceURL[i], map, globalVars.incomingOverlayRotation[i]);    //create overlay with incoming
        globalVars.overlaysOnMap[i].setMap(map);                                                                                                       //set the overlay to the map

        setGhostOverlay(i, globalVars.incomingOverlayBounds[i]);                                                                                       //set hotspot on top of overlay
        de("I created ghost: " + i);
    }
    globalVars.overlaysCurrentlyDisplayed = true;
}

//clears all incoming overlays
function clearIncomingOverlays() {
    for (var i = 0; i < globalVars.incomingOverlayBounds.length; i++) {                                                                                //go through and display overlays as long as there is an overlay to display
        globalVars.overlaysOnMap[i].setMap(null);
        globalVars.overlaysOnMap[i] = null;
        globalVars.ghostOverlayRectangle[i].setMap(null);
        globalVars.ghostOverlayRectangle[i] = null;
    }
    //globalVars.overlaysCurrentlyDisplayed = false;
    globalVars.preservedOpacity = globalVars.defaultOpacity;
}

//Creates and sets the ghost overlays (used to tie actions with actual overlay)
function setGhostOverlay(ghostIndex, ghostBounds) {

    //create ghost directly over an overlay
    globalVars.ghostOverlayRectangle[ghostIndex] = new google.maps.Rectangle();        //init ghost
    globalVars.ghostOverlayRectangle[ghostIndex].setOptions(globalVars.ghosting);                 //set globalVars.ghosting 
    globalVars.ghostOverlayRectangle[ghostIndex].setBounds(ghostBounds);               //set bounds
    globalVars.ghostOverlayRectangle[ghostIndex].setMap(map);                          //set to map

    //create listener for if clicked
    google.maps.event.addListener(globalVars.ghostOverlayRectangle[ghostIndex], 'click', function () {
        if (globalVars.pageMode == "edit") {
            if (globalVars.currentlyEditing == "yes") {                                                            //if editing is being done, save
                if (globalVars.workingOverlayIndex == null) {
                    globalVars.currentlyEditing = "no";
                } else {
                    cacheSaveOverlay(ghostIndex);                                                       //trigger a cache of current working overlay
                    globalVars.ghostOverlayRectangle[globalVars.workingOverlayIndex].setOptions(globalVars.ghosting);                    //set globalVars.rectangle to globalVars.ghosting
                    globalVars.currentlyEditing = "no";                                                            //reset editing marker
                    globalVars.preservedRotation = 0;                                                              //reset preserved rotation
                }
            }
            if (globalVars.currentlyEditing == "no") {                                                             //if editing is not being done, start editing
                $("#toolbox").show();                                                                   //show the toolbox
                globalVars.toolboxDisplayed = true;                                                                //mark that the toolbox is open
                $("#toolboxTabs").accordion({ active: 3 });                                             //open edit overlay tab in toolbox
                globalVars.currentlyEditing = "yes";                                                               //enable editing marker
                globalVars.workingOverlayIndex = ghostIndex;                                                       //set this overay as the one being e
                globalVars.ghostOverlayRectangle[ghostIndex].setOptions(globalVars.editable);                                 //show ghost
                globalVars.currentTopZIndex++;                                                                     //iterate top z index
                document.getElementById("overlay" + ghostIndex).style.zIndex = globalVars.currentTopZIndex;        //bring overlay to front
                globalVars.ghostOverlayRectangle[ghostIndex].setOptions({ zIndex: globalVars.currentTopZIndex });             //bring ghost to front
                for (var i = 0; i < globalVars.savingOverlayIndex.length; i++) {                                   //set rotation if the overlay was previously saved
                    if (ghostIndex == globalVars.savingOverlayIndex[i]) {
                        globalVars.preservedRotation = globalVars.savingOverlayRotation[i];
                    }
                }
            }
        }
    });

    //set listener for bounds changed
    google.maps.event.addListener(globalVars.ghostOverlayRectangle[ghostIndex], 'bounds_changed', function () {
        if (globalVars.pageMode == "edit") {
            globalVars.overlaysOnMap[ghostIndex].setMap(null);                                                                                                                                 //hide previous overlay
            globalVars.overlaysOnMap[ghostIndex] = null;                                                                                                                                       //delete previous overlay values
            globalVars.overlaysOnMap[ghostIndex] = new CustomOverlay(ghostIndex, globalVars.ghostOverlayRectangle[ghostIndex].getBounds(), globalVars.incomingOverlaySourceURL[ghostIndex], map, globalVars.preservedRotation); //redraw the overlay within the new bounds
            globalVars.overlaysOnMap[ghostIndex].setMap(map);                                                                                                                                  //set the overlay with new bounds to the map
            globalVars.currentlyEditing = "yes";                                                                                                                                               //enable editing marker
            cacheSaveOverlay(ghostIndex);                                                                                                                                           //trigger a cache of current working overlay
        }
    });

    //set listener for right click (fixes reset issue over overlays)
    google.maps.event.addListener(globalVars.ghostOverlayRectangle[ghostIndex], 'rightclick', function () {
        drawingManager.setDrawingMode(null); //reset drawing manager no matter what
    });

}

//Stores the overlays to save and their associated data
function cacheSaveOverlay(index) {
    de("caching save overlay");
    de("current save overlay index: " + globalVars.csoi);
    //is this the first save
    globalVars.firstSaveOverlay = true;
    de("firstSaveOvelay: " + globalVars.firstSaveOverlay);
    //set overlay index to save
    globalVars.savingOverlayIndex[globalVars.csoi] = globalVars.workingOverlayIndex;
    de("globalVars.savingOverlayIndex[globalVars.csoi]:" + globalVars.savingOverlayIndex[globalVars.csoi]);
    //set label to save
    globalVars.savingOverlayLabel[globalVars.csoi] = globalVars.incomingOverlayLabel[globalVars.workingOverlayIndex];
    de("globalVars.savingOverlayLabel[globalVars.csoi]:" + globalVars.savingOverlayLabel[globalVars.csoi]);
    //set source url to save
    globalVars.savingOverlaySourceURL[globalVars.csoi] = globalVars.incomingOverlaySourceURL[globalVars.workingOverlayIndex];
    de("globalVars.savingOverlaySourceURL[globalVars.csoi]:" + globalVars.savingOverlaySourceURL[globalVars.csoi]);
    //set bounds to save
    globalVars.savingOverlayBounds[globalVars.csoi] = globalVars.ghostOverlayRectangle[globalVars.workingOverlayIndex].getBounds();
    de("globalVars.savingOverlayBounds[globalVars.csoi]:" + globalVars.savingOverlayBounds[globalVars.csoi]);
    //set rotation to save
    globalVars.savingOverlayRotation[globalVars.csoi] = globalVars.preservedRotation;
    de("globalVars.savingOverlayRotation[globalVars.csoi]:" + globalVars.savingOverlayRotation[globalVars.csoi]);
    //check to see if we just recached or if it was a unique cache
    if (globalVars.savingOverlayIndex[globalVars.csoi] != index) {
        //iterate the current save overlay index   
        globalVars.csoi++;
    }
    de("save overlay cached");
}

//Starts the creation of a custom overlay div which contains a rectangular image.
//Supporting URL: https://developers.google.com/maps/documentation/javascript/overlays#CustomOverlays
function CustomOverlay(id, bounds, image, map, rotation) {
    globalVars.overlayCount++;                 //iterate how many overlays have been drawn
    this.bounds_ = bounds;          //set the bounds
    this.image_ = image;            //set source url
    this.map_ = map;                //set to map
    globalVars.preservedRotation = rotation;   //set the rotation
    this.div_ = null;               //defines a property to hold the image's div. We'll actually create this div upon receipt of the onAdd() method so we'll leave it null for now.
    this.index_ = id;               //set the index/id of this overlay
}

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
    div.style.opacity = globalVars.preservedOpacity;

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
    if (globalVars.preservedRotation != 0) {
        keepRotate(globalVars.preservedRotation);
    }

};

//Not currently used
//Supporting URL: https://developers.google.com/maps/documentation/javascript/overlays#CustomOverlays
CustomOverlay.prototype.onRemove = function () {
    this.div_.parentNode.removeChild(this.div_);
    this.div_ = null;
};

//start this whole mess once the google map is loaded
//google.maps.event.addDomListener(window, 'load', initialize);


//start the whole thing

initDeclarations(); //init global vars

//todo make this dynamic and read from the collection
setupInterface("stAugustine"); //defines interface

//#region Define google map objects

google.maps.visualRefresh = true; //Enable the visual refresh (new gmaps)

//set the google map instance options
//supporting url: https://developers.google.com/maps/documentation/javascript/controls
var gmapPageDivId = "googleMap"; //get page div where google maps is to reside
var gmapOptions = {
    disableDefaultUI: false,                                    //set to false to start from a clean slate of map controls
    zoom: globalVars.defaultZoomLevel,                                     //starting zoom level
    minZoom: globalVars.maxZoomLevel,                                      //highest zoom out level
    center: globalVars.mapCenter,                                          //default center point
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