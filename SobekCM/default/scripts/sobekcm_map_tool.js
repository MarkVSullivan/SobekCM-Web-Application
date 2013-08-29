
// we'll set up a SobekMap top level object to create our namespace
var SobekCM = { }; 

// This is our class creation function
SobekCM.klass = function() {
    return function() {
        if (this.init) {
            return this.init.apply(this, arguments);
        } else {
            throw 'Cannot have a SobekCM class without an init function.';
        }
    };
};

// Class to extend an object
SobekCM.extend = function(inherited, base) {
    for (var prop in base) {
        inherited[prop] = base[prop];
    }
    return inherited;
};

/// <member name="T:SobekCM.Javascript.Map">
///     <summary> This class draws a Google map and accepts other options, such as toolbox type </summary>
///     <remarks> This is main controller class for the SobekCM implementation of Google maps </remarks>
/// </member>
SobekCM.Map = SobekCM.klass();
SobekCM.Map.prototype = {

    /// <member name="M:SobekCM.Javascript.Map.init(object opt_options)">
    ///     <summary> Initialization routine for a new instance of the Map object </summary>
    ///     <param name="map_div"> Name of the div on the web page to add this google map and other tools to </param>
    ///     <param name="map_options"> List of options to apply to the google map </param>
    ///     <remarks> To be constructed like: var map = new SobekCM.Map( 'map_div_name', options ) </remarks>
    /// </member>
    init: function(map_div, map_options) {

        var t = this;

        this.globals = {
            innermap: null,
            maptool: null,
            areaSelectPoly: null,
            rectangle_marker1: null,
            rectangle_marker2: null,
            pointSelectMarker: null,
            polygonLabelOverlay: null,
            mapKey: null,
            pointer_image: 'http://ufdc.uflib.ufl.edu/default/images/red-pushpin.png'
        };

        // Create the inner google map
        this.globals.innermap = new google.maps.Map(document.getElementById(map_div), map_options);

    },

    /** 
    * Method adds the map tool
    **/
    setMapTool: function(type) {
        var t = this;

        // TODO: If there was already a map tool included, remove it

        // Add the standard control, if that was requested
        if (type == SobekCM_MapTool_Type_Enum.standard) {
            // Create the map tool ( standard by default )
            this.globals.maptool = new SobekCM.MapTool_Standard(this.globals.innermap);
        }

        // Add the advanced control, if that was requested
        if (type == SobekCM_MapTool_Type_Enum.advanced) {
            // Create the map tool ( standard by default )
            this.globals.maptool = new SobekCM.MapTool_Advanced(this.globals.innermap);
        }

        // If a map tool was created, add it and add any events
        if (this.globals.maptool != null) {
            // Add this as a custom control
            this.globals.innermap.controls[google.maps.ControlPosition.TOP_LEFT].push(this.globals.maptool.get_main_button_div());

            // Add interest in the "events" fired when the map tool is manipulated
            this.globals.maptool.OnNewPointSelected = function(point) { t.set_selection_point(point, true, false, 8); };
            this.globals.maptool.OnNewRectangleSelected = function(point1, point2) { t.set_selection_rectangle(point1, point2, true, false); };
            this.globals.maptool.OnSelectionStarted = function() { t.clear_selection(); t.OnNewSelectionStarted(); };
        }
    },

    // Disable point searching
    disable_point_search: function() {

        if (this.globals.maptool != null) {
            this.globals.maptool.disable_point_search();
        }
    },


    // Add the customized map key
    addMapKey: function(search_type, areas_shown, points_shown, area_name) {

        // Create the map key
        this.globals.mapKey = new SobekCM.Map_Key(search_type, areas_shown, points_shown, area_name);

        // Add this key to the map
        this.globals.innermap.controls[google.maps.ControlPosition.BOTTOM_LEFT].push(this.globals.mapKey.get_main_div());
    },

    /**
    * Method called to add a selected area as if the user had selected it
    */
    clear_selection: function() {
        var G = this.globals;

        // Remove any rectangular area selected
        if (G.areaSelectPoly) {
            G.areaSelectPoly.setMap(null);
            G.areaSelectPoly = null;
        }

        // Remove any rectangular markers selected
        if (G.rectangle_marker1) {
            G.rectangle_marker1.setMap(null);
            G.rectangle_marker1 = null;
        }

        // Remove any rectangular markers selected
        if (G.rectangle_marker2) {
            G.rectangle_marker2.setMap(null);
            G.rectangle_marker2 = null;
        }

        // Remove any existing points selected
        if (G.pointSelectMarker) {
            G.pointSelectMarker.setMap(null);
            G.pointSelectMarker = null;
        }
    },

    /**
    * Method called to add a selected area as if the user had selected it
    */
    set_selection_rectangle: function(point1, point2, fire_event, center_on_rectangle) {
        var G = this.globals;
        var t = this;

        // Remove any selections if they still exist
        this.clear_selection();

        // Adds the polygon to the inner map
        var latLngBounds = new google.maps.LatLngBounds(point1, point2);
        G.areaSelectPoly = new google.maps.Rectangle({ strokeColor: "#ff0000", strokeOpacity: 1, strokeWeight: 3, fillColor: "#ff0000", fillOpacity: 0.2 });
        G.areaSelectPoly.setBounds(latLngBounds);
        G.areaSelectPoly.setMap(G.innermap);

        // Also, add markers to the inner map
        G.rectangle_marker1 = new google.maps.Marker({ position: point1, draggable: true, map: G.innermap, icon: G.pointer_image });
        G.rectangle_marker2 = new google.maps.Marker({ position: point2, draggable: true, map: G.innermap, icon: G.pointer_image });

        // When a drag completes, just set the rectangular area with this function again
        google.maps.event.addListener(G.rectangle_marker1, 'dragend', function() {
            var drag_point = G.rectangle_marker1.getPosition();
            var existing_point = G.rectangle_marker2.getPosition();
            t.set_selection_rectangle(drag_point, existing_point, fire_event, center_on_rectangle);
        });
        google.maps.event.addListener(G.rectangle_marker2, 'dragend', function() {
            var drag_point = G.rectangle_marker2.getPosition();
            var existing_point = G.rectangle_marker1.getPosition();
            t.set_selection_rectangle(existing_point, drag_point, fire_event, center_on_rectangle);
        });

        // During a drag, just do a simple redraw of the polygon
        google.maps.event.addListener(G.rectangle_marker1, 'drag', function() {
            var drag_point = G.rectangle_marker1.getPosition();
            var existing_point = G.rectangle_marker2.getPosition();
            t.redraw_rectangle(drag_point, existing_point);
        });
        google.maps.event.addListener(G.rectangle_marker2, 'drag', function() {
            var drag_point = G.rectangle_marker2.getPosition();
            var existing_point = G.rectangle_marker1.getPosition();
            t.redraw_rectangle(existing_point, drag_point);
        });

        // Center the map if that was requested
        if (center_on_rectangle) {
            // Set the center and zoom based on the polygon
            G.innermap.setCenter(latLngBounds.getCenter());
            G.innermap.fitBounds(latLngBounds);
        }

        // Fire the "event" to let any listener know about this
        if (fire_event) {
            var points_latlng = new Array();
            points_latlng.push(point1.lat());
            points_latlng.push(point1.lng());
            points_latlng.push(point2.lat());
            points_latlng.push(point2.lng());
            this.OnNewSelection(points_latlng);
        }
    },

    /**
    * Method simply redraws the rectangle without firing any events 
    **/
    redraw_rectangle: function(point1, point2) {
        var G = this.globals;

        if (G.areaSelectPoly) {
            var latLngBounds = new google.maps.LatLngBounds(point1, point2);
            G.areaSelectPoly.setBounds(latLngBounds);
        }
    },

    /**
    * Method called to add a selected point as if the user had selected it
    */
    set_selection_point: function(point, fire_event, center_on_point, zoom_level) {
        var G = this.globals;
        var t = this;

        // Remove any selections if they still exist
        this.clear_selection();

        // If there is a map tool, then set draggable to true
        var allowDrag = false;
        if (G.maptool != null)
            allowDrag = true;

        // Add a marker to this map
        G.pointSelectMarker = new google.maps.Marker({ position: point, draggable: allowDrag, map: G.innermap, icon: G.pointer_image });
        google.maps.event.addListener(G.pointSelectMarker, 'dragend', function() {
            var drag_point = G.pointSelectMarker.getPosition();
            var drag_points_latlng = new Array();
            drag_points_latlng.push(drag_point.lat());
            drag_points_latlng.push(drag_point.lng());
            if (t.OnNewSelection != null) {
                t.OnNewSelection(drag_points_latlng);
            }
        });

        // Center the map if that was requested
        if ((center_on_point) && (zoom_level >= 1)) {
            G.innermap.setCenter(point);
            G.innermap.setZoom(zoom_level);
        }

        // Fire the "event" to let any listener know about this
        if (fire_event) {
            var points_latlng = new Array();
            points_latlng.push(point.lat());
            points_latlng.push(point.lng());
            if (this.OnNewSelection != null) {
                this.OnNewSelection(points_latlng);
            }
        }
    },

    /**
    * Method called to initiate an area select as if the user had clicked the area button.
    */
    initiateAreaSelect: function() { if (this.globals.maptool) { this.globals.maptool.initiateAreaSelect(); } },

    /**
    * Method called to initiate a point select as if the user had clicked the point button.
    */
    initiatePointSelect: function() { if (this.globals.maptool) { this.globals.maptool.initiatePointSelect(); } },
  
    // Add a non-selection (simple) point marker to this map
    add_point: function (point, label) {
      var newPoint = new google.maps.Marker({ position: point, draggable: false, map: this.globals.innermap });
      
      //create a center point from incoming point
      var searchString = String(point);
      searchString = searchString.replace("(","");
      searchString = searchString.replace(")","");
      var commaPos = searchString.indexOf(',');
      var coordinatesLat = parseFloat(searchString.substring(0, commaPos));
      var coordinatesLong = parseFloat(searchString.substring(commaPos + 1, searchString.length));
      var center2 = new google.maps.LatLng(coordinatesLat, coordinatesLong);
      
      //center on the point
      this.globals.innermap.setCenter(center2);
      
      //create and set zoom level
      var bounds2 = new google.maps.LatLngBounds();
      bounds2.extend(center2);
      this.globals.innermap.fitBounds(bounds2);

    },

    // Adds a non-selection (simple) polygon to this map
    add_polygon: function(points, fillcolor, strokecolor, strokeweight, label, link) {
        var t = this;

        // Determine the stroke opacity
        var strokeopacity = 0.8;
        if (strokeweight == 0)
            strokeopacity = 0.2;

        // Declare the new polygon and set the map
        var polygon = new google.maps.Polygon({
            paths: points,
            strokeColor: strokecolor,
            strokeOpacity: strokeopacity,
            strokeWeight: strokeweight,
            fillColor: fillcolor,
            fillOpacity: 0.2,
            title: label
        });
        polygon.setMap(this.globals.innermap);

        // If there was a link, add that event listener
        if (link.length > 0) {
            google.maps.event.addListener(polygon, 'click', function() {
                window.location.href = link;
            });
        }

        // If there was a label, add an event to tell the polygon label overlay the label
        if (label.length > 0) {

            // Ensure the polygon label overlay has been added
            if (this.globals.polygonLabelOverlay == null) {
                this.globals.polygonLabelOverlay = new SobekCM.Polygon_Label_Overlay(this.globals.innermap);
                google.maps.event.addListener(this.globals.innermap, 'mousemove', function(mEvent) { t.globals.polygonLabelOverlay.setLabel(''); });
            }

            // Add the mouse move event to this polygon
            google.maps.event.addListener(polygon, 'mousemove', function() {
                t.globals.polygonLabelOverlay.setLabel(label);
                polygon.setOptions({ strokeWeight: 10, strokeOpacity: 1.0 });
            });

            // Add the mouse out event to this polygon
            google.maps.event.addListener(polygon, 'mouseout', function() {
                t.globals.polygonLabelOverlay.setLabel('');
                polygon.setOptions({ strokeWeight: strokeweight, strokeOpacity: strokeopacity });
            });
        }

    },

    // Add a rectangle
    add_rectangle: function(lat1, long1, lat2, long2, color) {
        // Pick the corners
        var c1 = new google.maps.LatLng(lat1, long1);
        var c2 = new google.maps.LatLng(lat2, long2);

        // Define the rectangle
        var latLngBounds = new google.maps.LatLngBounds(c1, c2);
        var rectangle = new google.maps.Rectangle({ strokeColor: color, strokeOpacity: 1, strokeWeight: 3, fillColor: color, fillOpacity: 0.0 });
        rectangle.setBounds(latLngBounds);
        rectangle.setMap(this.globals.innermap);
        
        //center on the first lat/long
        this.globals.innermap.setCenter(new google.maps.LatLng(lat1, long1));

    },



    // Zooms the map to a provided bounds
    zoom_to_bounds: function(bounds) {
        if (!bounds.isEmpty()) {
          // Set the center based on the bounds
          this.globals.innermap.setCenter(bounds.getCenter());

          // If this has only a single point, set the zoom to 8, otherwise, allow the bounds to set it
          var ne = bounds.getNorthEast();
          var sw = bounds.getSouthWest();
          if ((ne.lat() == sw.lat()) && (ne.lng() == sw.lng())) {
            this.globals.innermap.setZoom(8);
          } else {
            this.globals.innermap.fitBounds(bounds);
          }
        }
    }
}

/* Overlay displays the labels for the polygons */
SobekCM.Polygon_Label_Overlay = SobekCM.klass();
SobekCM.extend(SobekCM.Polygon_Label_Overlay.prototype, google.maps.OverlayView.prototype);

/* Add additional methods */
SobekCM.extend(SobekCM.Polygon_Label_Overlay.prototype, {
    init: function(map) {
    
        this.globals = {
            anchorOffset: new google.maps.Point(12, -4),
            htmlNode: null,
            labelString: ''
        };
        
         /**
        * Pointer to the HTML container.
        */
        this.globals.htmlNode = this.createHtmlNode_();

        // Add control to the map. Position is irrelevant.
        map.controls[google.maps.ControlPosition.TOP].push(this.globals.htmlNode );

        // Bind this OverlayView to the map so we can access MapCanvasProjection
        // to convert LatLng to Point coordinates.
        this.setMap(map);

        // Register event listeners
        var t = this;
        google.maps.event.addListener(map, 'mouseover', function(mEvent) { t.set('visible', true); });
        google.maps.event.addListener(map, 'mouseout', function(mEvent) { t.set('visible', false); });
        google.maps.event.addListener(map, 'mousemove', function(mEvent) { t.updatePosition(mEvent.latLng); });

        // Register an MVC property to indicate whether this custom control
        // is visible or hidden. Initially hide control until mouse is over map.
        this.set('visible', false);
    },
    
    setLabel : function( newLabel )
    {
        this.globals.labelString = newLabel;
        //if ( newLabel.length == 0 )
        //    this.globals.htmlNode.style.display = 'none';
        //else
        //    this.globals.htmlNode.style.display = 'block';
    },

    /**
    * @private
    * Helper function creates the HTML node which is the control container.
    * @return {HTMLDivElement}
    */
    createHtmlNode_: function() {
        var divNode = document.createElement('div');
        divNode.style.cssText = 'background: #ffc; border: 1px solid #676767; font-family: arial, helvetica, sans-serif; font-size: 0.7em; padding: 2px 4px; position: absolute;';
        divNode.index = 100;
        return divNode;
    },

    /**
    * MVC property's state change handler function to show/hide the
    * control container.
    */
    visible_changed: function() {
        this.globals.htmlNode.style.display = this.get('visible') ? '' : 'none';
    },

    /**
    * Specified LatLng value is used to calculate pixel coordinates and
    * update the control display. Container is also repositioned.
    * @param {google.maps.LatLng} latLng Position to display
    */
    updatePosition: function(latLng) {
        var projection = this.getProjection();
        var point = projection.fromLatLngToContainerPixel(latLng);

        // Update control position to be anchored next to mouse position.
        this.globals.htmlNode .style.left = point.x + this.globals.anchorOffset.x + 'px';
        this.globals.htmlNode .style.top = point.y + this.globals.anchorOffset.y + 'px';

        // Update control to display latlng and coordinates.
        this.globals.htmlNode.innerHTML = this.globals.labelString;
    },

    // Implement on add
    onAdd: function() {
        // do nothing
    },

    // Implement onRemove
    onRemove: function() {
        // do nothing
    },

    // Implement draw
    draw: function() {
        // do nothing
    }
});

/* here we begin the map key class */
SobekCM.Map_Key = SobekCM.klass();
SobekCM.Map_Key.prototype = {
    /* to be constructed like: 
    var mapKey = new SobekCM.Map_Key(); */
    init: function(search_type, areas_shown, points_shown, area_name) {
        var t = this;

        // Define some global values which will be used throughout this tool
        this.globals = {
            containerDiv: null
        };

        // Add the button container here instead
        this.globals.containerDiv = document.createElement("div");
        this.globals.containerDiv.setAttribute('id', 'MapKeyDiv');
        this.globals.containerDiv.style.cssText = 'border: solid 1px Gray; font-family:Arial, Helvetica, sans-serif; margin-bottom: 10px; background-color:white;';
        this.globals.containerDiv.style.width = '115px';
        this.globals.containerDiv.style.zIndex = '1000';

        // Compute the inner text
        var innerBuilder = '<table width="100%" style="font-size:x-small; text-align:left" >';
        if (search_type == SobekCM_Search_Type_Enum.point)
            innerBuilder = innerBuilder + '<tr><td width="25px"><img src="http://ufdc.uflib.ufl.edu/default/images/legend_red_pushpin.png" /></td><td>Search Point</td></tr>';
        if (search_type == SobekCM_Search_Type_Enum.rectangle)
            innerBuilder = innerBuilder + '<tr><td width="25px"><img src="http://ufdc.uflib.ufl.edu/default/images/legend_search_area.png" /></td><td>Search Area</td></tr>';
        if (areas_shown)
            innerBuilder = innerBuilder + '<tr><td width="25px"><img src="http://ufdc.uflib.ufl.edu/default/images/legend_selected_polygon.png" /></td><td>Matching ' + area_name + '</td></tr><tr><td><img src="http://ufdc.uflib.ufl.edu/default/images/legend_nonselected_polygon.png" /></td><td>Other ' + area_name + '</td></tr>';
        if (points_shown)
            innerBuilder = innerBuilder + '<tr><td width="25px"><img src="http://ufdc.uflib.ufl.edu/default/images/legend_point_interest.png" /></td><td>Point of Interest</td></tr>';
        this.globals.containerDiv.innerHTML = innerBuilder + '</table>';
    },

    /** 
    * Function returns the main button division to be added to the google map as a control
    */
    get_main_div: function() { return this.globals.containerDiv; }
}

/* here we begin the map tool class */
SobekCM.MapTool_base = SobekCM.klass();
SobekCM.MapTool_base.prototype = {
    /* to be constructed like: 
    var sobekMapTool = new SobekCM.MapTool(map); */
    init: function(map) {
        this.map = map;
        this.originalCenter = map.getCenter();
        var t = this;

        // Define some global values which will be used throughout this tool
        this.globals = {
            map: map,
            currentStatus: SobekCM_Status_Enum.standard,
            rectangleAreaFirstPoint: null,
            rectangleTemporary: null,
            buttonContainerDiv: null,
            areaSelectColor: "#ff0000"
        };

        // Add the button container here instead
        this.globals.buttonContainerDiv = document.createElement("div");
        this.globals.buttonContainerDiv.setAttribute('id', 'MapSelectButtons');
        this.globals.buttonContainerDiv.style.cssText = 'padding: 10px 0px 0px 0px';
        this.globals.buttonContainerDiv.style.width = '400px';
        this.globals.buttonContainerDiv.style.zIndex = '1000';

        // Initialize the "extending class"
        this.initialize_tool();

        // Add the click event used for point (and later polygon) definitions
        google.maps.event.addListener(map, "click", function(point) { t.mapClick_(point); });
        google.maps.event.addListener(map, "mousemove", function(point) { t.drag_(point); });
    },

    /** 
    * Function returns the main button division to be added to the google map as a control
    */
    get_main_button_div: function() { return this.globals.buttonContainerDiv; },

    /**
    * Function called when the standard (default) button's click event is captured.
    */
    standardButtonClick_: function() {
        // Reset the button mode
        this._setButtonMode(SobekCM_Status_Enum.standard);
    },

    /**
    * Function called when the rectangle area selection button's click event is captured.
    */
    areaButtonClick_: function() {
        // If currently selecting a rectangle, reset everything
        if (this.globals.currentStatus == SobekCM_Status_Enum.rectangle) {
            // Reset the button mode
            this._setButtonMode(SobekCM_Status_Enum.standard);
        }
        else {
            // Set the button mode correctly
            this._setButtonMode(SobekCM_Status_Enum.rectangle);

            // Fire event that a selection has started
            this.OnSelectionStarted();
        }
    },

    /**
    * Function called when the point selection button's click event is captured.
    */
    pointButtonClick_: function() {
        // If currently selecting a point, reset everything
        if (this.globals.currentStatus == SobekCM_Status_Enum.point) {
            // Reset the button mode
            this._setButtonMode(SobekCM_Status_Enum.standard);
        }
        else {
            // Set the button mode correctly
            this._setButtonMode(SobekCM_Status_Enum.point);

            // Fire event that a selection has started
            this.OnSelectionStarted();
        }
    },

    /**
    * Function called when the user clicks on the map
    */
    mapClick_: function(point) {
        if (point != null) {
            // Was this a point selection?
            if (this.globals.currentStatus == SobekCM_Status_Enum.point) {
                // Fire the "event" that a new point was selected
                this.OnNewPointSelected(point.latLng);

                // Reset the current status to standard functionality
                this._setButtonMode(SobekCM_Status_Enum.standard);
            }

            // Was this an area selection?
            if (this.globals.currentStatus == SobekCM_Status_Enum.rectangle) {
                // Has the start of the rectangle been set yet?
                if (!this.globals.rectangleAreaFirstPoint) {
                    // set the first point
                    this.globals.rectangleAreaFirstPoint = point;
                    this._intermediary_point_selected();
                }
                else {
                    // Clear the temporary selection rectangle
                    this.globals.rectangleTemporary.setMap(null);
                    this.globals.rectangleTemporary = null;

                    // Fire the "event" that a new rectangle was selected
                    this.OnNewRectangleSelected(this.globals.rectangleAreaFirstPoint.latLng, point.latLng);

                    // Reset the current status to standard functionality
                    this.globals.rectangleAreaFirstPoint = null;
                    this._setButtonMode(SobekCM_Status_Enum.standard);
                }
            }
        }
    },

    /**
    * Function called when the user moves the mouse on the map
    */
    drag_: function(point) {
        if (point != null) {
            var myG = this.globals;

            // Was this an area selection?
            if (this.globals.currentStatus == SobekCM_Status_Enum.rectangle) {
                // Has the start of the rectangle been set yet?
                if (this.globals.rectangleAreaFirstPoint != null) {
                    // Is the rectangle already there?
                    if (myG.rectangleTemporary != null) {
                        var latLngBounds2 = new google.maps.LatLngBounds(this.globals.rectangleAreaFirstPoint.latLng, point.latLng);
                        myG.rectangleTemporary.setBounds(latLngBounds2);
                    }
                    else {
                        // Adds the rectangle to the inner map
                        var latLngBounds = new google.maps.LatLngBounds(this.globals.rectangleAreaFirstPoint.latLng, point.latLng);
                        myG.rectangleTemporary = new google.maps.Rectangle({ strokeColor: this.globals.areaSelectColor, strokeOpacity: 1, strokeWeight: 3, fillColor: this.globals.areaSelectColor, fillOpacity: 0.2 });
                        myG.rectangleTemporary.setBounds(latLngBounds);
                        myG.rectangleTemporary.setMap(this.globals.map);

                        // Since rectangles catch mousedown events, add it to the rectangle as well
                        var t = this;
                        google.maps.event.addDomListener(myG.rectangleTemporary, "mousedown", function(e) { t.mapClick_(e); });

                    }
                }
            }
        }
    }
}

/* The standard map tool extends the base map tool class */
SobekCM.MapTool_Standard = SobekCM.klass();
SobekCM.extend(SobekCM.MapTool_Standard.prototype, SobekCM.MapTool_base.prototype);

// Implementing our own custom logic
SobekCM.extend(SobekCM.MapTool_Standard.prototype, {
    initialize_tool: function() {
        var t = this;

        // Set some global values for this tool type
        this.tooltype_globals =
        {
            buttonStartingStyle: { border: '1px solid black', padding: '2px', width: '195px', margin: '2px', zIndex: 1000 },
            buttonStyle: { background: '#FFF' },
            buttonActionStyle: { background: '#FF0', width: '195px' },
            areaButtonHTML: '<b> &nbsp; PRESS TO SELECT AREA</b>',
            areaButtonActionHTML: '<b> &nbsp; SELECT THE FIRST POINT</b>',
            areaButtonActionHTML2: '<b> &nbsp; SELECT THE SECOND POINT</b>',
            pointButtonHTML: '<b> &nbsp; PRESS TO SELECT POINT</b>',
            pointButtonActionHTML: '<b>SELECT A POINT ON THE MAP</b>',
            areaButtonDiv: null,
            pointButtonDiv: null
        };

        // create and initialize both the area button and point button
        this.tooltype_globals.areaButtonDiv = this._initAreaButton(this.globals.buttonContainerDiv);
        this.tooltype_globals.pointButtonDiv = this._initPointButton(this.globals.buttonContainerDiv);

        // add event listeners to the buttons
        google.maps.event.addDomListener(this.tooltype_globals.areaButtonDiv, "click", function(e) { t.areaButtonClick_(e); });
        google.maps.event.addDomListener(this.tooltype_globals.pointButtonDiv, "click", function(e) { t.pointButtonClick_(e); });

    },

    /**
    * Creates a new button to control area selection and appends to the button container div.
    * @param {DOM Node} buttonContainerDiv created in main .initialize code
    */
    _initAreaButton: function(buttonContainerDiv) {
        var G = this.globals;
        var TG = this.tooltype_globals;
        var areaButtonDiv = document.createElement('div');
        areaButtonDiv.innerHTML = TG.areaButtonHTML;
        SobekCM_Utilities.setStyle([areaButtonDiv], { cursor: 'pointer', zIndex: 1000 });
        SobekCM_Utilities.setStyle([areaButtonDiv], TG.buttonStartingStyle);
        SobekCM_Utilities.setStyle([areaButtonDiv], TG.buttonStyle);
        buttonContainerDiv.appendChild(areaButtonDiv);
        return areaButtonDiv;
    },

    /**
    * Creates a new button to control point selection and appends to the button container div.
    * @param {DOM Node} buttonContainerDiv created in main .initialize code
    */
    _initPointButton: function(buttonContainerDiv) {
        var G = this.globals;
        var TG = this.tooltype_globals;
        var pointButtonDiv = document.createElement('div');
        pointButtonDiv.innerHTML = TG.pointButtonHTML;
        SobekCM_Utilities.setStyle([pointButtonDiv], { cursor: 'pointer', zIndex: 1000 });
        SobekCM_Utilities.setStyle([pointButtonDiv], TG.buttonStartingStyle);
        SobekCM_Utilities.setStyle([pointButtonDiv], TG.buttonStyle);
        buttonContainerDiv.appendChild(pointButtonDiv);
        return pointButtonDiv;
    },


    /**
    * Sets button mode to zooming or otherwise, changes CSS & HTML.
    * @param {String} mode Either "area_select", "point_select", or neither
    */
    _setButtonMode: function(newMode) {
        var G = this.globals;
        var TG = this.tooltype_globals;

        // Set the new mode
        G.currentStatus = newMode;
        G.rectangleAreaFirstPoint = null;

        // Set the buttons appropriately
        if (newMode == SobekCM_Status_Enum.rectangle) {
            TG.areaButtonDiv.innerHTML = TG.areaButtonActionHTML;
            TG.pointButtonDiv.innerHTML = TG.pointButtonHTML;
            SobekCM_Utilities.setStyle([TG.areaButtonDiv], TG.buttonStartingStyle);
            SobekCM_Utilities.setStyle([TG.areaButtonDiv], TG.buttonActionStyle);
            SobekCM_Utilities.setStyle([TG.pointButtonDiv], TG.buttonStartingStyle);
            SobekCM_Utilities.setStyle([TG.pointButtonDiv], TG.buttonStyle);
            return;
        }

        if (newMode == SobekCM_Status_Enum.point) {
            TG.areaButtonDiv.innerHTML = TG.areaButtonHTML;
            TG.pointButtonDiv.innerHTML = TG.pointButtonActionHTML;
            SobekCM_Utilities.setStyle([TG.areaButtonDiv], TG.buttonStartingStyle);
            SobekCM_Utilities.setStyle([TG.areaButtonDiv], TG.buttonStyle);
            SobekCM_Utilities.setStyle([TG.pointButtonDiv], TG.buttonStartingStyle);
            SobekCM_Utilities.setStyle([TG.pointButtonDiv], TG.buttonActionStyle);
            if (G.rectangleTemporary != null) {
                G.rectangleTemporary.setMap(null);
                G.rectangleTemporary = null;
            }
            return;
        }

        TG.areaButtonDiv.innerHTML = TG.areaButtonHTML;
        TG.pointButtonDiv.innerHTML = TG.pointButtonHTML;
        SobekCM_Utilities.setStyle([TG.areaButtonDiv], TG.buttonStartingStyle);
        SobekCM_Utilities.setStyle([TG.areaButtonDiv], TG.buttonStyle);
        SobekCM_Utilities.setStyle([TG.pointButtonDiv], TG.buttonStartingStyle);
        SobekCM_Utilities.setStyle([TG.pointButtonDiv], TG.buttonStyle);
        if (G.rectangleTemporary != null) {
            G.rectangleTemporary.setMap(null);
            G.rectangleTemporary = null;
        }
    },
    
    _intermediary_point_selected: function() {
        var G = this.globals;
        var TG = this.tooltype_globals;
        if ( G.currentStatus == SobekCM_Status_Enum.rectangle )
        {
            TG.areaButtonDiv.innerHTML = TG.areaButtonActionHTML2;
        }
    },

    // Disable point searching
    disable_point_search: function() {
        this.tooltype_globals.pointButtonDiv.style.display = 'none';
    }
});

/* The advanced map tool extends the base map tool class */
SobekCM.MapTool_Advanced = SobekCM.klass();
SobekCM.extend(SobekCM.MapTool_Advanced.prototype, SobekCM.MapTool_base.prototype);

// Implementing our own custom logic
SobekCM.extend(SobekCM.MapTool_Advanced.prototype, {
    initialize_tool: function() {
        var t = this;
        
        // Set some global values for this tool type
        this.tooltype_globals =
        {
            buttonStartingStyle: { border: '1px solid #888', width: '32px', height: '32px', zIndex: 1000 },
            buttonStyle: { background: '#FFF' },
            buttonActionStyle: { background: '#FF0' },
            standardButtonImage: 'default/images/map_drag_hand.gif',
            rectangleButtonImage: 'default/images/map_rectangle2.gif',
            polygonButtonImage: 'default/images/map_polygon2.gif',
            pointButtonImage: 'default/images/map_point.gif',
            standardButtonDiv: null,
            pointButtonDiv: null,
            rectangleButtonDiv: null,
            polygonButtonDiv: null            
         };   
    
    
        // create and initialize both the area button and point button
        this.tooltype_globals.standardButtonDiv = this._initStandardButton(this.globals.buttonContainerDiv);
        this.tooltype_globals.pointButtonDiv = this._initPointButton(this.globals.buttonContainerDiv);
        this.tooltype_globals.rectangleButtonDiv = this._initRectangleButton(this.globals.buttonContainerDiv);
        this.tooltype_globals.polygonButtonDiv = this._initPolygonButton(this.globals.buttonContainerDiv);

        // add event listeners to the buttons
        google.maps.event.addDomListener(this.tooltype_globals.standardButtonDiv, "click", function(e) { t.standardButtonClick_(e); });
        google.maps.event.addDomListener(this.tooltype_globals.rectangleButtonDiv, "click", function(e) { t.areaButtonClick_(e); });
        google.maps.event.addDomListener(this.tooltype_globals.pointButtonDiv, "click", function(e) { t.pointButtonClick_(e); });
        
    },
    
    
    /**
    * Creates a new button for the standard action in the google maps (sort of a default)
    * @param {DOM Node} buttonContainerDiv created in main .initialize code
    */
    _initStandardButton: function(buttonContainerDiv) {
        var G = this.globals;
        var TG = this.tooltype_globals;
        var standardButtonDiv = document.createElement('div');
        standardButtonDiv.innerHTML = '<img src=' + TG.standardButtonImage + ' title=\"Standard cursor\" />';
        SobekCM_Utilities.setStyle([standardButtonDiv], { cursor: 'pointer', zIndex: 1000 });
        SobekCM_Utilities.setStyle([standardButtonDiv], TG.buttonStartingStyle);
        SobekCM_Utilities.setStyle([standardButtonDiv], TG.buttonActionStyle);
        buttonContainerDiv.appendChild(standardButtonDiv);
        return standardButtonDiv;
    },
    
    /**
    * Creates a new button to control point selection and appends to the button container div.
    * @param {DOM Node} buttonContainerDiv created in main .initialize code
    */
    _initPointButton: function(buttonContainerDiv) {
        var G = this.globals;
        var TG = this.tooltype_globals;
        var pointButtonDiv = document.createElement('div');
        pointButtonDiv.innerHTML = '<img src=' + TG.pointButtonImage + ' title=\"Select Point\" />';
        SobekCM_Utilities.setStyle([pointButtonDiv], { cursor: 'pointer', zIndex: 1000 });
        SobekCM_Utilities.setStyle([pointButtonDiv], TG.buttonStartingStyle);
        SobekCM_Utilities.setStyle([pointButtonDiv], TG.buttonStyle);
        buttonContainerDiv.appendChild(pointButtonDiv);
        return pointButtonDiv;
    },
    
        /**
    * Creates a new button to control rectangular area selection and appends to the button container div.
    * @param {DOM Node} buttonContainerDiv created in main .initialize code
    */
    _initRectangleButton: function(buttonContainerDiv) {
        var G = this.globals;
        var TG = this.tooltype_globals;
        var rectangleButtonDiv = document.createElement('div');
        rectangleButtonDiv.innerHTML = '<img src=' + TG.rectangleButtonImage + ' title=\"Select Rectangle\" />';
        SobekCM_Utilities.setStyle([rectangleButtonDiv], { cursor: 'pointer', zIndex: 1000 });
        SobekCM_Utilities.setStyle([rectangleButtonDiv], TG.buttonStartingStyle);
        SobekCM_Utilities.setStyle([rectangleButtonDiv], TG.buttonStyle);
        buttonContainerDiv.appendChild(rectangleButtonDiv);
        return rectangleButtonDiv;
    },

        /**
    * Creates a new button to control polygon area selection and appends to the button container div.
    * @param {DOM Node} buttonContainerDiv created in main .initialize code
    */
    _initPolygonButton: function(buttonContainerDiv) {
        var G = this.globals;
        var TG = this.tooltype_globals;
        var polygonButtonDiv = document.createElement('div');
        polygonButtonDiv.innerHTML = '<img src=' + TG.polygonButtonImage + ' title=\"Select Polygon\" />';
        SobekCM_Utilities.setStyle([polygonButtonDiv], { cursor: 'pointer', zIndex: 1000 });
        SobekCM_Utilities.setStyle([polygonButtonDiv], TG.buttonStartingStyle);
        SobekCM_Utilities.setStyle([polygonButtonDiv], TG.buttonStyle);
        buttonContainerDiv.appendChild(polygonButtonDiv);
        return polygonButtonDiv;
    },


    /**
    * Sets button mode to zooming or otherwise, changes CSS & HTML.
    * @param {String} mode Either "area_select", "point_select", or neither
    */
    _setButtonMode: function(newMode) {
        var G = this.globals;
        var TG = this.tooltype_globals;

        // Set the new mode
        G.currentStatus = newMode;
        G.rectangleAreaFirstPoint = null;

        // Set the buttons appropriately
        if (newMode == SobekCM_Status_Enum.rectangle) {
            SobekCM_Utilities.setStyle([TG.standardButtonDiv], TG.buttonStartingStyle);
            SobekCM_Utilities.setStyle([TG.standardButtonDiv], TG.buttonStyle);
            SobekCM_Utilities.setStyle([TG.rectangleButtonDiv], TG.buttonStartingStyle);
            SobekCM_Utilities.setStyle([TG.rectangleButtonDiv], TG.buttonActionStyle);
            SobekCM_Utilities.setStyle([TG.pointButtonDiv], TG.buttonStartingStyle);
            SobekCM_Utilities.setStyle([TG.pointButtonDiv], TG.buttonStyle);
            return;
        }

        if (newMode == SobekCM_Status_Enum.point) {
            SobekCM_Utilities.setStyle([TG.standardButtonDiv], TG.buttonStartingStyle);
            SobekCM_Utilities.setStyle([TG.standardButtonDiv], TG.buttonStyle);
            SobekCM_Utilities.setStyle([TG.rectangleButtonDiv], TG.buttonStartingStyle);
            SobekCM_Utilities.setStyle([TG.rectangleButtonDiv], TG.buttonStyle);
            SobekCM_Utilities.setStyle([TG.pointButtonDiv], TG.buttonStartingStyle);
            SobekCM_Utilities.setStyle([TG.pointButtonDiv], TG.buttonActionStyle);
            if ( G.rectangleTemporary != null )
            {
                G.rectangleTemporary.setMap( null );
                G.rectangleTemporary = null;
            }
            return;
        }

        SobekCM_Utilities.setStyle([TG.standardButtonDiv], TG.buttonStartingStyle);
        SobekCM_Utilities.setStyle([TG.standardButtonDiv], TG.buttonActionStyle);
        SobekCM_Utilities.setStyle([TG.rectangleButtonDiv], TG.buttonStartingStyle);
        SobekCM_Utilities.setStyle([TG.rectangleButtonDiv], TG.buttonStyle);
        SobekCM_Utilities.setStyle([TG.pointButtonDiv], TG.buttonStartingStyle);
        SobekCM_Utilities.setStyle([TG.pointButtonDiv], TG.buttonStyle);
        if ( G.rectangleTemporary != null )
        {
            G.rectangleTemporary.setMap( null );
            G.rectangleTemporary = null;
        }
    },
    
    _intermediary_point_selected: function()
    {
        // Do nothing currently
    }
});

/* Enumeration for current status of selection */
var SobekCM_Search_Type_Enum = {
    none: 0,
    point: 1,
    rectangle: 2,
    polygon: 3
};

/* Enumeration for current status of selection */
var SobekCM_Status_Enum =  {
    standard: 0,
    point: 1,
    rectangle: 2,
    polygon: 3
};


/* Enumeration for the current map tool type */
var SobekCM_MapTool_Type_Enum =  {
    none: 0,
    standard: 1,
    advanced: 2
};

/* utility functions in SobekCM_Utilities namespace */
var SobekCM_Utilities = {};

/**
* A general-purpose function to get the absolute position
* of the mouse.
* @param {Object} e  Mouse event
* @return {Object} Describes position
*/
SobekCM_Utilities.getMousePosition = function(e) {
    var posX = 0;
    var posY = 0;
    if (!e) var e = window.event;
    if (e.pageX || e.pageY) {
        posX = e.pageX;
        posY = e.pageY;
    } else if (e.clientX || e.clientY) {
        posX = e.clientX +
      (document.documentElement.scrollLeft ? document.documentElement.scrollLeft : document.body.scrollLeft);
        posY = e.clientY +
      (document.documentElement.scrollTop ? document.documentElement.scrollTop : document.body.scrollTop);
    }
    return { left: posX, top: posY };
};


/**
* Applies styles to DOM objects 
* @param {String/Object} elements Either comma-delimited list of ids 
*   or an array of DOM objects
* @param {Object} styles Hash of styles to be applied
*/
SobekCM_Utilities.setStyle = function(elements, styles) {
    if (typeof (elements) == 'string') {
        elements = SobekCM_Utilities.getManyElements(elements);
    }
    for (var i = 0; i < elements.length; i++) {
        for (var s in styles) {
            elements[i].style[s] = styles[s];
        }
    }
};

/**
* Gets DOM elements array according to list of IDs
* @param {String} elementsString Comma-delimited list of IDs
* @return {Array} Array of DOM elements corresponding to s
*/
SobekCM_Utilities.getManyElements = function(idsString) {
    var idsArray = idsString.split(',');
    var elements = [];
    for (var i = 0; i < idsArray.length; i++) {
        elements[elements.length] = SobekCM_Utilities.gE(idsArray[i])
    };
    return elements;
};

/**
* Gets position of element
* @param {Object} element
* @return {Object} Describes position
*/
SobekCM_Utilities.getElementPosition = function(element) {
    var leftPos = element.offsetLeft;          // initialize var to store calculations
    var topPos = element.offsetTop;            // initialize var to store calculations
    var parElement = element.offsetParent;     // identify first offset parent element  
    while (parElement != null) {                // move up through element hierarchy
        leftPos += parElement.offsetLeft;      // appending left offset of each parent
        topPos += parElement.offsetTop;
        parElement = parElement.offsetParent;  // until no more offset parents exist
    }
    return { left: leftPos, top: topPos };
};

/**
* Alias function for getting element by id
* @param {String} sId
* @return {Object} DOM object with sId id
*/
SobekCM_Utilities.gE = function(sId) {
    return document.getElementById(sId);
};