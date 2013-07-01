function resetAll() {
    document.location.reload(true); //refresh page
}

function toggleVis(id) {
    switch (id) {
        case "mapControls":
            //toggleAllMapControlsTool();
            break;
        case "toolbox":
            //toggletoolbox(4);
            break;
        case "toolbar":
            //
            break;
        case "":
            //
            break;
        case "":
            //
            break;
            
        default:
            //toggle that item if not found above
            break;
    }
}

function changeMapLayer(layer) {
    switch (layer) {
        case "roadmap":
            map.setMapTypeId(google.maps.MapTypeId.ROADMAP);
            break;
        case "terrain":
            map.setMapTypeId(google.maps.MapTypeId.TERRAIN);
            break;
        case "satellite":
            map.setMapTypeId(google.maps.MapTypeId.SATELLITE);
            break;
        case "hybrid":
            map.setMapTypeId(google.maps.MapTypeId.HYBRID);
            break;
        case "custom":
            if (kmlOn == "no") {
                kmlLayer.setMap(map);
                kmlOn = "yes";
            } else {
                kmlLayer.setMap(null);
                kmlOn = "no";
            }
            break;
        case "reset":
            map.setMapTypeId(google.maps.MapTypeId.ROADMAP);
            //2do make this set to default
            break;
    }
}

function panMap(direction) {
    switch (direction) {
        case "up":
            map.panBy(0, -100);
            testBounds();
            break;
        case "down":
            map.panBy(0, 100);
            testBounds();
            break;
        case "left":
            map.panBy(-100, 0);
            testBounds();
            break;
        case "right":
            map.panBy(100, 0);
            testBounds();
            break;
        case "reset":
            map.panTo(mapCenter);
            break;
    }
}

function zoomMap(direction) {
    switch (direction) {
        case "in":
            map.setZoom(map.getZoom() + 1);
            break;
        case "out":
            map.setZoom(map.getZoom() - 1);
            break;
        case "reset":
            map.setZoom(defaultZoomLevel);
            break;
    }
}

function action(id) {
    switch (id) {
        case "manageItem":
            show(toolbox);
            toggletoolbox(2);
            toolboxDisplayed = true;
            placerType = "item";
            break;
        case "manageOverlay":
            show(toolbox);
            toggletoolbox(2);
            toolboxDisplayed = true;
            placerType = "overlay";
            break;
        case "managePOI":
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
            show(toolbox);
            toggletoolbox(2);
            toolboxDisplayed = true;
            placerType = "poi";
            break;
    }
}