//contains global declarations

//Declarations

//#region Declarations

//must remain outside fcns at top level
var globalVar; //holds global vars
var localize; //holds localization stuff

//other global vars
//todo move into init object (it gets dicey)
CustomOverlay.prototype = new google.maps.OverlayView(); //used to display custom overlay
var KmlLayer;               //must be pingable by google
var geocoder;               //must define before use
var label = [];             //used as label of poi
var infoWindow = [];        //poi infowindow

//call declarations init fcn
//todo move into a document onload listener (TEMP)
initDeclarations();

//init declarations
function initDeclarations() {

    //init global object    
    globalVar = function () {
        //private

        //public
        return {
            //public vars

            //init global vars
            //global defines (do not change here)
            toServerSuccess: false,                     //holds a marker indicating if toserver was sucessfull
            tempYo: false,                              //holds tempyo for fixing ff info window issue
            buttonActive_searchResultToggle: false,     //holds is button active markers
            buttonActive_itemPlace: false,              //holds is button active markers
            //buttonActive_overlayEdit: false,            //not currently used
            //buttonActive_overlayPlace: false,           //not currently used
            buttonActive_overlayToggle: false,          //holds is button active markers
            buttonActive_poiPlace: false,               //not currently used
            buttonActive_poiToggle: false,              //holds is button active markers
            buttonActive_poiMarker: false,              //not currently used
            buttonActive_poiCircle: false,              //not currently used
            buttonActive_poiRectangle: false,           //not currently used
            buttonActive_poiPolygon: false,             //not currently used
            buttonActive_poiLine: false,                //not currently used
            userMayLoseData: false,                     //holds a marker to determine if signifigant changes have been made to require a save
            baseURL: null,                              //holds place for server written vars
            defaultOpacity: 0.5,                        //holds default opacity settings
            isConvertedOverlay: false,                  //holds a marker for converted overlay
            RIBMode: false,                             //holds a marker for running in background mode (do not display messages)
            debugMode: null,                            //holds debug marker
            hasCustomMapType: null,                     //holds marker for determining if there is a custom map type
            messageCount: 0,                            //holds the running count of all the messages written in a session
            poiToggleState: "displayed",                //holds marker for displayed/hidden pois
            poiCount: 0,                                //holds a marker for pois drawn (fixes first poi desc issue)
            firstSaveItem: null,                        //holds first save marker (used to determine if saving or applying changes)
            firstSaveOverlay: null,                     //holds first save marker (used to determine if saving or applying changes)
            firstSavePOI: null,                         //holds first save marker (used to determine if saving or applying changes)
            baseImageDirURL: null,                      //holds the base image directory url
            mapDrawingManagerDisplayed: null,           //holds marker for drawing manager
            mapLayerActive: null,                       //holds the current map layer active
            prevMapLayerActive: null,                   //holds the previous active map layer
            actionActive: null,                         //holds the current active action
            prevActionActive: null,                     //holds the previous active action
            overlaysCurrentlyDisplayed: null,           //holds marker for overlays on map
            pageMode: null,                             //holds the page/viewer type
            mapCenter: null,                            //used to center map on load
            mapControlsDisplayed: null,                 //by default, are map controls displayed (true/false)
            defaultDisplayDrawingMangerTool: null,      //by default, is the drawingmanger displayed (true/false)
            toolboxDisplayed: null,                     //by default, is the toolbox displayed (true/false)
            toolbarDisplayed: null,                     //by default, is the toolbar open (yes/no)
            kmlDisplayed: null,                         //by default, is kml layer on (yes/no)
            defaultZoomLevel: null,                     //zoom level, starting
            maxZoomLevel: null,                         //max zoom out, default (21:lowest level, 1:highest level)
            minZoomLevel_Terrain: null,                 //max zoom in, terrain 
            minZoomLevel_Satellite: null,               //max zoom in, sat + hybrid
            minZoomLevel_Roadmap: null,                 //max zoom in, roadmap (default)
            minZoomLevel_BlockLot: null,                //max zoom in, used for special layers not having default of roadmap
            isCustomOverlay: null,                      //used to determine if other overlays (block/lot etc) //used in testbounds unknown if needed
            preservedRotation: null,                    //rotation, default
            knobRotationValue: null,                    //rotation to display by default 
            preservedOpacity: null,                     //opacity, default value (0-1,1:opaque)
            overlaysOnMap: [],                          //holds all overlays
            csoi: 0,                                    //hold current saved overlay index
            pendingOverlaySave: false,                  //hold the marker to indicate if we need to save the overlay (this prevents a save if we already saved)
            oomCount: 0,                                //counts how many overlays are on the map
            searchCount: 0,                             //interates how many searches
            degree: 0,                                  //initializing degree
            firstMarker: 0,                             //used to iterate if marker placement was the first (to prevent duplicates)
            overlayCount: 0,                            //iterater (contains how many overlays are not deleted)
            mapInBounds: null,                          //is the map in bounds
            searchResult: null,                         //will contain object
            circleCenter: null,                         //hold center point of circle
            markerCenter: null,                         //hold center of marker
            placerType: null,                           //type of data (marker,overlay,poi)
            poiType: [],                                //typle of poi (marker, circle, rectangle, polygon, polyline)
            poiKML: [],                                 //pou kml layer (or other geographic info)
            poi_i: -1,                                  //increment the poi count (used to make IDs and such)
            poiObj: [],                                 //poi object placholder
            poiCoord: [],                               //poi coord placeholder
            poiDesc: [],                                //desc poi placeholder
            globalPolyline: null,                       //unknown
            rectangle: null,                            //must define before use
            firstDraw: 0,                               //used to increment first drawing of rectangle
            getCoord: null,                             //used to store coords from marker
            itemMarker: null,                           //hold current item marker
            savingMarkerCenter: null,                   //holds marker coords to save
            CustomOverlay: null,                        //does nothing
            cCoordsFrozen: "no",                        //used to freeze/unfreeze coordinate viewer
            mainCount: 0,                               //hold debug main count
            incomingACL: "item",                        //hold default incoming ACL (determined in displaying points/overlays)
            overlayType: null,                          //draw = overlay was drawn
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
            ghostOverlayRectangle: [],                  //holds ghost overlay rectangles (IE overlay hotspots)
            convertedOverlayIndex: 0,                   //holds the place for indexing a converted overlay
            workingOverlayIndex: null,                  //holds the index of the overlay we are working with (and saving)
            currentlyEditing: "no",                     //tells us if we are editing anything
            currentTopZIndex: 5,                        //current top zindex (used in displaying overlays over overlays)
            savingOverlayIndex: [],                     //holds index of the overlay we are saving
            savingOverlayLabel: [],                     //holds label of the overlay we are saving
            savingOverlaySourceURL: [],                 //hold the source url of the overlay to save
            savingOverlayBounds: [],                    //holds bounds of the overlay we are saving
            savingOverlayRotation: [],                  //holds rotation of the overlay we are saving
            savingOverlayPageId: [],                    //holds page ID of saving overlay
            strictBounds: null,                         //holds the strict bounds
            ghosting: {                                 //define options for globalVar.ghosting (IE being invisible)
                strokeOpacity: 0.0,                     //make border invisible
                fillOpacity: 0.0,                       //make fill transparent
                editable: false,                        //sobek standard
                draggable: false,                       //sobek standard
                clickable: false,                       //sobek standard
                zindex: 5                               //perhaps higher?
            },
            editable: {                                 //define options for visible and globalVar.editable
                editable: true,                         //sobek standard
                draggable: true,                        //sobek standard
                clickable: true,                        //sobek standard
                strokeOpacity: 0.2,                     //sobek standard
                strokeWeight: 1,                        //sobek standard
                fillOpacity: 0.0,                       //sobek standard 
                zindex: 5                               //sobek standard
            }//,
            ////public methods
            //addListeners: function () {
            //    //alert(globalVar.test1);
            //},
            //add: function () {
            //    //globalVar.test2++;
            //    //alert(globalVar.test2);
            //}
        };
    }();
    //get and set c# vars to js  
    initServerToClientVars();
}

//#endregion