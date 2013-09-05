//other 

//#region jquery UI elements

//jquery UI elements
$(function () {
    $('ul.sf-menu').superfish();

    //draggable content settings
    $("#mapper_container_toolbox").draggable({
        handle: "#mapper_container_toolboxMinibar"//, //div used as handle
        //containment: "#mapper_container" //bind to map container 
    });
    //accordian settings
    $("#mapper_container_toolboxTabs").accordion({
        //animate: false, //turn off all animations (this got rid of search icon problem) //if ever set, check WORKAROUND in openToolboxTabs(id)
        active: 0, //which tab is active
        icons: false, //default icons?
        heightStyle: "content" //set hieght to?
    });
    //tooltips (the tooltip text is the title of the element defined in localization js)
    $("#content_toolbarGrabber").tooltip({ track: true });
    //$("#content_toolbar_button_reset").tooltip({ track: true });
    $("#content_toolbar_button_toggleMapControls").tooltip({track:true});
    $("#content_toolbar_button_toggleToolbox").tooltip({track:true});
    $("#content_toolbar_button_layerRoadmap").tooltip({track:true});
    $("#content_toolbar_button_layerSatellite").tooltip({track:true});
    $("#content_toolbar_button_layerTerrain").tooltip({track:true});
    $("#content_toolbar_button_layerControls").tooltip({track:true});
    $("#content_toolbar_button_layerHybrid").tooltip({track:true});
    $("#content_toolbar_button_layerCustom").tooltip({track:true});
    $("#content_toolbar_button_layerReset").tooltip({track:true});
    $("#content_toolbar_button_zoomIn").tooltip({track:true});
    $("#content_toolbar_button_zoomReset").tooltip({track:true});
    $("#content_toolbar_button_zoomOut").tooltip({track:true});
    $("#content_toolbar_button_panUp").tooltip({track:true});
    $("#content_toolbar_button_panDown").tooltip({track:true});
    $("#content_toolbar_button_panReset").tooltip({track:true});
    $("#content_toolbar_button_panLeft").tooltip({track:true});
    $("#content_toolbar_button_panRight").tooltip({track:true});
    $("#content_toolbar_button_manageItem").tooltip({track:true});
    $("#content_toolbar_button_manageOverlay").tooltip({track:true});
    $("#content_toolbar_button_managePOI").tooltip({ track: true });
    $("#content_toolbox_button_reset").tooltip({track:true});
    $("#content_toolbox_button_toggleMapControls").tooltip({track:true});
    $("#content_toolbox_button_layerRoadmap").tooltip({track:true});
    $("#content_toolbox_button_layerSatellite").tooltip({track:true});
    $("#content_toolbox_button_layerTerrain").tooltip({track:true});
    $("#content_toolbox_button_layerControls").tooltip({track:true});
    $("#content_toolbox_button_layerHybrid").tooltip({track:true});
    $("#content_toolbox_button_layerCustom").tooltip({track:true});
    $("#content_toolbox_button_layerReset").tooltip({track:true});
    $("#content_toolbox_button_zoomIn").tooltip({track:true});
    $("#content_toolbox_button_zoomReset").tooltip({track:true});
    $("#content_toolbox_button_zoomOut").tooltip({track:true});
    $("#content_toolbox_button_panUp").tooltip({track:true});
    $("#content_toolbox_button_panDown").tooltip({track:true});
    $("#content_toolbox_button_panReset").tooltip({track:true});
    $("#content_toolbox_button_panLeft").tooltip({track:true});
    $("#content_toolbox_button_panRight").tooltip({track:true});
    $("#content_toolbox_button_manageItem").tooltip({track:true});
    $("#content_toolbox_button_manageOverlay").tooltip({track:true});
    $("#content_toolbox_button_managePOI").tooltip({track:true});
    $("#content_toolbox_button_placeItem").tooltip({track:true});
    $("#content_toolbox_button_overlayPlace").tooltip({track:true});
    $("#content_toolbox_button_placePOI").tooltip({track:true});
    $("#content_toolbox_button_poiMarker").tooltip({track:true});
    $("#content_toolbox_button_poiCircle").tooltip({track:true});
    $("#content_toolbox_button_poiRectangle").tooltip({track:true});
    $("#content_toolbox_button_poiPolygon").tooltip({track:true});
    $("#content_toolbox_button_poiLine").tooltip({track:true});
    $("#rotationKnob").tooltip({track:true});
    $("#content_toolbox_rotationClockwise").tooltip({track:true});
    $("#content_toolbox_rotationReset").tooltip({track:true});
    $("#content_toolbox_rotationCounterClockwise").tooltip({track:true});
    $("#transparency").tooltip({track:true});
    $("#content_toolbox_rgItem").tooltip({track:true});
    $("#content_toolbox_posItem").tooltip({track:true});
    $("#content_toolbox_button_placeItem").tooltip({track:true});
    $("#descItem").tooltip({track:true});
    $("#content_toolbox_button_saveItem").tooltip({track:true});
    $("#content_toolbox_button_overlayPlace").tooltip({track:true});
    $("#content_toolbox_button_saveOverlay").tooltip({track:true});
    $("#content_toolbox_button_placePOI").tooltip({track:true});
    $("#descPOI").tooltip({track:true});
    $("#content_toolbox_button_savePOI").tooltip({track:true});
    $("#content_toolbox_button_itemGetUserLocation").tooltip({track:true});
    $("#content_toolbox_button_overlayGetUserLocation").tooltip({ track: true });
    $("#content_toolbox_button_overlayEdit").tooltip({ track: true });
    $("#content_toolbox_button_overlayToggle").tooltip({ track: true });
    $("#content_toolbox_button_useSearchAsLocation").tooltip({ track: true });
    $("#content_toolbox_button_convertToOverlay").tooltip({ track: true });
    $("#content_toolbox_button_poiGetUserLocation").tooltip({ track: true });
    $("#content_toolbox_button_poiToggle").tooltip({ track: true });
    $("#content_toolbox_button_clearItem").tooltip({track:true});
    $("#content_toolbox_button_clearOverlay").tooltip({track:true});
    $("#content_toolbox_button_clearPOI").tooltip({track:true});
    $("#content_toolbar_searchField").tooltip({track:true});
    $("#content_toolbar_searchButton").tooltip({track:true});
    $("#content_toolbox_searchField").tooltip({track:true});
    $("#content_toolbox_searchButton").tooltip({track:true});
    $("#searchResults_container").tooltip({track:true});
    //$(".selector").tooltip({ content: "Awesome title!" });

});                               

//#endregion

